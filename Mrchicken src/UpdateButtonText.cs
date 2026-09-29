using System.Collections.Generic;
using Seralyth.Classes.Menu;
using Seralyth.Extensions;
using Seralyth.Menu;
using Seralyth.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateButtonText : MonoBehaviour
{
	public ButtonInfo button;

	public int buttonIndex;

	public float offset;

	private TextMeshPro tmp;

	private UIColorChanger colorChanger;

	private bool initialized;

	private string lastRendered;

	public void Init(ButtonInfo b, int idx, float off)
	{
		button = b;
		buttonIndex = idx;
		offset = off;
		EnsureReferences();
		ApplyLayout();
		initialized = true;
	}

	private void EnsureReferences()
	{
		if ((Object)(object)tmp == (Object)null)
		{
			tmp = ((Component)this).GetComponent<TextMeshPro>();
		}
		if ((Object)(object)colorChanger == (Object)null)
		{
			colorChanger = ((Component)this).GetComponent<UIColorChanger>() ?? ((Component)this).gameObject.AddComponent<UIColorChanger>();
		}
	}

	private void ApplyLayout()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		RectTransform rectTransform = ((TMP_Text)tmp).rectTransform;
		((Transform)rectTransform).localPosition = Vector3.zero;
		rectTransform.sizeDelta = new Vector2((button != null && button.incremental && Main.incrementalButtons) ? 0.18f : 0.2f, 0.03f * (Main.ButtonDistance / 0.1f));
		if (Main.NoAutoSizeText)
		{
			rectTransform.sizeDelta = new Vector2(9f, 0.015f);
		}
		if (Main.hideTextOnCamera)
		{
			((Component)rectTransform).gameObject.layer = 19;
		}
		((Transform)rectTransform).localPosition = new Vector3(0.064f, 0f, 0.111f - offset / 2.6f);
		((Transform)rectTransform).localRotation = Quaternion.Euler(180f, 90f, 90f);
		((TMP_Text)tmp).font = Main.activeFont;
		((TMP_Text)tmp).spriteAsset = Main.ButtonSpriteSheet;
		((TMP_Text)tmp).richText = true;
		((TMP_Text)tmp).fontSize = 1f;
		((TMP_Text)tmp).alignment = (TextAlignmentOptions)(Main.checkMode ? 513 : 514);
		((TMP_Text)tmp).fontStyle = Main.activeFontStyle;
		((TMP_Text)tmp).enableAutoSizing = true;
		((TMP_Text)tmp).fontSizeMin = 0f;
	}

	public void UpdateText()
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (initialized && button != null)
		{
			EnsureReferences();
			ApplyLayout();
			TextMeshProExtensions.SafeSetText(text: lastRendered = ButtonText(), tmp: (TMP_Text)(object)tmp);
			Main.FollowMenuSettings((TMP_Text)(object)tmp);
			if (Main.joystickMenu && buttonIndex == Main.joystickButtonSelected && Main.themeType == 30)
			{
				((Graphic)tmp).color = Color.red;
			}
			else
			{
				colorChanger.colors = Main.textColors[(!button.enabled) ? 1 : 2];
			}
		}
	}

	private void LateUpdate()
	{
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		if (initialized && button != null && !((Object)(object)tmp == (Object)null))
		{
			EnsureReferences();
			string text = ButtonText();
			if (lastRendered != text || string.IsNullOrEmpty(((TMP_Text)tmp).text))
			{
				lastRendered = text;
				((TMP_Text)(object)tmp).SafeSetText(text);
				Main.FollowMenuSettings((TMP_Text)(object)tmp);
			}
			if (Main.joystickMenu && buttonIndex == Main.joystickButtonSelected && Main.themeType == 30)
			{
				((Graphic)tmp).color = Color.red;
			}
			else
			{
				colorChanger.colors = Main.textColors[(!button.enabled) ? 1 : 2];
			}
		}
	}

	private string ButtonText()
	{
		string text = button.overlapText ?? button.buttonText;
		if (button.detected)
		{
			text = "<color=red>" + text + "</color>";
		}
		if (Main.adaptiveButtons)
		{
			switch (ControllerUtilities.GetLeftControllerType())
			{
			case ControllerUtilities.ControllerType.ValveIndex:
				foreach (KeyValuePair<string, string> item in new Dictionary<string, string>
				{
					{ "x", "la" },
					{ "y", "lb" }
				})
				{
					text = text.Replace("<color=green>" + item.Key.ToUpper() + "</color>", "<color=green>" + item.Value.ToUpper() + "</color>");
				}
				break;
			case ControllerUtilities.ControllerType.VIVE:
				foreach (KeyValuePair<string, string> item2 in new Dictionary<string, string> { { "x", "ltp" } })
				{
					text = text.Replace("<color=green>" + item2.Key.ToUpper() + "</color>", "<color=green>" + item2.Value.ToUpper() + "</color>");
				}
				break;
			}
			switch (ControllerUtilities.GetRightControllerType())
			{
			case ControllerUtilities.ControllerType.ValveIndex:
				foreach (KeyValuePair<string, string> item3 in new Dictionary<string, string>
				{
					{ "a", "ra" },
					{ "b", "rb" }
				})
				{
					text = text.Replace("<color=green>" + item3.Key.ToUpper() + "</color>", "<color=green>" + item3.Value.ToUpper() + "</color>");
				}
				break;
			case ControllerUtilities.ControllerType.VIVE:
				foreach (KeyValuePair<string, string> item4 in new Dictionary<string, string> { { "a", "rtp" } })
				{
					text = text.Replace("<color=green>" + item4.Key.ToUpper() + "</color>", "<color=green>" + item4.Value.ToUpper() + "</color>");
				}
				break;
			}
		}
		if (button.rebindKey != null && text.Contains("</color><color=grey>]</color>"))
		{
			text = text.Split("<color=grey>[</color><color=green>")[0] + "<color=grey>[</color><color=green>" + button.rebindKey + "</color><color=grey>]</color>";
		}
		if (button.customBind != null)
		{
			text = ((!text.Contains("</color><color=grey>]</color>")) ? (text + " <color=grey>[</color><color=green>" + button.customBind + "</color><color=grey>]</color>") : text.Replace("</color><color=grey>]</color>", "/" + button.customBind + "</color><color=grey>]</color>"));
		}
		if (Main.inputTextColor != "green")
		{
			text = text.Replace(" <color=grey>[</color><color=green>", " <color=grey>[</color><color=" + Main.inputTextColor + ">");
		}
		text = Main.FixTMProTags(text);
		text = Main.FollowMenuSettings(text);
		if (Main.favorites.Contains(button.buttonText))
		{
			text = "    " + text + "    <sprite name=\"Favorite\">";
		}
		if (button.customBind != null || button.pcBindKey != null)
		{
			text += " <color=yellow>✎</color>";
		}
		return text;
	}
}
