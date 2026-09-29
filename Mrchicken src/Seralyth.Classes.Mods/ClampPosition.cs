using UnityEngine;

namespace Seralyth.Classes.Mods;

public class ClampPosition : MonoBehaviour
{
	public Transform targetTransform;

	public void Start()
	{
		Update();
	}

	public void Update()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)targetTransform == (Object)null || (Object)(object)((Component)targetTransform).gameObject == (Object)null)
		{
			Object.Destroy((Object)(object)this);
		}
		((Component)this).transform.position = targetTransform.position;
		((Component)this).transform.rotation = targetTransform.rotation;
	}
}
