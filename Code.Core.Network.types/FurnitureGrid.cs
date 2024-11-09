namespace Code.Core.Network.types;

public class FurnitureGrid
{
	private readonly Sequenced2dByteArray _bytes;

	private readonly vector2 _pivot;

	private readonly sbyte _isSquare;

	public FurnitureGrid(MessageReader reader)
	{
		_bytes = new Sequenced2dByteArray(reader);
		_pivot = new vector2(reader);
		_isSquare = reader.ReadInt8();
	}

	public FurnitureGrid(Sequenced2dByteArray bytes, vector2 pivot, sbyte isSquare)
	{
		_bytes = bytes;
		_pivot = pivot;
		_isSquare = isSquare;
	}

	public Sequenced2dByteArray GetBytes()
	{
		return _bytes;
	}

	public vector2 GetPivot()
	{
		return _pivot;
	}

	public sbyte GetIsSquare()
	{
		return _isSquare;
	}

	public int Size()
	{
		return 9 + _bytes.Size();
	}

	public void Write(MessageWriter writer)
	{
		_bytes.Write(writer);
		_pivot.Write(writer);
		writer.WriteInt8(_isSquare);
	}
}
