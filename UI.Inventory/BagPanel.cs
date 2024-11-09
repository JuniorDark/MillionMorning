using Code.Core.EventSystem;
using Core;
using Core.GameEvent;
using UI.Elements;
using UI.LockState;

namespace UI.Inventory;

public class BagPanel : Panel
{
	public void Awake()
	{
		GameEvent.ToggleInventoryEvent.RegisterAction(base.Toggle);
	}

	private void OnDestroy()
	{
		GameEvent.ToggleInventoryEvent.UnregisterAction(base.Toggle);
	}

	public override void Open()
	{
		if (Singleton<LockStateManager>.Instance.HasUnlockedBag)
		{
			MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_ToggleBag");
			base.Open();
		}
	}
}
