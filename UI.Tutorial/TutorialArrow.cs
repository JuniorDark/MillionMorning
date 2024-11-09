using Core.GameEvent;
using UnityEngine;

namespace UI.Tutorial;

public class TutorialArrow : MonoBehaviour
{
	[Header("Setup")]
	[SerializeField]
	private string arrowEventName;

	[SerializeField]
	private ArrowPresetSO preset;

	private GameObject _arrowObject;

	private Vector3 _targetPosition;

	private int _arrowAnimation;

	private LeanTweenType _currentLeanTweenType;

	private void Awake()
	{
		if (!preset)
		{
			Debug.LogWarning("TutorialArrow " + base.gameObject.name + ": Missing preset");
		}
		else if (!preset.arrowPrefab.RuntimeKeyIsValid())
		{
			Debug.LogWarning("TutorialArrow " + base.gameObject.name + ": Missing prefab");
		}
	}

	private void Start()
	{
		GameEvent.OnTutorialArrowEvent.RegisterAction(OnTutorialArrowTriggered);
		if ((bool)preset)
		{
			preset.OnChange += OnPresetChanged;
		}
	}

	private void OnDestroy()
	{
		GameEvent.OnTutorialArrowEvent.UnregisterAction(OnTutorialArrowTriggered);
	}

	private void OnTutorialArrowTriggered(string tutorialArrow)
	{
		if (!(tutorialArrow != arrowEventName))
		{
			TriggerArrowAnimation();
			GameEvent.OnCloseTutorialArrowEvent.RegisterAction(RemoveArrow);
		}
	}

	private void TriggerArrowAnimation()
	{
		_arrowObject = preset.arrowPrefab.InstantiateAsync(base.transform).WaitForCompletion();
		if (!_arrowObject)
		{
			Debug.LogWarning("TutorialArrow " + base.gameObject.name + ": ArrowObject is null");
			return;
		}
		ResetArrowPosition();
		RotateArrow();
		StartAnimation();
	}

	private void OnPresetChanged()
	{
		if ((bool)_arrowObject && _currentLeanTweenType != preset.leanTweenType)
		{
			StopAnimation();
			ResetArrowPosition();
			RotateArrow();
			StartAnimation();
			_currentLeanTweenType = preset.leanTweenType;
		}
	}

	private void ResetArrowPosition()
	{
		RectTransform component = GetComponent<RectTransform>();
		Vector3 position = base.transform.position;
		Vector2 vector = (component ? component.rect.center : Vector2.zero);
		Vector3 vector2 = new Vector3(position.x + vector.x, position.y + vector.y);
		Vector3 normalized = (new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f) - vector2).normalized;
		_targetPosition = vector2 + normalized * 30f;
		_arrowObject.transform.position = vector2 + normalized * preset.animationDistance;
	}

	private void RotateArrow()
	{
		Vector3 vector = _targetPosition - _arrowObject.transform.position;
		float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, num + -90f));
		_arrowObject.transform.rotation = rotation;
	}

	private void StartAnimation()
	{
		_arrowAnimation = LeanTween.move(_arrowObject, _targetPosition, preset.animationDuration).setLoopPingPong().setEase(preset.leanTweenType)
			.id;
	}

	private void StopAnimation()
	{
		if (_arrowAnimation != 0)
		{
			LeanTween.cancel(_arrowAnimation);
			_arrowAnimation = 0;
		}
	}

	private void RemoveArrow()
	{
		StopAnimation();
		GameEvent.OnCloseTutorialArrowEvent.UnregisterAction(RemoveArrow);
		Object.Destroy(_arrowObject);
	}
}
