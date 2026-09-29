using System;
using System.Collections.Generic;
using System.IO;
using Seralyth.Classes.Menu;
using Seralyth.Menu;
using Seralyth.Utilities;
using UnityEngine;

namespace Seralyth.Managers;

public class SoundManager
{
	private static AudioSource _buttonSource;

	public static readonly Dictionary<string, Dictionary<string, object>> Sounds = new Dictionary<string, Dictionary<string, object>>
	{
		["Buttons"] = new Dictionary<string, object>
		{
			{ "Wood", 8 },
			{ "Keyboard", 66 },
			{ "Default", 67 },
			{ "Bubble", 84 },
			{ "Steal", "Audio/Menu/Buttons/steal.ogg" },
			{ "Anthrax", "Audio/Menu/Buttons/anthrax.ogg" },
			{ "Lever", "Audio/Menu/Buttons/lever.ogg" },
			{ "Minecraft", "Audio/Menu/Buttons/minecraft.ogg" },
			{ "Rec Room", "Audio/Menu/Buttons/lever$1.ogg" },
			{ "Watch", "Audio/Menu/Buttons/watch.ogg" },
			{ "Membrane", "Audio/Menu/Buttons/membrane.ogg" },
			{ "Jar", 106 },
			{ "Slider", "Audio/Menu/Buttons/slider.ogg" },
			{ "Can", "Audio/Menu/Buttons/can.ogg" },
			{ "Cut", "Audio/Menu/Buttons/cut.ogg" },
			{ "Creamy", "Audio/Menu/Buttons/creamy.ogg" },
			{ "Roblox Button", "Audio/Menu/Buttons/robloxbutton.ogg" },
			{ "Roblox Tick", "Audio/Menu/Buttons/robloxtick.ogg" },
			{ "Mouse", "Audio/Menu/Buttons/mouse.ogg" },
			{ "Valve", "Audio/Menu/Buttons/valve.ogg" },
			{ "Nintendo", "Audio/Menu/Buttons/nintendo.ogg" },
			{ "Windows", "Audio/Menu/Buttons/windows.ogg" },
			{ "Destiny", "Audio/Menu/Buttons/destiny.ogg" },
			{ "Untitled", "Audio/Menu/Buttons/untitled.ogg" },
			{ "Slap", 338 },
			{ "Dog", "Audio/Menu/Buttons/dog.ogg" },
			{ "GMod Spawn", "Audio/Menu/Buttons/gmod.ogg" },
			{ "GMod Undo", "Audio/Menu/Buttons/undo.ogg" },
			{ "Half Life", "Audio/Menu/Buttons/hl1.ogg" },
			{ "Mine", "Audio/Menu/Buttons/mine.ogg" },
			{ "Sensation", "Audio/Menu/Buttons/sensation.ogg" },
			{ "Thocky Click", "https://raw.githubusercontent.com/lyfedev-csharp/elysor-resources/refs/heads/main/Sounds/thocky-click.mp3" }
		},
		["Menu"] = new Dictionary<string, object>
		{
			{ "Next", "Audio/Menu/next.ogg" },
			{ "Previous", "Audio/Menu/prev.ogg" },
			{ "Up", "Audio/Menu/up.ogg" },
			{ "Down", "Audio/Menu/down.ogg" },
			{ "Open", "Audio/Menu/open.ogg" },
			{ "Close", "Audio/Menu/close.ogg" },
			{ "Select", "Audio/Menu/select.ogg" },
			{ "Achievement", "Audio/Menu/achievement.ogg" },
			{ "Admin", "Audio/Menu/admin.ogg" },
			{ "Patreon", "Audio/Menu/patreon.ogg" }
		},
		["Notifications"] = new Dictionary<string, object>
		{
			{ "None", "" },
			{ "Pop", "Audio/Menu/Notifications/pop.ogg" },
			{ "Ding", "Audio/Menu/Notifications/ding.ogg" },
			{ "Twitter", "Audio/Menu/Notifications/twitter.ogg" },
			{ "Discord", "Audio/Menu/Notifications/discord.ogg" },
			{ "Whatsapp", "Audio/Menu/Notifications/whatsapp.ogg" },
			{ "Grindr", "Audio/Menu/Notifications/grindr.ogg" },
			{ "iOS", "Audio/Menu/Notifications/ios.ogg" },
			{ "XP Notify", "Audio/Menu/Notifications/xpnotify.ogg" },
			{ "XP Ding", "Audio/Menu/Notifications/xptrueding.ogg" },
			{ "XP Question", "Audio/Menu/Notifications/xpding.ogg" },
			{ "XP Error", "Audio/Menu/Notifications/xperror.ogg" },
			{ "Roblox Bass", "Audio/Menu/Notifications/robloxbass.ogg" },
			{ "Oculus", "Audio/Menu/Notifications/oculus.ogg" },
			{ "Nintendo", "Audio/Menu/Notifications/nintendo.ogg" },
			{ "Telegram", "Audio/Menu/Notifications/telegram.ogg" },
			{ "7 Ding", "Audio/Menu/Notifications/win7-ding.ogg" },
			{ "7 Error", "Audio/Menu/Notifications/win7-error.ogg" },
			{ "7 Exclamation", "Audio/Menu/Notifications/win7-exc.ogg" },
			{ "AOL Alert", "Audio/Menu/Notifications/aol-alert.ogg" },
			{ "AOL Message", "Audio/Menu/Notifications/aol-msg.ogg" },
			{ "Thunderbird", "Audio/Menu/Notifications/thunderbird.ogg" },
			{ "Pixie Dust", "Audio/Menu/Notifications/pixiedust.ogg" },
			{ "Moon Beam", "Audio/Menu/Notifications/moonbeam.ogg" },
			{ "Dog", "Audio/Menu/Notifications/dog.ogg" },
			{ "GMod Error", "Audio/Menu/Notifications/gmod-error.ogg" }
		}
	};

