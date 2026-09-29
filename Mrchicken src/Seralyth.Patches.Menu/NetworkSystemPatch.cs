using HarmonyLib;
using Seralyth.Menu;

namespace Seralyth.Patches.Menu;

public class NetworkSystemPatch
{
	[HarmonyPatch(typeof(NetworkSystemPUN), "ConnectToRoom")]
	public class ConnectToRoom
	{
		public static bool Prefix(string roomName, RoomConfig opts, int regionIndex = -1)
		{
			if (Buttons.GetIndex("Unlock Fan Club Subscription").enabled)
			{
				if (opts.MaxPlayers == 20)
				{
					opts.MaxPlayers = 10;
				}
				if ((string)opts.CustomProps[(object)"fan_club"] == "true")
				{
					opts.CustomProps[(object)"fan_club"] = "false";
				}
			}
			return true;
		}
	}
}
