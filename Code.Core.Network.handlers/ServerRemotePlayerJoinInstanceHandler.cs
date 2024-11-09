using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerRemotePlayerJoinInstanceHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("remote_player_join_level", message);
	}
}
