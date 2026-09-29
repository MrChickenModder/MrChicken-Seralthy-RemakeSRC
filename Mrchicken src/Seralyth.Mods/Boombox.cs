using System;
using System.Collections;
using System.IO;
using Seralyth.Managers;
using UnityEngine;
using UnityEngine.Networking;

namespace Seralyth.Mods;

public static class Boombox
{
	private static GameObject boomboxObj;

	private static AudioSource audioSource;

	private static AudioClip musicClip;

	private static bool musicLoaded;

	public static void BoomboxUpdate()
	{
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)boomboxObj == (Object)null)
		{
			boomboxObj = GameObject.CreatePrimitive((PrimitiveType)3);
			((Object)boomboxObj).name = "Seralyth_Boombox";
			boomboxObj.transform.localScale = new Vector3(0.5f, 0.3f, 0.2f);
			Object.Destroy((Object)(object)boomboxObj.GetComponent<Collider>());
			Renderer component = boomboxObj.GetComponent<Renderer>();
			string path = "C:\\Users\\kalew\\OneDrive\\Pictures\\download.jfif";
			if (File.Exists(path))
			{
				byte[] array = File.ReadAllBytes(path);
				Texture2D val = new Texture2D(2, 2);
				ImageConversion.LoadImage(val, array);
				component.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
				component.material.SetTexture("_BaseMap", (Texture)(object)val);
			}
			else
			{
				component.material = new Material(Shader.Find("GorillaTag/UberShader"));
			}
			component.material.color = Color.white;
			audioSource = boomboxObj.AddComponent<AudioSource>();
			audioSource.spatialBlend = 1f;
			audioSource.loop = true;
			audioSource.volume = 0.5f;
			if (!musicLoaded)
			{
				string path2 = "C:\\Users\\kalew\\OneDrive\\Pictures\\music.ogg";
				if (File.Exists(path2))
				{
					((MonoBehaviour)CoroutineManager.instance).StartCoroutine(LoadMusic(path2));
				}
			}
		}
		Transform leftHandTransform = GorillaTagger.Instance.leftHandTransform;
		boomboxObj.transform.position = leftHandTransform.position;
		boomboxObj.transform.rotation = leftHandTransform.rotation;
	}

	public static void DisableBoombox()
	{
		if ((Object)(object)audioSource != (Object)null)
		{
			audioSource.Stop();
			audioSource = null;
		}
		if ((Object)(object)musicClip != (Object)null)
		{
			Object.Destroy((Object)(object)musicClip);
			musicClip = null;
		}
		if ((Object)(object)boomboxObj != (Object)null)
		{
			Object.Destroy((Object)(object)boomboxObj);
			boomboxObj = null;
		}
		musicLoaded = false;
	}

	private static IEnumerator LoadMusic(string path)
	{
		string url = "file:///" + path;
		AudioType type = GetAudioType(Path.GetExtension(path));
		UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip(url, type);
		try
		{
			yield return req.SendWebRequest();
			if ((int)req.result == 1)
			{
				musicClip = DownloadHandlerAudioClip.GetContent(req);
				if ((Object)(object)musicClip != (Object)null && (Object)(object)audioSource != (Object)null)
				{
					audioSource.clip = musicClip;
					audioSource.Play();
					musicLoaded = true;
				}
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
