using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Seralyth.Classes.Menu;
using Seralyth.Extensions;
using Seralyth.Managers;
using Seralyth.Menu;
using Seralyth.Utilities;
using UnityEngine;

namespace Seralyth.Mods.CustomMaps.Maps;

public class MonkeMagic : CustomMap
{
	private static float lightningDelay;

	private static float materialDelay;

	private static float lucyDelay;

	public static float crashDelay;

	public override long MapID => 5107228L;

	public override ButtonInfo[] Buttons => new ButtonInfo[15]
	{
		new ButtonInfo
		{
			buttonText = "Lightning Strike Self",
			method = LightningStrikeSelf,
			toolTip = "Strikes yourself with lightning."
		},
		new ButtonInfo
		{
			buttonText = "Lightning Strike Gun",
			method = LightningStrikeGun,
			toolTip = "Strikes whoever your hand desires with lightning."
		},
		new ButtonInfo
		{
			buttonText = "Lightning Strike All",
			method = LightningStrikeAll,
			toolTip = "Strikes everyone in the room with lightning."
		},
		new ButtonInfo
		{
			buttonText = "Change Material Self",
			method = ChangeMaterialSelf,
			toolTip = "Changes your material."
		},
		new ButtonInfo
		{
			buttonText = "Change Material Gun",
			method = ChangeMaterialGun,
			toolTip = "Changes the material of whoever your hand desires."
		},
		new ButtonInfo
		{
			buttonText = "Change Material All",
			method = ChangeMaterialAll,
			toolTip = "Changes the material of everyone in the room."
		},
		new ButtonInfo
		{
			buttonText = "Spawn Lucy Self",
			isTogglable = false,
			method = SpawnLucySelf,
			toolTip = "Spawns lucy on yourself."
		},
		new ButtonInfo
		{
			buttonText = "Spawn Lucy Gun",
			method = SpawnLucyGun,
			toolTip = "Spawns lucy on whoever your hand desires."
		},
		new ButtonInfo
		{
			buttonText = "Spawn Lucy All",
			isTogglable = false,
			method = SpawnLucyAll,
			toolTip = "Spawns lucy on everyone in the room."
		},
		new ButtonInfo
		{
			buttonText = "Monke Magic Crash Gun",
			overlapText = "Crash Gun",
			method = CrashGun,
			toolTip = "Crashes whoever your hand desires in the custom map."
		},
		new ButtonInfo
		{
			buttonText = "Monke Magic Crash All",
			overlapText = "Crash All",
			method = CrashAll,
			isTogglable = false,
			toolTip = "Crashes everyone in the custom map."
		},
		new ButtonInfo
		{
			buttonText = "Monke Magic Anti Report",
			overlapText = "Anti Report <color=grey>[</color><color=green>Crash</color><color=grey>]</color>",
			method = AntiReportCrash,
			toolTip = "Crashes everyone who tries to report you."
		},
		new ButtonInfo
		{
			buttonText = "Monke Magic Crash Aura",
			overlapText = "Crash Aura",
			method = CrashAura,
			toolTip = "Crashes players nearby you in the custom map."
		},
		new ButtonInfo
		{
			buttonText = "Monke Magic Crash On Touch",
			overlapText = "Crash On Touch",
			method = CrashOnTouch,
			toolTip = "Crashes whoever you touch in the custom map."
		},
		new ButtonInfo
		{
			buttonText = "Monke Magic Crash When Touched",
			overlapText = "Crash When Touched",
			method = CrashWhenTouched,
			toolTip = "Crashes whoever touches you in the custom map."
		}
	};

