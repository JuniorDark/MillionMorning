using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.PVP;

public class PVPCircle : MonoBehaviour
{
	[SerializeField]
	private Image circle1;

	[SerializeField]
	private Image circle2;

	[SerializeField]
	[Range(0f, 5f)]
	private int speed;

	private bool _isOn;

	public void SetSpeed(int newSpeed)
	{
		speed = Mathf.Clamp(newSpeed, 0, 5);
	}

	private void Update()
	{
		if (speed == 0)
		{
			if (_isOn)
			{
				LeanTween.scale(circle1.gameObject, Vector3.zero, 0.3f);
				LeanTween.scale(circle2.gameObject, Vector3.zero, 0.3f);
				circle1.color = new Color(1f, 1f, 1f, 0f);
				circle2.color = new Color(1f, 1f, 1f, 0f);
				_isOn = false;
			}
			return;
		}
		if (!_isOn)
		{
			LeanTween.scale(circle1.gameObject, Vector3.one, 0.3f);
			LeanTween.scale(circle2.gameObject, Vector3.one, 0.3f);
			circle1.color = new Color(1f, 1f, 1f, 0.05f);
			circle2.color = new Color(1f, 1f, 1f, 0.2f);
			_isOn = true;
		}
		Transform transform = circle1.transform;
		Transform obj = circle2.transform;
		Vector3 localEulerAngles = transform.localEulerAngles;
		Vector3 localEulerAngles2 = obj.localEulerAngles;
		localEulerAngles.z += Time.deltaTime * (float)speed * 10f;
		localEulerAngles2.z += Time.deltaTime * (float)speed * 8f;
		transform.localEulerAngles = localEulerAngles;
		obj.localEulerAngles = localEulerAngles2;
	}
}
