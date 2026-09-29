using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ExitGames.Client.Photon;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using Seralyth.Classes.Menu;
using Seralyth.Extensions;
using Seralyth.Managers;
using Seralyth.Menu;
using Seralyth.Patches.Menu;
using Seralyth.Utilities;
using UnityEngine;

namespace Seralyth.Mods;

public static class Sound
{
	private class SoundData
	{
		public VoiceManager.Clip Clip;

		public AudioClip AudioClip;
	}

	public static bool LegacySoundboard = false;

	public static bool LoopAudio = false;

	public static bool OverlapAudio = false;

	public static int BindMode;

	public static string Subdirectory = "";

	public static readonly Dictionary<string, ButtonInfo[]> CachedButtons = new Dictionary<string, ButtonInfo[]>();

	public static bool AudioIsPlaying;

	public static float RecoverTime = -1f;

	private static GameObject soundboardAudioManager;

	public static bool disableLocalSoundboard;

	private static readonly Dictionary<string, SoundData> activeSounds = new Dictionary<string, SoundData>();

	private static bool lastBindPressed;

	public static float sendEffectDelay;

	private static float soundSpamDelay;

	private static bool squeakToggle;

	private static bool sirenToggle;

	public static int soundId;

	public static void LoadSoundboard(bool openCategory = true)
	{
		string key = Subdirectory ?? "";
		if (CachedButtons.TryGetValue(key, out var value))
		{
			Buttons.buttons[Buttons.GetCategory("Soundboard")] = value;
			if (openCategory)
			{
				Buttons.CurrentCategoryName = "Soundboard";
			}
			return;
		}
		string path = Path.Combine("SeralythMenu", "Sounds", Subdirectory.TrimStart('/'));
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		List<ButtonInfo> list = new List<ButtonInfo>();
		if (Subdirectory != "")
		{
			list.Add(new ButtonInfo
			{
				buttonText = "Exit Subdirectory",
				overlapText = "Exit " + Subdirectory.Split("/")[^1],
				method = delegate
				{
					Subdirectory = FileUtilities.RemoveLastDirectory(Subdirectory);
					LoadSoundboard();
				},
				isTogglable = false,
				toolTip = "Returns you back to the last folder."
			});
		}
		else
		{
			list.Add(new ButtonInfo
			{
				buttonText = "Exit Soundboard",
				method = delegate
				{
					Buttons.CurrentCategoryName = "Sound Mods";
				},
				isTogglable = false,
				toolTip = "Returns you back to the sound mods."
			});
		}
		string[] directories = Directory.GetDirectories(path);
		string[] files = Directory.GetFiles(path);
		list.AddRange(from folder in directories
			let relativePath = Path.GetRelativePath(path, folder)
			select new ButtonInfo
			{
				buttonText = "SoundboardFolder" + relativePath.Hash(),
				overlapText = "<sprite name=\"Folder\">  " + relativePath + "  ",
				method = delegate
				{
					OpenFolder(relativePath);
				},
				isTogglable = false,
				toolTip = "Opens the " + relativePath + " folder."
			});
		if (!RecorderPatch.enabled)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>WARNING</color><color=grey>]</color> You are using the legacy microphone system. Modern soundboard features will not be implemented.");
		}
		string[] array = files;
		foreach (string path2 in array)
		{
			string relativePath = Path.GetRelativePath(path, path2);
			string soundName = FileUtilities.RemoveFileExtension(relativePath).Replace("_", " ");
			string soundPath = Path.GetRelativePath("SeralythMenu", path2).Replace("\\", "/");
			string buttonText = "SoundboardSound" + soundName.Hash();
			if (RecorderPatch.enabled)
			{
				ButtonInfo buttonInfo = null;
				buttonInfo = new ButtonInfo
				{
					buttonText = buttonText,
					overlapText = soundName,
					toolTip = "Allows you to view " + soundName + "'s properties.",
					method = delegate
					{
						LoadSoundProperties(soundName, soundPath, soundName.Hash());
					},
					isTogglable = false
				};
				list.Add(buttonInfo);
			}
			else
			{
				if (RecorderPatch.enabled && !LegacySoundboard)
				{
					continue;
				}
				if (BindMode > 0)
				{
					list.Add(new ButtonInfo
					{
						buttonText = buttonText,
						overlapText = soundName,
						method = delegate
						{
							PrepareBindAudio(soundPath);
						},
						disableMethod = StopAllSounds,
						toolTip = "Plays " + relativePath + " through your microphone."
					});
				}
				else if (LoopAudio)
				{
					list.Add(new ButtonInfo
					{
						buttonText = buttonText,
						overlapText = soundName,
						enableMethod = delegate
						{
							PlayAudio(soundPath);
						},
						disableMethod = StopAllSounds,
						toolTip = "Plays " + relativePath + " through your microphone."
					});
				}
				else
				{
					list.Add(new ButtonInfo
					{
						buttonText = buttonText,
						overlapText = relativePath,
						method = delegate
						{
							PlayAudio(soundPath);
						},
						isTogglable = false,
						toolTip = "Plays " + relativePath + " through your microphone."
					});
				}
			}
		}
		list.Add(new ButtonInfo
		{
			buttonText = "Stop All Sounds",
			method = StopAllSounds,
			isTogglable = false,
			toolTip = "Stops all currently playing sounds."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Open Sound Folder",
			aliases = new string[1] { "Open Soundboard Folder" },
			method = OpenSoundFolder,
			isTogglable = false,
			toolTip = "Opens a folder containing all of your sounds."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Reload Sounds",
			method = delegate
			{
				CachedButtons.Clear();
				LoadSoundboard();
			},
			isTogglable = false,
			toolTip = "Reloads all of your sounds."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Get More Sounds",
			method = LoadSoundLibrary,
			isTogglable = false,
			toolTip = "Opens a public audio library, where you can download your own sounds."
		});
		CachedButtons[key] = list.ToArray();
		Buttons.buttons[Buttons.GetCategory("Soundboard")] = CachedButtons[key];
		if (openCategory)
		{
			Buttons.CurrentCategoryName = "Soundboard";
		}
	}

	public static void OpenFolder(string folder)
	{
		if (string.IsNullOrEmpty(Subdirectory))
		{
			Subdirectory = "/" + folder;
		}
		else
		{
			Subdirectory = Subdirectory.TrimEnd('/') + "/" + folder;
		}
		LoadSoundboard();
	}

	public static void LoadSoundLibrary()
	{
		string http = Main.GetHttp("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Mods/Fun/Soundboard/SoundLibrary.txt");
		string[] array = Main.AlphabetizeNoSkip(http.Split("\n"));
		List<ButtonInfo> list = new List<ButtonInfo>
		{
			new ButtonInfo
			{
				buttonText = "Exit Sound Library",
				method = delegate
				{
					LoadSoundboard();
				},
				isTogglable = false,
				toolTip = "Returns you back to the soundboard."
			}
		};
		int num = 0;
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text.Length > 2)
			{
				num++;
				string[] Data = text.Split(";");
				list.Add(new ButtonInfo
				{
					buttonText = "SoundboardDownload" + num,
					overlapText = Data[0],
					method = delegate
					{
						DownloadSound(Data[0], "https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Mods/Fun/Soundboard/Sounds/" + Data[1]);
					},
					isTogglable = false,
					toolTip = "Downloads " + Data[0] + " to your sound library."
				});
			}
		}
		Buttons.buttons[Buttons.GetCategory("Sound Library")] = list.ToArray();
		Buttons.CurrentCategoryName = "Sound Library";
	}

	public static void LoadSoundProperties(string soundName, string soundPath, string hash)
	{
		ButtonInfo playButton = null;
		ButtonInfo pauseButton = null;
		ButtonInfo durationButton = null;
		ButtonInfo volumeButton = null;
		ButtonInfo speedButton = null;
		float skipAmount = 1f;
		ButtonInfo skipAmountButton = null;
		ButtonInfo skipButton = null;
		bool flag = activeSounds.ContainsKey(hash);
		string text = soundName.Hash();
		playButton = new ButtonInfo
		{
			buttonText = "Play or Pause SoundboardSound " + text,
			toolTip = "Plays or pauses the sound " + soundName + ".",
			overlapText = (flag ? "Stop" : "Play"),
			enableMethod = Play,
			disableMethod = Stop,
			enabled = flag
		};
		durationButton = new ButtonInfo
		{
			label = true,
			buttonText = "SoundboardSound " + text + "'s Duration",
			overlapText = "Duration: Loading..",
			method = delegate
			{
				try
				{
					if (activeSounds.TryGetValue(hash, out var value) && value.Clip != null)
					{
						durationButton.overlapText = "Duration: " + FormatDuration(value.Clip.CurrentTime);
					}
					else
					{
						durationButton.overlapText = "Duration: N/A";
					}
				}
				catch
				{
				}
			}
		};
		List<ButtonInfo> list = new List<ButtonInfo>
		{
			new ButtonInfo
			{
				buttonText = "Exit " + soundName + "'s Properties",
				method = delegate
				{
					LoadSoundboard();
				},
				isTogglable = false,
				toolTip = "Returns you back to the soundboard."
			},
			playButton
		};
		if (activeSounds.TryGetValue(hash, out var sound) && sound.Clip != null)
		{
			pauseButton = new ButtonInfo
			{
				buttonText = "Pause SoundboardSound " + text,
				overlapText = "Pause",
				enableMethod = delegate
				{
					sound.Clip.Pause();
					pauseButton.overlapText = "Resume";
				},
				disableMethod = delegate
				{
					sound.Clip.Resume();
					pauseButton.overlapText = "Pause";
				},
				toolTip = "Pauses or resumes the sound."
			};
			list.Add(pauseButton);
			list.Add(new ButtonInfo
			{
				buttonText = "Loop SoundboardSound " + text,
				overlapText = "Loop",
				enableMethod = delegate
				{
					sound.Clip.Looping = true;
				},
				disableMethod = delegate
				{
					sound.Clip.Looping = false;
				},
				toolTip = "Makes the song loop when it ends."
			});
			skipButton = new ButtonInfo
			{
				buttonText = "Skip SoundboardSound " + text,
				overlapText = $"Skip <color=grey>[</color><color=green>{skipAmount:0.0}s</color><color=grey>]</color>",
				method = delegate
				{
					sound.Clip.CurrentTime = Mathf.Clamp(sound.Clip.CurrentTime + skipAmount, 0f, sound.Clip.Length);
				},
				enableMethod = delegate
				{
					sound.Clip.CurrentTime = Mathf.Clamp(sound.Clip.CurrentTime + skipAmount, 0f, sound.Clip.Length);
				},
				disableMethod = delegate
				{
					sound.Clip.CurrentTime = Mathf.Clamp(sound.Clip.CurrentTime - skipAmount, 0f, sound.Clip.Length);
				},
				incremental = true,
				isTogglable = false,
				toolTip = "Skips forward or backward in the sound by the skip amount."
			};
			list.Add(skipButton);
			list.Add(durationButton);
			list.Add(new ButtonInfo
			{
				label = true,
				buttonText = "SoundboardSound " + text + "'s Length",
				overlapText = "Length: " + FormatDuration(sound.Clip.Length)
			});
			volumeButton = new ButtonInfo
			{
				buttonText = "Change SoundboardSound " + text + "'s Volume",
				overlapText = $"Change Volume <color=grey>[</color><color=green>{Math.Round(sound.Clip.Volume, 1)}</color><color=grey>]</color>",
				method = delegate
				{
					sound.Clip.Volume = Mathf.Clamp(sound.Clip.Volume + 0.1f, 0f, 10f);
					volumeButton.overlapText = $"Change Volume <color=grey>[</color><color=green>{Math.Round(sound.Clip.Volume, 1)}</color><color=grey>]</color>";
				},
				enableMethod = delegate
				{
					sound.Clip.Volume = Mathf.Clamp(sound.Clip.Volume + 0.1f, 0f, 10f);
					volumeButton.overlapText = $"Change Volume <color=grey>[</color><color=green>{Math.Round(sound.Clip.Volume, 1)}</color><color=grey>]</color>";
				},
				disableMethod = delegate
				{
					sound.Clip.Volume = Mathf.Clamp(sound.Clip.Volume - 0.1f, 0f, 10f);
					volumeButton.overlapText = $"Change Volume <color=grey>[</color><color=green>{Math.Round(sound.Clip.Volume, 1)}</color><color=grey>]</color>";
				},
				incremental = true,
				isTogglable = false,
				toolTip = "Changes the volume of the sound. Higher volumes will make the sound louder, while lower volumes will make it quieter."
			};
			speedButton = new ButtonInfo
			{
				buttonText = "Change SoundboardSound " + text + "'s Speed",
				overlapText = $"Change Speed <color=grey>[</color><color=green>{Math.Round(sound.Clip.Speed, 1)}</color><color=grey>]</color>",
				method = delegate
				{
					sound.Clip.Speed = Mathf.Clamp(sound.Clip.Speed + 0.1f, 0f, 5f);
					speedButton.overlapText = $"Change Speed <color=grey>[</color><color=green>{Math.Round(sound.Clip.Speed, 1)}</color><color=grey>]</color>";
				},
				enableMethod = delegate
				{
					sound.Clip.Speed = Mathf.Clamp(sound.Clip.Speed + 0.1f, 0f, 5f);
					speedButton.overlapText = $"Change Speed <color=grey>[</color><color=green>{Math.Round(sound.Clip.Speed, 1)}</color><color=grey>]</color>";
				},
				disableMethod = delegate
				{
					sound.Clip.Speed = Mathf.Clamp(sound.Clip.Speed - 0.1f, 0f, 5f);
					speedButton.overlapText = $"Change Speed <color=grey>[</color><color=green>{Math.Round(sound.Clip.Speed, 1)}</color><color=grey>]</color>";
				},
				incremental = true,
				isTogglable = false,
				toolTip = "Changes the speed of the sound. Higher speeds will make the pitch higher, while lower speeds will make the pitch lower."
			};
			skipAmountButton = new ButtonInfo
			{
				buttonText = "Change Skip Amount SoundboardSound " + text,
				overlapText = $"Skip Amount <color=grey>[</color><color=green>{skipAmount:0.0}s</color><color=grey>]</color>",
				method = delegate
				{
					skipAmount += 0.5f;
					skipAmountButton.overlapText = $"Skip Amount <color=grey>[</color><color=green>{skipAmount:0.0}s</color><color=grey>]</color>";
					skipButton.overlapText = $"Skip <color=grey>[</color><color=green>{skipAmount:0.0}s</color><color=grey>]</color>";
				},
				enableMethod = delegate
				{
					skipAmount += 0.5f;
					skipAmountButton.overlapText = $"Skip Amount <color=grey>[</color><color=green>{skipAmount:0.0}s</color><color=grey>]</color>";
					skipButton.overlapText = $"Skip <color=grey>[</color><color=green>{skipAmount:0.0}s</color><color=grey>]</color>";
				},
				disableMethod = delegate
				{
					skipAmount = Mathf.Max(0f, skipAmount - 0.5f);
					skipAmountButton.overlapText = $"Skip Amount <color=grey>[</color><color=green>{skipAmount:0.0}s</color><color=grey>]</color>";
					skipButton.overlapText = $"Skip <color=grey>[</color><color=green>{skipAmount:0.0}s</color><color=grey>]</color>";
				},
				incremental = true,
				isTogglable = false,
				toolTip = "Changes how much time is skipped when you press the skip button."
			};
			list.Add(skipAmountButton);
			list.Add(volumeButton);
			list.Add(speedButton);
		}
		Buttons.buttons[Buttons.GetCategory("Sound Properties")] = list.ToArray();
		Buttons.CurrentCategoryName = "Sound Properties";
		static string FormatDuration(double totalSeconds)
		{
			if (totalSeconds < 0.0)
			{
				totalSeconds = 0.0;
			}
			int num = (int)(totalSeconds / 86400.0);
			int num2 = (int)(totalSeconds % 86400.0 / 3600.0);
			int num3 = (int)(totalSeconds % 3600.0 / 60.0);
			int num4 = (int)(totalSeconds % 60.0);
			List<string> list2 = new List<string>();
			if (num > 0)
			{
				list2.Add(string.Format("{0} day{1}", num, (num == 1) ? "" : "s"));
			}
			if (num2 > 0)
			{
				list2.Add(string.Format("{0} hour{1}", num2, (num2 == 1) ? "" : "s"));
			}
			if (num3 > 0)
			{
				list2.Add(string.Format("{0} minute{1}", num3, (num3 == 1) ? "" : "s"));
			}
			if (num4 > 0 || list2.Count == 0)
			{
				list2.Add(string.Format("{0} second{1}", num4, (num4 == 1) ? "" : "s"));
			}
			return string.Join(" ", list2);
		}
		void Play()
		{
			if (OverlapAudio)
			{
				PlayAudio(soundPath);
				playButton.enabled = false;
			}
			else if (!activeSounds.ContainsKey(hash))
			{
				playButton.overlapText = "Stop";
				PlaySoundboardSound(soundPath, hash, LoopAudio, BindMode > 0);
				((MonoBehaviour)CoroutineManager.instance).StartCoroutine(Reload());
			}
		}
		IEnumerator Reload()
		{
			while (!activeSounds.ContainsKey(hash))
			{
				yield return null;
			}
			LoadSoundProperties(soundName, soundPath, hash);
			Main.ReloadMenu();
		}
		void Stop()
		{
			if (activeSounds.ContainsKey(hash))
			{
				playButton.overlapText = "Play";
				StopSoundboardSound(hash);
				LoadSoundProperties(soundName, soundPath, hash);
				Main.ReloadMenu();
			}
		}
	}

	public static void DownloadSound(string name, string url)
	{
		if (name.Contains(".."))
		{
			name = name.Replace("..", "");
		}
		if (name.Contains(":"))
		{
			return;
		}
		string text = Path.Combine("Sounds", Subdirectory.TrimStart('/'), name + "." + FileUtilities.GetFileExtension(url));
		if (File.Exists("SeralythMenu/" + text))
		{
			File.Delete("SeralythMenu/" + text);
		}
		AssetUtilities.audioFilePool.Remove(name);
		AssetUtilities.LoadSoundFromURL(url, text, delegate(AudioClip clip)
		{
			if (clip.length < 20f)
			{
				Main.Play2DAudio(clip);
			}
		});
		CachedButtons.Remove(Subdirectory ?? "");
		NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully downloaded " + name + " to the soundboard.");
	}

	public static void PlayAudio(AudioClip sound, bool disableMicrophone = false)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		if (!PhotonNetwork.InRoom)
		{
			if ((Object)(object)soundboardAudioManager == (Object)null)
			{
				soundboardAudioManager = new GameObject("2DAudioMgr");
				AudioSource val = soundboardAudioManager.AddComponent<AudioSource>();
				val.spatialBlend = 0f;
			}
			AudioSource component = soundboardAudioManager.GetComponent<AudioSource>();
			component.volume = 1f;
			component.clip = sound;
			component.loop = false;
			component.Play();
			AudioIsPlaying = true;
			RecoverTime = Time.time + sound.length;
		}
		else
		{
			if (RecorderPatch.enabled)
			{
				VoiceManager.Get().AudioClip(sound, disableMicrophone);
			}
			else
			{
				NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.SourceType = (InputSourceType)1;
				NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.AudioClip = sound;
				NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.RestartRecording(true);
			}
			if (!LoopAudio)
			{
				AudioIsPlaying = true;
				RecoverTime = Time.time + sound.length + 0.4f;
			}
		}
	}

	public static void PlaySoundboardSound(object file, string hash, bool loopAudio, bool bind)
	{
		bool[] array = new bool[10]
		{
			Main.rightPrimary,
			Main.rightSecondary,
			Main.leftPrimary,
			Main.leftSecondary,
			Main.leftGrab,
			Main.rightGrab,
			Main.leftTrigger > 0.5f,
			Main.rightTrigger > 0.5f,
			Main.leftJoystickClick,
			Main.rightJoystickClick
		};
		bool flag = true;
		if (bind && BindMode > 0)
		{
			bool flag2 = array[BindMode - 1];
			flag = flag2 && !lastBindPressed;
			lastBindPressed = flag2;
		}
		if (!flag)
		{
			return;
		}
		if (file is string fileName)
		{
			AssetUtilities.LoadSoundFromFile(fileName, Play);
			return;
		}
		AudioClip val = (AudioClip)((file is AudioClip) ? file : null);
		if (val != null)
		{
			Play(val);
		}
		void Play(AudioClip clip)
		{
			if (!((Object)(object)clip == (Object)null))
			{
				if (!activeSounds.ContainsKey(hash) && RecorderPatch.enabled && PhotonNetwork.InRoom)
				{
					VoiceManager.Clip clip2 = VoiceManager.Get().AudioClip(clip);
					activeSounds[hash] = new SoundData
					{
						Clip = clip2,
						AudioClip = clip
					};
				}
				IReadOnlyList<VoiceManager.Clip> audioClips = VoiceManager.Get().AudioClips;
				List<string> list = activeSounds.Keys.ToList();
			}
		}
	}

	public static void StopSoundboardSound(string hash)
	{
		if (activeSounds.Any() && activeSounds.ContainsKey(hash))
		{
			if (RecorderPatch.enabled)
			{
				VoiceManager.Get().StopAudioClip(activeSounds[hash].Clip);
			}
			activeSounds.Remove(hash);
		}
	}

	public static void PlayAudio(string file)
	{
		if (PhotonNetwork.InRoom)
		{
			AssetUtilities.LoadSoundFromFile(file, delegate(AudioClip clip)
			{
				PlayAudio(clip);
			});
		}
	}

	public static void StopAllSounds()
	{
		if ((Object)(object)soundboardAudioManager != (Object)null)
		{
			soundboardAudioManager.GetComponent<AudioSource>().Stop();
		}
		foreach (ButtonInfo[] value in CachedButtons.Values)
		{
			ButtonInfo[] array = value;
			foreach (ButtonInfo buttonInfo in array)
			{
				if (buttonInfo != null && buttonInfo.enabled)
				{
					buttonInfo.enabled = false;
				}
			}
		}
		if (PhotonNetwork.InRoom)
		{
			if (RecorderPatch.enabled)
			{
				if (activeSounds != null)
				{
					HashSet<string> hashSet = new HashSet<string>(activeSounds.Keys);
					ButtonInfo[][] buttons = Buttons.buttons;
					foreach (ButtonInfo[] array2 in buttons)
					{
						ButtonInfo[] array3 = array2;
						foreach (ButtonInfo buttonInfo2 in array3)
						{
							if (hashSet.Contains(buttonInfo2.buttonText))
							{
								buttonInfo2.enabled = false;
							}
						}
					}
					activeSounds.Clear();
				}
				VoiceManager.Get().StopAudioClips();
				NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.DebugEchoMode = false;
			}
			else
			{
				NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.SourceType = (InputSourceType)0;
				NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.AudioClip = null;
				NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.RestartRecording(true);
				NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.DebugEchoMode = false;
			}
		}
		AudioIsPlaying = false;
		RecoverTime = -1f;
	}

	public static void FixMicrophone()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (RecorderPatch.enabled)
		{
			if (activeSounds != null)
			{
				HashSet<string> hashSet = new HashSet<string>(activeSounds.Keys);
				ButtonInfo[][] buttons = Buttons.buttons;
				foreach (ButtonInfo[] array in buttons)
				{
					ButtonInfo[] array2 = array;
					foreach (ButtonInfo buttonInfo in array2)
					{
						if (hashSet.Contains(buttonInfo.buttonText))
						{
							buttonInfo.enabled = false;
						}
					}
				}
				activeSounds.Clear();
			}
			VoiceManager.Get().StopAudioClips();
		}
		NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.SourceType = (InputSourceType)0;
		NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.AudioClip = null;
		NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.RestartRecording(true);
		NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.DebugEchoMode = false;
	}

	public static void PrepareBindAudio(string file)
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Invalid comparison between Unknown and I4
		bool[] array = new bool[10]
		{
			Main.rightPrimary,
			Main.rightSecondary,
			Main.leftPrimary,
			Main.leftSecondary,
			Main.leftGrab,
			Main.rightGrab,
			Main.leftTrigger > 0.5f,
			Main.rightTrigger > 0.5f,
			Main.leftJoystickClick,
			Main.rightJoystickClick
		};
		bool flag = array[BindMode - 1];
		if (flag && !lastBindPressed)
		{
			if ((int)NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.SourceType == 1)
			{
				FixMicrophone();
			}
			else
			{
				PlayAudio(file);
			}
		}
		lastBindPressed = flag;
	}

	public static void OpenSoundFolder()
	{
		string fileName = FileUtilities.GetGamePath() + "/SeralythMenu/Sounds";
		Process.Start(fileName);
	}

	public static void SoundBindings(bool positive = true)
	{
		string[] array = new string[11]
		{
			"None", "A", "B", "X", "Y", "Left Grip", "Right Grip", "Left Trigger", "Right Trigger", "Left Joystick",
			"Right Joystick"
		};
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				BindMode++;
			}
			else
			{
				BindMode--;
			}
		}
		BindMode %= array.Length;
		if (BindMode < 0)
		{
			BindMode = array.Length - 1;
		}
		Buttons.GetIndex("Sound Bindings").overlapText = "Sound Bindings <color=grey>[</color><color=green>" + array[BindMode] + "</color><color=grey>]</color>";
	}

	public static void BetaPlayTag(int id, float volume)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
		}
		else if (Time.time > sendEffectDelay)
		{
			object[] array = new object[3] { id, volume, false };
			object[] array2 = new object[3]
			{
				PhotonNetwork.ServerTimestamp,
				(byte)3,
				array
			};
			try
			{
				PhotonNetwork.RaiseEvent((byte)3, (object)array2, new RaiseEventOptions
				{
					Receivers = (ReceiverGroup)1
				}, SendOptions.SendUnreliable);
			}
			catch
			{
			}
			Main.RPCProtection();
			sendEffectDelay = Time.time + 0.2f;
		}
	}

	public static void SoundSpam(int soundId, bool constant = false)
	{
		if ((Main.rightGrab || constant) && Time.time > soundSpamDelay)
		{
			soundSpamDelay = Time.time + 0.1f;
			if (PhotonNetwork.InRoom)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", (RpcTarget)0, new object[3] { soundId, false, 999999f });
				Main.RPCProtection();
			}
			else
			{
				VRRig.LocalRig.PlayHandTapLocal(soundId, false, 999999f);
			}
		}
	}

	public static void JmancurlySoundSpam()
	{
		SoundSpam(Random.Range(336, 338));
	}

	public static void RandomSoundSpam()
	{
		SoundSpam(Random.Range(0, GTPlayer.Instance.materialData.Count));
	}

	public static void CrystalSoundSpam()
	{
		int[] array = new int[2]
		{
			Random.Range(40, 54),
			Random.Range(214, 221)
		};
		SoundSpam(array[Random.Range(0, 1)]);
	}

	public static void SqueakSoundSpam()
	{
		if (Time.time > soundSpamDelay)
		{
			squeakToggle = !squeakToggle;
		}
		SoundSpam(squeakToggle ? 75 : 76);
	}

	public static void SirenSoundSpam()
	{
		if (Time.time > soundSpamDelay)
		{
			sirenToggle = !sirenToggle;
		}
		SoundSpam(sirenToggle ? 48 : 50);
	}

	public static void DecreaseSoundID()
	{
		soundId--;
		if (soundId < 0)
		{
			soundId = GTPlayer.Instance.materialData.Count - 1;
		}
		Buttons.GetIndex("Custom Sound Spam").overlapText = "Custom Sound Spam <color=grey>[</color><color=green>" + soundId + "</color><color=grey>]</color>";
	}

	public static void IncreaseSoundID()
	{
		if (!Settings.isLoadingPreferences)
		{
			soundId++;
		}
		soundId %= GTPlayer.Instance.materialData.Count;
		Buttons.GetIndex("Custom Sound Spam").overlapText = "Custom Sound Spam <color=grey>[</color><color=green>" + soundId + "</color><color=grey>]</color>";
	}

	public static void CustomSoundSpam()
	{
		SoundSpam(soundId);
	}

	public static void BetaSoundSpam(int id)
	{
		if (Main.rightGrab)
		{
			BetaPlayTag(id, 999999f);
		}
	}
}
