using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Seralyth.Menu;
using UnityEngine;
using UnityEngine.Networking;
using Valve.Newtonsoft.Json;
using Valve.Newtonsoft.Json.Linq;

namespace Seralyth.Managers;

public class TranslationManager
{
	public static readonly Dictionary<string, float> waitingForTranslate = new Dictionary<string, float>();

	public static readonly Dictionary<string, string> translateCache = new Dictionary<string, string>();

	public static string language;

	public static string TranslateText(string input, Action<string> onTranslated = null)
	{
		if (translateCache.TryGetValue(input, out var value))
		{
			return value;
		}
		if (!waitingForTranslate.ContainsKey(input))
		{
			waitingForTranslate.Add(input, Time.time + 10f);
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(GetTranslation(input, onTranslated));
		}
		else
		{
			if (!(Time.time > waitingForTranslate[input]))
			{
				return "Loading...";
			}
			waitingForTranslate.Remove(input);
			waitingForTranslate.Add(input, Time.time + 10f);
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(GetTranslation(input, onTranslated));
		}
		return "Loading...";
	}

	public static IEnumerator GetTranslation(string text, Action<string> onTranslated = null)
	{
		if (string.IsNullOrEmpty(text))
		{
			yield break;
		}
		if (translateCache.TryGetValue(text, out var cached))
		{
			onTranslated?.Invoke(cached);
			yield break;
		}
		string fileName = Main.GetSHA256(text) + ".txt";
		string directoryPath = "SeralythMenu/TranslationData" + language.ToUpper();
		if (!Directory.Exists(directoryPath))
		{
			Directory.CreateDirectory(directoryPath);
		}
		string filePath = Path.Combine(directoryPath, fileName);
		string translation = null;
		if (!File.Exists(filePath))
		{
			string cleanText = Regex.Replace(text, "([\"'$`\\\\])", "\\$1");
			cleanText = cleanText.Substring(0, Mathf.Min(cleanText.Length, 4096));
			string cleanLang = Regex.Replace(language, "[^a-zA-Z0-9]", "");
			cleanLang = cleanLang.Substring(0, Mathf.Min(cleanLang.Length, 6));
			string url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl=" + cleanLang + "&dt=t&q=" + UnityWebRequest.EscapeURL(cleanText);
			UnityWebRequest request = UnityWebRequest.Get(url);
			try
			{
				yield return request.SendWebRequest();
				if ((int)request.result == 1)
				{
					try
					{
						List<object> parsed = JsonConvert.DeserializeObject<List<object>>(request.downloadHandler.text);
						object obj = parsed[0];
						JArray sentences = (JArray)((obj is JArray) ? obj : null);
						StringBuilder sb = new StringBuilder();
						foreach (JToken sentence in sentences)
						{
							sb.Append(sentence[(object)0]);
						}
						translation = sb.ToString();
						File.WriteAllText(filePath, translation);
					}
					catch (Exception ex)
					{
						Exception e = ex;
						Debug.LogError((object)$"Translation parse error: {e}");
					}
				}
				else
				{
					Debug.LogError((object)("Translation request failed: " + request.error));
				}
			}
			finally
			{
				((IDisposable)request)?.Dispose();
			}
		}
		else
		{
			translation = File.ReadAllText(filePath);
		}
		if (!string.IsNullOrEmpty(translation))
		{
			translateCache[text] = translation;
			onTranslated?.Invoke(translation);
		}
	}
}
