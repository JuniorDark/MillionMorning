using System;
using System.Linq;
using Code.Core.Config;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.Items;
using Code.Core.Monetization;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Code.Core.Utility;
using Code.World.CharacterShop.RemoteShop;
using Code.World.GUI.ShopPopups;
using Code.World.Player;
using UnityEngine;

namespace Code.World.CharacterShop;

public class MilMo_ItemCard : MilMo_Button
{
	private enum ThumbnailBehavior
	{
		Default,
		Shirt,
		Pants,
		Shoe,
		Hat,
		Accessory,
		Dress,
		Skirt
	}

	private MilMo_ShopItemPopup _popup;

	private Vector2 _textAreaSize = new Vector2(220f, 220f);

	private MilMo_LocString _description;

	public MilMo_Button BuyButton;

	public MilMo_Button Thumbnail;

	public MilMo_Button GiftButton;

	public MilMo_Widget PriceTag;

	public MilMo_Widget CoinsPriceTag;

	private MilMo_Widget _textFade;

	private MilMo_Widget _currencyIcon;

	private MilMo_Widget _coinsIcon;

	private MilMo_Widget _tryLabel;

	private MilMo_Widget _tryArrow;

	private MilMo_Widget _specialTag;

	private Vector2 _currentThumbScale = new Vector2(64f, 64f);

	private Vector2 _currentThumbPos = new Vector2(50f, 80f);

	private MilMo_TimerEvent _showPopupSchedule;

	private MilMo_TimerEvent _showTryArrowSchedule;

	private readonly MilMo_EventSystem.MilMo_Callback _showtryarrow;

	private readonly MilMo_EventSystem.MilMo_Callback _showpopup;

	private readonly MilMo_EventSystem.MilMo_Callback _impulse;

	private readonly MilMo_AudioClip _tickSound = new MilMo_AudioClip("Content/Sounds/Batch01/GUI/Generic/Tick");

	private bool _iconHasArrived;

	private ThumbnailBehavior _thumbnailBehavior;

	public MilMo_ShopItem ShopItem { get; private set; }

	public bool HaveIcon { get; private set; }

	public MilMo_ItemCard(MilMo_UserInterface ui, MilMo_CharacterShop shop, MilMo_ShopItem shopItem)
		: base(ui)
	{
		HaveIcon = false;
		Identifier = "ItemCard" + MilMo_UserInterface.GetRandomID();
		ShopItem = shopItem;
		_description = MilMo_Localization.GetNotLocalizedLocString("...");
		_showpopup = ShowPopup;
		_showtryarrow = ShowTryArrow;
		_impulse = OnImpulse;
		Setup();
		SetupCurrencyWidget();
		SetupCoinsIcon();
		SetupPriceTag();
		SetupCoinsPriceTag();
		SetupTryLabel();
		SetupThumbnail(shop);
		SetupBuyButton();
		SetupGiftButton();
		SetupTryArrow();
		SetupTextFade();
		SetupSpecialTag();
		CustomFunction = EnterView;
		RefreshUI();
		MilMo_Localization.RegisterLanguageChangedCallback(UpdateSpecialTag);
	}

	private void SetupSpecialTag()
	{
		_specialTag = new MilMo_Widget(UI);
		_specialTag.SetAlignment(MilMo_GUI.Align.BottomLeft);
		_specialTag.AllowPointerFocus = false;
		_specialTag.SetDefaultColor(1f, 1f, 1f, 1f);
		_specialTag.FadeToDefaultColor = false;
		_specialTag.UseParentAlpha = false;
		_specialTag.Enabled = false;
		AddChild(_specialTag);
	}

	private void SetupTextFade()
	{
		_textFade = new MilMo_Widget(UI)
		{
			Identifier = "TextFade",
			AllowPointerFocus = false
		};
		_textFade.SetTexture("Batch01/Textures/Shop/ItemCardTextFade");
		_textFade.SetAlignment(MilMo_GUI.Align.TopRight);
		AddChild(_textFade);
	}

