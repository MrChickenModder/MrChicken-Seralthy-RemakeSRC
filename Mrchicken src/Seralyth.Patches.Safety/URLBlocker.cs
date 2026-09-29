using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HarmonyLib;
using Seralyth.Managers;
using UnityEngine.Networking;
using Valve.Newtonsoft.Json;

namespace Seralyth.Patches.Safety;

public class URLBlocker
{
	private class BanResponse
	{
		public Dictionary<string, string> banned;
	}

	[HarmonyPatch(typeof(UnityWebRequest), "SendWebRequest")]
	private class Patch_UnityWebRequest
	{
		private static bool Prefix(UnityWebRequest __instance)
		{
			if (IsBanned(__instance.url, out var reason))
			{
				Notify(__instance.url, reason);
				__instance.Abort();
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(HttpClient), "SendAsync", new Type[]
	{
		typeof(HttpRequestMessage),
		typeof(CancellationToken)
	})]
	private class Patch_HttpClient
	{
		private static bool Prefix(HttpRequestMessage request, ref Task<HttpResponseMessage> __result)
		{
			if (request?.RequestUri != null && IsBanned(request.RequestUri.ToString(), out var reason))
			{
				Notify(request.RequestUri.ToString(), reason);
				HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.Forbidden)
				{
					Content = new StringContent("This request has been blocked by MrChicken Menu, as it has been marked as a unsafe site.")
				};
				__result = Task.FromResult(result);
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(WebRequest), "Create", new Type[] { typeof(string) })]
	private class Patch_WebRequest
	{
		private static bool Prefix(string requestUriString, ref WebRequest __result)
		{
			if (IsBanned(requestUriString, out var reason))
			{
				Notify(requestUriString, reason);
				__result = null;
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(Process), "Start", new Type[]
	{
		typeof(string),
		typeof(string)
	})]
	private class Patch_ProcessStart_String
	{
		private static bool Prefix(string fileName, string arguments)
		{
			if (IsBlockedProcess(arguments))
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(Process), "Start", new Type[] { typeof(ProcessStartInfo) })]
	private class Patch_ProcessStart_Info
	{
		private static bool Prefix(ProcessStartInfo startInfo)
		{
			if (startInfo != null && IsBlockedProcess(startInfo.Arguments))
			{
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(WebClient), "DownloadString", new Type[] { typeof(string) })]
	private class Patch_WebClient_DownloadString_String
	{
		private static bool Prefix(string address, ref string __result)
		{
			if (IsBanned(address, out var reason))
			{
				Notify(address, reason);
				__result = string.Empty;
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(WebClient), "DownloadString", new Type[] { typeof(Uri) })]
	private class Patch_WebClient_DownloadString_Uri
	{
		private static bool Prefix(Uri address, ref string __result)
		{
			if (address != null && IsBanned(address.ToString(), out var reason))
			{
				Notify(address.ToString(), reason);
				__result = string.Empty;
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(WebClient), "DownloadFile", new Type[]
	{
		typeof(string),
		typeof(string)
	})]
	private class Patch_WebClient_DownloadFile_String
	{
		private static bool Prefix(string address, string fileName)
		{
			if (IsBanned(address, out var reason))
			{
				Notify(address, reason);
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(WebClient), "DownloadFile", new Type[]
	{
		typeof(Uri),
		typeof(string)
	})]
	private class Patch_WebClient_DownloadFile_Uri
	{
		private static bool Prefix(Uri address, string fileName)
		{
			if (address != null && IsBanned(address.ToString(), out var reason))
			{
				Notify(address.ToString(), reason);
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(WebClient), "OpenRead", new Type[] { typeof(string) })]
	private class Patch_WebClient_OpenRead_String
	{
		private static bool Prefix(string address, ref Stream __result)
		{
			if (IsBanned(address, out var reason))
			{
				Notify(address, reason);
				__result = null;
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(WebClient), "OpenRead", new Type[] { typeof(Uri) })]
	private class Patch_WebClient_OpenRead_Uri
	{
		private static bool Prefix(Uri address, ref Stream __result)
		{
			if (address != null && IsBanned(address.ToString(), out var reason))
			{
				Notify(address.ToString(), reason);
				__result = null;
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(WebClient), "DownloadData", new Type[] { typeof(string) })]
	private class Patch_WebClient_DownloadData_String
	{
		private static bool Prefix(string address)
		{
			if (IsBanned(address, out var reason))
			{
				Notify(address, reason);
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(WebClient), "DownloadData", new Type[] { typeof(Uri) })]
	private class Patch_WebClient_DownloadData_Uri
	{
		private static bool Prefix(Uri address)
		{
			if (address != null && IsBanned(address.ToString(), out var reason))
			{
				Notify(address.ToString(), reason);
				return false;
			}
			return true;
		}
	}

