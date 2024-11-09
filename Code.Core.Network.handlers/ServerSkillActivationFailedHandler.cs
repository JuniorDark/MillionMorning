using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerSkillActivationFailedHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("skill_activation_failed", message);
	}
}
