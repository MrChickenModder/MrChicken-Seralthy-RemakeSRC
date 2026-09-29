using System;
using System.Collections.Generic;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using Seralyth.Managers;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(PhotonNetwork), "RPC", new Type[]
{
	typeof(PhotonView),
	typeof(string),
	typeof(RpcTarget),
	typeof(Player),
	typeof(bool),
	typeof(object[])
})]
public class RPCFilter
{
	public static Dictionary<string, Func<bool>> FilteredRPCs = new Dictionary<string, Func<bool>>();

	public static bool Prefix(PhotonView view, string methodName, RpcTarget target, Player player, bool encrypt, params object[] parameters)
	{
		if (FilteredRPCs.Count <= 0)
		{
			return true;
		}
		try
		{
			if (FilteredRPCs.TryGetValue(methodName, out var value))
			{
				return value?.Invoke() ?? true;
			}
		}
		catch (Exception arg)
		{
			LogManager.LogError($"Error in RPCFilter.FilteredRPCs.{methodName}: {arg}");
		}
		return true;
	}
}
