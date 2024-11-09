namespace Code.Core.Network.types;

public class IntraTeleport : Teleport
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new IntraTeleport(reader);
		}
	}

	private readonly string _room;

	private readonly sbyte _noPlop;

	private const int TYPE_ID = 81;

	public override int GetTypeId()
	{
		return 81;
	}

	public IntraTeleport(MessageReader reader)
		: base(reader)
	{
		_room = reader.ReadString();
		_noPlop = reader.ReadInt8();
	}

	public IntraTeleport(string room, sbyte noPlop, string arriveSound, string type, TemplateReference reference)
		: base(arriveSound, type, reference)
	{
		_room = room;
		_noPlop = noPlop;
	}

	public string GetRoom()
	{
		return _room;
	}

	public sbyte GetNoPlop()
	{
		return _noPlop;
	}

	public override int Size()
	{
		return 3 + base.Size() + MessageWriter.GetSize(_room);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_room);
		writer.WriteInt8(_noPlop);
	}
}
