using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerGameplayObjectShrinkHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("gameplay_object_shrink", message);
	}
}
