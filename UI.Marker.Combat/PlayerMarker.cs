using Code.Core.Avatar;
using Code.Core.BuddyBackend;
using Code.Core.Network.nexus;
using Code.Core.ResourceSystem;
using Code.World.Level;
using Code.World.Player;
using Core;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace UI.Marker.Combat;

public class PlayerMarker : TargetMarker, IHasAvatar
{
	public new const string ADDRESSABLE_ADDRESS = "PlayerMarker";

	private const float SQR_LOD_DISTANCE = 2000f;

	private const float SQR_ICON_LOD_DISTANCE = 1600f;

	private const float SQR_TITLE_LOD_DISTANCE = 900f;

	[Header("PlayerSpecific")]
	[SerializeField]
	private HealthBar healthBar;

	[SerializeField]
	private TMP_Text targetTitle;

	[SerializeField]
	private Badges targetBadges;

	[SerializeField]
	private Image targetPlop;

	private MilMo_Avatar _avatar;

	private MilMo_Player PlayerInstance { get; set; }

	private GroupManager GroupInstance { get; set; }

	private MilMo_BuddyBackend BuddyBackend { get; set; }

	public void Initialize(MilMo_Avatar avatar)
	{
		if (avatar == null)
		{
			Debug.LogWarning(base.gameObject.name + ": avatar is null");
			return;
		}
		_avatar = avatar;
		PlayerInstance = MilMo_Player.Instance;
		GroupInstance = Singleton<GroupManager>.Instance;
		BuddyBackend = Singleton<MilMo_BuddyBackend>.Instance;
		base.Initialize(avatar);
		_avatar.OnHealthChanged += RefreshHealth;
		_avatar.OnAvatarTitleUpdated += RefreshTitle;
		if (GroupInstance != null)
		{
			GroupInstance.OnGroupChanged += RefreshColor;
		}
		if (BuddyBackend != null)
		{
			BuddyBackend.OnFriendAdded += FriendStatusChanged;
			BuddyBackend.OnFriendRemoved += FriendStatusChanged;
		}
		RefreshHealth();
		RefreshTitle();
		RefreshColor();
	}

	protected override void Awake()
	{
		base.Awake();
		if (healthBar == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find healthBar");
		}
		else if (targetTitle == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find targetTitle");
		}
		else if (targetBadges == null)
		{
			Debug.LogError(base.gameObject.name + ": Unable to find targetBadges");
		}
	}

	private void OnEnable()
	{
		LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
	}

	private void OnDisable()
	{
		LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
	}

	protected override void Update()
	{
		base.Update();
		bool flag = RefreshVisibility();
		if (Visible != flag)
		{
			Show(flag);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (_avatar != null)
		{
			_avatar.OnHealthChanged -= RefreshHealth;
			_avatar.OnAvatarTitleUpdated -= RefreshTitle;
			if (GroupInstance != null)
			{
				GroupInstance.OnGroupChanged -= RefreshColor;
			}
			if (BuddyBackend != null)
			{
				BuddyBackend.OnFriendAdded -= FriendStatusChanged;
				BuddyBackend.OnFriendRemoved -= FriendStatusChanged;
			}
		}
	}

	private void FriendStatusChanged(Friend obj)
	{
		RefreshColor();
	}

	private void OnLocaleChanged(Locale locale)
	{
		RefreshTitle();
	}

	private void RefreshColor()
	{
		if (_avatar != null)
		{
			SetMarkerTextColor(_avatar.TargetColor);
			SetPlopColor(_avatar.TargetColor);
		}
	}

	private void ShowBadges(bool shouldShow)
	{
		if (targetBadges != null)
		{
			targetBadges.Show(shouldShow);
		}
	}

	private void ShowHealthBar(bool shouldShow)
	{
		if (healthBar != null)
		{
			healthBar.Show(shouldShow);
		}
	}

	private void RefreshHealth()
	{
		if (_avatar != null && healthBar != null)
		{
			healthBar.UpdateHealth(_avatar.Health, _avatar.MaxHealth);
		}
	}

	private void ShowTitle(bool show)
	{
		if (!(targetTitle == null) && targetTitle.gameObject.activeSelf != show)
		{
			targetTitle.gameObject.SetActive(show);
		}
	}

	private void RefreshTitle()
	{
		if (!(targetTitle == null))
		{
			string identifier = ((_avatar != null) ? _avatar.Title : "");
			targetTitle.text = MilMo_Localization.GetLocString(identifier).String;
		}
	}

	private void ShowPlop(bool shouldShow)
	{
		if (!(targetPlop == null))
		{
			targetPlop.enabled = shouldShow;
		}
	}

	private void SetPlopColor(Color color)
	{
		if (targetPlop != null)
		{
			targetPlop.color = color;
		}
	}

	private bool RefreshVisibility()
	{
		if (PlayerInstance?.Avatar == null)
		{
			return false;
		}
		if (_avatar == null)
		{
			return false;
		}
		if (_avatar.Id == PlayerInstance.Avatar.Id)
		{
			return false;
		}
		if (PlayerInstance.InSinglePlayerArea)
		{
			return false;
		}
		MilMo_Player playerInstance = PlayerInstance;
		if (playerInstance != null && playerInstance.InSpline)
		{
			return false;
		}
		if (_avatar.Room != PlayerInstance.Avatar.Room)
		{
			return false;
		}
		if (MilMo_Player.InHome)
		{
			return false;
		}
		float sqrMagnitude = (PlayerInstance.Avatar.Head.position - base.transform.position).sqrMagnitude;
		ShowBadges(sqrMagnitude <= 1600f);
		ShowTitle(sqrMagnitude <= 900f);
		bool num = GroupInstance != null && GroupInstance.InGroup(_avatar.Id);
		bool flag = MilMo_Level.CurrentLevel != null && MilMo_Level.CurrentLevel.PvpHandler.IsTeamMate(_avatar.Id);
		bool shouldShow = (num || flag) && sqrMagnitude <= 900f;
		ShowHealthBar(shouldShow);
		ShowName(sqrMagnitude <= 2000f);
		ShowPlop(sqrMagnitude > 2000f);
		return true;
	}

	public MilMo_Avatar GetAvatar()
	{
		return _avatar;
	}
}
