using System;

namespace Seralyth.Classes.Menu;

public class ButtonInfo
{
	public string buttonText = "-";

	public string overlapText;

	public string[] aliases;

	public string toolTip = "This button doesn't have a tooltip/tutorial.";

	public Action method;

	public Action postMethod;

	public Action enableMethod;

	public Action disableMethod;

	public bool enabled;

	public bool isTogglable = true;

	public bool hideFromArraylist;

	public bool label;

	public bool incremental;

	public bool detected;

	public bool legal;

	public string customBind;

	public string rebindKey;

	public string pcBindKey;

	public bool isSetting;

	public object value;

	public Action onValueChanged;

	public Action<bool> cycleValue;

	public bool excludeFromSave;

	internal bool firstFrame = true;

	public T GetValue<T>()
	{
		if (value == null)
		{
			return default(T);
		}
		Type typeFromHandle = typeof(T);
		try
		{
			if (typeFromHandle.IsEnum)
			{
				return (value is string text) ? ((T)Enum.Parse(typeFromHandle, text, ignoreCase: true)) : ((T)Enum.ToObject(typeFromHandle, value));
			}
			return (T)Convert.ChangeType(value, typeFromHandle);
		}
		catch (Exception)
		{
			throw;
		}
	}

	public void SetValue<T>(T v)
	{
		value = v;
	}

	public void SetEnabled(bool value, bool save = true)
	{
		enabled = value;
	}
}
