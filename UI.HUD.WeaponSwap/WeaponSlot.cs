using System;
using System.Threading.Tasks;
using Code.World.Inventory;
using Code.World.Player;
using Core.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.WeaponSwap;

public class WeaponSlot : MonoBehaviour
{
	private enum WeaponSlotType
	{
		PreviousPrevious,
		Previous,
		Current,
		Next,
		NextNext
	}

	[SerializeField]
	private Image icon;

	private RectTransform _rectTransform;

	private MilMo_InventoryEntry _entry;

	private WeaponSlots _slots;

	[SerializeField]
	private WeaponSlotType type;

	private const float TRANSITION_SPEED = 0.5f;

	private int _moveAnimationId;

	private int _sizeAnimationId;

	private Vector3 _originalPosition;

	private Vector2 _originalSize;

	private const LeanTweenType TRANSITION_EASE = LeanTweenType.easeOutSine;

	private Vector2 GetPosition()
	{
		return (_originalPosition == Vector3.zero) ? base.transform.position : _originalPosition;
	}

	private Vector2 GetSize()
	{
		if (!(_originalSize == Vector2.zero))
		{
			return _originalSize;
		}
		return _rectTransform.sizeDelta;
	}

	private void Awake()
	{
		if (icon == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing icon");
		}
		else
		{
			_rectTransform = icon.GetComponent<RectTransform>();
		}
	}

	private void Start()
	{
		WeaponSlots slots = GetSlots();
		if (slots != null)
		{
			_slots = slots;
			WeaponSlots slots2 = _slots;
			slots2.OnToggleLeft = (Action)Delegate.Combine(slots2.OnToggleLeft, new Action(Refresh));
			WeaponSlots slots3 = _slots;
			slots3.OnToggleRight = (Action)Delegate.Combine(slots3.OnToggleRight, new Action(Refresh));
			WeaponSlots slots4 = _slots;
			slots4.OnWeaponSlotUpdate = (Action)Delegate.Combine(slots4.OnWeaponSlotUpdate, new Action(Refresh));
			Refresh();
		}
	}

	private WeaponSlots GetSlots()
	{
		return MilMo_Player.Instance?.EquipSlots;
	}

	public bool CanToggleSlots()
	{
		if (_slots == null)
		{
			_slots = GetSlots();
		}
		return _slots.HasSomethingToToggleTo();
	}

	public void AnimateFrom(WeaponSlot otherSlot)
	{
		if (otherSlot == null || icon == null)
		{
			return;
		}
		Vector2 originalPosition = GetPosition();
		Vector3 originalScale = Vector3.one;
		Vector2 position = otherSlot.GetPosition();
		Vector2 vector = otherSlot.GetSize() / GetSize();
		if (_moveAnimationId != 0)
		{
			LeanTween.cancel(_moveAnimationId);
			_moveAnimationId = 0;
		}
		if (_sizeAnimationId != 0)
		{
			LeanTween.cancel(_sizeAnimationId);
			_sizeAnimationId = 0;
		}
		_moveAnimationId = LeanTween.move(icon.gameObject, position, 0f).setOnComplete((Action)delegate
		{
			_moveAnimationId = LeanTween.move(icon.gameObject, originalPosition, 0.5f).setEase(LeanTweenType.easeOutSine).setOnComplete((Action)delegate
			{
				_moveAnimationId = 0;
			})
				.id;
		}).id;
		_sizeAnimationId = LeanTween.scale(icon.gameObject, vector, 0f).setOnComplete((Action)delegate
		{
			_sizeAnimationId = LeanTween.scale(icon.gameObject, originalScale, 0.5f).setEase(LeanTweenType.easeOutSine).setOnComplete((Action)delegate
			{
				_sizeAnimationId = 0;
			})
				.id;
		}).id;
	}

	private async void Refresh()
	{
		await GetEntry();
	}

	private async Task GetEntry()
	{
		_entry = type switch
		{
			WeaponSlotType.PreviousPrevious => _slots.GetPrevious(2), 
			WeaponSlotType.Previous => _slots.GetPrevious(), 
			WeaponSlotType.Current => _slots.GetCurrent(), 
			WeaponSlotType.Next => _slots.GetNext(), 
			WeaponSlotType.NextNext => _slots.GetNext(2), 
			_ => throw new ArgumentOutOfRangeException(), 
		};
		await InitializeValue();
	}

	private async Task InitializeValue()
	{
		if (!(icon == null))
		{
			icon.sprite = null;
			icon.enabled = false;
			icon.gameObject.SetActive(value: false);
			if (_entry.Id != 0)
			{
				Texture2D newTexture = await (_entry?.Item.AsyncGetIcon());
				Core.Utilities.UI.SetIcon(icon, newTexture);
				icon.gameObject.SetActive(value: true);
			}
		}
	}
}
