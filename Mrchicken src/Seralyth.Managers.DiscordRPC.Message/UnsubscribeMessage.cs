using Seralyth.Managers.DiscordRPC.RPC.Payload;

namespace Seralyth.Managers.DiscordRPC.Message;

public class UnsubscribeMessage : IMessage
{
	public override MessageType Type => MessageType.Unsubscribe;

	public EventType Event { get; internal set; }

	internal UnsubscribeMessage(ServerEvent evt)
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
