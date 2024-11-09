using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerInviteResponseHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("on_invite_request", message);
	}
}
