using System;
using System.Collections.Generic;
using System.Linq;
using GorillaGameModes;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Realtime;
using Seralyth.Menu;
using Seralyth.Mods;
using Seralyth.Utilities;
using UnityEngine;

namespace Seralyth.Extensions;

public static class VRRigExtensions
{
	private static readonly List<VRRig> _rigs = new List<VRRig>();

	private static int _lastFrame = -1;

	private static readonly object _lock = new object();

	public static List<VRRig> ActiveRigs
	{
		get
		{
			int frameCount = Time.frameCount;
			if (frameCount == _lastFrame)
			{
				return _rigs;
			}
			lock (_lock)
			{
				if (frameCount == _lastFrame)
				{
					return _rigs;
				}
				_lastFrame = frameCount;
				_rigs.Clear();
				foreach (VRRig activeRig in VRRigCache.ActiveRigs)
				{
					if ((Object)(object)activeRig != (Object)null && !Settings.Blocked.Contains(activeRig))
					{
						_rigs.Add(activeRig);
					}
				}
			}
			return _rigs;
		}
	}

	public static bool IsLocal(this VRRig rig)
	{
		return (Object)(object)rig != (Object)null && (rig.isLocal || ((Object)(object)Main.GhostRig != (Object)null && (Object)(object)rig == (Object)(object)Main.GhostRig));
	}

	public static bool IsTagged(this VRRig rig)
	{
		if ((Object)(object)rig == (Object)null)
		{
			return false;
		}
		List<NetPlayer> list = GameModeUtilities.InfectedList();
		NetPlayer player = rig.GetPlayer();
		return list.Contains(player);
	}

	public static bool IsSteam(this VRRig rig)
	{
		return rig.GetPlatform() != "Standalone";
	}

	public static bool IsKIDRestricted(this VRRig rig)
	{
		return !rig.IsMicEnabled && rig.GetName().ToLower().StartsWith("gorilla");
	}

