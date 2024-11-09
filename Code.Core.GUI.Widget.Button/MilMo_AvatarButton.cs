using Code.Core.GUI.Core;
using Code.World.CharBuilder.MakeOverStudio;

namespace Code.Core.GUI.Widget.Button;

public sealed class MilMo_AvatarButton : MilMo_Button
{
	public MilMo_AvatarIcon AvatarIcon;

	public MilMo_AvatarButton(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "AvatarButton " + MilMo_UserInterface.GetRandomID();
		UI = ui;
		SetAlignment(MilMo_GUI.Align.TopLeft);
	}

	public override void Draw()
	{
		if (IsEnabled())
		{
			if (AvatarIcon != null)
			{
				AvatarIcon.SetScreenRect(GetScreenPosition());
				AvatarIcon.HighlightNow(CurrentColor);
			}
			base.Draw();
		}
	}
}
