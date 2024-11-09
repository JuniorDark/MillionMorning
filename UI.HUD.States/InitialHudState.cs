using Core.GameEvent;

namespace UI.HUD.States;

public class InitialHudState : HudState
{
	public InitialHudState(HUD currentContext)
		: base(currentContext)
	{
	}

	public override void EnterState()
	{
		LockStateManager.UnlockAll();
		Toggle(shouldEnable: false);
	}

	public override void ExitState()
	{
	}

	private void Toggle(bool shouldEnable)
	{
		base.Context.classSelectionButton.SetHudVisibility(shouldEnable);
		base.Context.topMenu.SetHudVisibility(shouldEnable);
		base.Context.homeMenu.SetHudVisibility(shouldEnable);
		base.Context.healthBar.SetHudVisibility(shouldEnable);
		base.Context.combatWidget.SetHudVisibility(shouldEnable);
		base.Context.chat.SetHudVisibility(shouldEnable);
		base.Context.pvpActionbar.SetHudVisibility(shouldEnable);
		base.Context.actionbar.SetHudVisibility(shouldEnable);
		base.Context.bagButton.SetHudVisibility(shouldEnable);
		base.Context.useWidget.SetHudVisibility(shouldEnable);
		base.Context.counterContainer.SetHudVisibility(shouldEnable);
		GameEvent.ShowGemCounterEvent.RaiseEvent(shouldEnable);
		GameEvent.ShowCoinCounterEvent.RaiseEvent(shouldEnable);
		GameEvent.ShowExplorationCounterEvent.RaiseEvent(shouldEnable);
		base.Context.slidingPaneContainer.SetHudVisibility(shouldEnable);
		base.Context.weaponSwap.SetHudVisibility(shouldEnable);
		base.Context.contextMenu.SetHudVisibility(shouldEnable);
		base.Context.controllerChoice.SetHudVisibility(shouldEnable);
	}
}
