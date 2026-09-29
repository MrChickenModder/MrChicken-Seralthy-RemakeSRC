using System;
using HarmonyLib;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(VRRigSerializer), "OnHandTapRPCShared")]
public class HandTapPatch
{
	public static Action<VRRig, Vector3> OnHandTap;

	public static bool enabled;

	public static bool Prefix(VRRigSerializer __instance, int audioClipIndex, bool isDownTap, bool isLeftHand, float handTapSpeed, long packedDirFromHitToHand, PhotonMessageInfoWrapped info)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		OnHandTap?.Invoke(__instance.vrrig, isLeftHand ? __instance.vrrig.leftHandTransform.position : __instance.vrrig.rightHandTransform.position);
		return !enabled;
	}
}
