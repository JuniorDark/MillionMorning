using Code.Core.BuddyBackend;
using Code.Core.EventSystem;
using Code.Core.ResourceSystem;
using Code.World.Player;
using Core;
using Core.Input;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI.Profile;

public class ProfileTweet : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField inputField;

	private ProfilePanel _profilePanel;

	private MilMo_Profile _profile;

	public UnityEvent onSuccess;

	public UnityEvent onFail;

	private void Awake()
	{
		_profilePanel = GetComponentInParent<ProfilePanel>();
		if (!_profilePanel)
		{
			Debug.LogWarning(base.gameObject.name + ": Could not find ProfilePanel");
		}
		MilMo_EventSystem.Listen("yourtweet", OnYourTweet).Repeating = true;
	}

	private void OnYourTweet(object data)
	{
	}

	private void OnEnable()
	{
		_profile = ((_profilePanel != null) ? _profilePanel.profile : null);
		RefreshInputField();
	}

	public void OnSelect()
	{
		if (Singleton<InputController>.Instance != null)
		{
			Singleton<InputController>.Instance.SetInputController();
		}
	}

	public void OnDeselect()
	{
		if (Singleton<InputController>.Instance != null)
		{
			Singleton<InputController>.Instance.RestorePreviousController();
		}
	}

	private void RefreshInputField()
	{
		if (!(inputField == null))
		{
			if (_profile == null)
			{
				inputField.enabled = false;
				return;
			}
			inputField.enabled = true;
			bool isMe = _profile.isMe;
			inputField.interactable = isMe;
			inputField.text = (isMe ? MilMo_Player.Instance.Tweet : _profile.tweet);
		}
	}

	public void OnEndEdit()
	{
		EventSystem.current.SetSelectedGameObject(null);
		string text = inputField.text;
		MilMo_BadWordFilter.StringIntegrity stringIntegrity = MilMo_BadWordFilter.GetStringIntegrity(text);
		if (stringIntegrity == MilMo_BadWordFilter.StringIntegrity.Bad || stringIntegrity == MilMo_BadWordFilter.StringIntegrity.IRLContactAttempt)
		{
			onFail?.Invoke();
			return;
		}
		Singleton<MilMo_BuddyBackend>.Instance.SendChangeTweet(text);
		MilMo_EventSystem.Instance.PostEvent("yourtweet", text);
		onSuccess?.Invoke();
	}
}
