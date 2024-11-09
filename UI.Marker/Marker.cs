using Core.Interaction;
using UnityEngine;

namespace UI.Marker;

public abstract class Marker : WorldSpaceObject
{
	[Header("Scene")]
	public Interactable interactable;

	public GameObject markerContainer;

	protected override void Awake()
	{
		base.Awake();
		if (interactable == null)
		{
			Debug.LogError(base.gameObject.name + ": interactable is null");
		}
		else if (markerContainer == null)
		{
			Debug.LogWarning(base.gameObject.name + ": markerContainer is null");
		}
		else
		{
			markerContainer.gameObject.transform.localScale = Vector3.zero;
		}
	}

	public void SetInteractionType(Interactable.InteractionType? type)
	{
		if (interactable != null)
		{
			interactable.SetInteractionMarkerType(type);
		}
	}

	public void SetMarkerOffset(Vector3 localPosition)
	{
		if (markerContainer != null)
		{
			markerContainer.transform.localPosition = localPosition;
		}
	}

	public void Disable(bool isDisabled)
	{
		if (interactable != null)
		{
			interactable.gameObject.SetActive(!isDisabled);
		}
		if (markerContainer != null)
		{
			markerContainer.gameObject.SetActive(!isDisabled);
		}
	}
}
