using System.Collections.Generic;
using UnityEngine;

namespace Code.Core.Avatar.Ragdoll;

public class CharacterJointInfo
{
	private readonly string _connectedBody;

	private readonly Vector3 _anchor;

	private readonly Vector3 _axis;

	private readonly Vector3 _swinAxis;

	private readonly Vector4 _lowTwistLimit;

	private readonly Vector4 _highTwistLimit;

	private readonly Vector4 _swing1Limit;

	private readonly Vector4 _swing2Limit;

	public CharacterJointInfo(string connectedBody, Vector3 anchor, Vector3 axis, Vector3 swinAxis, Vector4 lowTwistLimit, Vector4 highTwistLimit, Vector4 swing1Limit, Vector4 swing2Limit)
	{
		_connectedBody = connectedBody;
		_anchor = anchor;
		_axis = axis;
		_swinAxis = swinAxis;
		_lowTwistLimit = lowTwistLimit;
		_highTwistLimit = highTwistLimit;
		_swing1Limit = swing1Limit;
		_swing2Limit = swing2Limit;
	}

	public void Attach(GameObject gameObject, IDictionary<string, Transform> iterationTransforms)
	{
		CharacterJoint characterJoint = gameObject.AddComponent<CharacterJoint>();
		characterJoint.connectedBody = iterationTransforms[_connectedBody].gameObject.GetComponent<Rigidbody>();
		characterJoint.anchor = _anchor;
		characterJoint.axis = _axis;
		characterJoint.swingAxis = _swinAxis;
		SoftJointLimit lowTwistLimit = new SoftJointLimit
		{
			bounciness = _lowTwistLimit.x,
			limit = _lowTwistLimit.z
		};
		characterJoint.lowTwistLimit = lowTwistLimit;
		SoftJointLimit highTwistLimit = new SoftJointLimit
		{
			bounciness = _highTwistLimit.x,
			limit = _highTwistLimit.z
		};
		characterJoint.highTwistLimit = highTwistLimit;
		SoftJointLimit swing1Limit = new SoftJointLimit
		{
			bounciness = _swing1Limit.x,
			limit = _swing1Limit.z
		};
		characterJoint.swing1Limit = swing1Limit;
		SoftJointLimit swing2Limit = new SoftJointLimit
		{
			bounciness = _swing2Limit.x,
			limit = _swing2Limit.z
		};
		characterJoint.swing2Limit = swing2Limit;
	}
}
