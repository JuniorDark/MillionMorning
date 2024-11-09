namespace Code.Core.Network.types;

public class Sequenced2dByteArray
{
	private readonly byte[] _array;

	private readonly int _columns;

	public Sequenced2dByteArray(MessageReader reader)
	{
		_array = reader.ReadBytes();
		_columns = reader.ReadInt32();
	}

	public Sequenced2dByteArray(byte[] array, int columns)
	{
		_array = array;
		_columns = columns;
	}

	public byte[] GetArray()
	{
		return _array;
	}

	public int GetColumns()
	{
		return _columns;
	}

	public int Size()
	{
		return 6 + (short)_array.Length;
	}

	public void Write(MessageWriter writer)
	{
		writer.WriteBytes(_array);
		writer.WriteInt32(_columns);
	}
}
