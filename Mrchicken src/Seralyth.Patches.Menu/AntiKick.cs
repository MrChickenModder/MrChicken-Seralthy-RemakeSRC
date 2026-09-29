using System.Linq;
using HarmonyLib;
using Photon.Pun;
using Seralyth.Extensions;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(PhotonNetwork), "OnEvent")]
public class AntiKick
{
	public static bool enabled;

	public static bool Prefix()
	{
		if (enabled && VRRigCache.ActiveRigs.All((VRRig rig) => (Object)(object)rig != (Object)null && rig.GetPing() > 500))
		{
			return false;
		}
		return true;
	}
}
