using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.Items;
using Code.Core.ResourceSystem;
using Code.World.Quest;
using UnityEngine;

namespace Code.World.GUI.QuestLog;

public sealed class MilMo_QuestRewardWidget : MilMo_Widget
{
	private MilMo_Widget _mIcon;

	private MilMo_Widget _mTextWidget;

	public MilMo_QuestRewardWidget(MilMo_UserInterface ui, MilMo_QuestRewardItem item)
		: base(ui)
	{
		MilMo_QuestRewardWidget milMo_QuestRewardWidget = this;
		SetUpDefaults();
		MilMo_Item.AsyncGetItem(item.ItemTemplateIdentifier, delegate(MilMo_Item theItem)
		{
			theItem.AsyncGetIcon(delegate(Texture2D texture)
			{
				milMo_QuestRewardWidget._mIcon.SetTexture(texture);
				MilMo_LocString milMo_LocString = theItem.Template.DisplayName;
				if (theItem.Template.DisplayName.Length > 16)
				{
					milMo_LocString = MilMo_Localization.GetNotLocalizedLocString(theItem.Template.DisplayName.String.Substring(0, 14) + "...");
				}
				if (item.Amount > 1)
				{
					MilMo_LocString copy = MilMo_Localization.GetNotLocalizedLocString("{0} {1}").GetCopy();
					copy.SetFormatArgs(item.Amount.ToString(), milMo_LocString);
					milMo_QuestRewardWidget._mTextWidget.SetText(copy);
				}
				else
				{
					milMo_QuestRewardWidget._mTextWidget.SetText(milMo_LocString);
				}
			});
		});
	}

	public MilMo_QuestRewardWidget(MilMo_UserInterface ui, string texturePath, int itemCount, MilMo_LocString itemName)
		: base(ui)
	{
		SetUpDefaults();
		_mIcon.SetTexture(texturePath, prefixStandardGuiPath: false);
		if (itemCount > 1)
		{
			MilMo_LocString copy = MilMo_Localization.GetNotLocalizedLocString("{0} {1}").GetCopy();
			copy.SetFormatArgs(itemCount.ToString(), itemName);
			_mTextWidget.SetText(copy);
		}
		else
		{
			_mTextWidget.SetText(itemName);
		}
	}

	public MilMo_QuestRewardWidget(MilMo_UserInterface ui, int expReward)
		: base(ui)
	{
		SetUpDefaults();
		_mIcon.SetFont(MilMo_GUI.Font.EborgMedium);
		_mIcon.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		_mIcon.SetDefaultTextColor(0f, 1f, 0f, 1f);
		_mIcon.SetTextNoLocalization("XP");
		_mIcon.SetFontPreset(MilMo_GUI.FontPreset.DropShadow);
		_mTextWidget.SetTextNoLocalization(expReward + " xp");
	}

	private void SetUpDefaults()
	{
		SetScale(150f, 35f);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		_mIcon = new MilMo_Widget(UI);
		_mIcon.SetAlignment(MilMo_GUI.Align.CenterCenter);
		_mIcon.SetPosition(16f, 16f);
		_mIcon.SetScale(32f, 32f);
		AddChild(_mIcon);
		_mTextWidget = new MilMo_Widget(UI);
		_mTextWidget.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mTextWidget.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		_mTextWidget.SetPosition(34f, 7f);
		_mTextWidget.SetScale(116f, 21f);
		_mTextWidget.SetFont(MilMo_GUI.Font.EborgSmall);
		_mTextWidget.SetFontScale(0.75f);
		AddChild(_mTextWidget);
	}
}
