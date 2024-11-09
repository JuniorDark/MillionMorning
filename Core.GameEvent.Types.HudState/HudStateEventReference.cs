using Core.GameEvent.Types.Base;
using UI.HUD.States;

namespace Core.GameEvent.Types.HudState;

public class HudStateEventReference : EventReference<UI.HUD.States.HudState.States, BaseGameEvent<UI.HUD.States.HudState.States>>
{
	public HudStateEventReference(string addressableKey)
		: base(addressableKey)
	{
	}
}
