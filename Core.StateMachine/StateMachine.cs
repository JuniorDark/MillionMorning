using UnityEngine;

namespace Core.StateMachine;

public abstract class StateMachine : MonoBehaviour
{
	protected State CurrentState;

	public void SetState(State state)
	{
		CurrentState = state;
		StartCoroutine(CurrentState.Start());
	}
}
