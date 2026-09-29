using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using Seralyth.Managers;
using UnityEngine;
using UnityEngine.Networking;

namespace Seralyth.Utilities;

public class AssetUtilities
{
	private static AssetBundle assetBundle;

	public static readonly Dictionary<string, AudioClip> audioFilePool = new Dictionary<string, AudioClip>();

	public static readonly Dictionary<string, Texture2D> textureResourceDictionary = new Dictionary<string, Texture2D>();

	public static readonly Dictionary<string, Texture2D> textureUrlDictionary = new Dictionary<string, Texture2D>();

	public static readonly Dictionary<string, Texture2D> textureFileDirectory = new Dictionary<string, Texture2D>();

	private static void LoadAssetBundle()
	{
		Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SeralythMenu.Resources.Client.seralythmenu");
		if (manifestResourceStream != null)
		{
			assetBundle = AssetBundle.LoadFromStream(manifestResourceStream);
		}
		else
		{
			LogManager.LogError("Failed to load assetbundle");
		}
	}

	public static T LoadObject<T>(string assetName) where T : Object
	{
		if ((Object)(object)assetBundle == (Object)null)
		{
			LoadAssetBundle();
		}
		return Object.Instantiate<T>(assetBundle.LoadAsset<T>(assetName));
	}

	public static T LoadAsset<T>(string assetName) where T : Object
	{
		if ((Object)(object)assetBundle == (Object)null)
		{
			LoadAssetBundle();
		}
		Object obj = assetBundle.LoadAsset(assetName);
		return (T)(object)((obj is T) ? obj : null);
	}

	public static void LoadSoundFromFile(string fileName, Action<AudioClip> onLoaded)
	{
		((MonoBehaviour)CoroutineManager.instance).StartCoroutine(Load());
		IEnumerator Load()
		{
			if (audioFilePool.TryGetValue(fileName, out var cached) && (Object)(object)cached != (Object)null)
			{
				onLoaded?.Invoke(cached);
				yield break;
			}
			string filePath = FileUtilities.GetGamePath() + "/SeralythMenu/" + fileName;
			string url = "file://" + filePath;
			DownloadHandlerAudioClip handler = new DownloadHandlerAudioClip(url, FileUtilities.GetAudioType(FileUtilities.GetFileExtension(fileName)));
			UnityWebRequest request = new UnityWebRequest(url, "GET", (DownloadHandler)(object)handler, (UploadHandler)null);
			try
			{
				yield return request.SendWebRequest();
				if ((int)request.result != 1)
				{
					LogManager.LogError("Failed to load audio file '" + fileName + "': " + request.error + "\nPath: " + url);
					onLoaded?.Invoke(null);
				}
				else
				{
					AudioClip clip = handler.audioClip;
					if ((Object)(object)clip != (Object)null)
					{
						audioFilePool[fileName] = clip;
					}
					onLoaded?.Invoke(clip);
				}
			}
			finally
			{
				((IDisposable)request)?.Dispose();
			}
		}
	}

	public static void LoadSoundFromURL(string resourcePath, string fileName, Action<AudioClip> action = null)
	{
		if (audioFilePool.TryGetValue(fileName, out var value) && (Object)(object)value != (Object)null)
		{
			action?.Invoke(value);
			return;
		}
		string filePath = "SeralythMenu/" + fileName;
		string directoryName = Path.GetDirectoryName(filePath);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		if (File.Exists(filePath))
		{
			LoadSoundFromFile(fileName, action);
		}
		else
		{
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(Load());
		}
		IEnumerator Load()
		{
			UnityWebRequest request = UnityWebRequest.Get(resourcePath);
			try
			{
				yield return request.SendWebRequest();
				if ((int)request.result != 1)
				{
					LogManager.LogError("Failed to download " + fileName + ": " + request.error);
					action?.Invoke(null);
				}
				else
				{
					byte[] remoteData = request.downloadHandler.data;
					LogManager.Log("Downloaded " + fileName);
					File.WriteAllBytes(filePath, remoteData);
					if (action != null)
					{
						LoadSoundFromFile(fileName, action);
					}
				}
			}
			finally
			{
				((IDisposable)request)?.Dispose();
			}
		}
	}

	public static Texture2D LoadTextureFromResource(string resourcePath)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		if (textureResourceDictionary.TryGetValue(resourcePath, out var value))
		{
			return value;
		}
		Texture2D val = new Texture2D(2, 2);
		Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
		if (manifestResourceStream != null)
		{
			byte[] array = new byte[manifestResourceStream.Length];
			manifestResourceStream.Read(array, 0, (int)manifestResourceStream.Length);
			ImageConversion.LoadImage(val, array);
		}
		else
		{
			LogManager.LogError("Failed to load texture from resource: " + resourcePath);
		}
		textureResourceDictionary[resourcePath] = val;
		return val;
	}

	public static Texture2D LoadTextureFromURL(string resourcePath, string fileName)
	{
		if (textureUrlDictionary.TryGetValue(resourcePath, out var value))
		{
			return value;
		}
		string text = "SeralythMenu/" + fileName;
		string directoryName = Path.GetDirectoryName(text);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		if (!File.Exists(text))
		{
			LogManager.Log("Downloading " + fileName);
			WebClient webClient = new WebClient();
			webClient.DownloadFile(resourcePath, text);
		}
		Texture2D val = LoadTextureFromFile(fileName);
		textureUrlDictionary[resourcePath] = val;
		return val;
	}

	public static Texture2D LoadTextureFromFile(string fileName)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		if (textureFileDirectory.TryGetValue(fileName, out var value))
		{
			return value;
		}
		string path = "SeralythMenu/" + fileName;
		string directoryName = Path.GetDirectoryName(path);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		Texture2D val = new Texture2D(2, 2);
		byte[] array = File.ReadAllBytes(path);
		ImageConversion.LoadImage(val, array);
		textureFileDirectory[fileName] = val;
		return val;
	}
}
