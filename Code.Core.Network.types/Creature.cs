using System.Collections.Generic;

namespace Code.Core.Network.types;

public class Creature : LevelObject
{
	public new class Factory : LevelObject.Factory
	{
		public override LevelObject Create(MessageReader reader)
		{
			return new Creature(reader);
		}
	}

	private readonly TemplateReference _template;

	private const int TYPE_ID = 2;

	public override int GetTypeId()
	{
		return 2;
	}

	public Creature(MessageReader reader)
		: base(reader)
	{
		_template = new TemplateReference(reader);
	}

	public Creature(TemplateReference template, string type, int id, string fullLevelName, string visualRep, vector3 position, vector3 rotation, IList<string> spawnTypes, IList<string> removalEffects, float lifespan)
		: base(type, id, fullLevelName, visualRep, position, rotation, spawnTypes, removalEffects, lifespan)
	{
		_template = template;
	}

	public TemplateReference GetTemplate()
	{
		return _template;
	}

	public override int Size()
	{
		return base.Size() + _template.Size();
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		_template.Write(writer);
	}
}
