using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerRequestHomeBoxPositionHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("generate_home_delivery_box_position", message);
	}
}
