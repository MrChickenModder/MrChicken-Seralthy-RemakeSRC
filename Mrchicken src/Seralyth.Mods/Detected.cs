using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using GorillaGameModes;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.CloudScriptModels;
using Seralyth.Extensions;
using Seralyth.Managers;
using Seralyth.Menu;
using Seralyth.Patches.Menu;
using Seralyth.Utilities;
using TMPro;
using UnityEngine;

namespace Seralyth.Mods;

public static class Detected
{
	public static float masterDelay;

	public static Dictionary<VRRig, int> viewIdArchive = new Dictionary<VRRig, int>();

	private static readonly Dictionary<GorillaPlayerScoreboardLine, VRRig> linerig = new Dictionary<GorillaPlayerScoreboardLine, VRRig>();

	public static float muteDelay;

	private static Coroutine disablePatchCoroutine;

	public static string name = "SERALYTH";

	private static float customPropertyDelay;

	private static float spazGamemodeDelay;

	public static bool moddedGamemode;

	public static void ObliterateGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		//IL_00c6: Expected O, but got Unknown
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
		if (!Object.op_Implicit((Object)(object)componentInParent) || componentInParent.IsLocal())
		{
			return;
		}
		ValueIterator<int, PhotonView> enumerator = PhotonNetwork.PhotonViewCollection.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				PhotonView current = enumerator.Current;
				Hashtable val2 = new Hashtable();
				val2.Add((byte)0, (object)current.ViewID);
				RaiseEventOptions val3 = new RaiseEventOptions();
				val3.TargetActors = new int[1] { componentInParent.Creator.ActorNumber };
				Destroy(componentInParent, val2, val3);
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
	}

	public static void ObliterateAll()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_007a: Expected O, but got Unknown
		foreach (VRRig activeRig in VRRigExtensions.ActiveRigs)
		{
			ValueIterator<int, PhotonView> enumerator2 = PhotonNetwork.PhotonViewCollection.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					PhotonView current2 = enumerator2.Current;
					Hashtable val = new Hashtable();
					val.Add((byte)0, (object)current2.ViewID);
					RaiseEventOptions val2 = new RaiseEventOptions();
					val2.TargetActors = new int[1] { activeRig.Creator.ActorNumber };
					Destroy(activeRig, val, val2);
				}
			}
			finally
			{
				((IDisposable)enumerator2/*cast due to .constrained prefix*/).Dispose();
			}
		}
	}

	public static void BanSelf()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		for (int i = 0; i > 2; i++)
		{
			((GorillaServer)GorillaServer.Instance).CheckForBadName(new CheckForBadNameRequest
			{
				name = ((GorillaComputer)GorillaComputer.instance).anywhereTwoWeek[Random.Range(0, ((GorillaComputer)GorillaComputer.instance).anywhereTwoWeek.Length)],
				forRoom = true,
				forTroop = false
			}, (Action<ExecuteFunctionResult>)null, (Action<PlayFabError>)null);
		}
	}

	public static void EnterDetectedTab()
	{
		if (!Main.allowDetected)
		{
			AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Menu/danger.ogg", "Audio/Menu/danger.ogg", delegate(AudioClip clip)
			{
				Main.Play2DAudio(clip, (float)Main.buttonClickVolume / 10f);
			});
			Main.Prompt("The mods in this category are detected. <b>Unless you know what you're doing, you will get banned.</b> Are you sure you would like to continue?", delegate
			{
				Main.allowDetected = true;
				Buttons.CurrentCategoryName = "Detected Mods";
				AchievementManager.UnlockAchievement(new AchievementManager.Achievement
				{
					name = "Sinister",
					description = "Open the \"Detected Mods\" category.",
					icon = "Images/Achievements/sinister.png"
				});
			});
		}
		else
		{
			Buttons.CurrentCategoryName = "Detected Mods";
		}
	}

	public static void SetMasterClientGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true))
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal() && Time.time > masterDelay)
			{
				PhotonNetwork.SetMasterClient(componentInParent.GetPhotonPlayer());
				masterDelay = Time.time + 0.02f;
			}
		}
	}

	public static void SetMasterClientAll()
	{
		if (Time.time > masterDelay)
		{
			PhotonNetwork.SetMasterClient(RigUtilities.GetTargetPlayer().GetPhotonPlayer());
			masterDelay = Time.time + 0.02f;
		}
	}

	public static void SetMasterClientAura()
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
		foreach (VRRig item in list.Where((VRRig nearbyPlayer) => Time.time > masterDelay))
		{
			PhotonNetwork.SetMasterClient(item.GetPhotonPlayer());
			masterDelay = Time.time + 0.02f;
		}
	}

	public static void SetMasterClientOnTouch()
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
		foreach (VRRig item in list.Where((VRRig rig) => Time.time > masterDelay))
		{
			PhotonNetwork.SetMasterClient(item.GetPhotonPlayer());
			masterDelay = Time.time + 0.02f;
		}
	}

	public static void AutoSetMasterClient()
	{
		if (PhotonNetwork.InRoom && !PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
		}
	}

	public static void Destroy(object target, Hashtable hashtable = null, RaiseEventOptions raiseEventOptions = null, int viewID = -1)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected O, but got Unknown
		//IL_0063: Expected O, but got Unknown
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		VRRig val = (VRRig)((target is VRRig) ? target : null);
		if (val == null)
		{
			Player val2 = (Player)((target is Player) ? target : null);
			if (val2 == null)
			{
				if (target is GameObject)
				{
				}
				return;
			}
			if (hashtable == null)
			{
				Hashtable val3 = new Hashtable();
				val3.Add((byte)0, (object)val2.ActorNumber);
				hashtable = val3;
			}
			if (raiseEventOptions == null)
			{
				RaiseEventOptions val4 = new RaiseEventOptions();
				val4.TargetActors = new int[1] { val2.ActorNumber };
				raiseEventOptions = val4;
			}
			PhotonNetwork.NetworkingClient.OpRaiseEvent((byte)207, (object)hashtable, raiseEventOptions, SendOptions.SendReliable);
		}
		else
		{
			if (hashtable == null)
			{
				PhotonView photonViewFromVRRig = RigUtilities.GetPhotonViewFromVRRig(val);
				Hashtable val5 = new Hashtable();
				val5.Add((byte)0, (object)((viewID == -1) ? photonViewFromVRRig.ViewID : viewID));
				hashtable = val5;
			}
			if (raiseEventOptions == null)
			{
				RaiseEventOptions val4 = new RaiseEventOptions();
				val4.TargetActors = new int[1] { val.GetPlayer().ActorNumber };
				raiseEventOptions = val4;
			}
			PhotonNetwork.NetworkingClient.OpRaiseEvent((byte)204, (object)hashtable, raiseEventOptions, SendOptions.SendReliable);
		}
	}

	public static void CrashGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				PhotonNetwork.SetMasterClient(Main.lockTarget.GetPhotonPlayer());
				PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
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

	public static void CrashAll()
	{
		PhotonNetwork.SetMasterClient(RigUtilities.GetTargetPlayer().GetPhotonPlayer());
		PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
	}

	public static void CrashAura()
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
			PhotonNetwork.SetMasterClient(item.GetPhotonPlayer());
			PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
		}
	}

	public static void CrashOnTouch()
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
			PhotonNetwork.SetMasterClient(item.GetPhotonPlayer());
			PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
		}
	}

	public static void CrashWhenTouched()
	{
		foreach (NetPlayer item in from vrrig in VRRigCache.ActiveRigs
			where !vrrig.isMyPlayer && !vrrig.isOfflineVRRig && ((double)Vector3.Distance(vrrig.rightHandTransform.position, ((Component)VRRig.LocalRig).transform.position) <= 0.5 || (double)Vector3.Distance(vrrig.leftHandTransform.position, ((Component)VRRig.LocalRig).transform.position) <= 0.5 || (double)Vector3.Distance(((Component)vrrig).transform.position, ((Component)VRRig.LocalRig).transform.position) <= 0.5)
			select RigUtilities.GetPlayerFromVRRig(vrrig))
		{
			PhotonNetwork.SetMasterClient(item.GetPlayer());
			PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer);
		}
	}

	public static void GhostGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_0119: Expected O, but got Unknown
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
		if (!Object.op_Implicit((Object)(object)componentInParent) || componentInParent.IsLocal())
		{
			return;
		}
		PhotonView view = RigUtilities.GetPhotonViewFromVRRig(componentInParent);
		if ((Object)(object)view != (Object)null)
		{
			viewIdArchive[componentInParent] = view.ViewID;
			Hashtable val2 = new Hashtable();
			val2.Add((byte)0, (object)view.ViewID);
			Destroy(componentInParent, val2, new RaiseEventOptions
			{
				TargetActors = (from p in PhotonNetwork.PlayerList
					where p != view.Owner
					select p.ActorNumber).ToArray()
			});
		}
	}

	public static void GhostAll()
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_00dd: Expected O, but got Unknown
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			try
			{
				if (activeRig.IsLocal())
				{
					continue;
				}
				PhotonView view = RigUtilities.GetPhotonViewFromVRRig(activeRig);
				if ((Object)(object)view != (Object)null)
				{
					viewIdArchive[activeRig] = view.ViewID;
					int[] targetActors = (from p in PhotonNetwork.PlayerList
						where p != view.Owner
						select p.ActorNumber).ToArray();
					Hashtable val = new Hashtable();
					val.Add((byte)0, (object)view.ViewID);
					Destroy(activeRig, val, new RaiseEventOptions
					{
						TargetActors = targetActors
					});
				}
			}
			catch
			{
			}
		}
	}

	public static void GhostAura()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Expected O, but got Unknown
		//IL_0185: Expected O, but got Unknown
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
		foreach (VRRig item in list.ToList())
		{
			PhotonView view = RigUtilities.GetPhotonViewFromVRRig(item);
			if ((Object)(object)view != (Object)null)
			{
				viewIdArchive[item] = view.ViewID;
				int[] targetActors = (from p in PhotonNetwork.PlayerList
					where p != view.Owner
					select p.ActorNumber).ToArray();
				Hashtable val = new Hashtable();
				val.Add((byte)0, (object)view.ViewID);
				Destroy(item, val, new RaiseEventOptions
				{
					TargetActors = targetActors
				});
				list.Remove(item);
			}
		}
	}

	public static void GhostOnTouch()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Expected O, but got Unknown
		//IL_0186: Expected O, but got Unknown
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
		foreach (VRRig item in list)
		{
			PhotonView view = RigUtilities.GetPhotonViewFromVRRig(item);
			if ((Object)(object)view != (Object)null)
			{
				viewIdArchive[item] = view.ViewID;
				Hashtable val = new Hashtable();
				val.Add((byte)0, (object)view.ViewID);
				Destroy(item, val, new RaiseEventOptions
				{
					TargetActors = (from p in PhotonNetwork.PlayerList
						where p != view.Owner
						select p.ActorNumber).ToArray()
				});
			}
		}
	}

	public static void LeaderboardGhost()
	{
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Expected O, but got Unknown
		//IL_01cb: Expected O, but got Unknown
		foreach (GorillaScoreBoard item in GorillaScoreboardTotalUpdater.allScoreboards.Where((GorillaScoreBoard scoreboard) => ((TMP_Text)scoreboard.buttonText).text.Contains("REPORT")))
		{
			((TMP_Text)item.buttonText).text = ((TMP_Text)item.buttonText).text.Replace("REPORT", "GHOST");
		}
		foreach (GorillaPlayerScoreboardLine item2 in GorillaScoreboardTotalUpdater.allScoreboardLines.Where((GorillaPlayerScoreboardLine line) => line.linePlayer != NetworkSystem.Instance.LocalPlayer))
		{
			if (item2.reportInProgress)
			{
				item2.SetReportState(false, (ButtonType)5);
				item2.reportButton.isOn = true;
				item2.reportButton.UpdateColor();
				PhotonView view = RigUtilities.GetPhotonViewFromVRRig(item2.linePlayer.VRRig());
				if ((Object)(object)view != (Object)null)
				{
					viewIdArchive[item2.linePlayer.VRRig()] = view.ViewID;
					linerig.Add(item2, item2.linePlayer.VRRig());
					VRRig target = item2.linePlayer.VRRig();
					Hashtable val = new Hashtable();
					val.Add((byte)0, (object)view.ViewID);
					Destroy(target, val, new RaiseEventOptions
					{
						TargetActors = (from p in PhotonNetwork.PlayerList
							where p != view.Owner
							select p.ActorNumber).ToArray()
					});
				}
			}
			if (item2.reportButton.isOn && item2.reportInProgress)
			{
				item2.SetReportState(false, (ButtonType)5);
				item2.reportButton.isOn = false;
				item2.reportButton.UpdateColor();
				int viewID = viewIdArchive[item2.linePlayer.VRRig()];
				Destroy(item2.linePlayer.VRRig(), null, null, viewID);
			}
		}
	}

	public static void DisableLeaderboardGhost()
	{
		foreach (GorillaScoreBoard item in GorillaScoreboardTotalUpdater.allScoreboards.Where((GorillaScoreBoard scoreboard) => ((TMP_Text)scoreboard.buttonText).text.Contains("GHOST")))
		{
			((TMP_Text)item.buttonText).text = ((TMP_Text)item.buttonText).text.Replace("GHOST", "REPORT");
		}
		foreach (GorillaPlayerScoreboardLine allScoreboardLine in GorillaScoreboardTotalUpdater.allScoreboardLines)
		{
			allScoreboardLine.SetReportState(false, (ButtonType)5);
			allScoreboardLine.reportButton.isOn = false;
			allScoreboardLine.reportButton.UpdateColor();
		}
	}

	public static void LeaderboardMute()
	{
		if (!(Time.time > muteDelay))
		{
			return;
		}
		muteDelay = Time.time + 0.15f;
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig rig) => !rig.IsLocal() && rig.muted))
		{
			try
			{
				Destroy(item);
			}
			catch
			{
			}
		}
	}

	public static void UnghostGun()
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
				int viewID = viewIdArchive[componentInParent];
				Destroy(componentInParent, null, null, viewID);
			}
		}
	}

	public static void UnghostAll()
	{
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (viewIdArchive.TryGetValue(activeRig, out var value))
			{
				Destroy(activeRig, null, null, value);
			}
		}
	}

	public static void UnghostAura()
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
			int viewID = viewIdArchive[item];
			Destroy(item, null, null, viewID);
		}
	}

	public static void UnghostOnTouch()
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
		foreach (VRRig item in list)
		{
			int viewID = viewIdArchive[item];
			Destroy(item, null, null, viewID);
		}
	}

	public static void IsolateGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		//IL_0105: Expected O, but got Unknown
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
		if (!Object.op_Implicit((Object)(object)componentInParent) || componentInParent.IsLocal())
		{
			return;
		}
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			bool flag = !Buttons.GetIndex("Isolate Others").enabled || !activeRig.IsLocal();
			PhotonView photonViewFromVRRig = RigUtilities.GetPhotonViewFromVRRig(activeRig);
			if (flag && (Object)(object)activeRig != (Object)(object)componentInParent)
			{
				Hashtable val2 = new Hashtable();
				val2.Add((byte)0, (object)photonViewFromVRRig.ViewID);
				RaiseEventOptions val3 = new RaiseEventOptions();
				val3.TargetActors = new int[1] { componentInParent.GetPlayer().ActorNumber };
				Destroy(activeRig, val2, val3);
			}
		}
	}

	public static void IsolateAll()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected O, but got Unknown
		//IL_00c5: Expected O, but got Unknown
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!Buttons.GetIndex("Isolate Others").enabled || !activeRig.IsLocal())
			{
				PhotonView view = RigUtilities.GetPhotonViewFromVRRig(activeRig);
				Hashtable val = new Hashtable();
				val.Add((byte)0, (object)view.ViewID);
				Destroy(activeRig, val, new RaiseEventOptions
				{
					TargetActors = (from plr in PhotonNetwork.PlayerList
						where plr.ActorNumber != view.Owner.ActorNumber
						select plr.ActorNumber).ToArray()
				});
			}
		}
	}

	public static void IsolateAura()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Expected O, but got Unknown
		//IL_013e: Expected O, but got Unknown
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
			if (!Buttons.GetIndex("Isolate Others").enabled || !item.IsLocal())
			{
				PhotonView photonViewFromVRRig = RigUtilities.GetPhotonViewFromVRRig(item);
				Hashtable val = new Hashtable();
				val.Add((byte)0, (object)photonViewFromVRRig.ViewID);
				RaiseEventOptions val2 = new RaiseEventOptions();
				val2.TargetActors = new int[1] { photonViewFromVRRig.Owner.ActorNumber };
				Destroy(item, val, val2);
			}
		}
	}

	public static void IsolateOnTouch()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Expected O, but got Unknown
		//IL_0174: Expected O, but got Unknown
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
		foreach (VRRig item in list)
		{
			foreach (VRRig activeRig2 in VRRigCache.ActiveRigs)
			{
				bool flag = !Buttons.GetIndex("Isolate Others").enabled || !activeRig2.IsLocal();
				PhotonView photonViewFromVRRig = RigUtilities.GetPhotonViewFromVRRig(activeRig2);
				if (flag && (Object)(object)activeRig2 != (Object)(object)item)
				{
					Hashtable val = new Hashtable();
					val.Add((byte)0, (object)photonViewFromVRRig.ViewID);
					RaiseEventOptions val2 = new RaiseEventOptions();
					val2.TargetActors = new int[1] { item.GetPlayer().ActorNumber };
					Destroy(activeRig2, val, val2);
				}
			}
		}
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
				Destroy(Main.lockTarget.GetPhotonPlayer());
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
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig rig) => !rig.IsLocal()))
		{
			Destroy(item.GetPhotonPlayer());
		}
	}

	public static void LagAura()
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
			Destroy(item.GetPhotonPlayer());
		}
	}

	public static void LagOnTouch()
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
			Destroy(item.GetPhotonPlayer());
		}
	}

	public static void MuteGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && Time.time > muteDelay)
			{
				Destroy(Main.lockTarget.GetPhotonPlayer());
				muteDelay = Time.time + 0.15f;
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

	public static void MuteAll()
	{
		if (!(Time.time > muteDelay))
		{
			return;
		}
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig rig) => !rig.IsLocal()))
		{
			Destroy(item.GetPhotonPlayer());
		}
		muteDelay = Time.time + 0.15f;
	}

	public static void MuteAura()
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
		if (list.Count <= 0 || !(Time.time > muteDelay))
		{
			return;
		}
		foreach (VRRig item in list)
		{
			Destroy(item.GetPhotonPlayer());
			muteDelay = Time.time + 0.15f;
		}
	}

	public static void MuteOnTouch()
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
		if (list.Count <= 0 || !(Time.time > muteDelay))
		{
			return;
		}
		foreach (VRRig item in list)
		{
			Destroy(item.GetPhotonPlayer());
		}
		muteDelay = Time.time + 0.15f;
	}

	public static IEnumerator DisablePatch()
	{
		while (PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient)
		{
			yield return null;
		}
		GameModePatch.enabled = false;
	}

	public static void PromptNameChange()
	{
		Main.Prompt("Would you like to set a name?", delegate
		{
			Main.PromptSingleText("Please enter the name you'd like to use:", delegate
			{
				name = Main.keyboardInput;
			}, "Done");
		});
	}

	public static void ChangeNameGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Hashtable val2 = new Hashtable { [byte.MaxValue] = name };
				PhotonNetwork.CurrentRoom.LoadBalancingClient.OpSetPropertiesOfActor(Main.lockTarget.GetPlayer().ActorNumber, val2, (Hashtable)null, (WebFlags)null);
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

	public static void ChangeNameAll()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		Player[] playerListOthers = PhotonNetwork.PlayerListOthers;
		foreach (Player val in playerListOthers)
		{
			Hashtable val2 = new Hashtable { [byte.MaxValue] = name };
			PhotonNetwork.CurrentRoom.LoadBalancingClient.OpSetPropertiesOfActor(val.ActorNumber, val2, (Hashtable)null, (WebFlags)null);
		}
	}

	public static void ChangeNameAura()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
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
			Hashtable val = new Hashtable { [byte.MaxValue] = name };
			PhotonNetwork.CurrentRoom.LoadBalancingClient.OpSetPropertiesOfActor(item.GetPlayer().ActorNumber, val, (Hashtable)null, (WebFlags)null);
		}
	}

	public static void ChangeNameOnTouch()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Expected O, but got Unknown
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
			Hashtable val = new Hashtable { [byte.MaxValue] = name };
			PhotonNetwork.CurrentRoom.LoadBalancingClient.OpSetPropertiesOfActor(item.GetPlayer().ActorNumber, val, (Hashtable)null, (WebFlags)null);
		}
	}

	public static void BanGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Hashtable val2 = new Hashtable { [byte.MaxValue] = ((GorillaComputer)GorillaComputer.instance).anywhereTwoWeek[Random.Range(0, ((GorillaComputer)GorillaComputer.instance).anywhereTwoWeek.Length)] };
				PhotonNetwork.CurrentRoom.LoadBalancingClient.OpSetPropertiesOfActor(Main.lockTarget.GetPlayer().ActorNumber, val2, (Hashtable)null, (WebFlags)null);
				((MonkeAgent)MonkeAgent.instance).SendReport("evading the name ban", Main.lockTarget.GetPlayer().UserId, Main.lockTarget.GetPlayer().NickName);
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

	public static void BanAll()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		Player[] playerListOthers = PhotonNetwork.PlayerListOthers;
		foreach (Player val in playerListOthers)
		{
			Hashtable val2 = new Hashtable { [byte.MaxValue] = ((GorillaComputer)GorillaComputer.instance).anywhereTwoWeek[Random.Range(0, ((GorillaComputer)GorillaComputer.instance).anywhereTwoWeek.Length)] };
			PhotonNetwork.CurrentRoom.LoadBalancingClient.OpSetPropertiesOfActor(val.ActorNumber, val2, (Hashtable)null, (WebFlags)null);
			((MonkeAgent)MonkeAgent.instance).SendReport("evading the name ban", val.UserId, val.NickName);
		}
	}

	public static void BanAura()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Expected O, but got Unknown
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
			Hashtable val = new Hashtable { [byte.MaxValue] = ((GorillaComputer)GorillaComputer.instance).anywhereTwoWeek[Random.Range(0, ((GorillaComputer)GorillaComputer.instance).anywhereTwoWeek.Length)] };
			PhotonNetwork.CurrentRoom.LoadBalancingClient.OpSetPropertiesOfActor(item.GetPlayer().ActorNumber, val, (Hashtable)null, (WebFlags)null);
			((MonkeAgent)MonkeAgent.instance).SendReport("evading the name ban", item.GetPlayer().UserId, item.GetPlayer().NickName);
		}
	}

	public static void BanOnTouch()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
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
			Hashtable val = new Hashtable { [byte.MaxValue] = ((GorillaComputer)GorillaComputer.instance).anywhereTwoWeek[Random.Range(0, ((GorillaComputer)GorillaComputer.instance).anywhereTwoWeek.Length)] };
			PhotonNetwork.CurrentRoom.LoadBalancingClient.OpSetPropertiesOfActor(item.GetPlayer().ActorNumber, val, (Hashtable)null, (WebFlags)null);
			((MonkeAgent)MonkeAgent.instance).SendReport("evading the name ban", item.GetPlayer().UserId, item.GetPlayer().NickName);
		}
	}

	public static void BypassModCheckersGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
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
		if (!Object.op_Implicit((Object)(object)componentInParent) || componentInParent.IsLocal() || !(Time.time > customPropertyDelay))
		{
			return;
		}
		customPropertyDelay = Time.time + 0.25f;
		Player photonPlayer = componentInParent.GetPhotonPlayer();
		if (photonPlayer == null || photonPlayer.CustomProperties == null || ((Dictionary<object, object>)(object)photonPlayer.CustomProperties).Count == 0)
		{
			return;
		}
		Hashtable val2 = new Hashtable();
		foreach (string item in from keyObj in ((Dictionary<object, object>)(object)photonPlayer.CustomProperties).Keys.ToList()
			select keyObj?.ToString() into key
			where key != null
			where !key.Equals("didTutorial")
			select key)
		{
			val2[(object)item] = null;
		}
		if (((Dictionary<object, object>)(object)val2).Count > 0)
		{
			photonPlayer.SetCustomProperties(val2, (Hashtable)null, (WebFlags)null);
		}
	}

	public static void BypassModCheckersAll()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		Player[] playerList = PhotonNetwork.PlayerList;
		foreach (Player val in playerList)
		{
			if (val == null)
			{
				continue;
			}
			if (val.CustomProperties == null || ((Dictionary<object, object>)(object)val.CustomProperties).Count == 0)
			{
				break;
			}
			Hashtable val2 = new Hashtable();
			foreach (string item in from keyObj in ((Dictionary<object, object>)(object)val.CustomProperties).Keys.ToList()
				select keyObj?.ToString() into key
				where key != null
				where !key.Equals("didTutorial")
				select key)
			{
				val2[(object)item] = null;
			}
			if (((Dictionary<object, object>)(object)val2).Count > 0)
			{
				val.SetCustomProperties(val2, (Hashtable)null, (WebFlags)null);
			}
		}
	}

	public static void BypassModCheckersAura()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Expected O, but got Unknown
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
		foreach (Player item in from nearbyPlayer in list
			select nearbyPlayer.GetPhotonPlayer() into player
			where player != null
			select player)
		{
			if (item.CustomProperties == null || ((Dictionary<object, object>)(object)item.CustomProperties).Count == 0)
			{
				break;
			}
			Hashtable val = new Hashtable();
			foreach (string item2 in from keyObj in ((Dictionary<object, object>)(object)item.CustomProperties).Keys.ToList()
				select keyObj?.ToString() into key
				where key != null
				where !key.Equals("didTutorial")
				select key)
			{
				val[(object)item2] = null;
			}
			if (((Dictionary<object, object>)(object)val).Count > 0)
			{
				item.SetCustomProperties(val, (Hashtable)null, (WebFlags)null);
			}
		}
	}

	public static void BypassModCheckersOnTouch()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Expected O, but got Unknown
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
		foreach (Player item in from rig in list
			select rig.GetPhotonPlayer() into player
			where player != null
			select player)
		{
			if (item.CustomProperties == null || ((Dictionary<object, object>)(object)item.CustomProperties).Count == 0)
			{
				break;
			}
			Hashtable val = new Hashtable();
			foreach (string item2 in from keyObj in ((Dictionary<object, object>)(object)item.CustomProperties).Keys.ToList()
				select keyObj?.ToString() into key
				where key != null
				where !key.Equals("didTutorial")
				select key)
			{
				val[(object)item2] = null;
			}
			if (((Dictionary<object, object>)(object)val).Count > 0)
			{
				item.SetCustomProperties(val, (Hashtable)null, (WebFlags)null);
			}
		}
	}

	public static void BreakModCheckersGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
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
		if (!Object.op_Implicit((Object)(object)componentInParent) || componentInParent.IsLocal() || !(Time.time > customPropertyDelay))
		{
			return;
		}
		customPropertyDelay = Time.time + 0.25f;
		Hashtable val2 = new Hashtable();
		foreach (string key in Visuals.modDictionary.Keys)
		{
			val2[(object)key] = true;
		}
		componentInParent.GetPhotonPlayer().SetCustomProperties(val2, (Hashtable)null, (WebFlags)null);
	}

	public static void BreakModCheckersAll()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		Hashtable val = new Hashtable();
		foreach (string key in Visuals.modDictionary.Keys)
		{
			val[(object)key] = true;
		}
		Player[] playerList = PhotonNetwork.PlayerList;
		foreach (Player val2 in playerList)
		{
			val2.SetCustomProperties(val, (Hashtable)null, (WebFlags)null);
		}
	}

	public static void BreakModCheckersAura()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
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
			Hashtable val = new Hashtable();
			foreach (string key in Visuals.modDictionary.Keys)
			{
				val[(object)key] = true;
			}
			item.GetPhotonPlayer().SetCustomProperties(val, (Hashtable)null, (WebFlags)null);
		}
	}

	public static void BreakModCheckersOnTouch()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Expected O, but got Unknown
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
			Hashtable val = new Hashtable();
			foreach (string key in Visuals.modDictionary.Keys)
			{
				val[(object)key] = true;
			}
			item.GetPhotonPlayer().SetCustomProperties(val, (Hashtable)null, (WebFlags)null);
		}
	}

	public static void GamemodeIncludeGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		//IL_0091: Expected O, but got Unknown
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true))
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal() && Time.time > customPropertyDelay)
			{
				customPropertyDelay = Time.time + 0.25f;
				Hashtable val2 = new Hashtable();
				((Dictionary<object, object>)val2).Add((object)"didTutorial", (object)true);
				Hashtable val3 = val2;
				componentInParent.GetPhotonPlayer().SetCustomProperties(val3, (Hashtable)null, (WebFlags)null);
			}
		}
	}

	public static void GamemodeIncludeAll()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0029: Expected O, but got Unknown
		Player[] playerList = PhotonNetwork.PlayerList;
		foreach (Player val in playerList)
		{
			Hashtable val2 = new Hashtable();
			((Dictionary<object, object>)val2).Add((object)"didTutorial", (object)true);
			Hashtable val3 = val2;
			val.SetCustomProperties(val3, (Hashtable)null, (WebFlags)null);
		}
	}

	public static void GamemodeIncludeAura()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_00d9: Expected O, but got Unknown
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
			Hashtable val = new Hashtable();
			((Dictionary<object, object>)val).Add((object)"didTutorial", (object)true);
			Hashtable val2 = val;
			item.GetPhotonPlayer().SetCustomProperties(val2, (Hashtable)null, (WebFlags)null);
		}
	}

	public static void GamemodeIncludeOnTouch()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		//IL_00f8: Expected O, but got Unknown
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
			Hashtable val = new Hashtable();
			((Dictionary<object, object>)val).Add((object)"didTutorial", (object)true);
			Hashtable val2 = val;
			item.GetPhotonPlayer().SetCustomProperties(val2, (Hashtable)null, (WebFlags)null);
		}
	}

	public static void GamemodeExcludeGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		//IL_0091: Expected O, but got Unknown
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true))
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal() && Time.time > customPropertyDelay)
			{
				customPropertyDelay = Time.time + 0.25f;
				Hashtable val2 = new Hashtable();
				((Dictionary<object, object>)val2).Add((object)"didTutorial", (object)false);
				Hashtable val3 = val2;
				componentInParent.GetPhotonPlayer().SetCustomProperties(val3, (Hashtable)null, (WebFlags)null);
			}
		}
	}

	public static void GamemodeExcludeAll()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0029: Expected O, but got Unknown
		Player[] playerList = PhotonNetwork.PlayerList;
		foreach (Player val in playerList)
		{
			Hashtable val2 = new Hashtable();
			((Dictionary<object, object>)val2).Add((object)"didTutorial", (object)false);
			Hashtable val3 = val2;
			val.SetCustomProperties(val3, (Hashtable)null, (WebFlags)null);
		}
	}

	public static void GamemodeExcludeAura()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_00d9: Expected O, but got Unknown
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
			Hashtable val = new Hashtable();
			((Dictionary<object, object>)val).Add((object)"didTutorial", (object)false);
			Hashtable val2 = val;
			item.GetPhotonPlayer().SetCustomProperties(val2, (Hashtable)null, (WebFlags)null);
		}
	}

	public static void GamemodeExcludeOnTouch()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		//IL_00f8: Expected O, but got Unknown
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
			Hashtable val = new Hashtable();
			((Dictionary<object, object>)val).Add((object)"didTutorial", (object)false);
			Hashtable val2 = val;
			item.GetPhotonPlayer().SetCustomProperties(val2, (Hashtable)null, (WebFlags)null);
		}
	}

	public static void BreakGamemode(bool breaking)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_001c: Expected O, but got Unknown
		Hashtable val = new Hashtable();
		((Dictionary<object, object>)val).Add((object)"didTutorial", (object)(!breaking));
		Hashtable val2 = val;
		Player[] playerList = PhotonNetwork.PlayerList;
		foreach (Player val3 in playerList)
		{
			val3.SetCustomProperties(val2, (Hashtable)null, (WebFlags)null);
		}
	}

	public static void BreakNetworkTriggers()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0076: Expected O, but got Unknown
		string text = (Buttons.GetIndex("Switch to Modded Gamemode").enabled ? (((GorillaComputer)GorillaComputer.instance).currentQueue + "MODDED_") : ((GorillaComputer)GorillaComputer.instance).currentQueue);
		Hashtable val = new Hashtable();
		((Dictionary<object, object>)val).Add((object)"gameMode", (object)(string.Join("", ((GorillaComputer)GorillaComputer.instance).allowedMapsToJoin) + text + ((GorillaComputer)GorillaComputer.instance).currentGameMode.Value));
		Hashtable val2 = val;
		PhotonNetwork.CurrentRoom.SetCustomProperties(val2, (Hashtable)null, (WebFlags)null);
	}

	public static void KickNetworkTriggers()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_0039: Expected O, but got Unknown
		if (NetworkSystem.Instance.SessionIsPrivate)
		{
			Overpowered.SetRoomStatus(status: false);
		}
		Hashtable val = new Hashtable();
		((Dictionary<object, object>)val).Add((object)"gameMode", (object)((GorillaComputer)GorillaComputer.instance).currentGameMode.Value);
		Hashtable val2 = val;
		PhotonNetwork.CurrentRoom.SetCustomProperties(val2, (Hashtable)null, (WebFlags)null);
	}

	public static void SpazGamemode()
	{
		if (Time.time > spazGamemodeDelay)
		{
			ChangeGamemode((GameModeType)Random.Range(0, 13));
			spazGamemodeDelay = Time.time + 0.1f;
		}
	}

	public unsafe static void ChangeGamemode(GameModeType gamemode)
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		//IL_00bd: Expected O, but got Unknown
		if (!PhotonNetwork.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			return;
		}
		if (disablePatchCoroutine != null)
		{
			disablePatchCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(DisablePatch());
		}
		GameModePatch.enabled = true;
		NetworkSystem.Instance.NetDestroy(((Component)((GorillaWrappedSerializer)GameMode.activeNetworkHandler).NetView).gameObject);
		string arg = (moddedGamemode ? (((GorillaComputer)GorillaComputer.instance).currentQueue + "MODDED_") : ((GorillaComputer)GorillaComputer.instance).currentQueue);
		Hashtable val = new Hashtable();
		((Dictionary<object, object>)val).Add((object)"gameMode", (object)$"{PhotonNetwork.CurrentRoom.IsVisible}|{arg}|{gamemode}");
		Hashtable val2 = val;
		PhotonNetwork.CurrentRoom.SetCustomProperties(val2, (Hashtable)null, (WebFlags)null);
		GameMode.activeGameMode.StopPlaying();
		GameMode.activeGameMode = null;
		GameMode.activeNetworkHandler = null;
		GameMode.LoadGameMode(((object)(*(GameModeType*)(&gamemode))/*cast due to .constrained prefix*/).ToString());
	}
}
