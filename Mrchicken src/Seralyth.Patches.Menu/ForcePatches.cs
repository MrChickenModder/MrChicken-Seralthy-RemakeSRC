using HarmonyLib;

namespace Seralyth.Patches.Menu;

public class ForcePatches
{
	[HarmonyPatch(typeof(ForceVolume), "OnTriggerEnter")]
	public class OnTriggerEnter
	{
		public static bool Prefix()
		{
			return !enabled;
		}
	}

	[HarmonyPatch(typeof(ForceVolume), "OnTriggerExit")]
	public class OnTriggerExit
	{
		public static bool Prefix()
		{
			return !enabled;
		}
	}

	[HarmonyPatch(typeof(ForceVolume), "OnTriggerStay")]
	public class OnTriggerStay
	{
		public static bool Prefix()
		{
			return !enabled;
		}
	}

	public static bool enabled;
}
