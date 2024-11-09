using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerQuestConditionUpdateHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("internal_quest_condition_update", message);
	}
}
