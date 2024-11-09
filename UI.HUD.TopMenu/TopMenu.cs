namespace UI.HUD.TopMenu;

public class TopMenu : HudElement
{
	public override void SetHudVisibility(bool shouldShow)
	{
		base.gameObject.SetActive(shouldShow);
	}
}
