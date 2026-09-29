using GorillaTag.Cosmetics;
using HarmonyLib;
using Photon.Pun;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(DreidelHoldable), "OnActivate")]
public class DreidelPatch
{
	public static bool enabled;

	public static double? time;

	public static void Prefix()
	{
		if (enabled)
		{
			time = PhotonNetwork.frametime;
			PhotonNetwork.frametime = double.MaxValue;
		}
	}

	public static void Postfix()
	{
		if (enabled)
		{
			PhotonNetwork.frametime = time ?? ((double)PhotonNetwork.ServerTimestamp / 1000.0);
		}
		time = null;
	}
}
