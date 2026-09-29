using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Seralyth.Classes.Menu;
using Seralyth.Managers;
using Seralyth.Menu;
using UnityEngine;
using UnityEngine.Networking;

namespace Seralyth.Mods;

public static class Quests
{
	private enum Diff
	{
		Easy,
		Medium,
		Hard
	}

	public static string activeQuestName;

	public static string activeQuestDare;

	public static Func<bool> activeQuestCheck;

	public static string activeHint;

	public static string activeDifficulty;

	public static int completedCount;

	public static float nextQuestTime;

	public static string lastCompletedName;

	public static string lastCompletedDifficulty;

	public static float lastCompletedTime;

	public static int lastCompletedLevel;

	public static bool lastWasLevelUp;

	public static int playerLevel = 1;

	public static int questsUntilNextLevel = 2;

	public static string selectedDifficulty = "Random";

	private static Random rng = new Random();

	private static string lastQuestName = "";

	private static int hintStep = 0;

	private static string lastQuestType = "";

	private static object[] lastQuestParams = new object[0];

	private static readonly string gorillaTagPhotosPath = "C:\\Users\\kalew\\OneDrive\\Pictures\\Gorilla Tag photos";

	private static string lastShownPhoto = "";

	public static string sharedWithPlayerId = null;

	public static string sharedWithPlayerName = null;

	public static bool isSharingQuest = false;

	private static string[] maps = new string[10] { "Forest", "Cave", "Beach", "Canyon", "Mountain", "City", "Clouds", "Basement", "Metropolis", "Bayou" };

	private static string[] adj = new string[26]
	{
		"Epic", "Wild", "Crazy", "Brave", "Sneaky", "Swift", "Bold", "Chill", "Lucky", "Daring",
		"Fierce", "Quick", "Sly", "Nimble", "Tough", "Frosty", "Savage", "Wicked", "Turbo", "Mega",
		"Ultra", "Shadow", "Phantom", "Blazing", "Toxic", "Insane"
	};

	private static string[] title = new string[25]
	{
		"Explorer", "Champion", "Legend", "Ninja", "Pirate", "Wizard", "Warrior", "Hunter", "Prowler", "Scout",
		"Pathfinder", "Challenger", "Daredevil", "Wanderer", "Stalker", "Lurker", "Survivor", "Nomad", "Rebel", "Ghost",
		"Dragon", "Titan", "Viking", "Samurai", "Knight"
	};

	private static string[] difficultyOrder = new string[4] { "Random", "Easy", "Medium", "Hard" };

	private static string[] difficultyColors = new string[4] { "white", "green", "yellow", "red" };

	private static string GetRandomGorillaTagPhoto()
	{
		if (!Directory.Exists(gorillaTagPhotosPath))
		{
			return null;
		}
		string[] array = (from f in Directory.GetFiles(gorillaTagPhotosPath, "*.*", SearchOption.TopDirectoryOnly)
			where f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
			select f).ToArray();
		if (array.Length == 0)
		{
			return null;
		}
		if (array.Length == 1)
		{
			return array[0];
		}
		int num = 0;
		string text;
		do
		{
			text = array[rng.Next(array.Length)];
			num++;
		}
		while (text == lastShownPhoto && num < 10);
		lastShownPhoto = text;
		return text;
	}

	private static void TakeScreenshotToFolder()
	{
		string text = gorillaTagPhotosPath;
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		string path = $"GT_Screenshot_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
		string text2 = Path.Combine(text, path);
		if ((Object)(object)Camera.main != (Object)null)
		{
			Camera.main.Render();
		}
		ScreenCapture.CaptureScreenshot(text2);
		((MonoBehaviour)CoroutineManager.instance).StartCoroutine(ScreenshotNotify(text2));
	}

	private static IEnumerator ScreenshotNotify(string path)
	{
		yield return (object)new WaitForEndOfFrame();
		NotificationManager.SendNotification("<color=grey>[</color><color=green>SCREENSHOT</color><color=grey>]</color> Screenshot saved to " + Path.GetFileName(path));
	}

