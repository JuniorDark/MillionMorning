using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerInventoryRemoveHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("inventory_remove", message);
	}
}
