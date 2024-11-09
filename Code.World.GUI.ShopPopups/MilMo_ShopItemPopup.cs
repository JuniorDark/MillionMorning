using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget.SimpleWindow.Window.Popup;
using Code.Core.ResourceSystem;
using Code.World.CharacterShop.RemoteShop;
using UnityEngine;

namespace Code.World.GUI.ShopPopups;

public class MilMo_ShopItemPopup : MilMo_PicturePopup
{
	protected readonly MilMo_Widget MDarkBack;

	protected MilMo_PriceWidget MPriceWidget;

	public MilMo_ShopItemPopup(MilMo_UserInterface ui, MilMo_LocString caption, MilMo_LocString description, MilMo_Texture texture)
		: this(ui, caption, description, Vector2.zero, texture)
	{
	}

	protected MilMo_ShopItemPopup(MilMo_UserInterface ui, MilMo_LocString caption, MilMo_LocString description, Vector2 textArea, MilMo_Texture texture)
		: base(ui, caption, description, (textArea == Vector2.zero) ? new Vector2(242f, 70f) : textArea)
	{
		Identifier = "ItemPopup";
		base.Icon.SetTexture(texture);
		SetColor(0.496f, 0.495f, 0.89f, 1f);
		Background.SetDefaultColor(0.496f, 0.495f, 0.89f, 1f);
		MDarkBack = new MilMo_Widget(UI);
		MDarkBack.SetTextureWhite();
		MDarkBack.SetDefaultColor(0.6f, 0.6f, 1f, 1f);
		MDarkBack.SetPosition(1f, 24f);
		MDarkBack.SetScale(Scale.x - ShadePadding, 84f);
		MDarkBack.SetAlignment(MilMo_GUI.Align.TopLeft);
		MDarkBack.FadeToDefaultColor = false;
		MDarkBack.SetFadeSpeed(0.08f);
		MDarkBack.UseParentAlpha = false;
		MDarkBack.AllowPointerFocus = false;
		AddChild(MDarkBack);
		SendToBack(MDarkBack);
		SendToBack(Background);
		SendToBack(BlackOutline);
		SendToBack(BackShade);
		BottomPad = 58f;
	}

	protected override void RefreshWidgets()
	{
		base.RefreshWidgets();
		MPriceWidget?.RefreshUI();
		if (base.Icon != null)
		{
			base.Icon.SetScale(48f, 48f);
			base.Icon.ScaleTo(80f, 80f);
			BringToFront(base.Icon);
		}
		MDarkBack.SetXScale(Scale.x - ShadePadding - 2f);
	}

	public override void RefreshIconAlignment()
	{
		base.RefreshIconAlignment();
		base.TextWidget?.SetPosition(10f, 113f);
		base.Icon?.SetPosition(216f, 66f);
	}

	protected override void FadeIn()
	{
		MDarkBack.SetAlpha(0f);
		MDarkBack.AlphaTo(MDarkBack.DefaultColor.a);
		if (MPriceWidget != null)
		{
			MPriceWidget.SetAlpha(0f);
			MPriceWidget.AlphaTo(1f);
			foreach (MilMo_Widget child in MPriceWidget.Children)
			{
				child.SetAlpha(0f);
				child.AlphaTo(child.DefaultColor.a);
			}
		}
		base.FadeIn();
	}

	public override void FadeOut()
	{
		MDarkBack.AlphaTo(0f);
		if (MPriceWidget != null)
		{
			MPriceWidget.AlphaTo(0f);
			foreach (MilMo_Widget child in MPriceWidget.Children)
			{
				child.SetAlpha(0f);
			}
		}
		base.FadeOut();
	}

	protected override void InitWidgets()
	{
		RefreshIconAlignment();
		MTextWidget.SetPosition(10f + base.IconScale, 10f + base.CaptionOffset);
		float num = base.IconScale + base.CaptionOffset + MIconPadding * 2f + BottomPad + 2f;
		if (ScaleMover.Target.y < num)
		{
			SetYScale(num);
		}
	}

	public void SetPrice(MilMo_ShopItem item)
	{
		InitWidgets();
		RefreshWidgets();
		if (MPriceWidget == null)
		{
			MPriceWidget = new MilMo_PriceWidget(UI, item);
			MPriceWidget.SetDefaultColor(MDarkBack.CurrentColor);
		}
		AddChild(MPriceWidget);
		float num = Scale.y + 70f;
		if (ScaleMover.Target.y < num)
		{
			SetYScale(num);
		}
		MPriceWidget.SetScale(Scale.x - ShadePadding - 2f, 70f);
		MPriceWidget.SetPosition(1f, Scale.y - 12f - 70f - 1f);
		if (item.CashDiscount.IsActive() || item.CoinDiscount.IsActive() || item.GiftDiscount.IsActive())
		{
			MPriceWidget.SetDiscount(item.CoinDiscount, item.CashDiscount, item.CashDiscount.IsActive() ? item.CashDiscount : item.GiftDiscount);
		}
		RefreshWidgets();
	}
}
