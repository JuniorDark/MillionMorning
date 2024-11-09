using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Items;
using Code.Core.Network;
using Code.Core.Sound;
using Code.World.Inventory;
using Core;
using Core.GameEvent;
using UnityEngine;

namespace Code.World.Player;

public class WeaponSlots : IWeaponSlots
{
	private readonly LinkedList<MilMo_InventoryEntry> _weaponSlots;

	private LinkedListNode<MilMo_InventoryEntry> _currentSlot;

	private LinkedListNode<MilMo_InventoryEntry> _lastSlot;

	private readonly LinkedListNode<MilMo_InventoryEntry> _unarmedSlot;

	private readonly MilMo_InventoryEntry _unarmedEntry;

	private IWeaponSlots.Mode _mode;

	private readonly MilMo_Player _player;

	public Action OnToggleLeft;

	public Action OnToggleRight;

	public Action OnWeaponSlotUpdate;

	public MilMo_Wieldable CurrentItem => _currentSlot.Value.Item as MilMo_Wieldable;

	public int CurrentItemInventoryId => _currentSlot.Value.Id;

	public MilMo_InventoryEntry CurrentItemInventoryEntry => _currentSlot.Value;

	public IWeaponSlots.Mode CurrentMode
	{
		get
		{
			return _mode;
		}
		set
		{
			if (_mode == value)
			{
				return;
			}
			if (value == IWeaponSlots.Mode.None)
			{
				UnwieldCurrent();
				LastMode = _mode;
				_mode = value;
				return;
			}
			if (_mode == IWeaponSlots.Mode.None)
			{
				_mode = value;
				ReWieldLast();
				return;
			}
			LastMode = _mode;
			_mode = value;
			if (_mode == IWeaponSlots.Mode.AllExceptEmpty && CurrentItem == null)
			{
				ToggleRight();
			}
			if (CurrentItem != null && !IsEnabled(CurrentItem))
			{
				UnwieldCurrent();
			}
			else if (CurrentItem == null)
			{
				ReWieldLast();
			}
		}
	}

	public IWeaponSlots.Mode LastMode { get; private set; }

	private LinkedListNode<MilMo_InventoryEntry> StepLeft(int steps)
	{
		if (steps < 1 || steps > 10)
		{
			return _currentSlot;
		}
		LinkedListNode<MilMo_InventoryEntry> linkedListNode = GetFilteredNode();
		if (linkedListNode == null)
		{
			return _currentSlot;
		}
		for (int i = 1; i <= steps; i++)
		{
			linkedListNode = linkedListNode.Previous ?? linkedListNode.List.Last;
		}
		return linkedListNode;
	}

	private LinkedListNode<MilMo_InventoryEntry> StepRight(int steps)
	{
		if (steps < 1 || steps > 10)
		{
			return _currentSlot;
		}
		LinkedListNode<MilMo_InventoryEntry> linkedListNode = GetFilteredNode();
		if (linkedListNode == null)
		{
			return _currentSlot;
		}
		for (int i = 1; i <= steps; i++)
		{
			linkedListNode = linkedListNode.Next ?? linkedListNode.List.First;
		}
		return linkedListNode;
	}

	private LinkedListNode<MilMo_InventoryEntry> GetFilteredNode()
	{
		List<MilMo_InventoryEntry> list = _weaponSlots.Where(IsToggleEnabled).ToList();
		if (list.Count < 1)
		{
			return null;
		}
		MilMo_InventoryEntry value = list.FirstOrDefault((MilMo_InventoryEntry e) => e == _currentSlot.Value) ?? list.First();
		LinkedList<MilMo_InventoryEntry> linkedList = new LinkedList<MilMo_InventoryEntry>();
		list.ForEach(delegate(MilMo_InventoryEntry entry)
		{
			linkedList.AddLast(entry);
		});
		return linkedList.Find(value);
	}

	public MilMo_InventoryEntry GetCurrent()
	{
		return _currentSlot?.Value;
	}

	public MilMo_InventoryEntry GetPrevious(int steps = 1)
	{
		return StepLeft(steps).Value;
	}

	public MilMo_InventoryEntry GetNext(int steps = 1)
	{
		return StepRight(steps).Value;
	}

