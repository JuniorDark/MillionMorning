using Core.Colors;
using UI.AvatarBuilder.Handlers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.AvatarBuilder;

public class ColorApplier : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	private ColorHandler _handler;

	private ScriptableColor _color;

	public void Init(ColorHandler handler, ScriptableColor color)
	{
		_handler = handler;
		_color = color;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (_handler == null)
		{
			Debug.LogError("Unable to get ColorHandler!");
			return;
		}
		_handler.Setup(_color);
		_handler.Handle();
	}
}
