using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerProjectileSpawnedHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("level_projectile_create", message);
	}
}
