using System;
using UnityEngine;

namespace Code.Core.Utility;

[Serializable]
public class NonAffectedJoints
{
	public Transform joint;

	public float effect;

	public Transform child;
}
