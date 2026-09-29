using UnityEngine;

namespace Seralyth.Extensions;

public static class CallLimiterExtensions
{
	public static float GetTimeDelay(this CallLimiter limiter, float time)
	{
		if (limiter == null || limiter.callTimeHistory == null || limiter.callHistoryLength <= 0)
		{
			return 0f;
		}
		int num = Mathf.Clamp(limiter.oldTimeIndex, 0, limiter.callHistoryLength - 1);
		float num2 = limiter.callTimeHistory[num];
		if (num2 == float.MinValue)
		{
			return 0f;
		}
		return Mathf.Max(0f, num2 - time);
	}

	public static bool CanCallNow(this CallLimiter limiter, float time)
	{
		return limiter.GetTimeDelay(time) <= 0f;
	}
}
