using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.GUI.Widget;

public sealed class MilMo_ToggleBar : MilMo_Widget
{
	private const string BUTTON_TEXTURE = "Batch01/Textures/Dialog/ButtonNormal";

	private const string BUTTON_TEXTURE_MO = "Batch01/Textures/Dialog/ButtonMO";

	private const string BUTTON_TEXTURE_PRESSED = "Batch01/Textures/Dialog/ButtonPressed";

	private static readonly Color GreyOutColor = new Color(0f, 0f, 0f, 0.4f);

	private readonly MilMo_Widget _mTextLabel;

	private readonly MilMo_Widget _mLeftButtonGreyOut;

	private readonly MilMo_Widget _mRightButtonGreyOut;

	private readonly MilMo_Button _mLeftButton;

	private readonly MilMo_Button _mRightButton;

	public bool UseBackground = true;

	public MilMo_Widget TextLabel => _mTextLabel;

	public MilMo_Button LeftButton => _mLeftButton;

	public MilMo_Button RightButton => _mRightButton;

	public MilMo_ToggleBar(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "ToggleBar " + MilMo_UserInterface.GetRandomID();
		ScaleMover.Target.x = 200f;
		ScaleMover.Target.y = 75f;
		ScaleMover.Val.x = ScaleMover.Target.x;
		ScaleMover.Val.y = ScaleMover.Target.y;
		PosMover.Target.x = PosMover.Val.x;
		PosMover.Target.y = PosMover.Val.y;
		PosMover.Val.x = PosMover.Target.x;
		PosMover.Val.y = PosMover.Target.y;
		SetFont(MilMo_GUI.Font.ArialRounded);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetTextAlignment(MilMo_GUI.Align.TopCenter);
		SetText(MilMo_Localization.GetLocString("Generic_89"));
		_mTextLabel = new MilMo_Widget(UI);
		_mTextLabel.GoToNow(ScaleMover.Target.x / 2f, ScaleMover.Target.y / 2f);
		_mTextLabel.ScaleNow(ScaleMover.Target.x - UI.Padding.x / UI.Res.x * 2f, ScaleMover.Target.y / 2f);
		_mTextLabel.SetText(MilMo_Localization.GetLocString("Generic_90"));
		_mTextLabel.SetTexture("Batch01/Textures/Core/BlackTransparent");
		_mTextLabel.SetAlignment(MilMo_GUI.Align.CenterCenter);
		_mTextLabel.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		_mTextLabel.SetFont(MilMo_GUI.Font.EborgSmall);
		_mTextLabel.AllowPointerFocus = false;
		AddChild(_mTextLabel);
		_mLeftButton = new MilMo_Button(UI);
		_mLeftButton.GoToNow(UI.Align.Left, ScaleMover.Target.y / 2f);
		_mLeftButton.ScaleNow(ScaleMover.Target.x / 4f, ScaleMover.Target.y / 2f);
		_mLeftButton.SetTextNoLocalization("<");
		_mLeftButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		_mLeftButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		_mLeftButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		_mLeftButton.SetAlignment(MilMo_GUI.Align.CenterLeft);
		_mLeftButton.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		_mLeftButton.SetFont(MilMo_GUI.Font.EborgSmall);
		AddChild(_mLeftButton);
		_mLeftButtonGreyOut = new MilMo_Widget(UI);
		_mLeftButtonGreyOut.SetTextureWhite();
		_mLeftButtonGreyOut.SetDefaultColor(GreyOutColor);
		_mLeftButtonGreyOut.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mLeftButtonGreyOut.SetPosition(0f, 0f);
		_mLeftButtonGreyOut.SetScale(_mLeftButton.Scale);
		_mLeftButtonGreyOut.AllowPointerFocus = false;
		_mLeftButtonGreyOut.Enabled = false;
		_mLeftButton.AddChild(_mLeftButtonGreyOut);
		_mRightButton = new MilMo_Button(UI);
		_mRightButton.GoToNow(UI.Align.Right, ScaleMover.Target.y / 2f);
		_mRightButton.ScaleNow(ScaleMover.Target.x / 4f, ScaleMover.Target.y / 2f);
		_mRightButton.SetTextNoLocalization(">");
		_mRightButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		_mRightButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		_mRightButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		_mRightButton.SetAlignment(MilMo_GUI.Align.CenterRight);
		_mRightButton.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		_mRightButton.SetFont(MilMo_GUI.Font.EborgSmall);
		AddChild(_mRightButton);
		_mRightButtonGreyOut = new MilMo_Widget(UI);
		_mRightButtonGreyOut.SetTextureWhite();
		_mRightButtonGreyOut.SetDefaultColor(GreyOutColor);
		_mRightButtonGreyOut.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mRightButtonGreyOut.SetPosition(0f, 0f);
		_mRightButtonGreyOut.SetScale(_mRightButton.Scale);
		_mRightButtonGreyOut.AllowPointerFocus = false;
		_mRightButtonGreyOut.Enabled = false;
		_mRightButton.AddChild(_mRightButtonGreyOut);
	}

