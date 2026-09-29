using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(RoomSystem), "SearchForNearby")]
public class GroupPatch
{
	public static bool enabled;

	public static bool Prefix()
	{
		return !enabled;
	}
}
