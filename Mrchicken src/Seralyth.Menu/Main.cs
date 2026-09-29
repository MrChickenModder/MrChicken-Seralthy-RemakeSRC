using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using GorillaTagScripts;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using Seralyth.Classes.Menu;
using Seralyth.Classes.Mods;
using Seralyth.Extensions;
using Seralyth.Managers;
using Seralyth.Mods;
using Seralyth.Patches;
using Seralyth.Patches.Menu;
using Seralyth.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.TextCore;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR;
using Valve.Newtonsoft.Json;
using Valve.VR;

namespace Seralyth.Menu;

[HarmonyPatch(typeof(GTPlayer), "LateUpdate")]
public class Main : MonoBehaviour
{
	public class PromptData
	{
		public bool IsText;

		public string Message;

		public string AcceptText;

		public string DeclineText;

		public Action AcceptAction;

		public Action DeclineAction;
	}

	private static readonly List<string> postActions = new List<string>();

	public static List<Key> lastPressedKeys = new List<Key>();

	public static readonly Dictionary<Key, (float, float)> keyPressedTimes = new Dictionary<Key, (float, float)>();

	public static readonly Key[] detectedKeys;

	private static TMP_SpriteAsset buttonSpriteSheet;

	private static Vector3? recenterPosition;

	private static Quaternion? recenterRotation;

	private static int menuOpenCount;

	private static int suppressCloseFrames;

	public static List<PromptData> prompts;

	public static Material promptMaterial;

	private static string versionArchive;

	public static readonly Dictionary<(Color, Color), Texture2D> cacheGradients;

	private static readonly List<float> volumeArchive;

	private static Vector3 GunPositionSmoothed;

	public static GameObject GunPointer;

	private static LineRenderer GunLine;

	public static Vector3 GunStartPos;

	public static Vector3 GunEndPos;

	public static bool GunActiveThisFrame;

	private static VRRig _giveGunTarget;

	public static bool isAdmin;

	public static string adminName;

	public static bool isModerator;

	public static string moderatorName;

	public static bool isOwner;

	public static string ownerName;

	private static bool tagManagerButtonsAdded;

	private static bool adminModsButtonAdded;

	private static bool consoleAssetsButtonAdded;

	private static string adminWelcomeText;

	private static int adminWelcomeCharIndex;

	private static float adminWelcomeTypingTime;

	private static bool adminWelcomeDone;

	public static Dictionary<string, SnowballThrowable> snowballDict;

	private static bool allSnowballsInitialized;

	public static readonly Dictionary<Type, object[]> typePool;

	private static readonly Dictionary<Type, float> receiveTypeDelay;

	private static float randomIndex;

	private static float randomDecayTime;

	private static readonly Dictionary<string, GameObject> objectPool;

	public static GameObject audioManager;

	public static GameObject handAudioManager;

	private static Coroutine translationCoroutine;

	public static bool vibrantColors;

	private static Gradient richtextGradientGradient;

	public static bool inRoomStatus;

	public static string lastRoom;

	public static Vector3 ServerSyncPos;

	public static Vector3 ServerSyncLeftHandPos;

	public static Vector3 ServerSyncRightHandPos;

	public static Vector3 ServerPos;

	public static Vector3 ServerLeftHandPos;

	public static Vector3 ServerRightHandPos;

	public static readonly Dictionary<VRRig, int> playerPing;

	private static int? noInvisLayerMask;

	public static TMP_FontAsset activeFont;

	public static FontStyles activeFontStyle;

	public static Font currentFont;

	public static TMP_FontAsset AgencyFB;

	public static TMP_FontAsset FreeSans;

	public static TMP_FontAsset Candara;

	public static TMP_FontAsset ComicSans;

	public static TMP_FontAsset CascadiaMono;

	public static TMP_FontAsset Anton;

	public static TMP_FontAsset Minecraft;

	public static TMP_FontAsset MSGothic;

	public static TMP_FontAsset OpenDyslexic;

	public static TMP_FontAsset SimSun;

	public static TMP_FontAsset Taiko;

	public static TMP_FontAsset Terminal;

	public static TMP_FontAsset Utopium;

	public static TMP_FontAsset DejaVuSans;

	public static TMP_FontAsset LiberationSans;

	public static string rat;

	public static bool isOnPC;

	public static bool IsSteam;

	public static bool Lockdown;

	public static bool HasLoaded;

	public static bool hasLoadedPreferences;

	public static bool allowDetected;

	public static float loadPreferencesTime;

	public static float playTime;

	public static int frameCount;

	public static float badAppleTime;

	public static bool thinMenu;

	public static bool longmenu;

	public static bool hidetitle;

	public static bool disorganized;

	public static bool flipMenu;

	public static bool shinyMenu;

	public static bool transparentMenu;

	public static bool crystallizeMenu;

	public static bool zeroGravityMenu;

	public static bool menuCollisions;

	public static bool dropOnRemove;

	public static bool shouldOutline;

	public static bool outlineText;

	public static bool underlineText;

	public static bool strikethroughText;

	public static bool smallCapsText;

	public static bool innerOutline;

	public static bool smoothLines;

	public static bool shouldRound;

	public static bool isMouseDown;

	public static bool openedwithright;

	public static bool oneHand;

	public static bool frozenMenu;

	public static Vector3 closeFrozenPosition;

	public static bool lineMenu;

	public static bool legalOnly;

	public static bool clickGUI;

	public static int _pageSize;

	public static int pageOffset;

	public static int pageNumber;

	public static bool pageScrolling;

	public static bool noPageNumber;

	public static bool disablePageButtons;

	public static bool swapButtonColors;

	public static int pageButtonType;

	public static float pageButtonChangeDelay;

	public static bool pcKeyboardSounds;

	public static int buttonClickSound;

	public static int buttonClickVolume;

	public static int buttonOffset;

	public static int menuButtonIndex;

	public static bool toggleButton;

	public static bool toggleButtonHeld;

	public static bool toggleButtonActive;

	public static bool keyboardWithToggleButton;

	public static int characterDistance;

	public static bool doButtonsVibrate;

	public static bool serversidedButtonSounds;

	public static bool networkMenuEnabled;

	public static bool joystickMenu;

	public static bool joystickMenuSearching;

	public static bool physicalMenu;

	public const byte SeralythPresenceByte = 79;

	public static Dictionary<int, string> seralythUsers;

	private static bool seralythUserCountSent;

	public static bool barkMenu;

	private static bool barkMenuOpen;

	private static bool? barkMenuGrabbed;

	private static float barkBangDelay;

	private static int barkBangCount;

	private static bool previousBarkBangState;

	public static Vector3 physicalOpenPosition;

	public static Quaternion physicalOpenRotation;

	public static Vector3 smoothTargetPosition;

	public static Quaternion smoothTargetRotation;

	public static bool joystickOpen;

	public static bool smoothMenuPosition;

	public static bool smoothMenuRotation;

	public static int joystickButtonSelected;

	public static string joystickSelectedButton;

	public static float joystickDelay;

	public static float scrollDelay;

	public static int joystickMenuPosition;

	public static Vector3[] joystickMenuPositions;

	public static bool rightHand;

	public static bool isRightHand;

	public static bool bothHands;

	public static bool wristMenu;

	public static bool explodeMenu;

	public static bool watchMenu;

	public static bool watchUsed;

	public static float watchTimer;

	public static bool wristOpen;

	public static float wristMenuDelay;

	public static bool stackNotifications;

	public static bool hideBrackets;

	public static bool disableNotifications;

	public static bool disableMasterClientNotifications;

	public static bool disableRoomNotifications;

	public static bool disablePlayerNotifications;

	public static bool clearNotificationsOnDisconnect;

	public static string narratorName;

	public static int narratorIndex;

	public static bool showEnabledModsVR;

	public static bool advancedArraylist;

	public static bool flipArraylist;

	public static bool hideSettings;

	public static bool hideMacros;

	public static bool hideTextOnCamera;

	public static bool hidePointer;

	public static bool incrementalButtons;

	public static bool incrementalBoost;

	public static bool disableDisconnectButton;

	public static bool disableFpsCounter;

	public static bool disableCategoryDisplay;

	public static int categoryDisplayMode;

	public static bool disableSearchButton;

	public static bool disableReturnButton;

	public static bool enableDebugButton;

	public static bool ghostException;

	public static bool disableGhostview;

	public static bool legacyGhostview;

	public static bool checkMode;

	public static bool lastChecker;

	public static bool rockWatermark;

	public static bool disableWatermark;

	public static string CosmeticsOwned;

	public static Vector3 MidPosition;

	public static Vector3 MidVelocity;

	public static bool SmoothGunPointer;

	public static bool smallGunPointer;

	public static bool disableGunPointer;

	public static bool disableGunLine;

	public static bool SwapGunHand;

	public static bool GriplessGuns;

	public static bool TriggerlessGuns;

	public static bool HardGunLocks;

	public static bool GunSounds;

	public static bool GunVibrations;

	public static bool GunParticles;

	public static int gunVariation;

	public static int GunDirection;

	public static int GunLineQuality;

	public static bool GunLibLine;

	public static bool GunLibTrail;

	public static int GunLibShape;

	public static bool gunPreviewEnabled;

	private static GameObject GunPreviewPointer;

	private static LineRenderer GunPreviewLine;

	public static bool GunSpawned;

	public static bool gunLocked;

	public static VRRig lockTarget;

	public static bool lastGunSpawned;

	public static bool lastGunTrigger;

	public static bool lastGunSpawnedVibration;

	public static int fontCycle;

	public static bool NoAutoSizeText;

	public static bool doCustomName;

	public static string customMenuName;

	public static readonly string menuName;

	public static bool doCustomMenuBackground;

	public static bool menuTrail;

	public static bool adaptiveButtons;

	public static int pcbg;

	public static bool isSearching;

	public static bool nonGlobalSearch;

	public static bool isKeyboardPc;

	public static bool inTextInput;

	public static string keyboardInput;

	public static int? fullModAmount;

	public static int amountPartying;

	public static bool waitForPlayerJoin;

	public static bool scaleWithPlayer;

	public static float menuScale;

	public static int notificationScale;

	public static int overlayScale;

	public static int arraylistScale;

	public static bool dynamicSounds;

	public static bool exclusivePageSounds;

	public static bool dynamicAnimations;

	public static bool slowDynamicAnimations;

	public static bool dynamicGradients;

	public static bool horizontalGradients;

	public static bool scrollingGradients;

	public static bool particleSpawnEffect;

	public static bool animatedTitle;

	public static bool gradientTitle;

	public static string lastClickedName;

	public static bool isMenuButtonHeld;

	public static bool shouldBePC;

	public static bool leftPrimary;

	public static bool leftSecondary;

	public static bool rightPrimary;

	public static bool rightSecondary;

	public static bool leftGrab;

	public static bool rightGrab;

	public static float leftTrigger;

	public static float rightTrigger;

	public static bool leftTriggerPressed;

	public static bool rightTriggerPressed;

	public static Vector2 leftJoystick;

	public static Vector2 rightJoystick;

	public static bool leftJoystickClick;

	public static bool rightJoystickClick;

	public static bool ToggleBindings;

	public static bool OverwriteKeybinds;

	public static bool IsBinding;

	public static string BindInput;

	public static bool IsRebinding;

	public static bool IsPCBindMode;

	public static string PCBindPendingMod;

	public static readonly Dictionary<string, List<string>> ModBindings;

	public static readonly Dictionary<string, bool> BindStates;

	public static readonly List<string> quickActions;

	public static readonly List<string> recentlyUsed;

	public static Camera TPC;

	public static GameObject menu;

	public static GameObject menuBackground;

	public static GameObject pcBackground;

	public static GameObject reference;

	public static VideoPlayer videoPlayer;

	public static VideoPlayer promptVideoPlayer;

	public static SphereCollider buttonCollider;

	public static GameObject canvasObj;

	public static TextMeshPro fpsCount;

	public static Image watermarkImage;

	private static float fpsAvgTime;

	private static float fpsAverageNumber;

	private static float? potatoTime;

	private static float? adminTime;

	public static bool fpsCountTimed;

	public static bool fpsCountAverage;

	public static bool ftCount;

	public static float lastDeltaTime;

	public static TextMeshPro keyboardInputObject;

	public static TextMeshPro title;

	public static VRRig GhostRig;

	public static GameObject legacyGhostViewLeft;

	public static GameObject legacyGhostViewRight;

	public static Material GhostMaterial;

	public static Material CrystalMaterial;

	public static Material searchMat;

	public static Material updateMat;

	public static Material promptMat;

	public static Material watermarkMat;

	public static Material returnMat;

	public static Material debugMat;

	public static Material donateMat;

	public static GameObject lKeyReference;

	public static SphereCollider lKeyCollider;

	public static GameObject rKeyReference;

	public static SphereCollider rKeyCollider;

	public static GameObject VRKeyboard;

	public static GameObject menuSpawnPosition;

	public static GameObject TryOnRoom;

	public static GameObject watchobject;

	public static GameObject watchText;

	public static GameObject watchShell;

	public static GameObject watchEnabledIndicator;

	public static Material watchIndicatorMat;

	public static int watchMenuIndex;

	public static GameObject regwatchobject;

	public static GameObject regwatchText;

	public static GameObject regwatchShell;

	public static Material glass;

	public static Material cannabisMat;

	public static Texture2D cann;

	public static Texture2D pride;

	public static Texture2D trans;

	public static Texture2D gay;

	public static Texture2D searchIcon;

	public static Texture2D returnIcon;

	public static Texture2D debugIcon;

	public static Texture2D donateIcon;

	public static Texture2D updateIcon;

	public static Texture2D fixTexture;

	public static Texture2D customMenuBackgroundImage;

	public static Texture2D customWatermark;

	public static readonly List<string> favorites;

	public static readonly List<string> skipButtons;

	public static bool translate;

	public static string serverLink;

	public static int arrowType;

	public static readonly string[][] arrowTypes;

	public static int themeType;

	public static bool slowFadeColors;

	public static ExtGradient backgroundColor;

	public static ExtGradient menuBackgroundColor;

	public static ExtGradient[] buttonColors;

	public static ExtGradient[] textColors;

	public static Vector3 closePosition;

	public static Vector3 pointerOffset;

	public static int pointerIndex;

	public static string partyLastCode;

	public static float partyTime;

	public static bool partyKickReconnecting;

	public static float timeMenuStarted;

	public static float buttonCooldown;

	public static float autoSaveDelay;

	public static bool backupPreferences;

	public static int preferenceBackupCount;

	public static int notificationDecayTime;

	public static float ShootStrength;

	public static bool shift;

	public static bool lockShift;

	public static bool lowercaseMode;

	public static bool uppercaseMode;

	public static bool redactText;

	public static string inputTextColor;

	public static bool toggleDebugEchoMode;

	public static bool annoyingMode;

	public static readonly string[] facts;

	public static TMP_SpriteAsset ButtonSpriteSheet
	{
		get
		{
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Expected O, but got Unknown
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Expected O, but got Unknown
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Expected O, but got Unknown
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Expected O, but got Unknown
			if ((Object)(object)buttonSpriteSheet != (Object)null)
			{
				return buttonSpriteSheet;
			}
			buttonSpriteSheet = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
			((Object)buttonSpriteSheet).name = "Seralyth_SpriteSheet";
			List<Texture2D> textureList = new List<Texture2D>();
			List<(string name, int index)> spriteDataList = new List<(string, int)>();
			AddSprite("Favorite", AssetUtilities.LoadTextureFromResource("SeralythMenu.Resources.Client.favorite.png"));
			AddSprite("Folder", AssetUtilities.LoadTextureFromResource("SeralythMenu.Resources.Client.folder.png"));
			for (int i = 1; i <= 3; i++)
			{
				AddSprite("Left" + i, AssetUtilities.LoadTextureFromResource(string.Format("{0}.left{1}.png", "SeralythMenu.Resources.Client", i)));
				AddSprite("Right" + i, AssetUtilities.LoadTextureFromResource(string.Format("{0}.right{1}.png", "SeralythMenu.Resources.Client", i)));
			}
			int num = 512;
			Texture2D val = new Texture2D(num, num);
			Rect[] array = val.PackTextures(textureList.ToArray(), 2, num);
			buttonSpriteSheet.spriteSheet = (Texture)(object)val;
			((TMP_Asset)buttonSpriteSheet).material = new Material(Shader.Find("TextMeshPro/Sprite"))
			{
				mainTexture = (Texture)(object)val
			};
			buttonSpriteSheet.spriteInfoList = new List<TMP_Sprite>();
			Traverse.Create((object)buttonSpriteSheet).Field("m_Version").SetValue((object)"1.1.0");
			buttonSpriteSheet.spriteGlyphTable.Clear();
			for (int j = 0; j < spriteDataList.Count; j++)
			{
				Rect val2 = array[j];
				TMP_SpriteGlyph item = new TMP_SpriteGlyph
				{
					index = (uint)j,
					metrics = new GlyphMetrics(((Rect)(ref val2)).width * (float)((Texture)val).width, ((Rect)(ref val2)).height * (float)((Texture)val).height, (0f - ((Rect)(ref val2)).width * (float)((Texture)val).width) / 2f, ((Rect)(ref val2)).height * (float)((Texture)val).height * 0.8f, ((Rect)(ref val2)).width * (float)((Texture)val).width),
					glyphRect = new GlyphRect((int)(((Rect)(ref val2)).x * (float)((Texture)val).width), (int)(((Rect)(ref val2)).y * (float)((Texture)val).height), (int)(((Rect)(ref val2)).width * (float)((Texture)val).width), (int)(((Rect)(ref val2)).height * (float)((Texture)val).height)),
					scale = 1f,
					atlasIndex = 0
				};
				buttonSpriteSheet.spriteGlyphTable.Add(item);
			}
			buttonSpriteSheet.spriteCharacterTable.Clear();
			for (int k = 0; k < spriteDataList.Count; k++)
			{
				string item2 = spriteDataList[k].name;
				TMP_SpriteCharacter item3 = new TMP_SpriteCharacter(65534u, buttonSpriteSheet.spriteGlyphTable[k])
				{
					name = item2,
					scale = 1f,
					glyphIndex = (uint)k
				};
				buttonSpriteSheet.spriteCharacterTable.Add(item3);
			}
			buttonSpriteSheet.UpdateLookupTables();
			return buttonSpriteSheet;
			void AddSprite(string name, Texture2D tex)
			{
				spriteDataList.Add((name, textureList.Count));
				textureList.Add(tex);
			}
		}
	}

	public static PromptData CurrentPrompt => (prompts.Count > 0) ? prompts[0] : null;

	public static VRRig GiveGunTarget
	{
		get
		{
			if (!VRRigCache.ActiveRigs.Contains(_giveGunTarget))
			{
				_giveGunTarget = null;
			}
			return _giveGunTarget;
		}
		set
		{
			_giveGunTarget = value;
		}
	}

	public static int PageSize
	{
		get
		{
			return _pageSize - buttonOffset;
		}
		set
		{
			_pageSize = value;
		}
	}

	public static int DisplayedItemCount
	{
		get
		{
			int num = Buttons.buttons[Buttons.CurrentCategoryIndex].Length;
			if (Buttons.CurrentCategoryName == "Favorite Mods")
			{
				num = favorites.Count;
			}
			if (Buttons.CurrentCategoryName == "Enabled Mods")
			{
				List<string> list = new List<string> { "Exit Enabled Mods" };
				for (int i = 0; i < Buttons.buttons.Length; i++)
				{
					ButtonInfo[] source = Buttons.buttons[i];
					string text = Buttons.categoryNames[i];
					bool flag = hideSettings && text.Contains("Settings");
					bool flag2 = hideMacros && text.Contains("Macro");
					if (!(flag || flag2))
					{
						list.AddRange(from v in source
							where v.enabled
							select v.buttonText);
					}
				}
				num = list.Count - 1;
			}
			if (Buttons.CurrentCategoryName == "Main")
			{
				ButtonInfo[] array = Buttons.buttons[Buttons.CurrentCategoryIndex];
				foreach (ButtonInfo buttonInfo in array)
				{
					if (skipButtons.Contains(buttonInfo.buttonText))
					{
						num--;
					}
				}
			}
			if (!isSearching)
			{
				return num;
			}
			List<ButtonInfo> list2 = new List<ButtonInfo>();
			if (nonGlobalSearch && Buttons.CurrentCategoryName != "Main")
			{
				ButtonInfo[] array2 = Buttons.buttons[Buttons.CurrentCategoryIndex];
				foreach (ButtonInfo buttonInfo2 in array2)
				{
					try
					{
						List<string> list3 = ((buttonInfo2.aliases == null) ? new List<string>() : buttonInfo2.aliases.ToList());
						list3.Add(buttonInfo2.overlapText ?? buttonInfo2.buttonText);
						if (list3.Any((string buttonText) => buttonText.ClearTags().Replace(" ", "").ToLower()
							.Contains(keyboardInput.Replace(" ", "").ToLower())))
						{
							list2.Add(buttonInfo2);
						}
					}
					catch
					{
					}
				}
			}
			else
			{
				int num4 = 0;
				ButtonInfo[][] buttons = Buttons.buttons;
				foreach (ButtonInfo[] array3 in buttons)
				{
					ButtonInfo[] array4 = array3;
					foreach (ButtonInfo buttonInfo3 in array4)
					{
						try
						{
							if (((!Buttons.categoryNames[num4].Contains("Admin") && !(Buttons.categoryNames[num4] == "Mod Givers")) || isAdmin) && (!buttonInfo3.detected || allowDetected) && (!(Buttons.CurrentCategoryName == "Main") || !skipButtons.Contains(buttonInfo3.buttonText)))
							{
								string text2 = buttonInfo3.overlapText ?? buttonInfo3.buttonText;
								if (text2.Replace(" ", "").ToLower().Contains(keyboardInput.Replace(" ", "").ToLower()))
								{
									list2.Add(buttonInfo3);
								}
							}
						}
						catch
						{
						}
					}
					num4++;
				}
			}
			return list2.Count;
		}
	}

	public static float ButtonDistance => 0.8f / (float)(PageSize + buttonOffset);

	public static int LastPage => (DisplayedItemCount + PageSize - 1) / PageSize - 1;

	[Obsolete("currentCategoryIndex is obsolete. Use Buttons.CurrentCategoryIndex instead.")]
	public static int currentCategoryIndex
	{
		get
		{
			return Buttons.CurrentCategoryIndex;
		}
		set
		{
			Buttons.CurrentCategoryIndex = value;
		}
	}

	[Obsolete("currentCategoryName is obsolete. Use Buttons.CurrentCategoryName instead.")]
	public static string currentCategoryName
	{
		get
		{
			return Buttons.CurrentCategoryName;
		}
		set
		{
			Buttons.CurrentCategoryName = value;
		}
	}

	public static event Action OnMenuOpened;

	public static event Action OnMenuClosed;

	public static void OnLaunch()
	{
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Expected O, but got Unknown
		if ((Object)(object)CoroutineManager.instance == (Object)null)
		{
			LogManager.LogError("CoroutineManager instance is null on menu launch. Features may not function properly.");
		}
		if ((Object)(object)NotificationManager.Instance == (Object)null)
		{
			LogManager.LogError("CoroutineManager instance is null on menu launch. Features may not function properly.");
		}
		timeMenuStarted = Time.time;
		IsSteam = Object.op_Implicit((Object)(object)((PlayFabAuthenticator)PlayFabAuthenticator.instance).platform);
		InitializeFonts();
		Changelog.RefreshCategory();
		activeFont = AgencyFB;
		if (Bootstrapper.FirstLaunch && Directory.Exists("iisStupidMenu"))
		{
			Prompt("It seems like you have used ii's Stupid Menu before! Would you like to move all your enabled mods, settings and sounds to MrChicken Menu?", Settings.MergePreferences_iisStupidMenu);
		}
		NetworkSystem instance = NetworkSystem.Instance;
		instance.OnJoinedRoomEvent = (DelegateListProcessorPlusMinus<DelegateListProcessor, Action>)(object)instance.OnJoinedRoomEvent + (Action)OnJoinRoom;
		NetworkSystem instance2 = NetworkSystem.Instance;
		instance2.OnReturnedToSinglePlayer = (DelegateListProcessorPlusMinus<DelegateListProcessor, Action>)(object)instance2.OnReturnedToSinglePlayer + (Action)OnLeaveRoom;
		NetworkSystem instance3 = NetworkSystem.Instance;
		instance3.OnMasterClientSwitchedEvent = (DelegateListProcessorPlusMinus<DelegateListProcessor<NetPlayer>, Action<NetPlayer>>)(object)instance3.OnMasterClientSwitchedEvent + (Action<NetPlayer>)OnMasterClientSwitch;
		NetworkSystem instance4 = NetworkSystem.Instance;
		instance4.OnPlayerJoined = (DelegateListProcessorPlusMinus<DelegateListProcessor<NetPlayer>, Action<NetPlayer>>)(object)instance4.OnPlayerJoined + (Action<NetPlayer>)OnPlayerJoin;
		NetworkSystem instance5 = NetworkSystem.Instance;
		instance5.OnPlayerLeft = (DelegateListProcessorPlusMinus<DelegateListProcessor<NetPlayer>, Action<NetPlayer>>)(object)instance5.OnPlayerLeft + (Action<NetPlayer>)OnPlayerLeave;
		PhotonNetwork.NetworkingClient.EventReceived += SeralythPresenceEvent;
		SerializePatch.OnSerialize += OnSerialize;
		PlayerSerializePatch.OnPlayerSerialize += OnPlayerSerialize;
		GameObject obj = GetObject("Environment Objects/LocalObjects_Prefab/ForestToCave/C_Crystal_Chunk");
		object crystalMaterial;
		if (obj == null)
		{
			crystalMaterial = null;
		}
		else
		{
			Renderer component = obj.GetComponent<Renderer>();
			crystalMaterial = ((component != null) ? component.material : null);
		}
		CrystalMaterial = (Material)crystalMaterial;
		TryOnRoom = GetObject("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Cosmetics Room Triggers/TryOnRoom");
		int valueOrDefault = fullModAmount.GetValueOrDefault();
		if (!fullModAmount.HasValue)
		{
			valueOrDefault = Buttons.buttons.SelectMany((ButtonInfo[] list) => list).ToArray().Length;
			fullModAmount = valueOrDefault;
		}
		Quests.UpdateVRButtons();
		Important.LoadCreatedData();
		GameObject val = Seralyth.Classes.Menu.Console.LoadConsoleImmediately();
		val.AddComponent<NetworkMenuManager>();
		if (ServerData.ServerDataEnabled)
		{
			val.AddComponent<FriendManager>();
			val.AddComponent<PatreonManager>();
		}
		try
		{
			string path = "SeralythMenu/AllButtons.txt";
			string[] array = (from button in Buttons.buttons.SelectMany((ButtonInfo[] list) => list)
				select button.buttonText).ToArray();
			if (File.Exists(path))
			{
				string[] source = File.ReadAllText(path).Split("\n");
				string[] array2 = array;
				foreach (string text in array2)
				{
					if (!source.Contains(text))
					{
						ButtonInfo index = Buttons.GetIndex(text);
						string text2 = index.overlapText ?? index.buttonText;
						ButtonInfo buttonInfo = index;
						if (buttonInfo.overlapText == null)
						{
							buttonInfo.overlapText = text2 + " <color=grey>[</color><color=green>New</color><color=grey>]</color>";
						}
					}
				}
			}
			File.WriteAllText(path, string.Join("\n", array));
		}
		catch
		{
		}
		try
		{
			PluginManager.LoadPlugins();
		}
		catch (Exception ex)
		{
			LogManager.LogError("Error with PluginManager.LoadPlugins() at " + ex.StackTrace + ": " + ex.Message);
		}
		try
		{
			Sound.LoadSoundboard(openCategory: false);
		}
		catch (Exception ex2)
		{
			LogManager.LogError("Error with Sound.LoadSoundboard() at " + ex2.StackTrace + ": " + ex2.Message);
		}
		try
		{
			Movement.LoadMacros();
		}
		catch (Exception ex3)
		{
			LogManager.LogError("Error with Movement.LoadMacros() at " + ex3.StackTrace + ": " + ex3.Message);
		}
		loadPreferencesTime = Time.time;
		if (File.Exists("SeralythMenu/Seralyth_Preferences.txt"))
		{
			try
			{
				Settings.LoadPreferences();
			}
			catch (Exception ex4)
			{
				LogManager.LogError("Error with Settings.LoadPreferences() at " + ex4.StackTrace + ": " + ex4.Message);
				((MonoBehaviour)CoroutineManager.instance).StartCoroutine(DelayLoadPreferences());
			}
		}
		try
		{
			Settings.LoadPCControls();
		}
		catch (Exception ex5)
		{
			LogManager.LogError("Error with Settings.LoadPCControls() at " + ex5.StackTrace + ": " + ex5.Message);
		}
		if (new DirectoryInfo(Path.Combine(FileUtilities.GetGamePath(), "SeralythMenu.Resources.Client")).CreationTime >= DateTime.Now.AddYears(1))
		{
			AchievementManager.UnlockAchievement(new AchievementManager.Achievement
			{
				name = "Veteran",
				description = "Use the menu for over a year.",
				icon = "Images/Achievements/veteran.png"
			});
		}
		if (PatchHandler.PatchErrors > 0)
		{
			if (PatchHandler.CriticalPatchFailed)
			{
				string text3 = "A critical patch has failed, and you have been blocked from joining rooms for safety reasons. Please report this as an issue to the GitHub repository.";
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> " + text3, 10000);
				((GorillaComputer)GorillaComputer.instance).GeneralFailureMessage(text3);
				if (NetworkSystem.Instance.InRoom)
				{
					NetworkSystem.Instance.ReturnToSinglePlayer();
				}
			}
			else
			{
				NotificationManager.SendNotification(string.Format("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> {0} patch{1} failed to initialize. Please report this as an issue to the GitHub repository.", PatchHandler.PatchErrors, (PatchHandler.PatchErrors > 1) ? "es" : ""), 10000);
			}
		}
		try
		{
			GameObject val2 = new GameObject("Seralyth_StumpUpdateDisplay");
			val2.AddComponent<StumpUpdateDisplay>();
			val2.SetActive(true);
		}
		catch (Exception ex6)
		{
			LogManager.LogError("Error initializing StumpUpdateDisplay: " + ex6.Message);
		}
	}

