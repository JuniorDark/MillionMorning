using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerAmmoUpdateHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("ammo_update", message);
	}
}
