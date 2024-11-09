using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerPlayerHitAttackHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("player_attack_hit", message);
	}
}
