using System;
using HarmonyLib;
using Photon.Pun;

namespace Seralyth.Patches.Safety;

public class IncrementRPCPatches
{
	[HarmonyPatch(typeof(VRRig), "IncrementRPC", new Type[]
	{
		typeof(PhotonMessageInfoWrapped),
		typeof(string)
	})]
	public class NoIncrementRPC
	{
		private static bool Prefix(PhotonMessageInfoWrapped info, string sourceCall)
		{
			return false;
		}
	}

	[HarmonyPatch(typeof(MonkeAgent), "IncrementRPCCall", new Type[]
	{
		typeof(PhotonMessageInfo),
		typeof(string)
	})]
	public class NoIncrementRPCCall
	{
		private static bool Prefix(PhotonMessageInfo info, string callingMethod = "")
		{
			return false;
		}
	}

	[HarmonyPatch(typeof(MonkeAgent), "IncrementRPCCallLocal")]
	public class NoIncrementRPCCallLocal
	{
		private static bool Prefix(PhotonMessageInfoWrapped infoWrapped, string rpcFunction)
		{
			return false;
		}
	}
}
