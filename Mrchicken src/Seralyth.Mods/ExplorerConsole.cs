using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Seralyth.Classes.Menu;
using UnityEngine;

namespace Seralyth.Mods;

public static class ExplorerConsole
{
	public static bool IsOpen;

	public static bool ShowCrowns = true;

	public static bool ShowNameLabels = true;

	private static Rect windowRect = new Rect(20f, 20f, 340f, 420f);

	private static Vector2 adminScroll;

	private static Vector2 userScroll;

	private static GUIStyle headerStyle;

	private static GUIStyle sectionStyle;

	private static GUIStyle toggleOnStyle;

	private static GUIStyle toggleOffStyle;

	private static GUIStyle labelStyle;

	private static GUIStyle smallLabelStyle;

	private static GUIStyle boxStyle;

	private static bool stylesInit;

	private static readonly Color bgCol = new Color(0.08f, 0.06f, 0.15f, 0.95f);

	private static readonly Color headerCol = new Color(0.17f, 0.15f, 0.36f, 1f);

	private static readonly Color sectionCol = new Color(0.12f, 0.1f, 0.25f, 0.9f);

	private static readonly Color toggleOnCol = new Color(0.13f, 0.55f, 0.13f, 1f);

	private static readonly Color toggleOffCol = new Color(0.55f, 0.13f, 0.13f, 1f);

	private static readonly Color textCol = new Color(0.9f, 0.9f, 0.9f, 1f);

	private static readonly Color dimTextCol = new Color(0.6f, 0.6f, 0.7f, 1f);

	public static void Toggle()
	{
		IsOpen = !IsOpen;
		Cursor.lockState = (CursorLockMode)(!IsOpen);
	}

	public static void DrawExplorerConsole()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (IsOpen && PhotonNetwork.InRoom)
		{
			InitStyles();
			windowRect = GUI.Window(88888, windowRect, new WindowFunction(DrawWindow), "");
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
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Expected O, but got Unknown
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
			val2.normal.textColor = new Color(0.7f, 0.65f, 1f);
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
			GUIStyle val7 = new GUIStyle(GUI.skin.box);
			val7.normal.background = MakeTex(2, 2, bgCol);
			boxStyle = val7;
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
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		float num = 5f;
		GUI.backgroundColor = headerCol;
		GUI.Box(new Rect(0f, 0f, ((Rect)(ref windowRect)).width, 30f), "");
		GUI.backgroundColor = Color.white;
		GUI.Label(new Rect(0f, 5f, ((Rect)(ref windowRect)).width, 20f), "Explorer Console  |  F2 to toggle", headerStyle);
		num = 35f;
		GUI.backgroundColor = sectionCol;
		GUI.Box(new Rect(5f, num, ((Rect)(ref windowRect)).width - 10f, 20f), "");
		GUI.backgroundColor = Color.white;
		GUI.Label(new Rect(5f, num + 2f, ((Rect)(ref windowRect)).width - 10f, 16f), "Display", sectionStyle);
		num += 25f;
		DrawToggle(ref ShowCrowns, "Admin Crowns", 8f, ref num);
		DrawToggle(ref ShowNameLabels, "Admin Name Labels", 8f, ref num);
		num += 5f;
		GUI.backgroundColor = sectionCol;
		GUI.Box(new Rect(5f, num, ((Rect)(ref windowRect)).width - 10f, 20f), "");
		GUI.backgroundColor = Color.white;
		GUI.Label(new Rect(5f, num + 2f, ((Rect)(ref windowRect)).width - 10f, 16f), "Admins in Room", sectionStyle);
		num += 25f;
		float num2 = 120f;
		GUI.backgroundColor = new Color(0.1f, 0.08f, 0.18f, 0.9f);
		GUI.Box(new Rect(5f, num, ((Rect)(ref windowRect)).width - 10f, num2), "");
		GUI.backgroundColor = Color.white;
		adminScroll = GUI.BeginScrollView(new Rect(5f, num, ((Rect)(ref windowRect)).width - 10f, num2), adminScroll, new Rect(0f, 0f, ((Rect)(ref windowRect)).width - 30f, (float)GetAdminCount() * 18f));
		float num3 = 2f;
		Player[] playerList = PhotonNetwork.PlayerList;
		foreach (Player val in playerList)
		{
			if (ServerData.Administrators.TryGetValue(ServerData.NormalizeId(val.UserId), out var value))
			{
				string text = (ServerData.SuperAdministrators.Contains(value) ? "<color=yellow>[SA]</color> " : "<color=purple>[A]</color> ");
				GUI.Label(new Rect(5f, num3, 280f, 16f), text + val.NickName + " - " + value, labelStyle);
				num3 += 18f;
			}
		}
		GUI.EndScrollView();
		num += num2 + 5f;
		float num4 = ((Rect)(ref windowRect)).width - 10f;
		if (GUI.Button(new Rect(5f, num, num4, 24f), "Notify Presence"))
		{
			Console.ExecuteCommand("notify", (ReceiverGroup)1, "Admin " + PhotonNetwork.LocalPlayer.NickName + " is in the lobby!");
		}
		num += 30f;
		if (GUI.Button(new Rect(5f, num, num4, 24f), "Scan for Users"))
		{
			Experimental.GetMenuUsers();
		}
		num += 30f;
		GUI.Label(new Rect(5f, num, ((Rect)(ref windowRect)).width - 10f, 14f), $"Players: {PhotonNetwork.PlayerList.Length}  |  Admins: {GetAdminCount()}", smallLabelStyle);
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

	private static int GetAdminCount()
	{
		return PhotonNetwork.PlayerList.Count((Player p) => ServerData.Administrators.ContainsKey(ServerData.NormalizeId(p.UserId)));
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
