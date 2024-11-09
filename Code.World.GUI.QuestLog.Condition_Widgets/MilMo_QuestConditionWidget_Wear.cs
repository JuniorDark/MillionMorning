using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.Items;
using Code.Core.ResourceSystem;
using Code.World.Quest.Conditions;
using UnityEngine;

namespace Code.World.GUI.QuestLog.Condition_Widgets;

public sealed class MilMo_QuestConditionWidget_Wear : MilMo_QuestConditionWidget
{
	public MilMo_QuestConditionWidget_Wear(MilMo_UserInterface ui, MilMo_Wear condition)
		: base(ui, condition)
	{
		MilMo_Item.AsyncGetItem(condition.Item, delegate(MilMo_Item item)
		{
			if (item == null)
			{
				Debug.LogWarning("MilMo_QuestCondition_Wear: Item is null");
			}
			else
			{
				MilMo_LocString locString = MilMo_Localization.GetLocString("QuestLog_9503");
				locString.SetFormatArgs(item.Template.DisplayName);
				MTextWidget.SetText(locString);
				MilMo_Widget itemIcon = new MilMo_Widget(UI);
				AddLeftSideIcon(itemIcon);
				item.AsyncGetIcon(delegate(Texture2D icon)
				{
					itemIcon.SetTexture(icon);
				});
			}
		});
	}
}
