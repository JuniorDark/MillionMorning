using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerAmmoTypesHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("ammo_types", message);
	}
}
