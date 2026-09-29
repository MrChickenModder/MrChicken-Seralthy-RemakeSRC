using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Seralyth.Classes.Menu;
using Seralyth.Managers;
using Seralyth.Menu;
using Seralyth.Mods;
using Seralyth.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;

namespace Seralyth.Classes.Mods;

public class StumpUpdateDisplay : MonoBehaviour
{
	private GameObject canvasObject;

	private Canvas canvas;

	private TextMeshProUGUI titleText;

	private TextMeshProUGUI contentText;

	private TextMeshProUGUI pageLabelText;

	private int currentPage;

	private int itemsPerPage = 5;

	private List<ChangelogEntry> allEntries;

	private Coroutine autoScrollCoroutine;

	public static bool AutoScrollEnabled = true;

	public static long LastSeenDllTimestamp;

	private static bool HasPoppedThisSession;

	private static readonly Vector3 stumpPosition = new Vector3(-66f, 12f, -79f);

	private LineRenderer pointerLine;

	private GameObject pointerDot;

	private GraphicRaycaster graphicRaycaster;

	private EventSystem eventSystem;

	private PointerEventData pointerData;

	private readonly List<RaycastResult> uiResults = new List<RaycastResult>();

	private GameObject currentHover;

	private GameObject pressedUI;

	private bool lastTriggerClick;

	private Vector2 lastPointerPos;

	private readonly Dictionary<GameObject, Action> buttonActions = new Dictionary<GameObject, Action>();

	public static StumpUpdateDisplay Instance { get; private set; }

	private static long CurrentDllTimestamp => new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime.Ticks;

	private void Awake()
	{
		Instance = this;
		allEntries = new List<ChangelogEntry>();
	}

	private void Start()
	{
		allEntries = new List<ChangelogEntry>(Changelog.Entries);
		allEntries.Reverse();
		if (CurrentDllTimestamp != LastSeenDllTimestamp && !HasPoppedThisSession)
		{
			((MonoBehaviour)this).StartCoroutine(DelayedShow());
		}
	}

	private IEnumerator DelayedShow()
	{
		while ((Object)(object)GorillaTagger.Instance == (Object)null || (Object)(object)GorillaTagger.Instance.mainCamera == (Object)null)
		{
			yield return null;
		}
		Show();
	}

