using System;
using System.Collections.Generic;
using GorillaNetworking;
using PlayFab;

namespace Seralyth.Patches.Menu;

public class ErrorPatches
{
	public static bool enabled;

	public static bool ErrorCall(PlayFabError error)
	{
		if (!enabled)
		{
			return true;
		}
		if (!error.ErrorMessage.Contains("is currently banned"))
		{
			return true;
		}
		using Dictionary<string, List<string>>.Enumerator enumerator = error.ErrorDetails.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			return false;
		}
		KeyValuePair<string, List<string>> current = enumerator.Current;
		bool flag = current.Value[0] == "Indefinite";
		DateTime dateTime = (flag ? DateTime.MaxValue : DateTime.Parse(current.Value[0]));
		TimeSpan time = dateTime - DateTime.UtcNow;
		string text = "Your account " + ((PlayFabAuthenticator)PlayFabAuthenticator.instance).GetPlayFabPlayerId() + " has been banned.\nBan Reason: " + current.Key + "\nTime Left: " + (flag ? "Indefinite" : FormatTimeLeft(time)) + "\nUnban Date: " + (flag ? "Never" : dateTime.ToString("MMMM dd, yyyy h:mm tt"));
		((GorillaComputer)GorillaComputer.instance).GeneralFailureMessage(text);
		return false;
	}

	private static string FormatTimeLeft(TimeSpan time)
	{
		if (time <= TimeSpan.Zero)
		{
			return "Expired";
		}
		List<string> list = new List<string>();
		int num = time.Days / 30;
		int num2 = time.Days % 30 / 7;
		int num3 = time.Days % 30 % 7;
		if (num > 0)
		{
			list.Add($"{num} months");
		}
		if (num2 > 0)
		{
			list.Add($"{num2} weeks");
		}
		if (num3 > 0)
		{
			list.Add($"{num3} days");
		}
		if (time.Hours > 0)
		{
			list.Add($"{time.Hours} hours");
		}
		if (time.Minutes > 0)
		{
			list.Add($"{time.Minutes} minutes");
		}
		if (time.Seconds > 0)
		{
			list.Add($"{time.Seconds} seconds");
		}
		return string.Join(" ", list);
	}
}
