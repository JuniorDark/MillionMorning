using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Camera.CameraActions;

public class MilMo_CameraActionAngleImpulse : MilMo_CameraAction
{
	private Vector3 _impulse;

	public MilMo_CameraActionAngleImpulse(float time)
		: base(time)
	{
	}

	public override void Read(MilMo_SFFile file)
	{
		_impulse = file.GetVector3();
	}

	protected override void ExecuteInternal(MilMo_MovieCameraController cameraController)
	{
		cameraController.AngleImpulse(_impulse);
	}
}
