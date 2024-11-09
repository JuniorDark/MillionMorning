using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerEnterGameplayObjectHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("enter_gameplay_object", message);
	}
}
