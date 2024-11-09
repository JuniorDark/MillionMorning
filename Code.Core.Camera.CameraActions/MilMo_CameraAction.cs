using Code.Core.ResourceSystem;

namespace Code.Core.Camera.CameraActions;

public abstract class MilMo_CameraAction
{
	public float Time { get; private set; }

	protected MilMo_CameraAction(float time)
	{
		Time = time;
	}

	public void Execute(object cameraController)
	{
		ExecuteInternal((MilMo_MovieCameraController)cameraController);
	}

	public abstract void Read(MilMo_SFFile file);

	protected virtual void ExecuteInternal(MilMo_MovieCameraController cameraController)
	{
	}
}
