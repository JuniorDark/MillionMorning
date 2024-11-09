using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerRequestPlayerNameResponseHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("requested_player_name_response", message);
	}
}
