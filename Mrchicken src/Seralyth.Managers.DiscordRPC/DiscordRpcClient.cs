using System;
using System.Diagnostics;
using Seralyth.Managers.DiscordRPC.Events;
using Seralyth.Managers.DiscordRPC.Exceptions;
using Seralyth.Managers.DiscordRPC.IO;
using Seralyth.Managers.DiscordRPC.Logging;
using Seralyth.Managers.DiscordRPC.Message;
using Seralyth.Managers.DiscordRPC.RPC;
using Seralyth.Managers.DiscordRPC.RPC.Commands;
using Seralyth.Managers.DiscordRPC.RPC.Payload;

namespace Seralyth.Managers.DiscordRPC;

public sealed class DiscordRpcClient : IDisposable
{
	private ILogger _logger;

	private readonly RpcConnection connection;

	private bool _shutdownOnly = true;

	private readonly object _sync = new object();

	public bool HasRegisteredUriScheme { get; private set; }

	public string ApplicationID { get; private set; }

	public string SteamID { get; private set; }

	public int ProcessID { get; private set; }

	public int MaxQueueSize { get; private set; }

	public bool IsDisposed { get; private set; }

	public ILogger Logger
	{
		get
		{
			return _logger;
		}
		set
		{
			_logger = value;
			if (connection != null)
			{
				connection.Logger = value;
			}
		}
	}

	public bool AutoEvents { get; private set; }

	public bool SkipIdenticalPresence { get; set; }

	public int TargetPipe { get; private set; }

	public RichPresence CurrentPresence { get; private set; }

	public EventType Subscription { get; private set; }

	public User CurrentUser { get; private set; }

	public Configuration Configuration { get; private set; }

	public bool IsInitialized { get; private set; }

	public bool ShutdownOnly
	{
		get
		{
			return _shutdownOnly;
		}
		set
		{
			_shutdownOnly = value;
			if (connection != null)
			{
				connection.ShutdownOnly = value;
			}
		}
	}

	public event OnReadyEvent OnReady;

	public event OnCloseEvent OnClose;

	public event OnErrorEvent OnError;

	public event OnPresenceUpdateEvent OnPresenceUpdate;

	public event OnSubscribeEvent OnSubscribe;

	public event OnUnsubscribeEvent OnUnsubscribe;

	public event OnJoinEvent OnJoin;

	[Obsolete("Spectating is no longer supported by Discord.")]
	public event OnSpectateEvent OnSpectate;

	public event OnJoinRequestedEvent OnJoinRequested;

	public event OnConnectionEstablishedEvent OnConnectionEstablished;

	public event OnConnectionFailedEvent OnConnectionFailed;

	public event OnRpcMessageEvent OnRpcMessage;

	public DiscordRpcClient(string applicationID)
		: this(applicationID, -1, null, autoEvents: true, null)
	{
	}

	public DiscordRpcClient(string applicationID, int pipe = -1, ILogger logger = null, bool autoEvents = true, INamedPipeClient client = null)
	{
		if (string.IsNullOrEmpty(applicationID))
		{
			throw new ArgumentNullException("applicationID");
		}
		ApplicationID = applicationID.Trim();
		TargetPipe = pipe;
		ProcessID = Process.GetCurrentProcess().Id;
		HasRegisteredUriScheme = false;
		AutoEvents = autoEvents;
		SkipIdenticalPresence = true;
		_logger = logger ?? new NullLogger();
		connection = new RpcConnection(ApplicationID, ProcessID, TargetPipe, client ?? new ManagedNamedPipeClient(), (!autoEvents) ? 128u : 0u)
		{
			ShutdownOnly = _shutdownOnly,
			Logger = _logger
		};
		connection.OnRpcMessage += delegate(object sender, IMessage msg)
		{
			if (this.OnRpcMessage != null)
			{
				this.OnRpcMessage(this, msg);
			}
			if (AutoEvents)
			{
				ProcessMessage(msg);
			}
		};
	}

	public IMessage[] Invoke()
	{
		if (AutoEvents)
		{
			Logger.Error("Cannot Invoke client when AutomaticallyInvokeEvents has been set.");
			return new IMessage[0];
		}
		IMessage[] array = connection.DequeueMessages();
		foreach (IMessage message in array)
		{
			ProcessMessage(message);
		}
		return array;
	}

