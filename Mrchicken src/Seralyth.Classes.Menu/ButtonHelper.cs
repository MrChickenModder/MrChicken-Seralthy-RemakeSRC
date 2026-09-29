using System;
using UnityEngine;

namespace Seralyth.Classes.Menu;

public static class ButtonHelper
{
	public static int Wrap(int current, int min, int max, bool positive)
	{
		int num = (positive ? (current + 1) : (current - 1));
		if (num > max)
		{
			num = min;
		}
		if (num < min)
		{
			num = max;
		}
		return num;
	}

	public static ButtonInfo Create(string buttonText, Func<string[]> getNames, string defaultName, Action<int> apply, string toolTip = null, bool legal = false, string overlapText = null, Action<bool> onCycle = null)
	{
		string label = overlapText ?? buttonText;
		ButtonInfo button = new ButtonInfo
		{
			buttonText = buttonText,
			isTogglable = false,
			isSetting = true,
			incremental = true,
			value = defaultName,
			toolTip = toolTip,
			legal = legal
		};
		if (!string.IsNullOrEmpty(overlapText))
		{
			button.overlapText = overlapText;
		}
		button.onValueChanged = delegate
		{
			string[] array = getNames();
			if (array == null || array.Length == 0)
			{
				button.overlapText = label + " <color=grey>[</color><color=green>Loading..</color><color=grey>]</color>";
			}
			else
			{
				int num = CurrentIndex(array);
				button.value = array[num];
				button.overlapText = label + " <color=grey>[</color><color=green>" + array[num] + "</color><color=grey>]</color>";
				apply(num);
			}
		};
		button.cycleValue = delegate(bool positive)
		{
			string[] array = getNames();
			if (array != null && array.Length != 0)
			{
				int current = CurrentIndex(array);
				int num = Wrap(current, 0, array.Length - 1, positive);
				button.value = array[num];
				button.onValueChanged();
				onCycle?.Invoke(positive);
			}
		};
		return button;
		int CurrentIndex(string[] names)
		{
			if (names == null || names.Length == 0)
			{
				return -1;
			}
			if (button.value is string value)
			{
				int num = Array.IndexOf(names, value);
				return (num >= 0) ? num : 0;
			}
			if (button.value is int num2 && num2 >= 0 && num2 < names.Length)
			{
				return num2;
			}
			return 0;
		}
	}

	public static ButtonInfo Create(string buttonText, Func<string[]> getNames, int defaultIndex, Action<int> apply, string toolTip = null, bool legal = false, string overlapText = null, Action<bool> onCycle = null)
	{
		string[] array = getNames();
		string defaultName = ((array != null && defaultIndex >= 0 && defaultIndex < array.Length) ? array[defaultIndex] : null);
		return Create(buttonText, getNames, defaultName, apply, toolTip, legal, overlapText, onCycle);
	}

	public static ButtonInfo CreateNumeric(string buttonText, int min, int max, int defaultValue, Action<int> apply, Func<int, string> display = null, string toolTip = null, bool legal = false, string overlapText = null, Action<bool> onCycle = null, Func<int> getStep = null, bool persist = true)
	{
		string label = overlapText ?? buttonText;
		if (getStep == null)
		{
			getStep = () => 1;
		}
		ButtonInfo button = new ButtonInfo
		{
			buttonText = buttonText,
			isTogglable = false,
			isSetting = persist,
			incremental = true,
			value = defaultValue,
			toolTip = toolTip,
			legal = legal,
			excludeFromSave = !persist
		};
		if (!string.IsNullOrEmpty(overlapText))
		{
			button.overlapText = overlapText;
		}
		button.onValueChanged = delegate
		{
			int value = button.GetValue<int>();
			if (display != null)
			{
				button.overlapText = label + " <color=grey>[</color><color=green>" + display(value) + "</color><color=grey>]</color>";
			}
			apply(value);
		};
		button.cycleValue = delegate(bool positive)
		{
			int num = Mathf.Max(1, getStep());
			int num2 = (positive ? (button.GetValue<int>() + num) : (button.GetValue<int>() - num));
			int num3 = max - min + 1;
			int num4 = (num2 - min) % num3;
			if (num4 < 0)
			{
				num4 += num3;
			}
			button.value = min + num4;
			button.onValueChanged();
			onCycle?.Invoke(positive);
		};
		return button;
	}
}
