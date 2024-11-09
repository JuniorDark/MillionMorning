using Code.Core.EventSystem;

namespace Code.Core.Network.handlers;

public class ServerJuneCashItemsHandler : IHandler
{
	public void Handle(IMessage message, IZenListener listener)
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("junecash_items", message);
	}
}
