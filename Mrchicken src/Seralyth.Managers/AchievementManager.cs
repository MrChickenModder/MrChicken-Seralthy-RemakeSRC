using System.Collections.Generic;
using System.IO;
using System.Linq;
using Seralyth.Classes.Menu;
using Seralyth.Extensions;
using Seralyth.Menu;
using Seralyth.Utilities;
using UnityEngine;
using Valve.Newtonsoft.Json.Linq;

namespace Seralyth.Managers;

public static class AchievementManager
{
	public struct Achievement
	{
		public string name;

		public string description;

		public string icon;

		public readonly JObject ToJObject()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Expected O, but got Unknown
			return new JObject
			{
				["name"] = JToken.op_Implicit(name),
				["description"] = JToken.op_Implicit(description),
				["icon"] = JToken.op_Implicit(icon)
			};
		}

		public static Achievement FromJObject(JObject obj)
		{
			return new Achievement
			{
				name = (string)obj["name"],
				description = (string)obj["description"],
				icon = (string)obj["icon"]
			};
		}
	}

	private static List<Achievement> _achievements;

	public static List<Achievement> Achievements
	{
		get
		{
			if (_achievements != null)
			{
				return _achievements;
			}
			_achievements = new List<Achievement>();
			string[] files = Directory.GetFiles("SeralythMenu/Achievements");
			string[] array = files;
			foreach (string text in array)
			{
				if (text.EndsWith(".json"))
				{
					_achievements.Add(Achievement.FromJObject(JObject.Parse(File.ReadAllText(text))));
				}
			}
			return _achievements;
		}
		set
		{
			_achievements = value;
		}
	}

	public static void EnterAchievementTab()
	{
		int count = Achievements.Count;
		List<ButtonInfo> list = new List<ButtonInfo>
		{
			new ButtonInfo
			{
				buttonText = "Exit Achievements",
				method = delegate
				{
					Buttons.CurrentCategoryName = "Main";
				},
				isTogglable = false,
				toolTip = "Returns you back to the main page."
			}
		};
		if (count <= 0)
		{
			list.Add(new ButtonInfo
			{
				buttonText = "You have no achievements.",
				label = true
			});
		}
		else
		{
			for (int num = 0; num < count; num++)
			{
				Achievement achievement = Achievements[num];
				list.Add(new ButtonInfo
				{
					buttonText = $"Achievement{num}",
					overlapText = achievement.name,
					method = delegate
					{
						Main.PromptSingle(string.Concat(str2: (achievement.icon.StartsWith("http") || achievement.icon.StartsWith("file:///")) ? achievement.icon : ((!File.Exists("SeralythMenu/" + achievement.icon)) ? ("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/" + achievement.icon) : ("file:///" + Path.GetFullPath("SeralythMenu/" + achievement.icon).Replace("\\", "/"))), str0: achievement.description, str1: "<", str3: ">"), null, "Done");
					},
					isTogglable = false,
					toolTip = achievement.description
				});
			}
		}
		Buttons.buttons[Buttons.GetCategory("Achievements")] = list.ToArray();
		Buttons.CurrentCategoryName = "Achievements";
	}

	public static bool HasAchievement(string name)
	{
		return Achievements.Any((Achievement a) => a.name == name);
	}

	public static void UnlockAchievement(Achievement achievement)
	{
		if (!HasAchievement(achievement.name))
		{
			AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Menu/achievement.ogg", "Audio/Menu/achievement.ogg", delegate(AudioClip clip)
			{
				Main.Play2DAudio(clip, (float)Main.buttonClickVolume / 10f);
			});
			NotificationManager.SendNotification("<color=grey>[</color><color=purple>ACHIEVEMENT</color><color=grey>]</color> Achievement unlocked! \"" + achievement.name + "\"");
			Achievements.Add(achievement);
			File.WriteAllText("SeralythMenu/Achievements/" + achievement.name.Hash() + ".json", ((object)achievement.ToJObject()).ToString());
		}
	}
}
