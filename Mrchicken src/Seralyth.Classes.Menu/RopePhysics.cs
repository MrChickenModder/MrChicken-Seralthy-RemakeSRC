using GorillaLocomotion;
using UnityEngine;

namespace Seralyth.Classes.Menu;

public class RopePhysics : MonoBehaviour
{
	public float segmentLength = 0.5f;

	private readonly int solverIterations = 50;

	private readonly float gravity = 9.81f;

	private readonly float damping = 0.98f;

	private readonly float mass = 1f;

	private readonly bool pinStart = true;

	private readonly bool pinEnd = true;

	private Vector3 startPosition;

	private Vector3 endPosition;

	private readonly float airResistance = 0.02f;

	private readonly float stiffness = 1f;

	private readonly bool enableCollisions = false;

	private readonly float collisionRadius = 0.1f;

	private LineRenderer lr;

	private Vector3[] points;

	private Vector3[] prevPoints;

	private Vector3[] accelerations;

	private int pointCount;

	private void Awake()
	{
		lr = ((Component)this).GetComponent<LineRenderer>();
		pointCount = lr.positionCount;
		if (pointCount < 2)
		{
			Object.Destroy((Object)(object)this);
		}
		else
		{
			InitializeRope();
		}
	}

	private void InitializeRope()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		points = (Vector3[])(object)new Vector3[pointCount];
		prevPoints = (Vector3[])(object)new Vector3[pointCount];
		accelerations = (Vector3[])(object)new Vector3[pointCount];
		for (int i = 0; i < pointCount; i++)
		{
			points[i] = lr.GetPosition(i);
			prevPoints[i] = points[i];
			accelerations[i] = Vector3.zero;
		}
		if (pinStart)
		{
			startPosition = points[0];
		}
		if (pinEnd)
		{
			endPosition = points[pointCount - 1];
		}
	}

	private void FixedUpdate()
	{
		float fixedDeltaTime = Time.fixedDeltaTime;
		Simulate(fixedDeltaTime);
		UpdateLineRenderer();
	}

	private void Simulate(float dt)
	{
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < pointCount; i++)
		{
			if ((i != 0 || !pinStart) && (i != pointCount - 1 || !pinEnd))
			{
				Vector3 val = (points[i] - prevPoints[i]) / dt;
				Vector3 val2 = -val * airResistance;
				Vector3 val3 = Vector3.down * gravity * mass;
				accelerations[i] = (val3 + val2) / mass;
				Vector3 val4 = points[i] + val * dt * damping + accelerations[i] * dt * dt;
				if (enableCollisions)
				{
					val4 = HandleCollision(points[i], val4);
				}
				prevPoints[i] = points[i];
				points[i] = val4;
			}
		}
		for (int j = 0; j < solverIterations; j++)
		{
			ApplyConstraints();
		}
		if (pinStart)
		{
			points[0] = startPosition;
			prevPoints[0] = points[0];
		}
		if (pinEnd)
		{
			points[pointCount - 1] = endPosition;
			prevPoints[pointCount - 1] = points[pointCount - 1];
		}
	}

	private void ApplyConstraints()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < pointCount - 1; i++)
		{
			Vector3 val = points[i + 1] - points[i];
			float magnitude = ((Vector3)(ref val)).magnitude;
			if (magnitude == 0f)
			{
				continue;
			}
			float num = (magnitude - segmentLength) / magnitude;
			Vector3 val2 = val * (num * 0.5f * stiffness);
			bool flag = i == 0 && pinStart;
			bool flag2 = i == pointCount - 2 && pinEnd;
			if (!flag)
			{
				if (!flag2)
				{
					ref Vector3 reference = ref points[i];
					reference += val2;
					ref Vector3 reference2 = ref points[i + 1];
					reference2 -= val2;
				}
				else
				{
					ref Vector3 reference3 = ref points[i];
					reference3 += val2 * 2f;
				}
			}
			else if (!flag2)
			{
				ref Vector3 reference4 = ref points[i + 1];
				reference4 -= val2 * 2f;
			}
		}
	}

	private Vector3 HandleCollision(Vector3 oldPos, Vector3 newPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = newPos - oldPos;
		float magnitude = ((Vector3)(ref val)).magnitude;
		RaycastHit val2 = default(RaycastHit);
		if (magnitude < 0.001f || !Physics.SphereCast(oldPos, collisionRadius, ((Vector3)(ref val)).normalized, ref val2, magnitude, LayerMask.op_Implicit(GTPlayer.Instance.locomotionEnabledLayers)))
		{
			return newPos;
		}
		return ((RaycastHit)(ref val2)).point + ((RaycastHit)(ref val2)).normal * collisionRadius;
	}

	private void UpdateLineRenderer()
	{
		lr.SetPositions(points);
	}

	public void ApplyForceToPoint(int pointIndex, Vector3 force)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (pointIndex >= 0 && pointIndex < pointCount)
		{
			Vector3 val = force / mass;
			ref Vector3 reference = ref points[pointIndex];
			reference += val * Time.fixedDeltaTime * Time.fixedDeltaTime;
		}
	}

	public void ApplyImpulseToPoint(int pointIndex, Vector3 impulse)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (pointIndex >= 0 && pointIndex < pointCount)
		{
			Vector3 val = impulse / mass;
			ref Vector3 reference = ref points[pointIndex];
			reference += val * Time.fixedDeltaTime;
		}
	}

	public void SetStartPosition(Vector3 pos)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		startPosition = pos;
		if (pinStart)
		{
			points[0] = pos;
			prevPoints[0] = pos;
		}
	}

	public void SetEndPosition(Vector3 pos)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		endPosition = pos;
		if (pinEnd)
		{
			points[pointCount - 1] = pos;
			prevPoints[pointCount - 1] = pos;
		}
	}

	public void GrabPoint(int pointIndex, Vector3 position)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (pointIndex >= 0 && pointIndex < pointCount)
		{
			points[pointIndex] = position;
			prevPoints[pointIndex] = position;
		}
	}
}
