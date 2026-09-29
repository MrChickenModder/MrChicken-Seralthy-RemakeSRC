using System;
using System.Text;
using Seralyth.Managers.DiscordRPC.Exceptions;
using Valve.Newtonsoft.Json;

namespace Seralyth.Managers.DiscordRPC;

public class Button
{
	private string _label;

	private string _url;

	[JsonProperty("label")]
	public string Label
	{
		get
		{
			return _label;
		}
		set
		{
			if (!BaseRichPresence.ValidateString(value, out _label, useBytes: true, 31, Encoding.UTF8))
			{
				throw new StringOutOfRangeException(31);
			}
		}
	}

	[JsonProperty("url")]
	public string Url
	{
		get
		{
			return _url;
		}
		set
		{
			if (!BaseRichPresence.ValidateString(value, out _url, useBytes: false, 512))
			{
				throw new StringOutOfRangeException(512);
			}
			if (!BaseRichPresence.ValidateUrl(_url))
			{
				throw new ArgumentException("Url must be a valid URI");
			}
		}
	}
}