	public static void LightningStrikeSelf()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		if (Time.time > lightningDelay)
		{
			lightningDelay = Time.time + 0.2f;
			PhotonNetwork.RaiseEvent((byte)180, (object)new object[2]
			{
				"SummonThunder",
				(double)PhotonNetwork.LocalPlayer.ActorNumber
			}, new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)1
			}, SendOptions.SendReliable);
			Main.RPCProtection();
		}
	}

	public static void LightningStrikeGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && Time.time > lightningDelay)
			{
				lightningDelay = Time.time + 0.1f;
				PhotonNetwork.RaiseEvent((byte)180, (object)new object[2]
				{
					"SummonThunder",
					(double)RigUtilities.GetPlayerFromVRRig(Main.lockTarget).ActorNumber
				}, new RaiseEventOptions
				{
					Receivers = (ReceiverGroup)1
				}, SendOptions.SendReliable);
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

	public static void LightningStrikeAll()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		if (Time.time > lightningDelay)
		{
			lightningDelay = Time.time + 0.1f;
			PhotonNetwork.RaiseEvent((byte)180, (object)new object[2]
			{
				"SummonThunder",
				(double)RigUtilities.GetRandomPlayer(includeSelf: false).ActorNumber
			}, new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)1
			}, SendOptions.SendReliable);
			Main.RPCProtection();
		}
	}

	public static void ChangeMaterialSelf()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		if (Time.time > materialDelay)
		{
			materialDelay = Time.time + 0.1f;
			PhotonNetwork.RaiseEvent((byte)180, (object)new object[3]
			{
				"ChangingMaterial",
				(double)PhotonNetwork.LocalPlayer.ActorNumber,
				(double)Random.Range(0, VRRig.LocalRig.materialsToChangeTo.Length)
			}, new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)1
			}, SendOptions.SendReliable);
			Main.RPCProtection();
		}
	}

	public static void ChangeMaterialGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && Time.time > materialDelay)
			{
				materialDelay = Time.time + 0.1f;
				PhotonNetwork.RaiseEvent((byte)180, (object)new object[3]
				{
					"ChangingMaterial",
					(double)PhotonNetwork.LocalPlayer.ActorNumber,
					(double)Random.Range(0, VRRig.LocalRig.materialsToChangeTo.Length)
				}, new RaiseEventOptions
				{
					Receivers = (ReceiverGroup)1
				}, SendOptions.SendReliable);
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

	public static void ChangeMaterialAll()
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		if (Time.time > materialDelay)
		{
			materialDelay = Time.time + 0.2f;
			PhotonNetwork.RaiseEvent((byte)180, (object)new object[3]
			{
				"ChangingMaterial",
				(double)RigUtilities.GetPlayerFromVRRig(Main.lockTarget).ActorNumber,
				(double)Random.Range(0, VRRig.LocalRig.materialsToChangeTo.Length)
			}, new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)1
			}, SendOptions.SendReliable);
			Main.RPCProtection();
		}
	}

	public static void SpawnLucySelf()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		PhotonNetwork.RaiseEvent((byte)180, (object)new object[2]
		{
			"SummonLucy",
			(double)PhotonNetwork.LocalPlayer.ActorNumber
		}, new RaiseEventOptions
		{
			Receivers = (ReceiverGroup)1
		}, SendOptions.SendReliable);
		Main.RPCProtection();
	}

	public static void SpawnLucyGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true))
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal() && Time.time > lucyDelay)
			{
				lucyDelay = Time.time + 0.2f;
				PhotonNetwork.RaiseEvent((byte)180, (object)new object[2]
				{
					"SummonLucy",
					(double)RigUtilities.GetPlayerFromVRRig(Main.lockTarget).ActorNumber
				}, new RaiseEventOptions
				{
					Receivers = (ReceiverGroup)1
				}, SendOptions.SendReliable);
				Main.RPCProtection();
			}
		}
	}

	public static void SpawnLucyAll()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
		foreach (NetPlayer val in playerListOthers)
		{
			PhotonNetwork.RaiseEvent((byte)180, (object)new object[2]
			{
				"SummonLucy",
				(double)val.ActorNumber
			}, new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)1
			}, SendOptions.SendReliable);
			Main.RPCProtection();
		}
	}

	public static void CrashPlayer(int ActorNumber)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		object[] obj = new object[4]
		{
			"leaveGame",
			(double)ActorNumber,
			false,
			(double)ActorNumber
		};
		RaiseEventOptions val = new RaiseEventOptions();
		val.TargetActors = new int[1] { ActorNumber };
		PhotonNetwork.RaiseEvent((byte)180, (object)obj, val, SendOptions.SendReliable);
		Main.RPCProtection();
	}

	public static void CrashGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && Time.time > crashDelay)
			{
				NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(Main.lockTarget);
				CrashPlayer(playerFromVRRig.ActorNumber);
				crashDelay = Time.time + 0.2f;
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

	public static void CrashAura()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time < crashDelay || !PhotonNetwork.InRoom)
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
			CrashPlayer(item.GetPlayer().ActorNumber);
			crashDelay = Time.time + 0.2f;
		}
	}

	public static void CrashOnTouch()
	{
		if (Time.time < crashDelay)
		{
			return;
		}
		foreach (NetPlayer item in from rig in VRRigCache.ActiveRigs
			where !rig.isLocal && (Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, rig.headMesh.transform.position) < 0.25f || Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, rig.headMesh.transform.position) < 0.25f)
			select RigUtilities.GetPlayerFromVRRig(rig))
		{
			CrashPlayer(item.ActorNumber);
			crashDelay = Time.time + 0.2f;
		}
	}

	public static void CrashWhenTouched()
	{
		if (Time.time < crashDelay)
		{
			return;
		}
		foreach (NetPlayer item in from vrrig in VRRigCache.ActiveRigs
			where !vrrig.isMyPlayer && !vrrig.isOfflineVRRig && ((double)Vector3.Distance(vrrig.rightHandTransform.position, ((Component)GorillaTagger.Instance.offlineVRRig).transform.position) <= 0.5 || (double)Vector3.Distance(vrrig.leftHandTransform.position, ((Component)GorillaTagger.Instance.offlineVRRig).transform.position) <= 0.5 || (double)Vector3.Distance(((Component)vrrig).transform.position, ((Component)GorillaTagger.Instance.offlineVRRig).transform.position) <= 0.5)
			select RigUtilities.GetPlayerFromVRRig(vrrig))
		{
			CrashPlayer(item.ActorNumber);
			crashDelay = Time.time + 0.2f;
		}
	}

	public static void CrashAll()
	{
		if (Time.time > crashDelay)
		{
			NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
			foreach (NetPlayer val in playerListOthers)
			{
				CrashPlayer(val.ActorNumber);
			}
			crashDelay = Time.time + 0.1f;
		}
	}

	public static void AntiReportCrash()
	{
		Safety.AntiReport(delegate(VRRig vrrig, Vector3 position)
		{
			if (Time.time > crashDelay)
			{
				NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(vrrig);
				CrashPlayer(playerFromVRRig.ActorNumber);
				crashDelay = Time.time + 0.5f;
				NotificationManager.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> " + RigUtilities.GetPlayerFromVRRig(vrrig).NickName + " attempted to report you, they have been crashed.");
			}
		});
	}
}