	public static Dictionary<string, string> DefaultSounds = new Dictionary<string, string>
	{
		{ "Button", "Default" },
		{ "Notification", "None" },
		{ "Next", "Next" },
		{ "Previous", "Previous" },
		{ "Up", "Up" },
		{ "Down", "Down" },
		{ "Open", "Open" },
		{ "Close", "Close" },
		{ "Select", "Select" }
	};

	public static readonly Dictionary<string, Dictionary<string, object>> Soundpacks = new Dictionary<string, Dictionary<string, object>>
	{
		["None"] = null,
		["SteamVR"] = new Dictionary<string, object>
		{
			["Buttons"] = new Dictionary<string, object> { { "Default", "Audio/Menu/Preset/SteamVR/click.ogg" } },
			["Menu"] = new Dictionary<string, object>
			{
				{ "Open", "Audio/Menu/Preset/SteamVR/open.ogg" },
				{ "Close", "Audio/Menu/Preset/SteamVR/close.ogg" },
				{ "Achievement", "Audio/Menu/Preset/SteamVR/achievement.ogg" },
				{ "Admin", "Audio/Menu/Preset/SteamVR/patreon.ogg" },
				{ "Patreon", "Audio/Menu/Preset/SteamVR/patreon.ogg" }
			},
			["Notifications"] = new Dictionary<string, object> { { "None", "Audio/Menu/Preset/SteamVR/notification.ogg" } }
		},
		["Minecraft Legacy Edition"] = new Dictionary<string, object>
		{
			["Buttons"] = new Dictionary<string, object> { { "Default", "Audio/Menu/Preset/MinecraftLegacyEdition/click.ogg" } },
			["Menu"] = new Dictionary<string, object>
			{
				{ "Admin", "Audio/Menu/Preset/MinecraftLegacyEdition/patreon.ogg" },
				{ "Patreon", "Audio/Menu/Preset/MinecraftLegacyEdition/patreon.ogg" },
				{ "Return", "Audio/Menu/Preset/MinecraftLegacyEdition/back.ogg" }
			},
			["Notifications"] = new Dictionary<string, object> { { "None", "Audio/Menu/Preset/MinecraftLegacyEdition/notification.ogg" } }
		},
		["Playstation 2"] = new Dictionary<string, object>
		{
			["Buttons"] = new Dictionary<string, object> { { "Default", "Audio/Menu/Preset/Playstation2/click.ogg" } },
			["Notifications"] = new Dictionary<string, object> { { "None", "Audio/Menu/Preset/Playstation2/notification.ogg" } }
		},
		["Nintendo Switch"] = new Dictionary<string, object>
		{
			["Buttons"] = new Dictionary<string, object> { { "Default", "Audio/Menu/Preset/NintendoSwitch/click.ogg" } },
			["Menu"] = new Dictionary<string, object>
			{
				{ "Achievement", "Audio/Menu/Preset/NintendoSwitch/achievement.ogg" },
				{ "Return", "Audio/Menu/Preset/NintendoSwitch/back.ogg" }
			},
			["Notifications"] = new Dictionary<string, object> { { "None", "Audio/Menu/Preset/NintendoSwitch/notification.ogg" } }
		}
	};

	public static string DefaultSoundpack = "None";

