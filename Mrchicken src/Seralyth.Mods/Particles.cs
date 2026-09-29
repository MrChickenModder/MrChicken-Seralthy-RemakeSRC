using GorillaLocomotion;
using Seralyth.Managers;
using UnityEngine;

namespace Seralyth.Mods;

internal class Particles
{
	private static bool isIceSpearCast = false;

	private static float iceSpearSpeed = 15f;

	private static bool hasCastLightning = false;

	private static bool isFireballCast = false;

	private static float fireballSpeed = 10f;

	private static float timeSinceLastStrike = 0f;

	private static float strikeInterval = 1f;

	private static bool hasCastVoidRift = false;

	private static bool hasCastFrostOrb = false;

	public static void CreateDomain2()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(-63.2589f, 9.4352f, -65.2775f);
		for (int i = 0; i < 1; i++)
		{
			GameObject val2 = new GameObject("LineRenderer_" + i);
			LineRenderer val3 = val2.AddComponent<LineRenderer>();
			val3.startWidth = 0.05f;
			val3.endWidth = 0.05f;
			((Renderer)val3).material = new Material(Shader.Find("Unlit/Color"));
			Color endColor = (val3.startColor = GetRandomColor());
			val3.endColor = endColor;
			val3.positionCount = 2;
			Vector3 val4 = val + new Vector3(Random.Range(-25f, 25f), Random.Range(-25f, 25f), Random.Range(-25f, 25f));
			Vector3 val5 = val + new Vector3(Random.Range(-25f, 25f), Random.Range(-10f, 10f), Random.Range(-25f, 25f));
			val3.SetPosition(0, val4);
			val3.SetPosition(1, val5);
			Object.Destroy((Object)(object)val2, 1.5f);
		}
		for (int j = 0; j < 2; j++)
		{
			GameObject val6 = GameObject.CreatePrimitive((PrimitiveType)0);
			val6.transform.position = val + new Vector3(Random.Range(-25f, 25f), Random.Range(-25f, 25f), Random.Range(-25f, 25f));
			val6.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
			val6.GetComponent<Collider>().enabled = false;
			Color randomColor2 = GetRandomColor();
			val6.GetComponent<Renderer>().material.color = randomColor2;
			Object.Destroy((Object)(object)val6, 2f);
		}
	}

	public static void CreateFireEffect()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		CreateFireAtPosition(GorillaTagger.Instance.leftHandTransform.position);
		CreateFireAtPosition(GorillaTagger.Instance.rightHandTransform.position);
	}

	private static void CreateFireAtPosition(Vector3 position)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("FireEffect");
		val.transform.position = position;
		ParticleSystem val2 = val.AddComponent<ParticleSystem>();
		MainModule main = val2.main;
		((MainModule)(ref main)).startColor = new MinMaxGradient(Color.red, Color.black);
		((MainModule)(ref main)).startSize = MinMaxCurve.op_Implicit(0.05f);
		((MainModule)(ref main)).startSpeed = MinMaxCurve.op_Implicit(0.25f);
		((MainModule)(ref main)).startLifetime = MinMaxCurve.op_Implicit(1.5f);
		((MainModule)(ref main)).loop = true;
		((MainModule)(ref main)).simulationSpace = (ParticleSystemSimulationSpace)1;
		((MainModule)(ref main)).maxParticles = 30;
		ParticleSystemRenderer component = ((Component)val2).GetComponent<ParticleSystemRenderer>();
		((Renderer)component).material = new Material(Shader.Find("Particles/Standard Unlit"));
		((Renderer)component).material.SetColor("_Color", Color.red);
		EmissionModule emission = val2.emission;
		((EmissionModule)(ref emission)).rateOverTime = MinMaxCurve.op_Implicit(5f);
		ShapeModule shape = val2.shape;
		((ShapeModule)(ref shape)).shapeType = (ParticleSystemShapeType)4;
		((ShapeModule)(ref shape)).angle = 20f;
		((ShapeModule)(ref shape)).radius = 0.1f;
		Object.Destroy((Object)(object)val, 0.5f);
	}

	public static void CreateBlackHole()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(-63.2589f, 9.4352f, -65.2775f);
		GameObject val2 = new GameObject("BlackHoleEffect");
		val2.transform.position = val;
		ParticleSystem val3 = val2.AddComponent<ParticleSystem>();
		MainModule main = val3.main;
		((MainModule)(ref main)).startColor = new MinMaxGradient(new Color(0f, 0f, 0f), new Color(0.1f, 0.1f, 0.1f));
		((MainModule)(ref main)).startSize = MinMaxCurve.op_Implicit(0.4f);
		((MainModule)(ref main)).startSpeed = MinMaxCurve.op_Implicit(0.5f);
		((MainModule)(ref main)).startLifetime = MinMaxCurve.op_Implicit(2f);
		((MainModule)(ref main)).loop = true;
		((MainModule)(ref main)).simulationSpace = (ParticleSystemSimulationSpace)1;
		((MainModule)(ref main)).maxParticles = 150;
		ParticleSystemRenderer component = ((Component)val3).GetComponent<ParticleSystemRenderer>();
		((Renderer)component).material = new Material(Shader.Find("Particles/Standard Unlit"));
		EmissionModule emission = val3.emission;
		((EmissionModule)(ref emission)).rateOverTime = MinMaxCurve.op_Implicit(30f);
		ShapeModule shape = val3.shape;
		((ShapeModule)(ref shape)).shapeType = (ParticleSystemShapeType)0;
		((ShapeModule)(ref shape)).radius = 2.5f;
		((ShapeModule)(ref shape)).randomDirectionAmount = 0.1f;
		RotationOverLifetimeModule rotationOverLifetime = val3.rotationOverLifetime;
		((RotationOverLifetimeModule)(ref rotationOverLifetime)).z = new MinMaxCurve(0.5f, 1f);
		VelocityOverLifetimeModule velocityOverLifetime = val3.velocityOverLifetime;
		((VelocityOverLifetimeModule)(ref velocityOverLifetime)).x = new MinMaxCurve(0f, 0f);
		((VelocityOverLifetimeModule)(ref velocityOverLifetime)).y = new MinMaxCurve(0f, 0f);
		((VelocityOverLifetimeModule)(ref velocityOverLifetime)).z = new MinMaxCurve(-1f, -2f);
		val3.Play();
		Object.Destroy((Object)(object)val3, 2f);
		Rigidbody attachedRigidbody = ((Collider)GTPlayer.Instance.bodyCollider).attachedRigidbody;
		if ((Object)(object)attachedRigidbody != (Object)null)
		{
			Vector3 val4 = val - ((Component)GTPlayer.Instance.bodyCollider).transform.position;
			float magnitude = ((Vector3)(ref val4)).magnitude;
			float num = Mathf.Clamp(1000f / magnitude, 0f, 10f);
			attachedRigidbody.AddForce(((Vector3)(ref val4)).normalized * num * Time.deltaTime, (ForceMode)2);
		}
		Collider[] array = Physics.OverlapSphere(val, 10f);
		Collider[] array2 = array;
		foreach (Collider val5 in array2)
		{
			Rigidbody component2 = ((Component)val5).GetComponent<Rigidbody>();
			if ((Object)(object)component2 != (Object)null)
			{
				Vector3 val6 = val - ((Component)val5).transform.position;
				float magnitude2 = ((Vector3)(ref val6)).magnitude;
				float num2 = Mathf.Clamp(1000f / magnitude2, 0f, 10f);
				component2.AddForce(((Vector3)(ref val6)).normalized * num2 * Time.deltaTime, (ForceMode)2);
			}
		}
	}

	public static void CreateWhiteHole()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(-63.2589f, 9.4352f, -65.2775f);
		GameObject val2 = new GameObject("WhiteHoleEffect");
		val2.transform.position = val;
		ParticleSystem val3 = val2.AddComponent<ParticleSystem>();
		MainModule main = val3.main;
		((MainModule)(ref main)).startColor = new MinMaxGradient(new Color(1f, 1f, 1f), new Color(0.9f, 0.9f, 0.9f));
		((MainModule)(ref main)).startSize = MinMaxCurve.op_Implicit(0.4f);
		((MainModule)(ref main)).startSpeed = MinMaxCurve.op_Implicit(0.5f);
		((MainModule)(ref main)).startLifetime = MinMaxCurve.op_Implicit(2f);
		((MainModule)(ref main)).loop = true;
		((MainModule)(ref main)).simulationSpace = (ParticleSystemSimulationSpace)1;
		((MainModule)(ref main)).maxParticles = 150;
		ParticleSystemRenderer component = ((Component)val3).GetComponent<ParticleSystemRenderer>();
		((Renderer)component).material = new Material(Shader.Find("Particles/Standard Unlit"));
		EmissionModule emission = val3.emission;
		((EmissionModule)(ref emission)).rateOverTime = MinMaxCurve.op_Implicit(30f);
		ShapeModule shape = val3.shape;
		((ShapeModule)(ref shape)).shapeType = (ParticleSystemShapeType)0;
		((ShapeModule)(ref shape)).radius = 2.5f;
		((ShapeModule)(ref shape)).randomDirectionAmount = 0.1f;
		RotationOverLifetimeModule rotationOverLifetime = val3.rotationOverLifetime;
		((RotationOverLifetimeModule)(ref rotationOverLifetime)).z = new MinMaxCurve(0.5f, 1f);
		VelocityOverLifetimeModule velocityOverLifetime = val3.velocityOverLifetime;
		((VelocityOverLifetimeModule)(ref velocityOverLifetime)).x = new MinMaxCurve(0f, 0f);
		((VelocityOverLifetimeModule)(ref velocityOverLifetime)).y = new MinMaxCurve(0f, 0f);
		((VelocityOverLifetimeModule)(ref velocityOverLifetime)).z = new MinMaxCurve(1f, 2f);
		val3.Play();
		Object.Destroy((Object)(object)val3, 2f);
		Rigidbody attachedRigidbody = ((Collider)GTPlayer.Instance.bodyCollider).attachedRigidbody;
		if ((Object)(object)attachedRigidbody != (Object)null)
		{
			Vector3 val4 = ((Component)attachedRigidbody).transform.position - val;
			float magnitude = ((Vector3)(ref val4)).magnitude;
			float num = Mathf.Clamp(1000f / magnitude, 0f, 10f);
			attachedRigidbody.AddForce(((Vector3)(ref val4)).normalized * num * Time.deltaTime, (ForceMode)2);
		}
		Collider[] array = Physics.OverlapSphere(val, 10f);
		Collider[] array2 = array;
		foreach (Collider val5 in array2)
		{
			Rigidbody component2 = ((Component)val5).GetComponent<Rigidbody>();
			if ((Object)(object)component2 != (Object)null)
			{
				Vector3 val6 = ((Component)val5).transform.position - val;
				float magnitude2 = ((Vector3)(ref val6)).magnitude;
				float num2 = Mathf.Clamp(1000f / magnitude2, 0f, 10f);
				component2.AddForce(((Vector3)(ref val6)).normalized * num2 * Time.deltaTime, (ForceMode)2);
			}
		}
	}

	public static void CastMagicSpell()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Expected O, but got Unknown
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		if (((ControllerInputPoller)ControllerInputPoller.instance).rightControllerPrimaryButton)
		{
			Vector3 position = GorillaTagger.Instance.rightHandTransform.position;
			Quaternion rotation = GorillaTagger.Instance.rightHandTransform.rotation;
			GameObject val = new GameObject("MagicSpellEffect");
			val.transform.position = position;
			ParticleSystem val2 = val.AddComponent<ParticleSystem>();
			MainModule main = val2.main;
			((MainModule)(ref main)).startColor = new MinMaxGradient(new Color(0.2f, 0.3f, 1f), new Color(0.6f, 0.8f, 1f));
			((MainModule)(ref main)).startSize = MinMaxCurve.op_Implicit(0.05f);
			((MainModule)(ref main)).startSpeed = MinMaxCurve.op_Implicit(10f);
			((MainModule)(ref main)).startLifetime = MinMaxCurve.op_Implicit(2f);
			((MainModule)(ref main)).loop = false;
			((MainModule)(ref main)).simulationSpace = (ParticleSystemSimulationSpace)1;
			EmissionModule emission = val2.emission;
			((EmissionModule)(ref emission)).rateOverTime = MinMaxCurve.op_Implicit(0f);
			((EmissionModule)(ref emission)).SetBursts((Burst[])(object)new Burst[1]
			{
				new Burst(0f, (short)20)
			});
			ShapeModule shape = val2.shape;
			((ShapeModule)(ref shape)).shapeType = (ParticleSystemShapeType)4;
			((ShapeModule)(ref shape)).angle = 15f;
			((ShapeModule)(ref shape)).radius = 0.5f;
			((Component)val2).transform.rotation = rotation;
			VelocityOverLifetimeModule velocityOverLifetime = val2.velocityOverLifetime;
			((VelocityOverLifetimeModule)(ref velocityOverLifetime)).x = new MinMaxCurve(0f);
			((VelocityOverLifetimeModule)(ref velocityOverLifetime)).y = new MinMaxCurve(0f);
			((VelocityOverLifetimeModule)(ref velocityOverLifetime)).z = new MinMaxCurve(10f);
			ParticleSystemRenderer component = ((Component)val2).GetComponent<ParticleSystemRenderer>();
			((Renderer)component).material = new Material(Shader.Find("Particles/Standard Unlit"));
			((Renderer)component).material.SetColor("_Color", new Color(0.2f, 0.3f, 1f));
			val2.Play();
			Object.Destroy((Object)(object)val, 2f);
		}
	}

	private static Color GetRandomColor()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return new Color(Random.value, Random.value, Random.value);
	}

	public static void SwordSlash()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Expected O, but got Unknown
		if (((ControllerInputPoller)ControllerInputPoller.instance).rightControllerPrimaryButton && !isIceSpearCast)
		{
			isIceSpearCast = true;
			Vector3 position = GorillaTagger.Instance.rightHandTransform.position;
			Quaternion rotation = GorillaTagger.Instance.rightHandTransform.rotation;
			GameObject val = GameObject.CreatePrimitive((PrimitiveType)2);
			val.transform.position = position;
			val.transform.rotation = rotation;
			val.transform.localScale = new Vector3(0.3f, 2f, 0.3f);
			Rigidbody val2 = val.AddComponent<Rigidbody>();
			val2.useGravity = false;
			val2.AddForce(rotation * Vector3.forward * iceSpearSpeed, (ForceMode)2);
			ParticleSystem val3 = val.AddComponent<ParticleSystem>();
			MainModule main = val3.main;
			((MainModule)(ref main)).startColor = new MinMaxGradient(new Color(0.5f, 0.8f, 1f), new Color(0.2f, 0.5f, 1f));
			((MainModule)(ref main)).startSize = MinMaxCurve.op_Implicit(0.3f);
			((MainModule)(ref main)).startLifetime = MinMaxCurve.op_Implicit(1.5f);
			((MainModule)(ref main)).startSpeed = MinMaxCurve.op_Implicit(1f);
			EmissionModule emission = val3.emission;
			((EmissionModule)(ref emission)).rateOverTime = MinMaxCurve.op_Implicit(40f);
			TrailModule trails = val3.trails;
			((TrailModule)(ref trails)).enabled = true;
			((TrailModule)(ref trails)).widthOverTrail = MinMaxCurve.op_Implicit(0.2f);
			ShapeModule shape = val3.shape;
			((ShapeModule)(ref shape)).shapeType = (ParticleSystemShapeType)4;
			((ShapeModule)(ref shape)).angle = 5f;
			ParticleSystemRenderer component = ((Component)val3).GetComponent<ParticleSystemRenderer>();
			((Renderer)component).material = new Material(Shader.Find("Particles/Standard Unlit"));
			Object.Destroy((Object)(object)val, 5f);
		}
		if (!((ControllerInputPoller)ControllerInputPoller.instance).rightControllerPrimaryButton && isIceSpearCast)
		{
			isIceSpearCast = false;
		}
	}

	public static void CastFireballMagic()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Expected O, but got Unknown
		if (((ControllerInputPoller)ControllerInputPoller.instance).rightControllerPrimaryButton && !isFireballCast)
		{
			isFireballCast = true;
			Vector3 position = GorillaTagger.Instance.rightHandTransform.position;
			Quaternion rotation = GorillaTagger.Instance.rightHandTransform.rotation;
			GameObject val = GameObject.CreatePrimitive((PrimitiveType)0);
			val.transform.position = position;
			val.transform.rotation = rotation;
			val.transform.localScale = new Vector3(1f, 1f, 1f);
			Rigidbody val2 = val.AddComponent<Rigidbody>();
			val2.useGravity = false;
			val2.AddForce(rotation * Vector3.forward * fireballSpeed, (ForceMode)2);
			ParticleSystem val3 = val.AddComponent<ParticleSystem>();
			MainModule main = val3.main;
			((MainModule)(ref main)).startColor = new MinMaxGradient(new Color(1f, 0.3f, 0f), new Color(1f, 0.6f, 0f));
			((MainModule)(ref main)).startSize = MinMaxCurve.op_Implicit(0.5f);
			((MainModule)(ref main)).startLifetime = MinMaxCurve.op_Implicit(2f);
			((MainModule)(ref main)).startSpeed = MinMaxCurve.op_Implicit(1f);
			EmissionModule emission = val3.emission;
			((EmissionModule)(ref emission)).rateOverTime = MinMaxCurve.op_Implicit(50f);
			((EmissionModule)(ref emission)).SetBursts((Burst[])(object)new Burst[1]
			{
				new Burst(0f, (short)50)
			});
			ShapeModule shape = val3.shape;
			((ShapeModule)(ref shape)).shapeType = (ParticleSystemShapeType)0;
			((ShapeModule)(ref shape)).radius = 0.5f;
			ParticleSystemRenderer component = ((Component)val3).GetComponent<ParticleSystemRenderer>();
			((Renderer)component).material = new Material(Shader.Find("Particles/Standard Unlit"));
			Object.Destroy((Object)(object)val, 5f);
		}
		if (!((ControllerInputPoller)ControllerInputPoller.instance).rightControllerPrimaryButton && isFireballCast)
		{
			isFireballCast = false;
		}
	}

	public static void CastSparkMagic()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Expected O, but got Unknown
		if (((ControllerInputPoller)ControllerInputPoller.instance).rightControllerPrimaryButton)
		{
			Vector3 position = GorillaTagger.Instance.rightHandTransform.position;
			Quaternion rotation = GorillaTagger.Instance.rightHandTransform.rotation;
			GameObject val = new GameObject("SparkSpellEffect");
			val.transform.position = position;
			ParticleSystem val2 = val.AddComponent<ParticleSystem>();
			MainModule main = val2.main;
			((MainModule)(ref main)).startColor = new MinMaxGradient(new Color(0.8f, 0.8f, 1f), new Color(1f, 0.9f, 0.2f));
			((MainModule)(ref main)).startSize = MinMaxCurve.op_Implicit(0.03f);
			((MainModule)(ref main)).startSpeed = MinMaxCurve.op_Implicit(8f);
			((MainModule)(ref main)).startLifetime = MinMaxCurve.op_Implicit(1.5f);
			((MainModule)(ref main)).loop = false;
			((MainModule)(ref main)).simulationSpace = (ParticleSystemSimulationSpace)1;
			EmissionModule emission = val2.emission;
			((EmissionModule)(ref emission)).rateOverTime = MinMaxCurve.op_Implicit(0f);
			((EmissionModule)(ref emission)).SetBursts((Burst[])(object)new Burst[1]
			{
				new Burst(0f, (short)30)
			});
			ShapeModule shape = val2.shape;
			((ShapeModule)(ref shape)).shapeType = (ParticleSystemShapeType)4;
			((ShapeModule)(ref shape)).angle = 25f;
			((ShapeModule)(ref shape)).radius = 0.05f;
			((Component)val2).transform.rotation = rotation;
			VelocityOverLifetimeModule velocityOverLifetime = val2.velocityOverLifetime;
			((VelocityOverLifetimeModule)(ref velocityOverLifetime)).x = new MinMaxCurve(-1f, 1f);
			((VelocityOverLifetimeModule)(ref velocityOverLifetime)).y = new MinMaxCurve(0f, 1f);
			((VelocityOverLifetimeModule)(ref velocityOverLifetime)).z = new MinMaxCurve(5f, 10f);
			ParticleSystemRenderer component = ((Component)val2).GetComponent<ParticleSystemRenderer>();
			((Renderer)component).material = new Material(Shader.Find("Particles/Standard Unlit"));
			val2.Play();
			Object.Destroy((Object)(object)val, 1.5f);
		}
	}

	public static void CastLightMagic()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Expected O, but got Unknown
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		if (((ControllerInputPoller)ControllerInputPoller.instance).rightControllerPrimaryButton)
		{
			Vector3 position = GorillaTagger.Instance.rightHandTransform.position;
			Quaternion rotation = GorillaTagger.Instance.rightHandTransform.rotation;
			GameObject val = new GameObject("SparkEffect");
			val.transform.position = position;
			ParticleSystem val2 = val.AddComponent<ParticleSystem>();
			MainModule main = val2.main;
			((MainModule)(ref main)).startColor = new MinMaxGradient(Color.yellow, Color.white);
			((MainModule)(ref main)).startSize = MinMaxCurve.op_Implicit(0.05f);
			((MainModule)(ref main)).startSpeed = MinMaxCurve.op_Implicit(6f);
			((MainModule)(ref main)).startLifetime = MinMaxCurve.op_Implicit(1f);
			((MainModule)(ref main)).loop = false;
			((MainModule)(ref main)).simulationSpace = (ParticleSystemSimulationSpace)1;
			EmissionModule emission = val2.emission;
			((EmissionModule)(ref emission)).rateOverTime = MinMaxCurve.op_Implicit(0f);
			((EmissionModule)(ref emission)).SetBursts((Burst[])(object)new Burst[1]
			{
				new Burst(0f, (short)10)
			});
			ShapeModule shape = val2.shape;
			((ShapeModule)(ref shape)).shapeType = (ParticleSystemShapeType)0;
			((ShapeModule)(ref shape)).radius = 0.1f;
			((Component)val2).transform.rotation = rotation;
			VelocityOverLifetimeModule velocityOverLifetime = val2.velocityOverLifetime;
			((VelocityOverLifetimeModule)(ref velocityOverLifetime)).x = new MinMaxCurve(0f);
			((VelocityOverLifetimeModule)(ref velocityOverLifetime)).y = new MinMaxCurve(0f);
			((VelocityOverLifetimeModule)(ref velocityOverLifetime)).z = new MinMaxCurve(6f);
			ParticleSystemRenderer component = ((Component)val2).GetComponent<ParticleSystemRenderer>();
			((Renderer)component).material = new Material(Shader.Find("Particles/Standard Unlit"));
			((Renderer)component).material.SetColor("_Color", Color.yellow);
			val2.Play();
			Object.Destroy((Object)(object)val, 1.5f);
		}
	}

	public static void Draw()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Expected O, but got Unknown
		if (((ControllerInputPoller)ControllerInputPoller.instance).rightControllerPrimaryButton)
		{
			Vector3 position = GorillaTagger.Instance.rightHandTransform.position;
			Quaternion rotation = GorillaTagger.Instance.rightHandTransform.rotation;
			GameObject val = new GameObject("RasenganEffect");
			val.transform.position = position;
			ParticleSystem val2 = val.AddComponent<ParticleSystem>();
			MainModule main = val2.main;
			((MainModule)(ref main)).startColor = new MinMaxGradient(new Color(0.4f, 0.7f, 1f), new Color(0.9f, 0.4f, 1f));
			((MainModule)(ref main)).startSize = MinMaxCurve.op_Implicit(0.05f);
			((MainModule)(ref main)).startSpeed = MinMaxCurve.op_Implicit(0.1f);
			((MainModule)(ref main)).startLifetime = MinMaxCurve.op_Implicit(2f);
			((MainModule)(ref main)).loop = true;
			((MainModule)(ref main)).simulationSpace = (ParticleSystemSimulationSpace)1;
			EmissionModule emission = val2.emission;
			((EmissionModule)(ref emission)).rateOverTime = MinMaxCurve.op_Implicit(100f);
			ShapeModule shape = val2.shape;
			((ShapeModule)(ref shape)).shapeType = (ParticleSystemShapeType)0;
			((ShapeModule)(ref shape)).radius = 0.005f;
			VelocityOverLifetimeModule velocityOverLifetime = val2.velocityOverLifetime;
			((VelocityOverLifetimeModule)(ref velocityOverLifetime)).x = new MinMaxCurve(0f, 0f);
			((VelocityOverLifetimeModule)(ref velocityOverLifetime)).y = new MinMaxCurve(0f, 0f);
			((VelocityOverLifetimeModule)(ref velocityOverLifetime)).z = new MinMaxCurve(8f, 12f);
			((Component)val2).transform.rotation = rotation;
			val.transform.Rotate(Vector3.up, Time.time * 300f, (Space)0);
			ParticleSystemRenderer component = ((Component)val2).GetComponent<ParticleSystemRenderer>();
			((Renderer)component).material = new Material(Shader.Find("Particles/Standard Unlit"));
			val2.Play();
			Object.Destroy((Object)(object)val, 2f);
		}
	}

	public static void CastLightningBolt()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Expected O, but got Unknown
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Expected O, but got Unknown
		if (((ControllerInputPoller)ControllerInputPoller.instance).rightControllerPrimaryButton && !hasCastLightning)
		{
			hasCastLightning = true;
			Vector3 position = GorillaTagger.Instance.rightHandTransform.position;
			Quaternion rotation = GorillaTagger.Instance.rightHandTransform.rotation;
			Vector3 val = position + rotation * Vector3.forward * 10f;
			GameObject val2 = new GameObject("LightningBolt");
			LineRenderer val3 = val2.AddComponent<LineRenderer>();
			val3.positionCount = 10;
			val3.startWidth = 0.3f;
			val3.endWidth = 0.1f;
			((Renderer)val3).material = new Material(Shader.Find("Particles/Standard Unlit"));
			val3.startColor = Color.white;
			val3.endColor = new Color(0.5f, 0.5f, 1f);
			for (int i = 0; i < 10; i++)
			{
				float num = (float)i / 9f;
				Vector3 val4 = Vector3.Lerp(position, val, num);
				val4 += new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0f);
				val3.SetPosition(i, val4);
			}
			GameObject val5 = new GameObject("LightningSparks");
			val5.transform.position = val;
			ParticleSystem val6 = val5.AddComponent<ParticleSystem>();
			MainModule main = val6.main;
			((MainModule)(ref main)).startColor = new MinMaxGradient(Color.white, Color.cyan);
			((MainModule)(ref main)).startSize = MinMaxCurve.op_Implicit(0.5f);
			((MainModule)(ref main)).startLifetime = MinMaxCurve.op_Implicit(0.2f);
			((MainModule)(ref main)).startSpeed = MinMaxCurve.op_Implicit(2f);
			EmissionModule emission = val6.emission;
			((EmissionModule)(ref emission)).rateOverTime = MinMaxCurve.op_Implicit(50f);
			ShapeModule shape = val6.shape;
			((ShapeModule)(ref shape)).shapeType = (ParticleSystemShapeType)0;
			((ShapeModule)(ref shape)).radius = 0.5f;
			ParticleSystemRenderer component = ((Component)val6).GetComponent<ParticleSystemRenderer>();
			((Renderer)component).material = new Material(Shader.Find("Particles/Standard Unlit"));
			val6.Play();
			Object.Destroy((Object)(object)val2, 0.3f);
			Object.Destroy((Object)(object)val5, 0.5f);
		}
		if (!((ControllerInputPoller)ControllerInputPoller.instance).rightControllerPrimaryButton && hasCastLightning)
		{
			hasCastLightning = false;
		}
	}

	public static void CreateNebulaStorm()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Expected O, but got Unknown
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Expected O, but got Unknown
		Vector3 position = default(Vector3);
		((Vector3)(ref position))._002Ector(-63.2589f, 9.4352f, -65.2775f);
		GameObject val = new GameObject("NebulaStorm");
		val.transform.position = position;
		ParticleSystem val2 = val.AddComponent<ParticleSystem>();
		MainModule main = val2.main;
		((MainModule)(ref main)).startColor = new MinMaxGradient(new Color(0.2f, 0.3f, 0.6f, 0.3f), new Color(0.6f, 0.1f, 0.7f, 0.4f));
		((MainModule)(ref main)).startSize = MinMaxCurve.op_Implicit(3f);
		((MainModule)(ref main)).startLifetime = MinMaxCurve.op_Implicit(4f);
		((MainModule)(ref main)).startSpeed = MinMaxCurve.op_Implicit(0f);
		((MainModule)(ref main)).loop = true;
		((MainModule)(ref main)).simulationSpace = (ParticleSystemSimulationSpace)1;
		((MainModule)(ref main)).maxParticles = 100;
		EmissionModule emission = val2.emission;
		((EmissionModule)(ref emission)).rateOverTime = MinMaxCurve.op_Implicit(5f);
		ShapeModule shape = val2.shape;
		((ShapeModule)(ref shape)).shapeType = (ParticleSystemShapeType)0;
		((ShapeModule)(ref shape)).radius = 2f;
		ParticleSystemRenderer component = ((Component)val2).GetComponent<ParticleSystemRenderer>();
		((Renderer)component).material = new Material(Shader.Find("Particles/Standard Unlit"));
		GameObject val3 = new GameObject("NebulaStars");
		val3.transform.parent = val.transform;
		val3.transform.localPosition = Vector3.zero;
		ParticleSystem val4 = val3.AddComponent<ParticleSystem>();
		MainModule main2 = val4.main;
		((MainModule)(ref main2)).startColor = new MinMaxGradient(Color.white, new Color(1f, 1f, 0.8f));
		((MainModule)(ref main2)).startSize = MinMaxCurve.op_Implicit(0.05f);
		((MainModule)(ref main2)).startLifetime = MinMaxCurve.op_Implicit(2f);
		((MainModule)(ref main2)).startSpeed = MinMaxCurve.op_Implicit(0f);
		((MainModule)(ref main2)).loop = true;
		((MainModule)(ref main2)).simulationSpace = (ParticleSystemSimulationSpace)1;
		((MainModule)(ref main2)).maxParticles = 50;
		EmissionModule emission2 = val4.emission;
		((EmissionModule)(ref emission2)).rateOverTime = MinMaxCurve.op_Implicit(10f);
		ShapeModule shape2 = val4.shape;
		((ShapeModule)(ref shape2)).shapeType = (ParticleSystemShapeType)0;
		((ShapeModule)(ref shape2)).radius = 2.5f;
		ParticleSystemRenderer component2 = ((Component)val4).GetComponent<ParticleSystemRenderer>();
		((Renderer)component2).material = new Material(Shader.Find("Particles/Standard Unlit"));
		val2.Play();
		val4.Play();
		Object.Destroy((Object)(object)val, 5f);
	}

	public static void CreateLightningEffect()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		timeSinceLastStrike += Time.deltaTime;
		if (!(timeSinceLastStrike < strikeInterval))
		{
			timeSinceLastStrike = 0f;
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(-63.2589f, 9.4352f, -65.2775f);
			GameObject val2 = new GameObject("LightningStrike");
			val2.transform.position = val;
			LineRenderer val3 = val2.AddComponent<LineRenderer>();
			val3.startWidth = 0.1f;
			val3.endWidth = 0.1f;
			val3.startColor = Color.white;
			val3.endColor = new Color(0.5f, 0.5f, 1f);
			((Renderer)val3).material = new Material(Shader.Find("Sprites/Default"));
			val3.useWorldSpace = true;
			Vector3 val4 = default(Vector3);
			((Vector3)(ref val4))._002Ector(val.x + (float)Random.Range(-25, 25), val.y + Random.Range(10f, 20f), val.z + (float)Random.Range(-25, 25));
			Vector3 val5 = default(Vector3);
			((Vector3)(ref val5))._002Ector(val.x + Random.Range(-10f, 10f), val.y - Random.Range(10f, 20f), val.z + Random.Range(-10f, 10f));
			val3.positionCount = 2;
			val3.SetPosition(0, val4);
			val3.SetPosition(1, val5);
			Light val6 = val2.AddComponent<Light>();
			val6.color = Color.white;
			val6.intensity = Random.Range(4f, 8f);
			val6.range = 10f;
			val6.shadows = (LightShadows)0;
			Object.Destroy((Object)(object)val2, Random.Range(0.1f, 0.5f));
			Object.Destroy((Object)(object)val6, 0.1f);
		}
	}

	public static void CastVoidRift()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Expected O, but got Unknown
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Expected O, but got Unknown
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Expected O, but got Unknown
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Expected O, but got Unknown
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Expected O, but got Unknown
		if (((ControllerInputPoller)ControllerInputPoller.instance).rightControllerPrimaryButton && !hasCastVoidRift)
		{
			hasCastVoidRift = true;
			Vector3 position = GorillaTagger.Instance.rightHandTransform.position;
			Quaternion rotation = GorillaTagger.Instance.rightHandTransform.rotation;
			Vector3 val = rotation * Vector3.forward;
			GameObject val2 = new GameObject("VoidRift");
			val2.transform.position = position + val * 2f;
			val2.transform.rotation = rotation;
			ParticleSystem val3 = val2.AddComponent<ParticleSystem>();
			MainModule main = val3.main;
			((MainModule)(ref main)).startColor = MinMaxGradient.op_Implicit(new Color(0f, 0f, 0f, 1f));
			((MainModule)(ref main)).startSize = MinMaxCurve.op_Implicit(0.6f);
			((MainModule)(ref main)).startLifetime = MinMaxCurve.op_Implicit(1.2f);
			((MainModule)(ref main)).loop = false;
			((MainModule)(ref main)).simulationSpace = (ParticleSystemSimulationSpace)1;
			ShapeModule shape = val3.shape;
			((ShapeModule)(ref shape)).shapeType = (ParticleSystemShapeType)17;
			((ShapeModule)(ref shape)).radius = 0.5f;
			EmissionModule emission = val3.emission;
			((EmissionModule)(ref emission)).rateOverTime = MinMaxCurve.op_Implicit(30f);
			ParticleSystemRenderer component = ((Component)val3).GetComponent<ParticleSystemRenderer>();
			((Renderer)component).material = new Material(Shader.Find("Particles/Standard Unlit"));
			GameObject val4 = new GameObject("RiftLightning");
			val4.transform.parent = val2.transform;
			val4.transform.localPosition = Vector3.zero;
			ParticleSystem val5 = val4.AddComponent<ParticleSystem>();
			MainModule main2 = val5.main;
			((MainModule)(ref main2)).startColor = MinMaxGradient.op_Implicit(Color.white);
			((MainModule)(ref main2)).startSize = MinMaxCurve.op_Implicit(0.15f);
			((MainModule)(ref main2)).startLifetime = MinMaxCurve.op_Implicit(0.3f);
			EmissionModule emission2 = val5.emission;
			((EmissionModule)(ref emission2)).rateOverTime = MinMaxCurve.op_Implicit(10f);
			ShapeModule shape2 = val5.shape;
			((ShapeModule)(ref shape2)).shapeType = (ParticleSystemShapeType)0;
			((ShapeModule)(ref shape2)).radius = 0.6f;
			ParticleSystemRenderer component2 = ((Component)val5).GetComponent<ParticleSystemRenderer>();
			((Renderer)component2).material = new Material(Shader.Find("Particles/Standard Unlit"));
			GameObject val6 = new GameObject("RiftSparks");
			val6.transform.parent = val2.transform;
			val6.transform.localPosition = Vector3.zero;
			ParticleSystem val7 = val6.AddComponent<ParticleSystem>();
			MainModule main3 = val7.main;
			((MainModule)(ref main3)).startColor = MinMaxGradient.op_Implicit(new Color(1f, 0.8f, 0.6f, 1f));
			((MainModule)(ref main3)).startSize = MinMaxCurve.op_Implicit(0.05f);
			((MainModule)(ref main3)).startLifetime = MinMaxCurve.op_Implicit(0.2f);
			((MainModule)(ref main3)).startSpeed = MinMaxCurve.op_Implicit(0.5f);
			EmissionModule emission3 = val7.emission;
			((EmissionModule)(ref emission3)).rateOverTime = MinMaxCurve.op_Implicit(20f);
			ParticleSystemRenderer component3 = ((Component)val7).GetComponent<ParticleSystemRenderer>();
			((Renderer)component3).material = new Material(Shader.Find("Particles/Standard Unlit"));
			val3.Play();
			val5.Play();
			val7.Play();
			Object.Destroy((Object)(object)val2, 1.5f);
		}
		if (!((ControllerInputPoller)ControllerInputPoller.instance).rightControllerPrimaryButton && hasCastVoidRift)
		{
			hasCastVoidRift = false;
		}
	}

	public static void CastFrostOrb()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Expected O, but got Unknown
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Expected O, but got Unknown
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Expected O, but got Unknown
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Expected O, but got Unknown
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Expected O, but got Unknown
		if (((ControllerInputPoller)ControllerInputPoller.instance).rightControllerPrimaryButton && !hasCastFrostOrb)
		{
			hasCastFrostOrb = true;
			Vector3 position = GorillaTagger.Instance.rightHandTransform.position;
			Quaternion rotation = GorillaTagger.Instance.rightHandTransform.rotation;
			Vector3 val = rotation * Vector3.forward;
			GameObject val2 = GameObject.CreatePrimitive((PrimitiveType)0);
			val2.transform.position = position + val * 0.5f;
			val2.transform.rotation = rotation;
			val2.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
			Rigidbody val3 = val2.AddComponent<Rigidbody>();
			val3.useGravity = false;
			val3.linearVelocity = val * 4f;
			Material val4 = new Material(Shader.Find("Particles/Standard Unlit"));
			val4.color = new Color(0.5f, 0.8f, 1f, 1f);
			val2.GetComponent<Renderer>().material = val4;
			GameObject val5 = new GameObject("FrostMist");
			val5.transform.parent = val2.transform;
			val5.transform.localPosition = Vector3.zero;
			ParticleSystem val6 = val5.AddComponent<ParticleSystem>();
			MainModule main = val6.main;
			((MainModule)(ref main)).startColor = MinMaxGradient.op_Implicit(new Color(0.6f, 0.9f, 1f, 0.5f));
			((MainModule)(ref main)).startSize = MinMaxCurve.op_Implicit(0.05f);
			((MainModule)(ref main)).startLifetime = MinMaxCurve.op_Implicit(1.5f);
			((MainModule)(ref main)).startSpeed = MinMaxCurve.op_Implicit(0.3f);
			((MainModule)(ref main)).loop = true;
			((MainModule)(ref main)).simulationSpace = (ParticleSystemSimulationSpace)1;
			EmissionModule emission = val6.emission;
			((EmissionModule)(ref emission)).rateOverTime = MinMaxCurve.op_Implicit(20f);
			ShapeModule shape = val6.shape;
			((ShapeModule)(ref shape)).shapeType = (ParticleSystemShapeType)0;
			((ShapeModule)(ref shape)).radius = 0.2f;
			ParticleSystemRenderer component = ((Component)val6).GetComponent<ParticleSystemRenderer>();
			((Renderer)component).material = new Material(Shader.Find("Particles/Standard Unlit"));
			val6.Play();
			GameObject val7 = new GameObject("FrostSnowflakes");
			val7.transform.parent = val2.transform;
			val7.transform.localPosition = Vector3.zero;
			ParticleSystem val8 = val7.AddComponent<ParticleSystem>();
			MainModule main2 = val8.main;
			((MainModule)(ref main2)).startColor = MinMaxGradient.op_Implicit(Color.white);
			((MainModule)(ref main2)).startSize = MinMaxCurve.op_Implicit(0.05f);
			((MainModule)(ref main2)).startLifetime = MinMaxCurve.op_Implicit(1.5f);
			((MainModule)(ref main2)).startSpeed = MinMaxCurve.op_Implicit(0.2f);
			((MainModule)(ref main2)).loop = true;
			((MainModule)(ref main2)).simulationSpace = (ParticleSystemSimulationSpace)1;
			EmissionModule emission2 = val8.emission;
			((EmissionModule)(ref emission2)).rateOverTime = MinMaxCurve.op_Implicit(30f);
			ShapeModule shape2 = val8.shape;
			((ShapeModule)(ref shape2)).shapeType = (ParticleSystemShapeType)0;
			((ShapeModule)(ref shape2)).radius = 0.3f;
			ParticleSystemRenderer component2 = ((Component)val8).GetComponent<ParticleSystemRenderer>();
			((Renderer)component2).material = new Material(Shader.Find("Particles/Standard Unlit"));
			val8.Play();
			Object.Destroy((Object)(object)val2, 5f);
		}
		if (!((ControllerInputPoller)ControllerInputPoller.instance).rightControllerPrimaryButton && hasCastFrostOrb)
		{
			hasCastFrostOrb = false;
		}
	}

	public static void CreateDomain()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(-63.2589f, 9.4352f, -65.2775f);
		for (int i = 0; i < 30; i++)
		{
			GameObject val2 = new GameObject("LineRenderer_" + i);
			LineRenderer val3 = val2.AddComponent<LineRenderer>();
			val3.startWidth = 0.05f;
			val3.endWidth = 0.05f;
			((Renderer)val3).material = new Material(Shader.Find("Unlit/Color"));
			val3.startColor = new Color(0.5f, 0f, 0f);
			val3.endColor = new Color(0f, 0f, 0f);
			val3.positionCount = 2;
			Vector3 val4 = val + new Vector3(Random.Range(-30f, 30f), Random.Range(-30f, 30f), Random.Range(-30f, 30f));
			val3.SetPosition(0, val);
			val3.SetPosition(1, val4);
			Object.Destroy((Object)(object)val2, 2f);
		}
		for (int j = 0; j < 20; j++)
		{
			GameObject val5 = ((Random.Range(0, 2) != 0) ? GameObject.CreatePrimitive((PrimitiveType)0) : GameObject.CreatePrimitive((PrimitiveType)3));
			val5.transform.position = val + new Vector3(Random.Range(-10f, 10f), Random.Range(-5f, 5f), Random.Range(-10f, 10f));
			val5.transform.localScale = new Vector3(Random.Range(0.2f, 0.5f), Random.Range(0.2f, 0.5f), Random.Range(0.2f, 0.5f));
			val5.GetComponent<Renderer>().material.color = new Color(0.3f, 0f, 0f);
			Object.Destroy((Object)(object)val5.GetComponent<Collider>());
			Object.Destroy((Object)(object)val5, 2f);
		}
	}

	public static void ParticleGun()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		GunLib.GunInstance();
		if (GunLib.data.IsGripping && GunLib.data.IsTriggered)
		{
			FireParticle(GunLib.data.HitPos);
		}
	}

	private static void FireParticle(Vector3 position)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("FireEffect");
		val.transform.position = position;
		ParticleSystem val2 = val.AddComponent<ParticleSystem>();
		MainModule main = val2.main;
		((MainModule)(ref main)).startColor = new MinMaxGradient(Color.red, Color.black);
		((MainModule)(ref main)).startSize = MinMaxCurve.op_Implicit(0.05f);
		((MainModule)(ref main)).startSpeed = MinMaxCurve.op_Implicit(0.25f);
		((MainModule)(ref main)).startLifetime = MinMaxCurve.op_Implicit(1.5f);
		((MainModule)(ref main)).loop = true;
		((MainModule)(ref main)).simulationSpace = (ParticleSystemSimulationSpace)1;
		((MainModule)(ref main)).maxParticles = 30;
		ParticleSystemRenderer component = ((Component)val2).GetComponent<ParticleSystemRenderer>();
		((Renderer)component).material = new Material(Shader.Find("Particles/Standard Unlit"));
		((Renderer)component).material.SetColor("_Color", Color.red);
		EmissionModule emission = val2.emission;
		((EmissionModule)(ref emission)).rateOverTime = MinMaxCurve.op_Implicit(5f);
		ShapeModule shape = val2.shape;
		((ShapeModule)(ref shape)).shapeType = (ParticleSystemShapeType)4;
		((ShapeModule)(ref shape)).angle = 20f;
		((ShapeModule)(ref shape)).radius = 0.1f;
		Object.Destroy((Object)(object)val, 0.5f);
	}
}
