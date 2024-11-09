using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerQuestAcceptedHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("quest_accepted", message);
	}
}
