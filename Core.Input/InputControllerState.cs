using Core.GameEvent;
using UnityEngine.InputSystem;

namespace Core.Input;

public class InputControllerState : ControllerState, PlayerControls.IInputActions
{
	private readonly PlayerControls.InputActions? _actions;

	public InputControllerState(InputController context)
		: base(context)
	{
		_actions = Controls.Input;
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

	public void OnSubmit(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Core.GameEvent.GameEvent.OnSubmit?.Invoke();
		}
	}

	public void OnCancel(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Core.GameEvent.GameEvent.OnCancel?.Invoke();
		}
	}
}
