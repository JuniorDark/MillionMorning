using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerLocalPlayerJoinLevelHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("join_level", message);
	}
}
