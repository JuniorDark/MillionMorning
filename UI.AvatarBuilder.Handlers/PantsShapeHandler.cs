using UnityEngine;

namespace UI.AvatarBuilder.Handlers;

public class PantsShapeHandler : ShapeHandler
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
			AvatarEditor.PantsHandler.SetPants(Shape.GetIdentifier());
		}
	}
}
