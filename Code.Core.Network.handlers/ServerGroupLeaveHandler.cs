using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerGroupLeaveHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("player_leave_group", message);
	}
}
