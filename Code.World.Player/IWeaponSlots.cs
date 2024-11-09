using Code.Core.Items;
using Code.World.Inventory;

namespace Code.World.Player;

public interface IWeaponSlots
{
	public enum Mode
	{
		All,
		AllExceptEmpty,
		FoodOnly,
		None
	}

	MilMo_Wieldable CurrentItem { get; }

	int CurrentItemInventoryId { get; }

	MilMo_InventoryEntry CurrentItemInventoryEntry { get; }

	Mode CurrentMode { get; set; }

	Mode LastMode { get; }

	void Destroy();

	bool HasItems();

	void AddToSlot(MilMo_InventoryEntry item, bool forceWield);

	void ToggleRight();

	void ToggleLeft();

	void RemoveFromSlot(int id);

	void Wield(string itemIdentifier);

	void WieldableUpgraded(int id);

	bool ShouldWieldNewUpgradable(MilMo_UpgradableInventoryEntry newItem);

	bool IsEnabled(MilMo_Wieldable item);

	bool IsToggleEnabled(MilMo_InventoryEntry item);

	void UnwieldCurrent(bool sendToServer = true);

	void ReWieldLast();

	void SetFavorite(MilMo_InventoryEntry entry);

	void UnsetFavorite(MilMo_InventoryEntry entry);
}
