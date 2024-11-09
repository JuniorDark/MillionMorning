using UnityEngine;
using UnityEngine.UI;

namespace Core.BodyShapes;

public class ShapeLoader : MonoBehaviour, IIdentifier
{
	[SerializeField]
	private ScriptableShape shape;

	[SerializeField]
	private Image target;

	private void Start()
	{
		if (shape != null)
		{
			SetTargetIcon();
		}
	}

	public void SetShape(ScriptableShape newShape)
	{
		shape = newShape;
		if (shape != null)
		{
			SetTargetIcon();
		}
	}

	public string GetIdentifier()
	{
		if (!(shape != null))
		{
			return null;
		}
		return shape.GetIdentifier();
	}

	private void SetTargetIcon()
	{
		target.sprite = shape.GetIcon();
		target.SetMaterialDirty();
	}
}
