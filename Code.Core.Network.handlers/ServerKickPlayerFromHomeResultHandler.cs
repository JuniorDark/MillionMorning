using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerKickPlayerFromHomeResultHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("kick_player_from_home_result", message);
	}
}
