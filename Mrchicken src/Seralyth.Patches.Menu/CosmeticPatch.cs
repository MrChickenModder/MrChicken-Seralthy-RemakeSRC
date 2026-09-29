using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(VRRig), "IsItemAllowed")]
public class CosmeticPatch
{
	public static bool enabled;

	public static void Postfix(VRRig __instance, ref bool __result)
	{
		if (enabled)
		{
			__result = true;
		}
	}
}
