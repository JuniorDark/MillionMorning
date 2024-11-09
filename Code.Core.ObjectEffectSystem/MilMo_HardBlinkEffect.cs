using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_HardBlinkEffect : MilMo_ObjectEffect
{
	private readonly Vector3 _originalScale;

	private float _time;

	private float _nextChange;

	private bool _visible = true;

	private MilMo_HardBlinkEffectTemplate Template => EffectTemplate as MilMo_HardBlinkEffectTemplate;

	public override float Duration => Template.Duration;

	public MilMo_HardBlinkEffect(GameObject gameObject, MilMo_HardBlinkEffectTemplate template)
		: base(gameObject, template)
	{
		if (GameObject != null)
		{
			_originalScale = GameObject.transform.localScale;
		}
	}

	public override bool Update()
	{
		if (GameObject == null)
		{
			Destroy();
			return false;
		}
		_time += Time.deltaTime;
		if (_time >= Duration)
		{
			GameObject.transform.localScale = _originalScale;
			Destroy();
			return false;
		}
		if (_time >= _nextChange)
		{
			if (_visible)
			{
				GameObject.transform.localScale = Vector3.zero;
				_nextChange += Template.InvisibleTime;
			}
			else
			{
				GameObject.transform.localScale = _originalScale;
				_nextChange += Template.VisibleTime;
			}
			_visible = !_visible;
		}
		return true;
	}
}
