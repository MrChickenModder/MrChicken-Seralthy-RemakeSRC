using System;
using HarmonyLib;
using Photon.Voice;
using Photon.Voice.Unity;
using Seralyth.Managers;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(Recorder))]
public class RecorderPatch
{
	public static bool enabled = true;

	[HarmonyPatch(/*Could not decode attribute arguments.*/)]
	public static bool Prefix(ref InputSourceType __result)
	{
		if (enabled)
		{
			__result = (InputSourceType)2;
			return false;
		}
		return true;
	}

	[HarmonyPatch(/*Could not decode attribute arguments.*/)]
	public static bool Prefix(ref Func<IAudioDesc> __result)
	{
		if (enabled)
		{
			__result = () => (IAudioDesc)(object)VoiceManager.Get();
			return false;
		}
		return true;
	}

	[HarmonyPatch("CreateLocalVoiceAudioAndSource")]
	public static bool Prefix(Recorder __instance)
	{
		if (enabled)
		{
			__instance.SourceType = (InputSourceType)2;
			__instance.InputFactory = () => (IAudioDesc)(object)VoiceManager.Get();
		}
		return true;
	}
}
