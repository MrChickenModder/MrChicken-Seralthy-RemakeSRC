using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Seralyth.Extensions;
using Seralyth.Utilities;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(SIGadgetChargeBlaster), "FireProjectile")]
public class FirePatch
{
	public static bool enabled;

	public static void Prefix(SIGadgetChargeBlaster __instance, float firedAtChargeLevel, int fireId, Vector3 position, Quaternion rotation)
	{
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		if (enabled && __instance.blaster.LocalEquippedOrActivated)
		{
			List<NetPlayer> infected = GameModeUtilities.InfectedList();
			List<VRRig> source = (from rig in VRRigCache.ActiveRigs
				where !rig.isLocal
				where !infected.Contains(rig.GetPlayer())
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
					Vector3 val3 = ((Component)rig).transform.position - head.position;
					return new
					{
						Rig = rig,
						ToRig = ((Vector3)(ref val3)).normalized,
						Distance = Vector3.Distance(head.position, ((Component)rig).transform.position)
					};
				})
				orderby Vector3.Angle(head.forward, x.ToRig) + x.Distance * 0.1f
				select x.Rig).FirstOrDefault();
			Vector3 val2 = val.headMesh.transform.position - position;
			rotation = Quaternion.LookRotation(((Vector3)(ref val2)).normalized);
		}
	}
}
