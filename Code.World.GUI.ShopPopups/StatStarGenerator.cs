using Code.Core.GUI;
using Code.Core.GUI.Core;

namespace Code.World.GUI.ShopPopups;

public sealed class StatStarGenerator
{
	private readonly MilMo_UserInterface _ui;

	public StatStarGenerator(MilMo_UserInterface ui)
	{
		_ui = ui;
	}

	public void SetStars(MilMo_Widget container, int points)
	{
		for (int i = 0; i < 5; i++)
		{
			if (i < points / 2)
			{
				MilMo_Widget w = CreateStar("Batch01/Textures/Voting/starFilled", 0f + (float)(i * 15), 0f);
				container.AddChild(w);
				continue;
			}
			MilMo_Widget w2 = CreateStar("Batch01/Textures/Voting/starEmpty", 0f + (float)(i * 15), 0f);
			container.AddChild(w2);
			if ((float)i < (float)points / 2f)
			{
				MilMo_Widget w3 = CreateStar("Batch01/Textures/Voting/starFilled", 0f + (float)(i * 15), 0.5f);
				container.AddChild(w3);
			}
		}
	}

	private MilMo_Widget CreateStar(string texturePath, float position, float crop)
	{
		MilMo_Widget milMo_Widget = new MilMo_Widget(_ui);
		milMo_Widget.SetTexture(texturePath);
		milMo_Widget.SetScale(15f, 15f);
		milMo_Widget.SetAlignment(MilMo_GUI.Align.TopLeft);
		milMo_Widget.SetPosition(position, 0f);
		milMo_Widget.AllowPointerFocus = false;
		milMo_Widget.SetAlpha(0.35f);
		if (!(crop > 0f))
		{
			return milMo_Widget;
		}
		milMo_Widget.SetCropMode(MilMo_GUI.CropMode.Cropadelic);
		milMo_Widget.MxFillAmount = crop;
		return milMo_Widget;
	}
}
