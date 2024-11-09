using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerHomeEquipmentHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("home_equipment_add", message);
	}
}
