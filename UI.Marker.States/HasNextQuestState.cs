using System.Collections;
using Core.Interaction;
using UI.Marker.NPC;

namespace UI.Marker.States;

public class HasNextQuestState : MarkerState
{
	public HasNextQuestState(NPCMarker sm)
		: base(sm)
	{
	}

	public override IEnumerator Start()
	{
		Context.ChangeMarkerIcon(Context.NextQuestMarkerSprite);
		Context.SetInteractionType(Interactable.InteractionType.Gold);
		yield break;
	}
}
