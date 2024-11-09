using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerActivateAbilityHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("ability_activated", message);
	}
}
