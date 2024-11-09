using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI.Control;

public sealed class MilMo_FightControlWidget : MilMo_BaseControlWidget
{
	public MilMo_FightControlWidget(MilMo_UserInterface ui)
		: base(ui)
	{
		SetupMainWindow();
		CreateLabel(new Vector2(25f, base.CurrentRow), MilMo_Localization.GetLocString("Options_9190"));
		CreateImageButton(new Vector2(35f, NextRow()), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKeyC", null);
		CreateSmallLabel(new Vector2(70f, base.CurrentRow - 5f), MilMo_Localization.GetLocString("Options_9191"));
		CreateImageButton(new Vector2(0f, NextRow()), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKeyQ", null);
		CreateImageButtonWithLabel(new Vector2(35f, base.CurrentRow), MilMo_Localization.GetNotLocalizedLocString("DEL"), null);
		CreateSmallLabel(new Vector2(70f, base.CurrentRow - 5f), MilMo_Localization.GetLocString("Options_9192"));
		CreateImageButton(new Vector2(0f, NextRow()), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKeyE", null);
		CreateImageButtonWithLabel(new Vector2(35f, base.CurrentRow), MilMo_Localization.GetNotLocalizedLocString("PG\nDN"), null);
		CreateSmallLabel(new Vector2(70f, base.CurrentRow - 5f), MilMo_Localization.GetLocString("Options_9193"));
		CreateWideImageButton(new Vector2(0f, NextRow()), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKeySpace", null);
		CreateSmallLabel(new Vector2(70f, base.CurrentRow - 5f), MilMo_Localization.GetLocString("Options_9194"));
		CreateImageButton(new Vector2(0f, NextRow()), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKey1", null);
		CreateImageButton(new Vector2(35f, base.CurrentRow), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKey4", null);
		CreateSmallLabel(new Vector2(70f, base.CurrentRow - 5f), MilMo_Localization.GetLocString("Options_9195"));
		CreateImageButton(new Vector2(35f, NextRow()), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKeyR", null);
		CreateSmallLabel(new Vector2(70f, base.CurrentRow - 5f), MilMo_Localization.GetLocString("Options_9196"));
	}

	private void SetupMainWindow()
	{
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetScale(200f, 220f);
		SetEnabled(e: true);
	}
}
