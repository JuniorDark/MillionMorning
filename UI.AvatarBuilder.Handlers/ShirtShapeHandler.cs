using UnityEngine;

namespace UI.AvatarBuilder.Handlers;

public class ShirtShapeHandler : ShapeHandler
{
	public override void Handle()
	{
		if (Shape == null)
		{
			Debug.LogError("Unable to find ShirtShape");
		}
		else if (AvatarEditor == null)
		{
			Debug.LogError("Unable to find AvatarEditor");
		}
		else
		{
			AvatarEditor.ShirtHandler.SetShirt(Shape.GetIdentifier());
		}
	}
}
