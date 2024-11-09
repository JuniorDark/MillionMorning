using Code.World.CharBuilder;
using Core.BodyShapes;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AvatarBuilder.SelectionHandler;

public class ShapeSelectionHandling : SelectionHandling
{
	[SerializeField]
	private AvatarSelection.Shapes shapeTarget;

	[SerializeField]
	private GameObject target;

	private void Awake()
	{
		if (shapeTarget == AvatarSelection.Shapes.Undefined)
		{
			Debug.LogError(base.gameObject.name + ": shapeTarget is undefined.");
		}
		else if (target == null)
		{
			Debug.LogError(base.gameObject.name + ": Target is undefined");
		}
	}

	public override void Init()
	{
		base.Init();
		UpdateToggles();
	}

	public override void GenderSwitch()
	{
		base.GenderSwitch();
		UpdateToggles();
	}

	private void UpdateToggles()
	{
		string shape = AvatarEditor.GetCurrentSelection().GetShape(shapeTarget);
		ShapeLoader[] componentsInChildren = target.GetComponentsInChildren<ShapeLoader>(includeInactive: true);
		foreach (ShapeLoader loader in componentsInChildren)
		{
			UpdateToggle(loader, shape);
		}
	}

	private void UpdateToggle(ShapeLoader loader, string selectedShape)
	{
		Toggle component = loader.GetComponent<Toggle>();
		if (component == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find Toggle");
		}
		else if (selectedShape == loader.GetIdentifier())
		{
			component.isOn = true;
		}
		else
		{
			component.SetIsOnWithoutNotify(value: false);
		}
	}
}
