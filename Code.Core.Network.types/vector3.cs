using UnityEngine;

namespace Code.Core.Network.types;

public class vector3
{
	private readonly float _x;

	private readonly float _y;

	private readonly float _z;

	public vector3(MessageReader reader)
	{
		_x = reader.ReadFloat();
		_y = reader.ReadFloat();
		_z = reader.ReadFloat();
	}

	public vector3(float x, float y, float z)
	{
		_x = x;
		_y = y;
		_z = z;
	}

	public float GetX()
	{
		return _x;
	}

	public float GetY()
	{
		return _y;
	}

	public float GetZ()
	{
		return _z;
	}

	public Vector3 GetAsVector()
	{
		return new Vector3(_x, _y, _z);
	}

	public int Size()
	{
		return 12;
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteFloat(_x);
		writer.WriteFloat(_y);
		writer.WriteFloat(_z);
	}
}
