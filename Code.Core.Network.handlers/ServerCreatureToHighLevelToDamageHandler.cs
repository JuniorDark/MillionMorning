using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerCreatureToHighLevelToDamageHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("creature_to_high_level_to_damage");
	}
}
