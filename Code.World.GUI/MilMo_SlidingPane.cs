using Code.Core.EventSystem;
using Code.Core.GUI;
using Code.Core.GUI.Core;
using Code.Core.Sound;
using UnityEngine;

namespace Code.World.GUI;

public class MilMo_SlidingPane : MilMo_Widget
{
	public Vector2 SpawnPosition;

	public Vector2 TargetPosition;

	public Color FlashColor = new Color(1f, 0f, 0f, 1f);

	public Vector2 NumberImpulse = new Vector2(0f, -20f);

	public float NumberFontScale = 1f;

	public float ExtraNumberFontScale = 0.8f;

	public float ImpulseFontScale = 1.2f;

	public float NumberImpulseAngle = 10f;

	public float NumberAngle = 3f;

	protected Vector2 IconTargetPos = new Vector2(44f, -7f);

	protected Vector2 IconSpawnPos = new Vector2(239f, -7f);

	protected Vector2 NumberSpawnPos = new Vector2(210f, 0f);

	protected Vector2 NumberTargetPos = new Vector2(113f, 0f);

	protected Vector2 ExtraNumberSpawnPos = new Vector2(210f, 10f);

	protected Vector2 ExtraNumberTargetPos = new Vector2(113f, 10f);

	protected float ExtraNumberAngle = 3f;

	protected bool IsActive;

	private MilMo_TimerEvent _disableSchedule;

	private MilMo_TimerEvent _showIconSchedule;

	protected MilMo_TimerEvent SetNumberSchedule;

	private MilMo_TimerEvent _showNumberSchedule;

	public string NumberText = "";

	protected MilMo_SoundType NumberSound = MilMo_SoundType.Pick;

	public MilMo_Widget Pane { get; private set; }

	public MilMo_Widget Icon { get; private set; }

	public MilMo_Widget Number { get; private set; }

	public MilMo_Widget ExtraNumber { get; private set; }

	protected MilMo_SlidingPane(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "SlidingPane";
		SetAlignment(MilMo_GUI.Align.CenterRight);
		SetPosPull(0.1f, 0.1f);
		SetPosDrag(0.7f, 0.7f);
		SpawnPosition = new Vector2(0f, 0f);
		TargetPosition = SpawnPosition;
		Scale = new Vector2(175f, 66f);
		base.FixedRes = true;
		AllowPointerFocus = false;
		Pane = new MilMo_Widget(UI);
		Pane.SetTexture("Batch01/Textures/SlidingPane/RedPane");
		Pane.SetAlignment(MilMo_GUI.Align.CenterRight);
		Pane.SetPosition(175f, 0f);
		Pane.SetScale(175f, 66f);
		Pane.SetScalePull(0.08f, 0.06f);
		Pane.SetScaleDrag(0.6f, 0.8f);
		Pane.FixedRes = true;
		AddChild(Pane);
		Number = new MilMo_Widget(UI);
		Number.SetAlignment(MilMo_GUI.Align.CenterCenter);
		Number.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		Number.SetScale(128f, 64f);
		Number.SetPosPull(0.04f, 0.07f);
		Number.SetPosDrag(0.7f, 0.5f);
		Number.SetTextNoLocalization(NumberText);
		Number.SetFont(MilMo_GUI.Font.EborgXL);
		Number.SetFontScale(NumberFontScale);
		Number.TextDropShadowColor = new Color(0f, 0f, 0f, 0.75f);
		Number.SetTextDropShadowPos(2f, 4f);
		Number.SetDefaultTextColor(1f, 0.85f, 0f, 1f);
		Number.SetTextOutline(2f, 2f);
		Number.TextOutlineColor = new Color(0f, 0f, 0f, 0.75f);
		Number.SetFadeSpeed(0.025f);
		Number.SetAngle(NumberAngle);
		Number.SetAngleDrag(0.7f);
		Number.FixedRes = true;
		AddChild(Number);
		ExtraNumber = new MilMo_Widget(UI);
		ExtraNumber.SetAlignment(MilMo_GUI.Align.CenterCenter);
		ExtraNumber.SetTextAlignment(MilMo_GUI.Align.CenterCenter);
		ExtraNumber.SetScale(128f, 64f);
		ExtraNumber.SetPosPull(0.04f, 0.07f);
		ExtraNumber.SetPosDrag(0.7f, 0.5f);
		ExtraNumber.SetTextNoLocalization(NumberText);
		ExtraNumber.SetFont(MilMo_GUI.Font.EborgXL);
		ExtraNumber.SetFontScale(NumberFontScale);
		ExtraNumber.TextDropShadowColor = new Color(0f, 0f, 0f, 0.75f);
		ExtraNumber.SetTextDropShadowPos(2f, 4f);
		ExtraNumber.SetDefaultTextColor(1f, 0.85f, 0f, 1f);
		ExtraNumber.SetTextOutline(2f, 2f);
		ExtraNumber.TextOutlineColor = new Color(0f, 0f, 0f, 0.75f);
		ExtraNumber.SetFadeSpeed(0.025f);
		ExtraNumber.SetAngle(NumberAngle);
		ExtraNumber.SetAngleDrag(0.7f);
		ExtraNumber.FixedRes = true;
		ExtraNumber.SetEnabled(e: false);
		AddChild(ExtraNumber);
		Icon = new MilMo_Widget(UI);
		Icon.SetAlignment(MilMo_GUI.Align.CenterCenter);
		Icon.SetPosition(IconSpawnPos);
		Icon.SetScale(60f, 60f);
		Icon.SetPosPull(0.04f, 0.04f);
		Icon.SetPosDrag(0.8f, 0.8f);
		Icon.SetAnglePull(0.04f);
		Icon.SetAngleDrag(0.8f);
		Icon.FixedRes = true;
		AddChild(Icon);
		Enabled = false;
	}

