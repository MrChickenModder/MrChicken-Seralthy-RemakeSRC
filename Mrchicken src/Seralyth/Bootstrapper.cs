using System;
using System.Collections;
using System.IO;
using System.Linq;
using Seralyth.Classes.Menu;
using Seralyth.Managers;
using Seralyth.Menu;
using Seralyth.Mods;
using Seralyth.Patches;
using Seralyth.Patches.Menu;
using UnityEngine;

namespace Seralyth;

internal static class Bootstrapper
{
	private static bool initialized;

	public static bool FirstLaunch;

	public static GameObject Loader;

	internal static void Initialize()
	{
		if (initialized)
		{
			return;
		}
		initialized = true;
		FirstLaunch = !Directory.Exists("SeralythMenu");
		string[] array = new string[11]
		{
			"", "/Sounds", "/Plugins", "/Backups", "/Macros", "/TTS", "/PlayerInfo", "/CustomScripts", "/Friends", "/Friends/Messages",
			"/Achievements"
		};
		string[] array2 = array;
		foreach (string text in array2)
		{
			string path = "SeralythMenu" + text;
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}
		PatchHandler.PatchAll(awake: true);
		if (File.Exists("SeralythMenu/Seralyth_Preferences.txt") && File.ReadAllLines("SeralythMenu/Seralyth_Preferences.txt")[0].Split(";;").Contains("Accept TOS"))
		{
			TOSPatches.enabled = true;
		}
		if (File.Exists("SeralythMenu/Seralyth_DisableTelemetry.txt"))
		{
			ServerData.DisableTelemetry = true;
		}
		GorillaTagger.OnPlayerSpawned((Action)LoadMenu);
	}

	private static void LoadMenu()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		PatchHandler.PatchAll();
		Loader = new GameObject("Seralyth_Loader");
		CoroutineManager coroutineManager = Loader.AddComponent<CoroutineManager>();
		Loader.AddComponent<NotificationManager>();
		Loader.AddComponent<CustomBoardManager>();
		Loader.AddComponent<UI>();
		Loader.AddComponent<PCOnGUIMenu>();
		Object.DontDestroyOnLoad((Object)(object)Loader);
		PlayerTagManager.Load();
		((MonoBehaviour)coroutineManager).StartCoroutine(PatchIntegrityCheck());
	}

	private static IEnumerator PatchIntegrityCheck()
	{
		if (PatchHandler.instance == null)
		{
			yield return null;
		}
		PatchHandler.PatchIntegrityCheck();
	}
}
