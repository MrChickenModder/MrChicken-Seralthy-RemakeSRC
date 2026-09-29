using HarmonyLib;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(GuardianRPCs), "GuardianLaunchPlayer")]
public class LaunchPatch
{
	public static bool Prefix(Vector3 velocity)
	{
		return !GrabPatch.enabled;
	}
}
