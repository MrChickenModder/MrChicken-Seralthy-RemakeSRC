using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ExitGames.Client.Photon;
using GameObjectScheduling;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag.Rendering;
using GorillaTagScripts;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using Seralyth.Classes.Menu;
using Seralyth.Classes.Mods;
using Seralyth.Extensions;
using Seralyth.Managers;
using Seralyth.Menu;
using Seralyth.Patches.Menu;
using Seralyth.Utilities;
using TMPro;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.UI;

namespace Seralyth.Mods;

public class Visuals
{
	public struct WatchInfo
	{
		public GameObject gameObject;

		public TextMeshProUGUI text;

		public GameObject shell;

		public Image indicator;
	}

	public class SkinnedWireframeRenderer : MonoBehaviour
	{
		public SkinnedMeshRenderer skinnedMeshRenderer;

		public Mesh lineMesh;

		public MeshFilter meshFilter;

		public MeshRenderer meshRenderer;

		public GameObject wireframeObj;

		public Color Color
		{
			get
			{
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				return ((Renderer)meshRenderer).material.color;
			}
			set
			{
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				((Renderer)meshRenderer).material.color = value;
			}
		}

		private void Awake()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Expected O, but got Unknown
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Expected O, but got Unknown
			skinnedMeshRenderer = ((Component)this).GetComponent<SkinnedMeshRenderer>();
			wireframeObj = new GameObject("Wireframe");
			wireframeObj.transform.SetParent(((Component)this).transform, false);
			meshFilter = wireframeObj.AddComponent<MeshFilter>();
			meshRenderer = wireframeObj.AddComponent<MeshRenderer>();
			((Renderer)meshRenderer).material = new Material(Shader.Find("GUI/Text Shader"))
			{
				color = Color.green
			};
			lineMesh = new Mesh();
			meshFilter.mesh = lineMesh;
		}

		private void Update()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			if (Time.frameCount % 3 > 0)
			{
				return;
			}
			Mesh val = new Mesh();
			skinnedMeshRenderer.BakeMesh(val);
			Vector3[] vertices = val.vertices;
			int[] triangles = val.triangles;
			List<Vector3> list = new List<Vector3>();
			List<int> list2 = new List<int>();
			for (int i = 0; i < triangles.Length; i += 3)
			{
				int num = triangles[i];
				int num2 = triangles[i + 1];
				int num3 = triangles[i + 2];
				list.Add(vertices[num]);
				list.Add(vertices[num2]);
				list.Add(vertices[num2]);
				list.Add(vertices[num3]);
				list.Add(vertices[num3]);
				list.Add(vertices[num]);
				int num4 = list.Count - 6;
				for (int j = 0; j < 6; j++)
				{
					list2.Add(num4 + j);
				}
			}
			lineMesh.Clear();
			lineMesh.SetVertices(list);
			lineMesh.SetIndices(list2.ToArray(), (MeshTopology)3, 0);
		}

		private void OnDestroy()
		{
			if ((Object)(object)lineMesh != (Object)null)
			{
				Object.Destroy((Object)(object)lineMesh);
				lineMesh = null;
			}
			if ((Object)(object)wireframeObj != (Object)null)
			{
				Object.Destroy((Object)(object)wireframeObj);
				wireframeObj = null;
			}
			if ((Object)(object)meshRenderer != (Object)null && (Object)(object)((Renderer)meshRenderer).material != (Object)null)
			{
				Object.Destroy((Object)(object)((Renderer)meshRenderer).material);
				((Renderer)meshRenderer).material = null;
			}
		}
	}

	public static readonly Dictionary<(long, float), GameObject> auraPool = new Dictionary<(long, float), GameObject>();

	public static readonly Dictionary<long, GameObject> cubePool = new Dictionary<long, GameObject>();

	public static readonly Dictionary<long, GameObject> cylinderPool = new Dictionary<long, GameObject>();

	private static readonly Dictionary<TimeOfDayDependentAudio, float> ambientObjects = new Dictionary<TimeOfDayDependentAudio, float>();

	private static bool previousFullbrightStatus;

	private static float removeBlindfoldDelay;

	private static TMP_SpriteAsset _infoSpriteAsset;

	public static List<WatchInfo> Watches = new List<WatchInfo>();

	public static bool infoWatchMenuName;

	public static bool infoWatchTime;

	public static bool infoWatchClip;

	public static bool infoWatchFPS;

	public static bool infoWatchCode;

	public static Material oldSkyMat;

	public static TrailRenderer trailRenderer;

	private static Material tapMat;

	private static Texture2D tapTxt;

	private static Texture2D warningTxt;

	public static List<object[]> handTaps = new List<object[]>();

	public static bool PerformanceVisuals;

	public static float PerformanceModeStep = 0.2f;

	public static int PerformanceModeStepIndex = 2;

	public static float PerformanceVisualDelay;

	public static int DelayChangeStep;

	public static readonly Dictionary<string, GameObject> labelDictionary = new Dictionary<string, GameObject>();

	public static readonly Dictionary<bool, List<int>> labelDistances = new Dictionary<bool, List<int>>();

	public static string OverallPlaytime;

	private static float playtime;

	private static float startTime;

	private static float endTime;

	private static bool lastWasTagged;

	private static GameObject visualizerObject;

	private static GameObject visualizerOutline;

	private static GameObject headPos;

	private static GameObject leftHandPos;

	private static GameObject rightHandPos;

	private static readonly Dictionary<VRRig, LineRenderer> predictions = new Dictionary<VRRig, LineRenderer>();

	private static readonly Dictionary<VRRig, GameObject> hitboxESP = new Dictionary<VRRig, GameObject>();

	public static readonly Dictionary<SlingshotProjectile, LineRenderer> trajectoryPool = new Dictionary<SlingshotProjectile, LineRenderer>();

	public static LineRenderer localTrajectoryLine;

	private static readonly Dictionary<VRRig, List<int>> ntDistanceList = new Dictionary<VRRig, List<int>>();

	private static int optimizeChangeStep;

	private static float optimizeDelay;

	private static readonly Dictionary<VRRig, GameObject> nametags = new Dictionary<VRRig, GameObject>();

	public static bool nameTagChams;

	public static bool anchorNameTag;

	public static bool selfNameTag;

	private static readonly Dictionary<VRRig, GameObject> velnametags = new Dictionary<VRRig, GameObject>();

	private static readonly Dictionary<VRRig, GameObject> fpsNametags = new Dictionary<VRRig, GameObject>();

	private static readonly Dictionary<VRRig, GameObject> idNameTags = new Dictionary<VRRig, GameObject>();

	private static readonly Dictionary<VRRig, GameObject> platformTags = new Dictionary<VRRig, GameObject>();

	private static readonly Dictionary<VRRig, GameObject> kidNameTags = new Dictionary<VRRig, GameObject>();

	private static readonly Dictionary<VRRig, GameObject> subNameTags = new Dictionary<VRRig, GameObject>();

	private static readonly Dictionary<VRRig, GameObject> creationDateTags = new Dictionary<VRRig, GameObject>();

	private static readonly Dictionary<VRRig, GameObject> pingNameTags = new Dictionary<VRRig, GameObject>();

	private static readonly Dictionary<VRRig, GameObject> turnNameTags = new Dictionary<VRRig, GameObject>();

	private static readonly Dictionary<VRRig, GameObject> taggedNameTags = new Dictionary<VRRig, GameObject>();

	public static readonly Dictionary<string, string> modDictionary = new Dictionary<string, string>
	{
		{ "genesis", "Genesis" },
		{ "HP_Left", "Holdable Pad" },
		{ "GrateVersion", "Grate" },
		{ "void", "Void" },
		{ "BANANAOS", "Banana OS" },
		{ "GC", "Gorilla Craft" },
		{ "CarName", "Gorilla Vehicles" },
		{ "6p72ly3j85pau2g9mda6ib8px", "CCM V2" },
		{ "FPS-Nametags for Zlothy", "FPS Tags" },
		{ "cronos", "Cronos" },
		{ "ORBIT", "Orbit" },
		{ "Violet On Top", "Violet" },
		{ "MP25", "Monke Phone" },
		{ "GorillaWatch", "Gorilla Watch" },
		{ "InfoWatch", "Gorilla Info Watch" },
		{ "BananaPhone", "Banana Phone" },
		{ "Vivid", "Vivid" },
		{ "RGBA", "Custom Cosmetics" },
		{ "cheese is gouda", "Whos Icheating" },
		{ "shirtversion", "Gorilla Shirts" },
		{ "gpronouns", "Gorilla Pronouns" },
		{ "gfaces", "Gorilla Faces" },
		{ "monkephone", "Monke Phone" },
		{ "pmversion", "Player Models" },
		{ "gtrials", "Gorilla Trials" },
		{ "msp", "Monke Smartphone" },
		{ "gorillastats", "Gorilla Stats" },
		{ "using gorilladrift", "Gorilla Drift" },
		{ "monkehavocversion", "Monke Havoc" },
		{ "tictactoe", "Tic Tac Toe" },
		{ "ccolor", "Index" },
		{ "imposter", "Gorilla Among Us" },
		{ "spectapeversion", "Spec Tape" },
		{ "cats", "Cats" },
		{ "made by biotest05 :3", "Dogs" },
		{ "fys cool magic mod", "Fys Magic Mod" },
		{ "colour", "Custom Cosmetics" },
		{ "chainedtogether", "Chained Together" },
		{ "goofywalkversion", "Goofy Walk" },
		{ "void_menu_open", "Void" },
		{ "violetpaiduser", "Violet Paid" },
		{ "violetfree", "Violet Free" },
		{ "obsidianmc", "Obsidian.Lol" },
		{ "dark", "Shiba GT Dark" },
		{ "hidden menu", "Hidden" },
		{ "oblivionuser", "Oblivion" },
		{ "hgrehngio889584739_hugb\n", "Resurgence" },
		{ "eyerock reborn", "Eye Rock" },
		{ "asteroidlite", "Asteroid Lite" },
		{ "elux", "Elux" },
		{ "cokecosmetics", "Coke Cosmetx" },
		{ "GFaces", "G Faces" },
		{ "github.com/maroon-shadow/SimpleBoards", "Simple Boards" },
		{ "ObsidianMC", "Obsidian" },
		{ "hgrehngio889584739_hugb", "Resurgence" },
		{ "GTrials", "G Trials" },
		{ "github.com/ZlothY29IQ/GorillaMediaDisplay", "Gorilla Media Display" },
		{ "github.com/ZlothY29IQ/TooMuchInfo", "Too Much Info" },
		{ "github.com/ZlothY29IQ/RoomUtils-IW", "Room Utils IW" },
		{ "github.com/ZlothY29IQ/MonkeClick", "Monke Click" },
		{ "github.com/ZlothY29IQ/MonkeClick-CI", "Monke Click CI" },
		{ "github.com/ZlothY29IQ/MonkeRealism", "Monke Realism" },
		{ "MediaPad", "Media Pad" },
		{ "GorillaCinema", "Gorilla Cinema" },
		{ "ChainedTogetherActive", "Chained Together" },
		{ "GPronouns", "G Pronouns" },
		{ "CSVersion", "Custom Skin" },
		{ "github.com/ZlothY29IQ/Zloth-RecRoomRig", "Zloth Rec Room Rig" },
		{ "ShirtProperties", "Shirts Old" },
		{ "GorillaShirts", "Shirts" },
		{ "GS", "Old Shirts" },
		{ "6XpyykmrCthKhFeUfkYGxv7xnXpoe2", "CCM V2" },
		{ "Body Tracking", "Body Track Old" },
		{ "Body Estimation", "Han Body Est" },
		{ "Gorilla Track", "Body Track" },
		{ "CustomMaterial", "Custom Cosmetics" },
		{ "I like cheese", "Rec Room Rig" },
		{ "silliness", "Silliness" },
		{ "EmoteWheel", "Fortnite Emote Wheel" },
		{ "untitled", "Untitled" },
		{ "BoyDoILoveInformation Public", "BoyDoILoveInformation" },
		{ "DTAOI", "DTAOI" },
		{ "GorillaShop", "GorillaShop" },
		{ "Fusioned", "Fusioned" },
		{ "y u lookin in here weirdo", "Malachi Menu Reborn" },
		{ "ØƦƁƖƬ", "Orbit" },
		{ "Atlas", "Atlas" }
	};

	private static readonly Dictionary<VRRig, GameObject> modNameTags = new Dictionary<VRRig, GameObject>();

	public static readonly Dictionary<string, string> specialCosmetics = new Dictionary<string, string>
	{
		{ "LBAAD.", "Administrator" },
		{ "LBAAK.", "Forest Guide" },
		{ "LBADE.", "Finger Painter" },
		{ "LBAGS.", "Illustrator" },
		{ "LMAPY.", "Forest Guide" },
		{ "LBANI.", "AA Creator" }
	};

	private static readonly Dictionary<VRRig, GameObject> cosmeticNameTags = new Dictionary<VRRig, GameObject>();

	public static readonly Dictionary<string, string> verifiedDictionary = new Dictionary<string, string>
	{
		{ "9DBC90CF7449EF64", "StyledSnail" },
		{ "33FFCE29A8DB5BB", "Jmancurly?" },
		{ "6FE5FF4D5DF68843", "Pine" },
		{ "52529F0635BE0CDF", "PapaSmurf" },
		{ "BAC5807405123060", "britishmonke" },
		{ "A6FFC7318E1301AF", "jmancurly" },
		{ "3B9FD2EEF24ACB3", "VMT" },
		{ "33FFA45DBFD33B01", "will" },
		{ "D6971CA01F82A975", "Elliot" },
		{ "7FB16B1EDEB71A4C", "Elliot" },
		{ "636D8846E76C9B5A", "Clown" },
		{ "65CB0CCF1AED2BF", "Ethyb" },
		{ "48437FE432DE48BE", "Rose" },
		{ "61AD990FF3A423B7", "Boda 1" },
		{ "AAB44BFD0BA34829", "Boda 2" },
		{ "6713DA80D2E9BFB5", "AHauntedArmy" },
		{ "B4A3FF01312B55B1", "Pluto" },
		{ "339E0D392565DC39", "kishark" },
		{ "F08CE3118F9E793E", "TurboAlligator" },
		{ "5380BEF3DA4A857D", "Tuxedo" },
		{ "D6E20BE9655C798", "TTTPIG 1" },
		{ "71AA09D13C0F408D", "TTTPIG 2" },
		{ "1D6E20BE9655C798", "TTTPIG 3" },
		{ "22A7BCEFFD7A0BBA", "TTTPIG 4" },
		{ "C3878B068886F6C3", "ZZEN" },
		{ "6F79BE7CB34642AC", "CodyO'Quinn" },
		{ "5AA1231973BE8A62", "Apollo" },
		{ "7F31BEEC604AE189", "ElectronicWall 1" },
		{ "42C809327652ECDD", "ElectronicWall 2" },
		{ "ECDE8A2FF8510934", "Antoca" },
		{ "80279945E7D3B57D", "Jolyne" },
		{ "7E44E8337DF02CC1", "Nunya" },
		{ "DE601BC40DB68CE0", "Graic" },
		{ "F5B5C64914C13B83", "HatGirl" },
		{ "660814E013F31EFA", "HOLLOWZZGT" },
		{ "2E408ED946D55D51", "Haunted" },
		{ "D345FE394607F946", "Bzzz the 18th" },
		{ "498D4C2F23853B37", "POGTROLL" },
		{ "BC9764E1EADF8BE0", "Circuit" },
		{ "D0CB396539676DD8", "FrogIlla" },
		{ "A1A99D33645E4A94", "STEAMVRAVTS / YEAT" },
		{ "CBCCBBB6C28A94CF", "PTMstar" },
		{ "6DC06EEFFE9DBD39", "Lucio" },
		{ "4ACA3C76B334B17F", "Wihz" },
		{ "571776944B6162F1", "CubCub" },
		{ "FB5FCEBC4A0E0387", "PepsiDee" },
		{ "8ED59EACCDC6BA86", "PepsiDee?" },
		{ "645222265FB972B", "Chaotic Asriel" },
		{ "BC99FA914F506AB8", "Lemming 1" },
		{ "3A16560CA65A51DE", "Lemming 2" },
		{ "59F3FE769DE93AB9", "Lemming 3" },
		{ "EE9FB127CF7DBBD5", "NOTMARK" },
		{ "54DCB69545BE0800", "Biffbish" },
		{ "A04005517920EB0", "K9" },
		{ "ABD60175B46E45C5", "Saltwater" },
		{ "964C4A68F65A804C", "YottaBite" },
		{ "8FECBBC89D69575E", "KyleTheScientist" },
		{ "4D5EB238C8253D04", "Person" },
		{ "8B047CEF4F695F3A", "AlecVR" },
		{ "E5883BD27F60F99A", "AlecVR?" },
		{ "70EEBA9507E8381E", "H4KPY?" },
		{ "911691C9FEB63D9F", "H4KPY?" },
		{ "D322FC7F6A9875DB", "DecalFree" },
		{ "FBE1690495D63A05", "Azora" },
		{ "3509A9A428FCD55C", "Polar" },
		{ "3F179DCC75FECA1", "Polar" },
		{ "450EE31CA7FBDE4C", "ProximusVR" },
		{ "180E486699D14963", "Legion" },
		{ "AC67B4E838EFB5D3", "PartyMonkeyGT" },
		{ "4BB02313F55AA741", "Mosa?" },
		{ "E61FD8B23F3264C0", "Authority" },
		{ "36B456067A5E1453", "Lofiat" },
		{ "FC8CB7FED6EFDC81", "CJVR" },
		{ "6C85D07DC2586DC9", "Arctrie" },
		{ "22CDF30B107A9BDB", "Durag" },
		{ "1940CCB76316556F", "Genet1c" },
		{ "6B4FB3EF97A8BB71", "Crisp" },
		{ "17BCC7B56F88287A", "Vortex" },
		{ "2D35DBED9A3BE6A0", "Tortise" },
		{ "2D7D32651E93866", "Graze" },
		{ "B4E45E48C5CE0656", "ZBR" },
		{ "F7EE771EB6794ABE", "OfficialLemon" },
		{ "36FD11C9FB61E50B", "Cryptik" },
		{ "28579AFACDE1FB19", "Pepsi Dee" },
		{ "8A062E735BBC89ED", "GLTCH" },
		{ "A100E9E6C4D91E75", "Mycrafts 1" },
		{ "7952F9E08FEF8E83", "Mycrafts 2" },
		{ "10E12F25533C13F2", "Kirpi4" },
		{ "10621E029A675705", "AA_Mike" },
		{ "F8FF7B812B0B2F72", "Foggy" },
		{ "1E8298E1E1F40CB2", "Faaduu" },
		{ "289C8FAD58A09D6D", "Pixel" },
		{ "172E4982BEE4A8AD", "H4KPY" },
		{ "A339740A8ED97FC2", "Coffeeperson" },
		{ "502575B001FE6FCD", "Mikeyourman" },
		{ "7DC729B66A15F9DE", "TrumpGT" }
	};

	private static readonly Dictionary<VRRig, GameObject> verifiedNameTags = new Dictionary<VRRig, GameObject>();

	private static readonly Dictionary<VRRig, GameObject> crashedNameTags = new Dictionary<VRRig, GameObject>();

	private static readonly Dictionary<VRRig, GameObject> compactNameTags = new Dictionary<VRRig, GameObject>();

	private static readonly Dictionary<VRRig, GameObject> compactTagBackgrounds = new Dictionary<VRRig, GameObject>();

	private static readonly Dictionary<VRRig, GameObject> minecraftNameTags = new Dictionary<VRRig, GameObject>();

	private static readonly Dictionary<VRRig, GameObject> minecraftTagBackgrounds = new Dictionary<VRRig, GameObject>();

	private static readonly Dictionary<VRRig, GameObject> castingNameTags = new Dictionary<VRRig, GameObject>();

	public static string _leavesName;

	public static readonly List<GameObject> leaves = new List<GameObject>();

	public static readonly List<GameObject> cosmetics = new List<GameObject>();

	public static readonly List<Renderer> disabledRenderers = new List<Renderer>();

	public static readonly Dictionary<VRRig, Coroutine> rigLerpCoroutines = new Dictionary<VRRig, Coroutine>();

	private static readonly Dictionary<VRRig, GameObject> cosmeticIndicators = new Dictionary<VRRig, GameObject>();

	private static readonly Dictionary<string, Texture2D> cosmeticTextures = new Dictionary<string, Texture2D>();

	private static Material cosmeticMat;

	private static Material platformMat;

	private static Material platformEspMat;

	private static readonly Dictionary<VRRig, GameObject> platformIndicators = new Dictionary<VRRig, GameObject>();

	private static Material voiceMat;

	private static Material voiceEspMat;

	private static Texture2D voiceTxt;

	private static readonly Dictionary<VRRig, GameObject> voiceIndicators = new Dictionary<VRRig, GameObject>();

	private static GameObject l;

	private static GameObject r;

	private static readonly Dictionary<VRRig, List<LineRenderer>> boneESP = new Dictionary<VRRig, List<LineRenderer>>();

	public static readonly int[] bones = new int[38]
	{
		4, 3, 5, 4, 19, 18, 20, 19, 3, 18,
		21, 20, 22, 21, 25, 21, 29, 21, 31, 29,
		27, 25, 24, 22, 6, 5, 7, 6, 10, 6,
		14, 6, 16, 14, 12, 10, 9, 7
	};

	private static readonly Dictionary<VRRig, SkinnedWireframeRenderer> wireframes = new Dictionary<VRRig, SkinnedWireframeRenderer>();

	private static readonly List<VRRig> convertedRigs = new List<VRRig>();

	public static Shader uberChams;

	private static readonly Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();

	private static readonly Dictionary<VRRig, GameObject> boxESP = new Dictionary<VRRig, GameObject>();

	private static readonly Dictionary<VRRig, GameObject> hollowBoxESP = new Dictionary<VRRig, GameObject>();

	private static readonly Dictionary<VRRig, TrailRenderer> breadcrumbs = new Dictionary<VRRig, TrailRenderer>();

	private static GameObject LeftSphere;

	private static GameObject RightSphere;

	private static readonly List<TextMeshPro> nameTagPool = new List<TextMeshPro>();

	private static GameObject nameTagHolder;

	public static bool isNameTagQueued;

	private static readonly List<LineRenderer> linePool = new List<LineRenderer>();

	private static GameObject lineRenderHolder;

	public static bool isLineRenderQueued = false;

	private static bool wasResetPressed;

	private static bool modEnabled;

	private static float lastEspScan;

	private static VRRig[] cachedEspRigs;

	private static float lastCosmeticScan;

	private static int cachedWearingCount;

	private static TextMesh cachedDisplay;

	private static GameObject auraCircleObj;

	private static LineRenderer circleRenderer;

	private static GameObject tracerObj;

	private static LineRenderer tracerRenderer;

	private static List<GameObject> activeVisuals = new List<GameObject>();

	public static bool InfoPlayerModEnabled;

	private static string typewriterFullText;

	private static int typewriterIndex;

	private static float typewriterTimer;

	private static float typewriterSpeed = 30f;

	private static string typewriterTargetId;

	private static readonly Dictionary<VRRig, GameObject> ballESP = new Dictionary<VRRig, GameObject>();

	private static readonly Dictionary<VRRig, GameObject> gripIndicators = new Dictionary<VRRig, GameObject>();

	private static Material gripEspMat;

	private static Texture2D gripTxt;

	public static TMP_SpriteAsset InfoSprites
	{
		get
		{
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Expected O, but got Unknown
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Expected O, but got Unknown
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Expected O, but got Unknown
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Expected O, but got Unknown
			List<Texture2D> textureList;
			List<(string name, int index)> spriteDataList;
			if ((Object)(object)_infoSpriteAsset == (Object)null)
			{
				_infoSpriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
				((Object)_infoSpriteAsset).name = "Seralyth_InfoSprites";
				textureList = new List<Texture2D>();
				spriteDataList = new List<(string, int)>();
				AddSprite("Steam", AssetUtilities.LoadTextureFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Images/Mods/Visuals/steam.png", "Images/Mods/Visuals/steam.png"));
				AddSprite("Standalone", AssetUtilities.LoadTextureFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Images/Mods/Visuals/oculus.png", "Images/Mods/Visuals/oculus.png"));
				AddSprite("PC", AssetUtilities.LoadTextureFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Images/Mods/Visuals/pc.png", "Images/Mods/Visuals/pc.png"));
				for (int i = 1; i <= 5; i++)
				{
					AddSprite($"Ping{i}", AssetUtilities.LoadTextureFromURL(string.Format("{0}/Images/Mods/Visuals/ping{1}.png", "https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server", i), $"Images/Mods/Visuals/ping{i}.png"));
				}
				int num = 512;
				Texture2D val = new Texture2D(num, num);
				Rect[] array = val.PackTextures(textureList.ToArray(), 2, num);
				_infoSpriteAsset.spriteSheet = (Texture)(object)val;
				((TMP_Asset)_infoSpriteAsset).material = new Material(Shader.Find("TextMeshPro/Sprite"))
				{
					mainTexture = (Texture)(object)val
				};
				_infoSpriteAsset.spriteInfoList = new List<TMP_Sprite>();
				Traverse.Create((object)_infoSpriteAsset).Field("m_Version").SetValue((object)"1.1.0");
				_infoSpriteAsset.spriteGlyphTable.Clear();
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
					_infoSpriteAsset.spriteGlyphTable.Add(item);
				}
				_infoSpriteAsset.spriteCharacterTable.Clear();
				for (int k = 0; k < spriteDataList.Count; k++)
				{
					string item2 = spriteDataList[k].name;
					TMP_SpriteCharacter item3 = new TMP_SpriteCharacter(65534u, _infoSpriteAsset.spriteGlyphTable[k])
					{
						name = item2,
						scale = 1f,
						glyphIndex = (uint)k
					};
					_infoSpriteAsset.spriteCharacterTable.Add(item3);
				}
				_infoSpriteAsset.UpdateLookupTables();
			}
			return _infoSpriteAsset;
			void AddSprite(string name, Texture2D tex)
			{
				spriteDataList.Add((name, textureList.Count));
				textureList.Add(tex);
			}
		}
	}

	public static string LeavesName
	{
		get
		{
			if (_leavesName == null)
			{
				GameObject forest = Main.GetObject("Environment Objects/LocalObjects_Prefab/Forest");
				_leavesName = (from t in forest.GetComponentsInChildren<Transform>(true)
					where ((Object)t).name.StartsWith("UnityTempFile") && (Object)(object)t.parent != (Object)null && (Object)(object)t.parent == (Object)(object)forest.transform
					group t by ((Object)t).name into g
					where g.Count() == 3
					orderby g.First().GetSiblingIndex() descending
					select g).FirstOrDefault()?.Key ?? "UnityTempFile";
			}
			return _leavesName;
		}
	}

	public static void VisualizeAura(Vector3 position, float range, Color color, long? indexId = null, float alpha = 0.25f)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		long item = indexId ?? BitPackUtils.PackWorldPosForNetwork(position);
		(long, float) key = (item, range);
		if (!auraPool.TryGetValue(key, out var value))
		{
			value = GameObject.CreatePrimitive((PrimitiveType)0);
			Object.Destroy((Object)(object)value.GetComponent<Collider>());
			auraPool.Add(key, value);
		}
		value.SetActive(true);
		value.transform.position = position;
		value.transform.localScale = new Vector3(range, range, range);
		if (Buttons.GetIndex("Hidden on Camera").enabled)
		{
			value.layer = 19;
		}
		Renderer component = value.GetComponent<Renderer>();
		Color color2 = color;
		color2.a = alpha;
		component.material.shader = Shader.Find("GUI/Text Shader");
		component.material.color = color2;
	}

	public static void VisualizeCube(Vector3 position, Quaternion rotation, Vector3 scale, Color color, long? indexId = null, float alpha = 0.25f)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		long num = indexId ?? BitPackUtils.PackWorldPosForNetwork(position);
		long key = num;
		if (!cubePool.TryGetValue(key, out var value))
		{
			value = GameObject.CreatePrimitive((PrimitiveType)3);
			Object.Destroy((Object)(object)value.GetComponent<Collider>());
			cubePool.Add(key, value);
		}
		value.SetActive(true);
		value.transform.position = position;
		value.transform.localScale = scale;
		value.transform.rotation = rotation;
		if (Buttons.GetIndex("Hidden on Camera").enabled)
		{
			value.layer = 19;
		}
		Renderer component = value.GetComponent<Renderer>();
		Color color2 = color;
		color2.a = alpha;
		component.material.shader = Shader.Find("GUI/Text Shader");
		component.material.color = color2;
	}

	public static void VisualizeCylinder(Vector3 position, Quaternion rotation, Vector3 scale, Color color, long? indexId = null, float alpha = 0.25f)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		long num = indexId ?? BitPackUtils.PackWorldPosForNetwork(position);
		long key = num;
		if (!cylinderPool.TryGetValue(key, out var value))
		{
			value = GameObject.CreatePrimitive((PrimitiveType)2);
			Object.Destroy((Object)(object)value.GetComponent<Collider>());
			cylinderPool.Add(key, value);
		}
		value.SetActive(true);
		value.transform.position = position;
		value.transform.localScale = scale;
		value.transform.rotation = rotation;
		if (Buttons.GetIndex("Hidden on Camera").enabled)
		{
			value.layer = 19;
		}
		Renderer component = value.GetComponent<Renderer>();
		Color color2 = color;
		color2.a = alpha;
		component.material.shader = Shader.Find("GUI/Text Shader");
		component.material.color = color2;
	}

	public static GameObject VisualizeAuraObject(Vector3 position, float range, Color color, float alpha = 0.25f)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)0);
		Object.Destroy((Object)(object)val.GetComponent<Collider>());
		val.SetActive(true);
		val.transform.position = position;
		val.transform.localScale = new Vector3(range, range, range);
		if (Buttons.GetIndex("Hidden on Camera").enabled)
		{
			val.layer = 19;
		}
		Renderer component = val.GetComponent<Renderer>();
		Color color2 = color;
		color2.a = alpha;
		component.material.shader = Shader.Find("GUI/Text Shader");
		component.material.color = color2;
		return val;
	}

	public static GameObject VisualizeCubeObject(Vector3 position, Quaternion rotation, Vector3 scale, Color color, float alpha = 0.25f)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
		Object.Destroy((Object)(object)val.GetComponent<Collider>());
		val.SetActive(true);
		val.transform.position = position;
		val.transform.localScale = scale;
		val.transform.rotation = rotation;
		if (Buttons.GetIndex("Hidden on Camera").enabled)
		{
			val.layer = 19;
		}
		Renderer component = val.GetComponent<Renderer>();
		Color color2 = color;
		color2.a = alpha;
		component.material.shader = Shader.Find("GUI/Text Shader");
		component.material.color = color2;
		return val;
	}

	public static void ConductDebug()
	{
		string text = "";
		text = text + string.Format("<color=blue><b>Seralyth</b></color> {0}  <color=grey>|</color>  Users Online:  {1}", "10.0.2", ServerData.onlineUsers) + "\\n \\n";
		string text2 = "<color=red>" + MathF.Floor(PlayerPrefs.GetFloat("redValue") * 255f) + "</color>";
		string text3 = ", <color=green>" + MathF.Floor(PlayerPrefs.GetFloat("greenValue") * 255f) + "</color>";
		string text4 = ", <color=blue>" + MathF.Floor(PlayerPrefs.GetFloat("blueValue") * 255f) + "</color>";
		string text5 = "<color=red>" + MathF.Round(PlayerPrefs.GetFloat("redValue") * 9f) + "</color>";
		string text6 = ", <color=green>" + MathF.Round(PlayerPrefs.GetFloat("greenValue") * 9f) + "</color>";
		string text7 = ", <color=blue>" + MathF.Round(PlayerPrefs.GetFloat("blueValue") * 9f) + "</color>";
		text = text + "<color=green>Color</color><color=grey>:</color> " + text2 + text3 + text4 + " <color=grey>[</color>" + text5 + text6 + text7 + "<color=grey>]</color>\\n";
		string text8 = ((PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient) ? "<color=grey> [</color><color=red>Master</color><color=grey>]</color>" : "");
		string[] obj = new string[5] { text, "<color=green>Name</color><color=grey>:</color> ", null, null, null };
		Player localPlayer = PhotonNetwork.LocalPlayer;
		obj[2] = ((localPlayer != null) ? localPlayer.NickName : null);
		obj[3] = text8;
		obj[4] = "\\n";
		text = string.Concat(obj);
		string text9 = text;
		object obj2;
		if (!Settings.hideId)
		{
			Player localPlayer2 = PhotonNetwork.LocalPlayer;
			obj2 = ((localPlayer2 != null) ? localPlayer2.UserId : null);
		}
		else
		{
			obj2 = "Hidden";
		}
		text = text9 + "<color=green>ID</color><color=grey>:</color> " + (string?)obj2 + "\\n";
		string text10 = text;
		string systemCopyBuffer = GUIUtility.systemCopyBuffer;
		text = text10 + "<color=green>Clip</color><color=grey>:</color> " + ((systemCopyBuffer != null && systemCopyBuffer.Length > 35) ? GUIUtility.systemCopyBuffer.Substring(0, 35) : GUIUtility.systemCopyBuffer) + "\\n";
		text = text + Main.lastDeltaTime + " <color=green>FPS</color> <color=grey>|</color> " + PhotonNetwork.GetPing() + " <color=green>Ping</color>\\n";
		string text11 = ((!PhotonNetwork.InRoom) ? "Not in room" : (NetworkSystem.Instance.SessionIsPrivate ? "Private" : "Public"));
		text = text + "<color=green>" + NetworkSystem.Instance.regionNames[NetworkSystem.Instance.currentRegionIndex].ToUpper() + "</color> " + PhotonNetwork.PlayerList.Length + " <color=green>Players</color> <color=grey>|</color> " + text11 + "\\n \\n";
		string text12 = "";
		if (Time.time > 5f)
		{
			Dictionary<string, string> administrators = ServerData.Administrators;
			Player localPlayer3 = PhotonNetwork.LocalPlayer;
			if (administrators.TryGetValue(((localPlayer3 != null) ? localPlayer3.UserId : null) ?? string.Empty, out var value))
			{
				text12 = " <color=grey>|</color> <color=red>Console " + (ServerData.SuperAdministrators.Contains(value) ? "Super " : "") + "Admin</color>";
			}
		}
		text = text + "<color=green>Theme</color> " + Main.themeType + text12 + "\n";
		text = text + "<color=green>Preferences Directory</color><color=grey>:</color> " + FileUtilities.GetGamePath() + "/SeralythMenu";
		((TMP_Text)(object)Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData").GetComponent<TextMeshPro>()).SafeSetText(text);
	}

	public static void ToggleSnow(bool enable)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		GameObject gameObject = ((Component)Main.GetObject("Environment Objects/LocalObjects_Prefab/Forest/Environment/WeatherDayNight").transform.Find("snow")).gameObject;
		gameObject.SetActive(enable);
		Transform transform = gameObject.transform;
		transform.position += Vector3.one * (enable ? 0.001f : (-0.001f));
		((Behaviour)gameObject.GetComponent<TimeOfDayDependentAudio>()).enabled = !enable;
		((Component)gameObject.transform.Find("snow partic")).gameObject.SetActive(enable);
	}

	public static void WeatherChange(bool rain)
	{
		for (int i = 0; i < ((BetterDayNightManager)BetterDayNightManager.instance).weatherCycle.Length; i++)
		{
			((BetterDayNightManager)BetterDayNightManager.instance).weatherCycle[i] = (WeatherType)(rain ? 1 : 0);
		}
	}

	public static void DisableFog()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		ZoneShaderSettings.activeInstance.SetGroundFogValue(Color.clear, 0f, 0f, 0f);
	}

	public static void EnableFog()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		ZoneShaderSettings.activeInstance.SetGroundFogValue(new Color(0.9569f, 0.6941f, 0.502f, 0.1216f), 40f, 10f, 40f);
	}

	public static void DisableAmbience()
	{
		TimeOfDayDependentAudio[] allType = Main.GetAllType<TimeOfDayDependentAudio>(5f);
		foreach (TimeOfDayDependentAudio val in allType)
		{
			if (val.currentVolume != 0f && !ambientObjects.ContainsKey(val))
			{
				ambientObjects.Add(val, val.currentVolume);
				val.currentVolume = 0f;
			}
		}
	}

	public static void EnableAmbience()
	{
		foreach (KeyValuePair<TimeOfDayDependentAudio, float> ambientObject in ambientObjects)
		{
			ambientObject.Key.currentVolume = ambientObject.Value;
		}
		ambientObjects.Clear();
	}

	public static void ResetFog()
	{
		ZoneShaderSettings.activeInstance.CopySettings(ZoneShaderSettings.defaultsInstance, false);
	}

	public static void CoreESP()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Transparent Theme").enabled;
		Color val = (Color)(enabled ? Main.backgroundColor.GetCurrentColor() : new Color(0.41f, 0.05f, 0.7f));
		if (enabled2)
		{
			val.a = 0.5f;
		}
		List<GameEntity> list = ManagerRegistry.GhostReactor.GameEntityManager.entities.Where((GameEntity entity) => (Object)(object)entity != (Object)null && entity.typeId == Overpowered.ObjectByName["GhostReactorCollectibleCore"]).ToList();
		if (list.Count > 0)
		{
			for (int num = 0; num < list.Count; num++)
			{
				Transform transform = ((Component)list[num]).transform;
				VisualizeAura(transform.position, 0.15f, val, num + 29875, val.a);
			}
		}
	}

	public static void CritterESP()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Transparent Theme").enabled;
		Color val = (enabled ? Main.backgroundColor.GetCurrentColor() : Color.green);
		if (enabled2)
		{
			val.a = 0.5f;
		}
		List<CrittersPawn> crittersPawns = ((CrittersManager)CrittersManager.instance).crittersPawns;
		if (crittersPawns.Count > 0)
		{
			for (int i = 0; i < crittersPawns.Count; i++)
			{
				Transform transform = ((Component)crittersPawns[i]).transform;
				VisualizeAura(transform.position, 0.15f, val, i - 192398, val.a);
			}
		}
	}

	public static void CreatureESP()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Transparent Theme").enabled;
		Color val = (enabled ? Main.backgroundColor.GetCurrentColor() : Color.green);
		if (enabled2)
		{
			val.a = 0.5f;
		}
		ThrowableBug[] allType = Main.GetAllType<ThrowableBug>(5f);
		if (allType.Length != 0)
		{
			for (int i = 0; i < allType.Length; i++)
			{
				Transform transform = ((Component)allType[i]).transform;
				VisualizeAura(transform.position, 0.15f, val, i - 201782, val.a);
			}
		}
	}

	public static void EnemyESP()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Transparent Theme").enabled;
		Color val = (Color)(enabled ? Main.backgroundColor.GetCurrentColor() : new Color(0.41f, 0.05f, 0.7f));
		if (enabled2)
		{
			val.a = 0.5f;
		}
		List<GameEntity> list = ManagerRegistry.GhostReactor.GameEntityManager.entities.Where((GameEntity entity) => (Object)(object)entity != (Object)null && ((Object)((Component)entity).gameObject).name.ToLower().Contains("enemy")).ToList();
		if (list.Count > 0)
		{
			for (int num = 0; num < list.Count; num++)
			{
				Transform transform = ((Component)list[num]).transform;
				VisualizeAura(transform.position, 0.15f, val, num + 451980, val.a);
			}
		}
	}

	public static void ResourceESP()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Transparent Theme").enabled;
		Color val = (enabled ? Main.backgroundColor.GetCurrentColor() : Color.green);
		if (enabled2)
		{
			val.a = 0.5f;
		}
		List<GameEntity> list = ManagerRegistry.SuperInfection.GameEntityManager.entities.Where((GameEntity entity) => (Object)(object)entity != (Object)null && ((Object)((Component)entity).gameObject).name.Contains("Resource")).ToList();
		if (list.Count > 0)
		{
			for (int num = 0; num < list.Count; num++)
			{
				Transform transform = ((Component)list[num]).transform;
				VisualizeAura(transform.position, 0.15f, val, num + 451961280, val.a);
			}
		}
	}

	public static void SetFullbrightStatus(bool fullBright)
	{
		if (fullBright)
		{
			previousFullbrightStatus = ((GameLightingManager)GameLightingManager.instance).customVertexLightingEnabled;
			((GameLightingManager)GameLightingManager.instance).SetCustomDynamicLightingEnabled(false);
		}
		else if (previousFullbrightStatus)
		{
			((GameLightingManager)GameLightingManager.instance).SetCustomDynamicLightingEnabled(true);
		}
	}

	public static void RemoveBlindfold()
	{
		if (!PhotonNetwork.InRoom || !(Time.time > removeBlindfoldDelay))
		{
			return;
		}
		removeBlindfoldDelay = Time.time + 0.5f;
		GameObject val = Main.GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/Main Camera");
		int childCount = val.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			GameObject gameObject = ((Component)val.transform.GetChild(i)).gameObject;
			if (((Object)gameObject).name == "PropHunt_Blindfold_ForCameras_Prefab(Clone)")
			{
				Object.Destroy((Object)(object)gameObject);
			}
		}
	}

	public static void LeaderboardInfo()
	{
		foreach (GorillaScoreBoard allScoreboard in GorillaScoreboardTotalUpdater.allScoreboards)
		{
			((TMP_Text)allScoreboard.boardText).richText = true;
			TextMeshPro boardText = allScoreboard.boardText;
			if (((TMP_Text)boardText).spriteAsset == null)
			{
				TMP_SpriteAsset val = (((TMP_Text)boardText).spriteAsset = InfoSprites);
			}
		}
	}

	public static void InstantiateWatch()
	{
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		GameObject gameObject = ((Component)((Component)VRRig.LocalRig).transform.Find("rig/hand.L/huntcomputer (1)")).gameObject;
		GameObject val = Object.Instantiate<GameObject>(gameObject, Main.rightHand ? ((Component)((Component)VRRig.LocalRig).transform.Find("rig/hand.R")).transform : ((Component)((Component)VRRig.LocalRig).transform.Find("rig/hand.L")).transform, false);
		Object.DestroyImmediate((Object)(object)val.GetComponent<GorillaHuntComputer>());
		Transform val2 = val.transform.Find("HuntWatch_ScreenLocal/Canvas/Anchor");
		((Component)val2.Find("Hat")).gameObject.SetActive(false);
		((Component)val2.Find("Face")).gameObject.SetActive(false);
		((Component)val2.Find("Badge")).gameObject.SetActive(false);
		((Component)val2.Find("Material")).gameObject.SetActive(false);
		((Component)val2.Find("Right Hand")).gameObject.SetActive(false);
		((Component)val2.Find("Left Hand")).gameObject.SetActive(false);
		GameObject gameObject2 = ((Component)val.transform.Find("HuntWatch_ScreenLocal")).gameObject;
		if (Main.rightHand)
		{
			gameObject2.transform.localRotation = Quaternion.Euler(0f, 140f, 0f);
			Transform parent = gameObject2.transform.parent;
			parent.localPosition += new Vector3(0.025f, 0f, 0f);
			Transform transform = gameObject2.transform;
			transform.localPosition += new Vector3(0.025f, 0f, -0.035f);
		}
		GameObject gameObject3 = ((Component)val2.Find("Text")).gameObject;
		Object.DestroyImmediate((Object)(object)gameObject3.GetComponent<Text>());
		TextMeshProUGUI val3 = gameObject3.AddComponent<TextMeshProUGUI>();
		((TMP_Text)(object)val3).SafeSetFont(Main.activeFont);
		((TMP_Text)(object)val3).SafeSetFontStyle(Main.activeFontStyle);
		((TMP_Text)(object)val3).SafeSetFontSize(10f);
		((TMP_Text)val3).enableWordWrapping = true;
		((TMP_Text)val3).overflowMode = (TextOverflowModes)3;
		Watches.Add(new WatchInfo
		{
			gameObject = val,
			text = val3,
			shell = gameObject2,
			indicator = ((Component)val2.Find("Left Hand")).gameObject.GetComponent<Image>()
		});
	}

	public static void WatchOn()
	{
		Watches[1].gameObject.SetActive(true);
	}

	public static void WatchStep()
	{
		TextMeshProUGUI text = Watches[1].text;
		if (Object.op_Implicit((Object)(object)text))
		{
			bool flag = !infoWatchMenuName && !infoWatchTime && !infoWatchClip && !infoWatchFPS && !infoWatchCode;
			string text2 = "";
			if (infoWatchMenuName || flag)
			{
				text2 = (Main.doCustomName ? Main.NoRichtextTags(Main.customMenuName) : "MrChicken Menu") + "\n<color=grey>";
			}
			else if (!infoWatchMenuName && !flag)
			{
				text2 = "<color=grey>";
			}
			if (infoWatchFPS || flag)
			{
				text2 = text2 + Main.lastDeltaTime + " FPS\n";
			}
			if (infoWatchTime || flag)
			{
				text2 = text2 + DateTime.Now.ToString("hh:mm tt") + "\n";
			}
			if (infoWatchCode)
			{
				text2 = text2 + (PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.Name : "Not in room") + "\n";
			}
			if (infoWatchClip)
			{
				string systemCopyBuffer = GUIUtility.systemCopyBuffer;
				text2 = text2 + "Clip: " + ((systemCopyBuffer.Length > 20) ? systemCopyBuffer.Substring(0, 20) : systemCopyBuffer) + "\n";
			}
			text2 += "</color>";
			((TMP_Text)(object)text).SafeSetText(Main.lowercaseMode ? text2.ToLower() : (Main.uppercaseMode ? text2.ToUpper() : text2));
		}
		else
		{
			LogManager.LogError("Watch text component not found");
		}
	}

	public static void WatchOff()
	{
		Watches[1].gameObject.SetActive(false);
	}

	public static void DoCustomSkyboxColor()
	{
		GameObject val = Main.GetObject("Environment Objects/LocalObjects_Prefab/Standard Sky");
		oldSkyMat = val.GetComponent<Renderer>().material;
	}

	public static void CustomSkyboxColor()
	{
		Main.GetObject("Environment Objects/LocalObjects_Prefab/Standard Sky").GetComponent<Renderer>().material = CustomBoardManager.BoardMaterial;
	}

	public static void UnCustomSkyboxColor()
	{
		GameObject val = Main.GetObject("Environment Objects/LocalObjects_Prefab/Standard Sky");
		val.GetComponent<Renderer>().material = oldSkyMat;
	}

	public static void DrawGun()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		GameObject item = Main.RenderGun(LayerMask.op_Implicit(GTPlayer.Instance.locomotionEnabledLayers)).NewPointer;
		if ((Object)(object)trailRenderer == (Object)null)
		{
			GameObject val = new GameObject("Seralyth_DrawGunTrail");
			trailRenderer = val.AddComponent<TrailRenderer>();
			trailRenderer.startWidth = 0.1f;
			trailRenderer.endWidth = 0.1f;
			trailRenderer.minVertexDistance = 0.05f;
			((Renderer)trailRenderer).material.shader = Shader.Find("GUI/Text Shader");
			trailRenderer.time = float.PositiveInfinity;
			trailRenderer.startColor = Color.black;
			trailRenderer.endColor = Color.black;
			if (Main.smoothLines)
			{
				trailRenderer.numCapVertices = 10;
				trailRenderer.numCornerVertices = 5;
			}
		}
		trailRenderer.emitting = Main.GetGunInput(isShooting: true);
		((Component)trailRenderer).gameObject.transform.position = item.transform.position;
	}

	public static void DisableDrawGun()
	{
		if ((Object)(object)trailRenderer != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)trailRenderer).gameObject);
		}
		trailRenderer = null;
	}

	public static void OnHandTapGamesenseRing(VRRig rig, Vector3 position)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		if (!rig.isLocal && !(Vector3.Distance(((Component)GorillaTagger.Instance.bodyCollider).transform.position, position) > 20f))
		{
			bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
			bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
			bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
			GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
			Object.Destroy((Object)(object)val.GetComponent<Collider>());
			if (enabled2)
			{
				val.layer = 19;
			}
			if ((Object)(object)tapMat == (Object)null)
			{
				tapMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
				tapMat.SetFloat("_Surface", 1f);
				tapMat.SetFloat("_Blend", 0f);
				tapMat.SetFloat("_SrcBlend", 5f);
				tapMat.SetFloat("_DstBlend", 10f);
				tapMat.SetFloat("_ZWrite", 0f);
				tapMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
				tapMat.renderQueue = 3000;
			}
			if ((Object)(object)tapTxt == (Object)null)
			{
				tapTxt = AssetUtilities.LoadTextureFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Images/Mods/Visuals/footstep.png", "footstep.png");
			}
			if ((Object)(object)warningTxt == (Object)null)
			{
				warningTxt = AssetUtilities.LoadTextureFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Images/Mods/Visuals/warning.png", "warning.png");
			}
			Renderer component = val.GetComponent<Renderer>();
			component.material = tapMat;
			component.material.mainTexture = (Texture)(object)((VRRig.LocalRig.IsTagged() == rig.IsTagged()) ? tapTxt : warningTxt);
			Color val2 = rig.GetColor();
			if (enabled)
			{
				val2 = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				((Color)(ref val2))._002Ector(val2.r, val2.g, val2.b, 0.5f);
			}
			component.material.color = val2;
			handTaps.Add(new object[5]
			{
				rig,
				position,
				Time.time,
				val,
				val2
			});
		}
	}

	public static void GamesenseRing()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val5 = default(Vector2);
		for (int num = handTaps.Count - 1; num >= 0; num--)
		{
			object[] array = handTaps[num];
			Vector3 val = (Vector3)array[1];
			float num2 = (float)array[2];
			GameObject val2 = (GameObject)array[3];
			Color val3 = (Color)array[4];
			float num3 = Time.time - num2;
			if (num3 > 1f || (Object)(object)val2 == (Object)null)
			{
				if ((Object)(object)val2 != (Object)null)
				{
					Object.Destroy((Object)(object)val2);
				}
				handTaps.RemoveAt(num);
			}
			else
			{
				Vector3 val4 = val - ((Component)Camera.main).transform.position;
				float num4 = Vector3.Dot(((Vector3)(ref val4)).normalized, ((Component)Camera.main).transform.right);
				val4 = val - ((Component)Camera.main).transform.position;
				float num5 = Vector3.Dot(((Vector3)(ref val4)).normalized, ((Component)Camera.main).transform.up);
				((Vector2)(ref val5))._002Ector(num4, num5);
				val2.transform.position = ((Component)Camera.main).transform.position + ((Component)Camera.main).transform.forward * 0.5f + (((Component)Camera.main).transform.right * val5.x + ((Component)Camera.main).transform.up * val5.y) * 0.2f;
				val2.transform.rotation = Quaternion.LookRotation(val2.transform.position - ((Component)Camera.main).transform.position, ((Component)Camera.main).transform.up);
				val2.transform.localScale = new Vector3(0.05f, 0.05f, 0.01f);
				float num6 = Mathf.Clamp01(1f - num3);
				val2.GetComponent<Renderer>().material.color = new Color(val3.r, val3.g, val3.b, num6);
			}
		}
	}

	public static void DisableGamesenseRing()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		foreach (object[] handTap in handTaps)
		{
			GameObject val = (GameObject)handTap[3];
			if ((Object)(object)val != (Object)null)
			{
				Object.Destroy((Object)(object)val);
			}
		}
		handTaps.Clear();
		HandTapPatch.OnHandTap = (Action<VRRig, Vector3>)Delegate.Remove(HandTapPatch.OnHandTap, new Action<VRRig, Vector3>(OnHandTapGamesenseRing));
	}

	public static void ChangePerformanceModeVisualStep(bool positive = true)
	{
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				PerformanceModeStepIndex++;
			}
			else
			{
				PerformanceModeStepIndex--;
			}
		}
		PerformanceModeStepIndex %= 11;
		if (PerformanceModeStepIndex < 0)
		{
			PerformanceModeStepIndex = 10;
		}
		PerformanceModeStep = (float)PerformanceModeStepIndex / 10f;
		Buttons.GetIndex("Change Performance Visuals Step").overlapText = "Change Performance Visuals Step <color=grey>[</color><color=green>" + PerformanceModeStep + "</color><color=grey>]</color>";
	}

	public static float GetLabelDistance(bool leftHand)
	{
		if (!labelDistances.TryGetValue(leftHand, out var value))
		{
			value = new List<int> { Time.frameCount };
			labelDistances[leftHand] = value;
			return 0.2f;
		}
		if (value[0] == Time.frameCount)
		{
			value.Add(Time.frameCount);
			return 0.1f + (float)Time.frameCount * 0.1f;
		}
		value.Clear();
		value.Add(Time.frameCount);
		return 0.1f + (float)value.Count * 0.1f;
	}

	public static void GetLabel(string codeName, bool leftHand, string text, Color color)
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		if (!labelDictionary.TryGetValue(codeName, out var value))
		{
			value = new GameObject(codeName);
			if (Buttons.GetIndex("Hidden Labels").enabled)
			{
				value.layer = 19;
			}
			value.transform.localScale = Vector3.one * (0.25f * (Main.scaleWithPlayer ? GTPlayer.Instance.scale : 1f));
			labelDictionary.Add(codeName, value);
		}
		value.SetActive(true);
		TextMeshPro orAddComponent = GTExt.GetOrAddComponent<TextMeshPro>(value);
		((Graphic)orAddComponent).color = color;
		((TMP_Text)orAddComponent).fontSize = 2.4f;
		((TMP_Text)(object)orAddComponent).SafeSetFontStyle(Main.activeFontStyle);
		((TMP_Text)(object)orAddComponent).SafeSetFont(Main.activeFont);
		((TMP_Text)orAddComponent).alignment = (TextAlignmentOptions)514;
		((TMP_Text)(object)orAddComponent).SafeSetText(text);
		value.transform.position = (leftHand ? GorillaTagger.Instance.leftHandTransform : GorillaTagger.Instance.rightHandTransform).position + Vector3.up * (GetLabelDistance(leftHand) * (Main.scaleWithPlayer ? GTPlayer.Instance.scale : 1f));
		value.transform.LookAt(((Component)Camera.main).transform.position);
		value.transform.Rotate(0f, 180f, 0f);
	}

	public static void UpdatePlaytime()
	{
		((MonoBehaviour)CoroutineManager.instance).StartCoroutine(Updateplaytime());
	}

	private static IEnumerator Updateplaytime()
	{
		playtime += Time.deltaTime;
		TimeSpan time = TimeSpan.FromSeconds(playtime);
		OverallPlaytime = $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
		yield return (object)new WaitForSeconds(0.1f);
	}

	public static void ExtraRoomInfo(bool? overlapInRoom = null)
	{
		if (overlapInRoom ?? PhotonNetwork.InRoom)
		{
			((Dictionary<object, object>)(object)NetworkSystem.Instance.CurrentRoom.CustomProps).TryGetValue((object)"platform", out object value);
			((Dictionary<object, object>)(object)NetworkSystem.Instance.CurrentRoom.CustomProps).TryGetValue((object)"language", out object value2);
			((Dictionary<object, object>)(object)NetworkSystem.Instance.CurrentRoom.CustomProps).TryGetValue((object)"mmrTier", out object value3);
			NotificationManager.information["Language"] = value2?.ToString() ?? "Unknown";
			NotificationManager.information["Platform"] = value?.ToString() ?? "Unknown";
			if (value3 != null)
			{
				NotificationManager.information["MMR Tier"] = value3.ToString();
			}
		}
		else
		{
			NotificationManager.information.Remove("Language");
			NotificationManager.information.Remove("MMR Tier");
		}
	}

	public static void VelocityLabel()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (!DoPerformanceCheck())
		{
			Vector3 linearVelocity = GorillaTagger.Instance.rigidbody.linearVelocity;
			string text = $"{((Vector3)(ref linearVelocity)).magnitude:F1}m/s";
			linearVelocity = GorillaTagger.Instance.rigidbody.linearVelocity;
			GetLabel("Velocity", leftHand: false, text, (((Vector3)(ref linearVelocity)).magnitude >= GTPlayer.Instance.maxJumpSpeed) ? Color.green : Color.white);
		}
	}

	private static string FormatTimer(int seconds)
	{
		int num = seconds / 60;
		int num2 = seconds % 60;
		return $"{num:D2}:{num2:D2}";
	}

	public static void TimeLabel()
	{
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		if (DoPerformanceCheck() || !PhotonNetwork.InRoom)
		{
			return;
		}
		if (GameModeUtilities.InfectedList().Count > 0)
		{
			bool flag = VRRig.LocalRig.IsTagged();
			if (flag)
			{
				if (!lastWasTagged)
				{
					endTime = Time.time - startTime;
				}
			}
			else if (lastWasTagged)
			{
				startTime = Time.time;
			}
			lastWasTagged = flag;
			GetLabel("Time", leftHand: false, FormatTimer(Mathf.FloorToInt(flag ? endTime : (Time.time - startTime))), flag ? Color.green : Color.white);
		}
		else
		{
			startTime = Time.time;
		}
	}

	public static void PingOverlay()
	{
		VRRig val = PhotonNetwork.MasterClient?.VRRig();
		if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient || (Object)(object)val == (Object)null || !Main.playerPing.ContainsKey(val))
		{
			NotificationManager.information["Ping"] = PhotonNetwork.GetPing() + "ms";
		}
		else
		{
			NotificationManager.information["Ping"] = val.GetPing() + PhotonNetwork.GetPing() + "ms";
		}
	}

	public static void NearbyTaggerOverlay()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (DoPerformanceCheck())
		{
			return;
		}
		float num = float.MaxValue;
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (activeRig.IsTagged() != VRRig.LocalRig.IsTagged())
			{
				float num2 = Vector3.Distance(((Component)GorillaTagger.Instance.headCollider).transform.position, activeRig.headMesh.transform.position);
				if (num2 < num)
				{
					num = num2;
				}
			}
		}
		if (!Mathf.Approximately(num, float.MaxValue))
		{
			NotificationManager.information["Nearby"] = $"{num:F1}m";
		}
		else
		{
			NotificationManager.information.Remove("Nearby");
		}
	}

	public static void InfoOverlayGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				NotificationManager.information["Name"] = Main.lockTarget.GetName();
				NotificationManager.information["Color"] = Main.lockTarget.GetColor().ToRGBString();
				NotificationManager.information["ID"] = Main.lockTarget.GetPlayer().UserId;
				NotificationManager.information["Platform"] = Main.lockTarget.GetPlatform();
				NotificationManager.information["Ping"] = Main.lockTarget.GetPing().ToString();
				NotificationManager.information["FPS"] = Main.lockTarget.fps.ToString();
				NotificationManager.information["Creation Date"] = Main.lockTarget.GetCreationDate();
				NotificationManager.information["Turn"] = $"{Main.lockTarget.turnType.ToTitleCase()} {Main.lockTarget.turnFactor}";
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			NotificationManager.information.Remove("Name");
			NotificationManager.information.Remove("Color");
			NotificationManager.information.Remove("ID");
			NotificationManager.information.Remove("Platform");
			NotificationManager.information.Remove("Ping");
			NotificationManager.information.Remove("FPS");
			NotificationManager.information.Remove("Creation Date");
			NotificationManager.information.Remove("Turn");
		}
	}

	public static void EnableDebugHUD()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		DebugHudStats component = ((Component)((Component)Camera.main).transform.Find("DebugCanvas")).GetComponent<DebugHudStats>();
		component.builder = new StringBuilder();
		component.drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count", 1, (ProfilerRecorderOptions)24);
		component.trisRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Tris Count", 1, (ProfilerRecorderOptions)24);
		((Component)component).gameObject.SetActive(true);
		((Component)component.text).gameObject.SetActive(true);
		((Behaviour)component).enabled = true;
	}

	public static void DisableDebugHUD()
	{
		DebugHudStats component = ((Component)((Component)Camera.main).transform.Find("DebugCanvas")).GetComponent<DebugHudStats>();
		((Component)component).gameObject.SetActive(false);
	}

	public static void NearbyTaggerLabel()
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		if (DoPerformanceCheck() || VRRig.LocalRig.IsTagged())
		{
			return;
		}
		float num = float.MaxValue;
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (activeRig.IsTagged() != VRRig.LocalRig.IsTagged())
			{
				float num2 = Vector3.Distance(((Component)GorillaTagger.Instance.headCollider).transform.position, activeRig.headMesh.transform.position);
				if (num2 < num)
				{
					num = num2;
				}
			}
		}
		if (!Mathf.Approximately(num, float.MaxValue))
		{
			Color color = Color.green;
			if (num < 30f)
			{
				color = Color.yellow;
			}
			if (num < 20f)
			{
				color = Color32.op_Implicit(new Color32(byte.MaxValue, (byte)90, (byte)0, byte.MaxValue));
			}
			if (num < 10f)
			{
				color = Color.red;
			}
			GetLabel("NearbyTagger", leftHand: true, $"{num:F1}m", color);
		}
	}

	public static void LastLabel()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (!DoPerformanceCheck() && PhotonNetwork.InRoom)
		{
			bool flag = GameModeUtilities.InfectedList().Count > 0;
			int num = PhotonNetwork.PlayerList.Length - GameModeUtilities.InfectedList().Count;
			if (flag)
			{
				GetLabel("LastLabel", leftHand: true, num + " left", (num <= 1 && !VRRig.LocalRig.IsTagged()) ? Color.green : Color.white);
			}
		}
	}

	public static void FakeUnbanSelf()
	{
		((PhotonNetworkController)PhotonNetworkController.Instance).UpdateTriggerScreens();
		GorillaScoreboardTotalUpdater.instance.ClearOfflineFailureText();
		((GorillaComputer)GorillaComputer.instance).screenText.DisableFailedState();
		((GorillaComputer)GorillaComputer.instance).functionSelectText.DisableFailedState();
	}

	public static void CreateAudioVisualizer()
	{
		visualizerObject = GameObject.CreatePrimitive((PrimitiveType)2);
		visualizerOutline = GameObject.CreatePrimitive((PrimitiveType)2);
		Object.Destroy((Object)(object)visualizerObject.GetComponent<Collider>());
		Object.Destroy((Object)(object)visualizerOutline.GetComponent<Collider>());
	}

	public static void AudioVisualizer()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		visualizerObject.GetComponent<Renderer>().material.color = Main.backgroundColor.GetCurrentColor();
		visualizerOutline.GetComponent<Renderer>().material.color = Main.buttonColors[0].GetCurrentColor();
		RaycastHit val = default(RaycastHit);
		Physics.Raycast(((Component)GorillaTagger.Instance.bodyCollider).transform.position - new Vector3(0f, 0.2f, 0f), Vector3.down, ref val, 512f, LayerMask.op_Implicit(GTPlayer.Instance.locomotionEnabledLayers));
		visualizerObject.transform.position = ((RaycastHit)(ref val)).point;
		visualizerObject.transform.rotation = Quaternion.LookRotation(((RaycastHit)(ref val)).normal) * Quaternion.Euler(90f, 0f, 0f);
		float num = 0f;
		GorillaSpeakerLoudness component = ((Component)VRRig.LocalRig).GetComponent<GorillaSpeakerLoudness>();
		if ((Object)(object)component != (Object)null)
		{
			num = component.Loudness;
		}
		num *= 16f;
		visualizerObject.transform.localScale = new Vector3(num, 0.05f, num);
		visualizerObject.GetComponent<Renderer>().enabled = num > 0.05f;
		visualizerOutline.GetComponent<Renderer>().enabled = num > 0.05f;
		visualizerOutline.transform.position = visualizerObject.transform.position;
		visualizerOutline.transform.rotation = visualizerObject.transform.rotation;
		visualizerOutline.transform.localScale = new Vector3(num + 0.05f, 0.025f, num + 0.05f);
	}

	public static void DestroyAudioVisualizer()
	{
		Object.Destroy((Object)(object)visualizerObject);
		Object.Destroy((Object)(object)visualizerOutline);
	}

	public static void ShowServerPosition()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)headPos == (Object)null)
		{
			headPos = GameObject.CreatePrimitive((PrimitiveType)0);
			Object.Destroy((Object)(object)headPos.GetComponent<Collider>());
			headPos.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
		}
		if ((Object)(object)leftHandPos == (Object)null)
		{
			leftHandPos = GameObject.CreatePrimitive((PrimitiveType)0);
			Object.Destroy((Object)(object)leftHandPos.GetComponent<Collider>());
			leftHandPos.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		}
		if ((Object)(object)rightHandPos == (Object)null)
		{
			rightHandPos = GameObject.CreatePrimitive((PrimitiveType)0);
			Object.Destroy((Object)(object)rightHandPos.GetComponent<Collider>());
			rightHandPos.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		}
		headPos.transform.position = Main.ServerPos;
		leftHandPos.transform.position = Main.ServerLeftHandPos;
		rightHandPos.transform.position = Main.ServerRightHandPos;
	}

	public static void ShowScheduledObjects()
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		GameObjectScheduler[] allType = Main.GetAllType<GameObjectScheduler>(5f);
		foreach (GameObjectScheduler val in allType)
		{
			((Component)val).gameObject.SetActive(true);
			GameObject[] scheduledGameObject = val.scheduledGameObject;
			foreach (GameObject val2 in scheduledGameObject)
			{
				val2.SetActive(true);
			}
			foreach (Transform item in ((Component)val).gameObject.transform)
			{
				Transform val3 = item;
				((Component)val3).gameObject.SetActive(true);
			}
			((Behaviour)val).enabled = false;
		}
	}

	public static void DisableShowServerPosition()
	{
		if ((Object)(object)headPos != (Object)null)
		{
			Object.Destroy((Object)(object)headPos);
		}
		if ((Object)(object)leftHandPos != (Object)null)
		{
			Object.Destroy((Object)(object)leftHandPos);
		}
		if ((Object)(object)rightHandPos != (Object)null)
		{
			Object.Destroy((Object)(object)rightHandPos);
		}
	}

	public static void JumpPredictions()
	{
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Expected O, but got Unknown
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, LineRenderer> item in predictions.Where((KeyValuePair<VRRig, LineRenderer> lines) => !VRRigCache.ActiveRigs.Contains(lines.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)((Component)item.Value).gameObject);
		}
		foreach (VRRig item2 in list)
		{
			predictions.Remove(item2);
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		bool enabled4 = Buttons.GetIndex("Thin Tracers").enabled;
		foreach (VRRig item3 in VRRigCache.ActiveRigs.Where((VRRig rig) => !rig.isLocal))
		{
			if (!predictions.TryGetValue(item3, out var value))
			{
				GameObject val = new GameObject("LineObject");
				value = val.AddComponent<LineRenderer>();
				if (Main.smoothLines)
				{
					value.numCapVertices = 10;
					value.numCornerVertices = 5;
				}
				((Renderer)value).material.shader = Shader.Find("GUI/Text Shader");
				value.startWidth = 0.025f;
				value.endWidth = 0.025f;
				value.positionCount = 25;
				value.useWorldSpace = true;
				predictions.Add(item3, value);
			}
			if (enabled2)
			{
				((Component)value).gameObject.layer = 19;
			}
			Color val2 = item3.GetColor();
			if (enabled)
			{
				val2 = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				((Color)(ref val2))._002Ector(val2.r, val2.g, val2.b, 0.5f);
			}
			float endWidth = (value.startWidth = (enabled4 ? 0.0075f : 0.025f));
			value.endWidth = endWidth;
			value.startColor = val2;
			value.endColor = val2;
			Vector3 syncPos = item3.syncPos;
			Vector3 velocity = item3.LatestVelocity();
			if (!(((Vector3)(ref velocity)).magnitude < 1.5f))
			{
				DrawTrajectory(syncPos, velocity, value);
			}
		}
	}

	public static void DisableJumpPredictions()
	{
		foreach (KeyValuePair<VRRig, LineRenderer> prediction in predictions)
		{
			Object.Destroy((Object)(object)((Component)prediction.Value).gameObject);
		}
		predictions.Clear();
	}

	public static void HitboxPredictions()
	{
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, GameObject> item in hitboxESP.Where((KeyValuePair<VRRig, GameObject> box) => !VRRigCache.ActiveRigs.Contains(box.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			hitboxESP.Remove(item2);
		}
		foreach (VRRig item3 in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal))
		{
			if (!hitboxESP.TryGetValue(item3, out var value))
			{
				value = GameObject.CreatePrimitive((PrimitiveType)1);
				Object.Destroy((Object)(object)value.GetComponent<BoxCollider>());
				value.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
				value.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
				hitboxESP.Add(item3, value);
			}
			Color color = item3.playerColor;
			if (enabled)
			{
				color = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				color.a = 0.5f;
			}
			if (enabled2)
			{
				value.layer = 19;
			}
			Vector3 val = item3.LatestVelocity();
			value.GetComponent<Renderer>().enabled = ((Vector3)(ref val)).magnitude > 1f;
			value.GetComponent<Renderer>().material.color = color;
			value.transform.position = ((Component)item3).transform.position + val * 0.5f;
		}
	}

	public static void DisableHitboxPredictions()
	{
		foreach (KeyValuePair<VRRig, GameObject> item in hitboxESP)
		{
			Object.Destroy((Object)(object)item.Value);
		}
		hitboxESP.Clear();
	}

	public static void PaintbrawlTrajectories()
	{
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Expected O, but got Unknown
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Expected O, but got Unknown
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		bool fmt = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool hoc = Buttons.GetIndex("Hidden on Camera").enabled;
		bool tt = Buttons.GetIndex("Transparent Theme").enabled;
		bool thinTracers = Buttons.GetIndex("Thin Tracers").enabled;
		List<SlingshotProjectile> list = new List<SlingshotProjectile>();
		foreach (KeyValuePair<SlingshotProjectile, LineRenderer> item in trajectoryPool)
		{
			if (!((Component)item.Value).gameObject.activeSelf)
			{
				list.Add(item.Key);
				Object.Destroy((Object)(object)((Component)item.Value).gameObject);
			}
			else
			{
				((Component)item.Value).gameObject.SetActive(false);
			}
		}
		foreach (SlingshotProjectile item2 in list)
		{
			trajectoryPool.Remove(item2);
		}
		foreach (LoopingArray<ProjectileInfo> value5 in ProjectileTracker.m_playerProjectiles.Values)
		{
			LoopProjectileArray(value5);
		}
		LoopProjectileArray(ProjectileTracker.m_localProjectiles);
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (activeRig.IsLocal())
			{
				continue;
			}
			ProjectileWeapon slingshot = activeRig.GetSlingshot();
			Slingshot val = (Slingshot)(object)((slingshot is Slingshot) ? slingshot : null);
			if (!((Object)(object)val != (Object)null) || !val.InDrawingState() || !((Object)(object)val.dummyProjectile != (Object)null))
			{
				continue;
			}
			SlingshotProjectile component = val.dummyProjectile.GetComponent<SlingshotProjectile>();
			if ((Object)(object)component == (Object)null || !((Component)component).gameObject.activeSelf)
			{
				continue;
			}
			if (!trajectoryPool.TryGetValue(component, out var value))
			{
				GameObject val2 = new GameObject("LineObject");
				value = val2.AddComponent<LineRenderer>();
				if (Main.smoothLines)
				{
					value.numCapVertices = 10;
					value.numCornerVertices = 5;
				}
				((Renderer)value).material.shader = Shader.Find("GUI/Text Shader");
				value.startWidth = 0.025f;
				value.endWidth = 0.025f;
				value.positionCount = 25;
				value.useWorldSpace = true;
				trajectoryPool.Add(component, value);
			}
			((Component)value).gameObject.SetActive(true);
			if (hoc)
			{
				((Component)value).gameObject.layer = 19;
			}
			Color val3 = activeRig.GetColor();
			if (fmt)
			{
				val3 = Main.backgroundColor.GetCurrentColor();
			}
			if (tt)
			{
				((Color)(ref val3))._002Ector(val3.r, val3.g, val3.b, 0.5f);
			}
			float num = (thinTracers ? 0.0075f : 0.025f);
			float endWidth = (value.startWidth = num);
			value.endWidth = endWidth;
			value.startColor = val3;
			value.endColor = val3;
			Vector3 trueLaunchPosition = val.GetTrueLaunchPosition();
			Vector3 networkedLaunchVelocity = val.GetNetworkedLaunchVelocity();
			Vector3 gravity = Physics.gravity;
			ConstantForce forceComponent = component.forceComponent;
			Vector3 value2 = gravity + ((forceComponent != null) ? forceComponent.force : Vector3.zero);
			DrawTrajectory(trueLaunchPosition, networkedLaunchVelocity, value, Main.NoInvisLayerMask(), value2);
		}
		if ((Object)(object)localTrajectoryLine != (Object)null)
		{
			if (!((Component)localTrajectoryLine).gameObject.activeSelf)
			{
				Object.Destroy((Object)(object)((Component)localTrajectoryLine).gameObject);
				localTrajectoryLine = null;
			}
			else
			{
				((Component)localTrajectoryLine).gameObject.SetActive(false);
			}
		}
		ProjectileWeapon slingshot2 = VRRig.LocalRig.GetSlingshot();
		Slingshot val4 = (Slingshot)(object)((slingshot2 is Slingshot) ? slingshot2 : null);
		if ((Object)(object)val4 == (Object)null || !val4.InDrawingState())
		{
			return;
		}
		if ((Object)(object)localTrajectoryLine == (Object)null)
		{
			GameObject val5 = new GameObject("LineObject");
			localTrajectoryLine = val5.AddComponent<LineRenderer>();
			if (Main.smoothLines)
			{
				localTrajectoryLine.numCapVertices = 10;
				localTrajectoryLine.numCornerVertices = 5;
			}
			((Renderer)localTrajectoryLine).material.shader = Shader.Find("GUI/Text Shader");
			localTrajectoryLine.startWidth = 0.025f;
			localTrajectoryLine.endWidth = 0.025f;
			localTrajectoryLine.positionCount = 25;
			localTrajectoryLine.useWorldSpace = true;
		}
		((Component)localTrajectoryLine).gameObject.SetActive(true);
		if (hoc)
		{
			((Component)localTrajectoryLine).gameObject.layer = 19;
		}
		Color val6 = VRRig.LocalRig.GetColor();
		if (fmt)
		{
			val6 = Main.backgroundColor.GetCurrentColor();
		}
		if (tt)
		{
			((Color)(ref val6))._002Ector(val6.r, val6.g, val6.b, 0.5f);
		}
		float num3 = (thinTracers ? 0.0075f : 0.025f);
		localTrajectoryLine.startWidth = num3;
		localTrajectoryLine.endWidth = num3;
		localTrajectoryLine.startColor = val6;
		localTrajectoryLine.endColor = val6;
		Vector3 trueLaunchPosition2 = val4.GetTrueLaunchPosition();
		Vector3 launchVelocity = ((ProjectileWeapon)val4).GetLaunchVelocity();
		DrawTrajectory(trueLaunchPosition2, launchVelocity, localTrajectoryLine, Main.NoInvisLayerMask(), Vector3.down * 10.79f);
		void LoopProjectileArray(LoopingArray<ProjectileInfo> projectileArray)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Expected O, but got Unknown
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			if (projectileArray != null && projectileArray.Length > 0)
			{
				for (int i = 0; i < projectileArray.Length; i++)
				{
					SlingshotProjectile projectileInstance = projectileArray[i].projectileInstance;
					if (!((Object)(object)projectileInstance == (Object)null) && ((Component)projectileInstance).gameObject.activeSelf && (!Object.op_Implicit((Object)(object)((Slingshot)/*isinst with value type is only supported in some contexts*/).dummyProjectile) || !((Object)(object)((Slingshot)/*isinst with value type is only supported in some contexts*/).dummyProjectile.GetComponent<SlingshotProjectile>() == (Object)(object)projectileInstance)))
					{
						if (!trajectoryPool.TryGetValue(projectileInstance, out var value3))
						{
							GameObject val7 = new GameObject("LineObject");
							value3 = val7.AddComponent<LineRenderer>();
							if (Main.smoothLines)
							{
								value3.numCapVertices = 10;
								value3.numCornerVertices = 5;
							}
							((Renderer)value3).material.shader = Shader.Find("GUI/Text Shader");
							value3.startWidth = 0.025f;
							value3.endWidth = 0.025f;
							value3.positionCount = 25;
							value3.useWorldSpace = true;
							trajectoryPool.Add(projectileInstance, value3);
						}
						((Component)value3).gameObject.SetActive(true);
						if (hoc)
						{
							((Component)value3).gameObject.layer = 19;
						}
						Color val8 = projectileInstance.teamColor;
						if (fmt)
						{
							val8 = Main.backgroundColor.GetCurrentColor();
						}
						if (tt)
						{
							((Color)(ref val8))._002Ector(val8.r, val8.g, val8.b, 0.5f);
						}
						float num4 = (thinTracers ? 0.0075f : 0.025f);
						float endWidth2 = (value3.startWidth = num4);
						value3.endWidth = endWidth2;
						value3.startColor = val8;
						value3.endColor = val8;
						Vector3 position = ((Component)projectileInstance).transform.position;
						Vector3 linearVelocity = projectileInstance.projectileRigidbody.linearVelocity;
						Vector3 gravity2 = Physics.gravity;
						ConstantForce forceComponent2 = projectileInstance.forceComponent;
						Vector3 value4 = gravity2 + ((forceComponent2 != null) ? forceComponent2.force : Vector3.zero);
						DrawTrajectory(position, linearVelocity, value3, Main.NoInvisLayerMask(), value4);
					}
				}
			}
		}
	}

	public static void DisablePaintbrawlTrajectories()
	{
		foreach (KeyValuePair<SlingshotProjectile, LineRenderer> item in trajectoryPool)
		{
			Object.Destroy((Object)(object)((Component)item.Value).gameObject);
		}
		trajectoryPool.Clear();
		if ((Object)(object)localTrajectoryLine != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)localTrajectoryLine).gameObject);
			localTrajectoryLine = null;
		}
	}

	public static void DrawTrajectory(Vector3 position, Vector3 velocity, LineRenderer lineRenderer, int? overrideLayerMask = null, Vector3? overrideGravity = null)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		((Renderer)lineRenderer).enabled = true;
		int num = 25;
		Vector3[] array = (Vector3[])(object)new Vector3[num];
		VRRig val = null;
		int i;
		RaycastHit val7 = default(RaycastHit);
		for (i = 0; i < num; i++)
		{
			array[i] = position;
			Vector3 val2 = velocity + (Vector3)(((_003F?)overrideGravity) ?? Physics.gravity) * 0.1f;
			Vector3 val3 = position + velocity * 0.1f;
			if (i >= 1)
			{
				Vector3 val4 = position;
				Vector3 val5 = val3 - position;
				Vector3 val6 = val3 - position;
				if (Physics.Raycast(val4, val5, ref val7, ((Vector3)(ref val6)).magnitude, overrideLayerMask ?? LayerMask.op_Implicit(GTPlayer.Instance.locomotionEnabledLayers)))
				{
					array[i] = ((RaycastHit)(ref val7)).point;
					i++;
					VRRig componentInParent = ((Component)((RaycastHit)(ref val7)).collider).GetComponentInParent<VRRig>();
					if (Object.op_Implicit((Object)(object)componentInParent))
					{
						val = componentInParent;
					}
					break;
				}
			}
			position = val3;
			velocity = val2;
		}
		Color val8 = lineRenderer.startColor;
		if ((Object)(object)val != (Object)null)
		{
			val8 = (val.IsLocal() ? Color.red : Color.green);
		}
		lineRenderer.startColor = val8;
		lineRenderer.endColor = val8;
		Vector3[] array2 = (Vector3[])(object)new Vector3[i];
		Array.Copy(array, array2, i);
		lineRenderer.positionCount = array2.Length;
		lineRenderer.SetPositions(array2);
	}

	public static void VisualizeNetworkTriggers()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = Main.GetObject("Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab");
		for (int i = 0; i < val.transform.childCount; i++)
		{
			try
			{
				Transform child = val.transform.GetChild(i);
				if (((Component)child).gameObject.activeSelf)
				{
					VisualizeCube(child.position, child.rotation, child.lossyScale, Color.red);
				}
			}
			catch
			{
			}
		}
	}

	public static void VisualizeWindBarriers()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		ForceVolume[] allType = Main.GetAllType<ForceVolume>(5f);
		foreach (ForceVolume val in allType)
		{
			try
			{
				VisualizeCube(((Component)val).transform.position, ((Component)val).transform.rotation, ((Component)val).transform.lossyScale, Color.blue);
			}
			catch
			{
			}
		}
	}

	public static void VisualizeMapTriggers()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = Main.GetObject("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab");
		for (int i = 0; i < val.transform.childCount; i++)
		{
			try
			{
				Transform child = val.transform.GetChild(i);
				if (((Component)child).gameObject.activeSelf)
				{
					VisualizeCube(child.position, child.rotation, child.lossyScale, Main.backgroundColor.GetCurrentColor());
				}
			}
			catch
			{
			}
		}
	}

	public static float GetTagDistance(VRRig rig)
	{
		if (ntDistanceList.ContainsKey(rig))
		{
			if (ntDistanceList[rig][0] == Time.frameCount)
			{
				ntDistanceList[rig].Add(Time.frameCount);
				return (0.25f + (float)ntDistanceList[rig].Count * 0.15f) * rig.scaleFactor;
			}
			ntDistanceList[rig].Clear();
			ntDistanceList[rig].Add(Time.frameCount);
			return (0.25f + (float)ntDistanceList[rig].Count * 0.15f) * rig.scaleFactor;
		}
		ntDistanceList.Add(rig, new List<int> { Time.frameCount });
		return 0.4f * rig.scaleFactor;
	}

	public static bool NameTagOptimize()
	{
		if (Time.time < optimizeDelay)
		{
			if (Time.frameCount != optimizeChangeStep)
			{
				return false;
			}
		}
		else
		{
			optimizeDelay = Time.time + (PerformanceVisuals ? PerformanceVisualDelay : 0.1f);
			optimizeChangeStep = Time.frameCount;
		}
		return true;
	}

	public static Vector3 GetNameTagPosition(VRRig rig)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Transform val = (anchorNameTag ? ((Component)rig).transform : rig.headMesh.transform);
		return val.position + val.up * GetTagDistance(rig);
	}

	public static Transform GetNameTagTransform(VRRig rig)
	{
		return anchorNameTag ? ((Component)rig).transform : rig.headMesh.transform;
	}

	public static void NameTags()
	{
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		List<KeyValuePair<VRRig, GameObject>> source = nametags.ToList();
		foreach (KeyValuePair<VRRig, GameObject> item in source.Where((KeyValuePair<VRRig, GameObject> nametag) => !VRRigCache.ActiveRigs.Contains(nametag.Key)))
		{
			Object.Destroy((Object)(object)item.Value);
			nametags.Remove(item.Key);
		}
		foreach (VRRig item2 in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal || selfNameTag))
		{
			if (!nametags.ContainsKey(item2))
			{
				GameObject val = new GameObject("Seralyth_Nametag");
				val.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
				TextMeshPro val2 = val.AddComponent<TextMeshPro>();
				((TMP_Text)val2).fontSize = 4.8f;
				((TMP_Text)val2).alignment = (TextAlignmentOptions)514;
				nametags.Add(item2, val);
			}
			GameObject val3 = nametags[item2];
			TextMeshPro orAddComponent = GTExt.GetOrAddComponent<TextMeshPro>(val3);
			if (NameTagOptimize())
			{
				((TMP_Text)(object)orAddComponent).SafeSetText(Main.CleanPlayerName(RigUtilities.GetPlayerFromVRRig(item2).NickName));
				((Graphic)orAddComponent).color = item2.GetColor();
				((TMP_Text)(object)orAddComponent).SafeSetFontStyle(Main.activeFontStyle);
				((TMP_Text)(object)orAddComponent).SafeSetFont(Main.activeFont);
			}
			if (nameTagChams)
			{
				((TMP_Text)(object)orAddComponent).Chams();
			}
			val3.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * item2.scaleFactor;
			val3.transform.position = GetNameTagPosition(item2);
			val3.transform.LookAt(((Component)Camera.main).transform.position);
			val3.transform.Rotate(0f, 180f, 0f);
		}
	}

	public static void DisableNameTags()
	{
		foreach (KeyValuePair<VRRig, GameObject> nametag in nametags)
		{
			Object.Destroy((Object)(object)nametag.Value);
		}
		nametags.Clear();
	}

	public static void VelocityTags()
	{
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		List<KeyValuePair<VRRig, GameObject>> source = velnametags.ToList();
		foreach (KeyValuePair<VRRig, GameObject> item in source.Where((KeyValuePair<VRRig, GameObject> nametag) => !VRRigCache.ActiveRigs.Contains(nametag.Key)))
		{
			Object.Destroy((Object)(object)item.Value);
			velnametags.Remove(item.Key);
		}
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			try
			{
				if (!activeRig.isLocal || selfNameTag)
				{
					if (!velnametags.ContainsKey(activeRig))
					{
						GameObject val = new GameObject("Seralyth_Veltag");
						val.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
						TextMeshPro val2 = val.AddComponent<TextMeshPro>();
						((TMP_Text)val2).fontSize = 4.8f;
						((TMP_Text)val2).alignment = (TextAlignmentOptions)514;
						velnametags.Add(activeRig, val);
					}
					GameObject val3 = velnametags[activeRig];
					TextMeshPro orAddComponent = GTExt.GetOrAddComponent<TextMeshPro>(val3);
					if (NameTagOptimize())
					{
						Vector3 val4 = activeRig.LatestVelocity();
						((TMP_Text)(object)orAddComponent).SafeSetText($"{((Vector3)(ref val4)).magnitude:F1}m/s");
						((Graphic)orAddComponent).color = activeRig.GetColor();
						((TMP_Text)(object)orAddComponent).SafeSetFontStyle(Main.activeFontStyle);
						((TMP_Text)(object)orAddComponent).SafeSetFont(Main.activeFont);
					}
					if (nameTagChams)
					{
						((TMP_Text)(object)orAddComponent).Chams();
					}
					val3.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * activeRig.scaleFactor;
					val3.transform.position = GetNameTagPosition(activeRig);
					val3.transform.LookAt(((Component)Camera.main).transform.position);
					val3.transform.Rotate(0f, 180f, 0f);
				}
			}
			catch
			{
			}
		}
	}

	public static void DisableVelocityTags()
	{
		foreach (KeyValuePair<VRRig, GameObject> velnametag in velnametags)
		{
			Object.Destroy((Object)(object)velnametag.Value);
		}
		velnametags.Clear();
	}

	public static void FPSTags()
	{
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		List<KeyValuePair<VRRig, GameObject>> source = fpsNametags.ToList();
		foreach (KeyValuePair<VRRig, GameObject> item in source.Where((KeyValuePair<VRRig, GameObject> nametag) => !VRRigCache.ActiveRigs.Contains(nametag.Key)))
		{
			Object.Destroy((Object)(object)item.Value);
			fpsNametags.Remove(item.Key);
		}
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			try
			{
				if (!activeRig.isLocal || selfNameTag)
				{
					if (!fpsNametags.ContainsKey(activeRig))
					{
						GameObject val = new GameObject("Seralyth_FPStag");
						val.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
						TextMeshPro val2 = val.AddComponent<TextMeshPro>();
						((TMP_Text)val2).fontSize = 4.8f;
						((TMP_Text)val2).alignment = (TextAlignmentOptions)514;
						fpsNametags.Add(activeRig, val);
					}
					GameObject val3 = fpsNametags[activeRig];
					TextMeshPro orAddComponent = GTExt.GetOrAddComponent<TextMeshPro>(val3);
					if (NameTagOptimize())
					{
						((TMP_Text)(object)orAddComponent).SafeSetText($"{activeRig.fps} FPS");
						((Graphic)orAddComponent).color = activeRig.GetColor();
						((TMP_Text)(object)orAddComponent).SafeSetFontStyle(Main.activeFontStyle);
						((TMP_Text)(object)orAddComponent).SafeSetFont(Main.activeFont);
					}
					if (nameTagChams)
					{
						((TMP_Text)(object)orAddComponent).Chams();
					}
					val3.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * activeRig.scaleFactor;
					val3.transform.position = GetNameTagPosition(activeRig);
					val3.transform.LookAt(((Component)Camera.main).transform.position);
					val3.transform.Rotate(0f, 180f, 0f);
				}
			}
			catch
			{
			}
		}
	}

	public static void DisableFPSTags()
	{
		foreach (KeyValuePair<VRRig, GameObject> fpsNametag in fpsNametags)
		{
			Object.Destroy((Object)(object)fpsNametag.Value);
		}
		fpsNametags.Clear();
	}

	public static void IDTags()
	{
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		List<KeyValuePair<VRRig, GameObject>> source = idNameTags.ToList();
		foreach (KeyValuePair<VRRig, GameObject> item in source.Where((KeyValuePair<VRRig, GameObject> nametag) => !VRRigCache.ActiveRigs.Contains(nametag.Key)))
		{
			Object.Destroy((Object)(object)item.Value);
			idNameTags.Remove(item.Key);
		}
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			try
			{
				if (!activeRig.isLocal || selfNameTag)
				{
					if (!idNameTags.ContainsKey(activeRig))
					{
						GameObject val = new GameObject("Seralyth_IDtag");
						val.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
						TextMeshPro val2 = val.AddComponent<TextMeshPro>();
						((TMP_Text)val2).fontSize = 4.8f;
						((TMP_Text)val2).alignment = (TextAlignmentOptions)514;
						idNameTags.Add(activeRig, val);
					}
					GameObject val3 = idNameTags[activeRig];
					TextMeshPro orAddComponent = GTExt.GetOrAddComponent<TextMeshPro>(val3);
					if (NameTagOptimize())
					{
						((TMP_Text)(object)orAddComponent).SafeSetText(RigUtilities.GetPlayerFromVRRig(activeRig).UserId);
						((Graphic)orAddComponent).color = activeRig.GetColor();
						((TMP_Text)(object)orAddComponent).SafeSetFontStyle(Main.activeFontStyle);
						((TMP_Text)(object)orAddComponent).SafeSetFont(Main.activeFont);
					}
					if (nameTagChams)
					{
						((TMP_Text)(object)orAddComponent).Chams();
					}
					val3.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * activeRig.scaleFactor;
					val3.transform.position = GetNameTagPosition(activeRig);
					val3.transform.LookAt(((Component)Camera.main).transform.position);
					val3.transform.Rotate(0f, 180f, 0f);
				}
			}
			catch
			{
			}
		}
	}

	public static void DisableIDTags()
	{
		foreach (KeyValuePair<VRRig, GameObject> idNameTag in idNameTags)
		{
			Object.Destroy((Object)(object)idNameTag.Value);
		}
		idNameTags.Clear();
	}

	public static void PlatformTags()
	{
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		List<KeyValuePair<VRRig, GameObject>> source = platformTags.ToList();
		foreach (KeyValuePair<VRRig, GameObject> item in source.Where((KeyValuePair<VRRig, GameObject> nametag) => !VRRigCache.ActiveRigs.Contains(nametag.Key)))
		{
			Object.Destroy((Object)(object)item.Value);
			platformTags.Remove(item.Key);
		}
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			try
			{
				if (!activeRig.isLocal || selfNameTag)
				{
					if (!platformTags.ContainsKey(activeRig))
					{
						GameObject val = new GameObject("Seralyth_PlatformTag");
						val.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
						TextMeshPro val2 = val.AddComponent<TextMeshPro>();
						((TMP_Text)val2).fontSize = 4.8f;
						((TMP_Text)val2).alignment = (TextAlignmentOptions)514;
						platformTags.Add(activeRig, val);
					}
					GameObject val3 = platformTags[activeRig];
					TextMeshPro orAddComponent = GTExt.GetOrAddComponent<TextMeshPro>(val3);
					if (NameTagOptimize())
					{
						((TMP_Text)(object)orAddComponent).SafeSetText(activeRig.GetPlatform() ?? "");
						((Graphic)orAddComponent).color = activeRig.GetColor();
						((TMP_Text)(object)orAddComponent).SafeSetFontStyle(Main.activeFontStyle);
						((TMP_Text)(object)orAddComponent).SafeSetFont(Main.activeFont);
					}
					if (nameTagChams)
					{
						((TMP_Text)(object)orAddComponent).Chams();
					}
					val3.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * activeRig.scaleFactor;
					val3.transform.position = GetNameTagPosition(activeRig);
					val3.transform.LookAt(((Component)Camera.main).transform.position);
					val3.transform.Rotate(0f, 180f, 0f);
				}
			}
			catch
			{
			}
		}
	}

	public static void DisablePlatformTags()
	{
		foreach (KeyValuePair<VRRig, GameObject> platformTag in platformTags)
		{
			Object.Destroy((Object)(object)platformTag.Value);
		}
		platformTags.Clear();
	}

	public static void KIDNameTags()
	{
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Expected O, but got Unknown
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		List<KeyValuePair<VRRig, GameObject>> list = kidNameTags.ToList();
		foreach (KeyValuePair<VRRig, GameObject> item in list)
		{
			if (!VRRigCache.ActiveRigs.Contains(item.Key))
			{
				Object.Destroy((Object)(object)item.Value);
				kidNameTags.Remove(item.Key);
			}
			else if (!item.Key.IsKIDRestricted())
			{
				Object.Destroy((Object)(object)item.Value);
				kidNameTags.Remove(item.Key);
			}
		}
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			try
			{
				if (activeRig.isLocal && !selfNameTag)
				{
					continue;
				}
				if (!kidNameTags.ContainsKey(activeRig) && activeRig.IsKIDRestricted())
				{
					GameObject val = new GameObject("Seralyth_Kidtag");
					val.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
					TextMeshPro orAddComponent = GTExt.GetOrAddComponent<TextMeshPro>(val);
					((TMP_Text)orAddComponent).fontSize = 4.8f;
					((TMP_Text)orAddComponent).alignment = (TextAlignmentOptions)514;
					((Graphic)orAddComponent).color = Color32.op_Implicit(new Color32((byte)56, (byte)126, (byte)138, byte.MaxValue));
					((TMP_Text)(object)orAddComponent).SafeSetText("k-ID Restricted");
					((TMP_Text)(object)orAddComponent).SafeSetFontStyle(Main.activeFontStyle);
					((TMP_Text)(object)orAddComponent).SafeSetFont(Main.activeFont);
					kidNameTags.Add(activeRig, val);
				}
				if (kidNameTags.TryGetValue(activeRig, out var value))
				{
					TextMeshPro orAddComponent2 = GTExt.GetOrAddComponent<TextMeshPro>(value);
					if (nameTagChams)
					{
						((TMP_Text)(object)orAddComponent2).Chams();
					}
					value.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * activeRig.scaleFactor;
					value.transform.position = GetNameTagPosition(activeRig);
					value.transform.LookAt(((Component)Camera.main).transform.position);
					value.transform.Rotate(0f, 180f, 0f);
				}
			}
			catch
			{
			}
		}
	}

	public static void DisableKIDNameTags()
	{
		foreach (KeyValuePair<VRRig, GameObject> kidNameTag in kidNameTags)
		{
			Object.Destroy((Object)(object)kidNameTag.Value);
		}
		kidNameTags.Clear();
	}

	public static void SubscriberNameTags()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Expected O, but got Unknown
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		List<KeyValuePair<VRRig, GameObject>> list = subNameTags.ToList();
		foreach (KeyValuePair<VRRig, GameObject> item in list)
		{
			if (!VRRigCache.ActiveRigs.Contains(item.Key))
			{
				Object.Destroy((Object)(object)item.Value);
				subNameTags.Remove(item.Key);
			}
			else if (SubscriptionManager.GetSubscriptionDetails(item.Key).tier <= 0)
			{
				Object.Destroy((Object)(object)item.Value);
				subNameTags.Remove(item.Key);
			}
		}
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			try
			{
				if (activeRig.isLocal && !selfNameTag)
				{
					continue;
				}
				if (!subNameTags.ContainsKey(activeRig))
				{
					SubscriptionDetails subscriptionDetails = SubscriptionManager.GetSubscriptionDetails(activeRig);
					if (subscriptionDetails.tier > 0)
					{
						GameObject val = new GameObject("Seralyth_SubscriberTag");
						val.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
						TextMeshPro orAddComponent = GTExt.GetOrAddComponent<TextMeshPro>(val);
						((TMP_Text)orAddComponent).fontSize = 4.8f;
						((TMP_Text)orAddComponent).alignment = (TextAlignmentOptions)514;
						((Graphic)orAddComponent).color = SubscriptionManager.SUBSCRIBER_NAME_COLOR;
						((TMP_Text)(object)orAddComponent).SafeSetText("VIM Subscriber");
						((TMP_Text)(object)orAddComponent).SafeSetFontStyle(Main.activeFontStyle);
						((TMP_Text)(object)orAddComponent).SafeSetFont(Main.activeFont);
						subNameTags.Add(activeRig, val);
					}
				}
				if (subNameTags.TryGetValue(activeRig, out var value))
				{
					TextMeshPro orAddComponent2 = GTExt.GetOrAddComponent<TextMeshPro>(value);
					if (nameTagChams)
					{
						((TMP_Text)(object)orAddComponent2).Chams();
					}
					value.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * activeRig.scaleFactor;
					value.transform.position = GetNameTagPosition(activeRig);
					value.transform.LookAt(((Component)Camera.main).transform.position);
					value.transform.Rotate(0f, 180f, 0f);
				}
			}
			catch
			{
			}
		}
	}

	public static void DisableSubscriberNameTags()
	{
		foreach (KeyValuePair<VRRig, GameObject> subNameTag in subNameTags)
		{
			Object.Destroy((Object)(object)subNameTag.Value);
		}
		subNameTags.Clear();
	}

	public static void CreationDateTags()
	{
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		List<KeyValuePair<VRRig, GameObject>> source = creationDateTags.ToList();
		foreach (KeyValuePair<VRRig, GameObject> item in source.Where((KeyValuePair<VRRig, GameObject> nametag) => !VRRigCache.ActiveRigs.Contains(nametag.Key)))
		{
			Object.Destroy((Object)(object)item.Value);
			creationDateTags.Remove(item.Key);
		}
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			try
			{
				if (!activeRig.isLocal || selfNameTag)
				{
					if (!creationDateTags.ContainsKey(activeRig))
					{
						GameObject val = new GameObject("Seralyth_CreationTag");
						val.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
						TextMeshPro val2 = val.AddComponent<TextMeshPro>();
						((TMP_Text)val2).fontSize = 4.8f;
						((TMP_Text)val2).alignment = (TextAlignmentOptions)514;
						creationDateTags.Add(activeRig, val);
					}
					GameObject val3 = creationDateTags[activeRig];
					TextMeshPro orAddComponent = GTExt.GetOrAddComponent<TextMeshPro>(val3);
					if (NameTagOptimize())
					{
						((TMP_Text)(object)orAddComponent).SafeSetText(RigUtilities.GetCreationDate(RigUtilities.GetPlayerFromVRRig(activeRig).UserId));
						((Graphic)orAddComponent).color = activeRig.GetColor();
						((TMP_Text)(object)orAddComponent).SafeSetFontStyle(Main.activeFontStyle);
						((TMP_Text)(object)orAddComponent).SafeSetFont(Main.activeFont);
					}
					if (nameTagChams)
					{
						((TMP_Text)(object)orAddComponent).Chams();
					}
					val3.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * activeRig.scaleFactor;
					val3.transform.position = GetNameTagPosition(activeRig);
					val3.transform.LookAt(((Component)Camera.main).transform.position);
					val3.transform.Rotate(0f, 180f, 0f);
				}
			}
			catch
			{
			}
		}
	}

	public static void DisableCreationDateTags()
	{
		foreach (KeyValuePair<VRRig, GameObject> creationDateTag in creationDateTags)
		{
			Object.Destroy((Object)(object)creationDateTag.Value);
		}
		creationDateTags.Clear();
	}

	public static void PingTags()
	{
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		List<KeyValuePair<VRRig, GameObject>> source = pingNameTags.ToList();
		foreach (KeyValuePair<VRRig, GameObject> item in source.Where((KeyValuePair<VRRig, GameObject> nametag) => !VRRigCache.ActiveRigs.Contains(nametag.Key)))
		{
			Object.Destroy((Object)(object)item.Value);
			pingNameTags.Remove(item.Key);
		}
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			try
			{
				if (!activeRig.isLocal || selfNameTag)
				{
					if (!pingNameTags.ContainsKey(activeRig))
					{
						GameObject val = new GameObject("Seralyth_Pingtag");
						val.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
						TextMeshPro val2 = val.AddComponent<TextMeshPro>();
						((TMP_Text)val2).fontSize = 4.8f;
						((TMP_Text)val2).alignment = (TextAlignmentOptions)514;
						pingNameTags.Add(activeRig, val);
					}
					GameObject val3 = pingNameTags[activeRig];
					TextMeshPro orAddComponent = GTExt.GetOrAddComponent<TextMeshPro>(val3);
					if (NameTagOptimize())
					{
						((TMP_Text)(object)orAddComponent).SafeSetText($"{activeRig.GetPing()}ms");
						((Graphic)orAddComponent).color = activeRig.GetColor();
						((TMP_Text)(object)orAddComponent).SafeSetFontStyle(Main.activeFontStyle);
						((TMP_Text)(object)orAddComponent).SafeSetFont(Main.activeFont);
					}
					if (nameTagChams)
					{
						((TMP_Text)(object)orAddComponent).Chams();
					}
					val3.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * activeRig.scaleFactor;
					val3.transform.position = GetNameTagPosition(activeRig);
					val3.transform.LookAt(((Component)Camera.main).transform.position);
					val3.transform.Rotate(0f, 180f, 0f);
				}
			}
			catch
			{
			}
		}
	}

	public static void DisablePingTags()
	{
		foreach (KeyValuePair<VRRig, GameObject> pingNameTag in pingNameTags)
		{
			Object.Destroy((Object)(object)pingNameTag.Value);
		}
		pingNameTags.Clear();
	}

	public static void TurnTags()
	{
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		List<KeyValuePair<VRRig, GameObject>> source = turnNameTags.ToList();
		foreach (KeyValuePair<VRRig, GameObject> item in source.Where((KeyValuePair<VRRig, GameObject> nametag) => !VRRigCache.ActiveRigs.Contains(nametag.Key)))
		{
			Object.Destroy((Object)(object)item.Value);
			turnNameTags.Remove(item.Key);
		}
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			try
			{
				if (!activeRig.isLocal || selfNameTag)
				{
					if (!turnNameTags.ContainsKey(activeRig))
					{
						GameObject val = new GameObject("Seralyth_Turntag");
						val.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
						TextMeshPro val2 = val.AddComponent<TextMeshPro>();
						((TMP_Text)val2).fontSize = 4.8f;
						((TMP_Text)val2).alignment = (TextAlignmentOptions)514;
						turnNameTags.Add(activeRig, val);
					}
					string turnType = activeRig.turnType;
					int turnFactor = activeRig.turnFactor;
					GameObject val3 = turnNameTags[activeRig];
					TextMeshPro orAddComponent = GTExt.GetOrAddComponent<TextMeshPro>(val3);
					if (NameTagOptimize())
					{
						((TMP_Text)(object)orAddComponent).SafeSetText((turnType == "NONE") ? "None" : (Main.ToTitleCase(turnType) + " " + turnFactor));
						((Graphic)orAddComponent).color = activeRig.GetColor();
						((TMP_Text)(object)orAddComponent).SafeSetFontStyle(Main.activeFontStyle);
						((TMP_Text)(object)orAddComponent).SafeSetFont(Main.activeFont);
					}
					if (nameTagChams)
					{
						((TMP_Text)(object)orAddComponent).Chams();
					}
					val3.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * activeRig.scaleFactor;
					val3.transform.position = GetNameTagPosition(activeRig);
					val3.transform.LookAt(((Component)Camera.main).transform.position);
					val3.transform.Rotate(0f, 180f, 0f);
				}
			}
			catch
			{
			}
		}
	}

	public static void DisableTurnTags()
	{
		foreach (KeyValuePair<VRRig, GameObject> turnNameTag in turnNameTags)
		{
			Object.Destroy((Object)(object)turnNameTag.Value);
		}
		turnNameTags.Clear();
	}

	public static void TaggedTags()
	{
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		List<KeyValuePair<VRRig, GameObject>> source = taggedNameTags.ToList();
		foreach (KeyValuePair<VRRig, GameObject> item in source.Where((KeyValuePair<VRRig, GameObject> nametag) => !VRRigCache.ActiveRigs.Contains(nametag.Key)))
		{
			Object.Destroy((Object)(object)item.Value);
			taggedNameTags.Remove(item.Key);
		}
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			try
			{
				if (activeRig.isLocal && !selfNameTag)
				{
					continue;
				}
				if (!taggedNameTags.ContainsKey(activeRig))
				{
					GameObject val = new GameObject("Seralyth_Taggedtag");
					val.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
					TextMeshPro val2 = val.AddComponent<TextMeshPro>();
					((TMP_Text)val2).fontSize = 4.8f;
					((TMP_Text)val2).alignment = (TextAlignmentOptions)514;
					taggedNameTags.Add(activeRig, val);
				}
				GameObject val3 = taggedNameTags[activeRig];
				TextMeshPro orAddComponent = GTExt.GetOrAddComponent<TextMeshPro>(val3);
				if (NameTagOptimize())
				{
					if (activeRig.IsTagged())
					{
						int taggedById = activeRig.taggedById;
						NetPlayer val4 = NetPlayer.op_Implicit(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(taggedById, false));
						if (val4 != null)
						{
							((TMP_Text)(object)orAddComponent).SafeSetText("Tagged by " + ((val4 != null) ? val4.NickName : null));
						}
					}
					else
					{
						((TMP_Text)(object)orAddComponent).SafeSetText("");
					}
					((Graphic)orAddComponent).color = activeRig.GetColor();
					((TMP_Text)(object)orAddComponent).SafeSetFontStyle(Main.activeFontStyle);
					((TMP_Text)(object)orAddComponent).SafeSetFont(Main.activeFont);
				}
				if (nameTagChams)
				{
					((TMP_Text)(object)orAddComponent).Chams();
				}
				val3.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * activeRig.scaleFactor;
				val3.transform.position = GetNameTagPosition(activeRig);
				val3.transform.LookAt(((Component)Camera.main).transform.position);
				val3.transform.Rotate(0f, 180f, 0f);
			}
			catch
			{
			}
		}
	}

	public static void DisableTaggedTags()
	{
		foreach (KeyValuePair<VRRig, GameObject> taggedNameTag in taggedNameTags)
		{
			Object.Destroy((Object)(object)taggedNameTag.Value);
		}
		taggedNameTags.Clear();
	}

	public unsafe static void ModTags()
	{
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		List<KeyValuePair<VRRig, GameObject>> source = modNameTags.ToList();
		foreach (KeyValuePair<VRRig, GameObject> item in source.Where((KeyValuePair<VRRig, GameObject> nametag) => !VRRigCache.ActiveRigs.Contains(nametag.Key)))
		{
			Object.Destroy((Object)(object)item.Value);
			modNameTags.Remove(item.Key);
		}
		foreach (VRRig vrrig in VRRigCache.ActiveRigs)
		{
			try
			{
				if (vrrig.isLocal && !selfNameTag)
				{
					continue;
				}
				if (!modNameTags.ContainsKey(vrrig))
				{
					GameObject val = new GameObject("Seralyth_Modtag");
					val.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
					TextMeshPro val2 = val.AddComponent<TextMeshPro>();
					((TMP_Text)val2).fontSize = 4.8f;
					((TMP_Text)val2).alignment = (TextAlignmentOptions)514;
					modNameTags.Add(vrrig, val);
				}
				GameObject val3 = modNameTags[vrrig];
				TextMeshPro orAddComponent = GTExt.GetOrAddComponent<TextMeshPro>(val3);
				if (NameTagOptimize())
				{
					string text = null;
					Dictionary<string, object> customProps = new Dictionary<string, object>();
					DictionaryEntryEnumerator enumerator3 = vrrig.GetPhotonPlayer().CustomProperties.GetEnumerator();
					try
					{
						while (((DictionaryEntryEnumerator)(ref enumerator3)).MoveNext())
						{
							DictionaryEntry current2 = ((DictionaryEntryEnumerator)(ref enumerator3)).Current;
							customProps[current2.Key.ToString().ToLower()] = current2.Value;
						}
					}
					finally
					{
						((IDisposable)(*(DictionaryEntryEnumerator*)(&enumerator3))/*cast due to .constrained prefix*/).Dispose();
					}
					foreach (KeyValuePair<string, string> item2 in modDictionary.Where((KeyValuePair<string, string> mod) => customProps.ContainsKey(mod.Key.ToLower())))
					{
						text = ((text != null) ? ((!text.Contains("&")) ? (text + " & " + item2.Value) : (item2.Value + ", " + text)) : item2.Value);
					}
					CosmeticSet cosmeticSet = vrrig.cosmeticSet;
					if (cosmeticSet.items.Any((CosmeticItem cosmetic) => !cosmetic.isNullItem && !vrrig.Cosmetics().Contains(cosmetic.itemName)))
					{
						text = ((text == null) ? "Cosmetx" : ((!text.Contains("&")) ? (text + " & Cosmetx") : ("Cosmetx, " + text)));
					}
					((TMP_Text)(object)orAddComponent).SafeSetText(text);
					((Graphic)orAddComponent).color = vrrig.GetColor();
					((TMP_Text)(object)orAddComponent).SafeSetFontStyle(Main.activeFontStyle);
					((TMP_Text)(object)orAddComponent).SafeSetFont(Main.activeFont);
					if (nameTagChams)
					{
						((TMP_Text)(object)orAddComponent).Chams();
					}
				}
				if (!string.IsNullOrEmpty(((TMP_Text)orAddComponent).text))
				{
					val3.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * vrrig.scaleFactor;
					val3.transform.position = GetNameTagPosition(vrrig);
					val3.transform.LookAt(((Component)Camera.main).transform.position);
					val3.transform.Rotate(0f, 180f, 0f);
				}
			}
			catch
			{
			}
		}
	}

	public static void DisableModTags()
	{
		foreach (KeyValuePair<VRRig, GameObject> modNameTag in modNameTags)
		{
			Object.Destroy((Object)(object)modNameTag.Value);
		}
		modNameTags.Clear();
	}

	public static void CosmeticTags()
	{
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Expected O, but got Unknown
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		List<KeyValuePair<VRRig, GameObject>> source = cosmeticNameTags.ToList();
		foreach (KeyValuePair<VRRig, GameObject> item in source.Where((KeyValuePair<VRRig, GameObject> nametag) => !VRRigCache.ActiveRigs.Contains(nametag.Key)))
		{
			Object.Destroy((Object)(object)item.Value);
			cosmeticNameTags.Remove(item.Key);
		}
		foreach (VRRig vrrig in VRRigCache.ActiveRigs)
		{
			try
			{
				if ((vrrig.isLocal && !selfNameTag) || cosmetics != null)
				{
					continue;
				}
				if (!cosmeticNameTags.ContainsKey(vrrig))
				{
					GameObject val = new GameObject("Seralyth_Modtag");
					val.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
					TextMeshPro val2 = val.AddComponent<TextMeshPro>();
					((TMP_Text)val2).fontSize = 4.8f;
					((TMP_Text)val2).alignment = (TextAlignmentOptions)514;
					cosmeticNameTags.Add(vrrig, val);
				}
				GameObject val3 = cosmeticNameTags[vrrig];
				TextMeshPro orAddComponent = GTExt.GetOrAddComponent<TextMeshPro>(val3);
				if (NameTagOptimize())
				{
					string text = null;
					foreach (KeyValuePair<string, string> item2 in specialCosmetics.Where((KeyValuePair<string, string> cosmetic) => vrrig.Cosmetics().Contains(cosmetic.Key)))
					{
						text = ((text != null) ? ((!text.Contains("&")) ? (text + " & " + item2.Value) : (item2.Value + ", " + text)) : item2.Value);
					}
					((TMP_Text)(object)orAddComponent).SafeSetText(text);
					((Graphic)orAddComponent).color = vrrig.GetColor();
					((TMP_Text)(object)orAddComponent).SafeSetFontStyle(Main.activeFontStyle);
					((TMP_Text)(object)orAddComponent).SafeSetFont(Main.activeFont);
				}
				if (nameTagChams)
				{
					((TMP_Text)(object)orAddComponent).Chams();
				}
				val3.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * vrrig.scaleFactor;
				val3.transform.position = GetNameTagPosition(vrrig);
				val3.transform.LookAt(((Component)Camera.main).transform.position);
				val3.transform.Rotate(0f, 180f, 0f);
			}
			catch
			{
			}
		}
	}

	public static void DisableCosmeticTags()
	{
		foreach (KeyValuePair<VRRig, GameObject> cosmeticNameTag in cosmeticNameTags)
		{
			Object.Destroy((Object)(object)cosmeticNameTag.Value);
		}
		cosmeticNameTags.Clear();
	}

	public static void VerifiedTags()
	{
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Expected O, but got Unknown
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		List<KeyValuePair<VRRig, GameObject>> source = verifiedNameTags.ToList();
		foreach (KeyValuePair<VRRig, GameObject> item in source.Where((KeyValuePair<VRRig, GameObject> nametag) => !VRRigCache.ActiveRigs.Contains(nametag.Key)))
		{
			Object.Destroy((Object)(object)item.Value);
			verifiedNameTags.Remove(item.Key);
		}
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			try
			{
				if (activeRig.isLocal && !selfNameTag)
				{
					continue;
				}
				if (!verifiedNameTags.ContainsKey(activeRig))
				{
					string userId = RigUtilities.GetPlayerFromVRRig(activeRig).UserId;
					string value2;
					if (verifiedDictionary.TryGetValue(userId, out var value))
					{
						GameObject val = new GameObject("Seralyth_Verifiedtag");
						val.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
						TextMeshPro orAddComponent = GTExt.GetOrAddComponent<TextMeshPro>(val);
						((TMP_Text)orAddComponent).fontSize = 4.8f;
						((TMP_Text)orAddComponent).alignment = (TextAlignmentOptions)514;
						((TMP_Text)(object)orAddComponent).SafeSetText(value);
						verifiedNameTags.Add(activeRig, val);
					}
					else if (ServerData.Administrators.TryGetValue(userId, out value2))
					{
						GameObject val2 = new GameObject("Seralyth_Verifiedtag");
						val2.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
						TextMeshPro orAddComponent2 = GTExt.GetOrAddComponent<TextMeshPro>(val2);
						((TMP_Text)orAddComponent2).fontSize = 4.8f;
						((TMP_Text)orAddComponent2).alignment = (TextAlignmentOptions)514;
						((TMP_Text)(object)orAddComponent2).SafeSetText(value2);
						verifiedNameTags.Add(activeRig, val2);
					}
				}
				if (verifiedNameTags.TryGetValue(activeRig, out var value3))
				{
					TextMeshPro orAddComponent3 = GTExt.GetOrAddComponent<TextMeshPro>(value3);
					((Graphic)orAddComponent3).color = activeRig.GetColor();
					if (NameTagOptimize())
					{
						((TMP_Text)(object)orAddComponent3).SafeSetFontStyle(Main.activeFontStyle);
						((TMP_Text)(object)orAddComponent3).SafeSetFont(Main.activeFont);
					}
					if (nameTagChams)
					{
						((TMP_Text)(object)orAddComponent3).Chams();
					}
					value3.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * activeRig.scaleFactor;
					value3.transform.position = GetNameTagPosition(activeRig);
					value3.transform.LookAt(((Component)Camera.main).transform.position);
					value3.transform.Rotate(0f, 180f, 0f);
				}
			}
			catch
			{
			}
		}
	}

	public static void DisableVerifiedTags()
	{
		foreach (KeyValuePair<VRRig, GameObject> verifiedNameTag in verifiedNameTags)
		{
			Object.Destroy((Object)(object)verifiedNameTag.Value);
		}
		verifiedNameTags.Clear();
	}

	public static void CrashedTags()
	{
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Expected O, but got Unknown
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		List<KeyValuePair<VRRig, GameObject>> list = crashedNameTags.ToList();
		foreach (KeyValuePair<VRRig, GameObject> item in list)
		{
			if (!VRRigCache.ActiveRigs.Contains(item.Key))
			{
				Object.Destroy((Object)(object)item.Value);
				crashedNameTags.Remove(item.Key);
			}
			else if (item.Key.GetTruePing() <= 500)
			{
				Object.Destroy((Object)(object)item.Value);
				crashedNameTags.Remove(item.Key);
			}
		}
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			try
			{
				if (activeRig.isLocal && !selfNameTag)
				{
					continue;
				}
				if (!crashedNameTags.ContainsKey(activeRig))
				{
					int truePing = activeRig.GetTruePing();
					if (truePing > 500)
					{
						Color color = Color.yellow;
						if (truePing > 5000)
						{
							color = Color.black;
						}
						else if (truePing > 2500)
						{
							color = Color.red;
						}
						else if (truePing > 1500)
						{
							color = Color32.op_Implicit(new Color32(byte.MaxValue, (byte)128, (byte)0, byte.MaxValue));
						}
						GameObject val = new GameObject("Seralyth_Crashedtag");
						val.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
						TextMeshPro orAddComponent = GTExt.GetOrAddComponent<TextMeshPro>(val);
						((TMP_Text)orAddComponent).fontSize = 4.8f;
						((TMP_Text)orAddComponent).alignment = (TextAlignmentOptions)514;
						((Graphic)orAddComponent).color = color;
						((TMP_Text)(object)orAddComponent).SafeSetText("Lagging");
						((TMP_Text)(object)orAddComponent).SafeSetFontStyle(Main.activeFontStyle);
						((TMP_Text)(object)orAddComponent).SafeSetFont(Main.activeFont);
						crashedNameTags.Add(activeRig, val);
					}
				}
				if (crashedNameTags.TryGetValue(activeRig, out var value))
				{
					double num = Math.Abs(activeRig.velocityHistoryList[0].time * 1000.0 - (double)PhotonNetwork.ServerTimestamp);
					Color color2 = Color.yellow;
					if (num > 2500.0)
					{
						color2 = Color.red;
					}
					else if (num > 1500.0)
					{
						color2 = Color32.op_Implicit(new Color32(byte.MaxValue, (byte)128, (byte)0, byte.MaxValue));
					}
					TextMeshPro orAddComponent2 = GTExt.GetOrAddComponent<TextMeshPro>(value);
					((Graphic)orAddComponent2).color = color2;
					if (nameTagChams)
					{
						((TMP_Text)(object)orAddComponent2).Chams();
					}
					value.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * activeRig.scaleFactor;
					value.transform.position = GetNameTagPosition(activeRig);
					value.transform.LookAt(((Component)Camera.main).transform.position);
					value.transform.Rotate(0f, 180f, 0f);
				}
			}
			catch
			{
			}
		}
	}

	public static void DisableCrashedTags()
	{
		foreach (KeyValuePair<VRRig, GameObject> crashedNameTag in crashedNameTags)
		{
			Object.Destroy((Object)(object)crashedNameTag.Value);
		}
		crashedNameTags.Clear();
	}

	public static string GetPrettyPlatform(VRRig vrrig)
	{
		string platform = vrrig.GetPlatform();
		if (1 == 0)
		{
		}
		string result = platform switch
		{
			"PC" => "<color=#00FFFF>PC</color>", 
			"Steam" => "<color=blue>Steam</color>", 
			"Standalone" => "<color=green>Standalone</color>", 
			_ => platform, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public static string GetPrettyPing(VRRig vrrig)
	{
		int ping = vrrig.GetPing();
		return (ping < 300) ? $"<color=green>{ping}</color>" : ((ping < 1000) ? $"<color=yellow>{ping}</color>" : $"<color=red>{ping}</color>");
	}

	public static string GetPrettyFPS(VRRig vrrig)
	{
		int fps = vrrig.fps;
		return (fps < 30) ? $"<color=red>{fps}</color>" : ((fps < 60) ? $"<color=yellow>{fps}</color>" : $"<color=green>{fps}</color>");
	}

	public static void CompactTags()
	{
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected O, but got Unknown
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Expected O, but got Unknown
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Expected O, but got Unknown
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Expected O, but got Unknown
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Expected O, but got Unknown
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Expected O, but got Unknown
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Unknown result type (might be due to invalid IL or missing references)
		//IL_0678: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06df: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Unknown result type (might be due to invalid IL or missing references)
		//IL_073a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0788: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_080a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0817: Unknown result type (might be due to invalid IL or missing references)
		//IL_081c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0825: Unknown result type (might be due to invalid IL or missing references)
		//IL_083e: Unknown result type (might be due to invalid IL or missing references)
		//IL_086c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0875: Unknown result type (might be due to invalid IL or missing references)
		//IL_087f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0884: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a0: Unknown result type (might be due to invalid IL or missing references)
		bool enabled = Buttons.GetIndex("Hidden on Camera").enabled;
		List<KeyValuePair<VRRig, GameObject>> source = compactNameTags.ToList();
		foreach (KeyValuePair<VRRig, GameObject> item in source.Where((KeyValuePair<VRRig, GameObject> nametag) => !VRRigCache.ActiveRigs.Contains(nametag.Key)))
		{
			Object.Destroy((Object)(object)item.Value);
			Object.Destroy((Object)(object)compactTagBackgrounds[item.Key]);
			compactNameTags.Remove(item.Key);
			compactTagBackgrounds.Remove(item.Key);
		}
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			try
			{
				if (activeRig.isLocal && !selfNameTag)
				{
					continue;
				}
				if (!compactNameTags.ContainsKey(activeRig))
				{
					GameObject val = new GameObject("seralyth_vrctag_text");
					if (enabled)
					{
						val.layer = 19;
					}
					GameObject val2 = new GameObject("infotag");
					val2.transform.parent = val.transform;
					val2.transform.localPosition = new Vector3(0f, 0.5f, 0f);
					val2.transform.localScale = Vector3.one;
					TextMeshPro val3 = val2.AddComponent<TextMeshPro>();
					((TMP_Text)val3).fontSize = 2.4f;
					((TMP_Text)val3).alignment = (TextAlignmentOptions)514;
					((Graphic)val3).color = Color.white;
					((TMP_Text)val3).richText = true;
					GameObject val4 = new GameObject("nametag");
					val4.transform.parent = val.transform;
					val4.transform.localPosition = Vector3.zero;
					val4.transform.localScale = Vector3.one;
					TextMeshPro val5 = val4.AddComponent<TextMeshPro>();
					((TMP_Text)val5).fontSize = 3.2f;
					((TMP_Text)val5).alignment = (TextAlignmentOptions)514;
					((TMP_Text)val5).richText = true;
					GameObject val6 = new GameObject("seralyth_vrctag_background");
					if (enabled)
					{
						val6.layer = 19;
					}
					GameObject val7 = new GameObject("infobg");
					val7.transform.parent = val6.transform;
					val7.transform.localPosition = new Vector3(0f, 0.5f, 0f);
					val7.transform.localScale = Vector3.one;
					val7.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
					Object.Destroy((Object)(object)val7.GetComponent<Collider>());
					LineRenderer val8 = val7.AddComponent<LineRenderer>();
					((Renderer)val8).material.shader = (nameTagChams ? AssetUtilities.LoadAsset<Shader>("Chams") : Shader.Find("Sprites/Default"));
					((Renderer)val8).material.color = new Color(0.2f, 0.2f, 0.2f, 0.6f);
					val8.numCapVertices = 10;
					val8.numCornerVertices = 5;
					val8.useWorldSpace = false;
					val8.positionCount = 2;
					val8.startWidth = 0.05f;
					val8.endWidth = 0.05f;
					GameObject val9 = new GameObject("namebg");
					val9.transform.parent = val6.transform;
					val9.transform.localPosition = Vector3.zero;
					val9.transform.localScale = Vector3.one;
					val9.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
					Object.Destroy((Object)(object)val9.GetComponent<Collider>());
					LineRenderer val10 = val9.AddComponent<LineRenderer>();
					((Renderer)val10).material.shader = (nameTagChams ? AssetUtilities.LoadAsset<Shader>("Chams") : Shader.Find("Sprites/Default"));
					((Renderer)val10).material.color = new Color(0.2f, 0.2f, 0.2f, 0.6f);
					val10.numCapVertices = 10;
					val10.numCornerVertices = 5;
					val10.useWorldSpace = false;
					val10.positionCount = 2;
					val10.startWidth = 0.1f;
					val10.endWidth = 0.1f;
					compactNameTags.Add(activeRig, val);
					compactTagBackgrounds.Add(activeRig, val6);
				}
				GameObject val11 = compactNameTags[activeRig];
				GameObject val12 = compactTagBackgrounds[activeRig];
				Transform val13 = val11.transform.Find("infotag");
				Transform val14 = val11.transform.Find("nametag");
				Transform val15 = val12.transform.Find("infobg");
				Transform val16 = val12.transform.Find("namebg");
				string text = ((activeRig.GetTruePing() > 2500 && !activeRig.IsLocal()) ? "[<color=red>Crashed</color>] | " : "") + "[<color=#00FFFF>" + RigUtilities.GetCreationDate(activeRig.GetPlayer().UserId, null, "MMM dd, yyyy") + "</color>] | [" + GetPrettyPlatform(activeRig) + "] | [Ping: " + GetPrettyPing(activeRig) + "] | [FPS: " + GetPrettyFPS(activeRig) + "]" + ((SubscriptionManager.GetSubscriptionDetails(activeRig).tier > 0) ? " | [<color=yellow>VIM Subscriber</color>]" : "") + (activeRig.GetPlayer().IsMasterClient ? " | [<color=#00FFFF>Master</color>]" : "");
				if (NameTagOptimize())
				{
					((TMP_Text)(object)((Component)val13).GetComponent<TextMeshPro>()).SafeSetText(text);
					((TMP_Text)(object)((Component)val13).GetComponent<TextMeshPro>()).SafeSetFontStyle(Main.activeFontStyle);
					((TMP_Text)(object)((Component)val13).GetComponent<TextMeshPro>()).SafeSetFont(Main.activeFont);
				}
				TextMeshPro component = ((Component)val13).GetComponent<TextMeshPro>();
				if (nameTagChams)
				{
					((TMP_Text)(object)component).Chams();
				}
				string text2 = Regex.Replace(text, "<.*?>", string.Empty);
				float num = ((TMP_Text)component).GetPreferredValues(text2).x * 0.65f;
				float num2 = num + 0.15f;
				string text3 = Main.CleanPlayerName(activeRig.GetPlayer().NickName);
				TextMeshPro component2 = ((Component)val14).GetComponent<TextMeshPro>();
				if (NameTagOptimize())
				{
					((TMP_Text)(object)component2).SafeSetText(text3);
					((TMP_Text)(object)component2).SafeSetFontStyle(Main.activeFontStyle);
					((TMP_Text)(object)component2).SafeSetFont(Main.activeFont);
				}
				if (nameTagChams)
				{
					((TMP_Text)(object)component2).Chams();
				}
				((Graphic)component2).color = activeRig.playerColor;
				float num3 = ((TMP_Text)component2).GetPreferredValues(text3).x * 0.5f;
				float num4 = num3 + 0.2f;
				Color color = Main.DarkenColor(activeRig.playerColor);
				color.a = 0.6f;
				((Renderer)((Component)val16).GetComponent<LineRenderer>()).material.color = color;
				float num5 = 0.15f * activeRig.scaleFactor;
				val11.transform.localScale = new Vector3(num5, num5, num5);
				val12.transform.localScale = new Vector3(num5, num5, num5);
				LineRenderer component3 = ((Component)val15).GetComponent<LineRenderer>();
				component3.startWidth = 0.05f * activeRig.scaleFactor;
				component3.endWidth = 0.05f * activeRig.scaleFactor;
				component3.SetPosition(0, new Vector3(0f, 0f - num2, 0f));
				component3.SetPosition(1, new Vector3(0f, num2, 0f));
				LineRenderer component4 = ((Component)val16).GetComponent<LineRenderer>();
				component4.startWidth = 0.1f * activeRig.scaleFactor;
				component4.endWidth = 0.1f * activeRig.scaleFactor;
				component4.SetPosition(0, new Vector3(0f, 0f - num4, 0f));
				component4.SetPosition(1, new Vector3(0f, num4, 0f));
				Vector3 nameTagPosition = GetNameTagPosition(activeRig);
				val11.transform.position = nameTagPosition;
				val11.transform.LookAt(((Component)Camera.main).transform.position);
				val11.transform.Rotate(0f, 180f, 0f);
				val12.transform.position = nameTagPosition - val11.transform.forward * 0.01f;
				val12.transform.LookAt(((Component)Camera.main).transform.position);
				val12.transform.Rotate(0f, 180f, 0f);
			}
			catch
			{
			}
		}
	}

	public static void DisableCompactTags()
	{
		foreach (KeyValuePair<VRRig, GameObject> compactNameTag in compactNameTags)
		{
			Object.Destroy((Object)(object)compactNameTag.Value);
		}
		foreach (KeyValuePair<VRRig, GameObject> compactTagBackground in compactTagBackgrounds)
		{
			Object.Destroy((Object)(object)compactTagBackground.Value);
		}
		compactNameTags.Clear();
		compactTagBackgrounds.Clear();
	}

	public static void MinecraftTags()
	{
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Expected O, but got Unknown
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Expected O, but got Unknown
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		bool enabled = Buttons.GetIndex("Hidden on Camera").enabled;
		List<KeyValuePair<VRRig, GameObject>> source = minecraftNameTags.ToList();
		foreach (KeyValuePair<VRRig, GameObject> item in source.Where((KeyValuePair<VRRig, GameObject> tag) => !VRRigCache.ActiveRigs.Contains(tag.Key)))
		{
			Object.Destroy((Object)(object)item.Value);
			Object.Destroy((Object)(object)minecraftTagBackgrounds[item.Key]);
			minecraftNameTags.Remove(item.Key);
			minecraftTagBackgrounds.Remove(item.Key);
		}
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (activeRig.isLocal && !selfNameTag)
			{
				continue;
			}
			if (!minecraftNameTags.ContainsKey(activeRig))
			{
				GameObject val = new GameObject("Seralyth_MinecraftTag");
				if (enabled)
				{
					val.layer = 19;
				}
				GameObject val2 = new GameObject("text");
				val2.transform.SetParent(val.transform, false);
				TextMeshPro val3 = val2.AddComponent<TextMeshPro>();
				((TMP_Text)val3).fontSize = 2.6f;
				((TMP_Text)val3).alignment = (TextAlignmentOptions)514;
				((Graphic)val3).color = Color.white;
				((TMP_Text)val3).richText = false;
				GameObject val4 = GameObject.CreatePrimitive((PrimitiveType)5);
				Object.Destroy((Object)(object)val4.GetComponent<Collider>());
				((Object)val4).name = "bg";
				val4.transform.SetParent(val.transform, false);
				val4.transform.localPosition = new Vector3(0f, 0f, 0.01f);
				val4.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
				val4.GetComponent<Renderer>().material.color = Color32.op_Implicit(new Color32((byte)0, (byte)0, (byte)0, (byte)128));
				minecraftNameTags.Add(activeRig, val);
				minecraftTagBackgrounds.Add(activeRig, val4);
			}
			GameObject val5 = minecraftNameTags[activeRig];
			GameObject val6 = minecraftTagBackgrounds[activeRig];
			TextMeshPro component = ((Component)val5.transform.Find("text")).GetComponent<TextMeshPro>();
			if (NameTagOptimize())
			{
				((TMP_Text)(object)component).SafeSetText(Main.CleanPlayerName(RigUtilities.GetPlayerFromVRRig(activeRig).NickName));
				((TMP_Text)(object)component).SafeSetFontStyle((FontStyles)0);
				((TMP_Text)(object)component).SafeSetFont(Main.Minecraft);
				((TMP_Text)component).margin = Vector4.zero;
			}
			if (nameTagChams)
			{
				((TMP_Text)(object)component).Chams();
			}
			Bounds textBounds = ((TMP_Text)component).textBounds;
			Vector2 val7 = Vector2.op_Implicit(((Bounds)(ref textBounds)).size);
			float num = 0.1f;
			float num2 = 0.03f;
			val6.transform.localScale = new Vector3(val7.x + num, val7.y + num2, 1f);
			Transform transform = val6.transform;
			textBounds = ((TMP_Text)component).textBounds;
			float x = ((Bounds)(ref textBounds)).center.x;
			textBounds = ((TMP_Text)component).textBounds;
			transform.localPosition = new Vector3(x, ((Bounds)(ref textBounds)).center.y, 0.01f);
			val5.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f) * activeRig.scaleFactor;
			Vector3 nameTagPosition = GetNameTagPosition(activeRig);
			val5.transform.position = nameTagPosition;
			val5.transform.LookAt(((Component)Camera.main).transform.position);
			val5.transform.Rotate(0f, 180f, 0f);
			val5.layer = (enabled ? 19 : val5.layer);
			val6.layer = (enabled ? 19 : val6.layer);
		}
	}

	public static void DisableMinecraftTags()
	{
		foreach (KeyValuePair<VRRig, GameObject> minecraftNameTag in minecraftNameTags)
		{
			Object.Destroy((Object)(object)minecraftNameTag.Value);
		}
		foreach (KeyValuePair<VRRig, GameObject> minecraftTagBackground in minecraftTagBackgrounds)
		{
			Object.Destroy((Object)(object)minecraftTagBackground.Value);
		}
		minecraftNameTags.Clear();
		minecraftTagBackgrounds.Clear();
	}

	public static void CastingTags()
	{
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		bool enabled = Buttons.GetIndex("Hidden on Camera").enabled;
		List<KeyValuePair<VRRig, GameObject>> source = castingNameTags.ToList();
		foreach (KeyValuePair<VRRig, GameObject> item in source.Where((KeyValuePair<VRRig, GameObject> nametag) => !VRRigCache.ActiveRigs.Contains(nametag.Key)))
		{
			Object.Destroy((Object)(object)item.Value);
			castingNameTags.Remove(item.Key);
		}
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			try
			{
				if (!activeRig.isLocal || selfNameTag)
				{
					if (!castingNameTags.ContainsKey(activeRig))
					{
						GameObject val = new GameObject("Seralyth_CastingTag");
						val.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
						TextMeshPro val2 = val.AddComponent<TextMeshPro>();
						((TMP_Text)val2).fontSize = 4.8f;
						((TMP_Text)val2).alignment = (TextAlignmentOptions)514;
						((TMP_Text)val2).richText = true;
						((TMP_Text)val2).spriteAsset = InfoSprites;
						castingNameTags.Add(activeRig, val);
					}
					GameObject val3 = castingNameTags[activeRig];
					TextMeshPro orAddComponent = GTExt.GetOrAddComponent<TextMeshPro>(val3);
					if (enabled)
					{
						val3.layer = 19;
					}
					else
					{
						val3.layer = 0;
					}
					if (NameTagOptimize())
					{
						string text = "<size=120%><sprite name=\"" + activeRig.GetPlatform() + "\"></size>";
						string text2 = Main.CleanPlayerName(RigUtilities.GetPlayerFromVRRig(activeRig).NickName);
						((TMP_Text)(object)orAddComponent).SafeSetText(text + "<space=-0.2em>" + text2);
						((Graphic)orAddComponent).color = Color.white;
						((TMP_Text)(object)orAddComponent).SafeSetFontStyle(Main.activeFontStyle);
						((TMP_Text)(object)orAddComponent).SafeSetFont(Main.activeFont);
					}
					if (nameTagChams)
					{
						((TMP_Text)(object)orAddComponent).Chams();
					}
					val3.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * activeRig.scaleFactor;
					val3.transform.position = GetNameTagPosition(activeRig);
					val3.transform.LookAt(((Component)Camera.main).transform.position);
					val3.transform.Rotate(0f, 180f, 0f);
				}
			}
			catch
			{
			}
		}
	}

	public static void DisableCastingTags()
	{
		foreach (KeyValuePair<VRRig, GameObject> castingNameTag in castingNameTags)
		{
			Object.Destroy((Object)(object)castingNameTag.Value);
		}
		castingNameTags.Clear();
	}

	public static void FixRigColors()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig vrrig) => ((Object)((Renderer)vrrig.mainSkin).material).name.Contains("gorilla_body") && (Object)(object)((Renderer)vrrig.mainSkin).material.shader == (Object)(object)Shader.Find("GorillaTag/UberShader")))
		{
			((Renderer)item.mainSkin).material.color = item.playerColor;
		}
	}

	public static void EnableRemoveLeaves()
	{
		GameObject val = Main.GetObject("Environment Objects/LocalObjects_Prefab/Forest");
		if ((Object)(object)val != (Object)null)
		{
			for (int i = 0; i < val.transform.childCount; i++)
			{
				GameObject gameObject = ((Component)val.transform.GetChild(i)).gameObject;
				if (((Object)gameObject).name.Contains(LeavesName))
				{
					gameObject.SetActive(false);
					leaves.Add(gameObject);
				}
			}
		}
		GameObject val2 = Main.GetObject("RankedMain/Ranked_Layout/Ranked_Forest_prefab");
		if (!((Object)(object)val2 != (Object)null))
		{
			return;
		}
		for (int j = 0; j < val2.transform.childCount; j++)
		{
			GameObject gameObject2 = ((Component)val2.transform.GetChild(j)).gameObject;
			if (((Object)gameObject2).name.Contains(LeavesName))
			{
				gameObject2.SetActive(false);
				leaves.Add(gameObject2);
			}
		}
	}

	public static void DisableRemoveLeaves()
	{
		foreach (GameObject leaf in leaves)
		{
			leaf.SetActive(true);
		}
		leaves.Clear();
	}

	public static void EnableStreamerRemoveLeaves()
	{
		GameObject val = Main.GetObject("Environment Objects/LocalObjects_Prefab/Forest");
		if ((Object)(object)val != (Object)null)
		{
			for (int i = 0; i < val.transform.childCount; i++)
			{
				GameObject gameObject = ((Component)val.transform.GetChild(i)).gameObject;
				if (((Object)gameObject).name.Contains(LeavesName))
				{
					gameObject.layer = 21;
					leaves.Add(gameObject);
				}
			}
		}
		GameObject val2 = Main.GetObject("RankedMain/Ranked_Layout/Ranked_Forest_prefab");
		if (!((Object)(object)val2 != (Object)null))
		{
			return;
		}
		for (int j = 0; j < val2.transform.childCount; j++)
		{
			GameObject gameObject2 = ((Component)val2.transform.GetChild(j)).gameObject;
			if (((Object)gameObject2).name.Contains(LeavesName))
			{
				gameObject2.layer = 21;
				leaves.Add(gameObject2);
			}
		}
	}

	public static void DisableStreamerRemoveLeaves()
	{
		foreach (GameObject leaf in leaves)
		{
			leaf.layer = 0;
		}
		leaves.Clear();
	}

	public static void DisableCosmetics()
	{
		try
		{
			Transform val = VRRig.LocalRig.mainCamera.transform.Find("HeadCosmetics");
			Transform val2 = ((Component)VRRig.LocalRig).transform.Find("rig/head");
			foreach (GameObject cosmetic in VRRig.LocalRig.cosmetics)
			{
				if (cosmetic.activeSelf && ((Object)(object)cosmetic.transform.parent == (Object)(object)val || (Object)(object)cosmetic.transform.parent == (Object)(object)val2))
				{
					cosmetics.Add(cosmetic);
					cosmetic.SetActive(false);
				}
			}
		}
		catch
		{
		}
	}

	public static void EnableCosmetics()
	{
		foreach (GameObject cosmetic in cosmetics)
		{
			cosmetic.SetActive(true);
		}
		cosmetics.Clear();
	}

	public static void Xray()
	{
		if (Main.rightTrigger > 0.5f)
		{
			if (disabledRenderers.Count > 0)
			{
				return;
			}
			{
				foreach (Renderer item in from rend in Main.GetAllType<Renderer>(5f)
					where (Object)(object)rend != (Object)null && (Object)(object)((Component)rend).gameObject != (Object)null && !(rend is SkinnedMeshRenderer) && rend.enabled && ((Component)rend).gameObject.activeSelf
					select rend)
				{
					item.enabled = false;
					disabledRenderers.Add(item);
				}
				return;
			}
		}
		if (disabledRenderers.Count <= 0)
		{
			return;
		}
		foreach (Renderer item2 in disabledRenderers.Where((Renderer rend) => (Object)(object)rend != (Object)null && (Object)(object)((Component)rend).gameObject != (Object)null))
		{
			item2.enabled = true;
		}
		disabledRenderers.Clear();
	}

	public static void NoSmoothRigs()
	{
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal))
		{
			item.lerpValueBody = 2f;
			item.lerpValueFingers = 1f;
		}
	}

	public static void ReSmoothRigs()
	{
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal))
		{
			item.lerpValueBody = VRRig.LocalRig.lerpValueBody;
			item.lerpValueFingers = VRRig.LocalRig.lerpValueFingers;
		}
	}

	public static IEnumerator LerpRig(VRRig rig)
	{
		Quaternion headStartRot = rig.head.rigTarget.localRotation;
		Vector3 syncStartPos = ((Component)rig).transform.position;
		Quaternion syncStartRot = ((Component)rig).transform.rotation;
		Vector3 leftHandStartPos = rig.leftHand.rigTarget.localPosition;
		Quaternion leftHandStartRot = rig.leftHand.rigTarget.localRotation;
		Vector3 rightHandStartPos = rig.rightHand.rigTarget.localPosition;
		Quaternion rightHandStartRot = rig.rightHand.rigTarget.localRotation;
		float startTime = Time.time;
		float length = 1f / (float)PhotonNetwork.SerializationRate;
		while (Time.time < startTime + length)
		{
			float t = (Time.time - startTime) / length;
			rig.head.rigTarget.localRotation = Quaternion.Lerp(headStartRot, rig.head.syncRotation, t);
			((Component)rig).transform.position = Vector3.Lerp(syncStartPos, rig.syncPos, t);
			((Component)rig).transform.rotation = Quaternion.Lerp(syncStartRot, rig.syncRotation, t);
			rig.leftHand.rigTarget.localPosition = Vector3.Lerp(leftHandStartPos, rig.leftHand.syncPos, t);
			rig.leftHand.rigTarget.localRotation = Quaternion.Lerp(leftHandStartRot, rig.leftHand.syncRotation, t);
			rig.rightHand.rigTarget.localPosition = Vector3.Lerp(rightHandStartPos, rig.rightHand.syncPos, t);
			rig.rightHand.rigTarget.localRotation = Quaternion.Lerp(rightHandStartRot, rig.rightHand.syncRotation, t);
			yield return null;
		}
		rigLerpCoroutines.Remove(rig);
	}

	public static void BetterRigLerping(VRRig rig)
	{
		if (rigLerpCoroutines.TryGetValue(rig, out var value))
		{
			((MonoBehaviour)CoroutineManager.instance).StopCoroutine(value);
		}
		rigLerpCoroutines[rig] = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(LerpRig(rig));
	}

	public static void CosmeticESP()
	{
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Expected O, but got Unknown
		List<KeyValuePair<VRRig, GameObject>> source = cosmeticIndicators.ToList();
		foreach (KeyValuePair<VRRig, GameObject> item in source.Where((KeyValuePair<VRRig, GameObject> nametag) => !VRRigCache.ActiveRigs.Contains(nametag.Key)))
		{
			Object.Destroy((Object)(object)item.Value);
			cosmeticIndicators.Remove(item.Key);
		}
		List<(string, string)> list = new List<(string, string)>
		{
			("LBAAD.", "admin"),
			("LBAAK.", "stick"),
			("LMAPY.", "forestguide"),
			("LBADE.", "fingerpainter"),
			("LBAGS.", "illustrator"),
			("LBANI.", "aa")
		};
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			string text = null;
			foreach (var (value, text2) in list)
			{
				if (activeRig.Cosmetics().Contains(value))
				{
					text = text2;
					break;
				}
			}
			GameObject value4;
			if (!activeRig.isLocal && text != null)
			{
				if (!cosmeticIndicators.TryGetValue(activeRig, out var value2))
				{
					value2 = GameObject.CreatePrimitive((PrimitiveType)3);
					Object.Destroy((Object)(object)value2.GetComponent<Collider>());
					if ((Object)(object)cosmeticMat == (Object)null)
					{
						cosmeticMat = new Material(AssetUtilities.LoadAsset<Shader>("Chams"));
					}
					value2.GetComponent<Renderer>().material = cosmeticMat;
					cosmeticIndicators.Add(activeRig, value2);
				}
				if (!cosmeticTextures.TryGetValue(text, out var value3))
				{
					value3 = AssetUtilities.LoadTextureFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Images/Mods/Visuals/" + text + ".png", "Images/Mods/Visuals/" + text + ".png");
					cosmeticTextures.Add(text, value3);
				}
				value2.GetComponent<Renderer>().material.mainTexture = (Texture)(object)value3;
				value2.transform.localScale = new Vector3(0.5f, 0.5f, 0.01f) * activeRig.scaleFactor;
				value2.transform.position = activeRig.headMesh.transform.position + activeRig.headMesh.transform.up * (Seralyth.Classes.Menu.Console.GetIndicatorDistance(activeRig) * activeRig.scaleFactor);
				value2.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
			}
			else if (text == null && cosmeticIndicators.TryGetValue(activeRig, out value4))
			{
				Object.Destroy((Object)(object)value4);
				cosmeticIndicators.Remove(activeRig);
			}
		}
	}

	public static void DisableCosmeticESP()
	{
		foreach (KeyValuePair<VRRig, GameObject> cosmeticIndicator in cosmeticIndicators)
		{
			Object.Destroy((Object)(object)cosmeticIndicator.Value);
		}
		cosmeticIndicators.Clear();
	}

	private static Texture2D GetPlatformTexture(VRRig rig)
	{
		string platform = rig.GetPlatform();
		if (1 == 0)
		{
		}
		Texture2D result = (Texture2D)(platform switch
		{
			"Steam" => AssetUtilities.LoadTextureFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Images/Mods/Visuals/steam.png", "Images/Mods/Visuals/steam.png"), 
			"Standalone" => AssetUtilities.LoadTextureFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Images/Mods/Visuals/oculus.png", "Images/Mods/Visuals/oculus.png"), 
			"PC" => AssetUtilities.LoadTextureFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Images/Mods/Visuals/pc.png", "Images/Mods/Visuals/pc.png"), 
			_ => AssetUtilities.LoadTextureFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Images/Mods/Visuals/unknown.png", "Images/Mods/Visuals/unknown.png"), 
		});
		if (1 == 0)
		{
		}
		return result;
	}

	public static void PlatformIndicators()
	{
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		List<KeyValuePair<VRRig, GameObject>> source = platformIndicators.ToList();
		foreach (KeyValuePair<VRRig, GameObject> item in source.Where((KeyValuePair<VRRig, GameObject> nametag) => !VRRigCache.ActiveRigs.Contains(nametag.Key)))
		{
			Object.Destroy((Object)(object)item.Value);
			platformIndicators.Remove(item.Key);
		}
		foreach (VRRig item2 in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal || selfNameTag))
		{
			if (!platformIndicators.TryGetValue(item2, out var value))
			{
				value = GameObject.CreatePrimitive((PrimitiveType)3);
				Object.Destroy((Object)(object)value.GetComponent<Collider>());
				if ((Object)(object)platformMat == (Object)null)
				{
					platformMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
					platformMat.SetFloat("_Surface", 1f);
					platformMat.SetFloat("_Blend", 0f);
					platformMat.SetFloat("_SrcBlend", 5f);
					platformMat.SetFloat("_DstBlend", 10f);
					platformMat.SetFloat("_ZWrite", 0f);
					platformMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
					platformMat.renderQueue = 3000;
				}
				value.GetComponent<Renderer>().material = platformMat;
				platformIndicators.Add(item2, value);
			}
			value.GetComponent<Renderer>().material.mainTexture = (Texture)(object)GetPlatformTexture(item2);
			value.GetComponent<Renderer>().material.color = item2.GetColor();
			value.transform.localScale = new Vector3(0.5f, 0.5f, 0.01f) * item2.scaleFactor;
			value.transform.position = item2.headMesh.transform.position + item2.headMesh.transform.up * (Seralyth.Classes.Menu.Console.GetIndicatorDistance(item2) * item2.scaleFactor);
			value.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
		}
	}

	public static void PlatformESP()
	{
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Expected O, but got Unknown
		List<KeyValuePair<VRRig, GameObject>> source = platformIndicators.ToList();
		foreach (KeyValuePair<VRRig, GameObject> item in source.Where((KeyValuePair<VRRig, GameObject> nametag) => !VRRigCache.ActiveRigs.Contains(nametag.Key)))
		{
			Object.Destroy((Object)(object)item.Value);
			platformIndicators.Remove(item.Key);
		}
		foreach (VRRig item2 in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal || selfNameTag))
		{
			if (!platformIndicators.TryGetValue(item2, out var value))
			{
				value = GameObject.CreatePrimitive((PrimitiveType)5);
				Object.Destroy((Object)(object)value.GetComponent<Collider>());
				value.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
				if ((Object)(object)platformEspMat == (Object)null)
				{
					platformEspMat = new Material(Shader.Find("GUI/Text Shader"));
				}
				value.GetComponent<Renderer>().material = platformEspMat;
				platformIndicators.Add(item2, value);
			}
			value.GetComponent<Renderer>().material.mainTexture = (Texture)(object)GetPlatformTexture(item2);
			value.GetComponent<Renderer>().material.color = item2.GetColor();
			value.transform.localScale = new Vector3(0.5f, 0.5f, 0.01f) * item2.scaleFactor;
			value.transform.position = item2.headMesh.transform.position + item2.headMesh.transform.up * (Seralyth.Classes.Menu.Console.GetIndicatorDistance(item2) * item2.scaleFactor);
			value.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
		}
	}

	public static void DisablePlatformIndicators()
	{
		foreach (KeyValuePair<VRRig, GameObject> platformIndicator in platformIndicators)
		{
			Object.Destroy((Object)(object)platformIndicator.Value);
		}
		platformIndicators.Clear();
	}

	public static void VoiceIndicators()
	{
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Expected O, but got Unknown
		List<KeyValuePair<VRRig, GameObject>> source = voiceIndicators.ToList();
		foreach (KeyValuePair<VRRig, GameObject> item in source.Where((KeyValuePair<VRRig, GameObject> nametag) => !VRRigCache.ActiveRigs.Contains(nametag.Key)))
		{
			Object.Destroy((Object)(object)item.Value);
			voiceIndicators.Remove(item.Key);
		}
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (activeRig.isLocal && !selfNameTag)
			{
				continue;
			}
			float num = 0f;
			GorillaSpeakerLoudness component = ((Component)activeRig).GetComponent<GorillaSpeakerLoudness>();
			if ((Object)(object)component != (Object)null)
			{
				num = component.Loudness * 3f;
			}
			GameObject value2;
			if (num > 0f)
			{
				if (!voiceIndicators.TryGetValue(activeRig, out var value))
				{
					value = GameObject.CreatePrimitive((PrimitiveType)3);
					Object.Destroy((Object)(object)value.GetComponent<Collider>());
					if ((Object)(object)voiceMat == (Object)null)
					{
						voiceMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
						if ((Object)(object)voiceTxt == (Object)null)
						{
							voiceTxt = AssetUtilities.LoadTextureFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Images/Mods/Visuals/speak.png", "Images/Mods/Visuals/speak.png");
						}
						voiceMat.mainTexture = (Texture)(object)voiceTxt;
						voiceMat.SetFloat("_Surface", 1f);
						voiceMat.SetFloat("_Blend", 0f);
						voiceMat.SetFloat("_SrcBlend", 5f);
						voiceMat.SetFloat("_DstBlend", 10f);
						voiceMat.SetFloat("_ZWrite", 0f);
						voiceMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
						voiceMat.renderQueue = 3000;
					}
					value.GetComponent<Renderer>().material = voiceMat;
					voiceIndicators.Add(activeRig, value);
				}
				value.GetComponent<Renderer>().material.color = activeRig.GetColor();
				value.transform.localScale = new Vector3(num, num, 0.01f) * activeRig.scaleFactor;
				value.transform.position = activeRig.headMesh.transform.position + activeRig.headMesh.transform.up * (Seralyth.Classes.Menu.Console.GetIndicatorDistance(activeRig) * activeRig.scaleFactor);
				value.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
			}
			else if (voiceIndicators.TryGetValue(activeRig, out value2))
			{
				Object.Destroy((Object)(object)value2);
				voiceIndicators.Remove(activeRig);
			}
		}
	}

	public static void VoiceESP()
	{
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected O, but got Unknown
		List<KeyValuePair<VRRig, GameObject>> source = voiceIndicators.ToList();
		foreach (KeyValuePair<VRRig, GameObject> item in source.Where((KeyValuePair<VRRig, GameObject> nametag) => !VRRigCache.ActiveRigs.Contains(nametag.Key)))
		{
			Object.Destroy((Object)(object)item.Value);
			voiceIndicators.Remove(item.Key);
		}
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (activeRig.isLocal)
			{
				continue;
			}
			float num = 0f;
			GorillaSpeakerLoudness component = ((Component)activeRig).GetComponent<GorillaSpeakerLoudness>();
			if ((Object)(object)component != (Object)null)
			{
				num = component.Loudness * 3f;
			}
			GameObject value2;
			if (num > 0f)
			{
				if (!voiceIndicators.TryGetValue(activeRig, out var value))
				{
					value = GameObject.CreatePrimitive((PrimitiveType)5);
					Object.Destroy((Object)(object)value.GetComponent<Collider>());
					if ((Object)(object)voiceEspMat == (Object)null)
					{
						voiceEspMat = new Material(Shader.Find("GUI/Text Shader"));
					}
					if ((Object)(object)voiceTxt == (Object)null)
					{
						voiceTxt = AssetUtilities.LoadTextureFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Images/Mods/Visuals/speak.png", "Images/Mods/Visuals/speak.png");
					}
					voiceEspMat.mainTexture = (Texture)(object)voiceTxt;
					value.GetComponent<Renderer>().material = voiceEspMat;
					voiceIndicators.Add(activeRig, value);
				}
				value.GetComponent<Renderer>().material.color = activeRig.GetColor();
				value.transform.localScale = new Vector3(num, num, 0.01f) * activeRig.scaleFactor;
				value.transform.position = activeRig.headMesh.transform.position + activeRig.headMesh.transform.up * (Seralyth.Classes.Menu.Console.GetIndicatorDistance(activeRig) * activeRig.scaleFactor);
				value.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
			}
			else if (voiceIndicators.TryGetValue(activeRig, out value2))
			{
				Object.Destroy((Object)(object)value2);
				voiceIndicators.Remove(activeRig);
			}
		}
	}

	public static void DisableVoiceIndicators()
	{
		foreach (KeyValuePair<VRRig, GameObject> voiceIndicator in voiceIndicators)
		{
			Object.Destroy((Object)(object)voiceIndicator.Value);
		}
		voiceIndicators.Clear();
	}

	private static void UpdateLimbColor()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		Color color = VRRig.LocalRig.GetColor();
		l.GetComponent<Renderer>().material.color = color;
		r.GetComponent<Renderer>().material.color = color;
	}

	public static void StartNoLimb()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		l = GameObject.CreatePrimitive((PrimitiveType)0);
		Object.Destroy((Object)(object)l.GetComponent<SphereCollider>());
		l.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		r = GameObject.CreatePrimitive((PrimitiveType)0);
		Object.Destroy((Object)(object)r.GetComponent<SphereCollider>());
		r.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		UpdateLimbColor();
	}

	public static void NoLimbMode()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		l.transform.position = ControllerUtilities.GetTrueLeftHand().position;
		r.transform.position = ControllerUtilities.GetTrueRightHand().position;
		((Renderer)VRRig.LocalRig.mainSkin).material.shader = Shader.Find("GUI/Text Shader");
		((Renderer)VRRig.LocalRig.mainSkin).material.color = new Color(((Renderer)VRRig.LocalRig.mainSkin).material.color.r, ((Renderer)VRRig.LocalRig.mainSkin).material.color.g, ((Renderer)VRRig.LocalRig.mainSkin).material.color.b, 0f);
		UpdateLimbColor();
	}

	public static void EndNoLimb()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		Object.Destroy((Object)(object)l);
		Object.Destroy((Object)(object)r);
		((Renderer)VRRig.LocalRig.mainSkin).material.shader = Shader.Find("GorillaTag/UberShader");
		((Renderer)VRRig.LocalRig.mainSkin).material.color = new Color(((Renderer)VRRig.LocalRig.mainSkin).material.color.r, ((Renderer)VRRig.LocalRig.mainSkin).material.color.g, ((Renderer)VRRig.LocalRig.mainSkin).material.color.b, 1f);
	}

	public static void CasualBoneESP()
	{
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		bool enabled4 = Buttons.GetIndex("Thin Tracers").enabled;
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, List<LineRenderer>> item in boneESP.Where((KeyValuePair<VRRig, List<LineRenderer>> boness) => !VRRigCache.ActiveRigs.Contains(boness.Key)))
		{
			list.Add(item.Key);
			foreach (LineRenderer item2 in item.Value)
			{
				Object.Destroy((Object)(object)item2);
			}
		}
		foreach (VRRig item3 in list)
		{
			boneESP.Remove(item3);
		}
		foreach (VRRig item4 in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal))
		{
			if (!boneESP.TryGetValue(item4, out var value))
			{
				value = new List<LineRenderer>();
				LineRenderer orAddComponent = GTExt.GetOrAddComponent<LineRenderer>(((Component)item4.head.rigTarget).gameObject);
				if (Main.smoothLines)
				{
					orAddComponent.numCapVertices = 10;
					orAddComponent.numCornerVertices = 5;
				}
				((Renderer)orAddComponent).material.shader = Shader.Find("GUI/Text Shader");
				value.Add(orAddComponent);
				for (int num = 0; num < 19; num++)
				{
					LineRenderer orAddComponent2 = GTExt.GetOrAddComponent<LineRenderer>(((Component)item4.mainSkin.bones[bones[num * 2]]).gameObject);
					if (Main.smoothLines)
					{
						orAddComponent2.numCapVertices = 10;
						orAddComponent2.numCornerVertices = 5;
					}
					((Renderer)orAddComponent2).material.shader = Shader.Find("GUI/Text Shader");
					value.Add(orAddComponent2);
				}
				boneESP.Add(item4, value);
			}
			LineRenderer val = value[0];
			Color val2 = item4.playerColor;
			if (enabled)
			{
				val2 = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				val2.a = 0.5f;
			}
			if (enabled2)
			{
				((Component)val).gameObject.layer = 19;
			}
			val.startWidth = (enabled4 ? 0.0075f : 0.025f);
			val.endWidth = (enabled4 ? 0.0075f : 0.025f);
			val.startColor = val2;
			val.endColor = val2;
			val.SetPosition(0, ((Component)item4.head.rigTarget).transform.position + new Vector3(0f, 0.16f, 0f));
			val.SetPosition(1, ((Component)item4.head.rigTarget).transform.position - new Vector3(0f, 0.4f, 0f));
			for (int num2 = 0; num2 < 19; num2++)
			{
				val = value[num2 + 1];
				if (enabled2)
				{
					((Component)val).gameObject.layer = 19;
				}
				val.startWidth = (enabled4 ? 0.0075f : 0.025f);
				val.endWidth = (enabled4 ? 0.0075f : 0.025f);
				val.startColor = val2;
				val.endColor = val2;
				((Renderer)val).material.shader = Shader.Find("GUI/Text Shader");
				val.SetPosition(0, item4.mainSkin.bones[bones[num2 * 2]].position);
				val.SetPosition(1, item4.mainSkin.bones[bones[num2 * 2 + 1]].position);
			}
		}
	}

	public static void InfectionBoneESP()
	{
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		bool enabled4 = Buttons.GetIndex("Thin Tracers").enabled;
		bool flag = VRRig.LocalRig.IsTagged();
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, List<LineRenderer>> item in boneESP.Where((KeyValuePair<VRRig, List<LineRenderer>> boness) => !VRRigCache.ActiveRigs.Contains(boness.Key)))
		{
			list.Add(item.Key);
			foreach (LineRenderer item2 in item.Value)
			{
				Object.Destroy((Object)(object)item2);
			}
		}
		foreach (VRRig item3 in list)
		{
			boneESP.Remove(item3);
		}
		foreach (VRRig item4 in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal))
		{
			if (!boneESP.TryGetValue(item4, out var value))
			{
				value = new List<LineRenderer>();
				LineRenderer orAddComponent = GTExt.GetOrAddComponent<LineRenderer>(((Component)item4.head.rigTarget).gameObject);
				if (Main.smoothLines)
				{
					orAddComponent.numCapVertices = 10;
					orAddComponent.numCornerVertices = 5;
				}
				((Renderer)orAddComponent).material.shader = Shader.Find("GUI/Text Shader");
				value.Add(orAddComponent);
				for (int num = 0; num < 19; num++)
				{
					LineRenderer orAddComponent2 = GTExt.GetOrAddComponent<LineRenderer>(((Component)item4.mainSkin.bones[bones[num * 2]]).gameObject);
					if (Main.smoothLines)
					{
						orAddComponent2.numCapVertices = 10;
						orAddComponent2.numCornerVertices = 5;
					}
					((Renderer)orAddComponent2).material.shader = Shader.Find("GUI/Text Shader");
					value.Add(orAddComponent2);
				}
				boneESP.Add(item4, value);
			}
			LineRenderer val = value[0];
			bool flag2 = item4.IsTagged();
			Color val2 = (flag ? item4.playerColor : item4.GetColor());
			if (enabled)
			{
				val2 = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				val2.a = 0.5f;
			}
			if (enabled2)
			{
				((Component)val).gameObject.layer = 19;
			}
			val.startWidth = (enabled4 ? 0.0075f : 0.025f);
			val.endWidth = (enabled4 ? 0.0075f : 0.025f);
			val.startColor = val2;
			val.endColor = val2;
			((Renderer)val).enabled = (flag ? (!flag2) : flag2) || GameModeUtilities.InfectedList().Count <= 0;
			val.SetPosition(0, ((Component)item4.head.rigTarget).transform.position + new Vector3(0f, 0.16f, 0f));
			val.SetPosition(1, ((Component)item4.head.rigTarget).transform.position - new Vector3(0f, 0.4f, 0f));
			for (int num2 = 0; num2 < 19; num2++)
			{
				val = value[num2 + 1];
				if (enabled2)
				{
					((Component)val).gameObject.layer = 19;
				}
				val.startWidth = (enabled4 ? 0.0075f : 0.025f);
				val.endWidth = (enabled4 ? 0.0075f : 0.025f);
				val.startColor = val2;
				val.endColor = val2;
				((Renderer)val).material.shader = Shader.Find("GUI/Text Shader");
				((Renderer)val).enabled = (flag ? (!flag2) : flag2) || GameModeUtilities.InfectedList().Count <= 0;
				val.SetPosition(0, item4.mainSkin.bones[bones[num2 * 2]].position);
				val.SetPosition(1, item4.mainSkin.bones[bones[num2 * 2 + 1]].position);
			}
		}
	}

	public static void HuntBoneESP()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom || (int)GorillaGameManager.instance.GameType() != 2)
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		bool enabled4 = Buttons.GetIndex("Thin Tracers").enabled;
		List<VRRig> list = new List<VRRig>();
		GorillaHuntManager val = (GorillaHuntManager)GorillaGameManager.instance;
		NetPlayer targetOf = val.GetTargetOf(NetworkSystem.Instance.LocalPlayer);
		foreach (KeyValuePair<VRRig, List<LineRenderer>> item in boneESP.Where((KeyValuePair<VRRig, List<LineRenderer>> boness) => !VRRigCache.ActiveRigs.Contains(boness.Key)))
		{
			list.Add(item.Key);
			foreach (LineRenderer item2 in item.Value)
			{
				Object.Destroy((Object)(object)item2);
			}
		}
		foreach (VRRig item3 in list)
		{
			boneESP.Remove(item3);
		}
		foreach (VRRig item4 in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal))
		{
			if (!boneESP.TryGetValue(item4, out var value))
			{
				value = new List<LineRenderer>();
				LineRenderer orAddComponent = GTExt.GetOrAddComponent<LineRenderer>(((Component)item4.head.rigTarget).gameObject);
				if (Main.smoothLines)
				{
					orAddComponent.numCapVertices = 10;
					orAddComponent.numCornerVertices = 5;
				}
				((Renderer)orAddComponent).material.shader = Shader.Find("GUI/Text Shader");
				value.Add(orAddComponent);
				for (int num = 0; num < 19; num++)
				{
					LineRenderer orAddComponent2 = GTExt.GetOrAddComponent<LineRenderer>(((Component)item4.mainSkin.bones[bones[num * 2]]).gameObject);
					if (Main.smoothLines)
					{
						orAddComponent2.numCapVertices = 10;
						orAddComponent2.numCornerVertices = 5;
					}
					((Renderer)orAddComponent2).material.shader = Shader.Find("GUI/Text Shader");
					value.Add(orAddComponent2);
				}
				boneESP.Add(item4, value);
			}
			LineRenderer val2 = value[0];
			NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(item4);
			NetPlayer targetOf2 = val.GetTargetOf(playerFromVRRig);
			Color val3 = ((playerFromVRRig == targetOf) ? item4.GetColor() : ((targetOf2 == NetworkSystem.Instance.LocalPlayer) ? Color.red : Color.clear));
			if (enabled)
			{
				val3 = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				val3.a = 0.5f;
			}
			if (enabled2)
			{
				((Component)val2).gameObject.layer = 19;
			}
			val2.startWidth = (enabled4 ? 0.0075f : 0.025f);
			val2.endWidth = (enabled4 ? 0.0075f : 0.025f);
			val2.startColor = val3;
			val2.endColor = val3;
			((Renderer)val2).enabled = playerFromVRRig == targetOf || targetOf2 == NetworkSystem.Instance.LocalPlayer;
			val2.SetPosition(0, ((Component)item4.head.rigTarget).transform.position + new Vector3(0f, 0.16f, 0f));
			val2.SetPosition(1, ((Component)item4.head.rigTarget).transform.position - new Vector3(0f, 0.4f, 0f));
			for (int num2 = 0; num2 < 19; num2++)
			{
				val2 = value[num2 + 1];
				if (enabled2)
				{
					((Component)val2).gameObject.layer = 19;
				}
				val2.startWidth = (enabled4 ? 0.0075f : 0.025f);
				val2.endWidth = (enabled4 ? 0.0075f : 0.025f);
				val2.startColor = val3;
				val2.endColor = val3;
				((Renderer)val2).material.shader = Shader.Find("GUI/Text Shader");
				((Renderer)val2).enabled = playerFromVRRig == targetOf || targetOf2 == NetworkSystem.Instance.LocalPlayer;
				val2.SetPosition(0, item4.mainSkin.bones[bones[num2 * 2]].position);
				val2.SetPosition(1, item4.mainSkin.bones[bones[num2 * 2 + 1]].position);
			}
		}
	}

	public static void DisableBoneESP()
	{
		foreach (LineRenderer item in boneESP.SelectMany((KeyValuePair<VRRig, List<LineRenderer>> bones) => bones.Value))
		{
			Object.Destroy((Object)(object)item);
		}
		boneESP.Clear();
	}

	public static void CasualSkeletonESP()
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal))
		{
			((Renderer)item.skeleton.renderer).enabled = true;
			((Renderer)item.skeleton.renderer).material.shader = Shader.Find("GUI/Text Shader");
			((Renderer)item.skeleton.renderer).material.color = item.playerColor;
			if (Buttons.GetIndex("Follow Menu Theme").enabled)
			{
				((Renderer)item.skeleton.renderer).material.color = Main.backgroundColor.GetCurrentColor();
			}
			if (Buttons.GetIndex("Transparent Theme").enabled)
			{
				((Renderer)item.skeleton.renderer).material.color = new Color(((Renderer)item.skeleton.renderer).material.color.r, ((Renderer)item.skeleton.renderer).material.color.g, ((Renderer)item.skeleton.renderer).material.color.b, 0.5f);
			}
		}
	}

	public static void InfectionSkeletonESP()
	{
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (activeRig.IsTagged())
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			if (!VRRig.LocalRig.IsTagged())
			{
				foreach (VRRig activeRig2 in VRRigCache.ActiveRigs)
				{
					if (activeRig2.IsTagged() && !activeRig2.isLocal)
					{
						((Renderer)activeRig2.skeleton.renderer).enabled = true;
						((Renderer)activeRig2.mainSkin).material.shader = Shader.Find("GUI/Text Shader");
						((Renderer)activeRig2.skeleton.renderer).material.color = activeRig2.GetColor();
						if (Buttons.GetIndex("Follow Menu Theme").enabled)
						{
							((Renderer)activeRig2.skeleton.renderer).material.color = Main.backgroundColor.GetCurrentColor();
						}
						if (Buttons.GetIndex("Transparent Theme").enabled)
						{
							((Renderer)activeRig2.skeleton.renderer).material.color = new Color(((Renderer)activeRig2.skeleton.renderer).material.color.r, ((Renderer)activeRig2.skeleton.renderer).material.color.g, ((Renderer)activeRig2.skeleton.renderer).material.color.b, 0.5f);
						}
					}
					else
					{
						((Renderer)activeRig2.skeleton.renderer).enabled = false;
						((Renderer)activeRig2.skeleton.renderer).material.shader = Shader.Find("GorillaTag/UberShader");
						if (((Object)((Renderer)activeRig2.skeleton.renderer).material).name.Contains("gorilla_body"))
						{
							((Renderer)activeRig2.skeleton.renderer).material.color = activeRig2.playerColor;
						}
					}
				}
				return;
			}
			{
				foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.IsTagged() && !vrrig.isLocal))
				{
					((Renderer)item.skeleton.renderer).enabled = true;
					((Renderer)item.skeleton.renderer).material.shader = Shader.Find("GUI/Text Shader");
					((Renderer)item.skeleton.renderer).material.color = item.playerColor;
					if (Buttons.GetIndex("Follow Menu Theme").enabled)
					{
						((Renderer)item.skeleton.renderer).material.color = Main.backgroundColor.GetCurrentColor();
					}
					if (Buttons.GetIndex("Transparent Theme").enabled)
					{
						((Renderer)item.skeleton.renderer).material.color = new Color(((Renderer)item.skeleton.renderer).material.color.r, ((Renderer)item.skeleton.renderer).material.color.g, ((Renderer)item.skeleton.renderer).material.color.b, 0.5f);
					}
				}
				return;
			}
		}
		foreach (VRRig item2 in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal))
		{
			((Renderer)item2.skeleton.renderer).enabled = true;
			((Renderer)item2.skeleton.renderer).material.shader = Shader.Find("GUI/Text Shader");
			if (((Object)((Renderer)item2.skeleton.renderer).material).name.Contains("gorilla_body"))
			{
				((Renderer)item2.skeleton.renderer).material.color = item2.playerColor;
			}
		}
	}

	public static void HuntSkeletonESP()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom || (int)GorillaGameManager.instance.GameType() != 2)
		{
			return;
		}
		GorillaHuntManager val = (GorillaHuntManager)GorillaGameManager.instance;
		NetPlayer targetOf = val.GetTargetOf(NetPlayer.op_Implicit(PhotonNetwork.LocalPlayer));
		Player[] playerList = PhotonNetwork.PlayerList;
		for (int i = 0; i < playerList.Length; i++)
		{
			NetPlayer val2 = NetPlayer.op_Implicit(playerList[i]);
			VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(val2);
			if (val2 == targetOf)
			{
				((Renderer)vRRigFromPlayer.skeleton.renderer).enabled = true;
				((Renderer)vRRigFromPlayer.skeleton.renderer).material.shader = Shader.Find("GUI/Text Shader");
				((Renderer)vRRigFromPlayer.skeleton.renderer).material.color = vRRigFromPlayer.playerColor;
				if (Buttons.GetIndex("Follow Menu Theme").enabled)
				{
					((Renderer)vRRigFromPlayer.skeleton.renderer).material.color = Main.backgroundColor.GetCurrentColor();
				}
				if (Buttons.GetIndex("Transparent Theme").enabled)
				{
					((Renderer)vRRigFromPlayer.skeleton.renderer).material.color = new Color(((Renderer)vRRigFromPlayer.skeleton.renderer).material.color.r, ((Renderer)vRRigFromPlayer.skeleton.renderer).material.color.g, ((Renderer)vRRigFromPlayer.skeleton.renderer).material.color.b, 0.5f);
				}
			}
			else if (val.GetTargetOf(val2) == NetPlayer.op_Implicit(PhotonNetwork.LocalPlayer))
			{
				((Renderer)vRRigFromPlayer.skeleton.renderer).enabled = true;
				((Renderer)vRRigFromPlayer.skeleton.renderer).material.shader = Shader.Find("GUI/Text Shader");
				((Renderer)vRRigFromPlayer.skeleton.renderer).material.color = Color.red;
				if (Buttons.GetIndex("Transparent Theme").enabled)
				{
					((Renderer)vRRigFromPlayer.skeleton.renderer).material.color = new Color(((Renderer)vRRigFromPlayer.skeleton.renderer).material.color.r, ((Renderer)vRRigFromPlayer.skeleton.renderer).material.color.g, ((Renderer)vRRigFromPlayer.skeleton.renderer).material.color.b, 0.5f);
				}
			}
			else
			{
				((Renderer)vRRigFromPlayer.skeleton.renderer).enabled = false;
				((Renderer)vRRigFromPlayer.skeleton.renderer).material.shader = Shader.Find("GorillaTag/UberShader");
				if (((Object)((Renderer)vRRigFromPlayer.skeleton.renderer).material).name.Contains("gorilla_body"))
				{
					((Renderer)vRRigFromPlayer.skeleton.renderer).material.color = vRRigFromPlayer.playerColor;
				}
			}
		}
	}

	public static void DisableSkeletonESP()
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal))
		{
			((Renderer)item.skeleton.renderer).enabled = false;
			((Renderer)item.skeleton.renderer).material.shader = Shader.Find("GorillaTag/UberShader");
			if (((Object)((Renderer)item.skeleton.renderer).material).name.Contains("gorilla_body"))
			{
				((Renderer)item.skeleton.renderer).material.color = item.playerColor;
			}
		}
	}

	public static void CasualWireframeESP()
	{
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, SkinnedWireframeRenderer> item in wireframes.Where((KeyValuePair<VRRig, SkinnedWireframeRenderer> lines) => !VRRigCache.ActiveRigs.Contains(lines.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			wireframes.Remove(item2);
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		foreach (VRRig item3 in VRRigCache.ActiveRigs.Where((VRRig rig) => !rig.isLocal))
		{
			if (!wireframes.TryGetValue(item3, out var value))
			{
				value = ComponentUtils.AddComponent<SkinnedWireframeRenderer>((Component)(object)item3.mainSkin);
				wireframes.Add(item3, value);
			}
			if (enabled2)
			{
				value.wireframeObj.layer = 19;
			}
			Color val = item3.GetColor();
			if (enabled)
			{
				val = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				((Color)(ref val))._002Ector(val.r, val.g, val.b, 0.5f);
			}
			((Renderer)value.meshRenderer).material.color = val;
			Vector3 val2 = ((Component)item3).transform.position - ((Component)GorillaTagger.Instance.headCollider).transform.position;
			float num = Vector3.Angle(((Component)GorillaTagger.Instance.headCollider).transform.forward, val2);
			bool flag = num <= Camera.main.fieldOfView / 1.75f;
			flag = (((Behaviour)value).enabled = flag & (Vector3.Distance(((Component)item3).transform.position, ((Component)GorillaTagger.Instance.headCollider).transform.position) < 35f && Vector3.Distance(item3.headMesh.transform.position, ((Component)GorillaTagger.Instance.headCollider).transform.position) < 35f && Vector3.Distance(item3.leftHandTransform.position, ((Component)GorillaTagger.Instance.headCollider).transform.position) < 35f && Vector3.Distance(item3.rightHandTransform.position, ((Component)GorillaTagger.Instance.headCollider).transform.position) < 35f));
			((Renderer)value.meshRenderer).enabled = flag;
			if (!flag)
			{
				FixRigMaterialESPColors(item3);
				((Renderer)item3.mainSkin).material.shader = Shader.Find("GUI/Text Shader");
				((Renderer)item3.mainSkin).material.color = val;
				continue;
			}
			((Renderer)item3.mainSkin).material.shader = Shader.Find("GorillaTag/UberShader");
			if (((Object)((Renderer)item3.mainSkin).material).name.Contains("gorilla_body"))
			{
				((Renderer)item3.mainSkin).material.color = item3.playerColor;
			}
		}
	}

	public static void InfectionWireframeESP()
	{
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, SkinnedWireframeRenderer> item in wireframes.Where((KeyValuePair<VRRig, SkinnedWireframeRenderer> lines) => !VRRigCache.ActiveRigs.Contains(lines.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			wireframes.Remove(item2);
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		bool flag = VRRig.LocalRig.IsTagged();
		foreach (VRRig item3 in VRRigCache.ActiveRigs.Where((VRRig rig) => !rig.isLocal))
		{
			if (!wireframes.TryGetValue(item3, out var value))
			{
				value = ComponentUtils.AddComponent<SkinnedWireframeRenderer>((Component)(object)item3.mainSkin);
				wireframes.Add(item3, value);
			}
			bool flag2 = item3.IsTagged();
			Color color = (flag ? item3.playerColor : item3.GetColor());
			if (enabled)
			{
				color = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				color.a = 0.5f;
			}
			if (enabled2)
			{
				value.wireframeObj.layer = 19;
			}
			((Renderer)value.meshRenderer).material.color = color;
			bool flag3 = (flag ? (!flag2) : flag2) || GameModeUtilities.InfectedList().Count <= 0;
			Vector3 val = ((Component)item3).transform.position - ((Component)GorillaTagger.Instance.headCollider).transform.position;
			float num = Vector3.Angle(((Component)GorillaTagger.Instance.headCollider).transform.forward, val);
			flag3 &= num <= Camera.main.fieldOfView / 1.75f;
			flag3 = (((Behaviour)value).enabled = flag3 & (Vector3.Distance(((Component)item3).transform.position, ((Component)GorillaTagger.Instance.headCollider).transform.position) < 35f && Vector3.Distance(item3.headMesh.transform.position, ((Component)GorillaTagger.Instance.headCollider).transform.position) < 35f && Vector3.Distance(item3.leftHandTransform.position, ((Component)GorillaTagger.Instance.headCollider).transform.position) < 35f && Vector3.Distance(item3.rightHandTransform.position, ((Component)GorillaTagger.Instance.headCollider).transform.position) < 35f));
			((Renderer)value.meshRenderer).enabled = flag3;
			if (!flag3)
			{
				FixRigMaterialESPColors(item3);
				((Renderer)item3.mainSkin).material.shader = Shader.Find("GUI/Text Shader");
				((Renderer)item3.mainSkin).material.color = color;
				continue;
			}
			((Renderer)item3.mainSkin).material.shader = Shader.Find("GorillaTag/UberShader");
			if (((Object)((Renderer)item3.mainSkin).material).name.Contains("gorilla_body"))
			{
				((Renderer)item3.mainSkin).material.color = item3.playerColor;
			}
		}
	}

	public static void HuntWireframeESP()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom || (int)GorillaGameManager.instance.GameType() != 2)
		{
			return;
		}
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, SkinnedWireframeRenderer> item in wireframes.Where((KeyValuePair<VRRig, SkinnedWireframeRenderer> lines) => !VRRigCache.ActiveRigs.Contains(lines.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			wireframes.Remove(item2);
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		GorillaHuntManager val = (GorillaHuntManager)GorillaGameManager.instance;
		NetPlayer targetOf = val.GetTargetOf(NetworkSystem.Instance.LocalPlayer);
		foreach (VRRig item3 in VRRigCache.ActiveRigs.Where((VRRig rig) => !rig.isLocal))
		{
			if (!wireframes.TryGetValue(item3, out var value))
			{
				value = ComponentUtils.AddComponent<SkinnedWireframeRenderer>((Component)(object)item3.mainSkin);
				wireframes.Add(item3, value);
			}
			NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(item3);
			NetPlayer targetOf2 = val.GetTargetOf(playerFromVRRig);
			Color color = ((playerFromVRRig == targetOf) ? item3.GetColor() : ((targetOf2 == NetworkSystem.Instance.LocalPlayer) ? Color.red : Color.clear));
			if (enabled)
			{
				color = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				color.a = 0.5f;
			}
			if (enabled2)
			{
				value.wireframeObj.layer = 19;
			}
			((Renderer)value.meshRenderer).material.color = color;
			bool flag = playerFromVRRig == targetOf || targetOf2 == NetworkSystem.Instance.LocalPlayer;
			Vector3 val2 = ((Component)item3).transform.position - ((Component)GorillaTagger.Instance.headCollider).transform.position;
			float num = Vector3.Angle(((Component)GorillaTagger.Instance.headCollider).transform.forward, val2);
			flag &= num <= Camera.main.fieldOfView / 1.75f;
			flag = (((Behaviour)value).enabled = flag & (Vector3.Distance(((Component)item3).transform.position, ((Component)GorillaTagger.Instance.headCollider).transform.position) < 35f && Vector3.Distance(item3.headMesh.transform.position, ((Component)GorillaTagger.Instance.headCollider).transform.position) < 35f && Vector3.Distance(item3.leftHandTransform.position, ((Component)GorillaTagger.Instance.headCollider).transform.position) < 35f && Vector3.Distance(item3.rightHandTransform.position, ((Component)GorillaTagger.Instance.headCollider).transform.position) < 35f));
			((Renderer)value.meshRenderer).enabled = flag;
			if (!flag)
			{
				FixRigMaterialESPColors(item3);
				((Renderer)item3.mainSkin).material.shader = Shader.Find("GUI/Text Shader");
				((Renderer)item3.mainSkin).material.color = color;
				continue;
			}
			((Renderer)item3.mainSkin).material.shader = Shader.Find("GorillaTag/UberShader");
			if (((Object)((Renderer)item3.mainSkin).material).name.Contains("gorilla_body"))
			{
				((Renderer)item3.mainSkin).material.color = item3.playerColor;
			}
		}
	}

	public static void DisableWireframeESP()
	{
		foreach (KeyValuePair<VRRig, SkinnedWireframeRenderer> wireframe in wireframes)
		{
			((Renderer)wireframe.Key.mainSkin).material.shader = Shader.Find("GorillaTag/UberShader");
			Object.Destroy((Object)(object)wireframe.Value);
		}
		wireframes.Clear();
	}

	public static void FixRigMaterialESPColors(VRRig rig)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (!convertedRigs.Contains(rig))
		{
			convertedRigs.Add(rig);
			rig.mainSkin.sharedMesh.colors32 = Enumerable.Repeat<Color32>(Color32.op_Implicit(Color.white), rig.mainSkin.sharedMesh.colors32.Length).ToArray();
			rig.mainSkin.sharedMesh.colors = Enumerable.Repeat<Color>(Color.white, rig.mainSkin.sharedMesh.colors.Length).ToArray();
		}
	}

	public static void Chams()
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected O, but got Unknown
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Expected O, but got Unknown
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Expected O, but got Unknown
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal && vrrig.colorInitialized && vrrig.initializedCosmetics && ((Object)((Renderer)vrrig.mainSkin).material.shader).name != "Custom/UberChams"))
		{
			if (!Object.op_Implicit((Object)(object)uberChams))
			{
				uberChams = AssetUtilities.LoadAsset<Shader>("UberChams");
			}
			SkinnedMeshRenderer mainSkin = item.mainSkin;
			if (!originalMaterials.ContainsKey((Renderer)(object)mainSkin))
			{
				Material[] array = (Material[])(object)new Material[((Renderer)mainSkin).materials.Length];
				for (int num = 0; num < ((Renderer)mainSkin).materials.Length; num++)
				{
					array[num] = new Material(((Renderer)mainSkin).materials[num]);
				}
				originalMaterials[(Renderer)(object)mainSkin] = array;
			}
			Material[] materials = ((Renderer)mainSkin).materials;
			for (int num2 = 0; num2 < materials.Length; num2++)
			{
				updateShader(materials[num2], (num2 == 0) ? item.materialsToChangeTo[item.setMatIndex] : originalMaterials[(Renderer)(object)mainSkin][num2], materials[num2].color);
			}
			Renderer targetFaceRenderer = item.myMouthFlap.targetFaceRenderer;
			if (!originalMaterials.ContainsKey(targetFaceRenderer))
			{
				originalMaterials[targetFaceRenderer] = (Material[])(object)new Material[1]
				{
					new Material(targetFaceRenderer.material)
				};
			}
			updateShader(targetFaceRenderer.material, originalMaterials[targetFaceRenderer][0], Color.white, isFace: true);
			if (!Buttons.GetIndex("Show Cosmetics").enabled)
			{
				continue;
			}
			foreach (GameObject cosmetic in item.cosmetics)
			{
				Renderer[] componentsInChildren = cosmetic.GetComponentsInChildren<Renderer>();
				Renderer[] array2 = componentsInChildren;
				foreach (Renderer val in array2)
				{
					if (!originalMaterials.ContainsKey(val))
					{
						Material[] array3 = (Material[])(object)new Material[val.materials.Length];
						for (int num4 = 0; num4 < val.materials.Length; num4++)
						{
							array3[num4] = new Material(val.materials[num4]);
						}
						originalMaterials[val] = array3;
					}
					updateShader(val.material, originalMaterials[val][0], val.material.color);
				}
			}
		}
		static void updateShader(Material target, Material src, Color color, bool isFace = false)
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			target.shader = uberChams;
			Texture texture = src.GetTexture("_BaseMap_Atlas");
			Texture2DArray val2 = (Texture2DArray)(object)((texture is Texture2DArray) ? texture : null);
			float num5 = src.GetFloat("_BaseMap_AtlasSlice");
			target.SetTexture("_BaseMap_Atlas", (Texture)(object)val2);
			target.SetFloat("_BaseMap_AtlasSlice", num5);
			if (src.HasProperty("_BaseMap_ST"))
			{
				Vector4 vector = src.GetVector("_BaseMap_ST");
				target.SetVector("_BaseMap_ST", vector);
			}
			if (isFace)
			{
				target.SetFloat("_UseMouthMap", 1f);
				if (src.HasProperty("_MouthMap"))
				{
					Texture texture2 = src.GetTexture("_MouthMap");
					if ((Object)(object)texture2 != (Object)null)
					{
						target.SetTexture("_MouthMap", texture2);
					}
				}
				if (src.HasProperty("_MouthMap_ST"))
				{
					Vector4 vector2 = src.GetVector("_MouthMap_ST");
					target.SetVector("_MouthMap_ST", vector2);
				}
			}
			else
			{
				target.SetFloat("_UseMouthMap", 0f);
			}
			target.SetColor("_Color", color);
		}
	}

	public static void DisableShaderChams()
	{
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Expected O, but got Unknown
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (activeRig.isLocal)
			{
				continue;
			}
			SkinnedMeshRenderer mainSkin = activeRig.mainSkin;
			if (originalMaterials.ContainsKey((Renderer)(object)mainSkin))
			{
				Material[] array = (Material[])(object)new Material[originalMaterials[(Renderer)(object)mainSkin].Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new Material(originalMaterials[(Renderer)(object)mainSkin][i]);
				}
				((Renderer)mainSkin).materials = array;
				originalMaterials.Remove((Renderer)(object)mainSkin);
			}
			Renderer targetFaceRenderer = activeRig.myMouthFlap.targetFaceRenderer;
			if (originalMaterials.ContainsKey(targetFaceRenderer))
			{
				targetFaceRenderer.material = new Material(originalMaterials[targetFaceRenderer][0]);
				originalMaterials.Remove(targetFaceRenderer);
			}
			foreach (GameObject cosmetic in activeRig.cosmetics)
			{
				Renderer[] componentsInChildren = cosmetic.GetComponentsInChildren<Renderer>();
				Renderer[] array2 = componentsInChildren;
				foreach (Renderer val in array2)
				{
					if (originalMaterials.ContainsKey(val))
					{
						Material[] array3 = (Material[])(object)new Material[originalMaterials[val].Length];
						for (int k = 0; k < array3.Length; k++)
						{
							array3[k] = new Material(originalMaterials[val][k]);
						}
						val.materials = array3;
						originalMaterials.Remove(val);
					}
				}
			}
		}
	}

	public static void CasualChams()
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal))
		{
			FixRigMaterialESPColors(item);
			((Renderer)item.mainSkin).material.shader = Shader.Find("GUI/Text Shader");
			((Renderer)item.mainSkin).material.color = item.playerColor;
			if (Buttons.GetIndex("Follow Menu Theme").enabled)
			{
				((Renderer)item.mainSkin).material.color = Main.backgroundColor.GetCurrentColor();
			}
			if (Buttons.GetIndex("Transparent Theme").enabled)
			{
				((Renderer)item.mainSkin).material.color = new Color(((Renderer)item.mainSkin).material.color.r, ((Renderer)item.mainSkin).material.color.g, ((Renderer)item.mainSkin).material.color.b, 0.5f);
			}
		}
	}

	public static void InfectionChams()
	{
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (activeRig.IsTagged())
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			if (!VRRig.LocalRig.IsTagged())
			{
				foreach (VRRig activeRig2 in VRRigCache.ActiveRigs)
				{
					if (activeRig2.IsTagged() && !activeRig2.isLocal)
					{
						FixRigMaterialESPColors(activeRig2);
						((Renderer)activeRig2.mainSkin).material.shader = Shader.Find("GUI/Text Shader");
						((Renderer)activeRig2.mainSkin).material.color = activeRig2.GetColor();
						if (Buttons.GetIndex("Follow Menu Theme").enabled)
						{
							((Renderer)activeRig2.mainSkin).material.color = Main.backgroundColor.GetCurrentColor();
						}
						if (Buttons.GetIndex("Transparent Theme").enabled)
						{
							((Renderer)activeRig2.mainSkin).material.color = new Color(((Renderer)activeRig2.mainSkin).material.color.r, ((Renderer)activeRig2.mainSkin).material.color.g, ((Renderer)activeRig2.mainSkin).material.color.b, 0.5f);
						}
					}
					else
					{
						((Renderer)activeRig2.mainSkin).material.shader = Shader.Find("GorillaTag/UberShader");
						if (((Object)((Renderer)activeRig2.mainSkin).material).name.Contains("gorilla_body"))
						{
							((Renderer)activeRig2.mainSkin).material.color = activeRig2.playerColor;
						}
					}
				}
				return;
			}
			{
				foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.IsTagged() && !vrrig.isLocal))
				{
					FixRigMaterialESPColors(item);
					((Renderer)item.mainSkin).material.shader = Shader.Find("GUI/Text Shader");
					((Renderer)item.mainSkin).material.color = item.playerColor;
					if (Buttons.GetIndex("Follow Menu Theme").enabled)
					{
						((Renderer)item.mainSkin).material.color = Main.backgroundColor.GetCurrentColor();
					}
					if (Buttons.GetIndex("Transparent Theme").enabled)
					{
						((Renderer)item.mainSkin).material.color = new Color(((Renderer)item.mainSkin).material.color.r, ((Renderer)item.mainSkin).material.color.g, ((Renderer)item.mainSkin).material.color.b, 0.5f);
					}
				}
				return;
			}
		}
		foreach (VRRig item2 in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal))
		{
			FixRigMaterialESPColors(item2);
			((Renderer)item2.mainSkin).material.shader = Shader.Find("GUI/Text Shader");
			if (((Object)((Renderer)item2.mainSkin).material).name.Contains("gorilla_body"))
			{
				((Renderer)item2.mainSkin).material.color = item2.playerColor;
			}
		}
	}

	public static void HuntChams()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom || (int)GorillaGameManager.instance.GameType() != 2)
		{
			return;
		}
		GorillaHuntManager val = (GorillaHuntManager)GorillaGameManager.instance;
		NetPlayer targetOf = val.GetTargetOf(NetPlayer.op_Implicit(PhotonNetwork.LocalPlayer));
		Player[] playerList = PhotonNetwork.PlayerList;
		for (int i = 0; i < playerList.Length; i++)
		{
			NetPlayer val2 = NetPlayer.op_Implicit(playerList[i]);
			VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(val2);
			if (val2 == targetOf)
			{
				FixRigMaterialESPColors(vRRigFromPlayer);
				((Renderer)vRRigFromPlayer.mainSkin).material.shader = Shader.Find("GUI/Text Shader");
				((Renderer)vRRigFromPlayer.mainSkin).material.color = vRRigFromPlayer.playerColor;
				if (Buttons.GetIndex("Follow Menu Theme").enabled)
				{
					((Renderer)vRRigFromPlayer.mainSkin).material.color = Main.backgroundColor.GetCurrentColor();
				}
				if (Buttons.GetIndex("Transparent Theme").enabled)
				{
					((Renderer)vRRigFromPlayer.mainSkin).material.color = new Color(((Renderer)vRRigFromPlayer.mainSkin).material.color.r, ((Renderer)vRRigFromPlayer.mainSkin).material.color.g, ((Renderer)vRRigFromPlayer.mainSkin).material.color.b, 0.5f);
				}
			}
			else if (val.GetTargetOf(val2) == NetPlayer.op_Implicit(PhotonNetwork.LocalPlayer))
			{
				((Renderer)vRRigFromPlayer.mainSkin).material.shader = Shader.Find("GUI/Text Shader");
				((Renderer)vRRigFromPlayer.mainSkin).material.color = Color.red;
				if (Buttons.GetIndex("Transparent Theme").enabled)
				{
					((Renderer)vRRigFromPlayer.mainSkin).material.color = new Color(((Renderer)vRRigFromPlayer.mainSkin).material.color.r, ((Renderer)vRRigFromPlayer.mainSkin).material.color.g, ((Renderer)vRRigFromPlayer.mainSkin).material.color.b, 0.5f);
				}
			}
			else
			{
				((Renderer)vRRigFromPlayer.mainSkin).material.shader = Shader.Find("GorillaTag/UberShader");
				if (((Object)((Renderer)vRRigFromPlayer.mainSkin).material).name.Contains("gorilla_body"))
				{
					((Renderer)vRRigFromPlayer.mainSkin).material.color = vRRigFromPlayer.playerColor;
				}
			}
		}
	}

	public static void DisableChams()
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal))
		{
			Material[] materials = ((Renderer)item.mainSkin).materials;
			foreach (Material val in materials)
			{
				val.shader = Shader.Find("GorillaTag/UberShader");
			}
			if (((Object)((Renderer)item.mainSkin).material).name.Contains("gorilla_body"))
			{
				((Renderer)item.mainSkin).material.color = item.playerColor;
			}
		}
	}

	public static void CasualBoxESP()
	{
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, GameObject> item in boxESP.Where((KeyValuePair<VRRig, GameObject> box) => !VRRigCache.ActiveRigs.Contains(box.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			boxESP.Remove(item2);
		}
		foreach (VRRig item3 in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal))
		{
			if (!boxESP.TryGetValue(item3, out var value))
			{
				value = GameObject.CreatePrimitive((PrimitiveType)3);
				Object.Destroy((Object)(object)value.GetComponent<BoxCollider>());
				value.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
				value.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
				boxESP.Add(item3, value);
			}
			Color color = item3.playerColor;
			if (enabled)
			{
				color = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				color.a = 0.5f;
			}
			if (enabled2)
			{
				value.layer = 19;
			}
			value.GetComponent<Renderer>().material.color = color;
			value.transform.position = ((Component)item3).transform.position;
			value.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
		}
	}

	public static void InfectionBoxESP()
	{
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		bool flag = VRRig.LocalRig.IsTagged();
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, GameObject> item in boxESP.Where((KeyValuePair<VRRig, GameObject> box) => !VRRigCache.ActiveRigs.Contains(box.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			boxESP.Remove(item2);
		}
		foreach (VRRig item3 in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal))
		{
			if (!boxESP.TryGetValue(item3, out var value))
			{
				value = GameObject.CreatePrimitive((PrimitiveType)3);
				Object.Destroy((Object)(object)value.GetComponent<BoxCollider>());
				value.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
				value.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
				boxESP.Add(item3, value);
			}
			Color color = (flag ? item3.playerColor : item3.GetColor());
			if (enabled)
			{
				color = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				color.a = 0.5f;
			}
			if (enabled2)
			{
				value.layer = 19;
			}
			value.GetComponent<Renderer>().material.color = color;
			bool flag2 = item3.IsTagged();
			value.SetActive((flag ? (!flag2) : flag2) || GameModeUtilities.InfectedList().Count <= 0);
			value.transform.position = ((Component)item3).transform.position;
			value.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
		}
	}

	public static void HuntBoxESP()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom || (int)GorillaGameManager.instance.GameType() != 2)
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		List<VRRig> list = new List<VRRig>();
		GorillaHuntManager val = (GorillaHuntManager)GorillaGameManager.instance;
		NetPlayer targetOf = val.GetTargetOf(NetworkSystem.Instance.LocalPlayer);
		foreach (KeyValuePair<VRRig, GameObject> item in boxESP.Where((KeyValuePair<VRRig, GameObject> box) => !VRRigCache.ActiveRigs.Contains(box.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			boxESP.Remove(item2);
		}
		foreach (VRRig item3 in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal))
		{
			if (!boxESP.TryGetValue(item3, out var value))
			{
				value = GameObject.CreatePrimitive((PrimitiveType)3);
				Object.Destroy((Object)(object)value.GetComponent<BoxCollider>());
				value.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
				value.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
				boxESP.Add(item3, value);
			}
			NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(item3);
			NetPlayer targetOf2 = val.GetTargetOf(playerFromVRRig);
			Color color = ((playerFromVRRig == targetOf) ? item3.GetColor() : ((targetOf2 == NetworkSystem.Instance.LocalPlayer) ? Color.red : Color.clear));
			if (enabled)
			{
				color = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				color.a = 0.5f;
			}
			if (enabled2)
			{
				value.layer = 19;
			}
			value.GetComponent<Renderer>().material.color = color;
			value.SetActive(playerFromVRRig == targetOf || targetOf2 == NetworkSystem.Instance.LocalPlayer);
			value.transform.position = ((Component)item3).transform.position;
			value.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
		}
	}

	public static void DisableBoxESP()
	{
		foreach (KeyValuePair<VRRig, GameObject> item in boxESP)
		{
			Object.Destroy((Object)(object)item.Value);
		}
		boxESP.Clear();
	}

	public static void CasualHollowBoxESP()
	{
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		bool enabled4 = Buttons.GetIndex("Thin Tracers").enabled;
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, GameObject> item in hollowBoxESP.Where((KeyValuePair<VRRig, GameObject> box) => !VRRigCache.ActiveRigs.Contains(box.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			hollowBoxESP.Remove(item2);
		}
		foreach (VRRig item3 in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal))
		{
			if (!hollowBoxESP.TryGetValue(item3, out var value))
			{
				value = GameObject.CreatePrimitive((PrimitiveType)3);
				value.transform.position = ((Component)item3).transform.position;
				Object.Destroy((Object)(object)value.GetComponent<BoxCollider>());
				value.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
				value.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
				Renderer component = value.GetComponent<Renderer>();
				component.enabled = false;
				component.material.shader = Shader.Find("GUI/Text Shader");
				GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
				val.transform.SetParent(value.transform);
				val.transform.localPosition = new Vector3(0f, 0.5f, 0f);
				Object.Destroy((Object)(object)val.GetComponent<BoxCollider>());
				val.transform.localScale = new Vector3(1f, enabled4 ? 0.025f : 0.1f, 1f);
				val.transform.localRotation = Quaternion.identity;
				val.AddComponent<ClampColor>().targetRenderer = component;
				val = GameObject.CreatePrimitive((PrimitiveType)3);
				val.transform.SetParent(value.transform);
				val.transform.localPosition = new Vector3(0f, -0.5f, 0f);
				Object.Destroy((Object)(object)val.GetComponent<BoxCollider>());
				val.transform.localScale = new Vector3(enabled4 ? 1.025f : 1.1f, enabled4 ? 0.025f : 0.1f, 1f);
				val.transform.localRotation = Quaternion.identity;
				val.AddComponent<ClampColor>().targetRenderer = component;
				val = GameObject.CreatePrimitive((PrimitiveType)3);
				val.transform.SetParent(value.transform);
				val.transform.localPosition = new Vector3(0.5f, 0f, 0f);
				Object.Destroy((Object)(object)val.GetComponent<BoxCollider>());
				val.transform.localScale = new Vector3(enabled4 ? 0.025f : 0.1f, enabled4 ? 1.025f : 1.1f, 1f);
				val.transform.localRotation = Quaternion.identity;
				val.AddComponent<ClampColor>().targetRenderer = component;
				val = GameObject.CreatePrimitive((PrimitiveType)3);
				val.transform.SetParent(value.transform);
				val.transform.localPosition = new Vector3(-0.5f, 0f, 0f);
				Object.Destroy((Object)(object)val.GetComponent<BoxCollider>());
				val.transform.localScale = new Vector3(enabled4 ? 0.025f : 0.1f, enabled4 ? 1.025f : 1.1f, 1f);
				val.transform.localRotation = Quaternion.identity;
				val.AddComponent<ClampColor>().targetRenderer = component;
				hollowBoxESP.Add(item3, value);
			}
			Color color = item3.playerColor;
			if (enabled)
			{
				color = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				color.a = 0.5f;
			}
			if (enabled2)
			{
				value.layer = 19;
			}
			value.GetComponent<Renderer>().material.color = color;
			value.transform.position = ((Component)item3).transform.position;
			value.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
		}
	}

	public static void HollowInfectionBoxESP()
	{
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		bool enabled4 = Buttons.GetIndex("Thin Tracers").enabled;
		bool flag = VRRig.LocalRig.IsTagged();
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, GameObject> item in hollowBoxESP.Where((KeyValuePair<VRRig, GameObject> box) => !VRRigCache.ActiveRigs.Contains(box.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			hollowBoxESP.Remove(item2);
		}
		foreach (VRRig item3 in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal))
		{
			if (!hollowBoxESP.TryGetValue(item3, out var value))
			{
				value = GameObject.CreatePrimitive((PrimitiveType)3);
				value.transform.position = ((Component)item3).transform.position;
				Object.Destroy((Object)(object)value.GetComponent<BoxCollider>());
				value.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
				value.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
				Renderer component = value.GetComponent<Renderer>();
				component.enabled = false;
				component.material.shader = Shader.Find("GUI/Text Shader");
				GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
				val.transform.SetParent(value.transform);
				val.transform.localPosition = new Vector3(0f, 0.5f, 0f);
				Object.Destroy((Object)(object)val.GetComponent<BoxCollider>());
				val.transform.localScale = new Vector3(1f, enabled4 ? 0.025f : 0.1f, 1f);
				val.transform.localRotation = Quaternion.identity;
				val.AddComponent<ClampColor>().targetRenderer = component;
				val = GameObject.CreatePrimitive((PrimitiveType)3);
				val.transform.SetParent(value.transform);
				val.transform.localPosition = new Vector3(0f, -0.5f, 0f);
				Object.Destroy((Object)(object)val.GetComponent<BoxCollider>());
				val.transform.localScale = new Vector3(enabled4 ? 1.025f : 1.1f, enabled4 ? 0.025f : 0.1f, 1f);
				val.transform.localRotation = Quaternion.identity;
				val.AddComponent<ClampColor>().targetRenderer = component;
				val = GameObject.CreatePrimitive((PrimitiveType)3);
				val.transform.SetParent(value.transform);
				val.transform.localPosition = new Vector3(0.5f, 0f, 0f);
				Object.Destroy((Object)(object)val.GetComponent<BoxCollider>());
				val.transform.localScale = new Vector3(enabled4 ? 0.025f : 0.1f, enabled4 ? 1.025f : 1.1f, 1f);
				val.transform.localRotation = Quaternion.identity;
				val.AddComponent<ClampColor>().targetRenderer = component;
				val = GameObject.CreatePrimitive((PrimitiveType)3);
				val.transform.SetParent(value.transform);
				val.transform.localPosition = new Vector3(-0.5f, 0f, 0f);
				Object.Destroy((Object)(object)val.GetComponent<BoxCollider>());
				val.transform.localScale = new Vector3(enabled4 ? 0.025f : 0.1f, enabled4 ? 1.025f : 1.1f, 1f);
				val.transform.localRotation = Quaternion.identity;
				val.AddComponent<ClampColor>().targetRenderer = component;
				hollowBoxESP.Add(item3, value);
			}
			Color color = (flag ? item3.playerColor : item3.GetColor());
			if (enabled)
			{
				color = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				color.a = 0.5f;
			}
			if (enabled2)
			{
				value.layer = 19;
			}
			value.GetComponent<Renderer>().material.color = color;
			bool flag2 = item3.IsTagged();
			value.SetActive((flag ? (!flag2) : flag2) || GameModeUtilities.InfectedList().Count <= 0);
			value.transform.position = ((Component)item3).transform.position;
			value.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
		}
	}

	public static void HollowHuntBoxESP()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom || (int)GorillaGameManager.instance.GameType() != 2)
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		bool enabled4 = Buttons.GetIndex("Thin Tracers").enabled;
		List<VRRig> list = new List<VRRig>();
		GorillaHuntManager val = (GorillaHuntManager)GorillaGameManager.instance;
		NetPlayer targetOf = val.GetTargetOf(NetworkSystem.Instance.LocalPlayer);
		foreach (KeyValuePair<VRRig, GameObject> item in hollowBoxESP.Where((KeyValuePair<VRRig, GameObject> box) => !VRRigCache.ActiveRigs.Contains(box.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			hollowBoxESP.Remove(item2);
		}
		foreach (VRRig item3 in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal))
		{
			if (!hollowBoxESP.TryGetValue(item3, out var value))
			{
				value = GameObject.CreatePrimitive((PrimitiveType)3);
				value.transform.position = ((Component)item3).transform.position;
				Object.Destroy((Object)(object)value.GetComponent<BoxCollider>());
				value.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
				value.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
				Renderer component = value.GetComponent<Renderer>();
				component.enabled = false;
				component.material.shader = Shader.Find("GUI/Text Shader");
				GameObject val2 = GameObject.CreatePrimitive((PrimitiveType)3);
				val2.transform.SetParent(value.transform);
				val2.transform.localPosition = new Vector3(0f, 0.5f, 0f);
				Object.Destroy((Object)(object)val2.GetComponent<BoxCollider>());
				val2.transform.localScale = new Vector3(1f, enabled4 ? 0.025f : 0.1f, 1f);
				val2.transform.localRotation = Quaternion.identity;
				val2.AddComponent<ClampColor>().targetRenderer = component;
				val2 = GameObject.CreatePrimitive((PrimitiveType)3);
				val2.transform.SetParent(value.transform);
				val2.transform.localPosition = new Vector3(0f, -0.5f, 0f);
				Object.Destroy((Object)(object)val2.GetComponent<BoxCollider>());
				val2.transform.localScale = new Vector3(enabled4 ? 1.025f : 1.1f, enabled4 ? 0.025f : 0.1f, 1f);
				val2.transform.localRotation = Quaternion.identity;
				val2.AddComponent<ClampColor>().targetRenderer = component;
				val2 = GameObject.CreatePrimitive((PrimitiveType)3);
				val2.transform.SetParent(value.transform);
				val2.transform.localPosition = new Vector3(0.5f, 0f, 0f);
				Object.Destroy((Object)(object)val2.GetComponent<BoxCollider>());
				val2.transform.localScale = new Vector3(enabled4 ? 0.025f : 0.1f, enabled4 ? 1.025f : 1.1f, 1f);
				val2.transform.localRotation = Quaternion.identity;
				val2.AddComponent<ClampColor>().targetRenderer = component;
				val2 = GameObject.CreatePrimitive((PrimitiveType)3);
				val2.transform.SetParent(value.transform);
				val2.transform.localPosition = new Vector3(-0.5f, 0f, 0f);
				Object.Destroy((Object)(object)val2.GetComponent<BoxCollider>());
				val2.transform.localScale = new Vector3(enabled4 ? 0.025f : 0.1f, enabled4 ? 1.025f : 1.1f, 1f);
				val2.transform.localRotation = Quaternion.identity;
				val2.AddComponent<ClampColor>().targetRenderer = component;
				hollowBoxESP.Add(item3, value);
			}
			NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(item3);
			NetPlayer targetOf2 = val.GetTargetOf(playerFromVRRig);
			Color color = ((playerFromVRRig == targetOf) ? item3.GetColor() : ((targetOf2 == NetworkSystem.Instance.LocalPlayer) ? Color.red : Color.clear));
			if (enabled)
			{
				color = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				color.a = 0.5f;
			}
			if (enabled2)
			{
				value.layer = 19;
			}
			value.GetComponent<Renderer>().material.color = color;
			value.SetActive(playerFromVRRig == targetOf || targetOf2 == NetworkSystem.Instance.LocalPlayer);
			value.transform.position = ((Component)item3).transform.position;
			value.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
		}
	}

	public static void DisableHollowBoxESP()
	{
		foreach (KeyValuePair<VRRig, GameObject> item in hollowBoxESP)
		{
			Object.Destroy((Object)(object)item.Value);
		}
		hollowBoxESP.Clear();
	}

	public static void CasualBreadcrumbs()
	{
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, TrailRenderer> item in breadcrumbs.Where((KeyValuePair<VRRig, TrailRenderer> lines) => !VRRigCache.ActiveRigs.Contains(lines.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			breadcrumbs.Remove(item2);
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		bool enabled4 = Buttons.GetIndex("Thin Tracers").enabled;
		bool enabled5 = Buttons.GetIndex("Short Breadcrumbs").enabled;
		foreach (VRRig item3 in VRRigCache.ActiveRigs.Where((VRRig rig) => !rig.isLocal))
		{
			if (!breadcrumbs.TryGetValue(item3, out var value))
			{
				value = GTExt.GetOrAddComponent<TrailRenderer>(((Component)item3.head.rigTarget).gameObject);
				value.minVertexDistance = 0.05f;
				if (Main.smoothLines)
				{
					value.numCapVertices = 10;
					value.numCornerVertices = 5;
				}
				((Renderer)value).material.shader = Shader.Find("GUI/Text Shader");
				value.time = (enabled5 ? 1f : 10f);
				breadcrumbs.Add(item3, value);
			}
			value.startWidth = (enabled4 ? 0.0075f : 0.025f);
			value.endWidth = (enabled4 ? 0.0075f : 0.025f);
			if (enabled2)
			{
				((Component)value).gameObject.layer = 19;
			}
			Color val = item3.GetColor();
			if (enabled)
			{
				val = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				((Color)(ref val))._002Ector(val.r, val.g, val.b, 0.5f);
			}
			value.startColor = val;
			value.endColor = val;
		}
	}

	public static void InfectionBreadcrumbs()
	{
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, TrailRenderer> item in breadcrumbs.Where((KeyValuePair<VRRig, TrailRenderer> lines) => !VRRigCache.ActiveRigs.Contains(lines.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			breadcrumbs.Remove(item2);
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		bool enabled4 = Buttons.GetIndex("Thin Tracers").enabled;
		bool enabled5 = Buttons.GetIndex("Short Breadcrumbs").enabled;
		bool flag = VRRig.LocalRig.IsTagged();
		foreach (VRRig item3 in VRRigCache.ActiveRigs.Where((VRRig rig) => !rig.isLocal))
		{
			if (!breadcrumbs.TryGetValue(item3, out var value))
			{
				value = GTExt.GetOrAddComponent<TrailRenderer>(((Component)item3.head.rigTarget).gameObject);
				value.minVertexDistance = 0.05f;
				if (Main.smoothLines)
				{
					value.numCapVertices = 10;
					value.numCornerVertices = 5;
				}
				((Renderer)value).material.shader = Shader.Find("GUI/Text Shader");
				value.time = (enabled5 ? 1f : 10f);
				breadcrumbs.Add(item3, value);
			}
			value.startWidth = (enabled4 ? 0.0075f : 0.025f);
			value.endWidth = (enabled4 ? 0.0075f : 0.025f);
			bool flag2 = item3.IsTagged();
			Color val = (flag ? item3.playerColor : item3.GetColor());
			if (enabled)
			{
				val = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				val.a = 0.5f;
			}
			if (enabled2)
			{
				((Component)value).gameObject.layer = 19;
			}
			value.startColor = val;
			value.endColor = val;
			((Renderer)value).enabled = (flag ? (!flag2) : flag2) || GameModeUtilities.InfectedList().Count <= 0;
		}
	}

	public static void HuntBreadcrumbs()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Invalid comparison between Unknown and I4
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Expected O, but got Unknown
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom || (int)GorillaGameManager.instance.GameType() != 2)
		{
			return;
		}
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, TrailRenderer> item in breadcrumbs.Where((KeyValuePair<VRRig, TrailRenderer> lines) => !VRRigCache.ActiveRigs.Contains(lines.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			breadcrumbs.Remove(item2);
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		bool enabled4 = Buttons.GetIndex("Thin Tracers").enabled;
		bool enabled5 = Buttons.GetIndex("Short Breadcrumbs").enabled;
		GorillaHuntManager val = (GorillaHuntManager)GorillaGameManager.instance;
		NetPlayer targetOf = val.GetTargetOf(NetworkSystem.Instance.LocalPlayer);
		foreach (VRRig item3 in VRRigCache.ActiveRigs.Where((VRRig rig) => !rig.isLocal))
		{
			if (!breadcrumbs.TryGetValue(item3, out var value))
			{
				value = GTExt.GetOrAddComponent<TrailRenderer>(((Component)item3.head.rigTarget).gameObject);
				value.minVertexDistance = 0.05f;
				if (Main.smoothLines)
				{
					value.numCapVertices = 10;
					value.numCornerVertices = 5;
				}
				((Renderer)value).material.shader = Shader.Find("GUI/Text Shader");
				value.time = (enabled5 ? 1f : 10f);
				breadcrumbs.Add(item3, value);
			}
			value.startWidth = (enabled4 ? 0.0075f : 0.025f);
			value.endWidth = (enabled4 ? 0.0075f : 0.025f);
			NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(item3);
			NetPlayer targetOf2 = val.GetTargetOf(playerFromVRRig);
			Color val2 = ((playerFromVRRig == targetOf) ? item3.GetColor() : ((targetOf2 == NetworkSystem.Instance.LocalPlayer) ? Color.red : Color.clear));
			if (enabled)
			{
				val2 = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				val2.a = 0.5f;
			}
			if (enabled2)
			{
				((Component)value).gameObject.layer = 19;
			}
			value.startColor = val2;
			value.endColor = val2;
			((Renderer)value).enabled = playerFromVRRig == targetOf || targetOf2 == NetworkSystem.Instance.LocalPlayer;
		}
	}

	public static void DisableBreadcrumbs()
	{
		foreach (KeyValuePair<VRRig, TrailRenderer> breadcrumb in breadcrumbs)
		{
			Object.Destroy((Object)(object)breadcrumb.Value);
		}
		breadcrumbs.Clear();
	}

	public static bool DoPerformanceCheck()
	{
		if (PerformanceVisuals)
		{
			if (Time.time < PerformanceVisualDelay)
			{
				if (Time.frameCount != DelayChangeStep)
				{
					return true;
				}
			}
			else
			{
				PerformanceVisualDelay = Time.time + PerformanceModeStep;
				DelayChangeStep = Time.frameCount;
			}
		}
		return false;
	}

	public static void ShowButtonColliders()
	{
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)LeftSphere == (Object)null || (Object)(object)RightSphere == (Object)null)
		{
			if ((Object)(object)LeftSphere == (Object)null)
			{
				LeftSphere = GameObject.CreatePrimitive((PrimitiveType)0);
				Object.Destroy((Object)(object)LeftSphere.GetComponent<SphereCollider>());
				LeftSphere.transform.parent = GorillaTagger.Instance.leftHandTransform;
				LeftSphere.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
			}
			if ((Object)(object)RightSphere == (Object)null)
			{
				RightSphere = GameObject.CreatePrimitive((PrimitiveType)0);
				Object.Destroy((Object)(object)RightSphere.GetComponent<SphereCollider>());
				RightSphere.transform.parent = GorillaTagger.Instance.leftHandTransform;
				RightSphere.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
			}
		}
		LeftSphere.GetComponent<Renderer>().material.color = Main.backgroundColor.GetCurrentColor();
		LeftSphere.transform.localPosition = Main.pointerOffset;
		RightSphere.GetComponent<Renderer>().material.color = Main.backgroundColor.GetCurrentColor();
		RightSphere.transform.localPosition = Main.pointerOffset;
	}

	public static void HideButtonColliders()
	{
		Object.Destroy((Object)(object)LeftSphere);
		Object.Destroy((Object)(object)RightSphere);
		LeftSphere = null;
		RightSphere = null;
	}

	public static void AutomaticESP(Action infection, Action hunt, Action other)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected I4, but got Unknown
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		try
		{
			if ((Object)(object)GorillaGameManager.instance == (Object)null)
			{
				other();
				return;
			}
			GameModeType val = GorillaGameManager.instance.GameType();
			GameModeType val2 = val;
			switch (val2 - 1)
			{
			case 0:
			case 3:
			case 4:
			case 5:
			case 8:
			case 9:
				infection();
				break;
			case 1:
				hunt();
				break;
			default:
				other();
				break;
			}
		}
		catch
		{
		}
	}

	public static void CasualTracers()
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		if (DoPerformanceCheck())
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Transparent Theme").enabled;
		float num = (Buttons.GetIndex("Thin Tracers").enabled ? 0.0075f : 0.025f) * (Main.scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
		Color currentColor = Main.backgroundColor.GetCurrentColor();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.isLocal)
			{
				Color val = activeRig.playerColor;
				LineRenderer lineRender = GetLineRender();
				if (enabled)
				{
					val = currentColor;
				}
				if (enabled2)
				{
					val.a = 0.5f;
				}
				lineRender.startColor = val;
				lineRender.endColor = val;
				lineRender.startWidth = num;
				lineRender.endWidth = num;
				lineRender.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
				lineRender.SetPosition(1, ((Component)activeRig).transform.position);
			}
		}
	}

	public static void NearestTracer()
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		if (DoPerformanceCheck())
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Transparent Theme").enabled;
		float num = (Buttons.GetIndex("Thin Tracers").enabled ? 0.0075f : 0.025f) * (Main.scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
		Color currentColor = Main.backgroundColor.GetCurrentColor();
		float distance = float.MaxValue;
		VRRig val = VRRig.LocalRig;
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig rig) => distance > Vector3.Distance(((Component)rig).transform.position, ((Component)VRRig.LocalRig).transform.position) && !rig.isLocal))
		{
			distance = Vector3.Distance(((Component)item).transform.position, ((Component)VRRig.LocalRig).transform.position);
			val = item;
		}
		if (!val.isLocal)
		{
			Color val2 = val.playerColor;
			LineRenderer lineRender = GetLineRender();
			if (enabled)
			{
				val2 = currentColor;
			}
			if (enabled2)
			{
				val2.a = 0.5f;
			}
			lineRender.startColor = val2;
			lineRender.endColor = val2;
			lineRender.startWidth = num;
			lineRender.endWidth = num;
			lineRender.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
			lineRender.SetPosition(1, ((Component)val).transform.position);
		}
	}

	public static void FarthestTracer()
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		if (DoPerformanceCheck())
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Transparent Theme").enabled;
		float num = (Buttons.GetIndex("Thin Tracers").enabled ? 0.0075f : 0.025f) * (Main.scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
		Color currentColor = Main.backgroundColor.GetCurrentColor();
		float num2 = 0f;
		VRRig val = null;
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.isLocal)
			{
				float num3 = Vector3.Distance(((Component)activeRig).transform.position, ((Component)VRRig.LocalRig).transform.position);
				if (num3 > num2)
				{
					num2 = num3;
					val = activeRig;
				}
			}
		}
		if ((Object)(object)val != (Object)null)
		{
			Color val2 = val.playerColor;
			LineRenderer lineRender = GetLineRender();
			if (enabled)
			{
				val2 = currentColor;
			}
			if (enabled2)
			{
				val2.a = 0.5f;
			}
			lineRender.startColor = val2;
			lineRender.endColor = val2;
			lineRender.startWidth = num;
			lineRender.endWidth = num;
			lineRender.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
			lineRender.SetPosition(1, ((Component)val).transform.position);
		}
	}

	private static float GetDist(VRRig rig)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Distance(((Component)rig).transform.position, ((Component)VRRig.LocalRig).transform.position);
	}

	private static VRRig FindNearest()
	{
		float num = float.MaxValue;
		VRRig result = null;
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.isLocal)
			{
				float dist = GetDist(activeRig);
				if (dist < num)
				{
					num = dist;
					result = activeRig;
				}
			}
		}
		return result;
	}

	private static VRRig FindFarthest()
	{
		float num = 0f;
		VRRig result = null;
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.isLocal)
			{
				float dist = GetDist(activeRig);
				if (dist > num)
				{
					num = dist;
					result = activeRig;
				}
			}
		}
		return result;
	}

	public static void NearestBoxESP()
	{
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, GameObject> item in boxESP.Where((KeyValuePair<VRRig, GameObject> box) => !VRRigCache.ActiveRigs.Contains(box.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			boxESP.Remove(item2);
		}
		VRRig val = FindNearest();
		list.Clear();
		foreach (KeyValuePair<VRRig, GameObject> item3 in boxESP)
		{
			if ((Object)(object)item3.Key != (Object)(object)val)
			{
				list.Add(item3.Key);
				Object.Destroy((Object)(object)item3.Value);
			}
		}
		foreach (VRRig item4 in list)
		{
			boxESP.Remove(item4);
		}
		if (!((Object)(object)val == (Object)null))
		{
			if (!boxESP.TryGetValue(val, out var value))
			{
				value = GameObject.CreatePrimitive((PrimitiveType)3);
				Object.Destroy((Object)(object)value.GetComponent<BoxCollider>());
				value.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
				value.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
				boxESP.Add(val, value);
			}
			Color color = val.playerColor;
			if (enabled)
			{
				color = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				color.a = 0.5f;
			}
			if (enabled2)
			{
				value.layer = 19;
			}
			value.GetComponent<Renderer>().material.color = color;
			value.transform.position = ((Component)val).transform.position;
			value.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
		}
	}

	public static void FarthestBoxESP()
	{
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, GameObject> item in boxESP.Where((KeyValuePair<VRRig, GameObject> box) => !VRRigCache.ActiveRigs.Contains(box.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			boxESP.Remove(item2);
		}
		VRRig val = FindFarthest();
		list.Clear();
		foreach (KeyValuePair<VRRig, GameObject> item3 in boxESP)
		{
			if ((Object)(object)item3.Key != (Object)(object)val)
			{
				list.Add(item3.Key);
				Object.Destroy((Object)(object)item3.Value);
			}
		}
		foreach (VRRig item4 in list)
		{
			boxESP.Remove(item4);
		}
		if (!((Object)(object)val == (Object)null))
		{
			if (!boxESP.TryGetValue(val, out var value))
			{
				value = GameObject.CreatePrimitive((PrimitiveType)3);
				Object.Destroy((Object)(object)value.GetComponent<BoxCollider>());
				value.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
				value.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
				boxESP.Add(val, value);
			}
			Color color = val.playerColor;
			if (enabled)
			{
				color = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				color.a = 0.5f;
			}
			if (enabled2)
			{
				value.layer = 19;
			}
			value.GetComponent<Renderer>().material.color = color;
			value.transform.position = ((Component)val).transform.position;
			value.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
		}
	}

	public static void NearestHollowBoxESP()
	{
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0541: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		bool enabled4 = Buttons.GetIndex("Thin Tracers").enabled;
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, GameObject> item in hollowBoxESP.Where((KeyValuePair<VRRig, GameObject> box) => !VRRigCache.ActiveRigs.Contains(box.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			hollowBoxESP.Remove(item2);
		}
		VRRig val = FindNearest();
		list.Clear();
		foreach (KeyValuePair<VRRig, GameObject> item3 in hollowBoxESP)
		{
			if ((Object)(object)item3.Key != (Object)(object)val)
			{
				list.Add(item3.Key);
				Object.Destroy((Object)(object)item3.Value);
			}
		}
		foreach (VRRig item4 in list)
		{
			hollowBoxESP.Remove(item4);
		}
		if (!((Object)(object)val == (Object)null))
		{
			if (!hollowBoxESP.TryGetValue(val, out var value))
			{
				value = GameObject.CreatePrimitive((PrimitiveType)3);
				value.transform.position = ((Component)val).transform.position;
				Object.Destroy((Object)(object)value.GetComponent<BoxCollider>());
				value.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
				value.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
				Renderer component = value.GetComponent<Renderer>();
				component.enabled = false;
				component.material.shader = Shader.Find("GUI/Text Shader");
				GameObject val2 = GameObject.CreatePrimitive((PrimitiveType)3);
				val2.transform.SetParent(value.transform);
				val2.transform.localPosition = new Vector3(0f, 0.5f, 0f);
				Object.Destroy((Object)(object)val2.GetComponent<BoxCollider>());
				val2.transform.localScale = new Vector3(1f, enabled4 ? 0.025f : 0.1f, 1f);
				val2.transform.localRotation = Quaternion.identity;
				val2.AddComponent<ClampColor>().targetRenderer = component;
				val2 = GameObject.CreatePrimitive((PrimitiveType)3);
				val2.transform.SetParent(value.transform);
				val2.transform.localPosition = new Vector3(0f, -0.5f, 0f);
				Object.Destroy((Object)(object)val2.GetComponent<BoxCollider>());
				val2.transform.localScale = new Vector3(enabled4 ? 1.025f : 1.1f, enabled4 ? 0.025f : 0.1f, 1f);
				val2.transform.localRotation = Quaternion.identity;
				val2.AddComponent<ClampColor>().targetRenderer = component;
				val2 = GameObject.CreatePrimitive((PrimitiveType)3);
				val2.transform.SetParent(value.transform);
				val2.transform.localPosition = new Vector3(0.5f, 0f, 0f);
				Object.Destroy((Object)(object)val2.GetComponent<BoxCollider>());
				val2.transform.localScale = new Vector3(enabled4 ? 0.025f : 0.1f, enabled4 ? 1.025f : 1.1f, 1f);
				val2.transform.localRotation = Quaternion.identity;
				val2.AddComponent<ClampColor>().targetRenderer = component;
				val2 = GameObject.CreatePrimitive((PrimitiveType)3);
				val2.transform.SetParent(value.transform);
				val2.transform.localPosition = new Vector3(-0.5f, 0f, 0f);
				Object.Destroy((Object)(object)val2.GetComponent<BoxCollider>());
				val2.transform.localScale = new Vector3(enabled4 ? 0.025f : 0.1f, enabled4 ? 1.025f : 1.1f, 1f);
				val2.transform.localRotation = Quaternion.identity;
				val2.AddComponent<ClampColor>().targetRenderer = component;
				hollowBoxESP.Add(val, value);
			}
			Color color = val.playerColor;
			if (enabled)
			{
				color = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				color.a = 0.5f;
			}
			if (enabled2)
			{
				value.layer = 19;
			}
			value.GetComponent<Renderer>().material.color = color;
			value.transform.position = ((Component)val).transform.position;
			value.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
		}
	}

	public static void FarthestHollowBoxESP()
	{
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0541: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		bool enabled4 = Buttons.GetIndex("Thin Tracers").enabled;
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, GameObject> item in hollowBoxESP.Where((KeyValuePair<VRRig, GameObject> box) => !VRRigCache.ActiveRigs.Contains(box.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			hollowBoxESP.Remove(item2);
		}
		VRRig val = FindFarthest();
		list.Clear();
		foreach (KeyValuePair<VRRig, GameObject> item3 in hollowBoxESP)
		{
			if ((Object)(object)item3.Key != (Object)(object)val)
			{
				list.Add(item3.Key);
				Object.Destroy((Object)(object)item3.Value);
			}
		}
		foreach (VRRig item4 in list)
		{
			hollowBoxESP.Remove(item4);
		}
		if (!((Object)(object)val == (Object)null))
		{
			if (!hollowBoxESP.TryGetValue(val, out var value))
			{
				value = GameObject.CreatePrimitive((PrimitiveType)3);
				value.transform.position = ((Component)val).transform.position;
				Object.Destroy((Object)(object)value.GetComponent<BoxCollider>());
				value.transform.localScale = new Vector3(0.5f, 0.5f, 0f);
				value.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
				Renderer component = value.GetComponent<Renderer>();
				component.enabled = false;
				component.material.shader = Shader.Find("GUI/Text Shader");
				GameObject val2 = GameObject.CreatePrimitive((PrimitiveType)3);
				val2.transform.SetParent(value.transform);
				val2.transform.localPosition = new Vector3(0f, 0.5f, 0f);
				Object.Destroy((Object)(object)val2.GetComponent<BoxCollider>());
				val2.transform.localScale = new Vector3(1f, enabled4 ? 0.025f : 0.1f, 1f);
				val2.transform.localRotation = Quaternion.identity;
				val2.AddComponent<ClampColor>().targetRenderer = component;
				val2 = GameObject.CreatePrimitive((PrimitiveType)3);
				val2.transform.SetParent(value.transform);
				val2.transform.localPosition = new Vector3(0f, -0.5f, 0f);
				Object.Destroy((Object)(object)val2.GetComponent<BoxCollider>());
				val2.transform.localScale = new Vector3(enabled4 ? 1.025f : 1.1f, enabled4 ? 0.025f : 0.1f, 1f);
				val2.transform.localRotation = Quaternion.identity;
				val2.AddComponent<ClampColor>().targetRenderer = component;
				val2 = GameObject.CreatePrimitive((PrimitiveType)3);
				val2.transform.SetParent(value.transform);
				val2.transform.localPosition = new Vector3(0.5f, 0f, 0f);
				Object.Destroy((Object)(object)val2.GetComponent<BoxCollider>());
				val2.transform.localScale = new Vector3(enabled4 ? 0.025f : 0.1f, enabled4 ? 1.025f : 1.1f, 1f);
				val2.transform.localRotation = Quaternion.identity;
				val2.AddComponent<ClampColor>().targetRenderer = component;
				val2 = GameObject.CreatePrimitive((PrimitiveType)3);
				val2.transform.SetParent(value.transform);
				val2.transform.localPosition = new Vector3(-0.5f, 0f, 0f);
				Object.Destroy((Object)(object)val2.GetComponent<BoxCollider>());
				val2.transform.localScale = new Vector3(enabled4 ? 0.025f : 0.1f, enabled4 ? 1.025f : 1.1f, 1f);
				val2.transform.localRotation = Quaternion.identity;
				val2.AddComponent<ClampColor>().targetRenderer = component;
				hollowBoxESP.Add(val, value);
			}
			Color color = val.playerColor;
			if (enabled)
			{
				color = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				color.a = 0.5f;
			}
			if (enabled2)
			{
				value.layer = 19;
			}
			value.GetComponent<Renderer>().material.color = color;
			value.transform.position = ((Component)val).transform.position;
			value.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
		}
	}

	public static void NearestBreadcrumbs()
	{
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, TrailRenderer> item in breadcrumbs.Where((KeyValuePair<VRRig, TrailRenderer> lines) => !VRRigCache.ActiveRigs.Contains(lines.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			breadcrumbs.Remove(item2);
		}
		VRRig val = FindNearest();
		list.Clear();
		foreach (KeyValuePair<VRRig, TrailRenderer> breadcrumb in breadcrumbs)
		{
			if ((Object)(object)breadcrumb.Key != (Object)(object)val)
			{
				list.Add(breadcrumb.Key);
				Object.Destroy((Object)(object)breadcrumb.Value);
			}
		}
		foreach (VRRig item3 in list)
		{
			breadcrumbs.Remove(item3);
		}
		if ((Object)(object)val == (Object)null)
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		bool enabled4 = Buttons.GetIndex("Thin Tracers").enabled;
		bool enabled5 = Buttons.GetIndex("Short Breadcrumbs").enabled;
		if (!breadcrumbs.TryGetValue(val, out var value))
		{
			value = GTExt.GetOrAddComponent<TrailRenderer>(((Component)val.head.rigTarget).gameObject);
			value.minVertexDistance = 0.05f;
			if (Main.smoothLines)
			{
				value.numCapVertices = 10;
				value.numCornerVertices = 5;
			}
			((Renderer)value).material.shader = Shader.Find("GUI/Text Shader");
			value.time = (enabled5 ? 1f : 10f);
			breadcrumbs.Add(val, value);
		}
		value.startWidth = (enabled4 ? 0.0075f : 0.025f);
		value.endWidth = (enabled4 ? 0.0075f : 0.025f);
		if (enabled2)
		{
			((Component)value).gameObject.layer = 19;
		}
		Color val2 = val.GetColor();
		if (enabled)
		{
			val2 = Main.backgroundColor.GetCurrentColor();
		}
		if (enabled3)
		{
			((Color)(ref val2))._002Ector(val2.r, val2.g, val2.b, 0.5f);
		}
		value.startColor = val2;
		value.endColor = val2;
	}

	public static void FarthestBreadcrumbs()
	{
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, TrailRenderer> item in breadcrumbs.Where((KeyValuePair<VRRig, TrailRenderer> lines) => !VRRigCache.ActiveRigs.Contains(lines.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			breadcrumbs.Remove(item2);
		}
		VRRig val = FindFarthest();
		list.Clear();
		foreach (KeyValuePair<VRRig, TrailRenderer> breadcrumb in breadcrumbs)
		{
			if ((Object)(object)breadcrumb.Key != (Object)(object)val)
			{
				list.Add(breadcrumb.Key);
				Object.Destroy((Object)(object)breadcrumb.Value);
			}
		}
		foreach (VRRig item3 in list)
		{
			breadcrumbs.Remove(item3);
		}
		if ((Object)(object)val == (Object)null)
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		bool enabled4 = Buttons.GetIndex("Thin Tracers").enabled;
		bool enabled5 = Buttons.GetIndex("Short Breadcrumbs").enabled;
		if (!breadcrumbs.TryGetValue(val, out var value))
		{
			value = GTExt.GetOrAddComponent<TrailRenderer>(((Component)val.head.rigTarget).gameObject);
			value.minVertexDistance = 0.05f;
			if (Main.smoothLines)
			{
				value.numCapVertices = 10;
				value.numCornerVertices = 5;
			}
			((Renderer)value).material.shader = Shader.Find("GUI/Text Shader");
			value.time = (enabled5 ? 1f : 10f);
			breadcrumbs.Add(val, value);
		}
		value.startWidth = (enabled4 ? 0.0075f : 0.025f);
		value.endWidth = (enabled4 ? 0.0075f : 0.025f);
		if (enabled2)
		{
			((Component)value).gameObject.layer = 19;
		}
		Color val2 = val.GetColor();
		if (enabled)
		{
			val2 = Main.backgroundColor.GetCurrentColor();
		}
		if (enabled3)
		{
			((Color)(ref val2))._002Ector(val2.r, val2.g, val2.b, 0.5f);
		}
		value.startColor = val2;
		value.endColor = val2;
	}

	public static void NearestBoneESP()
	{
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		bool enabled4 = Buttons.GetIndex("Thin Tracers").enabled;
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, List<LineRenderer>> item in boneESP.Where((KeyValuePair<VRRig, List<LineRenderer>> boness) => !VRRigCache.ActiveRigs.Contains(boness.Key)))
		{
			list.Add(item.Key);
			foreach (LineRenderer item2 in item.Value)
			{
				Object.Destroy((Object)(object)item2);
			}
		}
		foreach (VRRig item3 in list)
		{
			boneESP.Remove(item3);
		}
		VRRig val = FindNearest();
		list.Clear();
		foreach (KeyValuePair<VRRig, List<LineRenderer>> item4 in boneESP)
		{
			if (!((Object)(object)item4.Key != (Object)(object)val))
			{
				continue;
			}
			list.Add(item4.Key);
			foreach (LineRenderer item5 in item4.Value)
			{
				Object.Destroy((Object)(object)item5);
			}
		}
		foreach (VRRig item6 in list)
		{
			boneESP.Remove(item6);
		}
		if ((Object)(object)val == (Object)null)
		{
			return;
		}
		if (!boneESP.TryGetValue(val, out var value))
		{
			value = new List<LineRenderer>();
			LineRenderer orAddComponent = GTExt.GetOrAddComponent<LineRenderer>(((Component)val.head.rigTarget).gameObject);
			if (Main.smoothLines)
			{
				orAddComponent.numCapVertices = 10;
				orAddComponent.numCornerVertices = 5;
			}
			((Renderer)orAddComponent).material.shader = Shader.Find("GUI/Text Shader");
			value.Add(orAddComponent);
			for (int num = 0; num < 19; num++)
			{
				LineRenderer orAddComponent2 = GTExt.GetOrAddComponent<LineRenderer>(((Component)val.mainSkin.bones[bones[num * 2]]).gameObject);
				if (Main.smoothLines)
				{
					orAddComponent2.numCapVertices = 10;
					orAddComponent2.numCornerVertices = 5;
				}
				((Renderer)orAddComponent2).material.shader = Shader.Find("GUI/Text Shader");
				value.Add(orAddComponent2);
			}
			boneESP.Add(val, value);
		}
		LineRenderer val2 = value[0];
		Color val3 = val.playerColor;
		if (enabled)
		{
			val3 = Main.backgroundColor.GetCurrentColor();
		}
		if (enabled3)
		{
			val3.a = 0.5f;
		}
		if (enabled2)
		{
			((Component)val2).gameObject.layer = 19;
		}
		val2.startWidth = (enabled4 ? 0.0075f : 0.025f);
		val2.endWidth = (enabled4 ? 0.0075f : 0.025f);
		val2.startColor = val3;
		val2.endColor = val3;
		val2.SetPosition(0, ((Component)val.head.rigTarget).transform.position + new Vector3(0f, 0.16f, 0f));
		val2.SetPosition(1, ((Component)val.head.rigTarget).transform.position - new Vector3(0f, 0.4f, 0f));
		for (int num2 = 0; num2 < 19; num2++)
		{
			val2 = value[num2 + 1];
			if (enabled2)
			{
				((Component)val2).gameObject.layer = 19;
			}
			val2.startWidth = (enabled4 ? 0.0075f : 0.025f);
			val2.endWidth = (enabled4 ? 0.0075f : 0.025f);
			val2.startColor = val3;
			val2.endColor = val3;
			((Renderer)val2).material.shader = Shader.Find("GUI/Text Shader");
			val2.SetPosition(0, val.mainSkin.bones[bones[num2 * 2]].position);
			val2.SetPosition(1, val.mainSkin.bones[bones[num2 * 2 + 1]].position);
		}
	}

	public static void FarthestBoneESP()
	{
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		bool enabled4 = Buttons.GetIndex("Thin Tracers").enabled;
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, List<LineRenderer>> item in boneESP.Where((KeyValuePair<VRRig, List<LineRenderer>> boness) => !VRRigCache.ActiveRigs.Contains(boness.Key)))
		{
			list.Add(item.Key);
			foreach (LineRenderer item2 in item.Value)
			{
				Object.Destroy((Object)(object)item2);
			}
		}
		foreach (VRRig item3 in list)
		{
			boneESP.Remove(item3);
		}
		VRRig val = FindFarthest();
		list.Clear();
		foreach (KeyValuePair<VRRig, List<LineRenderer>> item4 in boneESP)
		{
			if (!((Object)(object)item4.Key != (Object)(object)val))
			{
				continue;
			}
			list.Add(item4.Key);
			foreach (LineRenderer item5 in item4.Value)
			{
				Object.Destroy((Object)(object)item5);
			}
		}
		foreach (VRRig item6 in list)
		{
			boneESP.Remove(item6);
		}
		if ((Object)(object)val == (Object)null)
		{
			return;
		}
		if (!boneESP.TryGetValue(val, out var value))
		{
			value = new List<LineRenderer>();
			LineRenderer orAddComponent = GTExt.GetOrAddComponent<LineRenderer>(((Component)val.head.rigTarget).gameObject);
			if (Main.smoothLines)
			{
				orAddComponent.numCapVertices = 10;
				orAddComponent.numCornerVertices = 5;
			}
			((Renderer)orAddComponent).material.shader = Shader.Find("GUI/Text Shader");
			value.Add(orAddComponent);
			for (int num = 0; num < 19; num++)
			{
				LineRenderer orAddComponent2 = GTExt.GetOrAddComponent<LineRenderer>(((Component)val.mainSkin.bones[bones[num * 2]]).gameObject);
				if (Main.smoothLines)
				{
					orAddComponent2.numCapVertices = 10;
					orAddComponent2.numCornerVertices = 5;
				}
				((Renderer)orAddComponent2).material.shader = Shader.Find("GUI/Text Shader");
				value.Add(orAddComponent2);
			}
			boneESP.Add(val, value);
		}
		LineRenderer val2 = value[0];
		Color val3 = val.playerColor;
		if (enabled)
		{
			val3 = Main.backgroundColor.GetCurrentColor();
		}
		if (enabled3)
		{
			val3.a = 0.5f;
		}
		if (enabled2)
		{
			((Component)val2).gameObject.layer = 19;
		}
		val2.startWidth = (enabled4 ? 0.0075f : 0.025f);
		val2.endWidth = (enabled4 ? 0.0075f : 0.025f);
		val2.startColor = val3;
		val2.endColor = val3;
		val2.SetPosition(0, ((Component)val.head.rigTarget).transform.position + new Vector3(0f, 0.16f, 0f));
		val2.SetPosition(1, ((Component)val.head.rigTarget).transform.position - new Vector3(0f, 0.4f, 0f));
		for (int num2 = 0; num2 < 19; num2++)
		{
			val2 = value[num2 + 1];
			if (enabled2)
			{
				((Component)val2).gameObject.layer = 19;
			}
			val2.startWidth = (enabled4 ? 0.0075f : 0.025f);
			val2.endWidth = (enabled4 ? 0.0075f : 0.025f);
			val2.startColor = val3;
			val2.endColor = val3;
			((Renderer)val2).material.shader = Shader.Find("GUI/Text Shader");
			val2.SetPosition(0, val.mainSkin.bones[bones[num2 * 2]].position);
			val2.SetPosition(1, val.mainSkin.bones[bones[num2 * 2 + 1]].position);
		}
	}

	public static void NearestSkeletonESP()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		DisableSkeletonESP();
		VRRig val = FindNearest();
		if (!((Object)(object)val == (Object)null))
		{
			((Renderer)val.skeleton.renderer).enabled = true;
			((Renderer)val.skeleton.renderer).material.shader = Shader.Find("GUI/Text Shader");
			((Renderer)val.skeleton.renderer).material.color = val.playerColor;
			if (Buttons.GetIndex("Follow Menu Theme").enabled)
			{
				((Renderer)val.skeleton.renderer).material.color = Main.backgroundColor.GetCurrentColor();
			}
			if (Buttons.GetIndex("Transparent Theme").enabled)
			{
				((Renderer)val.skeleton.renderer).material.color = new Color(((Renderer)val.skeleton.renderer).material.color.r, ((Renderer)val.skeleton.renderer).material.color.g, ((Renderer)val.skeleton.renderer).material.color.b, 0.5f);
			}
		}
	}

	public static void FarthestSkeletonESP()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		DisableSkeletonESP();
		VRRig val = FindFarthest();
		if (!((Object)(object)val == (Object)null))
		{
			((Renderer)val.skeleton.renderer).enabled = true;
			((Renderer)val.skeleton.renderer).material.shader = Shader.Find("GUI/Text Shader");
			((Renderer)val.skeleton.renderer).material.color = val.playerColor;
			if (Buttons.GetIndex("Follow Menu Theme").enabled)
			{
				((Renderer)val.skeleton.renderer).material.color = Main.backgroundColor.GetCurrentColor();
			}
			if (Buttons.GetIndex("Transparent Theme").enabled)
			{
				((Renderer)val.skeleton.renderer).material.color = new Color(((Renderer)val.skeleton.renderer).material.color.r, ((Renderer)val.skeleton.renderer).material.color.g, ((Renderer)val.skeleton.renderer).material.color.b, 0.5f);
			}
		}
	}

	public static void NearestWireframeESP()
	{
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, SkinnedWireframeRenderer> item in wireframes.Where((KeyValuePair<VRRig, SkinnedWireframeRenderer> lines) => !VRRigCache.ActiveRigs.Contains(lines.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			wireframes.Remove(item2);
		}
		VRRig val = FindNearest();
		list.Clear();
		foreach (KeyValuePair<VRRig, SkinnedWireframeRenderer> wireframe in wireframes)
		{
			if ((Object)(object)wireframe.Key != (Object)(object)val)
			{
				list.Add(wireframe.Key);
				Object.Destroy((Object)(object)wireframe.Value);
			}
		}
		foreach (VRRig item3 in list)
		{
			wireframes.Remove(item3);
		}
		if ((Object)(object)val == (Object)null)
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		if (!wireframes.TryGetValue(val, out var value))
		{
			value = ComponentUtils.AddComponent<SkinnedWireframeRenderer>((Component)(object)val.mainSkin);
			wireframes.Add(val, value);
		}
		if (enabled2)
		{
			value.wireframeObj.layer = 19;
		}
		Color val2 = val.GetColor();
		if (enabled)
		{
			val2 = Main.backgroundColor.GetCurrentColor();
		}
		if (enabled3)
		{
			((Color)(ref val2))._002Ector(val2.r, val2.g, val2.b, 0.5f);
		}
		((Renderer)value.meshRenderer).material.color = val2;
		Vector3 val3 = ((Component)val).transform.position - ((Component)GorillaTagger.Instance.headCollider).transform.position;
		float num = Vector3.Angle(((Component)GorillaTagger.Instance.headCollider).transform.forward, val3);
		bool flag = num <= Camera.main.fieldOfView / 1.75f;
		flag &= Vector3.Distance(((Component)val).transform.position, ((Component)GorillaTagger.Instance.headCollider).transform.position) < 35f;
		flag &= Vector3.Distance(val.headMesh.transform.position, ((Component)GorillaTagger.Instance.headCollider).transform.position) < 35f;
		flag &= Vector3.Distance(val.leftHandTransform.position, ((Component)GorillaTagger.Instance.headCollider).transform.position) < 35f;
		flag = (((Behaviour)value).enabled = flag & (Vector3.Distance(val.rightHandTransform.position, ((Component)GorillaTagger.Instance.headCollider).transform.position) < 35f));
		((Renderer)value.meshRenderer).enabled = flag;
		if (!flag)
		{
			FixRigMaterialESPColors(val);
			((Renderer)val.mainSkin).material.shader = Shader.Find("GUI/Text Shader");
			((Renderer)val.mainSkin).material.color = val2;
			return;
		}
		((Renderer)val.mainSkin).material.shader = Shader.Find("GorillaTag/UberShader");
		if (((Object)((Renderer)val.mainSkin).material).name.Contains("gorilla_body"))
		{
			((Renderer)val.mainSkin).material.color = val.playerColor;
		}
	}

	public static void FarthestWireframeESP()
	{
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, SkinnedWireframeRenderer> item in wireframes.Where((KeyValuePair<VRRig, SkinnedWireframeRenderer> lines) => !VRRigCache.ActiveRigs.Contains(lines.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			wireframes.Remove(item2);
		}
		VRRig val = FindFarthest();
		list.Clear();
		foreach (KeyValuePair<VRRig, SkinnedWireframeRenderer> wireframe in wireframes)
		{
			if ((Object)(object)wireframe.Key != (Object)(object)val)
			{
				list.Add(wireframe.Key);
				Object.Destroy((Object)(object)wireframe.Value);
			}
		}
		foreach (VRRig item3 in list)
		{
			wireframes.Remove(item3);
		}
		if ((Object)(object)val == (Object)null)
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		if (!wireframes.TryGetValue(val, out var value))
		{
			value = ComponentUtils.AddComponent<SkinnedWireframeRenderer>((Component)(object)val.mainSkin);
			wireframes.Add(val, value);
		}
		if (enabled2)
		{
			value.wireframeObj.layer = 19;
		}
		Color val2 = val.GetColor();
		if (enabled)
		{
			val2 = Main.backgroundColor.GetCurrentColor();
		}
		if (enabled3)
		{
			((Color)(ref val2))._002Ector(val2.r, val2.g, val2.b, 0.5f);
		}
		((Renderer)value.meshRenderer).material.color = val2;
		Vector3 val3 = ((Component)val).transform.position - ((Component)GorillaTagger.Instance.headCollider).transform.position;
		float num = Vector3.Angle(((Component)GorillaTagger.Instance.headCollider).transform.forward, val3);
		bool flag = num <= Camera.main.fieldOfView / 1.75f;
		flag &= Vector3.Distance(((Component)val).transform.position, ((Component)GorillaTagger.Instance.headCollider).transform.position) < 35f;
		flag &= Vector3.Distance(val.headMesh.transform.position, ((Component)GorillaTagger.Instance.headCollider).transform.position) < 35f;
		flag &= Vector3.Distance(val.leftHandTransform.position, ((Component)GorillaTagger.Instance.headCollider).transform.position) < 35f;
		flag = (((Behaviour)value).enabled = flag & (Vector3.Distance(val.rightHandTransform.position, ((Component)GorillaTagger.Instance.headCollider).transform.position) < 35f));
		((Renderer)value.meshRenderer).enabled = flag;
		if (!flag)
		{
			FixRigMaterialESPColors(val);
			((Renderer)val.mainSkin).material.shader = Shader.Find("GUI/Text Shader");
			((Renderer)val.mainSkin).material.color = val2;
			return;
		}
		((Renderer)val.mainSkin).material.shader = Shader.Find("GorillaTag/UberShader");
		if (((Object)((Renderer)val.mainSkin).material).name.Contains("gorilla_body"))
		{
			((Renderer)val.mainSkin).material.color = val.playerColor;
		}
	}

	public static void NearestChams()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		DisableChams();
		VRRig val = FindNearest();
		if (!((Object)(object)val == (Object)null))
		{
			FixRigMaterialESPColors(val);
			((Renderer)val.mainSkin).material.shader = Shader.Find("GUI/Text Shader");
			((Renderer)val.mainSkin).material.color = val.playerColor;
			if (Buttons.GetIndex("Follow Menu Theme").enabled)
			{
				((Renderer)val.mainSkin).material.color = Main.backgroundColor.GetCurrentColor();
			}
			if (Buttons.GetIndex("Transparent Theme").enabled)
			{
				((Renderer)val.mainSkin).material.color = new Color(((Renderer)val.mainSkin).material.color.r, ((Renderer)val.mainSkin).material.color.g, ((Renderer)val.mainSkin).material.color.b, 0.5f);
			}
		}
	}

	public static void FarthestChams()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		DisableChams();
		VRRig val = FindFarthest();
		if (!((Object)(object)val == (Object)null))
		{
			FixRigMaterialESPColors(val);
			((Renderer)val.mainSkin).material.shader = Shader.Find("GUI/Text Shader");
			((Renderer)val.mainSkin).material.color = val.playerColor;
			if (Buttons.GetIndex("Follow Menu Theme").enabled)
			{
				((Renderer)val.mainSkin).material.color = Main.backgroundColor.GetCurrentColor();
			}
			if (Buttons.GetIndex("Transparent Theme").enabled)
			{
				((Renderer)val.mainSkin).material.color = new Color(((Renderer)val.mainSkin).material.color.r, ((Renderer)val.mainSkin).material.color.g, ((Renderer)val.mainSkin).material.color.b, 0.5f);
			}
		}
	}

	public static void NearestBeacons()
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		if (DoPerformanceCheck() || (Object)(object)GorillaGameManager.instance == (Object)null)
		{
			return;
		}
		VRRig val = FindNearest();
		if (!((Object)(object)val == (Object)null))
		{
			bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
			bool enabled2 = Buttons.GetIndex("Transparent Theme").enabled;
			bool enabled3 = Buttons.GetIndex("Thin Tracers").enabled;
			Color currentColor = Main.backgroundColor.GetCurrentColor();
			Color val2 = val.playerColor;
			LineRenderer lineRender = GetLineRender();
			if (enabled)
			{
				val2 = currentColor;
			}
			if (enabled2)
			{
				val2.a = 0.5f;
			}
			lineRender.startColor = val2;
			lineRender.endColor = val2;
			float endWidth = (lineRender.startWidth = (enabled3 ? 0.0075f : 0.025f));
			lineRender.endWidth = endWidth;
			lineRender.SetPosition(0, ((Component)val).transform.position + new Vector3(0f, 9999f, 0f));
			lineRender.SetPosition(1, ((Component)val).transform.position - new Vector3(0f, 9999f, 0f));
		}
	}

	public static void FarthestBeacons()
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		if (DoPerformanceCheck() || (Object)(object)GorillaGameManager.instance == (Object)null)
		{
			return;
		}
		VRRig val = FindFarthest();
		if (!((Object)(object)val == (Object)null))
		{
			bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
			bool enabled2 = Buttons.GetIndex("Transparent Theme").enabled;
			bool enabled3 = Buttons.GetIndex("Thin Tracers").enabled;
			Color currentColor = Main.backgroundColor.GetCurrentColor();
			Color val2 = val.playerColor;
			LineRenderer lineRender = GetLineRender();
			if (enabled)
			{
				val2 = currentColor;
			}
			if (enabled2)
			{
				val2.a = 0.5f;
			}
			lineRender.startColor = val2;
			lineRender.endColor = val2;
			float endWidth = (lineRender.startWidth = (enabled3 ? 0.0075f : 0.025f));
			lineRender.endWidth = endWidth;
			lineRender.SetPosition(0, ((Component)val).transform.position + new Vector3(0f, 9999f, 0f));
			lineRender.SetPosition(1, ((Component)val).transform.position - new Vector3(0f, 9999f, 0f));
		}
	}

	public static void NearestDistanceESP()
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		if (DoPerformanceCheck() || (Object)(object)GorillaGameManager.instance == (Object)null)
		{
			return;
		}
		VRRig val = FindNearest();
		if ((Object)(object)val == (Object)null)
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Transparent Theme").enabled;
		bool enabled3 = Buttons.GetIndex("Hidden on Camera").enabled;
		Color currentColor = Main.backgroundColor.GetCurrentColor();
		Color color = (enabled ? Main.textColors[0].GetCurrentColor() : Color.white);
		Color color2 = val.playerColor;
		if (enabled)
		{
			color2 = currentColor;
		}
		if (enabled2)
		{
			color2.a = 0.5f;
			color.a = 0.5f;
		}
		TextMeshPro nameTag = GetNameTag(enabled3);
		((Component)nameTag).gameObject.transform.position = ((Component)val).transform.position + new Vector3(0f, -0.2f, 0f);
		((Graphic)nameTag).color = color;
		_ = $"{Vector3.Distance(((Component)Camera.main).transform.position, ((Component)val).transform.position):F1}m";
		Transform[] componentsInChildren = ((Component)nameTag).gameObject.GetComponentsInChildren<Transform>();
		foreach (Transform val2 in componentsInChildren)
		{
			if (((Object)((Component)val2).gameObject).name == "bg")
			{
				((Component)val2).gameObject.GetComponent<Renderer>().material.color = color2;
				Bounds bounds = ((Component)nameTag).GetComponent<Renderer>().bounds;
				val2.localScale = new Vector3(((Bounds)(ref bounds)).size.x + 0.2f, 0.2f, 0.01f);
			}
		}
	}

	public static void FarthestDistanceESP()
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		if (DoPerformanceCheck() || (Object)(object)GorillaGameManager.instance == (Object)null)
		{
			return;
		}
		VRRig val = FindFarthest();
		if ((Object)(object)val == (Object)null)
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Transparent Theme").enabled;
		bool enabled3 = Buttons.GetIndex("Hidden on Camera").enabled;
		Color currentColor = Main.backgroundColor.GetCurrentColor();
		Color color = (enabled ? Main.textColors[0].GetCurrentColor() : Color.white);
		Color color2 = val.playerColor;
		if (enabled)
		{
			color2 = currentColor;
		}
		if (enabled2)
		{
			color2.a = 0.5f;
			color.a = 0.5f;
		}
		TextMeshPro nameTag = GetNameTag(enabled3);
		((Component)nameTag).gameObject.transform.position = ((Component)val).transform.position + new Vector3(0f, -0.2f, 0f);
		((Graphic)nameTag).color = color;
		_ = $"{Vector3.Distance(((Component)Camera.main).transform.position, ((Component)val).transform.position):F1}m";
		Transform[] componentsInChildren = ((Component)nameTag).gameObject.GetComponentsInChildren<Transform>();
		foreach (Transform val2 in componentsInChildren)
		{
			if (((Object)((Component)val2).gameObject).name == "bg")
			{
				((Component)val2).gameObject.GetComponent<Renderer>().material.color = color2;
				Bounds bounds = ((Component)nameTag).GetComponent<Renderer>().bounds;
				val2.localScale = new Vector3(((Bounds)(ref bounds)).size.x + 0.2f, 0.2f, 0.01f);
			}
		}
	}

	public static void InfectionTracers()
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		if (DoPerformanceCheck())
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Transparent Theme").enabled;
		float num = (Buttons.GetIndex("Thin Tracers").enabled ? 0.0075f : 0.025f) * (Main.scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
		bool flag = VRRig.LocalRig.IsTagged();
		bool flag2 = GameModeUtilities.InfectedList().Count == 0;
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (activeRig.isLocal)
			{
				continue;
			}
			Color val = activeRig.playerColor;
			if (!flag2)
			{
				if (flag)
				{
					if (activeRig.IsTagged())
					{
						continue;
					}
				}
				else
				{
					if (!activeRig.IsTagged())
					{
						continue;
					}
					val = activeRig.GetColor();
				}
			}
			LineRenderer lineRender = GetLineRender();
			if (enabled)
			{
				val.a = 0.5f;
			}
			lineRender.startColor = val;
			lineRender.endColor = val;
			lineRender.startWidth = num;
			lineRender.endWidth = num;
			lineRender.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
			lineRender.SetPosition(1, ((Component)activeRig).transform.position);
		}
	}

	public static void HuntTracers()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Invalid comparison between Unknown and I4
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		if (DoPerformanceCheck() || (Object)(object)GorillaGameManager.instance == (Object)null || (int)GorillaGameManager.instance.GameType() != 2)
		{
			return;
		}
		GorillaHuntManager val = (GorillaHuntManager)GorillaGameManager.instance;
		if ((Object)(object)val == (Object)null)
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Transparent Theme").enabled;
		float num = (Buttons.GetIndex("Thin Tracers").enabled ? 0.0075f : 0.025f) * (Main.scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
		NetPlayer targetOf = val.GetTargetOf(NetPlayer.op_Implicit(PhotonNetwork.LocalPlayer));
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig playerRig) => !playerRig.isLocal))
		{
			if (RigUtilities.GetPlayerFromVRRig(item) == targetOf)
			{
				Color playerColor = item.playerColor;
				LineRenderer lineRender = GetLineRender();
				if (enabled)
				{
					playerColor.a = 0.5f;
				}
				lineRender.startColor = playerColor;
				lineRender.endColor = playerColor;
				lineRender.startWidth = num;
				lineRender.endWidth = num;
				lineRender.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
				lineRender.SetPosition(1, ((Component)item).transform.position);
			}
			else if (val.IsTargetOf(RigUtilities.GetPlayerFromVRRig(item), NetPlayer.op_Implicit(PhotonNetwork.LocalPlayer)))
			{
				Color red = Color.red;
				LineRenderer lineRender2 = GetLineRender();
				if (enabled)
				{
					red.a = 0.5f;
				}
				lineRender2.startColor = red;
				lineRender2.endColor = red;
				lineRender2.startWidth = num;
				lineRender2.endWidth = num;
				lineRender2.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
				lineRender2.SetPosition(1, ((Component)item).transform.position);
			}
		}
	}

	public static void CasualBeacons()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		if (DoPerformanceCheck() || (Object)(object)GorillaGameManager.instance == (Object)null)
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Transparent Theme").enabled;
		_ = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Thin Tracers").enabled;
		Color currentColor = Main.backgroundColor.GetCurrentColor();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.isLocal)
			{
				Color val = activeRig.playerColor;
				LineRenderer lineRender = GetLineRender();
				if (enabled)
				{
					val = currentColor;
				}
				if (enabled2)
				{
					val.a = 0.5f;
				}
				lineRender.startColor = val;
				lineRender.endColor = val;
				float endWidth = (lineRender.startWidth = (enabled3 ? 0.0075f : 0.025f));
				lineRender.endWidth = endWidth;
				lineRender.SetPosition(0, ((Component)activeRig).transform.position + new Vector3(0f, 9999f, 0f));
				lineRender.SetPosition(1, ((Component)activeRig).transform.position - new Vector3(0f, 9999f, 0f));
			}
		}
	}

	public static void InfectionBeacons()
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		if (DoPerformanceCheck() || (Object)(object)GorillaGameManager.instance == (Object)null)
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Transparent Theme").enabled;
		_ = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Thin Tracers").enabled;
		bool flag = VRRig.LocalRig.IsTagged();
		bool flag2 = GameModeUtilities.InfectedList().Count == 0;
		Color currentColor = Main.backgroundColor.GetCurrentColor();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (activeRig.isLocal)
			{
				continue;
			}
			Color val = activeRig.playerColor;
			if (!flag2)
			{
				if (flag)
				{
					if (activeRig.IsTagged())
					{
						continue;
					}
				}
				else
				{
					if (!activeRig.IsTagged())
					{
						continue;
					}
					val = activeRig.GetColor();
				}
			}
			LineRenderer lineRender = GetLineRender();
			if (enabled)
			{
				val = currentColor;
			}
			if (enabled2)
			{
				val.a = 0.5f;
			}
			lineRender.startColor = val;
			lineRender.endColor = val;
			float endWidth = (lineRender.startWidth = (enabled3 ? 0.0075f : 0.025f));
			lineRender.endWidth = endWidth;
			lineRender.SetPosition(0, ((Component)activeRig).transform.position + new Vector3(0f, 9999f, 0f));
			lineRender.SetPosition(1, ((Component)activeRig).transform.position - new Vector3(0f, 9999f, 0f));
		}
	}

	public static void HuntBeacons()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Invalid comparison between Unknown and I4
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		if (DoPerformanceCheck() || (Object)(object)GorillaGameManager.instance == (Object)null || (int)GorillaGameManager.instance.GameType() != 2)
		{
			return;
		}
		GorillaHuntManager val = (GorillaHuntManager)GorillaGameManager.instance;
		if ((Object)(object)val == (Object)null)
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Transparent Theme").enabled;
		bool enabled3 = Buttons.GetIndex("Thin Tracers").enabled;
		Color currentColor = Main.backgroundColor.GetCurrentColor();
		NetPlayer targetOf = val.GetTargetOf(NetPlayer.op_Implicit(PhotonNetwork.LocalPlayer));
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig playerRig) => !playerRig.isLocal))
		{
			if (RigUtilities.GetPlayerFromVRRig(item) == targetOf)
			{
				Color val2 = item.playerColor;
				LineRenderer lineRender = GetLineRender();
				if (enabled)
				{
					val2 = currentColor;
				}
				if (enabled2)
				{
					val2.a = 0.5f;
				}
				lineRender.startColor = val2;
				lineRender.endColor = val2;
				float endWidth = (lineRender.startWidth = (enabled3 ? 0.0075f : 0.025f));
				lineRender.endWidth = endWidth;
				lineRender.SetPosition(0, ((Component)item).transform.position + new Vector3(0f, 9999f, 0f));
				lineRender.SetPosition(1, ((Component)item).transform.position - new Vector3(0f, 9999f, 0f));
			}
			else if (val.IsTargetOf(RigUtilities.GetPlayerFromVRRig(item), NetPlayer.op_Implicit(PhotonNetwork.LocalPlayer)))
			{
				Color val3 = Color.red;
				LineRenderer lineRender2 = GetLineRender();
				if (enabled)
				{
					val3 = currentColor;
				}
				if (enabled2)
				{
					val3.a = 0.5f;
				}
				lineRender2.startColor = val3;
				lineRender2.endColor = val3;
				float endWidth2 = (lineRender2.startWidth = (enabled3 ? 0.0075f : 0.025f));
				lineRender2.endWidth = endWidth2;
				lineRender2.SetPosition(0, ((Component)item).transform.position + new Vector3(0f, 9999f, 0f));
				lineRender2.SetPosition(1, ((Component)item).transform.position - new Vector3(0f, 9999f, 0f));
			}
		}
	}

	public static void CasualDistanceESP()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		if (DoPerformanceCheck() || (Object)(object)GorillaGameManager.instance == (Object)null)
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Transparent Theme").enabled;
		bool enabled3 = Buttons.GetIndex("Hidden on Camera").enabled;
		Color currentColor = Main.backgroundColor.GetCurrentColor();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (activeRig.isLocal)
			{
				continue;
			}
			Color color = (enabled ? Main.textColors[0].GetCurrentColor() : Color.white);
			Color color2 = activeRig.playerColor;
			if (enabled)
			{
				color2 = currentColor;
			}
			if (enabled2)
			{
				color2.a = 0.5f;
				color.a = 0.5f;
			}
			TextMeshPro nameTag = GetNameTag(enabled3);
			((Component)nameTag).gameObject.transform.position = ((Component)activeRig).transform.position + new Vector3(0f, -0.2f, 0f);
			((Graphic)nameTag).color = color;
			_ = $"{Vector3.Distance(((Component)Camera.main).transform.position, ((Component)activeRig).transform.position):F1}m";
			Transform[] componentsInChildren = ((Component)nameTag).gameObject.GetComponentsInChildren<Transform>();
			foreach (Transform val in componentsInChildren)
			{
				if (((Object)((Component)val).gameObject).name == "bg")
				{
					((Component)val).gameObject.GetComponent<Renderer>().material.color = color2;
					Bounds bounds = ((Component)nameTag).GetComponent<Renderer>().bounds;
					val.localScale = new Vector3(((Bounds)(ref bounds)).size.x + 0.2f, 0.2f, 0.01f);
				}
			}
		}
	}

	public static void InfectionDistanceESP()
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		if (DoPerformanceCheck() || (Object)(object)GorillaGameManager.instance == (Object)null)
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Transparent Theme").enabled;
		bool enabled3 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool flag = VRRig.LocalRig.IsTagged();
		bool flag2 = GameModeUtilities.InfectedList().Count == 0;
		Color currentColor = Main.backgroundColor.GetCurrentColor();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (activeRig.isLocal)
			{
				continue;
			}
			Color color = (enabled ? Main.textColors[0].GetCurrentColor() : Color.white);
			Color color2 = activeRig.playerColor;
			if (!flag2)
			{
				if (flag)
				{
					if (activeRig.IsTagged())
					{
						continue;
					}
				}
				else
				{
					if (!activeRig.IsTagged())
					{
						continue;
					}
					color2 = activeRig.GetColor();
				}
			}
			if (enabled)
			{
				color2 = currentColor;
			}
			if (enabled2)
			{
				color2.a = 0.5f;
				color.a = 0.5f;
			}
			TextMeshPro nameTag = GetNameTag(enabled3);
			((Component)nameTag).gameObject.transform.position = ((Component)activeRig).transform.position + new Vector3(0f, -0.2f, 0f);
			((Graphic)nameTag).color = color;
			_ = $"{Vector3.Distance(((Component)Camera.main).transform.position, ((Component)activeRig).transform.position):F1}m";
			Transform[] componentsInChildren = ((Component)nameTag).gameObject.GetComponentsInChildren<Transform>();
			foreach (Transform val in componentsInChildren)
			{
				if (((Object)((Component)val).gameObject).name == "bg")
				{
					((Component)val).gameObject.GetComponent<Renderer>().material.color = color2;
					Bounds bounds = ((Component)nameTag).GetComponent<Renderer>().bounds;
					val.localScale = new Vector3(((Bounds)(ref bounds)).size.x + 0.2f, 0.2f, 0.01f);
				}
			}
		}
	}

	public static void HuntDistanceESP()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Invalid comparison between Unknown and I4
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		if (DoPerformanceCheck() || (Object)(object)GorillaGameManager.instance == (Object)null || (int)GorillaGameManager.instance.GameType() != 2)
		{
			return;
		}
		GorillaHuntManager val = (GorillaHuntManager)GorillaGameManager.instance;
		if ((Object)(object)val == (Object)null)
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Transparent Theme").enabled;
		bool enabled3 = Buttons.GetIndex("Hidden on Camera").enabled;
		Color currentColor = Main.backgroundColor.GetCurrentColor();
		NetPlayer targetOf = val.GetTargetOf(NetPlayer.op_Implicit(PhotonNetwork.LocalPlayer));
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig playerRig) => !playerRig.isLocal))
		{
			Bounds bounds;
			if (RigUtilities.GetPlayerFromVRRig(item) == targetOf)
			{
				Color color = (enabled ? Main.textColors[0].GetCurrentColor() : Color.white);
				Color color2 = item.playerColor;
				if (enabled)
				{
					color2 = currentColor;
				}
				if (enabled2)
				{
					color2.a = 0.5f;
					color.a = 0.5f;
				}
				TextMeshPro nameTag = GetNameTag(enabled3);
				((Component)nameTag).gameObject.transform.position = ((Component)item).transform.position + new Vector3(0f, -0.2f, 0f);
				((Graphic)nameTag).color = color;
				_ = $"{Vector3.Distance(((Component)Camera.main).transform.position, ((Component)item).transform.position):F1}m";
				Transform[] componentsInChildren = ((Component)nameTag).gameObject.GetComponentsInChildren<Transform>();
				foreach (Transform val2 in componentsInChildren)
				{
					if (((Object)((Component)val2).gameObject).name == "bg")
					{
						((Component)val2).gameObject.GetComponent<Renderer>().material.color = color2;
						bounds = ((Component)nameTag).GetComponent<Renderer>().bounds;
						val2.localScale = new Vector3(((Bounds)(ref bounds)).size.x + 0.2f, 0.2f, 0.01f);
					}
				}
			}
			else
			{
				if (!val.IsTargetOf(RigUtilities.GetPlayerFromVRRig(item), NetPlayer.op_Implicit(PhotonNetwork.LocalPlayer)))
				{
					continue;
				}
				Color color3 = (enabled ? Main.textColors[0].GetCurrentColor() : Color.white);
				Color color4 = Color.red;
				if (enabled)
				{
					color4 = currentColor;
				}
				if (enabled2)
				{
					color4.a = 0.5f;
					color3.a = 0.5f;
				}
				TextMeshPro nameTag2 = GetNameTag(enabled3);
				((Component)nameTag2).gameObject.transform.position = ((Component)item).transform.position + new Vector3(0f, -0.2f, 0f);
				((Graphic)nameTag2).color = color3;
				_ = $"{Vector3.Distance(((Component)Camera.main).transform.position, ((Component)item).transform.position):F1}m";
				Transform[] componentsInChildren2 = ((Component)nameTag2).gameObject.GetComponentsInChildren<Transform>();
				foreach (Transform val3 in componentsInChildren2)
				{
					if (((Object)((Component)val3).gameObject).name == "bg")
					{
						((Component)val3).gameObject.GetComponent<Renderer>().material.color = color4;
						bounds = ((Component)nameTag2).GetComponent<Renderer>().bounds;
						val3.localScale = new Vector3(((Bounds)(ref bounds)).size.x + 0.2f, 0.2f, 0.01f);
					}
				}
			}
		}
	}

	private static TextMeshPro GetNameTag(bool hideOnCamera)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)nameTagHolder == (Object)null)
		{
			nameTagHolder = new GameObject("NameTag_Holder");
		}
		TextMeshPro finalTextMeshPro = null;
		foreach (TextMeshPro item in nameTagPool.Where((TextMeshPro TextMeshPro) => (Object)(object)finalTextMeshPro == (Object)null && !((Component)TextMeshPro).gameObject.activeInHierarchy))
		{
			((Component)item).gameObject.SetActive(true);
			((Component)item).gameObject.transform.LookAt(((Component)Camera.main).transform.position);
			((Component)item).gameObject.transform.Rotate(0f, 180f, 0f);
			((TMP_Text)(object)item).SafeSetFontStyle(Main.activeFontStyle);
			((TMP_Text)(object)item).SafeSetFont(Main.activeFont);
			finalTextMeshPro = item;
		}
		if ((Object)(object)finalTextMeshPro == (Object)null)
		{
			GameObject val = new GameObject("TextMeshProObject");
			val.transform.parent = nameTagHolder.transform;
			TextMeshPro val2 = val.AddComponent<TextMeshPro>();
			Renderer component = ((Component)val2).GetComponent<Renderer>();
			((TMP_Text)val2).fontSize = 1.8f;
			((TMP_Text)(object)val2).SafeSetFontStyle(Main.activeFontStyle);
			((TMP_Text)val2).alignment = (TextAlignmentOptions)514;
			((Graphic)val2).color = Color.white;
			GameObject val3 = GameObject.CreatePrimitive((PrimitiveType)3);
			Object.Destroy((Object)(object)val3.GetComponent<Collider>());
			Renderer component2 = val3.GetComponent<Renderer>();
			((Object)val3).name = "bg";
			val3.transform.parent = val.transform;
			val3.transform.localPosition = Vector3.zero;
			Transform transform = val3.transform;
			Bounds bounds = component.bounds;
			transform.localScale = new Vector3(((Bounds)(ref bounds)).size.x + 0.2f, 0.2f, 0.01f);
			component2.material.shader = Shader.Find("GUI/Text Shader");
			component2.material.color = Color.white;
			component.material.renderQueue = component2.material.renderQueue + 2;
			((TMP_Text)val2).outlineWidth = 0.2f;
			((TMP_Text)val2).outlineColor = Color32.op_Implicit(Color.black);
			nameTagPool.Add(val2);
			finalTextMeshPro = val2;
		}
		((Component)finalTextMeshPro).gameObject.layer = (hideOnCamera ? 19 : nameTagHolder.layer);
		return finalTextMeshPro;
	}

	public static void ClearNameTagPool(bool destroy = false)
	{
		if (DoPerformanceCheck())
		{
			return;
		}
		foreach (TextMeshPro item in nameTagPool)
		{
			if (destroy || isNameTagQueued)
			{
				Object.Destroy((Object)(object)((Component)item).gameObject);
			}
			else
			{
				((Component)item).gameObject.SetActive(false);
			}
		}
		if (destroy || isNameTagQueued)
		{
			nameTagPool.Clear();
		}
		isNameTagQueued = false;
	}

	public static LineRenderer GetLineRender()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		bool enabled = Buttons.GetIndex("Hidden on Camera").enabled;
		if ((Object)(object)lineRenderHolder == (Object)null)
		{
			lineRenderHolder = new GameObject("LineRender_Holder");
		}
		LineRenderer finalRender = null;
		foreach (LineRenderer item in from line in linePool
			where (Object)(object)finalRender == (Object)null
			where !((Component)line).gameObject.activeInHierarchy
			select line)
		{
			((Component)item).gameObject.SetActive(true);
			finalRender = item;
		}
		if ((Object)(object)finalRender == (Object)null)
		{
			GameObject val = new GameObject("LineObject");
			val.transform.parent = lineRenderHolder.transform;
			LineRenderer val2 = val.AddComponent<LineRenderer>();
			if (Main.smoothLines)
			{
				val2.numCapVertices = 10;
				val2.numCornerVertices = 5;
			}
			((Renderer)val2).material.shader = Shader.Find("GUI/Text Shader");
			val2.startWidth = 0.025f;
			val2.endWidth = 0.025f;
			val2.positionCount = 2;
			val2.useWorldSpace = true;
			linePool.Add(val2);
			finalRender = val2;
		}
		((Component)finalRender).gameObject.layer = (enabled ? 19 : lineRenderHolder.layer);
		return finalRender;
	}

	public static void ClearLinePool(bool destroy = false)
	{
		if (DoPerformanceCheck())
		{
			return;
		}
		foreach (LineRenderer item in linePool)
		{
			if (destroy || isLineRenderQueued)
			{
				Object.Destroy((Object)(object)((Component)item).gameObject);
			}
			else
			{
				((Component)item).gameObject.SetActive(false);
			}
		}
		if (destroy || isLineRenderQueued)
		{
			linePool.Clear();
		}
	}

	public static void ConsoleBeacon(string id, string version, string menuName)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		NetPlayer playerFromID = RigUtilities.GetPlayerFromID(id);
		VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(playerFromID);
		Color red = Color.red;
		NotificationManager.SendNotification("<color=grey>[</color><color=purple>ADMIN</color><color=grey>]</color> " + playerFromID.NickName + " is using " + menuName + " version " + version + ".", 3000);
		VRRig.LocalRig.PlayHandTapLocal(29, false, 99999f);
		VRRig.LocalRig.PlayHandTapLocal(29, true, 99999f);
		GameObject val = new GameObject("Line");
		LineRenderer val2 = val.AddComponent<LineRenderer>();
		val2.startColor = red;
		val2.endColor = red;
		val2.startWidth = 0.25f;
		val2.endWidth = 0.25f;
		val2.positionCount = 2;
		val2.useWorldSpace = true;
		val2.SetPosition(0, ((Component)vRRigFromPlayer).transform.position + new Vector3(0f, 9999f, 0f));
		val2.SetPosition(1, ((Component)vRRigFromPlayer).transform.position - new Vector3(0f, 9999f, 0f));
		((Renderer)val2).material.shader = Shader.Find("GUI/Text Shader");
		Object.Destroy((Object)(object)val, 3f);
	}

	public static void playerinfogun()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Expected O, but got Unknown
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.isOfflineVRRig)
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
			if (!Main.gunLocked || !((Object)(object)Main.lockTarget != (Object)null))
			{
				return;
			}
			NetPlayer creator = Main.lockTarget.Creator;
			if (creator == null)
			{
				return;
			}
			string text = ((Main.lockTarget.GetPlatform() == "Standalone") ? "QUEST" : "STEAM");
			string text2 = (creator.IsMasterClient ? "<color=yellow>TRUE</color>" : "FALSE");
			float num;
			if (!((Object)(object)((Component)Main.lockTarget).GetComponent<Rigidbody>() != (Object)null))
			{
				num = 0f;
			}
			else
			{
				Vector3 velocity = ((Component)Main.lockTarget).GetComponent<Rigidbody>().velocity;
				num = ((Vector3)(ref velocity)).magnitude;
			}
			float num2 = num;
			float num3 = Vector3.Distance(((Component)GorillaTagger.Instance.offlineVRRig).transform.position, ((Component)Main.lockTarget).transform.position);
			string text3 = ((Main.lockTarget.setMatIndex == 1) ? "LAVA" : ((Main.lockTarget.setMatIndex == 2) ? "ROCK" : ((Main.lockTarget.setMatIndex == 3) ? "WATER" : "NO")));
			float num4 = 1f / Time.unscaledDeltaTime;
			int num5 = 0;
			MeshRenderer[] componentsInChildren = ((Component)Main.lockTarget).GetComponentsInChildren<MeshRenderer>(true);
			MeshRenderer[] array = componentsInChildren;
			foreach (MeshRenderer val2 in array)
			{
				if ((Object)(object)val2 != (Object)null && ((Component)val2).gameObject.activeInHierarchy && (Object)(object)((Component)val2).transform.parent != (Object)null && ((Object)((Component)val2).transform.parent).name.ToLower().Contains("cosmetic"))
				{
					num5++;
				}
			}
			string text4 = "<color=red>TARGET LOCKED</color>\nNAME: " + creator.NickName + "\nID: " + creator.UserId + "\nPLATFORM: " + text + "\nCOLOR: " + Mathf.RoundToInt(Main.lockTarget.playerColor.r * 9f) + "," + Mathf.RoundToInt(Main.lockTarget.playerColor.g * 9f) + "," + Mathf.RoundToInt(Main.lockTarget.playerColor.b * 9f) + "\nTAGGED: " + text3 + "\nMASTER: " + text2 + "\nActor: " + creator.ActorNumber + "\nITEMS: " + num5 + "\nFPS: " + Mathf.Ceil(num4) + "\nSPEED: " + num2.ToString("F1") + "\nDIST: " + num3.ToString("F2");
			TextMesh val3 = Main.lockTarget.headMesh.GetComponentInChildren<TextMesh>();
			if ((Object)(object)val3 == (Object)null)
			{
				GameObject val4 = new GameObject("PlayerInfoText");
				val4.transform.SetParent(Main.lockTarget.headMesh.transform);
				val4.transform.localPosition = Vector3.up * 0.6f;
				val3 = val4.AddComponent<TextMesh>();
				val3.fontSize = 50;
				val3.characterSize = 0.0032f;
				val3.alignment = (TextAlignment)0;
				val3.anchor = (TextAnchor)4;
			}
			val3.text = text4;
			((Component)val3).transform.rotation = Quaternion.LookRotation(((Component)val3).transform.position - GorillaTagger.Instance.mainCamera.transform.position);
			return;
		}
		Main.gunLocked = false;
		if ((Object)(object)Main.lockTarget != (Object)null)
		{
			TextMesh componentInChildren = Main.lockTarget.headMesh.GetComponentInChildren<TextMesh>();
			if ((Object)(object)componentInChildren != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)componentInChildren).gameObject);
			}
			Main.lockTarget = null;
		}
	}

	public static void playerinfohandgun()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Expected O, but got Unknown
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.isOfflineVRRig)
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
			if (!Main.gunLocked || !((Object)(object)Main.lockTarget != (Object)null))
			{
				return;
			}
			NetPlayer creator = Main.lockTarget.Creator;
			if (creator == null)
			{
				return;
			}
			string text = ((Main.lockTarget.GetPlatform() == "Standalone") ? "QUEST" : "STEAM");
			string text2 = (creator.IsMasterClient ? "<color=yellow>TRUE</color>" : "FALSE");
			float num;
			if (!((Object)(object)((Component)Main.lockTarget).GetComponent<Rigidbody>() != (Object)null))
			{
				num = 0f;
			}
			else
			{
				Vector3 velocity = ((Component)Main.lockTarget).GetComponent<Rigidbody>().velocity;
				num = ((Vector3)(ref velocity)).magnitude;
			}
			float num2 = num;
			float num3 = Vector3.Distance(((Component)GorillaTagger.Instance.offlineVRRig).transform.position, ((Component)Main.lockTarget).transform.position);
			string text3 = ((Main.lockTarget.setMatIndex == 1) ? "LAVA" : ((Main.lockTarget.setMatIndex == 2) ? "ROCK" : ((Main.lockTarget.setMatIndex == 3) ? "WATER" : "NO")));
			float num4 = 1f / Time.unscaledDeltaTime;
			int num5 = 0;
			MeshRenderer[] componentsInChildren = ((Component)Main.lockTarget).GetComponentsInChildren<MeshRenderer>(true);
			MeshRenderer[] array = componentsInChildren;
			foreach (MeshRenderer val2 in array)
			{
				if ((Object)(object)val2 != (Object)null && ((Component)val2).gameObject.activeInHierarchy && (Object)(object)((Component)val2).transform.parent != (Object)null && ((Object)((Component)val2).transform.parent).name.ToLower().Contains("cosmetic"))
				{
					num5++;
				}
			}
			string text4 = "<color=red>TARGET LOCKED</color>\nNAME: " + creator.NickName + "\nID: " + creator.UserId + "\nPLATFORM: " + text + "\nCOLOR: " + Mathf.RoundToInt(Main.lockTarget.playerColor.r * 9f) + "," + Mathf.RoundToInt(Main.lockTarget.playerColor.g * 9f) + "," + Mathf.RoundToInt(Main.lockTarget.playerColor.b * 9f) + "\nTAGGED: " + text3 + "\nMASTER: " + text2 + "\nActor: " + creator.ActorNumber + "\nITEMS: " + num5 + "\nFPS: " + Mathf.Ceil(num4) + "\nSPEED: " + num2.ToString("F1") + "\nDIST: " + num3.ToString("F2");
			TextMesh val3 = Main.lockTarget.headMesh.GetComponentInChildren<TextMesh>();
			if ((Object)(object)val3 == (Object)null)
			{
				GameObject val4 = new GameObject("PlayerInfoText_BIG");
				val4.transform.SetParent(Main.lockTarget.headMesh.transform);
				val4.transform.localPosition = Vector3.up * 1.2f;
				val3 = val4.AddComponent<TextMesh>();
				val3.fontSize = 80;
				val3.characterSize = 0.012f;
				val3.alignment = (TextAlignment)0;
				val3.anchor = (TextAnchor)4;
				val3.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
				((Renderer)((Component)val3).GetComponent<MeshRenderer>()).material = val3.font.material;
			}
			val3.text = text4;
			((Component)val3).transform.rotation = Quaternion.LookRotation(((Component)val3).transform.position - GorillaTagger.Instance.mainCamera.transform.position);
			return;
		}
		Main.gunLocked = false;
		if ((Object)(object)Main.lockTarget != (Object)null)
		{
			Transform val5 = Main.lockTarget.headMesh.transform.Find("PlayerInfoText_BIG");
			if ((Object)(object)val5 != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)val5).gameObject);
			}
			Main.lockTarget = null;
		}
	}

	public static void playerinfohandgunV2()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Expected O, but got Unknown
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.isOfflineVRRig)
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
			if (!Main.gunLocked || !((Object)(object)Main.lockTarget != (Object)null))
			{
				return;
			}
			NetPlayer creator = Main.lockTarget.Creator;
			if (creator == null)
			{
				return;
			}
			string text = ((Main.lockTarget.GetPlatform() == "Standalone") ? "QUEST" : "STEAM");
			string text2 = (creator.IsMasterClient ? "<color=yellow>TRUE</color>" : "FALSE");
			float num;
			if (!((Object)(object)((Component)Main.lockTarget).GetComponent<Rigidbody>() != (Object)null))
			{
				num = 0f;
			}
			else
			{
				Vector3 velocity = ((Component)Main.lockTarget).GetComponent<Rigidbody>().velocity;
				num = ((Vector3)(ref velocity)).magnitude;
			}
			float num2 = num;
			float num3 = Vector3.Distance(((Component)GorillaTagger.Instance.offlineVRRig).transform.position, ((Component)Main.lockTarget).transform.position);
			string text3 = ((Main.lockTarget.setMatIndex == 1) ? "LAVA" : ((Main.lockTarget.setMatIndex == 2) ? "ROCK" : ((Main.lockTarget.setMatIndex == 3) ? "WATER" : "NO")));
			float num4 = 1f / Time.unscaledDeltaTime;
			int num5 = 0;
			MeshRenderer[] componentsInChildren = ((Component)Main.lockTarget).GetComponentsInChildren<MeshRenderer>(true);
			MeshRenderer[] array = componentsInChildren;
			foreach (MeshRenderer val2 in array)
			{
				if ((Object)(object)val2 != (Object)null && ((Component)val2).gameObject.activeInHierarchy && (Object)(object)((Component)val2).transform.parent != (Object)null && ((Object)((Component)val2).transform.parent).name.ToLower().Contains("cosmetic"))
				{
					num5++;
				}
			}
			string text4 = "<color=red>TARGET LOCKED</color>\nNAME: " + creator.NickName + "\nID: " + creator.UserId + "\nPLATFORM: " + text + "\nCOLOR: " + Mathf.RoundToInt(Main.lockTarget.playerColor.r * 9f) + "," + Mathf.RoundToInt(Main.lockTarget.playerColor.g * 9f) + "," + Mathf.RoundToInt(Main.lockTarget.playerColor.b * 9f) + "\nTAGGED: " + text3 + "\nMASTER: " + text2 + "\nActor: " + creator.ActorNumber + "\nITEMS: " + num5 + "\nFPS: " + Mathf.Ceil(num4) + "\nSPEED: " + num2.ToString("F1") + "\nDIST: " + num3.ToString("F2");
			Transform rightHandTransform = GorillaTagger.Instance.rightHandTransform;
			GameObject val3 = GameObject.Find("MyRightHandInfoDisplay");
			if ((Object)(object)val3 == (Object)null)
			{
				val3 = new GameObject("MyRightHandInfoDisplay");
				val3.transform.SetParent(rightHandTransform);
				val3.transform.localPosition = new Vector3(0f, 0.15f, 0f);
				TextMesh val4 = val3.AddComponent<TextMesh>();
				val4.fontSize = 60;
				val4.characterSize = 0.0035f;
				val4.alignment = (TextAlignment)0;
				val4.anchor = (TextAnchor)6;
				val4.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
				((Renderer)((Component)val4).GetComponent<MeshRenderer>()).material = val4.font.material;
			}
			TextMesh component = val3.GetComponent<TextMesh>();
			component.text = text4;
			val3.transform.LookAt(GorillaTagger.Instance.mainCamera.transform);
			val3.transform.Rotate(0f, 180f, 0f);
		}
		else
		{
			Main.gunLocked = false;
			Main.lockTarget = null;
			GameObject val5 = GameObject.Find("MyRightHandInfoDisplay");
			if ((Object)(object)val5 != (Object)null)
			{
				Object.Destroy((Object)(object)val5);
			}
		}
	}

	public static void playerinfohandgunV3()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Expected O, but got Unknown
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.isOfflineVRRig)
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
			if (!Main.gunLocked || !((Object)(object)Main.lockTarget != (Object)null))
			{
				return;
			}
			NetPlayer creator = Main.lockTarget.Creator;
			if (creator == null)
			{
				return;
			}
			string text = ((Main.lockTarget.GetPlatform() == "Standalone") ? "QUEST" : "STEAM");
			string text2 = (creator.IsMasterClient ? "<color=yellow>TRUE</color>" : "FALSE");
			float num;
			if (!((Object)(object)((Component)Main.lockTarget).GetComponent<Rigidbody>() != (Object)null))
			{
				num = 0f;
			}
			else
			{
				Vector3 velocity = ((Component)Main.lockTarget).GetComponent<Rigidbody>().velocity;
				num = ((Vector3)(ref velocity)).magnitude;
			}
			float num2 = num;
			float num3 = Vector3.Distance(((Component)GorillaTagger.Instance.offlineVRRig).transform.position, ((Component)Main.lockTarget).transform.position);
			string text3 = ((Main.lockTarget.setMatIndex == 1) ? "LAVA" : ((Main.lockTarget.setMatIndex == 2) ? "ROCK" : ((Main.lockTarget.setMatIndex == 3) ? "WATER" : "NO")));
			float num4 = 1f / Time.unscaledDeltaTime;
			Random random = new Random(creator.UserId.GetHashCode());
			int num5 = random.Next(2021, 2024);
			string text4 = random.Next(1, 13) + "/??/" + num5;
			int num6 = 0;
			MeshRenderer[] componentsInChildren = ((Component)Main.lockTarget).GetComponentsInChildren<MeshRenderer>(true);
			MeshRenderer[] array = componentsInChildren;
			foreach (MeshRenderer val2 in array)
			{
				if ((Object)(object)val2 != (Object)null && ((Component)val2).gameObject.activeInHierarchy && (Object)(object)((Component)val2).transform.parent != (Object)null && ((Object)((Component)val2).transform.parent).name.ToLower().Contains("cosmetic"))
				{
					num6++;
				}
			}
			string text5 = "<color=red>TARGET LOCKED</color>\nNAME: " + creator.NickName + "\nID: " + creator.UserId + "\nCREATED: " + text4 + "\nPLATFORM: " + text + "\nCOLOR: " + Mathf.RoundToInt(Main.lockTarget.playerColor.r * 9f) + "," + Mathf.RoundToInt(Main.lockTarget.playerColor.g * 9f) + "," + Mathf.RoundToInt(Main.lockTarget.playerColor.b * 9f) + "\nTAGGED: " + text3 + "\nMASTER: " + text2 + "\nActor: " + creator.ActorNumber + "\nITEMS: " + num6 + "\nFPS: " + Mathf.Ceil(num4) + "\nSPEED: " + num2.ToString("F1") + "\nDIST: " + num3.ToString("F2");
			Transform rightHandTransform = GorillaTagger.Instance.rightHandTransform;
			GameObject val3 = GameObject.Find("MyRightHandInfoDisplay");
			if ((Object)(object)val3 == (Object)null)
			{
				val3 = new GameObject("MyRightHandInfoDisplay");
				val3.transform.SetParent(rightHandTransform);
				val3.transform.localPosition = new Vector3(0f, 0.15f, 0f);
				TextMesh val4 = val3.AddComponent<TextMesh>();
				val4.fontSize = 60;
				val4.characterSize = 0.0035f;
				val4.alignment = (TextAlignment)0;
				val4.anchor = (TextAnchor)6;
				val4.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
				((Renderer)((Component)val4).GetComponent<MeshRenderer>()).material = val4.font.material;
			}
			TextMesh component = val3.GetComponent<TextMesh>();
			component.text = text5;
			val3.transform.LookAt(GorillaTagger.Instance.mainCamera.transform);
			val3.transform.Rotate(0f, 180f, 0f);
		}
		else
		{
			Main.gunLocked = false;
			Main.lockTarget = null;
			GameObject val5 = GameObject.Find("MyRightHandInfoDisplay");
			if ((Object)(object)val5 != (Object)null)
			{
				Object.Destroy((Object)(object)val5);
			}
		}
	}

	public static void playerinfogunnarrarate()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.isOfflineVRRig)
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
			if (!Main.gunLocked || !((Object)(object)Main.lockTarget != (Object)null))
			{
				return;
			}
			NetPlayer creator = Main.lockTarget.Creator;
			if (creator == null)
			{
				return;
			}
			string text = ((Main.lockTarget.GetPlatform() == "Standalone") ? "Quest" : "Steam");
			float num;
			if (!((Object)(object)((Component)Main.lockTarget).GetComponent<Rigidbody>() != (Object)null))
			{
				num = 0f;
			}
			else
			{
				Vector3 velocity = ((Component)Main.lockTarget).GetComponent<Rigidbody>().velocity;
				num = ((Vector3)(ref velocity)).magnitude;
			}
			float num2 = num;
			float num3 = Vector3.Distance(((Component)GorillaTagger.Instance.offlineVRRig).transform.position, ((Component)Main.lockTarget).transform.position);
			string text2 = ((Main.lockTarget.setMatIndex == 1) ? "Lava" : ((Main.lockTarget.setMatIndex == 2) ? "Rock" : ((Main.lockTarget.setMatIndex == 3) ? "Water" : "None")));
			Random random = new Random(creator.UserId.GetHashCode());
			int num4 = random.Next(2021, 2024);
			string text3 = random.Next(1, 13) + "/??/" + num4;
			int num5 = 0;
			MeshRenderer[] componentsInChildren = ((Component)Main.lockTarget).GetComponentsInChildren<MeshRenderer>(true);
			MeshRenderer[] array = componentsInChildren;
			foreach (MeshRenderer val2 in array)
			{
				if ((Object)(object)val2 != (Object)null && ((Component)val2).gameObject.activeInHierarchy && (Object)(object)((Component)val2).transform.parent != (Object)null && ((Object)((Component)val2).transform.parent).name.ToLower().Contains("cosmetic"))
				{
					num5++;
				}
			}
			string text4 = "Target: " + creator.NickName + ". Platform: " + text + ". Distance: " + num3.ToString("F1") + " meters. Speed: " + num2.ToString("F1") + ". Cosmetics: " + num5 + ". Material: " + text2 + ".";
			Main.SpeakText(text4);
		}
		else
		{
			Main.gunLocked = false;
			Main.lockTarget = null;
		}
	}

	public static void playerinfohandgunV4()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Expected O, but got Unknown
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Expected O, but got Unknown
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Expected O, but got Unknown
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0600: Expected O, but got Unknown
		//IL_062a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cc: Expected O, but got Unknown
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0760: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.isOfflineVRRig)
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
			if (!Main.gunLocked || !((Object)(object)Main.lockTarget != (Object)null))
			{
				return;
			}
			NetPlayer creator = Main.lockTarget.Creator;
			if (creator == null)
			{
				return;
			}
			string text = ((Main.lockTarget.GetPlatform() == "Standalone") ? "QUEST" : "STEAM");
			string text2 = (creator.IsMasterClient ? "<color=yellow>TRUE</color>" : "FALSE");
			float num;
			if (!((Object)(object)((Component)Main.lockTarget).GetComponent<Rigidbody>() != (Object)null))
			{
				num = 0f;
			}
			else
			{
				Vector3 velocity = ((Component)Main.lockTarget).GetComponent<Rigidbody>().velocity;
				num = ((Vector3)(ref velocity)).magnitude;
			}
			float num2 = num;
			float num3 = Vector3.Distance(((Component)GorillaTagger.Instance.offlineVRRig).transform.position, ((Component)Main.lockTarget).transform.position);
			string text3 = ((Main.lockTarget.setMatIndex == 1) ? "LAVA" : ((Main.lockTarget.setMatIndex == 2) ? "ROCK" : ((Main.lockTarget.setMatIndex == 3) ? "WATER" : "NO")));
			float num4 = 1f / Time.unscaledDeltaTime;
			int num5 = 0;
			MeshRenderer[] componentsInChildren = ((Component)Main.lockTarget).GetComponentsInChildren<MeshRenderer>(true);
			MeshRenderer[] array = componentsInChildren;
			foreach (MeshRenderer val2 in array)
			{
				if ((Object)(object)val2 != (Object)null && ((Component)val2).gameObject.activeInHierarchy && (Object)(object)((Component)val2).transform.parent != (Object)null && ((Object)((Component)val2).transform.parent).name.ToLower().Contains("cosmetic"))
				{
					num5++;
				}
			}
			string text4 = "<color=#00ff88>◆ TARGET ACQUIRED ◆</color>\n" + "<color=#555555>━━━━━━━━━━━━━━━━━━━━━</color>\n" + "  <color=#88ddff>NAME</color>     <color=#ffffff>" + creator.NickName + "</color>\n  <color=#88ddff>ID</color>       <color=#ffffff>" + creator.UserId + "</color>\n  <color=#88ddff>PLATFORM</color> <color=#ffffff>" + text + "</color>\n  <color=#88ddff>COLOR</color>    <color=#ffffff>" + Mathf.RoundToInt(Main.lockTarget.playerColor.r * 9f) + "," + Mathf.RoundToInt(Main.lockTarget.playerColor.g * 9f) + "," + Mathf.RoundToInt(Main.lockTarget.playerColor.b * 9f) + "</color>\n  <color=#88ddff>TAGGED</color>   <color=#ffffff>" + text3 + "</color>\n  <color=#88ddff>MASTER</color>   " + text2 + "\n  <color=#88ddff>ACTOR</color>    <color=#ffffff>#" + creator.ActorNumber + "</color>\n  <color=#88ddff>ITEMS</color>    <color=#ffffff>" + num5 + "</color>\n" + "<color=#555555>━━━━━━━━━━━━━━━━━━━━━</color>\n" + "  <color=#88ddff>FPS</color>      <color=#ffffff>" + Mathf.Ceil(num4) + " </color> <color=#88ddff>SPD</color> <color=#ffffff>" + num2.ToString("F1") + "</color>\n  <color=#88ddff>DIST</color>     <color=#ffffff>" + num3.ToString("F2") + "m</color>";
			Transform rightHandTransform = GorillaTagger.Instance.rightHandTransform;
			string text5 = "PlayerInfoHUD_Realistic";
			GameObject val3 = GameObject.Find(text5);
			if ((Object)(object)val3 == (Object)null)
			{
				val3 = new GameObject(text5);
				val3.transform.SetParent(rightHandTransform);
				val3.transform.localPosition = new Vector3(-0.03f, -0.08f, 0.06f);
				val3.transform.localRotation = Quaternion.Euler(20f, 0f, -10f);
				GameObject val4 = GameObject.CreatePrimitive((PrimitiveType)5);
				((Object)val4).name = "HUD_Backplate";
				val4.transform.SetParent(val3.transform);
				val4.transform.localPosition = Vector3.zero;
				val4.transform.localScale = new Vector3(0.065f, 0.09f, 1f);
				val4.transform.localRotation = Quaternion.identity;
				Renderer component = val4.GetComponent<Renderer>();
				component.material = new Material(Shader.Find("Unlit/Color"));
				component.material.color = new Color(0.03f, 0.03f, 0.06f, 0.85f);
				Object.Destroy((Object)(object)val4.GetComponent<Collider>());
				GameObject val5 = GameObject.CreatePrimitive((PrimitiveType)5);
				((Object)val5).name = "HUD_Frame";
				val5.transform.SetParent(val3.transform);
				val5.transform.localPosition = new Vector3(0f, 0f, -0.001f);
				val5.transform.localScale = new Vector3(0.068f, 0.093f, 1f);
				val5.transform.localRotation = Quaternion.identity;
				Renderer component2 = val5.GetComponent<Renderer>();
				component2.material = new Material(Shader.Find("Unlit/Color"));
				component2.material.color = new Color(0f, 0.8f, 0.45f, 0.5f);
				Object.Destroy((Object)(object)val5.GetComponent<Collider>());
				GameObject val6 = new GameObject("HUD_Scanline");
				val6.transform.SetParent(val3.transform);
				val6.transform.localPosition = new Vector3(-0.03f, 0.04f, 0.002f);
				TextMesh val7 = val6.AddComponent<TextMesh>();
				val7.fontSize = 24;
				val7.characterSize = 0.001f;
				val7.alignment = (TextAlignment)0;
				val7.anchor = (TextAnchor)0;
				val7.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
				((Renderer)((Component)val7).GetComponent<MeshRenderer>()).material = val7.font.material;
				val7.color = new Color(0f, 0.9f, 0.5f, 0.3f);
				val7.text = "▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄";
				GameObject val8 = new GameObject("HUD_Text");
				val8.transform.SetParent(val3.transform);
				val8.transform.localPosition = new Vector3(-0.028f, 0.035f, 0.002f);
				TextMesh val9 = val8.AddComponent<TextMesh>();
				val9.fontSize = 36;
				val9.characterSize = 0.001f;
				val9.alignment = (TextAlignment)0;
				val9.anchor = (TextAnchor)0;
				val9.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
				((Renderer)((Component)val9).GetComponent<MeshRenderer>()).material = val9.font.material;
				val9.color = Color.white;
			}
			TextMesh component3 = ((Component)val3.transform.Find("HUD_Text")).GetComponent<TextMesh>();
			component3.text = text4;
			val3.transform.LookAt(GorillaTagger.Instance.mainCamera.transform);
			val3.transform.Rotate(0f, 180f, 0f);
		}
		else
		{
			Main.gunLocked = false;
			Main.lockTarget = null;
			GameObject val10 = GameObject.Find("PlayerInfoHUD_Realistic");
			if ((Object)(object)val10 != (Object)null)
			{
				Object.Destroy((Object)(object)val10);
			}
		}
	}

	public static void playerinfoline()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Expected O, but got Unknown
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		Transform controllerTransform = GTPlayer.Instance.RightHand.controllerTransform;
		bool rightGrab = ((ControllerInputPoller)ControllerInputPoller.instance).rightGrab;
		bool flag = Main.rightTrigger > 0.5f;
		bool rightControllerPrimaryButton = ((ControllerInputPoller)ControllerInputPoller.instance).rightControllerPrimaryButton;
		if (rightGrab)
		{
			GameObject val = GameObject.Find("InfoLaser");
			if ((Object)(object)val == (Object)null)
			{
				val = new GameObject("InfoLaser");
				LineRenderer val2 = val.AddComponent<LineRenderer>();
				((Renderer)val2).material = new Material(Shader.Find("Sprites/Default"));
				val2.startWidth = 0.01f;
				val2.endWidth = 0.01f;
				val2.startColor = Color.red;
				val2.endColor = Color.red;
			}
			LineRenderer component = val.GetComponent<LineRenderer>();
			Vector3 val3 = controllerTransform.position + controllerTransform.forward * 100f;
			RaycastHit val4 = default(RaycastHit);
			if (Physics.Raycast(controllerTransform.position, controllerTransform.forward, ref val4))
			{
				val3 = ((RaycastHit)(ref val4)).point;
				if (flag)
				{
					VRRig componentInParent = ((Component)((RaycastHit)(ref val4)).collider).GetComponentInParent<VRRig>();
					if ((Object)(object)componentInParent != (Object)null && !componentInParent.isOfflineVRRig)
					{
						Main.gunLocked = true;
						Main.lockTarget = componentInParent;
					}
				}
			}
			component.SetPosition(0, controllerTransform.position);
			component.SetPosition(1, val3);
		}
		else
		{
			GameObject val5 = GameObject.Find("InfoLaser");
			if ((Object)(object)val5 != (Object)null)
			{
				Object.Destroy((Object)(object)val5);
			}
		}
		if (!Main.gunLocked || !((Object)(object)Main.lockTarget != (Object)null))
		{
			return;
		}
		NetPlayer creator = Main.lockTarget.Creator;
		if (creator != null)
		{
			string text = ((Main.lockTarget.GetPlatform() == "Standalone") ? "QUEST" : "STEAM");
			string text2 = (creator.IsMasterClient ? "<color=yellow>TRUE</color>" : "FALSE");
			float num;
			if (!((Object)(object)((Component)Main.lockTarget).GetComponent<Rigidbody>() != (Object)null))
			{
				num = 0f;
			}
			else
			{
				Vector3 velocity = ((Component)Main.lockTarget).GetComponent<Rigidbody>().velocity;
				num = ((Vector3)(ref velocity)).magnitude;
			}
			float num2 = num;
			float num3 = Vector3.Distance(((Component)GorillaTagger.Instance.offlineVRRig).transform.position, ((Component)Main.lockTarget).transform.position);
			string text3 = ((Main.lockTarget.setMatIndex == 1) ? "LAVA" : ((Main.lockTarget.setMatIndex == 2) ? "ROCK" : ((Main.lockTarget.setMatIndex == 3) ? "WATER" : "NO")));
			float num4 = 1f / Time.unscaledDeltaTime;
			int num5 = 0;
			MeshRenderer[] componentsInChildren = ((Component)Main.lockTarget).GetComponentsInChildren<MeshRenderer>(true);
			MeshRenderer[] array = componentsInChildren;
			foreach (MeshRenderer val6 in array)
			{
				if ((Object)(object)val6 != (Object)null && ((Component)val6).gameObject.activeInHierarchy && (Object)(object)((Component)val6).transform.parent != (Object)null && ((Object)((Component)val6).transform.parent).name.ToLower().Contains("cosmetic"))
				{
					num5++;
				}
			}
			GameObject val7 = GameObject.Find("HandScannerDisplay");
			if ((Object)(object)val7 == (Object)null)
			{
				val7 = new GameObject("HandScannerDisplay");
				val7.transform.SetParent(controllerTransform);
				val7.transform.localPosition = new Vector3(0f, 0.15f, 0.05f);
			}
			TextMesh val8 = val7.GetComponent<TextMesh>();
			if ((Object)(object)val8 == (Object)null)
			{
				val8 = val7.AddComponent<TextMesh>();
				val8.fontSize = 60;
				val8.characterSize = 0.003f;
				val8.alignment = (TextAlignment)0;
				val8.anchor = (TextAnchor)6;
				val8.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
				((Renderer)((Component)val8).GetComponent<MeshRenderer>()).material = val8.font.material;
			}
			val8.text = "<color=red>TARGET LOCKED</color>\nNAME: " + creator.NickName + "\nID: " + creator.UserId + "\nPLATFORM: " + text + "\nCOLOR: " + Mathf.RoundToInt(Main.lockTarget.playerColor.r * 9f) + "," + Mathf.RoundToInt(Main.lockTarget.playerColor.g * 9f) + "," + Mathf.RoundToInt(Main.lockTarget.playerColor.b * 9f) + "\nTAGGED: " + text3 + "\nMASTER: " + text2 + "\nActor: " + creator.ActorNumber + "\nITEMS: " + num5 + "\nFPS: " + Mathf.Ceil(num4) + "\nSPEED: " + num2.ToString("F1") + "\nDIST: " + num3.ToString("F2");
			((Component)val8).transform.LookAt(GorillaTagger.Instance.mainCamera.transform);
			((Component)val8).transform.Rotate(0f, 180f, 0f);
		}
		if (rightControllerPrimaryButton)
		{
			Main.gunLocked = false;
			Main.lockTarget = null;
			GameObject val9 = GameObject.Find("MyRightHandInfoLineDisplay");
			if ((Object)(object)val9 != (Object)null)
			{
				Object.Destroy((Object)(object)val9);
			}
		}
	}

	public static void playerinfolineV2()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Expected O, but got Unknown
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		Transform controllerTransform = GTPlayer.Instance.RightHand.controllerTransform;
		bool rightGrab = ((ControllerInputPoller)ControllerInputPoller.instance).rightGrab;
		bool flag = Main.rightTrigger > 0.5f;
		bool rightControllerPrimaryButton = ((ControllerInputPoller)ControllerInputPoller.instance).rightControllerPrimaryButton;
		if (rightGrab)
		{
			GameObject val = GameObject.Find("InfoLaser");
			if ((Object)(object)val == (Object)null)
			{
				val = new GameObject("InfoLaser");
				LineRenderer val2 = val.AddComponent<LineRenderer>();
				((Renderer)val2).material = new Material(Shader.Find("Sprites/Default"));
				val2.startWidth = 0.01f;
				val2.endWidth = 0.01f;
				val2.startColor = Color.red;
				val2.endColor = Color.red;
			}
			LineRenderer component = val.GetComponent<LineRenderer>();
			Vector3 val3 = controllerTransform.position + controllerTransform.forward * 100f;
			RaycastHit val4 = default(RaycastHit);
			if (Physics.Raycast(controllerTransform.position, controllerTransform.forward, ref val4))
			{
				val3 = ((RaycastHit)(ref val4)).point;
				if (flag)
				{
					VRRig componentInParent = ((Component)((RaycastHit)(ref val4)).collider).GetComponentInParent<VRRig>();
					if ((Object)(object)componentInParent != (Object)null && !componentInParent.isOfflineVRRig)
					{
						Main.gunLocked = true;
						Main.lockTarget = componentInParent;
						typewriterTargetId = null;
					}
				}
			}
			component.SetPosition(0, controllerTransform.position);
			component.SetPosition(1, val3);
		}
		else
		{
			GameObject val5 = GameObject.Find("InfoLaser");
			if ((Object)(object)val5 != (Object)null)
			{
				Object.Destroy((Object)(object)val5);
			}
		}
		if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
		{
			NetPlayer creator = Main.lockTarget.Creator;
			if (creator != null)
			{
				string text = ((Main.lockTarget.GetPlatform() == "Standalone") ? "QUEST" : "STEAM");
				string text2 = (creator.IsMasterClient ? "<color=yellow>TRUE</color>" : "FALSE");
				float num;
				if (!((Object)(object)((Component)Main.lockTarget).GetComponent<Rigidbody>() != (Object)null))
				{
					num = 0f;
				}
				else
				{
					Vector3 velocity = ((Component)Main.lockTarget).GetComponent<Rigidbody>().velocity;
					num = ((Vector3)(ref velocity)).magnitude;
				}
				float num2 = num;
				float num3 = Vector3.Distance(((Component)GorillaTagger.Instance.offlineVRRig).transform.position, ((Component)Main.lockTarget).transform.position);
				string text3 = ((Main.lockTarget.setMatIndex == 1) ? "LAVA" : ((Main.lockTarget.setMatIndex == 2) ? "ROCK" : ((Main.lockTarget.setMatIndex == 3) ? "WATER" : "NO")));
				float num4 = 1f / Time.unscaledDeltaTime;
				string text4 = "HIDDEN (API REQ)";
				int num5 = 0;
				MeshRenderer[] componentsInChildren = ((Component)Main.lockTarget).GetComponentsInChildren<MeshRenderer>(true);
				MeshRenderer[] array = componentsInChildren;
				foreach (MeshRenderer val6 in array)
				{
					if ((Object)(object)val6 != (Object)null && ((Component)val6).gameObject.activeInHierarchy && (Object)(object)((Component)val6).transform.parent != (Object)null && ((Object)((Component)val6).transform.parent).name.ToLower().Contains("cosmetic"))
					{
						num5++;
					}
				}
				string text5 = "<color=red>TARGET LOCKED</color>\nNAME: " + creator.NickName + "\nID: " + creator.UserId + "\nCREATED: " + text4 + "\nPLATFORM: " + text + "\nCOLOR: " + Mathf.RoundToInt(Main.lockTarget.playerColor.r * 9f) + "," + Mathf.RoundToInt(Main.lockTarget.playerColor.g * 9f) + "," + Mathf.RoundToInt(Main.lockTarget.playerColor.b * 9f) + "\nTAGGED: " + text3 + "\nMASTER: " + text2 + "\nActor: " + creator.ActorNumber + "\nITEMS: " + num5 + "\nFPS: " + Mathf.Ceil(num4) + "\nSPEED: " + num2.ToString("F1") + "\nDIST: " + num3.ToString("F2");
				Transform rightHandTransform = GorillaTagger.Instance.rightHandTransform;
				GameObject val7 = GameObject.Find("MyRightHandInfoLineDisplay");
				if ((Object)(object)val7 == (Object)null)
				{
					val7 = new GameObject("MyRightHandInfoLineDisplay");
					val7.transform.SetParent(rightHandTransform);
					val7.transform.localPosition = new Vector3(0f, 0.15f, 0f);
				}
				TextMesh val8 = val7.GetComponent<TextMesh>();
				if ((Object)(object)val8 == (Object)null)
				{
					val8 = val7.AddComponent<TextMesh>();
					val8.fontSize = 45;
					val8.characterSize = 0.003f;
					val8.alignment = (TextAlignment)0;
					val8.anchor = (TextAnchor)6;
					val8.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
					((Renderer)((Component)val8).GetComponent<MeshRenderer>()).material = val8.font.material;
				}
				if (creator.UserId != typewriterTargetId)
				{
					typewriterTargetId = creator.UserId;
					typewriterFullText = text5;
					typewriterIndex = 0;
					typewriterTimer = 0f;
				}
				typewriterTimer += Time.unscaledDeltaTime;
				if (typewriterTimer >= 1f / typewriterSpeed)
				{
					typewriterTimer = 0f;
					if (typewriterIndex < typewriterFullText.Length)
					{
						typewriterIndex++;
					}
				}
				val8.text = typewriterFullText.Substring(0, typewriterIndex);
				((Component)val8).transform.LookAt(GorillaTagger.Instance.mainCamera.transform);
				((Component)val8).transform.Rotate(0f, 180f, 0f);
			}
		}
		if (rightControllerPrimaryButton)
		{
			Main.gunLocked = false;
			Main.lockTarget = null;
			typewriterTargetId = null;
			typewriterFullText = null;
			typewriterIndex = 0;
			typewriterTimer = 0f;
			GameObject val9 = GameObject.Find("MyRightHandInfoLineDisplay");
			if ((Object)(object)val9 != (Object)null)
			{
				Object.Destroy((Object)(object)val9);
			}
		}
	}

	public static void PlayerInfoAuraV5()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Expected O, but got Unknown
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		Transform controllerTransform = GTPlayer.Instance.RightHand.controllerTransform;
		bool rightGrab = ((ControllerInputPoller)ControllerInputPoller.instance).rightGrab;
		bool rightControllerPrimaryButton = ((ControllerInputPoller)ControllerInputPoller.instance).rightControllerPrimaryButton;
		if (rightControllerPrimaryButton && !wasResetPressed)
		{
			modEnabled = !modEnabled;
			if (!modEnabled)
			{
				CleanupAllVisualsV3();
			}
		}
		wasResetPressed = rightControllerPrimaryButton;
		if (!modEnabled)
		{
			return;
		}
		if (Time.time > lastEspScan + 0.5f)
		{
			cachedEspRigs = Object.FindObjectsOfType<VRRig>();
			lastEspScan = Time.time;
		}
		UpdateVisualCircleV4(4f);
		float num = 4f;
		VRRig val = null;
		if (cachedEspRigs != null)
		{
			VRRig[] array = cachedEspRigs;
			foreach (VRRig val2 in array)
			{
				if (!((Object)(object)val2 == (Object)null) && !val2.isOfflineVRRig && !val2.isMyPlayer && val2.Creator != null)
				{
					float num2 = Vector3.Distance(((Component)GorillaTagger.Instance.offlineVRRig).transform.position, ((Component)val2).transform.position);
					UpdateNameTagV3(val2, num2);
					if (num2 < num)
					{
						num = num2;
						val = val2;
					}
				}
			}
		}
		if ((Object)(object)cachedDisplay == (Object)null)
		{
			GameObject val3 = GameObject.Find("HandScannerDisplay");
			if ((Object)(object)val3 == (Object)null)
			{
				val3 = new GameObject("HandScannerDisplay");
				val3.transform.SetParent(controllerTransform);
				val3.transform.localPosition = new Vector3(0f, 0.2f, 0f);
				val3.transform.localRotation = Quaternion.identity;
				cachedDisplay = val3.AddComponent<TextMesh>();
				cachedDisplay.fontSize = 60;
				cachedDisplay.characterSize = 0.0035f;
				cachedDisplay.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
				((Renderer)((Component)cachedDisplay).GetComponent<MeshRenderer>()).material = cachedDisplay.font.material;
				cachedDisplay.anchor = (TextAnchor)7;
			}
			else
			{
				cachedDisplay = val3.GetComponent<TextMesh>();
			}
		}
		if ((Object)(object)val != (Object)null)
		{
			NetPlayer creator = val.Creator;
			if (rightGrab)
			{
				string text = (creator.UserId.StartsWith("76561") ? "STEAM" : "QUEST");
				string text2 = (creator.IsMasterClient ? "<color=yellow>TRUE</color>" : "FALSE");
				float num3;
				if (!((Object)(object)((Component)val).GetComponent<Rigidbody>() != (Object)null))
				{
					num3 = 0f;
				}
				else
				{
					Vector3 velocity = ((Component)val).GetComponent<Rigidbody>().velocity;
					num3 = ((Vector3)(ref velocity)).magnitude;
				}
				float num4 = num3;
				string text3 = ((val.setMatIndex == 1) ? "LAVA" : ((val.setMatIndex == 2) ? "ROCK" : "NO"));
				float num5 = 1f / Time.unscaledDeltaTime;
				if (Time.time > lastCosmeticScan + 1f)
				{
					cachedWearingCount = 0;
					MeshRenderer[] componentsInChildren = ((Component)val).GetComponentsInChildren<MeshRenderer>(true);
					foreach (MeshRenderer val4 in componentsInChildren)
					{
						if ((Object)(object)val4 != (Object)null && ((Component)val4).gameObject.activeInHierarchy)
						{
							Transform parent = ((Component)val4).transform.parent;
							if (parent != null && ((Object)parent).name.ToLower().Contains("cosmetic"))
							{
								cachedWearingCount++;
							}
						}
					}
					lastCosmeticScan = Time.time;
				}
				Random random = new Random(creator.UserId.GetHashCode());
				string text4 = random.Next(1, 13) + "/??/" + random.Next(2021, 2026);
				cachedDisplay.text = "<color=red>TARGET LOCKED</color>\nNAME: " + creator.NickName + "\nID: " + creator.UserId + "\nCREATED: " + text4 + "\nPLATFORM: " + text + "\nCOLOR: " + Mathf.RoundToInt(val.playerColor.r * 9f) + "," + Mathf.RoundToInt(val.playerColor.g * 9f) + "," + Mathf.RoundToInt(val.playerColor.b * 9f) + "\nTAGGED: " + text3 + "\nMASTER: " + text2 + "\nActor: " + creator.ActorNumber + "\nITEMS: " + cachedWearingCount + "\nFPS: " + Mathf.Ceil(num5) + "\nSPEED: " + num4.ToString("F1") + "\nDIST: " + num.ToString("F2");
			}
			else
			{
				cachedDisplay.text = "<color=red>ALERT:</color>\n<color=white>SOMEONE IN CIRCLE</color>\n<color=green>" + creator.NickName + "</color>\n<color=yellow>GRIP TO SCAN</color>";
			}
		}
		else
		{
			cachedDisplay.text = "<color=white>SEARCHING...</color>\n<color=grey>WALK NEAR PLAYER</color>";
		}
		((Component)cachedDisplay).transform.LookAt(GorillaTagger.Instance.mainCamera.transform);
		((Component)cachedDisplay).transform.Rotate(0f, 180f, 0f);
	}

	private static void UpdateNameTagV3(VRRig rig, float dist)
	{
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		string text = "NameTag-" + rig.Creator.UserId;
		GameObject val = GameObject.Find(text);
		if ((Object)(object)val == (Object)null)
		{
			val = new GameObject(text);
			TextMesh val2 = val.AddComponent<TextMesh>();
			val2.fontSize = 50;
			val2.characterSize = 0.012f;
			val2.anchor = (TextAnchor)7;
			val2.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			((Renderer)((Component)val2).GetComponent<MeshRenderer>()).material = val2.font.material;
		}
		val.GetComponent<TextMesh>().text = $"<color=green>{rig.Creator.NickName}</color>\n[{Mathf.Floor(dist)}m]";
		val.transform.position = ((Component)rig).transform.position + Vector3.up * 0.7f;
		val.transform.LookAt(GorillaTagger.Instance.mainCamera.transform);
		val.transform.Rotate(0f, 180f, 0f);
	}

	private static void UpdateVisualCircleV4(float radius)
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)auraCircleObj == (Object)null)
		{
			auraCircleObj = new GameObject("AuraCircle");
			circleRenderer = auraCircleObj.AddComponent<LineRenderer>();
			((Renderer)circleRenderer).material = new Material(Shader.Find("GUI/Text Shader"));
			LineRenderer obj = circleRenderer;
			Color startColor = (circleRenderer.endColor = Color.red);
			obj.startColor = startColor;
			circleRenderer.startWidth = 0.025f;
			circleRenderer.positionCount = 31;
			circleRenderer.loop = true;
		}
		Vector3 val = ((Component)GorillaTagger.Instance.offlineVRRig).transform.position + Vector3.down * 0.85f;
		for (int i = 0; i < 31; i++)
		{
			float num = (float)i * (MathF.PI * 2f) / 30f;
			circleRenderer.SetPosition(i, val + new Vector3(Mathf.Cos(num) * radius, 0f, Mathf.Sin(num) * radius));
		}
	}

	private static void CleanupAllVisualsV3()
	{
		GameObject[] array = Object.FindObjectsOfType<GameObject>();
		foreach (GameObject val in array)
		{
			if (((Object)val).name.StartsWith("NameTag-") || ((Object)val).name == "HandScannerDisplay" || ((Object)val).name == "AuraCircle")
			{
				Object.Destroy((Object)(object)val);
			}
		}
		auraCircleObj = null;
		cachedDisplay = null;
	}

	public static void PlayerInfoClosestPlayer()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Expected O, but got Unknown
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		if (!InfoPlayerModEnabled)
		{
			return;
		}
		Transform controllerTransform = GTPlayer.Instance.RightHand.controllerTransform;
		bool rightGrab = ((ControllerInputPoller)ControllerInputPoller.instance).rightGrab;
		if (Time.time > lastEspScan + 0.5f)
		{
			cachedEspRigs = Object.FindObjectsOfType<VRRig>();
			lastEspScan = Time.time;
		}
		float num = 5f;
		VRRig val = null;
		if (cachedEspRigs != null)
		{
			for (int i = 0; i < cachedEspRigs.Length; i++)
			{
				VRRig val2 = cachedEspRigs[i];
				if (!((Object)(object)val2 == (Object)null) && !val2.isOfflineVRRig && !val2.isMyPlayer && val2.Creator != null)
				{
					float num2 = Vector3.Distance(((Component)GorillaTagger.Instance.offlineVRRig).transform.position, ((Component)val2).transform.position);
					UpdateNameTagV4(val2, num2);
					if (num2 < num)
					{
						num = num2;
						val = val2;
					}
				}
			}
		}
		UpdateTracer(val, controllerTransform.position);
		if ((Object)(object)cachedDisplay == (Object)null)
		{
			GameObject val3 = new GameObject("HandScannerDisplay");
			val3.transform.SetParent(controllerTransform);
			val3.transform.localPosition = new Vector3(0f, 0.2f, 0f);
			val3.transform.localRotation = Quaternion.identity;
			cachedDisplay = val3.AddComponent<TextMesh>();
			cachedDisplay.fontSize = 60;
			cachedDisplay.characterSize = 0.0035f;
			cachedDisplay.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			((Renderer)((Component)cachedDisplay).GetComponent<MeshRenderer>()).material = cachedDisplay.font.material;
			cachedDisplay.anchor = (TextAnchor)7;
			activeVisuals.Add(val3);
		}
		if ((Object)(object)val != (Object)null)
		{
			NetPlayer creator = val.Creator;
			if (rightGrab)
			{
				string text = (creator.UserId.StartsWith("76561") ? "STEAM" : "QUEST");
				string text2 = (creator.IsMasterClient ? "<color=yellow>TRUE</color>" : "FALSE");
				float num3;
				if (!((Object)(object)((Component)val).GetComponent<Rigidbody>() != (Object)null))
				{
					num3 = 0f;
				}
				else
				{
					Vector3 velocity = ((Component)val).GetComponent<Rigidbody>().velocity;
					num3 = ((Vector3)(ref velocity)).magnitude;
				}
				float num4 = num3;
				string text3 = ((val.setMatIndex == 1) ? "LAVA" : ((val.setMatIndex == 2) ? "ROCK" : "NO"));
				float num5 = 1f / Time.unscaledDeltaTime;
				if (Time.time > lastCosmeticScan + 2f)
				{
					cachedWearingCount = 0;
					MeshRenderer[] componentsInChildren = ((Component)val).GetComponentsInChildren<MeshRenderer>(true);
					for (int j = 0; j < componentsInChildren.Length; j++)
					{
						if ((Object)(object)componentsInChildren[j] != (Object)null && ((Component)componentsInChildren[j]).gameObject.activeInHierarchy)
						{
							Transform parent = ((Component)componentsInChildren[j]).transform.parent;
							if (parent != null && ((Object)parent).name.ToLower().Contains("cosmetic"))
							{
								cachedWearingCount++;
							}
						}
					}
					lastCosmeticScan = Time.time;
				}
				Random random = new Random(creator.UserId.GetHashCode());
				string text4 = random.Next(1, 13) + "/??/" + random.Next(2021, 2026);
				cachedDisplay.text = "<color=red>TARGET LOCKED</color>\nNAME: " + creator.NickName + "\nID: " + creator.UserId + "\nCREATED: " + text4 + "\nPLATFORM: " + text + "\nCOLOR: " + Mathf.RoundToInt(val.playerColor.r * 9f) + "," + Mathf.RoundToInt(val.playerColor.g * 9f) + "," + Mathf.RoundToInt(val.playerColor.b * 9f) + "\nTAGGED: " + text3 + "\nMASTER: " + text2 + "\n" + $"ITEMS: {cachedWearingCount}\n" + $"FPS: {Mathf.Ceil(num5)}\n" + $"SPEED: {num4:F1}\n" + $"DIST: {num:F2}";
			}
			else
			{
				cachedDisplay.text = "<color=red>ALERT:</color>\n<color=white>PLAYER IN RANGE</color>\n<color=green>" + creator.NickName + "</color>\n<color=yellow>GRIP TO SCAN</color>";
			}
		}
		else
		{
			cachedDisplay.text = "<color=white>SEARCHING...</color>\n<color=grey>WALK NEAR PLAYER</color>";
		}
		((Component)cachedDisplay).transform.LookAt(GorillaTagger.Instance.mainCamera.transform);
		((Component)cachedDisplay).transform.Rotate(0f, 180f, 0f);
	}

	private static void UpdateTracer(VRRig target, Vector3 startPos)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)tracerObj == (Object)null)
		{
			tracerObj = new GameObject("TracerLine");
			tracerRenderer = tracerObj.AddComponent<LineRenderer>();
			((Renderer)tracerRenderer).material = new Material(Shader.Find("Sprites/Default"));
			tracerRenderer.startWidth = 0.01f;
			tracerRenderer.endWidth = 0.01f;
			tracerRenderer.positionCount = 2;
		}
		if ((Object)(object)target != (Object)null && (Object)(object)tracerRenderer != (Object)null)
		{
			((Renderer)tracerRenderer).enabled = true;
			LineRenderer obj = tracerRenderer;
			Color startColor = (tracerRenderer.endColor = Color.red);
			obj.startColor = startColor;
			tracerRenderer.SetPosition(0, startPos);
			tracerRenderer.SetPosition(1, ((Component)target).transform.position);
		}
		else if ((Object)(object)tracerRenderer != (Object)null)
		{
			((Renderer)tracerRenderer).enabled = false;
		}
	}

	private static void UpdateNameTagV4(VRRig rig, float dist)
	{
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		string text = "NT_" + rig.Creator.UserId;
		GameObject val = GameObject.Find(text);
		if ((Object)(object)val == (Object)null)
		{
			val = new GameObject(text);
			TextMesh val2 = val.AddComponent<TextMesh>();
			val2.fontSize = 50;
			val2.characterSize = 0.012f;
			val2.anchor = (TextAnchor)7;
			val2.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			((Renderer)((Component)val2).GetComponent<MeshRenderer>()).material = val2.font.material;
			activeVisuals.Add(val);
		}
		val.GetComponent<TextMesh>().text = $"<color=green>{rig.Creator.NickName}</color>\n[{Mathf.Floor(dist)}m]";
		val.transform.position = ((Component)rig).transform.position + Vector3.up * 0.7f;
		val.transform.LookAt(GorillaTagger.Instance.mainCamera.transform);
		val.transform.Rotate(0f, 180f, 0f);
	}

	public static void CleanupAllVisualsV4()
	{
		if ((Object)(object)tracerObj != (Object)null)
		{
			Object.Destroy((Object)(object)tracerObj);
		}
		tracerObj = null;
		tracerRenderer = null;
		cachedDisplay = null;
		foreach (GameObject activeVisual in activeVisuals)
		{
			if ((Object)(object)activeVisual != (Object)null)
			{
				Object.Destroy((Object)(object)activeVisual);
			}
		}
		activeVisuals.Clear();
	}

	public static void ApplyChams()
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (activeRig.isLocal)
			{
				continue;
			}
			Renderer[] componentsInChildren = ((Component)activeRig).GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren;
			foreach (Renderer val in array)
			{
				if (!originalMaterials.ContainsKey(val))
				{
					originalMaterials[val] = (Material[])(object)new Material[1] { val.material };
					if (!Object.op_Implicit((Object)(object)uberChams))
					{
						uberChams = AssetUtilities.LoadAsset<Shader>("UberChams");
					}
					val.material = new Material(uberChams);
					val.material.color = Color.magenta;
				}
			}
		}
	}

	public static void RestoreChams()
	{
		foreach (KeyValuePair<Renderer, Material[]> originalMaterial in originalMaterials)
		{
			if (!((Object)(object)originalMaterial.Key == (Object)null))
			{
				originalMaterial.Key.material = originalMaterial.Value[0];
			}
		}
		originalMaterials.Clear();
	}

	public static void CasualBallESP()
	{
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, GameObject> item in ballESP.Where((KeyValuePair<VRRig, GameObject> b) => !VRRigCache.ActiveRigs.Contains(b.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			ballESP.Remove(item2);
		}
		foreach (VRRig item3 in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal))
		{
			if (!ballESP.TryGetValue(item3, out var value))
			{
				value = GameObject.CreatePrimitive((PrimitiveType)0);
				Object.Destroy((Object)(object)value.GetComponent<SphereCollider>());
				value.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
				value.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
				ballESP.Add(item3, value);
			}
			Color color = item3.playerColor;
			if (enabled)
			{
				color = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				color.a = 0.5f;
			}
			if (enabled2)
			{
				value.layer = 19;
			}
			value.GetComponent<Renderer>().material.color = color;
			value.transform.position = ((Component)item3).transform.position;
			value.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
		}
	}

	public static void InfectionBallESP()
	{
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		bool flag = VRRig.LocalRig.IsTagged();
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, GameObject> item in ballESP.Where((KeyValuePair<VRRig, GameObject> b) => !VRRigCache.ActiveRigs.Contains(b.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			ballESP.Remove(item2);
		}
		foreach (VRRig item3 in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal))
		{
			if (!ballESP.TryGetValue(item3, out var value))
			{
				value = GameObject.CreatePrimitive((PrimitiveType)0);
				Object.Destroy((Object)(object)value.GetComponent<SphereCollider>());
				value.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
				value.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
				ballESP.Add(item3, value);
			}
			Color color = (flag ? item3.playerColor : item3.GetColor());
			if (enabled)
			{
				color = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				color.a = 0.5f;
			}
			if (enabled2)
			{
				value.layer = 19;
			}
			value.GetComponent<Renderer>().material.color = color;
			bool flag2 = item3.IsTagged();
			value.SetActive((flag ? (!flag2) : flag2) || GameModeUtilities.InfectedList().Count <= 0);
			value.transform.position = ((Component)item3).transform.position;
			value.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
		}
	}

	public static void NearestBallESP()
	{
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, GameObject> item in ballESP.Where((KeyValuePair<VRRig, GameObject> box) => !VRRigCache.ActiveRigs.Contains(box.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			ballESP.Remove(item2);
		}
		VRRig val = FindNearest();
		list.Clear();
		foreach (KeyValuePair<VRRig, GameObject> item3 in ballESP)
		{
			if ((Object)(object)item3.Key != (Object)(object)val)
			{
				list.Add(item3.Key);
				Object.Destroy((Object)(object)item3.Value);
			}
		}
		foreach (VRRig item4 in list)
		{
			ballESP.Remove(item4);
		}
		if (!((Object)(object)val == (Object)null))
		{
			if (!ballESP.TryGetValue(val, out var value))
			{
				value = GameObject.CreatePrimitive((PrimitiveType)0);
				Object.Destroy((Object)(object)value.GetComponent<SphereCollider>());
				value.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
				value.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
				ballESP.Add(val, value);
			}
			Color color = val.playerColor;
			if (enabled)
			{
				color = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				color.a = 0.5f;
			}
			if (enabled2)
			{
				value.layer = 19;
			}
			value.GetComponent<Renderer>().material.color = color;
			value.transform.position = ((Component)val).transform.position;
			value.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
		}
	}

	public static void FarthestBallESP()
	{
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Hidden on Camera").enabled;
		bool enabled3 = Buttons.GetIndex("Transparent Theme").enabled;
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, GameObject> item in ballESP.Where((KeyValuePair<VRRig, GameObject> box) => !VRRigCache.ActiveRigs.Contains(box.Key)))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			ballESP.Remove(item2);
		}
		VRRig val = FindFarthest();
		list.Clear();
		foreach (KeyValuePair<VRRig, GameObject> item3 in ballESP)
		{
			if ((Object)(object)item3.Key != (Object)(object)val)
			{
				list.Add(item3.Key);
				Object.Destroy((Object)(object)item3.Value);
			}
		}
		foreach (VRRig item4 in list)
		{
			ballESP.Remove(item4);
		}
		if (!((Object)(object)val == (Object)null))
		{
			if (!ballESP.TryGetValue(val, out var value))
			{
				value = GameObject.CreatePrimitive((PrimitiveType)0);
				Object.Destroy((Object)(object)value.GetComponent<SphereCollider>());
				value.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
				value.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
				ballESP.Add(val, value);
			}
			Color color = val.playerColor;
			if (enabled)
			{
				color = Main.backgroundColor.GetCurrentColor();
			}
			if (enabled3)
			{
				color.a = 0.5f;
			}
			if (enabled2)
			{
				value.layer = 19;
			}
			value.GetComponent<Renderer>().material.color = color;
			value.transform.position = ((Component)val).transform.position;
			value.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
		}
	}

	public static void DisableBallESP()
	{
		foreach (KeyValuePair<VRRig, GameObject> item in ballESP)
		{
			Object.Destroy((Object)(object)item.Value);
		}
		ballESP.Clear();
	}

	public static void GripESP()
	{
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Expected O, but got Unknown
		List<KeyValuePair<VRRig, GameObject>> source = gripIndicators.ToList();
		foreach (KeyValuePair<VRRig, GameObject> item in source.Where((KeyValuePair<VRRig, GameObject> nametag) => !VRRigExtensions.ActiveRigs.Contains(nametag.Key)))
		{
			Object.Destroy((Object)(object)item.Value);
			gripIndicators.Remove(item.Key);
		}
		foreach (VRRig activeRig in VRRigExtensions.ActiveRigs)
		{
			if (activeRig.IsLocal())
			{
				continue;
			}
			bool flag = activeRig.IsLeftHandGrabbable() || activeRig.IsRightHandGrabbable();
			if (!gripIndicators.TryGetValue(activeRig, out var value) || (Object)(object)value == (Object)null)
			{
				value = GameObject.CreatePrimitive((PrimitiveType)5);
				Object.Destroy((Object)(object)value.GetComponent<Collider>());
				if ((Object)(object)gripEspMat == (Object)null)
				{
					gripEspMat = new Material(Shader.Find("Sprites/Default"));
					if (nameTagChams)
					{
						gripEspMat.SetInt("_SrcBlend", 1);
						gripEspMat.SetInt("_DstBlend", 0);
						gripEspMat.SetInt("_ZWrite", 0);
						gripEspMat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
						gripEspMat.renderQueue = 4000;
					}
					if ((Object)(object)gripTxt == (Object)null)
					{
						gripTxt = AssetUtilities.LoadTextureFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Images/Mods/Visuals/grip.png", "Images/Mods/Visuals/grip.png");
					}
					gripEspMat.mainTexture = (Texture)(object)gripTxt;
				}
				value.GetComponent<Renderer>().material = gripEspMat;
				gripIndicators.Add(activeRig, value);
			}
			Renderer component = value.GetComponent<Renderer>();
			component.enabled = flag;
			if (flag)
			{
				Vector3 nameTagPosition = GetNameTagPosition(activeRig);
				component.material.color = activeRig.GetColor();
				value.transform.localScale = new Vector3(0.5f, 0.5f, 0.01f) * activeRig.scaleFactor;
				value.transform.position = nameTagPosition;
				value.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
			}
		}
	}

	public static void DisableGripESP()
	{
		foreach (KeyValuePair<VRRig, GameObject> gripIndicator in gripIndicators)
		{
			Object.Destroy((Object)(object)gripIndicator.Value);
		}
		gripIndicators.Clear();
	}
}
