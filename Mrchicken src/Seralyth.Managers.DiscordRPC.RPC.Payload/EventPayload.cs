using Seralyth.Managers.DiscordRPC.Converters;
using Valve.Newtonsoft.Json;
using Valve.Newtonsoft.Json.Linq;

namespace Seralyth.Managers.DiscordRPC.RPC.Payload;

internal class EventPayload : IPayload
{
	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public JObject Data { get; set; }

	[JsonProperty("evt")]
	[JsonConverter(typeof(EnumSnakeCaseConverter))]
	public ServerEvent? Event { get; set; }

	public EventPayload()
	{
		Data = null;
	}

	public EventPayload(long nonce)
		: base(nonce)
	{
		Data = null;
	}

	public T GetObject<T>()
	{
		return (Data == null) ? default(T) : ((JToken)Data).ToObject<T>();
	}

	public override string ToString()
	{
		return "Event " + base.ToString() + ", Event: " + (Event.HasValue ? Event.ToString() : "N/A");
	}
}
