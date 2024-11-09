using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI.Control;

public sealed class MilMo_MouseControlWidget : MilMo_BaseControlWidget
{
	public MilMo_MouseControlWidget(MilMo_UserInterface ui)
		: base(ui)
	{
		SetupMainWindow();
		CreateImageButton(new Vector2(100f, 35f), "Batch01/Textures/GameDialog/Tutorial/Controls/Mouse", null).SetScale(106f, 190f);
		CreateSmallLabel(new Vector2(40f, 35f), MilMo_Localization.GetLocString("Options_9197"));
		CreateSmallLabel(new Vector2(110f, 0f), MilMo_Localization.GetLocString("Options_9198"));
		CreateSmallLabel(new Vector2(210f, 35f), MilMo_Localization.GetLocString("Options_9199"));
		CreateSmallLabel(new Vector2(210f, 93f), MilMo_Localization.GetLocString("Options_9200"));
		CreateSmallLabel(new Vector2(210f, 135f), MilMo_Localization.GetLocString("Options_9201")).SetScale(200f, 100f);
	}

	private void SetupMainWindow()
	{
		SetAlignment(MilMo_GUI.Align.TopLeft);
		SetScale(300f, 220f);
		SetEnabled(e: true);
	}
}
