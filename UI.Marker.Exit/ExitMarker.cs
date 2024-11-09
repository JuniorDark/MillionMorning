using Core.Interaction;
using UI.FX;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Marker.Exit;

public class ExitMarker : Marker
{
	public static string AddressableAddress = "ExitMarker";

	private const float MAX_DISTANCE = 96f;

	private const float MIN_DISTANCE = 10f;

	[SerializeField]
	private Image exitArrow;

	[SerializeField]
	private UIAlphaFX arrowFade;

	[SerializeField]
	private int arrowMoveSpeed = 2;

	[SerializeField]
	private int arrowMoveAmount = 1;

	[SerializeField]
	private float arrowOffsetY = 3f;

	private Camera _cam;

	private bool _isShown;

	private Sprite _exitArrowSprite;

	public void Initialize(IHasInteraction interactableObject, Vector3 exitArrowPosition, float interactionRadius, bool silent = false, bool autoTrigger = false)
	{
		if (interactableObject == null)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		SetPosition(exitArrowPosition);
		interactable.Initialize(interactableObject, interactionRadius, silent, autoTrigger);
		SetMarkerOffset(interactableObject.GetMarkerOffset());
		base.Initialize();
	}

	protected override void Awake()
	{
		base.Awake();
		if (arrowFade != null)
		{
			arrowFade.FadeOutFast();
		}
	}

	private void OnEnable()
	{
		_cam = Camera.main;
	}

	protected void Update()
	{
		if ((bool)_cam)
		{
			float magnitude = (_cam.transform.position - base.transform.position).magnitude;
			bool flag = magnitude <= 96f && magnitude >= 10f;
			Show(flag);
			if (flag)
			{
				Animate();
			}
		}
	}

	private void Show(bool shouldShow)
	{
		if (_isShown == shouldShow)
		{
			return;
		}
		_isShown = shouldShow;
		if (!(arrowFade == null))
		{
			if (shouldShow)
			{
				arrowFade.FadeIn();
			}
			else
			{
				arrowFade.FadeOut();
			}
		}
	}

	public void SetPosition(Vector3 position)
	{
		base.transform.position = position;
	}

	private void Animate()
	{
		if (!(exitArrow == null))
		{
			Transform obj = exitArrow.transform;
			float y = Mathf.Sin(Time.time * (float)arrowMoveSpeed) * (float)arrowMoveAmount + arrowOffsetY;
			Vector3 localPosition = obj.localPosition;
			obj.localPosition = new Vector3(localPosition.x, y, localPosition.z);
		}
	}
}
