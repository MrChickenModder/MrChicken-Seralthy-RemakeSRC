using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GorillaLocomotion;
using Seralyth.Classes.Menu;
using Seralyth.Extensions;
using Seralyth.Menu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Seralyth.Managers;

public class NotificationManager : MonoBehaviour
{
	public GameObject canvas;

	private GameObject mainCamera;

	public static string PreviousNotifi;

	public static readonly Dictionary<string, string> information = new Dictionary<string, string>();

	public static TextMeshProUGUI notificationText;

	public static TextMeshProUGUI arraylistText;

	public static TextMeshProUGUI informationText;

	private bool hasInitialized;

	public static bool noRichText;

	public static bool soundOnError;

	public static bool noPrefix;

	public static bool narrateNotifications;

	public static int NotifiCounter;

	private static readonly List<Coroutine> clearCoroutines = new List<Coroutine>();

	private float updateArraylistTimer;

	public static NotificationManager Instance { get; private set; }

	private void Start()
	{
		Instance = this;
		LogManager.Log("Notifications loaded");
	}

	private void Init()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		mainCamera = ((Component)Camera.main).gameObject;
		GameObject val = new GameObject("Seralyth_NotificationParent");
		val.transform.position = mainCamera.transform.position;
		canvas = new GameObject("Canvas");
		canvas.AddComponent<Canvas>();
		canvas.AddComponent<CanvasScaler>();
		canvas.AddComponent<GraphicRaycaster>();
		Canvas component = canvas.GetComponent<Canvas>();
		((Behaviour)component).enabled = true;
		component.renderMode = (RenderMode)2;
		component.worldCamera = mainCamera.GetComponent<Camera>();
		RectTransform component2 = canvas.GetComponent<RectTransform>();
		component2.sizeDelta = new Vector2(5f, 5f);
		((Transform)component2).position = mainCamera.transform.position;
		canvas.transform.parent = val.transform;
		((Transform)component2).localPosition = new Vector3(0f, 0f, 1.6f);
		((Transform)component2).localScale = Vector3.one;
		Quaternion rotation = ((Transform)component2).rotation;
		Vector3 eulerAngles = ((Quaternion)(ref rotation)).eulerAngles;
		eulerAngles.y = -270f;
		((Transform)component2).rotation = Quaternion.Euler(eulerAngles);
		notificationText = CreateText(canvas.transform, new Vector3(-1f, -1f, -0.5f), new Vector2(450f, 210f), 30, (TextAlignmentOptions)1025);
		arraylistText = CreateText(canvas.transform, new Vector3(-1f, -1f, -0.5f), new Vector2(450f, 1000f), 20, (TextAlignmentOptions)257);
		informationText = CreateText(canvas.transform, new Vector3(-1f, -1f, 0.5f), new Vector2(450f, 1000f), 30, (TextAlignmentOptions)260);
		((MonoBehaviour)this).StartCoroutine(SetShaderAfterInit());
	}

	private TextMeshProUGUI CreateText(Transform parent, Vector3 localPos, Vector2 size, int fontSize, TextAlignmentOptions anchor)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Invalid comparison between Unknown and I4
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject();
		val.transform.parent = parent;
		GameObject val2 = val;
		TextMeshProUGUI val3 = val2.AddComponent<TextMeshProUGUI>();
		((TMP_Text)(object)val3).SafeSetText("");
		((TMP_Text)val3).fontSize = fontSize;
		((TMP_Text)(object)val3).SafeSetFont(Main.AgencyFB);
		((TMP_Text)val3).rectTransform.sizeDelta = size;
		((TMP_Text)val3).alignment = anchor;
		((TMP_Text)val3).overflowMode = (TextOverflowModes)(((int)anchor != 1025) ? 3 : 0);
		((Transform)((TMP_Text)val3).rectTransform).localScale = new Vector3(0.0033333334f, 0.0033333334f, 1f / 3f);
		((Transform)((TMP_Text)val3).rectTransform).localPosition = localPos;
		((TMP_Text)val3).characterSpacing = -9f;
		return val3;
	}

	private void FixedUpdate()
	{
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0625: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!hasInitialized && (Object)(object)Camera.main != (Object)null)
			{
				Init();
				hasInitialized = true;
			}
			canvas.GetComponent<CanvasScaler>().dynamicPixelsPerUnit = 2f;
			canvas.transform.position = mainCamera.transform.TransformPoint(0f, 0f, 1.6f);
			canvas.transform.rotation = mainCamera.transform.rotation * Quaternion.Euler(0f, 90f, 0f);
			canvas.transform.localScale = Vector3.one * (Main.scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
			try
			{
				((TMP_Text)(object)arraylistText).SafeSetFont(Main.activeFont);
				((TMP_Text)(object)arraylistText).SafeSetFontStyle(Main.activeFontStyle);
				((TMP_Text)(object)arraylistText).SafeSetFontSize(Main.arraylistScale);
				((TMP_Text)(object)arraylistText).Chams();
				((TMP_Text)(object)notificationText).SafeSetFont(Main.activeFont);
				((TMP_Text)(object)notificationText).SafeSetFontStyle(Main.activeFontStyle);
				((TMP_Text)(object)notificationText).SafeSetFontSize(Main.notificationScale);
				((Transform)((TMP_Text)notificationText).rectTransform).localPosition = new Vector3(-1f, Main.disableNotifications ? (-100f) : (-1f), -0.5f);
				((TMP_Text)(object)notificationText).Chams();
				((TMP_Text)(object)informationText).SafeSetFont(Main.activeFont);
				((TMP_Text)(object)informationText).SafeSetFontStyle(Main.activeFontStyle);
				((TMP_Text)(object)informationText).SafeSetFontSize(Main.overlayScale);
				((TMP_Text)(object)informationText).Chams();
				Main.FollowMenuSettings((TMP_Text)(object)arraylistText);
				Main.FollowMenuSettings((TMP_Text)(object)notificationText);
				Main.FollowMenuSettings((TMP_Text)(object)informationText);
			}
			catch
			{
			}
			((Transform)((TMP_Text)arraylistText).rectTransform).localPosition = new Vector3(-1f, -1f, Main.flipArraylist ? 0.5f : (-0.5f));
			((TMP_Text)arraylistText).alignment = (TextAlignmentOptions)(Main.flipArraylist ? 260 : 257);
			((Transform)((TMP_Text)informationText).rectTransform).localPosition = new Vector3(-1f, -1f, Main.flipArraylist ? (-0.5f) : 0.5f);
			((TMP_Text)informationText).alignment = (TextAlignmentOptions)(Main.flipArraylist ? 257 : 260);
			if (information.Count > 0)
			{
				Color targetColor = (Buttons.GetIndex("Swap GUI Colors").enabled ? Main.buttonColors[1].GetCurrentColor() : Main.backgroundColor.GetCurrentColor());
				List<string> values = (from item in information
					select "<color=#" + Main.ColorToHex(targetColor) + ">" + item.Key + "</color> <color=#" + Main.ColorToHex(Main.textColors[1].GetColor(0)) + ">" + item.Value + "</color>" into item
					orderby ((TMP_Text)informationText).GetPreferredValues(Main.NoRichtextTags(item)).x descending
					select item).ToList();
				((TMP_Text)(object)informationText).SafeSetText(string.Join("\n", values));
				((Graphic)informationText).color = Color.white;
			}
			else if (!StringUtils.IsNullOrEmpty(((TMP_Text)informationText).text))
			{
				((TMP_Text)(object)informationText).SafeSetText("");
			}
			if (Main.showEnabledModsVR)
			{
				if (Time.time > updateArraylistTimer)
				{
					updateArraylistTimer = Time.time + (Main.advancedArraylist ? 0.1f : 0.5f);
					List<string> list = new List<string>();
					int num = 0;
					ButtonInfo[][] buttons = Buttons.buttons;
					foreach (ButtonInfo[] array in buttons)
					{
						ButtonInfo[] array2 = array;
						foreach (ButtonInfo buttonInfo in array2)
						{
							try
							{
								if (!Buttons.buttons[Buttons.GetCategory("Temporary Category")].Contains(buttonInfo) && !buttonInfo.hideFromArraylist && buttonInfo.enabled && (!Main.hideSettings || (Main.hideSettings && !Buttons.categoryNames[num].Contains("Settings"))))
								{
									string text = buttonInfo.overlapText ?? buttonInfo.buttonText;
									if (Main.inputTextColor != "green")
									{
										text = text.Replace(" <color=grey>[</color><color=green>", " <color=grey>[</color><color=" + Main.inputTextColor + ">");
									}
									text = Main.FixTMProTags(text);
									text = Main.FollowMenuSettings(text);
									list.Add(text);
								}
							}
							catch
							{
							}
						}
						num++;
					}
					string[] array3 = list.OrderByDescending((string s) => ((TMP_Text)arraylistText).GetPreferredValues(Main.NoRichtextTags(s)).x).ToArray();
					string text2 = "";
					for (int num4 = 0; num4 < array3.Length; num4++)
					{
						text2 = ((!Main.advancedArraylist) ? (text2 + array3[num4] + "\n") : (text2 + (Main.flipArraylist ? ("<mark=#" + Main.ColorToHex(Main.backgroundColor.GetCurrentColor((float)num4 * -0.1f)) + "80>" + array3[num4] + "</mark><mark=#" + Main.ColorToHex(Main.buttonColors[1].GetCurrentColor((float)num4 * -0.1f)) + "> </mark>") : ("<mark=#" + Main.ColorToHex(Main.buttonColors[1].GetCurrentColor((float)num4 * -0.1f)) + "> </mark><mark=#" + Main.ColorToHex(Main.backgroundColor.GetCurrentColor((float)num4 * -0.1f)) + "80>" + array3[num4] + "</mark>")) + "\n"));
					}
					((TMP_Text)(object)arraylistText).SafeSetText(text2);
					((Graphic)arraylistText).color = (Buttons.GetIndex("Swap GUI Colors").enabled ? Main.textColors[1].GetColor(0) : Main.backgroundColor.GetCurrentColor());
				}
			}
			else if (!StringUtils.IsNullOrEmpty(((TMP_Text)arraylistText).text))
			{
				((TMP_Text)(object)arraylistText).SafeSetText("");
			}
			if (Main.lowercaseMode)
			{
				if (!StringUtils.IsNullOrEmpty(((TMP_Text)arraylistText).text))
				{
					((TMP_Text)(object)arraylistText).SafeSetText(((TMP_Text)arraylistText).text.ToLower());
				}
				if (!StringUtils.IsNullOrEmpty(((TMP_Text)notificationText).text))
				{
					((TMP_Text)(object)notificationText).SafeSetText(((TMP_Text)notificationText).text.ToLower());
				}
				if (!StringUtils.IsNullOrEmpty(((TMP_Text)informationText).text))
				{
					((TMP_Text)(object)informationText).SafeSetText(((TMP_Text)informationText).text.ToLower());
				}
			}
			if (Main.uppercaseMode)
			{
				if (!StringUtils.IsNullOrEmpty(((TMP_Text)arraylistText).text))
				{
					((TMP_Text)(object)arraylistText).SafeSetText(((TMP_Text)arraylistText).text.ToUpper());
				}
				if (!StringUtils.IsNullOrEmpty(((TMP_Text)notificationText).text))
				{
					((TMP_Text)(object)notificationText).SafeSetText(((TMP_Text)notificationText).text.ToUpper());
				}
				if (!StringUtils.IsNullOrEmpty(((TMP_Text)informationText).text))
				{
					((TMP_Text)(object)informationText).SafeSetText(((TMP_Text)informationText).text.ToUpper());
				}
			}
			canvas.layer = (Buttons.GetIndex("Hide Notifications on Camera").enabled ? 19 : 0);
		}
		catch (Exception log)
		{
			LogManager.Log(log);
		}
	}

	public static void SendNotification(string notificationText, int clearTime = -1)
	{
		if (clearTime < 0)
		{
			clearTime = Main.notificationDecayTime;
		}
		if (Main.disableNotifications && !Buttons.GetIndex("Conduct Notifications").enabled)
		{
			return;
		}
		try
		{
			if (Main.translate)
			{
				if (!TranslationManager.translateCache.ContainsKey(notificationText))
				{
					TranslationManager.TranslateText(notificationText, delegate
					{
						SendNotification(notificationText, clearTime);
					});
					return;
				}
				notificationText = TranslationManager.TranslateText(notificationText);
			}
			if ((!soundOnError || notificationText.Contains("<color=red>ERROR</color>")) && Time.time > Main.timeMenuStarted + 5f)
			{
				SoundManager.Play(SoundManager.DefaultSounds["Notification"], null, delegate(AudioClip clip)
				{
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					AudioSource.PlayClipAtPoint(clip, ((Component)Camera.main).transform.position, (float)Main.buttonClickVolume / 10f);
				});
			}
			if (Main.inputTextColor != "green")
			{
				notificationText = notificationText.Replace("<color=green>", "<color=" + Main.inputTextColor + ">");
			}
			notificationText = Main.FixTMProTags(notificationText);
			if (Main.hideBrackets)
			{
				notificationText = notificationText.Replace("[", "").Replace("]", "");
			}
			notificationText = notificationText.TrimEnd('\n', '\r');
			if (PreviousNotifi == notificationText && Main.stackNotifications)
			{
				NotifiCounter++;
				string[] array = ((TMP_Text)NotificationManager.notificationText).text.Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length != 0)
				{
					string text = array[^1];
					int num = text.IndexOf(" <color=grey>(x", StringComparison.Ordinal);
					if (num > 0)
					{
						text = text.Substring(0, num);
					}
					array[^1] = $"{text} <color=grey>(x{NotifiCounter + 1})</color>";
					((TMP_Text)(object)NotificationManager.notificationText).SafeSetText(string.Join(Environment.NewLine, array));
				}
				if (clearCoroutines.Count > 0)
				{
					CancelClear(clearCoroutines[0]);
				}
			}
			else
			{
				NotifiCounter = 0;
				PreviousNotifi = notificationText;
				if (!string.IsNullOrEmpty(((TMP_Text)NotificationManager.notificationText).text))
				{
					string text2 = ((TMP_Text)NotificationManager.notificationText).text.TrimEnd('\n', '\r');
					((TMP_Text)(object)NotificationManager.notificationText).SafeSetText(text2 + Environment.NewLine + notificationText);
				}
				else
				{
					((TMP_Text)(object)NotificationManager.notificationText).SafeSetText(notificationText);
				}
			}
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(TrackCoroutine(ClearHolder((float)clearTime / 1000f)));
			if (noRichText)
			{
				((TMP_Text)(object)NotificationManager.notificationText).SafeSetText(Main.NoRichtextTags(((TMP_Text)NotificationManager.notificationText).text));
			}
			if (Main.lowercaseMode)
			{
				((TMP_Text)(object)NotificationManager.notificationText).SafeSetText(((TMP_Text)NotificationManager.notificationText).text.ToLower());
			}
			if (Main.uppercaseMode)
			{
				((TMP_Text)(object)NotificationManager.notificationText).SafeSetText(((TMP_Text)NotificationManager.notificationText).text.ToUpper());
			}
			((TMP_Text)NotificationManager.notificationText).richText = !noRichText;
			if (narrateNotifications)
			{
				Main.NarrateText(Main.NoRichtextTags(noPrefix ? RemovePrefix(notificationText) : notificationText));
			}
		}
		catch (Exception ex)
		{
			LogManager.LogError("Notification failed.\nNotification: " + notificationText + "\nException: " + ex.Message);
		}
	}

	public static void PlayNotificationSound()
	{
		SoundManager.Play(SoundManager.DefaultSounds["Notification"], null, delegate(AudioClip clip)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			AudioSource.PlayClipAtPoint(clip, ((Component)Camera.main).transform.position, (float)Main.buttonClickVolume / 10f);
		});
	}

	public static void ClearAllNotifications()
	{
		((TMP_Text)(object)notificationText).SafeSetText("");
		foreach (Coroutine clearCoroutine in clearCoroutines)
		{
			((MonoBehaviour)CoroutineManager.instance).StopCoroutine(clearCoroutine);
		}
		clearCoroutines.Clear();
	}

	public static void ClearPastNotifications(int amount)
	{
		if (string.IsNullOrEmpty(((TMP_Text)notificationText).text))
		{
			return;
		}
		string[] array = ((TMP_Text)notificationText).text.Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
		if (amount >= array.Length)
		{
			((TMP_Text)(object)notificationText).SafeSetText("");
			return;
		}
		List<string> list = new List<string>();
		for (int i = amount; i < array.Length; i++)
		{
			list.Add(array[i]);
		}
		((TMP_Text)(object)notificationText).SafeSetText(string.Join(Environment.NewLine, list));
		((TMP_Text)(object)notificationText).SafeSetText(((TMP_Text)notificationText).text.TrimEnd('\n', '\r'));
	}

	private static IEnumerator TrackCoroutine(IEnumerator routine)
	{
		yield return Wrapper();
		IEnumerator Wrapper()
		{
			Coroutine self = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(routine);
			clearCoroutines.Add(self);
			yield return self;
			clearCoroutines.Remove(self);
		}
	}

	private static IEnumerator ClearHolder(float time = 1f)
	{
		yield return (object)new WaitForSeconds(time);
		ClearPastNotifications(1);
	}

	private IEnumerator SetShaderAfterInit()
	{
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		((TMP_Text)(object)notificationText).Chams();
		((TMP_Text)(object)arraylistText).Chams();
		((TMP_Text)(object)informationText).Chams();
	}

	private static void CancelClear(Coroutine coroutine)
	{
		if (clearCoroutines.Contains(coroutine))
		{
			clearCoroutines.Remove(coroutine);
			((MonoBehaviour)CoroutineManager.instance).StopCoroutine(coroutine);
		}
	}

	private static string RemovePrefix(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		string pattern = "^<color=grey>\\[</color><color=[^>]+>.*?</color><color=grey>\\]</color> ";
		return Regex.Replace(text, pattern, "");
	}
}
