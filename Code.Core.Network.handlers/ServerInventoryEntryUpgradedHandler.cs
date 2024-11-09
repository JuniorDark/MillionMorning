using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerInventoryEntryUpgradedHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("inventory_entry_upgraded", message);
	}
}
