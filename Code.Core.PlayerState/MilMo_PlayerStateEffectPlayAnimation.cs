using Code.Core.Avatar;
using Code.Core.Network.types;
using UnityEngine;

namespace Code.Core.PlayerState;

public class MilMo_PlayerStateEffectPlayAnimation : MilMo_PlayerStateEffect
{
	private readonly string _mAnimationName = "";

	public MilMo_PlayerStateEffectPlayAnimation(PlayerStateEffectCosmetic effectData, MilMo_Avatar avatar)
		: base(avatar)
	{
		if (effectData.GetParameters().Count > 0)
		{
			_mAnimationName = effectData.GetParameters()[0];
		}
		else
		{
			Debug.LogWarning("Missing parameter in \"PlayAnimation\" player state effect");
		}
	}

	public override void Activate()
	{
		if (Avatar != null)
		{
			Avatar.PlayAnimation(_mAnimationName);
		}
	}

	public override void Deactivate()
	{
	}
}
