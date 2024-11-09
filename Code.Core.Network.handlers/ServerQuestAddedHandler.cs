using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerQuestAddedHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("quest_added", message);
	}
}
