using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerGameplayObjectStartMovingHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("gameplay_object_start_moving", message);
	}
}
