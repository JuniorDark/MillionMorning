using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerSteamTransactionErrorHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("steam_transaction_error", message);
	}
}
