using Code.Core.EventSystem;
using Code.Core.GUI.Core;
using Code.Core.ResourceSystem;
using Code.Core.Sound;
using UnityEngine;

namespace Code.World.GUI.HudCounter.Counters;

public class MilMo_HudCounter : MilMo_SlidingPane
{
	protected Vector2 IconScale = new Vector2(64f, 64f);

	protected Vector2 IconFlashScale = new Vector2(96f, 96f);

	protected MilMo_HudCounter(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "Counter";
		SpawnPosition = new Vector2(-180f, 122f);
		TargetPosition = new Vector2(22f, 122f);
		FlashColor = new Color(0f, 1f, 0f, 1f);
		NumberImpulse = new Vector2(0f, -10f);
		NumberFontScale = 1f;
		ExtraNumberFontScale = 0.7f;
		ImpulseFontScale = 1.1f;
		SetAlignment(MilMo_GUI.Align.CenterLeft);
		SetPosPull(0.08f, 0.08f);
		SetPosDrag(0.6f, 0.6f);
		LoadAndSetPaneTextureAsync();
		base.Pane.SetDefaultColor(0f, 0f, 0.45f, 0.25f);
		base.Pane.SetAlignment(MilMo_GUI.Align.CenterLeft);
		base.Pane.SetPosition(0f, -8f);
		base.Pane.SetScale(110f, 50f);
		NumberSound = MilMo_SoundType.None;
		NumberAngle = -10f;
		NumberImpulseAngle = 0f;
		NumberSpawnPos = new Vector2(50f, -6f);
		NumberTargetPos = new Vector2(50f, -6f);
		ExtraNumberSpawnPos = new Vector2(50f, 10f);
		ExtraNumberTargetPos = new Vector2(50f, 10f);
		ExtraNumberAngle = -10f;
		base.Number.SetAlignment(MilMo_GUI.Align.CenterLeft);
		base.Number.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		base.Number.SetScale(128f, 32f);
		base.Number.SetPosPull(0.04f, 0.07f);
		base.Number.SetPosDrag(0.7f, 0.5f);
		base.Number.SetTextNoLocalization(NumberText);
		base.Number.SetFont(MilMo_GUI.Font.EborgMedium);
		base.Number.SetFontScale(NumberFontScale);
		base.Number.TextDropShadowColor = new Color(0f, 0f, 0f, 0.35f);
		base.Number.SetTextDropShadowPos(3f, 3f);
		base.Number.SetDefaultTextColor(1f, 1f, 1f, 1f);
		base.Number.SetTextOutline(1f, 1f);
		base.Number.TextOutlineColor = new Color(0f, 0f, 0f, 0.5f);
		base.Number.SetAngle(NumberAngle);
		base.ExtraNumber.SetAlignment(MilMo_GUI.Align.CenterLeft);
		base.ExtraNumber.SetTextAlignment(MilMo_GUI.Align.CenterLeft);
		base.ExtraNumber.SetScale(128f, 32f);
		base.ExtraNumber.SetPosPull(0.04f, 0.07f);
		base.ExtraNumber.SetPosDrag(0.7f, 0.5f);
		base.ExtraNumber.SetTextNoLocalization(NumberText);
		base.ExtraNumber.SetFont(MilMo_GUI.Font.EborgMedium);
		base.ExtraNumber.SetFontScale(NumberFontScale);
		base.ExtraNumber.TextDropShadowColor = new Color(0f, 0f, 0f, 0.35f);
		base.ExtraNumber.SetTextDropShadowPos(3f, 3f);
		base.ExtraNumber.SetDefaultTextColor(1f, 1f, 1f, 1f);
		base.ExtraNumber.SetTextOutline(1f, 1f);
		base.ExtraNumber.TextOutlineColor = new Color(0f, 0f, 0f, 0.5f);
		base.ExtraNumber.SetAngle(NumberAngle);
		IconSpawnPos = new Vector2(29f, -7f);
		IconTargetPos = new Vector2(29f, -7f);
		base.Icon.SetAlignment(MilMo_GUI.Align.CenterCenter);
		base.Icon.SetPosition(IconSpawnPos);
		base.Icon.SetScale(IconScale);
		base.Icon.SetScalePull(0.1f, 0.1f);
		base.Icon.SetScaleDrag(0.4f, 0.4f);
		AddChild(base.Icon);
		BringToFront(base.Number);
		BringToFront(base.ExtraNumber);
	}

	private async void LoadAndSetPaneTextureAsync()
	{
		Texture2D texture = await MilMo_ResourceManager.Instance.LoadTextureAsync("Content/GUI/Batch01/Textures/HUD/CounterBack");
		base.Pane.SetTexture(new MilMo_Texture(texture));
	}

	public override void Open()
	{
		if (!IsActive)
		{
			GoToNow(SpawnPosition);
			base.Open();
		}
	}

	public override void SetNumber(string number)
	{
		base.SetNumber(number);
		base.Icon.SetScale(IconFlashScale);
		base.Icon.ScaleTo(IconScale);
	}

	public void ShakeInRed()
	{
		base.Icon.SetScale(IconFlashScale);
		base.Icon.ScaleTo(IconScale);
		base.Number.Impulse(NumberImpulse);
		base.Number.SetAngle(NumberImpulseAngle);
		base.Number.Angle(NumberAngle);
		base.Number.SetFontScale(ImpulseFontScale);
		base.Number.SetTextColor(1f, 0f, 0f, 1f);
		MilMo_EventSystem.At(0.15f, delegate
		{
			SetFontScale(NumberFontScale);
		});
	}

	public override void Toggle()
	{
		if (!IsActive)
		{
			Open();
		}
		else
		{
			Close(null);
		}
	}
}