	private void ProcessMessage(IMessage message)
	{
		if (message == null)
		{
			return;
		}
		switch (message.Type)
		{
		case MessageType.PresenceUpdate:
			lock (_sync)
			{
				if (message is PresenceMessage presenceMessage)
				{
					if (presenceMessage.Presence == null)
					{
						CurrentPresence = null;
					}
					else if (CurrentPresence == null)
					{
						CurrentPresence = new RichPresence().Merge(presenceMessage.Presence);
					}
					else
					{
						CurrentPresence.Merge(presenceMessage.Presence);
					}
					presenceMessage.Presence = CurrentPresence;
				}
			}
			if (this.OnPresenceUpdate != null)
			{
				this.OnPresenceUpdate(this, message as PresenceMessage);
			}
			break;
		case MessageType.Ready:
			if (message is ReadyMessage readyMessage)
			{
				lock (_sync)
				{
					Configuration = readyMessage.Configuration;
					CurrentUser = readyMessage.User;
				}
				SynchronizeState();
			}
			if (this.OnReady != null)
			{
				this.OnReady(this, message as ReadyMessage);
			}
			break;
		case MessageType.Close:
			if (this.OnClose != null)
			{
				this.OnClose(this, message as CloseMessage);
			}
			break;
		case MessageType.Error:
			if (this.OnError != null)
			{
				this.OnError(this, message as ErrorMessage);
			}
			break;
		case MessageType.JoinRequest:
			if (Configuration != null && message is JoinRequestMessage joinRequestMessage)
			{
				joinRequestMessage.User.SetConfiguration(Configuration);
			}
			if (this.OnJoinRequested != null)
			{
				this.OnJoinRequested(this, message as JoinRequestMessage);
			}
			break;
		case MessageType.Subscribe:
			lock (_sync)
			{
				SubscribeMessage subscribeMessage = message as SubscribeMessage;
				Subscription |= subscribeMessage.Event;
			}
			if (this.OnSubscribe != null)
			{
				this.OnSubscribe(this, message as SubscribeMessage);
			}
			break;
		case MessageType.Unsubscribe:
			lock (_sync)
			{
				UnsubscribeMessage unsubscribeMessage = message as UnsubscribeMessage;
				Subscription &= ~unsubscribeMessage.Event;
			}
			if (this.OnUnsubscribe != null)
			{
				this.OnUnsubscribe(this, message as UnsubscribeMessage);
			}
			break;
		case MessageType.Join:
			if (this.OnJoin != null)
			{
				this.OnJoin(this, message as JoinMessage);
			}
			break;
		case MessageType.Spectate:
			if (this.OnSpectate != null)
			{
				this.OnSpectate(this, message as SpectateMessage);
			}
			break;
		case MessageType.ConnectionEstablished:
			if (this.OnConnectionEstablished != null)
			{
				this.OnConnectionEstablished(this, message as ConnectionEstablishedMessage);
			}
			break;
		case MessageType.ConnectionFailed:
			if (this.OnConnectionFailed != null)
			{
				this.OnConnectionFailed(this, message as ConnectionFailedMessage);
			}
			break;
		default:
			Logger.Error("Message was queued with no appropriate handle! {0}", message.Type);
			break;
		}
	}

	public void Respond(JoinRequestMessage request, bool acceptRequest)
	{
		Respond(request.User.ID, acceptRequest);
	}

	public void Respond(User user, bool acceptRequest)
	{
		Respond(user.ID, acceptRequest);
	}

	public void Respond(ulong userID, bool acceptRequest)
	{
		if (IsDisposed)
		{
			throw new ObjectDisposedException("Discord IPC Client");
		}
		if (connection == null)
		{
			throw new ObjectDisposedException("Connection", "Cannot initialize as the connection has been deinitialized");
		}
		if (!IsInitialized)
		{
			throw new UninitializedException();
		}
		connection.EnqueueCommand(new RespondCommand
		{
			Accept = acceptRequest,
			UserID = userID.ToString()
		});
	}

