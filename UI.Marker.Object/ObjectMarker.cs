using System;
using Code.Core.ResourceSystem;
using Core.Interaction;
using Core.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace UI.Marker.Object;

public class ObjectMarker : Marker
{
	private const float MAX_TEXT_DISTANCE = 3.5f;

	public static string AddressableAddress = "ObjectMarker";

	public static string AddressableAddressCapsule = "ObjectMarkerCapsule";

	[Header("ObjectMarker")]
	[SerializeField]
	private TMP_Text markerText;

	[SerializeField]
	private SphereCollider displayTextCollider;

	private int _textAnimation;

	private MilMo_LocString _markerText;

	private bool Silent { get; set; }

	public void Initialize(IHasInteraction interactableObject, MilMo_LocString objectName, float interactionRadius, bool silent = false, bool autoTrigger = false)
	{
		if (interactableObject == null)
		{
			Debug.LogWarning(base.gameObject.name + ": interactableObject is null");
			return;
		}
		Silent = silent;
		SetPosition(interactableObject.GetPosition());
		SetDisplayTextColliderRadius(3.5f);
		_markerText = objectName;
		RefreshText();
		ShowText(shouldEnable: false);
		interactable.Initialize(interactableObject, interactionRadius, silent, autoTrigger);
		SetMarkerOffset(interactableObject.GetMarkerOffset());
		base.Initialize();
	}

	protected override void Awake()
	{
		base.Awake();
		if (markerText == null)
		{
			Debug.LogWarning(base.gameObject.name + ": markerText is null");
			return;
		}
		markerText.gameObject.transform.localScale = Vector3.zero;
		if (displayTextCollider == null)
		{
			Debug.LogError(base.gameObject.name + ": displayTextCollider is null");
		}
	}

	private void OnEnable()
	{
		LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
	}

	private void OnDisable()
	{
		LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
	}

	private void OnLocaleChanged(Locale locale)
	{
		RefreshText();
	}

	public void SetPosition(Vector3 position)
	{
		base.transform.position = position;
	}

	private void ShowText(bool shouldEnable)
	{
		if (!(markerText == null))
		{
			if (_textAnimation != 0)
			{
				LeanTween.cancel(_textAnimation);
				_textAnimation = 0;
			}
			_textAnimation = LeanTween.scale(markerText.gameObject, shouldEnable ? Vector3.one : Vector3.zero, 0.2f).setOnComplete((Action)delegate
			{
				_textAnimation = 0;
			}).id;
		}
	}

	private void RefreshText()
	{
		if (markerText != null)
		{
			markerText.text = _markerText.String;
		}
	}

	private void SetDisplayTextColliderRadius(float radius)
	{
		if (displayTextCollider != null)
		{
			displayTextCollider.radius = radius;
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		if (!Silent && PlayerUtils.IsLocalPlayer(other.gameObject))
		{
			ShowText(shouldEnable: true);
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (!Silent && PlayerUtils.IsLocalPlayer(other.gameObject))
		{
			ShowText(shouldEnable: false);
		}
	}
}