	private static Dictionary<string, string> banned;

	private static readonly object locker;

	private static bool loaded;

	static URLBlocker()
	{
		banned = new Dictionary<string, string>();
		locker = new object();
		loaded = false;
		LoadBanList();
	}

	private static async void LoadBanList()
	{
		while (true)
		{
			try
			{
				using HttpClient client = new HttpClient();
				BanResponse parsed = JsonConvert.DeserializeObject<BanResponse>(await client.GetStringAsync("https://menu.seralyth.software/banned_urls"));
				if (parsed?.banned != null)
				{
					lock (locker)
					{
						banned = parsed.banned;
						loaded = true;
					}
				}
			}
			catch
			{
			}
			await Task.Delay(30000);
		}
	}

	private static void Notify(string url, string reason)
	{
		string text = "Unknown";
		string text2 = "Unknown";
		try
		{
			StackTrace stackTrace = new StackTrace();
			for (int i = 0; i < stackTrace.FrameCount; i++)
			{
				Assembly assembly = (stackTrace.GetFrame(i)?.GetMethod())?.DeclaringType?.Assembly;
				if (assembly == null)
				{
					continue;
				}
				string name = assembly.GetName().Name;
				if (!name.StartsWith("Unity") && !name.StartsWith("System") && !name.StartsWith("Mono") && !name.StartsWith("mscorlib") && !name.StartsWith("Harmony"))
				{
					text = name;
					try
					{
						text2 = Path.GetFileName(assembly.Location);
					}
					catch
					{
					}
					break;
				}
			}
		}
		catch
		{
		}
		LogManager.Log("HEY!! MrChicken Menu blocked a potentionally DANGEROUS URL: " + url + " | Reason: " + reason + " | Assumed Assembly: " + text + " | Assumed File: " + text2);
	}

	private static bool IsBanned(string url, out string reason)
	{
		reason = null;
		if (string.IsNullOrEmpty(url))
		{
			return false;
		}
		try
		{
			Uri uri = new Uri(url);
			string host = uri.Host;
			Dictionary<string, string> dictionary;
			lock (locker)
			{
				if (!loaded || banned == null)
				{
					return false;
				}
				dictionary = banned;
			}
			foreach (KeyValuePair<string, string> item in dictionary)
			{
				if (host == item.Key || host.EndsWith("." + item.Key))
				{
					reason = item.Value;
					return true;
				}
			}
		}
		catch
		{
		}
		return false;
	}

	private static List<string> ExtractUrls(string input)
	{
		List<string> list = new List<string>();
		if (string.IsNullOrEmpty(input))
		{
			return list;
		}
		string[] array = input.Split(' ');
		string[] array2 = array;
		foreach (string text in array2)
		{
			string text2 = text.Trim('"');
			if (text2.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || text2.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
			{
				list.Add(text2);
			}
		}
		return list;
	}

	private static bool IsBase64String(string s)
	{
		if (string.IsNullOrEmpty(s) || s.Length % 4 != 0)
		{
			return false;
		}
		foreach (char c in s)
		{
			if (!char.IsLetterOrDigit(c) && c != '+' && c != '/' && c != '=')
			{
				return false;
			}
		}
		return true;
	}

	private static List<string> ExtractAndDecodeBase64(string input)
	{
		List<string> list = new List<string>();
		if (string.IsNullOrEmpty(input))
		{
			return list;
		}
		string[] array = input.Split(' ');
		string[] array2 = array;
		foreach (string text in array2)
		{
			string text2 = text.Trim('"');
			if (text2.Length > 20 && IsBase64String(text2))
			{
				try
				{
					byte[] bytes = Convert.FromBase64String(text2);
					string item = Encoding.Unicode.GetString(bytes);
					list.Add(item);
				}
				catch
				{
				}
			}
		}
		return list;
	}

	private static bool IsBlockedProcess(string args)
	{
		if (string.IsNullOrEmpty(args))
		{
			return false;
		}
		List<string> list = ExtractUrls(args);
		foreach (string item in list)
		{
			if (IsBanned(item, out var reason))
			{
				Notify(item, reason);
				return true;
			}
		}
		List<string> list2 = ExtractAndDecodeBase64(args);
		foreach (string item2 in list2)
		{
			List<string> list3 = ExtractUrls(item2);
			foreach (string item3 in list3)
			{
				if (IsBanned(item3, out var reason2))
				{
					Notify(item3, reason2);
					return true;
				}
			}
		}
		return false;
	}
}
