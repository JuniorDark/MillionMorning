using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerStaticGemsHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("level_static_gems_create", message);
	}
}
