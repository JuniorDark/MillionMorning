using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerNewNotificationHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("notification", message);
	}
}
