using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerCoinTokenFoundHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("coin_token_found", message);
	}
}
