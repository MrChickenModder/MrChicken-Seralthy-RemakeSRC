using UnityEngine;
using UnityEngine.UI;

namespace Seralyth.Classes.Menu;

public class UIColorChanger : MonoBehaviour
{
	public MaskableGraphic targetGraphic;

	public ExtGradient colors;

	public void Start()
	{
		if (colors == null)
		{
			Object.Destroy((Object)(object)this);
			return;
		}
		targetGraphic = ((Component)this).gameObject.GetComponent<MaskableGraphic>();
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
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		((Graphic)targetGraphic).color = colors.GetCurrentColor();
	}
}
