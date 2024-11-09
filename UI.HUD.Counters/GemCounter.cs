using System;
using Core.State.Basic;
using UI.Tooltip.Triggers;
using UnityEngine;
using UnityEngine.Events;

namespace UI.HUD.Counters;

public class GemCounter : Counter
{
	[Header("States")]
	[SerializeField]
	private IntState playerTotalGems;

	[SerializeField]
	private IntState playerGemBonusTimeLeftInSeconds;

	[Header("Multiple tooltip setup")]
	[SerializeField]
	private SimpleTooltipTrigger tooltipTrigger;

	[SerializeField]
	private SimpleTooltipTrigger tooltipBonusTrigger;

	[Header("Events")]
	[SerializeField]
	private UnityEvent bonusModeEntered;

	private int _currentGemBonusModeSecondsLeft;

	private bool _hasBonus;

	private float _bonusModeEnds;

	private void OnEnable()
	{
		SyncWithState(0);
		UpdateTooltipText();
		playerTotalGems.OnChange += SyncWithState;
		playerGemBonusTimeLeftInSeconds.OnChange += SyncWithState;
	}

	private void OnDisable()
	{
		playerTotalGems.OnChange -= SyncWithState;
		playerGemBonusTimeLeftInSeconds.OnChange -= SyncWithState;
	}

	private void Update()
	{
		if (_currentGemBonusModeSecondsLeft <= 0)
		{
			return;
		}
		int num = (int)(_bonusModeEnds - Time.time);
		if (num != _currentGemBonusModeSecondsLeft)
		{
			_currentGemBonusModeSecondsLeft = num;
			if (!_hasBonus && _currentGemBonusModeSecondsLeft > 0)
			{
				bonusModeEntered?.Invoke();
			}
			_hasBonus = _currentGemBonusModeSecondsLeft > 0;
			UpdateTooltipText();
		}
	}

	private void SyncWithState(int newValue)
	{
		int num = playerTotalGems.Get();
		_currentGemBonusModeSecondsLeft = playerGemBonusTimeLeftInSeconds.Get();
		_bonusModeEnds = Time.time + (float)_currentGemBonusModeSecondsLeft;
		SetText($"{num:0}");
	}

	private void UpdateTooltipText()
	{
		if ((bool)tooltipTrigger && (bool)tooltipBonusTrigger)
		{
			if (_hasBonus)
			{
				tooltipTrigger.active = false;
				tooltipBonusTrigger.active = true;
			}
			else
			{
				tooltipTrigger.active = true;
				tooltipBonusTrigger.active = false;
			}
			tooltipTrigger.localizedString.RefreshString();
			TimeSpan timeSpan = TimeSpan.FromSeconds(_currentGemBonusModeSecondsLeft);
			string text = $"{timeSpan.Minutes}:{timeSpan.Seconds}";
			tooltipBonusTrigger.localizedString.Arguments = new object[1] { text };
			tooltipBonusTrigger.localizedString.RefreshString();
		}
	}
}
