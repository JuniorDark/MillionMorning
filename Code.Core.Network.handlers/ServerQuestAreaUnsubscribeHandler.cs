using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerQuestAreaUnsubscribeHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("world_quest_area_unsubscribe", message);
	}
}
