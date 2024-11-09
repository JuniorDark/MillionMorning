using System;
using UI.Tooltip.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace UI.Tooltip;

public abstract class Tooltip : MonoBehaviour
{
	[Header("Elements")]
	[SerializeField]
	private RectTransform container;

	[Header("Events")]
	[SerializeField]
	protected UnityEvent onShow;

	[SerializeField]
	protected UnityEvent onHide;

	private const int OFFSET_X = 10;

	private const int OFFSET_Y = 10;

	public abstract void SetData(TooltipData data);

	public virtual void Show()
	{
		base.gameObject.SetActive(value: true);
		onShow?.Invoke();
	}

	public void Hide()
	{
		onHide?.Invoke();
	}

	private void UpdatePosition()
	{
		Transform obj = base.transform;
		Vector2 vector = Pointer.current.position.ReadValue();
		Rect rect = container.rect;
		float num = (container ? (rect.width * 0.5f) : 0f);
		float num2 = (container ? (rect.height * 0.5f) : 0f);
		float x = Math.Clamp(vector.x + num + 10f, 10f + num, (float)Screen.width - num - 10f);
		float y = Math.Clamp(vector.y + num2 + 10f, 10f + num2, (float)Screen.height - num2 - 10f);
		obj.SetAsLastSibling();
		obj.position = new Vector3(x, y, 0f);
	}

	private void LateUpdate()
	{
		UpdatePosition();
	}
}
