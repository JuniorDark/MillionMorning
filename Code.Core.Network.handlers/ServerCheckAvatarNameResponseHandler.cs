using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerCheckAvatarNameResponseHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("check_avatar_name_response", message);
	}
}
