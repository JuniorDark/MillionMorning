using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerGroupInviteResponseHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("group_invite_response", message);
	}
}
