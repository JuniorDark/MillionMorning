using Code.Core.EventSystem;
using Code.Core.GUI.Core;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.GUI.Widget;

public sealed class MilMo_PulseCursor : MilMo_Widget
{
	public Vector2 MinSize;

	public Vector2 MaxSize;

	public Color MinColor;

	public Color MaxColor;

	public float PulseUpDelay;

	public float PulseDownDelay;

	private MilMo_TimerEvent _mPulseUpTimer;

	private MilMo_TimerEvent _mPulseDownTimer;

	private readonly MilMo_EventSystem.MilMo_Callback _onPulseUp;

	private readonly MilMo_EventSystem.MilMo_Callback _onPulseDown;

	public MilMo_PulseCursor(MilMo_UserInterface ui)
		: base(ui)
	{
		Identifier = "PulseCursor " + MilMo_UserInterface.GetRandomID();
		_onPulseUp = OnPulseUp;
		_onPulseDown = OnPulseDown;
		MinSize = new Vector2(40f, 40f);
		MaxSize = new Vector2(50f, 50f);
		MinColor = new Color(1f, 0.95f, 0f, 1f);
		MaxColor = new Color(1f, 0f, 0f, 1f);
		PulseUpDelay = 0.1f;
		PulseDownDelay = 0.1f;
		ScaleNow(MaxSize);
		SetTexture("Batch01/Textures/World/PulseCursor");
		SetPosPull(0.12f, 0.12f);
		SetPosDrag(0.6f, 0.6f);
		SetScalePull(0.045f, 0.035f);
		SetScaleDrag(0.6f, 0.9f);
		ScaleMover.MinVel.x = 0.1f;
		ScaleMover.MinVel.y = 0.1f;
		FadeToDefaultColor = false;
		AllowPointerFocus = false;
		SetFadeSpeed(0.1f);
		AngleMover.SetUpdateFunc(MilMo_Mover.UpdateFunc.Linear);
		SetAlignment(MilMo_GUI.Align.CenterCenter);
	}

	public void Start()
	{
		PulseUp();
	}

	private void PulseUp()
	{
		if (_mPulseUpTimer != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_mPulseUpTimer);
		}
		_mPulseUpTimer = MilMo_EventSystem.At(PulseUpDelay, _onPulseUp);
	}

	private void PulseDown()
	{
		if (_mPulseDownTimer != null)
		{
			MilMo_EventSystem.RemoveTimerEvent(_mPulseDownTimer);
		}
		_mPulseDownTimer = MilMo_EventSystem.At(PulseDownDelay, _onPulseDown);
	}

	private void OnPulseUp()
	{
		ScaleTo(MaxSize);
		ColorTo(MaxColor);
		ScaleMover.Arrive = PulseDown;
	}

	private void OnPulseDown()
	{
		ScaleTo(MinSize);
		ColorTo(MinColor);
		ScaleMover.Arrive = PulseUp;
	}
}
