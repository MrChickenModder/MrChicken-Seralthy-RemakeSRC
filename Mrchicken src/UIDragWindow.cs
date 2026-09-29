using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragWindow : MonoBehaviour, IDragHandler, IEventSystemHandler, IPointerDownHandler
{
	private RectTransform rectTransform;

	private Canvas canvas;

	private void Awake()
	{
		ref RectTransform reference = ref rectTransform;
		Transform transform = ((Component)this).transform;
		reference = (RectTransform)(object)((transform is RectTransform) ? transform : null);
		canvas = ((Component)this).GetComponentInParent<Canvas>();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		((Transform)rectTransform).SetAsLastSibling();
	}

	public void OnDrag(PointerEventData eventData)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		RectTransform obj = rectTransform;
		obj.anchoredPosition += eventData.delta / canvas.scaleFactor;
	}
}
