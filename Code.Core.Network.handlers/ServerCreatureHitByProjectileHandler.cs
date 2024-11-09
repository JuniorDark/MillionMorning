using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerCreatureHitByProjectileHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("creature_hit_by_projectile", message);
	}
}
