using Code.World.CharBuilder;
using UnityEngine;

namespace UI.AvatarBuilder.Handlers;

public class ShoesPrimaryColorHandler : ColorHandler
{
	public override void Handle()
	{
		if (Color == null)
		{
			Debug.LogError("Unable to find ShoesPrimaryColor");
			return;
		}
		if (AvatarEditor == null)
		{
			Debug.LogError("Unable to find AvatarEditor");
			return;
		}
		AvatarEditor.ShoesHandler.SetShoesPrimaryColor(int.Parse(Color.GetIdentifier()));
		AvatarEditor.ColorUpdatedForShape(AvatarSelection.Shapes.Shoes);
	}
}
