using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_SinkEffect : MilMo_ObjectEffect
{
	private float _time;

	private float _velocity;

	private bool _isSetup;

	private MilMo_SinkEffectTemplate Template => EffectTemplate as MilMo_SinkEffectTemplate;

	public override float Duration => Template.Duration;

	public MilMo_SinkEffect(GameObject gameObject, MilMo_SinkEffectTemplate template)
		: base(gameObject, template)
	{
	}

	public override bool Update()
	{
		if (GameObject == null || _time > Template.Duration)
		{
			Destroy();
			return false;
		}
		if (!_isSetup)
		{
			Setup();
		}
		GameObject.transform.position += Time.deltaTime * _velocity * Vector3.down;
		_time += Time.deltaTime;
		return true;
	}

	private void Setup()
	{
		if (Template.Duration > 0f)
		{
			Renderer componentInChildren = GameObject.GetComponentInChildren<Renderer>();
			if (!(componentInChildren == null))
			{
				float num = componentInChildren.bounds.extents.y * 2f;
				_velocity = num / Template.Duration - _time;
				_isSetup = true;
			}
		}
		else
		{
			_isSetup = true;
		}
	}
}
