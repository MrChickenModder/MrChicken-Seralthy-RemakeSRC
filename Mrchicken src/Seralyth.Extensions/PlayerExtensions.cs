using System.Linq;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using Seralyth.Utilities;
using UnityEngine;

namespace Seralyth.Extensions;

public static class PlayerExtensions
{
	public static Player GetPlayer(this NetPlayer self)
	{
		return RigUtilities.NetPlayerToPlayer(self);
	}

	public static VRRig VRRig(this NetPlayer self)
	{
		return RigUtilities.GetVRRigFromPlayer(self);
	}

	public static bool InRoom(this NetPlayer self)
	{
		return NetworkSystem.Instance.AllNetPlayers.Contains(self);
	}

	public static Hashtable GetCustomProperties(this NetPlayer self)
	{
		return self.GetPlayer().CustomProperties;
	}

	public static VRRig VRRig(this Player self)
	{
		return RigUtilities.GetVRRigFromPlayer(NetPlayer.op_Implicit(self));
	}

	public static bool InRoom(this Player self)
	{
		return PhotonNetwork.PlayerList.Contains(self);
	}

	public static bool IsGrounded(this GorillaTagger tagger, float maxDistance = 0.15f)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		return Physics.Raycast(((Component)tagger.bodyCollider).transform.position - new Vector3(0f, 0.2f, 0f), Vector3.down, maxDistance, LayerMask.op_Implicit(GTPlayer.Instance.locomotionEnabledLayers));
	}
}
