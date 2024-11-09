using System.Collections.Generic;

namespace Code.Core.Network.types;

public class RoomPresetTemplate : Template
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new RoomPresetTemplate(reader);
		}
	}

	private readonly TemplateReference _roomTemplate;

	private readonly HomeEquipment _door;

	private readonly IList<HomeEquipment> _equipment;

	private const int TYPE_ID = 15;

	public override int GetTypeId()
	{
		return 15;
	}

	public RoomPresetTemplate(MessageReader reader)
		: base(reader)
	{
		_roomTemplate = new TemplateReference(reader);
		_door = (HomeEquipment)Item.Create(reader.ReadTypeCode(), reader);
		_equipment = new List<HomeEquipment>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_equipment.Add((HomeEquipment)Item.Create(reader.ReadTypeCode(), reader));
		}
	}

	public RoomPresetTemplate(TemplateReference roomTemplate, HomeEquipment door, IList<HomeEquipment> equipment, string type, TemplateReference reference)
		: base(type, reference)
	{
		_roomTemplate = roomTemplate;
		_door = door;
		_equipment = equipment;
	}

	public TemplateReference GetRoomTemplate()
	{
		return _roomTemplate;
	}

	public HomeEquipment GetDoor()
	{
		return _door;
	}

	public IList<HomeEquipment> GetEquipment()
	{
		return _equipment;
	}

	public override int Size()
	{
		int num = 4 + base.Size();
		num += _roomTemplate.Size();
		num += _door.Size();
		num += (short)(_equipment.Count * 2);
		foreach (HomeEquipment item in _equipment)
		{
			num += item.Size();
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		_roomTemplate.Write(writer);
		writer.WriteTypeCode(_door.GetTypeId());
		_door.Write(writer);
		writer.WriteInt16((short)_equipment.Count);
		foreach (HomeEquipment item in _equipment)
		{
			writer.WriteTypeCode(item.GetTypeId());
			item.Write(writer);
		}
	}
}
