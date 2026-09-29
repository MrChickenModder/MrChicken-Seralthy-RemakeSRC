using System;
using GorillaTagScripts;
using HarmonyLib;

namespace Seralyth.Patches.Menu;

public class SubscriptionPatches
{
	[HarmonyPatch(typeof(SubscriptionManager), "IsLocalSubscribed")]
	public class IsLocalSubscribed
	{
		private static bool Prefix(ref bool __result)
		{
			if (!enabled)
			{
				return true;
			}
			__result = true;
			return false;
		}
	}

	[HarmonyPatch(typeof(SubscriptionManager), "LocalSubscriptionStatus")]
	public class LocalSubscriptionStatus
	{
		private static bool Prefix(ref SubscriptionStatus __result)
		{
			if (!enabled)
			{
				return true;
			}
			__result = (SubscriptionStatus)0;
			return false;
		}
	}

	[HarmonyPatch(typeof(SubscriptionManager), "LocalSubscriptionDetails")]
	public class LocalSubscriptionDetails
	{
		private static bool Prefix(ref SubscriptionDetails __result)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			if (!enabled)
			{
				return true;
			}
			__result = new SubscriptionDetails
			{
				active = true,
				daysAccrued = int.MaxValue,
				subscriptionFeatureSettings = new bool[2] { true, true },
				tier = int.MaxValue,
				subscriptionActiveUntilDate = DateTime.MaxValue,
				autoRenew = true,
				autoRenewMonths = int.MaxValue
			};
			return false;
		}
	}

	public static bool enabled;
}
