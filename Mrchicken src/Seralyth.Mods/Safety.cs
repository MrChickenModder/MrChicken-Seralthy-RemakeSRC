using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using Seralyth.Extensions;
using Seralyth.Managers;
using Seralyth.Menu;
using Seralyth.Patches.Menu;
using Seralyth.Patches.Safety;
using Seralyth.Utilities;
using UnityEngine;
using UnityEngine.Networking;

namespace Seralyth.Mods;

public static class Safety
{
	private static bool antiOculusReportHooked;

	public static Vector3 deadPosition = Vector3.zero;

	public static Vector3 lvel = Vector3.zero;

	private static string previousNickName;

	public static float flushCooldown;

	private static float lastCacheClearedTime;

	public static int antiReportRangeIndex;

	public static float threshold = 0.35f;

	public static bool smartAntiReport;

	public static int buttonClickTime;

	public static string buttonClickPlayer;

	public static bool antiMute;

	public static VRRig reportRig;

	public static float antiReportDelay;

	private static bool afrEnabled;

	private static string afrPendingReport;

	private static readonly HashSet<string> afrReportedPlayers = new HashSet<string>();

	public static float antiReportNotifyDelay;

	private static readonly HashSet<string> afMuted = new HashSet<string>();

	private static string ptTargetId;

	private static float ptTimeout;

	private static bool ptActive;

	private static bool previousSpecial;

	private static float lastVol;

	private static float startSilenceTime = -1f;

	private static bool reloaded;

	private static Vector3 smoothedLeftHandPosition;

	private static Vector3 smoothedRightHandPosition;

	private static bool previouslyInLobby;

	private static readonly List<VRRig> nameSpoofRigs = new List<VRRig>();

	private static readonly List<VRRig> colorSpoofRigs = new List<VRRig>();

	public static int fpsSpoofValue = 90;

	public static int pingSpoofValue = 200;

	public static readonly string[] namePrefix = new string[15]
	{
		"EPIC", "EPIK", "REAL", "NOT", "SILLY", "LITTLE", "BIG", "MAYBE", "MONKE", "SUB2",
		"OG", "FUN", "FR", "NOT", "NOTA"
	};

	public static readonly string[] nameSuffix = new string[11]
	{
		"GT", "VR", "LOL", "GTVR", "FAN", "XD", "LOL", "MONK", "YT", "NOT",
		"FR"
	};

	public static readonly string[] names = new string[89]
	{
		"0", "SHIBA", "PBBV", "J3VU", "BEES", "NAMO", "MANGO", "FROSTY", "FRISH", "LEMMING",
		"BILLY", "TIMMY", "MINIGAMES", "JMANCURLY", "VMT", "ELLIOT", "POLAR", "3CLIPCE", "DAISY09", "SHARKPUPPET",
		"DUCKY", "EDDIE", "EDDY", "RAKZZ", "CASEOH", "SKETCH", "SKY", "RETURN", "WATERMELON", "CRAZY",
		"MONK", "MONKE", "MONKI", "MONKEY", "MONKIY", "GORILL", "GOORILA", "GORILLA", "REDBERRY", "FOX",
		"RUFUS", "TTT", "TTTPIG", "PPPTIG", "K9", "BTC", "TICKLETIPJR", "BANANA", "PEANUTBUTTER", "GHOSTMONKE",
		"STATUE", "TURBOALLEN", "NOVA", "LUNAR", "MOON", "SUN", "RANDOM", "UNKNOWN", "GLITCH", "BUG",
		"ERROR", "CODE", "HACKER", "MODDER", "INVIS", "INVISIBLE", "TAGGER", "UNTAGGED", "BLUE", "RED",
		"GREEN", "PURPLE", "YELLOW", "BLACK", "WHITE", "BROWN", "CYAN", "GRAY", "GREY", "BANNED",
		"LEMON", "PLUSHIE", "CHEETO", "TIKTOK", "YOUTUBE", "TWITCH", "DISCORD", "MODDER", "HACKER"
	};

	public static string targetRank = "High";

	public static int rankIndex = 2;

	public static bool spoofingPlatform;

	public static int targetElo = 4000;

	public static int targetBadge = 7;

	private const string ArsPlayersUrl = "https://raw.githubusercontent.com/AutoReportSystem/ARSPlayerIDs/refs/heads/main/Player%20Ids.txt";

	private static string[] arsPlayersToReport;

	private static Coroutine arsCoroutine;

	public static void GeneralSafety()
	{
		if (!Buttons.GetIndex("Anti Report <color=grey>[</color><color=green>Disconnect</color><color=grey>]</color>").enabled)
		{
			AntiReportDisconnect();
		}
		if (!Buttons.GetIndex("Anti Report <color=grey>[</color><color=green>Anti Cheat</color><color=grey>]</color>").enabled)
		{
			AntiCheatPatches.SendReportPatch.AntiACReport = true;
		}
		if (!Buttons.GetIndex("Anti Moderator").enabled)
		{
			AntiModerator();
		}
		if (!Buttons.GetIndex("Anti Report <color=grey>[</color><color=green>Oculus</color><color=grey>]</color>").enabled && !antiOculusReportHooked)
		{
			antiOculusReportHooked = true;
			EnableAntiOculusReport();
		}
	}

	public static void DisableGeneral()
	{
		if (!Buttons.GetIndex("Anti Report <color=grey>[</color><color=green>Anti Cheat</color><color=grey>]</color>").enabled)
		{
			AntiCheatPatches.SendReportPatch.AntiACReport = false;
		}
		if (!Buttons.GetIndex("Anti Report <color=grey>[</color><color=green>Oculus</color><color=grey>]</color>").enabled)
		{
			DisableAntiOculusReport();
		}
	}

