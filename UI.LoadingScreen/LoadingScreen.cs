using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.LoadingScreen;

public class LoadingScreen : MonoBehaviour
{
	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private Text loadingText;

	[SerializeField]
	private Slider progressBar;

	private int _currentAnimationID;

	public void UpdateText(string newValue)
	{
		loadingText.text = newValue;
	}

	public void UpdateProgress(float newValue)
	{
		if (newValue < 1f)
		{
			Open();
		}
		if (_currentAnimationID != 0)
		{
			LeanTween.cancel(_currentAnimationID);
		}
		SlideValueFX(progressBar.value, newValue);
	}

	private void SlideValueFX(float from, float to)
	{
		float time = Math.Abs(to - from) * 2f;
		_currentAnimationID = LeanTween.value(base.gameObject, from, to, time).setEase(LeanTweenType.easeOutSine).setOnUpdate(delegate(float val)
		{
			progressBar.value = val;
		})
			.setOnComplete((Action)delegate
			{
				_currentAnimationID = 0;
				if (progressBar.value > 0f && Math.Abs(progressBar.value) > 0.99f)
				{
					Close();
				}
			})
			.id;
	}

	private void Open()
	{
		base.gameObject.SetActive(value: true);
		LeanTween.value(base.gameObject, canvasGroup.alpha, 1f, 0.1f).setEase(LeanTweenType.easeOutSine).setOnUpdate(delegate(float val)
		{
			canvasGroup.alpha = val;
		});
	}

	private void Close()
	{
		LeanTween.value(base.gameObject, canvasGroup.alpha, 0f, 1f).setEase(LeanTweenType.easeOutSine).setOnUpdate(delegate(float val)
		{
			canvasGroup.alpha = val;
		})
			.setOnComplete((Action)delegate
			{
				base.gameObject.SetActive(value: false);
			});
	}
}
