using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Seralyth.Classes.Menu;
using Seralyth.Extensions;
using Seralyth.Menu;
using Seralyth.Mods;
using Seralyth.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Seralyth.Managers;

public class PatreonManager : MonoBehaviour
{
	public readonly struct PatreonMembership
	{
		public readonly string TierName;

		public readonly string IconURL;

		public PatreonMembership(string tierName, string iconURL)
		{
			TierName = tierName;
			IconURL = iconURL;
		}
	}

	public static PatreonManager instance = null;

	public readonly Dictionary<string, PatreonMembership> PatreonMembers = new Dictionary<string, PatreonMembership>();

	private Material iconMaterial;

	private readonly Dictionary<VRRig, GameObject> iconPool = new Dictionary<VRRig, GameObject>();

	private static readonly List<Player> excludedIndicators = new List<Player>();

	public static bool IndicatorsEnabled = true;

	public const byte PatreonByte = 63;

	private static int lastPlayerCount;

	public void Awake()
	{
		instance = this;
		PhotonNetwork.NetworkingClient.EventReceived += EventReceived;
	}

	public static KeyValuePair<NetPlayer, PatreonMembership>[] GetAllMembersInRoom()
	{
		return (!NetworkSystem.Instance.InRoom) ? Array.Empty<KeyValuePair<NetPlayer, PatreonMembership>>() : (from player in NetworkSystem.Instance.PlayerListOthers
			where instance.PatreonMembers.ContainsKey(player.UserId)
			select new KeyValuePair<NetPlayer, PatreonMembership>(player, instance.PatreonMembers[player.UserId])).ToArray();
	}

	public static bool IsPlayerPatreonMember(NetPlayer player)
	{
		return instance.PatreonMembers.ContainsKey(player.UserId);
	}

	public static Color GetTierColor(string color)
	{
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		if (1 == 0)
		{
		}
		Color result = (Color)(color switch
		{
			"Donor" => Color32.op_Implicit(new Color32((byte)196, (byte)201, (byte)200, byte.MaxValue)), 
			"Supporter" => Color32.op_Implicit(new Color32((byte)241, (byte)196, (byte)15, byte.MaxValue)), 
			"Basic Tracker" => Color32.op_Implicit(new Color32((byte)189, (byte)221, (byte)244, byte.MaxValue)), 
			"Ultimate Tracker" => Color32.op_Implicit(new Color32((byte)170, (byte)184, (byte)194, byte.MaxValue)), 
			"Owner" => Color32.op_Implicit(new Color32((byte)108, (byte)190, (byte)127, byte.MaxValue)), 
			"Co-Owner" => Color32.op_Implicit(new Color32((byte)73, (byte)143, (byte)214, byte.MaxValue)), 
			"Console Owner" => Color32.op_Implicit(new Color32((byte)189, (byte)96, (byte)231, byte.MaxValue)), 
			"Menu Developer" => Color32.op_Implicit(new Color32((byte)212, (byte)132, (byte)61, byte.MaxValue)), 
			"Admin" => Color32.op_Implicit(new Color32(byte.MaxValue, (byte)110, (byte)118, byte.MaxValue)), 
			"Staff Manager" => Color32.op_Implicit(new Color32((byte)102, (byte)241, (byte)180, byte.MaxValue)), 
			"Moderator" => Color32.op_Implicit(new Color32((byte)88, (byte)101, (byte)242, byte.MaxValue)), 
			"Community Helper" => Color32.op_Implicit(new Color32((byte)253, (byte)215, (byte)101, byte.MaxValue)), 
			"Boyfriend" => Color32.op_Implicit(new Color32((byte)244, (byte)171, (byte)186, byte.MaxValue)), 
			_ => Color.white, 
		});
		if (1 == 0)
		{
		}
		return result;
	}

