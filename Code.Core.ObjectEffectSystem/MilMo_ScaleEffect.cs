using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_ScaleEffect : MilMo_ObjectEffect
{
	private readonly Vector3 _originalScale;

	private readonly float _duration;

	private MilMo_ScaleEffectTemplate Template => EffectTemplate as MilMo_ScaleEffectTemplate;

	public override float Duration => _duration;

	public MilMo_ScaleEffect(GameObject gameObject, MilMo_ScaleEffectTemplate template)
		: base(gameObject, template)
	{
		_originalScale = GameObject.transform.localScale;
		if (template.Speed > 0f)
		{
			GameObject.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
		}
		if (template.Speed == 0f)
		{
			_duration = 0f;
			return;
		}
		Vector3 localScale = GameObject.transform.localScale;
		_duration = ((!(template.Speed < 0f)) ? Mathf.Max(_originalScale.x - localScale.x, Mathf.Max(_originalScale.y - localScale.y, _originalScale.z - localScale.z)) : Mathf.Max(localScale.x, Mathf.Max(localScale.y, localScale.z))) / Mathf.Abs(template.Speed);
	}

	public override bool Update()
	{
		if (GameObject == null)
		{
			Destroy();
			return false;
		}
		GameObject.transform.localScale += Time.deltaTime * Template.Speed * Vector3.one;
		if (GameObject.transform.localScale.x >= _originalScale.x && GameObject.transform.localScale.y >= _originalScale.y && GameObject.transform.localScale.z >= _originalScale.z)
		{
			GameObject.transform.localScale = _originalScale;
			Destroy();
			return false;
		}
		if (GameObject.transform.localScale.x <= 0f && GameObject.transform.localScale.y <= 0f && GameObject.transform.localScale.z <= 0f)
		{
			Destroy();
			return false;
		}
		return true;
	}
}
