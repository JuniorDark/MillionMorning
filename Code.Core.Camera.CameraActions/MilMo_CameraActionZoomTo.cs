using Code.Core.ResourceSystem;

namespace Code.Core.Camera.CameraActions;

public class MilMo_CameraActionZoomTo : MilMo_CameraAction
{
	private float _targetZoom;

	public MilMo_CameraActionZoomTo(float time)
		: base(time)
	{
	}

	public override void Read(MilMo_SFFile file)
	{
		_targetZoom = file.GetFloat();
	}

	protected override void ExecuteInternal(MilMo_MovieCameraController cameraController)
	{
		cameraController.ZoomTo(_targetZoom);
	}
}
