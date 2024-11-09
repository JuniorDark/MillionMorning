using UnityEngine.EventSystems;

namespace UI.HUD.BagButton;

public class BagButton : HudElement
{
	public void Click()
	{
		EventSystem.current.SetSelectedGameObject(null);
	}

	public override void SetHudVisibility(bool shouldShow)
	{
		base.gameObject.SetActive(shouldShow);
	}
}
