using System;
using HarmonyLib;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(SlingshotProjectile), "OnCollisionEnter")]
public class CollisionPatch
{
	public static event Action<SlingshotProjectile, Collision> OnCollisionEnterEvent;

	private static void Prefix(SlingshotProjectile __instance, Collision collision)
	{
		if ((Object)(object)__instance != (Object)null && !__instance.dontDestroyOnHit && __instance.particleLaunched)
		{
			CollisionPatch.OnCollisionEnterEvent?.Invoke(__instance, collision);
		}
	}
}
