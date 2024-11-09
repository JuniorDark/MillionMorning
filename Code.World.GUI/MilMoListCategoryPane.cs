using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;

namespace Code.World.GUI;

public class MilMoListCategoryPane : MilMo_Button
{
	private bool m_Expanded = true;

	public bool Modified;

	public bool Expanded => m_Expanded;

	public MilMoListCategoryPane(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "ListCategoryPane";
		GoToNow(0f, 0f);
		ScaleNow(188f, 35f);
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetDefaultTextColor(1f, 1f, 1f, 0.5f);
		SetHoverTextColor(1f, 1f, 1f, 1f);
		SetHoverFadeMode(MilMo_GUI.HoverBehaviour.Fade);
		SetFadeInSpeed(0.04f);
		SetFadeOutSpeed(0.04f);
		SetFont(MilMo_GUI.Font.GothamMedium);
		SetFontScale(1f);
		Function = ToggleExpand;
	}

	public void ToggleExpand(object o)
	{
		if (m_Expanded)
		{
			m_Expanded = false;
		}
		else
		{
			m_Expanded = true;
		}
		Modified = true;
	}

	public override void Step()
	{
		base.Step();
	}

	public override void Draw()
	{
		base.Draw();
		DrawText();
		if (Parent != null)
		{
			ScaleNow(Parent.Scale.x, 35f);
		}
	}
}
