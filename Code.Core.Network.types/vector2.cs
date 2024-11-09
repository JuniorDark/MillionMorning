namespace Code.Core.Network.types;

public class vector2
{
	private readonly float _x;

	private readonly float _y;

	public vector2(MessageReader reader)
	{
		_x = reader.ReadFloat();
		_y = reader.ReadFloat();
	}

	public vector2(float x, float y)
	{
		_x = x;
		_y = y;
	}

	public float GetX()
	{
		return _x;
	}

	public float GetY()
	{
		return _y;
	}

	public int Size()
	{
		return 8;
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteFloat(_x);
		writer.WriteFloat(_y);
	}
}
