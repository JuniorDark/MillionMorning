using Code.Core.Avatar;
using Code.Core.Network.types;
using UnityEngine;

namespace Code.Core.PlayerState;

public class MilMo_PlayerStateEffectChangeAnimation : MilMo_PlayerStateEffect
{
	private readonly string _mNewAnimationName;

	private readonly string _mAnimationToChange;

	public MilMo_PlayerStateEffectChangeAnimation(PlayerStateEffectCosmetic effectData, MilMo_Avatar avatar)
		: base(avatar)
	{
		if (effectData.GetParameters().Count > 1)
		{
			_mAnimationToChange = effectData.GetParameters()[0];
			_mNewAnimationName = effectData.GetParameters()[1];
		}
		else
		{
			Debug.LogWarning("Missing parameter in \"ChangeAnimation\" player state effect");
		}
	}

	public override void Activate()
	{
		if (Avatar != null && _mAnimationToChange != null && _mNewAnimationName != null)
		{
			Avatar.StackAnimation(_mAnimationToChange, _mNewAnimationName);
		}
	}

	public override void Deactivate()
	{
		if (Avatar != null && _mAnimationToChange != null)
		{
			Avatar.UnstackAnimation(_mAnimationToChange, _mNewAnimationName);
		}
	}
}
