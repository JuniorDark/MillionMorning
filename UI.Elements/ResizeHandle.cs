using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Elements;

public class ResizeHandle : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IEndDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField]
	private float minHeight;

	[SerializeField]
	private float maxHeight;

	[SerializeField]
	private RectTransform targetTransform;

	[SerializeField]
	private Texture2D grabIcon;

	[SerializeField]
	private Texture2D pullIcon;

	private Vector2 _currentPointerPosition;

	private Vector2 _previousPointerPosition;

	private bool _dragging;

	private bool _hovering;

	private void Awake()
	{
		if (targetTransform == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing targetTransform");
		}
	}

	public void OnBeginDrag(PointerEventData data)
	{
		Debug.LogWarning("OnPointerDown");
		if (!(targetTransform == null))
		{
			Debug.LogWarning("targetTransform ready");
			RectTransformUtility.ScreenPointToLocalPointInRectangle(targetTransform, data.position, data.pressEventCamera, out _previousPointerPosition);
			_dragging = true;
			RefreshIcon();
		}
	}

	public void OnDrag(PointerEventData data)
	{
		Debug.LogWarning("OnDrag");
		if (!(targetTransform == null))
		{
			Debug.LogWarning("targetTransform ready");
			Vector2 sizeDelta = targetTransform.sizeDelta;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(targetTransform, data.position, data.pressEventCamera, out _currentPointerPosition);
			Vector2 vector = _currentPointerPosition - _previousPointerPosition;
			sizeDelta = new Vector2(sizeDelta.x, Mathf.Clamp(sizeDelta.y - vector.y, minHeight, maxHeight));
			targetTransform.sizeDelta = sizeDelta;
			Debug.LogWarning(sizeDelta);
			_previousPointerPosition = _currentPointerPosition;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		_dragging = false;
		RefreshIcon();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		_hovering = true;
		RefreshIcon();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_hovering = false;
		RefreshIcon();
	}

	private void RefreshIcon()
	{
		if (Cursor.visible)
		{
			if (_dragging && (bool)pullIcon)
			{
				Cursor.SetCursor(pullIcon, new Vector2((float)pullIcon.width * 0.5f, (float)pullIcon.height * 0.5f), CursorMode.Auto);
			}
			else if (_hovering && (bool)grabIcon)
			{
				Cursor.SetCursor(grabIcon, new Vector2((float)grabIcon.width * 0.5f, (float)grabIcon.height * 0.5f), CursorMode.Auto);
			}
			else
			{
				Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
			}
		}
	}
}
