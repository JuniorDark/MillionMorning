using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerCreatureToLowLevelForXpHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("creature_to_low_level_to_xp");
	}
}