	private void OnDestroy()
	{
		if (autoScrollCoroutine != null)
		{
			((MonoBehaviour)this).StopCoroutine(autoScrollCoroutine);
		}
		if ((Object)(object)pointerLine != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)pointerLine).gameObject);
		}
		if ((Object)(object)pointerDot != (Object)null)
		{
			Object.Destroy((Object)(object)pointerDot);
		}
		if ((Object)(object)Instance == (Object)(object)this)
		{
			Instance = null;
		}
	}

	private void Update()
	{
		if (!((Object)(object)canvasObject == (Object)null))
		{
			Billboard();
			DoPointerInteraction();
		}
	}

	private void Billboard()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = GorillaTagger.Instance.mainCamera.transform;
		Vector3 val = canvasObject.transform.position - transform.position;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		canvasObject.transform.rotation = Quaternion.LookRotation(normalized, Vector3.up);
	}

	private void DoPointerInteraction()
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Expected O, but got Unknown
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)graphicRaycaster == (Object)null)
		{
			return;
		}
		GameObject mainCamera = GorillaTagger.Instance.mainCamera;
		Camera val = ((mainCamera != null) ? mainCamera.GetComponent<Camera>() : null);
		if ((Object)(object)val == (Object)null)
		{
			return;
		}
		if ((Object)(object)canvas.worldCamera == (Object)null)
		{
			canvas.worldCamera = val;
		}
		if ((Object)(object)pointerLine == (Object)null)
		{
			return;
		}
		bool isDeviceActive = XRSettings.isDeviceActive;
		Vector3 val2;
		Vector3 val3;
		if (isDeviceActive)
		{
			(Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) trueRightHand = ControllerUtilities.GetTrueRightHand();
			Vector3 item = trueRightHand.position;
			Quaternion item2 = trueRightHand.rotation;
			Vector3 item3 = trueRightHand.forward;
			val2 = item;
			val3 = item3;
		}
		else
		{
			Ray val4 = val.ScreenPointToRay(Input.mousePosition);
			val2 = ((Ray)(ref val4)).origin + ((Ray)(ref val4)).direction * 0.5f;
			val3 = ((Ray)(ref val4)).direction;
		}
		RectTransform component = ((Component)canvas).GetComponent<RectTransform>();
		Plane val5 = default(Plane);
		((Plane)(ref val5))._002Ector(((Transform)component).forward, ((Transform)component).position);
		Ray val6 = default(Ray);
		((Ray)(ref val6))._002Ector(val2, val3);
		Vector3 val7 = val2 + val3 * 5f;
		float num = default(float);
		if (((Plane)(ref val5)).Raycast(val6, ref num) && num > 0f)
		{
			val7 = ((Ray)(ref val6)).GetPoint(num);
		}
		pointerLine.SetPosition(0, val2);
		pointerLine.SetPosition(1, val7);
		if ((Object)(object)pointerDot != (Object)null)
		{
			pointerDot.transform.position = val7;
		}
		if ((Object)(object)eventSystem == (Object)null)
		{
			eventSystem = EventSystem.current;
		}
		if (pointerData == null)
		{
			pointerData = new PointerEventData(eventSystem);
		}
		Vector3 val8 = val.WorldToScreenPoint(val7);
		if (val8.z < 0f)
		{
			ClearHover();
			return;
		}
		pointerData.position = Vector2.op_Implicit(val8);
		uiResults.Clear();
		((BaseRaycaster)graphicRaycaster).Raycast(pointerData, uiResults);
		object obj;
		if (uiResults.Count <= 0)
		{
			obj = null;
		}
		else
		{
			RaycastResult val9 = uiResults[0];
			obj = ((RaycastResult)(ref val9)).gameObject;
		}
		GameObject val10 = (GameObject)obj;
		if ((Object)(object)val10 != (Object)(object)currentHover)
		{
			if ((Object)(object)currentHover != (Object)null)
			{
				ExecuteEvents.Execute<IPointerExitHandler>(currentHover, (BaseEventData)(object)pointerData, ExecuteEvents.pointerExitHandler);
				SetButtonHighlight(currentHover, highlighted: false);
			}
			if ((Object)(object)val10 != (Object)null)
			{
				ExecuteEvents.Execute<IPointerEnterHandler>(val10, (BaseEventData)(object)pointerData, ExecuteEvents.pointerEnterHandler);
				SetButtonHighlight(val10, highlighted: true);
			}
			currentHover = val10;
		}
		bool flag = ((!isDeviceActive) ? Input.GetMouseButton(0) : (Main.rightTrigger > 0.5f || Main.rightGrab));
		pointerData.delta = pointerData.position - lastPointerPos;
		lastPointerPos = pointerData.position;
		if (flag && !lastTriggerClick && (Object)(object)val10 != (Object)null)
		{
			pressedUI = val10;
			pointerData.pressPosition = pointerData.position;
			pointerData.pointerPressRaycast = uiResults[0];
			ExecuteEvents.Execute<IPointerDownHandler>(val10, (BaseEventData)(object)pointerData, ExecuteEvents.pointerDownHandler);
			pointerData.pointerPress = val10;
			Action buttonAction = GetButtonAction(val10);
			if (buttonAction != null)
			{
				buttonAction();
				string name = ((Object)val10).name;
				if (1 == 0)
				{
				}
				string text = ((name == "PreviousPage") ? "Previous" : ((!(name == "NextPage")) ? "Button" : "Next"));
				if (1 == 0)
				{
				}
				string sound = text;
				SoundManager.Play(sound);
			}
		}
		if (!flag && lastTriggerClick && (Object)(object)pressedUI != (Object)null)
		{
			ExecuteEvents.Execute<IPointerUpHandler>(pressedUI, (BaseEventData)(object)pointerData, ExecuteEvents.pointerUpHandler);
			if ((Object)(object)pressedUI == (Object)(object)val10)
			{
				ExecuteEvents.Execute<IPointerClickHandler>(pressedUI, (BaseEventData)(object)pointerData, ExecuteEvents.pointerClickHandler);
			}
			pressedUI = null;
			pointerData.pointerPress = null;
		}
		lastTriggerClick = flag;
	}

	private void ClearHover()
	{
		if ((Object)(object)currentHover != (Object)null)
		{
			if (pointerData != null)
			{
				ExecuteEvents.Execute<IPointerExitHandler>(currentHover, (BaseEventData)(object)pointerData, ExecuteEvents.pointerExitHandler);
			}
			SetButtonHighlight(currentHover, highlighted: false);
			currentHover = null;
		}
	}

	private void SetButtonHighlight(GameObject btn, bool highlighted)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Image component = btn.GetComponent<Image>();
		if ((Object)(object)component != (Object)null)
		{
			((Graphic)component).color = (highlighted ? Main.buttonColors[1].GetCurrentColor() : GetOriginalColor(btn));
		}
	}

	private Color GetOriginalColor(GameObject btn)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		string name = ((Object)btn).name;
		if (name == "DoneButton")
		{
			return Main.buttonColors[0].GetCurrentColor();
		}
		if (name == "PreviousPage" || name == "NextPage")
		{
			return Main.buttonColors[1].GetCurrentColor();
		}
		return Main.buttonColors[0].GetCurrentColor();
	}

	private Action GetButtonAction(GameObject obj)
	{
		buttonActions.TryGetValue(obj, out var value);
		return value;
	}

	private void CreateCanvas()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected O, but got Unknown
		canvasObject = new GameObject("Seralyth_StumpUpdateCanvas");
		Transform transform = GorillaTagger.Instance.mainCamera.transform;
		canvasObject.transform.position = transform.position + transform.forward * 0.6f;
		canvasObject.transform.localScale = Vector3.one * 0.001f;
		canvas = canvasObject.AddComponent<Canvas>();
		canvas.renderMode = (RenderMode)2;
		CanvasScaler val = canvasObject.AddComponent<CanvasScaler>();
		val.dynamicPixelsPerUnit = 10f;
		graphicRaycaster = canvasObject.AddComponent<GraphicRaycaster>();
		RectTransform component = ((Component)canvas).GetComponent<RectTransform>();
		component.sizeDelta = new Vector2(900f, 550f);
		if ((Object)(object)EventSystem.current == (Object)null)
		{
			GameObject val2 = new GameObject("Seralyth_EventSystem");
			val2.AddComponent<EventSystem>();
			val2.AddComponent<StandaloneInputModule>();
		}
		eventSystem = EventSystem.current;
		CreatePointerLine();
		CreatePointerDot();
		CreateBackground();
		CreateTitle();
		CreateContentArea();
		CreatePageLabel();
		CreateDoneButton();
		CreatePageButtons();
		Billboard();
	}

	private void CreatePointerLine()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("Seralyth_StumpPointerLine");
		pointerLine = val.AddComponent<LineRenderer>();
		((Renderer)pointerLine).material = new Material(Shader.Find("GUI/Text Shader"));
		pointerLine.startWidth = 0.025f;
		pointerLine.endWidth = 0.025f;
		pointerLine.useWorldSpace = true;
		pointerLine.positionCount = 2;
		pointerLine.startColor = Main.buttonColors[0].GetCurrentColor();
		Color currentColor = Main.buttonColors[0].GetCurrentColor();
		currentColor.a = 0.5f;
		pointerLine.endColor = currentColor;
		pointerLine.numCapVertices = 10;
		pointerLine.numCornerVertices = 5;
	}

	private void CreatePointerDot()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		pointerDot = GameObject.CreatePrimitive((PrimitiveType)0);
		((Object)pointerDot).name = "Seralyth_StumpPointerDot";
		pointerDot.transform.localScale = Vector3.one * 0.05f;
		pointerDot.GetComponent<Renderer>().material.color = Main.buttonColors[0].GetCurrentColor();
		pointerDot.GetComponent<Collider>().enabled = false;
	}

	private void CreateBackground()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("Background");
		val.transform.SetParent(canvasObject.transform, false);
		Image val2 = val.AddComponent<Image>();
		Color currentColor = Main.menuBackgroundColor.GetCurrentColor();
		currentColor.a = 46f / 51f;
		((Graphic)val2).color = currentColor;
		RectTransform component = val.GetComponent<RectTransform>();
		component.anchorMin = Vector2.zero;
		component.anchorMax = Vector2.one;
		component.sizeDelta = Vector2.zero;
		component.offsetMin = Vector2.zero;
		component.offsetMax = Vector2.zero;
		GameObject val3 = new GameObject("Border");
		val3.transform.SetParent(canvasObject.transform, false);
		Image val4 = val3.AddComponent<Image>();
		((Graphic)val4).color = Main.buttonColors[0].GetCurrentColor();
		RectTransform component2 = val3.GetComponent<RectTransform>();
		component2.anchorMin = Vector2.zero;
		component2.anchorMax = Vector2.one;
		component2.sizeDelta = new Vector2(8f, 8f);
		component2.offsetMin = new Vector2(-4f, -4f);
		component2.offsetMax = new Vector2(4f, 4f);
		((Transform)component2).SetAsFirstSibling();
	}

	private void CreateTitle()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("Title");
		val.transform.SetParent(canvasObject.transform, false);
		titleText = val.AddComponent<TextMeshProUGUI>();
		((TMP_Text)titleText).text = "Seralyth Remake Updates";
		((Graphic)titleText).color = Main.textColors[0].GetCurrentColor();
		((TMP_Text)titleText).fontSize = 32f;
		((TMP_Text)titleText).alignment = (TextAlignmentOptions)258;
		((TMP_Text)titleText).fontStyle = (FontStyles)1;
		RectTransform component = val.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, 0.85f);
		component.anchorMax = new Vector2(1f, 1f);
		component.sizeDelta = new Vector2(0f, -6f);
		component.offsetMin = new Vector2(12f, 0f);
		component.offsetMax = new Vector2(-12f, -4f);
	}

	private void CreateContentArea()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("Content");
		val.transform.SetParent(canvasObject.transform, false);
		contentText = val.AddComponent<TextMeshProUGUI>();
		((TMP_Text)contentText).fontSize = 20f;
		((TMP_Text)contentText).alignment = (TextAlignmentOptions)257;
		((TMP_Text)contentText).lineSpacing = 24f;
		((Graphic)contentText).color = Main.textColors[1].GetCurrentColor();
		RectTransform component = val.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0f, 0.15f);
		component.anchorMax = new Vector2(1f, 0.8f);
		component.sizeDelta = new Vector2(-24f, -6f);
		component.offsetMin = new Vector2(12f, 0f);
		component.offsetMax = new Vector2(-12f, -4f);
		UpdateContent();
	}

	private void CreatePageLabel()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("PageLabel");
		val.transform.SetParent(canvasObject.transform, false);
		pageLabelText = val.AddComponent<TextMeshProUGUI>();
		((TMP_Text)pageLabelText).fontSize = 16f;
		((TMP_Text)pageLabelText).alignment = (TextAlignmentOptions)1028;
		((Graphic)pageLabelText).color = Main.textColors[1].GetCurrentColor();
		((TMP_Text)pageLabelText).alpha = 0.6f;
		RectTransform component = val.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(1f, 0f);
		component.anchorMax = new Vector2(1f, 0f);
		component.sizeDelta = new Vector2(120f, 20f);
		component.anchoredPosition = new Vector2(-32f, 50f);
		UpdatePageLabel();
	}

	private void UpdateContent()
	{
		if (!((Object)(object)contentText == (Object)null))
		{
			int num = Mathf.Max(1, Mathf.CeilToInt((float)allEntries.Count / (float)itemsPerPage));
			currentPage = Mathf.Clamp(currentPage, 0, num - 1);
			int num2 = currentPage * itemsPerPage;
			int num3 = Mathf.Min(num2 + itemsPerPage, allEntries.Count);
			string text = "";
			for (int i = num2; i < num3; i++)
			{
				ChangelogEntry changelogEntry = allEntries[i];
				string text2 = ((changelogEntry.type == "ADDED") ? "green" : ((changelogEntry.type == "REMOVED") ? "red" : ((changelogEntry.type == "UPDATED") ? "yellow" : "purple")));
				string typeDisplayName = Changelog.GetTypeDisplayName(changelogEntry.type);
				text = text + "<color=" + text2 + ">[" + typeDisplayName + "]</color> " + changelogEntry.description + "\n\n";
			}
			((TMP_Text)contentText).text = text.TrimEnd('\n');
			UpdatePageLabel();
		}
	}

	private void UpdatePageLabel()
	{
		if (!((Object)(object)pageLabelText == (Object)null))
		{
			int num = Mathf.Max(1, Mathf.CeilToInt((float)allEntries.Count / (float)itemsPerPage));
			((TMP_Text)pageLabelText).text = $"Page {currentPage + 1} / {num}";
		}
	}

	private void CreateButtonBase(string name, Vector2 anchoredPos, Vector2 size, Color color, string text, Action action)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject(name);
		val.transform.SetParent(canvasObject.transform, false);
		Image val2 = val.AddComponent<Image>();
		((Graphic)val2).color = color;
		RectTransform component = val.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(1f, 0f);
		component.anchorMax = new Vector2(1f, 0f);
		component.sizeDelta = size;
		component.anchoredPosition = anchoredPos;
		GameObject val3 = new GameObject("Text");
		val3.transform.SetParent(val.transform, false);
		TextMeshProUGUI val4 = val3.AddComponent<TextMeshProUGUI>();
		((TMP_Text)val4).text = text;
		((TMP_Text)val4).fontSize = 18f;
		((TMP_Text)val4).alignment = (TextAlignmentOptions)514;
		((Graphic)val4).color = Main.textColors[1].GetCurrentColor();
		RectTransform component2 = val3.GetComponent<RectTransform>();
		component2.anchorMin = Vector2.zero;
		component2.anchorMax = Vector2.one;
		component2.sizeDelta = Vector2.zero;
		buttonActions[val] = action;
	}

	private void CreateDoneButton()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject("DoneButton");
		val.transform.SetParent(canvasObject.transform, false);
		Image val2 = val.AddComponent<Image>();
		((Graphic)val2).color = Main.buttonColors[0].GetCurrentColor();
		RectTransform component = val.GetComponent<RectTransform>();
		component.anchorMin = new Vector2(0.5f, 0f);
		component.anchorMax = new Vector2(0.5f, 0f);
		component.sizeDelta = new Vector2(90f, 34f);
		component.anchoredPosition = new Vector2(0f, 10f);
		GameObject val3 = new GameObject("Text");
		val3.transform.SetParent(val.transform, false);
		TextMeshProUGUI val4 = val3.AddComponent<TextMeshProUGUI>();
		((TMP_Text)val4).text = "Done";
		((TMP_Text)val4).fontSize = 18f;
		((TMP_Text)val4).alignment = (TextAlignmentOptions)514;
		((Graphic)val4).color = Main.textColors[1].GetCurrentColor();
		RectTransform component2 = val3.GetComponent<RectTransform>();
		component2.anchorMin = Vector2.zero;
		component2.anchorMax = Vector2.one;
		component2.sizeDelta = Vector2.zero;
		buttonActions[val] = delegate
		{
			Hide();
		};
	}

	private void CreatePageButtons()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		CreateButtonBase("PreviousPage", new Vector2(-110f, 8f), new Vector2(80f, 34f), Main.buttonColors[1].GetCurrentColor(), "< Prev", PreviousPage);
		CreateButtonBase("NextPage", new Vector2(-32f, 8f), new Vector2(80f, 34f), Main.buttonColors[1].GetCurrentColor(), "Next >", NextPage);
	}

	public void PreviousPage()
	{
		int num = Mathf.Max(1, Mathf.CeilToInt((float)allEntries.Count / (float)itemsPerPage));
		currentPage--;
		if (currentPage < 0)
		{
			currentPage = num - 1;
		}
		UpdateContent();
	}

	public void NextPage()
	{
		int num = Mathf.Max(1, Mathf.CeilToInt((float)allEntries.Count / (float)itemsPerPage));
		currentPage++;
		if (currentPage >= num)
		{
			currentPage = 0;
		}
		UpdateContent();
	}

	public void Show()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		allEntries = new List<ChangelogEntry>(Changelog.Entries);
		allEntries.Reverse();
		if ((Object)(object)canvasObject == (Object)null)
		{
			currentPage = 0;
			CreateCanvas();
		}
		else
		{
			Transform transform = GorillaTagger.Instance.mainCamera.transform;
			canvasObject.transform.position = transform.position + transform.forward * 0.6f;
			canvasObject.SetActive(true);
			if ((Object)(object)pointerLine != (Object)null)
			{
				((Component)pointerLine).gameObject.SetActive(true);
			}
			if ((Object)(object)pointerDot != (Object)null)
			{
				pointerDot.SetActive(true);
			}
			UpdateContent();
		}
		if (AutoScrollEnabled && autoScrollCoroutine == null)
		{
			autoScrollCoroutine = ((MonoBehaviour)this).StartCoroutine(AutoScroll());
		}
	}

	public void Hide()
	{
		HasPoppedThisSession = true;
		LastSeenDllTimestamp = CurrentDllTimestamp;
		Settings.SavePreferences();
		ClearNewBadge();
		if (autoScrollCoroutine != null)
		{
			((MonoBehaviour)this).StopCoroutine(autoScrollCoroutine);
			autoScrollCoroutine = null;
		}
		if ((Object)(object)canvasObject != (Object)null)
		{
			canvasObject.SetActive(false);
		}
		if ((Object)(object)pointerLine != (Object)null)
		{
			((Component)pointerLine).gameObject.SetActive(false);
		}
		if ((Object)(object)pointerDot != (Object)null)
		{
			pointerDot.SetActive(false);
		}
		ClearHover();
	}

	public void Toggle()
	{
		if ((Object)(object)canvasObject != (Object)null && canvasObject.activeSelf)
		{
			Hide();
		}
		else
		{
			Show();
		}
	}

	private static void ClearNewBadge()
	{
		ButtonInfo index = Buttons.GetIndex("Stump Updates");
		if (index == null || index.overlapText == null)
		{
			return;
		}
		string text = " <color=grey>[</color><color=green>New</color><color=grey>]</color>";
		if (index.overlapText.Contains(text))
		{
			index.overlapText = index.overlapText.Replace(text, "");
			if (index.overlapText == index.buttonText)
			{
				index.overlapText = index.buttonText;
			}
		}
	}

	private IEnumerator AutoScroll()
	{
		while (true)
		{
			yield return (object)new WaitForSeconds(6f);
			if ((Object)(object)canvasObject != (Object)null && canvasObject.activeSelf)
			{
				NextPage();
			}
		}
	}
}
