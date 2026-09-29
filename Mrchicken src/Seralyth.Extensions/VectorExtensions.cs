using System;
using Seralyth.Utilities;
using UnityEngine;

namespace Seralyth.Extensions;

public static class VectorExtensions
{
	public static float Distance(this Vector3 point, Vector3 to)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Distance(point, to);
	}

	public static Vector3 Lerp(this Vector3 a, Vector3 b, float t)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Lerp(a, b, t);
	}

	public static Vector3 XyZ(this Vector3 a)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(a.x, Mathf.Max(a.y, 0f), a.z);
	}

	public static long Pack(this Vector3 vec)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return BitPackUtils.PackWorldPosForNetwork(vec);
	}

	public static Vector3 Random(this Vector3 _, float power = 1f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return RandomUtilities.RandomVector3(power);
	}

	public static Vector3 ClampMagnitude(this Vector3 vec, float magnitude)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.ClampMagnitude(vec, magnitude);
	}

	public static Vector3 ClampSqrMagnitude(this Vector3 vec, float sqrMagnitude)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		float sqrMagnitude2 = ((Vector3)(ref vec)).sqrMagnitude;
		if (!(sqrMagnitude2 > sqrMagnitude) || !(sqrMagnitude2 > 0f))
		{
			return vec;
		}
		float num = MathF.Sqrt(sqrMagnitude / sqrMagnitude2);
		vec *= num;
		return vec;
	}
}
