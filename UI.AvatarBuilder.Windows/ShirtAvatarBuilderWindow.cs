namespace UI.AvatarBuilder.Windows;

public class ShirtAvatarBuilderWindow : AvatarBuilderWindow
{
	protected override void OnEnable()
	{
		base.OnEnable();
		if (!(AnimationHandler == null) && !(CameraHandler == null))
		{
			AnimationHandler.SetCurrentCategory("SHIRT");
			CameraHandler.LookAtClothes();
		}
	}
}
