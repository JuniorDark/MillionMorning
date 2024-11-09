using System.Collections.Generic;
using Code.Core.Avatar;
using Code.Core.Network.types;

namespace Code.Core.PlayerState;

public sealed class MilMo_PlayerState
{
	private readonly MilMo_PlayerStateTemplate _template;

	private readonly MilMo_Avatar _avatar;

	private readonly List<MilMo_PlayerStateEffect> _onActivation = new List<MilMo_PlayerStateEffect>();

	private readonly List<MilMo_PlayerStateEffect> _onDeactivation = new List<MilMo_PlayerStateEffect>();

	public MilMo_PlayerStateTemplate Template => _template;

	public MilMo_PlayerState(MilMo_Avatar avatar, MilMo_PlayerStateTemplate template)
	{
		_avatar = avatar;
		_template = template;
		foreach (PlayerStateEffectCosmetic item in _template.OnActivationCosmetic)
		{
			MilMo_PlayerStateEffect milMo_PlayerStateEffect = MilMo_PlayerStateEffect.Create(item, avatar);
			if (milMo_PlayerStateEffect != null)
			{
				_onActivation.Add(milMo_PlayerStateEffect);
			}
		}
		foreach (PlayerStateEffectCosmetic item2 in _template.OnDeactivationCosmetic)
		{
			MilMo_PlayerStateEffect milMo_PlayerStateEffect2 = MilMo_PlayerStateEffect.Create(item2, avatar);
			if (milMo_PlayerStateEffect2 != null)
			{
				_onDeactivation.Add(milMo_PlayerStateEffect2);
			}
		}
	}

	public void Activate()
	{
		foreach (MilMo_PlayerStateEffect item in _onActivation)
		{
			item.Activate();
		}
		_avatar.AddActiveState(this);
	}

	public void Update()
	{
		foreach (MilMo_PlayerStateEffect item in _onActivation)
		{
			item.Update();
		}
	}

	public void Deactivate()
	{
		foreach (MilMo_PlayerStateEffect item in _onActivation)
		{
			item.Deactivate();
		}
		foreach (MilMo_PlayerStateEffect item2 in _onDeactivation)
		{
			item2.Activate();
		}
		_avatar.RemoveActiveState(this);
	}
}
