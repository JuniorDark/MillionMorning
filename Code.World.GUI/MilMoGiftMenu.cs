using System.Collections.Generic;
using System.Linq;
using Code.Core.BuddyBackend;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.GUI.Widget.Button;
using Code.Core.GUI.Widget.SimpleWindow;
using Code.Core.Network.nexus;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using Core;
using UnityEngine;

namespace Code.World.GUI;

public sealed class MilMoGiftMenu : MilMo_Window
{
	private sealed class FriendListItem : MilMo_Button
	{
		public readonly int MId;

		private readonly MilMo_Widget _mIcon;

		private bool _mIsCurrent;

		public readonly MilMo_CheckBox MCheckbox;

		public Friend Buddy;

		public bool IsCurrent
		{
			get
			{
				return _mIsCurrent;
			}
			set
			{
				_mIsCurrent = value;
				if (!_mIsCurrent)
				{
					SetTextColor(1f, 1f, 1f, 1f);
				}
			}
		}

		public FriendListItem(MilMo_UserInterface ui, MilMo_LocString displayName, int id)
			: base(ui)
		{
			SetText(displayName);
			AllowPointerFocus = true;
			SetFont(MilMo_GUI.Font.EborgSmall);
			SetFontScale(0.8f);
			SetTextAlignment(MilMo_GUI.Align.CenterLeft);
			SetAlignment(MilMo_GUI.Align.TopLeft);
			SetScale(190f, 25f);
			SetHoverTextColor(1f, 1f, 0.4f, 1f);
			MId = id;
			_mIcon = new MilMo_Widget(ui);
			_mIcon.SetScale(28f, 28f);
			_mIcon.SetAlignment(MilMo_GUI.Align.CenterLeft);
			_mIcon.UseParentAlpha = false;
			_mIcon.SetTexture("Batch01/Textures/Shop/IconGift");
			_mIcon.SetYPos(15f);
			_mIcon.AllowPointerFocus = false;
			_mIcon.SetColor(1f, 1f, 1f, 1f);
			_mIcon.FadeToDefaultColor = false;
			AddChild(_mIcon);
			MCheckbox = new MilMo_CheckBox(UI);
			MCheckbox.SetScale(25f, 25f);
			MCheckbox.Checked = false;
			MCheckbox.SetAlignment(MilMo_GUI.Align.CenterLeft);
			MCheckbox.SetYPos(15f);
			MCheckbox.UseParentAlpha = false;
			MCheckbox.AllowPointerFocus = true;
			AddChild(MCheckbox);
		}

		protected override void DrawText()
		{
			Pos.y -= 6f;
			if (_mIsCurrent)
			{
				Pos.x = 45f;
				MCheckbox.SetXPos(-25f);
				_mIcon.SetXPos(-33f);
				_mIcon.SetAlpha(1f);
				MCheckbox.SetAlpha(0f);
				SetTextColor(0.4f, 1f, 0.4f, 1f);
				SetFontScale(1f);
			}
			else
			{
				Pos.x = 40f;
				MCheckbox.SetXPos(-20f);
				_mIcon.SetXPos(-28f);
				_mIcon.SetAlpha(0f);
				MCheckbox.SetAlpha(0f);
				SetFontScale(0.8f);
			}
			base.DrawText();
		}
	}

	private readonly List<FriendListItem> _friendButtons;

	private MilMo_Button _setFriendButton;

	public readonly MilMo_Button SendGiftButton;

	private readonly MilMo_Button _cancelButton;

	private readonly MilMo_TextBlock _friendHelp;

	private readonly Vector2 _windowSize = new Vector2(260f, 350f);

	private readonly Vector2 _tabOffset = new Vector2(0f, 0f);

	private MilMo_Tab _friendTab;

