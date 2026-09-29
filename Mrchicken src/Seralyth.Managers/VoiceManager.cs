using System;
using System.Collections.Generic;
using Photon.Voice;
using Seralyth.Mods;
using UnityEngine;

namespace Seralyth.Managers;

public class VoiceManager : IAudioReader<float>, IDataReader<float>, IDisposable, IAudioDesc
{
	public sealed class Clip
	{
		public float[] Samples;

		public int Channels;

		private float _samplePosition;

		public Guid Id { get; set; }

		public AudioClip Source { get; set; }

		public float Step { get; set; }

		public bool MuteMicrophone { get; set; }

		public float Volume { get; set; } = 1f;

		public float Speed { get; set; } = 1f;

		public bool IsPaused { get; set; }

		public bool Looping { get; set; }

		public float Length => (Samples != null && Channels > 0) ? ((float)(Samples.Length / Channels) / (float)Instance.OutputRate) : 0f;

		public float CurrentTime
		{
			get
			{
				return (Samples != null && Channels > 0) ? (_samplePosition / (float)(Samples.Length / Channels) * Length) : 0f;
			}
			set
			{
				Seek(value);
			}
		}

		public float InternalPosition
		{
			get
			{
				return _samplePosition;
			}
			set
			{
				_samplePosition = value;
			}
		}

		public void Pause()
		{
			IsPaused = true;
		}

		public void Resume()
		{
			IsPaused = false;
		}

		public void Seek(float seconds)
		{
			if (Samples != null && Channels > 0)
			{
				float num = seconds * (float)Instance.OutputRate;
				int num2 = Samples.Length / Channels;
				_samplePosition = Mathf.Clamp(num, 0f, (float)num2);
			}
		}
	}

	private int samplingRate = 48000;

	private int outputRate = 48000;

	private float gain = 1f;

	private float clipVolume = 1f;

	private float pitch = 1f;

	private float clipSpeed = 1f;

	private readonly int loopLength;

	private string currentDevice;

	public AudioClip microphoneClip;

	private int lastSamplePosition;

	private float step;

	private string error;

	private float[] rawMicrophoneData;

	private float[] microphoneBuffer;

	private float resamplePointer;

	private readonly object audioClipsLock = new object();

	private readonly List<Clip> audioClips = new List<Clip>();

	private bool muteMicrophone;

	public readonly Dictionary<string, Action<float[]>> PostProcessors = new Dictionary<string, Action<float[]>>();

	public IReadOnlyList<Clip> AudioClips
	{
		get
		{
			lock (audioClipsLock)
			{
				return audioClips.ToArray();
			}
		}
	}

	public bool MuteMicrophone
	{
		get
		{
			return muteMicrophone;
		}
		set
		{
			muteMicrophone = value;
		}
	}

	public int SamplingRate
	{
		get
		{
			return samplingRate;
		}
		set
		{
			samplingRate = Mathf.Max(8000, value);
			RestartMicrophone();
		}
	}

	public int OutputRate
	{
		get
		{
			return outputRate;
		}
		set
		{
			outputRate = Mathf.Max(8000, value);
			RestartMicrophone();
		}
	}

	public float Gain
	{
		get
		{
			return gain;
		}
		set
		{
			gain = Mathf.Max(0f, value);
		}
	}

	public float ClipVolume
	{
		get
		{
			return clipVolume;
		}
		set
		{
			clipVolume = Mathf.Max(0f, value);
		}
	}

	public float Pitch
	{
		get
		{
			return pitch;
		}
		set
		{
			pitch = Mathf.Max(0.1f, value);
		}
	}

	public float ClipSpeed
	{
		get
		{
			return clipSpeed;
		}
		set
		{
			clipSpeed = Mathf.Max(0.1f, value);
		}
	}

	public bool PostProcessClip { get; set; }

	public int Channels => 2;

	public string Error => error;

	public string CurrentDevice => currentDevice;

	public static VoiceManager Instance { get; private set; }

