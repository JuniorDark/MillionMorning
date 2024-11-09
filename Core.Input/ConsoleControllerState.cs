using Core.GameEvent;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Input;

public class ConsoleControllerState : ControllerState, PlayerControls.IConsoleActions
{
	private readonly PlayerControls.ConsoleActions? _actions;

	public ConsoleControllerState(InputController context)
		: base(context)
	{
		if (Controls == null)
		{
			Debug.LogError("Could not initialize actions");
			return;
		}
		_actions = Controls.Console;
		_actions?.SetCallbacks(this);
	}

	public override void EnterState()
	{
		_actions?.Enable();
	}

	public override void UpdateState()
	{
	}

	public override void ExitState()
	{
		_actions?.Disable();
	}

	public void OnSend(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Core.GameEvent.GameEvent.OnSend?.Invoke();
		}
	}

	public void OnExit(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Core.GameEvent.GameEvent.OnExit?.Invoke();
		}
	}

	public void OnHistoryBackward(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Core.GameEvent.GameEvent.OnHistoryBackward?.Invoke();
		}
	}

	public void OnHistoryForward(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Core.GameEvent.GameEvent.OnHistoryForward?.Invoke();
		}
	}

	public void OnNextCandidate(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Core.GameEvent.GameEvent.OnNextCandidate?.Invoke();
		}
	}
}