	public MilMoGiftMenu(MilMo_UserInterface ui)
		: base(ui)
	{
		base.FixedRes = true;
		Identifier = "GiftMenu";
		HasCloseButton = false;
		base.CloseButton.Function = delegate
		{
			Close(null);
		};
		SetScale(_windowSize);
		SetText(MilMo_Localization.GetLocString("World_5957"));
		MCaption.SetFont(MilMo_GUI.Font.EborgSmall);
		MCaption.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.SetPosition(0f, 36f);
		milMo_Widget.IsInvisible = true;
		milMo_Widget.Identifier = "dragBlocker";
		milMo_Widget.SetScale(_windowSize);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		AddChild(milMo_Widget);
		MilMo_Widget milMo_Widget2 = new MilMo_Widget(UI);
		milMo_Widget2.SetTexture("Batch01/Textures/Core/BlackTransparent");
		milMo_Widget2.Identifier = "blackBack";
		milMo_Widget2.SetDefaultColor(0f, 0f, 0f, 0.4f);
		milMo_Widget2.SetPosition(10f, 39f);
		milMo_Widget2.SetScale(_windowSize.x - 20f, _windowSize.y - 90f);
		milMo_Widget2.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget2.AllowPointerFocus = false;
		AddChild(milMo_Widget2);
		_friendButtons = new List<FriendListItem>();
		SendGiftButton = new MilMo_Button(UI);
		SendGiftButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		SendGiftButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		SendGiftButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		SendGiftButton.SetFont(MilMo_GUI.Font.EborgSmall);
		SendGiftButton.SetFontScale(0.8f);
		SendGiftButton.SetText(MilMo_Localization.GetLocString("World_5958"));
		SendGiftButton.SetAlignment(MilMo_GUI.Align.BottomLeft);
		SendGiftButton.SetScale(125f, 30f);
		SendGiftButton.SetPosition(15f, _windowSize.y - 15f);
		SendGiftButton.UseParentAlpha = false;
		SendGiftButton.FadeToDefaultColor = false;
		SendGiftButton.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		AddChild(SendGiftButton);
		_cancelButton = new MilMo_Button(UI);
		_cancelButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		_cancelButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		_cancelButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		_cancelButton.SetFont(MilMo_GUI.Font.EborgSmall);
		_cancelButton.SetFontScale(0.8f);
		_cancelButton.SetText(MilMo_Localization.GetLocString("Generic_Cancel"));
		_cancelButton.SetAlignment(MilMo_GUI.Align.BottomRight);
		_cancelButton.SetScale(90f, 30f);
		_cancelButton.SetPosition(_windowSize.x - 15f, _windowSize.y - 15f);
		_cancelButton.UseParentAlpha = false;
		_cancelButton.FadeToDefaultColor = false;
		_cancelButton.Function = Close;
		_cancelButton.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		AddChild(_cancelButton);
		_friendHelp = new MilMo_TextBlock(UI, MilMo_Localization.GetLocString("World_5959"), new Vector2(168f, 10f));
		_friendHelp.TextWidget.SetTextAlignment(MilMo_GUI.Align.TopCenter);
		_friendHelp.SetEnabled(e: false);
		_friendHelp.FadeToDefaultColor = false;
		AddChild(_friendHelp);
		InitTab();
	}

	public override void Draw()
	{
		_friendHelp.TextWidget.SetYScale(200f);
		_friendHelp.SetYScale(200f);
		UnityEngine.GUI.skin = UI.Font0;
		base.Draw();
	}

