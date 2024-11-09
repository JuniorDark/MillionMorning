using System.Collections;
using Code.Core.Avatar;
using Code.World.CharBuilder;
using Core.Dependencies;
using UnityEngine;

namespace Core.Avatar;

public class AvatarEditorAnimations : MonoBehaviour
{
	[SerializeField]
	private DependencyLoader dependencyLoader;

	[SerializeField]
	private AvatarEditor avatarEditor;

	private bool _ready;

	private bool _error;

	private MilMo_ShopEmoteHandler _emoteHandler;

	private MilMo_Avatar _avatar;

	private string _currentCategory;

	private Coroutine _scheduledAutoEmote;

	public bool IsReady => _ready;

	public bool Failed => _error;

	private async void Start()
	{
		StartListeners();
		if (!(await dependencyLoader.LoadDependencies()))
		{
			Debug.LogError("Failed to meet dependencies!");
			_error = true;
		}
		else
		{
			_emoteHandler = new MilMo_ShopEmoteHandler();
			_ready = true;
		}
	}

	private void OnDestroy()
	{
		StopSchedule();
		StopListeners();
	}

	private void StartListeners()
	{
		avatarEditor.OnInitialized += PlayIntroEmote;
		avatarEditor.OnGenderChanged += UpdateCurrentAvatar;
		avatarEditor.OnAvatarApply += PlayApplyAnimations;
	}

	private void StopListeners()
	{
		avatarEditor.OnInitialized -= PlayIntroEmote;
		avatarEditor.OnGenderChanged -= UpdateCurrentAvatar;
		avatarEditor.OnAvatarApply -= PlayApplyAnimations;
	}

	private void StartEmotes()
	{
		RestartSchedule();
		PlayIdleAnimation();
	}

	public void SetCurrentCategory(string category)
	{
		_currentCategory = category;
		if (_avatar != null)
		{
			StartEmotes();
		}
	}

	private void UpdateCurrentAvatar()
	{
		_avatar = avatarEditor.CurrentAvatarHandler.GetAvatar();
		StartEmotes();
	}

	private void RestartSchedule()
	{
		StopSchedule();
		_scheduledAutoEmote = StartCoroutine(PlayScheduledAutoEmote());
	}

	private void StopSchedule()
	{
		if (_scheduledAutoEmote != null)
		{
			StopCoroutine(_scheduledAutoEmote);
			_scheduledAutoEmote = null;
		}
	}

	private IEnumerator PlayScheduledAutoEmote()
	{
		int num = Random.Range(8, 16);
		yield return new WaitForSeconds(num);
		PlayAutoEmote();
		RestartSchedule();
	}

	private void PlayIntroEmote()
	{
		_emoteHandler.PlayIntroEmote(_avatar);
	}

	private void PlayIdleAnimation()
	{
		_emoteHandler.PlayIdleAnimation(_avatar, _currentCategory);
	}

	private void PlayApplyAnimations()
	{
		if (_currentCategory == "EYES")
		{
			_emoteHandler.PlayRandomBlink(_avatar);
		}
		_emoteHandler.PlayFirstEmote(_avatar, _currentCategory);
	}

	private void PlayAutoEmote()
	{
		_emoteHandler.PlayAutoEmote(_avatar, _currentCategory);
	}
}
