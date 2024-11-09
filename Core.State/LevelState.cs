using UnityEngine;

namespace Core.State;

[CreateAssetMenu(fileName = "newLevelState", menuName = "State/Level State")]
public class LevelState : ScriptableObject
{
	[SerializeField]
	public LevelTokenState coinState;

	[SerializeField]
	public LevelTokenState explorationTokenState;
}
