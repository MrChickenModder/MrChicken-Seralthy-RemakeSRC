using UnityEngine;

namespace Seralyth.Extensions;

public static class QuaternionExtensions
{
	public static Quaternion Lerp(this Quaternion a, Quaternion b, float t)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return Quaternion.Lerp(a, b, t);
	}
}
