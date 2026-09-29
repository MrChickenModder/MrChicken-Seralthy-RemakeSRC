using System;
using Seralyth.Managers.DiscordRPC.Message;

namespace Seralyth.Managers.DiscordRPC.Events;

[Obsolete("Spectating is no longer supported by Discord.")]
public delegate void OnSpectateEvent(object sender, SpectateMessage args);
