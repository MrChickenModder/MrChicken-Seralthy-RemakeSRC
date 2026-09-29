using GorillaNetworking;
using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(GorillaNetworkJoinTrigger), "OnBoxTriggered")]
public class NetworkTriggerPatch
{
	public static bool enabled;

	public static bool Prefix()
	{
		return !enabled;
	}
}
