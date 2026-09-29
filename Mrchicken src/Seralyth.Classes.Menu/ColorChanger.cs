using GorillaExtensions;
using Seralyth.Menu;
using UnityEngine;

namespace Seralyth.Classes.Menu;

public class ColorChanger : MonoBehaviour
{
	public Renderer targetRenderer;

	public ExtGradient colors;

	public bool? overrideTransparency;

	public void Start()
	{
		if (colors == null)
		{
			Object.Destroy((Object)(object)this);
			return;
		}
		targetRenderer = ((Component)this).GetComponent<Renderer>();
		if (colors.IsFlat())
		{
			Update();
			Object.Destroy((Object)(object)this);
		}
		else
		{
			Update();
		}
	}

	public void Update()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected O, but got Unknown
		targetRenderer.enabled = overrideTransparency ?? (!colors.transparent);
		if (colors.transparent)
		{
			return;
		}
		if (!Main.dynamicGradients)
		{
			targetRenderer.material.color = colors.GetCurrentColor();
		}
		else if (colors.IsFlat())
		{
			targetRenderer.material.color = colors.GetColor(0);
		}
		else if (((Object)targetRenderer.material.shader).name != "Universal Render Pipeline/Unlit" && (Object)(object)targetRenderer.material.mainTexture == (Object)null)
		{
			targetRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"))
			{
				mainTexture = (Texture)(object)Main.GetGradientTexture(colors.GetColor(0), colors.GetColor(1))
			};
			if (Main.scrollingGradients)
			{
				GTExt.GetOrAddComponent<ScrollMaterial>(((Component)this).gameObject);
			}
		}
		if (Main.transparentMenu)
		{
			Color color = targetRenderer.material.color;
			color.a = 0.5f;
			targetRenderer.material.color = color;
		}
	}
}