	private void SetupTryArrow()
	{
		_tryArrow = new MilMo_Widget(UI)
		{
			AllowPointerFocus = false,
			Identifier = "TryArrow" + MilMo_UserInterface.GetRandomID()
		};
		_tryArrow.SetTexture("Batch01/Textures/Shop/FatArrow");
		_tryArrow.SetAlignment(MilMo_GUI.Align.CenterCenter);
		_tryArrow.ColorNow(1f, 1f, 1f, 0f);
		_tryArrow.FadeToDefaultColor = false;
		_tryArrow.SetFadeSpeed(0.1f);
		_tryArrow.SetPosPull(0.06f, 0.06f);
		_tryArrow.SetPosDrag(0.7f, 0.7f);
		AddChild(_tryArrow);
	}

	private void SetupGiftButton()
	{
		GiftButton = new MilMo_Button(UI);
		GiftButton.SetTexture("Batch01/Textures/Core/Invisible");
		GiftButton.Identifier = "GiftButton" + MilMo_UserInterface.GetRandomID();
		GiftButton.SetHoverTexture("Batch01/Textures/Core/Invisible");
		GiftButton.SetPressedTexture("Batch01/Textures/Core/Invisible");
		GiftButton.SetText(MilMo_Localization.GetLocString("CharacterShop_269"));
		GiftButton.SetFont(MilMo_GUI.Font.GothamMedium);
		GiftButton.SetAlignment(MilMo_GUI.Align.TopLeft);
		GiftButton.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		GiftButton.SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Impulse);
		GiftButton.SetDefaultTextColor(1f, 1f, 1f, 0.4f);
		GiftButton.SetHoverTextColor(1f, 1f, 1f, 0.8f);
		GiftButton.SetHoverImpulseColor(1f, 1f, 1f, 1f);
		GiftButton.SetFadeInSpeed(0.009f);
		GiftButton.SetFadeOutSpeed(0.6f);
		GiftButton.SetHoverSound(_tickSound);
		AddChild(GiftButton);
	}

	private void SetupBuyButton()
	{
		BuyButton = new MilMo_Button(UI);
		BuyButton.SetTexture("Batch01/Textures/Core/Invisible");
		BuyButton.Identifier = "BuyButton" + MilMo_UserInterface.GetRandomID();
		BuyButton.SetHoverTexture("Batch01/Textures/Core/Invisible");
		BuyButton.SetPressedTexture("Batch01/Textures/Core/Invisible");
		BuyButton.SetText(MilMo_Localization.GetLocString("CharacterShop_268"));
		BuyButton.SetFont(MilMo_GUI.Font.GothamLarge);
		BuyButton.SetAlignment(MilMo_GUI.Align.BottomLeft);
		BuyButton.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		BuyButton.SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Impulse);
		BuyButton.SetDefaultTextColor(1f, 1f, 1f, 0.75f);
		BuyButton.SetHoverTextColor(1f, 1f, 1f, 1f);
		BuyButton.SetHoverImpulseColor(1f, 1f, 1f, 1f);
		BuyButton.SetFadeInSpeed(0.009f);
		BuyButton.SetFadeOutSpeed(0.6f);
		BuyButton.SetHoverSound(_tickSound);
		AddChild(BuyButton);
	}

	private void SetSpecificMouseOverBehaviour(MilMo_CharacterShop shop)
	{
		foreach (MilMo_Button item in shop.CurCategory.TabList.Where((MilMo_Button tab) => tab.Info == shop.CurCategory.CurrentSubCategory))
		{
			_thumbnailBehavior = item.Identifier switch
			{
				"Dresses" => ThumbnailBehavior.Dress, 
				"Hats" => ThumbnailBehavior.Hat, 
				"Caps" => ThumbnailBehavior.Hat, 
				"Skirts" => ThumbnailBehavior.Skirt, 
				_ => _thumbnailBehavior, 
			};
		}
	}

	private void SetGenericMouseOverBehaviour(MilMo_CharacterShop shop)
	{
		if (shop.CurCategory.IdentifierName != null)
		{
			_thumbnailBehavior = shop.CurCategory.IdentifierName switch
			{
				"Upper_Body" => ThumbnailBehavior.Shirt, 
				"Lower_Body" => ThumbnailBehavior.Pants, 
				"Shoes" => ThumbnailBehavior.Shoe, 
				"Accessories" => ThumbnailBehavior.Accessory, 
				_ => ThumbnailBehavior.Default, 
			};
		}
	}

	private void SetupThumbnail(MilMo_CharacterShop shop)
	{
		Thumbnail = new MilMo_Button(UI);
		Thumbnail.TextColorTo(0f, 1f, 1f, 1f);
		Thumbnail.Identifier = "Thumbnail" + MilMo_UserInterface.GetRandomID();
		Thumbnail.SetHoverScaleMode(MilMo_GUI.HoverBehaviour.Impulse);
		Thumbnail.SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
		Thumbnail.ScaleMover.MinVel = new Vector2(0.01f, 0.01f);
		Thumbnail.SetDefaultColor(1f, 1f, 1f, 1f);
		Thumbnail.SetHoverColor(1f, 1f, 1f, 1f);
		Thumbnail.SetAnglePull(0.05f);
		Thumbnail.SetAngleDrag(0.6f);
		Thumbnail.SetAlignment(MilMo_GUI.Align.CenterCenter);
		Thumbnail.SetPosPull(0.05f, 0.05f);
		Thumbnail.SetPosDrag(0.6f, 0.6f);
		Thumbnail.SetFadeSpeed(0.001f);
		Thumbnail.SetFadeOutSpeed(0.016f);
		Thumbnail.SetFadeInSpeed(0.016f);
		Thumbnail.Function = null;
		Thumbnail.Args = null;
		Thumbnail.AllowPointerFocus = false;
		SetGenericMouseOverBehaviour(shop);
		SetSpecificMouseOverBehaviour(shop);
		AddChild(Thumbnail);
	}

	private void SetupTryLabel()
	{
		_tryLabel = new MilMo_Widget(UI)
		{
			AllowPointerFocus = false,
			Identifier = "TryLabel" + MilMo_UserInterface.GetRandomID()
		};
		_tryLabel.SetTexture("Batch01/Textures/Core/Invisible");
		_tryLabel.SetText(MilMo_Localization.GetLocString("CharacterShop_267"));
		_tryLabel.SetFont(MilMo_GUI.Font.GothamMedium);
		_tryLabel.SetAlignment(MilMo_GUI.Align.TopRight);
		_tryLabel.SetTextAlignment(MilMo_GUI.Align.CenterRight);
		_tryLabel.TextColorNow(1f, 1f, 1f, 0f);
		_tryLabel.SetFadeSpeed(0.12f);
		_tryLabel.FadeToDefaultTextColor = false;
		_tryLabel.SetPosPull(0.15f, 0.15f);
		_tryLabel.SetPosDrag(0.5f, 0.5f);
		AddChild(_tryLabel);
	}

	private void SetupCoinsPriceTag()
	{
		CoinsPriceTag = new MilMo_Widget(UI)
		{
			Identifier = "CoinsPriceTag" + MilMo_UserInterface.GetRandomID(),
			AllowPointerFocus = false
		};
		CoinsPriceTag.SetAlignment(MilMo_GUI.Align.CenterLeft);
		CoinsPriceTag.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		CoinsPriceTag.SetScalePull(0.05f, 0.05f);
		CoinsPriceTag.SetScaleDrag(0.5f, 0.5f);
		CoinsPriceTag.TextColorNow(0f, 0f, 0f, 0.52f);
		CoinsPriceTag.SetTextDropShadowPos(3f, 3f);
		CoinsPriceTag.TextDropShadowColor = new Color(0f, 0f, 0f, 0.4f);
		CoinsPriceTag.SetDefaultTextColor(1f, 0.92f, 0f, 1f);
		CoinsPriceTag.SetPosPull(0.04f, 0.04f);
		CoinsPriceTag.SetPosDrag(0.7f, 0.7f);
		CoinsPriceTag.SetFont(MilMo_GUI.Font.EborgMedium);
		CoinsPriceTag.SetFadeSpeed(0.008f);
		AddChild(CoinsPriceTag);
	}

	private void SetupPriceTag()
	{
		PriceTag = new MilMo_Widget(UI)
		{
			Identifier = "PriceTag" + MilMo_UserInterface.GetRandomID(),
			AllowPointerFocus = false
		};
		PriceTag.SetAlignment(MilMo_GUI.Align.CenterLeft);
		PriceTag.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		PriceTag.SetScalePull(0.05f, 0.05f);
		PriceTag.SetScaleDrag(0.5f, 0.5f);
		PriceTag.TextColorNow(0f, 0f, 0f, 0.52f);
		PriceTag.SetTextDropShadowPos(3f, 3f);
		PriceTag.TextDropShadowColor = new Color(0f, 0f, 0f, 0.4f);
		PriceTag.SetDefaultTextColor(1f, 0.92f, 0f, 1f);
		PriceTag.SetPosPull(0.04f, 0.04f);
		PriceTag.SetPosDrag(0.7f, 0.7f);
		PriceTag.SetFont(MilMo_GUI.Font.EborgMedium);
		PriceTag.SetFadeSpeed(0.008f);
		AddChild(PriceTag);
	}

	private void SetupCoinsIcon()
	{
		_coinsIcon = new MilMo_Widget(UI)
		{
			Identifier = "IngameCurrencyIcon" + MilMo_UserInterface.GetRandomID(),
			AllowPointerFocus = false
		};
		_coinsIcon.SetTexture("Batch01/Textures/Shop/Coins");
		_coinsIcon.SetAlignment(MilMo_GUI.Align.CenterLeft);
		_coinsIcon.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		_coinsIcon.SetFadeSpeed(0.008f);
		AddChild(_coinsIcon);
	}

	private void SetupCurrencyWidget()
	{
		_currencyIcon = new MilMo_Widget(UI)
		{
			Identifier = "CurrencyIcon" + MilMo_UserInterface.GetRandomID(),
			AllowPointerFocus = false
		};
		_currencyIcon.SetTexture(MilMo_Monetization.Instance.Currency.IconPath);
		_currencyIcon.SetAlignment(MilMo_GUI.Align.CenterLeft);
		_currencyIcon.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		_currencyIcon.SetFadeSpeed(0.008f);
		AddChild(_currencyIcon);
	}

	private void Setup()
	{
		SetAllTextures("Batch01/Textures/Shop/ItemCard");
		TextColorNow(1f, 1f, 1f, 1f);
		TextColorTo(1f, 1f, 1f, 1f);
		SetFontScale(1f);
		SetTextAlignment(MilMo_GUI.Align.TopLeft);
		SetAnglePull(0.05f);
		SetAngleDrag(0.6f);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetPosPull(0.05f, 0.05f);
		SetPosDrag(0.6f, 0.6f);
		SetFont(MilMo_GUI.Font.GothamMedium);
		SetTextDropShadowPos(3f, 2f);
		TextDropShadowColor = new Color(0.1f, 0.2f, 0.43f, 0.5f);
		PosMover.Arrive = ScaleUpThumbnail;
		PointerHoverFunction = OnItemMouseOver;
		PointerLeaveFunction = OnItemMouseLeave;
		SetTextOffset(2f, 2f);
		SetExtraDrawTextSize(-18f, 0f);
		SetDefaultColor(1f, 1f, 1f, 1f);
		SetColor(0f, 0f, 0f, 0f);
		SetFadeOutSpeed(0.04f);
		FadeToDefaultColor = true;
	}

	private void RemoveDiscount()
	{
		_specialTag.Enabled = false;
		PriceTag.SetDefaultTextColor(1f, 0.92f, 0f, 1f);
		CoinsPriceTag.SetDefaultTextColor(1f, 0.92f, 0f, 1f);
		CoinsPriceTag.SetTextNoLocalization(ShopItem.GetCoinPrice().ToString());
		PriceTag.SetTextNoLocalization(ShopItem.GetPrice(MilMo_Player.Instance.IsMember, isGift: false).ToString());
	}

	public void SetIcon(Texture2D icon)
	{
		if (icon == null)
		{
			Thumbnail.SetAllTextures("Batch01/Textures/World/StreamingArrow");
			_currentThumbPos = new Vector2(50.5f, 70f);
			Thumbnail.SetPosition(_currentThumbPos);
			Thumbnail.SetScaleAbsolute(UI.ScaleToLowestUIRes(_currentThumbScale));
			Thumbnail.HasLinearRotation = true;
			Thumbnail.AngleMover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Linear);
			Thumbnail.AngleMover.Vel.x = 5f;
			Thumbnail.SetDefaultColor(1f, 1f, 1f, 0.4f);
			Thumbnail.SetHoverColor(1f, 1f, 1f, 0.4f);
			Thumbnail.SetColor(1f, 1f, 1f, 0.4f);
		}
		else if (!_iconHasArrived)
		{
			_iconHasArrived = true;
			Thumbnail.SetTexture(new MilMo_Texture(icon));
			Thumbnail.SetHoverTexture(new MilMo_Texture(icon));
			Thumbnail.SetPressedTexture(new MilMo_Texture(icon));
			_currentThumbPos = new Vector2(50.5f, 80f);
			Thumbnail.SetPosition(_currentThumbPos);
			_currentThumbScale = new Vector2(100f, 100f);
			Thumbnail.SetScaleAbsolute(UI.ScaleToLowestUIRes(_currentThumbScale));
			Thumbnail.SetDefaultScale(_currentThumbScale);
			Thumbnail.SetAngle(0f);
			Thumbnail.HasLinearRotation = false;
			Thumbnail.AngleMover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Spring);
			Thumbnail.SetDefaultColor(1f, 1f, 1f, 1f);
			Thumbnail.SetHoverColor(1f, 1f, 1f, 1f);
			Thumbnail.SetColor(1f, 1f, 1f, 0f);
			Thumbnail.SetAlignment(MilMo_GUI.Align.CenterCenter);
			Thumbnail.SetScalePull(0.15f, 0.15f);
			Thumbnail.SetScaleDrag(0.6f, 0.6f);
			Thumbnail.SetScale(0f, 0f);
			switch (_thumbnailBehavior)
			{
			case ThumbnailBehavior.Shirt:
				_currentThumbScale = new Vector2(95f, 95f);
				Thumbnail.ScaleToAbsolute(UI.ScaleToLowestUIRes(_currentThumbScale));
				break;
			case ThumbnailBehavior.Skirt:
				_currentThumbScale = new Vector2(95f, 95f);
				Thumbnail.ScaleToAbsolute(UI.ScaleToLowestUIRes(_currentThumbScale));
				break;
			case ThumbnailBehavior.Accessory:
				_currentThumbScale = new Vector2(95f, 95f);
				Thumbnail.ScaleToAbsolute(UI.ScaleToLowestUIRes(_currentThumbScale));
				break;
			case ThumbnailBehavior.Pants:
				Thumbnail.SetScalePull(0.1f, 0.1f);
				Thumbnail.SetScaleDrag(0.7f, 0.7f);
				Thumbnail.SetAlignment(MilMo_GUI.Align.TopCenter);
				_currentThumbPos = new Vector2(50.5f, 30f);
				Thumbnail.SetPosition(_currentThumbPos);
				_currentThumbScale = new Vector2(95f, 95f);
				Thumbnail.ScaleToAbsolute(UI.ScaleToLowestUIRes(_currentThumbScale));
				break;
			case ThumbnailBehavior.Dress:
				Thumbnail.SetScalePull(0.1f, 0.1f);
				Thumbnail.SetScaleDrag(0.9f, 0.9f);
				Thumbnail.SetAlignment(MilMo_GUI.Align.TopCenter);
				_currentThumbPos = new Vector2(50.5f, 30f);
				Thumbnail.SetPosition(50f, 10f);
				_currentThumbScale = new Vector2(95f, 95f);
				Thumbnail.ScaleToAbsolute(UI.ScaleToLowestUIRes(_currentThumbScale));
				break;
			default:
				_currentThumbScale = new Vector2(95f, 95f);
				Thumbnail.ScaleToAbsolute(UI.ScaleToLowestUIRes(_currentThumbScale));
				break;
			}
			RefreshThumbScale();
		}
	}

	public void RefreshUI()
	{
		SetScale(200f, 120f);
		SetFixedPointerZoneSize(100f, 100f);
		SetFontScale(1f);
		UpdateTextFade();
		UpdateTags();
		UpdateTryLabel();
		UpdateThumbnail();
		UpdateButtons();
		_tryArrow.SetPosition(50f, 85f);
		_tryArrow.SetScale(50f, 50f);
		_specialTag.SetScale(128f, 64f);
		_specialTag.SetPosition(-6f, 135f);
	}

	private void UpdateTryLabel()
	{
		_tryLabel.SetPosition(190f, 85f);
		_tryLabel.SetScale(250f, 50f);
		_tryLabel.SetFontScale(1f);
		_tryLabel.SetTextDropShadowPos(2f, 2f);
	}

	private void UpdateButtons()
	{
		BuyButton.SetPosition(97f, 92f);
		BuyButton.SetScale(100f, 20f);
		BuyButton.SetFontScale(1f);
		if (MilMo_Localization.CurrentLanguage.IsPortugueseBrazilian)
		{
			BuyButton.SetFontScale(0.82f, 1f);
		}
		BuyButton.SetTextOffset(0f, 0f);
		GiftButton.SetFontScale(1f);
		GiftButton.SetPosition(97f, 93f);
		GiftButton.SetScale(100f, 20f);
		GiftButton.SetTextOffset(0f, -6f);
	}

	private void UpdateThumbnail()
	{
		Thumbnail.SetFixedPointerZoneSize(198f, 98f);
		Thumbnail.SetFixedPointerZoneOffset(0f, -10f);
		Thumbnail.SetPosition(_currentThumbPos);
		Thumbnail.SetDefaultScale(_currentThumbScale);
		Thumbnail.SetScaleAbsolute(UI.ScaleToLowestUIRes(_currentThumbScale));
		float num = Mathf.Min(Thumbnail.ScaleMover.Target.x, Thumbnail.ScaleMover.Target.y);
		Thumbnail.ScaleMover.Target.x = num;
		Thumbnail.ScaleMover.Target.y = num;
		Thumbnail.ScaleMover.Val.x = num;
		Thumbnail.ScaleMover.Val.y = num;
		Thumbnail.Scale.x = num;
		Thumbnail.Scale.y = num;
		Thumbnail.DefaultScale.x = num;
		Thumbnail.DefaultScale.y = num;
	}

	private void UpdateTags()
	{
		UpdatePriceTag();
		UpdateCurrencyIcon();
		MilMo_ShopItem shopItem = ShopItem;
		if (shopItem != null && shopItem.CanBuyWithCoins)
		{
			CoinsPriceTag.SetPosition(129f, 36f);
			CoinsPriceTag.SetScale(80f, 30f);
			_coinsIcon.SetPosition(97f, 36f);
			_coinsIcon.SetScale(28f, 28f);
		}
		else
		{
			PriceTag.SetPosition(129f, 48f);
			_currencyIcon.SetPosition(93f, 50f);
			_currencyIcon.SetScale(30f, 30f);
			CoinsPriceTag.SetEnabled(e: false);
			_coinsIcon.SetEnabled(e: false);
		}
		PriceTag.SetFontScale(1f);
		CoinsPriceTag.SetFontScale(1f);
	}

	private void UpdateCurrencyIcon()
	{
		_currencyIcon.SetPosition(97f, 60f);
		_currencyIcon.SetScale(24f, 24f);
	}

	private void UpdatePriceTag()
	{
		PriceTag.SetPosition(129f, 58f);
		PriceTag.SetScale(80f, 30f);
	}

	private void UpdateTextFade()
	{
		_textFade.SetPosition(194f, 6f);
		_textFade.SetScale(40f, 17f);
	}

	private void RefreshThumbScale()
	{
		float num = Mathf.Min(Thumbnail.ScaleMover.Target.x, Thumbnail.ScaleMover.Target.y);
		Thumbnail.ScaleMover.Target.x = num;
		Thumbnail.ScaleMover.Target.y = num;
		Thumbnail.DefaultScale.x = num;
		Thumbnail.DefaultScale.y = num;
	}

	public override void Draw()
	{
		base.Draw();
		if (_iconHasArrived)
		{
			Thumbnail.CurrentColor = CurrentColor;
		}
		DoPopupPosition();
	}

	public override void Step()
	{
		if (!_specialTag.Enabled)
		{
			if (ShopItem.IsNew || ShopItem.CoinDiscount.IsActive() || ShopItem.CashDiscount.IsActive() || ShopItem.GiftDiscount.IsActive())
			{
				UpdateSpecialTag();
			}
		}
		else if (_specialTag.Enabled && !ShopItem.IsNew && !ShopItem.CoinDiscount.IsActive() && !ShopItem.CashDiscount.IsActive() && !ShopItem.GiftDiscount.IsActive())
		{
			RemoveDiscount();
		}
		base.Step();
	}

	private void UpdateSpecialTag()
	{
		string text = "Batch01/Textures/Shop/";
		if (ShopItem.IsNew)
		{
			text += "ItemTagNew";
		}
		else if (ShopItem.CashDiscount.IsActive() || (ShopItem.CoinDiscount.IsActive() && ShopItem.GiftDiscount.IsActive()))
		{
			text += "ItemTagSale";
		}
		else if (ShopItem.CoinDiscount.IsActive())
		{
			text += "ItemTagCoinsSale";
		}
		else
		{
			if (!ShopItem.GiftDiscount.IsActive())
			{
				return;
			}
			text += "ItemTagGiftSale";
		}
		_specialTag.Enabled = true;
		_specialTag.SetTexture(MilMo_Localization.GetLocTexturePath(text));
	}

	private void DoPopupPosition()
	{
		if (_popup != null)
		{
			MilMo_ScrollView milMo_ScrollView = (MilMo_ScrollView)Parent;
			Vector2 position = default(Vector2);
			position.x = Pos.x / base.Res.x;
			position.y = Pos.y / base.Res.y;
			position.x += milMo_ScrollView.Pos.x / base.Res.x;
			position.y += milMo_ScrollView.Pos.y / base.Res.y;
			position.x += 30f;
			position.y -= _popup.Scale.y / base.Res.y;
			position.y += 10f;
			position.y -= milMo_ScrollView.SoftScroll.Val.y / base.Res.y;
			position.y = Mathf.Max(position.y, milMo_ScrollView.Pos.y / base.Res.y - _popup.Scale.y / base.Res.y);
			_popup.FixedRes = false;
			_popup.SetPosition(position);
			_popup.FixedRes = true;
		}
	}

	private void OnItemMouseOver()
	{
		ShowPopupSoon();
		if (_iconHasArrived)
		{
			Thumbnail.SetDefaultAngle(0f);
			Thumbnail.SetAngle(0f);
			Thumbnail.GoTo(50f, 80f);
			switch (_thumbnailBehavior)
			{
			case ThumbnailBehavior.Shirt:
				Thumbnail.ScaleTo(UI.ScaleToLowestUIRes(new Vector2(105f, 105f)));
				break;
			case ThumbnailBehavior.Skirt:
				Thumbnail.ScaleTo(UI.ScaleToLowestUIRes(new Vector2(105f, 105f)));
				break;
			case ThumbnailBehavior.Pants:
				_currentThumbPos = new Vector2(50.5f, 30f);
				Thumbnail.SetPosition(_currentThumbPos);
				Thumbnail.ScaleMover.Vel.y = 5f;
				Thumbnail.ScaleMover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Spring);
				break;
			case ThumbnailBehavior.Dress:
				_currentThumbPos = new Vector2(50.5f, 30f);
				Thumbnail.SetPosition(_currentThumbPos);
				Thumbnail.ScaleMover.Vel.y = 5f;
				Thumbnail.ScaleMover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Spring);
				break;
			case ThumbnailBehavior.Accessory:
				Thumbnail.ScaleTo(UI.ScaleToLowestUIRes(new Vector2(105f, 105f)));
				break;
			case ThumbnailBehavior.Hat:
				Thumbnail.GoTo(50f, 70f);
				Thumbnail.SetDefaultAngle(-15f);
				break;
			case ThumbnailBehavior.Shoe:
				Thumbnail.SetDefaultAngle(20f);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			case ThumbnailBehavior.Default:
				break;
			}
		}
		RefreshThumbScale();
	}

	private void OnItemMouseLeave()
	{
		HidePopupAndTryArrow();
		if (_iconHasArrived)
		{
			Thumbnail.GoTo(50f, 80f);
			if (_thumbnailBehavior == ThumbnailBehavior.Pants)
			{
				Thumbnail.GoTo(50f, 30f);
			}
			if (_thumbnailBehavior == ThumbnailBehavior.Dress)
			{
				Thumbnail.GoTo(50f, 30f);
			}
			Thumbnail.ScaleToAbsolute(UI.ScaleToLowestUIRes(_currentThumbScale));
			Thumbnail.SetDefaultAngle(0f);
		}
		RefreshThumbScale();
	}

	private void EnterView(object delay)
	{
		PosMover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Spring);
		Thumbnail.ScaleToAbsolute(UI.ScaleToLowestUIRes(_currentThumbScale));
	}

	private void ScaleUpThumbnail()
	{
		if (Parent.Info2 < 1)
		{
			MilMo_EventSystem.At(0.5f, null);
		}
	}

	private void ShowPopupSoon()
	{
		_showPopupSchedule = MilMo_EventSystem.At(0.5f, _showpopup);
		if (!MilMo_Config.Instance.IsTrue("Launcher.OfflineShop", defaultValue: false) && ShopItem.Item.IsWearable())
		{
			_showTryArrowSchedule = MilMo_EventSystem.At(4f, _showtryarrow);
		}
	}

	private void ShowPopup()
	{
		UI.BypassResolution();
		MilMo_Item item = ShopItem.Item;
		if (!(item is IMilMo_WeaponStats weapon))
		{
			if (item is MilMo_Converter)
			{
				CreateConverterPopup();
			}
			else
			{
				CreateItemPopup();
			}
		}
		else
		{
			CreateWeaponPopup(weapon);
		}
		_popup.SetPrice(ShopItem);
		UI.AddChild(_popup);
		_popup.PopupArrowOffset = -95f;
		_popup.SetTextColor(0f, 0f, 0f, 0.85f);
		_popup.SetCaptionTextColor(0f, 0f, 0f, 0.75f);
		UI.RestoreResolution();
	}

	private void CreateItemPopup()
	{
		_popup = new MilMo_ShopItemPopup(UI, ShopItem.DisplayName, _description, Thumbnail.Texture);
	}

	private void CreateConverterPopup()
	{
		_popup = new MilMo_ConverterPopup(UI, (MilMo_Converter)ShopItem.Item);
		_popup.PopupArrow.IsInvisible = false;
	}

	private void CreateWeaponPopup(IMilMo_WeaponStats weapon)
	{
		_textAreaSize.x = 242f;
		float getAttackStat = weapon.GetAttackStat;
		float getMagicStat = weapon.GetMagicStat;
		float getSpeedStat = weapon.GetSpeedStat;
		float getRangeStat = weapon.GetRangeStat;
		_popup = new MilMo_WeaponPopup(UI, ShopItem.Item.Template.DisplayName, _description, _textAreaSize, Thumbnail.Texture, getAttackStat, getMagicStat, getSpeedStat, getRangeStat);
		_popup.SetColor(0.6f, 0.6f, 1f, 1f);
		((MilMo_WeaponPopup)_popup).Icon.SetTexture(Thumbnail.Texture);
	}

	private void ShowTryArrow()
	{
		_tryLabel.PosMover.Vel.y = 0f;
		_tryLabel.TextColorNow(1f, 1f, 0f, 0f);
		_tryLabel.SetPosition(190f, 45f);
		_tryLabel.GoTo(190f, 81f);
		_tryLabel.TextColorTo(1f, 1f, 0f, 1f);
		_tryArrow.ColorTo(1f, 1f, 0f, 1f);
		_tryArrow.GoToNow(120f, 75f);
		_tryArrow.GoTo(175f, 75f);
		BuyButton.SetDefaultTextColor(1f, 1f, 1f, 0.2f);
		GiftButton.SetDefaultTextColor(1f, 1f, 1f, 0.2f);
	}

	private void HidePopupAndTryArrow()
	{
		if (_showTryArrowSchedule != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_showTryArrowSchedule);
		}
		if (_showPopupSchedule != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_showPopupSchedule);
		}
		_popup?.FadeOut();
		_tryLabel.TextColorTo(1f, 1f, 0f, 0f);
		_tryArrow.ColorTo(1f, 1f, 1f, 0f);
		BuyButton.SetDefaultTextColor(1f, 1f, 1f, 0.75f);
		GiftButton.SetDefaultTextColor(1f, 1f, 1f, 0.4f);
	}

	public void SetDescription(MilMo_LocString desc)
	{
		_description = desc;
		_textAreaSize.y = GetTextHeight(_description.String, _textAreaSize.x);
	}

	public void SetDescription(string desc)
	{
		SetDescription(MilMo_Localization.GetLocString(desc));
	}

	private float GetTextHeight(string msg, float width)
	{
		return UI.Font0.label.CalcHeight(new GUIContent(msg), width) + 15f;
	}

	public void Flash()
	{
		MilMo_EventSystem.At(1f, _impulse);
		MilMo_EventSystem.At(1.3f, _impulse);
		MilMo_EventSystem.At(1.6f, _impulse);
	}

	private void OnImpulse()
	{
		MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Attention);
		SetColor(1f, 1f, 0f, 1f);
		Impulse(0f, -10f);
	}
}
