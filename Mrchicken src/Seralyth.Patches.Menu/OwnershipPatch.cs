using System.Collections.Generic;
using HarmonyLib;
using Photon.Pun;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(RequestableOwnershipGuard), "OwnershipRequested")]
public class OwnershipPatch
{
	public static bool enabled;

	public static readonly List<RequestableOwnershipGuard> blacklistedGuards = new List<RequestableOwnershipGuard>();

	public static bool Prefix(RequestableOwnershipGuard __instance, string nonce, PhotonMessageInfo info)
	{
		return !enabled || (((MonoBehaviourPun)__instance).photonView.IsMine && !blacklistedGuards.Contains(__instance));
	}
}
