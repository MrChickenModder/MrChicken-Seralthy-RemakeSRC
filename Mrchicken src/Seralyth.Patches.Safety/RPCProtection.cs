using System.Collections.Generic;
using ExitGames.Client.Photon;
using HarmonyLib;
using Photon.Realtime;
using UnityEngine;

namespace Seralyth.Patches.Safety;

public class RPCProtection
{
	[HarmonyPatch(typeof(LoadBalancingClient), "OpRaiseEvent")]
	public class OpRaiseEventPatch
	{
		public static bool enabled = true;

		private const int MaxRPCs = 500;

		internal static float startTime = Time.unscaledTime;

		internal static int rpcCount = 0;

		private static bool Prefix(byte eventCode, object customEventContent, RaiseEventOptions raiseEventOptions, SendOptions sendOptions)
		{
			if (enabled)
			{
				float unscaledTime = Time.unscaledTime;
				if (unscaledTime - startTime > 1f)
				{
					startTime = unscaledTime;
					rpcCount = 0;
				}
				rpcCount++;
				if (rpcCount > 500)
				{
					if ((eventCode == 200 || eventCode == 201) && customEventContent != null)
					{
						Hashtable val = (Hashtable)((customEventContent is Hashtable) ? customEventContent : null);
						if (val != null)
						{
							foreach (object key in ((Dictionary<object, object>)(object)val).Keys)
							{
								if (key is byte b && b == 0)
								{
									if (val[key] is string text)
									{
										Debug.LogWarning((object)("Blocked RPC " + text + " as we are sending too much traffic over the network!"));
									}
									break;
								}
							}
							goto IL_0127;
						}
					}
					Debug.LogWarning((object)$"Blocked event {eventCode} as we are sending too much traffic over the network!");
					goto IL_0127;
				}
			}
			return true;
			IL_0127:
			return false;
		}
	}
}
