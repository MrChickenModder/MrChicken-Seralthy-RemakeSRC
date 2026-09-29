using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using Seralyth.Managers.DiscordRPC.Logging;

namespace Seralyth.Managers.DiscordRPC.IO;

public sealed class ManagedNamedPipeClient : INamedPipeClient, IDisposable
{
	private NamedPipeClientStream _stream;

	private readonly byte[] _buffer = new byte[PipeFrame.MAX_SIZE];

	private readonly Queue<PipeFrame> _framequeue = new Queue<PipeFrame>();

	private readonly object _framequeuelock = new object();

	private volatile bool _isDisposed = false;

	private volatile bool _isClosed = true;

	private readonly object l_stream = new object();

	public ILogger Logger { get; set; }

	public bool IsConnected
	{
		get
		{
			if (_isClosed)
			{
				return false;
			}
			lock (l_stream)
			{
				return _stream != null && _stream.IsConnected;
			}
		}
	}

	public int ConnectedPipe { get; private set; }

	public ManagedNamedPipeClient()
	{
		_buffer = new byte[PipeFrame.MAX_SIZE];
		Logger = new NullLogger();
		_stream = null;
	}

	public bool Connect(int pipe)
	{
		Logger.Trace("ManagedNamedPipeClient.Connection({0})", pipe);
		if (_isDisposed)
		{
			throw new ObjectDisposedException("NamedPipe");
		}
		if (pipe > 9)
		{
			throw new ArgumentOutOfRangeException("pipe", "Argument cannot be greater than 9");
		}
		int startPipe = 0;
		if (pipe >= 0)
		{
			startPipe = pipe;
		}
		foreach (string pipe2 in PipeLocation.GetPipes(startPipe))
		{
			if (AttemptConnection(pipe2))
			{
				BeginReadStream();
				return true;
			}
		}
		return false;
	}

	private bool AttemptConnection(string pipename)
	{
		if (_isDisposed)
		{
			throw new ObjectDisposedException("_stream");
		}
		try
		{
			lock (l_stream)
			{
				Logger.Info("Attempting to connect to '{0}'", pipename);
				_stream = new NamedPipeClientStream(".", pipename, PipeDirection.InOut, PipeOptions.Asynchronous);
				_stream.Connect(0);
				Logger.Trace("Waiting for connection...");
				do
				{
					Thread.Sleep(10);
				}
				while (!_stream.IsConnected);
			}
			Logger.Info("Connected to '{0}'", pipename);
			ConnectedPipe = int.Parse(pipename.Substring(pipename.LastIndexOf('-')));
			_isClosed = false;
		}
		catch (Exception ex)
		{
			Logger.Error("Failed connection to {0}. {1}", pipename, ex.Message);
			Close();
		}
		Logger.Trace("Done. Result: {0}", _isClosed);
		return !_isClosed;
	}

	private void BeginReadStream()
	{
		if (_isClosed)
		{
			return;
		}
		try
		{
			lock (l_stream)
			{
				if (_stream != null && _stream.IsConnected)
				{
					Logger.Trace("Begining Read of {0} bytes", _buffer.Length);
					_stream.BeginRead(_buffer, 0, _buffer.Length, EndReadStream, _stream.IsConnected);
				}
			}
		}
		catch (ObjectDisposedException)
		{
			Logger.Warning("Attempted to start reading from a disposed pipe");
		}
		catch (InvalidOperationException)
		{
			Logger.Warning("Attempted to start reading from a closed pipe");
		}
		catch (Exception ex3)
		{
			Logger.Error("An exception occured while starting to read a stream: {0}", ex3.Message);
			Logger.Error(ex3.StackTrace);
		}
	}

