using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerGroupInviteHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("group_invited", message);
	}
}
