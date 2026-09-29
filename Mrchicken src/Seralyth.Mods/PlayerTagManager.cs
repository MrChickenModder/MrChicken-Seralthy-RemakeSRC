using System;
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

public static class PlayerTagManager
{
	[Serializable]
	public class PlayerTagSaveData
	{
		public List<PlayerTagEntry> playerTags = new List<PlayerTagEntry>();
	}

	[Serializable]
	public class PlayerTagEntry
	{
		public string userId;

		public string playerName;

		public string tagText;

		public Color tagColor = Color.yellow;

		public List<PlayerIconData> icons = new List<PlayerIconData>();
	}

	[Serializable]
	public class PlayerIconData
	{
		public string name;

		public string url;

		public string fileName;

		public string text;

		public bool isText;

		public string cachedTexturePath;

		public string localPath;
	}

	private static readonly string SavePath = "SeralythMenu/PlayerTags.json";

	public static readonly List<PlayerTagEntry> Entries = new List<PlayerTagEntry>();

	public static bool IsOpen;

	private static Rect windowRect = new Rect(50f, 50f, 500f, 480f);

	private static Vector2 sidebarScroll;

	private static Vector2 contentScroll;

	private static int selectedCategory;

	private static int selectedEntryIndex = -1;

	private static string inputUserId = "";

	private static string inputPlayerName = "";

	private static string inputTagText = "VIP";

	private static Color inputTagColor = Color.yellow;

	private static string inputIconUrl = "";

	private static string inputIconText = "";

	private static bool showColorPicker;

	private static int localPickerPage;

	private static readonly string[] Categories = new string[2] { "Player Tags", "My Tag" };

	private const float SidebarWidth = 100f;

	private const float CategoryBtnHeight = 30f;

	private static readonly Dictionary<VRRig, GameObject> tagObjects = new Dictionary<VRRig, GameObject>();

	private static readonly Dictionary<VRRig, List<GameObject>> iconObjects = new Dictionary<VRRig, List<GameObject>>();

	private static Material iconMaterial;

	private static readonly Dictionary<GameObject, float> spawnTimes = new Dictionary<GameObject, float>();

	private const float PopDuration = 0.3f;

	private static GUIStyle headerStyle;

	private static GUIStyle sectionStyle;

	private static GUIStyle labelStyle;

	private static GUIStyle smallLabelStyle;

	private static GUIStyle textInputStyle;

	private static GUIStyle smallBtnStyle;

	private static GUIStyle dangerBtnStyle;

	private static GUIStyle accentBtnStyle;

	private static GUIStyle selectBtnStyle;

	private static GUIStyle categoryBtnStyle;

	private static GUIStyle categoryBtnActiveStyle;

	private static bool stylesInit;

	private static readonly Color bgCol = new Color(0.06f, 0.04f, 0.12f, 0.96f);

	private static readonly Color headerCol = new Color(0.2f, 0.1f, 0.35f, 1f);

	private static readonly Color sectionCol = new Color(0.1f, 0.07f, 0.2f, 0.9f);

	private static readonly Color sidebarCol = new Color(0.08f, 0.05f, 0.15f, 0.95f);

	private static readonly Color accentCol = new Color(0.5f, 0.2f, 1f, 1f);

	private static readonly Color textCol = new Color(0.9f, 0.9f, 0.9f, 1f);

	private static readonly Color dimTextCol = new Color(0.55f, 0.55f, 0.65f, 1f);

	private static readonly Color dangerCol = new Color(0.7f, 0.15f, 0.15f, 1f);

	private static readonly Color selectCol = new Color(0.2f, 0.4f, 0.7f, 1f);

	private static readonly Color catActiveCol = new Color(0.4f, 0.15f, 0.8f, 1f);

	private static readonly Color catInactiveCol = new Color(0.15f, 0.1f, 0.25f, 1f);

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

	public static void Toggle()
	{
		IsOpen = !IsOpen;
	}

