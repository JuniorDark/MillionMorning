using System;
using Core.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI;

public class DragWindow : MonoBehaviour, IDragHandler, IEventSystemHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
	[Header("Elements")]
	[SerializeField]
	private RectTransform dragTargetTransform;

	[Header("Events")]
	[SerializeField]
	private UnityEvent onBeginDrag;

	[SerializeField]
	private UnityEvent onEndDrag;

	private Canvas _canvas;

	private const float OFFSET_Y = 5f;

	private const float OFFSET_X = 5f;

	private void Awake()
	{
		if (dragTargetTransform == null)
		{
			dragTargetTransform = base.transform.parent.GetComponent<RectTransform>();
		}
		_canvas = Core.Utilities.UI.GetCanvas(dragTargetTransform.gameObject);
	}

	public void OnDrag(PointerEventData eventData)
	{
		CalculateAndUpdateNewPosition(eventData);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		onBeginDrag?.Invoke();
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		onEndDrag?.Invoke();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		dragTargetTransform.SetAsLastSibling();
	}

	private void CalculateAndUpdateNewPosition(PointerEventData eventData)
	{
		Vector2 vector = eventData.delta / _canvas.scaleFactor;
		Rect rect = dragTargetTransform.rect;
		float num = rect.width * 0.5f;
		float num2 = rect.height * 0.5f;
		dragTargetTransform.anchoredPosition += vector;
		Vector3 position = dragTargetTransform.position;
		position.x = Math.Max(5f + num, Mathf.Min(_canvas.pixelRect.width - num - 5f, position.x));
		position.y = Math.Max(5f + num2, Mathf.Min(_canvas.pixelRect.height - num2 - 5f, position.y));
		dragTargetTransform.position = position;
	}
}
