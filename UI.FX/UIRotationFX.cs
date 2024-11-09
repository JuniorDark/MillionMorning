using System;
using UnityEngine;
using UnityEngine.Events;

namespace UI.FX;

public class UIRotationFX : MonoBehaviour
{
	private float _defaultRotation;

	[SerializeField]
	private UnityEvent OnComplete;

	private int CurrentAnimationId { get; set; }

	private void Awake()
	{
		_defaultRotation = base.transform.localEulerAngles.z;
	}

	public void Run(UIRotationFXPresetSO preset)
	{
		if (CurrentAnimationId != 0)
		{
			LeanTween.cancel(CurrentAnimationId);
			CurrentAnimationId = 0;
		}
		Vector3 eulerAngles = base.gameObject.transform.rotation.eulerAngles;
		base.gameObject.transform.Rotate(eulerAngles.x, eulerAngles.y, preset.rotateFrom);
		CurrentAnimationId = LeanTween.rotateZ(base.gameObject, preset.rotateTo, preset.inTime).setEase(preset.inEase).setOnComplete((Action)delegate
		{
			CurrentAnimationId = LeanTween.rotateZ(base.gameObject, _defaultRotation, preset.outTime).setEase(preset.outEase).setDelay(preset.onTime)
				.setOnComplete((Action)delegate
				{
					CurrentAnimationId = 0;
					OnComplete?.Invoke();
				})
				.id;
		})
			.id;
	}
}
