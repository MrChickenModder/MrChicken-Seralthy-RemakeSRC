using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(KIDManager), "HasPermissionToUseFeature")]
public class PermissionPatch
{
	public static bool enabled;

	public static void Postfix(ref bool __result)
	{
		if (enabled)
		{
			__result = true;
		}
	}
}
