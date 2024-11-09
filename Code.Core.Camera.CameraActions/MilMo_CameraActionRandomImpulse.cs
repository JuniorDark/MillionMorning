using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Camera.CameraActions;

public class MilMo_CameraActionRandomImpulse : MilMo_CameraAction
{
	private Vector3 _min;

	private Vector3 _max;

	public MilMo_CameraActionRandomImpulse(float time)
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
		cameraController.RandomImpulse(_min, _max);
	}
}
