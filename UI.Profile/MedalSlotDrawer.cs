using System;
using System.Collections.Generic;
using System.Linq;
using Code.World.Achievements;
using Code.World.Player;
using UI.Elements.Slot;
using UnityEngine;

namespace UI.Profile;

public class MedalSlotDrawer : SlotDrawer
{
	private ProfilePanel _profilePanel;

	private MilMo_Profile _profile;

	protected override void Awake()
	{
		base.Awake();
		_profilePanel = GetComponentInParent<ProfilePanel>();
		if (!_profilePanel)
		{
			Debug.LogWarning(base.gameObject.name + ": Could not find ProfilePanel");
		}
	}

	protected override void OnEnable()
	{
		MilMo_Profile profile = _profile;
		_profile = ((_profilePanel != null) ? _profilePanel.profile : null);
		if (profile == null || _profile == null || profile.playerId != _profile.playerId)
		{
			Redraw();
		}
		base.OnEnable();
	}

	protected override string GetSectionLocaleKey(Enum sectionIdentifier)
	{
		MilMo_MedalCategory.MedalCategory key = (MilMo_MedalCategory.MedalCategory)(object)sectionIdentifier;
		if (!MilMo_MedalCategory.MedalCategoryLocales.TryGetValue(key, out var value))
		{
			return "";
		}
		return value;
	}

	private void Redraw()
	{
		DestroyContent();
		GetMedalEntries(out var entries);
		List<ISlotItemEntry> entries2 = entries.OfType<ISlotItemEntry>().ToList();
		AddSlots(entries2);
	}

	private void GetMedalEntries(out List<MilMo_MedalEntry> entries)
	{
		entries = new List<MilMo_MedalEntry>();
		if (_profile?.Medals == null || _profile.Medals.Count < 1)
		{
			return;
		}
		foreach (MilMo_Medal medal in _profile.Medals)
		{
			MilMo_MedalEntry milMo_MedalEntry = new MilMo_MedalEntry();
			milMo_MedalEntry.Medal = medal;
			entries.Add(milMo_MedalEntry);
		}
	}
}
