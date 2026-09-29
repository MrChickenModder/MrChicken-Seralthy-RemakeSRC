using System;
using Seralyth.Utilities;
using TMPro;
using UnityEngine;

namespace Seralyth.Extensions;

public static class TextMeshProExtensions
{
	private static Shader _tmpShader;

	public static Shader TmpShader
	{
		get
		{
			if ((Object)(object)_tmpShader == (Object)null)
			{
				_tmpShader = AssetUtilities.LoadAsset<Shader>("TMP_SDF-Mobile Overlay");
			}
			return _tmpShader;
		}
	}

	public static void SafeSetText(this TMP_Text tmp, string text)
	{
		if (!((Object)(object)tmp == (Object)null) && tmp.text != text)
		{
			tmp.text = text;
		}
	}

	public static void SafeSetFont(this TMP_Text tmp, TMP_FontAsset font)
	{
		if (!((Object)(object)tmp == (Object)null) && !((Object)(object)font == (Object)null) && ((TMP_Asset)tmp.font).hashCode != ((TMP_Asset)font).hashCode)
		{
			tmp.font = font;
		}
	}

	public static void SafeSetFontSize(this TMP_Text tmp, float size)
	{
		if (!((Object)(object)tmp == (Object)null) && Math.Abs(tmp.fontSize - size) > 0.01f)
		{
			tmp.fontSize = size;
		}
	}

	public static void SafeSetFontStyle(this TMP_Text tmp, FontStyles style)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)tmp == (Object)null) && tmp.fontStyle != style)
		{
			tmp.fontStyle = style;
		}
	}

	public static void SafeSetCharacterSpacing(this TMP_Text tmp, float targetSpacing)
	{
		if (!((Object)(object)tmp == (Object)null) && !Mathf.Approximately(tmp.characterSpacing, targetSpacing))
		{
			tmp.characterSpacing = targetSpacing;
		}
	}

	public static void Chams(this TMP_Text tmp)
	{
		if (!((Object)(object)tmp == (Object)null))
		{
			Material fontMaterial = tmp.fontMaterial;
			if ((Object)(object)fontMaterial != (Object)null && (Object)(object)fontMaterial.shader != (Object)(object)TmpShader)
			{
				fontMaterial.shader = TmpShader;
			}
		}
	}
}
