using Seralyth.Managers.DiscordRPC.Message;

namespace Seralyth.Managers.DiscordRPC.Events;

public delegate void OnConnectionEstablishedEvent(object sender, ConnectionEstablishedMessage args);
