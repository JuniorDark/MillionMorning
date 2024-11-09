using Code.World.CharBuilder;
using UnityEngine;

namespace UI.AvatarBuilder.Handlers;

public class PantsColorHandler : ColorHandler
{
	public override void Handle()
	{
		if (Color == null)
		{
			Debug.LogError("Unable to find PantsColor");
			return;
		}
		if (AvatarEditor == null)
		{
			Debug.LogError("Unable to find AvatarEditor");
			return;
		}
		AvatarEditor.PantsHandler.SetPantsColor(int.Parse(Color.GetIdentifier()));
		AvatarEditor.ColorUpdatedForShape(AvatarSelection.Shapes.Pants);
	}
}
