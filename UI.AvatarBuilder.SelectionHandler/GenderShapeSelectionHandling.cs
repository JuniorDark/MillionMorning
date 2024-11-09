using System.Collections.Generic;
using Code.World.CharBuilder;
using Core.BodyShapes;
using Core.Colors;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AvatarBuilder.SelectionHandler;

public class GenderShapeSelectionHandling : SelectionHandling
{
	[SerializeField]
	private AvatarSelection.Shapes shapeTarget;

	[SerializeField]
	private GameObject maleTarget;

	[SerializeField]
	private GameObject femaleTarget;

	private ColorLibrary _library;

	public override void Init()
	{
		base.Init();
		if (CheckColorLibrary())
		{
			UpdateSelectedShape();
			UpdateShapeColor();
		}
	}

	private bool CheckColorLibrary()
	{
		if (_library != null)
		{
			return true;
		}
		_library = Object.FindObjectOfType<ColorLibrary>();
		if (_library == null)
		{
			Debug.LogError(base.gameObject.name + ": Color library not found");
			return false;
		}
		return true;
	}

	public override void ColorChanged(AvatarSelection.Shapes shape)
	{
		base.ColorChanged(shape);
		if (shape == shapeTarget)
		{
			UpdateShapeColor();
		}
	}

	private void UpdateShapeColor()
	{
		UpdateShapeColor(maleTarget, AvatarGender.Male);
		UpdateShapeColor(femaleTarget, AvatarGender.Female);
	}

	private void UpdateShapeColor(GameObject target, AvatarGender gender)
	{
		List<string> shapeSelectedColorIdentifiers = GetHandler(gender).GetSelection().GetShapeSelectedColorIdentifiers(shapeTarget);
		string text = ((shapeSelectedColorIdentifiers.Count > 0) ? shapeSelectedColorIdentifiers[0] : null);
		string text2 = ((shapeSelectedColorIdentifiers.Count > 1) ? shapeSelectedColorIdentifiers[1] : null);
		ScriptableColor scriptableColor = ((text != null) ? _library.GetColorByIdentifier(text) : null);
		ScriptableColor scriptableColor2 = ((text2 != null) ? _library.GetColorByIdentifier(text2) : null);
		ColoredIcon[] componentsInChildren = target.GetComponentsInChildren<ColoredIcon>(includeInactive: true);
		foreach (ColoredIcon coloredIcon in componentsInChildren)
		{
			if (scriptableColor != null)
			{
				coloredIcon.UpdatePrimaryColor(scriptableColor.GetIconColor());
			}
			if (scriptableColor2 != null)
			{
				coloredIcon.UpdateSecondaryColor(scriptableColor2.GetIconColor());
			}
		}
	}

	private void UpdateSelectedShape()
	{
		UpdateSelectedShape(maleTarget, AvatarGender.Male);
		UpdateSelectedShape(femaleTarget, AvatarGender.Female);
	}

	private void UpdateSelectedShape(GameObject target, AvatarGender gender)
	{
		string shape = GetHandler(gender).GetSelection().GetShape(shapeTarget);
		ShapeLoader[] componentsInChildren = target.GetComponentsInChildren<ShapeLoader>(includeInactive: true);
		foreach (ShapeLoader shapeLoader in componentsInChildren)
		{
			Toggle component = shapeLoader.GetComponent<Toggle>();
			if (shape == shapeLoader.GetIdentifier())
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
