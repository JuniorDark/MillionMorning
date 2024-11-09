using UnityEngine;

namespace Code.Core.Camera;

public class MolMo_CameraShake : MonoBehaviour
{
	private float shakeDuration;

	private float shakeAmount = 0.7f;

	private float shakeTime;

	private MilMo_GameCameraController controller;

	private bool initialized;

	public void Init(float duration, float amount, MilMo_GameCameraController controller)
	{
		shakeDuration = duration;
		shakeAmount = amount;
		this.controller = controller;
		initialized = true;
	}

	private void Update()
	{
		if (initialized)
		{
			shakeTime += Time.deltaTime;
			if (shakeDuration <= 0f || shakeTime >= shakeDuration)
			{
				controller.EffectPosition = Vector3.zero;
				Object.Destroy(base.gameObject);
			}
			else
			{
				controller.EffectPosition = Random.insideUnitSphere * (shakeAmount * (shakeDuration / shakeTime));
			}
		}
	}
}
