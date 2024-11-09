using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerUpdateMatchStateHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("update_match_state", message);
	}
}
