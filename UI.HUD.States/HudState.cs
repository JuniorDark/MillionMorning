using Core;
using UI.LockState;

namespace UI.HUD.States;

public abstract class HudState
{
	public enum States
	{
		Normal,
		StarterLevel,
		Pvp,
		PvpAbilities,
		Home
	}

	protected readonly LockStateManager LockStateManager;

	protected HUD Context { get; }

	public HudState(HUD currentContext)
	{
		LockStateManager = Singleton<LockStateManager>.Instance;
		Context = currentContext;
	}

	public abstract void EnterState();

	public abstract void ExitState();

	public void SwitchState(HudState newState)
	{
		ExitState();
		newState.EnterState();
		Context.CurrentState = newState;
	}
}
