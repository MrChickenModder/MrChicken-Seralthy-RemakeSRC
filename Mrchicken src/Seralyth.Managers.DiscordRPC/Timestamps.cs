using System;
using Valve.Newtonsoft.Json;

namespace Seralyth.Managers.DiscordRPC;

[Serializable]
public class Timestamps
{
	public static Timestamps Now => new Timestamps(DateTime.UtcNow);

	[JsonIgnore]
	public DateTime? Start { get; set; }

	[JsonIgnore]
	public DateTime? End { get; set; }

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public ulong? StartUnixMilliseconds
	{
		get
		{
			return Start.HasValue ? new ulong?(ToUnixMilliseconds(Start.Value)) : ((ulong?)null);
		}
		set
		{
			Start = (value.HasValue ? new DateTime?(FromUnixMilliseconds(value.Value)) : ((DateTime?)null));
		}
	}

	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public ulong? EndUnixMilliseconds
	{
		get
		{
			return End.HasValue ? new ulong?(ToUnixMilliseconds(End.Value)) : ((ulong?)null);
		}
		set
		{
			End = (value.HasValue ? new DateTime?(FromUnixMilliseconds(value.Value)) : ((DateTime?)null));
		}
	}

	public static Timestamps FromTimeSpan(double seconds)
	{
		return FromTimeSpan(TimeSpan.FromSeconds(seconds));
	}

	public static Timestamps FromTimeSpan(TimeSpan timespan)
	{
		return new Timestamps
		{
			Start = DateTime.UtcNow,
			End = DateTime.UtcNow + timespan
		};
	}

	public Timestamps()
	{
		Start = null;
		End = null;
	}

	public Timestamps(DateTime start)
	{
		Start = start;
		End = null;
	}

	public Timestamps(DateTime start, DateTime end)
	{
		Start = start;
		End = end;
	}

	public static DateTime FromUnixMilliseconds(ulong unixTime)
	{
		return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(Convert.ToDouble(unixTime));
	}

	public static ulong ToUnixMilliseconds(DateTime date)
	{
		DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		return Convert.ToUInt64((date - dateTime).TotalMilliseconds);
	}
}
