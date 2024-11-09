using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerMoveHomeEquipmentFromStorageHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("home_equipment_move_from_storage", message);
	}
}
