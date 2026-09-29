using System.Collections.Generic;
using HarmonyLib;
using Seralyth.Mods;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(RankedProgressionManager), "SetLocalProgressionData")]
public class SetRankedPatch
{
	public static bool enabled;

	public static bool Prefix(RankedProgressionManager __instance, RankedModePlayerProgressionData data)
	{
		if (enabled)
		{
			Dictionary<int, int[]> dictionary = new Dictionary<int, int[]>();
			dictionary.Add(0, new int[2]);
			dictionary.Add(1, new int[2] { 0, 1 });
			dictionary.Add(2, new int[2] { 1, 0 });
			dictionary.Add(3, new int[2] { 1, 1 });
			dictionary.Add(4, new int[2] { 1, 2 });
			dictionary.Add(5, new int[2] { 2, 0 });
			dictionary.Add(6, new int[2] { 2, 1 });
			dictionary.Add(7, new int[2] { 2, 2 });
			Dictionary<int, int[]> dictionary2 = dictionary;
			RankedModeProgressionPlatformData[] platformData = data.platformData;
			foreach (RankedModeProgressionPlatformData val in platformData)
			{
				val.elo = Seralyth.Mods.Safety.targetElo;
				val.majorTier = dictionary2[Seralyth.Mods.Safety.targetBadge][0];
				val.minorTier = dictionary2[Seralyth.Mods.Safety.targetBadge][1];
			}
			__instance.ProgressionData = data;
			return false;
		}
		return true;
	}
}
