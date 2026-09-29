using Seralyth.Classes.Menu;
using UnityEngine;

namespace Seralyth.Classes.Mods;

public class ClampColor : MonoBehaviour
{
	public Renderer gameObjectRenderer;

	public Renderer targetRenderer;

	public void Start()
	{
		((Component)targetRenderer).gameObject.GetComponent<ColorChanger>()?.Start();
		gameObjectRenderer = ((Component)this).GetComponent<Renderer>();
		Update();
	}

	public void Update()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)gameObjectRenderer.material.shader != (Object)(object)targetRenderer.material.shader)
		{
			gameObjectRenderer.material = new Material(targetRenderer.material.shader);
		}
		if ((Object)(object)targetRenderer.material.mainTexture != (Object)null && (Object)(object)gameObjectRenderer.material.mainTexture != (Object)(object)targetRenderer.material.mainTexture)
		{
			gameObjectRenderer.material.mainTexture = targetRenderer.material.mainTexture;
		}
		gameObjectRenderer.material.color = targetRenderer.material.color;
	}
}
