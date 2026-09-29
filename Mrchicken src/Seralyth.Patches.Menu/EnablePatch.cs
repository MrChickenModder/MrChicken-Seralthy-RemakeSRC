using HarmonyLib;
using Seralyth.Mods;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(GrowingSnowballThrowable), "OnEnable")]
public class EnablePatch
{
	public static bool enabled;

	public static void Postfix(GrowingSnowballThrowable __instance)
	{
		if (enabled)
		{
			__instance.IncreaseSize(Overpowered.snowballScale);
		}
	}
}
