using System;
using System.Collections;
using UnityEngine;

namespace Code.Core.Utility;

public class HeadLookController : MonoBehaviour
{
	public Transform rootNode;

	public BendingSegment[] segments;

	public NonAffectedJoints[] nonAffectedJoints;

	public Vector3 headLookVector = Vector3.forward;

	public Vector3 headUpVector = Vector3.up;

	public Vector3 target = Vector3.zero;

	public bool overrideAnimation;

	public float rotationMultiplier = 1f;

	public bool paused;

	public void Initialize()
	{
		if (!rootNode)
		{
			rootNode = base.transform;
		}
		BendingSegment[] array = segments;
		foreach (BendingSegment bendingSegment in array)
		{
			Quaternion quaternion = Quaternion.Inverse(bendingSegment.firstTransform.parent.rotation);
			Quaternion rotation = rootNode.rotation;
			bendingSegment.ReferenceLookDir = quaternion * rotation * headLookVector.normalized;
			bendingSegment.ReferenceUpDir = quaternion * rotation * headUpVector.normalized;
			bendingSegment.AngleH = 0f;
			bendingSegment.AngleV = 0f;
			bendingSegment.DirUp = bendingSegment.ReferenceUpDir;
			bendingSegment.ChainLength = 1;
			Transform transform = bendingSegment.lastTransform;
			while (transform != bendingSegment.firstTransform && transform != transform.root)
			{
				bendingSegment.ChainLength++;
				transform = transform.parent;
			}
			bendingSegment.OrigRotations = new Quaternion[bendingSegment.ChainLength];
			transform = bendingSegment.lastTransform;
			for (int num = bendingSegment.ChainLength - 1; num >= 0; num--)
			{
				bendingSegment.OrigRotations[num] = transform.localRotation;
				transform = transform.parent;
			}
		}
	}

	public void Reset()
	{
		BendingSegment[] array = segments;
		foreach (BendingSegment obj in array)
		{
			obj.AngleH = 0f;
			obj.AngleV = 0f;
			obj.DirUp = obj.ReferenceUpDir;
		}
	}

