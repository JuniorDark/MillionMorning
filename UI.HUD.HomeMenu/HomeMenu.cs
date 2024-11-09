using Code.Core.EventSystem;
using Code.World.Player;

namespace UI.HUD.HomeMenu;

public class HomeMenu : HudElement
{
	public override void SetHudVisibility(bool value)
	{
		if (!MilMo_Player.InMyHome)
		{
			base.gameObject.SetActive(value: false);
		}
		else if (value != base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value);
		}
	}

	public void EnterFurnishingMode(bool shouldEnter)
	{
		MilMo_EventSystem.Instance.PostEvent(shouldEnter ? "enter_furnishing_mode" : "exit_furnishing_mode", null);
	}
}
