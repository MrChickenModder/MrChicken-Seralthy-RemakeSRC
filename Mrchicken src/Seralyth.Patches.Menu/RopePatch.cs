using GorillaLocomotion.Gameplay;
using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(GorillaRopeSwing), "AttachLocalPlayer")]
public class RopePatch
{
	public static bool enabled;

	public static float amplifier = 5f;

	public static void Prefix(XRNode xrNode, Transform grabbedBone, Vector3 offset, ref Vector3 velocity)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (enabled)
		{
			velocity *= amplifier;
		}
	}
}
