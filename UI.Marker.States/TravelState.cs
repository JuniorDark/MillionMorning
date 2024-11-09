using System.Collections;
using Core.Interaction;
using UI.Marker.NPC;

namespace UI.Marker.States;

public class TravelState : MarkerState
{
	public TravelState(NPCMarker sm)
		: base(sm)
	{
	}

	public override IEnumerator Start()
	{
		Context.ChangeMarkerIcon(Context.TravelMarkerSprite);
		Context.SetInteractionType(Interactable.InteractionType.Silver);
		yield break;
	}
}
