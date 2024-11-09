using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerLoginInfoLevelHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("server_login_info_level", message);
	}
}
