using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerMoveHomeEquipmentHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("home_equipment_move", message);
	}
}