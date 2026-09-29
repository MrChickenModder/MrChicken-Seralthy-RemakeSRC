using GorillaTagScripts;
using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(LurkerGhost), "ChangeState")]
public class LurkerPatch
{
	public static bool enabled;

	public static bool Prefix(LurkerGhost __instance, ghostState newState)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		return !enabled || (int)newState != 3 || __instance.targetPlayer != NetworkSystem.Instance.LocalPlayer;
	}
}
