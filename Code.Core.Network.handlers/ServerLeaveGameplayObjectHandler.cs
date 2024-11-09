using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerLeaveGameplayObjectHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("leave_gameplay_object", message);
	}
}
