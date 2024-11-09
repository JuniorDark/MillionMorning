using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerDeactivateAbilityHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("ability_deactivated", message);
	}
}
