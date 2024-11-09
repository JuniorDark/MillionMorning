using System;
using Core.GameEvent;

namespace UI.HUD.States;

public class LevelHudState : HudState
{
	public LevelHudState(HUD currentContext)
		: base(currentContext)
	{
	}

	public override void EnterState()
	{
		Toggle(shouldEnable: true);
		GameEvent.OnToggleInventoryEvent = (Action)Delegate.Combine(GameEvent.OnToggleInventoryEvent, new Action(base.Context.ToggleBag));
	}

	public override void ExitState()
	{
		Toggle(shouldEnable: false);
		GameEvent.OnToggleInventoryEvent = (Action)Delegate.Remove(GameEvent.OnToggleInventoryEvent, new Action(base.Context.ToggleBag));
	}

	private void Toggle(bool shouldEnable)
	{
		base.Context.topMenu.SetHudVisibility(shouldEnable);
		base.Context.healthBar.SetHudVisibility(shouldEnable);
		base.Context.combatWidget.SetHudVisibility(shouldEnable);
		base.Context.classSelectionButton.SetHudVisibility(shouldEnable);
		base.Context.chat.SetHudVisibility(shouldEnable);
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
	}
}