	public static void DrawGUI()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (IsOpen && PhotonNetwork.InRoom && ServerData.HasConsoleModAccess())
		{
			InitStyles();
			windowRect = GUI.Window(77778, windowRect, new WindowFunction(DrawWindow), "");
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
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected O, but got Unknown
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Expected O, but got Unknown
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Expected O, but got Unknown
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Expected O, but got Unknown
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Expected O, but got Unknown
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Expected O, but got Unknown
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Expected O, but got Unknown
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
			GUIStyle val3 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 11,
				alignment = (TextAnchor)3,
				padding = new RectOffset(4, 0, 2, 0)
			};
			val3.normal.textColor = textCol;
			labelStyle = val3;
			GUIStyle val4 = new GUIStyle(GUI.skin.label)
			{
				fontSize = 10,
				alignment = (TextAnchor)3,
				padding = new RectOffset(8, 0, 0, 0)
			};
			val4.normal.textColor = dimTextCol;
			smallLabelStyle = val4;
			GUIStyle val5 = new GUIStyle(GUI.skin.textField)
			{
				fontSize = 11
			};
			val5.normal.textColor = Color.white;
			val5.normal.background = MakeTex(2, 2, new Color(0.15f, 0.12f, 0.25f, 1f));
			val5.focused.textColor = Color.white;
			val5.focused.background = MakeTex(2, 2, new Color(0.2f, 0.15f, 0.35f, 1f));
			textInputStyle = val5;
			GUIStyle val6 = new GUIStyle(GUI.skin.button)
			{
				fontSize = 10,
				fontStyle = (FontStyle)1
			};
			val6.normal.textColor = Color.white;
			val6.normal.background = MakeTex(2, 2, new Color(0.25f, 0.2f, 0.4f, 1f));
			val6.hover.textColor = Color.white;
			val6.hover.background = MakeTex(2, 2, new Color(0.35f, 0.28f, 0.55f, 1f));
			smallBtnStyle = val6;
			GUIStyle val7 = new GUIStyle(GUI.skin.button)
			{
				fontSize = 10,
				fontStyle = (FontStyle)1
			};
			val7.normal.textColor = Color.white;
			val7.normal.background = MakeTex(2, 2, dangerCol);
			val7.hover.textColor = Color.white;
			val7.hover.background = MakeTex(2, 2, dangerCol * 1.3f);
			dangerBtnStyle = val7;
			GUIStyle val8 = new GUIStyle(GUI.skin.button)
			{
				fontSize = 10,
				fontStyle = (FontStyle)1
			};
			val8.normal.textColor = Color.white;
			val8.normal.background = MakeTex(2, 2, accentCol);
			val8.hover.textColor = Color.white;
			val8.hover.background = MakeTex(2, 2, accentCol * 1.2f);
			accentBtnStyle = val8;
			GUIStyle val9 = new GUIStyle(GUI.skin.button)
			{
				fontSize = 10,
				fontStyle = (FontStyle)1
			};
			val9.normal.textColor = Color.white;
			val9.normal.background = MakeTex(2, 2, selectCol);
			val9.hover.textColor = Color.white;
			val9.hover.background = MakeTex(2, 2, selectCol * 1.2f);
			selectBtnStyle = val9;
			GUIStyle val10 = new GUIStyle(GUI.skin.button)
			{
				fontSize = 11,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			val10.normal.textColor = Color.white;
			val10.normal.background = MakeTex(2, 2, catInactiveCol);
			val10.hover.textColor = Color.white;
			val10.hover.background = MakeTex(2, 2, catInactiveCol * 1.5f);
			categoryBtnStyle = val10;
			GUIStyle val11 = new GUIStyle(GUI.skin.button)
			{
				fontSize = 11,
				fontStyle = (FontStyle)1,
				alignment = (TextAnchor)4
			};
			val11.normal.textColor = Color.white;
			val11.normal.background = MakeTex(2, 2, catActiveCol);
			val11.hover.textColor = Color.white;
			val11.hover.background = MakeTex(2, 2, catActiveCol * 1.2f);
			categoryBtnActiveStyle = val11;
			stylesInit = true;
		}
	}

	private static void DrawWindow(int id)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		GUI.backgroundColor = headerCol;
		GUI.Box(new Rect(0f, 0f, ((Rect)(ref windowRect)).width, 30f), "");
		GUI.backgroundColor = Color.white;
		GUI.Label(new Rect(0f, 5f, ((Rect)(ref windowRect)).width, 20f), "Chicken Menu", headerStyle);
		float num = 5f;
		float num2 = ((Rect)(ref windowRect)).width - 100f - 15f;
		float num3 = 35f;
		float num4 = ((Rect)(ref windowRect)).height - 40f;
		GUI.backgroundColor = sidebarCol;
		GUI.Box(new Rect(((Rect)(ref windowRect)).width - 100f - 5f, 30f, 105f, num4), "");
		GUI.backgroundColor = Color.white;
		float num5 = ((Rect)(ref windowRect)).width - 100f - 3f;
		float num6 = 35f;
		float num7 = num4 - 5f;
		sidebarScroll = GUI.BeginScrollView(new Rect(num5, num6, 100f, num7), sidebarScroll, new Rect(0f, 0f, 90f, (float)Categories.Length * 34f));
		for (int i = 0; i < Categories.Length; i++)
		{
			GUIStyle val = ((i == selectedCategory) ? categoryBtnActiveStyle : categoryBtnStyle);
			if (GUI.Button(new Rect(0f, (float)i * 34f, 90f, 30f), Categories[i], val))
			{
				selectedCategory = i;
			}
		}
		GUI.EndScrollView();
		GUI.backgroundColor = new Color(0.05f, 0.03f, 0.1f, 0.9f);
		GUI.Box(new Rect(num, num3, num2, num4), "");
		GUI.backgroundColor = Color.white;
		contentScroll = GUI.BeginScrollView(new Rect(num, num3, num2, num4), contentScroll, new Rect(0f, 0f, num2 - 10f, 2000f));
		float y = 5f;
		switch (selectedCategory)
		{
		case 0:
			DrawPlayerTagsContent(num2, ref y);
			break;
		case 1:
			DrawMyTagContent(num2, ref y);
			break;
		}
		GUI.EndScrollView();
		GUI.DragWindow();
	}

	private static void DrawPlayerTagsContent(float w, ref float y)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_0553: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce9: Unknown result type (might be due to invalid IL or missing references)
		//IL_062d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e14: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0def: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0701: Unknown result type (might be due to invalid IL or missing references)
		//IL_0932: Unknown result type (might be due to invalid IL or missing references)
		//IL_0969: Unknown result type (might be due to invalid IL or missing references)
		//IL_0991: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b65: Unknown result type (might be due to invalid IL or missing references)
		GUI.backgroundColor = sectionCol;
		GUI.Box(new Rect(5f, y, w - 10f, 20f), "");
		GUI.backgroundColor = Color.white;
		GUI.Label(new Rect(5f, y + 2f, w - 10f, 16f), $"Saved Players ({Entries.Count})", sectionStyle);
		y += 25f;
		float num = Mathf.Min((float)Entries.Count * 24f + 5f, 100f);
		if (Entries.Count > 0)
		{
			GUI.backgroundColor = new Color(0.08f, 0.06f, 0.15f, 0.9f);
			GUI.Box(new Rect(5f, y, w - 10f, num), "");
			GUI.backgroundColor = Color.white;
			float num2 = 2f;
			for (int i = 0; i < Entries.Count; i++)
			{
				PlayerTagEntry playerTagEntry = Entries[i];
				bool flag = i == selectedEntryIndex;
				string text = $"[{playerTagEntry.playerName}] {playerTagEntry.tagText} ({playerTagEntry.icons.Count} icons)";
				GUIStyle val = (flag ? selectBtnStyle : smallBtnStyle);
				if (GUI.Button(new Rect(8f, num2, w - 85f, 20f), text, val))
				{
					selectedEntryIndex = (flag ? (-1) : i);
				}
				if (GUI.Button(new Rect(w - 72f, num2, 30f, 20f), "Edit", smallBtnStyle))
				{
					selectedEntryIndex = i;
					inputUserId = playerTagEntry.userId;
					inputPlayerName = playerTagEntry.playerName;
					inputTagText = playerTagEntry.tagText;
					inputTagColor = playerTagEntry.tagColor;
				}
				if (GUI.Button(new Rect(w - 40f, num2, 30f, 20f), "X", dangerBtnStyle))
				{
					Entries.RemoveAt(i);
					Save();
					if (selectedEntryIndex >= Entries.Count)
					{
						selectedEntryIndex = -1;
					}
					break;
				}
				num2 += 24f;
			}
		}
		y += num + 5f;
		GUI.backgroundColor = sectionCol;
		GUI.Box(new Rect(5f, y, w - 10f, 20f), "");
		GUI.backgroundColor = Color.white;
		GUI.Label(new Rect(5f, y + 2f, w - 10f, 16f), (selectedEntryIndex >= 0) ? "Edit Entry" : "New Entry", sectionStyle);
		y += 25f;
		GUI.Label(new Rect(8f, y + 2f, 70f, 20f), "Player ID:", labelStyle);
		inputUserId = GUI.TextField(new Rect(80f, y, w - 90f, 20f), inputUserId, textInputStyle);
		y += 24f;
		GUI.Label(new Rect(8f, y + 2f, 70f, 20f), "Name:", labelStyle);
		inputPlayerName = GUI.TextField(new Rect(80f, y, w - 90f, 20f), inputPlayerName, textInputStyle);
		y += 24f;
		GUI.Label(new Rect(8f, y + 2f, 70f, 20f), "Tag Text:", labelStyle);
		inputTagText = GUI.TextField(new Rect(80f, y, 140f, 20f), inputTagText, textInputStyle);
		y += 24f;
		GUI.Label(new Rect(8f, y + 2f, 70f, 20f), "Tag Color:", labelStyle);
		if (GUI.Button(new Rect(80f, y, 50f, 20f), "Pick", smallBtnStyle))
		{
			showColorPicker = !showColorPicker;
		}
		GUI.backgroundColor = inputTagColor;
		GUI.Box(new Rect(135f, y, 20f, 20f), "");
		GUI.backgroundColor = Color.white;
		y += 24f;
		if (showColorPicker)
		{
			DrawColorPicker(w, ref y);
		}
		if (selectedEntryIndex >= 0 && selectedEntryIndex < Entries.Count)
		{
			PlayerTagEntry playerTagEntry2 = Entries[selectedEntryIndex];
			GUI.backgroundColor = sectionCol;
			GUI.Box(new Rect(5f, y, w - 10f, 20f), "");
			GUI.backgroundColor = Color.white;
			GUI.Label(new Rect(5f, y + 2f, w - 10f, 16f), $"Icons ({playerTagEntry2.icons.Count})", sectionStyle);
			y += 25f;
			if (GUI.Button(new Rect(8f, y, 80f, 20f), "Crown", accentBtnStyle))
			{
				AddPresetToEntry(playerTagEntry2, "Crown");
			}
			if (GUI.Button(new Rect(93f, y, 80f, 20f), "Star", accentBtnStyle))
			{
				AddPresetToEntry(playerTagEntry2, "Star");
			}
			if (GUI.Button(new Rect(178f, y, 80f, 20f), "Shield", accentBtnStyle))
			{
				AddPresetToEntry(playerTagEntry2, "Shield");
			}
			if (GUI.Button(new Rect(263f, y, 80f, 20f), "Heart", accentBtnStyle))
			{
				AddPresetToEntry(playerTagEntry2, "Heart");
			}
			y += 24f;
			GUI.Label(new Rect(8f, y + 2f, 60f, 20f), "URL:", labelStyle);
			inputIconUrl = GUI.TextField(new Rect(60f, y, w - 160f, 20f), inputIconUrl, textInputStyle);
			if (GUI.Button(new Rect(w - 95f, y, 85f, 20f), "Add URL", accentBtnStyle) && !string.IsNullOrEmpty(inputIconUrl))
			{
				playerTagEntry2.icons.Add(new PlayerIconData
				{
					name = "Custom",
					url = inputIconUrl,
					fileName = $"player_{playerTagEntry2.userId}_{playerTagEntry2.icons.Count}.png",
					isText = false
				});
				Save();
				inputIconUrl = "";
			}
			y += 24f;
			if (ServerData.IsSuperAdmin())
			{
				if (ServerData.IsOwnerLocal())
				{
					GUI.Label(new Rect(8f, y + 2f, 70f, 20f), "Owner:", labelStyle);
					if (GUI.Button(new Rect(70f, y, 80f, 20f), "Chicken", accentBtnStyle))
					{
						string text2 = LocalIcons.EnsureChickenIcon();
						if (!string.IsNullOrEmpty(text2) && selectedEntryIndex >= 0 && selectedEntryIndex < Entries.Count)
						{
							Entries[selectedEntryIndex].icons.Add(new PlayerIconData
							{
								name = "Chicken",
								localPath = text2,
								isText = false
							});
							Save();
						}
					}
					GUI.Label(new Rect(156f, y + 2f, w - 164f, 20f), "Adds the chicken icon", smallLabelStyle);
					y += 24f;
				}
				localPickerPage = LocalIcons.DrawPicker(8f, ref y, w - 18f, localPickerPage, labelStyle, smallLabelStyle, smallBtnStyle, accentBtnStyle, delegate(string file)
				{
					if (selectedEntryIndex >= 0 && selectedEntryIndex < Entries.Count)
					{
						PlayerTagEntry playerTagEntry4 = Entries[selectedEntryIndex];
						playerTagEntry4.icons.Add(new PlayerIconData
						{
							name = Path.GetFileNameWithoutExtension(file),
							localPath = file,
							isText = false
						});
						Save();
					}
				});
			}
			else
			{
				GUI.Label(new Rect(8f, y + 2f, w - 18f, 20f), "Local icons: super admin only", smallLabelStyle);
				y += 24f;
			}
			GUI.Label(new Rect(8f, y + 2f, 60f, 20f), "Text:", labelStyle);
			inputIconText = GUI.TextField(new Rect(60f, y, w - 160f, 20f), inputIconText, textInputStyle);
			if (GUI.Button(new Rect(w - 95f, y, 85f, 20f), "Add Text", accentBtnStyle) && !string.IsNullOrEmpty(inputIconText))
			{
				playerTagEntry2.icons.Add(new PlayerIconData
				{
					text = inputIconText,
					isText = true
				});
				Save();
				inputIconText = "";
			}
			y += 24f;
			if (playerTagEntry2.icons.Count > 0)
			{
				float num3 = Mathf.Min((float)playerTagEntry2.icons.Count * 22f + 5f, 80f);
				GUI.backgroundColor = new Color(0.08f, 0.06f, 0.15f, 0.9f);
				GUI.Box(new Rect(5f, y, w - 10f, num3), "");
				GUI.backgroundColor = Color.white;
				float num4 = 2f;
				for (int num5 = 0; num5 < playerTagEntry2.icons.Count; num5++)
				{
					PlayerIconData playerIconData = playerTagEntry2.icons[num5];
					string text3 = (playerIconData.isText ? ("[T] " + playerIconData.text) : ((!string.IsNullOrEmpty(playerIconData.localPath)) ? ("[F] " + playerIconData.name) : ("[I] " + (playerIconData.name ?? "Custom"))));
					GUI.Label(new Rect(8f, num4, w - 45f, 18f), text3, labelStyle);
					if (GUI.Button(new Rect(w - 38f, num4, 25f, 18f), "X", dangerBtnStyle))
					{
						playerTagEntry2.icons.RemoveAt(num5);
						Save();
						break;
					}
					num4 += 22f;
				}
				y += num3 + 5f;
				if (GUI.Button(new Rect(5f, y, w - 10f, 22f), "Clear All Icons", dangerBtnStyle))
				{
					playerTagEntry2.icons.Clear();
					Save();
				}
				y += 26f;
			}
		}
		if (selectedEntryIndex >= 0)
		{
			if (GUI.Button(new Rect(5f, y, (w - 15f) / 2f, 24f), "Update Entry", accentBtnStyle) && !string.IsNullOrEmpty(inputUserId))
			{
				PlayerTagEntry playerTagEntry3 = Entries[selectedEntryIndex];
				playerTagEntry3.userId = inputUserId;
				playerTagEntry3.playerName = inputPlayerName;
				playerTagEntry3.tagText = inputTagText;
				playerTagEntry3.tagColor = inputTagColor;
				Save();
			}
			if (GUI.Button(new Rect(10f + (w - 15f) / 2f, y, (w - 15f) / 2f, 24f), "Cancel Edit", dangerBtnStyle))
			{
				selectedEntryIndex = -1;
				inputUserId = "";
				inputPlayerName = "";
				inputTagText = "VIP";
				inputTagColor = Color.yellow;
			}
		}
		else if (GUI.Button(new Rect(5f, y, w - 10f, 24f), "Add Player", accentBtnStyle) && !string.IsNullOrEmpty(inputUserId))
		{
			Entries.Add(new PlayerTagEntry
			{
				userId = inputUserId,
				playerName = inputPlayerName,
				tagText = inputTagText,
				tagColor = inputTagColor,
				icons = new List<PlayerIconData>()
			});
			Save();
			inputUserId = "";
			inputPlayerName = "";
			inputTagText = "VIP";
			inputTagColor = Color.yellow;
		}
		y += 28f;
		GUI.Label(new Rect(5f, y, w - 10f, 14f), $"Players: {Entries.Count}  |  Alt+P: Toggle  |  Changes save instantly", smallLabelStyle);
	}

	private static void DrawMyTagContent(float w, ref float y)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_0653: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0793: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_07be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0933: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_083d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0862: Unknown result type (might be due to invalid IL or missing references)
		GUI.backgroundColor = sectionCol;
		GUI.Box(new Rect(5f, y, w - 10f, 20f), "");
		GUI.backgroundColor = Color.white;
		GUI.Label(new Rect(5f, y + 2f, w - 10f, 16f), "Admin Tag", sectionStyle);
		y += 25f;
		DrawToggle(ref AdminTagGUI.ShowAdminTag, "Show Tag Above Head", 8f, ref y, w);
		GUI.Label(new Rect(8f, y + 2f, 80f, 20f), "Tag Text:", labelStyle);
		AdminTagGUI.AdminTagText = GUI.TextField(new Rect(90f, y, 140f, 20f), AdminTagGUI.AdminTagText, textInputStyle);
		y += 24f;
		GUI.Label(new Rect(8f, y + 2f, 80f, 20f), "Scale:", labelStyle);
		AdminTagGUI.AdminTagScale = GUI.HorizontalSlider(new Rect(90f, y + 4f, 140f, 16f), AdminTagGUI.AdminTagScale, 0.5f, 3f);
		GUI.Label(new Rect(235f, y + 2f, 40f, 20f), AdminTagGUI.AdminTagScale.ToString("F1"), smallLabelStyle);
		y += 22f;
		GUI.Label(new Rect(8f, y + 2f, 80f, 20f), "Tag Color:", labelStyle);
		if (GUI.Button(new Rect(90f, y, 50f, 20f), "Pick", smallBtnStyle))
		{
			showColorPicker = !showColorPicker;
		}
		GUI.backgroundColor = AdminTagGUI.AdminTagColor;
		GUI.Box(new Rect(145f, y, 20f, 20f), "");
		GUI.backgroundColor = Color.white;
		y += 24f;
		if (showColorPicker)
		{
			DrawColorPicker(w, ref y);
		}
		y += 5f;
		GUI.backgroundColor = sectionCol;
		GUI.Box(new Rect(5f, y, w - 10f, 20f), "");
		GUI.backgroundColor = Color.white;
		GUI.Label(new Rect(5f, y + 2f, w - 10f, 16f), "Custom Icons", sectionStyle);
		y += 25f;
		DrawToggle(ref AdminTagGUI.ShowCustomIcons, "Show Icons Above Head", 8f, ref y, w);
		GUI.Label(new Rect(8f, y + 2f, 60f, 20f), "Presets:", labelStyle);
		string[] array = PresetIcons.Keys.ToArray();
		if (GUI.Button(new Rect(70f, y, 80f, 20f), "Crown", accentBtnStyle))
		{
			AddPresetIcon("Crown");
		}
		if (GUI.Button(new Rect(155f, y, 80f, 20f), "Star", accentBtnStyle))
		{
			AddPresetIcon("Star");
		}
		y += 24f;
		if (GUI.Button(new Rect(70f, y, 80f, 20f), "Shield", accentBtnStyle))
		{
			AddPresetIcon("Shield");
		}
		if (GUI.Button(new Rect(155f, y, 80f, 20f), "Heart", accentBtnStyle))
		{
			AddPresetIcon("Heart");
		}
		y += 24f;
		GUI.Label(new Rect(8f, y + 2f, 60f, 20f), "URL:", labelStyle);
		AdminTagGUI.CustomIconUrl = GUI.TextField(new Rect(60f, y, w - 160f, 20f), AdminTagGUI.CustomIconUrl, textInputStyle);
		if (GUI.Button(new Rect(w - 95f, y, 85f, 20f), "Add URL", accentBtnStyle) && !string.IsNullOrEmpty(AdminTagGUI.CustomIconUrl))
		{
			AdminTagGUI.ActiveIcons.Add(new AdminTagGUI.AdminIconEntry
			{
				name = "Custom",
				url = AdminTagGUI.CustomIconUrl,
				fileName = $"adminicon_{AdminTagGUI.ActiveIcons.Count}.png",
				isText = false
			});
			AdminTagGUI.CustomIconUrl = "";
		}
		y += 24f;
		if (ServerData.IsSuperAdmin())
		{
			if (ServerData.IsOwnerLocal())
			{
				GUI.Label(new Rect(8f, y + 2f, 70f, 20f), "Owner:", labelStyle);
				if (GUI.Button(new Rect(70f, y, 80f, 20f), "Chicken", accentBtnStyle))
				{
					string text = LocalIcons.EnsureChickenIcon();
					if (!string.IsNullOrEmpty(text))
					{
						AdminTagGUI.ActiveIcons.Add(new AdminTagGUI.AdminIconEntry
						{
							name = "Chicken",
							localPath = text,
							isText = false
						});
					}
				}
				GUI.Label(new Rect(156f, y + 2f, w - 164f, 20f), "Adds the chicken icon", smallLabelStyle);
				y += 24f;
			}
			localPickerPage = LocalIcons.DrawPicker(8f, ref y, w - 18f, localPickerPage, labelStyle, smallLabelStyle, smallBtnStyle, accentBtnStyle, delegate(string file)
			{
				AdminTagGUI.ActiveIcons.Add(new AdminTagGUI.AdminIconEntry
				{
					name = Path.GetFileNameWithoutExtension(file),
					localPath = file,
					isText = false
				});
			});
		}
		else
		{
			GUI.Label(new Rect(8f, y + 2f, w - 18f, 20f), "Local icons: super admin only", smallLabelStyle);
			y += 24f;
		}
		GUI.Label(new Rect(8f, y + 2f, 60f, 20f), "Text:", labelStyle);
		AdminTagGUI.CustomTagText = GUI.TextField(new Rect(60f, y, w - 160f, 20f), AdminTagGUI.CustomTagText, textInputStyle);
		if (GUI.Button(new Rect(w - 95f, y, 85f, 20f), "Add Text", accentBtnStyle) && !string.IsNullOrEmpty(AdminTagGUI.CustomTagText))
		{
			AdminTagGUI.ActiveIcons.Add(new AdminTagGUI.AdminIconEntry
			{
				text = AdminTagGUI.CustomTagText,
				isText = true
			});
			AdminTagGUI.CustomTagText = "";
		}
		y += 24f;
		if (AdminTagGUI.ActiveIcons.Count > 0)
		{
			float num = Mathf.Min((float)AdminTagGUI.ActiveIcons.Count * 22f + 5f, 100f);
			GUI.backgroundColor = new Color(0.08f, 0.06f, 0.15f, 0.9f);
			GUI.Box(new Rect(5f, y, w - 10f, num), "");
			GUI.backgroundColor = Color.white;
			float num2 = 2f;
			for (int num3 = AdminTagGUI.ActiveIcons.Count - 1; num3 >= 0; num3--)
			{
				AdminTagGUI.AdminIconEntry adminIconEntry = AdminTagGUI.ActiveIcons[num3];
				string text2 = (adminIconEntry.isText ? ("[T] " + adminIconEntry.text) : ("[I] " + (adminIconEntry.name ?? "Custom")));
				GUI.Label(new Rect(8f, num2, w - 45f, 18f), text2, labelStyle);
				if (GUI.Button(new Rect(w - 38f, num2, 25f, 18f), "X", dangerBtnStyle))
				{
					AdminTagGUI.ActiveIcons.RemoveAt(num3);
					break;
				}
				num2 += 22f;
			}
			y += num + 5f;
			if (GUI.Button(new Rect(5f, y, w - 10f, 22f), "Clear All Icons", dangerBtnStyle))
			{
				AdminTagGUI.ActiveIcons.Clear();
			}
			y += 26f;
		}
		string arg = ((!ServerData.HasConsoleModAccess()) ? "No Access" : (Main.isAdmin ? "Active" : "Moderator"));
		GUI.Label(new Rect(5f, y, w - 10f, 14f), $"Icons: {AdminTagGUI.ActiveIcons.Count}  |  Status: {arg}", smallLabelStyle);
	}

	private static void DrawColorPicker(float w, ref float y)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		Color val = ((selectedCategory == 1) ? AdminTagGUI.AdminTagColor : inputTagColor);
		GUI.Label(new Rect(16f, y, 30f, 16f), "R:", smallLabelStyle);
		val.r = GUI.HorizontalSlider(new Rect(35f, y, 100f, 16f), val.r, 0f, 1f);
		GUI.Label(new Rect(140f, y, 40f, 16f), val.r.ToString("F2"), smallLabelStyle);
		y += 18f;
		GUI.Label(new Rect(16f, y, 30f, 16f), "G:", smallLabelStyle);
		val.g = GUI.HorizontalSlider(new Rect(35f, y, 100f, 16f), val.g, 0f, 1f);
		GUI.Label(new Rect(140f, y, 40f, 16f), val.g.ToString("F2"), smallLabelStyle);
		y += 18f;
		GUI.Label(new Rect(16f, y, 30f, 16f), "B:", smallLabelStyle);
		val.b = GUI.HorizontalSlider(new Rect(35f, y, 100f, 16f), val.b, 0f, 1f);
		GUI.Label(new Rect(140f, y, 40f, 16f), val.b.ToString("F2"), smallLabelStyle);
		y += 18f;
		GUI.Label(new Rect(16f, y, 30f, 16f), "A:", smallLabelStyle);
		val.a = GUI.HorizontalSlider(new Rect(35f, y, 100f, 16f), val.a, 0f, 1f);
		GUI.Label(new Rect(140f, y, 40f, 16f), val.a.ToString("F2"), smallLabelStyle);
		y += 20f;
		if (selectedCategory == 1)
		{
			AdminTagGUI.AdminTagColor = val;
		}
		else
		{
			inputTagColor = val;
		}
	}

	private static void DrawToggle(ref bool value, string label, float x, ref float y, float w)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		float num = 55f;
		float num2 = w - x - num - 25f;
		GUI.Label(new Rect(x, y + 2f, num2, 20f), label, labelStyle);
		GUIStyle val = (value ? selectBtnStyle : dangerBtnStyle);
		if (GUI.Button(new Rect(x + num2 + 5f, y, num, 20f), value ? "ON" : "OFF", val))
		{
			value = !value;
		}
		y += 24f;
	}

	private static void AddPresetToEntry(PlayerTagEntry entry, string presetName)
	{
		if (PresetIcons.TryGetValue(presetName, out var value))
		{
			entry.icons.Add(new PlayerIconData
			{
				name = presetName,
				url = "https://raw.githubusercontent.com/1x1x1x1736/Console-/refs/heads/master/ServerData/" + value,
				fileName = value,
				isText = false
			});
			Save();
		}
	}

	private static void AddPresetIcon(string presetName)
	{
		if (PresetIcons.TryGetValue(presetName, out var value))
		{
			AdminTagGUI.ActiveIcons.Add(new AdminTagGUI.AdminIconEntry
			{
				name = presetName,
				url = "https://raw.githubusercontent.com/1x1x1x1736/Console-/refs/heads/master/ServerData/" + value,
				fileName = value,
				isText = false
			});
		}
	}

	public static void UpdatePlayerTags()
	{
		if (!PhotonNetwork.InRoom || Entries.Count == 0)
		{
			CleanupAll();
			return;
		}
		IReadOnlyList<VRRig> activeRigs = VRRigCache.ActiveRigs;
		HashSet<VRRig> matchedRigs = new HashSet<VRRig>();
		foreach (PlayerTagEntry entry in Entries)
		{
			foreach (VRRig item in activeRigs)
			{
				if (!((Object)(object)item == (Object)null) && !item.isLocal && item.Creator != null)
				{
					string userId = item.Creator.UserId;
					if (!(userId != entry.userId))
					{
						matchedRigs.Add(item);
						RenderTagOnRig(item, entry);
						RenderIconsOnRig(item, entry);
						break;
					}
				}
			}
		}
		List<VRRig> list = tagObjects.Keys.Where((VRRig r) => !matchedRigs.Contains(r)).ToList();
		foreach (VRRig item2 in list)
		{
			if (tagObjects.TryGetValue(item2, out var value) && (Object)(object)value != (Object)null)
			{
				Object.Destroy((Object)(object)value);
			}
			tagObjects.Remove(item2);
		}
		List<VRRig> list2 = iconObjects.Keys.Where((VRRig r) => !matchedRigs.Contains(r)).ToList();
		foreach (VRRig item3 in list2)
		{
			if (iconObjects.TryGetValue(item3, out var value2))
			{
				foreach (GameObject item4 in value2)
				{
					if ((Object)(object)item4 != (Object)null)
					{
						Object.Destroy((Object)(object)item4);
					}
				}
			}
			iconObjects.Remove(item3);
		}
	}

	private static void RenderTagOnRig(VRRig rig, PlayerTagEntry entry)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(entry.tagText))
		{
			if (tagObjects.TryGetValue(rig, out var value))
			{
				Object.Destroy((Object)(object)value);
				tagObjects.Remove(rig);
			}
			return;
		}
		if (!tagObjects.TryGetValue(rig, out var value2))
		{
			value2 = new GameObject("PlayerTag_" + entry.userId);
			TextMeshPro val = value2.AddComponent<TextMeshPro>();
			((TMP_Text)val).font = Main.activeFont;
			((TMP_Text)val).fontSize = 4.8f;
			((TMP_Text)val).alignment = (TextAlignmentOptions)514;
			tagObjects.Add(rig, value2);
			spawnTimes[value2] = Time.time;
			PlayAppearSound();
		}
		TextMeshPro component = value2.GetComponent<TextMeshPro>();
		if ((Object)(object)component != (Object)null)
		{
			((TMP_Text)component).text = "<b>" + entry.tagText + "</b>";
			((Graphic)component).color = entry.tagColor;
			((TMP_Text)component).fontSize = 4.8f;
		}
		value2.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f) * rig.scaleFactor;
		value2.transform.position = rig.headMesh.transform.position + rig.headMesh.transform.up * (0.45f * rig.scaleFactor);
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

	private static void RenderIconsOnRig(VRRig rig, PlayerTagEntry entry)
	{
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Expected O, but got Unknown
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Expected O, but got Unknown
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Expected O, but got Unknown
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		if (entry.icons.Count == 0)
		{
			if (!iconObjects.TryGetValue(rig, out var value))
			{
				return;
			}
			foreach (GameObject item in value)
			{
				Object.Destroy((Object)(object)item);
			}
			iconObjects.Remove(rig);
			return;
		}
		if (!iconObjects.TryGetValue(rig, out var value2))
		{
			value2 = new List<GameObject>();
			iconObjects.Add(rig, value2);
		}
		if (value2.Count != entry.icons.Count)
		{
			foreach (GameObject item2 in value2)
			{
				Object.Destroy((Object)(object)item2);
			}
			value2.Clear();
			for (int i = 0; i < entry.icons.Count; i++)
			{
				PlayerIconData playerIconData = entry.icons[i];
				GameObject val;
				if (playerIconData.isText)
				{
					val = new GameObject($"PlayerIcon_{entry.userId}_{i}");
					TextMeshPro val2 = val.AddComponent<TextMeshPro>();
					((TMP_Text)val2).font = Main.activeFont;
					((TMP_Text)val2).fontSize = 3.6f;
					((TMP_Text)val2).alignment = (TextAlignmentOptions)514;
					((TMP_Text)val2).text = playerIconData.text;
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
					if (!string.IsNullOrEmpty(playerIconData.localPath))
					{
						Texture2D val3 = LocalIcons.LoadTexture(playerIconData.localPath);
						if ((Object)(object)val3 != (Object)null)
						{
							val.GetComponent<Renderer>().material.mainTexture = (Texture)(object)val3;
						}
					}
					else if (!string.IsNullOrEmpty(playerIconData.url))
					{
						try
						{
							string text = playerIconData.cachedTexturePath;
							if (string.IsNullOrEmpty(text))
							{
								text = "SeralythMenu/PlayerIcons/" + playerIconData.fileName;
								string directoryName = Path.GetDirectoryName(text);
								if (!Directory.Exists(directoryName))
								{
									Directory.CreateDirectory(directoryName);
								}
								if (!File.Exists(text))
								{
									using WebClient webClient = new WebClient();
									webClient.DownloadFile(playerIconData.url, text);
								}
								playerIconData.cachedTexturePath = text;
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
		for (int j = 0; j < value2.Count && j < entry.icons.Count; j++)
		{
			GameObject val5 = value2[j];
			if ((Object)(object)val5 == (Object)null)
			{
				continue;
			}
			float num3 = num + (float)j * num2;
			Vector3 localScale = new Vector3(0.3f, 0.3f, 0.01f) * rig.scaleFactor;
			val5.transform.localScale = localScale;
			val5.transform.position = rig.headMesh.transform.position + rig.headMesh.transform.up * (num3 * rig.scaleFactor);
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

	private static void Save()
	{
		try
		{
			PlayerTagSaveData playerTagSaveData = new PlayerTagSaveData
			{
				playerTags = Entries
			};
			string contents = JsonUtility.ToJson((object)playerTagSaveData, true);
			string directoryName = Path.GetDirectoryName(SavePath);
			if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			File.WriteAllText(SavePath, contents);
		}
		catch (Exception ex)
		{
			Seralyth.Classes.Menu.Console.Log("[PlayerTagManager] Save error: " + ex.Message);
		}
	}

	public static void Load()
	{
		try
		{
			if (File.Exists(SavePath))
			{
				string text = File.ReadAllText(SavePath);
				PlayerTagSaveData playerTagSaveData = JsonUtility.FromJson<PlayerTagSaveData>(text);
				if (playerTagSaveData?.playerTags != null)
				{
					Entries.Clear();
					Entries.AddRange(playerTagSaveData.playerTags);
				}
			}
		}
		catch (Exception ex)
		{
			Seralyth.Classes.Menu.Console.Log("[PlayerTagManager] Load error: " + ex.Message);
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
