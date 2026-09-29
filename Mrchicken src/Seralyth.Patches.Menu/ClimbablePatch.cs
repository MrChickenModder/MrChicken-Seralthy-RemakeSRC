using GorillaLocomotion.Climbing;
using HarmonyLib;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(GorillaHandClimber), "GetClosestClimbable")]
public class ClimbablePatch
{
	public static bool enabled;

	private static void Postfix(GorillaHandClimber __instance, ref GorillaClimbable __result)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		if (!enabled || !((Object)(object)__result == (Object)null))
		{
			return;
		}
		int count = __instance.potentialClimbables.Count;
		int num = count;
		if ((uint)num <= 1u)
		{
			return;
		}
		Vector3 position = ((Component)__instance).transform.position;
		Bounds bounds = __instance.col.bounds;
		Collider col = __instance.col;
		float num2 = ((SphereCollider)((col is SphereCollider) ? col : null)).radius + 0.05f;
		GorillaClimbable val = null;
		foreach (GorillaClimbable potentialClimbable in __instance.potentialClimbables)
		{
			float num3;
			if (Object.op_Implicit((Object)(object)potentialClimbable.colliderCache))
			{
				if (!((Bounds)(ref bounds)).Intersects(potentialClimbable.colliderCache.bounds))
				{
					continue;
				}
				Vector3 val2 = potentialClimbable.colliderCache.ClosestPoint(position);
				num3 = Vector3.Distance(position, val2);
			}
			else
			{
				num3 = Vector3.Distance(position, ((Component)potentialClimbable).transform.position);
			}
			if (num3 < num2)
			{
				val = potentialClimbable;
				num2 = num3;
			}
		}
		__result = val;
	}
}
