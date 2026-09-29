using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Photon.Pun;
using Photon.Realtime;
using Seralyth.Menu;
using Seralyth.Utilities;
using UnityEngine;

namespace Seralyth.Extensions;

public static class MiscellaneousExtensions
{
	private static readonly Dictionary<KeyCode, string> normalMap = new Dictionary<KeyCode, string>
	{
		{
			(KeyCode)32,
			" "
		},
		{
			(KeyCode)44,
			","
		},
		{
			(KeyCode)46,
			"."
		},
		{
			(KeyCode)47,
			"/"
		},
		{
			(KeyCode)92,
			"\\"
		},
		{
			(KeyCode)45,
			"-"
		},
		{
			(KeyCode)61,
			"="
		},
		{
			(KeyCode)59,
			";"
		},
		{
			(KeyCode)39,
			"'"
		},
		{
			(KeyCode)91,
			"["
		},
		{
			(KeyCode)93,
			"]"
		}
	};

	private static readonly Dictionary<KeyCode, string> shiftMap = new Dictionary<KeyCode, string>
	{
		{
			(KeyCode)49,
			"!"
		},
		{
			(KeyCode)50,
			"@"
		},
		{
			(KeyCode)51,
			"#"
		},
		{
			(KeyCode)52,
			"$"
		},
		{
			(KeyCode)53,
			"%"
		},
		{
			(KeyCode)54,
			"^"
		},
		{
			(KeyCode)55,
			"&"
		},
		{
			(KeyCode)56,
			"*"
		},
		{
			(KeyCode)57,
			"("
		},
		{
			(KeyCode)48,
			")"
		},
		{
			(KeyCode)45,
			"_"
		},
		{
			(KeyCode)61,
			"+"
		},
		{
			(KeyCode)91,
			"{"
		},
		{
			(KeyCode)93,
			"}"
		},
		{
			(KeyCode)92,
			"|"
		},
		{
			(KeyCode)59,
			":"
		},
		{
			(KeyCode)39,
			"\""
		},
		{
			(KeyCode)44,
			"<"
		},
		{
			(KeyCode)46,
			">"
		},
		{
			(KeyCode)47,
			"?"
		}
	};

	public static float GetDelay(this CallLimiter limiter)
	{
		return limiter.timeCooldown / (float)limiter.callHistoryLength;
	}

	public static float GetDelay(this FXSystemSettings settings, int index)
	{
		return settings.GetCallLimiter(index).GetDelay();
	}

	public static CallLimiter GetCallLimiter(this FXSystemSettings settings, int index)
	{
		return settings.callSettings[index].CallLimitSettings;
	}

	public static void Serialize(this PhotonView view, RaiseEventOptions options = null, int timeOffset = 0)
	{
		Main.SendSerialize(view, options, timeOffset);
	}

