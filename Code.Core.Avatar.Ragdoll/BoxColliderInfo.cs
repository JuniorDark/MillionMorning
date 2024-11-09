using UnityEngine;

namespace Code.Core.Avatar.Ragdoll;

public class BoxColliderInfo : ColliderInfo
{
	private readonly Vector3 _size;

	private readonly Vector3 _center;

	public BoxColliderInfo(Vector3 size, Vector3 center)
	{
		_size = size;
		_center = center;
	}

	public override void Attach(GameObject gameObject)
	{
		BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
		boxCollider.size = _size;
		boxCollider.center = _center;
	}
}
