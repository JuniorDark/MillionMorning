using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI.Control;

public sealed class MilMo_SocialControlWidget : MilMo_BaseControlWidget
{
	public MilMo_SocialControlWidget(MilMo_UserInterface ui)
		: base(ui)
	{
		SetupMainWindow();
		CreateLabel(new Vector2(25f, base.CurrentRow), MilMo_Localization.GetLocString("Options_9213"));
		CreateImageButton(new Vector2(35f, NextRow()), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKeyP", null);
		CreateSmallLabel(new Vector2(70f, base.CurrentRow - 5f), MilMo_Localization.GetLocString("Options_9214"));
		CreateWideImageButton(new Vector2(0f, NextRow()), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKeyReturn", null);
		CreateSmallLabel(new Vector2(70f, base.CurrentRow - 5f), MilMo_Localization.GetLocString("Options_9215"));
		CreateImageButton(new Vector2(0f, NextRow()), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKey5", null);
		CreateImageButton(new Vector2(35f, base.CurrentRow), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKey0", null);
		CreateSmallLabel(new Vector2(70f, base.CurrentRow - 5f), MilMo_Localization.GetLocString("Options_9216"));
	}

	private void SetupMainWindow()
	{
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetScale(200f, 220f);
		SetEnabled(e: true);
	}
}
