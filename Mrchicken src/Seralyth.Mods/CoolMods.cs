using GorillaLocomotion;
using Seralyth.Extensions;
using Seralyth.Managers;
using Seralyth.Menu;
using UnityEngine;

namespace Seralyth.Mods;

public static class CoolMods
{
	private static float originalTimeScale = 1f;

	private static int timeWarpIndex = 2;

	private static readonly float[] timeWarpSpeeds = new float[5] { 0.1f, 0.2f, 0.3f, 0.5f, 0.75f };

	private static readonly string[] timeWarpNames = new string[5] { "10%", "20%", "30%", "50%", "75%" };

	private static float pullRadius = 8f;

	private static float pullForce = 10f;

	private static float shockwaveRadius = 12f;

	private static float shockwaveForce = 20f;

	private static bool grappling;

	private static Vector3 grappleTarget;

	private static float grappleSpeed = 15f;

	private static float autoJukeRange = 8f;

	private static float autoJukeStrength = 14f;

	private static bool ajPrevTouching;

	private static bool ajHasCharge;

	private static int airJumpBoostIndex;

	private static readonly int[] airJumpBoosts = new int[19]
	{
		10, 15, 20, 25, 30, 35, 40, 45, 50, 55,
		60, 65, 70, 75, 80, 85, 90, 95, 100
	};

	private static float groundSlamCooldown;

	private static Rigidbody PlayerRigidbody => ((Collider)GTPlayer.Instance.bodyCollider).attachedRigidbody;

	public static void TimeWarp()
	{
		Time.timeScale = timeWarpSpeeds[timeWarpIndex];
	}

	public static void EnableTimeWarp()
	{
		originalTimeScale = Time.timeScale;
		NotificationManager.SendNotification("<color=grey>[</color><color=green>TIME WARP</color><color=grey>]</color> Time slowed to " + timeWarpNames[timeWarpIndex] + ".");
	}

	public static void DisableTimeWarp()
	{
		Time.timeScale = originalTimeScale;
		NotificationManager.SendNotification("<color=grey>[</color><color=green>TIME WARP</color><color=grey>]</color> Time restored.");
	}

	public static void ChangeTimeWarpSpeed(bool positive = true)
	{
		if (positive)
		{
			timeWarpIndex++;
		}
		else
		{
			timeWarpIndex--;
		}
		timeWarpIndex %= timeWarpSpeeds.Length;
		if (timeWarpIndex < 0)
		{
			timeWarpIndex = timeWarpSpeeds.Length - 1;
		}
		Buttons.GetIndex("Time Warp").overlapText = "Time Warp <color=grey>[</color><color=green>" + timeWarpNames[timeWarpIndex] + "</color><color=grey>]</color>";
		Buttons.GetIndex("Change Time Warp Speed").overlapText = "Change Time Warp Speed <color=grey>[</color><color=green>" + timeWarpNames[timeWarpIndex] + "</color><color=grey>]</color>";
	}

