using GorillaNetworking;
using HarmonyLib;
using Seralyth.Classes.Menu;
using Seralyth.Managers;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(GorillaComputer), "GeneralFailureMessage")]
public class FailurePatch
{
	public static void Prefix(string failMessage)
	{
		if (ServerData.ServerDataEnabled && failMessage.ToLower().Contains("your account"))
		{
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(ServerData.ReportFailureMessage(failMessage));
		}
	}
}