	public WeaponSlots(MilMo_Player player)
	{
		_player = player;
		_weaponSlots = new LinkedList<MilMo_InventoryEntry>();
		_unarmedEntry = new MilMo_InventoryEntry();
		_unarmedSlot = _weaponSlots.AddFirst(_unarmedEntry);
		_currentSlot = _weaponSlots.First;
		LastMode = IWeaponSlots.Mode.All;
		GameEvent.InventoryItemSetFavoriteEvent.RegisterAction(SetFavorite);
		GameEvent.InventoryItemUnsetFavoriteEvent.RegisterAction(UnsetFavorite);
	}

	public void Destroy()
	{
		Clear();
		GameEvent.InventoryItemSetFavoriteEvent.UnregisterAction(SetFavorite);
		GameEvent.InventoryItemUnsetFavoriteEvent.UnregisterAction(UnsetFavorite);
	}

	public void Clear()
	{
		for (int num = _weaponSlots.Count - 1; num >= 1; num--)
		{
			_weaponSlots.RemoveLast();
		}
		_currentSlot = _unarmedSlot;
		_lastSlot = _unarmedSlot;
	}

	public bool HasItems()
	{
		return _weaponSlots.Count > 1;
	}

	public bool HasSomethingToToggleTo()
	{
		return _weaponSlots.Where(IsToggleEnabled).ToList().Count > 1;
	}

	public void AddToSlot(MilMo_InventoryEntry item, bool forceWield)
	{
		if (item == null || (!item.IsFavorite && !item.IsEquipped && !(item is MilMo_UpgradableInventoryEntry)))
		{
			Debug.LogWarning($"WeaponSlots: Bailing. Item is not favorite: {item?.Item?.Template?.DisplayName}");
			return;
		}
		if (_weaponSlots.Find(item)?.Value != null)
		{
			string text = item.Item?.Template?.Identifier;
			Debug.LogWarning("WeaponSlots: Trying to add item that already exist: " + text);
			return;
		}
		if (_weaponSlots == _currentSlot.List)
		{
			_weaponSlots.AddAfter(_currentSlot, item);
		}
		else
		{
			_weaponSlots.AddLast(item);
		}
		if (forceWield)
		{
			LinkedListNode<MilMo_InventoryEntry> linkedListNode = _weaponSlots.Find(item);
			if (linkedListNode != null)
			{
				SetCurrentWield(item, linkedListNode);
				OnWeaponSlotUpdate?.Invoke();
			}
		}
	}

	public void RemoveFromSlot(int id)
	{
		MilMo_InventoryEntry entryById = GetEntryById(id);
		if (entryById == null)
		{
			return;
		}
		LinkedListNode<MilMo_InventoryEntry> linkedListNode = _weaponSlots.Find(entryById);
		if (linkedListNode != null)
		{
			if (linkedListNode.Equals(_currentSlot))
			{
				_player.Unwield(entryById.IsEquipped);
				_currentSlot = _unarmedSlot;
			}
			if (linkedListNode.Equals(_lastSlot))
			{
				_lastSlot = _unarmedSlot;
			}
			_weaponSlots.Remove(linkedListNode);
			OnWeaponSlotUpdate?.Invoke();
		}
	}

	private MilMo_InventoryEntry GetEntryById(int id)
	{
		return _weaponSlots.Where((MilMo_InventoryEntry slot) => slot.Id == id).FirstOrDefault();
	}

	public bool IsEnabled(MilMo_Wieldable item)
	{
		if (item == null)
		{
			return false;
		}
		if (_mode == IWeaponSlots.Mode.None)
		{
			return false;
		}
		IWeaponSlots.Mode mode = _mode;
		if (mode == IWeaponSlots.Mode.All || mode == IWeaponSlots.Mode.AllExceptEmpty)
		{
			return true;
		}
		if (_mode == IWeaponSlots.Mode.FoodOnly)
		{
			return item.IsFood();
		}
		return false;
	}

	public void ToggleRight()
	{
		LinkedListNode<MilMo_InventoryEntry> newSlot = StepLeft(1);
		Toggle(newSlot);
		OnToggleRight?.Invoke();
	}

	public void ToggleLeft()
	{
		LinkedListNode<MilMo_InventoryEntry> newSlot = StepRight(1);
		Toggle(newSlot);
		OnToggleLeft?.Invoke();
	}

	private void Toggle(LinkedListNode<MilMo_InventoryEntry> newSlot)
	{
		if (newSlot != null)
		{
			MilMo_InventoryEntry value = newSlot.Value;
			if (value != null && value.Item is MilMo_Wieldable)
			{
				SetCurrentWield(value, newSlot);
				return;
			}
			_currentSlot = _unarmedSlot;
			_player.Unwield(sendToServer: true, useCooldown: true);
		}
	}

