using GorillaLocomotion;
using HarmonyLib;
using Seralyth.Extensions;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(TakeMyHand_HandLink), "OnRelease")]
public class ReleasePatch
{
	public static bool enabled;

	public static bool Prefix(TakeMyHand_HandLink __instance, bool __result, DropZone zoneReleased, GameObject releasingHand)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Invalid comparison between Unknown and I4
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Invalid comparison between Unknown and I4
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		if (enabled)
		{
			if (!__instance.myRig.isOfflineVRRig)
			{
				bool flag = false;
				TakeMyHand_HandLink val = (((Object)(object)releasingHand == (Object)(object)((EquipmentInteractor)EquipmentInteractor.instance).leftHand) ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink);
				HandLinkAuthorityStatus val2 = GTPlayer.Instance.TakeMyHand_GetSelfHandLinkAuthority();
				int num = default(int);
				HandLinkAuthorityStatus chainAuthority = val.GetChainAuthority(ref num);
				if ((int)val2.type >= 1 && chainAuthority.type < val2.type)
				{
					flag = true;
				}
				else if ((Object)(object)val.myOtherHandLink.grabbedLink != (Object)null)
				{
					HandLinkAuthorityStatus chainAuthority2 = val.myOtherHandLink.GetChainAuthority(ref num);
					if ((int)chainAuthority2.type >= 1 && chainAuthority.type < chainAuthority2.type)
					{
						flag = true;
					}
				}
				if (flag)
				{
					Vector3 averageVelocity = (val.isLeftHand ? GTPlayer.Instance.LeftHand.velocityTracker : GTPlayer.Instance.RightHand.velocityTracker).GetAverageVelocity(true, 0.15f, false);
					Vector3 val3 = ((Vector3)(ref averageVelocity)).normalized * 20f;
					__instance.myRig.GetNetView().SendRPC("DroppedByPlayer", __instance.myRig.GetPlayer(), new object[1] { val3 });
					__instance.myRig.ApplyLocalTrajectoryOverride(val3);
				}
				val.BreakLink();
			}
			return false;
		}
		return true;
	}
}
