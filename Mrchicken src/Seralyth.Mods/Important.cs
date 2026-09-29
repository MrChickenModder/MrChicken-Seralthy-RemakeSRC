using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaGameModes;
using GorillaNetworking;
using GorillaTagScripts;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.CloudScriptModels;
using Seralyth.Classes.Menu;
using Seralyth.Extensions;
using Seralyth.Managers;
using Seralyth.Managers.DiscordRPC;
using Seralyth.Managers.DiscordRPC.Logging;
using Seralyth.Menu;
using Seralyth.Patches.Menu;
using Seralyth.Utilities;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Networking;
using UnityEngine.TextCore;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Valve.Newtonsoft.Json;

namespace Seralyth.Mods;

public static class Important
{
	public class CustomRoomConfig
	{
		public string name;

		public bool isPublic;

		public byte size;
	}

	internal enum VirtualKeyCodes : uint
	{
		NEXT_TRACK = 176u,
		PREVIOUS_TRACK = 177u,
		PLAY_PAUSE = 179u
	}

	private static readonly List<string> createdRooms = new List<string>();

	private static readonly List<string> createdNames = new List<string>();

	public static string oldId = "";

	public static Coroutine queueCoroutine;

	public static int reconnectDelay = 1;

	public static bool instantCreate;

	private static DiscordRpcClient discord;

	private static DateTime? startTime;

	private static DateTime? endTime;

	private static float updateTime;

	private static bool quickSongExists;

	private static float updateDataDelay;

	private static float inputDelay;

	private static GameObject mediaIcon;

	private static Material mediaIconMaterial;

	private static TextMeshPro mediaText;

	private static TMP_SpriteAsset _mediaSpriteSheet;

	private static bool wasenabled = true;

	public static float zoomFOV = 35f;

	private static bool reportMenuToggle;

	private static bool acceptedTOS;

	public static GameObject physicalQuitBox;

	private static float lastTime;

	public static int targetCustomFps = 90;

	private static Vector3? oldLocalPosition;

	private static bool lastTagLag;

	private static bool lastSteam;

	public static string[] roomBrowserMaps = new string[10] { "Forest", "Cave", "Beach", "Canyon", "Mountain", "City", "Clouds", "Basement", "Metropolis", "Bayou" };

	public static int roomBrowserMapIndex;

	private static readonly string gtScreenshotsPath = "C:\\Users\\kalew\\OneDrive\\Pictures\\Gorilla Tag photos";

	private static string CreatedRoomsFile => "SeralythMenu/CreatedRooms.txt";

	private static string CreatedNamesFile => "SeralythMenu/CreatedNames.txt";

	public static string Title { get; private set; } = "Unknown";

	public static string Artist { get; private set; } = "Unknown";

	public static Texture2D Icon { get; private set; } = new Texture2D(2, 2);

	public static bool Paused { get; private set; } = true;

	public static float StartTime { get; private set; }

	public static float EndTime { get; private set; }

	public static float ElapsedTime { get; private set; }

	public static bool ValidData { get; private set; }

	public static TMP_SpriteAsset MediaSpriteSheet
	{
		get
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Expected O, but got Unknown
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Expected O, but got Unknown
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Expected O, but got Unknown
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Expected O, but got Unknown
			List<Texture2D> textureList;
			List<(string name, int index)> spriteDataList;
			if ((Object)(object)_mediaSpriteSheet == (Object)null)
			{
				_mediaSpriteSheet = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
				((Object)_mediaSpriteSheet).name = "Seralyth_SpriteSheet";
				textureList = new List<Texture2D>();
				spriteDataList = new List<(string, int)>();
				AddSprite("Pause", AssetUtilities.LoadTextureFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Images/Mods/Important/pause.png", "Images/Mods/Important/pause.png"));
				int num = 512;
				Texture2D val = new Texture2D(num, num);
				Rect[] array = val.PackTextures(textureList.ToArray(), 2, num);
				_mediaSpriteSheet.spriteSheet = (Texture)(object)val;
				((TMP_Asset)_mediaSpriteSheet).material = new Material(Shader.Find("TextMeshPro/Sprite"))
				{
					mainTexture = (Texture)(object)val
				};
				_mediaSpriteSheet.spriteInfoList = new List<TMP_Sprite>();
				Traverse.Create((object)_mediaSpriteSheet).Field("m_Version").SetValue((object)"1.1.0");
				_mediaSpriteSheet.spriteGlyphTable.Clear();
				for (int i = 0; i < spriteDataList.Count; i++)
				{
					Rect val2 = array[i];
					TMP_SpriteGlyph item = new TMP_SpriteGlyph
					{
						index = (uint)i,
						metrics = new GlyphMetrics(((Rect)(ref val2)).width * (float)((Texture)val).width, ((Rect)(ref val2)).height * (float)((Texture)val).height, (0f - ((Rect)(ref val2)).width * (float)((Texture)val).width) / 2f, ((Rect)(ref val2)).height * (float)((Texture)val).height * 0.8f, ((Rect)(ref val2)).width * (float)((Texture)val).width),
						glyphRect = new GlyphRect((int)(((Rect)(ref val2)).x * (float)((Texture)val).width), (int)(((Rect)(ref val2)).y * (float)((Texture)val).height), (int)(((Rect)(ref val2)).width * (float)((Texture)val).width), (int)(((Rect)(ref val2)).height * (float)((Texture)val).height)),
						scale = 1f,
						atlasIndex = 0
					};
					_mediaSpriteSheet.spriteGlyphTable.Add(item);
				}
				_mediaSpriteSheet.spriteCharacterTable.Clear();
				for (int j = 0; j < spriteDataList.Count; j++)
				{
					string item2 = spriteDataList[j].name;
					TMP_SpriteCharacter item3 = new TMP_SpriteCharacter(65534u, _mediaSpriteSheet.spriteGlyphTable[j])
					{
						name = item2,
						scale = 1f,
						glyphIndex = (uint)j
					};
					_mediaSpriteSheet.spriteCharacterTable.Add(item3);
				}
				_mediaSpriteSheet.UpdateLookupTables();
			}
			return _mediaSpriteSheet;
			void AddSprite(string name, Texture2D tex)
			{
				spriteDataList.Add((name, textureList.Count));
				textureList.Add(tex);
			}
		}
	}

