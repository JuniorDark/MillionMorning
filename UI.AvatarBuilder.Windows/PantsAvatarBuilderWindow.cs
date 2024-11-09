namespace UI.AvatarBuilder.Windows;

public class PantsAvatarBuilderWindow : AvatarBuilderWindow
{
	protected override void OnEnable()
	{
		base.OnEnable();
		if (!(AnimationHandler == null) && !(CameraHandler == null))
		{
			AnimationHandler.SetCurrentCategory("PANTS");
			CameraHandler.LookAtClothes();
		}
	}
}