	private void LateUpdate()
	{
		if ((double)Math.Abs(Time.deltaTime) < 0.0)
		{
			return;
		}
		Vector3[] array = new Vector3[nonAffectedJoints.Length];
		for (int i = 0; i < nonAffectedJoints.Length; i++)
		{
			if (nonAffectedJoints[i] == null)
			{
				continue;
			}
			Transform transform = nonAffectedJoints[i].child;
			if (!transform)
			{
				{
					IEnumerator enumerator = nonAffectedJoints[i].joint.GetEnumerator();
					try
					{
						if (enumerator.MoveNext())
						{
							transform = (Transform)enumerator.Current;
						}
					}
					finally
					{
						IDisposable disposable = enumerator as IDisposable;
						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
				}
			}
			if (!transform)
			{
				array[i] = transform.position - nonAffectedJoints[i].joint.position;
			}
		}
		BendingSegment[] array2 = segments;
		foreach (BendingSegment bendingSegment in array2)
		{
			Transform transform2 = bendingSegment.lastTransform;
			if (overrideAnimation)
			{
				for (int num = bendingSegment.ChainLength - 1; num >= 0; num--)
				{
					transform2.localRotation = bendingSegment.OrigRotations[num];
					transform2 = transform2.parent;
				}
			}
			Quaternion rotation = bendingSegment.firstTransform.parent.rotation;
			Quaternion quaternion = Quaternion.Inverse(rotation);
			Vector3 normalized = (target - bendingSegment.lastTransform.position).normalized;
			Vector3 vector = quaternion * normalized;
			float f = AngleAroundAxis(bendingSegment.ReferenceLookDir, vector, bendingSegment.ReferenceUpDir);
			Vector3 axis = Vector3.Cross(bendingSegment.ReferenceUpDir, vector);
			float f2 = AngleAroundAxis(vector - Vector3.Project(vector, bendingSegment.ReferenceUpDir), vector, axis);
			float f3 = Mathf.Max(0f, Mathf.Abs(f) - bendingSegment.thresholdAngleDifference) * Mathf.Sign(f);
			float f4 = Mathf.Max(0f, Mathf.Abs(f2) - bendingSegment.thresholdAngleDifference) * Mathf.Sign(f2);
			f = Mathf.Max(Mathf.Abs(f3) * Mathf.Abs(bendingSegment.bendingMultiplier), Mathf.Abs(f) - bendingSegment.maxAngleDifference) * Mathf.Sign(f) * Mathf.Sign(bendingSegment.bendingMultiplier);
			f2 = Mathf.Max(Mathf.Abs(f4) * Mathf.Abs(bendingSegment.bendingMultiplier), Mathf.Abs(f2) - bendingSegment.maxAngleDifference) * Mathf.Sign(f2) * Mathf.Sign(bendingSegment.bendingMultiplier);
			f = Mathf.Clamp(f, 0f - bendingSegment.maxBendingAngle, bendingSegment.maxBendingAngle);
			f2 = Mathf.Clamp(f2, 0f - bendingSegment.maxBendingAngle, bendingSegment.maxBendingAngle);
			Vector3 axis2 = Vector3.Cross(bendingSegment.ReferenceUpDir, bendingSegment.ReferenceLookDir);
			if (!paused)
			{
				bendingSegment.AngleH = Mathf.Lerp(bendingSegment.AngleH, f, Time.deltaTime * bendingSegment.responsiveness);
				bendingSegment.AngleV = Mathf.Lerp(bendingSegment.AngleV, f2, Time.deltaTime * bendingSegment.responsiveness);
			}
			vector = Quaternion.AngleAxis(bendingSegment.AngleH, bendingSegment.ReferenceUpDir) * Quaternion.AngleAxis(bendingSegment.AngleV, axis2) * bendingSegment.ReferenceLookDir;
			Vector3 tangent = bendingSegment.ReferenceUpDir;
			Vector3.OrthoNormalize(ref vector, ref tangent);
			Vector3 normal = vector;
			if (!paused)
			{
				bendingSegment.DirUp = Vector3.Slerp(bendingSegment.DirUp, tangent, Time.deltaTime * 5f);
			}
			Vector3.OrthoNormalize(ref normal, ref bendingSegment.DirUp);
			Quaternion b = rotation * Quaternion.LookRotation(normal, bendingSegment.DirUp) * Quaternion.Inverse(rotation * Quaternion.LookRotation(bendingSegment.ReferenceLookDir, bendingSegment.ReferenceUpDir));
			Quaternion quaternion2 = Quaternion.Slerp(Quaternion.identity, b, rotationMultiplier / (float)bendingSegment.ChainLength);
			transform2 = bendingSegment.lastTransform;
			for (int k = 0; k < bendingSegment.ChainLength; k++)
			{
				transform2.rotation = quaternion2 * transform2.rotation;
				transform2 = transform2.parent;
			}
		}
		for (int l = 0; l < nonAffectedJoints.Length; l++)
		{
			Vector3 vector2 = Vector3.zero;
			if (nonAffectedJoints[l] == null)
			{
				continue;
			}
			Transform transform3 = nonAffectedJoints[l].child;
			if (!transform3)
			{
				{
					IEnumerator enumerator = nonAffectedJoints[l].joint.GetEnumerator();
					try
					{
						if (enumerator.MoveNext())
						{
							transform3 = (Transform)enumerator.Current;
						}
					}
					finally
					{
						IDisposable disposable2 = enumerator as IDisposable;
						if (disposable2 != null)
						{
							disposable2.Dispose();
						}
					}
				}
			}
			if (!transform3)
			{
				vector2 = transform3.position - nonAffectedJoints[l].joint.position;
			}
			Vector3 toDirection = Vector3.Slerp(array[l], vector2, nonAffectedJoints[l].effect);
			nonAffectedJoints[l].joint.rotation = Quaternion.FromToRotation(vector2, toDirection) * nonAffectedJoints[l].joint.rotation;
		}
	}

	private static float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
	{
		dirA -= Vector3.Project(dirA, axis);
		dirB -= Vector3.Project(dirB, axis);
		return Vector3.Angle(dirA, dirB) * (float)((!(Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0f)) ? 1 : (-1));
	}
}