	public static string ToHex(this Color input)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Main.ColorToHex(input);
	}

	public static string ToRichRGBString(this Color input, int roundAmount = 255)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		return $"<color=red>{Math.Round(input.r * (float)roundAmount)}</color> <color=green>{Math.Round(input.g * (float)roundAmount)}</color> <color=blue>{Math.Round(input.b * (float)roundAmount)}</color>";
	}

	public static string ToRGBString(this Color input, int roundAmount = 255)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		return $"{Math.Round(input.r * (float)roundAmount)} {Math.Round(input.g * (float)roundAmount)} {Math.Round(input.b * (float)roundAmount)}";
	}

	public unsafe static string Key(this KeyCode key)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Invalid comparison between Unknown and I4
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Invalid comparison between Unknown and I4
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Invalid comparison between Unknown and I4
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Invalid comparison between Unknown and I4
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Invalid comparison between Unknown and I4
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Invalid comparison between Unknown and I4
		if ((int)key >= 97 && (int)key <= 122)
		{
			return ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString().ToLower();
		}
		if ((int)key >= 48 && (int)key <= 57)
		{
			return ((char)(48 + (key - 48))).ToString();
		}
		if (normalMap.TryGetValue(key, out var value))
		{
			return value;
		}
		KeyCode val = key;
		KeyCode val2 = val;
		if ((int)val2 != 9)
		{
			if ((int)val2 == 13 || (int)val2 == 271)
			{
				return "\n";
			}
			return "";
		}
		return "\t";
	}

	public unsafe static string ShiftedKey(this KeyCode key)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Invalid comparison between Unknown and I4
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		string value;
		return ((int)key >= 97 && (int)key <= 122) ? ((object)(*(KeyCode*)(&key))/*cast due to .constrained prefix*/).ToString() : (shiftMap.TryGetValue(key, out value) ? value : key.Key());
	}

	public static IEnumerable<GameObject> Children(this Transform t)
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < t.childCount; i++)
		{
			list.Add(((Component)t.GetChild(i)).gameObject);
		}
		return list;
	}

	public static void Play(this AudioClip clip, float volume = 1f)
	{
		Main.Play2DAudio(clip, volume);
	}

	public static void PlayAt(this AudioClip clip, Vector3 position, float volume = 1f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Main.PlayPositionAudio(clip, position, volume);
	}

	public static void RequestGrab(this GameEntity gameEntity, bool isLeftHand, Vector3 localPosition, Quaternion localRotation, GameEntityManager manager = null)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		GameEntityManager val = manager ?? ManagerRegistry.GhostReactor.GameEntityManager;
		if (val.IsAuthority())
		{
			(manager ?? ManagerRegistry.GhostReactor.GameEntityManager).photonView.RPC("GrabEntityRPC", (RpcTarget)0, new object[4]
			{
				gameEntity.id,
				isLeftHand,
				BitPackUtils.PackHandPosRotForNetwork(localPosition, localRotation),
				NetworkSystem.Instance.LocalPlayer.GetPlayer()
			});
		}
		else
		{
			(manager ?? ManagerRegistry.GhostReactor.GameEntityManager).RequestGrabEntity(gameEntity.id, isLeftHand, localPosition, localRotation);
		}
	}

	public static void RequestThrow(this GameEntity gameEntity, bool isLeftHand, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angVelocity, GameEntityManager manager = null)
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		GameEntityManager val = manager ?? ManagerRegistry.GhostReactor.GameEntityManager;
		if (val.IsAuthority())
		{
			val.photonView.RPC("ThrowEntityRPC", (RpcTarget)0, new object[8]
			{
				gameEntity.id,
				isLeftHand,
				position,
				rotation,
				velocity,
				angVelocity,
				NetworkSystem.Instance.LocalPlayer.GetPlayer(),
				PhotonNetwork.Time
			});
		}
		else
		{
			val.photonView.RPC("RequestThrowEntityRPC", (RpcTarget)2, new object[6]
			{
				val.GetNetIdFromEntityId(gameEntity.id),
				isLeftHand,
				position,
				rotation,
				velocity,
				angVelocity
			});
		}
	}

	public static int CreateTypeNetId(this GameEntityManager manager, int typeId)
	{
		return manager.CreateNetId(1 + manager.FactoryGetBuiltInEntityCountById(typeId));
	}

	public static int GetInvalidNetId(this GameEntityManager manager)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return manager.GetNetIdFromEntityId(GameEntityId.Invalid);
	}

	public static bool TryGetComponentInParent<T>(this Component component, out T result) where T : Component
	{
		result = component.GetComponentInParent<T>();
		return (Object)(object)result != (Object)null;
	}

	public static bool TryGetComponentInParent<T>(this GameObject obj, out T result) where T : Component
	{
		result = obj.GetComponentInParent<T>();
		return (Object)(object)result != (Object)null;
	}

	public static bool TryGetKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, out TKey actualKey)
	{
		if (dictionary != null)
		{
			return TryGetKeyInternal(dictionary, key, out actualKey, null);
		}
		throw new ArgumentNullException("dictionary");
	}

	public static bool TryGetKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, out TKey actualKey, IEqualityComparer<TKey> comparer)
	{
		if (dictionary != null)
		{
			return TryGetKeyInternal(dictionary, key, out actualKey, comparer);
		}
		throw new ArgumentNullException("dictionary");
	}

	public static bool TryGetKey<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> pairs, TKey key, out TKey actualKey, IEqualityComparer<TKey> comparer = null)
	{
		if (pairs == null)
		{
			throw new ArgumentNullException("pairs");
		}
		if (comparer == null)
		{
			comparer = EqualityComparer<TKey>.Default;
		}
		foreach (KeyValuePair<TKey, TValue> pair in pairs)
		{
			if (!comparer.Equals(pair.Key, key))
			{
				continue;
			}
			actualKey = pair.Key;
			return true;
		}
		actualKey = default(TKey);
		return false;
	}

	public static bool TryGetKeyByValue(this Dictionary<string, string> dict, string value, out string key)
	{
		using (IEnumerator<KeyValuePair<string, string>> enumerator = dict.Where((KeyValuePair<string, string> kv) => string.Equals(kv.Value, value, StringComparison.OrdinalIgnoreCase)).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				key = enumerator.Current.Key;
				return true;
			}
		}
		key = null;
		return false;
	}

	private static bool TryGetKeyInternal<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, out TKey actualKey, IEqualityComparer<TKey> comparer)
	{
		if (comparer == null)
		{
			comparer = TryGetComparerFromDictionary(dictionary) ?? EqualityComparer<TKey>.Default;
		}
		foreach (TKey key2 in dictionary.Keys)
		{
			if (!comparer.Equals(key2, key))
			{
				continue;
			}
			actualKey = key2;
			return true;
		}
		actualKey = default(TKey);
		return false;
	}

	private static IEqualityComparer<TKey> TryGetComparerFromDictionary<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
	{
		if (dictionary is Dictionary<TKey, TValue> dictionary2)
		{
			return dictionary2.Comparer;
		}
		Type type = dictionary.GetType();
		PropertyInfo property = type.GetProperty("Comparer", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (property == null || !typeof(IEqualityComparer<TKey>).IsAssignableFrom(property.PropertyType))
		{
			return null;
		}
		return property.GetValue(dictionary) as IEqualityComparer<TKey>;
	}
}
