using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;
using PlayFab;
using PlayFab.CloudScriptModels;
using PlayFab.Internal;
using PlayFab.Json;
using Seralyth.Managers;
using UnityEngine;

namespace Seralyth.Patches.Menu;

public class BanPatches
{
	[HarmonyPatch(typeof(GorillaServer), "CheckForBadName")]
	public class AutoBanPlayfabFunction
	{
		public static bool Prefix(CheckForBadNameRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			//IL_0038: Expected O, but got Unknown
			if (enabled)
			{
				if (successCallback != null)
				{
					ExecuteFunctionResult val = new ExecuteFunctionResult();
					JsonObject val2 = new JsonObject();
					val2.Add("result", (object)0);
					val.FunctionResult = (object)val2;
					successCallback(val);
				}
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(GorillaComputer), "CheckAutoBanListForName")]
	public class CheckAutoBanListForName
	{
		public static bool Prefix(string nameToCheck, ref bool __result)
		{
			if (enabled)
			{
				__result = true;
				return false;
			}
			return true;
		}

		public static bool CheckBanList(string nameToCheck)
		{
			nameToCheck = nameToCheck.ToLower();
			nameToCheck = new string(Array.FindAll(nameToCheck.ToCharArray(), (char c) => char.IsLetterOrDigit(c)));
			string[] anywhereTwoWeek = ((GorillaComputer)GorillaComputer.instance).anywhereTwoWeek;
			foreach (string value in anywhereTwoWeek)
			{
				if (nameToCheck.IndexOf(value) >= 0)
				{
					return false;
				}
			}
			string[] anywhereOneWeek = ((GorillaComputer)GorillaComputer.instance).anywhereOneWeek;
			foreach (string value2 in anywhereOneWeek)
			{
				if (nameToCheck.IndexOf(value2) >= 0 && !nameToCheck.Contains("fagol"))
				{
					return false;
				}
			}
			string[] exactOneWeek = ((GorillaComputer)GorillaComputer.instance).exactOneWeek;
			for (int num3 = 0; num3 < exactOneWeek.Length; num3++)
			{
				if (exactOneWeek[num3] == nameToCheck)
				{
					return false;
				}
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(PlayFabUnityHttp), "MakeApiCall")]
	public class AntiBanCrash1
	{
		public static bool enabled;

		public static bool Prefix(object reqContainerObj)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			if (!enabled || reqContainerObj == null)
			{
				return true;
			}
			CallRequestContainer val = (CallRequestContainer)reqContainerObj;
			Action<PlayFabError> errorCallback;
			PlayFabError fakeError;
			if (val.ErrorCallback != null)
			{
				errorCallback = val.ErrorCallback;
				fakeError = null;
				val.ErrorCallback = overrideError;
				if (!PhotonNetwork.InRoom)
				{
					Task.Run(async delegate
					{
						while ((Object)(object)GorillaComputer.instance == (Object)null)
						{
							await Task.Delay(16);
						}
						((GorillaComputer)GorillaComputer.instance).GeneralFailureMessage(fakeError.ErrorMessage);
					});
				}
			}
			return true;
			void overrideError(PlayFabError error)
			{
				//IL_0164: Unknown result type (might be due to invalid IL or missing references)
				//IL_0169: Unknown result type (might be due to invalid IL or missing references)
				//IL_016f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0174: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00de: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_01bc: Expected O, but got Unknown
				//IL_014f: Unknown result type (might be due to invalid IL or missing references)
				//IL_015f: Expected O, but got Unknown
				if (error.ErrorMessage.ToLower().Contains("ban") || error.ErrorMessage.ToLower().Contains("banned") || error.ErrorMessage.ToLower().Contains("suspended") || error.ErrorMessage.ToLower().Contains("suspension"))
				{
					if (error.ErrorMessage.ToLower().Contains("this ip"))
					{
						NotificationManager.SendNotification("<color=grey>[</color><color=red>ANTI-BAN</color><color=grey>]</color> Your IP address is currently banned.");
					}
					else
					{
						NotificationManager.SendNotification("<color=grey>[</color><color=red>ANTI-BAN</color><color=grey>]</color> Your account is currently banned.");
					}
					Dictionary<string, List<string>>.Enumerator enumerator = error.ErrorDetails.GetEnumerator();
					if (enumerator.Current.Value[0] != "Indefinite")
					{
						fakeError = new PlayFabError
						{
							Error = (PlayFabErrorCode)1039,
							ErrorMessage = string.Format("Your {0} has been banned. Hours left: {1}", error.ErrorMessage.ToLower().Contains("this ip") ? "IP address" : "account", (int)((DateTime.Parse(enumerator.Current.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)),
							ErrorDetails = new Dictionary<string, List<string>>()
						};
					}
					else
					{
						fakeError = new PlayFabError
						{
							Error = (PlayFabErrorCode)1039,
							ErrorMessage = "Your " + (error.ErrorMessage.ToLower().Contains("this ip") ? "IP address" : "account") + " has been banned indefinitely.",
							ErrorDetails = new Dictionary<string, List<string>>()
						};
					}
					errorCallback?.Invoke(fakeError);
				}
				else
				{
					errorCallback?.Invoke(error);
				}
			}
		}
	}

	[HarmonyPatch(typeof(PlayFabWebRequest), "MakeApiCall")]
	public class AntiBanCrash2
	{
		public static bool Prefix(object reqContainerObj)
		{
			return AntiBanCrash1.Prefix(reqContainerObj);
		}
	}

	public static bool enabled;
}