	public static string GetPlatform(this VRRig rig)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		string text = rig.Cosmetics();
		if (text.Contains("S. FIRST LOGIN"))
		{
			num++;
		}
		if (text.Contains("FIRST LOGIN") || ((Dictionary<object, object>)(object)rig.GetPhotonPlayer().CustomProperties).Count >= 2)
		{
			num2++;
		}
		if (rig.currentRankedSubTierPC > 0)
		{
			num2++;
		}
		else if (rig.currentRankedSubTierQuest > 0)
		{
			num3++;
		}
		if (num > num2 && num > num3)
		{
			return "Steam";
		}
		if (num2 > num && num2 > num3)
		{
			return "PC";
		}
		if (num3 > num && num3 > num2)
		{
			return "Standalone";
		}
		Player photonPlayer = rig.GetPhotonPlayer();
		if (photonPlayer != null)
		{
			if (((Dictionary<object, object>)(object)photonPlayer.CustomProperties).Count >= 2)
			{
				return "Steam";
			}
			string userId = photonPlayer.UserId;
			if (!string.IsNullOrEmpty(userId) && userId.All(char.IsDigit) && userId.Length >= 16)
			{
				return "Steam";
			}
		}
		return "Standalone";
	}

	public static string GetCreationDate(this VRRig rig, Action<string> onTranslated = null, string format = "MMMM dd, yyyy h:mm tt")
	{
		return RigUtilities.GetCreationDate(rig.Creator.UserId, onTranslated, format);
	}

	public static Color GetColor(this VRRig rig)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between Unknown and I4
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		if (Buttons.GetIndex("Follow Player Colors").enabled)
		{
			return rig.playerColor;
		}
		if ((int)rig.bodyRenderer.cosmeticBodyType == 2)
		{
			return Color.green;
		}
		switch (rig.setMatIndex)
		{
		case 1:
			return Color.red;
		case 2:
		case 11:
			return Color32.op_Implicit(new Color32(byte.MaxValue, (byte)128, (byte)0, byte.MaxValue));
		case 3:
		case 7:
			return Color.blue;
		case 12:
			return Color.green;
		default:
			return rig.playerColor;
		}
	}

	public static bool Active(this VRRig rig)
	{
		return (Object)(object)rig != (Object)null && VRRigCache.ActiveRigs.Contains(rig);
	}

	public static float Distance(this VRRig rig, Vector3 position)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Distance(((Component)rig).transform.position, position);
	}

	public static float Distance(this VRRig rig, VRRig otherRig)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return rig.Distance(((Component)otherRig).transform.position);
	}

	public static float Distance(this VRRig rig)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return rig.Distance(((Component)GorillaTagger.Instance.bodyCollider).transform.position);
	}

	public static VRRig GetClosest(this VRRig rig)
	{
		return VRRigCache.ActiveRigs.Where((VRRig targetRig) => (Object)(object)targetRig != (Object)null && (Object)(object)targetRig != (Object)(object)rig).OrderBy(rig.Distance).FirstOrDefault();
	}

	public static int GetPing(this VRRig rig)
	{
		int value;
		return Main.playerPing.TryGetValue(rig, out value) ? value : PhotonNetwork.GetPing();
	}

	public static int GetTruePing(this VRRig rig)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		double a = Math.Abs((rig.velocityHistoryList[0].time - PhotonNetwork.Time) * 1000.0);
		return (int)Math.Clamp(Math.Round(a), 0.0, 2147483647.0);
	}

	public static string GetName(this VRRig rig)
	{
		NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(rig);
		return ((playerFromVRRig != null) ? playerFromVRRig.NickName : null) ?? "null";
	}

	public static NetPlayer GetPlayer(this VRRig rig)
	{
		return RigUtilities.GetPlayerFromVRRig(rig);
	}

	public static Player GetPhotonPlayer(this VRRig rig)
	{
		return RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(rig));
	}

	public static NetworkView GetNetView(this VRRig rig)
	{
		return rig.netView;
	}

	public static PhotonView GetPhotonView(this VRRig rig)
	{
		return rig.netView.GetView;
	}

	public static ProjectileWeapon GetSlingshot(this VRRig rig)
	{
		return rig.projectileWeapon;
	}

	public static float[] GetSpeed(this VRRig rig)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		NetPlayer player = rig.GetPlayer();
		GameModeType val = GorillaGameManager.instance.GameType();
		GameModeType val2 = val;
		if ((int)val2 == 1 || (int)val2 == 5 || val2 - 9 <= 1)
		{
			GorillaTagManager val3 = (GorillaTagManager)GorillaGameManager.instance;
			return (!val3.isCurrentlyTag) ? ((!val3.currentInfected.Contains(player)) ? new float[2]
			{
				val3.InterpolatedNoobJumpSpeed(val3.currentInfected.Count),
				val3.InterpolatedNoobJumpMultiplier(val3.currentInfected.Count)
			} : new float[2]
			{
				val3.InterpolatedInfectedJumpSpeed(val3.currentInfected.Count),
				val3.InterpolatedInfectedJumpMultiplier(val3.currentInfected.Count)
			}) : ((player != val3.currentIt) ? new float[2]
			{
				((GorillaGameManager)val3).slowJumpLimit,
				((GorillaGameManager)val3).slowJumpMultiplier
			} : new float[2]
			{
				((GorillaGameManager)val3).fastJumpLimit,
				((GorillaGameManager)val3).fastJumpMultiplier
			});
		}
		return new float[2] { 6.5f, 1.1f };
	}

	public static float GetMaxSpeed(this VRRig rig)
	{
		return rig.GetSpeed()[0];
	}

	public static float GetSpeedMultiplier(this VRRig rig)
	{
		return rig.GetSpeed()[1];
	}

	public static string Cosmetics(this VRRig rig)
	{
		return StringUtils.Concat((IEnumerable<string>)rig._playerOwnedCosmetics);
	}

	public static bool IsLeftHandGrabbable(this VRRig rig)
	{
		return (Object)(object)rig != (Object)null && rig.leftHandLink.CanBeGrabbed();
	}

	public static bool IsRightHandGrabbable(this VRRig rig)
	{
		return (Object)(object)rig != (Object)null && rig.rightHandLink.CanBeGrabbed();
	}

	public static bool IsVIMSubscriber(this VRRig rig)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return SubscriptionManager.Instance.subData[rig.GetPlayer()].active;
	}

	public static bool IsBeingTouched(this VRRig rig, VRRig otherRig = null, float distance = 0.35f)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)rig == (Object)null)
		{
			return false;
		}
		VRRig val = otherRig ?? VRRig.LocalRig;
		return rig.Distance(val.leftHand.rigTarget.position) <= distance || rig.Distance(val.rightHand.rigTarget.position) <= distance;
	}

	public static bool IsNear(this VRRig rig, VRRig otherRig = null, float distance = 3f)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)rig == (Object)null)
		{
			return false;
		}
		VRRig val = otherRig ?? VRRig.LocalRig;
		return rig.Distance(((Component)val).transform.position) <= distance;
	}

	public static bool IsTouchingMe(this VRRig rig, float distance = 0.35f)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)rig == (Object)null)
		{
			return false;
		}
		return rig.Distance(VRRig.LocalRig.leftHand.rigTarget.position) <= distance || rig.Distance(VRRig.LocalRig.rightHand.rigTarget.position) <= distance;
	}
}
