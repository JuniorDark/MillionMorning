using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.GUI.Widget;

public sealed class MilMo_EditableToggleBar : MilMo_Widget
{
	public delegate void InputTextChanged(string value);

	private MilMo_Widget _mLeftButtonGreyOut;

	private MilMo_Widget _mRightButtonGreyOut;

	public InputTextChanged InputTextChangedCallback;

	private string _mLastFrameInputText = "§";

	public MilMo_Button LeftButton { get; private set; }

	public MilMo_Button RightButton { get; private set; }

	public MilMo_SimpleTextField TextField { get; private set; }

	public MilMo_EditableToggleBar(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "EditableToggleBar" + MilMo_UserInterface.GetRandomID();
		Initialize();
	}

	public void SetToggleText(string text)
	{
		TextField.InputText = text;
		_mLastFrameInputText = text;
	}

	private void Initialize()
	{
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
		TextField = new MilMo_SimpleTextField(UI);
		TextField.SetAlignment(MilMo_GUI.Align.CenterCenter);
		TextField.Font = UI.Font4;
		TextField.InputText = "Choice";
		TextField.UseParentAlpha = false;
		AddChild(TextField);
		LeftButton = new MilMo_Button(UI);
		LeftButton.GoToNow(UI.Align.Left, ScaleMover.Target.y / 2f);
		LeftButton.ScaleNow(ScaleMover.Target.x / 4f, ScaleMover.Target.y / 2f);
		LeftButton.SetTextNoLocalization("<");
		LeftButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		LeftButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		LeftButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		LeftButton.SetAlignment(MilMo_GUI.Align.CenterLeft);
		LeftButton.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		LeftButton.SetFont(MilMo_GUI.Font.EborgSmall);
		AddChild(LeftButton);
		RightButton = new MilMo_Button(UI);
		RightButton.GoToNow(UI.Align.Right, ScaleMover.Target.y / 2f);
		RightButton.ScaleNow(ScaleMover.Target.x / 4f, ScaleMover.Target.y / 2f);
		RightButton.SetTextNoLocalization(">");
		RightButton.SetTexture("Batch01/Textures/Dialog/ButtonNormal");
		RightButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		RightButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
		RightButton.SetAlignment(MilMo_GUI.Align.CenterRight);
		RightButton.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		RightButton.SetFont(MilMo_GUI.Font.EborgSmall);
		AddChild(RightButton);
		_mRightButtonGreyOut = new MilMo_Widget(UI);
		_mRightButtonGreyOut.SetTextureWhite();
		_mRightButtonGreyOut.SetDefaultColor(0f, 0f, 0f, 0.4f);
		_mRightButtonGreyOut.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mRightButtonGreyOut.SetPosition(0f, 0f);
		_mRightButtonGreyOut.SetScale(RightButton.Scale);
		_mRightButtonGreyOut.AllowPointerFocus = false;
		_mRightButtonGreyOut.Enabled = false;
		RightButton.AddChild(_mRightButtonGreyOut);
		_mLeftButtonGreyOut = new MilMo_Widget(UI);
		_mLeftButtonGreyOut.SetTextureWhite();
		_mLeftButtonGreyOut.SetDefaultColor(0f, 0f, 0f, 0.4f);
		_mLeftButtonGreyOut.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mLeftButtonGreyOut.SetPosition(0f, 0f);
		_mLeftButtonGreyOut.SetScale(LeftButton.Scale);
		_mLeftButtonGreyOut.AllowPointerFocus = false;
		_mLeftButtonGreyOut.Enabled = false;
		LeftButton.AddChild(_mLeftButtonGreyOut);
		RefreshUI();
	}

	private void RefreshUI()
	{
	}

