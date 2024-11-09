using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerAvatarCreatedFailHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("avatar_created_failed", message);
	}
}
