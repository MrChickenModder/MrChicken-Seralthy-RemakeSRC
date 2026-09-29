using System;
using Seralyth.Managers.DiscordRPC.Helper;
using Valve.Newtonsoft.Json;

namespace Seralyth.Managers.DiscordRPC;

[Serializable]
public class Party
{
	public enum PrivacySetting
	{
		Private,
		Public
	}

	private string _partyid;

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public string ID
	{
		get
		{
			return _partyid;
		}
		set
		{
			_partyid = value.GetNullOrString();
		}
	}

	[JsonIgnore]
	public int Size { get; set; }

	[JsonIgnore]
	public int Max { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public PrivacySetting Privacy { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	private int[] _size
	{
		get
		{
			int num = Math.Max(1, Size);
			return new int[2]
			{
				num,
				Math.Max(num, Max)
			};
		}
		set
		{
			if (value.Length != 2)
			{
				Size = 0;
				Max = 0;
			}
			else
			{
				Size = value[0];
				Max = value[1];
			}
		}
	}
}
