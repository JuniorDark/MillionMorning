using System.Collections;

namespace Core.StateMachine;

public abstract class State
{
	public readonly StateMachine StateMachine;

	public State(StateMachine sm)
	{
		StateMachine = sm;
	}

	public virtual IEnumerator Start()
	{
		yield break;
	}
}
