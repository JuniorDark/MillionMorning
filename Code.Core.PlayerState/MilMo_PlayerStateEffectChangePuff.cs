using Code.Core.Avatar;
using Code.Core.Network.types;
using Code.Core.Visual.Effect;
using UnityEngine;

namespace Code.Core.PlayerState;

public class MilMo_PlayerStateEffectChangePuff : MilMo_PlayerStateEffect
{
	private readonly string _newPuffName;

	private readonly string _puffAction;

	private readonly string _puffSurface;

	private MilMo_Effect _oldPuff;

	public MilMo_PlayerStateEffectChangePuff(PlayerStateEffectCosmetic effectData, MilMo_Avatar avatar)
		: base(avatar)
	{
		if (effectData.GetParameters().Count > 2)
		{
			_puffSurface = effectData.GetParameters()[0];
			_puffAction = effectData.GetParameters()[1];
			_newPuffName = effectData.GetParameters()[2];
		}
		else
		{
			Debug.LogWarning("Missing parameter in \"ChangePuff\" player state effect");
		}
	}

	public override void Activate()
	{
		if (Avatar != null && _puffAction != null && _puffSurface != null && _newPuffName != null)
		{
			MilMo_Effect effect = MilMo_EffectContainer.GetEffect(_newPuffName, Avatar.GameObject);
			_oldPuff = Avatar.GetPuff(_puffAction, _puffSurface);
			Avatar.ChangePuff(_puffAction, _puffSurface, effect);
		}
	}

	public override void Deactivate()
	{
		if (Avatar != null && _puffAction != null && _puffSurface != null)
		{
			Avatar.ChangePuff(_puffAction, _puffSurface, _oldPuff);
		}
	}
}
