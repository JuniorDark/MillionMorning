using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerCoinTokensCreateHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("level_cointoken_create", message);
	}
}
