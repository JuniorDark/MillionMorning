using Code.Core.EventSystem;

namespace Code.Core.Network.handlers.PVP;

public class ServerPlayerDeliveredFlagHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("player_delivered_flag", message);
	}
}
