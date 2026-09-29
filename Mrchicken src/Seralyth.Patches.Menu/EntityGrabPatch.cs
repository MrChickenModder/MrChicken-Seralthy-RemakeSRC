using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(GameEntityManager), "TryGrabLocal")]
public class EntityGrabPatch
{
	public static bool enabled;

	public static bool Prefix(GameEntityManager __instance, Vector3 handPosition, bool isLeftHand, Vector3 closestPointOnBoundingBox, ref GameEntityId __result)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		if (enabled)
		{
			List<GameEntity> entities = __instance.entities;
			GameEntityId val = GameEntityId.Invalid;
			float num = float.MaxValue;
			for (int i = 0; i < entities.Count; i++)
			{
				GameEntity val2 = entities[i];
				if ((Object)(object)val2 != (Object)null && __instance.ValidateGrab(val2, NetworkSystem.Instance.LocalPlayer.ActorNumber, isLeftHand))
				{
					double num2 = 16.0;
					Vector3 val3 = handPosition - ((Component)val2).transform.position;
					float sqrMagnitude = ((Vector3)(ref val3)).sqrMagnitude;
					if ((double)sqrMagnitude < num2 && sqrMagnitude < num)
					{
						val = val2.id;
						num = sqrMagnitude;
					}
				}
			}
			__result = val;
			return false;
		}
		return true;
	}
}
