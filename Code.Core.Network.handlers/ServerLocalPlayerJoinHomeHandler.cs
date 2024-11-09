using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerLocalPlayerJoinHomeHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("join_home", message);
	}
}
