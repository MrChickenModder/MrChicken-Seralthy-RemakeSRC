using System;
using System.Collections.Generic;
using GorillaGameModes;
using HarmonyLib;
using Photon.Pun;
using Seralyth.Extensions;
using Seralyth.Menu;
using Seralyth.Utilities;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(GameMode), "ReportTag")]
public class TagPatch
{
	public static readonly List<NetPlayer> taggedPlayers = new List<NetPlayer>();

	public static bool enabled;

	public static float tagDelay;

	public static int tagCount;

	private static void PlaySound(string name)
	{
		AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Mods/Fun/TagSounds/" + name + ".wav", name + ".wav", delegate(AudioClip clip)
		{
			clip.Play((float)Main.buttonClickVolume / 10f);
		});
	}

	public static void Postfix(NetPlayer player)
	{
		if (!enabled || !PhotonNetwork.InRoom)
		{
			return;
		}
		if (Time.time > tagDelay)
		{
			taggedPlayers.Clear();
			tagCount = 0;
		}
		if (taggedPlayers.Contains(player))
		{
			return;
		}
		taggedPlayers.Add(player);
		tagCount = Math.Min(tagCount + 1, 7);
		tagDelay = Time.time + 10f;
		switch (tagCount)
		{
		case 1:
			if (GameModeUtilities.InfectedList().Count <= 1)
			{
				PlaySound("firstblood");
			}
			break;
		case 2:
			PlaySound("doublekill");
			break;
		case 3:
			PlaySound("triplekill");
			break;
		case 4:
			PlaySound("killingspree");
			break;
		case 5:
			PlaySound("wickedsick");
			break;
		case 6:
			PlaySound("monsterkill");
			break;
		case 7:
			PlaySound("rampage");
			break;
		}
	}
}
