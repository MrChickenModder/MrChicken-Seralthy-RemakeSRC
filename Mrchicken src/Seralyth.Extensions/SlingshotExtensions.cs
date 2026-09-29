using UnityEngine;

namespace Seralyth.Extensions;

public static class SlingshotExtensions
{
	public static Vector3 GetTrueLaunchPosition(this Slingshot slingshot)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = slingshot.drawingHand.transform.position;
		Vector3 val = slingshot.centerOrigin.position - slingshot.drawingHand.transform.position;
		return position + ((Vector3)(ref val)).normalized * (((EquipmentInteractor)EquipmentInteractor.instance).grabRadius - slingshot.dummyProjectileColliderRadius) * (slingshot.dummyProjectileInitialScale * Mathf.Abs(((Component)slingshot).transform.lossyScale.x));
	}

	public static Vector3 GetNetworkedLaunchVelocity(this Slingshot slingshot)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Abs(((Component)slingshot).transform.lossyScale.x);
		Vector3 val = slingshot.centerOrigin.position - slingshot.center.position;
		val /= num;
		Vector3 val2 = Mathf.Min(slingshot.springConstant * slingshot.maxDraw, ((Vector3)(ref val)).magnitude * slingshot.springConstant) * ((Vector3)(ref val)).normalized * num;
		Vector3 val3 = slingshot.myRig.LatestVelocity();
		return val2 + val3;
	}
}