	public void SetPresence(RichPresence presence)
	{
		if (IsDisposed)
		{
			throw new ObjectDisposedException("Discord IPC Client");
		}
		if (connection == null)
		{
			throw new ObjectDisposedException("Connection", "Cannot initialize as the connection has been deinitialized");
		}
		if (!IsInitialized)
		{
			Logger.Warning("The client is not yet initialized, storing the presence as a state instead.");
		}
		if (presence == null)
		{
			if (!SkipIdenticalPresence || CurrentPresence != null)
			{
				connection.EnqueueCommand(new PresenceCommand
				{
					PID = ProcessID,
					Presence = null
				});
			}
		}
		else
		{
			if (presence.HasSecrets() && !HasRegisteredUriScheme)
			{
				throw new BadPresenceException("Cannot send a presence with secrets as this object has not registered a URI scheme. Please enable the uri scheme registration in the DiscordRpcClient constructor.");
			}
			if (presence.HasParty() && presence.Party.Max < presence.Party.Size)
			{
				throw new BadPresenceException("Presence maximum party size cannot be smaller than the current size.");
			}
			if (presence.HasSecrets() && !presence.HasParty())
			{
				Logger.Warning("The presence has set the secrets but no buttons will show as there is no party available.");
			}
			if (!SkipIdenticalPresence || !presence.Matches(CurrentPresence))
			{
				connection.EnqueueCommand(new PresenceCommand
				{
					PID = ProcessID,
					Presence = presence.Clone()
				});
			}
		}
		lock (_sync)
		{
			CurrentPresence = presence?.Clone();
		}
	}

	public RichPresence Update(Action<RichPresence> func)
	{
		if (!IsInitialized)
		{
			throw new UninitializedException();
		}
		RichPresence richPresence;
		lock (_sync)
		{
			richPresence = ((CurrentPresence == null) ? new RichPresence() : CurrentPresence.Clone());
		}
		func(richPresence);
		SetPresence(richPresence);
		return richPresence;
	}

	public RichPresence UpdateType(ActivityType type)
	{
		return Update(delegate(RichPresence p)
		{
			p.Type = type;
		});
	}

	public RichPresence UpdateStatusDisplayType(StatusDisplayType type)
	{
		return Update(delegate(RichPresence p)
		{
			p.StatusDisplay = type;
		});
	}

	public RichPresence UpdateButtons(Button[] buttons = null)
	{
		return Update(delegate(RichPresence p)
		{
			p.Buttons = buttons;
		});
	}

	public RichPresence SetButton(Button button, int index = 0)
	{
		return Update(delegate(RichPresence p)
		{
			p.Buttons[index] = button;
		});
	}

	public RichPresence UpdateDetails(string details)
	{
		return Update(delegate(RichPresence p)
		{
			p.Details = details;
		});
	}

	public RichPresence UpdateState(string state)
	{
		return Update(delegate(RichPresence p)
		{
			p.State = state;
		});
	}

	public RichPresence UpdateParty(Party party)
	{
		return Update(delegate(RichPresence p)
		{
			p.Party = party;
		});
	}

	public RichPresence UpdatePartySize(int size)
	{
		return Update(delegate(RichPresence p)
		{
			if (p.Party == null)
			{
				throw new BadPresenceException("Cannot set the size of the party if the party does not exist");
			}
			p.Party.Size = size;
		});
	}

	public RichPresence UpdatePartySize(int size, int max)
	{
		return Update(delegate(RichPresence p)
		{
			if (p.Party == null)
			{
				throw new BadPresenceException("Cannot set the size of the party if the party does not exist");
			}
			p.Party.Size = size;
			p.Party.Max = max;
		});
	}

	public RichPresence UpdateLargeAsset(string key = null, string tooltip = null)
	{
		return Update(delegate(RichPresence p)
		{
			if (p.Assets == null)
			{
				Assets assets = (p.Assets = new Assets());
			}
			p.Assets.LargeImageKey = key ?? p.Assets.LargeImageKey;
			p.Assets.LargeImageText = tooltip ?? p.Assets.LargeImageText;
		});
	}

	public RichPresence UpdateSmallAsset(string key = null, string tooltip = null)
	{
		return Update(delegate(RichPresence p)
		{
			if (p.Assets == null)
			{
				Assets assets = (p.Assets = new Assets());
			}
			p.Assets.SmallImageKey = key ?? p.Assets.SmallImageKey;
			p.Assets.SmallImageText = tooltip ?? p.Assets.SmallImageText;
		});
	}

	public RichPresence UpdateSecrets(Secrets secrets)
	{
		return Update(delegate(RichPresence p)
		{
			p.Secrets = secrets;
		});
	}

	public RichPresence UpdateStartTime()
	{
		return UpdateStartTime(DateTime.UtcNow);
	}

