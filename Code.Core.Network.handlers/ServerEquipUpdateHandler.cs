using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerEquipUpdateHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("equip_update", message);
	}
}
