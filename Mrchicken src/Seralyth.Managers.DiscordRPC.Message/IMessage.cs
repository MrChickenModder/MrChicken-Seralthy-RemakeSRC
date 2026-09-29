using System;

namespace Seralyth.Managers.DiscordRPC.Message;

public abstract class IMessage
{
	private readonly DateTime _timecreated;

	public abstract MessageType Type { get; }

	public DateTime TimeCreated => _timecreated;

	public IMessage()
	{
		_timecreated = DateTime.Now;
	}
}
