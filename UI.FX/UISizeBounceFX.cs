using UnityEngine;

namespace UI.FX;

public class UISizeBounceFX : MonoBehaviour
{
	[SerializeField]
	private RectTransform imageTransform;

	private Vector2 _cachedSize;

	private int _currentAnimationID;

	private void Awake()
	{
		if (imageTransform == null)
		{
			imageTransform = GetComponent<RectTransform>();
		}
		_cachedSize = imageTransform.sizeDelta;
	}

	public void Run(UISizeBounceFXPresetSO preset)
	{
		if (_currentAnimationID != 0)
		{
			LeanTween.cancel(_currentAnimationID);
			_currentAnimationID = 0;
			RestoreSize();
		}
		_currentAnimationID = LeanTween.size(imageTransform, preset.toSize, preset.time).setEase(LeanTweenType.easeInOutSine).setLoopPingPong(1)
			.setOnComplete(RestoreSize)
			.id;
	}

	private void RestoreSize()
	{
		imageTransform.sizeDelta = _cachedSize;
	}
}
