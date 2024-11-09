using Core.GameEvent.Types.Base;
using UI.HUD.States;
using UnityEngine;

namespace Core.GameEvent.Types.HudState;

[CreateAssetMenu(menuName = "GameEvents/HUD State Event")]
public class HudStateEvent : BaseGameEvent<UI.HUD.States.HudState.States>
{
}