	public static void RoomCreator()
	{
		CustomRoomConfig room = new CustomRoomConfig();
		Main.prompts.Clear();
		Main.Prompt("Would you like to pick your own room name or have a random one?", delegate
		{
			Main.Prompt("Do you want to make your room name an emoji?", delegate
			{
				int category = Buttons.CurrentCategoryIndex;
				string[] array = new string[5] { "\ud83d\ude00", "\ud83d\ude02", "\ud83d\ude0d", "\ud83d\ude0e", "\ud83d\ude01" };
				List<ButtonInfo> list = new List<ButtonInfo>
				{
					new ButtonInfo
					{
						label = true,
						buttonText = "Click on an emoji you'd like to use!"
					}
				};
				string[] array2 = array;
				foreach (string emoji in array2)
				{
					list.Add(new ButtonInfo
					{
						buttonText = emoji,
						method = delegate
						{
							room.name = emoji;
							Buttons.CurrentCategoryIndex = category;
							Buttons.buttons[Buttons.GetCategory("Temporary Category")] = Array.Empty<ButtonInfo>();
							AskSize();
						},
						isTogglable = false
					});
				}
				Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list.ToArray();
				Buttons.CurrentCategoryName = "Temporary Category";
			}, delegate
			{
				Main.PromptSingleText("What would you like your room name to be?", delegate
				{
					if (StringUtils.IsNullOrEmpty(Main.keyboardInput))
					{
						Main.PromptSingle("Room name cannot be empty.");
						RoomCreator();
					}
					else
					{
						room.name = Main.keyboardInput;
						AskPublic();
					}
				}, "Ok");
			});
		}, delegate
		{
			room.name = RandomUtilities.RandomString();
			AskPublic();
		}, "My own", "Random");
		static void AddRoomButton(string name)
		{
			if (!createdRooms.Contains(name))
			{
				createdRooms.Add(name);
				int category = Buttons.GetCategory("Room Mods");
				Buttons.buttons[category] = Buttons.buttons[category].Append(new ButtonInfo
				{
					buttonText = "Join Room: " + name,
					method = delegate
					{
						((PhotonNetworkController)PhotonNetworkController.Instance).AttemptToJoinSpecificRoom(name, (JoinType)0);
					},
					isTogglable = false,
					toolTip = "Joins the room \"" + name + "\"."
				}).ToArray();
			}
		}
		void AskConfirm()
		{
			string displayName = (room.name.StartsWith("@") ? room.name.Substring(1) : room.name);
			displayName = displayName.Replace("<size=1000%>", "").Replace("</size>", "");
			string finalName = ((room.size > 10 && !room.name.StartsWith("@")) ? ("@" + room.name) : room.name);
			Main.Prompt("Are you ok with your custom room?:\nRoom Name: " + displayName + "\n" + $"Public: {room.isPublic}\n" + $"Size: {room.size}", delegate
			{
				CreateRoom(finalName, room.isPublic, room.size, (JoinType)0);
				AddRoomButton(displayName);
				NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Creating room, please be patient.");
				Main.PromptSingle("Success! Please be patient while the room is being created.");
			}, delegate
			{
				RoomCreator();
			});
		}
		void AskPublic()
		{
			Main.Prompt("Do you want your room to be public?", delegate
			{
				room.isPublic = true;
				AskRoomSize();
			}, delegate
			{
				room.isPublic = false;
				AskRoomSize();
			});
		}
		void AskRoomSize()
		{
			Main.PromptSingleText("What do you want your room size to be? (minimum 1, maximum 20)", delegate
			{
				byte result;
				int value = (byte.TryParse(Main.keyboardInput, out result) ? result : 10);
				room.size = (byte)Math.Clamp(value, 1, 20);
				AskConfirm();
			}, "Done");
		}
		void AskSize()
		{
			Main.Prompt("Would you like to enlarge the room name in-game?", delegate
			{
				string text = (room.name.StartsWith("@") ? room.name.Substring(1) : room.name);
				string text2 = (room.name.StartsWith("@") ? "@" : "");
				room.name = text2 + "<size=1000%>" + text + "</size>";
				AskPublic();
			}, delegate
			{
				AskPublic();
			});
		}
	}

	public static void NameCreator()
	{
		string emojiChoice = null;
		Main.Prompt("Would you like a custom name or random?", delegate
		{
			Main.Prompt("Do you want to make your name an emoji?", delegate
			{
				int category = Buttons.CurrentCategoryIndex;
				string[] array = new string[5] { "\ud83d\ude00", "\ud83d\ude02", "\ud83d\ude0d", "\ud83d\ude0e", "\ud83d\ude01" };
				List<ButtonInfo> list = new List<ButtonInfo>
				{
					new ButtonInfo
					{
						label = true,
						buttonText = "Click on an emoji you'd like to use!"
					}
				};
				string[] array2 = array;
				foreach (string text in array2)
				{
					string captured = text;
					list.Add(new ButtonInfo
					{
						buttonText = captured,
						method = delegate
						{
							emojiChoice = captured;
							Buttons.CurrentCategoryIndex = category;
							Buttons.buttons[Buttons.GetCategory("Temporary Category")] = Array.Empty<ButtonInfo>();
							AskConfirm(emojiChoice);
						},
						isTogglable = false
					});
				}
				Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list.ToArray();
				Buttons.CurrentCategoryName = "Temporary Category";
			}, delegate
			{
				Main.PromptSingleText("What would you like your name to be?", delegate
				{
					if (StringUtils.IsNullOrEmpty(Main.keyboardInput))
					{
						Main.PromptSingle("Name cannot be empty.");
						NameCreator();
					}
					else
					{
						AskConfirm(Main.keyboardInput);
					}
				}, "Ok");
			});
		}, delegate
		{
			AskConfirm(RandomRealName());
		}, "My own", "Random");
		static void AddNameButton(string name)
		{
			if (!createdNames.Contains(name))
			{
				createdNames.Add(name);
				int category = Buttons.GetCategory("Room Mods");
				Buttons.buttons[category] = Buttons.buttons[category].Append(new ButtonInfo
				{
					buttonText = "Set Name: " + name,
					method = delegate
					{
						Main.ChangeName(name);
					},
					isTogglable = false,
					toolTip = "Sets your name to \"" + name + "\"."
				}).ToArray();
			}
		}
		static void AskConfirm(string name)
		{
			Main.Prompt("Are you ok with this name?\n" + name, delegate
			{
				Main.ChangeName(name);
				AddNameButton(name);
				Main.PromptSingle("Name set successfully!");
			}, delegate
			{
				NameCreator();
			});
		}
	}

	public static string RandomRealName()
	{
		string text = ((Random.Range(0, 3) == 0) ? Safety.namePrefix[Random.Range(0, Safety.namePrefix.Length)] : "");
		string text2 = ((Random.Range(0, 3) == 0) ? Safety.nameSuffix[Random.Range(0, Safety.nameSuffix.Length)] : "");
		string text3 = text + Safety.names[Random.Range(0, Safety.names.Length)] + text2;
		return (text3.Length > 12) ? text3.Substring(0, 12) : text3;
	}

	public static void SaveCreatedRooms()
	{
		File.WriteAllLines(CreatedRoomsFile, createdRooms);
	}

	public static void SaveCreatedNames()
	{
		File.WriteAllLines(CreatedNamesFile, createdNames);
	}

	public static void AddNameButton(string name)
	{
		if (!createdNames.Contains(name))
		{
			createdNames.Add(name);
			SaveCreatedNames();
			int category = Buttons.GetCategory("Room Mods");
			Buttons.buttons[category] = Buttons.buttons[category].Append(new ButtonInfo
			{
				buttonText = "Set Name: " + name,
				method = delegate
				{
					Main.ChangeName(name);
				},
				isTogglable = false,
				toolTip = "Sets your name to \"" + name + "\"."
			}).ToArray();
		}
	}

	private static void AddRoomButton(string name)
	{
		if (!createdRooms.Contains(name))
		{
			createdRooms.Add(name);
			SaveCreatedRooms();
			int category = Buttons.GetCategory("Room Mods");
			Buttons.buttons[category] = Buttons.buttons[category].Append(new ButtonInfo
			{
				buttonText = "Join Room: " + name,
				method = delegate
				{
					((PhotonNetworkController)PhotonNetworkController.Instance).AttemptToJoinSpecificRoom(name, (JoinType)0);
				},
				isTogglable = false,
				toolTip = "Joins the room \"" + name + "\"."
			}).ToArray();
		}
	}

	public static void LoadCreatedData()
	{
		if (File.Exists(CreatedRoomsFile))
		{
			string[] array = File.ReadAllLines(CreatedRoomsFile);
			string[] array2 = array;
			foreach (string room in array2)
			{
				if (!string.IsNullOrEmpty(room) && !createdRooms.Contains(room))
				{
					createdRooms.Add(room);
					int category = Buttons.GetCategory("Room Mods");
					Buttons.buttons[category] = Buttons.buttons[category].Append(new ButtonInfo
					{
						buttonText = "Join Room: " + room,
						method = delegate
						{
							((PhotonNetworkController)PhotonNetworkController.Instance).AttemptToJoinSpecificRoom(room, (JoinType)0);
						},
						isTogglable = false,
						toolTip = "Joins the room \"" + room + "\"."
					}).ToArray();
				}
			}
		}
		if (!File.Exists(CreatedNamesFile))
		{
			return;
		}
		string[] array3 = File.ReadAllLines(CreatedNamesFile);
		string[] array4 = array3;
		foreach (string name in array4)
		{
			if (!string.IsNullOrEmpty(name) && !createdNames.Contains(name))
			{
				createdNames.Add(name);
				int category2 = Buttons.GetCategory("Room Mods");
				Buttons.buttons[category2] = Buttons.buttons[category2].Append(new ButtonInfo
				{
					buttonText = "Set Name: " + name,
					method = delegate
					{
						Main.ChangeName(name);
					},
					isTogglable = false,
					toolTip = "Sets your name to \"" + name + "\"."
				}).ToArray();
			}
		}
	}

