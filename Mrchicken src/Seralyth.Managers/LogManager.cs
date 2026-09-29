using System;
using UnityEngine;

namespace Seralyth.Managers;

public static class LogManager
{
	private static Action<Level, string> _sink;

	public static void SetLogger(Action<Level, string> sink)
	{
		_sink = sink;
	}

	private static void Write(Level level, object log)
	{
		string text = log?.ToString() ?? string.Empty;
		if (_sink == null)
		{
			Debug.Log((object)$"[{level}] {text}");
		}
		else
		{
			_sink(level, text);
		}
	}

	public static void Log(object log)
	{
		Write(Level.Info, log);
	}

	public static void Log(object log, object[] args)
	{
		Write(Level.Info, string.Format(log?.ToString() ?? "", args));
	}

	public static void LogError(object log)
	{
		Write(Level.Error, log);
	}

	public static void LogError(object log, object[] args)
	{
		Write(Level.Error, string.Format(log?.ToString() ?? "", args));
	}

	public static void LogWarning(object log)
	{
		Write(Level.Warning, log);
	}

	public static void LogWarning(object log, object[] args)
	{
		Write(Level.Warning, string.Format(log?.ToString() ?? "", args));
	}

	public static void LogDebug(object log)
	{
		Write(Level.Debug, log);
	}

	public static void LogDebug(object log, object[] args)
	{
		Write(Level.Debug, string.Format(log?.ToString() ?? "", args));
	}
}
