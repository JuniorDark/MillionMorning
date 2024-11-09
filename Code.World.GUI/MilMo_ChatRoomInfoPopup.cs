using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Utility;
using Code.World.Player;
using UnityEngine;

namespace Code.World.GUI;

public class MilMo_ChatRoomInfoPopup : MilMo_Widget
{
	protected class MilMo_PremiumTokenIcon : MilMo_Widget
	{
		private readonly MilMo_Widget _activeIcon;

		private readonly MilMo_Widget _inactiveIcon;

		private readonly MilMo_SimpleLabel _valueText;

		private readonly MilMo_ProgressBar _inactiveProgress;

		private readonly Vector2 _activeIconScale;

		private readonly Vector2 _inactiveIconScale;

		private readonly Vector2 _valueTextScale;

		private readonly Vector2 _myScale;

		public MilMo_PremiumTokenIcon(MilMo_UserInterface ui)
			: base(ui)
		{
			SetTextureInvisible();
			SetAlignment(MilMo_GUI.Align.TopLeft);
			_myScale = new Vector2(50f, 50f);
			_activeIconScale = new Vector2(20f, 20f);
			_inactiveIconScale = new Vector2(20f, 20f);
			_valueTextScale = new Vector2(20f, 20f);
			_activeIcon = new MilMo_Widget(ui)
			{
				FixedRes = true
			};
			_activeIcon.SetAlignment(MilMo_GUI.Align.CenterCenter);
			AddChild(_activeIcon);
			_inactiveIcon = new MilMo_Widget(ui);
			_inactiveIcon.SetTexture("Batch01/Textures/HUD/IconPremiumSmallInactive");
			_inactiveIcon.SetDefaultColor(1f, 1f, 1f, 0.8f);
			_inactiveIcon.FixedRes = true;
			_inactiveIcon.SetAlignment(MilMo_GUI.Align.TopLeft);
			AddChild(_inactiveIcon);
			_valueText = new MilMo_SimpleLabel(ui)
			{
				FixedRes = true
			};
			_valueText.SetTextAlignment(MilMo_GUI.Align.CenterRight);
			_valueText.SetTextOffset(3f, 0f);
			_valueText.SetDefaultTextColor(1f, 1f, 1f, 1f);
			_valueText.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
			_valueText.SetFont(MilMo_GUI.Font.EborgSmall);
			_valueText.SetTexture("Batch01/Textures/HUD/IconVoucherPoint");
			_valueText.SetEnabled(e: false);
			AddChild(_valueText);
			_inactiveProgress = new MilMo_ProgressBar(ui, new Vector2(2f, 26f), 25f, 3f, 1f, new Color(0.56f, 0.56f, 0.85f, 0.8f), new Color(1f, 0.8f, 0.08f, 0.8f), 0f);
			_inactiveProgress.SetAlignment(MilMo_GUI.Align.TopCenter);
			_inactiveProgress.FixedRes = true;
			AddChild(_inactiveProgress);
			SetScale();
		}

		private void SetScale()
		{
			SetScale(_myScale);
			_activeIcon.SetScale(_activeIconScale);
			_inactiveIcon.SetScale(_inactiveIconScale);
			_valueText.SetScale(_valueTextScale);
			_valueText.SetPosition(22f, 3f);
			_inactiveIcon.SetPosition(4f, 0f);
			_activeIcon.SetPosition(10f, 10f);
			_inactiveProgress.SetPosition(_inactiveIcon.Pos.x + _inactiveIcon.Scale.x * 0.5f, 26f);
			_inactiveProgress.SetScale(27f, 5f);
		}

		public void SetStateActiveNoneMember(int value)
		{
			SetScale();
			Enabled = true;
			foreach (MilMo_Widget child in base.Children)
			{
				child.Enabled = false;
			}
			_inactiveIcon.Enabled = true;
			_valueText.Enabled = false;
			_valueText.SetText((value > 1) ? MilMo_Localization.GetNotLocalizedLocString(value.ToString()) : MilMo_LocString.Empty);
		}

