using System;
using HarmonyLib;
using Photon.Pun;
using Seralyth.Managers;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(PhotonNetwork), "RunViewUpdate")]
public class SerializePatch
{
	public static Func<bool> OverrideSerialization;

	public static event Action OnSerialize;

	public static bool Prefix()
	{
		if (!PhotonNetwork.InRoom)
		{
			return true;
		}
		try
		{
			SerializePatch.OnSerialize?.Invoke();
		}
		catch (Exception arg)
		{
			LogManager.LogError($"Error in SerializePatch.OnSerialize: {arg}");
		}
		if (OverrideSerialization == null)
		{
			return true;
		}
		try
		{
			return OverrideSerialization();
		}
		catch (Exception arg2)
		{
			LogManager.LogError($"Error in SerializePatch.OverrideSerialization: {arg2}");
			return false;
		}
	}
}
