using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerInventoryAmountHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("inventory_amount", message);
	}
}
