using Code.Core.Avatar;
using Code.Core.Network.types;
using UnityEngine;

namespace Code.Core.PlayerState;

public class MilMo_PlayerStateEffectChangeParticle : MilMo_PlayerStateEffect
{
	private readonly string _newParticleName;

	private readonly string _particleToChange;

	public MilMo_PlayerStateEffectChangeParticle(PlayerStateEffectCosmetic effectData, MilMo_Avatar avatar)
		: base(avatar)
	{
		if (effectData.GetParameters().Count > 1)
		{
			_particleToChange = effectData.GetParameters()[0];
			_newParticleName = effectData.GetParameters()[1];
		}
		else
		{
			Debug.LogWarning("Missing parameter in \"ChangeParticle\" player state effect");
		}
	}

	public override void Activate()
	{
		if (Avatar != null && _particleToChange != null && _newParticleName != null)
		{
			Avatar.StackParticleEffect(_particleToChange, _newParticleName);
		}
	}

	public override void Deactivate()
	{
		if (Avatar != null && _particleToChange != null)
		{
			Avatar.UnstackParticleEffect(_particleToChange, _newParticleName);
		}
	}
}
