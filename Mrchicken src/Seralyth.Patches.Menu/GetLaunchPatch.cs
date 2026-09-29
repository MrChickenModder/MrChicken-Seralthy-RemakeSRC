using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Seralyth.Menu;
using Seralyth.Utilities;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(Slingshot), "GetLaunchVelocity")]
public class GetLaunchPatch
{
	public static bool enabled;

	public static void Postfix(Slingshot __instance, ref Vector3 __result)
	{
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		if (!enabled)
		{
			return;
		}
		if (!(((TransferrableObject)__instance).InLeftHand() ? (Main.leftTrigger > 0.5f) : (Main.rightTrigger > 0.5f)))
		{
			List<NetPlayer> infected = GameModeUtilities.InfectedList();
			List<VRRig> source = (from rig in VRRigCache.ActiveRigs
				where !rig.isLocal
				where !infected.Contains(RigUtilities.GetPlayerFromVRRig(rig))
				select rig).ToList();
			Transform head = ((Component)GorillaTagger.Instance.headCollider).transform;
			VRRig val = (from x in source.Where((VRRig rig) => (Object)(object)rig != (Object)null).Select(delegate(VRRig rig)
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Unknown result type (might be due to invalid IL or missing references)
					//IL_001c: Unknown result type (might be due to invalid IL or missing references)
					//IL_001f: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0035: Unknown result type (might be due to invalid IL or missing references)
					Vector3 val2 = ((Component)rig).transform.position - head.position;
					return new
					{
						Rig = rig,
						ToRig = ((Vector3)(ref val2)).normalized,
						Distance = Vector3.Distance(head.position, ((Component)rig).transform.position)
					};
				})
				orderby Vector3.Angle(head.forward, x.ToRig) + x.Distance * 0.1f
				select x.Rig).FirstOrDefault();
			if (!((Object)(object)val == (Object)null))
			{
				__result = CalcMinSpeed(((Component)__instance.center).transform.position, val);
			}
		}
	}

	private static Vector3 CalcMinSpeed(Vector3 origin, VRRig targetRig)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = targetRig.headMesh.transform.position;
		Vector3 val = targetRig.LatestVelocity();
		val.y /= 3f;
		Vector3 val2 = position - origin;
		Vector3 val3 = default(Vector3);
		((Vector3)(ref val3))._002Ector(val2.x, 0f, val2.z);
		float num = 0f - Physics.gravity.y;
		float magnitude = ((Vector3)(ref val3)).magnitude;
		float num2 = 20f;
		float num3 = magnitude / num2;
		Vector3 val4 = position + val * num3;
		val2 = val4 - origin;
		((Vector3)(ref val3))._002Ector(val2.x, 0f, val2.z);
		float y = val2.y;
		magnitude = ((Vector3)(ref val3)).magnitude;
		float num4 = Mathf.Sqrt(num * (y + Mathf.Sqrt(magnitude * magnitude + y * y)));
		float speed = num4 * 2.5f;
		return CalcVelocity(val2, speed);
	}

	private static Vector3 CalcVelocity(Vector3 displacement, float speed)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(displacement.x, 0f, displacement.z);
		float magnitude = ((Vector3)(ref val)).magnitude;
		float y = displacement.y;
		float num = 0f - Physics.gravity.y;
		float num2 = speed * speed;
		float num3 = num2 * num2 - num * (num * magnitude * magnitude + 2f * y * num2);
		if (num3 <= 0f)
		{
			return ((Vector3)(ref displacement)).normalized * speed;
		}
		float num4 = Mathf.Sqrt(num3);
		float num5 = Mathf.Atan((num2 - num4) / (num * magnitude));
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		return normalized * Mathf.Cos(num5) * speed + Vector3.up * Mathf.Sin(num5) * speed;
	}
}
