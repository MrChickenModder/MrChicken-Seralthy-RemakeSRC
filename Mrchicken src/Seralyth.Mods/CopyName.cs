using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Seralyth.Extensions;
using Seralyth.Managers;
using Seralyth.Menu;
using Seralyth.Utilities;
using UnityEngine;

namespace Seralyth.Mods;

public static class CopyName
{
	public static void Self()
	{
		GUIUtility.systemCopyBuffer = PhotonNetwork.LocalPlayer.NickName;
		NotificationManager.SendNotification("<color=grey>[</color><color=green>COPY NAME</color><color=grey>]</color> Copied your name to clipboard.");
	}

	public static void Gun()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true))
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				GUIUtility.systemCopyBuffer = Main.CleanPlayerName(RigUtilities.GetPlayerFromVRRig(componentInParent).NickName);
				NotificationManager.SendNotification("<color=grey>[</color><color=green>COPY NAME</color><color=grey>]</color> Copied target's name to clipboard.");
			}
		}
	}

	public static void All()
	{
		string systemCopyBuffer = string.Join(", ", PhotonNetwork.PlayerList.Select((Player player) => player.NickName));
		GUIUtility.systemCopyBuffer = systemCopyBuffer;
		NotificationManager.SendNotification("<color=grey>[</color><color=green>COPY NAME</color><color=grey>]</color> Copied all names to clipboard.");
	}
}
