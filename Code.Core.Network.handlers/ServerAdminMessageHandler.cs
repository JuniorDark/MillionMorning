using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerAdminMessageHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("server_admin_message", message);
	}
}
