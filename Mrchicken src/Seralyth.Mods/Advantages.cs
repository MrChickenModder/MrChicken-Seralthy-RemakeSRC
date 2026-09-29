using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using GorillaGameModes;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using Seralyth.Extensions;
using Seralyth.Managers;
using Seralyth.Menu;
using Seralyth.Patches.Menu;
using Seralyth.Utilities;
using UnityEngine;

namespace Seralyth.Mods;

public static class Advantages
{
	public static bool instantTag = true;

	public static float spamTagDelay;

	public static float tagAuraDistance = 1.666f;

	public static int tagAuraIndex = 1;

	public static int tagRangeIndex;

	private static float tagReachDistance = 0.3f;

	private static float reportTagDelay;

	private static float tagGunDelay;

	public static float paintbrawlSpamDelay;

	public static int paintbrawlKillIndex;

	public static readonly Dictionary<int, float> paintbrawlKillDelays = new Dictionary<int, float>();

	public static void TagSelf()
	{
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		if (PhotonNetwork.IsMasterClient)
		{
			GameModeUtilities.AddInfected(NetPlayer.op_Implicit(PhotonNetwork.LocalPlayer));
			NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> You have been tagged.");
			Buttons.GetIndex("Tag Self").enabled = false;
			return;
		}
		if (GameModeUtilities.InfectedList().Contains(NetPlayer.op_Implicit(PhotonNetwork.LocalPlayer)))
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> You have been tagged.");
			((Behaviour)VRRig.LocalRig).enabled = true;
			Buttons.GetIndex("Tag Self").enabled = false;
			if (instantTag)
			{
				SerializePatch.OverrideSerialization = null;
			}
			return;
		}
		VRRig rig = VRRigCache.ActiveRigs.Where((VRRig r) => !r.IsLocal() && r.IsTagged()).OrderBy(delegate(VRRig r)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			float num = Vector3.Distance(((Component)r).transform.position, ((Component)GorillaTagger.Instance.headCollider).transform.position);
			Vector3 val = r.LatestVelocity();
			return num + ((Vector3)(ref val)).magnitude;
		}).FirstOrDefault();
		if (instantTag)
		{
			SerializePatch.OverrideSerialization = delegate
			{
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Expected O, but got Unknown
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				if (VRRig.LocalRig.IsTagged())
				{
					return true;
				}
				Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
				Vector3 position = ((Component)VRRig.LocalRig).transform.position;
				((Component)VRRig.LocalRig).transform.position = ((Component)rig.rightHandTransform).transform.position;
				PhotonView photonView = VRRig.LocalRig.GetPhotonView();
				RaiseEventOptions val = new RaiseEventOptions();
				val.TargetActors = new int[2]
				{
					PhotonNetwork.MasterClient.ActorNumber,
					rig.GetPlayer().ActorNumber
				};
				Main.SendSerialize(photonView, val);
				Main.RPCProtection();
				((Component)VRRig.LocalRig).transform.position = position;
				return false;
			};
		}
		else if (rig.IsTagged())
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			if ((Object)(object)rig != (Object)null)
			{
				((Component)VRRig.LocalRig).transform.position = rig.rightHandTransform.position;
			}
			if (Buttons.GetIndex("Obnoxious Tag").enabled)
			{
				Quaternion rotation = Quaternion.Euler(new Vector3(0f, (float)Random.Range(0, 360), 0f));
				((Component)VRRig.LocalRig).transform.rotation = rotation;
				((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = RandomUtilities.RandomQuaternion();
				((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + RandomUtilities.RandomVector3();
				((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + RandomUtilities.RandomVector3();
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
			}
		}
	}

	public static void UntagSelf()
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			Important.Reconnect();
			NoTagOnJoin();
		}
		else
		{
			GameModeUtilities.RemoveInfected(NetPlayer.op_Implicit(PhotonNetwork.LocalPlayer));
		}
		GTPlayer.Instance.disableMovement = false;
	}

	public static void AntiTag()
	{
		if (PhotonNetwork.InRoom)
		{
			if (PhotonNetwork.IsMasterClient)
			{
				if (!ReportTagPatch.invinciblePlayers.Contains(NetworkSystem.Instance.LocalPlayer))
				{
					ReportTagPatch.invinciblePlayers.Add(NetworkSystem.Instance.LocalPlayer);
				}
			}
			else if (VRRig.LocalRig.IsTagged())
			{
				UntagSelf();
			}
		}
		else
		{
			NoTagOnJoin();
			ReportTagPatch.invinciblePlayers.Clear();
		}
	}

	public static void UntagAll()
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			return;
		}
		Player[] playerList = PhotonNetwork.PlayerList;
		foreach (Player val in playerList)
		{
			GameModeUtilities.RemoveInfected(NetPlayer.op_Implicit(val));
		}
	}

	public static void SpamTagSelf()
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
		else if (Time.time > spamTagDelay)
		{
			spamTagDelay = Time.time + 0.1f;
			if (GameModeUtilities.InfectedList().Contains(NetPlayer.op_Implicit(PhotonNetwork.LocalPlayer)))
			{
				GameModeUtilities.RemoveInfected(NetPlayer.op_Implicit(PhotonNetwork.LocalPlayer));
			}
			else
			{
				GameModeUtilities.AddInfected(NetPlayer.op_Implicit(PhotonNetwork.LocalPlayer));
			}
		}
	}

	public static void SpamTagGun()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				if (!NetworkSystem.Instance.IsMasterClient)
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
				}
				else if (Time.time > spamTagDelay)
				{
					spamTagDelay = Time.time + 0.1f;
					if (GameModeUtilities.InfectedList().Contains(RigUtilities.GetPlayerFromVRRig(Main.lockTarget)))
					{
						GameModeUtilities.RemoveInfected(RigUtilities.GetPlayerFromVRRig(Main.lockTarget));
					}
					else
					{
						GameModeUtilities.AddInfected(RigUtilities.GetPlayerFromVRRig(Main.lockTarget));
					}
				}
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal() && PhotonNetwork.IsMasterClient)
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

	public static void SpamTagAll()
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
		else
		{
			if (!(Time.time > spamTagDelay))
			{
				return;
			}
			spamTagDelay = Time.time + 0.1f;
			Player[] playerList = PhotonNetwork.PlayerList;
			foreach (Player val in playerList)
			{
				if (GameModeUtilities.InfectedList().Contains(NetPlayer.op_Implicit(val)))
				{
					GameModeUtilities.AddInfected(NetPlayer.op_Implicit(val));
				}
				else
				{
					GameModeUtilities.RemoveInfected(NetPlayer.op_Implicit(val));
				}
			}
		}
	}

	public static void TagLagGun()
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
			if (!Object.op_Implicit((Object)(object)componentInParent) || componentInParent.IsLocal())
			{
				return;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				if ((Object)(object)Main.lockTarget != (Object)null)
				{
					ReportTagPatch.blacklistedPlayers.Remove(RigUtilities.GetPlayerFromVRRig(Main.lockTarget));
				}
				Main.gunLocked = true;
				Main.lockTarget = componentInParent;
				ReportTagPatch.blacklistedPlayers.Add(RigUtilities.GetPlayerFromVRRig(componentInParent));
			}
			else
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
			ReportTagPatch.blacklistedPlayers.Remove(RigUtilities.GetPlayerFromVRRig(Main.lockTarget));
		}
	}

	public static void GiveTagLagGun()
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
			if (!Object.op_Implicit((Object)(object)componentInParent) || componentInParent.IsLocal())
			{
				return;
			}
			if (PhotonNetwork.IsMasterClient)
			{
				if ((Object)(object)Main.lockTarget != (Object)null)
				{
					ReportTagPatch.invinciblePlayers.Remove(RigUtilities.GetPlayerFromVRRig(Main.lockTarget));
				}
				Main.gunLocked = true;
				Main.lockTarget = componentInParent;
				ReportTagPatch.invinciblePlayers.Add(RigUtilities.GetPlayerFromVRRig(componentInParent));
			}
			else
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
			ReportTagPatch.invinciblePlayers.Remove(RigUtilities.GetPlayerFromVRRig(Main.lockTarget));
		}
	}

	public static void SetTagCooldown(float value)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		if (!NetworkSystem.Instance.InRoom)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not in a room.");
			return;
		}
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			return;
		}
		GorillaTagManager val = (GorillaTagManager)GorillaGameManager.instance;
		val.tagCoolDown = value;
	}

	public static void ChangeTagAuraRange(bool positive = true)
	{
		string[] array = new string[4] { "Short", "Normal", "Far", "Maximum" };
		float[] array2 = new float[4] { 0.777f, 1.666f, 3f, 5.5f };
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				tagAuraIndex++;
			}
			else
			{
				tagAuraIndex--;
			}
		}
		tagAuraIndex %= array.Length;
		if (tagAuraIndex < 0)
		{
			tagAuraIndex = array.Length - 1;
		}
		tagAuraDistance = array2[tagAuraIndex];
		Buttons.GetIndex("ctaRange").overlapText = "Change Tag Aura Range <color=grey>[</color><color=green>" + array[tagAuraIndex] + "</color><color=grey>]</color>";
	}

	public static void ChangeTagReachDistance(bool positive = true)
	{
		string[] array = new string[4] { "Unnoticable", "Normal", "Far", "Maximum" };
		float[] array2 = new float[4] { 0.3f, 0.5f, 1f, 3f };
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				tagRangeIndex++;
			}
			else
			{
				tagRangeIndex--;
			}
		}
		tagRangeIndex %= array.Length;
		if (tagRangeIndex < 0)
		{
			tagRangeIndex = array.Length - 1;
		}
		tagReachDistance = array2[tagRangeIndex];
		Buttons.GetIndex("ctrRange").overlapText = "Change Tag Reach Distance <color=grey>[</color><color=green>" + array[tagRangeIndex] + "</color><color=grey>]</color>";
	}

	public static void TagAura()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		Color val = Color.red;
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig vrrig) => VRRig.LocalRig.IsTagged() && !vrrig.IsTagged() && !GTPlayer.Instance.disableMovement && Vector3.Distance(vrrig.headMesh.transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) < tagAuraDistance))
		{
			val = Color.green;
			ReportTag(item);
		}
		if (Buttons.GetIndex("Visualize Tag Aura").enabled)
		{
			Visuals.VisualizeCylinder(VRRig.LocalRig.bodyTransform.position, Quaternion.identity, new Vector3(tagAuraDistance, 0.01f, tagAuraDistance), Buttons.GetIndex("Prettier Visualize").enabled ? val : Main.backgroundColor.GetCurrentColor(), -20170121181L, 0.1f);
		}
	}

	public static void GripTagAura()
	{
		if (Main.rightGrab)
		{
			TagAura();
		}
	}

	public static void TagAuraPlayer(VRRig giving)
	{
		foreach (VRRig item in from vrrig in VRRigCache.ActiveRigs
			let distance = Vector3.Distance(vrrig.headMesh.transform.position, ((Component)giving).transform.position)
			where giving.IsTagged() && !vrrig.IsTagged() && !GTPlayer.Instance.disableMovement && distance < tagAuraDistance && !VRRig.LocalRig.IsLocal() && VRRig.LocalRig.IsTagged()
			select vrrig)
		{
			TagPlayer(RigUtilities.GetPlayerFromVRRig(item));
		}
	}

	public static void TagAuraGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				TagAuraPlayer(Main.lockTarget);
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

	public static void TagAuraAll()
	{
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			TagAuraPlayer(activeRig);
		}
	}

	public static void TagReach()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		if (VRRig.LocalRig.IsTagged())
		{
			GorillaTagger.Instance.maxTagDistance = float.MaxValue;
			GorillaTagger.Instance.tagRadiusOverride = tagReachDistance;
			GorillaTagger.Instance.tagRadiusOverrideFrame = Time.frameCount + 16;
			if (Buttons.GetIndex("Visualize Tag Reach").enabled)
			{
				Visuals.VisualizeAura(GorillaTagger.Instance.leftHandTransform.position, tagReachDistance, Main.backgroundColor.GetCurrentColor(), -149286L);
				Visuals.VisualizeAura(GorillaTagger.Instance.rightHandTransform.position, tagReachDistance, Main.backgroundColor.GetCurrentColor(), -149285L);
			}
		}
	}

	public static bool ValidateTag(VRRig Rig)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Distance(Main.ServerSyncPos, ((Component)Rig).transform.position) < 6f;
	}

	public static void TagGun()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (instantTag)
		{
			InstantTagGun();
		}
		else if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				if (!Main.lockTarget.IsTagged())
				{
					((Behaviour)VRRig.LocalRig).enabled = false;
					if (!Buttons.GetIndex("Obnoxious Tag").enabled)
					{
						((Component)VRRig.LocalRig).transform.position = ((Component)Main.lockTarget).transform.position - new Vector3(0f, 3f, 0f);
					}
					else
					{
						Vector3 position = ((Component)Main.lockTarget).transform.position + RandomUtilities.RandomVector3();
						((Component)VRRig.LocalRig).transform.position = position;
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
					}
					if (ValidateTag(Main.lockTarget))
					{
						ReportTag(Main.lockTarget);
					}
				}
				else
				{
					Main.gunLocked = false;
					((Behaviour)VRRig.LocalRig).enabled = true;
				}
			}
			if (!Main.GetGunInput(isShooting: true))
			{
				return;
			}
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				if (PhotonNetwork.IsMasterClient)
				{
					GameModeUtilities.AddInfected(RigUtilities.GetPlayerFromVRRig(componentInParent));
				}
				else if (VRRig.LocalRig.IsTagged())
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

	public static void ReportTag(VRRig rig)
	{
		if (Time.time > reportTagDelay)
		{
			reportTagDelay = Time.time + 0.1f;
			GameMode.ReportTag(RigUtilities.GetPlayerFromVRRig(rig));
		}
	}

	public static void TagPlayer(NetPlayer player)
	{
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		if (PhotonNetwork.IsMasterClient)
		{
			GameModeUtilities.AddInfected(player);
			return;
		}
		if (!VRRig.LocalRig.IsTagged())
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must be tagged.");
			Buttons.GetIndex("Tag Player").enabled = false;
			return;
		}
		if (instantTag)
		{
			InstantTagPlayer(player);
			Buttons.GetIndex("Tag Player").enabled = false;
			return;
		}
		VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(player);
		if (!vRRigFromPlayer.IsTagged())
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			if (!Buttons.GetIndex("Obnoxious Tag").enabled)
			{
				((Component)VRRig.LocalRig).transform.position = ((Component)vRRigFromPlayer).transform.position - new Vector3(0f, 3f, 0f);
			}
			else
			{
				Vector3 position = ((Component)vRRigFromPlayer).transform.position + RandomUtilities.RandomVector3();
				((Component)VRRig.LocalRig).transform.position = position;
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
			}
			if (ValidateTag(vRRigFromPlayer))
			{
				ReportTag(vRRigFromPlayer);
			}
		}
		else
		{
			Buttons.GetIndex("Tag Player").enabled = false;
		}
	}

	public static void UntagGun()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
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
		if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal() && componentInParent.IsTagged())
		{
			if (PhotonNetwork.IsMasterClient)
			{
				GameModeUtilities.RemoveInfected(RigUtilities.GetPlayerFromVRRig(componentInParent));
			}
			else
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			}
		}
	}

	public static void FlickTagGun()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		GameObject item = Main.RenderGun(LayerMask.op_Implicit(GTPlayer.Instance.locomotionEnabledLayers)).NewPointer;
		if (Main.GetGunInput(isShooting: true))
		{
			GTPlayer.Instance.GetControllerTransform(false).position = item.transform.position;
			if (Vector3.Distance(GTPlayer.Instance.GetControllerTransform(false).position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) > 4f)
			{
				GTPlayer.Instance.GetControllerTransform(false).position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + (GTPlayer.Instance.GetControllerTransform(false).position - ((Component)GorillaTagger.Instance.bodyCollider).transform.position) * 4f;
			}
		}
	}

	public static void TagAll()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		if ((int)GorillaGameManager.instance.GameType() == 2)
		{
			HuntTagAll();
			return;
		}
		if (NetworkSystem.Instance.IsMasterClient)
		{
			Player[] playerList = PhotonNetwork.PlayerList;
			foreach (Player val in playerList)
			{
				GameModeUtilities.AddInfected(NetPlayer.op_Implicit(val));
			}
			Buttons.GetIndex("Tag All").enabled = false;
			NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Everyone is tagged!");
			return;
		}
		if (instantTag)
		{
			InstantTagAll();
			NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Everyone is tagged!");
			Buttons.GetIndex("Tag All").enabled = false;
			return;
		}
		if (!VRRig.LocalRig.IsTagged())
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must be tagged.");
			Buttons.GetIndex("Tag All").enabled = false;
			return;
		}
		if (VRRigCache.ActiveRigs.Any((VRRig vrrig) => !vrrig.IsTagged()))
		{
			foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.IsTagged()))
			{
				((Behaviour)VRRig.LocalRig).enabled = false;
				if (!Buttons.GetIndex("Obnoxious Tag").enabled)
				{
					((Component)VRRig.LocalRig).transform.position = ((Component)item).transform.position - new Vector3(0f, 3f, 0f);
				}
				else
				{
					Vector3 position = ((Component)item).transform.position + RandomUtilities.RandomVector3();
					((Component)VRRig.LocalRig).transform.position = position;
					((Component)VRRig.LocalRig).transform.rotation = RandomUtilities.RandomQuaternion();
					((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = RandomUtilities.RandomQuaternion();
					((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)item).transform.position + RandomUtilities.RandomVector3();
					((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)item).transform.position + RandomUtilities.RandomVector3();
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
				}
				if (ValidateTag(item))
				{
					ReportTag(item);
				}
			}
			return;
		}
		NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Everyone is tagged!");
		((Behaviour)VRRig.LocalRig).enabled = true;
		Buttons.GetIndex("Tag All").enabled = false;
	}

	public static void InstantTagPlayer(NetPlayer Target)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if (VRRig.LocalRig.IsTagged() && !Target.VRRig().IsTagged())
		{
			Vector3 position = ((Component)VRRig.LocalRig).transform.position;
			((Component)VRRig.LocalRig).transform.position = ((Component)RigUtilities.GetVRRigFromPlayer(Target)).transform.position;
			PhotonView photonView = VRRig.LocalRig.GetPhotonView();
			RaiseEventOptions val = new RaiseEventOptions();
			val.TargetActors = new int[1] { PhotonNetwork.MasterClient.ActorNumber };
			Main.SendSerialize(photonView, val);
			GameMode.ReportTag(Target);
			((Component)VRRig.LocalRig).transform.position = position;
			Main.RPCProtection();
		}
	}

	public static void InstantTagGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > tagGunDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				tagGunDelay = Time.time + 0.2f;
				InstantTagPlayer(NetPlayer.op_Implicit(RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(componentInParent))));
			}
		}
	}

	public static void InstantTagAll()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		if (!VRRig.LocalRig.IsTagged())
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You must be tagged.");
			return;
		}
		Vector3 position = ((Component)VRRig.LocalRig).transform.position;
		RaiseEventOptions val;
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.IsTagged()))
		{
			((Component)VRRig.LocalRig).transform.position = ((Component)item).transform.position;
			PhotonView photonView = VRRig.LocalRig.GetPhotonView();
			val = new RaiseEventOptions();
			val.TargetActors = new int[1] { PhotonNetwork.MasterClient.ActorNumber };
			Main.SendSerialize(photonView, val);
			GameMode.ReportTag(RigUtilities.GetPlayerFromVRRig(item));
		}
		((Component)VRRig.LocalRig).transform.position = position;
		PhotonView photonView2 = VRRig.LocalRig.GetPhotonView();
		val = new RaiseEventOptions();
		val.TargetActors = new int[1] { PhotonNetwork.MasterClient.ActorNumber };
		Main.SendSerialize(photonView2, val);
		Main.RPCProtection();
	}

	public static void HuntTagAll()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		GorillaHuntManager val = (GorillaHuntManager)GorillaGameManager.instance;
		NetPlayer targetOf = val.GetTargetOf(NetPlayer.op_Implicit(PhotonNetwork.LocalPlayer));
		if (!GTPlayer.Instance.disableMovement)
		{
			VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(targetOf);
			((Behaviour)VRRig.LocalRig).enabled = false;
			if (!Buttons.GetIndex("Obnoxious Tag").enabled)
			{
				((Component)VRRig.LocalRig).transform.position = ((Component)vRRigFromPlayer).transform.position - new Vector3(0f, 3f, 0f);
			}
			else
			{
				Vector3 position = ((Component)vRRigFromPlayer).transform.position + RandomUtilities.RandomVector3();
				((Component)VRRig.LocalRig).transform.position = position;
				((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = RandomUtilities.RandomQuaternion();
				((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)vRRigFromPlayer).transform.position + RandomUtilities.RandomVector3();
				((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)vRRigFromPlayer).transform.position + RandomUtilities.RandomVector3();
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
			}
			if (ValidateTag(vRRigFromPlayer))
			{
				ReportTag(vRRigFromPlayer);
			}
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Everyone is tagged!");
			((Behaviour)VRRig.LocalRig).enabled = true;
			Buttons.GetIndex("Tag All").enabled = false;
			Main.ReloadMenu();
		}
	}

	public static void TagBot()
	{
		if (PhotonNetwork.InRoom)
		{
			if (!VRRig.LocalRig.IsTagged())
			{
				if (GameModeUtilities.InfectedList().Count > 0)
				{
					TagSelf();
				}
			}
			else if (GameModeUtilities.InfectedList().Count != PhotonNetwork.PlayerList.Length)
			{
				TagAll();
			}
		}
		else
		{
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void NoTagOnJoin()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_005b: Expected O, but got Unknown
		((Dictionary<object, object>)(object)PhotonNetwork.LocalPlayer.CustomProperties).TryGetValue((object)"didTutorial", out object value);
		if (value != null)
		{
			bool flag = default(bool);
			int num;
			if (value is bool)
			{
				flag = (bool)value;
				num = 1;
			}
			else
			{
				num = 0;
			}
			if (((uint)num & (flag ? 1u : 0u)) == 0)
			{
				return;
			}
		}
		Player localPlayer = PhotonNetwork.LocalPlayer;
		Hashtable val = new Hashtable();
		((Dictionary<object, object>)val).Add((object)"didTutorial", (object)false);
		localPlayer.SetCustomProperties(val, (Hashtable)null, (WebFlags)null);
	}

	public static void TagOnJoin()
	{
		NetworkSystem.Instance.SetMyTutorialComplete();
	}

	public static void ReportAntiTag()
	{
		SerializePatch.OverrideSerialization = delegate
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Expected O, but got Unknown
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Expected O, but got Unknown
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			if (VRRig.LocalRig.IsTagged())
			{
				return true;
			}
			Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
			Vector3 position = ((Component)VRRig.LocalRig).transform.position;
			Main.SendSerialize(VRRig.LocalRig.GetPhotonView(), new RaiseEventOptions
			{
				TargetActors = (from plr in PhotonNetwork.PlayerList
					where plr.ActorNumber != PhotonNetwork.MasterClient.ActorNumber
					select plr.ActorNumber).ToArray()
			});
			((Component)VRRig.LocalRig).transform.position = new Vector3(99999f, 99999f, 99999f);
			PhotonView photonView = VRRig.LocalRig.GetPhotonView();
			RaiseEventOptions val = new RaiseEventOptions();
			val.TargetActors = new int[1] { PhotonNetwork.MasterClient.ActorNumber };
			Main.SendSerialize(photonView, val);
			Main.RPCProtection();
			((Component)VRRig.LocalRig).transform.position = position;
			return false;
		};
	}

	public static void PaintbrawlStartGame()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			return;
		}
		GorillaPaintbrawlManager val = (GorillaPaintbrawlManager)GorillaGameManager.instance;
		val.StartBattle();
	}

	public static void PaintbrawlEndGame()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			return;
		}
		GorillaPaintbrawlManager val = (GorillaPaintbrawlManager)GorillaGameManager.instance;
		val.BattleEnd();
	}

	public static void PaintbrawlRestartGame()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			return;
		}
		GorillaPaintbrawlManager val = (GorillaPaintbrawlManager)GorillaGameManager.instance;
		val.BattleEnd();
		val.StartBattle();
	}

	public static void PaintbrawlBalloonSpamSelf()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		if (!(Time.time < paintbrawlSpamDelay))
		{
			paintbrawlSpamDelay = Time.time + 0.1f;
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
				return;
			}
			GorillaPaintbrawlManager val = (GorillaPaintbrawlManager)GorillaGameManager.instance;
			val.playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = Random.Range(0, 4);
		}
	}

	public static void PaintbrawlBalloonSpamGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				if (!NetworkSystem.Instance.IsMasterClient)
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
				}
				else if (Time.time > paintbrawlSpamDelay)
				{
					paintbrawlSpamDelay = Time.time + 0.1f;
					if (!NetworkSystem.Instance.IsMasterClient)
					{
						NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
					}
					else
					{
						GorillaPaintbrawlManager val2 = (GorillaPaintbrawlManager)GorillaGameManager.instance;
						val2.playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = Random.Range(0, 4);
					}
				}
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal() && PhotonNetwork.IsMasterClient)
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

	public static void PaintbrawlBalloonSpam()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		if (Time.time < paintbrawlSpamDelay)
		{
			return;
		}
		paintbrawlSpamDelay = Time.time + 0.1f;
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			return;
		}
		GorillaPaintbrawlManager val = (GorillaPaintbrawlManager)GorillaGameManager.instance;
		Player[] playerList = PhotonNetwork.PlayerList;
		foreach (Player val2 in playerList)
		{
			val.playerLives[val2.ActorNumber] = Random.Range(0, 4);
		}
	}

	public static void PaintbrawlKillGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
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
		if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
		{
			NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(componentInParent);
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
				return;
			}
			GorillaPaintbrawlManager val2 = (GorillaPaintbrawlManager)GorillaGameManager.instance;
			val2.playerLives[playerFromVRRig.ActorNumber] = 0;
		}
	}

	public static void PaintbrawlKillPlayer(NetPlayer Target)
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			if (!paintbrawlKillDelays.TryGetValue(Target.ActorNumber, out var value) || !(Time.time > value))
			{
				paintbrawlKillDelays[Target.ActorNumber] = Time.time + 3.1f;
				VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(Target);
				((GorillaWrappedSerializer)GameMode.ActiveNetworkHandler).SendRPC("RPC_ReportSlingshotHit", false, new object[3]
				{
					RigUtilities.NetPlayerToPlayer(Target),
					((Component)vRRigFromPlayer).transform.position,
					paintbrawlKillIndex
				});
				Main.RPCProtection();
				paintbrawlKillIndex++;
			}
		}
		else
		{
			GorillaPaintbrawlManager val = (GorillaPaintbrawlManager)GorillaGameManager.instance;
			val.playerLives[Target.ActorNumber] = 0;
		}
	}

	public static void PaintbrawlKillSelf()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			PaintbrawlKillPlayer(NetworkSystem.Instance.LocalPlayer);
			return;
		}
		GorillaPaintbrawlManager val = (GorillaPaintbrawlManager)GorillaGameManager.instance;
		val.playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = 0;
	}

	public static void PaintbrawlKillAll()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			PaintbrawlKillPlayer(NetPlayer.op_Implicit(RigUtilities.GetRandomPlayer(includeSelf: false)));
			return;
		}
		GorillaPaintbrawlManager val = (GorillaPaintbrawlManager)GorillaGameManager.instance;
		Player[] playerList = PhotonNetwork.PlayerList;
		foreach (Player val2 in playerList)
		{
			val.playerLives[val2.ActorNumber] = 0;
		}
	}

	public static void PaintbrawlReviveGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
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
		if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
		{
			NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(componentInParent);
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
				return;
			}
			GorillaPaintbrawlManager val2 = (GorillaPaintbrawlManager)GorillaGameManager.instance;
			val2.playerLives[playerFromVRRig.ActorNumber] = 4;
		}
	}

	public static void PaintbrawlReviveSelf()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			return;
		}
		GorillaPaintbrawlManager val = (GorillaPaintbrawlManager)GorillaGameManager.instance;
		val.playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = 4;
	}

	public static void PaintbrawlReviveAll()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			return;
		}
		GorillaPaintbrawlManager val = (GorillaPaintbrawlManager)GorillaGameManager.instance;
		Player[] playerList = PhotonNetwork.PlayerList;
		foreach (Player val2 in playerList)
		{
			val.playerLives[val2.ActorNumber] = 4;
		}
	}

	public static void PaintbrawlNoDelay()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			return;
		}
		GorillaPaintbrawlManager val = (GorillaPaintbrawlManager)GorillaGameManager.instance;
		val.hitCooldown = 0f;
		val.tagCoolDown = 0f;
		val.stunGracePeriod = 0f;
	}

	public static void DisablePaintbrawlNoDelay()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		GorillaPaintbrawlManager val = (GorillaPaintbrawlManager)GorillaGameManager.instance;
		val.hitCooldown = 3f;
		val.tagCoolDown = 5f;
		val.stunGracePeriod = 2f;
	}

	public static void PaintbrawlGodMode()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			return;
		}
		GorillaPaintbrawlManager val = (GorillaPaintbrawlManager)GorillaGameManager.instance;
		val.playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = 4;
		GTPlayer.Instance.disableMovement = false;
	}
}
