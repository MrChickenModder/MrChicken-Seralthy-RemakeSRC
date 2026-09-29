using System.Collections.Generic;
using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(GorillaTagManager), "ReportTag")]
public class ReportTagPatch
{
	public static readonly List<NetPlayer> blacklistedPlayers = new List<NetPlayer>();

	public static readonly List<NetPlayer> invinciblePlayers = new List<NetPlayer>();

	public static bool Prefix(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		return !blacklistedPlayers.Contains(taggingPlayer) && !invinciblePlayers.Contains(taggedPlayer);
	}
}
