using System;
using Code.World.Inventory;
using UI.Elements.Slot;
using UnityEngine;

namespace Code.World.Achievements;

public class MilMo_MedalEntry : ISlotItemEntry
{
	public MilMo_Medal Medal { get; set; }

	public int GetId()
	{
		return 0;
	}

	public int GetAmount()
	{
		return 0;
	}

	public void RegisterOnAmountUpdated(Action<int> setAmount)
	{
	}

	public void UnregisterOnAmountUpdated(Action<int> setAmount)
	{
	}

	public Enum GetSection()
	{
		string achievementCategoryIdentifier = Medal.Template.AchievementCategoryIdentifier;
		if (!Enum.TryParse<MilMo_MedalCategory.MedalCategory>(achievementCategoryIdentifier, out var result))
		{
			Debug.LogWarning("Could not find medal category: " + achievementCategoryIdentifier);
		}
		return result;
	}

	public Enum GetCategory()
	{
		return MilMo_InventoryEntry.InventoryCategory.Unknown;
	}

	public IEntryItem GetItem()
	{
		return Medal;
	}
}
