using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.Items;
using Code.Core.ResourceSystem;
using Code.World.Inventory;
using Code.World.Player;
using Code.World.Quest.Conditions;
using UnityEngine;

namespace Code.World.GUI.QuestLog.Condition_Widgets;

public sealed class MilMo_QuestConditionWidget_CollectedAny : MilMo_QuestConditionWidget
{
	private readonly List<MilMo_Widget> _mTextWidgetRotationList;

	private readonly List<MilMo_Widget> _mIconWidgetRotationList;

	private int _mCurrentIndex;

	private readonly List<string> _mChangingCounters;

	public MilMo_QuestConditionWidget_CollectedAny(MilMo_UserInterface ui, MilMo_CollectedAny condition)
		: base(ui, condition)
	{
		_mIconWidgetRotationList = new List<MilMo_Widget>();
		_mTextWidgetRotationList = new List<MilMo_Widget>();
		_mChangingCounters = new List<string>();
		MilMo_LocString copy = MilMo_Localization.GetLocString("QuestLog_9352").GetCopy();
		copy.SetFormatArgs("");
		MTextWidget.SetText(copy);
		foreach (MilMo_CollectedAny.MilMo_CollectedInfo item in condition.Items)
		{
			CreateItemWidget(item);
			MilMo_InventoryEntry entry = MilMo_Player.Instance.Inventory.GetEntry(item.Item);
			int num = 0;
			if (entry != null)
			{
				num += entry.Amount;
			}
			_mChangingCounters.Add(num + "/" + item.AmountToCollect);
		}
		AddChild(_mIconWidgetRotationList[0]);
		AddChild(_mTextWidgetRotationList[0]);
		AddLeftSideCounter(int.TryParse(_mChangingCounters[0].Split('/')[0], out var result) ? result : 0, condition.Items[0].AmountToCollect);
		MLeftSideWidgets[0].FadeToDefaultTextColor = false;
		MTickEvent = MilMo_EventSystem.At(0.5f, Tick);
	}

	private void CreateItemWidget(MilMo_CollectedAny.MilMo_CollectedInfo item)
	{
		MilMo_Widget text = new MilMo_Widget(UI);
		text.SetTextAlignment(MTextWidget.TextAlign);
		text.SetAlignment(MilMo_GUI.Align.TopLeft);
		text.SetScale(MTextWidget.Scale);
		text.SetFont(MilMo_GUI.Font.EborgSmall);
		text.SetFontScale(0.75f);
		text.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		text.SetPosition(MTextWidget.Pos);
		text.FadeToDefaultTextColor = false;
		_mTextWidgetRotationList.Add(text);
		MilMo_Widget icon = new MilMo_Widget(UI);
		icon.SetScale(32f, 32f);
		icon.SetAlignment(MilMo_GUI.Align.CenterRight);
		icon.SetPosition(Scale.x - 42f, 16f);
		icon.FadeToDefaultColor = false;
		_mIconWidgetRotationList.Add(icon);
		MilMo_Item.AsyncGetItem(item.Item, delegate(MilMo_Item itemLoaded)
		{
			MilMo_LocString copy = MilMo_Localization.GetLocString("QuestLog_9352").GetCopy();
			if (itemLoaded.Template.DisplayName.String.Length > 14)
			{
				string @string = itemLoaded.Template.DisplayName.String;
				@string = @string.Remove(14);
				@string += "...";
				copy.SetFormatArgs(@string);
			}
			else
			{
				copy.SetFormatArgs(itemLoaded.Template.DisplayName);
			}
			text.SetText(copy);
			itemLoaded.AsyncGetIcon(delegate(Texture2D tex)
			{
				icon.SetTexture(tex);
			});
		});
	}

	private void Tick()
	{
		MLeftSideWidgets[0].TextColorTo(1f, 1f, 1f, 0f);
		_mIconWidgetRotationList[_mCurrentIndex].AlphaTo(0f);
		_mTextWidgetRotationList[_mCurrentIndex].TextColorTo(1f, 1f, 1f, 0f);
		_mCurrentIndex++;
		MTickEvent = MilMo_EventSystem.At(2f, delegate
		{
			RemoveChild(_mIconWidgetRotationList[_mCurrentIndex - 1]);
			RemoveChild(_mTextWidgetRotationList[_mCurrentIndex - 1]);
			if (_mCurrentIndex == _mTextWidgetRotationList.Count)
			{
				_mCurrentIndex = 0;
			}
			_mIconWidgetRotationList[_mCurrentIndex].SetAlpha(0f);
			_mTextWidgetRotationList[_mCurrentIndex].TextColorNow(1f, 1f, 1f, 0f);
			AddChild(_mIconWidgetRotationList[_mCurrentIndex]);
			AddChild(_mTextWidgetRotationList[_mCurrentIndex]);
			_mIconWidgetRotationList[_mCurrentIndex].AlphaTo(1f);
			_mTextWidgetRotationList[_mCurrentIndex].TextColorTo(1f, 1f, 1f, 1f);
			MLeftSideWidgets[0].TextColorTo(1f, 1f, 1f, 1f);
			MLeftSideWidgets[0].SetTextNoLocalization(_mChangingCounters[_mCurrentIndex]);
			MTickEvent = MilMo_EventSystem.At(2f, Tick);
		});
	}
}