	public VoiceManager(int loopLength = 1, string device = null)
	{
		this.loopLength = Mathf.Max(1, loopLength);
		if (Instance == null)
		{
			Instance = this;
		}
		StartRecording(device);
	}

	public static VoiceManager Get(int loopLength = 1, string device = null)
	{
		return Instance ?? (Instance = new VoiceManager(loopLength, device));
	}

	public bool StartRecording(string device = null)
	{
		error = null;
		if (Microphone.devices == null || Microphone.devices.Length == 0)
		{
			error = "No microphone devices found";
			LogManager.LogWarning(error);
			return false;
		}
		if (string.IsNullOrEmpty(device))
		{
			currentDevice = Microphone.devices[0];
		}
		else
		{
			bool flag = false;
			for (int i = 0; i < Microphone.devices.Length; i++)
			{
				if (Microphone.devices[i] == device)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				error = "Microphone device '" + device + "' not found";
				LogManager.LogError(error);
				return false;
			}
			currentDevice = device;
		}
		if (Microphone.IsRecording(currentDevice))
		{
			Microphone.End(currentDevice);
		}
		microphoneClip = Microphone.Start(currentDevice, true, loopLength, samplingRate);
		if ((Object)(object)microphoneClip == (Object)null)
		{
			error = "Failed to start microphone '" + currentDevice + "'";
			LogManager.LogError(error);
			return false;
		}
		lastSamplePosition = 0;
		step = (float)samplingRate / (float)OutputRate;
		resamplePointer = 0f;
		return true;
	}

	public bool StopRecording()
	{
		if (!string.IsNullOrEmpty(currentDevice) && Microphone.IsRecording(currentDevice))
		{
			Microphone.End(currentDevice);
		}
		microphoneClip = null;
		lastSamplePosition = 0;
		resamplePointer = 0f;
		return true;
	}

	public bool SwitchMicrophone(string device)
	{
		return StopRecording() && StartRecording(device);
	}

	public bool RestartMicrophone()
	{
		return StopRecording() && StartRecording(currentDevice);
	}

	public Clip AudioClip(AudioClip audioClip, bool disableMicrophone = false)
	{
		if ((Object)(object)audioClip == (Object)null)
		{
			return null;
		}
		Guid id = Guid.NewGuid();
		int num = Mathf.Max(1, audioClip.channels);
		float[] array = new float[audioClip.samples * num];
		audioClip.GetData(array, 0);
		try
		{
			if (audioClip.frequency != OutputRate)
			{
				array = Resample(array, audioClip.frequency, OutputRate, num);
			}
			Clip clip = new Clip
			{
				Id = id,
				Source = audioClip,
				Samples = array,
				Channels = num,
				Step = 1f,
				MuteMicrophone = disableMicrophone,
				Volume = clipVolume,
				Speed = clipSpeed,
				Looping = Sound.LoopAudio
			};
			lock (audioClipsLock)
			{
				audioClips.Add(clip);
			}
			return clip;
		}
		catch (Exception arg)
		{
			LogManager.LogError($"Failed to insert audio clip: {arg}");
			return null;
		}
	}

	public Clip GetAudioClip(Guid id)
	{
		lock (audioClipsLock)
		{
			int num = audioClips.FindIndex((Clip c) => c.Id == id);
			if (num == -1)
			{
				return null;
			}
			return audioClips[num];
		}
	}

