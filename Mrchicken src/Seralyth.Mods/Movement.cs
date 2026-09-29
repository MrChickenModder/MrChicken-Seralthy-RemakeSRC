using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Swimming;
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
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;
using Valve.Newtonsoft.Json.Linq;

namespace Seralyth.Mods;

public static class Movement
{
	public struct PlayerPosition
	{
		public Vector3 position;

		public Vector3 velocity;

		public (Vector3 position, Quaternion rotation) leftHand;

		public (Vector3 position, Quaternion rotation) rightHand;

		public bool leftGrip;

		public bool rightGrip;

		public bool leftTrigger;

		public bool rightTrigger;

		public readonly void MoveTo()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			Main.TeleportPlayer(position);
			GorillaTagger.Instance.rigidbody.linearVelocity = velocity;
			GorillaTagger.Instance.leftHandTransform.position = leftHand.position;
			GorillaTagger.Instance.leftHandTransform.rotation = leftHand.rotation;
			GorillaTagger.Instance.rightHandTransform.position = rightHand.position;
			GorillaTagger.Instance.rightHandTransform.rotation = rightHand.rotation;
			((ControllerInputPoller)ControllerInputPoller.instance).leftControllerGripFloat = (leftGrip ? 1f : 0f);
			((ControllerInputPoller)ControllerInputPoller.instance).rightControllerGripFloat = (rightGrip ? 1f : 0f);
			((ControllerInputPoller)ControllerInputPoller.instance).leftControllerIndexFloat = (leftTrigger ? 1f : 0f);
			((ControllerInputPoller)ControllerInputPoller.instance).rightControllerIndexFloat = (rightTrigger ? 1f : 0f);
		}

