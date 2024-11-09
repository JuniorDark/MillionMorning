using Core.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Marker.Combat;

public class HealthBar : MonoBehaviour
{
	[Header("HealthBar")]
	[SerializeField]
	protected Slider slider;

	[SerializeField]
	private Image fill;

	[SerializeField]
	private GameObject container;

	private void Awake()
	{
		if (slider == null)
		{
			Debug.LogError(base.gameObject.name + ": targetHealth is null");
		}
		if (fill == null)
		{
			Debug.LogError(base.gameObject.name + ": fill is null");
		}
		if (container == null)
		{
			Debug.LogError(base.gameObject.name + ": container is null");
		}
	}

	public void UpdateHealth(float newHealth, float newMaxHealth)
	{
		float num = ((newHealth != 0f && newMaxHealth != 0f) ? (newHealth / newMaxHealth) : 0f);
		if (slider != null && Maths.FloatNotEquals(slider.value, num))
		{
			slider.value = num;
		}
	}

	public void SetFillColor(Color color)
	{
		if (fill != null)
		{
			fill.color = color;
		}
	}

	public void Show(bool shouldShow)
	{
		if (!(container == null) && container.gameObject.activeSelf != shouldShow)
		{
			container.gameObject.SetActive(shouldShow);
		}
	}
}
