using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using Photon.Realtime;
using Seralyth.Menu;
using UnityEngine;
using UnityEngine.XR;

namespace Seralyth.Managers;

public class GunLib : MonoBehaviour
{
	public class GunLibData
	{
		public bool IsGripping { get; set; }

		public bool IsTriggered { get; set; }

		public Vector3 HitPos { get; set; }

		public VRRig LockedRig { get; set; }

		public VRRig LastLockedRig { get; set; }

		public bool GunReady { get; set; }

		public Collider Collider { get; set; }

		public GunLibData(bool gripped = false, bool triggered = false, Vector3 hitpos = default(Vector3), VRRig player = null, VRRig lastPlr = null, bool gunReady = false, Collider cPoint = null)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			IsGripping = gripped;
			IsTriggered = triggered;
			HitPos = hitpos;
			LockedRig = player;
			LastLockedRig = lastPlr;
			GunReady = gunReady;
			Collider = cPoint;
		}
	}

	public class RemoteGunPointer
	{
		public GameObject pointer;

		public Material pointerMat;

		public LineRenderer line;

		public Vector3 handPos;

		public Vector3 endPos;

		public Vector3 targetHandPos;

		public Vector3 targetEndPos;

		public Vector3 lastRigPos;

		public bool triggered;

		public bool active;

		public float lastUpdateTime;

		public Player player;
	}

	public static GunLibData data = new GunLibData();

	public static GameObject pObj;

	public static LineRenderer gunLine;

	public static Vector3 determinePos;

	public static Vector3 endPoint;

	public static Material pColor;

	public static TrailRenderer gunTrail;

	public static readonly Dictionary<int, GameObject> GunPtr = new Dictionary<int, GameObject>();

	public static bool rightGunHand;

	public static Color Default;

	public static Color Selected;

	private static Mesh originalSphereMesh;

	private static Mesh cachedCircleMesh;

	private static Mesh cachedSquareMesh;

	private static Mesh cachedTriangleMesh;

	private static Mesh cachedStarMesh;

	private static int cachedShapeIndex = -1;

	private static GameObject lockShape;

	private static Material lockShapeMat;

	private static int lockShapeActiveIndex = -1;

	public static readonly string[] bypassLayers = new string[8] { "Gorilla Trigger", "Gorilla Boundary", "GorillaHand", "GorillaObject", "Zone", "Water", "GorillaCosmetics", "GorillaParticle" };

	public static readonly LayerMask BypassLayers = LayerMask.op_Implicit(~LayerMask.GetMask(bypassLayers));

	public static readonly Dictionary<int, RemoteGunPointer> remoteGunPointers = new Dictionary<int, RemoteGunPointer>();

	public void Start()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		InitpObjs();
		Default = Main.backgroundColor.GetColor(0);
		Selected = Main.buttonColors[1].GetColor(0);
	}

	public static void ResetGL()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		GameObject obj = pObj;
		if (obj != null)
		{
			obj.SetActive(false);
		}
		HideLockShape();
		if (PhotonNetwork.InRoom)
		{
			PhotonNetwork.RaiseEvent((byte)22, (object)null, new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)1
			}, SendOptions.SendUnreliable);
		}
	}

	public static GameObject InitpObjs()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Expected O, but got Unknown
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Expected O, but got Unknown
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		pObj = GameObject.CreatePrimitive((PrimitiveType)0);
		Object.Destroy((Object)(object)pObj.GetComponent<Rigidbody>());
		Object.Destroy((Object)(object)pObj.GetComponent<SphereCollider>());
		pObj.transform.localScale = Vector3.one * 0.3f / 2f;
		Renderer component = pObj.GetComponent<Renderer>();
		component.material.shader = Shader.Find("GUI/Text Shader");
		component.material.color = Default;
		pColor = component.material;
		originalSphereMesh = pObj.GetComponent<MeshFilter>().sharedMesh;
		pObj.SetActive(false);
		gunLine = GTExt.GetOrAddComponent<LineRenderer>(pObj);
		((Renderer)gunLine).material.shader = Shader.Find("GUI/Text Shader");
		gunLine.startWidth = 0.006f;
		gunLine.useWorldSpace = true;
		((Renderer)gunLine).material.color = Default;
		gunLine.positionCount = 51;
		gunLine.startColor = Color.white;
		gunLine.endColor = Color.white;
		((Renderer)gunLine).enabled = true;
		gunTrail = pObj.AddComponent<TrailRenderer>();
		gunTrail.time = 1f;
		gunTrail.startWidth = 0.05f;
		gunTrail.endWidth = 0f;
		((Renderer)gunTrail).material = new Material(Shader.Find("Sprites/Default"));
		gunTrail.numCapVertices = 2;
		gunTrail.numCornerVertices = 2;
		Gradient val = new Gradient();
		val.SetKeys((GradientColorKey[])(object)new GradientColorKey[2]
		{
			new GradientColorKey(Color.white, 0f),
			new GradientColorKey(Color.white, 1f)
		}, (GradientAlphaKey[])(object)new GradientAlphaKey[2]
		{
			new GradientAlphaKey(1f, 0f),
			new GradientAlphaKey(0f, 1f)
		});
		gunTrail.colorGradient = val;
		gunTrail.emitting = false;
		return pObj;
	}

	public static bool DetermineGunHand(bool trigger)
	{
		return (!rightGunHand) ? (trigger ? Main.leftTriggerPressed : Main.leftGrab) : (trigger ? Main.rightTriggerPressed : Main.rightGrab);
	}

	public static Transform DetermineHand()
	{
		return rightGunHand ? GTPlayer.Instance.GetControllerTransform(false) : GTPlayer.Instance.GetControllerTransform(true);
	}

	public static void SendGunData()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Expected O, but got Unknown
		if (PhotonNetwork.InRoom)
		{
			Vector3 val = Vector3.zero;
			Vector3 val2 = Vector3.zero;
			bool flag = false;
			int num = 0;
			if (Main.GunActiveThisFrame)
			{
				val = Main.GunStartPos;
				val2 = Main.GunEndPos;
				flag = true;
				num = ((Main.GetGunInput(isShooting: true) || Main.gunLocked) ? 1 : 0);
			}
			else if ((Object)(object)pObj != (Object)null && pObj.activeSelf)
			{
				val = ((Component)GorillaTagger.Instance.headCollider).transform.position;
				val2 = endPoint;
				flag = true;
				num = (data.IsTriggered ? 1 : 0);
			}
			if (flag)
			{
				object[] array = new object[8] { "seralyth_netmenu_gundata", val.x, val.y, val.z, val2.x, val2.y, val2.z, num };
				PhotonNetwork.RaiseEvent((byte)71, (object)array, new RaiseEventOptions
				{
					Receivers = (ReceiverGroup)0
				}, SendOptions.SendUnreliable);
			}
		}
	}

	public static void HandleRemoteGunData(Player sender, object[] args)
	{
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Expected O, but got Unknown
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		if (args.Length >= 8)
		{
			float num = Convert.ToSingle(args[1]);
			float num2 = Convert.ToSingle(args[2]);
			float num3 = Convert.ToSingle(args[3]);
			float num4 = Convert.ToSingle(args[4]);
			float num5 = Convert.ToSingle(args[5]);
			float num6 = Convert.ToSingle(args[6]);
			bool flag = Convert.ToInt32(args[7]) == 1;
			Vector3 targetHandPos = default(Vector3);
			((Vector3)(ref targetHandPos))._002Ector(num, num2, num3);
			Vector3 targetEndPos = default(Vector3);
			((Vector3)(ref targetEndPos))._002Ector(num4, num5, num6);
			int actorNumber = sender.ActorNumber;
			if (!remoteGunPointers.TryGetValue(actorNumber, out var value))
			{
				value = new RemoteGunPointer();
				remoteGunPointers[actorNumber] = value;
			}
			value.lastUpdateTime = Time.time;
			value.player = sender;
			if ((Object)(object)value.pointer == (Object)null)
			{
				value.pointer = GameObject.CreatePrimitive((PrimitiveType)0);
				Object.Destroy((Object)(object)value.pointer.GetComponent<Rigidbody>());
				Object.Destroy((Object)(object)value.pointer.GetComponent<SphereCollider>());
				value.pointer.transform.localScale = Vector3.one * 0.3f / 2f;
				Renderer component = value.pointer.GetComponent<Renderer>();
				component.material.shader = Shader.Find("GUI/Text Shader");
				component.material.color = Color.white;
				value.pointerMat = component.material;
				GameObject val = new GameObject("RemoteGunLine_" + actorNumber);
				value.line = val.AddComponent<LineRenderer>();
				((Renderer)value.line).material.shader = Shader.Find("GUI/Text Shader");
				value.line.startWidth = 0.006f;
				value.line.endWidth = 0.006f;
				value.line.useWorldSpace = true;
				value.line.positionCount = 51;
				value.line.startColor = Color.white;
				value.line.endColor = Color.white;
			}
			Color currentColor = Main.buttonColors[0].GetCurrentColor();
			if (flag)
			{
				currentColor = Main.buttonColors[1].GetCurrentColor();
			}
			value.pointerMat.color = currentColor;
			value.line.startColor = currentColor;
			value.line.endColor = currentColor;
			value.targetHandPos = targetHandPos;
			value.targetEndPos = targetEndPos;
			value.triggered = flag;
			value.active = true;
		}
	}

	public static void UpdateRemoteGunPointers()
	{
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		List<int> list = null;
		foreach (KeyValuePair<int, RemoteGunPointer> remoteGunPointer in remoteGunPointers)
		{
			RemoteGunPointer value = remoteGunPointer.Value;
			if (Time.time - value.lastUpdateTime > 0.5f)
			{
				if (list == null)
				{
					list = new List<int>();
				}
				list.Add(remoteGunPointer.Key);
			}
			else if (value.active && !((Object)(object)value.pointer == (Object)null))
			{
				VRRig val = ((value.player != null) ? GorillaGameManager.StaticFindRigForPlayer(NetPlayer.op_Implicit(value.player)) : null);
				if ((Object)(object)val != (Object)null)
				{
					value.targetHandPos += ((Component)val).transform.position - value.lastRigPos;
					value.targetEndPos += ((Component)val).transform.position - value.lastRigPos;
					value.lastRigPos = ((Component)val).transform.position;
				}
				value.handPos = Vector3.Lerp(value.handPos, value.targetHandPos, Time.deltaTime * 15f);
				value.endPos = Vector3.Lerp(value.endPos, value.targetEndPos, Time.deltaTime * 15f);
				value.pointer.transform.position = value.endPos;
				Vector3 val2 = (value.handPos + value.endPos) * 0.5f;
				for (int i = 0; i < value.line.positionCount; i++)
				{
					float num = (float)i / (float)(value.line.positionCount - 1);
					Vector3 val3 = Vector3.Lerp(value.handPos, val2, num);
					Vector3 val4 = Vector3.Lerp(val2, value.endPos, num);
					value.line.SetPosition(i, Vector3.Lerp(val3, val4, num));
				}
			}
		}
		if (list == null)
		{
			return;
		}
		foreach (int item in list)
		{
			if (remoteGunPointers.TryGetValue(item, out var value2))
			{
				if ((Object)(object)value2.pointer != (Object)null)
				{
					Object.Destroy((Object)(object)value2.pointer);
				}
				if ((Object)(object)value2.line != (Object)null)
				{
					Object.Destroy((Object)(object)((Component)value2.line).gameObject);
				}
				remoteGunPointers.Remove(item);
			}
		}
	}

	public static void ClearRemoteGunPointers()
	{
		foreach (KeyValuePair<int, RemoteGunPointer> remoteGunPointer in remoteGunPointers)
		{
			if ((Object)(object)remoteGunPointer.Value.pointer != (Object)null)
			{
				Object.Destroy((Object)(object)remoteGunPointer.Value.pointer);
			}
			if ((Object)(object)remoteGunPointer.Value.line != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)remoteGunPointer.Value.line).gameObject);
			}
		}
		remoteGunPointers.Clear();
	}

	private static Mesh GetCircleMesh()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)cachedCircleMesh != (Object)null)
		{
			return cachedCircleMesh;
		}
		Mesh val = new Mesh();
		int num = 32;
		Vector3[] array = (Vector3[])(object)new Vector3[num + 1];
		int[] array2 = new int[num * 3];
		array[0] = Vector3.zero;
		for (int i = 0; i < num; i++)
		{
			float num2 = (float)i / (float)num * MathF.PI * 2f;
			array[i + 1] = new Vector3(Mathf.Cos(num2) * 0.5f, 0f, Mathf.Sin(num2) * 0.5f);
		}
		for (int j = 0; j < num; j++)
		{
			array2[j * 3] = 0;
			array2[j * 3 + 1] = j + 1;
			array2[j * 3 + 2] = (j + 1) % num + 1;
		}
		val.vertices = array;
		val.triangles = array2;
		val.RecalculateNormals();
		cachedCircleMesh = val;
		return val;
	}

	private static Mesh GetSquareMesh()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)cachedSquareMesh != (Object)null)
		{
			return cachedSquareMesh;
		}
		float num = 0.5f;
		Mesh val = new Mesh();
		val.vertices = (Vector3[])(object)new Vector3[4]
		{
			new Vector3(0f - num, 0f, 0f - num),
			new Vector3(0f - num, 0f, num),
			new Vector3(num, 0f, num),
			new Vector3(num, 0f, 0f - num)
		};
		val.triangles = new int[6] { 0, 1, 2, 0, 2, 3 };
		val.RecalculateNormals();
		cachedSquareMesh = val;
		return val;
	}

	private static Mesh GetTriangleMesh()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)cachedTriangleMesh != (Object)null)
		{
			return cachedTriangleMesh;
		}
		float num = 0.5f;
		Mesh val = new Mesh();
		val.vertices = (Vector3[])(object)new Vector3[3]
		{
			new Vector3(0f, 0f, num),
			new Vector3((0f - num) * 0.866f, 0f, (0f - num) * 0.5f),
			new Vector3(num * 0.866f, 0f, (0f - num) * 0.5f)
		};
		val.triangles = new int[3] { 0, 1, 2 };
		val.RecalculateNormals();
		cachedTriangleMesh = val;
		return val;
	}

	private static Mesh GetStarMesh()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)cachedStarMesh != (Object)null)
		{
			return cachedStarMesh;
		}
		Mesh val = new Mesh();
		int num = 5;
		float num2 = 0.5f;
		float num3 = 0.2f;
		Vector3[] array = (Vector3[])(object)new Vector3[num * 2 + 1];
		int[] array2 = new int[num * 6];
		array[0] = Vector3.zero;
		for (int i = 0; i < num; i++)
		{
			float num4 = (float)i / (float)num * MathF.PI * 2f - MathF.PI / 2f;
			float num5 = num4 + MathF.PI / (float)num;
			array[i * 2 + 1] = new Vector3(Mathf.Cos(num4) * num2, 0f, Mathf.Sin(num4) * num2);
			array[i * 2 + 2] = new Vector3(Mathf.Cos(num5) * num3, 0f, Mathf.Sin(num5) * num3);
		}
		for (int j = 0; j < num; j++)
		{
			int num6 = j * 6;
			int num7 = j * 2 + 1;
			int num8 = (num7 + 2) % (num * 2 + 1);
			if (num8 == 0)
			{
				num8 = 1;
			}
			array2[num6] = 0;
			array2[num6 + 1] = num7;
			array2[num6 + 2] = num7 + 1;
			array2[num6 + 3] = 0;
			array2[num6 + 4] = num7 + 1;
			array2[num6 + 5] = num8;
		}
		val.vertices = array;
		val.triangles = array2;
		val.RecalculateNormals();
		cachedStarMesh = val;
		return val;
	}

	private static void SetTrailColor(Color c)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		Gradient val = new Gradient();
		val.SetKeys((GradientColorKey[])(object)new GradientColorKey[2]
		{
			new GradientColorKey(c, 0f),
			new GradientColorKey(c, 1f)
		}, (GradientAlphaKey[])(object)new GradientAlphaKey[2]
		{
			new GradientAlphaKey(1f, 0f),
			new GradientAlphaKey(0f, 1f)
		});
		gunTrail.colorGradient = val;
		gunTrail.startColor = c;
		gunTrail.endColor = c;
	}

	private static void ShowLockShape(Vector3 position, int shapeIndex)
	{
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		if (shapeIndex <= 0)
		{
			HideLockShape();
			return;
		}
		if ((Object)(object)lockShape == (Object)null)
		{
			lockShape = GameObject.CreatePrimitive((PrimitiveType)0);
			Object.Destroy((Object)(object)lockShape.GetComponent<Rigidbody>());
			Object.Destroy((Object)(object)lockShape.GetComponent<SphereCollider>());
			lockShape.transform.localScale = Vector3.one * 0.15f;
			Renderer component = lockShape.GetComponent<Renderer>();
			component.material.shader = Shader.Find("GUI/Text Shader");
			component.material.color = Selected;
			lockShapeMat = component.material;
			((Object)lockShape).name = "GunLibLockShape";
		}
		lockShape.SetActive(true);
		lockShape.transform.position = position + Vector3.up * 0.5f;
		lockShape.transform.Rotate(Vector3.up, Time.deltaTime * 90f);
		if (shapeIndex != lockShapeActiveIndex)
		{
			lockShapeActiveIndex = shapeIndex;
			MeshFilter component2 = lockShape.GetComponent<MeshFilter>();
			switch (shapeIndex)
			{
			case 1:
				component2.sharedMesh = GetCircleMesh();
				break;
			case 2:
				component2.sharedMesh = GetSquareMesh();
				break;
			case 3:
				component2.sharedMesh = GetTriangleMesh();
				break;
			case 4:
				component2.sharedMesh = GetStarMesh();
				break;
			}
		}
		if ((Object)(object)lockShapeMat != (Object)null)
		{
			lockShapeMat.color = Selected;
		}
	}

	private static void HideLockShape()
	{
		if ((Object)(object)lockShape != (Object)null)
		{
			lockShape.SetActive(false);
		}
		lockShapeActiveIndex = -1;
	}

	public static GunLibData GunInstance(bool lockable = false)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		Ray val;
		Vector3 val2;
		if (!XRSettings.isDeviceActive)
		{
			val = GameObject.Find("Shoulder Camera").GetComponent<Camera>().ScreenPointToRay(UnityInput.mousePosition);
			val2 = ((Ray)(ref val)).origin;
		}
		else
		{
			val2 = ((Component)GorillaTagger.Instance.headCollider).transform.position;
		}
		Vector3 val3 = val2;
		Vector3 val4;
		if (!XRSettings.isDeviceActive)
		{
			val = GameObject.Find("Shoulder Camera").GetComponent<Camera>().ScreenPointToRay(UnityInput.mousePosition);
			val4 = ((Ray)(ref val)).direction;
		}
		else
		{
			val4 = ((Component)GorillaTagger.Instance.headCollider).transform.forward;
		}
		Vector3 val5 = val4;
		data.IsGripping = (XRSettings.isDeviceActive ? DetermineGunHand(trigger: false) : UnityInput.GetMouseButton(1));
		data.IsTriggered = (XRSettings.isDeviceActive ? DetermineGunHand(trigger: true) : UnityInput.GetMouseButton(0));
		if (data.IsGripping)
		{
			RaycastHit val6 = default(RaycastHit);
			Physics.Raycast(val3, val5, ref val6, float.PositiveInfinity, LayerMask.op_Implicit(BypassLayers));
			if (lockable)
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val6)).collider).GetComponentInParent<VRRig>();
				if (!Object.op_Implicit((Object)(object)data.LockedRig))
				{
					if (Object.op_Implicit((Object)(object)componentInParent) && data.IsTriggered)
					{
						data.LockedRig = componentInParent;
					}
					determinePos = ((data.IsTriggered && Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.isOfflineVRRig) ? ((Component)data.LockedRig).transform.position : ((RaycastHit)(ref val6)).point);
					Material obj = pColor;
					Color color = (((Renderer)gunLine).material.color = Default);
					obj.color = color;
					if (Main.GunLibTrail)
					{
						SetTrailColor(Default);
					}
					HideLockShape();
				}
				else if (data.IsTriggered && Object.op_Implicit((Object)(object)data.LockedRig))
				{
					data.GunReady = true;
					Vector3 val8 = (data.HitPos = ((Component)data.LockedRig).transform.position);
					determinePos = val8;
					Material obj2 = pColor;
					Color color = (((Renderer)gunLine).material.color = Selected);
					obj2.color = color;
					if (Main.GunLibTrail)
					{
						SetTrailColor(Selected);
					}
					ShowLockShape(((Component)data.LockedRig).transform.position, Main.GunLibShape);
				}
				else
				{
					determinePos = ((RaycastHit)(ref val6)).point;
					data.GunReady = false;
					data.LastLockedRig = data.LockedRig;
					data.LockedRig = null;
					Material obj3 = pColor;
					Color color = (((Renderer)gunLine).material.color = Default);
					obj3.color = color;
					if (Main.GunLibTrail)
					{
						SetTrailColor(Default);
					}
					HideLockShape();
				}
			}
			else
			{
				data.HitPos = ((RaycastHit)(ref val6)).point;
				determinePos = data.HitPos;
				Material obj4 = pColor;
				Color color = (((Renderer)gunLine).material.color = Default);
				obj4.color = color;
				if (Main.GunLibTrail)
				{
					gunTrail.startColor = Default;
					gunTrail.endColor = Default;
				}
				data.GunReady = data.IsTriggered;
				data.Collider = ((RaycastHit)(ref val6)).collider;
				HideLockShape();
			}
			endPoint = Vector3.Lerp(endPoint, determinePos, Time.deltaTime * 12f);
			if (Main.GunLibLine)
			{
				((Renderer)gunLine).enabled = true;
				Vector3 val11 = (((Component)GorillaTagger.Instance.headCollider).transform.position + endPoint) * 0.5f;
				for (int i = 0; i < gunLine.positionCount; i++)
				{
					float num = (float)i / (float)(gunLine.positionCount - 1);
					Vector3 val12 = Vector3.Lerp(((Component)GorillaTagger.Instance.headCollider).transform.position, val11, num);
					Vector3 val13 = Vector3.Lerp(val11, endPoint, num);
					gunLine.SetPosition(i, Vector3.Lerp(val12, val13, num));
				}
				pObj.transform.position = gunLine.GetPosition(gunLine.positionCount - 1);
			}
			else
			{
				((Renderer)gunLine).enabled = false;
				pObj.transform.position = endPoint;
			}
			pObj.SetActive(true);
			if ((Object)(object)gunTrail != (Object)null)
			{
				gunTrail.emitting = Main.GunLibTrail;
			}
		}
		else
		{
			ResetGL();
		}
		return data;
	}
}
