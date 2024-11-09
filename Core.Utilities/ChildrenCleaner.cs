using UnityEngine;

namespace Core.Utilities;

public class ChildrenCleaner : MonoBehaviour
{
	public Transform target;

	public void CleanChildren()
	{
		if (target == null)
		{
			Debug.LogError(base.name + ": Got no target!");
			return;
		}
		for (int num = target.childCount - 1; num >= 0; num--)
		{
			DestroyChild(target.GetChild(num).gameObject);
		}
	}

	private void DestroyChild(Object go)
	{
		if (Application.isEditor)
		{
			Object.DestroyImmediate(go);
		}
		else
		{
			Object.Destroy(go);
		}
	}
}
