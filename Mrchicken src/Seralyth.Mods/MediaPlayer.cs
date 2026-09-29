using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Seralyth.Classes.Menu;
using Seralyth.Managers;
using Seralyth.Menu;
using UnityEngine;
using UnityEngine.Networking;

namespace Seralyth.Mods;

public static class MediaPlayer
{
	private static AudioSource audioSrc;

	private static bool isPlaying;

	private static readonly string musicFolder = "C:\\Users\\kalew\\Music\\";

	private static string[] musicFiles;

	private static int currentMusic;

	private static AudioClip musicClip;

	private static GameObject musicPlayerObj;

	public static void PlayMusic()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		if ((Object)(object)musicPlayerObj == (Object)null)
		{
			musicPlayerObj = new GameObject("Seralyth_MusicPlayer");
			audioSrc = musicPlayerObj.AddComponent<AudioSource>();
			audioSrc.spatialBlend = 0f;
			audioSrc.volume = 0.5f;
		}
		if (ScanMusic())
		{
			PlayMusicFile(currentMusic);
		}
	}

	public static void StopMusic()
	{
		if ((Object)(object)audioSrc != (Object)null)
		{
			audioSrc.Stop();
			audioSrc.clip = null;
		}
		if ((Object)(object)musicClip != (Object)null)
		{
			Object.Destroy((Object)(object)musicClip);
			musicClip = null;
		}
		if ((Object)(object)musicPlayerObj != (Object)null)
		{
			Object.Destroy((Object)(object)musicPlayerObj);
			musicPlayerObj = null;
		}
		isPlaying = false;
	}

	public static void NextMusic()
	{
		if (musicFiles == null || musicFiles.Length == 0)
		{
			ScanMusic();
			if (musicFiles.Length == 0)
			{
				return;
			}
		}
		PlayMusicFile((currentMusic + 1) % musicFiles.Length);
	}

	public static void PrevMusic()
	{
		if (musicFiles == null || musicFiles.Length == 0)
		{
			ScanMusic();
			if (musicFiles.Length == 0)
			{
				return;
			}
		}
		PlayMusicFile((currentMusic - 1 + musicFiles.Length) % musicFiles.Length);
	}

	private static bool ScanMusic()
	{
		List<string> list = new List<string>();
		string[] array = new string[6] { "*.mp3", "*.wav", "*.ogg", "*.flac", "*.aac", "*.m4a" };
		string[] array2 = array;
		foreach (string searchPattern in array2)
		{
			if (Directory.Exists(musicFolder))
			{
				list.AddRange(Directory.GetFiles(musicFolder, searchPattern));
			}
		}
		musicFiles = list.ToArray();
		if (musicFiles.Length != 0)
		{
			ButtonInfo index = Buttons.GetIndex("Music Player File:");
			if (index != null)
			{
				index.overlapText = "Music Player File: <color=grey>[</color><color=green>" + Path.GetFileName(musicFiles[currentMusic]) + "</color><color=grey>]</color>";
			}
		}
		return musicFiles.Length != 0;
	}

	private static void PlayMusicFile(int index)
	{
		if (musicFiles.Length != 0)
		{
			currentMusic = index % musicFiles.Length;
			isPlaying = true;
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(LoadMusicFile(musicFiles[currentMusic]));
		}
	}

	private static IEnumerator LoadMusicFile(string path)
	{
		string url = "file:///" + path.Replace("\\", "/");
		AudioType type = GetAudioType(Path.GetExtension(path));
		UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(url, type);
		try
		{
			yield return req.SendWebRequest();
			if ((int)req.result == 1)
			{
				musicClip = DownloadHandlerAudioClip.GetContent(req);
				if ((Object)(object)musicClip != (Object)null)
				{
					if ((Object)(object)audioSrc == (Object)null)
					{
						musicPlayerObj = new GameObject("Seralyth_MusicPlayer");
						audioSrc = musicPlayerObj.AddComponent<AudioSource>();
						audioSrc.spatialBlend = 0f;
					}
					audioSrc.clip = musicClip;
					audioSrc.volume = 0.5f;
					audioSrc.Play();
				}
			}
			ButtonInfo btn = Buttons.GetIndex("Music Player File:");
			if (btn != null)
			{
				btn.overlapText = "Music Player File: <color=grey>[</color><color=green>" + Path.GetFileName(musicFiles[currentMusic]) + "</color><color=grey>]</color>";
			}
		}
		finally
		{
			((IDisposable)req)?.Dispose();
		}
	}

	private static AudioType GetAudioType(string ext)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		switch (ext.ToLower())
		{
		case ".mp3":
			return (AudioType)13;
		case ".wav":
			return (AudioType)20;
		case ".ogg":
			return (AudioType)14;
		case ".aac":
		case ".m4a":
			return (AudioType)13;
		default:
			return (AudioType)13;
		}
	}
}
