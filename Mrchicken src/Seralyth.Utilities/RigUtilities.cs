using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using Seralyth.Extensions;
using UnityEngine;

namespace Seralyth.Utilities;

public class RigUtilities
{
	private static VRRig rigTarget;

	private static float rigTargetChange;

	public static readonly Dictionary<string, float> waitingForCreationDate = new Dictionary<string, float>();

	public static readonly Dictionary<string, string> creationDateCache = new Dictionary<string, string>();

	public static VRRig GetVRRigFromPlayer(NetPlayer p)
	{
		return GorillaGameManager.StaticFindRigForPlayer(p);
	}

	public static NetPlayer GetPlayerFromVRRig(VRRig p)
	{
		return p.Creator ?? NetworkSystem.Instance.GetPlayer(NetworkSystem.Instance.GetOwningPlayerID(((Component)p.rigSerializer).gameObject));
	}

	public static NetPlayer GetPlayerFromID(string id)
	{
		return NetPlayer.op_Implicit(((IEnumerable<Player>)PhotonNetwork.PlayerList).FirstOrDefault((Func<Player, bool>)((Player player) => player.UserId == id)));
	}

	public static Player NetPlayerToPlayer(NetPlayer p)
	{
		return p.GetPlayerRef();
	}

	public static Player GetRandomPlayer(bool includeSelf)
	{
		return includeSelf ? PhotonNetwork.PlayerList[Random.Range(0, PhotonNetwork.PlayerList.Length)] : PhotonNetwork.PlayerListOthers[Random.Range(0, PhotonNetwork.PlayerListOthers.Length)];
	}

	public static VRRig GetTargetPlayer(float targetChangeDelay = 1f)
	{
		if (!(Time.time > rigTargetChange) && rigTarget.Active())
		{
			return rigTarget;
		}
		rigTargetChange = Time.time + targetChangeDelay;
		rigTarget = GetRandomVRRig(includeSelf: false);
		return rigTarget;
	}

	public static VRRig GetRandomVRRig(bool includeSelf)
	{
		return GetVRRigFromPlayer(NetPlayer.op_Implicit(GetRandomPlayer(includeSelf)));
	}

	public static NetworkView GetNetworkViewFromVRRig(VRRig p)
	{
		return p.netView;
	}

	public static PhotonView GetPhotonViewFromVRRig(VRRig p)
	{
		return GetNetworkViewFromVRRig(p).GetView;
	}

	public static VRRig GetClosestVRRig()
	{
		return VRRig.LocalRig.GetClosest();
	}

	public static string GetCreationDate(string input, Action<string> onTranslated = null, string format = "MMMM dd, yyyy h:mm tt")
	{
		if (creationDateCache.TryGetValue(input, out var value))
		{
			return value;
		}
		if (!waitingForCreationDate.ContainsKey(input))
		{
			waitingForCreationDate[input] = Time.time + 10f;
			GetCreationCoroutine(input, onTranslated, format);
		}
		else
		{
			if (!(Time.time > waitingForCreationDate[input]))
			{
				return "Loading...";
			}
			waitingForCreationDate[input] = Time.time + 10f;
			GetCreationCoroutine(input, onTranslated, format);
		}
		return "Loading...";
	}

	public static void GetCreationCoroutine(string userId, Action<string> onTranslated = null, string format = "MMMM dd, yyyy h:mm tt")
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		if (creationDateCache.TryGetValue(userId, out var value))
		{
			onTranslated?.Invoke(value);
			return;
		}
		PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest
		{
			PlayFabId = userId
		}, (Action<GetAccountInfoResult>)delegate(GetAccountInfoResult result)
		{
			string text = result.AccountInfo.Created.ToString(format);
			creationDateCache[userId] = text;
			onTranslated?.Invoke(text);
		}, (Action<PlayFabError>)delegate
		{
			creationDateCache[userId] = "Error";
			onTranslated?.Invoke("Error");
		}, (object)null, (Dictionary<string, string>)null);
	}
}
