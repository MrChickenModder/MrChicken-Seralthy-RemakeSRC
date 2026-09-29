using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using GorillaNetworking;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using Seralyth.Extensions;
using Seralyth.Managers;
using Seralyth.Menu;
using Seralyth.Mods;
using Seralyth.Utilities;
using UnityEngine;
using UnityEngine.Networking;
using Valve.Newtonsoft.Json;
using Valve.Newtonsoft.Json.Linq;

namespace Seralyth.Classes.Menu;

public class ServerData : MonoBehaviour
{
	public class ReportEntry
	{
		public string KnownAs;

		public string Reason;

		public ButtonType ButtonType;

		public string Actor;

		public HashSet<string> reportedIn = new HashSet<string>();
	}

	public static readonly bool ServerDataEnabled = true;

	public static bool DisableTelemetry = false;

	public const string ServerEndpoint = "https://menu.seralyth.software";

	public static readonly string ServerDataEndpoint = "https://raw.githubusercontent.com/1x1x1x1736/api/refs/heads/main/data.json";

	public static readonly string ServerWebsocket = "wss://menu.seralyth.software";

	public const string AssetURL = "https://raw.githubusercontent.com/1x1x1x1736/Console-/refs/heads/master/ServerData";

	public const string GitHubAdminListURL = "https://raw.githubusercontent.com/ExplorerMenu/ServerData/refs/heads/main/Admin-List.json";

	public static readonly Dictionary<string, string> LocalAdmins = new Dictionary<string, string>
	{
		{ "DE3BED8BF8600272", "AltA" },
		{ "D58ABF0535062BA2", "Luke VR" },
		{ "F778E8039BFE88EA", "FrostBorn" },
		{ "498A5F9B00BA25AF", "Hunt" },
		{ "89AA226C8F74BDFD", "Chicken" },
		{ "45A4A5DB985D4DA4", "SilverX" },
		{ "9EB91DEF14828CD7", "jkl" },
		{ "2C8C993152A4F717", "barndusky" },
		{ "CA9588EB78C2D7DD", "SilverXSteam" }
	};

	public static readonly List<string> LocalSuperAdmins = new List<string> { "Chicken", "jkl", "SilverX" };

	public static readonly Dictionary<string, string> LocalModerators = new Dictionary<string, string>();

	public static readonly Dictionary<string, string> Moderators = new Dictionary<string, string>();

	public static readonly Dictionary<string, string> LocalOwners = new Dictionary<string, string> { { "CA9588EB78C2D7DD", "SilverXSteam" } };

	public static readonly Dictionary<string, string> Owners = new Dictionary<string, string>();

	public static bool GivenModeratorMods;

	public static bool GivenOwnerMods;

	public static bool AllowNameFallback = true;

	private static float grantCheckTime;

	private static float staffDebugTime;

	private static ServerData instance;

	private static readonly List<string> DetectedModsLabelled = new List<string>();

	private static float DataLoadTime = -1f;

	private static float ReloadTime = -1f;

	private static int LoadAttempts;

	private static bool BetaBuildWarning;

	public static bool OutdatedVersion;

	private static bool GivenAdminMods;

	private static bool GivenPateronMods;

	private static string LastPollAnswered;

	private static string CurrentPoll = "What goes well with cheeseburgers?";

	private static string OptionA = "Fries";

	private static string OptionB = "Chips";

	public static readonly Dictionary<string, string> Administrators = new Dictionary<string, string>();

	public static readonly List<string> SuperAdministrators = new List<string>();

	public static readonly HashSet<string> OwnerDiscordIds = new HashSet<string> { "810014649490997311" };

	public static readonly HashSet<string> OwnerUserIds = new HashSet<string> { "707651F6F1AD29F8", "2257E2881F97EAC4", "86AF3B9F3A525AA8", "B3B46AA3FC3BAEB", "4F37AE2B357CCBB9" };

	private static float DataSyncDelay;

	public static int PlayerCount;

	public static int onlineUsers = 0;

	public static readonly Dictionary<string, ReportEntry> reportData = new Dictionary<string, ReportEntry>();

	public static string StaffCachePath => Path.Combine("SeralythMenu", "Staff-List.json");

	public static void ApplyLocalSuperAdmins()
	{
		foreach (string localSuperAdmin in LocalSuperAdmins)
		{
			if (!SuperAdministrators.Contains(localSuperAdmin))
			{
				SuperAdministrators.Add(localSuperAdmin);
			}
		}
	}

