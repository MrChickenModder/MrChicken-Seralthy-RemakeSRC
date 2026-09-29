using Seralyth.Managers.DiscordRPC.RPC.Payload;

namespace Seralyth.Managers.DiscordRPC.Message;

public class SubscribeMessage : IMessage
{
	public override MessageType Type => MessageType.Subscribe;

	public EventType Event { get; internal set; }

	internal SubscribeMessage(ServerEvent evt)
	{
		if (evt == ServerEvent.ActivityJoin || evt != ServerEvent.ActivityJoinRequest)
		{
			Event = EventType.Join;
		}
		else
		{
			Event = EventType.JoinRequest;
		}
	}
}
