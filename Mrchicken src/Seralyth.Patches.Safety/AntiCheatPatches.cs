using HarmonyLib;
using Photon.Pun;
using Seralyth.Managers;
using Seralyth.Mods;
using UnityEngine;

namespace Seralyth.Patches.Safety;

public class AntiCheatPatches
{
	[PatchHandler.SecurityPatch]
	[HarmonyPatch(typeof(MonkeAgent), "SendReport")]
	public class SendReportPatch
	{
		public static bool AntiCheatSelf;

		public static bool AntiCheatAll;

		public static bool AntiCheatReasonHide;

		public static bool AntiACReport;

		private static bool Prefix(string susReason, string susId, string susNick)
		{
			if (susReason.ToLower() == "empty rig")
			{
				return false;
			}
			if (AntiCheatSelf || AntiCheatAll)
			{
				if (susId == PhotonNetwork.LocalPlayer.UserId)
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=green>ANTI-CHEAT</color><color=grey>]</color> You have been reported for " + (AntiCheatReasonHide ? "hidden reason" : susReason) + ".");
				}
				else if (AntiCheatAll)
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=green>ANTI-CHEAT</color><color=grey>]</color> " + susNick + " was reported for " + (AntiCheatReasonHide ? "hidden reason" : susReason) + ".");
				}
			}
			if (AntiACReport)
			{
				Seralyth.Mods.Safety.AntiReportFRT(PhotonNetwork.LocalPlayer);
				NotificationManager.ClearAllNotifications();
				NotificationManager.SendNotification("<color=grey>[</color><color=purple>ANTI-REPORT</color><color=grey>]</color> The anti cheat attempted to report you, you have been disconnected.");
			}
			return false;
		}
	}

	[PatchHandler.SecurityPatch]
	[HarmonyPatch(typeof(MonkeAgent), "CloseInvalidRoom")]
	public class NoCloseInvalidRoom
	{
		private static bool Prefix()
		{
			return false;
		}
	}

	[PatchHandler.SecurityPatch]
	[HarmonyPatch(typeof(MonkeAgent), "CheckReports")]
	public class NoCheckReports
	{
		private static bool Prefix()
		{
			return false;
		}
	}

	[PatchHandler.SecurityPatch]
	[HarmonyPatch(typeof(MonkeAgent), "DispatchReport")]
	public class NoDispatchReport
	{
		private static bool Prefix()
		{
			return false;
		}
	}

	[PatchHandler.SecurityPatch]
	[HarmonyPatch(typeof(MonkeAgent), "GetRPCCallTracker")]
	internal class NoGetRPCCallTracker
	{
		private static bool Prefix()
		{
			return false;
		}
	}

	[PatchHandler.SecurityPatch]
	[HarmonyPatch(typeof(MonkeAgent), "LogErrorCount")]
	public class NoLogErrorCount
	{
		private static bool Prefix(string logString, string stackTrace, LogType type)
		{
			return false;
		}
	}

	[PatchHandler.SecurityPatch]
	[HarmonyPatch(/*Could not decode attribute arguments.*/)]
	public class NoQuitDelay
	{
		private static bool Prefix()
		{
			return false;
		}
	}

	[PatchHandler.SecurityPatch]
	[HarmonyPatch(typeof(GorillaGameManager), "ForceStopGame_DisconnectAndDestroy")]
	public class NoQuitOnBan
	{
		private static bool Prefix()
		{
			return false;
		}
	}

	[PatchHandler.SecurityPatch]
	[HarmonyPatch(typeof(MonkeAgent), "ShouldDisconnectFromRoom")]
	public class NoShouldDisconnectFromRoom
	{
		private static bool Prefix()
		{
			return false;
		}
	}

	[PatchHandler.SecurityPatch]
	[HarmonyPatch(typeof(GorillaNetworkPublicTestsJoin), "GracePeriod")]
	public class GracePeriodPatch1
	{
		private static bool Prefix()
		{
			return false;
		}
	}

	[PatchHandler.SecurityPatch]
	[HarmonyPatch(typeof(GorillaNetworkPublicTestJoin2), "GracePeriod")]
	public class GracePeriodPatch2
	{
		private static bool Prefix()
		{
			return false;
		}
	}
}
