namespace UI.HUD.Counters;

public class CounterContainer : HudElement
{
	public override void SetHudVisibility(bool shouldShow)
	{
		base.gameObject.SetActive(shouldShow);
	}
}
