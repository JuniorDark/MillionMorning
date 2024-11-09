using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerPremiumTokenFoundHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("premium_token_found", message);
	}
}
