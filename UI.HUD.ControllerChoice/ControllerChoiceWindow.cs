using Code.Core.EventSystem;
using Core.Analytics;
using Core.GameEvent;
using Core.Input.ControlModes;
using Core.Utilities;
using UI.FX;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace UI.HUD.ControllerChoice;

public class ControllerChoiceWindow : HudElement
{
	[SerializeField]
	private AssetReference controllerChoice;

	[SerializeField]
	private Transform choiceContainer;

	[SerializeField]
	private ControllerChoiceSO[] choices;

	[SerializeField]
	private Button button;

	[SerializeField]
	private GameObject window;

	[SerializeField]
	private UIAlphaFX fader;

	private void Awake()
	{
		if (button == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Unable to find close button");
			return;
		}
		if (fader == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Unable to get fader");
			return;
		}
		if (window == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to get window");
			return;
		}
		ControllerChoiceSO[] array = choices;
		foreach (ControllerChoiceSO controller in array)
		{
			Instantiator.Instantiate<ControllerOption>(controllerChoice, choiceContainer).Init(controller);
		}
		RegisterListeners();
		fader.FadeOutFast();
		window.SetActive(value: false);
	}

	private void OnDestroy()
	{
		UnregisterListeners();
	}

	private void RegisterListeners()
	{
		GameEvent.ShowControllerChoiceEvent.RegisterAction(Show);
		button.onClick.AddListener(Hide);
	}

	private void UnregisterListeners()
	{
		GameEvent.ShowControllerChoiceEvent.UnregisterAction(Show);
		button.onClick.RemoveListener(Hide);
	}

	public void Show()
	{
		window.SetActive(value: true);
		fader.FadeIn();
		Analytics.CustomEvent("show_controller_choice");
	}

	public void Hide()
	{
		fader.FadeOutFast();
		window.SetActive(value: false);
		MilMo_EventSystem.Instance.PostEvent("button_ResetCamera", null);
	}

	public override void SetHudVisibility(bool shouldShow)
	{
		window.SetActive(shouldShow);
	}
}
