using HarmonyLib;
using Seralyth.Utilities;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(GrowingSnowballThrowable), "PerformSnowballThrowAuthority")]
public class ThrowPatch
{
	public static bool enabled;

	public static readonly int extraCount = 5;

	public static bool Prefix(GrowingSnowballThrowable __instance)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (enabled)
		{
			enabled = false;
			Vector3 linearVelocity = ((SnowballThrowable)__instance).velocityEstimator.linearVelocity;
			for (int i = 0; i < extraCount; i++)
			{
				((SnowballThrowable)__instance).velocityEstimator.linearVelocity = linearVelocity + RandomUtilities.RandomVector3(2f);
				((SnowballThrowable)__instance).PerformSnowballThrowAuthority();
			}
			enabled = true;
			return false;
		}
		return true;
	}
}
