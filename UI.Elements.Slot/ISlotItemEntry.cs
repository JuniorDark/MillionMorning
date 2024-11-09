using System;

namespace UI.Elements.Slot;

public interface ISlotItemEntry
{
	int GetId();

	int GetAmount();

	void RegisterOnAmountUpdated(Action<int> setAmount);

	void UnregisterOnAmountUpdated(Action<int> setAmount);

	Enum GetSection();

	Enum GetCategory();

	IEntryItem GetItem();
}
