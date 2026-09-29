using System.Collections.Generic;
using ExitGames.Client.Photon;
using GorillaExtensions;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Seralyth.Patches.Menu;

public class AntiCrashPatches
{
	[HarmonyPatch(typeof(VRRig), "DroppedByPlayer")]
	public class DroppedByPlayer
	{
		public static bool enabled;

		public static bool Prefix(VRRig __instance, VRRig grabbedByRig, Vector3 throwVelocity)
		{
			int result;
			if (enabled && __instance.isLocal)
			{
				float num = 10000f;
				result = (GTExt.IsValid(ref throwVelocity, ref num) ? 1 : 0);
			}
			else
			{
				result = 1;
			}
			return (byte)result != 0;
		}
	}

	[HarmonyPatch(typeof(VRRig), "RequestCosmetics")]
	public class RequestCosmetics
	{
		private static readonly List<float> callTimestamps = new List<float>();

		public static bool Prefix(VRRig __instance)
		{
			if (enabled && __instance.isLocal)
			{
				callTimestamps.Add(Time.time);
				callTimestamps.RemoveAll((float t) => Time.time - t > 1f);
				return callTimestamps.Count < 15;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(VRRig), "RequestMaterialColor")]
	public class RequestMaterialColor
	{
		private static readonly List<float> callTimestamps = new List<float>();

		public static bool Prefix(VRRig __instance)
		{
			if (enabled && __instance.isLocal)
			{
				callTimestamps.Add(Time.time);
				callTimestamps.RemoveAll((float t) => Time.time - t > 1f);
				return callTimestamps.Count < 15;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(DeployedChild), "Deploy")]
	public class Deploy
	{
		public static void Postfix(DeployedChild __instance, DeployableObject parent, Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel, bool isRemote = false)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			if (enabled)
			{
				__instance._rigidbody.linearVelocity = GTExt.ClampMagnitudeSafe(__instance._rigidbody.linearVelocity, 100f);
			}
		}
	}

	[HarmonyPatch(typeof(LuauVm), "OnEvent")]
	public class OnEvent
	{
		public static bool Prefix(EventData eventData)
		{
			if (enabled)
			{
				if (eventData.Code != 180)
				{
					return false;
				}
				Player player = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(eventData.Sender, false);
				object[] array = ((eventData.CustomData == null) ? new object[0] : ((object[])eventData.CustomData));
				string text = ((array.Length != 0) ? ((string)array[0]) : "");
				if (player != PhotonNetwork.LocalPlayer && array[1] is double num && num == (double)PhotonNetwork.LocalPlayer.ActorNumber && text == "leaveGame")
				{
					return false;
				}
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(RoomSystem), "SearchForShuttle")]
	public class SearchForShuttle
	{
		public static bool Prefix(object[] shuffleData, PhotonMessageInfoWrapped info)
		{
			return !enabled;
		}
	}

	[HarmonyPatch(typeof(RoomInfo), "InternalCacheProperties")]
	public class InternalCacheProperties
	{
		public static bool Prefix(RoomInfo __instance, Hashtable propertiesToCache)
		{
			return __instance.masterClientId != PhotonNetwork.LocalPlayer.ActorNumber || ((Dictionary<object, object>)(object)propertiesToCache).Count != 1 || !propertiesToCache.ContainsKey((byte)248) || !enabled;
		}
	}

	[HarmonyPatch(typeof(GameEntityManager), "JoinWithItemsRPC")]
	public class JoinWithItemsRPC
	{
		public static bool Prefix(GameEntityManager __instance, byte[] stateData, int[] netIds, int joiningActorNum, PhotonMessageInfo info)
		{
			return stateData.Length <= 255;
		}
	}

	[HarmonyPatch(typeof(GorillaWrappedSerializer), "FailedToSpawn")]
	public class FailedToSpawn
	{
		public static bool Prefix(GorillaWrappedSerializer __instance)
		{
			if (enabled)
			{
				((Component)__instance).gameObject.SetActive(false);
				return false;
			}
			return true;
		}
	}

	public static bool enabled;
}
