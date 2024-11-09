using System;
using System.Collections.Generic;
using Code.Core.Network.types;
using Code.Core.Template;

namespace Code.Core.PlayerState;

public class MilMo_PlayerStateTemplate : MilMo_Template
{
	public enum MilMo_BuffType
	{
		Buff,
		Debuff,
		None
	}

	private MilMo_BuffType _mBuffType;

	private string _mVariable;

	private List<PlayerStateEffectFunction> _mOnActivationFunctional;

	private List<PlayerStateEffectCosmetic> _mOnActivationCosmetic;

	private List<PlayerStateEffectFunction> _mOnDeactivationFunctional;

	private List<PlayerStateEffectCosmetic> _mOnDeactivationCosmetic;

	public MilMo_BuffType BuffType => _mBuffType;

	public string Variable => _mVariable;

	public List<PlayerStateEffectFunction> OnActivationFunctional => _mOnActivationFunctional;

	public List<PlayerStateEffectCosmetic> OnActivationCosmetic => _mOnActivationCosmetic;

	public List<PlayerStateEffectFunction> OnDeactivationFunctional => _mOnDeactivationFunctional;

	public List<PlayerStateEffectCosmetic> OnDeactivationCosmetic => _mOnDeactivationCosmetic;

	protected MilMo_PlayerStateTemplate(string category, string path, string filePath)
		: base(category, path, filePath, "PlayerState")
	{
	}

	public override bool LoadFromNetwork(Code.Core.Network.types.Template t)
	{
		if (!(t is PlayerStateTemplate playerStateTemplate))
		{
			return true;
		}
		_mVariable = playerStateTemplate.GetVariable();
		_mOnActivationCosmetic = (List<PlayerStateEffectCosmetic>)playerStateTemplate.GetOnActivationCosmetic();
		_mOnDeactivationCosmetic = (List<PlayerStateEffectCosmetic>)playerStateTemplate.GetOnDeactivationCosmetic();
		_mOnActivationFunctional = (List<PlayerStateEffectFunction>)playerStateTemplate.GetOnActivationFunctional();
		_mOnDeactivationFunctional = (List<PlayerStateEffectFunction>)playerStateTemplate.GetOnDeactivationFunctional();
		if (playerStateTemplate.GetBuffType().Equals("Buff", StringComparison.InvariantCultureIgnoreCase))
		{
			_mBuffType = MilMo_BuffType.Buff;
		}
		else if (playerStateTemplate.GetBuffType().Equals("Debuff", StringComparison.InvariantCultureIgnoreCase))
		{
			_mBuffType = MilMo_BuffType.Debuff;
		}
		else
		{
			_mBuffType = MilMo_BuffType.None;
		}
		return true;
	}

	public static MilMo_PlayerStateTemplate Create(string category, string path, string filePath)
	{
		return new MilMo_PlayerStateTemplate(category, path, filePath);
	}
}
