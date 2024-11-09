using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerSkillActivatedHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("skill_activated", message);
	}
}
