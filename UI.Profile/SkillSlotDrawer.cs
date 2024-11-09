using System;
using System.Collections.Generic;
using System.Linq;
using Code.World.Player.Skills;
using Player;
using UI.Elements.Slot;
using UnityEngine;

namespace UI.Profile;

public class SkillSlotDrawer : SlotDrawer
{
	private SkillManager _skillManager;

	protected void Start()
	{
		_skillManager = SkillManager.GetPlayerSkillManager();
		if (_skillManager == null)
		{
			Debug.LogError(base.gameObject.name + ": Missing skill manager!");
			return;
		}
		if (_skillManager != null)
		{
			_skillManager.OnEntryAdded += AddSlot;
			_skillManager.OnEntryRemoved += RemoveSlot;
		}
		Redraw();
	}

	protected void OnDestroy()
	{
		if (_skillManager != null)
		{
			_skillManager.OnEntryAdded -= AddSlot;
			_skillManager.OnEntryRemoved -= RemoveSlot;
		}
	}

	protected override string GetSectionLocaleKey(Enum sectionIdentifier)
	{
		MilMo_SkillEntry.SkillSection skillSection = (MilMo_SkillEntry.SkillSection)(object)sectionIdentifier;
		if (!MilMo_SkillEntry.SkillSectionLocales.TryGetValue(skillSection, out var value))
		{
			return "";
		}
		return value;
	}

	private void Redraw()
	{
		DestroyContent();
		GetSkillEntries(out var entries);
		List<ISlotItemEntry> entries2 = entries.OfType<ISlotItemEntry>().ToList();
		AddSlots(entries2);
	}

	private void GetSkillEntries(out List<MilMo_SkillEntry> entries)
	{
		entries = ((_skillManager != null) ? _skillManager.GetAllEntries() : null);
	}
}
