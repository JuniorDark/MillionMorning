using Core.Settings;
using UnityEngine;
using UnityEngine.Localization;

namespace Core.Input.ControlModes;

[CreateAssetMenu(menuName = "Controller/new controller", fileName = "ControllerChoice", order = 0)]
public class ControllerChoiceSO : ScriptableObject
{
	[SerializeField]
	private LocalizedString controllerName;

	[SerializeField]
	private LocalizedString controllerDescription;

	[SerializeField]
	private LocalizedString inputType;

	[SerializeField]
	private Sprite controllerImage;

	[SerializeField]
	private ControlModeSetting controlMode;

	public string GetControllerName()
	{
		return controllerName.GetLocalizedString();
	}

	public string GetControllerDescription()
	{
		return controllerDescription.GetLocalizedString();
	}

	public string GetControllerInputType()
	{
		return inputType.GetLocalizedString();
	}

	public Sprite GetControllerImage()
	{
		return controllerImage;
	}

	public void SetAsActiveController()
	{
		Core.Settings.Settings.SetControlMode(controlMode);
	}

	public ControlModeSetting GetControlMode()
	{
		return controlMode;
	}
}
