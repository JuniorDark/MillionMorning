namespace UI.HUD.States;

public class PVPAbilitiesHudState : HudState
{
	public PVPAbilitiesHudState(HUD currentContext)
		: base(currentContext)
	{
	}

	public override void EnterState()
	{
		Toggle(shouldEnable: true);
	}

	public override void ExitState()
	{
		Toggle(shouldEnable: false);
	}

	private void Toggle(bool shouldEnable)
	{
		base.Context.healthBar.SetHudVisibility(shouldEnable);
		base.Context.chat.SetHudVisibility(shouldEnable);
		base.Context.pvpActionbar.SetHudVisibility(shouldEnable);
		base.Context.weaponSwap.SetHudVisibility(shouldEnable);
	}
}
