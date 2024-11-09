using UnityEngine;

namespace UI.AvatarBuilder.Handlers;

public class MouthShapeHandler : ShapeHandler
{
	public override void Handle()
	{
		if (Shape == null)
		{
			Debug.LogError("Unable to find EyeShape");
		}
		else if (AvatarEditor == null)
		{
			Debug.LogError("Unable to find AvatarEditor");
		}
		else
		{
			AvatarEditor.SetMouth(Shape.GetIdentifier());
		}
	}
}
