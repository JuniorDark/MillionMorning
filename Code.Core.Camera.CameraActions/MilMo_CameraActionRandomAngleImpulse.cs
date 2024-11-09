using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Camera.CameraActions;

public class MilMo_CameraActionRandomAngleImpulse : MilMo_CameraAction
{
	private Vector3 _min;

	private Vector3 _max;

	public MilMo_CameraActionRandomAngleImpulse(float time)
		: base(time)
	{
	}

	public override void Read(MilMo_SFFile file)
	{
		_min = file.GetVector3();
		_max = file.GetVector3();
	}

	protected override void ExecuteInternal(MilMo_MovieCameraController cameraController)
	{
		cameraController.RandomAngleImpulse(_min, _max);
	}
}
