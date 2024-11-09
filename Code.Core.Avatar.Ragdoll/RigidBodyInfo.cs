using UnityEngine;

namespace Code.Core.Avatar.Ragdoll;

public class RigidBodyInfo
{
	private readonly float _mass;

	private readonly float _drag;

	private readonly float _angularDrag;

	private readonly bool _useGravity;

	public RigidBodyInfo(float mass, float drag, float angularDrag, bool useGravity)
	{
		_mass = mass;
		_drag = drag;
		_angularDrag = angularDrag;
		_useGravity = useGravity;
	}

	public void Attach(GameObject gameObject)
	{
		Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
		rigidbody.mass = _mass;
		rigidbody.drag = _drag;
		rigidbody.angularDrag = _angularDrag;
		rigidbody.useGravity = _useGravity;
	}
}
