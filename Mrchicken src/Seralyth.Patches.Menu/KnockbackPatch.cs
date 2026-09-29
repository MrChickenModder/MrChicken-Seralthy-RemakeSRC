using GorillaLocomotion;
using HarmonyLib;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(GTPlayer), "ApplyKnockback")]
public class KnockbackPatch
{
	public static bool enabled;

	public static bool Prefix(Vector3 direction, float speed)
	{
		return !enabled;
	}
}
