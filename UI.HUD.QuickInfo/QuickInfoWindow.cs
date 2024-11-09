using System.Threading.Tasks;
using Core;
using Core.Audio.AudioData;
using TMPro;
using UI.Elements;
using UI.FX;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.HUD.QuickInfo;

public class QuickInfoWindow : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	[Header("Elements")]
	[SerializeField]
	private TMP_Text caption;

	[SerializeField]
	private TMP_Text message;

	[SerializeField]
	private TMP_Text callToActionLabel;

	[SerializeField]
	private Image icon;

	[Header("Sounds")]
	[SerializeField]
	protected UIAudioCueSO dialogueOpenSound;

	private UIAudioCueSO _openSound;

	[Header("Dialogue SO")]
	[SerializeField]
	protected QuickInfoSO quickInfoSO;

	private UIAlphaFX _fader;

	private Countdown _countdown;

	public event UnityAction OnShow = delegate
	{
	};

	public event UnityAction OnClose = delegate
	{
	};

	public void Init(QuickInfoSO so)
	{
		if (so == null)
		{
			Debug.LogError(base.name + ": DialogueSO is of wrong type");
			return;
		}
		quickInfoSO = so;
		SetCaption(so.GetCaption());
		SetMessage(so.GetMessage());
		SetCallToActionLabel(so.GetCallToLabel());
		SetSprite(so.GetSprite());
		SetSound(so.GetSound());
		OnShow += StartCountdown;
		OnClose += StopCountdown;
	}

	protected void Awake()
	{
		if (caption == null)
		{
			Debug.LogError(base.name + ": Missing caption");
			return;
		}
		caption.text = "";
		if (message == null)
		{
			Debug.LogError(base.name + ": Missing message");
			return;
		}
		message.text = "";
		if (callToActionLabel == null)
		{
			Debug.LogError(base.name + ": Missing callToAction");
			return;
		}
		callToActionLabel.text = "";
		if (icon == null)
		{
			Debug.LogError(base.name + ": Missing icon");
			return;
		}
		icon.enabled = false;
		_fader = GetComponent<UIAlphaFX>();
		if (!_fader)
		{
			Debug.LogWarning(base.name + ": Unable to find UIAlphaFX");
			return;
		}
		_countdown = GetComponent<Countdown>();
		if (!_countdown)
		{
			Debug.LogWarning(base.name + ": Unable to find Countdown");
		}
	}

	public async void Show()
	{
		int millisecondsDelay = 0;
		if (_fader != null)
		{
			_fader.FadeOutFast();
			_fader.FadeIn();
			millisecondsDelay = (int)(_fader.GetFadeInDuration() * 1000f);
		}
		base.gameObject.SetActive(value: true);
		if (dialogueOpenSound != null)
		{
			dialogueOpenSound.PlayAudioCue();
		}
		this.OnShow?.Invoke();
		await Task.Delay(millisecondsDelay);
	}

	public void Close()
	{
		Debug.LogWarning("Close");
		this.OnClose();
		Singleton<QuickInfoManager>.Instance.Close(quickInfoSO);
	}

	private async void DelayedClose()
	{
		Debug.LogWarning("DelayedClose");
		int millisecondsDelay = 0;
		if ((bool)_fader)
		{
			_fader.FadeOut();
			millisecondsDelay = (int)(_fader.GetFadeOutDuration() * 1000f);
		}
		await Task.Delay(millisecondsDelay);
		Close();
	}

	private void SetCaption(string newText)
	{
		if (!(caption == null))
		{
			caption.SetText(newText);
		}
	}

	private void SetMessage(string newText)
	{
		if (!(message == null))
		{
			message.SetText(newText);
		}
	}

	private void SetCallToActionLabel(string newText)
	{
		if (!(callToActionLabel == null))
		{
			callToActionLabel.SetText(newText);
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		quickInfoSO.CallToAction();
		Close();
	}

	private void SetSprite(Sprite sprite)
	{
		if (!(icon == null))
		{
			icon.enabled = false;
			if (!(sprite == null))
			{
				icon.sprite = sprite;
				icon.enabled = true;
			}
		}
	}

	private void SetSound(UIAudioCueSO sound)
	{
		if (!(sound == null))
		{
			_openSound = sound;
			OnShow += PlaySound;
		}
	}

	private void PlaySound()
	{
		_openSound.PlayAudioCue();
	}

	private void StartCountdown()
	{
		if ((bool)_countdown)
		{
			_countdown.Setup(quickInfoSO.GetLifetime(), null, DelayedClose);
			_countdown.StartCountdown();
		}
	}

	private void StopCountdown()
	{
		if ((bool)_countdown)
		{
			_countdown.StopCountdown();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if ((bool)_countdown)
		{
			_countdown.Pause();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if ((bool)_countdown)
		{
			_countdown.Resume();
		}
	}
}
