using System;
using UI.HUD.Chat;
using UnityEngine.Events;

namespace Core.GameEvent.Types.ChatChannel;

[Serializable]
public class UnityChatChannelEvent : UnityEvent<ChatChannelSO>
{
}
