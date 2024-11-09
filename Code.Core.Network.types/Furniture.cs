using System.Collections.Generic;

namespace Code.Core.Network.types;

public class Furniture : HomeEquipment
{
	public new class Factory : Item.Factory
	{
		public override Item Create(MessageReader reader)
		{
			return new Furniture(reader);
		}
	}

	private readonly sbyte _isRoomEntrance;

	private readonly sbyte _doorIsDifferentOnOtherSide;

	private readonly string _leadsToRoomName;

	private const int TYPE_ID = 2;

	public override int GetTypeId()
	{
		return 2;
	}

	public Furniture(MessageReader reader)
		: base(reader)
	{
		_isRoomEntrance = reader.ReadInt8();
		_doorIsDifferentOnOtherSide = reader.ReadInt8();
		_leadsToRoomName = reader.ReadString();
	}

	public Furniture(sbyte isRoomEntrance, sbyte doorIsDifferentOnOtherSide, string leadsToRoomName, long id, string gridCell, float rotation, sbyte inStorage, long inRoom, string type, TemplateReference template, IList<string> modifiers)
		: base(id, gridCell, rotation, inStorage, inRoom, type, template, modifiers)
	{
		_isRoomEntrance = isRoomEntrance;
		_doorIsDifferentOnOtherSide = doorIsDifferentOnOtherSide;
		_leadsToRoomName = leadsToRoomName;
	}

	public sbyte GetIsRoomEntrance()
	{
		return _isRoomEntrance;
	}

	public sbyte GetDoorIsDifferentOnOtherSide()
	{
		return _doorIsDifferentOnOtherSide;
	}

	public string GetLeadsToRoomName()
	{
		return _leadsToRoomName;
	}

	public override int Size()
	{
		return 4 + base.Size() + MessageWriter.GetSize(_leadsToRoomName);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteInt8(_isRoomEntrance);
		writer.WriteInt8(_doorIsDifferentOnOtherSide);
		writer.WriteString(_leadsToRoomName);
	}
}
