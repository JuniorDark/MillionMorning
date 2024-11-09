using System.Collections.Generic;
using Core.Utilities;
using UI.HUD.Actionbar;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Elements.Slot;

public class DraggableSlotItem : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
{
	[Header("Assets")]
	[SerializeField]
	private Image sourceImage;

	[Header("Events")]
	[SerializeField]
	private UnityEvent onBeginDrag;

	[SerializeField]
	private UnityEvent onEndDrag;

	private ISlot _source;

	private Canvas _canvas;

	private GameObject _dragClone;

	private void Awake()
	{
		_source = base.gameObject.GetComponentInParent<ISlot>();
		if (_source == null)
		{
			Debug.LogWarning("Missing slot item!");
			base.enabled = false;
		}
		else if (!sourceImage)
		{
			Debug.LogWarning("Missing source image!");
			base.enabled = false;
		}
	}

	private void Start()
	{
		_canvas = Core.Utilities.UI.GetCanvas(base.gameObject);
		if (!_canvas)
		{
			Debug.LogWarning("Missing canvas!");
			base.enabled = false;
		}
	}

	private void CreateDragClone()
	{
		_dragClone = new GameObject("DragClone");
		_dragClone.transform.parent = _canvas.transform;
		Image image = _dragClone.AddComponent<Image>();
		Rect rect = sourceImage.rectTransform.rect;
		image.sprite = sourceImage.sprite;
		image.rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
		image.color = new Color(1f, 1f, 1f, 0.7f);
		_dragClone.SetActive(value: true);
	}

	private void DestroyDragClone()
	{
		_dragClone.SetActive(value: false);
		Object.Destroy(_dragClone);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (!NotAllowedToDrag())
		{
			onBeginDrag?.Invoke();
			CreateDragClone();
		}
	}

	private bool NotAllowedToDrag()
	{
		SlotItem slotItem = _source.GetSlotItem();
		if ((bool)slotItem && slotItem.IsAssignable())
		{
			return !slotItem.HasEntry();
		}
		return true;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if ((bool)_dragClone)
		{
			_dragClone.transform.position = eventData.position;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if ((bool)_dragClone)
		{
			onEndDrag?.Invoke();
			DestroyDragClone();
			ISlot container = GetContainer(eventData);
			DropItemIntoContainer(container);
		}
	}

	private ISlot GetContainer(PointerEventData eventData)
	{
		List<RaycastResult> list = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventData, list);
		foreach (RaycastResult item in list)
		{
			ISlot componentInParent = item.gameObject.GetComponentInParent<ISlot>();
			if (componentInParent != null)
			{
				return componentInParent;
			}
		}
		return null;
	}

	private void DropItemIntoContainer(ISlot destination)
	{
		ISettableSlot obj = destination as ISettableSlot;
		ISlotItemEntry slotItemEntry = _source.GetSlotItem()?.GetEntry();
		ISlotItemEntry slotItemEntry2 = obj?.GetSlotItem()?.GetEntry();
		if (obj != null)
		{
			destination.SetSlotItemEntry(slotItemEntry);
		}
		if (_source is ISettableSlot)
		{
			_source.SetSlotItemEntry(slotItemEntry2);
		}
		if (destination is IActionSlot actionSlot)
		{
			actionSlot.RefreshServerUpdateAbilityHotkey();
		}
	}
}
