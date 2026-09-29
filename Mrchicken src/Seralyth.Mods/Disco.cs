using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Seralyth.Mods;

public static class Disco
{
	private static List<GameObject> discoLights;

	private static float discoLightHue;

	private static List<GameObject> floorTiles;

	private static float floorHue;

	private static GameObject discoBall;

	private static Light discoBallSpotlight;

	public static void DiscoLights()
	{
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		if (discoLights == null)
		{
			discoLights = new List<GameObject>();
			for (int i = 0; i < 8; i++)
			{
				GameObject val = new GameObject("Seralyth_DiscoLight_" + i);
				Light val2 = val.AddComponent<Light>();
				val2.type = (LightType)2;
				val2.range = 8f;
				val2.intensity = 2f;
				val2.shadows = (LightShadows)0;
				discoLights.Add(val);
			}
		}
		discoLightHue += Time.deltaTime * 0.05f;
		if (discoLightHue > 1f)
		{
			discoLightHue -= 1f;
		}
		Vector3 position = ((Component)GorillaTagger.Instance.headCollider).transform.position;
		for (int j = 0; j < discoLights.Count; j++)
		{
			float num = Time.time * (40f + (float)j * 20f) + (float)j * 0.785f;
			float num2 = Mathf.Sin(Time.time * (0.5f + (float)j * 0.15f) + (float)j) * 1.5f;
			float num3 = 2f + (float)j * 0.3f;
			discoLights[j].transform.position = position + new Vector3(Mathf.Sin(num * (MathF.PI / 180f)) * num3, num2 + 1f, Mathf.Cos(num * (MathF.PI / 180f)) * num3);
			discoLights[j].GetComponent<Light>().color = Color.HSVToRGB((discoLightHue + (float)j * 0.125f) % 1f, 1f, 1f);
		}
	}

	public static void FixDiscoLights()
	{
		if (discoLights == null)
		{
			return;
		}
		foreach (GameObject discoLight in discoLights)
		{
			Object.Destroy((Object)(object)discoLight);
		}
		discoLights = null;
	}

	public static void DiscoFloor()
	{
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		if (floorTiles == null)
		{
			floorTiles = new List<GameObject>();
			for (int i = -4; i <= 4; i++)
			{
				for (int j = -4; j <= 4; j++)
				{
					GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
					((Object)val).name = "Seralyth_DiscoFloor";
					val.transform.localScale = new Vector3(0.45f, 0.05f, 0.45f);
					Object.Destroy((Object)(object)val.GetComponent<Collider>());
					floorTiles.Add(val);
				}
			}
		}
		floorHue += Time.deltaTime * 0.03f;
		if (floorHue > 1f)
		{
			floorHue -= 1f;
		}
		Vector3 position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position;
		position.y = ((Component)GorillaTagger.Instance.bodyCollider).transform.position.y - 0.1f;
		int num = 0;
		for (int k = -4; k <= 4; k++)
		{
			for (int l = -4; l <= 4; l++)
			{
				Vector3 position2 = position + new Vector3((float)k * 0.5f, 0f, (float)l * 0.5f);
				floorTiles[num].transform.position = position2;
				float num2 = Mathf.Sqrt((float)(k * k + l * l));
				Color color = Color.HSVToRGB((floorHue + num2 * 0.1f) % 1f, 1f, 1f);
				floorTiles[num].GetComponent<Renderer>().material.color = color;
				floorTiles[num].GetComponent<Renderer>().material.shader = Shader.Find("GorillaTag/UberShader");
				num++;
			}
		}
	}

	public static void FixDiscoFloor()
	{
		if (floorTiles == null)
		{
			return;
		}
		foreach (GameObject floorTile in floorTiles)
		{
			Object.Destroy((Object)(object)floorTile);
		}
		floorTiles = null;
	}

	public static void DiscoBall()
	{
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)discoBall == (Object)null)
		{
			discoBall = GameObject.CreatePrimitive((PrimitiveType)0);
			((Object)discoBall).name = "Seralyth_DiscoBall";
			discoBall.transform.localScale = Vector3.one * 0.4f;
			Object.Destroy((Object)(object)discoBall.GetComponent<Collider>());
			Renderer component = discoBall.GetComponent<Renderer>();
			byte[] array = File.ReadAllBytes("C:\\Users\\kalew\\OneDrive\\Pictures\\Screenshots\\d8cb5dbe-541d-4738-895c-432900a2f706.png");
			Texture2D val = new Texture2D(2, 2);
			ImageConversion.LoadImage(val, array);
			((Texture)val).wrapMode = (TextureWrapMode)1;
			component.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
			component.material.SetTexture("_BaseMap", (Texture)(object)val);
			component.material.color = Color.white;
			discoBallSpotlight = new GameObject("Seralyth_DiscoSpotlight").AddComponent<Light>();
			discoBallSpotlight.type = (LightType)0;
			discoBallSpotlight.range = 15f;
			discoBallSpotlight.intensity = 3f;
			discoBallSpotlight.spotAngle = 60f;
			discoBallSpotlight.shadows = (LightShadows)0;
		}
		Vector3 position = ((Component)GorillaTagger.Instance.headCollider).transform.position;
		Vector3 position2 = position + Vector3.up * 2.5f;
		discoBall.transform.position = position2;
		discoBall.transform.Rotate(Vector3.up, 120f * Time.deltaTime);
		((Component)discoBallSpotlight).transform.position = position2;
		((Component)discoBallSpotlight).transform.rotation = Quaternion.Euler(Mathf.Sin(Time.time * 0.5f) * 30f, Time.time * 100f, 0f);
		discoBallSpotlight.color = Color.HSVToRGB(Time.time * 0.04f % 1f, 1f, 1f);
	}

	public static void FixDiscoBall()
	{
		if ((Object)(object)discoBall != (Object)null)
		{
			Object.Destroy((Object)(object)discoBall);
			discoBall = null;
		}
		if ((Object)(object)discoBallSpotlight != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)discoBallSpotlight).gameObject);
			discoBallSpotlight = null;
		}
	}

	public static void PartyMode()
	{
		DiscoLights();
		DiscoFloor();
		DiscoBall();
	}

	public static void FixPartyMode()
	{
		FixDiscoLights();
		FixDiscoFloor();
		FixDiscoBall();
	}
}
