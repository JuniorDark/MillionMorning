using UnityEngine;

namespace Code.Core.Camera;

public class MilMo_ShopCameraController : MilMo_CameraController
{
	public static readonly Vector3 ShopPosition = new Vector3(0f, -240f, 0f);

	private const float PAN_CHARACTER = 0f;

	private const float PAN_FLOOR_FURNITURE = 0f;

	private const float PAN_WALL_FURNITURE = 0f;

	private const float PAN_WALLPAPER = 0f;

	private const float PAN_FLOOR = 0f;

	private const float PAN_ROOM = 0f;

	private const float LOOKUP_CHARACTER = 0f;

	private const float LOOKUP_FLOOR_FURNITURE = 30f;

	private const float LOOKUP_WALL_FURNITURE = 0f;

	private const float LOOKUP_WALLPAPER = 0f;

	private const float LOOKUP_FLOOR = 30f;

	private const float LOOKUP_ROOM = 45f;

	private float _pan;

	private float _wantedPan;

	private float _lookup;

	private float _wantedLookup;

	private Color _backgroundColorResetVal;

	private float _farPlaneResetVal;

	private float _nearPlaneResetVal;

	private float _fovResetVal;

	private float _depthResetVal;

	private const float LOOKUP_DAMPING = 5f;

	private const float PAN_DAMPING = 5f;

	private const float DISTANCE = 7.6f;

	public override void HookUp()
	{
		base.HookUp();
		_backgroundColorResetVal = MilMo_CameraController.CameraComponent.backgroundColor;
		_farPlaneResetVal = MilMo_CameraController.CameraComponent.farClipPlane;
		_nearPlaneResetVal = MilMo_CameraController.CameraComponent.nearClipPlane;
		_fovResetVal = MilMo_CameraController.CameraComponent.fieldOfView;
		_depthResetVal = MilMo_CameraController.CameraComponent.depth;
		LookAtPosition = ShopPosition + new Vector3(0f, -49.4f, 0f);
		MilMo_CameraController.CameraComponent.backgroundColor = new Color(0.7f, 0.7f, 1f);
		MilMo_CameraController.CameraComponent.farClipPlane = 1000f;
		MilMo_CameraController.CameraComponent.nearClipPlane = 0.1f;
		MilMo_CameraController.CameraComponent.fieldOfView = 20f;
		MilMo_CameraController.CameraComponent.depth = 0f;
		_wantedPan = 0f;
		_wantedLookup = 0f;
		_pan = _wantedPan;
		_lookup = _wantedLookup;
		RenderSettings.fog = false;
	}

	public virtual void Update()
	{
		if (base.HookedUp)
		{
			_pan = Mathf.Lerp(_pan, _wantedPan, 5f * Time.deltaTime);
			_lookup = Mathf.LerpAngle(_lookup, _wantedLookup, 5f * Time.deltaTime);
			Quaternion quaternion = Quaternion.Euler(_lookup, _pan, 0f);
			Vector3 position = LookAtPosition + quaternion * new Vector3(0f, 0f, -7.6f);
			MilMo_CameraController.CameraTransform.position = position;
			MilMo_CameraController.CameraTransform.rotation = quaternion;
		}
	}

	public void SetViewModeCharacter()
	{
		_wantedLookup = 0f;
		_wantedPan = 0f;
	}

	public void SetViewModeFloorFurniture()
	{
		_wantedLookup = 30f;
		_wantedPan = 0f;
	}

	public void SetViewModeWallFurniture()
	{
		_wantedLookup = 0f;
		_wantedPan = 0f;
	}

	public void SetViewModeWallpaper()
	{
		_wantedLookup = 0f;
		_wantedPan = 0f;
	}

	public void SetViewModeRoom()
	{
		_wantedLookup = 45f;
		_wantedPan = 0f;
	}

	public void SetViewModeFloor()
	{
		_wantedLookup = 30f;
		_wantedPan = 0f;
	}

	public override void Unhook()
	{
		base.Unhook();
		MilMo_CameraController.CameraComponent.backgroundColor = _backgroundColorResetVal;
		MilMo_CameraController.CameraComponent.farClipPlane = _farPlaneResetVal;
		MilMo_CameraController.CameraComponent.nearClipPlane = _nearPlaneResetVal;
		MilMo_CameraController.CameraComponent.fieldOfView = _fovResetVal;
		MilMo_CameraController.CameraComponent.depth = _depthResetVal;
	}
}
