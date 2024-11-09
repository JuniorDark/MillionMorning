using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.World.Player;
using UnityEngine;

namespace Code.World.CharacterShop.RemoteShop;

public class MilMo_PriceWidget : MilMo_Widget
{
	private class MilMo_CurrencyWidget : MilMo_Widget
	{
		private MilMo_Widget _priceText;

		private MilMo_Widget _discountTag;

		private MilMo_Widget _icon;

		private readonly int _price;

		public MilMo_CurrencyWidget(MilMo_UserInterface ui, string currencyTexture, int price)
			: base(ui)
		{
			_price = price;
			AllowPointerFocus = false;
			FadeToDefaultColor = false;
			Initialize(currencyTexture);
		}

		private void Initialize(string currencyTexture)
		{
			SetScale(64f, 64f);
			SetFadeSpeed(0.08f);
			_icon = new MilMo_Widget(UI);
			_icon.SetTexture(currencyTexture);
			_icon.SetAlignment(MilMo_GUI.Align.TopCenter);
			_icon.SetFadeSpeed(0.08f);
			_icon.FadeToDefaultColor = false;
			_icon.FadeToDefaultTextColor = false;
			_icon.AllowPointerFocus = false;
			AddChild(_icon);
			_discountTag = new MilMo_Widget(UI);
			_discountTag.SetDefaultTextColor255(139f, 81f, 33f, 255f);
			_discountTag.SetTexture("Batch01/Textures/Shop/SaleTag");
			_discountTag.SetFont(MilMo_GUI.Font.EborgSmall);
			_discountTag.SetFadeSpeed(0.08f);
			_discountTag.SetAlpha(1f);
			_discountTag.FadeToDefaultColor = false;
			_discountTag.FadeToDefaultTextColor = false;
			_discountTag.AllowPointerFocus = false;
			_discountTag.SetFontScale(0.85f);
			_discountTag.TextOffset = new Vector2(-2f, -2f);
			_discountTag.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
			_priceText = new MilMo_Widget(UI);
			_priceText.SetAlignment(MilMo_GUI.Align.BottomCenter);
			_priceText.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
			_priceText.SetFont(MilMo_GUI.Font.EborgSmall);
			_priceText.SetFontPreset(MilMo_GUI.FontPreset.DropShadow);
			_priceText.FadeToDefaultColor = false;
			_priceText.FadeToDefaultTextColor = false;
			_priceText.AllowPointerFocus = false;
			_priceText.SetFadeSpeed(0.08f);
			_priceText.SetTextDropShadowPos(3f, 3f);
			_priceText.TextDropShadowColor = new Color(0f, 0f, 0f, 0.4f);
			_priceText.SetDefaultTextColor(1f, 0.92f, 0f, 1f);
			if (_price > 0)
			{
				_priceText.SetTextNoLocalization(_price.ToString());
			}
			else
			{
				_icon.SetDefaultColor(1f, 1f, 1f, 0.4f);
			}
			AddChild(_priceText);
			RefreshUI();
		}

		public void SetDiscount(MilMo_ShopItem.DiscountData discountData)
		{
			if (discountData.IsActive() && _price > 0)
			{
				_discountTag.SetTextNoLocalization("-" + discountData.Discount + "%");
				AddChild(_discountTag);
				RefreshUI();
			}
		}

		public void RefreshUI()
		{
			_icon.SetScale(40f, 40f);
			_icon.SetPosition(Scale.x * 0.5f, 0f);
			_discountTag.SetScale(64f, 32f);
			_discountTag.SetPosition(Scale.x - 10f, Scale.y * 0.5f - 15f);
			BringToFront(_discountTag);
			_priceText.SetScale(Scale.x, 32f);
			_priceText.SetPosition(Scale.x * 0.5f, Scale.y);
		}
	}

	private enum ETypes
	{
		Coin,
		Cash,
		Gift,
		NrOfTypes
	}

	private readonly MilMo_CurrencyWidget[] _currencyWidgets = new MilMo_CurrencyWidget[3];

	private readonly MilMo_ShopItem _item;

	public MilMo_PriceWidget(MilMo_UserInterface ui, MilMo_ShopItem item)
		: base(ui)
	{
		_item = item;
		SetTextureWhite();
		FadeToDefaultTextColor = false;
		AllowPointerFocus = false;
		UseParentAlpha = false;
		SetFadeSpeed(0.08f);
		FadeToDefaultColor = false;
		SetAlignment(MilMo_GUI.Align.TopLeft);
		Initialize();
	}

	private void Initialize()
	{
		_currencyWidgets[1] = new MilMo_CurrencyWidget(UI, "Batch01/Textures/Shop/JuneCash128", _item.GetPrice(MilMo_Player.Instance.IsMember, isGift: false));
		_currencyWidgets[0] = new MilMo_CurrencyWidget(UI, "Batch01/Textures/HUD/IconVoucherPoint", _item.CanBuyWithCoins ? _item.GetCoinPrice() : 0);
		_currencyWidgets[2] = new MilMo_CurrencyWidget(UI, "Batch01/Textures/Shop/IconGift", _item.GetPrice(MilMo_Player.Instance.IsMember, isGift: true));
		_currencyWidgets[1].SetAlignment(MilMo_GUI.Align.BottomLeft);
		_currencyWidgets[2].SetAlignment(MilMo_GUI.Align.BottomCenter);
		_currencyWidgets[0].SetAlignment(MilMo_GUI.Align.BottomRight);
		AddChild(_currencyWidgets[2]);
		AddChild(_currencyWidgets[0]);
		AddChild(_currencyWidgets[1]);
		RefreshUI();
	}

	public void SetDiscount(MilMo_ShopItem.DiscountData coin, MilMo_ShopItem.DiscountData cash, MilMo_ShopItem.DiscountData gift)
	{
		_currencyWidgets[1].SetDiscount(cash);
		_currencyWidgets[0].SetDiscount(coin);
		_currencyWidgets[2].SetDiscount(gift);
	}

	public void RefreshUI()
	{
		_currencyWidgets[1].SetPosition(11f, Scale.y);
		_currencyWidgets[2].SetPosition(Scale.x * 0.5f, Scale.y);
		_currencyWidgets[0].SetPosition(Scale.x - 16f, Scale.y);
		_currencyWidgets[1].RefreshUI();
		_currencyWidgets[0].RefreshUI();
		_currencyWidgets[2].RefreshUI();
	}
}
