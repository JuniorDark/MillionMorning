using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_SpinEffect : MilMo_ObjectEffect
{
	private float _time;

	private float _velocity;

	private MilMo_SpinEffectTemplate Template => EffectTemplate as MilMo_SpinEffectTemplate;

	public override float Duration => Template.Duration;

	public MilMo_SpinEffect(GameObject gameObject, MilMo_SpinEffectTemplate template)
		: base(gameObject, template)
	{
		_velocity = template.Velocity;
	}

	public override bool Update()
	{
		if (GameObject == null || _time > Template.Duration)
		{
			Destroy();
			return false;
		}
		float num = _velocity * Time.deltaTime + Template.Acceleration * Time.deltaTime * Time.deltaTime * 0.5f;
		GameObject.transform.Rotate(Vector3.up * num);
		_velocity += Template.Acceleration * Time.deltaTime;
		_time += Time.deltaTime;
		return true;
	}
}
