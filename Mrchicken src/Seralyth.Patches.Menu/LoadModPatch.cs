using GorillaTagScripts.VirtualStumpCustomMaps;
using HarmonyLib;
using Modio.Mods;
using Seralyth.Mods.CustomMaps;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(CustomMapManager), "LoadMap")]
public class LoadModPatch
{
	public static void Prefix(ModId modId)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Manager.UpdateCustomMapsTab(ModId.op_Implicit(modId));
	}
}