	public override void Draw()
	{
		if (!Enabled)
		{
			return;
		}
		if (UseBackground)
		{
			Color currentColor = CurrentColor;
			if (Parent != null && UseParentAlpha)
			{
				currentColor.a *= Parent.CurrentColor.a;
			}
			UnityEngine.GUI.color = currentColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
			UnityEngine.GUI.skin = Skin;
			UnityEngine.GUI.Box(GetScreenPosition(), "");
		}
		DrawText();
		CheckPointerFocus();
		foreach (MilMo_Widget child in base.Children)
		{
			child.Draw();
		}
	}

	public override void SetScale(Vector2 s)
	{
		SetScale(s.x, s.y);
	}

	public override void SetScale(float x, float y)
	{
		base.SetScale(x, y);
		if (!(UI.Res.x < 1f) || !(UI.Res.y < 1f))
		{
			_mTextLabel.GoToNow(ScaleMover.Target.x / UI.Res.x / 2f, ScaleMover.Target.y / UI.Res.y / 2f + ScaleMover.Target.y / UI.Res.y / 8f);
			_mTextLabel.ScaleNow(ScaleMover.Target.x / UI.Res.x - UI.Padding.x / UI.Res.x * 2f - 8f, ScaleMover.Target.y / UI.Res.y / 2f - 5f);
			_mLeftButton.GoToNow(UI.Align.Left / UI.Res.x + 2f, ScaleMover.Target.y / UI.Res.y / 2f + ScaleMover.Target.y / UI.Res.y / 8f);
			_mLeftButton.ScaleNow(ScaleMover.Target.x / UI.Res.x / 5f, ScaleMover.Target.y / UI.Res.y / 2f);
			_mLeftButtonGreyOut.SetScale(_mLeftButton.Scale);
			_mRightButton.GoToNow(_mTextLabel.PosMover.Target.x / UI.Res.x + _mTextLabel.ScaleMover.Target.x / UI.Res.x / 2f + 2f, ScaleMover.Target.y / UI.Res.y / 2f + ScaleMover.Target.y / UI.Res.y / 8f);
			_mRightButton.ScaleNow(ScaleMover.Target.x / UI.Res.x / 5f, ScaleMover.Target.y / UI.Res.y / 2f);
			_mRightButtonGreyOut.SetScale(_mRightButton.Scale);
		}
	}

	public void SetBaseScale(float x, float y)
	{
		base.SetScale(x, y);
	}

	public void AdjustToggleBarRight(float rightMargin)
	{
		float x = _mLeftButton.ScaleMover.Target.x;
		float num = _mTextLabel.ScaleMover.Target.x - 2f * x;
		float num2 = 2f * x + num;
		float x2 = ScaleMover.Target.x;
		_mLeftButton.GoToNow(x2 - num2 - rightMargin - 2f, _mLeftButton.PosMover.Target.y);
		_mTextLabel.GoToNow(_mLeftButton.PosMover.Target.x + x + num / 2f + 2f, _mTextLabel.PosMover.Target.y);
		_mRightButton.GoToNow(x2 - rightMargin + 2f, _mRightButton.PosMover.Target.y);
	}
}
