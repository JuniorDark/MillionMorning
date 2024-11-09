using Code.World.CharBuilder;
using Core.Colors;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AvatarBuilder.SelectionHandler;

public class GenderColorSelectionHandling : SelectionHandling
{
	[SerializeField]
	private AvatarSelection.Colors colorTarget;

	[SerializeField]
	private GameObject maleTarget;

	[SerializeField]
	private GameObject femaleTarget;

	public override void Init()
	{
		base.Init();
		UpdateTarget(maleTarget, AvatarGender.Male);
		UpdateTarget(femaleTarget, AvatarGender.Female);
	}

	private void UpdateTarget(GameObject target, AvatarGender gender)
	{
		AvatarSelection selection = GetHandler(gender).GetSelection();
		ColorLoader[] componentsInChildren = target.GetComponentsInChildren<ColorLoader>();
		foreach (ColorLoader colorLoader in componentsInChildren)
		{
			string color = selection.GetColor(colorTarget);
			Toggle component = colorLoader.GetComponent<Toggle>();
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
