using System;
using GorillaLocomotion;
using GorillaNetworking;
using HarmonyLib;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(GTPlayer), "LateUpdate")]
public class ComputerLateUpdatePatch
{
	public static void Postfix()
	{
		GorillaComputer instance = GorillaComputer.instance;
		if ((Object)(object)instance == (Object)null)
		{
			return;
		}
		string currentText = instance.screenText.currentText;
		if (ComputerCategory.InCategory)
		{
			ComputerCategory.DoCategory(instance);
			return;
		}
		if (currentText.TrimStart().StartsWith("CREDITS"))
		{
			ComputerCategory.InCategory = true;
			ComputerCategory._currentCategory = null;
			ComputerCategory.SelectedIndex = 0;
			ComputerCategory.ScrollOffset = 0;
			ComputerCategory.DoCategory(instance);
			return;
		}
		if (currentText.IndexOf("Credits", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			instance.screenText.Set(currentText.Replace("Credits", "SERALYTH").Replace("CREDITS", "SERALYTH"));
		}
		string currentText2 = instance.functionSelectText.currentText;
		if (currentText2.IndexOf("Credits", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			instance.functionSelectText.Set(currentText2.Replace("Credits", "SERALYTH").Replace("CREDITS", "SERALYTH"));
		}
	}
}
