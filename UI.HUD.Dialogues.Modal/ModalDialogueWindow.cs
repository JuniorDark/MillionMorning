using System.Threading.Tasks;
using Core.Audio.AudioData;
using JetBrains.Annotations;
using TMPro;
using UI.Elements;
using UI.FX;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.Dialogues.Modal;

[SelectionBase]
public class ModalDialogueWindow : DialogueWindow
{
	[Header("Elements")]
	[SerializeField]
	private TMP_Text caption;

	[SerializeField]
	[CanBeNull]
	private TMP_Text message;

	[SerializeField]
	[CanBeNull]
	private Image icon;

	[Header("Sounds")]
	[SerializeField]
	protected UIAudioCueSO dialogueOpenSound;

	[SerializeField]
	protected UIAudioCueSO dialogueCloseSound;

	[Header("Dialogue SO")]
	[SerializeField]
	protected ModalDialogueSO modalDialogueSO;

	private UIAlphaFX _fader;

	private UIScaleFX _scaler;

	private Countdown _countdown;

	public override async void Init(DialogueSO so)
	{
		modalDialogueSO = (ModalDialogueSO)so;
		if (modalDialogueSO == null)
		{
			Debug.LogError(base.name + ": DialogueSO is of wrong type");
			return;
		}
		ClearButtons();
		base.Init(so);
		SetCaption(modalDialogueSO.GetCaption());
		SetIcon(await modalDialogueSO.GetIcon());
		if (closeButton != null)
		{
			closeButton.onClick.AddListener(Cancel);
		}
		base.OnShow += StartProcessingModal;
		base.OnClose += StopCountdown;
	}

	private void Cancel()
	{
		modalDialogueSO.Cancel();
	}

	protected virtual void Awake()
	{
		if (caption == null)
		{
			Debug.LogError(base.name + ": Missing caption");
			return;
		}
		caption.text = "";
		SetMessage("");
		ShowIconPart(shouldShow: false);
		_fader = GetComponentInChildren<UIAlphaFX>();
		if (!_fader)
		{
			Debug.LogWarning(base.name + ": Unable to find UIAlphaFX");
		}
		_scaler = GetComponentInChildren<UIScaleFX>();
		if (!_scaler)
		{
			Debug.LogWarning(base.name + ": Unable to find UIScaleFX");
		}
		_countdown = GetComponent<Countdown>();
		if (!_countdown)
		{
			Debug.LogWarning(base.name + ": Unable to find Countdown");
		}
	}

	public override async void Show()
	{
		int millisecondsDelay = 0;
		if (_fader != null)
		{
			_fader.FadeOutFast();
			_fader.FadeIn();
			millisecondsDelay = (int)(_fader.GetFadeInDuration() * 1000f);
		}
		base.gameObject.SetActive(value: true);
		if ((bool)_scaler)
		{
			_scaler.Grow();
		}
		if (dialogueOpenSound != null)
		{
			dialogueOpenSound.PlayAudioCue();
		}
		await Task.Delay(millisecondsDelay);
		LockController();
		base.Show();
	}

	public override async void Close()
	{
		int millisecondsDelay = 0;
		if ((bool)_fader)
		{
			_fader.FadeOut();
			millisecondsDelay = (int)(_fader.GetFadeOutDuration() * 1000f);
		}
		if ((bool)_scaler)
		{
			_scaler.Shrink();
		}
		if (dialogueCloseSound != null)
		{
			dialogueCloseSound.PlayAudioCue();
		}
		await Task.Delay(millisecondsDelay);
		if (closeButton != null)
		{
			closeButton.onClick.RemoveListener(Cancel);
		}
		ReleaseController();
		base.Close();
	}

	private void StartProcessingModal()
	{
		RefreshActions();
		SetMessage(modalDialogueSO.GetMessage());
		if (modalDialogueSO.GetLifetime() > 0)
		{
			StartCountdown();
		}
		EnableButtons(shouldEnable: true);
	}

	private void SetCaption(string newText)
	{
		if (!(caption == null))
		{
			caption.SetText(newText);
		}
	}

	private void SetIcon(Sprite sprite)
	{
		ShowIconPart(shouldShow: false);
		if (!(sprite == null))
		{
			if (icon == null)
			{
				Debug.LogError(base.name + ": Missing icon");
				return;
			}
			icon.sprite = sprite;
			ShowIconPart(shouldShow: true);
		}
	}

	private void ShowIconPart(bool shouldShow)
	{
		if (icon != null)
		{
			icon.transform.parent.gameObject.SetActive(shouldShow);
		}
	}

	private void SetMessage(string newText)
	{
		if (!(message == null))
		{
			message.text = newText;
		}
	}

	private void StartCountdown()
	{
		if ((bool)_countdown)
		{
			_countdown.Setup(modalDialogueSO.GetLifetime(), Tick, base.DefaultChoice);
			_countdown.StartCountdown();
		}
	}

	private void Tick(int secondsLeft)
	{
		SetMessage(modalDialogueSO.GetMessage(secondsLeft));
	}

	private void StopCountdown()
	{
		if ((bool)_countdown)
		{
			_countdown.StopCountdown();
		}
	}
}
