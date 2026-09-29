using GorillaNetworking;
using HarmonyLib;
using PlayFab;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(GorillaComputer), "OnErrorShared")]
public class ComputerErrorShared
{
	public static bool Prefix(PlayFabError error)
	{
		return ErrorPatches.ErrorCall(error);
	}
}
