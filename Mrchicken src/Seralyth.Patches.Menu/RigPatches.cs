using HarmonyLib;
using UnityEngine;

namespace Seralyth.Patches.Menu;

public class RigPatches
{
	[HarmonyPatch(typeof(VRRig), "OnDisable")]
	public class OnDisable
	{
		public static bool Prefix(VRRig __instance)
		{
			return !__instance.isLocal;
		}
	}

	[HarmonyPatch(typeof(VRRig), "Awake")]
	public class Awake
	{
		public static bool Prefix(VRRig __instance)
		{
			return ((Object)((Component)__instance).gameObject).name != "Local Gorilla Player(Clone)";
		}
	}

	[HarmonyPatch(typeof(VRRig), "PostTick")]
	public class PostTick
	{
		public static bool Prefix(VRRig __instance)
		{
			return !__instance.isLocal || ((Behaviour)__instance).enabled;
		}
	}
}
