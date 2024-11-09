using Code.Core.ResourceSystem;

namespace Code.Core.Camera.CameraActions;

public class MilMo_CameraActionFadeIn : MilMo_CameraAction
{
	private float _duration;

	public MilMo_CameraActionFadeIn(float time)
		: base(time)
	{
	}

	public override void Read(MilMo_SFFile file)
	{
		_duration = file.GetFloat();
	}

	protected override void ExecuteInternal(MilMo_MovieCameraController cameraController)
	{
		cameraController.FadeIn(_duration);
	}
}
