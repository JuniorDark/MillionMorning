using System.Threading.Tasks;
using Core.Audio.AudioData;
using Core.Utilities;
using UI.FX;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.Dialogues.Feed;

public abstract class FeedDialogueWindow : DialogueWindow
{
	private FeedDialogueSO _feedDialogueSO;

	[Header("Sounds")]
	[SerializeField]
	protected UIAudioCueSO dialogueOpenSound;

	[SerializeField]
	protected UIAudioCueSO dialogueCloseSound;

	private UIAlphaFX _fader;

	private UISlidingFX _slider;

	public override void Init(DialogueSO so)
	{
		_feedDialogueSO = (FeedDialogueSO)so;
		if (_feedDialogueSO == null)
		{
			Debug.LogError(base.name + ": DialogueSO is of wrong type");
			return;
		}
		base.Init(so);
		ClearButtons();
		base.OnShow += StartProcessingFeed;
	}

	protected virtual void Awake()
	{
		_fader = GetComponentInChildren<UIAlphaFX>();
		if (!_fader)
		{
			Debug.LogWarning(base.name + ": Unable to find UIAlphaFX");
			return;
		}
		_slider = GetComponentInChildren<UISlidingFX>();
		if (!_slider)
		{
			Debug.LogWarning(base.name + ": Unable to find UISlidingFX");
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
		if ((bool)_slider)
		{
			_slider.SlideIn();
		}
		if (dialogueOpenSound != null)
		{
			dialogueOpenSound.PlayAudioCue();
		}
		await Task.Delay(millisecondsDelay);
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
		if ((bool)_slider)
		{
			_slider.SlideOut();
		}
		if (dialogueCloseSound != null)
		{
			dialogueCloseSound.PlayAudioCue();
		}
		await Task.Delay(millisecondsDelay);
		base.Close();
	}

	private void StartProcessingFeed()
	{
		RefreshHeader();
		RefreshBody();
		RefreshFooter();
	}

	protected abstract void RefreshHeader();

	protected abstract void RefreshBody();

	protected abstract void RefreshFooter();

	protected internal void SetIcon(Image target, Texture2D texture2D)
	{
		if (!(target == null))
		{
			target.enabled = false;
			if (!(texture2D == null))
			{
				Core.Utilities.UI.SetIcon(target, texture2D);
			}
		}
	}

	protected void SpawnFlyingIcon(Component source, Transform container, Transform destination)
	{
		if (!(source == null) && !(container == null) && !(destination == null))
		{
			Component component = Instantiator.Clone(source, container);
			component.transform.position = source.transform.position;
			component.gameObject.AddComponent<UIMoverFX>().MoveToTargetAndDie(destination);
		}
	}
}
