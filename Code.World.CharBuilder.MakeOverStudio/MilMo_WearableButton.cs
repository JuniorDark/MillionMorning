using Code.Core.GUI.Core;
using Code.Core.GUI.Widget;

namespace Code.World.CharBuilder.MakeOverStudio;

public sealed class MilMo_WearableButton : MilMo_Button
{
	public int ColorIndex { get; set; }

	public int SecondaryColorIndex { get; set; }

	public MilMo_WearableButton(MilMo_UserInterface ui)
		: base(ui)
	{
	}
}