	public static void NoFinger()
	{
		((ControllerInputPoller)ControllerInputPoller.instance).leftControllerGripFloat = 0f;
		((ControllerInputPoller)ControllerInputPoller.instance).rightControllerGripFloat = 0f;
		((ControllerInputPoller)ControllerInputPoller.instance).leftControllerIndexFloat = 0f;
		((ControllerInputPoller)ControllerInputPoller.instance).rightControllerIndexFloat = 0f;
		((ControllerInputPoller)ControllerInputPoller.instance).leftControllerPrimaryButton = false;
		((ControllerInputPoller)ControllerInputPoller.instance).leftControllerSecondaryButton = false;
		((ControllerInputPoller)ControllerInputPoller.instance).rightControllerPrimaryButton = false;
		((ControllerInputPoller)ControllerInputPoller.instance).rightControllerSecondaryButton = false;
		((ControllerInputPoller)ControllerInputPoller.instance).leftControllerPrimaryButtonTouch = false;
		((ControllerInputPoller)ControllerInputPoller.instance).leftControllerSecondaryButtonTouch = false;
		((ControllerInputPoller)ControllerInputPoller.instance).rightControllerPrimaryButtonTouch = false;
		((ControllerInputPoller)ControllerInputPoller.instance).rightControllerSecondaryButtonTouch = false;
	}

	public static void SetGamemodeButtonActive(bool active = true)
	{
		Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/ModeSelector_Group").SetActive(active);
	}

	public static void FakeOculusMenu()
	{
		if (Main.leftPrimary)
		{
			NoFinger();
			ConnectedControllerHandler.Instance.leftHandFollower.UpdatePositionRotation();
			ConnectedControllerHandler.Instance.rightHandFollower.UpdatePositionRotation();
		}
		Movement.SetHandEnabled(!Main.leftPrimary);
	}

	public static void FakeReportMenu()
	{
		if (Main.leftSecondary)
		{
			NoFinger();
		}
		GTPlayer.Instance.inOverlay = Main.leftPrimary;
	}

