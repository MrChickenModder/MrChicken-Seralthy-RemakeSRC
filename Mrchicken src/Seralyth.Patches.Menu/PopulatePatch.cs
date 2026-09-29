using System;
using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(FriendCard), "Populate", new Type[]
{
	typeof(Friend),
	typeof(bool)
})]
public class PopulatePatch
{
	public static bool enabled;

	public static void Postfix(FriendCard __instance, Friend friend)
	{
		if (enabled)
		{
			bool flag = friend.Presence.RoomId[0] == '@';
			__instance.SetRoom((flag ? friend.Presence.RoomId.Substring(1) : friend.Presence.RoomId).ToUpper());
			__instance.SetZone((flag ? "CUSTOM" : friend.Presence.Zone).ToUpper());
			__instance.joinable = true;
			__instance.UpdateComponentStates();
		}
	}
}
