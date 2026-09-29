using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(GorillaTagger), "LateUpdate")]
public class TimerPatch
{
	public static bool enabled;

	private static float oldDeltaTime;

	public static void Prefix(GorillaTagger __instance)
	{
		if (enabled)
		{
			oldDeltaTime = Time.fixedDeltaTime;
			__instance._framerateUpdated = true;
			Time.fixedDeltaTime = 1f / XRDevice.refreshRate;
		}
	}

	public static void Postfix(GorillaTagger __instance)
	{
		if (enabled)
		{
			Time.fixedDeltaTime = oldDeltaTime;
		}
	}
}
