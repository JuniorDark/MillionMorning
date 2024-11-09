using Core.GameEvent.Types.Base;
using UI.HUD.Dialogues;

namespace Core.GameEvent.Types.Dialogue;

public class DialogueEventReference : EventReference<DialogueWindow, BaseGameEvent<DialogueWindow>>
{
	public DialogueEventReference(string addressableKey)
		: base(addressableKey)
	{
	}
}
