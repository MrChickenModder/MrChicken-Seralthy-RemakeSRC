using System;
using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using Liv.Lck.Telemetry;
using PlayFab;
using PlayFab.EventsModels;

namespace Seralyth.Patches.Safety;

public class TelemetryPatches
{
	[PatchHandler.PatchOnAwake]
	[HarmonyPatch(typeof(GorillaTelemetry), "EnqueueTelemetryEvent")]
	public class EnqueueTelemetryEvent
	{
		private static bool Prefix(string eventName, object content, [CanBeNull] string[] customTags = null)
		{
			return !enabled;
		}
	}

	[PatchHandler.PatchOnAwake]
	[HarmonyPatch(typeof(GorillaTelemetry), "FlushMothershipTelemetry")]
	public class FlushMothershipTelemetry
	{
		private static bool Prefix()
		{
			return !enabled;
		}
	}

	[PatchHandler.PatchOnAwake]
	[HarmonyPatch(typeof(LckTelemetryClient), "SendTelemetry")]
	public class SendTelemetry
	{
		private static bool Prefix(LckTelemetryEvent lckTelemetryEvent)
		{
			return !enabled;
		}
	}

	[PatchHandler.PatchOnAwake]
	[HarmonyPatch(typeof(PlayFabEventsAPI), "WriteTelemetryEvents")]
	public class WriteTelemetryEvents
	{
		private static bool Prefix(WriteEventsRequest request, Action<WriteEventsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			return !enabled;
		}
	}

	public static bool enabled = true;
}
