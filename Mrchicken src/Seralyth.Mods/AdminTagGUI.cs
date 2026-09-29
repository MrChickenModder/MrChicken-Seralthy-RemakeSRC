using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Photon.Pun;
using Seralyth.Classes.Menu;
using Seralyth.Managers;
using Seralyth.Menu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Seralyth.Mods;

public static class AdminTagGUI
{
	public class AdminIconEntry
	{
		public string name;

		public string url;

		public string fileName;

		public string text;

		public bool isText;

		public string cachedTexturePath;

		public string localPath;
	}

	public static bool IsOpen;

	public static bool ShowAdminTag = true;

	public static string AdminTagText = "ADMIN";

	public static Color AdminTagColor = new Color(0.75f, 0f, 1f, 1f);

	public static float AdminTagScale = 1f;

	public static bool ShowCustomIcons = true;

	public static readonly List<AdminIconEntry> ActiveIcons = new List<AdminIconEntry>();

	private static readonly Dictionary<string, string> PresetIcons = new Dictionary<string, string>
	{
		{ "Crown", "crown.png" },
		{ "Super Admin", "icon.png" },
		{ "Shield", "shield.png" },
		{ "Star", "star.png" },
		{ "Heart", "heart.png" },
		{ "Diamond", "diamond.png" },
		{ "Fire", "fire.png" },
		{ "Lightning", "lightning.png" }
	};

	private static Rect windowRect = new Rect(20f, 20f, 380f, 520f);

	private static Vector2 scrollPos;

	public static string CustomIconUrl = "";

	public static string CustomTagText = "";

	private static int selectedPresetIndex;

	private static bool showPresetList;

	private static bool showColorPicker;

	private static int localPickerPage;

	private static readonly Color bgCol = new Color(0.06f, 0.04f, 0.12f, 0.96f);

	private static readonly Color headerCol = new Color(0.15f, 0.08f, 0.3f, 1f);

	private static readonly Color sectionCol = new Color(0.1f, 0.07f, 0.2f, 0.9f);

	private static readonly Color accentCol = new Color(0.5f, 0.2f, 1f, 1f);

	private static readonly Color toggleOnCol = new Color(0.13f, 0.55f, 0.13f, 1f);

	private static readonly Color toggleOffCol = new Color(0.55f, 0.13f, 0.13f, 1f);

	private static readonly Color textCol = new Color(0.9f, 0.9f, 0.9f, 1f);

	private static readonly Color dimTextCol = new Color(0.55f, 0.55f, 0.65f, 1f);

	private static readonly Color dangerCol = new Color(0.7f, 0.15f, 0.15f, 1f);

	private static GUIStyle headerStyle;

	private static GUIStyle sectionStyle;

	private static GUIStyle toggleOnStyle;

	private static GUIStyle toggleOffStyle;

	private static GUIStyle labelStyle;

	private static GUIStyle smallLabelStyle;

	private static GUIStyle textInputStyle;

	private static GUIStyle smallBtnStyle;

	private static GUIStyle dangerBtnStyle;

	private static GUIStyle accentBtnStyle;

	private static GUIStyle boxStyle;

	private static bool stylesInit;

	private static readonly Dictionary<VRRig, GameObject> tagObjects = new Dictionary<VRRig, GameObject>();

	private static readonly Dictionary<VRRig, List<GameObject>> iconObjects = new Dictionary<VRRig, List<GameObject>>();

	private static Material iconMaterial;

	private static readonly Dictionary<GameObject, float> spawnTimes = new Dictionary<GameObject, float>();

	private const float PopDuration = 0.3f;

	private static float lastToggleTime;

	private static bool prevXState;

	public static void Toggle()
	{
		IsOpen = !IsOpen;
		if (IsOpen)
		{
			CenterWindow();
		}
	}

	private static void CenterWindow()
	{
		((Rect)(ref windowRect)).x = ((float)Screen.width - ((Rect)(ref windowRect)).width) * 0.5f;
		((Rect)(ref windowRect)).y = ((float)Screen.height - ((Rect)(ref windowRect)).height) * 0.5f;
	}

	private static void PollXButton()
	{
		bool leftPrimary = Main.leftPrimary;
		if (leftPrimary && !prevXState && Time.time - lastToggleTime > 0.35f)
		{
			lastToggleTime = Time.time;
			Toggle();
		}
		prevXState = leftPrimary;
	}

