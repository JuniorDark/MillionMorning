using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.Utility;
using UnityEngine;

namespace Code.World.GUI.Ladder;

internal class jb_RatingPoint : MilMo_Widget
{
	public jb_RatingPoint(MilMo_UserInterface ui, string texturePath, Vector2 position, float xFillAmount)
		: base(ui)
	{
		if (xFillAmount < 1f)
		{
			MxFillAmount = xFillAmount;
			SetCropMode(MilMo_GUI.CropMode.Cropadelic);
		}
		Init(texturePath, position);
	}

	public jb_RatingPoint(MilMo_UserInterface ui, string texturePath, Vector2 position)
		: base(ui)
	{
		Init(texturePath, position);
	}

	private void Init(string texturePath, Vector2 position)
	{
		SetTexture(texturePath);
		SetPosition(position);
		SetEnabled(e: true);
		SetAlignment(MilMo_GUI.Align.BottomCenter);
		SetScale(0f, 0f);
		ScaleTo(25f, 25f);
		SetScalePull(0.05f, 0.05f);
		SetScaleDrag(0.7f, 0.7f);
		SetPosPull(0.05f, 0.05f);
		SetPosDrag(0.8f, 0.8f);
		SetAngle(0f - 25f * MilMo_Utility.Random() - 12.5f);
		Angle(0f);
		Impulse(10f, 0f);
		SetAlpha(0f);
		FadeSpeed = 0.05f;
	}
}
