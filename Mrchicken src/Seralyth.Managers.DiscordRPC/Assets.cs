using System;
using Seralyth.Managers.DiscordRPC.Exceptions;
using Valve.Newtonsoft.Json;

namespace Seralyth.Managers.DiscordRPC;

[Serializable]
public class Assets
{
	private const string EXTERNAL_KEY_PREFIX = "mp:external";

	private string _largeimagekey;

	private string _largeimagetext;

	private string _largeimageurl;

	private string _smallimagekey;

	private string _smallimagetext;

	private string _smallimageurl;

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string LargeImageKey
	{
		get
		{
			return _largeimagekey;
		}
		set
		{
			if (!BaseRichPresence.ValidateString(value, out _largeimagekey, useBytes: false, 256))
			{
				throw new StringOutOfRangeException(256);
			}
			LargeImageID = null;
		}
	}

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string LargeImageText
	{
		get
		{
			return _largeimagetext;
		}
		set
		{
			if (!BaseRichPresence.ValidateString(value, out _largeimagetext, useBytes: false, 128))
			{
				throw new StringOutOfRangeException(128);
			}
		}
	}

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string LargeImageUrl
	{
		get
		{
			return _largeimageurl;
		}
		set
		{
			if (!BaseRichPresence.ValidateString(value, out _largeimageurl, useBytes: false, 256))
			{
				throw new StringOutOfRangeException(256);
			}
			if (!BaseRichPresence.ValidateUrl(_largeimageurl))
			{
				throw new ArgumentException("Url must be a valid URI");
			}
		}
	}

	[JsonIgnore]
	public string LargeImageID { get; private set; }

	[JsonIgnore]
	public bool IsLargeImageKeyExternal { get; private set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string SmallImageKey
	{
		get
		{
			return _smallimagekey;
		}
		set
		{
			if (!BaseRichPresence.ValidateString(value, out _smallimagekey, useBytes: false, 256))
			{
				throw new StringOutOfRangeException(256);
			}
			SmallImageID = null;
		}
	}

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string SmallImageText
	{
		get
		{
			return _smallimagetext;
		}
		set
		{
			if (!BaseRichPresence.ValidateString(value, out _smallimagetext, useBytes: false, 128))
			{
				throw new StringOutOfRangeException(128);
			}
		}
	}

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string SmallImageUrl
	{
		get
		{
			return _smallimageurl;
		}
		set
		{
			if (!BaseRichPresence.ValidateString(value, out _smallimageurl, useBytes: false, 256))
			{
				throw new StringOutOfRangeException(256);
			}
			if (!BaseRichPresence.ValidateUrl(_smallimageurl))
			{
				throw new ArgumentException("Url must be a valid URI");
			}
		}
	}

	[JsonIgnore]
	public string SmallImageID { get; private set; }

	[JsonIgnore]
	public bool IsSmallImageKeyExternal { get; private set; }

	internal void Merge(Assets other)
	{
		_smallimagetext = other._smallimagetext;
		_smallimageurl = other._smallimageurl;
		_largeimagetext = other._largeimagetext;
		_largeimageurl = other._largeimageurl;
		string text = other._largeimagekey ?? "";
		ulong result;
		if (text.StartsWith("mp:external"))
		{
			IsLargeImageKeyExternal = true;
			LargeImageID = text;
		}
		else if (ulong.TryParse(text, out result))
		{
			IsLargeImageKeyExternal = false;
			LargeImageID = text;
		}
		else
		{
			IsLargeImageKeyExternal = false;
			LargeImageID = null;
			_largeimagekey = text;
		}
		string text2 = other._smallimagekey ?? "";
		if (text2.StartsWith("mp:external"))
		{
			IsSmallImageKeyExternal = true;
			SmallImageID = text2;
		}
		else if (ulong.TryParse(text2, out result))
		{
			IsSmallImageKeyExternal = false;
			SmallImageID = text2;
		}
		else
		{
			IsSmallImageKeyExternal = false;
			SmallImageID = null;
			_smallimagekey = text2;
		}
	}
}
