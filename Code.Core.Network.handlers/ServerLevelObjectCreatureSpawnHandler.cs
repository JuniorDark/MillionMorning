using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerLevelObjectCreatureSpawnHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("level_object_creature_spawn", message);
	}
}
