using Code.Core.GUI.Core;

namespace Core.Input;

public class OldKeyboardFocusControllerState : ControllerState
{
	public OldKeyboardFocusControllerState(InputController context)
		: base(context)
	{
	}

	public override void EnterState()
	{
		InputSwitch.SetEnabled(enable: true);
		MilMo_UserInterface.KeyboardFocus = true;
	}

	public override void UpdateState()
	{
	}

	public override void ExitState()
	{
		InputSwitch.SetEnabled(enable: false);
		MilMo_UserInterface.KeyboardFocus = false;
	}
}