		public static PlayerPosition CurrentPosition()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			return new PlayerPosition
			{
				position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position,
				velocity = GorillaTagger.Instance.rigidbody.linearVelocity,
				leftHand = (position: GorillaTagger.Instance.leftHandTransform.position, rotation: GorillaTagger.Instance.leftHandTransform.rotation),
				rightHand = (position: GorillaTagger.Instance.rightHandTransform.position, rotation: GorillaTagger.Instance.rightHandTransform.rotation),
				leftGrip = Main.leftGrab,
				rightGrip = Main.rightGrab,
				leftTrigger = false,
				rightTrigger = (Main.rightTrigger > 0.5f)
			};
		}

		public readonly JObject ToJObject()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Expected O, but got Unknown
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Expected O, but got Unknown
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Expected O, but got Unknown
			return new JObject
			{
				["position"] = (JToken)(object)Vec3ToJObject(position),
				["velocity"] = (JToken)(object)Vec3ToJObject(velocity),
				["leftHand"] = (JToken)new JObject
				{
					["position"] = (JToken)(object)Vec3ToJObject(leftHand.position),
					["rotation"] = (JToken)(object)QuatToJObject(leftHand.rotation)
				},
				["rightHand"] = (JToken)new JObject
				{
					["position"] = (JToken)(object)Vec3ToJObject(rightHand.position),
					["rotation"] = (JToken)(object)QuatToJObject(rightHand.rotation)
				},
				["leftGrip"] = JToken.op_Implicit(leftGrip),
				["rightGrip"] = JToken.op_Implicit(rightGrip),
				["leftTrigger"] = JToken.op_Implicit(leftTrigger),
				["rightTrigger"] = JToken.op_Implicit(rightTrigger)
			};
		}

		public static PlayerPosition FromJObject(JObject obj)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Expected O, but got Unknown
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Expected O, but got Unknown
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Expected O, but got Unknown
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Expected O, but got Unknown
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Expected O, but got Unknown
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			PlayerPosition result = new PlayerPosition
			{
				position = JObjectToVec3((JObject)obj["position"]),
				velocity = JObjectToVec3((JObject)obj["velocity"]),
				leftHand = (position: JObjectToVec3((JObject)obj["leftHand"][(object)"position"]), rotation: JObjectToQuat((JObject)obj["leftHand"][(object)"rotation"])),
				rightHand = (position: JObjectToVec3((JObject)obj["rightHand"][(object)"position"]), rotation: JObjectToQuat((JObject)obj["rightHand"][(object)"rotation"]))
			};
			if (obj["leftGrip"] != null)
			{
				result.leftGrip = (bool)obj["leftGrip"];
			}
			if (obj["rightGrip"] != null)
			{
				result.rightGrip = (bool)obj["rightGrip"];
			}
			if (obj["leftTrigger"] != null)
			{
				result.leftTrigger = (bool)obj["leftTrigger"];
			}
			if (obj["rightTrigger"] != null)
			{
				result.rightTrigger = (bool)obj["rightTrigger"];
			}
			return result;
		}

		private static JObject Vec3ToJObject(Vector3 v)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Expected O, but got Unknown
			return new JObject
			{
				["x"] = JToken.op_Implicit(v.x),
				["y"] = JToken.op_Implicit(v.y),
				["z"] = JToken.op_Implicit(v.z)
			};
		}

		private static Vector3 JObjectToVec3(JObject obj)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3((float)obj["x"], (float)obj["y"], (float)obj["z"]);
		}

		private static JObject QuatToJObject(Quaternion q)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Expected O, but got Unknown
			return new JObject
			{
				["x"] = JToken.op_Implicit(q.x),
				["y"] = JToken.op_Implicit(q.y),
				["z"] = JToken.op_Implicit(q.z),
				["w"] = JToken.op_Implicit(q.w)
			};
		}

		private static Quaternion JObjectToQuat(JObject obj)
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			return new Quaternion((float)obj["x"], (float)obj["y"], (float)obj["z"], (float)obj["w"]);
		}
	}

	public class Macro
	{
		public List<PlayerPosition> positions;

		public float macroStepDuration;

		public string name;

		public bool enabled;

		public string DumpJSON()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Expected O, but got Unknown
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Expected O, but got Unknown
			JObject val = new JObject
			{
				["name"] = JToken.op_Implicit(name),
				["enabled"] = JToken.op_Implicit(enabled),
				["positions"] = (JToken)new JArray((object)positions.ConvertAll((PlayerPosition p) => p.ToJObject())),
				["step-time"] = JToken.op_Implicit(macroStepDuration)
			};
			return ((object)val).ToString();
		}

		public static Macro LoadJSON(string json)
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Expected O, but got Unknown
			JObject val = JObject.Parse(json);
			Macro macro = new Macro
			{
				name = (string)val["name"],
				enabled = (bool)val["enabled"],
				positions = new List<PlayerPosition>()
			};
			foreach (JToken item in (JArray)val["positions"])
			{
				macro.positions.Add(PlayerPosition.FromJObject((JObject)item));
			}
			JToken val2 = default(JToken);
			macro.macroStepDuration = (val.TryGetValue("step-time", ref val2) ? ((float)val2) : 0.1f);
			return macro;
		}
	}

	private struct FlipState
	{
		public bool active;

		public float start;

		public float dir;

		public Quaternion rot;

		public Quaternion headRot;

		public Vector3 axis;
	}

	[Serializable]
	private class SavedTeleportData
	{
		public List<SavedTeleportEntry> spots = new List<SavedTeleportEntry>();
	}

	[Serializable]
	private class SavedTeleportEntry
	{
		public string name;

		public float x;

		public float y;

		public float z;
	}

	public static int platformMode;

	public static int platformShape;

	public static int flySpeedCycle = 1;

	public static float _flySpeed = 10f;

	public static int speedboostCycle = 1;

	public static float jspeed = 7.5f;

	public static float jmulti = 1.1f;

	public static int longarmCycle = 2;

	public static float armlength = 1.25f;

	public static GameObject leftplat;

	public static GameObject rightplat;

	private static readonly Dictionary<bool, List<GameObject>> frozonicPlatforms = new Dictionary<bool, List<GameObject>>();

	private static readonly Dictionary<bool, int> platformIndex = new Dictionary<bool, int>();

	public static int playspaceAbuseIndex;

	public static bool noclip;

	public static float startX = -1f;

	public static float startY = -1f;

	public static float subThingy;

	public static float subThingyZ;

	public static Vector3 lastPosition = Vector3.zero;

	private static float driveSpeed;

	public static int driveInt;

	public static int fastRopesInt;

	public static Vector2 driveLerpDirection = Vector2.zero;

	private static bool previousDash;

	private static readonly float revCooldown = 0.5f;

	private static float nextrevTime = 0f;

	private static float flapTime;

	private static float loaoalsode;

	public static Vector3 rightgrapplePoint;

	public static Vector3 leftgrapplePoint;

	public static SpringJoint rightjoint;

	public static SpringJoint leftjoint;

	public static bool isLeftGrappling;

	public static bool isRightGrappling;

	public static GameObject portalGun;

	public static GameObject bluePortal;

	public static GameObject orangePortal;

	public static GameObject crosshair;

	public static bool playedOpen;

	public static bool flipped;

	public static bool inPortal;

	public static float portalDelay;

	public static float flipDelay;

	private static List<Vector3> posArchive;

	public static Vector3 leftPos = Vector3.zero;

	public static Vector3 rightPos = Vector3.zero;

	public static bool lastOnBranch;

	private static readonly List<object[]> playerPositions = new List<object[]>();

	public static float macroPlaybackRange = 1f;

	public static int macroPlaybackRangeIndex = 1;

	public static Dictionary<string, Macro> macros = new Dictionary<string, Macro>();

	public static bool recordingMacro;

	public static float positionDelay;

	public const float defaultMacroStep = 0.05f;

	public static List<PlayerPosition> recordingData = new List<PlayerPosition>();

	public static Coroutine activeMacro;

	public static Dictionary<Color, (GameObject head, GameObject leftHand, GameObject rightHand)> positions = new Dictionary<Color, (GameObject, GameObject, GameObject)>();

	public static bool frameStepper;

	public static bool midpointMacros;

	public static bool didMacro;

	public static bool directionBased;

	private static bool frameCompleted;

	private static bool frameStepperNotified;

	private static Vector3 walkPos;

	private static Vector3 walkNormal;

	public static int wallWalkStrengthIndex = 2;

	private static float wallWalkStrength = 9.81f;

	public static bool leftWallWalk;

	public static bool bothWallWalk;

	private static int rememberPageNumber;

	public static readonly string[][] mapData = new string[17][]
	{
		new string[3] { "Forest", "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/TreeRoomSpawnForestZone", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Forest, Tree Exit" },
		new string[3] { "City", "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/ForestToCity", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - City Front" },
		new string[3] { "Canyons", "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/ForestCanyonTransition", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Canyon" },
		new string[3] { "Clouds", "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/CityToSkyJungle", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Clouds From Computer" },
		new string[3] { "Caves", "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/ForestToCave", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Cave" },
		new string[3] { "Beach", "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/BeachToForest", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Beach for Computer" },
		new string[3] { "Mountains", "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/CityToMountain", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Mountain" },
		new string[3] { "Basement", "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/CityToBasement", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Basement For Computer" },
		new string[3] { "Metropolis", "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/MetropolisOnly", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Metropolis from Computer" },
		new string[3] { "Arcade", "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/CityToArcade", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - City frm Arcade" },
		new string[3] { "Critters", "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/CityCrittersTransition", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - City from Critters" },
		new string[3] { "Skate Park", "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/ForestToHoverboard", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Hoverboard from Forest" },
		new string[3] { "Monke Blocks", "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/MonkeBlocksElevatorExit", "Environment Objects/05Maze_PersistentObjects/GhostReactorElevatorManager/MonkeBlocksElevator/Triggers/JoinRoomTrigger" },
		new string[3] { "Rotating", "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/CityToRotating", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Rotating Map" },
		new string[3] { "Bayou", "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/BayouOnly", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - BayouComputer2" },
		new string[3] { "Virtual Stump", "VSTUMP", "VSTUMP" },
		new string[3] { "Lava Forest", "Environment Objects/05Maze_PersistentObjects/GhostReactorElevatorManager/VIMForestLavaElevator/Triggers/VIMExp1_SetZoneTrigger", "Environment Objects/05Maze_PersistentObjects/GhostReactorElevatorManager/VIMForestLavaElevator/Triggers/JoinRoomTrigger" }
	};

	public static bool previousTeleportTrigger;

	public static GameObject CheckPoint;

	public static int selectedCheckpoint = -1;

	public static float selectedCheckpointDelay;

	public static readonly List<GameObject> checkpoints = new List<GameObject>();

	public static GameObject BombObject;

	private static GameObject pearl;

	private static Texture2D pearltxt;

	private static Material pearlmat;

	private static bool isrighthandedpearl;

	public static Playspace playspace;

	public static readonly List<GameObject> forestColliders = new List<GameObject>();

	public static bool wasDisabledAlready;

	public static bool invisMonke;

	public static bool lastHit;

	public static bool lastHit2;

	public static bool lastRG;

	private static bool ghostMonke;

	public static Vector3 offsetLH = Vector3.zero;

	public static Vector3 offsetRH = Vector3.zero;

	public static Vector3 offsetH = Vector3.zero;

	private static Quaternion? bodyJoystickRot;

	public static Vector3? startPosition;

	public static GameObject recBodyRotary;

	public static Vector3 stillBeybladeStartPos = Vector3.zero;

	private static Vector3 headPos = Vector3.zero;

	private static Vector3 headRot = Vector3.zero;

	private static Vector3 handPos_L = Vector3.zero;

	private static Vector3 handRot_L = Vector3.zero;

	private static Vector3 handPos_R = Vector3.zero;

	private static Vector3 handRot_R = Vector3.zero;

	private static bool hasAdded;

	public static float beesDelay;

	public static float sizeScale = 1f;

	private static Quaternion? vrrigJoystickRot;

	private static readonly Dictionary<GorillaSurfaceOverride, float> velocityArchive = new Dictionary<GorillaSurfaceOverride, float>();

	public static GameObject stickpart;

	private static bool leftisclimbing;

	private static bool rightisclimbing;

	private static GameObject climb;

	public static float oldSlide;

	public static readonly Vector3[] lastLeft = (Vector3[])(object)new Vector3[10]
	{
		Vector3.zero,
		Vector3.zero,
		Vector3.zero,
		Vector3.zero,
		Vector3.zero,
		Vector3.zero,
		Vector3.zero,
		Vector3.zero,
		Vector3.zero,
		Vector3.zero
	};

	public static readonly Vector3[] lastRight = (Vector3[])(object)new Vector3[10]
	{
		Vector3.zero,
		Vector3.zero,
		Vector3.zero,
		Vector3.zero,
		Vector3.zero,
		Vector3.zero,
		Vector3.zero,
		Vector3.zero,
		Vector3.zero,
		Vector3.zero
	};

	private static VRRig sithlord;

	private static bool sithright;

	private static float sithdist = 1f;

	public static readonly Dictionary<VRRig, List<GameObject>> RigColliders = new Dictionary<VRRig, List<GameObject>>();

	public static int pullPowerInt;

	private static float pullPower = 0.05f;

	private static readonly Dictionary<bool, bool> previousTouchingGround = new Dictionary<bool, bool>();

	public static GameObject leftThrow;

	public static GameObject rightThrow;

	public static GameObject flickLeft;

	public static GameObject flickRight;

	private static Quaternion initialRotationLeft = Quaternion.identity;

	private static Quaternion initialRotationRight = Quaternion.identity;

	public static bool passWorldScaleCheck;

	public static Vector3? lastFramePosition;

	public static float extendingTime;

	public static GameObject lvT;

	public static GameObject rvT;

	public static float predCount = 0.0125f;

	public static int predInt = 1;

	public static int fakeLagDelayIndex = 10;

	private static float fakeLagDelay = 1f;

	public static bool isBlinking;

	public static int timerPowerIndex = 15;

	private static float timerPower = 1.5f;

	public static Vector3? longJumpPower;

	public static float? keepVelocityUntil;

	public static Vector3? velocity;

	public static float playspaceAbusePower = 0.004f;

	private static float preBounciness;

	private static PhysicsMaterialCombine whateverthisis = (PhysicsMaterialCombine)3;

	private static float preFrictiness;

	public static GameObject airSwimPart;

	private static GameObject giveSwimWater;

	private static float giveSwimDelay;

	private static float? waterSurfaceJumpAmount;

	private static float? waterSurfaceJumpMaxSpeed;

	public static readonly Dictionary<VRRig, Vector3> followPositions = new Dictionary<VRRig, Vector3>();

	public static bool tinnitusSelf;

	public static int targetHz = 4000;

	public static Dictionary<AudioClip, VoiceManager.Clip> tinnitus = new Dictionary<AudioClip, VoiceManager.Clip>();

	private static float hoverboardSpawnDelay;

	private static float soundSpamDelay;

	public static float headspazDelay;

	public static bool headspazType;

	private static Vector3 headoffs = Vector3.zero;

	public static float laggyRigDelay;

	public static bool wasRightPrimaryPressed;

	public static int multiplicationAmount = 15;

	public static bool Zoomed;

	public static bool ShouldZoom;

	private static FlipState backflip;

	private static bool wasBackflipPressed;

	private static float lastBackflipEnd = float.NegativeInfinity;

	private const float FlipDuration = 0.5f;

	private const float FlipCooldown = 1f;

	public static VRRig trackedPlayer;

	public static float trackDistance;

	public static bool trackFollowing;

	private static int savedTeleportIndex = 0;

	private static List<SavedTeleportEntry> savedTeleports = new List<SavedTeleportEntry>();

	private static bool teleportsLoaded;

	private static bool flipping;

	private static float flipStart;

	private static Quaternion flipFrom;

	private static Vector3 flipAxis;

	private const float flipDuration = 1f;

	public static float FlySpeed
	{
		get
		{
			return _flySpeed * (Main.scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
		}
		set
		{
			_flySpeed = value;
		}
	}

	private static string SavedTeleportsPath => Path.Combine(Application.persistentDataPath, "SeralythTeleports.json");

	public static void ChangePlatformType(bool positive = true)
	{
		string[] array = new string[7] { "Normal", "Invisible", "Rainbow", "Random Color", "Noclip", "Glass", "Projectile" };
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				platformMode++;
			}
			else
			{
				platformMode--;
			}
		}
		platformMode %= array.Length;
		if (platformMode < 0)
		{
			platformMode = array.Length - 1;
		}
		Buttons.GetIndex("Change Platform Type").overlapText = "Change Platform Type <color=grey>[</color><color=green>" + array[platformMode] + "</color><color=grey>]</color>";
	}

	public static void ChangePlatformShape(bool positive = true)
	{
		string[] array = new string[8] { "Sphere", "Cube", "Cylinder", "Legacy", "Small", "Long", "1x1", "Massive" };
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				platformShape++;
			}
			else
			{
				platformShape--;
			}
		}
		platformShape %= array.Length;
		if (platformShape < 0)
		{
			platformShape = array.Length - 1;
		}
		Buttons.GetIndex("Change Platform Shape").overlapText = "Change Platform Shape <color=grey>[</color><color=green>" + array[platformShape] + "</color><color=grey>]</color>";
	}

	public static PrimitiveType GetPlatformPrimitiveType()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		int num = platformShape;
		if (1 == 0)
		{
		}
		PrimitiveType result = (PrimitiveType)(num switch
		{
			0 => 0, 
			1 => 3, 
			2 => 2, 
			_ => 3, 
		});
		if (1 == 0)
		{
		}
		return result;
	}

	public static Vector3 GetPlatformScale()
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
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		return (new Vector3[8]
		{
			new Vector3(0.333f, 0.333f, 0.333f),
			new Vector3(0.333f, 0.333f, 0.333f),
			new Vector3(0.333f, 0.333f, 0.333f),
			new Vector3(0.025f, 0.3f, 0.4f),
			new Vector3(0.025f, 0.15f, 0.2f),
			new Vector3(0.025f, 0.3f, 0.8f),
			new Vector3(0.1f, 0.1f, 0.1f),
			new Vector3(0.025f, 1f, 1f)
		})[platformShape] * (Main.scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
	}

	public static GameObject CreatePlatform()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = GameObject.CreatePrimitive(GetPlatformPrimitiveType());
		val.transform.localScale = GetPlatformScale();
		Renderer component = val.GetComponent<Renderer>();
		switch (platformMode)
		{
		case 1:
		case 6:
			component.enabled = false;
			break;
		case 4:
			UpdateClipColliders(enabled: false);
			break;
		case 5:
			val.AddComponent<GorillaSurfaceOverride>().overrideIndex = 29;
			if ((Object)(object)Main.glass == (Object)null)
			{
				Main.glass = new Material(Shader.Find("GUI/Text Shader"));
			}
			component.material = Main.glass;
			break;
		}
		if (!Buttons.GetIndex("Non-Sticky Platforms").enabled)
		{
			Main.FixStickyColliders(val);
		}
		if (component.enabled && Object.op_Implicit((Object)(object)val))
		{
			ColorChanger colorChanger = val.AddComponent<ColorChanger>();
			colorChanger.colors = Main.backgroundColor;
			if (platformMode == 2 || platformMode == 3)
			{
				colorChanger.colors = colorChanger.colors.Clone();
				colorChanger.colors.rainbow |= platformMode == 2;
				colorChanger.colors.epileptic |= platformMode == 3;
			}
		}
		if (Buttons.GetIndex("Platform Outlines").enabled)
		{
			GameObject val2 = GameObject.CreatePrimitive(GetPlatformPrimitiveType());
			Object.Destroy((Object)(object)val2.GetComponent<Collider>());
			val2.transform.parent = val.transform;
			val2.transform.localPosition = Vector3.zero;
			val2.transform.localRotation = Quaternion.identity;
			val2.transform.localScale = new Vector3(0.95f, 1.05f, 1.05f);
			ColorChanger colorChanger2 = val2.AddComponent<ColorChanger>();
			colorChanger2.colors = Main.buttonColors[0];
		}
		return val;
	}

	public static void SetPlatformPosition(GameObject platform, bool left)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position;
		Quaternion rotation;
		Vector3 val;
		if (left)
		{
			(position, rotation, _, _, val) = ControllerUtilities.GetTrueLeftHand();
		}
		else
		{
			(position, rotation, _, _, val) = ControllerUtilities.GetTrueRightHand();
		}
		platform.transform.position = position;
		platform.transform.rotation = rotation;
		if (Buttons.GetIndex("Non-Sticky Platforms").enabled)
		{
			Transform transform = platform.transform;
			transform.position += val * ((left ? 1f : (-1f)) * ((0.025f + platform.transform.localScale.x / 2f) * (Main.scaleWithPlayer ? GTPlayer.Instance.scale : 1f)));
		}
		FriendManager.PlatformSpawned(leftHand: true, platform.transform.position, platform.transform.rotation, platform.transform.localScale, GetPlatformPrimitiveType());
	}

	public static void ProcessPlatform(bool left, bool value)
	{
		GameObject val = (left ? leftplat : rightplat);
		if (value)
		{
			if ((Object)(object)val == (Object)null)
			{
				GameObject platform = CreatePlatform();
				SetPlatformPosition(platform, left);
				if (left)
				{
					leftplat = platform;
				}
				else
				{
					rightplat = platform;
				}
			}
		}
		else if ((Object)(object)val != (Object)null)
		{
			if (Buttons.GetIndex("Platform Gravity").enabled)
			{
				val.AddComponent(typeof(Rigidbody));
				Object.Destroy((Object)(object)val.GetComponent<Collider>());
				Object.Destroy((Object)(object)val, 2f);
			}
			else
			{
				Object.Destroy((Object)(object)val);
			}
			if (left)
			{
				leftplat = null;
			}
			else
			{
				rightplat = null;
			}
			if (platformMode == 4 && (Object)(object)rightplat == (Object)null)
			{
				UpdateClipColliders(enabled: true);
			}
			FriendManager.PlatformDespawned(leftHand: true);
		}
	}

	public static void Platforms(bool? left = null, bool? right = null)
	{
		if (platformMode == 6)
		{
			Projectiles.GrabProjectile();
		}
		ProcessPlatform(left: true, left ?? Main.leftGrab);
		ProcessPlatform(left: false, right ?? Main.rightGrab);
	}

	public static void HandleFrozone(bool left)
	{
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		bool flag = (left ? Main.leftGrab : Main.rightGrab);
		frozonicPlatforms.TryGetValue(left, out var value);
		if (value == null)
		{
			value = new List<GameObject>();
			frozonicPlatforms.Add(left, value);
		}
		platformIndex.TryGetValue(left, out var value2);
		if (flag)
		{
			GameObject val;
			if (value.Count >= 72)
			{
				val = value[value2];
			}
			else
			{
				val = GameObject.CreatePrimitive((PrimitiveType)3);
				val.GetComponent<Renderer>().material.color = Main.backgroundColor.GetCurrentColor();
				val.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f) * (Main.scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
				val.AddComponent<GorillaSurfaceOverride>().overrideIndex = 61;
				value.Add(val);
			}
			Vector3 val2;
			Quaternion rotation;
			Vector3 val3;
			if (left)
			{
				(val2, rotation, _, _, val3) = ControllerUtilities.GetTrueLeftHand();
			}
			else
			{
				(val2, rotation, _, _, val3) = ControllerUtilities.GetTrueRightHand();
			}
			val.transform.position = val2 + val3 * ((left ? 1f : (-1f)) * ((0.025f + val.transform.localScale.x / 2f) * (Main.scaleWithPlayer ? GTPlayer.Instance.scale : 1f)));
			val.transform.rotation = rotation;
			value2 = (value2 + 1) % 72;
		}
		platformIndex[left] = value2;
		if (!flag && value.Count > 0)
		{
			int index = value.Count - 1;
			Object.Destroy((Object)(object)value[index]);
			value.RemoveAt(index);
		}
	}

	public static void Frozone()
	{
		HandleFrozone(left: true);
		HandleFrozone(left: false);
		((Collider)GorillaTagger.Instance.bodyCollider).enabled = !Main.leftGrab && !Main.rightGrab;
	}

	public static void ChangeSpeedBoostAmount(bool positive = true)
	{
		float[] array = new float[5] { 2f, 7.5f, 8f, 9f, 200f };
		float[] array2 = new float[5] { 0.5f, 1.1f, 1.5f, 2f, 10f };
		string[] array3 = new string[5] { "Slow", "Normal", "Middle", "Fast", "Ultra Fast" };
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				speedboostCycle++;
			}
			else
			{
				speedboostCycle--;
			}
		}
		speedboostCycle %= array.Length;
		if (speedboostCycle < 0)
		{
			speedboostCycle = array.Length - 1;
		}
		jspeed = array[speedboostCycle];
		jmulti = array2[speedboostCycle];
		Buttons.GetIndex("Change Speed Boost Amount").overlapText = "Change Speed Boost Amount <color=grey>[</color><color=green>" + array3[speedboostCycle] + "</color><color=grey>]</color>";
	}

	public static void PlatformSpam()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Expected O, but got Unknown
		if (Main.rightGrab)
		{
			GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
			Object.Destroy((Object)(object)val.GetComponent<BoxCollider>());
			val.GetComponent<Renderer>().material.color = Main.backgroundColor.GetCurrentColor();
			val.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
			val.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f);
			val.transform.position = GorillaTagger.Instance.rightHandTransform.position;
			val.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
			Object.Destroy((Object)(object)val, 1f);
			PhotonNetwork.RaiseEvent((byte)69, (object)new object[2]
			{
				val.transform.position,
				val.transform.rotation
			}, new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)0
			}, SendOptions.SendReliable);
		}
	}

	public static void PlatformGun()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Expected O, but got Unknown
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true))
			{
				GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
				Object.Destroy((Object)(object)val.GetComponent<BoxCollider>());
				val.GetComponent<Renderer>().material.color = Main.backgroundColor.GetCurrentColor();
				val.GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
				val.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f);
				val.transform.position = item.transform.position;
				val.transform.rotation = Quaternion.Euler((float)Random.Range(0, 360), (float)Random.Range(0, 360), (float)Random.Range(0, 360));
				Object.Destroy((Object)(object)val, 1f);
				PhotonNetwork.RaiseEvent((byte)69, (object)new object[2]
				{
					val.transform.position,
					val.transform.rotation
				}, new RaiseEventOptions
				{
					Receivers = (ReceiverGroup)0
				}, SendOptions.SendReliable);
			}
		}
	}

	public static void ChangeFlySpeed(bool positive = true)
	{
		float[] array = new float[5] { 5f, 10f, 30f, 60f, 0.5f };
		string[] array2 = new string[5] { "Slow", "Normal", "Fast", "Extra Fast", "Extra Slow" };
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				flySpeedCycle++;
			}
			else
			{
				flySpeedCycle--;
			}
		}
		flySpeedCycle %= array.Length;
		if (flySpeedCycle < 0)
		{
			flySpeedCycle = array.Length - 1;
		}
		FlySpeed = array[flySpeedCycle];
		Buttons.GetIndex("Change Fly Speed").overlapText = "Change Fly Speed <color=grey>[</color><color=green>" + array2[flySpeedCycle] + "</color><color=grey>]</color>";
	}

	public static void ChangePlayspaceAbuseSpeed(bool positive = true)
	{
		float[] array = new float[5] { 0.004f, 0.01f, 0.1f, 0.001f, 0.002f };
		string[] array2 = new string[5] { "Normal", "Fast", "Extra Fast", "Extra Slow", "Slow" };
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				playspaceAbuseIndex++;
			}
			else
			{
				playspaceAbuseIndex--;
			}
		}
		playspaceAbuseIndex %= array.Length;
		if (playspaceAbuseIndex < 0)
		{
			playspaceAbuseIndex = array.Length - 1;
		}
		playspaceAbusePower = array[playspaceAbuseIndex];
		Buttons.GetIndex("Change Playspace Abuse Speed").overlapText = "Change Playspace Abuse Speed <color=grey>[</color><color=green>" + array2[playspaceAbuseIndex] + "</color><color=grey>]</color>";
	}

	public static void ChangeArmLength(bool positive = true)
	{
		float[] array = new float[5] { 0.75f, 1.1f, 1.25f, 1.5f, 2f };
		string[] array2 = new string[5] { "Shorter", "Unnoticable", "Normal", "Long", "Extreme" };
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				longarmCycle++;
			}
			else
			{
				longarmCycle--;
			}
		}
		longarmCycle %= array.Length;
		if (longarmCycle < 0)
		{
			longarmCycle = array.Length - 1;
		}
		armlength = array[longarmCycle];
		Buttons.GetIndex("Change Arm Length").overlapText = "Change Arm Length <color=grey>[</color><color=green>" + array2[longarmCycle] + "</color><color=grey>]</color>";
	}

	public static void Fly()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightPrimary)
		{
			Transform transform = ((Component)GTPlayer.Instance).transform;
			transform.position += ((Component)GorillaTagger.Instance.headCollider).transform.forward * (Time.deltaTime * FlySpeed);
			GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
		}
	}

	public static void TriggerFly()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightTrigger > 0.5f)
		{
			Transform transform = ((Component)GTPlayer.Instance).transform;
			transform.position += ((Component)GorillaTagger.Instance.headCollider).transform.forward * (Time.deltaTime * FlySpeed);
			GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
		}
	}

	public static void GripFly()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab)
		{
			Transform transform = ((Component)GTPlayer.Instance).transform;
			transform.position += ((Component)GorillaTagger.Instance.headCollider).transform.forward * (Time.deltaTime * FlySpeed);
			GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
		}
	}

	public static void NoclipFly()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightPrimary)
		{
			Transform transform = ((Component)GTPlayer.Instance).transform;
			transform.position += ((Component)GorillaTagger.Instance.headCollider).transform.forward * (Time.deltaTime * FlySpeed);
			GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
			if (!noclip)
			{
				noclip = true;
				UpdateClipColliders(enabled: false);
			}
		}
		else if (noclip)
		{
			noclip = false;
			UpdateClipColliders(enabled: true);
		}
	}

	public static void JoystickFly()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		Vector2 leftJoystick = Main.leftJoystick;
		if ((double)Mathf.Abs(leftJoystick.x) > 0.3 || (double)Mathf.Abs(leftJoystick.y) > 0.3)
		{
			Transform transform = ((Component)GTPlayer.Instance).transform;
			transform.position += ((Component)GorillaTagger.Instance.headCollider).transform.forward * (Time.deltaTime * (leftJoystick.y * FlySpeed)) + ((Component)GorillaTagger.Instance.headCollider).transform.right * (Time.deltaTime * (leftJoystick.x * FlySpeed));
			GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
		}
	}

	public static void BarkFly()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(Main.leftJoystick.x, Main.rightJoystick.y, Main.leftJoystick.y);
		Vector3 val2 = GTVector3Extensions.X_Z(((Component)GTPlayer.Instance.bodyCollider).transform.forward);
		Vector3 val3 = GTVector3Extensions.X_Z(((Component)GTPlayer.Instance.bodyCollider).transform.right);
		Vector3 val4 = val.x * val3 + val.y * Vector3.up + val.z * val2;
		val4 *= FlySpeed;
		GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.Lerp(GorillaTagger.Instance.rigidbody.linearVelocity, val4, 0.12875f);
		ZeroGravity();
	}

	public static void VelocityBarkFly()
	{
		if ((double)Mathf.Abs(Main.leftJoystick.x) > 0.3 || (double)Mathf.Abs(Main.leftJoystick.y) > 0.3 || (double)Mathf.Abs(Main.rightJoystick.x) > 0.3 || (double)Mathf.Abs(Main.rightJoystick.y) > 0.3)
		{
			BarkFly();
		}
	}

	public static void HandFly()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightPrimary)
		{
			Transform transform = ((Component)GTPlayer.Instance).transform;
			transform.position += ControllerUtilities.GetTrueRightHand().forward * (Time.deltaTime * FlySpeed);
			GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
		}
	}

	public static void FlyTowardsGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Transform transform = ((Component)GTPlayer.Instance).transform;
				transform.position += (((Component)Main.lockTarget).transform.position - ((Component)GorillaTagger.Instance.bodyCollider).transform.position) * (Time.deltaTime * FlySpeed);
				GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
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

	public static void FlyGun()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			(RaycastHit, GameObject) tuple = Main.RenderGun();
			Vector3 point = ((RaycastHit)(ref tuple.Item1)).point;
			if (Main.GetGunInput(isShooting: true))
			{
				Vector3 val = point - ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
				Vector3 normalized = ((Vector3)(ref val)).normalized;
				Transform transform = ((Component)GTPlayer.Instance).transform;
				transform.position += normalized * (Time.deltaTime * FlySpeed);
				GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
			}
		}
	}

	public static void SlingshotFly()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightPrimary)
		{
			Rigidbody rigidbody = GorillaTagger.Instance.rigidbody;
			rigidbody.linearVelocity += ((Component)GTPlayer.Instance.headCollider).transform.forward * (Time.deltaTime * (FlySpeed * 2f));
		}
	}

	public static void ZeroGravitySlingshotFly()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightPrimary)
		{
			ZeroGravity();
			Rigidbody rigidbody = GorillaTagger.Instance.rigidbody;
			rigidbody.linearVelocity += ((Component)GTPlayer.Instance.headCollider).transform.forward * (Time.deltaTime * FlySpeed);
		}
	}

	public static void EnableWASDFly()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		lastPosition = ((Component)GorillaTagger.Instance.rigidbody).transform.position;
		if (XRSettings.isDeviceActive && Time.time < Main.timeMenuStarted + 1f)
		{
			Main.Toggle("WASD Fly");
		}
	}

	public static void WASDFly()
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		bool flag = !Buttons.GetIndex("Disable Stationary WASD Fly").enabled;
		bool key = UnityInput.GetKey((Key)37);
		bool key2 = UnityInput.GetKey((Key)15);
		bool key3 = UnityInput.GetKey((Key)33);
		bool key4 = UnityInput.GetKey((Key)18);
		bool key5 = UnityInput.GetKey((Key)1);
		bool key6 = UnityInput.GetKey((Key)55);
		bool key7 = UnityInput.GetKey((Key)51);
		bool key8 = UnityInput.GetKey((Key)53);
		bool key9 = UnityInput.GetKey((Key)61);
		bool key10 = UnityInput.GetKey((Key)62);
		bool key11 = UnityInput.GetKey((Key)63);
		bool key12 = UnityInput.GetKey((Key)64);
		if (flag || key || key2 || key3 || key4 || key5 || key6)
		{
			GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
		}
		if (!Object.op_Implicit((Object)(object)Main.menu))
		{
			Transform parent = GTPlayer.Instance.GetControllerTransform(false).parent;
			float num = 250f;
			if (key9)
			{
				parent.eulerAngles += new Vector3(0f, 0f - num, 0f) * Time.deltaTime;
			}
			if (key10)
			{
				parent.eulerAngles += new Vector3(0f, num, 0f) * Time.deltaTime;
			}
			if (key11)
			{
				parent.eulerAngles += new Vector3(0f - num, 0f, 0f) * Time.deltaTime;
			}
			if (key12)
			{
				parent.eulerAngles += new Vector3(num, 0f, 0f) * Time.deltaTime;
			}
			if (Mouse.current.rightButton.isPressed)
			{
				Quaternion rotation = parent.rotation;
				Vector3 eulerAngles = ((Quaternion)(ref rotation)).eulerAngles;
				if (startX < 0f)
				{
					startX = eulerAngles.y;
					subThingy = ((Vector2)((InputControl<Vector2>)(object)((Pointer)Mouse.current).position).value).x / (float)Screen.width;
				}
				if (startY < 0f)
				{
					startY = eulerAngles.x;
					subThingyZ = ((Vector2)((InputControl<Vector2>)(object)((Pointer)Mouse.current).position).value).y / (float)Screen.height;
				}
				float num2 = startY - (((Vector2)((InputControl<Vector2>)(object)((Pointer)Mouse.current).position).value).y / (float)Screen.height - subThingyZ) * 360f * 1.33f;
				float num3 = startX + (((Vector2)((InputControl<Vector2>)(object)((Pointer)Mouse.current).position).value).x / (float)Screen.width - subThingy) * 360f * 1.33f;
				num2 = ((num2 > 180f) ? (num2 - 360f) : num2);
				num2 = Mathf.Clamp(num2, -90f, 90f);
				parent.rotation = Quaternion.Euler(num2, num3, eulerAngles.z);
			}
			else
			{
				startX = -1f;
				startY = -1f;
			}
			float num4 = FlySpeed;
			if (key7)
			{
				num4 *= 2f;
			}
			else if (key8)
			{
				num4 /= 2f;
			}
			if (key)
			{
				Transform transform = ((Component)GorillaTagger.Instance.rigidbody).transform;
				transform.position += GTPlayer.Instance.GetControllerTransform(false).parent.forward * (Time.deltaTime * num4);
			}
			if (key3)
			{
				Transform transform2 = ((Component)GorillaTagger.Instance.rigidbody).transform;
				transform2.position += GTPlayer.Instance.GetControllerTransform(false).parent.forward * (Time.deltaTime * (0f - num4));
			}
			if (key2)
			{
				Transform transform3 = ((Component)GorillaTagger.Instance.rigidbody).transform;
				transform3.position += GTPlayer.Instance.GetControllerTransform(false).parent.right * (Time.deltaTime * (0f - num4));
			}
			if (key4)
			{
				Transform transform4 = ((Component)GorillaTagger.Instance.rigidbody).transform;
				transform4.position += GTPlayer.Instance.GetControllerTransform(false).parent.right * (Time.deltaTime * num4);
			}
			if (key5)
			{
				Transform transform5 = ((Component)GorillaTagger.Instance.rigidbody).transform;
				transform5.position += new Vector3(0f, Time.deltaTime * num4, 0f);
			}
			if (key6)
			{
				Transform transform6 = ((Component)GorillaTagger.Instance.rigidbody).transform;
				transform6.position += new Vector3(0f, Time.deltaTime * (0f - num4), 0f);
			}
			((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = ((Component)GorillaTagger.Instance.headCollider).transform.rotation;
		}
		if (!key && !key2 && !key3 && !key4 && !key5 && !key6 && lastPosition != Vector3.zero && flag)
		{
			((Component)GorillaTagger.Instance.rigidbody).transform.position = lastPosition;
		}
		else
		{
			lastPosition = ((Component)GorillaTagger.Instance.rigidbody).transform.position;
		}
	}

	public static void ChangeDriveSpeed(bool positive = true)
	{
		float[] array = new float[5] { 10f, 30f, 50f, 100f, 3f };
		string[] array2 = new string[5] { "Normal", "Fast", "Ultra Fast", "The Flash", "Slow" };
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				driveInt++;
			}
			else
			{
				driveInt--;
			}
		}
		driveInt %= array.Length;
		if (driveInt < 0)
		{
			driveInt = array.Length - 1;
		}
		driveSpeed = array[driveInt];
		Buttons.GetIndex("cdSpeed").overlapText = "Change Drive Speed <color=grey>[</color><color=green>" + array2[driveInt] + "</color><color=grey>]</color>";
	}

	public static void ChangeFastRopesSpeed(bool positive = true)
	{
		float[] array = new float[3] { 5f, 10f, 30f };
		string[] array2 = new string[3] { "Normal", "Fast", "Ultra Fast" };
		if (positive)
		{
			fastRopesInt++;
		}
		else
		{
			fastRopesInt--;
		}
		fastRopesInt %= array.Length;
		if (fastRopesInt < 0)
		{
			fastRopesInt = array.Length - 1;
		}
		RopePatch.amplifier = array[driveInt];
		Buttons.GetIndex("Change Fast Ropes Speed").overlapText = "Change Fast Ropes Speed <color=grey>[</color><color=green>" + array2[driveInt] + "</color><color=grey>]</color>";
	}

	public static void Drive()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		Vector2 leftJoystick = Main.leftJoystick;
		driveLerpDirection = Vector2.Lerp(driveLerpDirection, leftJoystick, 0.05f);
		Vector3 val = ((Component)GorillaTagger.Instance.bodyCollider).transform.forward * driveLerpDirection.y + ((Component)GorillaTagger.Instance.bodyCollider).transform.right * driveLerpDirection.x;
		RaycastHit val2 = default(RaycastHit);
		Physics.Raycast(((Component)GorillaTagger.Instance.bodyCollider).transform.position - new Vector3(0f, 0.2f, 0f), Vector3.down, ref val2, 512f, LayerMask.op_Implicit(GTPlayer.Instance.locomotionEnabledLayers));
		Vector3 val3 = val * driveSpeed;
		if (((RaycastHit)(ref val2)).distance < 0.2f && (Mathf.Abs(driveLerpDirection.x) > 0.05f || Mathf.Abs(driveLerpDirection.y) > 0.05f))
		{
			GorillaTagger.Instance.rigidbody.linearVelocity = new Vector3(val3.x, GorillaTagger.Instance.rigidbody.linearVelocity.y, val3.z);
		}
	}

	public static void HardDrive()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		bool enabled = Buttons.GetIndex("Elevated Sticky Drive").enabled;
		if ((Mathf.Abs(Main.leftJoystick.x) > 0.05f || Mathf.Abs(Main.leftJoystick.y) > 0.05f) && Main.closePosition == Vector3.zero)
		{
			Vector3 val = ((Component)GorillaTagger.Instance.bodyCollider).transform.forward * Main.leftJoystick.y + ((Component)GorillaTagger.Instance.bodyCollider).transform.right * Main.leftJoystick.x;
			Vector3 val2 = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + Vector3.up * 5f + val * (Time.deltaTime * driveSpeed);
			RaycastHit val3 = default(RaycastHit);
			Physics.Raycast(val2, Vector3.down, ref val3, 50f, LayerMask.op_Implicit(GTPlayer.Instance.locomotionEnabledLayers));
			Vector3 val4 = ((((RaycastHit)(ref val3)).point == Vector3.zero) ? val2 : ((RaycastHit)(ref val3)).point);
			Main.TeleportPlayer(val4 + Vector3.up * (enabled ? 0.7f : 0.2f));
			GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
		}
	}

	public static void Dash()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightPrimary && !previousDash)
		{
			Rigidbody rigidbody = GorillaTagger.Instance.rigidbody;
			rigidbody.linearVelocity += ((Component)GTPlayer.Instance.headCollider).transform.forward * FlySpeed;
		}
		previousDash = Main.rightPrimary;
	}

	public static void ReverseVelocity()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!(Time.time < nextrevTime) && Main.rightPrimary)
		{
			GorillaTagger.Instance.rigidbody.linearVelocity = -GorillaTagger.Instance.rigidbody.linearVelocity;
			nextrevTime = Time.time + revCooldown;
		}
	}

	public static void BirdFly()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		RaycastHit val = default(RaycastHit);
		if (!(Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, ((Component)GorillaTagger.Instance.headCollider).transform.position) < 0.63f) && !(Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, ((Component)GorillaTagger.Instance.headCollider).transform.position) < 0.63f) && !(Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, GorillaTagger.Instance.rightHandTransform.position) < 1f) && !Physics.Raycast(((Collider)GorillaTagger.Instance.bodyCollider).attachedRigidbody.position, Vector3.down, ref val, -1f))
		{
			InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode((XRNode)4);
			InputDevice deviceAtXRNode2 = InputDevices.GetDeviceAtXRNode((XRNode)5);
			Vector3 val2 = default(Vector3);
			Vector3 val3 = default(Vector3);
			if (((InputDevice)(ref deviceAtXRNode)).TryGetFeatureValue(CommonUsages.deviceVelocity, ref val2) && ((InputDevice)(ref deviceAtXRNode2)).TryGetFeatureValue(CommonUsages.deviceVelocity, ref val3) && !(Time.time - flapTime < 0.4f) && val2.y < -1.2f && val3.y < -1.2f)
			{
				float num = Mathf.Min(6f * ((Mathf.Abs(val2.y) + Mathf.Abs(val3.y)) / 2f) / 1.2f, 9f);
				((Collider)GorillaTagger.Instance.bodyCollider).attachedRigidbody.AddForce(Vector3.up * num, (ForceMode)2);
				flapTime = Time.time;
			}
		}
	}

	public static void IronMan()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		Rigidbody rigidbody = GorillaTagger.Instance.rigidbody;
		Vector3 linearVelocity;
		if (Main.leftPrimary)
		{
			Vector3 val = FlySpeed * -GorillaTagger.Instance.leftHandTransform.right;
			rigidbody.AddForce(val * Time.deltaTime, (ForceMode)2);
			float num = GorillaTagger.Instance.tapHapticStrength / 50f;
			linearVelocity = rigidbody.linearVelocity;
			float num2 = num * ((Vector3)(ref linearVelocity)).magnitude;
			GorillaTagger.Instance.StartVibration(true, num2, GorillaTagger.Instance.tapHapticDuration);
		}
		if (Main.rightPrimary)
		{
			Vector3 val2 = FlySpeed * GorillaTagger.Instance.rightHandTransform.right;
			rigidbody.AddForce(val2 * Time.deltaTime, (ForceMode)2);
			float num3 = GorillaTagger.Instance.tapHapticStrength / 50f;
			linearVelocity = rigidbody.linearVelocity;
			float num4 = num3 * ((Vector3)(ref linearVelocity)).magnitude;
			GorillaTagger.Instance.StartVibration(false, num4, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	private static BalloonHoldable GetTargetBalloon()
	{
		BalloonHoldable[] allType = Main.GetAllType<BalloonHoldable>(5f);
		foreach (BalloonHoldable val in allType)
		{
			if (((TransferrableObject)val).IsMyItem())
			{
				return val;
			}
		}
		if (Time.time > loaoalsode)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must equip a balloon.");
			loaoalsode = Time.time + 1f;
		}
		return null;
	}

	public static void SpiderMan()
	{
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Expected O, but got Unknown
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_065c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0661: Unknown result type (might be due to invalid IL or missing references)
		//IL_0666: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0700: Unknown result type (might be due to invalid IL or missing references)
		//IL_0705: Unknown result type (might be due to invalid IL or missing references)
		//IL_0749: Unknown result type (might be due to invalid IL or missing references)
		//IL_0757: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Expected O, but got Unknown
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		if (Main.leftGrab)
		{
			if (!isLeftGrappling)
			{
				isLeftGrappling = true;
				Rigidbody rigidbody = GorillaTagger.Instance.rigidbody;
				rigidbody.linearVelocity += GorillaTagger.Instance.leftHandTransform.forward * 5f;
				if (PhotonNetwork.InRoom)
				{
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", (RpcTarget)0, new object[3] { 89, true, 999999f });
				}
				else
				{
					VRRig.LocalRig.PlayHandTapLocal(89, true, 999999f);
				}
				Main.RPCProtection();
				leftgrapplePoint = GorillaTagger.Instance.leftHandTransform.position + GorillaTagger.Instance.leftHandTransform.forward * 16f;
				leftjoint = ((Component)GorillaTagger.Instance).gameObject.AddComponent<SpringJoint>();
				((Joint)leftjoint).autoConfigureConnectedAnchor = false;
				((Joint)leftjoint).connectedAnchor = leftgrapplePoint;
				float num = Vector3.Distance(GorillaTagger.Instance.rigidbody.position, leftgrapplePoint);
				leftjoint.maxDistance = num * 0.8f;
				leftjoint.minDistance = num * 0.25f;
				leftjoint.spring = 10f;
				leftjoint.damper = 50f;
				((Joint)leftjoint).massScale = 12f;
			}
			GameObject val = new GameObject("Line");
			LineRenderer val2 = val.AddComponent<LineRenderer>();
			if (Main.smoothLines)
			{
				val2.numCapVertices = 10;
				val2.numCornerVertices = 5;
			}
			Color endColor = (val2.startColor = Color.red);
			val2.endColor = endColor;
			val2.startWidth = 0.025f;
			val2.endWidth = 0.025f;
			val2.positionCount = 2;
			val2.useWorldSpace = true;
			val2.SetPosition(0, GorillaTagger.Instance.leftHandTransform.position);
			val2.SetPosition(1, leftgrapplePoint);
			((Renderer)val2).material.shader = Shader.Find("GorillaTag/UberShader");
			Object.Destroy((Object)(object)val, Time.deltaTime);
		}
		else
		{
			Vector3 val3 = GorillaTagger.Instance.leftHandTransform.position + GorillaTagger.Instance.leftHandTransform.forward * 16f;
			GameObject val4 = new GameObject("Line");
			LineRenderer val5 = val4.AddComponent<LineRenderer>();
			if (Main.smoothLines)
			{
				val5.numCapVertices = 10;
				val5.numCornerVertices = 5;
			}
			((Renderer)val5).material.shader = Shader.Find("Sprites/Default");
			val5.startColor = Main.backgroundColor.GetCurrentColor() - Color32.op_Implicit(new Color32((byte)0, (byte)0, (byte)0, (byte)128));
			val5.endColor = Main.backgroundColor.GetCurrentColor(0.5f) - Color32.op_Implicit(new Color32((byte)0, (byte)0, (byte)0, (byte)128));
			val5.startWidth = 0.025f;
			val5.endWidth = 0.025f;
			val5.positionCount = 2;
			val5.useWorldSpace = true;
			val5.SetPosition(0, GorillaTagger.Instance.leftHandTransform.position);
			val5.SetPosition(1, val3);
			Object.Destroy((Object)(object)val4, Time.deltaTime);
			isLeftGrappling = false;
			Object.Destroy((Object)(object)leftjoint);
		}
		if (Main.rightGrab)
		{
			if (!isRightGrappling)
			{
				isRightGrappling = true;
				Rigidbody rigidbody2 = GorillaTagger.Instance.rigidbody;
				rigidbody2.linearVelocity += GorillaTagger.Instance.rightHandTransform.forward * 5f;
				if (PhotonNetwork.InRoom)
				{
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", (RpcTarget)0, new object[3] { 89, false, 999999f });
					Main.RPCProtection();
				}
				else
				{
					VRRig.LocalRig.PlayHandTapLocal(89, false, 999999f);
				}
				rightgrapplePoint = GorillaTagger.Instance.rightHandTransform.position + GorillaTagger.Instance.rightHandTransform.forward * 16f;
				if (rightgrapplePoint == Vector3.zero)
				{
					rightgrapplePoint = GorillaTagger.Instance.rightHandTransform.position + GorillaTagger.Instance.rightHandTransform.forward * 512f;
				}
				rightjoint = ((Component)GorillaTagger.Instance).gameObject.AddComponent<SpringJoint>();
				((Joint)rightjoint).autoConfigureConnectedAnchor = false;
				((Joint)rightjoint).connectedAnchor = rightgrapplePoint;
				float num2 = Vector3.Distance(GorillaTagger.Instance.rigidbody.position, rightgrapplePoint);
				rightjoint.maxDistance = num2 * 0.8f;
				rightjoint.minDistance = num2 * 0.25f;
				rightjoint.spring = 10f;
				rightjoint.damper = 50f;
				((Joint)rightjoint).massScale = 12f;
			}
			GameObject val6 = new GameObject("Line");
			LineRenderer val7 = val6.AddComponent<LineRenderer>();
			if (Main.smoothLines)
			{
				val7.numCapVertices = 10;
				val7.numCornerVertices = 5;
			}
			Color endColor2 = (val7.startColor = Color.red);
			val7.endColor = endColor2;
			val7.startWidth = 0.025f;
			val7.endWidth = 0.025f;
			val7.positionCount = 2;
			val7.useWorldSpace = true;
			val7.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
			val7.SetPosition(1, rightgrapplePoint);
			((Renderer)val7).material.shader = Shader.Find("GorillaTag/UberShader");
			Object.Destroy((Object)(object)val6, Time.deltaTime);
		}
		else
		{
			Vector3 val8 = GorillaTagger.Instance.rightHandTransform.position + GorillaTagger.Instance.rightHandTransform.forward * 16f;
			GameObject val9 = new GameObject("Line");
			LineRenderer val10 = val9.AddComponent<LineRenderer>();
			if (Main.smoothLines)
			{
				val10.numCapVertices = 10;
				val10.numCornerVertices = 5;
			}
			((Renderer)val10).material.shader = Shader.Find("Sprites/Default");
			val10.startColor = Main.backgroundColor.GetCurrentColor() - Color32.op_Implicit(new Color32((byte)0, (byte)0, (byte)0, (byte)128));
			val10.endColor = Main.backgroundColor.GetCurrentColor(0.5f) - Color32.op_Implicit(new Color32((byte)0, (byte)0, (byte)0, (byte)128));
			val10.startWidth = 0.025f;
			val10.endWidth = 0.025f;
			val10.positionCount = 2;
			val10.useWorldSpace = true;
			val10.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
			val10.SetPosition(1, val8);
			Object.Destroy((Object)(object)val9, Time.deltaTime);
			isRightGrappling = false;
			Object.Destroy((Object)(object)rightjoint);
		}
	}

	public static void GrapplingHooks()
	{
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Expected O, but got Unknown
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Expected O, but got Unknown
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_0553: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Expected O, but got Unknown
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Expected O, but got Unknown
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		if (Main.leftGrab)
		{
			if (!isLeftGrappling)
			{
				isLeftGrappling = true;
				Rigidbody rigidbody = GorillaTagger.Instance.rigidbody;
				rigidbody.linearVelocity += GorillaTagger.Instance.leftHandTransform.forward * 5f;
				if (PhotonNetwork.InRoom)
				{
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", (RpcTarget)0, new object[3] { 89, true, 999999f });
				}
				else
				{
					VRRig.LocalRig.PlayHandTapLocal(89, true, 999999f);
				}
				Main.RPCProtection();
				leftgrapplePoint = GorillaTagger.Instance.leftHandTransform.position + GorillaTagger.Instance.leftHandTransform.forward * 16f;
			}
			else
			{
				Rigidbody rigidbody2 = GorillaTagger.Instance.rigidbody;
				rigidbody2.linearVelocity += Vector3.Normalize(leftgrapplePoint - GorillaTagger.Instance.leftHandTransform.position) * 0.5f;
			}
			GameObject val = new GameObject("Line");
			LineRenderer val2 = val.AddComponent<LineRenderer>();
			if (Main.smoothLines)
			{
				val2.numCapVertices = 10;
				val2.numCornerVertices = 5;
			}
			Color endColor = (val2.startColor = Color.red);
			val2.endColor = endColor;
			val2.startWidth = 0.025f;
			val2.endWidth = 0.025f;
			val2.positionCount = 2;
			val2.useWorldSpace = true;
			val2.SetPosition(0, GorillaTagger.Instance.leftHandTransform.position);
			val2.SetPosition(1, leftgrapplePoint);
			((Renderer)val2).material.shader = Shader.Find("GorillaTag/UberShader");
			Object.Destroy((Object)(object)val, Time.deltaTime);
		}
		else
		{
			Vector3 val3 = GorillaTagger.Instance.leftHandTransform.position + GorillaTagger.Instance.leftHandTransform.forward * 16f;
			GameObject val4 = new GameObject("Line");
			LineRenderer val5 = val4.AddComponent<LineRenderer>();
			if (Main.smoothLines)
			{
				val5.numCapVertices = 10;
				val5.numCornerVertices = 5;
			}
			((Renderer)val5).material.shader = Shader.Find("Sprites/Default");
			val5.startColor = Main.backgroundColor.GetCurrentColor() - Color32.op_Implicit(new Color32((byte)0, (byte)0, (byte)0, (byte)128));
			val5.endColor = Main.backgroundColor.GetCurrentColor(0.5f) - Color32.op_Implicit(new Color32((byte)0, (byte)0, (byte)0, (byte)128));
			val5.startWidth = 0.025f;
			val5.endWidth = 0.025f;
			val5.positionCount = 2;
			val5.useWorldSpace = true;
			val5.SetPosition(0, GorillaTagger.Instance.leftHandTransform.position);
			val5.SetPosition(1, val3);
			Object.Destroy((Object)(object)val4, Time.deltaTime);
			isLeftGrappling = false;
			Object.Destroy((Object)(object)leftjoint);
		}
		if (Main.rightGrab)
		{
			if (!isRightGrappling)
			{
				isRightGrappling = true;
				Rigidbody rigidbody3 = GorillaTagger.Instance.rigidbody;
				rigidbody3.linearVelocity += GorillaTagger.Instance.rightHandTransform.forward * 5f;
				if (PhotonNetwork.InRoom)
				{
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", (RpcTarget)0, new object[3] { 89, false, 999999f });
					Main.RPCProtection();
				}
				else
				{
					VRRig.LocalRig.PlayHandTapLocal(89, false, 999999f);
				}
				rightgrapplePoint = GorillaTagger.Instance.rightHandTransform.position + GorillaTagger.Instance.rightHandTransform.forward * 16f;
			}
			else
			{
				Rigidbody rigidbody4 = GorillaTagger.Instance.rigidbody;
				rigidbody4.linearVelocity += Vector3.Normalize(rightgrapplePoint - GorillaTagger.Instance.rightHandTransform.position) * 0.5f;
			}
			GameObject val6 = new GameObject("Line");
			LineRenderer val7 = val6.AddComponent<LineRenderer>();
			if (Main.smoothLines)
			{
				val7.numCapVertices = 10;
				val7.numCornerVertices = 5;
			}
			Color endColor2 = (val7.startColor = Color.red);
			val7.endColor = endColor2;
			val7.startWidth = 0.025f;
			val7.endWidth = 0.025f;
			val7.positionCount = 2;
			val7.useWorldSpace = true;
			val7.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
			val7.SetPosition(1, rightgrapplePoint);
			((Renderer)val7).material.shader = Shader.Find("GorillaTag/UberShader");
			Object.Destroy((Object)(object)val6, Time.deltaTime);
		}
		else
		{
			Vector3 val8 = GorillaTagger.Instance.rightHandTransform.position + GorillaTagger.Instance.rightHandTransform.forward * 16f;
			GameObject val9 = new GameObject("Line");
			LineRenderer val10 = val9.AddComponent<LineRenderer>();
			if (Main.smoothLines)
			{
				val10.numCapVertices = 10;
				val10.numCornerVertices = 5;
			}
			((Renderer)val10).material.shader = Shader.Find("Sprites/Default");
			val10.startColor = Main.backgroundColor.GetCurrentColor() - Color32.op_Implicit(new Color32((byte)0, (byte)0, (byte)0, (byte)128));
			val10.endColor = Main.backgroundColor.GetCurrentColor(0.5f) - Color32.op_Implicit(new Color32((byte)0, (byte)0, (byte)0, (byte)128));
			val10.startWidth = 0.025f;
			val10.endWidth = 0.025f;
			val10.positionCount = 2;
			val10.useWorldSpace = true;
			val10.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
			val10.SetPosition(1, val8);
			Object.Destroy((Object)(object)val9, Time.deltaTime);
			isRightGrappling = false;
		}
	}

	public static void DisableSpiderMan()
	{
		isLeftGrappling = false;
		Object.Destroy((Object)(object)leftjoint);
		isRightGrappling = false;
		Object.Destroy((Object)(object)rightjoint);
	}

	public static void NetworkedGrappleMods()
	{
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Invalid comparison between Unknown and I4
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Invalid comparison between Unknown and I4
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Invalid comparison between Unknown and I4
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Invalid comparison between Unknown and I4
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		if (!Buttons.GetIndex("Spider Man").enabled && !Buttons.GetIndex("Grappling Hooks").enabled)
		{
			return;
		}
		if (isLeftGrappling || isRightGrappling)
		{
			BalloonHoldable targetBalloon = GetTargetBalloon();
			targetBalloon.balloonState = (BalloonStates)0;
			targetBalloon.maxDistanceFromOwner = float.MaxValue;
			((TransferrableObject)targetBalloon).rigidbodyInstance.isKinematic = true;
			((Component)targetBalloon).gameObject.GetComponent<BalloonDynamics>().stringLength = 512f;
			((Component)targetBalloon).gameObject.GetComponent<BalloonDynamics>().stringStrength = 512f;
			((Component)targetBalloon).gameObject.GetComponent<BalloonDynamics>().enableDynamics = false;
			if ((Object)(object)targetBalloon != (Object)null)
			{
				if ((isLeftGrappling || isRightGrappling) && !((Renderer)targetBalloon.lineRenderer).enabled)
				{
					((TransferrableObject)targetBalloon).currentState = (PositionState)4;
				}
				if (isLeftGrappling)
				{
					((Component)targetBalloon).transform.position = leftgrapplePoint;
					((Component)targetBalloon).transform.LookAt(GorillaTagger.Instance.leftHandTransform.position);
				}
				else
				{
					((Component)targetBalloon).transform.position = rightgrapplePoint;
					((Component)targetBalloon).transform.LookAt(GorillaTagger.Instance.rightHandTransform.position);
					((Component)targetBalloon).transform.Rotate(Vector3.left, 90f, (Space)1);
				}
			}
		}
		else
		{
			BalloonHoldable targetBalloon2 = GetTargetBalloon();
			BalloonStates balloonState = targetBalloon2.balloonState;
			if ((int)balloonState != 1 && (int)balloonState != 2 && (int)balloonState != 5 && (int)balloonState != 6)
			{
				targetBalloon2.balloonState = (BalloonStates)0;
			}
			((TransferrableObject)targetBalloon2).rigidbodyInstance.isKinematic = false;
			((Component)targetBalloon2).gameObject.GetComponent<BalloonDynamics>().stringLength = 0.5f;
			((Component)targetBalloon2).gameObject.GetComponent<BalloonDynamics>().stringStrength = 0.9f;
			((Component)targetBalloon2).gameObject.GetComponent<BalloonDynamics>().enableDynamics = true;
			if ((Object)(object)targetBalloon2 != (Object)null)
			{
				((TransferrableObject)targetBalloon2).currentState = (PositionState)128;
			}
		}
	}

	public static bool IsOverlapping(Collider collider, Vector3 center, float radius)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = collider.ClosestPoint(center);
		bool flag = val == center;
		Vector3 val2 = val - center;
		float sqrMagnitude = ((Vector3)(ref val2)).sqrMagnitude;
		bool flag2 = sqrMagnitude <= radius * radius;
		return flag || flag2;
	}

	public static bool IsOverlapping(Collider colliderA, Collider colliderB, float radius)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = colliderB.ClosestPoint(((Component)colliderA).transform.position);
		Vector3 val2 = colliderA.ClosestPoint(((Component)colliderB).transform.position);
		bool flag = val2 == val;
		Vector3 val3 = val2 - val;
		float sqrMagnitude = ((Vector3)(ref val3)).sqrMagnitude;
		bool flag2 = sqrMagnitude <= radius * radius;
		return flag || flag2;
	}

	public static void PortalGun()
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_070f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0815: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_089d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0930: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)portalGun == (Object)null)
		{
			portalGun = AssetUtilities.LoadObject<GameObject>("PortalGun");
			portalGun.transform.SetParent(VRRig.LocalRig.rightHandTransform.parent, false);
			AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Mods/Movement/PortalGun/portalgun_powerup.ogg", "Audio/Mods/Movement/PortalGun/portalgun_powerup.ogg", delegate(AudioClip clip)
			{
				Main.Play2DAudio(clip, (float)Main.buttonClickVolume / 10f);
			});
		}
		if (Object.op_Implicit((Object)(object)portalGun))
		{
			Transform val = portalGun.transform.Find("PortalGun/Ray");
			RaycastHit val2 = default(RaycastHit);
			Physics.Raycast(val.position, val.forward, ref val2, 512f, LayerMask.op_Implicit(GTPlayer.Instance.locomotionEnabledLayers));
			if ((Object)(object)crosshair == (Object)null)
			{
				crosshair = GameObject.CreatePrimitive((PrimitiveType)0);
				crosshair.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
				crosshair.GetComponent<Renderer>().material.color = Color.white;
				Object.Destroy((Object)(object)crosshair.GetComponent<Collider>());
			}
			if (Object.op_Implicit((Object)(object)crosshair))
			{
				crosshair.transform.position = ((((RaycastHit)(ref val2)).point == Vector3.zero) ? (((Component)val).transform.position + ((Component)val).transform.forward * 20f) : ((RaycastHit)(ref val2)).point);
			}
			if (Main.rightTrigger > 0.5f && Time.time > portalDelay)
			{
				GameObject val3 = (flipped ? bluePortal : orangePortal);
				if (Object.op_Implicit((Object)(object)val3) && (Vector3.Distance(((RaycastHit)(ref val2)).point, val3.transform.position) < 1f || ((RaycastHit)(ref val2)).point == Vector3.zero))
				{
					AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Mods/Movement/PortalGun/portal_invalid.ogg", "Audio/Mods/Movement/PortalGun/portal_invalid.ogg", delegate(AudioClip clip)
					{
						Main.Play2DAudio(clip, (float)Main.buttonClickVolume / 10f);
					});
					portalDelay = Time.time + 0.5f;
					return;
				}
				AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Mods/Movement/PortalGun/portalgun_blue.ogg", "Audio/Mods/Movement/PortalGun/portalgun_blue.ogg", delegate(AudioClip clip)
				{
					Main.Play2DAudio(clip, (float)Main.buttonClickVolume / 10f);
				});
				GameObject val4;
				if (flipped)
				{
					if ((Object)(object)orangePortal == (Object)null)
					{
						orangePortal = GetPortal(orange: true);
					}
					val4 = orangePortal;
				}
				else
				{
					if ((Object)(object)bluePortal == (Object)null)
					{
						bluePortal = GetPortal(orange: false);
					}
					val4 = bluePortal;
				}
				val4.transform.position = ((RaycastHit)(ref val2)).point + ((RaycastHit)(ref val2)).normal * 0.075f;
				val4.transform.rotation = Quaternion.LookRotation(((RaycastHit)(ref val2)).normal, Vector3.up) * Quaternion.Euler(90f, 0f, 0f);
				((MonoBehaviour)CoroutineManager.instance).StartCoroutine(AnimatePortalScale(val4, 0.3f));
				portalDelay = Time.time + 0.5f;
			}
			if (Main.rightGrab && Time.time > flipDelay)
			{
				flipped = !flipped;
				Color32 val5 = (flipped ? new Color32(byte.MaxValue, (byte)150, (byte)0, (byte)1) : new Color32((byte)0, (byte)183, byte.MaxValue, (byte)1));
				((Component)portalGun.transform.Find("PortalGun/Light")).GetComponent<Renderer>().material.color = Color32.op_Implicit(val5);
				((Component)portalGun.transform.Find("PortalGun/Light/Tube")).GetComponent<Renderer>().material.color = Color32.op_Implicit(val5);
				AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Mods/Movement/PortalGun/portalgun_fizzle.ogg", "Audio/Mods/Movement/PortalGun/portalgun_fizzle.ogg", delegate(AudioClip clip)
				{
					Main.Play2DAudio(clip, (float)Main.buttonClickVolume / 10f);
				});
				flipDelay = Time.time + 0.5f;
			}
			if (Object.op_Implicit((Object)(object)bluePortal) && Object.op_Implicit((Object)(object)orangePortal))
			{
				GameObject gameObject = ((Component)bluePortal.transform.Find("Rim/View")).gameObject;
				GameObject gameObject2 = ((Component)orangePortal.transform.Find("Rim/View")).gameObject;
				if (!gameObject.activeSelf)
				{
					gameObject.SetActive(true);
					((Component)bluePortal.transform.Find("Rim/Static")).gameObject.SetActive(false);
					if (Buttons.GetIndex("High Quality Portals").enabled)
					{
						UpdateCameraRes(((Component)bluePortal.transform.Find("Rim/Camera/Eye")).GetComponent<Camera>());
					}
				}
				if (!gameObject2.activeSelf)
				{
					gameObject2.SetActive(true);
					((Component)orangePortal.transform.Find("Rim/Static")).gameObject.SetActive(false);
					if (Buttons.GetIndex("High Quality Portals").enabled)
					{
						UpdateCameraRes(((Component)orangePortal.transform.Find("Rim/Camera/Eye")).GetComponent<Camera>());
					}
				}
				if (gameObject.activeSelf && gameObject2.activeSelf && !playedOpen)
				{
					string text = (flipped ? "portal_open1.ogg" : "portal_open2.ogg");
					AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Mods/Movement/PortalGun/" + text, "Audio/Mods/Movement/PortalGun/" + text, delegate(AudioClip clip)
					{
						Main.Play2DAudio(clip, (float)Main.buttonClickVolume / 10f);
					});
					PortalTrigger orAddComponent = GTExt.GetOrAddComponent<PortalTrigger>(gameObject);
					PortalTrigger orAddComponent2 = GTExt.GetOrAddComponent<PortalTrigger>(gameObject2);
					orAddComponent.destination = orangePortal;
					orAddComponent2.destination = bluePortal;
					playedOpen = true;
				}
			}
			if (Main.rightPrimary && (Object.op_Implicit((Object)(object)bluePortal) || Object.op_Implicit((Object)(object)orangePortal)))
			{
				AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Mods/Movement/PortalGun/portal_close.ogg", "Audio/Mods/Movement/PortalGun/portal_close.ogg", delegate(AudioClip clip)
				{
					Main.Play2DAudio(clip, (float)Main.buttonClickVolume / 10f);
				});
				Object.Destroy((Object)(object)bluePortal);
				Object.Destroy((Object)(object)orangePortal);
				playedOpen = false;
			}
		}
		if (!Object.op_Implicit((Object)(object)bluePortal) || !Object.op_Implicit((Object)(object)orangePortal))
		{
			return;
		}
		bool flag = false;
		GameObject[] array = (GameObject[])(object)new GameObject[2] { bluePortal, orangePortal };
		foreach (GameObject val6 in array)
		{
			if (Vector3.Distance(val6.transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) < 0.6f)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			if (!inPortal)
			{
				AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Mods/Movement/PortalGun/portal_enter1.ogg", "Audio/Mods/Movement/PortalGun/portal_enter1.ogg", delegate(AudioClip clip)
				{
					Main.Play2DAudio(clip, (float)Main.buttonClickVolume / 10f);
				});
			}
		}
		else if (inPortal)
		{
			AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Mods/Movement/PortalGun/portal_exit1.ogg", "Audio/Mods/Movement/PortalGun/portal_exit1.ogg", delegate(AudioClip clip)
			{
				Main.Play2DAudio(clip, (float)Main.buttonClickVolume / 10f);
			});
		}
		inPortal = flag;
		GTPlayer.Instance.leftHand.isHolding = false;
		GameObject[] array2 = (GameObject[])(object)new GameObject[2] { bluePortal, orangePortal };
		foreach (GameObject val7 in array2)
		{
			if (IsOverlapping(((Component)val7.transform.Find("Rim/View")).GetComponent<Collider>(), ControllerUtilities.GetTrueLeftHand().position, 0.1f))
			{
				GTPlayer.Instance.leftHand.isHolding = true;
				break;
			}
		}
		GTPlayer.Instance.rightHand.isHolding = false;
		GameObject[] array3 = (GameObject[])(object)new GameObject[2] { bluePortal, orangePortal };
		foreach (GameObject val8 in array3)
		{
			if (IsOverlapping(((Component)val8.transform.Find("Rim/View")).GetComponent<Collider>(), ControllerUtilities.GetTrueRightHand().position, 0.1f))
			{
				GTPlayer.Instance.rightHand.isHolding = true;
				break;
			}
		}
		((Collider)GorillaTagger.Instance.bodyCollider).enabled = true;
		GameObject[] array4 = (GameObject[])(object)new GameObject[2] { bluePortal, orangePortal };
		foreach (GameObject val9 in array4)
		{
			if (IsOverlapping(((Component)val9.transform.Find("Rim/View")).GetComponent<Collider>(), ((Component)GTPlayer.Instance.bodyCollider).transform.position, 0.05f))
			{
				((Collider)GTPlayer.Instance.bodyCollider).enabled = false;
				break;
			}
		}
	}

	public static GameObject GetPortal(bool orange)
	{
		return ((Component)AssetUtilities.LoadObject<GameObject>(orange ? "OrangePortal" : "BluePortal").transform.Find("Portal")).gameObject;
	}

	public static IEnumerator TeleportPortal(GameObject portal)
	{
		Vector3 up = portal.transform.up;
		Vector3 linearVelocity = GorillaTagger.Instance.rigidbody.linearVelocity;
		Vector3 velocity = up * ((Vector3)(ref linearVelocity)).magnitude * 1.2f;
		Main.TeleportPlayer(portal.transform.position + portal.transform.up);
		Quaternion rotation = portal.transform.rotation;
		Main.SetRotation(((Quaternion)(ref rotation)).eulerAngles.y);
		GorillaTagger.Instance.rigidbody.linearVelocity = velocity;
		float timer = 0f;
		while (timer < 0.1f)
		{
			((Collider)GorillaTagger.Instance.bodyCollider).enabled = false;
			((Collider)GorillaTagger.Instance.headCollider).enabled = false;
			GTPlayer.Instance.leftHand.isHolding = true;
			GTPlayer.Instance.rightHand.isHolding = true;
			timer += Time.deltaTime;
			yield return null;
		}
		((Collider)GorillaTagger.Instance.bodyCollider).enabled = true;
		((Collider)GorillaTagger.Instance.headCollider).enabled = true;
		GTPlayer.Instance.leftHand.isHolding = false;
		GTPlayer.Instance.rightHand.isHolding = false;
	}

	public static IEnumerator TeleportObject(GameObject obj, GameObject portal)
	{
		AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Mods/Movement/PortalGun/portal_exit1.ogg", "Audio/Mods/Movement/PortalGun/portal_exit1.ogg", delegate(AudioClip clip)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			clip.PlayAt(obj.transform.position, (float)Main.buttonClickSound / 20f);
		});
		Rigidbody rigidbody = default(Rigidbody);
		if (obj.TryGetComponent<Rigidbody>(ref rigidbody) || obj.TryGetComponentInParent<Rigidbody>(out rigidbody))
		{
			Rigidbody obj2 = rigidbody;
			Vector3 up = portal.transform.up;
			Vector3 linearVelocity = rigidbody.linearVelocity;
			obj2.linearVelocity = up * ((Vector3)(ref linearVelocity)).magnitude * 1.2f;
		}
		obj.transform.position = portal.transform.position;
		obj.transform.rotation = Quaternion.LookRotation(portal.transform.up);
		Collider collider = default(Collider);
		if (obj.TryGetComponent<Collider>(ref collider))
		{
			float timer = 0f;
			while (timer < 0.1f)
			{
				collider.enabled = false;
				timer += Time.deltaTime;
				yield return null;
			}
			collider.enabled = true;
		}
	}

	public static IEnumerator AnimatePortalScale(GameObject portal, float duration)
	{
		Vector3 originalScale = portal.transform.localScale;
		portal.transform.localScale = Vector3.zero;
		float timer = 0f;
		while (timer < duration)
		{
			float t = timer / duration;
			float sinT = Mathf.Sin(t * MathF.PI * 0.5f);
			portal.transform.localScale = Vector3.LerpUnclamped(Vector3.zero, originalScale, sinT);
			timer += Time.deltaTime;
			yield return null;
		}
		portal.transform.localScale = originalScale;
	}

	public static void UpdateCameraRes(Camera cam)
	{
		((Texture)cam.targetTexture).width = ((Texture)cam.targetTexture).width * 5;
		((Texture)cam.targetTexture).height = ((Texture)cam.targetTexture).height * 5;
	}

	public static void DisablePortalGun()
	{
		Object.Destroy((Object)(object)portalGun);
		Object.Destroy((Object)(object)bluePortal);
		Object.Destroy((Object)(object)orangePortal);
		Object.Destroy((Object)(object)crosshair);
		playedOpen = false;
	}

	public static void UpAndDown()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightTrigger > 0.5f || Main.rightGrab)
		{
			ZeroGravity();
		}
		if (Main.rightTrigger > 0.5f)
		{
			Rigidbody rigidbody = GorillaTagger.Instance.rigidbody;
			rigidbody.linearVelocity += Vector3.up * (Time.deltaTime * FlySpeed * 3f);
		}
		if (Main.rightGrab)
		{
			Rigidbody rigidbody2 = GorillaTagger.Instance.rigidbody;
			rigidbody2.linearVelocity += Vector3.up * (Time.deltaTime * FlySpeed * -3f);
		}
	}

	public static void LeftAndRight()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightTrigger > 0.5f || Main.rightGrab)
		{
			ZeroGravity();
		}
		if (Main.rightTrigger > 0.5f)
		{
			Rigidbody rigidbody = GorillaTagger.Instance.rigidbody;
			rigidbody.linearVelocity += ((Component)GorillaTagger.Instance.bodyCollider).transform.right * (Time.deltaTime * FlySpeed * -3f);
		}
		if (Main.rightGrab)
		{
			Rigidbody rigidbody2 = GorillaTagger.Instance.rigidbody;
			rigidbody2.linearVelocity += ((Component)GorillaTagger.Instance.bodyCollider).transform.right * (Time.deltaTime * FlySpeed * 3f);
		}
	}

	public static void ForwardsAndBackwards()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightTrigger > 0.5f || Main.rightGrab)
		{
			ZeroGravity();
		}
		if (Main.rightTrigger > 0.5f)
		{
			Rigidbody rigidbody = GorillaTagger.Instance.rigidbody;
			rigidbody.linearVelocity += ((Component)GorillaTagger.Instance.bodyCollider).transform.forward * (Time.deltaTime * FlySpeed * 3f);
		}
		if (Main.rightGrab)
		{
			Rigidbody rigidbody2 = GorillaTagger.Instance.rigidbody;
			rigidbody2.linearVelocity += ((Component)GorillaTagger.Instance.bodyCollider).transform.forward * (Time.deltaTime * FlySpeed * -3f);
		}
	}

	public static void AutoWalk()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		Vector2 leftJoystick = Main.leftJoystick;
		float num = 0.45f;
		float num2 = 9f;
		if (Main.leftJoystickClick)
		{
			num2 *= 1.5f;
		}
		if (Mathf.Abs(leftJoystick.y) > 0.05f || Mathf.Abs(leftJoystick.x) > 0.05f)
		{
			GorillaTagger.Instance.leftHandTransform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + ((Component)GorillaTagger.Instance.bodyCollider).transform.forward * (Mathf.Sin(Time.time * num2) * (leftJoystick.y * num)) + ((Component)GorillaTagger.Instance.bodyCollider).transform.right * (Mathf.Sin(Time.time * num2) * (leftJoystick.x * num) - 0.2f) + new Vector3(0f, -0.3f + Mathf.Cos(Time.time * num2) * 0.2f, 0f);
			GorillaTagger.Instance.rightHandTransform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + ((Component)GorillaTagger.Instance.bodyCollider).transform.forward * ((0f - Mathf.Sin(Time.time * num2)) * (leftJoystick.y * num)) + ((Component)GorillaTagger.Instance.bodyCollider).transform.right * ((0f - Mathf.Sin(Time.time * num2)) * (leftJoystick.x * num) + 0.2f) + new Vector3(0f, -0.3f + Mathf.Cos(Time.time * num2) * -0.2f, 0f);
		}
	}

	public static void AutoFunnyRun()
	{
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab)
		{
			if (Main.bothHands)
			{
				float num = Time.frameCount;
				GorillaTagger.Instance.rightHandTransform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position + ((Component)GorillaTagger.Instance.headCollider).transform.forward * MathF.Cos(num) / 10f + new Vector3(0f, -0.5f - MathF.Sin(num) / 7f, 0f) + ((Component)GorillaTagger.Instance.headCollider).transform.right * -0.05f;
				GorillaTagger.Instance.leftHandTransform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position + ((Component)GorillaTagger.Instance.headCollider).transform.forward * MathF.Cos(num + 180f) / 10f + new Vector3(0f, -0.5f - MathF.Sin(num + 180f) / 7f, 0f) + ((Component)GorillaTagger.Instance.headCollider).transform.right * 0.05f;
			}
			else
			{
				float x = Time.frameCount;
				GorillaTagger.Instance.rightHandTransform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position + ((Component)GorillaTagger.Instance.headCollider).transform.forward * MathF.Cos(x) / 10f + new Vector3(0f, -0.5f - MathF.Sin(x) / 7f, 0f);
			}
		}
	}

	public static void AutoPinchClimb()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab)
		{
			float x = (float)Time.frameCount / 3f;
			GorillaTagger.Instance.rightHandTransform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position + ((Component)GorillaTagger.Instance.headCollider).transform.right * (0.4f + MathF.Cos(x) * 0.4f) + ((Component)GorillaTagger.Instance.headCollider).transform.up * (MathF.Sin(x) * 0.6f) + ((Component)GorillaTagger.Instance.headCollider).transform.forward * 0.75f;
			GorillaTagger.Instance.leftHandTransform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position + ((Component)GorillaTagger.Instance.headCollider).transform.right * (0f - (0.4f + MathF.Cos(x) * 0.4f)) + ((Component)GorillaTagger.Instance.headCollider).transform.up * (MathF.Sin(x) * 0.6f) + ((Component)GorillaTagger.Instance.headCollider).transform.forward * 0.75f;
		}
	}

	public static void AutoElevatorClimb()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab)
		{
			float x = (float)Time.frameCount / 3f;
			GorillaTagger.Instance.rightHandTransform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position + ((Component)GorillaTagger.Instance.headCollider).transform.right * (0.4f + MathF.Cos(x) * 0.4f) + ((Component)GorillaTagger.Instance.headCollider).transform.up * (MathF.Sin(x) * 0.6f) + ((Component)GorillaTagger.Instance.headCollider).transform.forward * 0.75f;
		}
	}

	public static Vector3[] GetAllTreeBranchPositions()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		if (posArchive != null)
		{
			return posArchive.ToArray();
		}
		posArchive = new List<Vector3>();
		Vector3[] array = (Vector3[])(object)new Vector3[13]
		{
			new Vector3(-2.383f, 3.784f, 0.738f),
			new Vector3(1.55f, 5.559f, -1.56f),
			new Vector3(-2.225f, 7.214f, 0.063f),
			new Vector3(1.365f, 6.62f, 0.82f),
			new Vector3(0.405f, 8.865f, -2.759f),
			new Vector3(-2.227f, 9.763f, 2.071f),
			new Vector3(2.421f, 10.91f, 1.313f),
			new Vector3(1.618f, 13.169f, -1.216f),
			new Vector3(2.175f, 12.959f, -0.229f),
			new Vector3(1.855f, 13.837f, 1.215f),
			new Vector3(-0.265f, 14.953f, 2.935f),
			new Vector3(-2.049f, 14.962f, -1.708f),
			new Vector3(-1.249f, 18.93f, -1.62f)
		};
		string[] array2 = new string[2] { "Environment Objects/LocalObjects_Prefab/Forest/Terrain/SmallTrees/Group1", "Environment Objects/LocalObjects_Prefab/Forest/Terrain/SmallTrees/Group2" };
		string[] array3 = array2;
		foreach (string find in array3)
		{
			GameObject val = Main.GetObject(find);
			for (int j = 0; j < val.transform.childCount; j++)
			{
				GameObject gameObject = ((Component)val.transform.GetChild(j)).gameObject;
				Vector3 localScale = gameObject.transform.localScale;
				Transform transform = gameObject.transform;
				transform.localScale *= 5f;
				Vector3[] array4 = array;
				foreach (Vector3 val2 in array4)
				{
					posArchive.Add(gameObject.transform.TransformPoint(val2));
				}
				gameObject.transform.localScale = localScale;
			}
		}
		return posArchive.ToArray();
	}

	public static void AutoBranch()
	{
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		float num = 5f;
		float num2 = 0.3f;
		if (Main.rightGrab)
		{
			float num3 = float.MaxValue;
			Vector3 val = Vector3.zero;
			Vector3 position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
			Vector3[] allTreeBranchPositions = GetAllTreeBranchPositions();
			foreach (Vector3 val2 in allTreeBranchPositions)
			{
				float num4 = Vector3.Distance(position, val2);
				float num5 = Vector3.Dot(val2 - position, ((Component)GorillaTagger.Instance.bodyCollider).transform.right);
				if (num4 < num3 && num5 > 0f)
				{
					num3 = num4;
					val = val2;
				}
			}
			if (num3 < num)
			{
				flag = true;
				rightPos = Vector3.Lerp(rightPos, val, num2);
			}
			else
			{
				rightPos = Vector3.Lerp(rightPos, GorillaTagger.Instance.rightHandTransform.position, num2);
			}
			GorillaTagger.Instance.rightHandTransform.position = rightPos;
			Vector3 val3 = val;
			num3 = float.MaxValue;
			val = Vector3.zero;
			position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
			Vector3[] allTreeBranchPositions2 = GetAllTreeBranchPositions();
			foreach (Vector3 val4 in allTreeBranchPositions2)
			{
				float num6 = Vector3.Distance(position, val4);
				float num7 = Vector3.Dot(val4 - position, ((Component)GorillaTagger.Instance.bodyCollider).transform.right);
				if (num6 < num3 && val4 != val3 && num7 < 0f)
				{
					num3 = num6;
					val = val4;
				}
			}
			if (num3 < num)
			{
				flag = true;
				leftPos = Vector3.Lerp(leftPos, val, num2);
			}
			else
			{
				leftPos = Vector3.Lerp(leftPos, GorillaTagger.Instance.leftHandTransform.position, num2);
			}
			GorillaTagger.Instance.leftHandTransform.position = leftPos;
		}
		else
		{
			leftPos = GorillaTagger.Instance.leftHandTransform.position;
			rightPos = GorillaTagger.Instance.rightHandTransform.position;
		}
		if (flag)
		{
			GorillaTagger.Instance.rigidbody.linearVelocity = ((Component)GTPlayer.Instance.headCollider).transform.forward * 10f;
		}
		if (flag)
		{
			if (!lastOnBranch)
			{
				UpdateClipColliders(enabled: false);
			}
		}
		else if (lastOnBranch)
		{
			UpdateClipColliders(enabled: true);
		}
		lastOnBranch = flag;
	}

	public static void ForceTagFreeze()
	{
		GTPlayer.Instance.disableMovement = true;
	}

	public static void NoTagFreeze()
	{
		GTPlayer.Instance.disableMovement = false;
	}

	public static void FeatherFalling()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		Rigidbody rigidbody = GorillaTagger.Instance.rigidbody;
		Vector3 linearVelocity = rigidbody.linearVelocity;
		if (((Vector3)(ref linearVelocity)).magnitude > 0.1f || !GorillaTagger.Instance.IsGrounded(0.2f))
		{
			Vector3 val = rigidbody.linearVelocity * 4f;
			rigidbody.linearVelocity -= val * Time.fixedDeltaTime;
			if (GorillaTagger.Instance.IsGrounded() && rigidbody.linearVelocity.y < 0f)
			{
				rigidbody.linearVelocity = new Vector3(rigidbody.linearVelocity.x, Mathf.Max(rigidbody.linearVelocity.y, -2f), rigidbody.linearVelocity.z);
			}
		}
	}

	public static void LowGravity()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		GorillaTagger.Instance.rigidbody.AddForce(Vector3.up * 6.66f, (ForceMode)5);
	}

	public static void ZeroGravity()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		GorillaTagger.Instance.rigidbody.AddForce(-Physics.gravity, (ForceMode)5);
	}

	public static void HighGravity()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		GorillaTagger.Instance.rigidbody.AddForce(Vector3.down * 7.77f, (ForceMode)5);
	}

	public static void ReverseGravity()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		GorillaTagger.Instance.rigidbody.AddForce(Vector3.up * 19.62f, (ForceMode)5);
		GTPlayer.Instance.GetControllerTransform(false).parent.rotation = Quaternion.Euler(180f, 0f, 0f);
	}

	public static void UnflipCharacter()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		GTPlayer.Instance.GetControllerTransform(false).parent.rotation = Quaternion.identity;
	}

	public static void Rewind()
	{
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightTrigger > 0.5f)
		{
			if (playerPositions.Count > 0)
			{
				List<object[]> list = playerPositions;
				object[] array = list[list.Count - 1];
				Main.TeleportPlayer((Vector3)array[0], keepVelocity: false);
				GorillaTagger.Instance.leftHandTransform.position = (Vector3)array[1];
				GorillaTagger.Instance.leftHandTransform.rotation = (Quaternion)array[2];
				GorillaTagger.Instance.rightHandTransform.position = (Vector3)array[3];
				GorillaTagger.Instance.rightHandTransform.rotation = (Quaternion)array[4];
				GorillaTagger.Instance.rigidbody.linearVelocity = (Vector3)array[5] * -1f;
				playerPositions.RemoveAt(playerPositions.Count - 1);
			}
		}
		else
		{
			playerPositions.Add(new object[6]
			{
				((Component)GorillaTagger.Instance.bodyCollider).transform.position,
				GorillaTagger.Instance.leftHandTransform.position,
				GorillaTagger.Instance.leftHandTransform.rotation,
				GorillaTagger.Instance.rightHandTransform.position,
				GorillaTagger.Instance.rightHandTransform.rotation,
				GorillaTagger.Instance.rigidbody.linearVelocity
			});
			if (playerPositions.Count > 8640)
			{
				playerPositions.RemoveAt(0);
			}
		}
	}

	public static void ClearRewind()
	{
		playerPositions.Clear();
	}

	public static void ChangeMacroPlaybackRange(bool positive = true)
	{
		float[] array = new float[5] { 0.5f, 1f, 2f, 3f, 0.25f };
		string[] array2 = new string[5] { "Small", "Normal", "Large", "Extra Large", "Extra Small" };
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				macroPlaybackRangeIndex++;
			}
			else
			{
				macroPlaybackRangeIndex--;
			}
		}
		macroPlaybackRangeIndex %= array2.Length;
		if (macroPlaybackRangeIndex < 0)
		{
			macroPlaybackRangeIndex = array2.Length - 1;
		}
		macroPlaybackRange = array[macroPlaybackRangeIndex];
		Buttons.GetIndex("Change Macro Playback Range").overlapText = "Change Macro Playback Range <color=grey>[</color><color=green>" + array2[macroPlaybackRangeIndex] + "</color><color=grey>]</color>";
	}

	public static string FormatMacroName(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return input;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in input)
		{
			stringBuilder.Append(char.IsLetterOrDigit(c) ? c : '-');
		}
		return stringBuilder.ToString();
	}

	public static void LoadMacros()
	{
		macros.Clear();
		string[] files = Directory.GetFiles("SeralythMenu/Macros");
		foreach (string text in files)
		{
			if (text.EndsWith(".json"))
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
				Macro value = Macro.LoadJSON(File.ReadAllText(text));
				macros[fileNameWithoutExtension] = value;
			}
		}
		List<ButtonInfo> list = new List<ButtonInfo>
		{
			new ButtonInfo
			{
				buttonText = "Exit Macros",
				method = delegate
				{
					Buttons.CurrentCategoryName = "Movement Mods";
				},
				isTogglable = false,
				toolTip = "Returns you back to the movement mods."
			}
		};
		int num = 0;
		foreach (KeyValuePair<string, Macro> macro2 in macros)
		{
			Macro macro = macro2.Value;
			string macroName = macro2.Key;
			list.Add(new ButtonInfo
			{
				buttonText = "Macro" + macroName,
				overlapText = macro.name,
				enabled = macro.enabled,
				enableMethod = delegate
				{
					ToggleMacro(macroName, enabled: true);
				},
				method = delegate
				{
					ExecuteMacroButton(macro);
				},
				disableMethod = delegate
				{
					ToggleMacro(macroName, enabled: false);
				},
				toolTip = "Toggles on and off the " + macro.name + " macro."
			});
			num++;
		}
		list.AddRange(new ButtonInfo[6]
		{
			new ButtonInfo
			{
				buttonText = "Record <color=grey>[</color><color=green>T</color><color=grey>]</color>",
				method = RecordMacro,
				toolTip = "Record your macros with your <color=green>left trigger</color>."
			},
			new ButtonInfo
			{
				buttonText = "Macro Gun",
				method = MacroGun,
				toolTip = "Record your macros using a <color=green>gun</color>. Grip to aim, trigger to record."
			},
			new ButtonInfo
			{
				buttonText = "Open Macros Folder",
				method = OpenMacrosFolder,
				isTogglable = false,
				toolTip = "Opens the folder in which your plugins are located."
			},
			new ButtonInfo
			{
				buttonText = "Reload Macros",
				method = LoadMacros,
				isTogglable = false,
				toolTip = "Reloads your macros."
			},
			new ButtonInfo
			{
				buttonText = "Enable All Macros",
				method = delegate
				{
					foreach (KeyValuePair<string, Macro> macro3 in macros)
					{
						string key = macro3.Key;
						macro3.Value.enabled = true;
						ToggleMacro(key, enabled: true);
					}
					LoadMacros();
				},
				isTogglable = false,
				toolTip = "Enables all macros."
			},
			new ButtonInfo
			{
				buttonText = "Disable All Macro",
				method = delegate
				{
					foreach (KeyValuePair<string, Macro> macro4 in macros)
					{
						string key = macro4.Key;
						macro4.Value.enabled = false;
						ToggleMacro(key, enabled: false);
					}
					StopMacro();
					LoadMacros();
				},
				isTogglable = false,
				toolTip = "Disables all macros."
			}
		});
		Buttons.buttons[Buttons.GetCategory("Macros")] = list.ToArray();
	}

	public static void OpenMacrosFolder()
	{
		string fileName = FileUtilities.GetGamePath() + "/SeralythMenu/Macros";
		Process.Start(fileName);
	}

	public static void ToggleMacro(string macroName, bool enabled)
	{
		string path = "SeralythMenu/Macros/" + macroName + ".json";
		if (File.Exists(path))
		{
			Macro macro = macros[macroName];
			macro.enabled = enabled;
			File.WriteAllText(path, macro.DumpJSON());
		}
	}

	public static void RecordMacro()
	{
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		if (Main.leftTrigger > 0.5f)
		{
			if (!recordingMacro)
			{
				recordingData.Clear();
				recordingMacro = true;
				NotificationManager.SendNotification("<color=grey>[</color><color=green>RECORDING</color><color=grey>]</color> Started recording...");
			}
			((ControllerInputPoller)ControllerInputPoller.instance).leftControllerIndexFloat = 0f;
			if (recordingMacro && Time.time > positionDelay)
			{
				positionDelay = Time.time + 0.05f;
				recordingData.Add(PlayerPosition.CurrentPosition());
			}
			if (recordingMacro && recordingData.Count > 0)
			{
				VisualizePlayerPosition(recordingData[0], Color.green);
			}
		}
		else if (recordingMacro)
		{
			recordingMacro = false;
			NotificationManager.SendNotification("<color=grey>[</color><color=green>RECORDING</color><color=grey>]</color> Stopped recording.");
			FinalizeRecording();
		}
	}

	public static void MacroGun()
	{
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			if (!Main.gunLocked)
			{
				(RaycastHit, GameObject) tuple = Main.RenderGun();
				VRRig componentInParent = ((Component)((RaycastHit)(ref tuple.Item1)).collider).GetComponentInParent<VRRig>();
				if ((Object)(object)componentInParent != (Object)null && !componentInParent.IsLocal() && Main.GetGunInput(isShooting: true))
				{
					Main.lockTarget = componentInParent;
					Main.gunLocked = true;
					recordingData.Clear();
					recordingMacro = true;
					NotificationManager.SendNotification("<color=grey>[</color><color=green>MACRO GUN</color><color=grey>]</color> Recording <color=green>" + componentInParent.GetName() + "</color>...");
				}
			}
			Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && recordingMacro && Time.time > positionDelay)
			{
				positionDelay = Time.time + 0.05f;
				Rigidbody component = ((Component)Main.lockTarget).GetComponent<Rigidbody>();
				recordingData.Add(new PlayerPosition
				{
					position = ((Component)Main.lockTarget).transform.position,
					velocity = (((Object)(object)component != (Object)null) ? component.linearVelocity : Vector3.zero),
					leftHand = (position: Main.lockTarget.leftHandTransform.position, rotation: Main.lockTarget.leftHandTransform.rotation),
					rightHand = (position: Main.lockTarget.rightHandTransform.position, rotation: Main.lockTarget.rightHandTransform.rotation),
					leftGrip = false,
					rightGrip = false,
					leftTrigger = false,
					rightTrigger = false
				});
				if (recordingData.Count > 0)
				{
					VisualizePlayerPosition(recordingData[0], Color.green);
				}
			}
			if (!Main.GetGunInput(isShooting: true))
			{
				if (recordingMacro)
				{
					recordingMacro = false;
					NotificationManager.SendNotification("<color=grey>[</color><color=green>MACRO GUN</color><color=grey>]</color> Stopped recording.");
					FinalizeRecording();
				}
				Main.gunLocked = false;
				Main.lockTarget = null;
			}
		}
		else
		{
			if (recordingMacro)
			{
				recordingMacro = false;
				NotificationManager.SendNotification("<color=grey>[</color><color=green>MACRO GUN</color><color=grey>]</color> Stopped recording.");
				FinalizeRecording();
			}
			Main.gunLocked = false;
			Main.lockTarget = null;
		}
	}

	public static void FinalizeRecording()
	{
		List<PlayerPosition> savedRecordingData = recordingData;
		Main.Prompt("Would you like to save your macro?", delegate
		{
			Main.PromptText("Please name your macro:", delegate
			{
				string text = Main.keyboardInput;
				if (StringUtils.IsNullOrEmpty(text))
				{
					text = $"Macro #{macros.Count + 1}";
				}
				Macro macro = new Macro
				{
					name = text,
					positions = savedRecordingData,
					enabled = true,
					macroStepDuration = 0.05f
				};
				string path = Path.Combine("SeralythMenu", "Macros", FormatMacroName(text) + ".json");
				File.WriteAllText(path, macro.DumpJSON());
				LoadMacros();
			}, null, "Done", "Cancel");
		});
	}

	public static IEnumerator PlayMacro(Macro macro, int startFromPosition = 0)
	{
		List<PlayerPosition> positions = macro.positions;
		PlayerPosition startPosition = PlayerPosition.CurrentPosition();
		if (startFromPosition > 0 && startFromPosition < positions.Count)
		{
			positions = positions.GetRange(startFromPosition, positions.Count - startFromPosition);
		}
		float macroStartTime = Time.time;
		float macroEndTime = (float)positions.Count * macro.macroStepDuration;
		while (Time.time < macroStartTime + macroEndTime)
		{
			if (Main.rightTrigger < 0.5f)
			{
				StopMacro();
				yield break;
			}
			float elapsed = Time.time - macroStartTime;
			float stepElapsed = elapsed % macro.macroStepDuration;
			int currentMacroPosition = Mathf.FloorToInt(elapsed / macro.macroStepDuration);
			currentMacroPosition = Mathf.Clamp(currentMacroPosition, 0, positions.Count);
			PlayerPosition lastPosition = ((currentMacroPosition - 1 < 0) ? startPosition : positions[currentMacroPosition - 1]);
			PlayerPosition currentPosition = positions[currentMacroPosition];
			float t = stepElapsed / macro.macroStepDuration;
			Vector3 position = lastPosition.position.Lerp(currentPosition.position, t);
			Main.TeleportPlayer(position);
			((Component)VRRig.LocalRig).transform.position = position;
			GorillaTagger.Instance.rigidbody.linearVelocity = lastPosition.velocity.Lerp(currentPosition.velocity, t);
			GorillaTagger.Instance.leftHandTransform.position = lastPosition.leftHand.position.Lerp(currentPosition.leftHand.position, t);
			GorillaTagger.Instance.leftHandTransform.rotation = lastPosition.leftHand.rotation.Lerp(currentPosition.leftHand.rotation, t);
			GorillaTagger.Instance.rightHandTransform.position = lastPosition.rightHand.position.Lerp(currentPosition.rightHand.position, t);
			GorillaTagger.Instance.rightHandTransform.rotation = lastPosition.rightHand.rotation.Lerp(currentPosition.rightHand.rotation, t);
			((ControllerInputPoller)ControllerInputPoller.instance).leftControllerGripFloat = Mathf.Lerp(lastPosition.leftGrip ? 1f : 0f, currentPosition.leftGrip ? 1f : 0f, t);
			((ControllerInputPoller)ControllerInputPoller.instance).rightControllerGripFloat = Mathf.Lerp(lastPosition.rightGrip ? 1f : 0f, currentPosition.rightGrip ? 1f : 0f, t);
			((ControllerInputPoller)ControllerInputPoller.instance).leftControllerIndexFloat = Mathf.Lerp(lastPosition.leftTrigger ? 1f : 0f, currentPosition.leftTrigger ? 1f : 0f, t);
			NotificationManager.information["Macro Time"] = $"{macroEndTime - elapsed:F1}s";
			NotificationManager.information["Macro Name"] = macro.name;
			yield return null;
			if (currentMacroPosition + (int)(1f / macro.macroStepDuration) < positions.Count)
			{
				PlayerPosition futurePosition = positions[currentMacroPosition + (int)(1f / macro.macroStepDuration)];
				VisualizePositionCoroutine(futurePosition, Color.cyan);
			}
			else
			{
				RemovePosition(Color.cyan);
			}
			List<PlayerPosition> list = positions;
			VisualizePositionCoroutine(list[list.Count - 1], Color.red);
		}
		StopMacro();
	}

	public static void StopMacro()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (activeMacro != null)
		{
			((MonoBehaviour)CoroutineManager.instance).StopCoroutine(activeMacro);
			activeMacro = null;
		}
		NotificationManager.information.Remove("Macro Time");
		NotificationManager.information.Remove("Macro Name");
		RemovePosition(Color.cyan);
		RemovePosition(Color.red);
	}

	public static void VisualizePlayerPosition(PlayerPosition position, Color color, float alpha = 0.15f)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		Visuals.VisualizeCube(position.position, Quaternion.LookRotation(position.velocity), new Vector3(0.1f, 0.1f, 0.25f), color, -39228393L, alpha);
		Visuals.VisualizeCube(position.position + ((Vector3)(ref position.velocity)).normalized * 0.125f, Quaternion.LookRotation(position.velocity), new Vector3(0.15f, 0.15f, 0.05f), color, -48492012L, alpha);
		Visuals.VisualizeAura(position.leftHand.position, 0.15f, color, null, alpha);
		Visuals.VisualizeAura(position.rightHand.position, 0.15f, color, null, alpha);
	}

	public static void VisualizePositionCoroutine(PlayerPosition position, Color color)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if (!positions.TryGetValue(color, out (GameObject, GameObject, GameObject) value))
		{
			value = (Visuals.VisualizeCubeObject(position.position, Quaternion.LookRotation(position.velocity), new Vector3(0.1f, 0.1f, 0.25f), color), Visuals.VisualizeAuraObject(position.leftHand.position, 0.15f, color), Visuals.VisualizeAuraObject(position.rightHand.position, 0.15f, color));
			positions[color] = value;
		}
		value.Item1.transform.position = position.position;
		value.Item1.transform.rotation = Quaternion.LookRotation(position.velocity);
		value.Item2.transform.position = position.leftHand.position;
		value.Item3.transform.position = position.rightHand.position;
	}

	public static void RemovePosition(Color color)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (positions.TryGetValue(color, out (GameObject, GameObject, GameObject) value))
		{
			Object.Destroy((Object)(object)value.Item1);
			Object.Destroy((Object)(object)value.Item2);
			Object.Destroy((Object)(object)value.Item3);
			positions.Remove(color);
		}
	}

	public static void ExecuteMacroButton(Macro macro)
	{
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		didMacro = midpointMacros && (didMacro ? (Main.rightTrigger >= 0.5f) : (activeMacro != null));
		if (Main.rightTrigger < 0.5f || activeMacro != null || didMacro)
		{
			return;
		}
		int num = 0;
		if (midpointMacros)
		{
			num = (from x in macro.positions.Select((PlayerPosition position2, int index) => new
				{
					position = position2,
					index = index,
					distance = Vector3.Distance(((Component)GorillaTagger.Instance.bodyCollider).transform.position, position2.position)
				})
				orderby x.distance
				select x).FirstOrDefault().index;
		}
		PlayerPosition position = macro.positions[num];
		if (frameStepper)
		{
			if (Main.rightTriggerPressed && !frameStepperNotified)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=green>MACRO</color><color=grey>]</color> Frame Stepper is on. Hit the A button to progress instead.");
				frameStepperNotified = true;
			}
			if (!frameCompleted && Main.rightPrimary)
			{
				frameCompleted = true;
			}
			if (!frameCompleted)
			{
				VisualizePlayerPosition(position, Color.white, 0.05f);
				return;
			}
			frameCompleted = false;
		}
		int num2;
		if (directionBased)
		{
			Vector3 linearVelocity = GorillaTagger.Instance.rigidbody.linearVelocity;
			if (((Vector3)(ref linearVelocity)).magnitude > 2f)
			{
				Vector3 normalized = ((Vector3)(ref position.velocity)).normalized;
				linearVelocity = GorillaTagger.Instance.rigidbody.linearVelocity;
				num2 = ((Vector3.Angle(normalized, ((Vector3)(ref linearVelocity)).normalized) < 70f) ? 1 : 0);
			}
			else
			{
				num2 = 0;
			}
		}
		else
		{
			num2 = 1;
		}
		bool flag = (byte)num2 != 0;
		VisualizePlayerPosition(position, flag ? Main.buttonColors[1].GetCurrentColor() : Color.white, flag ? 0.15f : 0.05f);
		if (flag)
		{
			Visuals.VisualizeAura(position.position, 1f, Main.buttonColors[1].GetCurrentColor(), null, 0.05f);
			if (Vector3.Distance(((Component)GorillaTagger.Instance.bodyCollider).transform.position, position.position) < 1f)
			{
				activeMacro = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(PlayMacro(macro, num));
			}
		}
	}

	public static void ChangeWallWalkStrength(bool positive = true)
	{
		float[] array = new float[5] { 2f, 5f, 9.81f, 15f, 50f };
		string[] array2 = new string[5] { "Very Weak", "Weak", "Normal", "Strong", "Very Strong" };
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				wallWalkStrengthIndex++;
			}
			else
			{
				wallWalkStrengthIndex--;
			}
		}
		wallWalkStrengthIndex %= array.Length;
		if (wallWalkStrengthIndex < 0)
		{
			wallWalkStrengthIndex = array.Length - 1;
		}
		wallWalkStrength = array[wallWalkStrengthIndex];
		Buttons.GetIndex("Change Wall Walk Strength").overlapText = "Change Wall Walk Strength <color=grey>[</color><color=green>" + array2[wallWalkStrengthIndex] + "</color><color=grey>]</color>";
	}

	public static void WallWalk()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (GTPlayer.Instance.IsHandTouching(true) || GTPlayer.Instance.IsHandTouching(false))
		{
			RaycastHit lastHitInfoHand = GTPlayer.Instance.lastHitInfoHand;
			walkPos = ((RaycastHit)(ref lastHitInfoHand)).point;
			walkNormal = ((RaycastHit)(ref lastHitInfoHand)).normal;
		}
		bool flag = ((!bothWallWalk) ? (leftWallWalk ? Main.leftGrab : Main.rightGrab) : (Main.leftGrab || Main.rightGrab));
		if (walkPos != Vector3.zero && flag)
		{
			GorillaTagger.Instance.rigidbody.AddForce(walkNormal * (0f - wallWalkStrength), (ForceMode)5);
			ZeroGravity();
		}
	}

	public static void LegitimateWallWalk()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.2f;
		float num2 = -2f;
		if (Main.leftGrab && (leftWallWalk || Main.bothHands))
		{
			RaycastHit lastHitInfoHand = GTPlayer.Instance.lastHitInfoHand;
			RaycastHit val = default(RaycastHit);
			if (Physics.Raycast(ControllerUtilities.GetTrueLeftHand().position, -((RaycastHit)(ref lastHitInfoHand)).normal, ref val, num, LayerMask.op_Implicit(GTPlayer.Instance.locomotionEnabledLayers)))
			{
				GorillaTagger.Instance.rigidbody.AddForce(((RaycastHit)(ref val)).normal * num2, (ForceMode)5);
			}
		}
		if (Main.rightGrab && (!leftWallWalk || Main.bothHands))
		{
			RaycastHit lastHitInfoHand2 = GTPlayer.Instance.lastHitInfoHand;
			RaycastHit val2 = default(RaycastHit);
			if (Physics.Raycast(ControllerUtilities.GetTrueRightHand().position, -((RaycastHit)(ref lastHitInfoHand2)).normal, ref val2, num, LayerMask.op_Implicit(GTPlayer.Instance.locomotionEnabledLayers)))
			{
				GorillaTagger.Instance.rigidbody.AddForce(((RaycastHit)(ref val2)).normal * num2, (ForceMode)5);
			}
		}
	}

	public static void SpiderWalk()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if (GTPlayer.Instance.IsHandTouching(true) || GTPlayer.Instance.IsHandTouching(false))
		{
			RaycastHit lastHitInfoHand = GTPlayer.Instance.lastHitInfoHand;
			walkPos = ((RaycastHit)(ref lastHitInfoHand)).point;
			walkNormal = ((RaycastHit)(ref lastHitInfoHand)).normal;
		}
		if (walkPos != Vector3.zero)
		{
			GorillaTagger.Instance.rigidbody.AddForce(walkNormal * -9.81f, (ForceMode)5);
			GTPlayer.Instance.GetControllerTransform(false).parent.rotation = Quaternion.Lerp(GTPlayer.Instance.GetControllerTransform(false).parent.rotation, Quaternion.LookRotation(walkNormal) * Quaternion.Euler(90f, 0f, 0f), Time.deltaTime);
			ZeroGravity();
		}
	}

	public static void TeleportToRandom()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Main.closePosition = Vector3.zero;
		Main.TeleportPlayer(((Component)RigUtilities.GetRandomVRRig(includeSelf: false)).transform.position);
		GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
	}

	public static void TeleportToPlayer(NetPlayer plr)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Main.TeleportPlayer(RigUtilities.GetVRRigFromPlayer(plr).headMesh.transform.position);
	}

	public static void ExitTeleportToMap()
	{
		Buttons.CurrentCategoryName = "Movement Mods";
		Main.pageNumber = rememberPageNumber;
	}

	public static void EnterTeleportToMap()
	{
		rememberPageNumber = Main.pageNumber;
		List<ButtonInfo> list = new List<ButtonInfo>();
		list.Add(new ButtonInfo
		{
			buttonText = "Exit Teleport to Map",
			method = ExitTeleportToMap,
			isTogglable = false,
			toolTip = "Returns you back to the movement mods."
		});
		List<ButtonInfo> list2 = list;
		string[][] array = mapData;
		foreach (string[] Data in array)
		{
			list2.Add(new ButtonInfo
			{
				buttonText = "TeleportMap" + list2.Count,
				overlapText = Data[0],
				method = delegate
				{
					TeleportToMap(Data[1], Data[2]);
				},
				isTogglable = false,
				toolTip = "Teleports you to the " + Data[0] + " map."
			});
		}
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list2.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void TeleportToMap(string zone, string pos)
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		if (zone == "VSTUMP")
		{
			VirtualStumpTeleporter component = Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/VirtualStump_HeadsetTeleporter/TeleporterTrigger").GetComponent<VirtualStumpTeleporter>();
			((Component)((Component)component).gameObject.transform.parent.parent.parent.parent.parent.parent).gameObject.SetActive(true);
			((Component)((Component)component).gameObject.transform.parent.parent.parent.parent).gameObject.SetActive(true);
			component.TeleportPlayer();
			return;
		}
		GameObject obj = Main.GetObject(zone);
		if (obj != null)
		{
			GorillaSetZoneTrigger component2 = obj.GetComponent<GorillaSetZoneTrigger>();
			if (component2 != null)
			{
				((GorillaTriggerBox)component2).OnBoxTriggered();
			}
		}
		GameObject obj2 = Main.GetObject(pos);
		Main.TeleportPlayer((obj2 != null) ? obj2.transform.position : ((Component)VRRig.LocalRig).transform.position);
	}

	public static void TeleportGun()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true) && !previousTeleportTrigger)
			{
				Main.closePosition = Vector3.zero;
				Main.TeleportPlayer(item.transform.position + Vector3.up);
				GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
			}
			previousTeleportTrigger = Main.GetGunInput(isShooting: true);
		}
	}

	public static void Airstrike()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true) && !previousTeleportTrigger)
			{
				GorillaTagger.Instance.rigidbody.linearVelocity = new Vector3(0f, -20f, 0f);
				Main.TeleportPlayer(item.transform.position + new Vector3(0f, 30f, 0f));
				GorillaTagger.Instance.rigidbody.linearVelocity = new Vector3(0f, -20f, 0f);
			}
			previousTeleportTrigger = Main.GetGunInput(isShooting: true);
		}
	}

	public static void Checkpoint()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab)
		{
			if ((Object)(object)CheckPoint == (Object)null)
			{
				CheckPoint = GameObject.CreatePrimitive((PrimitiveType)0);
				Object.Destroy((Object)(object)CheckPoint.GetComponent<SphereCollider>());
				CheckPoint.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
			}
			CheckPoint.transform.position = GorillaTagger.Instance.rightHandTransform.position;
		}
		if ((Object)(object)CheckPoint != (Object)null)
		{
			if (Main.rightPrimary)
			{
				CheckPoint.GetComponent<Renderer>().material.color = Main.backgroundColor.GetColor(0);
				Main.TeleportPlayer(CheckPoint.transform.position);
				GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
			}
			else
			{
				CheckPoint.GetComponent<Renderer>().material.color = Main.buttonColors[0].GetColor(0);
			}
		}
	}

	public static void DisableCheckpoint()
	{
		if ((Object)(object)CheckPoint != (Object)null)
		{
			Object.Destroy((Object)(object)CheckPoint);
			CheckPoint = null;
		}
	}

	public static void AdvancedCheckpoints()
	{
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Expected O, but got Unknown
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab)
		{
			bool flag = false;
			using (IEnumerator<GameObject> enumerator = checkpoints.Where((GameObject checkpoint) => Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, checkpoint.transform.position) < 0.2f).GetEnumerator())
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
				val.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
				val.transform.position = GorillaTagger.Instance.rightHandTransform.position;
				val.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
				val.GetComponent<Renderer>().material.color = Main.backgroundColor.GetCurrentColor();
				GameObject val2 = new GameObject("Label");
				val2.transform.parent = val.transform;
				val2.transform.localPosition = Vector3.zero;
				TextMeshPro val3 = val2.AddComponent<TextMeshPro>();
				Renderer component = ((Component)val3).GetComponent<Renderer>();
				((Component)component).GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
				((TMP_Text)val3).fontSize = 1.2f;
				((TMP_Text)(object)val3).SafeSetFont(Main.activeFont);
				((TMP_Text)(object)val3).SafeSetFontStyle(Main.activeFontStyle);
				((TMP_Text)val3).alignment = (TextAlignmentOptions)514;
				((TMP_Text)(object)val3).Chams();
				((Graphic)val3).color = Color.white;
				((TMP_Text)val3).text = (checkpoints.Count + 1).ToString();
				component.material.renderQueue = val.GetComponent<Renderer>().material.renderQueue + 2;
				checkpoints.Add(val);
			}
		}
		if (Main.rightTrigger > 0.5f)
		{
			foreach (GameObject item in from checkpoint in checkpoints.ToList()
				where Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, checkpoint.transform.position) < 0.2f
				select checkpoint)
			{
				checkpoints.Remove(item);
				Object.Destroy((Object)(object)item);
			}
		}
		if (Main.rightPrimary)
		{
			Main.TeleportPlayer(checkpoints[selectedCheckpoint].transform.position);
		}
		foreach (GameObject checkpoint in checkpoints)
		{
			checkpoint.GetComponent<Renderer>().material.color = Main.backgroundColor.GetCurrentColor();
			GameObject gameObject = ((Component)checkpoint.transform.Find("Label")).gameObject;
			gameObject.transform.LookAt(((Component)Camera.main).transform.position);
			gameObject.transform.Rotate(0f, 180f, 0f);
		}
		if (Mathf.Abs(Main.rightJoystick.y) > 0.5f && Time.time > selectedCheckpointDelay)
		{
			selectedCheckpointDelay = Time.time + 0.2f;
			selectedCheckpoint += ((Main.rightJoystick.y > 0f) ? 1 : (-1));
			if (selectedCheckpoint < 0)
			{
				selectedCheckpoint = checkpoints.Count - 1;
			}
		}
		if (selectedCheckpoint < 0 && checkpoints.Count > 0)
		{
			selectedCheckpoint = 0;
		}
		if (selectedCheckpoint > checkpoints.Count - 1)
		{
			selectedCheckpoint = 0;
		}
		Visuals.GetLabel("CheckpointLabel", leftHand: false, (selectedCheckpoint + 1).ToString(), Color.white);
	}

	public static void DisableAdvancedCheckpoints()
	{
		selectedCheckpoint = 0;
		foreach (GameObject checkpoint in checkpoints)
		{
			Object.Destroy((Object)(object)checkpoint);
		}
		checkpoints.Clear();
	}

	public static void Bomb()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
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
		if ((Object)(object)BombObject != (Object)null)
		{
			if (Main.rightPrimary)
			{
				Vector3 val = ((Component)GorillaTagger.Instance.bodyCollider).transform.position - BombObject.transform.position;
				((Vector3)(ref val)).Normalize();
				Rigidbody rigidbody = GorillaTagger.Instance.rigidbody;
				rigidbody.linearVelocity += 25f * val;
				Object.Destroy((Object)(object)BombObject);
				BombObject = null;
			}
			else
			{
				BombObject.GetComponent<Renderer>().material.color = Main.buttonColors[0].GetColor(0);
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

	public static void EnderPearl()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab || Main.leftGrab)
		{
			if ((Object)(object)pearl == (Object)null)
			{
				pearl = GameObject.CreatePrimitive((PrimitiveType)3);
				Object.Destroy((Object)(object)pearl.GetComponent<Collider>());
				pearl.transform.localScale = new Vector3(0.1f, 0.1f, 0.01f);
				if ((Object)(object)pearlmat == (Object)null)
				{
					pearlmat = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
					{
						color = Color.white
					};
					if ((Object)(object)pearltxt == (Object)null)
					{
						pearltxt = AssetUtilities.LoadTextureFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Images/Mods/Movement/pearl.png", "Images/Mods/Movement/pearl.png");
						((Texture)pearltxt).filterMode = (FilterMode)0;
						((Texture)pearltxt).wrapMode = (TextureWrapMode)1;
					}
					pearlmat.mainTexture = (Texture)(object)pearltxt;
					pearlmat.SetFloat("_Surface", 1f);
					pearlmat.SetFloat("_Blend", 0f);
					pearlmat.SetFloat("_SrcBlend", 5f);
					pearlmat.SetFloat("_DstBlend", 10f);
					pearlmat.SetFloat("_ZWrite", 0f);
					pearlmat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
					pearlmat.renderQueue = 3000;
				}
				pearl.GetComponent<Renderer>().material = pearlmat;
			}
			if ((Object)(object)pearl.GetComponent<Rigidbody>() != (Object)null)
			{
				Object.Destroy((Object)(object)pearl.GetComponent<Rigidbody>());
			}
			isrighthandedpearl = Main.rightGrab;
			pearl.transform.position = (Main.rightGrab ? GorillaTagger.Instance.rightHandTransform.position : GorillaTagger.Instance.leftHandTransform.position);
		}
		else if ((Object)(object)pearl != (Object)null)
		{
			if ((Object)(object)pearl.GetComponent<Rigidbody>() == (Object)null)
			{
				Component obj = pearl.AddComponent(typeof(Rigidbody));
				Rigidbody val = (Rigidbody)(object)((obj is Rigidbody) ? obj : null);
				val.linearVelocity = (isrighthandedpearl ? GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0f, false) : GTPlayer.Instance.LeftHand.velocityTracker.GetAverageVelocity(true, 0f, false));
			}
			RaycastHit val2 = default(RaycastHit);
			Physics.Raycast(pearl.transform.position, pearl.GetComponent<Rigidbody>().linearVelocity, ref val2, 0.25f, LayerMask.op_Implicit(GTPlayer.Instance.locomotionEnabledLayers));
			if ((Object)(object)((RaycastHit)(ref val2)).collider != (Object)null)
			{
				Main.TeleportPlayer(pearl.transform.position);
				if (PhotonNetwork.InRoom)
				{
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", (RpcTarget)0, new object[3] { 84, true, 999999f });
				}
				else
				{
					VRRig.LocalRig.PlayHandTapLocal(84, true, 999999f);
				}
				Main.RPCProtection();
				Object.Destroy((Object)(object)pearl);
			}
		}
		if ((Object)(object)pearl != (Object)null)
		{
			pearl.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
			Rigidbody component = pearl.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.AddForce(Vector3.up * (Time.deltaTime * (6.66f / Time.deltaTime)), (ForceMode)5);
			}
		}
	}

	public static void DestroyEnderPearl()
	{
		if ((Object)(object)pearl != (Object)null)
		{
			Object.Destroy((Object)(object)pearl);
		}
	}

	public static void SpeedBoost()
	{
		float num = jspeed;
		float num2 = jmulti;
		if (Buttons.GetIndex("Factored Speed Boost").enabled)
		{
			num = num / 6.5f * GTPlayer.Instance.maxJumpSpeed;
			num2 = num2 / 1.1f * GTPlayer.Instance.jumpMultiplier;
		}
		if (!Buttons.GetIndex("Disable Max Speed Modification").enabled)
		{
			GTPlayer.Instance.maxJumpSpeed = num;
		}
		GTPlayer.Instance.jumpMultiplier = num2;
	}

	public static void FunMove()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Rigidbody rigidbody = GorillaTagger.Instance.rigidbody;
		rigidbody.linearVelocity += GorillaTagger.Instance.rigidbody.linearVelocity * Time.deltaTime;
	}

	public static void DynamicSpeedBoost()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		bool isTagged = VRRig.LocalRig.IsTagged();
		VRRig val = (from rig in VRRigCache.ActiveRigs
			where (Object)(object)rig != (Object)null && !rig.isLocal && (isTagged ? (!rig.IsTagged()) : rig.IsTagged())
			orderby Vector3.Distance(((Component)rig).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position)
			select rig).FirstOrDefault();
		float num = (((Object)(object)val == (Object)null) ? float.MaxValue : Vector3.Distance(((Component)GorillaTagger.Instance.bodyCollider).transform.position, ((Component)val).transform.position));
		if (num < 15f)
		{
			float num2 = jspeed;
			float num3 = jmulti;
			if (Buttons.GetIndex("Factored Speed Boost").enabled)
			{
				num2 = num2 / 6.5f * GTPlayer.Instance.maxJumpSpeed;
				num3 = num3 / 1.1f * GTPlayer.Instance.jumpMultiplier;
			}
			num2 = Mathf.Lerp(GTPlayer.Instance.maxJumpSpeed, num2, Mathf.Clamp(num, 1f, 15f) / 15f);
			num3 = Mathf.Lerp(GTPlayer.Instance.jumpMultiplier, num3, Mathf.Clamp(num, 1f, 15f) / 15f);
			if (!Buttons.GetIndex("Disable Max Speed Modification").enabled)
			{
				GTPlayer.Instance.maxJumpSpeed = num2;
			}
			GTPlayer.Instance.jumpMultiplier = num3;
		}
	}

	public static void AlwaysMaxVelocity()
	{
		if (Buttons.GetIndex("Uncap Max Velocity").enabled)
		{
			Main.Toggle("Uncap Max Velocity");
		}
		else
		{
			GTPlayer.Instance.jumpMultiplier = 99999f;
		}
	}

	public static void DisableVelocityCap()
	{
		playspace = Main.GetAllType<Playspace>(5f).FirstOrDefault();
		((Behaviour)playspace).enabled = false;
	}

	public static void UpdateClipColliders(bool enabled)
	{
		MeshCollider[] array = Resources.FindObjectsOfTypeAll<MeshCollider>();
		foreach (MeshCollider val in array)
		{
			((Collider)val).enabled = enabled;
		}
	}

	public static void Noclip()
	{
		bool num;
		if (!Buttons.GetIndex("Grip Noclip").enabled)
		{
			if (Main.rightTrigger > 0.5f)
			{
				goto IL_003d;
			}
			num = Buttons.GetIndex("Constant Noclip").enabled;
		}
		else
		{
			num = Main.rightGrab;
		}
		if (!num)
		{
			if (noclip)
			{
				noclip = false;
				UpdateClipColliders(enabled: true);
			}
			return;
		}
		goto IL_003d;
		IL_003d:
		if (!noclip)
		{
			noclip = true;
			UpdateClipColliders(enabled: false);
		}
	}

	public static void RemoveForestColliders()
	{
		Transform transform = Main.GetObject("Environment Objects/LocalObjects_Prefab/ForestToHoverboard/TurnOnInForestAndHoverboard/ForestDome_CollisionOnly").transform;
		if (!((Object)(object)transform == (Object)null))
		{
			for (int i = 2; i < 4; i++)
			{
				GameObject gameObject = ((Component)((Component)transform).transform.GetChild(i)).gameObject;
				gameObject.SetActive(false);
				forestColliders.Add(gameObject);
			}
		}
	}

	public static void RestoreForestColliders()
	{
		foreach (GameObject forestCollider in forestColliders)
		{
			forestCollider.SetActive(true);
		}
		forestColliders.Clear();
	}

	public static void Invisible()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		bool rightSecondary = Main.rightSecondary;
		if (Buttons.GetIndex("Non-Togglable Invisible").enabled)
		{
			invisMonke = rightSecondary;
		}
		if (invisMonke)
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position - Vector3.up * 99999f;
		}
		if (rightSecondary && !lastHit2)
		{
			invisMonke = !invisMonke;
			if (invisMonke)
			{
				wasDisabledAlready = ((Behaviour)VRRig.LocalRig).enabled;
			}
			else
			{
				((Behaviour)VRRig.LocalRig).enabled = wasDisabledAlready;
			}
		}
		lastHit2 = rightSecondary;
	}

	public static void Ghost()
	{
		bool rightPrimary = Main.rightPrimary;
		if (Buttons.GetIndex("Non-Togglable Ghost").enabled)
		{
			ghostMonke = rightPrimary;
		}
		((Behaviour)VRRig.LocalRig).enabled = !ghostMonke;
		if (rightPrimary && !lastHit)
		{
			ghostMonke = !ghostMonke;
		}
		lastHit = rightPrimary;
	}

	public static void EnableRig()
	{
		((Behaviour)VRRig.LocalRig).enabled = true;
		Main.ghostException = false;
	}

	public static void RigGun()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true))
			{
				((Behaviour)VRRig.LocalRig).enabled = false;
				((Component)VRRig.LocalRig).transform.position = item.transform.position + new Vector3(0f, 1f, 0f);
			}
			else
			{
				((Behaviour)VRRig.LocalRig).enabled = true;
			}
		}
	}

	public static void GrabRig()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab)
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = GorillaTagger.Instance.rightHandTransform.position;
			((Component)VRRig.LocalRig).transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
		}
		else
		{
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void EnableSpazRig()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		Main.ghostException = true;
		offsetLH = VRRig.LocalRig.leftHand.trackingPositionOffset;
		offsetRH = VRRig.LocalRig.rightHand.trackingPositionOffset;
		offsetH = VRRig.LocalRig.head.trackingPositionOffset;
	}

	public static void SpazRig()
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightPrimary)
		{
			float range = 0.1f;
			Main.ghostException = true;
			VRRig.LocalRig.leftHand.trackingPositionOffset = offsetLH + RandomUtilities.RandomVector3(range);
			VRRig.LocalRig.rightHand.trackingPositionOffset = offsetRH + RandomUtilities.RandomVector3(range);
			VRRig.LocalRig.head.trackingPositionOffset = offsetH + RandomUtilities.RandomVector3(range);
		}
		else
		{
			Main.ghostException = false;
			VRRig.LocalRig.leftHand.trackingPositionOffset = offsetLH;
			VRRig.LocalRig.rightHand.trackingPositionOffset = offsetRH;
			VRRig.LocalRig.head.trackingPositionOffset = offsetH;
		}
	}

	public static void DisableSpazRig()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		Main.ghostException = false;
		VRRig.LocalRig.leftHand.trackingPositionOffset = offsetLH;
		VRRig.LocalRig.rightHand.trackingPositionOffset = offsetRH;
		VRRig.LocalRig.head.trackingPositionOffset = offsetH;
	}

	public static void SpazHands()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightPrimary)
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + new Vector3(0f, 0.15f, 0f);
			((Component)VRRig.LocalRig).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation;
			((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = ((Component)GorillaTagger.Instance.headCollider).transform.rotation;
			((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = RandomUtilities.RandomQuaternion();
			((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = RandomUtilities.RandomQuaternion();
			((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = GorillaTagger.Instance.leftHandTransform.position + ((Component)VRRig.LocalRig.leftHand.rigTarget).transform.forward * 3f;
			((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = GorillaTagger.Instance.rightHandTransform.position + ((Component)VRRig.LocalRig.rightHand.rigTarget).transform.forward * 3f;
		}
		else
		{
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void SpiderCrawl()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = ((Component)GorillaTagger.Instance.headCollider).transform;
		Quaternion rotation = ((Component)GorillaTagger.Instance.headCollider).transform.rotation;
		transform.rotation = Quaternion.Euler(-270f, ((Quaternion)(ref rotation)).eulerAngles.y, 0f);
	}

	public static void UpwardsBody()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = ((Component)GorillaTagger.Instance.headCollider).transform;
		Quaternion rotation = ((Component)GorillaTagger.Instance.headCollider).transform.rotation;
		float y = ((Quaternion)(ref rotation)).eulerAngles.y;
		rotation = ((Component)GorillaTagger.Instance.headCollider).transform.rotation;
		transform.rotation = Quaternion.Euler(-180f, y, ((Quaternion)(ref rotation)).eulerAngles.z);
	}

	public static void ControlBodyRotation()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightJoystickClick || Main.leftJoystickClick || UnityInput.GetKeyDown((Key)2))
		{
			bodyJoystickRot = null;
		}
		Vector2 rightJoystick = Main.rightJoystick;
		Vector2 leftJoystick = Main.leftJoystick;
		Vector2 val = ((((Vector2)(ref leftJoystick)).sqrMagnitude > ((Vector2)(ref rightJoystick)).sqrMagnitude) ? leftJoystick : rightJoystick);
		Vector2 zero = Vector2.zero;
		if (UnityInput.GetKey((Key)61))
		{
			zero.x -= 1f;
		}
		if (UnityInput.GetKey((Key)62))
		{
			zero.x += 1f;
		}
		if (UnityInput.GetKey((Key)63))
		{
			zero.y += 1f;
		}
		if (UnityInput.GetKey((Key)64))
		{
			zero.y -= 1f;
		}
		if (((Vector2)(ref zero)).sqrMagnitude > 0f)
		{
			((Vector2)(ref zero)).Normalize();
		}
		if (((Vector2)(ref zero)).sqrMagnitude > ((Vector2)(ref val)).sqrMagnitude)
		{
			val = zero;
		}
		float num = 200f;
		float num2 = 200f;
		if ((double)((Vector2)(ref val)).sqrMagnitude > 0.0025)
		{
			if (!bodyJoystickRot.HasValue)
			{
				bodyJoystickRot = ((Component)VRRig.LocalRig).transform.rotation;
			}
			float num3 = val.x * num * (float)Buttons.GetIndex("Joystick Rotation Speed").GetValue<int>() * Time.deltaTime;
			float num4 = (0f - val.y) * num2 * (float)Buttons.GetIndex("Joystick Rotation Speed").GetValue<int>() * Time.deltaTime;
			Quaternion val2 = bodyJoystickRot.Value;
			val2 = Quaternion.AngleAxis(num3, Vector3.up) * val2;
			Vector3 val3 = val2 * Vector3.right;
			val2 = Quaternion.AngleAxis(num4, val3) * val2;
			((Quaternion)(ref val2)).Normalize();
			bodyJoystickRot = val2;
		}
		if (bodyJoystickRot.HasValue)
		{
			((Component)VRRig.LocalRig).transform.rotation = bodyJoystickRot.Value;
		}
	}

	public static void SpazRealHands()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightPrimary)
		{
			GTPlayer.Instance.GetControllerTransform(true).rotation = RandomUtilities.RandomQuaternion();
			GTPlayer.Instance.GetControllerTransform(true).position = GorillaTagger.Instance.leftHandTransform.position + GTPlayer.Instance.GetControllerTransform(true).forward * 3f;
			GTPlayer.Instance.GetControllerTransform(false).rotation = RandomUtilities.RandomQuaternion();
			GTPlayer.Instance.GetControllerTransform(false).position = GorillaTagger.Instance.rightHandTransform.position + GTPlayer.Instance.GetControllerTransform(false).forward * 3f;
		}
		else
		{
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void FreezeRigLimbs()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		((Behaviour)VRRig.LocalRig).enabled = false;
		((Component)VRRig.LocalRig).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + new Vector3(0f, 0.15f, 0f);
		((Component)VRRig.LocalRig).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation;
	}

	public static void FixRigHandRotation()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = ((Component)VRRig.LocalRig.leftHand.rigTarget).transform;
		transform.rotation *= Quaternion.Euler(VRRig.LocalRig.leftHand.trackingRotationOffset);
		Transform transform2 = ((Component)VRRig.LocalRig.rightHand.rigTarget).transform;
		transform2.rotation *= Quaternion.Euler(VRRig.LocalRig.rightHand.trackingRotationOffset);
	}

	public static void FreezeRigBody()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		((Behaviour)VRRig.LocalRig).enabled = false;
		var (position, rotation, _, _, _) = ControllerUtilities.GetTrueLeftHand();
		var (position2, rotation2, _, _, _) = ControllerUtilities.GetTrueRightHand();
		((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = position;
		((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = position2;
		((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = rotation;
		((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = rotation2;
		FixRigHandRotation();
		((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = ((Component)GorillaTagger.Instance.headCollider).transform.rotation;
	}

	public static void FreezeRig()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!startPosition.HasValue)
		{
			startPosition = ((Component)VRRig.LocalRig).transform.position;
		}
		((Behaviour)VRRig.LocalRig).enabled = true;
		VRRig.LocalRig.PostTick();
		((Component)VRRig.LocalRig).transform.position = startPosition.Value;
		((Behaviour)VRRig.LocalRig).enabled = false;
	}

	public static void ParalyzeRig()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		((Behaviour)VRRig.LocalRig).enabled = false;
		((Component)VRRig.LocalRig).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + new Vector3(0f, 0.15f, 0f);
		((Component)VRRig.LocalRig).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation;
		((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = ((Component)GorillaTagger.Instance.headCollider).transform.rotation;
		((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + ((Component)GorillaTagger.Instance.bodyCollider).transform.right * -0.08f + ((Component)GorillaTagger.Instance.bodyCollider).transform.up * 0.12f;
		((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + ((Component)GorillaTagger.Instance.bodyCollider).transform.right * 0.08f + ((Component)GorillaTagger.Instance.bodyCollider).transform.up * 0.12f;
		((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation * Quaternion.Euler(0f, 180f, 180f);
		((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation * Quaternion.Euler(0f, 180f, 180f);
	}

	public static void ChickenRig()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		((Behaviour)VRRig.LocalRig).enabled = false;
		((Component)VRRig.LocalRig).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + new Vector3(0f, 0.15f, 0f);
		((Component)VRRig.LocalRig).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation;
		((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = ((Component)GorillaTagger.Instance.headCollider).transform.rotation;
		((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + ((Component)GorillaTagger.Instance.bodyCollider).transform.right * 0.2f + ((Component)GorillaTagger.Instance.bodyCollider).transform.up * -0.2f;
		((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + ((Component)GorillaTagger.Instance.bodyCollider).transform.right * -0.2f + ((Component)GorillaTagger.Instance.bodyCollider).transform.up * -0.2f;
		((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation;
		((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation;
	}

	public static void AmputateRig()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		((Behaviour)VRRig.LocalRig).enabled = false;
		((Component)VRRig.LocalRig).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + new Vector3(0f, 0.15f, 0f);
		((Component)VRRig.LocalRig).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation;
		((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation * Quaternion.Euler(160f, 90f, 0f);
		((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + ((Component)GorillaTagger.Instance.bodyCollider).transform.right * -0.08f + ((Component)GorillaTagger.Instance.bodyCollider).transform.up * 0.12f;
		((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + ((Component)GorillaTagger.Instance.bodyCollider).transform.right * 0.08f + ((Component)GorillaTagger.Instance.bodyCollider).transform.up * 0.12f;
		((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation * Quaternion.Euler(0f, 180f, 180f);
		((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation * Quaternion.Euler(0f, 180f, 180f);
	}

	public static void DecapitateRigUpdate()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation * Quaternion.Euler(160f, 90f, 0f);
	}

	public static void SetBodyPatch(bool enabled, int mode = 0)
	{
		TorsoPatch.enabled = enabled;
		TorsoPatch.mode = mode;
		if (!enabled && (Object)(object)recBodyRotary != (Object)null)
		{
			Object.Destroy((Object)(object)recBodyRotary);
		}
	}

	public static void RecRoomBody()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		SetBodyPatch(enabled: true, 3);
		if ((Object)(object)recBodyRotary == (Object)null)
		{
			recBodyRotary = new GameObject("ii_recBodyRotary");
		}
		Transform transform = recBodyRotary.transform;
		Quaternion rotation = recBodyRotary.transform.rotation;
		Quaternion rotation2 = ((Component)GorillaTagger.Instance.headCollider).transform.rotation;
		transform.rotation = Quaternion.Lerp(rotation, Quaternion.Euler(0f, ((Quaternion)(ref rotation2)).eulerAngles.y, 0f), Time.deltaTime * 6.5f);
	}

	public static void FreezeBodyRotation()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		SetBodyPatch(enabled: true, 3);
		if ((Object)(object)recBodyRotary == (Object)null)
		{
			recBodyRotary = new GameObject("ii_recBodyRotary");
		}
		Transform transform = recBodyRotary.transform;
		Quaternion rotation2;
		if (!Main.rightGrab)
		{
			Quaternion rotation = ((Component)GorillaTagger.Instance.headCollider).transform.rotation;
			rotation2 = Quaternion.Euler(0f, ((Quaternion)(ref rotation)).eulerAngles.y, 0f);
		}
		else
		{
			rotation2 = recBodyRotary.transform.rotation;
		}
		transform.rotation = rotation2;
	}

	public static void AutoDance()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightPrimary)
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			Vector3 val = ((Component)GorillaTagger.Instance.bodyCollider).transform.right * (Mathf.Cos((float)Time.frameCount / 20f) * 0.3f) + new Vector3(0f, Mathf.Abs(Mathf.Sin((float)Time.frameCount / 20f) * 0.2f), 0f);
			((Component)VRRig.LocalRig).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + new Vector3(0f, 0.15f, 0f) + val;
			((Component)VRRig.LocalRig).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation;
			((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation;
			((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + ((Component)VRRig.LocalRig).transform.forward * 0.2f + ((Component)VRRig.LocalRig).transform.right * -0.4f + ((Component)VRRig.LocalRig).transform.up * (0.3f + Mathf.Sin((float)Time.frameCount / 20f) * 0.2f);
			((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + ((Component)VRRig.LocalRig).transform.forward * 0.2f + ((Component)VRRig.LocalRig).transform.right * 0.4f + ((Component)VRRig.LocalRig).transform.up * (0.3f + Mathf.Sin((float)Time.frameCount / 20f) * -0.2f);
			((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
			((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
			FixRigHandRotation();
		}
		else
		{
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void AutoGriddy()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightPrimary)
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			Vector3 val = ((Component)VRRig.LocalRig).transform.forward * (5f * Time.deltaTime);
			((Component)VRRig.LocalRig).transform.position = ((Component)VRRig.LocalRig).transform.position + val;
			((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
			((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + ((Component)VRRig.LocalRig).transform.right * -0.33f + ((Component)VRRig.LocalRig).transform.forward * (0.5f * Mathf.Cos((float)Time.frameCount / 10f)) + ((Component)VRRig.LocalRig).transform.up * (-0.5f * Mathf.Abs(Mathf.Sin((float)Time.frameCount / 10f)));
			((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + ((Component)VRRig.LocalRig).transform.right * 0.33f + ((Component)VRRig.LocalRig).transform.forward * (0.5f * Mathf.Cos((float)Time.frameCount / 10f)) + ((Component)VRRig.LocalRig).transform.up * (-0.5f * Mathf.Abs(Mathf.Sin((float)Time.frameCount / 10f)));
			((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
			((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
			FixRigHandRotation();
		}
		else
		{
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void AutoTPose()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightPrimary)
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + new Vector3(0f, 0.15f, 0f);
			((Component)VRRig.LocalRig).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation;
			((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation;
			((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + ((Component)VRRig.LocalRig).transform.right * -1f;
			((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + ((Component)VRRig.LocalRig).transform.right * 1f;
			((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
			((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
			FixRigHandRotation();
		}
		else
		{
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void Helicopter()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightPrimary)
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			Transform transform = ((Component)VRRig.LocalRig).transform;
			transform.position += new Vector3(0f, 0.05f, 0f);
			Transform transform2 = ((Component)VRRig.LocalRig).transform;
			Quaternion rotation = ((Component)VRRig.LocalRig).transform.rotation;
			transform2.rotation = Quaternion.Euler(((Quaternion)(ref rotation)).eulerAngles + new Vector3(0f, 10f, 0f));
			((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
			((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + ((Component)VRRig.LocalRig).transform.right * -1f;
			((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + ((Component)VRRig.LocalRig).transform.right * 1f;
			((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
			((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
			FixRigHandRotation();
		}
		else
		{
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void Beyblade()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightPrimary)
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + new Vector3(0f, 0.15f, 0f);
			Transform transform = ((Component)VRRig.LocalRig).transform;
			Quaternion rotation = ((Component)VRRig.LocalRig).transform.rotation;
			transform.rotation = Quaternion.Euler(((Quaternion)(ref rotation)).eulerAngles + new Vector3(0f, 10f, 0f));
			((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
			((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + ((Component)VRRig.LocalRig).transform.right * -1f;
			((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + ((Component)VRRig.LocalRig).transform.right * 1f;
			((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
			((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
			FixRigHandRotation();
		}
		else
		{
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void StillBeyblade()
	{
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightPrimary)
		{
			if (stillBeybladeStartPos == Vector3.zero)
			{
				stillBeybladeStartPos = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + new Vector3(0f, 0.15f, 0f);
			}
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = stillBeybladeStartPos;
			Transform transform = ((Component)VRRig.LocalRig).transform;
			Quaternion rotation = ((Component)VRRig.LocalRig).transform.rotation;
			transform.rotation = Quaternion.Euler(((Quaternion)(ref rotation)).eulerAngles + new Vector3(0f, 10f, 0f));
			((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
			((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + ((Component)VRRig.LocalRig).transform.right * -1f;
			((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + ((Component)VRRig.LocalRig).transform.right * 1f;
			((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
			((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
			FixRigHandRotation();
		}
		else
		{
			stillBeybladeStartPos = Vector3.zero;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void TorsoPatch_VRRigLateUpdate()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = ((Component)VRRig.LocalRig).transform;
		transform.rotation *= Quaternion.Euler(0f, Time.time * 180f % 360f, 0f);
	}

	public static void Fan()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightPrimary)
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + new Vector3(0f, 0.15f, 0f);
			((Component)VRRig.LocalRig).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation;
			((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
			((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + (((Component)VRRig.LocalRig).transform.up * (Mathf.Cos(Time.time * 15f) * 2f) + ((Component)VRRig.LocalRig).transform.right * (Mathf.Sin(Time.time * 15f) * 2f));
			((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position - (((Component)VRRig.LocalRig).transform.up * (Mathf.Cos(Time.time * 15f) * 2f) + ((Component)VRRig.LocalRig).transform.right * (Mathf.Sin(Time.time * 15f) * 2f));
			((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
			((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
			FixRigHandRotation();
		}
		else
		{
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void GhostAnimations()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		((Behaviour)VRRig.LocalRig).enabled = false;
		if (headPos == Vector3.zero)
		{
			headPos = ((Component)GorillaTagger.Instance.headCollider).transform.position;
		}
		Quaternion val;
		if (headRot == Vector3.zero)
		{
			val = ((Component)GorillaTagger.Instance.headCollider).transform.rotation;
			headRot = ((Quaternion)(ref val)).eulerAngles;
		}
		if (handPos_L == Vector3.zero)
		{
			handPos_L = ((Component)GorillaTagger.Instance.leftHandTransform).transform.position;
		}
		if (handRot_L == Vector3.zero)
		{
			val = ((Component)GorillaTagger.Instance.leftHandTransform).transform.rotation;
			handRot_L = ((Quaternion)(ref val)).eulerAngles;
		}
		if (handPos_R == Vector3.zero)
		{
			handPos_R = ((Component)GorillaTagger.Instance.rightHandTransform).transform.position;
		}
		if (handRot_R == Vector3.zero)
		{
			val = ((Component)GorillaTagger.Instance.rightHandTransform).transform.rotation;
			handRot_R = ((Quaternion)(ref val)).eulerAngles;
		}
		float num = 0.01f;
		float num2 = 2f;
		float num3 = 0.05f;
		float num4 = 11.5f;
		if (Vector3.Distance(headPos, ((Component)GorillaTagger.Instance.headCollider).transform.position) > num3)
		{
			headPos += Vector3.Normalize(((Component)GorillaTagger.Instance.headCollider).transform.position - headPos) * num;
		}
		if (Quaternion.Angle(Quaternion.Euler(headRot), ((Component)GorillaTagger.Instance.headCollider).transform.rotation) > num4)
		{
			val = Quaternion.RotateTowards(Quaternion.Euler(headRot), ((Component)GorillaTagger.Instance.headCollider).transform.rotation, num2);
			headRot = ((Quaternion)(ref val)).eulerAngles;
		}
		if (Vector3.Distance(handPos_L, ((Component)GorillaTagger.Instance.leftHandTransform).transform.position) > num3)
		{
			handPos_L += Vector3.Normalize(((Component)GorillaTagger.Instance.leftHandTransform).transform.position - handPos_L) * num;
		}
		if (Quaternion.Angle(Quaternion.Euler(handRot_L), ((Component)GorillaTagger.Instance.leftHandTransform).transform.rotation) > num4)
		{
			val = Quaternion.RotateTowards(Quaternion.Euler(handRot_L), ((Component)GorillaTagger.Instance.leftHandTransform).transform.rotation, num2);
			handRot_L = ((Quaternion)(ref val)).eulerAngles;
		}
		if (Vector3.Distance(handPos_R, ((Component)GorillaTagger.Instance.rightHandTransform).transform.position) > num3)
		{
			handPos_R += Vector3.Normalize(((Component)GorillaTagger.Instance.rightHandTransform).transform.position - handPos_R) * num;
		}
		if (Quaternion.Angle(Quaternion.Euler(handRot_R), ((Component)GorillaTagger.Instance.rightHandTransform).transform.rotation) > num4)
		{
			val = Quaternion.RotateTowards(Quaternion.Euler(handRot_R), ((Component)GorillaTagger.Instance.rightHandTransform).transform.rotation, num2);
			handRot_R = ((Quaternion)(ref val)).eulerAngles;
		}
		((Component)VRRig.LocalRig).transform.position = headPos - new Vector3(0f, 0.15f, 0f);
		((Component)VRRig.LocalRig).transform.rotation = Quaternion.Euler(new Vector3(0f, headRot.y, 0f));
		((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = Quaternion.Euler(headRot);
		((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = handPos_L;
		((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = handPos_R;
		((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = Quaternion.Euler(handRot_L);
		((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = Quaternion.Euler(handRot_R);
		((VRMap)VRRig.LocalRig.leftIndex).calcT = Main.leftTrigger;
		((VRMap)VRRig.LocalRig.leftMiddle).calcT = (Main.leftGrab ? 1 : 0);
		((VRMap)VRRig.LocalRig.leftThumb).calcT = ((Main.leftPrimary || Main.leftSecondary) ? 1 : 0);
		((VRMap)VRRig.LocalRig.leftIndex).LerpFinger(1f, false);
		((VRMap)VRRig.LocalRig.leftMiddle).LerpFinger(1f, false);
		((VRMap)VRRig.LocalRig.leftThumb).LerpFinger(1f, false);
		((VRMap)VRRig.LocalRig.rightIndex).calcT = Main.rightTrigger;
		((VRMap)VRRig.LocalRig.rightMiddle).calcT = (Main.rightGrab ? 1 : 0);
		((VRMap)VRRig.LocalRig.rightThumb).calcT = ((Main.rightPrimary || Main.rightSecondary) ? 1 : 0);
		((VRMap)VRRig.LocalRig.rightIndex).LerpFinger(1f, false);
		((VRMap)VRRig.LocalRig.rightMiddle).LerpFinger(1f, false);
		((VRMap)VRRig.LocalRig.rightThumb).LerpFinger(1f, false);
		FixRigHandRotation();
	}

	public static void DisableGhostAnimations()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		headPos = Vector3.zero;
		headRot = Vector3.zero;
		handPos_L = Vector3.zero;
		handRot_L = Vector3.zero;
		handPos_R = Vector3.zero;
		handRot_R = Vector3.zero;
		((Behaviour)VRRig.LocalRig).enabled = true;
	}

	public static void MinecraftAnimations()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		((Behaviour)VRRig.LocalRig).enabled = false;
		((Component)VRRig.LocalRig).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + new Vector3(0f, 0.15f, 0f);
		((Component)VRRig.LocalRig).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation;
		((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation;
		((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation;
		((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation;
		if (Main.rightPrimary)
		{
			((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + ((Component)GorillaTagger.Instance.bodyCollider).transform.right * -0.25f + ((Component)GorillaTagger.Instance.bodyCollider).transform.up * -1f + ((Component)GorillaTagger.Instance.bodyCollider).transform.forward * Mathf.Sin((float)Time.frameCount / 10f);
			((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + ((Component)GorillaTagger.Instance.bodyCollider).transform.right * 0.25f + ((Component)GorillaTagger.Instance.bodyCollider).transform.up * -1f + -(((Component)GorillaTagger.Instance.bodyCollider).transform.forward * Mathf.Sin((float)Time.frameCount / 10f));
		}
		else
		{
			((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + ((Component)GorillaTagger.Instance.bodyCollider).transform.right * -0.25f + ((Component)GorillaTagger.Instance.bodyCollider).transform.up * -1f;
			((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + ((Component)GorillaTagger.Instance.bodyCollider).transform.right * 0.25f + ((Component)GorillaTagger.Instance.bodyCollider).transform.up * -1f;
		}
		if (Main.rightSecondary)
		{
			((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + ((Component)GorillaTagger.Instance.bodyCollider).transform.right * 0.25f + Vector3.Lerp(GorillaTagger.Instance.rightHandTransform.forward, -GorillaTagger.Instance.rightHandTransform.up, 0.5f) * 2f;
			((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
		}
		FixRigHandRotation();
	}

	public static void StareAtNearby()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		VRRig.LocalRig.head.rigTarget.LookAt(RigUtilities.GetClosestVRRig().headMesh.transform.position);
	}

	public static void StareAtTarget()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		VRRig.LocalRig.head.rigTarget.LookAt(Main.lockTarget.headMesh.transform.position);
	}

	public static void StareAtGun()
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
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				if (!hasAdded)
				{
					hasAdded = true;
					TorsoPatch.VRRigLateUpdate += StareAtTarget;
				}
				Main.gunLocked = true;
				Main.lockTarget = componentInParent;
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
			if (hasAdded)
			{
				hasAdded = false;
				TorsoPatch.VRRigLateUpdate -= StareAtTarget;
			}
		}
	}

	public static void StareAtAll()
	{
		SerializePatch.OverrideSerialization = delegate
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Expected O, but got Unknown
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
			Quaternion rotation = ((Component)VRRig.LocalRig.head.rigTarget).transform.rotation;
			NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
			foreach (NetPlayer val in playerListOthers)
			{
				((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = Quaternion.LookRotation(Vector3.Normalize(RigUtilities.GetVRRigFromPlayer(val).headMesh.transform.position));
				PhotonView photonView = VRRig.LocalRig.GetPhotonView();
				RaiseEventOptions val2 = new RaiseEventOptions();
				val2.TargetActors = new int[1] { val.ActorNumber };
				Main.SendSerialize(photonView, val2);
			}
			Main.RPCProtection();
			((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = rotation;
			return false;
		};
	}

	public static void EyeContact()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		RaycastHit val = default(RaycastHit);
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig rig) => !rig.IsLocal()))
		{
			if (Physics.SphereCast(item.headMesh.transform.position + item.headMesh.transform.forward * 0.25f, 0.25f, item.headMesh.transform.forward, ref val, 512f, Main.NoInvisLayerMask()))
			{
				VRRig.LocalRig.head.rigTarget.LookAt(item.headMesh.transform.position);
				break;
			}
		}
	}

	public static void EnableFloatingRig()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		offsetH = VRRig.LocalRig.head.trackingPositionOffset;
	}

	public static void FloatingRig()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		VRRig.LocalRig.head.trackingPositionOffset = offsetH + new Vector3(0f, 0.65f + Mathf.Sin((float)Time.frameCount / 40f) * 0.2f, 0f);
	}

	public static void DisableFloatingRig()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		VRRig.LocalRig.head.trackingPositionOffset = offsetH;
	}

	public static void Bees()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		((Behaviour)VRRig.LocalRig).enabled = false;
		if (Time.time > beesDelay)
		{
			VRRig randomVRRig = RigUtilities.GetRandomVRRig(includeSelf: false);
			((Component)VRRig.LocalRig).transform.position = ((Component)randomVRRig).transform.position + Vector3.up;
			((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)randomVRRig).transform.position;
			((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)randomVRRig).transform.position;
			beesDelay = Time.time + 0.777f;
		}
	}

	public static void SizeChanger()
	{
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.05f;
		if (!Buttons.GetIndex("Disable Size Changer Buttons").enabled)
		{
			if (Main.leftTrigger > 0.5f)
			{
				num = 0.2f;
			}
			if (Main.leftGrab)
			{
				num = 0.01f;
			}
			if (Main.rightTrigger > 0.5f)
			{
				sizeScale += num;
			}
			if (Main.rightGrab)
			{
				sizeScale -= num;
			}
			if (Main.rightPrimary)
			{
				sizeScale = 1f;
			}
		}
		if (sizeScale < 0.05f)
		{
			sizeScale = 0.05f;
		}
		((Component)VRRig.LocalRig).transform.localScale = Vector3.one * sizeScale;
		VRRig.LocalRig.NativeScale = sizeScale;
		GTPlayer.Instance.nativeScale = sizeScale;
	}

	public static void SilentRotate(bool enable, Quaternion rot)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (enable)
		{
			if (SerializePatch.OverrideSerialization != null)
			{
				return;
			}
			SerializePatch.OverrideSerialization = delegate
			{
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				if (!PhotonNetwork.InRoom)
				{
					return true;
				}
				Quaternion rotation = ((Component)VRRig.LocalRig).transform.rotation;
				((Component)VRRig.LocalRig).transform.rotation = rot;
				Main.SendSerialize(VRRig.LocalRig.GetPhotonView());
				((Component)VRRig.LocalRig).transform.rotation = rotation;
				return false;
			};
		}
		else if (!enable)
		{
			SerializePatch.OverrideSerialization = null;
		}
	}

	public static void Rotate(Quaternion rot)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		((Component)VRRig.LocalRig).transform.rotation = rot;
	}

	public static void VRRigLateUpdate_Dinnerbone()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		((Component)VRRig.LocalRig).transform.RotateAround(VRRig.LocalRig.bodyTransform.position, ((Component)Camera.main).transform.forward, 180f);
	}

	public static void VRRigLateUpdate_SpazBody()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Rotate(Random.rotationUniform);
	}

	public static void VRRigLateUpdate_FakeFBT()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Rotate(((Component)Camera.main).transform.rotation);
		VRRig.LocalRig.head.MapMine(VRRig.LocalRig.lastScaleFactor, VRRig.LocalRig.playerOffsetTransform);
		VRRig.LocalRig.leftHand.MapMine(VRRig.LocalRig.lastScaleFactor, VRRig.LocalRig.playerOffsetTransform);
		VRRig.LocalRig.rightHand.MapMine(VRRig.LocalRig.lastScaleFactor, VRRig.LocalRig.playerOffsetTransform);
	}

	public static void VRRigLateUpdate_Joystick()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightJoystickClick || Main.leftJoystickClick)
		{
			vrrigJoystickRot = null;
		}
		Vector2 rightJoystick = Main.rightJoystick;
		Vector2 leftJoystick = Main.leftJoystick;
		Vector2 val = ((((Vector2)(ref leftJoystick)).sqrMagnitude > ((Vector2)(ref rightJoystick)).sqrMagnitude) ? leftJoystick : rightJoystick);
		if ((double)((Vector2)(ref val)).sqrMagnitude > 0.0025)
		{
			if (!vrrigJoystickRot.HasValue)
			{
				vrrigJoystickRot = ((Component)VRRig.LocalRig).transform.rotation;
			}
			float num = val.x * 200f * Time.deltaTime;
			float num2 = (0f - val.y) * 200f * Time.deltaTime;
			Quaternion val2 = vrrigJoystickRot.Value;
			val2 = Quaternion.AngleAxis(num, Vector3.up) * val2;
			Vector3 val3 = val2 * Vector3.right;
			val2 = Quaternion.AngleAxis(num2, val3) * val2;
			((Quaternion)(ref val2)).Normalize();
			vrrigJoystickRot = val2;
		}
		if (vrrigJoystickRot.HasValue)
		{
			((Component)VRRig.LocalRig).transform.rotation = vrrigJoystickRot.Value;
		}
	}

	public static void DisableSizeChanger()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		sizeScale = 1f;
		((Component)VRRig.LocalRig).transform.localScale = Vector3.one * sizeScale;
		VRRig.LocalRig.NativeScale = sizeScale;
		GTPlayer.Instance.nativeScale = sizeScale;
	}

	public static void SlipSlap()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		GorillaSurfaceOverride[] allType = Main.GetAllType<GorillaSurfaceOverride>(5f);
		foreach (GorillaSurfaceOverride val in allType)
		{
			float num = ((val.slidePercentageOverride > 0f) ? val.slidePercentageOverride : GTPlayer.Instance.materialData[val.overrideIndex].slidePercent);
			if (num > 0f)
			{
				velocityArchive[val] = val.extraVelMultiplier;
				val.extraVelMultiplier += num;
			}
		}
	}

	public static void DisableSlipSlap()
	{
		foreach (KeyValuePair<GorillaSurfaceOverride, float> item in velocityArchive)
		{
			item.Key.extraVelMultiplier = item.Value;
		}
		velocityArchive.Clear();
	}

	public static void StickyHands()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)stickpart == (Object)null)
		{
			stickpart = GameObject.CreatePrimitive((PrimitiveType)0);
			Main.FixStickyColliders(stickpart);
			stickpart.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
			stickpart.GetComponent<Renderer>().enabled = false;
		}
		if (GTPlayer.Instance.IsHandTouching(true))
		{
			stickpart.transform.position = ControllerUtilities.GetTrueLeftHand().position;
		}
		if (GTPlayer.Instance.IsHandTouching(false))
		{
			stickpart.transform.position = ControllerUtilities.GetTrueRightHand().position;
		}
		if (GTPlayer.Instance.IsHandTouching(true) && GTPlayer.Instance.IsHandTouching(false))
		{
			stickpart.transform.position = Vector3.zero;
		}
	}

	public static void DisableStickyHands()
	{
		if ((Object)(object)stickpart != (Object)null)
		{
			Object.Destroy((Object)(object)stickpart);
			stickpart = null;
		}
	}

	public static void ClimbyHands()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)climb == (Object)null)
		{
			climb = new GameObject("GR");
			climb.AddComponent<GorillaClimbable>();
		}
		if (Main.leftGrab)
		{
			if (GTPlayer.Instance.IsHandTouching(true) && !leftisclimbing)
			{
				climb.transform.position = GorillaTagger.Instance.leftHandTransform.position;
				leftisclimbing = true;
				GTPlayer.Instance.BeginClimbing(climb.AddComponent<GorillaClimbable>(), Main.GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHand Controller/GorillaHandClimber").GetComponent<GorillaHandClimber>(), (GorillaClimbableRef)null);
			}
		}
		else
		{
			leftisclimbing = false;
		}
		if (Main.rightGrab)
		{
			if (GTPlayer.Instance.IsHandTouching(false) && !rightisclimbing)
			{
				climb.transform.position = GorillaTagger.Instance.rightHandTransform.position;
				rightisclimbing = true;
				GTPlayer.Instance.BeginClimbing(climb.AddComponent<GorillaClimbable>(), Main.GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHand Controller/GorillaHandClimber").GetComponent<GorillaHandClimber>(), (GorillaClimbableRef)null);
			}
		}
		else
		{
			rightisclimbing = false;
		}
	}

	public static void DisableClimbyHands()
	{
		if ((Object)(object)climb != (Object)null)
		{
			Object.Destroy((Object)(object)climb);
			climb = null;
		}
	}

	public static void SetHandEnabled(bool value)
	{
		GTPlayer.Instance.leftHand.isHolding = !value;
		GTPlayer.Instance.rightHand.isHolding = !value;
	}

	public static void EnableSlideControl()
	{
		oldSlide = GTPlayer.Instance.slideControl;
		GTPlayer.Instance.slideControl = 1f;
	}

	public static void EnableWeakSlideControl()
	{
		oldSlide = GTPlayer.Instance.slideControl;
		GTPlayer.Instance.slideControl = oldSlide * 2f;
	}

	public static void DisableSlideControl()
	{
		GTPlayer.Instance.slideControl = oldSlide;
	}

	public static void PunchMod()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		int num = -1;
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal))
		{
			num++;
			Vector3 position = item.rightHandTransform.position;
			Vector3 position2 = VRRig.LocalRig.bodyTransform.position;
			float num2 = Vector3.Distance(position, position2);
			if (num2 < 0.25f)
			{
				Rigidbody rigidbody = GorillaTagger.Instance.rigidbody;
				rigidbody.linearVelocity += Vector3.Normalize(item.rightHandTransform.position - lastRight[num]) * 10f;
			}
			lastRight[num] = item.rightHandTransform.position;
			position = item.leftHandTransform.position;
			num2 = Vector3.Distance(position, position2);
			if (num2 < 0.25f)
			{
				Rigidbody rigidbody2 = GorillaTagger.Instance.rigidbody;
				rigidbody2.linearVelocity += Vector3.Normalize(item.leftHandTransform.position - lastLeft[num]) * 10f;
			}
			lastLeft[num] = item.leftHandTransform.position;
		}
	}

	public static void Telekinesis()
	{
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)sithlord == (Object)null)
		{
			{
				RaycastHit val = default(RaycastHit);
				RaycastHit val2 = default(RaycastHit);
				foreach (VRRig activeRig in VRRigCache.ActiveRigs)
				{
					try
					{
						if (activeRig.isLocal)
						{
							continue;
						}
						if (((VRMap)activeRig.rightIndex).calcT < 0.5f && ((VRMap)activeRig.rightMiddle).calcT > 0.5f)
						{
							Vector3 up = ((Component)activeRig).transform.Find("rig/hand.R").up;
							Physics.SphereCast(activeRig.rightHandTransform.position + up * 0.1f, 0.3f, up, ref val, 512f, Main.NoInvisLayerMask());
							VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
							if (Object.op_Implicit((Object)(object)componentInParent) && componentInParent.isLocal)
							{
								sithlord = activeRig;
								sithright = true;
								sithdist = ((RaycastHit)(ref val)).distance;
							}
						}
						if (((VRMap)activeRig.leftIndex).calcT < 0.5f && ((VRMap)activeRig.leftMiddle).calcT > 0.5f)
						{
							Vector3 up2 = ((Component)activeRig).transform.Find("rig/hand.L").up;
							Physics.SphereCast(activeRig.leftHandTransform.position + up2 * 0.1f, 0.3f, up2, ref val2, 512f, Main.NoInvisLayerMask());
							VRRig componentInParent2 = ((Component)((RaycastHit)(ref val2)).collider).GetComponentInParent<VRRig>();
							if (Object.op_Implicit((Object)(object)componentInParent2) && componentInParent2.isLocal)
							{
								sithlord = activeRig;
								sithright = false;
								sithdist = ((RaycastHit)(ref val2)).distance;
							}
						}
					}
					catch
					{
					}
				}
				return;
			}
		}
		bool num;
		if (!sithright)
		{
			if (((VRMap)sithlord.leftMiddle).calcT < 0.5f)
			{
				num = ((VRMap)sithlord.leftMiddle).calcT > 0.5f;
				goto IL_0251;
			}
		}
		else if (((VRMap)sithlord.rightIndex).calcT < 0.5f)
		{
			num = ((VRMap)sithlord.rightMiddle).calcT > 0.5f;
			goto IL_0251;
		}
		goto IL_0312;
		IL_0312:
		sithlord = null;
		return;
		IL_0251:
		if (num)
		{
			Transform val3 = (sithright ? sithlord.rightHandTransform : sithlord.leftHandTransform);
			Vector3 val4 = (sithright ? ((Component)sithlord).transform.Find("rig/hand.R").up : ((Component)sithlord).transform.Find("rig/hand.L").up);
			Main.TeleportPlayer(Vector3.Lerp(((Component)GorillaTagger.Instance.bodyCollider).transform.position, val3.position + val4 * sithdist, 0.1f));
			GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
			ZeroGravity();
			return;
		}
		goto IL_0312;
	}

	public static void SafetyBubble()
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		foreach (VRRig item in from rig in VRRigCache.ActiveRigs
			where (Object)(object)rig != (Object)null && !rig.isLocal
			orderby Vector3.Distance(((Component)rig).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position)
			select rig)
		{
			if (Vector3.Distance(((Component)item).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) < 2f)
			{
				Vector3 val = ((Component)GorillaTagger.Instance.bodyCollider).transform.position - ((Component)item).transform.position;
				Vector3 val2 = new Vector3(val.x, 0f, val.z);
				val = ((Vector3)(ref val2)).normalized;
				Main.TeleportPlayer(((Component)GorillaTagger.Instance.bodyCollider).transform.position + val * 2f);
			}
		}
	}

	public static void SolidPlayers()
	{
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		List<VRRig> list = new List<VRRig>();
		foreach (VRRig key in RigColliders.Keys)
		{
			if (!VRRigCache.ActiveRigs.Contains(key))
			{
				list.Add(key);
			}
		}
		foreach (GameObject item in list.SelectMany((VRRig removeRig) => RigColliders[removeRig]))
		{
			Object.Destroy((Object)(object)item);
		}
		foreach (VRRig item2 in list)
		{
			RigColliders.Remove(item2);
		}
		list.Clear();
		foreach (VRRig item3 in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal))
		{
			if (!RigColliders.TryGetValue(item3, out var value))
			{
				value = new List<GameObject>();
				GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
				value.Add(val);
				bool flag = (val.GetComponent<Renderer>().enabled = false);
				bool flag3 = flag;
				val.transform.localScale = new Vector3(0.3f, 0.55f, 0.3f);
				for (int num = 0; num < 19; num++)
				{
					GameObject val2 = GameObject.CreatePrimitive((PrimitiveType)3);
					value.Add(val2);
					flag = (val2.GetComponent<Renderer>().enabled = false);
					bool flag5 = flag;
					val2.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
				}
				RigColliders[item3] = value;
			}
			value[0].transform.position = ((Component)item3.head.rigTarget).transform.position + new Vector3(0f, -0.12f, 0f);
			value[0].transform.rotation = ((Component)item3).transform.rotation;
			for (int num2 = 0; num2 < 19; num2++)
			{
				GameObject val3 = value[num2 + 1];
				Vector3 position = item3.mainSkin.bones[Visuals.bones[num2 * 2]].position;
				Vector3 position2 = item3.mainSkin.bones[Visuals.bones[num2 * 2 + 1]].position;
				val3.transform.position = Vector3.Lerp(position, position2, 0.5f);
				val3.transform.LookAt(position2);
				val3.transform.localScale = new Vector3(0.2f, 0.2f, Vector3.Distance(position, position2));
			}
		}
	}

	public static void DisableSolidPlayers()
	{
		foreach (GameObject item in RigColliders.Values.SelectMany((List<GameObject> gameObjects) => gameObjects))
		{
			Object.Destroy((Object)(object)item);
		}
		RigColliders.Clear();
	}

	public static void ChangePullModPower(bool positive = true)
	{
		float[] array = new float[4] { 0.05f, 0.1f, 0.2f, 0.4f };
		string[] array2 = new string[4] { "Normal", "Medium", "Strong", "Powerful" };
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				pullPowerInt++;
			}
			else
			{
				pullPowerInt--;
			}
		}
		pullPowerInt %= array2.Length;
		if (pullPowerInt < 0)
		{
			pullPowerInt = array2.Length - 1;
		}
		pullPower = array[pullPowerInt];
		Buttons.GetIndex("Change Pull Mod Power").overlapText = "Change Pull Mod Power <color=grey>[</color><color=green>" + array2[pullPowerInt] + "</color><color=grey>]</color>";
	}

	public static void ProcessPullHand(bool left)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		if (!(left ? (!Main.leftGrab) : (!Main.rightGrab)))
		{
			bool flag = GTPlayer.Instance.IsHandTouching(left);
			previousTouchingGround.TryGetValue(left, out var value);
			if (!flag && value)
			{
				Vector3 normal = ((RaycastHit)(ref GTPlayer.Instance.lastHitInfoHand)).normal;
				Vector3 val = GTVector3Extensions.X_Z(GorillaTagger.Instance.rigidbody.linearVelocity);
				Transform transform = ((Component)GTPlayer.Instance).transform;
				Vector3 position = transform.position;
				Vector3 val2 = val - normal * Vector3.Dot(val, normal);
				transform.position = position + ((Vector3)(ref val2)).normalized * (((Vector3)(ref val)).magnitude / GTPlayer.Instance.maxJumpSpeed * (pullPower * 5f)) * (Main.scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
			}
			previousTouchingGround[left] = flag;
		}
	}

	public static void PullMod()
	{
		ProcessPullHand(left: false);
		ProcessPullHand(left: true);
	}

	public static void ThrowControllers()
	{
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		if (Main.leftPrimary)
		{
			if ((Object)(object)leftThrow != (Object)null)
			{
				GTPlayer.Instance.GetControllerTransform(true).position = leftThrow.transform.position;
				GTPlayer.Instance.GetControllerTransform(true).rotation = leftThrow.transform.rotation;
			}
			else
			{
				leftThrow = GameObject.CreatePrimitive((PrimitiveType)3);
				leftThrow.GetComponent<Renderer>().enabled = false;
				Object.Destroy((Object)(object)leftThrow.GetComponent<BoxCollider>());
				leftThrow.transform.position = GTPlayer.Instance.GetControllerTransform(true).position;
				leftThrow.transform.rotation = GTPlayer.Instance.GetControllerTransform(true).rotation;
				Component obj = leftThrow.AddComponent(typeof(Rigidbody));
				Rigidbody val = (Rigidbody)(object)((obj is Rigidbody) ? obj : null);
				val.linearVelocity = GTPlayer.Instance.LeftHand.velocityTracker.GetAverageVelocity(true, 0f, false);
				try
				{
					if ((Object)(object)Main.GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHand Controller").GetComponent<GorillaVelocityEstimator>() == (Object)null)
					{
						Main.GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHand Controller").AddComponent<GorillaVelocityEstimator>();
					}
					val.angularVelocity = Main.GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/LeftHand Controller").GetComponent<GorillaVelocityEstimator>().angularVelocity;
				}
				catch
				{
				}
			}
		}
		else if ((Object)(object)leftThrow != (Object)null)
		{
			Object.Destroy((Object)(object)leftThrow);
			leftThrow = null;
		}
		if (Main.rightPrimary)
		{
			if (!((Object)(object)rightThrow != (Object)null))
			{
				rightThrow = GameObject.CreatePrimitive((PrimitiveType)3);
				rightThrow.GetComponent<Renderer>().enabled = false;
				Object.Destroy((Object)(object)rightThrow.GetComponent<BoxCollider>());
				rightThrow.transform.position = GTPlayer.Instance.GetControllerTransform(false).position;
				rightThrow.transform.rotation = GTPlayer.Instance.GetControllerTransform(false).rotation;
				Component obj3 = rightThrow.AddComponent(typeof(Rigidbody));
				Rigidbody val2 = (Rigidbody)(object)((obj3 is Rigidbody) ? obj3 : null);
				val2.linearVelocity = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0f, false);
				try
				{
					if ((Object)(object)Main.GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHand Controller").GetComponent<GorillaVelocityEstimator>() == (Object)null)
					{
						Main.GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHand Controller").AddComponent<GorillaVelocityEstimator>();
					}
					val2.angularVelocity = Main.GetObject("Player Objects/Player VR Controller/GorillaPlayer/TurnParent/RightHand Controller").GetComponent<GorillaVelocityEstimator>().angularVelocity;
					return;
				}
				catch
				{
					return;
				}
			}
			GTPlayer.Instance.GetControllerTransform(false).position = rightThrow.transform.position;
			GTPlayer.Instance.GetControllerTransform(false).rotation = rightThrow.transform.rotation;
		}
		else if ((Object)(object)rightThrow != (Object)null)
		{
			Object.Destroy((Object)(object)rightThrow);
			rightThrow = null;
		}
	}

	public static void EnableControllerFlick()
	{
		flickLeft = GameObject.CreatePrimitive((PrimitiveType)3);
		Object.Destroy((Object)(object)flickLeft.GetComponent<BoxCollider>());
		flickLeft.GetComponent<Renderer>().enabled = false;
		flickLeft.AddComponent<GorillaVelocityTracker>();
		Rigidbody val = flickLeft.AddComponent<Rigidbody>();
		val.isKinematic = true;
		val.useGravity = false;
		flickRight = GameObject.CreatePrimitive((PrimitiveType)3);
		Object.Destroy((Object)(object)flickRight.GetComponent<BoxCollider>());
		flickRight.GetComponent<Renderer>().enabled = false;
		flickRight.AddComponent<GorillaVelocityTracker>();
		Rigidbody val2 = flickRight.AddComponent<Rigidbody>();
		val2.isKinematic = true;
		val2.useGravity = false;
	}

	public static void DisableControllerFlick()
	{
		Object.Destroy((Object)(object)flickLeft);
		Object.Destroy((Object)(object)flickRight);
	}

	public static void ControllerFlick()
	{
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		if (Main.leftPrimary)
		{
			if (initialRotationLeft == Quaternion.identity)
			{
				initialRotationLeft = GorillaTagger.Instance.leftHandTransform.rotation;
			}
			Rigidbody component = flickLeft.GetComponent<Rigidbody>();
			if (component.isKinematic)
			{
				component.isKinematic = false;
				component.linearVelocity = ((Component)component).GetComponent<GorillaVelocityTracker>().GetAverageVelocity(true, 0f, false);
			}
			GorillaTagger.Instance.leftHandTransform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position - flickLeft.transform.position;
			GorillaTagger.Instance.leftHandTransform.rotation = initialRotationLeft;
		}
		else
		{
			Rigidbody component2 = flickLeft.GetComponent<Rigidbody>();
			component2.isKinematic = true;
			flickLeft.transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position - GorillaTagger.Instance.leftHandTransform.position;
			initialRotationLeft = Quaternion.identity;
		}
		if (Main.rightPrimary)
		{
			if (initialRotationRight == Quaternion.identity)
			{
				initialRotationRight = GorillaTagger.Instance.rightHandTransform.rotation;
			}
			Rigidbody component3 = flickRight.GetComponent<Rigidbody>();
			if (component3.isKinematic)
			{
				component3.isKinematic = false;
				component3.linearVelocity = ((Component)component3).GetComponent<GorillaVelocityTracker>().GetAverageVelocity(true, 0f, false);
			}
			GorillaTagger.Instance.rightHandTransform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position - flickRight.transform.position;
			GorillaTagger.Instance.rightHandTransform.rotation = initialRotationRight;
		}
		else
		{
			Rigidbody component4 = flickRight.GetComponent<Rigidbody>();
			component4.isKinematic = true;
			flickRight.transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position - GorillaTagger.Instance.rightHandTransform.position;
			initialRotationRight = Quaternion.identity;
		}
	}

	public static void StickLongArms()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		((Component)GTPlayer.Instance.GetControllerTransform(true)).transform.position = GorillaTagger.Instance.leftHandTransform.position + GorillaTagger.Instance.leftHandTransform.forward * ((armlength - 0.917f) * GTPlayer.Instance.scale);
		((Component)GTPlayer.Instance.GetControllerTransform(false)).transform.position = GorillaTagger.Instance.rightHandTransform.position + GorillaTagger.Instance.rightHandTransform.forward * ((armlength - 0.917f) * GTPlayer.Instance.scale);
	}

	public static void EnableSteamLongArms()
	{
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		if (passWorldScaleCheck)
		{
			Vector3 point = ControllerInputPoller.DevicePosition((XRNode)3);
			Vector3 to = ControllerInputPoller.DevicePosition((XRNode)4);
			Vector3 to2 = ControllerInputPoller.DevicePosition((XRNode)5);
			Vector3 averageVelocity = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0f, false);
			int num;
			if (((Vector3)(ref averageVelocity)).magnitude < 2f)
			{
				averageVelocity = GTPlayer.Instance.LeftHand.velocityTracker.GetAverageVelocity(true, 0f, false);
				num = ((((Vector3)(ref averageVelocity)).magnitude < 2f) ? 1 : 0);
			}
			else
			{
				num = 0;
			}
			bool flag = (byte)num != 0;
			if (point.Distance(to) < 0.2f && point.Distance(to2) < 0.2f && flag)
			{
				Vector3 position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
				DisableSteamLongArms();
				if (!lastFramePosition.HasValue)
				{
					Main.TeleportPlayer(position);
				}
				lastFramePosition = position;
				return;
			}
			if (lastFramePosition.HasValue)
			{
				Main.TeleportPlayer(lastFramePosition.Value);
				lastFramePosition = null;
			}
		}
		((Component)GTPlayer.Instance).transform.localScale = Vector3.one * (VRRig.LocalRig.NativeScale * armlength);
	}

	public static void DisableSteamLongArms()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((Component)GTPlayer.Instance).transform.localScale = Vector3.one * VRRig.LocalRig.NativeScale;
	}

	public static void Extenders()
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		extendingTime += (Main.rightJoystickClick ? (0f - Time.unscaledDeltaTime) : Time.unscaledDeltaTime);
		if (extendingTime > 1f)
		{
			extendingTime = 1f;
		}
		if (extendingTime < 0f)
		{
			extendingTime = 0f;
		}
		float num = (armlength - 1f) * extendingTime + 1f;
		((Component)GTPlayer.Instance).transform.localScale = new Vector3(num, num, num);
	}

	public static void MultipliedLongArms()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		((Component)GTPlayer.Instance.GetControllerTransform(true)).transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position - (((Component)GorillaTagger.Instance.headCollider).transform.position - GorillaTagger.Instance.leftHandTransform.position) * armlength;
		((Component)GTPlayer.Instance.GetControllerTransform(false)).transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position - (((Component)GorillaTagger.Instance.headCollider).transform.position - GorillaTagger.Instance.rightHandTransform.position) * armlength;
	}

	public static void VerticalLongArms()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)GorillaTagger.Instance.headCollider).transform.position - GorillaTagger.Instance.leftHandTransform.position;
		val.y *= armlength;
		Vector3 val2 = ((Component)GorillaTagger.Instance.headCollider).transform.position - GorillaTagger.Instance.rightHandTransform.position;
		val2.y *= armlength;
		((Component)GTPlayer.Instance.GetControllerTransform(true)).transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position - val;
		((Component)GTPlayer.Instance.GetControllerTransform(false)).transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position - val2;
	}

	public static void HorizontalLongArms()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)GorillaTagger.Instance.headCollider).transform.position - GorillaTagger.Instance.leftHandTransform.position;
		val.x *= armlength;
		val.z *= armlength;
		Vector3 val2 = ((Component)GorillaTagger.Instance.headCollider).transform.position - GorillaTagger.Instance.rightHandTransform.position;
		val2.x *= armlength;
		val2.z *= armlength;
		GTPlayer.Instance.GetControllerTransform(true).position = ((Component)GorillaTagger.Instance.headCollider).transform.position - val;
		GTPlayer.Instance.GetControllerTransform(false).position = ((Component)GorillaTagger.Instance.headCollider).transform.position - val2;
	}

	public static void CreateVelocityTrackers()
	{
		lvT = GameObject.CreatePrimitive((PrimitiveType)3);
		Object.Destroy((Object)(object)lvT.GetComponent<BoxCollider>());
		lvT.GetComponent<Renderer>().enabled = false;
		lvT.AddComponent<GorillaVelocityTracker>();
		rvT = GameObject.CreatePrimitive((PrimitiveType)3);
		Object.Destroy((Object)(object)rvT.GetComponent<BoxCollider>());
		rvT.GetComponent<Renderer>().enabled = false;
		rvT.AddComponent<GorillaVelocityTracker>();
	}

	public static void DestroyVelocityTrackers()
	{
		Object.Destroy((Object)(object)lvT);
		Object.Destroy((Object)(object)rvT);
	}

	public static void ChangePredictionAmount(bool positive = true)
	{
		float[] array = new float[4]
		{
			1f / 160f,
			0.0125f,
			0.025f,
			0.05f
		};
		string[] array2 = new string[4] { "Low", "Normal", "High", "Extreme" };
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				predInt++;
			}
			else
			{
				predInt--;
			}
		}
		predInt %= array.Length;
		if (predInt < 0)
		{
			predInt = array.Length - 1;
		}
		predCount = array[predInt];
		Buttons.GetIndex("Change Prediction Amount").overlapText = "Change Prediction Amount <color=grey>[</color><color=green>" + array2[predInt] + "</color><color=grey>]</color>";
	}

	public static void VelocityLongArms()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		lvT.transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position - GorillaTagger.Instance.leftHandTransform.position;
		rvT.transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position - GorillaTagger.Instance.rightHandTransform.position;
		Transform transform = ((Component)GTPlayer.Instance.GetControllerTransform(true)).transform;
		transform.position -= lvT.GetComponent<GorillaVelocityTracker>().GetAverageVelocity(true, 0f, false) * predCount;
		Transform transform2 = ((Component)GTPlayer.Instance.GetControllerTransform(false)).transform;
		transform2.position -= rvT.GetComponent<GorillaVelocityTracker>().GetAverageVelocity(true, 0f, false) * predCount;
	}

	public static void ChangeFakeLagStrength(bool positive = true)
	{
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				fakeLagDelayIndex++;
			}
			else
			{
				fakeLagDelayIndex--;
			}
		}
		fakeLagDelayIndex %= 21;
		if (fakeLagDelayIndex < 0)
		{
			fakeLagDelayIndex = 20;
		}
		fakeLagDelay = (float)fakeLagDelayIndex / 10f;
		Buttons.GetIndex("Change Fake Lag Strength").overlapText = "Change Fake Lag Strength <color=grey>[</color><color=green>" + fakeLagDelayIndex + "</color><color=grey>]</color>";
	}

	public static void FakeLag()
	{
		PlayerSerializePatch.delay = (Buttons.GetIndex("Fake Lag Others").enabled ? new float?(fakeLagDelay) : ((float?)null));
		SerializePatch.OverrideSerialization = ((!Buttons.GetIndex("Disable Fake Lag Self").enabled) ? ((Func<bool>)delegate
		{
			Main.MassSerialize(exclude: true, null, 0, fakeLagDelay);
			return false;
		}) : null);
	}

	public static void LagRange()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		bool isTagged = VRRig.LocalRig.IsTagged();
		VRRig val = (from rig in VRRigCache.ActiveRigs
			where (Object)(object)rig != (Object)null && !rig.isLocal && (isTagged ? (!rig.IsTagged()) : rig.IsTagged())
			orderby Vector3.Distance(((Component)rig).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position)
			select rig).FirstOrDefault();
		float num = (((Object)(object)val == (Object)null) ? float.MaxValue : Vector3.Distance(((Component)GorillaTagger.Instance.bodyCollider).transform.position, ((Component)val).transform.position));
		if (num < 15f)
		{
			float num2 = Mathf.Clamp(num, 1f, 15f) / 15f;
			PhotonNetwork.SerializationRate = 4 + (int)Math.Ceiling(num2 * 6f);
		}
	}

	public static void Blink()
	{
		SerializePatch.OverrideSerialization = () => false;
		PlayerSerializePatch.stopSerialization = true;
		isBlinking = true;
	}

	public static void DisableBlink()
	{
		SerializePatch.OverrideSerialization = null;
		PlayerSerializePatch.stopSerialization = false;
		isBlinking = false;
	}

	public static void ChangeTimerSpeed(bool positive = true)
	{
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				timerPowerIndex++;
			}
			else
			{
				timerPowerIndex--;
			}
		}
		timerPowerIndex %= 51;
		if (timerPowerIndex < 1)
		{
			timerPowerIndex = 50;
		}
		timerPower = (float)timerPowerIndex / 10f;
		Buttons.GetIndex("Change Timer Speed").overlapText = "Change Timer Speed <color=grey>[</color><color=green>" + (float)timerPowerIndex / 10f + "</color><color=grey>]</color>";
	}

	public static void Timer()
	{
		GTPlayer.Instance.debugMovement = true;
		Time.timeScale = timerPower;
	}

	public static void FlickJump()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightPrimary)
		{
			((Component)GTPlayer.Instance.GetControllerTransform(false)).transform.position = GorillaTagger.Instance.rightHandTransform.position + new Vector3(0f, -1.5f, 0f);
		}
	}

	public static void PlayspaceAbuse()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightPrimary)
		{
			float valueOrDefault = keepVelocityUntil.GetValueOrDefault();
			if (!keepVelocityUntil.HasValue)
			{
				valueOrDefault = Time.time + 0.5f;
				keepVelocityUntil = valueOrDefault;
			}
			Vector3 valueOrDefault2 = velocity.GetValueOrDefault();
			if (!velocity.HasValue)
			{
				valueOrDefault2 = GorillaTagger.Instance.rigidbody.linearVelocity;
				velocity = valueOrDefault2;
			}
			if (Time.time < keepVelocityUntil)
			{
				GorillaTagger.Instance.rigidbody.linearVelocity = velocity.Value;
			}
			valueOrDefault2 = longJumpPower.GetValueOrDefault();
			if (!longJumpPower.HasValue)
			{
				valueOrDefault2 = GTVector3Extensions.X_Z(GorillaTagger.Instance.rigidbody.linearVelocity * playspaceAbusePower);
				longJumpPower = valueOrDefault2;
			}
			Transform transform = ((Component)GTPlayer.Instance).transform;
			transform.position += longJumpPower.Value;
		}
		else
		{
			longJumpPower = null;
			velocity = null;
		}
	}

	public static void BunnyHop()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		RaycastHit val = default(RaycastHit);
		Physics.Raycast(((Component)GorillaTagger.Instance.bodyCollider).transform.position - new Vector3(0f, 0.2f, 0f), Vector3.down, ref val, 512f, LayerMask.op_Implicit(GTPlayer.Instance.locomotionEnabledLayers));
		if (((RaycastHit)(ref val)).distance < 0.15f)
		{
			GorillaTagger.Instance.rigidbody.linearVelocity = new Vector3(GorillaTagger.Instance.rigidbody.linearVelocity.x, GTPlayer.Instance.jumpMultiplier * 2.7272727f, GorillaTagger.Instance.rigidbody.linearVelocity.z);
		}
	}

	public static void Strafe()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val;
		if (!Buttons.GetIndex("Hand Oriented Strafe").enabled)
		{
			val = ((Component)GorillaTagger.Instance.bodyCollider).transform.forward;
		}
		else
		{
			Vector3 val2 = new Vector3(0f - GorillaTagger.Instance.rightHandTransform.up.x, 0f, 0f - GorillaTagger.Instance.rightHandTransform.up.z);
			val = ((Vector3)(ref val2)).normalized;
		}
		Vector3 val3 = val;
		float maxJumpSpeed = GTPlayer.Instance.maxJumpSpeed;
		Vector3 val4 = val3 * maxJumpSpeed;
		GorillaTagger.Instance.rigidbody.linearVelocity = new Vector3(val4.x, GorillaTagger.Instance.rigidbody.linearVelocity.y, val4.z);
	}

	public static void DynamicStrafe()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val;
		Vector3 val2;
		if (!Buttons.GetIndex("Hand Oriented Strafe").enabled)
		{
			val = ((Component)GorillaTagger.Instance.bodyCollider).transform.forward;
		}
		else
		{
			val2 = new Vector3(0f - GorillaTagger.Instance.rightHandTransform.up.x, 0f, 0f - GorillaTagger.Instance.rightHandTransform.up.z);
			val = ((Vector3)(ref val2)).normalized;
		}
		Vector3 val3 = val;
		val2 = new Vector3(GorillaTagger.Instance.rigidbody.linearVelocity.x, 0f, GorillaTagger.Instance.rigidbody.linearVelocity.z);
		float magnitude = ((Vector3)(ref val2)).magnitude;
		Vector3 val4 = val3 * magnitude;
		GorillaTagger.Instance.rigidbody.linearVelocity = new Vector3(val4.x, GorillaTagger.Instance.rigidbody.linearVelocity.y, val4.z);
	}

	public static void GroundHelper()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab)
		{
			Vector3 linearVelocity = GorillaTagger.Instance.rigidbody.linearVelocity;
			if (linearVelocity.y > 0f)
			{
				GorillaTagger.Instance.rigidbody.linearVelocity = new Vector3(linearVelocity.x, 0f, linearVelocity.z);
			}
		}
	}

	public static void LowFPSMovement()
	{
		GTPlayer.Instance.velocityHistorySize = 12;
		GTPlayer.Instance.InitializeValues();
	}

	public static void PreBouncy()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		preBounciness = ((Collider)GorillaTagger.Instance.bodyCollider).material.bounciness;
		whateverthisis = ((Collider)GorillaTagger.Instance.bodyCollider).material.bounceCombine;
		preFrictiness = ((Collider)GorillaTagger.Instance.bodyCollider).material.dynamicFriction;
	}

	public static void Bouncy()
	{
		((Collider)GorillaTagger.Instance.bodyCollider).material.bounciness = 1f;
		((Collider)GorillaTagger.Instance.bodyCollider).material.bounceCombine = (PhysicsMaterialCombine)3;
		((Collider)GorillaTagger.Instance.bodyCollider).material.dynamicFriction = 0f;
	}

	public static void PostBouncy()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		((Collider)GorillaTagger.Instance.bodyCollider).material.bounciness = preBounciness;
		((Collider)GorillaTagger.Instance.bodyCollider).material.bounceCombine = whateverthisis;
		((Collider)GorillaTagger.Instance.bodyCollider).material.dynamicFriction = preFrictiness;
	}

	public static void DisableWater()
	{
		WaterVolume[] allType = Main.GetAllType<WaterVolume>(5f);
		foreach (WaterVolume val in allType)
		{
			GameObject gameObject = ((Component)val).gameObject;
			gameObject.layer = LayerMask.NameToLayer("TransparentFX");
		}
	}

	public static void SolidWater()
	{
		WaterVolume[] allType = Main.GetAllType<WaterVolume>(5f);
		foreach (WaterVolume val in allType)
		{
			GameObject gameObject = ((Component)val).gameObject;
			gameObject.layer = LayerMask.NameToLayer("Default");
		}
	}

	public static void FixWater()
	{
		WaterVolume[] allType = Main.GetAllType<WaterVolume>(5f);
		foreach (WaterVolume val in allType)
		{
			GameObject gameObject = ((Component)val).gameObject;
			gameObject.layer = LayerMask.NameToLayer("Water");
		}
	}

	public static void AirSwim()
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)airSwimPart == (Object)null)
		{
			airSwimPart = Object.Instantiate<GameObject>(Main.GetObject("Environment Objects/LocalObjects_Prefab/ForestToBeach/ForestToBeach_Prefab_V4/ForestToBeach_Geo/CaveWaterVolume"));
			airSwimPart.transform.localScale = new Vector3(5f, 5f, 5f);
			airSwimPart.GetComponent<Renderer>().enabled = false;
		}
		else
		{
			GTPlayer.Instance.audioManager.UnsetMixerSnapshot(0.1f);
			airSwimPart.transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(0f, 2.5f, 0f);
		}
	}

	public static void DisableAirSwim()
	{
		if ((Object)(object)airSwimPart != (Object)null)
		{
			Object.Destroy((Object)(object)airSwimPart);
		}
	}

	public static void GiveSwimGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > giveSwimDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.isOfflineVRRig)
			{
				giveSwimDelay = Time.time + 0.5f;
				Main.gunLocked = !Main.gunLocked;
				Main.lockTarget = (Main.gunLocked ? componentInParent : null);
				if (Main.gunLocked && (Object)(object)giveSwimWater == (Object)null)
				{
					giveSwimWater = Object.Instantiate<GameObject>(Main.GetObject("Environment Objects/LocalObjects_Prefab/ForestToBeach/ForestToBeach_Prefab_V4/ForestToBeach_Geo/CaveWaterVolume"));
					giveSwimWater.transform.localScale = new Vector3(5f, 5f, 5f);
					giveSwimWater.GetComponent<Renderer>().enabled = false;
				}
				else if (!Main.gunLocked && (Object)(object)giveSwimWater != (Object)null)
				{
					Object.Destroy((Object)(object)giveSwimWater);
					giveSwimWater = null;
				}
			}
		}
		if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && (Object)(object)giveSwimWater != (Object)null)
		{
			GTPlayer.Instance.audioManager.UnsetMixerSnapshot(0.1f);
			giveSwimWater.transform.position = ((Component)Main.lockTarget).transform.position + new Vector3(0f, 2.5f, 0f);
		}
	}

	public static void DisableGiveSwimGun()
	{
		Main.gunLocked = false;
		Main.lockTarget = null;
		if ((Object)(object)giveSwimWater != (Object)null)
		{
			Object.Destroy((Object)(object)giveSwimWater);
			giveSwimWater = null;
		}
	}

	public static void SetSwimSpeed(float speed = 3f)
	{
		object swimmingParams = GetSwimmingParams();
		if (swimmingParams != null)
		{
			Traverse.Create(swimmingParams).Field("swimmingVelocityOutOfWaterDrainRate").SetValue((object)speed);
		}
	}

	private static object GetSwimmingParams()
	{
		return Traverse.Create((object)GTPlayer.Instance).Field("swimmingParams").GetValue();
	}

	public static void WaterRunHelper(bool enable)
	{
		object swimmingParams = GetSwimmingParams();
		if (swimmingParams != null)
		{
			Traverse val = Traverse.Create(swimmingParams);
			if (enable)
			{
				waterSurfaceJumpAmount = val.Field("waterSurfaceJumpAmount").GetValue<float>();
				waterSurfaceJumpMaxSpeed = val.Field("waterSurfaceJumpMaxSpeed").GetValue<float>();
				val.Field("waterSurfaceJumpAmount").SetValue((object)1.25f);
				val.Field("waterSurfaceJumpMaxSpeed").SetValue((object)4.333f);
			}
			else
			{
				val.Field("waterSurfaceJumpAmount").SetValue((object)(waterSurfaceJumpAmount ?? 0.6f));
				val.Field("waterSurfaceJumpMaxSpeed").SetValue((object)(waterSurfaceJumpMaxSpeed ?? 1f));
			}
		}
	}

	public static void PiggybackGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Main.TeleportPlayer(((Component)Main.lockTarget).transform.position + new Vector3(0f, 0.5f, 0f));
				GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
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

	public static void PiggybackAll()
	{
		SerializePatch.OverrideSerialization = delegate
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Expected O, but got Unknown
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
			Vector3 position = ((Component)VRRig.LocalRig).transform.position;
			NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
			foreach (NetPlayer val in playerListOthers)
			{
				((Component)VRRig.LocalRig).transform.position = RigUtilities.GetVRRigFromPlayer(val).headMesh.transform.position;
				PhotonView photonView = VRRig.LocalRig.GetPhotonView();
				RaiseEventOptions val2 = new RaiseEventOptions();
				val2.TargetActors = new int[1] { val.ActorNumber };
				Main.SendSerialize(photonView, val2);
			}
			Main.RPCProtection();
			((Component)VRRig.LocalRig).transform.position = position;
			return false;
		};
	}

	public static void CopyMovementGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				CopyMovementPlayer(RigUtilities.GetPlayerFromVRRig(Main.lockTarget));
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

	public static void CopyMovementPlayer(NetPlayer player, bool fingers = true)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(player);
		((Behaviour)VRRig.LocalRig).enabled = false;
		((Component)VRRig.LocalRig).transform.position = vRRigFromPlayer.syncPos;
		((Component)VRRig.LocalRig).transform.rotation = vRRigFromPlayer.syncRotation;
		((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)vRRigFromPlayer.leftHand.rigTarget).transform.position;
		((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)vRRigFromPlayer.rightHand.rigTarget).transform.position;
		((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = ((Component)vRRigFromPlayer.leftHand.rigTarget).transform.rotation;
		((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = ((Component)vRRigFromPlayer.rightHand.rigTarget).transform.rotation;
		if (fingers)
		{
			((VRMap)VRRig.LocalRig.leftIndex).calcT = ((VRMap)vRRigFromPlayer.leftIndex).calcT;
			((VRMap)VRRig.LocalRig.leftMiddle).calcT = ((VRMap)vRRigFromPlayer.leftMiddle).calcT;
			((VRMap)VRRig.LocalRig.leftThumb).calcT = ((VRMap)vRRigFromPlayer.leftThumb).calcT;
			((VRMap)VRRig.LocalRig.leftIndex).LerpFinger(1f, false);
			((VRMap)VRRig.LocalRig.leftMiddle).LerpFinger(1f, false);
			((VRMap)VRRig.LocalRig.leftThumb).LerpFinger(1f, false);
			((VRMap)VRRig.LocalRig.rightIndex).calcT = ((VRMap)vRRigFromPlayer.rightIndex).calcT;
			((VRMap)VRRig.LocalRig.rightMiddle).calcT = ((VRMap)vRRigFromPlayer.rightMiddle).calcT;
			((VRMap)VRRig.LocalRig.rightThumb).calcT = ((VRMap)vRRigFromPlayer.rightThumb).calcT;
			((VRMap)VRRig.LocalRig.rightIndex).LerpFinger(1f, false);
			((VRMap)VRRig.LocalRig.rightMiddle).LerpFinger(1f, false);
			((VRMap)VRRig.LocalRig.rightThumb).LerpFinger(1f, false);
		}
		((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = ((Component)vRRigFromPlayer.head.rigTarget).transform.rotation;
	}

	public static void CopyMovementAll()
	{
		SerializePatch.OverrideSerialization = delegate
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Expected O, but got Unknown
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
			Vector3 position = ((Component)VRRig.LocalRig).transform.position;
			Quaternion rotation = ((Component)VRRig.LocalRig).transform.rotation;
			Vector3 position2 = VRRig.LocalRig.leftHand.rigTarget.position;
			Quaternion rotation2 = VRRig.LocalRig.leftHand.rigTarget.rotation;
			Vector3 position3 = VRRig.LocalRig.rightHand.rigTarget.position;
			Quaternion rotation3 = VRRig.LocalRig.rightHand.rigTarget.rotation;
			Quaternion rotation4 = ((Component)VRRig.LocalRig.head.rigTarget).transform.rotation;
			NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
			foreach (NetPlayer val in playerListOthers)
			{
				CopyMovementPlayer(val, fingers: false);
				PhotonView photonView = VRRig.LocalRig.GetPhotonView();
				RaiseEventOptions val2 = new RaiseEventOptions();
				val2.TargetActors = new int[1] { val.ActorNumber };
				Main.SendSerialize(photonView, val2);
			}
			Main.RPCProtection();
			((Behaviour)VRRig.LocalRig).enabled = true;
			((Component)VRRig.LocalRig).transform.position = position;
			((Component)VRRig.LocalRig).transform.rotation = rotation;
			VRRig.LocalRig.leftHand.rigTarget.position = position2;
			VRRig.LocalRig.leftHand.rigTarget.rotation = rotation2;
			VRRig.LocalRig.rightHand.rigTarget.position = position3;
			VRRig.LocalRig.rightHand.rigTarget.rotation = rotation3;
			((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = rotation4;
			return false;
		};
	}

	public static void FollowPlayer(NetPlayer player, bool fingers = true)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(player);
		((Behaviour)VRRig.LocalRig).enabled = false;
		Vector3 val = ((Component)vRRigFromPlayer).transform.position - ((Component)VRRig.LocalRig).transform.position;
		((Vector3)(ref val)).Normalize();
		Vector3 position = ((Component)VRRig.LocalRig).transform.position + val * (FlySpeed / 2f * Time.deltaTime);
		((Component)VRRig.LocalRig).transform.position = position;
		((Component)VRRig.LocalRig).transform.LookAt(((Component)vRRigFromPlayer).transform.position);
		((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
		((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + ((Component)VRRig.LocalRig).transform.right * -1f;
		((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + ((Component)VRRig.LocalRig).transform.right * 1f;
		((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
		((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
		FixRigHandRotation();
		if (fingers)
		{
			((VRMap)VRRig.LocalRig.leftIndex).calcT = 0f;
			((VRMap)VRRig.LocalRig.leftMiddle).calcT = 0f;
			((VRMap)VRRig.LocalRig.leftThumb).calcT = 0f;
			((VRMap)VRRig.LocalRig.leftIndex).LerpFinger(1f, false);
			((VRMap)VRRig.LocalRig.leftMiddle).LerpFinger(1f, false);
			((VRMap)VRRig.LocalRig.leftThumb).LerpFinger(1f, false);
			((VRMap)VRRig.LocalRig.rightIndex).calcT = 0f;
			((VRMap)VRRig.LocalRig.rightMiddle).calcT = 0f;
			((VRMap)VRRig.LocalRig.rightThumb).calcT = 0f;
			((VRMap)VRRig.LocalRig.rightIndex).LerpFinger(1f, false);
			((VRMap)VRRig.LocalRig.rightMiddle).LerpFinger(1f, false);
			((VRMap)VRRig.LocalRig.rightThumb).LerpFinger(1f, false);
		}
	}

	public static void FollowPlayerGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				FollowPlayer(RigUtilities.GetPlayerFromVRRig(Main.lockTarget));
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

	public static void FollowAllPlayers()
	{
		SerializePatch.OverrideSerialization = delegate
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Expected O, but got Unknown
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
			Vector3 position = ((Component)VRRig.LocalRig).transform.position;
			Quaternion rotation = ((Component)VRRig.LocalRig).transform.rotation;
			Vector3 position2 = VRRig.LocalRig.leftHand.rigTarget.position;
			Quaternion rotation2 = VRRig.LocalRig.leftHand.rigTarget.rotation;
			Vector3 position3 = VRRig.LocalRig.rightHand.rigTarget.position;
			Quaternion rotation3 = VRRig.LocalRig.rightHand.rigTarget.rotation;
			Quaternion rotation4 = ((Component)VRRig.LocalRig.head.rigTarget).transform.rotation;
			NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
			foreach (NetPlayer val in playerListOthers)
			{
				VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(val);
				Vector3 valueOrDefault = followPositions.GetValueOrDefault(vRRigFromPlayer, position);
				Vector3 val2 = ((Component)vRRigFromPlayer).transform.position - valueOrDefault;
				((Vector3)(ref val2)).Normalize();
				valueOrDefault += val2 * (FlySpeed / 2f * Time.deltaTime);
				followPositions.Remove(vRRigFromPlayer);
				followPositions.Add(vRRigFromPlayer, valueOrDefault);
				((Component)VRRig.LocalRig).transform.position = valueOrDefault;
				((Component)VRRig.LocalRig).transform.LookAt(((Component)vRRigFromPlayer).transform.position);
				((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
				((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + ((Component)VRRig.LocalRig).transform.right * -1f;
				((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + ((Component)VRRig.LocalRig).transform.right * 1f;
				((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
				((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
				FixRigHandRotation();
				PhotonView photonView = VRRig.LocalRig.GetPhotonView();
				RaiseEventOptions val3 = new RaiseEventOptions();
				val3.TargetActors = new int[1] { val.ActorNumber };
				Main.SendSerialize(photonView, val3);
			}
			Main.RPCProtection();
			((Component)VRRig.LocalRig).transform.position = position;
			((Component)VRRig.LocalRig).transform.rotation = rotation;
			VRRig.LocalRig.leftHand.rigTarget.position = position2;
			VRRig.LocalRig.leftHand.rigTarget.rotation = rotation2;
			VRRig.LocalRig.rightHand.rigTarget.position = position3;
			VRRig.LocalRig.rightHand.rigTarget.rotation = rotation3;
			((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = rotation4;
			return false;
		};
	}

	public static void OrbitPlayerGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				((Behaviour)VRRig.LocalRig).enabled = false;
				((Component)VRRig.LocalRig).transform.position = ((Component)Main.lockTarget).transform.position + new Vector3(Mathf.Cos((float)Time.frameCount / 20f), 0.5f, Mathf.Sin((float)Time.frameCount / 20f));
				((Component)VRRig.LocalRig).transform.LookAt(((Component)Main.lockTarget).transform.position);
				((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
				((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + ((Component)VRRig.LocalRig).transform.right * -1f;
				((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + ((Component)VRRig.LocalRig).transform.right * 1f;
				((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
				((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
				FixRigHandRotation();
				((VRMap)VRRig.LocalRig.leftIndex).calcT = 0f;
				((VRMap)VRRig.LocalRig.leftMiddle).calcT = 0f;
				((VRMap)VRRig.LocalRig.leftThumb).calcT = 0f;
				((VRMap)VRRig.LocalRig.leftIndex).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.leftMiddle).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.leftThumb).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.rightIndex).calcT = 0f;
				((VRMap)VRRig.LocalRig.rightMiddle).calcT = 0f;
				((VRMap)VRRig.LocalRig.rightThumb).calcT = 0f;
				((VRMap)VRRig.LocalRig.rightIndex).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.rightMiddle).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.rightThumb).LerpFinger(1f, false);
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

	public static void OrbitAllPlayers()
	{
		SerializePatch.OverrideSerialization = delegate
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Expected O, but got Unknown
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
			Vector3 position = ((Component)VRRig.LocalRig).transform.position;
			Quaternion rotation = ((Component)VRRig.LocalRig).transform.rotation;
			Vector3 position2 = VRRig.LocalRig.leftHand.rigTarget.position;
			Quaternion rotation2 = VRRig.LocalRig.leftHand.rigTarget.rotation;
			Vector3 position3 = VRRig.LocalRig.rightHand.rigTarget.position;
			Quaternion rotation3 = VRRig.LocalRig.rightHand.rigTarget.rotation;
			Quaternion rotation4 = ((Component)VRRig.LocalRig.head.rigTarget).transform.rotation;
			NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
			foreach (NetPlayer val in playerListOthers)
			{
				VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(val);
				((Component)VRRig.LocalRig).transform.position = ((Component)vRRigFromPlayer).transform.position + new Vector3(Mathf.Cos((float)Time.frameCount / 20f), 0.5f, Mathf.Sin((float)Time.frameCount / 20f));
				((Component)VRRig.LocalRig).transform.LookAt(((Component)vRRigFromPlayer).transform.position);
				((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
				((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + ((Component)VRRig.LocalRig).transform.right * -1f;
				((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + ((Component)VRRig.LocalRig).transform.right * 1f;
				((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
				((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation;
				PhotonView photonView = VRRig.LocalRig.GetPhotonView();
				RaiseEventOptions val2 = new RaiseEventOptions();
				val2.TargetActors = new int[1] { val.ActorNumber };
				Main.SendSerialize(photonView, val2);
			}
			Main.RPCProtection();
			((Component)VRRig.LocalRig).transform.position = position;
			((Component)VRRig.LocalRig).transform.rotation = rotation;
			VRRig.LocalRig.leftHand.rigTarget.position = position2;
			VRRig.LocalRig.leftHand.rigTarget.rotation = rotation2;
			VRRig.LocalRig.rightHand.rigTarget.position = position3;
			VRRig.LocalRig.rightHand.rigTarget.rotation = rotation3;
			((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = rotation4;
			return false;
		};
	}

	public static void JumpscareGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				((Behaviour)VRRig.LocalRig).enabled = false;
				((Component)VRRig.LocalRig).transform.position = Main.lockTarget.headMesh.transform.position + Main.lockTarget.headMesh.transform.forward * Random.Range(0.1f, 0.5f);
				((Component)VRRig.LocalRig.head.rigTarget).transform.LookAt(Main.lockTarget.headMesh.transform.position);
				Quaternion rotation = ((Component)VRRig.LocalRig.head.rigTarget).transform.rotation;
				((Component)VRRig.LocalRig).transform.rotation = rotation;
				((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = Main.lockTarget.headMesh.transform.position + Main.lockTarget.headMesh.transform.right * 0.2f;
				((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = Main.lockTarget.headMesh.transform.position + Main.lockTarget.headMesh.transform.right * -0.2f;
				((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = rotation;
				Transform transform = ((Component)VRRig.LocalRig.leftHand.rigTarget).transform;
				Quaternion rotation2 = ((Component)VRRig.LocalRig).transform.rotation;
				transform.rotation = Quaternion.Euler(((Quaternion)(ref rotation2)).eulerAngles + new Vector3(0f, 180f, 0f));
				Transform transform2 = ((Component)VRRig.LocalRig.rightHand.rigTarget).transform;
				rotation2 = ((Component)VRRig.LocalRig).transform.rotation;
				transform2.rotation = Quaternion.Euler(((Quaternion)(ref rotation2)).eulerAngles + new Vector3(0f, 180f, 0f));
				FixRigHandRotation();
				((VRMap)VRRig.LocalRig.leftIndex).calcT = 0f;
				((VRMap)VRRig.LocalRig.leftMiddle).calcT = 0f;
				((VRMap)VRRig.LocalRig.leftThumb).calcT = 0f;
				((VRMap)VRRig.LocalRig.leftIndex).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.leftMiddle).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.leftThumb).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.rightIndex).calcT = 0f;
				((VRMap)VRRig.LocalRig.rightMiddle).calcT = 0f;
				((VRMap)VRRig.LocalRig.rightThumb).calcT = 0f;
				((VRMap)VRRig.LocalRig.rightIndex).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.rightMiddle).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.rightThumb).LerpFinger(1f, false);
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

	public static void JumpscareAll()
	{
		SerializePatch.OverrideSerialization = delegate
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Expected O, but got Unknown
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
			Vector3 position = ((Component)VRRig.LocalRig).transform.position;
			Quaternion rotation = ((Component)VRRig.LocalRig).transform.rotation;
			Vector3 position2 = VRRig.LocalRig.leftHand.rigTarget.position;
			Quaternion rotation2 = VRRig.LocalRig.leftHand.rigTarget.rotation;
			Vector3 position3 = VRRig.LocalRig.rightHand.rigTarget.position;
			Quaternion rotation3 = VRRig.LocalRig.rightHand.rigTarget.rotation;
			Quaternion rotation4 = ((Component)VRRig.LocalRig.head.rigTarget).transform.rotation;
			NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
			foreach (NetPlayer val in playerListOthers)
			{
				VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(val);
				((Component)VRRig.LocalRig).transform.position = vRRigFromPlayer.headMesh.transform.position + vRRigFromPlayer.headMesh.transform.forward * Random.Range(0.1f, 0.5f);
				((Component)VRRig.LocalRig.head.rigTarget).transform.LookAt(vRRigFromPlayer.headMesh.transform.position);
				Quaternion rotation5 = ((Component)VRRig.LocalRig.head.rigTarget).transform.rotation;
				((Component)VRRig.LocalRig).transform.rotation = rotation5;
				((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = vRRigFromPlayer.headMesh.transform.position + vRRigFromPlayer.headMesh.transform.right * 0.2f;
				((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = vRRigFromPlayer.headMesh.transform.position + vRRigFromPlayer.headMesh.transform.right * -0.2f;
				((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = rotation5;
				Transform transform = ((Component)VRRig.LocalRig.leftHand.rigTarget).transform;
				Quaternion rotation6 = ((Component)VRRig.LocalRig).transform.rotation;
				transform.rotation = Quaternion.Euler(((Quaternion)(ref rotation6)).eulerAngles + new Vector3(0f, 180f, 0f));
				Transform transform2 = ((Component)VRRig.LocalRig.rightHand.rigTarget).transform;
				rotation6 = ((Component)VRRig.LocalRig).transform.rotation;
				transform2.rotation = Quaternion.Euler(((Quaternion)(ref rotation6)).eulerAngles + new Vector3(0f, 180f, 0f));
				PhotonView photonView = VRRig.LocalRig.GetPhotonView();
				RaiseEventOptions val2 = new RaiseEventOptions();
				val2.TargetActors = new int[1] { val.ActorNumber };
				Main.SendSerialize(photonView, val2);
			}
			Main.RPCProtection();
			((Component)VRRig.LocalRig).transform.position = position;
			((Component)VRRig.LocalRig).transform.rotation = rotation;
			VRRig.LocalRig.leftHand.rigTarget.position = position2;
			VRRig.LocalRig.leftHand.rigTarget.rotation = rotation2;
			VRRig.LocalRig.rightHand.rigTarget.position = position3;
			VRRig.LocalRig.rightHand.rigTarget.rotation = rotation3;
			((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = rotation4;
			return false;
		};
	}

	public static void AnnoyPlayerGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				((Behaviour)VRRig.LocalRig).enabled = false;
				Vector3 position = ((Component)Main.lockTarget).transform.position + RandomUtilities.RandomVector3();
				((Component)VRRig.LocalRig).transform.position = position;
				((Component)VRRig.LocalRig).transform.LookAt(((Component)Main.lockTarget).transform.position);
				((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = RandomUtilities.RandomQuaternion();
				((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)Main.lockTarget).transform.position + RandomUtilities.RandomVector3();
				((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)Main.lockTarget).transform.position + RandomUtilities.RandomVector3();
				((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = RandomUtilities.RandomQuaternion();
				((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = RandomUtilities.RandomQuaternion();
				((VRMap)VRRig.LocalRig.leftIndex).calcT = 0f;
				((VRMap)VRRig.LocalRig.leftMiddle).calcT = 0f;
				((VRMap)VRRig.LocalRig.leftThumb).calcT = 0f;
				((VRMap)VRRig.LocalRig.leftIndex).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.leftMiddle).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.leftThumb).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.rightIndex).calcT = 0f;
				((VRMap)VRRig.LocalRig.rightMiddle).calcT = 0f;
				((VRMap)VRRig.LocalRig.rightThumb).calcT = 0f;
				((VRMap)VRRig.LocalRig.rightIndex).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.rightMiddle).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.rightThumb).LerpFinger(1f, false);
				Sound.SoundSpam(337, constant: true);
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

	public static void AnnoyAllPlayers()
	{
		SerializePatch.OverrideSerialization = delegate
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Expected O, but got Unknown
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
			Vector3 position = ((Component)VRRig.LocalRig).transform.position;
			Quaternion rotation = ((Component)VRRig.LocalRig).transform.rotation;
			Vector3 position2 = VRRig.LocalRig.leftHand.rigTarget.position;
			Quaternion rotation2 = VRRig.LocalRig.leftHand.rigTarget.rotation;
			Vector3 position3 = VRRig.LocalRig.rightHand.rigTarget.position;
			Quaternion rotation3 = VRRig.LocalRig.rightHand.rigTarget.rotation;
			Quaternion rotation4 = ((Component)VRRig.LocalRig.head.rigTarget).transform.rotation;
			NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
			foreach (NetPlayer val in playerListOthers)
			{
				VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(val);
				Vector3 position4 = ((Component)vRRigFromPlayer).transform.position + RandomUtilities.RandomVector3();
				((Component)VRRig.LocalRig).transform.position = position4;
				((Component)VRRig.LocalRig).transform.LookAt(((Component)vRRigFromPlayer).transform.position);
				((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = RandomUtilities.RandomQuaternion();
				((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)vRRigFromPlayer).transform.position + RandomUtilities.RandomVector3();
				((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)vRRigFromPlayer).transform.position + RandomUtilities.RandomVector3();
				((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = RandomUtilities.RandomQuaternion();
				((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = RandomUtilities.RandomQuaternion();
				PhotonView photonView = VRRig.LocalRig.GetPhotonView();
				RaiseEventOptions val2 = new RaiseEventOptions();
				val2.TargetActors = new int[1] { val.ActorNumber };
				Main.SendSerialize(photonView, val2);
			}
			Main.RPCProtection();
			((Component)VRRig.LocalRig).transform.position = position;
			((Component)VRRig.LocalRig).transform.rotation = rotation;
			VRRig.LocalRig.leftHand.rigTarget.position = position2;
			VRRig.LocalRig.leftHand.rigTarget.rotation = rotation2;
			VRRig.LocalRig.rightHand.rigTarget.position = position3;
			VRRig.LocalRig.rightHand.rigTarget.rotation = rotation3;
			((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = rotation4;
			return false;
		};
	}

	public static void ConfusePlayerGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				((Behaviour)VRRig.LocalRig).enabled = false;
				((Component)VRRig.LocalRig).transform.position = ((Component)Main.lockTarget).transform.position - new Vector3(0f, 2f, 0f);
				if (Time.time > Fun.splashDel)
				{
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RigUtilities.GetPlayerFromVRRig(Main.lockTarget), new object[6]
					{
						((Component)Main.lockTarget).transform.position + RandomUtilities.RandomVector3(0.5f),
						RandomUtilities.RandomQuaternion(),
						4f,
						100f,
						true,
						false
					});
					Main.RPCProtection();
					Fun.splashDel = Time.time + 0.1f;
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

	public static void ConfuseAllPlayers()
	{
		SerializePatch.OverrideSerialization = delegate
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Expected O, but got Unknown
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
			Vector3 position = ((Component)VRRig.LocalRig).transform.position;
			NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
			foreach (NetPlayer val in playerListOthers)
			{
				VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(val);
				((Component)VRRig.LocalRig).transform.position = ((Component)vRRigFromPlayer).transform.position - Vector3.up * 2f;
				PhotonView photonView = VRRig.LocalRig.GetPhotonView();
				RaiseEventOptions val2 = new RaiseEventOptions();
				val2.TargetActors = new int[1] { val.ActorNumber };
				Main.SendSerialize(photonView, val2);
			}
			Main.RPCProtection();
			((Component)VRRig.LocalRig).transform.position = position;
			return false;
		};
	}

	public static void ConfuseAllPlayersSplash()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > Fun.splashDel)
		{
			Fun.splashDel = Time.time + 0.05f;
			VRRig randomVRRig = RigUtilities.GetRandomVRRig(includeSelf: false);
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RigUtilities.GetPlayerFromVRRig(randomVRRig), new object[6]
			{
				((Component)randomVRRig).transform.position + RandomUtilities.RandomVector3(0.5f),
				RandomUtilities.RandomQuaternion(),
				4f,
				100f,
				true,
				false
			});
		}
	}

	public static void ChangeTinnitusHz(bool positive = true)
	{
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				targetHz += 500;
			}
			else
			{
				targetHz -= 500;
			}
		}
		if (targetHz > 7500)
		{
			targetHz = 4000;
		}
		if (targetHz < 4000)
		{
			targetHz = 7500;
		}
		Buttons.GetIndex("Change Tinnitus Hertz").overlapText = "Change Tinnitus Hertz <color=grey>[</color><color=green>" + targetHz + "</color><color=grey>]</color>";
	}

	public static AudioClip CreateTinnitusSound(float seconds = 180f)
	{
		VoiceManager voiceManager = VoiceManager.Get();
		int outputRate = voiceManager.OutputRate;
		int num = Mathf.Clamp(targetHz, 1, outputRate / 2 - 1);
		int num2 = Mathf.CeilToInt((float)outputRate * seconds);
		AudioClip val = AudioClip.Create("tinnitus", num2, 1, outputRate, false);
		float[] array = new float[num2];
		double num3 = (double)num / (double)outputRate;
		for (int i = 0; i < num2; i++)
		{
			array[i] = (float)Math.Sin(Math.PI * 2.0 * (double)num * (double)i / (double)outputRate);
		}
		val.SetData(array, 0);
		return val;
	}

	public static void TinnitusGun()
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
			if (!Object.op_Implicit((Object)(object)componentInParent) || componentInParent.IsLocal() || Main.gunLocked || !PhotonNetwork.InRoom)
			{
				return;
			}
			Main.gunLocked = true;
			Main.lockTarget = componentInParent;
			AudioClip val2 = CreateTinnitusSound();
			VoiceManager.Clip value = VoiceManager.Get().AudioClip(val2);
			tinnitus[val2] = value;
			NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.DebugEchoMode = tinnitusSelf;
			SerializePatch.OverrideSerialization = delegate
			{
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				//IL_0099: Expected O, but got Unknown
				//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
				//IL_0102: Unknown result type (might be due to invalid IL or missing references)
				//IL_0107: Unknown result type (might be due to invalid IL or missing references)
				//IL_0157: Expected O, but got Unknown
				//IL_0168: Unknown result type (might be due to invalid IL or missing references)
				NetPlayer target = RigUtilities.GetPlayerFromVRRig(Main.lockTarget);
				Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
				Vector3 position = ((Component)VRRig.LocalRig).transform.position;
				((Component)VRRig.LocalRig).transform.position = ((Component)Main.lockTarget).transform.position - Main.lockTarget.headMesh.transform.forward * 0.2f;
				PhotonView photonView = VRRig.LocalRig.GetPhotonView();
				RaiseEventOptions val3 = new RaiseEventOptions();
				val3.TargetActors = new int[1] { target.ActorNumber };
				Main.SendSerialize(photonView, val3);
				((Component)VRRig.LocalRig).transform.position = new Vector3(Random.Range(-99999f, 99999f), 99999f, Random.Range(-99999f, 99999f));
				Main.SendSerialize(VRRig.LocalRig.GetPhotonView(), new RaiseEventOptions
				{
					TargetActors = (from plr in PhotonNetwork.PlayerList
						where plr.ActorNumber != target.ActorNumber
						select plr.ActorNumber).ToArray()
				});
				Main.RPCProtection();
				((Component)VRRig.LocalRig).transform.position = position;
				return false;
			};
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
			Sound.FixMicrophone();
			SerializePatch.OverrideSerialization = null;
		}
	}

	public static void TinnitusAll()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		AudioClip val = CreateTinnitusSound();
		VoiceManager.Clip value = VoiceManager.Get().AudioClip(val);
		tinnitus[val] = value;
		NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.DebugEchoMode = tinnitusSelf;
		SerializePatch.OverrideSerialization = delegate
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Expected O, but got Unknown
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
			Vector3 position = ((Component)VRRig.LocalRig).transform.position;
			NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
			foreach (NetPlayer val2 in playerListOthers)
			{
				VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(val2);
				((Component)VRRig.LocalRig).transform.position = ((Component)vRRigFromPlayer).transform.position - vRRigFromPlayer.headMesh.transform.forward * 0.2f;
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

	public static void DisableTinnitus()
	{
		SerializePatch.OverrideSerialization = null;
		foreach (VoiceManager.Clip value in tinnitus.Values)
		{
			VoiceManager.Get().StopAudioClip(value);
		}
		tinnitus.Clear();
	}

	public static void OverstimulateGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				if (Time.time > Fun.splashDel)
				{
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RigUtilities.GetPlayerFromVRRig(Main.lockTarget), new object[6]
					{
						Main.lockTarget.headMesh.transform.TransformPoint(Vector3.forward * 0.2f),
						RandomUtilities.RandomQuaternion(),
						4f,
						100f,
						true,
						false
					});
					Fun.splashDel = Time.time + 0.1f;
				}
				if (Random.Range(0f, 1f) > 0.5f)
				{
					if (Time.time > soundSpamDelay)
					{
						soundSpamDelay = Time.time + 0.1f;
						GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", NetPlayer.op_Implicit(Main.lockTarget.GetPhotonPlayer()), new object[3]
						{
							Random.Range(336, 338),
							false,
							999999f
						});
					}
				}
				else if (Time.time > soundSpamDelay)
				{
					int[] array = new int[2]
					{
						Random.Range(40, 54),
						Random.Range(214, 221)
					};
					soundSpamDelay = Time.time + 0.1f;
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", NetPlayer.op_Implicit(Main.lockTarget.GetPhotonPlayer()), new object[3]
					{
						array[Random.Range(0, 1)],
						false,
						999999f
					});
				}
				if (Time.time > hoverboardSpawnDelay)
				{
					hoverboardSpawnDelay = Time.time + 0.25f;
					DataPerPlayer orCreatePlayerData = FreeHoverboardManager.instance.GetOrCreatePlayerData(NetworkSystem.Instance.LocalPlayer.ActorNumber);
					int num = (((Component)orCreatePlayerData.board0).gameObject.activeSelf ? ((!((Component)orCreatePlayerData.board1).gameObject.activeSelf) ? 1 : (1 - FreeHoverboardManager.instance.localPlayerLastSpawnedBoardIndex)) : 0);
					((NetworkSceneObject)FreeHoverboardManager.instance).photonView.RPC("DropBoard_RPC", Main.lockTarget.GetPhotonPlayer(), new object[6]
					{
						num == 1,
						BitPackUtils.PackWorldPosForNetwork(Main.lockTarget.headMesh.transform.TransformPoint(Vector3.forward * 0.2f)),
						BitPackUtils.PackQuaternionForNetwork(RandomUtilities.RandomQuaternion()),
						BitPackUtils.PackWorldPosForNetwork(RandomUtilities.RandomVector3(3f)),
						BitPackUtils.PackWorldPosForNetwork(RandomUtilities.RandomVector3(3f)),
						BitPackUtils.PackColorForNetwork(RandomUtilities.RandomColor())
					});
					FreeHoverboardManager.instance.localPlayerLastSpawnedBoardIndex = num;
				}
				Main.RPCProtection();
			}
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
			SerializePatch.OverrideSerialization = delegate
			{
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Expected O, but got Unknown
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0122: Expected O, but got Unknown
				NetPlayer target = RigUtilities.GetPlayerFromVRRig(Main.lockTarget);
				Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
				Vector3 position = ((Component)VRRig.LocalRig).transform.position;
				((Component)VRRig.LocalRig).transform.position = ((Component)Main.lockTarget).transform.position + RandomUtilities.RandomVector3();
				PhotonView photonView = VRRig.LocalRig.GetPhotonView();
				RaiseEventOptions val2 = new RaiseEventOptions();
				val2.TargetActors = new int[1] { target.ActorNumber };
				Main.SendSerialize(photonView, val2);
				Main.RPCProtection();
				((Component)VRRig.LocalRig).transform.position = position;
				Main.SendSerialize(VRRig.LocalRig.GetPhotonView(), new RaiseEventOptions
				{
					TargetActors = (from plr in PhotonNetwork.PlayerList
						where plr.ActorNumber != target.ActorNumber
						select plr.ActorNumber).ToArray()
				});
				return false;
			};
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
			SerializePatch.OverrideSerialization = null;
		}
	}

	public static void OverstimulateAll()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		if (SerializePatch.OverrideSerialization == null)
		{
			SerializePatch.OverrideSerialization = delegate
			{
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Expected O, but got Unknown
				//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
				Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
				Vector3 position = ((Component)VRRig.LocalRig).transform.position;
				NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
				foreach (NetPlayer val in playerListOthers)
				{
					VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(val);
					((Component)VRRig.LocalRig).transform.position = ((Component)vRRigFromPlayer).transform.position + RandomUtilities.RandomVector3();
					PhotonView photonView = VRRig.LocalRig.GetPhotonView();
					RaiseEventOptions val2 = new RaiseEventOptions();
					val2.TargetActors = new int[1] { val.ActorNumber };
					Main.SendSerialize(photonView, val2);
				}
				Main.RPCProtection();
				((Component)VRRig.LocalRig).transform.position = position;
				return false;
			};
		}
		VRRig randomVRRig = RigUtilities.GetRandomVRRig(includeSelf: false);
		if (Time.time > Fun.splashDel)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", RigUtilities.GetPlayerFromVRRig(randomVRRig), new object[6]
			{
				randomVRRig.headMesh.transform.TransformPoint(Vector3.forward * 0.2f),
				RandomUtilities.RandomQuaternion(),
				4f,
				100f,
				true,
				false
			});
			Fun.splashDel = Time.time + 0.1f;
		}
		if (Random.Range(0f, 1f) > 0.5f)
		{
			if (Time.time > soundSpamDelay)
			{
				soundSpamDelay = Time.time + 0.1f;
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", NetPlayer.op_Implicit(randomVRRig.GetPhotonPlayer()), new object[3]
				{
					Random.Range(336, 338),
					false,
					999999f
				});
			}
		}
		else if (Time.time > soundSpamDelay)
		{
			int[] array = new int[2]
			{
				Random.Range(40, 54),
				Random.Range(214, 221)
			};
			soundSpamDelay = Time.time + 0.1f;
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", NetPlayer.op_Implicit(randomVRRig.GetPhotonPlayer()), new object[3]
			{
				array[Random.Range(0, 1)],
				false,
				999999f
			});
		}
		if (Time.time > hoverboardSpawnDelay)
		{
			hoverboardSpawnDelay = Time.time + 0.25f;
			DataPerPlayer orCreatePlayerData = FreeHoverboardManager.instance.GetOrCreatePlayerData(NetworkSystem.Instance.LocalPlayer.ActorNumber);
			int num = (((Component)orCreatePlayerData.board0).gameObject.activeSelf ? ((!((Component)orCreatePlayerData.board1).gameObject.activeSelf) ? 1 : (1 - FreeHoverboardManager.instance.localPlayerLastSpawnedBoardIndex)) : 0);
			((NetworkSceneObject)FreeHoverboardManager.instance).photonView.RPC("DropBoard_RPC", randomVRRig.GetPhotonPlayer(), new object[6]
			{
				num == 1,
				BitPackUtils.PackWorldPosForNetwork(randomVRRig.headMesh.transform.TransformPoint(Vector3.forward * 0.2f)),
				BitPackUtils.PackQuaternionForNetwork(RandomUtilities.RandomQuaternion()),
				BitPackUtils.PackWorldPosForNetwork(RandomUtilities.RandomVector3(3f)),
				BitPackUtils.PackWorldPosForNetwork(RandomUtilities.RandomVector3(3f)),
				BitPackUtils.PackColorForNetwork(RandomUtilities.RandomColor())
			});
			FreeHoverboardManager.instance.localPlayerLastSpawnedBoardIndex = num;
		}
	}

	public static void ShutdownHeadsetGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Fun.HoverboardScreenTarget(Main.lockTarget, Color.black);
			}
			if (!Main.GetGunInput(isShooting: true))
			{
				return;
			}
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				Main.gunLocked = true;
				Main.lockTarget = componentInParent;
				AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Mods/Fun/shutdown.ogg", "Audio/Mods/Fun/shutdown.ogg", delegate(AudioClip clip)
				{
					Sound.PlayAudio(clip);
				});
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void ShutdownHeadsetAll()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Fun.HoverboardScreenAll(Color.black);
	}

	public static void SchizophrenicGun()
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
			SerializePatch.OverrideSerialization = delegate
			{
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a6: Expected O, but got Unknown
				//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ef: Expected O, but got Unknown
				//IL_0126: Unknown result type (might be due to invalid IL or missing references)
				NetPlayer target = RigUtilities.GetPlayerFromVRRig(Main.lockTarget);
				Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
				Vector3 position = ((Component)VRRig.LocalRig).transform.position;
				Main.SendSerialize(VRRig.LocalRig.GetPhotonView(), new RaiseEventOptions
				{
					TargetActors = (from plr in PhotonNetwork.PlayerList
						where plr.ActorNumber != target.ActorNumber
						select plr.ActorNumber).ToArray()
				});
				((Component)VRRig.LocalRig).transform.position = new Vector3(Random.Range(-99999f, 99999f), 99999f, Random.Range(-99999f, 99999f));
				PhotonView photonView = VRRig.LocalRig.GetPhotonView();
				RaiseEventOptions val2 = new RaiseEventOptions();
				val2.TargetActors = new int[1] { target.ActorNumber };
				Main.SendSerialize(photonView, val2);
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

	public static void ReverseSchizoGun()
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
			SerializePatch.OverrideSerialization = delegate
			{
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Expected O, but got Unknown
				//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0115: Expected O, but got Unknown
				//IL_0126: Unknown result type (might be due to invalid IL or missing references)
				NetPlayer target = RigUtilities.GetPlayerFromVRRig(Main.lockTarget);
				Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
				Vector3 position = ((Component)VRRig.LocalRig).transform.position;
				PhotonView photonView = VRRig.LocalRig.GetPhotonView();
				RaiseEventOptions val2 = new RaiseEventOptions();
				val2.TargetActors = new int[1] { target.ActorNumber };
				Main.SendSerialize(photonView, val2);
				((Component)VRRig.LocalRig).transform.position = new Vector3(Random.Range(-99999f, 99999f), 99999f, Random.Range(-99999f, 99999f));
				Main.SendSerialize(VRRig.LocalRig.GetPhotonView(), new RaiseEventOptions
				{
					TargetActors = (from plr in PhotonNetwork.PlayerList
						where plr.ActorNumber != target.ActorNumber
						select plr.ActorNumber).ToArray()
				});
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

	public static void IntercourseGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				((Behaviour)VRRig.LocalRig).enabled = false;
				if (!Buttons.GetIndex("Reverse Intercourse").enabled)
				{
					((Component)VRRig.LocalRig).transform.position = ((Component)Main.lockTarget).transform.position + ((Component)Main.lockTarget).transform.forward * (0f - (0.2f + Mathf.Sin((float)Time.frameCount / 8f) * 0.1f));
					((Component)VRRig.LocalRig).transform.rotation = ((Component)Main.lockTarget).transform.rotation;
					((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)Main.lockTarget).transform.position + ((Component)Main.lockTarget).transform.right * -0.2f + ((Component)Main.lockTarget).transform.up * -0.4f;
					((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)Main.lockTarget).transform.position + ((Component)Main.lockTarget).transform.right * 0.2f + ((Component)Main.lockTarget).transform.up * -0.4f;
					((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = ((Component)Main.lockTarget).transform.rotation;
					((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = ((Component)Main.lockTarget).transform.rotation;
				}
				else
				{
					((Component)VRRig.LocalRig).transform.position = ((Component)Main.lockTarget).transform.position + ((Component)Main.lockTarget).transform.forward * (0.2f + Mathf.Sin((float)Time.frameCount / 8f) * 0.1f);
					((Component)VRRig.LocalRig).transform.rotation = ((Component)Main.lockTarget).transform.rotation;
					((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)Main.lockTarget).transform.position + ((Component)Main.lockTarget).transform.right * -0.2f + ((Component)Main.lockTarget).transform.up * -0.4f;
					((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)Main.lockTarget).transform.position + ((Component)Main.lockTarget).transform.right * 0.2f + ((Component)Main.lockTarget).transform.up * -0.4f;
					Transform transform = ((Component)VRRig.LocalRig.leftHand.rigTarget).transform;
					Quaternion rotation = ((Component)Main.lockTarget).transform.rotation;
					transform.rotation = Quaternion.Euler(((Quaternion)(ref rotation)).eulerAngles + new Vector3(0f, 180f, 0f));
					Transform transform2 = ((Component)VRRig.LocalRig.rightHand.rigTarget).transform;
					rotation = ((Component)Main.lockTarget).transform.rotation;
					transform2.rotation = Quaternion.Euler(((Quaternion)(ref rotation)).eulerAngles + new Vector3(0f, 180f, 0f));
					((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = ((Component)Main.lockTarget).transform.rotation;
				}
				FixRigHandRotation();
				IntercourseNoises();
				((VRMap)VRRig.LocalRig.leftIndex).calcT = 0f;
				((VRMap)VRRig.LocalRig.leftMiddle).calcT = 0f;
				((VRMap)VRRig.LocalRig.leftThumb).calcT = 0f;
				((VRMap)VRRig.LocalRig.leftIndex).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.leftMiddle).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.leftThumb).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.rightIndex).calcT = 0f;
				((VRMap)VRRig.LocalRig.rightMiddle).calcT = 0f;
				((VRMap)VRRig.LocalRig.rightThumb).calcT = 0f;
				((VRMap)VRRig.LocalRig.rightIndex).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.rightMiddle).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.rightThumb).LerpFinger(1f, false);
				((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = ((Component)Main.lockTarget).transform.rotation;
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

	public static void IntercourseNoises()
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		if (Time.frameCount % 45 != 0)
		{
			return;
		}
		if (PhotonNetwork.InRoom)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", (RpcTarget)0, new object[3] { 64, true, 999999f });
			if (Buttons.GetIndex("Splash Intercourse").enabled)
			{
				Fun.BetaWaterSplash(((Component)VRRig.LocalRig).transform.position, ((Component)VRRig.LocalRig).transform.rotation, 4f, 100f, bigSplash: true, enteringWater: false);
			}
			Main.RPCProtection();
		}
		else
		{
			VRRig.LocalRig.PlayHandTapLocal(64, true, 999999f);
		}
	}

	public static void IntercourseAll()
	{
		SerializePatch.OverrideSerialization = delegate
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0500: Unknown result type (might be due to invalid IL or missing references)
			//IL_0517: Unknown result type (might be due to invalid IL or missing references)
			//IL_0533: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_041d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_045f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0466: Expected O, but got Unknown
			Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
			Vector3 position = ((Component)VRRig.LocalRig).transform.position;
			Quaternion rotation = ((Component)VRRig.LocalRig).transform.rotation;
			Vector3 position2 = VRRig.LocalRig.leftHand.rigTarget.position;
			Quaternion rotation2 = VRRig.LocalRig.leftHand.rigTarget.rotation;
			Vector3 position3 = VRRig.LocalRig.rightHand.rigTarget.position;
			Quaternion rotation3 = VRRig.LocalRig.rightHand.rigTarget.rotation;
			Quaternion rotation4 = ((Component)VRRig.LocalRig.head.rigTarget).transform.rotation;
			NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
			foreach (NetPlayer val in playerListOthers)
			{
				VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(val);
				if (!Buttons.GetIndex("Reverse Intercourse").enabled)
				{
					((Component)VRRig.LocalRig).transform.position = ((Component)vRRigFromPlayer).transform.position + ((Component)vRRigFromPlayer).transform.forward * (0f - (0.2f + Mathf.Sin((float)Time.frameCount / 8f) * 0.1f));
					((Component)VRRig.LocalRig).transform.rotation = ((Component)vRRigFromPlayer).transform.rotation;
					((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)vRRigFromPlayer).transform.position + ((Component)vRRigFromPlayer).transform.right * -0.2f + ((Component)vRRigFromPlayer).transform.up * -0.4f;
					((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)vRRigFromPlayer).transform.position + ((Component)vRRigFromPlayer).transform.right * 0.2f + ((Component)vRRigFromPlayer).transform.up * -0.4f;
					((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = ((Component)vRRigFromPlayer).transform.rotation;
					((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = ((Component)vRRigFromPlayer).transform.rotation;
				}
				else
				{
					((Component)VRRig.LocalRig).transform.position = ((Component)vRRigFromPlayer).transform.position + ((Component)vRRigFromPlayer).transform.forward * (0.2f + Mathf.Sin((float)Time.frameCount / 8f) * 0.1f);
					((Component)VRRig.LocalRig).transform.rotation = ((Component)vRRigFromPlayer).transform.rotation;
					((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)vRRigFromPlayer).transform.position + ((Component)vRRigFromPlayer).transform.right * -0.2f + ((Component)vRRigFromPlayer).transform.up * -0.4f;
					((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)vRRigFromPlayer).transform.position + ((Component)vRRigFromPlayer).transform.right * 0.2f + ((Component)vRRigFromPlayer).transform.up * -0.4f;
					Transform transform = ((Component)VRRig.LocalRig.leftHand.rigTarget).transform;
					Quaternion rotation5 = ((Component)vRRigFromPlayer).transform.rotation;
					transform.rotation = Quaternion.Euler(((Quaternion)(ref rotation5)).eulerAngles + new Vector3(0f, 180f, 0f));
					Transform transform2 = ((Component)VRRig.LocalRig.rightHand.rigTarget).transform;
					rotation5 = ((Component)vRRigFromPlayer).transform.rotation;
					transform2.rotation = Quaternion.Euler(((Quaternion)(ref rotation5)).eulerAngles + new Vector3(0f, 180f, 0f));
					((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = ((Component)vRRigFromPlayer).transform.rotation;
				}
				FixRigHandRotation();
				PhotonView photonView = VRRig.LocalRig.GetPhotonView();
				RaiseEventOptions val2 = new RaiseEventOptions();
				val2.TargetActors = new int[1] { val.ActorNumber };
				Main.SendSerialize(photonView, val2);
			}
			Main.RPCProtection();
			((Component)VRRig.LocalRig).transform.position = position;
			((Component)VRRig.LocalRig).transform.rotation = rotation;
			VRRig.LocalRig.leftHand.rigTarget.position = position2;
			VRRig.LocalRig.leftHand.rigTarget.rotation = rotation2;
			VRRig.LocalRig.rightHand.rigTarget.position = position3;
			VRRig.LocalRig.rightHand.rigTarget.rotation = rotation3;
			((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = rotation4;
			return false;
		};
	}

	public static void PromptForSex()
	{
		Main.Prompt("You have to be age verified to use this mod. Would you like to proceed to the age verification process?", delegate
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>SEX</color><color=grey>]</color> A browser tab has been opened on your computer.");
			Main.PromptSingle("A browser tab has been opened on your computer. Please go and verify your age.", null, "Ok frick off buddy");
			Application.OpenURL("https://seralyth.software/age_verification");
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(Sex());
		});
	}

	public static IEnumerator Sex()
	{
		while (Application.isFocused)
		{
			yield return null;
		}
		float time = Time.time + 5f;
		while (!Application.isFocused && time > Time.time)
		{
			yield return null;
		}
		GameObject sex = AssetUtilities.LoadObject<GameObject>("sex");
		sex.layer = 8;
		Rigidbody rb = sex.GetComponent<Rigidbody>();
		rb.useGravity = false;
		sex.transform.position = VRRig.LocalRig.headMesh.transform.position + VRRig.LocalRig.headMesh.transform.forward * 1.5f;
		sex.transform.LookAt(VRRig.LocalRig.headMesh.transform);
		AudioClip clip = null;
		bool loaded = false;
		AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Mods/Fun/sex.ogg", "Audio/Mods/Fun/sex.ogg", delegate(AudioClip c)
		{
			clip = c;
			loaded = true;
		});
		while (!loaded)
		{
			yield return null;
		}
		AudioSource ausrc = Main.audioManager.GetComponent<AudioSource>();
		ausrc.volume = 1f;
		ausrc.PlayOneShot(clip);
		yield return (object)new WaitForSeconds(clip.length);
		rb.useGravity = true;
		float grabRadius = 0.3f;
		Transform activeHand = null;
		Vector3 velocity = Vector3.zero;
		while (true)
		{
			Transform rightHand = GTPlayer.Instance.rightHand.controllerTransform;
			Transform leftHand = GTPlayer.Instance.leftHand.controllerTransform;
			bool rightHolding = (Object)(object)activeHand == (Object)(object)rightHand && Main.rightGrab;
			bool leftHolding = (Object)(object)activeHand == (Object)(object)leftHand && Main.leftGrab;
			if ((Object)(object)activeHand != (Object)null)
			{
				if (rightHolding || leftHolding)
				{
					velocity = activeHand.position;
					sex.transform.SetPositionAndRotation(activeHand.position, activeHand.rotation);
					if ((Object)(object)rb != (Object)null)
					{
						rb.isKinematic = true;
					}
				}
				else
				{
					if ((Object)(object)rb != (Object)null)
					{
						rb.isKinematic = false;
						rb.linearVelocity = (activeHand.position - velocity) / Time.deltaTime;
					}
					activeHand = null;
				}
			}
			else if (Main.rightGrab && (Object)(object)rightHand != (Object)null && Vector3.Distance(sex.transform.position, rightHand.position) <= grabRadius)
			{
				activeHand = rightHand;
			}
			else if (Main.leftGrab && (Object)(object)leftHand != (Object)null && Vector3.Distance(sex.transform.position, leftHand.position) <= grabRadius)
			{
				activeHand = leftHand;
			}
			yield return null;
		}
	}

	public static void HeadGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				((Behaviour)VRRig.LocalRig).enabled = false;
				Quaternion rotation;
				if (!Buttons.GetIndex("Reverse Intercourse").enabled)
				{
					((Component)VRRig.LocalRig).transform.position = ((Component)Main.lockTarget).transform.position + ((Component)Main.lockTarget).transform.forward * (0.2f + Mathf.Sin((float)Time.frameCount / 8f) * 0.1f) + ((Component)Main.lockTarget).transform.up * -0.4f;
					Transform transform = ((Component)VRRig.LocalRig).transform;
					rotation = ((Component)Main.lockTarget).transform.rotation;
					transform.rotation = Quaternion.Euler(((Quaternion)(ref rotation)).eulerAngles + new Vector3(0f, 180f, 0f));
					((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)Main.lockTarget).transform.position + ((Component)Main.lockTarget).transform.right * 0.2f + ((Component)Main.lockTarget).transform.up * -0.4f;
					((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)Main.lockTarget).transform.position + ((Component)Main.lockTarget).transform.right * -0.2f + ((Component)Main.lockTarget).transform.up * -0.4f;
					Transform transform2 = ((Component)VRRig.LocalRig.leftHand.rigTarget).transform;
					rotation = ((Component)Main.lockTarget).transform.rotation;
					transform2.rotation = Quaternion.Euler(((Quaternion)(ref rotation)).eulerAngles + new Vector3(0f, 180f, 0f));
					Transform transform3 = ((Component)VRRig.LocalRig.rightHand.rigTarget).transform;
					rotation = ((Component)Main.lockTarget).transform.rotation;
					transform3.rotation = Quaternion.Euler(((Quaternion)(ref rotation)).eulerAngles + new Vector3(0f, 180f, 0f));
					Transform transform4 = ((Component)VRRig.LocalRig.head.rigTarget).transform;
					rotation = ((Component)Main.lockTarget).transform.rotation;
					transform4.rotation = Quaternion.Euler(((Quaternion)(ref rotation)).eulerAngles + new Vector3(0f, 180f, 0f));
				}
				else
				{
					((Component)VRRig.LocalRig).transform.position = ((Component)Main.lockTarget).transform.position + ((Component)Main.lockTarget).transform.forward * (0.2f + Mathf.Sin((float)Time.frameCount / 8f) * 0.1f) + ((Component)Main.lockTarget).transform.up * 0.4f;
					Transform transform5 = ((Component)VRRig.LocalRig).transform;
					rotation = ((Component)Main.lockTarget).transform.rotation;
					transform5.rotation = Quaternion.Euler(((Quaternion)(ref rotation)).eulerAngles + new Vector3(0f, 180f, 0f));
					((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)Main.lockTarget).transform.position + ((Component)Main.lockTarget).transform.right * 0.2f + ((Component)Main.lockTarget).transform.up * 0.4f;
					((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)Main.lockTarget).transform.position + ((Component)Main.lockTarget).transform.right * -0.2f + ((Component)Main.lockTarget).transform.up * 0.4f;
					Transform transform6 = ((Component)VRRig.LocalRig.leftHand.rigTarget).transform;
					rotation = ((Component)Main.lockTarget).transform.rotation;
					transform6.rotation = Quaternion.Euler(((Quaternion)(ref rotation)).eulerAngles + new Vector3(0f, 180f, 0f));
					Transform transform7 = ((Component)VRRig.LocalRig.rightHand.rigTarget).transform;
					rotation = ((Component)Main.lockTarget).transform.rotation;
					transform7.rotation = Quaternion.Euler(((Quaternion)(ref rotation)).eulerAngles + new Vector3(0f, 180f, 0f));
					Transform transform8 = ((Component)VRRig.LocalRig.head.rigTarget).transform;
					rotation = ((Component)Main.lockTarget).transform.rotation;
					transform8.rotation = Quaternion.Euler(((Quaternion)(ref rotation)).eulerAngles + new Vector3(0f, 180f, 0f));
				}
				((VRMap)VRRig.LocalRig.leftIndex).calcT = 0f;
				((VRMap)VRRig.LocalRig.leftMiddle).calcT = 0f;
				((VRMap)VRRig.LocalRig.leftThumb).calcT = 0f;
				((VRMap)VRRig.LocalRig.leftIndex).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.leftMiddle).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.leftThumb).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.rightIndex).calcT = 0f;
				((VRMap)VRRig.LocalRig.rightMiddle).calcT = 0f;
				((VRMap)VRRig.LocalRig.rightThumb).calcT = 0f;
				((VRMap)VRRig.LocalRig.rightIndex).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.rightMiddle).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.rightThumb).LerpFinger(1f, false);
				FixRigHandRotation();
				IntercourseNoises();
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

	public static void HeadAll()
	{
		SerializePatch.OverrideSerialization = delegate
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0603: Unknown result type (might be due to invalid IL or missing references)
			//IL_0619: Unknown result type (might be due to invalid IL or missing references)
			//IL_062f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0645: Unknown result type (might be due to invalid IL or missing references)
			//IL_065c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0678: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_0440: Unknown result type (might be due to invalid IL or missing references)
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0477: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0514: Unknown result type (might be due to invalid IL or missing references)
			//IL_0519: Unknown result type (might be due to invalid IL or missing references)
			//IL_051d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0531: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0561: Unknown result type (might be due to invalid IL or missing references)
			//IL_0566: Unknown result type (might be due to invalid IL or missing references)
			//IL_056a: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0583: Unknown result type (might be due to invalid IL or missing references)
			//IL_0588: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ab: Expected O, but got Unknown
			Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
			Vector3 position = ((Component)VRRig.LocalRig).transform.position;
			Quaternion rotation = ((Component)VRRig.LocalRig).transform.rotation;
			Vector3 position2 = VRRig.LocalRig.leftHand.rigTarget.position;
			Quaternion rotation2 = VRRig.LocalRig.leftHand.rigTarget.rotation;
			Vector3 position3 = VRRig.LocalRig.rightHand.rigTarget.position;
			Quaternion rotation3 = VRRig.LocalRig.rightHand.rigTarget.rotation;
			Quaternion rotation4 = ((Component)VRRig.LocalRig.head.rigTarget).transform.rotation;
			NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
			foreach (NetPlayer val in playerListOthers)
			{
				VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(val);
				Quaternion rotation5;
				if (!Buttons.GetIndex("Reverse Intercourse").enabled)
				{
					((Component)VRRig.LocalRig).transform.position = ((Component)vRRigFromPlayer).transform.position + ((Component)vRRigFromPlayer).transform.forward * (0.2f + Mathf.Sin((float)Time.frameCount / 8f) * 0.1f) + ((Component)vRRigFromPlayer).transform.up * -0.4f;
					Transform transform = ((Component)VRRig.LocalRig).transform;
					rotation5 = ((Component)vRRigFromPlayer).transform.rotation;
					transform.rotation = Quaternion.Euler(((Quaternion)(ref rotation5)).eulerAngles + new Vector3(0f, 180f, 0f));
					((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)vRRigFromPlayer).transform.position + ((Component)vRRigFromPlayer).transform.right * 0.2f + ((Component)vRRigFromPlayer).transform.up * -0.4f;
					((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)vRRigFromPlayer).transform.position + ((Component)vRRigFromPlayer).transform.right * -0.2f + ((Component)vRRigFromPlayer).transform.up * -0.4f;
					Transform transform2 = ((Component)VRRig.LocalRig.leftHand.rigTarget).transform;
					rotation5 = ((Component)vRRigFromPlayer).transform.rotation;
					transform2.rotation = Quaternion.Euler(((Quaternion)(ref rotation5)).eulerAngles + new Vector3(0f, 180f, 0f));
					Transform transform3 = ((Component)VRRig.LocalRig.rightHand.rigTarget).transform;
					rotation5 = ((Component)vRRigFromPlayer).transform.rotation;
					transform3.rotation = Quaternion.Euler(((Quaternion)(ref rotation5)).eulerAngles + new Vector3(0f, 180f, 0f));
					Transform transform4 = ((Component)VRRig.LocalRig.head.rigTarget).transform;
					rotation5 = ((Component)vRRigFromPlayer).transform.rotation;
					transform4.rotation = Quaternion.Euler(((Quaternion)(ref rotation5)).eulerAngles + new Vector3(0f, 180f, 0f));
				}
				else
				{
					((Component)VRRig.LocalRig).transform.position = ((Component)vRRigFromPlayer).transform.position + ((Component)vRRigFromPlayer).transform.forward * (0.2f + Mathf.Sin((float)Time.frameCount / 8f) * 0.1f) + ((Component)vRRigFromPlayer).transform.up * 0.4f;
					Transform transform5 = ((Component)VRRig.LocalRig).transform;
					rotation5 = ((Component)vRRigFromPlayer).transform.rotation;
					transform5.rotation = Quaternion.Euler(((Quaternion)(ref rotation5)).eulerAngles + new Vector3(0f, 180f, 0f));
					((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)vRRigFromPlayer).transform.position + ((Component)vRRigFromPlayer).transform.right * 0.2f + ((Component)vRRigFromPlayer).transform.up * 0.4f;
					((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)vRRigFromPlayer).transform.position + ((Component)vRRigFromPlayer).transform.right * -0.2f + ((Component)vRRigFromPlayer).transform.up * 0.4f;
					Transform transform6 = ((Component)VRRig.LocalRig.leftHand.rigTarget).transform;
					rotation5 = ((Component)vRRigFromPlayer).transform.rotation;
					transform6.rotation = Quaternion.Euler(((Quaternion)(ref rotation5)).eulerAngles + new Vector3(0f, 180f, 0f));
					Transform transform7 = ((Component)VRRig.LocalRig.rightHand.rigTarget).transform;
					rotation5 = ((Component)vRRigFromPlayer).transform.rotation;
					transform7.rotation = Quaternion.Euler(((Quaternion)(ref rotation5)).eulerAngles + new Vector3(0f, 180f, 0f));
					Transform transform8 = ((Component)VRRig.LocalRig.head.rigTarget).transform;
					rotation5 = ((Component)vRRigFromPlayer).transform.rotation;
					transform8.rotation = Quaternion.Euler(((Quaternion)(ref rotation5)).eulerAngles + new Vector3(0f, 180f, 0f));
				}
				FixRigHandRotation();
				PhotonView photonView = VRRig.LocalRig.GetPhotonView();
				RaiseEventOptions val2 = new RaiseEventOptions();
				val2.TargetActors = new int[1] { val.ActorNumber };
				Main.SendSerialize(photonView, val2);
			}
			Main.RPCProtection();
			((Component)VRRig.LocalRig).transform.position = position;
			((Component)VRRig.LocalRig).transform.rotation = rotation;
			VRRig.LocalRig.leftHand.rigTarget.position = position2;
			VRRig.LocalRig.leftHand.rigTarget.rotation = rotation2;
			VRRig.LocalRig.rightHand.rigTarget.position = position3;
			VRRig.LocalRig.rightHand.rigTarget.rotation = rotation3;
			((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = rotation4;
			return false;
		};
	}

	public static void RemoveCopy()
	{
		Main.gunLocked = false;
		Main.lockTarget = null;
		((Behaviour)VRRig.LocalRig).enabled = true;
	}

	public static void SpazHead()
	{
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (((Behaviour)VRRig.LocalRig).enabled)
		{
			VRRig.LocalRig.head.trackingRotationOffset.x = Random.Range(0f, 360f);
			VRRig.LocalRig.head.trackingRotationOffset.y = Random.Range(0f, 360f);
			VRRig.LocalRig.head.trackingRotationOffset.z = Random.Range(0f, 360f);
		}
		else
		{
			((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = RandomUtilities.RandomQuaternion();
		}
	}

	public static void RandomSpazHead()
	{
		if (headspazType)
		{
			SpazHead();
			if (Time.time > headspazDelay)
			{
				headspazType = false;
				headspazDelay = Time.time + Random.Range(1000f, 4000f) / 1000f;
			}
		}
		else
		{
			Fun.FixHead();
			if (Time.time > headspazDelay)
			{
				headspazType = true;
				headspazDelay = Time.time + Random.Range(200f, 1000f) / 1000f;
			}
		}
	}

	public static void EnableSpazHead()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		headoffs = VRRig.LocalRig.head.trackingPositionOffset;
	}

	public static void SpazHeadPosition()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		VRRig.LocalRig.head.trackingPositionOffset = headoffs + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
	}

	public static void FixHeadPosition()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		VRRig.LocalRig.head.trackingPositionOffset = headoffs;
	}

	public static void RandomSpazHeadPosition()
	{
		if (headspazType)
		{
			SpazHeadPosition();
			if (Time.time > headspazDelay)
			{
				headspazType = false;
				headspazDelay = Time.time + Random.Range(1000f, 4000f) / 1000f;
			}
		}
		else
		{
			FixHeadPosition();
			if (Time.time > headspazDelay)
			{
				headspazType = true;
				headspazDelay = Time.time + Random.Range(200f, 1000f) / 1000f;
			}
		}
	}

	public static void LaggyRig()
	{
		((Behaviour)VRRig.LocalRig).enabled = false;
		Main.ghostException = true;
		if (Time.time > laggyRigDelay)
		{
			((Behaviour)VRRig.LocalRig).enabled = true;
			VRRig.LocalRig.PostTick();
			((Behaviour)VRRig.LocalRig).enabled = false;
			laggyRigDelay = Time.time + 0.5f;
		}
	}

	public static void UpdateRig()
	{
		((Behaviour)VRRig.LocalRig).enabled = false;
		Main.ghostException = true;
		if (Main.rightPrimary && !wasRightPrimaryPressed)
		{
			((Behaviour)VRRig.LocalRig).enabled = true;
			VRRig.LocalRig.PostTick();
			((Behaviour)VRRig.LocalRig).enabled = false;
		}
		wasRightPrimaryPressed = Main.rightPrimary;
	}

	public static void MultiplicationAmount(bool positive = true)
	{
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				multiplicationAmount++;
			}
			else
			{
				multiplicationAmount--;
			}
		}
		multiplicationAmount %= 1281;
		if (multiplicationAmount < 0)
		{
			multiplicationAmount = 1280;
		}
		Buttons.GetIndex("Knockback Multiplication Amount").overlapText = "Knockback Multiplication Amount <color=grey>[</color><color=green>" + (float)multiplicationAmount / 10f + "</color><color=grey>]</color>";
	}

	public static IEnumerator Zoom(float from, float to)
	{
		float t = 0f;
		while (t < 0.2f)
		{
			XRDevice.fovZoomFactor = Mathf.Lerp(from, to, t / 0.2f);
			t += Time.deltaTime;
			yield return null;
		}
		XRDevice.fovZoomFactor = to;
	}

	public static void VRRigLateUpdate_BackFlip()
	{
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		VRRig localRig = VRRig.LocalRig;
		if (!Object.op_Implicit((Object)(object)localRig) || !Object.op_Implicit((Object)(object)((Component)localRig).transform))
		{
			return;
		}
		if (backflip.active)
		{
			float num = (Time.time - backflip.start) / 0.5f;
			if (num >= 1f)
			{
				backflip.active = false;
				lastBackflipEnd = Time.time;
				((Component)localRig).transform.rotation = backflip.rot;
				((Component)localRig.head.rigTarget).transform.rotation = backflip.headRot;
			}
			else
			{
				Quaternion val = Quaternion.AngleAxis(backflip.dir * 360f * num, backflip.axis);
				((Component)localRig).transform.rotation = val * backflip.rot;
				((Component)localRig.head.rigTarget).transform.rotation = val * backflip.headRot;
			}
			return;
		}
		bool leftSecondary = Main.leftSecondary;
		if (leftSecondary && !wasBackflipPressed && Time.time - lastBackflipEnd >= 1f)
		{
			FlipState flipState = new FlipState
			{
				active = true,
				rot = ((Component)localRig).transform.rotation,
				headRot = ((Component)localRig.head.rigTarget).transform.rotation
			};
			Vector3 right = ((Component)localRig).transform.right;
			flipState.axis = ((Vector3)(ref right)).normalized;
			flipState.start = Time.time;
			flipState.dir = -1f;
			backflip = flipState;
		}
		wasBackflipPressed = leftSecondary;
	}

	public static void Zoom()
	{
		if (Main.rightTrigger > 0.5f && !Zoomed)
		{
			ShouldZoom = !ShouldZoom;
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(Zoom(ShouldZoom ? 1f : 4f, ShouldZoom ? 4f : 1f));
		}
		Zoomed = Main.rightTrigger > 0.5f;
	}

	public static void TrackPlayer()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)trackedPlayer == (Object)null || !trackedPlayer.Active())
		{
			trackedPlayer = RigUtilities.GetClosestVRRig();
		}
		if ((Object)(object)trackedPlayer == (Object)null || trackedPlayer.IsLocal())
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=purple>TRACK</color><color=grey>]</color> No players to track.");
			return;
		}
		trackDistance = Vector3.Distance(((Component)VRRig.LocalRig).transform.position, ((Component)trackedPlayer).transform.position);
		NotificationManager.information["Tracking"] = trackedPlayer.GetName() + " (" + trackDistance.ToString("F1") + "m)";
		if (trackFollowing)
		{
			Vector3 val = ((Component)trackedPlayer).transform.position - ((Component)VRRig.LocalRig).transform.position;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			float num = FlySpeed * 2f;
			((Collider)GTPlayer.Instance.bodyCollider).attachedRigidbody.linearVelocity = normalized * num;
		}
	}

	public static void DisableTrackPlayer()
	{
		NotificationManager.information.Remove("Tracking");
		trackedPlayer = null;
		trackDistance = 0f;
		trackFollowing = false;
	}

	public static void TeleportToTracked()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)trackedPlayer == (Object)null || !trackedPlayer.Active())
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=purple>TRACK</color><color=grey>]</color> No player tracked.");
			return;
		}
		Main.closePosition = Vector3.zero;
		Main.TeleportPlayer(trackedPlayer.headMesh.transform.position);
		GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
		VRRig.LocalRig.PlayHandTapLocal(50, Main.rightHand, 0.4f);
		NotificationManager.SendNotification("<color=grey>[</color><color=purple>TRACK</color><color=grey>]</color> Teleported to <color=green>" + trackedPlayer.GetName() + "</color>.");
	}

	public static void CycleTrackedPlayer()
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		List<VRRig> list = VRRigCache.ActiveRigs.Where((VRRig r) => !r.IsLocal()).ToList();
		if (list.Count == 0)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=purple>TRACK</color><color=grey>]</color> No players to track.");
			return;
		}
		int num = (((Object)(object)trackedPlayer != (Object)null) ? list.IndexOf(trackedPlayer) : (-1));
		int index = (num + 1) % list.Count;
		trackedPlayer = list[index];
		trackDistance = Vector3.Distance(((Component)VRRig.LocalRig).transform.position, ((Component)trackedPlayer).transform.position);
		VRRig.LocalRig.PlayHandTapLocal(50, Main.rightHand, 0.4f);
		NotificationManager.SendNotification("<color=grey>[</color><color=purple>TRACK</color><color=grey>]</color> Tracking <color=green>" + trackedPlayer.GetName() + "</color> (" + trackDistance.ToString("F1") + "m).");
	}

	private static void SaveTeleports()
	{
		try
		{
			SavedTeleportData savedTeleportData = new SavedTeleportData();
			foreach (SavedTeleportEntry savedTeleport in savedTeleports)
			{
				savedTeleportData.spots.Add(savedTeleport);
			}
			File.WriteAllText(SavedTeleportsPath, JsonUtility.ToJson((object)savedTeleportData, true));
		}
		catch
		{
		}
	}

	private static void LoadTeleports()
	{
		try
		{
			if (!File.Exists(SavedTeleportsPath))
			{
				return;
			}
			string text = File.ReadAllText(SavedTeleportsPath);
			SavedTeleportData savedTeleportData = JsonUtility.FromJson<SavedTeleportData>(text);
			if (savedTeleportData == null || savedTeleportData.spots == null)
			{
				return;
			}
			savedTeleports.Clear();
			foreach (SavedTeleportEntry spot in savedTeleportData.spots)
			{
				if (!string.IsNullOrEmpty(spot.name))
				{
					savedTeleports.Add(spot);
				}
			}
		}
		catch
		{
		}
	}

	public static void OpenQuickTeleports()
	{
		if (!teleportsLoaded)
		{
			LoadTeleports();
			teleportsLoaded = true;
		}
		rememberPageNumber = Main.pageNumber;
		List<ButtonInfo> list = new List<ButtonInfo>
		{
			new ButtonInfo
			{
				buttonText = "Exit Quick Teleports",
				method = delegate
				{
					Buttons.CurrentCategoryName = "Movement Mods";
				},
				isTogglable = false,
				toolTip = "Returns you back to the movement mods."
			}
		};
		list.Add(new ButtonInfo
		{
			buttonText = "Save Current Position",
			overlapText = "Save Position <color=grey>[</color><color=green>" + savedTeleports.Count + "/5</color><color=grey>]</color>",
			method = SaveCurrentPosition,
			isTogglable = false,
			toolTip = "Saves your current position as a quick teleport spot."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Go to Spawn",
			method = delegate
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				Main.TeleportPlayer(((Component)VRRig.LocalRig).transform.position);
			},
			isTogglable = false,
			toolTip = "Teleports you to the spawn point."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Go to Stump",
			method = delegate
			{
				TeleportToMap("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/TreeRoomSpawnForestZone", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Forest, Tree Exit");
			},
			isTogglable = false,
			toolTip = "Teleports you to the forest stump."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Go to City",
			method = delegate
			{
				TeleportToMap("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/ForestToCity", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - City Front");
			},
			isTogglable = false,
			toolTip = "Teleports you to the city."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Go to Caves",
			method = delegate
			{
				TeleportToMap("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/ForestToCave", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Cave");
			},
			isTogglable = false,
			toolTip = "Teleports you to the caves."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Go to Beach",
			method = delegate
			{
				TeleportToMap("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/BeachToForest", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Beach for Computer");
			},
			isTogglable = false,
			toolTip = "Teleports you to the beach."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Go to Canyons",
			method = delegate
			{
				TeleportToMap("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/ForestCanyonTransition", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Canyon");
			},
			isTogglable = false,
			toolTip = "Teleports you to the canyons."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Go to Mountains",
			method = delegate
			{
				TeleportToMap("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/CityToMountain", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Mountain");
			},
			isTogglable = false,
			toolTip = "Teleports you to the mountains."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Go to Clouds",
			method = delegate
			{
				TeleportToMap("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/CityToSkyJungle", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Clouds From Computer");
			},
			isTogglable = false,
			toolTip = "Teleports you to the clouds."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Go to Basement",
			method = delegate
			{
				TeleportToMap("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/CityToBasement", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Basement For Computer");
			},
			isTogglable = false,
			toolTip = "Teleports you to the basement."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Go to Bayou",
			method = delegate
			{
				TeleportToMap("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/BayouOnly", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - BayouComputer2");
			},
			isTogglable = false,
			toolTip = "Teleports you to the bayou."
		});
		if (savedTeleports.Count > 0)
		{
			list.Add(new ButtonInfo
			{
				buttonText = "--- Saved Spots ---",
				label = true,
				toolTip = "Your saved teleport spots."
			});
			for (int num = 0; num < savedTeleports.Count; num++)
			{
				int idx = num;
				SavedTeleportEntry spot = savedTeleports[num];
				list.Add(new ButtonInfo
				{
					buttonText = "TeleportSaved" + num,
					overlapText = "<color=green>" + spot.name + "</color>",
					method = delegate
					{
						//IL_0022: Unknown result type (might be due to invalid IL or missing references)
						//IL_0038: Unknown result type (might be due to invalid IL or missing references)
						Main.TeleportPlayer(new Vector3(spot.x, spot.y, spot.z), keepVelocity: false);
						GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
						VRRig.LocalRig.PlayHandTapLocal(50, Main.rightHand, 0.4f);
						NotificationManager.SendNotification("<color=grey>[</color><color=yellow>TELEPORT</color><color=grey>]</color> Teleported to <color=green>" + spot.name + "</color>.");
					},
					isTogglable = false,
					toolTip = "Teleports you to " + spot.name + "."
				});
				list.Add(new ButtonInfo
				{
					buttonText = "DeleteSaved" + num,
					overlapText = "<color=red>Delete " + spot.name + "</color>",
					method = delegate
					{
						string name = savedTeleports[idx].name;
						savedTeleports.RemoveAt(idx);
						SaveTeleports();
						OpenQuickTeleports();
						NotificationManager.SendNotification("<color=grey>[</color><color=yellow>TELEPORT</color><color=grey>]</color> Deleted <color=red>" + name + "</color>.");
					},
					isTogglable = false,
					toolTip = "Deletes the saved spot " + spot.name + "."
				});
			}
			list.Add(new ButtonInfo
			{
				buttonText = "Clear All Saved Spots",
				method = delegate
				{
					savedTeleports.Clear();
					SaveTeleports();
					OpenQuickTeleports();
					NotificationManager.SendNotification("<color=grey>[</color><color=yellow>TELEPORT</color><color=grey>]</color> Cleared all saved spots.");
				},
				isTogglable = false,
				toolTip = "Deletes all saved teleport spots."
			});
		}
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	private static void SaveCurrentPosition()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (savedTeleports.Count >= 5)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=yellow>TELEPORT</color><color=grey>]</color> Max 5 saved spots. Delete one first.");
			return;
		}
		Vector3 position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
		string text = "Spot " + (savedTeleports.Count + 1);
		savedTeleports.Add(new SavedTeleportEntry
		{
			name = text,
			x = position.x,
			y = position.y,
			z = position.z
		});
		SaveTeleports();
		OpenQuickTeleports();
		NotificationManager.SendNotification("<color=grey>[</color><color=yellow>TELEPORT</color><color=grey>]</color> Saved <color=green>" + text + "</color> at your position.");
	}

	public static void Flip()
	{
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		bool enabled = Buttons.GetIndex("Silent Flip").enabled;
		if (!flipping && (Main.rightPrimary || Main.rightSecondary) && ((Behaviour)VRRig.LocalRig).enabled && Object.op_Implicit((Object)(object)GTPlayer.Instance.playerRigidBody))
		{
			flipping = true;
			flipStart = Time.time;
			flipAxis = (Main.rightPrimary ? ((Component)VRRig.LocalRig).transform.right : (-((Component)VRRig.LocalRig).transform.right));
			GTPlayer instance = GTPlayer.Instance;
			Quaternion? obj;
			if (instance == null)
			{
				obj = null;
			}
			else
			{
				Rigidbody playerRigidBody = instance.playerRigidBody;
				obj = ((playerRigidBody != null) ? new Quaternion?(playerRigidBody.rotation) : ((Quaternion?)null));
			}
			flipFrom = (Quaternion)(((_003F?)obj) ?? Quaternion.identity);
		}
		if (!flipping)
		{
			return;
		}
		float num = (Time.time - flipStart) / 1f;
		if (num >= 1f)
		{
			flipping = false;
			if (enabled)
			{
				((Component)VRRig.LocalRig).transform.rotation = flipFrom;
			}
			else
			{
				GTPlayerTransform.ApplyRotationOverride(ref flipFrom, Time.frameCount);
			}
		}
		else
		{
			Quaternion val = Quaternion.AngleAxis(-360f * num, flipAxis) * flipFrom;
			if (enabled)
			{
				((Component)VRRig.LocalRig).transform.rotation = Quaternion.Euler(0f, ((Component)GorillaTagger.Instance.bodyCollider).transform.eulerAngles.y, 0f) * val;
			}
			else
			{
				GTPlayerTransform.ApplyRotationOverride(ref val, Time.frameCount);
			}
		}
	}
}
