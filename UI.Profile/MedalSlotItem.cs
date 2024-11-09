using Code.World.Achievements;
using UI.Elements.Slot;
using UnityEngine;

namespace UI.Profile;

public class MedalSlotItem : SlotItem
{
	private MilMo_Medal _medal;

	public override void SetEntry(ISlotItemEntry entry)
	{
		base.SetEntry(entry);
		if (!(Entry?.GetItem() is MilMo_Medal medal))
		{
			Debug.LogWarning(base.name + ": Entry is not a MilMo_Medal");
			return;
		}
		_medal = medal;
		base.OnIconReady += RefreshIconColor;
		_medal.OnAcquired += RefreshIconColor;
		RefreshIconColor();
	}

	public override void ClearEntry()
	{
		if (_medal != null)
		{
			base.OnIconReady -= RefreshIconColor;
			_medal.OnAcquired -= RefreshIconColor;
		}
	}

	private void RefreshIconColor()
	{
		MilMo_Medal medal = _medal;
		Color iconColor = ((medal != null && medal.Acquired) ? Color.white : new Color(0f, 0f, 0f, 0.5f));
		SetIconColor(iconColor);
	}
}
