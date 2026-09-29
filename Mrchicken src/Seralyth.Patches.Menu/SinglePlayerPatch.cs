using System.Threading.Tasks;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(NetworkSystemPUN), "InternalDisconnect")]
public class SinglePlayerPatch
{
	public static bool enabled;

	private static bool Prefix(NetworkSystemPUN __instance, ref Task __result)
	{
		if (!enabled)
		{
			return true;
		}
		__instance.internalState = (InternalState)7;
		PhotonNetwork.Disconnect();
		Object.Destroy((Object)(object)__instance.VoiceNetworkObject);
		((NetworkSystem)__instance).UpdatePlayers();
		((NetworkSystem)__instance).SinglePlayerStarted();
		__result = InternalDisconnect(__instance);
		return false;
	}

	private static async Task InternalDisconnect(NetworkSystemPUN instance)
	{
		await instance.WaitForStateCheck((InternalState)8, 10f);
		instance.internalState = (InternalState)6;
	}
}
