using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(SIGadgetChargeBlaster), "OnUpdateAuthority")]
public class CooldownPatch
{
	public static bool enabled;

	public static void Prefix(SIGadgetChargeBlaster __instance, float dt)
	{
		if (enabled)
		{
			__instance.fireCooldown = 0f;
		}
	}
}
