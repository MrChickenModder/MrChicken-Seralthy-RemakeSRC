using Seralyth.Menu;
using UnityEngine;

namespace Seralyth.Mods;

public static class MonkeClicker
{
	private static GameObject lineObj;

	private static LineRenderer line;

	private static Vector3? oldLocalPos;

	private static bool wasTriggering;

	public static void Enable()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		if ((Object)(object)lineObj == (Object)null)
		{
			lineObj = new GameObject("MonkeClicker_Line");
			Object.DontDestroyOnLoad((Object)(object)lineObj);
			line = lineObj.AddComponent<LineRenderer>();
			line.positionCount = 2;
			line.startWidth = 0.015f;
			line.endWidth = 0.003f;
			line.startColor = Color.green;
			line.endColor = Color.red;
			((Renderer)line).material = new Material(Shader.Find("Unlit/Color"));
			((Renderer)line).enabled = false;
		}
	}

	public static void Disable()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (oldLocalPos.HasValue)
		{
			GorillaTagger.Instance.rightHandTriggerCollider.transform.localPosition = oldLocalPos.Value;
			((Behaviour)GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<TransformFollow>()).enabled = true;
			oldLocalPos = null;
		}
		wasTriggering = false;
		if ((Object)(object)lineObj != (Object)null)
		{
			Object.Destroy((Object)(object)lineObj);
			lineObj = null;
			line = null;
		}
	}

	public static void Run()
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)lineObj == (Object)null || (Object)(object)line == (Object)null)
		{
			Enable();
		}
		bool flag = Main.leftGrab || Main.rightGrab;
		bool flag2 = Main.leftTrigger > 0.5f || Main.rightTrigger > 0.5f;
		Transform val = ((Main.rightGrab || (Main.rightTrigger > 0.5f && !Main.leftGrab)) ? GorillaTagger.Instance.rightHandTransform : GorillaTagger.Instance.leftHandTransform);
		float num = 512f;
		if (flag || flag2)
		{
			((Renderer)line).enabled = true;
			line.SetPosition(0, val.position);
		}
		Vector3 forward = val.forward;
		Ray val2 = default(Ray);
		((Ray)(ref val2))._002Ector(val.position + forward / 4f, forward);
		RaycastHit val3 = default(RaycastHit);
		if (Physics.Raycast(val2, ref val3, num, Main.NoInvisLayerMask()))
		{
			if (flag || flag2)
			{
				line.SetPosition(1, ((RaycastHit)(ref val3)).point);
			}
			if (flag2)
			{
				if (!wasTriggering)
				{
					wasTriggering = true;
					Vector3 valueOrDefault = oldLocalPos.GetValueOrDefault();
					if (!oldLocalPos.HasValue)
					{
						valueOrDefault = GorillaTagger.Instance.rightHandTriggerCollider.transform.localPosition;
						oldLocalPos = valueOrDefault;
					}
					((Behaviour)GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<TransformFollow>()).enabled = false;
				}
				GorillaTagger.Instance.rightHandTriggerCollider.transform.position = ((RaycastHit)(ref val3)).point;
			}
			else if (wasTriggering)
			{
				wasTriggering = false;
				if (oldLocalPos.HasValue)
				{
					GorillaTagger.Instance.rightHandTriggerCollider.transform.localPosition = oldLocalPos.Value;
					oldLocalPos = null;
				}
				((Behaviour)GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<TransformFollow>()).enabled = true;
			}
		}
		else if (flag || flag2)
		{
			line.SetPosition(1, val.position + val.forward * num);
		}
		if (flag || flag2)
		{
			return;
		}
		((Renderer)line).enabled = false;
		if (wasTriggering)
		{
			wasTriggering = false;
			if (oldLocalPos.HasValue)
			{
				GorillaTagger.Instance.rightHandTriggerCollider.transform.localPosition = oldLocalPos.Value;
				oldLocalPos = null;
			}
			((Behaviour)GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<TransformFollow>()).enabled = true;
		}
	}
}
