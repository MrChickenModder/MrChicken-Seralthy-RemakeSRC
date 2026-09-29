using Seralyth.Managers.DiscordRPC.Message;

namespace Seralyth.Managers.DiscordRPC.Events;

public delegate void OnPresenceUpdateEvent(object sender, PresenceMessage args);