	public static void OnGUI()
	{
		DrawAdminTagGUI();
	}

	public static void DrawAdminTagGUI()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		PollXButton();
		if (IsOpen && PhotonNetwork.InRoom && ServerData.HasConsoleModAccess())
		{
			InitStyles();
			windowRect = GUI.Window(77777, windowRect, new WindowFunction(DrawWindow), "");
		}
	}

	private static void InitStyles()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Expected O, but got Unknown
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Expected O, but got Unknown
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Expected O, but got Unknown
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Expected O, but got Unknown
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Expected O, but got Unknown
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Expected O, but got Unknown
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Expected O, but got Unknown
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Expected O, but got Unknown
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Expected O, but got Unknown
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Expected O, but got Unknown
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Expected O, but got Unknown
		if (!stylesInit)
		{
			GUIStyle val = new GUIStyle(GUI.skin.label)
			{
				fontSize = 14,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			val.normal.textColor = Color.white;
			headerStyle = val;
			GUIStyle val2 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 12,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)3,
				padding = new RectOffset(8, 0, 0, 0)
			};
			val2.normal.textColor = new Color(0.8f, 0.6f, 1f);
			sectionStyle = val2;
			GUIStyle val3 = new GUIStyle(GUI.skin.button)
			{
				fontSize = 11,
				fontStyle = (FontStyle)1
			};
			val3.normal.textColor = Color.white;
			val3.normal.background = MakeTex(2, 2, toggleOnCol);
			val3.hover.textColor = Color.white;
			val3.hover.background = MakeTex(2, 2, toggleOnCol * 1.2f);
			toggleOnStyle = val3;
			GUIStyle val4 = new GUIStyle(GUI.skin.button)
			{
				fontSize = 11,
				fontStyle = (FontStyle)1
			};
			val4.normal.textColor = Color.white;
			val4.normal.background = MakeTex(2, 2, toggleOffCol);
			val4.hover.textColor = Color.white;
			val4.hover.background = MakeTex(2, 2, toggleOffCol * 1.2f);
			toggleOffStyle = val4;
			GUIStyle val5 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 11,
				alignment = (TextAnchor)3,
				padding = new RectOffset(4, 0, 2, 0)
			};
			val5.normal.textColor = textCol;
			labelStyle = val5;
			GUIStyle val6 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 10,
				alignment = (TextAnchor)3,
				padding = new RectOffset(8, 0, 0, 0)
			};
			val6.normal.textColor = dimTextCol;
			smallLabelStyle = val6;
			GUIStyle val7 = new GUIStyle(GUI.skin.textField)
			{
				fontSize = 11
			};
			val7.normal.textColor = Color.white;
			val7.normal.background = MakeTex(2, 2, new Color(0.15f, 0.12f, 0.25f, 1f));
			val7.focused.textColor = Color.white;
			val7.focused.background = MakeTex(2, 2, new Color(0.2f, 0.15f, 0.35f, 1f));
			textInputStyle = val7;
			GUIStyle val8 = new GUIStyle(GUI.skin.button)
			{
				fontSize = 10,
				fontStyle = (FontStyle)1
			};
			val8.normal.textColor = Color.white;
			val8.normal.background = MakeTex(2, 2, new Color(0.25f, 0.2f, 0.4f, 1f));
			val8.hover.textColor = Color.white;
			val8.hover.background = MakeTex(2, 2, new Color(0.35f, 0.28f, 0.55f, 1f));
			smallBtnStyle = val8;
			GUIStyle val9 = new GUIStyle(GUI.skin.button)
			{
				fontSize = 10,
				fontStyle = (FontStyle)1
			};
			val9.normal.textColor = Color.white;
			val9.normal.background = MakeTex(2, 2, dangerCol);
			val9.hover.textColor = Color.white;
			val9.hover.background = MakeTex(2, 2, dangerCol * 1.3f);
			dangerBtnStyle = val9;
			GUIStyle val10 = new GUIStyle(GUI.skin.button)
			{
				fontSize = 10,
				fontStyle = (FontStyle)1
			};
			val10.normal.textColor = Color.white;
			val10.normal.background = MakeTex(2, 2, accentCol);
			val10.hover.textColor = Color.white;
			val10.hover.background = MakeTex(2, 2, accentCol * 1.2f);
			accentBtnStyle = val10;
			GUIStyle val11 = new GUIStyle(GUI.skin.box);
			val11.normal.background = MakeTex(2, 2, bgCol);
			boxStyle = val11;
			stylesInit = true;
		}
	}

	private static void DrawWindow(int id)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0653: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0719: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0902: Unknown result type (might be due to invalid IL or missing references)
		//IL_0932: Unknown result type (might be due to invalid IL or missing references)
		//IL_0966: Unknown result type (might be due to invalid IL or missing references)
		//IL_078f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a10: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a32: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0832: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d02: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c10: Unknown result type (might be due to invalid IL or missing references)
		float num = 5f;
		GUI.backgroundColor = headerCol;
		GUI.Box(new Rect(0f, 0f, ((Rect)(ref windowRect)).width, 30f), "");
		GUI.backgroundColor = Color.white;
		GUI.Label(new Rect(0f, 5f, ((Rect)(ref windowRect)).width, 20f), "Admin Tag Manager  |  X / Alt: Close", headerStyle);
		num = 35f;
		GUI.backgroundColor = sectionCol;
		GUI.Box(new Rect(5f, num, ((Rect)(ref windowRect)).width - 10f, 20f), "");
		GUI.backgroundColor = Color.white;
		GUI.Label(new Rect(5f, num + 2f, ((Rect)(ref windowRect)).width - 10f, 16f), "Admin Tag", sectionStyle);
		num += 25f;
		DrawToggle(ref ShowAdminTag, "Show Tag Above Head", 8f, ref num);
		GUI.Label(new Rect(8f, num + 2f, 80f, 20f), "Tag Text:", labelStyle);
		AdminTagText = GUI.TextField(new Rect(90f, num, 140f, 20f), AdminTagText, textInputStyle);
		num += 24f;
		GUI.Label(new Rect(8f, num + 2f, 80f, 20f), "Scale:", labelStyle);
		AdminTagScale = GUI.HorizontalSlider(new Rect(90f, num + 4f, 140f, 16f), AdminTagScale, 0.5f, 3f);
		GUI.Label(new Rect(235f, num + 2f, 40f, 20f), AdminTagScale.ToString("F1"), smallLabelStyle);
		num += 22f;
		GUI.Label(new Rect(8f, num + 2f, 80f, 20f), "Tag Color:", labelStyle);
		if (GUI.Button(new Rect(90f, num, 50f, 20f), "Pick", smallBtnStyle))
		{
			showColorPicker = !showColorPicker;
		}
		GUI.backgroundColor = AdminTagColor;
		GUI.Box(new Rect(145f, num, 20f, 20f), "");
		GUI.backgroundColor = Color.white;
		num += 24f;
		if (showColorPicker)
		{
			GUI.Label(new Rect(16f, num, 30f, 16f), "R:", smallLabelStyle);
			AdminTagColor.r = GUI.HorizontalSlider(new Rect(35f, num, 100f, 16f), AdminTagColor.r, 0f, 1f);
			GUI.Label(new Rect(140f, num, 40f, 16f), AdminTagColor.r.ToString("F2"), smallLabelStyle);
			num += 18f;
			GUI.Label(new Rect(16f, num, 30f, 16f), "G:", smallLabelStyle);
			AdminTagColor.g = GUI.HorizontalSlider(new Rect(35f, num, 100f, 16f), AdminTagColor.g, 0f, 1f);
			GUI.Label(new Rect(140f, num, 40f, 16f), AdminTagColor.g.ToString("F2"), smallLabelStyle);
			num += 18f;
			GUI.Label(new Rect(16f, num, 30f, 16f), "B:", smallLabelStyle);
			AdminTagColor.b = GUI.HorizontalSlider(new Rect(35f, num, 100f, 16f), AdminTagColor.b, 0f, 1f);
			GUI.Label(new Rect(140f, num, 40f, 16f), AdminTagColor.b.ToString("F2"), smallLabelStyle);
			num += 18f;
			GUI.Label(new Rect(16f, num, 30f, 16f), "A:", smallLabelStyle);
			AdminTagColor.a = GUI.HorizontalSlider(new Rect(35f, num, 100f, 16f), AdminTagColor.a, 0f, 1f);
			GUI.Label(new Rect(140f, num, 40f, 16f), AdminTagColor.a.ToString("F2"), smallLabelStyle);
			num += 20f;
		}
		num += 5f;
		GUI.backgroundColor = sectionCol;
		GUI.Box(new Rect(5f, num, ((Rect)(ref windowRect)).width - 10f, 20f), "");
		GUI.backgroundColor = Color.white;
		GUI.Label(new Rect(5f, num + 2f, ((Rect)(ref windowRect)).width - 10f, 16f), "Custom Icons", sectionStyle);
		num += 25f;
		DrawToggle(ref ShowCustomIcons, "Show Icons Above Head", 8f, ref num);
		GUI.Label(new Rect(8f, num + 2f, 80f, 20f), "Presets:", labelStyle);
		string[] array = PresetIcons.Keys.ToArray();
		selectedPresetIndex = GUI.Toolbar(new Rect(90f, num, ((Rect)(ref windowRect)).width - 100f, 20f), selectedPresetIndex, array, smallBtnStyle);
		num += 24f;
		if (GUI.Button(new Rect(8f, num, (((Rect)(ref windowRect)).width - 20f) / 2f, 22f), "Add Preset Icon", accentBtnStyle) && selectedPresetIndex >= 0 && selectedPresetIndex < array.Length)
		{
			AddPresetIcon(array[selectedPresetIndex]);
		}
		num += 26f;
		GUI.Label(new Rect(8f, num + 2f, 80f, 20f), "URL:", labelStyle);
		CustomIconUrl = GUI.TextField(new Rect(90f, num, ((Rect)(ref windowRect)).width - 180f, 20f), CustomIconUrl, textInputStyle);
		if (GUI.Button(new Rect(((Rect)(ref windowRect)).width - 85f, num, 75f, 20f), "Add URL", accentBtnStyle) && !string.IsNullOrEmpty(CustomIconUrl))
		{
			AddCustomIcon(CustomIconUrl);
		}
		num += 24f;
		if (ServerData.IsSuperAdmin())
		{
			if (ServerData.IsOwnerLocal())
			{
				GUI.Label(new Rect(8f, num + 2f, 70f, 20f), "Owner:", labelStyle);
				if (GUI.Button(new Rect(70f, num, 80f, 20f), "Chicken", accentBtnStyle))
				{
					string text = LocalIcons.EnsureChickenIcon();
					if (!string.IsNullOrEmpty(text))
					{
						ActiveIcons.Add(new AdminIconEntry
						{
							name = "Chicken",
							localPath = text,
							isText = false
						});
					}
				}
				GUI.Label(new Rect(156f, num + 2f, ((Rect)(ref windowRect)).width - 164f, 20f), "Adds the chicken icon", smallLabelStyle);
				num += 24f;
			}
			localPickerPage = LocalIcons.DrawPicker(8f, ref num, ((Rect)(ref windowRect)).width - 18f, localPickerPage, labelStyle, smallLabelStyle, smallBtnStyle, accentBtnStyle, delegate(string file)
			{
				ActiveIcons.Add(new AdminIconEntry
				{
					name = Path.GetFileNameWithoutExtension(file),
					localPath = file,
					isText = false
				});
			});
		}
		else
		{
			GUI.Label(new Rect(8f, num + 2f, ((Rect)(ref windowRect)).width - 18f, 20f), "Local icons: super admin only", smallLabelStyle);
			num += 24f;
		}
		GUI.Label(new Rect(8f, num + 2f, 80f, 20f), "Text Icon:", labelStyle);
		CustomTagText = GUI.TextField(new Rect(90f, num, ((Rect)(ref windowRect)).width - 180f, 20f), CustomTagText, textInputStyle);
		if (GUI.Button(new Rect(((Rect)(ref windowRect)).width - 85f, num, 75f, 20f), "Add Text", accentBtnStyle) && !string.IsNullOrEmpty(CustomTagText))
		{
			AddTextIcon(CustomTagText);
		}
		num += 24f;
		float num2 = Mathf.Min((float)ActiveIcons.Count * 22f + 5f, 120f);
		GUI.backgroundColor = new Color(0.08f, 0.06f, 0.15f, 0.9f);
		GUI.Box(new Rect(5f, num, ((Rect)(ref windowRect)).width - 10f, num2), "");
		GUI.backgroundColor = Color.white;
		scrollPos = GUI.BeginScrollView(new Rect(5f, num, ((Rect)(ref windowRect)).width - 10f, num2), scrollPos, new Rect(0f, 0f, ((Rect)(ref windowRect)).width - 30f, (float)ActiveIcons.Count * 22f + 5f));
		float num3 = 2f;
		for (int num4 = ActiveIcons.Count - 1; num4 >= 0; num4--)
		{
			AdminIconEntry adminIconEntry = ActiveIcons[num4];
			string text2 = (adminIconEntry.isText ? ("[T] " + adminIconEntry.text) : ((!string.IsNullOrEmpty(adminIconEntry.localPath)) ? ("[F] " + adminIconEntry.name) : ("[I] " + (adminIconEntry.name ?? "Custom"))));
			GUI.Label(new Rect(5f, num3, ((Rect)(ref windowRect)).width - 70f, 18f), text2, labelStyle);
			if (GUI.Button(new Rect(((Rect)(ref windowRect)).width - 60f, num3, 25f, 18f), "^", smallBtnStyle) && num4 > 0)
			{
				ActiveIcons.Insert(num4 - 1, adminIconEntry);
				ActiveIcons.RemoveAt(num4 + 1);
			}
			if (GUI.Button(new Rect(((Rect)(ref windowRect)).width - 33f, num3, 25f, 18f), "v", smallBtnStyle) && num4 < ActiveIcons.Count - 1)
			{
				ActiveIcons.Insert(num4 + 2, adminIconEntry);
				ActiveIcons.RemoveAt(num4);
			}
			if (GUI.Button(new Rect(((Rect)(ref windowRect)).width - 85f, num3, 20f, 18f), "X", dangerBtnStyle))
			{
				ActiveIcons.RemoveAt(num4);
			}
			num3 += 22f;
		}
		GUI.EndScrollView();
		num += num2 + 5f;
		if (ActiveIcons.Count > 0)
		{
			if (GUI.Button(new Rect(5f, num, ((Rect)(ref windowRect)).width - 10f, 22f), "Clear All Icons", dangerBtnStyle))
			{
				ActiveIcons.Clear();
			}
			num += 26f;
		}
		string arg = ((!ServerData.HasConsoleModAccess()) ? "No Access" : (Main.isAdmin ? "Active" : "Moderator"));
		GUI.Label(new Rect(5f, num, ((Rect)(ref windowRect)).width - 10f, 14f), $"Icons: {ActiveIcons.Count}  |  Status: {arg}  |  Stack from top", smallLabelStyle);
		GUI.DragWindow();
	}

	private static void DrawToggle(ref bool value, string label, float x, ref float y)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		float num = 55f;
		float num2 = ((Rect)(ref windowRect)).width - x - num - 25f;
		GUI.Label(new Rect(x, y + 2f, num2, 20f), label, labelStyle);
		GUIStyle val = (value ? toggleOnStyle : toggleOffStyle);
		if (GUI.Button(new Rect(x + num2 + 5f, y, num, 20f), value ? "ON" : "OFF", val))
		{
			value = !value;
		}
		y += 24f;
	}

	private static void AddPresetIcon(string presetName)
	{
		if (PresetIcons.TryGetValue(presetName, out var value))
		{
			string url = "https://raw.githubusercontent.com/1x1x1x1736/Console-/refs/heads/master/ServerData/" + value;
			ActiveIcons.Add(new AdminIconEntry
			{
				name = presetName,
				url = url,
				fileName = value,
				isText = false
			});
		}
	}

	private static void AddCustomIcon(string url)
	{
		string fileName = $"adminicon_{ActiveIcons.Count}.png";
		ActiveIcons.Add(new AdminIconEntry
		{
			name = "Custom",
			url = url,
			fileName = fileName,
			isText = false
		});
		CustomIconUrl = "";
	}

	private static void AddTextIcon(string text)
	{
		ActiveIcons.Add(new AdminIconEntry
		{
			text = text,
			isText = true
		});
		CustomTagText = "";
	}

	public static void UpdateAdminTags()
	{
		if (!PhotonNetwork.InRoom)
		{
			CleanupAll();
		}
		else if (ServerData.HasConsoleModAccess())
		{
			RenderTag();
			RenderIcons();
		}
	}

	private static void RenderTag()
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		if (!ShowAdminTag)
		{
			if (tagObjects.TryGetValue(VRRig.LocalRig, out var value))
			{
				Object.Destroy((Object)(object)value);
				tagObjects.Remove(VRRig.LocalRig);
			}
			return;
		}
		VRRig localRig = VRRig.LocalRig;
		if ((Object)(object)localRig == (Object)null)
		{
			return;
		}
		if (!tagObjects.TryGetValue(localRig, out var value2))
		{
			value2 = new GameObject("MrChicken_AdminTag");
			TextMeshPro val = value2.AddComponent<TextMeshPro>();
			((TMP_Text)val).font = Main.activeFont;
			((TMP_Text)val).fontSize = 4.8f;
			((TMP_Text)val).alignment = (TextAlignmentOptions)514;
			tagObjects.Add(localRig, value2);
			spawnTimes[value2] = Time.time;
			PlayAppearSound();
		}
		TextMeshPro component = value2.GetComponent<TextMeshPro>();
		if ((Object)(object)component != (Object)null)
		{
			((TMP_Text)component).text = "<b>" + AdminTagText + "</b>";
			((Graphic)component).color = AdminTagColor;
			((TMP_Text)component).fontSize = 4.8f * AdminTagScale;
		}
		value2.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f) * localRig.scaleFactor;
		value2.transform.position = localRig.headMesh.transform.position + localRig.headMesh.transform.up * (0.45f * localRig.scaleFactor);
		value2.transform.LookAt(((Component)Camera.main).transform.position);
		value2.transform.Rotate(0f, 180f, 0f);
		if (spawnTimes.TryGetValue(value2, out var value3))
		{
			float num = Time.time - value3;
			if (num < 0.3f)
			{
				float num2 = num / 0.3f;
				float num3 = Mathf.Lerp(0f, 1f, 1f - Mathf.Pow(1f - num2, 3f));
				Transform transform = value2.transform;
				transform.localScale *= num3;
			}
			else
			{
				spawnTimes.Remove(value2);
			}
		}
	}

	private static void RenderIcons()
	{
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Expected O, but got Unknown
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Expected O, but got Unknown
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Expected O, but got Unknown
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		if (!ShowCustomIcons || ActiveIcons.Count == 0)
		{
			if (!iconObjects.TryGetValue(VRRig.LocalRig, out var value))
			{
				return;
			}
			foreach (GameObject item in value)
			{
				Object.Destroy((Object)(object)item);
			}
			iconObjects.Remove(VRRig.LocalRig);
			return;
		}
		VRRig localRig = VRRig.LocalRig;
		if ((Object)(object)localRig == (Object)null)
		{
			return;
		}
		if (!iconObjects.TryGetValue(localRig, out var value2))
		{
			value2 = new List<GameObject>();
			iconObjects.Add(localRig, value2);
		}
		if (value2.Count != ActiveIcons.Count)
		{
			foreach (GameObject item2 in value2)
			{
				Object.Destroy((Object)(object)item2);
			}
			value2.Clear();
			for (int i = 0; i < ActiveIcons.Count; i++)
			{
				AdminIconEntry adminIconEntry = ActiveIcons[i];
				GameObject val;
				if (adminIconEntry.isText)
				{
					val = new GameObject($"MrChicken_AdminIcon_{i}");
					TextMeshPro val2 = val.AddComponent<TextMeshPro>();
					((TMP_Text)val2).font = Main.activeFont;
					((TMP_Text)val2).fontSize = 3.6f;
					((TMP_Text)val2).alignment = (TextAlignmentOptions)514;
					((TMP_Text)val2).text = adminIconEntry.text;
					((Graphic)val2).color = Color.white;
					spawnTimes[val] = Time.time;
				}
				else
				{
					val = GameObject.CreatePrimitive((PrimitiveType)3);
					Object.Destroy((Object)(object)val.GetComponent<Collider>());
					if ((Object)(object)iconMaterial == (Object)null)
					{
						iconMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
						{
							mainTexture = null
						};
						iconMaterial.SetFloat("_Surface", 1f);
						iconMaterial.SetFloat("_Blend", 0f);
						iconMaterial.SetFloat("_SrcBlend", 5f);
						iconMaterial.SetFloat("_DstBlend", 10f);
						iconMaterial.SetFloat("_ZWrite", 0f);
						iconMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
						iconMaterial.renderQueue = 3000;
					}
					val.GetComponent<Renderer>().material = new Material(iconMaterial);
					if (!string.IsNullOrEmpty(adminIconEntry.localPath))
					{
						Texture2D val3 = LocalIcons.LoadTexture(adminIconEntry.localPath);
						if ((Object)(object)val3 != (Object)null)
						{
							val.GetComponent<Renderer>().material.mainTexture = (Texture)(object)val3;
						}
					}
					else if (!string.IsNullOrEmpty(adminIconEntry.url))
					{
						try
						{
							string text = adminIconEntry.cachedTexturePath;
							if (string.IsNullOrEmpty(text))
							{
								text = "SeralythMenu/AdminIcons/" + adminIconEntry.fileName;
								string directoryName = Path.GetDirectoryName(text);
								if (!Directory.Exists(directoryName))
								{
									Directory.CreateDirectory(directoryName);
								}
								if (!File.Exists(text))
								{
									using WebClient webClient = new WebClient();
									webClient.DownloadFile(adminIconEntry.url, text);
								}
								adminIconEntry.cachedTexturePath = text;
							}
							Texture2D val4 = LocalIcons.LoadTexture(text);
							if ((Object)(object)val4 != (Object)null)
							{
								val.GetComponent<Renderer>().material.mainTexture = (Texture)(object)val4;
							}
						}
						catch
						{
						}
					}
				}
				value2.Add(val);
			}
		}
		float num = 0.55f;
		float num2 = 0.35f;
		for (int j = 0; j < value2.Count && j < ActiveIcons.Count; j++)
		{
			GameObject val5 = value2[j];
			if ((Object)(object)val5 == (Object)null)
			{
				continue;
			}
			float num3 = num + (float)j * num2;
			Vector3 localScale = new Vector3(0.3f, 0.3f, 0.01f) * localRig.scaleFactor;
			val5.transform.localScale = localScale;
			val5.transform.position = localRig.headMesh.transform.position + localRig.headMesh.transform.up * (num3 * localRig.scaleFactor);
			val5.transform.LookAt(((Component)Camera.main).transform.position);
			val5.transform.Rotate(0f, 180f, 0f);
			if (spawnTimes.TryGetValue(val5, out var value3))
			{
				float num4 = Time.time - value3;
				if (num4 < 0.3f)
				{
					float num5 = num4 / 0.3f;
					float num6 = Mathf.Lerp(0f, 1f, 1f - Mathf.Pow(1f - num5, 3f));
					Transform transform = val5.transform;
					transform.localScale *= num6;
				}
				else
				{
					spawnTimes.Remove(val5);
				}
			}
		}
	}

	public static void CleanupAll()
	{
		foreach (KeyValuePair<VRRig, GameObject> tagObject in tagObjects)
		{
			if ((Object)(object)tagObject.Value != (Object)null)
			{
				Object.Destroy((Object)(object)tagObject.Value);
			}
		}
		tagObjects.Clear();
		foreach (KeyValuePair<VRRig, List<GameObject>> iconObject in iconObjects)
		{
			foreach (GameObject item in iconObject.Value)
			{
				if ((Object)(object)item != (Object)null)
				{
					Object.Destroy((Object)(object)item);
				}
			}
		}
		iconObjects.Clear();
		spawnTimes.Clear();
	}

	private static void PlayAppearSound()
	{
		try
		{
			if (SoundManager.DefaultSounds.TryGetValue("Select", out var value))
			{
				SoundManager.Play(value, null, delegate(AudioClip clip)
				{
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					AudioSource.PlayClipAtPoint(clip, ((Component)Camera.main).transform.position, 0.3f);
				});
			}
		}
		catch
		{
		}
	}

	private static Texture2D MakeTex(int width, int height, Color color)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Texture2D val = new Texture2D(width, height);
		Color[] array = (Color[])(object)new Color[width * height];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = color;
		}
		val.SetPixels(array);
		val.Apply();
		return val;
	}
}
