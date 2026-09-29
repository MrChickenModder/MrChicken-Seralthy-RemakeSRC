using GorillaLocomotion;
using Seralyth.Menu;
using Seralyth.Utilities;
using UnityEngine;

namespace Seralyth.Classes.Mods;

public class CustomParticle : MonoBehaviour
{
	public float spawnTime;

	public float startScale;

	public Renderer renderer;

	public Vector3 velocity;

	public void Awake()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		spawnTime = Time.time;
		startScale = ((Component)this).transform.localScale.x;
		renderer = ((Component)this).gameObject.GetComponent<Renderer>() ?? null;
		velocity = RandomUtilities.RandomVector3(Main.scaleWithPlayer ? GTPlayer.Instance.scale : 1f);
		Update();
	}

	public void Update()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)renderer != (Object)null)
		{
			renderer.material.color = Main.buttonColors[1].GetCurrentColor();
		}
		if (Time.time > spawnTime + 1f)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
			return;
		}
		Transform transform = ((Component)this).transform;
		transform.position += velocity * Time.unscaledDeltaTime;
		((Component)this).transform.localScale = Vector3.one * Mathf.Lerp(startScale, 0f, Time.time - spawnTime);
	}
}
