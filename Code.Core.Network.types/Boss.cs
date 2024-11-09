using System.Collections.Generic;

namespace Code.Core.Network.types;

public class Boss : LevelObject
{
	public new class Factory : LevelObject.Factory
	{
		public override LevelObject Create(MessageReader reader)
		{
			return new Boss(reader);
		}
	}

	private readonly float _health;

	private readonly TemplateReference _template;

	private readonly TemplateReference _currentMode;

	private const int TYPE_ID = 5;

	public override int GetTypeId()
	{
		return 5;
	}

	public Boss(MessageReader reader)
		: base(reader)
	{
		_health = reader.ReadFloat();
		_template = new TemplateReference(reader);
		_currentMode = new TemplateReference(reader);
	}

	public Boss(float health, TemplateReference template, TemplateReference currentMode, string type, int id, string fullLevelName, string visualRep, vector3 position, vector3 rotation, IList<string> spawnTypes, IList<string> removalEffects, float lifespan)
		: base(type, id, fullLevelName, visualRep, position, rotation, spawnTypes, removalEffects, lifespan)
	{
		_health = health;
		_template = template;
		_currentMode = currentMode;
	}

	public float GetHealth()
	{
		return _health;
	}

	public TemplateReference GetTemplate()
	{
		return _template;
	}

	public TemplateReference GetCurrentMode()
	{
		return _currentMode;
	}

	public override int Size()
	{
		return 4 + base.Size() + _template.Size() + _currentMode.Size();
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteFloat(_health);
		_template.Write(writer);
		_currentMode.Write(writer);
	}
}
