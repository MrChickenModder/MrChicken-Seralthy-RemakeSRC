using GorillaNetworking;
using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(PhotonNetworkController), "OnJoinedRoom")]
public class JoinedRoomPatch
{
	public static bool enabled;

	private static void Prefix()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (enabled)
		{
			((PhotonNetworkController)PhotonNetworkController.Instance).currentJoinType = (JoinType)6;
		}
	}
}
