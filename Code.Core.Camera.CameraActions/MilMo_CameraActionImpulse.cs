using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Camera.CameraActions;

public class MilMo_CameraActionImpulse : MilMo_CameraAction
{
	private Vector3 _impulse;

	public MilMo_CameraActionImpulse(float time)
		: base(time)
	{
	}

	public override void Read(MilMo_SFFile file)
	{
		_impulse = file.GetVector3();
	}

	protected override void ExecuteInternal(MilMo_MovieCameraController cameraController)
	{
		cameraController.Impulse(_impulse);
	}
}
