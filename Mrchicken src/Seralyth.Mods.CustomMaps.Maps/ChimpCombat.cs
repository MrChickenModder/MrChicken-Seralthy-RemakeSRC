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

public class ChimpCombat : CustomMap
{
	public static float killDelay;

	public static float crashDelay;

	public override long MapID => 5135423L;

	public override ButtonInfo[] Buttons => new ButtonInfo[16]
	{
		new ButtonInfo
		{
			buttonText = "Kill Self Player",
			overlapText = "Kill Self",
			method = KillSelf,
			isTogglable = false,
			toolTip = "Kills yourself."
		},
		new ButtonInfo
		{
			buttonText = "Kill Player Gun",
			overlapText = "Kill Gun",
			method = KillGun,
			toolTip = "Kills whoever your hand desires."
		},
		new ButtonInfo
		{
			buttonText = "Kill All Players",
			overlapText = "Kill All",
			method = KillAll,
			isTogglable = false,
			toolTip = "Kills everyone in the room."
		},
		new ButtonInfo
		{
			buttonText = "God Mode_",
			overlapText = "God Mode",
			enableMethod = GodMode,
			disableMethod = DisableGodMode,
			toolTip = "Prevents you from getting killed."
		},
		new ButtonInfo
		{
			buttonText = "No Grenade Cooldown",
			enableMethod = NoGrenadeCooldown,
			disableMethod = DisableNoGrenadeCooldown,
			toolTip = "Disables the cooldown on spawning grenades."
		},
		new ButtonInfo
		{
			buttonText = "No Shoot Cooldown",
			enableMethod = NoShootCooldown,
			disableMethod = DisableNoShootCooldown,
			toolTip = "Disables the cooldown on shooting."
		},
		new ButtonInfo
		{
			buttonText = "Rapid Fire",
			enableMethod = RapidFire,
			disableMethod = DisableRapidFire,
			toolTip = "Automatically shoots when holding down right trigger."
		},
		new ButtonInfo
		{
			buttonText = "Instant Kill",
			enableMethod = InstantKill,
			disableMethod = DisableInstantKill,
			toolTip = "Makes your gun instant kill players."
		},
		new ButtonInfo
		{
			buttonText = "Infinite Points",
			enableMethod = InfinitePoints,
			disableMethod = DisableInfinitePoints,
			toolTip = "Gives you an infinite amount of points."
		},
		new ButtonInfo
		{
			buttonText = "Infinite Ammo",
			enableMethod = InfiniteAmmo,
			disableMethod = DisableInfiniteAmmo,
			toolTip = "Gives you an infinite amount of ammo."
		},
		new ButtonInfo
		{
			buttonText = "Chimp Combat Crash Gun",
			overlapText = "Crash Gun",
			method = CrashGun,
			toolTip = "Crashes whoever your hand desires in the custom map."
		},
		new ButtonInfo
		{
			buttonText = "Chimp Combat Crash All",
			overlapText = "Crash All",
			method = CrashAll,
			isTogglable = false,
			toolTip = "Crashes everyone in the custom map."
		},
		new ButtonInfo
		{
			buttonText = "Chimp Combat Anti Report",
			overlapText = "Anti Report <color=grey>[</color><color=green>Crash</color><color=grey>]</color>",
			method = AntiReportCrash,
			toolTip = "Crashes everyone who tries to report you."
		},
		new ButtonInfo
		{
			buttonText = "Chimp Combat Crash Aura",
			overlapText = "Crash Aura",
			method = CrashAura,
			toolTip = "Crashes players nearby you in the custom map."
		},
		new ButtonInfo
		{
			buttonText = "Chimp Combat Crash On Touch",
			overlapText = "Crash On Touch",
			method = CrashOnTouch,
			toolTip = "Crashes whoever you touch in the custom map."
		},
		new ButtonInfo
		{
			buttonText = "Chimp Combat Crash When Touched",
			overlapText = "Crash When Touched",
			method = CrashWhenTouched,
			toolTip = "Crashes whoever touches you in the custom map."
		}
	};

	public static void KillPlayer(int ActorNumber)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		object[] obj = new object[4]
		{
			"HitPlayer",
			(double)ActorNumber,
			false,
			(double)ActorNumber
		};
		RaiseEventOptions val = new RaiseEventOptions();
		val.TargetActors = new int[1] { ActorNumber };
		PhotonNetwork.RaiseEvent((byte)180, (object)obj, val, SendOptions.SendReliable);
		Main.RPCProtection();
	}

	public static void KillSelf()
	{
		NetPlayer val = NetPlayer.op_Implicit(PhotonNetwork.LocalPlayer);
		KillPlayer(val.ActorNumber);
	}

	public static void KillGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && Time.time > killDelay)
			{
				NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(Main.lockTarget);
				KillPlayer(playerFromVRRig.ActorNumber);
				killDelay = Time.time + 0.2f;
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

	public static void KillAll()
	{
		if (Time.time > killDelay)
		{
			NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
			foreach (NetPlayer val in playerListOthers)
			{
				KillPlayer(val.ActorNumber);
			}
			killDelay = Time.time + 0.1f;
		}
	}

	public static void GodMode()
	{
		Manager.ModifyCustomScript(new Dictionary<int, string> { { 957, "if not IsMe then PlayerData[Player.playerID].Health -= Modules.roundToQuarter(dmg) end" } });
	}

	public static void DisableGodMode()
	{
		Manager.RevertCustomScript(957);
	}

	public static void NoGrenadeCooldown()
	{
		Manager.ModifyCustomScript(new Dictionary<int, string> { { 1296, "grenadeCooldown = 0" } });
	}

	public static void DisableNoGrenadeCooldown()
	{
		Manager.RevertCustomScript(1296);
	}

	public static void NoShootCooldown()
	{
		Manager.ModifyCustomScript(new Dictionary<int, string> { { 1243, "shootCooldown = 0" } });
	}

	public static void DisableNoShootCooldown()
	{
		Manager.RevertCustomScript(1243);
	}

	public static void InfiniteAmmo()
	{
		Manager.ModifyCustomScript(new Dictionary<int, string> { { 1244, "" } });
	}

	public static void DisableInfiniteAmmo()
	{
		Manager.RevertCustomScript(1244);
	}

	public static void InstantKill()
	{
		Manager.ModifyCustomScript(new Dictionary<int, string> { { 1278, "emitAndOnEvent(\"HitPlayer\", {found.playerID, 99999.0, LocalPlayer.playerID})" } });
	}

	public static void DisableInstantKill()
	{
		Manager.RevertCustomScript(1278);
	}

	public static void InfinitePoints()
	{
		Manager.ModifyCustomScript(new Dictionary<int, string> { { 496, "saveData[\"Points\"] = 999999" } });
	}

	public static void DisableInfinitePoints()
	{
		Manager.RevertCustomScript(496);
	}

	public static void RapidFire()
	{
		Manager.ModifyCustomScript(new Dictionary<int, string> { { 2041, "needsLetGoR = false" } });
	}

	public static void DisableRapidFire()
	{
		Manager.RevertCustomScript(2041);
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
