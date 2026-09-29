using GorillaLocomotion;
using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(GTPlayer), "TakeMyHand_ProcessMovement")]
public class HandLinkPatch
{
	public static bool enabled;

	public static bool Prefix(GTPlayer __instance)
	{
		return !enabled;
	}
}
