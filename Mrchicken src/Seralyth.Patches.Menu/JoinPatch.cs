using System.Collections.Generic;
using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(GameEntityManager), "JoinWithItems")]
public class JoinPatch
{
	public static bool enabled;

	public static bool Prefix(List<GameEntity> entities)
	{
		return !enabled;
	}
}
