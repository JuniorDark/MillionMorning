using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerHomeInstanceInfoHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("server_home_instance_info", message);
	}
}
