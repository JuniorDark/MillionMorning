using System;
using Code.Core.Utility;
using Core.State;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.HealthBar;

public class Heart : MonoBehaviour
{
	[Header("Elements")]
	[SerializeField]
	private Image slotImage;

	[SerializeField]
	private Image fillImage;

	[Header("Sprites")]
	[SerializeField]
	private Sprite slot;

	[SerializeField]
	private Sprite slotHalf;

	[SerializeField]
	private Sprite fill;

	[SerializeField]
	private Sprite fillHalf;

	[Header("State")]
	public HeartState heartState;

	private int _currentShowID;

	private int _currentHideID;

	private void Awake()
	{
		heartState = ScriptableObject.CreateInstance<HeartState>();
		heartState.OnSizeChange += UpdateSlot;
		heartState.OnValueChange += UpdateFill;
		HideFast();
	}

	private void OnDestroy()
	{
		heartState.OnSizeChange -= UpdateSlot;
		heartState.OnValueChange -= UpdateFill;
		UnityEngine.Object.Destroy(heartState);
	}

	private void UpdateSlot(HeartState.HeartSize newHeartSize)
	{
		Sprite sprite = ((newHeartSize == HeartState.HeartSize.Half) ? slotHalf : slot);
		if ((bool)sprite)
		{
			slotImage.sprite = sprite;
		}
	}

	private void UpdateFill(HeartState.HeartValue newHeartValue)
	{
		LeanTween.delayedCall(fillImage.gameObject, 0.3f, (Action)delegate
		{
			fillImage.enabled = false;
			if (newHeartValue != 0)
			{
				Sprite sprite = ((newHeartValue == HeartState.HeartValue.Half) ? fillHalf : fill);
				if ((bool)sprite)
				{
					fillImage.sprite = sprite;
					fillImage.enabled = true;
				}
			}
		});
	}

	public void Remove()
	{
		base.enabled = false;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void Beat()
	{
		GameObject obj = fillImage.gameObject;
		LeanTween.scale(obj, new Vector3(1.2f, 1.2f, 1.2f), 0f);
		LeanTween.scale(obj, Vector3.one, 0.25f).setEase(LeanTweenType.easeSpring);
	}

	public void DamageFX()
	{
		GameObject obj = fillImage.gameObject;
		LeanTween.moveLocalX(obj, -3f, 0.3f).setEaseShake();
		LeanTween.moveLocalX(obj, 0f, 0.7f).setDelay(0.3f).setEaseShake();
		LeanTween.moveLocalY(obj, -1f, 0.3f).setEaseSpring();
		LeanTween.moveLocalY(obj, 0f, 0.7f).setDelay(0.3f).setEaseSpring();
		LeanTween.scale(obj, new Vector3(0.9f, 0.9f, 0.9f), 0f);
		LeanTween.scale(obj, Vector3.one, 1f).setEase(LeanTweenType.easeSpring);
	}

	public void GainFX()
	{
		GameObject obj = fillImage.gameObject;
		LeanTween.moveLocalY(obj, 5f, 0.3f).setEaseSpring();
		LeanTween.moveLocalY(obj, 0f, 0.7f).setDelay(0.3f).setEaseSpring();
		LeanTween.scale(obj, new Vector3(1.5f, 1.5f, 1.5f), 0f);
		LeanTween.scale(obj, Vector3.one, 1f).setEase(LeanTweenType.easeSpring);
	}

	public void Show()
	{
		if (_currentShowID == 0)
		{
			if (_currentHideID != 0)
			{
				LeanTween.cancel(_currentHideID);
				_currentHideID = 0;
			}
			_currentShowID = LeanTween.moveLocalY(base.gameObject, 0f, 1f).setEase(LeanTweenType.easeOutQuad).setOnComplete((Action)delegate
			{
				_currentShowID = 0;
			})
				.id;
			LeanTween.scale(base.gameObject, Vector3.one, 1f).setDelay(0.2f).setEase(LeanTweenType.easeSpring);
			LeanTween.rotate(base.gameObject, Vector3.zero, 1f).setDelay(0.2f);
		}
	}

	public void ShowFast()
	{
		LeanTween.moveLocalY(base.gameObject, 0f, 0f);
		LeanTween.scale(base.gameObject, Vector3.one, 0f);
		LeanTween.rotate(base.gameObject, Vector3.zero, 0f);
	}

	public void Hide()
	{
		if (_currentHideID != 0)
		{
			return;
		}
		if (_currentShowID != 0)
		{
			LeanTween.cancel(_currentShowID);
			_currentShowID = 0;
		}
		_currentHideID = LeanTween.moveLocalY(base.gameObject, -5f, 0.2f).setEase(LeanTweenType.easeSpring).setOnComplete((Action)delegate
		{
			_currentHideID = LeanTween.moveLocalY(base.gameObject, 50f, 0.4f).setEase(LeanTweenType.easeSpring).setOnComplete((Action)delegate
			{
				_currentHideID = 0;
			})
				.id;
		})
			.id;
	}

	public void HideFast()
	{
		LeanTween.moveLocalY(base.gameObject, 50f, 0f);
		LeanTween.scale(base.gameObject, Vector3.zero, 0f);
		LeanTween.rotateAround(base.gameObject, Vector3.forward, -25f * MilMo_Utility.RandomFloat(-1f, 1f), 0f);
	}
}
