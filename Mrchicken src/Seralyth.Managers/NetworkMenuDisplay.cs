using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Seralyth.Classes.Menu;
using Seralyth.Menu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Seralyth.Managers;

internal static class NetworkMenuDisplay
{
	private static Shader _cachedShader;

	private static readonly Color EnabledGreen = new Color(0.2f, 0.85f, 0.3f);

	private static readonly Color DisabledRed = new Color(0.85f, 0.25f, 0.25f);

	private static Shader CachedShader
	{
		get
		{
			if ((Object)(object)_cachedShader == (Object)null)
			{
				_cachedShader = Shader.Find("Universal Render Pipeline/Unlit");
			}
			return _cachedShader;
		}
	}

	private static void SetColor(Renderer r, Color c)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		r.material.SetColor("_BaseColor", c);
	}

	private static Material MakeMaterial(Color c)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Material val = new Material(CachedShader);
		val.SetColor("_BaseColor", c);
		return val;
	}

	public static void Create(NetworkMenuManager.RemoteMenuState state)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Expected O, but got Unknown
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_061d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		//IL_0676: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)state.displayObject != (Object)null)
		{
			return;
		}
		bool thinMenu = state.thinMenu;
		GameObject val = new GameObject("SeralythNetMenu_" + state.player.ActorNumber);
		val.transform.localScale = new Vector3(0.1f, 0.3f, 0.3825f);
		GameObject val2 = GameObject.CreatePrimitive((PrimitiveType)3);
		Object.Destroy((Object)(object)val2.GetComponent<BoxCollider>());
		val2.transform.parent = val.transform;
		val2.transform.localRotation = Quaternion.identity;
		val2.transform.localScale = (thinMenu ? new Vector3(0.1f, 1f, 1f) : new Vector3(0.1f, 1.5f, 1f));
		val2.transform.localPosition = new Vector3(0.5f, 0f, 0f);
		((Object)val2).name = "seralyth_bg";
		Renderer component = val2.GetComponent<Renderer>();
		component.material = MakeMaterial(state.menuBgColor);
		GameObject val3 = new GameObject("Canvas");
		val3.transform.parent = val.transform;
		Canvas val4 = val3.AddComponent<Canvas>();
		val4.renderMode = (RenderMode)2;
		CanvasScaler val5 = val3.AddComponent<CanvasScaler>();
		val3.AddComponent<GraphicRaycaster>();
		val5.dynamicPixelsPerUnit = 2500f;
		val5.referencePixelsPerUnit = 100f;
		string text = (Main.doCustomName ? Main.customMenuName : "MrChicken Menu");
		string text2 = (string.IsNullOrEmpty(state.player.NickName) ? "Unknown" : state.player.NickName);
		CreateTMPText(val3.transform, "MenuTitle", text, state.textColor0, (TextAlignmentOptions)514, new Vector2(0.28f, 0.05f), new Vector3(0.06f, 0f, 0.165f), Quaternion.Euler(180f, 90f, 90f));
		string text3 = "<b>" + text2 + "</b>";
		text3 += $" <color=grey>[</color><color=white>{state.page + 1}</color><color=grey>]</color>";
		CreateTMPText(val3.transform, "PlayerName", text3, state.textColor0, (TextAlignmentOptions)514, new Vector2(0.28f, 0.02f), new Vector3(0.06f, 0f, 0.135f), Quaternion.Euler(180f, 90f, 90f));
		string category = state.category;
		if (category != "Main")
		{
			CreateTMPText(val3.transform, "CategoryLabel", "[" + category + "]", state.textColor1, (TextAlignmentOptions)514, new Vector2(0.28f, 0.02f), new Vector3(0.06f, 0f, 0.105f), Quaternion.Euler(180f, 90f, 90f));
		}
		int num = 0;
		foreach (KeyValuePair<string, bool> buttonState in state.buttonStates)
		{
			if (buttonState.Value)
			{
				num++;
			}
		}
		CreateTMPText(val3.transform, "EnabledCount", num + " enabled", EnabledGreen, (TextAlignmentOptions)514, new Vector2(0.28f, 0.02f), new Vector3(0.06f, 0f, 0.075f), Quaternion.Euler(180f, 90f, 90f));
		float num2 = -0.3f;
		float num3 = 0.28f - num2;
		Color c = (state.swapButtonColors ? state.btnColor1 : state.btnColor0);
		GameObject val6 = GameObject.CreatePrimitive((PrimitiveType)3);
		Object.Destroy((Object)(object)val6.GetComponent<Rigidbody>());
		((Collider)val6.GetComponent<BoxCollider>()).isTrigger = true;
		val6.transform.parent = val.transform;
		val6.transform.localRotation = Quaternion.identity;
		val6.transform.localScale = (thinMenu ? new Vector3(0.09f, 0.9f, Main.ButtonDistance * 0.8f) : new Vector3(0.09f, 1.3f, Main.ButtonDistance * 0.8f));
		val6.transform.localPosition = new Vector3(0.56f, 0f, num3);
		((Object)val6).name = "Disconnect";
		val6.GetComponent<Renderer>().material = MakeMaterial(c);
		val6.AddComponent<NetworkMenuBtnCollider>().relatedText = "Disconnect";
		CreateTMPText(val3.transform, "DisconnectText", "Disconnect", state.textColor1, (TextAlignmentOptions)514, new Vector2(0.2f, 0.03f * (Main.ButtonDistance / 0.1f)), new Vector3(0.064f, 0f, 0.111f - num2 / 2.6f), Quaternion.Euler(180f, 90f, 90f));
		Color btnColor = (state.swapButtonColors ? state.btnColor1 : state.btnColor0);
		CreateNavButton(val, val3.transform, "PreviousPage", "<", state.textColor1, new Vector3(0.09f, 0.2f, 0.9f), new Vector3(0.56f, thinMenu ? 0.65f : 0.9f, 0f), new Vector3(0.064f, thinMenu ? 0.195f : 0.267f, 0f), btnColor);
		CreateNavButton(val, val3.transform, "NextPage", ">", state.textColor1, new Vector3(0.09f, 0.2f, 0.9f), new Vector3(0.56f, thinMenu ? (-0.65f) : (-0.9f), 0f), new Vector3(0.064f, thinMenu ? (-0.195f) : (-0.267f), 0f), btnColor);
		BuildButtonPage(state, val, val3, thinMenu);
		state.displayObject = val;
		UpdatePosition(state);
		if (Main.dynamicAnimations)
		{
			((MonoBehaviour)NetworkMenuManager.instance).StartCoroutine(OpenAnimation(state));
		}
		else
		{
			val.transform.localScale = new Vector3(0.1f, 0.3f, 0.3825f);
		}
	}

	private static void CreateTMPText(Transform parent, string name, string text, Color color, TextAlignmentOptions alignment, Vector2 sizeDelta, Vector3 localPos, Quaternion localRot)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = new GameObject(name);
		val.transform.parent = parent;
		TextMeshPro val2 = val.AddComponent<TextMeshPro>();
		((TMP_Text)val2).font = Main.activeFont;
		((TMP_Text)val2).text = text;
		((TMP_Text)val2).fontSize = 1f;
		((Graphic)val2).color = color;
		((TMP_Text)val2).fontStyle = Main.activeFontStyle;
		((TMP_Text)val2).alignment = alignment;
		((TMP_Text)val2).richText = true;
		((TMP_Text)val2).enableAutoSizing = true;
		((TMP_Text)val2).fontSizeMin = 0f;
		RectTransform component = val.GetComponent<RectTransform>();
		((Transform)component).localPosition = Vector3.zero;
		component.sizeDelta = sizeDelta;
		((Transform)component).localPosition = localPos;
		((Transform)component).localRotation = localRot;
	}

	private static void CreateNavButton(GameObject root, Transform canvasParent, string name, string label, Color textColor, Vector3 btnScale, Vector3 btnPos, Vector3 textPos, Color btnColor)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
		Object.Destroy((Object)(object)val.GetComponent<Rigidbody>());
		((Collider)val.GetComponent<BoxCollider>()).isTrigger = true;
		val.transform.parent = root.transform;
		val.transform.localRotation = Quaternion.identity;
		val.transform.localScale = btnScale;
		val.transform.localPosition = btnPos;
		((Object)val).name = name;
		Renderer component = val.GetComponent<Renderer>();
		component.material = MakeMaterial(btnColor);
		val.AddComponent<NetworkMenuBtnCollider>().relatedText = name;
		CreateTMPText(canvasParent, name + "Text", label, textColor, (TextAlignmentOptions)514, new Vector2(0.2f, 0.03f), textPos, Quaternion.Euler(180f, 90f, 90f));
	}

	private static void BuildButtonPage(NetworkMenuManager.RemoteMenuState state, GameObject root, GameObject canvasObj, bool thin)
	{
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		List<ButtonInfo> categoryButtons = GetCategoryButtons(state.category, state.buttonStates);
		int pageSize = Main.PageSize;
		int num = state.page * pageSize;
		int num2 = Mathf.Min(pageSize, categoryButtons.Count - num);
		float buttonDistance = Main.ButtonDistance;
		float num3 = (thin ? 0.399f : 0.599f);
		float num4 = (thin ? 0.12f : 0.18f);
		bool value = default(bool);
		for (int i = 0; i < num2; i++)
		{
			float num5 = (float)i * buttonDistance;
			ButtonInfo buttonInfo = categoryButtons[num + i];
			string buttonText = buttonInfo.buttonText;
			string text = ((!string.IsNullOrEmpty(buttonInfo.overlapText)) ? buttonInfo.overlapText : buttonText);
			bool label = buttonInfo.label;
			bool isTogglable = buttonInfo.isTogglable;
			bool incremental = buttonInfo.incremental;
			bool detected = buttonInfo.detected;
			bool flag = isTogglable && state.buttonStates.TryGetValue(buttonText, out value) && value;
			if (detected)
			{
				text = "<color=red>" + text + "</color>";
			}
			if (label)
			{
				CreateTMPText(canvasObj.transform, "btn_" + i, text, state.textColor0, (TextAlignmentOptions)514, new Vector2(0.2f, 0.03f * (buttonDistance / 0.1f)), new Vector3(0.064f, 0f, 0.111f - num5 / 2.6f), Quaternion.Euler(180f, 90f, 90f));
				continue;
			}
			Vector3 localScale = ((!incremental) ? (thin ? new Vector3(0.09f, 0.9f, buttonDistance * 0.8f) : new Vector3(0.09f, 1.3f, buttonDistance * 0.8f)) : (thin ? new Vector3(0.09f, 0.646f, buttonDistance * 0.8f) : new Vector3(0.09f, 1.046f, buttonDistance * 0.8f)));
			GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
			Object.Destroy((Object)(object)val.GetComponent<Rigidbody>());
			((Collider)val.GetComponent<BoxCollider>()).isTrigger = true;
			val.transform.parent = root.transform;
			val.transform.localRotation = Quaternion.identity;
			val.transform.localScale = localScale;
			val.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - num5);
			Renderer component = val.GetComponent<Renderer>();
			Color c = ((!isTogglable) ? state.btnColor0 : (flag ? state.btnColor1 : state.btnColor0));
			component.material = MakeMaterial(c);
			val.AddComponent<NetworkMenuBtnCollider>().relatedText = buttonText;
			if (incremental)
			{
				Color c2 = (state.swapButtonColors ? state.btnColor1 : state.btnColor0);
				GameObject val2 = GameObject.CreatePrimitive((PrimitiveType)3);
				Object.Destroy((Object)(object)val2.GetComponent<Rigidbody>());
				((Collider)val2.GetComponent<BoxCollider>()).isTrigger = true;
				val2.transform.parent = root.transform;
				val2.transform.localRotation = Quaternion.identity;
				val2.transform.localScale = new Vector3(0.09f, 0.102f, buttonDistance * 0.8f);
				val2.transform.localPosition = new Vector3(0.56f, num3, 0.28f - num5);
				val2.GetComponent<Renderer>().material = MakeMaterial(c2);
				val2.AddComponent<NetworkMenuBtnCollider>().relatedText = "Minus_" + buttonText;
				CreateTMPText(canvasObj.transform, "inc_minus_" + i, "-", state.textColor1, (TextAlignmentOptions)514, new Vector2(0.2f, 0.03f * (buttonDistance / 0.1f)), new Vector3(0.064f, num4, 0.111f - num5 / 2.6f), Quaternion.Euler(180f, 90f, 90f));
				GameObject val3 = GameObject.CreatePrimitive((PrimitiveType)3);
				Object.Destroy((Object)(object)val3.GetComponent<Rigidbody>());
				((Collider)val3.GetComponent<BoxCollider>()).isTrigger = true;
				val3.transform.parent = root.transform;
				val3.transform.localRotation = Quaternion.identity;
				val3.transform.localScale = new Vector3(0.09f, 0.102f, buttonDistance * 0.8f);
				val3.transform.localPosition = new Vector3(0.56f, 0f - num3, 0.28f - num5);
				val3.GetComponent<Renderer>().material = MakeMaterial(c2);
				val3.AddComponent<NetworkMenuBtnCollider>().relatedText = "Plus_" + buttonText;
				CreateTMPText(canvasObj.transform, "inc_plus_" + i, "+", state.textColor1, (TextAlignmentOptions)514, new Vector2(0.2f, 0.03f * (buttonDistance / 0.1f)), new Vector3(0.064f, 0f - num4, 0.111f - num5 / 2.6f), Quaternion.Euler(180f, 90f, 90f));
			}
			Color color = ((!isTogglable) ? state.textColor1 : (flag ? state.textColor2 : state.textColor1));
			float num6 = (incremental ? 0.18f : 0.2f);
			CreateTMPText(canvasObj.transform, "btn_" + i, text, color, (TextAlignmentOptions)514, new Vector2(num6, 0.03f * (buttonDistance / 0.1f)), new Vector3(0.064f, 0f, 0.111f - num5 / 2.6f), Quaternion.Euler(180f, 90f, 90f));
		}
	}

	private static List<ButtonInfo> GetCategoryButtons(string categoryName, Dictionary<string, bool> buttonStates = null)
	{
		if (categoryName == "Enabled Mods" && buttonStates != null)
		{
			List<ButtonInfo> list = new List<ButtonInfo>();
			ButtonInfo[][] buttons = Buttons.buttons;
			bool value = default(bool);
			foreach (ButtonInfo[] array in buttons)
			{
				if (array == null)
				{
					continue;
				}
				ButtonInfo[] array2 = array;
				foreach (ButtonInfo buttonInfo in array2)
				{
					if (buttonInfo.isTogglable && buttonStates.TryGetValue(buttonInfo.buttonText, out value) && value)
					{
						list.Add(buttonInfo);
					}
				}
			}
			list = list.OrderBy((ButtonInfo v) => v.buttonText).ToList();
			ButtonInfo buttonInfo2 = null;
			int category = Buttons.GetCategory("Enabled Mods");
			if (category >= 0 && category < Buttons.buttons.Length)
			{
				ButtonInfo[] array3 = Buttons.buttons[category];
				foreach (ButtonInfo buttonInfo3 in array3)
				{
					if (buttonInfo3.buttonText == "Exit Enabled Mods")
					{
						buttonInfo2 = buttonInfo3;
						break;
					}
				}
			}
			if (buttonInfo2 != null)
			{
				list.Insert(0, buttonInfo2);
			}
			return list;
		}
		int category2 = Buttons.GetCategory(categoryName);
		if (category2 < 0 || category2 >= Buttons.buttons.Length)
		{
			return new List<ButtonInfo>();
		}
		return new List<ButtonInfo>(Buttons.buttons[category2]);
	}

	private static IEnumerator OpenAnimation(NetworkMenuManager.RemoteMenuState state)
	{
		GameObject root = state.displayObject;
		if ((Object)(object)root == (Object)null)
		{
			yield break;
		}
		float elapsed = 0f;
		Vector3 startScale = root.transform.localScale;
		Vector3 targetScale = new Vector3(0.1f, 0.3f, 0.3825f);
		while (elapsed < 0.3f)
		{
			if ((Object)(object)root == (Object)null || state.closing)
			{
				yield break;
			}
			float t = elapsed / 0.3f;
			float s = 1.70158f;
			t -= 1f;
			float bounce = t * t * ((s + 1f) * t + s) + 1f;
			root.transform.localScale = Vector3.LerpUnclamped(startScale, targetScale, bounce);
			elapsed += Time.deltaTime;
			yield return null;
		}
		if ((Object)(object)root != (Object)null)
		{
			root.transform.localScale = targetScale;
		}
	}

	public static void CloseAndDestroy(NetworkMenuManager.RemoteMenuState state)
	{
		if (!((Object)(object)state.displayObject == (Object)null) && !state.closing)
		{
			state.closing = true;
			if (Main.dynamicAnimations)
			{
				((MonoBehaviour)NetworkMenuManager.instance).StartCoroutine(CloseAnimation(state));
				return;
			}
			Object.Destroy((Object)(object)state.displayObject);
			state.displayObject = null;
			NetworkMenuManager.RemoveRemoteMenu(state.player);
		}
	}

	private static IEnumerator CloseAnimation(NetworkMenuManager.RemoteMenuState state)
	{
		GameObject root = state.displayObject;
		if ((Object)(object)root == (Object)null)
		{
			yield break;
		}
		float elapsed = 0f;
		Vector3 startScale = root.transform.localScale;
		while (elapsed < 0.3f)
		{
			if ((Object)(object)root == (Object)null)
			{
				yield break;
			}
			float t = elapsed / 0.3f;
			float s = 1.70158f;
			t -= 1f;
			float bounce = t * t * ((s + 1f) * t - s);
			root.transform.localScale = Vector3.LerpUnclamped(startScale, Vector3.zero, bounce);
			elapsed += Time.deltaTime;
			yield return null;
		}
		if ((Object)(object)root != (Object)null)
		{
			Object.Destroy((Object)(object)root);
		}
		state.displayObject = null;
	}

	public static void UpdateState(NetworkMenuManager.RemoteMenuState state)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Expected O, but got Unknown
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Expected O, but got Unknown
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Expected O, but got Unknown
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)state.displayObject == (Object)null || state.closing)
		{
			return;
		}
		GameObject displayObject = state.displayObject;
		bool thinMenu = state.thinMenu;
		GameObject val = null;
		foreach (Transform item in displayObject.transform)
		{
			Transform val2 = item;
			if (((Object)val2).name == "Canvas")
			{
				val = ((Component)val2).gameObject;
				break;
			}
		}
		foreach (Transform item2 in displayObject.transform)
		{
			Transform val3 = item2;
			NetworkMenuBtnCollider component = ((Component)val3).GetComponent<NetworkMenuBtnCollider>();
			if ((Object)(object)component != (Object)null && component.relatedText != "PreviousPage" && component.relatedText != "NextPage" && component.relatedText != "Disconnect")
			{
				Object.Destroy((Object)(object)((Component)val3).gameObject);
			}
		}
		if ((Object)(object)val != (Object)null)
		{
			foreach (Transform item3 in val.transform)
			{
				Transform val4 = item3;
				if (((Object)val4).name != "MenuTitle" && ((Object)val4).name != "PlayerName" && ((Object)val4).name != "DisconnectText")
				{
					Object.Destroy((Object)(object)((Component)val4).gameObject);
				}
			}
		}
		else
		{
			val = new GameObject("Canvas");
			val.transform.parent = displayObject.transform;
			Canvas val5 = val.AddComponent<Canvas>();
			val5.renderMode = (RenderMode)2;
			CanvasScaler val6 = val.AddComponent<CanvasScaler>();
			val.AddComponent<GraphicRaycaster>();
			val6.dynamicPixelsPerUnit = 2500f;
			val6.referencePixelsPerUnit = 100f;
		}
		string text = (Main.doCustomName ? Main.customMenuName : "MrChicken Menu");
		foreach (Transform item4 in val.transform)
		{
			Transform val7 = item4;
			if (((Object)val7).name == "MenuTitle")
			{
				TextMeshPro component2 = ((Component)val7).GetComponent<TextMeshPro>();
				if ((Object)(object)component2 != (Object)null)
				{
					((TMP_Text)component2).text = text;
				}
			}
			if (((Object)val7).name == "PlayerName")
			{
				TextMeshPro component3 = ((Component)val7).GetComponent<TextMeshPro>();
				if ((Object)(object)component3 != (Object)null)
				{
					string text2 = "<b>" + state.player.NickName + "</b>";
					text2 += $" <color=grey>[</color><color=white>{state.page + 1}</color><color=grey>]</color>";
					((TMP_Text)component3).text = text2;
				}
			}
		}
		string category = state.category;
		if (category != "Main")
		{
			CreateTMPText(val.transform, "CategoryLabel", "[" + category + "]", state.textColor1, (TextAlignmentOptions)514, new Vector2(0.28f, 0.02f), new Vector3(0.06f, 0f, 0.105f), Quaternion.Euler(180f, 90f, 90f));
		}
		int num = 0;
		foreach (KeyValuePair<string, bool> buttonState in state.buttonStates)
		{
			if (buttonState.Value)
			{
				num++;
			}
		}
		CreateTMPText(val.transform, "EnabledCount", num + " enabled", EnabledGreen, (TextAlignmentOptions)514, new Vector2(0.28f, 0.02f), new Vector3(0.06f, 0f, 0.075f), Quaternion.Euler(180f, 90f, 90f));
		BuildButtonPage(state, displayObject, val, thinMenu);
		UpdateColors(state);
		UpdatePosition(state);
	}

	public static void UpdateColors(NetworkMenuManager.RemoteMenuState state)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Expected O, but got Unknown
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Expected O, but got Unknown
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_0608: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ef: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)state.displayObject == (Object)null || state.closing)
		{
			return;
		}
		foreach (Transform item in state.displayObject.transform)
		{
			Transform val = item;
			if (((Object)val).name == "seralyth_bg")
			{
				Renderer component = ((Component)val).GetComponent<Renderer>();
				if ((Object)(object)component != (Object)null)
				{
					SetColor(component, state.menuBgColor);
				}
				bool thinMenu = state.thinMenu;
				val.localScale = (thinMenu ? new Vector3(0.1f, 1f, 1f) : new Vector3(0.1f, 1.5f, 1f));
				continue;
			}
			NetworkMenuBtnCollider component2 = ((Component)val).GetComponent<NetworkMenuBtnCollider>();
			if ((Object)(object)component2 == (Object)null || string.IsNullOrEmpty(component2.relatedText))
			{
				continue;
			}
			Renderer component3 = ((Component)val).GetComponent<Renderer>();
			if ((Object)(object)component3 == (Object)null)
			{
				continue;
			}
			if (component2.relatedText == "PreviousPage" || component2.relatedText == "NextPage")
			{
				SetColor(component3, state.swapButtonColors ? state.btnColor1 : state.btnColor0);
				continue;
			}
			if (component2.relatedText == "Disconnect")
			{
				SetColor(component3, state.swapButtonColors ? state.btnColor1 : state.btnColor0);
				continue;
			}
			if (component2.relatedText.StartsWith("Minus_") || component2.relatedText.StartsWith("Plus_"))
			{
				SetColor(component3, state.swapButtonColors ? state.btnColor1 : state.btnColor0);
				continue;
			}
			List<ButtonInfo> categoryButtons = GetCategoryButtons(state.category, state.buttonStates);
			ButtonInfo buttonInfo = null;
			foreach (ButtonInfo item2 in categoryButtons)
			{
				if (item2.buttonText == component2.relatedText)
				{
					buttonInfo = item2;
					break;
				}
			}
			if (buttonInfo == null || !buttonInfo.label)
			{
				if (buttonInfo != null && !buttonInfo.isTogglable)
				{
					SetColor(component3, state.btnColor0);
					continue;
				}
				bool value;
				bool flag = state.buttonStates.TryGetValue(component2.relatedText, out value) && value;
				SetColor(component3, flag ? state.btnColor1 : state.btnColor0);
			}
		}
		bool value2 = default(bool);
		foreach (Transform item3 in state.displayObject.transform)
		{
			Transform val2 = item3;
			if (((Object)val2).name != "Canvas")
			{
				continue;
			}
			foreach (Transform item4 in val2)
			{
				Transform val3 = item4;
				TextMeshPro component4 = ((Component)val3).GetComponent<TextMeshPro>();
				if ((Object)(object)component4 == (Object)null)
				{
					continue;
				}
				if (((Object)val3).name == "MenuTitle")
				{
					((Graphic)component4).color = state.textColor0;
				}
				else if (((Object)val3).name == "PlayerName")
				{
					((Graphic)component4).color = state.textColor0;
				}
				else if (((Object)val3).name == "CategoryLabel")
				{
					((Graphic)component4).color = state.textColor1;
				}
				else if (((Object)val3).name == "EnabledCount")
				{
					int num = 0;
					foreach (KeyValuePair<string, bool> buttonState in state.buttonStates)
					{
						if (buttonState.Value)
						{
							num++;
						}
					}
					((TMP_Text)component4).text = num + " enabled";
					((Graphic)component4).color = EnabledGreen;
				}
				else if (((Object)val3).name == "DisconnectText")
				{
					((Graphic)component4).color = state.textColor1;
				}
				else if (((Object)val3).name.StartsWith("inc_minus_") || ((Object)val3).name.StartsWith("inc_plus_"))
				{
					((Graphic)component4).color = state.textColor1;
				}
				else
				{
					if (!((Object)val3).name.StartsWith("btn_"))
					{
						continue;
					}
					int result = 0;
					if (!int.TryParse(((Object)val3).name.Substring(4), out result))
					{
						continue;
					}
					List<ButtonInfo> categoryButtons2 = GetCategoryButtons(state.category, state.buttonStates);
					int pageSize = Main.PageSize;
					int num2 = state.page * pageSize;
					if (result + num2 < categoryButtons2.Count)
					{
						ButtonInfo buttonInfo2 = categoryButtons2[num2 + result];
						string buttonText = buttonInfo2.buttonText;
						string text = ((!string.IsNullOrEmpty(buttonInfo2.overlapText)) ? buttonInfo2.overlapText : buttonText);
						if (buttonInfo2.detected)
						{
							text = "<color=red>" + text + "</color>";
						}
						bool label = buttonInfo2.label;
						bool isTogglable = buttonInfo2.isTogglable;
						bool flag2 = isTogglable && !label && state.buttonStates.TryGetValue(buttonText, out value2) && value2;
						Color color = (label ? state.textColor0 : ((!isTogglable) ? state.textColor1 : (flag2 ? state.textColor2 : state.textColor1)));
						((Graphic)component4).color = color;
						((TMP_Text)component4).text = text;
					}
				}
			}
		}
		UpdatePosition(state);
	}

	public static void UpdatePosition(NetworkMenuManager.RemoteMenuState state)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)state.displayObject == (Object)null) && !state.closing)
		{
			state.displayObject.transform.position = state.position;
			state.displayObject.transform.rotation = state.rotation;
		}
	}
}
