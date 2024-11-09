using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI.Elements.Slot;

public class Slot : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, ISlot
{
	[Header("Assets")]
	[SerializeField]
	protected SlotItem item;

	[Header("Events")]
	[SerializeField]
	private UnityEvent onEnter;

	[SerializeField]
	private UnityEvent onExit;

	public ISlotItemEntry GetSlotItemEntry()
	{
		if (!item)
		{
			return null;
		}
		return item.GetEntry();
	}

	public SlotItem GetSlotItem()
	{
		return item;
	}

	public virtual void SetSlotItemEntry(ISlotItemEntry newItem)
	{
		ClearSlotItem();
		if (newItem != null)
		{
			if (item == null)
			{
				Debug.LogWarning("Item is null, unable to assign it an entry");
			}
			else
			{
				item.SetEntry(newItem);
			}
		}
	}

	public virtual void ClearSlotItem()
	{
		if ((bool)item && (bool)item.gameObject)
		{
			item.ClearEntry();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		onEnter?.Invoke();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		onExit?.Invoke();
		eventData.selectedObject = null;
	}
}
