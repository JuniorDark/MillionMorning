using UnityEngine;

namespace UI.AvatarBuilder.Handlers;

public class SkinColorHandler : ColorHandler
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
			AvatarEditor.SetSkinColor(Color.GetIdentifier());
		}
	}
}
