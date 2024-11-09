using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerPlayerHitByProjectileHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("player_hit_by_projectile", message);
	}
}