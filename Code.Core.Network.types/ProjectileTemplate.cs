using System.Collections.Generic;

namespace Code.Core.Network.types;

public class ProjectileTemplate : Template
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new ProjectileTemplate(reader);
		}
	}

	private readonly string _impactSound;

	private readonly string _impactParticle;

	private readonly string _trail;

	private readonly sbyte _hasGravity;

	private readonly float _damageArea;

	private readonly float _speed;

	private readonly float _range;

	private readonly sbyte _hitScan;

	private readonly IList<Damage> _damage;

	private readonly IList<string> _spawnEffects;

	private readonly string _visualRep;

	private const int TYPE_ID = 3;

	public override int GetTypeId()
	{
		return 3;
	}

	public ProjectileTemplate(MessageReader reader)
		: base(reader)
	{
		_impactSound = reader.ReadString();
		_impactParticle = reader.ReadString();
		_trail = reader.ReadString();
		_hasGravity = reader.ReadInt8();
		_damageArea = reader.ReadFloat();
		_speed = reader.ReadFloat();
		_range = reader.ReadFloat();
		_hitScan = reader.ReadInt8();
		_damage = new List<Damage>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_damage.Add(new Damage(reader));
		}
		_spawnEffects = new List<string>();
		short num3 = reader.ReadInt16();
		for (short num4 = 0; num4 < num3; num4++)
		{
			_spawnEffects.Add(reader.ReadString());
		}
		_visualRep = reader.ReadString();
	}

	public ProjectileTemplate(string impactSound, string impactParticle, string trail, sbyte hasGravity, float damageArea, float speed, float range, sbyte hitScan, IList<Damage> damage, IList<string> spawnEffects, string visualRep, string type, TemplateReference reference)
		: base(type, reference)
	{
		_impactSound = impactSound;
		_impactParticle = impactParticle;
		_trail = trail;
		_hasGravity = hasGravity;
		_damageArea = damageArea;
		_speed = speed;
		_range = range;
		_hitScan = hitScan;
		_damage = damage;
		_spawnEffects = spawnEffects;
		_visualRep = visualRep;
	}

	public string GetImpactSound()
	{
		return _impactSound;
	}

	public string GetImpactParticle()
	{
		return _impactParticle;
	}

	public string GetTrail()
	{
		return _trail;
	}

	public sbyte GetHasGravity()
	{
		return _hasGravity;
	}

	public float GetDamageArea()
	{
		return _damageArea;
	}

	public float GetSpeed()
	{
		return _speed;
	}

	public float GetRange()
	{
		return _range;
	}

	public sbyte GetHitScan()
	{
		return _hitScan;
	}

	public IList<Damage> GetDamage()
	{
		return _damage;
	}

	public IList<string> GetSpawnEffects()
	{
		return _spawnEffects;
	}

	public string GetVisualRep()
	{
		return _visualRep;
	}

	public override int Size()
	{
		int num = 26 + base.Size();
		num += MessageWriter.GetSize(_impactSound);
		num += MessageWriter.GetSize(_impactParticle);
		num += MessageWriter.GetSize(_trail);
		foreach (Damage item in _damage)
		{
			num += item.Size();
		}
		num += (short)(2 * _spawnEffects.Count);
		foreach (string spawnEffect in _spawnEffects)
		{
			num += MessageWriter.GetSize(spawnEffect);
		}
		return num + MessageWriter.GetSize(_visualRep);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_impactSound);
		writer.WriteString(_impactParticle);
		writer.WriteString(_trail);
		writer.WriteInt8(_hasGravity);
		writer.WriteFloat(_damageArea);
		writer.WriteFloat(_speed);
		writer.WriteFloat(_range);
		writer.WriteInt8(_hitScan);
		writer.WriteInt16((short)_damage.Count);
		foreach (Damage item in _damage)
		{
			item.Write(writer);
		}
		writer.WriteInt16((short)_spawnEffects.Count);
		foreach (string spawnEffect in _spawnEffects)
		{
			writer.WriteString(spawnEffect);
		}
		writer.WriteString(_visualRep);
	}
}