	public static void Prefix()
	{
		//IL_14f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_14fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_2842: Unknown result type (might be due to invalid IL or missing references)
		//IL_2847: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_287e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2853: Unknown result type (might be due to invalid IL or missing references)
		//IL_285d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2862: Unknown result type (might be due to invalid IL or missing references)
		//IL_2877: Unknown result type (might be due to invalid IL or missing references)
		//IL_2883: Unknown result type (might be due to invalid IL or missing references)
		//IL_2888: Unknown result type (might be due to invalid IL or missing references)
		//IL_288d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_23d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_23de: Expected O, but got Unknown
		//IL_23f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_2405: Unknown result type (might be due to invalid IL or missing references)
		//IL_2420: Unknown result type (might be due to invalid IL or missing references)
		//IL_2425: Unknown result type (might be due to invalid IL or missing references)
		//IL_24e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_24f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_28be: Unknown result type (might be due to invalid IL or missing references)
		//IL_2899: Unknown result type (might be due to invalid IL or missing references)
		//IL_28a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_28a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_28b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_2519: Unknown result type (might be due to invalid IL or missing references)
		//IL_2523: Expected O, but got Unknown
		//IL_28c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_28c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_28cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_2569: Unknown result type (might be due to invalid IL or missing references)
		//IL_28fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_28d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_28e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_28e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_28f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_19cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_19d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_25bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_2903: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d40: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a04: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d51: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a53: Unknown result type (might be due to invalid IL or missing references)
		//IL_3996: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e04: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dfd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c43: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c52: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aa2: Unknown result type (might be due to invalid IL or missing references)
		//IL_27d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_27e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_261d: Unknown result type (might be due to invalid IL or missing references)
		//IL_260a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e09: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2622: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e21: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c77: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c86: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0686: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2090: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e67: Unknown result type (might be due to invalid IL or missing references)
		//IL_20cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_20d6: Expected O, but got Unknown
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2109: Unknown result type (might be due to invalid IL or missing references)
		//IL_20f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_26a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_26bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_26e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_26fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_2674: Unknown result type (might be due to invalid IL or missing references)
		//IL_267e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e54: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e95: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea4: Unknown result type (might be due to invalid IL or missing references)
		//IL_2137: Unknown result type (might be due to invalid IL or missing references)
		//IL_2130: Unknown result type (might be due to invalid IL or missing references)
		//IL_2da8: Unknown result type (might be due to invalid IL or missing references)
		//IL_2dad: Unknown result type (might be due to invalid IL or missing references)
		//IL_2db0: Unknown result type (might be due to invalid IL or missing references)
		//IL_2db3: Invalid comparison between Unknown and I4
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_213c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2774: Unknown result type (might be due to invalid IL or missing references)
		//IL_2776: Unknown result type (might be due to invalid IL or missing references)
		//IL_2789: Unknown result type (might be due to invalid IL or missing references)
		//IL_2db5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_2154: Unknown result type (might be due to invalid IL or missing references)
		//IL_2dbe: Unknown result type (might be due to invalid IL or missing references)
		//IL_2dc1: Invalid comparison between Unknown and I4
		//IL_2179: Unknown result type (might be due to invalid IL or missing references)
		//IL_2169: Unknown result type (might be due to invalid IL or missing references)
		//IL_21c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ec5: Unknown result type (might be due to invalid IL or missing references)
		//IL_21ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_21dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f80: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		if (Time.frameCount % 30 == 0)
		{
			Quests.CheckQuests();
		}
		try
		{
			rightPrimary = ((ControllerInputPoller)ControllerInputPoller.instance).rightControllerPrimaryButton || UnityInput.GetKey(Settings.pcBindings[Settings.ControllerBinding.RightPrimaryButton]);
			rightSecondary = ((ControllerInputPoller)ControllerInputPoller.instance).rightControllerSecondaryButton || UnityInput.GetKey(Settings.pcBindings[Settings.ControllerBinding.RightSecondaryButton]);
			leftPrimary = ((ControllerInputPoller)ControllerInputPoller.instance).leftControllerPrimaryButton || UnityInput.GetKey(Settings.pcBindings[Settings.ControllerBinding.LeftPrimaryButton]);
			leftSecondary = ((ControllerInputPoller)ControllerInputPoller.instance).leftControllerSecondaryButton || UnityInput.GetKey(Settings.pcBindings[Settings.ControllerBinding.LeftSecondaryButton]);
			leftGrab = ((ControllerInputPoller)ControllerInputPoller.instance).leftGrab || UnityInput.GetKey(Settings.pcBindings[Settings.ControllerBinding.LeftGrip]);
			rightGrab = ((ControllerInputPoller)ControllerInputPoller.instance).rightGrab || UnityInput.GetKey(Settings.pcBindings[Settings.ControllerBinding.RightGrip]);
			leftTrigger = ControllerInputPoller.TriggerFloat((XRNode)4);
			rightTrigger = ControllerInputPoller.TriggerFloat((XRNode)5);
			leftTriggerPressed = leftTrigger >= 0.5f;
			rightTriggerPressed = rightTrigger >= 0.5f;
			if (UnityInput.GetKey(Settings.pcBindings[Settings.ControllerBinding.LeftTrigger]))
			{
				leftTrigger = 1f;
			}
			if (UnityInput.GetKey(Settings.pcBindings[Settings.ControllerBinding.RightTrigger]))
			{
				rightTrigger = 1f;
			}
			if (IsSteam)
			{
				leftJoystick = SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.GetAxis((SteamVR_Input_Sources)1);
				rightJoystick = SteamVR_Actions.gorillaTag_RightJoystick2DAxis.GetAxis((SteamVR_Input_Sources)2);
				leftJoystickClick = SteamVR_Actions.gorillaTag_LeftJoystickClick.GetState((SteamVR_Input_Sources)1);
				rightJoystickClick = SteamVR_Actions.gorillaTag_RightJoystickClick.GetState((SteamVR_Input_Sources)2);
			}
			else
			{
				((InputDevice)(ref ((ControllerInputPoller)ControllerInputPoller.instance).leftControllerDevice)).TryGetFeatureValue(CommonUsages.primary2DAxis, ref leftJoystick);
				((InputDevice)(ref ((ControllerInputPoller)ControllerInputPoller.instance).rightControllerDevice)).TryGetFeatureValue(CommonUsages.primary2DAxis, ref rightJoystick);
				((InputDevice)(ref ((ControllerInputPoller)ControllerInputPoller.instance).leftControllerDevice)).TryGetFeatureValue(CommonUsages.primary2DAxisClick, ref leftJoystickClick);
				((InputDevice)(ref ((ControllerInputPoller)ControllerInputPoller.instance).rightControllerDevice)).TryGetFeatureValue(CommonUsages.primary2DAxisClick, ref rightJoystickClick);
			}
			bool flag = UnityInput.GetKey((Key)63) || UnityInput.GetKey((Key)64) || UnityInput.GetKey((Key)61) || UnityInput.GetKey((Key)62);
			bool key = UnityInput.GetKey(Settings.pcBindings[Settings.ControllerBinding.LeftOverride]);
			if (flag)
			{
				Vector2 val = default(Vector2);
				((Vector2)(ref val))._002Ector((UnityInput.GetKey((Key)62) ? 1f : 0f) + (UnityInput.GetKey((Key)61) ? (-1f) : 0f), (UnityInput.GetKey((Key)63) ? 1f : 0f) + (UnityInput.GetKey((Key)64) ? (-1f) : 0f));
				if (key)
				{
					rightJoystick = val;
				}
				else
				{
					leftJoystick = val;
				}
			}
			if (UnityInput.GetKey(Settings.pcBindings[Settings.ControllerBinding.JoystickClick]))
			{
				if (key)
				{
					rightJoystickClick = true;
				}
				else
				{
					leftJoystickClick = true;
				}
			}
			if (adaptiveButtons)
			{
				switch (ControllerUtilities.GetLeftControllerType())
				{
				case ControllerUtilities.ControllerType.ValveIndex:
					leftGrab = ((ControllerInputPoller)ControllerInputPoller.instance).leftControllerGripFloat > 0.75f;
					break;
				case ControllerUtilities.ControllerType.VIVE:
					leftPrimary = leftJoystickClick;
					break;
				}
				switch (ControllerUtilities.GetRightControllerType())
				{
				case ControllerUtilities.ControllerType.ValveIndex:
					rightGrab = ((ControllerInputPoller)ControllerInputPoller.instance).rightControllerGripFloat > 0.75f;
					break;
				case ControllerUtilities.ControllerType.VIVE:
					rightPrimary = rightJoystickClick;
					break;
				}
			}
			shouldBePC = !XRSettings.isDeviceActive;
		}
		catch
		{
		}
		try
		{
			if (!HasLoaded)
			{
				HasLoaded = true;
				OnLaunch();
			}
			Dictionary<int, bool> dictionary = new Dictionary<int, bool>
			{
				{ 0, leftPrimary },
				{ 1, leftSecondary },
				{ 2, leftGrab },
				{
					3,
					leftTrigger > 0.5f
				},
				{ 4, leftJoystickClick }
			};
			Dictionary<int, bool> dictionary2 = new Dictionary<int, bool>
			{
				{ 0, rightPrimary },
				{ 1, rightSecondary },
				{ 2, rightGrab },
				{
					3,
					rightTrigger > 0.5f
				},
				{ 4, rightJoystickClick }
			};
			bool flag2 = UnityInput.GetKey((Key)31) || (inTextInput && isKeyboardPc);
			bool flag3 = (rightHand ? dictionary2[menuButtonIndex] : dictionary[menuButtonIndex]);
			if (oneHand)
			{
				flag3 = (rightHand ? dictionary[menuButtonIndex] : dictionary2[menuButtonIndex]);
			}
			if (bothHands)
			{
				flag3 = dictionary2[menuButtonIndex] || dictionary[menuButtonIndex];
				if (flag3)
				{
					openedwithright = dictionary2[menuButtonIndex];
				}
			}
			if (!XRSettings.isDeviceActive)
			{
				flag3 = false;
			}
			if (wristMenu)
			{
				bool flag4 = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position + GorillaTagger.Instance.leftHandTransform.forward * 0.1f, ControllerUtilities.GetTrueRightHand().position) < 0.1f;
				if (rightHand)
				{
					flag4 = Vector3.Distance(ControllerUtilities.GetTrueLeftHand().position, GorillaTagger.Instance.rightHandTransform.position + GorillaTagger.Instance.rightHandTransform.forward * 0.1f) < 0.1f;
				}
				if (flag4 && !lastChecker)
				{
					wristOpen = !wristOpen;
				}
				lastChecker = flag4;
				flag3 = wristOpen;
			}
			if (joystickMenu)
			{
				bool flag5 = rightJoystickClick;
				if (flag5 && !lastChecker)
				{
					joystickOpen = !joystickOpen;
					joystickDelay = Time.time + 0.2f;
				}
				lastChecker = flag5;
				flag3 = joystickOpen;
			}
			else
			{
				joystickButtonSelected = 0;
			}
			if (physicalMenu)
			{
				if (flag3)
				{
					physicalOpenPosition = Vector3.zero;
				}
				flag3 = true;
			}
			if (lineMenu)
			{
				flag3 = true;
			}
			if (toggleButton)
			{
				if ((flag3 || flag2) && !toggleButtonHeld)
				{
					toggleButtonActive = !toggleButtonActive;
					keyboardWithToggleButton = flag2;
				}
				toggleButtonHeld = flag3 || flag2;
				flag3 = toggleButtonActive;
				flag2 = toggleButtonActive && keyboardWithToggleButton;
			}
			flag3 = flag3 || flag2;
			flag3 |= inTextInput;
			flag3 &= !Lockdown;
			if (watchMenu)
			{
				flag3 = flag2;
			}
			if (barkMenu)
			{
				flag3 = flag2 || barkMenuOpen;
			}
			isMenuButtonHeld = flag3;
			if (suppressCloseFrames > 0)
			{
				suppressCloseFrames--;
				flag3 = !((Object)(object)menu == (Object)null) || true;
			}
			if (flag3)
			{
				if ((Object)(object)menu == (Object)null)
				{
					OpenMenu();
				}
			}
			else if ((Object)(object)menu != (Object)null)
			{
				CloseMenu();
			}
			if (flag3 && (Object)(object)menu != (Object)null)
			{
				RecenterMenu();
			}
			try
			{
				if ((Object)(object)TPC == (Object)null)
				{
					try
					{
						TPC = GetObject("Player Objects/Third Person Camera/Shoulder Camera").GetComponent<Camera>();
					}
					catch
					{
						TPC = GetObject("Shoulder Camera").GetComponent<Camera>();
					}
				}
			}
			catch
			{
			}
			if (fpsCountAverage)
			{
				fpsAverageNumber *= 99f;
				fpsAverageNumber += 1f / Time.unscaledDeltaTime;
				fpsAverageNumber /= 100f;
			}
			else
			{
				fpsAverageNumber = 1f / Time.unscaledDeltaTime;
			}
			if (Time.time > fpsAvgTime || !fpsCountTimed)
			{
				lastDeltaTime = Mathf.Ceil(fpsAverageNumber);
				fpsAvgTime = Time.time + 1f;
			}
			if ((Object)(object)fpsCount != (Object)null)
			{
				string text = (ftCount ? $"FT: {Mathf.Floor(1f / lastDeltaTime * 10000f) / 10f} ms" : $"FPS: {lastDeltaTime}");
				if (hidetitle && !noPageNumber)
				{
					text += "      ";
				}
				if (disableFpsCounter)
				{
					text = "";
				}
				if (hidetitle && !noPageNumber)
				{
					text = text + "Page " + (pageNumber + 1);
				}
				if (!hidetitle && categoryDisplayMode == 0 && !disableCategoryDisplay && Buttons.CurrentCategoryName != "Main")
				{
					text = text + "  [" + Buttons.CurrentCategoryName + "]";
					if ((Buttons.CurrentCategoryName == "Admin Mods" || Buttons.CurrentCategoryName == "Console Assets") && adminWelcomeText != null)
					{
						if (!adminWelcomeDone)
						{
							if (adminWelcomeCharIndex < adminWelcomeText.Length)
							{
								if (Time.time >= adminWelcomeTypingTime)
								{
									adminWelcomeCharIndex++;
									adminWelcomeTypingTime = Time.time + 0.05f;
									if (adminWelcomeCharIndex >= adminWelcomeText.Length)
									{
										adminWelcomeDone = true;
									}
								}
							}
							else
							{
								adminWelcomeDone = true;
							}
						}
						text = text + "  " + adminWelcomeText.Substring(0, adminWelcomeCharIndex);
					}
				}
				((TMP_Text)fpsCount).text = FollowMenuSettings(text, translateText: false);
			}
			if (potatoTime.HasValue)
			{
				if (1f / Time.unscaledDeltaTime < 15f)
				{
					potatoTime += Time.unscaledDeltaTime;
					if (potatoTime > 60f)
					{
						potatoTime = null;
						AchievementManager.UnlockAchievement(new AchievementManager.Achievement
						{
							name = "Potato",
							description = "Have 15 FPS for over a minute.",
							icon = "Images/Achievements/potato.png"
						});
					}
				}
				else
				{
					potatoTime = 0f;
				}
			}
			if (adminTime.HasValue && PhotonNetwork.InRoom)
			{
				if (PhotonNetwork.PlayerListOthers.Any((Player player) => ServerData.Administrators.ContainsKey(player.UserId) && !Seralyth.Classes.Menu.Console.excludedCones.Contains(player)))
				{
					adminTime += Time.unscaledDeltaTime;
					if (adminTime > 10f)
					{
						adminTime = null;
						AchievementManager.UnlockAchievement(new AchievementManager.Achievement
						{
							name = "EEEEKK!",
							description = "Be in the same room as a Console administrator.",
							icon = "Images/Achievements/eeeekk.png"
						});
					}
				}
				else
				{
					adminTime = 0f;
				}
			}
			if ((Object)(object)watermarkImage != (Object)null)
			{
				((Transform)((Component)watermarkImage).GetComponent<RectTransform>()).localRotation = Quaternion.Euler(new Vector3(0f, 90f, 90f - (rockWatermark ? (Mathf.Sin(Time.time * 2f) * 10f) : 0f)));
			}
			if (animatedTitle && (Object)(object)title != (Object)null)
			{
				string text2 = (doCustomName ? NoRichtextTags(customMenuName) : "MrChicken Menu");
				if (categoryDisplayMode == 2 && !disableCategoryDisplay && Buttons.CurrentCategoryName != "Main")
				{
					text2 = Buttons.CurrentCategoryName;
				}
				else if (categoryDisplayMode == 1 && !disableCategoryDisplay && Buttons.CurrentCategoryName != "Main")
				{
					text2 = text2 + " <color=grey>[</color><color=white>" + Buttons.CurrentCategoryName + "</color><color=grey>]</color>";
				}
				int num = (int)Mathf.PingPong(Time.time / 0.25f, (float)(text2.Length + 1));
				((TMP_Text)title).text = ((num > 0) ? text2.Substring(0, num) : "");
			}
			if (gradientTitle && (Object)(object)title != (Object)null)
			{
				((TMP_Text)title).text = RichtextGradient(NoRichtextTags(((TMP_Text)title).text), (GradientColorKey[])(object)new GradientColorKey[3]
				{
					new GradientColorKey(BrightenColor(buttonColors[0].GetColor(0)), 0f),
					new GradientColorKey(BrightenColor(buttonColors[0].GetColor(0), 0.95f), 0.5f),
					new GradientColorKey(BrightenColor(buttonColors[0].GetColor(0)), 1f)
				});
			}
			if ((Object)(object)keyboardInputObject != (Object)null)
			{
				((TMP_Text)keyboardInputObject).text = FollowMenuSettings(keyboardInput, translateText: false) + ((Time.frameCount / 45 % 2 == 0) ? "|" : " ");
			}
			if (disorganized && Buttons.CurrentCategoryName != "Main")
			{
				Buttons.CurrentCategoryName = "Main";
				ReloadMenu();
			}
			if (longmenu && pageNumber != 0)
			{
				pageNumber = 0;
				ReloadMenu();
			}
			if (physicalMenu && (Object)(object)menu != (Object)null)
			{
				try
				{
					if (Vector3.Distance(((Component)GorillaTagger.Instance.bodyCollider).transform.position, menu.transform.position) < 1.5f)
					{
						if ((Object)(object)reference == (Object)null)
						{
							CreateReference();
						}
					}
					else if ((Object)(object)reference != (Object)null)
					{
						Object.Destroy((Object)(object)reference);
						reference = null;
					}
				}
				catch
				{
				}
			}
			if (themeType == 62)
			{
				if ((Object)(object)menu != (Object)null)
				{
					badAppleTime += Time.deltaTime;
					badAppleTime %= 203f;
				}
				if ((Object)(object)videoPlayer != (Object)null)
				{
					if (videoPlayer.isPlaying && (Object)(object)menu == (Object)null)
					{
						videoPlayer.Stop();
					}
					if (!videoPlayer.isPlaying && (Object)(object)menu != (Object)null)
					{
						videoPlayer.Play();
					}
				}
			}
			else if ((Object)(object)videoPlayer != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)videoPlayer).gameObject);
				videoPlayer = null;
			}
			try
			{
				if (GunSounds)
				{
					if (GunSpawned)
					{
						if (!lastGunSpawned)
						{
							AudioSource audioSource = (SwapGunHand ? VRRig.LocalRig.leftHandPlayer : VRRig.LocalRig.rightHandPlayer);
							audioSource.volume = (float)buttonClickVolume / 10f;
							AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Guns/grip-press.ogg", "Audio/Guns/grip-press.ogg", delegate(AudioClip clip)
							{
								audioSource.PlayOneShot(clip);
							});
						}
						if (GetGunInput(isShooting: true) && (!lastGunTrigger || ((Object)(object)handAudioManager != (Object)null && !handAudioManager.GetComponent<AudioSource>().isPlaying)))
						{
							AudioSource audioSource2 = (SwapGunHand ? VRRig.LocalRig.leftHandPlayer : VRRig.LocalRig.rightHandPlayer);
							audioSource2.volume = (float)buttonClickVolume / 10f;
							AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Guns/trigger-press.ogg", "Audio/Guns/trigger-press.ogg", delegate(AudioClip clip)
							{
								audioSource2.PlayOneShot(clip);
							});
							AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Guns/trigger-hold.ogg", "Audio/Guns/trigger-hold.ogg", delegate(AudioClip clip)
							{
								PlayHandAudio(clip, (float)buttonClickVolume / 10f, SwapGunHand);
							});
						}
						if (!GetGunInput(isShooting: true) && lastGunTrigger)
						{
							AudioSource audioSource3 = (SwapGunHand ? VRRig.LocalRig.leftHandPlayer : VRRig.LocalRig.rightHandPlayer);
							audioSource3.volume = (float)buttonClickVolume / 10f;
							AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Guns/trigger-release.ogg", "Audio/Guns/trigger-release.ogg", delegate(AudioClip clip)
							{
								audioSource3.PlayOneShot(clip);
							});
							GameObject obj5 = handAudioManager;
							if (obj5 != null)
							{
								obj5.GetComponent<AudioSource>().Stop();
							}
						}
					}
					else
					{
						if (lastGunSpawned)
						{
							AudioSource audioSource4 = (SwapGunHand ? VRRig.LocalRig.leftHandPlayer : VRRig.LocalRig.rightHandPlayer);
							audioSource4.volume = (float)buttonClickVolume / 10f;
							AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Guns/grip-release.ogg", "Audio/Guns/grip-release.ogg", delegate(AudioClip clip)
							{
								audioSource4.PlayOneShot(clip);
							});
						}
						if ((Object)(object)handAudioManager != (Object)null && handAudioManager.GetComponent<AudioSource>().isPlaying)
						{
							handAudioManager.GetComponent<AudioSource>().Stop();
							AudioSource audioSource5 = (SwapGunHand ? VRRig.LocalRig.leftHandPlayer : VRRig.LocalRig.rightHandPlayer);
							audioSource5.volume = (float)buttonClickVolume / 10f;
							AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Guns/trigger-release.ogg", "Audio/Guns/trigger-release.ogg", delegate(AudioClip clip)
							{
								audioSource5.PlayOneShot(clip);
							});
						}
					}
					lastGunSpawned = GunSpawned;
					lastGunTrigger = GetGunInput(isShooting: true);
				}
			}
			catch
			{
			}
			try
			{
				if (GunVibrations)
				{
					if (GunSpawned)
					{
						if (!lastGunSpawnedVibration)
						{
							GorillaTagger.Instance.StartVibration(SwapGunHand, GorillaTagger.Instance.tagHapticStrength / 2f, 0.05f);
						}
						if (GetGunInput(isShooting: true))
						{
							GorillaTagger.Instance.StartVibration(SwapGunHand, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
						}
					}
					else if (lastGunSpawnedVibration)
					{
						GorillaTagger.Instance.StartVibration(SwapGunHand, GorillaTagger.Instance.tagHapticStrength / 2f, 0.015f);
					}
					lastGunSpawnedVibration = GunSpawned;
				}
			}
			catch
			{
			}
			GunSpawned = false;
			UpdateKeyboard();
			if (annoyingMode)
			{
				CustomBoardManager.BoardMaterial.color = Color32.op_Implicit(new Color32((byte)226, (byte)74, (byte)44, byte.MaxValue));
				int num2 = Random.Range(1, 400);
				if (num2 == 21)
				{
					VRRig.LocalRig.PlayHandTapLocal(84, true, 0.4f);
					VRRig.LocalRig.PlayHandTapLocal(84, false, 0.4f);
					NotificationManager.SendNotification("<color=grey>[</color><color=#FF00FF>FUN FACT</color><color=grey>]</color> " + facts[Random.Range(0, facts.Length - 1)]);
				}
			}
			if (PhotonNetwork.InRoom)
			{
				if (partyKickReconnecting)
				{
					partyLastCode = null;
					partyKickReconnecting = false;
					NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully " + (waitForPlayerJoin ? "banned" : "kicked") + " " + amountPartying + " party member.");
					FriendshipGroupDetection.Instance.LeaveParty();
				}
				else if (partyLastCode != null && Time.time > partyTime && (!waitForPlayerJoin || PhotonNetwork.PlayerListOthers.Length != 0))
				{
					if (Buttons.GetIndex("Rejoin on Kick").enabled)
					{
						LogManager.Log("Attempting rejoin");
						NetworkSystem.Instance.ReturnToSinglePlayer();
						partyKickReconnecting = true;
					}
					else
					{
						NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully " + (waitForPlayerJoin ? "banned" : "kicked") + " " + amountPartying + " party member.");
						partyKickReconnecting = false;
						partyLastCode = null;
					}
				}
			}
			else if (partyKickReconnecting && partyLastCode != null && Time.time > partyTime && (!waitForPlayerJoin || PhotonNetwork.PlayerListOthers.Length != 0))
			{
				LogManager.Log("Attempting rejoin");
				((PhotonNetworkController)PhotonNetworkController.Instance).AttemptToJoinSpecificRoom(partyLastCode, (JoinType)0);
				partyTime = Time.time + (float)Important.reconnectDelay;
			}
			try
			{
				if (!RecorderPatch.enabled)
				{
					if (Sound.AudioIsPlaying && Time.time > Sound.RecoverTime)
					{
						Sound.StopAllSounds();
					}
				}
				else if (RecorderPatch.enabled)
				{
					bool flag6 = (VoiceManager.Get().AudioClips.Any() && !Sound.disableLocalSoundboard) || VoiceManager.Get().PostProcessors.Any() || VoiceManager.Get().Pitch != 1f;
					if (toggleDebugEchoMode != flag6)
					{
						NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.DebugEchoMode = flag6;
						toggleDebugEchoMode = flag6;
					}
				}
			}
			catch
			{
			}
			if (CurrentPrompt != null && CurrentPrompt.IsText && !inTextInput)
			{
				Settings.SpawnKeyboard();
			}
			if ((Object)(object)menu != (Object)null && (pageButtonType == 3 || pageButtonType == 4))
			{
				bool flag7 = ((pageButtonType == 3) ? leftGrab : (leftTrigger > 0.5f));
				bool flag8 = ((pageButtonType == 3) ? rightGrab : (rightTrigger > 0.5f));
				if (Time.time > pageButtonChangeDelay)
				{
					if (flag7)
					{
						pageButtonChangeDelay = Time.time + 0.2f;
						if (exclusivePageSounds || (!string.IsNullOrEmpty(SoundManager.DefaultSoundpack) && SoundManager.DefaultSoundpack != "None"))
						{
							SoundManager.Play("Previous");
						}
						else
						{
							SoundManager.Play("Button");
						}
						Toggle("PreviousPage");
					}
					if (flag8)
					{
						pageButtonChangeDelay = Time.time + 0.2f;
						if (exclusivePageSounds || (!string.IsNullOrEmpty(SoundManager.DefaultSoundpack) && SoundManager.DefaultSoundpack != "None"))
						{
							SoundManager.Play("Next");
						}
						else
						{
							SoundManager.Play("Button");
						}
						Toggle("NextPage");
					}
				}
				if (!flag7 && !flag8)
				{
					pageButtonChangeDelay = -1f;
				}
			}
			if (joystickMenu && joystickOpen)
			{
				Vector2 val2 = leftJoystick;
				if (Time.time > joystickDelay)
				{
					int num3 = PageSize;
					if (joystickMenuSearching)
					{
						num3++;
					}
					if (val2.x > 0.5f)
					{
						if (dynamicSounds)
						{
							SoundManager.Play("Next");
						}
						Toggle("NextPage");
						joystickDelay = Time.time + 0.2f;
					}
					if (val2.x < -0.5f)
					{
						if (dynamicSounds)
						{
							SoundManager.Play("Previous");
						}
						Toggle("PreviousPage");
						joystickDelay = Time.time + 0.2f;
					}
					if (val2.y > 0.5f)
					{
						if (dynamicSounds)
						{
							SoundManager.Play("Up");
						}
						joystickButtonSelected--;
						if (joystickButtonSelected < 0)
						{
							joystickButtonSelected = num3 - 1;
						}
						ReloadMenu();
						joystickDelay = Time.time + 0.2f;
					}
					if (val2.y < -0.5f)
					{
						if (dynamicSounds)
						{
							SoundManager.Play("Down");
						}
						joystickButtonSelected++;
						joystickButtonSelected %= num3;
						ReloadMenu();
						joystickDelay = Time.time + 0.2f;
					}
					if (leftJoystickClick)
					{
						if (dynamicSounds)
						{
							SoundManager.Play("Select");
						}
						ButtonInfo index = Buttons.GetIndex(joystickSelectedButton);
						if (index.incremental)
						{
							ToggleIncremental(joystickSelectedButton, leftTrigger < 0.5f);
						}
						else
						{
							Toggle(joystickSelectedButton, fromMenu: true);
						}
						ReloadMenu();
						joystickDelay = Time.time + 0.2f;
					}
				}
			}
			if (barkMenu)
			{
				if (barkMenuOpen)
				{
					if (!barkMenuGrabbed.HasValue)
					{
						if ((Object)(object)menuBackground != (Object)null)
						{
							bool flag9 = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, menuBackground.transform.position) < 0.4f && leftGrab;
							bool flag10 = Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, menuBackground.transform.position) < 0.4f && rightGrab;
							if (flag9 || flag10)
							{
								barkMenuGrabbed = flag9;
								rightHand = flag10;
								if ((Object)(object)reference != (Object)null)
								{
									Object.Destroy((Object)(object)reference);
									reference = null;
									CreateReference();
								}
							}
						}
					}
					else if (!(barkMenuGrabbed.Value ? leftGrab : rightGrab))
					{
						barkMenuGrabbed = null;
						barkMenuOpen = false;
					}
				}
				else
				{
					bool flag11 = IsBangingPosition(ControllerUtilities.GetTrueLeftHand().position) || IsBangingPosition(ControllerUtilities.GetTrueRightHand().position);
					if (flag11 && !previousBarkBangState)
					{
						if (Time.time > barkBangDelay)
						{
							barkBangCount = 0;
						}
						barkBangDelay = Time.time + 0.5f;
						barkBangCount++;
						if (barkBangCount >= 3)
						{
							barkMenuOpen = true;
						}
					}
					previousBarkBangState = flag11;
				}
			}
			if (pageScrolling)
			{
				bool flag12 = false;
				if (pageNumber != 0)
				{
					pageNumber = 0;
					flag12 = true;
				}
				Vector2 val3 = (rightHand ? leftJoystick : rightJoystick);
				if (Time.time > scrollDelay)
				{
					if (val3.y > 0.5f)
					{
						pageOffset = Mathf.Clamp(pageOffset - 1, 0, DisplayedItemCount - PageSize);
						flag12 = true;
						scrollDelay = Time.time + 0.1f;
					}
					if (val3.y < -0.5f)
					{
						pageOffset = Mathf.Clamp(pageOffset + 1, 0, DisplayedItemCount - PageSize);
						flag12 = true;
						scrollDelay = Time.time + 0.1f;
					}
				}
				if (flag12)
				{
					ReloadMenu();
				}
			}
			try
			{
				if (watchMenu)
				{
					watchShell.GetComponent<Renderer>().material = CustomBoardManager.BoardMaterial;
					ButtonInfo[] array = Buttons.buttons[Buttons.CurrentCategoryIndex];
					if (Buttons.CurrentCategoryName == "Favorite Mods")
					{
						array = StringsToInfos(favorites.ToArray());
					}
					if (Buttons.CurrentCategoryName == "Enabled Mods")
					{
						List<ButtonInfo> list = new List<ButtonInfo>();
						int categoryIndex = 0;
						ButtonInfo[][] buttons = Buttons.buttons;
						foreach (ButtonInfo[] source in buttons)
						{
							list.AddRange(source.Where((ButtonInfo v) => v.enabled && (!hideSettings || !Buttons.categoryNames[categoryIndex].Contains("Settings")) && (!hideMacros || !Buttons.categoryNames[categoryIndex].Contains("Macro"))));
							categoryIndex++;
						}
						list = list.OrderBy((ButtonInfo v) => v.overlapText ?? v.buttonText).ToList();
						list.Insert(0, Buttons.GetIndex("Exit Enabled Mods"));
						array = list.ToArray();
					}
					Text component = watchText.GetComponent<Text>();
					component.text = array[watchMenuIndex].buttonText;
					if (array[watchMenuIndex].overlapText != null)
					{
						component.text = array[watchMenuIndex].overlapText;
					}
					component.text += $"\n<color=grey>[{watchMenuIndex + 1}/{array.Length}]\n{DateTime.Now:hh:mm tt}</color>";
					((Graphic)component).color = textColors[0].GetCurrentColor();
					component.text = FollowMenuSettings(component.text, translateText: false);
					if ((Object)(object)watchIndicatorMat == (Object)null)
					{
						watchIndicatorMat = new Material(Shader.Find("GorillaTag/UberShader"));
					}
					watchIndicatorMat.color = (array[watchMenuIndex].enabled ? buttonColors[1].GetCurrentColor() : buttonColors[0].GetCurrentColor());
					((Graphic)watchEnabledIndicator.GetComponent<Image>()).material = watchIndicatorMat;
					Vector2 val4 = (rightHand ? rightJoystick : leftJoystick);
					if (Time.time > wristMenuDelay)
					{
						if (val4.x > 0.5f || (rightHand ? (val4.y < -0.5f) : (val4.y > 0.5f)))
						{
							watchMenuIndex++;
							if (watchMenuIndex > array.Length - 1)
							{
								watchMenuIndex = 0;
							}
							wristMenuDelay = Time.time + 0.2f;
						}
						if (val4.x < -0.5f || (rightHand ? (val4.y > 0.5f) : (val4.y < -0.5f)))
						{
							watchMenuIndex--;
							if (watchMenuIndex < 0)
							{
								watchMenuIndex = array.Length - 1;
							}
							wristMenuDelay = Time.time + 0.2f;
						}
						if (rightHand ? rightJoystickClick : leftJoystickClick)
						{
							int num5 = Buttons.CurrentCategoryIndex;
							Toggle(array[watchMenuIndex].buttonText, fromMenu: true);
							if (Buttons.CurrentCategoryIndex != num5)
							{
								watchMenuIndex = 0;
							}
							wristMenuDelay = Time.time + 0.2f;
						}
					}
				}
			}
			catch
			{
			}
			try
			{
				if (!hasLoadedPreferences && Time.time > loadPreferencesTime + 5f)
				{
					loadPreferencesTime = Time.time;
					try
					{
						LogManager.Log("Loading preferences due to load errors");
						Settings.LoadPreferences();
					}
					catch
					{
						LogManager.Log("Could not load preferences");
					}
				}
			}
			catch
			{
			}
			try
			{
				if (Time.time > autoSaveDelay && !Lockdown && hasLoadedPreferences)
				{
					autoSaveDelay = Time.time + 60f;
					Settings.SavePreferences();
					LogManager.Log("Automatically saved preferences");
					if (backupPreferences)
					{
						if (preferenceBackupCount >= 5)
						{
							File.WriteAllText("SeralythMenu/Backups/" + CurrentTimestamp().Replace(":", ".") + ".txt", Settings.SavePreferencesToText());
							preferenceBackupCount = 0;
						}
						preferenceBackupCount++;
					}
				}
			}
			catch
			{
			}
			try
			{
				if (!legacyGhostview && (Object)(object)GhostRig == (Object)null)
				{
					GameObject val5 = new GameObject("ghostRigHolder");
					val5.SetActive(false);
					GhostRig = Object.Instantiate<VRRig>(VRRig.LocalRig, ((Component)GTPlayer.Instance).transform.position, ((Component)GTPlayer.Instance).transform.rotation, val5.transform);
					GhostRig.headBodyOffset = Vector3.zero;
					((Component)GhostRig).gameObject.SetActive(false);
					((Component)GhostRig).transform.SetParent(((Component)VRRig.LocalRig).transform.parent);
					Object.Destroy((Object)(object)val5);
					((Component)((Component)GhostRig).transform.Find("VR Constraints/LeftArm/Left Arm IK/SlideAudio")).gameObject.SetActive(false);
					((Component)((Component)GhostRig).transform.Find("VR Constraints/RightArm/Right Arm IK/SlideAudio")).gameObject.SetActive(false);
					((Component)((Component)GhostRig).transform.Find("rig/body_pivot/SlideAudio")).gameObject.SetActive(false);
					((Behaviour)((Component)GhostRig).GetComponent<OwnershipGaurd>()).enabled = false;
					Visuals.FixRigMaterialESPColors(GhostRig);
					((Component)GhostRig).transform.position = Vector3.one * float.MaxValue;
				}
				if ((Object)(object)GhostMaterial == (Object)null)
				{
					GhostMaterial = new Material(Shader.Find("GUI/Text Shader"));
				}
				if ((Object)(object)legacyGhostViewLeft == (Object)null)
				{
					legacyGhostViewLeft = GameObject.CreatePrimitive((PrimitiveType)0);
					Object.Destroy((Object)(object)legacyGhostViewLeft.GetComponent<SphereCollider>());
					legacyGhostViewLeft.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
				}
				if ((Object)(object)legacyGhostViewRight == (Object)null)
				{
					legacyGhostViewRight = GameObject.CreatePrimitive((PrimitiveType)0);
					Object.Destroy((Object)(object)legacyGhostViewRight.GetComponent<SphereCollider>());
					legacyGhostViewRight.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
				}
				if ((!((Behaviour)VRRig.LocalRig).enabled || ghostException) && !disableGhostview)
				{
					Color val6 = (Buttons.GetIndex("Swap Ghostview Colors").enabled ? buttonColors[1].GetCurrentColor() : backgroundColor.GetCurrentColor());
					if (legacyGhostview)
					{
						if ((Object)(object)GhostRig != (Object)null && ((Component)GhostRig).gameObject.activeSelf)
						{
							((Component)GhostRig).gameObject.SetActive(false);
							((Component)GhostRig).transform.position = Vector3.one * float.MaxValue;
						}
						legacyGhostViewLeft.SetActive(true);
						legacyGhostViewLeft.transform.position = ControllerUtilities.GetTrueLeftHand().position;
						legacyGhostViewLeft.GetComponent<Renderer>().material.color = val6;
						legacyGhostViewRight.SetActive(true);
						legacyGhostViewRight.transform.position = ControllerUtilities.GetTrueRightHand().position;
						legacyGhostViewRight.GetComponent<Renderer>().material.color = val6;
					}
					else
					{
						if ((Object)(object)legacyGhostViewLeft != (Object)null && legacyGhostViewLeft.activeSelf)
						{
							legacyGhostViewLeft.SetActive(false);
						}
						if ((Object)(object)legacyGhostViewRight != (Object)null && legacyGhostViewRight.activeSelf)
						{
							legacyGhostViewRight.SetActive(false);
						}
						((Component)GhostRig).gameObject.SetActive(true);
						Color color = val6;
						color.a = 0.5f;
						GhostMaterial.color = color;
						((Renderer)GhostRig.mainSkin).material = GhostMaterial;
					}
				}
				else
				{
					if ((Object)(object)GhostRig != (Object)null)
					{
						((Component)GhostRig).gameObject.SetActive(false);
						((Component)GhostRig).transform.position = Vector3.one * float.MaxValue;
					}
					legacyGhostViewLeft.SetActive(false);
					legacyGhostViewRight.SetActive(false);
				}
			}
			catch
			{
			}
			frameCount++;
			playTime += Time.unscaledDeltaTime;
			if ((Object)(object)Settings.TutorialObject != (Object)null)
			{
				Settings.UpdateTutorial();
			}
			ServerPos = ((ServerPos == Vector3.zero) ? ServerSyncPos : Vector3.Lerp(ServerPos, VRRig.LocalRig.SanitizeVector3(ServerSyncPos), VRRig.LocalRig.lerpValueBody * 0.66f));
			ServerLeftHandPos = ((ServerLeftHandPos == Vector3.zero) ? ServerSyncLeftHandPos : Vector3.Lerp(ServerLeftHandPos, VRRig.LocalRig.SanitizeVector3(ServerSyncLeftHandPos), VRRig.LocalRig.lerpValueBody));
			ServerRightHandPos = ((ServerRightHandPos == Vector3.zero) ? ServerSyncRightHandPos : Vector3.Lerp(ServerRightHandPos, VRRig.LocalRig.SanitizeVector3(ServerSyncRightHandPos), VRRig.LocalRig.lerpValueBody));
			try
			{
				Dictionary<string, bool> dictionary3 = new Dictionary<string, bool>
				{
					{ "A", rightPrimary },
					{ "B", rightSecondary },
					{ "X", leftPrimary },
					{ "Y", leftSecondary },
					{ "LG", leftGrab },
					{ "RG", rightGrab },
					{
						"LT",
						leftTrigger > 0.5f
					},
					{
						"RT",
						rightTrigger > 0.5f
					},
					{ "LJ", leftJoystickClick },
					{ "RJ", rightJoystickClick }
				};
				foreach (KeyValuePair<string, List<string>> modBinding in ModBindings)
				{
					string key2 = modBinding.Key;
					List<string> value = modBinding.Value;
					if (value.Count <= 0)
					{
						continue;
					}
					bool flag13 = dictionary3[key2];
					foreach (string item in value)
					{
						ButtonInfo index2 = Buttons.GetIndex(item);
						if (index2 != null)
						{
							index2.customBind = key2;
							if ((ToggleBindings || !index2.isTogglable) && flag13 && !BindStates[key2])
							{
								Toggle(item, fromMenu: true, ignoreForce: true);
							}
							if (!ToggleBindings && ((flag13 && !index2.enabled) || (!flag13 && index2.enabled)))
							{
								Toggle(item, fromMenu: true, ignoreForce: true);
							}
						}
					}
					BindStates[key2] = flag13;
				}
				if (!string.IsNullOrEmpty(PCBindPendingMod))
				{
					ButtonInfo pendingBtn = Buttons.GetIndex(PCBindPendingMod);
					if (!IsPCBindMode)
					{
						string text3 = null;
						if (rightPrimary)
						{
							text3 = "A";
						}
						else if (rightSecondary)
						{
							text3 = "B";
						}
						else if (leftPrimary)
						{
							text3 = "X";
						}
						else if (leftSecondary)
						{
							text3 = "Y";
						}
						else if (leftGrab)
						{
							text3 = "LG";
						}
						else if (rightGrab)
						{
							text3 = "RG";
						}
						else if (leftTrigger > 0.5f)
						{
							text3 = "LT";
						}
						else if (rightTrigger > 0.5f)
						{
							text3 = "RT";
						}
						else if (leftJoystickClick)
						{
							text3 = "LJ";
						}
						else if (rightJoystickClick)
						{
							text3 = "RJ";
						}
						if (text3 != null && pendingBtn != null)
						{
							using (IEnumerator<KeyValuePair<string, List<string>>> enumerator3 = ModBindings.Where((KeyValuePair<string, List<string>> b) => b.Value.Contains(pendingBtn.buttonText)).GetEnumerator())
							{
								if (enumerator3.MoveNext())
								{
									enumerator3.Current.Value.Remove(pendingBtn.buttonText);
								}
							}
							pendingBtn.customBind = text3;
							pendingBtn.rebindKey = null;
							pendingBtn.pcBindKey = null;
							ModBindings[text3].Add(pendingBtn.buttonText);
							PCBindPendingMod = "";
							IsBinding = false;
							BindInput = "";
							NotificationManager.SendNotification("<color=grey>[</color><color=purple>BINDS</color><color=grey>]</color> Bound <color=green>" + pendingBtn.buttonText + "</color> to <color=green>" + text3 + "</color>.");
						}
					}
					else if (Input.anyKeyDown)
					{
						foreach (KeyCode value2 in Enum.GetValues(typeof(KeyCode)))
						{
							if ((int)value2 < 0 || !Input.GetKeyDown(value2) || (int)value2 <= 0)
							{
								continue;
							}
							if (pendingBtn != null)
							{
								using (IEnumerator<KeyValuePair<string, List<string>>> enumerator5 = ModBindings.Where((KeyValuePair<string, List<string>> b) => b.Value.Contains(pendingBtn.buttonText)).GetEnumerator())
								{
									if (enumerator5.MoveNext())
									{
										enumerator5.Current.Value.Remove(pendingBtn.buttonText);
									}
								}
								pendingBtn.customBind = null;
								pendingBtn.rebindKey = null;
								pendingBtn.pcBindKey = ((object)value2/*cast due to .constrained prefix*/).ToString();
							}
							PCBindPendingMod = "";
							IsBinding = false;
							BindInput = "";
							IsPCBindMode = false;
							NotificationManager.SendNotification($"<color=grey>[</color><color=purple>BINDS</color><color=grey>]</color> Bound <color=green>{pendingBtn?.buttonText}</color> to <color=green>{(object)value2}</color>.");
							break;
						}
					}
				}
			}
			catch
			{
			}
			try
			{
				Visuals.ClearLinePool();
				Visuals.ClearNameTagPool();
				if ((Object)(object)GunPointer != (Object)null)
				{
					if (!GunPointer.activeSelf)
					{
						Object.Destroy((Object)(object)GunPointer);
					}
					else
					{
						GunPointer.SetActive(false);
					}
				}
				if ((Object)(object)GunLine != (Object)null)
				{
					if (!((Component)GunLine).gameObject.activeSelf)
					{
						Object.Destroy((Object)(object)((Component)GunLine).gameObject);
						GunLine = null;
					}
					else
					{
						((Component)GunLine).gameObject.SetActive(false);
					}
				}
				List<(long, float)> list2 = new List<(long, float)>();
				foreach (KeyValuePair<(long, float), GameObject> item2 in Visuals.auraPool)
				{
					if (!item2.Value.activeSelf)
					{
						list2.Add(item2.Key);
						Object.Destroy((Object)(object)item2.Value);
					}
					else
					{
						item2.Value.SetActive(false);
					}
				}
				foreach (var item3 in list2)
				{
					Visuals.auraPool.Remove(item3);
				}
				List<long> list3 = new List<long>();
				foreach (KeyValuePair<long, GameObject> item4 in Visuals.cubePool)
				{
					if (!item4.Value.activeSelf)
					{
						list3.Add(item4.Key);
						Object.Destroy((Object)(object)item4.Value);
					}
					else
					{
						item4.Value.SetActive(false);
					}
				}
				foreach (long item5 in list3)
				{
					Visuals.cubePool.Remove(item5);
				}
				List<long> list4 = new List<long>();
				foreach (KeyValuePair<long, GameObject> item6 in Visuals.cylinderPool)
				{
					if (!item6.Value.activeSelf)
					{
						list3.Add(item6.Key);
						Object.Destroy((Object)(object)item6.Value);
					}
					else
					{
						item6.Value.SetActive(false);
					}
				}
				foreach (long item7 in list4)
				{
					Visuals.cylinderPool.Remove(item7);
				}
				List<string> list5 = new List<string>();
				foreach (KeyValuePair<string, GameObject> item8 in Visuals.labelDictionary)
				{
					if (!item8.Value.activeSelf)
					{
						list5.Add(item8.Key);
						Object.Destroy((Object)(object)item8.Value);
					}
					else
					{
						item8.Value.SetActive(false);
					}
				}
				foreach (string item9 in list5)
				{
					Visuals.labelDictionary.Remove(item9);
				}
			}
			catch
			{
			}
			PluginManager.ExecuteUpdate();
			foreach (ButtonInfo item10 in from button in Buttons.buttons.SelectMany((ButtonInfo[] result2) => result2)
				where (button.enabled || button.label) && (button.method != null || button.postMethod != null)
				select button)
			{
				try
				{
					bool flag14 = leftPrimary;
					bool flag15 = leftSecondary;
					bool flag16 = rightPrimary;
					bool flag17 = rightSecondary;
					bool flag18 = leftGrab;
					bool flag19 = rightGrab;
					float num6 = leftTrigger;
					float num7 = rightTrigger;
					bool flag20 = leftJoystickClick;
					bool flag21 = rightJoystickClick;
					if (OverwriteKeybinds && item10.customBind != null)
					{
						leftPrimary = true;
						leftSecondary = true;
						rightPrimary = true;
						rightSecondary = true;
						leftGrab = true;
						rightGrab = true;
						leftTrigger = 1f;
						rightTrigger = 1f;
						leftJoystickClick = true;
						rightJoystickClick = true;
					}
					try
					{
						bool flag22 = false;
						if (item10.rebindKey != null)
						{
							flag22 = true;
							float num8 = 0f;
							switch (item10.rebindKey)
							{
							case "A":
								num8 = (flag16 ? 1f : 0f);
								break;
							case "B":
								num8 = (flag17 ? 1f : 0f);
								break;
							case "X":
								num8 = (flag14 ? 1f : 0f);
								break;
							case "Y":
								num8 = (flag15 ? 1f : 0f);
								break;
							case "LG":
								num8 = (flag18 ? 1f : 0f);
								break;
							case "RG":
								num8 = (flag19 ? 1f : 0f);
								break;
							case "LT":
								num8 = num6;
								break;
							case "RT":
								num8 = num7;
								break;
							case "LJ":
								num8 = (flag20 ? 1f : 0f);
								break;
							case "RJ":
								num8 = (flag21 ? 1f : 0f);
								break;
							}
							leftPrimary = num8 > 0.5f;
							leftSecondary = num8 > 0.5f;
							rightPrimary = num8 > 0.5f;
							rightSecondary = num8 > 0.5f;
							leftGrab = num8 > 0.5f;
							rightGrab = num8 > 0.5f;
							leftTrigger = num8;
							rightTrigger = num8;
							leftJoystickClick = num8 > 0.5f;
							rightJoystickClick = num8 > 0.5f;
						}
						else if (item10.pcBindKey != null)
						{
							flag22 = true;
							float num9 = 0f;
							if (Enum.TryParse<KeyCode>(item10.pcBindKey, out KeyCode result))
							{
								num9 = (Input.GetKey(result) ? 1f : 0f);
							}
							leftPrimary = num9 > 0.5f;
							leftSecondary = num9 > 0.5f;
							rightPrimary = num9 > 0.5f;
							rightSecondary = num9 > 0.5f;
							leftGrab = num9 > 0.5f;
							rightGrab = num9 > 0.5f;
							leftTrigger = num9;
							rightTrigger = num9;
							leftJoystickClick = num9 > 0.5f;
							rightJoystickClick = num9 > 0.5f;
						}
						if (item10.postMethod != null)
						{
							postActions.Add(item10.buttonText);
						}
						item10.method?.Invoke();
						if (flag22)
						{
							leftPrimary = flag14;
							leftSecondary = flag15;
							rightPrimary = flag16;
							rightSecondary = flag17;
							leftGrab = flag18;
							rightGrab = flag19;
							leftTrigger = num6;
							rightTrigger = num7;
							leftJoystickClick = flag20;
							rightJoystickClick = flag21;
						}
					}
					catch (Exception ex)
					{
						LogManager.LogError("Error with mod method " + item10.buttonText + " at " + ex.StackTrace + ": " + ex.Message);
					}
					if (OverwriteKeybinds && item10.customBind != null)
					{
						leftPrimary = flag14;
						leftSecondary = flag15;
						rightPrimary = flag16;
						rightSecondary = flag17;
						leftGrab = flag18;
						rightGrab = flag19;
						leftTrigger = num6;
						rightTrigger = num7;
						leftJoystickClick = flag20;
						rightJoystickClick = flag21;
					}
				}
				catch
				{
				}
			}
		}
		catch (Exception ex2)
		{
			LogManager.LogError("Error with prefix at " + ex2.StackTrace + ": " + ex2.Message);
		}
		static bool IsBangingPosition(Vector3 position)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			Vector3 point = ((Collider)GorillaTagger.Instance.bodyCollider).ClosestPoint(position);
			return point.Distance(position) <= 0.1f;
		}
	}

	public static void Postfix()
	{
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			foreach (string postAction in postActions)
			{
				try
				{
					ButtonInfo index = Buttons.GetIndex(postAction);
					bool flag = leftPrimary;
					bool flag2 = leftSecondary;
					bool flag3 = rightPrimary;
					bool flag4 = rightSecondary;
					bool flag5 = leftGrab;
					bool flag6 = rightGrab;
					float num = leftTrigger;
					float num2 = rightTrigger;
					bool flag7 = leftJoystickClick;
					bool flag8 = rightJoystickClick;
					if (OverwriteKeybinds && index.customBind != null)
					{
						leftPrimary = true;
						leftSecondary = true;
						rightPrimary = true;
						rightSecondary = true;
						leftGrab = true;
						rightGrab = true;
						leftTrigger = 1f;
						rightTrigger = 1f;
						leftJoystickClick = true;
						rightJoystickClick = true;
					}
					try
					{
						bool flag9 = false;
						if (index.rebindKey != null)
						{
							flag9 = true;
							float num3 = 0f;
							switch (index.rebindKey)
							{
							case "A":
								num3 = (flag3 ? 1f : 0f);
								break;
							case "B":
								num3 = (flag4 ? 1f : 0f);
								break;
							case "X":
								num3 = (flag ? 1f : 0f);
								break;
							case "Y":
								num3 = (flag2 ? 1f : 0f);
								break;
							case "LG":
								num3 = (flag5 ? 1f : 0f);
								break;
							case "RG":
								num3 = (flag6 ? 1f : 0f);
								break;
							case "LT":
								num3 = num;
								break;
							case "RT":
								num3 = num2;
								break;
							case "LJ":
								num3 = (flag7 ? 1f : 0f);
								break;
							case "RJ":
								num3 = (flag8 ? 1f : 0f);
								break;
							}
							leftPrimary = num3 > 0.5f;
							leftSecondary = num3 > 0.5f;
							rightPrimary = num3 > 0.5f;
							rightSecondary = num3 > 0.5f;
							leftGrab = num3 > 0.5f;
							rightGrab = num3 > 0.5f;
							leftTrigger = num3;
							rightTrigger = num3;
							leftJoystickClick = num3 > 0.5f;
							rightJoystickClick = num3 > 0.5f;
						}
						else if (index.pcBindKey != null)
						{
							flag9 = true;
							float num4 = 0f;
							if (Enum.TryParse<KeyCode>(index.pcBindKey, out KeyCode result))
							{
								num4 = (Input.GetKey(result) ? 1f : 0f);
							}
							leftPrimary = num4 > 0.5f;
							leftSecondary = num4 > 0.5f;
							rightPrimary = num4 > 0.5f;
							rightSecondary = num4 > 0.5f;
							leftGrab = num4 > 0.5f;
							rightGrab = num4 > 0.5f;
							leftTrigger = num4;
							rightTrigger = num4;
							leftJoystickClick = num4 > 0.5f;
							rightJoystickClick = num4 > 0.5f;
						}
						index.postMethod();
						if (flag9)
						{
							leftPrimary = flag;
							leftSecondary = flag2;
							rightPrimary = flag3;
							rightSecondary = flag4;
							leftGrab = flag5;
							rightGrab = flag6;
							leftTrigger = num;
							rightTrigger = num2;
							leftJoystickClick = flag7;
							rightJoystickClick = flag8;
						}
					}
					catch (Exception ex)
					{
						LogManager.LogError("Error with mod postMethod " + index.buttonText + " at " + ex.StackTrace + ": " + ex.Message);
					}
					if (OverwriteKeybinds && index.customBind != null)
					{
						leftPrimary = flag;
						leftSecondary = flag2;
						rightPrimary = flag3;
						rightSecondary = flag4;
						leftGrab = flag5;
						rightGrab = flag6;
						leftTrigger = num;
						rightTrigger = num2;
						leftJoystickClick = flag7;
						rightJoystickClick = flag8;
					}
				}
				catch
				{
				}
			}
		}
		catch (Exception ex2)
		{
			LogManager.LogError("Error with postfix at " + ex2.StackTrace + ": " + ex2.Message);
		}
		postActions.Clear();
	}

	private unsafe static void UpdateKeyboard()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Invalid comparison between Unknown and I4
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Invalid comparison between Unknown and I4
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Invalid comparison between Unknown and I4
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Invalid comparison between Unknown and I4
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Invalid comparison between Unknown and I4
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Invalid comparison between Unknown and I4
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Invalid comparison between Unknown and I4
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Invalid comparison between Unknown and I4
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Invalid comparison between Unknown and I4
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Invalid comparison between Unknown and I4
		if ((Object)(object)VRKeyboard != (Object)null && Vector3.Distance(VRKeyboard.transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) > menuScale && !leftSecondary)
		{
			VRKeyboard.transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
			VRKeyboard.transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation;
		}
		if (!inTextInput || !isKeyboardPc)
		{
			lastPressedKeys.Clear();
			Key[] array = detectedKeys;
			foreach (Key val in array)
			{
				if (UnityInput.GetKey(val))
				{
					lastPressedKeys.Add(val);
				}
			}
			keyPressedTimes.Clear();
			return;
		}
		List<Key> list = new List<Key>();
		Key[] array2 = detectedKeys;
		for (int j = 0; j < array2.Length; j++)
		{
			Key val2 = array2[j];
			if (UnityInput.GetKey(val2))
			{
				if (keyPressedTimes.TryGetValue(val2, out var value))
				{
					float num = Mathf.Max(value.Item2 * 0.75f, 0.05f);
					if (!(Time.time > value.Item1))
					{
						list.Add(val2);
						continue;
					}
					keyPressedTimes[val2] = (Time.time + num, num);
				}
				else
				{
					keyPressedTimes[val2] = (Time.time + 0.5f, 0.5f);
				}
				list.Add(val2);
				if (lastPressedKeys.Contains(val2))
				{
					continue;
				}
				if (UnityInput.GetKey((Key)55))
				{
					Key val3 = val2;
					Key val4 = val3;
					if ((int)val4 <= 17)
					{
						if ((int)val4 != 15)
						{
							if ((int)val4 == 17)
							{
								GUIUtility.systemCopyBuffer = keyboardInput;
							}
						}
						else
						{
							keyboardInput = "";
						}
					}
					else if ((int)val4 != 36)
					{
						if ((int)val4 == 65 && !string.IsNullOrEmpty(keyboardInput))
						{
							string text = keyboardInput;
							keyboardInput = text.Substring(0, text.Length - 1);
						}
					}
					else
					{
						keyboardInput += GUIUtility.systemCopyBuffer;
					}
				}
				else
				{
					Key val5 = val2;
					Key val6 = val5;
					if ((int)val6 <= 2)
					{
						if ((int)val6 != 1)
						{
							if ((int)val6 != 2)
							{
								goto IL_0340;
							}
							HandleSearchOrPrompt();
						}
						else
						{
							keyboardInput += " ";
						}
					}
					else if ((int)val6 != 60)
					{
						if ((int)val6 != 65)
						{
							goto IL_0340;
						}
						if (!string.IsNullOrEmpty(keyboardInput))
						{
							string text = keyboardInput;
							keyboardInput = text.Substring(0, text.Length - 1);
						}
					}
					else
					{
						Toggle(isSearching ? "Search" : "Decline Prompt");
					}
				}
				goto IL_03d2;
			}
			keyPressedTimes.Remove(val2);
			continue;
			IL_0340:
			string text2 = ((object)(*(Key*)(&val2))/*cast due to .constrained prefix*/).ToString();
			if (text2.Length == 1)
			{
				bool flag = UnityInput.GetKey((Key)51) || UnityInput.GetKey((Key)52);
				keyboardInput += (flag ? text2.ToUpper() : text2.ToLower());
			}
			else if (text2.StartsWith("Digit"))
			{
				keyboardInput += text2.Replace("Digit", "");
			}
			goto IL_03d2;
			IL_03d2:
			if (pcKeyboardSounds)
			{
				VRRig.LocalRig.PlayHandTapLocal(66, false, (float)buttonClickVolume / 10f);
			}
			pageNumber = 0;
			if (!clickGUI)
			{
				ReloadMenu();
			}
			else if (clickGUI && isSearching)
			{
				Settings.UpdateSearch();
			}
		}
		lastPressedKeys = list;
	}

	private static void HandleSearchOrPrompt()
	{
		if (isSearching)
		{
			List<ButtonInfo> list = new List<ButtonInfo>();
			if (nonGlobalSearch && Buttons.CurrentCategoryName != "Main")
			{
				ButtonInfo[] array = Buttons.buttons[Buttons.CurrentCategoryIndex];
				foreach (ButtonInfo buttonInfo in array)
				{
					try
					{
						List<string> list2 = ((buttonInfo.aliases == null) ? new List<string>() : buttonInfo.aliases.ToList());
						list2.Add(buttonInfo.overlapText ?? buttonInfo.buttonText);
						if (list2.Any((string buttonText) => buttonText.ClearTags().Replace(" ", "").ToLower()
							.Contains(keyboardInput.Replace(" ", "").ToLower())))
						{
							list.Add(buttonInfo);
						}
					}
					catch
					{
					}
				}
			}
			else
			{
				int num = 0;
				ButtonInfo[][] buttons = Buttons.buttons;
				foreach (ButtonInfo[] array2 in buttons)
				{
					ButtonInfo[] array3 = array2;
					foreach (ButtonInfo buttonInfo2 in array3)
					{
						try
						{
							if (((!Buttons.categoryNames[num].Contains("Admin") && !(Buttons.categoryNames[num] == "Mod Givers")) || isAdmin) && (!buttonInfo2.detected || allowDetected))
							{
								List<string> list3 = ((buttonInfo2.aliases == null) ? new List<string>() : buttonInfo2.aliases.ToList());
								list3.Add(buttonInfo2.overlapText ?? buttonInfo2.buttonText);
								if (list3.Any((string buttonText) => buttonText.ClearTags().Replace(" ", "").ToLower()
									.Contains(keyboardInput.Replace(" ", "").ToLower())))
								{
									list.Add(buttonInfo2);
								}
							}
						}
						catch
						{
						}
					}
					num++;
				}
			}
			if (list.Count > 0)
			{
				ButtonInfo[] array4 = StringsToInfos(Alphabetize(InfosToStrings(list.ToArray())));
				ButtonInfo buttonInfo3 = array4[0];
				if (buttonInfo3.incremental)
				{
					ToggleIncremental(buttonInfo3.buttonText, UnityInput.GetKey((Key)51));
				}
				else
				{
					Toggle(array4[0].buttonText, fromMenu: true);
				}
			}
		}
		else if (CurrentPrompt != null && CurrentPrompt.IsText)
		{
			Toggle("Accept Prompt");
		}
	}

	public static void PressKeyboardKey(string key)
	{
		switch (key)
		{
		case "Space":
			keyboardInput += " ";
			break;
		case "Backspace":
			if (!string.IsNullOrEmpty(keyboardInput))
			{
				string text2 = keyboardInput;
				keyboardInput = text2.Substring(0, text2.Length - 1);
			}
			break;
		case "Shift":
			shift = !shift;
			break;
		case "CapsLock":
			lockShift = !lockShift;
			break;
		case "Clear":
			keyboardInput = "";
			break;
		case "Copy":
			GUIUtility.systemCopyBuffer = keyboardInput;
			break;
		case "Paste":
			keyboardInput += GUIUtility.systemCopyBuffer;
			break;
		default:
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{ "1", "!" },
				{ "2", "@" },
				{ "3", "#" },
				{ "4", "$" },
				{ "5", "%" },
				{ "6", "^" },
				{ "7", "&" },
				{ "8", "*" },
				{ "9", "(" },
				{ "0", ")" },
				{ "-", "_" },
				{ "=", "+" },
				{ "[", "{" },
				{ "]", "}" },
				{ "\\", "|" },
				{ ";", ":" },
				{ "'", "\"" },
				{ ",", "<" },
				{ ".", ">" },
				{ "/", "?" },
				{ "`", "~" }
			};
			bool flag = lockShift ^ shift;
			string text = key.ToLower();
			if (flag)
			{
				if (dictionary.TryGetValue(text, out var value))
				{
					keyboardInput += value;
				}
				else
				{
					keyboardInput += text.ToUpper();
				}
			}
			else
			{
				keyboardInput += text.ToLower();
			}
			shift = false;
			break;
		}
		}
		GTExt.GetOrAddComponent<ColorChanger>(((Component)KeyboardKey.keyLookupDictionary["CapsLock"]).gameObject).colors = buttonColors[lockShift ? 1 : 0];
		GTExt.GetOrAddComponent<ColorChanger>(((Component)KeyboardKey.keyLookupDictionary["Shift"]).gameObject).colors = buttonColors[shift ? 1 : 0];
		pageNumber = 0;
		if (!clickGUI)
		{
			ReloadMenu();
		}
		else if (clickGUI && isSearching)
		{
			Settings.UpdateSearch();
		}
	}

	private static void AddButton(float offset, int buttonIndex, ButtonInfo method)
	{
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		if (1 == 0)
		{
			return;
		}
		GameObject buttonObj = null;
		if (method != null && !method.label)
		{
			GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
			if (!UnityInput.GetKey((Key)31) && !isKeyboardPc)
			{
				val.layer = 2;
			}
			((Collider)val.GetComponent<BoxCollider>()).isTrigger = true;
			val.transform.parent = menu.transform;
			val.transform.rotation = Quaternion.identity;
			val.transform.localScale = (thinMenu ? new Vector3(0.09f, 0.9f, ButtonDistance * 0.8f) : new Vector3(0.09f, 1.3f, ButtonDistance * 0.8f));
			if (longmenu && buttonIndex >= PageSize)
			{
				Transform transform = menuBackground.transform;
				transform.localScale += new Vector3(0f, 0f, 0.1f);
				Transform transform2 = menuBackground.transform;
				transform2.localPosition += new Vector3(0f, 0f, -0.05f);
			}
			val.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - offset);
			if (checkMode && buttonIndex > -1)
			{
				val.transform.localScale = new Vector3(0.09f, 0.102f, 0.08f);
				val.transform.localPosition = (thinMenu ? new Vector3(0.56f, 0.399f, 0.28f - offset) : new Vector3(0.56f, 0.599f, 0.28f - offset));
			}
			ButtonCollider buttonCollider = val.AddComponent<ButtonCollider>();
			buttonCollider.relatedText = method.buttonText;
			if (incrementalButtons && method.incremental)
			{
				if (checkMode && buttonIndex > -1)
				{
					buttonCollider.incremental = true;
					buttonCollider.positive = false;
					RenderIncrementalText(increment: false, offset);
					RenderIncrementalButton(increment: true, offset, buttonIndex, method);
				}
				else
				{
					Transform transform3 = val.transform;
					transform3.localScale -= new Vector3(0f, 0.254f, 0f);
					Object.Destroy((Object)(object)buttonCollider);
					RenderIncrementalButton(increment: false, offset, buttonIndex, method);
					RenderIncrementalButton(increment: true, offset, buttonIndex, method);
				}
			}
			bool flag2 = swapButtonColors && buttonIndex < 0;
			if (lastClickedName != method.buttonText)
			{
				ColorChanger colorChanger = val.AddComponent<ColorChanger>();
				colorChanger.colors = buttonColors[(flag2 ^ method.enabled) ? 1 : 0];
				if (joystickMenu && buttonIndex == joystickButtonSelected)
				{
					joystickSelectedButton = method.buttonText;
					if (!colorChanger.colors.transparent)
					{
						ExtGradient extGradient = colorChanger.colors.Clone();
						extGradient.SetColor(0, Color.red);
						colorChanger.colors = extGradient;
					}
				}
			}
			else
			{
				((MonoBehaviour)CoroutineManager.instance).StartCoroutine(ButtonClick(buttonIndex, val.GetComponent<Renderer>()));
			}
			FollowMenuSettings(val, flag2 ? method.enabled : (!method.enabled));
			buttonObj = val;
		}
		GameObject val2 = new GameObject();
		val2.transform.parent = canvasObj.transform;
		TextMeshPro val3 = val2.AddComponent<TextMeshPro>();
		UpdateButtonText updateButtonText = ((Component)val3).gameObject.AddComponent<UpdateButtonText>();
		updateButtonText.Init(method, buttonIndex, offset);
		updateButtonText.UpdateText();
		if (dynamicAnimations && method != null && !method.label)
		{
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(RevealButton(buttonObj, val3, (float)buttonIndex * (slowDynamicAnimations ? 0.04f : 0.02f)));
		}
	}

	private static void AddSearchButton()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Expected O, but got Unknown
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
		if (!UnityInput.GetKey((Key)31) && !isKeyboardPc)
		{
			val.layer = 2;
		}
		((Collider)val.GetComponent<BoxCollider>()).isTrigger = true;
		val.transform.parent = menu.transform;
		val.transform.rotation = Quaternion.identity;
		val.transform.localScale = new Vector3(0.09f, 0.102f, 0.08f);
		val.transform.localPosition = (thinMenu ? new Vector3(0.56f, -0.45f, -0.58f) : new Vector3(0.56f, -0.7f, -0.58f));
		val.AddComponent<ButtonCollider>().relatedText = "Search";
		ColorChanger colorChanger = val.AddComponent<ColorChanger>();
		colorChanger.colors = buttonColors[(!(isSearching ^ !swapButtonColors)) ? 1u : 0u];
		if (joystickMenuSearching && joystickButtonSelected == PageSize)
		{
			joystickSelectedButton = "Search";
			ExtGradient extGradient = colorChanger.colors.Clone();
			extGradient.SetColor(0, Color.red);
			colorChanger.colors = extGradient;
		}
		FollowMenuSettings(val, isSearching ^ !swapButtonColors);
		GameObject val2 = new GameObject();
		val2.transform.parent = canvasObj.transform;
		Image val3 = val2.AddComponent<Image>();
		if ((Object)(object)searchIcon == (Object)null)
		{
			searchIcon = AssetUtilities.LoadTextureFromResource("SeralythMenu.Resources.Client.search.png");
		}
		if ((Object)(object)searchMat == (Object)null)
		{
			searchMat = new Material(((Graphic)val3).material);
		}
		((Graphic)val3).material = searchMat;
		((Graphic)val3).material.SetTexture("_MainTex", (Texture)(object)searchIcon);
		ComponentUtils.AddComponent<UIColorChanger>((Component)(object)val3).colors = textColors[(!isSearching) ? 1 : 2];
		RectTransform component = ((Component)val3).GetComponent<RectTransform>();
		((Transform)component).localPosition = Vector3.zero;
		component.sizeDelta = new Vector2(0.03f, 0.03f);
		((Transform)component).localPosition = (thinMenu ? new Vector3(0.064f, -7f / 52f, -29f / 130f) : new Vector3(0.064f, -0.20940171f, -29f / 130f));
		((Transform)component).rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
		FollowMenuSettings((MaskableGraphic)(object)val3);
	}

	private static void AddDebugButton()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Expected O, but got Unknown
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		bool enabled = Buttons.GetIndex("Info Screen").enabled;
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
		if (!UnityInput.GetKey((Key)31) && !isKeyboardPc)
		{
			val.layer = 2;
		}
		((Collider)val.GetComponent<BoxCollider>()).isTrigger = true;
		val.transform.parent = menu.transform;
		val.transform.rotation = Quaternion.identity;
		val.transform.localScale = new Vector3(0.09f, 0.102f, 0.08f);
		val.transform.localPosition = (thinMenu ? new Vector3(0.56f, 0.45f, -0.58f) : new Vector3(0.56f, 0.7f, -0.58f));
		val.AddComponent<ButtonCollider>().relatedText = "Info Screen";
		ColorChanger colorChanger = val.AddComponent<ColorChanger>();
		colorChanger.colors = buttonColors[(!(enabled ^ swapButtonColors)) ? 1u : 0u];
		FollowMenuSettings(val, enabled ^ swapButtonColors);
		GameObject val2 = new GameObject();
		val2.transform.parent = canvasObj.transform;
		Image val3 = val2.AddComponent<Image>();
		if ((Object)(object)debugIcon == (Object)null)
		{
			debugIcon = AssetUtilities.LoadTextureFromResource("SeralythMenu.Resources.Client.debug.png");
		}
		if ((Object)(object)debugMat == (Object)null)
		{
			debugMat = new Material(((Graphic)val3).material);
		}
		((Graphic)val3).material = debugMat;
		((Graphic)val3).material.SetTexture("_MainTex", (Texture)(object)debugIcon);
		ComponentUtils.AddComponent<UIColorChanger>((Component)(object)val3).colors = textColors[1];
		RectTransform component = ((Component)val3).GetComponent<RectTransform>();
		((Transform)component).localPosition = Vector3.zero;
		component.sizeDelta = new Vector2(0.03f, 0.03f);
		((Transform)component).localPosition = (thinMenu ? new Vector3(0.064f, 7f / 52f, -29f / 130f) : new Vector3(0.064f, 0.20940171f, -29f / 130f));
		((Transform)component).rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
		FollowMenuSettings((MaskableGraphic)(object)val3);
	}

	private static void AddDonateButton()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Expected O, but got Unknown
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
		if (!UnityInput.GetKey((Key)31) && !isKeyboardPc)
		{
			val.layer = 2;
		}
		((Collider)val.GetComponent<BoxCollider>()).isTrigger = true;
		val.transform.parent = menu.transform;
		val.transform.rotation = Quaternion.identity;
		val.transform.localScale = new Vector3(0.09f, 0.102f, 0.08f);
		val.transform.localPosition = (thinMenu ? new Vector3(0.56f, 0.45f, -0.58f) : new Vector3(0.56f, 0.7f, -0.58f));
		val.AddComponent<ButtonCollider>().relatedText = "Donate Button";
		ColorChanger colorChanger = val.AddComponent<ColorChanger>();
		colorChanger.colors = buttonColors[swapButtonColors ? 1 : 0];
		FollowMenuSettings(val, !swapButtonColors);
		GameObject val2 = new GameObject();
		val2.transform.parent = canvasObj.transform;
		Image val3 = val2.AddComponent<Image>();
		if ((Object)(object)donateIcon == (Object)null)
		{
			donateIcon = AssetUtilities.LoadTextureFromResource("SeralythMenu.Resources.Client.donate.png");
		}
		if ((Object)(object)donateMat == (Object)null)
		{
			donateMat = new Material(((Graphic)val3).material);
		}
		((Graphic)val3).material = donateMat;
		((Graphic)val3).material.SetTexture("_MainTex", (Texture)(object)donateIcon);
		ComponentUtils.AddComponent<UIColorChanger>((Component)(object)val3).colors = textColors[1];
		RectTransform component = ((Component)val3).GetComponent<RectTransform>();
		((Transform)component).localPosition = Vector3.zero;
		component.sizeDelta = new Vector2(0.03f, 0.03f);
		((Transform)component).localPosition = (thinMenu ? new Vector3(0.064f, 7f / 52f, -29f / 130f) : new Vector3(0.064f, 0.20940171f, -29f / 130f));
		((Transform)component).rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
		FollowMenuSettings((MaskableGraphic)(object)val3);
	}

	private static void AddUpdateButton()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Expected O, but got Unknown
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
		if (!UnityInput.GetKey((Key)31) && !isKeyboardPc)
		{
			val.layer = 2;
		}
		((Collider)val.GetComponent<BoxCollider>()).isTrigger = true;
		val.transform.parent = menu.transform;
		val.transform.rotation = Quaternion.identity;
		val.transform.localScale = new Vector3(0.09f, 0.102f, 0.08f);
		val.transform.localPosition = (thinMenu ? new Vector3(0.56f, 0.45f, -0.58f) : new Vector3(0.56f, 0.7f, -0.58f));
		val.AddComponent<ButtonCollider>().relatedText = "Update Button";
		ColorChanger colorChanger = val.AddComponent<ColorChanger>();
		colorChanger.colors = buttonColors[swapButtonColors ? 1 : 0];
		FollowMenuSettings(val, !swapButtonColors);
		GameObject val2 = new GameObject();
		val2.transform.parent = canvasObj.transform;
		Image val3 = val2.AddComponent<Image>();
		if ((Object)(object)updateIcon == (Object)null)
		{
			updateIcon = AssetUtilities.LoadTextureFromResource("SeralythMenu.Resources.Client.update.png");
		}
		if ((Object)(object)updateMat == (Object)null)
		{
			updateMat = new Material(((Graphic)val3).material);
		}
		((Graphic)val3).material = updateMat;
		((Graphic)val3).material.SetTexture("_MainTex", (Texture)(object)updateIcon);
		ComponentUtils.AddComponent<UIColorChanger>((Component)(object)val3).colors = textColors[1];
		RectTransform component = ((Component)val3).GetComponent<RectTransform>();
		((Transform)component).localPosition = Vector3.zero;
		component.sizeDelta = new Vector2(0.03f, 0.03f);
		((Transform)component).localPosition = (thinMenu ? new Vector3(0.064f, 7f / 52f, -29f / 130f) : new Vector3(0.064f, 0.20940171f, -29f / 130f));
		((Transform)component).rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
		FollowMenuSettings((MaskableGraphic)(object)val3);
	}

	private static void AddReturnButton(bool offcenteredPosition)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Expected O, but got Unknown
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
		if (!UnityInput.GetKey((Key)31) && !isKeyboardPc)
		{
			val.layer = 2;
		}
		((Collider)val.GetComponent<BoxCollider>()).isTrigger = true;
		val.transform.parent = menu.transform;
		val.transform.rotation = Quaternion.identity;
		val.transform.localScale = new Vector3(0.09f, 0.102f, 0.08f);
		val.transform.localPosition = (thinMenu ? new Vector3(0.56f, -0.45f, -0.58f) : new Vector3(0.56f, -0.7f, -0.58f));
		if (offcenteredPosition)
		{
			Transform transform = val.transform;
			transform.localPosition += new Vector3(0f, 0.16f, 0f);
		}
		val.AddComponent<ButtonCollider>().relatedText = "Global Return";
		if (lastClickedName != "Global Return")
		{
			ColorChanger colorChanger = val.AddComponent<ColorChanger>();
			colorChanger.colors = (colorChanger.colors = buttonColors[swapButtonColors ? 1 : 0]);
		}
		else
		{
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(ButtonClick(-99, val.GetComponent<Renderer>()));
		}
		FollowMenuSettings(val, !swapButtonColors);
		GameObject val2 = new GameObject();
		val2.transform.parent = canvasObj.transform;
		Image val3 = val2.AddComponent<Image>();
		if ((Object)(object)returnIcon == (Object)null)
		{
			returnIcon = AssetUtilities.LoadTextureFromResource("SeralythMenu.Resources.Client.return.png");
		}
		if ((Object)(object)returnMat == (Object)null)
		{
			returnMat = new Material(((Graphic)val3).material);
		}
		((Graphic)val3).material = returnMat;
		((Graphic)val3).material.SetTexture("_MainTex", (Texture)(object)returnIcon);
		ComponentUtils.AddComponent<UIColorChanger>((Component)(object)val3).colors = textColors[1];
		RectTransform component = ((Component)val3).GetComponent<RectTransform>();
		((Transform)component).localPosition = Vector3.zero;
		component.sizeDelta = new Vector2(0.03f, 0.03f);
		((Transform)component).localPosition = (thinMenu ? new Vector3(0.064f, -7f / 52f, -29f / 130f) : new Vector3(0.064f, -0.20940171f, -29f / 130f));
		if (offcenteredPosition)
		{
			((Transform)component).localPosition = ((Transform)component).localPosition + new Vector3(0f, 0.0475f, 0f);
		}
		((Transform)component).rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
		FollowMenuSettings((MaskableGraphic)(object)val3);
	}

	private static void RenderIncrementalButton(bool increment, float offset, int buttonIndex, ButtonInfo method)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		if (!method.label)
		{
			GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
			if (!UnityInput.GetKey((Key)31) && !isKeyboardPc)
			{
				val.layer = 2;
			}
			((Collider)val.GetComponent<BoxCollider>()).isTrigger = true;
			val.transform.parent = menu.transform;
			val.transform.rotation = Quaternion.identity;
			val.transform.localScale = new Vector3(0.09f, 0.102f, ButtonDistance * 0.8f);
			val.transform.localPosition = (thinMenu ? new Vector3(0.56f, 0.399f, 0.28f - offset) : new Vector3(0.56f, 0.599f, 0.28f - offset));
			ButtonCollider buttonCollider = val.AddComponent<ButtonCollider>();
			buttonCollider.relatedText = method.buttonText;
			buttonCollider.incremental = true;
			buttonCollider.positive = increment;
			if (increment)
			{
				val.transform.localPosition = new Vector3(val.transform.localPosition.x, 0f - val.transform.localPosition.y, val.transform.localPosition.z);
			}
			if (lastClickedName != method.buttonText + (increment ? "+" : "-"))
			{
				ColorChanger colorChanger = val.AddComponent<ColorChanger>();
				colorChanger.colors = buttonColors[0];
			}
			else
			{
				((MonoBehaviour)CoroutineManager.instance).StartCoroutine(ButtonClick(buttonIndex, val.GetComponent<Renderer>()));
			}
			FollowMenuSettings(val);
		}
		RenderIncrementalText(increment, offset);
	}

	public static void RenderIncrementalText(bool increment, float offset)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject();
		val.transform.parent = canvasObj.transform;
		TextMeshPro val2 = val.AddComponent<TextMeshPro>();
		((TMP_Text)val2).font = activeFont;
		((TMP_Text)val2).text = (increment ? "+" : "-");
		((TMP_Text)val2).richText = true;
		((TMP_Text)val2).fontSize = 1f;
		ComponentUtils.AddComponent<UIColorChanger>((Component)(object)val2).colors = textColors[1];
		((TMP_Text)val2).alignment = (TextAlignmentOptions)514;
		((TMP_Text)val2).fontStyle = activeFontStyle;
		((TMP_Text)val2).enableAutoSizing = true;
		((TMP_Text)val2).fontSizeMin = 0f;
		RectTransform component = ((Component)val2).GetComponent<RectTransform>();
		((Transform)component).localPosition = Vector3.zero;
		component.sizeDelta = new Vector2(0.2f, 0.03f * (ButtonDistance / 0.1f));
		if (NoAutoSizeText)
		{
			component.sizeDelta = new Vector2(9f, 0.015f);
		}
		if (hideTextOnCamera)
		{
			((Component)component).gameObject.layer = 19;
		}
		((Transform)component).localPosition = (thinMenu ? new Vector3(0.064f, increment ? (-0.12f) : 0.12f, 0.111f - offset / 2.6f) : new Vector3(0.064f, increment ? (-0.18f) : 0.18f, 0.111f - offset / 2.6f));
		((Transform)component).rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
		FollowMenuSettings((TMP_Text)(object)val2);
	}

	public static void CreateReference(bool? rightHandOverride = null)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		reference = GameObject.CreatePrimitive((PrimitiveType)0);
		Transform transform = reference.transform;
		bool? flag = rightHandOverride;
		bool num;
		if (!flag.HasValue)
		{
			if (rightHand)
			{
				goto IL_0058;
			}
			if (!bothHands)
			{
				goto IL_004c;
			}
			num = ((ControllerInputPoller)ControllerInputPoller.instance).rightControllerSecondaryButton;
		}
		else
		{
			num = flag == true;
		}
		if (!num)
		{
			goto IL_004c;
		}
		goto IL_0058;
		IL_0058:
		Transform parent = GorillaTagger.Instance.leftHandTransform;
		goto IL_0062;
		IL_0062:
		transform.parent = parent;
		reference.transform.localPosition = pointerOffset;
		reference.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		buttonCollider = reference.GetComponent<SphereCollider>();
		if (hidePointer)
		{
			reference.GetComponent<Renderer>().enabled = false;
			return;
		}
		ColorChanger colorChanger = reference.AddComponent<ColorChanger>();
		colorChanger.colors = backgroundColor;
		return;
		IL_004c:
		parent = GorillaTagger.Instance.rightHandTransform;
		goto IL_0062;
	}

	public static GameObject CreateMenu()
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fae: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fe8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fed: Unknown result type (might be due to invalid IL or missing references)
		//IL_2036: Unknown result type (might be due to invalid IL or missing references)
		//IL_2073: Unknown result type (might be due to invalid IL or missing references)
		//IL_208a: Unknown result type (might be due to invalid IL or missing references)
		//IL_20a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_20c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f47: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f81: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e02: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e07: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e08: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e18: Expected O, but got Unknown
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_085c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_072e: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b5: Expected O, but got Unknown
		//IL_07e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0767: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b0: Expected O, but got Unknown
		//IL_092e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0933: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ef2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f09: Unknown result type (might be due to invalid IL or missing references)
		//IL_1103: Unknown result type (might be due to invalid IL or missing references)
		//IL_1108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f42: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f92: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f97: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fdc: Expected O, but got Unknown
		//IL_1022: Unknown result type (might be due to invalid IL or missing references)
		//IL_1039: Unknown result type (might be due to invalid IL or missing references)
		//IL_1055: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_121b: Unknown result type (might be due to invalid IL or missing references)
		//IL_127f: Unknown result type (might be due to invalid IL or missing references)
		//IL_126e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0baf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bbe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d74: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_12a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_12c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e04: Unknown result type (might be due to invalid IL or missing references)
		//IL_14d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_151b: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_1549: Unknown result type (might be due to invalid IL or missing references)
		//IL_1574: Unknown result type (might be due to invalid IL or missing references)
		//IL_1579: Unknown result type (might be due to invalid IL or missing references)
		//IL_1624: Unknown result type (might be due to invalid IL or missing references)
		//IL_165c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1691: Unknown result type (might be due to invalid IL or missing references)
		//IL_16b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_16d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_171f: Unknown result type (might be due to invalid IL or missing references)
		//IL_173b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1740: Unknown result type (might be due to invalid IL or missing references)
		if (clickGUI)
		{
			menu = AssetUtilities.LoadObject<GameObject>("ClickGUI");
			Transform transform = menu.transform;
			transform.localScale *= menuScale * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
			Settings.InitializeClickGUI();
			RecenterMenu();
			return menu;
		}
		menu = GameObject.CreatePrimitive((PrimitiveType)3);
		Object.Destroy((Object)(object)menu.GetComponent<BoxCollider>());
		Object.Destroy((Object)(object)menu.GetComponent<Renderer>());
		menu.transform.localScale = new Vector3(0.1f, 0.3f, 0.3825f);
		if (annoyingMode)
		{
			menu.transform.localScale = new Vector3(0.1f, Random.Range(10f, 40f) / 100f, 0.3825f);
			backgroundColor = new ExtGradient
			{
				colors = ExtGradient.GetSimpleGradient(RandomUtilities.RandomColor(), RandomUtilities.RandomColor())
			};
			buttonColors[0] = new ExtGradient
			{
				colors = ExtGradient.GetSimpleGradient(RandomUtilities.RandomColor(), RandomUtilities.RandomColor())
			};
			buttonColors[1] = new ExtGradient
			{
				colors = ExtGradient.GetSimpleGradient(RandomUtilities.RandomColor(), RandomUtilities.RandomColor())
			};
			textColors[0] = new ExtGradient
			{
				colors = ExtGradient.GetSimpleGradient(RandomUtilities.RandomColor(), RandomUtilities.RandomColor())
			};
			textColors[1] = new ExtGradient
			{
				colors = ExtGradient.GetSimpleGradient(RandomUtilities.RandomColor(), RandomUtilities.RandomColor())
			};
			textColors[2] = new ExtGradient
			{
				colors = ExtGradient.GetSimpleGradient(RandomUtilities.RandomColor(), RandomUtilities.RandomColor())
			};
		}
		menuBackground = GameObject.CreatePrimitive((PrimitiveType)3);
		Object.Destroy((Object)(object)menuBackground.GetComponent<BoxCollider>());
		menuBackground.transform.parent = menu.transform;
		menuBackground.transform.localPosition = new Vector3(0.5f, 0f, 0f);
		menuBackground.transform.rotation = Quaternion.identity;
		menuBackground.transform.localScale = (thinMenu ? new Vector3(0.1f, 1f, 1f) : new Vector3(0.1f, 1.5f, 1f));
		if (innerOutline || themeType == 33)
		{
			GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
			Object.Destroy((Object)(object)val.GetComponent<BoxCollider>());
			val.transform.parent = menuBackground.transform;
			val.transform.rotation = Quaternion.identity;
			val.transform.localPosition = new Vector3(0f, -0.4840625f, 0f);
			val.transform.localScale = new Vector3(1.025f, 0.0065f, 0.98f);
			ColorChanger colorChanger = val.AddComponent<ColorChanger>();
			colorChanger.colors = buttonColors[1];
			val = GameObject.CreatePrimitive((PrimitiveType)3);
			Object.Destroy((Object)(object)val.GetComponent<BoxCollider>());
			val.transform.parent = menuBackground.transform;
			val.transform.rotation = Quaternion.identity;
			val.transform.localPosition = new Vector3(0f, 0.4840625f, 0f);
			val.transform.localScale = new Vector3(1.025f, 0.0065f, 0.98f);
			colorChanger = val.AddComponent<ColorChanger>();
			colorChanger.colors = buttonColors[1];
			val = GameObject.CreatePrimitive((PrimitiveType)3);
			Object.Destroy((Object)(object)val.GetComponent<BoxCollider>());
			val.transform.parent = menuBackground.transform;
			val.transform.rotation = Quaternion.identity;
			val.transform.localPosition = new Vector3(0f, 0f, -0.4875f);
			val.transform.localScale = new Vector3(1.025f, 0.968125f, 0.005f);
			colorChanger = val.AddComponent<ColorChanger>();
			colorChanger.colors = buttonColors[1];
			val = GameObject.CreatePrimitive((PrimitiveType)3);
			Object.Destroy((Object)(object)val.GetComponent<BoxCollider>());
			val.transform.parent = menuBackground.transform;
			val.transform.rotation = Quaternion.identity;
			val.transform.localPosition = new Vector3(0f, 0f, 0.4875f);
			val.transform.localScale = new Vector3(1.025f, 0.968125f, 0.005f);
			colorChanger = val.AddComponent<ColorChanger>();
			colorChanger.colors = buttonColors[1];
		}
		if (themeType == 24 || themeType == 25 || themeType == 26 || themeType == 62)
		{
			Renderer component = menuBackground.GetComponent<Renderer>();
			switch (themeType)
			{
			case 25:
				if ((Object)(object)pride == (Object)null)
				{
					pride = AssetUtilities.LoadTextureFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Images/Themes/pride.png", "Images/Themes/pride.png");
					((Texture)pride).filterMode = (FilterMode)0;
					((Texture)pride).wrapMode = (TextureWrapMode)1;
				}
				component.material.shader = Shader.Find("Universal Render Pipeline/Unlit");
				component.material.color = Color.white;
				component.material.mainTexture = (Texture)(object)pride;
				break;
			case 26:
				if ((Object)(object)trans == (Object)null)
				{
					trans = AssetUtilities.LoadTextureFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Images/Themes/trans.png", "Images/Themes/trans.png");
					((Texture)trans).filterMode = (FilterMode)0;
					((Texture)trans).wrapMode = (TextureWrapMode)1;
				}
				component.material.shader = Shader.Find("Universal Render Pipeline/Unlit");
				component.material.color = Color.white;
				component.material.mainTexture = (Texture)(object)trans;
				break;
			case 27:
				if ((Object)(object)gay == (Object)null)
				{
					gay = AssetUtilities.LoadTextureFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Images/Themes/mlm.png", "Images/Themes/mlm.png");
					((Texture)gay).filterMode = (FilterMode)0;
					((Texture)gay).wrapMode = (TextureWrapMode)1;
				}
				component.material.shader = Shader.Find("Universal Render Pipeline/Unlit");
				component.material.color = Color.white;
				component.material.mainTexture = (Texture)(object)gay;
				break;
			case 63:
				if ((Object)(object)videoPlayer == (Object)null)
				{
					videoPlayer = new GameObject("Seralyth_VideoPlayer").AddComponent<VideoPlayer>();
					videoPlayer.playOnAwake = true;
					videoPlayer.isLooping = true;
					videoPlayer.url = "https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Videos/Themes/badapple.mp4";
					RenderTexture val2 = new RenderTexture(192, 144, 0);
					val2.Create();
					videoPlayer.targetTexture = val2;
				}
				component.material.shader = Shader.Find("Universal Render Pipeline/Unlit");
				component.material.color = Color.white;
				component.material.SetTexture("_BaseMap", (Texture)(object)videoPlayer.targetTexture);
				videoPlayer.time = badAppleTime;
				break;
			}
		}
		else if (doCustomMenuBackground)
		{
			menuBackground.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Unlit");
			menuBackground.GetComponent<Renderer>().material.color = Color.white;
			menuBackground.GetComponent<Renderer>().material.mainTexture = (Texture)(object)customMenuBackgroundImage;
		}
		else
		{
			ColorChanger colorChanger2 = menuBackground.AddComponent<ColorChanger>();
			colorChanger2.colors = menuBackgroundColor;
		}
		FollowMenuSettings(menuBackground, shouldBeEnabled: false);
		canvasObj = new GameObject();
		canvasObj.transform.parent = menu.transform;
		Canvas val3 = canvasObj.AddComponent<Canvas>();
		if (hideTextOnCamera)
		{
			canvasObj.layer = 19;
		}
		CanvasScaler val4 = canvasObj.AddComponent<CanvasScaler>();
		val3.renderMode = (RenderMode)2;
		val4.dynamicPixelsPerUnit = 2500f;
		canvasObj.AddComponent<GraphicRaycaster>();
		if (!hidetitle)
		{
			GameObject val5 = new GameObject();
			val5.transform.parent = canvasObj.transform;
			title = val5.AddComponent<TextMeshPro>();
			((TMP_Text)title).font = activeFont;
			((TMP_Text)title).text = (translate ? "MrChicken Menu" : "<b>MrChicken Menu</b>");
			if (doCustomName)
			{
				((TMP_Text)title).text = customMenuName;
			}
			if (annoyingMode)
			{
				string[] array = new string[15]
				{
					"ModderX", "ShibaGT Gold", "Kman Menu", "WM TROLLING MENU", "ShibaGT Dark", "ShibaGT-X v5.5", "ii stupid", "ii's <b>Stupid</b> Menu", "bvunt menu", "GorillaTaggingKid Menu",
					"fart", "steal.lol", "Unttile menu", "Seralyth", "<b>Seralyth</b>"
				};
				if (Random.Range(1, 5) == 2)
				{
					((TMP_Text)title).text = array[Random.Range(0, array.Length)] + " v" + Random.Range(8, 159);
				}
			}
			((TMP_Text)title).text = FollowMenuSettings(((TMP_Text)title).text, !doCustomName, reloadOnTranslate: true);
			if (!noPageNumber)
			{
				TextMeshPro obj = title;
				((TMP_Text)obj).text = ((TMP_Text)obj).text + $" <color=grey>[</color><color=white>{(pageScrolling ? pageOffset : pageNumber) + 1}</color><color=grey>]</color>";
			}
			if (categoryDisplayMode == 1 && !disableCategoryDisplay && Buttons.CurrentCategoryName != "Main")
			{
				TextMeshPro obj2 = title;
				((TMP_Text)obj2).text = ((TMP_Text)obj2).text + " <color=grey>[</color><color=white>" + Buttons.CurrentCategoryName + "</color><color=grey>]</color>";
			}
			if (categoryDisplayMode == 2 && !disableCategoryDisplay && Buttons.CurrentCategoryName != "Main")
			{
				((TMP_Text)title).text = FollowMenuSettings(Buttons.CurrentCategoryName, translateText: true, reloadOnTranslate: true);
			}
			if (gradientTitle)
			{
				((TMP_Text)title).text = RichtextGradient(NoRichtextTags(((TMP_Text)title).text), (GradientColorKey[])(object)new GradientColorKey[3]
				{
					new GradientColorKey(BrightenColor(buttonColors[0].GetColor(0)), 0f),
					new GradientColorKey(BrightenColor(buttonColors[0].GetColor(0), 0.95f), 0.5f),
					new GradientColorKey(BrightenColor(buttonColors[0].GetColor(0)), 1f)
				});
			}
			if (animatedTitle)
			{
				string text = (doCustomName ? NoRichtextTags(customMenuName) : "MrChicken Menu");
				if (categoryDisplayMode == 2 && !disableCategoryDisplay && Buttons.CurrentCategoryName != "Main")
				{
					text = Buttons.CurrentCategoryName;
				}
				else if (categoryDisplayMode == 1 && !disableCategoryDisplay && Buttons.CurrentCategoryName != "Main")
				{
					text = text + " <color=grey>[</color><color=white>" + Buttons.CurrentCategoryName + "</color><color=grey>]</color>";
				}
				int num = (int)Mathf.PingPong(Time.time / 0.25f, (float)text.Length);
				((TMP_Text)title).text = ((num > 0) ? text.Substring(0, num) : "");
			}
			((TMP_Text)title).fontSize = 1f;
			ComponentUtils.AddComponent<UIColorChanger>((Component)(object)title).colors = textColors[0];
			((TMP_Text)title).richText = true;
			((TMP_Text)title).fontStyle = activeFontStyle;
			((TMP_Text)title).alignment = (TextAlignmentOptions)514;
			((TMP_Text)title).enableAutoSizing = true;
			((TMP_Text)title).fontSizeMin = 0f;
			RectTransform component2 = ((Component)title).GetComponent<RectTransform>();
			((Transform)component2).localPosition = Vector3.zero;
			component2.sizeDelta = new Vector2(0.28f, 0.05f);
			if (NoAutoSizeText)
			{
				component2.sizeDelta = new Vector2(0.28f, 0.015f);
			}
			if (hideTextOnCamera)
			{
				((Component)component2).gameObject.layer = 19;
			}
			((Transform)component2).localPosition = new Vector3(0.06f, 0f, 0.165f);
			((Transform)component2).rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
			FollowMenuSettings((TMP_Text)(object)title);
		}
		if (!backgroundColor.transparent)
		{
			GameObject val6 = new GameObject();
			val6.transform.parent = canvasObj.transform;
			TextMeshPro val7 = val6.AddComponent<TextMeshPro>();
			((TMP_Text)val7).font = activeFont;
			((TMP_Text)val7).text = "Build 10.0.2";
			((TMP_Text)val7).text = FollowMenuSettings(((TMP_Text)val7).text);
			((TMP_Text)val7).fontSize = 1f;
			ComponentUtils.AddComponent<UIColorChanger>((Component)(object)val7).colors = textColors[0];
			((TMP_Text)val7).richText = true;
			((TMP_Text)val7).fontStyle = activeFontStyle;
			((TMP_Text)val7).alignment = (TextAlignmentOptions)516;
			((TMP_Text)val7).enableAutoSizing = true;
			((TMP_Text)val7).fontSizeMin = 0f;
			RectTransform component3 = ((Component)val7).GetComponent<RectTransform>();
			((Transform)component3).localPosition = Vector3.zero;
			component3.sizeDelta = new Vector2(0.28f, 0.02f);
			((Transform)component3).position = (thinMenu ? new Vector3(0.04f, 0f, -0.17f) : new Vector3(0.04f, 0.07f, -0.17f));
			((Transform)component3).rotation = Quaternion.Euler(new Vector3(0f, 90f, 90f));
			FollowMenuSettings((TMP_Text)(object)val7);
			if (!disableWatermark)
			{
				GameObject val8 = new GameObject();
				val8.transform.parent = canvasObj.transform;
				watermarkImage = val8.AddComponent<Image>();
				if ((Object)(object)watermarkMat == (Object)null)
				{
					watermarkMat = new Material(((Graphic)watermarkImage).material);
				}
				((Graphic)watermarkImage).material = watermarkMat;
				((Graphic)watermarkImage).material.SetTexture("_MainTex", (Texture)(object)(customWatermark ?? AssetUtilities.LoadTextureFromResource("SeralythMenu.Resources.Client.icon.png")));
				RectTransform component4 = ((Component)watermarkImage).GetComponent<RectTransform>();
				((Transform)component4).localPosition = Vector3.zero;
				component4.sizeDelta = new Vector2(0.15f, 0.15f);
				((Transform)component4).localPosition = new Vector3(0.04f, 0f, 0f);
				FollowMenuSettings((MaskableGraphic)(object)watermarkImage);
				((Transform)component4).localRotation = Quaternion.Euler(new Vector3(0f, 90f, 90f - (rockWatermark ? (Mathf.Sin(Time.time * 2f) * 10f) : 0f)));
				if ((Object)(object)customWatermark == (Object)null)
				{
					ComponentUtils.AddComponent<UIColorChanger>((Component)(object)watermarkImage).colors = textColors[0];
				}
				else
				{
					((Graphic)watermarkImage).material.color = Color.white;
				}
			}
		}
		if (!disableFpsCounter)
		{
			GameObject val9 = new GameObject();
			val9.transform.parent = canvasObj.transform;
			TextMeshPro val10 = val9.AddComponent<TextMeshPro>();
			((TMP_Text)val10).font = activeFont;
			string text2 = (ftCount ? $"FT: {Mathf.Floor(1f / lastDeltaTime * 10000f) / 10f} ms" : $"FPS: {lastDeltaTime}");
			if (hidetitle && !noPageNumber)
			{
				text2 += "      ";
			}
			if (hidetitle && !noPageNumber)
			{
				text2 = text2 + "Page " + (pageNumber + 1);
			}
			((TMP_Text)val10).text = FollowMenuSettings(text2, translateText: false);
			ComponentUtils.AddComponent<UIColorChanger>((Component)(object)val10).colors = textColors[0];
			fpsCount = val10;
			((TMP_Text)val10).fontSize = 1f;
			((TMP_Text)val10).richText = true;
			((TMP_Text)val10).fontStyle = activeFontStyle;
			((TMP_Text)val10).alignment = (TextAlignmentOptions)514;
			((TMP_Text)val10).overflowMode = (TextOverflowModes)0;
			((TMP_Text)val10).enableAutoSizing = true;
			((TMP_Text)val10).fontSizeMin = 0f;
			RectTransform component5 = ((Component)val10).GetComponent<RectTransform>();
			component5.sizeDelta = (NoAutoSizeText ? new Vector2(9f, 0.015f) : new Vector2(0.28f, 0.02f));
			((Transform)component5).localPosition = new Vector3(0.06f, 0f, hidetitle ? 0.175f : 0.135f);
			((Transform)component5).rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
			if (hideTextOnCamera)
			{
				((Component)component5).gameObject.layer = 19;
			}
			FollowMenuSettings((TMP_Text)(object)val10);
		}
		float num2 = -0.3f;
		if (!disableDisconnectButton)
		{
			AddButton(-0.3f, -1, Buttons.GetIndex("Disconnect"));
			num2 -= ButtonDistance;
		}
		if (quickActions.Count > 0)
		{
			foreach (string item in quickActions.ToList())
			{
				ButtonInfo index = Buttons.GetIndex(item);
				if (index == null)
				{
					quickActions.Remove(item);
					continue;
				}
				AddButton(num2, -1, index);
				num2 -= ButtonDistance;
			}
		}
		if (!disableSearchButton)
		{
			AddSearchButton();
			if (!disableReturnButton && Buttons.CurrentCategoryName != "Main")
			{
				AddReturnButton(offcenteredPosition: true);
			}
		}
		else if (!disableReturnButton && Buttons.CurrentCategoryName != "Main")
		{
			AddReturnButton(offcenteredPosition: false);
		}
		if (enableDebugButton)
		{
			AddDebugButton();
		}
		else if (ServerData.OutdatedVersion)
		{
			AddUpdateButton();
		}
		if (!disablePageButtons && CurrentPrompt == null && !pageScrolling)
		{
			AddPageButtons();
		}
		if (inTextInput)
		{
			GameObject val11 = GameObject.CreatePrimitive((PrimitiveType)3);
			if (!UnityInput.GetKey((Key)31) && !isKeyboardPc)
			{
				val11.layer = 2;
			}
			((Collider)val11.GetComponent<BoxCollider>()).isTrigger = true;
			val11.transform.parent = menu.transform;
			val11.transform.rotation = Quaternion.identity;
			val11.transform.localScale = (thinMenu ? new Vector3(0.09f, 0.9f, ButtonDistance * 0.8f) : new Vector3(0.09f, 1.3f, ButtonDistance * 0.8f));
			val11.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - (float)buttonOffset * ButtonDistance);
			ColorChanger colorChanger3 = val11.AddComponent<ColorChanger>();
			colorChanger3.colors = buttonColors[0];
			FollowMenuSettings(val11);
			GameObject val12 = new GameObject();
			val12.transform.parent = canvasObj.transform;
			keyboardInputObject = val12.AddComponent<TextMeshPro>();
			((TMP_Text)keyboardInputObject).font = activeFont;
			((TMP_Text)keyboardInputObject).text = FollowMenuSettings(keyboardInput, translateText: false) + ((Time.time % 1f > 0.5f) ? "|" : "");
			((TMP_Text)keyboardInputObject).richText = true;
			((TMP_Text)keyboardInputObject).fontSize = 1f;
			if (joystickMenu && joystickButtonSelected == 0 && themeType == 29)
			{
				((Graphic)keyboardInputObject).color = Color.red;
			}
			else
			{
				ComponentUtils.AddComponent<UIColorChanger>((Component)(object)keyboardInputObject).colors = textColors[1];
			}
			((TMP_Text)keyboardInputObject).alignment = (TextAlignmentOptions)514;
			((TMP_Text)keyboardInputObject).fontStyle = activeFontStyle;
			((TMP_Text)keyboardInputObject).enableAutoSizing = true;
			((TMP_Text)keyboardInputObject).fontSizeMin = 0f;
			RectTransform component6 = ((Component)keyboardInputObject).GetComponent<RectTransform>();
			((Transform)component6).localPosition = Vector3.zero;
			component6.sizeDelta = new Vector2(0.2f, 0.03f * (ButtonDistance / 0.1f));
			if (NoAutoSizeText)
			{
				component6.sizeDelta = new Vector2(9f, 0.015f);
			}
			if (hideTextOnCamera)
			{
				((Component)component6).gameObject.layer = 19;
			}
			((Transform)component6).localPosition = new Vector3(0.064f, 0f, 0.111f - (float)buttonOffset * ButtonDistance / 2.6f);
			((Transform)component6).rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
			FollowMenuSettings((TMP_Text)(object)keyboardInputObject);
		}
		if ((Object)(object)promptVideoPlayer != (Object)null)
		{
			promptVideoPlayer.Stop();
			ObjectExtensions.Destroy((Object)(object)((Component)promptVideoPlayer).gameObject);
			promptVideoPlayer = null;
		}
		if (CurrentPrompt != null)
		{
			RenderPrompt();
		}
		else
		{
			int num3 = 0;
			try
			{
				ButtonInfo[] array5;
				if (isSearching)
				{
					List<ButtonInfo> list = new List<ButtonInfo>();
					if (nonGlobalSearch && Buttons.CurrentCategoryName != "Main")
					{
						ButtonInfo[] array2 = Buttons.buttons[Buttons.CurrentCategoryIndex];
						foreach (ButtonInfo buttonInfo in array2)
						{
							try
							{
								List<string> list2 = ((buttonInfo.aliases == null) ? new List<string>() : buttonInfo.aliases.ToList());
								list2.Add(buttonInfo.overlapText ?? buttonInfo.buttonText);
								if (list2.Any((string buttonText) => buttonText.ClearTags().Replace(" ", "").ToLower()
									.Contains(keyboardInput.Replace(" ", "").ToLower())))
								{
									list.Add(buttonInfo);
								}
							}
							catch
							{
							}
						}
					}
					else
					{
						int num4 = 0;
						ButtonInfo[][] buttons = Buttons.buttons;
						foreach (ButtonInfo[] array3 in buttons)
						{
							ButtonInfo[] array4 = array3;
							foreach (ButtonInfo buttonInfo2 in array4)
							{
								try
								{
									if (((!Buttons.categoryNames[num4].Contains("Admin") && !(Buttons.categoryNames[num4] == "Mod Givers")) || isAdmin) && (!buttonInfo2.detected || allowDetected))
									{
										List<string> list3 = ((buttonInfo2.aliases == null) ? new List<string>() : buttonInfo2.aliases.ToList());
										list3.Add(buttonInfo2.overlapText ?? buttonInfo2.buttonText);
										if (list3.Any((string buttonText) => buttonText.ClearTags().Replace(" ", "").ToLower()
											.Contains(keyboardInput.Replace(" ", "").ToLower())))
										{
											list.Add(buttonInfo2);
										}
									}
								}
								catch
								{
								}
							}
							num4++;
						}
					}
					num3++;
					array5 = list.ToArray();
				}
				else if (annoyingMode && Random.Range(1, 5) == 3)
				{
					ButtonInfo index2 = Buttons.GetIndex("Disconnect");
					array5 = Enumerable.Repeat(index2, 15).ToArray();
				}
				else
				{
					switch (Buttons.CurrentCategoryName)
					{
					case "Main":
					{
						List<ButtonInfo> list5 = new List<ButtonInfo>();
						ButtonInfo[] array6 = Buttons.buttons[Buttons.CurrentCategoryIndex];
						foreach (ButtonInfo buttonInfo3 in array6)
						{
							if (!skipButtons.Contains(buttonInfo3.buttonText))
							{
								list5.Add(buttonInfo3);
							}
						}
						array5 = list5.ToArray();
						break;
					}
					case "Favorite Mods":
						foreach (string item2 in favorites.Where((string favoriteMod) => Buttons.GetIndex(favoriteMod) == null).ToList())
						{
							favorites.Remove(item2);
						}
						array5 = StringsToInfos(favorites.ToArray());
						break;
					case "Enabled Mods":
					{
						List<ButtonInfo> list4 = new List<ButtonInfo>();
						int categoryIndex = 0;
						ButtonInfo[][] buttons2 = Buttons.buttons;
						foreach (ButtonInfo[] source in buttons2)
						{
							list4.AddRange(source.Where((ButtonInfo v) => v.enabled && (!hideSettings || !Buttons.categoryNames[categoryIndex].Contains("Settings")) && (!hideMacros || !Buttons.categoryNames[categoryIndex].Contains("Macro"))));
							categoryIndex++;
						}
						list4 = list4.OrderBy((ButtonInfo v) => v.buttonText).ToList();
						list4.Insert(0, Buttons.GetIndex("Exit Enabled Mods"));
						array5 = list4.ToArray();
						break;
					}
					case "Friends":
						FriendManager.FriendsListUpdated();
						array5 = Buttons.buttons[Buttons.CurrentCategoryIndex];
						break;
					default:
						array5 = Buttons.buttons[Buttons.CurrentCategoryIndex];
						break;
					}
				}
				if (legalOnly)
				{
					array5 = array5.Where((ButtonInfo b) => b.legal || b.label).ToArray();
				}
				if (Buttons.GetIndex("Alphabetize Menu").enabled || isSearching)
				{
					array5 = StringsToInfos(Alphabetize(InfosToStrings(array5)));
				}
				if (!longmenu)
				{
					array5 = array5.Skip(pageNumber * (PageSize - num3) + pageOffset).Take(PageSize - num3).ToArray();
				}
				for (int num9 = 0; num9 < array5.Length; num9++)
				{
					AddButton((float)(num9 + num3 + buttonOffset) * ButtonDistance, num9, array5[num9]);
				}
			}
			catch
			{
				LogManager.Log("Menu draw is erroring, returning to home page");
				Buttons.CurrentCategoryName = "Main";
			}
		}
		RecenterMenu();
		if (themeType == 49)
		{
			for (int num10 = 0; num10 < 15; num10++)
			{
				GameObject val13 = GameObject.CreatePrimitive((PrimitiveType)3);
				val13.transform.position = menuBackground.transform.position;
				val13.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
				Object.Destroy((Object)(object)val13.GetComponent<BoxCollider>());
				Object.Destroy((Object)(object)val13, 2f);
				val13.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Unlit");
				val13.GetComponent<Renderer>().material.color = Color.white;
				if ((Object)(object)cannabisMat == (Object)null)
				{
					cannabisMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
					{
						color = Color.white
					};
					if ((Object)(object)cann == (Object)null)
					{
						cann = AssetUtilities.LoadTextureFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Images/Themes/cannabis.png", "Images/Themes/cannabis.png");
					}
					cannabisMat.mainTexture = (Texture)(object)cann;
					cannabisMat.SetFloat("_Surface", 1f);
					cannabisMat.SetFloat("_Blend", 0f);
					cannabisMat.SetFloat("_SrcBlend", 5f);
					cannabisMat.SetFloat("_DstBlend", 10f);
					cannabisMat.SetFloat("_ZWrite", 0f);
					cannabisMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
					cannabisMat.renderQueue = 3000;
				}
				val13.GetComponent<Renderer>().material = cannabisMat;
				Component obj6 = val13.AddComponent(typeof(Rigidbody));
				Rigidbody val14 = (Rigidbody)(object)((obj6 is Rigidbody) ? obj6 : null);
				val14.position = menuBackground.transform.position;
				val14.linearVelocity = new Vector3(Random.Range(-3f, 3f), Random.Range(3f, 5f), Random.Range(-3f, 3f));
				val14.angularVelocity = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), Random.Range(-3f, 3f));
			}
		}
		Transform transform2 = menu.transform;
		transform2.localScale *= ((scaleWithPlayer && XRSettings.isDeviceActive) ? (GTPlayer.Instance.scale * menuScale) : menuScale);
		GameObject val15 = new GameObject("Features");
		val15.transform.parent = canvasObj.transform;
		Text val16 = val15.AddComponent<Text>();
		val16.font = currentFont;
		val16.text = "CREDITS\n 1x1x1x1VR12 \n Jalaxy \n FarmingKing \n Nebula for making loader";
		val16.fontSize = 1;
		((Graphic)val16).color = textColors[0].GetCurrentColor();
		val16.supportRichText = true;
		val16.fontStyle = (FontStyle)2;
		val16.alignment = (TextAnchor)4;
		val16.resizeTextForBestFit = true;
		val16.resizeTextMinSize = 0;
		RectTransform component7 = ((Component)val16).GetComponent<RectTransform>();
		((Transform)component7).localPosition = Vector3.zero;
		component7.sizeDelta = new Vector2(0.28f, 0.08f);
		((Transform)component7).position = new Vector3(0.06f, 0.275f, 0.125f);
		((Transform)component7).rotation = Quaternion.Euler(180f, 90f, 90f);
		return menu;
	}

	public static void RecenterMenu()
	{
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a20: Unknown result type (might be due to invalid IL or missing references)
		//IL_09dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a59: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a62: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a67: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a69: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a84: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a90: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a92: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0566: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_060a: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b13: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b43: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0750: Unknown result type (might be due to invalid IL or missing references)
		//IL_0755: Unknown result type (might be due to invalid IL or missing references)
		//IL_0774: Unknown result type (might be due to invalid IL or missing references)
		//IL_078d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0812: Unknown result type (might be due to invalid IL or missing references)
		//IL_0817: Unknown result type (might be due to invalid IL or missing references)
		//IL_081b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0820: Unknown result type (might be due to invalid IL or missing references)
		//IL_0822: Unknown result type (might be due to invalid IL or missing references)
		//IL_0833: Unknown result type (might be due to invalid IL or missing references)
		//IL_0838: Unknown result type (might be due to invalid IL or missing references)
		//IL_083d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0849: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1067: Unknown result type (might be due to invalid IL or missing references)
		//IL_106c: Unknown result type (might be due to invalid IL or missing references)
		//IL_103a: Unknown result type (might be due to invalid IL or missing references)
		//IL_104f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1011: Unknown result type (might be due to invalid IL or missing references)
		//IL_1016: Unknown result type (might be due to invalid IL or missing references)
		//IL_1025: Unknown result type (might be due to invalid IL or missing references)
		//IL_102a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_0707: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_0685: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_10dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1078: Unknown result type (might be due to invalid IL or missing references)
		//IL_1087: Unknown result type (might be due to invalid IL or missing references)
		//IL_1097: Unknown result type (might be due to invalid IL or missing references)
		//IL_090a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0929: Unknown result type (might be due to invalid IL or missing references)
		//IL_0899: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08be: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1119: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1108: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_10bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_111e: Unknown result type (might be due to invalid IL or missing references)
		//IL_112d: Unknown result type (might be due to invalid IL or missing references)
		//IL_094b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0950: Unknown result type (might be due to invalid IL or missing references)
		//IL_0954: Unknown result type (might be due to invalid IL or missing references)
		//IL_0959: Unknown result type (might be due to invalid IL or missing references)
		//IL_095b: Unknown result type (might be due to invalid IL or missing references)
		//IL_096c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0971: Unknown result type (might be due to invalid IL or missing references)
		//IL_0976: Unknown result type (might be due to invalid IL or missing references)
		//IL_0982: Unknown result type (might be due to invalid IL or missing references)
		//IL_0984: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c34: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c76: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cdb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e87: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0edc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fbb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f46: Unknown result type (might be due to invalid IL or missing references)
		bool flag = UnityInput.GetKey((Key)31) || (inTextInput && isKeyboardPc);
		Quaternion rotation;
		if (clickGUI)
		{
			if (!recenterPosition.HasValue || Vector3.Distance(recenterPosition.Value, ((Component)GorillaTagger.Instance.bodyCollider).transform.TransformPoint(new Vector3(0f, 0f, 1.5f))) > 1f)
			{
				menu.transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.TransformPoint(new Vector3(0f, 0f, 1.5f));
				menu.transform.position = new Vector3(menu.transform.position.x, ((Component)GorillaTagger.Instance.headCollider).transform.position.y + 0.15f, menu.transform.position.z);
				menu.transform.LookAt(((Component)GorillaTagger.Instance.bodyCollider).transform);
				menu.transform.rotation = Quaternion.Euler(0f, menu.transform.eulerAngles.y + 180f, 0f);
				recenterPosition = menu.transform.position;
				recenterRotation = menu.transform.rotation;
			}
			menu.transform.position = ((clickGUI && !XRSettings.isDeviceActive) ? Vector3.zero : recenterPosition.Value);
			menu.transform.rotation = (Quaternion)((clickGUI && !XRSettings.isDeviceActive) ? Quaternion.identity : (((_003F?)recenterRotation) ?? menu.transform.rotation));
		}
		else if (joystickMenu)
		{
			menu.transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.TransformPoint(joystickMenuPositions[joystickMenuPosition]);
			menu.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform);
			rotation = menu.transform.rotation;
			Vector3 eulerAngles = ((Quaternion)(ref rotation)).eulerAngles;
			eulerAngles += new Vector3(-90f, 0f, -90f);
			menu.transform.rotation = Quaternion.Euler(eulerAngles);
		}
		else if (!wristMenu)
		{
			if (frozenMenu)
			{
				menu.transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.TransformPoint(new Vector3(0f, 0f, 0.75f));
				menu.transform.position = new Vector3(menu.transform.position.x, ((Component)GorillaTagger.Instance.bodyCollider).transform.position.y + 0.3f, menu.transform.position.z);
				menu.transform.LookAt(((Component)GorillaTagger.Instance.bodyCollider).transform);
				menu.transform.rotation = Quaternion.Euler(0f, menu.transform.eulerAngles.y, 0f);
				rotation = menu.transform.rotation;
				Vector3 eulerAngles2 = ((Quaternion)(ref rotation)).eulerAngles;
				eulerAngles2 += new Vector3(-90f, 0f, -90f);
				menu.transform.rotation = Quaternion.Euler(eulerAngles2);
			}
			else if (lineMenu)
			{
				menu.transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.TransformPoint(new Vector3(0f, 0f, 0.75f));
				menu.transform.position = new Vector3(menu.transform.position.x, ((Component)GorillaTagger.Instance.bodyCollider).transform.position.y + 0.3f, menu.transform.position.z);
				menu.transform.LookAt(((Component)GorillaTagger.Instance.bodyCollider).transform);
				menu.transform.rotation = Quaternion.Euler(0f, menu.transform.eulerAngles.y, 0f);
				rotation = menu.transform.rotation;
				Vector3 eulerAngles3 = ((Quaternion)(ref rotation)).eulerAngles;
				eulerAngles3 += new Vector3(-90f, 0f, -90f);
				menu.transform.rotation = Quaternion.Euler(eulerAngles3);
			}
			else if (oneHand)
			{
				menu.transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.TransformPoint(new Vector3(0f, -0.1f, 0.5f));
				menu.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform);
				rotation = menu.transform.rotation;
				Vector3 eulerAngles4 = ((Quaternion)(ref rotation)).eulerAngles;
				eulerAngles4 += new Vector3(-90f, 0f, -90f);
				menu.transform.rotation = Quaternion.Euler(eulerAngles4);
			}
			else if (barkMenu && barkMenuOpen)
			{
				if (barkMenuGrabbed.HasValue)
				{
					if (barkMenuGrabbed.Value)
					{
						menu.transform.position = GorillaTagger.Instance.leftHandTransform.position;
						menu.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
					}
					else
					{
						menu.transform.position = GorillaTagger.Instance.rightHandTransform.position;
						rotation = GorillaTagger.Instance.rightHandTransform.rotation;
						Vector3 eulerAngles5 = ((Quaternion)(ref rotation)).eulerAngles;
						eulerAngles5 += new Vector3(0f, 0f, 180f);
						menu.transform.rotation = Quaternion.Euler(eulerAngles5);
					}
				}
				else
				{
					menu.transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.TransformPoint(new Vector3(0f, 0f, 0.5f));
					menu.transform.position = new Vector3(menu.transform.position.x, ((Component)GorillaTagger.Instance.headCollider).transform.position.y, menu.transform.position.z);
					menu.transform.LookAt(((Component)GorillaTagger.Instance.bodyCollider).transform);
					menu.transform.rotation = Quaternion.Euler(0f, menu.transform.eulerAngles.y, 0f);
					rotation = menu.transform.rotation;
					Vector3 eulerAngles6 = ((Quaternion)(ref rotation)).eulerAngles;
					eulerAngles6 += new Vector3(-90f, 0f, -90f);
					menu.transform.rotation = Quaternion.Euler(eulerAngles6);
				}
			}
			else
			{
				if (rightHand || (bothHands && ((ControllerInputPoller)ControllerInputPoller.instance).rightControllerSecondaryButton))
				{
					menu.transform.position = GorillaTagger.Instance.rightHandTransform.position;
					rotation = GorillaTagger.Instance.rightHandTransform.rotation;
					Vector3 eulerAngles7 = ((Quaternion)(ref rotation)).eulerAngles;
					eulerAngles7 += new Vector3(0f, 0f, 180f);
					menu.transform.rotation = Quaternion.Euler(eulerAngles7);
				}
				else
				{
					menu.transform.position = GorillaTagger.Instance.leftHandTransform.position;
					menu.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
				}
				if (flipMenu)
				{
					rotation = menu.transform.rotation;
					Vector3 eulerAngles8 = ((Quaternion)(ref rotation)).eulerAngles;
					eulerAngles8 += new Vector3(0f, 0f, 180f);
					menu.transform.rotation = Quaternion.Euler(eulerAngles8);
				}
			}
		}
		else
		{
			menu.transform.localPosition = Vector3.zero;
			menu.transform.localRotation = Quaternion.identity;
			menu.transform.position = (rightHand ? (GorillaTagger.Instance.rightHandTransform.position + new Vector3(0f, 0.3f, 0f)) : (GorillaTagger.Instance.leftHandTransform.position + new Vector3(0f, 0.3f, 0f)));
			menu.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
			rotation = menu.transform.rotation;
			Vector3 eulerAngles9 = ((Quaternion)(ref rotation)).eulerAngles;
			eulerAngles9 += new Vector3(-90f, 0f, -90f);
			menu.transform.rotation = Quaternion.Euler(eulerAngles9);
		}
		if (inTextInput && !isKeyboardPc && !clickGUI)
		{
			menu.transform.position = menuSpawnPosition.transform.position;
			menu.transform.rotation = menuSpawnPosition.transform.rotation;
			rotation = menu.transform.rotation;
			Vector3 eulerAngles10 = ((Quaternion)(ref rotation)).eulerAngles;
			eulerAngles10 += new Vector3(-90f, 90f, -90f);
			menu.transform.rotation = Quaternion.Euler(eulerAngles10);
		}
		if (flag)
		{
			((Component)GetObject("Shoulder Camera").transform.Find("CM vcam1")).gameObject.SetActive(false);
			if ((Object)(object)TPC != (Object)null)
			{
				isOnPC = true;
				if (!XRSettings.isDeviceActive)
				{
					PrivateUIRoom.instance.ToggleLevelVisibility(true);
				}
				if (joystickMenu)
				{
					Toggle("Joystick Menu");
				}
				if (watchMenu)
				{
					Toggle("Watch Menu");
				}
				if (physicalMenu)
				{
					Toggle("Physical Menu");
				}
				if (lineMenu)
				{
					Toggle("Line Menu");
				}
				Vector3[] array = new Vector3[6];
				Camera tPC = TPC;
				array[0] = ((tPC != null) ? ((Component)tPC).transform.position : ((Component)GorillaTagger.Instance.headCollider).transform.position);
				array[1] = new Vector3(10f, 10f, 10f);
				array[2] = new Vector3(10f, 10f, 10f);
				array[3] = new Vector3(-67.9299f, 11.9144f, -84.2019f);
				array[4] = new Vector3(-63f, 3.634f, -65f);
				array[5] = ((Component)VRRig.LocalRig).transform.position + ((Component)VRRig.LocalRig).transform.forward * 1.2f;
				Vector3[] array2 = (Vector3[])(object)array;
				((Component)TPC).transform.position = array2[pcbg];
				if (pcbg != 4 && pcbg != 0)
				{
					((Component)TPC).transform.rotation = Quaternion.identity;
				}
				if (pcbg == 5)
				{
					if ((Object)(object)pcBackground == (Object)null)
					{
						pcBackground = GameObject.CreatePrimitive((PrimitiveType)3);
						pcBackground.transform.localScale = new Vector3(10f, 10f, 0.01f);
						((Component)pcBackground.transform).transform.position = ((Component)TPC).transform.position + ((Component)TPC).transform.forward;
						OnMenuClosed += delegate
						{
							Object.Destroy((Object)(object)pcBackground);
						};
					}
					Color currentColor = menuBackgroundColor.GetCurrentColor();
					pcBackground.GetComponent<Renderer>().material.color = Color32.op_Implicit(new Color32((byte)(currentColor.r * 50f), (byte)(currentColor.g * 50f), (byte)(currentColor.b * 50f), byte.MaxValue));
				}
				menu.transform.parent = ((Component)TPC).transform;
				menu.transform.position = ((Component)TPC).transform.position + ((Component)TPC).transform.forward * 0.5f;
				menu.transform.rotation = ((clickGUI && !XRSettings.isDeviceActive) ? Quaternion.identity : (((Component)TPC).transform.rotation * Quaternion.Euler(-90f, 90f, 0f)));
				if ((Object)(object)reference != (Object)null)
				{
					if (Mouse.current.leftButton.isPressed && !isMouseDown)
					{
						Ray val = TPC.ScreenPointToRay(Vector2.op_Implicit(((InputControl<Vector2>)(object)((Pointer)Mouse.current).position).ReadValue()));
						RaycastHit val2 = default(RaycastHit);
						if (Physics.Raycast(val, ref val2, 512f, NoInvisLayerMask()))
						{
							ButtonCollider component = ((Component)((RaycastHit)(ref val2)).transform).gameObject.GetComponent<ButtonCollider>();
							if ((Object)(object)component != (Object)null)
							{
								component.OnTriggerEnter((Collider)(object)buttonCollider);
								buttonCooldown = -1f;
							}
						}
					}
					else
					{
						reference.transform.position = new Vector3(999f, -999f, -999f);
					}
					isMouseDown = Mouse.current.leftButton.isPressed;
				}
			}
		}
		else
		{
			isOnPC = false;
		}
		if (physicalMenu)
		{
			if (physicalOpenPosition == Vector3.zero)
			{
				physicalOpenPosition = menu.transform.position;
				physicalOpenRotation = menu.transform.rotation;
			}
			menu.transform.position = physicalOpenPosition;
			menu.transform.rotation = physicalOpenRotation;
		}
		if (smoothMenuPosition)
		{
			smoothTargetPosition = ((smoothTargetPosition == Vector3.zero) ? menu.transform.position : Vector3.Lerp(smoothTargetPosition, menu.transform.position, Time.deltaTime * 10f));
			menu.transform.position = smoothTargetPosition;
		}
		if (smoothMenuRotation)
		{
			smoothTargetRotation = ((smoothTargetRotation == Quaternion.identity) ? menu.transform.rotation : Quaternion.Lerp(smoothTargetRotation, menu.transform.rotation, Time.deltaTime * 10f));
			menu.transform.rotation = smoothTargetRotation;
		}
	}

	public static void ForceOpenMenu()
	{
		suppressCloseFrames = 10;
	}

	public static void OpenMenu()
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Main.OnMenuOpened?.Invoke();
		}
		catch
		{
		}
		if (dynamicSounds)
		{
			SoundManager.Play(SoundManager.DefaultSounds["Open"], null, null, null, overlapHand: false, leftOverlap: false, global: true);
		}
		CreateMenu();
		if (dynamicAnimations)
		{
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(GrowCoroutine());
		}
		if (particleSpawnEffect)
		{
			for (int i = 0; i < 25; i++)
			{
				GameObject val = GameObject.CreatePrimitive((PrimitiveType)0);
				val.transform.position = menu.transform.position;
				val.transform.localScale = Vector3.one * (0.025f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f));
				val.AddComponent<CustomParticle>();
				Object.Destroy((Object)(object)val.GetComponent<Collider>());
			}
		}
		menuOpenCount++;
		if (menuOpenCount == 100)
		{
			AchievementManager.UnlockAchievement(new AchievementManager.Achievement
			{
				name = "Persistent",
				description = "Open the menu 100 times.",
				icon = "Images/Achievements/persistent.png"
			});
		}
		networkMenuEnabled = true;
		NetworkMenuManager.EnableNetworkMenu();
		if (!joystickMenu && (Object)(object)reference == (Object)null)
		{
			CreateReference();
		}
	}

	public static void CloseMenu()
	{
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Expected O, but got Unknown
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Expected O, but got Unknown
		try
		{
			Main.OnMenuClosed?.Invoke();
		}
		catch
		{
		}
		((Component)GetObject("Shoulder Camera").transform.Find("CM vcam1")).gameObject.SetActive(true);
		if (dynamicSounds)
		{
			SoundManager.Play(SoundManager.DefaultSounds["Close"], null, null, null, overlapHand: false, leftOverlap: false, global: true);
		}
		try
		{
			if ((isOnPC || keyboardWithToggleButton || isKeyboardPc) && (Object)(object)TPC != (Object)null && ((Object)((Component)((Component)TPC).transform.parent).gameObject).name.Contains("CameraTablet"))
			{
				isOnPC = false;
				((Component)TPC).transform.position = ((Component)TPC).transform.parent.position;
				((Component)TPC).transform.rotation = ((Component)TPC).transform.parent.rotation;
			}
		}
		catch
		{
		}
		VideoPlayer obj3 = promptVideoPlayer;
		if (obj3 != null)
		{
			obj3.Stop();
		}
		smoothTargetPosition = Vector3.zero;
		smoothTargetRotation = Quaternion.identity;
		recenterPosition = null;
		networkMenuEnabled = false;
		NetworkMenuManager.DisableNetworkMenu();
		if (!dynamicAnimations || explodeMenu)
		{
			if (!dropOnRemove)
			{
				Object.Destroy((Object)(object)menu);
				menu = null;
				Object.Destroy((Object)(object)reference);
				reference = null;
				return;
			}
			if (explodeMenu)
			{
				try
				{
					if (dynamicSounds)
					{
						AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Menu/explosion.ogg", "Audio/Menu/explosion.ogg", delegate(AudioClip clip)
						{
							Play2DAudio(clip, (float)buttonClickVolume / 10f);
						});
					}
					foreach (GameObject item in from transform in menu.transform.Children()
						select transform.gameObject)
					{
						item.transform.SetParent((Transform)null, true);
						Rigidbody orAddComponent = GTExt.GetOrAddComponent<Rigidbody>(item);
						if (zeroGravityMenu)
						{
							orAddComponent.useGravity = false;
						}
						if (menuCollisions)
						{
							GameObject val = new GameObject("Collision");
							val.transform.SetParent(item.transform, false);
							val.layer = 3;
							val.AddComponent<BoxCollider>();
						}
						orAddComponent.linearVelocity = RandomUtilities.RandomVector3(5f);
						orAddComponent.angularVelocity = RandomUtilities.RandomVector3(50f);
						Object.Destroy((Object)(object)item, 5f);
					}
				}
				catch
				{
				}
			}
			else
			{
				try
				{
					Rigidbody orAddComponent2 = GTExt.GetOrAddComponent<Rigidbody>(menu);
					if (zeroGravityMenu)
					{
						orAddComponent2.useGravity = false;
					}
					if (menuCollisions)
					{
						GameObject val2 = new GameObject("Collision");
						val2.transform.SetParent(menuBackground.transform, false);
						val2.layer = 3;
						val2.AddComponent<BoxCollider>();
					}
					if (rightHand || (bothHands && openedwithright))
					{
						orAddComponent2.linearVelocity = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0f, false);
						orAddComponent2.angularVelocity = GTExt.GetOrAddComponent<GorillaVelocityEstimator>(GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHand Controller")).angularVelocity;
					}
					else
					{
						orAddComponent2.linearVelocity = GTPlayer.Instance.LeftHand.velocityTracker.GetAverageVelocity(true, 0f, false);
						orAddComponent2.angularVelocity = GTExt.GetOrAddComponent<GorillaVelocityEstimator>(GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHand Controller")).angularVelocity;
					}
					if (annoyingMode)
					{
						orAddComponent2.linearVelocity = RandomUtilities.RandomVector3(5f);
						orAddComponent2.angularVelocity = RandomUtilities.RandomVector3(50f);
					}
				}
				catch
				{
				}
				if (menuTrail)
				{
					try
					{
						TrailRenderer val3 = menu.AddComponent<TrailRenderer>();
						val3.startColor = backgroundColor.GetColor(0);
						val3.endColor = backgroundColor.GetColor(1);
						val3.startWidth = 0.025f;
						val3.endWidth = 0f;
						val3.minVertexDistance = 0.05f;
						if (smoothLines)
						{
							val3.numCapVertices = 10;
							val3.numCornerVertices = 5;
						}
						((Renderer)val3).material.shader = Shader.Find("Sprites/Default");
						val3.time = 2f;
					}
					catch
					{
					}
				}
			}
			Object.Destroy((Object)(object)menu, 5f);
			menu = null;
			Object.Destroy((Object)(object)reference);
			reference = null;
		}
		else
		{
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(ShrinkCoroutine());
			Object.Destroy((Object)(object)reference);
			reference = null;
		}
	}

	private static void AddPageButtons()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		ExtGradient color = buttonColors[swapButtonColors ? 1 : 0];
		switch (pageButtonType)
		{
		case 1:
			CreatePageButtonPair("PreviousPage", "NextPage", new Vector3(0.09f, 0.2f, 0.9f), new Vector3(0.56f, thinMenu ? 0.65f : 0.9f, 0f), new Vector3(0.56f, thinMenu ? (-0.65f) : (-0.9f), 0f), new Vector3(0.064f, thinMenu ? 0.195f : 0.267f, 0f), new Vector3(0.064f, thinMenu ? (-0.195f) : (-0.267f), 0f), color);
			break;
		case 2:
			CreatePageButtonPair("PreviousPage", "NextPage", new Vector3(0.09f, thinMenu ? 0.9f : 1.3f, ButtonDistance * 0.8f), new Vector3(0.56f, 0f, 0.28f - ButtonDistance * (float)(buttonOffset - 2)), new Vector3(0.56f, 0f, 0.28f - ButtonDistance * (float)(buttonOffset - 1)), new Vector3(0.064f, 0f, 0.109f - ButtonDistance * (float)(buttonOffset - 2) / 2.55f), new Vector3(0.064f, 0f, 0.109f - ButtonDistance * (float)(buttonOffset - 1) / 2.55f), color);
			break;
		case 5:
			CreatePageButtonPair("PreviousPage", "NextPage", new Vector3(0.09f, hidetitle ? 0.1f : 0.3f, 0.05f), new Vector3(0.56f, (thinMenu ? 0.299f : 0.499f) + (hidetitle ? 0.1f : 0f), 0.355f + (hidetitle ? 0.1f : 0f)), new Vector3(0.56f, (thinMenu ? (-0.299f) : (-0.499f)) - (hidetitle ? 0.1f : 0f), 0.355f + (hidetitle ? 0.1f : 0f)), new Vector3(0.064f, (thinMenu ? 0.09f : 0.15f) + (hidetitle ? 0.035f : 0f), 0.135f + (hidetitle ? 0.0375f : 0f)), new Vector3(0.064f, (thinMenu ? (-0.09f) : (-0.15f)) - (hidetitle ? 0.035f : 0f), 0.135f + (hidetitle ? 0.0375f : 0f)), color);
			break;
		case 6:
			CreatePageButtonPair("PreviousPage", "NextPage", new Vector3(0.09f, 0.102f, 0.08f), new Vector3(0.56f, thinMenu ? 0.45f : 0.7f, -0.58f), new Vector3(0.56f, thinMenu ? 0.45f : 0.7f, -0.58f) - new Vector3(0f, 0.16f, 0f), new Vector3(0.064f, thinMenu ? (7f / 52f) : 0.20940171f, -29f / 135f), new Vector3(0.064f, thinMenu ? (7f / 52f) : 0.20940171f, -29f / 135f) - new Vector3(0f, 0.0475f, 0f), color, (Vector2?)new Vector2(0.03f, 0.03f));
			break;
		case 3:
		case 4:
			break;
		}
	}

	private static void RenderPrompt()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Expected O, but got Unknown
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Expected O, but got Unknown
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0627: Unknown result type (might be due to invalid IL or missing references)
		//IL_0645: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		//IL_0661: Unknown result type (might be due to invalid IL or missing references)
		//IL_0683: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_078e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07af: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_087e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0883: Unknown result type (might be due to invalid IL or missing references)
		//IL_08af: Unknown result type (might be due to invalid IL or missing references)
		//IL_092b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0949: Unknown result type (might be due to invalid IL or missing references)
		//IL_0955: Unknown result type (might be due to invalid IL or missing references)
		//IL_0965: Unknown result type (might be due to invalid IL or missing references)
		//IL_0987: Unknown result type (might be due to invalid IL or missing references)
		//IL_084d: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09de: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject();
		val.transform.parent = canvasObj.transform;
		TextMeshPro val2 = val.AddComponent<TextMeshPro>();
		((TMP_Text)val2).font = activeFont;
		((TMP_Text)val2).text = CurrentPrompt.Message;
		string text = ExtractPromptImage(CurrentPrompt.Message);
		if (text != null)
		{
			((TMP_Text)val2).text = ((TMP_Text)val2).text.Replace("<" + text + ">", "");
		}
		((TMP_Text)val2).text = FollowMenuSettings(((TMP_Text)val2).text);
		((TMP_Text)val2).fontSize = 1f;
		((TMP_Text)val2).lineSpacing = 0.8f;
		ComponentUtils.AddComponent<UIColorChanger>((Component)(object)val2).colors = textColors[0];
		((TMP_Text)val2).richText = true;
		((TMP_Text)val2).fontStyle = activeFontStyle;
		((TMP_Text)val2).alignment = (TextAlignmentOptions)514;
		((TMP_Text)val2).enableAutoSizing = true;
		((TMP_Text)val2).fontSizeMin = 0f;
		RectTransform component = ((Component)val2).GetComponent<RectTransform>();
		component.sizeDelta = new Vector2(0.28f, CurrentPrompt.IsText ? 0.25f : 0.28f);
		((Transform)component).localPosition = new Vector3(0.06f, 0f, CurrentPrompt.IsText ? (-0.025f) : 0f);
		((Transform)component).rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
		if (text != null)
		{
			string fileName = text.Split("/")[^1];
			string fileExtension = FileUtilities.GetFileExtension(fileName);
			GameObject val3 = new GameObject();
			val3.transform.parent = canvasObj.transform;
			Image val4 = val3.AddComponent<Image>();
			component.sizeDelta = new Vector2(component.sizeDelta.x, 0.03f);
			((Transform)component).localPosition = new Vector3(0.06f, 0f, 0.1f);
			if ((Object)(object)promptMat == (Object)null)
			{
				promptMat = new Material(((Graphic)val4).material);
			}
			((Graphic)val4).material = promptMat;
			RectTransform component2 = ((Component)val4).GetComponent<RectTransform>();
			((Transform)component2).localPosition = Vector3.zero;
			component2.sizeDelta = new Vector2(0.2f, 0.2f);
			((Transform)component2).localPosition = new Vector3(0.06f, 0f, StringUtils.IsNullOrEmpty(((TMP_Text)val2).text) ? 0f : (-0.03f));
			((Transform)component2).rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
			FollowMenuSettings((MaskableGraphic)(object)val4);
			switch (fileExtension)
			{
			case "png":
			case "jpg":
				((Graphic)val4).material.SetTexture("_MainTex", (Texture)(object)AssetUtilities.LoadTextureFromURL(text, fileName));
				((Graphic)val4).material.color = Color.white;
				break;
			case "mp4":
			case "webm":
			case "mov":
			{
				promptVideoPlayer = new GameObject("Seralyth_PromptVideoPlayer").AddComponent<VideoPlayer>();
				promptVideoPlayer.playOnAwake = true;
				promptVideoPlayer.isLooping = true;
				promptVideoPlayer.url = text;
				RenderTexture val5 = new RenderTexture(192, 144, 0);
				val5.Create();
				promptVideoPlayer.targetTexture = val5;
				((Graphic)val4).material = promptMat;
				((Graphic)val4).material.color = Color.white;
				((Graphic)val4).material.SetTexture("_MainTex", (Texture)(object)val5);
				break;
			}
			case "mat":
				((Graphic)val4).material = promptMaterial;
				break;
			}
		}
		FollowMenuSettings((TMP_Text)(object)val2);
		joystickButtonSelected %= 2;
		GameObject val6 = GameObject.CreatePrimitive((PrimitiveType)3);
		if (!UnityInput.GetKey((Key)31) && (!inTextInput || !isKeyboardPc))
		{
			val6.layer = 2;
		}
		((Collider)val6.GetComponent<BoxCollider>()).isTrigger = true;
		val6.transform.parent = menu.transform;
		val6.transform.rotation = Quaternion.identity;
		val6.transform.localScale = new Vector3(0.09f, (CurrentPrompt.DeclineText == null) ? 0.9f : 0.4375f, 0.08f);
		val6.transform.localPosition = new Vector3(0.56f, (CurrentPrompt.DeclineText == null) ? 0f : 0.2375f, -0.43f);
		val6.AddComponent<ButtonCollider>().relatedText = "Accept Prompt";
		if (lastClickedName != "Accept Prompt")
		{
			ColorChanger colorChanger = val6.AddComponent<ColorChanger>();
			colorChanger.colors = buttonColors[0];
			if (joystickMenu && joystickButtonSelected == 0)
			{
				joystickSelectedButton = "Accept Prompt";
				ExtGradient extGradient = colorChanger.colors.Clone();
				extGradient.SetColor(0, Color.red);
				colorChanger.colors = extGradient;
			}
		}
		else
		{
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(ButtonClick(0, val6.GetComponent<Renderer>()));
		}
		GameObject val7 = new GameObject();
		val7.transform.parent = canvasObj.transform;
		TextMeshPro val8 = val7.AddComponent<TextMeshPro>();
		((TMP_Text)val8).font = activeFont;
		((TMP_Text)val8).fontStyle = activeFontStyle;
		((TMP_Text)val8).text = FollowMenuSettings(CurrentPrompt.AcceptText);
		((TMP_Text)val8).fontSize = 1f;
		((TMP_Text)val8).alignment = (TextAlignmentOptions)514;
		((TMP_Text)val8).enableAutoSizing = true;
		((TMP_Text)val8).fontSizeMin = 0f;
		ComponentUtils.AddComponent<UIColorChanger>((Component)(object)val8).colors = textColors[1];
		RectTransform component3 = ((Component)val8).GetComponent<RectTransform>();
		component3.sizeDelta = new Vector2(0.2f, 0.03f);
		if (arrowType == 11)
		{
			component3.sizeDelta = new Vector2(component3.sizeDelta.x, component3.sizeDelta.y * 6f);
		}
		if (NoAutoSizeText)
		{
			component3.sizeDelta = new Vector2(9f, 0.015f);
		}
		if (hideTextOnCamera)
		{
			((Component)component3).gameObject.layer = 19;
		}
		((Transform)component3).localPosition = new Vector3(0.064f, (CurrentPrompt.DeclineText != null) ? 0.075f : 0f, -0.16f);
		((Transform)component3).rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
		FollowMenuSettings((TMP_Text)(object)val8);
		FollowMenuSettings(val6);
		if (CurrentPrompt.DeclineText == null)
		{
			return;
		}
		GameObject val9 = GameObject.CreatePrimitive((PrimitiveType)3);
		if (!UnityInput.GetKey((Key)31) && (!inTextInput || !isKeyboardPc))
		{
			val9.layer = 2;
		}
		((Collider)val9.GetComponent<BoxCollider>()).isTrigger = true;
		val9.transform.parent = menu.transform;
		val9.transform.rotation = Quaternion.identity;
		val9.transform.localScale = new Vector3(0.09f, 0.4375f, 0.08f);
		val9.transform.localPosition = new Vector3(0.56f, -0.2375f, -0.43f);
		val9.AddComponent<ButtonCollider>().relatedText = "Decline Prompt";
		if (lastClickedName != "Decline Prompt")
		{
			ColorChanger colorChanger2 = val9.AddComponent<ColorChanger>();
			colorChanger2.colors = buttonColors[0];
			if (joystickMenu && joystickButtonSelected == 1)
			{
				joystickSelectedButton = "Decline Prompt";
				ExtGradient extGradient2 = colorChanger2.colors.Clone();
				extGradient2.SetColor(0, Color.red);
				colorChanger2.colors = extGradient2;
			}
		}
		else
		{
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(ButtonClick(1, val9.GetComponent<Renderer>()));
		}
		GameObject val10 = new GameObject();
		val10.transform.parent = canvasObj.transform;
		TextMeshPro val11 = val10.AddComponent<TextMeshPro>();
		((TMP_Text)val11).font = activeFont;
		((TMP_Text)val11).fontStyle = activeFontStyle;
		((TMP_Text)val11).text = FollowMenuSettings(CurrentPrompt.DeclineText);
		((TMP_Text)val11).fontSize = 1f;
		((TMP_Text)val11).alignment = (TextAlignmentOptions)514;
		((TMP_Text)val11).enableAutoSizing = true;
		((TMP_Text)val11).fontSizeMin = 0f;
		ComponentUtils.AddComponent<UIColorChanger>((Component)(object)val11).colors = textColors[1];
		RectTransform component4 = ((Component)val11).GetComponent<RectTransform>();
		component4.sizeDelta = new Vector2(0.2f, 0.03f);
		if (arrowType == 11)
		{
			component4.sizeDelta = new Vector2(component4.sizeDelta.x, component4.sizeDelta.y * 6f);
		}
		if (NoAutoSizeText)
		{
			component4.sizeDelta = new Vector2(9f, 0.015f);
		}
		if (hideTextOnCamera)
		{
			((Component)component4).gameObject.layer = 19;
		}
		((Transform)component4).localPosition = new Vector3(0.064f, -0.075f, -0.16f);
		((Transform)component4).rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
		FollowMenuSettings((TMP_Text)(object)val11);
		FollowMenuSettings(val9);
	}

	private static void CreatePageButtonPair(string prevButtonName, string nextButtonName, Vector3 buttonScale, Vector3 prevButtonPos, Vector3 nextButtonPos, Vector3 prevTextPos, Vector3 nextTextPos, ExtGradient color, Vector2? textSize = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		AdvancedAddButton(prevButtonName, buttonScale, prevButtonPos, prevTextPos, color, textSize, 0);
		AdvancedAddButton(nextButtonName, buttonScale, nextButtonPos, nextTextPos, color, textSize, 1);
	}

	public static string ExtractPromptImage(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return null;
		}
		Match match = Regex.Match(input, "<(?<url>(?:https?://|file:///)[^>]+)>");
		return match.Success ? match.Groups["url"].Value : null;
	}

	private static GameObject AdvancedAddButton(string buttonName, Vector3 scale, Vector3 position, Vector3 textPosition, ExtGradient color, Vector2? textSize, int arrowIndex)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
		if (!UnityInput.GetKey((Key)31) && (!inTextInput || !isKeyboardPc))
		{
			val.layer = 2;
		}
		((Collider)val.GetComponent<BoxCollider>()).isTrigger = true;
		val.transform.parent = menu.transform;
		val.transform.rotation = Quaternion.identity;
		val.transform.localScale = scale;
		val.transform.localPosition = position;
		val.AddComponent<ButtonCollider>().relatedText = buttonName;
		if (lastClickedName != buttonName)
		{
			ColorChanger colorChanger = val.AddComponent<ColorChanger>();
			colorChanger.colors = color;
		}
		else
		{
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(ButtonClick(-99, val.GetComponent<Renderer>()));
		}
		GameObject val2 = new GameObject();
		val2.transform.parent = canvasObj.transform;
		TextMeshPro val3 = val2.AddComponent<TextMeshPro>();
		((TMP_Text)val3).font = activeFont;
		((TMP_Text)val3).text = arrowTypes[arrowType][arrowIndex];
		((TMP_Text)val3).fontSize = 1f;
		((TMP_Text)val3).alignment = (TextAlignmentOptions)514;
		((TMP_Text)val3).enableAutoSizing = true;
		((TMP_Text)val3).fontSizeMin = 0f;
		((TMP_Text)val3).spriteAsset = ButtonSpriteSheet;
		ComponentUtils.AddComponent<UIColorChanger>((Component)(object)val3).colors = textColors[1];
		RectTransform component = ((Component)val3).GetComponent<RectTransform>();
		component.sizeDelta = (Vector2)(((_003F?)textSize) ?? new Vector2(0.2f, 0.03f));
		if (arrowType == 11)
		{
			component.sizeDelta = new Vector2(component.sizeDelta.x, component.sizeDelta.y * 6f);
		}
		if (NoAutoSizeText)
		{
			component.sizeDelta = new Vector2(9f, 0.015f);
		}
		if (hideTextOnCamera)
		{
			((Component)component).gameObject.layer = 19;
		}
		((Transform)component).localPosition = textPosition;
		((Transform)component).rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
		FollowMenuSettings((TMP_Text)(object)val3);
		FollowMenuSettings(val, !swapButtonColors);
		if (dynamicAnimations)
		{
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(RevealButton(val, val3, (float)PageSize * (slowDynamicAnimations ? 0.04f : 0.02f) + (float)arrowIndex * (slowDynamicAnimations ? 0.03f : 0.015f)));
		}
		return val;
	}

	public static void OutlineMenuObject(GameObject toOut, bool shouldBeEnabled)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		Transform parent = toOut.transform.parent;
		GameObject obj = menu;
		if ((Object)(object)parent != (Object)(object)((obj != null) ? obj.transform : null))
		{
			OutlineObject(toOut, shouldBeEnabled);
			return;
		}
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
		Object.Destroy((Object)(object)val.GetComponent<BoxCollider>());
		val.transform.parent = menu.transform;
		val.transform.rotation = Quaternion.identity;
		val.transform.localPosition = toOut.transform.localPosition;
		val.transform.localScale = toOut.transform.localScale + new Vector3(-0.01f, 0.01f, 0.0075f);
		ColorChanger colorChanger = val.AddComponent<ColorChanger>();
		colorChanger.colors = buttonColors[shouldBeEnabled ? 1 : 0];
		if (shouldRound)
		{
			RoundMenuObject(val, 0.024f);
		}
	}

	public static void OutlineObject(GameObject toOut, bool shouldBeEnabled)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
		Object.Destroy((Object)(object)val.GetComponent<BoxCollider>());
		val.transform.parent = toOut.transform.parent;
		val.transform.parent = toOut.transform.parent;
		val.transform.rotation = toOut.transform.rotation;
		val.transform.localPosition = toOut.transform.localPosition;
		val.transform.localScale = toOut.transform.localScale + new Vector3(0.005f, 0.005f, -0.001f);
		ColorChanger colorChanger = val.AddComponent<ColorChanger>();
		colorChanger.colors = buttonColors[shouldBeEnabled ? 1 : 0];
	}

	public static void OutlineCanvasObject(MaskableGraphic canvasObject)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Outline val = ((Component)canvasObject).gameObject.AddComponent<Outline>();
		((Shadow)val).effectColor = Color.black;
		((Shadow)val).effectDistance = new Vector2(0.001f, 0.001f);
		((Shadow)val).useGraphicAlpha = true;
	}

	public static void RoundMenuObject(GameObject toRound, float Bevel = 0.02f)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		Transform parent = toRound.transform.parent;
		GameObject obj = menu;
		if ((Object)(object)parent != (Object)(object)((obj != null) ? obj.transform : null))
		{
			RoundObject(toRound, Bevel);
			return;
		}
		Renderer component = toRound.GetComponent<Renderer>();
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
		val.GetComponent<Renderer>().enabled = component.enabled;
		Object.Destroy((Object)(object)val.GetComponent<Collider>());
		val.transform.parent = menu.transform;
		val.transform.rotation = Quaternion.identity;
		val.transform.localPosition = toRound.transform.localPosition;
		val.transform.localScale = toRound.transform.localScale + new Vector3(0f, Bevel * -2.55f, 0f);
		GameObject val2 = GameObject.CreatePrimitive((PrimitiveType)3);
		val2.GetComponent<Renderer>().enabled = component.enabled;
		Object.Destroy((Object)(object)val2.GetComponent<Collider>());
		val2.transform.parent = menu.transform;
		val2.transform.rotation = Quaternion.identity;
		val2.transform.localPosition = toRound.transform.localPosition;
		val2.transform.localScale = toRound.transform.localScale + new Vector3(0f, 0f, (0f - Bevel) * 2f);
		GameObject val3 = GameObject.CreatePrimitive((PrimitiveType)2);
		val3.GetComponent<Renderer>().enabled = component.enabled;
		Object.Destroy((Object)(object)val3.GetComponent<Collider>());
		val3.transform.parent = menu.transform;
		val3.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 0f, 90f);
		val3.transform.localPosition = toRound.transform.localPosition + new Vector3(0f, toRound.transform.localScale.y / 2f - Bevel * 1.275f, toRound.transform.localScale.z / 2f - Bevel);
		val3.transform.localScale = new Vector3(Bevel * 2.55f, toRound.transform.localScale.x / 2f, Bevel * 2f);
		GameObject val4 = GameObject.CreatePrimitive((PrimitiveType)2);
		val4.GetComponent<Renderer>().enabled = component.enabled;
		Object.Destroy((Object)(object)val4.GetComponent<Collider>());
		val4.transform.parent = menu.transform;
		val4.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 0f, 90f);
		val4.transform.localPosition = toRound.transform.localPosition + new Vector3(0f, 0f - toRound.transform.localScale.y / 2f + Bevel * 1.275f, toRound.transform.localScale.z / 2f - Bevel);
		val4.transform.localScale = new Vector3(Bevel * 2.55f, toRound.transform.localScale.x / 2f, Bevel * 2f);
		GameObject val5 = GameObject.CreatePrimitive((PrimitiveType)2);
		val5.GetComponent<Renderer>().enabled = component.enabled;
		Object.Destroy((Object)(object)val5.GetComponent<Collider>());
		val5.transform.parent = menu.transform;
		val5.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 0f, 90f);
		val5.transform.localPosition = toRound.transform.localPosition + new Vector3(0f, toRound.transform.localScale.y / 2f - Bevel * 1.275f, 0f - toRound.transform.localScale.z / 2f + Bevel);
		val5.transform.localScale = new Vector3(Bevel * 2.55f, toRound.transform.localScale.x / 2f, Bevel * 2f);
		GameObject val6 = GameObject.CreatePrimitive((PrimitiveType)2);
		val6.GetComponent<Renderer>().enabled = component.enabled;
		Object.Destroy((Object)(object)val6.GetComponent<Collider>());
		val6.transform.parent = menu.transform;
		val6.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 0f, 90f);
		val6.transform.localPosition = toRound.transform.localPosition + new Vector3(0f, 0f - toRound.transform.localScale.y / 2f + Bevel * 1.275f, 0f - toRound.transform.localScale.z / 2f + Bevel);
		val6.transform.localScale = new Vector3(Bevel * 2.55f, toRound.transform.localScale.x / 2f, Bevel * 2f);
		GameObject[] array = (GameObject[])(object)new GameObject[6] { val, val2, val3, val4, val5, val6 };
		GameObject[] array2 = array;
		foreach (GameObject val7 in array2)
		{
			ClampColor clampColor = val7.AddComponent<ClampColor>();
			clampColor.targetRenderer = component;
		}
		component.enabled = false;
		ColorChanger component2 = ((Component)component).GetComponent<ColorChanger>();
		if (Object.op_Implicit((Object)(object)component2))
		{
			component2.overrideTransparency = false;
		}
	}

	public static void RoundObject(GameObject toRound, float bevel = 0.02f)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		Renderer component = toRound.GetComponent<Renderer>();
		if (!((Object)(object)component == (Object)null))
		{
			Transform transform = toRound.transform;
			Vector3 localScale = transform.localScale;
			bool enabled = component.enabled;
			GameObject val = CreatePrimitive((PrimitiveType)3, transform, enabled);
			val.transform.localPosition = Vector3.zero;
			val.transform.localRotation = Quaternion.identity;
			val.transform.localScale = new Vector3(localScale.x, localScale.y - bevel * 2f, localScale.z);
			GameObject val2 = CreatePrimitive((PrimitiveType)3, transform, enabled);
			val2.transform.localPosition = Vector3.zero;
			val2.transform.localRotation = Quaternion.identity;
			val2.transform.localScale = new Vector3(localScale.x, localScale.y, localScale.z - bevel * 2f);
			GameObject[] array = (GameObject[])(object)new GameObject[4];
			Vector3[] array2 = (Vector3[])(object)new Vector3[4]
			{
				new Vector3(0f, localScale.y / 2f - bevel, localScale.z / 2f - bevel),
				new Vector3(0f, (0f - localScale.y) / 2f + bevel, localScale.z / 2f - bevel),
				new Vector3(0f, localScale.y / 2f - bevel, (0f - localScale.z) / 2f + bevel),
				new Vector3(0f, (0f - localScale.y) / 2f + bevel, (0f - localScale.z) / 2f + bevel)
			};
			for (int i = 0; i < 4; i++)
			{
				array[i] = CreatePrimitive((PrimitiveType)2, transform, enabled);
				array[i].transform.localPosition = array2[i];
				array[i].transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
				array[i].transform.localScale = new Vector3(bevel * 2f, localScale.x / 2f, bevel * 2f);
			}
			GameObject[] array3 = (GameObject[])(object)new GameObject[6]
			{
				val,
				val2,
				array[0],
				array[1],
				array[2],
				array[3]
			};
			GameObject[] array4 = array3;
			foreach (GameObject val3 in array4)
			{
				ClampColor clampColor = val3.AddComponent<ClampColor>();
				clampColor.targetRenderer = component;
			}
			component.enabled = false;
			ColorChanger component2 = ((Component)component).GetComponent<ColorChanger>();
			if ((Object)(object)component2 != (Object)null)
			{
				component2.overrideTransparency = false;
			}
		}
		static GameObject CreatePrimitive(PrimitiveType type, Transform parent, bool rendererEnabled)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			GameObject val4 = GameObject.CreatePrimitive(type);
			val4.GetComponent<Renderer>().enabled = rendererEnabled;
			Collider component3 = val4.GetComponent<Collider>();
			if ((Object)(object)component3 != (Object)null)
			{
				Object.Destroy((Object)(object)component3);
			}
			val4.transform.SetParent(parent, false);
			return val4;
		}
	}

	public static void Prompt(string Message, Action Accept = null, Action Decline = null, string AcceptButton = "Yes", string DeclineButton = "No")
	{
		prompts.Add(new PromptData
		{
			Message = Message,
			AcceptAction = Accept,
			DeclineAction = Decline,
			AcceptText = AcceptButton,
			DeclineText = DeclineButton,
			IsText = false
		});
		if ((Object)(object)menu != (Object)null && prompts.Count <= 1)
		{
			ReloadMenu();
		}
	}

	public static void PromptSingle(string Message, Action Accept = null, string AcceptButton = "Yes")
	{
		prompts.Add(new PromptData
		{
			Message = Message,
			AcceptAction = Accept,
			DeclineAction = null,
			AcceptText = AcceptButton,
			DeclineText = null,
			IsText = false
		});
		if ((Object)(object)menu != (Object)null && prompts.Count <= 1)
		{
			ReloadMenu();
		}
	}

	public static void PromptText(string Message, Action Accept = null, Action Decline = null, string AcceptButton = "Yes", string DeclineButton = "No")
	{
		prompts.Add(new PromptData
		{
			Message = Message,
			AcceptAction = Accept,
			DeclineAction = Decline,
			AcceptText = AcceptButton,
			DeclineText = DeclineButton,
			IsText = true
		});
		if ((Object)(object)menu != (Object)null && prompts.Count <= 1)
		{
			ReloadMenu();
		}
	}

	public static void PromptSingleText(string Message, Action Accept = null, string AcceptButton = "Yes")
	{
		prompts.Add(new PromptData
		{
			Message = Message,
			AcceptAction = Accept,
			DeclineAction = null,
			AcceptText = AcceptButton,
			DeclineText = null,
			IsText = true
		});
		if ((Object)(object)menu != (Object)null && prompts.Count <= 1)
		{
			ReloadMenu();
		}
	}

	public static void UpdatePrompt(string newVersion = null)
	{
		if (versionArchive == null)
		{
			versionArchive = newVersion;
		}
		AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Menu/Notifications/win7-exc.ogg", "Audio/Menu/Notifications/win7-exc.ogg", delegate(AudioClip clip)
		{
			Play2DAudio(clip, (float)buttonClickVolume / 10f);
		});
		Prompt("A new version is available (" + versionArchive + "). Would you like to update?", Settings.UpdateMenu);
	}

	public static Texture2D GetGradientTexture(Color colorA, Color colorB)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		(Color, Color) key = (colorA, colorB);
		if (cacheGradients.TryGetValue(key, out var value))
		{
			return value;
		}
		Texture2D val = new Texture2D(128, 128);
		Color[] array = (Color[])(object)new Color[16384];
		if (horizontalGradients)
		{
			for (int i = 0; i < 128; i++)
			{
				Color val2 = Color.Lerp(colorA, colorB, (float)i / 128f);
				for (int j = 0; j < 128; j++)
				{
					array[i * 128 + j] = val2;
				}
			}
		}
		else
		{
			for (int k = 0; k < 128; k++)
			{
				Color val3 = Color.Lerp(colorA, colorB, (float)k / 128f);
				for (int l = 0; l < 128; l++)
				{
					array[l * 128 + k] = val3;
				}
			}
		}
		val.SetPixels(array);
		((Texture)val).wrapMode = (TextureWrapMode)2;
		val.Apply();
		cacheGradients.Add(key, val);
		return val;
	}

	public static void RPCProtection()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		try
		{
			((MonkeAgent)MonkeAgent.instance).rpcErrorMax = int.MaxValue;
			((MonkeAgent)MonkeAgent.instance).rpcCallLimit = int.MaxValue;
			((MonkeAgent)MonkeAgent.instance).logErrorMax = int.MaxValue;
			((MonkeAgent)MonkeAgent.instance).userRPCCalls.Clear();
			PhotonNetwork.MaxResendsBeforeDisconnect = int.MaxValue;
			PhotonNetwork.QuickResends = int.MaxValue;
			PhotonNetwork.SendAllOutgoingCommands();
		}
		catch
		{
			LogManager.Log("RPC protection failed, are you in a lobby?");
		}
	}

	public static string GetHttp(string url)
	{
		WebRequest webRequest = WebRequest.Create(url);
		WebResponse response = webRequest.GetResponse();
		Stream responseStream = response.GetResponseStream();
		string result = "";
		if (responseStream == null)
		{
			return result;
		}
		using StreamReader streamReader = new StreamReader(responseStream);
		return streamReader.ReadToEnd();
	}

	public static (RaycastHit Ray, GameObject NewPointer) RenderGun(int? overrideLayerMask = null)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Expected O, but got Unknown
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_0696: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d68: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d85: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0daf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dbb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0def: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dfb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e05: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e33: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dcb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dcd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f82: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fc5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0710: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0998: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ead: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ebe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fb2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fbb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f34: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0720: Unknown result type (might be due to invalid IL or missing references)
		//IL_0721: Unknown result type (might be due to invalid IL or missing references)
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_0735: Unknown result type (might be due to invalid IL or missing references)
		//IL_073e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0812: Unknown result type (might be due to invalid IL or missing references)
		//IL_0813: Unknown result type (might be due to invalid IL or missing references)
		//IL_0822: Unknown result type (might be due to invalid IL or missing references)
		//IL_0827: Unknown result type (might be due to invalid IL or missing references)
		//IL_0830: Unknown result type (might be due to invalid IL or missing references)
		//IL_0832: Unknown result type (might be due to invalid IL or missing references)
		//IL_084c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0851: Unknown result type (might be due to invalid IL or missing references)
		//IL_087a: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0910: Unknown result type (might be due to invalid IL or missing references)
		//IL_0928: Unknown result type (might be due to invalid IL or missing references)
		//IL_0954: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a71: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a86: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b74: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b93: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc1: Unknown result type (might be due to invalid IL or missing references)
		//IL_078a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0756: Unknown result type (might be due to invalid IL or missing references)
		//IL_078f: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cbd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ccb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cda: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cdf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d24: Unknown result type (might be due to invalid IL or missing references)
		GunSpawned = true;
		Transform val = (SwapGunHand ? GorillaTagger.Instance.leftHandTransform : GorillaTagger.Instance.rightHandTransform);
		Vector3 val2 = val.position;
		Vector3 val3 = val.forward;
		Vector3 val4 = -val.up;
		Vector3 val5 = val.right;
		switch (GunDirection)
		{
		case 1:
			val4 = val.forward;
			val3 = -val.up;
			break;
		case 2:
			val4 = val.forward;
			val5 = -val.up;
			val3 = val.right * (SwapGunHand ? 1f : (-1f));
			break;
		case 3:
		{
			Vector3 item;
			Vector3 item2;
			Vector3 item3;
			if (!SwapGunHand)
			{
				(Vector3, Quaternion, Vector3, Vector3, Vector3) trueRightHand = ControllerUtilities.GetTrueRightHand();
				item = trueRightHand.Item3;
				item2 = trueRightHand.Item4;
				item3 = trueRightHand.Item5;
			}
			else
			{
				(Vector3, Quaternion, Vector3, Vector3, Vector3) trueRightHand = ControllerUtilities.GetTrueLeftHand();
				item = trueRightHand.Item3;
				item2 = trueRightHand.Item4;
				item3 = trueRightHand.Item5;
			}
			val4 = item;
			val5 = item3;
			val3 = item2;
			break;
		}
		case 4:
			val4 = ((Component)GorillaTagger.Instance.headCollider).transform.up;
			val5 = ((Component)GorillaTagger.Instance.headCollider).transform.right;
			val3 = ((Component)GorillaTagger.Instance.headCollider).transform.forward;
			val2 = ((Component)GorillaTagger.Instance.headCollider).transform.position + val4 * 0.1f;
			break;
		}
		if ((Object)(object)GiveGunTarget != (Object)null)
		{
			val = (SwapGunHand ? GiveGunTarget.leftHandTransform : GiveGunTarget.rightHandTransform);
			val2 = val.position;
			val3 = val.up;
			val4 = val.forward;
			val5 = val.right;
		}
		RaycastHit item4 = default(RaycastHit);
		Physics.Raycast(val2 + val3 / 4f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f), val3, ref item4, 512f, overrideLayerMask ?? NoInvisLayerMask());
		if (shouldBePC)
		{
			Ray val6 = TPC.ScreenPointToRay(Vector2.op_Implicit(((InputControl<Vector2>)(object)((Pointer)Mouse.current).position).ReadValue()));
			Physics.Raycast(val6, ref item4, 512f, NoInvisLayerMask());
			val3 = ((Ray)(ref val6)).direction;
		}
		Vector3 val7 = (gunLocked ? ((Component)lockTarget).transform.position : ((RaycastHit)(ref item4)).point);
		if (val7 == Vector3.zero)
		{
			val7 = val2 + val3 * 512f;
		}
		if (SmoothGunPointer)
		{
			GunPositionSmoothed = Vector3.Lerp(GunPositionSmoothed, val7, Time.deltaTime * 6f);
			val7 = GunPositionSmoothed;
		}
		GunStartPos = val2 + val3 / 4f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
		GunEndPos = val7;
		GunActiveThisFrame = true;
		if ((Object)(object)GunPointer == (Object)null)
		{
			GunPointer = GameObject.CreatePrimitive((PrimitiveType)0);
		}
		GunPointer.SetActive(true);
		GunPointer.transform.localScale = (smallGunPointer ? new Vector3(0.1f, 0.1f, 0.1f) : new Vector3(0.2f, 0.2f, 0.2f)) * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
		GunPointer.transform.position = val7;
		Renderer component = GunPointer.GetComponent<Renderer>();
		if (((Object)component.material.shader).name != "GUI/Text Shader")
		{
			component.material.shader = Shader.Find("GUI/Text Shader");
		}
		component.material.color = ((gunLocked || GetGunInput(isShooting: true)) ? buttonColors[1].GetCurrentColor() : buttonColors[0].GetCurrentColor());
		if (disableGunPointer)
		{
			component.enabled = false;
		}
		if (GunParticles && (GetGunInput(isShooting: true) || gunLocked))
		{
			GameObject val8 = GameObject.CreatePrimitive((PrimitiveType)0);
			val8.transform.position = val7;
			val8.transform.localScale = Vector3.one * (0.025f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f));
			val8.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
			val8.AddComponent<CustomParticle>();
			Object.Destroy((Object)(object)val8.GetComponent<Collider>());
		}
		Collider component2 = GunPointer.GetComponent<Collider>();
		if ((Object)(object)component2 != (Object)null)
		{
			Object.Destroy((Object)(object)component2);
		}
		if (disableGunLine)
		{
			return (Ray: item4, NewPointer: GunPointer);
		}
		if ((Object)(object)GunLine == (Object)null)
		{
			GameObject val9 = new GameObject("Seralyth_GunLine");
			GunLine = val9.AddComponent<LineRenderer>();
		}
		((Component)GunLine).gameObject.SetActive(true);
		if (((Object)((Renderer)GunLine).material.shader).name != "GUI/Text Shader")
		{
			((Renderer)GunLine).material.shader = Shader.Find("GUI/Text Shader");
		}
		GunLine.startColor = backgroundColor.GetCurrentColor();
		GunLine.endColor = backgroundColor.GetCurrentColor(0.5f);
		GunLine.startWidth = 0.025f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
		GunLine.endWidth = 0.025f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
		GunLine.useWorldSpace = true;
		if (smoothLines)
		{
			GunLine.numCapVertices = 10;
			GunLine.numCornerVertices = 5;
		}
		if (gunVariation != 9)
		{
			GunLine.positionCount = 2;
			GunLine.SetPosition(0, val2);
			GunLine.SetPosition(1, val7);
		}
		int gunLineQuality = GunLineQuality;
		switch (gunVariation)
		{
		case 1:
			if (GetGunInput(isShooting: true) || gunLocked)
			{
				GunLine.positionCount = gunLineQuality;
				GunLine.SetPosition(0, val2);
				for (int num9 = 1; num9 < gunLineQuality - 1; num9++)
				{
					Vector3 val20 = Vector3.Lerp(val2, val7, (float)num9 / ((float)gunLineQuality - 1f));
					GunLine.SetPosition(num9, val20 + (Vector3)((Random.Range(0f, 1f) > 0.75f) ? new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f)) : Vector3.zero));
				}
				GunLine.SetPosition(gunLineQuality - 1, val7);
			}
			break;
		case 2:
			if (GetGunInput(isShooting: true) || gunLocked)
			{
				GunLine.positionCount = gunLineQuality;
				GunLine.SetPosition(0, val2);
				for (int j = 1; j < gunLineQuality - 1; j++)
				{
					float num = (float)j / (float)gunLineQuality * 50f;
					Vector3 val11 = Vector3.Lerp(val2, val7, (float)j / ((float)gunLineQuality - 1f));
					GunLine.SetPosition(j, val11 + val4 * (Mathf.Sin(Time.time * -10f + num) * 0.1f));
				}
				GunLine.SetPosition(gunLineQuality - 1, val7);
			}
			break;
		case 3:
			if (GetGunInput(isShooting: true) || gunLocked)
			{
				GunLine.positionCount = gunLineQuality;
				GunLine.SetPosition(0, val2);
				for (int num6 = 1; num6 < gunLineQuality - 1; num6++)
				{
					Vector3 val18 = Vector3.Lerp(val2, val7, (float)num6 / ((float)gunLineQuality - 1f));
					GunLine.SetPosition(num6, new Vector3(Mathf.Round(val18.x * 25f) / 25f, Mathf.Round(val18.y * 25f) / 25f, Mathf.Round(val18.z * 25f) / 25f));
				}
				GunLine.SetPosition(gunLineQuality - 1, val7);
			}
			break;
		case 4:
			gunLineQuality = GunLineQuality / 2;
			if (GetGunInput(isShooting: true) || gunLocked)
			{
				GunLine.positionCount = gunLineQuality;
				GunLine.SetPosition(0, val2);
				for (int l = 1; l < gunLineQuality - 1; l++)
				{
					Vector3 val13 = Vector3.Lerp(val2, val7, (float)l / ((float)gunLineQuality - 1f));
					GunLine.SetPosition(l, val13 + val4 * (Mathf.Sin(Time.time * 10f) * ((l % 2 == 0) ? 0.1f : (-0.1f))));
				}
				GunLine.SetPosition(gunLineQuality - 1, val7);
			}
			break;
		case 5:
			if (GetGunInput(isShooting: true) || gunLocked)
			{
				GunLine.positionCount = gunLineQuality;
				GunLine.SetPosition(0, val2);
				for (int k = 1; k < gunLineQuality - 1; k++)
				{
					float num2 = (float)k / (float)gunLineQuality * 50f;
					Vector3 val12 = Vector3.Lerp(val2, val7, (float)k / ((float)gunLineQuality - 1f));
					GunLine.SetPosition(k, val12 + val5 * (Mathf.Cos(Time.time * -10f + num2) * 0.1f) + val4 * (Mathf.Sin(Time.time * -10f + num2) * 0.1f));
				}
				GunLine.SetPosition(gunLineQuality - 1, val7);
			}
			break;
		case 6:
			if (GetGunInput(isShooting: true) || gunLocked)
			{
				GunLine.positionCount = gunLineQuality;
				GunLine.SetPosition(0, val2);
				for (int n = 1; n < gunLineQuality - 1; n++)
				{
					float num5 = (float)n / (float)gunLineQuality * 15f;
					GunLine.SetPosition(n, Vector3.Lerp(val2, val7, (float)n / ((float)gunLineQuality - 1f)) + val4 * (Mathf.Abs(Mathf.Sin(Time.time * -10f + num5)) * 0.3f));
				}
				GunLine.SetPosition(gunLineQuality - 1, val7);
			}
			break;
		case 7:
		{
			if (!GetGunInput(isShooting: true) && !gunLocked)
			{
				break;
			}
			float num7 = 0f;
			if (gunLocked)
			{
				GorillaSpeakerLoudness component3 = ((Component)lockTarget).GetComponent<GorillaSpeakerLoudness>();
				if ((Object)(object)component3 != (Object)null)
				{
					num7 += component3.Loudness * 3f;
				}
			}
			GorillaSpeakerLoudness component4 = ((Component)VRRig.LocalRig).GetComponent<GorillaSpeakerLoudness>();
			if ((Object)(object)component4 != (Object)null)
			{
				num7 += component4.Loudness * 3f;
			}
			volumeArchive.Insert(0, (volumeArchive.Count == 0) ? 0f : (num7 - volumeArchive[0] * 0.1f));
			if (volumeArchive.Count > gunLineQuality)
			{
				volumeArchive.Remove(gunLineQuality);
			}
			GunLine.positionCount = gunLineQuality;
			GunLine.SetPosition(0, val2);
			for (int num8 = 1; num8 < gunLineQuality - 1; num8++)
			{
				Vector3 val19 = Vector3.Lerp(val2, val7, (float)num8 / ((float)gunLineQuality - 1f));
				GunLine.SetPosition(num8, val19 + val4 * (((num8 >= volumeArchive.Count) ? 0f : volumeArchive[num8]) * ((num8 % 2 == 0) ? 1f : (-1f))));
			}
			GunLine.SetPosition(gunLineQuality - 1, val7);
			break;
		}
		case 8:
		{
			Vector3 val14 = Vector3.Lerp(val2, val7, 0.5f);
			float num3 = Time.time * 3f;
			Vector3 val15 = val4 * (Mathf.Sin(num3) * 0.15f) + val5 * (Mathf.Cos(num3 * 1.3f) * 0.15f);
			Vector3 val16 = val14 + val15;
			if (MidPosition == Vector3.zero)
			{
				MidPosition = val16;
			}
			Vector3 val17 = (val16 - MidPosition) * 40f;
			MidVelocity += val17 * Time.deltaTime;
			MidVelocity *= Mathf.Exp(-6f * Time.deltaTime);
			MidPosition += MidVelocity * Time.deltaTime;
			GunLine.positionCount = gunLineQuality;
			GunLine.SetPosition(0, val2);
			Vector3[] array = (Vector3[])(object)new Vector3[gunLineQuality];
			for (int m = 0; m < gunLineQuality; m++)
			{
				float num4 = (float)m / (float)(gunLineQuality - 1);
				array[m] = Mathf.Pow(1f - num4, 2f) * val2 + 2f * (1f - num4) * num4 * MidPosition + Mathf.Pow(num4, 2f) * val7;
			}
			GunLine.positionCount = gunLineQuality;
			GunLine.SetPositions(array);
			break;
		}
		case 9:
		{
			GunLine.positionCount = gunLineQuality;
			RopePhysics ropePhysics = ((Component)GunLine).gameObject.GetComponent<RopePhysics>();
			if ((Object)(object)ropePhysics == (Object)null)
			{
				for (int i = 0; i < gunLineQuality; i++)
				{
					Vector3 val10 = Vector3.Lerp(val2, val7, (float)i / ((float)gunLineQuality - 1f));
					GunLine.SetPosition(i, val10);
				}
				ropePhysics = ((Component)GunLine).gameObject.AddComponent<RopePhysics>();
			}
			ropePhysics.segmentLength = Vector3.Distance(val2, val7) / (float)(gunLineQuality - 1) * ((GetGunInput(isShooting: true) || gunLocked) ? 1.1f : 1.2f);
			ropePhysics.SetStartPosition(val2);
			ropePhysics.SetEndPosition(val7);
			break;
		}
		}
		return (Ray: item4, NewPointer: GunPointer);
	}

	public static void GunPreview()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Expected O, but got Unknown
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		if (!gunPreviewEnabled)
		{
			return;
		}
		Transform val = (SwapGunHand ? GorillaTagger.Instance.leftHandTransform : GorillaTagger.Instance.rightHandTransform);
		Vector3 position = val.position;
		Vector3 val2 = val.forward;
		switch (GunDirection)
		{
		case 1:
			val2 = -val.up;
			break;
		case 2:
			val2 = GorillaTagger.Instance.mainCamera.transform.forward;
			break;
		case 3:
			val2 = GorillaTagger.Instance.rightHandTransform.position - GorillaTagger.Instance.leftHandTransform.position;
			break;
		case 4:
			val2 = GorillaTagger.Instance.mainCamera.transform.forward;
			break;
		}
		Vector3 val3 = position + val2 * 5f;
		float num = (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
		if ((Object)(object)GunPreviewPointer == (Object)null)
		{
			GunPreviewPointer = GameObject.CreatePrimitive((PrimitiveType)0);
			Object.Destroy((Object)(object)GunPreviewPointer.GetComponent<Collider>());
			GunPreviewPointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
		}
		GunPreviewPointer.SetActive(true);
		GunPreviewPointer.transform.position = val3;
		GunPreviewPointer.transform.localScale = (smallGunPointer ? new Vector3(0.1f, 0.1f, 0.1f) : new Vector3(0.2f, 0.2f, 0.2f)) * num;
		GunPreviewPointer.GetComponent<Renderer>().material.color = buttonColors[0].GetCurrentColor();
		if (disableGunPointer)
		{
			GunPreviewPointer.GetComponent<Renderer>().enabled = false;
		}
		else
		{
			GunPreviewPointer.GetComponent<Renderer>().enabled = true;
		}
		if (disableGunLine)
		{
			return;
		}
		if ((Object)(object)GunPreviewLine == (Object)null)
		{
			GameObject val4 = new GameObject("Seralyth_GunPreviewLine");
			GunPreviewLine = val4.AddComponent<LineRenderer>();
			((Renderer)GunPreviewLine).material.shader = Shader.Find("GUI/Text Shader");
			GunPreviewLine.useWorldSpace = true;
		}
		((Component)GunPreviewLine).gameObject.SetActive(true);
		GunPreviewLine.startColor = backgroundColor.GetCurrentColor();
		GunPreviewLine.endColor = backgroundColor.GetCurrentColor(0.5f);
		GunPreviewLine.startWidth = 0.025f * num;
		GunPreviewLine.endWidth = 0.025f * num;
		switch (gunVariation)
		{
		case 0:
			GunPreviewLine.positionCount = 2;
			GunPreviewLine.SetPosition(0, position);
			GunPreviewLine.SetPosition(1, val3);
			break;
		case 1:
		{
			GunPreviewLine.positionCount = GunLineQuality;
			for (int j = 0; j < GunLineQuality; j++)
			{
				float num3 = (float)j / (float)(GunLineQuality - 1);
				Vector3 val6 = Vector3.Lerp(position, val3, num3);
				val6 += Vector2.op_Implicit(Random.insideUnitCircle) * Mathf.Sin(num3 * 20f) * 0.15f * num;
				GunPreviewLine.SetPosition(j, val6);
			}
			break;
		}
		case 2:
		{
			GunPreviewLine.positionCount = GunLineQuality;
			for (int k = 0; k < GunLineQuality; k++)
			{
				float num4 = (float)k / (float)(GunLineQuality - 1);
				Vector3 val7 = Vector3.Lerp(position, val3, num4);
				val7 += val.right * Mathf.Sin(num4 * 30f) * 0.2f * num;
				val7 += val.up * Mathf.Cos(num4 * 20f) * 0.1f * num;
				GunPreviewLine.SetPosition(k, val7);
			}
			break;
		}
		default:
		{
			GunPreviewLine.positionCount = GunLineQuality;
			for (int i = 0; i < GunLineQuality; i++)
			{
				float num2 = (float)i / (float)(GunLineQuality - 1);
				Vector3 val5 = Vector3.Lerp(position, val3, num2);
				val5 += Vector2.op_Implicit(Random.insideUnitCircle) * Mathf.Sin(num2 * 15f) * 0.1f * num;
				GunPreviewLine.SetPosition(i, val5);
			}
			break;
		}
		}
	}

	public static void DisableGunPreview()
	{
		gunPreviewEnabled = false;
		if ((Object)(object)GunPreviewPointer != (Object)null)
		{
			Object.Destroy((Object)(object)GunPreviewPointer);
			GunPreviewPointer = null;
		}
		if ((Object)(object)GunPreviewLine != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)GunPreviewLine).gameObject);
			GunPreviewLine = null;
		}
	}

	public static bool GetGunInput(bool isShooting)
	{
		return (!((Object)(object)GiveGunTarget != (Object)null)) ? ((!isShooting) ? (GriplessGuns || (SwapGunHand ? leftGrab : rightGrab) || (HardGunLocks && gunLocked && !rightSecondary) || Mouse.current.rightButton.isPressed) : (TriggerlessGuns || (SwapGunHand ? (leftTrigger > 0.5f) : (rightTrigger > 0.5f)) || Mouse.current.leftButton.isPressed)) : ((!isShooting) ? (GriplessGuns || (SwapGunHand ? (((VRMap)GiveGunTarget.leftMiddle).calcT > 0.5f) : (((VRMap)GiveGunTarget.rightMiddle).calcT > 0.5f))) : (TriggerlessGuns || (SwapGunHand ? (((VRMap)GiveGunTarget.leftIndex).calcT > 0.5f) : (((VRMap)GiveGunTarget.rightIndex).calcT > 0.5f))));
	}

	public static Vector3 GetGunDirection(Transform transform)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		int gunDirection = GunDirection;
		if (1 == 0)
		{
		}
		Vector3 result = (Vector3)(gunDirection switch
		{
			1 => transform.forward, 
			2 => -transform.up, 
			3 => ((Object)(object)transform == (Object)(object)GorillaTagger.Instance.rightHandTransform) ? ControllerUtilities.GetTrueRightHand().forward : ControllerUtilities.GetTrueLeftHand().forward, 
			4 => ((Component)GorillaTagger.Instance.headCollider).transform.forward, 
			_ => transform.forward, 
		});
		if (1 == 0)
		{
		}
		return result;
	}

	public static IEnumerator TranscribeText(string text, Action<AudioClip> onComplete, string customFileName = null, string customPath = null)
	{
		if (Time.time < timeMenuStarted + 5f)
		{
			onComplete?.Invoke(null);
			yield break;
		}
		string fileName = ((!string.IsNullOrEmpty(customPath)) ? FileUtilities.SanitizeFileName(customFileName) : (GetSHA256(text) + ((narratorIndex == 0) ? ".wav" : ".mp3")));
		string directoryPath = ((!string.IsNullOrEmpty(customPath)) ? customPath : ("SeralythMenu/TTS" + ((narratorName == "Default") ? "" : narratorName)));
		string filePath = directoryPath + "/" + fileName;
		if (!Directory.Exists(directoryPath))
		{
			Directory.CreateDirectory(directoryPath);
		}
		if (!File.Exists(filePath))
		{
			switch (narratorIndex)
			{
			case 0:
			{
				string postData = JsonConvert.SerializeObject((object)new { text });
				UnityWebRequest request3 = new UnityWebRequest("https://menu.seralyth.software/tts", "POST");
				try
				{
					byte[] raw = Encoding.UTF8.GetBytes(postData);
					request3.uploadHandler = (UploadHandler)new UploadHandlerRaw(raw);
					request3.SetRequestHeader("Content-Type", "application/json");
					request3.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
					yield return request3.SendWebRequest();
					if ((int)request3.result != 1)
					{
						LogManager.LogError("Error downloading TTS: " + request3.error);
						onComplete?.Invoke(null);
						yield break;
					}
					byte[] response = request3.downloadHandler.data;
					File.WriteAllBytes(filePath, response);
				}
				finally
				{
					((IDisposable)request3)?.Dispose();
				}
				break;
			}
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			{
				if (text.Length > 550)
				{
					text = text.Substring(0, 550);
				}
				UnityWebRequest request5 = new UnityWebRequest("https://lazypy.ro/tts/request_tts.php?service=Streamlabs&voice=" + narratorName + "&text=" + UnityWebRequest.EscapeURL(text), "POST");
				try
				{
					request5.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
					yield return request5.SendWebRequest();
					if ((int)request5.result != 1)
					{
						LogManager.LogError("Error getting TTS: " + request5.error);
						onComplete?.Invoke(null);
						yield break;
					}
					string jsonResponse4 = request5.downloadHandler.text;
					Dictionary<string, object> responseData4 = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse4);
					UnityWebRequest dataRequest4 = UnityWebRequest.Get(responseData4["audio_url"].ToString().Replace("\\", ""));
					try
					{
						yield return dataRequest4.SendWebRequest();
						if ((int)dataRequest4.result != 1)
						{
							LogManager.LogError("Error downloading TTS: " + responseData4["audio_url"]);
						}
						else
						{
							File.WriteAllBytes(filePath, dataRequest4.downloadHandler.data);
						}
					}
					finally
					{
						((IDisposable)dataRequest4)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)request5)?.Dispose();
				}
				break;
			}
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
			case 14:
			case 15:
			case 16:
			case 17:
			case 18:
			case 19:
			case 20:
			case 21:
			case 22:
			{
				Dictionary<int, string> voiceCodenames2 = new Dictionary<int, string>
				{
					{ 9, "en_us_001" },
					{ 10, "en_female_grandma" },
					{ 11, "en_male_grinch" },
					{ 12, "en_male_ukneighbor" },
					{ 13, "en_us_ghostface" },
					{ 14, "en_female_zombie" },
					{ 15, "en_male_narration" },
					{ 16, "en_male_pirate" },
					{ 17, "en_male_m03_sunshine_soon" },
					{ 18, "en_us_006" },
					{ 19, "en_male_david_gingerman" },
					{ 20, "en_male_chris" },
					{ 21, "en_male_sing_funny_thanksgiving" },
					{ 22, "en_male_santa_effect" }
				};
				if (text.Length > 300)
				{
					text = text.Substring(0, 300);
				}
				UnityWebRequest request2 = new UnityWebRequest("https://lazypy.ro/tts/request_tts.php?service=TikTok&voice=" + voiceCodenames2[narratorIndex] + "&text=" + UnityWebRequest.EscapeURL(text), "POST");
				try
				{
					request2.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
					yield return request2.SendWebRequest();
					if ((int)request2.result != 1)
					{
						LogManager.LogError("Error getting TTS: " + request2.error);
						onComplete?.Invoke(null);
						yield break;
					}
					string jsonResponse2 = request2.downloadHandler.text;
					Dictionary<string, object> responseData2 = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse2);
					UnityWebRequest dataRequest2 = UnityWebRequest.Get(responseData2["audio_url"].ToString().Replace("\\", ""));
					try
					{
						yield return dataRequest2.SendWebRequest();
						if ((int)dataRequest2.result != 1)
						{
							LogManager.LogError("Error downloading TTS: " + responseData2["audio_url"]);
						}
						else
						{
							File.WriteAllBytes(filePath, dataRequest2.downloadHandler.data);
						}
					}
					finally
					{
						((IDisposable)dataRequest2)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)request2)?.Dispose();
				}
				break;
			}
			case 23:
			case 24:
			{
				Dictionary<int, string> voiceCodenames3 = new Dictionary<int, string>
				{
					{ 23, "en-us" },
					{ 24, "en-gb" }
				};
				if (text.Length > 300)
				{
					text = text.Substring(0, 300);
				}
				UnityWebRequest request4 = new UnityWebRequest("https://lazypy.ro/tts/request_tts.php?service=Google%20Translate&voice=" + voiceCodenames3[narratorIndex] + "&text=" + UnityWebRequest.EscapeURL(text), "POST");
				try
				{
					request4.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
					yield return request4.SendWebRequest();
					if ((int)request4.result != 1)
					{
						LogManager.LogError("Error getting TTS: " + request4.error);
						onComplete?.Invoke(null);
						yield break;
					}
					string jsonResponse3 = request4.downloadHandler.text;
					Dictionary<string, object> responseData3 = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse3);
					UnityWebRequest dataRequest3 = UnityWebRequest.Get(responseData3["audio_url"].ToString().Replace("\\", ""));
					try
					{
						yield return dataRequest3.SendWebRequest();
						if ((int)dataRequest3.result != 1)
						{
							LogManager.LogError("Error downloading TTS: " + responseData3["audio_url"]);
						}
						else
						{
							File.WriteAllBytes(filePath, dataRequest3.downloadHandler.data);
						}
					}
					finally
					{
						((IDisposable)dataRequest3)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)request4)?.Dispose();
				}
				break;
			}
			case 25:
			case 26:
			case 27:
			case 28:
			case 29:
			{
				Dictionary<int, string> voiceCodenames = new Dictionary<int, string>
				{
					{ 25, "Dog" },
					{ 26, "Jerkface" },
					{ 27, "Robot" },
					{ 28, "Vlad" },
					{ 29, "Obama" }
				};
				if (text.Length > 300)
				{
					text = text.Substring(0, 300);
				}
				UnityWebRequest request = new UnityWebRequest("https://lazypy.ro/tts/request_tts.php?service=VoiceForge&voice=" + voiceCodenames[narratorIndex] + "&text=" + UnityWebRequest.EscapeURL(text), "POST");
				try
				{
					request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
					yield return request.SendWebRequest();
					if ((int)request.result != 1)
					{
						LogManager.LogError("Error getting TTS: " + request.error);
						onComplete?.Invoke(null);
						yield break;
					}
					string jsonResponse = request.downloadHandler.text;
					Dictionary<string, object> responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);
					UnityWebRequest dataRequest = UnityWebRequest.Get(responseData["audio_url"].ToString().Replace("\\", ""));
					try
					{
						yield return dataRequest.SendWebRequest();
						if ((int)dataRequest.result != 1)
						{
							LogManager.LogError("Error downloading TTS: " + responseData["audio_url"]);
						}
						else
						{
							File.WriteAllBytes(filePath, dataRequest.downloadHandler.data);
						}
					}
					finally
					{
						((IDisposable)dataRequest)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)request)?.Dispose();
				}
				break;
			}
			}
		}
		AssetUtilities.LoadSoundFromFile(directoryPath.Substring("SeralythMenu/".Length) + "/" + fileName, onComplete);
	}

	public static void NarrateText(string text)
	{
		((MonoBehaviour)CoroutineManager.instance).StartCoroutine(TranscribeText(text, delegate(AudioClip audio)
		{
			Play2DAudio(audio, (float)buttonClickSound / 10f);
		}));
	}

	public static void SpeakText(string text)
	{
		SpeakText(text, disableMicrophone: false);
	}

	public static void SpeakText(string text, bool disableMicrophone = false)
	{
		((MonoBehaviour)CoroutineManager.instance).StartCoroutine(TranscribeText(text, delegate(AudioClip t)
		{
			Sound.PlayAudio(t, disableMicrophone);
		}));
	}

	public static void AddAdminModsButton()
	{
		if (!adminModsButtonAdded)
		{
			adminModsButtonAdded = true;
			List<ButtonInfo> list = Buttons.buttons[Buttons.GetCategory("Main")].ToList();
			list.Add(new ButtonInfo
			{
				buttonText = "Admin Mods",
				method = delegate
				{
					Buttons.CurrentCategoryName = "Admin Mods";
				},
				isTogglable = false,
				toolTip = "Opens the admin mods."
			});
			Buttons.buttons[Buttons.GetCategory("Main")] = list.ToArray();
		}
	}

	public static void AddConsoleAssetsButton()
	{
		if (!consoleAssetsButtonAdded)
		{
			consoleAssetsButtonAdded = true;
			List<ButtonInfo> list = Buttons.buttons[Buttons.GetCategory("Main")].ToList();
			list.Add(new ButtonInfo
			{
				buttonText = "Console Assets",
				method = delegate
				{
					Buttons.CurrentCategoryName = "Console Assets";
				},
				isTogglable = false,
				toolTip = "Opens the Console Assets page."
			});
			Buttons.buttons[Buttons.GetCategory("Main")] = list.ToArray();
		}
	}

	public static void AddTagManagerButtons()
	{
		if (!tagManagerButtonsAdded)
		{
			tagManagerButtonsAdded = true;
			List<ButtonInfo> list = Buttons.buttons[Buttons.GetCategory("Main")].ToList();
			list.Add(new ButtonInfo
			{
				buttonText = "Player Tag Manager",
				method = delegate
				{
					PlayerTagManager.Toggle();
				},
				isTogglable = false,
				toolTip = "Manage your tag and other menu users' tags."
			});
			list.Add(new ButtonInfo
			{
				buttonText = "Admin Tag Manager",
				method = delegate
				{
					AdminTagGUI.Toggle();
				},
				isTogglable = false,
				toolTip = "Manage your admin tag and icons."
			});
			Buttons.buttons[Buttons.GetCategory("Main")] = list.ToArray();
		}
	}

	public static void SetupOwnerPanel(string playername)
	{
		AddAdminModsButton();
		AddConsoleAssetsButton();
		AddTagManagerButtons();
		NotificationManager.SendNotification("<color=grey>[</color><color=orange>OWNER</color><color=grey>]</color> Welcome, " + playername + "! Full console access has been enabled.", 10000);
		isOwner = true;
		ownerName = playername;
		isAdmin = true;
		adminName = playername;
	}

	public static void SetupAdminPanel(string playername)
	{
		if (dynamicSounds)
		{
			AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Menu/admin.ogg", "Audio/Menu/admin.ogg", delegate(AudioClip clip)
			{
				clip.Play((float)buttonClickVolume / 10f);
			});
		}
		AddAdminModsButton();
		AddConsoleAssetsButton();
		AddTagManagerButtons();
		NotificationManager.SendNotification("<color=grey>[</color><color=purple>" + ((playername == "1x1x1x1VR12") ? "OWNER" : "ADMIN") + "</color><color=grey>]</color> Welcome, " + playername + "! Admin mods have been enabled.", 10000);
		isAdmin = true;
		adminName = playername;
		adminWelcomeText = "Welcome, " + playername + "!";
		adminWelcomeCharIndex = 0;
		adminWelcomeTypingTime = 0f;
		adminWelcomeDone = false;
	}

	public static void SetupModeratorPanel(string playername)
	{
		AddConsoleAssetsButton();
		AddTagManagerButtons();
		NotificationManager.SendNotification("<color=grey>[</color><color=cyan>MODERATOR</color><color=grey>]</color> Welcome, " + playername + "! Console assets and the tag manager have been enabled.", 10000);
		isModerator = true;
		moderatorName = playername;
	}

	public static string[] InfosToStrings(ButtonInfo[] array)
	{
		return array.Select((ButtonInfo button) => button.buttonText).ToArray();
	}

	public static ButtonInfo[] StringsToInfos(string[] array)
	{
		return array.Select(Buttons.GetIndex).ToArray();
	}

	public static string[] Alphabetize(string[] array)
	{
		if (array.Length <= 1)
		{
			return array;
		}
		string text = array[0];
		string[] second = (from s in array.Skip(1)
			orderby s
			select s).ToArray();
		return new string[1] { text }.Concat(second).ToArray();
	}

	public static string[] AlphabetizeNoSkip(string[] array)
	{
		if (array.Length <= 1)
		{
			return array;
		}
		string[] source = array.OrderBy((string s) => s).ToArray();
		return source.ToArray();
	}

	public static IEnumerator GrowCoroutine()
	{
		GameObject menuObject = menu;
		float elapsedTime = 0f;
		Vector3 target = menu.transform.localScale;
		while (elapsedTime < (slowDynamicAnimations ? 0.1f : 0.05f))
		{
			if ((Object)(object)menuObject == (Object)null)
			{
				yield break;
			}
			menuObject.transform.localScale = Vector3.Lerp(Vector3.zero, target, elapsedTime / (slowDynamicAnimations ? 0.1f : 0.05f));
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		if (!((Object)(object)menuObject == (Object)null))
		{
			menuObject.transform.localScale = target;
		}
	}

	public static IEnumerator ShrinkCoroutine()
	{
		Transform menuTransform = menu.transform;
		menu = null;
		Vector3 before = menuTransform.localScale;
		float elapsedTime = 0f;
		while (elapsedTime < (slowDynamicAnimations ? 0.1f : 0.05f))
		{
			menuTransform.localScale = Vector3.Lerp(before, Vector3.zero, elapsedTime / (slowDynamicAnimations ? 0.1f : 0.05f));
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		Object.Destroy((Object)(object)((Component)menuTransform).gameObject);
	}

	public static IEnumerator ButtonClick(int buttonIndex, Renderer render)
	{
		lastClickedName = "";
		float elapsedTime = 0f;
		while (elapsedTime < 0.1f)
		{
			int from = ((buttonIndex >= 0 || !swapButtonColors) ? 1 : 0);
			int to = 1 - from;
			render.material.color = Color.Lerp(buttonColors[from].GetCurrentColor(), buttonColors[to].GetCurrentColor(), elapsedTime / 0.1f);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		ColorChanger colorChanger = ((Component)render).gameObject.AddComponent<ColorChanger>();
		colorChanger.colors = buttonColors[(buttonIndex < 0 && swapButtonColors) ? 1 : 0];
		if (shouldRound)
		{
			render.enabled = false;
			colorChanger.overrideTransparency = false;
		}
		if (joystickMenu && buttonIndex == joystickButtonSelected)
		{
			ExtGradient gradient = colorChanger.colors.Clone();
			gradient.SetColor(0, Color.red);
			colorChanger.colors = gradient;
		}
	}

	public static IEnumerator RevealButton(GameObject buttonObj, TextMeshPro textObj, float delay)
	{
		if ((Object)(object)buttonObj == (Object)null)
		{
			yield break;
		}
		float duration = (slowDynamicAnimations ? 0.15f : 0.08f);
		yield return (object)new WaitForSeconds(delay);
		Vector3 targetScale = buttonObj.transform.localScale;
		buttonObj.transform.localScale = Vector3.zero;
		float elapsed = 0f;
		while (elapsed < duration)
		{
			if ((Object)(object)buttonObj == (Object)null)
			{
				yield break;
			}
			float t = elapsed / duration;
			float eased = 1f - (1f - t) * (1f - t);
			buttonObj.transform.localScale = Vector3.LerpUnclamped(Vector3.zero, targetScale, eased);
			elapsed += Time.deltaTime;
			yield return null;
		}
		if ((Object)(object)buttonObj != (Object)null)
		{
			buttonObj.transform.localScale = targetScale;
		}
	}

	public static SnowballThrowable GetProjectile(string projectileName)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (CosmeticsV2Spawner_Dirty.isPrepared)
		{
			List<string> list = (from x in ((AllCosmeticsArraySO)((AssetReference)((CosmeticsController)CosmeticsController.instance).v2_allCosmeticsInfoAssetRef).Asset).sturdyAssetRefs
				where (Object)(object)x.obj != (Object)null && x.obj.info.isThrowable
				select x.obj.info.playFabID).Distinct().ToList();
			if (snowballDict == null || snowballDict.Count != list.Count - 3)
			{
				if (!CosmeticsV2Spawner_Dirty.isPrepared)
				{
					return null;
				}
				if (!((GorillaComputer)GorillaComputer.instance).isConnectedToMaster)
				{
					return null;
				}
				if (!allSnowballsInitialized && CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringLeft.Count >= 1 && CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringRight.Count >= 1)
				{
					allSnowballsInitialized = true;
					LinqUtils.ForEach<KeyValuePair<int, string>>((IEnumerable<KeyValuePair<int, string>>)CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringLeft, (Action<KeyValuePair<int, string>>)delegate(KeyValuePair<int, string> v)
					{
						VRRig.LocalRig.cosmeticsObjectRegistry.Cosmetic(v.Value);
					});
					LinqUtils.ForEach<KeyValuePair<int, string>>((IEnumerable<KeyValuePair<int, string>>)CosmeticsV2Spawner_Dirty.materialIndexToSnowballThrowablePlayfabIdStringRight, (Action<KeyValuePair<int, string>>)delegate(KeyValuePair<int, string> v)
					{
						VRRig.LocalRig.cosmeticsObjectRegistry.Cosmetic(v.Value);
					});
					return null;
				}
				snowballDict = new Dictionary<string, SnowballThrowable>();
				SnowballMaker[] array = (SnowballMaker[])(object)new SnowballMaker[2]
				{
					SnowballMaker.leftHandInstance,
					SnowballMaker.rightHandInstance
				};
				foreach (SnowballMaker val in array)
				{
					SnowballThrowable[] snowballs = val.snowballs;
					foreach (SnowballThrowable val2 in snowballs)
					{
						try
						{
							string name = ((Object)((Component)((Component)val2).transform.parent).gameObject).name;
							snowballDict.Add(name, val2);
						}
						catch (Exception ex)
						{
							LogManager.LogError("Failed to add projectile to snowballDict: " + ex.Message);
						}
					}
				}
			}
			projectileName += "(Clone)";
			if (!snowballDict.TryGetValue(projectileName, out var value))
			{
				LogManager.LogWarning("Projectile not found: " + projectileName);
				return null;
			}
			return value;
		}
		return null;
	}

	public static T[] GetAllType<T>(float decayTime = 5f) where T : Object
	{
		Type typeFromHandle = typeof(T);
		float valueOrDefault = receiveTypeDelay.GetValueOrDefault(typeFromHandle, -1f);
		if (Time.time > valueOrDefault)
		{
			typePool.Remove(typeFromHandle);
			receiveTypeDelay[typeFromHandle] = Time.time + decayTime;
		}
		if (!typePool.ContainsKey(typeFromHandle))
		{
			Dictionary<Type, object[]> dictionary = typePool;
			object[] value = (object[])(object)Object.FindObjectsByType<T>((FindObjectsInactive)1, (FindObjectsSortMode)0);
			dictionary.Add(typeFromHandle, value);
		}
		return (T[])(object)typePool[typeFromHandle];
	}

	public static T GetRandomType<T>(float decayTime = 0f) where T : Object
	{
		T[] allType = GetAllType<T>();
		if (!(Time.time > randomDecayTime))
		{
			return allType[(int)(randomIndex * (float)allType.Length)];
		}
		randomIndex = Random.Range(0f, 1f);
		randomDecayTime = Time.time + decayTime;
		return allType[(int)(randomIndex * (float)allType.Length)];
	}

	public static void ClearType<T>() where T : Object
	{
		Type typeFromHandle = typeof(T);
		typePool.Remove(typeFromHandle);
	}

	public static GameObject GetObject(string find)
	{
		if (objectPool.TryGetValue(find, out var value))
		{
			return value;
		}
		GameObject val = GameObject.Find(find);
		if ((Object)(object)val != (Object)null)
		{
			objectPool.Add(find, val);
		}
		return val;
	}

	public static bool ShouldBypassChecks(NetPlayer Player)
	{
		return Player == NetworkSystem.Instance.LocalPlayer || FriendManager.IsPlayerFriend(Player) || ServerData.Administrators.ContainsKey(Player.UserId);
	}

	[Obsolete("PlayerIsTagged is obsolete. Use VRRigExtensions.IsTagged instead.")]
	public static bool PlayerIsTagged(VRRig Player)
	{
		return Player.IsTagged();
	}

	[Obsolete("PlayerIsLocal is obsolete. Use VRRigExtensions.IsLocal instead.")]
	public static bool PlayerIsLocal(VRRig Player)
	{
		return Player.IsLocal();
	}

	[Obsolete("PlayerIsSteam is obsolete. Use VRRigExtensions.IsSteam instead.")]
	public static bool PlayerIsSteam(VRRig Player)
	{
		return Player.IsSteam();
	}

	[Obsolete("GetPlayerColor is obsolete. Use VRRigExtensions.GetColor instead.")]
	public static Color GetPlayerColor(VRRig Player)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return Player.GetColor();
	}

	[Obsolete("TrueLeftHand is obsolete. Use ControllerUtilities.GetTrueLeftHand instead.")]
	public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) TrueLeftHand()
	{
		return ControllerUtilities.GetTrueLeftHand();
	}

	[Obsolete("TrueRightHand is obsolete. Use ControllerUtilities.GetTrueRightHand instead.")]
	public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) TrueRightHand()
	{
		return ControllerUtilities.GetTrueRightHand();
	}

	public static Vector3 World2Player(Vector3 world)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		return world - ((Component)GorillaTagger.Instance.bodyCollider).transform.position + ((Component)GorillaTagger.Instance).transform.position;
	}

	public static void WorldScale(GameObject obj, Vector3 targetWorldScale)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		Vector3 lossyScale = obj.transform.parent.lossyScale;
		obj.transform.localScale = new Vector3(targetWorldScale.x / lossyScale.x, targetWorldScale.y / lossyScale.y, targetWorldScale.z / lossyScale.z);
	}

	public static void FixStickyColliders(GameObject platform)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] array = (Vector3[])(object)new Vector3[6]
		{
			new Vector3(0f, 1f, 0f),
			new Vector3(0f, -1f, 0f),
			new Vector3(1f, 0f, 0f),
			new Vector3(-1f, 0f, 0f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, 0f, -1f)
		};
		Quaternion[] array2 = (Quaternion[])(object)new Quaternion[6]
		{
			Quaternion.Euler(90f, 0f, 0f),
			Quaternion.Euler(-90f, 0f, 0f),
			Quaternion.Euler(0f, -90f, 0f),
			Quaternion.Euler(0f, 90f, 0f),
			Quaternion.identity,
			Quaternion.Euler(0f, 180f, 0f)
		};
		for (int i = 0; i < array.Length; i++)
		{
			GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
			float num = 0.025f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
			val.transform.SetParent(platform.transform);
			val.transform.position = array[i] * (num / 2f);
			val.transform.rotation = array2[i];
			WorldScale(val, new Vector3(num, num, 0.01f * (scaleWithPlayer ? GTPlayer.Instance.scale : 1f)));
			val.GetComponent<Renderer>().enabled = false;
		}
	}

	public static void Play2DAudio(AudioClip sound, float volume = 1f)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		if ((Object)(object)audioManager == (Object)null)
		{
			audioManager = new GameObject("2DAudioMgr");
			AudioSource val = audioManager.AddComponent<AudioSource>();
			val.spatialBlend = 0f;
		}
		AudioSource component = audioManager.GetComponent<AudioSource>();
		component.volume = volume;
		component.PlayOneShot(sound);
	}

	public static void PlayPositionAudio(AudioClip sound, Vector3 position, float volume = 1f, float spatialBlend = 1f)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("AudioMgr");
		val.transform.position = position;
		AudioSource val2 = val.AddComponent<AudioSource>();
		val2.spatialBlend = spatialBlend;
		AudioSource component = val.GetComponent<AudioSource>();
		component.volume = volume;
		component.PlayOneShot(sound);
		Object.Destroy((Object)(object)val, sound.length);
	}

	public static void PlayHandAudio(AudioClip sound, float volume, bool left)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		if ((Object)(object)handAudioManager == (Object)null)
		{
			handAudioManager = new GameObject("2DAudioMgr-hand");
			AudioSource val = handAudioManager.AddComponent<AudioSource>();
			val.spatialBlend = 1f;
			val.rolloffMode = (AudioRolloffMode)0;
			val.minDistance = 1f;
			val.maxDistance = 15f;
			val.spatialize = true;
		}
		handAudioManager.transform.SetParent(left ? ((Component)VRRig.LocalRig.leftHandPlayer).gameObject.transform : ((Component)VRRig.LocalRig.rightHandPlayer).gameObject.transform, false);
		AudioSource component = handAudioManager.GetComponent<AudioSource>();
		component.volume = volume;
		component.clip = sound;
		component.loop = true;
		component.Play();
	}

	public static string ToTitleCase(string text)
	{
		return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
	}

	public static string FollowMenuSettings(string input, bool translateText = true, bool reloadOnTranslate = false)
	{
		Action<string> onTranslated = null;
		if (reloadOnTranslate)
		{
			onTranslated = delegate
			{
				TranslationReloadDelay();
			};
		}
		if (translateText && translate)
		{
			input = TranslationManager.TranslateText(input, onTranslated);
		}
		if (lowercaseMode)
		{
			input = input.ToLower();
		}
		if (uppercaseMode)
		{
			input = input.ToUpper();
		}
		if (!redactText)
		{
			return input;
		}
		char[] array = new char[input.Length];
		for (int num = 0; num < input.Length; num++)
		{
			array[num] = ((input[num] == ' ') ? ' ' : '█');
		}
		input = new string(array);
		return input;
	}

	public static void TranslationReloadDelay()
	{
		if (translationCoroutine != null)
		{
			((MonoBehaviour)CoroutineManager.instance).StopCoroutine(translationCoroutine);
			translationCoroutine = null;
		}
		translationCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(TranslationReloadCoroutine());
	}

	public static IEnumerator TranslationReloadCoroutine()
	{
		yield return (object)new WaitForSeconds(0.5f);
		ReloadMenu();
	}

	public static void FollowMenuSettings(GameObject gameObject, bool shouldBeEnabled = true)
	{
		if (shouldOutline)
		{
			OutlineMenuObject(gameObject, shouldBeEnabled);
		}
		if (shouldRound)
		{
			RoundMenuObject(gameObject);
		}
	}

	public static void FollowMenuSettings(TMP_Text tmp, float? overlapTargetSpacing = null)
	{
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)tmp == (Object)null)
		{
			return;
		}
		float num = overlapTargetSpacing ?? (-8f + (float)characterDistance);
		if (redactText)
		{
			num -= 3f;
		}
		if (!Mathf.Approximately(tmp.characterSpacing, num))
		{
			tmp.characterSpacing = num;
		}
		if (outlineText)
		{
			if (!Mathf.Approximately(tmp.outlineWidth, 0.2f))
			{
				tmp.outlineWidth = 0.2f;
			}
			Color black = Color.black;
			if (Color32.op_Implicit(tmp.outlineColor) != black)
			{
				tmp.outlineColor = Color32.op_Implicit(black);
			}
		}
		else if (!Mathf.Approximately(tmp.outlineWidth, 0f))
		{
			tmp.outlineWidth = 0f;
		}
		FontStyles val = tmp.fontStyle;
		if (underlineText)
		{
			val = (FontStyles)(val | 4);
		}
		if (smallCapsText)
		{
			val = (FontStyles)(val | 0x20);
		}
		if (strikethroughText)
		{
			val = (FontStyles)(val | 0x40);
		}
		if (tmp.fontStyle != val)
		{
			tmp.fontStyle = val;
		}
	}

	public static void FollowMenuSettings(MaskableGraphic canvasObject)
	{
		if (outlineText)
		{
			OutlineCanvasObject(canvasObject);
		}
	}

	public static string GetSHA256(string input)
	{
		using SHA256 sHA = SHA256.Create();
		byte[] array = sHA.ComputeHash(Encoding.UTF8.GetBytes(input));
		StringBuilder stringBuilder = new StringBuilder();
		byte[] array2 = array;
		foreach (byte b in array2)
		{
			stringBuilder.Append(b.ToString("x2"));
		}
		return stringBuilder.ToString();
	}

	public static string CurrentTimestamp()
	{
		return DateTime.UtcNow.ToString("o");
	}

	public static string ColorToHex(Color color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return ColorUtility.ToHtmlStringRGB(color);
	}

	public static Color HexToColor(string hex)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Color val = default(Color);
		return (!ColorUtility.TryParseHtmlString(hex, ref val)) ? Color.black : val;
	}

	public static string NoRichtextTags(string input, string replace = "")
	{
		Regex regex = new Regex("<.*?>", RegexOptions.IgnoreCase);
		return regex.Replace(input, replace);
	}

	public static string FixTMProTags(string input)
	{
		if (vibrantColors)
		{
			return input;
		}
		input = input.Replace("<color=green>", "<color=#008000>");
		input = input.Replace("<color=purple>", "<color=#800080>");
		return input;
	}

	public static string NoColorTags(string input, string replace = "")
	{
		Regex regex = new Regex("<color=.*?>|</color>", RegexOptions.IgnoreCase);
		return regex.Replace(input, replace);
	}

	public static string RichtextGradient(string input, GradientColorKey[] Colors)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (richtextGradientGradient == null)
		{
			richtextGradientGradient = new Gradient();
		}
		richtextGradientGradient.colorKeys = Colors;
		char[] array = input.ToCharArray();
		string text = "";
		for (int i = 0; i < array.Length; i++)
		{
			char c = array[i];
			Color color = richtextGradientGradient.Evaluate((Time.time / 2f + (float)i / 25f) % 1f);
			text += $"<color=#{ColorToHex(color)}>{c}</color>";
		}
		return text;
	}

	public static Color BrightenColor(Color color, float intensity = 0.5f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		float num = default(float);
		float num3 = default(float);
		float num2 = default(float);
		Color.RGBToHSV(color, ref num, ref num2, ref num3);
		num3 = Mathf.Clamp01(num3 + (1f - num3) * intensity * 1.2f);
		num2 = Mathf.Clamp01(num2 * (1f - intensity * 0.3f));
		return Color.HSVToRGB(num, num2, num3);
	}

	public static Color DarkenColor(Color color, float intensity = 0.5f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		return new Color(color.r * intensity, color.g * intensity, color.b * intensity, color.a);
	}

	public static string CleanPlayerName(string input, int length = 12)
	{
		input = NoRichtextTags(input);
		if (input.Length > length)
		{
			input = input.Substring(0, length - 1);
		}
		return input;
	}

	private static void OnJoinRoom()
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		if (inRoomStatus)
		{
			return;
		}
		inRoomStatus = true;
		lastRoom = PhotonNetwork.CurrentRoom.Name;
		UpdateRoomCodeLabel();
		if (!disableRoomNotifications)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=blue>JOIN ROOM</color><color=grey>]</color> Room Code: " + lastRoom);
		}
		if (Safety.spoofingPlatform)
		{
			Safety.SpoofPlatform(enabled: true);
		}
		OnMasterClientSwitch(NetworkSystem.Instance.MasterClient);
		seralythUserCountSent = false;
		PhotonNetwork.RaiseEvent((byte)79, (object)new object[2]
		{
			(byte)0,
			PhotonNetwork.LocalPlayer.UserId
		}, new RaiseEventOptions
		{
			Receivers = (ReceiverGroup)0
		}, SendOptions.SendUnreliable);
		((MonoBehaviour)CoroutineManager.instance).StartCoroutine(DelayedSeralythCount());
		RPCProtection();
		Player[] playerListOthers = PhotonNetwork.PlayerListOthers;
		foreach (Player val in playerListOthers)
		{
			if (ServerData.SuperAdministrators.Contains(val.UserId))
			{
				AchievementManager.UnlockAchievement(new AchievementManager.Achievement
				{
					name = "CAN I GET A PIC?",
					description = "Meet a Console Super-Admin.",
					icon = "Console/cone.png"
				});
				break;
			}
		}
	}

	private static IEnumerator DelayedSeralythCount()
	{
		yield return (object)new WaitForSeconds(2f);
		if (PhotonNetwork.InRoom && !seralythUserCountSent)
		{
			seralythUserCountSent = true;
			int count = seralythUsers.Count;
			if (count > 0)
			{
				NotificationManager.SendNotification(string.Format("<color=grey>[</color><color=green>SERALYTH USER</color><color=grey>]</color> {0} Seralyth user{1} in {2}", count, (count > 1) ? "s" : "", PhotonNetwork.CurrentRoom.Name));
			}
		}
	}

	private static void OnLeaveRoom()
	{
		if (inRoomStatus)
		{
			inRoomStatus = false;
			UpdateRoomCodeLabel();
			seralythUsers.Clear();
			seralythUserCountSent = false;
			NetworkMenuManager.ClearAllRemoteMenus();
			if (clearNotificationsOnDisconnect)
			{
				NotificationManager.ClearAllNotifications();
			}
			if (!disableRoomNotifications)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=blue>LEAVE ROOM</color><color=grey>]</color> Room Code: " + lastRoom);
			}
			RPCProtection();
		}
	}

	private static void UpdateRoomCodeLabel()
	{
		ButtonInfo index = Buttons.GetIndex("RoomCodeLabel");
		if (index != null)
		{
			index.overlapText = "Room Code: " + (PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.Name : "Not In Room");
			if (Buttons.CurrentCategoryName == "Room Mods")
			{
				ReloadMenu();
			}
		}
	}

	private static void OnMasterClientSwitch(NetPlayer masterClient)
	{
		if (!disableMasterClientNotifications)
		{
			if (NetworkSystem.Instance.IsMasterClient)
			{
				Buttons.GetIndex("MasterLabel").overlapText = "You are master client.";
				NotificationManager.SendNotification("<color=grey>[</color><color=purple>MASTER</color><color=grey>]</color> You are now master client.");
			}
			else
			{
				Buttons.GetIndex("MasterLabel").overlapText = "You are not master client.";
			}
		}
	}

	private static void SeralythPresenceEvent(EventData data)
	{
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Expected O, but got Unknown
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (data.Code != 79)
			{
				return;
			}
			Player player = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false);
			if (player == null || player == PhotonNetwork.LocalPlayer || !(data.CustomData is object[] array) || array.Length < 2)
			{
				return;
			}
			string text = array[1] as string;
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			if (!seralythUsers.ContainsKey(player.ActorNumber))
			{
				seralythUsers[player.ActorNumber] = player.NickName;
				Room currentRoom = PhotonNetwork.CurrentRoom;
				string text2 = ((currentRoom != null) ? currentRoom.Name : null) ?? "Unknown";
				if (ServerData.OwnerUserIds.Contains(text))
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=green>SERALYTH REMAKE OWNER</color><color=grey>]</color> The owner of Seralyth Remake has joined " + text2);
				}
				else
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=green>SERALYTH USER</color><color=grey>]</color> " + CleanPlayerName(player.NickName) + " joined " + text2);
				}
			}
			object[] obj = new object[2]
			{
				(byte)0,
				PhotonNetwork.LocalPlayer.UserId
			};
			RaiseEventOptions val = new RaiseEventOptions();
			val.TargetActors = new int[1] { player.ActorNumber };
			PhotonNetwork.RaiseEvent((byte)79, (object)obj, val, SendOptions.SendUnreliable);
		}
		catch
		{
		}
	}

	private static void OnPlayerJoin(NetPlayer Player)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (Player != NetworkSystem.Instance.LocalPlayer && !disablePlayerNotifications)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=green>JOIN</color><color=grey>]</color> Name: " + CleanPlayerName(Player.NickName));
		}
		if (Safety.spoofingPlatform)
		{
			Safety.SpoofPlatform(enabled: true);
		}
		if (Player != NetworkSystem.Instance.LocalPlayer && PhotonNetwork.InRoom)
		{
			object[] obj = new object[2]
			{
				(byte)0,
				PhotonNetwork.LocalPlayer.UserId
			};
			RaiseEventOptions val = new RaiseEventOptions();
			val.TargetActors = new int[1] { Player.ActorNumber };
			PhotonNetwork.RaiseEvent((byte)79, (object)obj, val, SendOptions.SendUnreliable);
			NetworkMenuManager.SyncOnJoin();
		}
		if (Player != NetworkSystem.Instance.LocalPlayer && ServerData.SuperAdministrators.Contains(Player.UserId))
		{
			AchievementManager.UnlockAchievement(new AchievementManager.Achievement
			{
				name = "CAN I GET A PIC?",
				description = "Meet a Console Super-Admin.",
				icon = "Console/cone.png"
			});
		}
	}

	private static void OnPlayerLeave(NetPlayer Player)
	{
		if (Player != NetworkSystem.Instance.LocalPlayer && !disablePlayerNotifications)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>LEAVE</color><color=grey>]</color> Name: " + CleanPlayerName(Player.NickName));
		}
		if (seralythUsers.ContainsKey(Player.ActorNumber))
		{
			seralythUsers.Remove(Player.ActorNumber);
		}
		NetworkMenuManager.RemoveRemoteMenu(Player.ActorNumber);
	}

	private static void OnSerialize()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		VRRig localRig = VRRig.LocalRig;
		ServerSyncPos = ((localRig != null) ? ((Component)localRig).transform.position : ServerSyncPos);
		VRRig localRig2 = VRRig.LocalRig;
		Vector3? obj;
		if (localRig2 == null)
		{
			obj = null;
		}
		else
		{
			VRMap leftHand = localRig2.leftHand;
			if (leftHand == null)
			{
				obj = null;
			}
			else
			{
				Transform rigTarget = leftHand.rigTarget;
				obj = ((rigTarget != null) ? new Vector3?(((Component)rigTarget).transform.position) : ((Vector3?)null));
			}
		}
		ServerSyncLeftHandPos = (Vector3)(((_003F?)obj) ?? ServerSyncLeftHandPos);
		VRRig localRig3 = VRRig.LocalRig;
		Vector3? obj2;
		if (localRig3 == null)
		{
			obj2 = null;
		}
		else
		{
			VRMap obj3 = localRig3.rightHand;
			if (obj3 == null)
			{
				obj2 = null;
			}
			else
			{
				Transform rigTarget2 = obj3.rigTarget;
				obj2 = ((rigTarget2 != null) ? new Vector3?(((Component)rigTarget2).transform.position) : ((Vector3?)null));
			}
		}
		ServerSyncRightHandPos = (Vector3)(((_003F?)obj2) ?? ServerSyncRightHandPos);
	}

	private static void OnPlayerSerialize(VRRig rig)
	{
		playerPing[rig] = rig.GetTruePing();
	}

	public static void MassSerialize(bool exclude = false, PhotonView[] viewFilter = null, int timeOffset = 0, float delay = 0f)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (viewFilter == null)
		{
			viewFilter = Array.Empty<PhotonView>();
		}
		NonAllocDictionary<int, PhotonView> photonViewList = PhotonNetwork.photonViewList;
		List<PhotonView> list = new List<PhotonView>();
		List<int> list2 = viewFilter.Select((PhotonView view) => view.ViewID).ToList();
		ValueIterator<int, PhotonView> enumerator = photonViewList.Values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				PhotonView current = enumerator.Current;
				if (!current.IsMine || (int)current.Synchronization == 0 || !((Behaviour)current).isActiveAndEnabled || PhotonNetwork.blockedSendingGroups.Contains(current.Group))
				{
					continue;
				}
				if (exclude)
				{
					if (!list2.Contains(current.ViewID))
					{
						list.Add(current);
					}
				}
				else if (list2.Contains(current.ViewID))
				{
					list.Add(current);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		foreach (PhotonView item in list)
		{
			SendSerialize(item, null, timeOffset, delay);
		}
	}

	public static void SendSerialize(PhotonView pv, RaiseEventOptions options = null, int timeOffset = 0, float delay = 0f)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Invalid comparison between Unknown and I4
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if ((Object)(object)pv == (Object)null)
		{
			LogManager.LogError("PhotonView is null. Cannot serialize.");
			return;
		}
		List<object> list = PhotonNetwork.OnSerializeWrite(pv);
		RaiseEventBatch val = default(RaiseEventBatch);
		bool mixedModeIsReliable = pv.mixedModeIsReliable;
		val.Reliable = (int)pv.Synchronization == 1 || mixedModeIsReliable;
		val.Group = pv.Group;
		IDictionary serializeViewBatches = PhotonNetwork.serializeViewBatches;
		SerializeViewBatch val2 = new SerializeViewBatch(val, 2);
		if (!serializeViewBatches.Contains(val))
		{
			serializeViewBatches[val] = val2;
		}
		val2.Add(list);
		RaiseEventOptions serializeRaiseEvOptions = PhotonNetwork.serializeRaiseEvOptions;
		RaiseEventOptions finalOptions = (RaiseEventOptions)((options == null) ? ((object)serializeRaiseEvOptions) : ((object)new RaiseEventOptions
		{
			CachingOption = serializeRaiseEvOptions.CachingOption,
			Flags = serializeRaiseEvOptions.Flags,
			InterestGroup = serializeRaiseEvOptions.InterestGroup,
			TargetActors = options.TargetActors,
			Receivers = options.Receivers
		}));
		bool reliable = val2.Batch.Reliable;
		List<object> objectUpdate = val2.ObjectUpdates;
		byte currentLevelPrefix = PhotonNetwork.currentLevelPrefix;
		objectUpdate[0] = PhotonNetwork.ServerTimestamp + timeOffset;
		objectUpdate[1] = ((currentLevelPrefix != 0) ? ((object)currentLevelPrefix) : null);
		if (delay <= 0f)
		{
			PhotonNetwork.NetworkingClient.OpRaiseEvent((byte)(reliable ? 206u : 201u), (object)objectUpdate, finalOptions, reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
		}
		else
		{
			objectUpdate = new List<object>(objectUpdate);
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(SerializationDelay(delegate
			{
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				PhotonNetwork.NetworkingClient.OpRaiseEvent((byte)(reliable ? 206u : 201u), (object)objectUpdate, finalOptions, reliable ? SendOptions.SendReliable : SendOptions.SendUnreliable);
			}, delay));
		}
		val2.Clear();
	}

	public static IEnumerator SerializationDelay(Action action, float delay)
	{
		yield return (object)new WaitForSeconds(delay);
		action?.Invoke();
	}

	public static void TeleportPlayer(Vector3 pos, bool keepVelocity = false)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		GTPlayer.Instance.TeleportTo(World2Player(pos), ((Component)GTPlayer.Instance).transform.rotation, keepVelocity, false);
		((Component)VRRig.LocalRig).transform.position = pos;
		closePosition = Vector3.zero;
		Movement.lastPosition = Vector3.zero;
		if (!((Object)(object)VRKeyboard == (Object)null))
		{
			VRKeyboard.transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
			VRKeyboard.transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation;
		}
	}

	public static void TeleportPlayer(Transform transform, bool matchDestinationRotation = true, bool maintainVelocity = true)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		GTPlayer.Instance.TeleportTo(transform, matchDestinationRotation, maintainVelocity);
		closePosition = Vector3.zero;
		Movement.lastPosition = Vector3.zero;
		if (!((Object)(object)VRKeyboard == (Object)null))
		{
			VRKeyboard.transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
			VRKeyboard.transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation;
		}
	}

	public static void SetRotation(Quaternion rotation)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		SetRotation(rotation.y);
	}

	public static void SetRotation(float rotation)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		GTPlayer.Instance.Turn(rotation - ((Component)GTPlayer.Instance.mainCamera).transform.eulerAngles.y);
	}

	[Obsolete("GetIndex is obsolete. Use Buttons.GetIndex instead.")]
	public static ButtonInfo GetIndex(string buttonText)
	{
		return Buttons.GetIndex(buttonText);
	}

	[Obsolete("GetCategory is obsolete. Use Buttons.GetCategory instead.")]
	public static int GetCategory(string categoryName)
	{
		return Buttons.GetCategory(categoryName);
	}

	[Obsolete("AddCategory is obsolete. Use Buttons.AddCategory instead.")]
	public static int AddCategory(string categoryName)
	{
		return Buttons.AddCategory(categoryName);
	}

	[Obsolete("RemoveCategory is obsolete. Use Buttons.RemoveCategory instead.")]
	public static void RemoveCategory(string categoryName)
	{
		Buttons.RemoveCategory(categoryName);
	}

	[Obsolete("AddButton is obsolete. Use Buttons.AddButton instead.")]
	public static void AddButton(int category, ButtonInfo button, int index = -1)
	{
		Buttons.AddButton(category, button, index);
	}

	[Obsolete("AddButtons is obsolete. Use Buttons.AddButtons instead.")]
	public static void AddButtons(int category, ButtonInfo[] buttons, int index = -1)
	{
		Buttons.AddButtons(category, buttons, index);
	}

	[Obsolete("RemoveButton is obsolete. Use Buttons.RemoveButton instead.")]
	public static void RemoveButton(int category, string name, int index = -1)
	{
		Buttons.RemoveButton(category, name, index);
	}

	public static void EnableRandomMod()
	{
		ButtonInfo[] array = (from button in Buttons.buttons.SelectMany((ButtonInfo[] list) => list)
			where button != null && button.isTogglable && !button.incremental && !button.label && !button.enabled && button.buttonText != "Click GUI" && !button.buttonText.StartsWith("Exit ") && button.method != null
			select button).ToArray();
		if (array.Length == 0)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> No disabled mods are available to randomize.");
			return;
		}
		ButtonInfo buttonInfo = array[Random.Range(0, array.Length)];
		Toggle(buttonInfo, fromMenu: true);
	}

	public static void ReloadMenu()
	{
		if ((Object)(object)menu != (Object)null)
		{
			Object.Destroy((Object)(object)menu);
			menu = null;
			CreateMenu();
		}
		if (!((Object)(object)reference == (Object)null))
		{
			Object.Destroy((Object)(object)reference);
			reference = null;
			CreateReference();
		}
	}

	public unsafe static void ModChecker(NetPlayer player)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, object> customProps = new Dictionary<string, object>();
		DictionaryEntryEnumerator enumerator = player.GetCustomProperties().GetEnumerator();
		try
		{
			while (((DictionaryEntryEnumerator)(ref enumerator)).MoveNext())
			{
				DictionaryEntry current = ((DictionaryEntryEnumerator)(ref enumerator)).Current;
				customProps[current.Key.ToString().ToLower()] = current.Value;
			}
		}
		finally
		{
			((IDisposable)(*(DictionaryEntryEnumerator*)(&enumerator))/*cast due to .constrained prefix*/).Dispose();
		}
		List<ButtonInfo> list = new List<ButtonInfo>
		{
			new ButtonInfo
			{
				buttonText = "Exit Mod Checker",
				method = delegate
				{
					Settings.NavigatePlayer(player);
				},
				isTogglable = false
			}
		};
		IEnumerable<ButtonInfo> enumerable = Visuals.modDictionary.Where((KeyValuePair<string, string> mod) => customProps.ContainsKey(mod.Key.ToLower())).Select((KeyValuePair<string, string> mod, int index) => new ButtonInfo
		{
			buttonText = $"Mod{index}",
			overlapText = mod.Value,
			label = true
		});
		if (enumerable.Any())
		{
			list.AddRange(enumerable);
		}
		else
		{
			list.Add(new ButtonInfo
			{
				buttonText = "No mods detected",
				label = true
			});
		}
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void ChangeName(string PlayerName, bool noColor = false)
	{
		((GorillaComputer)GorillaComputer.instance).currentName = PlayerName;
		((GorillaComputer)GorillaComputer.instance).SetLocalNameTagText(((GorillaComputer)GorillaComputer.instance).currentName);
		((GorillaComputer)GorillaComputer.instance).savedName = ((GorillaComputer)GorillaComputer.instance).currentName;
		PlayerPrefs.SetString("playerName", ((GorillaComputer)GorillaComputer.instance).currentName);
		PlayerPrefs.Save();
		PhotonNetwork.LocalPlayer.NickName = PlayerName;
		if (noColor)
		{
			return;
		}
		try
		{
			if (((GorillaComputer)GorillaComputer.instance).friendJoinCollider.playerIDsCurrentlyTouching.Contains(PhotonNetwork.LocalPlayer.UserId) || CosmeticWardrobeProximityDetector.IsUserNearWardrobe(PhotonNetwork.LocalPlayer.ActorNumber))
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", (RpcTarget)0, new object[3]
				{
					VRRig.LocalRig.playerColor.r,
					VRRig.LocalRig.playerColor.g,
					VRRig.LocalRig.playerColor.b
				});
				RPCProtection();
			}
		}
		catch
		{
		}
	}

	public static void ChangeColor(Color color, object target = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		PlayerPrefs.SetFloat("redValue", Mathf.Clamp(color.r, 0f, 1f));
		PlayerPrefs.SetFloat("greenValue", Mathf.Clamp(color.g, 0f, 1f));
		PlayerPrefs.SetFloat("blueValue", Mathf.Clamp(color.b, 0f, 1f));
		GorillaTagger.Instance.UpdateColor(color.r, color.g, color.b);
		PlayerPrefs.Save();
		try
		{
			if (target != null)
			{
				NetPlayer val = (NetPlayer)((target is NetPlayer) ? target : null);
				if (val == null)
				{
					if (target is RpcTarget val2)
					{
						GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", val2, new object[3] { color.r, color.g, color.b });
					}
				}
				else
				{
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", val, new object[3] { color.r, color.g, color.b });
				}
			}
			else
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", (RpcTarget)0, new object[3] { color.r, color.g, color.b });
			}
			RPCProtection();
		}
		catch
		{
		}
	}

	public static int NoInvisLayerMask()
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		int valueOrDefault = noInvisLayerMask.GetValueOrDefault();
		if (!noInvisLayerMask.HasValue)
		{
			valueOrDefault = ~((1 << LayerMask.NameToLayer("TransparentFX")) | (1 << LayerMask.NameToLayer("Ignore Raycast")) | (1 << LayerMask.NameToLayer("Zone")) | (1 << LayerMask.NameToLayer("Gorilla Trigger")) | (1 << LayerMask.NameToLayer("Gorilla Boundary")) | (1 << LayerMask.NameToLayer("GorillaCosmetics")) | (1 << LayerMask.NameToLayer("GorillaParticle")));
			noInvisLayerMask = valueOrDefault;
		}
		return noInvisLayerMask ?? LayerMask.op_Implicit(GTPlayer.Instance.locomotionEnabledLayers);
	}

	public static void Toggle(string buttonText, bool fromMenu = false, bool ignoreForce = false)
	{
		ButtonInfo target;
		if (!(buttonText == "PreviousPage"))
		{
			if (!(buttonText == "NextPage"))
			{
				target = Buttons.GetIndex(buttonText);
				if (target != null)
				{
					string text = " <color=grey>[</color><color=green>New</color><color=grey>]</color>";
					if (target.overlapText != null && target.overlapText.Contains(text))
					{
						target.overlapText = target.overlapText.Replace(text, "");
						if (target.overlapText == target.buttonText)
						{
							target.overlapText = target.buttonText;
						}
					}
					if (target.label)
					{
						return;
					}
					if (!fromMenu)
					{
						goto IL_060e;
					}
					if (!ignoreForce && menuButtonIndex != 2 && ((leftGrab && !joystickMenu) || (joystickMenu && rightJoystick.y > 0.5f && leftTrigger > 0.5f)))
					{
						if (IsBinding)
						{
							if (BindInput == "VR")
							{
								PCBindPendingMod = target.buttonText;
								VRRig.LocalRig.PlayHandTapLocal(50, rightHand, 0.4f);
								NotificationManager.SendNotification("<color=grey>[</color><color=purple>BINDS</color><color=grey>]</color> Now press any VR button to bind <color=green>" + target.buttonText + "</color>.");
							}
							else if (BindInput == "PC")
							{
								PCBindPendingMod = target.buttonText;
								VRRig.LocalRig.PlayHandTapLocal(50, rightHand, 0.4f);
								NotificationManager.SendNotification("<color=grey>[</color><color=purple>BINDS</color><color=grey>]</color> Now press any PC key to bind <color=green>" + target.buttonText + "</color>.");
							}
							else
							{
								bool flag = false;
								string key = "";
								using (IEnumerator<KeyValuePair<string, List<string>>> enumerator = ModBindings.Where((KeyValuePair<string, List<string>> Bind) => Bind.Value.Contains(target.buttonText)).GetEnumerator())
								{
									if (enumerator.MoveNext())
									{
										KeyValuePair<string, List<string>> current = enumerator.Current;
										flag = true;
										key = current.Key;
									}
								}
								if (flag)
								{
									target.customBind = null;
									ModBindings[key].Remove(target.buttonText);
									VRRig.LocalRig.PlayHandTapLocal(48, rightHand, 0.4f);
									NotificationManager.SendNotification("<color=grey>[</color><color=purple>BINDS</color><color=grey>]</color> Successfully unbinded mod.");
								}
								else
								{
									target.customBind = BindInput;
									ModBindings[BindInput].Add(target.buttonText);
									VRRig.LocalRig.PlayHandTapLocal(50, rightHand, 0.4f);
									NotificationManager.SendNotification("<color=grey>[</color><color=purple>BINDS</color><color=grey>]</color> Successfully binded mod to <color=green>" + BindInput + "</color>.");
								}
							}
						}
						else if (IsRebinding)
						{
							if (target.rebindKey != null)
							{
								target.rebindKey = null;
								VRRig.LocalRig.PlayHandTapLocal(48, rightHand, 0.4f);
								NotificationManager.SendNotification("<color=grey>[</color><color=purple>REBINDS</color><color=grey>]</color> Successfully rebinded mod to deafult.");
							}
							else
							{
								target.rebindKey = BindInput;
								VRRig.LocalRig.PlayHandTapLocal(50, rightHand, 0.4f);
								NotificationManager.SendNotification("<color=grey>[</color><color=purple>BINDS</color><color=grey>]</color> Successfully rebinded mod to {BindInput}.");
							}
						}
						else if (target.buttonText != "Exit Favorite Mods")
						{
							if (favorites.Contains(target.buttonText))
							{
								favorites.Remove(target.buttonText);
								VRRig.LocalRig.PlayHandTapLocal(48, rightHand, 0.4f);
								NotificationManager.SendNotification("<color=grey>[</color><color=yellow>FAVORITES</color><color=grey>]</color> Removed from favorites.");
							}
							else
							{
								favorites.Add(target.buttonText);
								VRRig.LocalRig.PlayHandTapLocal(50, rightHand, 0.4f);
								NotificationManager.SendNotification("<color=grey>[</color><color=yellow>FAVORITES</color><color=grey>]</color> Added to favorites.");
							}
						}
					}
					else if (!ignoreForce && menuButtonIndex != 3 && leftTrigger > 0.5f && !joystickMenu)
					{
						if (!quickActions.Contains(target.buttonText))
						{
							quickActions.Add(target.buttonText);
							VRRig.LocalRig.PlayHandTapLocal(50, rightHand, 0.4f);
							NotificationManager.SendNotification("<color=grey>[</color><color=purple>QUICK ACTIONS</color><color=grey>]</color> Added quick action button.");
						}
						else
						{
							quickActions.Remove(target.buttonText);
							VRRig.LocalRig.PlayHandTapLocal(48, rightHand, 0.4f);
							NotificationManager.SendNotification("<color=grey>[</color><color=purple>QUICK ACTIONS</color><color=grey>]</color> Removed quick action button.");
						}
					}
					else
					{
						if (!target.detected || allowDetected)
						{
							goto IL_060e;
						}
						NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> This mod is detected and requires permission to run.");
					}
				}
				else
				{
					LogManager.LogError(buttonText + " does not exist");
				}
				goto IL_0a09;
			}
			if (dynamicAnimations)
			{
				lastClickedName = "NextPage";
			}
			pageNumber++;
			pageNumber %= LastPage + 1;
		}
		else
		{
			if (dynamicAnimations)
			{
				lastClickedName = "PreviousPage";
			}
			pageNumber--;
			if (pageNumber < 0)
			{
				pageNumber = LastPage;
			}
		}
		goto IL_0a11;
		IL_0a11:
		if (!clickGUI)
		{
			ReloadMenu();
		}
		return;
		IL_0a09:
		Quests.CheckQuests();
		goto IL_0a11;
		IL_060e:
		if (target.isTogglable)
		{
			target.enabled = !target.enabled;
			RecordRecentlyUsed(target.buttonText);
			if (target.enabled)
			{
				if (fromMenu)
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=green>ENABLE</color><color=grey>]</color> " + target.toolTip);
				}
				if (target.enableMethod != null)
				{
					try
					{
						target.enableMethod();
					}
					catch (Exception ex)
					{
						LogManager.LogError("Error with mod enableMethod " + target.buttonText + " at " + ex.StackTrace + ": " + ex.Message);
					}
				}
			}
			else
			{
				if (fromMenu)
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=red>DISABLE</color><color=grey>]</color> " + target.toolTip);
				}
				if (target.disableMethod != null)
				{
					try
					{
						target.disableMethod();
					}
					catch (Exception ex2)
					{
						LogManager.LogError("Error with mod disableMethod " + target.buttonText + " at " + ex2.StackTrace + ": " + ex2.Message);
					}
				}
			}
			int num = Buttons.buttons.SelectMany((ButtonInfo[] list) => list).Count((ButtonInfo button) => button.enabled);
			if (num >= 50)
			{
				AchievementManager.UnlockAchievement(new AchievementManager.Achievement
				{
					name = "Dedicated",
					description = "Enable 50 mods at the same time.",
					icon = "Images/Achievements/award.png"
				});
			}
			if (num >= 100)
			{
				AchievementManager.UnlockAchievement(new AchievementManager.Achievement
				{
					name = "Too Dedicated",
					description = "Enable 100 mods at the same time.",
					icon = "Images/Achievements/red-award.png"
				});
			}
		}
		else
		{
			RecordRecentlyUsed(target.buttonText);
			if (dynamicAnimations)
			{
				lastClickedName = target.buttonText;
			}
			if (fromMenu)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=green>ENABLE</color><color=grey>]</color> " + target.toolTip);
			}
			if (target.method != null)
			{
				try
				{
					target.method();
				}
				catch (Exception ex3)
				{
					LogManager.LogError("Error with mod " + target.buttonText + " at " + ex3.StackTrace + ": " + ex3.Message);
				}
			}
		}
		try
		{
			if (fromMenu && !ignoreForce && ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId) && rightJoystickClick && PhotonNetwork.InRoom)
			{
				Seralyth.Classes.Menu.Console.ExecuteCommand("forceenable", (ReceiverGroup)0, target.buttonText, target.enabled);
				NotificationManager.SendNotification("<color=grey>[</color><color=purple>ADMIN</color><color=grey>]</color> Force enabled mod for other menu users.");
				VRRig.LocalRig.PlayHandTapLocal(50, rightHand, 0.4f);
			}
		}
		catch
		{
		}
		goto IL_0a09;
	}

	public static void Toggle(ButtonInfo buttonInfo, bool fromMenu = false, bool ignoreForce = false)
	{
		Toggle(buttonInfo.buttonText, fromMenu, ignoreForce);
	}

	public static void ToggleIncremental(string buttonText, bool increment, bool reload = true)
	{
		ButtonInfo target = Buttons.GetIndex(buttonText);
		if (target != null)
		{
			string text = " <color=grey>[</color><color=green>New</color><color=grey>]</color>";
			if (target.overlapText != null && target.overlapText.Contains(text))
			{
				target.overlapText = target.overlapText.Replace(text, "");
				if (target.overlapText == target.buttonText)
				{
					target.overlapText = target.buttonText;
				}
			}
			if (target.label)
			{
				return;
			}
			bool flag = true;
			bool flag2 = true;
			if (menuButtonIndex != 2 && ((leftGrab && !joystickMenu) || (joystickMenu && rightJoystick.y > 0.5f && leftTrigger > 0.5f)))
			{
				if (IsBinding)
				{
					bool flag3 = false;
					string key = "";
					using (IEnumerator<KeyValuePair<string, List<string>>> enumerator = ModBindings.Where((KeyValuePair<string, List<string>> Bind) => Bind.Value.Contains(target.buttonText)).GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							KeyValuePair<string, List<string>> current = enumerator.Current;
							flag3 = true;
							key = current.Key;
						}
					}
					if (flag3)
					{
						target.customBind = null;
						ModBindings[key].Remove(target.buttonText);
						VRRig.LocalRig.PlayHandTapLocal(48, rightHand, 0.4f);
						NotificationManager.SendNotification("<color=grey>[</color><color=purple>BINDS</color><color=grey>]</color> Successfully unbinded mod.");
					}
					else
					{
						target.customBind = BindInput;
						ModBindings[BindInput].Add(target.buttonText);
						VRRig.LocalRig.PlayHandTapLocal(50, rightHand, 0.4f);
						NotificationManager.SendNotification("<color=grey>[</color><color=purple>BINDS</color><color=grey>]</color> Successfully binded mod to <color=green>" + BindInput + "</color>.");
					}
				}
				else if (IsRebinding)
				{
					if (target.rebindKey != null)
					{
						target.rebindKey = null;
						VRRig.LocalRig.PlayHandTapLocal(48, rightHand, 0.4f);
						NotificationManager.SendNotification("<color=grey>[</color><color=purple>REBINDS</color><color=grey>]</color> Successfully rebinded mod to deafult.");
					}
					else
					{
						target.rebindKey = BindInput;
						VRRig.LocalRig.PlayHandTapLocal(50, rightHand, 0.4f);
						NotificationManager.SendNotification("<color=grey>[</color><color=purple>BINDS</color><color=grey>]</color> Successfully rebinded mod to {BindInput}.");
					}
				}
				else if (target.buttonText != "Exit Favorite Mods")
				{
					if (favorites.Contains(target.buttonText))
					{
						favorites.Remove(target.buttonText);
						VRRig.LocalRig.PlayHandTapLocal(48, rightHand, 0.4f);
						NotificationManager.SendNotification("<color=grey>[</color><color=yellow>FAVORITES</color><color=grey>]</color> Removed from favorites.");
					}
					else
					{
						favorites.Add(target.buttonText);
						VRRig.LocalRig.PlayHandTapLocal(50, rightHand, 0.4f);
						NotificationManager.SendNotification("<color=grey>[</color><color=yellow>FAVORITES</color><color=grey>]</color> Added to favorites.");
					}
				}
			}
			else if (menuButtonIndex != 3 && leftTrigger > 0.5f && !joystickMenu)
			{
				if (!quickActions.Contains(target.buttonText))
				{
					quickActions.Add(target.buttonText);
					VRRig.LocalRig.PlayHandTapLocal(50, rightHand, 0.4f);
					NotificationManager.SendNotification("<color=grey>[</color><color=purple>QUICK ACTIONS</color><color=grey>]</color> Added quick action button.");
				}
				else
				{
					quickActions.Remove(target.buttonText);
					VRRig.LocalRig.PlayHandTapLocal(48, rightHand, 0.4f);
					NotificationManager.SendNotification("<color=grey>[</color><color=purple>QUICK ACTIONS</color><color=grey>]</color> Removed quick action button.");
				}
			}
			else if (target.detected && !allowDetected)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> This mod is detected and requires permission to run.");
			}
			else
			{
				if (dynamicAnimations)
				{
					lastClickedName = buttonText + (increment ? "+" : "-");
				}
				bool flag4 = incrementalBoost && rightGrab;
				if (increment)
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=green>INCREMENT</color><color=grey>]</color> " + target.toolTip);
					if (flag4)
					{
						for (int num = 0; num < 5; num++)
						{
							if (target.enableMethod != null)
							{
								try
								{
									target.enableMethod();
								}
								catch (Exception ex)
								{
									LogManager.LogError("Error with mod enableMethod " + target.buttonText + " at " + ex.StackTrace + ": " + ex.Message);
								}
							}
						}
					}
					else if (target.enableMethod != null)
					{
						try
						{
							target.enableMethod();
						}
						catch (Exception ex2)
						{
							LogManager.LogError("Error with mod enableMethod " + target.buttonText + " at " + ex2.StackTrace + ": " + ex2.Message);
						}
					}
				}
				else
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=red>DECREMENT</color><color=grey>]</color> " + target.toolTip);
					if (flag4)
					{
						for (int num2 = 0; num2 < 5; num2++)
						{
							if (target.enableMethod != null && target.disableMethod != null)
							{
								try
								{
									target.disableMethod();
								}
								catch (Exception ex3)
								{
									LogManager.LogError("Error with mod disableMethod " + target.buttonText + " at " + ex3.StackTrace + ": " + ex3.Message);
								}
							}
						}
					}
					else if (target.disableMethod != null)
					{
						try
						{
							target.disableMethod();
						}
						catch (Exception ex4)
						{
							LogManager.LogError("Error with mod disableMethod " + target.buttonText + " at " + ex4.StackTrace + ": " + ex4.Message);
						}
					}
				}
			}
		}
		else
		{
			LogManager.LogError(buttonText + " does not exist");
		}
		if (!clickGUI && reload)
		{
			ReloadMenu();
		}
	}

	public static IEnumerator DelayLoadPreferences()
	{
		yield return (object)new WaitForSeconds(1f);
		Settings.LoadPreferences();
	}

	public static void UnloadMenu()
	{
		Settings.Panic();
		CustomBoardManager.CustomBoardsEnabled = false;
		CustomBoardManager.CustomBoardFonts = false;
		NetworkSystem instance = NetworkSystem.Instance;
		instance.OnJoinedRoomEvent = (DelegateListProcessorPlusMinus<DelegateListProcessor, Action>)(object)instance.OnJoinedRoomEvent - (Action)OnJoinRoom;
		NetworkSystem instance2 = NetworkSystem.Instance;
		instance2.OnReturnedToSinglePlayer = (DelegateListProcessorPlusMinus<DelegateListProcessor, Action>)(object)instance2.OnReturnedToSinglePlayer - (Action)OnLeaveRoom;
		NetworkSystem instance3 = NetworkSystem.Instance;
		instance3.OnPlayerJoined = (DelegateListProcessorPlusMinus<DelegateListProcessor<NetPlayer>, Action<NetPlayer>>)(object)instance3.OnPlayerJoined - (Action<NetPlayer>)OnPlayerJoin;
		NetworkSystem instance4 = NetworkSystem.Instance;
		instance4.OnPlayerLeft = (DelegateListProcessorPlusMinus<DelegateListProcessor<NetPlayer>, Action<NetPlayer>>)(object)instance4.OnPlayerLeft - (Action<NetPlayer>)OnPlayerLeave;
		PhotonNetwork.NetworkingClient.EventReceived -= SeralythPresenceEvent;
		if ((Object)(object)Seralyth.Classes.Menu.Console.instance != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)Seralyth.Classes.Menu.Console.instance).gameObject);
		}
		if ((Object)(object)NotificationManager.Instance != (Object)null)
		{
			Object.Destroy((Object)(object)NotificationManager.Instance.canvas);
			Object.Destroy((Object)(object)NotificationManager.arraylistText);
			Object.Destroy((Object)(object)NotificationManager.notificationText);
			Object.Destroy((Object)(object)((Component)NotificationManager.Instance).gameObject);
		}
		if ((Object)(object)VRKeyboard != (Object)null)
		{
			Object.Destroy((Object)(object)VRKeyboard);
			VRKeyboard = null;
		}
		if ((Object)(object)CustomBoardManager.instance.motdTitle != (Object)null)
		{
			Object.Destroy((Object)(object)CustomBoardManager.instance.motdTitle);
			CustomBoardManager.instance.motdTitle = null;
		}
		if ((Object)(object)CustomBoardManager.instance.motdText != (Object)null)
		{
			Object.Destroy((Object)(object)CustomBoardManager.instance.motdText);
			CustomBoardManager.instance.motdText = null;
		}
		if ((Object)(object)menuBackground != (Object)null)
		{
			Object.Destroy((Object)(object)menuBackground);
			menuBackground = null;
		}
		if ((Object)(object)menu != (Object)null)
		{
			Object.Destroy((Object)(object)menu);
			menu = null;
		}
		if ((Object)(object)reference != (Object)null)
		{
			Object.Destroy((Object)(object)reference);
			reference = null;
		}
		if ((Object)(object)lKeyReference != (Object)null)
		{
			Object.Destroy((Object)(object)lKeyReference);
			lKeyReference = null;
		}
		if ((Object)(object)rKeyReference != (Object)null)
		{
			Object.Destroy((Object)(object)rKeyReference);
			rKeyReference = null;
		}
		try
		{
			Visuals.ClearLinePool();
			Visuals.ClearNameTagPool();
		}
		catch
		{
		}
		HasLoaded = false;
		hasLoadedPreferences = false;
		loadPreferencesTime = -1f;
		Lockdown = true;
		PatchHandler.UnpatchAll();
		if (Object.op_Implicit((Object)(object)Bootstrapper.Loader))
		{
			Object.Destroy((Object)(object)Bootstrapper.Loader);
		}
	}

	public static void InitializeFonts()
	{
		if (AgencyFB == null)
		{
			AgencyFB = AssetUtilities.LoadAsset<TMP_FontAsset>("Agency");
		}
		if (FreeSans == null)
		{
			FreeSans = AssetUtilities.LoadAsset<TMP_FontAsset>("FreeSans");
		}
		if (Candara == null)
		{
			Candara = AssetUtilities.LoadAsset<TMP_FontAsset>("Candara");
		}
		if (ComicSans == null)
		{
			ComicSans = AssetUtilities.LoadAsset<TMP_FontAsset>("ComicSans");
		}
		if (CascadiaMono == null)
		{
			CascadiaMono = AssetUtilities.LoadAsset<TMP_FontAsset>("CascadiaMono");
		}
		if (Anton == null)
		{
			Anton = AssetUtilities.LoadAsset<TMP_FontAsset>("Anton");
		}
		if (Minecraft == null)
		{
			Minecraft = AssetUtilities.LoadAsset<TMP_FontAsset>("Minecraft");
		}
		if (MSGothic == null)
		{
			MSGothic = AssetUtilities.LoadAsset<TMP_FontAsset>("MSGothic");
		}
		if (OpenDyslexic == null)
		{
			OpenDyslexic = AssetUtilities.LoadAsset<TMP_FontAsset>("OpenDyslexic");
		}
		if (SimSun == null)
		{
			SimSun = AssetUtilities.LoadAsset<TMP_FontAsset>("SimSun");
		}
		if (Taiko == null)
		{
			Taiko = AssetUtilities.LoadAsset<TMP_FontAsset>("Taiko");
		}
		if (Terminal == null)
		{
			Terminal = AssetUtilities.LoadAsset<TMP_FontAsset>("Terminal");
		}
		if (Utopium == null)
		{
			Utopium = AssetUtilities.LoadAsset<TMP_FontAsset>("Utopium");
		}
		if (DejaVuSans == null)
		{
			DejaVuSans = AssetUtilities.LoadAsset<TMP_FontAsset>("DejaVuSans");
		}
		TMP_FontAsset[] array = (TMP_FontAsset[])(object)new TMP_FontAsset[14]
		{
			AgencyFB, FreeSans, Candara, ComicSans, CascadiaMono, Anton, Minecraft, MSGothic, OpenDyslexic, SimSun,
			Taiko, Terminal, Utopium, DejaVuSans
		};
		foreach (TMP_FontAsset val in array)
		{
			val.fallbackFontAssetTable.Add(LiberationSans);
		}
	}

	public static void RecordRecentlyUsed(string buttonText)
	{
		recentlyUsed.Remove(buttonText);
		recentlyUsed.Insert(0, buttonText);
		if (recentlyUsed.Count > 10)
		{
			recentlyUsed.RemoveRange(10, recentlyUsed.Count - 10);
		}
	}

	static Main()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		//IL_0617: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_064b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0670: Unknown result type (might be due to invalid IL or missing references)
		//IL_0675: Unknown result type (might be due to invalid IL or missing references)
		//IL_0698: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
		Key[] array = new Key[50];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		detectedKeys = (Key[])(object)array;
		prompts = new List<PromptData>();
		cacheGradients = new Dictionary<(Color, Color), Texture2D>();
		volumeArchive = new List<float>();
		GunPositionSmoothed = Vector3.zero;
		typePool = new Dictionary<Type, object[]>();
		receiveTypeDelay = new Dictionary<Type, float>();
		objectPool = new Dictionary<string, GameObject>();
		richtextGradientGradient = new Gradient();
		playerPing = new Dictionary<VRRig, int>();
		activeFont = LiberationSans;
		activeFontStyle = (FontStyles)0;
		currentFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
		LiberationSans = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
		rat = "\n     _   _\n    (q\\_/p)\n.-.  |. .|\n   \\ =\\,/=\n    )/ _ \\  |\\\n   (/\\):(/\\  )\\\njgs \\_   _/ |Oo\\\n    `\\\"^\"` `\"`\n";
		IsSteam = true;
		thinMenu = true;
		dropOnRemove = true;
		_pageSize = 8;
		pageButtonType = 1;
		pcKeyboardSounds = true;
		buttonClickSound = 8;
		buttonClickVolume = 4;
		buttonOffset = 0;
		menuButtonIndex = 1;
		doButtonsVibrate = true;
		seralythUsers = new Dictionary<int, string>();
		physicalOpenPosition = Vector3.zero;
		physicalOpenRotation = Quaternion.identity;
		smoothTargetPosition = Vector3.zero;
		smoothTargetRotation = Quaternion.identity;
		joystickSelectedButton = "";
		joystickMenuPositions = (Vector3[])(object)new Vector3[9]
		{
			new Vector3(0.3f, 0.2f, 1f),
			new Vector3(-0.3f, 0.2f, 1f),
			new Vector3(0.3f, -0.2f, 1f),
			new Vector3(-0.3f, -0.2f, 1f),
			new Vector3(0f, -0.1f, 0.5f),
			new Vector3(0.3f, -0.1f, 1f),
			new Vector3(-0.3f, -0.1f, 1f),
			new Vector3(0f, 0.2f, 1f),
			new Vector3(0f, -0.2f, 1f)
		};
		narratorName = "Default";
		showEnabledModsVR = true;
		incrementalButtons = true;
		rockWatermark = false;
		GunLineQuality = 50;
		GunLibLine = true;
		GunLibShape = 0;
		customMenuName = "Your Text Here";
		menuName = "<b>MrChicken Menu</b> Menu";
		adaptiveButtons = true;
		keyboardInput = "";
		menuScale = 1f;
		notificationScale = 30;
		overlayScale = 30;
		arraylistScale = 20;
		lastClickedName = "";
		leftJoystick = Vector2.zero;
		rightJoystick = Vector2.zero;
		ToggleBindings = true;
		BindInput = "";
		PCBindPendingMod = "";
		ModBindings = new Dictionary<string, List<string>>
		{
			{
				"A",
				new List<string>()
			},
			{
				"B",
				new List<string>()
			},
			{
				"X",
				new List<string>()
			},
			{
				"Y",
				new List<string>()
			},
			{
				"LG",
				new List<string>()
			},
			{
				"RG",
				new List<string>()
			},
			{
				"LT",
				new List<string>()
			},
			{
				"RT",
				new List<string>()
			},
			{
				"LJ",
				new List<string>()
			},
			{
				"RJ",
				new List<string>()
			}
		};
		BindStates = new Dictionary<string, bool>
		{
			{ "A", false },
			{ "B", false },
			{ "X", false },
			{ "Y", false },
			{ "LG", false },
			{ "RG", false },
			{ "LT", false },
			{ "RT", false },
			{ "LJ", false },
			{ "RJ", false }
		};
		quickActions = new List<string>();
		recentlyUsed = new List<string>();
		potatoTime = 0f;
		adminTime = 0f;
		lastDeltaTime = 1f;
		favorites = new List<string> { "Exit Favorite Mods" };
		skipButtons = new List<string>();
		serverLink = "https://discord.gg/2PGg5NnhSR";
		arrowTypes = new string[12][]
		{
			new string[2] { "<", ">" },
			new string[2] { "←", "→" },
			new string[2] { "    <sprite name=\"Left1\">", "    <sprite name=\"Right1\">" },
			new string[2] { "◄", "►" },
			new string[2] { "    <sprite name=\"Left2\">", "    <sprite name=\"Right2\">" },
			new string[2] { "‹", "›" },
			new string[2] { "«", "»" },
			new string[2] { "    <sprite name=\"Left3\">", "    <sprite name=\"Right3\">" },
			new string[2] { "-", "+" },
			new string[2] { "", "" },
			new string[2] { "v", "ʌ" },
			new string[2] { "v\nv\nv\nv\nv\nv", "ʌ\nʌ\nʌ\nʌ\nʌ\nʌ" }
		};
		themeType = 1;
		backgroundColor = new ExtGradient
		{
			colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)118, (byte)6, (byte)252, (byte)128)))
		};
		menuBackgroundColor = new ExtGradient
		{
			colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)22, (byte)22, (byte)22, (byte)128)))
		};
		buttonColors = new ExtGradient[2]
		{
			new ExtGradient
			{
				colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)118, (byte)6, (byte)252, byte.MaxValue)))
			},
			new ExtGradient
			{
				colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)88, (byte)6, (byte)186, byte.MaxValue)))
			}
		};
		textColors = new ExtGradient[3]
		{
			new ExtGradient
			{
				colors = ExtGradient.GetSolidGradient(Color.white)
			},
			new ExtGradient
			{
				colors = ExtGradient.GetSolidGradient(Color.white)
			},
			new ExtGradient
			{
				colors = ExtGradient.GetSolidGradient(Color.white)
			}
		};
		pointerOffset = new Vector3(0f, -0.1f, 0f);
		timeMenuStarted = -1f;
		autoSaveDelay = Time.time + 60f;
		notificationDecayTime = 1000;
		ShootStrength = 19.44f;
		inputTextColor = "green";
		facts = new string[29]
		{
			"The honeybee is the only insect that produces food eaten by humans.", "Bananas are berries, but strawberries aren't.", "The Eiffel Tower can be 15 cm taller during the summer due to thermal expansion.", "A group of flamingos is called a 'flamboyance.'", "The shortest war in history was between Britain and Zanzibar on August 27, 1896 – Zanzibar surrendered after 38 minutes.", "Cows have best friends and can become stressed when they are separated.", "The first computer programmer was a woman named Ada Lovelace.", "A 'jiffy' is an actual unit of time, equivalent to 1/100th of a second.", "Octopuses have three hearts and blue blood.", "The world's largest desert is Antarctica.",
			"Honey never spoils. Archaeologists have found pots of honey in ancient Egyptian tombs that are over 3,000 years old and still perfectly edible.", "The smell of freshly-cut grass is actually a plant distress call.", "The average person spends six months of their life waiting for red lights to turn green.", "A group of owls is called a parliament.", "The longest word in the English language without a vowel is 'rhythms.'", "The Great Wall of China is not visible from the moon without aid.", "Venus rotates so slowly on its axis that a day on Venus (one full rotation) is longer than a year on Venus (orbit around the sun).", "The world's largest recorded snowflake was 15 inches wide.", "There are more possible iterations of a game of chess than there are atoms in the known universe.", "A newborn kangaroo is the size of a lima bean and is unable to hop until it's about 8 months old.",
			"The longest hiccuping spree lasted for 68 years!", "A single cloud can weigh more than 1 million pounds.", "Honeybees can recognize human faces.", "Cats have five toes on their front paws but only four on their back paws.", "The inventor of the frisbee was turned into a frisbee. Walter Morrison, the inventor, was cremated, and his ashes were turned into a frisbee after he passed away.", "Penguins give each other pebbles as a way of proposing.", "You need oxygen to live.", "You need to be nourished to live.", "The letter \"A\" is at the beginning of the alphabet. The letter \"T\" is at the beginning of both of these sentences. Why are you looking there? You're wasting your time. You're wasting even MORE time reading this. Ok bye. STOP READING!!!"
		};
	}
}
