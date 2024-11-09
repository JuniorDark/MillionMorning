using Core.GameEvent;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Input;

public class GameplayControllerState : ControllerState, PlayerControls.IPlayerActions
{
	private readonly PlayerControls.PlayerActions? _actions;

	public GameplayControllerState(InputController context)
		: base(context)
	{
		if (Controls == null)
		{
			Debug.LogError("Could not initialize actions");
			return;
		}
		_actions = Controls.Player;
		_actions?.SetCallbacks(this);
	}

	public override void EnterState()
	{
		_actions?.Enable();
		InputSwitch.SetEnabled(enable: true);
	}

	public override void UpdateState()
	{
	}

	public override void ExitState()
	{
		_actions?.Disable();
		InputSwitch.SetEnabled(enable: false);
	}

	public void OnMove(InputAction.CallbackContext context)
	{
	}

	public void OnJump(InputAction.CallbackContext context)
	{
	}

	public void OnLook(InputAction.CallbackContext context)
	{
	}

	public void OnFire(InputAction.CallbackContext context)
	{
	}

	public void OnPrevWeapon(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Core.GameEvent.GameEvent.PrevWeaponEvent?.RaiseEvent();
		}
	}

	public void OnNextWeapon(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Core.GameEvent.GameEvent.NextWeaponEvent?.RaiseEvent();
		}
	}

	public void OnSkill(InputAction.CallbackContext context)
	{
	}

	public void OnUse(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Core.GameEvent.GameEvent.OnUseEvent?.RaiseEvent();
		}
	}

	public void OnAbilitySlot1(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Core.GameEvent.GameEvent.OnAbilitySlotEvent?.Invoke(0);
		}
	}

	public void OnAbilitySlot2(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Core.GameEvent.GameEvent.OnAbilitySlotEvent?.Invoke(1);
		}
	}

	public void OnAbilitySlot3(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Core.GameEvent.GameEvent.OnAbilitySlotEvent?.Invoke(2);
		}
	}

	public void OnAbilitySlot4(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Core.GameEvent.GameEvent.OnAbilitySlotEvent?.Invoke(3);
		}
	}

	public void OnAbilitySlot5(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Core.GameEvent.GameEvent.OnAbilitySlotEvent?.Invoke(4);
		}
	}

	public void OnEmote1(InputAction.CallbackContext context)
	{
	}

	public void OnEmote2(InputAction.CallbackContext context)
	{
	}

	public void OnEmote3(InputAction.CallbackContext context)
	{
	}

	public void OnEmote4(InputAction.CallbackContext context)
	{
	}

	public void OnEmote5(InputAction.CallbackContext context)
	{
	}

	public void OnEmote6(InputAction.CallbackContext context)
	{
	}

	public void OnRespawn(InputAction.CallbackContext context)
	{
	}

	public void OnBag(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Core.GameEvent.GameEvent.OnToggleInventoryEvent?.Invoke();
		}
	}

	public void OnWorldMap(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Core.GameEvent.GameEvent.OpenNavigatorEvent?.RaiseEvent();
		}
	}

	public void OnFriendList(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Core.GameEvent.GameEvent.ToggleFriendListEvent?.RaiseEvent();
		}
	}

	public void OnChat(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Core.GameEvent.GameEvent.FocusChatEvent?.RaiseEvent();
		}
	}

	public void OnMenu(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Core.GameEvent.GameEvent.OnToggleEscapeMenu?.RaiseEvent();
		}
	}

	public void OnQuestLog(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Core.GameEvent.GameEvent.ToggleQuestLOGEvent?.RaiseEvent();
		}
	}

	public void OnConsole(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Core.GameEvent.GameEvent.OnToggleConsoleEvent?.RaiseEvent();
		}
	}
}
