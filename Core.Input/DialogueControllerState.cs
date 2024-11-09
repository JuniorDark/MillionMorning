using Core.GameEvent;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Input;

public class DialogueControllerState : ControllerState, PlayerControls.IDialogueActions
{
	private readonly PlayerControls.DialogueActions? _actions;

	public DialogueControllerState(InputController context)
		: base(context)
	{
		if (Controls == null)
		{
			Debug.LogError("Could not initialize actions");
			return;
		}
		_actions = Controls.Dialogue;
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

	public void OnCloseDialogue(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Core.GameEvent.GameEvent.OnCloseDialogue?.Invoke();
		}
	}
}
