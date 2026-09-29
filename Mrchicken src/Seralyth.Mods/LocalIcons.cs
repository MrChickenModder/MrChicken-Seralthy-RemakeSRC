using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Seralyth.Utilities;
using UnityEngine;

namespace Seralyth.Mods;

public static class LocalIcons
{
	public static readonly string Folder = Path.Combine("SeralythMenu", "TagIcons");

	private static List<string> files;

	private static readonly Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();

	private static readonly HashSet<string> extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tga" };

	public static string ChickenPath => Path.Combine(Folder, "chicken.png");

	public static string EnsureChickenIcon()
	{
		try
		{
			if (!Directory.Exists(Folder))
			{
				Directory.CreateDirectory(Folder);
			}
			if (!File.Exists(ChickenPath))
			{
				Texture2D val = AssetUtilities.LoadTextureFromResource("SeralythMenu.Resources.Client.icon.png");
				if ((Object)(object)val == (Object)null)
				{
					return null;
				}
				byte[] array = ImageConversion.EncodeToPNG(val);
				if (array == null || array.Length == 0)
				{
					return null;
				}
				File.WriteAllBytes(ChickenPath, array);
			}
			files = null;
			return ChickenPath;
		}
		catch
		{
			return null;
		}
	}

	public static List<string> GetIcons()
	{
		if (files != null)
		{
			return files;
		}
		try
		{
			if (!Directory.Exists(Folder))
			{
				Directory.CreateDirectory(Folder);
			}
			files = (from f in Directory.GetFiles(Folder)
				where extensions.Contains(Path.GetExtension(f))
				select f).OrderBy<string, string>((string f) => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase).ToList();
		}
		catch
		{
			files = new List<string>();
		}
		return files;
	}

	public static void Refresh()
	{
		files = null;
		foreach (Texture2D value in textureCache.Values)
		{
			if ((Object)(object)value != (Object)null)
			{
				Object.Destroy((Object)(object)value);
			}
		}
		textureCache.Clear();
	}

	public static Texture2D LoadTexture(string path)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		if (string.IsNullOrEmpty(path))
		{
			return null;
		}
		if (textureCache.TryGetValue(path, out var value) && (Object)(object)value != (Object)null)
		{
			return value;
		}
		try
		{
			if (!File.Exists(path))
			{
				return null;
			}
			Texture2D val = new Texture2D(2, 2);
			if (!ImageConversion.LoadImage(val, File.ReadAllBytes(path)))
			{
				Object.Destroy((Object)(object)val);
				return null;
			}
			textureCache[path] = val;
			return val;
		}
		catch
		{
			return null;
		}
	}

	public static int DrawPicker(float x, ref float y, float w, int page, GUIStyle labelStyle, GUIStyle smallLabelStyle, GUIStyle smallBtnStyle, GUIStyle accentBtnStyle, Action<string> onSelect)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		List<string> icons = GetIcons();
		GUI.Label(new Rect(x, y + 2f, 45f, 20f), "Local:", labelStyle);
		if (GUI.Button(new Rect(x + 48f, y, 60f, 20f), "Rescan", smallBtnStyle))
		{
			Refresh();
		}
		GUI.Label(new Rect(x + 113f, y + 2f, w - 115f, 16f), (icons.Count == 0) ? "No images in TagIcons folder" : $"{icons.Count} image(s) in TagIcons", smallLabelStyle);
		y += 24f;
		if (icons.Count == 0)
		{
			return page;
		}
		float num = (w - 6f) / 3f;
		int num2 = Mathf.Max(1, Mathf.CeilToInt((float)icons.Count / 9f));
		page = Mathf.Clamp(page, 0, num2 - 1);
		int num3 = page * 9;
		int num4 = Mathf.Min(9, icons.Count - num3);
		for (int i = 0; i < num4; i++)
		{
			int num5 = i % 3;
			int num6 = i / 3;
			string text = icons[num3 + i];
			string text2 = Path.GetFileNameWithoutExtension(text);
			if (text2.Length > 12)
			{
				text2 = text2.Substring(0, 11) + "...";
			}
			if (GUI.Button(new Rect(x + (float)num5 * (num + 3f), y + (float)num6 * 23f, num, 20f), text2, accentBtnStyle))
			{
				onSelect?.Invoke(text);
			}
		}
		int num7 = Mathf.CeilToInt((float)num4 / 3f);
		y += (float)num7 * 23f + 2f;
		if (num2 > 1)
		{
			if (GUI.Button(new Rect(x, y, 30f, 20f), "<", smallBtnStyle))
			{
				page--;
			}
			GUI.Label(new Rect(x + 34f, y + 2f, w - 68f, 16f), $"Page {page + 1}/{num2}", smallLabelStyle);
			if (GUI.Button(new Rect(x + w - 30f, y, 30f, 20f), ">", smallBtnStyle))
			{
				page++;
			}
			y += 24f;
			page = Mathf.Clamp(page, 0, num2 - 1);
		}
		return page;
	}
}
