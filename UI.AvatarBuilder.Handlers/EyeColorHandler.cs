using UnityEngine;

namespace UI.AvatarBuilder.Handlers;

public class EyeColorHandler : ColorHandler
{
	public override void Handle()
	{
		if (Color == null)
		{
			Debug.LogError("Unable to find SkinColor");
		}
		else if (AvatarEditor == null)
		{
			Debug.LogError("Unable to find AvatarEditor");
		}
		else
		{
			AvatarEditor.SetEyeColor(Color.GetIdentifier());
		}
	}
}
