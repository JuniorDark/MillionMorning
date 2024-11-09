using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerSkillsAvailableUpdateHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("player_available_skills_update", message);
	}
}
