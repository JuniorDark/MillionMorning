using UnityEngine;

namespace Code.Core.Utility;

public class MilMo_Transform
{
	public Vector3 Position { get; set; }

	public Vector3 EulerRotation { get; set; }

	public Vector3 Scale { get; set; }

	public static Transform GetChildByName(Transform transform, string name)
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (child.name == name)
			{
				return child;
			}
			Transform childByName = GetChildByName(child, name);
			if (childByName != null)
			{
				return childByName;
			}
		}
		return null;
	}

	public static Transform GetChildWithAnimation(Transform transform)
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (child.gameObject != null && child.gameObject.GetComponent<Animation>() != null)
			{
				return child;
			}
			Transform childWithAnimation = GetChildWithAnimation(child);
			if (childWithAnimation != null)
			{
				return childWithAnimation;
			}
		}
		return null;
	}

	public MilMo_Transform()
	{
		Scale = Vector3.one;
		EulerRotation = Vector3.zero;
		Position = Vector3.zero;
	}

	public MilMo_Transform(Vector3 position, Vector3 eulerRotation)
	{
		Scale = Vector3.one;
		Position = position;
		EulerRotation = eulerRotation;
	}
}
