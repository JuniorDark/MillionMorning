using Code.World.CharBuilder;
using UnityEngine;

namespace UI.AvatarBuilder.Handlers;

public class HairColorHandler : ColorHandler
{
	public override void Handle()
	{
		if (Color == null)
		{
			Debug.LogError("Unable to find SkinColor");
			return;
		}
		if (AvatarEditor == null)
		{
			Debug.LogError("Unable to find AvatarEditor");
			return;
		}
		AvatarEditor.HairStyleHandler.SetHairColor(int.Parse(Color.GetIdentifier()));
		AvatarEditor.ColorUpdatedForShape(AvatarSelection.Shapes.Hair);
	}
}
