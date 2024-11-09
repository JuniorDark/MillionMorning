using System;
using UnityEngine;

namespace Code.Core.Utility;

[Serializable]
public class BendingSegment
{
	public Transform firstTransform;

	public Transform lastTransform;

	public float thresholdAngleDifference;

	public float bendingMultiplier = 0.6f;

	public float maxAngleDifference = 30f;

	public float maxBendingAngle = 80f;

	public float responsiveness = 5f;

	internal float AngleH;

	internal float AngleV;

	internal Vector3 DirUp;

	internal Vector3 ReferenceLookDir;

	internal Vector3 ReferenceUpDir;

	internal int ChainLength;

	internal Quaternion[] OrigRotations;
}
