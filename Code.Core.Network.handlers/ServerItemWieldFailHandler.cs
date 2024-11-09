using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerItemWieldFailHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("wield_fail", message);
	}
}
