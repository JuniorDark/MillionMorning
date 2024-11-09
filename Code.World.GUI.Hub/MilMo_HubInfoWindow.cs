using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.Input;
using UnityEngine;

namespace Code.World.GUI.Hub;

public class MilMo_HubInfoWindow : MilMo_Widget
{
	private enum FrameParts
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

	private readonly MilMo_Widget _mInsideWidget;

	private const float M_FADE_SPEED = 0.04f;

	private MilMo_Button _mCloseButton;

	private readonly MilMo_Widget[] _mFrame;

	private readonly MilMo_Button _mBackgroundBlocker;

	public Vector2 FullScale => _mInsideWidget.Scale;

	public MilMo_HubInfoWindow(MilMo_UserInterface ui, Vector2 scale)
		: base(ui)
	{
		_mBackgroundBlocker = new MilMo_Button(UI);
		_mBackgroundBlocker.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mBackgroundBlocker.IgnoreNextClick = true;
		_mBackgroundBlocker.Function = delegate
		{
			Close();
		};
		Identifier = "HUBInfoWindow" + MilMo_UserInterface.GetRandomID();
		SetAlignment(MilMo_GUI.Align.TopLeft);
		base.FixedRes = true;
		_mInsideWidget = new MilMo_Widget(UI);
		_mInsideWidget.SetScale(scale);
		AddChild(_mInsideWidget);
		_mInsideWidget.SetTextureBlack();
		_mInsideWidget.SetDefaultColor(0f, 0f, 0f, 0.75f);
		_mInsideWidget.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mInsideWidget.SetPosition(0f, 0f);
		_mFrame = new MilMo_Widget[8];
		for (int i = 0; i < 8; i++)
		{
			_mFrame[i] = new MilMo_Widget(UI);
			_mFrame[i].FixedRes = true;
			AddChild(_mFrame[i]);
		}
		_mFrame[4].SetScale(14f, 14f);
		_mFrame[5].SetScale(14f, 14f);
		_mFrame[6].SetScale(14f, 14f);
		_mFrame[7].SetScale(14f, 14f);
		_mFrame[4].SetAlignment(MilMo_GUI.Align.BottomRight);
		_mFrame[5].SetAlignment(MilMo_GUI.Align.BottomLeft);
		_mFrame[6].SetAlignment(MilMo_GUI.Align.TopRight);
		_mFrame[7].SetAlignment(MilMo_GUI.Align.TopLeft);
		_mFrame[0].SetAlignment(MilMo_GUI.Align.BottomCenter);
		_mFrame[2].SetAlignment(MilMo_GUI.Align.CenterRight);
		_mFrame[3].SetAlignment(MilMo_GUI.Align.CenterLeft);
		_mFrame[1].SetAlignment(MilMo_GUI.Align.TopCenter);
		_mFrame[4].SetTexture("Batch01/Textures/Generic/FrameWhiteInner_CornerTopLeftDark");
		_mFrame[5].SetTexture("Batch01/Textures/Generic/FrameWhiteInner_CornerTopRightDark");
		_mFrame[6].SetTexture("Batch01/Textures/Generic/FrameWhiteInner_CornerBottomLeftDark");
		_mFrame[7].SetTexture("Batch01/Textures/Generic/FrameWhiteInner_CornerBottomRightDark");
		_mFrame[0].SetTexture("Batch01/Textures/Generic/FrameWhiteLine_TopDark");
		_mFrame[3].SetTexture("Batch01/Textures/Generic/FrameWhiteLine_RightDark");
		_mFrame[2].SetTexture("Batch01/Textures/Generic/FrameWhiteLine_LeftDark");
		_mFrame[1].SetTexture("Batch01/Textures/Generic/FrameWhiteLine_BottomDark");
		_mInsideWidget.SetFadeSpeed(0.03f);
		SetAllowMouseFocus(state: true);
		Close();
	}

