using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using GorillaNetworking;
using GorillaTagScripts;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Ionic.Zlib;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using Seralyth.Extensions;
using Seralyth.Managers;
using Seralyth.Menu;
using Seralyth.Mods.CustomMaps;
using Seralyth.Patches.Menu;
using Seralyth.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Seralyth.Mods;

public static class Overpowered
{
	public static bool muteOnFreeze;

	private static float guardianDelay;

	private static float playerColorDelay;

	private static readonly Dictionary<VRRig, int> materialState = new Dictionary<VRRig, int>();

	public static float materialDelay;

	private static float guardianSpazDelay;

	private static bool guardianSpazToggle;

	public static float alwaysGuardianDelay;

	public static float guardianProtectorDelay;

	private static float crashAllDelay;

	private static float spazDriverDelay;

	private static long? id;

	private static float setMapDelay;

	public const int ItemCrashCount = 500;

	public static int masterVisualizationType;

	private static float reportDelay;

	private static GameObject point;

	public static bool notifyGrab;

	private static float propHuntSpazDelay;

	private static bool propHuntSpazMode;

	public static float ghostReactorDelay;

	public static float throwDelay;

	public static Dictionary<string, bool[][]> Letters = new Dictionary<string, bool[][]>
	{
		{
			"A",
			new bool[5][]
			{
				new bool[5] { false, true, true, true, false },
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, true, true, true, true },
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, false, false, false, true }
			}
		},
		{
			"B",
			new bool[5][]
			{
				new bool[5] { true, true, true, true, false },
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, true, true, true, false },
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, true, true, true, true }
			}
		},
		{
			"C",
			new bool[5][]
			{
				new bool[5] { false, true, true, true, true },
				new bool[5] { true, false, false, false, false },
				new bool[5] { true, false, false, false, false },
				new bool[5] { true, false, false, false, false },
				new bool[5] { false, true, true, true, true }
			}
		},
		{
			"D",
			new bool[5][]
			{
				new bool[5] { true, true, true, true, false },
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, true, true, true, false }
			}
		},
		{
			"E",
			new bool[5][]
			{
				new bool[5] { true, true, true, true, true },
				new bool[5] { true, false, false, false, false },
				new bool[5] { true, true, true, false, false },
				new bool[5] { true, false, false, false, false },
				new bool[5] { true, true, true, true, true }
			}
		},
		{
			"F",
			new bool[5][]
			{
				new bool[5] { true, true, true, true, true },
				new bool[5] { true, false, false, false, false },
				new bool[5] { true, true, true, false, false },
				new bool[5] { true, false, false, false, false },
				new bool[5] { true, false, false, false, false }
			}
		},
		{
			"G",
			new bool[5][]
			{
				new bool[5] { true, true, true, true, true },
				new bool[5] { true, false, false, false, false },
				new bool[5] { true, false, false, true, true },
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, true, true, true, true }
			}
		},
		{
			"H",
			new bool[5][]
			{
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, true, true, true, true },
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, false, false, false, true }
			}
		},
		{
			"I",
			new bool[5][]
			{
				new bool[5] { true, true, true, true, true },
				new bool[5] { false, false, true, false, false },
				new bool[5] { false, false, true, false, false },
				new bool[5] { false, false, true, false, false },
				new bool[5] { true, true, true, true, true }
			}
		},
		{
			"J",
			new bool[5][]
			{
				new bool[5] { false, false, false, false, true },
				new bool[5] { false, false, false, false, true },
				new bool[5] { false, false, false, false, true },
				new bool[5] { true, false, false, false, true },
				new bool[5] { false, true, true, true, false }
			}
		},
		{
			"K",
			new bool[5][]
			{
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, false, false, true, false },
				new bool[5] { true, true, true, false, false },
				new bool[5] { true, false, false, true, false },
				new bool[5] { true, false, false, false, true }
			}
		},
		{
			"L",
			new bool[5][]
			{
				new bool[5] { true, false, false, false, false },
				new bool[5] { true, false, false, false, false },
				new bool[5] { true, false, false, false, false },
				new bool[5] { true, false, false, false, false },
				new bool[5] { true, true, true, true, true }
			}
		},
		{
			"M",
			new bool[5][]
			{
				new bool[5] { true, true, false, true, true },
				new bool[5] { true, false, true, false, true },
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, false, false, false, true }
			}
		},
		{
			"N",
			new bool[5][]
			{
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, true, false, false, true },
				new bool[5] { true, false, true, false, true },
				new bool[5] { true, false, false, true, true },
				new bool[5] { true, false, false, false, true }
			}
		},
		{
			"O",
			new bool[5][]
			{
				new bool[5] { false, true, true, true, false },
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, false, false, false, true },
				new bool[5] { false, true, true, true, false }
			}
		},
		{
			"P",
			new bool[5][]
			{
				new bool[5] { true, true, true, true, false },
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, true, true, true, false },
				new bool[5] { true, false, false, false, false },
				new bool[5] { true, false, false, false, false }
			}
		},
		{
			"Q",
			new bool[5][]
			{
				new bool[5] { false, true, true, true, false },
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, false, true, false, true },
				new bool[5] { true, false, false, true, false },
				new bool[5] { false, true, true, false, true }
			}
		},
		{
			"R",
			new bool[5][]
			{
				new bool[5] { true, true, true, true, true },
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, true, true, true, true },
				new bool[5] { true, false, false, true, false },
				new bool[5] { true, false, false, false, true }
			}
		},
		{
			"S",
			new bool[5][]
			{
				new bool[5] { true, true, true, true, true },
				new bool[5] { true, false, false, false, false },
				new bool[5] { true, true, true, true, true },
				new bool[5] { false, false, false, false, true },
				new bool[5] { true, true, true, true, true }
			}
		},
		{
			"T",
			new bool[5][]
			{
				new bool[5] { true, true, true, true, true },
				new bool[5] { false, false, true, false, false },
				new bool[5] { false, false, true, false, false },
				new bool[5] { false, false, true, false, false },
				new bool[5] { false, false, true, false, false }
			}
		},
		{
			"U",
			new bool[5][]
			{
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, false, false, false, true },
				new bool[5] { false, true, true, true, false }
			}
		},
		{
			"V",
			new bool[5][]
			{
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, false, false, false, true },
				new bool[5] { false, true, false, true, false },
				new bool[5] { false, true, false, true, false },
				new bool[5] { false, false, true, false, false }
			}
		},
		{
			"W",
			new bool[5][]
			{
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, false, false, false, true },
				new bool[5] { true, false, true, false, true },
				new bool[5] { false, true, false, true, false }
			}
		},
		{
			"X",
			new bool[5][]
			{
				new bool[5] { true, false, false, false, true },
				new bool[5] { false, true, false, true, false },
				new bool[5] { false, false, true, false, false },
				new bool[5] { false, true, false, true, false },
				new bool[5] { true, false, false, false, true }
			}
		},
		{
			"Y",
			new bool[5][]
			{
				new bool[5] { true, false, false, false, true },
				new bool[5] { false, true, false, true, false },
				new bool[5] { false, false, true, false, false },
				new bool[5] { false, false, true, false, false },
				new bool[5] { false, false, true, false, false }
			}
		},
		{
			"Z",
			new bool[5][]
			{
				new bool[5] { true, true, true, true, true },
				new bool[5] { false, false, false, true, false },
				new bool[5] { false, false, true, false, false },
				new bool[5] { false, true, false, false, false },
				new bool[5] { true, true, true, true, true }
			}
		},
		{
			".",
			new bool[5][]
			{
				new bool[5],
				new bool[5],
				new bool[5],
				new bool[5],
				new bool[5] { false, false, true, false, false }
			}
		},
		{
			"/",
			new bool[5][]
			{
				new bool[5] { false, false, false, false, true },
				new bool[5] { false, false, false, true, false },
				new bool[5] { false, false, true, false, false },
				new bool[5] { false, true, false, false, false },
				new bool[5] { true, false, false, false, false }
			}
		},
		{
			" ",
			new bool[5][]
			{
				new bool[5],
				new bool[5],
				new bool[5],
				new bool[5],
				new bool[5]
			}
		}
	};

	public static string textToRender;

	public static float textDelay;

	public static int characterIndex;

	public static Vector3? basePosition;

	private static float destroyDelay;

	private static float resourceIncrementDelay;

	public static int? spawnedNetId;

	public static int spawnedFrame;

	public static float blasterDelay;

	public static Coroutine BlasterCoroutine;

	public static readonly Dictionary<NetPlayer, float> perPlayerDictionary = new Dictionary<NetPlayer, float>();

	private static float siErrorCooldown;

	public static HalloweenGhostChaser _lucy;

	public static LurkerGhost _lurker;

	public static float lucyDelay;

	public static float lurkerDelay;

	private static float grabDelay;

	private static float releaseDelay;

	private static float flingDelay;

	private static int archiveIncrement;

	private static float timeSinceCallInvalidated;

	public static int snowballScale = 5;

	public static int snowballMultiplicationFactor = 1;

	public static float _snowballSpawnDelay;

	public static Coroutine DisableCoroutine;

	public static bool SnowballHandIndex;

	public static bool NoTeleportSnowballs;

	public static bool InvisibleSnowballs;

	private static Vector3? snowballNukePosition;

	private static Vector3 snowballNukeVelocity;

	public static GameObject FountainObject;

	private static float wallDelay;

	public static GameObject BombObject;

	private static float rpgDelay;

	private static bool rpgShot;

	private static readonly Dictionary<VRRig, float> boxingDelay = new Dictionary<VRRig, float>();

	public static AudioClip KameStart;

	public static AudioClip KameStop;

	public static VoiceManager.Clip KameSound;

	public static Coroutine KameStartCoroutine;

	public static GameObject cursor;

	public static readonly List<GameObject> flingZones = new List<GameObject>();

	private static float antiReportFlingDelay;

	private static float thingdeb;

	private static float slamDel;

	private static bool flip;

	private static float closeRoomDelay;

	public static float zaWarudoNotificationDelay;

	public static Coroutine ZaWarudo_StartCoroutineVariable;

	public static Coroutine ZaWarudo_EndCoroutineVariable;

	public static AudioClip ZaWarudo_Start;

	public static AudioClip ZaWarudo_Stop;

	private static bool zaWarudoTrigger;

	public static int lagIndex = 1;

	public static int lagAmount;

	public static float lagDelay;

	public static int lagTypeIndex;

	private static float lagDebounce;

	private static float crashDelay;

	private static float barrelAllDelay;

	public const int BarrelIndex = 618;

	private static float throwableProjectileTimeout;

	private static float notifyTime;

	public static float delay;

	public static bool kickToPublic;

	public static bool rejoinOnKick;

	public static string specificRoom;

	private static float kickDelay;

	private static float elevatorKickDelay;

	public static Coroutine kickCoroutine;

	private static float greyZoneDelay;

	private static Coroutine wipeOverride;

	public static float spazGreyDelay;

	public static bool greyState;

	public static Coroutine partyKickDelayCoroutine;

	public static bool previousInParty;

	private static float breakDelay;

	public static bool legacyKickFreeze;

	private static bool _optimizeEvents;

	private static float antiReportLagDelay;

	public static float setMasterDelay;

	private static float rockDebounce;

	private static float slowDelay;

	private static float vibrateDelay;

	public static Coroutine RopeCoroutine;

	private static float randomRopeDelay;

	private static GorillaRopeSwing randomRope;

	private static float RopeDelay;

	private static Coroutine CritterCoroutine;

	private static float critterGrabDelay;

	private static bool didWarnForTransparentRig;

	public static Dictionary<string, int> ObjectByName => ManagerRegistry.GhostReactor.GameEntityManager.itemPrefabFactory.ToDictionary((KeyValuePair<int, GameObject> prefab) => ((Object)prefab.Value).name, (KeyValuePair<int, GameObject> prefab) => prefab.Key);

	public static Dictionary<string, int> GadgetByName => ManagerRegistry.SuperInfection.GameEntityManager.itemPrefabFactory.ToDictionary((KeyValuePair<int, GameObject> prefab) => ((Object)prefab.Value).name, (KeyValuePair<int, GameObject> prefab) => prefab.Key);

	public static HalloweenGhostChaser Lucy
	{
		get
		{
			if (_lucy == null)
			{
				_lucy = Main.GetObject("Environment Objects/05Maze_PersistentObjects/2025_Halloween1_PersistentObjects/Halloween Ghosts/Lucy/Halloween Ghost/FloatingChaseSkeleton").GetComponent<HalloweenGhostChaser>();
			}
			return _lucy;
		}
		set
		{
			_lucy = value;
		}
	}

	public static LurkerGhost Lurker
	{
		get
		{
			if (_lurker == null)
			{
				_lurker = Main.GetObject("Environment Objects/05Maze_PersistentObjects/2025_Halloween1_PersistentObjects/Halloween Ghosts/Lurker Ghost/GhostLurker_Prefab").GetComponent<LurkerGhost>();
			}
			return _lurker;
		}
		set
		{
			_lurker = value;
		}
	}

	public static float SnowballSpawnDelay
	{
		get
		{
			return _snowballSpawnDelay;
		}
		set
		{
			_snowballSpawnDelay = value;
		}
	}

	public static bool OptimizeEvents
	{
		get
		{
			return _optimizeEvents;
		}
		set
		{
			if (_optimizeEvents == value)
			{
				return;
			}
			_optimizeEvents = value;
			if (_optimizeEvents)
			{
				if (legacyKickFreeze)
				{
					SerializePatch.OverrideSerialization = () => false;
					return;
				}
				PhotonNetwork.SerializationRate = 2;
				RPCFilter.FilteredRPCs["OnHandTapRPC"] = () => false;
				RPCFilter.FilteredRPCs["RPC_UpdateCosmeticsWithTryonPacked"] = () => false;
				SerializePatch.OverrideSerialization = delegate
				{
					Main.SendSerialize(VRRig.LocalRig.GetPhotonView());
					return true;
				};
			}
			else
			{
				if (SerializePatch.OverrideSerialization != null)
				{
					SerializePatch.OverrideSerialization = null;
				}
				PhotonNetwork.SerializationRate = 10;
				RPCFilter.FilteredRPCs.Remove("OnHandTapRPC");
				RPCFilter.FilteredRPCs.Remove("RPC_UpdateCosmeticsWithTryonPacked");
			}
		}
	}

	public static void SetGuardianTarget(NetPlayer target)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			return;
		}
		GorillaGuardianManager val = (GorillaGuardianManager)GorillaGameManager.instance;
		if (val.IsPlayerGuardian(target))
		{
			return;
		}
		TappableGuardianIdol[] allType = Main.GetAllType<TappableGuardianIdol>(5f);
		foreach (TappableGuardianIdol val2 in allType)
		{
			if (Object.op_Implicit((Object)(object)((Tappable)val2).manager) && Object.op_Implicit((Object)(object)((NetworkSceneObject)((Tappable)val2).manager).photonView) && !val2.isChangingPositions)
			{
				GorillaGuardianZoneManager zoneManager = val2.zoneManager;
				if (zoneManager.IsZoneValid() && Object.op_Implicit((Object)(object)((Tappable)val2).manager) && zoneManager.CurrentGuardian == null)
				{
					zoneManager.SetGuardian(target);
					break;
				}
			}
		}
	}

	public static void GuardianSelf()
	{
		SetGuardianTarget(NetPlayer.op_Implicit(PhotonNetwork.LocalPlayer));
	}

	private static bool IsLeftHandAvailableForGrab(VRRig rig)
	{
		return ((VRMap)rig.leftMiddle).calcT > 0.8f && rig.leftHandLink.grabbedPlayer == null;
	}

	private static bool IsRightHandAvailableForGrab(VRRig rig)
	{
		return ((VRMap)rig.rightMiddle).calcT > 0.8f && rig.rightHandLink.grabbedPlayer == null;
	}

	public static void GuardianGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > guardianDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				SetGuardianTarget(RigUtilities.GetPlayerFromVRRig(componentInParent));
				guardianDelay = Time.time + 0.1f;
			}
		}
	}

	public static void GuardianAll()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			int num = 0;
			{
				foreach (GorillaGuardianZoneManager item in GorillaGuardianZoneManager.zoneManagers.Where((GorillaGuardianZoneManager gorillaGuardianZoneManager) => ((Behaviour)gorillaGuardianZoneManager).enabled && gorillaGuardianZoneManager.IsZoneValid()))
				{
					item.SetGuardian(NetPlayer.op_Implicit(PhotonNetwork.PlayerList[num]));
					num++;
				}
				return;
			}
		}
		NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
	}

	public static void UnguardianSelf()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			foreach (GorillaGuardianZoneManager item in from gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers
				where ((Behaviour)gorillaGuardianZoneManager).enabled && gorillaGuardianZoneManager.IsZoneValid()
				where gorillaGuardianZoneManager.CurrentGuardian == NetworkSystem.Instance.LocalPlayer
				select gorillaGuardianZoneManager)
			{
				item.SetGuardian((NetPlayer)null);
			}
			return;
		}
		NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
	}

	public static void UnguardianGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (!Main.GetGunInput(isShooting: true) || !(Time.time > guardianDelay))
		{
			return;
		}
		VRRig gunTarget = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
		if (!Object.op_Implicit((Object)(object)gunTarget) || gunTarget.IsLocal())
		{
			return;
		}
		if (NetworkSystem.Instance.IsMasterClient)
		{
			foreach (GorillaGuardianZoneManager item in from gorillaGuardianZoneManager in GorillaGuardianZoneManager.zoneManagers
				where ((Behaviour)gorillaGuardianZoneManager).enabled && gorillaGuardianZoneManager.IsZoneValid()
				where gorillaGuardianZoneManager.CurrentGuardian == RigUtilities.GetPlayerFromVRRig(gunTarget)
				select gorillaGuardianZoneManager)
			{
				item.SetGuardian((NetPlayer)null);
			}
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
		guardianDelay = Time.time + 0.1f;
	}

	public static void UnguardianAll()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			foreach (GorillaGuardianZoneManager item in GorillaGuardianZoneManager.zoneManagers.Where((GorillaGuardianZoneManager gorillaGuardianZoneManager) => ((Behaviour)gorillaGuardianZoneManager).enabled && gorillaGuardianZoneManager.IsZoneValid()))
			{
				item.SetGuardian((NetPlayer)null);
			}
			return;
		}
		NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
	}

	public static void SetPlayerColors(Dictionary<int, int> colors)
	{
		IEnumerable<NetPlayer> source = NetworkSystem.Instance.AllNetPlayers.Where((NetPlayer p) => colors.ContainsKey(p.ActorNumber));
		MonkeBallGame.Instance.photonView.RPC("RequestSetGameStateRPC", (RpcTarget)0, new object[7]
		{
			2,
			PhotonNetwork.Time + (double)(MonkeBallGame.Instance.gameDuration - 1f),
			source.Select((NetPlayer p) => p.ActorNumber).ToArray(),
			source.Select((NetPlayer p) => colors[p.ActorNumber]).ToArray(),
			new int[MonkeBallGame.Instance.team.Count],
			MonkeBallGame.Instance.startingBalls.Select((MonkeBall ball) => BitPackUtils.PackHandPosRotForNetwork(((Component)ball).transform.position, ((Component)ball).transform.rotation)).ToArray(),
			MonkeBallGame.Instance.startingBalls.Select((MonkeBall ball) => BitPackUtils.PackWorldPosForNetwork(ball.gameBall.GetVelocity())).ToArray()
		});
	}

	public static void SetColorSelf(int color)
	{
		SetPlayerColors(new Dictionary<int, int> { 
		{
			NetworkSystem.Instance.LocalPlayer.ActorNumber,
			color
		} });
	}

	public static void SetColorGun(int color)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (!Main.GetGunInput(isShooting: true) || !(Time.time > playerColorDelay))
		{
			return;
		}
		VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
		if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
		{
			playerColorDelay = Time.time + 0.1f;
			if (PhotonNetwork.IsMasterClient)
			{
				SetPlayerColors(new Dictionary<int, int> { 
				{
					RigUtilities.GetPlayerFromVRRig(componentInParent).ActorNumber,
					color
				} });
			}
			else
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			}
		}
	}

	public static void SetColorAll(int color)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			SetPlayerColors(NetworkSystem.Instance.AllNetPlayers.ToDictionary((NetPlayer p) => p.ActorNumber, (NetPlayer p) => color));
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
	}

	public static void StrobeColorSelf()
	{
		if (Time.time > playerColorDelay)
		{
			playerColorDelay = Time.time + 0.1f;
			if (NetworkSystem.Instance.IsMasterClient)
			{
				SetColorSelf((Time.time % 0.2f > 0.1f) ? 1 : 0);
			}
			else
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			}
		}
	}

	public static void StrobeColorGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && Time.time > playerColorDelay)
			{
				playerColorDelay = Time.time + 0.1f;
				if (NetworkSystem.Instance.IsMasterClient)
				{
					SetPlayerColors(new Dictionary<int, int> { 
					{
						RigUtilities.GetPlayerFromVRRig(Main.lockTarget).ActorNumber,
						(Time.time % 0.2f > 0.1f) ? 1 : 0
					} });
				}
				else
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
				}
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal() && !componentInParent.IsTagged() && PhotonNetwork.IsMasterClient)
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
		}
	}

	public static void StrobeColorAll()
	{
		if (Time.time > playerColorDelay)
		{
			playerColorDelay = Time.time + 0.1f;
			if (NetworkSystem.Instance.IsMasterClient)
			{
				SetColorAll((Time.time % 0.2f > 0.1f) ? 1 : 0);
			}
			else
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			}
		}
	}

	public static void MaterialTarget(VRRig rig)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Invalid comparison between Unknown and I4
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			return;
		}
		NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(rig);
		materialState.TryGetValue(rig, out var value);
		value++;
		value %= 6;
		if ((int)GorillaGameManager.instance.GameType() == 0 && value < 4)
		{
			value = 4;
		}
		materialState[rig] = value;
		switch (value)
		{
		case 0:
			GameModeUtilities.AddInfected(playerFromVRRig);
			break;
		case 1:
			GameModeUtilities.RemoveInfected(playerFromVRRig);
			break;
		case 2:
			GameModeUtilities.AddRock(playerFromVRRig);
			break;
		case 3:
			GameModeUtilities.RemoveRock(playerFromVRRig);
			break;
		case 4:
			SetPlayerColors(new Dictionary<int, int> { { playerFromVRRig.ActorNumber, 0 } });
			break;
		case 5:
			SetPlayerColors(new Dictionary<int, int> { { playerFromVRRig.ActorNumber, 1 } });
			break;
		}
	}

	public static void MaterialGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				if (!NetworkSystem.Instance.IsMasterClient)
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
				}
				else if (Time.time > materialDelay)
				{
					materialDelay = Time.time + 0.1f;
					MaterialTarget(Main.lockTarget);
				}
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal() && !componentInParent.IsTagged() && PhotonNetwork.IsMasterClient)
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
		}
	}

	public static void MaterialAll()
	{
		if (!(Time.time > materialDelay))
		{
			return;
		}
		materialDelay = Time.time + 0.1f;
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			MaterialTarget(activeRig);
		}
	}

	public static void GuardianSpaz()
	{
		if (Time.time > guardianSpazDelay)
		{
			guardianSpazDelay = Time.time + 0.1f;
			guardianSpazToggle = !guardianSpazToggle;
			if (guardianSpazToggle)
			{
				GuardianAll();
			}
			else
			{
				UnguardianAll();
			}
		}
	}

	public static void AlwaysGuardian()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom || (int)GorillaGameManager.instance.GameType() != 8)
		{
			return;
		}
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (!((Behaviour)VRRig.LocalRig).enabled)
			{
				((Behaviour)VRRig.LocalRig).enabled = true;
			}
			GorillaGuardianManager val = (GorillaGuardianManager)GorillaGameManager.instance;
			if (!val.IsPlayerGuardian(NetPlayer.op_Implicit(PhotonNetwork.LocalPlayer)))
			{
				SetGuardianTarget(NetPlayer.op_Implicit(PhotonNetwork.LocalPlayer));
			}
			return;
		}
		GorillaGuardianManager val2 = (GorillaGuardianManager)GorillaGameManager.instance;
		TappableGuardianIdol[] allType = Main.GetAllType<TappableGuardianIdol>(5f);
		foreach (TappableGuardianIdol val3 in allType)
		{
			if (Object.op_Implicit((Object)(object)((Tappable)val3).manager) && Object.op_Implicit((Object)(object)((NetworkSceneObject)((Tappable)val3).manager).photonView) && !val3.isChangingPositions)
			{
				GorillaGuardianZoneManager zoneManager = val3.zoneManager;
				if (!val2.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer) && zoneManager.IsZoneValid() && Object.op_Implicit((Object)(object)((Tappable)val3).manager))
				{
					((Behaviour)VRRig.LocalRig).enabled = false;
					((Component)VRRig.LocalRig).transform.position = ((Component)val3).transform.position + RandomUtilities.RandomVector3(0.1f);
					((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)val3).transform.position;
					((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)val3).transform.position;
					if (Time.time > alwaysGuardianDelay)
					{
						alwaysGuardianDelay = Time.time + ((zoneManager._currentActivationTime >= zoneManager.requiredActivationTime - 1f) ? 0f : 0.2f);
						((Tappable)val3).OnTap(Random.Range(0f, 1f));
						Main.RPCProtection();
					}
				}
			}
			else
			{
				((Behaviour)VRRig.LocalRig).enabled = true;
			}
		}
	}

	public static void GuardianProtector()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		GorillaGuardianManager val = (GorillaGuardianManager)GorillaGameManager.instance;
		if (!val.IsPlayerGuardian(NetPlayer.op_Implicit(PhotonNetwork.LocalPlayer)))
		{
			return;
		}
		TappableGuardianIdol[] allType = Main.GetAllType<TappableGuardianIdol>(5f);
		foreach (TappableGuardianIdol tgi in allType)
		{
			if (!Object.op_Implicit((Object)(object)((Tappable)tgi).manager) || !Object.op_Implicit((Object)(object)((NetworkSceneObject)((Tappable)tgi).manager).photonView))
			{
				continue;
			}
			foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig rig) => !rig.isLocal && Vector3.Distance(((Component)rig).transform.position, ((Component)tgi).transform.position) < 2f && Time.time > guardianProtectorDelay))
			{
				NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(item);
				Vector3 val2 = ((Component)item).transform.position - ((Component)tgi).transform.position;
				BetaSetVelocityPlayer(playerFromVRRig, ((Vector3)(ref val2)).normalized * 50f);
				guardianProtectorDelay = Time.time + 0.1f;
			}
		}
	}

	public static void GuardianKickTarget(NetPlayer target)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > crashAllDelay)
		{
			crashAllDelay = Time.time + 0.1f;
			VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(target);
			_003F velocity;
			if (!(((Component)vRRigFromPlayer).transform.position.z < -28.5f))
			{
				velocity = ((((Component)vRRigFromPlayer).transform.position.z < -23f) ? new Vector3(-50f, 0f, 50f) : (Vector3.left * 50f));
			}
			else
			{
				Vector3 val = new Vector3(-47.82025f, 6.460508f, -29.04836f) - ((Component)vRRigFromPlayer).transform.position;
				velocity = ((Vector3)(ref val)).normalized * 50f;
			}
			BetaSetVelocityPlayer(target, (Vector3)velocity);
			Main.RPCProtection();
		}
	}

	public static void GuardianKickGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && Time.time > crashAllDelay)
			{
				crashAllDelay = Time.time + 0.1f;
				NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(Main.lockTarget);
				_003F velocity;
				if (!(((Component)Main.lockTarget).transform.position.z < -28.5f))
				{
					velocity = ((((Component)Main.lockTarget).transform.position.z < -23f) ? new Vector3(-50f, 0f, 50f) : (Vector3.left * 50f));
				}
				else
				{
					Vector3 val2 = new Vector3(-47.82025f, 6.460508f, -29.04836f) - ((Component)Main.lockTarget).transform.position;
					velocity = ((Vector3)(ref val2)).normalized * 50f;
				}
				BetaSetVelocityPlayer(playerFromVRRig, (Vector3)velocity);
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
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
		}
	}

	public static void GuardianKickAll()
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		if (!(Main.rightTrigger > 0.5f) || !(Time.time > crashAllDelay))
		{
			return;
		}
		crashAllDelay = Time.time + 0.1f;
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig rig) => !rig.isLocal))
		{
			NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(item);
			_003F velocity;
			if (!(((Component)item).transform.position.z < -28.5f))
			{
				velocity = ((((Component)item).transform.position.z < -23f) ? new Vector3(-50f, 0f, 50f) : (Vector3.left * 50f));
			}
			else
			{
				Vector3 val = new Vector3(-47.82025f, 6.460508f, -29.04836f) - ((Component)item).transform.position;
				velocity = ((Vector3)(ref val)).normalized * 50f;
			}
			BetaSetVelocityPlayer(playerFromVRRig, (Vector3)velocity);
			Main.RPCProtection();
		}
	}

	public static void GuardianCrashPlayer(NetPlayer target)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(target);
		if (Time.time > crashAllDelay && ((Component)vRRigFromPlayer).transform.position.x < -5f)
		{
			crashAllDelay = Time.time + 0.1f;
			BetaSetVelocityPlayer(target, ((((Component)vRRigFromPlayer).transform.position.y > 55f) ? Vector3.right : Vector3.up) * 50f);
			Main.RPCProtection();
		}
	}

	public static void GuardianCrashGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && Time.time > crashAllDelay && ((Component)Main.lockTarget).transform.position.x < -5f)
			{
				crashAllDelay = Time.time + 0.1f;
				BetaSetVelocityPlayer(RigUtilities.GetPlayerFromVRRig(Main.lockTarget), ((((Component)Main.lockTarget).transform.position.y > 55f) ? Vector3.right : Vector3.up) * 50f);
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
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
		}
	}

	public static void GuardianCrashAll()
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (!(Main.rightTrigger > 0.5f) || !(Time.time > crashAllDelay))
		{
			return;
		}
		crashAllDelay = Time.time + 0.1f;
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig rig) => !rig.isLocal && ((Component)rig).transform.position.x < -5f))
		{
			BetaSetVelocityPlayer(RigUtilities.GetPlayerFromVRRig(item), ((((Component)item).transform.position.y > 55f) ? Vector3.right : Vector3.up) * 50f);
			Main.RPCProtection();
		}
	}

	public static void DriverStatus(bool locked)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			((GorillaSerializer)CustomMapsTerminal.instance.mapTerminalNetworkObject).SendRPC("SetTerminalControlStatus_RPC", true, new object[2]
			{
				locked,
				PhotonNetwork.LocalPlayer.ActorNumber
			});
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
	}

	public static void SpazDriver()
	{
		if (PhotonNetwork.IsMasterClient && Time.time > spazDriverDelay)
		{
			spazDriverDelay = Time.time + 0.1f;
			((GorillaSerializer)CustomMapsTerminal.instance.mapTerminalNetworkObject).SendRPC("SetTerminalControlStatus_RPC", true, new object[2]
			{
				true,
				PhotonNetwork.LocalPlayer.ActorNumber
			});
			((GorillaSerializer)CustomMapsTerminal.instance.mapTerminalNetworkObject).SendRPC("SetTerminalControlStatus_RPC", true, new object[2]
			{
				false,
				PhotonNetwork.LocalPlayer.ActorNumber
			});
		}
	}

	public static void DriverStatusGun(bool locked)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && PhotonNetwork.IsMasterClient)
			{
				((GorillaSerializer)CustomMapsTerminal.instance.mapTerminalNetworkObject).SendRPC("SetTerminalControlStatus_RPC", true, new object[2]
				{
					locked,
					Main.lockTarget.GetPlayer().ActorNumber
				});
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
			Main.gunLocked = false;
		}
	}

	public static void SpazDriverStatusGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && PhotonNetwork.IsMasterClient && Time.time > spazDriverDelay)
			{
				spazDriverDelay = Time.time + 0.1f;
				((GorillaSerializer)CustomMapsTerminal.instance.mapTerminalNetworkObject).SendRPC("SetTerminalControlStatus_RPC", true, new object[2]
				{
					true,
					Main.lockTarget.GetPlayer().ActorNumber
				});
				((GorillaSerializer)CustomMapsTerminal.instance.mapTerminalNetworkObject).SendRPC("SetTerminalControlStatus_RPC", true, new object[2]
				{
					false,
					Main.lockTarget.GetPlayer().ActorNumber
				});
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
			Main.gunLocked = false;
		}
	}

	public static void BecomeDriver()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			((GorillaSerializer)CustomMapsTerminal.instance.mapTerminalNetworkObject).SendRPC("SetTerminalControlStatus_RPC", true, new object[2]
			{
				true,
				PhotonNetwork.LocalPlayer.ActorNumber
			});
			((GorillaSerializer)CustomMapsTerminal.instance.mapTerminalNetworkObject).photonView.OwnerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
	}

	public static void VirtualStumpKickGun()
	{
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			id = null;
			return;
		}
		if (!PhotonNetwork.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			Main.Toggle("Virtual Stump Kick All");
			return;
		}
		if (!id.HasValue && Time.time > setMapDelay)
		{
			setMapDelay = Time.time + 1f;
			if (CustomMapsTerminal.GetDriverID() != PhotonNetwork.LocalPlayer.ActorNumber)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=purple>VSTUMP</color><color=grey>]</color> Gaining control of the terminal, please wait...");
				BecomeDriver();
				return;
			}
			if (CustomMapManager.IsRemotePlayerInVirtualStump(NetworkSystem.Instance.LocalPlayer.UserId))
			{
				id = ((Manager.currentMapId == 4977315) ? 5024157 : 4977315);
				((GorillaSerializer)CustomMapsTerminal.instance.mapTerminalNetworkObject).photonView.RPC("UpdateScreen_RPC", Main.lockTarget.GetPhotonPlayer(), new object[3]
				{
					6,
					id,
					CustomMapsTerminal.GetDriverID()
				});
				NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully assigned ID. You may now kick.");
			}
			else
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Please temporarily enter the Virtual Stump.");
			}
		}
		if (!Main.GetGunInput(isShooting: false) || !id.HasValue)
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true))
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				((GorillaSerializer)CustomMapsTerminal.instance.mapTerminalNetworkObject).photonView.RPC("SetRoomMap_RPC", Main.lockTarget.GetPhotonPlayer(), new object[1] { id.Value });
			}
		}
	}

	public static void VirtualStumpKickAll()
	{
		if (!PhotonNetwork.InRoom)
		{
			id = null;
			return;
		}
		if (!PhotonNetwork.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			Main.Toggle("Virtual Stump Kick All");
			return;
		}
		if (!id.HasValue && Time.time > setMapDelay)
		{
			setMapDelay = Time.time + 1f;
			if (CustomMapsTerminal.GetDriverID() != PhotonNetwork.LocalPlayer.ActorNumber)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=purple>VSTUMP</color><color=grey>]</color> Gaining control of the terminal, please wait...");
				BecomeDriver();
				return;
			}
			if (CustomMapManager.IsRemotePlayerInVirtualStump(NetworkSystem.Instance.LocalPlayer.UserId))
			{
				id = ((Manager.currentMapId == 4977315) ? 5024157 : 4977315);
				((GorillaSerializer)CustomMapsTerminal.instance.mapTerminalNetworkObject).photonView.RPC("UpdateScreen_RPC", Main.lockTarget.GetPhotonPlayer(), new object[3]
				{
					6,
					id,
					CustomMapsTerminal.GetDriverID()
				});
				NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully assigned ID. You may now kick.");
			}
			else
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Please temporarily enter the Virtual Stump.");
			}
		}
		((GorillaSerializer)CustomMapsTerminal.instance.mapTerminalNetworkObject).photonView.RPC("SetRoomMap_RPC", (RpcTarget)1, new object[1] { id.Value });
		NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully kicked others.");
		Main.Toggle("Virtual Stump Kick All");
	}

	public static void GameEntityCrash(GameEntityManager manager, object target, Vector3? targetPosition = null)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)manager == (Object)null))
		{
			Vector3 valueOrDefault = targetPosition.GetValueOrDefault();
			if (!targetPosition.HasValue)
			{
				valueOrDefault = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
				targetPosition = valueOrDefault;
			}
			int[] array = manager.itemPrefabFactory.Keys.ToArray();
			int[] array2 = new int[500];
			Vector3[] array3 = (Vector3[])(object)new Vector3[500];
			Quaternion[] array4 = (Quaternion[])(object)new Quaternion[500];
			for (int i = 0; i < 500; i++)
			{
				array2[i] = array[Random.Range(0, array.Length)];
				array3[i] = targetPosition.Value;
				array4[i] = Quaternion.identity;
			}
			CreateItems(target, array2, array3, array4, null, manager);
		}
	}

	public static void MasterVisualizationType(bool positive = true)
	{
		string[] array = new string[3] { "Sphere", "Cube", "Tracer" };
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				masterVisualizationType++;
			}
			else
			{
				masterVisualizationType--;
			}
		}
		masterVisualizationType %= array.Length;
		if (masterVisualizationType < 0)
		{
			masterVisualizationType = array.Length - 1;
		}
		Buttons.GetIndex("Master Visualization Type").overlapText = "Master Visualization Type <color=grey>[</color><color=green>" + array[masterVisualizationType] + "</color><color=grey>]</color>";
	}

	public static void VisualizeMasterClient()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		if (Visuals.DoPerformanceCheck() || NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		VRRig val = NetworkSystem.Instance.MasterClient.VRRig();
		if (!((Object)(object)val == (Object)null))
		{
			long value = 2017928L;
			switch (masterVisualizationType)
			{
			case 0:
				Visuals.VisualizeAura(((Component)val).transform.position, 0.15f, Color.blue, value);
				break;
			case 1:
				Visuals.VisualizeCube(((Component)val).transform.position, Quaternion.Euler(Time.time * 90f, Time.time * 60f, Time.time * 30f), new Vector3(0.3f, 0.3f, 0.3f), Color.blue);
				break;
			case 2:
			{
				LineRenderer lineRender = Visuals.GetLineRender();
				lineRender.startColor = Color.blue;
				lineRender.endColor = Color.blue;
				float endWidth = (lineRender.startWidth = 0.025f);
				lineRender.endWidth = endWidth;
				lineRender.SetPosition(0, ((Component)val).transform.position);
				lineRender.SetPosition(1, GorillaTagger.Instance.rightHandTransform.position);
				break;
			}
			}
		}
	}

	public static void VirtualStumpCrashGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				GameEntityCrash(ManagerRegistry.CustomMaps.GameEntityManager, Main.lockTarget.GetPhotonPlayer(), ((Component)Main.lockTarget).transform.position);
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
			Main.gunLocked = false;
		}
	}

	public static void VirtualStumpCrashAll()
	{
		GameEntityCrash(ManagerRegistry.CustomMaps.GameEntityManager, (object)(RpcTarget)1);
	}

	public static void GhostReactorCrashGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				GameEntityCrash(ManagerRegistry.GhostReactor.GameEntityManager, Main.lockTarget.GetPhotonPlayer(), ((Component)Main.lockTarget).transform.position);
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
			Main.gunLocked = false;
		}
	}

	public static void GhostReactorCrashAll()
	{
		GameEntityCrash(ManagerRegistry.GhostReactor.GameEntityManager, (object)(RpcTarget)1);
	}

	public static void SuperInfectionCrashGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				GameEntityCrash(ManagerRegistry.SuperInfection.GameEntityManager, Main.lockTarget.GetPhotonPlayer(), ((Component)Main.lockTarget).transform.position);
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
			Main.gunLocked = false;
		}
	}

	public static void SuperInfectionCrashAll()
	{
		GameEntityCrash(ManagerRegistry.SuperInfection.GameEntityManager, (object)(RpcTarget)1);
	}

	public static void SuperInfectionBreakAudioGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				CreateItem(Main.lockTarget.GetPlayer(), GadgetByName["WristJetGadgetPropellor"], ((Component)Main.lockTarget).transform.position, RandomUtilities.RandomQuaternion(), Vector3.zero, Vector3.zero, 0L, ManagerRegistry.SuperInfection.GameEntityManager);
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
			Main.gunLocked = false;
		}
	}

	public static void SuperInfectionBreakAudioAll()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		CreateItem((object)(RpcTarget)1, GadgetByName["WristJetGadgetPropellor"], ((Component)GorillaTagger.Instance.bodyCollider).transform.position, RandomUtilities.RandomQuaternion(), Vector3.zero, Vector3.zero, 0L, ManagerRegistry.SuperInfection.GameEntityManager);
	}

	public static void DelayBanGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (!Main.GetGunInput(isShooting: true))
			{
				return;
			}
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (!Object.op_Implicit((Object)(object)componentInParent) || componentInParent.IsLocal() || Main.gunLocked)
			{
				return;
			}
			Main.gunLocked = true;
			Main.lockTarget = componentInParent;
			if (VRRig.LocalRig.IsTagged())
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must not be tagged.");
				return;
			}
			if (!Main.lockTarget.IsTagged())
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> The target must be tagged.");
				return;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must not be master client.");
				return;
			}
			if (Time.time > reportDelay)
			{
				reportDelay = Time.time + 0.5f;
				GorillaPlayerScoreboardLine.ReportPlayer(RigUtilities.GetPlayerFromVRRig(Main.lockTarget).UserId, (ButtonType)1, RigUtilities.GetPlayerFromVRRig(Main.lockTarget).NickName);
			}
			SerializePatch.OverrideSerialization = delegate
			{
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ae: Expected O, but got Unknown
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e3: Expected O, but got Unknown
				//IL_011d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0132: Unknown result type (might be due to invalid IL or missing references)
				//IL_0138: Expected O, but got Unknown
				//IL_0173: Unknown result type (might be due to invalid IL or missing references)
				RigUtilities.GetPlayerFromVRRig(Main.lockTarget);
				Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
				Vector3 position = ((Component)VRRig.LocalRig).transform.position;
				Main.SendSerialize(VRRig.LocalRig.GetPhotonView(), new RaiseEventOptions
				{
					TargetActors = (from plr in PhotonNetwork.PlayerList
						where !Extensions.Contains(new int[2]
						{
							PhotonNetwork.MasterClient.ActorNumber,
							RigUtilities.GetPlayerFromVRRig(Main.lockTarget).ActorNumber
						}, plr.ActorNumber)
						select plr.ActorNumber).ToArray()
				});
				((Component)VRRig.LocalRig).transform.position = new Vector3(99999f, 99999f, 99999f);
				PhotonView photonView = VRRig.LocalRig.GetPhotonView();
				RaiseEventOptions val2 = new RaiseEventOptions();
				val2.TargetActors = new int[1] { PhotonNetwork.MasterClient.ActorNumber };
				Main.SendSerialize(photonView, val2);
				((Component)VRRig.LocalRig).transform.position = Main.lockTarget.rightHandTransform.position;
				PhotonView photonView2 = VRRig.LocalRig.GetPhotonView();
				val2 = new RaiseEventOptions();
				val2.TargetActors = new int[1] { RigUtilities.GetPlayerFromVRRig(Main.lockTarget).ActorNumber };
				Main.SendSerialize(photonView2, val2);
				Main.RPCProtection();
				((Component)VRRig.LocalRig).transform.position = position;
				return false;
			};
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
			SerializePatch.OverrideSerialization = null;
		}
	}

	public static void DelayBanAll()
	{
		SerializePatch.OverrideSerialization = delegate
		{
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Expected O, but got Unknown
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Expected O, but got Unknown
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Expected O, but got Unknown
			if (VRRig.LocalRig.IsTagged())
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must not be tagged.");
				return true;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must not be master client.");
				return true;
			}
			RigUtilities.GetPlayerFromVRRig(Main.lockTarget);
			Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
			Vector3 position = ((Component)VRRig.LocalRig).transform.position;
			Main.SendSerialize(VRRig.LocalRig.GetPhotonView(), new RaiseEventOptions
			{
				TargetActors = (from player in PhotonNetwork.PlayerList
					where !player.IsMasterClient && player.VRRig().IsTagged()
					select player.ActorNumber).ToArray()
			});
			((Component)VRRig.LocalRig).transform.position = new Vector3(99999f, 99999f, 99999f);
			PhotonView photonView = VRRig.LocalRig.GetPhotonView();
			RaiseEventOptions val = new RaiseEventOptions();
			val.TargetActors = new int[1] { PhotonNetwork.MasterClient.ActorNumber };
			Main.SendSerialize(photonView, val);
			NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
			foreach (NetPlayer val2 in playerListOthers)
			{
				VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(val2);
				if (!val2.IsMasterClient && vRRigFromPlayer.IsTagged())
				{
					((Component)VRRig.LocalRig).transform.position = vRRigFromPlayer.rightHandTransform.position;
					PhotonView photonView2 = VRRig.LocalRig.GetPhotonView();
					val = new RaiseEventOptions();
					val.TargetActors = new int[1] { val2.ActorNumber };
					Main.SendSerialize(photonView2, val);
				}
			}
			Main.RPCProtection();
			((Component)VRRig.LocalRig).transform.position = position;
			return false;
		};
	}

	public static void GuardianObliteratePlayer(NetPlayer target)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > crashAllDelay)
		{
			crashAllDelay = Time.time + 0.1f;
			VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(target);
			BetaSetVelocityPlayer(target, ((((Component)vRRigFromPlayer).transform.position.y > 55f) ? Vector3.right : Vector3.up) * 50f);
			Main.RPCProtection();
		}
	}

	public static void DirectionOnGrab(Vector3 direction)
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		((Behaviour)VRRig.LocalRig).enabled = true;
		foreach (VRRig item in from rig in VRRigCache.ActiveRigs
			where !rig.isLocal
			where rig.leftHandLink.grabbedPlayer == NetworkSystem.Instance.LocalPlayer || rig.rightHandLink.grabbedPlayer == NetworkSystem.Instance.LocalPlayer
			select rig)
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			Transform transform = ((Component)VRRig.LocalRig).transform;
			transform.position += ((Vector3)(ref direction)).normalized * 2000f;
		}
	}

	public static void TowardsPointOnGrab()
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)point == (Object)null)
		{
			point = GameObject.CreatePrimitive((PrimitiveType)0);
			Object.Destroy((Object)(object)point.GetComponent<Collider>());
			point.transform.position = GorillaTagger.Instance.rightHandTransform.position;
			point.transform.localScale = Vector3.one * 0.2f;
		}
		point.GetComponent<Renderer>().material.color = Main.buttonColors[1].GetCurrentColor();
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true))
			{
				point.transform.position = item.transform.position;
			}
		}
		TowardsPositionOnGrab(point.transform.position);
	}

	public static void DisableTowardsPointOnGrab()
	{
		if ((Object)(object)point != (Object)null)
		{
			Object.Destroy((Object)(object)point);
			point = null;
		}
	}

	public static void GiveFlyOnGrab()
	{
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.isLocal && (activeRig.leftHandLink.grabbedPlayer == NetworkSystem.Instance.LocalPlayer || activeRig.rightHandLink.grabbedPlayer == NetworkSystem.Instance.LocalPlayer) && ((VRRig.LocalRig.leftHandLink.grabbedPlayer == activeRig.GetPlayer()) ? (((VRMap)activeRig.leftIndex).calcT > 0f) : (((VRMap)activeRig.rightIndex).calcT > 0f)))
			{
				Transform transform = ((Component)GTPlayer.Instance).transform;
				transform.position += activeRig.headMesh.transform.forward * (Time.deltaTime * Movement.FlySpeed);
				GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
			}
		}
	}

	public static void FlingShotgun()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (VRRig.LocalRig.leftHandLink.IsLinkActive() || VRRig.LocalRig.rightHandLink.IsLinkActive())
		{
			bool flag = VRRig.LocalRig.leftHandLink.IsLinkActive();
			if (flag ? Main.leftTriggerPressed : Main.rightTriggerPressed)
			{
				((Component)VRRig.LocalRig).transform.position = ((Component)VRRig.LocalRig).transform.position + Main.GetGunDirection(flag ? GorillaTagger.Instance.leftHandTransform : GorillaTagger.Instance.rightHandTransform) * Main.ShootStrength;
			}
		}
	}

	public static void ForceGrab(Vector3 targetTransform)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			ForceGrab(activeRig, targetTransform);
		}
	}

	public static bool ForceGrab(VRRig rig, Vector3 targetTransform, bool returnOnGrab = false, bool enableRigOnceDone = false)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		if (rig.IsLocal() || (Object)(object)rig == (Object)null)
		{
			return false;
		}
		((Behaviour)VRRig.LocalRig).enabled = false;
		((Component)VRRig.LocalRig).transform.position = targetTransform;
		bool flag = IsLeftHandAvailableForGrab(rig);
		bool flag2 = IsRightHandAvailableForGrab(rig);
		if (!flag && !flag2)
		{
			return false;
		}
		TakeMyHand_HandLink val = (flag ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink);
		TakeMyHand_HandLink val2 = (flag ? rig.leftHandLink : rig.rightHandLink);
		if (val2.grabbedPlayer != NetworkSystem.Instance.LocalPlayer)
		{
			if (grabDelay == 0f)
			{
				grabDelay = ((val2.rejectGrabsUntilTimestamp > Time.time) ? val2.rejectGrabsUntilTimestamp : (Time.time + 1f));
			}
			if (Time.time > grabDelay)
			{
				((Component)VRRig.LocalRig).transform.position = rig.syncPos;
				val.TentacleTryCreateLink(val2);
				NotificationManager.SendNotification("<color=grey>[</color><color=purple>MENU</color><color=grey>]</color> Tried to grab " + rig.GetPlayer().NickName + ".");
				grabDelay = ((val2.rejectGrabsUntilTimestamp > Time.time) ? val2.rejectGrabsUntilTimestamp : (Time.time + 1f));
			}
		}
		else if (val2.grabbedPlayer == NetworkSystem.Instance.LocalPlayer && returnOnGrab)
		{
			if (enableRigOnceDone && !((Behaviour)VRRig.LocalRig).enabled)
			{
				((Behaviour)VRRig.LocalRig).enabled = true;
			}
			return true;
		}
		return false;
	}

	public static void TowardsPositionOnGrab(Vector3 position)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (VRRig.LocalRig.leftHandLink.IsLinkActive() || VRRig.LocalRig.rightHandLink.IsLinkActive())
		{
			((Component)VRRig.LocalRig).transform.position = position;
		}
	}

	public static void FlingOnGrab()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		if (VRRig.LocalRig.leftHandLink.IsLinkActive() || VRRig.LocalRig.rightHandLink.IsLinkActive())
		{
			VRRig val = VRRig.LocalRig.leftHandLink.grabbedPlayer.VRRig() ?? VRRig.LocalRig.rightHandLink.grabbedPlayer.VRRig();
			Vector3 val2 = Vector3.up + ((Component)GorillaTagger.Instance.bodyCollider).transform.forward * 1f;
			Vector3 val3 = ((Vector3)(ref val2)).normalized * 3f;
			val.GetNetView().SendRPC("DroppedByPlayer", RigUtilities.GetPlayerFromVRRig(val), new object[1] { val3 });
		}
		else
		{
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void SpazPropHuntObjects()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Invalid comparison between Unknown and I4
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		if (Time.time > propHuntSpazDelay)
		{
			propHuntSpazDelay = Time.time + 0.1f;
			propHuntSpazMode = !propHuntSpazMode;
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			}
			else if (PhotonNetwork.InRoom && (int)GorillaGameManager.instance.GameType() == 9)
			{
				GorillaPropHuntGameManager val = (GorillaPropHuntGameManager)GorillaGameManager.instance;
				val._ph_timeRoundStartedMillis = (propHuntSpazMode ? 1 : 2);
				val._ph_randomSeed = Random.Range(1, int.MaxValue);
			}
		}
	}

	public static void SpazPropHunt()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Invalid comparison between Unknown and I4
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		if (Time.time > propHuntSpazDelay)
		{
			propHuntSpazDelay = Time.time + 0.1f;
			propHuntSpazMode = !propHuntSpazMode;
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			}
			else if (PhotonNetwork.InRoom && (int)GorillaGameManager.instance.GameType() == 9)
			{
				GorillaPropHuntGameManager val = (GorillaPropHuntGameManager)GorillaGameManager.instance;
				val._ph_timeRoundStartedMillis = ((!propHuntSpazMode) ? 1 : 0);
			}
		}
	}

	public static void CreateItem(object target, int hash, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, long sendData = 0L, GameEntityManager manager = null)
	{
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		GameEntityManager gameEntityManager = manager ?? ManagerRegistry.GhostReactor.GameEntityManager;
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (Time.time < ghostReactorDelay)
			{
				return;
			}
			ghostReactorDelay = Time.time + gameEntityManager.m_RpcSpamChecks.m_callLimiters[0].GetDelay();
			int num = gameEntityManager.CreateTypeNetId(hash);
			NetPlayer val = (NetPlayer)((target is NetPlayer) ? target : null);
			if (val != null)
			{
				target = RigUtilities.NetPlayerToPlayer(val);
			}
			object[] array = new object[6]
			{
				new int[1] { num },
				new int[1] { hash },
				new long[1] { BitPackUtils.PackWorldPosForNetwork(position) },
				new int[1] { BitPackUtils.PackQuaternionForNetwork(rotation) },
				new long[1] { sendData },
				new int[1] { gameEntityManager.GetInvalidNetId() }
			};
			object obj = target;
			object obj2 = obj;
			if (!(obj2 is RpcTarget val2))
			{
				Player val3 = (Player)((obj2 is Player) ? obj2 : null);
				if (val3 != null)
				{
					gameEntityManager.photonView.RPC("CreateItemRPC", val3, array);
				}
			}
			else
			{
				gameEntityManager.photonView.RPC("CreateItemRPC", val2, array);
			}
			if ((velocity != Vector3.zero || angVelocity != Vector3.zero || Buttons.GetIndex("Entity Gravity").enabled) && Time.time > throwDelay)
			{
				throwDelay = Time.time + gameEntityManager.m_RpcSpamChecks.m_callLimiters[5].GetDelay();
				velocity = velocity.ClampSqrMagnitude(1600f);
				object[] array2 = new object[8]
				{
					num,
					true,
					position,
					rotation,
					velocity,
					angVelocity,
					PhotonNetwork.LocalPlayer,
					PhotonNetwork.Time
				};
				object obj3 = target;
				object obj4 = obj3;
				if (!(obj4 is RpcTarget val4))
				{
					Player val5 = (Player)((obj4 is Player) ? obj4 : null);
					if (val5 != null)
					{
						gameEntityManager.photonView.RPC("ThrowEntityRPC", val5, array2);
					}
				}
				else
				{
					gameEntityManager.photonView.RPC("ThrowEntityRPC", val4, array2);
				}
			}
			Main.RPCProtection();
			return;
		}
		float maxDistance = 12f;
		if (Vector3.Distance(Main.ServerLeftHandPos, position) > maxDistance)
		{
			Vector3 serverLeftHandPos = Main.ServerLeftHandPos;
			Vector3 val6 = position - Main.ServerLeftHandPos;
			position = serverLeftHandPos + ((Vector3)(ref val6)).normalized * maxDistance;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(PhotonNetwork.LocalPlayer);
		if (gamePlayer.IsHoldingEntity(gameEntityManager, true) && Time.time > ghostReactorDelay)
		{
			((Behaviour)VRRig.LocalRig).enabled = true;
			if (!(Main.ServerLeftHandPos.Distance(position) < maxDistance))
			{
				return;
			}
			gameEntityManager.GetGameEntity(gamePlayer.GetGrabbedGameEntityId(GamePlayer.GetHandIndex(true))).RequestThrow(isLeftHand: true, position, rotation, velocity, angVelocity, gameEntityManager);
		}
		List<GameEntity> list = gameEntityManager.entities.Where((GameEntity e) => (Object)(object)e != (Object)null && e.typeId == hash && Vector3.Distance(Main.ServerLeftHandPos, ((Component)e).transform.position) < maxDistance && Vector3.Distance(((Component)GorillaTagger.Instance.bodyCollider).transform.position, ((Component)e).transform.position) > 3f && gameEntityManager.ValidateGrab(e, PhotonNetwork.LocalPlayer.actorNumber, true)).ToList();
		if (list.Count <= 0)
		{
			list = gameEntityManager.entities.Where((GameEntity e) => (Object)(object)e != (Object)null && e.typeId == hash && Vector3.Distance(Main.ServerLeftHandPos, ((Component)e).transform.position) < maxDistance && gameEntityManager.ValidateGrab(e, PhotonNetwork.LocalPlayer.actorNumber, true)).ToList();
		}
		if (list.Count <= 0)
		{
			list = gameEntityManager.entities.Where((GameEntity e) => (Object)(object)e != (Object)null && e.typeId == hash && gameEntityManager.ValidateGrab(e, PhotonNetwork.LocalPlayer.actorNumber, true)).ToList();
		}
		if (list.Count <= 0)
		{
			list = gameEntityManager.entities.Where((GameEntity e) => (Object)(object)e != (Object)null && e.typeId == hash).ToList();
		}
		if (list.Count <= 0)
		{
			return;
		}
		GameEntity val7 = list.OrderByDescending((GameEntity entity) => ((Component)entity).transform.position.Distance(((Component)GorillaTagger.Instance.bodyCollider).transform.position)).FirstOrDefault();
		if (Vector3.Distance(((Component)val7).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) > maxDistance)
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = ((Component)val7).transform.position - Vector3.one * 5f;
			if (CritterCoroutine != null)
			{
				((MonoBehaviour)CoroutineManager.instance).StopCoroutine(CritterCoroutine);
			}
			CritterCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(RopeEnableRig());
		}
		if (Vector3.Distance(((Component)val7).transform.position, Main.ServerPos) < maxDistance && Time.time > ghostReactorDelay)
		{
			ghostReactorDelay = Time.time + 0.1f;
			((Component)val7).transform.position = GorillaTagger.Instance.rightHandTransform.position;
			((Component)val7).transform.rotation = RandomUtilities.RandomQuaternion();
			val7.RequestGrab(isLeftHand: true, Vector3.zero, Quaternion.identity, gameEntityManager);
			Main.RPCProtection();
		}
	}

	public static void CreateItems(object target, int[] hashes, Vector3[] positions, Quaternion[] rotations, long[] sendData = null, GameEntityManager manager = null)
	{
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Expected I4, but got Unknown
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		GameEntityManager val = manager ?? ManagerRegistry.GhostReactor.GameEntityManager;
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (Time.time < ghostReactorDelay)
			{
				return;
			}
			ghostReactorDelay = Time.time + val.m_RpcSpamChecks.m_callLimiters[1].GetDelay();
			NetPlayer val2 = (NetPlayer)((target is NetPlayer) ? target : null);
			if (val2 != null)
			{
				target = RigUtilities.NetPlayerToPlayer(val2);
			}
			if (sendData == null)
			{
				sendData = Enumerable.Repeat(0L, hashes.Length).ToArray();
			}
			byte[] array = new byte[15360];
			MemoryStream output = new MemoryStream(array);
			BinaryWriter binaryWriter = new BinaryWriter(output);
			binaryWriter.Write(hashes.Length);
			for (int i = 0; i < hashes.Length; i++)
			{
				binaryWriter.Write(manager.CreateTypeNetId(hashes[i]));
				binaryWriter.Write(hashes[i]);
				binaryWriter.Write(BitPackUtils.PackWorldPosForNetwork(positions[i]));
				binaryWriter.Write(BitPackUtils.PackQuaternionForNetwork(rotations[i]));
				binaryWriter.Write(sendData[i]);
				binaryWriter.Write(val.GetInvalidNetId());
			}
			byte[] array2 = GZipStream.CompressBuffer(array);
			object[] array3 = new object[2]
			{
				(int)manager.zone,
				array2
			};
			object obj = target;
			object obj2 = obj;
			if (!(obj2 is RpcTarget val3))
			{
				Player val4 = (Player)((obj2 is Player) ? obj2 : null);
				if (val4 != null)
				{
					val.photonView.RPC("CreateItemsRPC", val4, array3);
				}
			}
			else
			{
				val.photonView.RPC("CreateItemsRPC", val3, array3);
			}
			Main.RPCProtection();
		}
		else
		{
			CreateItem(target, hashes[0], positions[0], rotations[0], Vector3.zero, Vector3.zero, (sendData.Length != 0) ? sendData[1] : 0, manager);
		}
	}

	public static void SpamObjectGrip(int objectId)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab)
		{
			CreateItem((object)(RpcTarget)0, objectId, GorillaTagger.Instance.rightHandTransform.position, RandomUtilities.RandomQuaternion(), GorillaTagger.Instance.rightHandTransform.forward * Main.ShootStrength, Vector3.zero, 0L);
		}
	}

	public static void SpamEntityGrip()
	{
		int[] array = ObjectByName.Select((KeyValuePair<string, int> x) => x.Value).ToArray();
		SpamObjectGrip(array[Random.Range(0, array.Length)]);
	}

	public static void ToolSpamGrip()
	{
		int[] array = (from x in ObjectByName
			where x.Key.Contains("Tool")
			select x.Value).ToArray();
		SpamObjectGrip(array[Random.Range(0, array.Length)]);
	}

	public static void ToolSpamGun()
	{
		int[] array = (from x in ObjectByName
			where x.Key.Contains("Tool")
			select x.Value).ToArray();
		SpamObjectGun(array[Random.Range(0, array.Length)]);
	}

	public static void SpamObjectGun(int objectId)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true))
			{
				CreateItem((object)(RpcTarget)0, objectId, item.transform.position, RandomUtilities.RandomQuaternion(), Vector3.zero, Vector3.zero, 0L);
			}
		}
	}

	public static void SpamEntityGun()
	{
		int[] array = ObjectByName.Select((KeyValuePair<string, int> x) => x.Value).ToArray();
		SpamObjectGun(array[Random.Range(0, array.Length)]);
	}

	public static void RainEntities()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		int[] array = ObjectByName.Select((KeyValuePair<string, int> x) => x.Value).ToArray();
		CreateItem((object)(RpcTarget)0, array[Random.Range(0, array.Length)], ((Component)VRRig.LocalRig).transform.position + new Vector3(Random.Range(-3f, 3f), 4f, Random.Range(-3f, 3f)), Quaternion.identity, Vector3.down, Vector3.zero, 0L);
	}

	public static void EntityAura()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		int[] array = ObjectByName.Select((KeyValuePair<string, int> x) => x.Value).ToArray();
		object target = (object)(RpcTarget)0;
		int hash = array[Random.Range(0, array.Length)];
		Vector3 position = ((Component)VRRig.LocalRig).transform.position;
		Vector3 val = RandomUtilities.RandomVector3();
		CreateItem(target, hash, position + ((Vector3)(ref val)).normalized * 2f, Quaternion.identity, Vector3.down, Vector3.zero, 0L);
	}

	public static void EntityFountain()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		int[] array = ObjectByName.Select((KeyValuePair<string, int> x) => x.Value).ToArray();
		CreateItem((object)(RpcTarget)0, array[Random.Range(0, array.Length)], ((Component)VRRig.LocalRig).transform.position + Vector3.up * 3f, Quaternion.identity, RandomUtilities.RandomVector3(15f), Vector3.zero, 0L);
	}

	public static void GhostReactorTextGun()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		GameObject item = Main.RenderGun().NewPointer;
		if (Main.GetGunInput(isShooting: true))
		{
			if (!basePosition.HasValue)
			{
				basePosition = item.transform.position + Vector3.up;
			}
			if (!(Time.time > textDelay))
			{
				return;
			}
			textDelay = Time.time + 0.1f;
			bool[][] array = Letters[textToRender[characterIndex].ToString()];
			List<Vector3> list = new List<Vector3>();
			Vector3 val = default(Vector3);
			for (int i = 0; i < array.Length; i++)
			{
				bool[] array2 = array[i];
				for (int j = 0; j < array2.Length; j++)
				{
					bool flag = array2[j];
					((Vector3)(ref val))._002Ector((float)j * 0.2f + (float)characterIndex * 1.2f, (float)i * -0.2f, 0f);
					if (flag)
					{
						list.Add(basePosition.Value + val);
					}
				}
			}
			CreateItems((object)(RpcTarget)0, Enumerable.Repeat(ObjectByName["GhostReactorCollectibleFlower"], list.Count).ToArray(), list.ToArray(), Enumerable.Repeat<Quaternion>(Quaternion.identity, list.Count).ToArray());
			characterIndex++;
		}
		else
		{
			characterIndex = 0;
			basePosition = null;
		}
	}

	public static void SuperInfectionTextGun()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		GameObject item = Main.RenderGun().NewPointer;
		if (Main.GetGunInput(isShooting: true))
		{
			if (!basePosition.HasValue)
			{
				basePosition = item.transform.position + Vector3.up;
			}
			if (!(Time.time > textDelay))
			{
				return;
			}
			textDelay = Time.time + 0.1f;
			bool[][] array = Letters[textToRender[characterIndex].ToString()];
			List<Vector3> list = new List<Vector3>();
			Vector3 val = default(Vector3);
			for (int i = 0; i < array.Length; i++)
			{
				bool[] array2 = array[i];
				for (int j = 0; j < array2.Length; j++)
				{
					bool flag = array2[j];
					((Vector3)(ref val))._002Ector((float)j * 0.2f + (float)characterIndex * 1.2f, (float)i * -0.2f, 0f);
					if (flag)
					{
						list.Add(basePosition.Value + val);
					}
				}
			}
			CreateItems((object)(RpcTarget)0, Enumerable.Repeat(GadgetByName["SIGadgetDashYoyo"], list.Count).ToArray(), list.ToArray(), Enumerable.Repeat<Quaternion>(Quaternion.identity, list.Count).ToArray(), null, ManagerRegistry.SuperInfection.GameEntityManager);
			characterIndex++;
		}
		else
		{
			characterIndex = 0;
			basePosition = null;
		}
	}

	public static IEnumerator DrawSmallDelay(Vector3 position, int id, GameEntityManager manager)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		GameObject Temporary = GameObject.CreatePrimitive((PrimitiveType)0);
		Temporary.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
		Temporary.transform.position = position;
		Object.Destroy((Object)(object)Temporary.GetComponent<Collider>());
		yield return (object)new WaitForSeconds(0.5f);
		CreateItem((object)(RpcTarget)0, id, Temporary.transform.position + new Vector3(0f, 0.1f, 0f), RandomUtilities.RandomQuaternion(), Vector3.zero, Vector3.zero, 0L, manager);
		Object.Destroy((Object)(object)Temporary);
		Main.RPCProtection();
	}

	public static void GhostReactorDrawGun()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true))
			{
				((MonoBehaviour)CoroutineManager.instance).StartCoroutine(DrawSmallDelay(item.transform.position, ObjectByName["GhostReactorCollectibleCore"], ManagerRegistry.GhostReactor.GameEntityManager));
			}
		}
	}

	public static void SuperInfectionDrawGun()
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		GameObject item = Main.RenderGun().NewPointer;
		if (Main.GetGunInput(isShooting: true))
		{
			int[] array = GadgetByName.Select((KeyValuePair<string, int> element) => element.Value).ToArray();
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(DrawSmallDelay(item.transform.position, array[Random.Range(0, array.Length)], ManagerRegistry.SuperInfection.GameEntityManager));
		}
	}

	public static void DestroyEntityGun()
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		GameObject item = Main.RenderGun().NewPointer;
		if (!Main.GetGunInput(isShooting: true) || !(Time.time > destroyDelay))
		{
			return;
		}
		GameEntity val = null;
		float num = float.MaxValue;
		foreach (GameEntity entity in ManagerRegistry.GhostReactor.GameEntityManager.entities)
		{
			if ((Object)(object)entity != (Object)null)
			{
				float num2 = Vector3.Distance(item.transform.position, ((Component)entity).transform.position);
				if (num2 < 0.75f && num2 < num)
				{
					val = entity;
					num = num2;
				}
			}
		}
		if ((Object)(object)val != (Object)null)
		{
			destroyDelay = Time.time + 0.02f;
			if (NetworkSystem.Instance.IsMasterClient)
			{
				ManagerRegistry.GhostReactor.GameEntityManager.photonView.RPC("DestroyItemRPC", (RpcTarget)0, new object[1] { new int[1] { val.GetNetId() } });
				Main.RPCProtection();
			}
			else
			{
				val.RequestGrab(isLeftHand: true, Vector3.zero, Quaternion.identity);
				val.RequestThrow(isLeftHand: true, ((Component)GorillaTagger.Instance.bodyCollider).transform.position - Vector3.up * 14f, Quaternion.identity, Vector3.zero, Vector3.zero);
			}
		}
	}

	public static void InfiniteResources()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		SIPlayer val = SIPlayer.Get(NetworkSystem.Instance.LocalPlayer.ActorNumber);
		for (int i = 0; i < val.CurrentProgression.resourceArray.Length; i++)
		{
			val.CurrentProgression.resourceArray[i] = int.MaxValue;
		}
		if (!(Time.time > resourceIncrementDelay))
		{
			return;
		}
		resourceIncrementDelay = Time.time + 1f;
		for (int j = 0; j < 6; j++)
		{
			ProgressionManager.Instance.IncrementSIResource(((object)(ResourceType)j/*cast due to .constrained prefix*/).ToString(), (Action<string>)null, (Action<string>)delegate
			{
				resourceIncrementDelay = Time.time + 10f;
			});
		}
	}

	public static void CompleteAllQuests()
	{
		for (int i = 0; i < SIProgression.Instance.activeQuestIds.Length; i++)
		{
			RotatingQuest questById = SIProgression.Instance.questSourceList.GetQuestById(SIProgression.Instance.activeQuestIds[i]);
			questById.SetProgress(questById.requiredOccurenceCount);
		}
	}

	public static void ClaimAllTerminals()
	{
		SICombinedTerminal[] siTerminals = ManagerRegistry.SuperInfection.ZoneSuperInfection.siTerminals;
		foreach (SICombinedTerminal val in siTerminals)
		{
			if (val != null)
			{
				val.PlayerHandScanned(NetworkSystem.Instance.LocalPlayer.ActorNumber);
			}
		}
	}

	public static void UnlockAllGadgets()
	{
		bool[][] unlockedTechTreeData = SIProgression.Instance.unlockedTechTreeData;
		foreach (bool[] array in unlockedTechTreeData)
		{
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = true;
			}
		}
	}

	public static void DebugBlasterAimbot()
	{
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		List<NetPlayer> infected = GameModeUtilities.InfectedList();
		List<VRRig> source = (from rig in VRRigCache.ActiveRigs
			where !rig.isLocal
			where !infected.Contains(RigUtilities.GetPlayerFromVRRig(rig))
			select rig).ToList();
		Transform head = ((Component)GorillaTagger.Instance.headCollider).transform;
		VRRig val = (from x in source.Where((VRRig rig) => (Object)(object)rig != (Object)null).Select(delegate(VRRig rig)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				Vector3 val2 = ((Component)rig).transform.position - head.position;
				return new
				{
					Rig = rig,
					ToRig = ((Vector3)(ref val2)).normalized,
					Distance = Vector3.Distance(head.position, ((Component)rig).transform.position)
				};
			})
			orderby Vector3.Angle(head.forward, x.ToRig), x.Distance
			select x.Rig).FirstOrDefault();
		if (!((Object)(object)val == (Object)null))
		{
			Visuals.VisualizeAura(val.headMesh.transform.position, 0.1f, Color.green, -91752L);
		}
	}

	public static SIGadgetChargeBlaster GetBlaster()
	{
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
		((ControllerInputPoller)ControllerInputPoller.instance).leftGrab = true;
		((ControllerInputPoller)ControllerInputPoller.instance).leftControllerGripFloat = 1f;
		if ((Object)(object)SuperInfectionManager.activeSuperInfectionManager == (Object)null)
		{
			if (Time.time > siErrorCooldown)
			{
				NotificationManager.SendNotification("<color=red>[ERROR]</color> Super Infection manager is not active.");
				siErrorCooldown = Time.time + 3f;
			}
			return null;
		}
		GameEntityManager gameEntityManager = ManagerRegistry.SuperInfection.GameEntityManager;
		if ((Object)(object)gameEntityManager == (Object)null)
		{
			if (Time.time > siErrorCooldown)
			{
				NotificationManager.SendNotification("<color=red>[ERROR]</color> Super Infection GameEntityManager is null.");
				siErrorCooldown = Time.time + 3f;
			}
			return null;
		}
		int hash = GadgetByName["MegaChargeBlasterGadget"];
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(PhotonNetwork.LocalPlayer);
		if (gamePlayer.IsHoldingEntity(gameEntityManager, true))
		{
			GameEntity gameEntity = gameEntityManager.GetGameEntity(gamePlayer.GetGrabbedGameEntityId(GamePlayer.GetHandIndex(true)));
			SIGadgetChargeBlaster result = default(SIGadgetChargeBlaster);
			if (((Component)gameEntity).gameObject.TryGetComponent<SIGadgetChargeBlaster>(ref result))
			{
				return result;
			}
			gameEntity.RequestThrow(isLeftHand: true, ((Component)gameEntity).transform.position, ((Component)gameEntity).transform.rotation, Vector3.zero, Vector3.zero, gameEntityManager);
		}
		else if (NetworkSystem.Instance.IsMasterClient)
		{
			if (spawnedNetId.HasValue && spawnedFrame > Time.frameCount - 10)
			{
				GameEntity gameEntity2 = gameEntityManager.GetGameEntity(spawnedNetId.Value);
				gameEntity2.RequestGrab(isLeftHand: true, Vector3.zero, Quaternion.identity, gameEntityManager);
				SIGadgetChargeBlaster result2 = default(SIGadgetChargeBlaster);
				if (((Component)gameEntity2).gameObject.TryGetComponent<SIGadgetChargeBlaster>(ref result2))
				{
					return result2;
				}
			}
			if (Time.time < ghostReactorDelay)
			{
				return null;
			}
			ghostReactorDelay = Time.time + gameEntityManager.m_RpcSpamChecks.m_callLimiters[0].GetDelay();
			int num = gameEntityManager.CreateTypeNetId(hash);
			object[] array = new object[6]
			{
				new int[1] { num },
				new int[1] { hash },
				new long[1] { BitPackUtils.PackWorldPosForNetwork(GorillaTagger.Instance.leftHandTransform.position) },
				new int[1] { BitPackUtils.PackQuaternionForNetwork(GorillaTagger.Instance.leftHandTransform.rotation) },
				new long[1],
				new int[1] { gameEntityManager.GetInvalidNetId() }
			};
			gameEntityManager.photonView.RPC("CreateItemRPC", (RpcTarget)0, array);
			spawnedNetId = num;
			spawnedFrame = Time.frameCount;
			Main.RPCProtection();
		}
		else
		{
			Vector3 position = GorillaTagger.Instance.leftHandTransform.position;
			float maxDistance = 12f;
			if (Vector3.Distance(Main.ServerLeftHandPos, position) > maxDistance)
			{
				Vector3 serverLeftHandPos = Main.ServerLeftHandPos;
				Vector3 val = position - Main.ServerLeftHandPos;
				position = serverLeftHandPos + ((Vector3)(ref val)).normalized * maxDistance;
			}
			List<GameEntity> list = gameEntityManager.entities.Where((GameEntity e) => (Object)(object)e != (Object)null && e.typeId == hash && Vector3.Distance(Main.ServerLeftHandPos, ((Component)e).transform.position) < maxDistance && Vector3.Distance(((Component)GorillaTagger.Instance.bodyCollider).transform.position, ((Component)e).transform.position) > 3f && gameEntityManager.ValidateGrab(e, PhotonNetwork.LocalPlayer.actorNumber, true)).ToList();
			if (list.Count <= 0)
			{
				list = gameEntityManager.entities.Where((GameEntity e) => (Object)(object)e != (Object)null && e.typeId == hash && Vector3.Distance(Main.ServerLeftHandPos, ((Component)e).transform.position) < maxDistance && gameEntityManager.ValidateGrab(e, PhotonNetwork.LocalPlayer.actorNumber, true)).ToList();
			}
			if (list.Count <= 0)
			{
				list = gameEntityManager.entities.Where((GameEntity e) => (Object)(object)e != (Object)null && e.typeId == hash && gameEntityManager.ValidateGrab(e, PhotonNetwork.LocalPlayer.actorNumber, true)).ToList();
			}
			if (list.Count <= 0)
			{
				list = gameEntityManager.entities.Where((GameEntity e) => (Object)(object)e != (Object)null && e.typeId == hash).ToList();
			}
			if (list.Count <= 0)
			{
				return null;
			}
			GameEntity val2 = list.OrderByDescending((GameEntity entity) => ((Component)entity).transform.position.Distance(((Component)GorillaTagger.Instance.bodyCollider).transform.position)).FirstOrDefault();
			if (Vector3.Distance(((Component)val2).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) > maxDistance)
			{
				((Behaviour)VRRig.LocalRig).enabled = false;
				((Component)VRRig.LocalRig).transform.position = ((Component)val2).transform.position - Vector3.one * 5f;
				if (CritterCoroutine != null)
				{
					((MonoBehaviour)CoroutineManager.instance).StopCoroutine(CritterCoroutine);
				}
				CritterCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(RopeEnableRig());
			}
			if (Vector3.Distance(((Component)val2).transform.position, Main.ServerPos) < maxDistance && Time.time > ghostReactorDelay)
			{
				ghostReactorDelay = Time.time + 0.1f;
				((Component)val2).transform.position = GorillaTagger.Instance.rightHandTransform.position;
				((Component)val2).transform.rotation = RandomUtilities.RandomQuaternion();
				val2.RequestGrab(isLeftHand: true, Vector3.zero, Quaternion.identity, gameEntityManager);
				Main.RPCProtection();
			}
		}
		return null;
	}

	public static void BetaFireBlaster(Vector3 position, Vector3 direction)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		SIGadgetChargeBlaster blaster = GetBlaster();
		if ((Object)(object)blaster == (Object)null)
		{
			return;
		}
		Quaternion val = Quaternion.LookRotation(direction);
		if (Time.time < blasterDelay)
		{
			return;
		}
		blasterDelay = Time.time + SIPlayer.LocalPlayer.clientToClientRPCLimiter.GetDelay();
		Vector3 val2 = position - GorillaTagger.Instance.leftHandTransform.position;
		if (((Vector3)(ref val2)).magnitude > blaster.blaster.maxLagDistance)
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = position - Vector3.one;
			if (BlasterCoroutine != null)
			{
				((MonoBehaviour)CoroutineManager.instance).StopCoroutine(BlasterCoroutine);
			}
			BlasterCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(RopeEnableRig());
		}
		blaster.blaster.lastFired = 0f;
		blaster.FireProjectile(blaster.maxChargeDiff, blaster.blaster.NextFireId(), position, val);
		Main.RPCProtection();
	}

	public static void BetaFireBlaster(Vector3 position, Vector3 direction, NetPlayer target, bool ignoreDistnace = false)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		perPlayerDictionary.TryGetValue(target, out var value);
		Quaternion val = Quaternion.LookRotation(direction);
		if (Time.time < value)
		{
			return;
		}
		SIGadgetChargeBlaster blaster = GetBlaster();
		if ((Object)(object)blaster == (Object)null)
		{
			return;
		}
		perPlayerDictionary[target] = Time.time + SIPlayer.LocalPlayer.clientToClientRPCLimiter.GetDelay();
		if (!ignoreDistnace)
		{
			Vector3 val2 = position - GorillaTagger.Instance.leftHandTransform.position;
			if (((Vector3)(ref val2)).magnitude > blaster.blaster.maxLagDistance)
			{
				((Behaviour)VRRig.LocalRig).enabled = false;
				((Component)VRRig.LocalRig).transform.position = position - Vector3.one;
				if (BlasterCoroutine != null)
				{
					((MonoBehaviour)CoroutineManager.instance).StopCoroutine(BlasterCoroutine);
				}
				BlasterCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(RopeEnableRig());
			}
		}
		blaster.blaster.lastFired = 0f;
		SuperInfectionManager sIManagerForZone = SuperInfectionManager.GetSIManagerForZone(((SIGadget)blaster.blaster).gameEntity.manager.zone);
		if (sIManagerForZone != null)
		{
			sIManagerForZone.photonView.RPC("SIClientToClientRPC", target.GetPlayer(), new object[2]
			{
				3,
				new object[3]
				{
					((SIGadget)blaster.blaster).gameEntity.GetNetId(),
					0,
					new object[4]
					{
						blaster.maxChargeDiff,
						blaster.blaster.NextFireId(),
						position,
						val
					}
				}
			});
		}
		Main.RPCProtection();
	}

	public static void BlasterLaserSpam()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab)
		{
			BetaFireBlaster(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.forward);
		}
	}

	public static void BlasterFlingGun(Vector3 direction)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				BetaFireBlaster(((Component)Main.lockTarget).transform.position, ((Vector3)(ref direction)).normalized);
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
			Main.gunLocked = false;
		}
	}

	public static void BlasterFlingAll(Vector3 direction)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)GetBlaster() == (Object)null)
		{
			SerializePatch.OverrideSerialization = null;
		}
		else if (SerializePatch.OverrideSerialization == null)
		{
			SerializePatch.OverrideSerialization = delegate
			{
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Expected O, but got Unknown
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
				Vector3 position = ((Component)VRRig.LocalRig).transform.position;
				NetPlayer[] playerListOthers2 = NetworkSystem.Instance.PlayerListOthers;
				foreach (NetPlayer val2 in playerListOthers2)
				{
					VRRig vRRigFromPlayer2 = RigUtilities.GetVRRigFromPlayer(val2);
					((Component)VRRig.LocalRig).transform.position = ((Component)vRRigFromPlayer2).transform.position - Vector3.up;
					PhotonView photonView = VRRig.LocalRig.GetPhotonView();
					RaiseEventOptions val3 = new RaiseEventOptions();
					val3.TargetActors = new int[1] { val2.ActorNumber };
					Main.SendSerialize(photonView, val3);
				}
				Main.RPCProtection();
				((Component)VRRig.LocalRig).transform.position = position;
				return false;
			};
		}
		NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
		foreach (NetPlayer val in playerListOthers)
		{
			VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(val);
			BetaFireBlaster(((Component)vRRigFromPlayer).transform.position, direction, val, ignoreDistnace: true);
		}
	}

	public static void BlasterFlingTowardsGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				BetaFireBlaster(((Component)Main.lockTarget).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position - ((Component)Main.lockTarget).transform.position);
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
			Main.gunLocked = false;
		}
	}

	public static void BlasterFlingTowardsAll()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)GetBlaster() == (Object)null)
		{
			SerializePatch.OverrideSerialization = null;
		}
		else if (SerializePatch.OverrideSerialization == null)
		{
			SerializePatch.OverrideSerialization = delegate
			{
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Expected O, but got Unknown
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
				Vector3 position = ((Component)VRRig.LocalRig).transform.position;
				NetPlayer[] playerListOthers2 = NetworkSystem.Instance.PlayerListOthers;
				foreach (NetPlayer val2 in playerListOthers2)
				{
					VRRig vRRigFromPlayer2 = RigUtilities.GetVRRigFromPlayer(val2);
					((Component)VRRig.LocalRig).transform.position = ((Component)vRRigFromPlayer2).transform.position - Vector3.up;
					PhotonView photonView = VRRig.LocalRig.GetPhotonView();
					RaiseEventOptions val3 = new RaiseEventOptions();
					val3.TargetActors = new int[1] { val2.ActorNumber };
					Main.SendSerialize(photonView, val3);
				}
				Main.RPCProtection();
				((Component)VRRig.LocalRig).transform.position = position;
				return false;
			};
		}
		NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
		foreach (NetPlayer val in playerListOthers)
		{
			VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(val);
			BetaFireBlaster(((Component)vRRigFromPlayer).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position - ((Component)vRRigFromPlayer).transform.position, val, ignoreDistnace: true);
		}
	}

	public static void BlasterFlingAwayGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				BetaFireBlaster(((Component)Main.lockTarget).transform.position, ((Component)Main.lockTarget).transform.position - ((Component)GorillaTagger.Instance.bodyCollider).transform.position);
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
			Main.gunLocked = false;
		}
	}

	public static void BlasterFlingAwayAll()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)GetBlaster() == (Object)null)
		{
			SerializePatch.OverrideSerialization = null;
		}
		else if (SerializePatch.OverrideSerialization == null)
		{
			SerializePatch.OverrideSerialization = delegate
			{
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Expected O, but got Unknown
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
				Vector3 position = ((Component)VRRig.LocalRig).transform.position;
				NetPlayer[] playerListOthers2 = NetworkSystem.Instance.PlayerListOthers;
				foreach (NetPlayer val2 in playerListOthers2)
				{
					VRRig vRRigFromPlayer2 = RigUtilities.GetVRRigFromPlayer(val2);
					((Component)VRRig.LocalRig).transform.position = ((Component)vRRigFromPlayer2).transform.position - Vector3.up;
					PhotonView photonView = VRRig.LocalRig.GetPhotonView();
					RaiseEventOptions val3 = new RaiseEventOptions();
					val3.TargetActors = new int[1] { val2.ActorNumber };
					Main.SendSerialize(photonView, val3);
				}
				Main.RPCProtection();
				((Component)VRRig.LocalRig).transform.position = position;
				return false;
			};
		}
		NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
		foreach (NetPlayer val in playerListOthers)
		{
			VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(val);
			BetaFireBlaster(((Component)vRRigFromPlayer).transform.position, ((Component)vRRigFromPlayer).transform.position - ((Component)GorillaTagger.Instance.bodyCollider).transform.position, val, ignoreDistnace: true);
		}
	}

	public static void BlasterKickGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Vector3 position = ((Component)Main.lockTarget).transform.position;
				_003F direction;
				if (!(((Component)Main.lockTarget).transform.position.z < -28.5f))
				{
					direction = ((((Component)Main.lockTarget).transform.position.z < -23f) ? new Vector3(-50f, 0f, 50f) : Vector3.left);
				}
				else
				{
					Vector3 val2 = new Vector3(-47.82025f, 6.460508f, -29.04836f) - ((Component)Main.lockTarget).transform.position;
					direction = ((Vector3)(ref val2)).normalized;
				}
				BetaFireBlaster(position, (Vector3)direction);
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
			Main.gunLocked = false;
		}
	}

	public static void BlasterKickAll()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)GetBlaster() == (Object)null)
		{
			SerializePatch.OverrideSerialization = null;
		}
		else if (SerializePatch.OverrideSerialization == null)
		{
			SerializePatch.OverrideSerialization = delegate
			{
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Expected O, but got Unknown
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
				Vector3 position2 = ((Component)VRRig.LocalRig).transform.position;
				NetPlayer[] playerListOthers2 = NetworkSystem.Instance.PlayerListOthers;
				foreach (NetPlayer val3 in playerListOthers2)
				{
					VRRig vRRigFromPlayer2 = RigUtilities.GetVRRigFromPlayer(val3);
					((Component)VRRig.LocalRig).transform.position = ((Component)vRRigFromPlayer2).transform.position - Vector3.up;
					PhotonView photonView = VRRig.LocalRig.GetPhotonView();
					RaiseEventOptions val4 = new RaiseEventOptions();
					val4.TargetActors = new int[1] { val3.ActorNumber };
					Main.SendSerialize(photonView, val4);
				}
				Main.RPCProtection();
				((Component)VRRig.LocalRig).transform.position = position2;
				return false;
			};
		}
		NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
		foreach (NetPlayer val in playerListOthers)
		{
			VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(val);
			Vector3 position = ((Component)vRRigFromPlayer).transform.position;
			_003F direction;
			if (!(((Component)vRRigFromPlayer).transform.position.z < -28.5f))
			{
				direction = ((((Component)vRRigFromPlayer).transform.position.z < -23f) ? new Vector3(-50f, 0f, 50f) : Vector3.left);
			}
			else
			{
				Vector3 val2 = new Vector3(-47.82025f, 6.460508f, -29.04836f) - ((Component)vRRigFromPlayer).transform.position;
				direction = ((Vector3)(ref val2)).normalized;
			}
			BetaFireBlaster(position, (Vector3)direction, val, ignoreDistnace: true);
		}
	}

	public static void BlasterCrashGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				BetaFireBlaster(((Component)Main.lockTarget).transform.position, (((Component)Main.lockTarget).transform.position.y > 55f) ? Vector3.right : Vector3.up);
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
			Main.gunLocked = false;
		}
	}

	public static void BlasterCrashAll()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)GetBlaster() == (Object)null)
		{
			SerializePatch.OverrideSerialization = null;
		}
		else if (SerializePatch.OverrideSerialization == null)
		{
			SerializePatch.OverrideSerialization = delegate
			{
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Expected O, but got Unknown
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
				Vector3 position = ((Component)VRRig.LocalRig).transform.position;
				NetPlayer[] playerListOthers2 = NetworkSystem.Instance.PlayerListOthers;
				foreach (NetPlayer val2 in playerListOthers2)
				{
					VRRig vRRigFromPlayer2 = RigUtilities.GetVRRigFromPlayer(val2);
					((Component)VRRig.LocalRig).transform.position = ((Component)vRRigFromPlayer2).transform.position - Vector3.up;
					PhotonView photonView = VRRig.LocalRig.GetPhotonView();
					RaiseEventOptions val3 = new RaiseEventOptions();
					val3.TargetActors = new int[1] { val2.ActorNumber };
					Main.SendSerialize(photonView, val3);
				}
				Main.RPCProtection();
				((Component)VRRig.LocalRig).transform.position = position;
				return false;
			};
		}
		NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
		foreach (NetPlayer val in playerListOthers)
		{
			VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(val);
			BetaFireBlaster(((Component)vRRigFromPlayer).transform.position, (((Component)vRRigFromPlayer).transform.position.y > 55f) ? Vector3.right : Vector3.up, val, ignoreDistnace: true);
		}
	}

	public static void BlasterControlGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && Main.lockTarget.Distance(((Component)GorillaTagger.Instance.bodyCollider).transform.position) > 0.5f)
			{
				BetaFireBlaster(((Component)Main.lockTarget).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position - ((Component)Main.lockTarget).transform.position);
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
			Main.gunLocked = false;
		}
	}

	public static void SpamGadgetGrip(int objectId)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.rightGrab)
		{
			return;
		}
		if ((Object)(object)SuperInfectionManager.activeSuperInfectionManager == (Object)null)
		{
			if (Time.time > siErrorCooldown)
			{
				NotificationManager.SendNotification("<color=red>[ERROR]</color> Super Infection manager is not active.");
				siErrorCooldown = Time.time + 3f;
			}
		}
		else
		{
			CreateItem((object)(RpcTarget)0, objectId, GorillaTagger.Instance.rightHandTransform.position, RandomUtilities.RandomQuaternion(), GorillaTagger.Instance.rightHandTransform.forward * Main.ShootStrength, Vector3.zero, 0L, ManagerRegistry.SuperInfection.GameEntityManager);
		}
	}

	public static void SpamGadgetGun(int objectId)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		GameObject item = Main.RenderGun().NewPointer;
		if (!Main.GetGunInput(isShooting: true))
		{
			return;
		}
		if ((Object)(object)SuperInfectionManager.activeSuperInfectionManager == (Object)null)
		{
			if (Time.time > siErrorCooldown)
			{
				NotificationManager.SendNotification("<color=red>[ERROR]</color> Super Infection manager is not active.");
				siErrorCooldown = Time.time + 3f;
			}
		}
		else
		{
			CreateItem((object)(RpcTarget)0, objectId, item.transform.position, RandomUtilities.RandomQuaternion(), Vector3.zero, Vector3.zero, 0L, ManagerRegistry.SuperInfection.GameEntityManager);
		}
	}

	public static void GadgetSpamGrip()
	{
		if ((Object)(object)SuperInfectionManager.activeSuperInfectionManager == (Object)null)
		{
			if (Time.time > siErrorCooldown)
			{
				NotificationManager.SendNotification("<color=red>[ERROR]</color> Super Infection manager is not active.");
				siErrorCooldown = Time.time + 3f;
			}
		}
		else
		{
			int[] array = GadgetByName.Select((KeyValuePair<string, int> element) => element.Value).ToArray();
			SpamGadgetGrip(array[Random.Range(0, array.Length)]);
		}
	}

	public static void GadgetSpamGun()
	{
		if ((Object)(object)SuperInfectionManager.activeSuperInfectionManager == (Object)null)
		{
			if (Time.time > siErrorCooldown)
			{
				NotificationManager.SendNotification("<color=red>[ERROR]</color> Super Infection manager is not active.");
				siErrorCooldown = Time.time + 3f;
			}
		}
		else
		{
			int[] array = GadgetByName.Select((KeyValuePair<string, int> element) => element.Value).ToArray();
			SpamGadgetGun(array[Random.Range(0, array.Length)]);
		}
	}

	public static void RainGadgets()
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)SuperInfectionManager.activeSuperInfectionManager == (Object)null)
		{
			if (Time.time > siErrorCooldown)
			{
				NotificationManager.SendNotification("<color=red>[ERROR]</color> Super Infection manager is not active.");
				siErrorCooldown = Time.time + 3f;
			}
		}
		else
		{
			int[] array = GadgetByName.Select((KeyValuePair<string, int> element) => element.Value).ToArray();
			CreateItem((object)(RpcTarget)0, array[Random.Range(0, array.Length)], ((Component)VRRig.LocalRig).transform.position + new Vector3(Random.Range(-3f, 3f), 4f, Random.Range(-3f, 3f)), Quaternion.identity, Vector3.down, Vector3.zero, 0L, ManagerRegistry.SuperInfection.GameEntityManager);
		}
	}

	public static void GadgetAura()
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)SuperInfectionManager.activeSuperInfectionManager == (Object)null)
		{
			if (Time.time > siErrorCooldown)
			{
				NotificationManager.SendNotification("<color=red>[ERROR]</color> Super Infection manager is not active.");
				siErrorCooldown = Time.time + 3f;
			}
			return;
		}
		int[] array = GadgetByName.Select((KeyValuePair<string, int> element) => element.Value).ToArray();
		object target = (object)(RpcTarget)0;
		int hash = array[Random.Range(0, array.Length)];
		Vector3 position = ((Component)VRRig.LocalRig).transform.position;
		Vector3 val = RandomUtilities.RandomVector3();
		CreateItem(target, hash, position + ((Vector3)(ref val)).normalized * 2f, Quaternion.identity, Vector3.down, Vector3.zero, 0L, ManagerRegistry.SuperInfection.GameEntityManager);
	}

	public static void GadgetFountain()
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)SuperInfectionManager.activeSuperInfectionManager == (Object)null)
		{
			if (Time.time > siErrorCooldown)
			{
				NotificationManager.SendNotification("<color=red>[ERROR]</color> Super Infection manager is not active.");
				siErrorCooldown = Time.time + 3f;
			}
		}
		else
		{
			int[] array = GadgetByName.Select((KeyValuePair<string, int> element) => element.Value).ToArray();
			CreateItem((object)(RpcTarget)0, array[Random.Range(0, array.Length)], ((Component)VRRig.LocalRig).transform.position + Vector3.up * 3f, Quaternion.identity, RandomUtilities.RandomVector3(15f), Vector3.zero, 0L, ManagerRegistry.SuperInfection.GameEntityManager);
		}
	}

	public static void ResourceSpamGrip()
	{
		if ((Object)(object)SuperInfectionManager.activeSuperInfectionManager == (Object)null)
		{
			if (Time.time > siErrorCooldown)
			{
				NotificationManager.SendNotification("<color=red>[ERROR]</color> Super Infection manager is not active.");
				siErrorCooldown = Time.time + 3f;
			}
			return;
		}
		int[] array = (from x in GadgetByName
			where x.Key.Contains("Resource")
			select x.Value).ToArray();
		SpamGadgetGrip(array[Random.Range(0, array.Length)]);
	}

	public static void ResourceSpamGun()
	{
		if ((Object)(object)SuperInfectionManager.activeSuperInfectionManager == (Object)null)
		{
			if (Time.time > siErrorCooldown)
			{
				NotificationManager.SendNotification("<color=red>[ERROR]</color> Super Infection manager is not active.");
				siErrorCooldown = Time.time + 3f;
			}
			return;
		}
		int[] array = (from x in GadgetByName
			where x.Key.Contains("Resource")
			select x.Value).ToArray();
		SpamGadgetGun(array[Random.Range(0, array.Length)]);
	}

	public static void DestroyGadgetGun()
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		GameObject item = Main.RenderGun().NewPointer;
		if (!Main.GetGunInput(isShooting: true) || !(Time.time > destroyDelay))
		{
			return;
		}
		GameEntity val = null;
		float num = float.MaxValue;
		foreach (GameEntity entity in ManagerRegistry.SuperInfection.GameEntityManager.entities)
		{
			if ((Object)(object)entity != (Object)null)
			{
				float num2 = Vector3.Distance(item.transform.position, ((Component)entity).transform.position);
				if (num2 < 0.75f && num2 < num)
				{
					val = entity;
					num = num2;
				}
			}
		}
		if ((Object)(object)val != (Object)null)
		{
			destroyDelay = Time.time + 0.02f;
			if (NetworkSystem.Instance.IsMasterClient)
			{
				ManagerRegistry.SuperInfection.GameEntityManager.photonView.RPC("DestroyItemRPC", (RpcTarget)0, new object[1] { new int[1] { val.GetNetId() } });
				Main.RPCProtection();
			}
			else
			{
				val.RequestGrab(isLeftHand: true, Vector3.zero, Quaternion.identity, ManagerRegistry.SuperInfection.GameEntityManager);
				val.RequestThrow(isLeftHand: true, ((Component)GorillaTagger.Instance.bodyCollider).transform.position - Vector3.up * 14f, Quaternion.identity, Vector3.zero, Vector3.zero, ManagerRegistry.SuperInfection.GameEntityManager);
			}
		}
	}

	public static void SpawnBlueLucy()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		HalloweenGhostChaser lucy = Lucy;
		if (((NetworkView)lucy).IsMine)
		{
			lucy.timeGongStarted = Time.time;
			lucy.currentState = (ChaseState)4;
			lucy.isSummoned = false;
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
	}

	public static void SpawnRedLucy()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		HalloweenGhostChaser lucy = Lucy;
		if (((NetworkView)lucy).IsMine)
		{
			lucy.timeGongStarted = Time.time;
			lucy.currentState = (ChaseState)4;
			lucy.isSummoned = true;
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
	}

	public static void DespawnLucy()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		HalloweenGhostChaser lucy = Lucy;
		if (((NetworkView)lucy).IsMine)
		{
			lucy.currentState = (ChaseState)1;
			lucy.isSummoned = false;
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
	}

	public static void LucyChase(NetPlayer player)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		HalloweenGhostChaser lucy = Lucy;
		if (((NetworkView)lucy).IsMine)
		{
			lucy.currentState = (ChaseState)8;
			lucy.targetPlayer = player;
			lucy.followTarget = ((Component)GorillaTagger.Instance.offlineVRRig).transform;
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
	}

	public static void LucyChaseGun()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true))
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				LucyChase(componentInParent.GetPlayer());
			}
		}
	}

	public static void LucyAttack(NetPlayer player)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		HalloweenGhostChaser lucy = Lucy;
		if (((NetworkView)lucy).IsMine)
		{
			if (Time.time > lucy.grabTime + lucy.grabDuration + 0.1f)
			{
				if (lucy.targetPlayer != player)
				{
					lucy.currentState = (ChaseState)1;
					Main.SendSerialize(((NetworkView)lucy).GetView);
				}
				lucy.currentState = (ChaseState)16;
				lucy.grabTime = Time.time;
				lucy.targetPlayer = player;
			}
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
	}

	public static void LucyAttackGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				LucyAttack(Main.lockTarget.GetPlayer());
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
			Main.gunLocked = false;
		}
	}

	public static void LucyHarassGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Invalid comparison between Unknown and I4
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				HalloweenGhostChaser lucy = Lucy;
				if (((NetworkView)lucy).IsMine)
				{
					if (Time.time > lucyDelay)
					{
						lucy.currentState = (ChaseState)(((int)lucy.currentState == 16) ? 8 : 16);
						((Component)lucy).transform.position = ((Component)Main.lockTarget).transform.position + Vector3.up;
						lucy.currentSpeed = 0f;
						lucy.targetPlayer = RigUtilities.GetPlayerFromVRRig(Main.lockTarget);
						lucy.followTarget = ((Component)Main.lockTarget).transform;
						lucyDelay = Time.time + 0.1f;
					}
				}
				else
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
				}
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
			Main.gunLocked = false;
		}
	}

	public static void LucyAttackAll()
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected O, but got Unknown
		HalloweenGhostChaser hgc = Lucy;
		if (SerializePatch.OverrideSerialization != null)
		{
			SerializePatch.OverrideSerialization = delegate
			{
				Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { ((NetworkView)hgc).GetView });
				return false;
			};
		}
		if (((NetworkView)hgc).IsMine)
		{
			if (Time.time > hgc.grabTime + hgc.grabDuration + 0.1f)
			{
				NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
				foreach (NetPlayer val in playerListOthers)
				{
					hgc.currentState = (ChaseState)16;
					hgc.grabTime = Time.time;
					hgc.targetPlayer = val;
					PhotonView getView = ((NetworkView)Lucy).GetView;
					RaiseEventOptions val2 = new RaiseEventOptions();
					val2.TargetActors = new int[1] { val.ActorNumber };
					Main.SendSerialize(getView, val2);
				}
			}
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
	}

	public static void SpazLucy()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		HalloweenGhostChaser lucy = Lucy;
		if (((NetworkView)lucy).IsMine)
		{
			if (Time.time > lucyDelay)
			{
				lucy.timeGongStarted = ((lucy.timeGongStarted == 0f) ? Time.time : 0f);
				lucy.currentState = (ChaseState)4;
				lucy.isSummoned = true;
				lucyDelay = Time.time + 0.1f;
			}
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
	}

	public static void AnnoyingLucy()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Invalid comparison between Unknown and I4
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		HalloweenGhostChaser lucy = Lucy;
		if (((NetworkView)lucy).IsMine)
		{
			if (Time.time > lucyDelay)
			{
				lucy.timeGongStarted = Time.time;
				lucy.grabTime = Time.time;
				lucy.currentState = (ChaseState)(((int)lucy.currentState == 4) ? 16 : 4);
				lucy.targetPlayer = NetPlayer.op_Implicit(RigUtilities.GetRandomPlayer(includeSelf: true));
				lucyDelay = Time.time + 0.1f;
			}
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
	}

	public static void BecomeLucy()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
		else if ((Object)(object)Lucy != (Object)null)
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position - Vector3.up * 99999f;
			((Component)Lucy).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
			((Component)Lucy).transform.rotation = ((Component)GorillaTagger.Instance.headCollider).transform.rotation;
			Lucy.currentState = (ChaseState)8;
			Lucy.targetPlayer = null;
		}
	}

	public static void MoveLucyGun()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		GameObject item = Main.RenderGun().NewPointer;
		if (Main.GetGunInput(isShooting: true))
		{
			if (((NetworkView)Lucy).IsMine)
			{
				((Component)Lucy).transform.position = item.transform.position + Vector3.up;
			}
			else
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			}
		}
	}

	public static void FastLucy()
	{
		HalloweenGhostChaser lucy = Lucy;
		if (((NetworkView)lucy).IsMine)
		{
			lucy.currentSpeed = 10f;
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
	}

	public static void SlowLucy()
	{
		HalloweenGhostChaser lucy = Lucy;
		if (((NetworkView)lucy).IsMine)
		{
			lucy.currentSpeed = 1f;
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
	}

	public static void SpawnLurker()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (((NetworkView)Lurker).IsMine)
		{
			Lurker.currentState = (ghostState)0;
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
	}

	public static void MoveLurkerGun()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		GameObject item = Main.RenderGun().NewPointer;
		if (Main.GetGunInput(isShooting: true))
		{
			if (((NetworkView)Lurker).IsMine)
			{
				((Component)Lurker).transform.position = item.transform.position + Vector3.up;
			}
			else
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			}
		}
	}

	public static void DespawnLurker()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (((NetworkView)Lurker).IsMine)
		{
			Lurker.currentState = (ghostState)0;
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
	}

	public static void LurkerAttack(NetPlayer player)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (((NetworkView)Lurker).IsMine)
		{
			if (Lurker.targetPlayer != player)
			{
				Lurker.ChangeState((ghostState)0);
				Main.SendSerialize(((NetworkView)Lurker).GetView);
			}
			Lurker.currentState = (ghostState)3;
			Lurker.targetPlayer = player;
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
	}

	public static void LurkerAttackGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				LurkerAttack(Main.lockTarget.GetPlayer());
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
			Main.gunLocked = false;
		}
	}

	public static void LurkerAttackAll()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Invalid comparison between Unknown and I4
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		if (SerializePatch.OverrideSerialization != null)
		{
			SerializePatch.OverrideSerialization = delegate
			{
				Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { ((NetworkView)Lurker).GetView });
				return false;
			};
		}
		if (((NetworkView)Lurker).IsMine)
		{
			if ((int)Lurker.currentState != 3)
			{
				NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
				foreach (NetPlayer val in playerListOthers)
				{
					Lurker.currentState = (ghostState)3;
					Lurker.targetPlayer = val;
					PhotonView getView = ((NetworkView)Lucy).GetView;
					RaiseEventOptions val2 = new RaiseEventOptions();
					val2.TargetActors = new int[1] { val.ActorNumber };
					Main.SendSerialize(getView, val2);
				}
			}
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
	}

	public static void SpazLurker()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Invalid comparison between Unknown and I4
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (((NetworkView)Lurker).IsMine)
		{
			if (Time.time > lurkerDelay)
			{
				Lurker.currentState = (ghostState)(((int)Lurker.currentState == 2) ? 1 : 2);
				Lurker.targetPlayer = NetPlayer.op_Implicit(RigUtilities.GetRandomPlayer(includeSelf: true));
				lurkerDelay = Time.time + 0.1f;
			}
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
	}

	public static void BreakLurker()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Invalid comparison between Unknown and I4
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (((NetworkView)Lurker).IsMine)
		{
			Lurker.currentState = (ghostState)(((int)Lurker.currentState == 2) ? 3 : 2);
			Lurker.targetPlayer = NetPlayer.op_Implicit(RigUtilities.GetRandomPlayer(includeSelf: true));
			Main.SendSerialize(((NetworkView)Lurker).GetView);
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
	}

	public static void AnnoyingLurker()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Invalid comparison between Unknown and I4
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (((NetworkView)Lurker).IsMine)
		{
			if (Time.time > lurkerDelay)
			{
				Lurker.currentState = (ghostState)(((int)Lurker.currentState == 3) ? 2 : 3);
				Lurker.targetPlayer = NetPlayer.op_Implicit(RigUtilities.GetRandomPlayer(includeSelf: true));
				lurkerDelay = Time.time + 0.1f;
			}
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
	}

	public static void BecomeLurker()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
		else
		{
			if (!((Object)(object)Lurker != (Object)null))
			{
				return;
			}
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position - Vector3.up * 99999f;
			((Component)Lurker).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
			((Component)Lurker).transform.rotation = ((Component)GorillaTagger.Instance.headCollider).transform.rotation;
			Lurker.currentState = (ghostState)1;
			SerializePatch.OverrideSerialization = delegate
			{
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Expected O, but got Unknown
				Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
				NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
				foreach (NetPlayer val in playerListOthers)
				{
					Lurker.targetPlayer = val;
					PhotonView getView = ((NetworkView)Lurker).GetView;
					RaiseEventOptions val2 = new RaiseEventOptions();
					val2.TargetActors = new int[1] { val.ActorNumber };
					Main.SendSerialize(getView, val2);
				}
				Main.RPCProtection();
				return false;
			};
		}
	}

	public static void BetaSetVelocityPlayer(NetPlayer victim, Vector3 velocity)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		if (((Vector3)(ref velocity)).sqrMagnitude > 20f)
		{
			velocity = Vector3.Normalize(velocity) * 20f;
		}
		GorillaGuardianManager val = (GorillaGuardianManager)GorillaGameManager.instance;
		if (val.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
		{
			RigUtilities.GetNetworkViewFromVRRig(RigUtilities.GetVRRigFromPlayer(victim)).SendRPC("GrabbedByPlayer", victim, new object[3] { true, false, false });
			RigUtilities.GetNetworkViewFromVRRig(RigUtilities.GetVRRigFromPlayer(victim)).SendRPC("DroppedByPlayer", victim, new object[1] { velocity });
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must be guardian.");
		}
	}

	public static void BetaSetVelocityTargetGroup(RpcTarget victim, Vector3 velocity)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected I4, but got Unknown
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		if (((Vector3)(ref velocity)).sqrMagnitude > 20f)
		{
			velocity = Vector3.Normalize(velocity) * 20f;
		}
		GorillaGuardianManager val = (GorillaGuardianManager)GorillaGameManager.instance;
		if (val.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
		{
			switch ((int)victim)
			{
			case 0:
			{
				foreach (VRRig activeRig in VRRigCache.ActiveRigs)
				{
					RigUtilities.GetNetworkViewFromVRRig(activeRig).SendRPC("GrabbedByPlayer", RigUtilities.GetPlayerFromVRRig(activeRig), new object[3] { true, false, false });
					RigUtilities.GetNetworkViewFromVRRig(activeRig).SendRPC("DroppedByPlayer", RigUtilities.GetPlayerFromVRRig(activeRig), new object[1] { velocity });
				}
				break;
			}
			case 1:
			{
				foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig rig) => !rig.isLocal))
				{
					RigUtilities.GetNetworkViewFromVRRig(item).SendRPC("GrabbedByPlayer", RigUtilities.GetPlayerFromVRRig(item), new object[3] { true, false, false });
					RigUtilities.GetNetworkViewFromVRRig(item).SendRPC("DroppedByPlayer", RigUtilities.GetPlayerFromVRRig(item), new object[1] { velocity });
				}
				break;
			}
			case 2:
				RigUtilities.GetNetworkViewFromVRRig(RigUtilities.GetVRRigFromPlayer(NetworkSystem.Instance.MasterClient)).SendRPC("GrabbedByPlayer", (RpcTarget)1, new object[3] { true, false, false });
				RigUtilities.GetNetworkViewFromVRRig(RigUtilities.GetVRRigFromPlayer(NetworkSystem.Instance.MasterClient)).SendRPC("DroppedByPlayer", (RpcTarget)1, new object[1] { velocity });
				break;
			}
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must be guardian.");
		}
	}

	public static void GuardianGrabGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (!Main.GetGunInput(isShooting: true) || !(Time.time > grabDelay))
		{
			return;
		}
		VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
		if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
		{
			GorillaGuardianManager val2 = (GorillaGuardianManager)GorillaGameManager.instance;
			if (val2.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
			{
				RigUtilities.GetNetworkViewFromVRRig(componentInParent).SendRPC("GrabbedByPlayer", (RpcTarget)1, new object[3] { true, false, false });
				Main.RPCProtection();
			}
			else
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must be guardian.");
			}
			grabDelay = Time.time + 0.1f;
		}
	}

	public static void GuardianGrabAll()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		if (!Main.rightGrab || !(Time.time > grabDelay))
		{
			return;
		}
		grabDelay = Time.time + 0.1f;
		GorillaGuardianManager val = (GorillaGuardianManager)GorillaGameManager.instance;
		if (val.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
		{
			foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig plr) => !plr.isLocal))
			{
				RigUtilities.GetNetworkViewFromVRRig(item).SendRPC("GrabbedByPlayer", (RpcTarget)1, new object[3] { true, false, false });
				Main.RPCProtection();
			}
			return;
		}
		NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must be guardian.");
	}

	public static void GuardianReleaseGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (!Main.GetGunInput(isShooting: true) || !(Time.time > releaseDelay))
		{
			return;
		}
		VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
		if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
		{
			GorillaGuardianManager val2 = (GorillaGuardianManager)GorillaGameManager.instance;
			if (val2.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
			{
				RigUtilities.GetNetworkViewFromVRRig(componentInParent).SendRPC("DroppedByPlayer", (RpcTarget)1, new object[1] { (object)new Vector3(0f, 0f, 0f) });
				Main.RPCProtection();
			}
			else
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must be guardian.");
			}
			releaseDelay = Time.time + 0.1f;
		}
	}

	public static void GuardianReleaseAll()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		if (!(Main.rightTrigger > 0.5f) || !(Time.time > releaseDelay))
		{
			return;
		}
		releaseDelay = Time.time + 0.1f;
		GorillaGuardianManager val = (GorillaGuardianManager)GorillaGameManager.instance;
		if (val.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
		{
			foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig plr) => !plr.isLocal))
			{
				RigUtilities.GetNetworkViewFromVRRig(item).SendRPC("DroppedByPlayer", (RpcTarget)1, new object[1] { (object)new Vector3(0f, 0f, 0f) });
				Main.RPCProtection();
			}
			return;
		}
		NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must be guardian.");
	}

	public static void GuardianFlingGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > flingDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				BetaSetVelocityPlayer(RigUtilities.GetPlayerFromVRRig(componentInParent), new Vector3(0f, 19.9f, 0f));
				Main.RPCProtection();
				flingDelay = Time.time + 0.1f;
			}
		}
	}

	public static void GuardianFlingAll()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightTrigger > 0.5f && Time.time > flingDelay)
		{
			flingDelay = Time.time + 0.1f;
			BetaSetVelocityTargetGroup((RpcTarget)1, new Vector3(0f, 19.9f, 0f));
			Main.RPCProtection();
		}
	}

	public static void GuardianSpazPlayerGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && Time.time > flingDelay)
			{
				BetaSetVelocityPlayer(RigUtilities.GetPlayerFromVRRig(Main.lockTarget), RandomUtilities.RandomVector3(50f));
				Main.RPCProtection();
				flingDelay = Time.time + 0.1f;
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
			Main.gunLocked = false;
		}
	}

	public static void GuardianSpazAllPlayers()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightTrigger > 0.5f && Time.time > flingDelay)
		{
			flingDelay = Time.time + 0.1f;
			BetaSetVelocityTargetGroup((RpcTarget)1, RandomUtilities.RandomVector3(50f));
			Main.RPCProtection();
		}
	}

	public static void BlockCrashGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				if (!NetworkSystem.Instance.IsMasterClient)
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
					return;
				}
				Fun.RequestCreatePiece(1934114066, new Vector3(-127.6248f, 16.99441f, -217.2094f), Quaternion.identity, 0, RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(Main.lockTarget)), overrideFreeze: false, forceGravity: true);
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
			Main.gunLocked = false;
		}
	}

	public static void BlockCrashAll()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightTrigger > 0.5f)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			}
			else
			{
				Fun.RequestCreatePiece(1934114066, new Vector3(-127.6248f, 16.99441f, -217.2094f), Quaternion.identity, 0, (object)(RpcTarget)1, overrideFreeze: false, forceGravity: true);
			}
		}
	}

	public static int GetProjectileIncrement(Vector3 Position, Vector3 Velocity, float Scale)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			GameObject val = new GameObject("SlingshotProjectileHolder");
			SlingshotProjectile val2 = val.AddComponent<SlingshotProjectile>();
			int result = (archiveIncrement = ProjectileTracker.AddAndIncrementLocalProjectile(val2, Velocity, Position, Scale));
			Object.Destroy((Object)(object)val);
			return result;
		}
		catch
		{
			LogManager.Log("Falling back to archiveIncrement");
			archiveIncrement++;
			return archiveIncrement;
		}
	}

	public static void DisableSnowballImpactEffect()
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		if (PhotonNetwork.InRoom && Time.time > timeSinceCallInvalidated)
		{
			timeSinceCallInvalidated = Time.time + ((CallLimiter)RoomSystem.callbackInstance.roomSettings.PlayerEffectLimiter).timeCooldown;
			object[] array = new object[6] { -1, -1, null, null, null, null };
			PhotonNetwork.RaiseEvent((byte)3, (object)new object[3]
			{
				NetworkSystem.Instance.ServerTimestamp,
				(byte)6,
				array
			}, new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)1
			}, SendOptions.SendUnreliable);
			Main.RPCProtection();
		}
	}

	public static void ChangeSnowballScale(bool positive = true)
	{
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				snowballScale++;
			}
			else
			{
				snowballScale--;
			}
		}
		if (snowballScale > 5)
		{
			snowballScale = 0;
		}
		if (snowballScale < 0)
		{
			snowballScale = 5;
		}
		Buttons.GetIndex("Change Snowball Scale").overlapText = "Change Snowball Scale <color=grey>[</color><color=green>" + (snowballScale + 1) + "</color><color=grey>]</color>";
	}

	public static void ChangeSnowballMultiplicationFactor(bool positive = true)
	{
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				snowballMultiplicationFactor++;
			}
			else
			{
				snowballMultiplicationFactor--;
			}
		}
		if (snowballMultiplicationFactor > 5)
		{
			snowballMultiplicationFactor = 1;
		}
		if (snowballMultiplicationFactor < 1)
		{
			snowballMultiplicationFactor = 5;
		}
		Buttons.GetIndex("Change Snowball Multiplication Factor").overlapText = "Change Snowball Multiplication Factor <color=grey>[</color><color=green>" + snowballMultiplicationFactor + "</color><color=grey>]</color>";
	}

	public static IEnumerator DisableSnowball(bool rigDisabled)
	{
		yield return (object)new WaitForSeconds(0.3f);
		if (rigDisabled)
		{
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
		DistancePatch.enabled = false;
		foreach (SnowballThrowable snowball in Main.snowballDict.Values)
		{
			try
			{
				snowball.SetSnowballActiveLocal(false);
			}
			catch
			{
			}
		}
	}

	public static IEnumerator InvisibleSnowball(Vector3 Pos, Vector3 Vel, int Mode, Player Target = null, int? customScale = null, bool ignoreMultiply = false)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			yield break;
		}
		RaiseEventOptions options = null;
		switch (Mode)
		{
		case 0:
			options = new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)1
			};
			break;
		case 1:
			options = new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)0
			};
			break;
		case 2:
		{
			RaiseEventOptions val = new RaiseEventOptions();
			val.TargetActors = new int[1] { Target.ActorNumber };
			options = val;
			break;
		}
		}
		SnowballThrowable left = Main.GetProjectile(Projectiles.SnowballName + "LeftAnchor");
		SnowballThrowable right = Main.GetProjectile(Projectiles.SnowballName + "RightAnchor");
		left.SetSnowballActiveLocal(true);
		right.SetSnowballActiveLocal(true);
		Main.SendSerialize(GorillaTagger.Instance.myVRRig.reliableView, options);
		left.SetSnowballActiveLocal(false);
		right.SetSnowballActiveLocal(false);
		Main.RPCProtection();
		yield return null;
		yield return null;
		InvisibleSnowballs = false;
		BetaSpawnSnowball(Pos, Vel, Mode, Target, customScale, ignoreMultiply);
		InvisibleSnowballs = true;
		foreach (SnowballThrowable snowball in Main.snowballDict.Values)
		{
			try
			{
				snowball.SetSnowballActiveLocal(false);
			}
			catch
			{
			}
		}
		Main.SendSerialize(GorillaTagger.Instance.myVRRig.reliableView, options);
		Main.RPCProtection();
	}

	public static void BetaSpawnSnowball(Vector3 Pos, Vector3 Vel, int Mode, Player Target = null, int? customScale = null, bool ignoreMultiply = false)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Invalid comparison between Unknown and I4
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Invalid comparison between Unknown and I4
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		if (InvisibleSnowballs)
		{
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(InvisibleSnowball(Pos, Vel, Mode, Target, customScale, ignoreMultiply));
			return;
		}
		try
		{
			RaiseEventOptions val = null;
			switch (Mode)
			{
			case 0:
				val = new RaiseEventOptions
				{
					Receivers = (ReceiverGroup)1
				};
				break;
			case 1:
				val = new RaiseEventOptions
				{
					Receivers = (ReceiverGroup)0
				};
				break;
			case 2:
			{
				RaiseEventOptions val2 = new RaiseEventOptions();
				val2.TargetActors = new int[1] { Target.ActorNumber };
				val = val2;
				break;
			}
			}
			bool flag = (int)val.Receivers == 1 || (val.TargetActors != null && Extensions.Contains(val.TargetActors, PhotonNetwork.LocalPlayer.ActorNumber));
			if (flag)
			{
				if ((int)val.Receivers == 1)
				{
					val.Receivers = (ReceiverGroup)0;
				}
				if (val.TargetActors != null && Extensions.Contains(val.TargetActors, PhotonNetwork.LocalPlayer.ActorNumber))
				{
					List<int> list = val.TargetActors.ToList();
					list.Remove(PhotonNetwork.LocalPlayer.ActorNumber);
					val.TargetActors = list.ToArray();
				}
			}
			Vector3 position = ((Component)VRRig.LocalRig).transform.position;
			bool flag2 = Vector3.Distance(Pos, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) > 3.9f;
			if (flag2)
			{
				if (!NoTeleportSnowballs)
				{
					((Behaviour)VRRig.LocalRig).enabled = false;
				}
				((Component)VRRig.LocalRig).transform.position = Pos + new Vector3(0f, (Vel.y > 0f) ? (-3f) : 3f, 0f);
			}
			if (NoTeleportSnowballs && flag2)
			{
				Main.SendSerialize(VRRig.LocalRig.GetPhotonView(), val, -10);
				Main.SendSerialize(VRRig.LocalRig.GetPhotonView(), val);
			}
			int num = customScale ?? snowballScale;
			for (int i = 0; i < (ignoreMultiply ? 1 : snowballMultiplicationFactor); i++)
			{
				SnowballHandIndex = !SnowballHandIndex;
				if (((Vector3)(ref Vel)).magnitude > 9999f)
				{
					Vel = ((Vector3)(ref Vel)).normalized * 9999f;
				}
				DistancePatch.enabled = true;
				if (DisableCoroutine != null)
				{
					((MonoBehaviour)CoroutineManager.instance).StopCoroutine(DisableCoroutine);
				}
				DisableCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(DisableSnowball(flag2 && !NoTeleportSnowballs));
				SnowballThrowable projectile = Main.GetProjectile(Projectiles.SnowballName + (SnowballHandIndex ? "Right" : "Left") + "Anchor");
				GrowingSnowballThrowable val3 = (GrowingSnowballThrowable)(object)((projectile is GrowingSnowballThrowable) ? projectile : null);
				((SnowballThrowable)val3).linSpeedMultiplier = 10f;
				((SnowballThrowable)val3).maxLinSpeed = 99999f;
				int projectileIncrement = GetProjectileIncrement(Pos, Vel, num);
				if (flag)
				{
					SlingshotProjectile val4 = val3.SpawnGrowingSnowball(ref Vel, (float)num);
					val4.Launch(Pos, Vel, NetworkSystem.Instance.LocalPlayer, false, false, projectileIncrement, (float)num, true, Color.white);
				}
				object[] obj = new object[2]
				{
					val3.changeSizeEvent._eventId,
					num
				};
				RaiseEventOptions obj2 = val;
				SendOptions val5 = default(SendOptions);
				((SendOptions)(ref val5)).Reliability = false;
				PhotonNetwork.RaiseEvent((byte)176, (object)obj, obj2, val5);
				object[] obj3 = new object[4]
				{
					val3.snowballThrowEvent._eventId,
					Pos,
					Vel,
					projectileIncrement
				};
				RaiseEventOptions obj4 = val;
				val5 = default(SendOptions);
				((SendOptions)(ref val5)).Reliability = false;
				PhotonNetwork.RaiseEvent((byte)176, (object)obj3, obj4, val5);
				SnowballThrowable projectile2 = Main.GetProjectile(Projectiles.SnowballName + (SnowballHandIndex ? "Left" : "Right") + "Anchor");
				GrowingSnowballThrowable val6 = (GrowingSnowballThrowable)(object)((projectile2 is GrowingSnowballThrowable) ? projectile2 : null);
				((SnowballThrowable)val6).linSpeedMultiplier = 10f;
				((SnowballThrowable)val6).maxLinSpeed = 99999f;
				((SnowballThrowable)val6).SetSnowballActiveLocal(true);
			}
			if (NoTeleportSnowballs && flag2)
			{
				((Component)VRRig.LocalRig).transform.position = position;
				Main.SendSerialize(VRRig.LocalRig.GetPhotonView(), val);
			}
		}
		catch (Exception arg)
		{
			LogManager.LogError($"Error in BetaSpawnSnowball: {arg}");
		}
		Main.RPCProtection();
	}

	public static void BetaSnowballImpact(Player Target)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		object[] array = new object[6] { Target.ActorNumber, 0, null, null, null, null };
		PhotonNetwork.RaiseEvent((byte)3, (object)new object[3]
		{
			NetworkSystem.Instance.ServerTimestamp,
			(byte)6,
			array
		}, new RaiseEventOptions
		{
			Receivers = (ReceiverGroup)1
		}, SendOptions.SendUnreliable);
		Main.RPCProtection();
	}

	public static void SnowballAirstrikeGun()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true))
			{
				BetaSpawnSnowball(item.transform.position + new Vector3(0f, 50f, 0f), Vector3.zero, 0);
			}
		}
	}

	public static void SnowballNukeGun()
	{
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true))
			{
				Vector3 valueOrDefault = snowballNukePosition.GetValueOrDefault();
				if (!snowballNukePosition.HasValue)
				{
					valueOrDefault = item.transform.position + Vector3.up * 50f;
					snowballNukePosition = valueOrDefault;
				}
				Vector3? val = snowballNukePosition;
				valueOrDefault = Time.unscaledDeltaTime * Physics.gravity;
				snowballNukePosition = (val.HasValue ? new Vector3?(val.GetValueOrDefault() + valueOrDefault) : ((Vector3?)null));
				snowballNukeVelocity += Time.unscaledDeltaTime * Physics.gravity;
				BetaSpawnSnowball(snowballNukePosition.Value + GTVector3Extensions.X_Z(RandomUtilities.RandomVector3(((Vector3)(ref snowballNukeVelocity)).magnitude * 0.25f)), snowballNukeVelocity, 0);
			}
		}
		else
		{
			snowballNukePosition = null;
			snowballNukeVelocity = Vector3.zero;
		}
	}

	public static void SnowballRain()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightTrigger > 0.5f)
		{
			BetaSpawnSnowball(((Component)VRRig.LocalRig).transform.position + new Vector3(Random.Range(-5f, 5f), 5f, Random.Range(-5f, 5f)), Vector3.zero, 0);
		}
	}

	public static void SnowballHail()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightTrigger > 0.5f)
		{
			BetaSpawnSnowball(((Component)VRRig.LocalRig).transform.position + new Vector3(Random.Range(-5f, 5f), 5f, Random.Range(-5f, 5f)), new Vector3(0f, -50f, 0f), 0);
		}
	}

	public static void SnowballFountain()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightTrigger > 0.5f)
		{
			BetaSpawnSnowball(((Component)VRRig.LocalRig).transform.position + Vector3.up, new Vector3(Random.Range(-15f, 15f), Random.Range(20f, 25f), Random.Range(-15f, 15f)), 0);
		}
	}

	public static void SnowballPositionalFountain()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab)
		{
			if ((Object)(object)FountainObject == (Object)null)
			{
				FountainObject = GameObject.CreatePrimitive((PrimitiveType)0);
				Object.Destroy((Object)(object)FountainObject.GetComponent<SphereCollider>());
				FountainObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
			}
			FountainObject.transform.position = GorillaTagger.Instance.rightHandTransform.position;
		}
		if ((Object)(object)FountainObject != (Object)null)
		{
			if (Main.rightTrigger > 0.5f)
			{
				BetaSpawnSnowball(FountainObject.transform.position, new Vector3(Random.Range(-15f, 15f), Random.Range(20f, 25f), Random.Range(-15f, 15f)), 0);
			}
			else
			{
				FountainObject.GetComponent<Renderer>().material.color = Main.buttonColors[0].GetColor(0);
			}
		}
	}

	public static void DisableSnowballPositionalFountain()
	{
		if ((Object)(object)FountainObject != (Object)null)
		{
			Object.Destroy((Object)(object)FountainObject);
			FountainObject = null;
		}
	}

	public static void SnowballOrbit()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightTrigger > 0.5f)
		{
			BetaSpawnSnowball(((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(MathF.Cos((float)Time.frameCount / 30f), 2f, MathF.Sin((float)Time.frameCount / 30f)), new Vector3(0f, 50f, 0f), 0);
		}
	}

	public static void SnowballAura()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightTrigger > 0.5f)
		{
			BetaSpawnSnowball(((Component)GorillaTagger.Instance.headCollider).transform.position + RandomUtilities.RandomVector3(), RandomUtilities.RandomVector3() * 20f, 0);
		}
	}

	public static void SnowballMushroom()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightTrigger > 0.5f)
		{
			int num = 15;
			for (int i = 0; i < num; i++)
			{
				float num2 = ((i == 0) ? 0f : (360f / (float)num * (float)i));
				Vector3 pos = ((Component)GorillaTagger.Instance.headCollider).transform.position + Vector3.up;
				Vector3 val = Vector3.up * 15f;
				Vector3 val2 = Quaternion.Euler(0f, num2, 0f) * Vector3.forward;
				BetaSpawnSnowball(pos, val + ((Vector3)(ref val2)).normalized * 2.5f, 0);
			}
		}
	}

	public static void SnowballSpam()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.rightGrab && !Mouse.current.leftButton.isPressed)
		{
			return;
		}
		Vector3 pos = GorillaTagger.Instance.rightHandTransform.position;
		Vector3 val = GTPlayer.Instance.RigidbodyVelocity;
		if (Buttons.GetIndex("Shoot Projectiles").enabled)
		{
			val = GTPlayer.Instance.RigidbodyVelocity + Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * Main.ShootStrength;
			if (Mouse.current.leftButton.isPressed)
			{
				Ray val2 = Main.TPC.ScreenPointToRay(Vector2.op_Implicit(((InputControl<Vector2>)(object)((Pointer)Mouse.current).position).ReadValue()));
				RaycastHit val3 = default(RaycastHit);
				Physics.Raycast(val2, ref val3, 512f, Main.NoInvisLayerMask());
				val = ((RaycastHit)(ref val3)).point - ((Component)GorillaTagger.Instance.rightHandTransform).transform.position;
				((Vector3)(ref val)).Normalize();
				val *= Main.ShootStrength * 2f;
			}
		}
		if (Buttons.GetIndex("Random Direction").enabled)
		{
			val = RandomUtilities.RandomVector3(100f);
		}
		if (Buttons.GetIndex("Above Players").enabled)
		{
			VRRig targetPlayer = RigUtilities.GetTargetPlayer();
			pos = ((Component)targetPlayer).transform.position + Vector3.up;
		}
		if (Buttons.GetIndex("Rain Projectiles").enabled)
		{
			pos = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(Random.Range(-2f, 2f), 2f, Random.Range(-2f, 2f));
			val = Vector3.zero;
		}
		if (Buttons.GetIndex("Projectile Aura").enabled)
		{
			float num = Time.frameCount;
			pos = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(MathF.Cos(num / 20f), 2f, MathF.Sin(num / 20f));
		}
		if (Buttons.GetIndex("True Projectile Aura").enabled)
		{
			pos = ((Component)GorillaTagger.Instance.headCollider).transform.position + RandomUtilities.RandomVector3();
			val = RandomUtilities.RandomVector3(10f);
		}
		if (Buttons.GetIndex("Projectile Fountain").enabled)
		{
			pos = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(0f, 1f, 0f);
			((Vector3)(ref val))._002Ector((float)Random.Range(-10, 10), 15f, (float)Random.Range(-10, 10));
		}
		if (Buttons.GetIndex("Include Hand Velocity").enabled)
		{
			val = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0f, false);
		}
		BetaSpawnSnowball(pos, val, 0);
	}

	public static void SnowballGun()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true))
			{
				BetaSpawnSnowball(item.transform.position + Vector3.up, new Vector3(0f, 30f, 0f), 0);
			}
		}
	}

	public static void SnowballMinigun()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab || Mouse.current.leftButton.isPressed)
		{
			Vector3 val = Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * Main.ShootStrength;
			if (Mouse.current.leftButton.isPressed)
			{
				Ray val2 = Main.TPC.ScreenPointToRay(Vector2.op_Implicit(((InputControl<Vector2>)(object)((Pointer)Mouse.current).position).ReadValue()));
				RaycastHit val3 = default(RaycastHit);
				Physics.Raycast(val2, ref val3, 512f, Main.NoInvisLayerMask());
				val = ((RaycastHit)(ref val3)).point - ((Component)GorillaTagger.Instance.rightHandTransform).transform.position;
				((Vector3)(ref val)).Normalize();
				val *= Main.ShootStrength * 2f;
			}
			BetaSpawnSnowball(GorillaTagger.Instance.rightHandTransform.position, val, 0);
		}
	}

	public static void SnowballShotgun()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab || Mouse.current.leftButton.isPressed)
		{
			Vector3 val = Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * Main.ShootStrength;
			if (Mouse.current.leftButton.isPressed)
			{
				Ray val2 = Main.TPC.ScreenPointToRay(Vector2.op_Implicit(((InputControl<Vector2>)(object)((Pointer)Mouse.current).position).ReadValue()));
				RaycastHit val3 = default(RaycastHit);
				Physics.Raycast(val2, ref val3, 512f, Main.NoInvisLayerMask());
				val = ((RaycastHit)(ref val3)).point - ((Component)GorillaTagger.Instance.rightHandTransform).transform.position;
				((Vector3)(ref val)).Normalize();
				val *= Main.ShootStrength * 2f;
			}
			for (int i = 0; i < 5; i++)
			{
				BetaSpawnSnowball(GorillaTagger.Instance.rightHandTransform.position, val + RandomUtilities.RandomVector3(5f), 0);
			}
		}
	}

	public static void SnowballWall()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.rightGrab && !Mouse.current.leftButton.isPressed)
		{
			return;
		}
		Vector3 val = Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * Main.ShootStrength;
		if (Mouse.current.leftButton.isPressed)
		{
			Ray val2 = Main.TPC.ScreenPointToRay(Vector2.op_Implicit(((InputControl<Vector2>)(object)((Pointer)Mouse.current).position).ReadValue()));
			RaycastHit val3 = default(RaycastHit);
			Physics.Raycast(val2, ref val3, 512f, Main.NoInvisLayerMask());
			val = ((RaycastHit)(ref val3)).point - ((Component)GorillaTagger.Instance.rightHandTransform).transform.position;
			((Vector3)(ref val)).Normalize();
			val *= Main.ShootStrength * 2f;
		}
		if (!(Time.time > wallDelay))
		{
			return;
		}
		for (int i = -2; i <= 2; i++)
		{
			for (int j = -2; j <= 2; j++)
			{
				(Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) trueRightHand = ControllerUtilities.GetTrueRightHand();
				Vector3 item = trueRightHand.up;
				Vector3 item2 = trueRightHand.right;
				BetaSpawnSnowball(GorillaTagger.Instance.rightHandTransform.position + item2 * ((float)i * 0.666f) + item * ((float)j * 0.666f), val, 0);
			}
		}
		wallDelay = Time.time + 0.1f;
	}

	public static void SnowballBomb()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab)
		{
			if ((Object)(object)BombObject == (Object)null)
			{
				BombObject = GameObject.CreatePrimitive((PrimitiveType)0);
				Object.Destroy((Object)(object)BombObject.GetComponent<SphereCollider>());
				BombObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
			}
			BombObject.transform.position = GorillaTagger.Instance.rightHandTransform.position;
		}
		if (!((Object)(object)BombObject != (Object)null))
		{
			return;
		}
		if (Main.rightPrimary)
		{
			for (int i = 0; i < 10; i++)
			{
				BetaSpawnSnowball(BombObject.transform.position, RandomUtilities.RandomVector3(500f), 0);
			}
			Object.Destroy((Object)(object)BombObject);
			BombObject = null;
		}
		else
		{
			BombObject.GetComponent<Renderer>().material.color = Main.buttonColors[0].GetColor(0);
		}
	}

	public static void SnowballGrenade()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab && !rpgShot)
		{
			rpgShot = true;
			SnowballThrowable projectile = Main.GetProjectile("SnowballRightAnchor");
			projectile.SetSnowballActiveLocal(true);
			Color val = default(Color);
			((Color)(ref val))._002Ector(0f, 0.6f, 0f, 1f);
			VRRig.LocalRig.SetThrowableProjectileColor(true, Color32.op_Implicit(val));
			projectile.randomizeColor = true;
			projectile.ApplyColor(val);
		}
	}

	public static void SnowballRPG()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab && Time.time > rpgDelay)
		{
			rpgDelay = Time.time + 1f;
			rpgShot = true;
			BetaSpawnSnowball(GorillaTagger.Instance.rightHandTransform.position, Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * Main.ShootStrength, 0);
		}
	}

	public static void OnSnowballHit(SlingshotProjectile slingshotProjectile, Collision _)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (rpgShot && slingshotProjectile.projectileOwner == NetworkSystem.Instance.LocalPlayer)
		{
			rpgShot = false;
			for (int i = 0; i < 10; i++)
			{
				BetaSpawnSnowball(((Component)slingshotProjectile).transform.position + Vector3.up * 0.3f, RandomUtilities.RandomVector3(500f), 0);
			}
		}
	}

	public static void DisableBomb()
	{
		if ((Object)(object)BombObject != (Object)null)
		{
			Object.Destroy((Object)(object)BombObject);
			BombObject = null;
		}
	}

	public static void GiveSnowballMinigun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Vector3 vel = ((Component)Main.lockTarget.rightHandTransform).transform.forward * Main.ShootStrength;
				BetaSpawnSnowball(((Component)Main.lockTarget.rightHandTransform).transform.position, vel, 0);
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
			Main.gunLocked = false;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void SnowballParticleGun()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true))
			{
				BetaSpawnSnowball(item.transform.position + new Vector3(0f, 0.1f, 0f), new Vector3(0f, 0f, 0f), 0);
			}
		}
	}

	public static void SnowballImpactEffectGun()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true))
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				BetaSnowballImpact(RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(componentInParent)));
			}
		}
	}

	public static void SnowballPunchMod()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.isLocal && (Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, activeRig.headMesh.transform.position) < 0.25f || Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, activeRig.headMesh.transform.position) < 0.25f))
			{
				Vector3 val = ((Component)GorillaTagger.Instance.headCollider).transform.position - activeRig.headMesh.transform.position;
				Vector3 val2 = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(0f, 0.5f, 0f);
				Vector3 val3 = new Vector3(val.x, 0f, val.z);
				BetaSpawnSnowball(val2 + ((Vector3)(ref val3)).normalized / 1.7f, new Vector3(0f, -500f, 0f), 2, RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(activeRig)));
				if (Buttons.GetIndex("Graphic Punch Mod").enabled)
				{
					Projectiles.BetaFireProjectile("AppleLeftAnchor", activeRig.headMesh.transform.position, Vector3.down * 600f, Color32.op_Implicit(new Color32((byte)100, (byte)0, (byte)0, byte.MaxValue)));
				}
			}
		}
	}

	public static void SnowballBoxing()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (Time.time < GetBoxingDelay(activeRig))
			{
				continue;
			}
			foreach (VRRig activeRig2 in VRRigCache.ActiveRigs)
			{
				if (!((Object)(object)activeRig2 == (Object)(object)activeRig) && (Vector3.Distance(activeRig2.leftHandTransform.position, activeRig.headMesh.transform.position) < 0.25f || Vector3.Distance(activeRig2.rightHandTransform.position, activeRig.headMesh.transform.position) < 0.25f))
				{
					Vector3 val = activeRig2.headMesh.transform.position - activeRig.headMesh.transform.position;
					Vector3 val2 = activeRig.headMesh.transform.position + new Vector3(0f, 0.5f, 0f);
					Vector3 val3 = new Vector3(val.x, 0f, val.z);
					BetaSpawnSnowball(val2 + ((Vector3)(ref val3)).normalized / 1.7f, new Vector3(0f, -500f, 0f), 2, activeRig.GetPhotonPlayer());
					SetBoxingDelay(activeRig);
				}
			}
		}
	}

	public static void SnowballDash()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (Time.time < GetBoxingDelay(activeRig))
			{
				break;
			}
			if (!activeRig.isOfflineVRRig && ((VRMap)activeRig.rightThumb).calcT > 0.5f)
			{
				BetaSpawnSnowball(activeRig.headMesh.transform.position + new Vector3(0f, 0.5f, 0f) + new Vector3(0f - activeRig.headMesh.transform.forward.x, 0f, 0f - activeRig.headMesh.transform.forward.z) * 1.5f, new Vector3(0f, -300f, 0f), 2, activeRig.GetPhotonPlayer());
				SetBoxingDelay(activeRig);
			}
		}
	}

	public static void SnowballHighJump()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		RaycastHit val = default(RaycastHit);
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (Time.time < GetBoxingDelay(activeRig))
			{
				break;
			}
			Physics.Raycast(activeRig.bodyTransform.position - new Vector3(0f, 0.2f, 0f), Vector3.down, ref val, 512f, LayerMask.op_Implicit(GTPlayer.Instance.locomotionEnabledLayers));
			if (!activeRig.isOfflineVRRig && ((RaycastHit)(ref val)).distance > 0.12f && ((RaycastHit)(ref val)).distance < 0.2f)
			{
				BetaSpawnSnowball(activeRig.headMesh.transform.position + new Vector3(0f, -0.7f, 0f), new Vector3(0f, -500f, 0f), 2, activeRig.GetPhotonPlayer());
				SetBoxingDelay(activeRig);
			}
		}
	}

	public static void Enable_Kamehameha()
	{
		AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Mods/Overpowered/Kamehameha/start.ogg", "Audio/Mods/Overpowered/Kamehameha/start.ogg", delegate(AudioClip clip)
		{
			KameStart = clip;
		});
		AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Mods/Overpowered/Kamehameha/end.ogg", "Audio/Mods/Overpowered/Kamehameha/end.ogg", delegate(AudioClip clip)
		{
			KameStop = clip;
		});
	}

	public static void Kamehameha()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (Main.leftGrab && Main.rightGrab && Main.leftTrigger > 0.5f && Main.rightTrigger > 0.5f)
		{
			if (KameStartCoroutine == null)
			{
				KameStartCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(StartKame());
			}
		}
		else if (KameStartCoroutine != null)
		{
			((MonoBehaviour)CoroutineManager.instance).StopCoroutine(KameStartCoroutine);
			KameStartCoroutine = null;
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(EndKame());
		}
	}

	public static IEnumerator StartKame()
	{
		PlayKameSound(KameStart);
		yield return (object)new WaitForSeconds(0.5f);
		SnowballThrowable projectile = Main.GetProjectile(Projectiles.SnowballName + "LeftAnchor");
		GrowingSnowballThrowable leftSnowball = (GrowingSnowballThrowable)(object)((projectile is GrowingSnowballThrowable) ? projectile : null);
		SnowballThrowable projectile2 = Main.GetProjectile(Projectiles.SnowballName + "RightAnchor");
		GrowingSnowballThrowable rightSnowball = (GrowingSnowballThrowable)(object)((projectile2 is GrowingSnowballThrowable) ? projectile2 : null);
		GrowingSnowballThrowable[] snowballs = (GrowingSnowballThrowable[])(object)new GrowingSnowballThrowable[2] { leftSnowball, rightSnowball };
		GrowingSnowballThrowable[] array = snowballs;
		foreach (GrowingSnowballThrowable snowball in array)
		{
			((SnowballThrowable)snowball).SetSnowballActiveLocal(true);
		}
		float startTime = Time.time;
		RaycastHit RayPoint = default(RaycastHit);
		while (true)
		{
			try
			{
				if ((Object)(object)cursor == (Object)null)
				{
					cursor = GameObject.CreatePrimitive((PrimitiveType)0);
					cursor.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
					cursor.GetComponent<Renderer>().material.color = Color.white;
					Object.Destroy((Object)(object)cursor.GetComponent<Collider>());
				}
				Physics.Raycast(((Component)GorillaTagger.Instance.headCollider).transform.position, ((Component)GorillaTagger.Instance.headCollider).transform.forward, ref RayPoint, 512f, LayerMask.op_Implicit(GTPlayer.Instance.locomotionEnabledLayers));
				cursor.transform.position = ((((RaycastHit)(ref RayPoint)).point == Vector3.zero) ? (((RaycastHit)(ref RayPoint)).transform.position + ((RaycastHit)(ref RayPoint)).transform.forward * 20f) : ((RaycastHit)(ref RayPoint)).point);
			}
			catch
			{
			}
			try
			{
				int sizeIndex = Mathf.Clamp((int)Mathf.Floor((Time.time - startTime) * 1.75f), 0, 5);
				GrowingSnowballThrowable[] array2 = snowballs;
				foreach (GrowingSnowballThrowable snowball2 in array2)
				{
					if (snowball2.sizeLevel != sizeIndex)
					{
						snowball2.SetSizeLevelAuthority(sizeIndex);
					}
				}
				VRRig.LocalRig.SetThrowableProjectileColor(true, Color32.op_Implicit(Color.white));
				((SnowballThrowable)leftSnowball).randomizeColor = true;
				((SnowballThrowable)leftSnowball).ApplyColor(Color.white);
				VRRig.LocalRig.SetThrowableProjectileColor(false, Color32.op_Implicit(Color.cyan));
				((SnowballThrowable)rightSnowball).randomizeColor = true;
				((SnowballThrowable)rightSnowball).ApplyColor(Color.cyan);
			}
			catch
			{
			}
			Vector3 snowballPosition = GorillaTagger.Instance.leftHandTransform.position.Lerp(GorillaTagger.Instance.rightHandTransform.position, 0.5f);
			Transform[] array3 = (Transform[])(object)new Transform[2]
			{
				GorillaTagger.Instance.leftHandTransform,
				GorillaTagger.Instance.rightHandTransform
			};
			foreach (Transform handTransform in array3)
			{
				handTransform.position = snowballPosition;
				handTransform.rotation = RandomUtilities.RandomQuaternion();
			}
			yield return null;
		}
	}

	public static IEnumerator EndKame()
	{
		PlayKameSound(KameStop);
		yield return (object)new WaitForSeconds(0.5f);
		float startTime = Time.time;
		RaycastHit RayPoint = default(RaycastHit);
		Vector3 val;
		while (Time.time - startTime < 3f)
		{
			if ((Object)(object)cursor == (Object)null)
			{
				cursor = GameObject.CreatePrimitive((PrimitiveType)0);
				cursor.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
				cursor.GetComponent<Renderer>().material.color = Color.white;
				Object.Destroy((Object)(object)cursor.GetComponent<Collider>());
			}
			try
			{
				Physics.Raycast(((Component)GorillaTagger.Instance.headCollider).transform.position, ((Component)GorillaTagger.Instance.headCollider).transform.forward, ref RayPoint, 512f, LayerMask.op_Implicit(GTPlayer.Instance.locomotionEnabledLayers));
				cursor.transform.position = ((((RaycastHit)(ref RayPoint)).point == Vector3.zero) ? (((RaycastHit)(ref RayPoint)).transform.position + ((RaycastHit)(ref RayPoint)).transform.forward * 20f) : ((RaycastHit)(ref RayPoint)).point);
			}
			catch
			{
			}
			Vector3 snowballPosition = GorillaTagger.Instance.leftHandTransform.position.Lerp(GorillaTagger.Instance.rightHandTransform.position, 0.5f);
			val = cursor.transform.position - snowballPosition;
			Vector3 targetDirection = ((Vector3)(ref val)).normalized * 60f;
			BetaSpawnSnowball(snowballPosition, targetDirection, 0);
			yield return (object)new WaitForSeconds(SnowballSpawnDelay);
		}
		Vector3 _ = GorillaTagger.Instance.leftHandTransform.position.Lerp(GorillaTagger.Instance.rightHandTransform.position, 0.5f);
		val = cursor.transform.position - _;
		Vector3 __ = ((Vector3)(ref val)).normalized * 30f;
		for (int i = 5; i >= 0; i--)
		{
			BetaSpawnSnowball(_, __, 0, null, i);
			yield return (object)new WaitForSeconds(0.1f);
		}
		if ((Object)(object)cursor != (Object)null)
		{
			Object.Destroy((Object)(object)cursor);
		}
	}

	public static void PlayKameSound(AudioClip clip)
	{
		if (RecorderPatch.enabled)
		{
			if (KameSound != null)
			{
				VoiceManager.Get().StopAudioClip(KameSound);
				KameSound = null;
			}
			KameSound = VoiceManager.Get().AudioClip(clip);
		}
		else
		{
			Sound.PlayAudio(clip);
		}
	}

	public static void Disable_Kamehameha()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		VRRig.LocalRig.SetThrowableProjectileColor(false, Color32.op_Implicit(Color.white));
		VRRig.LocalRig.SetThrowableProjectileColor(false, Color32.op_Implicit(Color.white));
	}

	public static void SnowballSafetyBubble()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.isLocal && Vector3.Distance(((Component)GorillaTagger.Instance.bodyCollider).transform.position, ((Component)activeRig).transform.position) < 3f)
			{
				Vector3 val = ((Component)GorillaTagger.Instance.headCollider).transform.position - activeRig.headMesh.transform.position;
				Vector3 val2 = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(0f, 0.5f, 0f);
				Vector3 val3 = new Vector3(val.x, 0f, val.z);
				BetaSpawnSnowball(val2 + ((Vector3)(ref val3)).normalized / 1.7f, new Vector3(0f, -500f, 0f), 2, RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(activeRig)));
				if (PhotonNetwork.InRoom)
				{
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", (RpcTarget)0, new object[3] { 248, false, 999999f });
				}
				else
				{
					VRRig.LocalRig.PlayHandTapLocal(248, false, 999999f);
				}
			}
		}
	}

	public static void FlingPlayer(NetPlayer player)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)RigUtilities.GetVRRigFromPlayer(player)).transform.position + new Vector3(0f, 0.5f, 0f);
		Vector3 val2 = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
		BetaSpawnSnowball(val + ((Vector3)(ref val2)).normalized / 1.7f, new Vector3(0f, -500f, 0f), 2, RigUtilities.NetPlayerToPlayer(player));
	}

	public static void FlingPlayer(VRRig player)
	{
		FlingPlayer(RigUtilities.GetPlayerFromVRRig(player));
	}

	public static void SnowballFlingGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				FlingPlayer(Main.lockTarget);
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
			Main.gunLocked = false;
		}
	}

	public static void SnowballLaunchGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				int num = 10;
				for (int i = 0; i < num; i++)
				{
					float num2 = ((i == 0) ? 0f : (360f / (float)num * (float)i));
					Vector3 val2 = ((Component)Main.lockTarget).transform.position + new Vector3(0f, 0.5f, 0f);
					Vector3 val3 = Quaternion.Euler(0f, num2, 0f) * Vector3.forward;
					BetaSpawnSnowball(val2 + ((Vector3)(ref val3)).normalized / 1.7f, new Vector3(0f, -500f, 0f), 2, Main.lockTarget.GetPhotonPlayer());
				}
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
			Main.gunLocked = false;
		}
	}

	public static void SnowballFlingZone()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab)
		{
			bool flag = false;
			using (IEnumerator<GameObject> enumerator = flingZones.Where((GameObject checkpoint) => Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, checkpoint.transform.position) < 0.5f).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					GameObject current = enumerator.Current;
					flag = true;
					current.transform.position = ((Component)GorillaTagger.Instance.rightHandTransform).transform.position;
				}
			}
			if (!flag)
			{
				GameObject val = GameObject.CreatePrimitive((PrimitiveType)0);
				Object.Destroy((Object)(object)val.GetComponent<SphereCollider>());
				val.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
				val.transform.position = GorillaTagger.Instance.rightHandTransform.position;
				val.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
				val.GetComponent<Renderer>().material.color = new Color(1f, 0f, 0f, 0.3f);
				flingZones.Add(val);
			}
		}
		if (Main.rightTrigger > 0.5f)
		{
			foreach (GameObject item in from checkpoint in flingZones.ToList()
				where Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, checkpoint.transform.position) < 0.5f
				select checkpoint)
			{
				flingZones.Remove(item);
				Object.Destroy((Object)(object)item);
			}
		}
		foreach (VRRig item2 in VRRigCache.ActiveRigs.Where((VRRig rig) => !rig.IsLocal()))
		{
			foreach (GameObject flingZone in flingZones)
			{
				if (Vector3.Distance(((Component)item2).transform.position, flingZone.transform.position) < 0.5f || Vector3.Distance(item2.leftHandTransform.position, flingZone.transform.position) < 0.5f || Vector3.Distance(item2.rightHandTransform.position, flingZone.transform.position) < 0.5f)
				{
					BetaSpawnSnowball(flingZone.transform.position, new Vector3(0f, -500f, 0f), 2, item2.GetPhotonPlayer());
				}
			}
		}
	}

	public static void DisableSnowballFlingZone()
	{
		foreach (GameObject flingZone in flingZones)
		{
			Object.Destroy((Object)(object)flingZone);
		}
		flingZones.Clear();
	}

	public static void SnowballFlingAll()
	{
		if (Main.rightTrigger > 0.5f)
		{
			FlingPlayer(RigUtilities.GetTargetPlayer(SnowballSpawnDelay));
		}
	}

	public static void SnowballFlingVerticalGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				BetaSpawnSnowball(Main.lockTarget.headMesh.transform.position + new Vector3(0f, -0.7f, 0f), new Vector3(0f, -500f, 0f), 2, RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(Main.lockTarget)));
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
			Main.gunLocked = false;
		}
	}

	public static void SnowballFlingVerticalAll()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightTrigger > 0.5f)
		{
			Player val = RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(RigUtilities.GetTargetPlayer(SnowballSpawnDelay)));
			BetaSpawnSnowball(((Component)RigUtilities.GetVRRigFromPlayer(NetPlayer.op_Implicit(val))).transform.position + new Vector3(0f, -0.7f, 0f), new Vector3(0f, -500f, 0f), 2, val);
		}
	}

	public static void SnowballFlingTowardsGun()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true))
			{
				Player val = RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(RigUtilities.GetTargetPlayer(0.5f)));
				Vector3 val2 = item.transform.position - RigUtilities.GetVRRigFromPlayer(NetPlayer.op_Implicit(val)).headMesh.transform.position;
				Vector3 normalized = ((Vector3)(ref val2)).normalized;
				BetaSpawnSnowball(((Component)RigUtilities.GetVRRigFromPlayer(NetPlayer.op_Implicit(val))).transform.position + new Vector3(0f, 0.5f, 0f) + new Vector3(0f - normalized.x, 0f, 0f - normalized.z) / 1.7f, new Vector3(0f, -500f, 0f), 2, val);
			}
		}
	}

	public static void SnowballFlingAwayGun()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true))
			{
				BetaSpawnSnowball(item.transform.position + new Vector3(0f, 0.1f, 0f), new Vector3(0f, -500f, 0f), 1);
			}
		}
	}

	public static void SnowballFlingPlayerTowardsGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Vector3 val2 = Main.lockTarget.headMesh.transform.position - ((Component)GorillaTagger.Instance.headCollider).transform.position;
				Vector3 normalized = ((Vector3)(ref val2)).normalized;
				BetaSpawnSnowball(Main.lockTarget.headMesh.transform.position + new Vector3(0f, 0.5f, 0f) + new Vector3(normalized.x, 0f, normalized.z) * 1.5f, new Vector3(0f, -100f, 0f), 2, RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(Main.lockTarget)));
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
			Main.gunLocked = false;
		}
	}

	public static void SnowballFlingPlayerAwayGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Vector3 val2 = ((Component)GorillaTagger.Instance.headCollider).transform.position - Main.lockTarget.headMesh.transform.position;
				Vector3 normalized = ((Vector3)(ref val2)).normalized;
				BetaSpawnSnowball(Main.lockTarget.headMesh.transform.position + new Vector3(0f, 0.5f, 0f) + new Vector3(normalized.x, 0f, normalized.z) * 1.5f, new Vector3(0f, -100f, 0f), 2, RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(Main.lockTarget)));
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
			Main.gunLocked = false;
		}
	}

	public static void SnowballPushGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Vector3 val2 = ((Component)GorillaTagger.Instance.headCollider).transform.position - Main.lockTarget.headMesh.transform.position;
				Vector3 val3 = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(0f, 0.5f, 0f);
				Vector3 val4 = new Vector3(val2.x, 0f, val2.z);
				BetaSpawnSnowball(val3 + ((Vector3)(ref val4)).normalized / 1.7f, new Vector3(0f, -500f, 0f), 2, RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(Main.lockTarget)));
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
			Main.gunLocked = false;
		}
	}

	public static void SnowballStrongFlingGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				BetaSpawnSnowball(new Vector3(((Component)GorillaTagger.Instance.headCollider).transform.position.x, 1000f, ((Component)GorillaTagger.Instance.headCollider).transform.position.z), new Vector3(0f, -9999f, 0f), 2, RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(Main.lockTarget)));
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
			Main.gunLocked = false;
		}
	}

	public static void AntiReportFling()
	{
		if (Time.time > antiReportFlingDelay)
		{
			Safety.AntiReport(delegate(VRRig vrrig, Vector3 position)
			{
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				antiReportFlingDelay = Time.time + 0.1f;
				BetaSetVelocityPlayer(RigUtilities.GetPlayerFromVRRig(vrrig), (((Component)vrrig).transform.position - position) * 50f);
				NotificationManager.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + RigUtilities.GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, they have been flung.");
			});
		}
	}

	public static void AntiReportSnowballFling()
	{
		Safety.AntiReport(delegate(VRRig vrrig, Vector3 position)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			BetaSpawnSnowball(position, new Vector3(0f, -500f, 0f), 2, RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(vrrig)));
			NotificationManager.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + RigUtilities.GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, they have been flung.");
		});
	}

	public static bool SpecialTimeRPC(PhotonView photonView, int timeOffset, string method, RaiseEventOptions options, params object[] parameters)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Invalid comparison between Unknown and I4
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Invalid comparison between Unknown and I4
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)photonView != (Object)null && parameters != null && !string.IsNullOrEmpty(method))
		{
			Hashtable val = new Hashtable();
			val.Add((byte)0, (object)photonView.ViewID);
			val.Add((byte)2, (object)(PhotonNetwork.ServerTimestamp + timeOffset));
			val.Add((byte)3, (object)method);
			val.Add((byte)4, (object)parameters);
			Hashtable val2 = val;
			if (photonView.Prefix > 0)
			{
				val2[(byte)1] = (short)photonView.Prefix;
			}
			if (PhotonNetwork.PhotonServerSettings.RpcList.Contains(method))
			{
				val2[(byte)5] = (byte)PhotonNetwork.PhotonServerSettings.RpcList.IndexOf(method);
			}
			if ((int)options.Receivers == 1 || (options.TargetActors != null && Extensions.Contains(options.TargetActors, NetworkSystem.Instance.LocalPlayer.ActorNumber)))
			{
				if ((int)options.Receivers == 1)
				{
					options.Receivers = (ReceiverGroup)0;
				}
				if (options.TargetActors != null && Extensions.Contains(options.TargetActors, NetworkSystem.Instance.LocalPlayer.ActorNumber))
				{
					options.TargetActors = options.TargetActors.Where((int id) => id != NetworkSystem.Instance.LocalPlayer.ActorNumber).ToArray();
				}
				PhotonNetwork.ExecuteRpc(val2, PhotonNetwork.LocalPlayer);
			}
			else
			{
				LoadBalancingPeer loadBalancingPeer = PhotonNetwork.NetworkingClient.LoadBalancingPeer;
				SendOptions val3 = default(SendOptions);
				((SendOptions)(ref val3)).Reliability = true;
				val3.DeliveryMode = (DeliveryMode)3;
				val3.Encrypt = false;
				loadBalancingPeer.OpRaiseEvent((byte)200, (object)val2, options, val3);
			}
		}
		return false;
	}

	public static void GuardianPhysicalFreezeGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && Time.time > flingDelay)
			{
				BetaSetVelocityPlayer(RigUtilities.GetPlayerFromVRRig(Main.lockTarget), Vector3.zero);
				Main.RPCProtection();
				flingDelay = Time.time + 0.1f;
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
			Main.gunLocked = false;
		}
	}

	public static void GuardianPhysicalFreezeAll()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightTrigger > 0.5f && Time.time > flingDelay)
		{
			flingDelay = Time.time + 0.1f;
			BetaSetVelocityTargetGroup((RpcTarget)1, Vector3.zero);
			Main.RPCProtection();
		}
	}

	public static void GuardianBringPlayer(NetPlayer player)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > flingDelay)
		{
			Vector3 val = ((Component)GorillaTagger.Instance.bodyCollider).transform.position - ((Component)RigUtilities.GetVRRigFromPlayer(player)).transform.position;
			BetaSetVelocityPlayer(player, ((Vector3)(ref val)).normalized * 20f);
			Main.RPCProtection();
			flingDelay = Time.time + 0.1f;
		}
	}

	public static void GuardianBringPlayerGun(NetPlayer player)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true) && Time.time > flingDelay)
			{
				BetaSetVelocityPlayer(player, Vector3.Normalize(item.transform.position - ((Component)RigUtilities.GetVRRigFromPlayer(player)).transform.position) * 50f);
				Main.RPCProtection();
				flingDelay = Time.time + 0.2f;
			}
		}
	}

	public static void GuardianBringGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && Time.time > flingDelay)
			{
				NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(Main.lockTarget);
				Vector3 val2 = ((Component)GorillaTagger.Instance.bodyCollider).transform.position - ((Component)Main.lockTarget).transform.position;
				BetaSetVelocityPlayer(playerFromVRRig, ((Vector3)(ref val2)).normalized * 20f);
				Main.RPCProtection();
				flingDelay = Time.time + 0.1f;
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
			Main.gunLocked = false;
		}
	}

	public static void GuardianBringAll()
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		if (!(Main.rightTrigger > 0.5f) || !(Time.time > flingDelay))
		{
			return;
		}
		flingDelay = Time.time + 0.2f;
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig plr) => !plr.isLocal))
		{
			NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(item);
			Vector3 val = ((Component)GorillaTagger.Instance.bodyCollider).transform.position - ((Component)item).transform.position;
			BetaSetVelocityPlayer(playerFromVRRig, ((Vector3)(ref val)).normalized * 20f);
			Main.RPCProtection();
		}
	}

	public static void GuardianBringAwayGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && Time.time > flingDelay)
			{
				NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(Main.lockTarget);
				Vector3 val2 = ((Component)Main.lockTarget).transform.position - ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
				BetaSetVelocityPlayer(playerFromVRRig, ((Vector3)(ref val2)).normalized * 20f);
				Main.RPCProtection();
				flingDelay = Time.time + 0.1f;
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
			Main.gunLocked = false;
		}
	}

	public static void GuardianBringAwayAll()
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		if (!(Main.rightTrigger > 0.5f) || !(Time.time > flingDelay))
		{
			return;
		}
		flingDelay = Time.time + 0.2f;
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig plr) => !plr.isLocal))
		{
			NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(item);
			Vector3 val = ((Component)item).transform.position - ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
			BetaSetVelocityPlayer(playerFromVRRig, ((Vector3)(ref val)).normalized * 20f);
			Main.RPCProtection();
		}
	}

	public static void GuardianOrbitAll()
	{
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		float num = 5f;
		if (Main.rightTrigger > 0.5f && Time.time > flingDelay)
		{
			flingDelay = Time.time + 0.2f;
			int num2 = 0;
			VRRig[] array = VRRigCache.ActiveRigs.Where((VRRig rig) => !rig.isLocal).ToArray();
			VRRig[] array2 = array;
			foreach (VRRig val in array2)
			{
				float num4 = 360f / (float)array.Length * (float)num2;
				Vector3 val2 = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(MathF.Cos(num4 + Time.time) * num, 2f, MathF.Sin(num4 + Time.time) * num);
				BetaSetVelocityPlayer(RigUtilities.GetPlayerFromVRRig(val), (val2 - ((Component)val).transform.position) * 1f);
				Main.RPCProtection();
				num2++;
			}
		}
	}

	public static void GuardianGiveFlyGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && Time.time > thingdeb)
			{
				if (((VRMap)Main.lockTarget.rightThumb).calcT > 0.5f)
				{
					BetaSetVelocityPlayer(RigUtilities.GetPlayerFromVRRig(Main.lockTarget), Main.lockTarget.headMesh.transform.forward * Movement._flySpeed);
					Main.RPCProtection();
				}
				thingdeb = Time.time + 0.1f;
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
			Main.gunLocked = false;
		}
	}

	public static void GuardianGiveFlyAll()
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		if (!(Time.time > thingdeb))
		{
			return;
		}
		thingdeb = Time.time + 0.1f;
		foreach (VRRig item in from plr in VRRigCache.ActiveRigs
			where !plr.isLocal
			where ((VRMap)plr.rightThumb).calcT > 0.5f
			select plr)
		{
			BetaSetVelocityPlayer(RigUtilities.GetPlayerFromVRRig(item), item.headMesh.transform.forward * Movement._flySpeed);
			Main.RPCProtection();
		}
	}

	public static void GuardianPunchMod()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
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
				Vector3 velocity = (flag2 ? GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0f, false) : GTPlayer.Instance.LeftHand.velocityTracker.GetAverageVelocity(true, 0f, false));
				BetaSetVelocityPlayer(RigUtilities.GetPlayerFromVRRig(activeRig), velocity);
				thingdeb = Time.time + 0.1f;
				if (Buttons.GetIndex("Graphic Punch Mod").enabled)
				{
					Projectiles.BetaFireProjectile("AppleLeftAnchor", activeRig.headMesh.transform.position, Vector3.down * 600f, Color32.op_Implicit(new Color32((byte)100, (byte)0, (byte)0, byte.MaxValue)));
				}
			}
		}
	}

	private static float GetBoxingDelay(VRRig rig)
	{
		return boxingDelay.GetValueOrDefault(rig, -1f);
	}

	private static void SetBoxingDelay(VRRig rig)
	{
		boxingDelay.Remove(rig);
		boxingDelay.Add(rig, SnowballSpawnDelay);
	}

	public static void GuardianBoxing()
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		foreach (VRRig rig1 in VRRigCache.ActiveRigs)
		{
			if (Time.time < GetBoxingDelay(rig1))
			{
				continue;
			}
			foreach (Vector3 item in from val in VRRigCache.ActiveRigs
				where (Object)(object)val != (Object)(object)rig1
				where Vector3.Distance(val.leftHandTransform.position, rig1.headMesh.transform.position) < 0.25f || Vector3.Distance(val.rightHandTransform.position, rig1.headMesh.transform.position) < 0.25f
				select (rig1.headMesh.transform.position - val.headMesh.transform.position) * 20f)
			{
				BetaSetVelocityPlayer(RigUtilities.GetPlayerFromVRRig(rig1), item);
				SetBoxingDelay(rig1);
			}
		}
	}

	public static void GuardianBringAllGun()
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		GameObject item = Main.RenderGun().NewPointer;
		if (!Main.GetGunInput(isShooting: true) || !(Time.time > flingDelay))
		{
			return;
		}
		foreach (VRRig item2 in VRRigCache.ActiveRigs.Where((VRRig plr) => !plr.isLocal))
		{
			BetaSetVelocityPlayer(RigUtilities.GetPlayerFromVRRig(item2), Vector3.Normalize(item.transform.position - ((Component)item2).transform.position) * 50f);
		}
		Main.RPCProtection();
		flingDelay = Time.time + 0.2f;
	}

	public static void GuardianBringAwayAllGun()
	{
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		GameObject item = Main.RenderGun().NewPointer;
		if (!Main.GetGunInput(isShooting: true) || !(Time.time > flingDelay))
		{
			return;
		}
		foreach (VRRig item2 in VRRigCache.ActiveRigs.Where((VRRig plr) => !plr.isLocal))
		{
			BetaSetVelocityPlayer(RigUtilities.GetPlayerFromVRRig(item2), Vector3.Normalize(((Component)item2).transform.position - item.transform.position) * 50f);
		}
		Main.RPCProtection();
		flingDelay = Time.time + 0.2f;
	}

	public static void GuardianAntiStump()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (!(Time.time > flingDelay))
		{
			return;
		}
		Vector3 val = default(Vector3);
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.isLocal)
			{
				((Vector3)(ref val))._002Ector(-66f, 12f, -79f);
				if (Vector3.Distance(val, ((Component)activeRig).transform.position) < 3f)
				{
					NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(activeRig);
					Vector3 val2 = ((Component)activeRig).transform.position - val;
					BetaSetVelocityPlayer(playerFromVRRig, ((Vector3)(ref val2)).normalized * 20f);
					flingDelay = Time.time + 0.2f;
				}
			}
		}
	}

	public static void GuardianEffectSpamHands()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab && Time.time > slamDel)
		{
			GorillaGuardianManager val = (GorillaGuardianManager)GorillaGameManager.instance;
			if (val.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
			{
				((GorillaWrappedSerializer)GameMode.ActiveNetworkHandler).NetView.GetView.RPC(flip ? "ShowSlamEffect" : "ShowSlapEffects", (RpcTarget)0, new object[2]
				{
					GorillaTagger.Instance.rightHandTransform.position,
					(object)new Vector3((float)Random.Range(0, 360), (float)Random.Range(0, 360), (float)Random.Range(0, 360))
				});
				Main.RPCProtection();
				flip = !flip;
			}
			else
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must be guardian.");
			}
			slamDel = Time.time + 0.05f;
		}
		if (Main.leftGrab && Time.time > slamDel)
		{
			GorillaGuardianManager val2 = (GorillaGuardianManager)GorillaGameManager.instance;
			if (val2.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
			{
				((GorillaWrappedSerializer)GameMode.ActiveNetworkHandler).NetView.GetView.RPC(flip ? "ShowSlamEffect" : "ShowSlapEffects", (RpcTarget)0, new object[2]
				{
					GorillaTagger.Instance.leftHandTransform.position,
					(object)new Vector3((float)Random.Range(0, 360), (float)Random.Range(0, 360), (float)Random.Range(0, 360))
				});
				Main.RPCProtection();
				flip = !flip;
			}
			else
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must be guardian.");
			}
			slamDel = Time.time + 0.05f;
		}
	}

	public static void GuardianEffectSpamGun()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		GameObject item = Main.RenderGun().NewPointer;
		if (!Main.GetGunInput(isShooting: true))
		{
			return;
		}
		GorillaGuardianManager val = (GorillaGuardianManager)GorillaGameManager.instance;
		if (Time.time > slamDel)
		{
			if (val.IsPlayerGuardian(NetworkSystem.Instance.LocalPlayer))
			{
				((GorillaWrappedSerializer)GameMode.ActiveNetworkHandler).NetView.GetView.RPC(flip ? "ShowSlamEffect" : "ShowSlapEffects", (RpcTarget)0, new object[2]
				{
					item.transform.position,
					(object)new Vector3((float)Random.Range(0, 360), (float)Random.Range(0, 360), (float)Random.Range(0, 360))
				});
				Main.RPCProtection();
				flip = !flip;
			}
			else
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must be guardian.");
			}
			slamDel = Time.time + 0.05f;
		}
	}

	public static void FreezeServer(float delay = 1f, int eventCount = 11, RaiseEventOptions options = null)
	{
	}

	public static void CloseRoom()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (PhotonNetwork.InRoom && !(Time.time < closeRoomDelay))
		{
			closeRoomDelay = Time.time + 0.1f;
			for (int i = 0; i < 40; i++)
			{
				WebFlags flags = new WebFlags(byte.MaxValue);
				RaiseEventOptions val = new RaiseEventOptions
				{
					Flags = flags,
					Receivers = (ReceiverGroup)1,
					CachingOption = (EventCaching)5
				};
				byte b = 51;
				PhotonNetwork.RaiseEvent(b, (object)new object[1] { Main.serverLink }, val, SendOptions.SendUnreliable);
			}
			Main.RPCProtection();
		}
	}

	public static void ZaWarudo_enableMethod()
	{
		AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Mods/Overpowered/Timestop/start.ogg", "Audio/Mods/Overpowered/Timestop/start.ogg", delegate(AudioClip clip)
		{
			ZaWarudo_Start = clip;
		});
		AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Mods/Overpowered/Timestop/end.ogg", "Audio/Mods/Overpowered/Timestop/end.ogg", delegate(AudioClip clip)
		{
			ZaWarudo_Stop = clip;
		});
	}

	public static void ZaWarudo()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (Main.rightTrigger > 0.5f)
		{
			if (!Buttons.GetIndex("No Freeze Za Warudo").enabled)
			{
				return;
			}
			if (!Buttons.GetIndex("No Freeze Za Warudo").enabled)
			{
				SerializePatch.OverrideSerialization = () => false;
			}
			if (!zaWarudoTrigger)
			{
				if (ZaWarudo_StartCoroutineVariable != null)
				{
					((MonoBehaviour)CoroutineManager.instance).StopCoroutine(ZaWarudo_StartCoroutineVariable);
					ZaWarudo_StartCoroutineVariable = null;
				}
				if (ZaWarudo_EndCoroutineVariable != null)
				{
					((MonoBehaviour)CoroutineManager.instance).StopCoroutine(ZaWarudo_StartCoroutineVariable);
					ZaWarudo_EndCoroutineVariable = null;
				}
				ZaWarudo_StartCoroutineVariable = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(ZaWarudo_StartCoroutine());
			}
			zaWarudoTrigger = true;
			Movement.LowGravity();
			if (!Buttons.GetIndex("No Freeze Za Warudo").enabled)
			{
				FreezeServer();
			}
			return;
		}
		if (zaWarudoTrigger)
		{
			if (ZaWarudo_StartCoroutineVariable != null)
			{
				((MonoBehaviour)CoroutineManager.instance).StopCoroutine(ZaWarudo_StartCoroutineVariable);
				ZaWarudo_StartCoroutineVariable = null;
			}
			if (ZaWarudo_EndCoroutineVariable != null)
			{
				((MonoBehaviour)CoroutineManager.instance).StopCoroutine(ZaWarudo_EndCoroutineVariable);
				ZaWarudo_EndCoroutineVariable = null;
			}
			ZaWarudo_EndCoroutineVariable = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(ZaWarudo_StopCoroutine());
		}
		SerializePatch.OverrideSerialization = null;
		zaWarudoTrigger = false;
	}

	public static IEnumerator ZaWarudo_StartCoroutine()
	{
		Sound.PlayAudio(ZaWarudo_Start);
		yield return (object)new WaitForSeconds(2.4f);
		float endWhiteFadeTime = Time.time;
		Vector3 originPoint = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
		while (Time.time < endWhiteFadeTime + 0.2f)
		{
			float t = (Time.time - endWhiteFadeTime) / 0.2f;
			Fun.HueShift(Color.Lerp(Color.clear, Color.white, t));
			Main.TeleportPlayer(originPoint + RandomUtilities.RandomVector3(t * 0.2f));
			yield return null;
		}
		float purpleFadeTime = Time.time;
		while (Time.time < purpleFadeTime + 2f)
		{
			float t2 = (Time.time - purpleFadeTime) / 2f;
			Fun.HueShift(Color.Lerp(Color.white, Color32.op_Implicit(new Color32((byte)120, (byte)47, (byte)196, (byte)100)), t2));
			Main.TeleportPlayer(originPoint + RandomUtilities.RandomVector3((1f - t2) * 0.2f));
			yield return null;
		}
		Main.TeleportPlayer(originPoint);
		Fun.HueShift(Color32.op_Implicit(new Color32((byte)120, (byte)47, (byte)196, (byte)100)));
		ZaWarudo_StartCoroutineVariable = null;
	}

	public static IEnumerator ZaWarudo_StopCoroutine()
	{
		Sound.PlayAudio(ZaWarudo_Stop);
		yield return (object)new WaitForSeconds(0.5f);
		float purpleFadeTime = Time.time;
		while (Time.time < purpleFadeTime + 1f)
		{
			float t = Time.time - purpleFadeTime;
			Fun.HueShift(Color.Lerp(Color32.op_Implicit(new Color32((byte)120, (byte)47, (byte)196, (byte)100)), Color32.op_Implicit(new Color32((byte)120, (byte)47, (byte)196, (byte)0)), t));
			yield return null;
		}
		Fun.HueShift(Color.clear);
		ZaWarudo_EndCoroutineVariable = null;
	}

	public static void ChangeLagPower(bool positive = true)
	{
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				lagIndex++;
			}
			else
			{
				lagIndex--;
			}
		}
		lagIndex %= 3;
		if (lagIndex < 0)
		{
			lagIndex = 2;
		}
		lagAmount = (new int[5] { 40, 113, 425, 1000, 3800 })[lagIndex];
		lagDelay = (new float[5] { 0.1f, 0.25f, 1f, 3f, 8f })[lagIndex];
		Buttons.GetIndex("Change Lag Power").overlapText = "Change Lag Power <color=grey>[</color><color=green>" + (new string[5] { "Light", "Heavy", "Spike", "Stutter", "Freeze" })[lagIndex] + "</color><color=grey>]</color>";
	}

	public static void ChangeLagType(bool positive = true)
	{
		string[] array = new string[1] { "Destroy" };
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				lagTypeIndex++;
			}
			else
			{
				lagTypeIndex--;
			}
		}
		lagTypeIndex %= array.Length;
		if (lagTypeIndex < 0)
		{
			lagTypeIndex = array.Length - 1;
		}
		Buttons.GetIndex("Change Lag Type").overlapText = "Change Lag Type <color=grey>[</color><color=green>" + array[lagTypeIndex] + "</color><color=grey>]</color>";
	}

	public static bool IsLagMethodRPC()
	{
		int num = lagTypeIndex;
		if (1 == 0)
		{
		}
		bool result = num switch
		{
			0 => true, 
			1 => false, 
			_ => true, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public static void LagTarget(object target)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Invalid comparison between Unknown and I4
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom || Time.time < lagDebounce)
		{
			return;
		}
		VRRig val = (VRRig)((target is VRRig) ? target : null);
		if (val != null)
		{
			target = val.GetPhotonPlayer();
		}
		NetPlayer val2 = (NetPlayer)((target is NetPlayer) ? target : null);
		if (val2 != null)
		{
			target = val2.GetPlayer();
		}
		lagDebounce = Time.time + lagDelay;
		byte b = 186;
		object obj = new object[1] { float.NaN };
		SendOptions val3 = default(SendOptions);
		((SendOptions)(ref val3)).Reliability = false;
		val3.DeliveryMode = (DeliveryMode)0;
		SendOptions val4 = val3;
		RaiseEventOptions val5 = new RaiseEventOptions
		{
			CachingOption = (EventCaching)0
		};
		object obj2 = target;
		object obj3 = obj2;
		if (!(obj3 is RpcTarget val6))
		{
			Player val7 = (Player)((obj3 is Player) ? obj3 : null);
			if (val7 == null)
			{
				if (obj3 is int[] targetActors)
				{
					val5.TargetActors = targetActors;
				}
			}
			else
			{
				val5.TargetActors = new int[1] { val7.ActorNumber };
			}
		}
		else
		{
			val5.Receivers = (ReceiverGroup)(((int)val6 == 0) ? 1 : (((int)val6 == 2) ? 2 : 0));
		}
		for (int i = 0; i < lagAmount; i++)
		{
			PhotonNetwork.NetworkingClient.OpRaiseEvent(b, obj, val5, val4);
		}
		Main.RPCProtection();
	}

	public static void ForceGrabGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				ForceGrab(Main.lockTarget, ((Component)VRRig.LocalRig).transform.position);
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
			if (Main.gunLocked)
			{
				Main.gunLocked = false;
			}
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void FlingGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				ForceGrab(Main.lockTarget, new Vector3(500f, 500f, 500f));
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
			if (Main.gunLocked)
			{
				Main.gunLocked = false;
			}
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void FlingAll()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		ForceGrab(new Vector3(500f, 500f, 500f));
	}

	public static void BringPlayerGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				ForceGrab(Main.lockTarget, ((Component)VRRig.LocalRig).transform.position - ((Component)VRRig.LocalRig).transform.forward * 10f);
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
			Main.gunLocked = false;
		}
	}

	public static void BringAllPlayers()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ForceGrab(((Component)VRRig.LocalRig).transform.position - ((Component)VRRig.LocalRig).transform.forward * 10f);
	}

	public static void PushPlayerGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				ForceGrab(Main.lockTarget, ((Component)Main.lockTarget).transform.position - ((Component)Main.lockTarget).transform.forward * 10f);
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
			if (Main.gunLocked)
			{
				Main.gunLocked = false;
			}
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void PushAllPlayers()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			ForceGrab(activeRig, ((Component)activeRig).transform.position - ((Component)activeRig).transform.forward * 10f);
		}
	}

	public static void CrashGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				ForceGrab(Main.lockTarget, new Vector3(50000f, 50000f, 50000f));
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
			if (Main.gunLocked)
			{
				Main.gunLocked = false;
			}
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void CrashAll()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		ForceGrab(new Vector3(50000f, 50000f, 50000f));
	}

	public static void LagGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				LagTarget(Main.lockTarget.GetPlayer());
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
			Main.gunLocked = false;
		}
	}

	public static void LagAll()
	{
		LagTarget((object)(RpcTarget)1);
	}

	public static void LagAura()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<int> list = new List<int>();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (Vector3.Distance(((Component)activeRig).transform.position, ((Component)VRRig.LocalRig).transform.position) < 4f && !activeRig.IsTagged())
			{
				list.Add(RigUtilities.GetPlayerFromVRRig(activeRig).ActorNumber);
			}
			else if (list.Contains(RigUtilities.GetPlayerFromVRRig(activeRig).ActorNumber))
			{
				list.Remove(RigUtilities.GetPlayerFromVRRig(activeRig).ActorNumber);
			}
		}
		if (list.Count > 0)
		{
			LagTarget(list);
		}
	}

	public static void LagOnTouch()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<int> list = new List<int>();
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig rig) => !rig.IsLocal()))
		{
			if (Vector3.Distance(((Component)item).transform.position, VRRig.LocalRig.rightHandTransform.position) <= 0.35f || Vector3.Distance(((Component)item).transform.position, VRRig.LocalRig.leftHandTransform.position) <= 0.35f)
			{
				list.Add(RigUtilities.GetPlayerFromVRRig(item).ActorNumber);
			}
		}
		if (list.Count > 0)
		{
			LagTarget(list);
		}
	}

	public static void MuteTarget(object target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		RaiseEventOptions val = new RaiseEventOptions();
		if (target is ReceiverGroup receivers)
		{
			val.Receivers = receivers;
		}
		else if (target is int[] targetActors)
		{
			val.TargetActors = targetActors;
		}
		else
		{
			RaiseEventOptions val2 = (RaiseEventOptions)((target is RaiseEventOptions) ? target : null);
			if (val2 == null)
			{
				return;
			}
			val = val2;
		}
		SendOptions val3 = default(SendOptions);
		((SendOptions)(ref val3)).Reliability = false;
		val3.Channel = 0;
		SendOptions val4 = val3;
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>
		{
			{ 1, 255 },
			{
				2,
				VoiceManager.Get().SamplingRate
			},
			{ 3, 2 },
			{ 4, 20000 },
			{ 5, 30000 },
			{ 10, null },
			{
				11,
				(byte)0
			},
			{
				12,
				(object)(Codec)11
			}
		};
		object[] array = new object[3]
		{
			(byte)0,
			(byte)1,
			new object[1] { dictionary }
		};
		((LoadBalancingClient)((VoiceConnection)PhotonVoiceNetwork.Instance).Client).OpRaiseEvent((byte)202, (object)array, val, val4);
	}

	public static void ServerMuteAll()
	{
		for (int i = 0; i < 2; i++)
		{
			MuteTarget((object)(ReceiverGroup)1);
		}
	}

	public static void DeafenGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				for (int i = 0; i < 2; i++)
				{
					MuteTarget(new int[1] { Main.lockTarget.GetPlayer().ActorNumber });
				}
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
			Main.gunLocked = false;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void DeafenAll()
	{
		for (int i = 0; i < 2; i++)
		{
			MuteTarget((object)(ReceiverGroup)0);
		}
	}

	public static void BarrelFlingGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Vector3 pos = ((Component)Main.lockTarget).transform.position + new Vector3(0f, -1E-09f, 0f);
				Vector3 vel = new Vector3(0f, 1020f, 0f);
				Quaternion identity = Quaternion.identity;
				RaiseEventOptions val2 = new RaiseEventOptions();
				val2.TargetActors = new int[1] { RigUtilities.GetPlayerFromVRRig(Main.lockTarget).ActorNumber };
				SendBarrelProjectile(pos, vel, identity, val2);
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
			Main.gunLocked = false;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void BarrelFlingAll()
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		SerializePatch.OverrideSerialization = () => false;
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig TargetRig) => !TargetRig.IsTagged()))
		{
			Vector3 pos = ((Component)item).transform.position + new Vector3(0f, -1E-09f, 0f);
			Vector3 vel = new Vector3(0f, 1020f, 0f);
			Quaternion identity = Quaternion.identity;
			RaiseEventOptions val = new RaiseEventOptions();
			val.TargetActors = new int[1] { RigUtilities.GetPlayerFromVRRig(item).ActorNumber };
			SendBarrelProjectile(pos, vel, identity, val);
			if (Time.time > barrelAllDelay)
			{
				throwableProjectileTimeout = 0f;
			}
		}
		if (Time.time > barrelAllDelay)
		{
			barrelAllDelay = Time.time + 0.3f;
		}
	}

	public static void BarrelObliterateGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Vector3 pos = ((Component)Main.lockTarget).transform.position + new Vector3(0f, -1E-09f, 0f);
				Vector3 vel = new Vector3(0f, 8000f, 0f);
				Quaternion identity = Quaternion.identity;
				RaiseEventOptions val2 = new RaiseEventOptions();
				val2.TargetActors = new int[1] { RigUtilities.GetPlayerFromVRRig(Main.lockTarget).ActorNumber };
				SendBarrelProjectile(pos, vel, identity, val2);
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
			Main.gunLocked = false;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void BarrelObliterateAll()
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		SerializePatch.OverrideSerialization = () => false;
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig TargetRig) => !TargetRig.IsTagged()))
		{
			Vector3 pos = ((Component)item).transform.position + new Vector3(0f, -1E-09f, 0f);
			Vector3 vel = new Vector3(0f, 8000f, 0f);
			Quaternion identity = Quaternion.identity;
			RaiseEventOptions val = new RaiseEventOptions();
			val.TargetActors = new int[1] { RigUtilities.GetPlayerFromVRRig(item).ActorNumber };
			SendBarrelProjectile(pos, vel, identity, val);
			if (Time.time > barrelAllDelay)
			{
				throwableProjectileTimeout = 0f;
			}
		}
		if (Time.time > barrelAllDelay)
		{
			barrelAllDelay = Time.time + 0.3f;
		}
	}

	public static void BarrelPunchMod()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.isLocal && (Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, activeRig.headMesh.transform.position) < 0.25f || Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, activeRig.headMesh.transform.position) < 0.25f))
			{
				Vector3 val = activeRig.headMesh.transform.position - ((Component)GorillaTagger.Instance.headCollider).transform.position;
				Vector3 position = ((Component)activeRig).transform.position;
				Vector3 val2 = ((Component)GorillaTagger.Instance.headCollider).transform.position - activeRig.headMesh.transform.position;
				Vector3 pos = position + ((Vector3)(ref val2)).normalized * 0.1f;
				Vector3 vel = ((Vector3)(ref val)).normalized * 50f;
				Quaternion identity = Quaternion.identity;
				RaiseEventOptions val3 = new RaiseEventOptions();
				val3.TargetActors = new int[1] { RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(activeRig)).ActorNumber };
				SendBarrelProjectile(pos, vel, identity, val3);
				if (Buttons.GetIndex("Graphic Punch Mod").enabled)
				{
					Projectiles.BetaFireProjectile("AppleLeftAnchor", activeRig.headMesh.transform.position, Vector3.down * 600f, Color32.op_Implicit(new Color32((byte)100, (byte)0, (byte)0, byte.MaxValue)));
				}
			}
		}
	}

	public static void BarrelCrashGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Vector3 position = ((Component)Main.lockTarget).transform.position;
				Vector3 vel = new Vector3(0f, 5000f, 0f);
				Quaternion identity = Quaternion.identity;
				RaiseEventOptions val2 = new RaiseEventOptions();
				val2.TargetActors = new int[1] { RigUtilities.GetPlayerFromVRRig(Main.lockTarget).ActorNumber };
				SendBarrelProjectile(position, vel, identity, val2);
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
			Main.gunLocked = false;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void BarrelCrashAll()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		SerializePatch.OverrideSerialization = () => false;
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig TargetRig) => !TargetRig.IsTagged()))
		{
			Vector3 position = ((Component)item).transform.position;
			Vector3 vel = new Vector3(0f, 5000f, 0f);
			Quaternion identity = Quaternion.identity;
			RaiseEventOptions val = new RaiseEventOptions();
			val.TargetActors = new int[1] { RigUtilities.GetPlayerFromVRRig(item).ActorNumber };
			SendBarrelProjectile(position, vel, identity, val);
			if (Time.time > barrelAllDelay)
			{
				throwableProjectileTimeout = 0f;
			}
		}
		if (Time.time > barrelAllDelay)
		{
			barrelAllDelay = Time.time + 0.3f;
		}
	}

	public static void SendBarrelProjectile(Vector3 pos, Vector3 vel, Quaternion rot, RaiseEventOptions options = null, bool disableCooldown = false)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		if (options == null)
		{
			options = new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)1
			};
		}
		int num = 618;
		DistancePatch.enabled = true;
		if (Fun.DisableThrowableCoroutine != null)
		{
			((MonoBehaviour)CoroutineManager.instance).StopCoroutine(Fun.DisableThrowableCoroutine);
		}
		Fun.DisableThrowableCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(Fun.DisableThrowable(num));
		TransferrableObject val = VRRig.LocalRig.myBodyDockPositions.allObjects[num];
		string throwableItemName = Fun.GetThrowableItemName(val);
		if (throwableItemName == null || !Main.CosmeticsOwned.Contains(throwableItemName))
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = Main.TryOnRoom.transform.position;
		}
		if (!((Component)val).gameObject.activeSelf)
		{
			VRRig.LocalRig.SetActiveTransferrableObjectIndex(1, num);
			((Component)val).gameObject.SetActive(true);
		}
		val.storedZone = (DropPositions)2;
		val.currentState = (PositionState)8;
		if (((Component)val).gameObject.activeSelf && Time.time > throwableProjectileTimeout)
		{
			if (!disableCooldown)
			{
				throwableProjectileTimeout = Time.time + 0.3f;
			}
			DeployableObject component = ((Component)val).GetComponent<DeployableObject>();
			object[] array = new object[5]
			{
				((PhotonSignal)component._deploySignal)._signalID,
				PhotonNetwork.ServerTimestamp,
				BitPackUtils.PackWorldPosForNetwork(pos),
				BitPackUtils.PackQuaternionForNetwork(rot),
				BitPackUtils.PackWorldPosForNetwork(vel)
			};
			RaiseEventOptions obj = options;
			SendOptions val2 = default(SendOptions);
			((SendOptions)(ref val2)).Reliability = false;
			val2.DeliveryMode = (DeliveryMode)3;
			PhotonNetwork.RaiseEvent((byte)177, (object)array, obj, val2);
			component._child.Deploy(component, pos, rot, vel, false);
			component.DeployChild();
			Main.RPCProtection();
		}
	}

	public static void BarrelKickGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Vector3 val2 = new Vector3(-71.33718f, 101.4977f, -93.09029f) - Main.lockTarget.headMesh.transform.position;
				Vector3 position = ((Component)Main.lockTarget).transform.position;
				Vector3 val3 = Main.lockTarget.headMesh.transform.position - new Vector3(-71.33718f, 101.4977f, -93.09029f);
				Vector3 pos = position + ((Vector3)(ref val3)).normalized * 0.1f;
				Vector3 vel = ((Vector3)(ref val2)).normalized * 50f;
				Quaternion identity = Quaternion.identity;
				RaiseEventOptions val4 = new RaiseEventOptions();
				val4.TargetActors = new int[1] { RigUtilities.GetPlayerFromVRRig(Main.lockTarget).ActorNumber };
				SendBarrelProjectile(pos, vel, identity, val4);
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
			Main.gunLocked = false;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void BarrelKickAll()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Expected O, but got Unknown
		SerializePatch.OverrideSerialization = () => false;
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.IsTagged())
			{
				Vector3 val = new Vector3(-71.33718f, 101.4977f, -93.09029f) - activeRig.headMesh.transform.position;
				Vector3 position = ((Component)activeRig).transform.position;
				Vector3 val2 = activeRig.headMesh.transform.position - new Vector3(-71.33718f, 101.4977f, -93.09029f);
				Vector3 pos = position + ((Vector3)(ref val2)).normalized * 0.1f;
				Vector3 vel = ((Vector3)(ref val)).normalized * 50f;
				Quaternion identity = Quaternion.identity;
				RaiseEventOptions val3 = new RaiseEventOptions();
				val3.TargetActors = new int[1] { RigUtilities.GetPlayerFromVRRig(activeRig).ActorNumber };
				SendBarrelProjectile(pos, vel, identity, val3);
				if (Time.time > barrelAllDelay)
				{
					throwableProjectileTimeout = 0f;
				}
			}
		}
		if (Time.time > barrelAllDelay)
		{
			barrelAllDelay = Time.time + 0.3f;
		}
	}

	public static void BarrelFlingTowardsGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Vector3 position = ((Component)Main.lockTarget).transform.position;
				Vector3 val2 = ((Component)GorillaTagger.Instance.headCollider).transform.position - Main.lockTarget.headMesh.transform.position;
				Vector3 pos = position + ((Vector3)(ref val2)).normalized * 0.1f;
				val2 = ((Component)GorillaTagger.Instance.bodyCollider).transform.position - ((Component)Main.lockTarget).transform.position;
				Vector3 vel = ((Vector3)(ref val2)).normalized * 5000f;
				Quaternion identity = Quaternion.identity;
				RaiseEventOptions val3 = new RaiseEventOptions();
				val3.TargetActors = new int[1] { RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(Main.lockTarget)).ActorNumber };
				SendBarrelProjectile(pos, vel, identity, val3);
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
			Main.gunLocked = false;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void BarrelFlingTowardsAll()
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Expected O, but got Unknown
		SerializePatch.OverrideSerialization = () => false;
		if (!(Time.time > throwableProjectileTimeout))
		{
			return;
		}
		throwableProjectileTimeout = Time.time + 0.31f;
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig TargetRig) => !TargetRig.IsTagged()))
		{
			Vector3 position = ((Component)item).transform.position;
			Vector3 val = ((Component)GorillaTagger.Instance.headCollider).transform.position - item.headMesh.transform.position;
			Vector3 pos = position + ((Vector3)(ref val)).normalized * 0.1f;
			val = ((Component)GorillaTagger.Instance.bodyCollider).transform.position - ((Component)item).transform.position;
			Vector3 vel = ((Vector3)(ref val)).normalized * 5000f;
			Quaternion identity = Quaternion.identity;
			RaiseEventOptions val2 = new RaiseEventOptions();
			val2.TargetActors = new int[1] { RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(item)).ActorNumber };
			SendBarrelProjectile(pos, vel, identity, val2, disableCooldown: true);
		}
	}

	public static void CityKickGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Vector3 position = ((Component)Main.lockTarget).transform.position;
				Vector3 val2 = ((Component)Main.lockTarget).transform.position - new Vector3(-71.14215f, 13.73829f, -95.17883f);
				Vector3 pos = position + ((Vector3)(ref val2)).normalized * 0.1f;
				val2 = new Vector3(-71.14215f, 13.73829f, -95.17883f) - ((Component)Main.lockTarget).transform.position;
				Vector3 vel = ((Vector3)(ref val2)).normalized * 5000f;
				Quaternion identity = Quaternion.identity;
				RaiseEventOptions val3 = new RaiseEventOptions();
				val3.TargetActors = new int[1] { RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(Main.lockTarget)).ActorNumber };
				SendBarrelProjectile(pos, vel, identity, val3);
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
			Main.gunLocked = false;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void CityKickAll()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		SerializePatch.OverrideSerialization = () => false;
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.IsTagged())
			{
				Vector3 position = ((Component)activeRig).transform.position;
				Vector3 val = ((Component)activeRig).transform.position - new Vector3(-71.14215f, 13.73829f, -95.17883f);
				Vector3 pos = position + ((Vector3)(ref val)).normalized * 0.1f;
				val = new Vector3(-71.14215f, 13.73829f, -95.17883f) - ((Component)activeRig).transform.position;
				Vector3 vel = ((Vector3)(ref val)).normalized * 5000f;
				Quaternion identity = Quaternion.identity;
				RaiseEventOptions val2 = new RaiseEventOptions();
				val2.TargetActors = new int[1] { RigUtilities.GetPlayerFromVRRig(activeRig).ActorNumber };
				SendBarrelProjectile(pos, vel, identity, val2);
				if (Time.time > barrelAllDelay)
				{
					throwableProjectileTimeout = 0f;
				}
			}
		}
		if (Time.time > barrelAllDelay)
		{
			barrelAllDelay = Time.time + 0.3f;
		}
	}

	public static bool IsModded(bool notify)
	{
		if (!PhotonNetwork.InRoom)
		{
			return false;
		}
		bool flag = NetworkSystem.Instance.GameModeString.Contains("MODDED_");
		if (!flag && notify && Time.time > notifyTime)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not in a modded gamemode. Use Utilla to create one, or join an already existing one.");
			notifyTime = Time.time + 1f;
		}
		return flag;
	}

	public static void BetaNearbyFollowCommand(GorillaFriendCollider friendCollider, Player player)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		((PhotonNetworkController)PhotonNetworkController.Instance).FriendIDList.Add(player.UserId);
		object[] array = new object[2]
		{
			((PhotonNetworkController)PhotonNetworkController.Instance).shuffler,
			((PhotonNetworkController)PhotonNetworkController.Instance).keyStr
		};
		NetEventOptions val = new NetEventOptions();
		val.TargetActors = new int[1] { player.ActorNumber };
		NetEventOptions val2 = val;
		if (friendCollider.playerIDsCurrentlyTouching.Contains(PhotonNetwork.LocalPlayer.UserId) && friendCollider.playerIDsCurrentlyTouching.Contains(player.UserId) && player != PhotonNetwork.LocalPlayer)
		{
			RoomSystem.SendEvent((byte)4, array, ref val2, false);
		}
		else if (!friendCollider.playerIDsCurrentlyTouching.Contains(PhotonNetwork.LocalPlayer.UserId))
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not in stump.");
		}
	}

	public static IEnumerator StumpKickDelay(Action action, Action action2, float extraDelay = 0f, bool changeQueue = false)
	{
		((PhotonNetworkController)PhotonNetworkController.Instance).FriendIDList.Clear();
		yield return (object)new WaitForSeconds(extraDelay);
		bool joinedRoomPatchEnabled = JoinedRoomPatch.enabled;
		string queueArchive = ((GorillaComputer)GorillaComputer.instance).currentQueue;
		if (changeQueue)
		{
			((GorillaComputer)GorillaComputer.instance).currentQueue = RandomUtilities.RandomString();
		}
		action?.Invoke();
		yield return (object)new WaitForSeconds(0.3f);
		action2?.Invoke();
		yield return (object)new WaitForSeconds(1f);
		if (changeQueue)
		{
			((GorillaComputer)GorillaComputer.instance).currentQueue = queueArchive;
		}
		yield return (object)new WaitForSeconds(30f);
		JoinedRoomPatch.enabled = joinedRoomPatchEnabled;
	}

	public static void CreateKickRoom()
	{
		if (rejoinOnKick)
		{
			Important.BroadcastRoom(specificRoom ?? RandomUtilities.RandomString(), create: true, ((PhotonNetworkController)PhotonNetworkController.Instance).keyToFollow, ((PhotonNetworkController)PhotonNetworkController.Instance).shuffler);
			Important.Reconnect();
		}
		else
		{
			Important.CreateRoom(specificRoom ?? RandomUtilities.RandomString(), kickToPublic, 0, (JoinType)1);
		}
	}

	public static void StumpKickGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (!Main.GetGunInput(isShooting: true) || !(Time.time > kickDelay))
		{
			return;
		}
		VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
		if (!Object.op_Implicit((Object)(object)componentInParent) || componentInParent.IsLocal())
		{
			return;
		}
		NetPlayer player = RigUtilities.GetPlayerFromVRRig(componentInParent);
		kickDelay = Time.time + 0.5f;
		if (!((GorillaComputer)GorillaComputer.instance).friendJoinCollider.playerIDsCurrentlyTouching.Contains(player.UserId))
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> The player must be in stump.");
			return;
		}
		if (!NetworkSystem.Instance.SessionIsPrivate)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must be in a private room.");
			return;
		}
		((MonoBehaviour)CoroutineManager.instance).StartCoroutine(StumpKickDelay(delegate
		{
			((PhotonNetworkController)PhotonNetworkController.Instance).shuffler = Random.Range(0, 99).ToString().PadLeft(2, '0') + Random.Range(0, 99999999).ToString().PadLeft(8, '0');
			((PhotonNetworkController)PhotonNetworkController.Instance).keyStr = Random.Range(0, 99999999).ToString().PadLeft(8, '0');
			BetaNearbyFollowCommand(((GorillaComputer)GorillaComputer.instance).friendJoinCollider, RigUtilities.NetPlayerToPlayer(player));
			Main.RPCProtection();
		}, delegate
		{
			CreateKickRoom();
		}));
	}

	public static void StumpKickAll()
	{
		if (PhotonNetwork.InRoom)
		{
			if (!NetworkSystem.Instance.SessionIsPrivate)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must be in a private room.");
				return;
			}
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(StumpKickDelay(delegate
			{
				((PhotonNetworkController)PhotonNetworkController.Instance).shuffler = Random.Range(0, 99).ToString().PadLeft(2, '0') + Random.Range(0, 99999999).ToString().PadLeft(8, '0');
				((PhotonNetworkController)PhotonNetworkController.Instance).keyStr = Random.Range(0, 99999999).ToString().PadLeft(8, '0');
				foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig rig) => !rig.IsLocal() && ((GorillaComputer)GorillaComputer.instance).friendJoinCollider.playerIDsCurrentlyTouching.Contains(rig.GetPlayer().UserId)))
				{
					BetaNearbyFollowCommand(((GorillaComputer)GorillaComputer.instance).friendJoinCollider, RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(item)));
				}
				Main.RPCProtection();
			}, delegate
			{
				CreateKickRoom();
			}));
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not in a room.");
		}
	}

	public static void ElevatorKickGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected I4, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected I4, but got Unknown
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (!Main.GetGunInput(isShooting: true))
		{
			return;
		}
		VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
		if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal() && Time.time > elevatorKickDelay)
		{
			elevatorKickDelay = Time.time + 0.5f;
			if (PhotonNetwork.IsMasterClient)
			{
				PhotonView photonView = GRElevatorManager._instance.photonView;
				RaiseEventOptions val2 = new RaiseEventOptions();
				val2.TargetActors = new int[1] { componentInParent.GetPlayer().ActorNumber };
				SpecialTimeRPC(photonView, -750, "RemoteActivateTeleport", val2, (int)GRElevatorManager._instance.currentLocation, 3, GRElevatorManager.LowestActorNumberInElevator());
			}
			else
			{
				((NetworkView)GRElevatorManager._instance).SendRPC("RemoteElevatorButtonPress", (RpcTarget)2, new object[1] { new int[2]
				{
					3,
					(int)GRElevatorManager._instance.currentLocation
				} });
			}
			Main.RPCProtection();
		}
	}

	public static void ElevatorKickAll()
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected I4, but got Unknown
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected I4, but got Unknown
		//IL_005d: Expected O, but got Unknown
		if (PhotonNetwork.IsMasterClient)
		{
			SpecialTimeRPC(GRElevatorManager._instance.photonView, -750, "RemoteActivateTeleport", new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)0
			}, (int)GRElevatorManager._instance.currentLocation, 2, GRElevatorManager.LowestActorNumberInElevator());
		}
		else
		{
			((NetworkView)GRElevatorManager._instance).SendRPC("RemoteElevatorButtonPress", (RpcTarget)2, new object[1] { new int[2]
			{
				3,
				(int)GRElevatorManager._instance.currentLocation
			} });
		}
	}

	public static void ElevatorKickAura()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Expected I4, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Expected O, but got Unknown
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Expected I4, but got Unknown
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<VRRig> list = new List<VRRig>();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (Vector3.Distance(((Component)activeRig).transform.position, ((Component)VRRig.LocalRig).transform.position) < 4f && !activeRig.IsLocal())
			{
				list.Add(activeRig);
			}
			else if (list.Contains(activeRig))
			{
				list.Remove(activeRig);
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		foreach (VRRig item in list)
		{
			if (PhotonNetwork.IsMasterClient)
			{
				PhotonView photonView = GRElevatorManager._instance.photonView;
				RaiseEventOptions val = new RaiseEventOptions();
				val.TargetActors = new int[1] { item.GetPlayer().ActorNumber };
				SpecialTimeRPC(photonView, -750, "RemoteActivateTeleport", val, (int)GRElevatorManager._instance.currentLocation, 3, GRElevatorManager.LowestActorNumberInElevator());
			}
			else
			{
				((NetworkView)GRElevatorManager._instance).SendRPC("RemoteElevatorButtonPress", (RpcTarget)2, new object[1] { new int[2]
				{
					3,
					(int)GRElevatorManager._instance.currentLocation
				} });
			}
			Main.RPCProtection();
		}
	}

	public static void ElevatorKickOnTouch()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Expected I4, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Expected O, but got Unknown
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Expected I4, but got Unknown
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<VRRig> list = new List<VRRig>();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.IsLocal() && (Vector3.Distance(((Component)activeRig).transform.position, VRRig.LocalRig.rightHandTransform.position) <= 0.35f || Vector3.Distance(((Component)activeRig).transform.position, VRRig.LocalRig.leftHandTransform.position) <= 0.35f))
			{
				list.Add(activeRig);
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		foreach (VRRig item in list)
		{
			if (PhotonNetwork.IsMasterClient)
			{
				PhotonView photonView = GRElevatorManager._instance.photonView;
				RaiseEventOptions val = new RaiseEventOptions();
				val.TargetActors = new int[1] { item.GetPlayer().ActorNumber };
				SpecialTimeRPC(photonView, -750, "RemoteActivateTeleport", val, (int)GRElevatorManager._instance.currentLocation, 3, GRElevatorManager.LowestActorNumberInElevator());
			}
			else
			{
				((NetworkView)GRElevatorManager._instance).SendRPC("RemoteElevatorButtonPress", (RpcTarget)2, new object[1] { new int[2]
				{
					3,
					(int)GRElevatorManager._instance.currentLocation
				} });
			}
			Main.RPCProtection();
		}
	}

	public static IEnumerator KickMasterClient()
	{
		Buttons.GetIndex("Kick Master Client");
		if (!NetworkSystem.Instance.InRoom)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not in a room.");
			kickCoroutine = null;
			yield break;
		}
		if (NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are master client! You have no one to kick.");
			kickCoroutine = null;
			yield break;
		}
		SerializePatch.OverrideSerialization = () => false;
		Player player = PhotonNetwork.MasterClient;
		VRRig rig = RigUtilities.GetVRRigFromPlayer(NetPlayer.op_Implicit(PhotonNetwork.MasterClient));
		string name = "<color=#" + (((Object)(object)rig != (Object)null) ? ColorUtility.ToHtmlStringRGBA(rig.GetColor()) : "white") + ">" + player.NickName + "</color>";
		NotificationManager.SendNotification("<color=grey>[</color><color=purple>KICK</color><color=grey>]</color> Kicking " + name + ".");
		Main.RPCProtection();
		while (true)
		{
			float time = Time.time + 10f;
			int view = PhotonNetwork.AllocateViewID(0);
			for (int i = 0; i < 3965; i++)
			{
				LoadBalancingClient networkingClient = PhotonNetwork.NetworkingClient;
				Hashtable val = new Hashtable();
				val.Add((byte)0, (object)"GameMode");
				val.Add((byte)6, (object)PhotonNetwork.ServerTimestamp);
				val.Add((byte)7, (object)view);
				networkingClient.OpRaiseEvent((byte)202, (object)val, new RaiseEventOptions
				{
					Receivers = (ReceiverGroup)2
				}, SendOptions.SendReliable);
			}
			while (true)
			{
				if (PhotonNetwork.PlayerList.Contains(player))
				{
					if (Time.time > time)
					{
						break;
					}
					yield return null;
					continue;
				}
				SerializePatch.OverrideSerialization = null;
				NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> " + name + " has been kicked!");
				kickCoroutine = null;
				yield break;
			}
			NotificationManager.SendNotification("<color=grey>[</color><color=purple>KICK</color><color=grey>]</color> Could not kick " + name + ". Trying again..");
			yield return null;
		}
	}

	public static IEnumerator KickTarget(Player player = null)
	{
		float elapsed = 0f;
		bool kickingAll = false;
		if (player == null)
		{
			kickingAll = true;
		}
		if (!PhotonNetwork.InRoom)
		{
			Reset();
			yield break;
		}
		if (Time.time < Main.timeMenuStarted + 3f)
		{
			Reset();
			yield break;
		}
		NotificationManager.SendNotification("<color=grey>[</color><color=purple>KICK</color><color=grey>]</color> Waiting for the room to freeze. This usually occurs on player join.", 10000);
		NotificationManager.information["Kick Status"] = "Waiting for the room to freeze.";
		while (VRRigCache.ActiveRigs.Where((VRRig rig) => (Object)(object)rig != (Object)(object)VRRig.LocalRig).All((VRRig rig) => rig.GetTruePing() < 5000))
		{
			if (!PhotonNetwork.InRoom)
			{
				Reset();
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Stopped trying to kick because you left the room.");
				yield break;
			}
			FreezeServer();
			yield return null;
		}
		float time = Time.time;
		NotificationManager.SendNotification("<color=grey>[</color><color=purple>KICK</color><color=grey>]</color> Kicking " + ((player != null) ? player.NickName : "everyone") + ", please wait a bit..");
		SerializePatch.OverrideSerialization = () => false;
		while (PhotonNetwork.InRoom)
		{
			elapsed = Time.time - time;
			float remaining = Mathf.Max(0f, 30f - elapsed);
			NotificationManager.information["Kick Status"] = $"{Mathf.CeilToInt(elapsed)}s elapsed ({Mathf.CeilToInt(remaining)}s until I give up)";
			if (elapsed > 30f)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Kicking " + ((player != null) ? player.NickName : "everyone") + ", took longer than expected. Please wait a bit. If nothing happens, run this mod again.");
				PhotonNetwork.OpCleanRpcBuffer(VRRig.LocalRig.GetPhotonView());
				Reset();
				yield break;
			}
			((NetworkSceneObject)FriendshipGroupDetection.Instance).photonView.RPC("AddPartyMembers", (player == null) ? ((object)(RpcTarget)1) : player, Main.serverLink.PadRight(500), (short)12, new int[0], null);
			if ((!kickingAll && !PhotonNetwork.PlayerList.Contains(player)) || (kickingAll && PhotonNetwork.CurrentRoom.PlayerCount == 1))
			{
				if (!PhotonNetwork.InRoom)
				{
					Reset();
					NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Stopped trying to kick because you left the room.");
				}
				else
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=purple>KICK</color><color=grey>]</color> Kicked " + ((player != null) ? player.NickName : "everyone") + " successfully!");
					PhotonNetwork.OpCleanRpcBuffer(VRRig.LocalRig.GetPhotonView());
					Reset();
				}
				yield break;
			}
			if (VRRigCache.ActiveRigs.Where((VRRig rig) => (Object)(object)rig != (Object)(object)VRRig.LocalRig).All((VRRig rig) => rig.GetTruePing() < 150))
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Kicking " + ((player != null) ? player.NickName : "everyone") + ", failed, please try again!");
				Reset();
				yield break;
			}
			yield return null;
		}
		if (!PhotonNetwork.InRoom)
		{
			Reset();
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Stopped trying to kick because you left the room.");
		}
		void RemoveElapsedKickTime(bool success = false)
		{
			Task.Run(async delegate
			{
				if (success)
				{
					NotificationManager.information["Kick Status"] = $"Completed in {Mathf.CeilToInt(elapsed)}s seconds!";
					await Task.Delay(3000);
				}
				NotificationManager.information.Remove("Kick Status");
			});
		}
		void Reset()
		{
			SerializePatch.OverrideSerialization = null;
			RemoveElapsedKickTime();
			kickCoroutine = null;
			if (kickingAll)
			{
				Buttons.GetIndex("Kick All").enabled = false;
			}
		}
	}

	public static void KickGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && kickCoroutine == null)
			{
				kickCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(KickTarget(Main.lockTarget.GetPhotonPlayer()));
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
			Main.gunLocked = false;
		}
	}

	public static void KickAll()
	{
		if (kickCoroutine == null)
		{
			kickCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(KickTarget());
		}
	}

	public static void DisableKick()
	{
		SerializePatch.OverrideSerialization = null;
		NotificationManager.information.Remove("Kick Status");
		kickCoroutine = null;
	}

	public static void CreatePeerBase()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		((PhotonPeer)PhotonNetwork.NetworkingClient.LoadBalancingPeer).TransportProtocol = (ConnectionProtocol)1;
		((PhotonPeer)PhotonNetwork.NetworkingClient.LoadBalancingPeer).peerBase = (PeerBase)new TPeer
		{
			DoFraming = true,
			photonPeer = (PhotonPeer)(object)PhotonNetwork.NetworkingClient.LoadBalancingPeer,
			usedTransportProtocol = (ConnectionProtocol)1
		};
	}

	public static void UnloadPeerBase()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		((PhotonPeer)PhotonNetwork.NetworkingClient.LoadBalancingPeer).TransportProtocol = (ConnectionProtocol)0;
		((PhotonPeer)PhotonNetwork.NetworkingClient.LoadBalancingPeer).peerBase = (PeerBase)new EnetPeer
		{
			photonPeer = (PhotonPeer)(object)PhotonNetwork.NetworkingClient.LoadBalancingPeer,
			usedTransportProtocol = (ConnectionProtocol)0
		};
		NetworkSystem.Instance.ReturnToSinglePlayer();
	}

	public static void BetaShuttleFollowCommand(Player player)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		((PhotonNetworkController)PhotonNetworkController.Instance).FriendIDList.Add(player.UserId);
		object[] array = new object[2]
		{
			((PhotonNetworkController)PhotonNetworkController.Instance).shuffler,
			((PhotonNetworkController)PhotonNetworkController.Instance).keyStr
		};
		NetEventOptions val = new NetEventOptions();
		val.TargetActors = new int[1] { player.ActorNumber };
		NetEventOptions val2 = val;
		RoomSystem.SendEvent((byte)11, array, ref val2, false);
	}

	public static void ActivateGreyZoneGun(bool status)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true))
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal() && Time.time > greyZoneDelay)
			{
				greyZoneDelay = Time.time + 0.1f;
				ActivateGreyZone(status, componentInParent.GetPhotonPlayer());
			}
		}
	}

	public static IEnumerator ClearOverride()
	{
		yield return (object)new WaitUntil((Func<bool>)(() => !PhotonNetwork.InRoom));
		SerializePatch.OverrideSerialization = null;
		wipeOverride = null;
	}

	public static void ActivateGreyZone(bool status, Player target)
	{
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			return;
		}
		SerializePatch.OverrideSerialization = delegate
		{
			Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { ((MonoBehaviourPun)GreyZoneManager.Instance).photonView });
			SerializePatch.OverrideSerialization = null;
			return false;
		};
		if (wipeOverride == null)
		{
			wipeOverride = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(ClearOverride());
		}
		((GreyZoneManager)GreyZoneManager.Instance).greyZoneActive = status;
		((GreyZoneManager)GreyZoneManager.Instance).photonConnectedDuringActivation = PhotonNetwork.InRoom;
		((GreyZoneManager)GreyZoneManager.Instance).greyZoneActivationTime = (((GreyZoneManager)GreyZoneManager.Instance).photonConnectedDuringActivation ? PhotonNetwork.Time : ((double)Time.time));
		PhotonView photonView = ((MonoBehaviourPun)GreyZoneManager.Instance).photonView;
		RaiseEventOptions val = new RaiseEventOptions();
		val.TargetActors = new int[1] { target.ActorNumber };
		Main.SendSerialize(photonView, val);
	}

	public static void ActivateGreyZone(bool status)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			}
			else if (status)
			{
				((GreyZoneManager)GreyZoneManager.Instance).ActivateGreyZoneAuthority();
			}
			else if (!status)
			{
				((GreyZoneManager)GreyZoneManager.Instance).DeactivateGreyZoneAuthority();
			}
		}
	}

	public static void SpazGreyZoneGun()
	{
		if (Time.time > spazGreyDelay)
		{
			greyState = !greyState;
			spazGreyDelay = Time.time + 0.1f;
		}
		ActivateGreyZoneGun(greyState);
	}

	public static void SpazGreyZone()
	{
		if (Time.time > spazGreyDelay)
		{
			greyState = !greyState;
			ActivateGreyZone(greyState);
			spazGreyDelay = Time.time + 0.1f;
		}
	}

	public static void KickAllInParty()
	{
		if (FriendshipGroupDetection.Instance.IsInParty)
		{
			Main.partyLastCode = PhotonNetwork.CurrentRoom.Name;
			Main.waitForPlayerJoin = false;
			((PhotonNetworkController)PhotonNetworkController.Instance).AttemptToJoinSpecificRoom(Important.RandomRoomName(), (JoinType)4);
			Main.partyTime = Time.time + 0.25f;
			Main.partyKickReconnecting = false;
			Main.amountPartying = FriendshipGroupDetection.Instance.myPartyMemberIDs.Count - 1;
			NotificationManager.SendNotification("<color=grey>[</color><color=purple>PARTY</color><color=grey>]</color> Kicking " + Main.amountPartying + " party members, please be patient..");
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not in a party.");
		}
	}

	public static void BanAllInParty()
	{
		if (FriendshipGroupDetection.Instance.IsInParty)
		{
			Main.partyLastCode = PhotonNetwork.CurrentRoom.Name;
			Main.waitForPlayerJoin = true;
			((PhotonNetworkController)PhotonNetworkController.Instance).AttemptToJoinSpecificRoom("KKK", (JoinType)4);
			Main.partyTime = Time.time + 0.25f;
			Main.partyKickReconnecting = false;
			Main.amountPartying = FriendshipGroupDetection.Instance.myPartyMemberIDs.Count - 1;
			NotificationManager.SendNotification("<color=grey>[</color><color=purple>PARTY</color><color=grey>]</color> Banning " + Main.amountPartying + " party members, please be patient..");
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not in a party.");
		}
	}

	public static IEnumerator PartyKickDelay(bool ban)
	{
		yield return (object)new WaitForSeconds(0.25f);
		if (ban)
		{
			BanAllInParty();
		}
		else
		{
			KickAllInParty();
		}
		Coroutine thisCoroutine = partyKickDelayCoroutine;
		partyKickDelayCoroutine = null;
		((MonoBehaviour)CoroutineManager.instance).StopCoroutine(thisCoroutine);
	}

	public static void AutoPartyKick()
	{
		if (FriendshipGroupDetection.Instance.IsInParty && !previousInParty && partyKickDelayCoroutine == null)
		{
			partyKickDelayCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(PartyKickDelay(ban: false));
		}
		previousInParty = FriendshipGroupDetection.Instance.IsInParty;
	}

	public static void AutoPartyBan()
	{
		if (FriendshipGroupDetection.Instance.IsInParty && !previousInParty && partyKickDelayCoroutine == null)
		{
			partyKickDelayCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(PartyKickDelay(ban: true));
		}
		previousInParty = FriendshipGroupDetection.Instance.IsInParty;
	}

	public static void PartyBreakNetworkTriggers()
	{
		if (FriendshipGroupDetection.Instance.IsInParty && Time.time > breakDelay)
		{
			breakDelay = Time.time + 1f;
			((NetworkSceneObject)FriendshipGroupDetection.Instance).photonView.RPC("PartyMemberIsAboutToGroupJoin", (RpcTarget)0, Array.Empty<object>());
		}
	}

	public static void PartyKickGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				OptimizeEvents = true;
			}
			if (!Main.GetGunInput(isShooting: true))
			{
				return;
			}
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (!Object.op_Implicit((Object)(object)componentInParent) || componentInParent.IsLocal())
			{
				return;
			}
			if ((Object)(object)Main.lockTarget == (Object)null && FriendshipGroupDetection.Instance.IsInMyGroup(componentInParent.GetPlayer().UserId))
			{
				for (int i = 0; i < 3970; i++)
				{
					((NetworkSceneObject)FriendshipGroupDetection.Instance).photonView.RPC("RequestPartyGameMode", componentInParent.GetPhotonPlayer(), new object[1] { GameMode.gameModeKeyByName.Keys.ToArray()[Random.Range(0, GameMode.gameModeKeyByName.Keys.Count)] });
				}
				Main.RPCProtection();
			}
			Main.gunLocked = true;
			Main.lockTarget = componentInParent;
		}
		else
		{
			OptimizeEvents = false;
			if (Main.gunLocked)
			{
				Main.gunLocked = false;
			}
		}
	}

	public static void PartyKickAll()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected O, but got Unknown
		SerializePatch.OverrideSerialization = () => false;
		if (!(Time.time > kickDelay))
		{
			return;
		}
		kickDelay = Time.time + 10f;
		for (int num = 0; num < 3950; num++)
		{
			((NetworkSceneObject)FriendshipGroupDetection.Instance).photonView.RPC("RequestPartyGameMode", new RaiseEventOptions
			{
				TargetActors = (from plr in NetworkSystem.Instance.PlayerListOthers
					where FriendshipGroupDetection.Instance.IsInMyGroup(plr.UserId)
					select plr.ActorNumber).ToArray()
			}, GameMode.gameModeKeyByName.Keys.ToArray()[Random.Range(0, GameMode.gameModeKeyByName.Keys.Count)]);
		}
		Main.RPCProtection();
	}

	public static void PartyKickAura()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<VRRig> list = new List<VRRig>();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (Vector3.Distance(((Component)activeRig).transform.position, ((Component)VRRig.LocalRig).transform.position) < 4f && !activeRig.IsLocal())
			{
				list.Add(activeRig);
			}
			else if (list.Contains(activeRig))
			{
				list.Remove(activeRig);
			}
		}
		if (list.Count > 0)
		{
			SerializePatch.OverrideSerialization = () => false;
			{
				foreach (VRRig item in list)
				{
					for (int num = 0; num < 3950; num++)
					{
						((NetworkSceneObject)FriendshipGroupDetection.Instance).photonView.RPC("RequestPartyGameMode", item.GetPhotonPlayer(), new object[1] { GameMode.gameModeKeyByName.Keys.ToArray()[Random.Range(0, GameMode.gameModeKeyByName.Keys.Count)] });
					}
					Main.RPCProtection();
				}
				return;
			}
		}
		OptimizeEvents = false;
	}

	public static void PartyKickOnTouch()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<VRRig> list = new List<VRRig>();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.IsLocal() && (Vector3.Distance(((Component)activeRig).transform.position, VRRig.LocalRig.rightHandTransform.position) <= 0.35f || Vector3.Distance(((Component)activeRig).transform.position, VRRig.LocalRig.leftHandTransform.position) <= 0.35f))
			{
				list.Add(activeRig);
			}
		}
		if (list.Count > 0)
		{
			SerializePatch.OverrideSerialization = () => false;
			{
				foreach (VRRig item in list)
				{
					for (int num = 0; num < 3950; num++)
					{
						((NetworkSceneObject)FriendshipGroupDetection.Instance).photonView.RPC("RequestPartyGameMode", item.GetPhotonPlayer(), new object[1] { GameMode.gameModeKeyByName.Keys.ToArray()[Random.Range(0, GameMode.gameModeKeyByName.Keys.Count)] });
					}
					Main.RPCProtection();
				}
				return;
			}
		}
		OptimizeEvents = false;
	}

	public static void AntiReportLag()
	{
		if (Time.time > antiReportLagDelay)
		{
			List<int> actors = new List<int>();
			Safety.AntiReport(delegate(VRRig vrrig, Vector3 position)
			{
				antiReportLagDelay = Time.time + 0.1f;
				actors.Add(RigUtilities.GetPlayerFromVRRig(vrrig).ActorNumber);
				NotificationManager.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + RigUtilities.GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, they are being lagged.");
			});
			if (actors.Count > 0)
			{
				LagTarget(actors);
			}
		}
	}

	public static void SetMasterClient(bool skip = false)
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		if (PhotonNetwork.PlayerList.Length > 5)
		{
			if (!skip)
			{
				NotificationManager.SendNotification($"<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> {PhotonNetwork.PlayerList.Length - 5} people must leave for this mod to work.");
			}
		}
		else if (Time.time > setMasterDelay)
		{
			PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
			setMasterDelay = Time.time + 5f;
		}
	}

	public static void SetRoomStatus(bool status)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		Hashtable val = new Hashtable();
		val.Add((byte)253, (object)status);
		val.Add((byte)254, (object)status);
		val.Add(byte.MaxValue, (object)((!status) ? ((PhotonNetworkController)PhotonNetworkController.Instance).currentJoinTrigger.GetRoomSize(SubscriptionManager.IsLocalSubscribed()) : 0));
		dictionary.Add(251, (object)val);
		dictionary.Add(250, true);
		dictionary.Add(231, null);
		Dictionary<byte, object> dictionary2 = dictionary;
		((PhotonPeer)PhotonNetwork.CurrentRoom.LoadBalancingClient.LoadBalancingPeer).SendOperation((byte)252, dictionary2, SendOptions.SendReliable);
		GorillaScoreboardTotalUpdater.instance.UpdateActiveScoreboards();
	}

	public static void DestroyGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > destroyDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				DestroyPlayer(NetPlayer.op_Implicit(RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(componentInParent))));
				destroyDelay = Time.time + 0.5f;
			}
		}
	}

	public static void DestroyAll()
	{
		Player[] playerListOthers = PhotonNetwork.PlayerListOthers;
		foreach (Player val in playerListOthers)
		{
			DestroyPlayer(NetPlayer.op_Implicit(val));
		}
	}

	public static void DestroyAura()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<VRRig> list = new List<VRRig>();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (Vector3.Distance(((Component)activeRig).transform.position, ((Component)VRRig.LocalRig).transform.position) < 4f && !activeRig.IsLocal())
			{
				list.Add(activeRig);
			}
			else if (list.Contains(activeRig))
			{
				list.Remove(activeRig);
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		foreach (VRRig item in list)
		{
			DestroyPlayer(NetPlayer.op_Implicit(RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(item))));
		}
	}

	public static void DestroyOnTouch()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<VRRig> list = new List<VRRig>();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.IsLocal() && (Vector3.Distance(((Component)activeRig).transform.position, VRRig.LocalRig.rightHandTransform.position) <= 0.35f || Vector3.Distance(((Component)activeRig).transform.position, VRRig.LocalRig.leftHandTransform.position) <= 0.35f))
			{
				list.Add(activeRig);
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		foreach (VRRig item in list)
		{
			DestroyPlayer(NetPlayer.op_Implicit(RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(item))));
		}
	}

	public static void DestroyPlayer(NetPlayer player)
	{
		PhotonNetwork.OpRemoveCompleteCacheOfPlayer(player.ActorNumber);
	}

	public static void ChangeLavaState(RisingLavaState state)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		InfectionLavaController val = InfectionLavaController.ActiveControllers.FirstOrDefault();
		if ((Object)(object)val != (Object)null)
		{
			val.JumpToState(state);
			val.reliableState.stateStartTime = (NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : Time.timeAsDouble);
		}
	}

	public static void TargetSpam()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (NetworkSystem.Instance.IsMasterClient)
		{
			HitTargetNetworkState[] allType = Main.GetAllType<HitTargetNetworkState>(5f);
			foreach (HitTargetNetworkState val in allType)
			{
				val.hitCooldownTime = 0;
				val.TargetHit(Vector3.zero, Vector3.zero);
			}
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
	}

	public static void InfectionToTag()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			return;
		}
		GorillaTagManager val = (GorillaTagManager)GorillaGameManager.instance;
		val.infectedModeThreshold = PhotonNetwork.CurrentRoom.MaxPlayers + 1;
	}

	public static void TagToInfection()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			return;
		}
		GorillaTagManager val = (GorillaTagManager)GorillaGameManager.instance;
		val.infectedModeThreshold = 1;
	}

	public static void FixThreshold()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		GorillaTagManager val = (GorillaTagManager)GorillaGameManager.instance;
		val.infectedModeThreshold = 4;
	}

	public static void RockSelf()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			GameModeUtilities.AddRock(NetworkSystem.Instance.LocalPlayer);
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
	}

	public static void RockGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (!Main.GetGunInput(isShooting: true))
		{
			return;
		}
		VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
		if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal() && Time.time > rockDebounce)
		{
			rockDebounce = Time.time + 0.1f;
			if (PhotonNetwork.IsMasterClient)
			{
				GameModeUtilities.AddRock(RigUtilities.GetPlayerFromVRRig(componentInParent));
			}
			else
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			}
		}
	}

	public static void RockAura()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<VRRig> list = new List<VRRig>();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (Vector3.Distance(((Component)activeRig).transform.position, ((Component)VRRig.LocalRig).transform.position) < 4f && !activeRig.IsLocal())
			{
				list.Add(activeRig);
			}
			else if (list.Contains(activeRig))
			{
				list.Remove(activeRig);
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		foreach (VRRig item in list)
		{
			if (Time.time > rockDebounce)
			{
				rockDebounce = Time.time + 0.1f;
				if (PhotonNetwork.IsMasterClient)
				{
					GameModeUtilities.AddRock(RigUtilities.GetPlayerFromVRRig(item));
				}
				else
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
				}
			}
		}
	}

	public static void RockOnTouch()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<VRRig> list = new List<VRRig>();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.IsLocal() && (Vector3.Distance(((Component)activeRig).transform.position, VRRig.LocalRig.rightHandTransform.position) <= 0.35f || Vector3.Distance(((Component)activeRig).transform.position, VRRig.LocalRig.leftHandTransform.position) <= 0.35f))
			{
				list.Add(activeRig);
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		foreach (VRRig item in list)
		{
			if (Time.time > rockDebounce)
			{
				rockDebounce = Time.time + 0.1f;
				if (PhotonNetwork.IsMasterClient)
				{
					GameModeUtilities.AddRock(RigUtilities.GetPlayerFromVRRig(item));
				}
				else
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
				}
			}
		}
	}

	public static void RockAll()
	{
		if (Time.time > rockDebounce)
		{
			rockDebounce = Time.time + 0.1f;
			if (PhotonNetwork.IsMasterClient)
			{
				GameModeUtilities.AddRock(NetPlayer.op_Implicit(RigUtilities.GetRandomPlayer(includeSelf: true)));
			}
			else
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			}
		}
	}

	public static void BetaSetStatus(StatusEffects state, RaiseEventOptions reo)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected I4, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			return;
		}
		object[] array = new object[1] { (int)state };
		PhotonNetwork.RaiseEvent((byte)3, (object)new object[3]
		{
			NetworkSystem.Instance.ServerTimestamp,
			(byte)2,
			array
		}, reo, SendOptions.SendUnreliable);
	}

	public static void SlowSelf()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		NetPlayer val = NetPlayer.op_Implicit(PhotonNetwork.LocalPlayer);
		RaiseEventOptions val2 = new RaiseEventOptions();
		val2.TargetActors = new int[1] { val.ActorNumber };
		BetaSetStatus((StatusEffects)0, val2);
		Main.RPCProtection();
	}

	public static void SlowGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > slowDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(componentInParent);
				RaiseEventOptions val2 = new RaiseEventOptions();
				val2.TargetActors = new int[1] { playerFromVRRig.ActorNumber };
				BetaSetStatus((StatusEffects)0, val2);
				Main.RPCProtection();
				slowDelay = Time.time + 1f;
			}
		}
	}

	public static void SlowAura()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<VRRig> list = new List<VRRig>();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (Vector3.Distance(((Component)activeRig).transform.position, ((Component)VRRig.LocalRig).transform.position) < 4f && !activeRig.IsLocal())
			{
				list.Add(activeRig);
			}
			else if (list.Contains(activeRig))
			{
				list.Remove(activeRig);
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		foreach (VRRig item in list)
		{
			NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(item);
			RaiseEventOptions val = new RaiseEventOptions();
			val.TargetActors = new int[1] { playerFromVRRig.ActorNumber };
			BetaSetStatus((StatusEffects)0, val);
			Main.RPCProtection();
		}
	}

	public static void SlowOnTouch()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected O, but got Unknown
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<VRRig> list = new List<VRRig>();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.IsLocal() && (Vector3.Distance(((Component)activeRig).transform.position, VRRig.LocalRig.rightHandTransform.position) <= 0.35f || Vector3.Distance(((Component)activeRig).transform.position, VRRig.LocalRig.leftHandTransform.position) <= 0.35f))
			{
				list.Add(activeRig);
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		foreach (VRRig item in list)
		{
			NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(item);
			RaiseEventOptions val = new RaiseEventOptions();
			val.TargetActors = new int[1] { playerFromVRRig.ActorNumber };
			BetaSetStatus((StatusEffects)0, val);
			Main.RPCProtection();
		}
	}

	public static void SlowAll()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		if (Time.time > slowDelay)
		{
			BetaSetStatus((StatusEffects)0, new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)0
			});
			Main.RPCProtection();
			slowDelay = Time.time + 1f;
		}
	}

	public static void VibrateSelf()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		NetPlayer val = NetPlayer.op_Implicit(PhotonNetwork.LocalPlayer);
		RaiseEventOptions val2 = new RaiseEventOptions();
		val2.TargetActors = new int[1] { val.ActorNumber };
		BetaSetStatus((StatusEffects)1, val2);
		Main.RPCProtection();
	}

	public static void VibrateGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > vibrateDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(componentInParent);
				RaiseEventOptions val2 = new RaiseEventOptions();
				val2.TargetActors = new int[1] { playerFromVRRig.ActorNumber };
				BetaSetStatus((StatusEffects)1, val2);
				Main.RPCProtection();
				vibrateDelay = Time.time + 0.5f;
			}
		}
	}

	public static void VibrateAura()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<VRRig> list = new List<VRRig>();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (Vector3.Distance(((Component)activeRig).transform.position, ((Component)VRRig.LocalRig).transform.position) < 4f && !activeRig.IsLocal())
			{
				list.Add(activeRig);
			}
			else if (list.Contains(activeRig))
			{
				list.Remove(activeRig);
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		foreach (VRRig item in list)
		{
			NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(item);
			RaiseEventOptions val = new RaiseEventOptions();
			val.TargetActors = new int[1] { playerFromVRRig.ActorNumber };
			BetaSetStatus((StatusEffects)1, val);
			Main.RPCProtection();
		}
	}

	public static void VibrateOnTouch()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected O, but got Unknown
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<VRRig> list = new List<VRRig>();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.IsLocal() && (Vector3.Distance(((Component)activeRig).transform.position, VRRig.LocalRig.rightHandTransform.position) <= 0.35f || Vector3.Distance(((Component)activeRig).transform.position, VRRig.LocalRig.leftHandTransform.position) <= 0.35f))
			{
				list.Add(activeRig);
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		foreach (VRRig item in list)
		{
			NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(item);
			RaiseEventOptions val = new RaiseEventOptions();
			val.TargetActors = new int[1] { playerFromVRRig.ActorNumber };
			BetaSetStatus((StatusEffects)1, val);
			Main.RPCProtection();
		}
	}

	public static void VibrateAll()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		if (Time.time > vibrateDelay)
		{
			BetaSetStatus((StatusEffects)1, new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)0
			});
			Main.RPCProtection();
			vibrateDelay = Time.time + 0.5f;
		}
	}

	public static void GliderBlindGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
			if (!Main.gunLocked)
			{
				return;
			}
			GliderHoldable[] allType = Main.GetAllType<GliderHoldable>(5f);
			foreach (GliderHoldable val2 in allType)
			{
				if (((NetworkView)val2).GetView.Owner == PhotonNetwork.LocalPlayer)
				{
					((Component)val2).gameObject.transform.position = Main.lockTarget.headMesh.transform.position;
					((Component)val2).gameObject.transform.rotation = Quaternion.Euler(new Vector3((float)Random.Range(0, 360), (float)Random.Range(0, 360), (float)Random.Range(0, 360)));
				}
				else
				{
					((NetworkHoldableObject)val2).OnHover((InteractionPoint)null, (GameObject)null);
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
		}
	}

	public static void GliderBlindAll()
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		GliderHoldable[] allType = Main.GetAllType<GliderHoldable>(5f);
		int num = 0;
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal))
		{
			try
			{
				GliderHoldable val = allType[num];
				if (((NetworkView)val).GetView.Owner == PhotonNetwork.LocalPlayer)
				{
					((Component)val).gameObject.transform.position = item.headMesh.transform.position;
					((Component)val).gameObject.transform.rotation = Quaternion.Euler(new Vector3((float)Random.Range(0, 360), (float)Random.Range(0, 360), (float)Random.Range(0, 360)));
				}
				else
				{
					((NetworkHoldableObject)val).OnHover((InteractionPoint)null, (GameObject)null);
				}
			}
			catch
			{
			}
			num++;
		}
	}

	public static void BreakAudioGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RigUtilities.GetPlayerFromVRRig(Main.lockTarget), new object[3] { 111, false, 999999f });
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
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void BreakAudioAll()
	{
		if (Main.rightTrigger > 0.5f)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", (RpcTarget)1, new object[3] { 111, false, 999999f });
		}
	}

	public static IEnumerator RopeEnableRig()
	{
		yield return (object)new WaitForSeconds(0.3f);
		((Behaviour)VRRig.LocalRig).enabled = true;
	}

	public static void BetaSetRopeVelocity(int RopeId, Vector3 Velocity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		Velocity = GTExt.ClampMagnitudeSafe(Velocity, 100f);
		if (!RopeSwingManager.instance.ropes.TryGetValue(RopeId, out var value))
		{
			return;
		}
		var anon = (from x in value.nodes.Skip(1).Select((Transform v, int i) => new
			{
				index = i,
				transform = v,
				distance = Vector3.Distance(((Component)GorillaTagger.Instance.bodyCollider).transform.position, ((Component)v).transform.position)
			})
			orderby x.distance
			select x).First();
		if (anon.distance > 5f)
		{
			if (RopeCoroutine != null)
			{
				((MonoBehaviour)CoroutineManager.instance).StopCoroutine(RopeCoroutine);
			}
			RopeCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(RopeEnableRig());
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = anon.transform.position;
		}
		if (Vector3.Distance(Main.ServerPos, anon.transform.position) < 5f)
		{
			RopeSwingManager.instance.SendSetVelocity_RPC(RopeId, anon.index, GTExt.ClampMagnitudeSafe(Velocity, 100f), true);
		}
		else
		{
			RopeDelay = 0f;
		}
		Main.RPCProtection();
	}

	public static void BetaSetRopeVelocity(GorillaRopeSwing Rope, Vector3 Velocity)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		BetaSetRopeVelocity(RopeSwingManager.instance.ropes.FirstOrDefault((KeyValuePair<int, GorillaRopeSwing> x) => (Object)(object)x.Value == (Object)(object)Rope).Key, Velocity);
	}

	public static GorillaRopeSwing GetRandomRope()
	{
		if (Time.time > randomRopeDelay)
		{
			randomRopeDelay = Time.time + 0.5f;
			randomRope = RopeSwingManager.instance.ropes.Values.OrderBy((GorillaRopeSwing _) => Random.value).FirstOrDefault();
		}
		return randomRope;
	}

	public static void JoystickRopeControl()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		Vector2 rightJoystick = Main.rightJoystick;
		Vector2 leftJoystick = Main.leftJoystick;
		Vector2 val = ((((Vector2)(ref leftJoystick)).sqrMagnitude > ((Vector2)(ref rightJoystick)).sqrMagnitude) ? leftJoystick : rightJoystick);
		if (((Vector2)(ref val)).sqrMagnitude > 0.0025f && Time.time > RopeDelay)
		{
			RopeDelay = Time.time + 0.125f;
			GorillaRopeSwing randomType = Main.GetRandomType<GorillaRopeSwing>(0.25f);
			BetaSetRopeVelocity(randomType, new Vector3(val.x * 100f, val.y * 100f, 0f));
		}
	}

	public static void SpazRopeGun()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true))
		{
			GorillaRopeSwing componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<GorillaRopeSwing>();
			if (Object.op_Implicit((Object)(object)componentInParent) && Time.time > RopeDelay)
			{
				RopeDelay = Time.time + 0.25f;
				BetaSetRopeVelocity(componentInParent, RandomUtilities.RandomVector3(100f));
			}
		}
	}

	public static void SpazAllRopes()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightTrigger > 0.5f && Time.time > RopeDelay)
		{
			RopeDelay = Time.time + 0.125f;
			GorillaRopeSwing rope = GetRandomRope();
			BetaSetRopeVelocity(rope, RandomUtilities.RandomVector3(100f));
		}
	}

	public static void SpazGrabbedRopes()
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > RopeDelay)
		{
			RopeDelay = Time.time + 0.125f;
			VRRig val = (from _ in VRRigCache.ActiveRigs
				where (Object)(object)_.currentRopeSwing != (Object)null
				orderby Random.value
				select _).FirstOrDefault();
			if ((Object)(object)val != (Object)null)
			{
				BetaSetRopeVelocity(val.currentRopeSwing, RandomUtilities.RandomVector3(100f));
			}
		}
	}

	public static void FlingRopeGun()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true))
		{
			GorillaRopeSwing componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<GorillaRopeSwing>();
			if (Object.op_Implicit((Object)(object)componentInParent) && Time.time > RopeDelay)
			{
				RopeDelay = Time.time + 0.125f;
				BetaSetRopeVelocity(componentInParent, RandomUtilities.RandomVector3(100f));
			}
		}
	}

	public static void FlingAllRopesGun()
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true) && Time.time > RopeDelay)
			{
				RopeDelay = Time.time + 0.125f;
				GorillaRopeSwing val = GetRandomRope();
				Vector3 val2 = item.transform.position - ((Component)val).transform.position;
				BetaSetRopeVelocity(val, ((Vector3)(ref val2)).normalized * 100f);
			}
		}
	}

	public static void EffectSpam(CritterEvent critterEvent)
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected I4, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.rightGrab)
		{
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			CrittersPawn[] array = ((CrittersManager)CrittersManager.instance).crittersPawns.ToArray();
			if (array.Length != 0)
			{
				CrittersPawn val = array[0];
				((Component)val).transform.position = GorillaTagger.Instance.rightHandTransform.position;
				int actorId = ((CrittersActor)val).actorId;
				((CrittersManager)CrittersManager.instance).TriggerEvent(critterEvent, actorId, ((Component)val).transform.position, Quaternion.LookRotation(((Component)val).transform.up));
			}
			return;
		}
		CrittersActorType type = (CrittersActorType)17;
		Vector3 val2 = Vector3.down * 20f;
		switch ((int)critterEvent)
		{
		case 0:
			type = (CrittersActorType)13;
			break;
		case 2:
		case 3:
			type = (CrittersActorType)17;
			break;
		case 1:
			type = (CrittersActorType)16;
			break;
		}
		CrittersGrabber val3 = (from grabber in Main.GetAllType<CrittersGrabber>(5f)
			where ((CrittersActor)grabber).rigPlayerId == PhotonNetwork.LocalPlayer.ActorNumber && grabber.isLeft
			select grabber).FirstOrDefault();
		List<CrittersActor> list = (from critter in Main.GetAllType<CrittersActor>(5f)
			where (Object)(object)critter != (Object)null && critter.crittersActorType == type && Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) < 25f && Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) > 3f
			orderby Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) descending
			select critter).ToList();
		if (list.Count <= 0)
		{
			list = (from critter in Main.GetAllType<CrittersActor>(5f)
				where (Object)(object)critter != (Object)null && critter.crittersActorType == type && Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) < 25f
				orderby Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) descending
				select critter).ToList();
		}
		if (list.Count <= 0)
		{
			list = (from critter in Main.GetAllType<CrittersActor>(5f)
				where (Object)(object)critter != (Object)null && critter.crittersActorType == type
				orderby Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) descending
				select critter).ToList();
		}
		CrittersActor val4 = list[Random.Range(0, list.Count)];
		if (Vector3.Distance(((Component)val4).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) > 25f)
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = ((Component)val4).transform.position - Vector3.one * 5f;
			if (CritterCoroutine != null)
			{
				((MonoBehaviour)CoroutineManager.instance).StopCoroutine(CritterCoroutine);
			}
			CritterCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(RopeEnableRig());
		}
		if (Vector3.Distance(((Component)val4).transform.position, Main.ServerPos) < 25f && Time.time > critterGrabDelay)
		{
			critterGrabDelay = Time.time + 0.1f;
			((Component)val4).transform.position = GorillaTagger.Instance.rightHandTransform.position;
			((Component)val4).transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
			if (Object.op_Implicit((Object)(object)val4))
			{
				val4.SetImpulseVelocity(val2, Vector3.zero);
			}
			if ((Object)(object)val3 != (Object)null)
			{
				((NetworkView)CrittersManager.instance).SendRPC("RemoteCrittersActorGrabbedby", ((CrittersManager)CrittersManager.instance).guard.currentOwner, new object[5]
				{
					val4.actorId,
					((CrittersActor)val3).actorId,
					Quaternion.identity,
					Vector3.zero,
					false
				});
			}
			((NetworkView)CrittersManager.instance).SendRPC("RemoteCritterActorReleased", ((CrittersManager)CrittersManager.instance).guard.currentOwner, new object[6]
			{
				val4.actorId,
				false,
				((Component)val4).transform.rotation,
				((Component)val4).transform.position,
				val2,
				Vector3.zero
			});
		}
	}

	public static void EffectGun(CritterEvent critterEvent)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Expected I4, but got Unknown
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, val2) = Main.RenderGun();
		if (!Main.GetGunInput(isShooting: true))
		{
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			CrittersPawn[] array = ((CrittersManager)CrittersManager.instance).crittersPawns.ToArray();
			if (array.Length != 0)
			{
				CrittersPawn val3 = array[0];
				((Component)val3).transform.position = val2.transform.position;
				int actorId = ((CrittersActor)val3).actorId;
				((CrittersManager)CrittersManager.instance).TriggerEvent(critterEvent, actorId, ((Component)val3).transform.position, Quaternion.LookRotation(((Component)val3).transform.up));
			}
			return;
		}
		CrittersActorType type = (CrittersActorType)17;
		Vector3 val4 = -((RaycastHit)(ref val)).normal * 50f;
		switch ((int)critterEvent)
		{
		case 0:
			type = (CrittersActorType)13;
			break;
		case 2:
		case 3:
			type = (CrittersActorType)17;
			break;
		case 1:
			type = (CrittersActorType)16;
			break;
		}
		CrittersGrabber val5 = (from grabber in Main.GetAllType<CrittersGrabber>(5f)
			where ((CrittersActor)grabber).rigPlayerId == PhotonNetwork.LocalPlayer.ActorNumber && grabber.isLeft
			select grabber).FirstOrDefault();
		List<CrittersActor> list = (from critter in Main.GetAllType<CrittersActor>(5f)
			where (Object)(object)critter != (Object)null && critter.crittersActorType == type && Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) < 25f && Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) > 3f
			orderby Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) descending
			select critter).ToList();
		if (list.Count <= 0)
		{
			list = (from critter in Main.GetAllType<CrittersActor>(5f)
				where (Object)(object)critter != (Object)null && critter.crittersActorType == type && Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) < 25f
				orderby Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) descending
				select critter).ToList();
		}
		if (list.Count <= 0)
		{
			list = (from critter in Main.GetAllType<CrittersActor>(5f)
				where (Object)(object)critter != (Object)null && critter.crittersActorType == type
				orderby Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) descending
				select critter).ToList();
		}
		CrittersActor val6 = list[Random.Range(0, list.Count)];
		if (Vector3.Distance(((Component)val6).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) > 25f)
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = ((Component)val6).transform.position - Vector3.one * 5f;
			if (CritterCoroutine != null)
			{
				((MonoBehaviour)CoroutineManager.instance).StopCoroutine(CritterCoroutine);
			}
			CritterCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(RopeEnableRig());
		}
		if (Vector3.Distance(((Component)val6).transform.position, Main.ServerPos) < 25f && Time.time > critterGrabDelay)
		{
			critterGrabDelay = Time.time + 0.05f;
			((Component)val6).transform.position = val2.transform.position + ((RaycastHit)(ref val)).normal;
			((Component)val6).transform.rotation = RandomUtilities.RandomQuaternion();
			if (Object.op_Implicit((Object)(object)val6))
			{
				val6.SetImpulseVelocity(val4, Vector3.zero);
			}
			if ((Object)(object)val5 != (Object)null)
			{
				((NetworkView)CrittersManager.instance).SendRPC("RemoteCrittersActorGrabbedby", ((CrittersManager)CrittersManager.instance).guard.currentOwner, new object[5]
				{
					val6.actorId,
					((CrittersActor)val5).actorId,
					Quaternion.identity,
					Vector3.zero,
					false
				});
			}
			((NetworkView)CrittersManager.instance).SendRPC("RemoteCritterActorReleased", ((CrittersManager)CrittersManager.instance).guard.currentOwner, new object[6]
			{
				val6.actorId,
				false,
				((Component)val6).transform.rotation,
				((Component)val6).transform.position,
				val4,
				Vector3.zero
			});
		}
	}

	public static void CritterSpam()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Invalid comparison between Unknown and I4
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.rightGrab)
		{
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			List<CrittersPawn> list = ((CrittersManager)CrittersManager.instance).crittersPawns.Where((CrittersPawn critter) => (Object)(object)critter != (Object)null).ToList();
			CrittersPawn val = list[Random.Range(0, list.Count)];
			((Component)val).transform.position = GorillaTagger.Instance.rightHandTransform.position;
			((Component)val).transform.rotation = RandomUtilities.RandomQuaternion();
			return;
		}
		CrittersGrabber val2 = (from grabber in Main.GetAllType<CrittersGrabber>(5f)
			where ((CrittersActor)grabber).rigPlayerId == PhotonNetwork.LocalPlayer.ActorNumber && grabber.isLeft
			select grabber).FirstOrDefault();
		List<CrittersPawn> list2 = ((CrittersManager)CrittersManager.instance).crittersPawns.Where((CrittersPawn critter) => (Object)(object)critter != (Object)null && Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) < 25f && Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) > 3f).ToList();
		if (list2.Count <= 0)
		{
			list2 = ((CrittersManager)CrittersManager.instance).crittersPawns.Where((CrittersPawn critter) => (Object)(object)critter != (Object)null && Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) < 25f).ToList();
		}
		if (list2.Count <= 0)
		{
			list2 = ((CrittersManager)CrittersManager.instance).crittersPawns.Where((CrittersPawn critter) => (Object)(object)critter != (Object)null).ToList();
		}
		if (list2.Count <= 0)
		{
			return;
		}
		CrittersPawn val3 = list2[Random.Range(0, list2.Count)];
		if (Vector3.Distance(((Component)val3).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) > 25f)
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = ((Component)val3).transform.position - Vector3.one * 5f;
			if (CritterCoroutine != null)
			{
				((MonoBehaviour)CoroutineManager.instance).StopCoroutine(CritterCoroutine);
			}
			CritterCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(RopeEnableRig());
		}
		if (Vector3.Distance(((Component)val3).transform.position, Main.ServerPos) < 25f && (int)val3.currentState != 4 && Time.time > critterGrabDelay)
		{
			critterGrabDelay = Time.time + 0.05f;
			((Component)val3).transform.position = GorillaTagger.Instance.rightHandTransform.position;
			((Component)val3).transform.rotation = RandomUtilities.RandomQuaternion();
			if ((Object)(object)val2 != (Object)null)
			{
				((NetworkView)CrittersManager.instance).SendRPC("RemoteCrittersActorGrabbedby", ((CrittersManager)CrittersManager.instance).guard.currentOwner, new object[5]
				{
					((CrittersActor)val3).actorId,
					((CrittersActor)val2).actorId,
					Quaternion.identity,
					Vector3.zero,
					false
				});
			}
			((NetworkView)CrittersManager.instance).SendRPC("RemoteCritterActorReleased", ((CrittersManager)CrittersManager.instance).guard.currentOwner, new object[6]
			{
				((CrittersActor)val3).actorId,
				false,
				((Component)val3).transform.rotation,
				((Component)val3).transform.position,
				Vector3.zero,
				Vector3.zero
			});
		}
	}

	public static void CritterMinigun()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.rightGrab)
		{
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			List<CrittersPawn> list = ((CrittersManager)CrittersManager.instance).crittersPawns.Where((CrittersPawn critter) => (Object)(object)critter != (Object)null).ToList();
			CrittersPawn val = list[Random.Range(0, list.Count)];
			((Component)val).transform.position = GorillaTagger.Instance.rightHandTransform.position;
			((Component)val).transform.rotation = RandomUtilities.RandomQuaternion();
			if (((CrittersActor)val).usesRB)
			{
				((CrittersActor)val).SetImpulseVelocity(Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * Main.ShootStrength, RandomUtilities.RandomVector3(100f));
			}
			return;
		}
		CrittersGrabber val2 = (from grabber in Main.GetAllType<CrittersGrabber>(5f)
			where ((CrittersActor)grabber).rigPlayerId == PhotonNetwork.LocalPlayer.ActorNumber && grabber.isLeft
			select grabber).FirstOrDefault();
		List<CrittersPawn> list2 = (from critter in ((CrittersManager)CrittersManager.instance).crittersPawns
			where (Object)(object)critter != (Object)null && Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) < 25f && Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) > 3f
			orderby Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) descending
			select critter).ToList();
		if (list2.Count <= 0)
		{
			list2 = (from critter in ((CrittersManager)CrittersManager.instance).crittersPawns
				where (Object)(object)critter != (Object)null && Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) < 25f
				orderby Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) descending
				select critter).ToList();
		}
		if (list2.Count <= 0)
		{
			list2 = (from critter in ((CrittersManager)CrittersManager.instance).crittersPawns
				where (Object)(object)critter != (Object)null
				orderby Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) descending
				select critter).ToList();
		}
		CrittersPawn val3 = list2[Random.Range(0, list2.Count)];
		if (Vector3.Distance(((Component)val3).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) > 25f)
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = ((Component)val3).transform.position - Vector3.one * 5f;
			if (CritterCoroutine != null)
			{
				((MonoBehaviour)CoroutineManager.instance).StopCoroutine(CritterCoroutine);
			}
			CritterCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(RopeEnableRig());
		}
		if (Vector3.Distance(((Component)val3).transform.position, Main.ServerPos) < 25f && Time.time > critterGrabDelay)
		{
			critterGrabDelay = Time.time + 0.05f;
			((Component)val3).transform.position = GorillaTagger.Instance.rightHandTransform.position;
			((Component)val3).transform.rotation = RandomUtilities.RandomQuaternion();
			if ((Object)(object)val2 != (Object)null)
			{
				((NetworkView)CrittersManager.instance).SendRPC("RemoteCrittersActorGrabbedby", ((CrittersManager)CrittersManager.instance).guard.currentOwner, new object[5]
				{
					((CrittersActor)val3).actorId,
					((CrittersActor)val2).actorId,
					Quaternion.identity,
					Vector3.zero,
					false
				});
			}
			((NetworkView)CrittersManager.instance).SendRPC("RemoteCritterActorReleased", ((CrittersManager)CrittersManager.instance).guard.currentOwner, new object[6]
			{
				((CrittersActor)val3).actorId,
				false,
				((Component)val3).transform.rotation,
				((Component)val3).transform.position,
				Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * Main.ShootStrength,
				Vector3.zero
			});
		}
	}

	public static void CritterGun()
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		GameObject item = Main.RenderGun().NewPointer;
		if (!Main.GetGunInput(isShooting: true))
		{
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			List<CrittersPawn> list = ((CrittersManager)CrittersManager.instance).crittersPawns.Where((CrittersPawn critter) => (Object)(object)critter != (Object)null).ToList();
			CrittersPawn val = list[Random.Range(0, list.Count)];
			((Component)val).transform.position = item.transform.position;
			((Component)val).transform.rotation = RandomUtilities.RandomQuaternion();
			return;
		}
		CrittersGrabber val2 = (from grabber in Main.GetAllType<CrittersGrabber>(5f)
			where ((CrittersActor)grabber).rigPlayerId == PhotonNetwork.LocalPlayer.ActorNumber && grabber.isLeft
			select grabber).FirstOrDefault();
		List<CrittersPawn> list2 = (from critter in ((CrittersManager)CrittersManager.instance).crittersPawns
			where (Object)(object)critter != (Object)null && Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) < 25f && Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) > 3f
			orderby Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) descending
			select critter).ToList();
		if (list2.Count <= 0)
		{
			list2 = (from critter in ((CrittersManager)CrittersManager.instance).crittersPawns
				where (Object)(object)critter != (Object)null && Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) < 25f
				orderby Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) descending
				select critter).ToList();
		}
		if (list2.Count <= 0)
		{
			list2 = (from critter in ((CrittersManager)CrittersManager.instance).crittersPawns
				where (Object)(object)critter != (Object)null
				orderby Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) descending
				select critter).ToList();
		}
		CrittersPawn val3 = list2[Random.Range(0, list2.Count)];
		if (Vector3.Distance(((Component)val3).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) > 25f)
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = ((Component)val3).transform.position - Vector3.one * 5f;
			if (CritterCoroutine != null)
			{
				((MonoBehaviour)CoroutineManager.instance).StopCoroutine(CritterCoroutine);
			}
			CritterCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(RopeEnableRig());
		}
		if (Vector3.Distance(((Component)val3).transform.position, Main.ServerPos) < 25f && Time.time > critterGrabDelay)
		{
			critterGrabDelay = Time.time + 0.05f;
			((Component)val3).transform.position = item.transform.position + Vector3.up;
			((Component)val3).transform.rotation = RandomUtilities.RandomQuaternion();
			if ((Object)(object)val2 != (Object)null)
			{
				((NetworkView)CrittersManager.instance).SendRPC("RemoteCrittersActorGrabbedby", ((CrittersManager)CrittersManager.instance).guard.currentOwner, new object[5]
				{
					((CrittersActor)val3).actorId,
					((CrittersActor)val2).actorId,
					Quaternion.identity,
					Vector3.zero,
					false
				});
			}
			((NetworkView)CrittersManager.instance).SendRPC("RemoteCritterActorReleased", ((CrittersManager)CrittersManager.instance).guard.currentOwner, new object[6]
			{
				((CrittersActor)val3).actorId,
				false,
				((Component)val3).transform.rotation,
				((Component)val3).transform.position,
				Vector3.zero,
				Vector3.zero
			});
		}
	}

	public static void ObjectSpam(CrittersActorType type)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Invalid comparison between Unknown and I4
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Invalid comparison between Unknown and I4
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.rightGrab)
		{
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			CrittersActor val = ((CrittersManager)CrittersManager.instance).SpawnActor(type, -1);
			val.MoveActor(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.rotation, false, true, true);
			if (val.usesRB)
			{
				val.SetImpulseVelocity(Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * Main.ShootStrength, RandomUtilities.RandomVector3(100f));
			}
			return;
		}
		Vector3 val2 = Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * Main.ShootStrength;
		CrittersActorType val3 = type;
		CrittersActorType val4 = val3;
		if ((int)val4 != 2)
		{
			if ((int)val4 == 18)
			{
				type = (CrittersActorType)17;
				val2 = Vector3.down * 50f;
			}
		}
		else
		{
			type = (CrittersActorType)16;
			val2 = Vector3.down * 50f;
		}
		CrittersGrabber val5 = (from grabber in Main.GetAllType<CrittersGrabber>(5f)
			where ((CrittersActor)grabber).rigPlayerId == PhotonNetwork.LocalPlayer.ActorNumber && grabber.isLeft
			select grabber).FirstOrDefault();
		List<CrittersActor> list = (from critter in Main.GetAllType<CrittersActor>(5f)
			where (Object)(object)critter != (Object)null && critter.crittersActorType == type && Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) < 25f && Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) > 3f
			orderby Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) descending
			select critter).ToList();
		if (list.Count <= 0)
		{
			list = (from critter in Main.GetAllType<CrittersActor>(5f)
				where (Object)(object)critter != (Object)null && critter.crittersActorType == type && Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) < 25f
				orderby Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) descending
				select critter).ToList();
		}
		if (list.Count <= 0)
		{
			list = (from critter in Main.GetAllType<CrittersActor>(5f)
				where (Object)(object)critter != (Object)null && critter.crittersActorType == type
				orderby Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) descending
				select critter).ToList();
		}
		CrittersActor val6 = list[Random.Range(0, list.Count)];
		if (Vector3.Distance(((Component)val6).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) > 25f)
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = ((Component)val6).transform.position - Vector3.one * 5f;
			if (CritterCoroutine != null)
			{
				((MonoBehaviour)CoroutineManager.instance).StopCoroutine(CritterCoroutine);
			}
			CritterCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(RopeEnableRig());
		}
		if (Vector3.Distance(((Component)val6).transform.position, Main.ServerPos) < 25f && Time.time > critterGrabDelay)
		{
			critterGrabDelay = Time.time + 0.05f;
			((Component)val6).transform.position = GorillaTagger.Instance.rightHandTransform.position;
			((Component)val6).transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
			if (Object.op_Implicit((Object)(object)val6))
			{
				val6.SetImpulseVelocity(val2, Vector3.zero);
			}
			if ((Object)(object)val5 != (Object)null)
			{
				((NetworkView)CrittersManager.instance).SendRPC("RemoteCrittersActorGrabbedby", ((CrittersManager)CrittersManager.instance).guard.currentOwner, new object[5]
				{
					val6.actorId,
					((CrittersActor)val5).actorId,
					Quaternion.identity,
					Vector3.zero,
					false
				});
			}
			((NetworkView)CrittersManager.instance).SendRPC("RemoteCritterActorReleased", ((CrittersManager)CrittersManager.instance).guard.currentOwner, new object[6]
			{
				val6.actorId,
				false,
				((Component)val6).transform.rotation,
				((Component)val6).transform.position,
				val2,
				Vector3.zero
			});
		}
	}

	public static void ObjectGun(CrittersActorType type)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Invalid comparison between Unknown and I4
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Invalid comparison between Unknown and I4
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, val2) = Main.RenderGun();
		if (!Main.GetGunInput(isShooting: true))
		{
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			CrittersActor val3 = ((CrittersManager)CrittersManager.instance).SpawnActor(type, -1);
			val3.MoveActor(val2.transform.position + Vector3.up, RandomUtilities.RandomQuaternion(), false, true, true);
			return;
		}
		Vector3 val4 = Vector3.zero;
		Vector3 position = val2.transform.position + Vector3.up;
		CrittersActorType val5 = type;
		CrittersActorType val6 = val5;
		if ((int)val6 != 2)
		{
			if ((int)val6 == 18)
			{
				type = (CrittersActorType)17;
				val4 = ((RaycastHit)(ref val)).normal * -20f;
				position = val2.transform.position + ((RaycastHit)(ref val)).normal;
			}
		}
		else
		{
			type = (CrittersActorType)16;
			val4 = ((RaycastHit)(ref val)).normal * -20f;
			position = val2.transform.position + ((RaycastHit)(ref val)).normal;
		}
		CrittersGrabber val7 = (from grabber in Main.GetAllType<CrittersGrabber>(5f)
			where ((CrittersActor)grabber).rigPlayerId == PhotonNetwork.LocalPlayer.ActorNumber && grabber.isLeft
			select grabber).FirstOrDefault();
		List<CrittersActor> list = (from critter in Main.GetAllType<CrittersActor>(5f)
			where (Object)(object)critter != (Object)null && critter.crittersActorType == type && Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) < 25f && Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) > 3f
			orderby Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) descending
			select critter).ToList();
		if (list.Count <= 0)
		{
			list = (from critter in Main.GetAllType<CrittersActor>(5f)
				where (Object)(object)critter != (Object)null && critter.crittersActorType == type && Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) < 25f
				orderby Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) descending
				select critter).ToList();
		}
		if (list.Count <= 0)
		{
			list = (from critter in Main.GetAllType<CrittersActor>(5f)
				where (Object)(object)critter != (Object)null && critter.crittersActorType == type
				orderby Vector3.Distance(((Component)critter).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) descending
				select critter).ToList();
		}
		CrittersActor val8 = list[Random.Range(0, list.Count)];
		if (Vector3.Distance(((Component)val8).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) > 25f)
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = ((Component)val8).transform.position - Vector3.one * 5f;
			if (CritterCoroutine != null)
			{
				((MonoBehaviour)CoroutineManager.instance).StopCoroutine(CritterCoroutine);
			}
			CritterCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(RopeEnableRig());
		}
		if (Vector3.Distance(((Component)val8).transform.position, Main.ServerPos) < 25f && Time.time > critterGrabDelay)
		{
			critterGrabDelay = Time.time + 0.1f;
			((Component)val8).transform.position = position;
			((Component)val8).transform.rotation = RandomUtilities.RandomQuaternion();
			if (Object.op_Implicit((Object)(object)val8))
			{
				val8.SetImpulseVelocity(val4, Vector3.zero);
			}
			if ((Object)(object)val7 != (Object)null)
			{
				((NetworkView)CrittersManager.instance).SendRPC("RemoteCrittersActorGrabbedby", ((CrittersManager)CrittersManager.instance).guard.currentOwner, new object[5]
				{
					val8.actorId,
					((CrittersActor)val7).actorId,
					Quaternion.identity,
					Vector3.zero,
					false
				});
			}
			((NetworkView)CrittersManager.instance).SendRPC("RemoteCritterActorReleased", ((CrittersManager)CrittersManager.instance).guard.currentOwner, new object[6]
			{
				val8.actorId,
				false,
				((Component)val8).transform.rotation,
				((Component)val8).transform.position,
				val4,
				Vector3.zero
			});
		}
	}

	public static void CreateBlackHole()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(-63.2589f, 9.4352f, -65.2775f);
		GameObject val2 = new GameObject("BlackHoleEffect");
		val2.transform.position = val;
		ParticleSystem val3 = val2.AddComponent<ParticleSystem>();
		MainModule main = val3.main;
		((MainModule)(ref main)).startColor = new MinMaxGradient(new Color(0f, 0f, 0f), new Color(0.1f, 0.1f, 0.1f));
		((MainModule)(ref main)).startSize = MinMaxCurve.op_Implicit(0.4f);
		((MainModule)(ref main)).startSpeed = MinMaxCurve.op_Implicit(0.5f);
		((MainModule)(ref main)).startLifetime = MinMaxCurve.op_Implicit(2f);
		((MainModule)(ref main)).loop = true;
		((MainModule)(ref main)).simulationSpace = (ParticleSystemSimulationSpace)1;
		((MainModule)(ref main)).maxParticles = 150;
		ParticleSystemRenderer component = ((Component)val3).GetComponent<ParticleSystemRenderer>();
		((Renderer)component).material = new Material(Shader.Find("Particles/Standard Unlit"));
		EmissionModule emission = val3.emission;
		((EmissionModule)(ref emission)).rateOverTime = MinMaxCurve.op_Implicit(30f);
		ShapeModule shape = val3.shape;
		((ShapeModule)(ref shape)).shapeType = (ParticleSystemShapeType)0;
		((ShapeModule)(ref shape)).radius = 2.5f;
		((ShapeModule)(ref shape)).randomDirectionAmount = 0.1f;
		RotationOverLifetimeModule rotationOverLifetime = val3.rotationOverLifetime;
		((RotationOverLifetimeModule)(ref rotationOverLifetime)).enabled = true;
		((RotationOverLifetimeModule)(ref rotationOverLifetime)).z = new MinMaxCurve(0.5f, 1f);
		VelocityOverLifetimeModule velocityOverLifetime = val3.velocityOverLifetime;
		((VelocityOverLifetimeModule)(ref velocityOverLifetime)).enabled = true;
		((VelocityOverLifetimeModule)(ref velocityOverLifetime)).x = new MinMaxCurve(0f, 0f);
		((VelocityOverLifetimeModule)(ref velocityOverLifetime)).y = new MinMaxCurve(0f, 0f);
		((VelocityOverLifetimeModule)(ref velocityOverLifetime)).z = new MinMaxCurve(-1f, -2f);
		val3.Play();
		Object.Destroy((Object)(object)((Component)val3).gameObject, 2f);
		Rigidbody attachedRigidbody = ((Collider)GTPlayer.Instance.bodyCollider).attachedRigidbody;
		if ((Object)(object)attachedRigidbody != (Object)null)
		{
			Vector3 val4 = val - ((Component)GTPlayer.Instance.bodyCollider).transform.position;
			float magnitude = ((Vector3)(ref val4)).magnitude;
			float num = Mathf.Clamp(1000f / magnitude, 0f, 10f);
			attachedRigidbody.AddForce(((Vector3)(ref val4)).normalized * num * Time.deltaTime, (ForceMode)0);
		}
		Collider[] array = Physics.OverlapSphere(val, 10f);
		Collider[] array2 = array;
		foreach (Collider val5 in array2)
		{
			Rigidbody component2 = ((Component)val5).GetComponent<Rigidbody>();
			if ((Object)(object)component2 != (Object)null)
			{
				Vector3 val6 = val - ((Component)val5).transform.position;
				float magnitude2 = ((Vector3)(ref val6)).magnitude;
				float num2 = Mathf.Clamp(1000f / magnitude2, 0f, 10f);
				component2.AddForce(((Vector3)(ref val6)).normalized * num2 * Time.deltaTime, (ForceMode)0);
			}
		}
	}

	public static void BloodRain()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 25; i++)
		{
			GameObject val = GameObject.CreatePrimitive((PrimitiveType)0);
			val.transform.position = ((Component)GorillaTagger.Instance).transform.position + new Vector3((float)Random.Range(-5, 5), 10f, (float)Random.Range(-5, 5));
			val.transform.localScale = Vector3.one * 0.1f;
			val.GetComponent<Renderer>().material.color = new Color(0.6f, 0f, 0f);
			Rigidbody val2 = val.AddComponent<Rigidbody>();
			val2.useGravity = true;
			Object.Destroy((Object)(object)val, 4f);
		}
	}

	public static void CreateWhiteHole()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(-63.2589f, 9.4352f, -65.2775f);
		GameObject val2 = new GameObject("WhiteHoleEffect");
		val2.transform.position = val;
		ParticleSystem val3 = val2.AddComponent<ParticleSystem>();
		MainModule main = val3.main;
		((MainModule)(ref main)).startColor = new MinMaxGradient(new Color(1f, 1f, 1f), new Color(0.8f, 0.8f, 1f));
		((MainModule)(ref main)).startSize = MinMaxCurve.op_Implicit(0.4f);
		((MainModule)(ref main)).startSpeed = MinMaxCurve.op_Implicit(0.5f);
		((MainModule)(ref main)).startLifetime = MinMaxCurve.op_Implicit(2f);
		((MainModule)(ref main)).loop = true;
		((MainModule)(ref main)).simulationSpace = (ParticleSystemSimulationSpace)1;
		((MainModule)(ref main)).maxParticles = 150;
		ParticleSystemRenderer component = ((Component)val3).GetComponent<ParticleSystemRenderer>();
		((Renderer)component).material = new Material(Shader.Find("Particles/Standard Unlit"));
		EmissionModule emission = val3.emission;
		((EmissionModule)(ref emission)).rateOverTime = MinMaxCurve.op_Implicit(30f);
		ShapeModule shape = val3.shape;
		((ShapeModule)(ref shape)).shapeType = (ParticleSystemShapeType)0;
		((ShapeModule)(ref shape)).radius = 2.5f;
		((ShapeModule)(ref shape)).randomDirectionAmount = 0.1f;
		RotationOverLifetimeModule rotationOverLifetime = val3.rotationOverLifetime;
		((RotationOverLifetimeModule)(ref rotationOverLifetime)).enabled = true;
		((RotationOverLifetimeModule)(ref rotationOverLifetime)).z = new MinMaxCurve(0.5f, 1f);
		VelocityOverLifetimeModule velocityOverLifetime = val3.velocityOverLifetime;
		((VelocityOverLifetimeModule)(ref velocityOverLifetime)).enabled = true;
		((VelocityOverLifetimeModule)(ref velocityOverLifetime)).x = new MinMaxCurve(0f, 0f);
		((VelocityOverLifetimeModule)(ref velocityOverLifetime)).y = new MinMaxCurve(0f, 0f);
		((VelocityOverLifetimeModule)(ref velocityOverLifetime)).z = new MinMaxCurve(1f, 2f);
		val3.Play();
		Object.Destroy((Object)(object)((Component)val3).gameObject, 2f);
		Rigidbody attachedRigidbody = ((Collider)GTPlayer.Instance.bodyCollider).attachedRigidbody;
		if ((Object)(object)attachedRigidbody != (Object)null)
		{
			Vector3 val4 = ((Component)GTPlayer.Instance.bodyCollider).transform.position - val;
			float magnitude = ((Vector3)(ref val4)).magnitude;
			float num = Mathf.Clamp(1000f / magnitude, 0f, 10f);
			attachedRigidbody.AddForce(((Vector3)(ref val4)).normalized * num * Time.deltaTime, (ForceMode)0);
		}
		Collider[] array = Physics.OverlapSphere(val, 10f);
		Collider[] array2 = array;
		foreach (Collider val5 in array2)
		{
			Rigidbody component2 = ((Component)val5).GetComponent<Rigidbody>();
			if ((Object)(object)component2 != (Object)null)
			{
				Vector3 val6 = ((Component)val5).transform.position - val;
				float magnitude2 = ((Vector3)(ref val6)).magnitude;
				float num2 = Mathf.Clamp(1000f / magnitude2, 0f, 10f);
				component2.AddForce(((Vector3)(ref val6)).normalized * num2 * Time.deltaTime, (ForceMode)0);
			}
		}
	}

	public static void GreenRain()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 25; i++)
		{
			GameObject val = GameObject.CreatePrimitive((PrimitiveType)0);
			val.transform.position = ((Component)GorillaTagger.Instance).transform.position + new Vector3((float)Random.Range(-5, 5), 10f, (float)Random.Range(-5, 5));
			val.transform.localScale = Vector3.one * 0.1f;
			val.GetComponent<Renderer>().material.color = new Color(0f, 0.6f, 0f);
			Rigidbody val2 = val.AddComponent<Rigidbody>();
			val2.useGravity = true;
			Object.Destroy((Object)(object)val, 4f);
		}
	}

	public static void blueRain()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 25; i++)
		{
			GameObject val = GameObject.CreatePrimitive((PrimitiveType)0);
			val.transform.position = ((Component)GorillaTagger.Instance).transform.position + new Vector3((float)Random.Range(-5, 5), 10f, (float)Random.Range(-5, 5));
			val.transform.localScale = Vector3.one * 0.1f;
			val.GetComponent<Renderer>().material.color = new Color(0f, 0f, 0.6f);
			Rigidbody val2 = val.AddComponent<Rigidbody>();
			val2.useGravity = true;
			Object.Destroy((Object)(object)val, 4f);
		}
	}

	public static void pinkRain()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 25; i++)
		{
			GameObject val = GameObject.CreatePrimitive((PrimitiveType)0);
			val.transform.position = ((Component)GorillaTagger.Instance).transform.position + new Vector3((float)Random.Range(-5, 5), 10f, (float)Random.Range(-5, 5));
			val.transform.localScale = Vector3.one * 0.1f;
			val.GetComponent<Renderer>().material.color = new Color(0.6f, 0f, 0.6f);
			Rigidbody val2 = val.AddComponent<Rigidbody>();
			val2.useGravity = true;
			Object.Destroy((Object)(object)val, 4f);
		}
	}

	public static void PurpleRain()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 25; i++)
		{
			GameObject val = GameObject.CreatePrimitive((PrimitiveType)0);
			val.transform.position = ((Component)GorillaTagger.Instance).transform.position + new Vector3((float)Random.Range(-5, 5), 10f, (float)Random.Range(-5, 5));
			val.transform.localScale = Vector3.one * 0.1f;
			val.GetComponent<Renderer>().material.color = new Color(0.5f, 0f, 0.6f);
			Rigidbody val2 = val.AddComponent<Rigidbody>();
			val2.useGravity = true;
			Object.Destroy((Object)(object)val, 4f);
		}
	}

	public static void CyanRain()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 25; i++)
		{
			GameObject val = GameObject.CreatePrimitive((PrimitiveType)0);
			val.transform.position = ((Component)GorillaTagger.Instance).transform.position + new Vector3((float)Random.Range(-5, 5), 10f, (float)Random.Range(-5, 5));
			val.transform.localScale = Vector3.one * 0.1f;
			val.GetComponent<Renderer>().material.color = new Color(0f, 0.6f, 0.6f);
			Rigidbody val2 = val.AddComponent<Rigidbody>();
			val2.useGravity = true;
			Object.Destroy((Object)(object)val, 4f);
		}
	}

	public static void YellowRain()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 25; i++)
		{
			GameObject val = GameObject.CreatePrimitive((PrimitiveType)0);
			val.transform.position = ((Component)GorillaTagger.Instance).transform.position + new Vector3((float)Random.Range(-5, 5), 10f, (float)Random.Range(-5, 5));
			val.transform.localScale = Vector3.one * 0.1f;
			val.GetComponent<Renderer>().material.color = new Color(0.6f, 0.6f, 0f);
			Rigidbody val2 = val.AddComponent<Rigidbody>();
			val2.useGravity = true;
			Object.Destroy((Object)(object)val, 4f);
		}
	}

	private static void FireParticle(Vector3 position)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("FireEffect");
		val.transform.position = position;
		ParticleSystem val2 = val.AddComponent<ParticleSystem>();
		MainModule main = val2.main;
		((MainModule)(ref main)).startColor = new MinMaxGradient(Color.red, Color.black);
		((MainModule)(ref main)).startSize = MinMaxCurve.op_Implicit(0.05f);
		((MainModule)(ref main)).startSpeed = MinMaxCurve.op_Implicit(0.25f);
		((MainModule)(ref main)).startLifetime = MinMaxCurve.op_Implicit(1.5f);
		((MainModule)(ref main)).loop = true;
		((MainModule)(ref main)).simulationSpace = (ParticleSystemSimulationSpace)1;
		((MainModule)(ref main)).maxParticles = 30;
		ParticleSystemRenderer component = ((Component)val2).GetComponent<ParticleSystemRenderer>();
		((Renderer)component).material = new Material(Shader.Find("Particles/Standard Unlit"));
		((Renderer)component).material.SetColor("_Color", Color.red);
		EmissionModule emission = val2.emission;
		((EmissionModule)(ref emission)).rateOverTime = MinMaxCurve.op_Implicit(5f);
		ShapeModule shape = val2.shape;
		((ShapeModule)(ref shape)).shapeType = (ParticleSystemShapeType)4;
		((ShapeModule)(ref shape)).angle = 20f;
		((ShapeModule)(ref shape)).radius = 0.1f;
		Object.Destroy((Object)(object)val, 0.5f);
	}

	public static void fireHands()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		FireParticle(GorillaTagger.Instance.leftHandTransform.position);
		FireParticle(GorillaTagger.Instance.rightHandTransform.position);
	}

	public static void ParticleGun()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.GetGunInput(isShooting: true))
			{
				FireParticle(((RaycastHit)(ref val)).point);
			}
		}
	}

	public static void BarrelGun()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			(RaycastHit, GameObject) tuple = Main.RenderGun();
			if (Main.GetGunInput(isShooting: true))
			{
				SendBarrelProjectile(tuple.Item2.transform.position + Vector3.up, Vector3.zero, Quaternion.identity);
			}
		}
	}

	public static void TransparentRig()
	{
		if (!didWarnForTransparentRig)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>WARNING</color><color=grey>]</color> This mod can not make yourself visible again for other players, even when turned off. You must rejoin the room with the mod off.", 15000);
			didWarnForTransparentRig = true;
		}
		SetTransparent(status: true);
	}

	public static void SetTransparent(bool status, VRRig receiver = null)
	{
		((NetworkView)ManagerRegistry.GhostReactor.GhostReactorManager).GetView.RPC("PlayerStateChangeRPC", ((Object)(object)receiver != (Object)null) ? receiver.GetPhotonPlayer() : ((object)(RpcTarget)0), NetworkSystem.Instance.LocalPlayer.ActorNumber, NetworkSystem.Instance.LocalPlayer.ActorNumber, (object)(GRPlayerState)1);
		VRRig.LocalRig.bodyRenderer.SetGameModeBodyType((GorillaBodyType)(status ? (-1) : 0));
		VRRig.LocalRig.SetInvisibleToLocalPlayer(status);
	}

	public static void VIMKickGun()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (!VRRig.LocalRig.IsVIMSubscriber())
		{
			Main.PromptSingle("You are not a VIM subscriber, so this mod will not function.");
			Buttons.GetIndex("VIM Kick Gun").SetEnabled(value: false);
		}
		else
		{
			if (!Main.GetGunInput(isShooting: false))
			{
				return;
			}
			var (val, _) = Main.RenderGun();
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					RoomControls.KickPlayer(componentInParent.GetPlayer().ActorNumber);
				}
			}
		}
	}

	public static void VIMKickAll()
	{
		if (!VRRig.LocalRig.IsVIMSubscriber())
		{
			Main.PromptSingle("You are not a VIM subscriber, so this mod will not function.");
			return;
		}
		LinqUtils.ForEach<NetPlayer>((IEnumerable<NetPlayer>)NetworkSystem.Instance.PlayerListOthers, (Action<NetPlayer>)delegate(NetPlayer p)
		{
			RoomControls.KickPlayer(p.ActorNumber);
		});
	}

	public static void VIMBlockGun()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (!VRRig.LocalRig.IsVIMSubscriber())
		{
			Main.PromptSingle("You are not a VIM subscriber, so this mod will not function.");
			Buttons.GetIndex("VIM Block Gun").SetEnabled(value: false);
		}
		else
		{
			if (!Main.GetGunInput(isShooting: false))
			{
				return;
			}
			var (val, _) = Main.RenderGun();
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					RoomControls.KickAndBlockPlayer(componentInParent.GetPlayer().ActorNumber, -1);
				}
			}
		}
	}

	public static void VIMBlockAll()
	{
		if (!VRRig.LocalRig.IsVIMSubscriber())
		{
			Main.PromptSingle("You are not a VIM subscriber, so this mod will not function.");
			return;
		}
		LinqUtils.ForEach<NetPlayer>((IEnumerable<NetPlayer>)NetworkSystem.Instance.PlayerListOthers, (Action<NetPlayer>)delegate(NetPlayer p)
		{
			RoomControls.KickAndBlockPlayer(p.ActorNumber, -1);
		});
	}

	public static void VIMMuteGun()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (!VRRig.LocalRig.IsVIMSubscriber())
		{
			Main.PromptSingle("You are not a VIM subscriber, so this mod will not function.");
			Buttons.GetIndex("VIM Mute Gun").SetEnabled(value: false);
		}
		else if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && !RoomControls.MutedPlayers.ContainsKey(Main.lockTarget.GetPlayer().UserId))
			{
				RoomControls.MutePlayer(Main.lockTarget.GetPlayer().ActorNumber, -1);
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal() && !componentInParent.IsTagged() && PhotonNetwork.IsMasterClient)
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
		}
	}

	public static void VIMMuteAll()
	{
		if (!VRRig.LocalRig.IsVIMSubscriber())
		{
			Main.PromptSingle("You are not a VIM subscriber, so this mod will not function.");
			return;
		}
		LinqUtils.ForEach<NetPlayer>((IEnumerable<NetPlayer>)NetworkSystem.Instance.PlayerListOthers, (Action<NetPlayer>)delegate(NetPlayer p)
		{
			RoomControls.MutePlayer(p.ActorNumber, -1);
		});
	}

	public static void VIMUnmuteAll()
	{
		LinqUtils.ForEach<NetPlayer>((IEnumerable<NetPlayer>)NetworkSystem.Instance.PlayerListOthers, (Action<NetPlayer>)delegate(NetPlayer p)
		{
			RoomControls.UnmutePlayer(p.UserId);
		});
	}

	public static void GrabGun()
	{
		GuardianGrabGun();
	}

	public static void GrabAll()
	{
		GuardianGrabAll();
	}

	public static void ReleaseGun()
	{
		GuardianReleaseGun();
	}

	public static void ReleaseAll()
	{
		GuardianReleaseAll();
	}

	public static void BringGun()
	{
		GuardianBringGun();
	}

	public static void BringAll()
	{
		GuardianBringAll();
	}

	public static void BringAllGun()
	{
		GuardianBringAllGun();
	}

	public static void BringAwayGun()
	{
		GuardianBringAwayGun();
	}

	public static void BringAwayAll()
	{
		GuardianBringAwayAll();
	}

	public static void BringAwayAllGun()
	{
		GuardianBringAwayAllGun();
	}

	public static void AntiStump()
	{
		GuardianAntiStump();
	}

	public static void OrbitAll()
	{
		GuardianOrbitAll();
	}

	public static void PunchMod()
	{
		GuardianPunchMod();
	}

	public static void Boxing()
	{
		GuardianBoxing();
	}

	public static void GiveFlyGun()
	{
		GuardianGiveFlyGun();
	}

	public static void GiveFlyAll()
	{
		GuardianGiveFlyAll();
	}

	public static void SpazPlayerGun()
	{
		GuardianSpazPlayerGun();
	}

	public static void SpazAllPlayers()
	{
		GuardianSpazAllPlayers();
	}

	public static void EffectSpamHands()
	{
		GuardianEffectSpamHands();
	}

	public static void EffectSpamGun()
	{
		GuardianEffectSpamGun();
	}

	public static void PhysicalFreezeGun()
	{
		GuardianPhysicalFreezeGun();
	}

	public static void PhysicalFreezeAll()
	{
		GuardianPhysicalFreezeAll();
	}

	private static void NotifyUnavailableEffect(string effectName)
	{
		NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> " + effectName + " is unavailable in this build.");
	}
}
