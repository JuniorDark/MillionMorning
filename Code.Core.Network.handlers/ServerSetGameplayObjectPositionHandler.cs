using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerSetGameplayObjectPositionHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("set_gameplay_object_pos", message);
	}
}
