using System;
using UnityEngine;
using UnityEngine.Events;

namespace UI.FX;

public class UIAlphaFX : MonoBehaviour
{
	[SerializeField]
	private float fadeInSpeed = 1f;

	[SerializeField]
	private float fadeOutSpeed = 1f;

	[SerializeField]
	private float fadeInDelay = 0.3f;

	[SerializeField]
	private float fadeOutDelay = 0.3f;

	[Header("Events")]
	[SerializeField]
	private UnityEvent onFadeInStart;

	[SerializeField]
	private UnityEvent onFadeInComplete;

	[SerializeField]
	private UnityEvent onFadeOutComplete;

	private CanvasGroup _target;

	private int _currentFadeInAnimationID;

	private int _currentFadeOutAnimationID;

	private void Awake()
	{
		_target = GetComponent<CanvasGroup>();
		if (!_target)
		{
			_target = base.gameObject.AddComponent<CanvasGroup>();
		}
	}

	public float GetFadeInDuration(float extraDelay = 0f)
	{
		return fadeInSpeed + fadeInDelay + extraDelay;
	}

	public float GetFadeOutDuration(float extraDelay = 0f)
	{
		return fadeOutSpeed + fadeOutDelay + extraDelay;
	}

	public void FadeIn()
	{
		if (_currentFadeInAnimationID != 0)
		{
			return;
		}
		float delay = fadeInDelay;
		if (_currentFadeOutAnimationID != 0)
		{
			LeanTween.cancel(_currentFadeOutAnimationID);
			_currentFadeOutAnimationID = 0;
			delay = 0f;
		}
		if (!(_target == null))
		{
			onFadeInStart?.Invoke();
			_currentFadeInAnimationID = LeanTween.alphaCanvas(_target, 1f, fadeInSpeed).setEase(LeanTweenType.easeOutSine).setOnComplete((Action)delegate
			{
				onFadeInComplete?.Invoke();
				_currentFadeInAnimationID = 0;
			})
				.setDelay(delay)
				.id;
		}
	}

	public void FadeOut()
	{
		if (_currentFadeOutAnimationID != 0)
		{
			return;
		}
		float delay = fadeOutDelay;
		if (_currentFadeInAnimationID != 0)
		{
			LeanTween.cancel(_currentFadeInAnimationID);
			_currentFadeInAnimationID = 0;
			delay = 0f;
		}
		if (!(_target == null))
		{
			_currentFadeOutAnimationID = LeanTween.alphaCanvas(_target, 0f, fadeOutSpeed).setEase(LeanTweenType.easeOutSine).setOnComplete((Action)delegate
			{
				onFadeOutComplete?.Invoke();
				_currentFadeOutAnimationID = 0;
			})
				.setDelay(delay)
				.id;
		}
	}

	public void FadeOutFast()
	{
		if (_currentFadeInAnimationID != 0)
		{
			LeanTween.cancel(_currentFadeInAnimationID);
			_currentFadeInAnimationID = 0;
		}
		if (_currentFadeOutAnimationID != 0)
		{
			LeanTween.cancel(_currentFadeOutAnimationID);
			_currentFadeOutAnimationID = 0;
		}
		if (!(_target == null))
		{
			_target.alpha = 0f;
			onFadeOutComplete?.Invoke();
		}
	}
}
