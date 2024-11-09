using System.Collections;
using UI.Marker.NPC;

namespace UI.Marker.States;

public abstract class MarkerState
{
	protected readonly NPCMarker Context;

	protected MarkerState(NPCMarker sm)
	{
		Context = sm;
	}

	public abstract IEnumerator Start();

	public virtual IEnumerator NoMoreQuests()
	{
		Context.SetState(new NoMoreQuestsState(Context));
		yield break;
	}

	public virtual IEnumerator HasNextQuest()
	{
		Context.SetState(new HasNextQuestState(Context));
		yield break;
	}

	public virtual IEnumerator QuestActive()
	{
		Context.SetState(new QuestActiveState(Context));
		yield break;
	}

	public virtual IEnumerator QuestToFinish()
	{
		Context.SetState(new QuestToFinishState(Context));
		yield break;
	}

	public virtual IEnumerator InShop()
	{
		Context.SetState(new InShopState(Context));
		yield break;
	}

	public virtual IEnumerator Travel()
	{
		Context.SetState(new TravelState(Context));
		yield break;
	}
}
