using HarmonyLib;
using Seralyth.Extensions;
using Seralyth.Mods;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(SlingshotProjectile), "CheckForAOEKnockback")]
public class MultiplySelfKnockbackPatch
{
	public static bool enabled;

	public static void Prefix(SlingshotProjectile __instance)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (enabled && __instance.projectileOwner == VRRig.LocalRig.GetPlayer() && __instance.aoeKnockbackConfig.HasValue)
		{
			AOEKnockbackConfig value = __instance.aoeKnockbackConfig.Value;
			value.knockbackVelocity = value.knockbackVelocity * (float)Movement.multiplicationAmount / 10f;
			__instance.aoeKnockbackConfig = value;
		}
	}
}
