using Code.Core.ResourceSystem;

namespace Code.Core.Camera.CameraActions;

public class MilMo_CameraActionShake : MilMo_CameraAction
{
	private bool _shake;

	public MilMo_CameraActionShake(float time)
		: base(time)
	{
	}

	public override void Read(MilMo_SFFile file)
	{
		_shake = file.GetBool();
	}

	protected override void ExecuteInternal(MilMo_MovieCameraController cameraController)
	{
		cameraController.Shake(_shake);
	}
}
