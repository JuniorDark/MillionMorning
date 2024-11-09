using System.Collections.Generic;
using UnityEngine;

namespace Code.Core.Avatar.Ragdoll;

public class TransformInfo
{
	private readonly ColliderInfo _collider;

	private readonly RigidBodyInfo _rigidBody;

	private readonly CharacterJointInfo _characterJoint;

	public TransformInfo(ColliderInfo collider, RigidBodyInfo rigidBody, CharacterJointInfo characterJoint)
	{
		_collider = collider;
		_rigidBody = rigidBody;
		_characterJoint = characterJoint;
	}

	public void Setup(GameObject gameObject, IDictionary<string, Transform> iterationTransforms)
	{
		_collider?.Attach(gameObject);
		_rigidBody?.Attach(gameObject);
		_characterJoint?.Attach(gameObject, iterationTransforms);
	}
}
