using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerRemotePlayerInfoHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("remote_player_info", message);
	}
}