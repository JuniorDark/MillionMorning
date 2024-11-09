using Code.Core.Avatar;
using Code.Core.Network.types;
using Code.Core.Visual.Effect;
using UnityEngine;

namespace Code.Core.PlayerState;

public class MilMo_PlayerStateEffectParticle : MilMo_PlayerStateEffect
{
	private readonly string _particleName = "";

	private MilMo_Effect _effect;

	public MilMo_PlayerStateEffectParticle(PlayerStateEffectCosmetic effectData, MilMo_Avatar avatar)
		: base(avatar)
	{
		if (effectData.GetParameters().Count > 0)
		{
			_particleName = effectData.GetParameters()[0];
		}
		else
		{
			Debug.LogWarning("Missing parameter in \"Particle\" player state effect");
		}
	}

	public override void Activate()
	{
		if (Avatar != null)
		{
			_effect = MilMo_EffectContainer.GetEffect(_particleName, Avatar.GameObject);
		}
	}

	public override void Update()
	{
		base.Update();
		if (_effect != null)
		{
			_effect.Update();
		}
	}

	public override void Deactivate()
	{
		if (_effect != null)
		{
			_effect.Stop();
			_effect.Destroy();
		}
	}
}
