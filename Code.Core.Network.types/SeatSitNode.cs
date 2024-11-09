namespace Code.Core.Network.types;

public class SeatSitNode
{
	private readonly short _id;

	private readonly vector3 _position;

	private readonly vector3 _rotation;

	private readonly string _pose;

	public SeatSitNode(MessageReader reader)
	{
		_id = reader.ReadInt16();
		_position = new vector3(reader);
		_rotation = new vector3(reader);
		_pose = reader.ReadString();
	}

	public SeatSitNode(short id, vector3 position, vector3 rotation, string pose)
	{
		_id = id;
		_position = position;
		_rotation = rotation;
		_pose = pose;
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

	public string GetPose()
	{
		return _pose;
	}

	public int Size()
	{
		return 28 + MessageWriter.GetSize(_pose);
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteInt16(_id);
		_position.Write(writer);
		_rotation.Write(writer);
		writer.WriteString(_pose);
	}
}
