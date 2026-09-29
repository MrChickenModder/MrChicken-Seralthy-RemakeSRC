using HarmonyLib;
using Seralyth.Menu;
using Seralyth.Utilities;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(VRRig), "InitializeNoobMaterial")]
public class InitializeNoobMaterial
{
	public static bool Prefix(VRRig __instance, float red, float green, float blue, PhotonMessageInfoWrapped info)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		NetPlayer val = RigUtilities.GetPlayerFromVRRig(__instance) ?? null;
		if (val != null && Main.ShouldBypassChecks(val))
		{
			if (info.senderID == NetworkSystem.Instance.GetOwningPlayerID(((Component)__instance.rigSerializer).gameObject))
			{
				__instance.InitializeNoobMaterialLocal(red, green, blue);
			}
			return false;
		}
		return true;
	}
}