	private static AudioSource GetButtonSource()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		if ((Object)(object)_buttonSource == (Object)null)
		{
			GameObject val = new GameObject("SeralythButtonSound");
			Object.DontDestroyOnLoad((Object)(object)val);
			_buttonSource = val.AddComponent<AudioSource>();
			_buttonSource.spatialBlend = 0f;
			_buttonSource.playOnAwake = false;
		}
		return _buttonSource;
	}

	public static void Play(string sound, string outputPath = null, Action<AudioClip> action = null, string buttonText = null, bool overlapHand = false, bool leftOverlap = false, bool global = false)
	{
		if (string.IsNullOrEmpty(sound))
		{
			return;
		}
		if (Main.doButtonsVibrate)
		{
			GorillaTagger.Instance.StartVibration(Main.rightHand, GorillaTagger.Instance.tagHapticStrength / 2f, 0.05f);
		}
		try
		{
			bool rightHand = Main.rightHand;
			if (overlapHand)
			{
				Main.rightHand = leftOverlap;
			}
			object obj = ResolveSoundPath(sound);
			if (obj == null)
			{
				Main.rightHand = rightHand;
				return;
			}
			object obj2 = obj;
			object obj3 = obj2;
			if (!(obj3 is int num))
			{
				if (obj3 is string text)
				{
					if (string.IsNullOrEmpty(text))
					{
						Main.rightHand = rightHand;
						return;
					}
					AudioSource val = (Main.rightHand ? VRRig.LocalRig.leftHandPlayer : VRRig.LocalRig.rightHandPlayer);
					val.volume = (float)Main.buttonClickVolume / 10f;
					bool flag = text.StartsWith("http");
					string resourcePath = (flag ? text : ("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/" + text));
					string fileName = (flag ? ("Audio/Menu/Buttons/" + Path.GetFileName(new Uri(text).AbsolutePath)) : (outputPath ?? text));
					AssetUtilities.LoadSoundFromURL(resourcePath, fileName, action ?? ((Action<AudioClip>)delegate(AudioClip clip)
					{
						AudioSource buttonSource = GetButtonSource();
						buttonSource.PlayOneShot(clip, (float)Main.buttonClickVolume / 10f);
					}));
				}
			}
			else
			{
				float tapCoolDown = GorillaTagger.Instance.tapCoolDown;
				GorillaTagger.Instance.tapCoolDown = 0f;
				VRRig.LocalRig.PlayHandTapLocal(num, Main.rightHand, (float)Main.buttonClickVolume / 10f);
				GorillaTagger.Instance.tapCoolDown = tapCoolDown;
			}
			Main.rightHand = rightHand;
		}
		catch (Exception ex)
		{
			LogManager.LogError(ex.Message);
		}
	}

	private static object ResolveSoundPath(string sound, string buttonText = null)
	{
		if (string.IsNullOrEmpty(sound))
		{
			return null;
		}
		Dictionary<string, object> value = null;
		bool flag = !string.IsNullOrEmpty(DefaultSoundpack) && DefaultSoundpack != "None" && Soundpacks.TryGetValue(DefaultSoundpack, out value) && value != null;
		if (sound == "Return")
		{
			if (flag && value.TryGetValue("Menu", out var value2) && value2 is Dictionary<string, object> dictionary && dictionary.TryGetValue("Return", out var value3) && value3 is string text && !string.IsNullOrEmpty(text))
			{
				return text;
			}
			string value4;
			string key = (DefaultSounds.TryGetValue("Button", out value4) ? value4 : "Default");
			if (Sounds.TryGetValue("Buttons", out var value5) && value5.TryGetValue(key, out var value6) && value6 is string text2 && !string.IsNullOrEmpty(text2))
			{
				return text2;
			}
			if (Sounds.TryGetValue("Buttons", out var value7) && value7.TryGetValue("Default", out var value8) && value8 is string text3 && !string.IsNullOrEmpty(text3))
			{
				return text3;
			}
			return null;
		}
		string text4 = null;
		foreach (KeyValuePair<string, Dictionary<string, object>> sound2 in Sounds)
		{
			if (sound2.Value == null || !sound2.Value.ContainsKey(sound))
			{
				continue;
			}
			text4 = sound2.Key;
			break;
		}
		object value9 = null;
		if (text4 != null && Sounds.TryGetValue(text4, out var value10))
		{
			value10.TryGetValue(sound, out value9);
		}
		object value12;
		if (flag && text4 != null && value.TryGetValue(text4, out var value11) && value11 is Dictionary<string, object> dictionary2)
		{
			dictionary2.TryGetValue(sound, out value12);
			if (value12 == null && !dictionary2.TryGetValue("Default", out value12))
			{
				dictionary2.TryGetValue("None", out value12);
			}
			if (value12 == null)
			{
				value12 = value9;
			}
		}
		else
		{
			value12 = value9;
		}
		string text5 = value12 as string;
		if (text5 != null)
		{
			if (string.IsNullOrEmpty(text5))
			{
				return null;
			}
			if (text5.Contains("lever$1"))
			{
				if (!string.IsNullOrEmpty(buttonText))
				{
					ButtonInfo index = Buttons.GetIndex(buttonText);
					text5 = text5.Replace("$1", (index != null && index.enabled) ? "up" : "down");
				}
				else
				{
					text5 = text5.Replace("lever$1", "leverup");
				}
			}
			return text5;
		}
		if (value12 is int)
		{
			return value12;
		}
		return null;
	}
}
