using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Seralyth.Managers.DiscordRPC.IO;

public static class PipeLocation
{
	private const string DiscordPipePrefix = "discord-ipc-";

	private const int MaximumPipeVariations = 10;

	private static readonly string[] LinuxPackageManagers = new string[2] { "app/com.discordapp.Discord/", "snap.discord/" };

	public static IEnumerable<string> GetPipes(int startPipe = 0)
	{
		IsOSUnix();
		return IsOSUnix() ? Enumerable.Range(startPipe, 10).SelectMany(GetUnixPipes) : Enumerable.Range(startPipe, 10).SelectMany(GetWindowsPipes);
	}

	private static IEnumerable<string> GetWindowsPipes(int index)
	{
		yield return string.Format("{0}{1}", "discord-ipc-", index);
	}

	private static IEnumerable<string> GetUnixPipes(int index)
	{
		foreach (string tempDir in TemporaryDirectories())
		{
			yield return Path.Combine(tempDir, string.Format("{0}{1}", "discord-ipc-", index));
			string[] linuxPackageManagers = LinuxPackageManagers;
			foreach (string pmDir in linuxPackageManagers)
			{
				yield return Path.Combine(tempDir, pmDir, string.Format("{0}{1}", "discord-ipc-", index));
			}
		}
	}

	private static IEnumerable<string> TemporaryDirectories()
	{
		string temp = Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR");
		if (temp != null)
		{
			yield return temp;
		}
		temp = Environment.GetEnvironmentVariable("TMPDIR");
		if (temp != null)
		{
			yield return temp;
		}
		temp = Environment.GetEnvironmentVariable("TMP");
		if (temp != null)
		{
			yield return temp;
		}
		temp = Environment.GetEnvironmentVariable("TEMP");
		if (temp != null)
		{
			yield return temp;
		}
		yield return "/temp";
	}

	private static bool IsOSUnix()
	{
		if (Environment.OSVersion.Platform == PlatformID.Unix)
		{
			return true;
		}
		return false;
	}
}
