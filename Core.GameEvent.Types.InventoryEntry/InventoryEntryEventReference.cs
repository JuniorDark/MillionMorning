using Code.World.Inventory;
using Core.GameEvent.Types.Base;

namespace Core.GameEvent.Types.InventoryEntry;

public class InventoryEntryEventReference : EventReference<MilMo_InventoryEntry, BaseGameEvent<MilMo_InventoryEntry>>
{
	public InventoryEntryEventReference(string addressableKey)
		: base(addressableKey)
	{
	}
}
