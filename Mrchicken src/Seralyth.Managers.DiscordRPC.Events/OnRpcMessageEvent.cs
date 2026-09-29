using Seralyth.Managers.DiscordRPC.Message;

namespace Seralyth.Managers.DiscordRPC.Events;

public delegate void OnRpcMessageEvent(object sender, IMessage msg);
