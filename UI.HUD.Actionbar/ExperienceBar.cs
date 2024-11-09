using System;
using System.Collections.Generic;
using Code.World.Player;
using Core.GameEvent;
using Core.Utilities;
using TMPro;
using UI.Tooltip.Data;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.Actionbar;

public class ExperienceBar : MonoBehaviour
{
	[Header("Elements")]
	[SerializeField]
	private Slider progress;

	[SerializeField]
	private TMP_Text levelIconText;

	[SerializeField]
	private XPSpawner xpSpawner;

	private const int NO_EXPERIENCE_TO_LEVEL = -1;

	private int _currentExp;

	private int _currentAvatarLevelExpRequirement;

	private int _nextAvatarLevelExpRequirement;

	private int _currentLevel;

	private MilMo_Player PlayerInstance => MilMo_Player.Instance;

	private void Awake()
	{
		if (progress == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing progress");
		}
		if (levelIconText == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing levelIconText");
		}
		if (xpSpawner == null)
		{
			Debug.LogWarning(base.gameObject.name + ": Missing xpSpawner");
		}
	}

	private void OnEnable()
	{
		if (PlayerInstance?.Avatar != null)
		{
			MilMo_Player playerInstance = PlayerInstance;
			playerInstance.OnCurrentAvatarLevelExpRequirementUpdated = (Action<int>)Delegate.Combine(playerInstance.OnCurrentAvatarLevelExpRequirementUpdated, new Action<int>(UpdateCurrentAvatarLevelExpRequirement));
			MilMo_Player playerInstance2 = PlayerInstance;
			playerInstance2.OnNextAvatarLevelExpRequirementUpdated = (Action<int>)Delegate.Combine(playerInstance2.OnNextAvatarLevelExpRequirementUpdated, new Action<int>(UpdateNextAvatarLevelExpRequirement));
			PlayerInstance.Avatar.OnAvatarLevelUpdated += UpdateLevel;
			MilMo_Player playerInstance3 = PlayerInstance;
			playerInstance3.OnExpUpdated = (Action<int>)Delegate.Combine(playerInstance3.OnExpUpdated, new Action<int>(UpdateCurrentExp));
			SetCurrentValues();
		}
	}

	private void SetCurrentValues()
	{
		if (PlayerInstance != null)
		{
			_currentExp = PlayerInstance.Exp;
			_currentAvatarLevelExpRequirement = PlayerInstance.CurrentAvatarLevelExpRequirement;
			_nextAvatarLevelExpRequirement = PlayerInstance.NextAvatarLevelExpRequirement;
			_currentLevel = PlayerInstance.AvatarLevel;
			UpdateProgressbar();
			UpdateLevelIconText();
		}
	}

	private void OnDisable()
	{
		if (PlayerInstance?.Avatar != null)
		{
			MilMo_Player playerInstance = PlayerInstance;
			playerInstance.OnCurrentAvatarLevelExpRequirementUpdated = (Action<int>)Delegate.Remove(playerInstance.OnCurrentAvatarLevelExpRequirementUpdated, new Action<int>(UpdateCurrentAvatarLevelExpRequirement));
			MilMo_Player playerInstance2 = PlayerInstance;
			playerInstance2.OnNextAvatarLevelExpRequirementUpdated = (Action<int>)Delegate.Remove(playerInstance2.OnNextAvatarLevelExpRequirementUpdated, new Action<int>(UpdateNextAvatarLevelExpRequirement));
			PlayerInstance.Avatar.OnAvatarLevelUpdated -= UpdateLevel;
			MilMo_Player playerInstance3 = PlayerInstance;
			playerInstance3.OnExpUpdated = (Action<int>)Delegate.Remove(playerInstance3.OnExpUpdated, new Action<int>(UpdateCurrentExp));
		}
	}

	private void UpdateLevel(int level)
	{
		_currentLevel = level;
		UpdateProgressbar();
		UpdateLevelIconText();
	}

	private void UpdateLevelIconText()
	{
		levelIconText.text = $"{_currentLevel}";
	}

	private void UpdateProgressbar()
	{
		float num = Mathf.Max(0.01f, _currentExp - _currentAvatarLevelExpRequirement);
		float num2 = Mathf.Max(1.01f, _nextAvatarLevelExpRequirement - _currentAvatarLevelExpRequirement);
		float value = ((_nextAvatarLevelExpRequirement == -1) ? 1f : (num / num2));
		if (progress != null)
		{
			progress.value = value;
		}
	}

	private void UpdateCurrentExp(int newExp)
	{
		int xp = newExp - _currentExp;
		_currentExp = newExp;
		UpdateProgressbar();
		if (xpSpawner != null)
		{
			xpSpawner.Spawn(xp);
		}
	}

	private void UpdateCurrentAvatarLevelExpRequirement(int requiredExperience)
	{
		_currentAvatarLevelExpRequirement = requiredExperience;
	}

	private void UpdateNextAvatarLevelExpRequirement(int newExp)
	{
		_nextAvatarLevelExpRequirement = newExp;
	}

	public void ShowExperienceTooltip()
	{
		string localizedString = LocalizationHelper.GetLocalizedString("Profile", "Profile_Experience");
		TooltipData args = ((_nextAvatarLevelExpRequirement == -1) ? new TooltipData($"{_currentExp:N0} {localizedString}") : new TooltipData($"{_currentExp:N0} / {_nextAvatarLevelExpRequirement:N0} {localizedString}"));
		GameEvent.ShowTooltipEvent?.RaiseEvent(args);
	}

	public void ShowLevelTooltip()
	{
		TooltipData args = new TooltipData(LocalizationHelper.GetLocalizedString("Profile", "Profile_LevelTooltip", new List<string> { _currentLevel.ToString() }));
		GameEvent.ShowTooltipEvent?.RaiseEvent(args);
	}

	public void HideTooltip()
	{
		GameEvent.HideTooltipEvent?.RaiseEvent();
	}
}
