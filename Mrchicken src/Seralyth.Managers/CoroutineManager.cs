using System;
using System.Collections;
using UnityEngine;

namespace Seralyth.Managers;

public class CoroutineManager : MonoBehaviour
{
	public static CoroutineManager instance;

	private void Awake()
	{
		instance = this;
	}

	[Obsolete("RunCoroutine is obsolete. Use StartCoroutine directly on MonoBehaviour instances instead.")]
	public static Coroutine RunCoroutine(IEnumerator enumerator)
	{
		return ((MonoBehaviour)instance).StartCoroutine(enumerator);
	}

	[Obsolete("EndCoroutine is obsolete. Use StopCoroutine directly on MonoBehaviour instances instead.")]
	public static void EndCoroutine(Coroutine enumerator)
	{
		((MonoBehaviour)instance).StopCoroutine(enumerator);
	}
}
