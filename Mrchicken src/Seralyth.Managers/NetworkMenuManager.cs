using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Seralyth.Classes.Menu;
using Seralyth.Menu;
using UnityEngine;

namespace Seralyth.Managers;

public class NetworkMenuManager : MonoBehaviour
{
	public class CachedTheme
	{
		public ExtGradient menuBgGradient;

		public ExtGradient btnGradient0;

		public ExtGradient btnGradient1;

		public ExtGradient txtGradient0;

		public ExtGradient txtGradient1;

		public ExtGradient txtGradient2;

		public Color playerColor;

		public bool thinMenu;

		public bool swapButtonColors;

		public bool slowFadeColors;
	}

	public class RemoteMenuState
	{
		public Player player;

		public GameObject displayObject;

		public string category = "Main";

		public int page;

		public Vector3 position;

		public Quaternion rotation;

		public Dictionary<string, bool> buttonStates = new Dictionary<string, bool>();

		public float lastStateTime;

		public bool closing;

		public VRRig cachedRig;

		public Vector3 rigOffset;

		public Color menuBgColor;

		public Color btnColor0;

		public Color btnColor1;

		public Color textColor0;

		public Color textColor1;

		public Color textColor2;

		public Color playerColor;

		public bool thinMenu;

		public bool swapButtonColors;

		public ExtGradient menuBgGradient;

		public ExtGradient btnGradient0;

		public ExtGradient btnGradient1;

		public ExtGradient txtGradient0;

		public ExtGradient txtGradient1;

		public ExtGradient txtGradient2;

		public bool slowFadeColors;

