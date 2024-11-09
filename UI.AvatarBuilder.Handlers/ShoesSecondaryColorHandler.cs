using Code.World.CharBuilder;
using UnityEngine;

namespace UI.AvatarBuilder.Handlers;

public class ShoesSecondaryColorHandler : ColorHandler
{
	public override void Handle()
	{
		if (Color == null)
		{
			Debug.LogError("Unable to find ShoesSecondaryColor");
			return;
		}
		if (AvatarEditor == null)
		{
			Debug.LogError("Unable to find AvatarEditor");
			return;
		}
		AvatarEditor.ShoesHandler.SetShoesSecondaryColor(int.Parse(Color.GetIdentifier()));
		AvatarEditor.ColorUpdatedForShape(AvatarSelection.Shapes.Shoes);
	}
}
