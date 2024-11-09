using Core.BodyShapes;
using UI.AvatarBuilder.Handlers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.AvatarBuilder;

public class ShapeApplier : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	private ShapeHandler _handler;

	private ScriptableShape _shape;

	public void Init(ShapeHandler handler, ScriptableShape shape)
	{
		_handler = handler;
		_shape = shape;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (_handler == null)
		{
			Debug.LogError("Unable to get ShapeHandler!");
		}
		_handler.Setup(_shape);
		_handler.Handle();
	}
}