	public static float[] Resample(float[] source, int sourceRate, int targetRate, int channels)
	{
		if (source == null || source.Length == 0 || sourceRate <= 0 || sourceRate == targetRate)
		{
			return source;
		}
		int num = Mathf.Max(1, source.Length / channels);
		float num2 = (float)num / (float)sourceRate;
		int num3 = Mathf.Max(1, Mathf.RoundToInt(num2 * (float)targetRate));
		float[] array = new float[num3 * channels];
		if (num == 1 || num3 == 1)
		{
			for (int i = 0; i < channels && i < array.Length; i++)
			{
				array[i] = source[Mathf.Clamp(i, 0, source.Length - 1)];
			}
		}
		else
		{
			float num4 = ((float)num - 1f) / ((float)num3 - 1f);
			for (int j = 0; j < num3; j++)
			{
				float num5 = (float)j * num4;
				int num6 = Mathf.Clamp((int)num5, 0, num - 1);
				int num7 = Mathf.Clamp(num6 + 1, 0, num - 1);
				float num8 = num5 - (float)num6;
				for (int k = 0; k < channels; k++)
				{
					int num9 = j * channels + k;
					int num10 = Mathf.Clamp(num6 * channels + k, 0, source.Length - 1);
					int num11 = Mathf.Clamp(num7 * channels + k, 0, source.Length - 1);
					array[num9] = Mathf.Lerp(source[num10], source[num11], num8);
				}
			}
		}
		return array;
	}

	public bool StopAudioClip(Clip clip)
	{
		if (clip == null)
		{
			return false;
		}
		lock (audioClipsLock)
		{
			return audioClips.Remove(clip);
		}
	}

	public void StopAudioClips()
	{
		lock (audioClipsLock)
		{
			audioClips.Clear();
		}
	}

