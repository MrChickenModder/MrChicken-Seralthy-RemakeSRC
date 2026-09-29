using System;
using System.Collections;
using System.Linq;
using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;
using Seralyth.Managers;
using Seralyth.Menu;
using Seralyth.Mods;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(VRRig), "RequestCosmetics")]
public class RequestPatch
{
	public static bool enabled;

	public static bool bypassCosmeticCheck;

	public static Coroutine currentCoroutine;

	private static string[] archiveCosmetics;

	public static bool Prefix(VRRig __instance, PhotonMessageInfoWrapped info)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Expected O, but got Unknown
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		if (__instance.netView.IsMine && __instance.isLocal && CosmeticsController.hasInstance)
		{
			if (((CosmeticsController)CosmeticsController.instance).isHidingCosmeticsFromRemotePlayers)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_HideAllCosmetics", info.Sender, Array.Empty<object>());
				return false;
			}
			if (enabled)
			{
				if (currentCoroutine == null)
				{
					currentCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(LoadCosmetics());
				}
				return false;
			}
			if (bypassCosmeticCheck)
			{
				CosmeticSet val = new CosmeticSet((from cosmetic in ((CosmeticsController)CosmeticsController.instance).currentWornSet.ToDisplayNameArray()
					select Main.CosmeticsOwned.Contains(cosmetic) ? cosmetic : "null").ToArray(), CosmeticsController.instance);
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", NetworkSystem.Instance.GetPlayer(info.senderID), new object[3]
				{
					val.ToPackedIDArray(),
					((CosmeticsController)CosmeticsController.instance).tryOnSet.ToPackedIDArray(),
					false
				});
				return false;
			}
		}
		return true;
	}

	public static IEnumerator LoadCosmetics()
	{
		if (PhotonNetwork.InRoom)
		{
			Vector3 target = Main.TryOnRoom.transform.position;
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = target;
			string[] cosmeticArray = new string[16]
			{
				"LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.",
				"LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU."
			};
			archiveCosmetics = ((CosmeticsController)CosmeticsController.instance).currentWornSet.ToDisplayNameArray();
			((CosmeticsController)CosmeticsController.instance).currentWornSet = new CosmeticSet(cosmeticArray, CosmeticsController.instance);
			while (Vector3.Distance(Main.ServerPos, target) > 0.2f)
			{
				yield return null;
			}
			yield return (object)new WaitForSeconds(0.1f);
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", (RpcTarget)1, new object[3]
			{
				Fun.PackCosmetics(cosmeticArray),
				((CosmeticsController)CosmeticsController.instance).currentWornSet.ToPackedIDArray(),
				false
			});
			((Behaviour)VRRig.LocalRig).enabled = true;
			yield return (object)new WaitForSeconds(0.5f);
			((CosmeticsController)CosmeticsController.instance).currentWornSet = new CosmeticSet(archiveCosmetics, CosmeticsController.instance);
			VRRig.LocalRig.LocalUpdateCosmeticsWithTryon(((CosmeticsController)CosmeticsController.instance).currentWornSet, ((CosmeticsController)CosmeticsController.instance).tryOnSet, false);
			float delay = Time.time + 30f;
			while (Time.time < delay || PhotonNetwork.InRoom)
			{
				yield return null;
			}
			currentCoroutine = null;
		}
	}
}
