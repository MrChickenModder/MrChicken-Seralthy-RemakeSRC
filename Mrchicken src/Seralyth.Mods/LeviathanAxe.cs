using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion;
using Photon.Realtime;
using Seralyth.Classes.Menu;
using Seralyth.Menu;
using UnityEngine;

namespace Seralyth.Mods;

public static class LeviathanAxe
{
	private static int allocatedSwordId = -1;

	private static bool lastVelTooHigh;

	private static float swingDelay;

	public static void spawnLeviathanAxe()
	{
		if (allocatedSwordId < 0)
		{
			allocatedSwordId = Console.GetFreeAssetID();
			Console.ExecuteCommand("asset-spawn", (ReceiverGroup)1, "leviathan", "Leviathan", allocatedSwordId);
			Console.ExecuteCommand("asset-setanchor", (ReceiverGroup)1, allocatedSwordId, 2);
			Main.RPCProtection();
		}
	}

	public static void UpdateLeviathanAxe()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		if (allocatedSwordId < 0 || !Console.consoleAssets.TryGetValue(allocatedSwordId, out var asset) || (Object)(object)asset.assetObject == (Object)null)
		{
			return;
		}
		Vector3 val = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0f, false) - GorillaTagger.Instance.rigidbody.linearVelocity;
		bool flag = ((Vector3)(ref val)).magnitude > 10f;
		bool flag2 = false;
		if (flag && !lastVelTooHigh && Time.time > swingDelay)
		{
			swingDelay = Time.time + 0.3f;
			using (IEnumerator<VRRig> enumerator = VRRigCache.ActiveRigs.Where((VRRig r) => !r.isLocal && Vector3.Distance(r.bodyTransform.position, asset.assetObject.transform.GetChild(1).position) < 0.25f).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					VRRig current = enumerator.Current;
					flag2 = true;
					Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, allocatedSwordId, "Model", "Hit");
					int actorNumber = current.Creator.ActorNumber;
					object[] array = new object[1];
					val = ((Component)current).transform.position - GorillaTagger.Instance.rightHandTransform.position;
					array[0] = ((Vector3)(ref val)).normalized * 4f;
					Console.ExecuteCommand("vel", actorNumber, array);
				}
			}
			if (!flag2)
			{
				Console.ExecuteCommand("asset-playsound", (ReceiverGroup)1, allocatedSwordId, "Model", "Swing");
			}
		}
		lastVelTooHigh = flag;
	}

	public static void destroyLeviathanAxe()
	{
		if (allocatedSwordId >= 0)
		{
			Console.ExecuteCommand("asset-destroy", (ReceiverGroup)1, allocatedSwordId);
			allocatedSwordId = -1;
			lastVelTooHigh = false;
			swingDelay = 0f;
		}
	}

	public static void PLACEHOLDER()
	{
		PLACEHOLDER();
	}
}
