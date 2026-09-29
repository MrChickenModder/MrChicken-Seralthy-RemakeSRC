using System;
using HarmonyLib;
using Seralyth.Managers;
using Seralyth.Menu;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(VRRig), "SerializeReadShared")]
public class PlayerSerializePatch
{
	public static bool stopSerialization;

	public static float? delay;

	public static event Action<VRRig> OnPlayerSerialize;

	public static bool Prefix(VRRig __instance, InputStruct data)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (stopSerialization)
		{
			return false;
		}
		if (delay.HasValue)
		{
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(Main.SerializationDelay(delegate
			{
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				float value = delay.Value;
				delay = null;
				try
				{
					__instance.SerializeReadShared(data);
				}
				catch
				{
				}
				delay = value;
			}, delay.Value));
			return false;
		}
		return true;
	}

	public static void Postfix(VRRig __instance, InputStruct data)
	{
		PlayerSerializePatch.OnPlayerSerialize?.Invoke(__instance);
	}
}
