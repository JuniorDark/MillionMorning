using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerLevelInstanceInfoHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("server_level_instance_info", message);
	}
}
