namespace UI.Elements.Slot;

public interface ISlot
{
	void SetSlotItemEntry(ISlotItemEntry newItem);

	SlotItem GetSlotItem();
}
