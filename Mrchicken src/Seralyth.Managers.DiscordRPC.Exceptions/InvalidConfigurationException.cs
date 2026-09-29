using System;

namespace Seralyth.Managers.DiscordRPC.Exceptions;

public class InvalidConfigurationException : Exception
{
	internal InvalidConfigurationException(string message)
		: base(message)
	{
	}
}