	public static void FakeBrokenController()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = (Main.leftPrimary ? GorillaTagger.Instance.leftHandTransform.position : GorillaTagger.Instance.rightHandTransform.position);
		Quaternion rotation = (Main.leftPrimary ? GorillaTagger.Instance.leftHandTransform.rotation : GorillaTagger.Instance.rightHandTransform.rotation);
		GTPlayer.Instance.GetControllerTransform(true).position = ((Component)GTPlayer.Instance.headCollider).transform.position + ((Component)GTPlayer.Instance.headCollider).transform.up * (-0.5f * GTPlayer.Instance.scale);
		GTPlayer.Instance.GetControllerTransform(true).rotation = ((Component)Camera.main).transform.rotation * Quaternion.Euler(-55f, 90f, 0f);
		GTPlayer.Instance.GetControllerTransform(false).position = position;
		GTPlayer.Instance.GetControllerTransform(false).rotation = rotation;
		((ControllerInputPoller)ControllerInputPoller.instance).leftControllerGripFloat = 0f;
		((ControllerInputPoller)ControllerInputPoller.instance).leftControllerIndexFloat = 0f;
		((ControllerInputPoller)ControllerInputPoller.instance).leftControllerPrimaryButton = false;
		((ControllerInputPoller)ControllerInputPoller.instance).leftControllerSecondaryButton = false;
		((ControllerInputPoller)ControllerInputPoller.instance).leftControllerPrimaryButtonTouch = false;
		((ControllerInputPoller)ControllerInputPoller.instance).leftControllerSecondaryButtonTouch = false;
	}

	public static void FakePowerOff()
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (Main.leftJoystickClick)
		{
			if (deadPosition == Vector3.zero)
			{
				deadPosition = ((Component)GorillaTagger.Instance.rigidbody).transform.position;
				lvel = GorillaTagger.Instance.rigidbody.linearVelocity;
			}
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)GorillaTagger.Instance.rigidbody).transform.position = deadPosition;
			GorillaTagger.Instance.rigidbody.linearVelocity = lvel;
		}
		else
		{
			deadPosition = Vector3.zero;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void FakeValveTracking()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightJoystickClick)
		{
			((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = Quaternion.identity;
		}
	}

	public static void SpoofSupportPage()
	{
		((GorillaComputer)GorillaComputer.instance).screenText.Set(((GorillaComputer)GorillaComputer.instance).screenText.currentText.Replace("STEAM", "QUEST").Replace(((GorillaComputer)GorillaComputer.instance).buildDate, "05/30/2024 16:50:12\nBUILD CODE 4893\nMANAGED ACCOUNT: NO"));
	}

	public static void AntiNameBan()
	{
		if (previousNickName != PhotonNetwork.LocalPlayer.NickName && !BanPatches.CheckAutoBanListForName.CheckBanList(PhotonNetwork.LocalPlayer.NickName))
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>WARNING</color><color=grey>]</color> Your name, " + PhotonNetwork.LocalPlayer.NickName + ", is not allowed. It has been reset for your safety.");
			Main.ChangeName(RandomUtilities.RandomString(8));
		}
		previousNickName = PhotonNetwork.LocalPlayer.NickName;
	}

	public static void FlushRPCs()
	{
		if (Time.time > flushCooldown)
		{
			Main.RPCProtection();
			flushCooldown = Time.time + 5f;
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not meant to spam Flush RPCs. Only call it once after you are done spamming RPCs.");
		}
	}

	public static void AntiLurker()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		LurkerGhost lurker = Overpowered.Lurker;
		if ((int)lurker.currentState == 3 && lurker.targetPlayer == NetworkSystem.Instance.LocalPlayer)
		{
			lurker.ChangeState((ghostState)0);
		}
	}

	public static void AutoClearCache()
	{
		if (Time.time > lastCacheClearedTime)
		{
			lastCacheClearedTime = Time.time + 60f;
			GC.Collect();
		}
	}

	public static void ChangeAntiReportRange(bool positive = true)
	{
		string[] array = new string[3] { "Default", "Large", "Massive" };
		float[] array2 = new float[3] { 0.35f, 0.7f, 1.5f };
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				antiReportRangeIndex++;
			}
			else
			{
				antiReportRangeIndex--;
			}
		}
		antiReportRangeIndex %= array.Length;
		if (antiReportRangeIndex < 0)
		{
			antiReportRangeIndex = array.Length - 1;
		}
		threshold = array2[antiReportRangeIndex];
		Buttons.GetIndex("Change Anti Report Distance").overlapText = "Change Anti Report Distance <color=grey>[</color><color=green>" + array[antiReportRangeIndex] + "</color><color=grey>]</color>";
	}

	public static bool SmartAntiReport(NetPlayer linePlayer)
	{
		return smartAntiReport && linePlayer.UserId == buttonClickPlayer && Time.frameCount == buttonClickTime && PhotonNetwork.CurrentRoom.IsVisible && !((object)((RoomInfo)PhotonNetwork.CurrentRoom).CustomProperties).ToString().Contains("MODDED");
	}

	public static void EventReceived_SmartAntiReport(EventData data)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (data.Code == 200)
			{
				string text = PhotonNetwork.PhotonServerSettings.RpcList[int.Parse(((Hashtable)data.CustomData)[(byte)5].ToString())];
				object[] array = (object[])((Hashtable)data.CustomData)[(byte)4];
				if (text == "RPC_PlayHandTap" && (int)array[0] == 67)
				{
					buttonClickTime = Time.frameCount;
					buttonClickPlayer = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false).UserId;
				}
			}
		}
		catch
		{
		}
	}

	public static void EnableSmartAntiReport()
	{
		PhotonNetwork.NetworkingClient.EventReceived += EventReceived_SmartAntiReport;
		smartAntiReport = true;
	}

	public static void DisableSmartAntiReport()
	{
		PhotonNetwork.NetworkingClient.EventReceived -= EventReceived_SmartAntiReport;
		smartAntiReport = false;
	}

	public static void VisualizeAntiReport()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		foreach (GorillaPlayerScoreboardLine allScoreboardLine in GorillaScoreboardTotalUpdater.allScoreboardLines)
		{
			if (allScoreboardLine.linePlayer == NetworkSystem.Instance.LocalPlayer)
			{
				Transform transform = ((Component)allScoreboardLine.reportButton).gameObject.transform;
				Visuals.VisualizeAura(transform.position, threshold, Color.red);
				if (antiMute)
				{
					Visuals.VisualizeAura(((Component)allScoreboardLine.muteButton).gameObject.transform.position, threshold, Color.red);
				}
			}
		}
	}

	private static bool OverlappingButton(VRRig vrrig, Vector3 position)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		return ((IEnumerable<Vector3>)(object)new Vector3[4]
		{
			vrrig.rightHandTransform.position,
			vrrig.leftHandTransform.position,
			vrrig.rightHand.syncPos,
			vrrig.leftHand.syncPos
		}).Any((Vector3 handPos) => Vector3.Distance(handPos, position) < threshold);
	}

	public static void AntiReport(Action<VRRig, Vector3> onReport)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		if ((Object)(object)reportRig != (Object)null)
		{
			onReport?.Invoke(reportRig, ((Component)reportRig).transform.position);
			reportRig = null;
			AchievementManager.UnlockAchievement(new AchievementManager.Achievement
			{
				name = "Troublemaker",
				description = "Evade a player report.",
				icon = "Images/Achievements/troublemaker.png"
			});
			return;
		}
		foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
		{
			if (line.linePlayer != NetworkSystem.Instance.LocalPlayer)
			{
				continue;
			}
			Transform report = ((Component)line.reportButton).gameObject.transform;
			foreach (VRRig item in from vrrig in VRRigCache.ActiveRigs
				where !vrrig.isLocal
				where OverlappingButton(vrrig, report.position) || (antiMute && OverlappingButton(vrrig, ((Component)line.muteButton).gameObject.transform.position))
				where !smartAntiReport || SmartAntiReport(line.linePlayer)
				select vrrig)
			{
				onReport?.Invoke(item, ((Component)report).transform.position);
			}
		}
	}

	public static void AntiReportDisconnect()
	{
		AntiReport(delegate(VRRig vrrig, Vector3 position)
		{
			NetworkSystem.Instance.ReturnToSinglePlayer();
			Main.RPCProtection();
			if (Time.time > antiReportDelay)
			{
				antiReportDelay = Time.time + 1f;
				NotificationManager.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + RigUtilities.GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, you have been disconnected.");
			}
		});
	}

	public static void AntiReportReconnect()
	{
		AntiReport(delegate(VRRig vrrig, Vector3 position)
		{
			if (Time.time > antiReportDelay)
			{
				Important.Reconnect();
				Main.RPCProtection();
				antiReportDelay = Time.time + 1f;
				NotificationManager.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + RigUtilities.GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, you have been disconnected and will be reconnected shortly.");
			}
		});
	}

	public static void AntiReportJoinRandom()
	{
		AntiReport(delegate(VRRig vrrig, Vector3 position)
		{
			if (Time.time > antiReportDelay)
			{
				Important.JoinRandom();
				Main.RPCProtection();
				antiReportDelay = Time.time + 1f;
				NotificationManager.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + RigUtilities.GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, you have been disconnected and will be reconnected shortly.");
			}
		});
	}

	public static void EventReceived_AntiOculusReport(EventData data)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (data.Code != 200)
			{
				return;
			}
			string text = PhotonNetwork.PhotonServerSettings.RpcList[int.Parse(((Hashtable)data.CustomData)[(byte)5].ToString())];
			object[] array = (object[])((Hashtable)data.CustomData)[(byte)4];
			if (text == "RPC_PlayHandTap" && (int)array[0] == 67)
			{
				VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(NetPlayer.op_Implicit(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false)));
				if (Vector3.Distance(vRRigFromPlayer.leftHandTransform.position, vRRigFromPlayer.rightHandTransform.position) < 0.1f)
				{
					AntiReportFRT(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false));
				}
			}
		}
		catch
		{
		}
	}

	public static void EnableAntiOculusReport()
	{
		PhotonNetwork.NetworkingClient.EventReceived += EventReceived_AntiOculusReport;
	}

	public static void DisableAntiOculusReport()
	{
		PhotonNetwork.NetworkingClient.EventReceived -= EventReceived_AntiOculusReport;
	}

	public static void AntiReportNotify()
	{
		if (!(Time.time > antiReportNotifyDelay))
		{
			return;
		}
		string notifyText = "";
		AntiReport(delegate(VRRig vrrig, Vector3 position)
		{
			antiReportNotifyDelay = Time.time + 0.1f;
			if (notifyText == "")
			{
				notifyText = RigUtilities.GetPlayerFromVRRig(vrrig).NickName;
			}
			else if (notifyText.Contains("&"))
			{
				notifyText = RigUtilities.GetPlayerFromVRRig(vrrig).NickName + ", " + notifyText;
			}
			else
			{
				notifyText = notifyText + " & " + RigUtilities.GetPlayerFromVRRig(vrrig).NickName;
			}
		});
		if (notifyText != "")
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + notifyText + " " + ((notifyText.Contains("&") || notifyText.Contains(",")) ? "are" : "is") + " reporting you.");
		}
	}

	public static void AntiReportOverlay()
	{
		if (!(Time.time > antiReportNotifyDelay))
		{
			return;
		}
		string notifyText = null;
		AntiReport(delegate(VRRig vrrig, Vector3 position)
		{
			if (notifyText == null)
			{
				notifyText = RigUtilities.GetPlayerFromVRRig(vrrig).NickName;
			}
			else if (notifyText.Contains("&"))
			{
				notifyText = RigUtilities.GetPlayerFromVRRig(vrrig).NickName + ", " + notifyText;
			}
			else
			{
				notifyText = notifyText + " & " + RigUtilities.GetPlayerFromVRRig(vrrig).NickName;
			}
		});
		if (StringUtils.IsNullOrEmpty(notifyText))
		{
			NotificationManager.information.Remove("Anti-Report");
		}
		else
		{
			NotificationManager.information["Anti-Report"] = notifyText;
		}
	}

	private static void EventReceived_AFR(EventData data)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		if (!afrEnabled || data.Code != 200)
		{
			return;
		}
		try
		{
			string text = PhotonNetwork.PhotonServerSettings.RpcList[int.Parse(((Hashtable)data.CustomData)[(byte)5].ToString())];
			if (text != "RPC_PlayHandTap")
			{
				return;
			}
			object[] array = (object[])((Hashtable)data.CustomData)[(byte)4];
			if (array.Length == 0 || (int)array[0] != 67)
			{
				return;
			}
			NetPlayer val = NetPlayer.op_Implicit(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false));
			if (val == null || val == NetworkSystem.Instance.LocalPlayer)
			{
				return;
			}
			VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(val);
			if ((Object)(object)vRRigFromPlayer == (Object)null)
			{
				return;
			}
			foreach (GorillaPlayerScoreboardLine allScoreboardLine in GorillaScoreboardTotalUpdater.allScoreboardLines)
			{
				if (allScoreboardLine.linePlayer != NetworkSystem.Instance.LocalPlayer || !OverlappingButton(vRRigFromPlayer, ((Component)allScoreboardLine.reportButton).gameObject.transform.position))
				{
					continue;
				}
				afrPendingReport = val.UserId;
				break;
			}
		}
		catch
		{
		}
	}

	public static void EnableAFR()
	{
		afrEnabled = true;
		PhotonNetwork.NetworkingClient.EventReceived += EventReceived_AFR;
	}

	public static void DisableAFR()
	{
		afrEnabled = false;
		PhotonNetwork.NetworkingClient.EventReceived -= EventReceived_AFR;
		afrReportedPlayers.Clear();
	}

	public static void AFR()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		foreach (GorillaPlayerScoreboardLine allScoreboardLine in GorillaScoreboardTotalUpdater.allScoreboardLines)
		{
			if (allScoreboardLine.linePlayer != null && afrReportedPlayers.Contains(allScoreboardLine.linePlayer.UserId))
			{
				allScoreboardLine.SetReportState(false, (ButtonType)1);
				allScoreboardLine.reportButton.isOn = true;
				allScoreboardLine.reportButton.UpdateColor();
			}
		}
		if (!string.IsNullOrEmpty(afrPendingReport))
		{
			string id = afrPendingReport;
			afrPendingReport = null;
			NetPlayer playerFromID = RigUtilities.GetPlayerFromID(id);
			if (playerFromID != null && playerFromID != NetworkSystem.Instance.LocalPlayer)
			{
				HandleAFRReport(playerFromID);
			}
		}
		if ((Object)(object)reportRig != (Object)null && (Object)(object)reportRig != (Object)(object)VRRig.LocalRig)
		{
			VRRig p = reportRig;
			reportRig = null;
			NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(p);
			if (playerFromVRRig != null && playerFromVRRig != NetworkSystem.Instance.LocalPlayer)
			{
				HandleAFRReport(playerFromVRRig);
			}
		}
	}

	private static void HandleAFRReport(NetPlayer reporter)
	{
		Main.RPCProtection();
		afrReportedPlayers.Add(reporter.UserId);
		foreach (GorillaPlayerScoreboardLine allScoreboardLine in GorillaScoreboardTotalUpdater.allScoreboardLines)
		{
			if (allScoreboardLine.linePlayer != reporter)
			{
				continue;
			}
			allScoreboardLine.SetReportState(false, (ButtonType)1);
			allScoreboardLine.reportButton.isOn = true;
			allScoreboardLine.reportButton.UpdateColor();
			break;
		}
		GorillaPlayerScoreboardLine.ReportPlayer(reporter.UserId, (ButtonType)1, reporter.NickName);
		NotificationManager.SendNotification("<color=grey>[</color><color=red>AFR</color><color=grey>]</color> Auto Flush Report: reported " + reporter.NickName);
	}

	public static void EnableAF()
	{
		afMuted.Clear();
	}

	public static void DisableAF()
	{
		afMuted.Clear();
	}

	public static void AF()
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		foreach (GorillaPlayerScoreboardLine allScoreboardLine in GorillaScoreboardTotalUpdater.allScoreboardLines)
		{
			if (allScoreboardLine.linePlayer != NetworkSystem.Instance.LocalPlayer)
			{
				continue;
			}
			foreach (VRRig activeRig in VRRigCache.ActiveRigs)
			{
				if (activeRig.isLocal || !OverlappingButton(activeRig, ((Component)allScoreboardLine.muteButton).gameObject.transform.position))
				{
					continue;
				}
				NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(activeRig);
				if (playerFromVRRig == null || playerFromVRRig == NetworkSystem.Instance.LocalPlayer || !afMuted.Add(playerFromVRRig.UserId))
				{
					continue;
				}
				Main.RPCProtection();
				foreach (GorillaPlayerScoreboardLine allScoreboardLine2 in GorillaScoreboardTotalUpdater.allScoreboardLines)
				{
					if (allScoreboardLine2.linePlayer != playerFromVRRig)
					{
						continue;
					}
					allScoreboardLine2.muteButton.isOn = true;
					allScoreboardLine2.PressButton(true, (ButtonType)3);
					break;
				}
				NotificationManager.SendNotification("<color=grey>[</color><color=red>AF</color><color=grey>]</color> Auto Mute: muted " + playerFromVRRig.NickName);
				break;
			}
		}
	}

	public static void PTEnable()
	{
		Main.PromptText("Enter Player ID to track:", delegate
		{
			ptTargetId = Main.keyboardInput?.Trim();
			if (string.IsNullOrEmpty(ptTargetId))
			{
				ptActive = false;
			}
			else
			{
				ptTimeout = Time.time + 5f;
				ptActive = true;
				PhotonNetwork.NetworkingClient.OpFindFriends(new string[1] { ptTargetId }, (FindFriendsOptions)null);
				NotificationManager.SendNotification("<color=grey>[</color><color=red>PT</color><color=grey>]</color> Searching for player...");
			}
		});
	}

	public static void PTDisable()
	{
		ptActive = false;
		ptTargetId = null;
	}

	public static void PT()
	{
		if (ptActive && !string.IsNullOrEmpty(ptTargetId) && Time.time > ptTimeout)
		{
			ptActive = false;
			NotificationManager.SendNotification("<color=grey>[</color><color=red>PT</color><color=grey>]</color> Player search timed out.");
		}
	}

	public static void HandleFindFriendsResponse(OperationResponse response)
	{
		if (!ptActive || response.OperationCode != 222)
		{
			return;
		}
		ptActive = false;
		try
		{
			string[] array = response[(byte)2] as string[];
			bool[] array2 = response[(byte)3] as bool[];
			string[] array3 = response[(byte)4] as string[];
			if (array == null || array2 == null)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>PT</color><color=grey>]</color> Could not parse server response.");
				return;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (!(array[i] != ptTargetId))
				{
					if (array2[i] && array3 != null && !string.IsNullOrEmpty(array3[i]))
					{
						NotificationManager.SendNotification("<color=grey>[</color><color=red>PT</color><color=grey>]</color> Player found in room: " + array3[i]);
					}
					else if (array2[i])
					{
						NotificationManager.SendNotification("<color=grey>[</color><color=red>PT</color><color=grey>]</color> Player is online but not in a room.");
					}
					else
					{
						NotificationManager.SendNotification("<color=grey>[</color><color=red>PT</color><color=grey>]</color> Player is offline or not found.");
					}
					return;
				}
			}
			NotificationManager.SendNotification("<color=grey>[</color><color=red>PT</color><color=grey>]</color> Player ID not found in server response.");
		}
		catch
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>PT</color><color=grey>]</color> Error parsing server response.");
		}
	}

	public static void AntiReportFRT(Player subject)
	{
		reportRig = subject.VRRig();
	}

	public static void AntiModerator()
	{
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig vrrig) => (!vrrig.isOfflineVRRig && vrrig.Cosmetics().Contains("LBAAK")) || vrrig.Cosmetics().Contains("LBAAD") || vrrig.Cosmetics().Contains("LMAPY")))
		{
			try
			{
				VRRig val = item;
				NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(val);
				if (playerFromVRRig != null)
				{
					string text = "Room: " + PhotonNetwork.CurrentRoom.Name;
					float num = 0f;
					float num2 = 0f;
					float num3 = 0f;
					try
					{
						num = val.playerColor.r * 255f;
						num2 = val.playerColor.r * 255f;
						num3 = val.playerColor.r * 255f;
					}
					catch
					{
						LogManager.Log("Failed to log colors, rig most likely nonexistent");
					}
					try
					{
						text += "\n====================================\n";
						text = text + "Player Name: \"" + playerFromVRRig.NickName + "\", Player ID: \"" + playerFromVRRig.UserId + "\", Player Color: (R: " + num + ", G: " + num2 + ", B: " + num3 + ")";
					}
					catch
					{
						LogManager.Log("Failed to log player");
					}
					text += "\n====================================\n";
					text += "Text file generated with MrChicken Menu";
					string path = "SeralythMenu/" + playerFromVRRig.NickName + " - Anti Moderator.txt";
					File.WriteAllText(path, text);
				}
			}
			catch
			{
			}
			NetworkSystem.Instance.ReturnToSinglePlayer();
			NotificationManager.SendNotification("<color=grey>[</color><color=purple>ANTI-MODERATOR</color><color=grey>]</color> " + item.GetName() + " is a moderator, you have been disconnected. Their player ID and room code have been saved to a file.");
		}
	}

	public static void AntiContentCreator()
	{
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isOfflineVRRig && Visuals.specialCosmetics.Keys.Any((string x) => vrrig.Cosmetics().Contains(x))))
		{
			try
			{
				VRRig val = item;
				NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(val);
				if (playerFromVRRig != null)
				{
					string text = "Room: " + PhotonNetwork.CurrentRoom.Name;
					float num = 0f;
					float num2 = 0f;
					float num3 = 0f;
					try
					{
						num = val.playerColor.r * 255f;
						num2 = val.playerColor.r * 255f;
						num3 = val.playerColor.r * 255f;
					}
					catch
					{
						LogManager.Log("Failed to log colors, rig most likely nonexistent");
					}
					try
					{
						text += "\n====================================\n";
						text = text + "Player Name: \"" + playerFromVRRig.NickName + "\", Player ID: \"" + playerFromVRRig.UserId + "\", Player Color: (R: " + num + ", G: " + num2 + ", B: " + num3 + ")";
					}
					catch
					{
						LogManager.Log("Failed to log player");
					}
					text += "\n====================================\n";
					text += "Text file generated with MrChicken Menu";
					string path = "SeralythMenu/" + playerFromVRRig.NickName + " - Anti Content Creator.txt";
					File.WriteAllText(path, text);
				}
			}
			catch
			{
			}
			NetworkSystem.Instance.ReturnToSinglePlayer();
			NotificationManager.SendNotification("<color=grey>[</color><color=purple>ANTI-CONTENT CREATOR</color><color=grey>]</color> " + item.GetName() + " is a content creator, you have been disconnected. Their player ID and room code have been saved to a file.");
		}
	}

	public static void CosmeticNotifications()
	{
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		VRRig val = null;
		string text = null;
		foreach (VRRig rig in VRRigCache.ActiveRigs.Where((VRRig rig2) => !rig2.IsLocal()))
		{
			using (IEnumerator<KeyValuePair<string, string>> enumerator2 = Visuals.specialCosmetics.Where((KeyValuePair<string, string> cosmetic) => rig.Cosmetics().Contains(cosmetic.Key)).GetEnumerator())
			{
				if (enumerator2.MoveNext())
				{
					KeyValuePair<string, string> current = enumerator2.Current;
					val = rig;
					text = current.Value;
				}
			}
			if ((Object)(object)val != (Object)null)
			{
				break;
			}
		}
		if ((Object)(object)val != (Object)null && !previousSpecial)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=#" + val.GetColor().ToHex() + ">COSMETIC</color><color=grey>]</color> " + val.GetName() + " has " + text + ".");
		}
		previousSpecial = (Object)(object)val != (Object)null;
	}

	public static void BypassAutomod()
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Invalid comparison between Unknown and I4
		GorillaTagger.moderationMutedTime = -1f;
		if (((GorillaComputer)GorillaComputer.instance).autoMuteType != "OFF")
		{
			((GorillaComputer)GorillaComputer.instance).autoMuteType = "OFF";
			PlayerPrefs.SetInt("autoMute", 0);
			PlayerPrefs.Save();
		}
		Recorder primaryRecorder = NetworkSystem.Instance.VoiceConnection.PrimaryRecorder;
		if ((Object)(object)primaryRecorder == (Object)null || (int)primaryRecorder.SourceType == 1)
		{
			return;
		}
		float num = 0f;
		GorillaSpeakerLoudness component = ((Component)VRRig.LocalRig).GetComponent<GorillaSpeakerLoudness>();
		if ((Object)(object)component != (Object)null)
		{
			num = component.Loudness;
		}
		if (num == 0f)
		{
			if (lastVol != 0f)
			{
				startSilenceTime = Time.time;
				reloaded = false;
			}
			if (startSilenceTime > 0f && !reloaded && Time.time - startSilenceTime >= 0.25f)
			{
				primaryRecorder.RestartRecording(true);
				reloaded = true;
			}
		}
		else
		{
			startSilenceTime = -1f;
			reloaded = false;
		}
		lastVol = num;
	}

	public static void BypassModCheckers()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		Player localPlayer = PhotonNetwork.LocalPlayer;
		if (localPlayer == null || localPlayer.CustomProperties == null || ((Dictionary<object, object>)(object)localPlayer.CustomProperties).Count == 0)
		{
			return;
		}
		Hashtable val = new Hashtable();
		foreach (string item in from keyObj in ((Dictionary<object, object>)(object)localPlayer.CustomProperties).Keys.ToList()
			select keyObj?.ToString() into key
			where key != null
			where !key.Equals("didTutorial")
			select key)
		{
			val[(object)item] = null;
		}
		if (((Dictionary<object, object>)(object)val).Count > 0)
		{
			localPlayer.SetCustomProperties(val, (Hashtable)null, (WebFlags)null);
		}
	}

	public static void AntiPredictions()
	{
		SerializePatch.OverrideSerialization = delegate
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
			Vector3 localPosition = VRRig.LocalRig.leftHand.rigTarget.localPosition;
			Vector3 localPosition2 = VRRig.LocalRig.rightHand.rigTarget.localPosition;
			smoothedLeftHandPosition = Vector3.Lerp(smoothedLeftHandPosition, localPosition, 0.75f);
			smoothedRightHandPosition = Vector3.Lerp(smoothedRightHandPosition, localPosition2, 0.75f);
			VRRig.LocalRig.leftHand.rigTarget.localPosition = smoothedLeftHandPosition;
			VRRig.LocalRig.rightHand.rigTarget.localPosition = smoothedRightHandPosition;
			Main.SendSerialize(VRRig.LocalRig.GetPhotonView());
			VRRig.LocalRig.leftHand.rigTarget.localPosition = localPosition;
			VRRig.LocalRig.rightHand.rigTarget.localPosition = localPosition2;
			return false;
		};
	}

	public static void ChangeIdentity()
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		string text = "gorilla";
		for (int i = 0; i < 4; i++)
		{
			text += Random.Range(0, 9);
		}
		Main.ChangeName(text);
		byte b = (byte)Random.Range(0, 255);
		byte b2 = (byte)Random.Range(0, 255);
		byte b3 = (byte)Random.Range(0, 255);
		Main.ChangeColor(Color32.op_Implicit(new Color32(b, b2, b3, byte.MaxValue)));
	}

	public static void ChangeIdentityRegular()
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		string text = ((Random.Range(0, 3) == 0) ? namePrefix[Random.Range(0, namePrefix.Length)] : "");
		string text2 = ((Random.Range(0, 3) == 0) ? nameSuffix[Random.Range(0, nameSuffix.Length)] : "");
		string text3 = text + names[Random.Range(0, names.Length)] + text2;
		Main.ChangeName((text3.Length > 12) ? text3.Substring(0, 12) : text3);
		Color[] array = (Color[])(object)new Color[16]
		{
			Color.cyan,
			Color.yellow,
			Color.blue,
			Color.gray,
			Color.black,
			Color.white,
			Color.magenta,
			Color.yellow,
			Color.green,
			new Color(1f, 0.5f, 1f, 255f),
			new Color(0f, 0.5f, 0f, 255f),
			Color32.op_Implicit(new Color32((byte)113, (byte)0, (byte)198, byte.MaxValue)),
			Color32.op_Implicit(new Color32((byte)170, (byte)198, (byte)170, byte.MaxValue)),
			Color32.op_Implicit(new Color32((byte)170, (byte)170, (byte)170, byte.MaxValue)),
			Color32.op_Implicit(new Color32((byte)227, (byte)170, (byte)85, byte.MaxValue)),
			Color32.op_Implicit(new Color32((byte)0, (byte)226, byte.MaxValue, byte.MaxValue))
		};
		Main.ChangeColor(array[Random.Range(0, array.Length)]);
	}

	public static void ChangeIdentityCustom()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		string[] array = new string[2] { "seralyth", "me" };
		Color[] array2 = (Color[])(object)new Color[2]
		{
			Color32.op_Implicit(new Color32(byte.MaxValue, (byte)128, (byte)0, byte.MaxValue)),
			Color.white
		};
		string path = "SeralythMenu/CustomIdentities.txt";
		if (File.Exists(path))
		{
			string[] array3 = File.ReadAllText(path).Split("\n");
			array = array3[0].Split(";");
			array2 = array3[1].Split(";").Select(Main.HexToColor).ToArray();
		}
		else
		{
			File.WriteAllText(path, "seralyth;me\n9b59b6;ffffff");
		}
		string text = array[Random.Range(0, array.Length)];
		Color color = array2[Random.Range(0, array2.Length)];
		Main.ChangeName((text.Length > 12) ? text.Substring(0, 12) : text);
		Main.ChangeColor(color);
	}

	public static void ChangeIdentityOnDisconnect(Action identityType)
	{
		if (!PhotonNetwork.InRoom && previouslyInLobby)
		{
			identityType?.Invoke();
		}
		previouslyInLobby = PhotonNetwork.InRoom;
	}

	public static void NameSpoof()
	{
		List<VRRig> list = new List<VRRig>();
		foreach (VRRig nameSpoofRig in nameSpoofRigs)
		{
			if (!VRRigCache.ActiveRigs.Contains(nameSpoofRig))
			{
				list.Add(nameSpoofRig);
			}
		}
		foreach (VRRig item in list)
		{
			nameSpoofRigs.Remove(item);
		}
		list.Clear();
		string nickName = PhotonNetwork.NickName;
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.isLocal && !nameSpoofRigs.Contains(activeRig))
			{
				string text = ((Random.Range(0, 3) == 0) ? namePrefix[Random.Range(0, namePrefix.Length)] : "");
				string text2 = ((Random.Range(0, 3) == 0) ? nameSuffix[Random.Range(0, nameSuffix.Length)] : "");
				string str = text + names[Random.Range(0, names.Length)] + text2;
				Main.ChangeName(str.EnforceLength(12), noColor: true);
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RigUtilities.GetPlayerFromVRRig(activeRig), new object[3]
				{
					Random.Range(0f, 1f),
					Random.Range(0f, 1f),
					Random.Range(0f, 1f)
				});
				nameSpoofRigs.Add(activeRig);
			}
		}
		if (PhotonNetwork.NickName != nickName)
		{
			PhotonNetwork.NickName = nickName;
		}
	}

	public static void ColorSpoof()
	{
		List<VRRig> list = new List<VRRig>();
		foreach (VRRig colorSpoofRig in colorSpoofRigs)
		{
			if (!VRRigCache.ActiveRigs.Contains(colorSpoofRig))
			{
				list.Add(colorSpoofRig);
			}
		}
		foreach (VRRig item in list)
		{
			colorSpoofRigs.Remove(item);
		}
		list.Clear();
		foreach (VRRig item2 in from rig in VRRigCache.ActiveRigs
			where !rig.isLocal
			where !colorSpoofRigs.Contains(rig)
			select rig)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RigUtilities.GetPlayerFromVRRig(item2), new object[3]
			{
				Random.Range(0f, 1f),
				Random.Range(0f, 1f),
				Random.Range(0f, 1f)
			});
			colorSpoofRigs.Add(item2);
		}
	}

	public static void FPSSpoof()
	{
		FPSPatch.enabled = true;
		FPSPatch.spoofFPSValue = Random.Range(fpsSpoofValue - 10, fpsSpoofValue + 10);
	}

	public static void PingSpoof()
	{
		if (SerializePatch.OverrideSerialization == null)
		{
			SerializePatch.OverrideSerialization = delegate
			{
				Main.MassSerialize(exclude: false, null, pingSpoofValue);
				return false;
			};
		}
	}

	public static void ChangeFPSSpoofValue(bool positive = true)
	{
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				fpsSpoofValue += 5;
			}
			else
			{
				fpsSpoofValue -= 5;
			}
		}
		if (fpsSpoofValue > 140)
		{
			fpsSpoofValue = 5;
		}
		if (fpsSpoofValue < 5)
		{
			fpsSpoofValue = 140;
		}
		Buttons.GetIndex("Change FPS Spoof Value").overlapText = "Change FPS Spoof Value <color=grey>[</color><color=green>" + fpsSpoofValue + "</color><color=grey>]</color>";
	}

	public static void ChangePingSpoofValue(bool positive = true)
	{
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				pingSpoofValue += 100;
			}
			else
			{
				pingSpoofValue -= 100;
			}
		}
		if (pingSpoofValue > 10000)
		{
			pingSpoofValue = 100;
		}
		if (pingSpoofValue < 100)
		{
			pingSpoofValue = 10000;
		}
		Buttons.GetIndex("Change Ping Spoof Value").overlapText = "Change Ping Spoof Value <color=grey>[</color><color=green>" + pingSpoofValue + "</color><color=grey>]</color>";
	}

	public static void ChangeRankedTier(bool positive = true)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				rankIndex++;
			}
			else
			{
				rankIndex--;
			}
		}
		rankIndex %= 3;
		if (rankIndex < 0)
		{
			rankIndex = 2;
		}
		targetRank = ((object)(ERankedMatchmakingTier)rankIndex/*cast due to .constrained prefix*/).ToString();
		Buttons.GetIndex("Change Ranked Tier").overlapText = "Change Matchmaking Tier <color=grey>[</color><color=green>" + targetRank + "</color><color=grey>]</color>";
	}

	public static void ChangeELOValue(bool positive = true)
	{
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				targetElo += 100;
			}
			else
			{
				targetElo -= 100;
			}
		}
		if (targetElo > 4000)
		{
			targetElo = 0;
		}
		if (targetElo < 0)
		{
			targetElo = 4000;
		}
		Buttons.GetIndex("Change ELO Value").overlapText = "Change ELO Value <color=grey>[</color><color=green>" + targetElo + "</color><color=grey>]</color>";
	}

	public static void ChangeBadgeTier(bool positive = true)
	{
		string[] array = new string[8] { "Wood", "Rock", "Bronze", "Silver", "Gold", "Platinum", "Crystal", "Banana" };
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				targetBadge++;
			}
			else
			{
				targetBadge--;
			}
		}
		targetBadge %= 8;
		if (targetBadge < 0)
		{
			targetBadge = 7;
		}
		Buttons.GetIndex("Change Badge Tier").overlapText = "Change Badge Tier <color=grey>[</color><color=green>" + array[targetBadge] + "</color><color=grey>]</color>";
	}

	public static void SpoofRank(bool enabled, string tier = null)
	{
		RankedPatch.enabled = enabled;
		RankedPatch.targetTier = tier;
	}

	public static void SpoofPlatform(bool enabled, string target = null)
	{
		RankedPatch.enabled = enabled;
		RankedPatch.targetPlatform = target;
	}

	public static void SpoofPlatform(bool enabled)
	{
		spoofingPlatform = enabled;
		GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateRankedInfo", (RpcTarget)1, new object[3]
		{
			0,
			enabled ? 1 : 0,
			(!enabled) ? 1 : 0
		});
	}

	public static void SpoofBadge()
	{
		SetRankedPatch.enabled = true;
		if (!Mathf.Approximately(VRRig.LocalRig.currentRankedELO, (float)targetElo) || VRRig.LocalRig.currentRankedSubTierQuest != targetBadge || VRRig.LocalRig.currentRankedSubTierPC != targetBadge)
		{
			VRRig.LocalRig.SetRankedInfo((float)targetElo, targetBadge, targetBadge, true);
		}
	}

	public static void EnableAutoReportSystem()
	{
		if (arsCoroutine != null)
		{
			((MonoBehaviour)CoroutineManager.instance).StopCoroutine(arsCoroutine);
		}
		arsCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(AutoReportSystemCoroutine());
	}

	public static void DisableAutoReportSystem()
	{
		if (arsCoroutine != null)
		{
			((MonoBehaviour)CoroutineManager.instance).StopCoroutine(arsCoroutine);
		}
		arsCoroutine = null;
		arsPlayersToReport = null;
	}

	private static IEnumerator AutoReportSystemCoroutine()
	{
		UnityWebRequest www = UnityWebRequest.Get("https://raw.githubusercontent.com/AutoReportSystem/ARSPlayerIDs/refs/heads/main/Player%20Ids.txt");
		try
		{
			yield return www.SendWebRequest();
			if ((int)www.result == 1)
			{
				arsPlayersToReport = (from id in www.downloadHandler.text.Split(",")
					select id.Trim() into id
					where !string.IsNullOrEmpty(id)
					select id).ToArray();
				while (true)
				{
					if (arsPlayersToReport != null)
					{
						foreach (VRRig rig in VRRigCache.ActiveRigs)
						{
							if (rig.isLocal || rig.creator == null || !arsPlayersToReport.Contains(rig.creator.UserId))
							{
								continue;
							}
							NotificationManager.SendNotification("<color=green>ARS</color> Player " + rig.creator.SanitizedNickName + " is on the ARS list, reporting...");
							foreach (GorillaPlayerScoreboardLine line in GorillaScoreboardTotalUpdater.allScoreboardLines)
							{
								if ((Object)(object)line.playerVRRig == (Object)(object)rig)
								{
									line.reportedToxicity = true;
									line.PressButton(true, (ButtonType)2);
								}
							}
						}
					}
					yield return (object)new WaitForSeconds(1f);
				}
			}
			NotificationManager.SendNotification("<color=green>ARS</color> Failed to fetch player list.");
		}
		finally
		{
			((IDisposable)www)?.Dispose();
		}
	}

	public static void CopyRoomCode()
	{
		if (!PhotonNetwork.InRoom)
		{
			NotificationManager.SendNotification("You are not currently in a room!");
			return;
		}
		string text = (GUIUtility.systemCopyBuffer = PhotonNetwork.CurrentRoom.Name);
		NotificationManager.SendNotification("<color=grey>[</color><color=green>COPY ROOM CODE</color><color=grey>]</color> <color=white>Room Code: " + text + " copied to clipboard!</color>");
	}
}
