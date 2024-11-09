using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.SimpleWindow;
using Code.Core.Network;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.Portal.Steam;
using Code.Core.ResourceSystem;
using Core;
using UnityEngine;

namespace Code.World.GUI.Steam;

public sealed class SteamJuneCashDialog : MilMo_Window
{
	private MilMo_Button _mBuyButton;

	private MilMo_Button _mCloseButton;

	private MilMo_TextBlock _mBuyText;

	private MilMo_TextBlock _mCostText;

	private MilMo_ComboBox _mJuneCashBox;

	private MilMo_Widget _mJuneCashImage;

	private readonly Vector2 _mWindowSize = new Vector2(260f, 300f);

	private IList<JuneCashItem> _juneCashItems;

	public SteamJuneCashDialog(MilMo_UserInterface ui)
		: base(ui)
	{
		base.FixedRes = true;
		Identifier = "Buy June Cash";
		HasCloseButton = false;
		base.CloseButton.Function = delegate
		{
			Close(null);
		};
		SetScale(_mWindowSize);
		SpawnScale = _mWindowSize;
		TargetScale = _mWindowSize;
		ExitScale = _mWindowSize;
		SetText(MilMo_Localization.GetLocString("Steam_0002"));
		MCaption.SetFont(MilMo_GUI.Font.EborgSmall);
		MCaption.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		CreateDragBlocker();
		CreateBackdrop();
		CreateBuyButton();
		CreateCloseButton();
		CreateBuyText();
		CreateJuneCashImage();
		CreateCostText();
		RequestJuneCashItems();
	}

	public override void Draw()
	{
		_mBuyText.TextWidget.SetYScale(200f);
		_mBuyText.SetYScale(200f);
		UnityEngine.GUI.skin = UI.Font0;
		base.Draw();
	}

	public override void Open()
	{
		SpawnPos = new Vector2((float)Screen.width / 2f - _mWindowSize.x / 2f, 163f);
		TargetPos = SpawnPos;
		BringToFront();
		base.Open();
		SetPosition(SpawnPos);
	}

	public override void Refresh()
	{
		BringToFront(base.FadeBottom);
		BringToFront(base.FadeTop);
		SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
	}

	private void CreateDragBlocker()
	{
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.Identifier = "dragBlocker";
		milMo_Widget.IsInvisible = true;
		milMo_Widget.SetPosition(0f, 36f);
		milMo_Widget.SetScale(_mWindowSize);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		AddChild(milMo_Widget);
	}

	private void CreateBackdrop()
	{
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.SetTexture("Batch01/Textures/Core/BlackTransparent");
		milMo_Widget.Identifier = "blackBack";
		milMo_Widget.SetDefaultColor(0f, 0f, 0f, 0.4f);
		milMo_Widget.SetPosition(20f, 39f);
		milMo_Widget.SetScale(_mWindowSize.x - 20f, _mWindowSize.y - 90f);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.AllowPointerFocus = false;
		AddChild(milMo_Widget);
	}

	private void CreateJuneCashImage()
	{
		_mJuneCashImage = new MilMo_Widget(UI);
		_mJuneCashImage.SetPosition(_mWindowSize.x / 2f - 10f, 210f);
		_mJuneCashImage.SetScale(64f, 64f);
		AddChild(_mJuneCashImage);
	}

