using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerItemUnwieldOKHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("item_unwield_ok", message);
	}
}
