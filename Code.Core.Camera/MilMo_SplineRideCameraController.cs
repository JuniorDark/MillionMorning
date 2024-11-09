using Code.Core.Utility;
using Code.World.Gameplay;
using UnityEngine;

namespace Code.Core.Camera;

public class MilMo_SplineRideCameraController : MilMo_CameraController
{
	private const float TERRAIN_LOD_DISTANCE_MULTIPLIER = 1.5f;

	private const float DIRECTION_SMOOTHING_SPEED = 10f;

	private Vector3 _direction;

	public MilMo_PlayerSpline Spline { get; set; }

	public override void HookUp()
	{
		base.HookUp();
		if (Spline != null)
		{
			MilMo_CameraController.CameraComponent.fieldOfView = Spline.Template.FOV;
			MilMo_CameraController.Distance = Spline.Template.CameraDistance;
		}
		Vector3 vector = MilMo_CameraController.Player.position + 0.7f * MilMo_CameraController.Player.up;
		if (Spline != null)
		{
			_direction = (Spline.GetEndpoint() - vector).normalized;
		}
		Terrain.activeTerrain.basemapDistance *= 1.5f;
	}

	public override void Unhook()
	{
		base.Unhook();
		if ((bool)Terrain.activeTerrain)
		{
			Terrain.activeTerrain.basemapDistance /= 1.5f;
		}
	}

	public virtual void Update()
	{
		if (!base.HookedUp || Spline == null)
		{
			return;
		}
		Vector3 vector = MilMo_CameraController.Player.position + 0.7f * MilMo_CameraController.Player.up;
		Vector3 vector2 = Spline.GetEndpoint() - vector;
		if (!MilMo_Utility.Equals(vector2, Vector3.zero))
		{
			vector2.Normalize();
			Vector3 a = Vector3.Lerp(_direction, vector2, 10f * Time.deltaTime);
			if (!MilMo_Utility.Equals(a, Vector3.zero))
			{
				_direction = a.normalized;
			}
		}
		Vector3 position = vector - _direction * MilMo_CameraController.Distance;
		MilMo_CameraController.CameraTransform.position = position;
		MilMo_CameraController.SetupRotation(vector);
		MilMo_CameraController.UpdateAudioListenerPosition();
	}
}
