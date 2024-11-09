using System;
using System.Collections.Generic;
using Code.World.Inventory;
using UI.Elements.Slot;

namespace Code.World.Player.Skills;

public class MilMo_SkillEntry : ISlotItemEntry
{
	public enum SkillSection
	{
		ClassAbilities
	}

	public static readonly Dictionary<Enum, string> SkillSectionLocales = new Dictionary<Enum, string> { 
	{
		SkillSection.ClassAbilities,
		"ProfileWindow_10243"
	} };

	public MilMo_Skill Skill { get; set; }

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
		return SkillSection.ClassAbilities;
	}

	public Enum GetCategory()
	{
		return MilMo_InventoryEntry.InventoryCategory.Unknown;
	}

	public IEntryItem GetItem()
	{
		return Skill;
	}
}
