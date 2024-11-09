using Code.Core.EventSystem;

namespace Code.Core.Network.handlers.PVP;

public class ServerPlayersAliveCountHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("players_alive_count", message);
	}
}
