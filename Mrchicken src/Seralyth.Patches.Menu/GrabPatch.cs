using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(VRRig), "GrabbedByPlayer")]
public class GrabPatch
{
	public static bool enabled;

	public static bool Prefix(VRRig __instance, VRRig grabbedByRig, bool grabbedBody, bool grabbedLeftHand, bool grabbedWithLeftHand)
	{
		return !enabled;
	}
}
