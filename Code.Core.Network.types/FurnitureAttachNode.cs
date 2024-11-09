namespace Code.Core.Network.types;

public class FurnitureAttachNode
{
	private readonly short _id;

	private readonly vector3 _position;

	private readonly vector3 _rotation;

	public FurnitureAttachNode(MessageReader reader)
	{
		_id = reader.ReadInt16();
		_position = new vector3(reader);
		_rotation = new vector3(reader);
	}

	public FurnitureAttachNode(short id, vector3 position, vector3 rotation)
	{
		_id = id;
		_position = position;
		_rotation = rotation;
	}

	public short GetId()
	{
		return _id;
	}

	public vector3 GetPosition()
	{
		return _position;
	}

	public vector3 GetRotation()
	{
		return _rotation;
	}

	public int Size()
	{
		return 26;
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteInt16(_id);
		_position.Write(writer);
		_rotation.Write(writer);
	}
}
