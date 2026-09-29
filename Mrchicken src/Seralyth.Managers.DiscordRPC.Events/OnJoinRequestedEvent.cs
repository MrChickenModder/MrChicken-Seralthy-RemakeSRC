using Seralyth.Managers.DiscordRPC.Message;

namespace Seralyth.Managers.DiscordRPC.Events;

public delegate void OnJoinRequestedEvent(object sender, JoinRequestMessage args);
