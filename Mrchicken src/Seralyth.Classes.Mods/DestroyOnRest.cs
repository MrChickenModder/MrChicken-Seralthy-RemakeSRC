using UnityEngine;

namespace Seralyth.Classes.Mods;

public class DestroyOnRest : MonoBehaviour
{
	public Rigidbody rigidbody;

	public void Start()
	{
		rigidbody = ((Component)this).gameObject.GetComponent<Rigidbody>();
		Update();
	}

	public void Update()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 linearVelocity = rigidbody.linearVelocity;
		if (((Vector3)(ref linearVelocity)).magnitude < 0.01f)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
	}
}
