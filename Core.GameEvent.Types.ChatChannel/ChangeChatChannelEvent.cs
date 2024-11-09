using Core.GameEvent.Types.Base;
using UI.HUD.Chat;
using UnityEngine;

namespace Core.GameEvent.Types.ChatChannel;

[CreateAssetMenu(menuName = "GameEvents/Change Chat Channel Event")]
public class ChangeChatChannelEvent : BaseGameEvent<ChatChannelSO>
{
}
