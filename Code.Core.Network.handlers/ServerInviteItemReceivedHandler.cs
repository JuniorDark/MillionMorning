using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerInviteItemReceivedHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("received_invite_rewards", message);
	}
}
