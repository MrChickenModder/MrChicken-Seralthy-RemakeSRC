using System;
using System.Linq;
using System.Reflection;
using GorillaNetworking;
using HarmonyLib;
using Seralyth.Classes.Menu;
using Seralyth.Managers;
using Seralyth.Menu;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(GorillaComputer), "PressButton")]
public class ComputerPressButtonPatch
{
	public static bool Prefix(GorillaComputer __instance, GorillaKeyboardBindings buttonPressed)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected I4, but got Unknown
		if (!ComputerCategory.InCategory)
		{
			if ((int)buttonPressed == 14)
			{
				ComputerCategory.InCategory = true;
				ComputerCategory._currentCategory = null;
				ComputerCategory.SelectedIndex = 0;
				ComputerCategory.ScrollOffset = 0;
				ComputerCategory.DoCategory(__instance);
				return false;
			}
			return true;
		}
		if ((int)buttonPressed != 0)
		{
			switch (buttonPressed - 10)
			{
			case 0:
			case 5:
			{
				ButtonInfo[] array = ComputerCategory._currentCategory ?? ComputerCategory.GetVisibleButtons(Buttons.buttons[0]);
				if (ComputerCategory.SelectedIndex > 0)
				{
					ComputerCategory.SelectedIndex--;
					if (ComputerCategory.SelectedIndex < ComputerCategory.ScrollOffset)
					{
						ComputerCategory.ScrollOffset = ComputerCategory.SelectedIndex;
					}
				}
				break;
			}
			case 1:
			case 6:
			{
				ButtonInfo[] array2 = ComputerCategory._currentCategory ?? ComputerCategory.GetVisibleButtons(Buttons.buttons[0]);
				if (ComputerCategory.SelectedIndex < array2.Length - 1)
				{
					ComputerCategory.SelectedIndex++;
					if (ComputerCategory.SelectedIndex >= ComputerCategory.ScrollOffset + 6)
					{
						ComputerCategory.ScrollOffset = ComputerCategory.SelectedIndex - 6 + 1;
					}
				}
				break;
			}
			case 3:
			{
				ButtonInfo[] array3 = ComputerCategory._currentCategory ?? ComputerCategory.GetVisibleButtons(Buttons.buttons[0]);
				ButtonInfo buttonInfo = array3[ComputerCategory.SelectedIndex];
				if (Buttons.categoryNames.Contains(buttonInfo.buttonText))
				{
					int category = Buttons.GetCategory(buttonInfo.buttonText);
					if (category >= 0)
					{
						string text = ((Buttons.categoryNames[category] == "Credits") ? "SERALYTHREMAKE" : Buttons.categoryNames[category]);
						ButtonInfo[] all;
						switch (text)
						{
						case "Favorite Mods":
							all = Main.StringsToInfos(Main.favorites.ToArray());
							break;
						case "Enabled Mods":
							all = (from b in Buttons.buttons.SelectMany((ButtonInfo[] x) => x)
								where b.enabled && b.isTogglable
								select b).ToArray();
							break;
						case "Achievements":
							AchievementManager.EnterAchievementTab();
							all = Buttons.buttons[category];
							break;
						case "Friends":
							FriendManager.FriendsListUpdated();
							all = Buttons.buttons[category];
							break;
						default:
							all = Buttons.buttons[category];
							break;
						}
						all = ComputerCategory.GetVisibleButtons(all);
						ComputerCategory._currentCategory = all;
						ComputerCategory._currentCategoryName = text;
						ComputerCategory.SelectedIndex = 0;
						ComputerCategory.ScrollOffset = 0;
						ComputerCategory.DoCategory(__instance);
						break;
					}
				}
				if (buttonInfo.isTogglable)
				{
					Main.Toggle(buttonInfo);
				}
				buttonInfo.method?.Invoke();
				break;
			}
			case 2:
				if (ComputerCategory._currentCategory != null)
				{
					ComputerCategory._currentCategory = null;
					ComputerCategory.SelectedIndex = 0;
					ComputerCategory.ScrollOffset = 0;
					ComputerCategory.DoCategory(__instance);
				}
				else
				{
					ComputerCategory.Reset();
					__instance.UpdateScreen();
				}
				break;
			}
		}
		else
		{
			ComputerCategory.Reset();
			Type typeFromHandle = typeof(GorillaComputer);
			FieldInfo field = typeFromHandle.GetField("currentScreen", BindingFlags.Instance | BindingFlags.NonPublic);
			FieldInfo field2 = typeFromHandle.GetField("supportScreen", BindingFlags.Instance | BindingFlags.NonPublic);
			if (field != null && field2 != null)
			{
				object value = field2.GetValue(__instance);
				if (value != null)
				{
					field.SetValue(__instance, value);
				}
			}
			__instance.UpdateScreen();
		}
		return false;
	}
}
