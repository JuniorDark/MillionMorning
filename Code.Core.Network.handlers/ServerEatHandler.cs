using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerEatHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("player_eat", message);
	}
}
