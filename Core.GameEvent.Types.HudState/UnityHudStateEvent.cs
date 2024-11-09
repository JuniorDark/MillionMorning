using System;
using UI.HUD.States;
using UnityEngine.Events;

namespace Core.GameEvent.Types.HudState;

[Serializable]
public class UnityHudStateEvent : UnityEvent<UI.HUD.States.HudState.States>
{
}
