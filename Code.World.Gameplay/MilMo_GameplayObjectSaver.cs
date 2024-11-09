using UnityEngine;

namespace Code.World.Gameplay;

public class MilMo_GameplayObjectSaver : MonoBehaviour
{
	public void OnParentAboutToBeDestroyed()
	{
		base.transform.parent = null;
	}
}
