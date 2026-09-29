using System;

namespace Seralyth.Managers.DiscordRPC.Exceptions;

public class BadPresenceException : Exception
{
	internal BadPresenceException(string message)
		: base(message)
	{
	}
}
