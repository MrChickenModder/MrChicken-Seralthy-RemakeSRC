using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(GliderHoldable), "Respawn")]
public class GliderPatch
{
	public static bool enabled;

	public static bool Prefix()
	{
		return !enabled;
	}
}
