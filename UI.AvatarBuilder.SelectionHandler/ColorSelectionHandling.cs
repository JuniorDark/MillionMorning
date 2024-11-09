using Code.World.CharBuilder;
using Core.Colors;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AvatarBuilder.SelectionHandler;

public class ColorSelectionHandling : SelectionHandling
{
	[SerializeField]
	private AvatarSelection.Colors colorTarget;

	[SerializeField]
	private GameObject target;

	private void Awake()
	{
		if (colorTarget == AvatarSelection.Colors.Undefined)
		{
			Debug.LogError(base.gameObject.name + ": ColorTarget is undefined.");
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
		string color = AvatarEditor.GetCurrentSelection().GetColor(colorTarget);
		ColorLoader[] componentsInChildren = target.GetComponentsInChildren<ColorLoader>(includeInactive: true);
		foreach (ColorLoader colorLoader in componentsInChildren)
		{
			Toggle component = colorLoader.GetComponent<Toggle>();
			if (component == null)
			{
				Debug.LogError(base.gameObject.name + ": Unable to find Toggle");
				break;
			}
			if (color == colorLoader.GetIdentifier())
			{
				component.isOn = true;
			}
			else
			{
				component.SetIsOnWithoutNotify(value: false);
			}
		}
	}
}
