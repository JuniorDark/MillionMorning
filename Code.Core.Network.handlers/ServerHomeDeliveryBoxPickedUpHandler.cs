using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerHomeDeliveryBoxPickedUpHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("home_delivery_box_picked_up", message);
	}
}
