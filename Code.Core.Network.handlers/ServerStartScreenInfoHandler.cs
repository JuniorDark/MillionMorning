using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerStartScreenInfoHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("server_start_screen_info", message);
	}
}
