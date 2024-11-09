using System.Collections.Generic;

namespace Code.Core.Network.types;

public class MeleeCreatureAttackTemplate : TemplateCreatureAttack
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new MeleeCreatureAttackTemplate(reader);
		}
	}

	private readonly string _impactSound;

	private readonly float _impactDelay;

	private readonly float _playerImpulseY;

	private readonly float _playerImpulseXz;

	private readonly IList<Damage> _damage;

	private const int TYPE_ID = 76;

	public override int GetTypeId()
	{
		return 76;
	}

	public MeleeCreatureAttackTemplate(MessageReader reader)
		: base(reader)
	{
		_impactSound = reader.ReadString();
		_impactDelay = reader.ReadFloat();
		_playerImpulseY = reader.ReadFloat();
		_playerImpulseXz = reader.ReadFloat();
		_damage = new List<Damage>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_damage.Add(new Damage(reader));
		}
	}

	public MeleeCreatureAttackTemplate(string impactSound, float impactDelay, float playerImpulseY, float playerImpulseXz, IList<Damage> damage, vector3 offset, IList<string> attackEffects, IList<string> preparationEffects, string type, TemplateReference reference)
		: base(offset, attackEffects, preparationEffects, type, reference)
	{
		_impactSound = impactSound;
		_impactDelay = impactDelay;
		_playerImpulseY = playerImpulseY;
		_playerImpulseXz = playerImpulseXz;
		_damage = damage;
	}

	public string GetImpactSound()
	{
		return _impactSound;
	}

	public float GetImpactDelay()
	{
		return _impactDelay;
	}

	public float GetPlayerImpulseY()
	{
		return _playerImpulseY;
	}

	public float GetPlayerImpulseXz()
	{
		return _playerImpulseXz;
	}

	public IList<Damage> GetDamage()
	{
		return _damage;
	}

	public override int Size()
	{
		int num = 16 + base.Size();
		num += MessageWriter.GetSize(_impactSound);
		foreach (Damage item in _damage)
		{
			num += item.Size();
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_impactSound);
		writer.WriteFloat(_impactDelay);
		writer.WriteFloat(_playerImpulseY);
		writer.WriteFloat(_playerImpulseXz);
		writer.WriteInt16((short)_damage.Count);
		foreach (Damage item in _damage)
		{
			item.Write(writer);
		}
	}
}
