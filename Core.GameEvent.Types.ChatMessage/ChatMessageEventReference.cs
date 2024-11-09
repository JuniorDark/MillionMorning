using Core.GameEvent.Types.Base;
using UI.HUD.Chat;

namespace Core.GameEvent.Types.ChatMessage;

public class ChatMessageEventReference : EventReference<ChatMessageObject, BaseGameEvent<ChatMessageObject>>
{
	public ChatMessageEventReference(string addressableKey)
		: base(addressableKey)
	{
	}
}
