using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.Rendering;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using Seralyth.Managers;
using Seralyth.Menu;
using Seralyth.Mods;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Seralyth.Classes.Menu;

public class Console : MonoBehaviour
{
	public class ConsoleAsset
	{
		public int bindedToIndex = -1;

		public int bindPlayerActor;

		public readonly string assetName;

		public readonly string assetBundle;

		public readonly GameObject assetObject;

		public GameObject bindedObject;

		public bool modifiedPosition;

		public bool modifiedRotation;

		public bool modifiedLocalPosition;

		public bool modifiedLocalRotation;

		public bool modifiedScale;

		public bool pauseAudioUpdates;

		public int assetId { get; private set; }

		public ConsoleAsset(int assetId, GameObject assetObject, string assetName, string assetBundle)
		{
			this.assetId = assetId;
			this.assetObject = assetObject;
			this.assetName = assetName;
			this.assetBundle = assetBundle;
		}

		public void BindObject(int BindPlayer, int BindPosition)
		{
			bindedToIndex = BindPosition;
			bindPlayerActor = BindPlayer;
			VRRig vRRigFromPlayer = GetVRRigFromPlayer(NetPlayer.op_Implicit(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(bindPlayerActor, false)));
			GameObject val = null;
			switch (bindedToIndex)
			{
			case 0:
				val = vRRigFromPlayer.headMesh;
				break;
			case 1:
				val = ((Component)vRRigFromPlayer.leftHandTransform.parent).gameObject;
				break;
			case 2:
				val = ((Component)vRRigFromPlayer.rightHandTransform.parent).gameObject;
				break;
			case 3:
				val = ((Component)((Component)vRRigFromPlayer).transform.Find("rig/body_pivot")).gameObject;
				break;
			}
			bindedObject = val;
			assetObject.transform.SetParent(bindedObject.transform, false);
		}

		public void SetPosition(Vector3 position)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			modifiedPosition = true;
			assetObject.transform.position = position;
		}

