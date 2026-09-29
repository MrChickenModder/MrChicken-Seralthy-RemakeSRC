using HarmonyLib;
using Seralyth.Mods;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(SnowballThrowable), "GetRandomModelIndex")]
public class IndexPatch
{
	public static bool enabled;

	public static bool Prefix(SnowballThrowable __instance, ref int __result)
	{
		if (enabled)
		{
			if (__instance.localModels.Count == 0)
			{
				__result = -1;
				return false;
			}
			__instance.randModelIndex = Projectiles.targetProjectileIndex % __instance.localModels.Count;
			__result = Projectiles.targetProjectileIndex;
			return false;
		}
		return true;
	}
}
