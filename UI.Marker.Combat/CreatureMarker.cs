using System;
using Code.World.Level;
using Code.World.Level.LevelObject;
using Code.World.Player;
using TMPro;
using UnityEngine;

namespace UI.Marker.Combat;

public class CreatureMarker : TargetMarker
{
	public new const string ADDRESSABLE_ADDRESS = "CreatureMarker";

	private const float MAX_TEXT_DISTANCE = 100f;

	[Header("CreatureSpecific")]
	[SerializeField]
	private HealthBar healthBar;

	[SerializeField]
	protected TMP_Text targetLevel;

	private const int UNKNOWN = -1;

	private const int VERY_EASY = 0;

	private const int EASY = 1;

	private const int NORMAL = 2;

	private const int HARD = 3;

	private const int VERY_HARD = 4;

	private readonly Color[] _difficultyColors = new Color[5]
	{
		Color.gray,
		Color.white,
		Color.green,
		new Color(1f, 0.6f, 0f, 1f),
		Color.red
	};

	private bool _showHealthBar;

	private MilMo_Player PlayerInstance { get; set; }

	public override void Initialize(IMilMo_AttackTarget target)
	{
		base.Initialize(target);
		if (Target == null)
		{
			Debug.LogWarning(base.gameObject.name + ": target is null");
			return;
		}
		PlayerInstance = MilMo_Player.Instance;
		if (PlayerInstance != null)
		{
			MilMo_Player playerInstance = PlayerInstance;
			playerInstance.OnCurrentAvatarLevelExpRequirementUpdated = (Action<int>)Delegate.Combine(playerInstance.OnCurrentAvatarLevelExpRequirementUpdated, new Action<int>(OnPlayerLevelChanged));
		}
		Target.OnHealthChanged += RefreshHealth;
		RefreshHealth();
		RefreshTargetLevel();
		UpdateHealthBarColor();
	}

	protected override void Awake()
	{
		base.Awake();
		if (healthBar == null)
		{
			Debug.LogError(base.gameObject.name + ": healthBar is null");
		}
		if (targetLevel == null)
		{
			Debug.LogError(base.gameObject.name + ": targetLevel is null");
		}
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
		if (Target != null)
		{
			Target.OnHealthChanged -= RefreshHealth;
			if (PlayerInstance != null)
			{
				MilMo_Player playerInstance = PlayerInstance;
				playerInstance.OnCurrentAvatarLevelExpRequirementUpdated = (Action<int>)Delegate.Remove(playerInstance.OnCurrentAvatarLevelExpRequirementUpdated, new Action<int>(OnPlayerLevelChanged));
			}
		}
	}

	private void OnPlayerLevelChanged(int _)
	{
		RefreshTargetLevel();
		UpdateHealthBarColor();
	}

	public override void Show(bool shouldShow)
	{
		base.Show(shouldShow);
		ShowTargetArrow(shouldShow);
	}

	private void ShowHealthBar(bool shouldShow)
	{
		if (!(healthBar == null))
		{
			healthBar.Show(shouldShow);
		}
	}

	private void RefreshHealth()
	{
		if (Target != null && healthBar != null)
		{
			healthBar.UpdateHealth(Target.Health, Target.MaxHealth);
		}
	}

	private void UpdateHealthBarColor()
	{
		if (!(healthBar == null))
		{
			int playerLevel = PlayerInstance?.AvatarLevel ?? 0;
			int difficulty = GetDifficulty(playerLevel);
			if (difficulty >= 0)
			{
				Color fillColor = _difficultyColors[difficulty];
				healthBar.SetFillColor(fillColor);
			}
		}
	}

	private int GetDifficulty(int playerLevel)
	{
		if (Target == null)
		{
			return -1;
		}
		if (Target.AvatarLevel > playerLevel + 4)
		{
			return 4;
		}
		if (Target.AvatarLevel < playerLevel - 4)
		{
			return 0;
		}
		if (Target.AvatarLevel >= playerLevel + 2 && Target.AvatarLevel <= playerLevel + 3)
		{
			return 3;
		}
		if (Target.AvatarLevel >= playerLevel - 1 && Target.AvatarLevel <= playerLevel + 1)
		{
			return 2;
		}
		if (Target.AvatarLevel <= playerLevel - 2 && Target.AvatarLevel >= playerLevel - 3)
		{
			return 1;
		}
		return -1;
	}

	private void RefreshTargetLevel()
	{
		if (Target != null && targetLevel != null)
		{
			targetLevel.text = Target.AvatarLevel.ToString();
		}
	}

	private bool RefreshVisibility()
	{
		if (Target == null)
		{
			return false;
		}
		if (MilMo_Level.CurrentLevel == null)
		{
			return false;
		}
		if (PlayerInstance?.Avatar == null)
		{
			return false;
		}
		float sqrMagnitude = (PlayerInstance.Avatar.Head.position - base.transform.position).sqrMagnitude;
		bool num = Target.IsBoss();
		bool flag = Target.IsCritter();
		bool shouldShow = num || (!flag && sqrMagnitude <= 100f);
		ShowName(shouldShow);
		ShowHealthBar(shouldShow);
		return Visible;
	}
}
