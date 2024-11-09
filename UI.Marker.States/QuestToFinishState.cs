using System.Collections;
using Core.Interaction;
using UI.Marker.NPC;

namespace UI.Marker.States;

public class QuestToFinishState : MarkerState
{
	public QuestToFinishState(NPCMarker stateMachine)
		: base(stateMachine)
	{
	}

	public override IEnumerator Start()
	{
		Context.ChangeMarkerIcon(Context.CompleteQuestMarkerSprite);
		Context.SetInteractionType(Interactable.InteractionType.Gold);
		yield break;
	}
}
