using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using Seralyth.Classes.Menu;
using Seralyth.Classes.Mods;
using Seralyth.Extensions;
using Seralyth.Managers;
using Seralyth.Menu;
using Seralyth.Patches.Menu;
using Seralyth.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Windows.Speech;
using UnityEngine.XR;

namespace Seralyth.Mods;

public static class Settings
{
	public class TutorialButton : MonoBehaviour
	{
		public enum ButtonType
		{
			Pause,
			Close
		}

		public ButtonType buttonType;

		public void ClickButton()
		{
			switch (buttonType)
			{
			case ButtonType.Pause:
			{
				VideoPlayer component = ((Component)TutorialObject.transform.Find("Video")).GetComponent<VideoPlayer>();
				if (component.isPlaying)
				{
					component.Pause();
				}
				else
				{
					component.Play();
				}
				break;
			}
			case ButtonType.Close:
				Object.Destroy((Object)(object)TutorialObject);
				Object.Destroy((Object)(object)((Component)TutorialSelector).gameObject);
				break;
			}
		}
	}

	public enum ControllerBinding
	{
		None,
		LeftTrigger,
		RightTrigger,
		LeftGrip,
		RightGrip,
		LeftPrimaryButton,
		RightPrimaryButton,
		LeftSecondaryButton,
		RightSecondaryButton,
		JoystickClick,
		LeftOverride
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Func<VRRig, int> _003C_003E9__3_1;

		public static Func<VRRig, bool> _003C_003E9__3_2;

		public static Func<VRRig, int> _003C_003E9__3_3;

		public static Func<bool> _003C_003E9__3_0;

		public static Func<GameObject, GameObject> _003C_003E9__5_4;

		public static Func<string, Transform> _003C_003E9__5_0;

		public static Func<Transform, bool> _003C_003E9__5_1;

		public static Func<Transform, IEnumerable<GameObject>> _003C_003E9__5_2;

		public static Func<GameObject, GameObject> _003C_003E9__5_3;

		public static Action _003C_003E9__17_0;

		public static Action _003C_003E9__21_0;

		public static Action _003C_003E9__22_0;

		public static Action _003C_003E9__22_4;

		public static Action _003C_003E9__22_6;

		public static Action<string> _003C_003E9__22_29;

		public static Action _003C_003E9__26_0;

		public static Action _003C_003E9__64_0;

		public static Action _003C_003E9__64_1;

		public static Action _003C_003E9__64_2;

		public static Action _003C_003E9__64_3;

		public static Action _003C_003E9__65_0;

		public static Action _003C_003E9__65_1;

		public static Action _003C_003E9__65_2;

		public static Action _003C_003E9__66_0;

		public static Action _003C_003E9__66_1;

		public static Action _003C_003E9__66_2;

		public static Action _003C_003E9__66_3;

		public static Action _003C_003E9__66_4;

		public static Action _003C_003E9__66_5;

		public static Action _003C_003E9__66_6;

		public static Action _003C_003E9__66_7;

		public static Action _003C_003E9__66_8;

		public static Action _003C_003E9__66_9;

		public static Action _003C_003E9__67_0;

		public static Action _003C_003E9__67_1;

		public static Action _003C_003E9__67_2;

		public static Action _003C_003E9__67_3;

		public static Action _003C_003E9__67_4;

		public static Action _003C_003E9__67_5;

		public static Action _003C_003E9__67_6;

		public static Action _003C_003E9__67_7;

		public static Action _003C_003E9__67_8;

		public static Action _003C_003E9__67_9;

		public static Action _003C_003E9__69_0;

		public static Action _003C_003E9__70_0;

		public static Action _003C_003E9__70_1;

		public static Action _003C_003E9__70_2;

		public static Action _003C_003E9__71_0;

		public static Action _003C_003E9__71_1;

		public static Action _003C_003E9__71_2;

		public static Action _003C_003E9__71_3;

		public static Action _003C_003E9__71_4;

		public static Action _003C_003E9__71_5;

		public static Action _003C_003E9__71_6;

		public static Action _003C_003E9__71_7;

		public static Action _003C_003E9__71_8;

		public static Action _003C_003E9__71_9;

		public static Action _003C_003E9__72_0;

		public static Action _003C_003E9__72_1;

		public static Action _003C_003E9__72_2;

		public static Action _003C_003E9__72_3;

		public static Action _003C_003E9__72_4;

		public static Action _003C_003E9__72_5;

		public static Action _003C_003E9__72_6;

		public static Action _003C_003E9__72_7;

		public static Action _003C_003E9__72_8;

		public static Action _003C_003E9__72_9;

		public static Action _003C_003E9__73_0;

		public static Action _003C_003E9__73_1;

		public static Action _003C_003E9__73_2;

		public static Action _003C_003E9__73_3;

		public static Action _003C_003E9__73_4;

		public static Action _003C_003E9__73_5;

		public static Action _003C_003E9__73_6;

		public static Action _003C_003E9__73_7;

		public static Action _003C_003E9__73_8;

		public static Action _003C_003E9__73_9;

		public static Action _003C_003E9__74_0;

		public static Action _003C_003E9__74_1;

		public static Action _003C_003E9__74_2;

		public static Action _003C_003E9__74_3;

		public static Action _003C_003E9__74_4;

		public static Action _003C_003E9__74_5;

		public static Action _003C_003E9__74_6;

		public static Action _003C_003E9__74_7;

		public static Action _003C_003E9__74_8;

		public static Action _003C_003E9__76_0;

		public static Action _003C_003E9__76_1;

		public static Action _003C_003E9__76_2;

		public static Action _003C_003E9__76_3;

		public static Action _003C_003E9__76_4;

		public static Action _003C_003E9__76_5;

		public static Action _003C_003E9__76_6;

		public static Action _003C_003E9__76_7;

		public static Action _003C_003E9__76_8;

		public static Action _003C_003E9__77_0;

		public static Action _003C_003E9__77_1;

		public static Action _003C_003E9__77_2;

		public static Action _003C_003E9__77_3;

		public static Action _003C_003E9__77_4;

		public static Action _003C_003E9__77_5;

		public static Action _003C_003E9__77_6;

		public static Action _003C_003E9__77_7;

		public static Action _003C_003E9__77_8;

		public static Action _003C_003E9__77_9;

		public static Action _003C_003E9__78_0;

		public static Action _003C_003E9__78_1;

		public static Action _003C_003E9__78_2;

		public static Action _003C_003E9__78_3;

		public static Action _003C_003E9__78_4;

		public static Action _003C_003E9__78_5;

		public static Action _003C_003E9__78_6;

		public static Action _003C_003E9__78_7;

		public static Action _003C_003E9__78_8;

		public static Action _003C_003E9__78_9;

		public static Action _003C_003E9__109_0;

		public static Action _003C_003E9__109_1;

		public static Action _003C_003E9__124_2;

		public static Action _003C_003E9__124_0;

		public static Action<AudioClip> _003C_003E9__133_0;

		public static Action<AudioClip> _003C_003E9__134_1;

		public static Action<AudioClip> _003C_003E9__134_0;

		public static Action<AudioClip> _003C_003E9__136_0;

		public static PhraseRecognizedDelegate _003C_003E9__143_5;

		public static Action<AudioClip> _003C_003E9__144_4;

		public static Action<AudioClip> _003C_003E9__144_5;

		public static Action<AudioClip> _003C_003E9__144_6;

		public static DictationResultDelegate _003C_003E9__144_0;

		public static Action<AudioClip> _003C_003E9__144_7;

		public static DictationCompletedDelegate _003C_003E9__144_1;

		public static DictationHypothesisDelegate _003C_003E9__144_3;

		public static UnityAction _003C_003E9__167_2;

		public static UnityAction _003C_003E9__167_3;

		public static UnityAction _003C_003E9__167_4;

		public static Func<ButtonInfo, string> _003C_003E9__167_5;

		public static Func<string, ButtonInfo> _003C_003E9__167_8;

		public static Func<ButtonInfo, bool> _003C_003E9__167_9;

		public static Func<ButtonInfo[], IEnumerable<ButtonInfo>> _003C_003E9__167_10;

		public static Func<ButtonInfo, bool> _003C_003E9__167_11;

		public static Func<ButtonInfo, bool> _003C_003E9__167_12;

		public static UnityAction<string> _003C_003E9__167_7;

		internal bool _003CHandleBlockedPlayers_003Eb__3_0()
		{
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Expected O, but got Unknown
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Expected O, but got Unknown
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			if (Blocked.Count == 0)
			{
				return true;
			}
			int[] targetActors = Blocked.Select((VRRig rig) => rig.Creator.ActorNumber).ToArray();
			int[] targetActors2 = (from rig in VRRigExtensions.ActiveRigs
				where !Blocked.Contains(rig)
				select rig.Creator.ActorNumber).ToArray();
			Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
			Vector3 position = ((Component)VRRig.LocalRig).transform.position;
			Main.SendSerialize(VRRig.LocalRig.GetPhotonView(), new RaiseEventOptions
			{
				TargetActors = targetActors2
			});
			((Component)VRRig.LocalRig).transform.position = new Vector3(Random.Range(-99999f, 99999f), 99999f, Random.Range(-99999f, 99999f));
			Main.SendSerialize(VRRig.LocalRig.GetPhotonView(), new RaiseEventOptions
			{
				TargetActors = targetActors
			});
			Main.RPCProtection();
			((Component)VRRig.LocalRig).transform.position = position;
			return false;
		}

		internal int _003CHandleBlockedPlayers_003Eb__3_1(VRRig rig)
		{
			return rig.Creator.ActorNumber;
		}

		internal bool _003CHandleBlockedPlayers_003Eb__3_2(VRRig rig)
		{
			return !Blocked.Contains(rig);
		}

		internal int _003CHandleBlockedPlayers_003Eb__3_3(VRRig rig)
		{
			return rig.Creator.ActorNumber;
		}

		internal GameObject _003CSpawnKeyboard_003Eb__5_4(GameObject t)
		{
			return t.gameObject;
		}

		internal Transform _003CSpawnKeyboard_003Eb__5_0(string name)
		{
			return Main.VRKeyboard.transform.Find(name);
		}

		internal bool _003CSpawnKeyboard_003Eb__5_1(Transform t)
		{
			return (Object)(object)t != (Object)null;
		}

		internal IEnumerable<GameObject> _003CSpawnKeyboard_003Eb__5_2(Transform t)
		{
			return t.Children();
		}

		internal GameObject _003CSpawnKeyboard_003Eb__5_3(GameObject t)
		{
			return t.gameObject;
		}

		internal void _003CShowDebug_003Eb__17_0()
		{
			Main.Toggle("Info Screen");
		}

		internal void _003CPlayersTab_003Eb__21_0()
		{
			Buttons.CurrentCategoryName = "Main";
		}

		internal void _003CNavigatePlayer_003Eb__22_0()
		{
			PlayersTab();
		}

		internal void _003CNavigatePlayer_003Eb__22_4()
		{
			tracerTarget = null;
		}

		internal void _003CNavigatePlayer_003Eb__22_6()
		{
			Main.GiveGunTarget = null;
		}

		internal void _003CNavigatePlayer_003Eb__22_29(string creationDate)
		{
			Buttons.GetIndex("Player Creation Date").overlapText = "Creation Date: " + creationDate;
			Main.ReloadMenu();
		}

		internal void _003CCategorySettings_003Eb__26_0()
		{
			Buttons.CurrentCategoryName = "Settings";
			Buttons.buttons[Buttons.GetCategory("Temporary Category")] = Array.Empty<ButtonInfo>();
		}

		internal void _003CCustomMenuThemePage_003Eb__64_0()
		{
			ExitCustomMenuTheme();
		}

		internal void _003CCustomMenuThemePage_003Eb__64_1()
		{
			CMTBackground();
		}

		internal void _003CCustomMenuThemePage_003Eb__64_2()
		{
			CMTButton();
		}

		internal void _003CCustomMenuThemePage_003Eb__64_3()
		{
			CMTText();
		}

		internal void _003CCMTBackground_003Eb__65_0()
		{
			CustomMenuThemePage();
		}

		internal void _003CCMTBackground_003Eb__65_1()
		{
			CMTBackgroundFirst();
		}

		internal void _003CCMTBackground_003Eb__65_2()
		{
			CMTBackgroundSecond();
		}

		internal void _003CCMTBackgroundFirst_003Eb__66_0()
		{
			CMTBackground();
		}

		internal void _003CCMTBackgroundFirst_003Eb__66_1()
		{
			CMTRed();
		}

		internal void _003CCMTBackgroundFirst_003Eb__66_2()
		{
			CMTRed();
		}

		internal void _003CCMTBackgroundFirst_003Eb__66_3()
		{
			CMTRed(increase: false);
		}

		internal void _003CCMTBackgroundFirst_003Eb__66_4()
		{
			CMTGreen();
		}

		internal void _003CCMTBackgroundFirst_003Eb__66_5()
		{
			CMTGreen();
		}

		internal void _003CCMTBackgroundFirst_003Eb__66_6()
		{
			CMTGreen(increase: false);
		}

		internal void _003CCMTBackgroundFirst_003Eb__66_7()
		{
			CMTBlue();
		}

		internal void _003CCMTBackgroundFirst_003Eb__66_8()
		{
			CMTBlue();
		}

		internal void _003CCMTBackgroundFirst_003Eb__66_9()
		{
			CMTBlue(increase: false);
		}

		internal void _003CCMTBackgroundSecond_003Eb__67_0()
		{
			CMTBackground();
		}

		internal void _003CCMTBackgroundSecond_003Eb__67_1()
		{
			CMTRed();
		}

		internal void _003CCMTBackgroundSecond_003Eb__67_2()
		{
			CMTRed();
		}

		internal void _003CCMTBackgroundSecond_003Eb__67_3()
		{
			CMTRed(increase: false);
		}

		internal void _003CCMTBackgroundSecond_003Eb__67_4()
		{
			CMTGreen();
		}

		internal void _003CCMTBackgroundSecond_003Eb__67_5()
		{
			CMTGreen();
		}

		internal void _003CCMTBackgroundSecond_003Eb__67_6()
		{
			CMTGreen(increase: false);
		}

		internal void _003CCMTBackgroundSecond_003Eb__67_7()
		{
			CMTBlue();
		}

		internal void _003CCMTBackgroundSecond_003Eb__67_8()
		{
			CMTBlue();
		}

		internal void _003CCMTBackgroundSecond_003Eb__67_9()
		{
			CMTBlue(increase: false);
		}

		internal void _003CCMTButtonEnabled_003Eb__69_0()
		{
			CMTButtonEnabledSecond();
		}

		internal void _003CCMTButtonDisabled_003Eb__70_0()
		{
			CMTButton();
		}

		internal void _003CCMTButtonDisabled_003Eb__70_1()
		{
			CMTButtonDisabledFirst();
		}

		internal void _003CCMTButtonDisabled_003Eb__70_2()
		{
			CMTButtonDisabledSecond();
		}

		internal void _003CCMTButtonEnabledFirst_003Eb__71_0()
		{
			CMTButtonEnabled();
		}

		internal void _003CCMTButtonEnabledFirst_003Eb__71_1()
		{
			CMTRed();
		}

		internal void _003CCMTButtonEnabledFirst_003Eb__71_2()
		{
			CMTRed();
		}

		internal void _003CCMTButtonEnabledFirst_003Eb__71_3()
		{
			CMTRed(increase: false);
		}

		internal void _003CCMTButtonEnabledFirst_003Eb__71_4()
		{
			CMTGreen();
		}

		internal void _003CCMTButtonEnabledFirst_003Eb__71_5()
		{
			CMTGreen();
		}

		internal void _003CCMTButtonEnabledFirst_003Eb__71_6()
		{
			CMTGreen(increase: false);
		}

		internal void _003CCMTButtonEnabledFirst_003Eb__71_7()
		{
			CMTBlue();
		}

		internal void _003CCMTButtonEnabledFirst_003Eb__71_8()
		{
			CMTBlue();
		}

		internal void _003CCMTButtonEnabledFirst_003Eb__71_9()
		{
			CMTBlue(increase: false);
		}

		internal void _003CCMTButtonEnabledSecond_003Eb__72_0()
		{
			CMTButtonEnabled();
		}

		internal void _003CCMTButtonEnabledSecond_003Eb__72_1()
		{
			CMTRed();
		}

		internal void _003CCMTButtonEnabledSecond_003Eb__72_2()
		{
			CMTRed();
		}

		internal void _003CCMTButtonEnabledSecond_003Eb__72_3()
		{
			CMTRed(increase: false);
		}

		internal void _003CCMTButtonEnabledSecond_003Eb__72_4()
		{
			CMTGreen();
		}

		internal void _003CCMTButtonEnabledSecond_003Eb__72_5()
		{
			CMTGreen();
		}

		internal void _003CCMTButtonEnabledSecond_003Eb__72_6()
		{
			CMTGreen(increase: false);
		}

		internal void _003CCMTButtonEnabledSecond_003Eb__72_7()
		{
			CMTBlue();
		}

		internal void _003CCMTButtonEnabledSecond_003Eb__72_8()
		{
			CMTBlue();
		}

		internal void _003CCMTButtonEnabledSecond_003Eb__72_9()
		{
			CMTBlue(increase: false);
		}

		internal void _003CCMTButtonDisabledFirst_003Eb__73_0()
		{
			CMTButtonDisabled();
		}

		internal void _003CCMTButtonDisabledFirst_003Eb__73_1()
		{
			CMTRed();
		}

		internal void _003CCMTButtonDisabledFirst_003Eb__73_2()
		{
			CMTRed();
		}

		internal void _003CCMTButtonDisabledFirst_003Eb__73_3()
		{
			CMTRed(increase: false);
		}

		internal void _003CCMTButtonDisabledFirst_003Eb__73_4()
		{
			CMTGreen();
		}

		internal void _003CCMTButtonDisabledFirst_003Eb__73_5()
		{
			CMTGreen();
		}

		internal void _003CCMTButtonDisabledFirst_003Eb__73_6()
		{
			CMTGreen(increase: false);
		}

		internal void _003CCMTButtonDisabledFirst_003Eb__73_7()
		{
			CMTBlue();
		}

		internal void _003CCMTButtonDisabledFirst_003Eb__73_8()
		{
			CMTBlue();
		}

		internal void _003CCMTButtonDisabledFirst_003Eb__73_9()
		{
			CMTBlue(increase: false);
		}

		internal void _003CCMTButtonDisabledSecond_003Eb__74_0()
		{
			CMTRed();
		}

		internal void _003CCMTButtonDisabledSecond_003Eb__74_1()
		{
			CMTRed();
		}

		internal void _003CCMTButtonDisabledSecond_003Eb__74_2()
		{
			CMTRed(increase: false);
		}

		internal void _003CCMTButtonDisabledSecond_003Eb__74_3()
		{
			CMTGreen();
		}

		internal void _003CCMTButtonDisabledSecond_003Eb__74_4()
		{
			CMTGreen();
		}

		internal void _003CCMTButtonDisabledSecond_003Eb__74_5()
		{
			CMTGreen(increase: false);
		}

		internal void _003CCMTButtonDisabledSecond_003Eb__74_6()
		{
			CMTBlue();
		}

		internal void _003CCMTButtonDisabledSecond_003Eb__74_7()
		{
			CMTBlue();
		}

		internal void _003CCMTButtonDisabledSecond_003Eb__74_8()
		{
			CMTBlue(increase: false);
		}

		internal void _003CCMTTextTitle_003Eb__76_0()
		{
			CMTRed();
		}

		internal void _003CCMTTextTitle_003Eb__76_1()
		{
			CMTRed();
		}

		internal void _003CCMTTextTitle_003Eb__76_2()
		{
			CMTRed(increase: false);
		}

		internal void _003CCMTTextTitle_003Eb__76_3()
		{
			CMTGreen();
		}

		internal void _003CCMTTextTitle_003Eb__76_4()
		{
			CMTGreen();
		}

		internal void _003CCMTTextTitle_003Eb__76_5()
		{
			CMTGreen(increase: false);
		}

		internal void _003CCMTTextTitle_003Eb__76_6()
		{
			CMTBlue();
		}

		internal void _003CCMTTextTitle_003Eb__76_7()
		{
			CMTBlue();
		}

		internal void _003CCMTTextTitle_003Eb__76_8()
		{
			CMTBlue(increase: false);
		}

		internal void _003CCMTTextEnabled_003Eb__77_0()
		{
			CMTText();
		}

		internal void _003CCMTTextEnabled_003Eb__77_1()
		{
			CMTRed();
		}

		internal void _003CCMTTextEnabled_003Eb__77_2()
		{
			CMTRed();
		}

		internal void _003CCMTTextEnabled_003Eb__77_3()
		{
			CMTRed(increase: false);
		}

		internal void _003CCMTTextEnabled_003Eb__77_4()
		{
			CMTGreen();
		}

		internal void _003CCMTTextEnabled_003Eb__77_5()
		{
			CMTGreen();
		}

		internal void _003CCMTTextEnabled_003Eb__77_6()
		{
			CMTGreen(increase: false);
		}

		internal void _003CCMTTextEnabled_003Eb__77_7()
		{
			CMTBlue();
		}

		internal void _003CCMTTextEnabled_003Eb__77_8()
		{
			CMTBlue();
		}

		internal void _003CCMTTextEnabled_003Eb__77_9()
		{
			CMTBlue(increase: false);
		}

		internal void _003CCMTTextDisabled_003Eb__78_0()
		{
			CMTText();
		}

		internal void _003CCMTTextDisabled_003Eb__78_1()
		{
			CMTRed();
		}

		internal void _003CCMTTextDisabled_003Eb__78_2()
		{
			CMTRed();
		}

		internal void _003CCMTTextDisabled_003Eb__78_3()
		{
			CMTRed(increase: false);
		}

		internal void _003CCMTTextDisabled_003Eb__78_4()
		{
			CMTGreen();
		}

		internal void _003CCMTTextDisabled_003Eb__78_5()
		{
			CMTGreen();
		}

		internal void _003CCMTTextDisabled_003Eb__78_6()
		{
			CMTGreen(increase: false);
		}

		internal void _003CCMTTextDisabled_003Eb__78_7()
		{
			CMTBlue();
		}

		internal void _003CCMTTextDisabled_003Eb__78_8()
		{
			CMTBlue();
		}

		internal void _003CCMTTextDisabled_003Eb__78_9()
		{
			CMTBlue(increase: false);
		}

		internal void _003CKickToSpecificRoom_003Eb__109_0()
		{
			Overpowered.specificRoom = Main.keyboardInput.ToUpper();
		}

		internal void _003CKickToSpecificRoom_003Eb__109_1()
		{
			Main.Toggle("Kick to Specific Room");
		}

		internal void _003CCustomMenuName_003Eb__124_0()
		{
			Main.PromptSingleText("What would you like to set the menu name to?", delegate
			{
				File.WriteAllText("SeralythMenu/Seralyth_CustomMenuName.txt", Main.keyboardInput);
				_003CCustomMenuName_003Eg__Apply_007C124_1();
				Main.PromptSingle("You can always change this again by re-enabling the mod or changing it in the SeralythMenu folder! (located in the Gorilla Tag installation folder)");
			});
		}

		internal void _003CCustomMenuName_003Eb__124_2()
		{
			File.WriteAllText("SeralythMenu/Seralyth_CustomMenuName.txt", Main.keyboardInput);
			_003CCustomMenuName_003Eg__Apply_007C124_1();
			Main.PromptSingle("You can always change this again by re-enabling the mod or changing it in the SeralythMenu folder! (located in the Gorilla Tag installation folder)");
		}

		internal void _003CModRecognition_003Eb__133_0(AudioClip clip)
		{
			DictationPlay(clip, (float)Main.buttonClickVolume / 10f);
		}

		internal void _003CExecuteVoiceCommand_003Eb__134_1(AudioClip clip)
		{
			DictationPlay(clip, (float)Main.buttonClickVolume / 10f);
		}

		internal void _003CExecuteVoiceCommand_003Eb__134_0(AudioClip clip)
		{
			DictationPlay(clip, (float)Main.buttonClickVolume / 10f);
		}

		internal void _003CCancelModRecognition_003Eb__136_0(AudioClip clip)
		{
			DictationPlay(clip, (float)Main.buttonClickVolume / 10f);
		}

		internal void _003CDictationOn_003Eb__143_5(PhraseRecognizedEventArgs args)
		{
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(DictationRecognizer());
		}

		internal void _003CDictationRecognizer_003Eb__144_4(AudioClip clip)
		{
			DictationPlay(clip, (float)Main.buttonClickVolume / 10f);
		}

		internal void _003CDictationRecognizer_003Eb__144_5(AudioClip clip)
		{
			DictationPlay(clip, (float)Main.buttonClickVolume / 10f);
		}

