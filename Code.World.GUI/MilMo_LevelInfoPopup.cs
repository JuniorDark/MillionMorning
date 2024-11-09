using System;
using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.World.Player;

namespace Code.World.GUI;

public class MilMo_LevelInfoPopup : MilMo_ChatRoomInfoPopup
{
	public readonly MilMo_Button TelepodToButton;

	private MilMo_TimerEvent _openTimer;

	private MilMo_TimerEvent _closeTimer;

	private readonly bool _isGUIOpen;

	private readonly MilMo_Widget _icon;

	public bool ShouldOpen;

	private readonly MilMo_RequiredLevelTag _requiredLevelTag;

	public MilMo_LevelInfoPopup(MilMo_UserInterface ui, MilMo_LocString caption, MilMo_LocString message, float width, string iconTexture, bool isOpen, List<bool> explorationTokens, List<bool> coinTokens, PremiumToken premiumToken, bool isMembersOnly, int requiredLevel)
		: base(ui, caption, message, width, explorationTokens, coinTokens, premiumToken)
	{
		_isGUIOpen = isOpen;
		Identifier = "LevelInfoPopup";
		base.FixedRes = true;
		AllowPointerFocus = true;
		_icon = new MilMo_Widget(UI);
		_icon.SetScale(60f, 60f);
		_icon.SetPosition(10f, 5f);
		_icon.SetTexture(iconTexture, prefixStandardGuiPath: false);
		_icon.FixedRes = true;
		_icon.UseParentAlpha = false;
		_icon.SetAlignment(MilMo_GUI.Align.TopLeft);
		AddChild(_icon);
		if (isMembersOnly)
		{
			MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
			milMo_Widget.SetTexture("Batch01/Textures/HUD/TagPremium");
			milMo_Widget.AllowPointerFocus = false;
			milMo_Widget.SetScale(20f, 20f);
			milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
			milMo_Widget.SetPosition(45f, 40f);
			milMo_Widget.Enabled = true;
			milMo_Widget.UseParentAlpha = false;
			AddChild(milMo_Widget);
		}
		if (MilMo_Player.Instance.AvatarLevel < requiredLevel)
		{
			_requiredLevelTag = new MilMo_RequiredLevelTag(UI, requiredLevel);
			_requiredLevelTag.SetAlignment(MilMo_GUI.Align.TopLeft);
			_requiredLevelTag.SetPosition(-3f, -3f);
			_requiredLevelTag.FixedRes = true;
			AddChild(_requiredLevelTag);
		}
		CaptPos.x += 70f;
		CaptPos.y += 10f;
		TokenStartPos.x += 70f;
		TokenStartPos.y += 10f;
		TextPos.y += 20f;
		TextPos.x += 10f;
		ExtraY = 20f;
		InsideWidget.AllowPointerFocus = true;
		TelepodToButton = new MilMo_Button(UI);
		AddChild(TelepodToButton);
		TelepodToButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		TelepodToButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		TelepodToButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		TelepodToButton.SetText(MilMo_Localization.GetLocString("World_466"));
		TelepodToButton.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		TelepodToButton.SetFont(MilMo_GUI.Font.EborgSmall);
		TelepodToButton.SetAlignment(MilMo_GUI.Align.TopRight);
		TelepodToButton.UseParentAlpha = false;
		TelepodToButton.SetFadeSpeed(FadeSpeed);
		TelepodToButton.FixedRes = true;
		TelepodToButton.AllowPointerFocus = true;
		TelepodToButton.Enabled = false;
		TelepodToButton.SetScale(120f, 25f);
		TelepodToButton.SetPosition(InsideWidget.Scale.x - 15f, InsideWidget.Scale.y - 30f);
		TelepodToButton.SetFontScale(0.8f);
		TelepodToButton.IgnoreInputOffsetMode = true;
		Close();
	}

	private void Open(bool isCurrentLevel)
	{
		foreach (MilMo_Widget child in base.Children)
		{
			child.SetTextColor(1f, 1f, 1f, 0f);
			child.SetColor(1f, 1f, 1f, 0f);
		}
		Enabled = true;
		for (int i = 0; i < 8; i++)
		{
			Frame[i].Enabled = true;
		}
		float num = 0f;
		if (PremiumToken != null)
		{
			if (MilMo_Player.Instance.IsMember)
			{
				if (PremiumToken.GetProgress() >= 1f)
				{
					PremiumTokenIcon.SetStateActive(PremiumToken.GetValue());
					num = 25f;
				}
				else
				{
					PremiumTokenIcon.SetStateInactive(PremiumToken.GetProgress());
					num = 25f;
				}
			}
			else if (PremiumToken.GetValue() > 15)
			{
				PremiumTokenIcon.SetStateActiveNoneMember(PremiumToken.GetValue());
				num = 25f;
			}
		}
		_requiredLevelTag?.SetEnabled(MilMo_Player.Instance.AvatarLevel < _requiredLevelTag.RequiredLevel);
		if (!isCurrentLevel && _isGUIOpen)
		{
			num = 25f;
			TelepodToButton.Enabled = true;
			TelepodToButton.SetAlpha(0f);
			TelepodToButton.AlphaTo(1f);
			AllowPointerFocus = true;
			foreach (MilMo_Widget child2 in base.Children)
			{
				child2.AllowPointerFocus = true;
			}
		}
		else
		{
			TelepodToButton.Enabled = false;
			AllowPointerFocus = false;
			foreach (MilMo_Widget child3 in base.Children)
			{
				child3.AllowPointerFocus = false;
			}
		}
		UpdateScaleAndPosition(num);
		if ((double)Math.Abs(num - 25f) <= 0.0)
		{
			PremiumTokenIcon.SetPosition(5f, InsideWidget.Scale.y - 30f);
		}
		UI.BringToFront(this);
	}

	public void MoveWhenOutside(float x, float y)
	{
		SetPosition(x, y);
	}

	protected override void UpdateScaleAndPosition(float addY)
	{
		_icon.SetScale(60f, 60f);
		_icon.SetPosition(0f, 0f);
		base.UpdateScaleAndPosition(addY);
		TelepodToButton.SetScale(120f, 25f);
		TelepodToButton.SetPosition(InsideWidget.Scale.x - 15f, InsideWidget.Scale.y - 30f);
		TelepodToButton.SetFontScale(0.8f);
	}

	public void ShowLevelInfoPopup(bool isCurrentLevel)
	{
		ShouldOpen = true;
		MilMo_EventSystem.At(0.5f, delegate
		{
			if (ShouldOpen)
			{
				Open(isCurrentLevel);
			}
		});
	}

	public void HideLevelInfoPopup()
	{
		TelepodToButton.Enabled = false;
		Close();
	}
}
