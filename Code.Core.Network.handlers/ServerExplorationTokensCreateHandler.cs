using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerExplorationTokensCreateHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("level_explorationtoken_create", message);
	}
}
