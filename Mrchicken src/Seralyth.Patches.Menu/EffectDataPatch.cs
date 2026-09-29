using GorillaLocomotion;
using HarmonyLib;
using Photon.Pun;
using Seralyth.Menu;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(VRRig), "SetHandEffectData")]
public class EffectDataPatch
{
	public static bool enabled;

	public static bool tapsEnabled = true;

	public static bool doOverride;

	public static float overrideVolume = 99999f;

	public static int tapMultiplier = 1;

	public static int material = -1;

	private static bool Prefix(VRRig __instance, HandEffectContext effectContext, int audioClipIndex, bool isDownTap, bool isLeftHand, float handTapVolume, float handTapSpeed, Vector3 dirFromHitToHand)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		if (enabled && __instance.isLocal)
		{
			if (doOverride)
			{
				effectContext.soundFX = VRRig.LocalRig.GetHandSurfaceData(audioClipIndex).audio;
				effectContext.speed = overrideVolume;
				effectContext.soundVolume = overrideVolume;
				if (PhotonNetwork.InRoom && tapMultiplier > 1)
				{
					for (int i = 0; i < tapMultiplier; i++)
					{
						GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", (RpcTarget)0, new object[3] { audioClipIndex, isLeftHand, handTapSpeed });
					}
					Main.RPCProtection();
				}
				return false;
			}
			if (!tapsEnabled)
			{
				effectContext.speed = 0f;
				effectContext.soundVolume = 0f;
				GorillaTagger.Instance.handTapVolume = 0f;
				GorillaTagger.Instance.handTapSpeed = 0f;
				GorillaTagger.Instance.audioClipIndex = -1;
				return false;
			}
			if (material > 0)
			{
				GorillaTagger.Instance.audioClipIndex = material;
				audioClipIndex = material;
				if (isLeftHand)
				{
					GTPlayer.Instance.leftHand.materialTouchIndex = material;
				}
				else
				{
					GTPlayer.Instance.rightHand.materialTouchIndex = material;
				}
				effectContext.soundFX = VRRig.LocalRig.GetHandSurfaceData(material).audio;
				return false;
			}
		}
		return true;
	}
}
