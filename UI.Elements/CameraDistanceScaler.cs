using UnityEngine;

namespace UI.Elements;

public class CameraDistanceScaler : MonoBehaviour
{
	private Camera _cam;

	[SerializeField]
	private float scaleFactor = 0.03f;

	[SerializeField]
	private float maxDistance = 10f;

	private void OnEnable()
	{
		_cam = Camera.main;
	}

	protected void Update()
	{
		if ((bool)_cam)
		{
			float fieldOfView = _cam.fieldOfView;
			float num = scaleFactor * fieldOfView / 60f;
			float num2 = Mathf.Clamp((_cam.transform.position - base.transform.position).magnitude * num, 0f, maxDistance);
			base.transform.localScale = Vector3.one * num2;
		}
	}
}
