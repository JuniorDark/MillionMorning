using System.Collections;
using Core.Interaction;
using UI.Marker.NPC;

namespace UI.Marker.States;

public class QuestActiveState : MarkerState
{
	public QuestActiveState(NPCMarker stateMachine)
		: base(stateMachine)
	{
	}

	public override IEnumerator Start()
	{
		Context.ChangeMarkerIcon(Context.ActiveQuestMarkerSprite);
		Context.SetInteractionType(Interactable.InteractionType.Silver);
		yield break;
	}
}
