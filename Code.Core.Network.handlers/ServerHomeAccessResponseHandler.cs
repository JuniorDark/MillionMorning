using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerHomeAccessResponseHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("server_home_access_response", message);
	}
}