	private void CreateJuneCashBox()
	{
		_mJuneCashBox = new MilMo_ComboBox(UI, MilMo_ComboBox.ComboDropDirection.Down, MilMo_LocString.Empty, 180f, 220f, Color.black, Color.white);
		_mJuneCashBox.BoxTextureWidget.SetAllTextures("Batch01/Textures/Quest/QuestLogSelectionButton");
		_mJuneCashBox.BoxTextureWidget.SetHoverTexture("Batch01/Textures/Quest/QuestLogSelectionButtonMO");
		_mJuneCashBox.Background.SetTexture("Batch01/Textures/Quest/QuestLogBackground");
		_mJuneCashBox.Top.SetTexture("Batch01/Textures/Quest/QuestLogBackgroundTop");
		_mJuneCashBox.Top.SetYScale(5f);
		_mJuneCashBox.BackgroundColor = new Color(0f, 0f, 0.12f, 0.9f);
		_mJuneCashBox.ItemTextColor = Color.white;
		_mJuneCashBox.BoxTextureWidget.SetFont(MilMo_GUI.Font.EborgSmall);
		_mJuneCashBox.BoxTextureWidget.SetFontScale(0.7f);
		_mJuneCashBox.BoxTextureWidget.SetFontPreset(MilMo_GUI.FontPreset.DropShadow);
		_mJuneCashBox.SelectedItemTextColor = new Color(0.7f, 0.7f, 0.7f, 1f);
		_mJuneCashBox.SetPosition(45f, 150f);
		_mJuneCashBox.UseIcon = false;
		_mJuneCashBox.IndexChangedFunction = delegate
		{
			if (_juneCashItems != null && _juneCashItems.Count >= 1)
			{
				int index = Mathf.Clamp(_mJuneCashBox.SelectedIndex, 0, _juneCashItems.Count - 1);
				JuneCashItem juneCashItem = _juneCashItems[index];
				if (juneCashItem != null)
				{
					_mJuneCashImage.SetTexture(juneCashItem.GetTexture());
					_mCostText.SetText(MilMo_Localization.GetNotLocalizedLocString(juneCashItem.GetPrice() + " USD"));
				}
			}
		};
		foreach (JuneCashItem juneCashItem2 in _juneCashItems)
		{
			string title = juneCashItem2.GetTitle();
			_mJuneCashBox.AddItem(MilMo_Localization.GetNotLocalizedLocString(title));
		}
		_mJuneCashBox.SelectItem(0);
		AddChild(_mJuneCashBox);
	}

	private void CreateBuyButton()
	{
		_mBuyButton = new MilMo_Button(UI);
		_mBuyButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		_mBuyButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		_mBuyButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		_mBuyButton.SetFont(MilMo_GUI.Font.EborgSmall);
		_mBuyButton.SetFontScale(0.8f);
		_mBuyButton.SetText(MilMo_Localization.GetLocString("InGameShop_377"));
		_mBuyButton.SetAlignment(MilMo_GUI.Align.BottomLeft);
		_mBuyButton.SetScale(125f, 30f);
		_mBuyButton.SetPosition(15f, _mWindowSize.y - 15f);
		_mBuyButton.UseParentAlpha = false;
		_mBuyButton.FadeToDefaultColor = false;
		_mBuyButton.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		_mBuyButton.Function = BuyJuneCash;
		AddChild(_mBuyButton);
	}

	private void BuyJuneCash(object obj)
	{
		MilMo_SteamPortal.InitiateMicroTransaction(_juneCashItems[_mJuneCashBox.SelectedIndex], delegate
		{
			Close(null);
		});
	}

	private void CreateBuyText()
	{
		_mBuyText = new MilMo_TextBlock(UI, MilMo_Localization.GetLocString("Steam_0003"), new Vector2(168f, 10f));
		_mBuyText.TextWidget.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		_mBuyText.FadeToDefaultColor = false;
		AddChild(_mBuyText);
	}

	private void CreateCostText()
	{
		_mCostText = new MilMo_TextBlock(UI, MilMo_Localization.GetNotLocalizedLocString(""), new Vector2(168f, 10f));
		_mCostText.SetPosition(_mWindowSize.x / 2f - 35f, 190f);
		_mCostText.SetFont(MilMo_GUI.Font.GothamMedium);
		_mCostText.FadeToDefaultColor = false;
		AddChild(_mCostText);
	}

	private void CreateCloseButton()
	{
		_mCloseButton = new MilMo_Button(UI);
		_mCloseButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		_mCloseButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		_mCloseButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		_mCloseButton.SetFont(MilMo_GUI.Font.EborgSmall);
		_mCloseButton.SetFontScale(0.8f);
		_mCloseButton.SetText(MilMo_Localization.GetLocString("Generic_Close"));
		_mCloseButton.SetAlignment(MilMo_GUI.Align.BottomRight);
		_mCloseButton.SetScale(90f, 30f);
		_mCloseButton.SetPosition(_mWindowSize.x - 15f, _mWindowSize.y - 15f);
		_mCloseButton.UseParentAlpha = false;
		_mCloseButton.FadeToDefaultColor = false;
		_mCloseButton.Function = Close;
		_mCloseButton.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		AddChild(_mCloseButton);
	}

	private void RequestJuneCashItems()
	{
		MilMo_EventSystem.Listen("junecash_items", ReceivedJuneCashItems);
		Singleton<GameNetwork>.Instance.RequestJuneCashItems();
	}

	private void ReceivedJuneCashItems(object msg)
	{
		if (msg is ServerJuneCashItems serverJuneCashItems)
		{
			_juneCashItems = serverJuneCashItems.getJuneCashItems();
		}
		CreateJuneCashBox();
	}
}
