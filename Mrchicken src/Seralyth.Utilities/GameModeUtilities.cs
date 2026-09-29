using System.Collections.Generic;
using System.Linq;
using GorillaGameModes;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Seralyth.Utilities;

public class GameModeUtilities
{
	public static List<NetPlayer> InfectedList()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected I4, but got Unknown
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		List<NetPlayer> list = new List<NetPlayer>();
		if (!PhotonNetwork.InRoom || (Object)(object)GorillaGameManager.instance == (Object)null)
		{
			return list;
		}
		GameModeType val = GorillaGameManager.instance.GameType();
		GameModeType val2 = val;
		switch (val2 - 1)
		{
		case 0:
		case 4:
		case 8:
		case 9:
		case 10:
		{
			GorillaTagManager val4 = (GorillaTagManager)GorillaGameManager.instance;
			if (val4.isCurrentlyTag)
			{
				list.Add(val4.currentIt);
			}
			else
			{
				list.AddRange(val4.currentInfected);
			}
			break;
		}
		case 3:
		case 5:
		{
			GorillaAmbushManager val5 = (GorillaAmbushManager)GorillaGameManager.instance;
			if (((GorillaTagManager)val5).isCurrentlyTag)
			{
				list.Add(((GorillaTagManager)val5).currentIt);
			}
			else
			{
				list.AddRange(((GorillaTagManager)val5).currentInfected);
			}
			break;
		}
		case 2:
		{
			GorillaPaintbrawlManager val3 = (GorillaPaintbrawlManager)GorillaGameManager.instance;
			list.AddRange(from deadPlayer in (from element in val3.playerLives
					where element.Value <= 0
					select element.Key).ToArray()
				select PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(deadPlayer, false) into dummy
				select NetPlayer.op_Implicit(dummy));
			if (!list.Contains(NetworkSystem.Instance.LocalPlayer))
			{
				list.Add(NetworkSystem.Instance.LocalPlayer);
			}
			break;
		}
		}
		return list;
	}

	public static void AddInfected(NetPlayer plr)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected I4, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		if (!PhotonNetwork.InRoom || (Object)(object)GorillaGameManager.instance == (Object)null)
		{
			return;
		}
		GameModeType val = GorillaGameManager.instance.GameType();
		GameModeType val2 = val;
		switch (val2 - 1)
		{
		case 0:
		case 4:
		case 8:
		case 9:
		case 10:
		{
			GorillaTagManager val5 = (GorillaTagManager)GorillaGameManager.instance;
			if (val5.isCurrentlyTag)
			{
				val5.ChangeCurrentIt(plr, true);
			}
			else if (!val5.currentInfected.Contains(plr))
			{
				val5.AddInfectedPlayer(plr, true);
			}
			break;
		}
		case 3:
		case 5:
		{
			GorillaAmbushManager val4 = (GorillaAmbushManager)GorillaGameManager.instance;
			if (((GorillaTagManager)val4).isCurrentlyTag)
			{
				((GorillaTagManager)val4).ChangeCurrentIt(plr, true);
			}
			else if (!((GorillaTagManager)val4).currentInfected.Contains(plr))
			{
				((GorillaTagManager)val4).AddInfectedPlayer(plr, true);
			}
			break;
		}
		case 2:
		{
			GorillaPaintbrawlManager val3 = (GorillaPaintbrawlManager)GorillaGameManager.instance;
			val3.playerLives[plr.ActorNumber] = 0;
			break;
		}
		case 1:
		case 6:
		case 7:
			break;
		}
	}

	public static void RemoveInfected(NetPlayer plr)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected I4, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Expected O, but got Unknown
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		if (!PhotonNetwork.InRoom || (Object)(object)GorillaGameManager.instance == (Object)null)
		{
			return;
		}
		GameModeType val = GorillaGameManager.instance.GameType();
		GameModeType val2 = val;
		switch (val2 - 1)
		{
		case 0:
		case 4:
		case 8:
		case 9:
		case 10:
		{
			GorillaTagManager val5 = (GorillaTagManager)GorillaGameManager.instance;
			if (val5.isCurrentlyTag)
			{
				if (val5.currentIt == plr)
				{
					val5.currentIt = null;
				}
			}
			else if (val5.currentInfected.Contains(plr))
			{
				val5.currentInfected.Remove(plr);
			}
			break;
		}
		case 3:
		case 5:
		{
			GorillaAmbushManager val4 = (GorillaAmbushManager)GorillaGameManager.instance;
			if (((GorillaTagManager)val4).isCurrentlyTag)
			{
				if (((GorillaTagManager)val4).currentIt == plr)
				{
					((GorillaTagManager)val4).currentIt = null;
				}
			}
			else if (((GorillaTagManager)val4).currentInfected.Contains(plr))
			{
				((GorillaTagManager)val4).currentInfected.Remove(plr);
			}
			break;
		}
		case 2:
		{
			GorillaPaintbrawlManager val3 = (GorillaPaintbrawlManager)GorillaGameManager.instance;
			val3.playerLives[plr.ActorNumber] = 3;
			break;
		}
		case 1:
		case 6:
		case 7:
			break;
		}
	}

	public static void AddRock(NetPlayer plr)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected I4, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		if (PhotonNetwork.InRoom && !((Object)(object)GorillaGameManager.instance == (Object)null))
		{
			GameModeType val = GorillaGameManager.instance.GameType();
			GameModeType val2 = val;
			switch (val2 - 1)
			{
			case 0:
			case 4:
			case 8:
			case 9:
			case 10:
			{
				GorillaTagManager val5 = (GorillaTagManager)GorillaGameManager.instance;
				val5.ChangeCurrentIt(plr, true);
				break;
			}
			case 3:
			case 5:
			{
				GorillaAmbushManager val4 = (GorillaAmbushManager)GorillaGameManager.instance;
				((GorillaTagManager)val4).ChangeCurrentIt(plr, true);
				break;
			}
			case 2:
			{
				GorillaPaintbrawlManager val3 = (GorillaPaintbrawlManager)GorillaGameManager.instance;
				val3.playerLives[plr.ActorNumber] = 0;
				break;
			}
			case 1:
			case 6:
			case 7:
				break;
			}
		}
	}

	public static void RemoveRock(NetPlayer plr)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected I4, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		if (!PhotonNetwork.InRoom || (Object)(object)GorillaGameManager.instance == (Object)null)
		{
			return;
		}
		GameModeType val = GorillaGameManager.instance.GameType();
		GameModeType val2 = val;
		switch (val2 - 1)
		{
		case 0:
		case 4:
		case 8:
		case 9:
		case 10:
		{
			GorillaTagManager val5 = (GorillaTagManager)GorillaGameManager.instance;
			if (val5.currentIt == plr)
			{
				val5.ChangeCurrentIt((NetPlayer)null, true);
			}
			break;
		}
		case 3:
		case 5:
		{
			GorillaAmbushManager val4 = (GorillaAmbushManager)GorillaGameManager.instance;
			if (((GorillaTagManager)val4).currentIt == plr)
			{
				((GorillaTagManager)val4).ChangeCurrentIt((NetPlayer)null, true);
			}
			break;
		}
		case 2:
		{
			GorillaPaintbrawlManager val3 = (GorillaPaintbrawlManager)GorillaGameManager.instance;
			val3.playerLives[plr.ActorNumber] = 3;
			break;
		}
		case 1:
		case 6:
		case 7:
			break;
		}
	}
}
