using UnityEngine;

namespace Code.Core.Avatar.Ragdoll;

public class CapsuleColliderInfo : ColliderInfo
{
	private readonly float _radius;

	private readonly float _height;

	private readonly int _direction;

	private readonly Vector3 _center;

	public CapsuleColliderInfo(float radius, float height, int direction, Vector3 center)
	{
		_radius = radius;
		_height = height;
		_direction = direction;
		_center = center;
	}

	public override void Attach(GameObject gameObject)
	{
		CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
		capsuleCollider.radius = _radius;
		capsuleCollider.height = _height;
		capsuleCollider.direction = _direction;
		capsuleCollider.center = _center;
	}
}
