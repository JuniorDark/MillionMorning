using UnityEngine;

namespace UI.Elements;

public class Rotation : MonoBehaviour
{
	[SerializeField]
	private Vector3 rotationSpeed;

	private void Update()
	{
		base.transform.Rotate(rotationSpeed * Time.deltaTime);
	}
}
