using System;
using System.Text;
using Seralyth.Managers.DiscordRPC.Exceptions;
using Seralyth.Managers.DiscordRPC.Helper;
using Valve.Newtonsoft.Json;

namespace Seralyth.Managers.DiscordRPC;

[Serializable]
[JsonObject(/*Could not decode attribute arguments.*/)]
public class BaseRichPresence
{
	protected internal string _state;

	protected internal string _stateUrl;

	protected internal string _details;

	protected internal string _detailsUrl;

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string State
	{
		get
		{
			return _state;
		}
		set
		{
			if (!ValidateString(value, out _state, useBytes: false, 128))
			{
				throw new StringOutOfRangeException("State", 0, 128);
			}
		}
	}

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string StateUrl
	{
		get
		{
			return _stateUrl;
		}
		set
		{
			if (!ValidateString(value, out _stateUrl, useBytes: false, 256))
			{
				throw new StringOutOfRangeException(256);
			}
			if (!ValidateUrl(_stateUrl))
			{
				throw new ArgumentException("Url must be a valid URI");
			}
		}
	}

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string Details
	{
		get
		{
			return _details;
		}
		set
		{
			if (!ValidateString(value, out _details, useBytes: false, 128))
			{
				throw new StringOutOfRangeException(128);
			}
		}
	}

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string DetailsUrl
	{
		get
		{
			return _detailsUrl;
		}
		set
		{
			if (!ValidateString(value, out _detailsUrl, useBytes: false, 256))
			{
				throw new StringOutOfRangeException(256);
			}
			if (!ValidateUrl(_detailsUrl))
			{
				throw new ArgumentException("Url must be a valid URI");
			}
		}
	}

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public Timestamps Timestamps { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public Assets Assets { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public Party Party { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public Secrets Secrets { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public ActivityType Type { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public StatusDisplayType StatusDisplay { get; set; }

	public bool HasTimestamps()
	{
		return Timestamps != null && (Timestamps.Start.HasValue || Timestamps.End.HasValue);
	}

	public bool HasAssets()
	{
		return Assets != null;
	}

	public bool HasParty()
	{
		return Party != null && Party.ID != null;
	}

	public bool HasSecrets()
	{
		return Secrets != null && (Secrets.Join != null || Secrets.SpectateSecret != null);
	}

	internal static bool ValidateString(string str, out string result, bool useBytes, int length, Encoding encoding = null)
	{
		result = str;
		if (str == null)
		{
			return true;
		}
		string text = str.Trim();
		if ((useBytes && !text.WithinLength(length, encoding)) || text.Length > length)
		{
			return false;
		}
		result = text.GetNullOrString();
		return true;
	}

	internal static bool ValidateUrl(string url)
	{
		if (string.IsNullOrEmpty(url))
		{
			return true;
		}
		Uri result;
		return Uri.TryCreate(url, UriKind.Absolute, out result);
	}

	public static implicit operator bool(BaseRichPresence presence)
	{
		return presence != null;
	}

	internal virtual bool Matches(RichPresence other)
	{
		if (other == null)
		{
			return false;
		}
		if (State != other.State || StateUrl != other.StateUrl || Details != other.Details || DetailsUrl != other.DetailsUrl || Type != other.Type)
		{
			return false;
		}
		if (Timestamps != null)
		{
			if (other.Timestamps == null || other.Timestamps.StartUnixMilliseconds != Timestamps.StartUnixMilliseconds || other.Timestamps.EndUnixMilliseconds != Timestamps.EndUnixMilliseconds)
			{
				return false;
			}
		}
		else if (other.Timestamps != null)
		{
			return false;
		}
		if (Secrets != null)
		{
			if (other.Secrets == null || other.Secrets.Join != Secrets.Join || other.Secrets.SpectateSecret != Secrets.SpectateSecret)
			{
				return false;
			}
		}
		else if (other.Secrets != null)
		{
			return false;
		}
		if (Party != null)
		{
			if (other.Party == null || other.Party.ID != Party.ID || other.Party.Max != Party.Max || other.Party.Size != Party.Size || other.Party.Privacy != Party.Privacy)
			{
				return false;
			}
		}
		else if (other.Party != null)
		{
			return false;
		}
		if (Assets != null)
		{
			if (other.Assets == null || other.Assets.LargeImageKey != Assets.LargeImageKey || other.Assets.LargeImageText != Assets.LargeImageText || other.Assets.LargeImageUrl != Assets.LargeImageUrl || other.Assets.SmallImageKey != Assets.SmallImageKey || other.Assets.SmallImageText != Assets.SmallImageText || other.Assets.SmallImageUrl != Assets.SmallImageUrl)
			{
				return false;
			}
		}
		else if (other.Assets != null)
		{
			return false;
		}
		return true;
	}

	public RichPresence ToRichPresence()
	{
		RichPresence richPresence = new RichPresence
		{
			State = State,
			StateUrl = StateUrl,
			Details = Details,
			DetailsUrl = DetailsUrl,
			Type = Type,
			StatusDisplay = StatusDisplay,
			Party = ((!HasParty()) ? Party : null),
			Secrets = ((!HasSecrets()) ? Secrets : null)
		};
		if (HasAssets())
		{
			richPresence.Assets = new Assets
			{
				SmallImageKey = Assets.SmallImageKey,
				SmallImageText = Assets.SmallImageText,
				SmallImageUrl = Assets.SmallImageUrl,
				LargeImageKey = Assets.LargeImageKey,
				LargeImageText = Assets.LargeImageText,
				LargeImageUrl = Assets.LargeImageUrl
			};
		}
		if (HasTimestamps())
		{
			richPresence.Timestamps = new Timestamps();
			if (Timestamps.Start.HasValue)
			{
				richPresence.Timestamps.Start = Timestamps.Start;
			}
			if (Timestamps.End.HasValue)
			{
				richPresence.Timestamps.End = Timestamps.End;
			}
		}
		return richPresence;
	}
}
