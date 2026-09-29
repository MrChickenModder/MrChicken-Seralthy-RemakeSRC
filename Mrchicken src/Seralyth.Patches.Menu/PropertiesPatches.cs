using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using HarmonyLib;
using Photon.Realtime;

namespace Seralyth.Patches.Menu;

public class PropertiesPatches
{
	[HarmonyPatch(typeof(Player), "SetCustomProperties")]
	public class SetCustomPropertiesMethod
	{
		public static bool Prefix(Player __instance, ref Hashtable propertiesToSet)
		{
			if (__instance.IsLocal && enabled && ((IEnumerable<KeyValuePair<object, object>>)propertiesToSet).Any((KeyValuePair<object, object> prop) => prop.Key.ToString() != "didTutorial"))
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(/*Could not decode attribute arguments.*/)]
	public class SetCustomPropertiesField
	{
		public static bool Prefix(Player __instance, ref Hashtable value)
		{
			if (__instance.IsLocal && enabled && ((IEnumerable<KeyValuePair<object, object>>)value).Any((KeyValuePair<object, object> prop) => prop.Key.ToString() != "didTutorial"))
			{
				return false;
			}
			return true;
		}
	}

	public static bool enabled;
}
