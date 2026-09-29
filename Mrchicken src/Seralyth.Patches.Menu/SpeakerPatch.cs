using System.Collections.Generic;
using HarmonyLib;
using Photon.Voice;
using Photon.Voice.Unity;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(Speaker), "OnAudioFrame")]
public class SpeakerPatch
{
	public static bool enabled;

	public static Speaker targetSpeaker;

	public static List<float> SampleQueue = new List<float>();

	public static readonly object locked = new object();

	private static void Postfix(Speaker __instance, FrameOut<float> frame)
	{
		if (!enabled || (Object)(object)targetSpeaker == (Object)null || (Object)(object)__instance != (Object)(object)targetSpeaker)
		{
			return;
		}
		float[] buf = frame.Buf;
		if (buf == null || buf.Length == 0)
		{
			return;
		}
		lock (locked)
		{
			SampleQueue.AddRange(buf);
			if (SampleQueue.Count > ((VoiceInfo)(ref targetSpeaker.RemoteVoiceLink.Info)).SamplingRate)
			{
				SampleQueue.RemoveRange(0, SampleQueue.Count - ((VoiceInfo)(ref targetSpeaker.RemoteVoiceLink.Info)).SamplingRate);
			}
		}
	}
}
