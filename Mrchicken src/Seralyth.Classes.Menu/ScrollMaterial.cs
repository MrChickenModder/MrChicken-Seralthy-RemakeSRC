using Seralyth.Menu;
using UnityEngine;

namespace Seralyth.Classes.Menu;

public class ScrollMaterial : MonoBehaviour
{
	private Renderer renderer;

	private Material mat;

	private void Awake()
	{
		renderer = ((Component)this).GetComponent<Renderer>();
		mat = renderer.material;
		Update();
	}

	private void Update()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		float num = (Main.slowFadeColors ? (Time.time / 10f) : Time.time);
		Vector4 val = default(Vector4);
		((Vector4)(ref val))._002Ector(1f, 1f, num, num);
		mat.SetVector("_BaseMap_ST", val);
	}
}
