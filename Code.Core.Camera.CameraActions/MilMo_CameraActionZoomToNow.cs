using Code.Core.ResourceSystem;

namespace Code.Core.Camera.CameraActions;

public class MilMo_CameraActionZoomToNow : MilMo_CameraAction
{
	private float _targetZoom;

	public MilMo_CameraActionZoomToNow(float time)
		: base(time)
	{
	}

	public override void Read(MilMo_SFFile file)
	{
		_targetZoom = file.GetFloat();
	}

	protected override void ExecuteInternal(MilMo_MovieCameraController cameraController)
	{
		cameraController.ZoomToNow(_targetZoom);
	}
}
