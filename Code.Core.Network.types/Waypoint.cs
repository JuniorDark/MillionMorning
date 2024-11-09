namespace Code.Core.Network.types;

public class Waypoint
{
	private readonly vector3 _position;

	private readonly vector3 _rotation;

	private readonly float _time;

	public Waypoint(MessageReader reader)
	{
		_position = new vector3(reader);
		_rotation = new vector3(reader);
		_time = reader.ReadFloat();
	}

	public Waypoint(vector3 position, vector3 rotation, float time)
	{
		_position = position;
		_rotation = rotation;
		_time = time;
	}

	public vector3 GetPosition()
	{
		return _position;
	}

	public vector3 GetRotation()
	{
		return _rotation;
	}

	public float GetTime()
	{
		return _time;
	}

	public int Size()
	{
		return 28;
	}

	public void Write(MessageWriter writer)
	{
		_position.Write(writer);
		_rotation.Write(writer);
		writer.WriteFloat(_time);
	}
}
