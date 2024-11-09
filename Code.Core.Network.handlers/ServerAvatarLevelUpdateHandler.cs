using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerAvatarLevelUpdateHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("server_avatarlevel_update", message);
	}
}
