using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Photon.Pun;
using Seralyth.Classes.Menu;
using Seralyth.Menu;
using Seralyth.Mods;
using Seralyth.Utilities;
using UnityEngine;
using UnityEngine.Networking;

namespace Seralyth.Managers;

public class AIManager
{
	public static string SystemPrompt = "NAME: Seralyth's Voice Assistant\n        MENU VERSION: {2}\n        MOD COUNT: {0}\n\n        You are a voice assistant for a Gorilla Tag mod menu called \"MrChicken Menu\". You are not MrChicken, but represent the menu.\n        GitHub: https://github.com/Seralyth\n        Seralyth's Discord Server: {1}\n\n        Speak using simple 7th grade vocabulary. Limit all responses to 2 sentences and 300 characters. No emojis, em-dashes, markdown, or questions. Do not advertise other menus or AI unless asked. You may mention Pollinations AI only if asked.\n\n        When asked about mods, only reference Gorilla Tag or copy/fan games like Capuchin. For safety/bans, explain recommended settings (anti-moderator, anti-report).\n\n        # Commands\n        Use the following commands when users request actions:\n        <ENABLEMOD_\"ModName\"> - Enables the mod.\n        <DISABLEMOD_\"ModName\"> - Disables the mod.\n        <TOGGLEMOD_\"ModName\"> - Toggles the mod.\n        <SETNAME_\"Name\"> - Change user's name.\n        <SETCOLOR\"Hex\"> - Change color (hex code only).\n        <JOINROOM_\"RoomCode\"> - Join a room. Use \"RANDOM\" for random rooms.\n\n        If a requested mod isn't listed, attempt the closest ENABLE, DISABLE, or TOGGLE command. Always respond with the command **and** a brief comment. Never say a mod doesn't exist or isn't safe. Assume every user request can be interpreted as a mod action.\n\n        Always assist to the best of your ability, focusing on Gorilla Tag mod functionality.";

	public static bool customPrompt;

	public static bool generating;

	public static string URLEncode(string input)
	{
		return Uri.EscapeDataString(input);
	}

