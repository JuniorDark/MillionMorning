using System.Collections.Generic;

namespace Code.Core.Network.types;

public class TemplateCreature : MovableObjectTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new TemplateCreature(reader);
		}
	}

	private readonly string _mover;

	private readonly float _velocity;

	private readonly string _impactMover;

	private readonly float _pull;

	private readonly float _drag;

	private readonly TemplateReference _damageSusceptibility;

	private readonly string _damageSound;

	private readonly string _noDamageSound;

	private readonly IList<TemplateReference> _attacks;

	private readonly string _aggroSound;

	private readonly float _aggroVelocity;

	private readonly float _turnSpeed;

	private static readonly int TypeId = 74;

	public override int GetTypeId()
	{
		return TypeId;
	}

	public TemplateCreature(MessageReader reader)
		: base(reader)
	{
		_mover = reader.ReadString();
		_velocity = reader.ReadFloat();
		_impactMover = reader.ReadString();
		_pull = reader.ReadFloat();
		_drag = reader.ReadFloat();
		_damageSusceptibility = new TemplateReference(reader);
		_damageSound = reader.ReadString();
		_noDamageSound = reader.ReadString();
		_attacks = new List<TemplateReference>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_attacks.Add(new TemplateReference(reader));
		}
		_aggroSound = reader.ReadString();
		_aggroVelocity = reader.ReadFloat();
		_turnSpeed = reader.ReadFloat();
	}

	public TemplateCreature(string mover, float velocity, string impactMover, float pull, float drag, TemplateReference damageSusceptibility, string damageSound, string noDamageSound, IList<TemplateReference> attacks, string aggroSound, float aggroVelocity, float turnSpeed, string visualRep, float maxHealth, float collisionRadius, float impactHeight, float impactRadius, sbyte immobile, IList<string> deathEffectsPhase1, IList<string> deathEffectsPhase2, int level, string displayName, float markerYOffset, string type, TemplateReference reference)
		: base(visualRep, maxHealth, collisionRadius, impactHeight, impactRadius, immobile, deathEffectsPhase1, deathEffectsPhase2, level, displayName, markerYOffset, type, reference)
	{
		_mover = mover;
		_velocity = velocity;
		_impactMover = impactMover;
		_pull = pull;
		_drag = drag;
		_damageSusceptibility = damageSusceptibility;
		_damageSound = damageSound;
		_noDamageSound = noDamageSound;
		_attacks = attacks;
		_aggroSound = aggroSound;
		_aggroVelocity = aggroVelocity;
		_turnSpeed = turnSpeed;
	}

	public string GetMover()
	{
		return _mover;
	}

	public float GetVelocity()
	{
		return _velocity;
	}

	public string GetImpactMover()
	{
		return _impactMover;
	}

	public float GetPull()
	{
		return _pull;
	}

	public float GetDrag()
	{
		return _drag;
	}

	public TemplateReference GetDamageSusceptibility()
	{
		return _damageSusceptibility;
	}

	public string GetDamageSound()
	{
		return _damageSound;
	}

	public string GetNoDamageSound()
	{
		return _noDamageSound;
	}

	public IList<TemplateReference> GetAttacks()
	{
		return _attacks;
	}

	public string GetAggroSound()
	{
		return _aggroSound;
	}

	public float GetAggroVelocity()
	{
		return _aggroVelocity;
	}

	public float GetTurnSpeed()
	{
		return _turnSpeed;
	}

	public override int Size()
	{
		int num = 32 + base.Size();
		num += MessageWriter.GetSize(_mover);
		num += MessageWriter.GetSize(_impactMover);
		num += _damageSusceptibility.Size();
		num += MessageWriter.GetSize(_damageSound);
		num += MessageWriter.GetSize(_noDamageSound);
		foreach (TemplateReference attack in _attacks)
		{
			num += attack.Size();
		}
		return num + MessageWriter.GetSize(_aggroSound);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_mover);
		writer.WriteFloat(_velocity);
		writer.WriteString(_impactMover);
		writer.WriteFloat(_pull);
		writer.WriteFloat(_drag);
		_damageSusceptibility.Write(writer);
		writer.WriteString(_damageSound);
		writer.WriteString(_noDamageSound);
		writer.WriteInt16((short)_attacks.Count);
		foreach (TemplateReference attack in _attacks)
		{
			attack.Write(writer);
		}
		writer.WriteString(_aggroSound);
		writer.WriteFloat(_aggroVelocity);
		writer.WriteFloat(_turnSpeed);
	}
}