		public void SetStateActive(int value)
		{
			SetScale();
			Enabled = true;
			foreach (MilMo_Widget child in base.Children)
			{
				child.Enabled = false;
			}
			_activeIcon.Enabled = true;
			string filename = (Mathf.Clamp(value - 15, 0, int.MaxValue) / 5) switch
			{
				0 => "Content/GUI/Batch01/Textures/HUD/IconPremiumBlue64", 
				1 => "Content/GUI/Batch01/Textures/HUD/IconPremiumGreen64", 
				2 => "Content/GUI/Batch01/Textures/HUD/IconPremiumYellow64", 
				_ => "Content/GUI/Batch01/Textures/HUD/IconPremiumRed64", 
			};
			_activeIcon.SetTexture(filename, prefixStandardGuiPath: false);
			_valueText.Enabled = false;
			_valueText.SetText((value > 1) ? MilMo_Localization.GetNotLocalizedLocString(value.ToString()) : MilMo_LocString.Empty);
			Pulsate();
		}

		public void SetStateInactive(float progress)
		{
			SetScale();
			Enabled = true;
			foreach (MilMo_Widget child in base.Children)
			{
				child.Enabled = false;
			}
			_inactiveProgress.CurrentProgress = progress;
			_inactiveProgress.Enabled = true;
			_inactiveIcon.Enabled = true;
		}

		private void Pulsate()
		{
			_activeIcon.ScaleMover.SinRate = new Vector2(4.5f, 4.5f);
			_activeIcon.ScaleMover.SinAmp = new Vector2(1.6f, 1.6f);
			_activeIcon.SetScaleType(MilMo_Mover.UpdateFunc.Sinus);
		}

		private void StopPulsate()
		{
			_activeIcon.SetMoveType(MilMo_Mover.UpdateFunc.Nothing);
		}

		public void Close()
		{
			StopPulsate();
			foreach (MilMo_Widget child in base.Children)
			{
				child.Enabled = false;
			}
			Enabled = false;
		}
	}

	protected enum FrameParts
	{
		Top,
		Bot,
		Left,
		Right,
		TopLeft,
		TopRight,
		BotLeft,
		BotRight,
		NrOfParts
	}

	public readonly MilMo_Widget InsideWidget;

	private readonly MilMo_Widget _captionWidget;

	private readonly MilMo_Widget _textWidget;

	private readonly MilMo_LocString _txt;

	protected new float FadeSpeed = 0.04f;

	protected readonly MilMo_PremiumTokenIcon PremiumTokenIcon;

	protected PremiumToken PremiumToken;

	public readonly List<MilMo_Widget> ExplorationTokens;

	protected float TextHeight;

	protected readonly float InsideWidth;

	protected float InsideHeight;

	protected Vector2 CaptPos;

	protected Vector2 TextPos;

	protected Vector2 TokenStartPos;

	protected float ExtraY;

	private readonly Vector2 _captScale;

	private readonly bool _haveTokens = true;

	protected MilMo_Widget[] Frame;