	public RichPresence UpdateStartTime(DateTime time)
	{
		return Update(delegate(RichPresence p)
		{
			if (p.Timestamps == null)
			{
				Timestamps timestamps = (p.Timestamps = new Timestamps());
			}
			p.Timestamps.Start = time;
		});
	}

	public RichPresence UpdateEndTime()
	{
		return UpdateEndTime(DateTime.UtcNow);
	}

	public RichPresence UpdateEndTime(DateTime time)
	{
		return Update(delegate(RichPresence p)
		{
			if (p.Timestamps == null)
			{
				Timestamps timestamps = (p.Timestamps = new Timestamps());
			}
			p.Timestamps.End = time;
		});
	}

	public RichPresence UpdateClearTime()
	{
		return Update(delegate(RichPresence p)
		{
			p.Timestamps = null;
		});
	}

	public void ClearPresence()
	{
		if (IsDisposed)
		{
			throw new ObjectDisposedException("Discord IPC Client");
		}
		if (!IsInitialized)
		{
			throw new UninitializedException();
		}
		if (connection == null)
		{
			throw new ObjectDisposedException("Connection", "Cannot initialize as the connection has been deinitialized");
		}
		SetPresence(null);
	}

	public void Subscribe(EventType type)
	{
		SetSubscription(Subscription | type);
	}

	public void Unsubscribe(EventType type)
	{
		SetSubscription(Subscription & ~type);
	}

	public void SetSubscription(EventType type)
	{
		if (IsInitialized)
		{
			SubscribeToTypes(Subscription & ~type, isUnsubscribe: true);
			SubscribeToTypes(~Subscription & type, isUnsubscribe: false);
		}
		else
		{
			Logger.Warning("Client has not yet initialized, but events are being subscribed too. Storing them as state instead.");
		}
		lock (_sync)
		{
			Subscription = type;
		}
	}

	private void SubscribeToTypes(EventType type, bool isUnsubscribe)
	{
		if (type != EventType.None)
		{
			if (IsDisposed)
			{
				throw new ObjectDisposedException("Discord IPC Client");
			}
			if (!IsInitialized)
			{
				throw new UninitializedException();
			}
			if (connection == null)
			{
				throw new ObjectDisposedException("Connection", "Cannot initialize as the connection has been deinitialized");
			}
			if (!HasRegisteredUriScheme)
			{
				throw new InvalidConfigurationException("Cannot subscribe/unsubscribe to an event as this application has not registered a URI Scheme. Call RegisterUriScheme().");
			}
			if ((type & EventType.Spectate) == EventType.Spectate)
			{
				connection.EnqueueCommand(new SubscribeCommand
				{
					Event = ServerEvent.ActivitySpectate,
					IsUnsubscribe = isUnsubscribe
				});
			}
			if ((type & EventType.Join) == EventType.Join)
			{
				connection.EnqueueCommand(new SubscribeCommand
				{
					Event = ServerEvent.ActivityJoin,
					IsUnsubscribe = isUnsubscribe
				});
			}
			if ((type & EventType.JoinRequest) == EventType.JoinRequest)
			{
				connection.EnqueueCommand(new SubscribeCommand
				{
					Event = ServerEvent.ActivityJoinRequest,
					IsUnsubscribe = isUnsubscribe
				});
			}
		}
	}

	public void SynchronizeState()
	{
		if (!IsInitialized)
		{
			throw new UninitializedException();
		}
		SetPresence(CurrentPresence);
		if (HasRegisteredUriScheme)
		{
			SubscribeToTypes(Subscription, isUnsubscribe: false);
		}
	}

	public bool Initialize()
	{
		if (!IsDisposed)
		{
			if (!IsInitialized)
			{
				if (connection != null)
				{
					return IsInitialized = connection.AttemptConnection();
				}
				throw new ObjectDisposedException("Connection", "Cannot initialize as the connection has been deinitialized");
			}
			throw new UninitializedException("Cannot initialize a client that is already initialized");
		}
		throw new ObjectDisposedException("Discord IPC Client");
	}

	public void Deinitialize()
	{
		if (!IsInitialized)
		{
			throw new UninitializedException("Cannot deinitialize a client that has not been initalized.");
		}
		connection.Close();
		IsInitialized = false;
	}

	public void Dispose()
	{
		if (!IsDisposed)
		{
			if (IsInitialized)
			{
				Deinitialize();
			}
			IsDisposed = true;
		}
	}
}
