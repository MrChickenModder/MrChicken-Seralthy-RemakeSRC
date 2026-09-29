using HarmonyLib;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(VRRig), "PackCompetitiveData")]
public class FPSPatch
{
	public static bool enabled;

	public static int spoofFPSValue;

	public static void Postfix(ref short __result)
	{
		if (!enabled)
		{
			return;
		}
		GorillaSnapTurn gorillaSnapTurningComp = VRRig.LocalRig.GorillaSnapTurningComp;
		int num = 0;
		if ((Object)(object)gorillaSnapTurningComp != (Object)null)
		{
			VRRig.LocalRig.turnFactor = gorillaSnapTurningComp.turnFactor;
			VRRig.LocalRig.turnType = gorillaSnapTurningComp.turnType;
			if (!(gorillaSnapTurningComp.turnType == "SNAP"))
			{
				if (gorillaSnapTurningComp.turnType == "SMOOTH")
				{
					num = 2;
				}
			}
			else
			{
				num = 1;
			}
			num *= 10;
			num += gorillaSnapTurningComp.turnFactor;
		}
		__result = (short)(spoofFPSValue + (num << 8));
	}
}