	public override void Draw()
	{
		if (!Enabled)
		{
			return;
		}
		Color currentColor = CurrentColor;
		if (Parent != null && UseParentAlpha)
		{
			currentColor.a *= Parent.CurrentColor.a;
		}
		UnityEngine.GUI.color = currentColor * (IgnoreGlobalFade ? 1f : MilMo_GUI.GlobalFade);
		UnityEngine.GUI.skin = Skin;
		UnityEngine.GUI.Box(GetScreenPosition(), "");
		DrawText();
		CheckPointerFocus();
		foreach (MilMo_Widget child in base.Children)
		{
			if (child is MilMo_SimpleTextField)
			{
				TextAnchor alignment = UnityEngine.GUI.skin.textField.alignment;
				UnityEngine.GUI.skin.textField.alignment = TextAnchor.MiddleCenter;
				TextAnchor alignment2 = UnityEngine.GUI.skin.box.alignment;
				UnityEngine.GUI.skin.box.alignment = TextAnchor.MiddleCenter;
				child.Draw();
				UnityEngine.GUI.skin.textField.alignment = alignment;
				UnityEngine.GUI.skin.box.alignment = alignment2;
			}
			else
			{
				child.Draw();
			}
		}
	}

	public override void Step()
	{
		if (_mLastFrameInputText == "§")
		{
			_mLastFrameInputText = TextField.InputText;
		}
		if (_mLastFrameInputText != TextField.InputText)
		{
			InputTextChangedCallback(TextField.InputText);
			_mLastFrameInputText = TextField.InputText;
		}
		if (LeftButton.Hover() || RightButton.Hover())
		{
			TextField.IgnoreHover = true;
		}
		else if (TextField.Hover())
		{
			TextField.IgnoreHover = false;
		}
		else
		{
			MilMo_UserInterface.KeyboardFocus = true;
			TextField.IgnoreHover = true;
		}
		base.Step();
	}

	public override void SetScale(Vector2 s)
	{
		SetScale(s.x, s.y);
	}

	public override void SetScale(float x, float y)
	{
		base.SetScale(x, y);
		LeftButton.GoToNow(UI.Align.Left / UI.Res.x + 4f, ScaleMover.Target.y / UI.Res.y / 2f + ScaleMover.Target.y / UI.Res.y / 8f);
		LeftButton.ScaleNow(ScaleMover.Target.x / UI.Res.x / 5f, ScaleMover.Target.y / UI.Res.y / 2f - 4f);
		_mLeftButtonGreyOut.SetScale(LeftButton.Scale);
		TextField.GoToNow(ScaleMover.Target.x / UI.Res.x / 2f, ScaleMover.Target.y / UI.Res.y / 2f + ScaleMover.Target.y / UI.Res.y / 8f);
		TextField.ScaleNow(ScaleMover.Target.x / UI.Res.x - UI.Padding.x / UI.Res.x * 2f - 8f, ScaleMover.Target.y / UI.Res.y / 2f - 6f);
		RightButton.GoToNow(TextField.PosMover.Target.x / UI.Res.x + TextField.ScaleMover.Target.x / UI.Res.x / 2f, ScaleMover.Target.y / UI.Res.y / 2f + ScaleMover.Target.y / UI.Res.y / 8f);
		RightButton.ScaleNow(ScaleMover.Target.x / UI.Res.x / 5f, ScaleMover.Target.y / UI.Res.y / 2f - 4f);
		_mRightButtonGreyOut.SetScale(RightButton.Scale);
	}

	public void GreyOutLeftButton()
	{
		_mLeftButtonGreyOut.Enabled = true;
		LeftButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonNormal");
		LeftButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonNormal");
	}

	public void GreyOutRightButton()
	{
		_mRightButtonGreyOut.Enabled = true;
		RightButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonNormal");
		RightButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonNormal");
	}

	public void RemoveGreyOutLeftButton()
	{
		_mLeftButtonGreyOut.Enabled = false;
		LeftButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		LeftButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
	}

	public void RemoveGreyOutRightButton()
	{
		_mRightButtonGreyOut.Enabled = false;
		RightButton.SetHoverTexture("Batch01/Textures/Dialog/ButtonMO");
		RightButton.SetPressedTexture("Batch01/Textures/Dialog/ButtonPressed");
	}
}
