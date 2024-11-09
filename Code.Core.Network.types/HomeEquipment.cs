using System.Collections.Generic;

namespace Code.Core.Network.types;

public class HomeEquipment : Item
{
	public new class Factory : Item.Factory
	{
		public override Item Create(MessageReader reader)
		{
			return new HomeEquipment(reader);
		}
	}

	private readonly long _id;

	private readonly string _gridCell;

	private readonly float _rotation;

	private readonly sbyte _inStorage;

	private readonly long _inRoom;

	private readonly string _type;

	private const int TYPE_ID = 1;

	public override int GetTypeId()
	{
		return 1;
	}

	public HomeEquipment(MessageReader reader)
		: base(reader)
	{
		_id = reader.ReadInt64();
		_gridCell = reader.ReadString();
		_rotation = reader.ReadFloat();
		_inStorage = reader.ReadInt8();
		_inRoom = reader.ReadInt64();
		_type = reader.ReadString();
	}

	public HomeEquipment(long id, string gridCell, float rotation, sbyte inStorage, long inRoom, string type, TemplateReference template, IList<string> modifiers)
		: base(template, modifiers)
	{
		_id = id;
		_gridCell = gridCell;
		_rotation = rotation;
		_inStorage = inStorage;
		_inRoom = inRoom;
		_type = type;
	}

	public long GetId()
	{
		return _id;
	}

	public string GetGridCell()
	{
		return _gridCell;
	}

	public float GetRotation()
	{
		return _rotation;
	}

	public sbyte GetInStorage()
	{
		return _inStorage;
	}

	public long GetInRoom()
	{
		return _inRoom;
	}

	public string GetTemplateType()
	{
		return _type;
	}

	public override int Size()
	{
		return 25 + base.Size() + MessageWriter.GetSize(_gridCell) + MessageWriter.GetSize(_type);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteInt64(_id);
		writer.WriteString(_gridCell);
		writer.WriteFloat(_rotation);
		writer.WriteInt8(_inStorage);
		writer.WriteInt64(_inRoom);
		writer.WriteString(_type);
	}
}
