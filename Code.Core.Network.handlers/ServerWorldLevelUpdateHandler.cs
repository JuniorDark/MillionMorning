using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerWorldLevelUpdateHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("world_level_updated", message);
	}
}
