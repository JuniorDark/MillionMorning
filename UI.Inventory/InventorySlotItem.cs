using System;
using Code.Core.Items;
using Code.World.Inventory;
using Core.GameEvent;
using UI.Elements.Slot;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory;

public class InventorySlotItem : SlotItem
{
	[SerializeField]
	private Toggle favoriteToggle;

	private void Awake()
	{
		if (favoriteToggle == null)
		{
			Debug.LogWarning("Missing favorite component");
			base.enabled = false;
		}
		if (base.enabled && (object)favoriteToggle != null)
		{
			favoriteToggle.gameObject.SetActive(value: false);
		}
	}

	private void OnDestroy()
	{
		Cleanup();
	}

	public override void SetEntry(ISlotItemEntry entry)
	{
		base.SetEntry(entry);
		if (ShouldHaveFavoriteToggle() && GetInventoryEntry(out var inventoryEntry))
		{
			EnableFavoriteToggle(enable: true);
			RefreshFavoriteToggleValue(inventoryEntry.IsFavorite);
			RegisterListeners();
		}
	}

	private void Cleanup()
	{
		if (ShouldHaveFavoriteToggle())
		{
			UnregisterListeners();
		}
	}

	private bool GetInventoryEntry(out MilMo_InventoryEntry inventoryEntry)
	{
		inventoryEntry = Entry as MilMo_InventoryEntry;
		return inventoryEntry != null;
	}

	private bool ShouldHaveFavoriteToggle()
	{
		if (Entry?.GetItem() is MilMo_Item milMo_Item)
		{
			return milMo_Item.IsWieldable();
		}
		return false;
	}

	private void RegisterListeners()
	{
		if (GetInventoryEntry(out var inventoryEntry))
		{
			MilMo_InventoryEntry milMo_InventoryEntry = inventoryEntry;
			milMo_InventoryEntry.OnFavoriteUpdated = (Action<bool>)Delegate.Combine(milMo_InventoryEntry.OnFavoriteUpdated, new Action<bool>(RefreshFavoriteToggleValue));
		}
	}

	private void UnregisterListeners()
	{
		if (GetInventoryEntry(out var inventoryEntry))
		{
			MilMo_InventoryEntry milMo_InventoryEntry = inventoryEntry;
			milMo_InventoryEntry.OnFavoriteUpdated = (Action<bool>)Delegate.Remove(milMo_InventoryEntry.OnFavoriteUpdated, new Action<bool>(RefreshFavoriteToggleValue));
		}
	}

	private void EnableFavoriteToggle(bool enable)
	{
		if (!(favoriteToggle == null))
		{
			favoriteToggle.gameObject.SetActive(enable);
		}
	}

	private void RefreshFavoriteToggleValue(bool favorite)
	{
		if ((bool)favoriteToggle && favoriteToggle.enabled)
		{
			favoriteToggle.SetIsOnWithoutNotify(favorite);
		}
	}

	public void OnFavoriteChange(bool favorite)
	{
		if (GetInventoryEntry(out var inventoryEntry))
		{
			RefreshFavoriteToggleValue(favorite);
			if (favorite)
			{
				GameEvent.InventoryItemSetFavoriteEvent?.RaiseEvent(inventoryEntry);
			}
			else
			{
				GameEvent.InventoryItemUnsetFavoriteEvent?.RaiseEvent(inventoryEntry);
			}
		}
	}
}
