using UnityEngine;

namespace Core.Audio;

public class OrientListener : MonoBehaviour
{
	[SerializeField]
	private Transform _cameraTransform;

	private void LateUpdate()
	{
		if ((bool)_cameraTransform)
		{
			base.transform.forward = _cameraTransform.forward;
		}
	}
}
