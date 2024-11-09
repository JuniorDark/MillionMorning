using System.Collections.Generic;

namespace Code.Core.Network.types;

public class MovableObjectTemplate : Template
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new MovableObjectTemplate(reader);
		}
	}

	private readonly string _visualRep;

	private readonly float _maxHealth;

	private readonly float _collisionRadius;

	private readonly float _impactHeight;

	private readonly float _impactRadius;

	private readonly sbyte _immobile;

	private readonly IList<string> _deathEffectsPhase1;

	private readonly IList<string> _deathEffectsPhase2;

	private readonly int _level;

	private readonly string _displayName;

	private readonly float _markerYOffset;

	private const int TYPE_ID = 4;

	public override int GetTypeId()
	{
		return 4;
	}

	public MovableObjectTemplate(MessageReader reader)
		: base(reader)
	{
		_visualRep = reader.ReadString();
		_maxHealth = reader.ReadFloat();
		_collisionRadius = reader.ReadFloat();
		_impactHeight = reader.ReadFloat();
		_impactRadius = reader.ReadFloat();
		_immobile = reader.ReadInt8();
		_deathEffectsPhase1 = new List<string>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_deathEffectsPhase1.Add(reader.ReadString());
		}
		_deathEffectsPhase2 = new List<string>();
		short num3 = reader.ReadInt16();
		for (short num4 = 0; num4 < num3; num4++)
		{
			_deathEffectsPhase2.Add(reader.ReadString());
		}
		_level = reader.ReadInt32();
		_displayName = reader.ReadString();
		_markerYOffset = reader.ReadFloat();
	}

	public MovableObjectTemplate(string visualRep, float maxHealth, float collisionRadius, float impactHeight, float impactRadius, sbyte immobile, IList<string> deathEffectsPhase1, IList<string> deathEffectsPhase2, int level, string displayName, float markerYOffset, string type, TemplateReference reference)
		: base(type, reference)
	{
		_visualRep = visualRep;
		_maxHealth = maxHealth;
		_collisionRadius = collisionRadius;
		_impactHeight = impactHeight;
		_impactRadius = impactRadius;
		_immobile = immobile;
		_deathEffectsPhase1 = deathEffectsPhase1;
		_deathEffectsPhase2 = deathEffectsPhase2;
		_level = level;
		_displayName = displayName;
		_markerYOffset = markerYOffset;
	}

	public string GetVisualRep()
	{
		return _visualRep;
	}

	public float GetMaxHealth()
	{
		return _maxHealth;
	}

	public float GetCollisionRadius()
	{
		return _collisionRadius;
	}

	public float GetImpactHeight()
	{
		return _impactHeight;
	}

	public float GetImpactRadius()
	{
		return _impactRadius;
	}

	public sbyte GetImmobile()
	{
		return _immobile;
	}

	public IList<string> GetDeathEffectsPhase1()
	{
		return _deathEffectsPhase1;
	}

	public IList<string> GetDeathEffectsPhase2()
	{
		return _deathEffectsPhase2;
	}

	public int GetLevel()
	{
		return _level;
	}

	public string GetDisplayName()
	{
		return _displayName;
	}

	public float GetMarkerYOffset()
	{
		return _markerYOffset;
	}

	public override int Size()
	{
		int num = 33 + base.Size();
		num += MessageWriter.GetSize(_visualRep);
		num += (short)(2 * _deathEffectsPhase1.Count);
		foreach (string item in _deathEffectsPhase1)
		{
			num += MessageWriter.GetSize(item);
		}
		num += (short)(2 * _deathEffectsPhase2.Count);
		foreach (string item2 in _deathEffectsPhase2)
		{
			num += MessageWriter.GetSize(item2);
		}
		return num + MessageWriter.GetSize(_displayName);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_visualRep);
		writer.WriteFloat(_maxHealth);
		writer.WriteFloat(_collisionRadius);
		writer.WriteFloat(_impactHeight);
		writer.WriteFloat(_impactRadius);
		writer.WriteInt8(_immobile);
		writer.WriteInt16((short)_deathEffectsPhase1.Count);
		foreach (string item in _deathEffectsPhase1)
		{
			writer.WriteString(item);
		}
		writer.WriteInt16((short)_deathEffectsPhase2.Count);
		foreach (string item2 in _deathEffectsPhase2)
		{
			writer.WriteString(item2);
		}
		writer.WriteInt32(_level);
		writer.WriteString(_displayName);
		writer.WriteFloat(_markerYOffset);
	}
}
