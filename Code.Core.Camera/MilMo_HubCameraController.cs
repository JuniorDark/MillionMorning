using Code.World.GUI.Hub;
using UnityEngine;

namespace Code.Core.Camera;

public class MilMo_HubCameraController : MilMo_CameraController
{
	private const float PAN_TOWN = 0f;

	private const float LOOKUP_TOWN = 40f;

	private float _mPan;

	private float _mWantedPan;

	private float _mLookup;

	private float _mWantedLookup;

	private Color _mBackgroundColorResetVal;

	private float _mFarPlaneResetVal;

	private float _mNearPlaneResetVal;

	private float _mFovResetVal;

	private float _mDepthResetVal;

	private const float DISTANCE = 4.6f;

	public override void HookUp()
	{
		base.HookUp();
		_mBackgroundColorResetVal = MilMo_CameraController.CameraComponent.backgroundColor;
		_mFarPlaneResetVal = MilMo_CameraController.CameraComponent.farClipPlane;
		_mNearPlaneResetVal = MilMo_CameraController.CameraComponent.nearClipPlane;
		_mFovResetVal = MilMo_CameraController.CameraComponent.fieldOfView;
		_mDepthResetVal = MilMo_CameraController.CameraComponent.depth;
		MilMo_CameraController.CameraComponent.backgroundColor = new Color(0.22f, 0.86f, 1f, 1f);
		MilMo_CameraController.CameraComponent.farClipPlane = 1000f;
		MilMo_CameraController.CameraComponent.nearClipPlane = 0.1f;
		MilMo_CameraController.CameraComponent.fieldOfView = 45f;
		MilMo_CameraController.CameraComponent.depth = 0f;
		_mWantedPan = 0f;
		_mWantedLookup = 40f;
		_mPan = _mWantedPan;
		_mLookup = _mWantedLookup;
		LookAtPosition = MilMo_Hub.HubWorldPosition + new Vector3(0f, 0f, 0f);
		RenderSettings.fog = false;
	}

	public virtual void Update()
	{
		if (base.HookedUp)
		{
			_mPan = _mWantedPan;
			_mLookup = _mWantedLookup;
			Quaternion quaternion = Quaternion.Euler(_mLookup, _mPan, 0f);
			Vector3 position = LookAtPosition + quaternion * new Vector3(0f, 0f, -4.6f);
			MilMo_CameraController.CameraTransform.position = position;
			MilMo_CameraController.CameraTransform.rotation = quaternion;
		}
	}

	public void SetViewModeTown()
	{
		_mWantedLookup = 40f;
		_mWantedPan = 0f;
	}

	public override void Unhook()
	{
		base.Unhook();
		MilMo_CameraController.CameraComponent.backgroundColor = _mBackgroundColorResetVal;
		MilMo_CameraController.CameraComponent.farClipPlane = _mFarPlaneResetVal;
		MilMo_CameraController.CameraComponent.nearClipPlane = _mNearPlaneResetVal;
		MilMo_CameraController.CameraComponent.fieldOfView = _mFovResetVal;
		MilMo_CameraController.CameraComponent.depth = _mDepthResetVal;
	}
}
