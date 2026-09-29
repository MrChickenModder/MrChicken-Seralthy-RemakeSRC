using System;
using System.Collections.Generic;
using System.Linq;
using GorillaNetworking;
using Seralyth.Extensions;
using Seralyth.Menu;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Seralyth.Managers;

public class CustomBoardManager : MonoBehaviour
{
	private readonly struct BoardInformation
	{
		public readonly string GameObjectPath;

		public readonly Vector3 Position;

		public readonly Vector3 Rotation;

		public readonly Vector3 Scale;

		public BoardInformation(string path, Vector3 pos, Vector3 rot, Vector3 scale)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			GameObjectPath = path;
			Position = pos;
			Rotation = rot;
			Scale = scale;
		}
	}

	public static CustomBoardManager instance;

	private static bool _customBoardsEnabled = true;

	private static readonly Dictionary<TextMeshPro, float> characterDistanceArchive = new Dictionary<TextMeshPro, float>();

	private static bool _customBoardFonts;

	private static Material _screenRed;

	private static Material _screenBlack;

	public static bool CustomBoardTextEnabled = true;

	private static Material _boardMaterial = new Material(Shader.Find("GorillaTag/UberShader"));

	public const int StumpLeaderboardIndex = 3;

	public const int ForestLeaderboardIndex = 6;

	public static string motdTemplate = "You are using build {0}. This menu was created by Seralyth Software. This menu is completely free and open sourced, if you paid for this menu you have been scammed. There are a total of <b>{1}</b> mods on this menu. <color=red>Seralyth is not responsible for any bans using this menu.</color> If you get banned while using this, it's your responsibility.\n\nCurrent menu status: <b>Loading...</b>\nMade with <3 by the community.\n\n<alpha=128>{2} {0} {3}<alpha=255>";

	public Material forestMaterial;

	public Material stumpMaterial;

	public GameObject motdTitle;

	public GameObject motdText;

	private TMP_FontAsset archiveGorillaTagFont;

	private bool hasFoundAllBoards;

	public readonly Dictionary<string, GameObject> objectBoards = new Dictionary<string, GameObject>();

	public List<GorillaNetworkJoinTrigger> triggers = new List<GorillaNetworkJoinTrigger>();

	public readonly List<TextMeshPro> textMeshPro = new List<TextMeshPro>();

	public GameObject computerMonitor;

	private static readonly Dictionary<string, BoardInformation> BoardInformations = new Dictionary<string, BoardInformation>
	{
		["Canyon2"] = new BoardInformation("Canyon/CanyonScoreboardAnchor/GorillaScoreBoard", new Vector3(-24.5019f, -28.7746f, 0.1f), new Vector3(270f, 0f, 0f), new Vector3(21.5946f, 1f, 22.1782f)),
		["Skyjungle"] = new BoardInformation("skyjungle/UI/Scoreboard/GorillaScoreBoard", new Vector3(-21.2764f, -32.1928f, 0f), new Vector3(270.2987f, 0.2f, 359.9f), new Vector3(21.6f, 0.1f, 20.4909f)),
		["Mountain"] = new BoardInformation("Mountain/MountainScoreboardAnchor/GorillaScoreBoard", Vector3.zero, Vector3.zero, Vector3.one),
		["Metropolis"] = new BoardInformation("MetroMain/ComputerArea/Scoreboard/GorillaScoreBoard", new Vector3(-25.1f, -31f, 0.1502f), new Vector3(270.1958f, 0.2086f, 0f), new Vector3(21f, 102.9727f, 21.4f)),
		["Bayou"] = new BoardInformation("BayouMain/ComputerArea/GorillaScoreBoardPhysical", new Vector3(-28.3419f, -26.851f, 0.3f), new Vector3(270f, 0f, 0f), new Vector3(21.3636f, 38f, 21f)),
		["Beach"] = new BoardInformation("BeachScoreboardAnchor/GorillaScoreBoard", new Vector3(-22.1964f, -33.7126f, 0.1f), new Vector3(270.056f, 0f, 0f), new Vector3(21.2f, 2f, 21.6f)),
		["Cave"] = new BoardInformation("Cave_Main_Prefab/CrystalCaveScoreboardAnchor/GorillaScoreBoard", new Vector3(-22.1964f, -33.7126f, 0.1f), new Vector3(270.056f, 0f, 0f), new Vector3(21.2f, 2f, 21.6f)),
		["Rotating"] = new BoardInformation("RotatingPermanentEntrance/UI (1)/RotatingScoreboard/RotatingScoreboardAnchor/GorillaScoreBoard", new Vector3(-22.1964f, -33.7126f, 0.1f), new Vector3(270.056f, 0f, 0f), new Vector3(21.2f, 2f, 21.6f)),
		["MonkeBlocks"] = new BoardInformation("Environment Objects/MonkeBlocksRoomPersistent/AtticScoreBoard/AtticScoreboardAnchor/GorillaScoreBoard", new Vector3(-22.1964f, -24.5091f, 0.57f), new Vector3(270.1856f, 0.1f, 0f), new Vector3(21.6f, 1.2f, 20.8f)),
		["Basement"] = new BoardInformation("Basement/BasementScoreboardAnchor/GorillaScoreBoard/", new Vector3(-22.1964f, -24.5091f, 0.57f), new Vector3(270.1856f, 0.1f, 0f), new Vector3(21.6f, 1.2f, 20.8f)),
		["City"] = new BoardInformation("City_Pretty/CosmeticsScoreboardAnchor/GorillaScoreBoard", new Vector3(-22.1964f, -34.9f, 0.57f), new Vector3(270f, 0f, 0f), new Vector3(21.6f, 2.4f, 22f))
	};

	public static bool CustomBoardsEnabled
	{
		get
		{
			return _customBoardsEnabled;
		}
		set
		{
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Expected O, but got Unknown
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Expected O, but got Unknown
			_customBoardsEnabled = value;
			if (value)
			{
				instance.ReloadBoards();
				instance.motdTitle.SetActive(true);
				instance.motdText.SetActive(true);
				Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/motdBodyText").SetActive(false);
				Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/motdHeadingText").SetActive(false);
				return;
			}
			foreach (GorillaNetworkJoinTrigger allJoinTrigger in ((PhotonNetworkController)PhotonNetworkController.Instance).allJoinTriggers)
			{
				try
				{
					JoinTriggerUI ui = allJoinTrigger.ui;
					JoinTriggerUITemplate template = ui.template;
					if ((Object)(object)_screenRed == (Object)null)
					{
						_screenRed = new Material(Shader.Find("GorillaTag/UberShader"))
						{
							color = Color32.op_Implicit(new Color32((byte)226, (byte)73, (byte)41, byte.MaxValue))
						};
					}
					if ((Object)(object)_screenBlack == (Object)null)
					{
						_screenBlack = new Material(Shader.Find("GorillaTag/UberShader"))
						{
							color = Color32.op_Implicit(new Color32((byte)39, (byte)34, (byte)28, byte.MaxValue))
						};
					}
					template.ScreenBG_AbandonPartyAndSoloJoin = _screenRed;
					template.ScreenBG_AlreadyInRoom = _screenBlack;
					template.ScreenBG_Error = _screenRed;
				}
				catch
				{
				}
			}
			List<GameObject> list = (from x in Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom").transform.Children()
				where ((Object)x).name.Contains("UnityTempFile")
				select x).ToList();
			if (3 < list.Count)
			{
				GameObject val = list[3];
				if ((Object)(object)val != (Object)null && (Object)(object)instance.stumpMaterial != (Object)null)
				{
					val.GetComponent<Renderer>().material = instance.stumpMaterial;
				}
			}
			List<GameObject> list2 = (from x in Main.GetObject("Environment Objects/LocalObjects_Prefab/Forest").transform.Children()
				where ((Object)x).name.Contains("UnityTempFile")
				select x).ToList();
			if (6 < list2.Count)
			{
				GameObject val2 = list2[6];
				if ((Object)(object)val2 != (Object)null && (Object)(object)instance.forestMaterial != (Object)null)
				{
					val2.GetComponent<Renderer>().material = instance.forestMaterial;
				}
			}
			foreach (GameObject value2 in instance.objectBoards.Values)
			{
				Object.Destroy((Object)(object)value2);
			}
			instance.objectBoards.Clear();
			instance.motdTitle.SetActive(false);
			instance.motdText.SetActive(false);
			Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/motdHeadingText").SetActive(true);
			Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/motdBodyText").SetActive(true);
		}
	}

	public static bool CustomBoardFonts
	{
		get
		{
			return _customBoardFonts;
		}
		set
		{
			if (!value && _customBoardFonts)
			{
				foreach (TextMeshPro item in instance.textMeshPro.Where((TextMeshPro text) => ((Behaviour)text).isActiveAndEnabled))
				{
					((TMP_Text)(object)item).SafeSetFont(instance.archiveGorillaTagFont);
					((TMP_Text)(object)item).SafeSetFontStyle((FontStyles)0);
					if (characterDistanceArchive.TryGetValue(item, out var value2))
					{
						((TMP_Text)item).characterSpacing = value2;
					}
				}
			}
			_customBoardFonts = value;
		}
	}

	public static Material BoardMaterial
	{
		get
		{
			return _boardMaterial;
		}
		set
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			if ((Object)(object)value == (Object)null)
			{
				value = new Material(Shader.Find("GorillaTag/UberShader"));
			}
			_boardMaterial = value;
			instance.ReloadBoards();
		}
	}

	public void Awake()
	{
		instance = this;
		SceneManager.sceneLoaded += SceneLoaded;
	}

	public void ReloadBoards()
	{
		hasFoundAllBoards = false;
	}

	public void Update()
	{
		//IL_0700: Unknown result type (might be due to invalid IL or missing references)
		//IL_0705: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_071f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0724: Unknown result type (might be due to invalid IL or missing references)
		//IL_0769: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0661: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07db: Unknown result type (might be due to invalid IL or missing references)
		if (!hasFoundAllBoards)
		{
			try
			{
				foreach (GameObject value in objectBoards.Values)
				{
					Object.Destroy((Object)(object)value);
				}
				objectBoards.Clear();
				List<GameObject> list = (from x in Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom").transform.Children()
					where ((Object)x).name.Contains("UnityTempFile")
					select x).ToList();
				if (3 < list.Count)
				{
					GameObject val = list[3];
					if ((Object)(object)val != (Object)null)
					{
						if ((Object)(object)stumpMaterial == (Object)null)
						{
							stumpMaterial = val.GetComponent<Renderer>().material;
						}
						val.GetComponent<Renderer>().material = BoardMaterial;
					}
				}
				List<GameObject> list2 = (from x in Main.GetObject("Environment Objects/LocalObjects_Prefab/Forest").transform.Children()
					where ((Object)x).name.Contains("UnityTempFile")
					select x).ToList();
				if (6 < list2.Count)
				{
					GameObject val2 = list2[6];
					if ((Object)(object)val2 != (Object)null)
					{
						if ((Object)(object)forestMaterial == (Object)null)
						{
							forestMaterial = val2.GetComponent<Renderer>().material;
						}
						val2.GetComponent<Renderer>().material = BoardMaterial;
					}
				}
				foreach (GorillaNetworkJoinTrigger allJoinTrigger in ((PhotonNetworkController)PhotonNetworkController.Instance).allJoinTriggers)
				{
					try
					{
						JoinTriggerUI ui = allJoinTrigger.ui;
						JoinTriggerUITemplate template = ui.template;
						template.ScreenBG_AbandonPartyAndSoloJoin = BoardMaterial;
						template.ScreenBG_AlreadyInRoom = BoardMaterial;
						template.ScreenBG_ChangingGameModeSoloJoin = BoardMaterial;
						template.ScreenBG_Error = BoardMaterial;
						template.ScreenBG_InPrivateRoom = BoardMaterial;
						template.ScreenBG_LeaveRoomAndGroupJoin = BoardMaterial;
						template.ScreenBG_LeaveRoomAndSoloJoin = BoardMaterial;
						template.ScreenBG_NotConnectedSoloJoin = BoardMaterial;
						TextMeshPro screenText = ui.screenText;
						if (!textMeshPro.Contains(screenText))
						{
							textMeshPro.Add(screenText);
						}
					}
					catch
					{
					}
				}
				((PhotonNetworkController)PhotonNetworkController.Instance).UpdateTriggerScreens();
				string[] array = new string[4] { "Environment Objects/LocalObjects_Prefab/TreeRoom/CodeOfConductHeadingText", "Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData", "Environment Objects/LocalObjects_Prefab/TreeRoom/Data", "Environment Objects/LocalObjects_Prefab/TreeRoom/FunctionSelect" };
				string[] array2 = array;
				foreach (string text in array2)
				{
					GameObject val3 = Main.GetObject(text);
					if ((Object)(object)val3 != (Object)null)
					{
						TextMeshPro component = val3.GetComponent<TextMeshPro>();
						if (!textMeshPro.Contains(component))
						{
							textMeshPro.Add(component);
						}
					}
					else
					{
						LogManager.Log("Could not find " + text);
					}
				}
				Transform transform = Main.GetObject("Environment Objects/LocalObjects_Prefab/Forest/ForestScoreboardAnchor/GorillaScoreBoard").transform;
				for (int num2 = 0; num2 < ((Component)transform).transform.childCount; num2++)
				{
					GameObject gameObject = ((Component)transform.GetChild(num2)).gameObject;
					if ((((Object)gameObject).name.Contains("Board Text") || ((Object)gameObject).name.Contains("Scoreboard_OfflineText")) && gameObject.activeSelf)
					{
						TextMeshPro component2 = gameObject.GetComponent<TextMeshPro>();
						if (!textMeshPro.Contains(component2))
						{
							textMeshPro.Add(component2);
						}
					}
				}
				hasFoundAllBoards = true;
			}
			catch (Exception ex)
			{
				LogManager.LogError("Error with board colors at " + ex.StackTrace + ": " + ex.Message);
				hasFoundAllBoards = false;
			}
		}
		if ((Object)(object)computerMonitor == (Object)null)
		{
			computerMonitor = Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/GorillaComputerObject/ComputerUI/monitor/monitorScreen");
		}
		if ((Object)(object)computerMonitor != (Object)null)
		{
			computerMonitor.GetComponent<Renderer>().material = BoardMaterial;
		}
		try
		{
			BoardMaterial.color = (CustomBoardsEnabled ? Main.backgroundColor.GetCurrentColor() : Color32.op_Implicit(new Color32((byte)0, (byte)59, (byte)4, byte.MaxValue)));
			if ((Object)(object)motdTitle == (Object)null)
			{
				GameObject val4 = Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/motdHeadingText");
				motdTitle = Object.Instantiate<GameObject>(val4, val4.transform.parent);
				val4.SetActive(false);
			}
			TextMeshPro component3 = motdTitle.GetComponent<TextMeshPro>();
			if (!textMeshPro.Contains(component3))
			{
				textMeshPro.Add(component3);
			}
			((TMP_Text)component3).richText = true;
			((TMP_Text)(object)component3).SafeSetFontSize(100f);
			((TMP_Text)(object)component3).SafeSetText("Thanks for using " + (Main.doCustomName ? Main.customMenuName : Main.menuName) + "!");
			((TMP_Text)(object)component3).SafeSetFontStyle(Main.activeFontStyle);
			((TMP_Text)(object)component3).SafeSetFont(Main.activeFont);
			Main.FollowMenuSettings((TMP_Text)(object)component3, -4f);
			((TMP_Text)(object)component3).SafeSetText(Main.FollowMenuSettings(((TMP_Text)component3).text));
			((Graphic)component3).color = Main.textColors[0].GetCurrentColor();
			((TMP_Text)component3).overflowMode = (TextOverflowModes)0;
			if ((Object)(object)motdText == (Object)null)
			{
				GameObject val5 = Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/motdBodyText");
				motdText = Object.Instantiate<GameObject>(val5, val5.transform.parent);
				val5.SetActive(false);
				((Behaviour)motdText.GetComponent<PlayFabTitleDataTextDisplay>()).enabled = false;
			}
			TextMeshPro component4 = motdText.GetComponent<TextMeshPro>();
			if (!textMeshPro.Contains(component4))
			{
				textMeshPro.Add(component4);
			}
			((TMP_Text)component4).richText = true;
			((TMP_Text)(object)component4).SafeSetFontSize(100f);
			((Graphic)component4).color = Main.textColors[0].GetCurrentColor();
			((TMP_Text)(object)component4).SafeSetFontStyle(Main.activeFontStyle);
			((TMP_Text)(object)component4).SafeSetFont(Main.activeFont);
			Main.FollowMenuSettings((TMP_Text)(object)component4, -4f);
			((TMP_Text)(object)component4).SafeSetText(Main.FollowMenuSettings(string.Format(motdTemplate, "10.0.2", Main.fullModAmount, PluginInfo.BetaBuild ? "Beta" : "Release", "2026-09-29T13:10:37Z")));
		}
		catch
		{
		}
		try
		{
			Color color = Main.textColors[0].GetCurrentColor();
			if (!CustomBoardsEnabled || !CustomBoardTextEnabled)
			{
				color = Color.white;
			}
			foreach (TextMeshPro item in textMeshPro.Where((TextMeshPro val6) => ((Behaviour)val6).isActiveAndEnabled))
			{
				((Graphic)item).color = color;
				if (CustomBoardFonts)
				{
					if (archiveGorillaTagFont == null)
					{
						archiveGorillaTagFont = ((TMP_Text)item).font;
					}
					if (!characterDistanceArchive.ContainsKey(item))
					{
						characterDistanceArchive[item] = ((TMP_Text)item).characterSpacing;
					}
					((TMP_Text)item).characterSpacing = 0f;
					((TMP_Text)(object)item).SafeSetFont(Main.activeFont);
					((TMP_Text)(object)item).SafeSetFontStyle(Main.activeFontStyle);
				}
			}
		}
		catch
		{
		}
	}

	public void SceneLoaded(Scene scene, LoadSceneMode mode)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (CustomBoardsEnabled && BoardInformations.TryGetValue(((Scene)(ref scene)).name, out var value))
		{
			CreateObjectBoard(((Scene)(ref scene)).name, value.GameObjectPath, value.Position, value.Rotation, value.Scale);
		}
	}

	public void CreateObjectBoard(string scene, string gameObject, Vector3? position = null, Vector3? rotation = null, Vector3? scale = null)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (objectBoards.TryGetValue(scene, out var value))
			{
				if ((Object)(object)value != (Object)null)
				{
					Object.Destroy((Object)(object)value);
				}
				objectBoards.Remove(scene);
			}
			GameObject val = GameObject.CreatePrimitive((PrimitiveType)4);
			val.transform.parent = Main.GetObject(gameObject).transform;
			val.transform.localPosition = (Vector3)(((_003F?)position) ?? new Vector3(-22.1964f, -34.9f, 0.57f));
			val.transform.localRotation = Quaternion.Euler((Vector3)(((_003F?)rotation) ?? new Vector3(270f, 0f, 0f)));
			val.transform.localScale = (Vector3)(((_003F?)scale) ?? new Vector3(21.6f, 2.4f, 22f));
			Object.Destroy((Object)(object)val.GetComponent<Collider>());
			val.GetComponent<Renderer>().material = BoardMaterial;
			objectBoards.Add(scene, val);
		}
		catch (Exception arg)
		{
			LogManager.LogError($"Failed to create object board for scene {scene}: {arg}");
		}
	}
}