		public void RefreshGradientColors()
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			float time = Time.time / (slowFadeColors ? 10f : 2f) % 1f;
			if (menuBgGradient != null)
			{
				menuBgColor = menuBgGradient.GetColorTime(time);
			}
			if (btnGradient0 != null)
			{
				btnColor0 = btnGradient0.GetColorTime(time);
			}
			if (btnGradient1 != null)
			{
				btnColor1 = btnGradient1.GetColorTime(time);
			}
			if (txtGradient0 != null)
			{
				textColor0 = txtGradient0.GetColorTime(time);
			}
			if (txtGradient1 != null)
			{
				textColor1 = txtGradient1.GetColorTime(time);
			}
			if (txtGradient2 != null)
			{
				textColor2 = txtGradient2.GetColorTime(time);
			}
		}
	}

	public static NetworkMenuManager instance;

	public const byte NetworkMenuByte = 71;

	private const string CustomPropertyKey = "Seralyth NetworkMenu";

	private static float syncTimer;

	private static float heartbeatTimer;

	private static float themeTimer;

	private static float gunSyncTimer;

	private static readonly Dictionary<int, RemoteMenuState> remoteMenus = new Dictionary<int, RemoteMenuState>();

	private static readonly Dictionary<int, CachedTheme> cachedThemes = new Dictionary<int, CachedTheme>();

	private void Awake()
	{
		instance = this;
		PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
	}

	private void OnDestroy()
	{
		PhotonNetwork.NetworkingClient.EventReceived -= OnEventReceived;
	}

	public static void EnableNetworkMenu()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0019: Expected O, but got Unknown
		Hashtable val = new Hashtable();
		((Dictionary<object, object>)val).Add((object)"Seralyth NetworkMenu", (object)true);
		Hashtable val2 = val;
		if (PhotonNetwork.LocalPlayer != null)
		{
			PhotonNetwork.LocalPlayer.SetCustomProperties(val2, (Hashtable)null, (WebFlags)null);
		}
		SendThemeState();
	}

	public static void DisableNetworkMenu()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_001f: Expected O, but got Unknown
		SendMenuClose();
		Hashtable val = new Hashtable();
		((Dictionary<object, object>)val).Add((object)"Seralyth NetworkMenu", (object)false);
		Hashtable val2 = val;
		if (PhotonNetwork.LocalPlayer != null)
		{
			PhotonNetwork.LocalPlayer.SetCustomProperties(val2, (Hashtable)null, (WebFlags)null);
		}
	}

	private static string GetCurrentCategoryName()
	{
		return Buttons.CurrentCategoryName;
	}

	private static int GetCurrentPage()
	{
		return Main.pageNumber;
	}

	private static Vector3 GetMenuPosition()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)Main.menu != (Object)null)
		{
			return Main.menu.transform.position;
		}
		if (Main.rightHand)
		{
			return GorillaTagger.Instance.rightHandTransform.position;
		}
		return GorillaTagger.Instance.leftHandTransform.position;
	}

	private static Quaternion GetMenuRotation()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)Main.menu != (Object)null)
		{
			return Main.menu.transform.rotation;
		}
		if (Main.rightHand)
		{
			return GorillaTagger.Instance.rightHandTransform.rotation;
		}
		return GorillaTagger.Instance.leftHandTransform.rotation;
	}

	private static int ColorToPacked(Color c)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		int num = Mathf.Clamp(Mathf.RoundToInt(c.r * 255f), 0, 255);
		int num2 = Mathf.Clamp(Mathf.RoundToInt(c.g * 255f), 0, 255);
		int num3 = Mathf.Clamp(Mathf.RoundToInt(c.b * 255f), 0, 255);
		return (num << 16) | (num2 << 8) | num3;
	}

	private static Color PackedToColor(int packed)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)((packed >> 16) & 0xFF) / 255f;
		float num2 = (float)((packed >> 8) & 0xFF) / 255f;
		float num3 = (float)(packed & 0xFF) / 255f;
		return new Color(num, num2, num3);
	}

	private static void SerializeGradient(object[] args, ref int idx, ExtGradient grad)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		if (grad.rainbow)
		{
			num |= 1;
		}
		if (grad.pastelRainbow)
		{
			num |= 2;
		}
		if (grad.epileptic)
		{
			num |= 4;
		}
		if (grad.copyRigColor)
		{
			num |= 8;
		}
		if (grad.transparent)
		{
			num |= 0x10;
		}
		GradientColorKey[] array = grad.colors ?? ExtGradient.GetSolidGradient(Color.magenta);
		int num2 = Mathf.Clamp(array.Length, 2, 3);
		num |= num2 << 5;
		args[idx++] = num;
		for (int i = 0; i < num2; i++)
		{
			args[idx++] = ColorToPacked(array[i].color);
			args[idx++] = array[i].time;
		}
		if (num2 < 3)
		{
			Color color = array[num2 - 1].color;
			float time = array[num2 - 1].time;
			for (int j = num2; j < 3; j++)
			{
				args[idx++] = ColorToPacked(color);
				args[idx++] = time;
			}
		}
	}

	private static ExtGradient DeserializeGradient(object[] args, ref int idx)
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		int num = Convert.ToInt32(args[idx++]);
		bool rainbow = (num & 1) != 0;
		bool pastelRainbow = (num & 2) != 0;
		bool epileptic = (num & 4) != 0;
		bool copyRigColor = (num & 8) != 0;
		bool transparent = (num & 0x10) != 0;
		int num2 = (num >> 5) & 3;
		if (num2 < 2)
		{
			num2 = 2;
		}
		GradientColorKey[] array = (GradientColorKey[])(object)new GradientColorKey[num2];
		float[] array2 = ((num2 != 2) ? new float[3] { 0f, 0.5f, 1f } : new float[2] { 0f, 1f });
		for (int i = 0; i < num2; i++)
		{
			Color val = PackedToColor(Convert.ToInt32(args[idx++]));
			float num3 = Convert.ToSingle(args[idx++]);
			array[i] = new GradientColorKey(val, array2[i]);
		}
		idx += (3 - num2) * 2;
		return new ExtGradient
		{
			colors = array,
			rainbow = rainbow,
			pastelRainbow = pastelRainbow,
			epileptic = epileptic,
			copyRigColor = copyRigColor,
			transparent = transparent
		};
	}

	public static void SendMenuState()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Expected O, but got Unknown
		if (!Main.networkMenuEnabled || !PhotonNetwork.InRoom)
		{
			return;
		}
		string currentCategoryName = GetCurrentCategoryName();
		int currentPage = GetCurrentPage();
		Vector3 val = ((!((Object)(object)VRRig.LocalRig != (Object)null)) ? GetMenuPosition() : (GetMenuPosition() - ((Component)VRRig.LocalRig).transform.position));
		Quaternion menuRotation = GetMenuRotation();
		List<string> list = new List<string>();
		ButtonInfo[][] buttons = Buttons.buttons;
		foreach (ButtonInfo[] array in buttons)
		{
			if (array == null)
			{
				continue;
			}
			ButtonInfo[] array2 = array;
			foreach (ButtonInfo buttonInfo in array2)
			{
				if (buttonInfo.isTogglable && buttonInfo.enabled)
				{
					list.Add(buttonInfo.buttonText);
				}
			}
		}
		object[] array3 = new object[11]
		{
			"seralyth_netmenu_state",
			currentCategoryName,
			currentPage,
			val.x,
			val.y,
			val.z,
			menuRotation.x,
			menuRotation.y,
			menuRotation.z,
			menuRotation.w,
			list.ToArray()
		};
		PhotonNetwork.RaiseEvent((byte)71, (object)array3, new RaiseEventOptions
		{
			Receivers = (ReceiverGroup)0
		}, SendOptions.SendReliable);
	}

	private static void SendThemeState()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Expected O, but got Unknown
		if (PhotonNetwork.InRoom && Main.networkMenuEnabled)
		{
			Color c = Color.white;
			if ((Object)(object)VRRig.LocalRig != (Object)null)
			{
				c = VRRig.LocalRig.playerColor;
			}
			int num = (Main.thinMenu ? 1 : 0) | (Main.swapButtonColors ? 2 : 0) | (Main.slowFadeColors ? 4 : 0);
			object[] array = new object[51];
			array[0] = "seralyth_netmenu_theme_v2";
			array[1] = num;
			array[2] = ColorToPacked(c);
			int idx = 3;
			SerializeGradient(array, ref idx, Main.menuBackgroundColor);
			SerializeGradient(array, ref idx, Main.buttonColors[0]);
			SerializeGradient(array, ref idx, Main.buttonColors[1]);
			SerializeGradient(array, ref idx, Main.textColors[0]);
			SerializeGradient(array, ref idx, Main.textColors[1]);
			SerializeGradient(array, ref idx, Main.textColors[2]);
			PhotonNetwork.RaiseEvent((byte)71, (object)array, new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)0
			}, SendOptions.SendReliable);
		}
	}

	private static void SendMenuClose()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		if (Main.networkMenuEnabled && PhotonNetwork.InRoom)
		{
			object[] array = new object[1] { "seralyth_netmenu_close" };
			PhotonNetwork.RaiseEvent((byte)71, (object)array, new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)0
			}, SendOptions.SendReliable);
		}
	}

	private static void SendMenuHeartbeat()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		if (Main.networkMenuEnabled && PhotonNetwork.InRoom)
		{
			object[] array = new object[1] { "seralyth_netmenu_heartbeat" };
			PhotonNetwork.RaiseEvent((byte)71, (object)array, new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)0
			}, SendOptions.SendUnreliable);
		}
	}

	private void OnEventReceived(EventData data)
	{
		if (data.Code != 71)
		{
			return;
		}
		try
		{
			Room currentRoom = PhotonNetwork.CurrentRoom;
			Player val = ((currentRoom != null) ? currentRoom.GetPlayer(data.Sender, false) : null);
			if (val == null || val == PhotonNetwork.LocalPlayer || !(data.CustomData is object[] array) || array.Length < 1)
			{
				return;
			}
			string text = array[0] as string;
			if (!string.IsNullOrEmpty(text))
			{
				switch (text)
				{
				case "seralyth_netmenu_state":
					HandleRemoteMenuState(val, array);
					break;
				case "seralyth_netmenu_close":
					HandleRemoteMenuClose(val);
					break;
				case "seralyth_netmenu_heartbeat":
					HandleRemoteHeartbeat(val);
					break;
				case "seralyth_netmenu_theme_v2":
					HandleRemoteThemeV2(val, array);
					break;
				case "seralyth_netmenu_theme":
					HandleRemoteThemeLegacy(val, array);
					break;
				case "seralyth_netmenu_gundata":
					GunLib.HandleRemoteGunData(val, array);
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	private static void HandleRemoteMenuState(Player sender, object[] args)
	{
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05df: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		//IL_060a: Unknown result type (might be due to invalid IL or missing references)
		//IL_060c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0640: Unknown result type (might be due to invalid IL or missing references)
		//IL_0645: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		if (args.Length < 11)
		{
			return;
		}
		string text = (args[1] as string) ?? "Main";
		int num = Convert.ToInt32(args[2]);
		if (num < 0 || num > 100)
		{
			return;
		}
		object obj = args[3];
		float num2;
		if (obj is float)
		{
			num2 = (float)obj;
			if (true)
			{
				goto IL_006e;
			}
		}
		num2 = 0f;
		goto IL_006e;
		IL_00c2:
		float num3;
		float num4;
		if (float.IsNaN(num2) || float.IsInfinity(num2) || float.IsNaN(num3) || float.IsInfinity(num3) || float.IsNaN(num4) || float.IsInfinity(num4))
		{
			return;
		}
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(num2, num3, num4);
		Quaternion identity = Quaternion.identity;
		float num5 = 0f;
		float num6 = 0f;
		float num7 = 0f;
		float num8 = 1f;
		if (args[6] is float num9)
		{
			num5 = num9;
		}
		if (args[7] is float num10)
		{
			num6 = num10;
		}
		if (args[8] is float num11)
		{
			num7 = num11;
		}
		if (args[9] is float num12)
		{
			num8 = num12;
		}
		if (float.IsNaN(num5) || float.IsInfinity(num5))
		{
			return;
		}
		((Quaternion)(ref identity))._002Ector(num5, num6, num7, num8);
		HashSet<string> hashSet = new HashSet<string>();
		if (args[10] is string[] array)
		{
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				if (!string.IsNullOrEmpty(text2))
				{
					hashSet.Add(text2);
				}
			}
		}
		Color val2 = default(Color);
		((Color)(ref val2))._002Ector(0.086f, 0.086f, 0.086f, 0.5f);
		Color val3 = default(Color);
		((Color)(ref val3))._002Ector(0.463f, 0.024f, 0.988f, 1f);
		Color val4 = default(Color);
		((Color)(ref val4))._002Ector(0.345f, 0.024f, 0.729f, 1f);
		Color white = Color.white;
		Color menuBgColor = val2;
		Color btnColor = val3;
		Color btnColor2 = val4;
		Color textColor = white;
		Color textColor2 = white;
		Color textColor3 = white;
		bool thinMenu = true;
		bool swapButtonColors = false;
		if (cachedThemes.TryGetValue(sender.ActorNumber, out var value))
		{
			menuBgColor = ((value.menuBgGradient != null) ? value.menuBgGradient.GetColorTime(Time.time / (value.slowFadeColors ? 10f : 2f) % 1f) : val2);
			btnColor = ((value.btnGradient0 != null) ? value.btnGradient0.GetColorTime(Time.time / (value.slowFadeColors ? 10f : 2f) % 1f) : val3);
			btnColor2 = ((value.btnGradient1 != null) ? value.btnGradient1.GetColorTime(Time.time / (value.slowFadeColors ? 10f : 2f) % 1f) : val4);
			textColor = ((value.txtGradient0 != null) ? value.txtGradient0.GetColorTime(Time.time / (value.slowFadeColors ? 10f : 2f) % 1f) : white);
			textColor2 = ((value.txtGradient1 != null) ? value.txtGradient1.GetColorTime(Time.time / (value.slowFadeColors ? 10f : 2f) % 1f) : white);
			textColor3 = ((value.txtGradient2 != null) ? value.txtGradient2.GetColorTime(Time.time / (value.slowFadeColors ? 10f : 2f) % 1f) : white);
			thinMenu = value.thinMenu;
			swapButtonColors = value.swapButtonColors;
		}
		Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
		ButtonInfo[][] buttons = Buttons.buttons;
		foreach (ButtonInfo[] array3 in buttons)
		{
			if (array3 == null)
			{
				continue;
			}
			ButtonInfo[] array4 = array3;
			foreach (ButtonInfo buttonInfo in array4)
			{
				if (buttonInfo.isTogglable)
				{
					dictionary[buttonInfo.buttonText] = hashSet.Contains(buttonInfo.buttonText);
				}
			}
		}
		int actorNumber = sender.ActorNumber;
		RemoteMenuState value2;
		bool flag = !remoteMenus.TryGetValue(actorNumber, out value2);
		if (flag)
		{
			value2 = new RemoteMenuState
			{
				player = sender
			};
			remoteMenus[actorNumber] = value2;
		}
		bool flag2 = !flag && (value2.page != num || value2.category != text);
		value2.category = text;
		value2.page = num;
		VRRig val5 = (value2.cachedRig = GorillaGameManager.StaticFindRigForPlayer(NetPlayer.op_Implicit(sender)));
		if ((Object)(object)val5 != (Object)null)
		{
			value2.rigOffset = val;
			value2.position = ((Component)val5).transform.position + val;
		}
		else
		{
			value2.position = val;
		}
		value2.rotation = identity;
		value2.buttonStates = dictionary;
		value2.lastStateTime = Time.time;
		value2.menuBgColor = menuBgColor;
		value2.btnColor0 = btnColor;
		value2.btnColor1 = btnColor2;
		value2.textColor0 = textColor;
		value2.textColor1 = textColor2;
		value2.textColor2 = textColor3;
		value2.thinMenu = thinMenu;
		value2.swapButtonColors = swapButtonColors;
		if (cachedThemes.TryGetValue(sender.ActorNumber, out var value3))
		{
			value2.playerColor = value3.playerColor;
			value2.menuBgGradient = value3.menuBgGradient;
			value2.btnGradient0 = value3.btnGradient0;
			value2.btnGradient1 = value3.btnGradient1;
			value2.txtGradient0 = value3.txtGradient0;
			value2.txtGradient1 = value3.txtGradient1;
			value2.txtGradient2 = value3.txtGradient2;
			value2.slowFadeColors = value3.slowFadeColors;
		}
		if ((Object)(object)value2.displayObject == (Object)null)
		{
			NetworkMenuDisplay.Create(value2);
		}
		else if (flag2)
		{
			NetworkMenuDisplay.UpdateState(value2);
		}
		else
		{
			NetworkMenuDisplay.UpdateColors(value2);
		}
		NetworkMenuDisplay.UpdatePosition(value2);
		return;
		IL_0097:
		obj = args[5];
		if (obj is float)
		{
			num4 = (float)obj;
			if (true)
			{
				goto IL_00c2;
			}
		}
		num4 = 0f;
		goto IL_00c2;
		IL_006e:
		obj = args[4];
		if (obj is float)
		{
			num3 = (float)obj;
			if (true)
			{
				goto IL_0097;
			}
		}
		num3 = 0f;
		goto IL_0097;
	}

	private static void HandleRemoteMenuClose(Player sender)
	{
		int actorNumber = sender.ActorNumber;
		if (remoteMenus.TryGetValue(actorNumber, out var value) && (Object)(object)value.displayObject != (Object)null && !value.closing)
		{
			NetworkMenuDisplay.CloseAndDestroy(value);
		}
	}

	private static void HandleRemoteHeartbeat(Player sender)
	{
		if (remoteMenus.TryGetValue(sender.ActorNumber, out var value))
		{
			value.lastStateTime = Time.time;
		}
	}

	private static void HandleRemoteThemeV2(Player sender, object[] args)
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		if (args.Length >= 51)
		{
			int num = Convert.ToInt32(args[1]);
			int packed = Convert.ToInt32(args[2]);
			int idx = 3;
			ExtGradient menuBgGradient = DeserializeGradient(args, ref idx);
			ExtGradient btnGradient = DeserializeGradient(args, ref idx);
			ExtGradient btnGradient2 = DeserializeGradient(args, ref idx);
			ExtGradient txtGradient = DeserializeGradient(args, ref idx);
			ExtGradient txtGradient2 = DeserializeGradient(args, ref idx);
			ExtGradient txtGradient3 = DeserializeGradient(args, ref idx);
			cachedThemes[sender.ActorNumber] = new CachedTheme
			{
				menuBgGradient = menuBgGradient,
				btnGradient0 = btnGradient,
				btnGradient1 = btnGradient2,
				txtGradient0 = txtGradient,
				txtGradient1 = txtGradient2,
				txtGradient2 = txtGradient3,
				playerColor = PackedToColor(packed),
				thinMenu = ((num & 1) != 0),
				swapButtonColors = ((num & 2) != 0),
				slowFadeColors = ((num & 4) != 0)
			};
			if (remoteMenus.TryGetValue(sender.ActorNumber, out var value))
			{
				CachedTheme cachedTheme = cachedThemes[sender.ActorNumber];
				value.playerColor = cachedTheme.playerColor;
				value.thinMenu = cachedTheme.thinMenu;
				value.swapButtonColors = cachedTheme.swapButtonColors;
				value.slowFadeColors = cachedTheme.slowFadeColors;
				value.menuBgGradient = cachedTheme.menuBgGradient;
				value.btnGradient0 = cachedTheme.btnGradient0;
				value.btnGradient1 = cachedTheme.btnGradient1;
				value.txtGradient0 = cachedTheme.txtGradient0;
				value.txtGradient1 = cachedTheme.txtGradient1;
				value.txtGradient2 = cachedTheme.txtGradient2;
				value.RefreshGradientColors();
				NetworkMenuDisplay.UpdateColors(value);
			}
		}
	}

	private static void HandleRemoteThemeLegacy(Player sender, object[] args)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		if (args.Length >= 9)
		{
			int packed = Convert.ToInt32(args[1]);
			int packed2 = Convert.ToInt32(args[2]);
			int packed3 = Convert.ToInt32(args[3]);
			int packed4 = Convert.ToInt32(args[4]);
			int packed5 = Convert.ToInt32(args[5]);
			int packed6 = Convert.ToInt32(args[6]);
			int packed7 = Convert.ToInt32(args[7]);
			int num = Convert.ToInt32(args[8]);
			Color color = PackedToColor(packed);
			Color color2 = PackedToColor(packed2);
			Color color3 = PackedToColor(packed3);
			Color color4 = PackedToColor(packed4);
			Color color5 = PackedToColor(packed5);
			Color color6 = PackedToColor(packed6);
			cachedThemes[sender.ActorNumber] = new CachedTheme
			{
				menuBgGradient = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(color)
				},
				btnGradient0 = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(color2)
				},
				btnGradient1 = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(color3)
				},
				txtGradient0 = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(color4)
				},
				txtGradient1 = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(color5)
				},
				txtGradient2 = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(color6)
				},
				playerColor = PackedToColor(packed7),
				thinMenu = ((num & 1) != 0),
				swapButtonColors = ((num & 2) != 0)
			};
			if (remoteMenus.TryGetValue(sender.ActorNumber, out var value))
			{
				CachedTheme cachedTheme = cachedThemes[sender.ActorNumber];
				value.playerColor = cachedTheme.playerColor;
				value.thinMenu = cachedTheme.thinMenu;
				value.swapButtonColors = cachedTheme.swapButtonColors;
				value.menuBgGradient = cachedTheme.menuBgGradient;
				value.btnGradient0 = cachedTheme.btnGradient0;
				value.btnGradient1 = cachedTheme.btnGradient1;
				value.txtGradient0 = cachedTheme.txtGradient0;
				value.txtGradient1 = cachedTheme.txtGradient1;
				value.txtGradient2 = cachedTheme.txtGradient2;
				value.RefreshGradientColors();
				NetworkMenuDisplay.UpdateColors(value);
			}
		}
	}

	private void Update()
	{
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<int> list = null;
		foreach (KeyValuePair<int, RemoteMenuState> remoteMenu in remoteMenus)
		{
			if (remoteMenu.Value.closing)
			{
				continue;
			}
			if (!(Time.time - remoteMenu.Value.lastStateTime > 3.5f))
			{
				Room currentRoom = PhotonNetwork.CurrentRoom;
				if (((currentRoom != null) ? currentRoom.GetPlayer(remoteMenu.Key, false) : null) != null)
				{
					continue;
				}
			}
			if (list == null)
			{
				list = new List<int>();
			}
			list.Add(remoteMenu.Key);
		}
		if (list != null)
		{
			foreach (int item in list)
			{
				if (remoteMenus.TryGetValue(item, out var value))
				{
					if ((Object)(object)value.displayObject != (Object)null && !value.closing)
					{
						NetworkMenuDisplay.CloseAndDestroy(value);
					}
					remoteMenus.Remove(item);
					cachedThemes.Remove(item);
				}
			}
		}
		if (Time.time - heartbeatTimer >= 3f)
		{
			heartbeatTimer = Time.time;
			SendMenuHeartbeat();
		}
		if (Time.time - themeTimer >= 2f)
		{
			themeTimer = Time.time;
			SendThemeState();
		}
		foreach (KeyValuePair<int, RemoteMenuState> remoteMenu2 in remoteMenus)
		{
			RemoteMenuState value2 = remoteMenu2.Value;
			if ((Object)(object)value2.displayObject == (Object)null || value2.closing)
			{
				continue;
			}
			value2.RefreshGradientColors();
			NetworkMenuDisplay.UpdateColors(value2);
			if ((Object)(object)value2.cachedRig == (Object)null)
			{
				value2.cachedRig = GorillaGameManager.StaticFindRigForPlayer(NetPlayer.op_Implicit(value2.player));
				if ((Object)(object)value2.cachedRig != (Object)null)
				{
					value2.rigOffset = value2.position - ((Component)value2.cachedRig).transform.position;
				}
			}
			if ((Object)(object)value2.cachedRig != (Object)null)
			{
				Vector3 val = ((Component)value2.cachedRig).transform.position + value2.rigOffset;
				if (value2.position != val)
				{
					value2.position = val;
					NetworkMenuDisplay.UpdatePosition(value2);
				}
			}
		}
		if ((Object)(object)Main.menu != (Object)null && Time.time - syncTimer >= 0.033f)
		{
			syncTimer = Time.time;
			SendMenuState();
		}
		if (Time.time - gunSyncTimer >= 0.033f)
		{
			gunSyncTimer = Time.time;
			GunLib.SendGunData();
		}
		GunLib.UpdateRemoteGunPointers();
		Main.GunActiveThisFrame = false;
	}

	public static void SyncOnJoin()
	{
		if (Main.networkMenuEnabled && PhotonNetwork.InRoom && (Object)(object)Main.menu != (Object)null)
		{
			SendThemeState();
			((MonoBehaviour)instance).StartCoroutine(DelayedSync());
		}
	}

	private static IEnumerator DelayedSync()
	{
		yield return (object)new WaitForSeconds(1f);
		if (Main.networkMenuEnabled && PhotonNetwork.InRoom && (Object)(object)Main.menu != (Object)null)
		{
			SendMenuState();
		}
	}

	public static void RemoveRemoteMenu(Player player)
	{
		RemoveRemoteMenu(player.ActorNumber);
	}

	public static void RemoveRemoteMenu(int actorNumber)
	{
		if (remoteMenus.TryGetValue(actorNumber, out var value))
		{
			if ((Object)(object)value.displayObject != (Object)null && !value.closing)
			{
				NetworkMenuDisplay.CloseAndDestroy(value);
			}
			remoteMenus.Remove(actorNumber);
			cachedThemes.Remove(actorNumber);
		}
		if (GunLib.remoteGunPointers.TryGetValue(actorNumber, out var value2))
		{
			if ((Object)(object)value2.pointer != (Object)null)
			{
				Object.Destroy((Object)(object)value2.pointer);
			}
			if ((Object)(object)value2.line != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)value2.line).gameObject);
			}
			GunLib.remoteGunPointers.Remove(actorNumber);
		}
	}

	public static void ClearAllRemoteMenus()
	{
		foreach (KeyValuePair<int, RemoteMenuState> remoteMenu in remoteMenus)
		{
			if ((Object)(object)remoteMenu.Value.displayObject != (Object)null && !remoteMenu.Value.closing)
			{
				NetworkMenuDisplay.CloseAndDestroy(remoteMenu.Value);
			}
		}
		remoteMenus.Clear();
		cachedThemes.Clear();
		GunLib.ClearRemoteGunPointers();
	}
}
