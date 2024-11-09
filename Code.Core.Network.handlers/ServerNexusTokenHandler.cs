using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerNexusTokenHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("login_info_nexus", message);
	}
}