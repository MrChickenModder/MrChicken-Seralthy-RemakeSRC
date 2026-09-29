using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(GorillaGameManager), "ValidGameMode")]
public class GameModePatch
{
	public static bool enabled;

	public static void Postfix(GorillaGameManager __instance, ref bool __result)
	{
		if (enabled)
		{
			__result = true;
		}
	}
}
