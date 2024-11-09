using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.EventSystem;
using Code.Core.Network;
using Code.Core.ResourceSystem;
using Code.World.Achievements;
using Code.World.Player;
using Core;
using TMPro;
using UI.Elements.Badges;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Profile;

public class ProfileCard : MonoBehaviour
{
	[Serializable]
	public struct MemberIcon
	{
		public int days;

		public Texture2D texture;
	}

	[SerializeField]
	private Image portrait;

	[SerializeField]
	private Texture2D noPortrait;

	[SerializeField]
	private TMP_Text titleText;

	[SerializeField]
	private TMP_Dropdown titleDropdown;

	[SerializeField]
	private TMP_Text nameText;

	[SerializeField]
	private List<Badge> badges;

	[SerializeField]
	private List<MemberIcon> memberIcons;

	[SerializeField]
	private UnityEvent onTitleChangeSuccess;

	[SerializeField]
	private UnityEvent onTitleChangeFail;

	private ProfilePanel _profilePanel;

	private MilMo_Profile _profile;

	private List<string> _titles;

	private void Awake()
	{
		_titles = new List<string>();
		_profilePanel = GetComponentInParent<ProfilePanel>();
		if (!_profilePanel)
		{
			Debug.LogWarning(base.gameObject.name + ": Could not find ProfilePanel");
		}
		else if (titleDropdown == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing titleDropdown");
		}
		else
		{
			MilMo_EventSystem.Listen("player_title_change_failed", HandleChangeFail).Repeating = true;
		}
	}

	public void OnSelectionChanged(int val)
	{
		Singleton<GameNetwork>.Instance.RequestChangeTitle(_titles[val]);
	}

	private void HandleChangeFail(object t)
	{
		onTitleChangeFail?.Invoke();
		RefreshTitle();
	}

	private void OnTitleChange()
	{
		if (_profile != null)
		{
			if (_profile.isMe)
			{
				onTitleChangeSuccess?.Invoke();
			}
			RefreshTitle();
		}
	}

	private void OnEnable()
	{
		if ((bool)_profilePanel)
		{
			_profile = _profilePanel.profile;
			MilMo_Profile profile = _profile;
			profile.OnTitleChange = (Action)Delegate.Combine(profile.OnTitleChange, new Action(OnTitleChange));
			if (_profile != null && !string.IsNullOrEmpty(_profile.playerId))
			{
				RefreshCard();
			}
		}
	}

	private void OnDisable()
	{
		MilMo_Profile profile = _profile;
		profile.OnTitleChange = (Action)Delegate.Remove(profile.OnTitleChange, new Action(OnTitleChange));
	}

	private void RefreshTitleDropdown()
	{
		MilMo_Profile profile = _profile;
		if (profile == null || !profile.isMe || titleDropdown == null)
		{
			return;
		}
		List<MilMo_MedalCategory> list = Singleton<MilMo_AchievementHandler>.Instance.GetMedalCategories()?.ToList();
		if (list == null)
		{
			return;
		}
		_titles.Clear();
		titleDropdown.ClearOptions();
		List<TMP_Dropdown.OptionData> list2 = new List<TMP_Dropdown.OptionData>();
		int valueWithoutNotify = 0;
		int num = 0;
		TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
		optionData.text = MilMo_Localization.GetLocString("ProfileWindow_NoTitle")?.String;
		list2.Add(optionData);
		_titles.Add("");
		num++;
		foreach (MilMo_Medal item in from medal in list.SelectMany((MilMo_MedalCategory category) => category.Medals)
			orderby medal.GetDisplayName()?.String
			select medal)
		{
			if (item.Acquired)
			{
				TMP_Dropdown.OptionData optionData2 = new TMP_Dropdown.OptionData();
				optionData2.text = item.Template.DisplayName?.String;
				if (MilMo_Localization.GetLocString(_profile.title).String == optionData2.text)
				{
					valueWithoutNotify = num;
				}
				_titles.Add(item.Identifier);
				list2.Add(optionData2);
				num++;
			}
		}
		titleDropdown.options = list2;
		titleDropdown.SetValueWithoutNotify(valueWithoutNotify);
		titleDropdown.RefreshShownValue();
	}

