using System;
using Seralyth.Managers.DiscordRPC.Logging;

namespace Seralyth.Managers.DiscordRPC.IO;

public interface INamedPipeClient : IDisposable
{
	ILogger Logger { get; set; }

	bool IsConnected { get; }

	[Obsolete("The connected pipe is not neccessary information.")]
	int ConnectedPipe { get; }

	bool Connect(int pipe);

	bool ReadFrame(out PipeFrame frame);

	bool WriteFrame(PipeFrame frame);

	void Close();
}
