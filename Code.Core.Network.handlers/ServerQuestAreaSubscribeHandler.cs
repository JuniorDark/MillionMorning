using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerQuestAreaSubscribeHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("world_quest_area_subscribe", message);
	}
}
