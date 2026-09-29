using Seralyth.Managers.DiscordRPC.Message;

namespace Seralyth.Managers.DiscordRPC.Events;

public delegate void OnSubscribeEvent(object sender, SubscribeMessage args);
