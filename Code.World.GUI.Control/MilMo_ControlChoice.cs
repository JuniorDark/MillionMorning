using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI.Control;

public sealed class MilMo_ControlChoice : MilMo_Button
{
	private MilMo_Button _choiceButton;

	private MilMo_Widget _label;

	private MilMo_Widget _description;

	private MilMo_Widget _extraDescription;

	private bool _checked;

	public MilMo_Widget ControlImage { get; private set; }

	public bool Checked
	{
		get
		{
			return _checked;
		}
		set
		{
			_checked = value;
			_choiceButton.SetAllTextures(_checked ? "Batch01/Textures/CharBuilder/CheckboxTicked" : "Batch01/Textures/CharBuilder/CheckboxUnticked");
		}
	}

	public MilMo_ControlChoice(MilMo_UserInterface ui)
		: base(ui)
	{
		_checked = false;
		SetupMainWindow();
		CreateLabel();
		CreateDescription();
		CreateExtraDescription();
		CreateChoiceButton();
		CreateControlImage();
	}

	public void SetLabelText(MilMo_LocString text)
	{
		_label.SetText(text);
	}

	public void SetDescriptionText(MilMo_LocString text)
	{
		_description.SetText(text);
	}

	public void SetExtraDescriptionText(MilMo_LocString text)
	{
		_extraDescription.SetText(text);
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
			child.Draw();
		}
	}

	private void SetupMainWindow()
	{
		SetScale(280f, 330f);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetEnabled(e: true);
		SetFadeSpeed(0.2f);
		SetAlpha(1f);
		SetColor(Color.black);
		SetDefaultColor(Color.black);
		SetSkin(2);
	}

	private void CreateLabel()
	{
		_label = new MilMo_Widget(UI);
		_label.SetFont(MilMo_GUI.Font.GothamLarge);
		_label.SetFontScale(1.3f);
		_label.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		_label.SetScale(245f, 30f);
		_label.SetAlignment(MilMo_GUI.Align.TopCenter);
		_label.SetPosition(Scale.x / 2f, 10f);
		_label.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		_label.SetWordWrap(w: true);
		_label.AllowPointerFocus = false;
		AddChild(_label);
	}

	private void CreateDescription()
	{
		_description = new MilMo_Widget(UI);
		_description.SetFont(MilMo_GUI.Font.GothamMedium);
		_description.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		_description.SetScale(145f, 30f);
		_description.SetAlignment(MilMo_GUI.Align.TopCenter);
		_description.SetPosition(Scale.x / 2f, 170f);
		_description.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		_description.SetWordWrap(w: true);
		_description.AllowPointerFocus = false;
		AddChild(_description);
	}

	private void CreateExtraDescription()
	{
		_extraDescription = new MilMo_Widget(UI);
		_extraDescription.SetFont(MilMo_GUI.Font.GothamSmall);
		_extraDescription.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		_extraDescription.SetScale(245f, 60f);
		_extraDescription.SetAlignment(MilMo_GUI.Align.TopCenter);
		_extraDescription.SetPosition(Scale.x / 2f, 220f);
		_extraDescription.SetTextAlignment(MilMo_GUI.Align.TopLeft);
		_extraDescription.SetWordWrap(w: true);
		_extraDescription.AllowPointerFocus = false;
		AddChild(_extraDescription);
	}

	private void CreateChoiceButton()
	{
		_choiceButton = new MilMo_Button(UI);
		_choiceButton.SetAllTextures("Batch01/Textures/CharBuilder/CheckboxUnticked");
		_choiceButton.SetPosition(Scale.x / 2f, Scale.y - 70f);
		_choiceButton.SetScale(64f, 64f);
		_choiceButton.SetAlignment(MilMo_GUI.Align.TopCenter);
		_choiceButton.AllowPointerFocus = false;
		AddChild(_choiceButton);
	}

	private void CreateControlImage()
	{
		ControlImage = new MilMo_Widget(UI);
		ControlImage.SetScale(145f, 120f);
		ControlImage.SetAlignment(MilMo_GUI.Align.BottomCenter);
		ControlImage.SetPosition(Scale.x / 2f, 170f);
		ControlImage.AllowPointerFocus = false;
		AddChild(ControlImage);
	}
}
