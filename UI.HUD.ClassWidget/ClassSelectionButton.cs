using Code.World.Player;
using UI.FX;
using UI.HUD.Dialogues;
using UI.Tooltip.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.ClassWidget;

public class ClassSelectionButton : HudElement
{
	[Header("Tween Preset")]
	[SerializeField]
	private UIScaleFXPresetSO scalePreset;

	[Header("Component")]
	[SerializeField]
	private Image selectionImage;

	[SerializeField]
	private Button selectionButton;

	private const float HALF_ALPHA = 0.5f;

	private const float FULL_ALPHA = 1f;

	private UIAlphaFX _fader;

	private UIScaleFX _scale;

	private bool _shouldAnimate;

	private bool _canShow;

	private PlayerClassManager _manager;

	private SimpleTooltipTrigger _tooltipTrigger;

	private void Awake()
	{
		_fader = GetComponent<UIAlphaFX>();
		if (_fader == null)
		{
			Debug.LogWarning(base.gameObject.name + ": _fader is missing.");
			return;
		}
		_scale = GetComponent<UIScaleFX>();
		if (_scale == null)
		{
			Debug.LogWarning(base.gameObject.name + ": _scale is missing.");
			return;
		}
		_tooltipTrigger = GetComponent<SimpleTooltipTrigger>();
		if (_tooltipTrigger == null)
		{
			Debug.LogWarning(base.gameObject.name + ": _tooltipTrigger is missing.");
		}
	}

	private void Start()
	{
		_manager = MilMo_Player.Instance.PlayerClassManager;
		if (_manager == null)
		{
			Debug.LogWarning("Unable to get PlayerClassManager");
			return;
		}
		_manager.OnClassSelectionChanged += UpdateShow;
		UpdateShow();
	}

	private void OnDestroy()
	{
		if (_manager != null)
		{
			_manager.OnClassSelectionChanged -= UpdateShow;
		}
	}

	private void UpdateShow()
	{
		if (_manager == null)
		{
			Hide();
		}
		else if (!_canShow)
		{
			Hide();
		}
		else if (_manager.HasAvailableClassSelection())
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	public override void SetHudVisibility(bool shouldShow)
	{
		_canShow = shouldShow;
		UpdateShow();
	}

	private void Show()
	{
		_tooltipTrigger.enabled = true;
		_fader.FadeIn();
		_shouldAnimate = true;
		StartScaleTween();
	}

	private void Hide()
	{
		_tooltipTrigger.enabled = false;
		_fader.FadeOut();
		_shouldAnimate = false;
	}

	public void Click()
	{
		int firstAvailableClassSelection = _manager.GetFirstAvailableClassSelection();
		if (firstAvailableClassSelection != -1)
		{
			DialogueSpawner.SpawnSelectClassDialogue(firstAvailableClassSelection);
		}
	}

	public void CombatStatusChange(bool combatStatus)
	{
		Color color = selectionImage.color;
		color.a = (combatStatus ? 0.5f : 1f);
		selectionImage.color = color;
		selectionButton.enabled = !combatStatus;
	}

	public void StartScaleTween()
	{
		if (scalePreset != null && _shouldAnimate)
		{
			_scale.Run(scalePreset);
		}
	}
}
