using System;
using Valve.Newtonsoft.Json;

namespace Seralyth.Managers.DiscordRPC;

public class User
{
	public enum AvatarFormat
	{
		PNG,
		JPEG,
		WebP,
		GIF
	}

	public enum AvatarSize
	{
		x16 = 0x10,
		x32 = 0x20,
		x64 = 0x40,
		x128 = 0x80,
		x256 = 0x100,
		x512 = 0x200,
		x1024 = 0x400,
		x2048 = 0x800
	}

	public struct AvatarDecorationData
	{
		[JsonProperty("asset")]
		public string Asset { get; private set; }

		[JsonProperty("skuId")]
		public string SKU { get; private set; }
	}

	[Flags]
	public enum Flag
	{
		None = 0,
		Employee = 1,
		Partner = 2,
		HypeSquad = 4,
		BugHunter = 8,
		HouseBravery = 0x40,
		HouseBrilliance = 0x80,
		HouseBalance = 0x100,
		EarlySupporter = 0x200,
		TeamUser = 0x400,
		BugHunterLevel2 = 0x4000,
		VerifiedBot = 0x10000,
		VerifiedDeveloper = 0x20000,
		CertifiedModerator = 0x40000,
		BotHttpInteractions = 0x80000,
		ActiveDeveloper = 0x400000
	}

	public enum PremiumType
	{
		None,
		NitroClassic,
		Nitro,
		NitroBasic
	}

	[JsonProperty("id")]
	public ulong ID { get; private set; }

	[JsonProperty("username")]
	public string Username { get; private set; }

	[JsonProperty("discriminator")]
	[Obsolete("Discord no longer uses discriminators.")]
	public int Discriminator { get; private set; }

	[JsonProperty("global_name")]
	public string DisplayName { get; private set; }

	[JsonProperty("avatar")]
	public string Avatar { get; private set; }

	public bool IsAvatarAnimated => Avatar != null && Avatar.StartsWith("a_");

	[JsonProperty("avatar_decoration_data")]
	public AvatarDecorationData? AvatarDecoration { get; private set; }

	[JsonProperty("bot")]
	public bool Bot { get; private set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public Flag Flags { get; private set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public PremiumType Premium { get; private set; }

	public string CdnEndpoint { get; private set; }

	internal User()
	{
		CdnEndpoint = "cdn.discordapp.com";
	}

	internal void SetConfiguration(Configuration configuration)
	{
		CdnEndpoint = configuration.CdnHost;
	}

	public string GetAvatarURL()
	{
		return GetAvatarURL(AvatarFormat.PNG, AvatarSize.x128);
	}

	public string GetAvatarURL(AvatarFormat format)
	{
		return GetAvatarURL(format, AvatarSize.x128);
	}

	public string GetAvatarURL(AvatarFormat format, AvatarSize size)
	{
		string text = $"/avatars/{ID}/{Avatar}";
		if (string.IsNullOrEmpty(Avatar))
		{
			if (format != AvatarFormat.PNG)
			{
				throw new BadImageFormatException("The user has no avatar and the requested format " + format.ToString() + " is not supported. (Only supports PNG).");
			}
			int num = (int)((ID >> 22) % 6);
			if (Discriminator > 0)
			{
				num = Discriminator % 5;
			}
			text = $"/embed/avatars/{num}";
		}
		return $"https://{CdnEndpoint}{text}{GetAvatarExtension(format)}?size={(int)size}&animated=true";
	}

	public string GetAvatarDecorationURL()
	{
		return GetAvatarDecorationURL(AvatarFormat.PNG);
	}

	public string GetAvatarDecorationURL(AvatarFormat format)
	{
		if (!AvatarDecoration.HasValue)
		{
			return null;
		}
		string arg = "/avatar-decoration-presets/" + AvatarDecoration.Value.Asset;
		return $"https://{CdnEndpoint}{arg}{GetAvatarExtension(format)}";
	}

	public string GetAvatarExtension(AvatarFormat format)
	{
		return "." + format.ToString().ToLowerInvariant();
	}

	public override string ToString()
	{
		if (!string.IsNullOrEmpty(DisplayName))
		{
			return DisplayName;
		}
		if (Discriminator != 0)
		{
			return Username + "#" + Discriminator.ToString("D4");
		}
		return Username;
	}
}
