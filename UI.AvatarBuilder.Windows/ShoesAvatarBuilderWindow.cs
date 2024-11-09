namespace UI.AvatarBuilder.Windows;

public class ShoesAvatarBuilderWindow : AvatarBuilderWindow
{
	protected override void OnEnable()
	{
		base.OnEnable();
		if (!(AnimationHandler == null) && !(CameraHandler == null))
		{
			AnimationHandler.SetCurrentCategory("SHOES");
			CameraHandler.LookAtShoes();
		}
	}
}
