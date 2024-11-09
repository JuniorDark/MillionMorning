using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.FX;

[RequireComponent(typeof(Graphic))]
public class UIColorFX : MonoBehaviour
{
	private Graphic _target;

	private int _currentAnimationID;

	private Color _defaultColor = Color.white;

	[SerializeField]
	private UnityEvent OnComplete;

	private void Awake()
	{
		_target = GetComponent<Graphic>();
		if (!_target)
		{
			Debug.LogWarning("Missing graphic component! (" + base.name + ")");
		}
		else
		{
			_defaultColor = _target.color;
		}
	}

	public void Run(UIColorFXPresetSO preset)
	{
		if (_target == null || preset == null)
		{
			return;
		}
		if (_currentAnimationID != 0)
		{
			LeanTween.cancel(_currentAnimationID);
			_currentAnimationID = 0;
		}
		_currentAnimationID = LeanTween.value(base.gameObject, _defaultColor, preset.tintColor, preset.inTime).setOnUpdate(delegate(Color val)
		{
			_target.color = val;
		}).setEase(preset.inEase)
			.setOnComplete((Action)delegate
			{
				_currentAnimationID = LeanTween.value(base.gameObject, preset.tintColor, _defaultColor, preset.outTime).setOnUpdate(delegate(Color val)
				{
					_target.color = val;
				}).setEase(preset.outEase)
					.setDelay(preset.onTime)
					.setOnComplete((Action)delegate
					{
						_currentAnimationID = 0;
						_target.color = _defaultColor;
						OnComplete?.Invoke();
					})
					.id;
			})
			.id;
	}
}
