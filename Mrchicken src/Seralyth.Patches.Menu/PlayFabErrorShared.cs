using GorillaNetworking;
using HarmonyLib;
using PlayFab;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(PlayFabAuthenticator), "OnPlayFabError")]
public class PlayFabErrorShared
{
	public static bool Prefix(PlayFabError obj)
	{
		return ErrorPatches.ErrorCall(obj);
	}
}
