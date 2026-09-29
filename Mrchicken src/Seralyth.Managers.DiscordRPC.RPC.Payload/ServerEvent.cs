using Seralyth.Managers.DiscordRPC.Converters;

namespace Seralyth.Managers.DiscordRPC.RPC.Payload;

internal enum ServerEvent
{
	[EnumValue("READY")]
	Ready,
	[EnumValue("ERROR")]
	Error,
	[EnumValue("ACTIVITY_JOIN")]
	ActivityJoin,
	[EnumValue("ACTIVITY_SPECTATE")]
	ActivitySpectate,
	[EnumValue("ACTIVITY_JOIN_REQUEST")]
	ActivityJoinRequest
}