		public void SetRotation(Quaternion rotation)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			modifiedRotation = true;
			assetObject.transform.rotation = rotation;
		}

		public void SetLocalPosition(Vector3 position)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			modifiedLocalPosition = true;
			assetObject.transform.localPosition = position;
		}

		public void SetLocalRotation(Quaternion rotation)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			modifiedLocalRotation = true;
			assetObject.transform.localRotation = rotation;
		}

		public void SetScale(Vector3 scale)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			modifiedScale = true;
			assetObject.transform.localScale = scale;
		}

		public void PlayAudioSource(string objectName, string audioClipName = null)
		{
			AudioSource component = ((Component)(StringUtils.IsNullOrEmpty(objectName) ? assetObject.transform : assetObject.transform.Find(objectName))).GetComponent<AudioSource>();
			if (audioClipName != null)
			{
				component.clip = assetBundlePool[assetBundle].LoadAsset<AudioClip>(audioClipName);
			}
			component.Play();
		}

		public void PlayAudioSourceOneShot(string objectName, string audioClipName = null)
		{
			AudioSource component = ((Component)(StringUtils.IsNullOrEmpty(objectName) ? assetObject.transform : assetObject.transform.Find(objectName))).GetComponent<AudioSource>();
			AudioClip clip = component.clip;
			if (audioClipName != null)
			{
				component.clip = clip;
			}
			component.PlayOneShot(clip);
		}

		public void PlayAnimation(string objectName, string animationClip)
		{
			((Component)(StringUtils.IsNullOrEmpty(objectName) ? assetObject.transform : assetObject.transform.Find(objectName))).GetComponent<Animator>().Play(animationClip);
		}

		public void StopAudioSource(string objectName)
		{
			((Component)(StringUtils.IsNullOrEmpty(objectName) ? assetObject.transform : assetObject.transform.Find(objectName))).GetComponent<AudioSource>().Stop();
		}

		public void ChangeAudioVolume(string objectName, float volume)
		{
			AudioSource val = default(AudioSource);
			if (((Component)(StringUtils.IsNullOrEmpty(objectName) ? assetObject.transform : assetObject.transform.Find(objectName))).TryGetComponent<AudioSource>(ref val))
			{
				val.volume = volume;
			}
			VideoPlayer val2 = default(VideoPlayer);
			if (((Component)(StringUtils.IsNullOrEmpty(objectName) ? assetObject.transform : assetObject.transform.Find(objectName))).TryGetComponent<VideoPlayer>(ref val2))
			{
				val2.SetDirectAudioVolume((ushort)0, volume);
			}
		}

		public void SetVideoURL(string objectName, string urlName)
		{
			((Component)(StringUtils.IsNullOrEmpty(objectName) ? assetObject.transform : assetObject.transform.Find(objectName))).GetComponent<VideoPlayer>().url = urlName;
		}

		public void SetTextureURL(string objectName, string urlName)
		{
			((MonoBehaviour)instance).StartCoroutine(GetTextureResource(urlName, delegate(Texture2D texture)
			{
				((Component)(StringUtils.IsNullOrEmpty(objectName) ? assetObject.transform : assetObject.transform.Find(objectName))).GetComponent<Renderer>().material.SetTexture("_MainTex", (Texture)(object)texture);
			}));
		}

		public void SetColor(string objectName, Color color)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			((Component)(StringUtils.IsNullOrEmpty(objectName) ? assetObject.transform : assetObject.transform.Find(objectName))).GetComponent<Renderer>().material.color = color;
		}

		public void SetAudioURL(string objectName, string urlName)
		{
			pauseAudioUpdates = true;
			((MonoBehaviour)instance).StartCoroutine(GetSoundResource(urlName, delegate(AudioClip audio)
			{
				((Component)(StringUtils.IsNullOrEmpty(objectName) ? assetObject.transform : assetObject.transform.Find(objectName))).GetComponent<AudioSource>().clip = audio;
				pauseAudioUpdates = false;
			}));
		}

		public void DestroyObject()
		{
			Object.Destroy((Object)(object)assetObject);
			consoleAssets.Remove(assetId);
		}
	}

	public static readonly string MenuName = "mrchicken";

	public static readonly string MenuVersion = "10.0.2";

	public static readonly string ConsoleResourceLocation = "SeralythMenu/Console";

	public static readonly string ConsoleSuperAdminIcon = "https://raw.githubusercontent.com/1x1x1x1736/Console-/refs/heads/master/ServerData/icon.png";

	public static readonly string ConsoleAdminIcon = "https://raw.githubusercontent.com/1x1x1x1736/Console-/refs/heads/master/ServerData/crown.png";

	public static readonly string ConsoleVersion = "3.0.8";

	public static Console instance;

	public static bool IsMasterConsole;

	public const string LoadVersionEventKey = "%<CONSOLE>%LoadVersion";

	public const string SyncAssetsEventKey = "%<CONSOLE>%SyncAssets";

	private static readonly Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

	private static readonly Dictionary<string, AudioClip> audios = new Dictionary<string, AudioClip>();

	public const byte ConsoleByte = 68;

	public const string BlockedKey = "ConsoleBlocked";

	public static bool adminIsScaling;

	public static float adminScale = 1f;

	public static VRRig adminRigTarget;

	public static readonly List<Player> excludedCones = new List<Player>();

	public static readonly Dictionary<VRRig, GameObject> conePool = new Dictionary<VRRig, GameObject>();

	public static Material adminConeMaterial;

	public static Texture2D adminConeTexture;

	public static Material adminCrownMaterial;

	public static Texture2D adminCrownTexture;

	private static readonly Dictionary<VRRig, List<int>> indicatorDistanceList = new Dictionary<VRRig, List<int>>();

	private static readonly Dictionary<string, Color> menuColors = new Dictionary<string, Color>
	{
		{
			"seralyth",
			Color32.op_Implicit(new Color32((byte)118, (byte)6, (byte)252, (byte)128))
		},
		{
			"stupid",
			Color32.op_Implicit(new Color32((byte)155, (byte)89, (byte)182, byte.MaxValue))
		},
		{
			"symex",
			Color32.op_Implicit(new Color32((byte)138, (byte)43, (byte)226, byte.MaxValue))
		},
		{
			"colossal",
			Color32.op_Implicit(new Color32((byte)204, (byte)0, byte.MaxValue, byte.MaxValue))
		},
		{
			"ccm",
			Color32.op_Implicit(new Color32((byte)204, (byte)0, byte.MaxValue, byte.MaxValue))
		},
		{
			"untitled",
			Color32.op_Implicit(new Color32((byte)45, (byte)115, (byte)175, byte.MaxValue))
		},
		{
			"genesis",
			Color.blue
		},
		{
			"console",
			Color.gray
		},
		{
			"resurgence",
			Color32.op_Implicit(new Color32((byte)113, (byte)10, (byte)10, byte.MaxValue))
		},
		{
			"grate",
			Color32.op_Implicit(new Color32((byte)195, (byte)145, (byte)110, byte.MaxValue))
		},
		{
			"sodium",
			Color32.op_Implicit(new Color32((byte)220, (byte)208, byte.MaxValue, byte.MaxValue))
		},
		{
			"spectral",
			Color32.op_Implicit(new Color32((byte)164, (byte)94, (byte)229, byte.MaxValue))
		},
		{
			"hamburbur",
			new Color(0.1694782f, 0.1504984f, 0.3584906f)
		}
	};

	public static readonly int TransparentFX = LayerMask.NameToLayer("TransparentFX");

	public static readonly int IgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");

	public static readonly int Zone = LayerMask.NameToLayer("Zone");

	public static readonly int GorillaTrigger = LayerMask.NameToLayer("Gorilla Trigger");

	public static readonly int GorillaBoundary = LayerMask.NameToLayer("Gorilla Boundary");

	public static readonly int GorillaCosmetics = LayerMask.NameToLayer("GorillaCosmetics");

	public static readonly int GorillaParticle = LayerMask.NameToLayer("GorillaParticle");

	public static Coroutine laserCoroutine;

	public static Coroutine smoothTeleportCoroutine;

	public static Coroutine shakeCoroutine;

	public static long isBlocked;

	private static readonly Dictionary<VRRig, float> confirmUsingDelay = new Dictionary<VRRig, float>();

	public static readonly Dictionary<Player, (string, string)> userDictionary = new Dictionary<Player, (string, string)>();

	public static float indicatorDelay = 0f;

	public static bool allowKickSelf;

	public static bool disableFlingSelf;

	public static readonly Dictionary<string, AssetBundle> assetBundlePool = new Dictionary<string, AssetBundle>();

	public static readonly Dictionary<int, ConsoleAsset> consoleAssets = new Dictionary<int, ConsoleAsset>();

	public static bool DisableMenu
	{
		get
		{
			return Main.Lockdown;
		}
		set
		{
			Main.Lockdown = value;
		}
	}

	public static void SendNotification(string text, int sendTime = 1000)
	{
		NotificationManager.SendNotification(text, sendTime);
	}

	public static void TeleportPlayer(Vector3 position)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		GTPlayer.Instance.TeleportTo(World2Player(position), ((Component)GTPlayer.Instance).transform.rotation, true, false);
		((Component)VRRig.LocalRig).transform.position = position;
		Movement.lastPosition = position;
		Main.closePosition = position;
	}

	public static void EnableMod(string mod, bool enable)
	{
		if (!(mod == "Decline Prompt") && !(mod == "Accept Prompt"))
		{
			ButtonInfo index = Buttons.GetIndex(mod);
			if (!index.isTogglable)
			{
				index.method();
				return;
			}
			index.enabled = !enable;
			ToggleMod(index.buttonText);
		}
	}

	public static void ToggleMod(string mod)
	{
		if (!(mod == "Decline Prompt") && !(mod == "Accept Prompt"))
		{
			Main.Toggle(mod);
		}
	}

	public static IEnumerator JoinRoom(string room)
	{
		PhotonNetwork.Disconnect();
		yield return (object)new WaitForSeconds(5f);
		((PhotonNetworkController)PhotonNetworkController.Instance).AttemptToJoinSpecificRoom(room, (JoinType)0);
	}

	public static void ConfirmUsing(string id, string version, string menuName)
	{
		Visuals.ConsoleBeacon(id, version, menuName);
	}

	public static void Log(string text)
	{
		LogManager.Log(text);
	}

	public void Awake()
	{
		instance = this;
		PhotonNetwork.NetworkingClient.EventReceived += EventReceived;
		NetworkSystem obj = NetworkSystem.Instance;
		obj.OnReturnedToSinglePlayer = (DelegateListProcessorPlusMinus<DelegateListProcessor, Action>)(object)obj.OnReturnedToSinglePlayer + (Action)ClearConsoleAssets;
		NetworkSystem obj2 = NetworkSystem.Instance;
		obj2.OnPlayerJoined = (DelegateListProcessorPlusMinus<DelegateListProcessor<NetPlayer>, Action<NetPlayer>>)(object)obj2.OnPlayerJoined + (Action<NetPlayer>)SyncConsoleAssets;
		NetworkSystem obj3 = NetworkSystem.Instance;
		obj3.OnPlayerLeft = (DelegateListProcessorPlusMinus<DelegateListProcessor<NetPlayer>, Action<NetPlayer>>)(object)obj3.OnPlayerLeft + (Action<NetPlayer>)SyncConsoleUsers;
		if (PlayerPrefs.HasKey("ConsoleBlocked"))
		{
			isBlocked = long.Parse(PlayerPrefs.GetString("ConsoleBlocked"));
		}
		NetworkSystem obj4 = NetworkSystem.Instance;
		obj4.OnJoinedRoomEvent = (DelegateListProcessorPlusMinus<DelegateListProcessor, Action>)(object)obj4.OnJoinedRoomEvent + (Action)BlockedCheck;
		if (!Directory.Exists(ConsoleResourceLocation))
		{
			Directory.CreateDirectory(ConsoleResourceLocation);
		}
		((MonoBehaviour)instance).StartCoroutine(DownloadAdminTextures());
		((MonoBehaviour)instance).StartCoroutine(PreloadAssets());
		Log("\n\n     ▄▄·        ▐ ▄ .▄▄ ·       ▄▄▌  ▄▄▄ .\n    ▐█ ▌▪▪     •█▌▐█▐█ ▀. ▪     ██•  ▀▄.▀·\n    ██ ▄▄ ▄█▀▄ ▐█▐▐▌▄▀▀▀█▄ ▄█▀▄ ██▪  ▐▀▀▪▄\n    ▐███▌▐█▌.▐▌██▐█▌▐█▄▪▐█▐█▌.▐▌▐█▌▐▌▐█▄▄▌\n    ·▀▀▀  ▀█▄▀▪▀▀ █▪ ▀▀▀▀  ▀█▄▀▪.▀▀▀  ▀▀▀       \n           Console " + MenuName + " " + ConsoleVersion + "\n     Developed by Seralyth Software\n");
		RenderPipelineAsset currentRenderPipeline = GraphicsSettings.currentRenderPipeline;
		((UniversalRenderPipelineAsset)((currentRenderPipeline is UniversalRenderPipelineAsset) ? currentRenderPipeline : null)).supportsCameraOpaqueTexture = true;
		RenderPipelineAsset currentRenderPipeline2 = GraphicsSettings.currentRenderPipeline;
		((UniversalRenderPipelineAsset)((currentRenderPipeline2 is UniversalRenderPipelineAsset) ? currentRenderPipeline2 : null)).supportsCameraDepthTexture = true;
	}

	public static void LoadConsole()
	{
		GorillaTagger.OnPlayerSpawned((Action)delegate
		{
			LoadConsoleImmediately();
		});
	}

	public static void NoOverlapEvents(string eventName, int id)
	{
		if (!(eventName != "%<CONSOLE>%LoadVersion") && ServerData.VersionToNumber(ConsoleVersion) <= id)
		{
			PhotonNetwork.NetworkingClient.EventReceived -= EventReceived;
			PlayerGameEvents.OnMiscEvent += ConsoleAssetCommunication;
			IsMasterConsole = true;
		}
	}

	public static void ConsoleAssetCommunication(string eventName, int id)
	{
		if (eventName.StartsWith("%<CONSOLE>%SyncAssets"))
		{
			string[] array = eventName.Split("||");
			switch (array[0])
			{
			case "spawn":
			{
				string assetName = array[1];
				string assetBundle = array[2];
				string linkObjectName = array[3];
				bool addGorillaSurfaceOverride = bool.Parse(array[4]);
				((MonoBehaviour)instance).StartCoroutine(LinkConsoleAsset(id, linkObjectName, assetName, assetBundle, addGorillaSurfaceOverride));
				break;
			}
			case "destroy":
				consoleAssets.Remove(id);
				break;
			case "confirmusing":
				ConfirmUsing(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(id, false).UserId, array[1], array[2]);
				break;
			}
		}
	}

	public static void CommunicateConsole(string command, int id, params object[] args)
	{
		string text = "%<CONSOLE>%SyncAssets||" + command;
		if (args.Length != 0)
		{
			text = text + "||" + string.Join("||", args);
		}
		try
		{
			PlayerGameEvents.MiscEvent(text, id);
		}
		catch
		{
		}
	}

	public static IEnumerator LinkConsoleAsset(int id, string linkObjectName, string assetName, string assetBundle, bool addGorillaSurfaceOverride)
	{
		if (!PhotonNetwork.InRoom)
		{
			Log("Attempt to retrieve asset while not in room");
			yield break;
		}
		if ((Object)(object)GameObject.Find(linkObjectName) == (Object)null)
		{
			float timeoutTime = Time.time + 10f;
			while (Time.time < timeoutTime && (Object)(object)GameObject.Find(linkObjectName) == (Object)null)
			{
				yield return null;
			}
		}
		GameObject finalLink = GameObject.Find(linkObjectName);
		if ((Object)(object)finalLink == (Object)null)
		{
			Log("Failed to retrieve asset from link");
		}
		else if (!PhotonNetwork.InRoom)
		{
			Log("Attempt to retrieve asset while not in room");
		}
		else
		{
			consoleAssets.Add(id, new ConsoleAsset(id, ((Component)finalLink.transform.parent).gameObject, assetName, assetBundle));
		}
	}

	public static GameObject LoadConsoleImmediately()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			PlayerGameEvents.MiscEvent("%<CONSOLE>%LoadVersion", ServerData.VersionToNumber(ConsoleVersion));
		}
		catch
		{
		}
		PlayerGameEvents.OnMiscEvent += NoOverlapEvents;
		string text = "seralyth_Console";
		GameObject val = (GameObject)(((object)GameObject.Find(text)) ?? ((object)new GameObject(text)));
		val.AddComponent<Console>();
		if (ServerData.ServerDataEnabled)
		{
			val.AddComponent<ServerData>();
		}
		return val;
	}

	public void OnDisable()
	{
		PhotonNetwork.NetworkingClient.EventReceived -= EventReceived;
	}

	public static string SanitizeFileName(string fileName)
	{
		if (string.IsNullOrWhiteSpace(fileName))
		{
			return null;
		}
		string fileName2 = Path.GetFileName(fileName);
		return string.IsNullOrWhiteSpace(fileName2) ? null : Path.GetInvalidFileNameChars().Aggregate(fileName2, (string current, char c) => current.Replace(c.ToString(), ""));
	}

	public static IEnumerator GetTextureResource(string url, Action<Texture2D> onComplete = null)
	{
		if (!textures.TryGetValue(url, out var texture))
		{
			string fileName = ConsoleResourceLocation + "/" + SanitizeFileName(Uri.UnescapeDataString(url.Split("/")[^1]));
			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}
			Log("Downloading " + fileName);
			using HttpClient client = new HttpClient();
			Task<byte[]> downloadTask = client.GetByteArrayAsync(url);
			while (!downloadTask.IsCompleted)
			{
				yield return null;
			}
			if (downloadTask.Exception != null)
			{
				Log("Failed to download texture: " + downloadTask.Exception);
				yield break;
			}
			byte[] downloadedData = downloadTask.Result;
			Task writeTask = File.WriteAllBytesAsync(fileName, downloadedData);
			while (!writeTask.IsCompleted)
			{
				yield return null;
			}
			if (writeTask.Exception != null)
			{
				Log("Failed to save texture: " + writeTask.Exception);
				yield break;
			}
			Task<byte[]> readTask = File.ReadAllBytesAsync(fileName);
			while (!readTask.IsCompleted)
			{
				yield return null;
			}
			if (readTask.Exception != null)
			{
				Log("Failed to read texture file: " + readTask.Exception);
				yield break;
			}
			byte[] bytes = readTask.Result;
			texture = new Texture2D(2, 2);
			ImageConversion.LoadImage(texture, bytes);
		}
		textures[url] = texture;
		onComplete?.Invoke(texture);
	}

	public static IEnumerator GetSoundResource(string url, Action<AudioClip> onComplete = null)
	{
		if (!audios.TryGetValue(url, out var audio))
		{
			string fileName = ConsoleResourceLocation + "/" + SanitizeFileName(Uri.UnescapeDataString(url.Split("/")[^1]));
			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}
			Log("Downloading " + fileName);
			using HttpClient client = new HttpClient();
			Task<byte[]> downloadTask = client.GetByteArrayAsync(url);
			while (!downloadTask.IsCompleted)
			{
				yield return null;
			}
			if (downloadTask.Exception != null)
			{
				Log("Failed to download texture: " + downloadTask.Exception);
				yield break;
			}
			byte[] downloadedData = downloadTask.Result;
			Task writeTask = File.WriteAllBytesAsync(fileName, downloadedData);
			while (!writeTask.IsCompleted)
			{
				yield return null;
			}
			if (writeTask.Exception != null)
			{
				Log("Failed to save texture: " + writeTask.Exception);
				yield break;
			}
			string filePath = Assembly.GetExecutingAssembly().Location.Split("BepInEx\\")[0] + fileName;
			Log("Loading audio from " + filePath);
			UnityWebRequest audioRequest = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, GetAudioType(GetFileExtension(fileName)));
			try
			{
				yield return audioRequest.SendWebRequest();
				if ((int)audioRequest.result != 1)
				{
					Log("Failed to load audio: " + audioRequest.error);
					yield break;
				}
				audio = DownloadHandlerAudioClip.GetContent(audioRequest);
			}
			finally
			{
				((IDisposable)audioRequest)?.Dispose();
			}
		}
		audios[url] = audio;
		onComplete?.Invoke(audio);
	}

	public static IEnumerator PlaySoundMicrophone(AudioClip sound)
	{
		NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.SourceType = (InputSourceType)1;
		NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.AudioClip = sound;
		NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.RestartRecording(true);
		NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.DebugEchoMode = true;
		yield return (object)new WaitForSeconds(sound.length + 0.4f);
		NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.SourceType = (InputSourceType)0;
		NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.AudioClip = null;
		NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.RestartRecording(true);
		NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.DebugEchoMode = false;
	}

	public static IEnumerator DownloadAdminTextures()
	{
		string fileName = ConsoleResourceLocation + "/icon.png";
		if (File.Exists(fileName))
		{
			File.Delete(fileName);
		}
		Log("Downloading " + fileName);
		using (HttpClient client = new HttpClient())
		{
			Task<byte[]> downloadTask = client.GetByteArrayAsync(ConsoleSuperAdminIcon);
			while (!downloadTask.IsCompleted)
			{
				yield return null;
			}
			if (downloadTask.Exception != null)
			{
				Log("Failed to download texture: " + downloadTask.Exception);
				yield break;
			}
			byte[] downloadedData = downloadTask.Result;
			Task writeTask = File.WriteAllBytesAsync(fileName, downloadedData);
			while (!writeTask.IsCompleted)
			{
				yield return null;
			}
			if (writeTask.Exception != null)
			{
				Log("Failed to save texture: " + writeTask.Exception);
				yield break;
			}
			Task<byte[]> readTask = File.ReadAllBytesAsync(fileName);
			while (!readTask.IsCompleted)
			{
				yield return null;
			}
			if (readTask.Exception != null)
			{
				Log("Failed to read texture file: " + readTask.Exception);
				yield break;
			}
			byte[] bytes = readTask.Result;
			Texture2D texture = new Texture2D(2, 2);
			ImageConversion.LoadImage(texture, bytes);
			adminConeTexture = texture;
		}
		string fileName2 = ConsoleResourceLocation + "/crown.png";
		if (File.Exists(fileName2))
		{
			File.Delete(fileName2);
		}
		Log("Downloading " + fileName2);
		using HttpClient client2 = new HttpClient();
		Task<byte[]> downloadTask2 = client2.GetByteArrayAsync(ConsoleAdminIcon);
		while (!downloadTask2.IsCompleted)
		{
			yield return null;
		}
		if (downloadTask2.Exception != null)
		{
			Log("Failed to download texture: " + downloadTask2.Exception);
			yield break;
		}
		byte[] downloadedData2 = downloadTask2.Result;
		Task writeTask2 = File.WriteAllBytesAsync(fileName2, downloadedData2);
		while (!writeTask2.IsCompleted)
		{
			yield return null;
		}
		if (writeTask2.Exception != null)
		{
			Log("Failed to save texture: " + writeTask2.Exception);
			yield break;
		}
		Task<byte[]> readTask2 = File.ReadAllBytesAsync(fileName2);
		while (!readTask2.IsCompleted)
		{
			yield return null;
		}
		if (readTask2.Exception != null)
		{
			Log("Failed to read texture file: " + readTask2.Exception);
			yield break;
		}
		byte[] bytes2 = readTask2.Result;
		Texture2D texture2 = new Texture2D(2, 2);
		ImageConversion.LoadImage(texture2, bytes2);
		adminCrownTexture = texture2;
	}

	public static string GetFileExtension(string fileName)
	{
		return fileName.ToLower().Split(".")[fileName.Split(".").Length - 1];
	}

	public static AudioType GetAudioType(string extension)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		string text = extension.ToLower();
		if (1 == 0)
		{
		}
		AudioType result = (AudioType)(text switch
		{
			"mp3" => 13, 
			"wav" => 20, 
			"ogg" => 14, 
			"aiff" => 2, 
			_ => 20, 
		});
		if (1 == 0)
		{
		}
		return result;
	}

	public static IEnumerator PreloadAssets()
	{
		UnityWebRequest request = UnityWebRequest.Get("https://raw.githubusercontent.com/1x1x1x1736/Console-/refs/heads/master/ServerData/PreloadedAssets.txt");
		try
		{
			yield return request.SendWebRequest();
			if ((int)request.result != 1)
			{
				yield break;
			}
			string returnText = request.downloadHandler.text;
			string[] array = returnText.Split("\n");
			foreach (string assetBundle in array)
			{
				if (assetBundle.Length > 0)
				{
					((MonoBehaviour)instance).StartCoroutine(PreloadAssetBundle(assetBundle));
				}
			}
		}
		finally
		{
			((IDisposable)request)?.Dispose();
		}
	}

	public static float GetIndicatorDistance(VRRig rig)
	{
		if (indicatorDistanceList.ContainsKey(rig))
		{
			if (indicatorDistanceList[rig][0] == Time.frameCount)
			{
				indicatorDistanceList[rig].Add(Time.frameCount);
				return 0.3f + (float)indicatorDistanceList[rig].Count * 0.5f;
			}
			indicatorDistanceList[rig].Clear();
			indicatorDistanceList[rig].Add(Time.frameCount);
			return 0.3f + (float)indicatorDistanceList[rig].Count * 0.5f;
		}
		indicatorDistanceList.Add(rig, new List<int> { Time.frameCount });
		return 0.8f;
	}

	public void Update()
	{
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Expected O, but got Unknown
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Expected O, but got Unknown
		if (IsMasterConsole)
		{
			return;
		}
		if (PhotonNetwork.InRoom)
		{
			try
			{
				AdminTagGUI.UpdateAdminTags();
				PlayerTagManager.UpdatePlayerTags();
				List<VRRig> list = new List<VRRig>();
				foreach (KeyValuePair<VRRig, GameObject> item in from _003C_003Eh__TransparentIdentifier0 in conePool.Select(delegate(KeyValuePair<VRRig, GameObject> nametag)
					{
						KeyValuePair<VRRig, GameObject> keyValuePair = nametag;
						NetPlayer creator = keyValuePair.Key.Creator;
						return new
						{
							nametag = nametag,
							nametagPlayer = ((creator != null) ? creator.GetPlayerRef() : null)
						};
					})
					where !VRRigCache.ActiveRigs.Contains(_003C_003Eh__TransparentIdentifier0.nametag.Key) || _003C_003Eh__TransparentIdentifier0.nametagPlayer == null || !ServerData.Administrators.ContainsKey(ServerData.NormalizeId(_003C_003Eh__TransparentIdentifier0.nametagPlayer.UserId)) || excludedCones.Contains(_003C_003Eh__TransparentIdentifier0.nametagPlayer)
					select _003C_003Eh__TransparentIdentifier0.nametag)
				{
					Object.Destroy((Object)(object)item.Value);
					list.Add(item.Key);
				}
				foreach (VRRig item2 in list)
				{
					conePool.Remove(item2);
				}
				string value;
				bool flag = ServerData.Administrators.TryGetValue(ServerData.NormalizeId(PhotonNetwork.LocalPlayer.UserId), out value) && ServerData.SuperAdministrators.Contains(value);
				Player[] playerListOthers = PhotonNetwork.PlayerListOthers;
				foreach (Player val in playerListOthers)
				{
					if (!ServerData.Administrators.TryGetValue(ServerData.NormalizeId(val.UserId), out var value2) || (!flag && excludedCones.Contains(val)))
					{
						continue;
					}
					VRRig vRRigFromPlayer = GetVRRigFromPlayer(NetPlayer.op_Implicit(val));
					if ((Object)(object)vRRigFromPlayer == (Object)null)
					{
						continue;
					}
					if (!conePool.TryGetValue(vRRigFromPlayer, out var value3))
					{
						value3 = GameObject.CreatePrimitive((PrimitiveType)3);
						Object.Destroy((Object)(object)value3.GetComponent<Collider>());
						if ((Object)(object)adminCrownMaterial == (Object)null)
						{
							adminCrownMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
							{
								mainTexture = (Texture)(object)adminCrownTexture
							};
							adminCrownMaterial.SetFloat("_Surface", 1f);
							adminCrownMaterial.SetFloat("_Blend", 0f);
							adminCrownMaterial.SetFloat("_SrcBlend", 5f);
							adminCrownMaterial.SetFloat("_DstBlend", 10f);
							adminCrownMaterial.SetFloat("_ZWrite", 0f);
							adminCrownMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
							adminCrownMaterial.renderQueue = 3000;
						}
						if ((Object)(object)adminConeMaterial == (Object)null)
						{
							adminConeMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
							{
								mainTexture = (Texture)(object)adminConeTexture
							};
							adminConeMaterial.SetFloat("_Surface", 1f);
							adminConeMaterial.SetFloat("_Blend", 0f);
							adminConeMaterial.SetFloat("_SrcBlend", 5f);
							adminConeMaterial.SetFloat("_DstBlend", 10f);
							adminConeMaterial.SetFloat("_ZWrite", 0f);
							adminConeMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
							adminConeMaterial.renderQueue = 3000;
						}
						value3.GetComponent<Renderer>().material = (ServerData.SuperAdministrators.Contains(value2) ? adminConeMaterial : adminCrownMaterial);
						conePool.Add(vRRigFromPlayer, value3);
					}
					value3.GetComponent<Renderer>().material.color = vRRigFromPlayer.playerColor;
					value3.GetComponent<Renderer>().enabled = ExplorerConsole.ShowCrowns;
					value3.transform.localScale = new Vector3(0.4f, 0.4f, 0.01f) * vRRigFromPlayer.scaleFactor;
					value3.transform.position = Visuals.GetNameTagTransform(vRRigFromPlayer).position + Visuals.GetNameTagTransform(vRRigFromPlayer).up * (GetIndicatorDistance(vRRigFromPlayer) * vRRigFromPlayer.scaleFactor);
					value3.transform.LookAt(((Component)GorillaTagger.Instance.headCollider).transform.position);
				}
				if (adminIsScaling && (Object)(object)adminRigTarget != (Object)null)
				{
					adminRigTarget.NativeScale = adminScale;
					if (Mathf.Approximately(adminScale, 1f))
					{
						adminIsScaling = false;
					}
				}
			}
			catch
			{
			}
		}
		else
		{
			AdminTagGUI.CleanupAll();
			PlayerTagManager.CleanupAll();
			if (conePool.Count > 0)
			{
				foreach (KeyValuePair<VRRig, GameObject> item3 in conePool)
				{
					Object.Destroy((Object)(object)item3.Value);
				}
				conePool.Clear();
			}
		}
		SanitizeConsoleAssets();
	}

	public static void TeleportToMap(string mapName)
	{
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		string text = "";
		string text2 = "";
		if (mapName == "Forest")
		{
			text = "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/TreeRoomSpawnForestZone";
			text2 = "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Forest, Tree Exit";
		}
		if (mapName == "City")
		{
			text = "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/ForestToCity";
			text2 = "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - City Front";
		}
		if (mapName == "Canyons")
		{
			text = "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/ForestCanyonTransition";
			text2 = "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Canyon";
		}
		if (mapName == "Clouds")
		{
			text = "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/CityToSkyJungle";
			text2 = "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Clouds From Computer";
		}
		if (mapName == "Caves")
		{
			text = "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/ForestToCave";
			text2 = "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Cave";
		}
		if (mapName == "Beach")
		{
			text = "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/BeachToForest";
			text2 = "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Beach for Computer";
		}
		if (mapName == "Mountains")
		{
			text = "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/CityToMountain";
			text2 = "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Mountain";
		}
		if (mapName == "Basement")
		{
			text = "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/CityToBasement";
			text2 = "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Basement For Computer";
		}
		if (mapName == "Metropolis")
		{
			text = "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/MetropolisOnly";
			text2 = "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Metropolis from Computer";
		}
		if (mapName == "Arcade")
		{
			text = "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/CityToArcade";
			text2 = "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - City frm Arcade";
		}
		if (mapName == "Critters")
		{
			text = "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/CityCrittersTransition";
			text2 = "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - City from Critters";
		}
		if (mapName == "Rotating")
		{
			text = "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/CityToRotating";
			text2 = "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Rotating Map";
		}
		if (mapName == "Bayou")
		{
			text = "Environment Objects/TriggerZones_Prefab/ZoneTransitions_Prefab/Regional Transition/BayouOnly";
			text2 = "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - BayouComputer2";
		}
		if (mapName == "Virtual Stump")
		{
			VirtualStumpTeleporter component = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/VirtualStump_HeadsetTeleporter/TeleporterTrigger").GetComponent<VirtualStumpTeleporter>();
			((Component)((Component)component).gameObject.transform.parent.parent.parent.parent.parent.parent).gameObject.SetActive(true);
			((Component)((Component)component).gameObject.transform.parent.parent.parent.parent).gameObject.SetActive(true);
			component.TeleportPlayer();
			return;
		}
		if (mapName == "Lava Forest")
		{
			text = "Environment Objects/05Maze_PersistentObjects/GhostReactorElevatorManager/VIMForestLavaElevator/Triggers/VIMExp1_SetZoneTrigger";
			text2 = "Environment Objects/05Maze_PersistentObjects/GhostReactorElevatorManager/VIMForestLavaElevator/Triggers/JoinRoomTrigger";
		}
		GameObject obj = GameObject.Find(text);
		if (obj != null)
		{
			GorillaSetZoneTrigger component2 = obj.GetComponent<GorillaSetZoneTrigger>();
			if (component2 != null)
			{
				((GorillaTriggerBox)component2).OnBoxTriggered();
			}
		}
		GameObject obj2 = GameObject.Find(text2);
		if (obj2 != null)
		{
			obj2.SetActive(false);
		}
		GameObject obj3 = GameObject.Find(text);
		TeleportPlayer((obj3 != null) ? obj3.transform.position : ((Component)VRRig.LocalRig).transform.position);
	}

	public static int NoInvisLayerMask()
	{
		return ~((1 << TransparentFX) | (1 << IgnoreRaycast) | (1 << Zone) | (1 << GorillaTrigger) | (1 << GorillaBoundary) | (1 << GorillaCosmetics) | (1 << GorillaParticle));
	}

	public static Color GetMenuTypeName(string type)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Color value;
		return menuColors.TryGetValue(type, out value) ? value : Color.red;
	}

	public static Vector3 World2Player(Vector3 world)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		return world - ((Component)GorillaTagger.Instance.bodyCollider).transform.position + ((Component)GorillaTagger.Instance).transform.position;
	}

	public static VRRig GetVRRigFromPlayer(NetPlayer p)
	{
		return GorillaGameManager.StaticFindRigForPlayer(p);
	}

	public static NetPlayer GetPlayerFromID(string id)
	{
		return NetPlayer.op_Implicit(((IEnumerable<Player>)PhotonNetwork.PlayerList).FirstOrDefault((Func<Player, bool>)((Player player) => player.UserId == id)));
	}

	public static Player GetMasterAdministrator()
	{
		return (from player in PhotonNetwork.PlayerList
			where ServerData.Administrators.ContainsKey(ServerData.NormalizeId(player.UserId))
			orderby player.ActorNumber
			select player).FirstOrDefault();
	}

	public static void LightningStrike(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		Color cyan = Color.cyan;
		GameObject val = new GameObject("LightningOuter");
		LineRenderer val2 = val.AddComponent<LineRenderer>();
		val2.startColor = cyan;
		val2.endColor = cyan;
		val2.startWidth = 0.25f;
		val2.endWidth = 0.25f;
		val2.positionCount = 5;
		val2.useWorldSpace = true;
		Vector3 val3 = position;
		for (int i = 0; i < 5; i++)
		{
			VRRig.LocalRig.PlayHandTapLocal(68, false, 0.25f);
			VRRig.LocalRig.PlayHandTapLocal(68, true, 0.25f);
			val2.SetPosition(i, val3);
			val3 += new Vector3(Random.Range(-5f, 5f), 5f, Random.Range(-5f, 5f));
		}
		((Renderer)val2).material.shader = Shader.Find("GUI/Text Shader");
		Object.Destroy((Object)(object)val, 2f);
		GameObject val4 = new GameObject("LightningInner");
		LineRenderer val5 = val4.AddComponent<LineRenderer>();
		val5.startColor = Color.white;
		val5.endColor = Color.white;
		val5.startWidth = 0.15f;
		val5.endWidth = 0.15f;
		val5.positionCount = 5;
		val5.useWorldSpace = true;
		for (int j = 0; j < 5; j++)
		{
			val5.SetPosition(j, val2.GetPosition(j));
		}
		((Renderer)val5).material.shader = Shader.Find("GUI/Text Shader");
		((Renderer)val5).material.renderQueue = ((Renderer)val2).material.renderQueue + 1;
		Object.Destroy((Object)(object)val4, 2f);
	}

	public static IEnumerator RenderLaser(bool rightHand, VRRig rigTarget)
	{
		float stoplasar = Time.time + 0.2f;
		RaycastHit Ray = default(RaycastHit);
		while (Time.time < stoplasar)
		{
			rigTarget.PlayHandTapLocal(18, !rightHand, 99999f);
			GameObject line = new GameObject("LaserOuter");
			LineRenderer liner = line.AddComponent<LineRenderer>();
			liner.startColor = Color.red;
			liner.endColor = Color.red;
			liner.startWidth = 0.15f + Mathf.Sin(Time.time * 5f) * 0.01f;
			liner.endWidth = liner.startWidth;
			liner.positionCount = 2;
			liner.useWorldSpace = true;
			Vector3 startPos = (rightHand ? rigTarget.rightHandTransform.position : rigTarget.leftHandTransform.position) + (rightHand ? rigTarget.rightHandTransform.up : rigTarget.leftHandTransform.up) * 0.1f;
			Vector3 endPos = Vector3.zero;
			Vector3 dir = (rightHand ? rigTarget.rightHandTransform.right : (-rigTarget.leftHandTransform.right));
			try
			{
				Physics.Raycast(startPos + dir / 3f, dir, ref Ray, 512f, NoInvisLayerMask());
				endPos = ((RaycastHit)(ref Ray)).point;
				if (endPos == Vector3.zero)
				{
					endPos = startPos + dir * 512f;
				}
			}
			catch
			{
			}
			liner.SetPosition(0, startPos + dir * 0.1f);
			liner.SetPosition(1, endPos);
			((Renderer)liner).material.shader = Shader.Find("GUI/Text Shader");
			Object.Destroy((Object)(object)line, Time.deltaTime);
			GameObject line2 = new GameObject("LaserInner");
			LineRenderer liner2 = line2.AddComponent<LineRenderer>();
			liner2.startColor = Color.white;
			liner2.endColor = Color.white;
			liner2.startWidth = 0.1f;
			liner2.endWidth = 0.1f;
			liner2.positionCount = 2;
			liner2.useWorldSpace = true;
			liner2.SetPosition(0, startPos + dir * 0.1f);
			liner2.SetPosition(1, endPos);
			((Renderer)liner2).material.shader = Shader.Find("GUI/Text Shader");
			((Renderer)liner2).material.renderQueue = ((Renderer)liner).material.renderQueue + 1;
			Object.Destroy((Object)(object)line2, Time.deltaTime);
			GameObject whiteParticle = GameObject.CreatePrimitive((PrimitiveType)0);
			Object.Destroy((Object)(object)whiteParticle, 2f);
			Object.Destroy((Object)(object)whiteParticle.GetComponent<Collider>());
			whiteParticle.GetComponent<Renderer>().material.color = Color.yellow;
			whiteParticle.AddComponent<Rigidbody>().linearVelocity = new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(0f, 7.5f), Random.Range(-7.5f, 7.5f));
			whiteParticle.transform.position = endPos + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
			whiteParticle.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
			yield return null;
		}
	}

	public static IEnumerator ControllerPress(string buttton, float value, float duration)
	{
		float stop = Time.time + duration;
		while (Time.time < stop)
		{
			switch (buttton)
			{
			case "lGrip":
				((ControllerInputPoller)ControllerInputPoller.instance).leftControllerGripFloat = value;
				break;
			case "rGrip":
				((ControllerInputPoller)ControllerInputPoller.instance).rightControllerGripFloat = value;
				break;
			case "lIndex":
				((ControllerInputPoller)ControllerInputPoller.instance).leftControllerIndexFloat = value;
				break;
			case "rIndex":
				((ControllerInputPoller)ControllerInputPoller.instance).rightControllerIndexFloat = value;
				break;
			case "lPrimary":
				((ControllerInputPoller)ControllerInputPoller.instance).leftControllerPrimaryButtonTouch = value > 0.33f;
				((ControllerInputPoller)ControllerInputPoller.instance).leftControllerPrimaryButton = value > 0.66f;
				break;
			case "lSecondary":
				((ControllerInputPoller)ControllerInputPoller.instance).leftControllerSecondaryButtonTouch = value > 0.33f;
				((ControllerInputPoller)ControllerInputPoller.instance).leftControllerSecondaryButton = value > 0.66f;
				break;
			case "rPrimary":
				((ControllerInputPoller)ControllerInputPoller.instance).rightControllerPrimaryButtonTouch = value > 0.33f;
				((ControllerInputPoller)ControllerInputPoller.instance).rightControllerPrimaryButton = value > 0.66f;
				break;
			case "rSecondary":
				((ControllerInputPoller)ControllerInputPoller.instance).rightControllerSecondaryButtonTouch = value > 0.33f;
				((ControllerInputPoller)ControllerInputPoller.instance).rightControllerSecondaryButton = value > 0.66f;
				break;
			}
			yield return null;
		}
	}

	public static IEnumerator SmoothTeleport(Vector3 position, float time)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		float startTime = Time.time;
		Vector3 startPosition = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
		while (Time.time < startTime + time)
		{
			TeleportPlayer(Vector3.Lerp(startPosition, position, (Time.time - startTime) / time));
			GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
			yield return null;
		}
		smoothTeleportCoroutine = null;
	}

	public static IEnumerator AssetSmoothTeleport(ConsoleAsset asset, Vector3? position, Quaternion? rotation, float time)
	{
		float startTime = Time.time;
		Vector3 startPosition = asset.assetObject.transform.position;
		Quaternion startRotation = asset.assetObject.transform.rotation;
		Vector3 targetPosition = position ?? startPosition;
		Quaternion targetRotation = rotation ?? startRotation;
		while (Time.time < startTime + time)
		{
			asset.SetPosition(Vector3.Lerp(startPosition, targetPosition, (Time.time - startTime) / time));
			asset.SetRotation(Quaternion.Lerp(startRotation, targetRotation, (Time.time - startTime) / time));
			yield return null;
		}
	}

	public static IEnumerator Shake(float strength, float time, bool constant)
	{
		float startTime = Time.time;
		while (Time.time < startTime + time)
		{
			float shakePower = (constant ? strength : (strength * (1f - (Time.time - startTime) / time)));
			TeleportPlayer(((Component)GorillaTagger.Instance.bodyCollider).transform.position + new Vector3(Random.Range(0f - shakePower, shakePower), Random.Range(0f - shakePower, shakePower), Random.Range(0f - shakePower, shakePower)));
			yield return null;
		}
		shakeCoroutine = null;
	}

	public static void BlockedCheck()
	{
		if (isBlocked > DateTime.UtcNow.Ticks / 10000000 && PhotonNetwork.InRoom)
		{
			NetworkSystem.Instance.ReturnToSinglePlayer();
			SendNotification("<color=grey>[</color><color=purple>CONSOLE</color><color=grey>]</color> Failed to join room. You can join rooms in " + (isBlocked - DateTime.UtcNow.Ticks / 10000000) + "s.", 10000);
		}
	}

	public static void EventReceived(EventData data)
	{
		try
		{
			if (data.Code == 68)
			{
				Player player = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false);
				object[] array = ((data.CustomData == null) ? new object[0] : ((object[])data.CustomData));
				string command = ((array.Length != 0) ? ((string)array[0]) : "");
				BlockedCheck();
				HandleConsoleEvent(player, array, command);
			}
		}
		catch
		{
		}
	}

	private static void HandleConsoleEvent(Player sender, object[] args, string command)
	{
		//IL_0c0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f76: Unknown result type (might be due to invalid IL or missing references)
		//IL_1449: Unknown result type (might be due to invalid IL or missing references)
		//IL_1450: Expected O, but got Unknown
		//IL_1496: Unknown result type (might be due to invalid IL or missing references)
		//IL_14a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_1505: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a72: Unknown result type (might be due to invalid IL or missing references)
		//IL_24c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_2518: Unknown result type (might be due to invalid IL or missing references)
		//IL_2272: Unknown result type (might be due to invalid IL or missing references)
		//IL_2277: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fc3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fc8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dd9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dde: Unknown result type (might be due to invalid IL or missing references)
		//IL_11be: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d91: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d96: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ee3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1efc: Unknown result type (might be due to invalid IL or missing references)
		//IL_188a: Unknown result type (might be due to invalid IL or missing references)
		//IL_18a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_18c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1605: Unknown result type (might be due to invalid IL or missing references)
		//IL_108f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e69: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e82: Unknown result type (might be due to invalid IL or missing references)
		//IL_18f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1919: Unknown result type (might be due to invalid IL or missing references)
		//IL_161f: Unknown result type (might be due to invalid IL or missing references)
		//IL_15eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d49: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e21: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e26: Unknown result type (might be due to invalid IL or missing references)
		//IL_255e: Unknown result type (might be due to invalid IL or missing references)
		//IL_256a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1949: Unknown result type (might be due to invalid IL or missing references)
		//IL_196c: Unknown result type (might be due to invalid IL or missing references)
		//IL_164b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1650: Unknown result type (might be due to invalid IL or missing references)
		//IL_163c: Unknown result type (might be due to invalid IL or missing references)
		//IL_112d: Unknown result type (might be due to invalid IL or missing references)
		//IL_127c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_168b: Unknown result type (might be due to invalid IL or missing references)
		//IL_167c: Unknown result type (might be due to invalid IL or missing references)
		bool flag = sender == PhotonNetwork.LocalPlayer;
		bool flag2 = ServerData.IsOwner(sender.UserId);
		string value;
		bool flag3 = ServerData.Administrators.TryGetValue(ServerData.NormalizeId(sender.UserId), out value);
		bool flag4 = flag2 || flag3;
		bool flag5 = ServerData.IsModerator(sender.UserId);
		bool flag6 = command?.StartsWith("asset-") ?? false;
		if (flag4 || flag2 || (flag6 ? flag5 : flag))
		{
			bool flag7 = flag4 && ServerData.SuperAdministrators.Contains(value);
			switch (command)
			{
			case "staff-sync":
				if (flag4 || flag2)
				{
					ServerData.MergeStaffLists((string)args[1]);
				}
				break;
			case "kick":
			{
				NetPlayer playerFromID = GetPlayerFromID((string)args[1]);
				LightningStrike(GetVRRigFromPlayer(playerFromID).headMesh.transform.position);
				if ((allowKickSelf || !ServerData.Administrators.ContainsKey(ServerData.NormalizeId(playerFromID.UserId)) || flag7) && (string)args[1] == PhotonNetwork.LocalPlayer.UserId)
				{
					NetworkSystem.Instance.ReturnToSinglePlayer();
				}
				break;
			}
			case "silkick":
			{
				NetPlayer playerFromID = GetPlayerFromID((string)args[1]);
				if ((allowKickSelf || !ServerData.Administrators.ContainsKey(ServerData.NormalizeId(playerFromID.UserId)) || flag7) && (string)args[1] == PhotonNetwork.LocalPlayer.UserId)
				{
					NetworkSystem.Instance.ReturnToSinglePlayer();
				}
				break;
			}
			case "join":
				if (!ServerData.Administrators.ContainsKey(ServerData.NormalizeId(PhotonNetwork.LocalPlayer.UserId)) || flag7)
				{
					((MonoBehaviour)instance).StartCoroutine(JoinRoom((string)args[1]));
				}
				break;
			case "kickall":
			{
				Player[] array5 = (ServerData.Administrators.ContainsKey(ServerData.NormalizeId(PhotonNetwork.LocalPlayer.UserId)) ? PhotonNetwork.PlayerListOthers : PhotonNetwork.PlayerList);
				foreach (Player val8 in array5)
				{
					LightningStrike(GetVRRigFromPlayer(NetPlayer.op_Implicit(val8)).headMesh.transform.position);
				}
				if (!ServerData.Administrators.ContainsKey(ServerData.NormalizeId(PhotonNetwork.LocalPlayer.UserId)))
				{
					NetworkSystem.Instance.ReturnToSinglePlayer();
				}
				break;
			}
			case "block":
				if (flag7)
				{
					long value2 = (long)args[1];
					value2 = Math.Clamp(value2, 1L, flag7 ? 36000 : 1800);
					PlayerPrefs.SetString("ConsoleBlocked", (DateTime.UtcNow.Ticks / 10000000 + value2).ToString());
					PlayerPrefs.Save();
					isBlocked = DateTime.UtcNow.Ticks / 10000000 + value2;
					NetworkSystem.Instance.ReturnToSinglePlayer();
				}
				break;
			case "crash":
				if (flag7)
				{
					Application.Quit();
				}
				break;
			case "isusing":
				ExecuteCommand("confirmusing", sender.ActorNumber, MenuVersion, MenuName);
				break;
			case "sleep":
				if (!ServerData.Administrators.ContainsKey(ServerData.NormalizeId(PhotonNetwork.LocalPlayer.UserId)) || flag7)
				{
					Thread.Sleep((int)args[1]);
				}
				break;
			case "vibrate":
				switch ((int)args[1])
				{
				case 1:
					GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tagHapticStrength, Mathf.Clamp((float)args[2], 0f, 10f));
					break;
				case 2:
					GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tagHapticStrength, Mathf.Clamp((float)args[2], 0f, 10f));
					break;
				case 3:
					GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tagHapticStrength, Mathf.Clamp((float)args[2], 0f, 10f));
					GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tagHapticStrength, Mathf.Clamp((float)args[2], 0f, 10f));
					break;
				}
				break;
			case "forceenable":
				if (flag7)
				{
					string mod = (string)args[1];
					bool enable = (bool)args[2];
					EnableMod(mod, enable);
				}
				break;
			case "toggle":
				if (flag7)
				{
					string mod2 = (string)args[1];
					ToggleMod(mod2);
				}
				break;
			case "togglemenu":
				DisableMenu = (bool)args[1];
				break;
			case "tp":
				if (!disableFlingSelf || flag7 || !ServerData.Administrators.ContainsKey(ServerData.NormalizeId(PhotonNetwork.LocalPlayer.UserId)))
				{
					TeleportPlayer((Vector3)args[1]);
				}
				break;
			case "map":
				TeleportToMap((string)args[1]);
				break;
			case "nocone":
				if ((bool)args[1])
				{
					excludedCones.Add(sender);
				}
				else
				{
					excludedCones.Remove(sender);
				}
				break;
			case "vel":
				if (!disableFlingSelf || flag7 || !ServerData.Administrators.ContainsKey(ServerData.NormalizeId(PhotonNetwork.LocalPlayer.UserId)))
				{
					GorillaTagger.Instance.rigidbody.linearVelocity = (Vector3)args[1];
				}
				break;
			case "controller":
				((MonoBehaviour)instance).StartCoroutine(ControllerPress((string)args[1], (float)args[2], (float)args[3]));
				break;
			case "tpsmooth":
			case "smoothtp":
				if (smoothTeleportCoroutine != null)
				{
					((MonoBehaviour)instance).StopCoroutine(smoothTeleportCoroutine);
				}
				if ((float)args[2] > 0f)
				{
					smoothTeleportCoroutine = ((MonoBehaviour)instance).StartCoroutine(SmoothTeleport((Vector3)args[1], (float)args[2]));
				}
				break;
			case "shake":
				if (shakeCoroutine != null)
				{
					((MonoBehaviour)instance).StopCoroutine(shakeCoroutine);
				}
				shakeCoroutine = ((MonoBehaviour)instance).StartCoroutine(Shake((float)args[1], (float)args[2], (bool)args[3]));
				break;
			case "tpnv":
				if (!disableFlingSelf || flag7 || !ServerData.Administrators.ContainsKey(ServerData.NormalizeId(PhotonNetwork.LocalPlayer.UserId)))
				{
					TeleportPlayer((Vector3)args[1]);
					GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
				}
				break;
			case "scale":
			{
				VRRig vRRigFromPlayer = GetVRRigFromPlayer(NetPlayer.op_Implicit(sender));
				adminIsScaling = true;
				adminRigTarget = vRRigFromPlayer;
				adminScale = (float)args[1];
				break;
			}
			case "cosmetic":
				AccessTools.Method(((object)GetVRRigFromPlayer(NetPlayer.op_Implicit(sender))).GetType(), "AddCosmetic", (Type[])null, (Type[])null).Invoke(GetVRRigFromPlayer(NetPlayer.op_Implicit(sender)), new object[1] { (string)args[1] });
				GetVRRigFromPlayer(NetPlayer.op_Implicit(sender)).RefreshCosmetics();
				break;
			case "cosmetics":
			{
				string[] array4 = (string[])args[1];
				foreach (string text in array4)
				{
					AccessTools.Method(((object)GetVRRigFromPlayer(NetPlayer.op_Implicit(sender))).GetType(), "AddCosmetic", (Type[])null, (Type[])null).Invoke(GetVRRigFromPlayer(NetPlayer.op_Implicit(sender)), new object[1] { text });
				}
				GetVRRigFromPlayer(NetPlayer.op_Implicit(sender)).RefreshCosmetics();
				break;
			}
			case "strike":
				LightningStrike((Vector3)args[1]);
				break;
			case "laser":
				if (laserCoroutine != null)
				{
					((MonoBehaviour)instance).StopCoroutine(laserCoroutine);
				}
				if ((bool)args[1])
				{
					laserCoroutine = ((MonoBehaviour)instance).StartCoroutine(RenderLaser((bool)args[2], GetVRRigFromPlayer(NetPlayer.op_Implicit(sender))));
				}
				break;
			case "notify":
				SendNotification("<color=grey>[</color><color=red>ANNOUNCE</color><color=grey>]</color> " + (string)args[1], 5000);
				break;
			case "lr":
			{
				GameObject val2 = new GameObject("Line");
				LineRenderer val3 = val2.AddComponent<LineRenderer>();
				Color val4 = default(Color);
				((Color)(ref val4))._002Ector((float)args[1], (float)args[2], (float)args[3], (float)args[4]);
				val3.startColor = val4;
				val3.endColor = val4;
				val3.startWidth = (float)args[5];
				val3.endWidth = (float)args[5];
				val3.positionCount = 2;
				val3.useWorldSpace = true;
				val3.SetPosition(0, (Vector3)args[6]);
				val3.SetPosition(1, (Vector3)args[7]);
				((Renderer)val3).material.shader = Shader.Find("GUI/Text Shader");
				Object.Destroy((Object)(object)val2, (float)args[8]);
				break;
			}
			case "platf":
			{
				GameObject val7 = GameObject.CreatePrimitive((PrimitiveType)3);
				Object.Destroy((Object)(object)val7, (args.Length > 8) ? ((float)args[8]) : 60f);
				if (args.Length > 4)
				{
					if ((float)args[7] == 0f)
					{
						Object.Destroy((Object)(object)val7.GetComponent<Renderer>());
					}
					else
					{
						val7.GetComponent<Renderer>().material.color = new Color((float)args[4], (float)args[5], (float)args[6], (float)args[7]);
					}
				}
				else
				{
					val7.GetComponent<Renderer>().material.color = Color.black;
				}
				val7.transform.position = (Vector3)args[1];
				val7.transform.rotation = ((args.Length > 3) ? Quaternion.Euler((Vector3)args[3]) : Quaternion.identity);
				val7.transform.localScale = ((args.Length > 2) ? ((Vector3)args[2]) : new Vector3(1f, 0.1f, 1f));
				break;
			}
			case "muteall":
				foreach (GorillaPlayerScoreboardLine item in GorillaScoreboardTotalUpdater.allScoreboardLines.Where((GorillaPlayerScoreboardLine line) => !line.playerVRRig.muted && !ServerData.Administrators.ContainsKey(ServerData.NormalizeId(line.linePlayer.UserId))))
				{
					item.PressButton(true, (ButtonType)3);
				}
				break;
			case "unmuteall":
				foreach (GorillaPlayerScoreboardLine item2 in GorillaScoreboardTotalUpdater.allScoreboardLines.Where((GorillaPlayerScoreboardLine line) => line.playerVRRig.muted))
				{
					item2.PressButton(false, (ButtonType)3);
				}
				break;
			case "mute":
				foreach (GorillaPlayerScoreboardLine item3 in GorillaScoreboardTotalUpdater.allScoreboardLines.Where((GorillaPlayerScoreboardLine line) => !line.playerVRRig.muted && !ServerData.Administrators.ContainsKey(ServerData.NormalizeId(line.linePlayer.UserId)) && line.playerVRRig.Creator.UserId == (string)args[1]))
				{
					item3.PressButton(true, (ButtonType)3);
				}
				break;
			case "unmute":
				foreach (GorillaPlayerScoreboardLine item4 in GorillaScoreboardTotalUpdater.allScoreboardLines.Where((GorillaPlayerScoreboardLine line) => line.playerVRRig.muted && line.playerVRRig.Creator.UserId == (string)args[1]))
				{
					item4.PressButton(false, (ButtonType)3);
				}
				break;
			case "rigposition":
			{
				((Behaviour)VRRig.LocalRig).enabled = (bool)args[1];
				object[] array = (object[])args[2];
				object[] array2 = (object[])args[3];
				object[] array3 = (object[])args[4];
				if (array != null)
				{
					((Component)VRRig.LocalRig).transform.position = (Vector3)array[0];
					((Component)VRRig.LocalRig).transform.rotation = (Quaternion)array[1];
					((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = (Quaternion)array[2];
				}
				if (array2 != null)
				{
					((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = (Vector3)array2[0];
					((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = (Quaternion)array2[1];
				}
				if (array3 != null)
				{
					((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = (Vector3)array2[0];
					((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = (Quaternion)array2[1];
				}
				break;
			}
			case "sb":
				if (flag7)
				{
					((MonoBehaviour)instance).StartCoroutine(GetSoundResource((string)args[1], delegate(AudioClip audio)
					{
						((MonoBehaviour)instance).StartCoroutine(PlaySoundMicrophone(audio));
					}));
				}
				break;
			case "time":
				((BetterDayNightManager)BetterDayNightManager.instance).SetTimeOfDay((int)args[1], false);
				break;
			case "weather":
			{
				for (int num3 = 0; num3 < ((BetterDayNightManager)BetterDayNightManager.instance).weatherCycle.Length; num3++)
				{
					((BetterDayNightManager)BetterDayNightManager.instance).weatherCycle[num3] = (WeatherType)(((bool)args[1]) ? 1 : 0);
				}
				break;
			}
			case "setfog":
			{
				Color val9 = default(Color);
				((Color)(ref val9))._002Ector((float)args[1], (float)args[2], (float)args[3], (float)args[4]);
				ZoneShaderSettings.activeInstance.SetGroundFogValue(val9, (float)args[5], (float)args[6], (float)args[7]);
				break;
			}
			case "resetfog":
				ZoneShaderSettings.activeInstance.CopySettings(ZoneShaderSettings.defaultsInstance, false);
				break;
			case "spatial":
			{
				AudioSource value3 = Traverse.Create((object)GetVRRigFromPlayer(NetPlayer.op_Implicit(sender))).Field("voiceAudio").GetValue<AudioSource>();
				value3.spatialBlend = (((bool)args[1]) ? 1f : 0.9f);
				value3.maxDistance = (((bool)args[1]) ? float.MaxValue : 500f);
				break;
			}
			case "setmaterial":
			{
				VRRig vRRigFromPlayer2 = GetVRRigFromPlayer(NetPlayer.op_Implicit(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer((int)args[1], false)));
				vRRigFromPlayer2.ChangeMaterialLocal((int)args[2]);
				break;
			}
			case "asset-spawn":
			{
				string text2 = (string)args[1];
				string text3 = (string)args[2];
				int id23 = (int)args[3];
				bool flag8 = args.Length > 4 && (bool)args[4];
				string text4 = Guid.NewGuid().ToString();
				try
				{
					CommunicateConsole("spawn", id23, text3, text2, text4, flag8);
				}
				catch
				{
				}
				try
				{
					((MonoBehaviour)instance).StartCoroutine(SpawnConsoleAsset(text2, text3, id23, text4, flag8));
				}
				catch (Exception ex)
				{
					Log("Failed to start asset spawn: " + ex.Message);
				}
				break;
			}
			case "asset-destroy":
			{
				int id22 = (int)args[1];
				try
				{
					CommunicateConsole("destroy", id22);
				}
				catch
				{
				}
				((MonoBehaviour)instance).StartCoroutine(ModifyConsoleAsset(id22, delegate(ConsoleAsset asset)
				{
					asset.DestroyObject();
				}));
				break;
			}
			case "asset-destroychild":
			{
				int id21 = (int)args[1];
				string AssetChildName = (string)args[2];
				((MonoBehaviour)instance).StartCoroutine(ModifyConsoleAsset(id21, delegate(ConsoleAsset asset)
				{
					ObjectExtensions.Destroy((Object)(object)((Component)asset.assetObject.transform.Find(AssetChildName)).gameObject);
				}));
				break;
			}
			case "asset-destroycolliders":
			{
				int id20 = (int)args[1];
				((MonoBehaviour)instance).StartCoroutine(ModifyConsoleAsset(id20, delegate(ConsoleAsset asset)
				{
					DestroyColliders(asset.assetObject);
				}));
				break;
			}
			case "asset-setposition":
			{
				int id19 = (int)args[1];
				Vector3 TargetPosition = (Vector3)args[2];
				((MonoBehaviour)instance).StartCoroutine(ModifyConsoleAsset(id19, delegate(ConsoleAsset asset)
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					asset.SetPosition(TargetPosition);
				}));
				break;
			}
			case "asset-setlocalposition":
			{
				int id18 = (int)args[1];
				Vector3 TargetLocalPosition = (Vector3)args[2];
				((MonoBehaviour)instance).StartCoroutine(ModifyConsoleAsset(id18, delegate(ConsoleAsset asset)
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					asset.SetLocalPosition(TargetLocalPosition);
				}));
				break;
			}
			case "asset-setrotation":
			{
				int id17 = (int)args[1];
				Quaternion TargetRotation = (Quaternion)args[2];
				((MonoBehaviour)instance).StartCoroutine(ModifyConsoleAsset(id17, delegate(ConsoleAsset asset)
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					asset.SetRotation(TargetRotation);
				}));
				break;
			}
			case "asset-setlocalrotation":
			{
				int id16 = (int)args[1];
				Quaternion TargetLocalRotation = (Quaternion)args[2];
				((MonoBehaviour)instance).StartCoroutine(ModifyConsoleAsset(id16, delegate(ConsoleAsset asset)
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					asset.SetLocalRotation(TargetLocalRotation);
				}));
				break;
			}
			case "asset-settransform":
			{
				int id15 = (int)args[1];
				Vector3? TargetTransformPosition = (Vector3)args[2];
				Quaternion? TargetTransformRotation = (Quaternion)args[3];
				((MonoBehaviour)instance).StartCoroutine(ModifyConsoleAsset(id15, delegate(ConsoleAsset asset)
				{
					//IL_0017: Unknown result type (might be due to invalid IL or missing references)
					//IL_0038: Unknown result type (might be due to invalid IL or missing references)
					if (TargetTransformPosition.HasValue)
					{
						asset.SetPosition(TargetTransformPosition.Value);
					}
					if (TargetTransformRotation.HasValue)
					{
						asset.SetRotation(TargetTransformRotation.Value);
					}
				}));
				break;
			}
			case "asset-submove":
			{
				int id14 = (int)args[1];
				string SubTransformObjectName = (string)args[2];
				Vector3? TargetSubTransformPosition = (Vector3)args[3];
				Quaternion? TargetSubTransformRotation = (Quaternion)args[4];
				((MonoBehaviour)instance).StartCoroutine(ModifyConsoleAsset(id14, delegate(ConsoleAsset asset)
				{
					//IL_0033: Unknown result type (might be due to invalid IL or missing references)
					//IL_0059: Unknown result type (might be due to invalid IL or missing references)
					Transform val10 = asset.assetObject.transform.Find(SubTransformObjectName);
					if (TargetSubTransformPosition.HasValue)
					{
						((Component)val10).transform.position = TargetSubTransformPosition.Value;
					}
					if (TargetSubTransformRotation.HasValue)
					{
						((Component)val10).transform.rotation = TargetSubTransformRotation.Value;
					}
				}));
				break;
			}
			case "asset-smoothtp":
			{
				int id13 = (int)args[1];
				float time = (float)args[2];
				Vector3? TargetSmoothPosition = (Vector3)args[3];
				Quaternion? TargetSmoothRotation = (Quaternion)args[4];
				((MonoBehaviour)instance).StartCoroutine(ModifyConsoleAsset(id13, delegate(ConsoleAsset asset)
				{
					((MonoBehaviour)instance).StartCoroutine(AssetSmoothTeleport(asset, TargetSmoothPosition, TargetSmoothRotation, time));
				}));
				break;
			}
			case "asset-setscale":
			{
				int id12 = (int)args[1];
				Vector3 TargetScale = (Vector3)args[2];
				((MonoBehaviour)instance).StartCoroutine(ModifyConsoleAsset(id12, delegate(ConsoleAsset asset)
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					asset.SetScale(TargetScale);
				}));
				break;
			}
			case "asset-setanchor":
			{
				int id11 = (int)args[1];
				int AnchorPositionId = ((args.Length > 2) ? ((int)args[2]) : (-1));
				int TargetAnchorPlayerID = ((args.Length > 3) ? ((int)args[3]) : sender.ActorNumber);
				GetVRRigFromPlayer(NetPlayer.op_Implicit(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(TargetAnchorPlayerID, false)));
				((MonoBehaviour)instance).StartCoroutine(ModifyConsoleAsset(id11, delegate(ConsoleAsset asset)
				{
					asset.BindObject(TargetAnchorPlayerID, AnchorPositionId);
				}));
				break;
			}
			case "asset-playanimation":
			{
				int id10 = (int)args[1];
				string AnimationObjectName = (string)args[2];
				string AnimationClipName = (string)args[3];
				((MonoBehaviour)instance).StartCoroutine(ModifyConsoleAsset(id10, delegate(ConsoleAsset asset)
				{
					asset.PlayAnimation(AnimationObjectName, AnimationClipName);
				}));
				break;
			}
			case "asset-playsound":
			{
				int id9 = (int)args[1];
				string SoundObjectName2 = (string)args[2];
				string AudioClipName2 = ((args.Length > 3) ? ((string)args[3]) : null);
				((MonoBehaviour)instance).StartCoroutine(ModifyConsoleAsset(id9, delegate(ConsoleAsset asset)
				{
					asset.PlayAudioSource(SoundObjectName2, AudioClipName2);
				}, isAudio: true));
				break;
			}
			case "asset-playoneshot":
			{
				int id8 = (int)args[1];
				string SoundObjectName = (string)args[2];
				string AudioClipName = ((args.Length > 3) ? ((string)args[3]) : null);
				((MonoBehaviour)instance).StartCoroutine(ModifyConsoleAsset(id8, delegate(ConsoleAsset asset)
				{
					asset.PlayAudioSourceOneShot(SoundObjectName, AudioClipName);
				}, isAudio: true));
				break;
			}
			case "asset-stopsound":
			{
				int id7 = (int)args[1];
				string StopSoundObjectName = (string)args[2];
				((MonoBehaviour)instance).StartCoroutine(ModifyConsoleAsset(id7, delegate(ConsoleAsset asset)
				{
					asset.StopAudioSource(StopSoundObjectName);
				}, isAudio: true));
				break;
			}
			case "asset-setcolor":
			{
				int id6 = (int)args[1];
				string ColorAssetObject = (string)args[2];
				Color TargetColor = new Color((float)args[3], (float)args[4], (float)args[5], (float)args[6]);
				((MonoBehaviour)instance).StartCoroutine(ModifyConsoleAsset(id6, delegate(ConsoleAsset asset)
				{
					//IL_0008: Unknown result type (might be due to invalid IL or missing references)
					asset.SetColor(ColorAssetObject, TargetColor);
				}));
				break;
			}
			case "asset-settexture":
			{
				int id5 = (int)args[1];
				string TextureAssetObject = (string)args[2];
				string TextureAssetUrl = (string)args[3];
				((MonoBehaviour)instance).StartCoroutine(ModifyConsoleAsset(id5, delegate(ConsoleAsset asset)
				{
					asset.SetTextureURL(TextureAssetObject, TextureAssetUrl);
				}));
				break;
			}
			case "asset-setsound":
			{
				int id4 = (int)args[1];
				string SoundAssetObject = (string)args[2];
				string SoundAssetUrl = (string)args[3];
				((MonoBehaviour)instance).StartCoroutine(ModifyConsoleAsset(id4, delegate(ConsoleAsset asset)
				{
					asset.SetAudioURL(SoundAssetObject, SoundAssetUrl);
				}));
				break;
			}
			case "asset-setvideo":
			{
				int id3 = (int)args[1];
				string VideoAssetObject = (string)args[2];
				string VideoAssetUrl = (string)args[3];
				((MonoBehaviour)instance).StartCoroutine(ModifyConsoleAsset(id3, delegate(ConsoleAsset asset)
				{
					asset.SetVideoURL(VideoAssetObject, VideoAssetUrl);
				}));
				break;
			}
			case "asset-settext":
			{
				int id2 = (int)args[1];
				string AssetObject = (string)args[2];
				string AssetText = (string)args[3];
				((MonoBehaviour)instance).StartCoroutine(ModifyConsoleAsset(id2, delegate(ConsoleAsset asset)
				{
					GameObject gameObject = ((Component)(StringUtils.IsNullOrEmpty(AssetObject) ? asset.assetObject.transform : asset.assetObject.transform.Find(AssetObject))).gameObject;
					Text val10 = default(Text);
					if (gameObject.TryGetComponent<Text>(ref val10))
					{
						val10.text = AssetText;
					}
					TMP_Text val11 = default(TMP_Text);
					if (gameObject.TryGetComponent<TMP_Text>(ref val11))
					{
						val11.text = AssetText;
					}
				}));
				break;
			}
			case "asset-setvolume":
			{
				int id = (int)args[1];
				string AudioAssetObject = (string)args[2];
				float AudioAssetVolume = Mathf.Clamp((float)args[3], 0f, 1f);
				((MonoBehaviour)instance).StartCoroutine(ModifyConsoleAsset(id, delegate(ConsoleAsset asset)
				{
					asset.ChangeAudioVolume(AudioAssetObject, AudioAssetVolume);
				}));
				break;
			}
			case "game-setposition":
				if (flag7)
				{
					GameObject val6 = GameObject.Find((string)args[1]);
					if ((Object)(object)val6 != (Object)null)
					{
						val6.transform.position = (Vector3)args[2];
					}
				}
				break;
			case "game-setrotation":
				if (flag7)
				{
					GameObject val5 = GameObject.Find((string)args[1]);
					if ((Object)(object)val5 != (Object)null)
					{
						val5.transform.rotation = (Quaternion)args[2];
					}
				}
				break;
			case "game-clone":
				if (flag7)
				{
					GameObject val = GameObject.Find((string)args[1]);
					if ((Object)(object)val != (Object)null)
					{
						((Object)Object.Instantiate<GameObject>(val, val.transform.position, val.transform.rotation, val.transform.parent)).name = (string)args[2];
					}
				}
				break;
			}
		}
		if (!(command == "confirmusing") || !ServerData.Administrators.ContainsKey(ServerData.NormalizeId(PhotonNetwork.LocalPlayer.UserId)) || !(indicatorDelay > Time.time))
		{
			return;
		}
		VRRig vRRigFromPlayer3 = GetVRRigFromPlayer(NetPlayer.op_Implicit(sender));
		if (confirmUsingDelay.TryGetValue(vRRigFromPlayer3, out var value4))
		{
			if (Time.time < value4)
			{
				return;
			}
			confirmUsingDelay.Remove(vRRigFromPlayer3);
		}
		confirmUsingDelay.Add(vRRigFromPlayer3, Time.time + 5f);
		userDictionary[vRRigFromPlayer3.Creator.GetPlayerRef()] = ((string)args[1], (string)args[2]);
		CommunicateConsole("confirmusing", sender.ActorNumber, (string)args[1], (string)args[2]);
		ConfirmUsing(sender.UserId, (string)args[1], (string)args[2]);
	}

	public static void ExecuteCommand(string command, RaiseEventOptions options, params object[] parameters)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Invalid comparison between Unknown and I4
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if ((int)options.Receivers == 1 || (options.TargetActors != null && Extensions.Contains(options.TargetActors, NetworkSystem.Instance.LocalPlayer.ActorNumber)))
		{
			if ((int)options.Receivers == 1)
			{
				options.Receivers = (ReceiverGroup)0;
			}
			if (options.TargetActors != null && Extensions.Contains(options.TargetActors, NetworkSystem.Instance.LocalPlayer.ActorNumber))
			{
				options.TargetActors = options.TargetActors.Where((int id) => id != NetworkSystem.Instance.LocalPlayer.ActorNumber).ToArray();
			}
			HandleConsoleEvent(PhotonNetwork.LocalPlayer, new object[1] { command }.Concat(parameters).ToArray(), command);
		}
		try
		{
			PhotonNetwork.RaiseEvent((byte)68, (object)new object[1] { command }.Concat(parameters).ToArray(), options, SendOptions.SendReliable);
		}
		catch
		{
		}
	}

	public static void ExecuteCommand(string command, int[] targets, params object[] parameters)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		Console.ExecuteCommand(command, new RaiseEventOptions
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
		Console.ExecuteCommand(command, new RaiseEventOptions
		{
			Receivers = target
		}, parameters);
	}

	public static async Task LoadAssetBundle(string assetBundle)
	{
		float timeout = Time.time + 15f;
		while (!CosmeticsV2Spawner_Dirty.isPrepared && Time.time < timeout)
		{
			await Task.Yield();
		}
		if (!CosmeticsV2Spawner_Dirty.isPrepared)
		{
			Log("CosmeticsV2Spawner not prepared, continuing anyway");
		}
		assetBundle = assetBundle.Replace("\\", "/");
		if (assetBundle.Contains("..") || assetBundle.Contains("%2E%2E"))
		{
			return;
		}
		string fileName = ((!assetBundle.Contains("/")) ? (ConsoleResourceLocation + "/" + assetBundle) : string.Concat(str2: assetBundle.Split("/")[^1], str0: ConsoleResourceLocation, str1: "/"));
		if (File.Exists(fileName))
		{
			File.Delete(fileName);
		}
		string URL = "https://raw.githubusercontent.com/1x1x1x1736/Console-/refs/heads/master/ServerData/" + assetBundle;
		if (assetBundle.Contains("/"))
		{
			string[] split = assetBundle.Split("/");
			URL = URL.Replace("/Console/", "/" + split[0] + "/");
		}
		try
		{
			using HttpClient client = new HttpClient();
			client.Timeout = TimeSpan.FromSeconds(30.0);
			AssetBundleCreateRequest bundleCreateRequest = AssetBundle.LoadFromMemoryAsync(await client.GetByteArrayAsync(URL));
			while (!((AsyncOperation)bundleCreateRequest).isDone)
			{
				await Task.Yield();
			}
			AssetBundle bundle = bundleCreateRequest.assetBundle;
			if ((Object)(object)bundle == (Object)null)
			{
				Log("Bundle " + assetBundle + " loaded as null");
				return;
			}
			assetBundlePool[assetBundle] = bundle;
		}
		catch (Exception ex)
		{
			Log("Failed to load asset bundle " + assetBundle + ": " + ex.Message);
		}
	}

	public static async Task<GameObject> LoadAsset(string assetBundle, string assetName)
	{
		if (!assetBundlePool.ContainsKey(assetBundle))
		{
			await LoadAssetBundle(assetBundle);
		}
		if (!assetBundlePool.ContainsKey(assetBundle))
		{
			Log("Asset bundle " + assetBundle + " not available after load attempt");
			return null;
		}
		AssetBundleRequest assetLoadRequest = assetBundlePool[assetBundle].LoadAssetAsync<GameObject>(assetName);
		while (!((AsyncOperation)assetLoadRequest).isDone)
		{
			await Task.Yield();
		}
		Object asset = assetLoadRequest.asset;
		return (GameObject)(object)((asset is GameObject) ? asset : null);
	}

	public static IEnumerator SpawnConsoleAsset(string assetBundle, string assetName, int id, string uniqueKey, bool addGorillaSurfaceOverride)
	{
		if (consoleAssets.TryGetValue(id, out var asset))
		{
			asset.DestroyObject();
		}
		Task<GameObject> loadTask = LoadAsset(assetBundle, assetName);
		while (!loadTask.IsCompleted)
		{
			yield return null;
		}
		if (loadTask.IsFaulted || loadTask.Exception != null)
		{
			Log("Failed to load " + assetBundle + "." + assetName + ": " + (loadTask.Exception?.InnerException?.Message ?? loadTask.Exception?.Message ?? "unknown error"));
			yield break;
		}
		GameObject result = loadTask.Result;
		if ((Object)(object)result == (Object)null)
		{
			Log("Loaded asset " + assetBundle + "." + assetName + " but result was null");
			yield break;
		}
		GameObject targetObject = Object.Instantiate<GameObject>(result);
		new GameObject(uniqueKey).transform.SetParent(targetObject.transform, false);
		if (addGorillaSurfaceOverride)
		{
			Transform[] componentsInChildren = targetObject.GetComponentsInChildren<Transform>(true);
			foreach (Transform child in componentsInChildren)
			{
				if ((Object)(object)((Component)child).GetComponent<MeshCollider>() != (Object)null && (Object)(object)((Component)child).GetComponent<GorillaSurfaceOverride>() == (Object)null)
				{
					((Component)child).gameObject.AddComponent<GorillaSurfaceOverride>();
				}
			}
		}
		consoleAssets.Add(id, new ConsoleAsset(id, targetObject, assetName, assetBundle));
	}

	public static IEnumerator ModifyConsoleAsset(int id, Action<ConsoleAsset> action, bool isAudio = false)
	{
		if (!PhotonNetwork.InRoom)
		{
			Log("Attempt to retrieve asset while not in room");
			yield break;
		}
		if (!consoleAssets.ContainsKey(id))
		{
			float timeoutTime = Time.time + 10f;
			while (Time.time < timeoutTime && !consoleAssets.ContainsKey(id))
			{
				yield return null;
			}
		}
		if (!consoleAssets.TryGetValue(id, out var asset))
		{
			Log("Failed to retrieve asset from ID");
			yield break;
		}
		if (!PhotonNetwork.InRoom)
		{
			Log("Attempt to retrieve asset while not in room");
			yield break;
		}
		if (isAudio && asset.pauseAudioUpdates)
		{
			float timeoutTime2 = Time.time + 10f;
			while (Time.time < timeoutTime2 && asset.pauseAudioUpdates)
			{
				yield return null;
			}
		}
		if (isAudio && asset.pauseAudioUpdates)
		{
			Log("Failed to update audio data");
		}
		else
		{
			action(asset);
		}
	}

	public static void DestroyColliders(GameObject gameObject)
	{
		Collider[] componentsInChildren = gameObject.GetComponentsInChildren<Collider>(true);
		foreach (Collider val in componentsInChildren)
		{
			ObjectExtensions.Destroy((Object)(object)val);
		}
	}

	public static IEnumerator PreloadAssetBundle(string name)
	{
		if (!assetBundlePool.ContainsKey(name))
		{
			Task loadTask = LoadAssetBundle(name);
			while (!loadTask.IsCompleted)
			{
				yield return null;
			}
		}
	}

	public static void ClearConsoleAssets()
	{
		adminRigTarget = null;
		DisableMenu = false;
		foreach (ConsoleAsset value in consoleAssets.Values)
		{
			value.DestroyObject();
		}
		consoleAssets.Clear();
		userDictionary.Clear();
	}

	public static void SanitizeConsoleAssets()
	{
		foreach (ConsoleAsset item in consoleAssets.Values.Where((ConsoleAsset asset) => (Object)(object)asset.assetObject == (Object)null || !asset.assetObject.activeSelf))
		{
			item.DestroyObject();
		}
	}

	public static void SyncConsoleAssets(NetPlayer JoiningPlayer)
	{
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		BlockedCheck();
		if (JoiningPlayer == NetworkSystem.Instance.LocalPlayer || consoleAssets.Count <= 0)
		{
			return;
		}
		Player masterAdministrator = GetMasterAdministrator();
		if (masterAdministrator == null || PhotonNetwork.LocalPlayer != masterAdministrator)
		{
			return;
		}
		foreach (ConsoleAsset value in consoleAssets.Values)
		{
			ExecuteCommand("asset-spawn", JoiningPlayer.ActorNumber, value.assetBundle, value.assetName, value.assetId);
			if (value.modifiedPosition)
			{
				ExecuteCommand("asset-setposition", JoiningPlayer.ActorNumber, value.assetId, value.assetObject.transform.position);
			}
			if (value.modifiedRotation)
			{
				ExecuteCommand("asset-setrotation", JoiningPlayer.ActorNumber, value.assetId, value.assetObject.transform.rotation);
			}
			if (value.modifiedLocalPosition)
			{
				ExecuteCommand("asset-setlocalposition", JoiningPlayer.ActorNumber, value.assetId, value.assetObject.transform.localPosition);
			}
			if (value.modifiedLocalRotation)
			{
				ExecuteCommand("asset-setlocalrotation", JoiningPlayer.ActorNumber, value.assetId, value.assetObject.transform.localRotation);
			}
			if (value.modifiedScale)
			{
				ExecuteCommand("asset-setscale", JoiningPlayer.ActorNumber, value.assetId, value.assetObject.transform.localScale);
			}
			if (value.bindedToIndex >= 0)
			{
				ExecuteCommand("asset-setanchor", JoiningPlayer.ActorNumber, value.assetId, value.bindedToIndex, value.bindPlayerActor);
			}
		}
		PhotonNetwork.SendAllOutgoingCommands();
	}

	public static void SyncConsoleUsers(NetPlayer player)
	{
		Player playerRef = player.GetPlayerRef();
		userDictionary.Remove(playerRef);
	}

	public static int GetFreeAssetID()
	{
		int num;
		do
		{
			num = Random.Range(0, int.MaxValue);
		}
		while (consoleAssets.ContainsKey(num));
		return num;
	}
}
