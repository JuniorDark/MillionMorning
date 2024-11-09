using System.Linq;
using Code.World.Player;
using Player.Moods;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Profile;

public class ProfileMood : MonoBehaviour
{
	[SerializeField]
	private Image moodIcon;

	[SerializeField]
	private Button prev;

	[SerializeField]
	private Button next;

	[SerializeField]
	private TMP_Text moodEditText;

	[SerializeField]
	private TMP_Text moodDisplayText;

	[SerializeField]
	private GameObject moodEdit;

	[SerializeField]
	private GameObject moodDisplay;

	[SerializeField]
	private MoodConfiguration moodConfiguration;

	private Mood _currentMood;

	private ProfilePanel _profilePanel;

	private MilMo_Profile _profile;

	private MilMo_MoodHandler _moodHandler;

	private void Awake()
	{
		_profilePanel = GetComponentInParent<ProfilePanel>();
		if (!_profilePanel)
		{
			Debug.LogWarning(base.name + ": Could not find ProfilePanel");
		}
		else if (moodIcon == null)
		{
			Debug.LogWarning(base.name + ": Missing moodIcon");
		}
		else if (prev == null)
		{
			Debug.LogWarning(base.name + ": Missing prev");
		}
		else if (next == null)
		{
			Debug.LogWarning(base.name + ": Missing next");
		}
		else if (moodEditText == null)
		{
			Debug.LogWarning(base.name + ": Missing moodEditText");
		}
		else if (moodDisplayText == null)
		{
			Debug.LogWarning(base.name + ": Missing moodDisplayText");
		}
		else if (moodEdit == null)
		{
			Debug.LogWarning(base.name + ": Missing moodEdit");
		}
		else if (moodDisplay == null)
		{
			Debug.LogWarning(base.name + ": Missing moodDisplay");
		}
		else if (moodConfiguration == null)
		{
			Debug.LogWarning(base.name + ": Missing moods");
		}
	}

	private void OnEnable()
	{
		_profile = ((_profilePanel != null) ? _profilePanel.profile : null);
		_moodHandler = _profile?.GetPlayer()?.Avatar?.MoodHandler;
		_currentMood = ((_profile != null) ? moodConfiguration.GetMoodByKey(_profile.mood) : moodConfiguration.GetMoods().FirstOrDefault());
		RefreshButtons();
		RefreshMood();
	}

	private void RefreshButtons()
	{
		if (!(prev == null) && !(next == null) && !(moodEdit == null) && !(moodDisplay == null))
		{
			MilMo_Profile profile = _profile;
			bool flag = profile == null || profile.isMe;
			moodEdit.gameObject.SetActive(flag);
			prev.gameObject.SetActive(flag);
			next.gameObject.SetActive(flag);
			moodDisplay.gameObject.SetActive(!flag);
		}
	}

	private void RefreshMood()
	{
		if (!(_currentMood == null))
		{
			SetText(_currentMood.GetName());
			SetIcon(_currentMood.GetSprite());
		}
	}

	private void SetText(string newText)
	{
		if (moodEditText != null)
		{
			moodEditText.SetText(newText);
		}
		if (moodDisplayText != null)
		{
			moodDisplayText.SetText(newText);
		}
	}

	private void SetIcon(Sprite newSprite)
	{
		if (!(moodIcon == null) && (bool)newSprite)
		{
			moodIcon.sprite = newSprite;
		}
	}

	public void OnNext()
	{
		Mood nextMood = moodConfiguration.GetNextMood(_currentMood);
		SetMood(nextMood);
	}

	public void OnPrev()
	{
		Mood previousMood = moodConfiguration.GetPreviousMood(_currentMood);
		SetMood(previousMood);
	}

	private void SetMood(Mood newMood)
	{
		_currentMood = newMood;
		if (_profile != null)
		{
			_profile.mood = newMood.GetKey();
		}
		_moodHandler?.SetMood(newMood, send: true, persist: true);
		RefreshMood();
	}
}
