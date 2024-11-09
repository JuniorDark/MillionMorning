using System.Collections;
using UI.Marker.NPC;

namespace UI.Marker.States;

public class InitialState : MarkerState
{
	public InitialState(NPCMarker sm)
		: base(sm)
	{
	}

	public override IEnumerator Start()
	{
		Context.ShowIcon(shouldEnable: false);
		yield break;
	}
}
