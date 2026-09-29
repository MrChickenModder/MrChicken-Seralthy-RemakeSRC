using System.IO;
using Seralyth.Managers;
using Seralyth.Menu;

namespace Seralyth.Mods;

public static class Presets
{
	public static void LegitimatePreset()
	{
		string[] array = new string[13]
		{
			"Joystick Menu", "Thin Menu", "Disable Enabled GUI", "Disable Board Colors", "Disable Disconnect Button", "Disable Page Buttons", "Disable Search Button", "Disable Return Button", "Hidden on Camera", "Hide Notifications on Camera",
			"Hide Text on Camera", "Disable FPS Counter", "Fix Rig Colors"
		};
		Main.themeType = 28;
		Main.pageButtonType = 1;
		Main.fontCycle = -1;
		Settings.ChangeMenuTheme();
		Settings.ChangePageType();
		Settings.ChangeFontType();
		Settings.Panic();
		string[] array2 = array;
		foreach (string buttonText in array2)
		{
			Main.Toggle(buttonText);
		}
		NotificationManager.SendNotification("<color=grey>[</color><color=purple>PRESET</color><color=grey>]</color> Legitimate preset enabled successfully.");
	}

	public static void GhostPreset()
	{
		string[] array = new string[8] { "Ghost <color=grey>[</color><color=green>A</color><color=grey>]</color>", "Invisible <color=grey>[</color><color=green>B</color><color=grey>]</color>", "Noclip <color=grey>[</color><color=green>T</color><color=grey>]</color>", "Steam Long Arms", "Break Audio Gun", "No Finger Movement", "Platforms", "Change Arm Length" };
		Movement.longarmCycle = 2;
		Settings.Panic();
		string[] array2 = array;
		foreach (string buttonText in array2)
		{
			Main.Toggle(buttonText);
		}
		NotificationManager.SendNotification("<color=grey>[</color><color=purple>PRESET</color><color=grey>]</color> Ghost preset enabled successfully.");
	}

	public static void SaveCustomPreset(int id)
	{
		if (!Directory.Exists("SeralythMenu/SavedPresets"))
		{
			Directory.CreateDirectory("SeralythMenu/SavedPresets");
		}
		File.WriteAllText("SeralythMenu/SavedPresets/Preset_" + id + ".txt", Settings.SavePreferencesToText());
	}

	public static void LoadCustomPreset(int id)
	{
		if (Directory.Exists("SeralythMenu/SavedPresets"))
		{
			string text = File.ReadAllText("SeralythMenu/SavedPresets/Preset_" + id + ".txt");
			LogManager.Log(text);
			Settings.LoadPreferencesFromText(text);
		}
	}

	public static void PerformancePreset()
	{
		string[] array = new string[5] { "Disable Enabled GUI", "Disable Board Colors", "Disable FPS Counter", "FPS Boost", "Disable Ghostview" };
		Main.themeType = 30;
		Main.pageButtonType = 1;
		Main.fontCycle = 0;
		Settings.ChangeMenuTheme();
		Settings.ChangePageType();
		Settings.ChangeFontType();
		Settings.Panic();
		string[] array2 = array;
		foreach (string buttonText in array2)
		{
			Main.Toggle(buttonText);
		}
		NotificationManager.SendNotification("<color=grey>[</color><color=purple>PRESET</color><color=grey>]</color> Performance preset enabled successfully.");
	}

	public static void SafetyPreset()
	{
		string[] array = new string[7] { "No Finger Movement", "Fake Oculus Menu <color=grey>[</color><color=green>X</color><color=grey>]</color>", "Disable Gamemode Buttons", "Anti Crash", "Anti Moderator", "Anti Report <color=grey>[</color><color=green>Disconnect</color><color=grey>]</color>", "Show Anti Cheat Reports <color=grey>[</color><color=green>Self</color><color=grey>]</color>" };
		Main.themeType = 33;
		Main.pageButtonType = 1;
		Main.fontCycle = 0;
		Settings.ChangeMenuTheme();
		Settings.ChangePageType();
		Settings.ChangeFontType();
		Settings.Panic();
		string[] array2 = array;
		foreach (string buttonText in array2)
		{
			Main.Toggle(buttonText);
		}
		NotificationManager.SendNotification("<color=grey>[</color><color=purple>PRESET</color><color=grey>]</color> Safety preset enabled successfully.");
	}

	public static void SimplePreset()
	{
		string[] array = new string[4] { "Disable Enabled GUI", "Accept TOS", "Player Scale Menu", "PC Button Click" };
		Main.pageButtonType = 2;
		string[] array2 = array;
		foreach (string buttonText in array2)
		{
			Main.Toggle(buttonText);
		}
		NotificationManager.SendNotification("<color=grey>[</color><color=purple>PRESET</color><color=grey>]</color> Simple preset enabled successfully.");
	}
}
