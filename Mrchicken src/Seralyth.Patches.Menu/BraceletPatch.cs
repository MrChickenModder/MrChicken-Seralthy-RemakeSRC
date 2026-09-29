using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(VRRig), "UpdateFriendshipBracelet")]
public class BraceletPatch
{
	public static bool enabled;

	public static bool Prefix(VRRig __instance)
	{
		return !enabled;
	}
}
