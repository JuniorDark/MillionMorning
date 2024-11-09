using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerReceiveObjectHappyHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("receive_object_happy", message);
	}
}