	private static void GenerateAIPhoto(string prompt)
	{
		if (string.IsNullOrWhiteSpace(prompt))
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Please enter something for the AI to generate.");
		}
		else
		{
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(GenerateAIPhotoCoroutine(prompt));
		}
	}

	private static IEnumerator GenerateAIPhotoCoroutine(string prompt)
	{
		string folder = gorillaTagPhotosPath;
		if (!Directory.Exists(folder))
		{
			Directory.CreateDirectory(folder);
		}
		NotificationManager.SendNotification("<color=green>[</color><color=green>AI</color><color=grey>]</color> Generating your photo...");
		string url = "https://image.pollinations.ai/prompt/" + Uri.EscapeDataString(prompt) + "?width=1024&height=1024&nologo=true";
		UnityWebRequest request = UnityWebRequest.Get(url);
		try
		{
			request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
			yield return request.SendWebRequest();
			if ((int)request.result != 1)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Could not generate photo: " + request.error);
				yield break;
			}
			string fileName = $"GT_AI_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
			string fullPath = Path.Combine(folder, fileName);
			File.WriteAllBytes(fullPath, request.downloadHandler.data);
			NotificationManager.SendNotification("<color=green>[</color><color=green>AI</color><color=grey>]</color> Generated photo saved as " + fileName);
		}
		finally
		{
			((IDisposable)request)?.Dispose();
		}
	}

	private static ButtonInfo[] GetAllMods()
	{
		return (from b in Buttons.buttons.SelectMany((ButtonInfo[] b) => b)
			where b.isTogglable && !b.label
			select b).ToArray();
	}

	private static Diff GetDiff()
	{
		if (selectedDifficulty == "Easy")
		{
			return Diff.Easy;
		}
		if (selectedDifficulty == "Medium")
		{
			return Diff.Medium;
		}
		if (selectedDifficulty == "Hard")
		{
			return Diff.Hard;
		}
		return rng.Next(3) switch
		{
			1 => Diff.Medium, 
			0 => Diff.Easy, 
			_ => Diff.Hard, 
		};
	}

	private static string DiffLabel(Diff d)
	{
		return d switch
		{
			Diff.Easy => "<color=green>EASY</color>", 
			Diff.Medium => "<color=yellow>MEDIUM</color>", 
			_ => "<color=red>HARD</color>", 
		};
	}

	private static int DiffHeight(Diff d)
	{
		switch (d)
		{
		case Diff.Easy:
		{
			int[] array3 = new int[3] { 3, 5, 8 };
			return array3[rng.Next(array3.Length)];
		}
		case Diff.Medium:
		{
			int[] array2 = new int[4] { 10, 12, 15, 18 };
			return array2[rng.Next(array2.Length)];
		}
		default:
		{
			int[] array = new int[5] { 20, 25, 30, 35, 40 };
			return array[rng.Next(array.Length)];
		}
		}
	}

	private static float DiffDepth(Diff d)
	{
		return d switch
		{
			Diff.Easy => -2f, 
			Diff.Medium => -4f, 
			_ => -6f, 
		};
	}

	private static float DiffCloseDist(Diff d)
	{
		return d switch
		{
			Diff.Easy => Mathf.Round((float)(rng.NextDouble() * 1.5 + 1.5) * 10f) / 10f, 
			Diff.Medium => Mathf.Round((float)(rng.NextDouble() * 1.5 + 0.8) * 10f) / 10f, 
			_ => Mathf.Round((float)(rng.NextDouble() * 0.5 + 0.3) * 10f) / 10f, 
		};
	}

	private static float DiffFarDist(Diff d)
	{
		return d switch
		{
			Diff.Easy => 8f, 
			Diff.Medium => 12f, 
			_ => 18f, 
		};
	}

	private static int DiffPlayers(Diff d)
	{
		return d switch
		{
			Diff.Easy => rng.Next(2, 5), 
			Diff.Medium => rng.Next(4, 8), 
			_ => rng.Next(7, 12), 
		};
	}

	private static int DiffCrowdPlayers(Diff d)
	{
		return d switch
		{
			Diff.Easy => rng.Next(5, 8), 
			Diff.Medium => rng.Next(8, 11), 
			_ => rng.Next(10, 16), 
		};
	}

	private static float DiffHoldTime(Diff d)
	{
		return d switch
		{
			Diff.Easy => 3f, 
			Diff.Medium => 5f, 
			_ => 8f, 
		};
	}

	private static int DiffModCount(Diff d, int max)
	{
		return d switch
		{
			Diff.Easy => rng.Next(1, Math.Min(4, max + 1)), 
			Diff.Medium => rng.Next(3, Math.Min(7, max + 1)), 
			_ => rng.Next(6, Math.Min(max + 1, 15)), 
		};
	}

	public static void GenerateQuest()
	{
		string text = "";
		int num = 0;
		while (text == "" || text == lastQuestName)
		{
			text = GenerateRandomQuest();
			num++;
			if (num > 30)
			{
				break;
			}
		}
		lastQuestName = text;
		nextQuestTime = 0f;
		hintStep = 0;
		lastCompletedName = null;
		UpdateVRButtons();
		NotificationManager.SendNotification("<color=grey>[</color><color=yellow>NEW " + activeDifficulty + " QUEST</color><color=grey>]</color> <color=green>" + activeQuestName + "</color> - " + activeQuestDare);
		SendQuestSync();
	}

	private static string GenerateRandomQuest()
	{
		Diff diff = GetDiff();
		string name = ((rng.Next(3) == 0) ? (adj[rng.Next(adj.Length)] + " " + title[rng.Next(title.Length)]) : title[rng.Next(title.Length)]);
		ButtonInfo[] allMods = GetAllMods();
		if (allMods.Length != 0 && rng.Next(2) == 0)
		{
			return ModPlusGT(name, diff, allMods);
		}
		return PureGT(name, diff);
	}

	private static string PureGT(string name, Diff diff)
	{
		return rng.Next(12) switch
		{
			0 => Q_GoToMap(name, diff), 
			1 => Q_Climb(name, diff), 
			2 => Q_GetLow(name, diff), 
			3 => Q_NearPlayer(name, diff), 
			4 => Q_FarPlayer(name, diff), 
			5 => Q_JoinRoom(name, diff), 
			6 => Q_TwoMaps(name, diff), 
			7 => Q_SteadyHeight(name, diff), 
			8 => Q_VeryClose(name, diff), 
			9 => Q_Crowded(name, diff), 
			10 => Q_MapClimb(name, diff), 
			11 => Q_MapExplore(name, diff), 
			_ => Q_GoToMap(name, diff), 
		};
	}

	private static string ModPlusGT(string name, Diff diff, ButtonInfo[] mods)
	{
		ButtonInfo mod = mods[rng.Next(mods.Length)];
		return rng.Next(10) switch
		{
			0 => Q_ModGoToMap(name, diff, mod), 
			1 => Q_ModClimb(name, diff, mod), 
			2 => Q_ModNear(name, diff, mod), 
			3 => Q_ModMapClimb(name, diff, mod), 
			4 => Q_TwoMods(name, diff, mod), 
			5 => Q_ModCount(name, diff, mod), 
			6 => Q_ModSteady(name, diff, mod), 
			7 => Q_ModExplore(name, diff, mod), 
			8 => Q_ModCombo(name, diff, mod), 
			9 => Q_ModCrowded(name, diff, mod), 
			_ => Q_ModGoToMap(name, diff, mod), 
		};
	}

	private static string Q_GoToMap(string name, Diff diff)
	{
		string map = PickMap();
		activeQuestName = name;
		activeQuestDare = "Go to the " + map + " map.";
		activeDifficulty = DiffLabel(diff);
		activeHint = "Hint: Look for the portal or tunnel that leads to " + map + ".";
		activeQuestCheck = () => IsInMap(map);
		lastQuestType = "gt_map";
		lastQuestParams = new object[1] { map };
		return "gt_map";
	}

	private static string Q_Climb(string name, Diff diff)
	{
		int h = DiffHeight(diff);
		activeQuestName = name;
		activeQuestDare = $"Climb above {h}m.";
		activeDifficulty = DiffLabel(diff);
		activeHint = ((h > 20) ? "Hint: Try scaling the tallest structures or trees." : "Hint: Climb any surface to gain height.");
		activeQuestCheck = () => VRRig.LocalRig.headMesh.transform.position.y > (float)h;
		lastQuestType = "gt_climb";
		lastQuestParams = new object[1] { h };
		return "gt_climb";
	}

	private static string Q_GetLow(string name, Diff diff)
	{
		float depth = DiffDepth(diff);
		activeQuestName = name;
		activeQuestDare = $"Get below {depth}m.";
		activeDifficulty = DiffLabel(diff);
		activeHint = "Hint: Go underground or into caves and get low.";
		activeQuestCheck = () => VRRig.LocalRig.headMesh.transform.position.y < depth;
		lastQuestType = "gt_low";
		lastQuestParams = new object[1] { depth };
		return "gt_low";
	}

	private static string Q_NearPlayer(string name, Diff diff)
	{
		float dist = DiffCloseDist(diff);
		activeQuestName = name;
		activeQuestDare = $"Get within {dist}m of another player.";
		activeDifficulty = DiffLabel(diff);
		activeHint = "Hint: Walk up to someone and get close to them.";
		activeQuestCheck = delegate
		{
			float closestDist = GetClosestDist();
			return closestDist > 0f && closestDist < dist;
		};
		lastQuestType = "gt_near";
		lastQuestParams = new object[1] { dist };
		return "gt_near";
	}

	private static string Q_FarPlayer(string name, Diff diff)
	{
		float dist = DiffFarDist(diff);
		activeQuestName = name;
		activeQuestDare = $"Stay {dist}m+ from all players.";
		activeDifficulty = DiffLabel(diff);
		activeHint = "Hint: Find a quiet corner away from everyone.";
		activeQuestCheck = delegate
		{
			float closestDist = GetClosestDist();
			return closestDist > dist || closestDist == 0f;
		};
		lastQuestType = "gt_far";
		lastQuestParams = new object[1] { dist };
		return "gt_far";
	}

	private static string Q_JoinRoom(string name, Diff diff)
	{
		int players = DiffPlayers(diff);
		activeQuestName = name;
		activeQuestDare = $"Join a room with {players}+ players.";
		activeDifficulty = DiffLabel(diff);
		activeHint = "Hint: Join a public lobby or one with friends.";
		activeQuestCheck = () => PhotonNetwork.InRoom && PhotonNetwork.PlayerListOthers.Length >= players;
		lastQuestType = "gt_join";
		lastQuestParams = new object[1] { players };
		return "gt_join";
	}

	private static string Q_TwoMaps(string name, Diff diff)
	{
		string m1 = PickMap();
		string m2 = PickMap();
		while (m2 == m1)
		{
			m2 = PickMap();
		}
		activeQuestName = name;
		activeQuestDare = "Visit " + m1 + " then " + m2 + ".";
		activeDifficulty = DiffLabel(diff);
		activeHint = "Hint: Go to " + m1 + " first, then travel to " + m2 + ".";
		bool hit1 = false;
		activeQuestCheck = delegate
		{
			if (!hit1 && IsInMap(m1))
			{
				hit1 = true;
			}
			return hit1 && IsInMap(m2);
		};
		lastQuestType = "gt_twomap";
		lastQuestParams = new object[2] { m1, m2 };
		return "gt_twomap";
	}

	private static string Q_SteadyHeight(string name, Diff diff)
	{
		int h = DiffHeight(diff);
		float hold = DiffHoldTime(diff);
		activeQuestName = name;
		activeQuestDare = $"Stay above {h}m for {hold}s.";
		activeDifficulty = DiffLabel(diff);
		activeHint = "Hint: Climb up and hold your position.";
		float timer = 0f;
		activeQuestCheck = delegate
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			if (VRRig.LocalRig.headMesh.transform.position.y > (float)h)
			{
				timer += Time.deltaTime;
				return timer >= hold;
			}
			timer = 0f;
			return false;
		};
		lastQuestType = "gt_steady";
		lastQuestParams = new object[2] { h, hold };
		return "gt_steady";
	}

	private static string Q_VeryClose(string name, Diff diff)
	{
		float dist = diff switch
		{
			Diff.Medium => 0.5f, 
			Diff.Hard => 0.3f, 
			_ => 0.8f, 
		};
		activeQuestName = name;
		activeQuestDare = $"Get within {dist}m of another player.";
		activeDifficulty = DiffLabel(diff);
		activeHint = "Hint: Walk right up to someone.";
		activeQuestCheck = delegate
		{
			float closestDist = GetClosestDist();
			return closestDist > 0f && closestDist < dist;
		};
		lastQuestType = "gt_veryclose";
		lastQuestParams = new object[1] { dist };
		return "gt_veryclose";
	}

	private static string Q_Crowded(string name, Diff diff)
	{
		int players = DiffCrowdPlayers(diff);
		activeQuestName = name;
		activeQuestDare = $"Find a room with {players}+ players.";
		activeDifficulty = DiffLabel(diff);
		activeHint = "Hint: Try joining a popular public lobby.";
		activeQuestCheck = () => PhotonNetwork.InRoom && PhotonNetwork.PlayerListOthers.Length >= players;
		lastQuestType = "gt_crowded";
		lastQuestParams = new object[1] { players };
		return "gt_crowded";
	}

	private static string Q_MapClimb(string name, Diff diff)
	{
		string map = PickMap();
		int h = DiffHeight(diff);
		activeQuestName = name;
		activeQuestDare = $"Go to {map} and climb {h}m+.";
		activeDifficulty = DiffLabel(diff);
		activeHint = "Hint: Travel to " + map + " then start climbing.";
		activeQuestCheck = () => IsInMap(map) && VRRig.LocalRig.headMesh.transform.position.y > (float)h;
		lastQuestType = "gt_mapclimb";
		lastQuestParams = new object[2] { map, h };
		return "gt_mapclimb";
	}

	private static string Q_MapExplore(string name, Diff diff)
	{
		string map = PickMap();
		int h = DiffHeight(diff);
		activeQuestName = name;
		activeQuestDare = $"Explore {map} and climb {h}m+.";
		activeDifficulty = DiffLabel(diff);
		activeHint = "Hint: Head to " + map + " and explore upward.";
		activeQuestCheck = () => IsInMap(map) && VRRig.LocalRig.headMesh.transform.position.y > (float)h;
		lastQuestType = "gt_explore";
		lastQuestParams = new object[2] { map, h };
		return "gt_explore";
	}

	private static string Q_ModGoToMap(string name, Diff diff, ButtonInfo mod)
	{
		string map = PickMap();
		activeQuestName = name;
		activeQuestDare = "Go to " + map + " with " + mod.buttonText + " enabled.";
		activeDifficulty = DiffLabel(diff);
		activeHint = "Hint: Enable " + mod.buttonText + " first, then go to " + map + ".";
		activeQuestCheck = () => IsInMap(map) && mod.enabled;
		lastQuestType = "mgt_map";
		lastQuestParams = new object[2] { map, mod.buttonText };
		return "mgt_map";
	}

	private static string Q_ModClimb(string name, Diff diff, ButtonInfo mod)
	{
		int h = DiffHeight(diff);
		activeQuestName = name;
		activeQuestDare = $"Climb above {h}m with {mod.buttonText}.";
		activeDifficulty = DiffLabel(diff);
		activeHint = "Hint: Turn on " + mod.buttonText + " then climb up.";
		activeQuestCheck = () => mod.enabled && VRRig.LocalRig.headMesh.transform.position.y > (float)h;
		lastQuestType = "mgt_climb";
		lastQuestParams = new object[2] { h, mod.buttonText };
		return "mgt_climb";
	}

	private static string Q_ModNear(string name, Diff diff, ButtonInfo mod)
	{
		float dist = DiffCloseDist(diff);
		activeQuestName = name;
		activeQuestDare = $"Get within {dist}m of someone with {mod.buttonText}.";
		activeDifficulty = DiffLabel(diff);
		activeHint = "Hint: Enable " + mod.buttonText + " and approach a player.";
		activeQuestCheck = () => mod.enabled && GetClosestDist() < dist && GetClosestDist() > 0f;
		lastQuestType = "mgt_near";
		lastQuestParams = new object[2] { dist, mod.buttonText };
		return "mgt_near";
	}

	private static string Q_ModMapClimb(string name, Diff diff, ButtonInfo mod)
	{
		string map = PickMap();
		int h = DiffHeight(diff);
		activeQuestName = name;
		activeQuestDare = $"Go to {map} and climb {h}m+ with {mod.buttonText}.";
		activeDifficulty = DiffLabel(diff);
		activeHint = "Hint: Enable " + mod.buttonText + ", go to " + map + ", then climb.";
		activeQuestCheck = () => IsInMap(map) && mod.enabled && VRRig.LocalRig.headMesh.transform.position.y > (float)h;
		lastQuestType = "mgt_mapclimb";
		lastQuestParams = new object[3] { map, h, mod.buttonText };
		return "mgt_mapclimb";
	}

	private static string Q_TwoMods(string name, Diff diff, ButtonInfo mod)
	{
		ButtonInfo[] allMods = GetAllMods();
		ButtonInfo m2 = allMods[rng.Next(allMods.Length)];
		int num = 0;
		while (m2 == mod && num < 10)
		{
			m2 = allMods[rng.Next(allMods.Length)];
			num++;
		}
		if (m2 == mod)
		{
			return Q_ModGoToMap(name, diff, mod);
		}
		int num2 = rng.Next(3);
		switch (num2)
		{
		case 0:
		{
			string map = PickMap();
			activeQuestName = name;
			activeQuestDare = "Go to " + map + " with " + mod.buttonText + " and " + m2.buttonText + ".";
			activeDifficulty = DiffLabel(diff);
			activeHint = "Hint: Enable both " + mod.buttonText + " and " + m2.buttonText + ", then go to " + map + ".";
			activeQuestCheck = () => IsInMap(map) && mod.enabled && m2.enabled;
			lastQuestType = "mgt_two_map";
			lastQuestParams = new object[3] { map, mod.buttonText, m2.buttonText };
			break;
		}
		case 1:
		{
			int h = DiffHeight(diff);
			activeQuestName = name;
			activeQuestDare = $"Climb {h}m+ with {mod.buttonText} and {m2.buttonText}.";
			activeDifficulty = DiffLabel(diff);
			activeHint = "Hint: Turn on both mods then climb high.";
			activeQuestCheck = () => mod.enabled && m2.enabled && VRRig.LocalRig.headMesh.transform.position.y > (float)h;
			lastQuestType = "mgt_two_climb";
			lastQuestParams = new object[3] { h, mod.buttonText, m2.buttonText };
			break;
		}
		case 2:
		{
			float dist = DiffCloseDist(diff);
			activeQuestName = name;
			activeQuestDare = $"Get within {dist}m with {mod.buttonText} and {m2.buttonText}.";
			activeDifficulty = DiffLabel(diff);
			activeHint = "Hint: Enable both mods and get close to a player.";
			activeQuestCheck = () => mod.enabled && m2.enabled && GetClosestDist() < dist && GetClosestDist() > 0f;
			lastQuestType = "mgt_two_near";
			lastQuestParams = new object[3] { dist, mod.buttonText, m2.buttonText };
			break;
		}
		}
		return "mgt_two";
	}

	private static string Q_ModCount(string name, Diff diff, ButtonInfo mod)
	{
		ButtonInfo[] mods = GetAllMods();
		int count = DiffModCount(diff, mods.Length);
		activeQuestName = name;
		activeQuestDare = $"Enable {count} mods at once.";
		activeDifficulty = DiffLabel(diff);
		activeHint = $"Hint: Turn on {count} different mods from any category.";
		activeQuestCheck = () => mods.Count((ButtonInfo b) => b.enabled) >= count;
		lastQuestType = "mgt_count";
		lastQuestParams = new object[1] { count };
		return "mgt_count";
	}

	private static string Q_ModSteady(string name, Diff diff, ButtonInfo mod)
	{
		int h = DiffHeight(diff);
		float hold = DiffHoldTime(diff);
		activeQuestName = name;
		activeQuestDare = $"Stay above {h}m for {hold}s with {mod.buttonText}.";
		activeDifficulty = DiffLabel(diff);
		activeHint = "Hint: Enable " + mod.buttonText + ", climb up, and hold position.";
		float timer = 0f;
		activeQuestCheck = delegate
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			if (mod.enabled && VRRig.LocalRig.headMesh.transform.position.y > (float)h)
			{
				timer += Time.deltaTime;
				return timer >= hold;
			}
			timer = 0f;
			return false;
		};
		lastQuestType = "mgt_steady";
		lastQuestParams = new object[3] { h, hold, mod.buttonText };
		return "mgt_steady";
	}

	private static string Q_ModExplore(string name, Diff diff, ButtonInfo mod)
	{
		string map = PickMap();
		int h = DiffHeight(diff);
		activeQuestName = name;
		activeQuestDare = $"Explore {map} and climb {h}m+ with {mod.buttonText}.";
		activeDifficulty = DiffLabel(diff);
		activeHint = "Hint: Turn on " + mod.buttonText + " and explore " + map + ".";
		activeQuestCheck = () => IsInMap(map) && mod.enabled && VRRig.LocalRig.headMesh.transform.position.y > (float)h;
		lastQuestType = "mgt_explore";
		lastQuestParams = new object[3] { map, h, mod.buttonText };
		return "mgt_explore";
	}

	private static string Q_ModCombo(string name, Diff diff, ButtonInfo mod)
	{
		ButtonInfo[] allMods = GetAllMods();
		ButtonInfo m2 = allMods[rng.Next(allMods.Length)];
		int num = 0;
		while (m2 == mod && num < 10)
		{
			m2 = allMods[rng.Next(allMods.Length)];
			num++;
		}
		if (m2 == mod)
		{
			return Q_ModGoToMap(name, diff, mod);
		}
		string map = PickMap();
		float dist = DiffCloseDist(diff);
		activeQuestName = name;
		activeQuestDare = $"Go to {map}, get within {dist}m with {mod.buttonText} and {m2.buttonText}.";
		activeDifficulty = DiffLabel(diff);
		activeHint = "Hint: Enable both mods, go to " + map + ", approach a player.";
		activeQuestCheck = () => IsInMap(map) && mod.enabled && m2.enabled && GetClosestDist() < dist && GetClosestDist() > 0f;
		lastQuestType = "mgt_combo";
		lastQuestParams = new object[4] { map, dist, mod.buttonText, m2.buttonText };
		return "mgt_combo";
	}

	private static string Q_ModCrowded(string name, Diff diff, ButtonInfo mod)
	{
		int players = DiffCrowdPlayers(diff);
		activeQuestName = name;
		activeQuestDare = $"Find {players}+ players with {mod.buttonText}.";
		activeDifficulty = DiffLabel(diff);
		activeHint = "Hint: Enable " + mod.buttonText + " and join a busy lobby.";
		activeQuestCheck = () => mod.enabled && PhotonNetwork.InRoom && PhotonNetwork.PlayerListOthers.Length >= players;
		lastQuestType = "mgt_crowded";
		lastQuestParams = new object[2] { players, mod.buttonText };
		return "mgt_crowded";
	}

	private static string PickMap()
	{
		return maps[rng.Next(maps.Length)];
	}

	public static bool IsInMap(string mapName)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return false;
		}
		string text = ((object)VRRig.LocalRig.zoneEntity.currentZone/*cast due to .constrained prefix*/).ToString();
		if (text.IndexOf(mapName, StringComparison.OrdinalIgnoreCase) >= 0)
		{
			return true;
		}
		string text2 = PhotonNetwork.CurrentRoom.Name ?? "";
		return text2.IndexOf(mapName, StringComparison.OrdinalIgnoreCase) >= 0;
	}

	private static float GetClosestDist()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		float num = float.MaxValue;
		Vector3 position = VRRig.LocalRig.headMesh.transform.position;
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!((Object)(object)activeRig == (Object)null) && !((Object)(object)activeRig == (Object)(object)VRRig.LocalRig))
			{
				float num2 = Vector3.Distance(position, activeRig.headMesh.transform.position);
				if (num2 < num)
				{
					num = num2;
				}
			}
		}
		return (num == float.MaxValue) ? 0f : num;
	}

	public static void SendQuestSync()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		if (activeQuestName != null && !(lastQuestType == ""))
		{
			FriendManager.ExecuteCommand("quest", new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)0
			}, new object[6]
			{
				activeQuestName,
				activeQuestDare ?? "",
				activeDifficulty ?? "",
				activeHint ?? "",
				lastQuestType,
				lastQuestParams
			});
		}
	}

	public static void ReceiveQuestSync(object[] data)
	{
		if (data.Length >= 6)
		{
			string text = (string)data[0];
			string text2 = (string)data[1];
			string text3 = (string)data[2];
			string text4 = (string)data[3];
			string type = (string)data[4];
			object[] p = (object[])data[5];
			activeQuestName = text;
			activeQuestDare = text2;
			activeDifficulty = text3;
			activeHint = text4;
			lastQuestType = type;
			lastQuestParams = p;
			hintStep = 0;
			Func<bool> func = RebuildCheck(type, p);
			activeQuestCheck = func;
			nextQuestTime = 0f;
			lastCompletedName = null;
			UpdateVRButtons();
			NotificationManager.SendNotification("<color=grey>[</color><color=yellow>SHARED QUEST</color><color=grey>]</color> <color=green>" + text + "</color> - " + text2);
		}
	}

	private static Func<bool> RebuildCheck(string type, object[] p)
	{
		switch (type)
		{
		case "gt_map":
			return () => IsInMap((string)p[0]);
		case "gt_climb":
			return () => VRRig.LocalRig.headMesh.transform.position.y > Convert.ToSingle(p[0]);
		case "gt_low":
			return () => VRRig.LocalRig.headMesh.transform.position.y < Convert.ToSingle(p[0]);
		case "gt_near":
		{
			float d12 = Convert.ToSingle(p[0]);
			return delegate
			{
				float closestDist = GetClosestDist();
				return closestDist > 0f && closestDist < d12;
			};
		}
		case "gt_far":
		{
			float d11 = Convert.ToSingle(p[0]);
			return delegate
			{
				float closestDist = GetClosestDist();
				return closestDist > d11 || closestDist == 0f;
			};
		}
		case "gt_join":
		{
			int minP5 = Convert.ToInt32(p[0]);
			return () => PhotonNetwork.InRoom && PhotonNetwork.PlayerListOthers.Length >= minP5;
		}
		case "gt_twomap":
		{
			string m1 = (string)p[0];
			string m2 = (string)p[1];
			bool hit = false;
			return delegate
			{
				if (!hit && IsInMap(m1))
				{
					hit = true;
				}
				return hit && IsInMap(m2);
			};
		}
		case "gt_steady":
		{
			int h15 = Convert.ToInt32(p[0]);
			float hold3 = Convert.ToSingle(p[1]);
			float t3 = 0f;
			return delegate
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				if (VRRig.LocalRig.headMesh.transform.position.y > (float)h15)
				{
					t3 += Time.deltaTime;
					return t3 >= hold3;
				}
				t3 = 0f;
				return false;
			};
		}
		case "gt_veryclose":
		{
			float d10 = Convert.ToSingle(p[0]);
			return delegate
			{
				float closestDist = GetClosestDist();
				return closestDist > 0f && closestDist < d10;
			};
		}
		case "gt_crowded":
		{
			int minP4 = Convert.ToInt32(p[0]);
			return () => PhotonNetwork.InRoom && PhotonNetwork.PlayerListOthers.Length >= minP4;
		}
		case "gt_mapclimb":
		{
			string mp13 = (string)p[0];
			int h14 = Convert.ToInt32(p[1]);
			return () => IsInMap(mp13) && VRRig.LocalRig.headMesh.transform.position.y > (float)h14;
		}
		case "gt_explore":
		{
			string mp12 = (string)p[0];
			int h13 = Convert.ToInt32(p[1]);
			return () => IsInMap(mp12) && VRRig.LocalRig.headMesh.transform.position.y > (float)h13;
		}
		case "mgt_map":
		{
			string mp11 = (string)p[0];
			string modName15 = (string)p[1];
			ButtonInfo md29 = FindMod(modName15);
			return () => IsInMap(mp11) && (md29 == null || md29.enabled);
		}
		case "mgt_climb":
		{
			int h12 = Convert.ToInt32(p[0]);
			string modName14 = (string)p[1];
			ButtonInfo md28 = FindMod(modName14);
			return () => (md28 == null || md28.enabled) && VRRig.LocalRig.headMesh.transform.position.y > (float)h12;
		}
		case "mgt_near":
		{
			float d9 = Convert.ToSingle(p[0]);
			string modName13 = (string)p[1];
			ButtonInfo md27 = FindMod(modName13);
			return () => (md27 == null || md27.enabled) && GetClosestDist() < d9 && GetClosestDist() > 0f;
		}
		case "mgt_mapclimb":
		{
			string mp10 = (string)p[0];
			int h11 = Convert.ToInt32(p[1]);
			string modName12 = (string)p[2];
			ButtonInfo md26 = FindMod(modName12);
			return () => IsInMap(mp10) && (md26 == null || md26.enabled) && VRRig.LocalRig.headMesh.transform.position.y > (float)h11;
		}
		case "mgt_two_map":
		{
			string mp9 = (string)p[0];
			string modName10 = (string)p[1];
			string modName11 = (string)p[2];
			ButtonInfo md24 = FindMod(modName10);
			ButtonInfo md25 = FindMod(modName11);
			return () => IsInMap(mp9) && (md24 == null || md24.enabled) && (md25 == null || md25.enabled);
		}
		case "mgt_two_climb":
		{
			int h10 = Convert.ToInt32(p[0]);
			string modName8 = (string)p[1];
			string modName9 = (string)p[2];
			ButtonInfo md22 = FindMod(modName8);
			ButtonInfo md23 = FindMod(modName9);
			return () => (md22 == null || md22.enabled) && (md23 == null || md23.enabled) && VRRig.LocalRig.headMesh.transform.position.y > (float)h10;
		}
		case "mgt_two_near":
		{
			float d8 = Convert.ToSingle(p[0]);
			string modName6 = (string)p[1];
			string modName7 = (string)p[2];
			ButtonInfo md20 = FindMod(modName6);
			ButtonInfo md21 = FindMod(modName7);
			return () => (md20 == null || md20.enabled) && (md21 == null || md21.enabled) && GetClosestDist() < d8 && GetClosestDist() > 0f;
		}
		case "mgt_count":
		{
			int cnt = Convert.ToInt32(p[0]);
			return () => GetAllMods().Count((ButtonInfo b) => b.enabled) >= cnt;
		}
		case "mgt_steady":
		{
			int h9 = Convert.ToInt32(p[0]);
			float hold2 = Convert.ToSingle(p[1]);
			string modName5 = (string)p[2];
			ButtonInfo md19 = FindMod(modName5);
			float t2 = 0f;
			return delegate
			{
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				if ((md19 == null || md19.enabled) && VRRig.LocalRig.headMesh.transform.position.y > (float)h9)
				{
					t2 += Time.deltaTime;
					return t2 >= hold2;
				}
				t2 = 0f;
				return false;
			};
		}
		case "mgt_explore":
		{
			string mp8 = (string)p[0];
			int h8 = Convert.ToInt32(p[1]);
			string modName4 = (string)p[2];
			ButtonInfo md18 = FindMod(modName4);
			return () => IsInMap(mp8) && (md18 == null || md18.enabled) && VRRig.LocalRig.headMesh.transform.position.y > (float)h8;
		}
		case "mgt_combo":
		{
			string mp7 = (string)p[0];
			float d7 = Convert.ToSingle(p[1]);
			string modName2 = (string)p[2];
			string modName3 = (string)p[3];
			ButtonInfo md16 = FindMod(modName2);
			ButtonInfo md17 = FindMod(modName3);
			return () => IsInMap(mp7) && (md16 == null || md16.enabled) && (md17 == null || md17.enabled) && GetClosestDist() < d7 && GetClosestDist() > 0f;
		}
		case "mgt_crowded":
		{
			int minP3 = Convert.ToInt32(p[0]);
			string modName = (string)p[1];
			ButtonInfo md15 = FindMod(modName);
			return () => (md15 == null || md15.enabled) && PhotonNetwork.InRoom && PhotonNetwork.PlayerListOthers.Length >= minP3;
		}
		default:
			return () => false;
		}
	}

	private static ButtonInfo FindMod(string modName)
	{
		return GetAllMods().FirstOrDefault((ButtonInfo b) => b.buttonText == modName);
	}

	public static void ShareQuestWith(string targetUserId, string targetName)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		if (activeQuestName == null || lastQuestType == "")
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=yellow>QUEST</color><color=grey>]</color> You need an active quest to share!");
			return;
		}
		FriendManager.ExecuteCommand("sharequest", new RaiseEventOptions
		{
			Receivers = (ReceiverGroup)0
		}, new object[8]
		{
			targetUserId,
			PhotonNetwork.NickName,
			activeQuestName,
			activeQuestDare ?? "",
			activeDifficulty ?? "",
			activeHint ?? "",
			lastQuestType,
			lastQuestParams
		});
		NotificationManager.SendNotification("<color=grey>[</color><color=yellow>QUEST</color><color=grey>]</color> Quest share request sent to <color=green>" + targetName + "</color>!");
	}

	public static void AcceptShareQuest(string senderName, string senderId, object[] questData)
	{
		string text = (string)questData[0];
		string text2 = (string)questData[1];
		string text3 = (string)questData[2];
		string text4 = (string)questData[3];
		string type = (string)questData[4];
		object[] p = (object[])questData[5];
		activeQuestName = text;
		activeQuestDare = text2;
		activeDifficulty = text3;
		activeHint = text4;
		lastQuestType = type;
		lastQuestParams = p;
		hintStep = 0;
		activeQuestCheck = RebuildCheck(type, p);
		nextQuestTime = 0f;
		lastCompletedName = null;
		isSharingQuest = true;
		sharedWithPlayerId = senderId;
		sharedWithPlayerName = senderName;
		UpdateVRButtons();
		NotificationManager.SendNotification("<color=grey>[</color><color=yellow>SHARED QUEST</color><color=grey>]</color> You're now doing <color=green>" + text + "</color> with <color=green>" + senderName + "</color>!");
	}

	public static void StopSharingQuest()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		if (isSharingQuest)
		{
			FriendManager.ExecuteCommand("stopsharequest", new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)0
			}, new object[1] { sharedWithPlayerId });
			isSharingQuest = false;
			sharedWithPlayerId = null;
			sharedWithPlayerName = null;
			UpdateVRButtons();
			NotificationManager.SendNotification("<color=grey>[</color><color=yellow>QUEST</color><color=grey>]</color> Stopped sharing quest.");
		}
	}

	public static void HandleQuestCompletedByFriend(string friendName)
	{
		NotificationManager.SendNotification("<color=grey>[</color><color=yellow>QUEST</color><color=grey>]</color> <color=green>" + friendName + "</color> completed the shared quest!");
	}

	public static void HandleStopShareQuest()
	{
		isSharingQuest = false;
		sharedWithPlayerId = null;
		sharedWithPlayerName = null;
		UpdateVRButtons();
		NotificationManager.SendNotification("<color=grey>[</color><color=yellow>QUEST</color><color=grey>]</color> Your friend stopped sharing quests.");
	}

	public static void ShowQuestDetails()
	{
		string text = activeDifficulty + " <color=yellow>" + activeQuestName + "</color>\n\n" + activeQuestDare + "\n\n<color=grey>Hint: " + activeHint + "</color>";
		string randomGorillaTagPhoto = GetRandomGorillaTagPhoto();
		if (randomGorillaTagPhoto != null)
		{
			string text2 = "file:///" + randomGorillaTagPhoto.Replace("\\", "/");
			Main.PromptSingle(text + "\n\n<" + text2 + ">", null, "Ok");
			return;
		}
		Main.Prompt("Looks like you don't have a Gorilla Tag photo. Do you want to take one?", delegate
		{
			Main.Prompt("How would you like to get your photo?", delegate
			{
				TakeScreenshotToFolder();
			}, delegate
			{
				Main.PromptSingleText("What would you like the AI to create? Type a description and it will generate your photo.", delegate
				{
					GenerateAIPhoto(Main.keyboardInput);
				}, "Generate");
			}, "Take a Picture", "AI Create");
		});
	}

	public static void CheckQuests()
	{
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Expected O, but got Unknown
		float time = Time.time;
		if (activeQuestCheck == null)
		{
			if (time >= nextQuestTime)
			{
				GenerateQuest();
			}
		}
		else if (activeQuestCheck())
		{
			completedCount++;
			questsUntilNextLevel--;
			lastCompletedName = activeQuestName;
			lastCompletedDifficulty = activeDifficulty;
			lastCompletedTime = time;
			lastCompletedLevel = playerLevel;
			lastWasLevelUp = false;
			if (questsUntilNextLevel <= 0)
			{
				playerLevel++;
				questsUntilNextLevel = 2;
				lastWasLevelUp = true;
				NotificationManager.SendNotification($"<color=grey>[</color><color=green>LEVEL UP</color><color=grey>]</color> You are now <color=green>Lvl {playerLevel}</color>!");
			}
			NotificationManager.SendNotification($"<color=grey>[</color><color=green>QUEST COMPLETE</color><color=grey>]</color> {activeDifficulty} <color=yellow>{activeQuestName}</color>! ({questsUntilNextLevel} more to lvl {playerLevel + 1})");
			VRRig.LocalRig.PlayHandTapLocal(50, true, 0.6f);
			VRRig.LocalRig.PlayHandTapLocal(50, false, 0.6f);
			if (isSharingQuest && sharedWithPlayerId != null)
			{
				FriendManager.ExecuteCommand("questcompleted", new RaiseEventOptions
				{
					Receivers = (ReceiverGroup)0
				}, new object[2]
				{
					sharedWithPlayerId,
					PhotonNetwork.NickName
				});
			}
			activeQuestCheck = null;
			activeQuestName = null;
			activeQuestDare = null;
			activeHint = null;
			activeDifficulty = null;
			hintStep = 0;
			isSharingQuest = false;
			sharedWithPlayerId = null;
			sharedWithPlayerName = null;
			nextQuestTime = time + 60f;
			UpdateVRButtons();
		}
	}

	public static void GiveHint()
	{
		if (activeHint != null)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=green>HINT</color><color=grey>]</color> <color=white>" + activeHint + "</color>");
		}
	}

	public static void ChangeDifficulty(bool forward = true)
	{
		int num = Array.IndexOf(difficultyOrder, selectedDifficulty);
		if (num < 0)
		{
			num = 0;
		}
		num = (forward ? ((num + 1) % difficultyOrder.Length) : ((num - 1 + difficultyOrder.Length) % difficultyOrder.Length));
		selectedDifficulty = difficultyOrder[num];
		Buttons.GetIndex("Change Quest Difficulty").overlapText = "Change Quest Difficulty <color=grey>[</color><color=" + difficultyColors[num] + ">" + selectedDifficulty + "</color><color=grey>]</color>";
		NotificationManager.SendNotification("<color=grey>[</color><color=yellow>DIFF</color><color=grey>]</color> Difficulty set to <color=" + difficultyColors[num] + ">" + selectedDifficulty + "</color>");
	}

	public static void ResetAllQuests()
	{
		activeQuestCheck = null;
		activeQuestName = null;
		activeQuestDare = null;
		activeHint = null;
		activeDifficulty = null;
		hintStep = 0;
		completedCount = 0;
		playerLevel = 1;
		questsUntilNextLevel = 2;
		nextQuestTime = Time.time + 60f;
		lastCompletedName = null;
		UpdateVRButtons();
		NotificationManager.SendNotification("<color=grey>[</color><color=yellow>QUEST</color><color=grey>]</color> Reset! Level back to 1. New quest in 60 seconds.");
	}

	public static void UpdateVRButtons()
	{
		int category = Buttons.GetCategory("Quest Mods");
		if (category < 0)
		{
			return;
		}
		List<ButtonInfo> list = new List<ButtonInfo>();
		list.Add(new ButtonInfo
		{
			buttonText = "Exit Quests",
			method = delegate
			{
				Buttons.CurrentCategoryName = "Main";
			},
			isTogglable = false,
			toolTip = "Returns you back to the main page.",
			legal = true
		});
		list.Add(new ButtonInfo
		{
			buttonText = $"Level: {playerLevel}",
			overlapText = $"Level <color=grey>[</color><color=green>{playerLevel}</color><color=grey>]</color> ({2 - questsUntilNextLevel}/2)",
			isTogglable = false,
			toolTip = "Your quest level goes up every 2 completions. Infinite levels!",
			legal = true
		});
		string text = ((selectedDifficulty == "Random") ? "white" : ((selectedDifficulty == "Easy") ? "green" : ((selectedDifficulty == "Medium") ? "yellow" : "red")));
		list.Add(new ButtonInfo
		{
			buttonText = "Change Quest Difficulty",
			overlapText = "Change Quest Difficulty <color=grey>[</color><color=" + text + ">" + selectedDifficulty + "</color><color=grey>]</color>",
			method = delegate
			{
				ChangeDifficulty();
			},
			enableMethod = delegate
			{
				ChangeDifficulty();
			},
			disableMethod = delegate
			{
				ChangeDifficulty(forward: false);
			},
			incremental = true,
			isTogglable = false,
			toolTip = "Changes the quest difficulty.",
			legal = true
		});
		if (activeQuestCheck != null)
		{
			list.Add(new ButtonInfo
			{
				buttonText = "ActiveQuest",
				overlapText = activeDifficulty + " " + activeQuestName,
				isTogglable = false,
				toolTip = activeQuestDare,
				legal = true
			});
			list.Add(new ButtonInfo
			{
				buttonText = "Quest Details",
				method = ShowQuestDetails,
				isTogglable = false,
				toolTip = "View quest rules with a Gorilla Tag photo.",
				legal = true
			});
			list.Add(new ButtonInfo
			{
				buttonText = "Get Hint",
				method = GiveHint,
				isTogglable = false,
				toolTip = "Get a hint about the current quest.",
				legal = true
			});
		}
		else
		{
			float num = Mathf.Max(0, Mathf.CeilToInt(nextQuestTime - Time.time));
			list.Add(new ButtonInfo
			{
				buttonText = "Next Quest...",
				isTogglable = false,
				toolTip = $"New quest in {num}s",
				legal = true
			});
			if (lastCompletedName != null)
			{
				string text2 = (lastWasLevelUp ? $" <color=green>(LEVEL UP to {lastCompletedLevel + 1}!)</color>" : $" ({2 - questsUntilNextLevel}/2 to lvl {playerLevel + 1})");
				list.Add(new ButtonInfo
				{
					buttonText = "Last: " + lastCompletedDifficulty + " " + lastCompletedName + text2,
					overlapText = "<color=green>QUEST COMPLETE</color> " + lastCompletedDifficulty + " <color=yellow>" + lastCompletedName + "</color>" + text2,
					isTogglable = false,
					toolTip = "The quest you just completed!",
					legal = true
				});
			}
		}
		list.Add(new ButtonInfo
		{
			buttonText = $"Completed: {completedCount}",
			overlapText = $"Completed <color=grey>[</color><color=green>{completedCount}</color><color=grey>]</color>",
			isTogglable = false,
			toolTip = "Total quests completed.",
			legal = true
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Reset Quests",
			method = ResetAllQuests,
			isTogglable = false,
			toolTip = "Resets level, progress, and starts a new quest in 60 seconds.",
			legal = true
		});
		Buttons.buttons[category] = list.ToArray();
	}
}
