using System.Collections.Generic;

namespace Core.Input;

public class InputController : Singleton<InputController>
{
	private readonly Stack<ControllerState> _controllerStack = new Stack<ControllerState>();

	private ControllerState _currentState;

	private void Awake()
	{
		InputSwitch.SetEnabled(enable: false);
		SetGamePlayController();
	}

	private void OnDestroy()
	{
		_currentState?.ExitState();
		_currentState = null;
		_controllerStack.Clear();
	}

	public void SetCutsceneController()
	{
		SwitchState(new CutsceneControllerState(this));
	}

	public void SetInputController()
	{
		SwitchState(new InputControllerState(this));
	}

	public void SetOldKeyboardFocusController()
	{
		SwitchState(new OldKeyboardFocusControllerState(this));
	}

	public void SetMenuController()
	{
		SwitchState(new MenuControllerState(this));
	}

	public void SetConsoleController()
	{
		SwitchState(new ConsoleControllerState(this));
	}

	public void SetDialogueController()
	{
		SwitchState(new DialogueControllerState(this));
	}

	public void SetGamePlayController()
	{
		SwitchState(new GameplayControllerState(this));
	}

	private void SwitchState(ControllerState state)
	{
		if (_controllerStack.Count < 1 || _controllerStack.Peek()?.GetType() != state?.GetType())
		{
			_controllerStack.Push(state);
		}
		if (!(_currentState?.GetType() == state?.GetType()))
		{
			_currentState?.ExitState();
			_currentState = state;
			_currentState?.EnterState();
		}
	}

	public void RestorePreviousController()
	{
		if (_controllerStack.Count > 1)
		{
			_controllerStack.Pop();
		}
		if (_controllerStack.Count > 0)
		{
			SwitchState(_controllerStack.Peek());
		}
	}

	public static InputController Get()
	{
		return Singleton<InputController>.Instance;
	}
}
