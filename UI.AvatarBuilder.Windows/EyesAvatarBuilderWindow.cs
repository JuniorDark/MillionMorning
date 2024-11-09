namespace UI.AvatarBuilder.Windows;

public class EyesAvatarBuilderWindow : AvatarBuilderWindow
{
	protected override void OnEnable()
	{
		base.OnEnable();
		if (!(AnimationHandler == null) && !(CameraHandler == null))
		{
			AnimationHandler.SetCurrentCategory("EYES");
			CameraHandler.LookAtFace();
		}
	}
}
