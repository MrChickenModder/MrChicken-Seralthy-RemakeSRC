using System;
using HarmonyLib;
using Seralyth.Mods;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(VRRig), "PostTick")]
public class TorsoPatch
{
	public static bool enabled;

	public static int mode;

	public static event Action VRRigLateUpdate;

	public static void Postfix(VRRig __instance)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		if (!__instance.isLocal)
		{
			return;
		}
		if (enabled)
		{
			Quaternion rotation = Quaternion.identity;
			Quaternion rotation2;
			switch (mode)
			{
			case 0:
				rotation = Quaternion.Euler(0f, Time.time * 180f % 360f, 0f);
				break;
			case 1:
				rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
				break;
			case 2:
				rotation2 = ((Component)GorillaTagger.Instance.headCollider).transform.rotation;
				rotation = Quaternion.Euler(0f, ((Quaternion)(ref rotation2)).eulerAngles.y + 180f, 0f);
				break;
			case 3:
				rotation2 = Movement.recBodyRotary.transform.rotation;
				rotation = Quaternion.Euler(0f, ((Quaternion)(ref rotation2)).eulerAngles.y, 0f);
				break;
			}
			((Component)__instance).transform.rotation = rotation;
			__instance.head.MapMine(__instance.scaleFactor, __instance.playerOffsetTransform);
			__instance.leftHand.MapMine(__instance.scaleFactor, __instance.playerOffsetTransform);
			__instance.rightHand.MapMine(__instance.scaleFactor, __instance.playerOffsetTransform);
		}
		TorsoPatch.VRRigLateUpdate?.Invoke();
	}
}
