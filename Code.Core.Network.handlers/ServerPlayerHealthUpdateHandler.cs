using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerPlayerHealthUpdateHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("player_health_update", message);
	}
}
