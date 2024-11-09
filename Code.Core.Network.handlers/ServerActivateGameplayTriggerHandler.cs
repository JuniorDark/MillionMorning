using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerActivateGameplayTriggerHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("activate_gameplay_trigger", message);
	}
}
