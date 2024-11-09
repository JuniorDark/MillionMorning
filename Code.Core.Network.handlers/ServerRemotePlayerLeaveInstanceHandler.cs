using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerRemotePlayerLeaveInstanceHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("remote_player_leave_instance", message);
	}
}
