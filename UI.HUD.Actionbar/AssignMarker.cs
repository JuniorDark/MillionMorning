using System;
using UnityEngine;

namespace UI.HUD.Actionbar;

public class AssignMarker : MonoBehaviour
{
	private CanvasGroup _target;

	private int _fadeId;

	private int _rotationId;

	private int _impulseId;

	private void Awake()
	{
		_target = GetComponent<CanvasGroup>();
		if (!_target)
		{
			_target = base.gameObject.AddComponent<CanvasGroup>();
		}
		_target.alpha = 0f;
	}

	public void Show()
	{
		base.gameObject.SetActive(value: true);
		FadeIn();
		StartRotating();
	}

	public void Hide()
	{
		FadeOutAndClose();
	}

	public void Complete()
	{
		Impulse();
	}

	private void FadeIn()
	{
		if (_fadeId != 0)
		{
			LeanTween.cancel(_fadeId);
			_fadeId = 0;
		}
		_fadeId = LeanTween.alphaCanvas(_target, 1f, 1f).id;
	}

	private void FadeOutAndClose()
	{
		if (_fadeId != 0)
		{
			LeanTween.cancel(_fadeId);
			_fadeId = 0;
		}
		_fadeId = LeanTween.alphaCanvas(_target, 0f, 1f).setOnComplete((Action)delegate
		{
			StopRotating();
			base.gameObject.SetActive(value: false);
		}).id;
	}

	private void StartRotating()
	{
		if (_rotationId == 0)
		{
			_rotationId = LeanTween.rotateAroundLocal(base.gameObject, Vector3.forward, -360f, 4f).setRepeat(999).id;
		}
	}

	private void StopRotating()
	{
		if (_rotationId != 0)
		{
			LeanTween.cancel(_rotationId);
			_rotationId = 0;
		}
	}

	private void Impulse()
	{
		if (_impulseId != 0)
		{
			return;
		}
		_impulseId = LeanTween.scale(base.gameObject, Vector3.one * 1.2f, 0f).setOnComplete((Action)delegate
		{
			_impulseId = LeanTween.scale(base.gameObject, Vector3.one, 0.3f).setOnComplete((Action)delegate
			{
				base.gameObject.transform.localScale = Vector3.one;
				_impulseId = 0;
			}).id;
		}).id;
	}
}
