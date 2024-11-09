using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI.Control;

public abstract class MilMo_BaseControlWidget : MilMo_Widget
{
	protected const float LABEL_OFFSET = 5f;

	protected const float COLUMN_WIDTH = 35f;

	protected float CurrentRow { get; private set; }

	protected MilMo_BaseControlWidget(MilMo_UserInterface ui)
		: base(ui)
	{
		CurrentRow = 0f;
	}

	protected MilMo_Widget CreateLabel(Vector2 position, MilMo_LocString text)
	{
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.SetFont(MilMo_GUI.Font.GothamLarge);
		milMo_Widget.SetFontPreset(MilMo_GUI.FontPreset.OutlineDropShadow);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopCenter);
		milMo_Widget.SetPosition(position);
		milMo_Widget.SetWordWrap(w: true);
		milMo_Widget.SetText(text);
		milMo_Widget.SetScale(200f, 30f);
		AddChild(milMo_Widget);
		return milMo_Widget;
	}

	protected MilMo_Widget CreateSmallLabel(Vector2 position, MilMo_LocString text)
	{
		MilMo_Widget milMo_Widget = CreateLabel(position, text);
		milMo_Widget.SetFont(MilMo_GUI.Font.GothamMedium);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		milMo_Widget.SetScale(200f, 30f);
		return milMo_Widget;
	}

	protected void CreateWideImageButton(Vector2 position, string imagePath, MilMo_Button.ButtonFunc function)
	{
		CreateImageButton(position, imagePath, function).SetScale(65f, 30f);
	}

	protected MilMo_Button CreateImageButton(Vector2 position, string imagePath, MilMo_Button.ButtonFunc function)
	{
		MilMo_Button milMo_Button = new MilMo_Button(UI);
		milMo_Button.SetTexture(imagePath);
		milMo_Button.SetHoverTexture(imagePath);
		milMo_Button.SetPressedTexture(imagePath);
		milMo_Button.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Button.SetScale(30f, 30f);
		milMo_Button.SetPosition(position);
		milMo_Button.SetFont(MilMo_GUI.Font.EborgSmall);
		milMo_Button.SetFontPreset(MilMo_GUI.FontPreset.Outline);
		milMo_Button.Function = function;
		AddChild(milMo_Button);
		return milMo_Button;
	}

	protected void CreateImageButtonWithLabel(Vector2 position, MilMo_LocString text, MilMo_Button.ButtonFunc function)
	{
		MilMo_Button milMo_Button = CreateImageButton(position, "Batch01/Textures/GameDialog/Tutorial/Controls/TutKeyBlankWide", function);
		MilMo_Widget milMo_Widget = new MilMo_Widget(UI);
		milMo_Widget.SetPosition(0f, -3f);
		milMo_Widget.SetDefaultTextColor(Color.black);
		milMo_Widget.SetFont(MilMo_GUI.Font.GothamSmall);
		milMo_Widget.SetFontScale(0.8f);
		milMo_Widget.SetDefaultTextColor(0.235f, 0.231f, 0.219f, 0.8f);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		milMo_Widget.SetWordWrap(w: true);
		milMo_Widget.SetText(text);
		milMo_Widget.SetScale(milMo_Button.Scale);
		milMo_Button.AddChild(milMo_Widget);
	}

	protected float NextRow()
	{
		CurrentRow += 35f;
		return CurrentRow;
	}
}
