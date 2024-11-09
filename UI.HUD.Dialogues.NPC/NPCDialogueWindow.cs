using System.Threading.Tasks;
using Code.Core.EventSystem;
using Core.Audio.AudioData;
using TMPro;
using UI.FX;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.Dialogues.NPC;

public abstract class NPCDialogueWindow : DialogueWindow
{
	private NPCDialogueSO _npcDialogueSO;

	[Header("Actor")]
	[SerializeField]
	private Image actorIcon;

	[SerializeField]
	private TMP_Text actorName;

	[SerializeField]
	private UIAudioCueSO actorVoice;

	[Header("Message")]
	[SerializeField]
	private TMP_Text actorMessage;

	[SerializeField]
	private Button skipTextButton;

	[Header("Sounds")]
	[SerializeField]
	protected UIAudioCueSO dialogueOpenSound;

	[SerializeField]
	protected UIAudioCueSO dialogueCloseSound;

	[SerializeField]
	protected UIAudioCueSO dialogueTextSound;

	private UIAlphaFX _fader;

	private UIScaleFX _scaler;

	private string _actorMessageFullText;

	private bool _isAnimatingText;

	public override void Init(DialogueSO so)
	{
		_npcDialogueSO = (NPCDialogueSO)so;
		if (_npcDialogueSO == null)
		{
			Debug.LogError(base.name + ": DialogueSO is of wrong type");
			return;
		}
		base.Init(so);
		SetActorName(_npcDialogueSO.GetActorName());
		SetActorPortrait(_npcDialogueSO.GetActorPortrait());
		SetActorVoice(_npcDialogueSO.GetActorVoice());
		ClearButtons();
		ShowCloseButton(shouldShow: false);
		base.OnShow += StartProcessingNPCMessages;
		base.OnShow += StartTalking;
		base.OnClose += StopTalking;
	}

	protected virtual void Awake()
	{
		if (actorIcon == null)
		{
			Debug.LogError(base.name + ": Missing actorIcon");
			return;
		}
		actorIcon.enabled = false;
		if (actorName == null)
		{
			Debug.LogError(base.name + ": Missing actorName");
			return;
		}
		actorName.text = "";
		if (actorMessage == null)
		{
			Debug.LogError(base.name + ": Missing actorMessage");
			return;
		}
		actorMessage.text = "";
		if (skipTextButton == null)
		{
			Debug.LogWarning(base.name + ": Missing skipTextButton");
			return;
		}
		skipTextButton.onClick.AddListener(TextAnimationDone);
		_fader = GetComponentInChildren<UIAlphaFX>();
		if (!_fader)
		{
			Debug.LogWarning(base.name + ": Unable to find UIAlphaFX");
			return;
		}
		_scaler = GetComponentInChildren<UIScaleFX>();
		if (!_scaler)
		{
			Debug.LogWarning(base.name + ": Unable to find UIScaleFX");
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
		ReleaseController();
		base.Close();
	}

	private void SetActorName(string text)
	{
		if (!(actorName == null))
		{
			actorName.text = text;
		}
	}

	private void SetActorPortrait(Sprite sprite)
	{
		if (!(actorIcon == null))
		{
			actorIcon.enabled = false;
			if (!(sprite == null))
			{
				actorIcon.sprite = sprite;
				actorIcon.enabled = true;
			}
		}
	}

	private void SetActorVoice(UIAudioCueSO audioClip)
	{
		actorVoice = audioClip;
	}

	private void PlayVoice()
	{
		if (actorVoice != null)
		{
			actorVoice.PlayAudioCue();
		}
	}

	private void StartProcessingNPCMessages()
	{
		if (!(_npcDialogueSO == null))
		{
			_npcDialogueSO.ResetIndex();
			ShowNextMessage();
		}
	}

	protected void ShowNextMessage()
	{
		string nextMessage = _npcDialogueSO.GetNextMessage();
		RefreshActions();
		SetMessage(nextMessage);
	}

	private void SetMessage(string message)
	{
		_actorMessageFullText = message;
		StartTextAnimation();
	}

	private async void StartTextAnimation()
	{
		if (actorMessage == null || string.IsNullOrEmpty(_actorMessageFullText))
		{
			PlayVoice();
			TextAnimationDone();
			return;
		}
		_isAnimatingText = true;
		actorMessage.text = "";
		if (skipTextButton != null)
		{
			skipTextButton.gameObject.SetActive(value: true);
			EnableButtons(shouldEnable: false);
		}
		bool num = actorMessage.text.Length > 0;
		float num2 = ((_fader != null) ? _fader.GetFadeInDuration() : 0f);
		if (!num && num2 > 0f)
		{
			await Task.Delay((int)(num2 * 1000f));
		}
		PlayVoice();
		string shownText = "";
		for (int i = 0; i < _actorMessageFullText.Length; i++)
		{
			if (!_isAnimatingText)
			{
				return;
			}
			shownText += _actorMessageFullText[i];
			actorMessage.text = shownText;
			Sprite emote = _npcDialogueSO.GetEmote(i);
			if (emote != null)
			{
				SetActorPortrait(emote);
			}
			if (dialogueTextSound != null)
			{
				dialogueTextSound.PlayAudioCue();
			}
			await Task.Delay(20);
		}
		TextAnimationDone();
	}

	private void TextAnimationDone()
	{
		_isAnimatingText = false;
		Sprite lastEmote = _npcDialogueSO.GetLastEmote();
		if (lastEmote != null)
		{
			SetActorPortrait(lastEmote);
		}
		if (actorMessage != null && !string.IsNullOrEmpty(_actorMessageFullText))
		{
			actorMessage.text = _actorMessageFullText;
		}
		if (skipTextButton != null)
		{
			skipTextButton.gameObject.SetActive(value: false);
		}
		EnableButtons(shouldEnable: true);
		ShowCloseButton(shouldShow: true);
	}

	private void StartTalking()
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_TalkTo", _npcDialogueSO.GetNPCId());
	}

	private void StopTalking()
	{
		MilMo_EventSystem.Instance.AsyncPostEvent("tutorial_StopTalkTo", _npcDialogueSO.GetNPCId());
	}
}
