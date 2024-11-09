using System;
using UnityEngine;
using UnityEngine.Events;

namespace UI.FX;

public class UIScaleFX : MonoBehaviour
{
	private int _currentAnimationID;

	private Vector3 _defaultScale = Vector3.one;

	[SerializeField]
	private UnityEvent OnComplete;

	private void Awake()
	{
		_defaultScale = base.transform.localScale;
	}

	public void Run(UIScaleFXPresetSO preset)
	{
		CancelAnyCurrentAnimation();
		_currentAnimationID = LeanTween.scale(base.gameObject, preset.impulseScale, preset.inTime).setEase(preset.inEase).setOnComplete((Action)delegate
		{
			_currentAnimationID = LeanTween.scale(base.gameObject, _defaultScale, preset.outTime).setEase(preset.outEase).setDelay(preset.onTime)
				.setOnComplete((Action)delegate
				{
					_currentAnimationID = 0;
					OnComplete?.Invoke();
				})
				.id;
		})
			.id;
	}

	public void Grow()
	{
		CancelAnyCurrentAnimation();
		_currentAnimationID = LeanTween.scale(base.gameObject, _defaultScale * 0.8f, 0f).setOnComplete((Action)delegate
		{
			_currentAnimationID = LeanTween.scale(base.gameObject, _defaultScale, 0.4f).setEase(LeanTweenType.easeInOutBack).setOnComplete((Action)delegate
			{
				_currentAnimationID = 0;
				OnComplete?.Invoke();
			})
				.id;
		}).id;
	}

	public void Shrink()
	{
		CancelAnyCurrentAnimation();
		_currentAnimationID = LeanTween.scale(base.gameObject, _defaultScale, 0f).setOnComplete((Action)delegate
		{
			_currentAnimationID = LeanTween.scale(base.gameObject, _defaultScale * 0.8f, 0.8f).setEase(LeanTweenType.easeOutBack).setOnComplete((Action)delegate
			{
				_currentAnimationID = 0;
				OnComplete?.Invoke();
			})
				.id;
		}).id;
	}

	private void CancelAnyCurrentAnimation()
	{
		if (_currentAnimationID != 0)
		{
			LeanTween.cancel(_currentAnimationID);
			_currentAnimationID = 0;
		}
	}
}
