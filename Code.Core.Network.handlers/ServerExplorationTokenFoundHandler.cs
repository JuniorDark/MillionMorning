using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerExplorationTokenFoundHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("exploration_token_found", message);
	}
}
