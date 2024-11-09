using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI.Control;

public sealed class MilMo_CameraControlWidget : MilMo_BaseControlWidget
{
	public MilMo_CameraControlWidget(MilMo_UserInterface ui)
		: base(ui)
	{
		SetupMainWindow();
		CreateLabel(new Vector2(35f, base.CurrentRow), MilMo_Localization.GetLocString("Options_9188"));
		CreateImageButton(new Vector2(0f, NextRow()), "Batch01/Textures/GameDialog/Tutorial/Controls/TutKeyN", null);
		CreateSmallLabel(new Vector2(35f, base.CurrentRow - 5f), MilMo_Localization.GetLocString("Options_9185"));
		CreateImageButtonWithLabel(new Vector2(0f, NextRow()), MilMo_Localization.GetNotLocalizedLocString("ESC"), null);
		CreateSmallLabel(new Vector2(35f, base.CurrentRow - 5f), MilMo_Localization.GetNotLocalizedLocString("Menu"));
		CreateImageButtonWithLabel(new Vector2(0f, NextRow()), MilMo_Localization.GetNotLocalizedLocString("HO\nME"), null);
		CreateSmallLabel(new Vector2(35f, base.CurrentRow - 5f), MilMo_Localization.GetLocString("Options_9186"));
		CreateImageButtonWithLabel(new Vector2(0f, NextRow()), MilMo_Localization.GetNotLocalizedLocString("END"), null);
		CreateSmallLabel(new Vector2(35f, base.CurrentRow - 5f), MilMo_Localization.GetLocString("Options_9187"));
	}

	private void SetupMainWindow()
	{
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetScale(200f, 220f);
		SetEnabled(e: true);
	}
}
