using HarmonyLib;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(PropHuntHandFollower), "GeoCollisionPoint")]
public class PropPatch
{
	public static bool enabled;

	public static void Postfix(ref Vector3 __result, Vector3 sourcePos, Vector3 targetPos)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (enabled)
		{
			__result = targetPos;
		}
	}
}
