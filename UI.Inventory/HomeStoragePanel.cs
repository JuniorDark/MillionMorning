using Code.Core.EventSystem;
using Code.World.Player;
using Core.State;
using UI.Elements;

namespace UI.Inventory;

public class HomeStoragePanel : Panel
{
	public override void Open()
	{
		if (!(GlobalStates.Instance == null) && MilMo_Player.InMyHome)
		{
			MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_OpenStorage");
			base.Open();
		}
	}

	public void Show(bool value)
	{
		if (value)
		{
			Open();
		}
		else
		{
			Close();
		}
	}
}
