using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(LightningManager), "DoLightningStrike")]
public class LightningPatch
{
	public static bool enabled;

	public static bool Prefix()
	{
		return !enabled;
	}
}
