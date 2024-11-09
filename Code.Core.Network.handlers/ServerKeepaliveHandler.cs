using Code.Core.Network.messages.client;
using Core;

namespace Code.Core.Network.handlers;

public class ServerKeepaliveHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		if (Singleton<GameNetwork>.Instance != null)
		{
			Singleton<GameNetwork>.Instance.SendToGameServer(new ClientKeepalive());
		}
	}
}
