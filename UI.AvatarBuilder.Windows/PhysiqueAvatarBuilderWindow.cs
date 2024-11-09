namespace UI.AvatarBuilder.Windows;

public class PhysiqueAvatarBuilderWindow : AvatarBuilderWindow
{
	protected override void OnEnable()
	{
		base.OnEnable();
		if (!(AnimationHandler == null) && !(CameraHandler == null))
		{
			AnimationHandler.SetCurrentCategory("");
			CameraHandler.LookAtBody();
		}
	}
}