	public bool Read(float[] buffer)
	{
		if (buffer == null || buffer.Length == 0)
		{
			return false;
		}
		if ((Object)(object)microphoneClip == (Object)null || string.IsNullOrEmpty(currentDevice))
		{
			return false;
		}
		int num = buffer.Length / Channels;
		int num2 = Mathf.Max(1, microphoneClip.channels);
		int samples = microphoneClip.samples;
		int num3 = samples * num2;
		if (rawMicrophoneData == null || rawMicrophoneData.Length != num3)
		{
			rawMicrophoneData = new float[num3];
		}
		if (microphoneBuffer == null || microphoneBuffer.Length != buffer.Length)
		{
			microphoneBuffer = new float[buffer.Length];
		}
		int position = Microphone.GetPosition(currentDevice);
		int num4 = lastSamplePosition;
		int num5 = ((position < num4) ? (samples - num4 + position) : (position - num4));
		float num6 = microphoneClip.frequency;
		float num7 = num6 / (float)outputRate * pitch;
		int num8 = Mathf.CeilToInt((float)num * num7) + 2;
		if (num5 < num8)
		{
			return false;
		}
		microphoneClip.GetData(rawMicrophoneData, 0);
		bool flag = false;
		lock (audioClipsLock)
		{
			for (int i = 0; i < audioClips.Count; i++)
			{
				if (!audioClips[i].IsPaused && audioClips[i].MuteMicrophone)
				{
					flag = true;
					break;
				}
			}
		}
		float num9 = (float)num4 + resamplePointer;
		for (int j = 0; j < buffer.Length; j += Channels)
		{
			float num10 = 0f;
			float num11 = 0f;
			int num12 = (int)num9 % samples;
			int num13 = (num12 + 1) % samples;
			float num14 = num9 - Mathf.Floor(num9);
			if (!muteMicrophone && !flag)
			{
				if (num2 == 1)
				{
					float num15 = rawMicrophoneData[num12];
					float num16 = rawMicrophoneData[num13];
					num10 = (num11 = Mathf.Lerp(num15, num16, num14) * gain);
				}
				else
				{
					int num17 = num12 * num2;
					int num18 = num13 * num2;
					float num19 = rawMicrophoneData[Mathf.Clamp(num17, 0, rawMicrophoneData.Length - 1)];
					float num20 = rawMicrophoneData[Mathf.Clamp(num17 + 1, 0, rawMicrophoneData.Length - 1)];
					float num21 = rawMicrophoneData[Mathf.Clamp(num18, 0, rawMicrophoneData.Length - 1)];
					float num22 = rawMicrophoneData[Mathf.Clamp(num18 + 1, 0, rawMicrophoneData.Length - 1)];
					num10 = Mathf.Lerp(num19, num21, num14) * gain;
					num11 = Mathf.Lerp(num20, num22, num14) * gain;
				}
			}
			microphoneBuffer[j] = num10;
			if (Channels > 1 && j + 1 < buffer.Length)
			{
				microphoneBuffer[j + 1] = num11;
			}
			num9 += num7;
		}
		if (!PostProcessClip)
		{
			foreach (Action<float[]> value in PostProcessors.Values)
			{
				value?.Invoke(microphoneBuffer);
			}
		}
		for (int k = 0; k < buffer.Length; k++)
		{
			buffer[k] = microphoneBuffer[k];
		}
		lock (audioClipsLock)
		{
			for (int num23 = audioClips.Count - 1; num23 >= 0; num23--)
			{
				Clip clip = audioClips[num23];
				if (!clip.IsPaused)
				{
					bool flag2 = false;
					for (int l = 0; l < buffer.Length; l += Channels)
					{
						int num24 = (int)clip.InternalPosition;
						int num25 = clip.Samples.Length / clip.Channels;
						if (num24 >= num25)
						{
							if (clip.Looping)
							{
								clip.InternalPosition = 0f;
								continue;
							}
							flag2 = true;
							break;
						}
						int num26 = num24 + 1;
						float num27 = 0f;
						float num28 = 0f;
						if (num26 >= num25)
						{
							if (clip.Looping)
							{
								clip.InternalPosition = 0f;
								if (clip.Channels == 1)
								{
									num28 = clip.Samples[num24] * clip.Volume;
									continue;
								}
								int num29 = num24 * clip.Channels;
								num27 = clip.Samples[num29] * clip.Volume;
								num28 = clip.Samples[num29 + 1] * clip.Volume;
								continue;
							}
							if (clip.Channels == 1)
							{
								num27 = (num28 = clip.Samples[num24] * clip.Volume);
							}
							else
							{
								int num30 = num24 * clip.Channels;
								num27 = clip.Samples[num30] * clip.Volume;
								num28 = clip.Samples[num30 + 1] * clip.Volume;
							}
							flag2 = true;
						}
						else
						{
							float num31 = clip.InternalPosition - (float)num24;
							if (clip.Channels == 1)
							{
								num27 = (num28 = Mathf.Lerp(clip.Samples[num24], clip.Samples[num26], num31) * clip.Volume);
							}
							else
							{
								int num32 = num24 * clip.Channels;
								int num33 = num26 * clip.Channels;
								float num34 = clip.Samples[num32];
								float num35 = clip.Samples[num32 + 1];
								float num36 = clip.Samples[num33];
								float num37 = clip.Samples[num33 + 1];
								num27 = Mathf.Lerp(num34, num36, num31) * clip.Volume;
								num28 = Mathf.Lerp(num35, num37, num31) * clip.Volume;
							}
							clip.InternalPosition += Mathf.Max(0.0001f, clip.Step * clip.Speed);
						}
						buffer[l] += num27;
						if (Channels > 1 && l + 1 < buffer.Length)
						{
							buffer[l + 1] += num28;
						}
						if (!flag2)
						{
							continue;
						}
						break;
					}
					if (flag2 && !clip.Looping)
					{
						audioClips.RemoveAt(num23);
					}
				}
			}
		}
		if (PostProcessClip)
		{
			foreach (Action<float[]> value2 in PostProcessors.Values)
			{
				value2?.Invoke(buffer);
			}
		}
		for (int m = 0; m < buffer.Length; m++)
		{
			buffer[m] = Mathf.Clamp(buffer[m], -1f, 1f);
		}
		int num38 = Mathf.FloorToInt(num9) - num4;
		lastSamplePosition = (num4 + num38) % samples;
		resamplePointer = num9 - Mathf.Floor(num9);
		return true;
	}

	public void Dispose()
	{
		StopRecording();
		StopAudioClips();
		if (Instance == this)
		{
			Instance = null;
		}
	}
}
