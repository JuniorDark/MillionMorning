using System.Collections.Generic;
using Core.Analytics;
using Core.Input.ControlModes;
using Core.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.ControllerChoice;

[RequireComponent(typeof(Toggle))]
public class ControllerOption : MonoBehaviour
{
	[Header("Elements")]
	[SerializeField]
	private TMP_Text controllerName;

	[SerializeField]
	private TMP_Text controllerDescription;

	[SerializeField]
	private TMP_Text inputType;

	[SerializeField]
	private Image controllerImage;

	[SerializeField]
	private ControllerChoiceSO so;

	private Toggle _toggle;

	private void Awake()
	{
		_toggle = GetComponent<Toggle>();
		SetToggleGroup();
	}

	public void Init(ControllerChoiceSO controller)
	{
		so = controller;
		UpdateElements();
	}

	private void OnEnable()
	{
		if (so != null)
		{
			_toggle.isOn = Settings.ControlMode == so.GetControlMode();
		}
	}

	private void UpdateElements()
	{
		controllerName.text = so.GetControllerName();
		controllerDescription.text = so.GetControllerDescription();
		inputType.text = so.GetControllerInputType();
		controllerImage.sprite = so.GetControllerImage();
	}

	private void SetToggleGroup()
	{
		ToggleGroup componentInParent = GetComponentInParent<ToggleGroup>();
		if (!componentInParent)
		{
			Debug.LogWarning(base.gameObject.name + ": Unable to get toggleGroup in parent");
		}
		else
		{
			_toggle.group = componentInParent;
		}
	}

	public void SetAsActiveController(bool value)
	{
		if (value)
		{
			Analytics.CustomEventWithData("select_controller_choice", new Dictionary<string, object> { 
			{
				"controller_choice",
				(controllerName != null) ? controllerName.text : ""
			} });
			so.SetAsActiveController();
		}
	}
}
