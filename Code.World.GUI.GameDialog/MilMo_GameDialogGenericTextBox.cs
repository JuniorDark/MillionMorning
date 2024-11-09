using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI.GameDialog;

public abstract class MilMo_GameDialogGenericTextBox : MilMo_GameDialog
{
	private readonly Color _mTextBoxColor = new Color(0.733f, 0.929f, 0.549f);

	protected readonly MilMo_Widget TextBox;

	private readonly MilMo_Widget _mTextHeadline;

	protected readonly MilMo_Widget MTextBody;

	protected MilMo_GameDialogGenericTextBox(MilMo_UserInterface ui, ButtonMode buttonMode, MilMo_Button.ButtonFunc rightButtonFunction, MilMo_Button.ButtonFunc leftButtonFunction)
		: base(ui, buttonMode, rightButtonFunction, leftButtonFunction)
	{
		Vector2 res = ui.Res;
		ui.Res = new Vector2(1f, 1f);
		TextBox = new MilMo_Widget(ui);
		TextBox.SetPosition(14f, 65f);
		TextBox.SetScale(236f, 107f);
		TextBox.SetAlignment(MilMo_GUI.Align.TopLeft);
		TextBox.SetTexture("Batch01/Textures/GameDialog/GameDialogBoxBig");
		TextBox.SetDefaultColor(_mTextBoxColor);
		TextBox.SetEnabled(e: false);
		TextBox.AllowPointerFocus = false;
		AddChild(TextBox);
		_mTextHeadline = new MilMo_Widget(ui);
		_mTextHeadline.SetPosition(14f, 5f);
		_mTextHeadline.SetScale(210f, 46f);
		_mTextHeadline.SetAlignment(MilMo_GUI.Align.TopLeft);
		_mTextHeadline.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		_mTextHeadline.SetWordWrap(w: true);
		_mTextHeadline.SetFont(MilMo_GUI.Font.EborgSmall);
		_mTextHeadline.SetFontScale(0.9f);
		_mTextHeadline.SetText(MilMo_LocString.Empty);
		_mTextHeadline.SetDefaultTextColor(ItemTextColor);
		_mTextHeadline.SetAlpha(0f);
		_mTextHeadline.FadeToDefaultColor = false;
		_mTextHeadline.SetFadeSpeed(0.01f);
		_mTextHeadline.SetEnabled(e: false);
		_mTextHeadline.AllowPointerFocus = false;
		TextBox.AddChild(_mTextHeadline);
		MTextBody = new MilMo_Widget(ui);
		MTextBody.SetPosition(20f, 27f);
		MTextBody.SetScale(204f, 33f);
		MTextBody.SetAlignment(MilMo_GUI.Align.TopLeft);
		MTextBody.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		MTextBody.SetWordWrap(w: true);
		MTextBody.SetFont(MilMo_GUI.Font.ArialRounded);
		MTextBody.SetText(MilMo_LocString.Empty);
		MTextBody.SetDefaultTextColor(ItemTextColor);
		MTextBody.SetAlpha(0f);
		MTextBody.FadeToDefaultColor = false;
		MTextBody.SetFadeSpeed(0.01f);
		MTextBody.SetEnabled(e: false);
		MTextBody.AllowPointerFocus = false;
		TextBox.AddChild(MTextBody);
		SetHeight();
		ui.Res = res;
	}

	protected void SetText(MilMo_LocString headline, MilMo_LocString body)
	{
		MTextBody.SetYScale(MilMo_GameDialog.GetTextHeight(UI, body.String, MTextBody.ScaleMover.Target.x));
		SetHeight();
		_mTextHeadline.SetText(headline);
		MTextBody.SetText(body);
		TextBox.SetEnabled(e: true);
		_mTextHeadline.SetEnabled(e: true);
		MTextBody.SetEnabled(e: true);
	}

	protected override void ShowSecondaryWidgets()
	{
		base.ShowSecondaryWidgets();
		_mTextHeadline.AlphaTo(1f);
		MTextBody.AlphaTo(1f);
	}

	protected virtual void SetHeight()
	{
		TextBox.SetYScale(MTextBody.PosMover.Target.y + MTextBody.ScaleMover.Target.y + 15f);
		Height = TextBox.PosMover.Target.y + TextBox.ScaleMover.Target.y + 15f;
		Height += ButtonRight.ScaleMover.Target.y + 30f;
	}
}