	public void SetAllowMouseFocus(bool state)
	{
		if (state)
		{
			AllowPointerFocus = true;
			foreach (MilMo_Widget child in base.Children)
			{
				child.SetFadeSpeed(0.04f);
				child.AllowPointerFocus = true;
			}
			_mBackgroundBlocker.AllowPointerFocus = true;
			return;
		}
		AllowPointerFocus = false;
		foreach (MilMo_Widget child2 in base.Children)
		{
			child2.SetFadeSpeed(0.04f);
			child2.AllowPointerFocus = false;
		}
		_mBackgroundBlocker.AllowPointerFocus = false;
	}

	public void AddCloseButton()
	{
		_mCloseButton = new MilMo_Button(UI);
		_mCloseButton.SetScale(16f, 16f);
		_mCloseButton.SetAllTextures("Batch01/Textures/Core/Black");
		_mCloseButton.SetAlignment(MilMo_GUI.Align.TopRight);
		_mCloseButton.SetPosition(_mInsideWidget.Scale.x, -5f);
		_mCloseButton.SetTexture("Batch01/Textures/World/CloseButton");
		_mCloseButton.SetHoverTexture("Batch01/Textures/World/CloseButtonMO");
		_mCloseButton.SetPressedTexture("Batch01/Textures/World/CloseButton");
		_mCloseButton.Function = delegate
		{
			Close();
		};
		_mCloseButton.UseParentAlpha = false;
		AddChild(_mCloseButton);
	}

	protected virtual void UpdateScaleAndPosition()
	{
		float x = _mInsideWidget.Scale.x;
		float y = _mInsideWidget.Scale.y;
		_mFrame[0].SetScale(x, 14f);
		_mFrame[1].SetScale(x, 14f);
		_mFrame[2].SetScale(14f, y);
		_mFrame[3].SetScale(14f, y);
		SetFramePositions();
		SetScale(x, y);
	}

	protected void SetFramePositions()
	{
		_mFrame[0].SetPosition(_mInsideWidget.Scale.x * 0.5f, 0f);
		_mFrame[2].SetPosition(0f, _mInsideWidget.Scale.y * 0.5f);
		_mFrame[3].SetPosition(_mInsideWidget.Scale.x, _mInsideWidget.Scale.y * 0.5f);
		_mFrame[1].SetPosition(_mInsideWidget.Scale.x * 0.5f, _mInsideWidget.Scale.y);
		_mFrame[4].SetPosition(0f, 0f);
		_mFrame[5].SetPosition(_mInsideWidget.Scale.x, 0f);
		_mFrame[6].SetPosition(0f, _mInsideWidget.Scale.y);
		_mFrame[7].SetPosition(_mInsideWidget.Scale);
	}

	public virtual void Open(float x, float y)
	{
		SetPosition(x, y);
		Enabled = true;
		for (int i = 0; i < 8; i++)
		{
			_mFrame[i].Enabled = true;
		}
		UpdateScaleAndPosition();
		UI.AddChild(_mBackgroundBlocker);
		_mBackgroundBlocker.IgnoreNextClick = true;
		_mBackgroundBlocker.SetScale(Screen.width, Screen.height);
		_mBackgroundBlocker.SetPosition(0f, 0f);
		UI.BringToFront(this);
	}

	public void Open(Vector2 openPos)
	{
		Open(openPos.x, openPos.y);
	}

	public virtual void Close()
	{
		Enabled = false;
		for (int i = 0; i < 8; i++)
		{
			_mFrame[i].Enabled = false;
		}
		UI.RemoveChild(_mBackgroundBlocker);
	}

	public override void Step()
	{
		if (Enabled)
		{
			if (_mBackgroundBlocker.AllowPointerFocus && Hover() && MilMo_Pointer.LeftClick)
			{
				MilMo_Hub.ClickMade();
			}
			base.Step();
		}
	}
}
