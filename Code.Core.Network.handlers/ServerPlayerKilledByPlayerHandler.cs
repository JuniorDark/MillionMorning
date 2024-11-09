using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerPlayerKilledByPlayerHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("player_killed_player", message);
	}
}
