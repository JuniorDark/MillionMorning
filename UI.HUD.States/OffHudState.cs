namespace UI.HUD.States;

public class OffHudState : HudState
{
	public OffHudState(HUD currentContext)
		: base(currentContext)
	{
	}

	public override void EnterState()
	{
		base.Context.gameObject.SetActive(value: false);
	}

	public override void ExitState()
	{
		base.Context.gameObject.SetActive(value: true);
	}
}
