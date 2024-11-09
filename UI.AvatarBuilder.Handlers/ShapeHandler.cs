using Code.World.CharBuilder;
using Core.BodyShapes;
using Core.Handler;
using UnityEngine;

namespace UI.AvatarBuilder.Handlers;

public abstract class ShapeHandler : BaseHandler
{
	protected ScriptableShape Shape;

	protected AvatarEditor AvatarEditor;

	private void Start()
	{
		AvatarEditor = Object.FindObjectOfType<AvatarEditor>();
		if (AvatarEditor == null)
		{
			Debug.LogError("Unable to find AvatarEditor");
		}
	}

	public void Setup(ScriptableShape shape)
	{
		Shape = shape;
	}
}
