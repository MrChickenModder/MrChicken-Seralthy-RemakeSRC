using Seralyth.Menu;
using Seralyth.Utilities;

namespace Seralyth.Extensions;

public static class StringExtensions
{
	public static string ClearTags(this string input)
	{
		return Main.NoRichtextTags(input);
	}

	public static string ToTitleCase(this string input)
	{
		return Main.ToTitleCase(input);
	}

	public static string Hash(this string input)
	{
		return Main.GetSHA256(input);
	}

	public static string EnforceLength(this string str, int maxLength)
	{
		return (str.Length > maxLength) ? str.Substring(0, maxLength) : str;
	}

	public static string Random(this string _, int length)
	{
		return RandomUtilities.RandomString(length);
	}
}
