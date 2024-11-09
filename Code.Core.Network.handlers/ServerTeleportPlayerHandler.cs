using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerTeleportPlayerHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("teleport_player", message);
	}
}
