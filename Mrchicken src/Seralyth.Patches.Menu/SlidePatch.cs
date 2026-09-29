using GorillaLocomotion;
using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(GTPlayer), "GetSlidePercentage")]
public class SlidePatch
{
	public static bool everythingSlippery;

	public static bool everythingGrippy;

	public static bool minimalSlip;

	public static void Postfix(GTPlayer __instance, ref float __result)
	{
		if (everythingSlippery)
		{
			__result = 1f;
		}
		if (everythingGrippy)
		{
			__result = 0f;
		}
		if (minimalSlip)
		{
			__result *= 0.75f;
		}
	}
}