	public bool IsToggleEnabled(MilMo_InventoryEntry entry)
	{
		if (_mode != IWeaponSlots.Mode.AllExceptEmpty && entry.Equals(_unarmedEntry))
		{
			return true;
		}
		if (!(entry.Item is MilMo_Wieldable milMo_Wieldable))
		{
			return false;
		}
		if (_mode == IWeaponSlots.Mode.None)
		{
			return false;
		}
		if (_mode == IWeaponSlots.Mode.FoodOnly)
		{
			return milMo_Wieldable.IsFood();
		}
		IWeaponSlots.Mode mode = _mode;
		if (mode == IWeaponSlots.Mode.All || mode == IWeaponSlots.Mode.AllExceptEmpty)
		{
			return !milMo_Wieldable.IsFood();
		}
		return false;
	}

	public void Wield(string itemIdentifier)
	{
		MilMo_InventoryEntry milMo_InventoryEntry = _weaponSlots.Where((MilMo_InventoryEntry slot) => slot.Item?.Identifier == itemIdentifier).FirstOrDefault();
		if (milMo_InventoryEntry != null)
		{
			Wield(milMo_InventoryEntry);
		}
	}

	public void UnwieldCurrent(bool sendToServer = true)
	{
		if (_currentSlot != _unarmedSlot)
		{
			_lastSlot = _currentSlot;
			Toggle(_unarmedSlot);
			_player.Unwield(sendToServer);
		}
		else
		{
			_lastSlot = _unarmedSlot;
		}
	}

	public void ReWieldLast()
	{
		if (_mode != IWeaponSlots.Mode.None)
		{
			MilMo_Wieldable item = _lastSlot?.Value.Item as MilMo_Wieldable;
			if (_lastSlot != _unarmedSlot && IsEnabled(item))
			{
				Toggle(_lastSlot);
			}
		}
	}

	private void Wield(MilMo_InventoryEntry item, bool saveToLast = true)
	{
		MilMo_Wieldable item2 = item.Item as MilMo_Wieldable;
		if (!IsEnabled(item2))
		{
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Wrong);
			return;
		}
		LinkedListNode<MilMo_InventoryEntry> currentSlot = _weaponSlots.Find(item);
		if (saveToLast)
		{
			_lastSlot = _currentSlot;
		}
		if (_currentSlot != null)
		{
			_player.Unwield(sendToServer: true);
		}
		_currentSlot = currentSlot;
		_player.WieldWhenReady(item2);
		if (_currentSlot != null)
		{
			Singleton<GameNetwork>.Instance.RequestWield(_currentSlot.Value.Id);
		}
	}

	private void SetCurrentWield(MilMo_InventoryEntry item, LinkedListNode<MilMo_InventoryEntry> newSlot)
	{
		_lastSlot = _currentSlot;
		_currentSlot = newSlot ?? _unarmedSlot;
		MilMo_Wieldable item2 = item.Item as MilMo_Wieldable;
		Singleton<GameNetwork>.Instance.RequestWield(item.Id);
		_player.WieldWhenReady(item2);
	}

	public void WieldableUpgraded(int id)
	{
		MilMo_InventoryEntry entryById = GetEntryById(id);
		if (entryById != null)
		{
			MilMo_UpgradableInventoryEntry newItem = entryById as MilMo_UpgradableInventoryEntry;
			Wield(entryById, !ShouldWieldNewUpgradable(newItem));
		}
	}

	public bool ShouldWieldNewUpgradable(MilMo_UpgradableInventoryEntry newItem)
	{
		if (newItem == null)
		{
			return false;
		}
		if (!(CurrentItemInventoryEntry is MilMo_UpgradableInventoryEntry milMo_UpgradableInventoryEntry))
		{
			return true;
		}
		if (newItem.Item is MilMo_RangedWeapon || milMo_UpgradableInventoryEntry.Item is MilMo_RangedWeapon)
		{
			return false;
		}
		return newItem.CurrentUpgradeLevel >= milMo_UpgradableInventoryEntry.CurrentUpgradeLevel;
	}

	public void SetFavorite(MilMo_InventoryEntry entry)
	{
		bool isFavorite = entry.IsFavorite;
		entry.SetFavorite(favorite: true);
		AddToSlot(entry, !isFavorite);
	}

	public void UnsetFavorite(MilMo_InventoryEntry entry)
	{
		entry.SetFavorite(favorite: false);
		RemoveFromSlot(entry.Id);
	}
}
