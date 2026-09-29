using System;
using System.Text;
using Seralyth.Managers.DiscordRPC.Exceptions;
using Valve.Newtonsoft.Json;

namespace Seralyth.Managers.DiscordRPC;

[Serializable]
public class Secrets
{
	[Obsolete("This feature has been deprecated my Mason in issue #152 on the offical library. Was originally used as a Notify Me feature, it has been replaced with Join / Spectate.", true)]
	[JsonIgnore]
	public string MatchSecret;

	private string _joinSecret;

	public const int SecretLength = 128;

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string Join
	{
		get
		{
			return _joinSecret;
		}
		set
		{
			if (!BaseRichPresence.ValidateString(value, out _joinSecret, useBytes: false, 128))
			{
				throw new StringOutOfRangeException(128);
			}
		}
	}

	[Obsolete("Property name is redundant and replaced with Join.")]
	[JsonIgnore]
	public string JoinSecret
	{
		get
		{
			return Join;
		}
		set
		{
			Join = value;
		}
	}

	[Obsolete("Spectating is no longer supported by Discord.")]
	[JsonIgnore]
	public string SpectateSecret { get; set; }

	public static Encoding Encoding => Encoding.UTF8;

	public static string CreateSecret(Random random, int length = 128)
	{
		if (length < 1 || length > 128)
		{
			throw new ArgumentOutOfRangeException("length", "Secret length must be between 1 and 128 characters.");
		}
		byte[] array = new byte[length];
		random.NextBytes(array);
		return Encoding.GetString(array);
	}

	public static string CreateFriendlySecret(Random random, int length = 128)
	{
		if (length < 1 || length > 128)
		{
			throw new ArgumentOutOfRangeException("length", "Secret length must be between 1 and 128 characters.");
		}
		string text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < length; i++)
		{
			stringBuilder.Append(text[random.Next(text.Length)]);
		}
		return stringBuilder.ToString();
	}
}
