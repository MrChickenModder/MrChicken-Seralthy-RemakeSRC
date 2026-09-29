using HarmonyLib;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(GorillaSpeakerLoudness), "UpdateLoudness")]
public class LoudnessPatch
{
	public static bool enabled;

	private static bool Prefix(GorillaSpeakerLoudness __instance, ref bool ___isMicEnabled, ref bool ___isSpeaking, ref float ___loudness)
	{
		return !enabled || ((Object)((Component)__instance).gameObject).name != "Local Gorilla Player";
	}
}
