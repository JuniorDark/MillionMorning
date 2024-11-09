using UnityEngine;

namespace UI.Elements;

public class CameraFacingBillboard : MonoBehaviour
{
	[SerializeField]
	private bool onlyY;

	private Camera _cam;

	private void OnEnable()
	{
		_cam = Camera.main;
	}

	protected void Update()
	{
		if ((bool)_cam)
		{
			if (onlyY)
			{
				Vector3 view = base.transform.position - _cam.transform.position;
				Quaternion localRotation = default(Quaternion);
				localRotation.SetLookRotation(view, Vector3.up);
				localRotation.x = 0f;
				localRotation.z = 0f;
				base.transform.localRotation = localRotation;
			}
			else
			{
				Quaternion rotation = _cam.transform.rotation;
				base.transform.LookAt(base.transform.position + rotation * Vector3.forward, rotation * Vector3.up);
			}
		}
	}
}
