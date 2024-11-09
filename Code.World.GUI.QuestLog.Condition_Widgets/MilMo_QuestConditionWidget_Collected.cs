using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.Items;
using Code.Core.ResourceSystem;
using Code.World.Inventory;
using Code.World.Player;
using Code.World.Quest.Conditions;
using UnityEngine;

namespace Code.World.GUI.QuestLog.Condition_Widgets;

public sealed class MilMo_QuestConditionWidget_Collected : MilMo_QuestConditionWidget
{
	public MilMo_QuestConditionWidget_Collected(MilMo_UserInterface ui, MilMo_Collected condition)
		: base(ui, condition)
	{
		MilMo_QuestConditionWidget_Collected milMo_QuestConditionWidget_Collected = this;
		MilMo_Item.AsyncGetItem(condition.Item, delegate(MilMo_Item item)
		{
			if (item == null)
			{
				Debug.LogWarning("MilMo_QuestConditionWidget_Collected: Item was null");
			}
			else
			{
				MilMo_LocString copy = MilMo_Localization.GetLocString("QuestLog_9351").GetCopy();
				copy.SetFormatArgs(condition.Amount, item.Template.DisplayName);
				milMo_QuestConditionWidget_Collected.MTextWidget.SetText(copy);
				MilMo_Widget itemIcon = new MilMo_Widget(milMo_QuestConditionWidget_Collected.UI);
				item.AsyncGetIcon(delegate(Texture2D tex)
				{
					if (tex != null)
					{
						itemIcon.SetTexture(tex);
					}
				});
				MilMo_InventoryEntry entry = MilMo_Player.Instance.Inventory.GetEntry(condition.Item);
				int current = 0;
				if (entry != null)
				{
					current = entry.Amount;
				}
				milMo_QuestConditionWidget_Collected.AddLeftSideCounter(current, condition.Amount);
				milMo_QuestConditionWidget_Collected.AddLeftSideIcon(itemIcon);
			}
		});
	}
}
