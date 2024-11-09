using Core.Audio.AudioData;
using TMPro;
using UI.FX;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace UI.HUD.UseWidget;

public class UseWidget : HudElement
{
	[Header("Elements")]
	[SerializeField]
	private TMP_Text text;

	[Header("Assets")]
	[SerializeField]
	private UIAudioCueSO targetSound;

	private UIAlphaFX _fader;

	private string _interactableVerb;

	private void Awake()
	{
		_fader = GetComponent<UIAlphaFX>();
		if (_fader == null)
		{
			Debug.LogWarning(base.gameObject.name + ": _fader is missing.");
		}
		else
		{
			_fader.FadeOutFast();
		}
	}

	private void OnEnable()
	{
		LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
	}

	private void OnDisable()
	{
		LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
	}

	private void RefreshText()
	{
		if (text != null)
		{
			text.text = _interactableVerb;
		}
	}

	private void OnLocaleChanged(Locale obj)
	{
		RefreshText();
	}

	private void Show(string interactionVerb)
	{
		_interactableVerb = interactionVerb;
		RefreshText();
		if (targetSound != null)
		{
			targetSound.PlayAudioCue();
		}
		_fader.FadeIn();
	}

	private void Hide()
	{
		_fader.FadeOut();
	}

	public void ShowUseWidget(string interactionVerb)
	{
		if (!string.IsNullOrEmpty(interactionVerb))
		{
			Show(interactionVerb);
		}
		else
		{
			Hide();
		}
	}

	public override void SetHudVisibility(bool shouldShow)
	{
		base.gameObject.SetActive(shouldShow);
	}
}
