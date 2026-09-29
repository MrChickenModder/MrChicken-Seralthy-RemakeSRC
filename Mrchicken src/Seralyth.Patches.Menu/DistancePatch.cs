using HarmonyLib;
using Seralyth.Menu;
using Seralyth.Utilities;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(VRRig), "IsPositionInRange")]
public class DistancePatch
{
	public static bool enabled;

	public static void Postfix(VRRig __instance, ref bool __result, Vector3 position, float range)
	{
		NetPlayer val = RigUtilities.GetPlayerFromVRRig(__instance) ?? null;
		if ((enabled && __instance.isLocal) || (val != null && Main.ShouldBypassChecks(val)))
		{
			__result = true;
		}
	}
}
