using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Seralyth.Classes.Menu;

public static class UpdateChecker
{
	public const string UpdateDLLURL = "https://raw.githubusercontent.com/MrChickenModder/MrChickens-Seralthy-Remake/refs/heads/main/MrChicken-1.dll";

	public const string UpdateFileName = "MrChicken.dll";

	public static bool IsUpdateAvailable(string downloadedPath)
	{
		try
		{
			string location = typeof(ServerData).Assembly.Location;
			if (string.IsNullOrEmpty(location) || !File.Exists(location) || !File.Exists(downloadedPath))
			{
				return false;
			}
			return ComputeHash(location) != ComputeHash(downloadedPath);
		}
		catch (Exception ex)
		{
			Console.Log("[Update] Comparison failed: " + ex.Message);
			return false;
		}
	}

	public static void ScheduleUpdate(string downloadedPath)
	{
		try
		{
			string directoryName = Path.GetDirectoryName(typeof(ServerData).Assembly.Location);
			string text = Path.Combine(directoryName, "MrChicken.dll");
			string text2 = Path.Combine(directoryName, "MrChickenUpdate.bat");
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("@echo off");
			stringBuilder.AppendLine("title MrChicken Menu Update");
			stringBuilder.AppendLine(":wait");
			stringBuilder.AppendLine("tasklist /fi \"imagename eq Gorilla Tag.exe\" | find /i \"Gorilla Tag.exe\" >nul 2>&1");
			stringBuilder.AppendLine("if not errorlevel 1 (timeout /t 2 /nobreak >nul & goto wait)");
			stringBuilder.AppendLine("copy /y \"" + downloadedPath + "\" \"" + text + "\"");
			stringBuilder.AppendLine("if exist \"" + downloadedPath + "\" del \"" + downloadedPath + "\"");
			stringBuilder.AppendLine("del \"%~f0\"");
			File.WriteAllText(text2, stringBuilder.ToString());
			Process.Start(new ProcessStartInfo
			{
				FileName = "cmd.exe",
				Arguments = "/c \"" + text2 + "\"",
				WindowStyle = ProcessWindowStyle.Hidden,
				UseShellExecute = false,
				CreateNoWindow = true
			});
			Console.Log("[Update] Update scheduled, will be applied on game exit");
		}
		catch (Exception ex)
		{
			Console.Log("[Update] Failed to schedule update: " + ex.Message);
		}
	}

	public static string ComputeHash(string path)
	{
		using SHA256 sHA = SHA256.Create();
		using FileStream inputStream = File.OpenRead(path);
		return BitConverter.ToString(sHA.ComputeHash(inputStream));
	}
}