	public static void MagneticPull()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (!((ControllerInputPoller)ControllerInputPoller.instance).leftControllerSecondaryButton)
		{
			return;
		}
		Vector3 position = GorillaTagger.Instance.rightHandTransform.position;
		Collider[] array = Physics.OverlapSphere(position, pullRadius);
		Collider[] array2 = array;
		foreach (Collider val in array2)
		{
			if (Object.op_Implicit((Object)(object)val.attachedRigidbody) && (Object)(object)val.attachedRigidbody != (Object)(object)PlayerRigidbody)
			{
				Vector3 val2 = position - ((Component)val).transform.position;
				val.attachedRigidbody.AddForce(((Vector3)(ref val2)).normalized * pullForce, (ForceMode)5);
			}
		}
	}

	public static void EnableMagneticPull()
	{
		NotificationManager.SendNotification("<color=grey>[</color><color=purple>MAGNETIC PULL</color><color=grey>]</color> Hold <color=green>G</color> to pull objects toward your hand.");
	}

	public static void Shockwave()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (!((ControllerInputPoller)ControllerInputPoller.instance).leftControllerSecondaryButton)
		{
			return;
		}
		Vector3 position = ((Component)GTPlayer.Instance).transform.position;
		Collider[] array = Physics.OverlapSphere(position, shockwaveRadius);
		Collider[] array2 = array;
		foreach (Collider val in array2)
		{
			if (Object.op_Implicit((Object)(object)val.attachedRigidbody) && (Object)(object)val.attachedRigidbody != (Object)(object)PlayerRigidbody)
			{
				Vector3 val2 = ((Component)val).transform.position - position;
				val.attachedRigidbody.AddForce(((Vector3)(ref val2)).normalized * shockwaveForce, (ForceMode)1);
			}
		}
		NotificationManager.SendNotification("<color=grey>[</color><color=orange>SHOCKWAVE</color><color=grey>]</color> BOOM!");
	}

	public static void EnableShockwave()
	{
		NotificationManager.SendNotification("<color=grey>[</color><color=orange>SHOCKWAVE</color><color=grey>]</color> Press <color=green>G</color> to unleash a shockwave.");
	}

	public static void Grapple()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		if (((ControllerInputPoller)ControllerInputPoller.instance).leftControllerSecondaryButton)
		{
			if (!grappling)
			{
				Ray val = default(Ray);
				((Ray)(ref val))._002Ector(GorillaTagger.Instance.rightHandTransform.position, GorillaTagger.Instance.rightHandTransform.forward);
				RaycastHit val2 = default(RaycastHit);
				if (Physics.Raycast(val, ref val2, 50f))
				{
					grappling = true;
					grappleTarget = ((RaycastHit)(ref val2)).point;
				}
			}
			if (grappling)
			{
				Vector3 val3 = grappleTarget - ((Component)GTPlayer.Instance).transform.position;
				if (((Vector3)(ref val3)).magnitude > 1f)
				{
					PlayerRigidbody.AddForce(((Vector3)(ref val3)).normalized * grappleSpeed, (ForceMode)5);
				}
				else
				{
					grappling = false;
				}
			}
		}
		else
		{
			grappling = false;
		}
	}

	public static void EnableGrapple()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		grappling = false;
		if (Application.isPlaying)
		{
			GameObject val = GameObject.CreatePrimitive((PrimitiveType)0);
			Object.Destroy((Object)(object)val.GetComponent<Collider>());
			val.transform.localScale = Vector3.one * 0.05f;
			NotificationManager.SendNotification("<color=grey>[</color><color=lime>GRAPPLE</color><color=grey>]</color> Point and hold <color=green>G</color> to grapple.");
		}
	}

	public static void DisableGrapple()
	{
		grappling = false;
	}

	public static void AutoJuke()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)VRRig.LocalRig == (Object)null || VRRigCache.ActiveRigs == null)
		{
			return;
		}
		Transform transform = ((Component)GorillaTagger.Instance.headCollider).transform;
		Vector3 position = ((Component)VRRig.LocalRig).transform.position;
		Vector3 zero = Vector3.zero;
		foreach (VRRig activeRig in VRRigCache.ActiveRigs)
		{
			if (!((Object)(object)activeRig == (Object)null) && !activeRig.isLocal && activeRig.IsTagged() != VRRig.LocalRig.IsTagged())
			{
				Vector3 val = ((Component)activeRig).transform.position - position;
				Vector3 normalized = ((Vector3)(ref val)).normalized;
				float num = Vector3.Distance(((Component)activeRig).transform.position, position);
				if (!(num > autoJukeRange))
				{
					float num2 = Vector3.Dot(transform.forward, normalized);
					float num3 = Vector3.Dot(transform.right, normalized);
					Vector3 val2 = ((!(Mathf.Abs(num2) > Mathf.Abs(num3))) ? ((0f - Mathf.Sign(num3)) * transform.right) : ((0f - Mathf.Sign(num2)) * transform.forward));
					PlayerRigidbody.linearVelocity = ((Vector3)(ref val2)).normalized * autoJukeStrength;
					break;
				}
			}
		}
	}

	public static void EnableAutoJuke()
	{
	}

	public static void AirJump()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		bool flag = GTPlayer.Instance.IsHandTouching(true) || GTPlayer.Instance.IsHandTouching(false);
		if (flag && !ajPrevTouching)
		{
			ajHasCharge = true;
		}
		ajPrevTouching = flag;
		if (ajHasCharge && ((ControllerInputPoller)ControllerInputPoller.instance).rightControllerPrimaryButton)
		{
			PlayerRigidbody.linearVelocity = ((Component)GorillaTagger.Instance.headCollider).transform.forward * (float)airJumpBoosts[airJumpBoostIndex];
			ajHasCharge = false;
		}
	}

	public static void EnableAirJump()
	{
		ajPrevTouching = false;
		ajHasCharge = false;
	}

	public static void ChangeAirJumpBoost(bool positive = true)
	{
		if (positive)
		{
			airJumpBoostIndex++;
		}
		else
		{
			airJumpBoostIndex--;
		}
		airJumpBoostIndex %= airJumpBoosts.Length;
		if (airJumpBoostIndex < 0)
		{
			airJumpBoostIndex = airJumpBoosts.Length - 1;
		}
		Buttons.GetIndex("Air Jump").overlapText = "Air Jump <color=grey>[</color><color=green>" + airJumpBoosts[airJumpBoostIndex] + "</color><color=grey>]</color>";
		Buttons.GetIndex("Change Air Jump Boost").overlapText = "Change Air Jump Boost <color=grey>[</color><color=green>" + airJumpBoosts[airJumpBoostIndex] + "</color><color=grey>]</color>";
	}

	public static void GroundSlam()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (((ControllerInputPoller)ControllerInputPoller.instance).rightControllerSecondaryButton && !(Time.time < groundSlamCooldown))
		{
			GorillaTagger.Instance.rigidbody.linearVelocity = new Vector3(GorillaTagger.Instance.rigidbody.linearVelocity.x, -25f, GorillaTagger.Instance.rigidbody.linearVelocity.z);
			groundSlamCooldown = Time.time + 0.3f;
		}
	}

	public static void EnableGroundSlam()
	{
		groundSlamCooldown = 0f;
	}
}