	public MilMo_ChatRoomInfoPopup(MilMo_UserInterface UI, MilMo_LocString caption, MilMo_LocString message, float width, List<bool> explorationTokens, List<bool> coinTokens, PremiumToken premiumToken)
		: base(UI)
	{
		_captScale = new Vector2(width - 80f, 40f);
		CaptPos = new Vector2(0f, 0f);
		TextPos = new Vector2(0f, 50f);
		TokenStartPos = new Vector2(0f, 30f);
		Identifier = "ChatroomInfoPopup";
		SetAlignment(MilMo_GUI.Align.TopLeft);
		AllowPointerFocus = false;
		base.FixedRes = true;
		PremiumToken = premiumToken;
		InsideWidget = new MilMo_Widget(UI);
		AddChild(InsideWidget);
		InsideWidget.SetTextureBlack();
		InsideWidget.SetDefaultColor(0f, 0f, 0f, 0.75f);
		InsideWidget.SetAlignment(MilMo_GUI.Align.TopLeft);
		InsideWidget.SetPosition(0f, 0f);
		ExplorationTokens = new List<MilMo_Widget>();
		Vector2 tokenStartPos = TokenStartPos;
		if (explorationTokens.All((bool explorationTokenFound) => explorationTokenFound))
		{
			foreach (bool coinToken in coinTokens)
			{
				MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
				milMo_Widget.UseParentAlpha = false;
				milMo_Widget.SetTexture(coinToken ? "Batch01/Textures/WorldMap/IconSilverTokenSmall" : "Batch01/Textures/WorldMap/IconSilverTokenSmallUnfoundWhite");
				milMo_Widget.SetScale(20f, 20f);
				milMo_Widget.SetPosition(tokenStartPos);
				milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
				milMo_Widget.FixedRes = true;
				ExplorationTokens.Add(milMo_Widget);
				AddChild(milMo_Widget);
				tokenStartPos.x += 22f;
			}
		}
		else
		{
			foreach (bool explorationToken in explorationTokens)
			{
				MilMo_Widget milMo_Widget2 = new MilMo_Widget(UI)
				{
					UseParentAlpha = false
				};
				milMo_Widget2.SetTexture(explorationToken ? "Batch01/Textures/WorldMap/IconExplorationTokenSmall" : "Batch01/Textures/WorldMap/IconExplorationTokenSmallUnfoundWhite");
				milMo_Widget2.SetScale(20f, 20f);
				milMo_Widget2.SetPosition(tokenStartPos);
				milMo_Widget2.SetAlignment(MilMo_GUI.Align.TopLeft);
				milMo_Widget2.FixedRes = true;
				ExplorationTokens.Add(milMo_Widget2);
				AddChild(milMo_Widget2);
				tokenStartPos.x += 22f;
			}
		}
		_txt = message;
		InsideWidth = width;
		Frame = new MilMo_Widget[8];
		if (explorationTokens.Count == 0 && coinTokens.Count == 0)
		{
			TextPos.y -= 20f;
			_haveTokens = false;
		}
		_captionWidget = new MilMo_Widget(UI);
		_captionWidget.SetScale(_captScale);
		_captionWidget.SetAlignment(MilMo_GUI.Align.TopLeft);
		_captionWidget.SetPosition(CaptPos);
		_captionWidget.UseParentAlpha = false;
		_captionWidget.FixedRes = true;
		_captionWidget.SetFont(MilMo_GUI.Font.EborgSmall);
		_captionWidget.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		_captionWidget.SetText(caption);
		_captionWidget.SetDefaultTextColor(1f, 1f, 1f, 1f);
		AddChild(_captionWidget);
		_textWidget = new MilMo_Widget(UI);
		_textWidget.SetPosition(TextPos);
		_textWidget.SetFont(MilMo_GUI.Font.ArialRounded);
		_textWidget.SetWordWrap(w: true);
		_textWidget.SetAlignment(MilMo_GUI.Align.TopLeft);
		_textWidget.SetText(_txt);
		_textWidget.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		_textWidget.UseParentAlpha = false;
		_textWidget.FixedRes = true;
		_textWidget.SetDefaultTextColor(1f, 1f, 1f, 1f);
		AddChild(_textWidget);
		for (int i = 0; i < 8; i++)
		{
			Frame[i] = new MilMo_Widget(UI);
			Frame[i].FixedRes = true;
			AddChild(Frame[i]);
		}
		Frame[4].SetScale(14f, 14f);
		Frame[5].SetScale(14f, 14f);
		Frame[6].SetScale(14f, 14f);
		Frame[7].SetScale(14f, 14f);
		Frame[4].SetAlignment(MilMo_GUI.Align.BottomRight);
		Frame[5].SetAlignment(MilMo_GUI.Align.BottomLeft);
		Frame[6].SetAlignment(MilMo_GUI.Align.TopRight);
		Frame[7].SetAlignment(MilMo_GUI.Align.TopLeft);
		Frame[0].SetAlignment(MilMo_GUI.Align.BottomCenter);
		Frame[2].SetAlignment(MilMo_GUI.Align.CenterRight);
		Frame[3].SetAlignment(MilMo_GUI.Align.CenterLeft);
		Frame[1].SetAlignment(MilMo_GUI.Align.TopCenter);
		Frame[4].SetTexture("Batch01/Textures/Generic/FrameWhiteInner_CornerTopLeftDark");
		Frame[5].SetTexture("Batch01/Textures/Generic/FrameWhiteInner_CornerTopRightDark");
		Frame[6].SetTexture("Batch01/Textures/Generic/FrameWhiteInner_CornerBottomLeftDark");
		Frame[7].SetTexture("Batch01/Textures/Generic/FrameWhiteInner_CornerBottomRightDark");
		Frame[0].SetTexture("Batch01/Textures/Generic/FrameWhiteLine_TopDark");
		Frame[3].SetTexture("Batch01/Textures/Generic/FrameWhiteLine_RightDark");
		Frame[2].SetTexture("Batch01/Textures/Generic/FrameWhiteLine_LeftDark");
		Frame[1].SetTexture("Batch01/Textures/Generic/FrameWhiteLine_BottomDark");
		PremiumTokenIcon = new MilMo_PremiumTokenIcon(UI);
		AddChild(PremiumTokenIcon);
		foreach (MilMo_Widget child in base.Children)
		{
			child.SetFadeSpeed(base.FadeSpeed);
		}
		InsideWidget.SetFadeSpeed(base.FadeSpeed * 0.75f);
		Close();
	}

