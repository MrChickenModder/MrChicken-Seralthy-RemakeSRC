using GorillaNetworking;
using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(PhotonNetworkController), "AttemptToJoinRankedPublicRoom")]
public class RankedPatch
{
	public static bool enabled;

	public static string targetPlatform;

	public static string targetTier;

	public static bool Prefix(GorillaNetworkJoinTrigger triggeredTrigger, JoinType roomJoinType = (JoinType)0)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (enabled)
		{
			((PhotonNetworkController)PhotonNetworkController.Instance).AttemptToJoinRankedPublicRoomAsync(triggeredTrigger, targetTier ?? ((object)RankedProgressionManager.Instance.GetRankedMatchmakingTier()/*cast due to .constrained prefix*/).ToString(), targetPlatform ?? "PC", roomJoinType);
			return false;
		}
		return true;
	}
}
