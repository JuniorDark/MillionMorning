using Code.Core.ResourceSystem;
using Code.World;

namespace Code.Core.Camera.CameraActions;

public class MilMo_CameraActionEndMovie : MilMo_CameraAction
{
	public MilMo_CameraActionEndMovie(float time)
		: base(time)
	{
	}

	public override void Read(MilMo_SFFile file)
	{
	}

	protected override void ExecuteInternal(MilMo_MovieCameraController cameraController)
	{
		cameraController.StopMovie(defaultSettings: true);
		MilMo_World.Instance.Camera.HookupCurrentPlayCamera();
	}
}
