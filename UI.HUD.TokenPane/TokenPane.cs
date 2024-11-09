using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.HUD.TokenPane;

public class TokenPane : MonoBehaviour
{
	[Serializable]
	public struct ReplacementIcons
	{
		public int threshold;

		public Sprite icon;
	}

	[Header("Elements")]
	[SerializeField]
	private TMP_Text text;

	[SerializeField]
	private Image icon;

	[Header("Reactions")]
	[SerializeField]
	private UnityEvent onTriggered;

	[SerializeField]
	private UnityEvent onUpdatedValue;

	[SerializeField]
	private UnityEvent onDone;

	[Header("Extra")]
	[SerializeField]
	private ReplacementIcons[] replacementIcons;

	[SerializeField]
	private Sprite rewardIcon;

	private int _currentAnimationID;

	private int _displayedGain;

	private int _currentGain;

	private void Awake()
	{
		onDone?.Invoke();
	}

	private void OnEnable()
	{
		ResetGain();
	}

	private void OnDisable()
	{
		ResetGain();
	}

	public void OnGain(int gain)
	{
		if (_currentGain == 0)
		{
			LeanTween.rotateZ(icon.gameObject, -45f, 0f);
			LeanTween.rotateZ(icon.gameObject, 0f, 1f);
		}
		_currentGain += gain;
		ReplacementIcons replacementIcons = this.replacementIcons.LastOrDefault((ReplacementIcons pair) => _currentGain >= pair.threshold);
		if ((bool)replacementIcons.icon)
		{
			icon.sprite = replacementIcons.icon;
		}
		float time = Math.Min((float)_currentGain - (float)gain * 0.5f, 4f);
		if (_currentAnimationID != 0)
		{
			LeanTween.cancel(_currentAnimationID);
		}
		_currentAnimationID = LeanTween.value(base.gameObject, _displayedGain, _currentGain, time).setOnUpdate(delegate(float value)
		{
			if ((int)value != _displayedGain)
			{
				if ((bool)rewardIcon)
				{
					icon.sprite = rewardIcon;
				}
				_displayedGain = (int)value;
				SetText($"{_displayedGain:0}");
				onUpdatedValue?.Invoke();
			}
		}).setDelay(0.6f)
			.setOnComplete((Action)delegate
			{
				_currentAnimationID = LeanTween.delayedCall(1f, (Action)delegate
				{
					_currentAnimationID = 0;
					onDone?.Invoke();
				}).id;
			})
			.id;
		onTriggered?.Invoke();
	}

	private void ResetGain()
	{
		_currentGain = 0;
		_displayedGain = 0;
		SetText("");
	}

	private void SetText(string newValue)
	{
		text.text = newValue;
	}
}
