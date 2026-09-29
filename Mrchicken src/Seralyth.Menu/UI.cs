using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using GorillaNetworking;
using Photon.Pun;
using Seralyth.Classes.Menu;
using Seralyth.Extensions;
using Seralyth.Managers;
using Seralyth.Mods;
using Seralyth.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

namespace Seralyth.Menu;

public class UI : MonoBehaviour
{
	public static UI Instance;

	public static Texture2D watermarkImage;

	private bool isOpen = true;

	private bool focusedOnDebug;

	private GameObject uiPrefab;

	private GameObject debugUI;

	private Image watermark;

	private TextMeshProUGUI versionLabel;

	private Vector2 versionLabelDefaultAnchorMin;

	private Vector2 versionLabelDefaultAnchorMax;

	private Vector2 versionLabelDefaultPivot;

	private Vector2 versionLabelDefaultPosition;

	private TextMeshProUGUI roomStatus;

	private TextMeshProUGUI arraylist;

	private TMP_InputField r;

	private TMP_InputField g;

	private TMP_InputField b;

	private TMP_InputField textInput;

	private Image controlBackground;

	private List<TextMeshProUGUI> textObjects;

	private List<Image> imageObjects = new List<Image>();

	private float uiUpdateDelay;

	private readonly string hideGUIPath = "SeralythMenu/Seralyth_HideGUI.txt";

	private GameObject templateLine;

