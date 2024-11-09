using Code.World.CharBuilder;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AvatarBuilder.SelectionHandler;

public class GenderSelectionHandler : SelectionHandling
{
	[SerializeField]
	private Toggle male;

	[SerializeField]
	private Toggle female;

	protected override void Start()
	{
		base.Start();
		if (male == null)
		{
			Debug.LogError(base.name + ": Unable to find Toggle male");
		}
		else if (female == null)
		{
			Debug.LogError(base.name + ": Unable to find Toggle female");
		}
	}

	public override void Init()
	{
		RefreshSelectedGender();
	}

	public override void GenderSwitch()
	{
		RefreshSelectedGender();
	}

	private void RefreshSelectedGender()
	{
		AvatarSelection currentSelection = AvatarEditor.GetCurrentSelection();
		if (currentSelection != null)
		{
			if (currentSelection.Gender == 0 || 1 == 0)
			{
				male.isOn = true;
				female.isOn = false;
			}
			else
			{
				male.isOn = false;
				female.isOn = true;
			}
		}
	}
}
