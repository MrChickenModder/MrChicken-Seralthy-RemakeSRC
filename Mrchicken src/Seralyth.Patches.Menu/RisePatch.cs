using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(HalloweenGhostChaser), "RiseGrabbedLocalPlayer")]
public class RisePatch
{
	public static bool enabled;

	public static bool Prefix()
	{
		return !enabled;
	}
}
