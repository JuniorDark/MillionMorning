using Core.GameEvent;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Input;

public class MenuControllerState : ControllerState, PlayerControls.IMenuActions
{
	private readonly PlayerControls.MenuActions? _actions;

	public MenuControllerState(InputController context)
		: base(context)
	{
		if (Controls == null)
		{
			Debug.LogError("Could not initialize actions");
			return;
		}
		_actions = Controls.Menu;
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

	public void OnGoBack(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Core.GameEvent.GameEvent.OnGoBackEvent?.Invoke();
		}
	}
}
