using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UI.FX;

public class UISlidingFX : MonoBehaviour
{
	private enum SlidingAxis
	{
		X,
		Y,
		XY
	}

	[Header("Preset")]
	[SerializeField]
	private UISlidingFXPresetSO preset;

	[Header("Reactions")]
	[SerializeField]
	private UnityEvent onSlideIn;

	[SerializeField]
	private UnityEvent onSlideOut;

	[Header("Elements")]
	public List<GameObject> hideDuringLayoutGroupRefresh;

	private int _currentSlideInAnimationID;

	private int _currentSlideOutAnimationID;

	private SlidingAxis _slidingAxis;

	private void UpdateMoveType()
	{
		bool flag = (bool)preset && preset.outPosition.x != 0f && preset.outPosition.y == 0f;
		bool flag2 = (bool)preset && preset.outPosition.x == 0f && preset.outPosition.y != 0f;
		_slidingAxis = ((!flag) ? (flag2 ? SlidingAxis.Y : SlidingAxis.XY) : SlidingAxis.X);
	}

	public void Slide(bool value)
	{
		if (value)
		{
			SlideIn();
		}
		else
		{
			SlideOut();
		}
	}

	public void SlideIn()
	{
		if (_currentSlideInAnimationID != 0)
		{
			return;
		}
		if (_currentSlideOutAnimationID != 0)
		{
			LeanTween.cancel(_currentSlideOutAnimationID);
			_currentSlideOutAnimationID = 0;
		}
		if (!base.gameObject.activeSelf)
		{
			SlideOutFast();
			foreach (GameObject item in hideDuringLayoutGroupRefresh)
			{
				item.SetActive(value: false);
			}
			LeanTween.delayedCall(0.1f, (Action)delegate
			{
				foreach (GameObject item2 in hideDuringLayoutGroupRefresh)
				{
					item2.SetActive(value: true);
				}
			});
			base.gameObject.SetActive(value: true);
			onSlideIn?.Invoke();
		}
		UpdateMoveType();
		LTDescr lTDescr = _slidingAxis switch
		{
			SlidingAxis.X => LeanTween.moveLocalX(base.gameObject, 0f, 0.6f), 
			SlidingAxis.Y => LeanTween.moveLocalY(base.gameObject, 0f, 0.6f), 
			SlidingAxis.XY => LeanTween.moveLocal(base.gameObject, Vector2.zero, 0.6f), 
			_ => null, 
		};
		if (lTDescr != null)
		{
			lTDescr.setEase(preset.slideInEase).setOnComplete((Action)delegate
			{
				_currentSlideInAnimationID = 0;
			});
			_currentSlideInAnimationID = lTDescr.id;
		}
	}

	public void SlideOut()
	{
		if (_currentSlideOutAnimationID != 0)
		{
			return;
		}
		if (_currentSlideInAnimationID != 0)
		{
			LeanTween.cancel(_currentSlideInAnimationID);
			_currentSlideInAnimationID = 0;
		}
		onSlideOut?.Invoke();
		if (!preset)
		{
			return;
		}
		UpdateMoveType();
		LTDescr lTDescr = _slidingAxis switch
		{
			SlidingAxis.X => LeanTween.moveLocalX(base.gameObject, preset.outPosition.x, 0.6f), 
			SlidingAxis.Y => LeanTween.moveLocalY(base.gameObject, preset.outPosition.y, 0.6f), 
			SlidingAxis.XY => LeanTween.moveLocal(base.gameObject, preset.outPosition, 0.6f), 
			_ => null, 
		};
		if (lTDescr != null)
		{
			lTDescr.setEase(preset.slideOutEase).setOnComplete((Action)delegate
			{
				_currentSlideOutAnimationID = 0;
				base.gameObject.SetActive(value: false);
			});
			_currentSlideOutAnimationID = lTDescr.id;
		}
	}

	private void SlideOutFast()
	{
		if ((bool)preset)
		{
			UpdateMoveType();
			switch (_slidingAxis)
			{
			case SlidingAxis.X:
				LeanTween.moveLocalX(base.gameObject, preset.outPosition.x, 0f);
				break;
			case SlidingAxis.Y:
				LeanTween.moveLocalY(base.gameObject, preset.outPosition.y, 0f);
				break;
			case SlidingAxis.XY:
				LeanTween.moveLocal(base.gameObject, preset.outPosition, 0f);
				break;
			}
		}
	}
}
