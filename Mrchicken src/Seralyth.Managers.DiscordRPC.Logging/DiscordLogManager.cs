using System;

namespace Seralyth.Managers.DiscordRPC.Logging;

public class DiscordLogManager : ILogger
{
	public LogLevel Level { get; set; }

	public bool Coloured { get; set; }

	[Obsolete("Use Coloured")]
	public bool Colored
	{
		get
		{
			return Coloured;
		}
		set
		{
			Coloured = value;
		}
	}

	public DiscordLogManager()
	{
		Level = LogLevel.Info;
		Coloured = false;
	}

	public DiscordLogManager(LogLevel level)
		: this()
	{
		Level = level;
	}

	public DiscordLogManager(LogLevel level, bool coloured)
	{
		Level = level;
		Coloured = coloured;
	}

	public void Trace(string message, params object[] args)
	{
		if (Level <= LogLevel.Trace)
		{
			if (Coloured)
			{
				Console.ForegroundColor = ConsoleColor.Gray;
			}
			string log = "TRACE: " + message;
			if (args.Length != 0)
			{
				LogManager.Log(log, args);
			}
			else
			{
				LogManager.Log(log);
			}
		}
	}

	public void Info(string message, params object[] args)
	{
		if (Level <= LogLevel.Info)
		{
			string log = "INFO: " + message;
			if (args.Length != 0)
			{
				LogManager.Log(log, args);
			}
			else
			{
				LogManager.Log(log);
			}
		}
	}

	public void Warning(string message, params object[] args)
	{
		if (Level <= LogLevel.Warning)
		{
			string log = "WARN: " + message;
			if (args.Length != 0)
			{
				LogManager.LogWarning(log, args);
			}
			else
			{
				LogManager.LogWarning(log);
			}
		}
	}

	public void Error(string message, params object[] args)
	{
		if (Level <= LogLevel.Error)
		{
			string log = "ERR : " + message;
			if (args.Length != 0)
			{
				LogManager.LogError(log, args);
			}
			else
			{
				LogManager.LogError(log);
			}
		}
	}
}
