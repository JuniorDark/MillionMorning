using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerUpdateAvatarHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("avatar_updated", message);
	}
}
