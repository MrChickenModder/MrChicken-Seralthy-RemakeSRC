using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(SIGadgetPlatformDeployer), "OnUpdateAuthority")]
public class PlatformPatch
{
	public static bool enabled;

	public static void Prefix(SIGadgetPlatformDeployer __instance, float dt)
	{
		if (enabled)
		{
			__instance.remainingRechargeTime = 0f;
		}
	}
}
