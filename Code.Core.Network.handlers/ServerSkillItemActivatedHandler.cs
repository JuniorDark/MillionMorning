using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerSkillItemActivatedHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("skill_item_activated", message);
	}
}
