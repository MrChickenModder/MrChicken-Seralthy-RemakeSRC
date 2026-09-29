using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(SIGadgetWristJet), "OnUpdateAuthority")]
public class FuelPatch
{
	public static bool enabled;

	public static void Postfix(SIGadgetWristJet __instance, float dt)
	{
		if (enabled)
		{
			__instance.currentFuel = __instance.fuelSize;
		}
	}
}
