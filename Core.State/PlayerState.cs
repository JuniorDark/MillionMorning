using Core.State.Basic;
using UnityEngine;

namespace Core.State;

[CreateAssetMenu(fileName = "newPlayerState", menuName = "State/Player State")]
public class PlayerState : ScriptableObject
{
	[SerializeField]
	public IntState health;

	[SerializeField]
	public IntState maxHealth;

	[SerializeField]
	public IntState coins;

	[SerializeField]
	public IntState gems;

	[SerializeField]
	public IntState gemBonusTimeLeftInSeconds;

	[SerializeField]
	public StringState ammoType;

	[SerializeField]
	public IntState ammoAmount;
}
