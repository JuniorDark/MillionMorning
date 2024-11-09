using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerUpdateTeleportStonesHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("teleportstones_updated", message);
	}
}
