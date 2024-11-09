using UnityEngine;

namespace Core.Interaction;

public interface IHasInteraction
{
	int GetPrio();

	void UseReaction();

	Vector3 GetPosition();

	Vector3 GetMarkerOffset();

	string GetInteractionVerb();

	Interactable.InteractionType GetInteractionType();
}
