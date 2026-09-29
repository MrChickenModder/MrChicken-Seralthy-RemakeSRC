using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(HalloweenGhostChaser), "LateUpdate")]
public class LucyPatch
{
	public static bool enabled;

	public static bool Prefix()
	{
		return !enabled;
	}
}