		internal void _003CDictationRecognizer_003Eb__144_0(string text, ConfidenceLevel confidence)
		{
			if (debugDictation)
			{
				LogManager.Log("Dictation result: " + text);
			}
			if (cancelKeywords.Contains(text.ToLower()))
			{
				if (Main.dynamicSounds)
				{
					AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Menu/close.ogg", "Audio/Menu/close.ogg", delegate(AudioClip clip)
					{
						DictationPlay(clip, (float)Main.buttonClickVolume / 10f);
					});
				}
				NotificationManager.SendNotification("<color=grey>[</color><color=red>AI</color><color=grey>]</color> " + ((text.ToLower() == "i hate you") ? "I hate you too." : "Cancelling..."), 3000);
				((MonoBehaviour)CoroutineManager.instance).StartCoroutine(DictationRestart());
			}
			else
			{
				string narratorName = Main.narratorName;
				string text2 = narratorName;
				if (text2 == "Mommy ASMR")
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=#ffb6c1>MOMMY</color><color=grey>]</color> Let me get that for you..");
				}
				else
				{
					NotificationManager.SendNotification("<color=grey>[</color><color=blue>AI</color><color=grey>]</color> Generating response..");
				}
				((MonoBehaviour)CoroutineManager.instance).StartCoroutine(AIManager.AskAI(text));
			}
		}

		internal void _003CDictationRecognizer_003Eb__144_6(AudioClip clip)
		{
			DictationPlay(clip, (float)Main.buttonClickVolume / 10f);
		}

		internal unsafe void _003CDictationRecognizer_003Eb__144_1(DictationCompletionCause completionCause)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			if (debugDictation)
			{
				LogManager.Log($"completion cause: {completionCause}");
			}
			if (!(((object)(*(DictationCompletionCause*)(&completionCause))/*cast due to .constrained prefix*/).ToString() == "TimeoutExceeded"))
			{
				return;
			}
			if (Main.dynamicSounds)
			{
				AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Menu/close.ogg", "Audio/Menu/close.ogg", delegate(AudioClip clip)
				{
					DictationPlay(clip, (float)Main.buttonClickVolume / 10f);
				});
			}
			NotificationManager.SendNotification("<color=grey>[</color><color=red>AI</color><color=grey>]</color> Cancelling...", 3000);
		}

		internal void _003CDictationRecognizer_003Eb__144_7(AudioClip clip)
		{
			DictationPlay(clip, (float)Main.buttonClickVolume / 10f);
		}

		internal void _003CDictationRecognizer_003Eb__144_3(string text)
		{
			if (!AIManager.generating)
			{
				if (debugDictation)
				{
					LogManager.Log("Hypothesis: " + text);
				}
				NotificationManager.ClearAllNotifications();
				NotificationManager.SendNotification("<color=grey>[</color><color=green>VOICE</color><color=grey>]</color> " + text);
			}
		}

		internal void _003CInitializeClickGUIImpl_003Eb__167_2()
		{
			Buttons.CurrentCategoryIndex = 0;
			Main.ReloadMenu();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}

		internal void _003CInitializeClickGUIImpl_003Eb__167_3()
		{
			Main.Toggle("Accept Prompt");
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			Main.ReloadMenu();
		}

		internal void _003CInitializeClickGUIImpl_003Eb__167_4()
		{
			Main.Toggle("Decline Prompt");
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			Main.ReloadMenu();
		}

		internal string _003CInitializeClickGUIImpl_003Eb__167_5(ButtonInfo v)
		{
			return v.overlapText ?? v.buttonText;
		}

		internal ButtonInfo _003CInitializeClickGUIImpl_003Eb__167_8(string f)
		{
			return Buttons.GetIndex(f);
		}

		internal bool _003CInitializeClickGUIImpl_003Eb__167_9(ButtonInfo b)
		{
			return b != null;
		}

		internal IEnumerable<ButtonInfo> _003CInitializeClickGUIImpl_003Eb__167_10(ButtonInfo[] x)
		{
			return x;
		}

		internal bool _003CInitializeClickGUIImpl_003Eb__167_11(ButtonInfo b)
		{
			return b != null && b.enabled && b.isTogglable;
		}

		internal bool _003CInitializeClickGUIImpl_003Eb__167_12(ButtonInfo b)
		{
			return b.buttonText == "Exit Friends";
		}

		internal void _003CInitializeClickGUIImpl_003Eb__167_7(string _)
		{
			if (!Main.isSearching)
			{
				Search();
			}
		}
	}

	public static HashSet<VRRig> Blocked = new HashSet<VRRig>();

	public static GameObject TutorialObject;

	public static LineRenderer TutorialSelector;

	private static bool lastTrigger;

	public static bool hideId;

	private static VRRig tracerTarget;

	public static GameObject watchobject;

	public static GameObject watchText;

	public static GameObject watchEnabledIndicator;

	public static GameObject watchShell;

	public static int langInd;

	private static int menuScaleIndex = 10;

	private static int notificationScaleIndex = 6;

	private static int arraylistScaleIndex = 4;

	private static int overlayScaleIndex = 6;

	private static int modifyWhatId;

	private static int previousPage;

	private static TMP_FontAsset chosenFont;

	public static float fontTime;

	public static int fontStyleType = 2;

	public static int inputTextColorInt = 3;

	private static int gunLineQualityIndex = 2;

	public static bool currentmentalstate;

	private static bool lastFocused;

	private static KeywordRecognizer mainPhrases;

	private static KeywordRecognizer modPhrases;

	private static string[] keyWords = new string[26]
	{
		"jarvis", "seralyth", "seralith", "sarolith", "siri", "google", "alexa", "dummy", "computer", "stinky",
		"silly", "stupid", "console", "go go gadget", "monika", "wikipedia", "gideon", "a i", "ai", "a.i",
		"chat gpt", "chatgpt", "grok", "grock", "groq", "garmin"
	};

	private static readonly string[] cancelKeywords = new string[6] { "nevermind", "cancel", "never mind", "stop", "i hate you", "die" };

	private static Coroutine timeoutCoroutine;

	public static DictationRecognizer drec;

	public static KeywordRecognizer krec;

	public static bool debugDictation;

	public static bool restartOnFocus;

	public static float dRestartTime;

	private static LineRenderer clickGuiLine;

	private static bool lastTriggerClick;

	private static bool lastRightPrimary;

	private static EventSystem eventSystem;

	private static PointerEventData pointerData;

	private static readonly List<RaycastResult> uiResults = new List<RaycastResult>();

	private static GameObject currentUI;

	private static GameObject pressedUI;

	private static GameObject draggedUI;

	private static Vector2 lastPointerPos;

	private static Canvas canvas;

	private static bool isDragging;

	private static bool searchBuiltAll;

	private static string lastSearchText = "";

	public static GameObject selectObject;

	public static VRRig lastTarget;

	public static bool lastTriggerSelect;

	public static int loadingPreferencesFrame;

	public static bool isLoadingPreferences;

	public static readonly Dictionary<ControllerBinding, Key> pcBindings = new Dictionary<ControllerBinding, Key>
	{
		{
			ControllerBinding.RightPrimaryButton,
			(Key)19
		},
		{
			ControllerBinding.RightSecondaryButton,
			(Key)32
		},
		{
			ControllerBinding.LeftPrimaryButton,
			(Key)20
		},
		{
			ControllerBinding.LeftSecondaryButton,
			(Key)21
		},
		{
			ControllerBinding.LeftGrip,
			(Key)11
		},
		{
			ControllerBinding.RightGrip,
			(Key)12
		},
		{
			ControllerBinding.LeftTrigger,
			(Key)13
		},
		{
			ControllerBinding.RightTrigger,
			(Key)14
		},
		{
			ControllerBinding.JoystickClick,
			(Key)2
		},
		{
			ControllerBinding.LeftOverride,
			(Key)53
		}
	};

	public static readonly string[] ThemeNames = new string[65]
	{
		"Seralyth", "Blue Magenta", "Dark Mode", "Strobe", "Kman", "Rainbow", "Player Material", "Lava", "Rock", "Ice",
		"Water", "Minty", "Pink", "Purple", "Magenta Cyan", "Red Fade", "Orange Fade", "Yellow Fade", "Green Fade", "Blue Fade",
		"Purple Fade", "Magenta Fade", "Banana", "Pride", "Trans", "MLM or Gay", "Steal (old)", "Silence", "Transparent", "King",
		"Scoreboard", "Scoreboard (banned)", "Rift", "Blurple Dark", "ShibaGT Gold", "ShibaGT Genesis", "wyvern", "Steal (new)", "USA Menu (lol)", "Watch",
		"AZ Menu", "ImGUI", "Clean Dark", "Discord Light Mode (lmfao)", "The Hub", "EPILEPTIC", "Discord Blurple", "VS Zero", "Weed theme", "Pastel Rainbow",
		"Rift Light", "Rose (Solace)", "Tenacity (Solace)", "e621 (by iiDk)", "Catppuccin Mocha", "Rexon", "Tenacity (Minecraft)", "Mint Blue (Opal v2)", "Pink Blood (Opal v2)", "Purple Fire (Opal v2)",
		"Deep Ocean (Opal v2)", "Bad Apple (thanks random person in vc for idea)", "coolkidd", "Old ShibaGT RGB", "Old-ish ShibaGT RGB"
	};

	public static readonly string[] LanguageNames = new string[12]
	{
		"English", "Español", "Français", "Deutsch", "日本語", "Italiano", "Português", "Nederlands", "Русский", "Polski",
		"svenska", "dansk"
	};

	private static readonly string[] LanguageCodenames = new string[12]
	{
		"en", "es", "fr", "de", "ja", "it", "pt", "nl", "ru", "pl",
		"sw", "da"
	};

	public static readonly string[] MenuButtonNames = new string[5] { "Primary", "Secondary", "Grip", "Trigger", "Joystick" };

	public static readonly string[] InputColorNames = new string[12]
	{
		"Red", "Orange", "Yellow", "Green", "Blue", "Cyan", "Purple", "Pink", "White", "Grey",
		"Black", "Rose"
	};

	private static readonly string[] InputColorValues = new string[12]
	{
		"red", "#ff8000", "yellow", "green", "blue", "#00FFFF", "purple", "#FF00FF", "white", "grey",
		"black", "#ff005d"
	};

	public static readonly string[] NarratorNames = new string[30]
	{
		"Default", "Kimberly", "Brian", "Matthew", "Joey", "Justin", "Cristiano", "Giorgio", "Ewa", "TikTok",
		"Grandma", "Trickster", "Elf", "Ghostface", "Zombie", "Narrator", "Pirate", "Song", "TikTok Joey", "Gingerbread Man",
		"Chris", "Thanksgiving", "Santa", "Google US", "Google UK", "Dog", "Jerkface", "Robot", "Vlad", "Obama"
	};

	public static readonly string[] GunQualityNames = new string[5] { "Potato", "Low", "Normal", "High", "Extreme" };

	private static readonly int[] GunQualityValues = new int[5] { 10, 25, 50, 100, 250 };

	public static readonly string[] GunVariationNames = new string[10] { "Default", "Lightning", "Wavy", "Blocky", "Zigzag", "Spring", "Bouncy", "Audio", "Bezier", "Rope" };

	public static readonly string[] GunDirectionNames = new string[5] { "Default", "Legacy", "Laser", "Finger", "Face" };

	public static readonly string[] GunLibShapeNames = new string[5] { "Disabled", "Circle", "Square", "Triangle", "Star" };

	public static readonly Vector3[] PointerPositions = (Vector3[])(object)new Vector3[4]
	{
		new Vector3(0f, -0.1f, 0f),
		new Vector3(0f, -0.1f, -0.15f),
		new Vector3(0f, 0.1f, -0.05f),
		new Vector3(0f, 0.0666f, 0.1f)
	};

	public static readonly string[] FontTypeNames = new string[15]
	{
		"Agency FB", "FreeSans", "DejaVu Sans", "Utopium", "Comic Sans", "Cascadia Mono", "Candara", "MS Gothic", "Anton", "SimSun",
		"Minecraft", "Terminal", "OpenDyslexic", "Taiko", "Liberation Sans"
	};

	public static void BlockPlayer(VRRig rig)
	{
		Blocked.Add(rig);
		rig.DeactivateAllRenderers();
		rig.voiceAudio.volume = 0f;
	}

	public static void UnblockPlayer(VRRig rig)
	{
		Blocked.Remove(rig);
		rig.ReactivateAllRenderers();
		rig.voiceAudio.volume = 1f;
	}

	public static void HandleBlockedPlayers()
	{
		foreach (VRRig item in Blocked)
		{
			item.BreakHandLinks();
		}
		SerializePatch.OverrideSerialization = delegate
		{
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Expected O, but got Unknown
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Expected O, but got Unknown
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			if (Blocked.Count == 0)
			{
				return true;
			}
			int[] targetActors = Blocked.Select((VRRig rig) => rig.Creator.ActorNumber).ToArray();
			int[] targetActors2 = (from rig in VRRigExtensions.ActiveRigs
				where !Blocked.Contains(rig)
				select rig.Creator.ActorNumber).ToArray();
			Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
			Vector3 position = ((Component)VRRig.LocalRig).transform.position;
			Main.SendSerialize(VRRig.LocalRig.GetPhotonView(), new RaiseEventOptions
			{
				TargetActors = targetActors2
			});
			((Component)VRRig.LocalRig).transform.position = new Vector3(Random.Range(-99999f, 99999f), 99999f, Random.Range(-99999f, 99999f));
			Main.SendSerialize(VRRig.LocalRig.GetPhotonView(), new RaiseEventOptions
			{
				TargetActors = targetActors
			});
			Main.RPCProtection();
			((Component)VRRig.LocalRig).transform.position = position;
			return false;
		};
	}

	public static void Search()
	{
		Main.isSearching = !Main.isSearching;
		Main.pageNumber = 0;
		Main.keyboardInput = "";
		lastSearchText = "";
		if (Main.isSearching)
		{
			if (Main.clickGUI)
			{
				searchBuiltAll = true;
				InitializeClickGUI();
			}
			SpawnKeyboard();
		}
		else
		{
			DestroyKeyboard();
			if (Main.clickGUI)
			{
				searchBuiltAll = false;
				InitializeClickGUI();
			}
		}
	}

	public static void SpawnKeyboard()
	{
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		Main.isKeyboardPc = Main.isOnPC || (Main.toggleButtonActive && Main.keyboardWithToggleButton);
		Main.inTextInput = true;
		Main.keyboardInput = "";
		Main.shift = false;
		Main.lockShift = false;
		if (Main.isKeyboardPc)
		{
			Main.lastPressedKeys.Add((Key)31);
		}
		if (!Main.isKeyboardPc && (Object)(object)Main.VRKeyboard == (Object)null)
		{
			Main.VRKeyboard = AssetUtilities.LoadObject<GameObject>("VRKeyboard");
			Main.VRKeyboard.transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
			Main.VRKeyboard.transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation;
			Main.menuSpawnPosition = ((Component)Main.VRKeyboard.transform.Find("MenuSpawnPosition")).gameObject;
			ComponentUtils.AddComponent<ColorChanger>((Component)(object)Main.VRKeyboard.transform.Find("Canvas")).colors = Main.textColors[1];
			Transform transform = Main.VRKeyboard.transform;
			transform.localScale *= (Main.scaleWithPlayer ? (GTPlayer.Instance.scale * Main.menuScale) : Main.menuScale);
			Transform transform2 = Main.menuSpawnPosition.transform;
			transform2.localScale *= (Main.scaleWithPlayer ? (GTPlayer.Instance.scale * Main.menuScale) : Main.menuScale);
			ColorChanger colorChanger = ((Component)Main.VRKeyboard.transform.Find("Background")).gameObject.AddComponent<ColorChanger>();
			colorChanger.colors = Main.menuBackgroundColor;
			foreach (GameObject item in (from t in Main.VRKeyboard.transform.Find("Seperate").Children()
				select t.gameObject).Concat((IEnumerable<GameObject>)(object)new GameObject[1] { ((Component)Main.VRKeyboard.transform.Find("Keys/default")).gameObject }))
			{
				ColorChanger colorChanger2 = item.AddComponent<ColorChanger>();
				colorChanger2.colors = Main.buttonColors[0];
			}
			if (Main.shouldOutline)
			{
				Main.OutlineObject(((Component)Main.VRKeyboard.transform.Find("Background")).gameObject, shouldBeEnabled: true);
			}
			IEnumerable<GameObject> enumerable = from t in (from name in new string[4] { "Numbers", "Letters", "Special", "Seperate" }
					select Main.VRKeyboard.transform.Find(name) into t
					where (Object)(object)t != (Object)null
					select t).SelectMany((Transform t) => t.Children())
				select t.gameObject;
			foreach (GameObject item2 in enumerable)
			{
				item2.AddComponent<KeyboardKey>().key = ((Object)item2).name;
				item2.layer = 2;
				if (Main.shouldOutline)
				{
					Main.OutlineObject(item2, shouldBeEnabled: true);
				}
			}
		}
		if ((Object)(object)Main.lKeyReference == (Object)null)
		{
			Main.lKeyReference = GameObject.CreatePrimitive((PrimitiveType)0);
			Main.lKeyReference.transform.parent = GorillaTagger.Instance.leftHandTransform;
			Main.lKeyReference.GetComponent<Renderer>().material.color = Main.backgroundColor.GetColor(0);
			Main.lKeyReference.transform.localPosition = Main.pointerOffset;
			Main.lKeyReference.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
			Main.lKeyCollider = Main.lKeyReference.GetComponent<SphereCollider>();
			ColorChanger colorChanger3 = Main.lKeyReference.AddComponent<ColorChanger>();
			colorChanger3.colors = Main.backgroundColor;
		}
		if ((Object)(object)Main.rKeyReference == (Object)null)
		{
			Main.rKeyReference = GameObject.CreatePrimitive((PrimitiveType)0);
			Main.rKeyReference.transform.parent = GorillaTagger.Instance.rightHandTransform;
			Main.rKeyReference.GetComponent<Renderer>().material.color = Main.backgroundColor.GetColor(0);
			Main.rKeyReference.transform.localPosition = Main.pointerOffset;
			Main.rKeyReference.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
			Main.rKeyCollider = Main.rKeyReference.GetComponent<SphereCollider>();
			ColorChanger colorChanger4 = Main.rKeyReference.AddComponent<ColorChanger>();
			colorChanger4.colors = Main.backgroundColor;
		}
	}

	public static void DestroyKeyboard()
	{
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		Main.inTextInput = false;
		Main.isKeyboardPc = false;
		if ((Object)(object)Main.lKeyReference != (Object)null)
		{
			Object.Destroy((Object)(object)Main.lKeyReference);
			Main.lKeyReference = null;
		}
		if ((Object)(object)Main.rKeyReference != (Object)null)
		{
			Object.Destroy((Object)(object)Main.rKeyReference);
			Main.rKeyReference = null;
		}
		if ((Object)(object)Main.VRKeyboard != (Object)null)
		{
			Object.Destroy((Object)(object)Main.VRKeyboard);
			Main.VRKeyboard = null;
		}
		if ((Object)(object)Main.TPC != (Object)null && ((Object)((Component)((Component)Main.TPC).transform.parent).gameObject).name.Contains("CameraTablet") && Main.isOnPC)
		{
			Main.isOnPC = false;
			((Component)Main.TPC).transform.position = ((Component)Main.TPC).transform.parent.position;
			((Component)Main.TPC).transform.rotation = ((Component)Main.TPC).transform.parent.rotation;
		}
	}

	public static void GlobalReturn()
	{
		NotificationManager.ClearAllNotifications();
		Main.Toggle(Buttons.buttons[Buttons.CurrentCategoryIndex][Buttons.GetCategory("Main")].buttonText, fromMenu: true);
		SoundManager.Play("Return");
		if (Main.prompts.Count > 0)
		{
			StopCurrentPrompt();
		}
	}

	public static void StopCurrentPrompt()
	{
		Main.prompts.RemoveAt(0);
	}

	public static void MergePreferences_iisStupidMenu()
	{
		string text = "iisStupidMenu";
		string path = "iiMenu_Preferences.txt";
		if (!Directory.Exists(text))
		{
			return;
		}
		string text2 = Path.Combine(text, "Sounds");
		string newValue = Path.Combine("SeralythMenu", "Sounds");
		if (Directory.Exists(text2))
		{
			string[] directories = Directory.GetDirectories(text2, "*", SearchOption.AllDirectories);
			foreach (string text3 in directories)
			{
				string path2 = text3.Replace(text2, newValue);
				Directory.CreateDirectory(path2);
			}
			string[] files = Directory.GetFiles(text2, "*", SearchOption.AllDirectories);
			foreach (string text4 in files)
			{
				string text5 = text4.Replace(text2, newValue);
				Directory.CreateDirectory(Path.GetDirectoryName(text5));
				File.Copy(text4, text5);
			}
		}
		text2 = Path.Combine(text, path);
		newValue = Path.Combine("SeralythMenu", "Seralyth_Preferences.txt");
		if (File.Exists(text2))
		{
			string[] array = File.ReadAllLines(text2);
			if (array.Length >= 5)
			{
				string[] array2 = array[2].Split(new string[1] { ";;" }, StringSplitOptions.None);
				int num = 13;
				if (num < array2.Length && int.TryParse(array2[num], out var result))
				{
					array2[num] = Math.Clamp(result + 1, 0, 6).ToString();
				}
				array[2] = string.Join(";;", array2);
				if (int.TryParse(array[3], out var result2))
				{
					array[3] = Math.Clamp(result2 - 1, 0, 6).ToString();
				}
				if (int.TryParse(array[4], out var result3))
				{
					array[4] = Math.Clamp(result3 - 1, 0, 65).ToString();
				}
			}
			File.WriteAllLines(newValue, array);
		}
		LoadPreferences();
		Sound.LoadSoundboard(openCategory: false);
		NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully completed merge. Have fun using MrChicken Menu!");
	}

	public static void UpdateSoundPreferences()
	{
		string text = File.ReadAllText("SeralythMenu/Seralyth_Preferences.txt").Replace("\r", "");
		string[] array = text.Split('\n');
		string[] array2 = array[2].Split(";;");
		if (int.TryParse(array2[16], out var result) && int.TryParse(array2[25], out result))
		{
			SoundManager.DefaultSounds["Button"] = helper(array2[16], SoundManager.Sounds["Buttons"].Keys.ToArray(), "Default");
			SoundManager.DefaultSounds["Notification"] = helper(array2[25], SoundManager.Sounds["Notifications"].Keys.ToArray(), "None");
			array2[16] = SoundManager.DefaultSounds["Button"];
			array2[25] = SoundManager.DefaultSounds["Notification"];
			array[2] = string.Join(";;", array2);
			File.WriteAllText("SeralythMenu/Seralyth_Preferences.txt", string.Join("\n", array));
		}
		static string helper(string value, string[] keys, string defaultKey)
		{
			if (keys.Contains(value))
			{
				return value;
			}
			int num = int.Parse(value);
			num = Mathf.Clamp(num - 1, 0, keys.Length - 1);
			return keys[num];
		}
	}

	public static void ShowTutorial()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)TutorialObject != (Object)null)
		{
			Object.Destroy((Object)(object)TutorialObject);
		}
		TutorialObject = AssetUtilities.LoadObject<GameObject>("Tutorial");
		TutorialObject.transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + ((Component)GorillaTagger.Instance.bodyCollider).transform.forward * 1f + Vector3.up * 0.25f;
		TutorialObject.transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation * Quaternion.Euler(0f, 180f, 0f);
		string text = "q2";
		switch (ControllerUtilities.GetLeftControllerType())
		{
		case ControllerUtilities.ControllerType.Unknown:
		case ControllerUtilities.ControllerType.Quest2:
			text = "q2";
			break;
		case ControllerUtilities.ControllerType.Quest3:
			text = "q3";
			break;
		case ControllerUtilities.ControllerType.ValveIndex:
			text = "index";
			break;
		case ControllerUtilities.ControllerType.VIVE:
			text = "vive";
			break;
		}
		VideoPlayer component = ((Component)TutorialObject.transform.Find("Video")).GetComponent<VideoPlayer>();
		component.url = "https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Videos/Tutorial/tutorial-" + text + ".mp4";
		component.isLooping = true;
		ComponentUtils.AddComponent<TutorialButton>((Component)(object)component).buttonType = TutorialButton.ButtonType.Pause;
		ComponentUtils.AddComponent<TutorialButton>((Component)(object)TutorialObject.transform.Find("Close")).buttonType = TutorialButton.ButtonType.Close;
	}

	public static void UpdateTutorial()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		if (Vector3.Distance(TutorialObject.transform.position, ((Component)GorillaTagger.Instance.bodyCollider).transform.position) > 2f)
		{
			TutorialObject.transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + ((Component)GorillaTagger.Instance.bodyCollider).transform.forward * 1f + Vector3.up * 0.25f;
			TutorialObject.transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation * Quaternion.Euler(0f, 180f, 0f);
		}
		if ((Object)(object)TutorialSelector == (Object)null)
		{
			TutorialSelector = new GameObject("Seralyth_TutorialSelector").AddComponent<LineRenderer>();
			((Renderer)TutorialSelector).material.shader = Shader.Find("Sprites/Default");
			TutorialSelector.startWidth = 0.01f;
			TutorialSelector.endWidth = 0.01f;
			TutorialSelector.positionCount = 2;
			TutorialSelector.useWorldSpace = true;
		}
		TutorialSelector.startColor = Main.BrightenColor(Color32.op_Implicit(new Color32(byte.MaxValue, (byte)128, (byte)0, (byte)128)));
		TutorialSelector.endColor = Main.BrightenColor(Color32.op_Implicit(new Color32(byte.MaxValue, (byte)102, (byte)0, (byte)128)));
		Vector3 item = ControllerUtilities.GetTrueRightHand().forward;
		RaycastHit val = default(RaycastHit);
		Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position + item / 4f, item, ref val, 512f, Main.NoInvisLayerMask());
		if (!XRSettings.isDeviceActive)
		{
			Ray val2 = Main.TPC.ScreenPointToRay(Vector2.op_Implicit(((InputControl<Vector2>)(object)((Pointer)Mouse.current).position).ReadValue()));
			Physics.Raycast(val2, ref val, 512f, Main.NoInvisLayerMask());
		}
		TutorialSelector.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
		TutorialSelector.SetPosition(1, (((RaycastHit)(ref val)).point == Vector3.zero) ? GorillaTagger.Instance.rightHandTransform.position : ((RaycastHit)(ref val)).point);
		if ((Main.rightTrigger > 0.5f || Mouse.current.leftButton.isPressed) && !lastTrigger)
		{
			TutorialButton componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<TutorialButton>();
			if (Object.op_Implicit((Object)(object)componentInParent))
			{
				componentInParent.ClickButton();
			}
		}
		lastTrigger = Main.rightTrigger > 0.5f || Mouse.current.leftButton.isPressed;
	}

	public static void ShowDebug()
	{
		int category = Buttons.GetCategory("Temporary Category");
		string text = "10.0.2";
		if (PluginInfo.BetaBuild)
		{
			text = "<color=blue>Beta</color> " + text;
		}
		Buttons.AddButton(category, new ButtonInfo
		{
			buttonText = "Exit Info Screen",
			method = delegate
			{
				Main.Toggle("Info Screen");
			},
			isTogglable = false,
			toolTip = "Returns you back to the main page."
		});
		Buttons.AddButton(category, new ButtonInfo
		{
			buttonText = "DebugMenuName",
			overlapText = "<color=grey><b>MrChicken Menu </b></color>" + text,
			label = true
		});
		Buttons.AddButton(category, new ButtonInfo
		{
			buttonText = "DebugColor",
			overlapText = "Loading...",
			label = true
		});
		Buttons.AddButton(category, new ButtonInfo
		{
			buttonText = "DebugName",
			overlapText = "Loading...",
			label = true
		});
		Buttons.AddButton(category, new ButtonInfo
		{
			buttonText = "DebugId",
			overlapText = "Loading...",
			label = true
		});
		Buttons.AddButton(category, new ButtonInfo
		{
			buttonText = "DebugClip",
			overlapText = "Loading...",
			label = true
		});
		Buttons.AddButton(category, new ButtonInfo
		{
			buttonText = "DebugFps",
			overlapText = "Loading...",
			label = true
		});
		Buttons.AddButton(category, new ButtonInfo
		{
			buttonText = "DebugRoomA",
			overlapText = "Loading...",
			label = true
		});
		Buttons.AddButton(category, new ButtonInfo
		{
			buttonText = "DebugRoomB",
			overlapText = "Loading...",
			label = true
		});
		Debug();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void Debug()
	{
		string text = "<color=red>" + MathF.Floor(PlayerPrefs.GetFloat("redValue") * 255f) + "</color>";
		string text2 = ", <color=green>" + MathF.Floor(PlayerPrefs.GetFloat("greenValue") * 255f) + "</color>";
		string text3 = ", <color=blue>" + MathF.Floor(PlayerPrefs.GetFloat("blueValue") * 255f) + "</color>";
		Buttons.GetIndex("DebugColor").overlapText = "Color: " + text + text2 + text3;
		string text4 = ((PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient) ? "<color=red> [Master]</color>" : "");
		Buttons.GetIndex("DebugName").overlapText = PhotonNetwork.LocalPlayer.NickName + text4;
		Buttons.GetIndex("DebugId").overlapText = "<color=green>ID: </color>" + (hideId ? "Hidden" : PhotonNetwork.LocalPlayer.UserId);
		Buttons.GetIndex("DebugClip").overlapText = "<color=green>Clip: </color>" + ((GUIUtility.systemCopyBuffer.Length > 25) ? GUIUtility.systemCopyBuffer.Substring(0, 25) : GUIUtility.systemCopyBuffer);
		Buttons.GetIndex("DebugFps").overlapText = "<b>" + Main.lastDeltaTime + "</b> FPS <b>" + PhotonNetwork.GetPing() + "</b> Ping";
		Buttons.GetIndex("DebugRoomA").overlapText = "<color=blue>" + NetworkSystem.Instance.regionNames[NetworkSystem.Instance.currentRegionIndex].ToUpper() + "</color> " + PhotonNetwork.PlayerList.Length + " Players";
		string text5 = ((!PhotonNetwork.InRoom) ? "" : (NetworkSystem.Instance.SessionIsPrivate ? "Private" : "Public"));
		Buttons.GetIndex("DebugRoomB").overlapText = "<color=blue>" + text5 + "</color> " + (PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.Name : "Not in room");
	}

	public static void HideDebug()
	{
		int category = Buttons.GetCategory("Temporary Category");
		Buttons.RemoveButton(category, "DebugMenuName");
		Buttons.RemoveButton(category, "DebugColor");
		Buttons.RemoveButton(category, "DebugName");
		Buttons.RemoveButton(category, "DebugId");
		Buttons.RemoveButton(category, "DebugClip");
		Buttons.RemoveButton(category, "DebugFps");
		Buttons.RemoveButton(category, "DebugRoomA");
		Buttons.RemoveButton(category, "DebugRoomB");
		Buttons.CurrentCategoryName = "Main";
	}

	public static void PlayersTab()
	{
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		List<ButtonInfo> list = new List<ButtonInfo>
		{
			new ButtonInfo
			{
				buttonText = "Exit Players",
				method = delegate
				{
					Buttons.CurrentCategoryName = "Main";
				},
				isTogglable = false,
				toolTip = "Returns you back to the main page.",
				legal = true
			}
		};
		if (!PhotonNetwork.InRoom)
		{
			list.Add(new ButtonInfo
			{
				buttonText = "Not in a Room",
				label = true,
				legal = true
			});
		}
		else
		{
			for (int num = 0; num < NetworkSystem.Instance.PlayerListOthers.Length; num++)
			{
				NetPlayer player = NetworkSystem.Instance.PlayerListOthers[num];
				string text = "#ffffff";
				try
				{
					text = "#" + Main.ColorToHex(RigUtilities.GetVRRigFromPlayer(player).playerColor);
				}
				catch
				{
				}
				list.Add(new ButtonInfo
				{
					buttonText = $"PlayerButton{num}",
					overlapText = "<color=" + text + ">" + player.NickName + "</color>",
					method = delegate
					{
						NavigatePlayer(player);
					},
					isTogglable = false,
					toolTip = "See information on the player " + player.NickName + ".",
					legal = true
				});
			}
		}
		Buttons.buttons[Buttons.GetCategory("Players")] = list.ToArray();
		Buttons.CurrentCategoryName = "Players";
	}

	public static void NavigatePlayer(NetPlayer player)
	{
		//IL_08cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09da: Unknown result type (might be due to invalid IL or missing references)
		string nickName = player.NickName;
		VRRig playerRig = RigUtilities.GetVRRigFromPlayer(player) ?? null;
		List<ButtonInfo> list = new List<ButtonInfo>();
		list.Add(new ButtonInfo
		{
			buttonText = "Exit PlayerInspect",
			overlapText = "Exit " + nickName,
			method = delegate
			{
				PlayersTab();
			},
			isTogglable = false,
			toolTip = "Returns you back to the players tab.",
			legal = true
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Spectate Player",
			overlapText = "Spectate " + nickName,
			method = delegate
			{
				SpectatePlayer(playerRig);
			},
			isTogglable = false,
			toolTip = "Shows you what " + nickName + " sees.",
			legal = true
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Teleport to Player",
			overlapText = "Teleport to " + nickName,
			method = delegate
			{
				Movement.TeleportToPlayer(player);
			},
			isTogglable = false,
			toolTip = "Teleports you to " + nickName + "."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Player Tracers",
			overlapText = "Tracers: " + nickName,
			enableMethod = delegate
			{
				tracerTarget = playerRig;
			},
			disableMethod = delegate
			{
				tracerTarget = null;
			},
			method = PlayerTracers,
			toolTip = "Draws a tracer line to " + nickName + ".",
			legal = true
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Give Player Guns",
			overlapText = "Give " + nickName + " Guns",
			method = delegate
			{
				Main.GiveGunTarget = playerRig;
			},
			disableMethod = delegate
			{
				Main.GiveGunTarget = null;
			},
			toolTip = "Gives " + nickName + " every gun on the menu."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Copy Movement",
			overlapText = "Copy Movement " + nickName,
			method = delegate
			{
				Movement.CopyMovementPlayer(player);
			},
			disableMethod = Movement.EnableRig,
			toolTip = "Copies the movement of " + nickName + "."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Follow Player",
			overlapText = "Follow " + nickName,
			method = delegate
			{
				Movement.FollowPlayer(player);
			},
			disableMethod = Movement.EnableRig,
			toolTip = "Follows " + nickName + "."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Tag Player",
			overlapText = "Tag " + nickName,
			method = delegate
			{
				Advantages.TagPlayer(player);
			},
			disableMethod = Movement.EnableRig,
			toolTip = "Tags " + nickName + "."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Snowball Fling Player",
			overlapText = "Snowball Fling " + nickName,
			method = delegate
			{
				Overpowered.FlingPlayer(player);
			},
			toolTip = "Flings " + nickName + " with snowballs."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Projectile Blind Player",
			overlapText = "Projectile Blind " + nickName,
			method = delegate
			{
				Projectiles.ProjectileBlindPlayer(player);
			},
			toolTip = "Blinds " + nickName + " using the egg projectiles."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Projectile Lag Player",
			overlapText = "Projectile Lag " + nickName,
			method = delegate
			{
				Projectiles.ProjectileLagPlayer(player);
			},
			toolTip = "Lags " + nickName + " using the firework projectiles."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Lag Player",
			overlapText = "Lag " + nickName,
			method = delegate
			{
				Overpowered.LagTarget(player);
			},
			toolTip = "Lags " + nickName + "."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Destroy Player",
			overlapText = "Destroy " + nickName,
			method = delegate
			{
				Overpowered.DestroyPlayer(player);
			},
			toolTip = "Stops all new players from seeing " + nickName + "."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Guardian Bring Player",
			overlapText = "Guardian Bring " + nickName,
			method = delegate
			{
				Overpowered.GuardianBringPlayer(player);
			},
			toolTip = "Brings " + nickName + " to you."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Guardian Bring Player Gun",
			overlapText = "Guardian Bring " + nickName + " Gun",
			method = delegate
			{
				Overpowered.GuardianBringPlayerGun(player);
			},
			toolTip = "Brings " + nickName + " to wherever your hand desires."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Guardian Kick Player",
			overlapText = "Guardian Kick " + nickName,
			method = delegate
			{
				Overpowered.GuardianKickTarget(player);
			},
			toolTip = "Kicks " + nickName + "."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Guardian Obliterate Player",
			overlapText = "Guardian Obliterate " + nickName,
			method = delegate
			{
				Overpowered.GuardianObliteratePlayer(player);
			},
			toolTip = "Obliterates " + nickName + "."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Guardian Crash Player",
			overlapText = "Guardian Crash " + nickName,
			method = delegate
			{
				Overpowered.GuardianCrashPlayer(player);
			},
			toolTip = "Crashes " + nickName + "."
		});
		List<ButtonInfo> list2 = list;
		if (PhotonNetwork.IsMasterClient)
		{
			list2.AddRange(new ButtonInfo[2]
			{
				new ButtonInfo
				{
					buttonText = "Vibrate Player",
					overlapText = "Vibrate " + nickName,
					method = delegate
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Expected O, but got Unknown
						RaiseEventOptions val = new RaiseEventOptions();
						val.TargetActors = new int[1] { player.ActorNumber };
						Overpowered.BetaSetStatus((StatusEffects)1, val);
					},
					toolTip = "Vibrates " + nickName + "'s controllers."
				},
				new ButtonInfo
				{
					buttonText = "Slow Player",
					overlapText = "Slow " + nickName,
					method = delegate
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Expected O, but got Unknown
						RaiseEventOptions val = new RaiseEventOptions();
						val.TargetActors = new int[1] { player.ActorNumber };
						Overpowered.BetaSetStatus((StatusEffects)0, val);
					},
					toolTip = "Gives " + nickName + " tag freeze."
				}
			});
		}
		if (ServerData.Administrators.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
		{
			list2.AddRange(new ButtonInfo[3]
			{
				new ButtonInfo
				{
					buttonText = "Admin Kick Player",
					overlapText = "Admin Kick " + nickName,
					method = delegate
					{
						Seralyth.Classes.Menu.Console.ExecuteCommand("kick", (ReceiverGroup)1, player.UserId);
					},
					isTogglable = false,
					toolTip = "Kicks " + nickName + " if they're using the menu.",
					legal = true
				},
				new ButtonInfo
				{
					buttonText = "Admin Bring Player",
					overlapText = "Admin Bring " + nickName,
					method = delegate
					{
						//IL_0027: Unknown result type (might be due to invalid IL or missing references)
						Seralyth.Classes.Menu.Console.ExecuteCommand("tp", player.ActorNumber, ((Component)GorillaTagger.Instance.headCollider).transform.position);
					},
					isTogglable = false,
					toolTip = "Brings " + nickName + " to you if they're using the menu.",
					legal = true
				},
				new ButtonInfo
				{
					buttonText = "Admin Crash Player",
					overlapText = "Admin Crash " + nickName,
					method = delegate
					{
						Seralyth.Classes.Menu.Console.ExecuteCommand("crash", player.ActorNumber);
					},
					isTogglable = false,
					toolTip = "Crashes " + nickName + " if they're using the menu.",
					legal = true
				}
			});
		}
		Color playerColor = playerRig?.playerColor ?? Color.black;
		if (Object.op_Implicit((Object)(object)playerRig))
		{
			ButtonInfo[] obj = new ButtonInfo[7]
			{
				new ButtonInfo
				{
					buttonText = "Check " + player.NickName + "'s Mods",
					method = delegate
					{
						Main.ModChecker(player);
					},
					isTogglable = false,
					toolTip = "View all of \"" + player.NickName + "\"'s mods."
				},
				new ButtonInfo
				{
					buttonText = "Player Name",
					overlapText = "Name: " + player.NickName,
					method = delegate
					{
						Main.ChangeName(player.NickName);
					},
					isTogglable = false,
					toolTip = "Sets your name to \"" + player.NickName + "\".",
					legal = true
				},
				new ButtonInfo
				{
					buttonText = "Player Color",
					overlapText = "Color: " + playerColor.ToRichRGBString(),
					method = delegate
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						Main.ChangeColor(playerColor);
					},
					isTogglable = false,
					toolTip = "Sets your color to the same as " + nickName + ".",
					legal = true
				},
				new ButtonInfo
				{
					buttonText = "Player User ID",
					overlapText = "User ID: " + player.UserId,
					method = delegate
					{
						NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully copied " + player.UserId + " to the clipboard!", 5000);
						GUIUtility.systemCopyBuffer = player.UserId;
					},
					isTogglable = false,
					toolTip = "Copies " + player.UserId + " to your clipboard."
				},
				new ButtonInfo
				{
					buttonText = "Player Creation Date",
					overlapText = "Creation Date: " + RigUtilities.GetCreationDate(player.UserId, delegate(string creationDate)
					{
						Buttons.GetIndex("Player Creation Date").overlapText = "Creation Date: " + creationDate;
						Main.ReloadMenu();
					}),
					label = true
				},
				null,
				null
			};
			ButtonInfo obj2 = new ButtonInfo
			{
				buttonText = "Player Platform"
			};
			VRRig obj3 = playerRig;
			obj2.overlapText = "Platform: " + ((obj3 != null && obj3.IsSteam()) ? "Steam" : "Quest");
			obj2.label = true;
			obj[5] = obj2;
			obj[6] = new ButtonInfo
			{
				buttonText = "Player FPS",
				overlapText = $"FPS: {playerRig.fps}",
				label = true,
				legal = true
			};
			list2.AddRange(obj);
		}
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list2.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void PlayerTracers()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)tracerTarget == (Object)null))
		{
			LineRenderer lineRender = Visuals.GetLineRender();
			lineRender.startColor = tracerTarget.playerColor;
			lineRender.endColor = tracerTarget.playerColor;
			lineRender.startWidth = 0.025f;
			lineRender.endWidth = 0.025f;
			lineRender.SetPosition(0, GorillaTagger.Instance.rightHandTransform.position);
			lineRender.SetPosition(1, ((Component)tracerTarget).transform.position);
		}
	}

	public static void SpectatePlayer(VRRig rig)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		GameObject cameraObject = new GameObject("Seralyth_SpectateCamera");
		RenderTexture val = new RenderTexture(512, 512, 16);
		cameraObject.AddComponent<Camera>().targetTexture = val;
		cameraObject.transform.SetParent(rig.headMesh.transform, false);
		cameraObject.transform.localPosition = new Vector3(0f, 0.25f, 0.25f);
		Main.promptMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
		{
			mainTexture = (Texture)(object)val
		};
		Main.PromptSingle("<https://.mat>", delegate
		{
			Object.Destroy((Object)(object)cameraObject);
		}, "Done");
	}

	public static void CategorySettings()
	{
		List<ButtonInfo> list = new List<ButtonInfo>
		{
			new ButtonInfo
			{
				buttonText = "Exit Menu Settings",
				method = delegate
				{
					Buttons.CurrentCategoryName = "Settings";
					Buttons.buttons[Buttons.GetCategory("Temporary Category")] = Array.Empty<ButtonInfo>();
				},
				isTogglable = false,
				toolTip = "Returns you back to the settings menu.",
				legal = true
			}
		};
		ButtonInfo[] array = Buttons.buttons[Buttons.GetCategory("Main")];
		foreach (ButtonInfo button in array)
		{
			list.Add(new ButtonInfo
			{
				buttonText = "Category" + button.buttonText.Hash(),
				overlapText = button.buttonText,
				enabled = !Main.skipButtons.Contains(button.buttonText),
				enableMethod = delegate
				{
					Main.skipButtons.Remove(button.buttonText);
				},
				disableMethod = delegate
				{
					Main.skipButtons.Add(button.buttonText);
				},
				toolTip = "Toggles the visibility of the category " + button.buttonText + ".",
				hideFromArraylist = true,
				legal = true
			});
		}
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void RightHand()
	{
		Main.rightHand = true;
		if (Main.watchMenu)
		{
			Main.Toggle("Watch Menu");
			Main.Toggle("Watch Menu");
			NotificationManager.ClearAllNotifications();
		}
		if (Buttons.GetIndex("Info Watch").enabled)
		{
			Main.Toggle("Info Watch");
			Main.Toggle("Info Watch");
			NotificationManager.ClearAllNotifications();
		}
	}

	public static void LeftHand()
	{
		Main.rightHand = false;
		if (Main.watchMenu)
		{
			Main.Toggle("Watch Menu");
			Main.Toggle("Watch Menu");
			NotificationManager.ClearAllNotifications();
		}
		if (Buttons.GetIndex("Info Watch").enabled)
		{
			Main.Toggle("Info Watch");
			Main.Toggle("Info Watch");
			NotificationManager.ClearAllNotifications();
		}
	}

	public static void ClearAllKeybinds()
	{
		foreach (KeyValuePair<string, List<string>> modBinding in Main.ModBindings)
		{
			foreach (string item in modBinding.Value)
			{
				ButtonInfo index = Buttons.GetIndex(item);
				if (index != null)
				{
					index.customBind = null;
					index.pcBindKey = null;
				}
			}
			modBinding.Value.Clear();
		}
		ButtonInfo[][] buttons = Buttons.buttons;
		foreach (ButtonInfo[] array in buttons)
		{
			ButtonInfo[] array2 = array;
			foreach (ButtonInfo buttonInfo in array2)
			{
				buttonInfo.rebindKey = null;
				buttonInfo.pcBindKey = null;
			}
		}
	}

	public static void StartBind(string bind)
	{
		if (!Main.IsRebinding)
		{
			Main.IsBinding = true;
			Main.BindInput = bind;
		}
	}

	public static void StartRebind(string bind)
	{
		if (!Main.IsBinding)
		{
			Main.IsRebinding = true;
			Main.BindInput = bind;
		}
	}

	public static void RemoveRebinds()
	{
		ButtonInfo[][] buttons = Buttons.buttons;
		foreach (ButtonInfo[] array in buttons)
		{
			ButtonInfo[] array2 = array;
			foreach (ButtonInfo buttonInfo in array2)
			{
				buttonInfo.rebindKey = null;
				buttonInfo.pcBindKey = null;
			}
		}
		NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Removed all rebinds.");
	}

	public static void UpdateMenu()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Invalid comparison between Unknown and I4
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Invalid comparison between Unknown and I4
		if (File.Exists("SeralythMenu/.custom-build"))
		{
			Seralyth.Classes.Menu.Console.SendNotification("<color=yellow>Custom build detected.</color> Update skipped to preserve local changes.", 5000);
			return;
		}
		OperatingSystemFamily operatingSystemFamily = SystemInfo.operatingSystemFamily;
		OperatingSystemFamily val = operatingSystemFamily;
		if ((int)val != 2)
		{
			if ((int)val == 3)
			{
				string text = "";
				string[] array = "\n                                            %%%%%                                                   \n                                           %%% %%%%                                                 \n                                         %%%      %%%%                                              \n                                        %%%         %%%%        %%%  %                              \n                                      %%%%            %%%%%%%% %%%%  %%                             \n                                     %%%        %#####% %%%%%        %%                             \n                                    %%%       ############ %%%                                      \n                                  %%%       ######     %###  %%%%     %%%                           \n                                %%%%       ######        ###   %#%%    %%                           \n                             %%%#%        ######         ###%    %#%%                               \n                       %%%%  %%#%         ######         %###      %##% %%                          \n                 %%%%  %%   %##           ######%         ##%         %###%                         \n                           %#%             ######        ###            ###%                        \n                         %##%              %######%    #####              ###%                      \n#%   %##                  #######%                        ###                    \n                   %% %##                     %#######%                        ###%                 \n###                        %########%                       ###%               \n###                            %#######%                       %##%             \n                  %##                                %#######%                        ###           \n                %##%                                   %#######%                     ###%           \n###                   %##########%        #######%                   ###             \n##%                  %####%    %####        %######%                ###               \n###                  %###%        %##%         %######%              ###                \n###                 ###%          %%%           %######%            ##%                 \n###              %###                          #######          ####                  \n                %###           ####                          #######        %###                    \n####         ####                          #######       ###   ##                 \n                    %###       ####                         %######       ##%    ##%                \n###      ###                         ######      ###                         \n                         %###   ####                       ######      ###        %%%               \n####  %####                   %######     ###           #%               \n                            %%###% ####%              ########      ##%         %%%                 \n###%%######%%    %#########%      ###     %%%% %%%%                 \n                             %#   %### %###############%         ##%%%%% %%%%                       \n                              %%    %##%                       %##  %                               \n                                       %##                    %#%                                   \n                               %%        %#%%               %%%%                                    \n                               %%%         %%#%            %%%                                      \n                                      %%%%%  %%%%        %%%%                                       \n                                 %%%%           %%%     %%%                                         \n                                                  %%%% %%%                                          \n                                                    %%%%                                            ".Split("\n");
				foreach (string text2 in array)
				{
					text = text + Environment.NewLine + " \"    " + text2 + " \"";
				}
				string contents = "#!/bin/bash\nclear\necho " + text + "\necho\necho \"Your menu is updating, please wait...\"\necho\n\nBASE_DIR=\"$(cd \"$(dirname \"$0\")/..\" && pwd)/\"\nPLUGIN_PATH=\"$BASE_DIR/BepInEx/plugins\"\nMODS_PATH=\"$BASE_DIR/Mods\"\n\nMENU_FILE=\"\"\n\nfor f in \"$PLUGIN_PATH\"/*Seralyth*Menu*.dll \"$MODS_PATH\"/*Seralyth*Menu*.dll; do\n    if [ -f \"$f\" ]; then\n        MENU_FILE=\"$f\"\n        break\n    fi\ndone\n\nif [ -z \"$MENU_FILE\" ]; then\n    echo \"No menu file found, skipping update.\"\nelse\n    echo \"Found menu file: $MENU_FILE\"\n\n    DOWNLOAD_NAME=\"Seralyth.Menu.Debug\"\n    if echo \"$MENU_FILE\" | grep -qi \"Legal\"; then\n        DOWNLOAD_NAME=\"Seralyth.Menu.Legal\"\n    fi\n\n    echo \"Downloading latest release of $DOWNLOAD_NAME...\"\n    curl -L -o \"$MENU_FILE\" \\\n    \"https://github.com/1x1x1x1736/api/releases/latest/download/${DOWNLOAD_NAME}.dll\"\nfi\n\nwhile pgrep -f \"GorillaTag.exe\" > /dev/null; do\n    sleep 1\ndone\n\necho \"Launching Gorilla Tag...\"\nxdg-open \"steam://run/1533390\"\nread -n 1 -s -r -p \"Press any key to continue . . .\"\nexit 0";
				string text3 = "SeralythMenu/UpdateScript.sh";
				File.WriteAllText(text3, contents);
				Process.Start("chmod", "+x \"" + text3 + "\"");
				Process.Start(new ProcessStartInfo
				{
					FileName = "/bin/bash",
					Arguments = "\"" + text3 + "\"",
					UseShellExecute = false
				});
				Application.Quit();
			}
		}
		else
		{
			string text4 = "";
			string[] array2 = "\n                                            %%%%%                                                   \n                                           %%% %%%%                                                 \n                                         %%%      %%%%                                              \n                                        %%%         %%%%        %%%  %                              \n                                      %%%%            %%%%%%%% %%%%  %%                             \n                                     %%%        %#####% %%%%%        %%                             \n                                    %%%       ############ %%%                                      \n                                  %%%       ######     %###  %%%%     %%%                           \n                                %%%%       ######        ###   %#%%    %%                           \n                             %%%#%        ######         ###%    %#%%                               \n                       %%%%  %%#%         ######         %###      %##% %%                          \n                 %%%%  %%   %##           ######%         ##%         %###%                         \n                           %#%             ######        ###            ###%                        \n                         %##%              %######%    #####              ###%                      \n#%   %##                  #######%                        ###                    \n                   %% %##                     %#######%                        ###%                 \n###                        %########%                       ###%               \n###                            %#######%                       %##%             \n                  %##                                %#######%                        ###           \n                %##%                                   %#######%                     ###%           \n###                   %##########%        #######%                   ###             \n##%                  %####%    %####        %######%                ###               \n###                  %###%        %##%         %######%              ###                \n###                 ###%          %%%           %######%            ##%                 \n###              %###                          #######          ####                  \n                %###           ####                          #######        %###                    \n####         ####                          #######       ###   ##                 \n                    %###       ####                         %######       ##%    ##%                \n###      ###                         ######      ###                         \n                         %###   ####                       ######      ###        %%%               \n####  %####                   %######     ###           #%               \n                            %%###% ####%              ########      ##%         %%%                 \n###%%######%%    %#########%      ###     %%%% %%%%                 \n                             %#   %### %###############%         ##%%%%% %%%%                       \n                              %%    %##%                       %##  %                               \n                                       %##                    %#%                                   \n                               %%        %#%%               %%%%                                    \n                               %%%         %%#%            %%%                                      \n                                      %%%%%  %%%%        %%%%                                       \n                                 %%%%           %%%     %%%                                         \n                                                  %%%% %%%                                          \n                                                    %%%%                                            ".Split("\n");
			foreach (string text5 in array2)
			{
				text4 = text4 + Environment.NewLine + " \"    " + text5 + " \"";
			}
			string contents2 = "@echo off\ntitle MrChicken Menu Updater\ncolor 5\nsetlocal\n\ncls\necho." + text4 + "\necho.\n\necho Your menu is updating, please wait...\necho.\n\nfor %%I in (\"%~dp0..\") do set \"BASE_DIR=%%~fI\\\"\nset \"PLUGIN_PATH=%BASE_DIR%BepInEx\\plugins\"\nset \"MODS_PATH=%BASE_DIR%Mods\"\n\nset \"MENU_FILE=\"\n\nfor %%F in (\"%PLUGIN_PATH%\\*Seralyth*Menu*.dll\" \"%MODS_PATH%\\*Seralyth*Menu*.dll\") do (\n    if exist \"%%~fF\" (\n        set \"MENU_FILE=%%~fF\"\n        goto update\n    )\n)\n\necho No menu file found, skipping update.\ngoto restart\n\n:update\necho Found menu file: \"%MENU_FILE%\"\n\nset \"DOWNLOAD_NAME=Seralyth.Menu.Debug\"\necho %MENU_FILE% | find /I \"Legal\" >nul\nif %ERRORLEVEL%==0 set \"DOWNLOAD_NAME=Seralyth.Menu.Legal\"\n\necho Downloading latest release of %DOWNLOAD_NAME%...\n\ncurl -L -o \"%MENU_FILE%\" ^\n\"https://github.com/1x1x1x1736/api/releases/latest/download/%DOWNLOAD_NAME%.dll\"\n\n:WAIT_LOOP\ntasklist /FI \"IMAGENAME eq Gorilla Tag.exe\" | find /I \"Gorilla Tag.exe\" >nul\nif %ERRORLEVEL%==0 (\n    timeout /t 1 >nul\n    goto WAIT_LOOP\n)\n\n:restart\necho Launching Gorilla Tag...\nstart steam://run/1533390\npause\nexit";
			string text6 = "SeralythMenu/UpdateScript.bat";
			File.WriteAllText(text6, contents2);
			string fileName = FileUtilities.GetGamePath() + "/" + text6;
			Process.Start(fileName);
			Application.Quit();
		}
	}

	public static void JoystickMenuOff()
	{
		Main.joystickMenu = false;
		Main.joystickOpen = false;
	}

	public static void PhysicalMenuOn()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Main.physicalMenu = true;
		Main.physicalOpenPosition = Vector3.zero;
	}

	public static void PhysicalMenuOff()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Main.physicalMenu = false;
		Main.physicalOpenPosition = Vector3.zero;
	}

	public static void WatchMenuOn()
	{
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		Main.watchMenu = true;
		GameObject gameObject = ((Component)((Component)VRRig.LocalRig).transform.Find("rig/hand.L/huntcomputer (1)")).gameObject;
		watchobject = Object.Instantiate<GameObject>(gameObject, Main.rightHand ? ((Component)((Component)VRRig.LocalRig).transform.Find("rig/hand.R")).transform : ((Component)((Component)VRRig.LocalRig).transform.Find("rig/hand.L")).transform, false);
		Object.Destroy((Object)(object)watchobject.GetComponent<GorillaHuntComputer>());
		watchobject.SetActive(true);
		Transform val = watchobject.transform.Find("HuntWatch_ScreenLocal/Canvas/Anchor");
		((Component)val.Find("Hat")).gameObject.SetActive(false);
		((Component)val.Find("Face")).gameObject.SetActive(false);
		((Component)val.Find("Badge")).gameObject.SetActive(false);
		((Component)val.Find("Material")).gameObject.SetActive(false);
		((Component)val.Find("Right Hand")).gameObject.SetActive(false);
		watchText = ((Component)val.Find("Text")).gameObject;
		watchEnabledIndicator = ((Component)val.Find("Left Hand")).gameObject;
		watchShell = ((Component)watchobject.transform.Find("HuntWatch_ScreenLocal")).gameObject;
		watchShell.GetComponent<Renderer>().material = CustomBoardManager.BoardMaterial;
		if (Main.rightHand)
		{
			watchShell.transform.localRotation = Quaternion.Euler(0f, 140f, 0f);
			Transform parent = watchShell.transform.parent;
			parent.localPosition += new Vector3(0.025f, 0f, 0f);
			Transform transform = watchShell.transform;
			transform.localPosition += new Vector3(0.025f, 0f, -0.035f);
		}
	}

	public static void CheckWatchMenu()
	{
		if (Main.watchTimer == 0f)
		{
			Main.watchTimer = Time.time + 10f;
		}
		if (((Vector2)(ref Main.leftJoystick)).sqrMagnitude > 0.010000001f)
		{
			Main.watchTimer = 0f;
			Main.watchUsed = true;
		}
		else if (!Main.watchUsed && Time.time >= Main.watchTimer)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=purple>WATCH</color><color=grey>]</color> Seems that you got stuck using Watch Menu, automatically disabling..");
			Main.Toggle("Watch Menu");
		}
	}

	public static void WatchMenuOff()
	{
		Main.watchMenu = false;
		Main.watchUsed = false;
		Main.watchTimer = 0f;
		Object.Destroy((Object)(object)watchobject);
	}

	public static void ChangeMenuLanguage(bool positive = true)
	{
		string[] array = new string[12]
		{
			"English", "Español", "Français", "Deutsch", "日本語", "Italiano", "Português", "Nederlands", "Русский", "Polski",
			"svenska", "dansk"
		};
		string[] array2 = new string[12]
		{
			"en", "es", "fr", "de", "ja", "it", "pt", "nl", "ru", "pl",
			"sw", "da"
		};
		if (!isLoadingPreferences)
		{
			if (positive)
			{
				langInd++;
			}
			else
			{
				langInd--;
			}
		}
		langInd %= array.Length;
		if (langInd < 0)
		{
			langInd = array.Length - 1;
		}
		TranslationManager.translateCache.Clear();
		TranslationManager.language = array2[langInd];
		Buttons.GetIndex("Change Menu Language").overlapText = "Change Menu Language <color=grey>[</color><color=green>" + array[langInd] + "</color><color=grey>]</color>";
		Main.translate = langInd != 0;
	}

	public static void ChangeCategoryDisplay(bool positive = true)
	{
		string[] array = new string[3] { "Next to FPS", "Title", "Title Changer" };
		if (!isLoadingPreferences)
		{
			if (positive)
			{
				Main.categoryDisplayMode++;
			}
			else
			{
				Main.categoryDisplayMode--;
			}
		}
		Main.categoryDisplayMode %= array.Length;
		if (Main.categoryDisplayMode < 0)
		{
			Main.categoryDisplayMode = array.Length - 1;
		}
		Buttons.GetIndex("Change Category Display").overlapText = "Change Category Display <color=grey>[</color><color=green>" + array[Main.categoryDisplayMode] + "</color><color=grey>]</color>";
	}

	public static void ChangeMenuButton(bool positive = true)
	{
		string[] array = new string[5] { "Primary", "Secondary", "Grip", "Trigger", "Joystick" };
		if (positive)
		{
			Main.menuButtonIndex++;
		}
		else
		{
			Main.menuButtonIndex--;
		}
		Main.menuButtonIndex %= array.Length;
		if (Main.menuButtonIndex < 0)
		{
			Main.menuButtonIndex = array.Length - 1;
		}
		Buttons.GetIndex("Change Menu Button").overlapText = "Change Menu Button <color=grey>[</color><color=green>" + array[Main.menuButtonIndex] + "</color><color=grey>]</color>";
	}

	public static void ChangeMenuTheme(bool increment = true)
	{
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_062c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0686: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_071b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0733: Unknown result type (might be due to invalid IL or missing references)
		//IL_074b: Unknown result type (might be due to invalid IL or missing references)
		//IL_076b: Unknown result type (might be due to invalid IL or missing references)
		//IL_077d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0782: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0800: Unknown result type (might be due to invalid IL or missing references)
		//IL_0818: Unknown result type (might be due to invalid IL or missing references)
		//IL_0830: Unknown result type (might be due to invalid IL or missing references)
		//IL_0850: Unknown result type (might be due to invalid IL or missing references)
		//IL_0855: Unknown result type (might be due to invalid IL or missing references)
		//IL_0881: Unknown result type (might be due to invalid IL or missing references)
		//IL_0899: Unknown result type (might be due to invalid IL or missing references)
		//IL_089e: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0911: Unknown result type (might be due to invalid IL or missing references)
		//IL_0926: Unknown result type (might be due to invalid IL or missing references)
		//IL_092b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0957: Unknown result type (might be due to invalid IL or missing references)
		//IL_097f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0984: Unknown result type (might be due to invalid IL or missing references)
		//IL_0989: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a64: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a69: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aeb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b33: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c56: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d34: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d85: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e08: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e59: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e71: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e76: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f32: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f85: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0feb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1013: Unknown result type (might be due to invalid IL or missing references)
		//IL_1018: Unknown result type (might be due to invalid IL or missing references)
		//IL_104b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1050: Unknown result type (might be due to invalid IL or missing references)
		//IL_1078: Unknown result type (might be due to invalid IL or missing references)
		//IL_107d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1095: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_10fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1121: Unknown result type (might be due to invalid IL or missing references)
		//IL_1139: Unknown result type (might be due to invalid IL or missing references)
		//IL_1151: Unknown result type (might be due to invalid IL or missing references)
		//IL_1171: Unknown result type (might be due to invalid IL or missing references)
		//IL_1176: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_11dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_120d: Unknown result type (might be due to invalid IL or missing references)
		//IL_122d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1232: Unknown result type (might be due to invalid IL or missing references)
		//IL_125e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1276: Unknown result type (might be due to invalid IL or missing references)
		//IL_1299: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_12c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_12fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1300: Unknown result type (might be due to invalid IL or missing references)
		//IL_132c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1351: Unknown result type (might be due to invalid IL or missing references)
		//IL_1356: Unknown result type (might be due to invalid IL or missing references)
		//IL_1386: Unknown result type (might be due to invalid IL or missing references)
		//IL_138b: Unknown result type (might be due to invalid IL or missing references)
		//IL_13b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_13b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_13cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_13f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_141e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1436: Unknown result type (might be due to invalid IL or missing references)
		//IL_1459: Unknown result type (might be due to invalid IL or missing references)
		//IL_1471: Unknown result type (might be due to invalid IL or missing references)
		//IL_1489: Unknown result type (might be due to invalid IL or missing references)
		//IL_14bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_14f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_151f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1524: Unknown result type (might be due to invalid IL or missing references)
		//IL_1547: Unknown result type (might be due to invalid IL or missing references)
		//IL_155f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1577: Unknown result type (might be due to invalid IL or missing references)
		//IL_1597: Unknown result type (might be due to invalid IL or missing references)
		//IL_159c: Unknown result type (might be due to invalid IL or missing references)
		//IL_15c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_15e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1603: Unknown result type (might be due to invalid IL or missing references)
		//IL_161b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1633: Unknown result type (might be due to invalid IL or missing references)
		//IL_1667: Unknown result type (might be due to invalid IL or missing references)
		//IL_166c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1682: Unknown result type (might be due to invalid IL or missing references)
		//IL_1687: Unknown result type (might be due to invalid IL or missing references)
		//IL_16c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_16cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_16f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_16fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_172e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1733: Unknown result type (might be due to invalid IL or missing references)
		//IL_175c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1761: Unknown result type (might be due to invalid IL or missing references)
		//IL_178d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1792: Unknown result type (might be due to invalid IL or missing references)
		//IL_17bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_17c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_17d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_17dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1815: Unknown result type (might be due to invalid IL or missing references)
		//IL_181a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1840: Unknown result type (might be due to invalid IL or missing references)
		//IL_1845: Unknown result type (might be due to invalid IL or missing references)
		//IL_1876: Unknown result type (might be due to invalid IL or missing references)
		//IL_187b: Unknown result type (might be due to invalid IL or missing references)
		//IL_18a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_18a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_18cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_18d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_18fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1900: Unknown result type (might be due to invalid IL or missing references)
		//IL_1937: Unknown result type (might be due to invalid IL or missing references)
		//IL_193c: Unknown result type (might be due to invalid IL or missing references)
		//IL_195f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1964: Unknown result type (might be due to invalid IL or missing references)
		//IL_1987: Unknown result type (might be due to invalid IL or missing references)
		//IL_199f: Unknown result type (might be due to invalid IL or missing references)
		//IL_19b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_19d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_19e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_19eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a17: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a52: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a82: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aa2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ad5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1af4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b36: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b81: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bbe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bc3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bec: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bf1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c14: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c44: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c72: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cb2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cb7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ccf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cf2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d22: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d50: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d55: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d95: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1db2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ded: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e05: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e30: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e35: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e71: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ed6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1edb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f07: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f24: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f54: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f90: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fb3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fb8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fdb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ff3: Unknown result type (might be due to invalid IL or missing references)
		//IL_200b: Unknown result type (might be due to invalid IL or missing references)
		//IL_202b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2030: Unknown result type (might be due to invalid IL or missing references)
		//IL_205c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2074: Unknown result type (might be due to invalid IL or missing references)
		//IL_2097: Unknown result type (might be due to invalid IL or missing references)
		//IL_20af: Unknown result type (might be due to invalid IL or missing references)
		//IL_20c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_20e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_211e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2123: Unknown result type (might be due to invalid IL or missing references)
		//IL_2146: Unknown result type (might be due to invalid IL or missing references)
		//IL_214b: Unknown result type (might be due to invalid IL or missing references)
		//IL_216e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2186: Unknown result type (might be due to invalid IL or missing references)
		//IL_219e: Unknown result type (might be due to invalid IL or missing references)
		//IL_21cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_21d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_21ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_21f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_2229: Unknown result type (might be due to invalid IL or missing references)
		//IL_222e: Unknown result type (might be due to invalid IL or missing references)
		//IL_223e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2243: Unknown result type (might be due to invalid IL or missing references)
		//IL_2266: Unknown result type (might be due to invalid IL or missing references)
		//IL_226b: Unknown result type (might be due to invalid IL or missing references)
		//IL_227b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2280: Unknown result type (might be due to invalid IL or missing references)
		//IL_22a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_22bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_22d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_22fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_2303: Unknown result type (might be due to invalid IL or missing references)
		//IL_233a: Unknown result type (might be due to invalid IL or missing references)
		//IL_233f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2362: Unknown result type (might be due to invalid IL or missing references)
		//IL_2367: Unknown result type (might be due to invalid IL or missing references)
		//IL_238a: Unknown result type (might be due to invalid IL or missing references)
		//IL_23a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_23ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_23da: Unknown result type (might be due to invalid IL or missing references)
		//IL_23ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_23ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_2426: Unknown result type (might be due to invalid IL or missing references)
		//IL_242b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2443: Unknown result type (might be due to invalid IL or missing references)
		//IL_2466: Unknown result type (might be due to invalid IL or missing references)
		//IL_247e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2496: Unknown result type (might be due to invalid IL or missing references)
		//IL_24c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_24c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_24f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_250a: Unknown result type (might be due to invalid IL or missing references)
		//IL_252d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2545: Unknown result type (might be due to invalid IL or missing references)
		//IL_255d: Unknown result type (might be due to invalid IL or missing references)
		//IL_257d: Unknown result type (might be due to invalid IL or missing references)
		//IL_258b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2590: Unknown result type (might be due to invalid IL or missing references)
		//IL_25bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_25dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_25e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_2605: Unknown result type (might be due to invalid IL or missing references)
		//IL_261d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2635: Unknown result type (might be due to invalid IL or missing references)
		//IL_2660: Unknown result type (might be due to invalid IL or missing references)
		//IL_2665: Unknown result type (might be due to invalid IL or missing references)
		//IL_269c: Unknown result type (might be due to invalid IL or missing references)
		//IL_26a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_26c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_26cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_26ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_2707: Unknown result type (might be due to invalid IL or missing references)
		//IL_271f: Unknown result type (might be due to invalid IL or missing references)
		//IL_273f: Unknown result type (might be due to invalid IL or missing references)
		//IL_276b: Unknown result type (might be due to invalid IL or missing references)
		//IL_278e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2793: Unknown result type (might be due to invalid IL or missing references)
		//IL_27b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_27ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_27e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_2806: Unknown result type (might be due to invalid IL or missing references)
		//IL_2832: Unknown result type (might be due to invalid IL or missing references)
		//IL_285e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2863: Unknown result type (might be due to invalid IL or missing references)
		//IL_2886: Unknown result type (might be due to invalid IL or missing references)
		//IL_289e: Unknown result type (might be due to invalid IL or missing references)
		//IL_28b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_28d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_2913: Unknown result type (might be due to invalid IL or missing references)
		//IL_2918: Unknown result type (might be due to invalid IL or missing references)
		//IL_2930: Unknown result type (might be due to invalid IL or missing references)
		//IL_2953: Unknown result type (might be due to invalid IL or missing references)
		//IL_296b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2983: Unknown result type (might be due to invalid IL or missing references)
		//IL_29a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_29d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_29ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a18: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a30: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a48: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a79: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a97: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ad6: Unknown result type (might be due to invalid IL or missing references)
		//IL_2adb: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b07: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b62: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b67: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2bba: Unknown result type (might be due to invalid IL or missing references)
		//IL_2bbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_2bf6: Unknown result type (might be due to invalid IL or missing references)
		//IL_2bfb: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c23: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c51: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c56: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c79: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ca1: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ca6: Unknown result type (might be due to invalid IL or missing references)
		//IL_2cd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_2cd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ce7: Unknown result type (might be due to invalid IL or missing references)
		//IL_2cec: Unknown result type (might be due to invalid IL or missing references)
		//IL_2d25: Unknown result type (might be due to invalid IL or missing references)
		//IL_2d2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2d4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2d51: Unknown result type (might be due to invalid IL or missing references)
		//IL_2d74: Unknown result type (might be due to invalid IL or missing references)
		//IL_2d8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2da4: Unknown result type (might be due to invalid IL or missing references)
		//IL_2dc4: Unknown result type (might be due to invalid IL or missing references)
		//IL_2df7: Unknown result type (might be due to invalid IL or missing references)
		//IL_2e0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2e39: Unknown result type (might be due to invalid IL or missing references)
		//IL_2e51: Unknown result type (might be due to invalid IL or missing references)
		//IL_2e69: Unknown result type (might be due to invalid IL or missing references)
		//IL_2e94: Unknown result type (might be due to invalid IL or missing references)
		//IL_2e99: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ed0: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ed5: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f01: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f06: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f42: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f73: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2fb9: Unknown result type (might be due to invalid IL or missing references)
		//IL_2fbe: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ff8: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ffd: Unknown result type (might be due to invalid IL or missing references)
		//IL_3022: Unknown result type (might be due to invalid IL or missing references)
		//IL_3027: Unknown result type (might be due to invalid IL or missing references)
		//IL_304a: Unknown result type (might be due to invalid IL or missing references)
		//IL_3062: Unknown result type (might be due to invalid IL or missing references)
		//IL_307a: Unknown result type (might be due to invalid IL or missing references)
		//IL_30a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_30ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_30e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_30ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_3115: Unknown result type (might be due to invalid IL or missing references)
		//IL_311a: Unknown result type (might be due to invalid IL or missing references)
		//IL_313d: Unknown result type (might be due to invalid IL or missing references)
		//IL_3155: Unknown result type (might be due to invalid IL or missing references)
		//IL_316d: Unknown result type (might be due to invalid IL or missing references)
		//IL_319a: Unknown result type (might be due to invalid IL or missing references)
		//IL_319f: Unknown result type (might be due to invalid IL or missing references)
		//IL_31d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_31da: Unknown result type (might be due to invalid IL or missing references)
		//IL_31fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_3201: Unknown result type (might be due to invalid IL or missing references)
		//IL_3235: Unknown result type (might be due to invalid IL or missing references)
		//IL_323a: Unknown result type (might be due to invalid IL or missing references)
		//IL_3252: Unknown result type (might be due to invalid IL or missing references)
		//IL_326a: Unknown result type (might be due to invalid IL or missing references)
		//IL_3295: Unknown result type (might be due to invalid IL or missing references)
		//IL_329a: Unknown result type (might be due to invalid IL or missing references)
		//IL_32d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_32d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_32f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_32fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_3335: Unknown result type (might be due to invalid IL or missing references)
		//IL_333a: Unknown result type (might be due to invalid IL or missing references)
		//IL_3366: Unknown result type (might be due to invalid IL or missing references)
		//IL_336b: Unknown result type (might be due to invalid IL or missing references)
		//IL_3397: Unknown result type (might be due to invalid IL or missing references)
		//IL_339c: Unknown result type (might be due to invalid IL or missing references)
		//IL_33c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_33cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_3403: Unknown result type (might be due to invalid IL or missing references)
		//IL_3408: Unknown result type (might be due to invalid IL or missing references)
		//IL_342e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3433: Unknown result type (might be due to invalid IL or missing references)
		//IL_3456: Unknown result type (might be due to invalid IL or missing references)
		//IL_346e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3486: Unknown result type (might be due to invalid IL or missing references)
		//IL_34b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_34b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_34ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_34f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_351e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3523: Unknown result type (might be due to invalid IL or missing references)
		//IL_3539: Unknown result type (might be due to invalid IL or missing references)
		//IL_353e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3561: Unknown result type (might be due to invalid IL or missing references)
		//IL_3579: Unknown result type (might be due to invalid IL or missing references)
		//IL_3591: Unknown result type (might be due to invalid IL or missing references)
		//IL_35bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_35c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_35f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_35fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_3620: Unknown result type (might be due to invalid IL or missing references)
		//IL_3625: Unknown result type (might be due to invalid IL or missing references)
		//IL_363b: Unknown result type (might be due to invalid IL or missing references)
		//IL_3640: Unknown result type (might be due to invalid IL or missing references)
		//IL_3663: Unknown result type (might be due to invalid IL or missing references)
		//IL_367b: Unknown result type (might be due to invalid IL or missing references)
		//IL_3693: Unknown result type (might be due to invalid IL or missing references)
		//IL_36be: Unknown result type (might be due to invalid IL or missing references)
		//IL_36c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_36fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_36ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_372b: Unknown result type (might be due to invalid IL or missing references)
		//IL_3730: Unknown result type (might be due to invalid IL or missing references)
		//IL_3742: Unknown result type (might be due to invalid IL or missing references)
		//IL_3747: Unknown result type (might be due to invalid IL or missing references)
		//IL_376a: Unknown result type (might be due to invalid IL or missing references)
		//IL_3782: Unknown result type (might be due to invalid IL or missing references)
		//IL_379a: Unknown result type (might be due to invalid IL or missing references)
		//IL_37c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_37ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_3801: Unknown result type (might be due to invalid IL or missing references)
		//IL_3806: Unknown result type (might be due to invalid IL or missing references)
		//IL_3832: Unknown result type (might be due to invalid IL or missing references)
		//IL_3837: Unknown result type (might be due to invalid IL or missing references)
		//IL_384a: Unknown result type (might be due to invalid IL or missing references)
		//IL_384f: Unknown result type (might be due to invalid IL or missing references)
		//IL_3872: Unknown result type (might be due to invalid IL or missing references)
		//IL_388a: Unknown result type (might be due to invalid IL or missing references)
		//IL_38a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_38cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_38d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_38ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_3904: Unknown result type (might be due to invalid IL or missing references)
		//IL_392a: Unknown result type (might be due to invalid IL or missing references)
		//IL_392f: Unknown result type (might be due to invalid IL or missing references)
		//IL_393e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3943: Unknown result type (might be due to invalid IL or missing references)
		//IL_3966: Unknown result type (might be due to invalid IL or missing references)
		//IL_397e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3996: Unknown result type (might be due to invalid IL or missing references)
		//IL_39b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_39bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a10: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a28: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a40: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a60: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_3aa4: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ac7: Unknown result type (might be due to invalid IL or missing references)
		//IL_3adf: Unknown result type (might be due to invalid IL or missing references)
		//IL_3af7: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b22: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b31: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b38: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b42: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b47: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b58: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b64: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b73: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_3bbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_3bc9: Unknown result type (might be due to invalid IL or missing references)
		//IL_3bce: Unknown result type (might be due to invalid IL or missing references)
		//IL_3bd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_3bdf: Unknown result type (might be due to invalid IL or missing references)
		//IL_3be4: Unknown result type (might be due to invalid IL or missing references)
		//IL_3beb: Unknown result type (might be due to invalid IL or missing references)
		//IL_3bf5: Unknown result type (might be due to invalid IL or missing references)
		//IL_3bfa: Unknown result type (might be due to invalid IL or missing references)
		//IL_3c01: Unknown result type (might be due to invalid IL or missing references)
		//IL_3c0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_3c10: Unknown result type (might be due to invalid IL or missing references)
		//IL_3c30: Unknown result type (might be due to invalid IL or missing references)
		//IL_3c48: Unknown result type (might be due to invalid IL or missing references)
		//IL_3c60: Unknown result type (might be due to invalid IL or missing references)
		//IL_3c8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_3c95: Unknown result type (might be due to invalid IL or missing references)
		//IL_3c9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ca1: Unknown result type (might be due to invalid IL or missing references)
		//IL_3cab: Unknown result type (might be due to invalid IL or missing references)
		//IL_3cb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_3cb7: Unknown result type (might be due to invalid IL or missing references)
		//IL_3cc1: Unknown result type (might be due to invalid IL or missing references)
		//IL_3cc6: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ccd: Unknown result type (might be due to invalid IL or missing references)
		//IL_3cd7: Unknown result type (might be due to invalid IL or missing references)
		//IL_3cdc: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ce3: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ced: Unknown result type (might be due to invalid IL or missing references)
		//IL_3cf2: Unknown result type (might be due to invalid IL or missing references)
		//IL_3cf9: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d03: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d08: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d31: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d54: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d63: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d74: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d79: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d80: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d96: Unknown result type (might be due to invalid IL or missing references)
		//IL_3da0: Unknown result type (might be due to invalid IL or missing references)
		//IL_3da5: Unknown result type (might be due to invalid IL or missing references)
		//IL_3dac: Unknown result type (might be due to invalid IL or missing references)
		//IL_3db6: Unknown result type (might be due to invalid IL or missing references)
		//IL_3dbb: Unknown result type (might be due to invalid IL or missing references)
		//IL_3dc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_3dcc: Unknown result type (might be due to invalid IL or missing references)
		//IL_3dd1: Unknown result type (might be due to invalid IL or missing references)
		//IL_3df1: Unknown result type (might be due to invalid IL or missing references)
		//IL_3e09: Unknown result type (might be due to invalid IL or missing references)
		//IL_3e21: Unknown result type (might be due to invalid IL or missing references)
		if (!isLoadingPreferences)
		{
			if (increment)
			{
				Main.themeType++;
			}
			else
			{
				Main.themeType--;
			}
		}
		if (Main.themeType > 65)
		{
			Main.themeType = 1;
		}
		if (Main.themeType < 1)
		{
			Main.themeType = 65;
		}
		if (!Buttons.GetIndex("Custom Menu Theme").enabled)
		{
			switch (Main.themeType)
			{
			case 1:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)118, (byte)6, (byte)252, (byte)128)))
				};
				Main.menuBackgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)22, (byte)22, (byte)22, (byte)128)))
				};
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)118, (byte)6, (byte)252, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)88, (byte)6, (byte)186, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 2:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color.blue, Color.magenta)
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.blue)
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 3:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color.black)
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)50, (byte)50, (byte)50, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)20, (byte)20, (byte)20, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 4:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color.white, Color.black)
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSimpleGradient(Color.black, Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					}
				};
				break;
			case 5:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color.black, Color32.op_Implicit(new Color32((byte)110, (byte)0, (byte)0, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSimpleGradient(Color.black, Color32.op_Implicit(new Color32((byte)110, (byte)0, (byte)0, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)110, (byte)0, (byte)0, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 6:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color.black),
					rainbow = true
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black),
						rainbow = true
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 7:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color.black),
					copyRigColor = true
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black),
						copyRigColor = true
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 8:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color.black, Color32.op_Implicit(new Color32(byte.MaxValue, (byte)111, (byte)0, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSimpleGradient(Color32.op_Implicit(new Color32(byte.MaxValue, (byte)111, (byte)0, byte.MaxValue)), Color.black)
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 9:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color.black, Color.red)
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSimpleGradient(Color.red, Color.black)
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 10:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color.black, Color32.op_Implicit(new Color32((byte)0, (byte)174, byte.MaxValue, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSimpleGradient(Color32.op_Implicit(new Color32((byte)0, (byte)174, byte.MaxValue, byte.MaxValue)), Color.black)
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 11:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color32.op_Implicit(new Color32((byte)0, (byte)136, byte.MaxValue, byte.MaxValue)), Color32.op_Implicit(new Color32((byte)0, (byte)174, byte.MaxValue, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)0, (byte)100, (byte)188, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSimpleGradient(Color32.op_Implicit(new Color32((byte)0, (byte)174, byte.MaxValue, byte.MaxValue)), Color32.op_Implicit(new Color32((byte)0, (byte)136, byte.MaxValue, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					}
				};
				break;
			case 12:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color32.op_Implicit(new Color32((byte)0, byte.MaxValue, (byte)246, byte.MaxValue)), Color32.op_Implicit(new Color32((byte)0, byte.MaxValue, (byte)144, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSimpleGradient(Color32.op_Implicit(new Color32((byte)0, byte.MaxValue, (byte)144, byte.MaxValue)), Color32.op_Implicit(new Color32((byte)0, byte.MaxValue, (byte)246, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					}
				};
				break;
			case 13:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color32.op_Implicit(new Color32(byte.MaxValue, (byte)130, byte.MaxValue, byte.MaxValue)), Color.white)
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32(byte.MaxValue, (byte)130, byte.MaxValue, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					}
				};
				break;
			case 14:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color32.op_Implicit(new Color32((byte)122, (byte)35, (byte)159, byte.MaxValue)), Color32.op_Implicit(new Color32((byte)60, (byte)26, (byte)89, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)60, (byte)26, (byte)89, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)122, (byte)35, (byte)159, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 15:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color.magenta, Color.cyan)
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSimpleGradient(Color.magenta, Color.cyan)
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					}
				};
				break;
			case 16:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color.red, Color.black)
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.red)
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.red)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.red)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					}
				};
				break;
			case 17:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color32.op_Implicit(new Color32(byte.MaxValue, (byte)128, (byte)0, byte.MaxValue)), Color.black)
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32(byte.MaxValue, (byte)128, (byte)0, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32(byte.MaxValue, (byte)128, (byte)0, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32(byte.MaxValue, (byte)128, (byte)0, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					}
				};
				break;
			case 18:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color.yellow, Color.black)
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.yellow)
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.yellow)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.yellow)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					}
				};
				break;
			case 19:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color.green, Color.black)
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.green)
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.green)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.green)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					}
				};
				break;
			case 20:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color.blue, Color.black)
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.blue)
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.blue)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.blue)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					}
				};
				break;
			case 21:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color32.op_Implicit(new Color32((byte)119, (byte)0, byte.MaxValue, byte.MaxValue)), Color.black)
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)119, (byte)0, byte.MaxValue, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)119, (byte)0, byte.MaxValue, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)119, (byte)0, byte.MaxValue, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					}
				};
				break;
			case 22:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color.magenta, Color.black)
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.magenta)
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.magenta)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.magenta)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					}
				};
				break;
			case 23:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color32.op_Implicit(new Color32(byte.MaxValue, byte.MaxValue, (byte)130, byte.MaxValue)), Color.white)
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32(byte.MaxValue, byte.MaxValue, (byte)130, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					}
				};
				break;
			case 24:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color.red, Color.green)
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					}
				};
				break;
			case 25:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color32.op_Implicit(new Color32((byte)245, (byte)169, (byte)184, byte.MaxValue)), Color32.op_Implicit(new Color32((byte)91, (byte)206, (byte)250, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)245, (byte)169, (byte)184, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)91, (byte)206, (byte)250, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)91, (byte)206, (byte)250, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)91, (byte)206, (byte)250, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)245, (byte)169, (byte)184, byte.MaxValue)))
					}
				};
				break;
			case 26:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color32.op_Implicit(new Color32((byte)7, (byte)141, (byte)112, byte.MaxValue)), Color32.op_Implicit(new Color32((byte)61, (byte)26, (byte)220, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)7, (byte)141, (byte)112, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)61, (byte)26, (byte)220, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)61, (byte)26, (byte)220, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)61, (byte)26, (byte)220, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)7, (byte)141, (byte)112, byte.MaxValue)))
					}
				};
				break;
			case 27:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)50, (byte)50, (byte)50, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)50, (byte)50, (byte)50, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)75, (byte)75, (byte)75, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 28:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color.black, Color32.op_Implicit(new Color32((byte)80, (byte)0, (byte)80, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.green)
					}
				};
				break;
			case 29:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color.black),
					transparent = true
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white),
						transparent = true
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.green),
						transparent = true
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.green)
					}
				};
				break;
			case 30:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)100, (byte)60, (byte)170, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)150, (byte)100, (byte)240, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)150, (byte)100, (byte)240, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.cyan)
					}
				};
				break;
			case 31:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)0, (byte)59, (byte)4, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)192, (byte)190, (byte)171, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.red)
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					}
				};
				break;
			case 32:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)225, (byte)73, (byte)43, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)192, (byte)190, (byte)171, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.red)
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					}
				};
				break;
			case 33:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)25, (byte)25, (byte)25, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)40, (byte)40, (byte)40, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)167, (byte)66, (byte)191, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)144, (byte)144, (byte)144, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)144, (byte)144, (byte)144, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 34:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)26, (byte)26, (byte)61, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)26, (byte)26, (byte)61, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)43, (byte)17, (byte)84, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 35:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color.black, Color.gray)
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.yellow)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.magenta)
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					}
				};
				break;
			case 36:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color.black)
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)32, (byte)32, (byte)32, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)32, (byte)32, (byte)32, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 37:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color32.op_Implicit(new Color32((byte)199, (byte)115, (byte)173, byte.MaxValue)), Color32.op_Implicit(new Color32((byte)165, (byte)233, (byte)185, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSimpleGradient(Color32.op_Implicit(new Color32((byte)99, (byte)58, (byte)86, byte.MaxValue)), Color32.op_Implicit(new Color32((byte)83, (byte)116, (byte)92, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSimpleGradient(Color32.op_Implicit(new Color32((byte)99, (byte)58, (byte)86, byte.MaxValue)), Color32.op_Implicit(new Color32((byte)83, (byte)116, (byte)92, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.green)
					}
				};
				break;
			case 38:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)27, (byte)27, (byte)27, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)50, (byte)50, (byte)50, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)66, (byte)66, (byte)66, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 39:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color.black, Color32.op_Implicit(new Color32((byte)100, (byte)25, (byte)125, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)25, (byte)25, (byte)25, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.green)
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 40:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)27, (byte)27, (byte)27, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.red)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.green)
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 41:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color.black, Color32.op_Implicit(new Color32((byte)100, (byte)0, (byte)0, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)100, (byte)0, (byte)0, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 42:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)21, (byte)22, (byte)23, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)32, (byte)50, (byte)77, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)60, (byte)127, (byte)206, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 43:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color.black)
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)10, (byte)10, (byte)10, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 44:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color.white)
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)245, (byte)245, (byte)245, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					}
				};
				break;
			case 45:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color.black)
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32(byte.MaxValue, (byte)163, (byte)26, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 46:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color.black),
					epileptic = true
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black),
						epileptic = true
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 47:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color32.op_Implicit(new Color32((byte)111, (byte)143, byte.MaxValue, byte.MaxValue)), Color32.op_Implicit(new Color32((byte)163, (byte)184, byte.MaxValue, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)96, (byte)125, (byte)219, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)147, (byte)167, (byte)226, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)33, (byte)33, (byte)101, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)33, (byte)33, (byte)101, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)33, (byte)33, (byte)101, byte.MaxValue)))
					}
				};
				break;
			case 48:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)19, (byte)22, (byte)27, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)19, (byte)22, (byte)27, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)16, (byte)18, (byte)22, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)82, (byte)96, (byte)122, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)82, (byte)96, (byte)122, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)82, (byte)96, (byte)122, byte.MaxValue)))
					}
				};
				break;
			case 49:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color32.op_Implicit(new Color32((byte)0, (byte)136, (byte)16, byte.MaxValue)), Color32.op_Implicit(new Color32((byte)0, (byte)127, (byte)14, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)0, (byte)158, (byte)15, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)0, (byte)112, (byte)11, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 50:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color.white),
					pastelRainbow = true
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white),
						pastelRainbow = true
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					}
				};
				break;
			case 51:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)25, (byte)25, (byte)25, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)40, (byte)40, (byte)40, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)165, (byte)137, byte.MaxValue, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)144, (byte)144, (byte)144, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)144, (byte)144, (byte)144, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 52:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)176, (byte)12, (byte)64, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)140, (byte)10, (byte)51, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)250, (byte)2, (byte)81, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 53:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)124, (byte)25, (byte)194, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)88, (byte)9, (byte)145, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)136, (byte)9, (byte)227, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 54:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)1, (byte)73, (byte)149, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)1, (byte)46, (byte)87, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)0, (byte)37, (byte)74, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)252, (byte)179, (byte)40, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 55:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)30, (byte)30, (byte)46, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)88, (byte)91, (byte)112, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)49, (byte)50, (byte)68, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)205, (byte)214, (byte)244, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)186, (byte)194, (byte)222, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)166, (byte)173, (byte)200, byte.MaxValue)))
					}
				};
				break;
			case 56:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)45, (byte)25, (byte)75, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)40, (byte)15, (byte)60, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)100, (byte)30, (byte)140, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 57:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)32, (byte)32, (byte)32, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)45, (byte)46, (byte)51, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSimpleGradient(Color32.op_Implicit(new Color32((byte)231, (byte)133, (byte)209, byte.MaxValue)), Color32.op_Implicit(new Color32((byte)56, (byte)155, (byte)193, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 58:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)32, (byte)32, (byte)32, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)45, (byte)46, (byte)51, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSimpleGradient(Color32.op_Implicit(new Color32((byte)40, (byte)94, (byte)93, byte.MaxValue)), Color32.op_Implicit(new Color32((byte)66, (byte)158, (byte)157, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 59:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)32, (byte)32, (byte)32, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)45, (byte)46, (byte)51, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSimpleGradient(Color32.op_Implicit(new Color32(byte.MaxValue, (byte)166, (byte)201, byte.MaxValue)), Color32.op_Implicit(new Color32((byte)228, (byte)0, (byte)70, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 60:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)32, (byte)32, (byte)32, byte.MaxValue)))
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)45, (byte)46, (byte)51, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSimpleGradient(Color32.op_Implicit(new Color32((byte)177, (byte)162, (byte)202, byte.MaxValue)), Color32.op_Implicit(new Color32((byte)104, (byte)71, (byte)141, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 61:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)32, (byte)32, (byte)32, byte.MaxValue)))
				};
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color32.op_Implicit(new Color32((byte)45, (byte)46, (byte)51, byte.MaxValue)))
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSimpleGradient(Color32.op_Implicit(new Color32((byte)60, (byte)82, (byte)145, byte.MaxValue)), Color32.op_Implicit(new Color32((byte)0, (byte)20, (byte)64, byte.MaxValue)))
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 62:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSimpleGradient(Color.black, Color.white)
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						transparent = true
					},
					new ExtGradient
					{
						transparent = true
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 63:
				Main.backgroundColor = new ExtGradient
				{
					colors = ExtGradient.GetSolidGradient(Color.red)
				};
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.red)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			case 64:
			{
				ExtGradient extGradient = new ExtGradient();
				extGradient.colors = (GradientColorKey[])(object)new GradientColorKey[4]
				{
					new GradientColorKey(Color.red, 0f),
					new GradientColorKey(Color.green, 0.333f),
					new GradientColorKey(Color.blue, 0.666f),
					new GradientColorKey(Color.red, 1f)
				};
				Main.backgroundColor = extGradient;
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = (GradientColorKey[])(object)new GradientColorKey[4]
						{
							new GradientColorKey(Color.red, 0f),
							new GradientColorKey(Color.green, 0.333f),
							new GradientColorKey(Color.blue, 0.666f),
							new GradientColorKey(Color.red, 1f)
						}
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			}
			case 65:
			{
				ExtGradient extGradient = new ExtGradient();
				extGradient.colors = (GradientColorKey[])(object)new GradientColorKey[6]
				{
					new GradientColorKey(Color.yellow, 0f),
					new GradientColorKey(Color.red, 0.2f),
					new GradientColorKey(Color.magenta, 0.4f),
					new GradientColorKey(Color.blue, 0.6f),
					new GradientColorKey(Color.green, 0.8f),
					new GradientColorKey(Color.yellow, 1f)
				};
				Main.backgroundColor = extGradient;
				Main.menuBackgroundColor = Main.backgroundColor;
				Main.buttonColors = new ExtGradient[2]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.black)
					},
					new ExtGradient
					{
						colors = (GradientColorKey[])(object)new GradientColorKey[6]
						{
							new GradientColorKey(Color.yellow, 0f),
							new GradientColorKey(Color.red, 0.2f),
							new GradientColorKey(Color.magenta, 0.4f),
							new GradientColorKey(Color.blue, 0.6f),
							new GradientColorKey(Color.green, 0.8f),
							new GradientColorKey(Color.yellow, 1f)
						}
					}
				};
				Main.textColors = new ExtGradient[3]
				{
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					},
					new ExtGradient
					{
						colors = ExtGradient.GetSolidGradient(Color.white)
					}
				};
				break;
			}
			}
			string[] array = new string[65]
			{
				"Seralyth", "Blue Magenta", "Dark Mode", "Strobe", "Kman", "Rainbow", "Player Material", "Lava", "Rock", "Ice",
				"Water", "Minty", "Pink", "Purple", "Magenta Cyan", "Red Fade", "Orange Fade", "Yellow Fade", "Green Fade", "Blue Fade",
				"Purple Fade", "Magenta Fade", "Banana", "Pride", "Trans", "MLM or Gay", "Steal (old)", "Silence", "Transparent", "King",
				"Scoreboard", "Scoreboard (banned)", "Rift", "Blurple Dark", "ShibaGT Gold", "ShibaGT Genesis", "wyvern", "Steal (new)", "USA Menu", "Watch",
				"AZ Menu", "ImGUI", "Clean Dark", "Discord Light", "The Hub", "EPILEPTIC", "Discord Blurple", "VS Zero", "Weed", "Pastel Rainbow",
				"Rift Light", "Rose", "Tenacity", "e621", "Catppuccin Mocha", "Rexon", "Tenacity (MC)", "Mint Blue", "Pink Blood", "Purple Fire",
				"Deep Ocean", "Bad Apple", "coolkidd", "Old ShibaGT RGB", "Old-ish ShibaGT RGB"
			};
			string text = ((Main.themeType >= 1 && Main.themeType <= array.Length) ? array[Main.themeType - 1] : "Unknown");
			Buttons.GetIndex("Change Menu Theme").overlapText = "Change Menu Theme <color=grey>[</color><color=green>" + text + "</color><color=grey>]</color>";
		}
	}

	public static void ChangeMenuScale(bool positive = true)
	{
		if (!isLoadingPreferences)
		{
			if (positive)
			{
				menuScaleIndex++;
			}
			else
			{
				menuScaleIndex--;
			}
		}
		if (menuScaleIndex > 30)
		{
			menuScaleIndex = 2;
		}
		if (menuScaleIndex < 2)
		{
			menuScaleIndex = 30;
		}
		Main.menuScale = (float)menuScaleIndex / 10f;
		Buttons.GetIndex("Change Menu Scale").overlapText = "Change Menu Scale <color=grey>[</color><color=green>" + Main.menuScale + "</color><color=grey>]</color>";
	}

	public static void ChangeNotificationScale(bool positive = true)
	{
		if (!isLoadingPreferences)
		{
			if (positive)
			{
				notificationScaleIndex++;
			}
			else
			{
				notificationScaleIndex--;
			}
		}
		if (notificationScaleIndex > 20)
		{
			notificationScaleIndex = 1;
		}
		if (notificationScaleIndex < 1)
		{
			notificationScaleIndex = 20;
		}
		Main.notificationScale = notificationScaleIndex * 5;
		Buttons.GetIndex("Change Notification Scale").overlapText = "Change Notification Scale <color=grey>[</color><color=green>" + notificationScaleIndex + "</color><color=grey>]</color>";
	}

	public static void ChangeArraylistScale(bool positive = true)
	{
		if (!isLoadingPreferences)
		{
			if (positive)
			{
				arraylistScaleIndex++;
			}
			else
			{
				arraylistScaleIndex--;
			}
		}
		if (arraylistScaleIndex > 20)
		{
			arraylistScaleIndex = 1;
		}
		if (arraylistScaleIndex < 1)
		{
			arraylistScaleIndex = 20;
		}
		Main.arraylistScale = arraylistScaleIndex * 5;
		Buttons.GetIndex("Change Arraylist Scale").overlapText = "Change Arraylist Scale <color=grey>[</color><color=green>" + arraylistScaleIndex + "</color><color=grey>]</color>";
	}

	public static void ChangeOverlayScale(bool positive = true)
	{
		if (!isLoadingPreferences)
		{
			if (positive)
			{
				overlayScaleIndex++;
			}
			else
			{
				overlayScaleIndex--;
			}
		}
		if (overlayScaleIndex > 20)
		{
			overlayScaleIndex = 1;
		}
		if (overlayScaleIndex < 1)
		{
			overlayScaleIndex = 20;
		}
		Main.overlayScale = overlayScaleIndex * 5;
		Buttons.GetIndex("Change Overlay Scale").overlapText = "Change Overlay Scale <color=grey>[</color><color=green>" + overlayScaleIndex + "</color><color=grey>]</color>";
	}

	public static void CMTRed(bool increase = true)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_070a: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0766: Unknown result type (might be due to invalid IL or missing references)
		//IL_0778: Unknown result type (might be due to invalid IL or missing references)
		//IL_0782: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Unknown result type (might be due to invalid IL or missing references)
		//IL_0692: Unknown result type (might be due to invalid IL or missing references)
		//IL_069c: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0861: Unknown result type (might be due to invalid IL or missing references)
		//IL_0873: Unknown result type (might be due to invalid IL or missing references)
		//IL_087d: Unknown result type (might be due to invalid IL or missing references)
		switch (modifyWhatId)
		{
		case 0:
		{
			int num4 = (int)Math.Round(Main.backgroundColor.GetColor(0).r * 10f);
			num4 = ((!increase) ? (num4 - 1) : (num4 + 1));
			num4 %= 11;
			if (num4 < 0)
			{
				num4 = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.backgroundColor.SetColor(0, new Color((float)num4 / 10f, Main.backgroundColor.GetColor(0).g, Main.backgroundColor.GetColor(0).b));
			}
			Buttons.GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + num4 + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.backgroundColor.GetColor(0)) + ">Preview</color>";
			break;
		}
		case 1:
		{
			int num2 = (int)Math.Round(Main.backgroundColor.GetColor(1).r * 10f);
			num2 = ((!increase) ? (num2 - 1) : (num2 + 1));
			num2 %= 11;
			if (num2 < 0)
			{
				num2 = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.backgroundColor.SetColor(1, new Color((float)num2 / 10f, Main.backgroundColor.GetColor(1).g, Main.backgroundColor.GetColor(1).b));
			}
			Buttons.GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + num2 + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.backgroundColor.GetColor(1)) + ">Preview</color>";
			break;
		}
		case 2:
		{
			int num6 = (int)Math.Round(Main.buttonColors[0].GetColor(0).r * 10f);
			num6 = ((!increase) ? (num6 - 1) : (num6 + 1));
			num6 %= 11;
			if (num6 < 0)
			{
				num6 = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.buttonColors[0].SetColor(0, new Color((float)num6 / 10f, Main.buttonColors[0].GetColor(0).g, Main.buttonColors[0].GetColor(0).b));
			}
			Buttons.GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + num6 + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.buttonColors[0].GetColor(0)) + ">Preview</color>";
			break;
		}
		case 3:
		{
			int num3 = (int)Math.Round(Main.buttonColors[0].GetColor(1).r * 10f);
			num3 = ((!increase) ? (num3 - 1) : (num3 + 1));
			num3 %= 11;
			if (num3 < 0)
			{
				num3 = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.buttonColors[0].SetColor(1, new Color((float)num3 / 10f, Main.buttonColors[0].GetColor(1).g, Main.buttonColors[0].GetColor(1).b));
			}
			Buttons.GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + num3 + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.buttonColors[0].GetColor(1)) + ">Preview</color>";
			break;
		}
		case 4:
		{
			int num5 = (int)Math.Round(Main.buttonColors[1].GetColor(0).r * 10f);
			num5 = ((!increase) ? (num5 - 1) : (num5 + 1));
			num5 %= 11;
			if (num5 < 0)
			{
				num5 = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.buttonColors[1].SetColor(0, new Color((float)num5 / 10f, Main.buttonColors[1].GetColor(0).g, Main.buttonColors[1].GetColor(0).b));
			}
			Buttons.GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + num5 + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.buttonColors[1].GetColor(0)) + ">Preview</color>";
			break;
		}
		case 5:
		{
			int num7 = (int)Math.Round(Main.buttonColors[1].GetColor(1).r * 10f);
			num7 = ((!increase) ? (num7 - 1) : (num7 + 1));
			num7 %= 11;
			if (num7 < 0)
			{
				num7 = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.buttonColors[1].SetColor(1, new Color((float)num7 / 10f, Main.buttonColors[1].GetColor(1).g, Main.buttonColors[1].GetColor(1).b));
			}
			Buttons.GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + num7 + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.buttonColors[1].GetColor(1)) + ">Preview</color>";
			break;
		}
		case 6:
		{
			int num8 = (int)Math.Round(Main.textColors[0].GetColor(0).r * 10f);
			num8 = ((!increase) ? (num8 - 1) : (num8 + 1));
			num8 %= 11;
			if (num8 < 0)
			{
				num8 = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.textColors[0].SetColors(new Color((float)num8 / 10f, Main.textColors[0].GetColor(0).g, Main.textColors[0].GetColor(0).b));
			}
			Buttons.GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + num8 + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.textColors[0].GetColor(0)) + ">Preview</color>";
			break;
		}
		case 7:
		{
			int num9 = (int)Math.Round(Main.textColors[1].GetColor(0).r * 10f);
			num9 = ((!increase) ? (num9 - 1) : (num9 + 1));
			num9 %= 11;
			if (num9 < 0)
			{
				num9 = 10;
			}
			Main.textColors[1].SetColors(new Color((float)num9 / 10f, Main.textColors[1].GetColor(0).g, Main.textColors[1].GetColor(0).b));
			Buttons.GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + num9 + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.textColors[1].GetColor(0)) + ">Preview</color>";
			break;
		}
		case 8:
		{
			int num = (int)Math.Round(Main.textColors[2].GetColor(0).r * 10f);
			num = ((!increase) ? (num - 1) : (num + 1));
			num %= 11;
			if (num < 0)
			{
				num = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.textColors[2].SetColors(new Color((float)num / 10f, Main.textColors[2].GetColor(0).g, Main.textColors[2].GetColor(0).b));
			}
			Buttons.GetIndex("Red").overlapText = "Red <color=grey>[</color><color=green>" + num + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.textColors[2].GetColor(0)) + ">Preview</color>";
			break;
		}
		}
		WriteCustomTheme();
	}

	public static void CMTGreen(bool increase = true)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_070a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0805: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0677: Unknown result type (might be due to invalid IL or missing references)
		//IL_0692: Unknown result type (might be due to invalid IL or missing references)
		//IL_069c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0772: Unknown result type (might be due to invalid IL or missing references)
		//IL_078d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0797: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_086d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0888: Unknown result type (might be due to invalid IL or missing references)
		//IL_0892: Unknown result type (might be due to invalid IL or missing references)
		switch (modifyWhatId)
		{
		case 0:
		{
			int num5 = (int)Math.Round(Main.backgroundColor.GetColor(0).g * 10f);
			num5 = ((!increase) ? (num5 - 1) : (num5 + 1));
			num5 %= 11;
			if (num5 < 0)
			{
				num5 = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.backgroundColor.SetColor(0, new Color(Main.backgroundColor.GetColor(0).r, (float)num5 / 10f, Main.backgroundColor.GetColor(0).b));
			}
			Buttons.GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + num5 + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.backgroundColor.GetColor(0)) + ">Preview</color>";
			break;
		}
		case 1:
		{
			int num6 = (int)Math.Round(Main.backgroundColor.GetColor(1).g * 10f);
			num6 = ((!increase) ? (num6 - 1) : (num6 + 1));
			num6 %= 11;
			if (num6 < 0)
			{
				num6 = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.backgroundColor.SetColor(1, new Color(Main.backgroundColor.GetColor(1).r, (float)num6 / 10f, Main.backgroundColor.GetColor(1).b));
			}
			Buttons.GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + num6 + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.backgroundColor.GetColor(1)) + ">Preview</color>";
			break;
		}
		case 2:
		{
			int num8 = (int)Math.Round(Main.buttonColors[0].GetColor(0).g * 10f);
			num8 = ((!increase) ? (num8 - 1) : (num8 + 1));
			num8 %= 11;
			if (num8 < 0)
			{
				num8 = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.buttonColors[0].SetColor(0, new Color(Main.buttonColors[0].GetColor(0).r, (float)num8 / 10f, Main.buttonColors[0].GetColor(0).b));
			}
			Buttons.GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + num8 + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.buttonColors[0].GetColor(0)) + ">Preview</color>";
			break;
		}
		case 3:
		{
			int num2 = (int)Math.Round(Main.buttonColors[0].GetColor(1).g * 10f);
			num2 = ((!increase) ? (num2 - 1) : (num2 + 1));
			num2 %= 11;
			if (num2 < 0)
			{
				num2 = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.buttonColors[0].SetColor(1, new Color(Main.buttonColors[0].GetColor(1).r, (float)num2 / 10f, Main.buttonColors[0].GetColor(1).b));
			}
			Buttons.GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + num2 + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.buttonColors[0].GetColor(1)) + ">Preview</color>";
			break;
		}
		case 4:
		{
			int num7 = (int)Math.Round(Main.buttonColors[1].GetColor(0).g * 10f);
			num7 = ((!increase) ? (num7 - 1) : (num7 + 1));
			num7 %= 11;
			if (num7 < 0)
			{
				num7 = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.buttonColors[1].SetColor(0, new Color(Main.buttonColors[1].GetColor(0).r, (float)num7 / 10f, Main.buttonColors[1].GetColor(0).b));
			}
			Buttons.GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + num7 + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.buttonColors[1].GetColor(0)) + ">Preview</color>";
			break;
		}
		case 5:
		{
			int num4 = (int)Math.Round(Main.buttonColors[1].GetColor(1).g * 10f);
			num4 = ((!increase) ? (num4 - 1) : (num4 + 1));
			num4 %= 11;
			if (num4 < 0)
			{
				num4 = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.buttonColors[1].SetColor(1, new Color(Main.buttonColors[1].GetColor(1).r, (float)num4 / 10f, Main.buttonColors[1].GetColor(1).b));
			}
			Buttons.GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + num4 + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.buttonColors[1].GetColor(1)) + ">Preview</color>";
			break;
		}
		case 6:
		{
			int num3 = (int)Math.Round(Main.textColors[0].GetColor(0).g * 10f);
			num3 = ((!increase) ? (num3 - 1) : (num3 + 1));
			num3 %= 11;
			if (num3 < 0)
			{
				num3 = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.textColors[0].SetColors(new Color(Main.textColors[0].GetColor(0).r, (float)num3 / 10f, Main.textColors[0].GetColor(0).b));
			}
			Buttons.GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + num3 + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.textColors[0].GetColor(0)) + ">Preview</color>";
			break;
		}
		case 7:
		{
			int num9 = (int)Math.Round(Main.textColors[1].GetColor(0).g * 10f);
			num9 = ((!increase) ? (num9 - 1) : (num9 + 1));
			num9 %= 11;
			if (num9 < 0)
			{
				num9 = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.textColors[1].SetColors(new Color(Main.textColors[1].GetColor(0).r, (float)num9 / 10f, Main.textColors[1].GetColor(0).b));
			}
			Buttons.GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + num9 + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.textColors[1].GetColor(0)) + ">Preview</color>";
			break;
		}
		case 8:
		{
			int num = (int)Math.Round(Main.textColors[2].GetColor(0).g * 10f);
			num = ((!increase) ? (num - 1) : (num + 1));
			num %= 11;
			if (num < 0)
			{
				num = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.textColors[2].SetColors(new Color(Main.textColors[2].GetColor(0).r, (float)num / 10f, Main.textColors[2].GetColor(0).b));
			}
			Buttons.GetIndex("Green").overlapText = "Green <color=grey>[</color><color=green>" + num + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.textColors[2].GetColor(0)) + ">Preview</color>";
			break;
		}
		}
		WriteCustomTheme();
	}

	public static void CMTBlue(bool increase = true)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_070a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0805: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0677: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_069c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0772: Unknown result type (might be due to invalid IL or missing references)
		//IL_0784: Unknown result type (might be due to invalid IL or missing references)
		//IL_0797: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_086d: Unknown result type (might be due to invalid IL or missing references)
		//IL_087f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0892: Unknown result type (might be due to invalid IL or missing references)
		switch (modifyWhatId)
		{
		case 0:
		{
			int num5 = (int)Math.Round(Main.backgroundColor.GetColor(0).b * 10f);
			num5 = ((!increase) ? (num5 - 1) : (num5 + 1));
			num5 %= 11;
			if (num5 < 0)
			{
				num5 = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.backgroundColor.SetColor(0, new Color(Main.backgroundColor.GetColor(0).r, Main.backgroundColor.GetColor(0).g, (float)num5 / 10f));
			}
			Buttons.GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + num5 + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.backgroundColor.GetColor(0)) + ">Preview</color>";
			break;
		}
		case 1:
		{
			int num6 = (int)Math.Round(Main.backgroundColor.GetColor(1).b * 10f);
			num6 = ((!increase) ? (num6 - 1) : (num6 + 1));
			num6 %= 11;
			if (num6 < 0)
			{
				num6 = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.backgroundColor.SetColor(1, new Color(Main.backgroundColor.GetColor(1).r, Main.backgroundColor.GetColor(1).g, (float)num6 / 10f));
			}
			Buttons.GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + num6 + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.backgroundColor.GetColor(1)) + ">Preview</color>";
			break;
		}
		case 2:
		{
			int num8 = (int)Math.Round(Main.buttonColors[0].GetColor(0).b * 10f);
			num8 = ((!increase) ? (num8 - 1) : (num8 + 1));
			num8 %= 11;
			if (num8 < 0)
			{
				num8 = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.buttonColors[0].SetColor(0, new Color(Main.buttonColors[0].GetColor(0).r, Main.buttonColors[0].GetColor(0).g, (float)num8 / 10f));
			}
			Buttons.GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + num8 + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.buttonColors[0].GetColor(0)) + ">Preview</color>";
			break;
		}
		case 3:
		{
			int num2 = (int)Math.Round(Main.buttonColors[0].GetColor(1).b * 10f);
			num2 = ((!increase) ? (num2 - 1) : (num2 + 1));
			num2 %= 11;
			if (num2 < 0)
			{
				num2 = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.buttonColors[0].SetColor(1, new Color(Main.buttonColors[0].GetColor(1).r, Main.buttonColors[0].GetColor(1).g, (float)num2 / 10f));
			}
			Buttons.GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + num2 + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.buttonColors[0].GetColor(1)) + ">Preview</color>";
			break;
		}
		case 4:
		{
			int num7 = (int)Math.Round(Main.buttonColors[1].GetColor(0).b * 10f);
			num7 = ((!increase) ? (num7 - 1) : (num7 + 1));
			num7 %= 11;
			if (num7 < 0)
			{
				num7 = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.buttonColors[1].SetColor(0, new Color(Main.buttonColors[1].GetColor(0).r, Main.buttonColors[1].GetColor(0).g, (float)num7 / 10f));
			}
			Buttons.GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + num7 + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.buttonColors[1].GetColor(0)) + ">Preview</color>";
			break;
		}
		case 5:
		{
			int num4 = (int)Math.Round(Main.buttonColors[1].GetColor(1).b * 10f);
			num4 = ((!increase) ? (num4 - 1) : (num4 + 1));
			num4 %= 11;
			if (num4 < 0)
			{
				num4 = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.buttonColors[1].SetColor(1, new Color(Main.buttonColors[1].GetColor(1).r, Main.buttonColors[1].GetColor(1).g, (float)num4 / 10f));
			}
			Buttons.GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + num4 + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.buttonColors[1].GetColor(1)) + ">Preview</color>";
			break;
		}
		case 6:
		{
			int num3 = (int)Math.Round(Main.textColors[0].GetColor(0).b * 10f);
			num3 = ((!increase) ? (num3 - 1) : (num3 + 1));
			num3 %= 11;
			if (num3 < 0)
			{
				num3 = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.textColors[0].SetColors(new Color(Main.textColors[0].GetColor(0).r, Main.textColors[0].GetColor(0).g, (float)num3 / 10f));
			}
			Buttons.GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + num3 + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.textColors[0].GetColor(0)) + ">Preview</color>";
			break;
		}
		case 7:
		{
			int num9 = (int)Math.Round(Main.textColors[1].GetColor(0).b * 10f);
			num9 = ((!increase) ? (num9 - 1) : (num9 + 1));
			num9 %= 11;
			if (num9 < 0)
			{
				num9 = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.textColors[1].SetColors(new Color(Main.textColors[1].GetColor(0).r, Main.textColors[1].GetColor(0).g, (float)num9 / 10f));
			}
			Buttons.GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + num9 + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.textColors[1].GetColor(0)) + ">Preview</color>";
			break;
		}
		case 8:
		{
			int num = (int)Math.Round(Main.textColors[2].GetColor(0).b * 10f);
			num = ((!increase) ? (num - 1) : (num + 1));
			num %= 11;
			if (num < 0)
			{
				num = 10;
			}
			if (Buttons.GetIndex("Custom Menu Theme").enabled)
			{
				Main.textColors[2].SetColors(new Color(Main.textColors[2].GetColor(0).r, Main.textColors[2].GetColor(0).g, (float)num / 10f));
			}
			Buttons.GetIndex("Blue").overlapText = "Blue <color=grey>[</color><color=green>" + num + "</color><color=grey>]</color>";
			Buttons.GetIndex("PreviewLabel").overlapText = "<color=#" + Main.ColorToHex(Main.textColors[2].GetColor(0)) + ">Preview</color>";
			break;
		}
		}
		WriteCustomTheme();
	}

	public static void CustomMenuTheme()
	{
		if (!File.Exists("SeralythMenu/Seralyth_CustomThemeColor.txt"))
		{
			WriteCustomTheme();
		}
		ReadCustomTheme();
	}

	public static void ChangeCustomMenuTheme()
	{
		previousPage = Main.pageNumber;
		CustomMenuThemePage();
	}

	public static void CustomMenuThemePage()
	{
		List<ButtonInfo> list = new List<ButtonInfo>
		{
			new ButtonInfo
			{
				buttonText = "Exit Custom Menu Theme",
				method = delegate
				{
					ExitCustomMenuTheme();
				},
				isTogglable = false,
				toolTip = "Returns you back to the settings menu."
			},
			new ButtonInfo
			{
				buttonText = "Background",
				method = delegate
				{
					CMTBackground();
				},
				isTogglable = false,
				toolTip = "Choose what segment of the background you would like to modify."
			},
			new ButtonInfo
			{
				buttonText = "Buttons",
				method = delegate
				{
					CMTButton();
				},
				isTogglable = false,
				toolTip = "Choose what segment of the button you would like to modify."
			},
			new ButtonInfo
			{
				buttonText = "Text",
				method = delegate
				{
					CMTText();
				},
				isTogglable = false,
				toolTip = "Choose what segment of the text you would like to modify."
			}
		};
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void CMTBackground()
	{
		List<ButtonInfo> list = new List<ButtonInfo>
		{
			new ButtonInfo
			{
				buttonText = "Exit Background",
				method = delegate
				{
					CustomMenuThemePage();
				},
				isTogglable = false,
				toolTip = "Returns you back to the customize menu."
			},
			new ButtonInfo
			{
				buttonText = "First Color",
				method = delegate
				{
					CMTBackgroundFirst();
				},
				isTogglable = false,
				toolTip = "Change the color of the first color of the background."
			},
			new ButtonInfo
			{
				buttonText = "Second Color",
				method = delegate
				{
					CMTBackgroundSecond();
				},
				isTogglable = false,
				toolTip = "Change the color of the second color of the background."
			}
		};
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void CMTBackgroundFirst()
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		modifyWhatId = 0;
		List<ButtonInfo> list = new List<ButtonInfo>
		{
			new ButtonInfo
			{
				buttonText = "Exit First Color",
				method = delegate
				{
					CMTBackground();
				},
				isTogglable = false,
				toolTip = "Returns you back to the background menu."
			},
			new ButtonInfo
			{
				buttonText = "Red",
				overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(Main.backgroundColor.GetColor(0).r * 10f) + "</color><color=grey>]</color>",
				method = delegate
				{
					CMTRed();
				},
				enableMethod = delegate
				{
					CMTRed();
				},
				disableMethod = delegate
				{
					CMTRed(increase: false);
				},
				incremental = true,
				isTogglable = false,
				toolTip = "Change the red of the first color of the background."
			},
			new ButtonInfo
			{
				buttonText = "Green",
				overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(Main.backgroundColor.GetColor(0).g * 10f) + "</color><color=grey>]</color>",
				method = delegate
				{
					CMTGreen();
				},
				enableMethod = delegate
				{
					CMTGreen();
				},
				disableMethod = delegate
				{
					CMTGreen(increase: false);
				},
				incremental = true,
				isTogglable = false,
				toolTip = "Change the green of the first color of the background."
			},
			new ButtonInfo
			{
				buttonText = "Blue",
				overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(Main.backgroundColor.GetColor(0).b * 10f) + "</color><color=grey>]</color>",
				method = delegate
				{
					CMTBlue();
				},
				enableMethod = delegate
				{
					CMTBlue();
				},
				disableMethod = delegate
				{
					CMTBlue(increase: false);
				},
				incremental = true,
				isTogglable = false,
				toolTip = "Change the blue of the first color of the background."
			},
			new ButtonInfo
			{
				buttonText = "PreviewLabel",
				overlapText = "<color=#" + Main.ColorToHex(Main.backgroundColor.GetColor(0)) + ">Preview</color>",
				label = true
			}
		};
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void CMTBackgroundSecond()
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		modifyWhatId = 1;
		List<ButtonInfo> list = new List<ButtonInfo>
		{
			new ButtonInfo
			{
				buttonText = "Exit Second Color",
				method = delegate
				{
					CMTBackground();
				},
				isTogglable = false,
				toolTip = "Returns you back to the background menu."
			},
			new ButtonInfo
			{
				buttonText = "Red",
				overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(Main.backgroundColor.GetColor(1).r * 10f) + "</color><color=grey>]</color>",
				method = delegate
				{
					CMTRed();
				},
				enableMethod = delegate
				{
					CMTRed();
				},
				disableMethod = delegate
				{
					CMTRed(increase: false);
				},
				incremental = true,
				isTogglable = false,
				toolTip = "Change the red of the second color of the background."
			},
			new ButtonInfo
			{
				buttonText = "Green",
				overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(Main.backgroundColor.GetColor(1).g * 10f) + "</color><color=grey>]</color>",
				method = delegate
				{
					CMTGreen();
				},
				enableMethod = delegate
				{
					CMTGreen();
				},
				disableMethod = delegate
				{
					CMTGreen(increase: false);
				},
				incremental = true,
				isTogglable = false,
				toolTip = "Change the green of the second color of the background."
			},
			new ButtonInfo
			{
				buttonText = "Blue",
				overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(Main.backgroundColor.GetColor(1).b * 10f) + "</color><color=grey>]</color>",
				method = delegate
				{
					CMTBlue();
				},
				enableMethod = delegate
				{
					CMTBlue();
				},
				disableMethod = delegate
				{
					CMTBlue(increase: false);
				},
				incremental = true,
				isTogglable = false,
				toolTip = "Change the blue of the second color of the background."
			},
			new ButtonInfo
			{
				buttonText = "PreviewLabel",
				overlapText = "<color=#" + Main.ColorToHex(Main.backgroundColor.GetColor(1)) + ">Preview</color>",
				label = true
			}
		};
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void CMTButton()
	{
		List<ButtonInfo> list = new List<ButtonInfo>();
		list.Add(new ButtonInfo
		{
			buttonText = "Exit Buttons",
			method = CustomMenuThemePage,
			isTogglable = false,
			toolTip = "Returns you back to the customize menu."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Enabled",
			method = CMTButtonEnabled,
			isTogglable = false,
			toolTip = "Choose what type of button color to modify."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Disabled",
			method = CMTButtonDisabled,
			isTogglable = false,
			toolTip = "Change the color of the second color of the background."
		});
		List<ButtonInfo> list2 = list;
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list2.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void CMTButtonEnabled()
	{
		List<ButtonInfo> list = new List<ButtonInfo>();
		list.Add(new ButtonInfo
		{
			buttonText = "Exit Enabled",
			method = CMTButton,
			isTogglable = false,
			toolTip = "Returns you back to the customize menu."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "First Color",
			method = CMTButtonEnabledFirst,
			isTogglable = false,
			toolTip = "Change the color of the first color of the enabled button color."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Second Color",
			method = delegate
			{
				CMTButtonEnabledSecond();
			},
			isTogglable = false,
			toolTip = "Change the color of the second color of the enabled button color."
		});
		List<ButtonInfo> list2 = list;
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list2.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void CMTButtonDisabled()
	{
		List<ButtonInfo> list = new List<ButtonInfo>
		{
			new ButtonInfo
			{
				buttonText = "Exit Enabled",
				method = delegate
				{
					CMTButton();
				},
				isTogglable = false,
				toolTip = "Returns you back to the customize menu."
			},
			new ButtonInfo
			{
				buttonText = "First Color",
				method = delegate
				{
					CMTButtonDisabledFirst();
				},
				isTogglable = false,
				toolTip = "Change the color of the first color of the disabled button color."
			},
			new ButtonInfo
			{
				buttonText = "Second Color",
				method = delegate
				{
					CMTButtonDisabledSecond();
				},
				isTogglable = false,
				toolTip = "Change the color of the second color of the disabled button color."
			}
		};
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void CMTButtonEnabledFirst()
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		modifyWhatId = 4;
		List<ButtonInfo> list = new List<ButtonInfo>
		{
			new ButtonInfo
			{
				buttonText = "Exit First Color",
				method = delegate
				{
					CMTButtonEnabled();
				},
				isTogglable = false,
				toolTip = "Returns you back to the enabled button menu."
			},
			new ButtonInfo
			{
				buttonText = "Red",
				overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(Main.buttonColors[1].GetColor(0).r * 10f) + "</color><color=grey>]</color>",
				method = delegate
				{
					CMTRed();
				},
				enableMethod = delegate
				{
					CMTRed();
				},
				disableMethod = delegate
				{
					CMTRed(increase: false);
				},
				incremental = true,
				isTogglable = false,
				toolTip = "Change the red of the first color of the enabled button color."
			},
			new ButtonInfo
			{
				buttonText = "Green",
				overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(Main.buttonColors[1].GetColor(0).g * 10f) + "</color><color=grey>]</color>",
				method = delegate
				{
					CMTGreen();
				},
				enableMethod = delegate
				{
					CMTGreen();
				},
				disableMethod = delegate
				{
					CMTGreen(increase: false);
				},
				incremental = true,
				isTogglable = false,
				toolTip = "Change the green of the first color of the enabled button color."
			},
			new ButtonInfo
			{
				buttonText = "Blue",
				overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(Main.buttonColors[1].GetColor(0).b * 10f) + "</color><color=grey>]</color>",
				method = delegate
				{
					CMTBlue();
				},
				enableMethod = delegate
				{
					CMTBlue();
				},
				disableMethod = delegate
				{
					CMTBlue(increase: false);
				},
				incremental = true,
				isTogglable = false,
				toolTip = "Change the blue of the first color of the enabled button color."
			},
			new ButtonInfo
			{
				buttonText = "PreviewLabel",
				overlapText = "<color=#" + Main.ColorToHex(Main.buttonColors[1].GetColor(0)) + ">Preview</color>",
				label = true
			}
		};
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void CMTButtonEnabledSecond()
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		modifyWhatId = 5;
		List<ButtonInfo> list = new List<ButtonInfo>
		{
			new ButtonInfo
			{
				buttonText = "Exit Second Color",
				method = delegate
				{
					CMTButtonEnabled();
				},
				isTogglable = false,
				toolTip = "Returns you back to the enabled button menu."
			},
			new ButtonInfo
			{
				buttonText = "Red",
				overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(Main.buttonColors[1].GetColor(1).r * 10f) + "</color><color=grey>]</color>",
				method = delegate
				{
					CMTRed();
				},
				enableMethod = delegate
				{
					CMTRed();
				},
				disableMethod = delegate
				{
					CMTRed(increase: false);
				},
				incremental = true,
				isTogglable = false,
				toolTip = "Change the red of the first color of the enabled button color."
			},
			new ButtonInfo
			{
				buttonText = "Green",
				overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(Main.buttonColors[1].GetColor(1).g * 10f) + "</color><color=grey>]</color>",
				method = delegate
				{
					CMTGreen();
				},
				enableMethod = delegate
				{
					CMTGreen();
				},
				disableMethod = delegate
				{
					CMTGreen(increase: false);
				},
				incremental = true,
				isTogglable = false,
				toolTip = "Change the green of the first color of the enabled button color."
			},
			new ButtonInfo
			{
				buttonText = "Blue",
				overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(Main.buttonColors[1].GetColor(1).b * 10f) + "</color><color=grey>]</color>",
				method = delegate
				{
					CMTBlue();
				},
				enableMethod = delegate
				{
					CMTBlue();
				},
				disableMethod = delegate
				{
					CMTBlue(increase: false);
				},
				incremental = true,
				isTogglable = false,
				toolTip = "Change the blue of the first color of the enabled button color."
			},
			new ButtonInfo
			{
				buttonText = "PreviewLabel",
				overlapText = "<color=#" + Main.ColorToHex(Main.buttonColors[1].GetColor(1)) + ">Preview</color>",
				label = true
			}
		};
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void CMTButtonDisabledFirst()
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		modifyWhatId = 2;
		List<ButtonInfo> list = new List<ButtonInfo>
		{
			new ButtonInfo
			{
				buttonText = "Exit First Color",
				method = delegate
				{
					CMTButtonDisabled();
				},
				isTogglable = false,
				toolTip = "Returns you back to the disabled button menu."
			},
			new ButtonInfo
			{
				buttonText = "Red",
				overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(Main.buttonColors[0].GetColor(0).r * 10f) + "</color><color=grey>]</color>",
				method = delegate
				{
					CMTRed();
				},
				enableMethod = delegate
				{
					CMTRed();
				},
				disableMethod = delegate
				{
					CMTRed(increase: false);
				},
				incremental = true,
				isTogglable = false,
				toolTip = "Change the red of the first color of the disabled button color."
			},
			new ButtonInfo
			{
				buttonText = "Green",
				overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(Main.buttonColors[0].GetColor(0).g * 10f) + "</color><color=grey>]</color>",
				method = delegate
				{
					CMTGreen();
				},
				enableMethod = delegate
				{
					CMTGreen();
				},
				disableMethod = delegate
				{
					CMTGreen(increase: false);
				},
				incremental = true,
				isTogglable = false,
				toolTip = "Change the green of the first color of the disabled button color."
			},
			new ButtonInfo
			{
				buttonText = "Blue",
				overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(Main.buttonColors[0].GetColor(0).b * 10f) + "</color><color=grey>]</color>",
				method = delegate
				{
					CMTBlue();
				},
				enableMethod = delegate
				{
					CMTBlue();
				},
				disableMethod = delegate
				{
					CMTBlue(increase: false);
				},
				incremental = true,
				isTogglable = false,
				toolTip = "Change the blue of the first color of the disabled button color."
			},
			new ButtonInfo
			{
				buttonText = "PreviewLabel",
				overlapText = "<color=#" + Main.ColorToHex(Main.buttonColors[0].GetColor(0)) + ">Preview</color>",
				label = true
			}
		};
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void CMTButtonDisabledSecond()
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		modifyWhatId = 3;
		List<ButtonInfo> list = new List<ButtonInfo>();
		list.Add(new ButtonInfo
		{
			buttonText = "Exit Second Color",
			method = CMTButtonDisabled,
			isTogglable = false,
			toolTip = "Returns you back to the disabled button menu."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Red",
			overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(Main.buttonColors[0].GetColor(1).r * 10f) + "</color><color=grey>]</color>",
			method = delegate
			{
				CMTRed();
			},
			enableMethod = delegate
			{
				CMTRed();
			},
			disableMethod = delegate
			{
				CMTRed(increase: false);
			},
			incremental = true,
			isTogglable = false,
			toolTip = "Change the red of the first color of the disabled button color."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Green",
			overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(Main.buttonColors[0].GetColor(1).g * 10f) + "</color><color=grey>]</color>",
			method = delegate
			{
				CMTGreen();
			},
			enableMethod = delegate
			{
				CMTGreen();
			},
			disableMethod = delegate
			{
				CMTGreen(increase: false);
			},
			incremental = true,
			isTogglable = false,
			toolTip = "Change the green of the first color of the disabled button color."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Blue",
			overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(Main.buttonColors[0].GetColor(1).b * 10f) + "</color><color=grey>]</color>",
			method = delegate
			{
				CMTBlue();
			},
			enableMethod = delegate
			{
				CMTBlue();
			},
			disableMethod = delegate
			{
				CMTBlue(increase: false);
			},
			incremental = true,
			isTogglable = false,
			toolTip = "Change the blue of the first color of the disabled button color."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "PreviewLabel",
			overlapText = "<color=#" + Main.ColorToHex(Main.buttonColors[0].GetColor(1)) + ">Preview</color>",
			label = true
		});
		List<ButtonInfo> list2 = list;
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list2.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void CMTText()
	{
		List<ButtonInfo> list = new List<ButtonInfo>();
		list.Add(new ButtonInfo
		{
			buttonText = "Exit Text",
			method = CustomMenuThemePage,
			isTogglable = false,
			toolTip = "Returns you back to the customize menu."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Title",
			method = CMTTextTitle,
			isTogglable = false,
			toolTip = "Change the color of the title."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Enabled",
			method = CMTTextEnabled,
			isTogglable = false,
			toolTip = "Change the color of the enabled text."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Disabled",
			method = CMTTextDisabled,
			isTogglable = false,
			toolTip = "Change the color of the disabled text."
		});
		List<ButtonInfo> list2 = list;
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list2.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void CMTTextTitle()
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		modifyWhatId = 6;
		List<ButtonInfo> list = new List<ButtonInfo>();
		list.Add(new ButtonInfo
		{
			buttonText = "Exit Title",
			method = CMTText,
			isTogglable = false,
			toolTip = "Returns you back to the text menu."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Red",
			overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(Main.textColors[0].GetColor(0).r * 10f) + "</color><color=grey>]</color>",
			method = delegate
			{
				CMTRed();
			},
			enableMethod = delegate
			{
				CMTRed();
			},
			disableMethod = delegate
			{
				CMTRed(increase: false);
			},
			incremental = true,
			isTogglable = false,
			toolTip = "Change the red of the title color."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Green",
			overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(Main.textColors[0].GetColor(0).g * 10f) + "</color><color=grey>]</color>",
			method = delegate
			{
				CMTGreen();
			},
			enableMethod = delegate
			{
				CMTGreen();
			},
			disableMethod = delegate
			{
				CMTGreen(increase: false);
			},
			incremental = true,
			isTogglable = false,
			toolTip = "Change the green of the title color."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "Blue",
			overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(Main.textColors[0].GetColor(0).b * 10f) + "</color><color=grey>]</color>",
			method = delegate
			{
				CMTBlue();
			},
			enableMethod = delegate
			{
				CMTBlue();
			},
			disableMethod = delegate
			{
				CMTBlue(increase: false);
			},
			incremental = true,
			isTogglable = false,
			toolTip = "Change the blue of the title color."
		});
		list.Add(new ButtonInfo
		{
			buttonText = "PreviewLabel",
			overlapText = "<color=#" + Main.ColorToHex(Main.textColors[0].GetColor(0)) + ">Preview</color>",
			label = true
		});
		List<ButtonInfo> list2 = list;
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list2.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void CMTTextEnabled()
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		modifyWhatId = 8;
		List<ButtonInfo> list = new List<ButtonInfo>
		{
			new ButtonInfo
			{
				buttonText = "Exit Second Color",
				method = delegate
				{
					CMTText();
				},
				isTogglable = false,
				toolTip = "Returns you back to the text menu."
			},
			new ButtonInfo
			{
				buttonText = "Red",
				overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(Main.textColors[2].GetColor(0).r * 10f) + "</color><color=grey>]</color>",
				method = delegate
				{
					CMTRed();
				},
				enableMethod = delegate
				{
					CMTRed();
				},
				disableMethod = delegate
				{
					CMTRed(increase: false);
				},
				incremental = true,
				isTogglable = false,
				toolTip = "Change the red of the enabled text color."
			},
			new ButtonInfo
			{
				buttonText = "Green",
				overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(Main.textColors[2].GetColor(0).g * 10f) + "</color><color=grey>]</color>",
				method = delegate
				{
					CMTGreen();
				},
				enableMethod = delegate
				{
					CMTGreen();
				},
				disableMethod = delegate
				{
					CMTGreen(increase: false);
				},
				incremental = true,
				isTogglable = false,
				toolTip = "Change the green of the enabled text color."
			},
			new ButtonInfo
			{
				buttonText = "Blue",
				overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(Main.textColors[2].GetColor(0).b * 10f) + "</color><color=grey>]</color>",
				method = delegate
				{
					CMTBlue();
				},
				enableMethod = delegate
				{
					CMTBlue();
				},
				disableMethod = delegate
				{
					CMTBlue(increase: false);
				},
				incremental = true,
				isTogglable = false,
				toolTip = "Change the blue of the enabled text color."
			},
			new ButtonInfo
			{
				buttonText = "PreviewLabel",
				overlapText = "<color=#" + Main.ColorToHex(Main.textColors[2].GetColor(0)) + ">Preview</color>",
				label = true
			}
		};
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void CMTTextDisabled()
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		modifyWhatId = 7;
		List<ButtonInfo> list = new List<ButtonInfo>
		{
			new ButtonInfo
			{
				buttonText = "Exit Second Color",
				method = delegate
				{
					CMTText();
				},
				isTogglable = false,
				toolTip = "Returns you back to the text menu."
			},
			new ButtonInfo
			{
				buttonText = "Red",
				overlapText = "Red <color=grey>[</color><color=green>" + (int)Math.Round(Main.textColors[1].GetColor(0).r * 10f) + "</color><color=grey>]</color>",
				method = delegate
				{
					CMTRed();
				},
				enableMethod = delegate
				{
					CMTRed();
				},
				disableMethod = delegate
				{
					CMTRed(increase: false);
				},
				incremental = true,
				isTogglable = false,
				toolTip = "Change the red of the disabled text color."
			},
			new ButtonInfo
			{
				buttonText = "Green",
				overlapText = "Green <color=grey>[</color><color=green>" + (int)Math.Round(Main.textColors[1].GetColor(0).g * 10f) + "</color><color=grey>]</color>",
				method = delegate
				{
					CMTGreen();
				},
				enableMethod = delegate
				{
					CMTGreen();
				},
				disableMethod = delegate
				{
					CMTGreen(increase: false);
				},
				incremental = true,
				isTogglable = false,
				toolTip = "Change the green of the disabled text color."
			},
			new ButtonInfo
			{
				buttonText = "Blue",
				overlapText = "Blue <color=grey>[</color><color=green>" + (int)Math.Round(Main.textColors[1].GetColor(0).b * 10f) + "</color><color=grey>]</color>",
				method = delegate
				{
					CMTBlue();
				},
				enableMethod = delegate
				{
					CMTBlue();
				},
				disableMethod = delegate
				{
					CMTBlue(increase: false);
				},
				incremental = true,
				isTogglable = false,
				toolTip = "Change the blue of the disabled text color."
			},
			new ButtonInfo
			{
				buttonText = "PreviewLabel",
				overlapText = "<color=#" + Main.ColorToHex(Main.textColors[1].GetColor(0)) + ">Preview</color>",
				label = true
			}
		};
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}

	public static void ExitCustomMenuTheme()
	{
		Main.pageNumber = previousPage;
		Buttons.CurrentCategoryName = "Menu Settings";
	}

	public static void ReadCustomTheme()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		string[] array = File.ReadAllText("SeralythMenu/Seralyth_CustomThemeColor.txt").Split("\n");
		string[] array2 = array[0].Split(",");
		Main.backgroundColor.SetColor(0, Color32.op_Implicit(new Color32(byte.Parse(array2[0]), byte.Parse(array2[1]), byte.Parse(array2[2]), byte.MaxValue)));
		array2 = array[1].Split(",");
		Main.backgroundColor.SetColor(1, Color32.op_Implicit(new Color32(byte.Parse(array2[0]), byte.Parse(array2[1]), byte.Parse(array2[2]), byte.MaxValue)));
		array2 = array[2].Split(",");
		Main.buttonColors[0].SetColor(0, Color32.op_Implicit(new Color32(byte.Parse(array2[0]), byte.Parse(array2[1]), byte.Parse(array2[2]), byte.MaxValue)));
		array2 = array[3].Split(",");
		Main.buttonColors[0].SetColor(1, Color32.op_Implicit(new Color32(byte.Parse(array2[0]), byte.Parse(array2[1]), byte.Parse(array2[2]), byte.MaxValue)));
		array2 = array[4].Split(",");
		Main.buttonColors[1].SetColor(0, Color32.op_Implicit(new Color32(byte.Parse(array2[0]), byte.Parse(array2[1]), byte.Parse(array2[2]), byte.MaxValue)));
		array2 = array[5].Split(",");
		Main.buttonColors[1].SetColor(1, Color32.op_Implicit(new Color32(byte.Parse(array2[0]), byte.Parse(array2[1]), byte.Parse(array2[2]), byte.MaxValue)));
		array2 = array[6].Split(",");
		Main.textColors[0].SetColors(Color32.op_Implicit(new Color32(byte.Parse(array2[0]), byte.Parse(array2[1]), byte.Parse(array2[2]), byte.MaxValue)));
		array2 = array[7].Split(",");
		Main.textColors[1].SetColors(Color32.op_Implicit(new Color32(byte.Parse(array2[0]), byte.Parse(array2[1]), byte.Parse(array2[2]), byte.MaxValue)));
		array2 = array[8].Split(",");
		Main.textColors[2].SetColors(Color32.op_Implicit(new Color32(byte.Parse(array2[0]), byte.Parse(array2[1]), byte.Parse(array2[2]), byte.MaxValue)));
	}

	public static void ImportCustomTheme(string theme)
	{
		File.WriteAllText("SeralythMenu/Seralyth_CustomThemeColor.txt", theme);
		ReadCustomTheme();
	}

	public static string ExportCustomTheme()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		Color[] array = (Color[])(object)new Color[9]
		{
			Main.backgroundColor.GetColor(0),
			Main.backgroundColor.GetColor(1),
			Main.buttonColors[0].GetColor(0),
			Main.buttonColors[0].GetColor(1),
			Main.buttonColors[1].GetColor(0),
			Main.buttonColors[1].GetColor(1),
			Main.textColors[0].GetColor(0),
			Main.textColors[1].GetColor(0),
			Main.textColors[2].GetColor(0)
		};
		string text = "";
		Color[] array2 = array;
		foreach (Color val in array2)
		{
			if (text != "")
			{
				text += "\n";
			}
			text = text + Math.Round(Mathf.Round(val.r * 10f) / 10f * 255f) + "," + Math.Round(Mathf.Round(val.g * 10f) / 10f * 255f) + "," + Math.Round(Mathf.Round(val.b * 10f) / 10f * 255f);
		}
		return text;
	}

	public static void WriteCustomTheme()
	{
		File.WriteAllText("SeralythMenu/Seralyth_CustomThemeColor.txt", ExportCustomTheme());
	}

	public static void FixTheme()
	{
		Main.themeType--;
		ChangeMenuTheme();
	}

	public static void CustomMenuBackground()
	{
		if (!File.Exists("SeralythMenu/CustomBackground.png"))
		{
			AssetUtilities.LoadTextureFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Images/CustomBackground.png", "CustomBackground.png");
		}
		AssetUtilities.textureFileDirectory.Remove("CustomBackground.png");
		Main.doCustomMenuBackground = true;
		Main.customMenuBackgroundImage = AssetUtilities.LoadTextureFromFile("CustomBackground.png");
	}

	public static void FixMenuBackground()
	{
		Main.customMenuBackgroundImage = null;
		Main.doCustomMenuBackground = false;
	}

	public static void EnableWatermark()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		if (Buttons.GetIndex("Custom Watermark").enabled)
		{
			if (!File.Exists("SeralythMenu/CustomWatermark.png"))
			{
				AssetUtilities.LoadTextureFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Images/CustomWatermark.png", "CustomWatermark.png");
			}
			AssetUtilities.textureFileDirectory.Remove("CustomWatermark.png");
			Main.customWatermark = AssetUtilities.LoadTextureFromFile("CustomWatermark.png");
			return;
		}
		GameObject val = new GameObject();
		val.transform.parent = Main.canvasObj.transform;
		Main.watermarkImage = val.AddComponent<Image>();
		if ((Object)(object)Main.watermarkMat == (Object)null)
		{
			Main.watermarkMat = new Material(((Graphic)Main.watermarkImage).material);
		}
		((Graphic)Main.watermarkImage).material = Main.watermarkMat;
		((Graphic)Main.watermarkImage).material.SetTexture("_MainTex", (Texture)(object)(Main.customWatermark ?? AssetUtilities.LoadTextureFromResource("SeralythMenu.Resources.Client.icon.png")));
	}

	public static void CustomWatermark()
	{
		if (!File.Exists("SeralythMenu/CustomWatermark.png"))
		{
			AssetUtilities.LoadTextureFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Images/CustomWatermark.png", "CustomWatermark.png");
		}
		AssetUtilities.textureFileDirectory.Remove("CustomWatermark.png");
		Main.customWatermark = AssetUtilities.LoadTextureFromFile("CustomWatermark.png");
	}

	public static void CustomFontType()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		string text = "SeralythMenu/CustomFont.ttf";
		if (!File.Exists(text))
		{
			LogManager.Log("Downloading CustomFont.ttf");
			WebClient webClient = new WebClient();
			webClient.DownloadFile("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Fonts/LiberationSans.ttf", text);
		}
		chosenFont = TMP_FontAsset.CreateFontAsset(new Font(FileUtilities.GetGamePath() + "/" + text));
		PersistCustomFont();
	}

	public static void PersistCustomFont()
	{
		if ((Object)(object)Main.activeFont != (Object)(object)chosenFont)
		{
			Main.activeFont = chosenFont;
		}
	}

	public static void DisableCustomFont()
	{
		Main.fontCycle--;
		ChangeFontType();
	}

	public static void ChangePageType(bool positive = true)
	{
		if (!isLoadingPreferences)
		{
			if (positive)
			{
				Main.pageButtonType++;
			}
			else
			{
				Main.pageButtonType--;
			}
		}
		if (Main.pageButtonType > 6)
		{
			Main.pageButtonType = 1;
		}
		if (Main.pageButtonType < 1)
		{
			Main.pageButtonType = 6;
		}
		Main.buttonOffset = ((Main.pageButtonType == 2) ? 2 : 0);
	}

	public static void ChangePageSize(bool positive = true)
	{
		if (!isLoadingPreferences)
		{
			if (positive)
			{
				Main._pageSize++;
			}
			else
			{
				Main._pageSize--;
			}
		}
		if (Main._pageSize > 16)
		{
			Main._pageSize = 4;
		}
		if (Main._pageSize < 4)
		{
			Main._pageSize = 16;
		}
		Buttons.GetIndex("Change Page Size").overlapText = $"Change Page Size <color=grey>[</color><color=green>{Main._pageSize}</color><color=grey>]</color>";
	}

	public static void ChangeCharacterDistance(bool positive = true)
	{
		if (!isLoadingPreferences)
		{
			if (positive)
			{
				Main.characterDistance++;
			}
			else
			{
				Main.characterDistance--;
			}
		}
		if (Main.characterDistance > 15)
		{
			Main.characterDistance = 0;
		}
		if (Main.characterDistance < 0)
		{
			Main.characterDistance = 15;
		}
		Buttons.GetIndex("Change Character Distance").overlapText = $"Change Character Distance <color=grey>[</color><color=green>{Main.characterDistance + 1}</color><color=grey>]</color>";
	}

	public static void ChangeArrowType(bool positive = true)
	{
		if (!isLoadingPreferences)
		{
			if (positive)
			{
				Main.arrowType++;
			}
			else
			{
				Main.arrowType--;
			}
		}
		Main.arrowType %= Main.arrowTypes.Length;
		if (Main.arrowType < 0)
		{
			Main.arrowType = Main.arrowTypes.Length - 1;
		}
	}

	public static void ChangeFontType(bool positive = true)
	{
		if (!isLoadingPreferences)
		{
			if (positive)
			{
				Main.fontCycle++;
			}
			else
			{
				Main.fontCycle--;
			}
		}
		Main.fontCycle %= 15;
		if (Main.fontCycle < 0)
		{
			Main.fontCycle = 14;
		}
		switch (Main.fontCycle)
		{
		case 0:
			Main.activeFont = Main.AgencyFB;
			break;
		case 1:
			Main.activeFont = Main.FreeSans;
			break;
		case 2:
			Main.activeFont = Main.DejaVuSans;
			break;
		case 3:
			Main.activeFont = Main.Utopium;
			break;
		case 4:
			Main.activeFont = Main.ComicSans;
			break;
		case 5:
			Main.activeFont = Main.CascadiaMono;
			break;
		case 6:
			Main.activeFont = Main.Candara;
			break;
		case 7:
			Main.activeFont = Main.MSGothic;
			break;
		case 8:
			Main.activeFont = Main.Anton;
			break;
		case 9:
			Main.activeFont = Main.SimSun;
			break;
		case 10:
			Main.activeFont = Main.Minecraft;
			break;
		case 11:
			Main.activeFont = Main.Terminal;
			break;
		case 12:
			Main.activeFont = Main.OpenDyslexic;
			break;
		case 13:
			Main.activeFont = Main.Taiko;
			break;
		case 14:
			Main.activeFont = Main.LiberationSans;
			break;
		}
	}

	public static void ChangeFontRapid()
	{
		if (Time.time > fontTime)
		{
			ChangeFontType();
			fontTime = Time.time + 0.4f;
			Main.ReloadMenu();
		}
	}

	public static void ChangeFontStyleType(bool positive = true)
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		if (!isLoadingPreferences)
		{
			if (positive)
			{
				fontStyleType++;
			}
			else
			{
				fontStyleType--;
			}
		}
		fontStyleType %= 4;
		if (fontStyleType < 0)
		{
			fontStyleType = 3;
		}
		int num = fontStyleType;
		if (1 == 0)
		{
		}
		FontStyles activeFontStyle = (FontStyles)(num switch
		{
			0 => 0, 
			1 => 1, 
			2 => 2, 
			3 => 3, 
			_ => 0, 
		});
		if (1 == 0)
		{
		}
		Main.activeFontStyle = activeFontStyle;
	}

	public static void ChangeInputTextColor(bool positive = true)
	{
		string[] array = new string[12]
		{
			"Red", "Orange", "Yellow", "Green", "Blue", "Cyan", "Purple", "Pink", "White", "Grey",
			"Black", "Rose"
		};
		string[] array2 = new string[12]
		{
			"red", "#ff8000", "yellow", "green", "blue", "#00FFFF", "purple", "#FF00FF", "white", "grey",
			"black", "#ff005d"
		};
		if (!isLoadingPreferences)
		{
			if (positive)
			{
				inputTextColorInt++;
			}
			else
			{
				inputTextColorInt--;
			}
		}
		inputTextColorInt %= array2.Length;
		if (inputTextColorInt < 0)
		{
			inputTextColorInt = array2.Length - 1;
		}
		Main.inputTextColor = array2[inputTextColorInt];
		Buttons.GetIndex("Change Input Text Color").overlapText = "Change Input Text Color <color=grey>[</color><color=green>" + array[inputTextColorInt] + "</color><color=grey>]</color>";
	}

	public static void ChangePCUI(bool positive = true)
	{
		if (!isLoadingPreferences)
		{
			if (positive)
			{
				Main.pcbg++;
			}
			else
			{
				Main.pcbg--;
			}
		}
		Main.pcbg %= 6;
		if (Main.pcbg < 0)
		{
			Main.pcbg = 5;
		}
	}

	public static void ChangeJoystickMenuPosition(bool positive = true)
	{
		if (!isLoadingPreferences)
		{
			if (positive)
			{
				Main.joystickMenuPosition++;
			}
			else
			{
				Main.joystickMenuPosition--;
			}
		}
		Main.joystickMenuPosition %= Main.joystickMenuPositions.Length;
		if (Main.joystickMenuPosition < 0)
		{
			Main.joystickMenuPosition = Main.joystickMenuPositions.Length - 1;
		}
	}

	public static void ChangeNotificationTime(bool positive = true)
	{
		if (!isLoadingPreferences)
		{
			if (positive)
			{
				Main.notificationDecayTime += 1000;
			}
			else
			{
				Main.notificationDecayTime -= 1000;
			}
		}
		Main.notificationDecayTime %= 6000;
		if (Main.notificationDecayTime < 0)
		{
			Main.notificationDecayTime = 5000;
		}
		Buttons.GetIndex("Change Notification Time").overlapText = "Change Notification Time <color=grey>[</color><color=green>" + Main.notificationDecayTime / 1000 + "</color><color=grey>]</color>";
	}

	public static void ChangeNotificationSound(bool positive = true, bool fromMenu = false)
	{
		string[] array = SoundManager.Sounds["Notifications"].Keys.ToArray();
		string value = SoundManager.DefaultSounds["Notification"];
		int num = Array.IndexOf(array, value);
		if (num < 0)
		{
			num = 0;
		}
		num = (positive ? (num + 1) : (num - 1));
		if (num >= array.Length)
		{
			num = 0;
		}
		if (num < 0)
		{
			num = array.Length - 1;
		}
		string text = array[num];
		SoundManager.DefaultSounds["Notification"] = text;
		Buttons.GetIndex("Change Notification Sound").overlapText = "Change Notification Sound <color=grey>[</color><color=green>" + text + "</color><color=grey>]</color>";
		if (fromMenu)
		{
			GameObject audioManager = Main.audioManager;
			AudioSource val = ((audioManager != null) ? audioManager.GetComponent<AudioSource>() : null);
			if (val != null)
			{
				val.Stop();
			}
			SoundManager.Play(SoundManager.DefaultSounds["Notification"]);
		}
	}

	public static void ChangeNarrationVoice(bool positive = true)
	{
		string[] array = new string[30]
		{
			"Default", "Kimberly", "Brian", "Matthew", "Joey", "Justin", "Cristiano", "Giorgio", "Ewa", "TikTok",
			"Grandma", "Trickster", "Elf", "Ghostface", "Zombie", "Narrator", "Pirate", "Song", "TikTok Joey", "Gingerbread Man",
			"Chris", "Thanksgiving", "Santa", "Google US", "Google UK", "Dog", "Jerkface", "Robot", "Vlad", "Obama"
		};
		if (!isLoadingPreferences)
		{
			if (positive)
			{
				Main.narratorIndex++;
			}
			else
			{
				Main.narratorIndex--;
			}
		}
		Main.narratorIndex %= array.Length;
		if (Main.narratorIndex < 0)
		{
			Main.narratorIndex = array.Length - 1;
		}
		Buttons.GetIndex("Change Narration Voice").overlapText = "Change Narration Voice <color=grey>[</color><color=green>" + array[Main.narratorIndex] + "</color><color=grey>]</color>";
		Main.narratorName = array[Main.narratorIndex];
		if (krec != null && ((PhraseRecognizer)krec).IsRunning && Time.time > dRestartTime)
		{
			DictationRestart();
			dRestartTime = Time.time + 1f;
		}
	}

	public static void KickToSpecificRoom()
	{
		if (Time.time < Main.timeMenuStarted + 5f)
		{
			Buttons.GetIndex("Kick to Specific Room").enabled = false;
			return;
		}
		Main.PromptText("What would you like the room code to be?", delegate
		{
			Overpowered.specificRoom = Main.keyboardInput.ToUpper();
		}, delegate
		{
			Main.Toggle("Kick to Specific Room");
		}, "Done", "Cancel");
	}

	public static void ChangePointerPosition(bool positive = true)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] array = (Vector3[])(object)new Vector3[4]
		{
			new Vector3(0f, -0.1f, 0f),
			new Vector3(0f, -0.1f, -0.15f),
			new Vector3(0f, 0.1f, -0.05f),
			new Vector3(0f, 0.0666f, 0.1f)
		};
		if (!isLoadingPreferences)
		{
			if (positive)
			{
				Main.pointerIndex++;
			}
			else
			{
				Main.pointerIndex--;
			}
		}
		Main.pointerIndex %= array.Length;
		if (Main.pointerIndex < 0)
		{
			Main.pointerIndex = array.Length - 1;
		}
		Main.pointerOffset = array[Main.pointerIndex];
		try
		{
			Main.reference.transform.localPosition = Main.pointerOffset;
		}
		catch
		{
		}
	}

	public static void ChangeGunVariation(bool positive = true)
	{
		string[] array = new string[10] { "Default", "Lightning", "Wavy", "Blocky", "Zigzag", "Spring", "Bouncy", "Audio", "Bezier", "Rope" };
		if (!isLoadingPreferences)
		{
			if (positive)
			{
				Main.gunVariation++;
			}
			else
			{
				Main.gunVariation--;
			}
		}
		Main.gunVariation %= array.Length;
		if (Main.gunVariation < 0)
		{
			Main.gunVariation = array.Length - 1;
		}
		Buttons.GetIndex("Change Gun Variation").overlapText = "Change Gun Variation <color=grey>[</color><color=green>" + array[Main.gunVariation] + "</color><color=grey>]</color>";
	}

	public static void ChangeGunDirection(bool positive = true)
	{
		string[] array = new string[5] { "Default", "Legacy", "Laser", "Finger", "Face" };
		if (!isLoadingPreferences)
		{
			if (positive)
			{
				Main.GunDirection++;
			}
			else
			{
				Main.GunDirection--;
			}
		}
		Main.GunDirection %= array.Length;
		if (Main.GunDirection < 0)
		{
			Main.GunDirection = array.Length - 1;
		}
		Buttons.GetIndex("Change Gun Direction").overlapText = "Change Gun Direction <color=grey>[</color><color=green>" + array[Main.GunDirection] + "</color><color=grey>]</color>";
	}

	public static void ChangeGunLineQuality(bool positive = true)
	{
		string[] array = new string[5] { "Potato", "Low", "Normal", "High", "Extreme" };
		int[] array2 = new int[5] { 10, 25, 50, 100, 250 };
		if (!isLoadingPreferences)
		{
			if (positive)
			{
				gunLineQualityIndex++;
			}
			else
			{
				gunLineQualityIndex--;
			}
		}
		gunLineQualityIndex %= array.Length;
		if (gunLineQualityIndex < 0)
		{
			gunLineQualityIndex = array.Length - 1;
		}
		Main.GunLineQuality = array2[gunLineQualityIndex];
		Buttons.GetIndex("Change Gun Line Quality").overlapText = "Change Gun Line Quality <color=grey>[</color><color=green>" + array[gunLineQualityIndex] + "</color><color=grey>]</color>";
	}

	public static void ChangeGunLibShape(bool positive = true)
	{
		if (!isLoadingPreferences)
		{
			if (positive)
			{
				Main.GunLibShape++;
			}
			else
			{
				Main.GunLibShape--;
			}
		}
		Main.GunLibShape %= GunLibShapeNames.Length;
		if (Main.GunLibShape < 0)
		{
			Main.GunLibShape = GunLibShapeNames.Length - 1;
		}
		Buttons.GetIndex("Change GunLib Shape").overlapText = "Change GunLib Shape <color=grey>[</color><color=green>" + GunLibShapeNames[Main.GunLibShape] + "</color><color=grey>]</color>";
	}

	public static void FreezePlayerInMenu()
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (Main.physicalMenu ? Main.isMenuButtonHeld : ((Object)(object)Main.menu != (Object)null))
		{
			if (Main.closePosition == Vector3.zero)
			{
				Main.closePosition = ((Component)GorillaTagger.Instance.rigidbody).transform.position;
			}
			else
			{
				((Component)GorillaTagger.Instance.rigidbody).transform.position = Main.closePosition;
			}
			GorillaTagger.Instance.rigidbody.linearVelocity = new Vector3(0f, 0f, 0f);
		}
		else
		{
			Main.closePosition = Vector3.zero;
		}
	}

	public static void FreezeRigInMenu()
	{
		if ((Object)(object)Main.menu != (Object)null)
		{
			if (!currentmentalstate)
			{
				currentmentalstate = true;
				((Behaviour)VRRig.LocalRig).enabled = false;
			}
		}
		else if (currentmentalstate)
		{
			currentmentalstate = false;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void FrozenMenuUpdate()
	{
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)Main.menu != (Object)null)
		{
			if (Main.closeFrozenPosition == Vector3.zero)
			{
				Main.closeFrozenPosition = ((Component)GorillaTagger.Instance.rigidbody).transform.position;
			}
			else
			{
				((Component)GorillaTagger.Instance.rigidbody).transform.position = Main.closeFrozenPosition;
			}
			GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
			Vector3 forward = ((Component)GorillaTagger.Instance.headCollider).transform.forward;
			forward.y = 0f;
			if (forward != Vector3.zero)
			{
				((Component)GorillaTagger.Instance.bodyCollider).transform.rotation = Quaternion.LookRotation(forward);
			}
		}
		else
		{
			Main.closeFrozenPosition = Vector3.zero;
		}
	}

	public static void LineMenuUpdate()
	{
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)Main.menu != (Object)null)
		{
			if (Main.closeFrozenPosition == Vector3.zero)
			{
				Main.closeFrozenPosition = ((Component)GorillaTagger.Instance.rigidbody).transform.position;
			}
			else
			{
				((Component)GorillaTagger.Instance.rigidbody).transform.position = Main.closeFrozenPosition;
			}
			GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
			Vector3 forward = ((Component)GorillaTagger.Instance.headCollider).transform.forward;
			forward.y = 0f;
			if (forward != Vector3.zero)
			{
				((Component)GorillaTagger.Instance.bodyCollider).transform.rotation = Quaternion.LookRotation(forward);
			}
		}
		else
		{
			Main.closeFrozenPosition = Vector3.zero;
		}
	}

	public static void DisorganizeMenu()
	{
		if (Main.disorganized)
		{
			return;
		}
		Main.disorganized = true;
		ButtonInfo[][] buttons = Buttons.buttons;
		foreach (ButtonInfo[] array in buttons)
		{
			if (array.Length != 0)
			{
				for (int j = 0; j < array.Length; j++)
				{
					Buttons.buttons[Buttons.GetCategory("Main")] = Buttons.buttons[Buttons.GetCategory("Main")].Concat(new ButtonInfo[1] { array[j] }).ToArray();
				}
				Array.Clear(array, 0, array.Length);
			}
		}
	}

	public static void AnnoyingModeOff()
	{
		Main.annoyingMode = false;
		Main.themeType--;
		ChangeMenuTheme();
	}

	public static void DisablePageButtons()
	{
		if (Buttons.GetIndex("Joystick Menu").enabled)
		{
			Main.disablePageButtons = true;
			return;
		}
		Buttons.GetIndex("Disable Page Buttons").enabled = false;
		NotificationManager.SendNotification("<color=grey>[</color><color=red>DISABLE</color><color=grey>]</color> Disable Page Buttons can only be used when using Joystick Menu.");
	}

	public static void CustomMenuName()
	{
		if (!(Time.time > Main.timeMenuStarted + 10f))
		{
			return;
		}
		Main.Prompt("Would you like to set a custom menu name right now?", delegate
		{
			Main.PromptSingleText("What would you like to set the menu name to?", delegate
			{
				File.WriteAllText("SeralythMenu/Seralyth_CustomMenuName.txt", Main.keyboardInput);
				Apply();
				Main.PromptSingle("You can always change this again by re-enabling the mod or changing it in the SeralythMenu folder! (located in the Gorilla Tag installation folder)");
			});
		}, Apply);
		static void Apply()
		{
			Main.doCustomName = true;
			if (!File.Exists("SeralythMenu/Seralyth_CustomMenuName.txt"))
			{
				File.WriteAllText("SeralythMenu/Seralyth_CustomMenuName.txt", "Your Text Here");
			}
			Main.customMenuName = File.ReadAllText("SeralythMenu/Seralyth_CustomMenuName.txt");
		}
	}

	public static void CheckFocus()
	{
		if (!Application.isFocused && lastFocused && Time.time > Main.timeMenuStarted + 5f)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> You are not focused on Gorilla Tag. Voice transcription mods will not function. Please focus/click on the game.");
		}
		lastFocused = Application.isFocused;
		if (Application.isFocused && lastFocused)
		{
			DictationRestart();
		}
	}

	public static void VoiceRecognitionOn()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		if (!File.Exists("SeralythMenu/Seralyth_Keywords.txt"))
		{
			File.WriteAllLines("SeralythMenu/Seralyth_Keywords.txt", keyWords);
		}
		keyWords = File.ReadAllLines("SeralythMenu/Seralyth_Keywords.txt");
		mainPhrases = new KeywordRecognizer(keyWords);
		((PhraseRecognizer)mainPhrases).OnPhraseRecognized += new PhraseRecognizedDelegate(ModRecognition);
		((PhraseRecognizer)mainPhrases).Start();
	}

	public static void ModRecognition(PhraseRecognizedEventArgs args)
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Expected O, but got Unknown
		((PhraseRecognizer)mainPhrases).Stop();
		if (!Buttons.GetIndex("Chain Voice Commands").enabled)
		{
			timeoutCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(Timeout(string.Empty));
		}
		List<string> list = cancelKeywords.ToList();
		ButtonInfo[][] buttons = Buttons.buttons;
		foreach (ButtonInfo[] array in buttons)
		{
			ButtonInfo[] array2 = array;
			foreach (ButtonInfo buttonInfo in array2)
			{
				string text = buttonInfo.overlapText ?? buttonInfo.buttonText;
				if (text.Contains(" <color"))
				{
					text = text.Split(" <color")[0];
				}
				list.Add(text);
			}
		}
		modPhrases = new KeywordRecognizer(list.ToArray());
		((PhraseRecognizer)modPhrases).OnPhraseRecognized += new PhraseRecognizedDelegate(ExecuteVoiceCommand);
		((PhraseRecognizer)modPhrases).Start();
		if (Main.dynamicSounds)
		{
			AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Menu/select.ogg", "Audio/Menu/select.ogg", delegate(AudioClip clip)
			{
				DictationPlay(clip, (float)Main.buttonClickVolume / 10f);
			});
		}
		NotificationManager.SendNotification("<color=grey>[</color><color=purple>VOICE</color><color=grey>]</color> Listening...", 3000);
	}

	public static void ExecuteVoiceCommand(PhraseRecognizedEventArgs args)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		if (!Buttons.GetIndex("Chain Voice Commands").enabled)
		{
			((PhraseRecognizer)modPhrases).Stop();
			((PhraseRecognizer)mainPhrases).Start();
			((MonoBehaviour)CoroutineManager.instance).StopCoroutine(timeoutCoroutine);
		}
		if (cancelKeywords.Contains(args.text))
		{
			CancelModRecognition(args.text);
			return;
		}
		string text = null;
		bool flag = false;
		ButtonInfo[][] buttons = Buttons.buttons;
		foreach (ButtonInfo[] array in buttons)
		{
			if (flag)
			{
				break;
			}
			ButtonInfo[] array2 = array;
			foreach (ButtonInfo buttonInfo in array2)
			{
				if (flag)
				{
					break;
				}
				string text2 = buttonInfo.overlapText ?? buttonInfo.buttonText;
				if (text2.Contains(" <color"))
				{
					text2 = text2.Split(" <color")[0];
				}
				if (args.text.ToLower() == text2.ToLower())
				{
					text = buttonInfo.buttonText;
					flag = true;
				}
				else if (args.text.Contains(text2.ToLower()))
				{
					text = buttonInfo.buttonText;
				}
			}
		}
		if (text != null)
		{
			ButtonInfo index = Buttons.GetIndex(text);
			NotificationManager.SendNotification("<color=grey>[</color><color=" + (index.enabled ? "red" : "green") + ">VOICE</color><color=grey>]</color> " + (index.enabled ? "Disabling " : "Enabling ") + (index.overlapText ?? index.buttonText) + "...", 3000);
			if (Main.dynamicSounds)
			{
				AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Menu/confirm.ogg", "Audio/Menu/confirm.ogg", delegate(AudioClip clip)
				{
					DictationPlay(clip, (float)Main.buttonClickVolume / 10f);
				});
			}
			Main.Toggle(text, fromMenu: true, ignoreForce: true);
			return;
		}
		NotificationManager.SendNotification("<color=grey>[</color><color=red>VOICE</color><color=grey>]</color> No command found (" + args.text + ").", 3000);
		if (Main.dynamicSounds)
		{
			AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Menu/close.ogg", "Audio/Menu/close.ogg", delegate(AudioClip clip)
			{
				DictationPlay(clip, (float)Main.buttonClickVolume / 10f);
			});
		}
	}

	public static IEnumerator Timeout(string text)
	{
		yield return (object)new WaitForSeconds(10f);
		CancelModRecognition(text);
	}

	public static void CancelModRecognition(string text)
	{
		((PhraseRecognizer)modPhrases).Stop();
		((PhraseRecognizer)mainPhrases).Start();
		try
		{
			((MonoBehaviour)CoroutineManager.instance).StopCoroutine(timeoutCoroutine);
		}
		catch
		{
		}
		NotificationManager.SendNotification("<color=grey>[</color><color=red>VOICE</color><color=grey>]</color> " + ((text == "i hate you") ? "I hate you too." : "Cancelling..."), 3000);
		if (Main.dynamicSounds)
		{
			AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Menu/close.ogg", "Audio/Menu/close.ogg", delegate(AudioClip clip)
			{
				DictationPlay(clip, (float)Main.buttonClickVolume / 10f);
			});
		}
	}

	public static void VoiceRecognitionOff()
	{
		KeywordRecognizer obj = mainPhrases;
		if (obj != null)
		{
			((PhraseRecognizer)obj).Dispose();
		}
		KeywordRecognizer obj2 = mainPhrases;
		if (obj2 != null)
		{
			((PhraseRecognizer)obj2).Stop();
		}
		KeywordRecognizer obj3 = modPhrases;
		if (obj3 != null)
		{
			((PhraseRecognizer)obj3).Dispose();
		}
		KeywordRecognizer obj4 = modPhrases;
		if (obj4 != null)
		{
			((PhraseRecognizer)obj4).Stop();
		}
		mainPhrases = null;
		modPhrases = null;
		PhraseRecognitionSystem.Shutdown();
	}

	public static IEnumerator DictationOn()
	{
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
		ButtonInfo vc = Buttons.GetIndex("Voice Commands");
		if (vc.enabled)
		{
			Main.Prompt("You currently have Voice Commands enabled. Would you like to disable it?", delegate
			{
				vc.enabled = false;
			}, delegate
			{
				mod.enabled = false;
			});
		}
		else if ((int)PhraseRecognitionSystem.Status > 0)
		{
			Main.PromptSingle("You can not use AI Assistant while you have another voice-related mod on.", delegate
			{
				mod.enabled = false;
			}, "Ok");
		}
		if (!File.Exists("SeralythMenu/Seralyth_Keywords.txt"))
		{
			File.WriteAllLines("SeralythMenu/Seralyth_Keywords.txt", keyWords);
		}
		keyWords = File.ReadAllLines("SeralythMenu/Seralyth_Keywords.txt");
		while ((int)PhraseRecognitionSystem.Status > 0)
		{
			yield return null;
		}
		string[] kw = keyWords;
		if (Main.narratorName == "Mommy ASMR")
		{
			kw = kw.Concat(new string[2] { "mommy", "momma" }).ToArray();
		}
		krec = new KeywordRecognizer(kw);
		KeywordRecognizer obj = krec;
		object obj2 = _003C_003Ec._003C_003E9__143_5;
		if (obj2 == null)
		{
			PhraseRecognizedDelegate val = delegate
			{
				((MonoBehaviour)CoroutineManager.instance).StartCoroutine(DictationRecognizer());
			};
			_003C_003Ec._003C_003E9__143_5 = val;
			obj2 = (object)val;
		}
		((PhraseRecognizer)obj).OnPhraseRecognized += (PhraseRecognizedDelegate)obj2;
		((PhraseRecognizer)krec).Start();
	}

	public unsafe static IEnumerator DictationRecognizer()
	{
		if (AIManager.generating)
		{
			yield break;
		}
		ButtonInfo mod = Buttons.GetIndex("AI Assistant");
		PhraseRecognitionSystem.Shutdown();
		while ((int)PhraseRecognitionSystem.Status > 0)
		{
			yield return null;
		}
		string narratorName = Main.narratorName;
		string text = narratorName;
		if (text == "Mommy ASMR")
		{
			AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/TTS/yes_sweetheart.ogg", "Audio/TTS/yes_sweetheart.ogg", delegate(AudioClip clip)
			{
				DictationPlay(clip, (float)Main.buttonClickVolume / 10f);
			});
			NotificationManager.SendNotification("<color=grey>[</color><color=#ffb6c1>MOMMY</color><color=grey>]</color> Yes, sweetheart?", 3000);
		}
		else
		{
			AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Menu/select.ogg", "Audio/Menu/select.ogg", delegate(AudioClip clip)
			{
				DictationPlay(clip, (float)Main.buttonClickVolume / 10f);
			});
			NotificationManager.SendNotification("<color=grey>[</color><color=purple>VOICE</color><color=grey>]</color> Listening...", 3000);
		}
		if (debugDictation)
		{
			LogManager.Log("Dictation listening");
		}
		drec = new DictationRecognizer();
		DictationRecognizer obj = drec;
		object obj2 = _003C_003Ec._003C_003E9__144_0;
		if (obj2 == null)
		{
			DictationResultDelegate val = delegate(string text2, ConfidenceLevel confidence)
			{
				if (debugDictation)
				{
					LogManager.Log("Dictation result: " + text2);
				}
				if (cancelKeywords.Contains(text2.ToLower()))
				{
					if (Main.dynamicSounds)
					{
						AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Menu/close.ogg", "Audio/Menu/close.ogg", delegate(AudioClip clip)
						{
							DictationPlay(clip, (float)Main.buttonClickVolume / 10f);
						});
					}
					NotificationManager.SendNotification("<color=grey>[</color><color=red>AI</color><color=grey>]</color> " + ((text2.ToLower() == "i hate you") ? "I hate you too." : "Cancelling..."), 3000);
					((MonoBehaviour)CoroutineManager.instance).StartCoroutine(DictationRestart());
				}
				else
				{
					string narratorName2 = Main.narratorName;
					string text3 = narratorName2;
					if (text3 == "Mommy ASMR")
					{
						NotificationManager.SendNotification("<color=grey>[</color><color=#ffb6c1>MOMMY</color><color=grey>]</color> Let me get that for you..");
					}
					else
					{
						NotificationManager.SendNotification("<color=grey>[</color><color=blue>AI</color><color=grey>]</color> Generating response..");
					}
					((MonoBehaviour)CoroutineManager.instance).StartCoroutine(AIManager.AskAI(text2));
				}
			};
			_003C_003Ec._003C_003E9__144_0 = val;
			obj2 = (object)val;
		}
		obj.DictationResult += (DictationResultDelegate)obj2;
		DictationRecognizer obj3 = drec;
		object obj4 = _003C_003Ec._003C_003E9__144_1;
		if (obj4 == null)
		{
			DictationCompletedDelegate val2 = delegate(DictationCompletionCause completionCause)
			{
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				if (debugDictation)
				{
					LogManager.Log($"completion cause: {completionCause}");
				}
				if (((object)(*(DictationCompletionCause*)(&completionCause))/*cast due to .constrained prefix*/).ToString() == "TimeoutExceeded")
				{
					if (Main.dynamicSounds)
					{
						AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Menu/close.ogg", "Audio/Menu/close.ogg", delegate(AudioClip clip)
						{
							DictationPlay(clip, (float)Main.buttonClickVolume / 10f);
						});
					}
					NotificationManager.SendNotification("<color=grey>[</color><color=red>AI</color><color=grey>]</color> Cancelling...", 3000);
				}
			};
			_003C_003Ec._003C_003E9__144_1 = val2;
			obj4 = (object)val2;
		}
		obj3.DictationComplete += (DictationCompletedDelegate)obj4;
		drec.DictationError += (DictationErrorHandler)delegate(string error, int hresult)
		{
			if (debugDictation)
			{
				LogManager.LogError("Dictation error: " + error);
			}
			if (error.Contains("Dictation support is not enabled on this device"))
			{
				DictationOff();
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Online Speech Recognition is not enabled on this device. Either open the menu to enable it, or check your internet connection.", 3000);
				Main.Prompt("Online Speech Recognition is not enabled on your device. Would you like to open the Settings page to enable it?", delegate
				{
					Process.Start("ms-settings:privacy-speech");
					Main.PromptSingle("Once you enable Online Speech Recognition, turn this mod back on!", delegate
					{
						mod.enabled = false;
					}, "Ok");
				}, delegate
				{
					Main.PromptSingle("You will not be able to use this mod until you enable Online Speech Recognition.", delegate
					{
						mod.enabled = false;
					}, "Ok");
				});
			}
		};
		DictationRecognizer obj5 = drec;
		object obj6 = _003C_003Ec._003C_003E9__144_3;
		if (obj6 == null)
		{
			DictationHypothesisDelegate val3 = delegate(string text2)
			{
				if (!AIManager.generating)
				{
					if (debugDictation)
					{
						LogManager.Log("Hypothesis: " + text2);
					}
					NotificationManager.ClearAllNotifications();
					NotificationManager.SendNotification("<color=grey>[</color><color=green>VOICE</color><color=grey>]</color> " + text2);
				}
			};
			_003C_003Ec._003C_003E9__144_3 = val3;
			obj6 = (object)val3;
		}
		obj5.DictationHypothesis += (DictationHypothesisDelegate)obj6;
		DictationRecognizer obj7 = drec;
		if (obj7 != null)
		{
			obj7.Start();
		}
	}

	public static IEnumerator DictationRestart()
	{
		DictationOff();
		while ((int)PhraseRecognitionSystem.Status > 0)
		{
			yield return null;
		}
		((MonoBehaviour)CoroutineManager.instance).StartCoroutine(DictationOn());
	}

	public static void DictationOff()
	{
		DictationRecognizer obj = drec;
		if (obj != null)
		{
			obj.Dispose();
		}
		DictationRecognizer obj2 = drec;
		if (obj2 != null)
		{
			obj2.Stop();
		}
		drec = null;
		PhraseRecognitionSystem.Shutdown();
	}

	public static void DictationPlay(AudioClip clip, float volume)
	{
		if (Buttons.GetIndex("Global Dynamic Sounds").enabled)
		{
			Sound.PlayAudio(clip);
		}
		else
		{
			Main.Play2DAudio(clip, volume);
		}
	}

	private static void ResetClickGUIInput()
	{
		pressedUI = null;
		draggedUI = null;
		currentUI = null;
		lastTriggerClick = false;
		lastRightPrimary = false;
		isDragging = false;
		if (pointerData != null)
		{
			pointerData.pointerDrag = null;
			pointerData.pointerPress = null;
			pointerData.pointerEnter = null;
		}
	}

	private static GameObject GetClickableTarget(GameObject hitObject)
	{
		return ExecuteEvents.GetEventHandler<IPointerDownHandler>(hitObject) ?? ExecuteEvents.GetEventHandler<IPointerClickHandler>(hitObject) ?? ExecuteEvents.GetEventHandler<ISubmitHandler>(hitObject) ?? hitObject;
	}

	private static GameObject GetDragTarget(GameObject hitObject)
	{
		return ExecuteEvents.GetEventHandler<IDragHandler>(hitObject);
	}

	public static void ReloadOnCategoryChange()
	{
		Main.ReloadMenu();
	}

	public static void EnableClickGUI()
	{
		Main.clickGUI = true;
		Main.ReloadMenu();
		Buttons.OnCategoryChanged += ReloadOnCategoryChange;
	}

	public static void DisableClickGUI()
	{
		Main.clickGUI = false;
		Buttons.OnCategoryChanged -= ReloadOnCategoryChange;
		if ((Object)(object)clickGuiLine != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)clickGuiLine).gameObject);
			clickGuiLine = null;
		}
		canvas = null;
		ResetClickGUIInput();
	}

	public static void InitializeClickGUI()
	{
		try
		{
			InitializeClickGUIImpl();
		}
		catch (Exception ex)
		{
			LogManager.LogError("InitializeClickGUI failed: " + ex.Message + "\n" + ex.StackTrace);
		}
	}

	private static void InitializeClickGUIImpl()
	{
		//IL_1491: Unknown result type (might be due to invalid IL or missing references)
		//IL_1498: Expected O, but got Unknown
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Expected O, but got Unknown
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Expected O, but got Unknown
		//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Expected O, but got Unknown
		//IL_0687: Unknown result type (might be due to invalid IL or missing references)
		//IL_068e: Expected O, but got Unknown
		//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d14: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d1b: Expected O, but got Unknown
		//IL_091a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0921: Expected O, but got Unknown
		//IL_0dd9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0989: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ce: Expected O, but got Unknown
		//IL_09fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a08: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a33: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e85: Expected O, but got Unknown
		//IL_0bd9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be3: Expected O, but got Unknown
		//IL_107e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1083: Unknown result type (might be due to invalid IL or missing references)
		//IL_1089: Expected O, but got Unknown
		//IL_115a: Unknown result type (might be due to invalid IL or missing references)
		//IL_115f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1165: Expected O, but got Unknown
		//IL_0c5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb7: Unknown result type (might be due to invalid IL or missing references)
		//IL_16c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_16c9: Expected O, but got Unknown
		//IL_12d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_12da: Expected O, but got Unknown
		Transform obj = Main.menu.transform.Find("Canvas");
		canvas = ((obj != null) ? ((Component)obj).GetComponent<Canvas>() : null);
		if ((Object)(object)canvas == (Object)null)
		{
			canvas = Main.menu.AddComponent<Canvas>();
			canvas.renderMode = (RenderMode)(XRSettings.isDeviceActive ? 2 : 0);
		}
		GTExt.GetOrAddComponent<GraphicRaycaster>(((Component)canvas).gameObject);
		if (!XRSettings.isDeviceActive)
		{
			canvas.renderMode = (RenderMode)0;
			canvas.sortingOrder = 1;
		}
		Transform transform = ((Component)canvas).transform;
		TextMeshProUGUI[] componentsInChildren = ((Component)transform).GetComponentsInChildren<TextMeshProUGUI>(true);
		foreach (TextMeshProUGUI val in componentsInChildren)
		{
			Main.FollowMenuSettings((TMP_Text)(object)val);
			((TMP_Text)(object)val).Chams();
			string text = ((Object)((Component)val).gameObject).name.ToLower();
			if (text.Contains("watermark"))
			{
				((TMP_Text)(object)val).SafeSetText("Build 10.0.2");
			}
			else if (text.Contains("title") || text.Contains("name"))
			{
				Transform parent = ((TMP_Text)val).transform.parent;
				if ((Object)(object)parent != (Object)null && ((Object)parent).name == "Sidebar")
				{
					((TMP_Text)(object)val).SafeSetText(Main.doCustomName ? Main.NoRichtextTags(Main.customMenuName) : "MrChicken Menu");
				}
				else if ((Object)(object)parent != (Object)null && ((Object)parent).name == "PromptTab")
				{
					((TMP_Text)(object)val).SafeSetText(Main.CurrentPrompt?.Message ?? "");
				}
			}
		}
		MaskableGraphic[] componentsInChildren2 = ((Component)transform).GetComponentsInChildren<MaskableGraphic>(true);
		foreach (MaskableGraphic val2 in componentsInChildren2)
		{
			if (!(val2 is TMP_Text))
			{
				GTExt.GetOrAddComponent<UIColorChanger>(((Component)val2).gameObject).colors = Main.buttonColors[1];
			}
		}
		TMP_Text[] componentsInChildren3 = ((Component)transform).GetComponentsInChildren<TMP_Text>(true);
		foreach (TMP_Text val3 in componentsInChildren3)
		{
			UIColorChanger orAddComponent = GTExt.GetOrAddComponent<UIColorChanger>(((Component)val3).gameObject);
			orAddComponent.colors = (((Object)((Component)val3).gameObject).name.ToLower().Contains("watermark") ? Main.textColors[0] : Main.textColors[1]);
		}
		Transform val4 = transform.Find("Main");
		if ((Object)(object)val4 != (Object)null)
		{
			GTExt.GetOrAddComponent<UIColorChanger>(((Component)val4).gameObject).colors = Main.backgroundColor;
		}
		Transform val5 = transform.Find("Main/Sidebar");
		if ((Object)(object)val5 != (Object)null)
		{
			ExtGradient extGradient = Main.buttonColors[1].Clone();
			for (int l = 0; l < extGradient.colors.Length; l++)
			{
				extGradient.colors[l] = new GradientColorKey
				{
					time = extGradient.colors[l].time,
					color = Main.DarkenColor(extGradient.colors[l].color, 0.35f)
				};
			}
			GTExt.GetOrAddComponent<UIColorChanger>(((Component)val5).gameObject).colors = extGradient;
		}
		Transform val6 = transform.Find("Main/Separator");
		if ((Object)(object)val6 != (Object)null)
		{
			GTExt.GetOrAddComponent<UIColorChanger>(((Component)val6).gameObject).colors = Main.buttonColors[1];
		}
		Transform val7 = transform.Find("Main/Sidebar");
		if ((Object)(object)val7 != (Object)null)
		{
			string[] array = new string[3] { "Settings", "Players", "Friends" };
			foreach (string text2 in array)
			{
				Transform val8 = val7.Find(text2);
				if (!((Object)(object)val8 == (Object)null))
				{
					string captured = text2;
					((UnityEventBase)((Component)val8).GetComponent<Button>().onClick).RemoveAllListeners();
					((UnityEvent)((Component)val8).GetComponent<Button>().onClick).AddListener((UnityAction)delegate
					{
						Main.Toggle(captured);
						SoundManager.Play(SoundManager.DefaultSounds["Button"]);
					});
					Main.FollowMenuSettings((TMP_Text)(object)((Component)val8).GetComponentInChildren<TextMeshProUGUI>());
				}
			}
		}
		Transform val9 = transform.Find("Main/Sidebar/Scroll View/Viewport/Content");
		if ((Object)(object)val9 == (Object)null)
		{
			LogManager.LogError("ClickGUI: Missing Sidebar/Scroll View/Viewport/Content");
			return;
		}
		List<GameObject> list = new List<GameObject>();
		foreach (Transform item in val9)
		{
			Transform val10 = item;
			if (((Object)val10).name != "Home" && ((Object)val10).name != "Other")
			{
				list.Add(((Component)val10).gameObject);
			}
		}
		foreach (GameObject item2 in list)
		{
			Object.Destroy((Object)(object)item2);
		}
		Transform obj2 = val9.Find("Other");
		GameObject val11 = ((obj2 != null) ? ((Component)obj2).gameObject : null);
		if ((Object)(object)val11 == (Object)null)
		{
			LogManager.LogError("ClickGUI: Missing 'Other' tab template");
			return;
		}
		foreach (Transform item3 in val11.transform)
		{
			Transform val12 = item3;
			if (((Object)val12).name != "Title" && ((Object)val12).name != "Image")
			{
				Object.Destroy((Object)(object)((Component)val12).gameObject);
				continue;
			}
			Image component = ((Component)val12).GetComponent<Image>();
			if ((Object)(object)component != (Object)null)
			{
				((Behaviour)component).enabled = false;
			}
		}
		val11.SetActive(false);
		Transform val13 = val9.Find("Home");
		object obj3;
		if (val13 == null)
		{
			obj3 = null;
		}
		else
		{
			Transform obj4 = val13.Find("Selection");
			obj3 = ((obj4 != null) ? ((Component)obj4).gameObject : null);
		}
		GameObject val14 = (GameObject)obj3;
		if ((Object)(object)val14 == (Object)null)
		{
			val14 = new GameObject("Selection");
			((Transform)val14.AddComponent<RectTransform>()).SetParent(val13, false);
			((Graphic)val14.AddComponent<Image>()).color = Color.white;
			GTExt.GetOrAddComponent<UIColorChanger>(val14).colors = Main.buttonColors[1];
		}
		bool flag = false;
		ButtonInfo[][] buttons = Buttons.buttons;
		ButtonInfo[] array2 = ((buttons != null) ? buttons[0] : null);
		if (array2 == null)
		{
			LogManager.LogError("ClickGUI: buttons[0] is null");
			return;
		}
		List<string> list2 = new List<string>();
		list2.Add("Join Discord");
		ButtonInfo[] array3 = array2;
		foreach (ButtonInfo buttonInfo in array3)
		{
			if (buttonInfo != null && !(buttonInfo.buttonText == "Join Discord") && !(buttonInfo.buttonText == "configuration") && !buttonInfo.label && !buttonInfo.buttonText.StartsWith("Exit ") && ((!buttonInfo.buttonText.Contains("Admin") && !(buttonInfo.buttonText == "Mod Givers")) || Main.isAdmin) && (!buttonInfo.buttonText.Contains("Detected") || Main.allowDetected))
			{
				list2.Add(buttonInfo.buttonText);
			}
		}
		if (!list2.Contains("Favorite Mods"))
		{
			list2.Add("Favorite Mods");
		}
		if (!list2.Contains("Enabled Mods"))
		{
			list2.Add("Enabled Mods");
		}
		foreach (string item4 in list2)
		{
			GameObject val15 = Object.Instantiate<GameObject>(val11, val9, false);
			val15.SetActive(true);
			((Object)val15).name = item4;
			Transform obj5 = val15.transform.Find("Title");
			TextMeshProUGUI val16 = ((obj5 != null) ? ((Component)obj5).GetComponent<TextMeshProUGUI>() : null);
			if ((Object)(object)val16 != (Object)null)
			{
				((TMP_Text)(object)val16).SafeSetText(item4);
				Main.FollowMenuSettings((TMP_Text)(object)val16);
				((TMP_Text)(object)val16).Chams();
				GTExt.GetOrAddComponent<UIColorChanger>(((Component)val16).gameObject).colors = Main.textColors[1];
			}
			TextMeshProUGUI componentInChildren = val15.GetComponentInChildren<TextMeshProUGUI>();
			if ((Object)(object)componentInChildren != (Object)null)
			{
				Main.FollowMenuSettings((TMP_Text)(object)componentInChildren);
			}
			foreach (Transform item5 in val15.transform)
			{
				Transform val17 = item5;
				if (((Object)val17).name != "Title" && ((Object)val17).name != "Image")
				{
					Object.Destroy((Object)(object)((Component)val17).gameObject);
				}
			}
			GTExt.GetOrAddComponent<UIColorChanger>(val15).colors = new ExtGradient
			{
				colors = ExtGradient.GetSolidGradient(Color.black)
			};
			Transform val18 = val15.transform.Find("Image");
			if ((Object)(object)val18 == (Object)null)
			{
				GameObject val19 = new GameObject("Image");
				val19.transform.SetParent(val15.transform, false);
				RectTransform val20 = val19.AddComponent<RectTransform>();
				Vector2 anchorMin = (val20.anchorMax = new Vector2(0f, 0.5f));
				val20.anchorMin = anchorMin;
				val20.pivot = new Vector2(0.5f, 0.5f);
				val20.sizeDelta = new Vector2(20f, 20f);
				val20.anchoredPosition = Vector2.zero;
				TextMeshProUGUI val22 = val19.AddComponent<TextMeshProUGUI>();
				((TMP_Text)val22).fontSize = 16f;
				((Graphic)val22).color = Color.white;
				((TMP_Text)val22).alignment = (TextAlignmentOptions)514;
				val18 = val19.transform;
			}
			Image component2 = ((Component)val18).GetComponent<Image>();
			if ((Object)(object)component2 != (Object)null)
			{
				((Behaviour)component2).enabled = false;
			}
			TextMeshProUGUI val23 = ((Component)val18).GetComponent<TextMeshProUGUI>();
			if ((Object)(object)val23 == (Object)null)
			{
				val23 = ((Component)val18).gameObject.AddComponent<TextMeshProUGUI>();
			}
			bool flag2 = item4 == "Favorite Mods" || item4 == "Enabled Mods";
			if (!flag2)
			{
				int category = Buttons.GetCategory(item4);
				if (category >= 0 && category < Buttons.buttons.Length && Buttons.buttons[category] != null)
				{
					ButtonInfo[] array4 = Buttons.buttons[category];
					foreach (ButtonInfo buttonInfo2 in array4)
					{
						if (buttonInfo2 != null && buttonInfo2.enabled)
						{
							flag2 = true;
							break;
						}
					}
				}
			}
			if (flag2)
			{
				((Component)val18).gameObject.SetActive(true);
				((TMP_Text)(object)val23).SafeSetText("★");
				Main.FollowMenuSettings((TMP_Text)(object)val23);
			}
			else
			{
				((Component)val18).gameObject.SetActive(false);
			}
			string captured2 = item4;
			Button component3 = val15.GetComponent<Button>();
			((UnityEventBase)component3.onClick).RemoveAllListeners();
			((UnityEvent)component3.onClick).AddListener((UnityAction)delegate
			{
				switch (captured2)
				{
				case "Join Discord":
					Important.JoinDiscord();
					break;
				case "Players":
					PlayersTab();
					break;
				case "Detected Mods":
					Detected.EnterDetectedTab();
					break;
				case "Achievements":
					AchievementManager.EnterAchievementTab();
					break;
				case "Update Category":
					Changelog.RefreshCategory();
					goto default;
				default:
				{
					int category2 = Buttons.GetCategory(captured2);
					if (category2 >= 0)
					{
						Buttons.CurrentCategoryIndex = category2;
						Main.ReloadMenu();
					}
					break;
				}
				}
				SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			});
			string currentCategoryName = Buttons.CurrentCategoryName;
			if (currentCategoryName == captured2 || currentCategoryName.StartsWith(captured2))
			{
				flag = true;
				val14.transform.SetParent(val15.transform, false);
				continue;
			}
			Transform val24 = val15.transform.Find("Title");
			if ((Object)(object)val24 != (Object)null)
			{
				RectTransform component4 = ((Component)val24).GetComponent<RectTransform>();
				((Transform)component4).localPosition = ((Transform)component4).localPosition + Vector3.left * 10f;
			}
			Transform val25 = val15.transform.Find("Image");
			if ((Object)(object)val25 != (Object)null)
			{
				RectTransform component5 = ((Component)val25).GetComponent<RectTransform>();
				((Transform)component5).localPosition = ((Transform)component5).localPosition + Vector3.left * 10f;
			}
		}
		Transform val26 = val9.Find("Home");
		if ((Object)(object)val26 != (Object)null)
		{
			foreach (Transform item6 in val26)
			{
				Transform val27 = item6;
				if (((Object)val27).name != "Title" && ((Object)val27).name != "Image" && ((Object)val27).name != "Selection")
				{
					Object.Destroy((Object)(object)((Component)val27).gameObject);
				}
			}
			Transform val28 = val26.Find("Image");
			if ((Object)(object)val28 != (Object)null)
			{
				Image component6 = ((Component)val28).GetComponent<Image>();
				if ((Object)(object)component6 != (Object)null)
				{
					((Behaviour)component6).enabled = false;
				}
			}
			GTExt.GetOrAddComponent<UIColorChanger>(((Component)val26).gameObject).colors = new ExtGradient
			{
				colors = ExtGradient.GetSolidGradient(Color.black)
			};
			Transform obj6 = val26.Find("Title");
			TextMeshProUGUI val29 = ((obj6 != null) ? ((Component)obj6).GetComponent<TextMeshProUGUI>() : null);
			if ((Object)(object)val29 != (Object)null)
			{
				Main.FollowMenuSettings((TMP_Text)(object)val29);
				((TMP_Text)(object)val29).Chams();
				GTExt.GetOrAddComponent<UIColorChanger>(((Component)val29).gameObject).colors = Main.textColors[1];
			}
			Button component7 = ((Component)val26).GetComponent<Button>();
			((UnityEventBase)component7.onClick).RemoveAllListeners();
			ButtonClickedEvent onClick = component7.onClick;
			object obj7 = _003C_003Ec._003C_003E9__167_2;
			if (obj7 == null)
			{
				UnityAction val30 = delegate
				{
					Buttons.CurrentCategoryIndex = 0;
					Main.ReloadMenu();
					SoundManager.Play(SoundManager.DefaultSounds["Button"]);
				};
				_003C_003Ec._003C_003E9__167_2 = val30;
				obj7 = (object)val30;
			}
			((UnityEvent)onClick).AddListener((UnityAction)obj7);
			if (Buttons.CurrentCategoryIndex == 0)
			{
				flag = true;
				val14.transform.SetParent(val26, false);
			}
		}
		if (!flag)
		{
			val14.SetActive(false);
		}
		Transform val31 = transform.Find("Main/HomeTab");
		Transform val32 = transform.Find("Main/ModuleTab");
		Transform val33 = transform.Find("Main/PromptTab");
		if ((Object)(object)val31 != (Object)null)
		{
			((Component)val31).gameObject.SetActive(false);
		}
		if ((Object)(object)val32 != (Object)null)
		{
			((Component)val32).gameObject.SetActive(false);
		}
		if ((Object)(object)val33 != (Object)null)
		{
			((Component)val33).gameObject.SetActive(false);
		}
		if (Main.CurrentPrompt != null)
		{
			if ((Object)(object)val33 != (Object)null)
			{
				((Component)val33).gameObject.SetActive(true);
				Transform obj8 = val33.Find("Title");
				TextMeshProUGUI val34 = ((obj8 != null) ? ((Component)obj8).GetComponent<TextMeshProUGUI>() : null);
				if ((Object)(object)val34 != (Object)null)
				{
					((TMP_Text)(object)val34).SafeSetText(Main.CurrentPrompt.Message);
					Main.FollowMenuSettings((TMP_Text)(object)val34);
				}
				Transform val35 = val33.Find("Accept");
				if ((Object)(object)val35 != (Object)null)
				{
					GTExt.GetOrAddComponent<UIColorChanger>(((Component)val35).gameObject).colors = Main.buttonColors[0];
					Transform obj9 = val35.Find("Text");
					TextMeshProUGUI val36 = ((obj9 != null) ? ((Component)obj9).GetComponent<TextMeshProUGUI>() : null);
					if ((Object)(object)val36 != (Object)null)
					{
						((TMP_Text)(object)val36).SafeSetText(Main.CurrentPrompt.AcceptText);
						Main.FollowMenuSettings((TMP_Text)(object)val36);
						((TMP_Text)(object)val36).Chams();
					}
					((UnityEventBase)((Component)val35).GetComponent<Button>().onClick).RemoveAllListeners();
					ButtonClickedEvent onClick2 = ((Component)val35).GetComponent<Button>().onClick;
					object obj10 = _003C_003Ec._003C_003E9__167_3;
					if (obj10 == null)
					{
						UnityAction val37 = delegate
						{
							Main.Toggle("Accept Prompt");
							SoundManager.Play(SoundManager.DefaultSounds["Button"]);
							Main.ReloadMenu();
						};
						_003C_003Ec._003C_003E9__167_3 = val37;
						obj10 = (object)val37;
					}
					((UnityEvent)onClick2).AddListener((UnityAction)obj10);
				}
				Transform val38 = val33.Find("Decline");
				if ((Object)(object)val38 != (Object)null && Main.CurrentPrompt.DeclineText != null)
				{
					GTExt.GetOrAddComponent<UIColorChanger>(((Component)val38).gameObject).colors = Main.buttonColors[0];
					Transform obj11 = val38.Find("Text");
					TextMeshProUGUI val39 = ((obj11 != null) ? ((Component)obj11).GetComponent<TextMeshProUGUI>() : null);
					if ((Object)(object)val39 != (Object)null)
					{
						((TMP_Text)(object)val39).SafeSetText(Main.CurrentPrompt.DeclineText);
						Main.FollowMenuSettings((TMP_Text)(object)val39);
					}
					((UnityEventBase)((Component)val38).GetComponent<Button>().onClick).RemoveAllListeners();
					ButtonClickedEvent onClick3 = ((Component)val38).GetComponent<Button>().onClick;
					object obj12 = _003C_003Ec._003C_003E9__167_4;
					if (obj12 == null)
					{
						UnityAction val40 = delegate
						{
							Main.Toggle("Decline Prompt");
							SoundManager.Play(SoundManager.DefaultSounds["Button"]);
							Main.ReloadMenu();
						};
						_003C_003Ec._003C_003E9__167_4 = val40;
						obj12 = (object)val40;
					}
					((UnityEvent)onClick3).AddListener((UnityAction)obj12);
				}
				else if ((Object)(object)val38 != (Object)null)
				{
					((Component)val38).gameObject.SetActive(false);
				}
			}
		}
		else if (Buttons.CurrentCategoryIndex == 0 && (Object)(object)val31 != (Object)null)
		{
			((Component)val31).gameObject.SetActive(true);
			Transform obj13 = ((Component)canvas).transform.Find("Main/Button");
			GameObject val41 = ((obj13 != null) ? ((Component)obj13).gameObject : null);
			Transform obj14 = val31.Find("Title");
			TextMeshProUGUI val42 = ((obj14 != null) ? ((Component)obj14).GetComponent<TextMeshProUGUI>() : null);
			if ((Object)(object)val42 != (Object)null)
			{
				((TMP_Text)(object)val42).SafeSetText("Hey, " + (PhotonNetwork.LocalPlayer.NickName ?? "null") + "!");
			}
			Transform obj15 = val31.Find("EnabledTitle");
			TextMeshProUGUI val43 = ((obj15 != null) ? ((Component)obj15).GetComponent<TextMeshProUGUI>() : null);
			if ((Object)(object)val43 != (Object)null)
			{
				((TMP_Text)(object)val43).SafeSetText("Enabled Mods");
			}
			Transform obj16 = val31.Find("FavoritesTitle");
			TextMeshProUGUI val44 = ((obj16 != null) ? ((Component)obj16).GetComponent<TextMeshProUGUI>() : null);
			if ((Object)(object)val44 != (Object)null)
			{
				((TMP_Text)(object)val44).SafeSetText("Favorites");
			}
			Transform val45 = val31.Find("Enabled/Viewport/Content");
			if ((Object)(object)val45 != (Object)null)
			{
				foreach (Transform item7 in val45)
				{
					Transform val46 = item7;
					Object.Destroy((Object)(object)((Component)val46).gameObject);
				}
				List<ButtonInfo> list3 = new List<ButtonInfo>();
				int ci = 0;
				ButtonInfo[][] buttons2 = Buttons.buttons;
				foreach (ButtonInfo[] source in buttons2)
				{
					list3.AddRange(source.Where((ButtonInfo v) => v.enabled && (!Buttons.categoryNames[ci].Contains("Settings") || !Main.hideSettings) && (!Buttons.categoryNames[ci].Contains("Macro") || !Main.hideMacros)));
					ci++;
				}
				list3 = list3.OrderBy((ButtonInfo v) => v.overlapText ?? v.buttonText).ToList();
				Transform val47 = val45.Find("None");
				if (list3.Count > 0)
				{
					if ((Object)(object)val47 != (Object)null)
					{
						((Component)val47).gameObject.SetActive(false);
					}
					if ((Object)(object)val41 != (Object)null)
					{
						foreach (ButtonInfo item8 in list3)
						{
							try
							{
								CreateButton(val45, item8, val41);
							}
							catch
							{
							}
						}
					}
				}
				else if ((Object)(object)val47 != (Object)null)
				{
					((Component)val47).gameObject.SetActive(true);
				}
			}
			Transform val48 = val31.Find("Favorites/Viewport/Content");
			if ((Object)(object)val48 != (Object)null)
			{
				foreach (Transform item9 in val48)
				{
					Transform val49 = item9;
					Object.Destroy((Object)(object)((Component)val49).gameObject);
				}
				List<ButtonInfo> list4 = Main.StringsToInfos(Main.favorites.ToArray()).ToList();
				if (list4.Count > 0)
				{
					list4.RemoveAt(0);
				}
				Transform val50 = val48.Find("None");
				if (list4.Count > 0)
				{
					if ((Object)(object)val50 != (Object)null)
					{
						((Component)val50).gameObject.SetActive(false);
					}
					if ((Object)(object)val41 != (Object)null)
					{
						foreach (ButtonInfo item10 in list4)
						{
							try
							{
								CreateButton(val48, item10, val41);
							}
							catch
							{
							}
						}
					}
				}
				else if ((Object)(object)val50 != (Object)null)
				{
					((Component)val50).gameObject.SetActive(true);
				}
			}
		}
		else if ((Object)(object)val32 != (Object)null)
		{
			((Component)val32).gameObject.SetActive(true);
			Transform obj19 = val32.Find("Search/Text Area/Placeholder");
			TextMeshProUGUI val51 = ((obj19 != null) ? ((Component)obj19).GetComponent<TextMeshProUGUI>() : null);
			if ((Object)(object)val51 != (Object)null)
			{
				((TMP_Text)(object)val51).SafeSetText("Search " + Buttons.CurrentCategoryName + "...");
				Main.FollowMenuSettings((TMP_Text)(object)val51);
				GTExt.GetOrAddComponent<UIColorChanger>(((Component)val51).gameObject).colors = Main.textColors[1];
			}
			Transform obj20 = val32.Find("Search/Text Area/Text");
			TextMeshProUGUI val52 = ((obj20 != null) ? ((Component)obj20).GetComponent<TextMeshProUGUI>() : null);
			if ((Object)(object)val52 != (Object)null)
			{
				Main.FollowMenuSettings((TMP_Text)(object)val52);
				GTExt.GetOrAddComponent<UIColorChanger>(((Component)val52).gameObject).colors = Main.textColors[1];
			}
			Transform val53 = val32.Find("Modules/Viewport/Content");
			if ((Object)(object)val53 != (Object)null)
			{
				foreach (Transform item11 in val53)
				{
					Transform val54 = item11;
					Object.Destroy((Object)(object)((Component)val54).gameObject);
				}
				Transform obj21 = ((Component)canvas).transform.Find("Main/Button");
				GameObject val55 = ((obj21 != null) ? ((Component)obj21).gameObject : null);
				if ((Object)(object)val55 == (Object)null)
				{
					LogManager.LogError("ClickGUI: Missing Button template at Main/Button");
				}
				else
				{
					IEnumerable<ButtonInfo> enumerable;
					switch (Buttons.CurrentCategoryName)
					{
					case "Favorite Mods":
						enumerable = from f in Main.favorites
							select Buttons.GetIndex(f) into b
							where b != null
							select b;
						searchBuiltAll = false;
						break;
					case "Enabled Mods":
						enumerable = Buttons.buttons[Buttons.CurrentCategoryIndex].Concat(from b in Buttons.buttons.SelectMany((ButtonInfo[] x) => x)
							where b != null && b.enabled && b.isTogglable
							select b);
						searchBuiltAll = false;
						break;
					case "Friends":
					{
						List<ButtonInfo> list6 = new List<ButtonInfo>();
						list6.AddRange(Buttons.buttons[Buttons.CurrentCategoryIndex].Where((ButtonInfo b) => b.buttonText == "Exit Friends"));
						if (FriendManager.instance?.Friends.friends != null && FriendManager.instance.Friends.friends.Count > 0)
						{
							foreach (KeyValuePair<string, FriendManager.FriendData.Friend> friend in FriendManager.instance.Friends.friends)
							{
								string currentName = friend.Value.currentName;
								bool online = friend.Value.online;
								list6.Add(new ButtonInfo
								{
									buttonText = currentName,
									overlapText = currentName + " " + (online ? "<color=green>●</color>" : "<color=red>●</color>"),
									isTogglable = false,
									toolTip = "Room: " + (friend.Value.currentRoom ?? "Unknown")
								});
							}
						}
						else
						{
							list6.Add(new ButtonInfo
							{
								buttonText = "No friends found.",
								label = true,
								isTogglable = false
							});
						}
						enumerable = list6;
						searchBuiltAll = false;
						break;
					}
					default:
						if (Main.isSearching && !StringUtils.IsNullOrEmpty(Main.keyboardInput))
						{
							List<ButtonInfo> list5 = new List<ButtonInfo>();
							for (int num4 = 0; num4 < Buttons.buttons.Length; num4++)
							{
								if (num4 == 0)
								{
									continue;
								}
								bool flag3 = Buttons.categoryNames[num4].Contains("Admin") || Buttons.categoryNames[num4] == "Mod Givers";
								bool flag4 = Buttons.categoryNames[num4] == "Detected Mods";
								if ((flag3 && !Main.isAdmin) || (flag4 && !Main.allowDetected))
								{
									continue;
								}
								ButtonInfo[] array5 = Buttons.buttons[num4];
								foreach (ButtonInfo buttonInfo3 in array5)
								{
									try
									{
										if (!buttonInfo3.detected || Main.allowDetected)
										{
											list5.Add(buttonInfo3);
										}
									}
									catch
									{
									}
								}
							}
							enumerable = list5;
							searchBuiltAll = true;
						}
						else
						{
							enumerable = Buttons.buttons[Buttons.CurrentCategoryIndex];
							searchBuiltAll = false;
						}
						break;
					}
					foreach (ButtonInfo item12 in enumerable)
					{
						try
						{
							CreateButton(val53, item12, val55);
						}
						catch (Exception ex)
						{
							LogManager.LogError("CreateButton failed for " + item12.buttonText + ": " + ex.Message);
						}
					}
				}
			}
			Transform obj23 = val32.Find("Search");
			TMP_InputField val56 = ((obj23 != null) ? ((Component)obj23).GetComponent<TMP_InputField>() : null);
			if ((Object)(object)val56 != (Object)null)
			{
				((UnityEventBase)val56.onSelect).RemoveAllListeners();
				((UnityEventBase)val56.onDeselect).RemoveAllListeners();
				((UnityEvent<string>)(object)val56.onSelect).AddListener((UnityAction<string>)delegate
				{
					if (!Main.isSearching)
					{
						Search();
					}
				});
			}
		}
		Canvas.ForceUpdateCanvases();
		if (Main.isSearching)
		{
			UpdateSearch();
		}
	}

	private static void CreateButton(Transform parent, ButtonInfo info, GameObject template)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Expected O, but got Unknown
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = Object.Instantiate<GameObject>(template, parent, false);
		val.SetActive(true);
		((Object)val).name = info.overlapText ?? info.buttonText;
		foreach (Transform item in val.transform)
		{
			Transform val2 = item;
			if (((Object)val2).name != "Title" && ((Object)val2).name != "ToolTip")
			{
				Object.Destroy((Object)(object)((Component)val2).gameObject);
			}
		}
		string text = ((info.isTogglable && Main.favorites.Contains(info.buttonText)) ? "<color=yellow>★</color> " : "");
		string text2 = info.overlapText ?? info.buttonText;
		string text3 = "";
		if (info.isTogglable)
		{
			text2 = text2.Replace(" <color=grey>[</color><color=green>ON</color><color=grey>]</color>", "").Replace(" <color=grey>[</color><color=red>OFF</color><color=grey>]</color>", "");
			text3 = (info.enabled ? " <color=grey>[</color><color=green>ON</color><color=grey>]</color>" : " <color=grey>[</color><color=red>OFF</color><color=grey>]</color>");
		}
		string text4 = text + text2 + text3;
		if (Main.inputTextColor != "green")
		{
			text4 = text4.Replace(" <color=grey>[</color><color=green>", " <color=grey>[</color><color=" + Main.inputTextColor + ">");
			text4 = text4.Replace("<color=green>ON</color>", "<color=" + Main.inputTextColor + ">ON</color>");
			text4 = text4.Replace("<color=red>OFF</color>", "<color=" + Main.inputTextColor + ">OFF</color>");
		}
		text4 = Main.FixTMProTags(text4);
		Transform obj = val.transform.Find("Title");
		TextMeshProUGUI val3 = ((obj != null) ? ((Component)obj).GetComponent<TextMeshProUGUI>() : null);
		if ((Object)(object)val3 != (Object)null)
		{
			((TMP_Text)(object)val3).SafeSetText(text4);
			Main.FollowMenuSettings((TMP_Text)(object)val3);
			((Graphic)val3).color = (info.enabled ? Color.black : Main.textColors[1].GetCurrentColor());
		}
		Transform obj2 = val.transform.Find("ToolTip");
		TextMeshProUGUI val4 = ((obj2 != null) ? ((Component)obj2).GetComponent<TextMeshProUGUI>() : null);
		if ((Object)(object)val4 != (Object)null)
		{
			string text5 = info.toolTip;
			if (!string.IsNullOrEmpty(text5))
			{
				if (Main.inputTextColor != "green")
				{
					text5 = text5.Replace("<color=green>", "<color=" + Main.inputTextColor + ">");
				}
				text5 = Main.FixTMProTags(text5);
				text5 = Main.FollowMenuSettings(text5);
				((TMP_Text)(object)val4).SafeSetText(text5);
				Main.FollowMenuSettings((TMP_Text)(object)val4);
				((Graphic)val4).color = Main.textColors[1].GetCurrentColor();
			}
			else
			{
				((TMP_Text)(object)val4).SafeSetText("");
			}
		}
		Image component = val.GetComponent<Image>();
		if ((Object)(object)component != (Object)null)
		{
			((Graphic)component).color = Main.buttonColors[info.enabled ? 1 : 0].GetCurrentColor();
		}
		if (info.label)
		{
			return;
		}
		Button orAddComponent = GTExt.GetOrAddComponent<Button>(val);
		((UnityEventBase)orAddComponent.onClick).RemoveAllListeners();
		GameObject capturedButton = val;
		((UnityEvent)orAddComponent.onClick).AddListener((UnityAction)delegate
		{
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			Main.Toggle(info);
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
			if (!info.isTogglable && info.method != null)
			{
				InitializeClickGUI();
			}
			else
			{
				Image component2 = capturedButton.GetComponent<Image>();
				if ((Object)(object)component2 != (Object)null)
				{
					((Graphic)component2).color = Main.buttonColors[info.enabled ? 1 : 0].GetCurrentColor();
				}
				string text6 = ((info.isTogglable && Main.favorites.Contains(info.buttonText)) ? "<color=yellow>★</color> " : "");
				string text7 = info.overlapText ?? info.buttonText;
				string text8 = "";
				if (info.isTogglable)
				{
					text7 = text7.Replace(" <color=grey>[</color><color=green>ON</color><color=grey>]</color>", "").Replace(" <color=grey>[</color><color=red>OFF</color><color=grey>]</color>", "");
					text8 = (info.enabled ? " <color=grey>[</color><color=green>ON</color><color=grey>]</color>" : " <color=grey>[</color><color=red>OFF</color><color=grey>]</color>");
				}
				string text9 = text6 + text7 + text8;
				if (Main.inputTextColor != "green")
				{
					text9 = text9.Replace(" <color=grey>[</color><color=green>", " <color=grey>[</color><color=" + Main.inputTextColor + ">");
					text9 = text9.Replace("<color=green>ON</color>", "<color=" + Main.inputTextColor + ">ON</color>");
					text9 = text9.Replace("<color=red>OFF</color>", "<color=" + Main.inputTextColor + ">OFF</color>");
				}
				text9 = Main.FixTMProTags(text9);
				Transform obj3 = capturedButton.transform.Find("Title");
				TextMeshProUGUI val5 = ((obj3 != null) ? ((Component)obj3).GetComponent<TextMeshProUGUI>() : null);
				if ((Object)(object)val5 != (Object)null)
				{
					((TMP_Text)(object)val5).SafeSetText(text9);
					Main.FollowMenuSettings((TMP_Text)(object)val5);
					((Graphic)val5).color = (info.enabled ? Color.black : Main.textColors[1].GetCurrentColor());
				}
				if (Main.isSearching)
				{
					UpdateSearch();
				}
			}
		});
	}

	public static void UpdateSearch()
	{
		try
		{
			Canvas obj = canvas;
			Transform val = ((obj != null) ? ((Component)obj).transform.Find("Main/ModuleTab/Modules/Viewport/Content") : null);
			if ((Object)(object)val == (Object)null)
			{
				return;
			}
			string text = Main.keyboardInput?.Replace(" ", "").ToLower() ?? "";
			if (text == lastSearchText)
			{
				return;
			}
			lastSearchText = text;
			Canvas obj2 = canvas;
			Transform val2 = ((obj2 != null) ? ((Component)obj2).transform.Find("Main/ModuleTab/Search") : null);
			if ((Object)(object)val2 != (Object)null)
			{
				TMP_InputField component = ((Component)val2).GetComponent<TMP_InputField>();
				if ((Object)(object)component != (Object)null)
				{
					component.text = Main.keyboardInput;
				}
			}
			foreach (GameObject item in val.Children())
			{
				if (!((Object)(object)item == (Object)null))
				{
					item.SetActive(text == "" || ((Object)item).name.ClearTags().Replace(" ", "").ToLower()
						.Contains(text));
				}
			}
		}
		catch
		{
		}
	}

	public static void ClickGUI()
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Expected O, but got Unknown
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Expected O, but got Unknown
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Expected O, but got Unknown
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)Main.menu == (Object)null)
		{
			ResetClickGUIInput();
		}
		else
		{
			if (!XRSettings.isDeviceActive || (Object)(object)canvas == (Object)null)
			{
				return;
			}
			Transform val = ((Component)canvas).transform.Find("Main/Sidebar/Watermark");
			if ((Object)(object)val != (Object)null)
			{
				val.localRotation = Quaternion.Euler(0f, 0f, Main.rockWatermark ? (Mathf.Sin(Time.time * 2f) * 10f) : 0f);
			}
			Camera val2 = (((Object)(object)canvas.worldCamera != (Object)null) ? canvas.worldCamera : Camera.main);
			if ((Object)(object)val2 == (Object)null)
			{
				return;
			}
			if ((Object)(object)canvas.worldCamera == (Object)null)
			{
				canvas.worldCamera = val2;
			}
			if ((Object)(object)clickGuiLine == (Object)null)
			{
				clickGuiLine = GTExt.GetOrAddComponent<LineRenderer>(new GameObject("Seralyth_ClickGUILine"));
				((Renderer)clickGuiLine).material = new Material(Shader.Find("GUI/Text Shader"));
				clickGuiLine.startWidth = 0.025f * (Main.scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
				clickGuiLine.endWidth = clickGuiLine.startWidth;
				clickGuiLine.useWorldSpace = true;
				clickGuiLine.positionCount = 2;
				if (Main.smoothLines)
				{
					clickGuiLine.numCapVertices = 10;
					clickGuiLine.numCornerVertices = 5;
				}
			}
			clickGuiLine.startColor = Main.backgroundColor.GetCurrentColor();
			clickGuiLine.endColor = Main.backgroundColor.GetCurrentColor(0.5f);
			clickGuiLine.startWidth = 0.025f * (Main.scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
			clickGuiLine.endWidth = clickGuiLine.startWidth;
			GraphicRaycaster component = ((Component)canvas).GetComponent<GraphicRaycaster>();
			if (eventSystem == null)
			{
				eventSystem = EventSystem.current;
			}
			if (pointerData == null)
			{
				pointerData = new PointerEventData(eventSystem);
			}
			bool flag = Main.rightHand || (Main.bothHands && ((ControllerInputPoller)ControllerInputPoller.instance).rightControllerSecondaryButton);
			Vector3 val3 = (flag ? ControllerUtilities.GetTrueLeftHand().forward : ControllerUtilities.GetTrueRightHand().forward);
			Vector3 val4 = (flag ? GorillaTagger.Instance.leftHandTransform.position : GorillaTagger.Instance.rightHandTransform.position);
			Vector3 normalized = ((Vector3)(ref val3)).normalized;
			Vector3 val5 = val4 + normalized * 5f;
			Vector3 val6 = val5;
			RectTransform component2 = ((Component)canvas).GetComponent<RectTransform>();
			Plane val7 = default(Plane);
			((Plane)(ref val7))._002Ector(((Transform)component2).forward, ((Transform)component2).position);
			Ray val8 = default(Ray);
			((Ray)(ref val8))._002Ector(val4, normalized);
			float num = default(float);
			if (((Plane)(ref val7)).Raycast(val8, ref num) && num > 0f)
			{
				val6 = ((Ray)(ref val8)).GetPoint(num);
			}
			Vector3 val9 = val2.WorldToScreenPoint(val6);
			if (val9.z < 0f)
			{
				currentUI = null;
				clickGuiLine.SetPosition(0, val4);
				clickGuiLine.SetPosition(1, val5);
				ResetClickGUIInput();
				return;
			}
			pointerData.position = Vector2.op_Implicit(val9);
			uiResults.Clear();
			((BaseRaycaster)component).Raycast(pointerData, uiResults);
			object obj;
			if (uiResults.Count <= 0)
			{
				obj = null;
			}
			else
			{
				RaycastResult val10 = uiResults[0];
				obj = ((RaycastResult)(ref val10)).gameObject;
			}
			currentUI = (GameObject)obj;
			Vector3 val11 = (((Object)(object)currentUI != (Object)null) ? uiResults[0].worldPosition : val5);
			clickGuiLine.SetPosition(0, val4);
			clickGuiLine.SetPosition(1, val11);
			bool flag2 = (flag ? (Main.leftTrigger > 0.5f) : (Main.rightTrigger > 0.5f));
			Vector2 position = pointerData.position;
			pointerData.delta = position - lastPointerPos;
			lastPointerPos = position;
			if (flag2 && !lastTriggerClick && (Object)(object)currentUI != (Object)null)
			{
				pressedUI = GetClickableTarget(currentUI);
				pointerData.pressPosition = position;
				pointerData.pointerPressRaycast = uiResults[0];
				ExecuteEvents.Execute<IPointerDownHandler>(pressedUI, (BaseEventData)(object)pointerData, ExecuteEvents.pointerDownHandler);
				pointerData.pointerPress = pressedUI;
				isDragging = false;
				draggedUI = GetDragTarget(currentUI);
				pointerData.pointerDrag = draggedUI ?? null;
			}
			if (flag2)
			{
				if ((Object)(object)draggedUI != (Object)null)
				{
					if (!isDragging && Vector2.Distance(pointerData.pressPosition, position) > 15f)
					{
						isDragging = true;
						ExecuteEvents.Execute<IBeginDragHandler>(draggedUI, (BaseEventData)(object)pointerData, ExecuteEvents.beginDragHandler);
						if ((Object)(object)pressedUI != (Object)null && (Object)(object)pressedUI != (Object)(object)draggedUI)
						{
							ExecuteEvents.Execute<IPointerUpHandler>(pressedUI, (BaseEventData)(object)pointerData, ExecuteEvents.pointerUpHandler);
							pointerData.pointerPress = null;
						}
					}
					if (isDragging)
					{
						ExecuteEvents.Execute<IDragHandler>(draggedUI, (BaseEventData)(object)pointerData, ExecuteEvents.dragHandler);
					}
				}
			}
			else if (lastTriggerClick)
			{
				if ((Object)(object)pressedUI != (Object)null && !isDragging)
				{
					ExecuteEvents.Execute<IPointerUpHandler>(pressedUI, (BaseEventData)(object)pointerData, ExecuteEvents.pointerUpHandler);
					ExecuteEvents.Execute<IPointerClickHandler>(pressedUI, (BaseEventData)(object)pointerData, ExecuteEvents.pointerClickHandler);
				}
				else if ((Object)(object)pressedUI != (Object)null)
				{
					ExecuteEvents.Execute<IPointerUpHandler>(pressedUI, (BaseEventData)(object)pointerData, ExecuteEvents.pointerUpHandler);
				}
				if (isDragging && (Object)(object)draggedUI != (Object)null)
				{
					ExecuteEvents.Execute<IEndDragHandler>(draggedUI, (BaseEventData)(object)pointerData, ExecuteEvents.endDragHandler);
				}
				pressedUI = null;
				draggedUI = null;
				pointerData.pointerDrag = null;
				pointerData.pointerPress = null;
				isDragging = false;
			}
			lastTriggerClick = flag2;
			bool flag3 = XRSettings.isDeviceActive && ((ControllerInputPoller)ControllerInputPoller.instance).rightControllerPrimaryButton;
			if (flag3 && !lastRightPrimary && Main.isSearching)
			{
				Search();
			}
			lastRightPrimary = flag3;
		}
	}

	public static void PlayerSelect()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		if (!XRSettings.isDeviceActive)
		{
			return;
		}
		bool flag = Main.rightHand || (Main.bothHands && ((ControllerInputPoller)ControllerInputPoller.instance).rightControllerSecondaryButton);
		Vector3 val = (flag ? ControllerUtilities.GetTrueLeftHand().forward : ControllerUtilities.GetTrueRightHand().forward);
		if (NetworkSystem.Instance.InRoom && (Object)(object)Main.menu != (Object)null && (Object)(object)Main.reference != (Object)null && Vector3.Distance(Main.menu.transform.position, Main.reference.transform.position) > 0.5f)
		{
			if ((Object)(object)selectObject == (Object)null)
			{
				selectObject = new GameObject("Seralyth_PingLine");
			}
			Color val2 = (Buttons.GetIndex("Swap GUI Colors").enabled ? Main.buttonColors[1].GetCurrentColor() : Main.backgroundColor.GetCurrentColor());
			Color val3 = val2;
			val3.a = 0.15f;
			LineRenderer orAddComponent = GTExt.GetOrAddComponent<LineRenderer>(selectObject);
			((Renderer)orAddComponent).material.shader = Shader.Find("GUI/Text Shader");
			orAddComponent.startColor = val3;
			orAddComponent.endColor = val3;
			orAddComponent.startWidth = 0.025f * (Main.scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
			orAddComponent.endWidth = 0.025f * (Main.scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
			orAddComponent.positionCount = 2;
			orAddComponent.useWorldSpace = true;
			if (Main.smoothLines)
			{
				orAddComponent.numCapVertices = 10;
				orAddComponent.numCornerVertices = 5;
			}
			Vector3 val4 = (flag ? GorillaTagger.Instance.leftHandTransform.position : GorillaTagger.Instance.rightHandTransform.position);
			Vector3 val5 = val;
			RaycastHit val6 = default(RaycastHit);
			Physics.SphereCast(val4 + val5 / 4f * (Main.scaleWithPlayer ? GTPlayer.Instance.scale : 1f), 0.15f, val5, ref val6, 512f, Main.NoInvisLayerMask());
			Vector3 val7 = ((((RaycastHit)(ref val6)).point == Vector3.zero) ? (val4 + val5 * 512f) : ((RaycastHit)(ref val6)).point);
			orAddComponent.SetPosition(0, val4);
			orAddComponent.SetPosition(1, val7);
			VRRig componentInParent = ((Component)((RaycastHit)(ref val6)).collider).GetComponentInParent<VRRig>();
			if ((Object)(object)((RaycastHit)(ref val6)).collider != (Object)null && (Object)(object)componentInParent != (Object)null && !componentInParent.IsLocal())
			{
				if ((Object)(object)lastTarget != (Object)null && (Object)(object)lastTarget != (Object)(object)componentInParent)
				{
					((Renderer)lastTarget.mainSkin).material.shader = Shader.Find("GorillaTag/UberShader");
					if (((Object)((Renderer)lastTarget.mainSkin).material).name.Contains("gorilla_body"))
					{
						((Renderer)lastTarget.mainSkin).material.color = lastTarget.playerColor;
					}
					lastTarget = null;
				}
				if ((Object)(object)lastTarget == (Object)null)
				{
					Visuals.FixRigMaterialESPColors(componentInParent);
					((Renderer)componentInParent.mainSkin).material.shader = Shader.Find("GUI/Text Shader");
					((Renderer)componentInParent.mainSkin).material.color = val2;
					GorillaTagger.Instance.StartVibration(flag, GorillaTagger.Instance.tagHapticStrength / 2f, 0.05f);
					lastTarget = componentInParent;
				}
				else
				{
					((Renderer)lastTarget.mainSkin).material.color = val2;
				}
				bool flag2 = (flag ? (Main.leftTrigger > 0.5f) : (Main.rightTrigger > 0.5f));
				if (flag2 && !lastTriggerSelect)
				{
					VRRig.LocalRig.PlayHandTapLocal(50, flag, 0.4f);
					GorillaTagger.Instance.StartVibration(flag, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
					NavigatePlayer(RigUtilities.GetPlayerFromVRRig(componentInParent));
					Main.ReloadMenu();
					NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Selected player " + RigUtilities.GetPlayerFromVRRig(componentInParent).NickName + ".");
				}
				lastTriggerSelect = flag2;
			}
			else if ((Object)(object)lastTarget != (Object)null)
			{
				((Renderer)lastTarget.mainSkin).material.shader = Shader.Find("GorillaTag/UberShader");
				if (((Object)((Renderer)lastTarget.mainSkin).material).name.Contains("gorilla_body"))
				{
					((Renderer)lastTarget.mainSkin).material.color = lastTarget.playerColor;
				}
				lastTarget = null;
			}
			return;
		}
		if ((Object)(object)selectObject != (Object)null)
		{
			Object.Destroy((Object)(object)selectObject);
			selectObject = null;
		}
		if ((Object)(object)lastTarget != (Object)null)
		{
			((Renderer)lastTarget.mainSkin).material.shader = Shader.Find("GorillaTag/UberShader");
			if (((Object)((Renderer)lastTarget.mainSkin).material).name.Contains("gorilla_body"))
			{
				((Renderer)lastTarget.mainSkin).material.color = lastTarget.playerColor;
			}
			lastTarget = null;
		}
		lastTriggerSelect = false;
	}

	public static IEnumerator MenuIntroCoroutine()
	{
		if (Time.time < Main.timeMenuStarted)
		{
			yield return (object)new WaitForSeconds(1f);
		}
		float fps = 1f / Time.unscaledDeltaTime;
		yield return (object)new WaitUntil((Func<bool>)delegate
		{
			fps = Mathf.Lerp(fps, 1f / Time.unscaledDeltaTime, 0.1f);
			return fps > 30f;
		});
		GameObject menuIntro = AssetUtilities.LoadObject<GameObject>("Intro");
		menuIntro.transform.position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
		menuIntro.transform.rotation = ((Component)GorillaTagger.Instance.bodyCollider).transform.rotation;
		VideoPlayer videoPlayer = ((Component)menuIntro.transform.Find("Video")).GetComponent<VideoPlayer>();
		ParticleSystem particleSystem = ((Component)menuIntro.transform.Find("Particles")).GetComponent<ParticleSystem>();
		Color backgroundColor = Color.white;
		Fun.HueShift(Color.white);
		MainModule main = particleSystem.main;
		((MainModule)(ref main)).startColor = new MinMaxGradient(Main.backgroundColor.GetColor(0));
		float timeout = 0f;
		while (!videoPlayer.isPrepared)
		{
			timeout += Time.deltaTime;
			if (timeout > 5f)
			{
				EndImmediately();
				yield break;
			}
			yield return null;
		}
		bool videoEnded = false;
		videoPlayer.Play();
		videoPlayer.loopPointReached += (EventHandler)delegate
		{
			videoEnded = true;
		};
		yield return (object)new WaitUntil((Func<bool>)(() => videoEnded));
		float fadeEnd = Time.time + 1f;
		Color transparentColor = backgroundColor;
		transparentColor.a = 0f;
		while (Time.time < fadeEnd)
		{
			float t = 1f - (fadeEnd - Time.time);
			Fun.HueShift(Color.Lerp(backgroundColor, transparentColor, t));
			((Component)videoPlayer).gameObject.GetComponent<Renderer>().material.color = Color.Lerp(Color.white, Color.clear, t);
			MinMaxGradient startColor = ((MainModule)(ref main)).startColor;
			((MainModule)(ref main)).startColor = new MinMaxGradient(Color.Lerp(((MinMaxGradient)(ref startColor)).color, Color.clear, t));
			yield return null;
		}
		EndImmediately();
		void EndImmediately()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			Fun.HueShift(Color.clear);
			Object.Destroy((Object)(object)menuIntro);
		}
	}

	public static void MenuIntro()
	{
		((MonoBehaviour)CoroutineManager.instance).StartCoroutine(MenuIntroCoroutine());
	}

	public static void ResetVoiceCommandsKeywords()
	{
		if (!File.Exists("SeralythMenu/Seralyth_Keywords.txt"))
		{
			File.WriteAllLines("SeralythMenu/Seralyth_Keywords.txt", keyWords);
		}
	}

	public static void ResetSystemPrompt()
	{
		if (!File.Exists("SeralythMenu/Seralyth_SystemPrompt.txt"))
		{
			File.WriteAllText("SeralythMenu/Seralyth_SystemPrompt.txt", AIManager.SystemPrompt);
		}
	}

	public static string SavePreferencesToText()
	{
		string text = ";;";
		string text2 = "";
		ButtonInfo[][] buttons = Buttons.buttons;
		foreach (ButtonInfo[] array in buttons)
		{
			ButtonInfo[] array2 = array;
			foreach (ButtonInfo buttonInfo in array2)
			{
				if (!buttonInfo.detected && buttonInfo.enabled && buttonInfo.buttonText != "Save Preferences")
				{
					text2 = ((!(text2 == "")) ? (text2 + text + buttonInfo.buttonText) : (text2 + buttonInfo.buttonText));
				}
			}
		}
		string text3 = "";
		foreach (string favorite in Main.favorites)
		{
			text3 = ((!(text3 == "")) ? (text3 + text + favorite) : (text3 + favorite));
		}
		string[] obj = new string[79]
		{
			Movement.platformMode.ToString(),
			Movement.platformShape.ToString(),
			Movement.flySpeedCycle.ToString(),
			Movement.longarmCycle.ToString(),
			Movement.speedboostCycle.ToString(),
			Projectiles.projMode.ToString(),
			Movement.timerPowerIndex.ToString(),
			Projectiles.shootCycle.ToString(),
			Main.pointerIndex.ToString(),
			Advantages.tagAuraIndex.ToString(),
			Main.notificationDecayTime.ToString(),
			fontStyleType.ToString(),
			Main.arrowType.ToString(),
			Main.pcbg.ToString(),
			Important.reconnectDelay.ToString(),
			Safety.fpsSpoofValue.ToString(),
			SoundManager.DefaultSounds["Button"],
			Main.buttonClickVolume.ToString(),
			Safety.antiReportRangeIndex.ToString(),
			Advantages.tagRangeIndex.ToString(),
			Sound.BindMode.ToString(),
			Movement.driveInt.ToString(),
			langInd.ToString(),
			inputTextColorInt.ToString(),
			Movement.pullPowerInt.ToString(),
			SoundManager.DefaultSounds["Notification"],
			Visuals.PerformanceModeStepIndex.ToString(),
			Main.gunVariation.ToString(),
			Main.GunDirection.ToString(),
			Main.narratorIndex.ToString(),
			Movement.predInt.ToString(),
			gunLineQualityIndex.ToString(),
			Projectiles.projDebounceIndex.ToString(),
			Projectiles.red.ToString(),
			Projectiles.green.ToString(),
			Projectiles.blue.ToString(),
			Safety.rankIndex.ToString(),
			Overpowered.snowballScale.ToString(),
			Overpowered.lagIndex.ToString(),
			Fun.blockDebounceIndex.ToString(),
			Fun.nameCycleIndex.ToString(),
			menuScaleIndex.ToString(),
			Sound.soundId.ToString(),
			Fun.targetQuestScore.ToString(),
			notificationScaleIndex.ToString(),
			overlayScaleIndex.ToString(),
			arraylistScaleIndex.ToString(),
			((int)MathF.Ceiling(Main.playTime)).ToString(),
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null
		};
		Player localPlayer = PhotonNetwork.LocalPlayer;
		obj[48] = ((localPlayer != null) ? localPlayer.UserId : null) ?? "null";
		obj[49] = Main._pageSize.ToString();
		obj[50] = Overpowered.snowballMultiplicationFactor.ToString();
		obj[51] = Main.menuButtonIndex.ToString();
		obj[52] = Safety.targetElo.ToString();
		obj[53] = Safety.targetBadge.ToString();
		obj[54] = Movement.playspaceAbuseIndex.ToString();
		obj[55] = Movement.wallWalkStrengthIndex.ToString();
		obj[56] = Fun.headSpinIndex.ToString();
		obj[57] = Movement.macroPlaybackRangeIndex.ToString();
		obj[58] = Main.joystickMenuPosition.ToString();
		obj[59] = Movement.multiplicationAmount.ToString();
		obj[60] = Fun.targetFOV.ToString();
		obj[61] = Projectiles.targetProjectileIndex.ToString();
		obj[62] = Movement.fakeLagDelayIndex.ToString();
		obj[63] = Projectiles.snowballIndex.ToString();
		obj[64] = Main.characterDistance.ToString();
		obj[65] = Overpowered.lagTypeIndex.ToString();
		obj[66] = Overpowered.masterVisualizationType.ToString();
		obj[67] = Movement.targetHz.ToString();
		obj[68] = Safety.pingSpoofValue.ToString();
		obj[69] = Fun.soundboardVolumeIndex.ToString();
		obj[70] = Fun.soundboardSpeedIndex.ToString();
		obj[71] = SoundManager.DefaultSoundpack;
		obj[72] = Sound.disableLocalSoundboard.ToString();
		obj[73] = StumpUpdateDisplay.AutoScrollEnabled.ToString();
		obj[74] = StumpUpdateDisplay.LastSeenDllTimestamp.ToString();
		obj[75] = Main.GunLibLine.ToString();
		obj[76] = Main.GunLibTrail.ToString();
		obj[77] = Main.GunLibShape.ToString();
		obj[78] = Main.categoryDisplayMode.ToString();
		string[] value = obj;
		string text4 = string.Join(text, value);
		string text5 = "";
		foreach (KeyValuePair<string, List<string>> modBinding in Main.ModBindings)
		{
			if (text5 != "")
			{
				text5 += "~~";
			}
			string text6 = modBinding.Key;
			foreach (string item in modBinding.Value)
			{
				text6 = text6 + text + item;
			}
			text5 += text6;
		}
		string text7 = string.Join(text, Main.quickActions);
		string text8 = "";
		ButtonInfo[][] buttons2 = Buttons.buttons;
		foreach (ButtonInfo[] array3 in buttons2)
		{
			ButtonInfo[] array4 = array3;
			foreach (ButtonInfo buttonInfo2 in array4)
			{
				if (buttonInfo2.rebindKey != null || buttonInfo2.pcBindKey != null)
				{
					string text9 = buttonInfo2.buttonText + ";" + buttonInfo2.rebindKey + ";" + buttonInfo2.pcBindKey;
					text8 = ((!(text8 == "")) ? (text8 + text + text9) : (text8 + text9));
				}
			}
		}
		string text10 = string.Join(text, Main.skipButtons);
		return text2 + "\n" + text3 + "\n" + text4 + "\n" + Main.pageButtonType + "\n" + Main.themeType + "\n" + Main.fontCycle + "\n" + text5 + "\n" + text7 + "\n" + text8 + "\n" + text10;
	}

	public static void SavePreferences()
	{
		LogManager.Log("Saving menuButtonIndex: " + Main.menuButtonIndex);
		File.WriteAllText("SeralythMenu/Seralyth_Preferences.txt", SavePreferencesToText());
	}

	public static void LoadPreferencesFromText(string text)
	{
		loadingPreferencesFrame = Time.frameCount;
		isLoadingPreferences = true;
		Panic();
		string[] array = text.Split("\n");
		string[] array2 = array[0].Split(";;");
		for (int i = 0; i < array2.Length; i++)
		{
			Main.Toggle(array2[i]);
		}
		string[] array3 = array[1].Split(";;");
		Main.favorites.Clear();
		string[] array4 = array3;
		foreach (string item in array4)
		{
			Main.favorites.Add(item);
		}
		string[] array5 = array[2].Split(";;");
		try
		{
			Movement.platformMode = int.Parse(array5[0]);
			Movement.ChangePlatformType();
			Movement.platformShape = int.Parse(array5[1]);
			Movement.ChangePlatformShape();
			Movement.flySpeedCycle = int.Parse(array5[2]);
			Movement.ChangeFlySpeed();
			Movement.longarmCycle = int.Parse(array5[3]);
			Movement.ChangeArmLength();
			Movement.speedboostCycle = int.Parse(array5[4]);
			Movement.ChangeSpeedBoostAmount();
			Projectiles.projMode = int.Parse(array5[5]);
			Projectiles.ChangeProjectile();
			Movement.timerPowerIndex = int.Parse(array5[6]);
			Movement.ChangeTimerSpeed();
			Projectiles.shootCycle = int.Parse(array5[7]);
			Projectiles.ChangeShootSpeed();
			Main.pointerIndex = int.Parse(array5[8]);
			ChangePointerPosition();
			Advantages.tagAuraIndex = int.Parse(array5[9]);
			Advantages.ChangeTagAuraRange();
			Main.notificationDecayTime = int.Parse(array5[10]);
			ChangeNotificationTime();
			fontStyleType = int.Parse(array5[11]);
			ChangeFontStyleType();
			Main.arrowType = int.Parse(array5[12]);
			ChangeArrowType();
			Main.pcbg = int.Parse(array5[13]);
			ChangePCUI();
			Important.reconnectDelay = int.Parse(array5[14]);
			ChangeReconnectTime();
			Safety.fpsSpoofValue = (string.IsNullOrWhiteSpace(array5[15]) ? 85 : int.Parse(array5[15]));
			Safety.ChangeFPSSpoofValue();
			SoundManager.DefaultSounds["Button"] = array5[16];
			Buttons.GetIndex("Change Button Sound").overlapText = "Change Button Sound <color=grey>[</color><color=green>" + SoundManager.DefaultSounds["Button"] + "</color><color=grey>]</color>";
			Main.buttonClickVolume = int.Parse(array5[17]);
			ChangeButtonVolume();
			Safety.antiReportRangeIndex = int.Parse(array5[18]);
			Safety.ChangeAntiReportRange();
			Advantages.tagRangeIndex = int.Parse(array5[19]);
			Advantages.ChangeTagReachDistance();
			Sound.BindMode = int.Parse(array5[20]);
			Sound.SoundBindings();
			Movement.driveInt = int.Parse(array5[21]);
			Movement.ChangeDriveSpeed();
			langInd = int.Parse(array5[22]);
			ChangeMenuLanguage();
			inputTextColorInt = int.Parse(array5[23]);
			ChangeInputTextColor();
			Movement.pullPowerInt = int.Parse(array5[24]);
			Movement.ChangePullModPower();
			SoundManager.DefaultSounds["Notification"] = array5[25];
			Buttons.GetIndex("Change Notification Sound").overlapText = "Change Notification Sound <color=grey>[</color><color=green>" + SoundManager.DefaultSounds["Notification"] + "</color><color=grey>]</color>";
			Visuals.PerformanceModeStepIndex = int.Parse(array5[26]);
			Visuals.ChangePerformanceModeVisualStep();
			Main.gunVariation = int.Parse(array5[27]);
			ChangeGunVariation();
			Main.GunDirection = int.Parse(array5[28]);
			ChangeGunDirection();
			Main.narratorIndex = int.Parse(array5[29]);
			ChangeNarrationVoice();
			Movement.predInt = int.Parse(array5[30]);
			Movement.ChangePredictionAmount();
			gunLineQualityIndex = int.Parse(array5[31]);
			ChangeGunLineQuality();
			Projectiles.projDebounceIndex = int.Parse(array5[32]);
			Projectiles.ChangeProjectileDelay();
			Projectiles.red = int.Parse(array5[33]);
			Projectiles.IncreaseRed();
			Projectiles.green = int.Parse(array5[34]);
			Projectiles.IncreaseGreen();
			Projectiles.blue = int.Parse(array5[35]);
			Projectiles.IncreaseBlue();
			Safety.rankIndex = int.Parse(array5[36]);
			Safety.ChangeRankedTier();
			Overpowered.snowballScale = int.Parse(array5[37]);
			Overpowered.ChangeSnowballScale();
			Overpowered.lagIndex = int.Parse(array5[38]);
			Overpowered.ChangeLagPower();
			Fun.blockDebounceIndex = int.Parse(array5[39]);
			Fun.ChangeBlockDelay();
			Fun.nameCycleIndex = int.Parse(array5[40]);
			menuScaleIndex = int.Parse(array5[41]);
			ChangeMenuScale();
			Sound.soundId = int.Parse(array5[42]);
			Sound.IncreaseSoundID();
			Fun.targetQuestScore = int.Parse(array5[43]);
			Fun.ChangeCustomQuestScore();
			notificationScaleIndex = int.Parse(array5[44]);
			ChangeNotificationScale();
			overlayScaleIndex = int.Parse(array5[45]);
			ChangeOverlayScale();
			arraylistScaleIndex = int.Parse(array5[46]);
			ChangeArraylistScale();
			Main.playTime = int.Parse(array5[47]);
			Important.oldId = array5[48];
			Main._pageSize = int.Parse(array5[49]);
			ChangePageSize();
			Overpowered.snowballMultiplicationFactor = int.Parse(array5[50]);
			Overpowered.ChangeSnowballMultiplicationFactor();
			Safety.targetElo = int.Parse(array5[52]);
			Safety.ChangeELOValue();
			Safety.targetBadge = int.Parse(array5[53]);
			Safety.ChangeBadgeTier();
			Movement.playspaceAbuseIndex = int.Parse(array5[54]);
			Movement.ChangePlayspaceAbuseSpeed();
			Movement.wallWalkStrengthIndex = int.Parse(array5[55]);
			Movement.ChangeWallWalkStrength();
			Fun.headSpinIndex = int.Parse(array5[56]);
			Fun.ChangeHeadSpinSpeed();
			Movement.macroPlaybackRangeIndex = int.Parse(array5[57]);
			Movement.ChangeMacroPlaybackRange();
			Main.joystickMenuPosition = int.Parse(array5[58]);
			ChangeJoystickMenuPosition();
			Movement.multiplicationAmount = int.Parse(array5[59]);
			Movement.MultiplicationAmount();
			Fun.targetFOV = int.Parse(array5[60]);
			Fun.ChangeTargetFOV();
			Projectiles.targetProjectileIndex = int.Parse(array5[61]);
			Projectiles.ChangeProjectileIndex();
			Movement.fakeLagDelayIndex = int.Parse(array5[62]);
			Movement.ChangeFakeLagStrength();
			Projectiles.snowballIndex = int.Parse(array5[63]);
			Projectiles.ChangeGrowingProjectile();
			Main.characterDistance = int.Parse(array5[64]);
			ChangeCharacterDistance();
			Overpowered.lagTypeIndex = int.Parse(array5[65]);
			Overpowered.ChangeLagType();
			Overpowered.masterVisualizationType = int.Parse(array5[66]);
			Overpowered.MasterVisualizationType();
			Movement.targetHz = int.Parse(array5[67]);
			Movement.ChangeTinnitusHz();
			Safety.pingSpoofValue = int.Parse(array5[68]);
			Safety.ChangePingSpoofValue();
			Fun.soundboardVolumeIndex = float.Parse(array5[69]);
			Fun.ChangeSoundboardVolume();
			Fun.soundboardSpeedIndex = float.Parse(array5[70]);
			Fun.ChangeSoundboardSpeed();
			SoundManager.DefaultSoundpack = array5[71];
			Buttons.GetIndex("Change Menu Soundpack").overlapText = "Change Menu Soundpack <color=grey>[</color><color=green>" + SoundManager.DefaultSoundpack + "</color><color=grey>]</color>";
			Sound.disableLocalSoundboard = bool.Parse(array5[72]);
			if (array5.Length > 73)
			{
				StumpUpdateDisplay.AutoScrollEnabled = bool.Parse(array5[73]);
			}
			if (array5.Length > 74 && long.TryParse(array5[74], out var result))
			{
				StumpUpdateDisplay.LastSeenDllTimestamp = result;
			}
			if (array5.Length > 75)
			{
				Main.GunLibLine = bool.Parse(array5[75]);
			}
			if (array5.Length > 76)
			{
				Main.GunLibTrail = bool.Parse(array5[76]);
			}
			if (array5.Length > 77)
			{
				Main.GunLibShape = int.Parse(array5[77]);
				ChangeGunLibShape();
			}
			if (array5.Length > 78)
			{
				Main.categoryDisplayMode = int.Parse(array5[78]);
				ChangeCategoryDisplay();
			}
		}
		catch
		{
			LogManager.Log("Save file out of date");
		}
		if (int.TryParse(array5[51], out var result2))
		{
			Main.menuButtonIndex = result2;
		}
		else
		{
			LogManager.Log("Failed to parse menuButtonIndex from data[51]: " + ((array5.Length > 51) ? array5[51] : "MISSING"));
		}
		string[] array6 = new string[5] { "Primary", "Secondary", "Grip", "Trigger", "Joystick" };
		if (Main.menuButtonIndex >= 0 && Main.menuButtonIndex < array6.Length)
		{
			Buttons.GetIndex("Change Menu Button").overlapText = "Change Menu Button <color=grey>[</color><color=green>" + array6[Main.menuButtonIndex] + "</color><color=grey>]</color>";
		}
		else
		{
			Main.menuButtonIndex = 1;
			Buttons.GetIndex("Change Menu Button").overlapText = "Change Menu Button <color=grey>[</color><color=green>Secondary</color><color=grey>]</color>";
		}
		LogManager.Log("Loaded menuButtonIndex: " + Main.menuButtonIndex + " from data[51]: " + ((array5.Length > 51) ? array5[51] : "MISSING"));
		Main.pageButtonType = int.Parse(array[3]);
		Main.Toggle("Change Page Type");
		Main.themeType = int.Parse(array[4]);
		Main.Toggle("Change Menu Theme");
		Main.fontCycle = int.Parse(array[5]);
		Main.Toggle("Change Font Type");
		try
		{
			string[] array7 = array[6].Split("~~");
			foreach (string text2 in array7)
			{
				if (!text2.Contains(";;"))
				{
					continue;
				}
				string[] array8 = text2.Split(";;");
				string key = array8[0];
				List<string> list = new List<string>();
				for (int l = 1; l < array8.Length; l++)
				{
					string text3 = array8[l];
					if (Buttons.GetIndex(text3) != null)
					{
						list.Add(text3);
					}
				}
				Main.ModBindings[key] = list;
			}
		}
		catch
		{
		}
		try
		{
			Main.quickActions.Clear();
			string[] array9 = array[7].Split(";;");
			foreach (string text4 in array9)
			{
				ButtonInfo index = Buttons.GetIndex(text4);
				if (index != null)
				{
					Main.quickActions.Add(text4);
				}
			}
		}
		catch
		{
		}
		try
		{
			string[] array10 = array[8].Split(";;");
			foreach (string text5 in array10)
			{
				string[] array11 = text5.Split(";");
				string buttonText = array11[0];
				ButtonInfo index2 = Buttons.GetIndex(buttonText);
				if (index2 != null)
				{
					if (array11.Length > 1 && !string.IsNullOrEmpty(array11[1]))
					{
						index2.rebindKey = array11[1];
					}
					if (array11.Length > 2 && !string.IsNullOrEmpty(array11[2]))
					{
						index2.pcBindKey = array11[2];
					}
				}
			}
		}
		catch
		{
		}
		try
		{
			Main.skipButtons.Clear();
			string[] array12 = array[9].Split(";;");
			foreach (string text6 in array12)
			{
				ButtonInfo index3 = Buttons.GetIndex(text6);
				if (index3 != null)
				{
					Main.skipButtons.Add(text6);
				}
			}
		}
		catch
		{
		}
		isLoadingPreferences = false;
		Main.hasLoadedPreferences = true;
	}

	public static void LoadPreferences()
	{
		try
		{
			if (!File.Exists("SeralythMenu/Seralyth_Preferences.txt"))
			{
				Main.hasLoadedPreferences = true;
				return;
			}
			try
			{
				UpdateSoundPreferences();
			}
			catch (Exception ex)
			{
				LogManager.Log("UpdateSoundPreferences failed: " + ex.Message);
			}
			string text = File.ReadAllText("SeralythMenu/Seralyth_Preferences.txt");
			LoadPreferencesFromText(text);
		}
		catch (Exception ex2)
		{
			LogManager.Log("Error loading preferences: " + ex2.Message);
		}
	}

	public static void Panic()
	{
		AnnoyingModeOff();
		ButtonInfo[][] buttons = Buttons.buttons;
		foreach (ButtonInfo[] array in buttons)
		{
			ButtonInfo[] array2 = array;
			foreach (ButtonInfo buttonInfo in array2)
			{
				if (buttonInfo.enabled)
				{
					Main.Toggle(buttonInfo.buttonText);
				}
			}
		}
	}

	public static void LoadPCControls()
	{
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		string path = "SeralythMenu/Seralyth_PCControls.txt";
		if (File.Exists(path))
		{
			string text = File.ReadAllText(path);
			string[] array = text.Split('\n');
			pcBindings.Clear();
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				string text3 = text2.Trim();
				if (text3.Contains(" - "))
				{
					string[] array3 = text3.Split(" - ");
					if (Enum.TryParse<ControllerBinding>(array3[1], out var result) && Enum.TryParse<Key>(array3[0], out Key result2))
					{
						pcBindings[result] = result2;
					}
				}
			}
			return;
		}
		List<string> list = new List<string>();
		foreach (KeyValuePair<ControllerBinding, Key> pcBinding in pcBindings)
		{
			list.Add($"{pcBinding.Value} - {pcBinding.Key}");
		}
		File.WriteAllLines(path, list);
	}

	public static void ChangeReconnectTime(bool positive = true)
	{
		if (!isLoadingPreferences)
		{
			if (positive)
			{
				Important.reconnectDelay++;
			}
			else
			{
				Important.reconnectDelay--;
			}
		}
		if (Important.reconnectDelay > 5)
		{
			Important.reconnectDelay = 1;
		}
		if (Important.reconnectDelay < 1)
		{
			Important.reconnectDelay = 5;
		}
		Buttons.GetIndex("Change Reconnect Time").overlapText = "Change Reconnect Time <color=grey>[</color><color=green>" + Important.reconnectDelay + "</color><color=grey>]</color>";
	}

	public static void ChangeButtonSound(bool positive = true, bool fromMenu = false)
	{
		string[] array = SoundManager.Sounds["Buttons"].Keys.ToArray();
		int num = Array.IndexOf(array, SoundManager.DefaultSounds["Button"]);
		if (num < 0)
		{
			num = 0;
		}
		num = (positive ? (num + 1) : (num - 1));
		if (num >= array.Length)
		{
			num = 0;
		}
		if (num < 0)
		{
			num = array.Length - 1;
		}
		string text = array[num];
		SoundManager.DefaultSounds["Button"] = text;
		Buttons.GetIndex("Change Button Sound").overlapText = "Change Button Sound <color=grey>[</color><color=green>" + text + "</color><color=grey>]</color>";
		if (fromMenu && !((Object)(object)VRRig.LocalRig == (Object)null))
		{
			if ((Object)(object)VRRig.LocalRig.leftHandPlayer != (Object)null)
			{
				VRRig.LocalRig.leftHandPlayer.Stop();
			}
			if ((Object)(object)VRRig.LocalRig.rightHandPlayer != (Object)null)
			{
				VRRig.LocalRig.rightHandPlayer.Stop();
			}
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
	}

	public static void ChangeButtonVolume(bool positive = true, bool fromMenu = false)
	{
		if (!isLoadingPreferences)
		{
			if (positive)
			{
				Main.buttonClickVolume++;
			}
			else
			{
				Main.buttonClickVolume--;
			}
		}
		Main.buttonClickVolume %= 11;
		if (Main.buttonClickVolume < 0)
		{
			Main.buttonClickVolume = 10;
		}
		Buttons.GetIndex("Change Button Volume").overlapText = "Change Button Volume <color=grey>[</color><color=green>" + Main.buttonClickVolume + "</color><color=grey>]</color>";
		if (fromMenu)
		{
			VRRig.LocalRig.leftHandPlayer.Stop();
			VRRig.LocalRig.rightHandPlayer.Stop();
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
	}

	public static void ChangeMenuSoundpack(bool positive = true, bool fromMenu = false)
	{
		string[] array = SoundManager.Soundpacks.Keys.ToArray();
		int num = Array.IndexOf(array, SoundManager.DefaultSoundpack);
		if (num < 0)
		{
			num = 0;
		}
		num = (positive ? (num + 1) : (num - 1));
		if (num >= array.Length)
		{
			num = 0;
		}
		if (num < 0)
		{
			num = array.Length - 1;
		}
		string text = (SoundManager.DefaultSoundpack = array[num]);
		Buttons.GetIndex("Change Menu Soundpack").overlapText = "Change Menu Soundpack <color=grey>[</color><color=green>" + text + "</color><color=grey>]</color>";
		if (fromMenu && !((Object)(object)VRRig.LocalRig == (Object)null))
		{
			if ((Object)(object)VRRig.LocalRig.leftHandPlayer != (Object)null)
			{
				VRRig.LocalRig.leftHandPlayer.Stop();
			}
			if ((Object)(object)VRRig.LocalRig.rightHandPlayer != (Object)null)
			{
				VRRig.LocalRig.rightHandPlayer.Stop();
			}
			SoundManager.Play("Default");
		}
	}

	public static void ApplyMenuLanguage(int index)
	{
		langInd = index;
		TranslationManager.translateCache.Clear();
		TranslationManager.language = LanguageCodenames[langInd];
		Main.translate = langInd != 0;
	}

	public static void ApplyMenuButton(int index)
	{
		Main.menuButtonIndex = index;
	}

	public static void ApplyMenuTheme(int index)
	{
		Main.themeType = index;
		ChangeMenuTheme();
	}

	public static void ApplyMenuScale(int index)
	{
		menuScaleIndex = index;
		Main.menuScale = (float)index / 10f;
	}

	public static void ApplyNotificationScale(int index)
	{
		notificationScaleIndex = index;
		Main.notificationScale = index * 5;
	}

	public static void ApplyArraylistScale(int index)
	{
		arraylistScaleIndex = index;
		Main.arraylistScale = index * 5;
	}

	public static void ApplyOverlayScale(int index)
	{
		overlayScaleIndex = index;
		Main.overlayScale = index * 5;
	}

	public static void ApplyPageSize(int index)
	{
		Main._pageSize = index;
	}

	public static void ApplyCharacterDistance(int index)
	{
		Main.characterDistance = index;
	}

	public static void ApplyPageType(int index)
	{
		Main.pageButtonType = index;
		Main.buttonOffset = ((Main.pageButtonType == 2) ? 2 : 0);
	}

	public static void ApplyArrowType(int index)
	{
		Main.arrowType = index;
	}

	public static void ApplyFontType(int index)
	{
		Main.fontCycle = index;
		switch (Main.fontCycle)
		{
		case 0:
			Main.activeFont = Main.AgencyFB;
			break;
		case 1:
			Main.activeFont = Main.FreeSans;
			break;
		case 2:
			Main.activeFont = Main.DejaVuSans;
			break;
		case 3:
			Main.activeFont = Main.Utopium;
			break;
		case 4:
			Main.activeFont = Main.ComicSans;
			break;
		case 5:
			Main.activeFont = Main.CascadiaMono;
			break;
		case 6:
			Main.activeFont = Main.Candara;
			break;
		case 7:
			Main.activeFont = Main.MSGothic;
			break;
		case 8:
			Main.activeFont = Main.Anton;
			break;
		case 9:
			Main.activeFont = Main.SimSun;
			break;
		case 10:
			Main.activeFont = Main.Minecraft;
			break;
		case 11:
			Main.activeFont = Main.Terminal;
			break;
		case 12:
			Main.activeFont = Main.OpenDyslexic;
			break;
		case 13:
			Main.activeFont = Main.Taiko;
			break;
		case 14:
			Main.activeFont = Main.LiberationSans;
			break;
		}
	}

	public static void ApplyFontStyleType(int index)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		fontStyleType = index;
		int num = fontStyleType;
		if (1 == 0)
		{
		}
		FontStyles activeFontStyle = (FontStyles)(num switch
		{
			0 => 0, 
			1 => 1, 
			2 => 2, 
			3 => 3, 
			_ => 0, 
		});
		if (1 == 0)
		{
		}
		Main.activeFontStyle = activeFontStyle;
	}

	public static void ApplyInputTextColor(int index)
	{
		inputTextColorInt = index;
		Main.inputTextColor = InputColorValues[index];
	}

	public static void ApplyPCUI(int index)
	{
		Main.pcbg = index;
	}

	public static void ApplyJoystickMenuPosition(int index)
	{
		Main.joystickMenuPosition = index;
	}

	public static void ApplyNotificationTime(int index)
	{
		Main.notificationDecayTime = index * 1000;
	}

	public static void ApplyNotificationSound(int index)
	{
		string[] array = SoundManager.Sounds["Notifications"].Keys.ToArray();
		if (index >= 0 && index < array.Length)
		{
			SoundManager.DefaultSounds["Notification"] = array[index];
		}
	}

	public static void ApplyNarrationVoice(int index)
	{
		Main.narratorIndex = index;
		Main.narratorName = NarratorNames[index];
		if (krec != null && ((PhraseRecognizer)krec).IsRunning && Time.time > dRestartTime)
		{
			DictationRestart();
			dRestartTime = Time.time + 1f;
		}
	}

	public static void ApplyPointerPosition(int index)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Main.pointerIndex = index;
		Main.pointerOffset = PointerPositions[index];
		try
		{
			Main.reference.transform.localPosition = Main.pointerOffset;
		}
		catch
		{
		}
	}

	public static void ApplyGunLineQuality(int index)
	{
		gunLineQualityIndex = index;
		Main.GunLineQuality = GunQualityValues[index];
	}

	public static void ApplyGunVariation(int index)
	{
		Main.gunVariation = index;
	}

	public static void ApplyGunDirection(int index)
	{
		Main.GunDirection = index;
	}

	public static void ApplyButtonSound(int index)
	{
		string[] array = SoundManager.Sounds["Buttons"].Keys.ToArray();
		if (index >= 0 && index < array.Length)
		{
			SoundManager.DefaultSounds["Button"] = array[index];
		}
	}

	public static void ApplyButtonVolume(int index)
	{
		Main.buttonClickVolume = index;
	}

	public static void PreviewButtonVolume(bool positive)
	{
		if (!((Object)(object)VRRig.LocalRig == (Object)null))
		{
			AudioSource leftHandPlayer = VRRig.LocalRig.leftHandPlayer;
			if (leftHandPlayer != null)
			{
				leftHandPlayer.Stop();
			}
			AudioSource rightHandPlayer = VRRig.LocalRig.rightHandPlayer;
			if (rightHandPlayer != null)
			{
				rightHandPlayer.Stop();
			}
			SoundManager.Play(SoundManager.DefaultSounds["Button"]);
		}
	}

	public static void ApplyMenuSoundpack(int index)
	{
		string[] array = SoundManager.Soundpacks.Keys.ToArray();
		if (index >= 0 && index < array.Length)
		{
			SoundManager.DefaultSoundpack = array[index];
		}
	}

	[CompilerGenerated]
	internal static void _003CCustomMenuName_003Eg__Apply_007C124_1()
	{
		Main.doCustomName = true;
		if (!File.Exists("SeralythMenu/Seralyth_CustomMenuName.txt"))
		{
			File.WriteAllText("SeralythMenu/Seralyth_CustomMenuName.txt", "Your Text Here");
		}
		Main.customMenuName = File.ReadAllText("SeralythMenu/Seralyth_CustomMenuName.txt");
	}
}
