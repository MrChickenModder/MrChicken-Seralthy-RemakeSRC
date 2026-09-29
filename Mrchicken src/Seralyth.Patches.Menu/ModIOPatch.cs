using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(ModIOManager), "OnJoinedRoom")]
public class ModIOPatch
{
	public static bool Prefix(VRRig __instance)
	{
		return false;
	}
}
