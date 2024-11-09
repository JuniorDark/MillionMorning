using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerMoveHomeEquipmentFromOtherRoomHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("home_equipment_move_from_other_room", message);
	}
}
