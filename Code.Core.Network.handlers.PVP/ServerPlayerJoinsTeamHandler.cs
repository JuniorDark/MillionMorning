using Code.Core.EventSystem;

namespace Code.Core.Network.handlers.PVP;

public class ServerPlayerJoinsTeamHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("player_joins_team", message);
	}
}
