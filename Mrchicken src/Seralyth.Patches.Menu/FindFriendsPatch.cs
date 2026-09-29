using ExitGames.Client.Photon;
using HarmonyLib;
using Photon.Realtime;
using Seralyth.Mods;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(LoadBalancingClient), "OnOperationResponse")]
public static class FindFriendsPatch
{
	public static void Postfix(LoadBalancingClient __instance, OperationResponse operationResponse)
	{
		if (operationResponse.OperationCode == 222)
		{
			Seralyth.Mods.Safety.HandleFindFriendsResponse(operationResponse);
		}
	}
}
