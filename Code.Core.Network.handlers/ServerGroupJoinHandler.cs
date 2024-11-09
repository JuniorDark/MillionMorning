using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerGroupJoinHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("player_join_group", message);
	}
}
