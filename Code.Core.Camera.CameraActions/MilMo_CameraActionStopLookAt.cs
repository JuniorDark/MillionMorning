using Code.Core.ResourceSystem;

namespace Code.Core.Camera.CameraActions;

public class MilMo_CameraActionStopLookAt : MilMo_CameraAction
{
	public MilMo_CameraActionStopLookAt(float time)
		: base(time)
	{
	}

	public override void Read(MilMo_SFFile file)
	{
	}

	protected override void ExecuteInternal(MilMo_MovieCameraController cameraController)
	{
		cameraController.StopLookAt();
	}
}
