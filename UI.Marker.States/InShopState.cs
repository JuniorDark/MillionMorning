using System.Collections;
using Core.Interaction;
using UI.Marker.NPC;

namespace UI.Marker.States;

public class InShopState : MarkerState
{
	public InShopState(NPCMarker stateMachine)
		: base(stateMachine)
	{
	}

	public override IEnumerator Start()
	{
		Context.ChangeMarkerIcon(Context.ShopMarkerSprite);
		Context.SetInteractionType(Interactable.InteractionType.Silver);
		yield break;
	}
}
