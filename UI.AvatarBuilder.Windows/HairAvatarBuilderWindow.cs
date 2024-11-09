namespace UI.AvatarBuilder.Windows;

public class HairAvatarBuilderWindow : AvatarBuilderWindow
{
	protected override void OnEnable()
	{
		base.OnEnable();
		if (!(AnimationHandler == null) && !(CameraHandler == null))
		{
			AnimationHandler.SetCurrentCategory("HAIR");
			CameraHandler.LookAtHair();
		}
	}
}