	public void Update()
	{
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Expected O, but got Unknown
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Expected O, but got Unknown
		List<VRRig> list = new List<VRRig>();
		foreach (KeyValuePair<VRRig, GameObject> item in iconPool.Where((KeyValuePair<VRRig, GameObject> indicator) => !IndicatorsEnabled || !indicator.Key.Active() || !IsPlayerPatreonMember(RigUtilities.GetPlayerFromVRRig(indicator.Key)) || excludedIndicators.Contains(indicator.Key.GetPhotonPlayer())))
		{
			list.Add(item.Key);
			Object.Destroy((Object)(object)item.Value);
		}
		foreach (VRRig item2 in list)
		{
			iconPool.Remove(item2);
		}
		if (!IndicatorsEnabled || !NetworkSystem.Instance.InRoom)
		{
			return;
		}
		KeyValuePair<NetPlayer, PatreonMembership>[] allMembersInRoom = GetAllMembersInRoom();
		KeyValuePair<NetPlayer, PatreonMembership>[] array = allMembersInRoom;
		for (int num = 0; num < array.Length; num++)
		{
			KeyValuePair<NetPlayer, PatreonMembership> keyValuePair = array[num];
			VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(keyValuePair.Key);
			if ((Object)(object)vRRigFromPlayer == (Object)null || excludedIndicators.Contains(keyValuePair.Key.GetPlayer()))
			{
				continue;
			}
			if (!iconPool.TryGetValue(vRRigFromPlayer, out var value))
			{
				value = GameObject.CreatePrimitive((PrimitiveType)3);
				Object.Destroy((Object)(object)value.GetComponent<Collider>());
				if ((Object)(object)iconMaterial == (Object)null)
				{
					iconMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
					iconMaterial.SetFloat("_Surface", 1f);
					iconMaterial.SetFloat("_Blend", 0f);
					iconMaterial.SetFloat("_SrcBlend", 5f);
					iconMaterial.SetFloat("_DstBlend", 10f);
					iconMaterial.SetFloat("_ZWrite", 0f);
					iconMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
					iconMaterial.renderQueue = 3000;
				}
				value.GetComponent<Renderer>().material = iconMaterial;
				value.GetComponent<Renderer>().material.mainTexture = (Texture)(object)AssetUtilities.LoadTextureFromURL(keyValuePair.Value.IconURL, "Images/Patreon/" + keyValuePair.Key.UserId + "." + FileUtilities.GetFileExtension(keyValuePair.Value.IconURL));
				value.GetComponent<Renderer>().material.color = Color.white;
				GameObject val = new GameObject("Seralyth_Nametag");
				val.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
				TextMeshPro val2 = val.AddComponent<TextMeshPro>();
				((TMP_Text)val2).fontSize = 4.8f;
				((TMP_Text)val2).alignment = (TextAlignmentOptions)514;
				((TMP_Text)(object)val2).SafeSetText(keyValuePair.Value.TierName);
				((TMP_Text)(object)val2).SafeSetFontStyle(Main.activeFontStyle);
				((TMP_Text)(object)val2).SafeSetFont(Main.activeFont);
				((Graphic)val2).color = GetTierColor(keyValuePair.Value.TierName);
				val2.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
				val2.transform.SetParent(value.transform, false);
				iconPool.Add(vRRigFromPlayer, value);
			}
			float indicatorDistance = Seralyth.Classes.Menu.Console.GetIndicatorDistance(vRRigFromPlayer);
			value.transform.localScale = new Vector3(0.4f, 0.4f, 0.01f) * vRRigFromPlayer.scaleFactor;
			value.transform.position = Visuals.GetNameTagTransform(vRRigFromPlayer).position + Visuals.GetNameTagTransform(vRRigFromPlayer).up * (indicatorDistance * vRRigFromPlayer.scaleFactor);
			value.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
			GameObject gameObject = ((Component)value.transform.Find("Seralyth_Nametag")).gameObject;
			gameObject.transform.position = Visuals.GetNameTagTransform(vRRigFromPlayer).position + Visuals.GetNameTagTransform(vRRigFromPlayer).up * ((indicatorDistance + 0.25f) * vRRigFromPlayer.scaleFactor);
			gameObject.transform.LookAt(((Component)Camera.main).transform.position);
			gameObject.transform.Rotate(0f, 180f, 0f);
		}
	}

	public static void EventReceived(EventData data)
	{
		try
		{
			NetPlayer val = NetPlayer.op_Implicit(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false));
			if (data.Code != 63 || !IsPlayerPatreonMember(val))
			{
				return;
			}
			VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(val);
			object[] array = ((data.CustomData == null) ? new object[0] : ((object[])data.CustomData));
			string text = ((array.Length != 0) ? ((string)array[0]) : "");
			string text2 = text;
			string text3 = text2;
			if (text3 == "indicator" && array.Length > 1 && array[1] is bool flag && true && flag)
			{
				if (!excludedIndicators.Contains(val.GetPlayer()))
				{
					excludedIndicators.Add(val.GetPlayer());
				}
				else if (excludedIndicators.Contains(val.GetPlayer()))
				{
					excludedIndicators.Remove(val.GetPlayer());
				}
			}
		}
		catch
		{
		}
	}

	public static void ExecuteCommand(string command, RaiseEventOptions options, params object[] parameters)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (NetworkSystem.Instance.InRoom)
		{
			PhotonNetwork.RaiseEvent((byte)63, (object)new object[1] { command }.Concat(parameters).ToArray(), options, SendOptions.SendReliable);
		}
	}

	public static void ExecuteCommand(string command, int[] targets, params object[] parameters)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		PatreonManager.ExecuteCommand(command, new RaiseEventOptions
		{
			TargetActors = targets
		}, parameters);
	}

	public static void ExecuteCommand(string command, int target, params object[] parameters)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		RaiseEventOptions val = new RaiseEventOptions();
		val.TargetActors = new int[1] { target };
		ExecuteCommand(command, val, parameters);
	}

	public static void ExecuteCommand(string command, ReceiverGroup target, params object[] parameters)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		PatreonManager.ExecuteCommand(command, new RaiseEventOptions
		{
			Receivers = target
		}, parameters);
	}

	public static void SetupPatreonMods(string patreonName)
	{
		NotificationManager.SendNotification("<color=grey>[</color><color=purple>PATREON</color><color=grey>]</color> Welcome, " + patreonName + "! Patreon mods have been enabled.", 10000);
		List<ButtonInfo> list = Buttons.buttons[Buttons.GetCategory("Main")].ToList();
		list.Add(new ButtonInfo
		{
			buttonText = "Patreon Mods",
			method = delegate
			{
				Buttons.CurrentCategoryName = "Patreon Mods";
			},
			isTogglable = false,
			toolTip = "Opens the patreon mods."
		});
		Buttons.buttons[Buttons.GetCategory("Main")] = list.ToArray();
		if (Main.dynamicSounds)
		{
			AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Menu/patreon.ogg", "Audio/Menu/patreon.ogg", delegate(AudioClip clip)
			{
				clip?.Play((float)Main.buttonClickVolume / 10f);
			});
		}
	}

	public static void ShowIndicator(bool enabled)
	{
		ExecuteCommand("indicator", (ReceiverGroup)1, enabled);
	}

	public static void ConstantHideIndicator()
	{
		if (!PhotonNetwork.InRoom)
		{
			lastPlayerCount = -1;
		}
		if (PhotonNetwork.PlayerList.Length != lastPlayerCount && PhotonNetwork.InRoom)
		{
			ShowIndicator(enabled: false);
			lastPlayerCount = PhotonNetwork.PlayerList.Length;
		}
	}
}
