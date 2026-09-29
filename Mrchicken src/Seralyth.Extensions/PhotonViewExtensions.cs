using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Seralyth.Extensions;

public static class PhotonViewExtensions
{
	public static void RPC(this PhotonView view, string methodName, object target, params object[] parameters)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (!(target is RpcTarget val))
		{
			Player val2 = (Player)((target is Player) ? target : null);
			if (val2 != null)
			{
				view.RPC(methodName, val2, parameters);
			}
		}
		else
		{
			view.RPC(methodName, val, parameters);
		}
	}

	public static bool RPC(this PhotonView photonView, string methodName, RaiseEventOptions options, params object[] parameters)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Invalid comparison between Unknown and I4
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Invalid comparison between Unknown and I4
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)photonView != (Object)null && parameters != null && !string.IsNullOrEmpty(methodName))
		{
			Hashtable val = new Hashtable();
			val.Add((byte)0, (object)photonView.ViewID);
			val.Add((byte)2, (object)PhotonNetwork.ServerTimestamp);
			val.Add((byte)3, (object)methodName);
			val.Add((byte)4, (object)parameters);
			Hashtable val2 = val;
			if (photonView.Prefix > 0)
			{
				val2[(byte)1] = (short)photonView.Prefix;
			}
			if (PhotonNetwork.PhotonServerSettings.RpcList.Contains(methodName))
			{
				val2[(byte)5] = (byte)PhotonNetwork.PhotonServerSettings.RpcList.IndexOf(methodName);
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
}