	public static void ClearCreatedRooms()
	{
		createdRooms.Clear();
		if (File.Exists(CreatedRoomsFile))
		{
			File.Delete(CreatedRoomsFile);
		}
		int category = Buttons.GetCategory("Room Mods");
		Buttons.buttons[category] = Buttons.buttons[category].Where((ButtonInfo b) => !b.buttonText.StartsWith("Join Room: ")).ToArray();
	}

	public static void ClearCreatedNames()
	{
		createdNames.Clear();
		if (File.Exists(CreatedNamesFile))
		{
			File.Delete(CreatedNamesFile);
		}
		int category = Buttons.GetCategory("Room Mods");
		Buttons.buttons[category] = Buttons.buttons[category].Where((ButtonInfo b) => !b.buttonText.StartsWith("Set Name: ")).ToArray();
	}

	public static async void CheckNewAcc()
	{
		await Task.Delay(10000);
		if (PhotonNetwork.LocalPlayer.UserId != oldId)
		{
			Main.playTime = 0f;
		}
	}

	public static IEnumerator QueueRoomCoroutine(string roomName)
	{
		NetworkSystemPUN instance = (NetworkSystemPUN)NetworkSystem.Instance;
		((NetworkSystem)instance).ReturnToSinglePlayer();
		yield return (object)new WaitUntil((Func<bool>)(() => (int)((NetworkSystem)instance).netState == 2));
		yield return (object)new WaitForSeconds(0.5f);
		while (!((NetworkSystem)instance).InRoom)
		{
			((PhotonNetworkController)PhotonNetworkController.Instance).AttemptToJoinSpecificRoom(roomName, (JoinType)0);
			yield return (object)new WaitForSeconds((float)reconnectDelay);
		}
	}

	public static void QueueRoom(string roomName)
	{
		if (queueCoroutine != null)
		{
			((MonoBehaviour)CoroutineManager.instance).StopCoroutine(queueCoroutine);
		}
		queueCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(QueueRoomCoroutine(roomName));
	}

	public static void Reconnect()
	{
		string roomName = NetworkSystem.Instance.RoomName;
		NetworkSystem.Instance.ReturnToSinglePlayer();
		QueueRoom(roomName);
	}

	public static void CancelReconnect()
	{
		if (queueCoroutine != null)
		{
			((MonoBehaviour)CoroutineManager.instance).StopCoroutine(queueCoroutine);
		}
		NetworkSystem.Instance.netState = (NetSystemState)(NetworkSystem.Instance.InRoom ? 4 : 2);
		Main.partyLastCode = null;
		Main.partyKickReconnecting = false;
	}

	public static void JoinRandom()
	{
		if (PhotonNetwork.InRoom)
		{
			NetworkSystem.Instance.ReturnToSinglePlayer();
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(JoinRandomDelay());
		}
		else
		{
			GorillaNetworkJoinTrigger val = ((PhotonNetworkController)PhotonNetworkController.Instance).currentJoinTrigger ?? ((GorillaComputer)GorillaComputer.instance).GetJoinTriggerForZone("forest");
			((PhotonNetworkController)PhotonNetworkController.Instance).AttemptToJoinPublicRoom(val, (JoinType)0, (List<ValueTuple<string, string>>)null, false);
		}
	}

	public static IEnumerator JoinRandomDelay()
	{
		yield return (object)new WaitForSeconds(1.5f);
		JoinRandom();
	}

	public static async Task ForceCreateRoom(string name, RoomConfig options)
	{
		if (NetworkSystem.Instance.InRoom)
		{
			await NetworkSystem.Instance.ReturnToSinglePlayer();
		}
		NetworkSystem instance = NetworkSystem.Instance;
		await ((NetworkSystemPUN)((instance is NetworkSystemPUN) ? instance : null)).TryCreateRoom(name, options);
	}

