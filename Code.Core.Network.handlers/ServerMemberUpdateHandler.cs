using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerMemberUpdateHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("member_update", message);
	}
}
