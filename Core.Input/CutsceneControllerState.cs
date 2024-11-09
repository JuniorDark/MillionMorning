using Core.GameEvent;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Input;

public class CutsceneControllerState : ControllerState, PlayerControls.ICutsceneActions
{
	private readonly PlayerControls.CutsceneActions? _actions;

	public CutsceneControllerState(InputController context)
		: base(context)
	{
		if (Controls == null)
		{
			Debug.LogError("Could not initialize actions");
			return;
		}
		_actions = Controls.Cutscene;
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

	public void OnExitCutscene(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Core.GameEvent.GameEvent.OnExitCutsceneEvent?.Invoke();
		}
	}
}
