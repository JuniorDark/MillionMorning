using Core.State.Basic;
using UnityEngine;

namespace Core.State;

[CreateAssetMenu(fileName = "newLevelTokenState", menuName = "State/Level Token State")]
public class LevelTokenState : ScriptableObject
{
	[SerializeField]
	public IntState tokensFound;

	[SerializeField]
	public IntState tokensMax;
}