	private void Awake()
	{
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Expected O, but got Unknown
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Expected O, but got Unknown
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Expected O, but got Unknown
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Expected O, but got Unknown
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Expected O, but got Unknown
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		Instance = this;
		if (File.Exists(hideGUIPath))
		{
			isOpen = false;
		}
		uiPrefab = AssetUtilities.LoadObject<GameObject>("UI");
		Transform val = uiPrefab.transform.Find("Canvas");
		watermark = ((Component)val.Find("Watermark")).GetComponent<Image>();
		versionLabel = ((Component)val.Find("VersionLabel")).GetComponent<TextMeshProUGUI>();
		roomStatus = ((Component)val.Find("RoomStatus")).GetComponent<TextMeshProUGUI>();
		arraylist = ((Component)val.Find("Arraylist")).GetComponent<TextMeshProUGUI>();
		controlBackground = ((Component)val.Find("ControlUI")).GetComponent<Image>();
		Transform obj = val.Find("DebugUI");
		debugUI = ((obj != null) ? ((Component)obj).gameObject : null);
		debugUI.AddComponent<UIDragWindow>();
		Transform obj2 = debugUI.transform.Find("Lines/Line");
		templateLine = ((obj2 != null) ? ((Component)obj2).gameObject : null);
		r = ((Component)val.Find("ControlUI/R")).GetComponent<TMP_InputField>();
		g = ((Component)val.Find("ControlUI/G")).GetComponent<TMP_InputField>();
		b = ((Component)val.Find("ControlUI/B")).GetComponent<TMP_InputField>();
		textInput = ((Component)val.Find("ControlUI/TextInput")).GetComponent<TMP_InputField>();
		((UnityEvent)((Component)val.Find("ControlUI/QueueButton")).GetComponent<Button>().onClick).AddListener((UnityAction)delegate
		{
			Important.QueueRoom(textInput.text);
		});
		((UnityEvent)((Component)val.Find("ControlUI/JoinButton")).GetComponent<Button>().onClick).AddListener((UnityAction)delegate
		{
			((PhotonNetworkController)PhotonNetworkController.Instance).AttemptToJoinSpecificRoom(textInput.text, (JoinType)0);
		});
		((UnityEvent)((Component)val.Find("ControlUI/ColorButton")).GetComponent<Button>().onClick).AddListener((UnityAction)delegate
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			Main.ChangeColor(Color32.op_Implicit(new Color32(byte.Parse(r.text), byte.Parse(g.text), byte.Parse(b.text), byte.MaxValue)));
		});
		((UnityEvent)((Component)val.Find("ControlUI/NameButton")).GetComponent<Button>().onClick).AddListener((UnityAction)delegate
		{
			Main.ChangeName(textInput.text);
		});
		TMP_InputField inputField = ((Component)debugUI.transform.Find("TextInput")).gameObject.GetComponent<TMP_InputField>();
		((UnityEvent<string>)(object)inputField.onSelect).AddListener((UnityAction<string>)delegate
		{
			focusedOnDebug = true;
		});
		((UnityEvent<string>)(object)inputField.onDeselect).AddListener((UnityAction<string>)delegate
		{
			focusedOnDebug = false;
		});
		((UnityEvent<string>)(object)inputField.onEndEdit).AddListener((UnityAction<string>)delegate(string text)
		{
			if (focusedOnDebug && !StringUtils.IsNullOrEmpty(inputField.text))
			{
				HandleDebugCommand(text);
			}
			inputField.text = string.Empty;
		});
		textObjects = new List<TextMeshProUGUI>
		{
			((Component)val.Find("ControlUI/TextInput/Text Area/Text")).GetComponent<TextMeshProUGUI>(),
			((Component)val.Find("ControlUI/R/Text Area/Text")).GetComponent<TextMeshProUGUI>(),
			((Component)val.Find("ControlUI/G/Text Area/Text")).GetComponent<TextMeshProUGUI>(),
			((Component)val.Find("ControlUI/B/Text Area/Text")).GetComponent<TextMeshProUGUI>(),
			((Component)val.Find("ControlUI/QueueButton/Text")).GetComponent<TextMeshProUGUI>(),
			((Component)val.Find("ControlUI/JoinButton/Text")).GetComponent<TextMeshProUGUI>(),
			((Component)val.Find("ControlUI/ColorButton/Text")).GetComponent<TextMeshProUGUI>(),
			((Component)val.Find("ControlUI/NameButton/Text")).GetComponent<TextMeshProUGUI>(),
			((Component)val.Find("HideMessage")).GetComponent<TextMeshProUGUI>()
		};
		imageObjects = new List<Image>
		{
			((Component)val.Find("ControlUI/TextInput")).GetComponent<Image>(),
			((Component)val.Find("ControlUI/R")).GetComponent<Image>(),
			((Component)val.Find("ControlUI/G")).GetComponent<Image>(),
			((Component)val.Find("ControlUI/B")).GetComponent<Image>(),
			((Component)val.Find("ControlUI/QueueButton")).GetComponent<Image>(),
			((Component)val.Find("ControlUI/JoinButton")).GetComponent<Image>(),
			((Component)val.Find("ControlUI/ColorButton")).GetComponent<Image>(),
			((Component)val.Find("ControlUI/NameButton")).GetComponent<Image>(),
			((Component)debugUI.transform.Find("TextInput")).GetComponent<Image>(),
			((Component)debugUI.transform.Find("Lines")).GetComponent<Image>()
		};
		((Graphic)watermark).material = new Material(((Graphic)watermark).material);
		watermarkImage = AssetUtilities.LoadTextureFromResource("SeralythMenu.Resources.Client.icon.png");
		if (!Bootstrapper.FirstLaunch)
		{
			Transform obj3 = uiPrefab.transform.Find("Canvas");
			object obj4;
			if (obj3 == null)
			{
				obj4 = null;
			}
			else
			{
				Transform obj5 = obj3.Find("HideMessage");
				obj4 = ((obj5 != null) ? ((Component)obj5).gameObject : null);
			}
			GameObject val2 = (GameObject)obj4;
			if (val2 != null)
			{
				val2.SetActive(false);
			}
		}
		versionLabelDefaultAnchorMin = ((TMP_Text)versionLabel).rectTransform.anchorMin;
		versionLabelDefaultAnchorMax = ((TMP_Text)versionLabel).rectTransform.anchorMax;
		versionLabelDefaultPivot = ((TMP_Text)versionLabel).rectTransform.pivot;
		versionLabelDefaultPosition = ((TMP_Text)versionLabel).rectTransform.anchoredPosition;
		TextMeshProUGUI[] componentsInChildren = ((Component)val).GetComponentsInChildren<TextMeshProUGUI>(true);
		foreach (TextMeshProUGUI val3 in componentsInChildren)
		{
			if (!string.IsNullOrEmpty(((TMP_Text)val3).text))
			{
				((TMP_Text)val3).text = ((TMP_Text)val3).text.Replace("Seralyth Remake", "Chicken").Replace("Seralyth", "Chicken").Replace("SERALYTH REMAKE", "Chicken")
					.Replace("SERALYTH", "CHICKEN");
			}
		}
		Update();
	}

	private void Update()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_095f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0982: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0707: Unknown result type (might be due to invalid IL or missing references)
		//IL_0727: Unknown result type (might be due to invalid IL or missing references)
		//IL_0747: Unknown result type (might be due to invalid IL or missing references)
		//IL_0767: Unknown result type (might be due to invalid IL or missing references)
		//IL_0787: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Unknown result type (might be due to invalid IL or missing references)
		//IL_066f: Unknown result type (might be due to invalid IL or missing references)
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		if (((ButtonControl)Keyboard.current.backslashKey).wasPressedThisFrame)
		{
			ToggleGUI();
		}
		if (isOpen)
		{
			uiPrefab.SetActive(true);
			if (((ButtonControl)Keyboard.current.backquoteKey).wasPressedThisFrame)
			{
				ToggleDebug();
			}
			Color color = (Buttons.GetIndex("Swap GUI Colors").enabled ? Main.textColors[1].GetCurrentColor() : Main.backgroundColor.GetCurrentColor());
			((Graphic)versionLabel).color = color;
			((Graphic)roomStatus).color = color;
			((Graphic)arraylist).color = color;
			((Graphic)watermark).color = color;
			((Component)watermark).gameObject.SetActive(!Main.disableWatermark);
			((TMP_Text)(object)versionLabel).SafeSetFont(Main.activeFont);
			((TMP_Text)(object)roomStatus).SafeSetFont(Main.activeFont);
			((TMP_Text)(object)arraylist).SafeSetFont(Main.activeFont);
			((TMP_Text)(object)versionLabel).SafeSetFontStyle(Main.activeFontStyle);
			((TMP_Text)(object)roomStatus).SafeSetFontStyle(Main.activeFontStyle);
			((TMP_Text)(object)arraylist).SafeSetFontStyle(Main.activeFontStyle);
			((Graphic)controlBackground).color = Main.menuBackgroundColor.GetCurrentColor();
			foreach (TextMeshProUGUI textObject in textObjects)
			{
				((Graphic)textObject).color = Main.textColors[1].GetCurrentColor();
				((TMP_Text)(object)textObject).SafeSetFont(Main.activeFont);
				((TMP_Text)(object)textObject).SafeSetFontStyle(Main.activeFontStyle);
			}
			foreach (Image imageObject in imageObjects)
			{
				((Graphic)imageObject).color = Main.buttonColors[0].GetCurrentColor();
			}
			((Component)watermark).transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Sin(Time.time * 1.2f) * 8f);
			((TMP_Text)(object)versionLabel).SafeSetText(Main.FollowMenuSettings("Build") + " 10.0.2\n" + Main.serverLink.Replace("https://", ""));
			if (Main.disableWatermark)
			{
				((TMP_Text)versionLabel).rectTransform.anchorMin = new Vector2(1f, ((TMP_Text)versionLabel).rectTransform.anchorMin.y);
				((TMP_Text)versionLabel).rectTransform.anchorMax = new Vector2(1f, ((TMP_Text)versionLabel).rectTransform.anchorMax.y);
				((TMP_Text)versionLabel).rectTransform.pivot = new Vector2(1f, 0.5f);
				((TMP_Text)versionLabel).rectTransform.anchoredPosition = new Vector2(-10f, ((TMP_Text)versionLabel).rectTransform.anchoredPosition.y);
			}
			else
			{
				((TMP_Text)versionLabel).rectTransform.anchorMin = versionLabelDefaultAnchorMin;
				((TMP_Text)versionLabel).rectTransform.anchorMax = versionLabelDefaultAnchorMax;
				((TMP_Text)versionLabel).rectTransform.pivot = versionLabelDefaultPivot;
				((TMP_Text)versionLabel).rectTransform.anchoredPosition = versionLabelDefaultPosition;
			}
			((TMP_Text)(object)roomStatus).SafeSetText(Main.FollowMenuSettings((!PhotonNetwork.InRoom) ? "Not connected to room" : "Connected to room ") + (PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.Name : ""));
			if (debugUI.activeSelf)
			{
				((Graphic)debugUI.GetComponent<Image>()).color = Main.backgroundColor.GetCurrentColor();
				List<TextMeshProUGUI> list = new List<TextMeshProUGUI>
				{
					((Component)debugUI.transform.Find("Title")).GetComponent<TextMeshProUGUI>(),
					((Component)debugUI.transform.Find("TextInput/Text Area/Text")).GetComponent<TextMeshProUGUI>(),
					((Component)debugUI.transform.Find("TextInput/Text Area/Placeholder")).GetComponent<TextMeshProUGUI>()
				};
				list.AddRange(((Component)debugUI.transform.Find("Lines")).GetComponentsInChildren<TextMeshProUGUI>());
				foreach (TextMeshProUGUI item in list)
				{
					((Graphic)item).color = Main.textColors[1].GetCurrentColor();
					((TMP_Text)(object)item).SafeSetFont(Main.activeFont);
					((TMP_Text)(object)item).SafeSetFontStyle(Main.activeFontStyle);
				}
				((Graphic)((Component)debugUI.transform.Find("Title")).GetComponent<TextMeshProUGUI>()).color = Main.textColors[0].GetCurrentColor();
			}
			if (!(Time.time > uiUpdateDelay))
			{
				return;
			}
			Texture2D val = Main.customWatermark ?? watermarkImage;
			if ((Object)(object)watermark.sprite == (Object)null || (Object)(object)watermark.sprite.texture == (Object)null || (Object)(object)watermark.sprite.texture != (Object)(object)val)
			{
				Sprite sprite = Sprite.Create(val, new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), new Vector2(0.5f, 0.5f), 100f);
				watermark.sprite = sprite;
			}
			if (Main.flipArraylist)
			{
				((Graphic)controlBackground).rectTransform.anchoredPosition = new Vector2(10f, -10f);
				((Graphic)controlBackground).rectTransform.anchorMin = new Vector2(0f, 1f);
				((Graphic)controlBackground).rectTransform.anchorMax = new Vector2(0f, 1f);
				((TMP_Text)arraylist).rectTransform.anchoredPosition = new Vector2(-837.5001f, -523f);
				((TMP_Text)arraylist).rectTransform.anchorMin = new Vector2(1f, 1f);
				((TMP_Text)arraylist).rectTransform.anchorMax = new Vector2(1f, 1f);
				((TMP_Text)arraylist).alignment = (TextAlignmentOptions)260;
			}
			else
			{
				((Graphic)controlBackground).rectTransform.anchoredPosition = new Vector2(-250f, -10f);
				((Graphic)controlBackground).rectTransform.anchorMin = new Vector2(1f, 1f);
				((Graphic)controlBackground).rectTransform.anchorMax = new Vector2(1f, 1f);
				((TMP_Text)arraylist).rectTransform.anchoredPosition = new Vector2(837.5001f, -523f);
				((TMP_Text)arraylist).rectTransform.anchorMin = new Vector2(0f, 1f);
				((TMP_Text)arraylist).rectTransform.anchorMax = new Vector2(0f, 1f);
				((TMP_Text)arraylist).alignment = (TextAlignmentOptions)257;
			}
			uiUpdateDelay = Time.time + (Main.advancedArraylist ? 0.1f : 0.5f);
			List<string> list2 = new List<string>();
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
							list2.Add(text);
						}
					}
					catch
					{
					}
				}
				num++;
			}
			string[] array3 = list2.OrderByDescending((string s) => ((TMP_Text)arraylist).GetPreferredValues(Main.NoRichtextTags(s)).x).ToArray();
			string text2 = "";
			for (int num2 = 0; num2 < array3.Length; num2++)
			{
				text2 = ((!Main.advancedArraylist) ? (text2 + array3[num2] + "\n") : (text2 + (Main.flipArraylist ? ("<mark=#" + Main.ColorToHex(Main.backgroundColor.GetCurrentColor((float)num2 * -0.1f)) + "C0> " + array3[num2] + " </mark><mark=#" + Main.ColorToHex(Main.buttonColors[1].GetCurrentColor((float)num2 * -0.1f)) + "> </mark>") : ("<mark=#" + Main.ColorToHex(Main.buttonColors[1].GetCurrentColor((float)num2 * -0.1f)) + "> </mark><mark=#" + Main.ColorToHex(Main.backgroundColor.GetCurrentColor((float)num2 * -0.1f)) + "C0> " + array3[num2] + " </mark>")) + "\n"));
			}
			((TMP_Text)(object)arraylist).SafeSetText(text2);
		}
		else
		{
			uiPrefab.SetActive(false);
		}
	}

	private void ToggleGUI()
	{
		isOpen = !isOpen;
		if (isOpen)
		{
			if (File.Exists(hideGUIPath))
			{
				File.Delete(hideGUIPath);
			}
		}
		else if (!File.Exists(hideGUIPath))
		{
			File.WriteAllText(hideGUIPath, "Text file generated with MrChicken Menu");
		}
		Transform obj = uiPrefab.transform.Find("Canvas");
		object obj2;
		if (obj == null)
		{
			obj2 = null;
		}
		else
		{
			Transform obj3 = obj.Find("HideMessage");
			obj2 = ((obj3 != null) ? ((Component)obj3).gameObject : null);
		}
		GameObject val = (GameObject)obj2;
		if (val != null)
		{
			val.SetActive(false);
		}
	}

	private void ToggleDebug()
	{
		if (debugUI.activeSelf)
		{
			debugUI.SetActive(false);
			return;
		}
		if (Main.dynamicSounds)
		{
			AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Menu/console.ogg", "Audio/Menu/console.ogg", delegate(AudioClip clip)
			{
				clip.Play((float)Main.buttonClickVolume / 10f);
			});
		}
		debugUI.SetActive(true);
	}

	public void DebugPrint(string text)
	{
		if (debugUI.activeSelf)
		{
			GameObject val = Object.Instantiate<GameObject>(templateLine, debugUI.transform.Find("Lines"), false);
			val.SetActive(true);
			((TMP_Text)val.GetComponent<TextMeshProUGUI>()).text = text;
			if (debugUI.transform.Find("Lines").childCount > 14)
			{
				Object.Destroy((Object)(object)debugUI.transform.Find("Lines").GetChild(1));
			}
		}
	}

	public void HandleDebugCommand(string command)
	{
		string[] array = command.Split(' ');
		string text = array[0].ToLower();
		switch (text)
		{
		case "print":
			DebugPrint(StringUtils.Join(array.Skip(1), " "));
			break;
		case "admin":
		{
			string text5 = ((array.Length > 1) ? array[1] : PhotonNetwork.LocalPlayer.UserId);
			string text6 = ((array.Length > 2) ? array[2] : PhotonNetwork.LocalPlayer.NickName);
			ServerData.LocalAdmins.Add(text5, text6);
			DebugPrint("Added (" + text5 + ", " + text6 + ") to local administrators");
			break;
		}
		case "beta":
			PluginInfo.BetaBuild = array.Length > 1 && array[1].ToLower() == "true";
			DebugPrint($"PluginInfo.BetaBuild is now {PluginInfo.BetaBuild}");
			break;
		case "telemetry":
			ServerData.DisableTelemetry = array.Length < 1 || array[1] == "false";
			DebugPrint("Telemetry is now " + (ServerData.DisableTelemetry ? "disabled" : "enabled"));
			break;
		case "prompt":
		{
			MatchCollection source = Regex.Matches(StringUtils.Join(array.Skip(1), " "), "\\[(.*?)\\]");
			List<string> list = (from @group in source.Select((Match matches) => matches.Groups).SelectMany<GroupCollection, Group>((GroupCollection @group) => @group)
				select @group.Value).ToList();
			string text2 = ((array.Length > 1) ? array[1] : "Prompt text");
			string text3 = ((array.Length > 2) ? array[2] : "Accept");
			string text4 = ((array.Length > 3) ? array[3] : "Decline");
			Main.Prompt(text2, delegate
			{
				DebugPrint("Prompt accepted");
			}, delegate
			{
				DebugPrint("Prompt declined");
			}, text3, text4);
			DebugPrint("Propted user " + text2 + " " + text3 + " " + text4);
			break;
		}
		case "exit":
		case "quit":
		case "close":
			Application.Quit();
			break;
		default:
			DebugPrint("Unknown command: '" + text + "'");
			break;
		}
	}

	private void OnGUI()
	{
		if (isOpen)
		{
			PluginManager.ExecuteOnGUI();
		}
	}
}
