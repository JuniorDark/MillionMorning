namespace Code.Core.Network.types;

public class PlayerPositionUpdate
{
	private readonly int _playerID;

	private readonly short _x;

	private readonly short _y;

	private readonly short _z;

	public PlayerPositionUpdate(MessageReader reader)
	{
		_playerID = reader.ReadInt32();
		_x = reader.ReadInt16();
		_y = reader.ReadInt16();
		_z = reader.ReadInt16();
	}

	public PlayerPositionUpdate(int playerID, short x, short y, short z)
	{
		_playerID = playerID;
		_x = x;
		_y = y;
		_z = z;
	}

	public int GetPlayerID()
	{
		return _playerID;
	}

	public short GetX()
	{
		return _x;
	}

	public short GetY()
	{
		return _y;
	}

	public short GetZ()
	{
		return _z;
	}

	public short Size()
	{
		return 10;
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteInt32(_playerID);
		writer.WriteInt16(_x);
		writer.WriteInt16(_y);
		writer.WriteInt16(_z);
	}
}
