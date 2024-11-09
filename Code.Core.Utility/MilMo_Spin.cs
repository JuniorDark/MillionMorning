using UnityEngine;

namespace Code.Core.Utility;

public class MilMo_Spin : MonoBehaviour
{
	public Vector3 spin = Vector3.zero;

	private void FixedUpdate()
	{
		base.transform.Rotate(spin.x, spin.y, spin.z);
	}
}
