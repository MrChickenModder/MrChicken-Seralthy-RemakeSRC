using System;
using System.Linq;
using GorillaLocomotion;
using Seralyth.Managers;
using Seralyth.Mods;
using UnityEngine;

namespace Seralyth.Classes.Mods;

public class PortalTrigger : MonoBehaviour
{
	private static readonly Type[] allowedTypes = new Type[2]
	{
		typeof(ThrowableBug),
		typeof(SlingshotProjectile)
	};

	public GameObject destination;

	private static bool HasAllowedComponent(Collider col)
	{
		return allowedTypes.Any((Type t) => (Object)(object)((Component)col).GetComponent(t) != (Object)null);
	}

	public void OnTriggerEnter(Collider other)
	{
		if ((Object)(object)other == (Object)(object)GTPlayer.Instance.bodyCollider || (Object)(object)other == (Object)(object)GTPlayer.Instance.headCollider)
		{
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(Movement.TeleportPortal(destination));
		}
		else if (HasAllowedComponent(other))
		{
			((MonoBehaviour)CoroutineManager.instance).StartCoroutine(Movement.TeleportObject(((Component)other).gameObject, destination));
		}
	}
}
