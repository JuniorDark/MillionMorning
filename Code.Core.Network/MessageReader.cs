using System.IO;
using System.Text;

namespace Code.Core.Network;

public class MessageReader
{
	private BinaryWriter _bufferBw;

	private readonly BinaryReader _bufferBr;

	public MessageReader(byte[] buffer)
	{
		MemoryStream input = new MemoryStream(buffer);
		_bufferBr = new BinaryReader(input);
	}

	public sbyte ReadInt8()
	{
		return _bufferBr.ReadSByte();
	}

	public short ReadInt16()
	{
		return EndianConverter.NetworkToHostOrder(_bufferBr.ReadInt16());
	}

	public int ReadInt32()
	{
		return EndianConverter.NetworkToHostOrder(_bufferBr.ReadInt32());
	}

	public long ReadInt64()
	{
		return EndianConverter.NetworkToHostOrder(_bufferBr.ReadInt64());
	}

	public float ReadFloat()
	{
		return EndianConverter.NetworkToHostOrder(_bufferBr.ReadSingle());
	}

	public double ReadDouble()
	{
		return EndianConverter.NetworkToHostOrder(_bufferBr.ReadDouble());
	}

	public string ReadString()
	{
		short count = ReadInt16();
		return Encoding.UTF8.GetString(_bufferBr.ReadBytes(count));
	}

	public byte[] ReadBytes()
	{
		short count = ReadInt16();
		return _bufferBr.ReadBytes(count);
	}

	public int ReadOpCode()
	{
		return ReadInt16();
	}

	public int ReadTypeCode()
	{
		return ReadInt16();
	}
}