	private void RefreshCard()
	{
		RefreshPortrait();
		RefreshName();
		RefreshTitle();
		RefreshBadges();
	}

	private void RefreshPortrait()
	{
		if (portrait == null)
		{
			return;
		}
		SetTexture(noPortrait);
		MilMo_ProfileManager.GetPlayerPortrait(_profile.playerId, delegate(bool success, string playerId, Texture2D texture)
		{
			if (success)
			{
				SetTexture(texture);
			}
			else
			{
				Debug.LogWarning(base.gameObject.name + ": Failed to create portrait for: " + playerId);
			}
		});
	}

	private void SetTexture(Texture2D newTexture)
	{
		if (!(portrait == null) && (bool)newTexture && (!portrait.sprite || portrait.sprite.texture != newTexture))
		{
			Vector2 pivot = portrait.rectTransform.pivot;
			Rect rect = new Rect(0f, 0f, newTexture.width, newTexture.height);
			Sprite sprite = Sprite.Create(newTexture, rect, pivot);
			portrait.sprite = sprite;
		}
	}

	private void RefreshName()
	{
		if (!(nameText == null))
		{
			nameText.text = _profile.avatarName;
			nameText.color = (_profile.isFriend ? Color.green : Color.white);
		}
	}

	private void RefreshTitle()
	{
		bool isMe = _profile.isMe;
		if (titleText == null)
		{
			return;
		}
		titleText.text = MilMo_Localization.GetLocString(_profile.title).String;
		titleText.gameObject.SetActive(!isMe);
		if (!(titleDropdown == null))
		{
			titleDropdown.gameObject.SetActive(isMe);
			if (isMe)
			{
				RefreshTitleDropdown();
			}
		}
	}

	private void RefreshBadges()
	{
		RefreshRoleBadge();
		RefreshLevelBadge();
		RefreshMemberBadge();
	}

	private void RefreshRoleBadge()
	{
		Badge badge = badges.FirstOrDefault((Badge b) => b.type == Badge.BadgeType.Role);
		if (!(badge == null))
		{
			if (_profile.role == 1)
			{
				badge.SetTooltipText(MilMo_Localization.GetLocString("ProfileWindow_6091").String);
			}
			if (_profile.role >= 2)
			{
				badge.SetTooltipText(MilMo_Localization.GetLocString("Generic_Admin").String);
			}
			badge.Show(_profile.role > 0);
		}
	}

	private void RefreshLevelBadge()
	{
		Badge badge = badges.FirstOrDefault((Badge b) => b.type == Badge.BadgeType.Level);
		if (!(badge == null))
		{
			int avatarLevel = _profile.avatarLevel;
			badge.SetText(avatarLevel.ToString());
			badge.SetTooltipText(MilMo_Localization.GetLocString("World_5543").String);
			badge.Show(avatarLevel > 0);
		}
	}

	private void RefreshMemberBadge()
	{
		Badge badge = badges.FirstOrDefault((Badge b) => b.type == Badge.BadgeType.Member);
		if (badge == null)
		{
			return;
		}
		bool flag = _profile.member && _profile.memberDaysLeft > 0;
		if (flag)
		{
			Texture2D icon = null;
			foreach (MemberIcon memberIcon in memberIcons)
			{
				if (memberIcon.days > _profile.memberDaysLeft)
				{
					break;
				}
				icon = memberIcon.texture;
			}
			badge.SetIcon(icon);
			badge.SetTooltipText(MilMo_Localization.GetLocString("WorldMap_4747").String);
		}
		badge.Show(flag);
	}
}
