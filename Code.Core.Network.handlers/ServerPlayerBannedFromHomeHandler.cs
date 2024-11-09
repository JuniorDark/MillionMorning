using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerPlayerBannedFromHomeHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("home_remote_player_banned", message);
	}
}
