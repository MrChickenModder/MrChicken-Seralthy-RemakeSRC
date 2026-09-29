using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Photon.Pun;
using Photon.Realtime;
using Seralyth.Classes.Menu;
using Seralyth.Extensions;
using Seralyth.Managers;
using Seralyth.Menu;
using Seralyth.Patches.Menu;
using Seralyth.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

namespace Seralyth.Mods;

public static class Experimental
{
	public static class shibaholdable
	{
		private static int assetId = -1;

		public static void Enable()
		{
			assetId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "shibaholdable", "shiba", assetId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, assetId, 2);
		}

		public static void Disable()
		{
			if (assetId >= 0)
			{
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, assetId);
				assetId = -1;
			}
		}
	}

	public static class pigeon
	{
		private static int assetId = -1;

		public static void Enable()
		{
			assetId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "pigeon", "Pigeon", assetId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, assetId, 2);
		}

		public static void Disable()
		{
			if (assetId >= 0)
			{
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, assetId);
				assetId = -1;
			}
		}
	}

	public static class NoliStar
	{
		private enum NoliStarState
		{
			Default,
			Throwing,
			Respawning
		}

		private static int noliStarId = -1;

		private static int noliMusicId = -1;

		private static float updatedTimeDelay;

		private static float respawnTime;

		private static bool holdingTrigger;

		private static Vector3 throwDirection;

		private static Vector3 networkedPosition;

		private static Quaternion networkedRotation;

		private static NoliStarState noliStarState = NoliStarState.Default;

		public static void Enable()
		{
			noliStarId = -1;
			noliMusicId = -1;
			noliStarState = NoliStarState.Default;
			holdingTrigger = false;
		}

		public static void Run()
		{
			NoliStarMethod();
			if (Main.rightGrab)
			{
				NoliMusicMethod();
			}
		}

		public static void Disable()
		{
			if (noliStarId >= 0)
			{
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, noliStarId);
				noliStarId = -1;
			}
			if (noliMusicId >= 0)
			{
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, noliMusicId);
				noliMusicId = -1;
			}
			noliStarState = NoliStarState.Default;
			holdingTrigger = false;
			updatedTimeDelay = 0f;
			respawnTime = 0f;
		}

		private static void NoliStarMethod()
		{
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_057b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0586: Unknown result type (might be due to invalid IL or missing references)
			//IL_0592: Unknown result type (might be due to invalid IL or missing references)
			//IL_059d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0608: Unknown result type (might be due to invalid IL or missing references)
			//IL_063a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0440: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			if (noliStarId < 0)
			{
				noliStarId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "console.main1", "Star", noliStarId);
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, noliStarId, "Model", "StarSpawn");
				Main.RPCProtection();
			}
			if (!Seralyth.Classes.Menu.Console.consoleAssets.ContainsKey(noliStarId))
			{
				return;
			}
			GameObject assetObject = Seralyth.Classes.Menu.Console.consoleAssets[noliStarId].assetObject;
			if (Main.rightTrigger > 0.5f && noliStarState == NoliStarState.Default)
			{
				RaycastHit val = default(RaycastHit);
				Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.forward, ref val, 512f, LayerMask.op_Implicit(GTPlayer.Instance.locomotionEnabledLayers));
				GameObject val2 = GameObject.CreatePrimitive((PrimitiveType)0);
				val2.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
				val2.transform.position = ((((RaycastHit)(ref val)).point == Vector3.zero) ? (((RaycastHit)(ref val)).transform.position + ((RaycastHit)(ref val)).transform.forward * 20f) : ((RaycastHit)(ref val)).point);
				val2.GetComponent<Renderer>().material.color = Color.white;
				Object.Destroy((Object)(object)val2, Time.deltaTime);
				Object.Destroy((Object)(object)val2.GetComponent<Collider>());
			}
			if (Main.rightTrigger < 0.5f && holdingTrigger && noliStarState == NoliStarState.Default)
			{
				noliStarState = NoliStarState.Throwing;
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, noliStarId, "Model", "Throw");
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, noliStarId, "Model", "ThrowStar");
				RaycastHit val3 = default(RaycastHit);
				Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.forward, ref val3, 512f, LayerMask.op_Implicit(GTPlayer.Instance.locomotionEnabledLayers));
				Vector3 val4 = ((RaycastHit)(ref val3)).point - assetObject.transform.position;
				throwDirection = ((Vector3)(ref val4)).normalized;
			}
			holdingTrigger = Main.rightTrigger > 0.5f;
			switch (noliStarState)
			{
			case NoliStarState.Default:
				assetObject.transform.position = GorillaTagger.Instance.rightHandTransform.position + Vector3.up * 0.2f;
				assetObject.transform.rotation = Quaternion.Euler(Time.time * 32f, Time.time * 10f, Time.time * 47f);
				break;
			case NoliStarState.Throwing:
			{
				RaycastHit val5 = default(RaycastHit);
				Physics.Raycast(assetObject.transform.position, throwDirection, ref val5, 0.5f, LayerMask.op_Implicit(GTPlayer.Instance.locomotionEnabledLayers));
				if (((RaycastHit)(ref val5)).point == Vector3.zero)
				{
					Transform transform = assetObject.transform;
					transform.position += throwDirection * (Time.deltaTime * 15f);
					assetObject.transform.rotation = Quaternion.Euler(Time.time * 239f, Time.time * 201f, Time.time * 170f);
					break;
				}
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, noliStarId, "Model", "Explode");
				bool flag = false;
				foreach (VRRig activeRig in VRRigCache.ActiveRigs)
				{
					if (!activeRig.isLocal && Vector3.Distance(assetObject.transform.position, ((Component)activeRig).transform.position) < 2.32775f)
					{
						Seralyth.Classes.Menu.Console.ExecuteCommand("silkick", (ReceiverGroup)1, activeRig.OwningNetPlayer.UserId);
						flag = true;
					}
				}
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, noliStarId, "Model", flag ? "KillStar" : "BreakStar");
				noliStarState = NoliStarState.Respawning;
				respawnTime = Time.time + 3f;
				break;
			}
			case NoliStarState.Respawning:
				if (Time.time > respawnTime)
				{
					Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, noliStarId, "Model", "Default");
					Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, noliStarId, "Model", "StarSpawn");
					noliStarState = NoliStarState.Default;
				}
				break;
			}
			if (Time.time > updatedTimeDelay && (networkedRotation != assetObject.transform.rotation || networkedPosition != assetObject.transform.position))
			{
				updatedTimeDelay = Time.time + 0.05f;
				networkedPosition = assetObject.transform.position;
				networkedRotation = assetObject.transform.rotation;
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, noliStarId, assetObject.transform.position);
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setrotation", (ReceiverGroup)1, noliStarId, assetObject.transform.rotation);
			}
		}

		private static void NoliMusicMethod()
		{
			if (noliMusicId < 0)
			{
				noliMusicId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "console.main1", "RangedMusic", noliMusicId);
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, noliMusicId, 0);
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, noliMusicId, "Level1", "NoliLevel1");
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, noliMusicId, "Level2", "NoliLevel2");
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, noliMusicId, "Level3", "NoliLevel3");
				Main.RPCProtection();
			}
		}
	}

	private static readonly Dictionary<Renderer, Material> oldMats = new Dictionary<Renderer, Material>();

	public static int restartIndex;

	public static float restartDelay;

	public static Vector3 restartPosition;

	public static string restartRoom;

	private static float adminEventDelay;

	public static List<string> platExcluded = new List<string>();

	private static int allocatedTSEFId;

	public static bool isassetsbig;

	private static int allocated1xId = -1;

	private static int allocatedMiniTravisId = -1;

	private static int allocatedRSwordId = -1;

	private static bool lastVelTooHighRS;

	private static float pauseSfx;

	private static float slashDelay;

	public static bool AdminPlatformsLastLeft;

	public static bool AdminPlatformsLastRight;

	public static Vector3 speedLastVel;

	private static float jumpscareDelay;

	public static bool muted;

	private static readonly Dictionary<VRRig, Coroutine> freezePool = new Dictionary<VRRig, Coroutine>();

	private static readonly List<int> FullActorNumbers = new List<int>();

	private static bool lastInRoom2;

	private static int lastPlayerCount2 = -1;

	private static float stdell;

	private static VRRig thestrangled;

	private static VRRig thestrangledleft;

	private static float lastnetscale = 1f;

	private static float scalenetdel;

	private static int lastplayercount;

	private static Vector3 whereOriginalPlayerPos = Vector3.zero;

	private static Vector3 originalMePosition = Vector3.zero;

	private static bool lastInRoom;

	private static int lastPlayerCount = -1;

	public static bool userTagHooked;

	private static readonly Dictionary<VRRig, GameObject> nametags = new Dictionary<VRRig, GameObject>();

	public static bool tracerTagHooked;

	private static readonly Dictionary<VRRig, string> menuUsers = new Dictionary<VRRig, string>();

	public static readonly Dictionary<string, string> onConduct = new Dictionary<string, string>();

	public static float FindUserTime;

	public static bool isUserFound;

	private static float thingdeb;

	public static string targetRoom;

	public static string targetNotification;

	private static bool lastLasering;

	private static float beamDelay;

	private static float startTimeTrigger;

	private static bool lastTriggerLaserSpam;

	public static int[] oldCosmetics;

	public static int[] oldTryOn;

	private static int allocatedCoinId = -1;

	public static int coinChain;

	public static bool coinChainHeads;

	public static int coinHeads;

	public static int coinTails;

	private static bool lastFlipping;

	public static int assetId;

	public static bool hastwerked = false;

	private static int allocatedAxeId = -1;

	private static Coroutine nukeFallRoutine;

	private static int nukeAssetId = -1;

	private static int allocatedPhysId = -1;

	private static bool physGunLastGrip;

	private static VRRig physGunTargetHold;

	private static float physGunRigDistance;

	private static float physGunStandaloneTriggerDelay;

	private static float physGunPositionDelay;

	private static GameObject physGunCrosshair;

	private static int allocatedPistolId = -1;

	private static bool lastPistolTrigger;

	private static GameObject pistolCrosshair;

	private static int allocatedConcertId = -1;

	public static int concertVideoIndex;

	public static readonly string[] ConcertVideoNames = new string[24]
	{
		"MOJO JOJO", "New Tank", "CRANK", "Over", "POP OUT", "OPM BABI", "Long Time", "Punk Monk", "R.I.P. Fredo (Notice Me)", "Foreign",
		"Sky", "FINE SHIT", "JumpOutTheHouse", "Lean 4 Real", "DIAMONDS SPECIAL", "RADAR", "Mileage", "Rockstar Made", "SOME MORE", "I SEEEE YOU BABY BOI",
		"DRUGS GOT ME NUMB", "OLYMPIAN", "F33l Lik3 Dyin", "BACKD00R"
	};

	private static int allocatedModMenuId = -1;

	private static readonly List<float> beatIntervals = new List<float>();

	private static readonly float[] boomboxEnergyHistory = new float[43];

	private static readonly float[] boomboxSamples = new float[1024];

	public static int boomboxId = -1;

	public static float boomboxCurrentBpm;

	private static int boomboxHistoryIndex;

	private static float boomboxLastBeatTime;

	private static float boomboxNetworkDelay;

	private static Vector3 boomboxScaleNetworked = Vector3.one;

	private static int allocatedDonationNukeId = -1;

	private static int wiiRemoteAssetId = -1;

	private static int wiiClickerAssetId = -1;

	private static VRRig wiiSelectedRig;

	private static float wiiMoveDelay;

	private static float wiiUpdateCooldown;

	private static bool lastWiiPrimary;

	private static bool lastWiiTrigger;

	private static int allocatedSwordId = -1;

	private static bool lastSwordVelTooHigh;

	private static float swordSwingDelay;

	private static float swordSlashDelay;

	private static float swordPauseSfx;

	private static int allocatedShrekId = -1;

	private static int allocatedVideoPlayerId = -1;

	public static int videoPlayerIndex;

	private static readonly Dictionary<string, string> VideoPlayerUrls = new Dictionary<string, string>
	{
		{ "Elliot Likes Femboys", "https://files.hamburbur.org/ElliotLikesFemboys.mp4" },
		{ "Dancing Monkeys", "https://github.com/ZlothY29IQ/Mod-Resources/raw/refs/heads/main/monkeys_dancing.mp4" },
		{ "Sky - Carti", "https://github.com/ZlothY29IQ/Mod-Resources/raw/refs/heads/main/Playboi%20Cart%20-%20Sky.mp4" },
		{ "Over - Carti", "https://files.hamburbur.org/Over-PlayboiCarti.mp4" },
		{ "Rendezvous - Don Toliver", "https://files.hamburbur.org/Rendezvous-DonToliver.mp4" },
		{ "wokeuplikethis* - Carti", "https://github.com/ZlothY29IQ/Mod-Resources/raw/refs/heads/main/REmZhFKmOmo.mp4" },
		{ "GPT Mod Menu - SoupVR", "https://files.hamburbur.org/gptmodmenu-soupvr.mp4" },
		{ "Did you pray today?", "https://files.hamburbur.org/didyoupraytoday.mp4" },
		{ "Zimble Mod Checker", "https://files.hamburbur.org/zimblemodchecker.mov" },
		{ "Crazy Russian Guy", "https://files.hamburbur.org/crazyrussianguy.mp4" },
		{ "Tom Holland Moment", "https://files.hamburbur.org/tomhollandmoment.mp4" },
		{ "Im a Korean", "https://files.hamburbur.org/imakorean.mov" },
		{ "ShibaGT Gold Rat", "https://files.hamburbur.org/shibagoldrat.mov" },
		{ "USA Rat", "https://files.hamburbur.org/usamenu.mp4" },
		{ "Press Option 1 Now", "https://files.hamburbur.org/gorilla-tag-gorilla.mp4" },
		{ "Zimble Bad Boy", "https://files.hamburbur.org/zimblebadboy.mp4" },
		{ "Caramell Dansen", "https://files.hamburbur.org/caramelldansen.mp4" },
		{ "How to Protect Your Shopping Trolley", "https://files.hamburbur.org/How%20to%20Protect%20Your%20Shopping%20Trolley%20From%20Improvised%20Explosives.mp4" },
		{ "Theo Does Snacks", "https://files.hamburbur.org/TheoDoesSnacks.mov" },
		{ "ZlothY Locura", "https://files.hamburbur.org/ZlothYLocura.mov" },
		{ "Skidding is a Crime", "https://files.hamburbur.org/SkiddingIsACrime.mp4" },
		{ "Rizz", "https://files.hamburbur.org/rizz.mp4" },
		{ "Shimmy Shimmy ya", "https://files.hamburbur.org/shimmy%20shimmy%20ya%20but%20high%20quality%20(full).mp4" },
		{ "You got me jumping like", "https://files.hamburbur.org/YouGotMeJumpingLike.mov" },
		{ "Guardians of the Galaxy Vol 2", "https://files.hamburbur.org/Guardians%20of%20the%20Galaxy%20Vol.%202%20(2017)%20(Awafim.tv).mp4" },
		{ "Five Nights at Freddy's 2", "https://files.hamburbur.org/FNaF2_UnityReady.mp4" },
		{ "ep 1 rickandmorty", "https://fmovs.online/Items/f91ca0b70d444ed017fe0a86cae12986/Download?api_key=d3da2a6ef25e4bf9953b50c818e1a669" },
		{ "The Amazing Spider-Man", "https://fmovs.online/Items/9732a76ae9cee1cfdedab3f5c9701b41/Download?api_key=586f5aad06d24392a2f24e6976287b5b" },
		{ "South Park", "https://fmovs.online/Items/e40d4c2e1dfbc062d14ca8588acaf4be/Download?api_key=586f5aad06d24392a2f24e6976287b5b" },
		{ "South Park 2", "https://fmovs.online/Items/e5e1e74a1d1c5836a195bc04d796e7fe/Download?api_key=586f5aad06d24392a2f24e6976287b5b" }
	};

	private static readonly List<string> VideoPlayerKeys = VideoPlayerUrls.Keys.ToList();

	private static int allocatedSamsungId = -1;

	private static readonly string[] IPhoneVideoLinks = new string[5] { "https://github.com/ZlothY29IQ/Mod-Resources/raw/refs/heads/main/REmZhFKmOmo.mp4", "https://github.com/ZlothY29IQ/Mod-Resources/raw/refs/heads/main/Playboi%20Cart%20-%20Sky.mp4", "https://github.com/ZlothY29IQ/Mod-Resources/raw/refs/heads/main/monkeys_dancing.mp4", "https://drive.iidk.online/resources/iidk/shiba%20youtube.mp4", "https://github.com/ZlothY29IQ/Mod-Resources/raw/refs/heads/main/hamburger.mp4" };

	private static int allocatedIPhoneId = -1;

	private static int cherryBombAllocatedId = -1;

	private static bool cherryBombThing;

	private static float cherryBombTimeSinceSpawn;

	private static int cheezburgerAssetId = -1;

	private static float cheezburgerNextPlayTime;

	private static int scytheId = -1;

	private static float slashDelaySC;

	private static float pauseSfxSC;

	private static int tvAssetId = -1;

	private static int sofaAssetId = -1;

	private static Vector3 cachedStartPositionArena;

	private static Coroutine arenaPlatRoutine;

	private static int arenaAssetId = -1;

	private static int karambitAssetId = -1;

	private static bool lastVelTooHighK;

	private static float pauseSfxK;

	private static float slashDelayK;

	private static int allocatedBanHammerId = -1;

	private static bool lastVelTooHighBH;

	private static float pauseSfxBH;

	private static float slashDelayBH;

	public static long BanDuration = 300L;

	private static int hamburgerSwordId = -1;

	private static Dictionary<string, int> theEndAssetIds = new Dictionary<string, int>();

	private static string btoolsAnimation = "Grab";

	private static int btoolsId = -1;

	private static float btoolsUpdateCooldown;

	private static Seralyth.Classes.Menu.Console.ConsoleAsset btoolsGrabbingObject;

	private static float btoolsGrabUpdateCooldown;

	private static bool lastGripBtools;

	private static bool lastTriggerBtools;

	private static int btoolsToolId;

	private static float currentFogOpacity;

	private static Coroutine fadeFogCoroutine;

	private static float darkFogOpacity;

	private static Coroutine darkFadeFogCoroutine;

	private static int jailAssetId = -1;

	private static bool jailWasShooting;

	private static readonly List<int> ratAssetIds = new List<int>();

	private static float ratSpawnDelay;

	public static List<int> BurgerIds = new List<int>();

	private static float burgerSpawnDelay;

	private static readonly List<int> AssetGunIds = new List<int>();

	private static float assetGunSpawnDelay;

	private static VRRig assetGunTarget;

	private static bool assetGunLocked;

	private static int astroworldPlanetId = -1;

	public static readonly string[] FlashEffectNames = new string[2] { "Zoom Body Trail", "Ares Body Trail" };

	public static int flashEffectIndex;

	private static int allocatedFlashEffectId = -1;

	private static int allocatedIndustrysGoonengerId;

	public static bool HasIndustrysGoonengerGoonenged = false;

	public static bool hasslash1d = false;

	public static bool hasslash2d = false;

	public static bool boomboxMusicStarted = false;

	public static float boomboxPulseTime = 0f;

	public static int boomboxTrackIndex = 0;

	public static bool boomboxBWasDown = false;

	public static readonly string[] boomboxTrackNames = new string[2] { "Main Menu.mp3", "Raining taco's.mp3" };

	private static int pistolAssetID;

	public static bool pistolFling = false;

	public static bool pistolKick = false;

	private static Vector3 irPosition = Vector3.zero;

	public static float ImageRendererDelay;

	private static int ImageRendererId = -1;

	private static float irScale = 0.3f;

	private static int mctorchID = -1;

	private static int GreysonId = -1;

	private static int allocatedBasketballId = -1;

	private static float basketballHitDelay;

	private static int scaryLarryAssetId = -1;

	private static float scaryLarryFollowSpeed = 2f;

	private static float scaryLarryTouchDistance = 0.7f;

	private static bool scaryLarryHasCrashed = false;

	private static int scaryLarryCurrentTargetActor = -1;

	private static Vector3 scaryLarryCurrentPosition;

	private static int blackstarId = -1;

	private static int allocatedHeavenId = -1;

	private static float timeSinceSpawnHeaven;

	private static bool thingHeaven;

	private static float takeMeUpTimer;

	private static bool hasSpawnedTakeMeUp;

	private static bool hasStartedAnimations;

	private static Coroutine animationCoroutine;

	public static List<string> tiktokVideos = new List<string>
	{
		"https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/#australia #highschool #school #students #funny_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/#bulun_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/#fyp #tiktok #skit #comedy #funny_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/10 October 2025 (1)_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/10 October 2025_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/ACTUAL VIDEO VS BEHIND THE SCENES! - #shorts_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/AI Marketing Tools With No Restrictions_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/African parents be like \ud83d\ude21\ud83d\ude21_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/COMMENT FOR 7 YEARS OF GOOD LUCK! \ud83c\udf40\ud83d\ude05 - #dance #funny #couple #shorts IB@Zarathebanana_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Can you do this (1)_rotated.mp4",
		"https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Can you do this_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/DON'T CHECK SOUND BRO! (1)_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/DON'T CHECK SOUND BRO!_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/DON'T CLICK THE SOUND \ud83d\udc80_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Don't Check The Sound.. ⚠\ufe0f\ud83d\ude1e_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/HOW FAST CAN I INSTALL MODS FOR GORILLA TAG ⁉\ufe0f_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/He found something very cute #shorts_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/His Positive Attitude Brightens Everyone's Day…❤\ufe0f\ud83d\udc4f_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Hopefully we're not TOO strict\ud83d\ude2d\ud83d\udc80 @Prymrr #kanebailey #prymrr #kaneandprymrr_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/How to Fly in Gorilla Tag.. sorta_rotated.mp4",
		"https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/I Bought the CHEAPEST $1 SLIMES! \ud83e\udd11\ud83d\ude31  Unboxing & Haul_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/I Cooked A Pizza With Power Tools_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/I found a secret in Yatagarasu..._rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/I hope she had THE BEST DAY #explore #teacherlife #fyp #teacher_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/It was on beat too \ud83d\ude2d\ud83d\udc80 #basketball_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Just Use game mechanics  brutal \ud83d\ude2d_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Kids can now design their own 3D Games!_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/October 6 2025_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Outsmarted \ud83d\ude02_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Ranking Best Whirlpool Filter Moments_rotated.mp4",
		"https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Ranking the Funniest Useless Car Features \ud83d\ude97\ud83d\ude02_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/She fixes roads now... #shorts #shortsfeed #youtubeshorts #cringe #thecleangirl #comedy #funny_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Spiderman Destroyed Him \ud83d\ude02   The Amazing Spiderman   #shorts_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Stages of 99 Nights in The Forest Players fr #shorts #viral_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Stop saying ✨6 7✨ (1)_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Stop saying ✨6 7✨_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/The Best Drive Thru_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/The MOST CREATIVE Marketing Ever!\ud83e\udd2f\ud83d\udcc8   Milka's Last Square_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/The PERFECT Burger BUN ‼\ufe0f\ud83d\ude02 #TheManniiShow.com series_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/The opposites \ud83e\udd0d #shorts_rotated.mp4",
		"https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/This GRANDPA is an AMAZING gymnast! #interestingfacts (1)_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/This GRANDPA is an AMAZING gymnast! #interestingfacts_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/This Is The LUCKIEST Cat \ud83c\udf40\ud83d\udc08\u200d⬛ #shorts (1)_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Tired Girl Packs Soap Fast_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/WE CAN'T BELIEVE WE JUST HIT 23M FAMILY MEMBERS! \ud83e\udd79\ud83d\ude2d\ud83e\udd70 (1)_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/WE CAN'T BELIEVE WE JUST HIT 23M FAMILY MEMBERS! \ud83e\udd79\ud83d\ude2d\ud83e\udd70_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Watch what happens.. It was a trap \ud83e\udea4 \ud83d\ude05 #viral youtuber #viral #funny_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/Worlds Fastest PITSTOP! (@nocontroleracing)_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/You always Know \ud83d\ude02_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/pov you hand animated a lion in 1 day #blender3d #vfx_rotated.mp4",
		"https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/좋은 것만 주고 싶어\ud83e\udd70_rotated.mp4", "https://github.com/gorillanotaltlol/ytshorts/raw/refs/heads/main/\ud83d\udcf6 HOW TO LAG IN MONKE BLOCKS⁉\ufe0f #gorillatag #vr #gtag #gtagmods #monke_rotated.mp4"
	};

	private static Dictionary<int, int> allocatediPhoneTikTok = new Dictionary<int, int>();

	private static Dictionary<int, int> currentVideoDict = new Dictionary<int, int>();

	private static Dictionary<int, bool> phonePausedDict = new Dictionary<int, bool>();

	private static Dictionary<int, bool> lastTriggerDict = new Dictionary<int, bool>();

	private static Dictionary<int, bool> lastGripDict = new Dictionary<int, bool>();

	private static Dictionary<int, bool> lastPrimaryDict = new Dictionary<int, bool>();

	private static bool tiktokInit = false;

	private static int DiamondSwordid = -1;

	public static string CurrentVideoUrl => VideoPlayerUrls[VideoPlayerKeys[videoPlayerIndex]];

	public static void PLACEHOLDER()
	{
		NotificationManager.SendNotification("<color=grey>[</color><color=yellow>PLACEHOLDER</color><color=grey>]</color> This button does nothing yet.");
	}

	public static void FixDuplicateButtons()
	{
		int num = 0;
		List<string> list = new List<string>();
		ButtonInfo[][] buttons = Buttons.buttons;
		foreach (ButtonInfo[] array in buttons)
		{
			ButtonInfo[] array2 = array;
			foreach (ButtonInfo buttonInfo in array2)
			{
				if (list.Contains(buttonInfo.buttonText))
				{
					string overlapText = buttonInfo.overlapText ?? buttonInfo.buttonText;
					buttonInfo.overlapText = overlapText;
					buttonInfo.buttonText += "X";
					num++;
				}
				list.Add(buttonInfo.buttonText);
			}
		}
		NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully fixed " + num + " broken buttons.");
	}

	public static void BetterFPSBoost()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		Renderer[] array = Resources.FindObjectsOfTypeAll<Renderer>();
		foreach (Renderer val in array)
		{
			try
			{
				if (((Object)val.material.shader).name == "GorillaTag/UberShader")
				{
					oldMats.Add(val, val.material);
					Material material = new Material(Shader.Find("GorillaTag/UberShader"))
					{
						color = val.material.color
					};
					val.material = material;
				}
			}
			catch (Exception ex)
			{
				LogManager.LogError(string.Format("mat error {1} - {0}", ex.Message, ex.StackTrace));
			}
		}
	}

	public static void DisableBetterFPSBoost()
	{
		foreach (KeyValuePair<Renderer, Material> oldMat in oldMats)
		{
			oldMat.Key.material = oldMat.Value;
		}
	}

	public static void OnlySerializeNecessary()
	{
		SerializePatch.OverrideSerialization = delegate
		{
			Main.SendSerialize(VRRig.LocalRig.GetPhotonView());
			return false;
		};
	}

	public static void DumpSoundData()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		string text = "Handtap Sound Data\n(from GorillaLocomotion.GTPlayer.Instance.materialData)";
		int num = 0;
		foreach (MaterialData materialDatum in GTPlayer.Instance.materialData)
		{
			try
			{
				text += "\n====================================\n";
				text = text + num + " ; " + materialDatum.matName + " ; " + materialDatum.slidePercent + "% ; " + (((Object)(object)materialDatum.audio == (Object)null) ? "none" : ((Object)materialDatum.audio).name);
			}
			catch
			{
				LogManager.Log("Failed to log sound");
			}
			num++;
		}
		text += "\n====================================\n";
		text += "Text file generated with MrChicken Menu";
		string text2 = "SeralythMenu/SoundData.txt";
		File.WriteAllText(text2, text);
		string fileName = FileUtilities.GetGamePath() + "/" + text2;
		Process.Start(fileName);
	}

	public static void DumpCosmeticData()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		string text = "Cosmetic Data\n(from CosmeticsController.instance.allCosmetics)";
		foreach (CosmeticItem allCosmetic in ((CosmeticsController)CosmeticsController.instance).allCosmetics)
		{
			try
			{
				text += "\n====================================\n";
				text = text + allCosmetic.itemName + " ; " + allCosmetic.displayName + " (override " + allCosmetic.overrideDisplayName + ") ; " + allCosmetic.cost + "SR ; canTryOn = " + allCosmetic.canTryOn;
			}
			catch
			{
				LogManager.Log("Failed to log hat");
			}
		}
		text += "\n====================================\n";
		text += "Text file generated with MrChicken Menu";
		string text2 = "SeralythMenu/CosmeticData.txt";
		File.WriteAllText(text2, text);
		string fileName = FileUtilities.GetGamePath() + "/" + text2;
		Process.Start(fileName);
	}

	public static void DecryptableCosmeticData()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		string text = "";
		foreach (CosmeticItem allCosmetic in ((CosmeticsController)CosmeticsController.instance).allCosmetics)
		{
			try
			{
				text = text + allCosmetic.itemName + ";;" + allCosmetic.overrideDisplayName + ";;" + allCosmetic.cost + "\n";
			}
			catch
			{
				LogManager.Log("Failed to log hat");
			}
		}
		string text2 = "SeralythMenu/DecryptableCosmeticData.txt";
		File.WriteAllText(text2, text);
		string fileName = FileUtilities.GetGamePath() + "/" + text2;
		Process.Start(fileName);
	}

	public static void DumpRPCData()
	{
		string text = "RPC Data\n(from PhotonNetwork.PhotonServerSettings.RpcList)";
		int num = 0;
		foreach (string rpc in PhotonNetwork.PhotonServerSettings.RpcList)
		{
			try
			{
				text += "\n====================================\n";
				text = text + num + " ; " + rpc;
			}
			catch
			{
				LogManager.Log("Failed to log RPC");
			}
			num++;
		}
		text += "\n====================================\n";
		text += "Text file generated with MrChicken Menu";
		string text2 = "SeralythMenu/RPCData.txt";
		File.WriteAllText(text2, text);
		string fileName = FileUtilities.GetGamePath() + "/" + text2;
		Process.Start(fileName);
	}

	public static void BlankPage()
	{
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = Array.Empty<ButtonInfo>();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void CopyCustomGamemodeScript()
	{
		NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Copied map script to your clipboard.", 5000);
		GUIUtility.systemCopyBuffer = CustomGameMode.LuaScript;
	}

	public static void CopyCustomMapID()
	{
		string text = CustomMapManager.currentRoomMapModId._id.ToString();
		NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> " + text, 5000);
		GUIUtility.systemCopyBuffer = text;
	}

	public static void SafeRestartGame()
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		string path = "SeralythMenu/RestartData.txt";
		switch (restartIndex)
		{
		case 0:
			if (File.Exists(path))
			{
				string text = File.ReadAllText(path);
				restartRoom = text.Split(";")[0];
				List<string> list = text.Split(";")[1].Split(",").ToList();
				restartPosition = new Vector3(float.Parse(list[0]), float.Parse(list[1]), float.Parse(list[2]));
				restartIndex = 3;
			}
			else
			{
				restartRoom = (PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.Name : "");
				restartPosition = ((Component)GTPlayer.Instance).transform.position;
				restartIndex = 1;
			}
			restartDelay = Time.time + 6f;
			break;
		case 1:
			Settings.SavePreferences();
			File.WriteAllText(path, restartRoom + $";{restartPosition.x},{restartPosition.y},{restartPosition.z}");
			restartIndex = 2;
			break;
		case 2:
			if (File.Exists(path) && Time.time > restartDelay)
			{
				Important.RestartGame();
				restartIndex = 4;
			}
			break;
		case 3:
			if (!PhotonNetwork.InRoom && restartRoom != "")
			{
				if (Important.queueCoroutine == null && Time.time > restartDelay)
				{
					Important.QueueRoom(restartRoom);
				}
				break;
			}
			Main.TeleportPlayer(restartPosition);
			File.Delete(path);
			NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Restarted game with information.");
			restartIndex = 4;
			Buttons.GetIndex("Safe Restart Game").enabled = false;
			Settings.SavePreferences();
			break;
		}
	}

	public static void AdminKickGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > adminEventDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				adminEventDelay = Time.time + 0.1f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("kick", (ReceiverGroup)1, RigUtilities.GetPlayerFromVRRig(componentInParent).UserId);
			}
		}
	}

	public static void AdminFemboyGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > adminEventDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				adminEventDelay = Time.time + 0.1f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("sb", (ReceiverGroup)1, RigUtilities.GetPlayerFromVRRig(componentInParent).UserId, "https://files.hamburbur.org/ilikefemboys.mp3");
			}
		}
	}

	public static void AdminPlatToggleGun(bool exclude)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (!Main.GetGunInput(isShooting: true) || !(Time.time > adminEventDelay))
		{
			return;
		}
		VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
		if (!Object.op_Implicit((Object)(object)componentInParent) || componentInParent.IsLocal())
		{
			return;
		}
		string userId = RigUtilities.GetPlayerFromVRRig(componentInParent).UserId;
		adminEventDelay = Time.time + 0.1f;
		if (exclude)
		{
			if (!platExcluded.Contains(userId))
			{
				platExcluded.Add(userId);
				NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Player is now excluded.");
			}
			else
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Player is already excluded!");
			}
		}
		else if (platExcluded.Contains(userId))
		{
			platExcluded.Remove(userId);
			NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Player is now included.");
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Player is already included!");
		}
	}

	public static void TSEF()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		allocatedTSEFId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "travis", "TravisScott", allocatedTSEFId);
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, allocatedTSEFId, (object)new Vector3(-65f, 2f, -55f));
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, allocatedTSEFId, Vector3.one * 0.4f);
		if (isassetsbig)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, allocatedTSEFId, Vector3.one * 3.5f);
		}
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setrotation", (ReceiverGroup)1, allocatedTSEFId, Quaternion.Euler(0f, 20f, 0f));
	}

	public static void UTSEF()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, allocatedTSEFId);
	}

	public static void BigAssets()
	{
		isassetsbig = true;
	}

	public static void NoBigAssets()
	{
		isassetsbig = false;
	}

	public static void Forsaken()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		int freeAssetID = Seralyth.Classes.Menu.Console.GetFreeAssetID();
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "1x", "1x", allocated1xId);
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, allocated1xId, (object)new Vector3(-3.719f, -8.54f, -8.415412f));
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, allocated1xId, Vector3.one * 0.4f);
		if (isassetsbig)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, allocated1xId, Vector3.one * 3.5f);
		}
	}

	public static void UForsaken()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, allocated1xId);
	}

	public static void MiniTravis()
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		allocatedMiniTravisId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "minitravis", "travisscott", allocatedMiniTravisId);
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, allocatedMiniTravisId, 1);
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setlocalposition", (ReceiverGroup)1, allocatedMiniTravisId, (object)new Vector3(-0.6f, 0.2f, 0f));
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setlocalrotation", (ReceiverGroup)1, allocatedMiniTravisId, (object)new Vector3(80f, 160f, 180f));
	}

	public static void UMiniTravis()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, allocatedMiniTravisId);
	}

	public static void RSword()
	{
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		if (allocatedRSwordId < 0)
		{
			allocatedRSwordId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "rbsword", "Sword", allocatedRSwordId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, allocatedRSwordId, 2);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, allocatedRSwordId, "Sword", "Music");
			if (isassetsbig)
			{
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, allocatedRSwordId, Vector3.one * 5f);
			}
			Main.RPCProtection();
		}
		if (!Seralyth.Classes.Menu.Console.consoleAssets.TryGetValue(allocatedRSwordId, out var value))
		{
			return;
		}
		Transform val = value.assetObject.transform.Find("Sword/HitBox");
		RaycastHit val2 = default(RaycastHit);
		Physics.SphereCast(val.position, 0.1f, val.forward, ref val2, 0.7f, Main.NoInvisLayerMask());
		if (Time.time > slashDelay && (Object)(object)((RaycastHit)(ref val2)).collider != (Object)null)
		{
			try
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val2)).collider).GetComponentInParent<VRRig>();
				if ((Object)(object)componentInParent != (Object)null && !componentInParent.isLocal)
				{
					slashDelay = Time.time + 0.5f;
					pauseSfx = Time.time + 1f;
					Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, allocatedRSwordId, "Sword/SFX", $"Slash{Random.Range(1, 3)}");
					Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedRSwordId, "Sword", "Particles");
					NetPlayer creator = componentInParent.Creator;
					Seralyth.Classes.Menu.Console.ExecuteCommand("silkick", creator.ActorNumber, creator.UserId);
				}
			}
			catch
			{
			}
		}
		Vector3 val3 = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0f, false) - GorillaTagger.Instance.rigidbody.linearVelocity;
		bool flag = ((Vector3)(ref val3)).magnitude > 10f;
		if (flag && !lastVelTooHighRS && Time.time > pauseSfx)
		{
			pauseSfx = Time.time + 0.3f;
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, allocatedRSwordId, "Sword/SFX", $"Swing{Random.Range(1, 3)}");
		}
		lastVelTooHighRS = flag;
	}

	public static void URSword()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, allocatedRSwordId);
		allocatedRSwordId = -1;
	}

	public static void AdminKickAll()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("kickall", (ReceiverGroup)1);
	}

	public static void AdminCrashGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > adminEventDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				adminEventDelay = Time.time + 0.1f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("crash", RigUtilities.GetPlayerFromVRRig(componentInParent).ActorNumber);
			}
		}
	}

	public static void AdminCrashAll()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("crash", (ReceiverGroup)0);
	}

	public static void AdminLagSpikeGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > adminEventDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				adminEventDelay = Time.time + 0.5f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("sleep", RigUtilities.GetPlayerFromVRRig(componentInParent).ActorNumber, 1000);
			}
		}
	}

	public static void AdminLagGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && Time.time > adminEventDelay)
			{
				adminEventDelay = Time.time + 0.1f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("sleep", RigUtilities.GetPlayerFromVRRig(Main.lockTarget).ActorNumber, 50);
				Main.RPCProtection();
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
		else
		{
			Main.gunLocked = false;
		}
	}

	public static void AdminLagSpikeAll()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("sleep", (ReceiverGroup)0, 1000);
	}

	public static void AdminLagAll()
	{
		if (Time.time > adminEventDelay)
		{
			adminEventDelay = Time.time + 0.1f;
			Seralyth.Classes.Menu.Console.ExecuteCommand("sleep", (ReceiverGroup)0, 50);
			Main.RPCProtection();
		}
	}

	public static void AdminGiveFlyGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && Time.time > adminEventDelay && ((VRMap)Main.lockTarget.rightThumb).calcT > 0.5f)
			{
				adminEventDelay = Time.time + 0.1f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("vel", RigUtilities.GetPlayerFromVRRig(Main.lockTarget).ActorNumber, Main.lockTarget.headMesh.transform.forward * Movement._flySpeed);
				Main.RPCProtection();
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
		else
		{
			Main.gunLocked = false;
		}
	}

	public static void AdminGivePlatforms()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && Time.time > adminEventDelay)
			{
				if (((VRMap)Main.lockTarget.leftMiddle).calcT > 0.5f && !AdminPlatformsLastLeft)
				{
					adminEventDelay = Time.time + 0.1f;
					Seralyth.Classes.Menu.Console.ExecuteCommand("platf", RigUtilities.GetPlayerFromVRRig(Main.lockTarget).ActorNumber, Main.lockTarget.leftHandTransform.position - new Vector3(0f, 0.2f, 0f), (object)new Vector3(0.1f, 0.5f, 0.3f), Main.lockTarget.leftHandTransform.eulerAngles, Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f, 10f);
					Main.RPCProtection();
				}
				if (((VRMap)Main.lockTarget.rightMiddle).calcT > 0.5f && !AdminPlatformsLastRight)
				{
					adminEventDelay = Time.time + 0.1f;
					Seralyth.Classes.Menu.Console.ExecuteCommand("platf", RigUtilities.GetPlayerFromVRRig(Main.lockTarget).ActorNumber, Main.lockTarget.rightHandTransform.position - new Vector3(0f, 0.2f, 0f), (object)new Vector3(0.1f, 0.5f, 0.3f), Main.lockTarget.rightHandTransform.eulerAngles, Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f, 10f);
					Main.RPCProtection();
				}
				AdminPlatformsLastLeft = ((VRMap)Main.lockTarget.leftMiddle).calcT > 0.5f;
				AdminPlatformsLastRight = ((VRMap)Main.lockTarget.rightMiddle).calcT > 0.5f;
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
		else
		{
			Main.gunLocked = false;
		}
	}

	public static void AdminGiveTriggerFlyGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && Time.time > adminEventDelay && ((VRMap)Main.lockTarget.rightIndex).calcT > 0.5f)
			{
				adminEventDelay = Time.time + 0.1f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("vel", RigUtilities.GetPlayerFromVRRig(Main.lockTarget).ActorNumber, Main.lockTarget.headMesh.transform.forward * Movement._flySpeed);
				Main.RPCProtection();
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
		else
		{
			Main.gunLocked = false;
		}
	}

	public static void AdminGiveSpeedGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && Time.time > adminEventDelay)
			{
				adminEventDelay = Time.time + 0.2f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("vel", RigUtilities.GetPlayerFromVRRig(Main.lockTarget).ActorNumber, (Main.lockTarget.bodyTransform.position - speedLastVel) * 6f);
				speedLastVel = Main.lockTarget.bodyTransform.position;
				Main.RPCProtection();
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					speedLastVel = componentInParent.bodyTransform.position;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else
		{
			Main.gunLocked = false;
		}
	}

	public static void AdminGiveLowGravity()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && Time.time > adminEventDelay)
			{
				adminEventDelay = Time.time + 0.2f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("vel", RigUtilities.GetPlayerFromVRRig(Main.lockTarget).ActorNumber, (Main.lockTarget.bodyTransform.position - speedLastVel) * 5f + Vector3.up * 0.5f);
				speedLastVel = Main.lockTarget.bodyTransform.position;
				Main.RPCProtection();
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					speedLastVel = componentInParent.bodyTransform.position;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else
		{
			Main.gunLocked = false;
		}
	}

	public static void AdminVibrateGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > adminEventDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				adminEventDelay = Time.time + 0.2f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("vibrate", RigUtilities.GetPlayerFromVRRig(componentInParent).ActorNumber, 3, 1f);
			}
		}
	}

	public static void AdminVibrateAll()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("vibrate", (ReceiverGroup)0, 3, 1f);
	}

	public static void AdminBMuteGun(bool mute)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > adminEventDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				adminEventDelay = Time.time + 0.5f;
				Seralyth.Classes.Menu.Console.ExecuteCommand(mute ? "mute" : "unmute", (ReceiverGroup)1, RigUtilities.GetPlayerFromVRRig(componentInParent).UserId);
			}
		}
	}

	public static void AdminBlockGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > adminEventDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				adminEventDelay = Time.time + 5f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("block", RigUtilities.GetPlayerFromVRRig(componentInParent).ActorNumber, 300L);
			}
		}
	}

	public static void AdminABlockGun(bool Silent)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > adminEventDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				adminEventDelay = Time.time + 5f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("notify", (ReceiverGroup)1, RigUtilities.GetPlayerFromVRRig(componentInParent).NickName + " has been blocked" + (Silent ? "" : (" by " + ServerData.Administrators[PhotonNetwork.LocalPlayer.UserId])) + ".");
				Seralyth.Classes.Menu.Console.ExecuteCommand("block", RigUtilities.GetPlayerFromVRRig(componentInParent).ActorNumber, 300L);
				Main.RPCProtection();
			}
		}
	}

	public static void AdminBMuteAll(bool mute)
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand(mute ? "muteall" : "unmuteall", (ReceiverGroup)1);
	}

	public static void AdminButtonPressGun(string key)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > adminEventDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				adminEventDelay = Time.time + 0.8f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("controller", RigUtilities.GetPlayerFromVRRig(componentInParent).ActorNumber, key, 1f, 1f);
				Main.RPCProtection();
			}
		}
	}

	public static void FlipMenuGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > adminEventDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				adminEventDelay = Time.time + 0.1f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("toggle", RigUtilities.GetPlayerFromVRRig(componentInParent).ActorNumber, "Right Hand");
			}
		}
	}

	public static void AdminEnableGun(bool enable, string mod)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > adminEventDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				adminEventDelay = Time.time + 0.1f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("forceenable", RigUtilities.GetPlayerFromVRRig(componentInParent).ActorNumber, mod, enable);
			}
		}
	}

	public static void AdminJumpscareGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > jumpscareDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				jumpscareDelay = Time.time + 0.2f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("toggle", RigUtilities.GetPlayerFromVRRig(componentInParent).ActorNumber, "Jumpscare");
			}
		}
	}

	public static void AdminJumpscareAll()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("toggle", (ReceiverGroup)0, "Jumpscare");
	}

	public static void AdminMute()
	{
		if (Main.leftTrigger > 0.5f && !muted)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("forceenable", (ReceiverGroup)0, "Mute Microphone", true);
			muted = true;
		}
		else if (Main.leftTrigger < 0.5f && muted)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("forceenable", (ReceiverGroup)0, "Mute Microphone", false);
			muted = false;
		}
	}

	private static IEnumerator FreezeCoroutine(VRRig rig)
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("forceenable", RigUtilities.GetPlayerFromVRRig(rig).ActorNumber, "Zero Gravity", true);
		Vector3 pos = ((Component)rig).transform.position;
		while (VRRigCache.ActiveRigs.Contains(rig))
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("tp", RigUtilities.GetPlayerFromVRRig(rig).ActorNumber, pos);
			yield return (object)new WaitForSeconds(0.1f);
		}
	}

	public static void AdminFreezeGun(bool freeze)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (!Main.GetGunInput(isShooting: true) || !(Time.time > adminEventDelay))
		{
			return;
		}
		VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
		if (!Object.op_Implicit((Object)(object)componentInParent) || componentInParent.IsLocal())
		{
			return;
		}
		adminEventDelay = Time.time + 0.1f;
		if (freeze)
		{
			if (!freezePool.ContainsKey(componentInParent))
			{
				freezePool.Add(componentInParent, ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(FreezeCoroutine(componentInParent)));
			}
		}
		else if (freezePool.ContainsKey(componentInParent))
		{
			((MonoBehaviour)CoroutineManager.instance).StopCoroutine(freezePool[componentInParent]);
			Seralyth.Classes.Menu.Console.ExecuteCommand("forceenable", RigUtilities.GetPlayerFromVRRig(componentInParent).ActorNumber, "Zero Gravity", false);
			freezePool.Remove(componentInParent);
		}
	}

	public static void AdminTeleportGun()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true) && Time.time > adminEventDelay)
			{
				adminEventDelay = Time.time + 0.1f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("tp", (ReceiverGroup)0, item.transform.position);
			}
		}
	}

	public static void AdminFlingGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > adminEventDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				adminEventDelay = Time.time + 0.1f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("vel", RigUtilities.GetPlayerFromVRRig(componentInParent).ActorNumber, (object)new Vector3(0f, 50f, 0f));
			}
		}
	}

	public static void AdminCrashBypassGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > adminEventDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal() && !ServerData.Administrators.ContainsKey(RigUtilities.GetPlayerFromVRRig(componentInParent).UserId))
			{
				adminEventDelay = Time.time + 0.1f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("tp", RigUtilities.GetPlayerFromVRRig(componentInParent).ActorNumber, (object)new Vector3(0f, 1000000f, 0f));
			}
		}
	}

	public static void AdminLockdownGun(bool enable)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > adminEventDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				adminEventDelay = Time.time + 0.1f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("togglemenu", RigUtilities.GetPlayerFromVRRig(componentInParent).ActorNumber, enable);
			}
		}
	}

	public static void FullToggleMenu(int actorNumber, bool enable)
	{
		if (enable)
		{
			if (!FullActorNumbers.Contains(actorNumber))
			{
				Seralyth.Classes.Menu.Console.ExecuteCommand("forceenable", actorNumber, "Disable Autosave", true);
				Seralyth.Classes.Menu.Console.ExecuteCommand("forceenable", actorNumber, "Load Preferences");
				FullActorNumbers.Add(actorNumber);
			}
		}
		else if (FullActorNumbers.Contains(actorNumber))
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("toggle", actorNumber, "Save Preferences");
			Seralyth.Classes.Menu.Console.ExecuteCommand("forceenable", actorNumber, "Disable Autosave", true);
			Seralyth.Classes.Menu.Console.ExecuteCommand("forceenable", actorNumber, "Panic", true);
			FullActorNumbers.Remove(actorNumber);
		}
		Seralyth.Classes.Menu.Console.ExecuteCommand("togglemenu", actorNumber, enable);
	}

	public static void AdminFullLockdownGun(bool enable)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > adminEventDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				adminEventDelay = Time.time + 0.1f;
				FullToggleMenu(RigUtilities.GetPlayerFromVRRig(componentInParent).ActorNumber, enable);
			}
		}
	}

	public static void AdminLockdownAll(bool enable)
	{
		if (PhotonNetwork.InRoom && (!lastInRoom2 || PhotonNetwork.PlayerList.Length != lastPlayerCount2))
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("togglemenu", (ReceiverGroup)0, enable);
		}
		lastInRoom2 = PhotonNetwork.InRoom;
		lastPlayerCount2 = PhotonNetwork.PlayerList.Length;
		if (!PhotonNetwork.InRoom)
		{
			lastPlayerCount2 = -1;
		}
	}

	public static void AdminFullLockdownAll(bool enable)
	{
		NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
		foreach (NetPlayer val in playerListOthers)
		{
			FullToggleMenu(val.ActorNumber, enable);
		}
	}

	public static void AdminStrangle()
	{
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		if (Main.leftGrab)
		{
			if ((Object)(object)thestrangledleft == (Object)null)
			{
				foreach (VRRig item in from rig in VRRigCache.ActiveRigs
					where !rig.isLocal
					where Vector3.Distance(rig.headMesh.transform.position, GorillaTagger.Instance.leftHandTransform.position) < 0.2f
					select rig)
				{
					thestrangledleft = item;
					if (PhotonNetwork.InRoom)
					{
						GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", (RpcTarget)0, new object[3] { 89, true, 999999f });
					}
					else
					{
						VRRig.LocalRig.PlayHandTapLocal(89, true, 999999f);
					}
				}
			}
			else if (Time.time > stdell)
			{
				stdell = Time.time + 0.05f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("tp", RigUtilities.GetPlayerFromVRRig(thestrangledleft).ActorNumber, GorillaTagger.Instance.leftHandTransform.position);
			}
		}
		else if ((Object)(object)thestrangledleft != (Object)null)
		{
			try
			{
				Seralyth.Classes.Menu.Console.ExecuteCommand("tp", RigUtilities.GetPlayerFromVRRig(thestrangledleft).ActorNumber, GorillaTagger.Instance.leftHandTransform.position);
				Seralyth.Classes.Menu.Console.ExecuteCommand("vel", RigUtilities.GetPlayerFromVRRig(thestrangledleft).ActorNumber, GTPlayer.Instance.LeftHand.velocityTracker.GetAverageVelocity(true, 0f, false));
			}
			catch
			{
			}
			thestrangledleft = null;
			if (PhotonNetwork.InRoom)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", (RpcTarget)0, new object[3] { 89, true, 999999f });
			}
			else
			{
				VRRig.LocalRig.PlayHandTapLocal(89, true, 999999f);
			}
		}
		if (Main.rightGrab)
		{
			if ((Object)(object)thestrangled == (Object)null)
			{
				foreach (VRRig item2 in from rig in VRRigCache.ActiveRigs
					where !rig.isLocal
					where Vector3.Distance(rig.headMesh.transform.position, GorillaTagger.Instance.rightHandTransform.position) < 0.2f
					select rig)
				{
					thestrangled = item2;
					if (PhotonNetwork.InRoom)
					{
						GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", (RpcTarget)0, new object[3] { 89, false, 999999f });
					}
					else
					{
						VRRig.LocalRig.PlayHandTapLocal(89, false, 999999f);
					}
				}
				return;
			}
			if (Time.time > adminEventDelay)
			{
				adminEventDelay = Time.time + 0.05f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("tp", RigUtilities.GetPlayerFromVRRig(thestrangled).ActorNumber, GorillaTagger.Instance.rightHandTransform.position);
			}
		}
		else if ((Object)(object)thestrangled != (Object)null)
		{
			try
			{
				Seralyth.Classes.Menu.Console.ExecuteCommand("tp", RigUtilities.GetPlayerFromVRRig(thestrangled).ActorNumber, GorillaTagger.Instance.rightHandTransform.position);
				Seralyth.Classes.Menu.Console.ExecuteCommand("vel", RigUtilities.GetPlayerFromVRRig(thestrangled).ActorNumber, GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0f, false));
			}
			catch
			{
			}
			thestrangled = null;
			if (PhotonNetwork.InRoom)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", (RpcTarget)0, new object[3] { 89, false, 999999f });
			}
			else
			{
				VRRig.LocalRig.PlayHandTapLocal(89, false, 999999f);
			}
		}
	}

	public static void AdminObjectGun()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true) && Time.time > adminEventDelay)
			{
				adminEventDelay = Time.time + 0.1f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, item.transform.position);
			}
		}
	}

	public static void AdminRandomObjectGun()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true) && Time.time > adminEventDelay)
			{
				adminEventDelay = Time.time + 0.1f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, item.transform.position, RandomUtilities.RandomVector3(), RandomUtilities.RandomVector3(360f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
			}
		}
	}

	public static void AdminNetworkScale()
	{
		if (Time.time > scalenetdel && (!Mathf.Approximately(lastnetscale, VRRig.LocalRig.scaleFactor) || PhotonNetwork.PlayerList.Length != lastplayercount))
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("scale", (ReceiverGroup)1, VRRig.LocalRig.scaleFactor);
			scalenetdel = Time.time + 0.05f;
			lastnetscale = VRRig.LocalRig.scaleFactor;
			lastplayercount = PhotonNetwork.PlayerList.Length;
		}
	}

	public static void UnAdminNetworkScale()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("scale", (ReceiverGroup)1, 1f);
	}

	public static void LightningGun()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true) && Time.time > adminEventDelay)
			{
				adminEventDelay = Time.time + 0.1f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("strike", (ReceiverGroup)1, item.transform.position);
			}
		}
	}

	public static void LightningAura()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > adminEventDelay)
		{
			adminEventDelay = Time.time + 0.05f;
			Seralyth.Classes.Menu.Console.ExecuteCommand("strike", (ReceiverGroup)1, ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(MathF.Cos((float)Time.frameCount / 30f), 1f, MathF.Sin((float)Time.frameCount / 30f)));
		}
	}

	public static void LightningRain()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > adminEventDelay)
		{
			adminEventDelay = Time.time + 0.1f;
			RaycastHit val = default(RaycastHit);
			Physics.Raycast(((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(Random.Range(-10f, 10f), 10f, Random.Range(-10f, 10f)), Vector3.down, ref val, 512f, Main.NoInvisLayerMask());
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				adminEventDelay = Time.time + 0.1f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("kick", (ReceiverGroup)1, RigUtilities.GetPlayerFromVRRig(componentInParent).UserId);
			}
			else
			{
				Seralyth.Classes.Menu.Console.ExecuteCommand("strike", (ReceiverGroup)1, ((RaycastHit)(ref val)).point);
			}
		}
	}

	public static void AdminFearGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Main.TeleportPlayer(((Component)Main.lockTarget).transform.position + ((Component)Main.lockTarget).transform.forward);
				if (Time.time > adminEventDelay)
				{
					adminEventDelay = Time.time + 0.1f;
				}
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					originalMePosition = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
					whereOriginalPlayerPos = ((Component)componentInParent).transform.position;
					int actorNumber = RigUtilities.GetPlayerFromVRRig(componentInParent).ActorNumber;
					Seralyth.Classes.Menu.Console.ExecuteCommand("platf", new int[2]
					{
						actorNumber,
						PhotonNetwork.LocalPlayer.ActorNumber
					}, (object)new Vector3(0f, 16f, 0f), (object)new Vector3(10f, 1f, 10f));
					Seralyth.Classes.Menu.Console.ExecuteCommand("platf", new int[2]
					{
						actorNumber,
						PhotonNetwork.LocalPlayer.ActorNumber
					}, (object)new Vector3(0f, 24f, 0f), (object)new Vector3(10f, 1f, 10f));
					Seralyth.Classes.Menu.Console.ExecuteCommand("platf", new int[2]
					{
						actorNumber,
						PhotonNetwork.LocalPlayer.ActorNumber
					}, (object)new Vector3(4f, 20f, 0f), (object)new Vector3(1f, 10f, 10f));
					Seralyth.Classes.Menu.Console.ExecuteCommand("platf", new int[2]
					{
						actorNumber,
						PhotonNetwork.LocalPlayer.ActorNumber
					}, (object)new Vector3(-4f, 20f, 0f), (object)new Vector3(1f, 10f, 10f));
					Seralyth.Classes.Menu.Console.ExecuteCommand("platf", new int[2]
					{
						actorNumber,
						PhotonNetwork.LocalPlayer.ActorNumber
					}, (object)new Vector3(0f, 20f, 4f), (object)new Vector3(10f, 10f, 1f));
					Seralyth.Classes.Menu.Console.ExecuteCommand("platf", new int[2]
					{
						actorNumber,
						PhotonNetwork.LocalPlayer.ActorNumber
					}, (object)new Vector3(0f, 20f, -4f), (object)new Vector3(10f, 10f, 1f));
					GameObject val2 = GameObject.CreatePrimitive((PrimitiveType)3);
					Object.Destroy((Object)(object)val2, 60f);
					val2.GetComponent<Renderer>().material.color = Color.black;
					val2.transform.position = new Vector3(0f, 20f, 0f);
					val2.transform.localScale = new Vector3(10f, 1f, 10f);
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
			Main.TeleportPlayer(originalMePosition);
			Seralyth.Classes.Menu.Console.ExecuteCommand("tpnv", RigUtilities.GetPlayerFromVRRig(Main.lockTarget).ActorNumber, whereOriginalPlayerPos);
			Seralyth.Classes.Menu.Console.ExecuteCommand("unmuteall", RigUtilities.GetPlayerFromVRRig(Main.lockTarget).ActorNumber);
		}
	}

	public static void EnableNoAdminIndicator()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("nocone", (ReceiverGroup)1, true);
		lastplayercount = -1;
	}

	public static void NoAdminIndicator()
	{
		if (!PhotonNetwork.InRoom)
		{
			lastplayercount = -1;
		}
		if (PhotonNetwork.PlayerList.Length != lastplayercount && PhotonNetwork.InRoom)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("nocone", (ReceiverGroup)1, true);
			lastplayercount = PhotonNetwork.PlayerList.Length;
		}
	}

	public static void AdminIndicatorBack()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("nocone", (ReceiverGroup)1, false);
	}

	public static void EnableAdminMenuUserTags()
	{
		if (!userTagHooked)
		{
			userTagHooked = true;
			PhotonNetwork.NetworkingClient.EventReceived += AdminUserTagSys;
		}
	}

	public static void AdminUserTagSys(EventData data)
	{
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Player player = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false);
			if (data.Code != 68 || player == PhotonNetwork.LocalPlayer)
			{
				return;
			}
			object[] array = (object[])data.CustomData;
			string text = (string)array[0];
			string text2 = text;
			string text3 = text2;
			if (!(text3 == "confirmusing"))
			{
				return;
			}
			if (Buttons.GetIndex("Menu User Name Tags").enabled && ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
			{
				VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(NetPlayer.op_Implicit(player));
				if (!nametags.TryGetValue(vRRigFromPlayer, out var value))
				{
					GameObject val = new GameObject("Seralyth_MenuUserNametag");
					val.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
					TextMeshPro val2 = val.AddComponent<TextMeshPro>();
					((TMP_Text)val2).fontSize = 4.8f;
					((TMP_Text)val2).alignment = (TextAlignmentOptions)514;
					Color color = Color.red;
					if (array.Length > 2)
					{
						color = Seralyth.Classes.Menu.Console.GetMenuTypeName((string)array[2]);
					}
					((Graphic)val2).color = color;
					((TMP_Text)val2).text = Main.ToTitleCase((string)array[2]);
					nametags.Add(vRRigFromPlayer, val);
				}
				else
				{
					TextMeshPro component = value.GetComponent<TextMeshPro>();
					Color color2 = Color.red;
					if (array.Length > 2)
					{
						color2 = Seralyth.Classes.Menu.Console.GetMenuTypeName((string)array[2]);
					}
					if (Visuals.nameTagChams)
					{
						((TMP_Text)(object)component).Chams();
					}
					((Graphic)component).color = color2;
					((TMP_Text)component).text = Main.ToTitleCase((string)array[2]);
				}
			}
			if (Buttons.GetIndex("Conduct Menu Users").enabled && !onConduct.ContainsKey(player.UserId))
			{
				bool flag = ServerData.Administrators.ContainsKey(player.UserId);
				string text4 = player.NickName + " - " + Main.ToTitleCase((string)array[2]);
				if (flag)
				{
					text4 = "<color=red>" + text4 + "</color>";
				}
				onConduct.Add(player.UserId, text4);
			}
			if (Buttons.GetIndex("Admin Find User").enabled)
			{
				isUserFound = true;
			}
		}
		catch
		{
		}
	}

	public static void AdminMenuUserTags()
	{
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		if (PhotonNetwork.InRoom && (!lastInRoom || PhotonNetwork.PlayerList.Length != lastPlayerCount))
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("isusing", (ReceiverGroup)1);
		}
		lastInRoom = PhotonNetwork.InRoom;
		lastPlayerCount = PhotonNetwork.PlayerList.Length;
		if (!PhotonNetwork.InRoom)
		{
			lastPlayerCount = -1;
		}
		foreach (KeyValuePair<VRRig, GameObject> item in nametags.ToList())
		{
			if (!VRRigCache.ActiveRigs.Contains(item.Key))
			{
				Object.Destroy((Object)(object)item.Value);
				nametags.Remove(item.Key);
				continue;
			}
			((TMP_Text)item.Value.GetComponent<TextMeshPro>()).fontStyle = Main.activeFontStyle;
			((TMP_Text)item.Value.GetComponent<TextMeshPro>()).font = Main.activeFont;
			if (Visuals.nameTagChams)
			{
				((TMP_Text)(object)item.Value.GetComponent<TextMeshPro>()).Chams();
			}
			item.Value.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f) * item.Key.scaleFactor;
			item.Value.transform.position = Visuals.GetNameTagPosition(item.Key);
			item.Value.transform.LookAt(((Component)Camera.main).transform.position);
			item.Value.transform.Rotate(0f, 180f, 0f);
		}
	}

	public static void DisableAdminMenuUserTags()
	{
		foreach (KeyValuePair<VRRig, GameObject> nametag in nametags)
		{
			Object.Destroy((Object)(object)nametag.Value);
		}
		nametags.Clear();
	}

	public static void EnableAdminMenuUserTracers()
	{
		if (!tracerTagHooked)
		{
			tracerTagHooked = true;
			PhotonNetwork.NetworkingClient.EventReceived += AdminTracerSys;
		}
	}

	public static void AdminTracerSys(EventData data)
	{
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Player player = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false);
			if (data.Code != 68 || player == PhotonNetwork.LocalPlayer)
			{
				return;
			}
			object[] array = (object[])data.CustomData;
			string text = (string)array[0];
			string text2 = text;
			string text3 = text2;
			if (!(text3 == "confirmusing") || !ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
			{
				return;
			}
			VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(NetPlayer.op_Implicit(player));
			if (!nametags.TryGetValue(vRRigFromPlayer, out var value))
			{
				GameObject val = new GameObject("Seralyth_Nametag");
				val.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
				TextMeshPro val2 = val.AddComponent<TextMeshPro>();
				((TMP_Text)val2).fontSize = 48f;
				((TMP_Text)val2).alignment = (TextAlignmentOptions)514;
				Color color = Color.red;
				if (array.Length > 2)
				{
					color = Seralyth.Classes.Menu.Console.GetMenuTypeName((string)array[2]);
				}
				((Graphic)val2).color = color;
				((TMP_Text)val2).text = Main.ToTitleCase((string)array[2]);
				nametags.Add(vRRigFromPlayer, val);
			}
			else
			{
				TextMeshPro component = value.GetComponent<TextMeshPro>();
				Color color2 = Color.red;
				if (array.Length > 2)
				{
					color2 = Seralyth.Classes.Menu.Console.GetMenuTypeName((string)array[2]);
				}
				((Graphic)component).color = color2;
				((TMP_Text)component).text = Main.ToTitleCase((string)array[2]);
			}
		}
		catch
		{
		}
	}

	public static void MenuUserTracers()
	{
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		if (PhotonNetwork.InRoom && (!lastInRoom || PhotonNetwork.PlayerList.Length != lastPlayerCount))
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("isusing", (ReceiverGroup)1);
		}
		lastInRoom = PhotonNetwork.InRoom;
		lastPlayerCount = PhotonNetwork.PlayerList.Length;
		if (!PhotonNetwork.InRoom)
		{
			lastPlayerCount = -1;
		}
		if (Visuals.DoPerformanceCheck())
		{
			return;
		}
		bool enabled = Buttons.GetIndex("Follow Menu Theme").enabled;
		bool enabled2 = Buttons.GetIndex("Transparent Theme").enabled;
		_ = Buttons.GetIndex("Hidden on Camera").enabled;
		float num = (Buttons.GetIndex("Thin Tracers").enabled ? 0.0075f : 0.025f) * (Main.scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
		Color currentColor = Main.backgroundColor.GetCurrentColor();
		foreach (KeyValuePair<VRRig, string> menuUser in menuUsers)
		{
			VRRig key = menuUser.Key;
			if (!key.isLocal)
			{
				Color val = Seralyth.Classes.Menu.Console.GetMenuTypeName(menuUser.Value);
				LineRenderer lineRender = Visuals.GetLineRender();
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
				lineRender.SetPosition(1, ((Component)key).transform.position);
			}
		}
	}

	public static void ConsoleOnConduct()
	{
		if (PhotonNetwork.InRoom && (!lastInRoom || PhotonNetwork.PlayerList.Length != lastPlayerCount) && !Buttons.GetIndex("Menu User Name Tags").enabled)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("isusing", (ReceiverGroup)1);
		}
		string text = "";
		text = text + "<color=red>" + PhotonNetwork.LocalPlayer.NickName + " - " + Main.ToTitleCase(Seralyth.Classes.Menu.Console.MenuName) + "</color>\\n";
		foreach (KeyValuePair<string, string> item in onConduct)
		{
			if (RigUtilities.GetPlayerFromID(item.Key) == null)
			{
				onConduct.Remove(item.Key);
			}
			else
			{
				text = text + item.Value + "\\n";
			}
		}
		((TMP_Text)Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData").GetComponent<TextMeshPro>()).text = text;
	}

	public static void AdminFindUser()
	{
		if (!(Time.time < FindUserTime))
		{
			if (!PhotonNetwork.InRoom)
			{
				Important.JoinRandom();
				isUserFound = false;
				FindUserTime = Time.time + 7f;
			}
			else if (isUserFound)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Found menu user!");
				Buttons.GetIndex("Admin Find User").enabled = false;
				isUserFound = false;
			}
			else
			{
				NotificationManager.SendNotification("Nobody found, searching for players.");
				NetworkSystem.Instance.ReturnToSinglePlayer();
				FindUserTime = Time.time + 2f;
			}
		}
	}

	public static void AdminPunchMod()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		if (!(Time.time > thingdeb))
		{
			return;
		}
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			bool flag = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, activeRig.headMesh.transform.position) < 0.25f;
			bool flag2 = Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, activeRig.headMesh.transform.position) < 0.25f;
			if (!activeRig.isLocal && (flag || flag2))
			{
				Vector3 val = (flag2 ? GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0f, false) : GTPlayer.Instance.LeftHand.velocityTracker.GetAverageVelocity(true, 0f, false));
				Seralyth.Classes.Menu.Console.ExecuteCommand("vel", RigUtilities.GetPlayerFromVRRig(activeRig).ActorNumber, val);
				thingdeb = Time.time + 0.1f;
			}
		}
	}

	public static void GetTargetRoom()
	{
		Main.PromptText("What room would you like the users to join?", delegate
		{
			targetRoom = Main.keyboardInput;
		}, null, "Done", "Cancel");
	}

	public static void JoinGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > adminEventDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				adminEventDelay = Time.time + 0.1f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("join", RigUtilities.GetPlayerFromVRRig(componentInParent).ActorNumber, targetRoom.ToUpper());
			}
		}
	}

	public static void JoinAll()
	{
		Main.PromptText("What room would you like the users to join?", delegate
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("join", (ReceiverGroup)0, Main.keyboardInput.ToUpper());
		}, null, "Done", "Cancel");
	}

	public static void GetTargetNotification()
	{
		Main.PromptText("What notification would you like to send?", delegate
		{
			targetNotification = Main.keyboardInput;
			Buttons.GetIndex("NotifLabel").overlapText = "Notif: " + Main.keyboardInput;
		}, null, "Done", "Cancel");
	}

	public static void NotifySelf()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("notify", PhotonNetwork.LocalPlayer.ActorNumber, targetNotification);
	}

	public static void NotifyGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > adminEventDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				adminEventDelay = Time.time + 0.1f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("notify", RigUtilities.GetPlayerFromVRRig(componentInParent).ActorNumber, targetNotification);
			}
		}
	}

	public static void NotifyAll()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("notify", (ReceiverGroup)1, targetNotification);
	}

	public static void GetMenuUsers()
	{
		Seralyth.Classes.Menu.Console.indicatorDelay = Time.time + 2f;
		Seralyth.Classes.Menu.Console.ExecuteCommand("isusing", (ReceiverGroup)1);
	}

	public static void AdminLaser()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if (Main.leftPrimary || Main.rightPrimary)
		{
			Vector3 val = (Main.rightPrimary ? VRRig.LocalRig.rightHandTransform.right : (-VRRig.LocalRig.leftHandTransform.right));
			Vector3 val2 = (Main.rightPrimary ? VRRig.LocalRig.rightHandTransform.position : VRRig.LocalRig.leftHandTransform.position) + val * 0.1f;
			try
			{
				RaycastHit val3 = default(RaycastHit);
				Physics.Raycast(val2 + val / 3f, val, ref val3, 512f, Main.NoInvisLayerMask());
				VRRig componentInParent = ((Component)((RaycastHit)(ref val3)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Seralyth.Classes.Menu.Console.ExecuteCommand("silkick", (ReceiverGroup)1, RigUtilities.GetPlayerFromVRRig(componentInParent).UserId);
				}
			}
			catch
			{
			}
			if (Time.time > adminEventDelay)
			{
				adminEventDelay = Time.time + 0.1f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("laser", (ReceiverGroup)1, true, Main.rightPrimary);
			}
		}
		bool flag = Main.leftPrimary || Main.rightPrimary;
		if (lastLasering && !flag)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("laser", (ReceiverGroup)1, false, false);
		}
		lastLasering = flag;
	}

	public static void AdminBeam()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightTrigger > 0.5f && Time.time > beamDelay)
		{
			beamDelay = Time.time + 0.05f;
			float num = (float)Time.frameCount / 180f % 1f;
			Color val = Color.HSVToRGB(num, 1f, 1f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("lr", (ReceiverGroup)1, val.r, val.g, val.b, val.a, 0.5f, ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(0f, 0.5f, 0f), ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(Mathf.Cos((float)Time.frameCount / 30f) * 100f, 0.5f, Mathf.Sin((float)Time.frameCount / 30f) * 100f), 0.1f);
		}
	}

	public static void AdminFractals()
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightTrigger > 0.5f && !lastTriggerLaserSpam)
		{
			startTimeTrigger = Time.time;
		}
		lastTriggerLaserSpam = Main.rightTrigger > 0.5f;
		if (Main.rightTrigger > 0.5f && Time.time > beamDelay)
		{
			beamDelay = Time.time + 0.5f;
			float num = (float)Time.frameCount / 180f % 1f;
			Color.HSVToRGB(num, 1f, 1f);
			object[] obj = new object[9]
			{
				"lr",
				0f,
				1f,
				1f,
				0.3f,
				0.25f,
				((Component)GorillaTagger.Instance.bodyCollider).transform.position,
				null,
				null
			};
			Vector3 position = ((Component)GorillaTagger.Instance.headCollider).transform.position;
			Vector3 val = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
			obj[7] = position + ((Vector3)(ref val)).normalized * 1000f;
			obj[8] = 20f - (Time.time - startTimeTrigger);
			Seralyth.Classes.Menu.Console.ExecuteCommand("lr", (ReceiverGroup)1, obj);
		}
	}

	public static void FlyAllUsing()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > adminEventDelay)
		{
			adminEventDelay = Time.time + 0.05f;
			Seralyth.Classes.Menu.Console.ExecuteCommand("vel", (ReceiverGroup)0, (object)new Vector3(0f, 10f, 0f));
		}
	}

	public static void BouncyAllUsing()
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		if (!(Time.time > adminEventDelay))
		{
			return;
		}
		adminEventDelay = Time.time + 0.05f;
		List<Player> source = Seralyth.Classes.Menu.Console.userDictionary.Keys.Where((Player u) => !u.IsLocal).ToList();
		RaycastHit val = default(RaycastHit);
		foreach (VRRig item in source.Select((Player player) => RigUtilities.GetVRRigFromPlayer(NetPlayer.op_Implicit(player))))
		{
			if (Physics.Raycast(item.bodyTransform.position - new Vector3(0f, 0.2f, 0f), Vector3.down, ref val, 512f, LayerMask.op_Implicit(GTPlayer.Instance.locomotionEnabledLayers)) && ((RaycastHit)(ref val)).distance < 0.1f)
			{
				Vector3 normal = ((RaycastHit)(ref val)).normal;
				Vector3 val2 = item.LatestVelocity();
				Vector3 val3 = Vector3.Reflect(val2, normal);
				Vector3 val4 = val3 * 2f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("vel", item.GetPlayer().ActorNumber, val4);
			}
		}
	}

	public static void AdminBringGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > adminEventDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				adminEventDelay = Time.time + 0.1f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("tpnv", RigUtilities.GetPlayerFromVRRig(componentInParent).ActorNumber, ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(0f, 1.5f, 0f));
			}
		}
	}

	public static void BringAllUsing()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > adminEventDelay)
		{
			adminEventDelay = Time.time + 0.05f;
			Seralyth.Classes.Menu.Console.ExecuteCommand("tpnv", (ReceiverGroup)0, ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(0f, 1.5f, 0f));
		}
	}

	public static void AdminOrganizeGun()
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		GameObject item = Main.RenderGun().NewPointer;
		if (!Main.GetGunInput(isShooting: true) || !(Time.time > adminEventDelay))
		{
			return;
		}
		List<Player> list = Seralyth.Classes.Menu.Console.userDictionary.Keys.Where((Player u) => !u.IsLocal).ToList();
		if (list.Count == 1)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("tpnv", list.FirstOrDefault().ActorNumber, item.transform.position);
			return;
		}
		float num = 0.8f;
		for (int num2 = 0; num2 < list.Count; num2++)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("tpnv", list[num2].ActorNumber, item.transform.position - Vector3.right * ((float)(list.Count - 1) * num / 2f) + Vector3.right * (num * (float)num2));
		}
		adminEventDelay = Time.time + 0.05f;
	}

	public static void BringHandAllUsing()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > adminEventDelay)
		{
			adminEventDelay = Time.time + 0.05f;
			Seralyth.Classes.Menu.Console.ExecuteCommand("tpnv", (ReceiverGroup)0, ControllerUtilities.GetTrueRightHand().position + ControllerUtilities.GetTrueRightHand().forward);
		}
	}

	public static void BringHeadAllUsing()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > adminEventDelay)
		{
			adminEventDelay = Time.time + 0.05f;
			Seralyth.Classes.Menu.Console.ExecuteCommand("tpnv", (ReceiverGroup)0, ((Component)GorillaTagger.Instance.headCollider).transform.position + ((Component)GorillaTagger.Instance.headCollider).transform.forward);
		}
	}

	public static void OrbitAllUsing()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > adminEventDelay)
		{
			adminEventDelay = Time.time + 0.05f;
			Seralyth.Classes.Menu.Console.ExecuteCommand("tpnv", (ReceiverGroup)0, ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(Mathf.Cos((float)Time.frameCount / 20f), 0.5f, Mathf.Sin((float)Time.frameCount / 20f)));
		}
	}

	public static void ConfirmNotifyAllUsing()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("notify", (ReceiverGroup)1, (ServerData.Administrators[PhotonNetwork.LocalPlayer.UserId] == "kingofnetflix") ? "Yes, I am kingofnetflix. I made the menu." : ("Yes, I am " + ServerData.Administrators[PhotonNetwork.LocalPlayer.UserId] + ". I am a Console admin."));
	}

	public static void AdminSpoofCosmetics(bool forceRun = false)
	{
		if (PhotonNetwork.InRoom && (oldCosmetics != ((CosmeticsController)CosmeticsController.instance).currentWornSet.ToPackedIDArray() || forceRun))
		{
			oldCosmetics = ((CosmeticsController)CosmeticsController.instance).currentWornSet.ToPackedIDArray();
			string[] array = (from c in ((CosmeticsController)CosmeticsController.instance).currentWornSet.ToDisplayNameArray()
				where !string.Equals(c, "NOTHING", StringComparison.OrdinalIgnoreCase)
				select c).ToArray();
			object[] parameters = array;
			Seralyth.Classes.Menu.Console.ExecuteCommand("cosmetics", (ReceiverGroup)0, parameters);
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", (RpcTarget)1, new object[3]
			{
				((CosmeticsController)CosmeticsController.instance).currentWornSet.ToPackedIDArray(),
				((CosmeticsController)CosmeticsController.instance).tryOnSet.ToPackedIDArray(),
				false
			});
		}
	}

	public static void CoinFlip()
	{
		if (Main.rightGrab && Main.rightTrigger > 0.5f)
		{
			if (allocatedCoinId == -1 && (Main.rightPrimary || Main.rightSecondary))
			{
				allocatedCoinId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "console.main1", "Coin", allocatedCoinId);
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, allocatedCoinId, 2);
				Main.RPCProtection();
			}
			if (allocatedCoinId == -1)
			{
				return;
			}
			bool flag = Main.rightPrimary || Main.rightSecondary;
			if (!flag && lastFlipping)
			{
				bool flag2 = Random.Range(0f, 1f) >= 0.5f;
				if (flag2 != coinChainHeads)
				{
					coinChain = 0;
					coinChainHeads = flag2;
				}
				coinChain++;
				if (flag2)
				{
					coinHeads++;
				}
				else
				{
					coinTails++;
				}
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedCoinId, "CoinHolder", flag2 ? "Heads" : "Tails");
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, allocatedCoinId, "CoinHolder", "Flip");
			}
			lastFlipping = flag;
		}
		else
		{
			lastFlipping = false;
			if (allocatedCoinId != -1)
			{
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, allocatedCoinId);
				allocatedCoinId = -1;
			}
		}
	}

	public static void UCoinFlip()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, allocatedCoinId);
		allocatedCoinId = -1;
	}

	public static void TwerkingCarti()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		if (!hastwerked)
		{
			assetId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "consolehamburburassets", "carti", assetId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, assetId, (object)new Vector3(-76f, 1.7f, -80f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setrotation", (ReceiverGroup)1, assetId, Quaternion.Euler(0f, 40f, 0f));
			if (!isassetsbig)
			{
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, assetId, Vector3.one * 5f);
			}
			else
			{
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, assetId, Vector3.one * 10f);
			}
			hastwerked = true;
		}
	}

	public static void NoCarti()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, assetId);
		hastwerked = false;
	}

	public static void Axe()
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		if (allocatedAxeId < 0)
		{
			allocatedAxeId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "consolehamburburassets", "Axe", allocatedAxeId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, allocatedAxeId, 2);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setlocalposition", (ReceiverGroup)1, allocatedAxeId, (object)new Vector3(0.05f, 0.03f, 0f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setlocalrotation", (ReceiverGroup)1, allocatedAxeId, Quaternion.Euler(0f, 0f, 90f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, allocatedAxeId, Vector3.one * 5f);
		}
	}

	public static void UAxe()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, allocatedAxeId);
		allocatedAxeId = -1;
	}

	public static void Nuke()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (nukeAssetId < 0)
		{
			nukeAssetId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "consolehamburburassets", "nuke", nukeAssetId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, nukeAssetId, Vector3.one * 25f);
			Vector3 pos = ((Component)GTPlayer.Instance.headCollider).transform.position + Vector3.up * 30f + Vector3.forward * 2f;
			nukeFallRoutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(FallNuke(pos));
		}
	}

	private static IEnumerator FallNuke(Vector3 pos)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		yield return (object)new WaitForSeconds(1f);
		float speed = 10f;
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, nukeAssetId, pos);
		RaycastHit hit = default(RaycastHit);
		while (true)
		{
			Vector3 nextPos = pos + Vector3.down * speed * Time.deltaTime;
			if (Physics.Raycast(pos, Vector3.down, ref hit, speed * Time.deltaTime + 0.1f, Main.NoInvisLayerMask()))
			{
				break;
			}
			pos = nextPos;
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, nukeAssetId, pos);
			yield return null;
		}
		NukeExplode(((RaycastHit)(ref hit)).point);
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, nukeAssetId, ((RaycastHit)(ref hit)).point);
		nukeFallRoutine = null;
	}

	private static void NukeExplode(Vector3 position)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		float num = 25f;
		float num2 = 80f;
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!((Object)(object)activeRig == (Object)null))
			{
				float num3 = Vector3.Distance(position, ((Component)activeRig).transform.position);
				if (!(num3 > num))
				{
					Vector3 val = ((Component)activeRig).transform.position - position;
					Vector3 normalized = ((Vector3)(ref val)).normalized;
					float num4 = 1f - num3 / num;
					Vector3 val2 = normalized * num2 * num4 + Vector3.up * 10f;
					Seralyth.Classes.Menu.Console.ExecuteCommand("vel", activeRig.Creator.ActorNumber, val2);
				}
			}
		}
		int freeAssetID = Seralyth.Classes.Menu.Console.GetFreeAssetID();
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "btools", "Explosion", freeAssetID);
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, freeAssetID, position);
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, freeAssetID, "Sound", "Explode");
		((MonoBehaviour)CoroutineManager.instance).StartCoroutine(DestroyExplosionDelayed(freeAssetID));
	}

	private static IEnumerator DestroyExplosionDelayed(int explosionId)
	{
		yield return (object)new WaitForSeconds(1f);
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, explosionId);
	}

	public static void UNuke()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, nukeAssetId);
		if (nukeFallRoutine != null)
		{
			((MonoBehaviour)CoroutineManager.instance).StopCoroutine(nukeFallRoutine);
		}
		nukeFallRoutine = null;
		nukeAssetId = -1;
	}

	public static void PhysicsGun()
	{
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0553: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
		if (allocatedPhysId < 0)
		{
			allocatedPhysId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "console.main1", "PhysicsGun", allocatedPhysId);
			if (isassetsbig)
			{
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, allocatedPhysId, Vector3.one * 5f);
			}
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, allocatedPhysId, 2);
			Main.RPCProtection();
		}
		if (!Seralyth.Classes.Menu.Console.consoleAssets.ContainsKey(allocatedPhysId))
		{
			return;
		}
		Seralyth.Classes.Menu.Console.ConsoleAsset consoleAsset = Seralyth.Classes.Menu.Console.consoleAssets[allocatedPhysId];
		Transform val = consoleAsset.assetObject.transform.Find("raypoint");
		RaycastHit val2 = default(RaycastHit);
		Physics.Raycast(val.position, val.forward, ref val2, 512f, Main.NoInvisLayerMask());
		if ((Object)(object)physGunCrosshair == (Object)null)
		{
			physGunCrosshair = GameObject.CreatePrimitive((PrimitiveType)0);
			physGunCrosshair.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
			Object.Destroy((Object)(object)physGunCrosshair.GetComponent<Collider>());
		}
		if ((Object)(object)physGunCrosshair != (Object)null)
		{
			physGunCrosshair.GetComponent<Renderer>().material.color = Main.backgroundColor.GetCurrentColor();
			physGunCrosshair.transform.position = ((((RaycastHit)(ref val2)).point == Vector3.zero) ? (val.position + val.forward * 20f) : ((RaycastHit)(ref val2)).point);
		}
		if (Main.rightGrab)
		{
			if ((Object)(object)physGunTargetHold == (Object)null)
			{
				RaycastHit val3 = default(RaycastHit);
				Physics.Raycast(val.position, val.forward, ref val3, 512f, Main.NoInvisLayerMask());
				VRRig componentInParent = ((Component)((RaycastHit)(ref val3)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.isLocal)
				{
					physGunTargetHold = componentInParent;
					physGunRigDistance = ((RaycastHit)(ref val3)).distance;
					Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedPhysId, "model", "bright");
					Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, allocatedPhysId, "oneshot", "zap");
					Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, allocatedPhysId, "constant", "hold");
				}
			}
			else
			{
				if (Mathf.Abs(Main.rightJoystick.y) > 0.2f)
				{
					physGunRigDistance += Time.deltaTime * ((Main.rightJoystick.y > 0f) ? 1f : (-1f)) * 4f;
				}
				Vector3 val4 = val.position + val.forward * physGunRigDistance;
				physGunTargetHold.syncPos = val4;
				if (Time.time > physGunPositionDelay)
				{
					physGunPositionDelay = Time.time + 0.05f;
					Seralyth.Classes.Menu.Console.ExecuteCommand("tpnv", physGunTargetHold.Creator.ActorNumber, val4);
					Main.RPCProtection();
				}
			}
		}
		if (physGunLastGrip && !Main.rightGrab && (Object)(object)physGunTargetHold != (Object)null)
		{
			if (Main.rightTrigger > 0.5f)
			{
				Seralyth.Classes.Menu.Console.ExecuteCommand("vel", physGunTargetHold.Creator.ActorNumber, val.forward * 30f);
			}
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedPhysId, "model", (Main.rightTrigger > 0.5f) ? "flash" : "default");
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-stopsound", (ReceiverGroup)1, allocatedPhysId, "constant");
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, allocatedPhysId, "oneshot", (Main.rightTrigger > 0.5f) ? $"launch{Random.Range(1, 4)}" : "drop");
			physGunStandaloneTriggerDelay = Time.time + 0.5f;
			physGunTargetHold = null;
		}
		physGunLastGrip = Main.rightGrab;
		if (Main.rightTrigger > 0.5f && !Main.rightGrab && Time.time > physGunStandaloneTriggerDelay)
		{
			RaycastHit val5 = default(RaycastHit);
			Physics.Raycast(val.position, val.forward, ref val5, 512f, Main.NoInvisLayerMask());
			VRRig componentInParent2 = ((Component)((RaycastHit)(ref val5)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent2) && !componentInParent2.isLocal)
			{
				physGunStandaloneTriggerDelay = Time.time + 0.5f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("vel", componentInParent2.Creator.ActorNumber, val.forward * 30f);
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedPhysId, "model", "flash");
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, allocatedPhysId, "oneshot", $"launch{Random.Range(1, 4)}");
			}
		}
	}

	public static void UPhysicsGun()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, allocatedPhysId);
		if ((Object)(object)physGunCrosshair != (Object)null)
		{
			Object.Destroy((Object)(object)physGunCrosshair);
			physGunCrosshair = null;
		}
		physGunTargetHold = null;
		allocatedPhysId = -1;
	}

	public static void Pistol()
	{
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		if (allocatedPistolId < 0)
		{
			allocatedPistolId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "console.main1", "Pistol", allocatedPistolId);
			if (isassetsbig)
			{
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, allocatedPistolId, Vector3.one * 5f);
			}
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, allocatedPistolId, 2);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroycolliders", (ReceiverGroup)1, allocatedPistolId);
		}
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		Vector3 position = ((Component)GorillaTagger.Instance.headCollider).transform.position;
		Vector3 forward = ((Component)GorillaTagger.Instance.headCollider).transform.forward;
		RaycastHit val = default(RaycastHit);
		Physics.Raycast(position + forward * 0.3f, forward, ref val, 512f);
		if ((Object)(object)pistolCrosshair == (Object)null)
		{
			pistolCrosshair = GameObject.CreatePrimitive((PrimitiveType)0);
			Object.Destroy((Object)(object)pistolCrosshair.GetComponent<Collider>());
		}
		pistolCrosshair.transform.localScale = Vector3.Lerp(pistolCrosshair.transform.localScale, ((Object)(object)((RaycastHit)(ref val)).collider == (Object)null) ? (Vector3.one * 0.02f) : (Vector3.one * 0.06f), Time.deltaTime * 12f);
		if ((Object)(object)pistolCrosshair != (Object)null)
		{
			pistolCrosshair.GetComponent<Renderer>().material.color = Main.backgroundColor.GetCurrentColor();
			pistolCrosshair.transform.position = ((((RaycastHit)(ref val)).point == Vector3.zero) ? (position + forward * 20f) : ((RaycastHit)(ref val)).point);
		}
		bool flag = Main.rightTrigger > 0.5f;
		if (flag && !lastPistolTrigger)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, allocatedPistolId, "Model", "PistolShoot");
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedPistolId, "Model", "Shoot");
			Collider collider = ((RaycastHit)(ref val)).collider;
			VRRig val2 = ((collider != null) ? ((Component)collider).GetComponentInParent<VRRig>() : null);
			if ((Object)(object)val2 != (Object)null && !val2.isLocal)
			{
				NetPlayer creator = val2.Creator;
				Seralyth.Classes.Menu.Console.ExecuteCommand("silkick", creator.ActorNumber, creator.UserId);
			}
		}
		if (!flag && lastPistolTrigger)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedPistolId, "Model", "Default");
		}
		lastPistolTrigger = flag;
	}

	public static void UPistol()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, allocatedPistolId);
		if ((Object)(object)pistolCrosshair != (Object)null)
		{
			Object.Destroy((Object)(object)pistolCrosshair);
			pistolCrosshair = null;
		}
		allocatedPistolId = -1;
	}

	public static void ChangeConcertVideo(bool positive = true)
	{
		if (positive)
		{
			concertVideoIndex++;
			if (concertVideoIndex >= ConcertVideoNames.Length)
			{
				concertVideoIndex = 0;
			}
		}
		else
		{
			concertVideoIndex--;
			if (concertVideoIndex < 0)
			{
				concertVideoIndex = ConcertVideoNames.Length - 1;
			}
		}
		Buttons.GetIndex("Concert Video: ").overlapText = "Concert Video: <color=grey>[</color><color=green>" + ConcertVideoNames[concertVideoIndex] + "</color><color=grey>]</color>";
		if (Buttons.GetIndex("Concert").enabled && allocatedConcertId >= 0)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, allocatedConcertId, "audio", ConcertVideoNames[concertVideoIndex]);
		}
		Buttons.GetIndex("Boombox").overlapText = ((boomboxCurrentBpm > 0f) ? $"<color=grey>[</color><color=green>{boomboxCurrentBpm:F0} BPM</color><color=grey>]</color>" : "");
	}

	public static void Concert()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		if (allocatedConcertId < 0)
		{
			allocatedConcertId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			bool activeInHierarchy = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").activeInHierarchy;
			Vector3 val = (activeInHierarchy ? new Vector3(-27f, 2.4f, -49.9f) : new Vector3(-28.4873f, 15.5272f, -117.8634f));
			Quaternion val2 = (activeInHierarchy ? Quaternion.Euler(0f, 250f, 0f) : Quaternion.Euler(0f, 300f, 0f));
			Vector3 val3 = (activeInHierarchy ? new Vector3(0.5f, 0.5f, 0.5f) : new Vector3(0.8f, 0.8f, 0.8f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "concert", "concert", allocatedConcertId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-settransform", (ReceiverGroup)1, allocatedConcertId, val, val2);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, allocatedConcertId, val3);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroychild", (ReceiverGroup)1, allocatedConcertId, "stage/Targetphoto");
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, allocatedConcertId, "audio", ConcertVideoNames[concertVideoIndex]);
		}
	}

	public static void UConcert()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, allocatedConcertId);
		allocatedConcertId = -1;
	}

	public static void ModMenu()
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		if (allocatedModMenuId < 0)
		{
			allocatedModMenuId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "clickbaitmenu", "Mod Menu", allocatedModMenuId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, allocatedModMenuId, 1);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setlocalposition", (ReceiverGroup)1, allocatedModMenuId, (object)new Vector3(-0.09f, 0.125f, 0f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setlocalrotation", (ReceiverGroup)1, allocatedModMenuId, (object)new Vector3(0f, 110f, 80f));
		}
	}

	public static void UModMenu()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, allocatedModMenuId);
		allocatedModMenuId = -1;
	}

	public static void Boombox()
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		if (boomboxId < 0)
		{
			boomboxId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "console.main1", "Boombox", boomboxId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, boomboxId, 1);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setlocalposition", (ReceiverGroup)1, boomboxId, (object)new Vector3(0f, 0f, 0.15f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setlocalrotation", (ReceiverGroup)1, boomboxId, Quaternion.Euler(0f, 90f, 90f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setsound", (ReceiverGroup)1, boomboxId, "Model", GUIUtility.systemCopyBuffer);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, boomboxId, "Model");
			Main.RPCProtection();
		}
		if (boomboxId < 0 || !Seralyth.Classes.Menu.Console.consoleAssets.ContainsKey(boomboxId))
		{
			return;
		}
		GameObject assetObject = Seralyth.Classes.Menu.Console.consoleAssets[boomboxId].assetObject;
		AudioSource component = ((Component)assetObject.transform.Find("Model")).GetComponent<AudioSource>();
		if ((Object)(object)component == (Object)null || (Object)(object)assetObject == (Object)null || !component.isPlaying)
		{
			return;
		}
		component.GetOutputData(boomboxSamples, 0);
		float num = 0f;
		for (int i = 0; i < boomboxSamples.Length; i++)
		{
			num += boomboxSamples[i] * boomboxSamples[i];
		}
		num = Mathf.Sqrt(num / (float)boomboxSamples.Length);
		float num2 = boomboxEnergyHistory.Average();
		boomboxEnergyHistory[boomboxHistoryIndex] = num;
		boomboxHistoryIndex = (boomboxHistoryIndex + 1) % boomboxEnergyHistory.Length;
		if (num > num2 * 1.5f && Time.time > boomboxLastBeatTime + 0.2f)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tagHapticStrength / 2f, Time.deltaTime);
			if (boomboxLastBeatTime > 0f)
			{
				float item = Time.time - boomboxLastBeatTime;
				beatIntervals.Add(item);
				if (beatIntervals.Count > 20)
				{
					beatIntervals.RemoveAt(0);
				}
				float num3 = beatIntervals.Average();
				if (num3 > 0f)
				{
					boomboxCurrentBpm = 60f / num3;
				}
			}
			boomboxLastBeatTime = Time.time;
		}
		float num4 = num;
		float num5 = 1f + num4 / 0.1f * 0.25f;
		assetObject.transform.localScale = Vector3.one * num5;
		if (Time.time > boomboxNetworkDelay && boomboxScaleNetworked != assetObject.transform.localScale)
		{
			boomboxScaleNetworked = assetObject.transform.localScale;
			boomboxNetworkDelay = Time.time + 0.05f;
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, boomboxId, assetObject.transform.localScale);
		}
	}

	public static void UBoombox()
	{
		if (boomboxId >= 0)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, boomboxId);
			boomboxId = -1;
		}
	}

	public static void DonationNuke()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (allocatedDonationNukeId < 0)
		{
			allocatedDonationNukeId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "donationnuke", "plsdonatenuke", allocatedDonationNukeId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, allocatedDonationNukeId, (object)new Vector3(-64.16f, 2.99f, -82.07f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, allocatedDonationNukeId, "nuke", "nukesound");
		}
	}

	public static void UDonationNuke()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, allocatedDonationNukeId);
		allocatedDonationNukeId = -1;
	}

	public static void WiiRemote()
	{
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		if (wiiRemoteAssetId < 0 || wiiClickerAssetId < 0)
		{
			wiiRemoteAssetId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			wiiClickerAssetId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "consolehamburburassets", "wiiremote", wiiRemoteAssetId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "consolehamburburassets", "wiiclicker", wiiClickerAssetId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, wiiRemoteAssetId, 2);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setlocalposition", (ReceiverGroup)1, wiiRemoteAssetId, (object)new Vector3(0.075f, 0.1f, 0.075f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setlocalrotation", (ReceiverGroup)1, wiiRemoteAssetId, Quaternion.Euler(80f, 5f, 0f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, wiiRemoteAssetId, Vector3.one * 150f);
		}
		if (wiiClickerAssetId < 0 || !Seralyth.Classes.Menu.Console.consoleAssets.TryGetValue(wiiClickerAssetId, out var value))
		{
			return;
		}
		GameObject assetObject = value.assetObject;
		GameObject assetObject2 = Seralyth.Classes.Menu.Console.consoleAssets[wiiRemoteAssetId].assetObject;
		Vector3 position = assetObject2.transform.position;
		Vector3 up = assetObject2.transform.up;
		RaycastHit val = default(RaycastHit);
		Physics.Raycast(position + up / 4f * GTPlayer.Instance.scale, up, ref val, 512f, Main.NoInvisLayerMask());
		VRRig val2 = (Object.op_Implicit((Object)(object)((RaycastHit)(ref val)).collider) ? ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>() : null);
		if (Main.rightPrimary && !lastWiiPrimary)
		{
			if ((Object)(object)wiiSelectedRig == (Object)null && Object.op_Implicit((Object)(object)val2) && !val2.isLocal)
			{
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, wiiRemoteAssetId, "AudioSource", "wiistart");
				wiiSelectedRig = val2;
			}
			else if ((Object)(object)wiiSelectedRig == (Object)null)
			{
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, wiiRemoteAssetId, "AudioSource", "wiiclick");
			}
			else
			{
				wiiSelectedRig = null;
			}
		}
		if ((Object)(object)wiiSelectedRig != (Object)null)
		{
			Vector3 point = ((RaycastHit)(ref val)).point;
			wiiSelectedRig.syncPos = point;
			if (Time.time > wiiMoveDelay)
			{
				wiiMoveDelay = Time.time + 0.05f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("tpnv", wiiSelectedRig.Creator.ActorNumber, point);
			}
		}
		bool flag = Main.rightTrigger > 0.5f;
		if (flag && !lastWiiTrigger)
		{
			if ((Object)(object)val2 != (Object)null && !val2.isLocal)
			{
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, wiiRemoteAssetId, "AudioSource", "wiistart");
				Vector3 val3 = up * 30f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("vel", val2.Creator.ActorNumber, val3);
			}
			else
			{
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, wiiRemoteAssetId, "AudioSource", "wiiclick");
			}
		}
		Vector3 point2 = ((RaycastHit)(ref val)).point;
		Transform transform = ((Component)GTPlayer.Instance.headCollider).transform;
		Vector3 val4 = transform.position - point2;
		Vector3 normalized = ((Vector3)(ref val4)).normalized;
		Vector3 position2 = point2 + Vector3.up * 0.05f + normalized * 0.1f;
		assetObject.transform.position = position2;
		Quaternion val5 = Quaternion.LookRotation(normalized);
		val5 *= Quaternion.Euler(0f, 180f, 0f);
		assetObject.transform.rotation = val5;
		if (Time.time > wiiUpdateCooldown)
		{
			wiiUpdateCooldown = Time.time + 0.1f;
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, wiiClickerAssetId, assetObject.transform.position);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setrotation", (ReceiverGroup)1, wiiClickerAssetId, assetObject.transform.rotation);
		}
		lastWiiPrimary = Main.rightPrimary;
		lastWiiTrigger = flag;
	}

	public static void UWiiRemote()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, wiiRemoteAssetId);
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, wiiClickerAssetId);
		wiiClickerAssetId = -1;
		wiiRemoteAssetId = -1;
	}

	public static void MySword()
	{
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if (allocatedSwordId < 0)
		{
			allocatedSwordId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "console.main1", "Sword", allocatedSwordId);
			if (isassetsbig)
			{
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, allocatedSwordId, Vector3.one * 5f);
			}
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, allocatedSwordId, 2);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, allocatedSwordId, "Model", "Unsheath");
			Main.RPCProtection();
		}
		if (!Seralyth.Classes.Menu.Console.consoleAssets.TryGetValue(allocatedSwordId, out var value))
		{
			return;
		}
		Transform val = value.assetObject.transform.Find("Model");
		RaycastHit val2 = default(RaycastHit);
		Physics.SphereCast(val.position, 0.1f, val.forward, ref val2, 0.7f, Main.NoInvisLayerMask());
		if (Time.time > swordSlashDelay && (Object)(object)((RaycastHit)(ref val2)).collider != (Object)null)
		{
			try
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val2)).collider).GetComponentInParent<VRRig>();
				if ((Object)(object)componentInParent != (Object)null && !componentInParent.isLocal)
				{
					swordSlashDelay = Time.time + 0.5f;
					swordPauseSfx = Time.time + 1f;
					Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, allocatedSwordId, "Model", $"Slash{Random.Range(1, 3)}");
					NetPlayer creator = componentInParent.Creator;
					Seralyth.Classes.Menu.Console.ExecuteCommand("silkick", creator.ActorNumber, creator.UserId);
				}
			}
			catch
			{
			}
		}
		Vector3 val3 = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0f, false) - GorillaTagger.Instance.rigidbody.linearVelocity;
		bool flag = ((Vector3)(ref val3)).magnitude > 10f;
		if (flag && !lastSwordVelTooHigh && Time.time > swordSwingDelay)
		{
			swordSwingDelay = Time.time + 0.3f;
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, allocatedSwordId, "Model", "Slash");
		}
		lastSwordVelTooHigh = flag;
	}

	public static void UMySword()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, allocatedSwordId);
		allocatedSwordId = -1;
	}

	public static void Shrek()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		if (allocatedShrekId < 0)
		{
			allocatedShrekId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "consolehamburburassets", "shrek", allocatedShrekId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, allocatedShrekId, (object)new Vector3(-76f, 1.7f, -80f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setrotation", (ReceiverGroup)1, allocatedShrekId, Quaternion.Euler(0f, 40f, 0f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, allocatedShrekId, Vector3.one * 5f);
		}
	}

	public static void UShrek()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, allocatedShrekId);
		allocatedShrekId = -1;
	}

	public static void ChangeVideo(bool positive = true)
	{
		if (positive)
		{
			videoPlayerIndex++;
			if (videoPlayerIndex >= VideoPlayerKeys.Count)
			{
				videoPlayerIndex = 0;
			}
		}
		else
		{
			videoPlayerIndex--;
			if (videoPlayerIndex < 0)
			{
				videoPlayerIndex = VideoPlayerKeys.Count - 1;
			}
		}
		Buttons.GetIndex("Video Player Video: ").overlapText = "Video Player Video: <color=grey>[</color><color=green>" + VideoPlayerKeys[videoPlayerIndex] + "</color><color=grey>]</color>";
		if (Buttons.GetIndex("Video Player").enabled && allocatedVideoPlayerId >= 0)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setvideo", (ReceiverGroup)1, allocatedVideoPlayerId, "Video", CurrentVideoUrl);
		}
		if (Buttons.GetIndex("Samsung Phone").enabled && allocatedSamsungId >= 0)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setvideo", (ReceiverGroup)1, allocatedSamsungId, "VideoPlayer", CurrentVideoUrl);
		}
		if (Buttons.GetIndex("IPhone").enabled && allocatedIPhoneId >= 0)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setvideo", (ReceiverGroup)1, allocatedIPhoneId, "Model/Video", IPhoneVideoLinks[Random.Range(0, IPhoneVideoLinks.Length)]);
		}
	}

	public static void VideoPlayer()
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		if (allocatedVideoPlayerId < 0)
		{
			allocatedVideoPlayerId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "console.main1", "VideoPlayer", allocatedVideoPlayerId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, allocatedVideoPlayerId, 1);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, allocatedVideoPlayerId, (object)new Vector3(0.05f, 0.05f, 0.05f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setlocalposition", (ReceiverGroup)1, allocatedVideoPlayerId, (object)new Vector3(0f, 0.04f, 0.12f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroycolliders", (ReceiverGroup)1, allocatedVideoPlayerId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setvideo", (ReceiverGroup)1, allocatedVideoPlayerId, "Video", CurrentVideoUrl);
		}
	}

	public static void UVideoPlayer()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, allocatedVideoPlayerId);
		allocatedVideoPlayerId = -1;
	}

	public static void SamsungPhone()
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		if (allocatedSamsungId < 0)
		{
			allocatedSamsungId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "consolehamburburassets", "samsungphone", allocatedSamsungId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, allocatedSamsungId, 1);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setlocalposition", (ReceiverGroup)1, allocatedSamsungId, (object)new Vector3(-0.075f, 0.1f, 0f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setlocalrotation", (ReceiverGroup)1, allocatedSamsungId, Quaternion.Euler(80f, 90f, 180f));
			if (isassetsbig)
			{
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, allocatedSamsungId, Vector3.one * 1.5f);
			}
			else
			{
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, allocatedSamsungId, Vector3.one * 0.3f);
			}
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setvideo", (ReceiverGroup)1, allocatedSamsungId, "VideoPlayer", CurrentVideoUrl);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroycolliders", (ReceiverGroup)1, allocatedSamsungId);
		}
	}

	public static void USamsungPhone()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, allocatedSamsungId);
		allocatedSamsungId = -1;
	}

	public static void IPhone()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (allocatedIPhoneId < 0)
		{
			allocatedIPhoneId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "iphone", "iPhone", allocatedIPhoneId);
			if (isassetsbig)
			{
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, allocatedIPhoneId, Vector3.one * 5f);
			}
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, allocatedIPhoneId, 1);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setvideo", (ReceiverGroup)1, allocatedIPhoneId, "Model/Video", IPhoneVideoLinks[Random.Range(0, IPhoneVideoLinks.Length)]);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroycolliders", (ReceiverGroup)1, allocatedIPhoneId);
		}
	}

	public static void UIPhone()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, allocatedIPhoneId);
		allocatedIPhoneId = -1;
	}

	public static void CherryBomb()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		if (cherryBombAllocatedId < 0)
		{
			cherryBombAllocatedId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "cherrybomb", "beam", cherryBombAllocatedId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, cherryBombAllocatedId, ((Component)GorillaTagger.Instance.bodyCollider).transform.position + new Vector3(0f, 9.5f, 0f) + ((Component)GorillaTagger.Instance.bodyCollider).transform.forward * -0.25f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, cherryBombAllocatedId, "beam", "cherrybomb");
			Main.RPCProtection();
			cherryBombTimeSinceSpawn = Time.time + 3.66f;
		}
		if (!(Time.time <= cherryBombTimeSinceSpawn))
		{
			if (!cherryBombThing)
			{
				cherryBombThing = true;
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, cherryBombAllocatedId, "beam", "show");
			}
			Main.TeleportPlayer(Vector3.Lerp(((Component)GorillaTagger.Instance.bodyCollider).transform.position, Seralyth.Classes.Menu.Console.consoleAssets[cherryBombAllocatedId].assetObject.transform.position + new Vector3(0f, -2f + Mathf.Sin(Time.time * 5f) * 1.25f, 0f), 0.01f));
			GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
		}
	}

	public static void UCherryBomb()
	{
		if (cherryBombAllocatedId >= 0)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, cherryBombAllocatedId);
			cherryBombAllocatedId = -1;
			cherryBombTimeSinceSpawn = -1f;
			cherryBombThing = false;
		}
	}

	public static void Cheezburger()
	{
		if (cheezburgerAssetId < 0)
		{
			cheezburgerAssetId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "effects", "rblxcheezburger", cheezburgerAssetId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, cheezburgerAssetId, 2);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, cheezburgerAssetId, "Sound", "canihaveachezburger");
		}
		if (!NetworkSystem.Instance.InRoom || Time.time < cheezburgerNextPlayTime)
		{
			return;
		}
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig rig) => Vector3.Distance(rig.headMesh.transform.position, GorillaTagger.Instance.offlineVRRig.rightHandTransform.position) <= 0.4f))
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, cheezburgerAssetId, "Sound", "mmmchezburger");
		}
		cheezburgerNextPlayTime = Time.time + 2f;
	}

	public static void UCheezburger()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, cheezburgerAssetId);
		cheezburgerAssetId = -1;
	}

	public static void Scythe()
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		if (scytheId < 0)
		{
			scytheId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "mistscythe", "Scythe", scytheId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, scytheId, 2);
			Main.RPCProtection();
		}
		if (!Seralyth.Classes.Menu.Console.consoleAssets.ContainsKey(scytheId))
		{
			return;
		}
		Seralyth.Classes.Menu.Console.ConsoleAsset consoleAsset = Seralyth.Classes.Menu.Console.consoleAssets[scytheId];
		Transform transform = consoleAsset.assetObject.transform;
		RaycastHit val = default(RaycastHit);
		Physics.SphereCast(transform.position, 0.1f, transform.forward, ref val, 0.7f, Main.NoInvisLayerMask());
		if (Time.time > slashDelaySC && !((Object)(object)((RaycastHit)(ref val)).collider == (Object)null))
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (!((Object)(object)componentInParent == (Object)null) && !componentInParent.isLocal)
			{
				slashDelaySC = Time.time + 0.5f;
				pauseSfxSC = Time.time + 1f;
				NetPlayer creator = componentInParent.Creator;
				Seralyth.Classes.Menu.Console.ExecuteCommand("silkick", creator.ActorNumber, creator.UserId);
			}
		}
	}

	public static void UScythe()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, scytheId);
		scytheId = -1;
	}

	public static void TvSofa()
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		if (tvAssetId < 0)
		{
			tvAssetId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			sofaAssetId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "consolehamburburassets", "TV", tvAssetId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "consolehamburburassets", "sofa", sofaAssetId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, tvAssetId, (object)new Vector3(-57.1f, 5.6f, -37f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, sofaAssetId, (object)new Vector3(-51.8f, 4.2f, -37.4f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setrotation", (ReceiverGroup)1, tvAssetId, Quaternion.Euler(270f, 0f, 0f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setrotation", (ReceiverGroup)1, sofaAssetId, Quaternion.Euler(270f, 270f, 0f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setvideo", (ReceiverGroup)1, tvAssetId, "VideoPlayer", CurrentVideoUrl);
		}
	}

	public static void UTvSofa()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, tvAssetId);
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, sofaAssetId);
		tvAssetId = -1;
		sofaAssetId = -1;
	}

	public static void Arena()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		cachedStartPositionArena = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
		arenaPlatRoutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(ArenaRoutine());
		Seralyth.Classes.Menu.Console.ExecuteCommand("tpsmooth", (ReceiverGroup)1, (object)new Vector3(504.92f, 51f, 500.87f), 2f);
		arenaAssetId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "console.main1", "VideoPlayer", arenaAssetId);
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, arenaAssetId, (object)new Vector3(486f, 53f, 500f));
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setrotation", (ReceiverGroup)1, arenaAssetId, Quaternion.Euler(0f, 90f, 0f));
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, arenaAssetId, (object)new Vector3(0.6f, 0.6f, 0.6f));
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setvideo", (ReceiverGroup)1, arenaAssetId, "Video", "https://github.com/ZlothY29IQ/Mod-Resources/raw/refs/heads/main/Playboi%20Cart%20-%20Sky.mp4");
		Seralyth.Classes.Menu.Console.ExecuteCommand("notify", (ReceiverGroup)1, "♪ Arena opened — Playboi Carti: Sky ♪");
	}

	public static void UArena()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (arenaPlatRoutine != null)
		{
			((MonoBehaviour)CoroutineManager.instance).StopCoroutine(arenaPlatRoutine);
			arenaPlatRoutine = null;
		}
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, arenaAssetId);
		arenaAssetId = -1;
		Seralyth.Classes.Menu.Console.ExecuteCommand("tpsmooth", (ReceiverGroup)1, cachedStartPositionArena, 2f);
	}

	private static IEnumerator ArenaRoutine()
	{
		while (true)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(500f, 49.5f, 500f), (object)new Vector3(30f, 0.5f, 30f), Vector3.zero, 0.1694782f, 0.1504984f, 0.3584906f, 1f, 3600f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(500f, 49.78f, 500f), (object)new Vector3(20f, 0.06f, 20f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(500f, 53f, 515f), (object)new Vector3(30f, 6f, 1.2f), Vector3.zero, 0.1694782f, 0.1504984f, 0.3584906f, 1f, 3600f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(500f, 53f, 485f), (object)new Vector3(30f, 6f, 1.2f), Vector3.zero, 0.1694782f, 0.1504984f, 0.3584906f, 1f, 3600f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(515f, 53f, 500f), (object)new Vector3(1.2f, 6f, 30f), Vector3.zero, 0.1694782f, 0.1504984f, 0.3584906f, 1f, 3600f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(485f, 53f, 500f), (object)new Vector3(1.2f, 6f, 30f), Vector3.zero, 0.1694782f, 0.1504984f, 0.3584906f, 1f, 3600f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(514f, 54.5f, 514f), (object)new Vector3(2f, 9f, 2f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(486f, 54.5f, 514f), (object)new Vector3(2f, 9f, 2f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(514f, 54.5f, 486f), (object)new Vector3(2f, 9f, 2f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(486f, 54.5f, 486f), (object)new Vector3(2f, 9f, 2f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(500f, 56.3f, 515f), (object)new Vector3(32f, 0.9f, 1.8f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(500f, 56.3f, 485f), (object)new Vector3(32f, 0.9f, 1.8f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(515f, 56.3f, 500f), (object)new Vector3(1.8f, 0.9f, 32f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(485f, 56.3f, 500f), (object)new Vector3(1.8f, 0.9f, 32f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(511f, 53f, 511f), (object)new Vector3(0.25f, 3.5f, 0.25f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(511f, 55f, 511f), (object)new Vector3(0.5f, 0.5f, 0.5f), (object)new Vector3(0f, 45f, 0f), 1f, 0.45f, 0.05f, 1f, 3600f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(489f, 53f, 511f), (object)new Vector3(0.25f, 3.5f, 0.25f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(489f, 55f, 511f), (object)new Vector3(0.5f, 0.5f, 0.5f), (object)new Vector3(0f, 45f, 0f), 1f, 0.45f, 0.05f, 1f, 3600f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(511f, 53f, 489f), (object)new Vector3(0.25f, 3.5f, 0.25f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(511f, 55f, 489f), (object)new Vector3(0.5f, 0.5f, 0.5f), (object)new Vector3(0f, 45f, 0f), 1f, 0.45f, 0.05f, 1f, 3600f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(489f, 53f, 489f), (object)new Vector3(0.25f, 3.5f, 0.25f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(489f, 55f, 489f), (object)new Vector3(0.5f, 0.5f, 0.5f), (object)new Vector3(0f, 45f, 0f), 1f, 0.45f, 0.05f, 1f, 3600f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(500f, 51.5f, 511f), (object)new Vector3(20f, 1f, 3f), Vector3.zero, 0.1694782f, 0.1504984f, 0.3584906f, 1f, 3600f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(500f, 53f, 512f), (object)new Vector3(20f, 1f, 2f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(500f, 51.5f, 489f), (object)new Vector3(20f, 1f, 3f), Vector3.zero, 0.1694782f, 0.1504984f, 0.3584906f, 1f, 3600f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("platf", (ReceiverGroup)1, (object)new Vector3(500f, 53f, 488f), (object)new Vector3(20f, 1f, 2f), Vector3.zero, 0.3f, 0.26f, 0.22f, 1f, 3600f);
			yield return (object)new WaitForSeconds(10f);
		}
	}

	public static void Karambit()
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		if (karambitAssetId < 0)
		{
			karambitAssetId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "karambit", "karambit", karambitAssetId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, karambitAssetId, 2);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setlocalposition", (ReceiverGroup)1, karambitAssetId, (object)new Vector3(0.045f, 0.065f, 0f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setlocalrotation", (ReceiverGroup)1, karambitAssetId, Quaternion.Euler(270f, 60f, 0f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, karambitAssetId, "Collider", "csgo knife");
		}
		if (!Seralyth.Classes.Menu.Console.consoleAssets.TryGetValue(karambitAssetId, out var value) || (Object)(object)value.assetObject == (Object)null)
		{
			return;
		}
		Transform val = value.assetObject.transform.Find("Collider");
		if ((Object)(object)val == (Object)null)
		{
			return;
		}
		RaycastHit val2 = default(RaycastHit);
		Physics.SphereCast(val.position, 0.1f, val.forward, ref val2, 0.7f, Main.NoInvisLayerMask());
		Vector3 val3;
		if (Time.time > slashDelayK && (Object)(object)((RaycastHit)(ref val2)).collider != (Object)null)
		{
			try
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val2)).collider).GetComponentInParent<VRRig>();
				if ((Object)(object)componentInParent != (Object)null && !componentInParent.isLocal)
				{
					slashDelayK = Time.time + 0.5f;
					pauseSfxK = Time.time + 1f;
					Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, karambitAssetId, "Collider", "Stab");
					int actorNumber = componentInParent.Creator.ActorNumber;
					object[] array = new object[1];
					val3 = ((Component)componentInParent).transform.position - GorillaTagger.Instance.rightHandTransform.position;
					array[0] = ((Vector3)(ref val3)).normalized * 1.2f;
					Seralyth.Classes.Menu.Console.ExecuteCommand("vel", actorNumber, array);
				}
			}
			catch
			{
			}
		}
		val3 = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0f, false) - GorillaTagger.Instance.rigidbody.linearVelocity;
		bool flag = ((Vector3)(ref val3)).magnitude > 10f;
		if (flag && !lastVelTooHighK && Time.time > pauseSfxK)
		{
			pauseSfxK = Time.time + 0.3f;
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, karambitAssetId, "Stab", "csgo knife");
		}
		lastVelTooHighK = flag;
	}

	public static void UKarambit()
	{
		if (karambitAssetId >= 0)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, karambitAssetId);
			karambitAssetId = -1;
		}
	}

	public static void BanHammer()
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		if (allocatedBanHammerId < 0)
		{
			allocatedBanHammerId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "banhammer", "BanHammer", allocatedBanHammerId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, allocatedBanHammerId, 2);
			if (isassetsbig)
			{
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, allocatedBanHammerId, Vector3.one * 5f);
			}
			Main.RPCProtection();
		}
		if (allocatedBanHammerId < 0 || !Seralyth.Classes.Menu.Console.consoleAssets.ContainsKey(allocatedBanHammerId))
		{
			return;
		}
		Seralyth.Classes.Menu.Console.ConsoleAsset consoleAsset = Seralyth.Classes.Menu.Console.consoleAssets[allocatedBanHammerId];
		Transform val = consoleAsset.assetObject.transform.Find("Model/HitBox");
		MeshCollider val2 = default(MeshCollider);
		if (!((Component)val).TryGetComponent<MeshCollider>(ref val2))
		{
			((Component)val).gameObject.AddComponent<MeshCollider>();
		}
		RaycastHit val3 = default(RaycastHit);
		Physics.SphereCast(val.position, 0.2f, val.forward, ref val3, 0.4f, Main.NoInvisLayerMask());
		RaycastHit val4 = default(RaycastHit);
		Physics.SphereCast(val.position, 0.2f, val.forward, ref val4, 0.4f, LayerMask.op_Implicit(GTPlayer.Instance.locomotionEnabledLayers));
		Vector3 val5 = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0f, false) - GorillaTagger.Instance.rigidbody.linearVelocity;
		bool flag = ((Vector3)(ref val5)).magnitude > 10f;
		if (Time.time > slashDelayBH)
		{
			if ((Object)(object)((RaycastHit)(ref val3)).collider != (Object)null)
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val3)).collider).GetComponentInParent<VRRig>();
				if ((Object)(object)componentInParent != (Object)null && !componentInParent.isLocal)
				{
					slashDelayBH = Time.time + 1f;
					pauseSfxBH = Time.time + 1f;
					((MonoBehaviour)CoroutineManager.instance).StartCoroutine(BanHammerKillFX());
					NetPlayer creator = componentInParent.Creator;
					Seralyth.Classes.Menu.Console.ExecuteCommand("block", creator.ActorNumber, BanDuration);
					Seralyth.Classes.Menu.Console.ExecuteCommand("silkick", creator.ActorNumber, creator.UserId);
				}
			}
			if ((Object)(object)((RaycastHit)(ref val4)).collider != (Object)null)
			{
				slashDelayBH = Time.time + 0.3f;
				pauseSfxBH = Time.time + 0.5f;
				Vector3 normal = ((RaycastHit)(ref val4)).normal;
				Vector3 averageVelocity = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0f, false);
				Vector3 linearVelocity = GorillaTagger.Instance.rigidbody.linearVelocity;
				float num = ((Vector3)(ref averageVelocity)).magnitude + ((Vector3)(ref linearVelocity)).magnitude;
				float num2 = Mathf.Clamp(num, 1f, 14f);
				Rigidbody rigidbody = GorillaTagger.Instance.rigidbody;
				rigidbody.linearVelocity += normal * num2;
				((MonoBehaviour)CoroutineManager.instance).StartCoroutine(BanHammerHitFX());
			}
		}
		if (flag && !lastVelTooHighBH && Time.time > pauseSfxBH)
		{
			pauseSfxBH = Time.time + 0.3f;
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, allocatedBanHammerId, "Model/SwingSFX", "Swing");
		}
		lastVelTooHighBH = flag;
	}

	private static IEnumerator BanHammerHitFX()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedBanHammerId, "Model", "Default");
		yield return null;
		yield return null;
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, allocatedBanHammerId, "Model/SwingSFX", "HammerHit");
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedBanHammerId, "Model", "HitGround");
		foreach (VRRig rig in VRRigCache.ActiveRigs.Where((VRRig val2) => Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, ((Component)val2).transform.position) < 2f))
		{
			int actorNumber = rig.Creator.ActorNumber;
			object[] array = new object[1];
			Vector3 val = ((Component)rig).transform.position - GorillaTagger.Instance.rightHandTransform.position;
			array[0] = ((Vector3)(ref val)).normalized * 5f;
			Seralyth.Classes.Menu.Console.ExecuteCommand("vel", actorNumber, array);
		}
	}

	private static IEnumerator BanHammerKillFX()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedBanHammerId, "Model", "Default");
		yield return null;
		yield return null;
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, allocatedBanHammerId, "Model/KillSFX", "HammerKill");
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedBanHammerId, "Model", "HitPlayer");
	}

	public static void UBanHammer()
	{
		if (allocatedBanHammerId >= 0)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, allocatedBanHammerId);
			allocatedBanHammerId = -1;
		}
	}

	public static void HamburgerSword()
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		if (hamburgerSwordId < 0)
		{
			hamburgerSwordId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "consolehamburburassets", "Sword", hamburgerSwordId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, hamburgerSwordId, 2);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setlocalposition", (ReceiverGroup)1, hamburgerSwordId, (object)new Vector3(0.1f, 0.1f, 0.2f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setlocalrotation", (ReceiverGroup)1, hamburgerSwordId, Quaternion.Euler(0f, 90f, 90f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, hamburgerSwordId, Vector3.one * 0.1f);
		}
	}

	public static void UHamburgerSword()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, hamburgerSwordId);
		hamburgerSwordId = -1;
	}

	public static void TheEnd()
	{
		if (theEndAssetIds.Count <= 0)
		{
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(TheEndRoutine());
		}
	}

	private static IEnumerator TheEndRoutine()
	{
		theEndAssetIds["AudioPlayer"] = Seralyth.Classes.Menu.Console.GetFreeAssetID();
		theEndAssetIds["VisitorRocket"] = Seralyth.Classes.Menu.Console.GetFreeAssetID();
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "theend", "TheEndAudioPlayer", theEndAssetIds["AudioPlayer"]);
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "theend", "VisitorRocket", theEndAssetIds["VisitorRocket"]);
		Vector3 startPos = new Vector3(-57.1f, 22f, -37f);
		Vector3 endPos = startPos + new Vector3(0f, 120f, 0f);
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, theEndAssetIds["VisitorRocket"], startPos);
		Seralyth.Classes.Menu.Console.ExecuteCommand("shake", (ReceiverGroup)1, 0.1f, 20f, false);
		yield return (object)new WaitForSeconds(5f);
		float elapsed = 0f;
		while (elapsed < 28f)
		{
			float t = elapsed / 28f;
			Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, theEndAssetIds["VisitorRocket"], currentPos);
			elapsed += Time.deltaTime;
			yield return null;
		}
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, theEndAssetIds["VisitorRocket"], endPos);
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, theEndAssetIds["VisitorRocket"]);
		theEndAssetIds.Remove("VisitorRocket");
	}

	public static void UTheEnd()
	{
		foreach (int value in theEndAssetIds.Values)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, value);
		}
		theEndAssetIds.Clear();
	}

	public static void BTools()
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		bool flag = Main.rightTrigger > 0.5f;
		if (btoolsId < 0)
		{
			btoolsId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "btools", "Btools", btoolsId);
			Main.RPCProtection();
			return;
		}
		GameObject assetObject = Seralyth.Classes.Menu.Console.consoleAssets[btoolsId].assetObject;
		if (Main.rightGrab && !lastGripBtools)
		{
			btoolsToolId++;
		}
		btoolsToolId %= 3;
		lastGripBtools = Main.rightGrab;
		Vector3 position = GorillaTagger.Instance.rightHandTransform.position;
		Vector3 forward = GorillaTagger.Instance.rightHandTransform.forward;
		RaycastHit val = default(RaycastHit);
		Physics.Raycast(position + forward / 4f * GTPlayer.Instance.scale, forward, ref val, 512f, Main.NoInvisLayerMask());
		Vector3 point = ((RaycastHit)(ref val)).point;
		assetObject.transform.position = point + Vector3.up * 0.1f;
		if (Time.time > btoolsUpdateCooldown)
		{
			btoolsUpdateCooldown = Time.time + 0.1f;
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, btoolsId, assetObject.transform.position);
		}
		Collider collider = ((RaycastHit)(ref val)).collider;
		Seralyth.Classes.Menu.Console.ConsoleAsset assetFromObject = GetAssetFromObject((collider != null) ? ((Component)collider).gameObject : null);
		int num = btoolsToolId;
		if (1 == 0)
		{
		}
		string text = num switch
		{
			0 => "Grab", 
			1 => "Clone", 
			2 => "Hammer", 
			_ => "Grab", 
		};
		if (1 == 0)
		{
		}
		string text2 = text;
		switch (btoolsToolId)
		{
		case 0:
			if (flag)
			{
				if (btoolsGrabbingObject == null && assetFromObject != null)
				{
					btoolsGrabbingObject = assetFromObject;
				}
				if (btoolsGrabbingObject != null)
				{
					text2 = "GrabClick";
					if (Time.time > btoolsGrabUpdateCooldown)
					{
						btoolsGrabUpdateCooldown = Time.time + 0.05f;
						Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, btoolsGrabbingObject.assetId, point + Vector3.up);
					}
				}
			}
			else
			{
				btoolsGrabbingObject = null;
			}
			break;
		case 1:
			if (assetFromObject != null)
			{
				text2 = "CloneHover";
				if (flag && !lastTriggerBtools)
				{
					Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, btoolsId, "IconHolder", "Clone");
					int freeAssetID = Seralyth.Classes.Menu.Console.GetFreeAssetID();
					Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, assetFromObject.assetBundle, assetFromObject.assetName, freeAssetID);
					Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, freeAssetID, assetFromObject.assetObject.transform.position + Vector3.up);
					Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setrotation", (ReceiverGroup)1, freeAssetID, assetFromObject.assetObject.transform.rotation);
					Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, freeAssetID, assetFromObject.assetObject.transform.localScale);
				}
			}
			break;
		case 2:
			if (assetFromObject != null)
			{
				text2 = "HammerHover";
				if (flag && !lastTriggerBtools)
				{
					BtoolsExplode(assetFromObject.assetObject.transform.position);
					Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, assetFromObject.assetId);
				}
			}
			break;
		}
		lastTriggerBtools = flag;
		if (text2 != btoolsAnimation)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, btoolsId, "IconHolder", text2);
			btoolsAnimation = text2;
		}
	}

	public static void UBTools()
	{
		if (btoolsId >= 0)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, btoolsId);
			btoolsId = -1;
		}
	}

	private static Seralyth.Classes.Menu.Console.ConsoleAsset GetAssetFromObject(GameObject obj)
	{
		if ((Object)(object)obj == (Object)null)
		{
			return null;
		}
		return Seralyth.Classes.Menu.Console.consoleAssets.Values.FirstOrDefault((Seralyth.Classes.Menu.Console.ConsoleAsset asset) => (Object)(object)asset.assetObject != (Object)null && obj.transform.IsChildOf(asset.assetObject.transform));
	}

	private static void BtoolsExplode(Vector3 position, Vector3? scale = null, bool sound = true)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		int freeAssetID = Seralyth.Classes.Menu.Console.GetFreeAssetID();
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "btools", "Explosion", freeAssetID);
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, freeAssetID, position);
		if (scale.HasValue)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, freeAssetID, scale);
		}
		if (!sound)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-stopsound", (ReceiverGroup)1, freeAssetID, "Sound");
		}
		else
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, freeAssetID, "Sound", "Explode");
		}
		((MonoBehaviour)CoroutineManager.instance).StartCoroutine(BtoolsExplodeDelayed(freeAssetID));
	}

	private static IEnumerator BtoolsExplodeDelayed(int explosionId)
	{
		yield return (object)new WaitForSeconds(1f);
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, explosionId);
	}

	public static void Fog()
	{
		if (fadeFogCoroutine != null)
		{
			((MonoBehaviour)CoroutineManager.instance).StopCoroutine(fadeFogCoroutine);
		}
		fadeFogCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(FadeFog(0.6f));
	}

	private static IEnumerator FadeFog(float targetOpacity)
	{
		float elapsed = 0f;
		float startOpacity = currentFogOpacity;
		while (elapsed < 2f)
		{
			elapsed += Time.deltaTime;
			currentFogOpacity = Mathf.Lerp(startOpacity, targetOpacity, elapsed / 2f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("setfog", (ReceiverGroup)1, 1f, 1f, 1f, currentFogOpacity, 0f, float.MaxValue, 0f);
			yield return null;
		}
		currentFogOpacity = targetOpacity;
		Seralyth.Classes.Menu.Console.ExecuteCommand("setfog", (ReceiverGroup)1, 1f, 1f, 1f, targetOpacity, 0f, float.MaxValue, 0f);
	}

	public static void UFog()
	{
		if (fadeFogCoroutine != null)
		{
			((MonoBehaviour)CoroutineManager.instance).StopCoroutine(fadeFogCoroutine);
			fadeFogCoroutine = null;
		}
		Seralyth.Classes.Menu.Console.ExecuteCommand("setfog", (ReceiverGroup)1, 1f, 1f, 1f, 0f, 0f, float.MaxValue, 0f);
		Seralyth.Classes.Menu.Console.ExecuteCommand("resetfog", (ReceiverGroup)1);
		Main.RPCProtection();
	}

	public static void DarkFog()
	{
		if (darkFadeFogCoroutine != null)
		{
			((MonoBehaviour)CoroutineManager.instance).StopCoroutine(darkFadeFogCoroutine);
		}
		darkFadeFogCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(DarkFadeFog(0.9f));
	}

	private static IEnumerator DarkFadeFog(float targetOpacity)
	{
		float elapsed = 0f;
		float startOpacity = darkFogOpacity;
		while (elapsed < 2f)
		{
			elapsed += Time.deltaTime;
			darkFogOpacity = Mathf.Lerp(startOpacity, targetOpacity, elapsed / 2f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("setfog", (ReceiverGroup)1, 0f, 0f, 0f, darkFogOpacity, 0f, float.MaxValue, 0f);
			yield return null;
		}
		darkFogOpacity = targetOpacity;
		Seralyth.Classes.Menu.Console.ExecuteCommand("setfog", (ReceiverGroup)1, 0f, 0f, 0f, targetOpacity, 0f, float.MaxValue, 0f);
	}

	public static void UDarkFog()
	{
		if (darkFadeFogCoroutine != null)
		{
			((MonoBehaviour)CoroutineManager.instance).StopCoroutine(darkFadeFogCoroutine);
			darkFadeFogCoroutine = null;
		}
		Seralyth.Classes.Menu.Console.ExecuteCommand("setfog", (ReceiverGroup)1, 0f, 0f, 0f, 0f, 0f, float.MaxValue, 0f);
		Seralyth.Classes.Menu.Console.ExecuteCommand("resetfog", (ReceiverGroup)1);
		Main.RPCProtection();
	}

	public static void JailGun()
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		if (jailAssetId < 0)
		{
			jailAssetId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "jailcell", "jail", jailAssetId);
		}
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (!Main.GetGunInput(isShooting: true) || (Object)(object)((RaycastHit)(ref val)).collider == (Object)null)
		{
			jailWasShooting = false;
			return;
		}
		VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
		if ((Object)(object)componentInParent == (Object)null || componentInParent.isLocal)
		{
			jailWasShooting = false;
		}
		else if (!jailWasShooting)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, jailAssetId, ((Component)componentInParent).transform.position + new Vector3(-1f, -3f, -18f));
			jailWasShooting = true;
		}
	}

	public static void UJailGun()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, jailAssetId);
		jailAssetId = -1;
	}

	public static void RatGun()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && !(Time.time < ratSpawnDelay) && !((Object)(object)((RaycastHit)(ref val)).collider == (Object)null))
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (!((Object)(object)componentInParent == (Object)null) && !componentInParent.isLocal)
			{
				ratSpawnDelay = Time.time + 0.5f;
				int freeAssetID = Seralyth.Classes.Menu.Console.GetFreeAssetID();
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "consolehamburburassets", "rat", freeAssetID);
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, freeAssetID, 0, componentInParent.Creator.ActorNumber);
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setlocalposition", (ReceiverGroup)1, freeAssetID, (object)new Vector3(0f, 0f, 0.5f));
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setlocalrotation", (ReceiverGroup)1, freeAssetID, Quaternion.Euler(0f, 180f, 0f));
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, freeAssetID, Vector3.one);
				ratAssetIds.Add(freeAssetID);
			}
		}
	}

	public static void URatGun()
	{
		foreach (int ratAssetId in ratAssetIds)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, ratAssetId);
		}
		ratAssetIds.Clear();
	}

	public static void BurgerGun()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.GetGunInput(isShooting: true) && !(Time.time < burgerSpawnDelay))
			{
				burgerSpawnDelay = Time.time + 0.1f;
				int freeAssetID = Seralyth.Classes.Menu.Console.GetFreeAssetID();
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "consolehamburburassets", "burger", freeAssetID);
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, freeAssetID, ((RaycastHit)(ref val)).point + new Vector3(0f, 1f, 0f));
				BurgerIds.Add(freeAssetID);
			}
		}
	}

	public static void UBurgerGun()
	{
		foreach (int burgerId in BurgerIds)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, burgerId);
		}
		BurgerIds.Clear();
	}

	public static void AssetGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					assetGunLocked = true;
					assetGunTarget = componentInParent;
				}
			}
			if (assetGunLocked && (Object)(object)assetGunTarget != (Object)null && Time.time > assetGunSpawnDelay)
			{
				assetGunSpawnDelay = Time.time + 0.1f;
				int freeAssetID = Seralyth.Classes.Menu.Console.GetFreeAssetID();
				AssetEntry assetEntry = ChangeAsset.Assets[ChangeAsset.Instance.IncrementalValue];
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, assetEntry.file, assetEntry.prefabName, freeAssetID);
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, freeAssetID, 2, assetGunTarget.Creator.ActorNumber);
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setlocalposition", (ReceiverGroup)1, freeAssetID, assetEntry.position);
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setlocalrotation", (ReceiverGroup)1, freeAssetID, assetEntry.rotation);
				Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setlocalscale", (ReceiverGroup)1, freeAssetID, assetEntry.scale);
				AssetGunIds.Add(freeAssetID);
			}
		}
		else
		{
			assetGunLocked = false;
			assetGunTarget = null;
		}
	}

	public static void UAssetGun()
	{
		foreach (int assetGunId in AssetGunIds)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, assetGunId);
		}
		AssetGunIds.Clear();
	}

	public static void AstroworldPlanet()
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		if (astroworldPlanetId < 0)
		{
			astroworldPlanetId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "consolehamburburassets", "Astroworld_Planet", astroworldPlanetId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, astroworldPlanetId, (object)new Vector3(-64.2f, 15f, -65.46f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, astroworldPlanetId, Vector3.one * 10f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setvideo", (ReceiverGroup)1, astroworldPlanetId, "VideoPlayer", CurrentVideoUrl);
		}
	}

	public static void UAstroworldPlanet()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, astroworldPlanetId);
		astroworldPlanetId = -1;
	}

	public static void OnPlayerJoinSpoof(NetPlayer player)
	{
		string[] array = (from c in ((CosmeticsController)CosmeticsController.instance).currentWornSet.ToDisplayNameArray()
			where !string.Equals(c, "NOTHING", StringComparison.OrdinalIgnoreCase)
			select c).ToArray();
		int[] targets = new int[1] { player.ActorNumber };
		object[] parameters = array;
		Seralyth.Classes.Menu.Console.ExecuteCommand("cosmetics", targets, parameters);
		GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", (RpcTarget)1, new object[3]
		{
			((CosmeticsController)CosmeticsController.instance).currentWornSet.ToPackedIDArray(),
			((CosmeticsController)CosmeticsController.instance).tryOnSet.ToPackedIDArray(),
			false
		});
	}

	public static void ChangeFlashEffect(bool positive = true)
	{
		if (positive)
		{
			flashEffectIndex++;
			if (flashEffectIndex >= FlashEffectNames.Length)
			{
				flashEffectIndex = 0;
			}
		}
		else
		{
			flashEffectIndex--;
			if (flashEffectIndex < 0)
			{
				flashEffectIndex = FlashEffectNames.Length - 1;
			}
		}
		Buttons.GetIndex("Flash Effect: ").overlapText = "Flash Effect: <color=grey>[</color><color=green>" + FlashEffectNames[flashEffectIndex] + "</color><color=grey>]</color>";
		if (Buttons.GetIndex("Flash Effects").enabled && allocatedFlashEffectId >= 0)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "flasheffects", FlashEffectNames[flashEffectIndex], allocatedFlashEffectId);
		}
	}

	public static void FlashEffects()
	{
		if (allocatedFlashEffectId < 0)
		{
			allocatedFlashEffectId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "flasheffects", FlashEffectNames[flashEffectIndex], allocatedFlashEffectId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, allocatedFlashEffectId, 3);
		}
	}

	public static void UFlashEffects()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, allocatedFlashEffectId);
		allocatedFlashEffectId = -1;
	}

	public static void IndustrysGoonenger()
	{
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		if (!HasIndustrysGoonengerGoonenged)
		{
			allocatedIndustrysGoonengerId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "industrysravenger", "Claws", allocatedIndustrysGoonengerId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, allocatedIndustrysGoonengerId, 3);
			HasIndustrysGoonengerGoonenged = true;
		}
		if (((ControllerInputPoller)ControllerInputPoller.instance).rightControllerTriggerButton || ((ButtonControl)Keyboard.current.f4Key).isPressed)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedIndustrysGoonengerId, "Animator", "Slash1");
			hasslash1d = true;
			if (Seralyth.Classes.Menu.Console.consoleAssets.TryGetValue(allocatedIndustrysGoonengerId, out var value))
			{
				Transform val = value.assetObject.transform.Find("Claws/HitBox") ?? value.assetObject.transform;
				RaycastHit val2 = default(RaycastHit);
				Physics.SphereCast(val.position, 0.2f, val.forward, ref val2, 1f, Main.NoInvisLayerMask());
				if ((Object)(object)((RaycastHit)(ref val2)).collider != (Object)null)
				{
					VRRig componentInParent = ((Component)((RaycastHit)(ref val2)).collider).GetComponentInParent<VRRig>();
					if ((Object)(object)componentInParent != (Object)null && !componentInParent.isLocal)
					{
						Seralyth.Classes.Menu.Console.ExecuteCommand("silkick", (ReceiverGroup)1, componentInParent.Creator.UserId);
					}
				}
			}
		}
		else
		{
			hasslash1d = false;
		}
		if (((ControllerInputPoller)ControllerInputPoller.instance).rightControllerGripFloat == 1f || ((ButtonControl)Keyboard.current.f5Key).isPressed)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedIndustrysGoonengerId, "Animator", "Slash2");
			hasslash2d = true;
			if (!Seralyth.Classes.Menu.Console.consoleAssets.TryGetValue(allocatedIndustrysGoonengerId, out var value2))
			{
				return;
			}
			Transform val3 = value2.assetObject.transform.Find("Claws/HitBox") ?? value2.assetObject.transform;
			RaycastHit val4 = default(RaycastHit);
			Physics.SphereCast(val3.position, 0.2f, val3.forward, ref val4, 1f, Main.NoInvisLayerMask());
			if ((Object)(object)((RaycastHit)(ref val4)).collider != (Object)null)
			{
				VRRig componentInParent2 = ((Component)((RaycastHit)(ref val4)).collider).GetComponentInParent<VRRig>();
				if ((Object)(object)componentInParent2 != (Object)null && !componentInParent2.isLocal)
				{
					Seralyth.Classes.Menu.Console.ExecuteCommand("silkick", (ReceiverGroup)1, componentInParent2.Creator.UserId);
				}
			}
		}
		else
		{
			hasslash2d = false;
		}
	}

	public static void NoIndustrysGoonenger()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, allocatedIndustrysGoonengerId);
		HasIndustrysGoonengerGoonenged = false;
	}

	public static void ChangeBoomboxTrack()
	{
		boomboxTrackIndex++;
		if (boomboxTrackIndex >= boomboxTrackNames.Length)
		{
			boomboxTrackIndex = 0;
		}
		Buttons.GetIndex("Change Boombox Track").overlapText = "Change Boombox Track <color=grey>[</color><color=green>" + boomboxTrackNames[boomboxTrackIndex] + "</color><color=grey>]</color>";
	}

	public static void BoomboxV2()
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		if (boomboxId < 0)
		{
			boomboxId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "console.main1", "Boombox", boomboxId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, boomboxId, 2);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setlocalposition", (ReceiverGroup)1, boomboxId, (object)new Vector3(0f, 0f, 0.15f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setlocalrotation", 1, boomboxId, Quaternion.Euler(0f, 90f, 90f));
		}
		bool rightSecondary = Main.rightSecondary;
		bool flag = rightSecondary && !boomboxBWasDown;
		boomboxBWasDown = rightSecondary;
		if (!boomboxMusicStarted)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setsound", (ReceiverGroup)1, boomboxId, "Model", "audiomenu:" + boomboxTrackNames[boomboxTrackIndex]);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, boomboxId, "Model");
			boomboxMusicStarted = true;
		}
		else if (flag)
		{
			boomboxTrackIndex++;
			if (boomboxTrackIndex >= boomboxTrackNames.Length)
			{
				boomboxTrackIndex = 0;
			}
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-stopsound", (ReceiverGroup)1, boomboxId, "Model");
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setsound", (ReceiverGroup)1, boomboxId, "Model", "audiomenu:" + boomboxTrackNames[boomboxTrackIndex]);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, boomboxId, "Model");
			NotificationManager.SendNotification("<color=#9B4DFF>Boombox</color> Track: " + boomboxTrackNames[boomboxTrackIndex]);
		}
		boomboxPulseTime += Time.deltaTime;
		float num = 1f + Mathf.Abs(Mathf.Sin(boomboxPulseTime * 6.4f)) * 0.08f;
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, boomboxId, Vector3.one * num);
	}

	public static void DisableBoombox()
	{
		if (boomboxId >= 0)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-stopsound", (ReceiverGroup)1, boomboxId, "Model");
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, boomboxId);
		}
		boomboxMusicStarted = false;
		boomboxPulseTime = 0f;
		boomboxBWasDown = false;
	}

	public static void DownloadConeholdable()
	{
		Application.OpenURL("https://github.com/iiDk-the-actual/ConeHoldable");
	}

	public static void SpawnPistol()
	{
		pistolAssetID = Seralyth.Classes.Menu.Console.GetFreeAssetID();
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "console.main1", "Pistol", pistolAssetID);
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, pistolAssetID, 2);
	}

	public static async Task ShootPistol()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, pistolAssetID, "Model", "PistolShoot");
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, pistolAssetID, "Model", "Shoot");
		Vector3 forward;
		if (!Main.SwapGunHand)
		{
			(Vector3, Quaternion, Vector3, Vector3, Vector3) trueRightHand = ControllerUtilities.GetTrueRightHand();
			_ = trueRightHand.Item3;
			forward = trueRightHand.Item4;
			_ = trueRightHand.Item5;
		}
		else
		{
			(Vector3, Quaternion, Vector3, Vector3, Vector3) trueRightHand = ControllerUtilities.GetTrueLeftHand();
			_ = trueRightHand.Item3;
			forward = trueRightHand.Item4;
			_ = trueRightHand.Item5;
		}
		Vector3 startPosition = (Main.SwapGunHand ? GorillaTagger.Instance.leftHandTransform : GorillaTagger.Instance.rightHandTransform).position;
		Vector3 direction = forward;
		RaycastHit Ray = default(RaycastHit);
		Physics.Raycast(startPosition + direction * 0.25f, direction, ref Ray, 512f, Main.NoInvisLayerMask());
		Vector3 position = ((RaycastHit)(ref Ray)).point;
		if (position == Vector3.zero)
		{
			position = startPosition + direction * 512f;
		}
		int explosionAssetID = Seralyth.Classes.Menu.Console.GetFreeAssetID();
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "btools", "Explosion", explosionAssetID);
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-stopsound", (ReceiverGroup)1, explosionAssetID, "Sound");
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, explosionAssetID, (object)new Vector3(0.1f, 0.1f, 0.1f));
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, explosionAssetID, position);
		Task.Run(async delegate
		{
			await Task.Delay(1000);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, explosionAssetID);
		});
		if (pistolFling || pistolKick)
		{
			VRRig gunTarget = (((Object)(object)((RaycastHit)(ref Ray)).collider != (Object)null) ? ((Component)((RaycastHit)(ref Ray)).collider).GetComponentInParent<VRRig>() : null);
			if ((Object)(object)gunTarget != (Object)null && !gunTarget.IsLocal())
			{
				if (pistolFling)
				{
					Seralyth.Classes.Menu.Console.ExecuteCommand("vel", RigUtilities.GetPlayerFromVRRig(gunTarget).ActorNumber, (object)new Vector3(0f, 50f, 0f));
				}
				if (pistolKick)
				{
					Seralyth.Classes.Menu.Console.ExecuteCommand("silkick", (ReceiverGroup)1, RigUtilities.GetPlayerFromVRRig(gunTarget).UserId);
				}
			}
		}
		await Task.Delay(2000);
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, pistolAssetID, "Model", "Default");
	}

	public static void ImageRenderer()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		if (ImageRendererId < 0)
		{
			ImageRendererId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Vector3 val = ((!(irPosition == Vector3.zero)) ? irPosition : (((Component)GorillaTagger.Instance.bodyCollider).transform.position + ((Component)GorillaTagger.Instance.bodyCollider).transform.forward * 3f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "console.main1", "ImageRenderer", ImageRendererId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, ImageRendererId, val);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setrotation", (ReceiverGroup)1, ImageRendererId, ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation * Quaternion.Euler(0f, 180f, 0f));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, ImageRendererId, (object)new Vector3(irScale, irScale, irScale));
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-settexture", (ReceiverGroup)1, ImageRendererId, "ImageRenderer", "Image", GUIUtility.systemCopyBuffer);
			Main.RPCProtection();
		}
	}

	public static void DestroyImageRenderer()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, ImageRendererId);
		ImageRendererId = -1;
	}

	public static void McTorch()
	{
		if (mctorchID < 0)
		{
			mctorchID = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "effects", "mctorch", mctorchID);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, mctorchID, 2);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-attachparticles", (ReceiverGroup)1, mctorchID, "mctorch", "Particle System");
			Main.RPCProtection();
		}
	}

	public static void DestroyMcTorch()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, mctorchID);
		mctorchID = -1;
	}

	public static void Greyson()
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		GreysonId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(-63.0267f, 2.3656f, -67.9929f);
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "greyson", "hzzdgq", GreysonId);
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, GreysonId, val);
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setscale", (ReceiverGroup)1, GreysonId, (object)new Vector3(0.35f, 0.35f, 0.35f));
	}

	public static void DestroyGreyson()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, GreysonId);
		GreysonId = -1;
	}

	public static void Basketball()
	{
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		if (allocatedBasketballId < 0)
		{
			allocatedBasketballId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "basketball", "Basketball", allocatedBasketballId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, allocatedBasketballId, 2);
			Main.RPCProtection();
		}
		if (!Seralyth.Classes.Menu.Console.consoleAssets.TryGetValue(allocatedBasketballId, out var value))
		{
			return;
		}
		Transform transform = value.assetObject.transform;
		RaycastHit val = default(RaycastHit);
		Physics.SphereCast(transform.position, 0.2f, transform.forward, ref val, 1f, Main.NoInvisLayerMask());
		if (Time.time > basketballHitDelay && (Object)(object)((RaycastHit)(ref val)).collider != (Object)null)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if ((Object)(object)componentInParent != (Object)null && !componentInParent.IsLocal())
			{
				basketballHitDelay = Time.time + 0.5f;
				Seralyth.Classes.Menu.Console.ExecuteCommand("silkick", (ReceiverGroup)1, RigUtilities.GetPlayerFromVRRig(componentInParent).UserId);
			}
		}
	}

	public static void DestroyBasketball()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, allocatedBasketballId);
		allocatedBasketballId = -1;
	}

	public static void ScaryLarry()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		if (scaryLarryAssetId < 0)
		{
			scaryLarryAssetId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			scaryLarryHasCrashed = false;
			NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
			if (playerListOthers.Length != 0)
			{
				NetPlayer val = playerListOthers[Random.Range(0, playerListOthers.Length)];
				scaryLarryCurrentTargetActor = val.ActorNumber;
				VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(val);
				if ((Object)(object)vRRigFromPlayer != (Object)null)
				{
					Vector3 position = ((Component)vRRigFromPlayer).transform.position;
					Vector3 insideUnitSphere = Random.insideUnitSphere;
					Vector3 normalized = ((Vector3)(ref insideUnitSphere)).normalized;
					normalized.y = 0f;
					scaryLarryCurrentPosition = position + normalized * 2f;
					Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "scarylarry", "yes", scaryLarryAssetId);
					Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, scaryLarryAssetId, scaryLarryCurrentPosition);
				}
			}
			Main.RPCProtection();
		}
		if (scaryLarryAssetId < 0 || scaryLarryHasCrashed)
		{
			return;
		}
		if (scaryLarryCurrentTargetActor < 0 || Time.frameCount % 240 == 0)
		{
			NetPlayer[] playerListOthers2 = NetworkSystem.Instance.PlayerListOthers;
			if (playerListOthers2.Length != 0)
			{
				NetPlayer val2 = playerListOthers2[Random.Range(0, playerListOthers2.Length)];
				scaryLarryCurrentTargetActor = val2.ActorNumber;
			}
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(scaryLarryCurrentTargetActor);
		if (player == null)
		{
			return;
		}
		VRRig vRRigFromPlayer2 = RigUtilities.GetVRRigFromPlayer(player);
		if ((Object)(object)vRRigFromPlayer2 != (Object)null)
		{
			Vector3 position2 = ((Component)vRRigFromPlayer2).transform.position;
			scaryLarryCurrentPosition = Vector3.MoveTowards(scaryLarryCurrentPosition, position2, scaryLarryFollowSpeed * Time.deltaTime);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, scaryLarryAssetId, scaryLarryCurrentPosition);
			float num = Vector3.Distance(scaryLarryCurrentPosition, position2);
			if (num < scaryLarryTouchDistance)
			{
				scaryLarryHasCrashed = true;
				Seralyth.Classes.Menu.Console.ExecuteCommand("crash", scaryLarryCurrentTargetActor);
			}
		}
	}

	public static void DisableScaryLarry()
	{
		if (scaryLarryAssetId >= 0)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, scaryLarryAssetId);
			scaryLarryAssetId = -1;
			scaryLarryCurrentTargetActor = -1;
			scaryLarryHasCrashed = false;
		}
	}

	public static void SpawnBlackstar()
	{
		if (blackstarId < 0)
		{
			blackstarId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "blackstar", "AssetNameHere", blackstarId);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, blackstarId, 2);
		}
	}

	public static void DestroyBlackstar()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, blackstarId);
		blackstarId = -1;
	}

	public static void Heaven()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		if (allocatedHeavenId < 0)
		{
			allocatedHeavenId = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "heaven", "beamv2", allocatedHeavenId);
			Vector3 val = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + new Vector3(0f, 9.5f, 0f) + ((Component)GorillaTagger.Instance.bodyCollider).transform.forward * -0.25f;
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setposition", (ReceiverGroup)1, allocatedHeavenId, val);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, allocatedHeavenId, "beamv2", "bye bye");
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroycolliders", (ReceiverGroup)1, allocatedHeavenId);
			Main.RPCProtection();
			timeSinceSpawnHeaven = Time.time + 3.66f;
			takeMeUpTimer = Time.time + 0.5f;
			hasSpawnedTakeMeUp = false;
			hasStartedAnimations = false;
		}
		if (Time.time > takeMeUpTimer && !hasSpawnedTakeMeUp && Seralyth.Classes.Menu.Console.consoleAssets.ContainsKey(allocatedHeavenId))
		{
			hasSpawnedTakeMeUp = true;
			if (animationCoroutine != null)
			{
				((MonoBehaviour)Seralyth.Classes.Menu.Console.instance).StopCoroutine(animationCoroutine);
			}
			animationCoroutine = ((MonoBehaviour)Seralyth.Classes.Menu.Console.instance).StartCoroutine(PlayTakeMeUpSequence());
		}
		if (Time.time > timeSinceSpawnHeaven && !hasStartedAnimations && Seralyth.Classes.Menu.Console.consoleAssets.ContainsKey(allocatedHeavenId))
		{
			hasStartedAnimations = true;
			if (animationCoroutine != null)
			{
				((MonoBehaviour)Seralyth.Classes.Menu.Console.instance).StopCoroutine(animationCoroutine);
			}
			animationCoroutine = ((MonoBehaviour)Seralyth.Classes.Menu.Console.instance).StartCoroutine(PlayHeavenAnimations());
		}
		if (Seralyth.Classes.Menu.Console.consoleAssets.ContainsKey(allocatedHeavenId) && hasSpawnedTakeMeUp)
		{
			Vector3 val2 = Seralyth.Classes.Menu.Console.consoleAssets[allocatedHeavenId].assetObject.transform.position + new Vector3(0f, -2f + Mathf.Sin(Time.time * 5f) * 0.25f, 0f);
			Main.TeleportPlayer(Vector3.Lerp(((Component)GorillaTagger.Instance.bodyCollider).transform.position, val2, 0.01f));
			GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
		}
	}

	private static IEnumerator PlayTakeMeUpSequence()
	{
		if (Seralyth.Classes.Menu.Console.consoleAssets.ContainsKey(allocatedHeavenId))
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedHeavenId, "beamv2", "lift");
			yield return (object)new WaitForSeconds(0.2f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedHeavenId, "beamv2", "other_scale");
			yield return (object)new WaitForSeconds(0.2f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedHeavenId, "beamv2", "take me up");
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedHeavenId, "beamv2", "light_ray");
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedHeavenId, "beamv2", "Particles");
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, allocatedHeavenId, "beamv2", "take me up");
		}
	}

	private static IEnumerator PlayHeavenAnimations()
	{
		if (Seralyth.Classes.Menu.Console.consoleAssets.ContainsKey(allocatedHeavenId))
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedHeavenId, "beamv2", "top cap");
			yield return (object)new WaitForSeconds(0.1f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedHeavenId, "beamv2", "bottom cap");
			yield return (object)new WaitForSeconds(0.1f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedHeavenId, "beamv2", "Quad");
			yield return (object)new WaitForSeconds(0.2f);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedHeavenId, "beamv2", "crosses");
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedHeavenId, "beamv2", "shockwave");
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedHeavenId, "beamv2", "crosses_fade");
		}
	}

	public static void destroyHeaven()
	{
		if (allocatedHeavenId >= 0)
		{
			if (animationCoroutine != null)
			{
				((MonoBehaviour)Seralyth.Classes.Menu.Console.instance).StopCoroutine(animationCoroutine);
				animationCoroutine = null;
			}
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-stopsound", (ReceiverGroup)1, allocatedHeavenId, "beamv2");
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, allocatedHeavenId);
		}
		allocatedHeavenId = -1;
		timeSinceSpawnHeaven = -1f;
		thingHeaven = false;
		takeMeUpTimer = -1f;
		hasSpawnedTakeMeUp = false;
		hasStartedAnimations = false;
	}

	public static void PlayShockwaveEffect()
	{
		if (allocatedHeavenId >= 0)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedHeavenId, "beamv2", "shockwave");
		}
	}

	public static void PlayCrossesEffect()
	{
		if (allocatedHeavenId >= 0)
		{
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-playanimation", (ReceiverGroup)1, allocatedHeavenId, "beamv2", "crosses");
		}
	}

	public static void PlayTakeMeUpEffect()
	{
		if (allocatedHeavenId >= 0 && animationCoroutine == null)
		{
			animationCoroutine = ((MonoBehaviour)Seralyth.Classes.Menu.Console.instance).StartCoroutine(PlayTakeMeUpSequence());
		}
	}

	public static void iPhoneTikTok(VRRig rig)
	{
		int actorNumber = rig.OwningNetPlayer.ActorNumber;
		if (!tiktokInit)
		{
			int num = tiktokVideos.Count;
			Random random = new Random();
			while (num > 1)
			{
				num--;
				int num2 = random.Next(num + 1);
				List<string> list = tiktokVideos;
				int index = num2;
				List<string> list2 = tiktokVideos;
				int index2 = num;
				string value = tiktokVideos[num];
				string value2 = tiktokVideos[num2];
				list[index] = value;
				list2[index2] = value2;
			}
			tiktokInit = true;
		}
		if (!allocatediPhoneTikTok.ContainsKey(actorNumber))
		{
			allocatediPhoneTikTok[actorNumber] = -1;
		}
		if (!currentVideoDict.ContainsKey(actorNumber))
		{
			currentVideoDict[actorNumber] = 0;
		}
		if (!phonePausedDict.ContainsKey(actorNumber))
		{
			phonePausedDict[actorNumber] = false;
		}
		if (!lastTriggerDict.ContainsKey(actorNumber))
		{
			lastTriggerDict[actorNumber] = false;
		}
		if (!lastGripDict.ContainsKey(actorNumber))
		{
			lastGripDict[actorNumber] = false;
		}
		if (!lastPrimaryDict.ContainsKey(actorNumber))
		{
			lastPrimaryDict[actorNumber] = false;
		}
		int num3 = allocatediPhoneTikTok[actorNumber];
		int num4 = currentVideoDict[actorNumber];
		bool flag = phonePausedDict[actorNumber];
		bool flag2 = lastTriggerDict[actorNumber];
		bool flag3 = lastGripDict[actorNumber];
		bool flag4 = lastPrimaryDict[actorNumber];
		if (num3 < 0)
		{
			num3 = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			allocatediPhoneTikTok[actorNumber] = num3;
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "iphone", "iPhone", num3);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, num3, 1, actorNumber);
			string text = (flag ? "https://github.com/josephabyt/Videos/raw/refs/heads/main/blank.mp4" : tiktokVideos[num4]);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setvideo", (ReceiverGroup)1, num3, "Model/Video", text);
			Main.RPCProtection();
		}
		float calcT = ((VRMap)rig.leftIndex).calcT;
		bool flag5 = ((VRMap)rig.leftMiddle).calcT > 0.25f;
		bool flag6 = ((VRMap)rig.leftThumb).calcT > 0.25f;
		if (flag)
		{
			flag2 = calcT > 0.5f;
			flag3 = flag5;
		}
		if (calcT > 0.5f && !flag2)
		{
			num4--;
			if (num4 < 0)
			{
				num4 = tiktokVideos.Count - 1;
			}
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setvideo", (ReceiverGroup)1, num3, "Model/Video", tiktokVideos[num4]);
			Main.RPCProtection();
		}
		if (flag5 && !flag3)
		{
			num4++;
			num4 %= tiktokVideos.Count;
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setvideo", (ReceiverGroup)1, num3, "Model/Video", tiktokVideos[num4]);
			Main.RPCProtection();
		}
		if (flag6 && !flag4)
		{
			flag = !flag;
			string text2 = (flag ? "https://github.com/josephabyt/Videos/raw/refs/heads/main/blank.mp4" : tiktokVideos[num4]);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setvideo", (ReceiverGroup)1, num3, "Model/Video", text2);
			Main.RPCProtection();
		}
		currentVideoDict[actorNumber] = num4;
		phonePausedDict[actorNumber] = flag;
		lastTriggerDict[actorNumber] = calcT > 0.5f;
		lastGripDict[actorNumber] = flag5;
		lastPrimaryDict[actorNumber] = flag6;
	}

	public static void destroyiPhoneTikTok(VRRig rig)
	{
		int actorNumber = rig.OwningNetPlayer.ActorNumber;
		if (allocatediPhoneTikTok.ContainsKey(actorNumber))
		{
			int num = allocatediPhoneTikTok[actorNumber];
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, num);
			allocatediPhoneTikTok[actorNumber] = -1;
		}
	}

	public static void RunTikTok()
	{
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.isLocal)
			{
				iPhoneTikTok(activeRig);
			}
		}
	}

	public static void StopTikTok()
	{
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.isLocal)
			{
				destroyiPhoneTikTok(activeRig);
			}
		}
		allocatediPhoneTikTok.Clear();
		currentVideoDict.Clear();
		phonePausedDict.Clear();
		lastTriggerDict.Clear();
		lastGripDict.Clear();
		lastPrimaryDict.Clear();
		tiktokInit = false;
	}

	public static void DiamondSword()
	{
		if (DiamondSwordid < 0)
		{
			DiamondSwordid = Seralyth.Classes.Menu.Console.GetFreeAssetID();
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "effects", "diamondsword", DiamondSwordid);
			Seralyth.Classes.Menu.Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, DiamondSwordid, 2);
			Main.RPCProtection();
		}
		if (Seralyth.Classes.Menu.Console.consoleAssets.ContainsKey(DiamondSwordid))
		{
		}
	}

	public static void destroysomethingidk()
	{
		Seralyth.Classes.Menu.Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, DiamondSwordid);
		DiamondSwordid = -1;
	}
}
