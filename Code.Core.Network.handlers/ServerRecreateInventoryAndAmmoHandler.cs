using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerRecreateInventoryAndAmmoHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("recreate_inventory_and_ammo", message);
	}
}
