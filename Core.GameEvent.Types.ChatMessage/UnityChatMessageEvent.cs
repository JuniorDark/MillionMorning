using System;
using UI.HUD.Chat;
using UnityEngine.Events;

namespace Core.GameEvent.Types.ChatMessage;

[Serializable]
public class UnityChatMessageEvent : UnityEvent<ChatMessageObject>
{
}
