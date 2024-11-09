using UI.FX;
using UI.Tooltip.Triggers;
using UnityEngine;

namespace UI.HUD.CombatWidget;

public class CombatWidget : HudElement
{
	[Header("Tweens")]
	[SerializeField]
	private UIRotationFXPresetSO rotationPreset;

	[SerializeField]
	private UIScaleFXPresetSO scalePreset;

	[SerializeField]
	private UIColorFXPresetSO colorPreset;

	private UIAlphaFX _fader;

	private UIRotationFX _rotation;

	private UIScaleFX _scale;

	private UIColorFX _color;

	private SimpleTooltipTrigger _tooltipTrigger;

	private bool _shouldAnimate;

	private void Awake()
	{
		GetComponents();
	}

	private void Start()
	{
		if ((bool)_fader)
		{
			_fader.FadeOutFast();
		}
		_tooltipTrigger.enabled = false;
	}

	private void GetComponents()
	{
		_fader = GetComponent<UIAlphaFX>();
		if (_fader == null)
		{
			Debug.LogWarning(base.gameObject.name + ": _fader is missing.");
			return;
		}
		_rotation = GetComponentInChildren<UIRotationFX>();
		if (_rotation == null)
		{
			Debug.LogWarning(base.gameObject.name + ": _rotation is missing.");
			return;
		}
		_scale = GetComponentInChildren<UIScaleFX>();
		if (_scale == null)
		{
			Debug.LogWarning(base.gameObject.name + ": _scale is missing.");
			return;
		}
		_color = GetComponentInChildren<UIColorFX>();
		if (_color == null)
		{
			Debug.LogWarning(base.gameObject.name + ": _color is missing.");
		}
		_tooltipTrigger = GetComponent<SimpleTooltipTrigger>();
		if (_tooltipTrigger == null)
		{
			Debug.LogWarning(base.gameObject.name + ": _tooltipTrigger is missing.");
		}
	}

	public override void SetHudVisibility(bool shouldShow)
	{
		base.gameObject.SetActive(shouldShow);
	}

	public void OnInCombatEvent(bool inCombat)
	{
		if (inCombat)
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	private void Show()
	{
		_tooltipTrigger.enabled = true;
		_fader.FadeIn();
		StartTweenLoop();
	}

	private void Hide()
	{
		_tooltipTrigger.enabled = false;
		_fader.FadeOut();
		_shouldAnimate = false;
	}

	private void StartTweenLoop()
	{
		_shouldAnimate = true;
		StartRotationTween();
		StartScaleTween();
		StartColorTween();
	}

	public void StartColorTween()
	{
		if (colorPreset != null && _shouldAnimate)
		{
			_color.Run(colorPreset);
		}
	}

	public void StartScaleTween()
	{
		if (scalePreset != null && _shouldAnimate)
		{
			_scale.Run(scalePreset);
		}
	}

	public void StartRotationTween()
	{
		if (rotationPreset != null && _shouldAnimate)
		{
			_rotation.Run(rotationPreset);
		}
	}
}
