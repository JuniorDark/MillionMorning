using System.Collections;
using Core.Interaction;
using UI.Marker.NPC;

namespace UI.Marker.States;

public class NoMoreQuestsState : MarkerState
{
	public NoMoreQuestsState(NPCMarker sm)
		: base(sm)
	{
	}

	public override IEnumerator Start()
	{
		Context.ChangeMarkerIcon(null);
		Context.SetInteractionType(Interactable.InteractionType.Silver);
		yield break;
	}
}
