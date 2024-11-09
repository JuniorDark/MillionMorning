using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.Items;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.World.Inventory;
using Code.World.Player;
using Core;
using UnityEngine;

namespace Code.World.GUI.Converters;

internal sealed class MilMo_ConverterPopupIngredient : MilMo_Widget
{
	private readonly MilMo_Widget m_Icon;

	private readonly MilMo_SimpleLabel m_Txt;

	private MilMo_ConverterPopupIngredient(MilMo_UserInterface ui, float width)
		: base(ui)
	{
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetScale(width, 25f);
		UseParentAlpha = false;
		AllowPointerFocus = false;
		FadeToDefaultColor = false;
		m_Icon = new MilMo_Widget(UI);
		m_Icon.SetAlignment(MilMo_GUI.Align.CenterLeft);
		m_Icon.FadeToDefaultColor = false;
		AddChild(m_Icon);
		m_Txt = new MilMo_SimpleLabel(UI);
		m_Txt.SetAlignment(MilMo_GUI.Align.CenterLeft);
		m_Txt.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		m_Txt.SetDefaultTextColor(0f, 0f, 0f, 1f);
		m_Txt.FadeToDefaultTextColor = false;
		AddChild(m_Txt);
		m_Icon.SetScale(22f, 22f);
		m_Icon.SetPosition(10f, Scale.y * 0.5f);
		m_Txt.SetScale(Scale.x - 22f, 25f);
		m_Txt.SetPosition(38f, Scale.y * 0.5f);
	}

	public MilMo_ConverterPopupIngredient(MilMo_UserInterface ui, string texturePath, MilMo_LocString name, int amountRequired, int amountGot, float width)
		: this(ui, width)
	{
		MilMo_LocString notLocalizedLocString = MilMo_Localization.GetNotLocalizedLocString("{0} {1}/{2}");
		if (amountGot > amountRequired)
		{
			amountGot = amountRequired;
		}
		notLocalizedLocString.SetFormatArgs(name, amountGot, amountRequired);
		m_Txt.SetText(notLocalizedLocString);
		m_Icon.SetTexture(texturePath, prefixStandardGuiPath: false);
	}

	public MilMo_ConverterPopupIngredient(MilMo_UserInterface ui, TemplateReference ingredientItem, int amountRequired, float width)
		: this(ui, width)
	{
		MilMo_ConverterPopupIngredient milMo_ConverterPopupIngredient = this;
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(ingredientItem, delegate(MilMo_Template template, bool timeout)
		{
			if (template != null)
			{
				MilMo_Item.AsyncGetItem(template.Identifier, delegate(MilMo_Item item)
				{
					MilMo_LocString notLocalizedLocString = MilMo_Localization.GetNotLocalizedLocString("{0} {1}/{2}");
					MilMo_InventoryEntry entryByIdentifier = MilMo_Player.Instance.Inventory.GetEntryByIdentifier(item.Identifier);
					int num = 0;
					if (entryByIdentifier != null)
					{
						num = entryByIdentifier.Amount;
					}
					if (num > amountRequired)
					{
						num = amountRequired;
					}
					notLocalizedLocString.SetFormatArgs(item.Template.DisplayName, num, amountRequired);
					milMo_ConverterPopupIngredient.m_Txt.SetText(notLocalizedLocString);
					item.AsyncGetIcon(delegate(Texture2D texture)
					{
						milMo_ConverterPopupIngredient.m_Icon.SetTexture(texture);
					});
				});
			}
		});
	}
}
