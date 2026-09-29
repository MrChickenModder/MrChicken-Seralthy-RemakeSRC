using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(AprilFoolsGravityFX), "Start")]
public class AprilFoolsGravityFXEnablePatch
{
	public static bool enabled;

	private static bool Prefix()
	{
		return !enabled;
	}
}
