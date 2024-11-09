using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerPremiumTokensHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("premium_tokens_data", message);
	}
}