	private void InitTab()
	{
		AddPropertyPage();
		if (base.PropertyPage != null)
		{
			base.PropertyPage.ScaleTweak = new Vector2(0f, 53f);
			SetBackgroundTexture("Batch01/Textures/Core/Invisible");
			SetDividerTexture("Batch01/Textures/Core/Invisible");
			SetActiveTabTexture("Batch01/Textures/Core/Invisible");
			SetInactiveTabTexture("Batch01/Textures/Core/Invisible");
			base.PropertyPage.TabScroller.SetAlpha(0f);
			base.PropertyPage.SetPosition(0f, 0f);
			base.PropertyPage.LeftArrow.Enabled = false;
			base.PropertyPage.RightArrow.Enabled = false;
			base.PropertyPage.TabSize = new Vector2(0f, 0f);
			SetTabOffset(_tabOffset.x, _tabOffset.y);
			base.PropertyPage.CurrentTab = 0;
			base.PropertyPage.CoverFlowMargin = 0;
			base.PropertyPage.ScaleParent = true;
			base.PropertyPage.TabNonSelectedTextColor = new Color(0.7f, 0.7f, 0.9f, 0.75f);
			base.PropertyPage.TabNonSelectedMouseOverTextColor = new Color(1f, 1f, 1f, 1f);
			_friendTab = CreateTab(MilMo_Localization.GetLocString("World_5960"));
			_friendTab.Identifier = "Friends";
			_friendTab.WindowSize = _windowSize;
			_friendTab.SetTextOutline(1f, 1f);
			_friendTab.TextOutlineColor = new Color(0f, 0f, 0f, 1f);
			_friendTab.MForceScrollViewScale = true;
			_friendTab.ScrollView.SetScale(_windowSize.x - 10f, _windowSize.y - 95f);
			_friendTab.ScrollView.SetAlignment(MilMo_GUI.Align.TopLeft);
			_friendTab.ScrollView.SetPosition(0f, 42f);
			UpdateTabs();
		}
		else
		{
			MilMo_Window.Warning(0);
		}
	}

	private void FetchFriends()
	{
		_friendButtons.Clear();
		int num = 0;
		if (Singleton<MilMo_BuddyBackend>.Instance.GetBuddies() != null)
		{
			foreach (Friend buddy in Singleton<MilMo_BuddyBackend>.Instance.GetBuddies())
			{
				num++;
				AddFriendButton(buddy);
			}
		}
		_friendHelp.SetEnabled(e: false);
		SendGiftButton.Enabled = true;
		if (num == 0)
		{
			_friendHelp.SetEnabled(e: true);
			_friendHelp.SetAlpha(0f);
			MilMo_EventSystem.At(2f, delegate
			{
				_friendHelp.AlphaTo(1f);
			});
			_cancelButton.SetText(MilMo_Localization.GetLocString("Generic_Close"));
			SendGiftButton.Enabled = false;
		}
	}

	private void AddFriendButton(Friend friend)
	{
		FriendListItem button = new FriendListItem(UI, MilMo_Localization.GetNotLocalizedLocString(friend.Name), _friendButtons.Count);
		button.Buddy = friend;
		button.MCheckbox.Function = delegate
		{
			MilMo_GuiSoundManager.Instance.PlaySoundFx(MilMo_SoundType.Select);
			foreach (FriendListItem item in _friendButtons.Where((FriendListItem otherbutton) => otherbutton.MId != button.MId))
			{
				item.IsCurrent = false;
				item.MCheckbox.Checked = false;
			}
			button.MCheckbox.Checked = true;
			button.IsCurrent = true;
		};
		button.Function = button.MCheckbox.Function;
		_friendButtons.Add(button);
	}

	public Friend GetFriend()
	{
		Friend result = null;
		foreach (FriendListItem item in _friendButtons.Where((FriendListItem b) => b.IsCurrent))
		{
			result = item.Buddy;
		}
		return result;
	}

	public override void Open()
	{
		SpawnPos = new Vector2((float)Screen.width / 2f - _windowSize.x / 2f, 163f);
		TargetPos = SpawnPos;
		FetchFriends();
		BringToFront();
		base.Open();
		SetPosition(SpawnPos);
	}

	public override void Refresh()
	{
		if (!HasPropertyPage || base.PropertyPage == null)
		{
			MilMo_Window.Warning(0);
			return;
		}
		if (base.PropertyPage != null)
		{
			foreach (MilMo_Tab item in base.PropertyPage.TabScroller.Children.Cast<MilMo_Tab>())
			{
				item.ScrollView.RemoveAllChildren();
				UI.ResetLayout(5f, 10f, item.ScrollView);
				UI.Next.x += 5f;
				item.NextPos = UI.Next;
				item.SamePos = UI.Same;
			}
		}
		foreach (FriendListItem friendButton in _friendButtons)
		{
			AddWidget(friendButton, "Friends");
			BringToFront(friendButton);
		}
		_friendTab.ScrollView.RefreshViewSize(0f, 50f);
		BringToFront(base.FadeBottom);
		BringToFront(base.FadeTop);
		SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
	}
}
