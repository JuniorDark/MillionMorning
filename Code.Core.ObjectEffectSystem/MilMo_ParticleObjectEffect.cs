using Code.Core.Visual.Effect;
using UnityEngine;

namespace Code.Core.ObjectEffectSystem;

public class MilMo_ParticleObjectEffect : MilMo_ObjectEffect
{
	private MilMo_Effect _effect;

	public override float Duration
	{
		get
		{
			if (_effect == null)
			{
				return 0f;
			}
			return _effect.Duration;
		}
	}

	public MilMo_ParticleObjectEffect(GameObject gameObject, MilMo_ParticleObjectEffectTemplate template)
		: base(gameObject, template)
	{
		_effect = MilMo_EffectContainer.GetEffect(template.ParticleEffectScriptFile, gameObject);
	}

	public override bool Update()
	{
		if (_effect == null)
		{
			Debug.Log("Particle Object effect is null");
			Destroy();
			return false;
		}
		if (_effect.Update())
		{
			return true;
		}
		Destroy();
		return false;
	}

	public override void Destroy()
	{
		base.Destroy();
		if (_effect != null)
		{
			_effect.Destroy();
			_effect = null;
		}
	}
}
