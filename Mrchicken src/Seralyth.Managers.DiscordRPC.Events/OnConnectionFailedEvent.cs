using Seralyth.Managers.DiscordRPC.Message;

namespace Seralyth.Managers.DiscordRPC.Events;

public delegate void OnConnectionFailedEvent(object sender, ConnectionFailedMessage args);
