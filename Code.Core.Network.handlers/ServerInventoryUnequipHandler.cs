using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerInventoryUnequipHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("inventory_unequip", message);
	}
}