	public static void CreateRoom(string roomName, bool isPublic, byte roomSize = 0, JoinType roomJoinType = (JoinType)0)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Expected O, but got Unknown
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Expected O, but got Unknown
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Expected O, but got Unknown
		//IL_011d: Expected O, but got Unknown
		//IL_011e: Expected O, but got Unknown
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Invalid comparison between Unknown and I4
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Invalid comparison between Unknown and I4
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Expected I4, but got Unknown
		if (roomSize > 10 && !roomName.StartsWith("@"))
		{
			roomName = "@" + roomName;
		}
		GorillaNetworkJoinTrigger val = ((PhotonNetworkController)PhotonNetworkController.Instance).currentJoinTrigger ?? ((GorillaComputer)GorillaComputer.instance).GetJoinTriggerForZone("forest");
		RoomConfig val2 = new RoomConfig
		{
			createIfMissing = true,
			isJoinable = true,
			isPublic = isPublic,
			MaxPlayers = ((roomSize == 0) ? RoomSystem.GetRoomSizeForCreate(val.zone, Enum.Parse<GameModeType>(((GorillaComputer)GorillaComputer.instance).currentGameMode.Value, ignoreCase: true), !isPublic, SubscriptionManager.IsLocalSubscribed()) : roomSize)
		};
		Hashtable val3 = new Hashtable();
		((Dictionary<object, object>)val3).Add((object)"platform", (object)((PhotonNetworkController)PhotonNetworkController.Instance).platformTag);
		((Dictionary<object, object>)val3).Add((object)"gameMode", (object)val.GetFullDesiredGameModeString());
		((Dictionary<object, object>)val3).Add((object)"language", (object)((object)LocalisationManager.CurrentLanguage).ToString());
		((Dictionary<object, object>)val3).Add((object)"fan_club", (object)(SubscriptionManager.IsLocalSubscribed() ? "true" : "false"));
		((Dictionary<object, object>)val3).Add((object)"queueName", (object)((GorillaComputer)GorillaComputer.instance).currentQueue);
		val2.CustomProps = val3;
		RoomConfig val4 = val2;
		((PhotonNetworkController)PhotonNetworkController.Instance).currentJoinType = roomJoinType;
		if ((int)roomJoinType == 2 || (int)roomJoinType == 4)
		{
			Task.Run((Func<Task?>)((PhotonNetworkController)PhotonNetworkController.Instance).SendPartyFollowCommands);
		}
		switch (roomJoinType - 1)
		{
		case 0:
		case 2:
			val4.SetFriendIDs(((PhotonNetworkController)PhotonNetworkController.Instance).FriendIDList);
			break;
		case 1:
		case 3:
			val4.SetFriendIDs(FriendshipGroupDetection.Instance.PartyMemberIDs.ToList());
			break;
		}
		if (instantCreate)
		{
			NetworkSystem instance = NetworkSystem.Instance;
			((NetworkSystemPUN)((instance is NetworkSystemPUN) ? instance : null)).internalState = (InternalState)16;
			ForceCreateRoom(roomName, val4);
		}
		else
		{
			NetworkSystem.Instance.ConnectToRoom(roomName, val4, -1);
		}
	}

	public static void BroadcastRoom(string roomName, bool create, string key, string shuffler)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		string roomToJoin = NetworkSystem.ShuffleRoomName(roomName, shuffler.Substring(2, 8), true) + "|" + NetworkSystem.ShuffleRoomName("ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Substring(NetworkSystem.Instance.currentRegionIndex, 1), shuffler.Substring(0, 2), true);
		BroadcastMyRoomRequest val = new BroadcastMyRoomRequest
		{
			KeyToFollow = key,
			RoomToJoin = roomToJoin,
			Set = create
		};
		((GorillaServer)GorillaServer.Instance).BroadcastMyRoom(val, (Action<ExecuteFunctionResult>)delegate
		{
		}, (Action<PlayFabError>)delegate
		{
		});
	}

	public static void RestartGame()
	{
		string text = "\n                                            %%%%%                                                   \n                                           %%% %%%%                                                 \n                                         %%%      %%%%                                              \n                                        %%%         %%%%        %%%  %                              \n                                      %%%%            %%%%%%%% %%%%  %%                             \n                                     %%%        %#####% %%%%%        %%                             \n                                    %%%       ############ %%%                                      \n                                  %%%       ######     %###  %%%%     %%%                           \n                                %%%%       ######        ###   %#%%    %%                           \n                             %%%#%        ######         ###%    %#%%                               \n                       %%%%  %%#%         ######         %###      %##% %%                          \n                 %%%%  %%   %##           ######%         ##%         %###%                         \n                           %#%             ######        ###            ###%                        \n                         %##%              %######%    #####              ###%                      \n#%   %##                  #######%                        ###                    \n                   %% %##                     %#######%                        ###%                 \n###                        %########%                       ###%               \n###                            %#######%                       %##%             \n                  %##                                %#######%                        ###           \n                %##%                                   %#######%                     ###%           \n###                   %##########%        #######%                   ###             \n##%                  %####%    %####        %######%                ###               \n###                  %###%        %##%         %######%              ###                \n###                 ###%          %%%           %######%            ##%                 \n###              %###                          #######          ####                  \n                %###           ####                          #######        %###                    \n####         ####                          #######       ###   ##                 \n                    %###       ####                         %######       ##%    ##%                \n###      ###                         ######      ###                         \n                         %###   ####                       ######      ###        %%%               \n####  %####                   %######     ###           #%               \n                            %%###% ####%              ########      ##%         %%%                 \n###%%######%%    %#########%      ###     %%%% %%%%                 \n                             %#   %### %###############%         ##%%%%% %%%%                       \n                              %%    %##%                       %##  %                               \n                                       %##                    %#%                                   \n                               %%        %#%%               %%%%                                    \n                               %%%         %%#%            %%%                                      \n                                      %%%%%  %%%%        %%%%                                       \n                                 %%%%           %%%     %%%                                         \n                                                  %%%% %%%                                          \n                                                    %%%%                                            ".Split("\n").Aggregate("", (string current, string line) => current + Environment.NewLine + "echo      " + line);
		string contents = "@echo off\ntitle MrChicken Menu\ncolor 0E\n\ncls\necho." + text + "\necho.\n\necho Your game is restarting, please wait...\necho.\n\n:WAIT_LOOP\ntasklist /FI \"IMAGENAME eq Gorilla Tag.exe\" | find /I \"Gorilla Tag.exe\" >nul\nif %ERRORLEVEL%==0 (\n    timeout /t 1 >nul\n    goto WAIT_LOOP\n)\n\nstart steam://run/1533390\nexit";
		string text2 = "SeralythMenu/RestartScript.bat";
		File.WriteAllText(text2, contents);
		string fileName = FileUtilities.GetGamePath() + "/" + text2;
		Process.Start(fileName);
		Application.Quit();
	}

	public static void OpenGorillaTagFolder()
	{
		string fileName = Assembly.GetExecutingAssembly().Location.Split("BepInEx\\")[0];
		Process.Start(fileName);
	}

	public static void DiscordRPC()
	{
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		if (discord == null)
		{
			discord = new DiscordRpcClient("1396080212441042944")
			{
				Logger = new DiscordLogManager()
			};
			discord.Initialize();
		}
		if (NetworkSystem.Instance.InRoom)
		{
			endTime = null;
			if (!startTime.HasValue)
			{
				startTime = DateTime.UtcNow;
			}
		}
		else
		{
			startTime = null;
			if (!endTime.HasValue)
			{
				endTime = DateTime.UtcNow;
			}
		}
		if (Time.time > updateTime)
		{
			updateTime = Time.time + 1f;
			bool inRoom = NetworkSystem.Instance.InRoom;
			string arg = (inRoom ? NetworkSystem.Instance.RoomName : "-");
			discord.SetPresence(new RichPresence
			{
				Details = (inRoom ? ("Playing " + ((object)GorillaGameManager.instance.GameType()/*cast due to .constrained prefix*/).ToString().ToLower()) : "Playing alone"),
				State = (inRoom ? $"Room: {arg} ({PhotonNetwork.PlayerList.Length}/{PhotonNetwork.CurrentRoom.MaxPlayers})" : "Not in a room"),
				Assets = new Assets
				{
					LargeImageKey = "cone",
					LargeImageText = "MrChicken Menu",
					SmallImageKey = (inRoom ? "online" : "offline"),
					SmallImageText = (inRoom ? "Online" : "Offline")
				},
				Timestamps = (inRoom ? new Timestamps
				{
					Start = (startTime ?? endTime ?? DateTime.UtcNow)
				} : null),
				Buttons = new Button[2]
				{
					new Button
					{
						Label = "Discord Server",
						Url = Main.serverLink
					},
					new Button
					{
						Label = "Download",
						Url = "https://github.com/1x1x1x1736/api"
					}
				}
			});
		}
	}

	public static void DisableDiscordRPC()
	{
		if (discord != null)
		{
			discord.ClearPresence();
			discord.Dispose();
			discord = null;
		}
	}

	public static void EnsureIntegrationProgram()
	{
		quickSongExists = File.Exists("SeralythMenu/QuickSong.exe");
		if (quickSongExists)
		{
			return;
		}
		Main.Prompt("This mod requires the \"QuickSong\" library. Would you like to automatically download it? (16.3mb)", delegate
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Invalid comparison between Unknown and I4
			UnityWebRequest val = UnityWebRequest.Get("https://github.com/iiDk-the-actual/QuickSong/releases/latest/download/QuickSong.exe");
			try
			{
				UnityWebRequestAsyncOperation val2 = val.SendWebRequest();
				while (!((AsyncOperation)val2).isDone)
				{
				}
				if ((int)val.result == 1)
				{
					File.WriteAllBytes("SeralythMenu/QuickSong.exe", val.downloadHandler.data);
					NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully downloaded QuickSong to SeralythMenu/QuickSong.exe.");
				}
				else
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Could not download QuickSong: " + (StringUtils.IsNullOrEmpty(val.error) ? "Unknown error" : val.error));
				}
				quickSongExists = File.Exists("SeralythMenu/QuickSong.exe");
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}, delegate
		{
			Main.Toggle("Media Integration");
		});
	}

	public static async Task UpdateDataAsync()
	{
		ProcessStartInfo psi = new ProcessStartInfo
		{
			FileName = FileUtilities.GetGamePath() + "/SeralythMenu/QuickSong.exe",
			Arguments = "-all",
			UseShellExecute = false,
			RedirectStandardOutput = true,
			CreateNoWindow = true
		};
		Process proc = new Process
		{
			StartInfo = psi
		};
		try
		{
			proc.Start();
			string output = await proc.StandardOutput.ReadToEndAsync();
			await Task.Run(delegate
			{
				proc.WaitForExit();
			});
			Paused = true;
			Title = "Unknown";
			Artist = "Unknown";
			StartTime = 0f;
			EndTime = 0f;
			ElapsedTime = 0f;
			try
			{
				Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(output);
				Title = (string)data["Title"];
				Artist = (string)data["Artist"];
				StartTime = Convert.ToSingle(data["StartTime"]);
				EndTime = Convert.ToSingle(data["EndTime"]);
				ElapsedTime = Convert.ToSingle(data["ElapsedTime"]);
				Paused = (string)data["Status"] != "Playing";
				ImageConversion.LoadImage(Icon, Convert.FromBase64String((string)data["ThumbnailBase64"]));
				ValidData = true;
			}
			catch
			{
			}
		}
		finally
		{
			if (proc != null)
			{
				((IDisposable)proc).Dispose();
			}
		}
	}

	private static IEnumerator UpdateDataCoroutine(float delay = 0f)
	{
		yield return (object)new WaitForSeconds(delay);
		UpdateDataAsync();
		yield return null;
	}

	[DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
	internal static extern void keybd_event(uint bVk, uint bScan, uint dwFlags, uint dwExtraInfo);

	internal static void SendKey(VirtualKeyCodes virtualKeyCode)
	{
		keybd_event((uint)virtualKeyCode, 0u, 0u, 0u);
	}

	public static void PreviousTrack()
	{
		((MonoBehaviour)CoroutineManager.instance).StartCoroutine(UpdateDataCoroutine(0.1f));
		ElapsedTime = 0f;
		SendKey(VirtualKeyCodes.PREVIOUS_TRACK);
	}

	public static void PauseTrack()
	{
		Paused = !Paused;
		SendKey(VirtualKeyCodes.PLAY_PAUSE);
	}

	public static void SkipTrack()
	{
		((MonoBehaviour)CoroutineManager.instance).StartCoroutine(UpdateDataCoroutine(0.1f));
		ElapsedTime = 0f;
		SendKey(VirtualKeyCodes.NEXT_TRACK);
	}

	public static void MediaIntegration()
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Expected O, but got Unknown
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		if (!quickSongExists)
		{
			return;
		}
		if ((Object)(object)mediaIcon == (Object)null)
		{
			mediaIcon = GameObject.CreatePrimitive((PrimitiveType)3);
			Object.Destroy((Object)(object)mediaIcon.GetComponent<Collider>());
			if ((Object)(object)mediaIconMaterial == (Object)null)
			{
				mediaIconMaterial = new Material(AssetUtilities.LoadAsset<Shader>("Chams"));
			}
			mediaIcon.GetComponent<Renderer>().material = mediaIconMaterial;
		}
		mediaIcon.transform.localScale = new Vector3(0.25f, 0.25f, 0.01f) * VRRig.LocalRig.scaleFactor;
		mediaIcon.transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.TransformPoint(new Vector3(-0.5f, 0.2f, 1f));
		mediaIcon.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
		if ((Object)(object)mediaText == (Object)null)
		{
			GameObject val = new GameObject("Seralyth_MediaText");
			TextMeshPro orAddComponent = GTExt.GetOrAddComponent<TextMeshPro>(val);
			((Graphic)orAddComponent).color = Color.white;
			((TMP_Text)orAddComponent).fontSize = 0.75f;
			((TMP_Text)orAddComponent).fontStyle = Main.activeFontStyle;
			((TMP_Text)orAddComponent).font = Main.activeFont;
			((TMP_Text)orAddComponent).alignment = (TextAlignmentOptions)513;
			((TMP_Text)orAddComponent).spriteAsset = MediaSpriteSheet;
			((TMP_Text)orAddComponent).margin = new Vector4(0.5f, 0f, 0f, 0f);
			if ((Object)(object)orAddComponent != (Object)null && (Object)(object)((TMP_Text)orAddComponent).fontMaterial != (Object)null)
			{
				((TMP_Text)orAddComponent).fontMaterial.shader = TextMeshProExtensions.TmpShader;
			}
			mediaText = orAddComponent;
		}
		mediaText.transform.localScale = Vector3.one * VRRig.LocalRig.scaleFactor;
		mediaText.transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.TransformPoint(new Vector3(-0.35f, 0.2f, 1f));
		mediaText.transform.LookAt(((Component)Camera.main).transform.position);
		mediaText.transform.Rotate(0f, 180f, 0f);
		Transform transform = mediaText.transform;
		Vector3 position = transform.position;
		Vector3 right = mediaText.transform.right;
		Bounds bounds = ((TMP_Text)mediaText).bounds;
		transform.position = position + right * ((Bounds)(ref bounds)).size.x;
		Main.FollowMenuSettings((TMP_Text)(object)mediaText);
		float num = Mathf.Clamp(ElapsedTime, StartTime, EndTime);
		((TMP_Text)mediaText).text = string.Format("{0} - {1}\n{2}{3}:{4:00} - {5}:{6:00}", Artist, Title, Paused ? "  <sprite name=\"Pause\"> " : "", Mathf.Floor(num / 60f), Mathf.Floor(num % 60f), Mathf.Floor(EndTime / 60f), Mathf.Floor(EndTime % 60f));
		if (Time.time > updateDataDelay)
		{
			updateDataDelay = Time.time + 5f;
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(UpdateDataCoroutine());
		}
		if (!Paused)
		{
			ElapsedTime += Time.deltaTime;
		}
		if (Time.time > inputDelay)
		{
			if (Mathf.Abs(Main.leftJoystick.x) > 0.5f)
			{
				inputDelay = Time.time + 0.5f;
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				if (Main.leftJoystick.x > 0f)
				{
					SkipTrack();
				}
				else
				{
					PreviousTrack();
				}
			}
			if (Main.leftJoystickClick)
			{
				inputDelay = Time.time + 0.5f;
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				PauseTrack();
			}
		}
		Texture2D val2 = (((Object)(object)Icon == (Object)null || !ValidData) ? null : Icon);
		Renderer component = mediaIcon.GetComponent<Renderer>();
		if ((Object)(object)component.material.GetTexture("_MainTex") != (Object)(object)val2)
		{
			component.material.SetTexture("_MainTex", (Texture)(object)val2);
		}
	}

	public static void DisableMediaIntegration()
	{
		quickSongExists = false;
		if ((Object)(object)mediaIcon != (Object)null)
		{
			Object.Destroy((Object)(object)mediaIcon);
		}
		if ((Object)(object)mediaText != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)mediaText).gameObject);
		}
		mediaIcon = null;
		mediaText = null;
	}

	public static void EnableFPC()
	{
		if ((Object)(object)Main.TPC != (Object)null)
		{
			wasenabled = ((Behaviour)((Component)((Component)Main.TPC).gameObject.transform.Find("CM vcam1")).GetComponent<CinemachineVirtualCamera>()).enabled;
		}
	}

	public static void MoveFPC()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)Main.TPC != (Object)null && (!((Object)(object)Main.menu != (Object)null) || XRSettings.isDeviceActive))
		{
			float num = 90f;
			if (((ButtonControl)Keyboard.current.cKey).isPressed)
			{
				Vector2 val = ((InputControl<Vector2>)(object)Mouse.current.scroll).ReadValue();
				zoomFOV += (0f - val.y) * 5f;
				zoomFOV = Mathf.Clamp(zoomFOV, 10f, 90f);
				Main.TPC.fieldOfView = Mathf.Lerp(Main.TPC.fieldOfView, zoomFOV, 0.1f);
			}
			else
			{
				zoomFOV = 35f;
				Main.TPC.fieldOfView = Mathf.Lerp(Main.TPC.fieldOfView, num, 0.1f);
			}
			((Behaviour)((Component)((Component)Main.TPC).gameObject.transform.Find("CM vcam1")).GetComponent<CinemachineVirtualCamera>()).enabled = false;
			((Component)Main.TPC).gameObject.transform.position = (((ButtonControl)Keyboard.current.cKey).isPressed ? Vector3.Lerp(((Component)Main.TPC).transform.position, ((Component)GorillaTagger.Instance.headCollider).transform.position, 0.1f) : ((Component)GorillaTagger.Instance.headCollider).transform.position);
			((Component)Main.TPC).gameObject.transform.rotation = Quaternion.Lerp(((Component)Main.TPC).transform.rotation, ((Component)GorillaTagger.Instance.headCollider).transform.rotation, 0.075f);
		}
	}

	public static void DisableFPC()
	{
		if ((Object)(object)Main.TPC != (Object)null)
		{
			((Component)Main.TPC).GetComponent<Camera>().fieldOfView = 60f;
			((Behaviour)((Component)((Component)Main.TPC).gameObject.transform.Find("CM vcam1")).GetComponent<CinemachineVirtualCamera>()).enabled = wasenabled;
		}
	}

	public static void ForceEnableHands(bool enabled = true)
	{
		if (XRSettings.isDeviceActive)
		{
			ConnectedControllerHandler.Instance.overrideLeftEnable = enabled;
			ConnectedControllerHandler.Instance.overrideRightEnable = enabled;
			ConnectedControllerHandler.Instance.UpdateControllerStates();
		}
	}

	public static void OculusReportMenu()
	{
		if (Main.leftPrimary && !reportMenuToggle)
		{
			GorillaMetaReport component = ((Component)Main.GetObject("Miscellaneous Scripts").transform.Find("MetaReporting")).GetComponent<GorillaMetaReport>();
			((Component)component).gameObject.SetActive(true);
			((Behaviour)component).enabled = true;
			component.StartOverlay(false);
		}
		reportMenuToggle = Main.leftPrimary;
	}

	public static void AcceptTOS()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			GameObject val = Main.GetObject("Miscellaneous Scripts/PrivateUIRoom_HandRays");
			if ((Object)(object)val == (Object)null)
			{
				return;
			}
			HandRayController component = val.GetComponent<HandRayController>();
			PrivateUIRoom component2 = val.GetComponent<PrivateUIRoom>();
			if (!acceptedTOS && component2.inOverlay)
			{
				component.DisableHandRays();
				PrivateUIRoom.StopOverlay();
				component2.overlayForcedSources = (OverlaySource)0;
				if (!TOSPatches.enabled)
				{
					GorillaTagger.Instance.tapHapticStrength = 0.5f;
					GorillaSnapTurn.LoadSettingsFromCache();
					TOSPatches.enabled = true;
				}
				acceptedTOS = true;
			}
			if (val.activeSelf)
			{
				val.SetActive(false);
			}
		}
		catch
		{
		}
	}

	public static IEnumerator RedeemShinyRocks()
	{
		Task<GetPlayerData_Data> newSessionDataTask = KIDManager.TryGetPlayerData(true);
		while (!newSessionDataTask.IsCompleted)
		{
			yield return null;
		}
		if (newSessionDataTask.IsFaulted)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Failed to redeem shiny rocks.");
		}
		GetPlayerData_Data newSessionData = newSessionDataTask.Result;
		if ((int)newSessionData.responseType == 204)
		{
			Task optInTask = KIDManager.Server_OptIn();
			while (!optInTask.IsCompleted)
			{
				yield return null;
			}
			if (optInTask.IsFaulted)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Failed to redeem shiny rocks.");
			}
			NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully redeemed shiny rocks!");
			((CosmeticsController)CosmeticsController.instance).GetCurrencyBalance();
		}
		else
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You have already redeemed the shiny rocks.");
		}
	}

	public static void JoinDiscord()
	{
		Process.Start(Main.serverLink);
	}

	public static void CopyPlayerPosition()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		string text = "Body\n";
		Transform transform = ((Component)GorillaTagger.Instance.bodyCollider).transform;
		text += $"new Vector3({transform.position.x}f, {transform.position.y}f, {transform.position.z}f);";
		text += $"new Quaternion({transform.rotation.x}f, {transform.rotation.y}f, {transform.rotation.z}f, {transform.rotation.w}f);\n\n";
		text += "Head\n";
		transform = ((Component)GorillaTagger.Instance.headCollider).transform;
		text += $"new Vector3({transform.position.x}f, {transform.position.y}f, {transform.position.z}f);";
		text += $"new Quaternion({transform.rotation.x}f, {transform.rotation.y}f, {transform.rotation.z}f, {transform.rotation.w}f);\n\n";
		text += "Left Hand\n";
		transform = ((Component)VRRig.LocalRig.leftHand.rigTarget).transform;
		text += $"new Vector3({transform.position.x}f, {transform.position.y}f, {transform.position.z}f);";
		text += $"new Quaternion({transform.rotation.x}f, {transform.rotation.y}f, {transform.rotation.z}f, {transform.rotation.w}f);\n\n";
		text += "Right Hand\n";
		transform = ((Component)VRRig.LocalRig.rightHand.rigTarget).transform;
		text += $"new Vector3({transform.position.x}f, {transform.position.y}f, {transform.position.z}f);";
		text += $"new Quaternion({transform.rotation.x}f, {transform.rotation.y}f, {transform.rotation.z}f, {transform.rotation.w}f);";
		GUIUtility.systemCopyBuffer = text;
	}

	public static void PhysicalQuitbox()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = Main.GetObject("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/QuitBox");
		physicalQuitBox = GameObject.CreatePrimitive((PrimitiveType)3);
		physicalQuitBox.transform.position = val.transform.position;
		physicalQuitBox.transform.rotation = val.transform.rotation;
		physicalQuitBox.transform.localScale = val.transform.localScale;
		physicalQuitBox.GetComponent<Renderer>().material = CustomBoardManager.BoardMaterial;
		val.SetActive(false);
	}

	public static void DisablePhysicalQuitbox()
	{
		Object.Destroy((Object)(object)physicalQuitBox);
		Main.GetObject("Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/QuitBox").SetActive(true);
	}

	public static void BlockOnMute()
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		bool flag = VRRig.LocalRig.IsTagged();
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig rig) => !rig.IsLocal() && rig.muted))
		{
			if (GameModeUtilities.InfectedList().Count <= 0 || (flag ? (!item.IsTagged()) : item.IsTagged()))
			{
				((Component)item).transform.position = item.syncPos - Vector3.up * 99999f;
			}
		}
	}

	public static void DisablePitchScaling()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal))
		{
			item.voicePitchForRelativeScale = new AnimationCurve((Keyframe[])(object)new Keyframe[2]
			{
				new Keyframe(0f, 1f, 0f, 0f),
				new Keyframe(1f, 1f, 0f, 0f)
			});
		}
	}

	public static void EnablePitchScaling()
	{
		foreach (VRRig item in VRRigCache.ActiveRigs.Where((VRRig vrrig) => !vrrig.isLocal))
		{
			item.voicePitchForRelativeScale = VRRig.LocalRig.voicePitchForRelativeScale;
		}
	}

	public static void DisableMouthMovement()
	{
		VRRig.LocalRig.shouldSendSpeakingLoudness = false;
		LoudnessPatch.enabled = true;
	}

	public static void EnableMouthMovement()
	{
		VRRig.LocalRig.shouldSendSpeakingLoudness = true;
		LoudnessPatch.enabled = false;
	}

	public static void CapFPS(int fps)
	{
		float num = 1f / (float)fps;
		float num2 = Time.realtimeSinceStartup - lastTime;
		if (num2 < num)
		{
			int num3 = Mathf.FloorToInt((num - num2) * 1000f);
			if (num3 > 0)
			{
				Thread.Sleep(num3);
			}
		}
		lastTime = Time.realtimeSinceStartup;
	}

	public static void UncapFPS()
	{
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = int.MaxValue;
	}

	public static void CustomFPS()
	{
		CapFPS(targetCustomFps);
	}

	public static void ChangeCustomFPS(bool positive = true)
	{
		if (positive)
		{
			targetCustomFps += 5;
		}
		else
		{
			targetCustomFps -= 5;
		}
		if (targetCustomFps > 144)
		{
			targetCustomFps = 144;
		}
		if (targetCustomFps < 15)
		{
			targetCustomFps = 15;
		}
		Buttons.GetIndex("Change Custom FPS").overlapText = "Change Custom FPS <color=grey>[</color><color=green>" + targetCustomFps + "</color><color=grey>]</color>";
	}

	public static void PCButtonClick()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (Mouse.current.leftButton.isPressed && (Object)(object)Main.GunPointer == (Object)null)
		{
			Ray val = Main.TPC.ScreenPointToRay(Vector2.op_Implicit(((InputControl<Vector2>)(object)((Pointer)Mouse.current).position).ReadValue()));
			RaycastHit val2 = default(RaycastHit);
			Physics.Raycast(val, ref val2, 512f, Main.NoInvisLayerMask());
			Vector3 valueOrDefault = oldLocalPosition.GetValueOrDefault();
			if (!oldLocalPosition.HasValue)
			{
				valueOrDefault = GorillaTagger.Instance.rightHandTriggerCollider.transform.localPosition;
				oldLocalPosition = valueOrDefault;
			}
			((Behaviour)GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<TransformFollow>()).enabled = false;
			GorillaTagger.Instance.rightHandTriggerCollider.transform.position = ((RaycastHit)(ref val2)).point;
		}
		else
		{
			if (oldLocalPosition.HasValue)
			{
				GorillaTagger.Instance.rightHandTriggerCollider.transform.localPosition = oldLocalPosition.Value;
				oldLocalPosition = null;
			}
			((Behaviour)GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<TransformFollow>()).enabled = true;
		}
	}

	public static void DisablePCButtonClick()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (oldLocalPosition.HasValue)
		{
			GorillaTagger.Instance.rightHandTriggerCollider.transform.localPosition = oldLocalPosition.Value;
			oldLocalPosition = null;
		}
	}

	public static void PCControllerEmulation()
	{
		ControllerInputPoller instance = ControllerInputPoller.instance;
		((ControllerInputPoller)instance).rightControllerPrimaryButton = ((ControllerInputPoller)instance).rightControllerPrimaryButton | UnityInput.GetKey((Key)19);
		ControllerInputPoller instance2 = ControllerInputPoller.instance;
		((ControllerInputPoller)instance2).rightControllerSecondaryButton = ((ControllerInputPoller)instance2).rightControllerSecondaryButton | UnityInput.GetKey((Key)32);
		ControllerInputPoller instance3 = ControllerInputPoller.instance;
		((ControllerInputPoller)instance3).leftControllerPrimaryButton = ((ControllerInputPoller)instance3).leftControllerPrimaryButton | UnityInput.GetKey((Key)20);
		ControllerInputPoller instance4 = ControllerInputPoller.instance;
		((ControllerInputPoller)instance4).leftControllerSecondaryButton = ((ControllerInputPoller)instance4).leftControllerSecondaryButton | UnityInput.GetKey((Key)21);
		ControllerInputPoller instance5 = ControllerInputPoller.instance;
		((ControllerInputPoller)instance5).leftGrab = ((ControllerInputPoller)instance5).leftGrab | UnityInput.GetKey((Key)11);
		ControllerInputPoller instance6 = ControllerInputPoller.instance;
		((ControllerInputPoller)instance6).leftControllerGripFloat = ((ControllerInputPoller)instance6).leftControllerGripFloat + (UnityInput.GetKey((Key)11) ? 1f : 0f);
		ControllerInputPoller instance7 = ControllerInputPoller.instance;
		((ControllerInputPoller)instance7).rightGrab = ((ControllerInputPoller)instance7).rightGrab | UnityInput.GetKey((Key)12);
		ControllerInputPoller instance8 = ControllerInputPoller.instance;
		((ControllerInputPoller)instance8).rightControllerGripFloat = ((ControllerInputPoller)instance8).rightControllerGripFloat + (UnityInput.GetKey((Key)12) ? 1f : 0f);
		ControllerInputPoller instance9 = ControllerInputPoller.instance;
		((ControllerInputPoller)instance9).rightControllerTriggerButton = ((ControllerInputPoller)instance9).rightControllerTriggerButton | UnityInput.GetKey((Key)14);
		ControllerInputPoller instance10 = ControllerInputPoller.instance;
		((ControllerInputPoller)instance10).rightControllerIndexFloat = ((ControllerInputPoller)instance10).rightControllerIndexFloat + (UnityInput.GetKey((Key)14) ? 1f : 0f);
		ControllerInputPoller instance11 = ControllerInputPoller.instance;
		((ControllerInputPoller)instance11).leftControllerTriggerButton = ((ControllerInputPoller)instance11).leftControllerTriggerButton | UnityInput.GetKey((Key)13);
		ControllerInputPoller instance12 = ControllerInputPoller.instance;
		((ControllerInputPoller)instance12).leftControllerIndexFloat = ((ControllerInputPoller)instance12).leftControllerIndexFloat + (UnityInput.GetKey((Key)13) ? 1f : 0f);
		ControllerInputPoller instance13 = ControllerInputPoller.instance;
		((ControllerInputPoller)instance13).rightControllerTriggerButton = ((ControllerInputPoller)instance13).rightControllerTriggerButton | UnityInput.GetKey((Key)14);
		ControllerInputPoller instance14 = ControllerInputPoller.instance;
		((ControllerInputPoller)instance14).rightControllerIndexFloat = ((ControllerInputPoller)instance14).rightControllerIndexFloat + (UnityInput.GetKey((Key)14) ? 1f : 0f);
	}

	public static void DisableAprilFoolsFX()
	{
		try
		{
			AprilFoolsGravityFX[] array = Object.FindObjectsByType<AprilFoolsGravityFX>((FindObjectsSortMode)0);
			AprilFoolsGravityFX[] array2 = array;
			foreach (AprilFoolsGravityFX val in array2)
			{
				val.BackToNormal();
				ObjectExtensions.Destroy((Object)(object)val);
			}
			AprilFoolsGravityFXEnablePatch.enabled = true;
		}
		catch
		{
		}
	}

	public static void EnableAprilFoolsFX()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			AprilFoolsGravityFXEnablePatch.enabled = false;
			new GameObject().AddComponent<AprilFoolsGravityFX>().Start();
		}
		catch
		{
		}
	}

	public static void ConnectToRegion(string region)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		string text = PhotonNetwork.CloudRegion;
		if (!string.IsNullOrEmpty(text))
		{
			text = text.Replace("/*", "");
		}
		if (text != region)
		{
			PhotonNetwork.ConnectToRegion(region);
		}
		NetworkSystem.Instance.currentRegionIndex = Array.IndexOf(NetworkSystem.Instance.regionNames, region);
		NetworkSystemPUN val = (NetworkSystemPUN)NetworkSystem.Instance;
		for (int i = 0; i < val.regionData.Length; i++)
		{
			NetworkRegionInfo val2 = val.regionData[i];
			val2.pingToRegion = ((Array.IndexOf(NetworkSystem.Instance.regionNames, val2) != i) ? 9999 : 0);
		}
	}

	public static void TagLagDetector()
	{
		if (PhotonNetwork.InRoom && !NetworkSystem.Instance.IsMasterClient)
		{
			VRRig rig = PhotonNetwork.MasterClient.VRRig();
			bool flag = rig.GetTruePing() > 1000;
			if (flag)
			{
				if (!lastTagLag)
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=red>TAG LAG</color><color=grey>]</color> There is currently tag lag.");
				}
			}
			else if (lastTagLag)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=green>TAG LAG</color><color=grey>]</color> There is no longer tag lag.");
			}
			lastTagLag = flag;
		}
		else
		{
			if (lastTagLag)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=green>TAG LAG</color><color=grey>]</color> There is no longer tag lag.");
			}
			lastTagLag = false;
		}
	}

	public static void SteamDetector()
	{
		bool flag = VRRigCache.ActiveRigs.Any((VRRig vrrig) => !vrrig.IsLocal() && vrrig.IsSteam());
		if (flag && !lastSteam)
		{
			VRRig rig = VRRigCache.ActiveRigs.First((VRRig vrrig) => !vrrig.IsLocal() && vrrig.IsSteam());
			NotificationManager.SendNotification("<color=grey>[</color><color=red>STEAM</color><color=grey>]</color> " + rig.GetName() + " is on Steam.");
			AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Mods/Safety/steam.ogg", "Audio/Mods/Safety/steam.ogg", delegate(AudioClip clip)
			{
				Main.Play2DAudio(clip, (float)Main.buttonClickVolume / 10f);
			});
		}
		lastSteam = flag;
	}

	public static string RandomRoomName()
	{
		string text;
		do
		{
			text = RandomUtilities.RandomString();
		}
		while (!((GorillaComputer)GorillaComputer.instance).CheckAutoBanListForName(text));
		return text;
	}

	public static void RoomBrowserShow()
	{
		if (!PhotonNetwork.InRoom)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=blue>ROOM BROWSER</color><color=grey>]</color> You must be in a room to browse.");
			return;
		}
		Room currentRoom = PhotonNetwork.CurrentRoom;
		string name = currentRoom.Name;
		int playerCount = currentRoom.PlayerCount;
		int maxPlayers = currentRoom.MaxPlayers;
		bool flag = !currentRoom.IsVisible;
		string text = "";
		Player[] playerList = PhotonNetwork.PlayerList;
		foreach (Player val in playerList)
		{
			string text2 = (val.IsMasterClient ? "<color=yellow>*</color> " : "  ");
			text = text + "\n" + text2 + "<color=white>" + val.NickName + "</color>";
		}
		NotificationManager.SendNotification("<color=grey>[</color><color=blue>ROOM BROWSER</color><color=grey>]</color> " + $"<color=green>{name}</color> ({playerCount}/{maxPlayers})\n" + "Type: " + (flag ? "Public" : "Private") + "\nPlayers:" + text, 8000);
	}

	public static void RoomBrowserJoinByCode()
	{
		if (string.IsNullOrEmpty(Main.lastRoom))
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=blue>ROOM BROWSER</color><color=grey>]</color> No room code saved. Join a room first.");
			return;
		}
		NotificationManager.SendNotification("<color=grey>[</color><color=blue>ROOM BROWSER</color><color=grey>]</color> Joining <color=green>" + Main.lastRoom + "</color>...");
		((PhotonNetworkController)PhotonNetworkController.Instance).AttemptToJoinSpecificRoom(Main.lastRoom, (JoinType)0);
	}

	public static void RoomBrowserCycleMap()
	{
		roomBrowserMapIndex++;
		if (roomBrowserMapIndex >= roomBrowserMaps.Length)
		{
			roomBrowserMapIndex = 0;
		}
		VRRig.LocalRig.PlayHandTapLocal(50, Main.rightHand, 0.4f);
		NotificationManager.SendNotification("<color=grey>[</color><color=blue>ROOM BROWSER</color><color=grey>]</color> Map: <color=green>" + roomBrowserMaps[roomBrowserMapIndex] + "</color>");
	}

	public static void RoomBrowserJoinMap()
	{
		string text = roomBrowserMaps[roomBrowserMapIndex].ToLower();
		GorillaNetworkJoinTrigger joinTriggerForZone = ((GorillaComputer)GorillaComputer.instance).GetJoinTriggerForZone(text);
		if ((Object)(object)joinTriggerForZone == (Object)null)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=blue>ROOM BROWSER</color><color=grey>]</color> Could not find join trigger for <color=green>" + text + "</color>.");
			return;
		}
		NotificationManager.SendNotification("<color=grey>[</color><color=blue>ROOM BROWSER</color><color=grey>]</color> Joining public <color=green>" + roomBrowserMaps[roomBrowserMapIndex] + "</color> room...");
		((PhotonNetworkController)PhotonNetworkController.Instance).AttemptToJoinPublicRoom(joinTriggerForZone, (JoinType)0, (List<ValueTuple<string, string>>)null, false);
	}

	public static void RoomBrowserCreateAndJoin()
	{
		string text = RandomRoomName();
		bool isPublic = true;
		NotificationManager.SendNotification("<color=grey>[</color><color=blue>ROOM BROWSER</color><color=grey>]</color> Creating room <color=green>" + text + "</color>...");
		CreateRoom(text, isPublic, 0, (JoinType)0);
	}

	public static void ScreenshotMode()
	{
		Main.Prompt("How would you like to get your photo?", TakeScreenshot, delegate
		{
			Main.PromptSingleText("What would you like the AI to create? Type a description and it will generate your photo.", delegate
			{
				GenerateAIPhoto(Main.keyboardInput);
			}, "Generate");
		}, "Take a Picture", "AI Create");
	}

	private static void TakeScreenshot()
	{
		bool flag = (Object)(object)Main.menu != (Object)null;
		if (flag)
		{
			Main.menu.SetActive(false);
		}
		NotificationManager.Instance.canvas.SetActive(false);
		if (!Directory.Exists(gtScreenshotsPath))
		{
			Directory.CreateDirectory(gtScreenshotsPath);
		}
		Camera.main.Render();
		ScreenCapture.CaptureScreenshot(Path.Combine(gtScreenshotsPath, "Seralyth_Screenshot_" + DateTime.Now.ToString("HH-mm-ss") + ".png"));
		((MonoBehaviour)CoroutineManager.instance).StartCoroutine(ScreenshotRestore(flag));
	}

	private static void GenerateAIPhoto(string prompt)
	{
		if (string.IsNullOrWhiteSpace(prompt))
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Please enter something for the AI to generate.");
		}
		else
		{
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(GenerateAIPhotoCoroutine(prompt));
		}
	}

	private static IEnumerator GenerateAIPhotoCoroutine(string prompt)
	{
		if (!Directory.Exists(gtScreenshotsPath))
		{
			Directory.CreateDirectory(gtScreenshotsPath);
		}
		NotificationManager.SendNotification("<color=grey>[</color><color=green>AI</color><color=grey>]</color> Generating your photo...");
		string url = "https://image.pollinations.ai/prompt/" + Uri.EscapeDataString(prompt) + "?width=1024&height=1024&nologo=true";
		UnityWebRequest request = UnityWebRequest.Get(url);
		try
		{
			request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
			yield return request.SendWebRequest();
			if ((int)request.result != 1)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Could not generate photo: " + request.error);
				yield break;
			}
			string fileName = $"GT_AI_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
			string fullPath = Path.Combine(gtScreenshotsPath, fileName);
			File.WriteAllBytes(fullPath, request.downloadHandler.data);
			NotificationManager.SendNotification("<color=grey>[</color><color=green>AI</color><color=grey>]</color> Generated photo saved as " + fileName);
		}
		finally
		{
			((IDisposable)request)?.Dispose();
		}
	}

	public static IEnumerator ScreenshotRestore(bool wasMenu)
	{
		yield return (object)new WaitForEndOfFrame();
		NotificationManager.Instance.canvas.SetActive(true);
		NotificationManager.SendNotification("<color=grey>[</color><color=green>SCREENSHOT</color><color=grey>]</color> Screenshot saved!");
		if (wasMenu && (Object)(object)Main.menu != (Object)null)
		{
			Main.menu.SetActive(true);
		}
	}
}
