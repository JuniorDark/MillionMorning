using System.Collections.Generic;

namespace Code.Core.Network.types;

public class PlayerStateTemplate : Template
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new PlayerStateTemplate(reader);
		}
	}

	private readonly string _buffType;

	private readonly string _variable;

	private readonly IList<PlayerStateEffectFunction> _onActivationFunctional;

	private readonly IList<PlayerStateEffectFunction> _onDeactivationFunctional;

	private readonly IList<PlayerStateEffectCosmetic> _onActivationCosmetic;

	private readonly IList<PlayerStateEffectCosmetic> _onDeactivationCosmetic;

	private const int TYPE_ID = 2;

	public override int GetTypeId()
	{
		return 2;
	}

	public PlayerStateTemplate(MessageReader reader)
		: base(reader)
	{
		_buffType = reader.ReadString();
		_variable = reader.ReadString();
		_onActivationFunctional = new List<PlayerStateEffectFunction>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_onActivationFunctional.Add(new PlayerStateEffectFunction(reader));
		}
		_onDeactivationFunctional = new List<PlayerStateEffectFunction>();
		short num3 = reader.ReadInt16();
		for (short num4 = 0; num4 < num3; num4++)
		{
			_onDeactivationFunctional.Add(new PlayerStateEffectFunction(reader));
		}
		_onActivationCosmetic = new List<PlayerStateEffectCosmetic>();
		short num5 = reader.ReadInt16();
		for (short num6 = 0; num6 < num5; num6++)
		{
			_onActivationCosmetic.Add(new PlayerStateEffectCosmetic(reader));
		}
		_onDeactivationCosmetic = new List<PlayerStateEffectCosmetic>();
		short num7 = reader.ReadInt16();
		for (short num8 = 0; num8 < num7; num8++)
		{
			_onDeactivationCosmetic.Add(new PlayerStateEffectCosmetic(reader));
		}
	}

	public PlayerStateTemplate(string buffType, string variable, IList<PlayerStateEffectFunction> onActivationFunctional, IList<PlayerStateEffectFunction> onDeactivationFunctional, IList<PlayerStateEffectCosmetic> onActivationCosmetic, IList<PlayerStateEffectCosmetic> onDeactivationCosmetic, string type, TemplateReference reference)
		: base(type, reference)
	{
		_buffType = buffType;
		_variable = variable;
		_onActivationFunctional = onActivationFunctional;
		_onDeactivationFunctional = onDeactivationFunctional;
		_onActivationCosmetic = onActivationCosmetic;
		_onDeactivationCosmetic = onDeactivationCosmetic;
	}

	public string GetBuffType()
	{
		return _buffType;
	}

	public string GetVariable()
	{
		return _variable;
	}

	public IList<PlayerStateEffectFunction> GetOnActivationFunctional()
	{
		return _onActivationFunctional;
	}

	public IList<PlayerStateEffectFunction> GetOnDeactivationFunctional()
	{
		return _onDeactivationFunctional;
	}

	public IList<PlayerStateEffectCosmetic> GetOnActivationCosmetic()
	{
		return _onActivationCosmetic;
	}

	public IList<PlayerStateEffectCosmetic> GetOnDeactivationCosmetic()
	{
		return _onDeactivationCosmetic;
	}

	public override int Size()
	{
		int num = 12 + base.Size();
		num += MessageWriter.GetSize(_buffType);
		num += MessageWriter.GetSize(_variable);
		foreach (PlayerStateEffectFunction item in _onActivationFunctional)
		{
			num += item.Size();
		}
		foreach (PlayerStateEffectFunction item2 in _onDeactivationFunctional)
		{
			num += item2.Size();
		}
		foreach (PlayerStateEffectCosmetic item3 in _onActivationCosmetic)
		{
			num += item3.Size();
		}
		foreach (PlayerStateEffectCosmetic item4 in _onDeactivationCosmetic)
		{
			num += item4.Size();
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_buffType);
		writer.WriteString(_variable);
		writer.WriteInt16((short)_onActivationFunctional.Count);
		foreach (PlayerStateEffectFunction item in _onActivationFunctional)
		{
			item.Write(writer);
		}
		writer.WriteInt16((short)_onDeactivationFunctional.Count);
		foreach (PlayerStateEffectFunction item2 in _onDeactivationFunctional)
		{
			item2.Write(writer);
		}
		writer.WriteInt16((short)_onActivationCosmetic.Count);
		foreach (PlayerStateEffectCosmetic item3 in _onActivationCosmetic)
		{
			item3.Write(writer);
		}
		writer.WriteInt16((short)_onDeactivationCosmetic.Count);
		foreach (PlayerStateEffectCosmetic item4 in _onDeactivationCosmetic)
		{
			item4.Write(writer);
		}
	}
}
