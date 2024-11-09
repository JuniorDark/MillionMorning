namespace Core.Input;

public abstract class ControllerState
{
	protected readonly PlayerControls Controls;

	protected ControllerState(InputController context)
	{
		Controls = new PlayerControls();
	}

	public abstract void EnterState();

	public abstract void UpdateState();

	public abstract void ExitState();
}
