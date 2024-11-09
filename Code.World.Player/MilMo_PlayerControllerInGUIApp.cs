using Code.Core.EventSystem;
using Code.Core.Input;
using UnityEngine;

namespace Code.World.Player;

public class MilMo_PlayerControllerInGUIApp : MilMo_PlayerControllerBase
{
	private MilMo_KeyListener _leftArrowListener;

	private MilMo_KeyListener _rightArrowListener;

	private MilMo_KeyListener _upArrowListener;

	private MilMo_KeyListener _downArrowListener;

	private MilMo_KeyListener _returnListener;

	private MilMo_KeyListener _escListener;

	private MilMo_KeyListener _joystickButton1Listener;

	private MilMo_KeyListener _joystickButton2Listener;

	public static ControllerType PreviousControllerType { get; private set; }

	public override ControllerType Type => ControllerType.InGUIApp;

	public MilMo_PlayerControllerInGUIApp()
	{
		RegisterListeners();
	}

	public MilMo_PlayerControllerInGUIApp(ControllerType previousControllerType)
	{
		PreviousControllerType = previousControllerType;
		RegisterListeners();
	}

	private void RegisterListeners()
	{
		_leftArrowListener = new MilMo_KeyListener(KeyCode.LeftArrow, LeftArrowCallback, () => true);
		_rightArrowListener = new MilMo_KeyListener(KeyCode.RightArrow, RightArrowCallback, () => true);
		_upArrowListener = new MilMo_KeyListener(KeyCode.UpArrow, UpArrowCallback, () => true);
		_downArrowListener = new MilMo_KeyListener(KeyCode.DownArrow, DownArrowCallback, () => true);
		_returnListener = new MilMo_KeyListener(KeyCode.Return, OKCallback, () => true);
		_escListener = new MilMo_KeyListener(KeyCode.Escape, CancelCallback, () => true);
		_joystickButton1Listener = new MilMo_KeyListener(KeyCode.Joystick1Button1, OKCallback, () => true);
		_joystickButton2Listener = new MilMo_KeyListener(KeyCode.Joystick1Button2, CancelCallback, () => true);
		MilMo_Input.AddKeyListener(_leftArrowListener);
		MilMo_Input.AddKeyListener(_rightArrowListener);
		MilMo_Input.AddKeyListener(_upArrowListener);
		MilMo_Input.AddKeyListener(_downArrowListener);
		MilMo_Input.AddKeyListener(_returnListener);
		MilMo_Input.AddKeyListener(_joystickButton1Listener);
		MilMo_Input.AddKeyListener(_escListener);
		MilMo_Input.AddKeyListener(_joystickButton2Listener);
	}

	private void LeftArrowCallback(object data)
	{
		MilMo_EventSystem.Instance.PostEvent("button_Left", null);
		MilMo_Input.AddKeyListener(_leftArrowListener);
	}

	private void RightArrowCallback(object data)
	{
		MilMo_EventSystem.Instance.PostEvent("button_Right", null);
		MilMo_Input.AddKeyListener(_rightArrowListener);
	}

	private void UpArrowCallback(object data)
	{
		MilMo_EventSystem.Instance.PostEvent("button_Up", null);
		MilMo_Input.AddKeyListener(_upArrowListener);
	}

	private void DownArrowCallback(object data)
	{
		MilMo_EventSystem.Instance.PostEvent("button_Down", null);
		MilMo_Input.AddKeyListener(_downArrowListener);
	}

	private void OKCallback(object data)
	{
		MilMo_Input.AddKeyListener(_returnListener);
		MilMo_Input.AddKeyListener(_joystickButton1Listener);
		if (MilMo_Pointer.GamepadPointer)
		{
			MilMo_Pointer.GamepadClick = true;
		}
		else
		{
			MilMo_EventSystem.Instance.PostEvent("button_Ok", null);
		}
	}

	private void CancelCallback(object data)
	{
		MilMo_EventSystem.Instance.PostEvent("button_Cancel", null);
		MilMo_Input.AddKeyListener(_escListener);
		MilMo_Input.AddKeyListener(_joystickButton2Listener);
	}

	public override void Exit()
	{
		base.Exit();
		if (MilMo_Pointer.PointerIcon != null)
		{
			MilMo_Pointer.PointerIcon.SetEnabled(e: false);
		}
		MilMo_Input.RemoveKeyListener(_leftArrowListener);
		MilMo_Input.RemoveKeyListener(_rightArrowListener);
		MilMo_Input.RemoveKeyListener(_upArrowListener);
		MilMo_Input.RemoveKeyListener(_downArrowListener);
		MilMo_Input.RemoveKeyListener(_returnListener);
		MilMo_Input.RemoveKeyListener(_joystickButton1Listener);
		MilMo_Input.RemoveKeyListener(_escListener);
		MilMo_Input.RemoveKeyListener(_joystickButton2Listener);
	}
}
