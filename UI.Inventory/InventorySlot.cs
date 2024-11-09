using UI.Elements.Slot;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI.Inventory;

public class InventorySlot : Slot, IPointerClickHandler, IEventSystemHandler, IGettableSlot
{
	[Header("Events")]
	public UnityEvent onClick;

	private const float DOUBLE_CLICK_DELAY = 0.3f;

	private double _lastClick;

	public void OnPointerClick(PointerEventData eventData)
	{
		if (IsDoubleClick(eventData))
		{
			onClick?.Invoke();
		}
	}

	private bool IsDoubleClick(PointerEventData eventData)
	{
		bool result = (double)eventData.clickTime - _lastClick < 0.30000001192092896;
		_lastClick = eventData.clickTime;
		return result;
	}
}
