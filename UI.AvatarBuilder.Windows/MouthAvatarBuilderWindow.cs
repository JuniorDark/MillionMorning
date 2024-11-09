namespace UI.AvatarBuilder.Windows;

public class MouthAvatarBuilderWindow : AvatarBuilderWindow
{
	protected override void OnEnable()
	{
		base.OnEnable();
		if (!(AnimationHandler == null) && !(CameraHandler == null))
		{
			AnimationHandler.SetCurrentCategory("MOUTH");
			CameraHandler.LookAtFace();
		}
	}
}
