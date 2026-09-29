using GorillaLocomotion;
using HarmonyLib;
using Seralyth.Mods;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(GTPlayer), "ApplyKnockback")]
public class MultiplyKnockback
{
	public static bool enabled;

	public static void Prefix(ref float speed)
	{
		if (enabled)
		{
			speed = speed * (float)Movement.multiplicationAmount / 10f;
		}
	}
}