	private void EndReadStream(IAsyncResult callback)
	{
		Logger.Trace("Ending Read");
		int num = 0;
		try
		{
			lock (l_stream)
			{
				if (_stream == null || !_stream.IsConnected)
				{
					return;
				}
				num = _stream.EndRead(callback);
			}
		}
		catch (IOException)
		{
			Logger.Warning("Attempted to end reading from a closed pipe");
			return;
		}
		catch (NullReferenceException)
		{
			Logger.Warning("Attempted to read from a null pipe");
			return;
		}
		catch (ObjectDisposedException)
		{
			Logger.Warning("Attemped to end reading from a disposed pipe");
			return;
		}
		catch (Exception ex4)
		{
			Logger.Error("An exception occured while ending a read of a stream: {0}", ex4.Message);
			Logger.Error(ex4.StackTrace);
			return;
		}
		Logger.Trace("Read {0} bytes", num);
		if (num > 0)
		{
			using MemoryStream stream = new MemoryStream(_buffer, 0, num);
			try
			{
				PipeFrame item = default(PipeFrame);
				if (item.ReadStream(stream))
				{
					Logger.Trace("Read a frame: {0}", item.Opcode);
					lock (_framequeuelock)
					{
						_framequeue.Enqueue(item);
					}
				}
				else
				{
					Logger.Error("Pipe failed to read from the data received by the stream.");
					Close();
				}
			}
			catch (Exception ex5)
			{
				Logger.Error("A exception has occured while trying to parse the pipe data: {0}", ex5.Message);
				Close();
			}
		}
		else
		{
			Logger.Error("Empty frame was read on {0}, aborting.", Environment.OSVersion);
			Close();
		}
		if (!_isClosed && IsConnected)
		{
			Logger.Trace("Starting another read");
			BeginReadStream();
		}
	}

	public bool ReadFrame(out PipeFrame frame)
	{
		if (_isDisposed)
		{
			throw new ObjectDisposedException("_stream");
		}
		lock (_framequeuelock)
		{
			if (_framequeue.Count == 0)
			{
				frame = default(PipeFrame);
				return false;
			}
			frame = _framequeue.Dequeue();
			return true;
		}
	}

	public bool WriteFrame(PipeFrame frame)
	{
		if (_isDisposed)
		{
			throw new ObjectDisposedException("_stream");
		}
		if (_isClosed || !IsConnected)
		{
			Logger.Error("Failed to write frame because the stream is closed");
			return false;
		}
		try
		{
			frame.WriteStream(_stream);
			return true;
		}
		catch (IOException ex)
		{
			Logger.Error("Failed to write frame because of a IO Exception: {0}", ex.Message);
		}
		catch (ObjectDisposedException)
		{
			Logger.Warning("Failed to write frame as the stream was already disposed");
		}
		catch (InvalidOperationException)
		{
			Logger.Warning("Failed to write frame because of a invalid operation");
		}
		return false;
	}

	public void Close()
	{
		if (_isClosed)
		{
			Logger.Warning("Tried to close a already closed pipe.");
			return;
		}
		try
		{
			lock (l_stream)
			{
				if (_stream != null)
				{
					try
					{
						_stream.Flush();
						_stream.Dispose();
					}
					catch (Exception)
					{
					}
					_stream = null;
					_isClosed = true;
				}
				else
				{
					Logger.Warning("Stream was closed, but no stream was available to begin with!");
				}
			}
		}
		catch (ObjectDisposedException)
		{
			Logger.Warning("Tried to dispose already disposed stream");
		}
		finally
		{
			_isClosed = true;
		}
	}

	public void Dispose()
	{
		if (_isDisposed)
		{
			return;
		}
		if (!_isClosed)
		{
			Close();
		}
		lock (l_stream)
		{
			if (_stream != null)
			{
				_stream.Dispose();
				_stream = null;
			}
		}
		_isDisposed = true;
	}

	[Obsolete("Use PipePermutation.GetPipes instead", true)]
	public static string GetPipeName(int pipe)
	{
		return string.Empty;
	}

	[Obsolete("Use PipePermutation.GetPipes instead", true)]
	public static string GetPipeName(int pipe, string sandbox)
	{
		return string.Empty;
	}

	[Obsolete("Use PipePermutation.GetPipes instead", true)]
	public static string GetPipeSandbox()
	{
		return string.Empty;
	}

	[Obsolete("Use PipePermutation.GetPipes instead", true)]
	public static bool IsUnix()
	{
		return true;
	}
}
