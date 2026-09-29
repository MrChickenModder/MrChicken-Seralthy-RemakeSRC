using HarmonyLib;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(VRRig), "DroppedByPlayer")]
public class DropPatch
{
	public static bool Prefix(VRRig __instance, VRRig grabbedByRig, Vector3 throwVelocity)
	{
		return !GrabPatch.enabled;
	}
}
