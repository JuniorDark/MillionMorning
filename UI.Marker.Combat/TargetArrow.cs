using UnityEngine;

namespace UI.Marker.Combat;

public class TargetArrow : MonoBehaviour
{
	[Header("Arrow")]
	[SerializeField]
	private GameObject arrow;

	[SerializeField]
	private int arrowRotationSpeed = 100;

	[SerializeField]
	private int arrowMoveSpeed = 2;

	[SerializeField]
	private int arrowMoveAmount = 1;

	private Coroutine _rotationCoroutine;

	private void Awake()
	{
		if (arrow == null)
		{
			Debug.LogError(base.gameObject.name + ": Arrow mesh is null");
		}
	}

	public void Show(bool shouldShow)
	{
		if (base.gameObject.activeSelf != shouldShow)
		{
			if (arrow != null)
			{
				arrow.transform.localEulerAngles = Vector3.zero;
				arrow.transform.localPosition = Vector3.zero;
			}
			base.gameObject.SetActive(shouldShow);
		}
	}

	private void Update()
	{
		if (!(arrow == null))
		{
			float num = Time.deltaTime * (float)arrowRotationSpeed;
			Vector3 localEulerAngles = arrow.transform.localEulerAngles;
			arrow.transform.localEulerAngles = new Vector3(localEulerAngles.x, localEulerAngles.y + num, localEulerAngles.z);
			float y = Mathf.Sin(Time.time * (float)arrowMoveSpeed) * (float)arrowMoveAmount;
			Vector3 localPosition = arrow.transform.localPosition;
			arrow.transform.localPosition = new Vector3(localPosition.x, y, localPosition.z);
		}
	}
}
