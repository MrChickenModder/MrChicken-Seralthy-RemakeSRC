using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(TakeMyHand_HandLink), "LocalUpdate")]
public class GroundedPatch
{
	public static bool enabled;

	public static void Postfix(TakeMyHand_HandLink __instance, bool isGroundedHand, bool isGroundedButt, bool isGripPressed, bool isReadyForGrabbing)
	{
		if (enabled)
		{
			__instance.isGroundedHand = true;
		}
	}
}
