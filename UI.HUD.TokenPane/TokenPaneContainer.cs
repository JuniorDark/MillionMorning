namespace UI.HUD.TokenPane;

public class TokenPaneContainer : HudElement
{
	public override void SetHudVisibility(bool shouldShow)
	{
		base.gameObject.SetActive(shouldShow);
	}
}
