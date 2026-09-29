using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Swimming;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.Rendering;
using GorillaTagScripts;
using GorillaTagScripts.Builder;
using Ionic.Zlib;
using POpusCodec.Enums;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice;
using Photon.Voice.Unity;
using Photon.Voice.Unity.UtilityScripts;
using PlayFab;
using PlayFab.ClientModels;
using Seralyth.Classes.Menu;
using Seralyth.Classes.Mods;
using Seralyth.Extensions;
using Seralyth.Managers;
using Seralyth.Menu;
using Seralyth.Patches.Menu;
using Seralyth.Utilities;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

namespace Seralyth.Mods;

public static class Fun
{
	public class MicPitchShifter : VoiceComponent
	{
		public class PitchProcessor : IProcessor<float>, IDisposable
		{
			private readonly float pitch;

			public PitchProcessor(float pitchFactor)
			{
				pitch = Mathf.Clamp(pitchFactor, 0.5f, 2f);
			}

			public float[] Process(float[] buf)
			{
				int num = buf.Length;
				float[] array = new float[num];
				float num2 = 0f;
				for (int i = 0; i < num; i++)
				{
					int num3 = Mathf.FloorToInt(num2);
					int num4 = Mathf.Min(num3 + 1, num - 1);
					float num5 = num2 - (float)num3;
					float num6 = Mathf.Lerp(buf[num3], buf[num4], num5);
					array[i] = num6;
					num2 += pitch;
					if (num2 >= (float)(num - 1))
					{
						break;
					}
				}
				return array;
			}

			public void Dispose()
			{
			}
		}

		public float PitchFactor = 1.5f;

		public PitchProcessor floatProcessor;

		public void PhotonVoiceCreated(PhotonVoiceCreatedParams p)
		{
			LocalVoice voice = p.Voice;
			LocalVoiceAudioFloat val = (LocalVoiceAudioFloat)(object)((voice is LocalVoiceAudioFloat) ? voice : null);
			if (val != null)
			{
				floatProcessor = new PitchProcessor(PitchFactor);
				((LocalVoiceFramed<float>)(object)val).AddPostProcessor(new IProcessor<float>[1] { floatProcessor });
			}
		}
	}

	public class LoopbackFactory : IAudioReader<float>, IDataReader<float>, IDisposable, IAudioDesc
	{
		private readonly Queue<float> buffer = new Queue<float>();

		public int SamplingRate => 16000;

		public int Channels => 1;

		public string Error => null;

		public void Feed(float[] data)
		{
			foreach (float item in data)
			{
				buffer.Enqueue(item);
			}
		}

		public bool Read(float[] bufferOut)
		{
			if (buffer.Count < bufferOut.Length)
			{
				return false;
			}
			for (int i = 0; i < bufferOut.Length; i++)
			{
				bufferOut[i] = buffer.Dequeue();
			}
			return true;
		}

		public void Dispose()
		{
			buffer.Clear();
		}
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Func<Player, int> _003C_003E9__77_1;

		public static Func<object[], bool> _003C_003E9__97_0;

		public static Action<AudioClip> _003C_003E9__112_0;

		public static Func<GorillaPlayerScoreboardLine, bool> _003C_003E9__127_0;

		public static Func<GorillaPlayerScoreboardLine, bool> _003C_003E9__128_0;

		public static Func<GorillaPlayerScoreboardLine, bool> _003C_003E9__132_0;

		public static Func<GorillaPlayerScoreboardLine, Transform> _003C_003E9__132_1;

		public static Func<GorillaPlayerScoreboardLine, Transform> _003C_003E9__133_1;

		public static Func<Player, int> _003C_003E9__134_2;

		public static Func<bool> _003C_003E9__134_0;

		public static Action _003C_003E9__138_4;

		public static Func<KeyValuePair<string, string>, int, KeyValuePair<string, string>> _003C_003E9__138_2;

		public static Func<KeyValuePair<string, string>, int, ButtonInfo> _003C_003E9__138_3;

		public static Action _003C_003E9__138_0;

		public static Func<string, string> _003C_003E9__138_8;

		public static Action _003C_003E9__138_7;

		public static Action _003C_003E9__138_1;

		public static Func<GRBadge, GameEntity> _003C_003E9__183_0;

		public static Func<GameEntity, bool> _003C_003E9__183_1;

		public static Action<float[]> _003C_003E9__228_0;

		public static Func<IAudioDesc> _003C_003E9__234_0;

		public static Action<AudioClip> _003C_003E9__236_0;

		public static DictationResultDelegate _003C_003E9__238_2;

		public static DictationCompletedDelegate _003C_003E9__238_3;

		public static DictationHypothesisDelegate _003C_003E9__238_5;

		public static Func<List<BuilderPiece>, IEnumerable<BuilderPiece>> _003C_003E9__259_0;

		public static Func<VRRig, bool> _003C_003E9__269_0;

		public static Func<SnowballThrowable, bool> _003C_003E9__271_1;

		public static Func<bool> _003C_003E9__271_0;

		public static Func<VRRig, bool> _003C_003E9__296_0;

		public static Func<VRRig, bool> _003C_003E9__296_2;

		public static Func<_003C_003Ef__AnonymousType14<VRRig, Vector3, float>, float> _003C_003E9__296_5;

		public static Func<_003C_003Ef__AnonymousType14<VRRig, Vector3, float>, VRRig> _003C_003E9__296_6;

		public static Action<AudioClip> _003C_003E9__298_0;

		public static Func<ThrowableBug, bool> _003C_003E9__309_0;

		public static Func<PhotonView, bool> _003C_003E9__329_1;

		public static Func<bool> _003C_003E9__329_0;

		public static Func<BuilderPiece, bool> _003C_003E9__349_0;

		public static Func<BuilderPiece, bool> _003C_003E9__349_1;

		public static Func<GorillaPlayerScoreboardLine, bool> _003C_003E9__373_0;

		public static Func<BuilderPiece, bool> _003C_003E9__395_0;

		public static Func<BuilderPiece, bool> _003C_003E9__395_2;

		public static Func<BuilderPiece, bool> _003C_003E9__395_3;

		public static Func<BuilderPiece, bool> _003C_003E9__395_4;

		public static Func<BuilderPiece, float> _003C_003E9__395_5;

		public static Func<BuilderPiece, bool> _003C_003E9__395_6;

		public static Func<BuilderPiece, bool> _003C_003E9__395_7;

		public static Func<BuilderPiece, bool> _003C_003E9__395_8;

		public static Func<BuilderPiece, bool> _003C_003E9__395_9;

		public static Func<BuilderPiece, float> _003C_003E9__395_10;

		public static Func<BuilderDropZone, bool> _003C_003E9__399_0;

		public static Func<BuilderDropZone, bool> _003C_003E9__399_1;

		public static Func<BuilderDropZone, float> _003C_003E9__399_2;

		public static Func<BuilderPiece, bool> _003C_003E9__427_0;

		public static Func<BuilderPiece, bool> _003C_003E9__428_0;

		public static Func<BuilderPiece, bool> _003C_003E9__448_0;

		public static Func<BuilderPiece, bool> _003C_003E9__448_1;

		public static Func<BuilderPiece, bool> _003C_003E9__448_2;

		public static Func<BuilderPiece, bool> _003C_003E9__448_3;

		public static Func<BuilderPiece, bool> _003C_003E9__448_4;

		public static Func<BuilderPiece, float> _003C_003E9__448_5;

		public static Func<BuilderPiece, bool> _003C_003E9__448_6;

		public static Func<BuilderPiece, bool> _003C_003E9__448_7;

		public static Func<BuilderPiece, bool> _003C_003E9__449_0;

		public static Func<BuilderPiece, bool> _003C_003E9__449_1;

		public static Func<BuilderPiece, bool> _003C_003E9__449_2;

		public static Func<BuilderPiece, bool> _003C_003E9__449_3;

		public static Func<BuilderPiece, bool> _003C_003E9__449_4;

		public static Func<BuilderPiece, bool> _003C_003E9__449_5;

		public static Func<BuilderPiece, float> _003C_003E9__449_6;

		public static Func<BuilderPiece, bool> _003C_003E9__449_7;

		public static Func<BuilderPiece, bool> _003C_003E9__449_8;

		public static Func<CosmeticItem, bool> _003C_003E9__496_0;

		public static Func<CosmeticItem, bool> _003C_003E9__498_0;

		public static Func<CosmeticItem, bool> _003C_003E9__499_0;

		public static Func<CosmeticItem, bool> _003C_003E9__500_0;

		public static Func<CosmeticItem, bool> _003C_003E9__519_0;

		public static Func<CosmeticItem, bool> _003C_003E9__521_0;

		public static Func<CosmeticItem, string> _003C_003E9__521_1;

		public static Func<CosmeticItem, bool> _003C_003E9__525_0;

		public static Func<VRRig, string> _003C_003E9__528_0;

		public static Func<VRRig, string> _003C_003E9__529_0;

		public static Func<VRRig, string> _003C_003E9__530_0;

		public static Func<VRRig, string> _003C_003E9__546_0;

		public static Func<string, bool> _003C_003E9__546_1;

		public static Func<VRRig, string> _003C_003E9__547_0;

		public static Func<string, bool> _003C_003E9__547_1;

		public static Func<VRRig, string> _003C_003E9__548_0;

		public static Func<string, bool> _003C_003E9__548_1;

		public static Func<VRRig, string> _003C_003E9__551_0;

		public static Func<string, bool> _003C_003E9__551_1;

		public static Action<string> _003C_003E9__552_2;

		public static Func<VRRig, string> _003C_003E9__552_0;

		public static Func<string, bool> _003C_003E9__552_1;

		public static Func<VRRig, bool> _003C_003E9__553_0;

		public static Func<VRRig, bool> _003C_003E9__553_1;

		public static Action<string> _003C_003E9__553_4;

		public static Func<VRRig, string> _003C_003E9__553_2;

		public static Func<string, bool> _003C_003E9__553_3;

		public static Action<string> _003C_003E9__554_0;

		public static Func<Player, int> _003C_003E9__557_0;

		internal int _003CInstantParty_003Eb__77_1(Player player)
		{
			return player.ActorNumber;
		}

		internal bool _003CKeyboardTracker_003Eb__97_0(object[] keylog)
		{
			return Time.time > (float)keylog[2];
		}

		internal void _003CJumpscareCoroutine_003Eb__112_0(AudioClip clip)
		{
			clip.Play();
		}

		internal bool _003CMuteAll_003Eb__127_0(GorillaPlayerScoreboardLine line)
		{
			return !line.muteButton.isAutoOn;
		}

		internal bool _003CUnmuteAll_003Eb__128_0(GorillaPlayerScoreboardLine line)
		{
			return line.muteButton.isAutoOn;
		}

		internal bool _003CTriggerAntiReportGun_003Eb__132_0(GorillaPlayerScoreboardLine line)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			return line.linePlayer == RigUtilities.GetPlayerFromVRRig(Main.lockTarget) && Vector3.Distance(((Component)line.reportButton).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) < 50f;
		}

		internal Transform _003CTriggerAntiReportGun_003Eb__132_1(GorillaPlayerScoreboardLine line)
		{
			return ((Component)line.reportButton).gameObject.transform;
		}

		internal Transform _003CTriggerAntiReportAll_003Eb__133_1(GorillaPlayerScoreboardLine line)
		{
			return ((Component)line.reportButton).gameObject.transform;
		}

		internal bool _003CBypassAntiReport_003Eb__134_0()
		{
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Expected O, but got Unknown
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Expected O, but got Unknown
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass134_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass134_0();
			bool flag = false;
			CS_0024_003C_003E8__locals4.people = new List<int>();
			try
			{
				foreach (GorillaPlayerScoreboardLine allScoreboardLine in GorillaScoreboardTotalUpdater.allScoreboardLines)
				{
					Transform transform = ((Component)allScoreboardLine.reportButton).gameObject.transform;
					float num = Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, transform.position);
					float num2 = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, transform.position);
					if (num < 0.5f || num2 < 0.5f)
					{
						CS_0024_003C_003E8__locals4.people.Add(allScoreboardLine.linePlayer.ActorNumber);
						flag = true;
					}
				}
			}
			catch
			{
			}
			if ((Object)(object)GorillaTagger.Instance.myVRRig != (Object)null && flag)
			{
				Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
				Vector3 position = ((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position;
				Vector3 position2 = ((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position;
				Main.SendSerialize(VRRig.LocalRig.GetPhotonView(), new RaiseEventOptions
				{
					TargetActors = (from player in PhotonNetwork.PlayerListOthers
						where !Extensions.Contains(CS_0024_003C_003E8__locals4.people.ToArray(), player.ActorNumber)
						select player.ActorNumber).ToArray()
				});
				((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position - ((Component)GorillaTagger.Instance.headCollider).transform.forward * 100f;
				((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position - ((Component)GorillaTagger.Instance.headCollider).transform.forward * 100f;
				Main.SendSerialize(VRRig.LocalRig.GetPhotonView(), new RaiseEventOptions
				{
					TargetActors = CS_0024_003C_003E8__locals4.people.ToArray()
				});
				((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = position;
				((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = position2;
				Main.RPCProtection();
				return false;
			}
			return true;
		}

		internal int _003CBypassAntiReport_003Eb__134_2(Player player)
		{
			return player.ActorNumber;
		}

		internal void _003CCustomModSpoofer_003Eb__138_0()
		{
			List<ButtonInfo> list = new List<ButtonInfo>
			{
				new ButtonInfo
				{
					buttonText = "Exit Mod List",
					method = delegate
					{
						Buttons.CurrentCategoryName = "Main";
					},
					isTogglable = false,
					toolTip = "Returns you back to the main page."
				}
			};
			list.AddRange(Visuals.modDictionary.Select((KeyValuePair<string, string> t, int i) => Visuals.modDictionary.ElementAt(i)).Select(delegate(KeyValuePair<string, string> mod, int i)
			{
				_003C_003Ec__DisplayClass138_0 CS_0024_003C_003E8__locals6 = new _003C_003Ec__DisplayClass138_0
				{
					mod = mod
				};
				return new ButtonInfo
				{
					buttonText = $"Mod{i}",
					overlapText = CS_0024_003C_003E8__locals6.mod.Value,
					enableMethod = delegate
					{
						ReloadModsToSpoof(CS_0024_003C_003E8__locals6.mod.Key, CS_0024_003C_003E8__locals6.mod.Value);
					},
					disableMethod = delegate
					{
						ReloadModsToSpoof(CS_0024_003C_003E8__locals6.mod.Key, CS_0024_003C_003E8__locals6.mod.Value, add: false);
					},
					toolTip = "Show that you are using the mod " + CS_0024_003C_003E8__locals6.mod.Value + " to other players."
				};
			}));
			Buttons.buttons[Buttons.GetCategory("Mod List")] = list.ToArray();
			Buttons.CurrentCategoryName = "Mod List";
		}

		internal void _003CCustomModSpoofer_003Eb__138_4()
		{
			Buttons.CurrentCategoryName = "Main";
		}

		internal KeyValuePair<string, string> _003CCustomModSpoofer_003Eb__138_2(KeyValuePair<string, string> t, int i)
		{
			return Visuals.modDictionary.ElementAt(i);
		}

		internal ButtonInfo _003CCustomModSpoofer_003Eb__138_3(KeyValuePair<string, string> mod, int i)
		{
			_003C_003Ec__DisplayClass138_0 CS_0024_003C_003E8__locals6 = new _003C_003Ec__DisplayClass138_0
			{
				mod = mod
			};
			return new ButtonInfo
			{
				buttonText = $"Mod{i}",
				overlapText = CS_0024_003C_003E8__locals6.mod.Value,
				enableMethod = delegate
				{
					ReloadModsToSpoof(CS_0024_003C_003E8__locals6.mod.Key, CS_0024_003C_003E8__locals6.mod.Value);
				},
				disableMethod = delegate
				{
					ReloadModsToSpoof(CS_0024_003C_003E8__locals6.mod.Key, CS_0024_003C_003E8__locals6.mod.Value, add: false);
				},
				toolTip = "Show that you are using the mod " + CS_0024_003C_003E8__locals6.mod.Value + " to other players."
			};
		}

		internal void _003CCustomModSpoofer_003Eb__138_1()
		{
			Main.PromptSingleText("Please enter what you would like to spoof your mods to (seperated by commas).", delegate
			{
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Expected O, but got Unknown
				string[] array = (from s in Main.keyboardInput.Split(',')
					select s.Trim()).ToArray();
				Hashtable val = new Hashtable();
				string[] array2 = array;
				foreach (string text in array2)
				{
					string text2 = null;
					foreach (KeyValuePair<string, string> item in Visuals.modDictionary)
					{
						if (string.Equals(item.Value, text, StringComparison.OrdinalIgnoreCase))
						{
							text2 = item.Key;
							break;
						}
					}
					val[(object)(text2 ?? text)] = true;
				}
				PhotonNetwork.LocalPlayer.SetCustomProperties(val, (Hashtable)null, (WebFlags)null);
			}, "Done");
		}

		internal void _003CCustomModSpoofer_003Eb__138_7()
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Expected O, but got Unknown
			string[] array = (from s in Main.keyboardInput.Split(',')
				select s.Trim()).ToArray();
			Hashtable val = new Hashtable();
			string[] array2 = array;
			foreach (string text in array2)
			{
				string text2 = null;
				foreach (KeyValuePair<string, string> item in Visuals.modDictionary)
				{
					if (string.Equals(item.Value, text, StringComparison.OrdinalIgnoreCase))
					{
						text2 = item.Key;
						break;
					}
				}
				val[(object)(text2 ?? text)] = true;
			}
			PhotonNetwork.LocalPlayer.SetCustomProperties(val, (Hashtable)null, (WebFlags)null);
		}

		internal string _003CCustomModSpoofer_003Eb__138_8(string s)
		{
			return s.Trim();
		}

		internal GameEntity _003CGrabIDCard_003Eb__183_0(GRBadge grBadge)
		{
			return grBadge.gameEntity;
		}

		internal bool _003CGrabIDCard_003Eb__183_1(GameEntity entity)
		{
			return entity.onlyGrabActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		}

		internal void _003CLaggyMicrophone_003Eb__228_0(float[] buffer)
		{
			if (Random.value < 0.25f)
			{
				Array.Clear(buffer, 0, buffer.Length);
			}
		}

		internal IAudioDesc _003CCopyVoiceGun_003Eb__234_0()
		{
			return (IAudioDesc)(object)factory;
		}

		internal void _003CSaveNarration_003Eb__236_0(AudioClip audio)
		{
			Main.PromptSingleText("The narration has been saved in your Soundboard!");
		}

		internal void _003CMaskVoice_003Eb__238_2(string text, ConfidenceLevel confidence)
		{
			if (Settings.debugDictation)
			{
				LogManager.Log("Dictation result: " + text);
			}
			NotificationManager.SendNotification("<color=grey>[</color><color=green>VOICE</color><color=grey>]</color> " + text);
			if ((Object)(object)NetworkSystem.Instance.VoiceConnection.PrimaryRecorder != (Object)null)
			{
				NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.IsRecording = true;
				if (PhotonNetwork.InRoom)
				{
					Main.SpeakText(text, disableMicrophone: true);
				}
				else
				{
					Main.NarrateText(text);
				}
			}
			else
			{
				Main.NarrateText(text);
			}
		}

		internal void _003CMaskVoice_003Eb__238_3(DictationCompletionCause completionCause)
		{
			drec.Start();
		}

		internal void _003CMaskVoice_003Eb__238_5(string text)
		{
			if (Settings.debugDictation)
			{
				LogManager.Log("Hypothesis: " + text);
			}
			NotificationManager.ClearAllNotifications();
			NotificationManager.SendNotification("<color=grey>[</color><color=green>VOICE</color><color=grey>]</color> " + text);
		}

		internal IEnumerable<BuilderPiece> _003CGetAllBlockData_003Eb__259_0(List<BuilderPiece> list)
		{
			return list;
		}

		internal bool _003CProjectileRange_003Eb__269_0(VRRig rig)
		{
			return !rig.IsLocal();
		}

		internal bool _003CHookProjectileColors_003Eb__271_0()
		{
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			if (PhotonNetwork.InRoom)
			{
				Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
				List<SnowballThrowable> list = new List<SnowballThrowable>();
				SnowballMaker[] array = (SnowballMaker[])(object)new SnowballMaker[2]
				{
					SnowballMaker.leftHandInstance,
					SnowballMaker.rightHandInstance
				};
				foreach (SnowballMaker val in array)
				{
					list.AddRange(val.snowballs.Where((SnowballThrowable Throwable) => ((Component)Throwable).gameObject.activeSelf));
				}
				if (list.Count <= 0)
				{
					Main.SendSerialize(VRRig.LocalRig.GetPhotonView());
					return false;
				}
				foreach (SnowballThrowable item in list)
				{
					item.SetSnowballActiveLocal(false);
				}
				VRRig.LocalRig.reliableState.SetIsDirty();
				Main.SendSerialize(VRRig.LocalRig.GetPhotonView());
				foreach (SnowballThrowable item2 in list)
				{
					GrowingSnowballThrowable val2 = (GrowingSnowballThrowable)(object)((item2 is GrowingSnowballThrowable) ? item2 : null);
					if (val2 != null)
					{
						val2.maintainSizeLevelUntilLocalTime = Time.time;
					}
					item2.randomizeColor = true;
					VRRig.LocalRig.SetThrowableProjectileColor(((Object)((Component)item2).gameObject).name.ToLower().Contains("left"), Color32.op_Implicit(projHookColor));
					item2.SetSnowballActiveLocal(true);
					item2.ApplyColor(projHookColor);
				}
				VRRig.LocalRig.reliableState.SetIsDirty();
				Main.SendSerialize(VRRig.LocalRig.GetPhotonView());
				return false;
			}
			return true;
		}

		internal bool _003CHookProjectileColors_003Eb__271_1(SnowballThrowable Throwable)
		{
			return ((Component)Throwable).gameObject.activeSelf;
		}

		internal bool _003CDebugSlingshotAimbot_003Eb__296_0(VRRig rig)
		{
			return !rig.isLocal;
		}

		internal bool _003CDebugSlingshotAimbot_003Eb__296_2(VRRig rig)
		{
			return (Object)(object)rig != (Object)null;
		}

		internal float _003CDebugSlingshotAimbot_003Eb__296_5(_003C_003Ef__AnonymousType14<VRRig, Vector3, float> x)
		{
			return x.Distance;
		}

		internal VRRig _003CDebugSlingshotAimbot_003Eb__296_6(_003C_003Ef__AnonymousType14<VRRig, Vector3, float> x)
		{
			return x.Rig;
		}

		internal void _003CAngryBirdsSounds_003Eb__298_0(AudioClip clip)
		{
			clip.Play((float)Main.buttonClickVolume / 10f);
		}

		internal bool _003Cget_Firefly_003Eb__309_0(ThrowableBug bug)
		{
			return ((Component)bug).gameObject.activeInHierarchy && ((Object)((Component)bug).gameObject).name == "Floating Bug Holdable";
		}

		internal bool _003CEnableBugVibrateAll_003Eb__329_0()
		{
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0410: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Expected O, but got Unknown
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Expected O, but got Unknown
			ThrowableBug bug = GetBug("Floating Bug Holdable");
			ThrowableBug val = (((Object)(object)bug != (Object)null) ? GetBug("Firefly") : bug);
			object obj;
			if (bug == null)
			{
				obj = null;
			}
			else
			{
				ThrowableBugReliableState reliableState = bug.reliableState;
				obj = ((reliableState != null) ? ((Component)reliableState).gameObject : null);
			}
			PhotonView val2 = (((Object)obj != (Object)null) ? ((NetworkView)((Component)bug.reliableState).gameObject.GetComponent<GorillaNetworkTransform>()).punView : null);
			object obj2;
			if (val == null)
			{
				obj2 = null;
			}
			else
			{
				ThrowableBugReliableState reliableState2 = val.reliableState;
				obj2 = ((reliableState2 != null) ? ((Component)reliableState2).gameObject : null);
			}
			PhotonView val3 = (((Object)obj2 != (Object)null) ? ((NetworkView)((Component)val.reliableState).gameObject.GetComponent<GorillaNetworkTransform>()).punView : null);
			if ((Object)(object)val2 == (Object)null || (Object)(object)val3 == (Object)null)
			{
				return true;
			}
			Main.MassSerialize(exclude: true, ((IEnumerable<PhotonView>)(object)new PhotonView[2] { val2, val3 }).Where((PhotonView v) => (Object)(object)v != (Object)null).ToArray());
			GameObject gameObject = ((Component)bug.reliableState).gameObject;
			Vector3? obj3;
			if (gameObject == null)
			{
				obj3 = null;
			}
			else
			{
				Transform transform = gameObject.transform;
				obj3 = ((transform != null) ? new Vector3?(transform.position) : ((Vector3?)null));
			}
			Vector3 position = (Vector3)(((_003F?)obj3) ?? Vector3.zero);
			GameObject gameObject2 = ((Component)bug.reliableState).gameObject;
			Quaternion? obj4;
			if (gameObject2 == null)
			{
				obj4 = null;
			}
			else
			{
				Transform transform2 = gameObject2.transform;
				obj4 = ((transform2 != null) ? new Quaternion?(transform2.rotation) : ((Quaternion?)null));
			}
			Quaternion rotation = (Quaternion)(((_003F?)obj4) ?? Quaternion.identity);
			GameObject gameObject3 = ((Component)val.reliableState).gameObject;
			Vector3? obj5;
			if (gameObject3 == null)
			{
				obj5 = null;
			}
			else
			{
				Transform transform3 = gameObject3.transform;
				obj5 = ((transform3 != null) ? new Vector3?(transform3.position) : ((Vector3?)null));
			}
			Vector3 position2 = (Vector3)(((_003F?)obj5) ?? Vector3.zero);
			GameObject gameObject4 = ((Component)val.reliableState).gameObject;
			Quaternion? obj6;
			if (gameObject4 == null)
			{
				obj6 = null;
			}
			else
			{
				Transform transform4 = gameObject4.transform;
				obj6 = ((transform4 != null) ? new Quaternion?(transform4.rotation) : ((Quaternion?)null));
			}
			Quaternion rotation2 = (Quaternion)(((_003F?)obj6) ?? Quaternion.identity);
			NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
			foreach (NetPlayer val4 in playerListOthers)
			{
				VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(val4);
				if (!((Object)(object)vRRigFromPlayer == (Object)null))
				{
					if ((Object)(object)bug != (Object)null && (Object)(object)((Component)bug).transform != (Object)null && (Object)(object)vRRigFromPlayer.leftHandTransform != (Object)null && (Object)(object)val2 != (Object)null)
					{
						((Component)bug.reliableState).gameObject.transform.position = vRRigFromPlayer.leftHandTransform.position;
						((Component)bug.reliableState).gameObject.transform.rotation = RandomUtilities.RandomQuaternion();
						RaiseEventOptions val5 = new RaiseEventOptions();
						val5.TargetActors = new int[1] { val4.ActorNumber };
						Main.SendSerialize(val2, val5);
					}
					if ((Object)(object)val != (Object)null && (Object)(object)((Component)val).transform != (Object)null && (Object)(object)vRRigFromPlayer.rightHandTransform != (Object)null && (Object)(object)val3 != (Object)null)
					{
						((Component)val.reliableState).gameObject.transform.position = vRRigFromPlayer.rightHandTransform.position;
						((Component)val.reliableState).gameObject.transform.rotation = RandomUtilities.RandomQuaternion();
						RaiseEventOptions val5 = new RaiseEventOptions();
						val5.TargetActors = new int[1] { val4.ActorNumber };
						Main.SendSerialize(val3, val5);
					}
				}
			}
			if ((Object)(object)((bug != null) ? ((Component)bug).transform : null) != (Object)null)
			{
				((Component)bug.reliableState).gameObject.transform.position = position;
				((Component)bug.reliableState).gameObject.transform.rotation = rotation;
			}
			if ((Object)(object)((val != null) ? ((Component)val).transform : null) != (Object)null)
			{
				((Component)val.reliableState).gameObject.transform.position = position2;
				((Component)val.reliableState).gameObject.transform.rotation = rotation2;
			}
			Main.RPCProtection();
			return false;
		}

		internal bool _003CEnableBugVibrateAll_003Eb__329_1(PhotonView v)
		{
			return (Object)(object)v != (Object)null;
		}

		internal bool _003CDestroyBlocks_003Eb__349_0(BuilderPiece piece)
		{
			return ((Component)piece).gameObject.activeInHierarchy;
		}

		internal bool _003CDestroyBlocks_003Eb__349_1(BuilderPiece piece)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			return Vector3.Distance(((Component)piece).transform.position, GorillaTagger.Instance.leftHandTransform.position) < 2.5f;
		}

		internal bool _003CAtticAntiReport_003Eb__373_0(GorillaPlayerScoreboardLine line)
		{
			return line.linePlayer == NetworkSystem.Instance.LocalPlayer;
		}

		internal bool _003CRequestCreatePiece_003Eb__395_0(BuilderPiece piece)
		{
			return ((Component)piece).gameObject.activeInHierarchy;
		}

		internal bool _003CRequestCreatePiece_003Eb__395_2(BuilderPiece piece)
		{
			return !piece.isBuiltIntoTable;
		}

		internal bool _003CRequestCreatePiece_003Eb__395_3(BuilderPiece piece)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			return piece.CanPlayerGrabPiece(PhotonNetwork.LocalPlayer.ActorNumber, ((Component)piece).transform.position);
		}

		internal bool _003CRequestCreatePiece_003Eb__395_4(BuilderPiece piece)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return Vector3.Distance(((Component)piece).transform.position, Main.ServerLeftHandPos) < 2.5f;
		}

		internal float _003CRequestCreatePiece_003Eb__395_5(BuilderPiece piece)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return Vector3.Distance(((Component)piece).transform.position, Main.ServerLeftHandPos);
		}

		internal bool _003CRequestCreatePiece_003Eb__395_6(BuilderPiece piece)
		{
			return ((Component)piece).gameObject.activeInHierarchy;
		}

		internal bool _003CRequestCreatePiece_003Eb__395_7(BuilderPiece piece)
		{
			return !piece.isBuiltIntoTable;
		}

		internal bool _003CRequestCreatePiece_003Eb__395_8(BuilderPiece piece)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			return piece.CanPlayerGrabPiece(PhotonNetwork.LocalPlayer.ActorNumber, ((Component)piece).transform.position);
		}

		internal bool _003CRequestCreatePiece_003Eb__395_9(BuilderPiece piece)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return Vector3.Distance(((Component)piece).transform.position, Main.ServerLeftHandPos) < 2.5f;
		}

		internal float _003CRequestCreatePiece_003Eb__395_10(BuilderPiece piece)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return Vector3.Distance(((Component)piece).transform.position, Main.ServerLeftHandPos);
		}

		internal bool _003CRequestRecyclePiece_003Eb__399_0(BuilderDropZone zone)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			return (int)zone.dropType >= 1;
		}

		internal bool _003CRequestRecyclePiece_003Eb__399_1(BuilderDropZone zone)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return Vector3.Distance(((Component)zone).transform.position, Main.ServerLeftHandPos) < 2.5f;
		}

		internal float _003CRequestRecyclePiece_003Eb__399_2(BuilderDropZone zone)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return Vector3.Distance(((Component)zone).transform.position, Main.ServerLeftHandPos);
		}

		internal bool _003CNoclipBuilding_003Eb__427_0(BuilderPiece block)
		{
			return ((Component)block).gameObject.activeInHierarchy && !block.isBuiltIntoTable;
		}

		internal bool _003CDisableNoclipBuilding_003Eb__428_0(BuilderPiece block)
		{
			return ((Component)block).gameObject.activeInHierarchy && !block.isBuiltIntoTable;
		}

		internal bool _003CGrabAllBlocksNearby_003Eb__448_0(BuilderPiece piece)
		{
			return ((Component)piece).gameObject.activeInHierarchy;
		}

		internal bool _003CGrabAllBlocksNearby_003Eb__448_1(BuilderPiece piece)
		{
			return !piece.isBuiltIntoTable;
		}

		internal bool _003CGrabAllBlocksNearby_003Eb__448_2(BuilderPiece piece)
		{
			return piece.heldByPlayerActorNumber != PhotonNetwork.LocalPlayer.ActorNumber;
		}

		internal bool _003CGrabAllBlocksNearby_003Eb__448_3(BuilderPiece piece)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			return piece.CanPlayerGrabPiece(PhotonNetwork.LocalPlayer.ActorNumber, ((Component)piece).transform.position);
		}

		internal bool _003CGrabAllBlocksNearby_003Eb__448_4(BuilderPiece piece)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return Vector3.Distance(((Component)piece).transform.position, Main.ServerLeftHandPos) < 2.5f;
		}

		internal float _003CGrabAllBlocksNearby_003Eb__448_5(BuilderPiece piece)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return Vector3.Distance(((Component)piece).transform.position, Main.ServerLeftHandPos);
		}

		internal bool _003CGrabAllBlocksNearby_003Eb__448_6(BuilderPiece piece)
		{
			return ((Component)piece).gameObject.activeInHierarchy;
		}

		internal bool _003CGrabAllBlocksNearby_003Eb__448_7(BuilderPiece piece)
		{
			return piece.heldByPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		}

		internal bool _003CGrabAllSelectedNearby_003Eb__449_0(BuilderPiece piece)
		{
			return ((Component)piece).gameObject.activeInHierarchy;
		}

		internal bool _003CGrabAllSelectedNearby_003Eb__449_1(BuilderPiece piece)
		{
			return piece.pieceType == pieceIdSet;
		}

		internal bool _003CGrabAllSelectedNearby_003Eb__449_2(BuilderPiece piece)
		{
			return !piece.isBuiltIntoTable;
		}

		internal bool _003CGrabAllSelectedNearby_003Eb__449_3(BuilderPiece piece)
		{
			return piece.heldByPlayerActorNumber != PhotonNetwork.LocalPlayer.ActorNumber;
		}

		internal bool _003CGrabAllSelectedNearby_003Eb__449_4(BuilderPiece piece)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			return piece.CanPlayerGrabPiece(PhotonNetwork.LocalPlayer.ActorNumber, ((Component)piece).transform.position);
		}

		internal bool _003CGrabAllSelectedNearby_003Eb__449_5(BuilderPiece piece)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return Vector3.Distance(((Component)piece).transform.position, Main.ServerLeftHandPos) < 2.5f;
		}

		internal float _003CGrabAllSelectedNearby_003Eb__449_6(BuilderPiece piece)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return Vector3.Distance(((Component)piece).transform.position, Main.ServerLeftHandPos);
		}

		internal bool _003CGrabAllSelectedNearby_003Eb__449_7(BuilderPiece piece)
		{
			return ((Component)piece).gameObject.activeInHierarchy;
		}

		internal bool _003CGrabAllSelectedNearby_003Eb__449_8(BuilderPiece piece)
		{
			return piece.heldByPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
		}

		internal bool _003CGetOwnedCosmetics_003Eb__496_0(CosmeticItem cosmeticItem)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return VRRig.LocalRig._playerOwnedCosmetics.Contains(cosmeticItem.itemName);
		}

		internal bool _003CGetTryOnCosmetics_003Eb__498_0(CosmeticItem cosmeticItem)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return cosmeticItem.canTryOn;
		}

		internal bool _003CGetTryOnBalloons_003Eb__499_0(CosmeticItem cosmeticItem)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			return cosmeticItem.canTryOn && cosmeticItem.overrideDisplayName.ToLower().Contains("balloon");
		}

		internal bool _003CGetOwnedBalloons_003Eb__500_0(CosmeticItem cosmeticItem)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			return VRRig.LocalRig._playerOwnedCosmetics.Contains(cosmeticItem.itemName) && cosmeticItem.overrideDisplayName.ToLower().Contains("balloon");
		}

		internal bool _003CAutoPurchaseCosmetics_003Eb__519_0(CosmeticItem hat)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			return hat.cost == 0 && hat.canTryOn && !Main.CosmeticsOwned.Contains(hat.itemName);
		}

		internal bool _003CAutoPurchasePaidCosmetics_003Eb__521_0(CosmeticItem i)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			return (int)i.itemCategory == 1;
		}

		internal string _003CAutoPurchasePaidCosmetics_003Eb__521_1(CosmeticItem i)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return ((int)i.itemCategory == 1) ? i.itemName : null;
		}

		internal bool _003CUnlockAllCosmetics_003Eb__525_0(CosmeticItem item)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return !((CosmeticsController)CosmeticsController.instance).concatStringCosmeticsAllowed.Contains(item.itemName);
		}

		internal string _003CCopyIDAura_003Eb__528_0(VRRig nearbyPlayer)
		{
			return RigUtilities.GetPlayerFromVRRig(nearbyPlayer).UserId;
		}

		internal string _003CCopyIDOnTouch_003Eb__529_0(VRRig rig)
		{
			return RigUtilities.GetPlayerFromVRRig(rig).UserId;
		}

		internal string _003CCopyIDAll_003Eb__530_0(VRRig vrrig)
		{
			return RigUtilities.GetPlayerFromVRRig(vrrig).UserId;
		}

		internal string _003CCopyCreationDateAura_003Eb__546_0(VRRig nearbyPlayer)
		{
			return RigUtilities.GetCreationDate(RigUtilities.GetPlayerFromVRRig(nearbyPlayer).UserId, CopyCreationDate);
		}

		internal bool _003CCopyCreationDateAura_003Eb__546_1(string date)
		{
			return date != "Loading...";
		}

		internal string _003CCopyCreationDateOnTouch_003Eb__547_0(VRRig rig)
		{
			return RigUtilities.GetCreationDate(RigUtilities.GetPlayerFromVRRig(rig).UserId, CopyCreationDate);
		}

		internal bool _003CCopyCreationDateOnTouch_003Eb__547_1(string date)
		{
			return date != "Loading...";
		}

		internal string _003CCopyCreationDateAll_003Eb__548_0(VRRig vrrig)
		{
			return RigUtilities.GetCreationDate(RigUtilities.GetPlayerFromVRRig(vrrig).UserId, CopyCreationDate);
		}

		internal bool _003CCopyCreationDateAll_003Eb__548_1(string date)
		{
			return date != "Loading...";
		}

		internal string _003CNarrateCreationDateAll_003Eb__551_0(VRRig vrrig)
		{
			return RigUtilities.GetCreationDate(RigUtilities.GetPlayerFromVRRig(vrrig).UserId, Main.SpeakText);
		}

		internal bool _003CNarrateCreationDateAll_003Eb__551_1(string date)
		{
			return date != "Loading...";
		}

		internal string _003CNarrateCreationDateAura_003Eb__552_0(VRRig nearbyPlayer)
		{
			return RigUtilities.GetCreationDate(RigUtilities.GetPlayerFromVRRig(nearbyPlayer).UserId, delegate(string date)
			{
				Main.SpeakText(date);
			});
		}

		internal void _003CNarrateCreationDateAura_003Eb__552_2(string date)
		{
			Main.SpeakText(date);
		}

		internal bool _003CNarrateCreationDateAura_003Eb__552_1(string date)
		{
			return date != "Loading...";
		}

		internal bool _003CNarrateCreationDateOnTouch_003Eb__553_0(VRRig rig)
		{
			return !rig.IsLocal();
		}

		internal bool _003CNarrateCreationDateOnTouch_003Eb__553_1(VRRig rig)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			return Vector3.Distance(((Component)rig).transform.position, VRRig.LocalRig.rightHandTransform.position) <= 0.35f || Vector3.Distance(((Component)rig).transform.position, VRRig.LocalRig.leftHandTransform.position) <= 0.35f;
		}

		internal string _003CNarrateCreationDateOnTouch_003Eb__553_2(VRRig rig)
		{
			return RigUtilities.GetCreationDate(RigUtilities.GetPlayerFromVRRig(rig).UserId, delegate(string date)
			{
				Main.SpeakText(date);
			});
		}

		internal void _003CNarrateCreationDateOnTouch_003Eb__553_4(string date)
		{
			Main.SpeakText(date);
		}

		internal bool _003CNarrateCreationDateOnTouch_003Eb__553_3(string date)
		{
			return date != "Loading...";
		}

		internal void _003CNarrateCreationDateGun_003Eb__554_0(string date)
		{
			Main.SpeakText(date);
		}

		internal int _003CTrackEveryPlayer_003Eb__557_0(Player p)
		{
			return p.ActorNumber;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass134_0
	{
		public List<int> people;

		internal bool _003CBypassAntiReport_003Eb__1(Player player)
		{
			return !Extensions.Contains(people.ToArray(), player.ActorNumber);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass138_0
	{
		public KeyValuePair<string, string> mod;

		internal void _003CCustomModSpoofer_003Eb__5()
		{
			ReloadModsToSpoof(mod.Key, mod.Value);
		}

		internal void _003CCustomModSpoofer_003Eb__6()
		{
			ReloadModsToSpoof(mod.Key, mod.Value, add: false);
		}
	}

	private static float emoteTimer;

	private static int emoteStep;

	private static int emoteStyle;

	private static float touchFlingCooldown;

	private static bool touchFlingTriggered;

	private static Texture2D robuxTexture;

	private static GameObject trailObject;

	private static GameObject redTrailObject;

	private static GameObject blueTrailObject;

	private static GameObject greenTrailObject;

	private static GameObject yellowTrailObject;

	private static GameObject purpleTrailObject;

	private static GameObject orangeTrailObject;

	private static GameObject pinkTrailObject;

	private static GameObject cyanTrailObject;

	private static GameObject whiteTrailObject;

	private static List<GameObject> orbitzSpheres;

	private static List<GameObject> orbitzV2Spheres;

	private static List<GameObject> orbitzCubes;

	private static List<GameObject> orbitzCubesV2;

	private static float randomHeadDelay;

	public static float lastBangTime;

	public static readonly float BPM = 159f;

	public static float soundboardVolumeIndex = 1f;

	public static float soundboardSpeedIndex = 1f;

	public static int headSpinIndex;

	private static float headSpinSpeed = 10f;

	private static float instantPartyDelay;

	public static Coroutine waterSplashCoroutine;

	public static float splashDel;

	private static bool lastlhboop;

	private static bool lastrhboop;

	private static bool autoclickstate;

	public static readonly List<object[]> keyLogs = new List<object[]>();

	private static GameObject FreeCamObject;

	private static Vector3 CameraVelocity;

	public static float elapsedTime = Time.time;

	private static bool wasTagged;

	private static readonly string[] surpriseNarrations = new string[5] { "boo", "turn around", "you asked for this", "surprise", "behind you" };

	public static int targetFOV = 90;

	private static float muteDelay;

	private static float pressButtonDelay;

	public static readonly Dictionary<string, string> modsToSpoof = new Dictionary<string, string>();

	private static float tapDelay;

	private static float buttonDelay;

	private static float hitDelay;

	private static float moleMachineDelay;

	private static bool previousBraceletSpamState;

	private static float braceletSpamDelay;

	public static float isDirtyDelay;

	private static float lastTimeDingied;

	private static float delaybetweenscore;

	public static int targetQuestScore = 69;

	private static float spamDelay;

	private static bool returnOrTeleport;

	private static bool openOrClose;

	private static VirtualStumpAd virtualStumpAd;

	public static GameObject gunLibCheckpoint;

	private static bool previousGunLibCheckpointPrimary;

	private static float gunLibFlingV2Cooldown;

	private static bool gunLibFlingV2Initialized;

	private static float purchaseDelay;

	private static float killDelay;

	private static LoopbackFactory factory;

	private static float copyVoiceGunDelay;

	public static DictationRecognizer drec;

	public static Coroutine dropBoard;

	private static float hoverboardGunDelay;

	private static int pieceIdSet = -566818631;

	private static float blockDelay;

	private static float gbgd;

	private static Dictionary<int, string> blocks;

	public static Color projHookColor = Color.white;

	public static Coroutine DisableHoverboardCoroutine;

	private static float hoverboardSpamDelay;

	private static bool flashColor;

	private static float flashDelay;

	private static bool lastDrawing;

	public static int oldIndex = -1;

	public static LineRenderer paintbrawlTriggerLine;

	public static float triggerBotDelay;

	public static Coroutine BugCoroutine;

	public static ThrowableBug _firefly;

	public static float getOwnershipDelay;

	private static float bugSpamDelay;

	private static bool bugSpamToggle;

	private static float cameraSpamDelay;

	private static bool cameraSpamType;

	private static int objectIndex;

	private static float everythingSpamDelay;

	private static readonly Dictionary<string, bool> lastInAirValues = new Dictionary<string, bool>();

	private static bool grabbingCamera;

	private static bool grabbingHand;

	private static float delayer = -1f;

	public static Coroutine DisableThrowableCoroutine;

	private static float startTimeBuilding;

	private static float floatPower = 0.35f;

	public static Vector3 position = Vector3.zero;

	private static bool isFiring;

	public static int blockDebounceIndex = 2;

	public static float blockDebounce = 0.1f;

	private static float hoverboardAuraDelay;

	private static bool lastWasNull;

	private static float noclipBuildingDelay;

	public static int pieceId = -1;

	private static bool previousGripDown;

	private static bool previousTriggerDown;

	private static readonly List<BuilderPiece> potentialgrabbedpieces = new List<BuilderPiece>();

	public static float nameCycleDelay;

	public static int nameCycleIndex;

	public static int cycleSpeedIndex = 2;

	public static float nameCycleDebounce = 1f;

	public static string[] names = new string[0];

	public static string name;

	public static float colorChangerDelay;

	public static int colorChangeType;

	public static bool strobeColor;

	public static float stealIdentityDelay;

	private static float stealCosmeticsDelay;

	public static int accessoryType;

	public static int hat;

	public static bool lastHitL;

	public static bool lastHitR;

	public static bool lastHitLP;

	public static bool lastHitRP;

	public static bool lastHitRS;

	private static readonly Dictionary<string[], int[]> cachePacked = new Dictionary<string[], int[]>();

	private static List<string> ownedArchive;

	private static List<string> tryOnCosmetics;

	private static float delay;

	private static float delayonhold;

	private static int[] archiveCosmetics;

	private static int rememberdirectory;

	private static float lastTimeCosmeticsChecked;

	private static float lastTimePaidCosmeticsChecked;

	private static bool lasttagged;

	public static bool hasGivenCosmetics;

	private static float idgundelay;

	private static float allNarrationDelay;

	private static float creationDateDelay;

	private static float shootTimer = 0f;

	private static float shootDelay = 0.1f;

	private static float orbLifeTimer = -1f;

	private static int nextNotificationStep = 10;

	public static float strobeTimer = 0f;

	private static bool sending;

	public static string lastSentCode = "";

	private static float unmuteDelay;

	internal static object Bat;

	public static ThrowableBug Firefly
	{
		get
		{
			if ((Object)(object)_firefly == (Object)null)
			{
				_firefly = (from bug in Main.GetAllType<ThrowableBug>(5f)
					where ((Component)bug).gameObject.activeInHierarchy && ((Object)((Component)bug).gameObject).name == "Floating Bug Holdable"
					select bug).ToArray()[0];
			}
			return _firefly;
		}
	}

	public static void FixHead()
	{
		VRRig.LocalRig.head.trackingRotationOffset.x = 0f;
		VRRig.LocalRig.head.trackingRotationOffset.y = 0f;
		VRRig.LocalRig.head.trackingRotationOffset.z = 0f;
	}

	public static void AutoEmote()
	{
		if (!(Time.time < emoteTimer))
		{
			emoteStyle = (emoteStyle + 1) % 3;
			switch (emoteStyle)
			{
			case 0:
				VRRig.LocalRig.PlayHandTapLocal(50, true, 0.8f);
				VRRig.LocalRig.PlayHandTapLocal(50, false, 0.8f);
				emoteTimer = Time.time + 0.3f;
				break;
			case 1:
				VRRig.LocalRig.PlayHandTapLocal(42, true, 0.6f);
				VRRig.LocalRig.PlayHandTapLocal(42, false, 0.6f);
				emoteTimer = Time.time + 0.2f;
				break;
			case 2:
				VRRig.LocalRig.PlayHandTapLocal(36, true, 1f);
				VRRig.LocalRig.PlayHandTapLocal(36, false, 1f);
				emoteTimer = Time.time + 0.5f;
				break;
			}
			emoteStep++;
			if (emoteStep % 4 == 0)
			{
				emoteStyle = (emoteStyle + 1) % 3;
			}
		}
	}

	public static void AutoEmoteReset()
	{
		emoteTimer = 0f;
		emoteStep = 0;
		emoteStyle = 0;
	}

	public static void TouchFling()
	{
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time < touchFlingCooldown)
		{
			return;
		}
		Vector3 val2;
		if (Main.GetGunInput(isShooting: false))
		{
			(RaycastHit, GameObject) tuple = Main.RenderGun();
			if (Main.GetGunInput(isShooting: true) && !touchFlingTriggered)
			{
				Collider collider = ((RaycastHit)(ref tuple.Item1)).collider;
				VRRig val = ((collider != null) ? ((Component)collider).GetComponentInParent<VRRig>() : null);
				if ((Object)(object)val != (Object)null && !val.IsLocal())
				{
					val2 = ((Component)val).transform.position - ((Component)VRRig.LocalRig).transform.position;
					Vector3 val3 = ((Vector3)(ref val2)).normalized + Vector3.up * 3f;
					val.GetNetView().SendRPC("GrabbedByPlayer", RigUtilities.GetPlayerFromVRRig(val), new object[3] { true, false, false });
					val.GetNetView().SendRPC("DroppedByPlayer", RigUtilities.GetPlayerFromVRRig(val), new object[1] { val3 * 25f });
					Main.RPCProtection();
					touchFlingCooldown = Time.time + 0.3f;
				}
			}
			touchFlingTriggered = Main.GetGunInput(isShooting: true);
		}
		Vector3 val4 = GorillaTagger.Instance.rightHandTransform.position;
		Collider[] array = Physics.OverlapSphere(val4, 0.3f);
		Collider[] array2 = array;
		foreach (Collider val5 in array2)
		{
			VRRig componentInParent = ((Component)val5).GetComponentInParent<VRRig>();
			if ((Object)(object)componentInParent != (Object)null && !componentInParent.IsLocal())
			{
				val2 = ((Component)componentInParent).transform.position - ((Component)VRRig.LocalRig).transform.position;
				Vector3 val6 = ((Vector3)(ref val2)).normalized + Vector3.up * 3f;
				componentInParent.GetNetView().SendRPC("GrabbedByPlayer", RigUtilities.GetPlayerFromVRRig(componentInParent), new object[3] { true, false, false });
				componentInParent.GetNetView().SendRPC("DroppedByPlayer", RigUtilities.GetPlayerFromVRRig(componentInParent), new object[1] { val6 * 25f });
				Main.RPCProtection();
				touchFlingCooldown = Time.time + 0.3f;
				return;
			}
		}
		Vector3 val7 = GorillaTagger.Instance.leftHandTransform.position;
		Collider[] array3 = Physics.OverlapSphere(val7, 0.3f);
		Collider[] array4 = array3;
		foreach (Collider val8 in array4)
		{
			VRRig componentInParent2 = ((Component)val8).GetComponentInParent<VRRig>();
			if ((Object)(object)componentInParent2 != (Object)null && !componentInParent2.IsLocal())
			{
				val2 = ((Component)componentInParent2).transform.position - ((Component)VRRig.LocalRig).transform.position;
				Vector3 val9 = ((Vector3)(ref val2)).normalized + Vector3.up * 3f;
				componentInParent2.GetNetView().SendRPC("GrabbedByPlayer", RigUtilities.GetPlayerFromVRRig(componentInParent2), new object[3] { true, false, false });
				componentInParent2.GetNetView().SendRPC("DroppedByPlayer", RigUtilities.GetPlayerFromVRRig(componentInParent2), new object[1] { val9 * 25f });
				Main.RPCProtection();
				touchFlingCooldown = Time.time + 0.3f;
				break;
			}
		}
	}

	public static void TouchFlingReset()
	{
		touchFlingCooldown = 0f;
		touchFlingTriggered = false;
	}

	public static void RainbowTrail()
	{
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)trailObject == (Object)null)
		{
			trailObject = new GameObject("Seralyth_RainbowTrail");
			TrailRenderer val = trailObject.AddComponent<TrailRenderer>();
			val.time = 1.5f;
			val.startWidth = 0.15f;
			val.endWidth = 0f;
			((Renderer)val).material = new Material(Shader.Find("Sprites/Default"));
			Gradient val2 = new Gradient();
			val2.SetKeys((GradientColorKey[])(object)new GradientColorKey[5]
			{
				new GradientColorKey(Color.red, 0f),
				new GradientColorKey(Color.yellow, 0.25f),
				new GradientColorKey(Color.green, 0.5f),
				new GradientColorKey(Color.cyan, 0.75f),
				new GradientColorKey(Color.magenta, 1f)
			}, (GradientAlphaKey[])(object)new GradientAlphaKey[3]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 0.5f),
				new GradientAlphaKey(0f, 1f)
			});
			val.colorGradient = val2;
		}
		trailObject.transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
	}

	public static void FixRainbowTrail()
	{
		if ((Object)(object)trailObject != (Object)null)
		{
			Object.Destroy((Object)(object)trailObject);
			trailObject = null;
		}
	}

	public static void RedTrail()
	{
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)redTrailObject == (Object)null)
		{
			redTrailObject = new GameObject("Seralyth_RedTrail");
			TrailRenderer val = redTrailObject.AddComponent<TrailRenderer>();
			val.time = 1.5f;
			val.startWidth = 0.15f;
			val.endWidth = 0f;
			((Renderer)val).material = new Material(Shader.Find("Sprites/Default"));
			Gradient val2 = new Gradient();
			val2.SetKeys((GradientColorKey[])(object)new GradientColorKey[2]
			{
				new GradientColorKey(Color.red, 0f),
				new GradientColorKey(Color.red, 1f)
			}, (GradientAlphaKey[])(object)new GradientAlphaKey[3]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 0.5f),
				new GradientAlphaKey(0f, 1f)
			});
			val.colorGradient = val2;
		}
		redTrailObject.transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
	}

	public static void FixRedTrail()
	{
		if ((Object)(object)redTrailObject != (Object)null)
		{
			Object.Destroy((Object)(object)redTrailObject);
			redTrailObject = null;
		}
	}

	public static void BlueTrail()
	{
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)blueTrailObject == (Object)null)
		{
			blueTrailObject = new GameObject("Seralyth_BlueTrail");
			TrailRenderer val = blueTrailObject.AddComponent<TrailRenderer>();
			val.time = 1.5f;
			val.startWidth = 0.15f;
			val.endWidth = 0f;
			((Renderer)val).material = new Material(Shader.Find("Sprites/Default"));
			Gradient val2 = new Gradient();
			val2.SetKeys((GradientColorKey[])(object)new GradientColorKey[2]
			{
				new GradientColorKey(Color.blue, 0f),
				new GradientColorKey(Color.blue, 1f)
			}, (GradientAlphaKey[])(object)new GradientAlphaKey[3]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 0.5f),
				new GradientAlphaKey(0f, 1f)
			});
			val.colorGradient = val2;
		}
		blueTrailObject.transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
	}

	public static void FixBlueTrail()
	{
		if ((Object)(object)blueTrailObject != (Object)null)
		{
			Object.Destroy((Object)(object)blueTrailObject);
			blueTrailObject = null;
		}
	}

	public static void GreenTrail()
	{
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)greenTrailObject == (Object)null)
		{
			greenTrailObject = new GameObject("Seralyth_GreenTrail");
			TrailRenderer val = greenTrailObject.AddComponent<TrailRenderer>();
			val.time = 1.5f;
			val.startWidth = 0.15f;
			val.endWidth = 0f;
			((Renderer)val).material = new Material(Shader.Find("Sprites/Default"));
			Gradient val2 = new Gradient();
			val2.SetKeys((GradientColorKey[])(object)new GradientColorKey[2]
			{
				new GradientColorKey(Color.green, 0f),
				new GradientColorKey(Color.green, 1f)
			}, (GradientAlphaKey[])(object)new GradientAlphaKey[3]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 0.5f),
				new GradientAlphaKey(0f, 1f)
			});
			val.colorGradient = val2;
		}
		greenTrailObject.transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
	}

	public static void FixGreenTrail()
	{
		if ((Object)(object)greenTrailObject != (Object)null)
		{
			Object.Destroy((Object)(object)greenTrailObject);
			greenTrailObject = null;
		}
	}

	public static void YellowTrail()
	{
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)yellowTrailObject == (Object)null)
		{
			yellowTrailObject = new GameObject("Seralyth_YellowTrail");
			TrailRenderer val = yellowTrailObject.AddComponent<TrailRenderer>();
			val.time = 1.5f;
			val.startWidth = 0.15f;
			val.endWidth = 0f;
			((Renderer)val).material = new Material(Shader.Find("Sprites/Default"));
			Gradient val2 = new Gradient();
			val2.SetKeys((GradientColorKey[])(object)new GradientColorKey[2]
			{
				new GradientColorKey(Color.yellow, 0f),
				new GradientColorKey(Color.yellow, 1f)
			}, (GradientAlphaKey[])(object)new GradientAlphaKey[3]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 0.5f),
				new GradientAlphaKey(0f, 1f)
			});
			val.colorGradient = val2;
		}
		yellowTrailObject.transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
	}

	public static void FixYellowTrail()
	{
		if ((Object)(object)yellowTrailObject != (Object)null)
		{
			Object.Destroy((Object)(object)yellowTrailObject);
			yellowTrailObject = null;
		}
	}

	public static void PurpleTrail()
	{
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)purpleTrailObject == (Object)null)
		{
			purpleTrailObject = new GameObject("Seralyth_PurpleTrail");
			TrailRenderer val = purpleTrailObject.AddComponent<TrailRenderer>();
			val.time = 1.5f;
			val.startWidth = 0.15f;
			val.endWidth = 0f;
			((Renderer)val).material = new Material(Shader.Find("Sprites/Default"));
			Gradient val2 = new Gradient();
			val2.SetKeys((GradientColorKey[])(object)new GradientColorKey[2]
			{
				new GradientColorKey(Color.magenta, 0f),
				new GradientColorKey(Color.magenta, 1f)
			}, (GradientAlphaKey[])(object)new GradientAlphaKey[3]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 0.5f),
				new GradientAlphaKey(0f, 1f)
			});
			val.colorGradient = val2;
		}
		purpleTrailObject.transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
	}

	public static void FixPurpleTrail()
	{
		if ((Object)(object)purpleTrailObject != (Object)null)
		{
			Object.Destroy((Object)(object)purpleTrailObject);
			purpleTrailObject = null;
		}
	}

	public static void OrangeTrail()
	{
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)orangeTrailObject == (Object)null)
		{
			orangeTrailObject = new GameObject("Seralyth_OrangeTrail");
			TrailRenderer val = orangeTrailObject.AddComponent<TrailRenderer>();
			val.time = 1.5f;
			val.startWidth = 0.15f;
			val.endWidth = 0f;
			((Renderer)val).material = new Material(Shader.Find("Sprites/Default"));
			Gradient val2 = new Gradient();
			val2.SetKeys((GradientColorKey[])(object)new GradientColorKey[2]
			{
				new GradientColorKey(new Color(1f, 0.5f, 0f), 0f),
				new GradientColorKey(new Color(1f, 0.5f, 0f), 1f)
			}, (GradientAlphaKey[])(object)new GradientAlphaKey[3]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 0.5f),
				new GradientAlphaKey(0f, 1f)
			});
			val.colorGradient = val2;
		}
		orangeTrailObject.transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
	}

	public static void FixOrangeTrail()
	{
		if ((Object)(object)orangeTrailObject != (Object)null)
		{
			Object.Destroy((Object)(object)orangeTrailObject);
			orangeTrailObject = null;
		}
	}

	public static void PinkTrail()
	{
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)pinkTrailObject == (Object)null)
		{
			pinkTrailObject = new GameObject("Seralyth_PinkTrail");
			TrailRenderer val = pinkTrailObject.AddComponent<TrailRenderer>();
			val.time = 1.5f;
			val.startWidth = 0.15f;
			val.endWidth = 0f;
			((Renderer)val).material = new Material(Shader.Find("Sprites/Default"));
			Gradient val2 = new Gradient();
			val2.SetKeys((GradientColorKey[])(object)new GradientColorKey[2]
			{
				new GradientColorKey(new Color(1f, 0.4f, 0.7f), 0f),
				new GradientColorKey(new Color(1f, 0.4f, 0.7f), 1f)
			}, (GradientAlphaKey[])(object)new GradientAlphaKey[3]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 0.5f),
				new GradientAlphaKey(0f, 1f)
			});
			val.colorGradient = val2;
		}
		pinkTrailObject.transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
	}

	public static void FixPinkTrail()
	{
		if ((Object)(object)pinkTrailObject != (Object)null)
		{
			Object.Destroy((Object)(object)pinkTrailObject);
			pinkTrailObject = null;
		}
	}

	public static void CyanTrail()
	{
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)cyanTrailObject == (Object)null)
		{
			cyanTrailObject = new GameObject("Seralyth_CyanTrail");
			TrailRenderer val = cyanTrailObject.AddComponent<TrailRenderer>();
			val.time = 1.5f;
			val.startWidth = 0.15f;
			val.endWidth = 0f;
			((Renderer)val).material = new Material(Shader.Find("Sprites/Default"));
			Gradient val2 = new Gradient();
			val2.SetKeys((GradientColorKey[])(object)new GradientColorKey[2]
			{
				new GradientColorKey(Color.cyan, 0f),
				new GradientColorKey(Color.cyan, 1f)
			}, (GradientAlphaKey[])(object)new GradientAlphaKey[3]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 0.5f),
				new GradientAlphaKey(0f, 1f)
			});
			val.colorGradient = val2;
		}
		cyanTrailObject.transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
	}

	public static void FixCyanTrail()
	{
		if ((Object)(object)cyanTrailObject != (Object)null)
		{
			Object.Destroy((Object)(object)cyanTrailObject);
			cyanTrailObject = null;
		}
	}

	public static void WhiteTrail()
	{
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)whiteTrailObject == (Object)null)
		{
			whiteTrailObject = new GameObject("Seralyth_WhiteTrail");
			TrailRenderer val = whiteTrailObject.AddComponent<TrailRenderer>();
			val.time = 1.5f;
			val.startWidth = 0.15f;
			val.endWidth = 0f;
			((Renderer)val).material = new Material(Shader.Find("Sprites/Default"));
			Gradient val2 = new Gradient();
			val2.SetKeys((GradientColorKey[])(object)new GradientColorKey[2]
			{
				new GradientColorKey(Color.white, 0f),
				new GradientColorKey(Color.white, 1f)
			}, (GradientAlphaKey[])(object)new GradientAlphaKey[3]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 0.5f),
				new GradientAlphaKey(0f, 1f)
			});
			val.colorGradient = val2;
		}
		whiteTrailObject.transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
	}

	public static void FixWhiteTrail()
	{
		if ((Object)(object)whiteTrailObject != (Object)null)
		{
			Object.Destroy((Object)(object)whiteTrailObject);
			whiteTrailObject = null;
		}
	}

	public static void Orbitz()
	{
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (orbitzSpheres == null)
		{
			orbitzSpheres = new List<GameObject>();
			for (int i = 0; i < 6; i++)
			{
				GameObject val = GameObject.CreatePrimitive((PrimitiveType)0);
				((Object)val).name = "Seralyth_Orbitz";
				val.transform.localScale = Vector3.one * (0.1f + (float)i * 0.03f);
				Object.Destroy((Object)(object)val.GetComponent<Collider>());
				orbitzSpheres.Add(val);
			}
		}
		float num = Time.time * 0.08f % 1f;
		Vector3 val2 = ((Component)GorillaTagger.Instance.headCollider).transform.position;
		for (int j = 0; j < orbitzSpheres.Count; j++)
		{
			float num2 = Time.time * (50f + (float)j * 25f) + (float)j * 1.047f;
			float num3 = Mathf.Sin(Time.time * (0.8f + (float)j * 0.2f) + (float)j) * 0.6f;
			float num4 = 0.7f + (float)j * 0.12f;
			orbitzSpheres[j].transform.position = val2 + new Vector3(Mathf.Sin(num2 * (MathF.PI / 180f)) * num4, num3, Mathf.Cos(num2 * (MathF.PI / 180f)) * num4);
			orbitzSpheres[j].GetComponent<Renderer>().material.color = Color.HSVToRGB((num + (float)j * 0.15f) % 1f, 1f, 1f);
		}
	}

	public static void FixOrbitz()
	{
		if (orbitzSpheres == null)
		{
			return;
		}
		foreach (GameObject orbitzSphere in orbitzSpheres)
		{
			Object.Destroy((Object)(object)orbitzSphere);
		}
		orbitzSpheres = null;
	}

	public static void OrbitzV2()
	{
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (orbitzV2Spheres == null)
		{
			orbitzV2Spheres = new List<GameObject>();
			for (int i = 0; i < 6; i++)
			{
				GameObject val = GameObject.CreatePrimitive((PrimitiveType)0);
				((Object)val).name = "Seralyth_OrbitzV2";
				val.transform.localScale = Vector3.one * (0.1f + (float)i * 0.03f);
				Object.Destroy((Object)(object)val.GetComponent<Collider>());
				orbitzV2Spheres.Add(val);
			}
		}
		float num = Time.time * 0.08f % 1f;
		Vector3 val2 = ((Component)GorillaTagger.Instance.headCollider).transform.position;
		for (int j = 0; j < orbitzV2Spheres.Count; j++)
		{
			float num2 = Time.time * (50f + (float)j * 25f) + (float)j * 1.047f;
			float num3 = Mathf.Sin(Time.time * (0.8f + (float)j * 0.2f) + (float)j) * 0.6f;
			float num4 = 0.7f + (float)j * 0.12f;
			orbitzV2Spheres[j].transform.position = val2 + new Vector3(Mathf.Sin(num2 * (MathF.PI / 180f)) * num4, num3, Mathf.Cos(num2 * (MathF.PI / 180f)) * num4);
			orbitzV2Spheres[j].GetComponent<Renderer>().material.color = Color.HSVToRGB((num + (float)j * 0.15f) % 1f, 1f, 1f);
		}
	}

	public static void FixOrbitzV2()
	{
		if (orbitzV2Spheres == null)
		{
			return;
		}
		foreach (GameObject orbitzV2Sphere in orbitzV2Spheres)
		{
			Object.Destroy((Object)(object)orbitzV2Sphere);
		}
		orbitzV2Spheres = null;
	}

	public static void OrbitzCubes()
	{
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (orbitzCubes == null)
		{
			orbitzCubes = new List<GameObject>();
			for (int i = 0; i < 6; i++)
			{
				GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
				((Object)val).name = "Seralyth_OrbitzCubes";
				val.transform.localScale = Vector3.one * (0.1f + (float)i * 0.03f);
				Object.Destroy((Object)(object)val.GetComponent<Collider>());
				orbitzCubes.Add(val);
			}
		}
		float num = Time.time * 0.08f % 1f;
		Vector3 val2 = ((Component)GorillaTagger.Instance.headCollider).transform.position;
		for (int j = 0; j < orbitzCubes.Count; j++)
		{
			float num2 = Time.time * (50f + (float)j * 25f) + (float)j * 1.047f;
			float num3 = Mathf.Sin(Time.time * (0.8f + (float)j * 0.2f) + (float)j) * 0.6f;
			float num4 = 0.7f + (float)j * 0.12f;
			orbitzCubes[j].transform.position = val2 + new Vector3(Mathf.Sin(num2 * (MathF.PI / 180f)) * num4, num3, Mathf.Cos(num2 * (MathF.PI / 180f)) * num4);
			orbitzCubes[j].GetComponent<Renderer>().material.color = Color.HSVToRGB((num + (float)j * 0.15f) % 1f, 1f, 1f);
		}
	}

	public static void FixOrbitzCubes()
	{
		if (orbitzCubes == null)
		{
			return;
		}
		foreach (GameObject orbitzCube in orbitzCubes)
		{
			Object.Destroy((Object)(object)orbitzCube);
		}
		orbitzCubes = null;
	}

	public static void OrbitzCubesV2()
	{
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (orbitzCubesV2 == null)
		{
			orbitzCubesV2 = new List<GameObject>();
			for (int i = 0; i < 6; i++)
			{
				GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
				((Object)val).name = "Seralyth_OrbitzCubesV2";
				val.transform.localScale = Vector3.one * (0.1f + (float)i * 0.03f);
				Object.Destroy((Object)(object)val.GetComponent<Collider>());
				orbitzCubesV2.Add(val);
			}
		}
		float num = Time.time * 0.08f % 1f;
		Vector3 val2 = ((Component)GorillaTagger.Instance.headCollider).transform.position;
		for (int j = 0; j < orbitzCubesV2.Count; j++)
		{
			float num2 = Time.time * (50f + (float)j * 25f) + (float)j * 1.047f;
			float num3 = Mathf.Sin(Time.time * (0.8f + (float)j * 0.2f) + (float)j) * 0.6f;
			float num4 = 0.7f + (float)j * 0.12f;
			orbitzCubesV2[j].transform.position = val2 + new Vector3(Mathf.Sin(num2 * (MathF.PI / 180f)) * num4, num3, Mathf.Cos(num2 * (MathF.PI / 180f)) * num4);
			orbitzCubesV2[j].GetComponent<Renderer>().material.color = Color.HSVToRGB((num + (float)j * 0.15f) % 1f, 1f, 1f);
		}
	}

	public static void FixOrbitzCubesV2()
	{
		if (orbitzCubesV2 == null)
		{
			return;
		}
		foreach (GameObject item in orbitzCubesV2)
		{
			Object.Destroy((Object)(object)item);
		}
		orbitzCubesV2 = null;
	}

	public static void UpsideDownHead()
	{
		VRRig.LocalRig.head.trackingRotationOffset.z = 180f;
	}

	public static void BrokenNeck()
	{
		VRRig.LocalRig.head.trackingRotationOffset.z = 90f;
	}

	public static void BackwardsHead()
	{
		VRRig.LocalRig.head.trackingRotationOffset.y = 180f;
	}

	public static void SidewaysHead()
	{
		VRRig.LocalRig.head.trackingRotationOffset.y = 90f;
	}

	public static void RandomHead()
	{
		if (!(Time.time < randomHeadDelay))
		{
			randomHeadDelay = Time.time + Random.Range(0.2f, 0.8f);
			switch (Random.Range(0, 4))
			{
			case 0:
				UpsideDownHead();
				break;
			case 1:
				BackwardsHead();
				break;
			case 2:
				SidewaysHead();
				break;
			default:
				BrokenNeck();
				break;
			}
		}
	}

	public static void HeadBang()
	{
		if (Time.time > lastBangTime)
		{
			VRRig.LocalRig.head.trackingRotationOffset.x = 50f;
			lastBangTime = Time.time + 60f / BPM;
		}
		else
		{
			VRRig.LocalRig.head.trackingRotationOffset.x = Mathf.Lerp(VRRig.LocalRig.head.trackingRotationOffset.x, 0f, 0.1f);
		}
	}

	public static void ChangeSoundboardVolume(bool positive = true)
	{
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				soundboardVolumeIndex += 0.1f;
			}
			else
			{
				soundboardVolumeIndex -= 0.1f;
			}
		}
		if (soundboardVolumeIndex > 5f)
		{
			soundboardVolumeIndex = 0f;
		}
		if (soundboardVolumeIndex < 0f)
		{
			soundboardVolumeIndex = 5f;
		}
		soundboardVolumeIndex = Mathf.Round(soundboardVolumeIndex * 10f) / 10f;
		VoiceManager.Get().ClipVolume = soundboardVolumeIndex;
		Buttons.GetIndex("Change Soundboard Volume").overlapText = "Change Default Soundboard Volume <color=grey>[</color><color=green>" + soundboardVolumeIndex + "</color><color=grey>]</color>";
	}

	public static void ChangeSoundboardSpeed(bool positive = true)
	{
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				soundboardSpeedIndex += 0.1f;
			}
			else
			{
				soundboardSpeedIndex -= 0.1f;
			}
		}
		if (soundboardSpeedIndex > 5f)
		{
			soundboardSpeedIndex = 0f;
		}
		if (soundboardSpeedIndex < 0f)
		{
			soundboardSpeedIndex = 5f;
		}
		soundboardSpeedIndex = Mathf.Round(soundboardSpeedIndex * 10f) / 10f;
		VoiceManager.Get().ClipVolume = soundboardSpeedIndex;
		Buttons.GetIndex("Change Soundboard Speed").overlapText = "Change Default Soundboard Speed <color=grey>[</color><color=green>" + soundboardSpeedIndex + "</color><color=grey>]</color>";
	}

	public static void ChangeHeadSpinSpeed(bool positive = true)
	{
		float[] array = new float[5] { 2f, 7.5f, 8f, 9f, 200f };
		string[] array2 = new string[5] { "Very Slow", "Slow", "Normal", "Fast", "Very Fast" };
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				headSpinIndex++;
			}
			else
			{
				headSpinIndex--;
			}
		}
		headSpinIndex %= array.Length;
		if (headSpinIndex < 0)
		{
			headSpinIndex = array.Length - 1;
		}
		headSpinSpeed = array[headSpinIndex];
		Buttons.GetIndex("Change Head Spin Speed").overlapText = "Change Head Spin Speed <color=grey>[</color><color=green>" + array2[headSpinIndex] + "</color><color=grey>]</color>";
	}

	public static void SpinHead(string axis)
	{
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		if (((Behaviour)VRRig.LocalRig).enabled)
		{
			switch (axis.ToLower())
			{
			case "x":
				VRRig.LocalRig.head.trackingRotationOffset.x += headSpinSpeed;
				break;
			case "y":
				VRRig.LocalRig.head.trackingRotationOffset.y += headSpinSpeed;
				break;
			case "z":
				VRRig.LocalRig.head.trackingRotationOffset.z += headSpinSpeed;
				break;
			}
			return;
		}
		Quaternion rotation;
		switch (axis.ToLower())
		{
		case "x":
		{
			Transform transform3 = ((Component)VRRig.LocalRig.head.rigTarget).transform;
			rotation = ((Component)VRRig.LocalRig.head.rigTarget).transform.rotation;
			transform3.rotation = Quaternion.Euler(((Quaternion)(ref rotation)).eulerAngles + new Vector3(headSpinSpeed, 0f, 0f));
			break;
		}
		case "y":
		{
			Transform transform2 = ((Component)VRRig.LocalRig.head.rigTarget).transform;
			rotation = ((Component)VRRig.LocalRig.head.rigTarget).transform.rotation;
			transform2.rotation = Quaternion.Euler(((Quaternion)(ref rotation)).eulerAngles + new Vector3(0f, headSpinSpeed, 0f));
			break;
		}
		case "z":
		{
			Transform transform = ((Component)VRRig.LocalRig.head.rigTarget).transform;
			rotation = ((Component)VRRig.LocalRig.head.rigTarget).transform.rotation;
			transform.rotation = Quaternion.Euler(((Quaternion)(ref rotation)).eulerAngles + new Vector3(0f, 0f, headSpinSpeed));
			break;
		}
		}
	}

	public static void SpazHead(string axis)
	{
		int num = Random.Range(0, 360);
		switch (axis.ToLower())
		{
		case "x":
			VRRig.LocalRig.head.trackingRotationOffset.x = num;
			break;
		case "y":
			VRRig.LocalRig.head.trackingRotationOffset.y = num;
			break;
		case "z":
			VRRig.LocalRig.head.trackingRotationOffset.z = num;
			break;
		}
	}

	public static void FlipHands()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = GorillaTagger.Instance.leftHandTransform.position;
		Vector3 val2 = GorillaTagger.Instance.rightHandTransform.position;
		Quaternion rotation = GorillaTagger.Instance.leftHandTransform.rotation;
		Quaternion rotation2 = GorillaTagger.Instance.rightHandTransform.rotation;
		((Component)GTPlayer.Instance.GetControllerTransform(false)).transform.position = val;
		((Component)GTPlayer.Instance.GetControllerTransform(true)).transform.position = val2;
		((Component)GTPlayer.Instance.GetControllerTransform(false)).transform.rotation = rotation;
		((Component)GTPlayer.Instance.GetControllerTransform(true)).transform.rotation = rotation2;
	}

	public static void FixHandTaps()
	{
		EffectDataPatch.enabled = false;
		EffectDataPatch.tapsEnabled = true;
		EffectDataPatch.doOverride = false;
		EffectDataPatch.overrideVolume = 0.1f;
		EffectDataPatch.tapMultiplier = 1;
		GorillaTagger.Instance.handTapVolume = 0.1f;
	}

	public static void LoudHandTaps()
	{
		EffectDataPatch.enabled = true;
		EffectDataPatch.tapsEnabled = true;
		EffectDataPatch.doOverride = true;
		EffectDataPatch.overrideVolume = 99999f;
		EffectDataPatch.tapMultiplier = 10;
		GorillaTagger.Instance.handTapVolume = 99999f;
	}

	public static void SilentHandTaps()
	{
		EffectDataPatch.enabled = true;
		EffectDataPatch.tapsEnabled = false;
		EffectDataPatch.doOverride = false;
		EffectDataPatch.overrideVolume = 0f;
		EffectDataPatch.tapMultiplier = 0;
		GorillaTagger.Instance.handTapVolume = 0f;
	}

	public static void SilentHandTapsOnTag()
	{
		if (VRRig.LocalRig.IsTagged())
		{
			SilentHandTaps();
		}
		else
		{
			FixHandTaps();
		}
	}

	public static void InstantParty()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		if (!(Time.time > instantPartyDelay))
		{
			return;
		}
		instantPartyDelay = Time.time + 0.1f;
		FriendshipGroupDetection.Instance.suppressPartyCreationUntilTimestamp = 0f;
		FriendshipGroupDetection.Instance.groupCreateAfterTimestamp = 0f;
		List<int> provisionalMembers = FriendshipGroupDetection.Instance.playersInProvisionalGroup;
		if (provisionalMembers.Count > 0)
		{
			Color val = GTColor.RandomHSV(FriendshipGroupDetection.Instance.braceletRandomColorHSVRanges);
			FriendshipGroupDetection.Instance.myBraceletColor = val;
			List<int> list = new List<int> { PhotonNetwork.LocalPlayer.ActorNumber };
			list.AddRange(from player in PhotonNetwork.PlayerListOthers
				where FriendshipGroupDetection.Instance.IsInMyGroup(player.UserId) || provisionalMembers.Contains(player.ActorNumber)
				select player.ActorNumber);
			FriendshipGroupDetection.Instance.SendPartyFormedRPC(FriendshipGroupDetection.PackColor(val), list.ToArray(), false);
			Main.RPCProtection();
		}
	}

	public static IEnumerator EnableRig()
	{
		yield return (object)new WaitForSeconds(0.3f);
		((Behaviour)VRRig.LocalRig).enabled = true;
	}

	public static void BetaWaterSplash(Vector3 splashPosition, Quaternion splashRotation, float splashScale, float boundingRadius, bool bigSplash, bool enteringWater, object general = null)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Invalid comparison between Unknown and I4
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Expected O, but got Unknown
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		if (general == null)
		{
			general = (object)(RpcTarget)0;
		}
		splashScale = Mathf.Clamp(splashScale, 1E-05f, 1f);
		boundingRadius = Mathf.Clamp(boundingRadius, 0.0001f, 0.5f);
		Vector3 val = ((Component)GorillaTagger.Instance.bodyCollider).transform.position - splashPosition;
		if (((Vector3)(ref val)).sqrMagnitude >= 8.5f)
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = splashPosition + Vector3.down * 2f;
			if (waterSplashCoroutine != null)
			{
				((MonoBehaviour)CoroutineManager.instance).StopCoroutine(waterSplashCoroutine);
			}
			waterSplashCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(EnableRig());
		}
		object[] array = new object[6] { splashPosition, splashRotation, splashScale, boundingRadius, bigSplash, enteringWater };
		try
		{
			object obj = general;
			object obj2 = obj;
			NetPlayer val2 = (NetPlayer)((obj2 is NetPlayer) ? obj2 : null);
			if (val2 == null)
			{
				if (!(obj2 is RpcTarget val3))
				{
					if (obj2 is int[] array2)
					{
						if (Extensions.Contains(array2, NetworkSystem.Instance.LocalPlayer.ActorNumber))
						{
							ObjectPools.instance.Instantiate(GTPlayer.Instance.waterParams.rippleEffect, splashPosition, splashRotation, GTPlayer.Instance.waterParams.rippleEffectScale * boundingRadius * 2f, true);
						}
						VRRig.LocalRig.GetPhotonView().RPC("RPC_PlaySplashEffect", new RaiseEventOptions
						{
							TargetActors = array2
						}, array);
					}
				}
				else
				{
					if ((int)val3 == 0)
					{
						ObjectPools.instance.Instantiate(GTPlayer.Instance.waterParams.rippleEffect, splashPosition, splashRotation, GTPlayer.Instance.waterParams.rippleEffectScale * boundingRadius * 2f, true);
						ObjectPools.instance.Instantiate(GTPlayer.Instance.waterParams.splashEffect, splashPosition, splashRotation, splashScale, true).GetComponent<WaterSplashEffect>().PlayEffect(bigSplash, enteringWater, splashScale, (WaterVolume)null);
						val3 = (RpcTarget)1;
					}
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", val3, array);
				}
			}
			else
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlaySplashEffect", NetPlayer.op_Implicit(RigUtilities.NetPlayerToPlayer(val2)), array);
			}
		}
		catch
		{
		}
		Main.RPCProtection();
	}

	public static void WaterSplashHands()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > splashDel && (Main.rightGrab || Main.leftGrab))
		{
			BetaWaterSplash(Main.rightGrab ? GorillaTagger.Instance.rightHandTransform.position : GorillaTagger.Instance.leftHandTransform.position, Main.rightGrab ? GorillaTagger.Instance.rightHandTransform.rotation : GorillaTagger.Instance.leftHandTransform.rotation, 4f, 100f, bigSplash: true, enteringWater: false);
			splashDel = Time.time + 0.1f;
		}
	}

	public static void GiveWaterSplashHandsGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null && (((VRMap)Main.lockTarget.rightMiddle).calcT > 0.5f || ((VRMap)Main.lockTarget.leftMiddle).calcT > 0.5f) && Time.time > splashDel)
			{
				Vector3 splashPosition = ((((VRMap)Main.lockTarget.rightMiddle).calcT > 0.5f) ? Main.lockTarget.rightHandTransform.position : Main.lockTarget.leftHandTransform.position);
				Quaternion splashRotation = ((((VRMap)Main.lockTarget.rightMiddle).calcT > 0.5f) ? Main.lockTarget.rightHandTransform.rotation : Main.lockTarget.leftHandTransform.rotation);
				BetaWaterSplash(splashPosition, splashRotation, 4f, 100f, bigSplash: true, enteringWater: false);
				splashDel = Time.time + 0.1f;
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
		}
	}

	public static void WaterSplashAura()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > splashDel)
		{
			BetaWaterSplash(((Component)VRRig.LocalRig).transform.position + RandomUtilities.RandomVector3(2f), RandomUtilities.RandomQuaternion(), 4f, 100f, bigSplash: true, enteringWater: false);
			splashDel = Time.time + 0.1f;
		}
	}

	public static void OrbitWaterSplash()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > splashDel)
		{
			BetaWaterSplash(((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(MathF.Cos((float)Time.frameCount / 30f), 1f, MathF.Sin((float)Time.frameCount / 30f)), RandomUtilities.RandomQuaternion(), 4f, 100f, bigSplash: true, enteringWater: false);
			splashDel = Time.time + 0.1f;
		}
	}

	public static void WaterSplashGun()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true) && Time.time > splashDel)
			{
				splashDel = Time.time + 0.1f;
				BetaWaterSplash(item.transform.position, RandomUtilities.RandomQuaternion(), 4f, 100f, bigSplash: true, enteringWater: false);
			}
		}
	}

	public static void WaterSplashWalk()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > splashDel)
		{
			if (GTPlayer.Instance.IsHandTouching(true))
			{
				RaycastHit lastHitInfoHand = GTPlayer.Instance.lastHitInfoHand;
				BetaWaterSplash(GorillaTagger.Instance.leftHandTransform.position, Quaternion.Euler(((RaycastHit)(ref lastHitInfoHand)).normal), 4f, 100f, bigSplash: true, enteringWater: false);
				splashDel = Time.time + 0.1f;
			}
			else if (GTPlayer.Instance.IsHandTouching(false))
			{
				RaycastHit lastHitInfoHand2 = GTPlayer.Instance.lastHitInfoHand;
				BetaWaterSplash(GorillaTagger.Instance.rightHandTransform.position, Quaternion.Euler(((RaycastHit)(ref lastHitInfoHand2)).normal), 4f, 100f, bigSplash: true, enteringWater: false);
				splashDel = Time.time + 0.1f;
			}
		}
	}

	public static void WaterSplashOnTouch()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		foreach (VRRig activeRig in VRRigExtensions.ActiveRigs)
		{
			if (!activeRig.IsLocal() && activeRig.IsBeingTouched())
			{
				BetaWaterSplash(activeRig.head.rigTarget.position, RandomUtilities.RandomQuaternion(), 4f, 100f, bigSplash: true, enteringWater: false);
			}
		}
	}

	public static void Boop(int sound = 84)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		bool flag2 = false;
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.isLocal)
			{
				float num = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, activeRig.headMesh.transform.position);
				float num2 = Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, activeRig.headMesh.transform.position);
				float num3 = 0.275f;
				if (!flag)
				{
					flag = num < num3;
				}
				if (!flag2)
				{
					flag2 = num2 < num3;
				}
			}
		}
		if (flag && !lastlhboop)
		{
			if (PhotonNetwork.InRoom)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", (RpcTarget)0, new object[3] { sound, true, 999999f });
				Main.RPCProtection();
			}
			else
			{
				VRRig.LocalRig.PlayHandTapLocal(sound, true, 999999f);
			}
		}
		if (flag2 && !lastrhboop)
		{
			if (PhotonNetwork.InRoom)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", (RpcTarget)0, new object[3] { sound, false, 999999f });
				Main.RPCProtection();
			}
			else
			{
				VRRig.LocalRig.PlayHandTapLocal(sound, false, 999999f);
			}
		}
		lastlhboop = flag;
		lastrhboop = flag2;
	}

	public static void AutoClicker()
	{
		autoclickstate = !autoclickstate;
		if (Main.leftTrigger > 0.5f)
		{
			((ControllerInputPoller)ControllerInputPoller.instance).leftControllerIndexFloat = (autoclickstate ? 1f : 0f);
			VRRig.LocalRig.leftHand.calcT = (autoclickstate ? 1f : 0f);
			VRRig.LocalRig.leftHand.MapMyFinger(1f);
		}
		if (Main.rightTrigger > 0.5f)
		{
			((ControllerInputPoller)ControllerInputPoller.instance).rightControllerIndexFloat = (autoclickstate ? 1f : 0f);
			VRRig.LocalRig.rightHand.calcT = (autoclickstate ? 1f : 0f);
			VRRig.LocalRig.rightHand.MapMyFinger(1f);
		}
	}

	public static void EventReceived_KeyboardTracker(EventData data)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Expected O, but got Unknown
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Expected O, but got Unknown
		try
		{
			if (data.Code != 200)
			{
				return;
			}
			string text = PhotonNetwork.PhotonServerSettings.RpcList[int.Parse(((Hashtable)data.CustomData)[(byte)5].ToString())];
			object[] array = (object[])((Hashtable)data.CustomData)[(byte)4];
			if (!(text == "RPC_PlayHandTap") || (int)array[0] != 66)
			{
				return;
			}
			VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(NetPlayer.op_Implicit(PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender, false)));
			Transform transform = Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/GorillaComputerObject/ComputerUI/keyboard (1)").transform;
			if (!(Vector3.Distance(((Component)vRRigFromPlayer).transform.position, transform.position) < 3f))
			{
				return;
			}
			string text2 = (((bool)array[1]) ? "rig/hand.L/palm.01.L/f_index.01.L/f_index.02.L/f_index.03.L/f_index.03.L_end" : "rig/hand.R/palm.01.R/f_index.01.R/f_index.02.R/f_index.03.R/f_index.03.R_end");
			Vector3 val = ((Component)vRRigFromPlayer).gameObject.transform.Find(text2).position;
			GameObject gameObject = ((Component)transform.Find("Buttons/Keys")).gameObject;
			float num = float.MaxValue;
			string text3 = "[Null]";
			foreach (Transform item in gameObject.transform)
			{
				Transform val2 = item;
				float num2 = Vector3.Distance(val2.position, val);
				if (num2 < num)
				{
					num = num2;
					text3 = Main.ToTitleCase(((Object)val2).name);
				}
			}
			if (text3.Length > 1)
			{
				text3 = "[" + text3 + "]";
			}
			bool flag = false;
			for (int i = 0; i < keyLogs.Count; i++)
			{
				object[] array2 = keyLogs[i];
				if ((Object)(VRRig)array2[0] == (Object)(object)vRRigFromPlayer)
				{
					flag = true;
					string text4 = (string)array2[1];
					object[] array3 = keyLogs[i];
					string text5;
					if (!text3.Contains("Delete"))
					{
						text5 = text4 + text3;
					}
					else if (text4.Length != 0)
					{
						string text6 = text4;
						text5 = text6.Substring(0, text6.Length - 1);
					}
					else
					{
						text5 = text4;
					}
					array3[1] = text5;
					keyLogs[i][2] = Time.time + 5f;
					break;
				}
			}
			if (!flag && !text3.Contains("Delete"))
			{
				keyLogs.Add(new object[3]
				{
					vRRigFromPlayer,
					text3,
					Time.time + 5f
				});
			}
		}
		catch
		{
		}
	}

	public static void EnableKeyboardTracker()
	{
		PhotonNetwork.NetworkingClient.EventReceived += EventReceived_KeyboardTracker;
	}

	public static void KeyboardTracker()
	{
		if (keyLogs.Count <= 0)
		{
			return;
		}
		foreach (object[] item in keyLogs.Where((object[] keylog) => Time.time > (float)keylog[2]).ToList())
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=purple>KEYLOGS</color><color=grey>]</color> " + (string)item[1], 5000);
			keyLogs.Remove(item);
		}
	}

	public static void DisableKeyboardTracker()
	{
		PhotonNetwork.NetworkingClient.EventReceived -= EventReceived_KeyboardTracker;
	}

	public static void PreloadTagSounds()
	{
		string[] array = new string[7] { "firstblood", "doublekill", "triplekill", "killingspree", "wickedsick", "monsterkill", "rampage" };
		string[] array2 = array;
		foreach (string text in array2)
		{
			AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Mods/Fun/TagSounds/" + text + ".ogg", "Audio/Mods/Fun/TagSounds/" + text + ".ogg");
		}
	}

	public static void Freecam()
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)FreeCamObject == (Object)null)
		{
			FreeCamObject = new GameObject("Seralyth_CameraObj");
			FreeCamObject.transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position;
		}
		Camera orAddComponent = GTExt.GetOrAddComponent<Camera>(FreeCamObject);
		orAddComponent.nearClipPlane = 0.01f;
		orAddComponent.cameraType = (CameraType)1;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(Main.leftJoystick.x, Main.rightJoystick.y, Main.leftJoystick.y);
		Vector3 val2 = GTVector3Extensions.X_Z(((Component)GTPlayer.Instance.bodyCollider).transform.forward);
		Vector3 val3 = GTVector3Extensions.X_Z(((Component)GTPlayer.Instance.bodyCollider).transform.right);
		Vector3 val4 = val.x * val3 + val.y * Vector3.up + val.z * val2;
		val4 *= Movement.FlySpeed;
		CameraVelocity = Vector3.Lerp(CameraVelocity, val4, 0.12875f);
		Transform transform = FreeCamObject.transform;
		transform.position += CameraVelocity * Time.unscaledDeltaTime;
		FreeCamObject.transform.rotation = ((Component)GorillaTagger.Instance.headCollider).transform.rotation;
	}

	public static void ThirdPersonCamera()
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)FreeCamObject == (Object)null)
		{
			FreeCamObject = new GameObject("Seralyth_CameraObj");
			FreeCamObject.transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position;
		}
		Camera orAddComponent = GTExt.GetOrAddComponent<Camera>(FreeCamObject);
		orAddComponent.nearClipPlane = 0.01f;
		orAddComponent.cameraType = (CameraType)1;
		FreeCamObject.transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.TransformPoint(new Vector3(0f, 0.5f, -1.5f));
		FreeCamObject.transform.rotation = ((Component)GorillaTagger.Instance.headCollider).transform.rotation;
	}

	public static void FlipCamera()
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)FreeCamObject == (Object)null)
		{
			FreeCamObject = new GameObject("Seralyth_CameraObj");
			FreeCamObject.transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position;
		}
		Camera orAddComponent = GTExt.GetOrAddComponent<Camera>(FreeCamObject);
		orAddComponent.nearClipPlane = 0.01f;
		orAddComponent.cameraType = (CameraType)1;
		FreeCamObject.transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position;
		FreeCamObject.transform.rotation = ((Component)GorillaTagger.Instance.headCollider).transform.rotation * Quaternion.Euler(0f, 180f, 0f);
	}

	public static void Nausea()
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)FreeCamObject == (Object)null)
		{
			FreeCamObject = new GameObject("Seralyth_CameraObj");
			FreeCamObject.transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position;
		}
		float num = 15f;
		Camera orAddComponent = GTExt.GetOrAddComponent<Camera>(FreeCamObject);
		orAddComponent.nearClipPlane = 0.01f;
		orAddComponent.cameraType = (CameraType)1;
		FreeCamObject.transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position;
		FreeCamObject.transform.rotation = ((Component)GorillaTagger.Instance.headCollider).transform.rotation * Quaternion.Euler(Mathf.Sin(Time.time) * num, Mathf.Cos(Time.time * 0.7f) * num, Mathf.Sin(Time.time * 1.3f) * num);
	}

	public static void HueShift(Color color)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		ZoneShaderSettings.activeInstance.SetGroundFogValue(color, 0f, float.MaxValue, 0f);
	}

	public static void PreloadJumpscareData()
	{
		AssetUtilities.LoadTextureFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Images/Mods/Fun/jumpscare.png", "Images/Mods/Fun/jumpscare.png");
		AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Mods/Fun/jumpscare.ogg", "Audio/Mods/Fun/jumpscare.ogg");
	}

	public static void JumpscareOnTag()
	{
		bool flag = VRRig.LocalRig.IsTagged();
		if (flag && !wasTagged && Random.Range(0, 2000) == 1)
		{
			Jumpscare();
		}
		wasTagged = flag;
	}

	public static void Jumpscare()
	{
		((MonoBehaviour)CoroutineManager.instance).StartCoroutine(JumpscareCoroutine());
	}

	public static IEnumerator JumpscareCoroutine()
	{
		AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Mods/Fun/jumpscare.ogg", "Audio/Mods/Fun/jumpscare.ogg", delegate(AudioClip clip)
		{
			clip.Play();
		});
		HueShift(Color.black);
		GameObject jumpscareObject = GameObject.CreatePrimitive((PrimitiveType)3);
		jumpscareObject.transform.SetParent(((Component)GorillaTagger.Instance.headCollider).transform, false);
		jumpscareObject.transform.localPosition = Vector3.forward * 1f;
		jumpscareObject.transform.localScale = new Vector3(1f, 1f, 0.01f);
		jumpscareObject.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
		Object.Destroy((Object)(object)jumpscareObject.GetComponent<Collider>());
		Material jumpscareMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
		jumpscareMaterial.SetFloat("_Surface", 1f);
		jumpscareMaterial.SetFloat("_Blend", 0f);
		jumpscareMaterial.SetFloat("_SrcBlend", 5f);
		jumpscareMaterial.SetFloat("_DstBlend", 10f);
		jumpscareMaterial.SetFloat("_ZWrite", 0f);
		jumpscareMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
		jumpscareMaterial.renderQueue = 3000;
		jumpscareObject.GetComponent<Renderer>().material = jumpscareMaterial;
		jumpscareObject.GetComponent<Renderer>().material.mainTexture = (Texture)(object)AssetUtilities.LoadTextureFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Images/Mods/Fun/jumpscare.png", "Images/Mods/Fun/jumpscare.png");
		for (int i = 0; i < 10; i++)
		{
			jumpscareObject.GetComponent<Renderer>().material.color = Color.white * (float)((i + 1) % 2);
			yield return (object)new WaitForSeconds(0.05f);
		}
		Object.Destroy((Object)(object)jumpscareObject);
		HueShift(Color.clear);
	}

	public static void SurpriseMe()
	{
		((MonoBehaviour)CoroutineManager.instance).StartCoroutine(SurpriseMeCoroutine());
	}

	private static IEnumerator SurpriseMeCoroutine()
	{
		switch (Random.Range(0, 6))
		{
		case 0:
			Jumpscare();
			break;
		case 1:
			BrokenNeck();
			yield return (object)new WaitForSeconds(0.8f);
			FixHead();
			break;
		case 2:
			BackwardsHead();
			yield return (object)new WaitForSeconds(0.8f);
			FixHead();
			break;
		case 3:
			SidewaysHead();
			yield return (object)new WaitForSeconds(0.8f);
			FixHead();
			break;
		case 4:
		{
			for (int i = 0; i < 6; i++)
			{
				HueShift((i % 2 == 0) ? RandomUtilities.RandomColor() : Color.clear);
				yield return (object)new WaitForSeconds(0.1f);
			}
			HueShift(Color.clear);
			break;
		}
		default:
			Main.NarrateText(surpriseNarrations[Random.Range(0, surpriseNarrations.Length)]);
			break;
		}
	}

	public static void SpectateGun()
	{
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				if ((Object)(object)FreeCamObject == (Object)null)
				{
					FreeCamObject = new GameObject("Seralyth_CameraObj");
					FreeCamObject.transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position;
				}
				Camera orAddComponent = GTExt.GetOrAddComponent<Camera>(FreeCamObject);
				orAddComponent.nearClipPlane = 0.01f;
				orAddComponent.cameraType = (CameraType)1;
				FreeCamObject.transform.position = ((Component)Main.lockTarget.headMesh.transform).transform.TransformPoint(new Vector3(0f, 0.25f, 0.25f));
				FreeCamObject.transform.rotation = Main.lockTarget.headMesh.transform.rotation;
				return;
			}
			var (val, _) = Main.RenderGun();
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
			DisableFreecam();
		}
	}

	public static void DisableFreecam()
	{
		if ((Object)(object)FreeCamObject != (Object)null)
		{
			Object.Destroy((Object)(object)FreeCamObject.GetComponent<Camera>());
			Object.Destroy((Object)(object)FreeCamObject);
			FreeCamObject = null;
		}
	}

	public static void ChangeTargetFOV(bool positive = true)
	{
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				targetFOV += 5;
			}
			else
			{
				targetFOV -= 5;
			}
		}
		if (targetFOV > 180)
		{
			targetFOV = 0;
		}
		if (targetFOV < 0)
		{
			targetFOV = 180;
		}
		Buttons.GetIndex("Change Target FOV").overlapText = "Change Target FOV <color=grey>[</color><color=green>" + targetFOV + "</color><color=grey>]</color>";
	}

	public static void CameraFOV()
	{
		if ((Object)(object)Main.TPC != (Object)null)
		{
			((Component)Main.TPC).GetComponent<Camera>().fieldOfView = targetFOV;
		}
	}

	public static void FixCameraFOV()
	{
		if ((Object)(object)Main.TPC != (Object)null)
		{
			((Component)Main.TPC).GetComponent<Camera>().fieldOfView = 60f;
		}
	}

	public static void PrioritizeVoiceGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			RaycastHit item = Main.RenderGun().Ray;
			VRRig[] allRigs = VRRigCache.Instance.GetAllRigs();
			foreach (VRRig val in allRigs)
			{
				val.voiceAudio.volume = (((Object)(object)val != (Object)(object)Main.lockTarget) ? 0.1f : 2f);
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref item)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else
		{
			Main.gunLocked = false;
		}
	}

	public static void DeprioritizeVoiceGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			RaycastHit item = Main.RenderGun().Ray;
			VRRig[] allRigs = VRRigCache.Instance.GetAllRigs();
			foreach (VRRig val in allRigs)
			{
				val.voiceAudio.volume = (((Object)(object)val != (Object)(object)Main.lockTarget) ? 1f : 0.1f);
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref item)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else
		{
			Main.gunLocked = false;
		}
	}

	public static void ResetVoiceAll()
	{
		VRRig[] allRigs = VRRigCache.Instance.GetAllRigs();
		foreach (VRRig val in allRigs)
		{
			val.voiceAudio.volume = 1f;
		}
	}

	public static void MuteGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (!Main.GetGunInput(isShooting: true) || !(Time.time > muteDelay))
		{
			return;
		}
		VRRig gunTarget = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
		if (!Object.op_Implicit((Object)(object)gunTarget) || gunTarget.IsLocal())
		{
			return;
		}
		foreach (GorillaPlayerScoreboardLine item in GorillaScoreboardTotalUpdater.allScoreboardLines.Where((GorillaPlayerScoreboardLine line) => line.linePlayer == RigUtilities.GetPlayerFromVRRig(gunTarget)))
		{
			muteDelay = Time.time + 0.5f;
			item.muteButton.isOn = !item.muteButton.isOn;
			item.PressButton(item.muteButton.isOn, (ButtonType)3);
		}
	}

	public static void MuteAll()
	{
		foreach (GorillaPlayerScoreboardLine item in GorillaScoreboardTotalUpdater.allScoreboardLines.Where((GorillaPlayerScoreboardLine line) => !line.muteButton.isAutoOn))
		{
			item.muteButton.isOn = true;
			item.PressButton(true, (ButtonType)3);
		}
	}

	public static void UnmuteAll()
	{
		foreach (GorillaPlayerScoreboardLine item in GorillaScoreboardTotalUpdater.allScoreboardLines.Where((GorillaPlayerScoreboardLine line) => line.muteButton.isAutoOn))
		{
			item.muteButton.isOn = false;
			item.PressButton(false, (ButtonType)3);
		}
	}

	public static void ReportGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > muteDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				NetPlayer playerFromVRRig = RigUtilities.GetPlayerFromVRRig(componentInParent);
				GorillaPlayerScoreboardLine.ReportPlayer(playerFromVRRig.UserId, (ButtonType)1, playerFromVRRig.NickName);
				muteDelay = Time.time + 0.2f;
			}
		}
	}

	public static void ReportAll()
	{
		NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
		foreach (NetPlayer val in playerListOthers)
		{
			GorillaPlayerScoreboardLine.ReportPlayer(val.UserId, (ButtonType)1, val.NickName);
		}
	}

	public static void TriggerAntiReportGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				((Behaviour)VRRig.LocalRig).enabled = false;
				try
				{
					foreach (Transform item in from line in GorillaScoreboardTotalUpdater.allScoreboardLines
						where line.linePlayer == RigUtilities.GetPlayerFromVRRig(Main.lockTarget) && Vector3.Distance(((Component)line.reportButton).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) < 50f
						select ((Component)line.reportButton).gameObject.transform)
					{
						((Component)VRRig.LocalRig).transform.position = ((Component)item).transform.position;
						((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)item).transform.position;
						((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)item).transform.position;
					}
				}
				catch
				{
				}
				if (Time.time > pressButtonDelay)
				{
					pressButtonDelay = Time.time + 0.1f;
					GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", (RpcTarget)0, new object[3] { 67, false, 999999f });
					Main.RPCProtection();
				}
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			((Behaviour)VRRig.LocalRig).enabled = true;
			Main.gunLocked = false;
		}
	}

	public static void TriggerAntiReportAll()
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		((Behaviour)VRRig.LocalRig).enabled = false;
		try
		{
			Player triggerAntiReportTarget = RigUtilities.GetRandomPlayer(includeSelf: false);
			foreach (Transform item in from line in GorillaScoreboardTotalUpdater.allScoreboardLines
				where RigUtilities.NetPlayerToPlayer(line.linePlayer) == triggerAntiReportTarget && Vector3.Distance(((Component)line.reportButton).transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) < 50f
				select ((Component)line.reportButton).gameObject.transform)
			{
				((Component)VRRig.LocalRig).transform.position = ((Component)item).transform.position;
				((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)item).transform.position;
				((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)item).transform.position;
			}
		}
		catch
		{
		}
		if (Time.time > pressButtonDelay)
		{
			pressButtonDelay = Time.time + 0.1f;
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", (RpcTarget)0, new object[3] { 67, false, 999999f });
			Main.RPCProtection();
		}
	}

	public static void BypassAntiReport()
	{
		SerializePatch.OverrideSerialization = delegate
		{
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Expected O, but got Unknown
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Expected O, but got Unknown
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			List<int> people = new List<int>();
			try
			{
				foreach (GorillaPlayerScoreboardLine allScoreboardLine in GorillaScoreboardTotalUpdater.allScoreboardLines)
				{
					Transform transform = ((Component)allScoreboardLine.reportButton).gameObject.transform;
					float num = Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, transform.position);
					float num2 = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, transform.position);
					if (num < 0.5f || num2 < 0.5f)
					{
						people.Add(allScoreboardLine.linePlayer.ActorNumber);
						flag = true;
					}
				}
			}
			catch
			{
			}
			if ((Object)(object)GorillaTagger.Instance.myVRRig != (Object)null && flag)
			{
				Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
				Vector3 val = ((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position;
				Vector3 val2 = ((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position;
				Main.SendSerialize(VRRig.LocalRig.GetPhotonView(), new RaiseEventOptions
				{
					TargetActors = (from player in PhotonNetwork.PlayerListOthers
						where !Extensions.Contains(people.ToArray(), player.ActorNumber)
						select player.ActorNumber).ToArray()
				});
				((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position - ((Component)GorillaTagger.Instance.headCollider).transform.forward * 100f;
				((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position - ((Component)GorillaTagger.Instance.headCollider).transform.forward * 100f;
				Main.SendSerialize(VRRig.LocalRig.GetPhotonView(), new RaiseEventOptions
				{
					TargetActors = people.ToArray()
				});
				((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = val;
				((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = val2;
				Main.RPCProtection();
				return false;
			}
			return true;
		};
	}

	public static void BreakModCheckers()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		Hashtable val = new Hashtable();
		foreach (string key in Visuals.modDictionary.Keys)
		{
			val[(object)key] = true;
		}
		PhotonNetwork.LocalPlayer.SetCustomProperties(val, (Hashtable)null, (WebFlags)null);
	}

	public static void ReloadModsToSpoof(string key, string value, bool add = true)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		if (add)
		{
			modsToSpoof.Add(key, value);
		}
		else
		{
			modsToSpoof.Remove(key);
		}
		Hashtable val = new Hashtable();
		foreach (string key2 in modsToSpoof.Keys)
		{
			val[(object)key2] = true;
		}
		PhotonNetwork.LocalPlayer.SetCustomProperties(val, (Hashtable)null, (WebFlags)null);
	}

	public static void CustomModSpoofer()
	{
		Main.Prompt("Would you like to choose from a mod list or type the mod property?", delegate
		{
			List<ButtonInfo> list = new List<ButtonInfo>
			{
				new ButtonInfo
				{
					buttonText = "Exit Mod List",
					method = delegate
					{
						Buttons.CurrentCategoryName = "Main";
					},
					isTogglable = false,
					toolTip = "Returns you back to the main page."
				}
			};
			list.AddRange(Visuals.modDictionary.Select((KeyValuePair<string, string> t, int i) => Visuals.modDictionary.ElementAt(i)).Select((KeyValuePair<string, string> mod, int i) => new ButtonInfo
			{
				buttonText = $"Mod{i}",
				overlapText = mod.Value,
				enableMethod = delegate
				{
					ReloadModsToSpoof(mod.Key, mod.Value);
				},
				disableMethod = delegate
				{
					ReloadModsToSpoof(mod.Key, mod.Value, add: false);
				},
				toolTip = "Show that you are using the mod " + mod.Value + " to other players."
			}));
			Buttons.buttons[Buttons.GetCategory("Mod List")] = list.ToArray();
			Buttons.CurrentCategoryName = "Mod List";
		}, delegate
		{
			Main.PromptSingleText("Please enter what you would like to spoof your mods to (seperated by commas).", delegate
			{
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Expected O, but got Unknown
				string[] array = (from s in Main.keyboardInput.Split(',')
					select s.Trim()).ToArray();
				Hashtable val = new Hashtable();
				string[] array2 = array;
				foreach (string text in array2)
				{
					string text2 = null;
					foreach (KeyValuePair<string, string> item in Visuals.modDictionary)
					{
						if (string.Equals(item.Value, text, StringComparison.OrdinalIgnoreCase))
						{
							text2 = item.Key;
							break;
						}
					}
					val[(object)(text2 ?? text)] = true;
				}
				PhotonNetwork.LocalPlayer.SetCustomProperties(val, (Hashtable)null, (WebFlags)null);
			}, "Done");
		}, "Mod List", "Type");
	}

	public static void MuteDJSets()
	{
		RadioButtonGroupWearable[] allType = Main.GetAllType<RadioButtonGroupWearable>(5f);
		foreach (RadioButtonGroupWearable val in allType)
		{
			if (((Behaviour)val).enabled)
			{
				((Behaviour)val).enabled = false;
			}
		}
	}

	public static void UnmuteDJSets()
	{
		RadioButtonGroupWearable[] allType = Main.GetAllType<RadioButtonGroupWearable>(5f);
		foreach (RadioButtonGroupWearable val in allType)
		{
			if (!((Behaviour)val).enabled)
			{
				((Behaviour)val).enabled = true;
			}
		}
	}

	public static void TapAllClass<T>() where T : Tappable
	{
		if (Main.rightGrab && Time.time > tapDelay)
		{
			T[] allType = Main.GetAllType<T>(5f);
			for (int i = 0; i < allType.Length; i++)
			{
				Tappable val = (Tappable)(object)allType[i];
				val.OnTap(1f);
			}
			Main.RPCProtection();
			tapDelay = Time.time + 0.1f;
		}
	}

	public static void TriggerLeafPileGun()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true))
			{
				((Behaviour)VRRig.LocalRig).enabled = false;
				((Component)VRRig.LocalRig).transform.position = item.transform.position + Vector3.up * ((Time.frameCount % 2 == 1) ? 10f : (-10f));
				Main.SendSerialize(VRRig.LocalRig.GetPhotonView());
				((Component)VRRig.LocalRig).transform.position = item.transform.position + Vector3.up * ((Time.frameCount % 2 == 1) ? 10f : 0f);
			}
			else
			{
				((Behaviour)VRRig.LocalRig).enabled = true;
			}
		}
	}

	public static void ActivateAllDoors()
	{
		if (Main.rightGrab && Time.time > buttonDelay)
		{
			GhostLabButton[] allType = Main.GetAllType<GhostLabButton>(5f);
			foreach (GhostLabButton val in allType)
			{
				((GorillaPressableButton)val).ButtonActivation();
				Main.RPCProtection();
			}
			buttonDelay = Time.time + 0.1f;
		}
	}

	public static void AutoHitMoleType(bool isHazard)
	{
		Mole[] allType = Main.GetAllType<Mole>(5f);
		foreach (Mole val in allType)
		{
			int randomMolePickedIndex = val.randomMolePickedIndex;
			if (val.CanTap() && val.moleTypes[randomMolePickedIndex].isHazard == isHazard && Time.time > hitDelay)
			{
				hitDelay = Time.time + 0.1f;
				((Tappable)val).OnTap(1f);
				Main.RPCProtection();
			}
		}
	}

	public static void SpazMoleMachines()
	{
		if (Time.time > moleMachineDelay)
		{
			moleMachineDelay = Time.time + 0.25f;
			WhackAMole[] allType = Main.GetAllType<WhackAMole>(5f);
			foreach (WhackAMole val in allType)
			{
				((NetworkView)val).GetView.RPC("WhackAMoleButtonPressed", (RpcTarget)0, Array.Empty<object>());
				Main.RPCProtection();
			}
		}
	}

	public static void AutoStartMoles()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Invalid comparison between Unknown and I4
		if (!(Time.time > moleMachineDelay))
		{
			return;
		}
		moleMachineDelay = Time.time + 0.1f;
		WhackAMole[] allType = Main.GetAllType<WhackAMole>(5f);
		foreach (WhackAMole val in allType)
		{
			if ((int)val.currentState == 0 || (int)val.currentState == 4)
			{
				((NetworkView)val).GetView.RPC("WhackAMoleButtonPressed", (RpcTarget)0, Array.Empty<object>());
				Main.RPCProtection();
			}
		}
	}

	public static void SetBraceletState(bool enable, bool isLeftHand)
	{
		GorillaTagger.Instance.myVRRig.SendRPC("EnableNonCosmeticHandItemRPC", (RpcTarget)0, new object[2] { enable, isLeftHand });
	}

	public static void GetBracelet(bool state)
	{
		if (Main.leftGrab)
		{
			SetBraceletState(enable: false, isLeftHand: false);
			SetBraceletState(state, isLeftHand: true);
		}
		if (Main.rightGrab)
		{
			SetBraceletState(state, isLeftHand: false);
			SetBraceletState(enable: false, isLeftHand: true);
		}
		if (Main.leftGrab || Main.rightGrab)
		{
			Main.RPCProtection();
		}
	}

	public static void BraceletSpam()
	{
		if (Time.time > braceletSpamDelay)
		{
			GetBracelet(Time.frameCount % 2 == 0);
			braceletSpamDelay = Time.time + 0.1f;
			previousBraceletSpamState = !previousBraceletSpamState;
		}
	}

	public static void RemoveBracelet()
	{
		SetBraceletState(enable: false, isLeftHand: true);
		SetBraceletState(enable: false, isLeftHand: false);
		Main.RPCProtection();
	}

	public static void RainbowBracelet()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		BraceletPatch.enabled = true;
		if (!VRRig.LocalRig.nonCosmeticRightHandItem.IsEnabled)
		{
			SetBraceletState(enable: true, isLeftHand: false);
			Main.RPCProtection();
			VRRig.LocalRig.nonCosmeticRightHandItem.EnableItem(true);
		}
		List<Color> list = new List<Color>();
		for (int i = 0; i < 10; i++)
		{
			list.Add(Color.HSVToRGB(((float)Time.frameCount / 180f + (float)i / 10f) % 1f, 1f, 1f));
		}
		VRRig.LocalRig.reliableState.isBraceletLeftHanded = false;
		VRRig.LocalRig.reliableState.braceletSelfIndex = 99;
		VRRig.LocalRig.reliableState.braceletBeadColors = list;
		VRRig.LocalRig.friendshipBraceletRightHand.UpdateBeads(list, 99);
		if (Time.time > isDirtyDelay)
		{
			isDirtyDelay = Time.time + 0.1f;
			VRRig.LocalRig.reliableState.SetIsDirty();
		}
	}

	public static void RemoveRainbowBracelet()
	{
		BraceletPatch.enabled = false;
		if (!VRRig.LocalRig.nonCosmeticRightHandItem.IsEnabled)
		{
			SetBraceletState(enable: false, isLeftHand: false);
			Main.RPCProtection();
			VRRig.LocalRig.nonCosmeticRightHandItem.EnableItem(false);
		}
		VRRig.LocalRig.reliableState.isBraceletLeftHanded = false;
		VRRig.LocalRig.reliableState.braceletSelfIndex = 0;
		VRRig.LocalRig.reliableState.braceletBeadColors.Clear();
		VRRig.LocalRig.UpdateFriendshipBracelet();
		VRRig.LocalRig.reliableState.SetIsDirty();
	}

	public static void GiveBuilderWatch()
	{
		VRRig.LocalRig.EnableBuilderResizeWatch(true);
		Main.RPCProtection();
	}

	public static void RemoveBuilderWatch()
	{
		VRRig.LocalRig.EnableBuilderResizeWatch(false);
		Main.RPCProtection();
	}

	public static void QuestNoises()
	{
		if (Main.rightTrigger > 0.5f && Time.time > lastTimeDingied)
		{
			lastTimeDingied = Time.time + VRRig.LocalRig.fxSettings.GetDelay(10);
			RoomSystem.SendMonkePointsRedeemed(50);
		}
	}

	public static void MaxQuestScore()
	{
		if (Time.time > delaybetweenscore)
		{
			delaybetweenscore = Time.time + 1f;
			VRRig.LocalRig.SetQuestScore(int.MaxValue);
		}
	}

	public static void CustomQuestScore()
	{
		if (Time.time > delaybetweenscore)
		{
			delaybetweenscore = Time.time + 1f;
			VRRig.LocalRig.SetQuestScore(targetQuestScore);
		}
	}

	public static void SetQuestScore(int score)
	{
		VRRig.LocalRig.SetQuestScore(score);
	}

	public static void ArcadeTeleporterEffectSpam()
	{
		if (Time.time > spamDelay)
		{
			spamDelay = Time.time + 0.1f;
			returnOrTeleport = !returnOrTeleport;
			Main.GetObject("City_Pretty/CosmeticsScoreboardAnchor/Arcade_prefab/MainRoom/VRArea/ModIOArcadeTeleporter/NetObject_VRTeleporter").GetComponent<PhotonView>().RPC("ActivateTeleportVFX", (RpcTarget)0, new object[2]
			{
				returnOrTeleport,
				(short)Random.Range(0, 7)
			});
			Main.RPCProtection();
		}
	}

	public static void StumpTeleporterEffectSpam()
	{
		if (Time.time > spamDelay)
		{
			spamDelay = Time.time + 0.1f;
			returnOrTeleport = !returnOrTeleport;
			Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/StumpVRHeadset/VirtualStump_StumpTeleporter/NetObject_VRTeleporter").GetComponent<PhotonView>().RPC("ActivateTeleportVFX", (RpcTarget)0, new object[2]
			{
				returnOrTeleport,
				(short)0
			});
			Main.RPCProtection();
		}
	}

	public static void SetBasementDoorState(bool open)
	{
		if (Time.time > spamDelay)
		{
			delay = Time.time + 0.1f;
			Main.GetObject("Environment Objects/LocalObjects_Prefab/CityToBasement/DungeonEntrance/DungeonDoor_Prefab").GetComponent<PhotonView>().RPC("ChangeDoorState", (RpcTarget)5, new object[1] { (object)(DoorState)(open ? 5 : 2) });
			Main.RPCProtection();
		}
	}

	public static void SetElevatorDoorState(bool open)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > spamDelay)
		{
			delay = Time.time + 0.1f;
			GRElevatorManager.ElevatorButtonPressed((ButtonType)(open ? 4 : 5), GRElevatorManager._instance.currentLocation);
			Main.RPCProtection();
		}
	}

	public static void BasementDoorSpam()
	{
		if (Time.time > spamDelay)
		{
			delay = Time.time + 0.1f;
			openOrClose = !openOrClose;
			Main.GetObject("Environment Objects/LocalObjects_Prefab/CityToBasement/DungeonEntrance/DungeonDoor_Prefab").GetComponent<PhotonView>().RPC("ChangeDoorState", (RpcTarget)5, new object[1] { (object)(DoorState)(openOrClose ? 5 : 2) });
			Main.RPCProtection();
		}
	}

	public static void ElevatorDoorSpam()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > spamDelay)
		{
			delay = Time.time + 0.1f;
			openOrClose = !openOrClose;
			GRElevatorManager.ElevatorButtonPressed((ButtonType)(openOrClose ? 4 : 5), GRElevatorManager._instance.currentLocation);
			Main.RPCProtection();
		}
	}

	public static void CustomVirtualStumpVideo()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (virtualStumpAd == null)
		{
			virtualStumpAd = new GameObject("Seralyth_VirtualStumpAd").AddComponent<VirtualStumpAd>();
		}
	}

	public static void DisableCustomVirtualStumpVideo()
	{
		((Behaviour)virtualStumpAd).enabled = false;
		Object.Destroy((Object)(object)((Component)virtualStumpAd).gameObject);
	}

	public static void ChangeCustomQuestScore(bool positive = true)
	{
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				targetQuestScore++;
			}
			else
			{
				targetQuestScore--;
			}
		}
		targetQuestScore %= 100000;
		if (targetQuestScore < 0)
		{
			targetQuestScore = 99999;
		}
		Buttons.GetIndex("Change Custom Quest Score").overlapText = "Change Custom Quest Score <color=grey>[</color><color=green>" + targetQuestScore + "</color><color=grey>]</color>";
	}

	public static void FakeFPS()
	{
		FPSPatch.enabled = true;
		FPSPatch.spoofFPSValue = Random.Range(0, 255);
	}

	public static void GrabIDCard()
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab)
		{
			foreach (GameEntity item in from grBadge in GhostReactor.instance.employeeBadges.registeredBadges
				select grBadge.gameEntity into entity
				where entity.onlyGrabActorNumber == PhotonNetwork.LocalPlayer.ActorNumber
				select entity)
			{
				((Behaviour)VRRig.LocalRig).enabled = false;
				((Component)VRRig.LocalRig).transform.position = ((Component)item).transform.position;
				ManagerRegistry.GhostReactor.GameEntityManager.RequestGrabEntity(item.id, false, Vector3.zero, Quaternion.identity);
			}
			return;
		}
		((Behaviour)VRRig.LocalRig).enabled = true;
	}

	public static void GunLibCheckpoint()
	{
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		bool rightGunHand = GunLib.rightGunHand;
		GunLib.rightGunHand = true;
		GunLib.GunLibData gunLibData = GunLib.GunInstance();
		GunLib.rightGunHand = rightGunHand;
		if (gunLibData.IsGripping)
		{
			if (gunLibData.IsTriggered)
			{
				if ((Object)(object)gunLibCheckpoint == (Object)null)
				{
					gunLibCheckpoint = GameObject.CreatePrimitive((PrimitiveType)0);
					Object.Destroy((Object)(object)gunLibCheckpoint.GetComponent<SphereCollider>());
					gunLibCheckpoint.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
					gunLibCheckpoint.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
				}
				gunLibCheckpoint.transform.position = gunLibData.HitPos + Vector3.up;
				gunLibCheckpoint.GetComponent<Renderer>().material.color = Color.green;
			}
			else if ((Object)(object)gunLibCheckpoint != (Object)null)
			{
				gunLibCheckpoint.GetComponent<Renderer>().material.color = Color.red;
			}
			if (Main.rightPrimary && !previousGunLibCheckpointPrimary && (Object)(object)gunLibCheckpoint != (Object)null)
			{
				Vector3 val = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
				Vector3 val2 = gunLibCheckpoint.transform.position;
				float num = Vector3.Distance(val, val2);
				Vector3 val3 = val2 - val;
				Vector3 normalized = ((Vector3)(ref val3)).normalized;
				float num2 = Mathf.Clamp(num * 1.5f, 10f, 60f);
				float num3 = Mathf.Clamp(num * 0.15f, 1f, 10f);
				GorillaTagger.Instance.rigidbody.linearVelocity = normalized * num2 + Vector3.up * num3;
			}
			previousGunLibCheckpointPrimary = Main.rightPrimary;
		}
		else if ((Object)(object)gunLibCheckpoint != (Object)null)
		{
			gunLibCheckpoint.GetComponent<Renderer>().material.color = Main.backgroundColor.GetColor(0);
		}
	}

	public static void DisableGunLibCheckpoint()
	{
		if ((Object)(object)gunLibCheckpoint != (Object)null)
		{
			Object.Destroy((Object)(object)gunLibCheckpoint);
			gunLibCheckpoint = null;
		}
	}

	public static void GunLibFlingGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Overpowered.FlingPlayer(Main.lockTarget);
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
		}
	}

	public static void GunLibFlingGunV2()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		if (!gunLibFlingV2Initialized)
		{
			GunLib.InitpObjs();
			GunLib.Default = Main.backgroundColor.GetColor(0);
			GunLib.Selected = Main.buttonColors[1].GetColor(0);
			gunLibFlingV2Initialized = true;
		}
		GunLib.GunInstance();
		if (GunLib.data.IsGripping && Time.time >= gunLibFlingV2Cooldown)
		{
			Collider collider = GunLib.data.Collider;
			VRRig val = ((collider != null) ? ((Component)collider).GetComponentInParent<VRRig>() : null);
			if ((Object)(object)val != (Object)null && !val.IsLocal())
			{
				Vector3 val2 = ((Component)val).transform.position - ((Component)VRRig.LocalRig).transform.position;
				Vector3 val3 = ((Vector3)(ref val2)).normalized + Vector3.up * 3f;
				val.GetNetView().SendRPC("GrabbedByPlayer", RigUtilities.GetPlayerFromVRRig(val), new object[3] { true, false, false });
				val.GetNetView().SendRPC("DroppedByPlayer", RigUtilities.GetPlayerFromVRRig(val), new object[1] { val3 * 25f });
				Main.RPCProtection();
				gunLibFlingV2Cooldown = Time.time + 0.3f;
			}
		}
	}

	public static void SetPropDistanceLimit(float distance)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		if (PhotonNetwork.InRoom && (int)GorillaGameManager.instance.GameType() == 9)
		{
			GorillaPropHuntGameManager val = (GorillaPropHuntGameManager)GorillaGameManager.instance;
			val.m_ph_hand_follow_distance = distance;
		}
	}

	public static void PurchaseAllToolStations()
	{
		if (Time.time > purchaseDelay)
		{
			ManagerRegistry.GhostReactor.GhostReactorManager.ToolPurchaseStationRequest(Random.Range(0, ManagerRegistry.GhostReactor.GhostReactorManager.reactor.toolPurchasingStations.Count - 1), (ToolPurchaseStationAction)2);
			purchaseDelay = Time.time + 0.1f;
		}
	}

	public static void SetCurrencySelf(int currency = 0)
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			GRPlayer.Get(PhotonNetwork.LocalPlayer.ActorNumber).shiftCreditCache = currency;
		}
	}

	public static void SetCurrencyGun(int currency = 0)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (!Main.GetGunInput(isShooting: true))
		{
			return;
		}
		VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
		if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
		{
			if (PhotonNetwork.IsMasterClient)
			{
				GRPlayer.Get(RigUtilities.GetPlayerFromVRRig(componentInParent).ActorNumber).shiftCreditCache = currency;
			}
			else
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			}
		}
	}

	public static void SetCurrencyAll(int currency = 0)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			return;
		}
		Player[] playerList = PhotonNetwork.PlayerList;
		foreach (Player val in playerList)
		{
			GRPlayer val2 = GRPlayer.Get(val.ActorNumber);
			val2.shiftCreditCache = currency;
		}
	}

	public static void AddCurrencySelf(int currency = 0)
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			GRPlayer obj = GRPlayer.Get(PhotonNetwork.LocalPlayer.ActorNumber);
			obj.shiftCreditCache += currency;
		}
	}

	public static void AddCurrencyGun(int currency = 0)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (!Main.GetGunInput(isShooting: true))
		{
			return;
		}
		VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
		if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
		{
			if (PhotonNetwork.IsMasterClient)
			{
				GRPlayer obj = GRPlayer.Get(RigUtilities.GetPlayerFromVRRig(componentInParent).ActorNumber);
				obj.shiftCreditCache += currency;
			}
			else
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			}
		}
	}

	public static void AddCurrencyAll(int currency = 0)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			return;
		}
		Player[] playerList = PhotonNetwork.PlayerList;
		foreach (Player val in playerList)
		{
			GRPlayer val2 = GRPlayer.Get(val.ActorNumber);
			val2.shiftCreditCache += currency;
		}
	}

	public static void RemoveCurrencySelf()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			GRPlayer.Get(PhotonNetwork.LocalPlayer.ActorNumber).shiftCreditCache = 0;
		}
	}

	public static void RemoveCurrencyGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (!Main.GetGunInput(isShooting: true))
		{
			return;
		}
		VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
		if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
		{
			if (PhotonNetwork.IsMasterClient)
			{
				GRPlayer.Get(RigUtilities.GetPlayerFromVRRig(componentInParent).ActorNumber).shiftCreditCache = 0;
			}
			else
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			}
		}
	}

	public static void RemoveCurrencyAll()
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			return;
		}
		Player[] playerList = PhotonNetwork.PlayerList;
		foreach (Player val in playerList)
		{
			GRPlayer val2 = GRPlayer.Get(val.ActorNumber);
			val2.shiftCreditCache = 0;
		}
	}

	public static void Invincibility()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Invalid comparison between Unknown and I4
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			return;
		}
		GRPlayer val = GRPlayer.Get(PhotonNetwork.LocalPlayer.ActorNumber);
		if ((int)val.State == 1)
		{
			ManagerRegistry.GhostReactor.GhostReactorManager.RequestPlayerStateChange(val, (GRPlayerState)0);
		}
		val.hp = val.maxHp;
	}

	public static void StartShift()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Invalid comparison between Unknown and I4
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			return;
		}
		ManagerRegistry.GhostReactor.GhostReactorManager.RequestShiftStartAuthority((int)GhostReactor.instance.shiftManager.ShiftState == 2);
		Main.RPCProtection();
	}

	public static void EndShift()
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			return;
		}
		ManagerRegistry.GhostReactor.GhostReactorManager.RequestShiftEnd();
		Main.RPCProtection();
	}

	public static void SetQuota()
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			return;
		}
		GhostReactor.instance.shiftManager.shiftStats.SetShiftStat((GRShiftStatType)2, GhostReactor.instance.shiftManager.coresRequiredToDelveDeeper);
		Main.RPCProtection();
	}

	public static void GhostReactorFreezeGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Overpowered.CreateItem(Main.lockTarget.GetPlayer(), Overpowered.ObjectByName["GhostReactorEnergyCostGate"], Main.lockTarget.headMesh.transform.position + RandomUtilities.RandomVector3(), RandomUtilities.RandomQuaternion(), Vector3.zero, Vector3.zero, 0L);
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
		}
	}

	public static void GhostReactorFreezeAll()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		VRRig randomVRRig = RigUtilities.GetRandomVRRig(includeSelf: false);
		Overpowered.CreateItem(randomVRRig.GetPlayer(), Overpowered.ObjectByName["GhostReactorEnergyCostGate"], randomVRRig.headMesh.transform.position + RandomUtilities.RandomVector3(), RandomUtilities.RandomQuaternion(), Vector3.zero, Vector3.zero, 0L);
	}

	public static void SetPlayerState(Player Target, GRPlayerState State)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Invalid comparison between Unknown and I4
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Invalid comparison between Unknown and I4
		GRPlayer val = GRPlayer.Get(Target.ActorNumber);
		if (val.State != State)
		{
			if ((Target == PhotonNetwork.LocalPlayer && (int)State == 1) || (NetworkSystem.Instance.IsMasterClient && (int)State == 0))
			{
				ManagerRegistry.GhostReactor.GhostReactorManager.RequestPlayerStateChange(val, State);
				Main.RPCProtection();
			}
			else if (!NetworkSystem.Instance.IsMasterClient)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			}
			else if ((int)State == 1)
			{
				((MonoBehaviour)CoroutineManager.instance).StartCoroutine(KillTarget(Target));
			}
		}
	}

	public static void SetPlayerState(NetPlayer Target, GRPlayerState State)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		SetPlayerState(RigUtilities.NetPlayerToPlayer(Target), State);
	}

	public static void SetPlayerState(VRRig Target, GRPlayerState State)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		SetPlayerState(RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(Target)), State);
	}

	public static IEnumerator KillTarget(Player Target)
	{
		GRPlayer GRPlayer = GRPlayer.Get(Target.ActorNumber);
		VRRig Rig = RigUtilities.GetVRRigFromPlayer(NetPlayer.op_Implicit(Target));
		int netId = ManagerRegistry.GhostReactor.GameEntityManager.CreateTypeNetId(Overpowered.ObjectByName["GhostReactorEnemyChaserArmored"]);
		ManagerRegistry.GhostReactor.GameEntityManager.photonView.RPC("CreateItemRPC", Target, new object[7]
		{
			new int[1] { netId },
			new int[1] { (int)ManagerRegistry.GhostReactor.GameEntityManager.zone },
			new int[1] { Overpowered.ObjectByName["GhostReactorEnemyChaserArmored"] },
			new long[1] { BitPackUtils.PackWorldPosForNetwork(((Component)Rig).transform.position) },
			new int[1] { BitPackUtils.PackQuaternionForNetwork(((Component)Rig).transform.rotation) },
			new long[1],
			new int[1]
		});
		ManagerRegistry.GhostReactor.GhostReactorManager.gameAgentManager.photonView.RPC("ApplyBehaviorRPC", Target, new object[2]
		{
			new int[1] { netId },
			new byte[1] { 6 }
		});
		GRPlayer.ChangePlayerState((GRPlayerState)1, ManagerRegistry.GhostReactor.GhostReactorManager);
		Main.RPCProtection();
		yield return null;
		yield return null;
		yield return null;
		ManagerRegistry.GhostReactor.GameEntityManager.photonView.RPC("DestroyItemRPC", Target, new object[1] { new int[1] { netId } });
		Main.RPCProtection();
	}

	public static void SetStateSelf(int state)
	{
		SetPlayerState(PhotonNetwork.LocalPlayer, (GRPlayerState)state);
	}

	public static void SetStateAll(int state)
	{
		Player[] playerList = PhotonNetwork.PlayerList;
		foreach (Player target in playerList)
		{
			SetPlayerState(target, (GRPlayerState)state);
		}
	}

	public static void SetStateGun(int state)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true))
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				SetPlayerState(componentInParent, (GRPlayerState)state);
			}
		}
	}

	public static void SpazKillSelf()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > killDelay)
		{
			killDelay = Time.time + 0.1f;
			GRPlayer val = GRPlayer.Get(PhotonNetwork.LocalPlayer.ActorNumber);
			SetPlayerState(PhotonNetwork.LocalPlayer, (GRPlayerState)((int)val.State == 0));
		}
	}

	public static void SpazKillGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true))
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal() && Time.time > killDelay)
			{
				killDelay = Time.time + 0.1f;
				GRPlayer val2 = GRPlayer.Get(RigUtilities.GetPlayerFromVRRig(componentInParent).ActorNumber);
				SetPlayerState(componentInParent, (GRPlayerState)((int)val2.State == 0));
			}
		}
	}

	public static void SpazKillAll()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > killDelay)
		{
			Player[] playerList = PhotonNetwork.PlayerList;
			foreach (Player val in playerList)
			{
				killDelay = Time.time + 0.1f;
				GRPlayer val2 = GRPlayer.Get(val.ActorNumber);
				SetPlayerState(val, (GRPlayerState)((int)val2.State == 0));
			}
		}
	}

	public static void SpazToolStations()
	{
		if (Time.time > purchaseDelay)
		{
			ManagerRegistry.GhostReactor.GhostReactorManager.ToolPurchaseStationRequest(Random.Range(0, ManagerRegistry.GhostReactor.GhostReactorManager.reactor.toolPurchasingStations.Count - 1), (ToolPurchaseStationAction)Random.Range(0, 2));
			purchaseDelay = Time.time + 0.1f;
		}
	}

	public static void SetMicrophoneQuality(int bitrate, int samplingRate)
	{
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Invalid comparison between Unknown and I4
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Invalid comparison between Unknown and I4
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Invalid comparison between Unknown and I4
		if (!PhotonNetwork.InRoom || !Object.op_Implicit((Object)(object)NetworkSystem.Instance.LocalRecorder))
		{
			return;
		}
		Recorder primaryRecorder = NetworkSystem.Instance.VoiceConnection.PrimaryRecorder;
		if (RecorderPatch.enabled)
		{
			if ((int)NetworkSystem.Instance.VoiceSettings.SamplingRate != samplingRate)
			{
				NetworkSystem.Instance.VoiceSettings.SamplingRate = (SamplingRate)samplingRate;
			}
			if (NetworkSystem.Instance.VoiceSettings.Bitrate != bitrate)
			{
				NetworkSystem.Instance.VoiceSettings.Bitrate = bitrate;
			}
			if (primaryRecorder.IsRecording)
			{
				if (primaryRecorder.Bitrate != bitrate)
				{
					primaryRecorder.Bitrate = bitrate;
				}
				if ((int)primaryRecorder.SamplingRate != samplingRate)
				{
					primaryRecorder.SamplingRate = (SamplingRate)samplingRate;
				}
				primaryRecorder.RestartRecording(false);
			}
		}
		else if ((int)primaryRecorder.SamplingRate != samplingRate || primaryRecorder.Bitrate != bitrate)
		{
			primaryRecorder.SamplingRate = (SamplingRate)samplingRate;
			primaryRecorder.Bitrate = bitrate;
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(DelayReloadMicrophone());
		}
	}

	public static void SetMicrophoneAmplification(float gain)
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (RecorderPatch.enabled)
		{
			VoiceManager.Get().Gain = gain;
			return;
		}
		Recorder primaryRecorder = NetworkSystem.Instance.VoiceConnection.PrimaryRecorder;
		if (gain > 1f)
		{
			if ((Object)(object)((Component)primaryRecorder).gameObject.GetComponent<MicAmplifier>() != (Object)null)
			{
				return;
			}
			MicAmplifier orAddComponent = GTExt.GetOrAddComponent<MicAmplifier>(((Component)primaryRecorder).gameObject);
			orAddComponent.AmplificationFactor = 16f;
			orAddComponent.BoostValue = 16f;
		}
		else if (Object.op_Implicit((Object)(object)((Component)primaryRecorder).gameObject.GetComponent<MicAmplifier>()))
		{
			if ((Object)(object)((Component)primaryRecorder).gameObject.GetComponent<MicAmplifier>() == (Object)null)
			{
				return;
			}
			MicAmplifier component = ((Component)primaryRecorder).gameObject.GetComponent<MicAmplifier>();
			((Behaviour)component).enabled = false;
			Object.Destroy((Object)(object)((Component)primaryRecorder).gameObject.GetComponent<MicAmplifier>());
		}
		((MonoBehaviour)CoroutineManager.instance).StartCoroutine(DelayReloadMicrophone());
	}

	public static void EchoMicrophone(bool status)
	{
		ButtonInfo index = Buttons.GetIndex("Legacy Microphone");
		if (index.enabled)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are using Legacy Microphone. This mod does not support using the old microphone system.");
			index.enabled = false;
		}
		else if (status)
		{
			int samplingRate = VoiceManager.Get().SamplingRate;
			int samples = Mathf.Max(1, samplingRate / 4);
			float[] delayedBuffer = new float[samples];
			int index2 = 0;
			if (VoiceManager.Get().PostProcessors.ContainsKey("Echo"))
			{
				return;
			}
			VoiceManager.Get().PostProcessors["Echo"] = delegate(float[] buffer)
			{
				for (int i = 0; i < buffer.Length; i++)
				{
					float num = delayedBuffer[index2];
					float num2 = buffer[i];
					float num3 = num2 + num * 0.5f;
					buffer[i] = Mathf.Clamp(num3, -1f, 1f);
					delayedBuffer[index2] = num3;
					index2 = (index2 + 1) % samples;
				}
			};
		}
		else
		{
			VoiceManager.Get().PostProcessors.Remove("Echo");
		}
	}

	public static void GlitchyMicrophone(bool status)
	{
		ButtonInfo index = Buttons.GetIndex("Legacy Microphone");
		if (index.enabled)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are using Legacy Microphone. This mod does not support using the old microphone system.");
			index.enabled = false;
		}
		else if (status)
		{
			int rate = VoiceManager.Get().SamplingRate;
			int repeatLength = Mathf.Max(1, rate / 2);
			float[] history = new float[repeatLength];
			int historyIndex = 0;
			float[] repeatBuffer = new float[repeatLength];
			int repeatIndex = 0;
			int repeatsLeft = 0;
			int samplesUntilNext = Random.Range(rate, rate * 4);
			VoiceManager.Get().PostProcessClip = true;
			if (VoiceManager.Get().PostProcessors.ContainsKey("Glitch"))
			{
				return;
			}
			VoiceManager.Get().PostProcessors["Glitch"] = delegate(float[] buffer)
			{
				for (int i = 0; i < buffer.Length; i++)
				{
					history[historyIndex] = buffer[i];
					historyIndex = (historyIndex + 1) % repeatLength;
					if (repeatsLeft > 0)
					{
						buffer[i] = repeatBuffer[repeatIndex];
						repeatIndex++;
						if (repeatIndex >= repeatLength)
						{
							repeatIndex = 0;
							repeatsLeft--;
						}
					}
					else
					{
						samplesUntilNext--;
						if (samplesUntilNext <= 0)
						{
							for (int j = 0; j < repeatLength; j++)
							{
								int num = (historyIndex + j) % repeatLength;
								repeatBuffer[j] = history[num];
							}
							repeatsLeft = Random.Range(1, 2);
							repeatIndex = 0;
							samplesUntilNext = Random.Range(rate, rate * 4);
						}
					}
				}
			};
		}
		else
		{
			VoiceManager.Get().PostProcessors.Remove("Glitch");
		}
	}

	public static void LaggyMicrophone(bool status)
	{
		ButtonInfo index = Buttons.GetIndex("Legacy Microphone");
		if (index.enabled)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are using Legacy Microphone. This mod does not support using the old microphone system.");
			index.enabled = false;
		}
		else if (status)
		{
			if (VoiceManager.Get().PostProcessors.ContainsKey("Lag"))
			{
				return;
			}
			VoiceManager.Get().PostProcessors["Lag"] = delegate(float[] buffer)
			{
				if (Random.value < 0.25f)
				{
					Array.Clear(buffer, 0, buffer.Length);
				}
			};
		}
		else
		{
			VoiceManager.Get().PostProcessors.Remove("Lag");
		}
	}

	public static void MuteMicrophone(bool mute)
	{
		if (RecorderPatch.enabled)
		{
			VoiceManager.Get().MuteMicrophone = mute;
		}
		else if (PhotonNetwork.InRoom)
		{
			Recorder primaryRecorder = NetworkSystem.Instance.VoiceConnection.PrimaryRecorder;
			if (primaryRecorder.IsRecording == mute)
			{
				primaryRecorder.IsRecording = !mute;
			}
		}
	}

	public static void SetMicrophonePitch(float pitch)
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (RecorderPatch.enabled)
		{
			VoiceManager.Get().Pitch = pitch;
			return;
		}
		Recorder primaryRecorder = NetworkSystem.Instance.VoiceConnection.PrimaryRecorder;
		if (!Mathf.Approximately(pitch, 1f))
		{
			MicPitchShifter component = ((Component)primaryRecorder).gameObject.GetComponent<MicPitchShifter>();
			if ((Object)(object)component != (Object)null && Mathf.Approximately(component.PitchFactor, pitch))
			{
				return;
			}
			MicPitchShifter orAddComponent = GTExt.GetOrAddComponent<MicPitchShifter>(((Component)primaryRecorder).gameObject);
			orAddComponent.PitchFactor = pitch;
		}
		else
		{
			if (!Object.op_Implicit((Object)(object)((Component)primaryRecorder).gameObject.GetComponent<MicPitchShifter>()))
			{
				return;
			}
			MicPitchShifter component2 = ((Component)primaryRecorder).gameObject.GetComponent<MicPitchShifter>();
			((Behaviour)component2).enabled = false;
			Object.Destroy((Object)(object)((Component)primaryRecorder).gameObject.GetComponent<MicPitchShifter>());
		}
		((MonoBehaviour)CoroutineManager.instance).StartCoroutine(DelayReloadMicrophone());
	}

	public static void SetDebugEchoMode(bool value)
	{
		if (PhotonNetwork.InRoom && (Object)(object)NetworkSystem.Instance.VoiceConnection.PrimaryRecorder != (Object)null && NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.IsRecording)
		{
			NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.DebugEchoMode = value;
		}
	}

	public static void CopyVoiceGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					if (RecorderPatch.enabled)
					{
						SpeakerPatch.enabled = true;
						SpeakerPatch.targetSpeaker = ((Component)Main.lockTarget).gameObject.GetComponent<GorillaSpeakerLoudness>().speaker;
						if (!VoiceManager.Get().PostProcessors.ContainsKey("CopyVoice"))
						{
							float readPos = 0f;
							VoiceManager.Get().PostProcessors["CopyVoice"] = delegate(float[] buffer)
							{
								VoiceManager voiceManager = VoiceManager.Get();
								int channels = voiceManager.Channels;
								float num = (((Object)(object)SpeakerPatch.targetSpeaker != (Object)null && ((VoiceInfo)(ref SpeakerPatch.targetSpeaker.RemoteVoiceLink.Info)).SamplingRate > 0) ? ((VoiceInfo)(ref SpeakerPatch.targetSpeaker.RemoteVoiceLink.Info)).SamplingRate : 24000);
								float num2 = num / (float)voiceManager.OutputRate;
								lock (SpeakerPatch.locked)
								{
									if ((float)SpeakerPatch.SampleQueue.Count < num * 0.04f)
									{
										Array.Clear(buffer, 0, buffer.Length);
									}
									else
									{
										for (int i = 0; i < buffer.Length; i += channels)
										{
											int num3 = (int)readPos;
											int num4 = num3 + 1;
											if (num4 < SpeakerPatch.SampleQueue.Count)
											{
												float num5 = readPos - (float)num3;
												float num6 = Mathf.Lerp(SpeakerPatch.SampleQueue[num3], SpeakerPatch.SampleQueue[num4], num5);
												for (int j = 0; j < channels; j++)
												{
													int num7 = i + j;
													if (num7 < buffer.Length)
													{
														buffer[num7] = num6;
													}
												}
												readPos += num2;
											}
											else
											{
												for (int k = 0; k < channels; k++)
												{
													if (i + k < buffer.Length)
													{
														buffer[i + k] = 0f;
													}
												}
											}
											int num8 = (int)readPos;
											if (num8 > 0)
											{
												SpeakerPatch.SampleQueue.RemoveRange(0, Math.Min(num8, SpeakerPatch.SampleQueue.Count));
												readPos -= num8;
											}
										}
									}
								}
							};
						}
					}
					else if (Time.time > copyVoiceGunDelay)
					{
						copyVoiceGunDelay = Time.time + 0.5f;
						Main.gunLocked = true;
						Main.lockTarget = componentInParent;
						SpeakerPatch.enabled = true;
						SpeakerPatch.targetSpeaker = ((Component)Main.lockTarget).gameObject.GetComponent<GorillaSpeakerLoudness>().speaker;
						RecorderPatch.enabled = !Buttons.GetIndex("Legacy Microphone").enabled;
						VoiceManager.Get().PostProcessors["CopyVoice"] = null;
						factory?.Dispose();
						factory = new LoopbackFactory();
						NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.SourceType = (InputSourceType)2;
						NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.InputFactory = () => (IAudioDesc)(object)factory;
						((MonoBehaviour)CoroutineManager.instance).StartCoroutine(DelayReloadMicrophone());
					}
					NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.DebugEchoMode = true;
				}
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent2 = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent2) && !componentInParent2.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent2;
				}
			}
		}
		else
		{
			if (Main.gunLocked)
			{
				Main.gunLocked = false;
			}
			if (factory != null || VoiceManager.Get().PostProcessors.ContainsKey("CopyVoice"))
			{
				DisableCopyVoice();
			}
		}
	}

	public static void DisableCopyVoice()
	{
		factory?.Dispose();
		VoiceManager.Get().PostProcessors.Remove("CopyVoice");
		SpeakerPatch.enabled = false;
		Sound.FixMicrophone();
		RecorderPatch.enabled = !Buttons.GetIndex("Legacy Microphone").enabled;
		NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.DebugEchoMode = false;
	}

	public static void SaveNarration(string text)
	{
		string text2 = "SeralythMenu/Sounds/Narrations";
		if (!Directory.Exists(text2))
		{
			Directory.CreateDirectory(text2);
		}
		((MonoBehaviour)CoroutineManager.instance).StartCoroutine(Main.TranscribeText(text, delegate
		{
			Main.PromptSingleText("The narration has been saved in your Soundboard!");
		}, text, text2));
	}

	public static void MaskVoice()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Invalid comparison between Unknown and I4
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Expected O, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Expected O, but got Unknown
		ButtonInfo mod = Buttons.GetIndex("AI Assistant");
		if ((int)Application.platform == 2 && Environment.OSVersion.Version.Major < 10)
		{
			Main.PromptSingle("Your version of Windows is too old for this mod to run.", delegate
			{
				mod.enabled = false;
			});
		}
		else if ((int)Application.platform != 2)
		{
			Main.PromptSingle("You must be on Windows 10 or greater for this mod to run.", delegate
			{
				mod.enabled = false;
			});
		}
		drec = new DictationRecognizer();
		DictationRecognizer obj = drec;
		object obj2 = _003C_003Ec._003C_003E9__238_2;
		if (obj2 == null)
		{
			DictationResultDelegate val = delegate(string text, ConfidenceLevel confidence)
			{
				if (Settings.debugDictation)
				{
					LogManager.Log("Dictation result: " + text);
				}
				NotificationManager.SendNotification("<color=grey>[</color><color=green>VOICE</color><color=grey>]</color> " + text);
				if ((Object)(object)NetworkSystem.Instance.VoiceConnection.PrimaryRecorder != (Object)null)
				{
					NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.IsRecording = true;
					if (PhotonNetwork.InRoom)
					{
						Main.SpeakText(text, disableMicrophone: true);
					}
					else
					{
						Main.NarrateText(text);
					}
				}
				else
				{
					Main.NarrateText(text);
				}
			};
			_003C_003Ec._003C_003E9__238_2 = val;
			obj2 = (object)val;
		}
		obj.DictationResult += (DictationResultDelegate)obj2;
		DictationRecognizer obj3 = drec;
		object obj4 = _003C_003Ec._003C_003E9__238_3;
		if (obj4 == null)
		{
			DictationCompletedDelegate val2 = delegate
			{
				drec.Start();
			};
			_003C_003Ec._003C_003E9__238_3 = val2;
			obj4 = (object)val2;
		}
		obj3.DictationComplete += (DictationCompletedDelegate)obj4;
		drec.DictationError += (DictationErrorHandler)delegate(string error, int hresult)
		{
			if (Settings.debugDictation)
			{
				LogManager.LogError($"Dictation error: {error}; HResult = {hresult}.");
			}
			if (error.Contains("Dictation support is not enabled on this device"))
			{
				DisableMaskVoice();
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Online Speech Recognition is not enabled on this device. Either open the menu to enable it, or check your internet connection.", 3000);
				Main.Prompt("Online Speech Recognition is not enabled on your device. Would you like to open the Settings page to enable it?", delegate
				{
					Process.Start("ms-settings:privacy-speech");
					Main.PromptSingle("Once you enable Online Speech Recognition, turn this mod back on!", delegate
					{
						mod.enabled = false;
					});
				}, delegate
				{
					Main.PromptSingle("You will not be able to use this mod until you enable Online Speech Recognition.", delegate
					{
						mod.enabled = false;
					});
				});
			}
		};
		DictationRecognizer obj5 = drec;
		object obj6 = _003C_003Ec._003C_003E9__238_5;
		if (obj6 == null)
		{
			DictationHypothesisDelegate val3 = delegate(string text)
			{
				if (Settings.debugDictation)
				{
					LogManager.Log("Hypothesis: " + text);
				}
				NotificationManager.ClearAllNotifications();
				NotificationManager.SendNotification("<color=grey>[</color><color=green>VOICE</color><color=grey>]</color> " + text);
			};
			_003C_003Ec._003C_003E9__238_5 = val3;
			obj6 = (object)val3;
		}
		obj5.DictationHypothesis += (DictationHypothesisDelegate)obj6;
		drec.Start();
	}

	public static void DisableMaskVoice()
	{
		DictationRecognizer obj = drec;
		if (obj != null)
		{
			obj.Stop();
		}
		DictationRecognizer obj2 = drec;
		if (obj2 != null)
		{
			obj2.Dispose();
		}
		NetworkSystem.Instance.VoiceConnection.PrimaryRecorder.IsRecording = true;
	}

	public static void ProcessFrameBuffer(float[] data)
	{
		factory.Feed(data);
	}

	public static void ReloadMicrophone()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I4
		Recorder primaryRecorder = NetworkSystem.Instance.VoiceConnection.PrimaryRecorder;
		if (!RecorderPatch.enabled && (int)primaryRecorder.SourceType > 0)
		{
			primaryRecorder.SourceType = (InputSourceType)0;
		}
		primaryRecorder.RestartRecording(true);
	}

	public static IEnumerator DelayReloadMicrophone()
	{
		yield return (object)new WaitForSeconds(0.25f);
		ReloadMicrophone();
	}

	public static void ObjectToPointGun(string objectName)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		GameObject item = Main.RenderGun().NewPointer;
		if (Main.GetGunInput(isShooting: true))
		{
			ThrowableBug bug = GetBug(objectName);
			if ((Object)(object)bug != (Object)null)
			{
				((Component)bug).transform.position = item.transform.position + Vector3.up;
			}
		}
	}

	public static void CameraGun()
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true))
			{
				LckSocialCamera networkedCococam = LckSocialCameraManager.Instance._networkedCococam;
				networkedCococam.visible = true;
				networkedCococam.recording = true;
				networkedCococam.m_CameraVisuals.SetNetworkedVisualsActive(true);
				networkedCococam.m_CameraVisuals.SetRecordingState(true);
				((Component)networkedCococam).transform.position = item.transform.position + Vector3.up;
			}
		}
	}

	public static void TabletGun()
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true))
			{
				LckSocialCamera networkedTablet = LckSocialCameraManager.Instance._networkedTablet;
				networkedTablet.visible = true;
				networkedTablet.recording = true;
				networkedTablet.m_CameraVisuals.SetNetworkedVisualsActive(true);
				networkedTablet.m_CameraVisuals.SetRecordingState(true);
				((Component)networkedTablet).transform.position = item.transform.position + Vector3.up;
			}
		}
	}

	public static void GliderGun()
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		GameObject item = Main.RenderGun().NewPointer;
		if (!Main.GetGunInput(isShooting: true))
		{
			return;
		}
		GliderHoldable[] allType = Main.GetAllType<GliderHoldable>(5f);
		foreach (GliderHoldable val in allType)
		{
			if (((NetworkView)val).GetView.Owner == PhotonNetwork.LocalPlayer)
			{
				((Component)val).gameObject.transform.position = item.transform.position + Vector3.up;
			}
			else
			{
				((NetworkHoldableObject)val).OnHover((InteractionPoint)null, (GameObject)null);
			}
		}
	}

	public static void BetaDropBoard(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 avelocity, Color boardColor)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (Vector3.Distance(((Component)GorillaTagger.Instance.bodyCollider).transform.position, position) > 5f)
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = position + Vector3.down * 4f;
			if (dropBoard != null)
			{
				((MonoBehaviour)CoroutineManager.instance).StopCoroutine(dropBoard);
			}
			dropBoard = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(EnableRig());
		}
		FreeHoverboardManager.instance.SendDropBoardRPC(position, rotation, velocity, avelocity, boardColor);
		Main.RPCProtection();
	}

	public static void HoverboardGun()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true) && Time.time > hoverboardGunDelay)
			{
				hoverboardGunDelay = Time.time + 0.25f;
				BetaDropBoard(item.transform.position + Vector3.up, RandomUtilities.RandomQuaternion(), Vector3.zero, Vector3.zero, RandomUtilities.RandomColor());
			}
		}
	}

	public static void BlocksGun()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true))
			{
				RequestCreatePiece(pieceIdSet, item.transform.position + Vector3.up * 0.1f, Quaternion.identity, 0);
				Main.RPCProtection();
			}
		}
	}

	public static void SelectBlockGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true))
		{
			BuilderPiece componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<BuilderPiece>();
			if (Object.op_Implicit((Object)(object)componentInParent) && Time.time > gbgd)
			{
				gbgd = Time.time + 0.1f;
				pieceIdSet = componentInParent.pieceType;
				NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully selected piece " + componentInParent.displayName + ".");
			}
		}
	}

	public static void CopyBlockInfoGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true))
		{
			BuilderPiece componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<BuilderPiece>();
			if (Object.op_Implicit((Object)(object)componentInParent) && Time.time > gbgd)
			{
				gbgd = Time.time + 0.1f;
				GUIUtility.systemCopyBuffer = $"{componentInParent.displayName}\nPiece Type: {componentInParent.pieceType}\nPiece Name: {((Object)componentInParent).name}";
				NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully copied piece data of " + componentInParent.displayName + ".");
			}
		}
	}

	public static void SelectBlock(int type, string name)
	{
		pieceIdSet = type;
		NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully selected piece " + name.Replace("(Clone)", "") + ".");
	}

	public static Dictionary<int, string> GetAllBlockData()
	{
		if (blocks == null)
		{
			if ((Object)(object)ManagerRegistry.BuilderTable == (Object)null)
			{
				return new Dictionary<int, string>();
			}
			blocks = new Dictionary<int, string>();
			foreach (BuilderPiece item in ManagerRegistry.BuilderTable.builderPool.piecePools.SelectMany((List<BuilderPiece> list) => list))
			{
				try
				{
					blocks.Add(StaticHashExt.GetStaticHash(((Object)item).name.Replace("(Clone)", "")), item.displayName ?? item.displayName);
				}
				catch
				{
				}
			}
		}
		return blocks;
	}

	public static int[] GetAllBlockTypes()
	{
		return GetAllBlockData().Keys.ToArray();
	}

	public static int GetRandomBlockType()
	{
		int[] allBlockTypes = GetAllBlockTypes();
		return allBlockTypes[Random.Range(0, allBlockTypes.Length)];
	}

	public static void BlockBrowser()
	{
		rememberdirectory = Main.pageNumber;
		Dictionary<int, string> allBlockData = GetAllBlockData();
		List<ButtonInfo> list = new List<ButtonInfo>();
		list.Add(new ButtonInfo
		{
			buttonText = "Exit Building Block Browser",
			method = RemoveCosmeticBrowser,
			isTogglable = false,
			toolTip = "Returns you back to the fun mods."
		});
		List<ButtonInfo> list2 = list;
		int num = 0;
		foreach (KeyValuePair<int, string> block in allBlockData)
		{
			list2.Add(new ButtonInfo
			{
				buttonText = $"SelectBlock{num}",
				overlapText = block.Value,
				method = delegate
				{
					SelectBlock(block.Key, block.Value);
				},
				isTogglable = false,
				toolTip = "Selects the block \"" + block.Value + "\" to be used for the building mods."
			});
			num++;
		}
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list2.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void SetRespawnDistance(string objectName, float respawnDistance = float.MaxValue)
	{
		ThrowableBug bugObject = GetBugObject(objectName);
		((TransferrableObject)bugObject).maxDistanceFromOriginBeforeRespawn = respawnDistance;
		((TransferrableObject)bugObject).maxDistanceFromTargetPlayerBeforeRespawn = respawnDistance;
	}

	public static void PermanentOwnership(string objectName)
	{
		OwnershipPatch.enabled = true;
		ThrowableBug bugObject = GetBugObject(objectName);
		if (!PhotonNetwork.InRoom)
		{
			OwnershipPatch.blacklistedGuards.Clear();
		}
		else if (((TransferrableObject)bugObject).IsMyItem())
		{
			if ((Object)(object)((TransferrableObject)bugObject).targetRig != (Object)(object)VRRig.LocalRig)
			{
				((TransferrableObject)bugObject).SetTargetRig(VRRig.LocalRig);
			}
			if (!OwnershipPatch.blacklistedGuards.Contains(((TransferrableObject)bugObject).worldShareableInstance.guard))
			{
				OwnershipPatch.blacklistedGuards.Add(((TransferrableObject)bugObject).worldShareableInstance.guard);
			}
		}
	}

	public static void SpazSnowballs()
	{
		if (Main.leftGrab)
		{
			SnowballThrowable projectile = Main.GetProjectile(Projectiles.SnowballName + "LeftAnchor");
			GrowingSnowballThrowable val = (GrowingSnowballThrowable)(object)((projectile is GrowingSnowballThrowable) ? projectile : null);
			((SnowballThrowable)val).randomizeColor = true;
			((SnowballThrowable)val).SetSnowballActiveLocal(true);
			val.SetSizeLevelAuthority(Random.Range(1, 6));
		}
		if (Main.rightGrab)
		{
			SnowballThrowable projectile2 = Main.GetProjectile(Projectiles.SnowballName + "RightAnchor");
			GrowingSnowballThrowable val2 = (GrowingSnowballThrowable)(object)((projectile2 is GrowingSnowballThrowable) ? projectile2 : null);
			((SnowballThrowable)val2).randomizeColor = true;
			((SnowballThrowable)val2).SetSnowballActiveLocal(true);
			val2.SetSizeLevelAuthority(Random.Range(1, 6));
		}
	}

	public static void FastSnowballs()
	{
		SnowballMaker[] array = (SnowballMaker[])(object)new SnowballMaker[2]
		{
			SnowballMaker.leftHandInstance,
			SnowballMaker.rightHandInstance
		};
		foreach (SnowballMaker val in array)
		{
			SnowballThrowable[] snowballs = val.snowballs;
			foreach (SnowballThrowable val2 in snowballs)
			{
				val2.linSpeedMultiplier = 10f;
				val2.maxLinSpeed = 99999f;
			}
		}
	}

	public static void SlowSnowballs()
	{
		SnowballMaker[] array = (SnowballMaker[])(object)new SnowballMaker[2]
		{
			SnowballMaker.leftHandInstance,
			SnowballMaker.rightHandInstance
		};
		foreach (SnowballMaker val in array)
		{
			SnowballThrowable[] snowballs = val.snowballs;
			foreach (SnowballThrowable val2 in snowballs)
			{
				val2.linSpeedMultiplier = 0.2f;
				val2.maxLinSpeed = 6f;
			}
		}
	}

	public static void FixSnowballs()
	{
		SnowballMaker[] array = (SnowballMaker[])(object)new SnowballMaker[2]
		{
			SnowballMaker.leftHandInstance,
			SnowballMaker.rightHandInstance
		};
		foreach (SnowballMaker val in array)
		{
			SnowballThrowable[] snowballs = val.snowballs;
			foreach (SnowballThrowable val2 in snowballs)
			{
				val2.linSpeedMultiplier = 1f;
				val2.maxLinSpeed = 12f;
			}
		}
	}

	public static void ProjectileRange()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		LoopingArray<ProjectileInfo> localProjectiles = ProjectileTracker.m_localProjectiles;
		if (localProjectiles == null || localProjectiles.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < localProjectiles.Length; i++)
		{
			SlingshotProjectile projectileInstance = localProjectiles[i].projectileInstance;
			if ((Object)(object)projectileInstance == (Object)null || !((Component)projectileInstance).gameObject.activeSelf)
			{
				continue;
			}
			foreach (VRRig item in from rig in VRRigCache.ActiveRigs
				where !rig.IsLocal()
				where rig.Distance(((Component)projectileInstance).transform.position) < 0.5f
				select rig)
			{
				((Component)projectileInstance).transform.position = item.headMesh.transform.position;
			}
		}
	}

	public static void HookProjectileColors()
	{
		SerializePatch.OverrideSerialization = delegate
		{
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			if (PhotonNetwork.InRoom)
			{
				Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
				List<SnowballThrowable> list = new List<SnowballThrowable>();
				SnowballMaker[] array = (SnowballMaker[])(object)new SnowballMaker[2]
				{
					SnowballMaker.leftHandInstance,
					SnowballMaker.rightHandInstance
				};
				foreach (SnowballMaker val in array)
				{
					list.AddRange(val.snowballs.Where((SnowballThrowable Throwable) => ((Component)Throwable).gameObject.activeSelf));
				}
				if (list.Count <= 0)
				{
					Main.SendSerialize(VRRig.LocalRig.GetPhotonView());
					return false;
				}
				foreach (SnowballThrowable item in list)
				{
					item.SetSnowballActiveLocal(false);
				}
				VRRig.LocalRig.reliableState.SetIsDirty();
				Main.SendSerialize(VRRig.LocalRig.GetPhotonView());
				foreach (SnowballThrowable item2 in list)
				{
					GrowingSnowballThrowable val2 = (GrowingSnowballThrowable)(object)((item2 is GrowingSnowballThrowable) ? item2 : null);
					if (val2 != null)
					{
						val2.maintainSizeLevelUntilLocalTime = Time.time;
					}
					item2.randomizeColor = true;
					VRRig.LocalRig.SetThrowableProjectileColor(((Object)((Component)item2).gameObject).name.ToLower().Contains("left"), Color32.op_Implicit(projHookColor));
					item2.SetSnowballActiveLocal(true);
					item2.ApplyColor(projHookColor);
				}
				VRRig.LocalRig.reliableState.SetIsDirty();
				Main.SendSerialize(VRRig.LocalRig.GetPhotonView());
				return false;
			}
			return true;
		};
	}

	public static void SnowballButtocks()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		((Behaviour)VRRig.LocalRig).enabled = false;
		((Component)VRRig.LocalRig).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + new Vector3(0f, 0.15f, 0f);
		((Component)VRRig.LocalRig).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation;
		((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = ((Component)GorillaTagger.Instance.headCollider).transform.rotation;
		((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + ((Component)VRRig.LocalRig).transform.TransformDirection(new Vector3(-0.0436f, -0.3f, -0.1563f));
		((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + ((Component)VRRig.LocalRig).transform.TransformDirection(new Vector3(-0.0072f, -0.2964f, -0.1563f));
		((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation * Quaternion.Euler(330f, 344.5f, 0f);
		((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation * Quaternion.Euler(340f, 165.5f, 160f);
		((VRMap)VRRig.LocalRig.leftIndex).calcT = 1f;
		((VRMap)VRRig.LocalRig.leftMiddle).calcT = 1f;
		((VRMap)VRRig.LocalRig.leftThumb).calcT = 1f;
		((VRMap)VRRig.LocalRig.leftIndex).LerpFinger(1f, false);
		((VRMap)VRRig.LocalRig.leftMiddle).LerpFinger(1f, false);
		((VRMap)VRRig.LocalRig.leftThumb).LerpFinger(1f, false);
		((VRMap)VRRig.LocalRig.rightIndex).calcT = 1f;
		((VRMap)VRRig.LocalRig.rightMiddle).calcT = 1f;
		((VRMap)VRRig.LocalRig.rightThumb).calcT = 1f;
		((VRMap)VRRig.LocalRig.rightIndex).LerpFinger(1f, false);
		((VRMap)VRRig.LocalRig.rightMiddle).LerpFinger(1f, false);
		((VRMap)VRRig.LocalRig.rightThumb).LerpFinger(1f, false);
		SnowballThrowable projectile = Main.GetProjectile(Projectiles.SnowballName + "LeftAnchor");
		GrowingSnowballThrowable val = (GrowingSnowballThrowable)(object)((projectile is GrowingSnowballThrowable) ? projectile : null);
		if (!((Component)val).gameObject.activeSelf)
		{
			((SnowballThrowable)val).SetSnowballActiveLocal(true);
			val.SetSizeLevelAuthority(3);
			VRRig.LocalRig.SetThrowableProjectileColor(true, Color32.op_Implicit(VRRig.LocalRig.playerColor));
			((SnowballThrowable)val).ApplyColor(VRRig.LocalRig.playerColor);
		}
		SnowballThrowable projectile2 = Main.GetProjectile(Projectiles.SnowballName + "RightAnchor");
		GrowingSnowballThrowable val2 = (GrowingSnowballThrowable)(object)((projectile2 is GrowingSnowballThrowable) ? projectile2 : null);
		if (!((Component)val2).gameObject.activeSelf)
		{
			((SnowballThrowable)val2).SetSnowballActiveLocal(true);
			val2.SetSizeLevelAuthority(3);
			VRRig.LocalRig.SetThrowableProjectileColor(false, Color32.op_Implicit(VRRig.LocalRig.playerColor));
			((SnowballThrowable)val2).ApplyColor(VRRig.LocalRig.playerColor);
		}
	}

	public static void SnowballBreasts()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		((Behaviour)VRRig.LocalRig).enabled = false;
		((Component)VRRig.LocalRig).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + new Vector3(0f, 0.15f, 0f);
		((Component)VRRig.LocalRig).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation;
		((Component)VRRig.LocalRig.head.rigTarget).transform.rotation = ((Component)GorillaTagger.Instance.headCollider).transform.rotation;
		((Component)VRRig.LocalRig.leftHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + ((Component)VRRig.LocalRig).transform.TransformDirection(new Vector3(-0.08f, -0.0691f, 0f));
		((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)VRRig.LocalRig).transform.position + ((Component)VRRig.LocalRig).transform.TransformDirection(new Vector3(-0.0073f, -0.2182f, 0.0164f));
		((Component)VRRig.LocalRig.leftHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation * Quaternion.Euler(350f, 140f, 62f);
		((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = ((Component)VRRig.LocalRig).transform.rotation * Quaternion.Euler(8f, 30f, 8f);
		((VRMap)VRRig.LocalRig.leftIndex).calcT = 1f;
		((VRMap)VRRig.LocalRig.leftMiddle).calcT = 1f;
		((VRMap)VRRig.LocalRig.leftThumb).calcT = 1f;
		((VRMap)VRRig.LocalRig.leftIndex).LerpFinger(1f, false);
		((VRMap)VRRig.LocalRig.leftMiddle).LerpFinger(1f, false);
		((VRMap)VRRig.LocalRig.leftThumb).LerpFinger(1f, false);
		((VRMap)VRRig.LocalRig.rightIndex).calcT = 1f;
		((VRMap)VRRig.LocalRig.rightMiddle).calcT = 1f;
		((VRMap)VRRig.LocalRig.rightThumb).calcT = 1f;
		((VRMap)VRRig.LocalRig.rightIndex).LerpFinger(1f, false);
		((VRMap)VRRig.LocalRig.rightMiddle).LerpFinger(1f, false);
		((VRMap)VRRig.LocalRig.rightThumb).LerpFinger(1f, false);
		SnowballThrowable projectile = Main.GetProjectile(Projectiles.SnowballName + "LeftAnchor");
		GrowingSnowballThrowable val = (GrowingSnowballThrowable)(object)((projectile is GrowingSnowballThrowable) ? projectile : null);
		if (!((Component)val).gameObject.activeSelf)
		{
			((SnowballThrowable)val).SetSnowballActiveLocal(true);
			val.IncreaseSize(3);
			VRRig.LocalRig.SetThrowableProjectileColor(true, Color32.op_Implicit(VRRig.LocalRig.playerColor));
			((SnowballThrowable)val).ApplyColor(VRRig.LocalRig.playerColor);
		}
		SnowballThrowable projectile2 = Main.GetProjectile(Projectiles.SnowballName + "RightAnchor");
		GrowingSnowballThrowable val2 = (GrowingSnowballThrowable)(object)((projectile2 is GrowingSnowballThrowable) ? projectile2 : null);
		if (!((Component)val2).gameObject.activeSelf)
		{
			((SnowballThrowable)val2).SetSnowballActiveLocal(true);
			val2.IncreaseSize(3);
			VRRig.LocalRig.SetThrowableProjectileColor(false, Color32.op_Implicit(VRRig.LocalRig.playerColor));
			((SnowballThrowable)val2).ApplyColor(VRRig.LocalRig.playerColor);
		}
	}

	public static void DisableSnowballGenitals()
	{
		((Behaviour)VRRig.LocalRig).enabled = true;
		Main.GetProjectile(Projectiles.SnowballName + "LeftAnchor").SetSnowballActiveLocal(false);
		Main.GetProjectile(Projectiles.SnowballName + "lRightAnchor").SetSnowballActiveLocal(false);
	}

	public static void FastHoverboard()
	{
		GTPlayer.Instance.hoverboardPaddleBoostMax = float.MaxValue;
		GTPlayer.Instance.hoverboardPaddleBoostMultiplier = 5f;
		GTPlayer.Instance.hoverboardBoostGracePeriod = 0f;
		GTPlayer.Instance.hoverTiltAdjustsForwardFactor = 1f;
	}

	public static void SlowHoverboard()
	{
		GTPlayer.Instance.hoverboardPaddleBoostMax = 3.5f;
		GTPlayer.Instance.hoverboardPaddleBoostMultiplier = 0.025f;
		GTPlayer.Instance.hoverboardBoostGracePeriod = 3f;
		GTPlayer.Instance.hoverTiltAdjustsForwardFactor = 0.1f;
	}

	public static void FixHoverboard()
	{
		GTPlayer.Instance.hoverboardPaddleBoostMax = 10f;
		GTPlayer.Instance.hoverboardPaddleBoostMultiplier = 0.1f;
		GTPlayer.Instance.hoverboardBoostGracePeriod = 1f;
		GTPlayer.Instance.hoverTiltAdjustsForwardFactor = 0.2f;
	}

	public static IEnumerator DisableHoverboard()
	{
		yield return (object)new WaitForSeconds(0.3f);
		GTPlayer.Instance.SetHoverActive(false);
		VRRig.LocalRig.hoverboardVisual.SetNotHeld();
	}

	public static void HoverboardScreenTarget(VRRig rig, Color color)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		if (DisableHoverboardCoroutine != null)
		{
			((MonoBehaviour)CoroutineManager.instance).StopCoroutine(DisableHoverboardCoroutine);
		}
		DisableHoverboardCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(DisableHoverboard());
		Vector3 angularVelocity = GTExt.GetOrAddComponent<GorillaVelocityEstimator>(rig.headMesh).angularVelocity;
		Vector3 val = rig.headMesh.transform.TransformPoint(-0.3f, 0.1f, 0.3725f) + rig.LatestVelocity() * 0.5f;
		Quaternion val2 = rig.headMesh.transform.rotation * Quaternion.Euler(angularVelocity * (18f / MathF.PI)) * Quaternion.Euler(0f, 90f, 270f);
		((Behaviour)VRRig.LocalRig).enabled = false;
		((Component)VRRig.LocalRig).transform.position = val - Vector3.up * 0.5f;
		HoverboardVisual hoverboardVisual = VRRig.LocalRig.hoverboardVisual;
		hoverboardVisual.SetIsHeld(true, hoverboardVisual.NominalParentTransform.InverseTransformPoint(val), GTExt.InverseTransformRotation(hoverboardVisual.NominalParentTransform, val2), color);
		GTPlayer.Instance.SetHoverActive(false);
		hoverboardVisual.interpolatedLocalPosition = hoverboardVisual.NominalLocalPosition;
		hoverboardVisual.interpolatedLocalRotation = hoverboardVisual.NominalLocalRotation;
		GTPlayer.Instance.SetHoverboardPosRot(val, val2);
	}

	public static void HoverboardScreenGun(Color color)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				HoverboardScreenTarget(Main.lockTarget, color);
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void HoverboardScreenAll(Color color)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		SerializePatch.OverrideSerialization = delegate
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Expected O, but got Unknown
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			if (PhotonNetwork.InRoom)
			{
				Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
				Vector3 val = ((Component)VRRig.LocalRig).transform.position;
				NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
				foreach (NetPlayer val2 in playerListOthers)
				{
					HoverboardScreenTarget(RigUtilities.GetVRRigFromPlayer(val2), color);
					PhotonView photonView = VRRig.LocalRig.GetPhotonView();
					RaiseEventOptions val3 = new RaiseEventOptions();
					val3.TargetActors = new int[1] { val2.ActorNumber };
					Main.SendSerialize(photonView, val3);
				}
				Main.RPCProtection();
				((Behaviour)VRRig.LocalRig).enabled = true;
				((Component)VRRig.LocalRig).transform.position = val;
				return false;
			}
			return true;
		};
	}

	public static void SpawnHoverboard()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		BetaDropBoard(((Component)VRRig.LocalRig).transform.position, ((Component)VRRig.LocalRig).transform.rotation, Vector3.zero, Vector3.zero, RandomUtilities.RandomColor());
	}

	public static void HoverboardSpam()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab && Time.time > hoverboardSpamDelay)
		{
			hoverboardSpamDelay = Time.time + 0.5f;
			BetaDropBoard(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.rotation, Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * Main.ShootStrength, Vector3.zero, RandomUtilities.RandomColor());
		}
	}

	public static void OrbitHoverboards()
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > hoverboardSpamDelay)
		{
			hoverboardSpamDelay = Time.time + 0.25f;
			float num = 0f;
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(MathF.Cos(num + (float)Time.frameCount / 30f) * 2f, 1f, MathF.Sin(num + (float)Time.frameCount / 30f) * 2f);
			num = -25f;
			Vector3 val2 = default(Vector3);
			((Vector3)(ref val2))._002Ector(MathF.Cos(num + (float)Time.frameCount / 30f) * 2f, 1f, MathF.Sin(num + (float)Time.frameCount / 30f) * 2f);
			Vector3 val3 = ((Component)GorillaTagger.Instance.headCollider).transform.position + val;
			Vector3 val4 = ((Component)GorillaTagger.Instance.headCollider).transform.position - val;
			Quaternion rotation = Quaternion.Euler(((Vector3)(ref val4)).normalized);
			val4 = val2 - val;
			BetaDropBoard(val3, rotation, ((Vector3)(ref val4)).normalized * 6.5f, new Vector3(0f, 360f, 0f), RandomUtilities.RandomColor());
			num = 180f;
			((Vector3)(ref val))._002Ector(MathF.Cos(num + (float)Time.frameCount / 30f) * 2f, 1f, MathF.Sin(num + (float)Time.frameCount / 30f) * 2f);
			num = 155f;
			((Vector3)(ref val2))._002Ector(MathF.Cos(num + (float)Time.frameCount / 30f) * 2f, 1f, MathF.Sin(num + (float)Time.frameCount / 30f) * 2f);
			Vector3 val5 = ((Component)GorillaTagger.Instance.headCollider).transform.position + val;
			val4 = ((Component)GorillaTagger.Instance.headCollider).transform.position - val;
			Quaternion rotation2 = Quaternion.Euler(((Vector3)(ref val4)).normalized);
			val4 = val2 - val;
			BetaDropBoard(val5, rotation2, ((Vector3)(ref val4)).normalized * 6.5f, new Vector3(0f, 360f, 0f), RandomUtilities.RandomColor());
		}
	}

	public static void StartAllRaces()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		Race[] races = RacingManager.instance.races;
		foreach (Race val in races)
		{
			if ((int)val.racingState == 0)
			{
				val.Button_StartRace(5);
			}
		}
	}

	public static void RainbowHoverboard()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)VRRig.LocalRig.hoverboardVisual != (Object)null && VRRig.LocalRig.hoverboardVisual.IsHeld)
		{
			float num = (float)Time.frameCount / 180f % 1f;
			Color val = Color.HSVToRGB(num, 1f, 1f);
			VRRig.LocalRig.hoverboardVisual.SetIsHeld(VRRig.LocalRig.hoverboardVisual.IsLeftHanded, VRRig.LocalRig.hoverboardVisual.NominalLocalPosition, VRRig.LocalRig.hoverboardVisual.NominalLocalRotation, val);
		}
	}

	public static void StrobeHoverboard()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)VRRig.LocalRig.hoverboardVisual != (Object)null && VRRig.LocalRig.hoverboardVisual.IsHeld)
		{
			if (Time.time > flashDelay)
			{
				flashDelay = Time.time + 0.1f;
				flashColor = !flashColor;
			}
			Color val = (flashColor ? Color.white : Color.black);
			VRRig.LocalRig.hoverboardVisual.SetIsHeld(VRRig.LocalRig.hoverboardVisual.IsLeftHanded, VRRig.LocalRig.hoverboardVisual.NominalLocalPosition, VRRig.LocalRig.hoverboardVisual.NominalLocalRotation, val);
		}
	}

	public static void RandomHoverboard()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)VRRig.LocalRig.hoverboardVisual != (Object)null && VRRig.LocalRig.hoverboardVisual.IsHeld)
		{
			VRRig.LocalRig.hoverboardVisual.SetIsHeld(VRRig.LocalRig.hoverboardVisual.IsLeftHanded, VRRig.LocalRig.hoverboardVisual.NominalLocalPosition, VRRig.LocalRig.hoverboardVisual.NominalLocalRotation, RandomUtilities.RandomColor());
		}
	}

	public static void ModifyGliderSpeed(float pullUpLiftBonus, float dragVsSpeedDragFactor)
	{
		GliderHoldable[] allType = Main.GetAllType<GliderHoldable>(5f);
		foreach (GliderHoldable val in allType)
		{
			val.pullUpLiftBonus = pullUpLiftBonus;
			val.dragVsSpeedDragFactor = dragVsSpeedDragFactor;
		}
	}

	public static void FixGliderSpeed()
	{
		GliderHoldable[] allType = Main.GetAllType<GliderHoldable>(5f);
		foreach (GliderHoldable val in allType)
		{
			val.pullUpLiftBonus = 0.1f;
			val.dragVsSpeedDragFactor = 0.2f;
		}
	}

	public static void RopeGrabReach()
	{
		GorillaHandClimber[] array = (GorillaHandClimber[])(object)new GorillaHandClimber[2]
		{
			((EquipmentInteractor)EquipmentInteractor.instance).LeftClimber,
			((EquipmentInteractor)EquipmentInteractor.instance).rightClimber
		};
		foreach (GorillaHandClimber val in array)
		{
			Collider col = val.col;
			((SphereCollider)((col is SphereCollider) ? col : null)).radius = 0.5f;
		}
	}

	public static void DebugSlingshotAimbot()
	{
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)VRRig.LocalRig.GetSlingshot() == (Object)null) && !(((TransferrableObject)VRRig.LocalRig.GetSlingshot()).InLeftHand() ? (Main.leftTrigger > 0.5f) : (Main.rightTrigger > 0.5f)))
		{
			List<NetPlayer> infected = GameModeUtilities.InfectedList();
			List<VRRig> source = (from rig in VRRigCache.ActiveRigs
				where !rig.isLocal
				where !infected.Contains(RigUtilities.GetPlayerFromVRRig(rig))
				select rig).ToList();
			Transform head = ((Component)GorillaTagger.Instance.headCollider).transform;
			VRRig val = (from x in source.Where((VRRig rig) => (Object)(object)rig != (Object)null).Select(delegate(VRRig rig)
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Unknown result type (might be due to invalid IL or missing references)
					//IL_001c: Unknown result type (might be due to invalid IL or missing references)
					//IL_001f: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0035: Unknown result type (might be due to invalid IL or missing references)
					Vector3 val2 = ((Component)rig).transform.position - head.position;
					return new
					{
						Rig = rig,
						ToRig = ((Vector3)(ref val2)).normalized,
						Distance = Vector3.Distance(head.position, ((Component)rig).transform.position)
					};
				})
				orderby Vector3.Angle(head.forward, x.ToRig), x.Distance
				select x.Rig).FirstOrDefault();
			if (!((Object)(object)val == (Object)null))
			{
				Visuals.VisualizeAura(val.headMesh.transform.position, 0.1f, Color.green, -91752L);
			}
		}
	}

	public static void AngryBirdsSounds()
	{
		if (!Main.dynamicSounds)
		{
			return;
		}
		ProjectileWeapon slingshot = VRRig.LocalRig.GetSlingshot();
		Slingshot val = (Slingshot)(object)((slingshot is Slingshot) ? slingshot : null);
		if (!Object.op_Implicit((Object)(object)val))
		{
			return;
		}
		if (val.InDrawingState() && !lastDrawing)
		{
			AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Mods/Fun/AngryBirds/drawing.ogg", "Audio/Mods/Fun/AngryBirds/drawing.ogg", delegate(AudioClip clip)
			{
				clip.Play((float)Main.buttonClickVolume / 10f);
			});
		}
		lastDrawing = val.InDrawingState();
	}

	public static void SlingshotSelf(bool active = true)
	{
		try
		{
			ProjectileWeapon val = VRRig.LocalRig.projectileWeapon;
			if (oldIndex == -1)
			{
				oldIndex = VRRig.LocalRig.ActiveTransferrableObjectIndex(0);
			}
			if ((Object)(object)val == (Object)null)
			{
				val = (ProjectileWeapon)(object)((Component)((Component)VRRig.LocalRig).transform.Find("rig/body_pivot/Slingshot Chest Snap/DropZoneAnchor/Slingshot")).GetComponent<Slingshot>();
			}
			VRRig.LocalRig.SetActiveTransferrableObjectIndex(0, active ? 212 : oldIndex);
			((Component)val).gameObject.SetActive(active);
		}
		catch
		{
		}
	}

	public static void SlingshotHelper()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		ProjectileWeapon slingshot = VRRig.LocalRig.GetSlingshot();
		Slingshot val = (Slingshot)(object)((slingshot is Slingshot) ? slingshot : null);
		if (!((Object)(object)val == (Object)null) && (val.ForLeftHandSlingshot() ? Main.rightGrab : Main.leftGrab))
		{
			((TransferrableObject)val).itemState = (ItemStates)(val.ForLeftHandSlingshot() ? 4 : 8);
		}
	}

	public static void SlingshotTriggerBot()
	{
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		if ((Object)(object)paintbrawlTriggerLine != (Object)null)
		{
			if (!((Component)paintbrawlTriggerLine).gameObject.activeSelf)
			{
				paintbrawlTriggerLine = null;
				Object.Destroy((Object)(object)((Component)paintbrawlTriggerLine).gameObject);
			}
			else
			{
				((Component)paintbrawlTriggerLine).gameObject.SetActive(false);
			}
		}
		ProjectileWeapon slingshot = VRRig.LocalRig.GetSlingshot();
		Slingshot val = (Slingshot)(object)((slingshot is Slingshot) ? slingshot : null);
		if ((Object)(object)val == (Object)null || !val.InDrawingState())
		{
			return;
		}
		if ((Object)(object)paintbrawlTriggerLine == (Object)null)
		{
			GameObject val2 = new GameObject("LineObject");
			paintbrawlTriggerLine = val2.AddComponent<LineRenderer>();
			paintbrawlTriggerLine.positionCount = 25;
		}
		((Component)paintbrawlTriggerLine).gameObject.SetActive(true);
		paintbrawlTriggerLine.startColor = Color.black;
		paintbrawlTriggerLine.endColor = Color.black;
		Vector3 val3 = val.drawingHand.transform.position;
		Vector3 val4 = val.centerOrigin.position - val.drawingHand.transform.position;
		Vector3 val5 = val3 + ((Vector3)(ref val4)).normalized * ((((EquipmentInteractor)EquipmentInteractor.instance).grabRadius - val.dummyProjectileColliderRadius) * (val.dummyProjectileInitialScale * Mathf.Abs(((Component)val).transform.lossyScale.x)));
		Vector3 launchVelocity = ((ProjectileWeapon)val).GetLaunchVelocity();
		Visuals.DrawTrajectory(val5, launchVelocity, paintbrawlTriggerLine, Main.NoInvisLayerMask(), Vector3.down * 10.79f);
		((Renderer)paintbrawlTriggerLine).enabled = false;
		if (paintbrawlTriggerLine.startColor == Color.green && Time.time > triggerBotDelay)
		{
			triggerBotDelay = Time.time + 0.5f;
		}
		if (Time.time < triggerBotDelay)
		{
			if (val.ForLeftHandSlingshot())
			{
				((ControllerInputPoller)ControllerInputPoller.instance).rightControllerGripFloat = 0f;
				((ControllerInputPoller)ControllerInputPoller.instance).rightGrab = false;
			}
			else
			{
				((ControllerInputPoller)ControllerInputPoller.instance).leftControllerGripFloat = 0f;
				((ControllerInputPoller)ControllerInputPoller.instance).leftGrab = false;
			}
		}
	}

	public static IEnumerator ReturnRig()
	{
		yield return (object)new WaitForSeconds(0.2f);
		((Behaviour)VRRig.LocalRig).enabled = true;
		BugCoroutine = null;
	}

	public static ThrowableBug GetBugObject(string name)
	{
		GameObject val = ((name == "Firefly") ? ((Component)Firefly).gameObject : Main.GetObject(name));
		if ((Object)(object)val == (Object)null)
		{
			return null;
		}
		ThrowableBug component = val.GetComponent<ThrowableBug>();
		return component ?? null;
	}

	public static ThrowableBug GetBug(string name)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Invalid comparison between Unknown and I4
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Invalid comparison between Unknown and I4
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Invalid comparison between Unknown and I4
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Expected I4, but got Unknown
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		ThrowableBug bugObject = GetBugObject(name);
		if ((Object)(object)bugObject == (Object)null)
		{
			return null;
		}
		GameObject gameObject = ((Component)bugObject).gameObject;
		if (!PhotonNetwork.InRoom)
		{
			return bugObject;
		}
		RequestableOwnershipGuard guard = ((TransferrableObject)bugObject).worldShareableInstance.guard;
		if ((Object)(object)guard == (Object)null)
		{
			return null;
		}
		if (!((TransferrableObject)bugObject).IsMyItem())
		{
			if ((int)((TransferrableObject)bugObject).currentState != 128 && (int)((TransferrableObject)bugObject).currentState > 0)
			{
				return null;
			}
			((Behaviour)VRRig.LocalRig).enabled = true;
			if (Vector3.SqrMagnitude(gameObject.transform.position - ((Component)GorillaTagger.Instance.bodyCollider).transform.position) > 15f)
			{
				((Behaviour)VRRig.LocalRig).enabled = false;
				((Component)VRRig.LocalRig).transform.position = gameObject.transform.position;
				if (BugCoroutine != null)
				{
					((MonoBehaviour)CoroutineManager.instance).StopCoroutine(BugCoroutine);
				}
				BugCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(ReturnRig());
			}
			if (Vector3.SqrMagnitude(gameObject.transform.position - Main.ServerPos) > 15f)
			{
				return null;
			}
			if (Time.time < getOwnershipDelay)
			{
				return null;
			}
			getOwnershipDelay = Time.time + 0.5f;
			NetworkingState guardState = guard.currentState;
			Action b = null;
			if ((int)guardState < 3)
			{
				b = delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					guard.currentState = guardState;
				};
			}
			NetworkingState currentState = guard.currentState;
			NetworkingState val = currentState;
			switch ((int)val)
			{
			case 0:
				return null;
			case 1:
				guard.ownershipDenied = (Action)Delegate.Combine(guard.ownershipDenied, b);
				guard.currentState = (NetworkingState)5;
				return null;
			case 2:
				guard.ownershipDenied = (Action)Delegate.Combine(guard.ownershipDenied, b);
				guard.ownershipRequestNonce = Guid.NewGuid().ToString();
				guard.currentState = (NetworkingState)4;
				guard.netView.SendRPC("OwnershipRequested", guard.actualOwner, new object[1] { guard.ownershipRequestNonce });
				return null;
			case 3:
			case 4:
			case 5:
			case 6:
				guard.ownershipDenied = (Action)Delegate.Combine(guard.ownershipDenied, b);
				return null;
			default:
				return null;
			}
		}
		if (BugCoroutine != null)
		{
			((MonoBehaviour)CoroutineManager.instance).StopCoroutine(BugCoroutine);
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
		((TransferrableObject)bugObject).worldShareableInstance.transferableObjectState = (PositionState)128;
		return bugObject;
	}

	public static void BugSpam()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I4
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Invalid comparison between Unknown and I4
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Expected O, but got Unknown
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		ThrowableBug bugObject = GetBugObject("Floating Bug Holdable");
		if ((!((TransferrableObject)bugObject).IsMyItem() || ((int)((TransferrableObject)bugObject).currentState != 128 && (int)((TransferrableObject)bugObject).currentState != 0)) && (Object)(object)((Component)bugObject).GetComponent<ClampPosition>() != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)bugObject).GetComponent<ClampPosition>());
		}
		ThrowableBug bugObject2 = GetBugObject("Firefly");
		if ((!((TransferrableObject)bugObject2).IsMyItem() || ((int)((TransferrableObject)bugObject2).currentState != 128 && (int)((TransferrableObject)bugObject2).currentState != 0)) && (Object)(object)((Component)bugObject2).GetComponent<ClampPosition>() != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)bugObject2).GetComponent<ClampPosition>());
		}
		ThrowableBug bug = GetBug("Floating Bug Holdable");
		ThrowableBug val = (((Object)(object)bug != (Object)null) ? GetBug("Firefly") : bug);
		if (!Main.rightGrab || !(Time.time > bugSpamDelay))
		{
			return;
		}
		bugSpamToggle = !bugSpamToggle;
		bugSpamDelay = Time.time + 0.5f;
		ThrowableBug val2 = (bugSpamToggle ? bug : val);
		GameObject val3 = new GameObject("Seralyth_BugSpamObject");
		val3.transform.localScale = Vector3.one * 0.2f;
		val3.layer = 3;
		if (Buttons.GetIndex("Bug Colliders").enabled)
		{
			SphereCollider val4 = val3.AddComponent<SphereCollider>();
			if (Buttons.GetIndex("Bouncy Bug").enabled)
			{
				((Collider)val4).material.bounciness = 1f;
				((Collider)val4).material.bounceCombine = (PhysicsMaterialCombine)3;
				((Collider)val4).material.dynamicFriction = 0f;
			}
		}
		val3.transform.position = GorillaTagger.Instance.rightHandTransform.position + Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * 0.5f;
		val3.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
		Rigidbody val5 = val3.AddComponent<Rigidbody>();
		val5.linearVelocity = Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * Main.ShootStrength;
		val5.angularVelocity = RandomUtilities.RandomVector3(100f);
		val5.useGravity = !Buttons.GetIndex("Zero Gravity Bugs").enabled;
		GTExt.GetOrAddComponent<ClampPosition>(((Component)val2).gameObject).targetTransform = val3.transform;
		val3.AddComponent<DestroyOnRest>();
		Object.Destroy((Object)(object)val3, 30f);
	}

	public static void CameraSpam()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.rightGrab || !(Time.time > cameraSpamDelay))
		{
			return;
		}
		cameraSpamDelay = Time.time + 0.25f;
		cameraSpamType = !cameraSpamType;
		LckSocialCamera val = (cameraSpamType ? LckSocialCameraManager.Instance._networkedCococam : LckSocialCameraManager.Instance._networkedTablet);
		GameObject val2 = new GameObject("Seralyth_CameraSpamObject");
		val2.transform.localScale = Vector3.one * 0.2f;
		val2.layer = 3;
		if (Buttons.GetIndex("Bug Colliders").enabled)
		{
			SphereCollider val3 = val2.AddComponent<SphereCollider>();
			if (Buttons.GetIndex("Bouncy Bug").enabled)
			{
				((Collider)val3).material.bounciness = 1f;
				((Collider)val3).material.bounceCombine = (PhysicsMaterialCombine)3;
				((Collider)val3).material.dynamicFriction = 0f;
			}
		}
		val2.transform.position = GorillaTagger.Instance.rightHandTransform.position + Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * 0.5f;
		val2.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
		Rigidbody val4 = val2.AddComponent<Rigidbody>();
		val4.linearVelocity = Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * Main.ShootStrength;
		val4.angularVelocity = RandomUtilities.RandomVector3(100f);
		val4.useGravity = !Buttons.GetIndex("Zero Gravity Bugs").enabled;
		val.visible = true;
		val.recording = true;
		val.m_CameraVisuals.SetNetworkedVisualsActive(true);
		val.m_CameraVisuals.SetRecordingState(true);
		GTExt.GetOrAddComponent<ClampPosition>(((Component)val).gameObject).targetTransform = val2.transform;
		val2.AddComponent<DestroyOnRest>();
		Object.Destroy((Object)(object)val2, 30f);
	}

	public static void EverythingSpam()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I4
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Invalid comparison between Unknown and I4
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Expected O, but got Unknown
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Expected O, but got Unknown
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0843: Unknown result type (might be due to invalid IL or missing references)
		//IL_0852: Unknown result type (might be due to invalid IL or missing references)
		//IL_0861: Unknown result type (might be due to invalid IL or missing references)
		//IL_086b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0870: Unknown result type (might be due to invalid IL or missing references)
		//IL_087f: Unknown result type (might be due to invalid IL or missing references)
		//IL_089b: Unknown result type (might be due to invalid IL or missing references)
		//IL_08aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Expected O, but got Unknown
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0698: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Expected O, but got Unknown
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_073e: Unknown result type (might be due to invalid IL or missing references)
		//IL_074d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0757: Unknown result type (might be due to invalid IL or missing references)
		//IL_075c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0778: Unknown result type (might be due to invalid IL or missing references)
		//IL_0798: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		ThrowableBug bugObject = GetBugObject("Floating Bug Holdable");
		if ((!((TransferrableObject)bugObject).IsMyItem() || ((int)((TransferrableObject)bugObject).currentState != 128 && (int)((TransferrableObject)bugObject).currentState != 0)) && (Object)(object)((Component)bugObject).GetComponent<ClampPosition>() != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)bugObject).GetComponent<ClampPosition>());
		}
		ThrowableBug bugObject2 = GetBugObject("Firefly");
		if ((!((TransferrableObject)bugObject2).IsMyItem() || ((int)((TransferrableObject)bugObject2).currentState != 128 && (int)((TransferrableObject)bugObject2).currentState != 0)) && (Object)(object)((Component)bugObject2).GetComponent<ClampPosition>() != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)bugObject2).GetComponent<ClampPosition>());
		}
		ThrowableBug bug = GetBug("Floating Bug Holdable");
		ThrowableBug val = (((Object)(object)bug != (Object)null) ? GetBug("Firefly") : bug);
		string projectileName = Projectiles.ProjectileObjectNames[Projectiles.projMode * 2];
		if (!Main.rightGrab || !(Time.time > everythingSpamDelay))
		{
			return;
		}
		SnowballThrowable projectile = Main.GetProjectile(projectileName);
		projectile.SetSnowballActiveLocal(true);
		((MonoBehaviour)CoroutineManager.instance).StartCoroutine(Projectiles.DisableProjectile(projectile));
		if (Overpowered.DisableCoroutine != null)
		{
			((MonoBehaviour)CoroutineManager.instance).StopCoroutine(Overpowered.DisableCoroutine);
		}
		Overpowered.DisableCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(Overpowered.DisableSnowball(rigDisabled: false));
		Main.GetProjectile(Projectiles.SnowballName + "RightAnchor").SetSnowballActiveLocal(true);
		everythingSpamDelay = Time.time + 0.0625f;
		objectIndex++;
		objectIndex %= 8;
		switch (objectIndex)
		{
		case 0:
		{
			ThrowableBug val8 = bug;
			GameObject val9 = new GameObject("Seralyth_BugSpamObject");
			val9.transform.localScale = Vector3.one * 0.2f;
			val9.layer = 3;
			if (Buttons.GetIndex("Bug Colliders").enabled)
			{
				SphereCollider val10 = val9.AddComponent<SphereCollider>();
				if (Buttons.GetIndex("Bouncy Bug").enabled)
				{
					((Collider)val10).material.bounciness = 1f;
					((Collider)val10).material.bounceCombine = (PhysicsMaterialCombine)3;
					((Collider)val10).material.dynamicFriction = 0f;
				}
			}
			val9.transform.position = GorillaTagger.Instance.rightHandTransform.position + Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * 0.5f;
			val9.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
			Rigidbody val11 = val9.AddComponent<Rigidbody>();
			val11.linearVelocity = Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * Main.ShootStrength;
			val11.angularVelocity = RandomUtilities.RandomVector3(100f);
			val11.useGravity = !Buttons.GetIndex("Zero Gravity Bugs").enabled;
			GTExt.GetOrAddComponent<ClampPosition>(((Component)val8).gameObject).targetTransform = val9.transform;
			val9.AddComponent<DestroyOnRest>();
			Object.Destroy((Object)(object)val9, 30f);
			break;
		}
		case 1:
		{
			ThrowableBug val12 = val;
			GameObject val13 = new GameObject("Seralyth_FireflySpamObject");
			val13.transform.localScale = Vector3.one * 0.2f;
			val13.layer = 3;
			if (Buttons.GetIndex("Bug Colliders").enabled)
			{
				SphereCollider val14 = val13.AddComponent<SphereCollider>();
				if (Buttons.GetIndex("Bouncy Bug").enabled)
				{
					((Collider)val14).material.bounciness = 1f;
					((Collider)val14).material.bounceCombine = (PhysicsMaterialCombine)3;
					((Collider)val14).material.dynamicFriction = 0f;
				}
			}
			val13.transform.position = GorillaTagger.Instance.rightHandTransform.position + Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * 0.5f;
			val13.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
			Rigidbody val15 = val13.AddComponent<Rigidbody>();
			val15.linearVelocity = Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * Main.ShootStrength;
			val15.angularVelocity = RandomUtilities.RandomVector3(100f);
			val15.useGravity = !Buttons.GetIndex("Zero Gravity Bugs").enabled;
			GTExt.GetOrAddComponent<ClampPosition>(((Component)val12).gameObject).targetTransform = val13.transform;
			val13.AddComponent<DestroyOnRest>();
			Object.Destroy((Object)(object)val13, 30f);
			break;
		}
		case 2:
		{
			if (!PhotonNetwork.InRoom)
			{
				break;
			}
			LckSocialCamera networkedCococam = LckSocialCameraManager.Instance._networkedCococam;
			GameObject val5 = new GameObject("Seralyth_CameraSpamObject");
			val5.transform.localScale = Vector3.one * 0.2f;
			val5.layer = 3;
			if (Buttons.GetIndex("Bug Colliders").enabled)
			{
				SphereCollider val6 = val5.AddComponent<SphereCollider>();
				if (Buttons.GetIndex("Bouncy Bug").enabled)
				{
					((Collider)val6).material.bounciness = 1f;
					((Collider)val6).material.bounceCombine = (PhysicsMaterialCombine)3;
					((Collider)val6).material.dynamicFriction = 0f;
				}
			}
			val5.transform.position = GorillaTagger.Instance.rightHandTransform.position + Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * 0.5f;
			val5.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
			Rigidbody val7 = val5.AddComponent<Rigidbody>();
			val7.linearVelocity = Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * Main.ShootStrength;
			val7.angularVelocity = RandomUtilities.RandomVector3(100f);
			val7.useGravity = !Buttons.GetIndex("Zero Gravity Bugs").enabled;
			networkedCococam.visible = true;
			networkedCococam.recording = true;
			networkedCococam.m_CameraVisuals.SetNetworkedVisualsActive(true);
			networkedCococam.m_CameraVisuals.SetRecordingState(true);
			GTExt.GetOrAddComponent<ClampPosition>(((Component)networkedCococam).gameObject).targetTransform = val5.transform;
			val5.AddComponent<DestroyOnRest>();
			Object.Destroy((Object)(object)val5, 30f);
			break;
		}
		case 3:
		{
			if (!PhotonNetwork.InRoom)
			{
				break;
			}
			LckSocialCamera networkedTablet = LckSocialCameraManager.Instance._networkedTablet;
			GameObject val2 = new GameObject("Seralyth_CameraSpamObject");
			val2.transform.localScale = Vector3.one * 0.2f;
			val2.layer = 3;
			if (Buttons.GetIndex("Bug Colliders").enabled)
			{
				SphereCollider val3 = val2.AddComponent<SphereCollider>();
				if (Buttons.GetIndex("Bouncy Bug").enabled)
				{
					((Collider)val3).material.bounciness = 1f;
					((Collider)val3).material.bounceCombine = (PhysicsMaterialCombine)3;
					((Collider)val3).material.dynamicFriction = 0f;
				}
			}
			val2.transform.position = GorillaTagger.Instance.rightHandTransform.position + Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * 0.5f;
			val2.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
			Rigidbody val4 = val2.AddComponent<Rigidbody>();
			val4.linearVelocity = Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * Main.ShootStrength;
			val4.angularVelocity = RandomUtilities.RandomVector3(100f);
			val4.useGravity = !Buttons.GetIndex("Zero Gravity Bugs").enabled;
			networkedTablet.visible = true;
			networkedTablet.recording = true;
			networkedTablet.m_CameraVisuals.SetNetworkedVisualsActive(true);
			networkedTablet.m_CameraVisuals.SetRecordingState(true);
			GTExt.GetOrAddComponent<ClampPosition>(((Component)networkedTablet).gameObject).targetTransform = val2.transform;
			val2.AddComponent<DestroyOnRest>();
			Object.Destroy((Object)(object)val2, 30f);
			break;
		}
		case 4:
		case 5:
			BetaDropBoard(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.rotation, Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * Main.ShootStrength, Vector3.zero, RandomUtilities.RandomColor());
			break;
		case 6:
			Projectiles.BetaFireProjectile(projectileName, GorillaTagger.Instance.rightHandTransform.position, Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * Main.ShootStrength, RandomUtilities.RandomColor());
			break;
		case 7:
			Overpowered.BetaSpawnSnowball(GorillaTagger.Instance.rightHandTransform.position, Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * Main.ShootStrength, 0);
			break;
		}
	}

	public static void DisableCameraSpam()
	{
		LckSocialCamera networkedCococam = LckSocialCameraManager.Instance._networkedCococam;
		if ((Object)(object)((Component)networkedCococam).GetComponent<ClampPosition>() != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)networkedCococam).GetComponent<ClampPosition>());
		}
	}

	public static void DisableEverythingSpam()
	{
		DisableBugSpam();
		DisableCameraSpam();
	}

	public static void DisableBugSpam()
	{
		ThrowableBug bugObject = GetBugObject("Floating Bug Holdable");
		if ((Object)(object)((Component)bugObject).GetComponent<ClampPosition>() != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)bugObject).GetComponent<ClampPosition>());
		}
		ThrowableBug bugObject2 = GetBugObject("Firefly");
		if ((Object)(object)((Component)bugObject2).GetComponent<ClampPosition>() != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)bugObject2).GetComponent<ClampPosition>());
		}
	}

	public static void BugPhallus()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		ThrowableBug bug = GetBug("Floating Bug Holdable");
		ThrowableBug bug2 = GetBug("Firefly");
		if ((Object)(object)bug != (Object)null && (Object)(object)bug2 != (Object)null)
		{
			((Component)bug).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + ((Component)GorillaTagger.Instance.bodyCollider).transform.TransformDirection(new Vector3(0f, -0.22f, 0.123f));
			((Component)bug2).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + ((Component)GorillaTagger.Instance.bodyCollider).transform.TransformDirection(new Vector3(0f, -0.22f, 0.24f));
			((Component)bug).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation * Quaternion.Euler(0f, 270f, 0f);
			((Component)bug2).transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation * Quaternion.Euler(0f, 90f, 0f);
		}
	}

	public static void BugPhallusGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				ThrowableBug bug = GetBug("Floating Bug Holdable");
				ThrowableBug bug2 = GetBug("Firefly");
				if ((Object)(object)bug != (Object)null && (Object)(object)bug2 != (Object)null)
				{
					((Component)bug).transform.position = ((Component)Main.lockTarget).transform.position + ((Component)Main.lockTarget).transform.TransformDirection(new Vector3(0f, -0.4f, 0.123f));
					((Component)bug2).transform.position = ((Component)Main.lockTarget).transform.position + ((Component)Main.lockTarget).transform.TransformDirection(new Vector3(0f, -0.4f, 0.24f));
					((Component)bug).transform.rotation = ((Component)Main.lockTarget).transform.rotation * Quaternion.Euler(0f, 270f, 0f);
					((Component)bug2).transform.rotation = ((Component)Main.lockTarget).transform.rotation * Quaternion.Euler(0f, 90f, 0f);
				}
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void BugVibrateGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				ThrowableBug bug = GetBug("Floating Bug Holdable");
				ThrowableBug val2 = (((Object)(object)bug != (Object)null) ? GetBug("Firefly") : null);
				if ((Object)(object)bug != (Object)null)
				{
					((Component)bug).transform.position = Main.lockTarget.leftHandTransform.position;
					((Component)bug).transform.rotation = RandomUtilities.RandomQuaternion();
				}
				if ((Object)(object)val2 != (Object)null)
				{
					((Component)val2).transform.position = Main.lockTarget.rightHandTransform.position;
					((Component)val2).transform.rotation = RandomUtilities.RandomQuaternion();
				}
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
		}
	}

	public static void BugVibrateAll()
	{
		ThrowableBug bug = GetBug("Floating Bug Holdable");
		if ((Object)(object)bug != (Object)null)
		{
			GetBug("Firefly");
		}
	}

	public static void EnableBugVibrateAll()
	{
		SerializePatch.OverrideSerialization = delegate
		{
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0410: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Expected O, but got Unknown
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Expected O, but got Unknown
			ThrowableBug bug = GetBug("Floating Bug Holdable");
			ThrowableBug val = (((Object)(object)bug != (Object)null) ? GetBug("Firefly") : bug);
			object obj;
			if (bug == null)
			{
				obj = null;
			}
			else
			{
				ThrowableBugReliableState reliableState = bug.reliableState;
				obj = ((reliableState != null) ? ((Component)reliableState).gameObject : null);
			}
			PhotonView val2 = (((Object)obj != (Object)null) ? ((NetworkView)((Component)bug.reliableState).gameObject.GetComponent<GorillaNetworkTransform>()).punView : null);
			object obj2;
			if (val == null)
			{
				obj2 = null;
			}
			else
			{
				ThrowableBugReliableState reliableState2 = val.reliableState;
				obj2 = ((reliableState2 != null) ? ((Component)reliableState2).gameObject : null);
			}
			PhotonView val3 = (((Object)obj2 != (Object)null) ? ((NetworkView)((Component)val.reliableState).gameObject.GetComponent<GorillaNetworkTransform>()).punView : null);
			if ((Object)(object)val2 == (Object)null || (Object)(object)val3 == (Object)null)
			{
				return true;
			}
			Main.MassSerialize(exclude: true, ((IEnumerable<PhotonView>)(object)new PhotonView[2] { val2, val3 }).Where((PhotonView v) => (Object)(object)v != (Object)null).ToArray());
			GameObject gameObject = ((Component)bug.reliableState).gameObject;
			Vector3? obj3;
			if (gameObject == null)
			{
				obj3 = null;
			}
			else
			{
				Transform transform = gameObject.transform;
				obj3 = ((transform != null) ? new Vector3?(transform.position) : ((Vector3?)null));
			}
			Vector3 val4 = (Vector3)(((_003F?)obj3) ?? Vector3.zero);
			GameObject gameObject2 = ((Component)bug.reliableState).gameObject;
			Quaternion? obj4;
			if (gameObject2 == null)
			{
				obj4 = null;
			}
			else
			{
				Transform transform2 = gameObject2.transform;
				obj4 = ((transform2 != null) ? new Quaternion?(transform2.rotation) : ((Quaternion?)null));
			}
			Quaternion rotation = (Quaternion)(((_003F?)obj4) ?? Quaternion.identity);
			GameObject gameObject3 = ((Component)val.reliableState).gameObject;
			Vector3? obj5;
			if (gameObject3 == null)
			{
				obj5 = null;
			}
			else
			{
				Transform transform3 = gameObject3.transform;
				obj5 = ((transform3 != null) ? new Vector3?(transform3.position) : ((Vector3?)null));
			}
			Vector3 val5 = (Vector3)(((_003F?)obj5) ?? Vector3.zero);
			GameObject gameObject4 = ((Component)val.reliableState).gameObject;
			Quaternion? obj6;
			if (gameObject4 == null)
			{
				obj6 = null;
			}
			else
			{
				Transform transform4 = gameObject4.transform;
				obj6 = ((transform4 != null) ? new Quaternion?(transform4.rotation) : ((Quaternion?)null));
			}
			Quaternion rotation2 = (Quaternion)(((_003F?)obj6) ?? Quaternion.identity);
			NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
			foreach (NetPlayer val6 in playerListOthers)
			{
				VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(val6);
				if (!((Object)(object)vRRigFromPlayer == (Object)null))
				{
					if ((Object)(object)bug != (Object)null && (Object)(object)((Component)bug).transform != (Object)null && (Object)(object)vRRigFromPlayer.leftHandTransform != (Object)null && (Object)(object)val2 != (Object)null)
					{
						((Component)bug.reliableState).gameObject.transform.position = vRRigFromPlayer.leftHandTransform.position;
						((Component)bug.reliableState).gameObject.transform.rotation = RandomUtilities.RandomQuaternion();
						RaiseEventOptions val7 = new RaiseEventOptions();
						val7.TargetActors = new int[1] { val6.ActorNumber };
						Main.SendSerialize(val2, val7);
					}
					if ((Object)(object)val != (Object)null && (Object)(object)((Component)val).transform != (Object)null && (Object)(object)vRRigFromPlayer.rightHandTransform != (Object)null && (Object)(object)val3 != (Object)null)
					{
						((Component)val.reliableState).gameObject.transform.position = vRRigFromPlayer.rightHandTransform.position;
						((Component)val.reliableState).gameObject.transform.rotation = RandomUtilities.RandomQuaternion();
						RaiseEventOptions val7 = new RaiseEventOptions();
						val7.TargetActors = new int[1] { val6.ActorNumber };
						Main.SendSerialize(val3, val7);
					}
				}
			}
			if ((Object)(object)((bug != null) ? ((Component)bug).transform : null) != (Object)null)
			{
				((Component)bug.reliableState).gameObject.transform.position = val4;
				((Component)bug.reliableState).gameObject.transform.rotation = rotation;
			}
			if ((Object)(object)((val != null) ? ((Component)val).transform : null) != (Object)null)
			{
				((Component)val.reliableState).gameObject.transform.position = val5;
				((Component)val.reliableState).gameObject.transform.rotation = rotation2;
			}
			Main.RPCProtection();
			return false;
		};
	}

	public static void HolsterObject(string objectName, PositionState state)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Invalid comparison between Unknown and I4
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		ThrowableBug bug = GetBug(objectName);
		if ((Object)(object)bug != (Object)null && ((int)((TransferrableObject)bug).currentState == 128 || (int)((TransferrableObject)bug).currentState == 0))
		{
			((TransferrableObject)bug).currentState = state;
		}
	}

	public static void FreezeObject(string objectName)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		ThrowableBug bug = GetBug(objectName);
		if ((Object)(object)bug != (Object)null)
		{
			bug.bugRotationalVelocity = Quaternion.identity;
			bug.targetVelocity = Vector3.zero;
			bug.thrownVeloicity = Vector3.zero;
			bug.thrownYVelocity = 0f;
			bug.reliableState.travelingDirection = Vector3.zero;
		}
	}

	public static void SetObjectSpeed(string objectName, float speed = 1f)
	{
		ThrowableBug bug = GetBug(objectName);
		if ((Object)(object)bug != (Object)null)
		{
			bug.maxNaturalSpeed = speed;
		}
	}

	public static void PhysicalObject(string objectName)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Invalid comparison between Unknown and I4
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Invalid comparison between Unknown and I4
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		ThrowableBug bug = GetBug(objectName);
		if (!((Object)(object)bug != (Object)null))
		{
			return;
		}
		GorillaVelocityTracker orAddComponent = GTExt.GetOrAddComponent<GorillaVelocityTracker>(((Component)bug).gameObject);
		if (((int)((TransferrableObject)bug).currentState == 4 || (int)((TransferrableObject)bug).currentState == 8) && (Object)(object)((Component)bug).GetComponent<ClampPosition>() != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)bug).GetComponent<ClampPosition>());
		}
		bool flag = (int)((TransferrableObject)bug).currentState == 0 || (int)((TransferrableObject)bug).currentState == 128;
		lastInAirValues.TryGetValue(objectName, out var value);
		if (flag && !value)
		{
			GameObject val = new GameObject("Seralyth_BugSpamObject");
			val.transform.localScale = Vector3.one * 0.2f;
			val.layer = 3;
			if (Buttons.GetIndex("Bug Colliders").enabled)
			{
				SphereCollider val2 = val.AddComponent<SphereCollider>();
				if (Buttons.GetIndex("Bouncy Bug").enabled)
				{
					((Collider)val2).material.bounciness = 1f;
					((Collider)val2).material.bounceCombine = (PhysicsMaterialCombine)3;
					((Collider)val2).material.dynamicFriction = 0f;
				}
			}
			val.transform.position = ((Component)bug).transform.position;
			val.transform.rotation = ((Component)bug).transform.rotation;
			Rigidbody val3 = val.AddComponent<Rigidbody>();
			val3.linearVelocity = orAddComponent.GetAverageVelocity(true, 0f, false);
			val3.angularVelocity = bug.velocityEstimator.angularVelocity;
			val3.useGravity = !Buttons.GetIndex("Zero Gravity Bugs").enabled;
			GTExt.GetOrAddComponent<ClampPosition>(((Component)bug).gameObject).targetTransform = val.transform;
			val.AddComponent<DestroyOnRest>();
			Object.Destroy((Object)(object)val, 30f);
		}
		lastInAirValues[objectName] = flag;
	}

	public static void PhysicalCamera()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Expected O, but got Unknown
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		LckSocialCamera networkedCococam = LckSocialCameraManager.Instance._networkedCococam;
		if (!networkedCococam.visible)
		{
			((Component)networkedCococam).transform.position = GorillaTagger.Instance.rightHandTransform.position;
			((Component)networkedCococam).transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
			networkedCococam.visible = true;
			networkedCococam.recording = true;
			networkedCococam.m_CameraVisuals.SetNetworkedVisualsActive(true);
			networkedCococam.m_CameraVisuals.SetRecordingState(true);
		}
		if (!grabbingCamera)
		{
			bool flag = Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, ((Component)networkedCococam).transform.position) < 0.3f && Main.leftGrab;
			bool flag2 = Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, ((Component)networkedCococam).transform.position) < 0.3f && Main.rightGrab;
			if (flag || flag2)
			{
				grabbingCamera = true;
				grabbingHand = flag;
			}
			return;
		}
		Transform val = (grabbingHand ? GorillaTagger.Instance.leftHandTransform : GorillaTagger.Instance.rightHandTransform);
		((Component)networkedCococam).transform.position = val.position;
		((Component)networkedCococam).transform.rotation = val.rotation;
		GorillaVelocityTracker orAddComponent = GTExt.GetOrAddComponent<GorillaVelocityTracker>(((Component)networkedCococam).gameObject);
		GorillaVelocityEstimator orAddComponent2 = GTExt.GetOrAddComponent<GorillaVelocityEstimator>(((Component)networkedCococam).gameObject);
		if (!(grabbingHand ? (!Main.leftGrab) : (!Main.rightGrab)))
		{
			return;
		}
		grabbingCamera = false;
		GameObject val2 = new GameObject("Seralyth_BugSpamObject");
		val2.transform.localScale = Vector3.one * 0.2f;
		val2.layer = 3;
		if (Buttons.GetIndex("Bug Colliders").enabled)
		{
			SphereCollider val3 = val2.AddComponent<SphereCollider>();
			if (Buttons.GetIndex("Bouncy Bug").enabled)
			{
				((Collider)val3).material.bounciness = 1f;
				((Collider)val3).material.bounceCombine = (PhysicsMaterialCombine)3;
				((Collider)val3).material.dynamicFriction = 0f;
			}
		}
		val2.transform.position = ((Component)networkedCococam).transform.position;
		val2.transform.rotation = ((Component)networkedCococam).transform.rotation;
		Rigidbody val4 = val2.AddComponent<Rigidbody>();
		val4.linearVelocity = orAddComponent.GetAverageVelocity(true, 0f, false);
		val4.angularVelocity = orAddComponent2.angularVelocity;
		val4.useGravity = !Buttons.GetIndex("Zero Gravity Bugs").enabled;
		GTExt.GetOrAddComponent<ClampPosition>(((Component)networkedCococam).gameObject).targetTransform = val2.transform;
		val2.AddComponent<DestroyOnRest>();
		Object.Destroy((Object)(object)val2, 30f);
	}

	public static void ObjectToHand(string objectName)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		ThrowableBug bug = GetBug(objectName);
		if (Main.rightGrab && (Object)(object)bug != (Object)null)
		{
			((Component)bug).transform.position = GorillaTagger.Instance.rightHandTransform.position;
			((Component)bug).transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
		}
	}

	public static void GrabCamera()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab)
		{
			LckSocialCamera networkedCococam = LckSocialCameraManager.Instance._networkedCococam;
			networkedCococam.visible = true;
			networkedCococam.recording = true;
			networkedCococam.m_CameraVisuals.SetNetworkedVisualsActive(true);
			networkedCococam.m_CameraVisuals.SetRecordingState(true);
			((Component)networkedCococam).transform.position = GorillaTagger.Instance.rightHandTransform.position;
			((Component)networkedCococam).transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
		}
	}

	public static void GrabTablet()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab)
		{
			LckSocialCamera networkedTablet = LckSocialCameraManager.Instance._networkedTablet;
			networkedTablet.visible = true;
			networkedTablet.recording = true;
			networkedTablet.m_CameraVisuals.SetNetworkedVisualsActive(true);
			networkedTablet.m_CameraVisuals.SetRecordingState(true);
			((Component)networkedTablet).transform.position = GorillaTagger.Instance.rightHandTransform.position;
			((Component)networkedTablet).transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
		}
	}

	public static void GrabGliders()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.rightGrab)
		{
			return;
		}
		GliderHoldable[] allType = Main.GetAllType<GliderHoldable>(5f);
		foreach (GliderHoldable val in allType)
		{
			if (((NetworkView)val).GetView.Owner == PhotonNetwork.LocalPlayer)
			{
				((Component)val).gameObject.transform.position = GorillaTagger.Instance.rightHandTransform.position;
				((Component)val).gameObject.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
			}
			else
			{
				((NetworkHoldableObject)val).OnHover((InteractionPoint)null, (GameObject)null);
			}
		}
	}

	public static void SpamGrabBlocks()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab)
		{
			RequestCreatePiece(pieceIdSet, GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.rotation, 0);
			Main.RPCProtection();
		}
	}

	public static void BuildingBlockMinigun()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab)
		{
			RequestCreatePiece(pieceIdSet, GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.rotation, 0, null, overrideFreeze: false, forceGravity: false, Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * Main.ShootStrength);
			Main.RPCProtection();
		}
	}

	public static void DestroyObject(string objectName)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ThrowableBug bug = GetBug(objectName);
		if ((Object)(object)bug != (Object)null)
		{
			((Component)bug).transform.position = new Vector3(99999f, 99999f, 99999f);
		}
	}

	public static void DestroyCamera()
	{
		LckSocialCamera networkedCococam = LckSocialCameraManager.Instance._networkedCococam;
		networkedCococam.visible = false;
		networkedCococam.recording = false;
		networkedCococam.m_CameraVisuals.SetNetworkedVisualsActive(false);
		networkedCococam.m_CameraVisuals.SetRecordingState(false);
	}

	public static void DestroyTablet()
	{
		LckSocialCamera networkedTablet = LckSocialCameraManager.Instance._networkedTablet;
		networkedTablet.visible = false;
		networkedTablet.recording = false;
		networkedTablet.m_CameraVisuals.SetNetworkedVisualsActive(false);
		networkedTablet.m_CameraVisuals.SetRecordingState(false);
	}

	public static void RespawnGliders()
	{
		GliderHoldable[] allType = Main.GetAllType<GliderHoldable>(5f);
		foreach (GliderHoldable val in allType)
		{
			if (((NetworkView)val).GetView.Owner == PhotonNetwork.LocalPlayer)
			{
				val.Respawn();
			}
			else
			{
				((NetworkHoldableObject)val).OnHover((InteractionPoint)null, (GameObject)null);
			}
		}
	}

	public static void DestroyBlocks()
	{
		if (!(Time.time > delayer))
		{
			return;
		}
		delayer = Time.time + 0.3f;
		BuilderPiece[] array = (PhotonNetwork.IsMasterClient ? Main.GetAllType<BuilderPiece>(5f) : (from piece in Main.GetAllType<BuilderPiece>(5f)
			where ((Component)piece).gameObject.activeInHierarchy
			where Vector3.Distance(((Component)piece).transform.position, GorillaTagger.Instance.leftHandTransform.position) < 2.5f
			select piece).ToArray());
		for (int num = 0; num < 100; num++)
		{
			BuilderPiece val = array[Random.Range(0, array.Length)];
			if (((Component)val).gameObject.activeSelf)
			{
				RequestRecyclePiece(val, playFX: true, 2);
			}
		}
	}

	public static void SaveBuilderTableData()
	{
		string text = "SeralythMenu/BuilderTableData.json";
		File.WriteAllText(text, ManagerRegistry.BuilderTable.WriteTableToJson());
		string fileName = FileUtilities.GetGamePath() + "/" + text;
		Process.Start(fileName);
	}

	public static void LoadBuilderTableData()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		string path = "SeralythMenu/BuilderTableData.json";
		if (File.Exists(path))
		{
			string text = File.ReadAllText(path);
			BuilderTable builderTable = ManagerRegistry.BuilderTable;
			BuilderTable val = builderTable;
			if (val.tableData == null)
			{
				val.tableData = new BuilderTableData();
			}
			builderTable.SetIsDirty(false);
			BuilderTableData tableData = builderTable.tableData;
			tableData.numEdits++;
			text = Convert.ToBase64String(GZipStream.CompressString(text));
			SharedBlocksManager.instance.OnSavePrivateScanSuccess += builderTable.OnSaveScanSuccess;
			SharedBlocksManager.instance.OnSavePrivateScanFailed += builderTable.OnSaveScanFailure;
			SharedBlocksManager.instance.RequestSavePrivateScan(builderTable.currentSaveSlot, text);
		}
	}

	public static IEnumerator DisableThrowable(int index)
	{
		yield return (object)new WaitForSeconds(0.3f);
		DistancePatch.enabled = false;
		((Behaviour)VRRig.LocalRig).enabled = true;
		GameObject proj = ((Component)VRRig.LocalRig.myBodyDockPositions.allObjects[index]).gameObject;
		proj.SetActive(true);
		VRRig.LocalRig.myBodyDockPositions.allObjects[index].storedZone = (DropPositions)2;
		VRRig.LocalRig.myBodyDockPositions.allObjects[index].currentState = (PositionState)2;
	}

	private static void EquipCosmetic(string cosmeticName)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		((CosmeticsController)CosmeticsController.instance).ApplyCosmeticItemToSet(((CosmeticsController)CosmeticsController.instance).currentWornSet, ((CosmeticsController)CosmeticsController.instance).GetItemFromDict(cosmeticName), true, false);
		((CosmeticsController)CosmeticsController.instance).ApplyCosmeticItemToSet(VRRig.LocalRig.tryOnSet, ((CosmeticsController)CosmeticsController.instance).GetItemFromDict(cosmeticName), true, false);
		((CosmeticsController)CosmeticsController.instance).UpdateWornCosmetics(PhotonNetwork.InRoom);
		Main.RPCProtection();
	}

	public static void CheckOwnedCosmetic(string cosmeticName)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (Time.frameCount == Settings.loadingPreferencesFrame)
		{
			return;
		}
		CosmeticItem cosmetic = ((CosmeticsController)CosmeticsController.instance).GetItemFromDict(cosmeticName);
		if (Main.CosmeticsOwned.Contains(cosmeticName))
		{
			return;
		}
		if (!cosmetic.canTryOn)
		{
			Main.PromptSingle("Looks like you don't own the cosmetic required for this mod (" + Main.ToTitleCase(cosmetic.overrideDisplayName) + "), but this cosmetic is currently offsale. This mod will only work for people with cosmetic giving mods.");
		}
		else if (((CosmeticsController)CosmeticsController.instance).CurrencyBalance >= cosmetic.cost)
		{
			Main.Prompt($"Looks like you don't own the cosmetic required for this mod ({Main.ToTitleCase(cosmetic.overrideDisplayName)}), meaning it will only work in city. Would you like to purchase the cosmetic? ({cosmetic.cost}SR)", delegate
			{
				PurchaseCosmetic(cosmetic.itemName);
			});
		}
		else
		{
			Main.PromptSingle("Looks like you don't own the cosmetic required for this mod (" + Main.ToTitleCase(cosmetic.overrideDisplayName) + "), meaning it will only work in city.");
		}
	}

	internal static string GetThrowableItemName(TransferrableObject obj)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)((obj != null) ? ((Component)obj).gameObject : null) == (Object)null)
		{
			return null;
		}
		string text = ((Object)((Component)obj).gameObject).name;
		if (text.EndsWith("(Clone)"))
		{
			text = text.Substring(0, text.Length - 7);
		}
		text = text.TrimEnd('.');
		if (CosmeticsController.hasInstance && (Object)(object)((Component)obj).transform.parent != (Object)null)
		{
			CosmeticItem itemFromDict = ((CosmeticsController)CosmeticsController.instance).GetItemFromDict(text);
			if (itemFromDict.itemName != text)
			{
				string text2 = ((Object)((Component)((Component)obj).transform.parent).gameObject).name;
				if (text2.EndsWith("(Clone)"))
				{
					text2 = text2.Substring(0, text2.Length - 7);
				}
				text2 = text2.TrimEnd('.');
				CosmeticItem itemFromDict2 = ((CosmeticsController)CosmeticsController.instance).GetItemFromDict(text2);
				if (itemFromDict2.itemName == text2)
				{
					text = text2;
				}
			}
		}
		return text;
	}

	private static TransferrableObject GetThrowableAtIndex(int index)
	{
		VRRig localRig = VRRig.LocalRig;
		object obj;
		if (localRig == null)
		{
			obj = null;
		}
		else
		{
			BodyDockPositions myBodyDockPositions = localRig.myBodyDockPositions;
			obj = ((myBodyDockPositions != null) ? myBodyDockPositions.allObjects : null);
		}
		if (obj == null)
		{
			return null;
		}
		if (index < 0 || index >= VRRig.LocalRig.myBodyDockPositions.allObjects.Length)
		{
			return null;
		}
		return VRRig.LocalRig.myBodyDockPositions.allObjects[index];
	}

	public static void CheckOwnedThrowable(int index)
	{
		if (Time.frameCount == Settings.loadingPreferencesFrame)
		{
			return;
		}
		TransferrableObject throwableAtIndex = GetThrowableAtIndex(index);
		if ((Object)(object)throwableAtIndex == (Object)null)
		{
			return;
		}
		string throwableItemName = GetThrowableItemName(throwableAtIndex);
		if (throwableItemName != null)
		{
			CheckOwnedCosmetic(throwableItemName);
			if (Main.CosmeticsOwned.Contains(throwableItemName))
			{
				EquipCosmetic(throwableItemName);
			}
		}
	}

	public static void FireSoundSpam()
	{
		if (Main.rightTrigger > 0.5f)
		{
			EquipCosmetic("LBALH.");
		}
	}

	public static void BubblerGun(int index, Quaternion handRotation, float handOffset = 0.5f)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun(LayerMask.op_Implicit(GTPlayer.Instance.locomotionEnabledLayers)).NewPointer;
			if (Main.GetGunInput(isShooting: true))
			{
				DisableThrowableCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(DisableThrowable(index));
				TransferrableObject val = VRRig.LocalRig.myBodyDockPositions.allObjects[index];
				string throwableItemName = GetThrowableItemName(val);
				if (throwableItemName == null || !Main.CosmeticsOwned.Contains(throwableItemName))
				{
					((Behaviour)VRRig.LocalRig).enabled = false;
					((Component)VRRig.LocalRig).transform.position = Main.TryOnRoom.transform.position;
				}
				if (!((Component)val).gameObject.activeSelf)
				{
					VRRig.LocalRig.SetActiveTransferrableObjectIndex(1, index);
					((Component)val).gameObject.SetActive(true);
				}
				val.storedZone = (DropPositions)2;
				val.currentState = (PositionState)8;
				((Behaviour)VRRig.LocalRig).enabled = false;
				((Component)VRRig.LocalRig).transform.position = item.transform.position - Vector3.up * 0.5f;
				((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = item.transform.position + Vector3.up * handOffset;
				((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = handRotation;
				((VRMap)VRRig.LocalRig.rightIndex).calcT = 1f;
				((VRMap)VRRig.LocalRig.rightMiddle).calcT = 1f;
				((VRMap)VRRig.LocalRig.rightIndex).LerpFinger(1f, false);
				((VRMap)VRRig.LocalRig.rightMiddle).LerpFinger(1f, false);
			}
		}
		else
		{
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void WhiteColorTarget(VRRig rig)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		int num = 629;
		DisableThrowableCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(DisableThrowable(num));
		TransferrableObject val = VRRig.LocalRig.myBodyDockPositions.allObjects[num];
		string throwableItemName = GetThrowableItemName(val);
		if (throwableItemName == null || !Main.CosmeticsOwned.Contains(throwableItemName))
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = Main.TryOnRoom.transform.position;
		}
		if (!((Component)val).gameObject.activeSelf)
		{
			VRRig.LocalRig.SetActiveTransferrableObjectIndex(1, num);
			((Component)val).gameObject.SetActive(true);
		}
		val.storedZone = (DropPositions)2;
		val.currentState = (PositionState)8;
		((Behaviour)VRRig.LocalRig).enabled = false;
		((Component)VRRig.LocalRig).transform.position = ((Component)rig).transform.position - Vector3.up;
		((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)rig).transform.position;
		((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = RandomUtilities.RandomQuaternion();
		((VRMap)VRRig.LocalRig.rightIndex).calcT = 1f;
		((VRMap)VRRig.LocalRig.rightMiddle).calcT = 1f;
		((VRMap)VRRig.LocalRig.rightIndex).LerpFinger(1f, false);
		((VRMap)VRRig.LocalRig.rightMiddle).LerpFinger(1f, false);
	}

	public static void WhiteColorGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				WhiteColorTarget(Main.lockTarget);
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void BlackColorTarget(VRRig rig)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		SendThrowableProjectile(600, ((Component)rig).transform.position, Vector3.zero, Quaternion.identity);
	}

	public static void BlackColorGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				BlackColorTarget(Main.lockTarget);
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void ChickenTarget(VRRig rig)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		int num = 651;
		DisableThrowableCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(DisableThrowable(num));
		TransferrableObject val = VRRig.LocalRig.myBodyDockPositions.allObjects[num];
		string throwableItemName = GetThrowableItemName(val);
		if (throwableItemName == null || !Main.CosmeticsOwned.Contains(throwableItemName))
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = Main.TryOnRoom.transform.position;
		}
		if (!((Component)val).gameObject.activeSelf)
		{
			EquipCosmetic("LMAQL.");
		}
		val.storedZone = (DropPositions)16;
		val.currentState = (PositionState)8;
		((Behaviour)VRRig.LocalRig).enabled = false;
		((Component)VRRig.LocalRig).transform.position = ((Component)rig).transform.position;
		((Component)VRRig.LocalRig.rightHand.rigTarget).transform.position = ((Component)rig).transform.position + Vector3.up * (Time.time * 5f % 1f - 0.5f);
		((Component)VRRig.LocalRig.rightHand.rigTarget).transform.rotation = Quaternion.identity;
		((VRMap)VRRig.LocalRig.rightIndex).calcT = 1f;
		((VRMap)VRRig.LocalRig.rightMiddle).calcT = 1f;
		((VRMap)VRRig.LocalRig.rightIndex).LerpFinger(1f, false);
		((VRMap)VRRig.LocalRig.rightMiddle).LerpFinger(1f, false);
	}

	public static void ChickenGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				ChickenTarget(Main.lockTarget);
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void SendThrowableProjectile(int index, Vector3 pos, Vector3 vel, Quaternion rot)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		DistancePatch.enabled = true;
		TransferrableObject val = VRRig.LocalRig.myBodyDockPositions.allObjects[index];
		string throwableItemName = GetThrowableItemName(val);
		if (throwableItemName == null || !Main.CosmeticsOwned.Contains(throwableItemName))
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = Main.TryOnRoom.transform.position;
			return;
		}
		if (!((Component)val).gameObject.activeSelf)
		{
			VRRig.LocalRig.SetActiveTransferrableObjectIndex(1, index);
			((Component)val).gameObject.SetActive(true);
		}
		val.storedZone = (DropPositions)2;
		val.currentState = (PositionState)8;
		Projectiles.BetaFireProjectile(throwableItemName, pos, vel, Color.white, null, bypassTeleport: true);
	}

	public static void ThrowableProjectileSpam(int projectileIndex)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab)
		{
			SendThrowableProjectile(projectileIndex, GorillaTagger.Instance.rightHandTransform.position, Vector3.zero, RandomUtilities.RandomQuaternion());
		}
	}

	public static void ThrowableProjectileMinigun(int projectileIndex)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab)
		{
			SendThrowableProjectile(projectileIndex, GorillaTagger.Instance.rightHandTransform.position, Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * Main.ShootStrength, RandomUtilities.RandomQuaternion());
		}
	}

	public static void ThrowableProjectileGun(int projectileIndex)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (Main.GetGunInput(isShooting: true))
			{
				SendThrowableProjectile(projectileIndex, item.transform.position + new Vector3(0f, 0.1f, 0f), new Vector3(0f, 0f, 0f), RandomUtilities.RandomQuaternion());
			}
		}
	}

	public static void EnableAtticAntiReport()
	{
		startTimeBuilding = Time.time + 5f;
	}

	public static void AtticAntiReport()
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > startTimeBuilding)
		{
			Buttons.GetIndex("Attic Anti Report").enabled = false;
		}
		foreach (GorillaPlayerScoreboardLine item in GorillaScoreboardTotalUpdater.allScoreboardLines.Where((GorillaPlayerScoreboardLine line) => line.linePlayer == NetworkSystem.Instance.LocalPlayer))
		{
			RequestCreatePiece(-566818631, ((Component)item.reportButton).transform.position + RandomUtilities.RandomVector3(0.3f), RandomUtilities.RandomQuaternion(), 0, null, overrideFreeze: true);
			Main.RPCProtection();
		}
	}

	public static void AtticDrawGun()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		GameObject item = Main.RenderGun().NewPointer;
		if (Main.GetGunInput(isShooting: true))
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			}
			else
			{
				((MonoBehaviour)CoroutineManager.instance).StartCoroutine(DrawSmallDelay(item.transform.position));
			}
		}
	}

	public static void AtticBuildGun()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		GameObject item = Main.RenderGun().NewPointer;
		if (Main.GetGunInput(isShooting: true))
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
				return;
			}
			RequestCreatePiece(pieceIdSet, item.transform.position, RandomUtilities.RandomQuaternion(), 0, null, overrideFreeze: true);
			Main.RPCProtection();
		}
	}

	public static IEnumerator DrawSmallDelay(Vector3 position)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		GameObject Temporary = GameObject.CreatePrimitive((PrimitiveType)0);
		Temporary.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
		Temporary.transform.position = position;
		Object.Destroy((Object)(object)Temporary.GetComponent<Collider>());
		yield return (object)new WaitForSeconds(0.5f);
		RequestCreatePiece(pieceIdSet, Temporary.transform.position, RandomUtilities.RandomQuaternion(), 0, null, overrideFreeze: true);
		Object.Destroy((Object)(object)Temporary);
		Main.RPCProtection();
	}

	public static void AtticFreezeGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				if (!PhotonNetwork.IsMasterClient)
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
				}
				else
				{
					Player target = RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(Main.lockTarget));
					RequestCreatePiece(-566818631, Main.lockTarget.headMesh.transform.position + RandomUtilities.RandomVector3(0.4f), RandomUtilities.RandomQuaternion(), 0, target, overrideFreeze: true);
					RequestCreatePiece(-566818631, Main.lockTarget.leftHandTransform.position + RandomUtilities.RandomVector3(0.4f), RandomUtilities.RandomQuaternion(), 0, target, overrideFreeze: true);
					RequestCreatePiece(-566818631, Main.lockTarget.rightHandTransform.position + RandomUtilities.RandomVector3(0.4f), RandomUtilities.RandomQuaternion(), 0, target, overrideFreeze: true);
					Main.RPCProtection();
				}
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
		}
	}

	public static void AtticFreezeAll()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightTrigger > 0.5f)
		{
			Player randomPlayer = RigUtilities.GetRandomPlayer(includeSelf: false);
			if (!PhotonNetwork.IsMasterClient)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
				return;
			}
			RigUtilities.GetVRRigFromPlayer(NetPlayer.op_Implicit(randomPlayer));
			RequestCreatePiece(-566818631, Main.lockTarget.headMesh.transform.position + RandomUtilities.RandomVector3(0.4f), RandomUtilities.RandomQuaternion(), 0, randomPlayer, overrideFreeze: true);
			RequestCreatePiece(-566818631, Main.lockTarget.leftHandTransform.position + RandomUtilities.RandomVector3(0.4f), RandomUtilities.RandomQuaternion(), 0, randomPlayer, overrideFreeze: true);
			RequestCreatePiece(-566818631, Main.lockTarget.rightHandTransform.position + RandomUtilities.RandomVector3(0.4f), RandomUtilities.RandomQuaternion(), 0, randomPlayer, overrideFreeze: true);
			Main.RPCProtection();
		}
	}

	public static void AtticFloatGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				if (!PhotonNetwork.IsMasterClient)
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
				}
				else
				{
					floatPower += (0.3f - floatPower) * 0.05f;
					RequestCreatePiece(-566818631, ((Component)Main.lockTarget).transform.position + Vector3.down * floatPower, Quaternion.Euler(0f, Random.Range(0f, 350f), 0f), 0, RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(Main.lockTarget)), overrideFreeze: true);
					Main.RPCProtection();
				}
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else
		{
			floatPower = 0.35f;
			if (Main.gunLocked)
			{
				Main.gunLocked = false;
			}
		}
	}

	public static void AtticFlingGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				if (!PhotonNetwork.IsMasterClient)
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
				}
				else
				{
					RequestCreatePiece(-566818631, ((Component)Main.lockTarget).transform.position + Vector3.down * 0.35f, Quaternion.Euler(0f, Random.Range(0f, 350f), 0f), 0, RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(Main.lockTarget)), overrideFreeze: false, forceGravity: true, Vector3.up * 50f);
					Main.RPCProtection();
				}
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else
		{
			floatPower = 0.35f;
			if (Main.gunLocked)
			{
				Main.gunLocked = false;
			}
		}
	}

	public static void AtticBringGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				if (!PhotonNetwork.IsMasterClient)
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
				}
				else
				{
					Vector3 val2 = ((Component)Main.lockTarget).transform.position;
					Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 350f), 0f);
					Player target = RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(Main.lockTarget));
					Vector3 val3 = ((Component)GorillaTagger.Instance.headCollider).transform.position - ((Component)Main.lockTarget).transform.position;
					RequestCreatePiece(-566818631, val2, rotation, 0, target, overrideFreeze: false, forceGravity: true, ((Vector3)(ref val3)).normalized * 50f);
					Main.RPCProtection();
				}
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else
		{
			floatPower = 0.35f;
			if (Main.gunLocked)
			{
				Main.gunLocked = false;
			}
		}
	}

	public static void AtticPushGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				if (!PhotonNetwork.IsMasterClient)
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
				}
				else
				{
					Vector3 val2 = ((Component)Main.lockTarget).transform.position;
					Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 350f), 0f);
					Player target = RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(Main.lockTarget));
					Vector3 val3 = ((Component)Main.lockTarget).transform.position - ((Component)GorillaTagger.Instance.headCollider).transform.position;
					RequestCreatePiece(-566818631, val2, rotation, 0, target, overrideFreeze: false, forceGravity: true, ((Vector3)(ref val3)).normalized * 50f);
					Main.RPCProtection();
				}
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else
		{
			floatPower = 0.35f;
			if (Main.gunLocked)
			{
				Main.gunLocked = false;
			}
		}
	}

	public static void AtticTowerGun()
	{
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			GameObject item = Main.RenderGun().NewPointer;
			if (position != Vector3.zero)
			{
				RequestCreatePiece(pieceIdSet, position, Quaternion.Euler(0f, (float)(Time.frameCount % 360), 0f), 0, null, overrideFreeze: true);
				Main.RPCProtection();
				position += new Vector3(0f, 0.1f, 0f);
			}
			if (Main.GetGunInput(isShooting: true))
			{
				position = item.transform.position;
			}
		}
		else
		{
			position = Vector3.zero;
		}
	}

	public static IEnumerator FireShotgun()
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			yield break;
		}
		isFiring = true;
		if (!File.Exists("SeralythMenu/shotgun.wav"))
		{
			AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Mods/Fun/shotgun.ogg", "Audio/Mods/Fun/shotgun.ogg");
		}
		Sound.PlayAudio("shotgun.wav");
		BuilderPiece bullet = null;
		yield return CreateGetPiece(1925587737, delegate(BuilderPiece piece)
		{
			bullet = piece;
		});
		while ((Object)(object)bullet == (Object)null)
		{
			yield return null;
		}
		RequestGrabPiece(bullet, isLefHand: true, Vector3.zero, Quaternion.identity);
		yield return null;
		RequestDropPiece(bullet, ControllerUtilities.GetTrueRightHand().position + ControllerUtilities.GetTrueRightHand().forward * 0.65f + ControllerUtilities.GetTrueRightHand().right * 0.03f + ControllerUtilities.GetTrueRightHand().up * 0.05f, ControllerUtilities.GetTrueRightHand().rotation, ControllerUtilities.GetTrueRightHand().forward * 19.9f, Vector3.zero);
		yield return null;
	}

	public static void UnlimitedBuilding()
	{
		((BuilderPieceInteractor)BuilderPieceInteractor.instance).maxHoldablePieceStackCount = int.MaxValue;
		UnlimitPatches.enabled = true;
	}

	public static void DisableUnlimitedBuilding()
	{
		((BuilderPieceInteractor)BuilderPieceInteractor.instance).maxHoldablePieceStackCount = 50;
		UnlimitPatches.enabled = false;
	}

	public static void PlaceBlockGun()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true))
		{
			BuilderPiece componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<BuilderPiece>();
			if (Object.op_Implicit((Object)(object)componentInParent))
			{
				RequestPlacePiece(PlacePatch._piece, componentInParent, PlacePatch._bumpOffsetX, PlacePatch._bumpOffsetZ, PlacePatch._twist, PlacePatch._parentPiece, PlacePatch._attachIndex, PlacePatch._parentAttachIndex);
				Main.RPCProtection();
			}
		}
	}

	public static void DestroyBlockGun()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true))
		{
			BuilderPiece componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<BuilderPiece>();
			if (Object.op_Implicit((Object)(object)componentInParent))
			{
				RequestRecyclePiece(componentInParent, playFX: true, 2);
				Main.RPCProtection();
			}
		}
	}

	public static void ChangeBlockDelay(bool positive = true)
	{
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				blockDebounceIndex++;
			}
			else
			{
				blockDebounceIndex--;
			}
		}
		if (blockDebounceIndex > 20)
		{
			blockDebounceIndex = 1;
		}
		if (blockDebounceIndex < 1)
		{
			blockDebounceIndex = 20;
		}
		blockDebounce = (float)blockDebounceIndex / 20f;
		Buttons.GetIndex("Change Block Delay").overlapText = "Change Block Delay <color=grey>[</color><color=green>" + blockDebounce + "</color><color=grey>]</color>";
	}

	public static void RequestCreatePiece(int pieceType, Vector3 position, Quaternion rotation, int materialType, object target = null, bool overrideFreeze = false, bool forceGravity = false, Vector3? velocity = null, Vector3? angVelocity = null)
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_058a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		if (Buttons.GetIndex("Random Block Type").enabled)
		{
			pieceType = GetRandomBlockType();
		}
		BuilderTable builderTable = ManagerRegistry.BuilderTable;
		BuilderTableNetworking builderNetworking = builderTable.builderNetworking;
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (!(Time.time > blockDelay))
			{
				return;
			}
			int num = builderTable.CreatePieceId();
			object[] array = new object[8]
			{
				pieceType,
				num,
				BitPackUtils.PackWorldPosForNetwork(position),
				BitPackUtils.PackQuaternionForNetwork(rotation),
				materialType,
				(byte)4,
				1,
				PhotonNetwork.LocalPlayer
			};
			if (!(target is RpcTarget val))
			{
				Player val2 = (Player)((target is Player) ? target : null);
				if (val2 != null)
				{
					((MonoBehaviourPun)builderNetworking).photonView.RPC("PieceCreatedByShelfRPC", val2, array);
				}
				else
				{
					((MonoBehaviourPun)builderNetworking).photonView.RPC("PieceCreatedByShelfRPC", (RpcTarget)0, array);
				}
			}
			else
			{
				((MonoBehaviourPun)builderNetworking).photonView.RPC("PieceCreatedByShelfRPC", val, array);
			}
			if (!((!overrideFreeze && !Buttons.GetIndex("Zero Gravity Blocks").enabled) || forceGravity))
			{
				return;
			}
			blockDelay = Time.time + 0.02f;
			array = new object[5]
			{
				builderNetworking.CreateLocalCommandId(),
				num,
				true,
				BitPackUtils.PackHandPosRotForNetwork(Vector3.zero, Quaternion.identity),
				PhotonNetwork.LocalPlayer
			};
			if (!(target is RpcTarget val3))
			{
				Player val4 = (Player)((target is Player) ? target : null);
				if (val4 != null)
				{
					((MonoBehaviourPun)builderNetworking).photonView.RPC("PieceGrabbedRPC", val4, array);
				}
				else
				{
					((MonoBehaviourPun)builderNetworking).photonView.RPC("PieceGrabbedRPC", (RpcTarget)0, array);
				}
			}
			else
			{
				((MonoBehaviourPun)builderNetworking).photonView.RPC("PieceGrabbedRPC", val3, array);
			}
			array = new object[7]
			{
				builderNetworking.CreateLocalCommandId(),
				num,
				position,
				rotation,
				(object)(Vector3)(((_003F?)velocity) ?? Vector3.zero),
				(object)(Vector3)(((_003F?)angVelocity) ?? Vector3.zero),
				PhotonNetwork.LocalPlayer
			};
			if (!(target is RpcTarget val5))
			{
				Player val6 = (Player)((target is Player) ? target : null);
				if (val6 != null)
				{
					((MonoBehaviourPun)builderNetworking).photonView.RPC("PieceDroppedRPC", val6, array);
				}
				else
				{
					((MonoBehaviourPun)builderNetworking).photonView.RPC("PieceDroppedRPC", (RpcTarget)0, array);
				}
			}
			else
			{
				((MonoBehaviourPun)builderNetworking).photonView.RPC("PieceDroppedRPC", val5, array);
			}
		}
		else
		{
			if (!(Time.time > blockDelay))
			{
				return;
			}
			blockDelay = Time.time + blockDebounce;
			BuilderPiece val7 = ((from piece in Main.GetAllType<BuilderPiece>(5f)
				where ((Component)piece).gameObject.activeInHierarchy
				where piece.pieceType == pieceType
				where !piece.isBuiltIntoTable
				where piece.CanPlayerGrabPiece(PhotonNetwork.LocalPlayer.ActorNumber, ((Component)piece).transform.position)
				where Vector3.Distance(((Component)piece).transform.position, Main.ServerLeftHandPos) < 2.5f
				orderby Vector3.Distance(((Component)piece).transform.position, Main.ServerLeftHandPos)
				select piece).FirstOrDefault() ?? null) ?? (from piece in Main.GetAllType<BuilderPiece>(5f)
				where ((Component)piece).gameObject.activeInHierarchy
				where !piece.isBuiltIntoTable
				where piece.CanPlayerGrabPiece(PhotonNetwork.LocalPlayer.ActorNumber, ((Component)piece).transform.position)
				where Vector3.Distance(((Component)piece).transform.position, Main.ServerLeftHandPos) < 2.5f
				orderby Vector3.Distance(((Component)piece).transform.position, Main.ServerLeftHandPos)
				select piece).FirstOrDefault() ?? null;
			if (!((Object)(object)val7 == (Object)null))
			{
				if (Vector3.Distance(Main.ServerLeftHandPos, position) > 2.5f)
				{
					Vector3 serverLeftHandPos = Main.ServerLeftHandPos;
					Vector3 val8 = position - Main.ServerLeftHandPos;
					position = serverLeftHandPos + ((Vector3)(ref val8)).normalized * 2.5f;
				}
				pieceId = val7.pieceId;
				builderNetworking.RequestGrabPiece(val7, true, Vector3.zero, Quaternion.identity);
				builderNetworking.RequestDropPiece(val7, position, rotation, (Vector3)(((_003F?)velocity) ?? Vector3.zero), (Vector3)(((_003F?)angVelocity) ?? Vector3.zero));
			}
		}
	}

	public static void RequestGrabPiece(BuilderPiece piece, bool isLefHand, Vector3 localPosition, Quaternion localRotation)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		BuilderTableNetworking builderNetworking = ManagerRegistry.BuilderTable.builderNetworking;
		if (NetworkSystem.Instance.IsMasterClient)
		{
			((MonoBehaviourPun)builderNetworking).photonView.RPC("PieceGrabbedRPC", (RpcTarget)0, new object[5]
			{
				builderNetworking.CreateLocalCommandId(),
				piece.pieceId,
				isLefHand,
				BitPackUtils.PackHandPosRotForNetwork(localPosition, localRotation),
				PhotonNetwork.LocalPlayer
			});
		}
		else
		{
			builderNetworking.RequestGrabPiece(piece, isLefHand, localPosition, localRotation);
		}
	}

	public static void RequestPlacePiece(BuilderPiece piece, BuilderPiece attachPiece, sbyte bumpOffsetX, sbyte bumpOffsetZ, byte twist, BuilderPiece parentPiece, int attachIndex, int parentAttachIndex)
	{
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		if (!attachPiece.isBuiltIntoTable)
		{
			BuilderTableNetworking builderNetworking = ManagerRegistry.BuilderTable.builderNetworking;
			if (NetworkSystem.Instance.IsMasterClient)
			{
				((MonoBehaviourPun)builderNetworking).photonView.RPC("PiecePlacedRPC", (RpcTarget)0, new object[9]
				{
					builderNetworking.CreateLocalCommandId(),
					piece.pieceId,
					((Object)(object)attachPiece != (Object)null) ? attachPiece.pieceId : (-1),
					BuilderTable.PackPiecePlacement(twist, bumpOffsetX, bumpOffsetZ),
					((Object)(object)parentPiece != (Object)null) ? parentPiece.pieceId : (-1),
					attachIndex,
					parentAttachIndex,
					PhotonNetwork.LocalPlayer,
					PhotonNetwork.ServerTimestamp
				});
			}
			else
			{
				builderNetworking.RequestGrabPiece(piece, true, Vector3.zero, Quaternion.identity);
				builderNetworking.RequestPlacePiece(piece, attachPiece, bumpOffsetX, bumpOffsetZ, twist, parentPiece, attachIndex, parentAttachIndex);
			}
		}
	}

	public static void RequestDropPiece(BuilderPiece piece, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		BuilderTableNetworking builderNetworking = ManagerRegistry.BuilderTable.builderNetworking;
		if (NetworkSystem.Instance.IsMasterClient)
		{
			((MonoBehaviourPun)builderNetworking).photonView.RPC("PieceDroppedRPC", (RpcTarget)0, new object[7]
			{
				builderNetworking.CreateLocalCommandId(),
				piece.pieceId,
				position,
				rotation,
				velocity,
				angVelocity,
				PhotonNetwork.LocalPlayer
			});
		}
		else
		{
			builderNetworking.RequestDropPiece(piece, position, rotation, velocity, angVelocity);
		}
	}

	public static void RequestRecyclePiece(BuilderPiece piece, bool playFX, int recyclerID)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		if (piece.isBuiltIntoTable)
		{
			return;
		}
		BuilderTable builderTable = ManagerRegistry.BuilderTable;
		BuilderTableNetworking builderNetworking = builderTable.builderNetworking;
		if (NetworkSystem.Instance.IsMasterClient)
		{
			((MonoBehaviourPun)builderNetworking).photonView.RPC("PieceDestroyedRPC", (RpcTarget)0, new object[5]
			{
				piece.pieceId,
				BitPackUtils.PackWorldPosForNetwork(((Component)piece).transform.position),
				BitPackUtils.PackQuaternionForNetwork(((Component)piece).transform.rotation),
				playFX,
				(short)recyclerID
			});
		}
		else
		{
			if (!(Time.time > blockDelay))
			{
				return;
			}
			blockDelay = Time.time + blockDebounce;
			if (piece.CanPlayerGrabPiece(PhotonNetwork.LocalPlayer.ActorNumber, ((Component)piece).transform.position) && Vector3.Distance(((Component)piece).transform.position, Main.ServerLeftHandPos) < 2.5f)
			{
				BuilderDropZone val = (from zone in builderTable.dropZones
					where (int)zone.dropType >= 1
					where Vector3.Distance(((Component)zone).transform.position, Main.ServerLeftHandPos) < 2.5f
					orderby Vector3.Distance(((Component)zone).transform.position, Main.ServerLeftHandPos)
					select zone).FirstOrDefault() ?? null;
				Vector3 val2 = (((Object)(object)val != (Object)null) ? ((Component)val).transform.position : (Main.ServerLeftHandPos + Vector3.down * 2f));
				RequestGrabPiece(piece, isLefHand: true, Vector3.zero, Quaternion.identity);
				RequestDropPiece(piece, val2, RandomUtilities.RandomQuaternion(), Vector3.down * 20f, Vector3.zero);
			}
		}
	}

	public static void BuildingBlockAura()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		int pieceType = pieceIdSet;
		Vector3 val = ((Component)VRRig.LocalRig).transform.position;
		Vector3 val2 = RandomUtilities.RandomVector3();
		RequestCreatePiece(pieceType, val + ((Vector3)(ref val2)).normalized * 2f, Quaternion.identity, 0);
		Main.RPCProtection();
	}

	public static void BuildingBlockTextGun()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		GameObject item = Main.RenderGun().NewPointer;
		if (Main.GetGunInput(isShooting: true))
		{
			if (!Overpowered.basePosition.HasValue)
			{
				Overpowered.basePosition = item.transform.position + Vector3.up;
			}
			if (!(Time.time > Overpowered.textDelay))
			{
				return;
			}
			Overpowered.textDelay = Time.time + 0.1f;
			bool[][] array = Overpowered.Letters[Overpowered.textToRender[Overpowered.characterIndex].ToString()];
			List<Vector3> list = new List<Vector3>();
			Vector3 val = default(Vector3);
			for (int i = 0; i < array.Length; i++)
			{
				bool[] array2 = array[i];
				for (int j = 0; j < array2.Length; j++)
				{
					bool flag = array2[j];
					((Vector3)(ref val))._002Ector((float)j * 0.2f + (float)Overpowered.characterIndex * 1.2f, (float)i * -0.2f, 0f);
					if (flag)
					{
						list.Add(Overpowered.basePosition.Value + val);
					}
				}
			}
			Overpowered.characterIndex++;
			foreach (Vector3 item2 in list)
			{
				RequestCreatePiece(pieceIdSet, item2, Quaternion.identity, 0);
			}
			Main.RPCProtection();
		}
		else
		{
			Overpowered.basePosition = null;
			Overpowered.characterIndex = 0;
		}
	}

	public static void RainBuildingBlocks()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		RequestCreatePiece(pieceIdSet, ((Component)VRRig.LocalRig).transform.position + new Vector3(Random.Range(-3f, 3f), 4f, Random.Range(-3f, 3f)), Quaternion.identity, 0);
		Main.RPCProtection();
	}

	public static void BuildingBlockFountain()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		RequestCreatePiece(pieceIdSet, ((Component)VRRig.LocalRig).transform.position + Vector3.up * 3f, Quaternion.identity, 0, null, overrideFreeze: false, forceGravity: false, RandomUtilities.RandomVector3(15f));
		Main.RPCProtection();
	}

	public static void SpazObject(string objectName)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		ThrowableBug bug = GetBug(objectName);
		if ((Object)(object)bug != (Object)null)
		{
			((Component)bug).transform.rotation = RandomUtilities.RandomQuaternion();
		}
	}

	public static void SpazCamera()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		LckSocialCamera networkedCococam = LckSocialCameraManager.Instance._networkedCococam;
		networkedCococam.visible = true;
		networkedCococam.recording = true;
		networkedCococam.m_CameraVisuals.SetNetworkedVisualsActive(true);
		networkedCococam.m_CameraVisuals.SetRecordingState(true);
		((Component)networkedCococam).transform.rotation = RandomUtilities.RandomQuaternion();
	}

	public static void SpazTablet()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		LckSocialCamera networkedTablet = LckSocialCameraManager.Instance._networkedTablet;
		networkedTablet.visible = true;
		networkedTablet.recording = true;
		networkedTablet.m_CameraVisuals.SetNetworkedVisualsActive(true);
		networkedTablet.m_CameraVisuals.SetRecordingState(true);
		((Component)networkedTablet).transform.rotation = RandomUtilities.RandomQuaternion();
	}

	public static void SpazGliders()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		GliderHoldable[] allType = Main.GetAllType<GliderHoldable>(5f);
		foreach (GliderHoldable val in allType)
		{
			if (((NetworkView)val).GetView.Owner == PhotonNetwork.LocalPlayer)
			{
				((Component)val).gameObject.transform.rotation = RandomUtilities.RandomQuaternion();
			}
			else
			{
				((NetworkHoldableObject)val).OnHover((InteractionPoint)null, (GameObject)null);
			}
		}
	}

	public static void SpazHoverboard()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)VRRig.LocalRig.hoverboardVisual != (Object)null && VRRig.LocalRig.hoverboardVisual.IsHeld)
		{
			VRRig.LocalRig.hoverboardVisual.SetIsHeld(VRRig.LocalRig.hoverboardVisual.IsLeftHanded, VRRig.LocalRig.hoverboardVisual.NominalLocalPosition, RandomUtilities.RandomQuaternion(), RandomUtilities.RandomColor());
		}
	}

	public static void OrbitObject(string objectName, float offset = 0f)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		ThrowableBug bug = GetBug(objectName);
		if ((Object)(object)bug != (Object)null)
		{
			((Component)bug).transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(MathF.Cos(offset + (float)Time.frameCount / 30f), 1f, MathF.Sin(offset + (float)Time.frameCount / 30f));
		}
	}

	public static void OrbitCamera()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		LckSocialCamera networkedCococam = LckSocialCameraManager.Instance._networkedCococam;
		networkedCococam.visible = true;
		networkedCococam.recording = true;
		networkedCococam.m_CameraVisuals.SetNetworkedVisualsActive(true);
		networkedCococam.m_CameraVisuals.SetRecordingState(true);
		((Component)networkedCococam).transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(MathF.Cos(240f + (float)Time.frameCount / 30f), 1f, MathF.Sin(240f + (float)Time.frameCount / 30f));
	}

	public static void OrbitTablet()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		LckSocialCamera networkedCococam = LckSocialCameraManager.Instance._networkedCococam;
		networkedCococam.visible = true;
		networkedCococam.recording = true;
		networkedCococam.m_CameraVisuals.SetNetworkedVisualsActive(true);
		networkedCococam.m_CameraVisuals.SetRecordingState(true);
		((Component)networkedCococam).transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(MathF.Cos(60f + (float)Time.frameCount / 30f), 1f, MathF.Sin(240f + (float)Time.frameCount / 30f));
	}

	public static void ObjectAura(string objectName)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		ThrowableBug bug = GetBug(objectName);
		if ((Object)(object)bug != (Object)null)
		{
			((Component)bug).transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position + RandomUtilities.RandomVector3();
			((Component)bug).transform.rotation = RandomUtilities.RandomQuaternion();
		}
	}

	public static void CameraAura()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		LckSocialCamera networkedCococam = LckSocialCameraManager.Instance._networkedCococam;
		networkedCococam.visible = true;
		networkedCococam.recording = true;
		networkedCococam.m_CameraVisuals.SetNetworkedVisualsActive(true);
		networkedCococam.m_CameraVisuals.SetRecordingState(true);
		((Component)networkedCococam).transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position + RandomUtilities.RandomVector3();
		((Component)networkedCococam).transform.rotation = RandomUtilities.RandomQuaternion();
	}

	public static void TabletAura()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		LckSocialCamera networkedTablet = LckSocialCameraManager.Instance._networkedTablet;
		networkedTablet.visible = true;
		networkedTablet.recording = true;
		networkedTablet.m_CameraVisuals.SetNetworkedVisualsActive(true);
		networkedTablet.m_CameraVisuals.SetRecordingState(true);
		((Component)networkedTablet).transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position + RandomUtilities.RandomVector3();
		((Component)networkedTablet).transform.rotation = RandomUtilities.RandomQuaternion();
	}

	public static void BalloonAura()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		BalloonHoldable[] allType = Main.GetAllType<BalloonHoldable>(5f);
		foreach (BalloonHoldable val in allType)
		{
			if (((TransferrableObject)val).ownerRig.isLocal)
			{
				((Component)val).gameObject.transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position + RandomUtilities.RandomVector3();
				((Component)val).gameObject.transform.rotation = RandomUtilities.RandomQuaternion();
			}
			else
			{
				((TransferrableObject)val).WorldShareableRequestOwnership();
			}
		}
	}

	public static void GliderAura()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		GliderHoldable[] allType = Main.GetAllType<GliderHoldable>(5f);
		foreach (GliderHoldable val in allType)
		{
			if (((NetworkView)val).GetView.Owner == PhotonNetwork.LocalPlayer)
			{
				((Component)val).gameObject.transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position + RandomUtilities.RandomVector3();
				((Component)val).gameObject.transform.rotation = RandomUtilities.RandomQuaternion();
			}
			else
			{
				((NetworkHoldableObject)val).OnHover((InteractionPoint)null, (GameObject)null);
			}
		}
	}

	public static void HoverboardAura()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > hoverboardAuraDelay)
		{
			hoverboardAuraDelay = Time.time + 0.25f;
			for (int i = 0; i < 2; i++)
			{
				BetaDropBoard(((Component)GorillaTagger.Instance.headCollider).transform.position + RandomUtilities.RandomVector3(), RandomUtilities.RandomQuaternion(), RandomUtilities.RandomVector3() * 20f, RandomUtilities.RandomVector3() * 20f, RandomUtilities.RandomColor());
			}
		}
	}

	public static void OrbitGliders()
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		GliderHoldable[] allType = Main.GetAllType<GliderHoldable>(5f);
		int num = 0;
		GliderHoldable[] array = allType;
		foreach (GliderHoldable val in array)
		{
			if (((NetworkView)val).GetView.Owner == PhotonNetwork.LocalPlayer)
			{
				float num2 = 360f / (float)allType.Length * (float)num;
				((Component)val).gameObject.transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(MathF.Cos(num2 + (float)Time.frameCount / 30f) * 5f, 2f, MathF.Sin(num2 + (float)Time.frameCount / 30f) * 5f);
			}
			else
			{
				((NetworkHoldableObject)val).OnHover((InteractionPoint)null, (GameObject)null);
			}
			num++;
		}
	}

	public static void OrbitBlocks()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		RequestCreatePiece(pieceIdSet, ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(MathF.Cos((float)Time.frameCount / 30f), 0f, MathF.Sin((float)Time.frameCount / 30f)), Quaternion.identity, 0);
		Main.RPCProtection();
	}

	public static void RideObject(string objectName)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		GameObject gameObject = ((Component)GetBugObject(objectName)).gameObject;
		Main.TeleportPlayer(gameObject.transform.position);
		GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
	}

	public static void BecomeObject(string objectName)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		ThrowableBug bug = GetBug(objectName);
		if ((Object)(object)bug != (Object)null)
		{
			((Behaviour)VRRig.LocalRig).enabled = false;
			((Component)VRRig.LocalRig).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position - Vector3.up * 99999f;
			((Component)bug).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
			((Component)bug).transform.rotation = ((Component)GorillaTagger.Instance.headCollider).transform.rotation;
		}
		else if (lastWasNull)
		{
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
		lastWasNull = (Object)(object)bug != (Object)null;
	}

	public static void BecomeCamera()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		((Behaviour)VRRig.LocalRig).enabled = false;
		((Component)VRRig.LocalRig).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position - Vector3.up * 99999f;
		LckSocialCamera networkedCococam = LckSocialCameraManager.Instance._networkedCococam;
		networkedCococam.visible = true;
		networkedCococam.recording = true;
		networkedCococam.m_CameraVisuals.SetNetworkedVisualsActive(true);
		networkedCococam.m_CameraVisuals.SetRecordingState(true);
		((Component)networkedCococam).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
		((Component)networkedCococam).transform.rotation = ((Component)GorillaTagger.Instance.headCollider).transform.rotation;
	}

	public static void BecomeTablet()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		((Behaviour)VRRig.LocalRig).enabled = false;
		((Component)VRRig.LocalRig).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position - Vector3.up * 99999f;
		LckSocialCamera networkedTablet = LckSocialCameraManager.Instance._networkedTablet;
		networkedTablet.visible = true;
		networkedTablet.recording = true;
		networkedTablet.m_CameraVisuals.SetNetworkedVisualsActive(true);
		networkedTablet.m_CameraVisuals.SetRecordingState(true);
		((Component)networkedTablet).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
		((Component)networkedTablet).transform.rotation = ((Component)GorillaTagger.Instance.headCollider).transform.rotation;
	}

	public static void NoclipBuilding()
	{
		if (!(Time.time > noclipBuildingDelay))
		{
			return;
		}
		noclipBuildingDelay = Time.time + 5f;
		foreach (BuilderPiece item in from block in Main.GetAllType<BuilderPiece>(5f)
			where ((Component)block).gameObject.activeInHierarchy && !block.isBuiltIntoTable
			select block)
		{
			item.SetColliderLayers<Collider>(item.colliders, BuilderTable.heldLayerLocal);
		}
	}

	public static void DisableNoclipBuilding()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		foreach (BuilderPiece item in from block in Main.GetAllType<BuilderPiece>(5f)
			where ((Component)block).gameObject.activeInHierarchy && !block.isBuiltIntoTable
			select block)
		{
			item.SetColliderLayers<Collider>(item.colliders, ((int)item.state == 0) ? BuilderTable.placedLayer : BuilderTable.droppedLayer);
		}
	}

	public static void MultiGrab()
	{
		((BuilderPieceInteractor)BuilderPieceInteractor.instance).handState[1] = (HandState)0;
		((BuilderPieceInteractor)BuilderPieceInteractor.instance).heldPiece[1] = null;
	}

	public static IEnumerator CreateGetPiece(int pieceType, Action<BuilderPiece> onComplete)
	{
		CreatePatch.enabled = true;
		CreatePatch.pieceTypeSearch = pieceType;
		yield return null;
		RequestCreatePiece(pieceType, ((Component)VRRig.LocalRig).transform.position + Vector3.up, Quaternion.identity, 0, null, overrideFreeze: true);
		Main.RPCProtection();
		while (pieceId < 0)
		{
			yield return null;
		}
		yield return null;
		pieceId = -1;
		CreatePatch.enabled = false;
		CreatePatch.pieceTypeSearch = 0;
		onComplete?.Invoke(ManagerRegistry.BuilderTable.GetPiece(pieceId));
	}

	public static IEnumerator CreateShotgun()
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
			yield break;
		}
		BuilderPiece basea = null;
		yield return CreateGetPiece(-1927069002, delegate(BuilderPiece piece)
		{
			basea = piece;
		});
		while ((Object)(object)basea == (Object)null)
		{
			yield return null;
		}
		RequestGrabPiece(basea, isLefHand: false, Vector3.zero, Quaternion.identity);
		yield return null;
		((BuilderPieceInteractor)BuilderPieceInteractor.instance).handState[1] = (HandState)0;
		((BuilderPieceInteractor)BuilderPieceInteractor.instance).heldPiece[1] = null;
		yield return null;
		BuilderPiece base2a = null;
		yield return CreateGetPiece(-1621444201, delegate(BuilderPiece piece)
		{
			base2a = piece;
		});
		while ((Object)(object)base2a == (Object)null)
		{
			yield return null;
		}
		RequestGrabPiece(base2a, isLefHand: false, Vector3.zero, Quaternion.identity);
		yield return null;
		RequestPlacePiece(base2a, base2a, 0, 0, 0, basea, 1, 0);
		yield return null;
		BuilderPiece slopea = null;
		yield return CreateGetPiece(-993249117, delegate(BuilderPiece piece)
		{
			slopea = piece;
		});
		while ((Object)(object)slopea == (Object)null)
		{
			yield return null;
		}
		RequestGrabPiece(slopea, isLefHand: false, Vector3.zero, Quaternion.identity);
		yield return null;
		RequestPlacePiece(slopea, slopea, 0, 0, 2, base2a, 1, 0);
		yield return null;
		BuilderPiece trigger = null;
		yield return CreateGetPiece(251444537, delegate(BuilderPiece piece)
		{
			trigger = piece;
		});
		while ((Object)(object)trigger == (Object)null)
		{
			yield return null;
		}
		RequestGrabPiece(trigger, isLefHand: false, Vector3.zero, Quaternion.identity);
		yield return null;
		RequestPlacePiece(trigger, trigger, -1, -2, 3, slopea, 1, 0);
		yield return null;
		BuilderPiece slopeb = null;
		yield return CreateGetPiece(-993249117, delegate(BuilderPiece piece)
		{
			slopeb = piece;
		});
		while ((Object)(object)slopeb == (Object)null)
		{
			yield return null;
		}
		RequestGrabPiece(slopeb, isLefHand: false, Vector3.zero, Quaternion.identity);
		yield return null;
		RequestPlacePiece(basea, trigger, 0, -2, 3, slopeb, 1, 0);
		yield return null;
		BuilderPiece base2b = null;
		yield return CreateGetPiece(-1621444201, delegate(BuilderPiece piece)
		{
			base2b = piece;
		});
		while ((Object)(object)base2b == (Object)null)
		{
			yield return null;
		}
		RequestGrabPiece(base2b, isLefHand: false, Vector3.zero, Quaternion.identity);
		yield return null;
		RequestPlacePiece(slopeb, slopeb, 0, 0, 2, base2b, 1, 0);
		yield return null;
		BuilderPiece baseb = null;
		yield return CreateGetPiece(-1927069002, delegate(BuilderPiece piece)
		{
			baseb = piece;
		});
		while ((Object)(object)baseb == (Object)null)
		{
			yield return null;
		}
		RequestGrabPiece(baseb, isLefHand: false, Vector3.zero, Quaternion.identity);
		yield return null;
		RequestPlacePiece(base2b, base2b, 0, 0, 0, baseb, 1, 0);
		yield return null;
		BuilderPiece minislopeb = null;
		yield return CreateGetPiece(1700655257, delegate(BuilderPiece piece)
		{
			minislopeb = piece;
		});
		while ((Object)(object)minislopeb == (Object)null)
		{
			yield return null;
		}
		RequestGrabPiece(minislopeb, isLefHand: false, Vector3.zero, Quaternion.identity);
		yield return null;
		RequestPlacePiece(baseb, slopea, 0, -3, 2, minislopeb, 2, 0);
		yield return null;
		BuilderPiece minislopea = null;
		yield return CreateGetPiece(1700655257, delegate(BuilderPiece piece)
		{
			minislopea = piece;
		});
		while ((Object)(object)minislopea == (Object)null)
		{
			yield return null;
		}
		RequestGrabPiece(minislopea, isLefHand: false, Vector3.zero, Quaternion.identity);
		yield return null;
		RequestPlacePiece(minislopeb, slopeb, 0, -3, 2, minislopea, 2, 0);
		yield return null;
		BuilderPiece minislope2a = null;
		yield return CreateGetPiece(1700655257, delegate(BuilderPiece piece)
		{
			minislope2a = piece;
		});
		while ((Object)(object)minislope2a == (Object)null)
		{
			yield return null;
		}
		RequestGrabPiece(minislope2a, isLefHand: false, Vector3.zero, Quaternion.identity);
		yield return null;
		RequestPlacePiece(minislopea, minislopeb, 0, 0, 2, minislope2a, 1, 0);
		yield return null;
		BuilderPiece minislope2b = null;
		yield return CreateGetPiece(1700655257, delegate(BuilderPiece piece)
		{
			minislope2b = piece;
		});
		while ((Object)(object)minislope2b == (Object)null)
		{
			yield return null;
		}
		RequestGrabPiece(minislope2b, isLefHand: false, Vector3.zero, Quaternion.identity);
		yield return null;
		RequestPlacePiece(minislope2a, minislopea, 0, 0, 2, minislope2b, 1, 0);
		yield return null;
		BuilderPiece flatthinga = null;
		yield return CreateGetPiece(477262573, delegate(BuilderPiece piece)
		{
			flatthinga = piece;
		});
		while ((Object)(object)flatthinga == (Object)null)
		{
			yield return null;
		}
		RequestGrabPiece(flatthinga, isLefHand: false, Vector3.zero, Quaternion.identity);
		yield return null;
		RequestPlacePiece(minislope2b, minislope2b, 0, -1, 2, flatthinga, 2, 0);
		yield return null;
		BuilderPiece flatthingb = null;
		yield return CreateGetPiece(477262573, delegate(BuilderPiece piece)
		{
			flatthingb = piece;
		});
		while ((Object)(object)flatthingb == (Object)null)
		{
			yield return null;
		}
		RequestGrabPiece(flatthingb, isLefHand: false, Vector3.zero, Quaternion.identity);
		yield return null;
		RequestPlacePiece(flatthinga, minislope2a, 0, -1, 2, flatthingb, 2, 0);
		yield return null;
		BuilderPiece connectorthinga = null;
		yield return CreateGetPiece(251444537, delegate(BuilderPiece piece)
		{
			connectorthinga = piece;
		});
		while ((Object)(object)connectorthinga == (Object)null)
		{
			yield return null;
		}
		RequestGrabPiece(connectorthinga, isLefHand: false, Vector3.zero, Quaternion.identity);
		yield return null;
		RequestPlacePiece(flatthingb, flatthinga, -1, 1, 3, connectorthinga, 1, 0);
		yield return null;
		BuilderPiece connectorthingb = null;
		yield return CreateGetPiece(661312857, delegate(BuilderPiece piece)
		{
			connectorthingb = piece;
		});
		while ((Object)(object)connectorthingb == (Object)null)
		{
			yield return null;
		}
		RequestGrabPiece(connectorthingb, isLefHand: false, Vector3.zero, Quaternion.identity);
		yield return null;
		RequestPlacePiece(connectorthinga, connectorthinga, -1, 0, 1, connectorthingb, 1, 0);
		yield return null;
		BuilderPiece connectorthingc = null;
		yield return CreateGetPiece(661312857, delegate(BuilderPiece piece)
		{
			connectorthingc = piece;
		});
		while ((Object)(object)connectorthingc == (Object)null)
		{
			yield return null;
		}
		RequestGrabPiece(connectorthingc, isLefHand: false, Vector3.zero, Quaternion.identity);
		yield return null;
		RequestPlacePiece(connectorthingb, connectorthinga, 0, 0, 1, connectorthingc, 1, 0);
		yield return null;
		BuilderPiece barrela = null;
		yield return CreateGetPiece(661312857, delegate(BuilderPiece piece)
		{
			barrela = piece;
		});
		while ((Object)(object)barrela == (Object)null)
		{
			yield return null;
		}
		RequestGrabPiece(barrela, isLefHand: false, Vector3.zero, Quaternion.identity);
		yield return null;
		RequestPlacePiece(connectorthingc, connectorthingb, 0, 0, 1, barrela, 1, 0);
		yield return null;
		BuilderPiece barrelb = null;
		yield return CreateGetPiece(661312857, delegate(BuilderPiece piece)
		{
			barrelb = piece;
		});
		while ((Object)(object)barrelb == (Object)null)
		{
			yield return null;
		}
		RequestGrabPiece(barrelb, isLefHand: false, Vector3.zero, Quaternion.identity);
		yield return null;
		RequestPlacePiece(barrela, barrela, 0, 0, 2, barrelb, 1, 0);
		yield return null;
		BuilderPiece scope = null;
		yield return CreateGetPiece(-648273975, delegate(BuilderPiece piece)
		{
			scope = piece;
		});
		while ((Object)(object)scope == (Object)null)
		{
			yield return null;
		}
		RequestGrabPiece(scope, isLefHand: false, Vector3.zero, Quaternion.identity);
		yield return null;
		RequestPlacePiece(barrelb, minislope2a, -2, 1, 3, scope, 1, 0);
		yield return null;
		RequestDropPiece(scope, GorillaTagger.Instance.rightHandTransform.position, Quaternion.identity, Vector3.zero, Vector3.zero);
		yield return null;
		RequestGrabPiece(basea, isLefHand: false, new Vector3(-0.2f, 0.01f, -0.3f), new Quaternion(0f, 0.1f, 0.75f, -0.6f));
		yield return null;
	}

	public static void Shotgun()
	{
		if (isFiring)
		{
			((ControllerInputPoller)ControllerInputPoller.instance).leftControllerGripFloat = 1f;
		}
		if (Main.rightGrab && !previousGripDown)
		{
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(CreateShotgun());
		}
		if (Main.rightGrab && Main.rightTrigger > 0.5f && !previousTriggerDown)
		{
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(FireShotgun());
		}
		previousGripDown = Main.rightGrab;
		previousTriggerDown = Main.rightTrigger > 0.5f;
	}

	public static IEnumerator CreateMassiveBlock()
	{
		VRRig.LocalRig.sizeManager.currentSizeLayerMaskValue = 2;
		yield return (object)new WaitForSeconds(0.6f);
		BuilderPiece stupid = null;
		yield return CreateGetPiece(pieceIdSet, delegate(BuilderPiece piece)
		{
			stupid = piece;
		});
		while ((Object)(object)stupid == (Object)null)
		{
			yield return null;
		}
		RequestGrabPiece(stupid, isLefHand: false, Vector3.zero, Quaternion.identity);
		yield return (object)new WaitForSeconds(0.2f);
		VRRig.LocalRig.sizeManager.currentSizeLayerMaskValue = 13;
		yield return null;
	}

	public static void MassiveBlock()
	{
		if (Main.rightGrab && !previousGripDown)
		{
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(CreateMassiveBlock());
		}
		previousGripDown = Main.rightGrab;
	}

	public static void AtticSizeToggle()
	{
		if (Main.rightTrigger > 0.5f)
		{
			VRRig.LocalRig.sizeManager.currentSizeLayerMaskValue = 13;
		}
		if (Main.rightGrab)
		{
			VRRig.LocalRig.sizeManager.currentSizeLayerMaskValue = 2;
		}
	}

	public static void SlowMonsters()
	{
		MonkeyeAI[] allType = Main.GetAllType<MonkeyeAI>(5f);
		foreach (MonkeyeAI val in allType)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
				break;
			}
			val.speed = 0.02f;
		}
	}

	public static void FastMonsters()
	{
		MonkeyeAI[] allType = Main.GetAllType<MonkeyeAI>(5f);
		foreach (MonkeyeAI val in allType)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
				break;
			}
			val.speed = 0.5f;
		}
	}

	public static void FixMonsters()
	{
		MonkeyeAI[] allType = Main.GetAllType<MonkeyeAI>(5f);
		foreach (MonkeyeAI val in allType)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
				break;
			}
			val.speed = 0.1f;
		}
	}

	public static void GrabMonsters()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.rightGrab)
		{
			return;
		}
		MonkeyeAI[] allType = Main.GetAllType<MonkeyeAI>(5f);
		foreach (MonkeyeAI val in allType)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
				break;
			}
			((Component)val).gameObject.transform.position = GorillaTagger.Instance.rightHandTransform.position;
		}
	}

	public static void MonsterGun()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		GameObject item = Main.RenderGun().NewPointer;
		if (!Main.GetGunInput(isShooting: true))
		{
			return;
		}
		MonkeyeAI[] allType = Main.GetAllType<MonkeyeAI>(5f);
		foreach (MonkeyeAI val in allType)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
				break;
			}
			((Component)val).gameObject.transform.position = item.transform.position + Vector3.up;
		}
	}

	public static void SpazMonsters()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		MonkeyeAI[] allType = Main.GetAllType<MonkeyeAI>(5f);
		foreach (MonkeyeAI val in allType)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
				break;
			}
			((Component)val).transform.rotation = Quaternion.Euler(new Vector3((float)Random.Range(0, 360), (float)Random.Range(0, 360), (float)Random.Range(0, 360)));
		}
	}

	public static void OrbitMonsters()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		MonkeyeAI[] allType = Main.GetAllType<MonkeyeAI>(5f);
		int num = 0;
		MonkeyeAI[] allType2 = Main.GetAllType<MonkeyeAI>(5f);
		foreach (MonkeyeAI val in allType2)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
				break;
			}
			float num2 = 360f / (float)allType.Length * (float)num;
			((Component)val).transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(MathF.Cos(num2 + (float)Time.frameCount / 30f) * 2f, 1f, MathF.Sin(num2 + (float)Time.frameCount / 30f) * 2f);
			num++;
		}
	}

	public static void DestroyMonsters()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		MonkeyeAI[] allType = Main.GetAllType<MonkeyeAI>(5f);
		foreach (MonkeyeAI val in allType)
		{
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not master client.");
				break;
			}
			((Component)val).gameObject.transform.position = new Vector3(99999f, 99999f, 99999f);
		}
	}

	public static void GrabAllBlocksNearby()
	{
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab && Time.time > blockDelay)
		{
			blockDelay = Time.time + blockDebounce;
			BuilderPiece val = (from piece in Main.GetAllType<BuilderPiece>(5f)
				where ((Component)piece).gameObject.activeInHierarchy
				where !piece.isBuiltIntoTable
				where piece.heldByPlayerActorNumber != PhotonNetwork.LocalPlayer.ActorNumber
				where piece.CanPlayerGrabPiece(PhotonNetwork.LocalPlayer.ActorNumber, ((Component)piece).transform.position)
				where Vector3.Distance(((Component)piece).transform.position, Main.ServerLeftHandPos) < 2.5f
				orderby Vector3.Distance(((Component)piece).transform.position, Main.ServerLeftHandPos)
				select piece).FirstOrDefault();
			if ((Object)(object)val == (Object)null)
			{
				return;
			}
			RequestGrabPiece(val, isLefHand: false, Buttons.GetIndex("No Random Position Grab").enabled ? Vector3.zero : RandomUtilities.RandomVector3(0.5f), Buttons.GetIndex("No Random Rotation Grab").enabled ? Quaternion.identity : RandomUtilities.RandomQuaternion());
			potentialgrabbedpieces.Add(val);
			Main.RPCProtection();
		}
		if (!(Main.rightTrigger > 0.5f) || !(Time.time > blockDelay))
		{
			return;
		}
		int num = 0;
		blockDelay = Time.time + 0.3f;
		foreach (BuilderPiece item in from piece in Main.GetAllType<BuilderPiece>(5f)
			where ((Component)piece).gameObject.activeInHierarchy
			where piece.heldByPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber
			select piece)
		{
			if (num > 100)
			{
				break;
			}
			num++;
			RequestDropPiece(item, GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.rotation, RandomUtilities.RandomVector3(19f), RandomUtilities.RandomVector3(19f));
		}
		potentialgrabbedpieces.Clear();
		Main.RPCProtection();
	}

	public static void GrabAllSelectedNearby()
	{
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab && Time.time > blockDelay)
		{
			blockDelay = Time.time + blockDebounce;
			BuilderPiece val = (from piece in Main.GetAllType<BuilderPiece>(5f)
				where ((Component)piece).gameObject.activeInHierarchy
				where piece.pieceType == pieceIdSet
				where !piece.isBuiltIntoTable
				where piece.heldByPlayerActorNumber != PhotonNetwork.LocalPlayer.ActorNumber
				where piece.CanPlayerGrabPiece(PhotonNetwork.LocalPlayer.ActorNumber, ((Component)piece).transform.position)
				where Vector3.Distance(((Component)piece).transform.position, Main.ServerLeftHandPos) < 2.5f
				orderby Vector3.Distance(((Component)piece).transform.position, Main.ServerLeftHandPos)
				select piece).FirstOrDefault();
			if ((Object)(object)val == (Object)null)
			{
				return;
			}
			RequestGrabPiece(val, isLefHand: false, Buttons.GetIndex("No Random Position Grab").enabled ? Vector3.zero : RandomUtilities.RandomVector3(0.5f), Buttons.GetIndex("No Random Rotation Grab").enabled ? Quaternion.identity : RandomUtilities.RandomQuaternion());
			potentialgrabbedpieces.Add(val);
			Main.RPCProtection();
		}
		if (!(Main.rightTrigger > 0.5f) || !(Time.time > blockDelay))
		{
			return;
		}
		int num = 0;
		blockDelay = Time.time + 0.3f;
		foreach (BuilderPiece item in from piece in Main.GetAllType<BuilderPiece>(5f)
			where ((Component)piece).gameObject.activeInHierarchy
			where piece.heldByPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber
			select piece)
		{
			if (num > 100)
			{
				break;
			}
			num++;
			RequestDropPiece(item, GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.rotation, RandomUtilities.RandomVector3(19f), RandomUtilities.RandomVector3(19f));
		}
		potentialgrabbedpieces.Clear();
		Main.RPCProtection();
	}

	public static void PopAllBalloons()
	{
		BalloonHoldable[] allType = Main.GetAllType<BalloonHoldable>(5f);
		foreach (BalloonHoldable val in allType)
		{
			val.OwnerPopBalloon();
		}
	}

	public static void GrabBalloons()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.rightGrab)
		{
			return;
		}
		BalloonHoldable[] allType = Main.GetAllType<BalloonHoldable>(5f);
		foreach (BalloonHoldable val in allType)
		{
			if (((TransferrableObject)val).ownerRig.isLocal)
			{
				((Component)val).gameObject.transform.position = GorillaTagger.Instance.rightHandTransform.position;
			}
			else
			{
				((TransferrableObject)val).WorldShareableRequestOwnership();
			}
		}
	}

	public static void SpazBalloons()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		BalloonHoldable[] allType = Main.GetAllType<BalloonHoldable>(5f);
		foreach (BalloonHoldable val in allType)
		{
			if (((TransferrableObject)val).ownerRig.isLocal)
			{
				((Component)val).gameObject.transform.rotation = Quaternion.Euler(new Vector3((float)Random.Range(0, 360), (float)Random.Range(0, 360), (float)Random.Range(0, 360)));
			}
			else
			{
				((TransferrableObject)val).WorldShareableRequestOwnership();
			}
		}
	}

	public static void OrbitBalloons()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		BalloonHoldable[] allType = Main.GetAllType<BalloonHoldable>(5f);
		int num = 0;
		BalloonHoldable[] array = allType;
		foreach (BalloonHoldable val in array)
		{
			if (((TransferrableObject)val).ownerRig.isLocal)
			{
				float num2 = 360f / (float)allType.Length * (float)num;
				((Component)val).gameObject.transform.position = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(MathF.Cos(num2 + (float)Time.frameCount / 30f) * 5f, 2f, MathF.Sin(num2 + (float)Time.frameCount / 30f) * 5f);
			}
			else
			{
				((TransferrableObject)val).WorldShareableRequestOwnership();
			}
			num++;
		}
	}

	public static void BalloonGun()
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		GameObject item = Main.RenderGun().NewPointer;
		if (!Main.GetGunInput(isShooting: true))
		{
			return;
		}
		BalloonHoldable[] allType = Main.GetAllType<BalloonHoldable>(5f);
		foreach (BalloonHoldable val in allType)
		{
			if (((TransferrableObject)val).ownerRig.isLocal)
			{
				((Component)val).gameObject.transform.position = item.transform.position + Vector3.up;
			}
			else
			{
				((TransferrableObject)val).WorldShareableRequestOwnership();
			}
		}
	}

	public static void DestroyBalloons()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		BalloonHoldable[] allType = Main.GetAllType<BalloonHoldable>(5f);
		foreach (BalloonHoldable val in allType)
		{
			if (((TransferrableObject)val).ownerRig.isLocal)
			{
				((Component)val).gameObject.transform.position = new Vector3(99999f, 99999f, 99999f);
			}
			else
			{
				((TransferrableObject)val).WorldShareableRequestOwnership();
			}
		}
	}

	public static void BecomeBalloon()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		((Behaviour)VRRig.LocalRig).enabled = false;
		((Component)VRRig.LocalRig).transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position - Vector3.up * 99999f;
		BalloonHoldable[] allType = Main.GetAllType<BalloonHoldable>(5f);
		foreach (BalloonHoldable val in allType)
		{
			if (((TransferrableObject)val).ownerRig.isLocal)
			{
				((Component)val).gameObject.transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
				((Component)val).gameObject.transform.rotation = ((Component)GorillaTagger.Instance.headCollider).transform.rotation;
				break;
			}
			((TransferrableObject)val).WorldShareableRequestOwnership();
		}
	}

	public static void BecomeHoverboard()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
		Quaternion rotation = ((Component)GorillaTagger.Instance.headCollider).transform.rotation;
		((Behaviour)VRRig.LocalRig).enabled = false;
		((Component)VRRig.LocalRig).transform.position = val - Vector3.up * 1f;
		GTPlayer.Instance.SetHoverActive(true);
		HoverboardVisual hoverboardVisual = VRRig.LocalRig.hoverboardVisual;
		hoverboardVisual.SetIsHeld(true, hoverboardVisual.NominalParentTransform.InverseTransformPoint(val), GTExt.InverseTransformRotation(hoverboardVisual.NominalParentTransform, rotation), VRRig.LocalRig.playerColor);
		hoverboardVisual.interpolatedLocalPosition = hoverboardVisual.NominalLocalPosition;
		hoverboardVisual.interpolatedLocalRotation = hoverboardVisual.NominalLocalRotation;
		GTPlayer.Instance.SetHoverboardPosRot(val, rotation);
	}

	public static void DestroyGliders()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		GliderHoldable[] allType = Main.GetAllType<GliderHoldable>(5f);
		foreach (GliderHoldable val in allType)
		{
			if (((NetworkView)val).GetView.Owner == PhotonNetwork.LocalPlayer)
			{
				((Component)val).gameObject.transform.position = new Vector3(99999f, 99999f, 99999f);
			}
			else
			{
				((NetworkHoldableObject)val).OnHover((InteractionPoint)null, (GameObject)null);
			}
		}
	}

	public static void ChangeCycleDelay(bool positive = true)
	{
		if (positive)
		{
			cycleSpeedIndex++;
		}
		else
		{
			cycleSpeedIndex--;
		}
		if (cycleSpeedIndex > 4)
		{
			cycleSpeedIndex = 1;
		}
		if (cycleSpeedIndex < 1)
		{
			cycleSpeedIndex = 4;
		}
		nameCycleDebounce = (float)cycleSpeedIndex / 2f;
		Buttons.GetIndex("Change Cycle Delay").overlapText = "Change Name Cycle Delay <color=grey>[</color><color=green>" + nameCycleDebounce + "</color><color=grey>]</color>";
	}

	public static void GoldenNameTag(bool isGolden)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		VRRig.LocalRig.ShowGoldNameTag = isGolden;
		((Graphic)VRRig.LocalRig.playerText1).color = (VRRig.LocalRig.ShowGoldNameTag ? SubscriptionManager.SUBSCRIBER_NAME_COLOR : Color.white);
	}

	public static void FlashNameTag()
	{
		GoldenNameTag(Time.time % 0.2f > 0.1f);
	}

	public static void NameCycle(string[] names)
	{
		if (Time.time > nameCycleDelay)
		{
			nameCycleIndex++;
			if (nameCycleIndex > names.Length - 1)
			{
				nameCycleIndex = 0;
			}
			Main.ChangeName(names[nameCycleIndex]);
			nameCycleDelay = Time.time + nameCycleDebounce;
		}
	}

	public static void RandomNameCycle()
	{
		NameCycle(new string[1] { RandomUtilities.RandomString(8) });
	}

	public static void EnableCustomNameCycle()
	{
		if (File.Exists("SeralythMenu/Seralyth_CustomNameCycle.txt"))
		{
			names = File.ReadAllText("SeralythMenu/Seralyth_CustomNameCycle.txt").Split('\n');
		}
		else
		{
			File.WriteAllText("SeralythMenu/Seralyth_CustomNameCycle.txt", "YOUR\nTEXT\nHERE");
		}
	}

	public static void AnimatedName()
	{
		if (!PhotonNetwork.InRoom)
		{
			Main.ChangeName(name);
			return;
		}
		if (string.IsNullOrEmpty(name))
		{
			name = PhotonNetwork.LocalPlayer.NickName;
		}
		int length = Mathf.Clamp((int)Mathf.PingPong(Time.time / 0.25f, (float)name.Length) + 1, 1, name.Length);
		Main.ChangeName(name.Substring(0, length));
	}

	public static void FlashColor()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > colorChangerDelay)
		{
			colorChangerDelay = Time.time + 0.05f;
			strobeColor = !strobeColor;
			Main.ChangeColor(strobeColor ? Color.white : Color.black);
		}
	}

	public static void StrobeColor()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > colorChangerDelay)
		{
			colorChangerDelay = Time.time + 0.05f;
			Main.ChangeColor(RandomUtilities.RandomColor());
		}
	}

	public static void RainbowColor()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > colorChangerDelay)
		{
			colorChangerDelay = Time.time + 0.05f;
			float num = (float)Time.frameCount / 180f % 1f;
			Main.ChangeColor(Color.HSVToRGB(num, 1f, 1f));
		}
	}

	public static void HardRainbowColor()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > colorChangerDelay)
		{
			colorChangerDelay = Time.time + 0.5f;
			colorChangeType++;
			if (colorChangeType > 3)
			{
				colorChangeType = 0;
			}
			Color[] array = (Color[])(object)new Color[4]
			{
				Color.red,
				Color.green,
				Color.blue,
				Color.magenta
			};
			Main.ChangeColor(array[colorChangeType]);
		}
	}

	public static void BecomePlayer(string name, Color color)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		Main.ChangeName(name);
		Main.ChangeColor(color);
	}

	public static void BecomeMinigamesKid()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		string[] array = new string[5] { "MINIGAMES", "MINIGAMESKID", "LITTLETIMMY", "TIMMY", "SILLYBILLY" };
		Color[] array2 = (Color[])(object)new Color[4]
		{
			Color.cyan,
			Color.green,
			Color.red,
			Color.magenta
		};
		BecomePlayer(array[Random.Range(0, array.Length)], array2[Random.Range(0, array2.Length)]);
	}

	public static void CopyIdentityGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > stealIdentityDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				Main.ChangeName(RigUtilities.GetPlayerFromVRRig(componentInParent).NickName);
				Main.ChangeColor(componentInParent.playerColor);
				stealIdentityDelay = Time.time + 0.5f;
			}
		}
	}

	public static void CopyCosmeticsGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > stealCosmeticsDelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", (RpcTarget)0, new object[3]
				{
					componentInParent.cosmeticSet.ToPackedIDArray(),
					componentInParent.tryOnSet.ToPackedIDArray(),
					false
				});
				stealCosmeticsDelay = Time.time + 0.5f;
			}
		}
	}

	public static void ChangeAccessories()
	{
		if (Main.leftGrab && !lastHitL)
		{
			hat--;
			if (hat < 1)
			{
				hat = 3;
			}
			switch (hat)
			{
			case 1:
				Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
				break;
			case 2:
				Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (1)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
				break;
			case 3:
				Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (2)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
				break;
			}
		}
		lastHitL = Main.leftGrab;
		if (Main.rightGrab && !lastHitR)
		{
			hat++;
			if (hat > 3)
			{
				hat = 1;
			}
			switch (hat)
			{
			case 1:
				Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
				break;
			case 2:
				Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (1)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
				break;
			case 3:
				Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (2)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
				break;
			}
		}
		lastHitR = Main.rightGrab;
		if (Main.leftPrimary && !lastHitLP)
		{
			((GorillaPressableButton)Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeLeftButton").GetComponent<WardrobeFunctionButton>()).ButtonActivation();
			switch (hat)
			{
			case 1:
				Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
				break;
			case 2:
				Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (1)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
				break;
			case 3:
				Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (2)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
				break;
			}
		}
		lastHitLP = Main.leftPrimary;
		if (Main.rightPrimary && !lastHitRP)
		{
			((GorillaPressableButton)Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeRightItem").GetComponent<WardrobeFunctionButton>()).ButtonActivation();
			switch (hat)
			{
			case 1:
				Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
				break;
			case 2:
				Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (1)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
				break;
			case 3:
				Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeItemButton (2)").GetComponent<GorillaPressableButton>().ButtonActivationWithHand(false);
				break;
			}
		}
		lastHitRP = Main.rightPrimary;
		if (Main.rightSecondary && !lastHitRS)
		{
			accessoryType++;
			if (accessoryType > 4)
			{
				accessoryType = 1;
			}
			switch (accessoryType)
			{
			case 1:
				((GorillaPressableButton)Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardobeHatButton").GetComponent<WardrobeFunctionButton>()).ButtonActivation();
				break;
			case 2:
				((GorillaPressableButton)Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeFaceButton").GetComponent<WardrobeFunctionButton>()).ButtonActivation();
				break;
			case 3:
				((GorillaPressableButton)Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeBadgeButton").GetComponent<WardrobeFunctionButton>()).ButtonActivation();
				break;
			case 4:
				((GorillaPressableButton)Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables/UI/Wardrobe/WardrobeHoldableButton").GetComponent<WardrobeFunctionButton>()).ButtonActivation();
				break;
			}
		}
		lastHitRS = Main.rightSecondary;
	}

	public static int[] PackCosmetics(string[] unpackedCosmetics)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		if (cachePacked.TryGetValue(unpackedCosmetics, out var value))
		{
			return value;
		}
		CosmeticSet val = new CosmeticSet(unpackedCosmetics, CosmeticsController.instance);
		int[] array = val.ToPackedIDArray();
		cachePacked.Add(unpackedCosmetics, array);
		return array;
	}

	private static string[] GetOwnedCosmetics()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (ownedArchive != null)
		{
			return ownedArchive.ToArray();
		}
		ownedArchive = new List<string>();
		foreach (CosmeticItem item in ((CosmeticsController)CosmeticsController.instance).allCosmetics.Where((CosmeticItem cosmeticItem) => VRRig.LocalRig._playerOwnedCosmetics.Contains(cosmeticItem.itemName)))
		{
			ownedArchive.Add(item.itemName);
		}
		return ownedArchive.ToArray();
	}

	private static string[] GetTryOnCosmetics()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (tryOnCosmetics != null)
		{
			return tryOnCosmetics.ToArray();
		}
		tryOnCosmetics = new List<string>();
		foreach (CosmeticItem item in ((CosmeticsController)CosmeticsController.instance).allCosmetics.Where((CosmeticItem cosmeticItem) => cosmeticItem.canTryOn))
		{
			tryOnCosmetics.Add(item.itemName);
		}
		return tryOnCosmetics.ToArray();
	}

	private static string[] GetTryOnBalloons()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (tryOnCosmetics != null)
		{
			return tryOnCosmetics.ToArray();
		}
		tryOnCosmetics = new List<string>();
		foreach (CosmeticItem item in ((CosmeticsController)CosmeticsController.instance).allCosmetics.Where((CosmeticItem cosmeticItem) => cosmeticItem.canTryOn && cosmeticItem.overrideDisplayName.ToLower().Contains("balloon")))
		{
			tryOnCosmetics.Add(item.itemName);
		}
		return tryOnCosmetics.ToArray();
	}

	private static string[] GetOwnedBalloons()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (ownedArchive == null)
		{
			ownedArchive = new List<string>();
			foreach (CosmeticItem item in ((CosmeticsController)CosmeticsController.instance).allCosmetics.Where((CosmeticItem cosmeticItem) => VRRig.LocalRig._playerOwnedCosmetics.Contains(cosmeticItem.itemName) && cosmeticItem.overrideDisplayName.ToLower().Contains("balloon")))
			{
				ownedArchive.Add(item.itemName);
			}
		}
		return ownedArchive.ToArray();
	}

	public static void SpazAccessories()
	{
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Expected O, but got Unknown
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected O, but got Unknown
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		if (!(Main.rightTrigger > 0.5f) || !(Time.time > delay))
		{
			return;
		}
		delay = Time.time + 0.05f;
		string[] array = (VRRig.LocalRig.inTryOnRoom ? GetTryOnCosmetics() : GetOwnedCosmetics());
		int num = Math.Clamp(array.Length, 0, 15);
		if (num > 0)
		{
			List<string> list = new List<string>();
			for (int i = 0; i <= num; i++)
			{
				list.Add(array[Random.Range(0, array.Length)]);
			}
			if (VRRig.LocalRig.inTryOnRoom)
			{
				((CosmeticsController)CosmeticsController.instance).tryOnSet = new CosmeticSet(list.ToArray(), CosmeticsController.instance);
				VRRig.LocalRig.tryOnSet = new CosmeticSet(list.ToArray(), CosmeticsController.instance);
			}
			else
			{
				((CosmeticsController)CosmeticsController.instance).currentWornSet = new CosmeticSet(list.ToArray(), CosmeticsController.instance);
				VRRig.LocalRig.cosmeticSet = new CosmeticSet(list.ToArray(), CosmeticsController.instance);
			}
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", (RpcTarget)0, new object[3]
			{
				PackCosmetics(list.ToArray()),
				PackCosmetics(list.ToArray()),
				false
			});
			Main.RPCProtection();
		}
	}

	public static void SpazAccessoriesBalloon()
	{
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Expected O, but got Unknown
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected O, but got Unknown
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		if (!(Main.rightTrigger > 0.5f) || !(Time.time > delay))
		{
			return;
		}
		delay = Time.time + 0.05f;
		string[] array = (VRRig.LocalRig.inTryOnRoom ? GetTryOnBalloons() : GetOwnedBalloons());
		int num = Math.Clamp(array.Length, 0, 15);
		if (num > 0)
		{
			List<string> list = new List<string>();
			for (int i = 0; i <= num; i++)
			{
				list.Add(array[Random.Range(0, array.Length)]);
			}
			if (VRRig.LocalRig.inTryOnRoom)
			{
				((CosmeticsController)CosmeticsController.instance).tryOnSet = new CosmeticSet(list.ToArray(), CosmeticsController.instance);
				VRRig.LocalRig.tryOnSet = new CosmeticSet(list.ToArray(), CosmeticsController.instance);
			}
			else
			{
				((CosmeticsController)CosmeticsController.instance).currentWornSet = new CosmeticSet(list.ToArray(), CosmeticsController.instance);
				VRRig.LocalRig.cosmeticSet = new CosmeticSet(list.ToArray(), CosmeticsController.instance);
			}
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", (RpcTarget)0, new object[3]
			{
				PackCosmetics(list.ToArray()),
				PackCosmetics(list.ToArray()),
				false
			});
			Main.RPCProtection();
		}
	}

	public static void SpazAccessoriesOthers()
	{
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Expected O, but got Unknown
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected O, but got Unknown
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		if (!(Main.rightTrigger > 0.5f) || !(Time.time > delay))
		{
			return;
		}
		delay = Time.time + 0.05f;
		string[] array = (VRRig.LocalRig.inTryOnRoom ? GetTryOnCosmetics() : GetOwnedCosmetics());
		int num = Math.Clamp(array.Length, 0, 15);
		if (num > 0)
		{
			List<string> list = new List<string>();
			for (int i = 0; i <= num; i++)
			{
				list.Add(array[Random.Range(0, array.Length)]);
			}
			if (VRRig.LocalRig.inTryOnRoom)
			{
				((CosmeticsController)CosmeticsController.instance).tryOnSet = new CosmeticSet(list.ToArray(), CosmeticsController.instance);
				VRRig.LocalRig.tryOnSet = new CosmeticSet(list.ToArray(), CosmeticsController.instance);
			}
			else
			{
				((CosmeticsController)CosmeticsController.instance).currentWornSet = new CosmeticSet(list.ToArray(), CosmeticsController.instance);
				VRRig.LocalRig.cosmeticSet = new CosmeticSet(list.ToArray(), CosmeticsController.instance);
			}
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", (RpcTarget)0, new object[3]
			{
				PackCosmetics(list.ToArray()),
				PackCosmetics(list.ToArray()),
				false
			});
			Main.RPCProtection();
		}
	}

	public static void StickyHoldables()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Invalid comparison between Unknown and I4
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Invalid comparison between Unknown and I4
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Invalid comparison between Unknown and I4
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Invalid comparison between Unknown and I4
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Invalid comparison between Unknown and I4
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if (!(Time.time > delayonhold))
		{
			return;
		}
		delayonhold = Time.time + 0.1f;
		TransferrableObject[] allType = Main.GetAllType<TransferrableObject>(5f);
		foreach (TransferrableObject val in allType)
		{
			if (val.IsMyItem())
			{
				if ((int)val.currentState == 1 || (int)val.currentState == 32)
				{
					val.currentState = (PositionState)4;
				}
				if ((int)val.currentState == 2 || (int)val.currentState == 64 || (int)val.currentState == 16)
				{
					val.currentState = (PositionState)8;
				}
			}
		}
	}

	public static void SpazHoldables()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Invalid comparison between Unknown and I4
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (!(Time.time > delayonhold))
		{
			return;
		}
		delayonhold = Time.time + 0.1f;
		TransferrableObject[] allType = Main.GetAllType<TransferrableObject>(5f);
		foreach (TransferrableObject val in allType)
		{
			if (val.IsMyItem())
			{
				val.currentState = (PositionState)(val.currentState * 2);
				if ((int)val.currentState > 128)
				{
					val.currentState = (PositionState)1;
				}
			}
		}
	}

	public static void TryOnAnywhere()
	{
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		string[] array = new string[16]
		{
			"LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.",
			"LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU."
		};
		archiveCosmetics = ((CosmeticsController)CosmeticsController.instance).currentWornSet.ToPackedIDArray();
		((CosmeticsController)CosmeticsController.instance).currentWornSet = new CosmeticSet(array, CosmeticsController.instance);
		VRRig.LocalRig.cosmeticSet = new CosmeticSet(array, CosmeticsController.instance);
		GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", (RpcTarget)0, new object[3]
		{
			PackCosmetics(array),
			((CosmeticsController)CosmeticsController.instance).tryOnSet.ToPackedIDArray(),
			false
		});
		Main.RPCProtection();
	}

	public static void TryOffAnywhere()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		((CosmeticsController)CosmeticsController.instance).currentWornSet = new CosmeticSet(archiveCosmetics, CosmeticsController.instance);
		VRRig.LocalRig.cosmeticSet = new CosmeticSet(archiveCosmetics, CosmeticsController.instance);
		GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", (RpcTarget)0, new object[3]
		{
			archiveCosmetics,
			((CosmeticsController)CosmeticsController.instance).tryOnSet.ToPackedIDArray(),
			false
		});
		Main.RPCProtection();
	}

	public static void AddCosmeticToCart(string cosmetic)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		((CosmeticsController)CosmeticsController.instance).currentCart.Insert(0, ((CosmeticsController)CosmeticsController.instance).GetItemFromDict(cosmetic));
		((CosmeticsController)CosmeticsController.instance).UpdateShoppingCart();
	}

	public static void PurchaseCosmetic(string cosmetic)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		CosmeticItem hat = ((CosmeticsController)CosmeticsController.instance).GetItemFromDict(cosmetic);
		PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
		{
			ItemId = hat.itemName,
			Price = hat.cost,
			VirtualCurrency = ((CosmeticsController)CosmeticsController.instance).currencyName,
			CatalogVersion = ((CosmeticsController)CosmeticsController.instance).catalog
		}, (Action<PurchaseItemResult>)delegate
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Item \"" + Main.ToTitleCase(hat.overrideDisplayName) + "\" has been purchased.", 5000);
			((CosmeticsController)CosmeticsController.instance).ProcessExternalUnlock(hat.itemName, false, false);
			CosmeticsController instance = CosmeticsController.instance;
			((CosmeticsController)instance).currencyBalance = ((CosmeticsController)instance).currencyBalance - hat.cost;
			Main.CosmeticsOwned += hat.itemName;
			if ((Object)(object)PCOnGUIMenu.Instance != (Object)null)
			{
				PCOnGUIMenu.Instance.SelectTab("Cosmetics");
			}
		}, (Action<PlayFabError>)null, (object)null, (Dictionary<string, string>)null);
	}

	public static void CosmeticBrowser()
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		rememberdirectory = Main.pageNumber;
		List<ButtonInfo> list = new List<ButtonInfo>();
		list.Add(new ButtonInfo
		{
			buttonText = "Exit Cosmetic Browser",
			method = RemoveCosmeticBrowser,
			isTogglable = false,
			toolTip = "Returns you back to the fun mods."
		});
		List<ButtonInfo> list2 = list;
		foreach (CosmeticItem hat in ((CosmeticsController)CosmeticsController.instance).allCosmetics)
		{
			if (hat.canTryOn)
			{
				list2.Add(new ButtonInfo
				{
					buttonText = Main.ToTitleCase(hat.overrideDisplayName),
					method = delegate
					{
						AddCosmeticToCart(hat.itemName);
					},
					isTogglable = false,
					toolTip = "Adds the " + hat.overrideDisplayName.ToLower() + "to your cart."
				});
			}
		}
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list2.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void RemoveCosmeticBrowser()
	{
		Main.pageNumber = rememberdirectory;
		Buttons.CurrentCategoryName = "Fun Mods";
	}

	public static void AutoLoadCosmetics()
	{
		RequestPatch.enabled = true;
		if (RequestPatch.currentCoroutine == null)
		{
			RequestPatch.currentCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(RequestPatch.LoadCosmetics());
		}
		((Behaviour)Main.TryOnRoom.GetComponent<CosmeticBoundaryTrigger>()).enabled = false;
	}

	public static void NoAutoLoadCosmetics()
	{
		((Behaviour)Main.TryOnRoom.GetComponent<CosmeticBoundaryTrigger>()).enabled = true;
		RequestPatch.enabled = false;
	}

	public static void AutoPurchaseCosmetics()
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (!((GorillaComputer)GorillaComputer.instance).isConnectedToMaster)
		{
			lastTimeCosmeticsChecked = Time.time + 120f;
		}
		if (!(Time.time > lastTimeCosmeticsChecked))
		{
			return;
		}
		lastTimeCosmeticsChecked = Time.time + 120f;
		foreach (CosmeticItem item in ((CosmeticsController)CosmeticsController.instance).allCosmetics.Where((CosmeticItem hat) => hat.cost == 0 && hat.canTryOn && !Main.CosmeticsOwned.Contains(hat.itemName)))
		{
			PurchaseCosmetic(item.itemName);
		}
	}

	public static void AutoPurchasePaidCosmetics()
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		if (!((GorillaComputer)GorillaComputer.instance).isConnectedToMaster)
		{
			lastTimePaidCosmeticsChecked = Time.time + 120f;
		}
		if (!(Time.time > lastTimePaidCosmeticsChecked))
		{
			return;
		}
		lastTimePaidCosmeticsChecked = Time.time + 120f;
		CosmeticItem[] items = ((CosmeticsController)CosmeticsController.instance).currentWornSet.items;
		int num = ((CosmeticsController)CosmeticsController.instance).CurrencyBalance;
		foreach (CosmeticItem item in from i in items
			orderby (int)i.itemCategory == 1 descending, ((int)i.itemCategory == 1) ? i.itemName : null
			select i)
		{
			if (num >= item.cost && item.canTryOn && !Main.CosmeticsOwned.Contains(item.itemName))
			{
				PurchaseCosmetic(item.itemName);
				num -= item.cost;
			}
		}
	}

	public static void DisableCosmeticsOnTag()
	{
		if (!lasttagged && VRRig.LocalRig.IsTagged())
		{
			string[] unpackedCosmetics = new string[16]
			{
				"null", "null", "null", "null", "null", "null", "null", "null", "null", "null",
				"null", "null", "null", "null", "null", "null"
			};
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", (RpcTarget)1, new object[3]
			{
				PackCosmetics(unpackedCosmetics),
				PackCosmetics(unpackedCosmetics),
				false
			});
			Main.RPCProtection();
		}
		if (lasttagged && !VRRig.LocalRig.IsTagged())
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", (RpcTarget)1, new object[3]
			{
				((CosmeticsController)CosmeticsController.instance).currentWornSet.ToPackedIDArray(),
				((CosmeticsController)CosmeticsController.instance).tryOnSet.ToPackedIDArray(),
				false
			});
			Main.RPCProtection();
		}
		lasttagged = VRRig.LocalRig.IsTagged();
	}

	public static void UnlockAllCosmetics()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		CosmeticPatch.enabled = true;
		if (!PostGetData.CosmeticsInitialized || hasGivenCosmetics)
		{
			return;
		}
		hasGivenCosmetics = true;
		MethodInfo method = typeof(CosmeticsController).GetMethod("UnlockItem", BindingFlags.Instance | BindingFlags.NonPublic);
		foreach (CosmeticItem item in ((CosmeticsController)CosmeticsController.instance).allCosmetics.Where((CosmeticItem item) => !((CosmeticsController)CosmeticsController.instance).concatStringCosmeticsAllowed.Contains(item.itemName)))
		{
			try
			{
				method.Invoke(CosmeticsController.instance, new object[2] { item.itemName, false });
			}
			catch
			{
			}
		}
	}

	public static void CopyIDGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > idgundelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				idgundelay = Time.time + 0.5f;
				string userId = RigUtilities.GetPlayerFromVRRig(componentInParent).UserId;
				NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> " + userId, 5000);
				GUIUtility.systemCopyBuffer = userId;
			}
		}
	}

	public static void CopyIDAura()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<VRRig> list = new List<VRRig>();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (Vector3.Distance(((Component)activeRig).transform.position, ((Component)VRRig.LocalRig).transform.position) < 4f && !activeRig.IsLocal())
			{
				list.Add(activeRig);
			}
			else if (list.Contains(activeRig))
			{
				list.Remove(activeRig);
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		foreach (string item in list.Select((VRRig nearbyPlayer) => RigUtilities.GetPlayerFromVRRig(nearbyPlayer).UserId))
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> " + item, 5000);
			GUIUtility.systemCopyBuffer = item;
		}
	}

	public static void CopyIDOnTouch()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<VRRig> list = new List<VRRig>();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.IsLocal() && (Vector3.Distance(((Component)activeRig).transform.position, VRRig.LocalRig.rightHandTransform.position) <= 0.35f || Vector3.Distance(((Component)activeRig).transform.position, VRRig.LocalRig.leftHandTransform.position) <= 0.35f))
			{
				list.Add(activeRig);
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		foreach (string item in list.Select((VRRig rig) => RigUtilities.GetPlayerFromVRRig(rig).UserId))
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> " + item, 5000);
			GUIUtility.systemCopyBuffer = item;
		}
	}

	public static void CopyIDAll()
	{
		foreach (string item in VRRigCache.ActiveRigs.Select((VRRig vrrig) => RigUtilities.GetPlayerFromVRRig(vrrig).UserId))
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> " + item, 5000);
			GUIUtility.systemCopyBuffer = item;
		}
	}

	public static void CopySelfID()
	{
		string userId = PhotonNetwork.LocalPlayer.UserId;
		NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> " + userId, 5000);
		GUIUtility.systemCopyBuffer = userId;
	}

	public static void NarrateIDGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > idgundelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				idgundelay = Time.time + 0.5f;
				Main.SpeakText("Name: " + RigUtilities.GetPlayerFromVRRig(componentInParent).NickName + ". I D: " + string.Join(" ", RigUtilities.GetPlayerFromVRRig(componentInParent).UserId));
			}
		}
	}

	public static void NarrateIDAll()
	{
		string text = "";
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.isLocal)
			{
				text = text + "Name: " + RigUtilities.GetPlayerFromVRRig(activeRig).NickName + ". I D: " + string.Join(" ", RigUtilities.GetPlayerFromVRRig(activeRig).UserId) + ". ";
			}
		}
		Main.SpeakText(text);
	}

	public static void NarrateIDAura()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<VRRig> list = new List<VRRig>();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (Vector3.Distance(((Component)activeRig).transform.position, ((Component)VRRig.LocalRig).transform.position) < 4f && !activeRig.IsLocal())
			{
				list.Add(activeRig);
			}
			else if (list.Contains(activeRig))
			{
				list.Remove(activeRig);
			}
		}
		if (list.Count <= 0 || !(Time.time > allNarrationDelay))
		{
			return;
		}
		allNarrationDelay = Time.time + 10f;
		string text = "";
		foreach (VRRig item in list)
		{
			text = text + "Name: " + RigUtilities.GetPlayerFromVRRig(item).NickName + ". I D: " + string.Join(" ", RigUtilities.GetPlayerFromVRRig(item).UserId) + ". ";
		}
		Main.SpeakText(text);
	}

	public static void NarrateIDOnTouch()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<VRRig> list = new List<VRRig>();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.IsLocal() && (Vector3.Distance(((Component)activeRig).transform.position, VRRig.LocalRig.rightHandTransform.position) <= 0.35f || Vector3.Distance(((Component)activeRig).transform.position, VRRig.LocalRig.leftHandTransform.position) <= 0.35f))
			{
				list.Add(activeRig);
			}
		}
		if (list.Count <= 0 || !(Time.time > allNarrationDelay))
		{
			return;
		}
		allNarrationDelay = Time.time + 10f;
		string text = "";
		foreach (VRRig item in list)
		{
			text = text + "Name: " + RigUtilities.GetPlayerFromVRRig(item).NickName + ". I D: " + string.Join(" ", RigUtilities.GetPlayerFromVRRig(item).UserId) + ". ";
		}
		Main.SpeakText(text);
	}

	public static void NarrateSelfID()
	{
		Main.SpeakText("Name: " + PhotonNetwork.LocalPlayer.NickName + ". I D: " + string.Join(" ", PhotonNetwork.LocalPlayer.UserId));
	}

	public static void NarrateFakeDoxxGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (Main.GetGunInput(isShooting: true) && Time.time > idgundelay)
		{
			VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
			if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
			{
				idgundelay = Time.time + 0.5f;
				Main.SpeakText("Name: " + RigUtilities.GetPlayerFromVRRig(componentInParent).NickName + ". I P  ADD DRESS: " + string.Join(" ", $"{Random.Range(1, 255)}.{Random.Range(1, 255)}.{Random.Range(1, 255)}"));
			}
		}
	}

	public static void NarrateFakeDoxxAll()
	{
		string text = "";
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.isLocal)
			{
				text = text + "Name: " + RigUtilities.GetPlayerFromVRRig(activeRig).NickName + ". I P  ADD DRESS: " + string.Join(" ", $"{Random.Range(1, 255)}.{Random.Range(1, 255)}.{Random.Range(1, 255)}") + ". ";
			}
		}
		Main.SpeakText(text);
	}

	public static void NarrateFakeDoxxAura()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<VRRig> list = new List<VRRig>();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (Vector3.Distance(((Component)activeRig).transform.position, ((Component)VRRig.LocalRig).transform.position) < 4f && !activeRig.IsLocal())
			{
				list.Add(activeRig);
			}
			else if (list.Contains(activeRig))
			{
				list.Remove(activeRig);
			}
		}
		if (list.Count <= 0 || !(Time.time > allNarrationDelay))
		{
			return;
		}
		allNarrationDelay = Time.time + 10f;
		string text = "";
		foreach (VRRig item in list)
		{
			text = text + "Name: " + RigUtilities.GetPlayerFromVRRig(item).NickName + ". I P  ADD DRESS: " + string.Join(" ", $"{Random.Range(1, 255)}.{Random.Range(1, 255)}.{Random.Range(1, 255)}") + ". ";
		}
		Main.SpeakText(text);
	}

	public static void NarrateFakeDoxxOnTouch()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<VRRig> list = new List<VRRig>();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.IsLocal() && (Vector3.Distance(((Component)activeRig).transform.position, VRRig.LocalRig.rightHandTransform.position) <= 0.35f || Vector3.Distance(((Component)activeRig).transform.position, VRRig.LocalRig.leftHandTransform.position) <= 0.35f))
			{
				list.Add(activeRig);
			}
		}
		if (list.Count <= 0 || !(Time.time > allNarrationDelay))
		{
			return;
		}
		allNarrationDelay = Time.time + 10f;
		string text = "";
		foreach (VRRig item in list)
		{
			text = text + "Name: " + RigUtilities.GetPlayerFromVRRig(item).NickName + ". I P  ADD DRESS: " + string.Join(" ", $"{Random.Range(1, 255)}.{Random.Range(1, 255)}.{Random.Range(1, 255)}") + ". ";
		}
		Main.SpeakText(text);
	}

	public static void NarrateFakeDoxxSelf()
	{
		Main.SpeakText("Name: " + PhotonNetwork.LocalPlayer.NickName + ". I P  ADD DRESS: " + string.Join(" ", $"{Random.Range(1, 255)}.{Random.Range(1, 255)}.{Random.Range(1, 255)}"));
	}

	public static void CopyCreationDateSelf()
	{
		string creationDate = RigUtilities.GetCreationDate(PhotonNetwork.LocalPlayer.UserId, CopyCreationDate);
		if (creationDate != "Loading...")
		{
			CopyCreationDate(creationDate);
		}
	}

	public static void CopyCreationDateGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (!Main.GetGunInput(isShooting: true))
		{
			return;
		}
		VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
		if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal() && Time.time > creationDateDelay)
		{
			creationDateDelay = Time.time + 0.5f;
			string creationDate = RigUtilities.GetCreationDate(RigUtilities.GetPlayerFromVRRig(componentInParent).UserId, CopyCreationDate);
			if (creationDate != "Loading...")
			{
				CopyCreationDate(creationDate);
			}
		}
	}

	public static void CopyCreationDateAura()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<VRRig> list = new List<VRRig>();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (Vector3.Distance(((Component)activeRig).transform.position, ((Component)VRRig.LocalRig).transform.position) < 4f && !activeRig.IsLocal())
			{
				list.Add(activeRig);
			}
			else if (list.Contains(activeRig))
			{
				list.Remove(activeRig);
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		foreach (string item in from nearbyPlayer in list
			select RigUtilities.GetCreationDate(RigUtilities.GetPlayerFromVRRig(nearbyPlayer).UserId, CopyCreationDate) into date
			where date != "Loading..."
			select date)
		{
			CopyCreationDate(item);
		}
	}

	public static void CopyCreationDateOnTouch()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<VRRig> list = new List<VRRig>();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!activeRig.IsLocal() && (Vector3.Distance(((Component)activeRig).transform.position, VRRig.LocalRig.rightHandTransform.position) <= 0.35f || Vector3.Distance(((Component)activeRig).transform.position, VRRig.LocalRig.leftHandTransform.position) <= 0.35f))
			{
				list.Add(activeRig);
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		foreach (string item in from rig in list
			select RigUtilities.GetCreationDate(RigUtilities.GetPlayerFromVRRig(rig).UserId, CopyCreationDate) into date
			where date != "Loading..."
			select date)
		{
			CopyCreationDate(item);
		}
	}

	public static void CopyCreationDateAll()
	{
		foreach (string item in from vrrig in VRRigCache.ActiveRigs
			select RigUtilities.GetCreationDate(RigUtilities.GetPlayerFromVRRig(vrrig).UserId, CopyCreationDate) into date
			where date != "Loading..."
			select date)
		{
			CopyCreationDate(item);
		}
	}

	public static void CopyCreationDate(string date)
	{
		NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> " + date, 5000);
		GUIUtility.systemCopyBuffer = date;
	}

	public static void NarrateCreationDateSelf()
	{
		string creationDate = RigUtilities.GetCreationDate(PhotonNetwork.LocalPlayer.UserId, Main.SpeakText);
		if (creationDate != "Loading...")
		{
			Main.SpeakText(creationDate);
		}
	}

	public static void NarrateCreationDateAll()
	{
		foreach (string item in from vrrig in VRRigCache.ActiveRigs
			select RigUtilities.GetCreationDate(RigUtilities.GetPlayerFromVRRig(vrrig).UserId, Main.SpeakText) into date
			where date != "Loading..."
			select date)
		{
			Main.SpeakText(item);
		}
	}

	public static void NarrateCreationDateAura()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<VRRig> list = new List<VRRig>();
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (Vector3.Distance(((Component)activeRig).transform.position, ((Component)VRRig.LocalRig).transform.position) < 4f && !activeRig.IsLocal())
			{
				list.Add(activeRig);
			}
			else if (list.Contains(activeRig))
			{
				list.Remove(activeRig);
			}
		}
		if (list.Count <= 0 || Time.time < allNarrationDelay)
		{
			return;
		}
		allNarrationDelay = Time.time + 10f;
		foreach (string item in from nearbyPlayer in list
			select RigUtilities.GetCreationDate(RigUtilities.GetPlayerFromVRRig(nearbyPlayer).UserId, delegate(string date)
			{
				Main.SpeakText(date);
			}) into date
			where date != "Loading..."
			select date)
		{
			Main.SpeakText(item);
		}
	}

	public static void NarrateCreationDateOnTouch()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<VRRig> list = (from rig in VRRigCache.ActiveRigs
			where !rig.IsLocal()
			where Vector3.Distance(((Component)rig).transform.position, VRRig.LocalRig.rightHandTransform.position) <= 0.35f || Vector3.Distance(((Component)rig).transform.position, VRRig.LocalRig.leftHandTransform.position) <= 0.35f
			select rig).ToList();
		if (list.Count <= 0 || Time.time < allNarrationDelay)
		{
			return;
		}
		allNarrationDelay = Time.time + 10f;
		foreach (string item in from rig in list
			select RigUtilities.GetCreationDate(RigUtilities.GetPlayerFromVRRig(rig).UserId, delegate(string date)
			{
				Main.SpeakText(date);
			}) into date
			where date != "Loading..."
			select date)
		{
			Main.SpeakText(item);
		}
	}

	public static void NarrateCreationDateGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (!Main.GetGunInput(isShooting: true))
		{
			return;
		}
		VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
		if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal() && Time.time > creationDateDelay)
		{
			creationDateDelay = Time.time + 0.5f;
			string creationDate = RigUtilities.GetCreationDate(RigUtilities.GetPlayerFromVRRig(componentInParent).UserId, delegate(string date)
			{
				Main.SpeakText(date);
			});
			if (creationDate != "Loading...")
			{
				Main.SpeakText(creationDate);
			}
		}
	}

	public static void GrabPlayerInfo()
	{
		string text = "Room: " + PhotonNetwork.CurrentRoom.Name;
		Player[] playerList = PhotonNetwork.PlayerList;
		foreach (Player val in playerList)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			string text2 = "";
			try
			{
				VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(NetPlayer.op_Implicit(val));
				num = vRRigFromPlayer.playerColor.r * 255f;
				num2 = vRRigFromPlayer.playerColor.g * 255f;
				num3 = vRRigFromPlayer.playerColor.b * 255f;
				text2 = vRRigFromPlayer.Cosmetics();
			}
			catch
			{
				LogManager.Log("Failed to log colors, rig most likely nonexistent");
			}
			try
			{
				text += "\n====================================\n";
				text = text + "Player Name: \"" + val.NickName + "\", Player ID: \"" + val.UserId + "\", Player Color: (R: " + num + ", G: " + num2 + ", B: " + num3 + "), Cosmetics: " + text2;
			}
			catch
			{
				LogManager.Log("Failed to log player");
			}
		}
		text += "\n====================================\n";
		text += "Text file generated with MrChicken Menu";
		string text3 = "SeralythMenu/PlayerInfo/" + PhotonNetwork.CurrentRoom.Name + ".txt";
		File.WriteAllText(text3, text);
		string fileName = FileUtilities.GetGamePath() + "/" + text3;
		Process.Start(fileName);
	}

	public static void RobuxSpam()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Expected O, but got Unknown
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Expected O, but got Unknown
		if (((ControllerInputPoller)ControllerInputPoller.instance).rightGrab)
		{
			GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
			if ((Object)(object)robuxTexture == (Object)null)
			{
				robuxTexture = new Texture2D(2, 2);
				byte[] array = File.ReadAllBytes("C:\\Users\\kalew\\OneDrive\\Pictures\\Screenshots\\images.jfif");
				ImageConversion.LoadImage(robuxTexture, array);
			}
			val.GetComponent<Renderer>().material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
			val.GetComponent<Renderer>().material.SetTexture("_BaseMap", (Texture)(object)robuxTexture);
			val.GetComponent<Renderer>().material.color = Color.white;
			val.transform.localScale = new Vector3(0.3f, 0.4f, 0.3f);
			val.transform.position = GTPlayer.Instance.RightHand.controllerTransform.position;
			val.transform.rotation = GTPlayer.Instance.RightHand.controllerTransform.rotation;
			Object.Destroy((Object)(object)val, 10f);
			Rigidbody val2 = val.AddComponent<Rigidbody>();
			val2.velocity = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0f, false);
		}
		if (((ControllerInputPoller)ControllerInputPoller.instance).leftGrab)
		{
			GameObject val3 = GameObject.CreatePrimitive((PrimitiveType)3);
			if ((Object)(object)robuxTexture == (Object)null)
			{
				robuxTexture = new Texture2D(2, 2);
				byte[] array2 = File.ReadAllBytes("C:\\Users\\kalew\\OneDrive\\Pictures\\Screenshots\\images.jfif");
				ImageConversion.LoadImage(robuxTexture, array2);
			}
			val3.GetComponent<Renderer>().material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
			val3.GetComponent<Renderer>().material.SetTexture("_BaseMap", (Texture)(object)robuxTexture);
			val3.GetComponent<Renderer>().material.color = Color.white;
			val3.transform.localScale = new Vector3(0.3f, 0.4f, 0.3f);
			val3.transform.position = GTPlayer.Instance.LeftHand.controllerTransform.position;
			val3.transform.rotation = GTPlayer.Instance.LeftHand.controllerTransform.rotation;
			Object.Destroy((Object)(object)val3, 10f);
			Rigidbody val4 = val3.AddComponent<Rigidbody>();
			val4.velocity = GTPlayer.Instance.LeftHand.velocityTracker.GetAverageVelocity(true, 0f, false);
		}
	}

	public static async void TrackEveryPlayer()
	{
		if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom == null || PhotonNetwork.PlayerList.Length == 0)
		{
			NotificationManager.SendNotification("<color=red>ERROR:</color> Not in a room.");
		}
		else
		{
			if (sending)
			{
				return;
			}
			sending = true;
			string webhook = "https://discord.com/api/webhooks/1481799083118170263/wUpCz_Bft1rxgbYfZutyWvP4OV-Te4U40s50q2mqz1DAvixa6nnR_ejEnyvN5tcXVaJK";
			List<Player> sortedPlayers = PhotonNetwork.PlayerList.OrderBy((Player val) => val.ActorNumber).ToList();
			int totalPlayers = sortedPlayers.Count;
			NotificationManager.SendNotification($"<color=green>SERALYTH</color> Logging {totalPlayers} players...");
			using HttpClient client = new HttpClient();
			for (int i = 0; i < sortedPlayers.Count; i++)
			{
				if (!PhotonNetwork.InRoom)
				{
					break;
				}
				Player p = sortedPlayers[i];
				int displayIndex = i + 1;
				string roleLabel = (p.IsMasterClient ? "?? MASTER" : "?? PLAYER");
				string roomName = PhotonNetwork.CurrentRoom.Name;
				string message = $"**[#{displayIndex} / {totalPlayers}] {roleLabel} DATA**\\n" + "**Name:** " + p.NickName + "\\n" + $"**Actor ID:** {p.ActorNumber}\\n" + "**Room:** " + roomName;
				StringContent payload = new StringContent("{\"content\":\"" + message + "\"}", Encoding.UTF8, "application/json");
				try
				{
					await client.PostAsync(webhook, payload);
					NotificationManager.SendNotification($"<color=green>[{displayIndex}/{totalPlayers}]</color> {p.NickName}");
				}
				catch
				{
					Debug.LogError((object)("Webhook failed for: " + p.NickName));
				}
				if (i < sortedPlayers.Count - 1)
				{
					await Task.Delay(8000);
				}
			}
			NotificationManager.SendNotification("<color=green>ALL PLAYERS LOGGED</color>");
			sending = false;
		}
	}

	public static void OrbGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		(RaycastHit, GameObject) tuple = Main.RenderGun();
		var (val, _) = tuple;
		if (Main.GetGunInput(isShooting: true))
		{
			GameObject val2 = GameObject.CreatePrimitive((PrimitiveType)0);
			val2.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			val2.transform.position = tuple.Item2.transform.position;
			val2.GetComponent<Renderer>().material.color = new Color(0.53f, 0.81f, 0.92f);
			Rigidbody val3 = val2.AddComponent<Rigidbody>();
			val3.mass = 0.5f;
			val3.useGravity = true;
			val3.collisionDetectionMode = (CollisionDetectionMode)1;
			Vector3 val4 = ((RaycastHit)(ref val)).point - tuple.Item2.transform.position;
			Vector3 val5 = ((Vector3)(ref val4)).normalized;
			if (((RaycastHit)(ref val)).point == Vector3.zero)
			{
				val5 = tuple.Item2.transform.forward;
			}
			val3.velocity = val5 * 25f;
			Object.Destroy((Object)(object)val2, 5f);
		}
	}

	public static void RobuxGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		(RaycastHit, GameObject) tuple = Main.RenderGun();
		var (val, _) = tuple;
		if (((ControllerInputPoller)ControllerInputPoller.instance).rightControllerIndexFloat > 0.5f)
		{
			GameObject val2 = GameObject.CreatePrimitive((PrimitiveType)3);
			if ((Object)(object)robuxTexture == (Object)null)
			{
				robuxTexture = new Texture2D(2, 2);
				byte[] array = File.ReadAllBytes("C:\\Users\\kalew\\OneDrive\\Pictures\\Screenshots\\images.jfif");
				ImageConversion.LoadImage(robuxTexture, array);
			}
			val2.GetComponent<Renderer>().material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
			val2.GetComponent<Renderer>().material.SetTexture("_BaseMap", (Texture)(object)robuxTexture);
			val2.GetComponent<Renderer>().material.color = Color.white;
			val2.transform.localScale = new Vector3(0.3f, 0.4f, 0.3f);
			val2.transform.position = tuple.Item2.transform.position;
			val2.transform.rotation = tuple.Item2.transform.rotation;
			Rigidbody val3 = val2.AddComponent<Rigidbody>();
			val3.mass = 0.5f;
			val3.useGravity = true;
			val3.velocity = tuple.Item2.transform.forward * 25f;
			Object.Destroy((Object)(object)val2, 10f);
		}
	}

	public static void ThinRobuxGun()
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		(RaycastHit, GameObject) tuple = Main.RenderGun();
		if (((ControllerInputPoller)ControllerInputPoller.instance).rightControllerIndexFloat > 0.5f && Time.time > shootTimer)
		{
			shootTimer = Time.time + shootDelay;
			GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
			if ((Object)(object)robuxTexture == (Object)null)
			{
				robuxTexture = new Texture2D(2, 2);
				byte[] array = File.ReadAllBytes("C:\\Users\\kalew\\OneDrive\\Pictures\\Screenshots\\images.jfif");
				ImageConversion.LoadImage(robuxTexture, array);
			}
			val.GetComponent<Renderer>().material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
			val.GetComponent<Renderer>().material.SetTexture("_BaseMap", (Texture)(object)robuxTexture);
			val.GetComponent<Renderer>().material.color = Color.white;
			val.transform.localScale = new Vector3(0.2f, 0.1f, 0.3f);
			val.transform.position = tuple.Item2.transform.position;
			val.transform.rotation = tuple.Item2.transform.rotation;
			Rigidbody val2 = val.AddComponent<Rigidbody>();
			val2.velocity = tuple.Item2.transform.forward * 30f;
			Object.Destroy((Object)(object)val, 5f);
		}
	}

	public static void OrbSpam()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		if (((ControllerInputPoller)ControllerInputPoller.instance).rightControllerGripFloat > 0.5f && Time.time > spamDelay)
		{
			spamDelay = Time.time + 0.02f;
			GameObject val = GameObject.CreatePrimitive((PrimitiveType)0);
			val.transform.localScale = new Vector3(0.12f, 0.12f, 0.12f);
			Transform rightHandTransform = GorillaTagger.Instance.rightHandTransform;
			val.transform.position = rightHandTransform.position;
			val.transform.rotation = rightHandTransform.rotation;
			val.GetComponent<Renderer>().material.color = new Color(0.53f, 0.81f, 0.92f);
			Rigidbody val2 = val.AddComponent<Rigidbody>();
			val2.useGravity = true;
			val2.velocity = rightHandTransform.forward * 35f;
			Object.Destroy((Object)(object)val, 10f);
		}
	}

	public static void OrbSpamV2()
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		if (((ControllerInputPoller)ControllerInputPoller.instance).rightControllerGripFloat > 0.5f)
		{
			if (orbLifeTimer <= 0f)
			{
				orbLifeTimer = Time.time + 10f;
				nextNotificationStep = 10;
				NotificationManager.SendNotification("<color=grey>[</color><color=green>V2</color><color=grey>]</color> Spawning: 10s Timer Started!");
			}
			if (Time.time > spamDelay)
			{
				spamDelay = Time.time + 0.02f;
				GameObject val = GameObject.CreatePrimitive((PrimitiveType)0);
				val.transform.localScale = new Vector3(0.12f, 0.12f, 0.12f);
				Transform rightHandTransform = GorillaTagger.Instance.rightHandTransform;
				val.transform.position = rightHandTransform.position;
				val.transform.rotation = rightHandTransform.rotation;
				val.GetComponent<Renderer>().material.color = new Color(0.53f, 0.81f, 0.92f);
				Rigidbody val2 = val.AddComponent<Rigidbody>();
				val2.useGravity = true;
				val2.velocity = rightHandTransform.forward * 35f;
				Object.Destroy((Object)(object)val, 10f);
			}
		}
		if (orbLifeTimer > 0f)
		{
			float num = orbLifeTimer - Time.time;
			int num2 = Mathf.CeilToInt(num);
			if (num2 <= nextNotificationStep && num2 >= 0)
			{
				string text = ((num2 <= 3) ? "red" : "green");
				NotificationManager.SendNotification("<color=green>V2</color>  First batch expires: <color=" + text + ">" + num2 + "</color>");
				nextNotificationStep = num2 - 1;
			}
			if (num <= 0f)
			{
				orbLifeTimer = -1f;
				NotificationManager.SendNotification("<color=red>[CLEAN]</color>  Cleanup Complete.");
			}
		}
	}

	public static void RGBStrobe()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		Color black = default(Color);
		if (Time.time * 20f % 2f > 1f)
		{
			float num = Mathf.PingPong(Time.time * 2f, 1f);
			float num2 = Mathf.PingPong(Time.time * 3f, 1f);
			float num3 = Mathf.PingPong(Time.time * 4f, 1f);
			((Color)(ref black))._002Ector(num, num2, num3);
		}
		else
		{
			black = Color.black;
		}
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		if ((Object)(object)offlineVRRig != (Object)null)
		{
			((Renderer)offlineVRRig.mainSkin).material.color = black;
			GorillaTagger.Instance.UpdateColor(black.r, black.g, black.b);
		}
	}

	public static void OrbGunV2()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			(RaycastHit, GameObject) tuple = Main.RenderGun();
			var (val, _) = tuple;
			if (Main.GetGunInput(isShooting: true))
			{
				if (orbLifeTimer <= 0f)
				{
					orbLifeTimer = Time.time + 10f;
					nextNotificationStep = 10;
					NotificationManager.SendNotification("<color=green>V2</color> Orb Gun Active: 10s Timer Started!");
				}
				if (Time.time > spamDelay)
				{
					spamDelay = Time.time + 0.02f;
					GameObject val2 = GameObject.CreatePrimitive((PrimitiveType)0);
					val2.transform.localScale = new Vector3(0.12f, 0.12f, 0.12f);
					val2.transform.position = tuple.Item2.transform.position;
					val2.GetComponent<Renderer>().material.color = new Color(0.53f, 0.81f, 0.92f);
					Rigidbody val3 = val2.AddComponent<Rigidbody>();
					val3.useGravity = true;
					Vector3 val4 = ((RaycastHit)(ref val)).point - tuple.Item2.transform.position;
					Vector3 val5 = ((Vector3)(ref val4)).normalized;
					if (((RaycastHit)(ref val)).point == Vector3.zero)
					{
						val5 = tuple.Item2.transform.forward;
					}
					val3.velocity = val5 * 40f;
					Object.Destroy((Object)(object)val2, 10f);
				}
			}
		}
		if (orbLifeTimer > 0f)
		{
			float num = orbLifeTimer - Time.time;
			int num2 = Mathf.CeilToInt(num);
			if (num2 <= nextNotificationStep && num2 >= 0)
			{
				string text = ((num2 <= 3) ? "red" : "green");
				NotificationManager.SendNotification("<color=green>V2</color>  Orbs expiring in: <color=" + text + ">" + num2 + "</color>");
				nextNotificationStep = num2 - 1;
			}
			if (num <= 0f)
			{
				orbLifeTimer = -1f;
				NotificationManager.SendNotification("<color=red>[CLEANCOMPLETE]</color>  First batch cleared.");
			}
		}
	}

	public static void UnmuteGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		var (val, _) = Main.RenderGun();
		if (!Main.GetGunInput(isShooting: true) || !(Time.time > unmuteDelay))
		{
			return;
		}
		VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
		if (!Object.op_Implicit((Object)(object)componentInParent) || componentInParent.IsLocal())
		{
			return;
		}
		NetPlayer player = RigUtilities.GetPlayerFromVRRig(componentInParent);
		foreach (GorillaPlayerScoreboardLine item in GorillaScoreboardTotalUpdater.allScoreboardLines.Where((GorillaPlayerScoreboardLine line) => line.linePlayer == player))
		{
			unmuteDelay = Time.time + 0.5f;
			if (item.muteButton.isOn)
			{
				item.muteButton.isOn = false;
				item.PressButton(false, (ButtonType)3);
			}
		}
	}

	public static async void DiscordLobbyLogger()
	{
		if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom == null)
		{
			return;
		}
		string currentCode = PhotonNetwork.CurrentRoom.Name;
		if (currentCode == lastSentCode || sending)
		{
			return;
		}
		lastSentCode = currentCode;
		sending = true;
		string webhook = "https://discord.com/api/webhooks/1429290589182230550/fFaEh_x0ZRJxDJyKFzqewk-Vnp2jEaBEhZioLA7qnB6lIdeUC9UloVOepwS7Vmy_pwv0";
		string message = "GTAG LOBBY INFO\nCode: " + currentCode + "\nPlayers: " + PhotonNetwork.PlayerList.Length;
		try
		{
			using (HttpClient client = new HttpClient())
			{
				StringContent payload = new StringContent("{\"content\":\"" + message.Replace("\n", "\\n") + "\"}", Encoding.UTF8, "application/json");
				await client.PostAsync(webhook, payload);
			}
			NotificationManager.SendNotification("<color=grey>[</color><color=green>WEBHOOK</color><color=grey>]</color> Sent lobby code");
		}
		catch
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>WEBHOOK ERROR</color><color=grey>]</color> Failed to send");
		}
		sending = false;
	}

	public static void July4thFireworks()
	{
	}

	public static void July4thFireworksV2()
	{
	}

	public static void FixJuly4thFireworks()
	{
	}
}
