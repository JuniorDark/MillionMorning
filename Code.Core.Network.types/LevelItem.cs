using System.Collections.Generic;

namespace Code.Core.Network.types;

public class LevelItem : LevelObject
{
	public new class Factory : LevelObject.Factory
	{
		public override LevelObject Create(MessageReader reader)
		{
			return new LevelItem(reader);
		}
	}

	private readonly TemplateReference _item;

	private readonly string _ownerId;

	private const int TYPE_ID = 3;

	public override int GetTypeId()
	{
		return 3;
	}

	public LevelItem(MessageReader reader)
		: base(reader)
	{
		_item = new TemplateReference(reader);
		_ownerId = reader.ReadString();
	}

	public LevelItem(TemplateReference item, string ownerId, string type, int id, string fullLevelName, string visualRep, vector3 position, vector3 rotation, IList<string> spawnTypes, IList<string> removalEffects, float lifespan)
		: base(type, id, fullLevelName, visualRep, position, rotation, spawnTypes, removalEffects, lifespan)
	{
		_item = item;
		_ownerId = ownerId;
	}

	public TemplateReference GetItem()
	{
		return _item;
	}

	public string GetOwnerId()
	{
		return _ownerId;
	}

	public override int Size()
	{
		return 2 + base.Size() + _item.Size() + MessageWriter.GetSize(_ownerId);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		_item.Write(writer);
		writer.WriteString(_ownerId);
	}
}
