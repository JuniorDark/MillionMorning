using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerInventoryModifierChangedHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("inventory_modifier_changed", message);
	}
}
