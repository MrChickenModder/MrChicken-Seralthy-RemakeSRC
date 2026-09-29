using System;
using System.Collections.Generic;
using System.Linq;
using Seralyth.Extensions;
using Seralyth.Menu;
using Seralyth.Utilities;
using UnityEngine;

namespace Seralyth.Classes.Menu;

public class ExtGradient
{
	public GradientColorKey[] colors = GetSolidGradient(Color.magenta);

	private static Gradient getColorGradient;

	public bool rainbow;

	public bool pastelRainbow;

	public bool epileptic;

	public bool copyRigColor;

	public bool transparent;

	public Func<Color> customColor;

	public static GradientColorKey[] GetSolidGradient(Color color)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		return (GradientColorKey[])(object)new GradientColorKey[2]
		{
			new GradientColorKey(color, 0f),
			new GradientColorKey(color, 1f)
		};
	}

	public static GradientColorKey[] GetSimpleGradient(Color a, Color b)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		return (GradientColorKey[])(object)new GradientColorKey[3]
		{
			new GradientColorKey(a, 0f),
			new GradientColorKey(b, 0.5f),
			new GradientColorKey(a, 1f)
		};
	}

	public Color GetColor(int index)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		if (rainbow)
		{
			return Color.HSVToRGB((Time.time + (float)(index / 8)) % 1f, 1f, 1f);
		}
		if (pastelRainbow)
		{
			return Color.HSVToRGB(Time.time + (float)(index / 8), 0.3f, 1f);
		}
		if (epileptic)
		{
			return RandomUtilities.RandomColor();
		}
		if (copyRigColor)
		{
			return VRRig.LocalRig.GetColor();
		}
		if (!transparent)
		{
			return (customColor == null) ? colors[index].color : (customColor?.Invoke() ?? Color.magenta);
		}
		Color color = colors[index].color;
		color.a = 0f;
		return color;
	}

	public void SetColor(int index, Color color, bool setMirror = true)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		rainbow = false;
		pastelRainbow = false;
		epileptic = false;
		copyRigColor = false;
		customColor = null;
		if (colors.Length <= 2)
		{
			colors = GetSimpleGradient(colors[0].color, colors[^1].color);
		}
		if (setMirror && index == 0)
		{
			colors[0].color = color;
			colors[^1].color = color;
		}
		else
		{
			colors[index].color = color;
		}
	}

	public void SetColors(Color color)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		rainbow = false;
		pastelRainbow = false;
		epileptic = false;
		copyRigColor = false;
		customColor = null;
		for (int i = 0; i < colors.Length; i++)
		{
			colors[i].color = color;
		}
	}

	public Color GetColorTime(float time)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		if (getColorGradient == null)
		{
			getColorGradient = new Gradient();
		}
		if (rainbow)
		{
			return Color.HSVToRGB(time, 1f, 1f);
		}
		if (pastelRainbow)
		{
			return Color.HSVToRGB(time, 0.3f, 1f);
		}
		if (epileptic)
		{
			return RandomUtilities.RandomColor();
		}
		if (copyRigColor)
		{
			return VRRig.LocalRig.GetColor();
		}
		if (transparent)
		{
			Color result = getColorGradient.Evaluate(time);
			result.a = 0f;
			return result;
		}
		if (customColor != null)
		{
			return customColor?.Invoke() ?? Color.magenta;
		}
		getColorGradient.colorKeys = colors;
		return getColorGradient.Evaluate(time);
	}

	public Color GetCurrentColor(float offset = 0f)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		return GetColorTime((offset + Time.time / (Main.slowFadeColors ? 10f : 2f)) % 1f);
	}

	public bool IsFlat()
	{
		return !rainbow && !pastelRainbow && !epileptic && !copyRigColor && colors.Length != 0 && colors.All((GradientColorKey key) => key.color == colors[0].color);
	}

	public ExtGradient Clone()
	{
		return new ExtGradient
		{
			rainbow = rainbow,
			pastelRainbow = pastelRainbow,
			epileptic = epileptic,
			copyRigColor = copyRigColor,
			customColor = customColor,
			colors = ((IEnumerable<GradientColorKey>)colors).Select((Func<GradientColorKey, GradientColorKey>)((GradientColorKey c) => new GradientColorKey(c.color, c.time))).ToArray()
		};
	}
}
