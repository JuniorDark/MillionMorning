using System.Collections.Generic;

namespace Code.Core.Network.types;

public class BossModeTemplate : Template
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new BossModeTemplate(reader);
		}
	}

	private readonly TemplateReference _damageSusceptibility;

	private readonly float _speed;

	private readonly float _aggroSpeed;

	private readonly float _turnSpeed;

	private readonly string _loopingAnimation;

	private readonly string _healingEffect;

	private readonly IList<TemplateReference> _actionTemplates;

	private readonly IList<string> _damageEffects;

	private readonly IList<string> _noDamageEffects;

	private readonly IList<string> _aggroEffects;

	private readonly IList<string> _enterEffects;

	private const int TYPE_ID = 12;

	public override int GetTypeId()
	{
		return 12;
	}

	public BossModeTemplate(MessageReader reader)
		: base(reader)
	{
		_damageSusceptibility = new TemplateReference(reader);
		_speed = reader.ReadFloat();
		_aggroSpeed = reader.ReadFloat();
		_turnSpeed = reader.ReadFloat();
		_loopingAnimation = reader.ReadString();
		_healingEffect = reader.ReadString();
		_actionTemplates = new List<TemplateReference>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_actionTemplates.Add(new TemplateReference(reader));
		}
		_damageEffects = new List<string>();
		short num3 = reader.ReadInt16();
		for (short num4 = 0; num4 < num3; num4++)
		{
			_damageEffects.Add(reader.ReadString());
		}
		_noDamageEffects = new List<string>();
		short num5 = reader.ReadInt16();
		for (short num6 = 0; num6 < num5; num6++)
		{
			_noDamageEffects.Add(reader.ReadString());
		}
		_aggroEffects = new List<string>();
		short num7 = reader.ReadInt16();
		for (short num8 = 0; num8 < num7; num8++)
		{
			_aggroEffects.Add(reader.ReadString());
		}
		_enterEffects = new List<string>();
		short num9 = reader.ReadInt16();
		for (short num10 = 0; num10 < num9; num10++)
		{
			_enterEffects.Add(reader.ReadString());
		}
	}

	public BossModeTemplate(TemplateReference damageSusceptibility, float speed, float aggroSpeed, float turnSpeed, string loopingAnimation, string healingEffect, IList<TemplateReference> actionTemplates, IList<string> damageEffects, IList<string> noDamageEffects, IList<string> aggroEffects, IList<string> enterEffects, string type, TemplateReference reference)
		: base(type, reference)
	{
		_damageSusceptibility = damageSusceptibility;
		_speed = speed;
		_aggroSpeed = aggroSpeed;
		_turnSpeed = turnSpeed;
		_loopingAnimation = loopingAnimation;
		_healingEffect = healingEffect;
		_actionTemplates = actionTemplates;
		_damageEffects = damageEffects;
		_noDamageEffects = noDamageEffects;
		_aggroEffects = aggroEffects;
		_enterEffects = enterEffects;
	}

	public TemplateReference GetDamageSusceptibility()
	{
		return _damageSusceptibility;
	}

	public float GetSpeed()
	{
		return _speed;
	}

	public float GetAggroSpeed()
	{
		return _aggroSpeed;
	}

	public float GetTurnSpeed()
	{
		return _turnSpeed;
	}

	public string GetLoopingAnimation()
	{
		return _loopingAnimation;
	}

	public string GetHealingEffect()
	{
		return _healingEffect;
	}

	public IList<TemplateReference> GetActionTemplates()
	{
		return _actionTemplates;
	}

	public IList<string> GetDamageEffects()
	{
		return _damageEffects;
	}

	public IList<string> GetNoDamageEffects()
	{
		return _noDamageEffects;
	}

	public IList<string> GetAggroEffects()
	{
		return _aggroEffects;
	}

	public IList<string> GetEnterEffects()
	{
		return _enterEffects;
	}

	public override int Size()
	{
		int num = 26 + base.Size();
		num += _damageSusceptibility.Size();
		num += MessageWriter.GetSize(_loopingAnimation);
		num += MessageWriter.GetSize(_healingEffect);
		foreach (TemplateReference actionTemplate in _actionTemplates)
		{
			num += actionTemplate.Size();
		}
		num += (short)(2 * _damageEffects.Count);
		foreach (string damageEffect in _damageEffects)
		{
			num += MessageWriter.GetSize(damageEffect);
		}
		num += (short)(2 * _noDamageEffects.Count);
		foreach (string noDamageEffect in _noDamageEffects)
		{
			num += MessageWriter.GetSize(noDamageEffect);
		}
		num += (short)(2 * _aggroEffects.Count);
		foreach (string aggroEffect in _aggroEffects)
		{
			num += MessageWriter.GetSize(aggroEffect);
		}
		num += (short)(2 * _enterEffects.Count);
		foreach (string enterEffect in _enterEffects)
		{
			num += MessageWriter.GetSize(enterEffect);
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		_damageSusceptibility.Write(writer);
		writer.WriteFloat(_speed);
		writer.WriteFloat(_aggroSpeed);
		writer.WriteFloat(_turnSpeed);
		writer.WriteString(_loopingAnimation);
		writer.WriteString(_healingEffect);
		writer.WriteInt16((short)_actionTemplates.Count);
		foreach (TemplateReference actionTemplate in _actionTemplates)
		{
			actionTemplate.Write(writer);
		}
		writer.WriteInt16((short)_damageEffects.Count);
		foreach (string damageEffect in _damageEffects)
		{
			writer.WriteString(damageEffect);
		}
		writer.WriteInt16((short)_noDamageEffects.Count);
		foreach (string noDamageEffect in _noDamageEffects)
		{
			writer.WriteString(noDamageEffect);
		}
		writer.WriteInt16((short)_aggroEffects.Count);
		foreach (string aggroEffect in _aggroEffects)
		{
			writer.WriteString(aggroEffect);
		}
		writer.WriteInt16((short)_enterEffects.Count);
		foreach (string enterEffect in _enterEffects)
		{
			writer.WriteString(enterEffect);
		}
	}
}
