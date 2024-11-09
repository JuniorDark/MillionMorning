using Core.GameEvent.Types.Base;
using UI.HUD.Chat;
using UnityEngine;

namespace Core.GameEvent.Types.ChatMessage;

[CreateAssetMenu(menuName = "GameEvents/Chat Message Event")]
public class ChatMessageEvent : BaseGameEvent<ChatMessageObject>
{
}
