using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerCapsuleAttackDamageHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("capsule_attack_damage", message);
	}
}