	public static int Duration(string input)
	{
		int num = input.Split(new char[4] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
		return num * 400 + 500;
	}

	public static IEnumerator AskAI(string text, Action<string> onResponse = null, bool silent = false)
	{
		string filePath = "SeralythMenu/Seralyth_SystemPrompt.txt";
		if (!File.Exists(filePath))
		{
			File.WriteAllText(filePath, SystemPrompt);
		}
		else if (customPrompt)
		{
			SystemPrompt = File.ReadAllText(filePath);
		}
		if (Time.time < Main.timeMenuStarted + 5f)
		{
			yield break;
		}
		if (Main.narratorName == "Mommy ASMR")
		{
			SystemPrompt += "And remember, you are a calm, confident, gently dominant mommy-style caretaker with a warm, slow, reassuring, and authoritative tone, offering structure, comfort, praise, soft correction, and clear caring boundaries; when the user asks for approval, reassurance, validation, or comfort, respond with immediate, direct affirmation and nurturing praise using simple, confident language. Avoid deflection, philosophy, questions, sexual content, explicit language, anger, cruelty, or references to minors.";
		}
		text = URLEncode(text);
		string prompt = URLEncode(string.Format(SystemPrompt, Main.fullModAmount, Main.serverLink, "10.0.2"));
		string api = "https://text.pollinations.ai/" + text + "?system=" + prompt + "&private=true&model=openai";
		UnityWebRequest request = UnityWebRequest.Get(api);
		try
		{
			request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
			generating = true;
			yield return request.SendWebRequest();
			if ((int)request.result != 1)
			{
				if (Settings.debugDictation)
				{
					LogManager.LogError("Error contacting AI api " + request.error + ".");
					DownloadHandler downloadHandler = request.downloadHandler;
					if (!string.IsNullOrEmpty((downloadHandler != null) ? downloadHandler.text : null))
					{
						LogManager.LogError("Response Body: " + request.downloadHandler.text);
					}
				}
				onResponse?.Invoke("Error: " + request.error);
				generating = false;
				if (!silent)
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> There was an issue generating your response. " + request.error, 4000);
					AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Menu/close.ogg", "Audio/Menu/close.ogg", delegate(AudioClip clip)
					{
						Settings.DictationPlay(clip, (float)Main.buttonClickVolume / 10f);
					});
					if (!Buttons.GetIndex("Chain Voice Commands").enabled)
					{
						((MonoBehaviour)CoroutineManager.instance).StartCoroutine(Settings.DictationRestart());
					}
				}
				yield break;
			}
			string response = request.downloadHandler.text;
			if (Settings.debugDictation)
			{
				LogManager.Log("AI Response: " + response);
			}
			MatchCollection matches = Regex.Matches(response, "<([A-Z]+)(?:_\"([^\"]*)\")?>");
			if (Main.dynamicSounds)
			{
				AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Menu/confirm.ogg", "Audio/Menu/confirm.ogg", delegate(AudioClip clip)
				{
					Settings.DictationPlay(clip, (float)Main.buttonClickVolume / 10f);
				});
			}
			string formatResponse = Regex.Replace(response, "<([A-Z]+)(?:_\"([^\"]*)\")?>", "").Replace("\n", "");
			onResponse?.Invoke(formatResponse);
			if (!silent)
			{
				NotificationManager.ClearAllNotifications();
				string narratorName = Main.narratorName;
				string text2 = narratorName;
				if (text2 == "Mommy ASMR")
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=#ffb6c1>MOMMY</color><color=grey>]</color> " + formatResponse, Duration(formatResponse));
				}
				else
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=blue>AI</color><color=grey>]</color> " + formatResponse, Duration(formatResponse));
				}
			}
			bool narrate = Buttons.GetIndex("Narrate Assistant").enabled;
			bool globalNarrate = Buttons.GetIndex("Global Narrate Assistant").enabled;
			if (narrate)
			{
				if (globalNarrate && PhotonNetwork.InRoom)
				{
					Main.SpeakText(formatResponse);
				}
				else
				{
					Main.NarrateText(formatResponse);
				}
			}
			foreach (Match match in matches)
			{
				string commandName = match.Groups[1].Value;
				string argument = (match.Groups[2].Success ? match.Groups[2].Value : null);
				switch (commandName)
				{
				case "ENABLEMOD":
				{
					ButtonInfo button = Buttons.GetIndex(argument);
					if (button == null)
					{
						button = Buttons.buttons.SelectMany(delegate(ButtonInfo[] buttonList, int i)
						{
							object result;
							if (Buttons.categoryNames[i].Contains("settings", StringComparison.OrdinalIgnoreCase))
							{
								result = Enumerable.Empty<ButtonInfo>();
							}
							else
							{
								result = buttonList;
							}
							return (IEnumerable<ButtonInfo>)result;
						}).FirstOrDefault((ButtonInfo b) => (b.overlapText ?? b.buttonText).Contains(argument, StringComparison.OrdinalIgnoreCase));
					}
					if (button != null)
					{
						if (!button.enabled)
						{
							Main.Toggle(button.buttonText, fromMenu: true);
						}
						else
						{
							NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Mod is already enabled.");
						}
					}
					else
					{
						NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Mod \"" + argument + "\" does not exist.");
					}
					break;
				}
				case "DISABLEMOD":
				{
					ButtonInfo button2 = Buttons.GetIndex(argument);
					if (button2 == null)
					{
						button2 = Buttons.buttons.SelectMany(delegate(ButtonInfo[] buttonList, int i)
						{
							object result;
							if (Buttons.categoryNames[i].Contains("settings", StringComparison.OrdinalIgnoreCase))
							{
								result = Enumerable.Empty<ButtonInfo>();
							}
							else
							{
								result = buttonList;
							}
							return (IEnumerable<ButtonInfo>)result;
						}).FirstOrDefault((ButtonInfo b) => (b.overlapText ?? b.buttonText).Contains(argument, StringComparison.OrdinalIgnoreCase));
					}
					if (button2 != null)
					{
						if (button2.enabled)
						{
							Main.Toggle(button2.buttonText, fromMenu: true);
						}
						else
						{
							NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Mod is already enabled.");
						}
					}
					else
					{
						NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Mod \"" + argument + "\" does not exist.");
					}
					break;
				}
				case "TOGGLEMOD":
				{
					ButtonInfo button3 = Buttons.GetIndex(argument);
					if (button3 == null)
					{
						button3 = Buttons.buttons.SelectMany(delegate(ButtonInfo[] buttonList, int i)
						{
							object result;
							if (Buttons.categoryNames[i].Contains("settings", StringComparison.OrdinalIgnoreCase))
							{
								result = Enumerable.Empty<ButtonInfo>();
							}
							else
							{
								result = buttonList;
							}
							return (IEnumerable<ButtonInfo>)result;
						}).FirstOrDefault((ButtonInfo b) => (b.overlapText ?? b.buttonText).Contains(argument, StringComparison.OrdinalIgnoreCase));
					}
					if (button3 != null)
					{
						Main.Toggle(button3.buttonText, fromMenu: true);
					}
					else
					{
						NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Mod \"" + argument + "\" does not exist.");
					}
					break;
				}
				case "JOINROOM":
					if (argument.ToLower() == "random")
					{
						Important.JoinRandom();
					}
					Important.QueueRoom(argument.ToUpper());
					break;
				case "SETNAME":
					Main.ChangeName(argument.ToUpper());
					break;
				case "SETCOLOR":
					Main.ChangeColor(Main.HexToColor(argument));
					break;
				}
			}
			if (!Buttons.GetIndex("Chain Voice Commands").enabled)
			{
				((MonoBehaviour)CoroutineManager.instance).StartCoroutine(Settings.DictationRestart());
			}
			generating = false;
		}
		finally
		{
			((IDisposable)request)?.Dispose();
		}
	}
}