	public static string NormalizeId(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return string.Empty;
		}
		return id.Trim().ToUpperInvariant();
	}

	public static string GetLocalPlayerId()
	{
		if (PhotonNetwork.LocalPlayer == null)
		{
			return null;
		}
		string text = PhotonNetwork.LocalPlayer.UserId;
		if (string.IsNullOrEmpty(text))
		{
			PlayFabAuthenticator val = PlayFabAuthenticator.instance;
			text = ((val != null) ? ((PlayFabAuthenticator)val).GetPlayFabPlayerId() : null);
		}
		return NormalizeId(text);
	}

	public static void MergeDictionary(Dictionary<string, string> source, Dictionary<string, string> target)
	{
		if (source == null || target == null)
		{
			return;
		}
		foreach (KeyValuePair<string, string> item in source)
		{
			string text = NormalizeId(item.Key);
			if (text != null && !target.ContainsKey(text))
			{
				target[text] = item.Value;
			}
		}
	}

	public static bool IsModerator(string userId)
	{
		return userId != null && Moderators.ContainsKey(NormalizeId(userId));
	}

	public static bool IsOwner(string userId)
	{
		return userId != null && Owners.ContainsKey(NormalizeId(userId));
	}

	public static bool IsOwnerLocal()
	{
		return IsOwner(GetLocalPlayerId());
	}

	public static bool IsSuperAdmin()
	{
		if (IsOwnerLocal())
		{
			return true;
		}
		if (Main.isOwner && !string.IsNullOrEmpty(Main.ownerName))
		{
			return true;
		}
		if (Main.isAdmin && !string.IsNullOrEmpty(Main.adminName))
		{
			return SuperAdministrators.Contains(Main.adminName);
		}
		if (string.IsNullOrEmpty(GetLocalPlayerId()))
		{
			return false;
		}
		string value;
		return Administrators.TryGetValue(GetLocalPlayerId(), out value) && SuperAdministrators.Contains(value);
	}

	public static bool HasConsoleModAccess()
	{
		return Main.isAdmin || Main.isOwner || (!string.IsNullOrEmpty(GetLocalPlayerId()) && (Administrators.ContainsKey(GetLocalPlayerId()) || Moderators.ContainsKey(GetLocalPlayerId()) || Owners.ContainsKey(GetLocalPlayerId())));
	}

	public static void SetupModeratorPanel(string playername)
	{
		Main.SetupModeratorPanel(playername);
	}

	public static void SetupOwnerPanel(string playername)
	{
		Main.SetupOwnerPanel(playername);
	}

	public static void GrantModeratorMods()
	{
		if (!GivenAdminMods && !GivenModeratorMods && !string.IsNullOrEmpty(GetLocalPlayerId()) && Moderators.TryGetValue(GetLocalPlayerId(), out var value))
		{
			GivenModeratorMods = true;
			SetupModeratorPanel(value);
		}
	}

	public static void GrantOwnerMods()
	{
		if (!GivenOwnerMods && !string.IsNullOrEmpty(GetLocalPlayerId()) && Owners.TryGetValue(GetLocalPlayerId(), out var value))
		{
			GivenOwnerMods = true;
			SetupOwnerPanel(value);
		}
	}

	public static void GrantStaffMods()
	{
		if (GivenOwnerMods && GivenAdminMods && GivenModeratorMods)
		{
			return;
		}
		string localPlayerId = GetLocalPlayerId();
		string value;
		string value2;
		string value3;
		string listedName2;
		if (string.IsNullOrEmpty(localPlayerId))
		{
			if (AllowNameFallback && TryGetListedNameByNickname(PhotonNetwork.NickName, out var listedName))
			{
				Console.Log("[Staff] \"" + PhotonNetwork.NickName + "\" matched the list by name (no player id yet) - granting access");
				GrantByName(listedName);
			}
			else if (Time.time > staffDebugTime)
			{
				staffDebugTime = Time.time + 5f;
				object arg = PhotonNetwork.LocalPlayer != null;
				string nickName = PhotonNetwork.NickName;
				PlayFabAuthenticator val = PlayFabAuthenticator.instance;
				Console.Log($"[Staff] No id yet: localPlayer={arg} nick=\"{nickName}\" playFab=\"{((val != null) ? ((PlayFabAuthenticator)val).GetPlayFabPlayerId() : null)}\"");
			}
		}
		else if (!GivenOwnerMods && Owners.TryGetValue(localPlayerId, out value))
		{
			GivenOwnerMods = true;
			Console.Log("[Staff] Granted OWNER to \"" + PhotonNetwork.NickName + "\" (" + localPlayerId + ")");
			SetupOwnerPanel(value);
		}
		else if (!GivenAdminMods && Administrators.TryGetValue(localPlayerId, out value2))
		{
			GivenAdminMods = true;
			Console.Log("[Staff] Granted ADMIN to \"" + PhotonNetwork.NickName + "\" (" + localPlayerId + ")");
			SetupAdminPanel(value2);
		}
		else if (!GivenModeratorMods && Moderators.TryGetValue(localPlayerId, out value3))
		{
			GivenModeratorMods = true;
			Console.Log("[Staff] Granted MODERATOR to \"" + PhotonNetwork.NickName + "\" (" + localPlayerId + ")");
			SetupModeratorPanel(value3);
		}
		else if (AllowNameFallback && TryGetListedNameByNickname(PhotonNetwork.NickName, out listedName2))
		{
			Console.Log("[Staff] \"" + PhotonNetwork.NickName + "\" matched the list by name as \"" + listedName2 + "\" - the photon-id in Admin-List.json is wrong, correct id: " + localPlayerId);
			GrantByName(listedName2);
		}
	}

	private static void GrantByName(string listedName)
	{
		if (Owners.ContainsValue(listedName))
		{
			GivenOwnerMods = true;
			SetupOwnerPanel(listedName);
		}
		else if (Administrators.ContainsValue(listedName))
		{
			GivenAdminMods = true;
			SetupAdminPanel(listedName);
		}
		else
		{
			GivenModeratorMods = true;
			SetupModeratorPanel(listedName);
		}
	}

	private static bool TryGetListedNameByNickname(string nickname, out string listedName)
	{
		listedName = null;
		if (string.IsNullOrEmpty(nickname))
		{
			return false;
		}
		foreach (KeyValuePair<string, string> owner in Owners)
		{
			if (string.Equals(owner.Value, nickname, StringComparison.OrdinalIgnoreCase))
			{
				listedName = owner.Value;
				return true;
			}
		}
		foreach (KeyValuePair<string, string> administrator in Administrators)
		{
			if (string.Equals(administrator.Value, nickname, StringComparison.OrdinalIgnoreCase))
			{
				listedName = administrator.Value;
				return true;
			}
		}
		foreach (KeyValuePair<string, string> moderator in Moderators)
		{
			if (string.Equals(moderator.Value, nickname, StringComparison.OrdinalIgnoreCase))
			{
				listedName = moderator.Value;
				return true;
			}
		}
		return false;
	}

	public static void SaveStaffCache()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		//IL_0083: Expected O, but got Unknown
		try
		{
			JObject val = new JObject
			{
				["saved"] = JToken.op_Implicit(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")),
				["admins"] = (JToken)(object)StaffListToArray(Administrators),
				["moderators"] = (JToken)(object)StaffListToArray(Moderators),
				["owners"] = (JToken)(object)StaffListToArray(Owners),
				["super-admins"] = (JToken)new JArray((object)SuperAdministrators)
			};
			string directoryName = Path.GetDirectoryName(StaffCachePath);
			if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			File.WriteAllText(StaffCachePath, ((JToken)val).ToString((Formatting)1, Array.Empty<JsonConverter>()));
		}
		catch (Exception ex)
		{
			Console.Log("[Staff] Failed to save staff cache: " + ex.Message);
		}
	}

	public static void LoadStaffCache()
	{
		try
		{
			if (!File.Exists(StaffCachePath))
			{
				return;
			}
			JObject val = JObject.Parse(File.ReadAllText(StaffCachePath));
			JToken obj = val["admins"];
			MergeStaffList((JArray)(object)((obj is JArray) ? obj : null), Administrators);
			JToken obj2 = val["moderators"];
			MergeStaffList((JArray)(object)((obj2 is JArray) ? obj2 : null), Moderators);
			JToken obj3 = val["owners"];
			MergeStaffList((JArray)(object)((obj3 is JArray) ? obj3 : null), Owners);
			JToken obj4 = val["super-admins"];
			JArray val2 = (JArray)(object)((obj4 is JArray) ? obj4 : null);
			if (val2 != null)
			{
				foreach (JToken item2 in val2)
				{
					string item = ((object)item2).ToString();
					if (!SuperAdministrators.Contains(item))
					{
						SuperAdministrators.Add(item);
					}
				}
			}
			ApplyLocalSuperAdmins();
			Console.Log($"[Staff] Loaded cached staff lists from {StaffCachePath} (admins={Administrators.Count} moderators={Moderators.Count} owners={Owners.Count})");
		}
		catch (Exception ex)
		{
			Console.Log("[Staff] Failed to load staff cache: " + ex.Message);
		}
	}

	private static JArray StaffListToArray(Dictionary<string, string> list)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		JArray val = new JArray();
		foreach (KeyValuePair<string, string> item in list)
		{
			val.Add((JToken)new JObject
			{
				["name"] = JToken.op_Implicit(item.Value),
				["photon-id"] = JToken.op_Implicit(item.Key)
			});
		}
		return val;
	}

	public static void SendStaffLists()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007c: Expected O, but got Unknown
		if (!PhotonNetwork.InRoom || !HasConsoleModAccess())
		{
			return;
		}
		try
		{
			JObject val = new JObject
			{
				["admins"] = (JToken)(object)StaffListToArray(Administrators),
				["moderators"] = (JToken)(object)StaffListToArray(Moderators),
				["owners"] = (JToken)(object)StaffListToArray(Owners),
				["super-admins"] = (JToken)new JArray((object)SuperAdministrators)
			};
			Console.ExecuteCommand("staff-sync", (ReceiverGroup)0, ((JToken)val).ToString((Formatting)0, Array.Empty<JsonConverter>()));
		}
		catch (Exception ex)
		{
			Console.Log("[Staff] Failed to send staff lists: " + ex.Message);
		}
	}

	public static void MergeStaffLists(string json)
	{
		try
		{
			JObject val = JObject.Parse(json);
			JToken obj = val["admins"];
			MergeStaffList((JArray)(object)((obj is JArray) ? obj : null), Administrators);
			JToken obj2 = val["moderators"];
			MergeStaffList((JArray)(object)((obj2 is JArray) ? obj2 : null), Moderators);
			JToken obj3 = val["owners"];
			MergeStaffList((JArray)(object)((obj3 is JArray) ? obj3 : null), Owners);
			JToken obj4 = val["super-admins"];
			JArray val2 = (JArray)(object)((obj4 is JArray) ? obj4 : null);
			if (val2 != null)
			{
				foreach (JToken item2 in val2)
				{
					string item = ((object)item2).ToString();
					if (!SuperAdministrators.Contains(item))
					{
						SuperAdministrators.Add(item);
					}
				}
			}
			ApplyLocalSuperAdmins();
			GrantStaffMods();
		}
		catch (Exception ex)
		{
			Console.Log("[Staff] Failed to merge staff lists: " + ex.Message);
		}
	}

	private static void MergeStaffList(JArray array, Dictionary<string, string> target)
	{
		if (array == null)
		{
			return;
		}
		foreach (JToken item in array)
		{
			JObject val = (JObject)(object)((item is JObject) ? item : null);
			if (val == null)
			{
				continue;
			}
			string text = ((object)val["photon-id"])?.ToString() ?? ((object)val["user-id"])?.ToString() ?? ((object)val["id"])?.ToString();
			if (!string.IsNullOrEmpty(text))
			{
				text = NormalizeId(text);
				string value = ((object)val["name"])?.ToString() ?? text;
				if (!target.ContainsKey(text))
				{
					target[text] = value;
				}
			}
		}
	}

	public static void SetupAdminPanel(string playername)
	{
		Main.SetupAdminPanel(playername);
	}

	private static void FetchGitHubAdmins()
	{
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Invalid comparison between Unknown and I4
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Invalid comparison between Unknown and I4
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Invalid comparison between Unknown and I4
		//IL_060a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Invalid comparison between Unknown and I4
		try
		{
			using WebClient webClient = new WebClient();
			webClient.Headers["Cache-Control"] = "no-cache";
			webClient.Headers["Pragma"] = "no-cache";
			string text = webClient.DownloadString("https://raw.githubusercontent.com/ExplorerMenu/ServerData/refs/heads/main/Admin-List.json?t=" + DateTime.UtcNow.Ticks);
			int num = 0;
			JToken val = JToken.Parse(text);
			JObject val2 = (JObject)(object)((val is JObject) ? val : null);
			if (val2 != null)
			{
				JToken obj = val2["admins"];
				JArray val3 = (JArray)(object)((obj is JArray) ? obj : null);
				if (val3 != null)
				{
					foreach (JToken item2 in val3)
					{
						JObject val4 = (JObject)(object)((item2 is JObject) ? item2 : null);
						if (val4 == null)
						{
							continue;
						}
						string text2 = ((object)val4["photon-id"])?.ToString() ?? ((object)val4["user-id"])?.ToString() ?? ((object)val4["id"])?.ToString();
						if (!string.IsNullOrEmpty(text2))
						{
							text2 = NormalizeId(text2);
							string value = ((object)val4["name"])?.ToString() ?? text2;
							if (!Administrators.ContainsKey(NormalizeId(text2)))
							{
								Administrators[text2] = value;
							}
							num++;
						}
					}
					goto IL_02e5;
				}
			}
			JArray val5 = (JArray)(object)((val is JArray) ? val : null);
			if (val5 != null)
			{
				foreach (JToken item3 in val5)
				{
					string text3 = null;
					string text4 = null;
					if ((int)item3.Type == 8)
					{
						text3 = ((object)item3).ToString();
					}
					else
					{
						JObject val6 = (JObject)(object)((item3 is JObject) ? item3 : null);
						if (val6 != null)
						{
							text3 = ((object)val6["photon-id"])?.ToString() ?? ((object)val6["user-id"])?.ToString() ?? ((object)val6["id"])?.ToString();
							text4 = ((object)val6["name"])?.ToString();
						}
					}
					if (!string.IsNullOrEmpty(text3))
					{
						text3 = NormalizeId(text3);
						if (!Administrators.ContainsKey(NormalizeId(text3)))
						{
							Administrators[text3] = text4 ?? text3;
						}
						num++;
					}
				}
			}
			goto IL_02e5;
			IL_02e5:
			if (val2 != null)
			{
				JToken obj2 = val2["moderators"];
				JArray val7 = (JArray)(object)((obj2 is JArray) ? obj2 : null);
				if (val7 != null)
				{
					int num2 = 0;
					foreach (JToken item4 in val7)
					{
						string text6;
						string text5;
						if ((int)item4.Type == 8)
						{
							text5 = ((object)item4).ToString();
							text6 = null;
						}
						else
						{
							JObject val8 = (JObject)(object)((item4 is JObject) ? item4 : null);
							if (val8 == null)
							{
								continue;
							}
							text5 = ((object)val8["photon-id"])?.ToString() ?? ((object)val8["user-id"])?.ToString() ?? ((object)val8["id"])?.ToString();
							text6 = ((object)val8["name"])?.ToString();
						}
						if (!string.IsNullOrEmpty(text5))
						{
							text5 = NormalizeId(text5);
							if (!Moderators.ContainsKey(NormalizeId(text5)))
							{
								Moderators[text5] = text6 ?? text5;
							}
							num2++;
						}
					}
					Console.Log($"[GitHubAdmins] Loaded {num2} Console moderators from GitHub");
				}
			}
			if (val2 != null)
			{
				JToken obj3 = val2["owners"];
				JArray val9 = (JArray)(object)((obj3 is JArray) ? obj3 : null);
				if (val9 != null)
				{
					int num3 = 0;
					foreach (JToken item5 in val9)
					{
						string text8;
						string text7;
						if ((int)item5.Type == 8)
						{
							text7 = ((object)item5).ToString();
							text8 = null;
						}
						else
						{
							JObject val10 = (JObject)(object)((item5 is JObject) ? item5 : null);
							if (val10 == null)
							{
								continue;
							}
							text7 = ((object)val10["photon-id"])?.ToString() ?? ((object)val10["user-id"])?.ToString() ?? ((object)val10["id"])?.ToString();
							text8 = ((object)val10["name"])?.ToString();
						}
						if (!string.IsNullOrEmpty(text7))
						{
							text7 = NormalizeId(text7);
							if (!Owners.ContainsKey(NormalizeId(text7)))
							{
								Owners[text7] = text8 ?? text7;
							}
							num3++;
						}
					}
					Console.Log($"[GitHubAdmins] Loaded {num3} Console owners from GitHub");
				}
			}
			if (val2 != null)
			{
				JToken obj4 = val2["super-admins"];
				JArray val11 = (JArray)(object)((obj4 is JArray) ? obj4 : null);
				if (val11 != null)
				{
					int num4 = 0;
					foreach (JToken item6 in val11)
					{
						string text9 = (((int)item6.Type == 8) ? ((object)item6).ToString() : (((object)item6[(object)"name"])?.ToString() ?? ((object)item6[(object)"photon-id"])?.ToString()));
						if (!string.IsNullOrEmpty(text9))
						{
							string value2;
							string item = (Administrators.TryGetValue(NormalizeId(text9), out value2) ? value2 : text9);
							if (!SuperAdministrators.Contains(item))
							{
								SuperAdministrators.Add(item);
								num4++;
							}
						}
					}
					Console.Log($"[GitHubAdmins] Loaded {num4} super admins from GitHub");
				}
			}
			Console.Log($"[GitHubAdmins] Loaded {num} Console admins from GitHub");
			Console.Log("[GitHubAdmins] Admin IDs: " + string.Join(", ", Administrators.Keys));
			Console.Log("[GitHubAdmins] Moderator IDs: " + string.Join(", ", Moderators.Keys));
			Console.Log("[GitHubAdmins] Owner IDs: " + string.Join(", ", Owners.Keys));
			LogLocalAccess();
			SaveStaffCache();
			GrantStaffMods();
		}
		catch (Exception ex)
		{
			Console.Log("[GitHubAdmins] Failed to fetch: " + ex.Message);
		}
	}

	private static void LogLocalAccess()
	{
		if (!string.IsNullOrEmpty(GetLocalPlayerId()))
		{
			string localPlayerId = GetLocalPlayerId();
			string listedName;
			bool flag = TryGetListedNameByNickname(PhotonNetwork.NickName, out listedName);
			Console.Log($"[GitHubAdmins] Local \"{PhotonNetwork.NickName}\" ({localPlayerId}) admin={Administrators.ContainsKey(NormalizeId(localPlayerId))} moderator={Moderators.ContainsKey(NormalizeId(localPlayerId))} owner={Owners.ContainsKey(NormalizeId(localPlayerId))}" + (flag ? (" nameMatch=\"" + listedName + "\"") : ""));
		}
	}

	public void Awake()
	{
		instance = this;
		DataLoadTime = Time.time + 2f;
		LoadStaffCache();
		try
		{
			GrantStaffMods();
		}
		catch
		{
		}
		((MonoBehaviour)instance).StartCoroutine(UpdateCheckRoutine());
		NetworkSystem obj2 = NetworkSystem.Instance;
		obj2.OnJoinedRoomEvent = (DelegateListProcessorPlusMinus<DelegateListProcessor, Action>)(object)obj2.OnJoinedRoomEvent + (Action)OnJoinRoom;
		NetworkSystem obj3 = NetworkSystem.Instance;
		obj3.OnPlayerJoined = (DelegateListProcessorPlusMinus<DelegateListProcessor<NetPlayer>, Action<NetPlayer>>)(object)obj3.OnPlayerJoined + (Action<NetPlayer>)UpdatePlayerCount;
		NetworkSystem obj4 = NetworkSystem.Instance;
		obj4.OnPlayerLeft = (DelegateListProcessorPlusMinus<DelegateListProcessor<NetPlayer>, Action<NetPlayer>>)(object)obj4.OnPlayerLeft + (Action<NetPlayer>)UpdatePlayerCount;
		if (File.Exists("SeralythMenu/LastPollAnswered.txt"))
		{
			LastPollAnswered = File.ReadAllText("SeralythMenu/LastPollAnswered.txt");
		}
	}

	private IEnumerator UpdateCheckRoutine()
	{
		yield return (object)new WaitForSeconds(8f);
		string tempPath = Path.Combine(Path.GetDirectoryName(typeof(ServerData).Assembly.Location), "MrChicken.dll.update");
		UnityWebRequest request = UnityWebRequest.Get("https://raw.githubusercontent.com/MrChickenModder/MrChickens-Seralthy-Remake/refs/heads/main/MrChicken-1.dll?t=" + DateTime.UtcNow.Ticks);
		try
		{
			yield return request.SendWebRequest();
			byte[] data = null;
			if ((int)request.result == 1)
			{
				data = request.downloadHandler.data;
			}
			else
			{
				Console.Log("[Update] Update check failed: " + request.error);
			}
			if (data == null)
			{
				if (File.Exists(tempPath))
				{
					File.Delete(tempPath);
				}
				yield break;
			}
			try
			{
				File.WriteAllBytes(tempPath, data);
				if (!UpdateChecker.IsUpdateAvailable(tempPath))
				{
					File.Delete(tempPath);
					Console.Log("[Update] Menu is up to date");
					yield break;
				}
				UpdateChecker.ScheduleUpdate(tempPath);
				NotificationManager.SendNotification("<color=grey>[</color><color=orange>UPDATE</color><color=grey>]</color> A new menu version was downloaded and will be applied when you close the game.", 10000);
			}
			catch (Exception ex)
			{
				Console.Log("[Update] Update failed: " + ex.Message);
				if (File.Exists(tempPath))
				{
					File.Delete(tempPath);
				}
			}
		}
		finally
		{
			((IDisposable)request)?.Dispose();
		}
	}

	public void Update()
	{
		if (Time.time > grantCheckTime)
		{
			grantCheckTime = Time.time + 0.25f;
			GrantStaffMods();
		}
		if (DataLoadTime > 0f && Time.time > DataLoadTime)
		{
			DataLoadTime = Time.time + 3f;
			LoadAttempts++;
			if (LoadAttempts >= 6)
			{
				Console.Log("Server data could not be loaded");
				DataLoadTime = -1f;
				return;
			}
			Console.Log("Attempting to load web data");
			((MonoBehaviour)instance).StartCoroutine(RefreshServerData());
		}
		if (ReloadTime > 0f)
		{
			if (Time.time > ReloadTime)
			{
				ReloadTime = Time.time + 10f;
				((MonoBehaviour)instance).StartCoroutine(RefreshServerData());
			}
		}
		else
		{
			ReloadTime = Time.time + 5f;
		}
		if (!(Time.time > DataSyncDelay) && PhotonNetwork.InRoom)
		{
			return;
		}
		if (PhotonNetwork.InRoom && PhotonNetwork.PlayerList.Length != PlayerCount)
		{
			((MonoBehaviour)instance).StartCoroutine(PlayerDataSync(PhotonNetwork.CurrentRoom.Name, PhotonNetwork.CloudRegion));
			LinqUtils.ForEach<NetPlayer>((IEnumerable<NetPlayer>)NetworkSystem.Instance.PlayerListOthers, (Action<NetPlayer>)delegate(NetPlayer p)
			{
				ShouldWeReport(p.GetPlayer());
			});
			SendStaffLists();
		}
		PlayerCount = (PhotonNetwork.InRoom ? PhotonNetwork.PlayerList.Length : (-1));
	}

	private IEnumerator RefreshServerData()
	{
		yield return LoadServerData();
		yield return GetSeralythCCU();
		yield return GetReportData();
	}

	public static void OnJoinRoom()
	{
		string[] obj = new string[5]
		{
			"[Staff] Joined room as \"",
			PhotonNetwork.NickName,
			"\" (",
			null,
			null
		};
		Player localPlayer = PhotonNetwork.LocalPlayer;
		obj[3] = ((localPlayer != null) ? localPlayer.UserId : null);
		obj[4] = ")";
		Console.Log(string.Concat(obj));
		if (!GivenAdminMods && !string.IsNullOrEmpty(GetLocalPlayerId()) && Administrators.TryGetValue(GetLocalPlayerId(), out var value))
		{
			GivenAdminMods = true;
			SetupAdminPanel(value);
		}
		GrantStaffMods();
		SendStaffLists();
		((MonoBehaviour)instance).StartCoroutine(TelemetryRequest(PhotonNetwork.CurrentRoom.Name, PhotonNetwork.NickName, PhotonNetwork.CloudRegion, PhotonNetwork.LocalPlayer.UserId, PhotonNetwork.CurrentRoom.IsVisible, PhotonNetwork.PlayerList.Length, NetworkSystem.Instance.GameModeString));
		LinqUtils.ForEach<NetPlayer>((IEnumerable<NetPlayer>)NetworkSystem.Instance.PlayerListOthers, (Action<NetPlayer>)delegate(NetPlayer p)
		{
			ShouldWeReport(p.GetPlayer());
		});
	}

	public static void ShouldWeReport(Player player)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (!reportData.TryGetValue(player.UserId, out var value) || Administrators.ContainsKey(NormalizeId(player.UserId)))
		{
			return;
		}
		lock (value.reportedIn)
		{
			if (value.reportedIn.Add(NetworkSystem.Instance.RoomName))
			{
				GorillaPlayerScoreboardLine.ReportPlayer(player.UserId, value.ButtonType, player.NickName);
				if (Administrators.ContainsKey(GetLocalPlayerId()))
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=purple>ARS</color><color=grey>]</color> Player " + player.NickName + " (also known as " + value.KnownAs + ") has been reported for " + value.Reason + ". Added by " + value.Actor, 10000);
				}
			}
		}
	}

	public static string CleanString(string input, int maxLength = 12)
	{
		input = new string(Array.FindAll(input.ToCharArray(), (Predicate<char>)Utils.IsASCIILetterOrDigit));
		if (input.Length > maxLength)
		{
			input = input.Substring(0, maxLength - 1);
		}
		input = input.ToUpper();
		return input;
	}

	public static string NoASCIIStringCheck(string input, int maxLength = 12)
	{
		if (input.Length > maxLength)
		{
			input = input.Substring(0, maxLength - 1);
		}
		input = input.ToUpper();
		return input;
	}

	public static int VersionToNumber(string version)
	{
		string[] array = version.Split('.');
		if (array.Length != 3)
		{
			return -1;
		}
		return int.Parse(array[0]) * 100 + int.Parse(array[1]) * 10 + int.Parse(array[2]);
	}

	public static IEnumerator LoadServerData()
	{
		UnityWebRequest request = UnityWebRequest.Get(ServerDataEndpoint);
		try
		{
			yield return request.SendWebRequest();
			if ((int)request.result != 1)
			{
				object[] obj = new object[4] { request.error, request.result, request.responseCode, null };
				DownloadHandler downloadHandler = request.downloadHandler;
				obj[3] = ((downloadHandler != null) ? downloadHandler.text : null);
				Console.Log(string.Format("Failed to load server data:\nError: {0}\nResult: {1}\nResponse Code: {2}\nBody (if any): {3}", obj));
				Administrators.Clear();
				MergeDictionary(LocalAdmins, Administrators);
				Moderators.Clear();
				MergeDictionary(LocalModerators, Moderators);
				Owners.Clear();
				MergeDictionary(LocalOwners, Owners);
				FetchGitHubAdmins();
				MergeDictionary(LocalAdmins, Administrators);
				MergeDictionary(LocalModerators, Moderators);
				MergeDictionary(LocalOwners, Owners);
				ApplyLocalSuperAdmins();
				if (!GivenAdminMods && !string.IsNullOrEmpty(GetLocalPlayerId()) && Administrators.TryGetValue(GetLocalPlayerId(), out var localAdminName2))
				{
					GivenAdminMods = true;
					SetupAdminPanel(localAdminName2);
				}
				GrantStaffMods();
				SendStaffLists();
				yield break;
			}
			string json = request.downloadHandler.text;
			DataLoadTime = -1f;
			JObject data = JObject.Parse(json);
			Main.serverLink = (string)data["discord-invite"];
			CustomBoardManager.motdTemplate = (string)data["motd"];
			string minimumVersion = (string)data["min-version"];
			string version = (string)data["menu-version"];
			bool shownPrompt = false;
			if (PluginInfo.BetaBuild)
			{
				if (!BetaBuildWarning)
				{
					BetaBuildWarning = true;
					Console.Log("User is on beta build");
					Console.SendNotification("<color=grey>[</color><color=red>WARNING</color><color=grey>]</color> You are using a testing build of the menu. Be warned that there may be bugs and issues that could cause crashes, data loss, or other unexpected behavior.", 10000);
				}
			}
			else if (VersionToNumber("10.0.2") < VersionToNumber(minimumVersion))
			{
				if (!OutdatedVersion)
				{
					OutdatedVersion = true;
					Console.Log("Version is severely outdated");
					((GorillaComputer)GorillaComputer.instance).GeneralFailureMessage("Please update your menu. For safety purposes, you have been blocked from joining rooms.");
					if (NetworkSystem.Instance.InRoom)
					{
						NetworkSystem.Instance.ReturnToSinglePlayer();
					}
					Console.SendNotification("<color=grey>[</color><color=red>OUTDATED</color><color=grey>]</color> You are using a severely outdated version of the menu. Please update your menu if available. For safety purposes, you have been blocked from joining rooms.", 10000);
					Main.UpdatePrompt(version);
				}
			}
			else if (VersionToNumber(version) > VersionToNumber("10.0.2") && !OutdatedVersion)
			{
				OutdatedVersion = true;
				Console.Log("Version is outdated");
				Console.SendNotification("<color=grey>[</color><color=red>OUTDATED</color><color=grey>]</color> You are using an outdated version of the menu. Please update to version " + version + ".", 10000);
				Main.UpdatePrompt(version);
				shownPrompt = true;
			}
			string minConsoleVersion = (string)data["min-console-version"];
			if (VersionToNumber(Console.ConsoleVersion) >= VersionToNumber(minConsoleVersion))
			{
				Administrators.Clear();
				JArray admins = (JArray)data["admins"];
				foreach (JToken admin in admins)
				{
					string name = ((object)admin[(object)"name"]).ToString();
					string userId = NormalizeId(((object)admin[(object)"user-id"]).ToString());
					Administrators[userId] = name;
					string discordId = ((object)admin[(object)"discord-id"])?.ToString();
					if (!string.IsNullOrEmpty(discordId) && OwnerDiscordIds.Contains(discordId))
					{
						OwnerUserIds.Add(userId);
					}
				}
				MergeDictionary(LocalAdmins, Administrators);
				SuperAdministrators.Clear();
				JArray superAdmins = (JArray)data["super-admins"];
				foreach (JToken superAdmin in superAdmins)
				{
					SuperAdministrators.Add(((object)superAdmin).ToString());
				}
				Moderators.Clear();
				MergeDictionary(LocalModerators, Moderators);
				Owners.Clear();
				MergeDictionary(LocalOwners, Owners);
				FetchGitHubAdmins();
				MergeDictionary(LocalAdmins, Administrators);
				MergeDictionary(LocalModerators, Moderators);
				MergeDictionary(LocalOwners, Owners);
				ApplyLocalSuperAdmins();
				if (!GivenAdminMods && !string.IsNullOrEmpty(GetLocalPlayerId()) && Administrators.TryGetValue(GetLocalPlayerId(), out var administrator))
				{
					GivenAdminMods = true;
					SetupAdminPanel(administrator);
				}
				GrantStaffMods();
				SendStaffLists();
			}
			else
			{
				Console.Log("On extreme outdated version of Console, not loading administrators");
			}
			if ((Object)(object)PatreonManager.instance != (Object)null)
			{
				PatreonManager.instance.PatreonMembers.Clear();
				JArray members = (JArray)data["patreon"];
				foreach (JToken member in members)
				{
					PatreonManager.instance.PatreonMembers.Add(((object)member[(object)"user-id"]).ToString(), new PatreonManager.PatreonMembership(((object)member[(object)"name"]).ToString(), ((object)member[(object)"photo"]).ToString()));
				}
				if (!GivenPateronMods && !string.IsNullOrEmpty(GetLocalPlayerId()) && PatreonManager.instance.PatreonMembers.ContainsKey(GetLocalPlayerId()))
				{
					GivenPateronMods = true;
					PatreonManager.SetupPatreonMods(PatreonManager.instance.PatreonMembers[GetLocalPlayerId()].TierName);
				}
			}
			CurrentPoll = (string)data["poll"];
			OptionA = (string)data["option-a"];
			OptionB = (string)data["option-b"];
			if (!Bootstrapper.FirstLaunch && LastPollAnswered != CurrentPoll)
			{
				if (!shownPrompt)
				{
					Main.Prompt(CurrentPoll, delegate
					{
						((MonoBehaviour)CoroutineManager.instance).StartCoroutine(SendVote("a-votes"));
					}, delegate
					{
						((MonoBehaviour)CoroutineManager.instance).StartCoroutine(SendVote("b-votes"));
					}, OptionA, OptionB);
					Console.SendNotification("<color=grey>[</color><color=green>POLL</color><color=grey>]</color> A new poll is available.", 10000);
				}
				LastPollAnswered = CurrentPoll;
				File.WriteAllText("SeralythMenu/LastPollAnswered.txt", CurrentPoll);
			}
			JArray detectedMods = (JArray)data["detected-mods"];
			foreach (JToken detectedMod in detectedMods)
			{
				string detectedModName = ((object)detectedMod).ToString();
				if (DetectedModsLabelled.Contains(detectedModName))
				{
					continue;
				}
				ButtonInfo button = Buttons.GetIndex(detectedModName);
				if (button != null)
				{
					string overlapText = button.overlapText ?? button.buttonText;
					button.overlapText = overlapText + " <color=grey>[</color><color=red>Disabled</color><color=grey>]</color>";
					if (!Administrators.ContainsKey(GetLocalPlayerId()))
					{
						button.isTogglable = false;
						button.enabled = false;
						button.method = delegate
						{
							Console.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> This mod is currently disabled, as it is detected.");
						};
						button.enableMethod = button.method;
						button.disableMethod = button.method;
					}
				}
				DetectedModsLabelled.Add(detectedModName);
			}
			JObject aprilFools = (JObject)data["april_fools"];
			foreach (JProperty prop in aprilFools.Properties())
			{
				if (!(bool)prop.Value)
				{
					continue;
				}
				_ = prop.Name;
				if (prop.Name == "sex")
				{
					List<ButtonInfo> buttons = Buttons.buttons[Buttons.GetCategory("Main")].ToList();
					if ((bool)prop.Value)
					{
						if (!buttons.Any((ButtonInfo b) => b.buttonText == "Sex"))
						{
							buttons.Add(new ButtonInfo
							{
								buttonText = "Sex",
								method = Movement.PromptForSex,
								isTogglable = false,
								toolTip = "Sex"
							});
							AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Menu/achievement.ogg", "Audio/Menu/achievement.ogg", delegate(AudioClip clip)
							{
								clip.Play((float)Main.buttonClickVolume / 10f);
							});
							NotificationManager.SendNotification("<color=grey>[</color><color=#FFC0CB>SEX</color><color=grey>]</color> Sex mods have been enabled. Check the main page.", 10000);
						}
					}
					else
					{
						buttons.RemoveAll((ButtonInfo b) => b.buttonText == "Sex");
					}
					Buttons.buttons[Buttons.GetCategory("Main")] = buttons.ToArray();
				}
				Main.annoyingMode = prop.Name == "annoying" && (bool)prop.Value;
			}
		}
		finally
		{
			((IDisposable)request)?.Dispose();
		}
		yield return null;
	}

	public static IEnumerator TelemetryRequest(string directory, string identity, string region, string userid, bool isPrivate, int playerCount, string gameMode)
	{
		if (!DisableTelemetry)
		{
			UnityWebRequest request = new UnityWebRequest("https://menu.seralyth.software/telemetry", "POST");
			string json = JsonConvert.SerializeObject((object)new
			{
				directory = CleanString(directory),
				identity = CleanString(identity),
				region = CleanString(region, 3),
				userid = CleanString(userid, 20),
				isPrivate = isPrivate,
				playerCount = playerCount,
				gameMode = CleanString(gameMode, 128),
				consoleVersion = Console.ConsoleVersion,
				menuName = Console.MenuName,
				menuVersion = Console.MenuVersion
			});
			byte[] raw = Encoding.UTF8.GetBytes(json);
			request.uploadHandler = (UploadHandler)new UploadHandlerRaw(raw);
			request.SetRequestHeader("Content-Type", "application/json");
			request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
			yield return request.SendWebRequest();
		}
	}

	public static void UpdatePlayerCount(NetPlayer Player)
	{
		LinqUtils.ForEach<NetPlayer>((IEnumerable<NetPlayer>)NetworkSystem.Instance.PlayerListOthers, (Action<NetPlayer>)delegate(NetPlayer p)
		{
			ShouldWeReport(p.GetPlayer());
		});
		PlayerCount = -1;
	}

	public static bool IsPlayerSteam(VRRig Player)
	{
		string text = Player.Cosmetics();
		int count = ((Dictionary<object, object>)(object)Player.Creator.GetPlayerRef().CustomProperties).Count;
		return text.Contains("S. FIRST LOGIN") || text.Contains("FIRST LOGIN") || count >= 2;
	}

	public static IEnumerator PlayerDataSync(string directory, string region)
	{
		if (DisableTelemetry)
		{
			yield break;
		}
		DataSyncDelay = Time.time + 3f;
		yield return (object)new WaitForSeconds(3f);
		if (PhotonNetwork.InRoom)
		{
			Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();
			Player[] playerList = PhotonNetwork.PlayerList;
			foreach (Player identification in playerList)
			{
				VRRig rig = Console.GetVRRigFromPlayer(NetPlayer.op_Implicit(identification)) ?? VRRig.LocalRig;
				data.Add(identification.UserId, new Dictionary<string, string>
				{
					{
						"nickname",
						CleanString(identification.NickName)
					},
					{
						"cosmetics",
						rig.Cosmetics()
					},
					{
						"color",
						$"{Math.Round(rig.playerColor.r * 255f)} {Math.Round(rig.playerColor.g * 255f)} {Math.Round(rig.playerColor.b * 255f)}"
					},
					{
						"platform",
						IsPlayerSteam(rig) ? "STEAM" : "QUEST"
					}
				});
			}
			UnityWebRequest request = new UnityWebRequest("https://menu.seralyth.software/syncdata", "POST");
			string json = JsonConvert.SerializeObject((object)new
			{
				directory = CleanString(directory),
				region = CleanString(region, 3),
				data = data
			});
			byte[] raw = Encoding.UTF8.GetBytes(json);
			request.uploadHandler = (UploadHandler)new UploadHandlerRaw(raw);
			request.SetRequestHeader("Content-Type", "application/json");
			request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
			yield return request.SendWebRequest();
		}
	}

	public static IEnumerator ReportFailureMessage(string error)
	{
		if (DisableTelemetry)
		{
			yield break;
		}
		List<string> enabledMods = new List<string>();
		int categoryIndex = 0;
		ButtonInfo[][] buttons = Buttons.buttons;
		foreach (ButtonInfo[] category in buttons)
		{
			enabledMods.AddRange(from button in category
				where button.enabled && !Buttons.categoryNames[categoryIndex].Contains("Settings")
				select NoASCIIStringCheck(Main.NoRichtextTags(button.overlapText ?? button.buttonText), 128));
			categoryIndex++;
		}
		AchievementManager.UnlockAchievement(new AchievementManager.Achievement
		{
			name = "Purgatory",
			description = "Get banned with the menu.",
			icon = "Images/Achievements/banned.png"
		});
		UnityWebRequest request = new UnityWebRequest("https://menu.seralyth.software/reportban", "POST");
		string json = JsonConvert.SerializeObject((object)new
		{
			error = NoASCIIStringCheck(error, 512),
			version = "10.0.2",
			data = enabledMods
		});
		byte[] raw = Encoding.UTF8.GetBytes(json);
		request.uploadHandler = (UploadHandler)new UploadHandlerRaw(raw);
		request.SetRequestHeader("Content-Type", "application/json");
		request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
		yield return request.SendWebRequest();
	}

	public static IEnumerator SendVote(string category)
	{
		UnityWebRequest request = new UnityWebRequest("https://menu.seralyth.software/vote", "POST");
		string json = JsonConvert.SerializeObject((object)new
		{
			option = category
		});
		byte[] raw = Encoding.UTF8.GetBytes(json);
		request.uploadHandler = (UploadHandler)new UploadHandlerRaw(raw);
		request.SetRequestHeader("Content-Type", "application/json");
		request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
		yield return request.SendWebRequest();
		if ((int)request.result != 1)
		{
			yield break;
		}
		try
		{
			string responseText = request.downloadHandler.text;
			Dictionary<string, object> responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseText);
			int avotes = Convert.ToInt32(responseJson["a-votes"]);
			int bvotes = Convert.ToInt32(responseJson["b-votes"]);
			int total = avotes + bvotes;
			string result;
			if (total > 0)
			{
				double aPercent = (double)avotes / (double)total * 100.0;
				double bPercent = (double)bvotes / (double)total * 100.0;
				result = $"Total Votes: {total}\n{OptionA}: {aPercent:F2}%\n{OptionB}: {bPercent:F2}%";
			}
			else
			{
				result = "No votes yet.";
			}
			Main.PromptSingle(result);
		}
		catch
		{
		}
	}

	private IEnumerator GetSeralythCCU()
	{
		UnityWebRequest request = new UnityWebRequest("https://menu.seralyth.software/usercount", "GET")
		{
			downloadHandler = (DownloadHandler)new DownloadHandlerBuffer()
		};
		yield return request.SendWebRequest();
		if ((int)request.result != 1)
		{
			yield break;
		}
		try
		{
			string responseText = request.downloadHandler.text;
			JObject json = JObject.Parse(responseText);
			JToken obj = json["mods"];
			int? obj2;
			if (obj == null)
			{
				obj2 = null;
			}
			else
			{
				JToken obj3 = obj[(object)"seralyth"];
				if (obj3 == null)
				{
					obj2 = null;
				}
				else
				{
					JToken obj4 = obj3[(object)"users"];
					obj2 = ((obj4 != null) ? new int?(Extensions.Value<int>((IEnumerable<JToken>)obj4)) : ((int?)null));
				}
			}
			int? num = obj2;
			onlineUsers = num.GetValueOrDefault();
		}
		catch
		{
		}
	}

	private IEnumerator GetReportData()
	{
		UnityWebRequest request = UnityWebRequest.Get("https://menu.seralyth.software/reportdata");
		try
		{
			yield return request.SendWebRequest();
			if ((int)request.result != 1)
			{
				yield break;
			}
			try
			{
				reportData.Clear();
				JObject json = JObject.Parse(request.downloadHandler.text);
				JToken val = json["report"];
				JObject report = (JObject)(object)((val is JObject) ? val : null);
				if (report != null)
				{
					foreach (JProperty item in report.Properties())
					{
						JToken value = item.Value;
						JObject value2 = (JObject)(object)((value is JObject) ? value : null);
						if (value2 != null)
						{
							ButtonType buttonType;
							ReportEntry entry = new ReportEntry
							{
								KnownAs = (((object)value2["known-as"]).ToString() ?? "Unknown"),
								Reason = (((object)value2["reason"]).ToString() ?? "No reason found"),
								ButtonType = (ButtonType)((!Enum.TryParse<ButtonType>(((object)value2["ButtonType"])?.ToString(), out buttonType)) ? 1 : ((int)buttonType)),
								Actor = (((object)value2["actor"]).ToString() ?? "Unknown")
							};
							if (reportData.TryGetValue(item.Name, out var existing))
							{
								entry.reportedIn = existing.reportedIn;
							}
							reportData[item.Name] = entry;
							existing = null;
						}
					}
				}
				if (NetworkSystem.Instance.InRoom)
				{
					LinqUtils.ForEach<NetPlayer>((IEnumerable<NetPlayer>)NetworkSystem.Instance.PlayerListOthers, (Action<NetPlayer>)delegate(NetPlayer p)
					{
						ShouldWeReport(p.GetPlayer());
					});
				}
			}
			catch
			{
			}
		}
		finally
		{
			((IDisposable)request)?.Dispose();
		}
	}
}
