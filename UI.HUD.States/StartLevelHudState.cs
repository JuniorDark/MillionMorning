using System;
using Core.GameEvent;

namespace UI.HUD.States;

public class StartLevelHudState : HudState
{
	public StartLevelHudState(HUD currentContext)
		: base(currentContext)
	{
	}

	public override void EnterState()
	{
		LockStateManager.Refresh();
		LockStateManager.OnChange += Refresh;
		LockStateManager.ListenForUnlockingEvents();
		UpdateEnabled(shouldEnable: true);
		GameEvent.OnToggleInventoryEvent = (Action)Delegate.Combine(GameEvent.OnToggleInventoryEvent, new Action(base.Context.ToggleBag));
	}

	public override void ExitState()
	{
		LockStateManager.StopListeningForUnlockingEvents();
		LockStateManager.OnChange -= Refresh;
		LockStateManager.UnlockAll();
		UpdateEnabled(shouldEnable: false);
		GameEvent.OnToggleInventoryEvent = (Action)Delegate.Remove(GameEvent.OnToggleInventoryEvent, new Action(base.Context.ToggleBag));
	}

	private void Refresh()
	{
		UpdateEnabled(shouldEnable: true);
	}

	private void UpdateEnabled(bool shouldEnable)
	{
		base.Context.topMenu.SetHudVisibility(shouldEnable);
		base.Context.healthBar.SetHudVisibility(shouldEnable);
		base.Context.healthBar.SetHudVisibility(shouldEnable);
		base.Context.combatWidget.SetHudVisibility(shouldEnable);
		base.Context.actionbar.SetHudVisibility(shouldEnable && LockStateManager.HasFoundActionBar);
		base.Context.bagButton.SetHudVisibility(shouldEnable);
		base.Context.useWidget.SetHudVisibility(shouldEnable);
		base.Context.counterContainer.SetHudVisibility(shouldEnable);
		GameEvent.ShowGemCounterEvent.RaiseEvent(shouldEnable && LockStateManager.HasFoundGems);
		GameEvent.ShowCoinCounterEvent.RaiseEvent(shouldEnable && LockStateManager.HasFoundCoins);
		GameEvent.ShowExplorationCounterEvent.RaiseEvent(shouldEnable && LockStateManager.HasFoundTokens);
		base.Context.slidingPaneContainer.SetHudVisibility(shouldEnable);
		base.Context.weaponSwap.SetHudVisibility(shouldEnable);
		base.Context.controllerChoice.SetHudVisibility(shouldEnable);
	}
}
