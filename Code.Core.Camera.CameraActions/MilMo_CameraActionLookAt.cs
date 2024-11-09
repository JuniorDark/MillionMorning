using Code.Core.ResourceSystem;
using UnityEngine;

namespace Code.Core.Camera.CameraActions;

public class MilMo_CameraActionLookAt : MilMo_CameraAction
{
	private Vector3 _target;

	public MilMo_CameraActionLookAt(float time)
		: base(time)
	{
	}

	public override void Read(MilMo_SFFile file)
	{
		_target = file.GetVector3();
	}

	protected override void ExecuteInternal(MilMo_MovieCameraController cameraController)
	{
		cameraController.LookAt(_target);
	}
}
