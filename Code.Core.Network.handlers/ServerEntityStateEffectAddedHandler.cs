using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerEntityStateEffectAddedHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("entity_state_effect_added", message);
	}
}
