using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.XR;

namespace Seralyth.Utilities;

public class ControllerUtilities
{
	public enum ControllerType
	{
		Unknown,
		Quest2,
		Quest3,
		ValveIndex,
		VIVE
	}

	private struct ControllerInfo
	{
		public ControllerType type;

		public float dataCacheTime;
	}

	private static readonly Dictionary<bool, ControllerInfo> controllerInfo = new Dictionary<bool, ControllerInfo>
	{
		{
			true,
			new ControllerInfo
			{
				type = ControllerType.Unknown,
				dataCacheTime = -1f
			}
		},
		{
			false,
			new ControllerInfo
			{
				type = ControllerType.Unknown,
				dataCacheTime = -1f
			}
		}
	};

	private static readonly Dictionary<string, ControllerType> controllerNames = new Dictionary<string, ControllerType>
	{
		{
			"quest2",
			ControllerType.Quest2
		},
		{
			"quest3",
			ControllerType.Quest3
		},
		{
			"knuckles",
			ControllerType.ValveIndex
		},
		{
			"vive",
			ControllerType.VIVE
		}
	};

	public static ControllerType GetControllerType(bool left)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!XRSettings.isDeviceActive)
			{
				return ControllerType.Unknown;
			}
			InputDevice val = (left ? ((ControllerInputPoller)ControllerInputPoller.instance).leftControllerDevice : ((ControllerInputPoller)ControllerInputPoller.instance).rightControllerDevice);
			if (controllerInfo.TryGetValue(left, out var value) && !(Time.time > value.dataCacheTime + 1f))
			{
				return value.type;
			}
			string name = ((InputDevice)(ref val)).name;
			ControllerType type = ControllerType.Unknown;
			foreach (KeyValuePair<string, ControllerType> controllerName in controllerNames)
			{
				if (name.IndexOf(controllerName.Key, StringComparison.OrdinalIgnoreCase) < 0)
				{
					continue;
				}
				type = controllerName.Value;
				break;
			}
			value = new ControllerInfo
			{
				type = type,
				dataCacheTime = Time.time
			};
			controllerInfo[left] = value;
			return value.type;
		}
		catch
		{
			return ControllerType.Unknown;
		}
	}

	public static ControllerType GetLeftControllerType()
	{
		return GetControllerType(left: true);
	}

	public static ControllerType GetRightControllerType()
	{
		return GetControllerType(left: false);
	}

	public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) GetTrueHandPosition(bool left)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		Transform val = (left ? GorillaTagger.Instance.leftHandTransform : GorillaTagger.Instance.rightHandTransform);
		HandState val2 = (left ? GTPlayer.Instance.LeftHand : GTPlayer.Instance.RightHand);
		Quaternion val3 = val.rotation * val2.handRotOffset;
		return (position: val.position + val.rotation * (val2.handOffset * GTPlayer.Instance.scale), rotation: val3, up: val3 * Vector3.up, forward: val3 * Vector3.forward, right: val3 * Vector3.right);
	}

	public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) GetTrueLeftHand()
	{
		return GetTrueHandPosition(left: true);
	}

	public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) GetTrueRightHand()
	{
		return GetTrueHandPosition(left: false);
	}
}
