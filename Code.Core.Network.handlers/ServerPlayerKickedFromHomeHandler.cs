using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerPlayerKickedFromHomeHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("player_kicked_from_home", message);
	}
}
