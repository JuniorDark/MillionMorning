using Code.Core.Items;
using Code.Core.Network;
using Core;
using UI.Elements.Cooldown;
using UI.Elements.Slot;
using UI.HUD.Actionbar.Hotkeys;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI.HUD.Actionbar;

public class ActionSlot : Slot, IPointerClickHandler, IEventSystemHandler, ISettableSlot, IActionSlot
{
	[Header("ActionSlot")]
	[SerializeField]
	private AssignMarker assignMarker;

	[SerializeField]
	private HotkeyType hotkey;

	[SerializeField]
	private CooldownTimer cooldownTimer;

	public UnityEvent onClick;

	public UnityEvent onActivationFailed;

	public UnityEvent onSkillActivated;

	public UnityEvent onConsumableUsed;

	private Actionbar _actionbar;

	private void Awake()
	{
		if (assignMarker == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing AssignMarker!");
			base.enabled = false;
		}
		assignMarker.gameObject.SetActive(value: false);
		if (!hotkey)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing Hotkey!");
			base.enabled = false;
		}
		if (cooldownTimer == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing CooldownTimer!");
			base.enabled = false;
		}
		_actionbar = GetComponentInParent<Actionbar>();
		if (!_actionbar)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing Actionbar!");
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		onClick?.Invoke();
	}

	public HotkeyType GetHotkey()
	{
		return hotkey;
	}

	public override void SetSlotItemEntry(ISlotItemEntry entry)
	{
		base.SetSlotItemEntry(entry);
		if (!(entry?.GetItem() is IAssignable))
		{
			ClearSlotItem();
			return;
		}
		AddListeners(entry);
		UpdateCooldown(entry.GetItem() as IHaveCooldown);
		ServerUpdateAbilityHotkey(entry, entry.GetItem() as IAssignable);
	}

	public override void ClearSlotItem()
	{
		if (cooldownTimer != null)
		{
			cooldownTimer.Clear();
		}
		if (item != null)
		{
			ISlotItemEntry entry = item.GetEntry();
			if (entry != null)
			{
				RemoveListeners(entry);
				ServerUpdateAbilityHotkey(entry, null);
			}
		}
		base.ClearSlotItem();
	}

	public bool IsContainingEntry(IEntryItem entry)
	{
		return item.GetEntry()?.GetItem() == entry;
	}

	private void AddListeners(ISlotItemEntry entry)
	{
		if (entry.GetItem() is IUsable usable)
		{
			usable.RegisterOnUsed(SkillActivated);
			usable.RegisterOnFailedToUse(SkillFailedToActivate);
			if (usable is MilMo_Consumable)
			{
				usable.RegisterOnUsed(ConsumableUsed);
			}
		}
	}

	private void RemoveListeners(ISlotItemEntry entry)
	{
		if (entry.GetItem() is IUsable usable)
		{
			usable.UnregisterOnUsed(SkillActivated);
			usable.UnregisterOnFailedToUse(SkillFailedToActivate);
			if (usable is MilMo_Consumable)
			{
				usable.UnregisterOnUsed(ConsumableUsed);
			}
		}
	}

	private void ConsumableUsed()
	{
		onConsumableUsed?.Invoke();
	}

	private void AbilityDeactivated()
	{
	}

	private void SkillFailedToActivate()
	{
		onActivationFailed?.Invoke();
	}

	private void SkillActivated()
	{
		if (cooldownTimer != null)
		{
			cooldownTimer.Refresh();
		}
		onSkillActivated?.Invoke();
	}

	private void UpdateCooldown(IHaveCooldown itemWithCooldown)
	{
		if (!(cooldownTimer == null))
		{
			if (itemWithCooldown != null)
			{
				cooldownTimer.Setup(itemWithCooldown);
			}
			else
			{
				cooldownTimer.Clear();
			}
		}
	}

	public void RefreshServerUpdateAbilityHotkey()
	{
		_actionbar.ClearDuplicateSlots(GetSlotItemEntry(), hotkey);
		IAssignable assignableItem = GetSlotItemEntry()?.GetItem() as IAssignable;
		ServerUpdateAbilityHotkey(GetSlotItemEntry(), assignableItem);
	}

	private void ServerUpdateAbilityHotkey(ISlotItemEntry entry, IAssignable assignableItem)
	{
		if (entry != null)
		{
			string equipIdentifier = "";
			if (assignableItem != null)
			{
				equipIdentifier = assignableItem.GetSaveString(entry.GetId());
			}
			int actionSlotIndex = _actionbar.GetActionSlotIndex(this);
			if (actionSlotIndex == -1)
			{
				Debug.LogWarning("Undefined buttonIndex");
			}
			else if (Singleton<GameNetwork>.Instance.IsConnectedToGameServer)
			{
				Singleton<GameNetwork>.Instance.RequestEquipAbility((sbyte)actionSlotIndex, equipIdentifier);
			}
		}
	}

	public void OnBeginDrag()
	{
		if (!(assignMarker == null))
		{
			assignMarker.Show();
		}
	}

	public void OnEndDrag()
	{
		if (!(assignMarker == null))
		{
			assignMarker.Hide();
		}
	}
}
