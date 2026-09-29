using UnityEngine;

namespace Seralyth.Utilities;

public class RandomUtilities
{
	public static Vector3 RandomVector3(float range = 1f)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(Random.Range(0f - range, range), Random.Range(0f - range, range), Random.Range(0f - range, range));
	}

	public static Quaternion RandomQuaternion(float range = 360f)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		return Quaternion.Euler(Random.Range(0f, range), Random.Range(0f, range), Random.Range(0f, range));
	}

	public static Color RandomColor(byte range = byte.MaxValue, byte alpha = byte.MaxValue)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		return Color32.op_Implicit(new Color32((byte)Random.Range(0, (int)range), (byte)Random.Range(0, (int)range), (byte)Random.Range(0, (int)range), alpha));
	}

	public static string RandomString(int length = 4)
	{
		string text = "";
		for (int i = 0; i < length; i++)
		{
			int num = Random.Range(0, 36);
			text += ((num < 26) ? ((char)(65 + num)) : ((char)(48 + (num - 26))));
		}
		return text;
	}
}
