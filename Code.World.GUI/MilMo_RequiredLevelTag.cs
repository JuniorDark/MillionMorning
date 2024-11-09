using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.World.GUI;

public sealed class MilMo_RequiredLevelTag : MilMo_Widget
{
	private static readonly Vector2 DefaultSize = new Vector2(32f, 32f);

	private static readonly Vector2 DefaultLevelNumberSize = new Vector2(20f, 20f);

	private const float DEFAULT_FONT_SCALE = 0.8f;

	private static readonly Vector2 DefaultLevelNumberOffset = new Vector3(-1f, 6f);

	private readonly MilMo_Widget _levelNumber;

	public int RequiredLevel { get; private set; }

	public MilMo_RequiredLevelTag(MilMo_UserInterface ui, int requiredLevel)
		: base(ui)
	{
		RequiredLevel = requiredLevel;
		SetScale(DefaultSize);
		SetTexture("Batch01/Textures/NPCDialog/TravelLevelLocked");
		_levelNumber = new MilMo_Widget(ui);
		_levelNumber.SetAlignment(MilMo_GUI.Align.TopLeft);
		_levelNumber.SetPosition(DefaultLevelNumberOffset);
		_levelNumber.SetScale(DefaultLevelNumberSize);
		_levelNumber.SetFont(MilMo_GUI.Font.EborgSmall);
		_levelNumber.SetDefaultTextColor(0f, 0f, 0f, 1f);
		_levelNumber.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		_levelNumber.SetFontScale(0.8f);
		_levelNumber.AllowPointerFocus = false;
		_levelNumber.SetText(MilMo_Localization.GetNotLocalizedLocString(RequiredLevel.ToString()));
		_levelNumber.SetPosPull(0.09f, 0.09f);
		_levelNumber.SetPosDrag(0.5f, 0.5f);
		_levelNumber.SetScalePull(0.1f, 0.1f);
		_levelNumber.SetScaleDrag(0.4f, 0.4f);
		_levelNumber.SetFadeSpeed(0.02f);
		AddChild(_levelNumber);
	}

	public void ScaleByFactor(float scaleFactor)
	{
		SetScale(DefaultSize.x * scaleFactor, DefaultSize.y * scaleFactor);
		_levelNumber.SetScale(DefaultLevelNumberSize.x * scaleFactor, DefaultLevelNumberSize.y * scaleFactor);
		_levelNumber.SetFontScale(0.8f * scaleFactor);
		_levelNumber.SetPosition(DefaultLevelNumberOffset.x * scaleFactor, DefaultLevelNumberOffset.y * scaleFactor);
	}

	public void SetRequiredLevel(int requiredLevel)
	{
		RequiredLevel = requiredLevel;
		_levelNumber.SetText(MilMo_Localization.GetNotLocalizedLocString(RequiredLevel.ToString()));
	}

	public void Flash()
	{
		_levelNumber.Impulse(new Vector2(0f, -20f));
		_levelNumber.SetTextColor(1f, 0f, 0f, 1f);
		_levelNumber.SetColor(1f, 0f, 0f, 1f);
	}
}
