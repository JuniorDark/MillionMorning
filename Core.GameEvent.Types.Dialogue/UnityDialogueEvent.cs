using System;
using UI.HUD.Dialogues;
using UnityEngine.Events;

namespace Core.GameEvent.Types.Dialogue;

[Serializable]
public class UnityDialogueEvent : UnityEvent<DialogueWindow>
{
}