	public virtual void Open()
	{
		if (!IsActive)
		{
			IsActive = true;
			if (_disableSchedule != null)
			{
				MilMo_EventSystem.RemoveTimerEvent(_disableSchedule);
			}
			SetEnabled(e: true);
			SetPosition(SpawnPosition);
			GoTo(TargetPosition);
			Icon.SetPosition(IconSpawnPos);
			if (_showIconSchedule != null)
			{
				MilMo_EventSystem.RemoveTimerEvent(_showIconSchedule);
			}
			_showIconSchedule = MilMo_EventSystem.At(0.25f, delegate
			{
				Icon.GoTo(IconTargetPos);
			});
			Number.SetPosition(NumberSpawnPos);
			Number.SetAngle(NumberAngle);
			Number.SetFontScale(NumberFontScale);
			if (_showNumberSchedule != null)
			{
				MilMo_EventSystem.RemoveTimerEvent(_showNumberSchedule);
			}
			_showNumberSchedule = MilMo_EventSystem.At(0.5f, delegate
			{
				Number.GoTo(NumberTargetPos);
				ExtraNumber.GoTo(ExtraNumberTargetPos);
			});
			ExtraNumber.SetPosition(ExtraNumberSpawnPos);
			ExtraNumber.SetAngle(ExtraNumberAngle);
			ExtraNumber.SetFontScale(ExtraNumberFontScale);
		}
	}

	public virtual void SetNumber(string number)
	{
		SetNumber(number, FlashColor);
	}

	protected void SetNumber(string number, Color flashColor)
	{
		Number.SetTextNoLocalization(number);
		Number.Impulse(NumberImpulse);
		Number.SetAngle(NumberImpulseAngle);
		Number.Angle(NumberAngle);
		Number.SetFontScale(ImpulseFontScale);
		Number.SetTextColor(flashColor);
		MilMo_EventSystem.At(0.15f, delegate
		{
			Number.SetFontScale(NumberFontScale);
		});
		MilMo_GuiSoundManager.Instance.PlaySoundFx(NumberSound);
	}

	public virtual void Close(object obj)
	{
		_disableSchedule = MilMo_EventSystem.At(1.2f, delegate
		{
			SetEnabled(e: false);
			IsActive = false;
		});
		GoTo(SpawnPosition);
	}

	public void Disable()
	{
		IsActive = false;
		Enabled = false;
		GoToNow(SpawnPosition);
	}

	public virtual void Toggle()
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

	public virtual void RefreshUI()
	{
		if (Enabled)
		{
			bool flag = PosMover.Target == SpawnPosition;
			SpawnPosition = new Vector2(Screen.width + 180, Screen.height - 233);
			TargetPosition = new Vector2(Screen.width + 20, Screen.height - 233);
			SetPosition(flag ? SpawnPosition : TargetPosition);
		}
	}
}
