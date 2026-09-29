using HarmonyLib;
using Seralyth.Menu;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(GameObject), "CreatePrimitive")]
public class ShaderFix
{
	private static void Postfix(GameObject __result)
	{
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		if (Main.crystallizeMenu && (Object)(object)Main.CrystalMaterial != (Object)null)
		{
			__result.GetComponent<Renderer>().material = Main.CrystalMaterial;
		}
		else if (Main.transparentMenu)
		{
			Material material = __result.GetComponent<Renderer>().material;
			material.shader = Shader.Find(Main.shinyMenu ? "Universal Render Pipeline/Lit" : "Universal Render Pipeline/Unlit");
			material.SetFloat("_Surface", 1f);
			material.SetFloat("_Blend", 0f);
			material.SetFloat("_SrcBlend", 5f);
			material.SetFloat("_DstBlend", 10f);
			material.SetFloat("_ZWrite", 0f);
			material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
			material.renderQueue = 3000;
		}
		else
		{
			__result.GetComponent<Renderer>().material.shader = Shader.Find(Main.shinyMenu ? "Universal Render Pipeline/Lit" : "GorillaTag/UberShader");
		}
		__result.GetComponent<Renderer>().material.color = Main.backgroundColor.GetColor(0);
	}
}
