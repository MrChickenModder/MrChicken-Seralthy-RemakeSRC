using System;
using GorillaNetworking;
using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(GorillaComputer), "UpdateScreen")]
public class ComputerUpdateScreenPatch
{
	public static void Postfix(GorillaComputer __instance)
	{
		string currentText = __instance.screenText.currentText;
		if (currentText.TrimStart().StartsWith("CREDITS"))
		{
			ComputerCategory.InCategory = true;
			ComputerCategory._currentCategory = null;
			ComputerCategory.SelectedIndex = 0;
			ComputerCategory.ScrollOffset = 0;
			ComputerCategory.DoCategory(__instance);
			return;
		}
		if (ComputerCategory.InCategory)
		{
			ComputerCategory.DoCategory(__instance);
			return;
		}
		if (currentText.IndexOf("Credits", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			__instance.screenText.Set(currentText.Replace("Credits", "SERALYTHREMAKE").Replace("CREDITS", "SERALYTHREMAKE"));
		}
		string currentText2 = __instance.functionSelectText.currentText;
		if (currentText2.IndexOf("Credits", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			__instance.functionSelectText.Set(currentText2.Replace("Credits", "SERALYTHREMAKE").Replace("CREDITS", "SERALYTHREMAKE"));
		}
	}
}
