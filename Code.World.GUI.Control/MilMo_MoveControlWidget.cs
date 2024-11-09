using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI.Control;

public sealed class MilMo_MoveControlWidget : MilMo_BaseControlWidget
{
	public MilMo_MoveControlWidget(MilMo_UserInterface ui)
		: base(ui)
	{
		SetupMainWindow();
		CreateLabel(new Vector2(25f, base.CurrentRow), MilMo_Localization.GetLocString("Options_9207"));
		CreateImageButton(new Vector2(0f, NextRow()), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKeyW", null);
		CreateImageButton(new Vector2(35f, base.CurrentRow), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKeyUp", null);
		CreateSmallLabel(new Vector2(70f, base.CurrentRow - 5f), MilMo_Localization.GetLocString("Options_9202"));
		CreateImageButton(new Vector2(0f, NextRow()), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKeyA", null);
		CreateImageButton(new Vector2(35f, base.CurrentRow), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKeyLeft", null);
		CreateSmallLabel(new Vector2(70f, base.CurrentRow - 5f), MilMo_Localization.GetLocString("Options_9203"));
		CreateImageButton(new Vector2(0f, NextRow()), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKeyS", null);
		CreateImageButton(new Vector2(35f, base.CurrentRow), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKeyDown", null);
		CreateSmallLabel(new Vector2(70f, base.CurrentRow - 5f), MilMo_Localization.GetLocString("Options_9204"));
		CreateImageButton(new Vector2(0f, NextRow()), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKeyD", null);
		CreateImageButton(new Vector2(35f, base.CurrentRow), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKeyRight", null);
		CreateSmallLabel(new Vector2(70f, base.CurrentRow - 5f), MilMo_Localization.GetLocString("Options_9205"));
		CreateWideImageButton(new Vector2(0f, NextRow()), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKeySpace", null);
		CreateSmallLabel(new Vector2(70f, base.CurrentRow - 5f), MilMo_Localization.GetLocString("Options_9206"));
	}

	private void SetupMainWindow()
	{
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetScale(200f, 200f);
		SetEnabled(e: true);
	}
}
