using System;
using System.Collections.Generic;
using HarmonyLib;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Internal;
using Seralyth.Utilities;
using UnityEngine;

namespace Seralyth.Patches.Safety;

public class PlayFabTelemetryPatches
{
	[PatchHandler.PatchOnAwake]
	[HarmonyPatch(typeof(PlayFabDeviceUtil), "SendDeviceInfoToPlayFab")]
	public class PlayfabUtil01
	{
		private static bool Prefix()
		{
			return false;
		}
	}

	[PatchHandler.PatchOnAwake]
	[HarmonyPatch(typeof(PlayFabClientInstanceAPI), "ReportDeviceInfo")]
	public class PlayfabUtil02
	{
		private static bool Prefix()
		{
			return false;
		}
	}

	[PatchHandler.PatchOnAwake]
	[HarmonyPatch(typeof(PlayFabClientAPI), "ReportDeviceInfo")]
	public class PlayfabUtil03
	{
		private static bool Prefix()
		{
			return false;
		}
	}

	[HarmonyPatch(typeof(PlayFabDeviceUtil), "GetAdvertIdFromUnity")]
	public class PlayfabUtil04
	{
		private static bool Prefix()
		{
			return false;
		}
	}

	[PatchHandler.PatchOnAwake]
	[HarmonyPatch(typeof(PlayFabClientAPI), "AttributeInstall")]
	public class PlayfabUtil05
	{
		private static bool Prefix()
		{
			return false;
		}
	}

	[PatchHandler.PatchOnAwake]
	[HarmonyPatch(typeof(PlayFabHttp), "InitializeScreenTimeTracker")]
	public class PlayfabUtil06
	{
		private static bool Prefix()
		{
			return false;
		}
	}

	[PatchHandler.PatchOnAwake]
	[HarmonyPatch(typeof(PlayFabClientAPI), "UpdateUserTitleDisplayName")]
	public class DisplayNamePatch
	{
		public static void Prefix(ref UpdateUserTitleDisplayNameRequest request, Action<UpdateUserTitleDisplayNameResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			request.DisplayName = RandomUtilities.RandomString(Random.Range(3, 12));
		}
	}
}
