using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerItemWieldOKHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("item_wield_ok", message);
	}
}