	public virtual void UpdateScaleAndPosition()
	{
		UpdateScaleAndPosition(0f);
	}

	protected virtual void UpdateScaleAndPosition(float addY)
	{
		Vector2 tokenStartPos = TokenStartPos;
		foreach (MilMo_Widget explorationToken in ExplorationTokens)
		{
			explorationToken.SetScale(20f, 20f);
			explorationToken.SetPosition(tokenStartPos);
			tokenStartPos.x += 22f;
		}
		_captionWidget.SetScale(_captScale);
		TextHeight = MilMo_ItemButton.GetTextHeight(UI, _txt.String, InsideWidth);
		if (TextHeight < 55f)
		{
			TextHeight += 15f;
		}
		if (!_haveTokens)
		{
			InsideHeight = TextHeight + 25f + addY + ExtraY;
		}
		else
		{
			InsideHeight = TextHeight + 45f + addY + ExtraY;
		}
		InsideWidget.SetScale(InsideWidth, InsideHeight);
		Frame[0].SetScale(InsideWidth, 14f);
		Frame[1].SetScale(InsideWidth, 14f);
		Frame[2].SetScale(14f, InsideHeight);
		Frame[3].SetScale(14f, InsideHeight);
		SetFramePositions();
		_textWidget.SetScale(InsideWidth - 15f, TextHeight);
		_captionWidget.SetPosition(CaptPos);
		_textWidget.SetPosition(TextPos);
		SetScale(InsideWidth, InsideHeight);
	}

	protected void SetFramePositions()
	{
		Frame[0].SetPosition(InsideWidget.Scale.x * 0.5f, 0f);
		Frame[2].SetPosition(0f, InsideWidget.Scale.y * 0.5f);
		Frame[3].SetPosition(InsideWidget.Scale.x, InsideWidget.Scale.y * 0.5f);
		Frame[1].SetPosition(InsideWidget.Scale.x * 0.5f, InsideWidget.Scale.y);
		Frame[4].SetPosition(0f, 0f);
		Frame[5].SetPosition(InsideWidget.Scale.x, 0f);
		Frame[6].SetPosition(0f, InsideWidget.Scale.y);
		Frame[7].SetPosition(InsideWidget.Scale);
	}

	public void Open(Vector2 openPos)
	{
		foreach (MilMo_Widget child in base.Children)
		{
			child.SetTextColor(1f, 1f, 1f, 0f);
			child.SetColor(1f, 1f, 1f, 0f);
		}
		SetPosition(openPos);
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
					num = 20f;
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
				num = 20f;
			}
		}
		UpdateScaleAndPosition(num);
		if ((double)Math.Abs(num - 25f) <= 0.0)
		{
			PremiumTokenIcon.SetPosition(0f, InsideWidget.Scale.y - 30f);
		}
		else
		{
			PremiumTokenIcon.SetPosition(0f, InsideWidget.Scale.y - 25f);
		}
		UI.BringToFront(this);
	}

	public void Close()
	{
		Enabled = false;
		for (int i = 0; i < 8; i++)
		{
			Frame[i].Enabled = false;
		}
		PremiumTokenIcon.Close();
	}
}
