using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerWearableItemsConfirmHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("inventory_wearable_confirmed", message);
	}
}
