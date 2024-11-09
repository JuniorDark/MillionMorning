using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerGameplayObjectChangeDirectionHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("gameplay_object_change_direction", message);
	}
}
