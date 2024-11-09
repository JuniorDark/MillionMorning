using System;
using Code.Core.Avatar;
using Code.Core.Network.types;

namespace Code.Core.PlayerState;

public abstract class MilMo_PlayerStateEffect
{
	protected MilMo_Avatar Avatar;

	protected MilMo_PlayerStateEffect(MilMo_Avatar avatar)
	{
		Avatar = avatar;
	}

	public static MilMo_PlayerStateEffect Create(PlayerStateEffectCosmetic effectData, MilMo_Avatar avatar)
	{
		if (effectData.GetTemplateType().Equals("PlayAnimation", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_PlayerStateEffectPlayAnimation(effectData, avatar);
		}
		if (effectData.GetTemplateType().Equals("Particle", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_PlayerStateEffectParticle(effectData, avatar);
		}
		if (effectData.GetTemplateType().Equals("Sound", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_PlayerStateEffectSound(effectData, avatar);
		}
		if (effectData.GetTemplateType().Equals("Mood", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_PlayerStateEffectMood(effectData, avatar);
		}
		if (effectData.GetTemplateType().Equals("ChangeAnimation", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_PlayerStateEffectChangeAnimation(effectData, avatar);
		}
		if (effectData.GetTemplateType().Equals("ChangeParticle", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_PlayerStateEffectChangeParticle(effectData, avatar);
		}
		if (effectData.GetTemplateType().Equals("ChangePuff", StringComparison.InvariantCultureIgnoreCase))
		{
			return new MilMo_PlayerStateEffectChangePuff(effectData, avatar);
		}
		return null;
	}

	public abstract void Activate();

	public virtual void Update()
	{
	}

	public abstract void Deactivate();
}
