using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI.Control;

public sealed class MilMo_QuestControlWidget : MilMo_BaseControlWidget
{
	public MilMo_QuestControlWidget(MilMo_UserInterface ui)
		: base(ui)
	{
		SetupMainWindow();
		CreateLabel(new Vector2(25f, base.CurrentRow), MilMo_Localization.GetLocString("Options_9208"));
		CreateImageButton(new Vector2(35f, NextRow()), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKeyF", null);
		CreateSmallLabel(new Vector2(70f, base.CurrentRow - 5f), MilMo_Localization.GetLocString("Options_9209"));
		CreateImageButton(new Vector2(0f, NextRow()), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKeyB", null);
		CreateImageButton(new Vector2(35f, base.CurrentRow), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKeyI", null);
		CreateSmallLabel(new Vector2(70f, base.CurrentRow - 5f), MilMo_Localization.GetLocString("Options_9210"));
		CreateImageButton(new Vector2(35f, NextRow()), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKeyM", null);
		CreateSmallLabel(new Vector2(70f, base.CurrentRow - 5f), MilMo_Localization.GetLocString("Options_9211"));
		CreateImageButton(new Vector2(35f, NextRow()), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKeyL", null);
		CreateSmallLabel(new Vector2(70f, base.CurrentRow - 5f), MilMo_Localization.GetLocString("Options_9217"));
	}

	private void SetupMainWindow()
	{
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetScale(200f, 220f);
		SetEnabled(e: true);
	}
}
