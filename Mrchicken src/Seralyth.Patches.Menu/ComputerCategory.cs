using System.Linq;
using System.Text;
using GorillaNetworking;
using Seralyth.Classes.Menu;
using Seralyth.Menu;
using UnityEngine;

namespace Seralyth.Patches.Menu;

public static class ComputerCategory
{
	public static bool InCategory;

	public static int SelectedIndex;

	public static int ScrollOffset;

	public static ButtonInfo[] _currentCategory;

	public static string _currentCategoryName;

	public const int VISIBLE_MODS = 6;

	internal static ButtonInfo[] GetVisibleButtons(ButtonInfo[] all)
	{
		return all;
	}

	public static void DoCategory(GorillaComputer instance)
	{
		if (_currentCategory == null)
		{
			ShowCategoryList(instance);
		}
		else
		{
			ShowModList(instance);
		}
	}

	private static void ShowCategoryList(GorillaComputer instance)
	{
		ButtonInfo[] visibleButtons = GetVisibleButtons(Buttons.buttons[0]);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine($"SERALYTH  ({SelectedIndex + 1}/{visibleButtons.Length})");
		stringBuilder.AppendLine();
		int num = Mathf.Min(ScrollOffset + 6, visibleButtons.Length);
		for (int i = ScrollOffset; i < num; i++)
		{
			ButtonInfo buttonInfo = visibleButtons[i];
			string text = ((i == SelectedIndex) ? "> " : "  ");
			stringBuilder.AppendLine(text + Truncate(buttonInfo.buttonText, 22) + "  [" + GetStatus(buttonInfo) + "]");
		}
		stringBuilder.AppendLine();
		stringBuilder.Append("OPT2 UP | OPT3 DOWN | ENT Select | DEL Exit");
		instance.screenText.Set(stringBuilder.ToString());
	}

	private static void ShowModList(GorillaComputer instance)
	{
		string text = _currentCategoryName ?? GetCategoryName(_currentCategory);
		if (text == "Credits")
		{
			text = "SERALYTHREMAKE";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine($"{text}  ({SelectedIndex + 1}/{_currentCategory.Length})");
		stringBuilder.AppendLine();
		int num = Mathf.Min(ScrollOffset + 6, _currentCategory.Length);
		for (int i = ScrollOffset; i < num; i++)
		{
			ButtonInfo buttonInfo = _currentCategory[i];
			string text2 = ((i == SelectedIndex) ? "> " : "  ");
			string text3 = (buttonInfo.isTogglable ? (buttonInfo.enabled ? "ON" : "OFF") : "");
			stringBuilder.AppendLine(text2 + Truncate(buttonInfo.buttonText, 22) + " " + (buttonInfo.isTogglable ? ("[" + text3 + "]") : ""));
		}
		stringBuilder.AppendLine();
		stringBuilder.Append("OPT2 UP | OPT3 DOWN | ENT Toggle | DEL Back");
		instance.screenText.Set(stringBuilder.ToString());
	}

	private static string GetStatus(ButtonInfo entry)
	{
		if (entry.isTogglable)
		{
			return entry.enabled ? "ON" : "OFF";
		}
		if (Buttons.categoryNames.Contains(entry.buttonText))
		{
			int category = Buttons.GetCategory(entry.buttonText);
			if (category >= 0 && category < Buttons.buttons.Length && GetVisibleButtons(Buttons.buttons[category]).Any((ButtonInfo b) => b.isTogglable && b.enabled))
			{
				return "ON";
			}
		}
		return "OFF";
	}

	private static string GetCategoryName(ButtonInfo[] cat)
	{
		for (int i = 0; i < Buttons.buttons.Length; i++)
		{
			if (Buttons.buttons[i] == cat)
			{
				return Buttons.categoryNames[i];
			}
		}
		return "Unknown";
	}

	private static string Truncate(string text, int maxLength)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		return (text.Length <= maxLength) ? text : (text.Substring(0, maxLength - 2) + ".");
	}

	public static void Reset()
	{
		InCategory = false;
		SelectedIndex = 0;
		ScrollOffset = 0;
		_currentCategory = null;
		_currentCategoryName = null;
	}
}
