using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Seralyth.Utilities;

public class FileUtilities
{
	public static string GetFileExtension(string fileName)
	{
		string path = fileName.Split('?')[0];
		return Path.GetExtension(path).TrimStart('.').ToLower();
	}

	public static string RemoveLastDirectory(string directory)
	{
		return (directory == "" || directory.LastIndexOf('/') <= 0) ? "" : directory.Substring(0, directory.LastIndexOf('/'));
	}

	public static string RemoveFileExtension(string file)
	{
		int num = 0;
		string text = "";
		string[] array = file.Split(".");
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			num++;
			if (num != array.Length)
			{
				if (num > 1)
				{
					text += ".";
				}
				text += text2;
			}
		}
		return text;
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

	public static string GetFullPath(Transform transform)
	{
		string text = "";
		while ((Object)(object)transform.parent != (Object)null)
		{
			transform = transform.parent;
			text = ((text == "") ? ((Object)transform).name : (((Object)transform).name + "/" + text));
		}
		return text;
	}

	public static string GetGamePath()
	{
		return AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
	}

	public static string SanitizeFileName(string input)
	{
		input = input.Trim();
		char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
		input = invalidFileNameChars.Aggregate(input, (string current, char c) => current.Replace(c, '_'));
		input = input.Replace("../", "").Replace("..\\", "").Replace("./", "")
			.Replace(".\\", "");
		input = input.Replace(":", "").Replace("\\", "").Replace("/", "");
		if (input.Length > 64)
		{
			input = input.Substring(0, 64);
		}
		if (string.IsNullOrWhiteSpace(input))
		{
			input = "file";
		}
		return input;
	}
}
