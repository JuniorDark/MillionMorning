using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerAllEquipmentInRoomSentHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("all_equipment_in_room_sent", message);
	}
}
