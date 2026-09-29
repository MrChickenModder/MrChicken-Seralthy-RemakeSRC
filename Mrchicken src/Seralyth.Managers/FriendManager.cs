using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using Seralyth.Classes.Menu;
using Seralyth.Menu;
using Seralyth.Mods;
using Seralyth.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Valve.Newtonsoft.Json;
using Valve.Newtonsoft.Json.Linq;

namespace Seralyth.Managers;

public class FriendManager : MonoBehaviour
{
	private struct GameObjectData
	{
		public GameObject AssociatedGameObject;

		public Vector3 TargetPosition;

		public Vector3 OldTargetPosition;

		public Quaternion TargetRotation;

		public Quaternion OldTargetRotation;

		public readonly void InterpolateBetween(float t)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			AssociatedGameObject.transform.position = Vector3.Lerp(OldTargetPosition, TargetPosition, t);
			AssociatedGameObject.transform.rotation = Quaternion.Lerp(OldTargetRotation, TargetRotation, t);
		}
	}

	public class FriendData
	{
		public class Friend
		{
			public bool online { get; }

			public string currentRoom { get; }

			public string currentName { get; }

			public string currentUserID { get; }

			public Friend(bool online, string currentRoom, string currentName, string currentUserID)
			{
				this.online = online;
				this.currentRoom = currentRoom;
				this.currentName = currentName;
				this.currentUserID = currentUserID;
			}
		}

		public class PendingFriend
		{
			public string currentName { get; }

			public string currentUserID { get; }

			public PendingFriend(string currentName, string currentUserID)
			{
				this.currentName = currentName;
				this.currentUserID = currentUserID;
			}
		}

		public Dictionary<string, Friend> friends { get; set; }

		public Dictionary<string, PendingFriend> incoming { get; set; }

		public Dictionary<string, PendingFriend> outgoing { get; set; }
	}

	public class FriendWebSocket : MonoBehaviour
	{
		public readonly string FriendWebsocket = "wss://menu.seralyth.software?mod=" + Seralyth.Classes.Menu.Console.MenuName;

		public ClientWebSocket ws;

		public CancellationTokenSource cts;

		public bool connected;

		public float reconnectTime = 14f;

		public static FriendWebSocket Instance { get; private set; }

		public void Awake()
		{
			Instance = this;
		}

		public void Start()
		{
			cts = new CancellationTokenSource();
		}

		public void Update()
		{
			if (!connected)
			{
				reconnectTime += Time.unscaledDeltaTime;
				if (reconnectTime >= 15f)
				{
					reconnectTime = 0f;
					Connect();
				}
			}
		}

		public async Task Connect()
		{
			if (ws != null && (ws.State == WebSocketState.Open || ws.State == WebSocketState.Connecting))
			{
				return;
			}
			try
			{
				ws = new ClientWebSocket();
				await ws.ConnectAsync(new Uri(FriendWebsocket), cts.Token);
				if (ws.State == WebSocketState.Open)
				{
					connected = true;
					LogManager.Log("Connected to friends websocket");
					Receive();
				}
			}
			catch (Exception ex)
			{
				connected = false;
				LogManager.LogError("Could not connect to friends websocket: " + ex.Message);
			}
		}

		public async Task Receive()
		{
			try
			{
				byte[] buffer = new byte[4096];
				while (ws.State == WebSocketState.Open)
				{
					StringBuilder messageBuilder = new StringBuilder();
					WebSocketReceiveResult result;
					do
					{
						result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
						if (result.MessageType == WebSocketMessageType.Close)
						{
							LogManager.Log("Server closed");
							connected = false;
							await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
							return;
						}
						messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
					}
					while (!result.EndOfMessage);
					string message = messageBuilder.ToString();
					HandleJSON(message);
				}
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				LogManager.LogError("WebSocket error: " + ex2.Message);
				connected = false;
			}
		}

		public async Task Send(string message)
		{
			if (ws != null && ws.State == WebSocketState.Open)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(message);
				await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, endOfMessage: true, cts.Token);
			}
			else
			{
				LogManager.LogError("WebSocket not connected");
			}
		}

		public static void HandleJSON(string json)
		{
			JObject val = JObject.Parse(json);
			string text = (string)val["command"];
			string from = (string)val["from"];
			FriendData.Friend value;
			bool flag = instance.Friends.friends.TryGetValue(from, out value);
			string text2 = (flag ? value.currentName : from);
			if (!flag && from != "Server")
			{
				if (instance.Friends.incoming.TryGetValue(from, out var value2))
				{
					text2 = value2.currentName;
				}
				else
				{
					if (!instance.Friends.outgoing.TryGetValue(from, out var value3))
					{
						return;
					}
					text2 = value3.currentName;
				}
			}
			switch (text)
			{
			case "invite":
			{
				if (!InviteNotifications)
				{
					break;
				}
				string to = (string)val["to"];
				if (NetworkSystem.Instance.InRoom && PhotonNetwork.CurrentRoom.Name == to)
				{
					break;
				}
				if (SoundEffects)
				{
					AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Friends/alert.ogg", "Audio/Friends/alert.ogg", delegate(AudioClip clip)
					{
						Main.Play2DAudio(clip, (float)Main.buttonClickVolume / 10f);
					});
				}
				NotificationManager.SendNotification("<color=grey>[</color><color=green>FRIENDS</color><color=grey>]</color> " + text2 + " has invited you to join them.", 5000);
				Main.Prompt(text2 + " has invited you to the room " + to + ", would you like to join them?", delegate
				{
					((PhotonNetworkController)PhotonNetworkController.Instance).AttemptToJoinSpecificRoom(to, (JoinType)0);
				});
				break;
			}
			case "reqinvite":
				if (!InviteNotifications || !NetworkSystem.Instance.InRoom)
				{
					break;
				}
				if (SoundEffects)
				{
					AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Friends/alert.ogg", "Audio/Friends/alert.ogg", delegate(AudioClip clip)
					{
						Main.Play2DAudio(clip, (float)Main.buttonClickVolume / 10f);
					});
				}
				NotificationManager.SendNotification("<color=grey>[</color><color=green>FRIENDS</color><color=grey>]</color> " + text2 + " has requested an invite from you.", 5000);
				Main.Prompt(text2 + " has requested an invite from you, would you like to invite them?", delegate
				{
					InviteFriend(from);
				});
				break;
			case "preferences":
			{
				if (!PreferenceSharing)
				{
					break;
				}
				if (SoundEffects)
				{
					AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Friends/alert.ogg", "Audio/Friends/alert.ogg", delegate(AudioClip clip)
					{
						Main.Play2DAudio(clip, (float)Main.buttonClickVolume / 10f);
					});
				}
				NotificationManager.SendNotification("<color=grey>[</color><color=green>FRIENDS</color><color=grey>]</color> " + text2 + " has shared their preferences with you.", 5000);
				string preferences = (string)val["data"];
				Main.Prompt(text2 + " has shared their preferences with you, would you like to use them?", delegate
				{
					Settings.SavePreferences();
					Settings.LoadPreferencesFromText(preferences);
				});
				break;
			}
			case "theme":
			{
				if (!ThemeSharing)
				{
					break;
				}
				if (SoundEffects)
				{
					AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Friends/alert.ogg", "Audio/Friends/alert.ogg", delegate(AudioClip clip)
					{
						Main.Play2DAudio(clip, (float)Main.buttonClickVolume / 10f);
					});
				}
				NotificationManager.SendNotification("<color=grey>[</color><color=green>FRIENDS</color><color=grey>]</color> " + text2 + " has shared their theme with you.", 5000);
				string theme = (string)val["data"];
				Main.Prompt(text2 + " has shared their theme with you, would you like to use it?", delegate
				{
					ButtonInfo index = Buttons.GetIndex("Custom Menu Theme");
					if (!index.enabled)
					{
						Main.Toggle(index);
					}
					Settings.ImportCustomTheme(theme);
				});
				break;
			}
			case "macro":
			{
				if (!MacroSharing)
				{
					break;
				}
				if (SoundEffects)
				{
					AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Friends/alert.ogg", "Audio/Friends/alert.ogg", delegate(AudioClip clip)
					{
						Main.Play2DAudio(clip, (float)Main.buttonClickVolume / 10f);
					});
				}
				Movement.Macro macro = Movement.Macro.LoadJSON((string)val["data"]);
				NotificationManager.SendNotification("<color=grey>[</color><color=green>FRIENDS</color><color=grey>]</color> " + text2 + " has shared their macro " + macro.name + " with you.", 5000);
				Main.Prompt(text2 + " has shared their macro " + macro.name + " with you, would you like to use it?", delegate
				{
					Movement.macros[Movement.FormatMacroName(macro.name)] = macro;
				});
				break;
			}
			case "notification":
			{
				string notificationText = (string)val["message"];
				int clearTime = (int)val["time"];
				if (SoundEffects)
				{
					AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Friends/alert.ogg", "Audio/Friends/alert.ogg", delegate(AudioClip clip)
					{
						Main.Play2DAudio(clip, (float)Main.buttonClickVolume / 10f);
					});
				}
				NotificationManager.SendNotification(notificationText, clearTime);
				break;
			}
			case "message":
			{
				if (!Messaging)
				{
					break;
				}
				if (SoundEffects)
				{
					AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Friends/receive.ogg", "Audio/Friends/receive.ogg", delegate(AudioClip clip)
					{
						Main.Play2DAudio(clip, (float)Main.buttonClickVolume / 10f);
					});
				}
				string text3 = (string)val["message"];
				string text4 = (string)val["color"];
				NotificationManager.SendNotification("<color=grey>[</color><color=#" + text4 + ">" + text2.ToUpper() + "</color><color=grey>]</color> " + Regex.Replace(text3, "<\\s*https?://[^\\s>]+\\s*>", "[Media]"), 5000);
				UpdateFriendMessage(from, "<color=grey>[</color><color=#" + text4 + ">" + text2.ToUpper() + "</color><color=grey>]</color> " + text3 + "        ");
				if (Buttons.CurrentCategoryIndex == 41)
				{
					ShowChatMessages(from);
					Main.ReloadMenu();
				}
				break;
			}
			case "friendrequest":
				if (SoundEffects)
				{
					AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Friends/dooropen.ogg", "Audio/Friends/dooropen.ogg", delegate(AudioClip clip)
					{
						Main.Play2DAudio(clip, (float)Main.buttonClickVolume / 10f);
					});
				}
				NotificationManager.SendNotification("<color=grey>[</color><color=green>FRIENDS</color><color=grey>]</color> You have a new friend request from " + text2 + ".", 5000);
				((MonoBehaviour)instance).StartCoroutine(instance.UpdateFriendsList());
				break;
			case "friendrequestaccepted":
				if (SoundEffects)
				{
					AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Friends/online.ogg", "Audio/Friends/online.ogg", delegate(AudioClip clip)
					{
						Main.Play2DAudio(clip, (float)Main.buttonClickVolume / 10f);
					});
				}
				NotificationManager.SendNotification("<color=grey>[</color><color=green>FRIENDS</color><color=grey>]</color> " + text2 + " accepted your friend request.", 5000);
				instance.pendingOutgoingRequests.Remove(from);
				((MonoBehaviour)instance).StartCoroutine(instance.UpdateFriendsList());
				break;
			}
		}
	}

	public static FriendManager instance = null;

	private readonly Dictionary<VRRig, (float, GameObjectData[], GameObject)> rigDatas = new Dictionary<VRRig, (float, GameObjectData[], GameObject)>();

	private readonly Dictionary<VRRig, float> rigUpdateDelays = new Dictionary<VRRig, float>();

	private float UpdateTime;

	private string FriendResponse;

	public const byte FriendByte = 53;

	private const float RigDespawnTime = 0.5f;

	public FriendData Friends = new FriendData
	{
		friends = new Dictionary<string, FriendData.Friend>(),
		incoming = new Dictionary<string, FriendData.PendingFriend>(),
		outgoing = new Dictionary<string, FriendData.PendingFriend>()
	};

	private readonly HashSet<string> pendingOutgoingRequests = new HashSet<string>();

	private static Material starMaterial;

	private static Texture2D starTexture;

	private static float updateRigDelay;

	private static bool pingingState;

	private static GameObject pingObject;

	private static readonly Dictionary<VRRig, GameObject> starPool = new Dictionary<VRRig, GameObject>();

	public static bool RigNetworking = true;

	public static bool PlatformNetworking = true;

	public static bool Pinging = true;

	public static bool InviteNotifications = true;

	public static bool PreferenceSharing = true;

	public static bool ThemeSharing = true;

	public static bool MacroSharing = true;

	public static bool SoundEffects = true;

	public static bool Messaging = true;

	public static bool PhysicalPlatforms;

	private static readonly Dictionary<VRRig, GameObject> leftPlatform = new Dictionary<VRRig, GameObject>();

	private static readonly Dictionary<VRRig, GameObject> rightPlatform = new Dictionary<VRRig, GameObject>();

	private static readonly Dictionary<VRRig, float> pingDelay = new Dictionary<VRRig, float>();

	private static int previousOnlineCount = -1;

	private static int previousIncomingCount = -1;

	public void Awake()
	{
		instance = this;
		UpdateTime = Time.time + 5f;
		((Component)this).gameObject.AddComponent<FriendWebSocket>();
		NetworkSystem obj = NetworkSystem.Instance;
		obj.OnJoinedRoomEvent = (DelegateListProcessorPlusMinus<DelegateListProcessor, Action>)(object)obj.OnJoinedRoomEvent + (Action)CheckAllPlayersFriends;
		NetworkSystem obj2 = NetworkSystem.Instance;
		obj2.OnPlayerJoined = (DelegateListProcessorPlusMinus<DelegateListProcessor<NetPlayer>, Action<NetPlayer>>)(object)obj2.OnPlayerJoined + (Action<NetPlayer>)CheckPlayerFriends;
		NetworkSystem obj3 = NetworkSystem.Instance;
		obj3.OnPlayerLeft = (DelegateListProcessorPlusMinus<DelegateListProcessor<NetPlayer>, Action<NetPlayer>>)(object)obj3.OnPlayerLeft + (Action<NetPlayer>)OnPlayerLeft;
		PhotonNetwork.NetworkingClient.EventReceived += EventReceived;
	}

	public void Update()
	{
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Expected O, but got Unknown
		//IL_083e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_0693: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_064c: Expected O, but got Unknown
		//IL_073b: Unknown result type (might be due to invalid IL or missing references)
		//IL_072a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0740: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_074e: Unknown result type (might be due to invalid IL or missing references)
		//IL_075f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0761: Unknown result type (might be due to invalid IL or missing references)
		//IL_0763: Unknown result type (might be due to invalid IL or missing references)
		//IL_076a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0787: Unknown result type (might be due to invalid IL or missing references)
		//IL_078c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0791: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e4: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > UpdateTime)
		{
			UpdateTime = Time.time + 30f;
			((MonoBehaviour)instance).StartCoroutine(UpdateFriendsList());
		}
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, GameObject> item6 in starPool.Where((KeyValuePair<VRRig, GameObject> star) => !VRRigCache.ActiveRigs.Contains(star.Key) || !IsPlayerFriend(RigUtilities.GetPlayerFromVRRig(star.Key))))
		{
			list.Add(item6.Key);
			Object.Destroy((Object)(object)item6.Value);
		}
		foreach (VRRig item7 in list)
		{
			starPool.Remove(item7);
		}
		Dictionary<VRRig, (float, GameObjectData[], GameObject)> dictionary = new Dictionary<VRRig, (float, GameObjectData[], GameObject)>();
		foreach (KeyValuePair<VRRig, (float, GameObjectData[], GameObject)> rigData in rigDatas)
		{
			rigData.Deconstruct(out var key, out var value);
			(float, GameObjectData[], GameObject) tuple = value;
			VRRig key2 = key;
			float item = tuple.Item1;
			GameObjectData[] item2 = tuple.Item2;
			GameObject item3 = tuple.Item3;
			float num = Time.time - item;
			if (num > 0.5f)
			{
				dictionary.Add(key2, rigDatas[key2]);
				continue;
			}
			float valueOrDefault = rigUpdateDelays.GetValueOrDefault(key2, 0.1f);
			float t = num / valueOrDefault;
			GameObjectData[] array = item2;
			foreach (GameObjectData gameObjectData in array)
			{
				gameObjectData.InterpolateBetween(t);
			}
			item3.transform.LookAt(((Component)Camera.main).transform.position);
			item3.transform.Rotate(0f, 180f, 0f);
		}
		foreach (KeyValuePair<VRRig, (float, GameObjectData[], GameObject)> item8 in dictionary)
		{
			rigDatas.Remove(item8.Key);
			GameObjectData[] item4 = item8.Value.Item2;
			for (int num3 = 0; num3 < item4.Length; num3++)
			{
				GameObjectData gameObjectData2 = item4[num3];
				Object.Destroy((Object)(object)gameObjectData2.AssociatedGameObject);
			}
			Object.Destroy((Object)(object)item8.Value.Item3);
		}
		if (NetworkSystem.Instance.InRoom)
		{
			NetPlayer[] allFriendsInRoom = GetAllFriendsInRoom();
			NetPlayer[] array2 = allFriendsInRoom;
			foreach (NetPlayer p in array2)
			{
				VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(p);
				if ((Object)(object)vRRigFromPlayer == (Object)null)
				{
					continue;
				}
				if (!starPool.TryGetValue(vRRigFromPlayer, out var value2))
				{
					value2 = GameObject.CreatePrimitive((PrimitiveType)3);
					Object.Destroy((Object)(object)value2.GetComponent<Collider>());
					if ((Object)(object)starMaterial == (Object)null)
					{
						if ((Object)(object)starTexture == (Object)null)
						{
							starTexture = AssetUtilities.LoadTextureFromResource("SeralythMenu.Resources.Client.star.png");
						}
						starMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
						{
							mainTexture = (Texture)(object)starTexture
						};
						starMaterial.SetFloat("_Surface", 1f);
						starMaterial.SetFloat("_Blend", 0f);
						starMaterial.SetFloat("_SrcBlend", 5f);
						starMaterial.SetFloat("_DstBlend", 10f);
						starMaterial.SetFloat("_ZWrite", 0f);
						starMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
						starMaterial.renderQueue = 3000;
					}
					value2.GetComponent<Renderer>().material = starMaterial;
					starPool.Add(vRRigFromPlayer, value2);
				}
				value2.GetComponent<Renderer>().material.color = vRRigFromPlayer.playerColor;
				value2.transform.localScale = new Vector3(0.4f, 0.4f, 0.01f) * vRRigFromPlayer.scaleFactor;
				value2.transform.position = Visuals.GetNameTagTransform(vRRigFromPlayer).position + Visuals.GetNameTagTransform(vRRigFromPlayer).up * (Seralyth.Classes.Menu.Console.GetIndicatorDistance(vRRigFromPlayer) * vRRigFromPlayer.scaleFactor);
				value2.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
			}
			int[] allNetworkActorNumbers = GetAllNetworkActorNumbers();
			if (allNetworkActorNumbers.Length != 0 && RigNetworking && !((Behaviour)VRRig.LocalRig).enabled && Time.time > updateRigDelay)
			{
				updateRigDelay = Time.time + 0.15f;
				ExecuteCommand("rig", allNetworkActorNumbers, new object[2]
				{
					((Component)GorillaTagger.Instance.headCollider).transform.position,
					((Component)GorillaTagger.Instance.headCollider).transform.rotation
				}, new object[2]
				{
					((Component)GorillaTagger.Instance.leftHandTransform).transform.position,
					((Component)GorillaTagger.Instance.leftHandTransform).transform.rotation
				}, new object[2]
				{
					((Component)GorillaTagger.Instance.rightHandTransform).transform.position,
					((Component)GorillaTagger.Instance.rightHandTransform).transform.rotation
				});
			}
			if (allNetworkActorNumbers.Length != 0 && Pinging)
			{
				if ((Object)(object)Main.menu != (Object)null)
				{
					if (Main.rightJoystickClick && !Main.joystickMenu)
					{
						if ((Object)(object)pingObject == (Object)null)
						{
							pingObject = new GameObject("Seralyth_PingLine");
						}
						Color playerColor = VRRig.LocalRig.playerColor;
						playerColor.a = 0.15f;
						LineRenderer orAddComponent = GTExt.GetOrAddComponent<LineRenderer>(pingObject);
						((Renderer)orAddComponent).material.shader = Shader.Find("GUI/Text Shader");
						orAddComponent.startColor = playerColor;
						orAddComponent.endColor = playerColor;
						orAddComponent.startWidth = 0.025f * (Main.scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
						orAddComponent.endWidth = 0.025f * (Main.scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
						orAddComponent.positionCount = 2;
						orAddComponent.useWorldSpace = true;
						if (Main.smoothLines)
						{
							orAddComponent.numCapVertices = 10;
							orAddComponent.numCornerVertices = 5;
						}
						Vector3 val = (Main.SwapGunHand ? GorillaTagger.Instance.leftHandTransform.position : GorillaTagger.Instance.rightHandTransform.position);
						Vector3 val2 = (Main.SwapGunHand ? ControllerUtilities.GetTrueLeftHand().forward : ControllerUtilities.GetTrueRightHand().forward);
						RaycastHit val3 = default(RaycastHit);
						Physics.Raycast(val + val2 / 4f * (Main.scaleWithPlayer ? GTPlayer.Instance.scale : 1f), val2, ref val3, 512f, Main.NoInvisLayerMask());
						Vector3 val4 = ((((RaycastHit)(ref val3)).point == Vector3.zero) ? (val + val2 * 512f) : ((RaycastHit)(ref val3)).point);
						orAddComponent.SetPosition(0, val);
						orAddComponent.SetPosition(1, val4);
					}
					else if (pingingState)
					{
						List<int> list2 = allNetworkActorNumbers.ToList();
						list2.Add(NetworkSystem.Instance.LocalPlayer.ActorNumber);
						ExecuteCommand("ping", list2.ToArray(), pingObject.GetComponent<LineRenderer>().GetPosition(1));
						if ((Object)(object)pingObject != (Object)null)
						{
							Object.Destroy((Object)(object)pingObject);
							pingObject = null;
						}
					}
					pingingState = Main.rightJoystickClick;
				}
				else
				{
					pingingState = false;
					if ((Object)(object)pingObject != (Object)null)
					{
						Object.Destroy((Object)(object)pingObject);
						pingObject = null;
					}
				}
			}
			Dictionary<VRRig, GameObject>[] array3 = new Dictionary<VRRig, GameObject>[2] { leftPlatform, rightPlatform };
			foreach (Dictionary<VRRig, GameObject> dictionary2 in array3)
			{
				List<VRRig> list3 = new List<VRRig>();
				foreach (KeyValuePair<VRRig, GameObject> item9 in dictionary2.Where((KeyValuePair<VRRig, GameObject> Platform) => !VRRigCache.ActiveRigs.Contains(Platform.Key)))
				{
					list3.Add(item9.Key);
					Object.Destroy((Object)(object)item9.Value);
				}
				foreach (VRRig item10 in list3)
				{
					dictionary2.Remove(item10);
				}
			}
			return;
		}
		foreach (KeyValuePair<VRRig, GameObject> item11 in starPool)
		{
			Object.Destroy((Object)(object)item11.Value);
		}
		starPool.Clear();
		foreach (KeyValuePair<VRRig, (float, GameObjectData[], GameObject)> rigData2 in rigDatas)
		{
			GameObjectData[] item5 = rigData2.Value.Item2;
			for (int num6 = 0; num6 < item5.Length; num6++)
			{
				GameObjectData gameObjectData3 = item5[num6];
				Object.Destroy((Object)(object)gameObjectData3.AssociatedGameObject);
			}
			Object.Destroy((Object)(object)rigData2.Value.Item3);
		}
		rigDatas.Clear();
		Dictionary<VRRig, GameObject>[] array4 = new Dictionary<VRRig, GameObject>[2] { leftPlatform, rightPlatform };
		foreach (Dictionary<VRRig, GameObject> dictionary3 in array4)
		{
			List<VRRig> list4 = new List<VRRig>();
			foreach (KeyValuePair<VRRig, GameObject> item12 in dictionary3)
			{
				list4.Add(item12.Key);
				Object.Destroy((Object)(object)item12.Value);
			}
			foreach (VRRig item13 in list4)
			{
				dictionary3.Remove(item13);
			}
		}
	}

	public static void CheckAllPlayersFriends()
	{
		NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
		foreach (NetPlayer player in playerListOthers)
		{
			CheckPlayerFriends(player);
		}
	}

	public static void CheckPlayerFriends(NetPlayer Player)
	{
		if (IsPlayerFriend(Player))
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=green>FRIENDS</color><color=grey>]</color> Your friend " + Player.NickName + " is in your current room.", 5000);
		}
	}

	public static void OnPlayerLeft(NetPlayer Player)
	{
		VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(Player);
		if (!((Object)(object)vRRigFromPlayer != (Object)null))
		{
			return;
		}
		if (starPool.TryGetValue(vRRigFromPlayer, out var value))
		{
			Object.Destroy((Object)(object)value);
			starPool.Remove(vRRigFromPlayer);
		}
		if ((Object)(object)instance != (Object)null && instance.rigDatas.TryGetValue(vRRigFromPlayer, out var value2))
		{
			GameObjectData[] item = value2.Item2;
			for (int i = 0; i < item.Length; i++)
			{
				GameObjectData gameObjectData = item[i];
				Object.Destroy((Object)(object)gameObjectData.AssociatedGameObject);
			}
			Object.Destroy((Object)(object)value2.Item3);
			instance.rigDatas.Remove(vRRigFromPlayer);
		}
		if (leftPlatform.TryGetValue(vRRigFromPlayer, out var value3))
		{
			Object.Destroy((Object)(object)value3);
			leftPlatform.Remove(vRRigFromPlayer);
		}
		if (rightPlatform.TryGetValue(vRRigFromPlayer, out var value4))
		{
			Object.Destroy((Object)(object)value4);
			rightPlatform.Remove(vRRigFromPlayer);
		}
	}

	public static NetPlayer[] GetAllFriendsInRoom()
	{
		return (!NetworkSystem.Instance.InRoom || instance?.Friends?.friends == null) ? Array.Empty<NetPlayer>() : NetworkSystem.Instance.PlayerListOthers.Where((NetPlayer player) => player != null && player.UserId != null && instance.Friends.friends.Values.Any((FriendData.Friend friend) => friend != null && player.UserId == friend.currentUserID)).ToArray();
	}

	public static int[] GetAllNetworkActorNumbers()
	{
		List<int> list = new List<int>();
		if (!NetworkSystem.Instance.InRoom || (Object)(object)instance == (Object)null)
		{
			return list.ToArray();
		}
		list.AddRange(from Player in GetAllFriendsInRoom()
			select Player.ActorNumber);
		list.AddRange(from Player in NetworkSystem.Instance.PlayerListOthers
			where ServerData.Administrators.ContainsKey(Player.UserId)
			select Player.ActorNumber);
		return list.ToArray();
	}

	public static bool IsPlayerFriend(NetPlayer Player)
	{
		return instance.Friends.friends.Values.Any((FriendData.Friend friend) => friend.currentUserID == Player.UserId);
	}

	public static void PlatformSpawned(bool leftHand, Vector3 position, Quaternion rotation, Vector3 scale, PrimitiveType spawnType)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected I4, but got Unknown
		if (PlatformNetworking && NetworkSystem.Instance.InRoom && GetAllNetworkActorNumbers().Length != 0)
		{
			ExecuteCommand("platformSpawn", GetAllNetworkActorNumbers(), leftHand, position, rotation, scale, (int)spawnType);
		}
	}

	public static void PlatformDespawned(bool leftHand)
	{
		if (PlatformNetworking && NetworkSystem.Instance.InRoom && GetAllNetworkActorNumbers().Length != 0)
		{
			ExecuteCommand("platformDespawn", GetAllNetworkActorNumbers(), leftHand);
		}
	}

	public static void EventReceived(EventData data)
	{
		//IL_0a8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a92: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a97: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a64: Unknown result type (might be due to invalid IL or missing references)
		//IL_0763: Unknown result type (might be due to invalid IL or missing references)
		//IL_0768: Unknown result type (might be due to invalid IL or missing references)
		//IL_076f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0776: Expected O, but got Unknown
		//IL_0782: Unknown result type (might be due to invalid IL or missing references)
		//IL_0790: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0814: Unknown result type (might be due to invalid IL or missing references)
		//IL_081e: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Expected O, but got Unknown
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_0553: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_061f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0624: Unknown result type (might be due to invalid IL or missing references)
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Unknown result type (might be due to invalid IL or missing references)
		//IL_0669: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0679: Unknown result type (might be due to invalid IL or missing references)
		//IL_067e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_068e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0699: Unknown result type (might be due to invalid IL or missing references)
		//IL_069e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_094a: Unknown result type (might be due to invalid IL or missing references)
		//IL_095a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0969: Unknown result type (might be due to invalid IL or missing references)
		//IL_0978: Unknown result type (might be due to invalid IL or missing references)
		//IL_098d: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		try
		{
			NetPlayer val = NetPlayer.op_Implicit(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false));
			if (data.Code != 53 || (!IsPlayerFriend(val) && !ServerData.Administrators.ContainsKey(val.UserId) && !ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId)))
			{
				return;
			}
			VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(val);
			object[] array = ((data.CustomData == null) ? new object[0] : ((object[])data.CustomData));
			switch ((array.Length != 0) ? ((string)array[0]) : "")
			{
			case "rig":
				if (RigNetworking)
				{
					object[] array3 = (object[])array[1];
					object[] array4 = (object[])array[2];
					object[] array5 = (object[])array[3];
					if (instance.rigDatas.TryGetValue(vRRigFromPlayer, out var value3))
					{
						instance.rigUpdateDelays[vRRigFromPlayer] = Time.time - value3.Item1;
						value3.Item1 = Time.time;
						value3.Item2[0].OldTargetPosition = value3.Item2[0].TargetPosition;
						value3.Item2[0].OldTargetRotation = value3.Item2[0].TargetRotation;
						value3.Item2[0].TargetPosition = (Vector3)array3[0];
						value3.Item2[0].TargetRotation = (Quaternion)array3[1];
						value3.Item2[1].OldTargetPosition = value3.Item2[1].TargetPosition;
						value3.Item2[1].OldTargetRotation = value3.Item2[1].TargetRotation;
						value3.Item2[1].TargetPosition = (Vector3)array4[0];
						value3.Item2[1].TargetRotation = (Quaternion)array4[1];
						value3.Item2[2].OldTargetPosition = value3.Item2[2].TargetPosition;
						value3.Item2[2].OldTargetRotation = value3.Item2[2].TargetRotation;
						value3.Item2[2].TargetPosition = (Vector3)array5[0];
						value3.Item2[2].TargetRotation = (Quaternion)array5[1];
						instance.rigDatas[vRRigFromPlayer] = value3;
						break;
					}
					GameObject val6 = GameObject.CreatePrimitive((PrimitiveType)3);
					Object.Destroy((Object)(object)val6.GetComponent<Collider>());
					val6.transform.localScale = Vector3.one * 0.3f;
					val6.GetComponent<Renderer>().material.color = vRRigFromPlayer.playerColor;
					GameObject val7 = new GameObject("Seralyth_Nametag");
					val7.transform.SetParent(val6.transform);
					val7.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
					val7.transform.localPosition = new Vector3(0f, 0.8f, 0f);
					TextMeshPro val8 = val7.AddComponent<TextMeshPro>();
					((TMP_Text)val8).fontSize = 24f;
					((TMP_Text)val8).font = Main.activeFont;
					((TMP_Text)val8).fontStyle = Main.activeFontStyle;
					((TMP_Text)val8).alignment = (TextAlignmentOptions)514;
					((TMP_Text)val8).text = val.SanitizedNickName;
					((Graphic)val8).color = vRRigFromPlayer.playerColor;
					((TMP_Text)val8).fontStyle = Main.activeFontStyle;
					GameObject val9 = GameObject.CreatePrimitive((PrimitiveType)3);
					Object.Destroy((Object)(object)val9.GetComponent<Collider>());
					val9.transform.localScale = Vector3.one * 0.1f;
					val9.GetComponent<Renderer>().material.color = vRRigFromPlayer.playerColor;
					GameObject val10 = GameObject.CreatePrimitive((PrimitiveType)3);
					Object.Destroy((Object)(object)val10.GetComponent<Collider>());
					val10.transform.localScale = Vector3.one * 0.1f;
					val10.GetComponent<Renderer>().material.color = vRRigFromPlayer.playerColor;
					GameObjectData[] item = new GameObjectData[3]
					{
						new GameObjectData
						{
							AssociatedGameObject = val6,
							TargetPosition = (Vector3)array3[0],
							OldTargetPosition = (Vector3)array3[0],
							TargetRotation = (Quaternion)array3[1],
							OldTargetRotation = (Quaternion)array3[1]
						},
						new GameObjectData
						{
							AssociatedGameObject = val9,
							TargetPosition = (Vector3)array4[0],
							OldTargetPosition = (Vector3)array4[0],
							TargetRotation = (Quaternion)array4[1],
							OldTargetRotation = (Quaternion)array4[1]
						},
						new GameObjectData
						{
							AssociatedGameObject = val10,
							TargetPosition = (Vector3)array5[0],
							OldTargetPosition = (Vector3)array5[0],
							TargetRotation = (Quaternion)array5[1],
							OldTargetRotation = (Quaternion)array5[1]
						}
					};
					instance.rigDatas[vRRigFromPlayer] = (Time.time, item, val7);
				}
				break;
			case "ping":
				if (Pinging)
				{
					pingDelay.TryGetValue(vRRigFromPlayer, out var value2);
					if (!(Time.time < value2))
					{
						Vector3 val3 = (Vector3)array[1];
						GameObject val4 = new GameObject("Line");
						LineRenderer val5 = val4.AddComponent<LineRenderer>();
						val5.startColor = vRRigFromPlayer.playerColor;
						val5.endColor = vRRigFromPlayer.playerColor;
						val5.startWidth = 0.25f;
						val5.endWidth = 0.25f;
						val5.positionCount = 2;
						val5.useWorldSpace = true;
						val5.SetPosition(0, val3);
						val5.SetPosition(1, val3 + Vector3.up * 99999f);
						((Renderer)val5).material.shader = Shader.Find("GUI/Text Shader");
						Main.PlayPositionAudio(GTPlayer.Instance.materialData[29].audio, val3);
						((MonoBehaviour)instance).StartCoroutine(FadePing(val4));
						pingDelay[vRRigFromPlayer] = Time.time + 0.1f;
					}
				}
				break;
			case "platformSpawn":
			{
				if (!PlatformNetworking || (Experimental.platExcluded.Contains(val.UserId) && ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId)))
				{
					break;
				}
				bool flag = (bool)array[1];
				Vector3 position = (Vector3)array[2];
				Quaternion rotation = (Quaternion)array[3];
				Vector3 localScale = GTExt.ClampMagnitudeSafe((Vector3)array[4], 1f);
				PrimitiveType val2 = (PrimitiveType)(int)array[5];
				float num = 10000f;
				if (!GTExt.IsValid(ref position, ref num))
				{
					break;
				}
				float num2 = 10000f;
				if (GTExt.IsValid(ref localScale, ref num2))
				{
					Dictionary<VRRig, GameObject> dictionary = (flag ? leftPlatform : rightPlatform);
					if (dictionary.TryGetValue(vRRigFromPlayer, out var value))
					{
						Object.Destroy((Object)(object)value);
						dictionary.Remove(vRRigFromPlayer);
					}
					value = GameObject.CreatePrimitive(val2);
					value.transform.position = position;
					value.transform.rotation = rotation;
					value.transform.localScale = localScale;
					value.GetComponent<Renderer>().material.color = vRRigFromPlayer.playerColor;
					if (!PhysicalPlatforms)
					{
						Object.Destroy((Object)(object)value.GetComponent<Collider>());
					}
					dictionary.Add(vRRigFromPlayer, value);
				}
				break;
			}
			case "platformDespawn":
			{
				Dictionary<VRRig, GameObject> dictionary2 = (((bool)array[1]) ? leftPlatform : rightPlatform);
				if (dictionary2.TryGetValue(vRRigFromPlayer, out var value4))
				{
					Object.Destroy((Object)(object)value4);
					dictionary2.Remove(vRRigFromPlayer);
				}
				break;
			}
			case "sendProjectile":
			{
				object[] array7 = (object[])array[1];
				Projectiles.LaunchLocalProjectile((Vector3)array7[0], (Vector3)array7[1], Convert.ToInt32(array7[2]), Convert.ToInt32(array7[3]), Convert.ToBoolean(array7[4]), new Color32(Convert.ToByte(array7[5]), Convert.ToByte(array7[6]), Convert.ToByte(array7[7]), Convert.ToByte(array7[8])), Convert.ToInt32(array7[9]), Convert.ToInt32(array7[10]), vRRigFromPlayer);
				break;
			}
			case "sendSnowball":
			{
				Vector3 val11 = (Vector3)array[1];
				Vector3 val12 = (Vector3)array[2];
				float num3 = (float)array[3];
				float num4 = (float)array[4];
				float num5 = (float)array[5];
				float num6 = Mathf.Clamp((float)array[6], 1f, 10f);
				int num7 = (int)array[7];
				SnowballThrowable projectile = Main.GetProjectile(Projectiles.SnowballName + "LeftAnchor");
				GrowingSnowballThrowable val13 = (GrowingSnowballThrowable)(object)((projectile is GrowingSnowballThrowable) ? projectile : null);
				SlingshotProjectile val14 = val13.SpawnGrowingSnowball(ref val12, num6);
				val14.Launch(val11, val12, val, false, false, num7, num6, true, new Color(num3, num4, num5, 1f));
				break;
			}
			case "quest":
			{
				object[] array6 = new object[array.Length - 1];
				Array.Copy(array, 1, array6, 0, array6.Length);
				Quests.ReceiveQuestSync(array6);
				break;
			}
			case "sharequest":
			{
				string text3 = (string)array[1];
				if (!(text3 != PhotonNetwork.LocalPlayer.UserId))
				{
					string senderName = (string)array[2];
					string senderId = val.UserId;
					string text4 = (string)array[3];
					string text5 = (string)array[4];
					string text6 = (string)array[5];
					string text7 = (string)array[6];
					string text8 = (string)array[7];
					object[] array2 = (object[])array[8];
					object[] fullData = new object[6] { text4, text5, text6, text7, text8, array2 };
					Main.Prompt(senderName + " wants to share a quest with you!\n\n<color=yellow>" + text4 + "</color>\n" + text5, delegate
					{
						Quests.AcceptShareQuest(senderName, senderId, fullData);
					});
				}
				break;
			}
			case "stopsharequest":
			{
				string text2 = (string)array[1];
				if (!(text2 != PhotonNetwork.LocalPlayer.UserId))
				{
					Quests.HandleStopShareQuest();
				}
				break;
			}
			case "questcompleted":
			{
				string text = (string)array[1];
				if (!(text != PhotonNetwork.LocalPlayer.UserId))
				{
					string friendName = (string)array[2];
					Quests.HandleQuestCompletedByFriend(friendName);
				}
				break;
			}
			}
		}
		catch (Exception ex)
		{
			LogManager.LogError("Error processing friend event: " + ex);
		}
	}

	public static void ExecuteCommand(string command, RaiseEventOptions options, params object[] parameters)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (NetworkSystem.Instance.InRoom)
		{
			PhotonNetwork.RaiseEvent((byte)53, (object)new object[1] { command }.Concat(parameters).ToArray(), options, SendOptions.SendReliable);
		}
	}

	public static void ExecuteCommand(string command, int[] targets, params object[] parameters)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		FriendManager.ExecuteCommand(command, new RaiseEventOptions
		{
			TargetActors = targets
		}, parameters);
	}

	public static void ExecuteCommand(string command, int target, params object[] parameters)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		RaiseEventOptions val = new RaiseEventOptions();
		val.TargetActors = new int[1] { target };
		ExecuteCommand(command, val, parameters);
	}

	public static void ExecuteCommand(string command, ReceiverGroup target, params object[] parameters)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		FriendManager.ExecuteCommand(command, new RaiseEventOptions
		{
			Receivers = target
		}, parameters);
	}

	public static IEnumerator FadePing(GameObject line)
	{
		float startTime = Time.time;
		LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
		Color startColor = lineRenderer.startColor;
		Color endColor = startColor;
		endColor.a = 0f;
		float startWidth = lineRenderer.startWidth;
		float endWidth = startWidth + 0.1f;
		while (Time.time < startTime + 3f)
		{
			float time = (Time.time - startTime) / 3f;
			Color targetColor = (lineRenderer.startColor = Color.Lerp(startColor, endColor, time));
			lineRenderer.endColor = targetColor;
			float targetWidth = (lineRenderer.startWidth = Mathf.Lerp(startWidth, endWidth, time));
			lineRenderer.endWidth = targetWidth;
			yield return null;
		}
		Object.Destroy((Object)(object)line);
	}

	public IEnumerator UpdateFriendsList()
	{
		UnityWebRequest request = new UnityWebRequest("https://menu.seralyth.software/getfriends", "GET");
		try
		{
			byte[] bodyRaw = Encoding.UTF8.GetBytes("{}");
			request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
			request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if ((int)request.result == 1)
			{
				FriendResponse = request.downloadHandler.text;
			}
			else
			{
				LogManager.Log("Friend data could not be loaded");
			}
			Friends = JsonConvert.DeserializeObject<FriendData>(FriendResponse);
			foreach (string uid in instance.Friends.outgoing.Keys)
			{
				instance.pendingOutgoingRequests.Remove(uid);
			}
			FriendsListUpdated();
		}
		finally
		{
			((IDisposable)request)?.Dispose();
		}
	}

	public static void SendFriendRequest(string uid)
	{
		if (instance.pendingOutgoingRequests.Contains(uid))
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=yellow>WARNING</color><color=grey>]</color> You already sent a friend request to this user.", 5000);
			return;
		}
		instance.pendingOutgoingRequests.Add(uid);
		((MonoBehaviour)instance).StartCoroutine(ExecuteAction(uid, "frienduser", delegate
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully sent friend request.", 5000);
			FriendWebSocket.Instance.Send(JsonConvert.SerializeObject((object)new
			{
				command = "friendrequest",
				target = uid
			}));
		}, delegate(string error)
		{
			instance.pendingOutgoingRequests.Remove(uid);
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Could not send friend request: " + error, 5000);
		}));
	}

	public static void AcceptFriendRequest(string uid)
	{
		((MonoBehaviour)instance).StartCoroutine(ExecuteAction(uid, "frienduser", delegate
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully accepted friend request.", 5000);
			FriendWebSocket.Instance.Send(JsonConvert.SerializeObject((object)new
			{
				command = "friendrequestaccepted",
				target = uid
			}));
		}, delegate(string error)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Could not accept friend request: " + error, 5000);
		}));
	}

	public static void RemoveFriend(string uid)
	{
		((MonoBehaviour)instance).StartCoroutine(ExecuteAction(uid, "unfrienduser", delegate
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Removed friend from friends list.", 5000);
		}, delegate(string error)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Could not remove friend from friends list: " + error, 5000);
		}));
	}

	public static void DenyFriendRequest(string uid)
	{
		((MonoBehaviour)instance).StartCoroutine(ExecuteAction(uid, "unfrienduser", delegate
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Denied friend request.", 5000);
			if (SoundEffects)
			{
				AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Friends/doorslam.ogg", "Audio/Friends/doorslam.ogg", delegate(AudioClip clip)
				{
					Main.Play2DAudio(clip, (float)Main.buttonClickVolume / 10f);
				});
			}
		}, delegate(string error)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Could not deny friend request: " + error, 5000);
		}));
	}

	public static void CancelFriendRequest(string uid)
	{
		((MonoBehaviour)instance).StartCoroutine(ExecuteAction(uid, "unfrienduser", delegate
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Cancelled friend request.", 5000);
		}, delegate(string error)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Could not cancel friend request: " + error, 5000);
		}));
	}

	public static void InviteFriend(string uid)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not in a room.", 5000);
			return;
		}
		FriendWebSocket.Instance.Send(JsonConvert.SerializeObject((object)new
		{
			command = "invite",
			target = uid,
			room = PhotonNetwork.CurrentRoom.Name
		}));
		NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully invited friend to room.", 5000);
	}

	public static void RequestInviteFriend(string uid)
	{
		FriendWebSocket.Instance.Send(JsonConvert.SerializeObject((object)new
		{
			command = "reqinvite",
			target = uid
		}));
		NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully requested invite from friend.", 5000);
	}

	public static void SharePreferences(string uid)
	{
		FriendWebSocket.Instance.Send(JsonConvert.SerializeObject((object)new
		{
			command = "preferences",
			target = uid,
			preferences = Settings.SavePreferencesToText()
		}));
		NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully shared preferences.", 5000);
	}

	public static void ShareTheme(string uid)
	{
		FriendWebSocket.Instance.Send(JsonConvert.SerializeObject((object)new
		{
			command = "theme",
			target = uid,
			theme = Settings.ExportCustomTheme()
		}));
		NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully shared theme.", 5000);
	}

	public static void ShareMacro(string uid, string name)
	{
		Movement.Macro macro = null;
		foreach (Movement.Macro item in from macroData in Movement.macros
			select macroData.Value into macroItem
			where string.Equals(macroItem.name.ToLower(), name.ToLower())
			select macroItem)
		{
			macro = item;
		}
		if (macro == null)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Macro \"" + name + "\" does not exist.", 5000);
			return;
		}
		FriendWebSocket.Instance.Send(JsonConvert.SerializeObject((object)new
		{
			command = "macro",
			target = uid,
			macro = macro.DumpJSON()
		}));
		NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully shared macro.", 5000);
	}

	public static void SendFriendMessage(string uid, string message)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		FriendWebSocket.Instance.Send(JsonConvert.SerializeObject((object)new
		{
			command = "message",
			target = uid,
			color = Main.ColorToHex(VRRig.LocalRig.playerColor),
			message = message
		}));
		if (SoundEffects)
		{
			AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Friends/send.ogg", "Audio/Friends/send.ogg", delegate(AudioClip clip)
			{
				Main.Play2DAudio(clip, (float)Main.buttonClickVolume / 10f);
			});
		}
		NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully sent message.", 5000);
	}

	public static void UpdateFriendMessage(string friendTarget, string message)
	{
		string path = "SeralythMenu/Friends/Messages/" + friendTarget + ".json";
		if (!File.Exists(path))
		{
			File.WriteAllText(path, "{\"messages\":[]}");
			return;
		}
		JObject val = JObject.Parse(File.ReadAllText(path));
		JToken obj = val["messages"];
		List<string> list = ((obj != null) ? obj.ToObject<List<string>>() : null) ?? new List<string>();
		list.Add(message);
		val["messages"] = (JToken)(object)JArray.FromObject((object)list);
		File.WriteAllText(path, ((object)val).ToString());
	}

	public static IEnumerator ExecuteAction(string uid, string action, Action success, Action<string> failure)
	{
		UnityWebRequest request = new UnityWebRequest("https://menu.seralyth.software/" + action, "POST");
		string json = JsonConvert.SerializeObject((object)new { uid });
		byte[] raw = Encoding.UTF8.GetBytes(json);
		request.uploadHandler = (UploadHandler)new UploadHandlerRaw(raw);
		request.SetRequestHeader("Content-Type", "application/json");
		request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
		yield return request.SendWebRequest();
		if ((int)request.result == 1)
		{
			success();
			instance.UpdateTime = 0f;
			yield break;
		}
		string reason = (StringUtils.IsNullOrEmpty(request.error) ? "Unknown error" : request.error);
		try
		{
			string responseText = request.downloadHandler.text;
			object value = default(object);
			if (JsonConvert.DeserializeObject<Dictionary<string, object>>(responseText)?.TryGetValue("error", out value) ?? false)
			{
				reason = value.ToString();
			}
		}
		catch
		{
		}
		failure(reason);
	}

	public static void FriendsListUpdated()
	{
		FriendData.Friend[] array = (from friend in instance.Friends.friends.Values
			where friend.online
			orderby friend.currentName
			select friend).ToArray();
		FriendData.Friend[] second = (from friend in instance.Friends.friends.Values
			where !friend.online
			orderby friend.currentName
			select friend).ToArray();
		FriendData.Friend[] array2 = array.Concat(second).ToArray();
		if (array2.Length != 0)
		{
			AchievementManager.UnlockAchievement(new AchievementManager.Achievement
			{
				name = "Not forever alone...",
				description = "Make a friend using the friend system.",
				icon = "Images/Achievements/notforeveralone.png"
			});
		}
		if (array2.Length >= 20)
		{
			AchievementManager.UnlockAchievement(new AchievementManager.Achievement
			{
				name = "Popular",
				description = "Have 25+ friends.",
				icon = "Images/Achievements/popular.png"
			});
		}
		if (array.Length > previousOnlineCount && array.Length != 0)
		{
			if (SoundEffects)
			{
				AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Friends/online.ogg", "Audio/Friends/online.ogg", delegate(AudioClip clip)
				{
					Main.Play2DAudio(clip, (float)Main.buttonClickVolume / 10f);
				});
			}
			NotificationManager.SendNotification(string.Format("<color=grey>[</color><color=green>FRIENDS</color><color=grey>]</color> You have {0}{1}friend{2} online.", array.Length - (previousOnlineCount + ((previousOnlineCount < 0) ? 1 : 0)), (previousOnlineCount < 0) ? " " : " new ", (array.Length > 1) ? "s" : ""), 5000);
		}
		if (instance.Friends.incoming.Values.Count > previousIncomingCount && instance.Friends.incoming.Values.Count > 0)
		{
			if (SoundEffects)
			{
				AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Friends/dooropen.ogg", "Audio/Friends/dooropen.ogg", delegate(AudioClip clip)
				{
					Main.Play2DAudio(clip, (float)Main.buttonClickVolume / 10f);
				});
			}
			NotificationManager.SendNotification(string.Format("<color=grey>[</color><color=green>FRIENDS</color><color=grey>]</color> You have {0}{1}friend request{2}.", instance.Friends.incoming.Values.Count - (previousIncomingCount + ((previousIncomingCount < 0) ? 1 : 0)), (previousIncomingCount < 0) ? " " : " new ", (instance.Friends.incoming.Values.Count > 1) ? "s" : ""), 5000);
		}
		previousOnlineCount = array.Length;
		previousIncomingCount = instance.Friends.incoming.Values.Count;
		Buttons.GetIndex("Friends").overlapText = ((array.Length != 0) ? $"Friends <color=grey>[</color><color=green>{array.Length} Online</color><color=grey>]</color>" : null);
		List<ButtonInfo> list = new List<ButtonInfo>
		{
			new ButtonInfo
			{
				buttonText = "Exit Friends",
				method = delegate
				{
					Buttons.CurrentCategoryName = "Main";
				},
				isTogglable = false,
				toolTip = "Returns you back to the main page."
			}
		};
		list.AddRange(array2.Select((FriendData.Friend friend, int i) => new ButtonInfo
		{
			buttonText = $"FriendButton{i}",
			overlapText = friend.currentName + (friend.online ? " <color=grey>[</color><color=green>Online</color><color=grey>]</color>" : " <color=grey>[</color><color=red>Offline</color><color=grey>]</color>"),
			method = delegate
			{
				InspectFriend(instance.Friends.friends.FirstOrDefault((KeyValuePair<string, FriendData.Friend> x) => x.Value == friend).Key);
			},
			isTogglable = false,
			toolTip = "See information on your friend " + friend.currentName + "."
		}));
		list.Add(new ButtonInfo
		{
			buttonText = "Add Friends",
			method = AddFriendsUI,
			isTogglable = false,
			toolTip = "Use this tab to add people as friends."
		});
		Buttons.buttons[Buttons.GetCategory("Friends")] = list.ToArray();
	}

	public static void AddFriendsUI()
	{
		List<ButtonInfo> list = new List<ButtonInfo>();
		list.Add(new ButtonInfo
		{
			buttonText = "Return to Friends",
			method = delegate
			{
				Buttons.CurrentCategoryName = "Friends";
			},
			isTogglable = false,
			toolTip = "Returns you back to the friends page."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Incoming Friend Requests",
			overlapText = "Incoming Friend Requests" + ((instance.Friends.incoming.Count > 0) ? $" <color=grey>[</color><color=green>{instance.Friends.incoming.Count}</color><color=grey>]</color>" : " "),
			method = IncomingFriendRequests,
			isTogglable = false,
			toolTip = "Shows your current incoming friend requests."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Outgoing Friend Requests",
			overlapText = "Outgoing Friend Requests" + ((instance.Friends.outgoing.Count > 0) ? $" <color=grey>[</color><color=green>{instance.Friends.outgoing.Count}</color><color=grey>]</color>" : " "),
			method = OutgoingFriendRequests,
			isTogglable = false,
			toolTip = "Shows your current outgoing friend requests."
		});
		List<ButtonInfo> list2 = list;
		if (NetworkSystem.Instance.InRoom)
		{
			list2.AddRange(from Player in NetworkSystem.Instance.PlayerListOthers
				where !IsPlayerFriend(Player) && !instance.pendingOutgoingRequests.Contains(Player.UserId)
				select new ButtonInfo
				{
					buttonText = "Friend <color=#" + Main.ColorToHex(RigUtilities.GetVRRigFromPlayer(Player).playerColor) + ">" + Player.NickName + "</color>",
					method = delegate
					{
						SendFriendRequest(Player.UserId);
					},
					isTogglable = false,
					toolTip = "Sends a friend request to " + Player.NickName + "."
				});
		}
		else
		{
			list2.Add(new ButtonInfo
			{
				buttonText = "Not in a Room",
				label = true
			});
		}
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list2.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void IncomingFriendRequests()
	{
		List<ButtonInfo> list = new List<ButtonInfo>();
		list.Add(new ButtonInfo
		{
			buttonText = "Return to Add Friends",
			method = AddFriendsUI,
			isTogglable = false,
			toolTip = "Returns you back to the add friends page."
		});
		List<ButtonInfo> list2 = list;
		FriendData.PendingFriend[] source = instance.Friends.incoming.Values.OrderBy((FriendData.PendingFriend friend) => friend.currentName).ToArray();
		list2.AddRange(source.Select((FriendData.PendingFriend friend, int i) => new ButtonInfo
		{
			buttonText = $"PendingFriend{i}",
			overlapText = friend.currentName,
			method = delegate
			{
				InspectPendingFriend(instance.Friends.incoming.FirstOrDefault((KeyValuePair<string, FriendData.PendingFriend> x) => x.Value == friend).Key);
			},
			isTogglable = false,
			toolTip = "Inspect " + friend.currentName + "'s friend request."
		}));
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list2.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void OutgoingFriendRequests()
	{
		List<ButtonInfo> list = new List<ButtonInfo>();
		list.Add(new ButtonInfo
		{
			buttonText = "Return to Add Friends",
			method = AddFriendsUI,
			isTogglable = false,
			toolTip = "Returns you back to the add friends page."
		});
		List<ButtonInfo> list2 = list;
		FriendData.PendingFriend[] source = instance.Friends.outgoing.Values.OrderBy((FriendData.PendingFriend friend) => friend.currentName).ToArray();
		list2.AddRange(source.Select((FriendData.PendingFriend friend, int i) => new ButtonInfo
		{
			buttonText = $"CancelFriend{i}",
			overlapText = friend.currentName,
			method = delegate
			{
				CancelFriendRequest(instance.Friends.outgoing.FirstOrDefault((KeyValuePair<string, FriendData.PendingFriend> x) => x.Value == friend).Key);
			},
			isTogglable = false,
			toolTip = "Cancels " + friend.currentName + "'s friend request."
		}));
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list2.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void InspectFriend(string friendTarget)
	{
		FriendData.Friend friend = instance.Friends.friends[friendTarget];
		List<ButtonInfo> list = new List<ButtonInfo>
		{
			new ButtonInfo
			{
				buttonText = "Return to Friends",
				method = delegate
				{
					Buttons.CurrentCategoryName = "Friends";
				},
				isTogglable = false,
				toolTip = "Returns you back to the friends page."
			}
		};
		if (friend.online && friend.currentRoom != "")
		{
			list.AddRange(new ButtonInfo[3]
			{
				new ButtonInfo
				{
					buttonText = "JoinFriend" + friendTarget,
					overlapText = "Join Friend",
					method = delegate
					{
						((PhotonNetworkController)PhotonNetworkController.Instance).AttemptToJoinSpecificRoom(instance.Friends.friends[friendTarget].currentRoom, (JoinType)0);
					},
					isTogglable = false,
					toolTip = "Joins the user " + friend.currentName + "'s current room."
				},
				new ButtonInfo
				{
					buttonText = "InviteFriend" + friendTarget,
					overlapText = "Invite Friend",
					method = delegate
					{
						InviteFriend(friendTarget);
					},
					isTogglable = false,
					toolTip = "Invites the user " + friend.currentName + " to your current room."
				},
				new ButtonInfo
				{
					buttonText = "RequestInviteFriend" + friendTarget,
					overlapText = "Request Invite",
					method = delegate
					{
						RequestInviteFriend(friendTarget);
					},
					isTogglable = false,
					toolTip = "Requests an invite from the user " + friend.currentName + "."
				}
			});
		}
		list.AddRange(new ButtonInfo[4]
		{
			new ButtonInfo
			{
				buttonText = "MessageLogs" + friendTarget,
				overlapText = "Message",
				method = delegate
				{
					ShowChatMessages(friendTarget);
				},
				isTogglable = false,
				toolTip = "Opens the chat menu for " + friend.currentName + "."
			},
			new ButtonInfo
			{
				buttonText = "SharePreferences" + friendTarget,
				overlapText = "Share Preferences",
				method = delegate
				{
					SharePreferences(friendTarget);
				},
				isTogglable = false,
				toolTip = "Sends your preferences to " + friend.currentName + "."
			},
			new ButtonInfo
			{
				buttonText = "ShareTheme" + friendTarget,
				overlapText = "Share Theme",
				method = delegate
				{
					ShareTheme(friendTarget);
				},
				isTogglable = false,
				toolTip = "Sends your theme to " + friend.currentName + "."
			},
			new ButtonInfo
			{
				buttonText = "ShareMacro" + friendTarget,
				overlapText = "Share Macro",
				method = delegate
				{
					Main.PromptText("What is the name of the macro you would like to send?", delegate
					{
						ShareMacro(friendTarget, Main.keyboardInput);
					}, null, "Done", "Cancel");
				},
				isTogglable = false,
				toolTip = "Sends a macro to " + friend.currentName + "."
			}
		});
		if (NetworkSystem.Instance.InRoom && friend.online && friend.currentRoom != "")
		{
			if (Quests.isSharingQuest && Quests.sharedWithPlayerId == friendTarget)
			{
				list.Add(new ButtonInfo
				{
					buttonText = "QuestShared" + friendTarget,
					overlapText = "<color=yellow>Quest Shared</color> <color=grey>[</color><color=green>" + Quests.sharedWithPlayerName + "</color><color=grey>]</color>",
					label = true
				});
				list.Add(new ButtonInfo
				{
					buttonText = "StopShareQuest" + friendTarget,
					overlapText = "Stop Sharing Quest",
					method = delegate
					{
						Quests.StopSharingQuest();
					},
					isTogglable = false,
					toolTip = "Stop sharing your quest with " + friend.currentName + "."
				});
			}
			else
			{
				list.Add(new ButtonInfo
				{
					buttonText = "ShareQuest" + friendTarget,
					overlapText = "Share Quest",
					method = delegate
					{
						Quests.ShareQuestWith(friendTarget, friend.currentName);
					},
					isTogglable = false,
					toolTip = "Share your active quest with " + friend.currentName + "."
				});
			}
		}
		list.Add(new ButtonInfo
		{
			buttonText = "RemoveFriend" + friendTarget,
			overlapText = "Remove Friend",
			method = delegate
			{
				RemoveFriend(friendTarget);
			},
			isTogglable = false,
			toolTip = "Removes the user " + friend.currentName + " from your friends list."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "FriendStatus" + friendTarget,
			overlapText = (friend.online ? "<color=grey>Status: </color><color=green>Online</color>" : "<color=grey>Status: </color><color=red>Offline</color>"),
			label = true
		});
		if (friend.online && friend.currentRoom != "")
		{
			list.Add(new ButtonInfo
			{
				buttonText = "FriendRoom" + friendTarget,
				overlapText = "Current Room: " + friend.currentRoom,
				label = true
			});
		}
		list.Add(new ButtonInfo
		{
			buttonText = "FriendName" + friendTarget,
			overlapText = "Current Name: " + friend.currentName,
			label = true
		});
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void InspectPendingFriend(string friendTarget)
	{
		FriendData.PendingFriend friend = instance.Friends.incoming[friendTarget];
		List<ButtonInfo> list = new List<ButtonInfo>();
		list.Add(new ButtonInfo
		{
			buttonText = "Return to Incoming Friend Requests",
			method = IncomingFriendRequests,
			isTogglable = false,
			toolTip = "Returns you back to the incoming friend requests page."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Accept Friend Request",
			method = delegate
			{
				AcceptFriendRequest(friend.currentUserID);
			},
			isTogglable = false,
			toolTip = "Accept " + friend.currentName + "'s friend request."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Deny Friend Request",
			method = delegate
			{
				DenyFriendRequest(friendTarget);
			},
			isTogglable = false,
			toolTip = "Deny " + friend.currentName + "'s friend request."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "FriendName" + friendTarget,
			overlapText = "Current Name: " + friend.currentName,
			label = true
		});
		List<ButtonInfo> list2 = list;
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list2.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void ShowChatMessages(string friendTarget)
	{
		FriendData.Friend friend = instance.Friends.friends[friendTarget];
		List<ButtonInfo> list = new List<ButtonInfo>
		{
			new ButtonInfo
			{
				buttonText = "Return to Friend Page",
				method = delegate
				{
					InspectFriend(friendTarget);
				},
				isTogglable = false,
				toolTip = "Returns you back to the page of your friend."
			}
		};
		List<string> list2 = new List<string>();
		int num = Main.PageSize - 2;
		string path = "SeralythMenu/Friends/Messages/" + friendTarget + ".json";
		if (!File.Exists(path))
		{
			File.WriteAllText(path, "{\"messages\":[]}");
		}
		else
		{
			JObject val = JObject.Parse(File.ReadAllText(path));
			JToken obj = val["messages"];
			list2 = ((obj != null) ? obj.ToObject<List<string>>() : null) ?? new List<string>();
			if (list2.Count > num)
			{
				list2 = list2.Skip(list2.Count - num).ToList();
			}
		}
		while (list2.Count < num)
		{
			list2.Insert(0, "");
		}
		for (int num2 = 0; num2 < list2.Count; num2++)
		{
			string text = list2[num2];
			string link = Main.ExtractPromptImage(text);
			string overlapText = ((link != null) ? text.Replace("<" + link + ">", "[Media]") : text);
			list.Add((link != null) ? new ButtonInfo
			{
				buttonText = $"FriendMessage{num2}",
				overlapText = overlapText,
				isTogglable = false,
				method = delegate
				{
					Main.Prompt("<" + link + ">", null, delegate
					{
						GUIUtility.systemCopyBuffer = link;
					}, "Done", "Copy");
				}
			} : new ButtonInfo
			{
				buttonText = $"FriendMessage{num2}",
				overlapText = overlapText,
				label = true
			});
		}
		list.Add(new ButtonInfo
		{
			buttonText = "Message" + friendTarget,
			overlapText = "Message",
			method = delegate
			{
				Main.PromptText("What would you like to send?", delegate
				{
					//IL_002d: Unknown result type (might be due to invalid IL or missing references)
					SendFriendMessage(friendTarget, Main.keyboardInput);
					UpdateFriendMessage(friendTarget, "        <color=grey>[</color><color=#" + Main.ColorToHex(VRRig.LocalRig.playerColor) + ">" + PhotonNetwork.NickName.ToUpper() + "</color><color=grey>]</color> " + Main.keyboardInput);
					ShowChatMessages(friendTarget);
					Main.ReloadMenu();
				}, null, "Done", "Cancel");
			},
			isTogglable = false,
			toolTip = "Sends a message to " + friend.currentName + "."
		});
		Buttons.buttons[Buttons.GetCategory("Chat Messages")] = list.ToArray();
		Buttons.CurrentCategoryName = "Chat Messages";
	}
}
