using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerLevelObjectPickupHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("level_object_pickup", message);
	}
}
